using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Interfaces
{
    public interface ISettingsGetter
    {
        public string FADownloadFolderPath { get; }
    }
}
