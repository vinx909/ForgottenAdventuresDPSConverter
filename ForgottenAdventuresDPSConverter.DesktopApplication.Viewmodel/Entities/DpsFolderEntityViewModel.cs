using ForgottenAdventuresDPSConverter.Core.Entities;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities
{
    public class DpsFolderEntityViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DpsFolderEntityViewModel()
        {
            Id = null;
            Name = string.Empty;
            Description = string.Empty;
        }

        public DpsFolderEntityViewModel(DpsFolder folder)
        {
            Id = folder.Id;
            Name = folder.Name;
            Description = folder.Description;
        }

        public DpsFolderEntityViewModel(DpsSubfolder subfolder)
        {
            Id = subfolder.Id;
            Name = subfolder.Name;
            Description = subfolder.Description;
        }
    }
}
