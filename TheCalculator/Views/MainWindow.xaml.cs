using System;
using System.Collections.Generic;
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
					if (this.History.Count <= 0) {
						this.ScrollViewer.Visibility = System.Windows.Visibility.Visible;
					}

					this.History.Add (new HistoryItem (this.TxtInput.Text, result.Result));
					this.TxtInput.Text = "";
				} else {

				}
			}
		}
	}
}
