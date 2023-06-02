using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Entities
{
    public class FAFolder : IdEntity, ICloneable
    {
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public int? ParentId { get; set; }
        public bool HasItems { get; set; }
        public int? NumberId { get; set; }
        public int? FolderId { get; set; }
        public int? SubfolderId { get; set; }
        public bool IsFloor { get; set; }
        public bool OtherCommands { get; set; }
        public string Commands { get; set; }
        public string Notes { get; set; }
        public FAFolder()
        {
            Name = string.Empty;
            RelativePath = string.Empty;
            Commands = string.Empty;
            Notes = string.Empty;
        }

        public object Clone()
        {
            FAFolder clone = new FAFolder()
            {
                Id = Id,
                Name = Name,
                RelativePath = RelativePath,
                ParentId = ParentId,
                HasItems = HasItems,
                NumberId = NumberId,
                FolderId = FolderId,
                SubfolderId = SubfolderId,
                IsFloor = IsFloor,
                OtherCommands = OtherCommands,
                Commands = Commands,
                Notes = Notes
            };
            return clone;
        }
    }
}
