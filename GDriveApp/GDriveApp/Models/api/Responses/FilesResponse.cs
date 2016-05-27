using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dto.Models;

namespace GDriveApp.Models.api.Responses
{
    public class FilesResponse
    {
        public IEnumerable<FileModel> files { get; set; }
        public IEnumerable<FileModel> folders { get; set; }
    }
}