using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DevCenterGallary.Converters
{
    public class BoolToVisibility:IValueConverter
    {
        public bool Invert { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (Invert ? !(bool)value : (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
