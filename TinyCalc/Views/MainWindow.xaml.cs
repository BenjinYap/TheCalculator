using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TinyCalc.Localization;
using TinyCalc.Models;
using TinyCalc.Models.Modules;
using TinyCalc.ViewModels;

namespace TinyCalc.Views {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow:Window, INotifyPropertyChanged {
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null) {
			PropertyChangedEventHandler handler = this.PropertyChanged;

			if (handler != null) {
				handler (this, new PropertyChangedEventArgs (propertyName));
			}
		}
		#endregion

		public History History { get; set; }
		public int HistoryIndex { get; set; }

		private string error;
		public string Error {
			get { return this.error; }
			set {
				this.error = value;
				this.OnPropertyChanged ();
			}
		}

		private List <AutocompleteItem> fullAutocompleteItems = new List <AutocompleteItem> ();

		public ObservableCollection <AutocompleteItem> AutocompleteItems { get; set; }
		public int AutocompleteItemIndex { get; set; }

		private Calc calc = new Calc ();

		private Regex autocompleteTokenRegex = new Regex ("[a-zA-Z]+$");

		public MainWindow () {
			this.History = new History (); 
			this.HistoryIndex = -1;

			this.AutocompleteItems = new ObservableCollection <AutocompleteItem> ();
			this.AutocompleteItemIndex = -1;
			
			List <string> tokens = new FunctionModule ().GetTokens ();

			foreach (string token in tokens) {
				this.fullAutocompleteItems.Add (new AutocompleteItem (AutoCompleteItemType.Constant, token, token));
			}

			InitializeComponent ();

			this.History.Add (new HistoryItem ("awdggawd", 123));
			this.History.Add (new HistoryItem ("awdagwd", 123));
			this.History.Add (new HistoryItem ("aggjwdawd", 123));
			this.ScrollViewer.Visibility = System.Windows.Visibility.Visible;
		}

		private void InputSelectionChanged (object sender, RoutedEventArgs e) {
			//reset autocomplete list
			this.AutocompleteItems.Clear ();
			this.AutoompleteList.SelectedIndex = -1;
			this.AutocompleteItemIndex = -1;
			
			if (this.TxtInput.Text.Length > 0) {
				//grab the input from the start until the caret position
				string source = this.TxtInput.Text.Substring (0, this.TxtInput.CaretIndex);

				//grab the last substring that resembles a token
				Match match = this.autocompleteTokenRegex.Match (source);
				
				if (match.Success) {
					string token = match.Value;

					//populate autocomplete list with items that contain the token
					for (int i = 0; i < this.fullAutocompleteItems.Count; i++) {
						AutocompleteItem item = this.fullAutocompleteItems [i];

						if (item.Name.Contains (token)) {
							this.AutocompleteItems.Add (item);
						}
					}

					//set selected item to the first item that matches the token from the start of the string
					for (int i = 0; i < this.AutocompleteItems.Count; i++) {
						if (this.AutocompleteItems [i].Name.IndexOf (token) == 0 && this.AutoompleteList.SelectedIndex == -1) {
							this.AutoompleteList.SelectedIndex = i;
						}
					}
				}
			}
		}

		private void InputTextChanged (object sender, RoutedEventArgs e) {
			//determine "Equation" placeholder visibility
			this.TxtPlaceholder.Visibility = this.TxtInput.Text.Length <= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
			this.TxtInput.Opacity = this.TxtInput.Text.Length <= 0 ? 0.5 : 1;

			
		}

		private void HandleEnter () {
			if (this.AutocompleteItems.Count > 0) {
				//grab the input from the start until the caret position
				string source = this.TxtInput.Text.Substring (0, this.TxtInput.CaretIndex);

				//grab the last substring that resembles a token
				Match match = this.autocompleteTokenRegex.Match (source);
				
				if (match.Success) {
					string token = match.Value;
					Regex regex = new Regex (token);
					AutocompleteItem item = (AutocompleteItem) this.AutoompleteList.SelectedItem;
					this.TxtInput.Text = regex.Replace (this.TxtInput.Text, item.Name, 1);
				}

				return;
			}

			//if input is empty, remove error and do nothing
			if (string.IsNullOrWhiteSpace (this.TxtInput.Text)) {
				this.Error = "";
				return;
			}

			//calcumalate the result
			CalcResult result = this.calc.Solve (this.TxtInput.Text);

			//if no error
			if (result.Error == CalcError.None) {
				//make the list visible for the first time
				if (this.History.Count <= 0) {
					this.ScrollViewer.Visibility = System.Windows.Visibility.Visible;
				}

				//add to the history
				this.History.Add (new HistoryItem (this.TxtInput.Text, result.Result));

				//remove error
				this.Error = "";
			} else {  //if error
				Dictionary <CalcError, string> errors = new Dictionary <CalcError, string> ();
				errors [CalcError.MissingLeftBracket] = Strings.MissingLeftBracket;
				errors [CalcError.MissingRightBracket] = Strings.MissingRightBracket;
				errors [CalcError.MissingFunctionBrackets] = Strings.MissingFunctionBracket;
				errors [CalcError.InfiniteLoop] = Strings.InfiniteLoop;
				errors [CalcError.UnknownToken] = Strings.UnknownToken;
				errors [CalcError.Unknown] = Strings.Unknown;
				this.Error = errors [result.Error];
			}

			//get the input incase of error
			string input = this.TxtInput.Text;

			//reset the history index
			this.HistoryIndex = -1;

			//reset the gui highlight
			this.SelectHistoryItem ();

			//if there was error
			if (result.Error != CalcError.None) {
				//set the textbox to be the original input and highlight it
				this.TxtInput.Text = input;
				this.TxtInput.SelectAll ();
			}
		}

		private void HandleUp () {
			//if there's something in autocomplete
			if (this.AutocompleteItems.Count > 0) {
				//move the autocomplete index
				if (this.AutoompleteList.SelectedIndex > 0) {
					this.AutoompleteList.SelectedIndex--;
				}
			} else {  //nothing in autocomplete
				//if index has not moved
				if (this.HistoryIndex <= -1) {
					//set index to bottom
					this.HistoryIndex = this.History.Count - 1;
				} else if (this.HistoryIndex > 0) {  //if index has moved and isn't at the top
					//move index up one
					this.HistoryIndex--;
				}

				this.SelectHistoryItem ();
			}
		}

		private void HandleDown () {
			//if there's something in autocomplete
			if (this.AutocompleteItems.Count > 0) {
				//move the autocomplete index
				if (this.AutoompleteList.SelectedIndex < this.AutocompleteItems.Count) {
					this.AutoompleteList.SelectedIndex++;
				}
			} else {  //nothing in autocomplete
				//move index down only if the index has been moved and isn't at the bottom
				if (this.HistoryIndex > -1 && this.HistoryIndex < this.History.Count) {
					this.HistoryIndex++;

					//if index went off the end
					if (this.HistoryIndex >= this.History.Count) {
						//reset the index
						this.HistoryIndex = -1;
					}

					this.SelectHistoryItem ();
				}
			}
		}

		private void InputKeyUpped (object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				this.HandleEnter ();
			} else if (e.Key == Key.Up) {
				this.HandleUp ();
			} else if (e.Key == Key.Down) {
				this.HandleDown ();
			} else if (Keyboard.IsKeyDown (Key.LeftCtrl) || Keyboard.IsKeyDown (Key.RightCtrl) && e.Key == Key.Q) {
				//quit on ctrl Q
				Application.Current.Shutdown ();
			}
		}

		private void SelectHistoryItem () {
			//if index isn't set, clear input
			if (this.HistoryIndex <= -1) {
				this.TxtInput.Text = "";
			} else {  //if index is set
				//set the input to the history item input
				this.TxtInput.Text = this.History [this.HistoryIndex].Input;
			}
			
			//force each item in the display list to update its background
			for (int i = 0; i < this.HistoryListBox.Items.Count; i++) {
				//get the item container
				ContentPresenter cp = this.HistoryListBox.ItemContainerGenerator.ContainerFromIndex (i) as ContentPresenter;

				//make sure container exists and has children
				if (cp != null && VisualTreeHelper.GetChildrenCount (cp) > 0) {
					//get the grid
					Grid grid = VisualTreeHelper.GetChild (cp, 0) as Grid;

					//make the grid's background binding refresh
					BindingOperations.GetMultiBindingExpression (grid, Grid.BackgroundProperty).UpdateTarget ();
				}
			}
			
			//highlight all text
			this.TxtInput.SelectAll ();
		}
	}
}
