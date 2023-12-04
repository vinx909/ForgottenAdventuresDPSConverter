using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels;
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

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.DpsSubfolder
{
    /// <summary>
    /// Interaction logic for DpsSubfolderDataPage.xaml
    /// </summary>
    public partial class DpsSubfolderDataPage : Page
    {
        private readonly DpsSubfolderViewModel viewModel;

        public DpsSubfolderDataPage(DpsSubfolderViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void SaveSubfolder(object sender, RoutedEventArgs e)
        {
            if (viewModel.WorkingOnNewSubfolder)
            {
                viewModel.AddSubfolder();
            }
            else
            {
                viewModel.UpdateSubfolder();
            }
            viewModel.UpdateSubfolders();
        }
    }
}
