using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Settings
{
    public class SettingsGetter : ISettingsGetter
    {
        public string FADownloadFolderPath => Properties.Settings.Default.FADownloadFolderPath;
    }
}
