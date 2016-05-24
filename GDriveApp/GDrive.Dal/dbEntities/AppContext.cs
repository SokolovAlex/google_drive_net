using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDrive.Dal.dbEntities
{

    public class AppContext : DbContext
    {
        public AppContext()
            : base("AppContext")
        {}

        public DbSet<File> Files { get; set; }

        public DbSet<Folder> Folders { get; set; }
    }
}
