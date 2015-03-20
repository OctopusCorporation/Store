using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StoreWebApp.Models;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace StoreWebApp.Controllers.API
{
    /// <summary>
    /// This API is created as Alpha release, please change database
    /// and algorithms
    /// </summary>
    public class OctoBinaryAppController : ApiController
    {
        OctoSQLBaseEntities octoBase = new OctoSQLBaseEntities();
        
        public HttpResponseMessage GetZip(int idBinary)
        {
            var data = from i in octoBase.AppBinary
                       where i.Id == idBinary
                       select i;
            AppBinary appZip = (AppBinary)data.SingleOrDefault();
            byte[] zipData = appZip.File;
            MemoryStream ms = new MemoryStream(zipData);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
            return response;
        }

        // POST api/<controller>
        public Task<IEnumerable<string>> Post()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string fullPath = HttpContext.Current.Server.MapPath("~/App_Data");
                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(fullPath);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);

                        AppBinary app = new AppBinary();
                        app.IdApp = Int32.Parse(streamProvider.FormData.GetValues("IdApp").SingleOrDefault());
                        app.File = File.ReadAllBytes(info.FullName);
                        octoBase.AppBinary.Add(app);
                        octoBase.SaveChanges();
                        return "App uploaded successfully!";
                    });
                    return fileInfo;
                });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
            }
        }

        
    }
}