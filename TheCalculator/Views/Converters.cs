

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace TheCalculator.Views {

	//public class IndexToBackgroundConverter:IValueConverter {
		
	//	public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
	//		return Brushes.Red;
	//	}

	//	public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
	//		throw new NotImplementedException ();
	//	}
	//}

	public class LengthToVisibilityConverter:IValueConverter {
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
			return string.IsNullOrEmpty (value as string) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException ();
		}
	}
}
