using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface INameConverter
    {
        /// <summary>
        /// converts all the names of the output folder to how they are prefered.
        /// generally no numbers at the start, replacing "_" with " ".
        /// </summary>
        /// <returns>a NameConverterReport with all relavent information</returns>
        public NameConverterReport ConvertNames();
    }
}
