using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Reports
{
    public class FAFolderCanExistReport : FinalizableReport
    {
        private bool idUnique;
        private bool nameNotNullOrWhiteSpace;
        private bool nameNotTooLong;
        private bool relativePathNotNullOrWhiteSpace;
        private bool relativePathNotTooLong;
        private bool relativePathUnique;
        private bool parentIdNull;
        private bool parentIdExists;
        private bool numberIdNull;
        private bool numberIdExists;
        private bool folderIdNull;
        private bool folderIdExists;
        private bool subfolderIdNull;
        private bool subFolderIdExists;
        private bool commandsNotNull;
        private bool commandsAreValid;
        private bool commandsNotTooLong;
        private bool notesNotNull;
        private bool notesNotTooLong;
        private bool parentIdIsNotId;

        public bool CanExist
        {
            get =>
                IdUnique &&
                NameNotNullOrWhiteSpace &&
                NameNotTooLong &&
                RelativePathNotNullOrWhiteSpace &&
                RelativePathNotTooLong &&
                RelativePathUnique &&
                (ParentIdNull ^ ParentIdExists) &&
                (NumberIdNull ^ NumberIdExists) &&
                (FolderIdNull ^ FolderIdExists) &&
                (SubfolderIdNull ^ SubFolderIdExists) &&
                CommandsAreValid &&
                CommandsNotTooLong &&
                NotesNotTooLong;
        }
        public bool IdUnique
        {
            get => idUnique;
            set { if (notFinalized) { idUnique = value; } }
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
        }//if this is the case then the repository needs updating
        public bool RelativePathNotNullOrWhiteSpace
        {
            get => relativePathNotNullOrWhiteSpace;
            set { if (notFinalized) { relativePathNotNullOrWhiteSpace = value; } }
        }
        public bool RelativePathNotTooLong
        {
            get => relativePathNotTooLong;
            set { if (notFinalized) { relativePathNotTooLong = value; } }
        }//if this is the case then the repository needs updating
        public bool RelativePathUnique
        {
            get => relativePathUnique;
            set { if (notFinalized) { relativePathUnique = value; } }
        }
        public bool ParentIdNull
        {
            get => parentIdNull;
            set { if (notFinalized) { parentIdNull = value; } }
        }
        public bool ParentIdIsNotId {
            get => parentIdIsNotId;
            set { if (notFinalized) { parentIdIsNotId = value; } }
        }
        public bool ParentIdExists
        {
            get => parentIdExists;
            set { if (notFinalized) { parentIdExists = value; } }
        }
        public bool NumberIdNull
        {
            get => numberIdNull;
            set { if (notFinalized) { numberIdNull = value; } }
        }
        public bool NumberIdExists
        {
            get => numberIdExists;
            set { if (notFinalized) { numberIdExists = value; } }
        }
        public bool FolderIdNull
        {
            get => folderIdNull;
            set { if (notFinalized) { folderIdNull = value; } }
        }
        public bool FolderIdExists
        {
            get => folderIdExists;
            set { if (notFinalized) { folderIdExists = value; } }
        }
        public bool SubfolderIdNull
        {
            get => subfolderIdNull;
            set { if (notFinalized) { subfolderIdNull = value; } }
        }
        public bool SubFolderIdExists
        {
            get => subFolderIdExists;
            set { if (notFinalized) { subFolderIdExists = value; } }
        }
        public bool CommandsNotNull
        {
            get => commandsNotNull;
            set { if (notFinalized) { commandsNotNull = value; } }
        }
        public bool CommandsAreValid
        {
            get => commandsAreValid;
            set { if (notFinalized) { commandsAreValid = value; } }
        }
        public bool CommandsNotTooLong
        {
            get => commandsNotTooLong;
            set { if (notFinalized) { commandsNotTooLong = value; } }
        }//if this is the case then the repository probably needs updating
        public bool NotesNotNull
        {
            get => notesNotNull;
            set { if (notFinalized) { notesNotNull = value; } }
        }
        public bool NotesNotTooLong
        {
            get => notesNotTooLong;
            set { if (notFinalized) { notesNotTooLong = value; } }
        }
    }
}
