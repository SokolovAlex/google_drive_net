using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dto.Models;

namespace GDriveApp.Models
{
    public class LevelView
    {
        public int Depth { set; get; }

        public IEnumerable<FileModel> Files { set; get; }

        public IEnumerable<FolderModel> Folders { set; get; }
    }
}