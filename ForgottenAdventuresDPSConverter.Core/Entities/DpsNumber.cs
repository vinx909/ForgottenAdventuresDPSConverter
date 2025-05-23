﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Entities
{
    public class DpsNumber : IdEntity
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DpsNumber()
        {
            Name = string.Empty;
            Description = string.Empty;
        }
    }
}
