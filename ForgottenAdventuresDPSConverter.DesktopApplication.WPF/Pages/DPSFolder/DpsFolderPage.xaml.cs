﻿using ForgottenAdventuresDPSConverter.Core.Interfaces;
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

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.DPSFolder
{
    /// <summary>
    /// Interaction logic for DpsFolderPage.xaml
    /// </summary>
    public partial class DpsFolderPage : Page
    {
        private readonly DpsFolderViewModel viewModel;

        public DpsFolderPage(IDpsFolderService foldersService)
        {
            InitializeComponent();
            this.viewModel = new(foldersService);
            DataContext = viewModel;

            viewModel.UpdateFolders();

            dataFrame.Content = new DpsFolderDataPage(viewModel);
        }

        private void FoldersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count>0)
            {
                Core.Entities.DpsFolder? selectedNumber = e.AddedItems[0] as Core.Entities.DpsFolder;
                if (selectedNumber != null)
                {
                    viewModel.SelectNewFolder(selectedNumber.Id);
                }
            }
        }
    }
}
