using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
using TheCalculator.Localization;
using TheCalculator.Models;
using TheCalculator.ViewModels;

namespace TheCalculator.Views {
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

		private string error;
		public string Error {
			get { return this.error; }
			set {
				this.error = value;
				this.OnPropertyChanged ();
			}
		}

		public int HistoryIndex { get; set; }

		public MainWindow () {
			this.History = new History (); 
			this.HistoryIndex = -1;

			InitializeComponent ();
			
			//this.History.Add (new HistoryItem ("awdggawd", 123));
			//this.History.Add (new HistoryItem ("awdagwd", 123));
			//this.History.Add (new HistoryItem ("aggjwdawd", 123));
			//this.ScrollViewer.Visibility = System.Windows.Visibility.Visible;
		}

		private void InputTextChanged (object sender, RoutedEventArgs e) {
			this.TxtPlaceholder.Visibility = this.TxtInput.Text.Length <= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
			this.TxtInput.Opacity = this.TxtInput.Text.Length <= 0 ? 0.5 : 1;
		}

		private void InputKeyUpped (object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				//if input is empty, remove error and do nothing
				if (string.IsNullOrWhiteSpace (this.TxtInput.Text)) {
					this.Error = "";
					return;
				}

				//calcumalate the result
				CalcumalateResult result = Calcumalator.Calcumalate (this.TxtInput.Text);

				//if no error
				if (result.Error == CalcumalateError.None) {
					//make the list visible for the first time
					if (this.History.Count <= 0) {
						this.ScrollViewer.Visibility = System.Windows.Visibility.Visible;
					}

					//add to the history
					this.History.Add (new HistoryItem (this.TxtInput.Text, result.Result));

					//remove error
					this.Error = "";
				} else {  //if error
					Dictionary <CalcumalateError, string> errors = new Dictionary <CalcumalateError, string> ();
					errors [CalcumalateError.MissingOpenBracket] = Strings.MissingOpenBracket;
					errors [CalcumalateError.MissingCloseBracket] = Strings.MissingCloseBracket;
					errors [CalcumalateError.UnknownOperator] = Strings.UnknownOperator;
					errors [CalcumalateError.SyntaxError] = Strings.SyntaxError;
					this.Error = errors [result.Error];
				}

				//get the input incase of error
				string input = this.TxtInput.Text;

				//reset the history index
				this.HistoryIndex = -1;

				//reset the gui highlight
				this.SelectHistoryItem ();

				//if there was error
				if (result.Error != CalcumalateError.None) {
					//set the textbox to be the original input and highlight it
					this.TxtInput.Text = input;
					this.TxtInput.SelectAll ();
				}
			} else if (e.Key == Key.Up) {
				//if index has not moved
				if (this.HistoryIndex <= -1) {
					//set index to bottom
					this.HistoryIndex = this.History.Count - 1;
				} else if (this.HistoryIndex > 0) {  //if index has moved and isn't at the top
					//move index up one
					this.HistoryIndex--;
				}
			} else if (e.Key == Key.Down) {
				//move index down only if the index has been moved and isn't at the bottom
				if (this.HistoryIndex > -1 && this.HistoryIndex < this.History.Count) {
					this.HistoryIndex++;

					//if index went off the end
					if (this.HistoryIndex >= this.History.Count) {
						//reset the index
						this.HistoryIndex = -1;
					}
				}
			} else if (Keyboard.IsKeyDown (Key.LeftCtrl) || Keyboard.IsKeyDown (Key.RightCtrl) && e.Key == Key.Q) {
				//quit on ctrl Q
				Application.Current.Shutdown ();
			}

			//update gui based on selected history item
			if (e.Key == Key.Up || e.Key == Key.Down) {
				this.SelectHistoryItem ();
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
