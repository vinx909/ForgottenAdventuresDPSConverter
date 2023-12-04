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

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.DPSFolder
{
    /// <summary>
    /// Interaction logic for DpsFolderDataPage.xaml
    /// </summary>
    public partial class DpsFolderDataPage : Page
    {
        private readonly DpsFolderViewModel viewModel;

        public DpsFolderDataPage(DpsFolderViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void SaveFolder(object sender, RoutedEventArgs e)
        {
            if (viewModel.WorkingOnNewFolder)
            {
                viewModel.AddFolder();
            }
            else
            {
                viewModel.UpdateFolder();
            }
            viewModel.UpdateFolders();
        }
    }
}
