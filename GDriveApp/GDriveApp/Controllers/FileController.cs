using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Dto.Models;
using GDriveApi.Services;
using GDriveApp.Models.api.Requests;
using GDriveApp.Models.api.Responses;

namespace GDriveApp.Controllers
{
    public class FileController : ApiController
    {
        [HttpGet, Route("api/files")]
        public HttpResponseMessage Get()
        {
            GoogleDriveService.MemorySelectedFolder(null);

            var authKeyPath = ConfigurationSettings.AppSettings["key_p12_path"];
            var keyPath = System.Web.Hosting.HostingEnvironment.MapPath(authKeyPath);
            GoogleDriveService.Authenticate(keyPath);

            var files = GoogleDriveService.GetRootFiles();
            var folders = GoogleDriveService.RetainFolders(files);

            return Request.CreateResponse(HttpStatusCode.OK, new FilesResponse
            {
                files = files,
                folders = folders
            });
        }

        [HttpGet, Route("api/files/{parent}")]
        public HttpResponseMessage GetIn(string parent)
        {
            GoogleDriveService.MemorySelectedFolder(parent);

            var files = GoogleDriveService.GetFiles(parent);
            var folders = GoogleDriveService.RetainFolders(files);

            return Request.CreateResponse(HttpStatusCode.OK, new FilesResponse
            {
                files = files,
                folders = folders
            });
        }

        [HttpPost, Route("api/upload/save")]
        public async Task<IHttpActionResult> UploadSave()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload");

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();

                var fs = new BinaryWriter(new FileStream(Path.Combine(filePath, filename), FileMode.Create, FileAccess.Write));
                fs.Write(buffer);
                fs.Close();
            }

            return Ok();
        }

        [HttpPost, Route("api/upload")]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var stream = await file.ReadAsStreamAsync();
                GoogleDriveService.UploadSharedFile(stream, filename);
            }

            return Ok();
        }

        [HttpDelete, Route("api/file/{id}")]
        public void Delete(string id)
        {
            GoogleDriveService.DeleteFile(id);
        }

        [HttpPost, Route("api/share/{id}")]
        public void Share(string id)
        {
            GoogleDriveService.Share(id);
        }

        [HttpPost, Route("api/folder")]
        public void CreateFolder(CreateFolderRequest req)
        {
            GoogleDriveService.CreateFolder(req.folderName, req.folderDesc, req.parentId);
        }

        [HttpGet, Route("api/folder")]
        public void GetFolders()
        {
            GoogleDriveService.GetFolders();
        }

        [HttpGet, Route("api/folder/{parent}")]
        public void GetFolders(string parent)
        {
            GoogleDriveService.GetFolders(parent);
        }
    }
}
