using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dto.Models;
using Dto.Models.Drive;
using GDriveApi.Model;
using GDriveApi_v2.Mappers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Newtonsoft.Json;
using DriveData = Google.Apis.Drive.v2.Data;

namespace GDriveApi_v2.Services
{
    public static class GoogleDriveService_v2
    {
        private static string[] scopes = { DriveService.Scope.DriveFile };
        private static DriveService service;

        #region private
        #endregion

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

        #region oath2 
        public static void OAuth2(string code)
        {
            //Scopes for use with the Google Drive API
            string[] scopes = new string[] { DriveService.Scope.Drive,
                                 DriveService.Scope.DriveFile};

            var clientId = ConfigurationSettings.AppSettings["client_id"];
            var clientSecret = ConfigurationSettings.AppSettings["client_secret"];

            // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
                scopes,
                Environment.UserName,
                CancellationToken.None).Result;


            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            });

        }
        #endregion

        public static Task<DriveData.FileList> GetFilesAsync()
        {
            FilesResource.ListRequest request = service.Files.List();
            return request.ExecuteAsync();
        }

        public static IEnumerable<FileModel> GetFiles(SearchFilter filter = null)
        {
            FilesResource.ListRequest request = service.Files.List();
            request.MaxResults = filter == null ? 20 : filter.PageSize;
            request.Q = filter == null ? null : filter.Query;
            var list = request.Execute();

            return list.Items.Select(Mapper.ToFileModel);
        }

        public static FileModel GetFile(string id)
        {
            var request = service.Files.Get(id);
            var file = request.Execute();
            return Mapper.ToFileModel(file);
        }

        public static void UploadFile(Stream stream, string name)
        {
            var insertRequest = service.Files.Insert(
                new DriveData.File
                {
                    Title = name,
                },
                stream,
                "image/jpeg");

            var task = insertRequest.UploadAsync();
            task.ContinueWith(t =>
            {
                stream.Dispose();
            });
        }

        public static void UploadFile(byte[] bytes, string name)
        {
            MemoryStream stream = new MemoryStream(bytes);

            var insertRequest = service.Files.Insert(
                new DriveData.File
                {
                    Title = name,
                },
                stream,
                "image/jpeg");

            var task = insertRequest.UploadAsync();
            task.ContinueWith(t =>
            {
                stream.Dispose();
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
                Title = name,
                Description = description,
                MimeType = "application/vnd.google-apps.folder"
            };
            try
            {
                var request = service.Files.Insert(body);
                newDirectory = request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return Mapper.ToFolderModel(newDirectory);
        }

        public static  void DeleteFile(string id)
        {
            var req = service.Files.Delete(id);
            req.Execute();
        }

        #region async 

        public static async void DeleteFileAsync(string id)
        {
            var req = service.Files.Delete(id);
            await req.ExecuteAsync();
        }
        #endregion
    }
}
