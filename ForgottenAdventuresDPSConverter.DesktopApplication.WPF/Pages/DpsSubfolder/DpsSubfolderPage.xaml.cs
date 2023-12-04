using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities;
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
    /// Interaction logic for DpsSubfolderPage.xaml
    /// </summary>
    public partial class DpsSubfolderPage : Page
    {
        private readonly DpsSubfolderViewModel viewModel;

        public DpsSubfolderPage(IDpsSubfolderService subfolderService)
        {
            InitializeComponent();
            this.viewModel = new(subfolderService);
            DataContext = viewModel;

            viewModel.UpdateSubfolders();

            dataFrame.Content = new DpsSubfolderDataPage(viewModel);
        }

        private void SubfolderListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count>0)
            {
                Core.Entities.DpsSubfolder? selectedNumber = e.AddedItems[0] as Core.Entities.DpsSubfolder;
                if (selectedNumber != null)
                {
                    viewModel.SelectNewSubfolder(selectedNumber.Id);
                }
            }
        }
    }
}
