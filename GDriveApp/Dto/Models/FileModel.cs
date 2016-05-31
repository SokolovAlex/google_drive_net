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

        public string IconLink { get; set; }

        public string FileExtension { get; set; }

        public string Thumbnail { get; set; }
        public string Description { get; set; }

        public string WebContentLink { get; set; }
        public string MimeType { get; set; }

        public string WebViewLink { get; set; }

        public IEnumerable<string> Parents { get; set; }
    }
}
