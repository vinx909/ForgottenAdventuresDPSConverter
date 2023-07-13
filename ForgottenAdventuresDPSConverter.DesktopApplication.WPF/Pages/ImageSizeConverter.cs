using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages
{
    internal class ImageSizeConverter : IValueConverter
    {
        private const int maxSize = 200;
        private const int divider = 6;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double availableSize = (double)value;
            return Math.Min(availableSize / divider, maxSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * divider;
        }
    }
}
