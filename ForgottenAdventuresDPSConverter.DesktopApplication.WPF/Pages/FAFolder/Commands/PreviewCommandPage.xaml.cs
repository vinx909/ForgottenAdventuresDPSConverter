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
    /// Interaction logic for PreviewCommandPage.xaml
    /// </summary>
    public partial class PreviewCommandPage : Page
    {
        private readonly FAFoldersViewModel viewModel;

        public PreviewCommandPage()
        {
            InitializeComponent();
        }

        public PreviewCommandPage(FAFoldersViewModel fAFoldersViewModel):this()
        {
            this.viewModel = fAFoldersViewModel;
            DataContext = viewModel;
        }

        private void CopyImagepath(object sender, MouseButtonEventArgs e)
        {
            const char imageSourseToStringNextFolderSign = '/';

            if (sender is Image image)
            {
                Clipboard.SetText(image.Source.ToString().Split(imageSourseToStringNextFolderSign).Last());
            }
        }
    }
}
