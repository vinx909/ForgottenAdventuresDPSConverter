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

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.FAFolder.Commands
{
    /// <summary>
    /// Interaction logic for CommandPage.xaml
    /// </summary>
    public partial class CommandPage : Page
    {
        private readonly CommandViewModel viewModel;

        public CommandPage()
        {
            InitializeComponent();
        }
        public CommandPage(CommandViewModel commandViewModel) : this()
        {
            viewModel = commandViewModel;
            DataContext = viewModel;

            SwitchToPreview(null, null);
        }

        private WallCommandPage wallCommandPage; 
        public WallCommandPage WallCommandPage
        {
            get
            {
                if (wallCommandPage == null)
                {
                    wallCommandPage = new WallCommandPage(viewModel.WallCommandsViewModel);
                }
                return wallCommandPage;
            }
        }

        private PreviewCommandPage previewCommandPage;
        public PreviewCommandPage PreviewCommandPage
        {
            get
            {
                if (previewCommandPage == null)
                {
                    previewCommandPage = new PreviewCommandPage(viewModel.FAFoldersViewModel);
                }
                return previewCommandPage;
            }
        }

        private void SwitchToSeperate(object sender, RoutedEventArgs e)
        {
            //specificCommandFrame.Content = 
            //TODO
        }

        private void SwitchToWall(object sender, RoutedEventArgs e)
        {
            specificCommandFrame.Content = WallCommandPage;
        }

        private void SwitchToPreview(object sender, RoutedEventArgs e)
        {
            specificCommandFrame.Content = PreviewCommandPage;
        }
    }
}
