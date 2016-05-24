using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto;

namespace GDrive.Dal.dbEntities
{
    public class File : BaseEntity
    {
        public string Url { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        [NotMapped]
        public Enums.FileType Type { get; set; }

        public int FolderId { get; set; }

        public virtual Folder Folder { get; set; }
    }
}
