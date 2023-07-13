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
using static ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels.FAFoldersViewModel;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages
{
    /// <summary>
    /// Interaction logic for DpsNumberDataPage.xaml
    /// </summary>
    public partial class DpsNumberDataPage : Page
    {
        private Viewmodel.ViewModels.DpsNumbersViewModel viewModel;

        public DpsNumberDataPage(Viewmodel.ViewModels.DpsNumbersViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void SaveNumber(object sender, RoutedEventArgs e)
        {
            if (viewModel.WorkingOnNewNumber)
            {
                viewModel.AddNumber();
            }
            else
            {
                viewModel.UpdateNumber();
            }
            viewModel.UpdateNumbers();
        }
    }
}
