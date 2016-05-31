using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dto.Models;
using Dto.Models.Drive;
using GDriveApi.Mappers;
using GDriveApi.Model;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Generators;
using DriveData = Google.Apis.Drive.v3.Data;

namespace GDriveApi.Services
{
    public static class GoogleDriveService
    {
        #region fields
        private static string[] scopes = { DriveService.Scope.DriveFile };

        private static DriveService service;

        private static string selectedFolder { get; set; }

        private static readonly string folderMimeType = "application/vnd.google-apps.folder";

        private static Permission allowPermission = new Permission
        {
            Type = "anyone",
            Role = "writer",
            AllowFileDiscovery = true,
        };
        #endregion

        #region private
        private static DriveData.File GetFile(string id, string[] fields)
        {
            var request = service.Files.Get(id);
            request.Fields = String.Join(",", fields);
            return request.Execute();
        }

        private static DriveData.File GetFile(string id)
        {
            return GetFile(id, new string[] { });
        }

        private static void Upload(Stream stream, string name, bool shared = true)
        {
            var file = new DriveData.File
            {
                Name = name
            };
            if (selectedFolder != null)
            {
                file.Parents = new List<string> { selectedFolder };
            }

            var uploadRequest = service.Files.Create(file, stream, "image/jpeg");
            uploadRequest.Fields = "id";
            var task = uploadRequest.UploadAsync();

           task.ContinueWith(t =>
            {
                stream.Dispose();
                if (shared)
                {
                    Share(uploadRequest.ResponseBody.Id);
                }
            });
        }

        private static void Upload(byte[] bytes, string name, bool shared = true)
        {
            MemoryStream stream = new MemoryStream(bytes);

            var file = new DriveData.File
            {
                Name = name
            };

            var uploadRequest = service.Files.Create(file, stream, "image/jpeg");
            uploadRequest.Fields = "id";
            var task = uploadRequest.UploadAsync();

            task.ContinueWith(t =>
            {
                stream.Dispose();
                if (shared)
                {
                    Share(uploadRequest.ResponseBody.Id);
                }
            });
        }
        #endregion

        #region auth
        public static void Authenticate(string pathToP12)
        {
            if (service != null) return;

            var serviceAccountEmail = ConfigurationSettings.AppSettings["client_email"];
            var certificate = new X509Certificate2(pathToP12, "notasecret", X509KeyStorageFlags.Exportable);

            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = scopes
            }
            .FromCertificate(certificate));

            service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            });
        }
        #endregion

        #region folders
        public static FolderModel CreateFolder(string name, string description, string parentId, bool shared = true)
        {
            DriveData.File newDirectory = null;
            DriveData.File body = new DriveData.File
            {
                Name = name,
                Description = description,
                MimeType = folderMimeType
            };
            if (parentId != null)
            {
                body.Parents = new List<string> {parentId};
            }
            try
            {
                var request = service.Files.Create(body);
                newDirectory = request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            var result = Mapper.ToFolderModel(newDirectory);

            if (shared)
            {
                Share(result.Id);
            }

            return result;
        }

        public static IEnumerable<FileModel> GetFolders()
        {
            return GetFiles(new SearchFilter
            {
                Query = String.Format("mimeType='{0}'", folderMimeType)
            });
        }

        public static IEnumerable<FileModel> GetFolders(string parent)
        {
            return GetFiles(new SearchFilter
            {
                Query = String.Format("mimeType='{0}' and '{1}' in parents", folderMimeType, parent)
            });
        }

        public static IEnumerable<FileModel> RetainFolders(IEnumerable<FileModel> files)
        {
            return files.Where(x => x.MimeType == folderMimeType);
        }

        #endregion

        #region get
        public static IEnumerable<FileModel> GetRootFiles()
        {
            return GetFiles(new SearchFilter
            {
                Query = "'root' in parents"
            });
        }

        public static IEnumerable<FileModel> GetFiles(string parent)
        {
            return GetFiles(new SearchFilter
            {
                Query = String.Format("'{0}' in parents", parent)
            });
        }

        public static async Task<FileList> GetFilesAsync(SearchFilter filter = null)
        {
            FilesResource.ListRequest request = service.Files.List();
            request.PageSize = filter == null ? 20 : filter.PageSize;
            request.Q = filter == null ? null : filter.Query;
            request.Fields = "files";
            return await request.ExecuteAsync();
        }

        public static IEnumerable<FileModel> GetFiles(SearchFilter filter = null)
        {
            FilesResource.ListRequest request = service.Files.List();
            request.PageSize = filter == null ? 20 : filter.PageSize;
            request.Q = filter == null ? null : filter.Query;
            request.Fields = "files";
            var list = request.Execute();
            return list.Files.Select(Mapper.ToFileModel);
        }

        public static void MemorySelectedFolder(string parent)
        {
            selectedFolder = parent;
        }

        #endregion

        #region upload

        public static void UploadSharedFile(Stream stream, string name)
        {
            Upload(stream, name);
        }

        public static void UploadSharedFile(byte[] bytes, string name)
        {
            Upload(bytes, name);
        }

        public static void UploadFile(Stream stream, string name)
        {
            Upload(stream, name, false);
        }

        public static void UploadFile(byte[] bytes, string name)
        {
            Upload(bytes, name, false);
        }

        #endregion

        #region share
        public static void Share(string fileId)
        {
            var req = service.Permissions.Create(allowPermission, fileId);
            req.Fields = "id";
            req.Execute();
        }

        public static async void ShareAsync(string fileId)
        {
            var req = service.Permissions.Create(allowPermission, fileId);
            req.Fields = "id";
            await req.ExecuteAsync();
        }
        #endregion

        #region delete
        public static void DeleteFile(string id)
        {
            var req = service.Files.Delete(id);
            req.Execute();
        }

        public static async void DeleteFileAsync(string id)
        {
            var req = service.Files.Delete(id);
            await req.ExecuteAsync();
        }
        #endregion
    }
}
