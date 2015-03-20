using StoreWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StoreWebApp.Controllers.API
{
    [EnableCors(origins: "http://octostore.azurewebsites.net", headers: "*", methods: "*")]
    public class OctoAppController : ApiController
    {
        public IEnumerable<AppRegister> Get()
        {
            OctoSQLBaseEntities octoBase = new OctoSQLBaseEntities();
            return (from a in octoBase.AppRegister
                    select a).ToList();
        }

        // GET api/<controller>/5
        public AppRegister Get(int idApp)
        {
            OctoSQLBaseEntities octoBase = new OctoSQLBaseEntities();
            AppRegister app = (from a in octoBase.AppRegister
                               where a.Id == idApp
                               select a).FirstOrDefault();
            return app;
        }

        [HttpPost]
        public int post(AppRegister app)
        {
            try
            {
                using (OctoSQLBaseEntities octoBase = new OctoSQLBaseEntities())
                {
                    octoBase.AppRegister.Add(app);
                    octoBase.SaveChanges();
                    return app.Id;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

        }
    }
}