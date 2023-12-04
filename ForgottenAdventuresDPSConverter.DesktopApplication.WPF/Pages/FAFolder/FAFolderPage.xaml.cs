using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Interfaces;
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
    /// Interaction logic for FAFolder.xaml
    /// </summary>
    public partial class FAFolderPage : Page
    {
        private readonly FAFoldersViewModel viewModel;

        public FAFolderPage(IFAFolderService fAFolderService, IDpsNumberService dpsNumberService, IDpsFolderService dpsFolderService, IDpsSubfolderService dpsSubfolderService, ISettingsGetter settings)
        {
            InitializeComponent();

            viewModel = new(fAFolderService, dpsNumberService, dpsFolderService, dpsSubfolderService, settings);
            DataContext = viewModel;

            dataFrame.Content = new FAFolderDataPage(viewModel);
            foreignKeyFrame.Content = new FAFolderForeignKeyPage(viewModel);
        }

        public void FolderSelectAll()
        {
            viewModel.GetAllFAFolders();
        }

        public void FolderSelectWithoutNumber()
        {
            viewModel.GetAllFAFoldersWithoutNumber();
        }

        public void FolderSelectWithoutFolder()
        {
            viewModel.GetAllFAFoldersWithoutFolder();
        }

        public void FolderSelectWithoutSubfolder()
        {
            viewModel.GetAllFAFoldersWithoutSubfolder();
        }

        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FATreeviewFolder? selectedFolder = e.NewValue as FATreeviewFolder;
            if(selectedFolder != null)
            {
                viewModel.SelectNewFolder(selectedFolder.Id);
            }
        }
    }
}
