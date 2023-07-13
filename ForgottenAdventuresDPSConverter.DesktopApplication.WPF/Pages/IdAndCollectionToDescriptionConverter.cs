using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages
{
    internal class IdAndCollectionToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? selectedItemId = (int?)value;
            List<DpsFolderEntityViewModel> items = parameter as List<DpsFolderEntityViewModel>;

            if(items != null)
            {
                DpsFolderEntityViewModel selectedItem = items.FirstOrDefault(i => i.Id == selectedItemId);
                return selectedItem?.Description;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
