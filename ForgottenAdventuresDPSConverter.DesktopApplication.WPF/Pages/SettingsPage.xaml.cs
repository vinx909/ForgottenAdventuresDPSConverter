using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.Core.Reports;
using ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Settings;
using ForgottenAdventuresDPSConverter.FileRepository;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private readonly IConverterService converterService;
        private readonly IFAFolderService FAFolderService;
        private readonly IFileRepositorySettings fileRepositorySettings;

        public SettingsPage(IFileRepositorySettings fileRepositorySettings, IFAFolderService fAFolderService, IConverterService converterService)
        {
            this.fileRepositorySettings = fileRepositorySettings;

            InitializeComponent();

            filelocation.Text = Properties.Settings.Default.RepositoryDirectoryPath;
            FAFolderService = fAFolderService;
            this.converterService = converterService;
        }

        private void UpdateForgottenAdventuresFolders(object sender, RoutedEventArgs e)
        {
            if ((bool)updateFiles.IsChecked && Directory.Exists(updateFilesPath.Text))
            {
                UpdateWindow updateWindow = new(FAFolderService);
                
                updateWindow.Show();

                Task<FAFolderUpdateReport> reportTask = FAFolderService.UpdateFolders(updateFilesPath.Text, updateWindow, updateWindow);

                SettingsChanger.ChangeDownloadFolderPath(updateFilesPath.Text);

                //updateWindow.UpdateUpdatedWindow(reportTask.Result);
            }
        }

        private void FindFilesInNewLocation(object sender, RoutedEventArgs e)
        {
            if ((bool)enableChangeDirectory.IsChecked)
            {
                if (SettingsChanger.ChangeRepositoryDirectory(fileRepositorySettings, filelocation.Text))
                {
                    MessageBox.Show("file location changed. the program must now be closed");
                    Window? window = Window.GetWindow(this);
                    window.Close();
                }
                else
                {
                    MessageBox.Show("unable to find files in the new location, no change has been made");
                } 
            }
        }

        private void MoveFilesToNewLocation(object sender, RoutedEventArgs e)
        {
            if ((bool)enableChangeDirectory.IsChecked)
            {
                if (SettingsChanger.ChangeRepositoryDirectoryAndMoveFiles(fileRepositorySettings, filelocation.Text))
                {
                    MessageBox.Show("file location changed. the program must now be closed");
                    Window? window = Window.GetWindow(this);
                    window.Close();
                }
                else
                {
                    MessageBox.Show("unable to move files, no change has been made");
                } 
            }
        }

        private void ConvertFiles(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(convertTargetPath.Text))
            {
                converterService.ConvertToDpsFolders(Properties.Settings.Default.FADownloadFolderPath, convertTargetPath.Text);
            }
        }
    }
}
