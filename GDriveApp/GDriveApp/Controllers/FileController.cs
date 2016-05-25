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
using File = GDrive.Dal.dbEntities.File;

namespace GDriveApp.Controllers
{
    public class FileController : ApiController
    {
        // GET api/values
        [Route("api/files")]
        public HttpResponseMessage Get()
        {
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileStorage-6817de257573.p12");
            GoogleDriveService.Authenticate(mappedPath);

            GoogleDriveService_v2.Authenticate(mappedPath);

            var files_v3 = GoogleDriveService.GetFiles();

            var files_v2 = GoogleDriveService_v2.GetFiles();

            return Request.CreateResponse(HttpStatusCode.OK, files_v2);
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
                var bytes = await file.ReadAsByteArrayAsync();

                GoogleDriveService.UploadFile2(bytes, filename);
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

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
