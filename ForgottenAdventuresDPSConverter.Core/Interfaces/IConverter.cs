using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IConverterService
    {
        public Task ConvertToDpsFolders(string SourceDirectory, string TargetDirectory);

        public string ConvertImageName(string Name);
    }
}
