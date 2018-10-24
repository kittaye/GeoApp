using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace GeoApp {
    public class ItemTappedEventArgsToItemTappedConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var eventArgs = value as ItemTappedEventArgs;
            var data = (Feature)eventArgs.Item;
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
