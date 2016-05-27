using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GDriveApp.Models.api.Requests
{
    public class CreateFolderRequest
    {
        public string folderName { get; set; }

        public string folderDesc { get; set; }

        public string parentId { get; set; }
    }
}