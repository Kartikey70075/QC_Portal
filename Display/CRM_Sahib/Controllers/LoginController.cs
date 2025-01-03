using CRM_Sahib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CRM_Sahib.Controllers
{
    public class LoginController : Controller
    {

        SahibDataContext con = null;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(_USERLOGIN user)
        {
            con = new SahibDataContext();

            var db = con._USERLOGINs.Where(gt => gt.U_UserID == user.U_UserID.Trim() && gt.U_UserPassword == user.U_UserPassword).Select(gt => gt).ToList();
            if (db.Count > 0)
            {
                Session["Userid"] = user.U_UserID;
                Session["Name"] = db[0].Name;
                Session["UserType"] = db[0].U_UserType;
                //Session["Branch"] = db[0].U_Branch;
               //Session["LoggedINTime"] = DateTime.Now;


                FormsAuthentication.SetAuthCookie(db[0].Name, false);
                var authTicket = new FormsAuthenticationTicket(1, db[0].Name, DateTime.Now, DateTime.Now.AddMinutes(20), false, db[0].U_UserType);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                //HttpContext.Response.Cookies.Add(authCookie);
                if (db[0].U_UserType == "Customer")
                {
                    return RedirectToAction("Index", "Customer");
                }
                else if(db[0].U_UserType == "admin")
                {
                    return RedirectToAction("Welcome", "Home");
                }
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ViewBag.message = "UserID or Password is wrong !!";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }
    }
}