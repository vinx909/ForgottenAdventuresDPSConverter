using ForgottenAdventuresDPSConverter.FileRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Settings
{
    public class FileRepositorySettingsFromSettings : FileRepositorySettings, IFileRepositorySettings
    {
        public override string DirectoryPath => Properties.Settings.Default.RepositoryDirectoryPath;

        public FileRepositorySettingsFromSettings(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
