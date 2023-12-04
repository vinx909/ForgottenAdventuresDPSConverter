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

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.FAFolder
{
    /// <summary>
    /// Interaction logic for FAFolderDataPage.xaml
    /// </summary>
    public partial class FAFolderDataPage : Page
    {
        private FAFoldersViewModel viewModel;

        public FAFolderDataPage(FAFoldersViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        public void SaveFolder(object sender, RoutedEventArgs e)
        {
            viewModel.SaveFolder();
        }
    }
}
