using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages;
using ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.DPSFolder;
using ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.DpsNumber;
using ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.DpsSubfolder;
using ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.FAFolder;
using ForgottenAdventuresDPSConverter.FileRepository;
using Microsoft.Extensions.DependencyInjection;
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

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider serviceProvider;

        public MainWindow()
        {
            serviceProvider = ServiceProviderProvider.SetupServiceProvider();

            InitializeComponent();
        }

        #region menu items functions
        #region Forgotten Adventures menu items functions
        public void FAFolderAll_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToFAFolderPage();
            FAFolderPage.FolderSelectAll();
        }

        public void FAFolderWithoutNumber_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToFAFolderPage();
            FAFolderPage.FolderSelectWithoutNumber();
        }

        public void FAFolderWithoutFolder_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToFAFolderPage();
            FAFolderPage.FolderSelectWithoutFolder();
        }

        public void FAFolderWithoutSubfolder_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToFAFolderPage();
            FAFolderPage.FolderSelectWithoutSubfolder();
        }

        public void FAFolderSearch_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToFAFolderPage();
            //todo: set main frame to a frame with the ability to search trough FAFolders. probably a new page with search options at the top and that has it's own frame with FAFolder with search results.
        }
        #endregion
        public void DpsNumberAll_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToDpsNumberPage();
        }

        public void DpsFolderAll_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToDpsFolderPage();
        }

        public void DpsSubfolderAll_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToDpsSubfolderPage();
        }

        public void Settings_Click(object sender, RoutedEventArgs e)
        {
            SetMainFrameToSettinsPage();
        }
        #endregion

        #region change page functionality
        private FAFolderPage fAFolderPage = null;
        private DpsNumberPage dpsNumberPage = null;
        private DpsFolderPage dpsFolderPage = null;
        private DpsSubfolderPage dpsSubfolderPage = null;
        private SettingsPage settingsPage = null;
        private FAFolderPage FAFolderPage {
            get
            {
                if(fAFolderPage == null)
                {
                    IFAFolderService? fAFolderService = serviceProvider.GetService<IFAFolderService>();
                    IDpsNumberService? dpsNumberService = serviceProvider.GetService<IDpsNumberService>();
                    IDpsFolderService? dpsFolderService = serviceProvider.GetService<IDpsFolderService>();
                    IDpsSubfolderService? dpsSubfolderService = serviceProvider.GetService<IDpsSubfolderService>();
                    ICommandsService commandsService = serviceProvider.GetService<ICommandsService>();
                    ISettingsGetter? settings = serviceProvider.GetService<ISettingsGetter>();
                    fAFolderPage = new FAFolderPage(fAFolderService, dpsNumberService, dpsFolderService, dpsSubfolderService, commandsService, settings);
                }
                return fAFolderPage;
            } 
        }
        private DpsNumberPage DpsNumerPage
        {
            get
            {
                if(dpsNumberPage == null)
                {
                    IDpsNumberService? dpsNumberService = serviceProvider.GetService<IDpsNumberService>();
                    dpsNumberPage = new(dpsNumberService);
                }
                return dpsNumberPage;
            }
        }
        private DpsFolderPage DpsFolderPage
        {
            get
            {
                if (dpsFolderPage == null)
                {
                    IDpsFolderService? dpsFolderService = serviceProvider.GetService<IDpsFolderService>();
                    dpsFolderPage = new(dpsFolderService);
                }
                return dpsFolderPage;
            }
        }
        private DpsSubfolderPage DpsSubfolderPage
        {
            get
            {
                if (dpsSubfolderPage == null)
                {
                    IDpsSubfolderService? dpsSubfolderService = serviceProvider.GetService<IDpsSubfolderService>();
                    dpsSubfolderPage = new(dpsSubfolderService);
                }
                return dpsSubfolderPage;
            }
        }
        private SettingsPage SettingsPage
        {
            get
            {
                if(settingsPage == null)
                {
                    settingsPage = new(serviceProvider.GetService<IFileRepositorySettings>(), serviceProvider.GetService<IFAFolderService>(), serviceProvider.GetService<IConverterService>());
                }
                return settingsPage;
            }
        }

        private void SetMainFrameToFAFolderPage()
        {
            if (mainFrame.Content == null || typeof(FAFolderPage) != mainFrame.Content)
            {
                mainFrame.Content = FAFolderPage;
            }
        }

        private void SetMainFrameToDpsNumberPage()
        {
            if(mainFrame.Content == null || typeof(DpsNumberPage) != mainFrame.Content)
            {
                mainFrame.Content = DpsNumerPage;
            }
        }

        private void SetMainFrameToDpsFolderPage()
        {
            if (mainFrame.Content == null || typeof(DpsFolderPage) != mainFrame.Content)
            {
                mainFrame.Content = DpsFolderPage;
            }
        }

        private void SetMainFrameToDpsSubfolderPage()
        {
            if (mainFrame.Content == null || typeof(DpsSubfolderPage) != mainFrame.Content)
            {
                mainFrame.Content = DpsSubfolderPage;
            }
        }

        private void SetMainFrameToSettinsPage()
        {
            if (mainFrame.Content == null || typeof(SettingsPage) != mainFrame.Content)
            {
                mainFrame.Content = SettingsPage;
            }
        }
        #endregion
    }
}
