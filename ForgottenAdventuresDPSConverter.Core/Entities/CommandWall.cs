using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Entities
{
    public class CommandWall
    {
        public string ImageName { get; set; }
        public int TopBottomTrim { get; set; }
        public int LeftTrim { get; set; }
        public int RightTrim { get; set; }

        public CommandWall()
        {
            ImageName = string.Empty;
        }
    }
}
