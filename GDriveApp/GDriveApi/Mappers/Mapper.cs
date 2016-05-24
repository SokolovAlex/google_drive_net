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
                IconLink = file.IconLink,
                FileExtension = file.FileExtension,
                Shared = file.Shared,
                Thumbnail = file.ThumbnailLink
            };
        }

        public static FolderModel ToFolderModel(File file)
        {
            return new FolderModel
            {
                Name = file.Name,
                IconLink = file.IconLink,
                FileExtension = file.FileExtension,
                Shared = file.Shared,
                Thumbnail = file.ThumbnailLink
            };
        }
    }
}
