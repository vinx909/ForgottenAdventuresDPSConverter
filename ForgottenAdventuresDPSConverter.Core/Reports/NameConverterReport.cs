using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Reports
{
    public class NameConverterReport
    {
        private bool successful;
        private bool finalized;

        public bool Successful { get => successful;
            set { if (!finalized) { successful = value; } }
        }
    }
}
