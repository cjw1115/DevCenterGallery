using DevCenterGallary.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DevCenterGallary.Converters
{
    public class PreinstallKitStatusConverter:IValueConverter
    {
        public PreinstallKitStatus Expected { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = (PreinstallKitStatus)value;
            return status == Expected ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
