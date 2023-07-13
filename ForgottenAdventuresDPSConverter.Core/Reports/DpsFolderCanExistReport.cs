using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Reports
{
    public class DpsFolderCanExistReport : FinalizableReport
    {
        private bool idUnique;
        private bool nameNotNullOrEmpty;
        private bool nameNotTooLong;
        private bool nameUnique;
        private bool nameAbriviationNotNullOrEmpty;
        private bool nameAbriviationNotTooLong;
        private bool nameAbriviationUnique;
        private bool descriptionNotNull;
        private bool descriptionNotTooLong;

        public bool CanExist
        {
            get =>
                IdUnique &&
                NameNotNullOrWhiteSpace &&
                NameNotTooLong &&
                NameUnique &&
                NameAbriviationNotNullOrWhiteSpace &&
                NameAbriviationNotTooLong &&
                NameAbriviationUnique&&
                DescriptionNotNull &&
                DescriptionNotTooLong;
        }
        public bool IdUnique
        {
            get => idUnique;
            set { if (notFinalized) { idUnique = value; } }
        }
        public bool NameNotNullOrWhiteSpace
        {
            get => nameNotNullOrEmpty;
            set { if (notFinalized) { nameNotNullOrEmpty = value; } }
        }
        public bool NameNotTooLong
        {
            get => nameNotTooLong;
            set { if (notFinalized) { nameNotTooLong = value; } }
        }
        public bool NameUnique {
            get => nameUnique;
            set { if (notFinalized) { nameUnique = value; } }
        }
        public bool NameAbriviationNotNullOrWhiteSpace
        {
            get => nameAbriviationNotNullOrEmpty;
            set { if (notFinalized) { nameAbriviationNotNullOrEmpty = value; } }
        }
        public bool NameAbriviationNotTooLong
        {
            get => nameAbriviationNotTooLong;
            set { if (notFinalized) { nameAbriviationNotTooLong = value; } }
        }
        public bool NameAbriviationUnique {
            get => nameAbriviationUnique;
            set { if (notFinalized) { nameAbriviationUnique = value; } }
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
