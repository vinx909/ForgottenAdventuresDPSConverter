using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Reports
{
    abstract public class FinalizableReport
    {
        protected bool notFinalized;

        public FinalizableReport()
        {
            notFinalized = true;
        }

        public void Finalize()
        {
            notFinalized = false;
        }
    }
}
