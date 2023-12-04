using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities;
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
using static ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels.FAFoldersViewModel;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.DpsNumber
{
    /// <summary>
    /// Interaction logic for DpsNumberPage.xaml
    /// </summary>
    public partial class DpsNumberPage : Page
    {
        private readonly Viewmodel.ViewModels.DpsNumbersViewModel viewModel;

        public DpsNumberPage(IDpsNumberService numberService)
        {
            InitializeComponent();
            this.viewModel = new(numberService);
            DataContext = viewModel;

            viewModel.UpdateNumbers();

            dataFrame.Content = new DpsNumberDataPage(viewModel);
        }

        private void NumbersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DpsNumberEntityViewModel? selectedNumber = e.AddedItems[0] as DpsNumberEntityViewModel;
            if (selectedNumber != null)
            {
                viewModel.SelectNewNumber(selectedNumber.Id);
            }
        }
    }
}
