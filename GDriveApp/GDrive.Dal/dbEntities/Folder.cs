using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDrive.Dal.dbEntities
{
    public class Folder : BaseEntity
    {
        public string Url { get; set; }

        public string Name { get; set; }

        public virtual ICollection<File> Files { get; set; }
    }
}
