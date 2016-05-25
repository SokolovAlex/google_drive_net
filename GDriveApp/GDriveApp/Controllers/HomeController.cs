using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDriveApi.Model;
using GDriveApi.Services;
using GDriveApp.Models;

namespace GDriveApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.User = CurrentUser.IsLoggedIn ? CurrentUser.Info.name: "Service account";
            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult GoogleRedirect()
        {
            var url = OAuth2Service.OAuth2Url();
            return Redirect(url.AbsoluteUri);
        }

        public ActionResult GoogleCallback(string code)
        {
            OAuth2Service.GetToken(code);
            var account = OAuth2Service.GetProfileInfo();
            CurrentUser.Info = account;

            return RedirectToAction("Index");
        }
    }
}
