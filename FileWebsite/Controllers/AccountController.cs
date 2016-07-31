using FileWebsite.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FileWebsite.Security;

namespace FileWebsite.Controllers
{

    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            LoginHelper loginHelper = new LoginHelper();

            if (loginHelper.LoginCheck(model.username, model.password))
            {

                Session["Username"] = model.username;

                Session["UserId"] = loginHelper.FindUserID(model.username).ToString();

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Register");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            RegisterHelper registerHelper = new RegisterHelper();

            if (ModelState.IsValid)
            {
                registerHelper.GenerateUser(model);
                return RedirectToAction("Login");
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}