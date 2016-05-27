using Dto.Models;
using Google.Apis.Drive.v2.Data;

namespace GDriveApi_v2.Mappers
{
    public static class Mapper
    {
        public static FileModel ToFileModel(File file)
        {
            return new FileModel
            {
                Name = file.Title,
                Id = file.Id,
                IconLink = file.IconLink,
                FileExtension = file.FileExtension,
                Shared = file.Shared,
                Thumbnail = file.ThumbnailLink,
                SelfLink = file.SelfLink,
                WebContentLink = file.WebContentLink,
                DownloadUrl = file.DownloadUrl
            };
        }

        public static FolderModel ToFolderModel(File file)
        {
            return new FolderModel
            {
                Id = file.Id,
                Name = file.Title,
                IconLink = file.IconLink,
                FileExtension = file.FileExtension,
                Shared = file.Shared,
                Thumbnail = file.ThumbnailLink,
                SelfLink = file.FileExtension,
                WebContentLink = file.WebContentLink,
                DownloadUrl = file.DownloadUrl
            };
        }
    }
}
