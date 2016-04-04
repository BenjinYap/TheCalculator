using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

		public AutocompleteList AutocompleteList { get; set; }
		
		private Calc calc = new Calc ();

		private int previousInputLength = 0;

		public MainWindow () {
			//set the window title
			Assembly ass = Assembly.GetExecutingAssembly ();
			this.Title = "TinyCalc " + FileVersionInfo.GetVersionInfo (ass.Location).FileVersion;

			this.History = new History (); 
			this.HistoryIndex = -1;

			this.AutocompleteList = new AutocompleteList ();
			
			InitializeComponent ();
		}

		private void InputSelectionChanged (object sender, RoutedEventArgs e) {
			//perform autocomplete only if 1 new character was entered or input was shortened by deleting
			if (this.TxtInput.Text.Length - this.previousInputLength == 1 || this.TxtInput.Text.Length < this.previousInputLength) {
				this.AutocompleteList.Populate (this.GetAutocompleteCandidate ());
			} else {  //clear autocomplete in all other cases
				this.AutocompleteList.Reset ();
			}
			
			this.previousInputLength = this.TxtInput.Text.Length;
		}

		private void InputTextChanged (object sender, RoutedEventArgs e) {
			//determine "Equation" placeholder visibility
			this.TxtPlaceholder.Visibility = this.TxtInput.Text.Length <= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
			this.TxtInput.Opacity = this.TxtInput.Text.Length <= 0 ? 0.5 : 1;
		}

		private string GetAutocompleteCandidate () {
			if (string.IsNullOrWhiteSpace (this.TxtInput.Text)) {
				return "";
			}

			//grab the input from the start until the caret position
			string source = this.TxtInput.Text.Substring (0, this.TxtInput.CaretIndex);

			//grab the last substring that resembles a token
			Match match = Regex.Match  (source, "[a-zA-Z]+$");
				
			if (match.Success) {
				return match.Value;
			}

			return "";
		}

		private void HandleEnter () {
			if (this.AutocompleteList.IsPopulated) {
				string token = this.GetAutocompleteCandidate ();
				Regex regex = new Regex (token);

				//calculate the new caret index after replacing
				int newCaretIndex = this.TxtInput.Text.IndexOf (token) + this.AutocompleteList.SelectedItemName.Length;

				//perform the replacement
				this.TxtInput.Text = regex.Replace (this.TxtInput.Text, this.AutocompleteList.SelectedItemName, 1);

				//set the caret index to be after the inserted token
				this.TxtInput.SelectionStart = newCaretIndex;
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
			if (this.AutocompleteList.IsPopulated) {
				//move the autocomplete index
				if (this.AutocompleteList.SelectedIndex > 0) {
					this.AutocompleteList.SelectedIndex--;
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
			if (this.AutocompleteList.IsPopulated) {
				//move the autocomplete index
				if (this.AutocompleteList.SelectedIndex < this.AutocompleteList.Count - 1) {
					this.AutocompleteList.SelectedIndex++;
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

		private void InputPreviewKeyDowned (object sender, KeyEventArgs e) {
			if (e.Key == Key.Up) {
				e.Handled = true;
				this.HandleUp ();
			} else if (e.Key == Key.Down) {
				e.Handled = true;
				this.HandleDown ();
			}
		}

		private void InputKeyDowned (object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				//solve the equation
				this.HandleEnter ();
			} else if (e.Key == Key.Escape) {
				//cancel the autocomplete
				this.AutocompleteList.Reset ();
			} else if (e.Key == Key.Space && this.IsCtrlDown ()) {
				//activate autocomplete at cursor
				e.Handled = true;
				this.AutocompleteList.Populate (this.GetAutocompleteCandidate ());

				//if nothing was found, show the master list
				if (this.AutocompleteList.IsPopulated == false) {
					this.AutocompleteList.PopulateAll ();
				}
			} else if (e.Key == Key.Q && this.IsCtrlDown ()) {
				//quit on ctrl Q
				Application.Current.Shutdown ();
			}
		}

		private bool IsCtrlDown () {
			return Keyboard.IsKeyDown (Key.LeftCtrl) || Keyboard.IsKeyDown (Key.RightCtrl);
		}

		private void SelectHistoryItem () {
			//if index isn't set, clear input
			if (this.HistoryIndex <= -1) {
				this.TxtInput.Text = "";
			} else {  //if index is set
				//set the input to the history item input
				this.TxtInput.Text = this.History [this.HistoryIndex].Input;

				//highlight all text
				this.TxtInput.SelectAll ();
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
		}

		private void WindowLocationChanged (object sender, EventArgs e) {
			this.AutocompletePopup.HorizontalOffset++;
			this.AutocompletePopup.HorizontalOffset--;
		}
	}
}
