using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Entities
{
    public class DpsSubfolder : IdEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public DpsSubfolder()
        {
            Name = string.Empty;
            Description = string.Empty;
        }
    }
}
