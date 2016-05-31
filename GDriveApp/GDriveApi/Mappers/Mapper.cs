using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto.Models;
using Google.Apis.Drive.v3.Data;

namespace GDriveApi.Mappers
{
    public static class Mapper
    {
        public static FileModel ToFileModel(File file)
        {
            return new FileModel
            {
                Name = file.Name,
                Id = file.Id,
                IconLink = file.IconLink,
                FileExtension = file.FileExtension,
                Thumbnail = file.ThumbnailLink,
                WebContentLink = file.WebContentLink,
                WebViewLink = file.WebViewLink,
                MimeType = file.MimeType,
                Parents = file.Parents
            };
        }

        public static FolderModel ToFolderModel(File file)
        {
            return new FolderModel
            {
                Name = file.Name,
                Id = file.Id,
                IconLink = file.IconLink,
                FileExtension = file.FileExtension,
                Thumbnail = file.ThumbnailLink,
                WebContentLink = file.WebContentLink,
                WebViewLink = file.WebViewLink,
                MimeType = file.MimeType,
                Parents = file.Parents
            };
        }
    }
}
