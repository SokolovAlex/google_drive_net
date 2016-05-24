using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dto.Models;
using Dto.Models.Drive;
using GDriveApi.Mappers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using DriveData = Google.Apis.Drive.v3.Data;

namespace GDriveApi.Services
{
    public static class GoogleDriveService
    {
        private static string[] scopes = { DriveService.Scope.Drive };
        private static DriveService service;

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

        public static async Task AuthenticateAsync(string pathToP12)
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

        public static Task<FileList> GetFilesAsync()
        {
            FilesResource.ListRequest request = service.Files.List();
            return request.ExecuteAsync();
        }

        public static IEnumerable<FileModel> GetFiles(SearchFilter filter = null)
        {
            FilesResource.ListRequest request = service.Files.List();
            request.PageSize = filter == null ? 20 : filter.PageSize;
            request.Q = filter == null ? null : filter.Query;
            var list = request.Execute();

            return list.Files.Select(Mapper.ToFileModel);
        }

        private static void Upload_ProgressChanged(IUploadProgress progress)
        {
            Console.WriteLine(progress.Status + " " + progress.BytesSent);
        }

        private static void Upload_ResponseReceived(DriveData.File file)
        {
            Console.WriteLine(file.Name + " was uploaded successfully");
        }

        public static void UploadFile()
        {
            var uploadStream = new System.IO.FileStream("FILE_NAME",
                                                        FileMode.Open,
                                                        FileAccess.Read);
            
            // Get the media upload request object.
            var insertRequest = service.Files.Create(
                new DriveData.File
                {
                    Name = "FILE_TITLE"
                },
                uploadStream,
                "image/jpeg");

            // Add handlers which will be notified on progress changes and upload completion.
            // Notification of progress changed will be invoked when the upload was started,
            // on each upload chunk, and on success or failure.
            insertRequest.ProgressChanged += Upload_ProgressChanged;
            insertRequest.ResponseReceived += Upload_ResponseReceived;

            var task = insertRequest.UploadAsync();
            task.ContinueWith(t =>
            {
                uploadStream.Dispose();
            });
        }

        public static bool Exist(string path)
        {
            return true;
        }

        public static FolderModel CreateDirectory(string name, string description)
        {
            DriveData.File newDirectory = null;
            DriveData.File body = new DriveData.File
            {
                Name = name,
                Description = description,
                MimeType = "application/vnd.google-apps.folder"
            };
            try
            {
                var request = service.Files.Create(body);
                newDirectory = request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return Mapper.ToFolderModel(newDirectory);
        }
    }
}
