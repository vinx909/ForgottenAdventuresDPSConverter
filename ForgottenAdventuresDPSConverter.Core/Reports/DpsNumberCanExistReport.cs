using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Reports
{
    public class DpsNumberCanExistReport : FinalizableReport
    {
        private bool idAvailable;
        private bool nameNotNullOrWhiteSpace;
        private bool nameNotTooLong;
        private bool nameUnique;
        private bool numberUnique;
        private bool descriptionNotNull;
        private bool descriptionNotTooLong;

        public bool CanExist
        {
            get =>
                IdAvailable &&
                NumberUnique &&
                NameNotNullOrWhiteSpace &&
                NameNotTooLong &&
                NameUnique &&
                DescriptionNotNull &&
                DescriptionNotTooLong;
        }
        public bool IdAvailable
        {
            get => idAvailable;
            set { if (notFinalized) { idAvailable = value; } }
        }
        public bool NumberUnique
        {
            get => numberUnique;
            set { if (notFinalized) { numberUnique = value; } }
        }
        public bool NameNotNullOrWhiteSpace
        {
            get => nameNotNullOrWhiteSpace;
            set { if (notFinalized) { nameNotNullOrWhiteSpace = value; } }
        }
        public bool NameNotTooLong
        {
            get => nameNotTooLong;
            set { if (notFinalized) { nameNotTooLong = value; } }
        }
        public bool NameUnique
        {
            get => nameUnique;
            set { if (notFinalized) { nameUnique = value; } }
        }
        public bool DescriptionNotNull
        {
            get => descriptionNotNull;
            set { if (notFinalized) { descriptionNotNull = value; } }
        }
        public bool DescriptionNotTooLong
        {
            get => descriptionNotTooLong;
            set { if (notFinalized) { descriptionNotTooLong = value; } }
        }
    }
}
