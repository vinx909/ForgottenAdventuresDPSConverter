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
    /// Interaction logic for FAFolderForeignKeyPage.xaml
    /// </summary>
    public partial class FAFolderForeignKeyPage : Page
    {
        private FAFoldersViewModel viewModel;

        public FAFolderForeignKeyPage()
        {
            InitializeComponent();
        }

        public FAFolderForeignKeyPage(FAFoldersViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;

        }

        private void FolderSearchBarLostFocus(object sender, RoutedEventArgs e)
        {
            viewModel.UpdateFolders();
        }

        private void SubfolderSearchBarLostFocus(object sender, RoutedEventArgs e)
        {
            viewModel.UpdateSubfolders();
        }
    }
}
