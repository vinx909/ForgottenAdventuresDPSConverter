using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities
{
    public class FATreeviewFolder
    {
        private const char blockOpening = '[';
        private const char blockClosing = ']';
        private const char space = ' ';
        private const char numberChar = 'N';
        private const char folderChar = 'F';
        private const char subfolderChar = 'S';
        private const string itemString = "[I]";

        internal bool HasNumber { get; set; }
        internal bool HasFolder { get; set; }
        internal bool HasSubfolder { get; set; }
        internal bool HasItem { get; set; }

        internal bool HasChildWithoutNumber
        {
            get
            {
                foreach (FATreeviewFolder child in Children)
                {
                    if((child.HasItem && !child.HasNumber) || child.HasChildWithoutNumber)
                    {
                        return true;
                    }
                }
                return false;
            }}
        internal bool HasChildWithoutFolder
        {
            get
            {
                foreach (FATreeviewFolder child in Children)
                {
                    if ((child.HasItem && !child.HasFolder) || child.HasChildWithoutFolder)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        internal bool HasChildWithoutSubfolder
        {
            get
            {
                foreach (FATreeviewFolder child in Children)
                {
                    if ((child.HasItem && !child.HasSubfolder) || child.HasChildWithoutSubfolder)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string DisplayName
        {
            get
            {
                string returnString = string.Empty;
                if (HasItem && (!HasNumber || !HasFolder || !HasSubfolder))
                {
                    returnString += blockOpening;
                }
                if (HasItem && !HasNumber)
                {
                    returnString += numberChar;
                }
                if (HasItem && !HasFolder)
                {
                    returnString += folderChar;
                }
                if (HasItem && !HasSubfolder)
                {
                    returnString += subfolderChar;
                }
                if (HasItem && (!HasNumber || !HasFolder || !HasSubfolder))
                {
                    returnString += string.Empty + blockClosing + space;
                }
                if (HasItem)
                {
                    returnString += string.Empty + itemString + space;
                }
                returnString += Name;
                if (HasChildWithoutNumber || HasChildWithoutFolder || HasChildWithoutSubfolder)
                {
                    returnString += string.Empty + space + blockOpening;
                }
                if (HasChildWithoutNumber)
                {
                    returnString += numberChar;
                }
                if (HasChildWithoutFolder)
                {
                    returnString += folderChar;
                }
                if (HasChildWithoutSubfolder)
                {
                    returnString += subfolderChar;
                }
                if (HasChildWithoutNumber || HasChildWithoutFolder || HasChildWithoutSubfolder)
                {
                    returnString += blockClosing;
                }
                return returnString;
            }
        }

        public ObservableCollection<FATreeviewFolder> Children { get; set; }

        public FATreeviewFolder()
        {
            Name = string.Empty;
            Children = new();
        }

        public FATreeviewFolder(int id, string name, bool hasNumber, bool hasFolder, bool hasSubfolder, bool hasItem)
        {
            Id = id;
            Name = name;

            this.HasNumber = hasNumber;
            this.HasFolder = hasFolder;
            this.HasSubfolder = hasSubfolder;
            this.HasItem = hasItem;

            Children = new();
        }

        public FATreeviewFolder SetHas(bool hasNumber, bool hasFolder, bool hasSubfolder, bool hasItem)
        {
            this.HasNumber = hasNumber;
            this.HasFolder = hasFolder;
            this.HasSubfolder = hasSubfolder;
            this.HasItem = hasItem;

            return this;
        }
    }
}
