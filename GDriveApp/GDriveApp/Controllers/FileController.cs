using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Dto.Models;
using GDrive.Dal.dbEntities;
using GDriveApi.Services;

namespace GDriveApp.Controllers
{
    public class FileController : ApiController
    {
        // GET api/values
        [Route("api/files")]
        public HttpResponseMessage Get()
        {
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileStorage-721027ef5d82.p12");
            GoogleDriveService.Authenticate(mappedPath);
            var files = GoogleDriveService.GetFiles();

            return Request.CreateResponse(HttpStatusCode.OK, files);
        }

        // GET api/values/5
        public File Get(int id)
        {
            return new File();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
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
