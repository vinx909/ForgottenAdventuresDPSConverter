using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels
{
    public class CommandViewModel : BaseViewModel
    {
        private readonly FAFoldersViewModel fAFoldersViewModel;
        private readonly WallCommandsViewModel wallCommandsViewModel;

        public WallCommandsViewModel WallCommandsViewModel { get => wallCommandsViewModel; }

        public FAFoldersViewModel FAFoldersViewModel { get => fAFoldersViewModel; }


        public CommandViewModel()
        {
            //useless but needed to be used as a datamodel
        }

        public CommandViewModel(FAFoldersViewModel fAFoldersViewModel, ICommandsService commandsService, ISettingsGetter settings) : this()
        {
            this.fAFoldersViewModel = fAFoldersViewModel;
            this.fAFoldersViewModel.PropertyChanged += CommandsChangedEventHandler;

            wallCommandsViewModel = new(this, commandsService, settings);
        }

        private void CommandsChangedEventHandler(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(fAFoldersViewModel.SelectedFolderCommands))
            {

            }
        }
    }
}
