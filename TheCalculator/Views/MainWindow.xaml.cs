using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using TheCalculator.Models;
using TheCalculator.ViewModels;

namespace TheCalculator.Views {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow:Window {
		public History History { get; set; }

		private int historyIndex = -1;

		public MainWindow () {
			this.History = new History (); 

			InitializeComponent ();

			//this.History.Add (new HistoryItem ("awdawd", 123));
			//this.History.Add (new HistoryItem ("awdawd", 123));
			//this.History.Add (new HistoryItem ("awdawd", 123));
		}

		private void InputTextChanged (object sender, RoutedEventArgs e) {
			
		}

		private void InputKeyUpped (object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				CalcumalateResult result = Calcumalator.Calcumalate (this.TxtInput.Text);

				if (result.Error == CalcumalateError.None) {
					//reset the history index
					this.historyIndex = -1;

					//make the list visible for the first time
					if (this.History.Count <= 0) {
						this.ScrollViewer.Visibility = System.Windows.Visibility.Visible;
					}

					//add to the history
					this.History.Add (new HistoryItem (this.TxtInput.Text, result.Result));

					//reset the textbox
					this.TxtInput.Text = "";
				} else {

				}
			} else if (e.Key == Key.Up) {
				//if index has not moved
				if (this.historyIndex <= -1) {
					//set index to bottom
					this.historyIndex = this.History.Count - 1;
				} else if (this.historyIndex > 0) {  //if index has moved and isn't at the top
					//move index up one
					this.historyIndex--;
				}
			} else if (e.Key == Key.Down) {
				//move index down only if the index has been moved and isn't at the bottom
				if (this.historyIndex > -1 && this.historyIndex < this.History.Count - 1) {
					this.historyIndex++;
				}
			}
		}
	}
}
