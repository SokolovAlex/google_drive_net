using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Models
{
    public class FileModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public string IconLink { get; set; }
        public string SharingUser { get; set; }

        public string FileExtension { get; set; }
        public bool? Shared { get; set; }

        public string Thumbnail { get; set; }
        public string Description { get; set; }
    }
}
