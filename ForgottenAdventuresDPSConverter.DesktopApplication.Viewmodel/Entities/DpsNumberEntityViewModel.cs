using ForgottenAdventuresDPSConverter.Core.Entities;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities
{
    public class DpsNumberEntityViewModel : DpsFolderEntityViewModel
    {
        private const string connector = " - ";

        public int Number { get; set; }

        public string DisplayName { get { return string.Empty + Number + connector + Name; } }

        public DpsNumberEntityViewModel()
        {
            Number = 0;
        }

        public DpsNumberEntityViewModel(DpsNumber baseNumber)
        {
            Id = baseNumber.Id;
            Number = baseNumber.Number;
            Name = baseNumber.Name;
            Description = baseNumber.Description;
        }

        public DpsNumber ToDpsNumber(bool idAllowedToBeNull = false)
        {
            if (Id == null && idAllowedToBeNull)
            {
                return new DpsNumber()
                {
                    Id = 0,
                    Number = Number,
                    Name = Name,
                    Description = Description,
                };
            }
            else if (Id == null)
            {
                return null;
            }
            else
            {
                return new DpsNumber()
                {
                    Id = (int)Id,
                    Number = Number,
                    Name = Name,
                    Description = Description,
                };
            }
        }
    }
}
