using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Entities
{
    public class DpsFolder : IdEntity
    {
        private const int nameAbriviationMaxLength = 2;

        public static int NameAbriviationMaxLength { get => nameAbriviationMaxLength; }

        public string Name { get; set; }
        public string NameAbriviation { get; set; }
        public string Description { get; set; }

        public DpsFolder()
        {
            Name = string.Empty;
            NameAbriviation = string.Empty;
            Description = string.Empty;
        }
    }
}
