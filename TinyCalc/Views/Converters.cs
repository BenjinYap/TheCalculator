

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace TinyCalc.Views {

	public class IndexToBackgroundConverter:IMultiValueConverter {
		public object Convert (object [] values, Type targetType, object parameter, CultureInfo culture) {
			return values [0].Equals (values [1]) ? Brushes.LightBlue : Brushes.Transparent;
		}

		public object [] ConvertBack (object value, Type [] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException ();
		}
	}

	public class LengthToVisibilityConverter:IValueConverter {
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
			return string.IsNullOrEmpty (value as string) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException ();
		}
	}
}
