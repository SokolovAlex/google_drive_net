using System;
using System.Collections.Generic;
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
using GDriveApi_v2.Services;
using GDriveApp.Models.api.Requests;
using GDriveApp.Models.api.Responses;

namespace GDriveApp.Controllers
{
    public class FileController : ApiController
    {
        [HttpGet, Route("api/files")]
        public HttpResponseMessage Get()
        {
            var keyPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileStorage-6817de257573.p12");
            GoogleDriveService.Authenticate(keyPath);

            var files_v3 = GoogleDriveService.GetRootFiles();

            //GoogleDriveService_v2.Authenticate(keyPath);
            //var files_v2 = GoogleDriveService_v2.GetFiles();

            var folders = GoogleDriveService.GetFolders(files_v3);

            GoogleDriveService.MemorySelectedFolder(null);

            return Request.CreateResponse(HttpStatusCode.OK, new FilesResponse
            {
                files = files_v3,
                folders = folders
            });
        }

        [HttpGet, Route("api/files/{parent}")]
        public HttpResponseMessage GetIn(string parent)
        {
            var keyPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileStorage-6817de257573.p12");
            GoogleDriveService.Authenticate(keyPath);

            var files_v3 = GoogleDriveService.GetFilesIn(parent);
            var folders = GoogleDriveService.GetFolders(files_v3);

            GoogleDriveService.MemorySelectedFolder(parent);

            return Request.CreateResponse(HttpStatusCode.OK, new FilesResponse
            {
                files = files_v3,
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
                GoogleDriveService.UploadFile(stream, filename);
            }

            return Ok();
        }

        [Route("api/upload2")]
        [HttpPost]
        public string MyFileUpload()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var request = HttpContext.Current.Request;
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload");

            using (var fs = new FileStream(Path.Combine(filePath, "someName"), FileMode.Create))
            {
                request.InputStream.CopyTo(fs);
            }
            return "uploaded";
        }

        [HttpDelete, Route("api/file/{id}")]
        public void Delete(string id)
        {
            GoogleDriveService.DeleteFile(id);
            //GoogleDriveService_v2.DeleteFile(id);
        }

        [HttpPost, Route("api/folder")]
        public void CreateFolder(CreateFolderRequest req)
        {
            //GoogleDriveService_v2.CreateDirectory(req.folderName, req.folderDesc);
            GoogleDriveService.CreateDirectory(req.folderName, req.folderDesc, req.parentId);
        }
    }
}
