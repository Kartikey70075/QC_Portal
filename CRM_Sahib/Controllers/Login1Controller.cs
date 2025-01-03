using V_Weave_Qc;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using V_Weave_Qc.Models;

namespace V_Weave_Qc.Controllers
{
    public class Login1Controller : Controller
    {


        HanaSQL Sqlhana = new HanaSQL();
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
        public ActionResult Login(_VWAVEQCLOGIN user)
        {
            HanaConnection con = new HanaConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Hana"].ConnectionString);
            string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
            string command = "select * from" + "\"" + SCHEMA + "\"" + ".\"@VWAVEQCLOGIN\" where  \"U_UserID\"=" + "'" + user.U_UserId + "'" + " and \"U_Password\"=" + "'" + user.U_Password + "'" + " and \"U_UserActivity\"=1";

            DataTable DT = new DataTable();
            DT = Sqlhana.GetHanaDataSQL(command);
            if (DT.Rows.Count > 0)
            {

                Session["UserID"] = user.U_UserId;
                Session["Password"] = user.U_Password;
                Session["Email"] = DT.Rows[0]["U_Email"];
                Session["UserName"] = DT.Rows[0]["U_UserName"];
                Session["LoggedINTime"] = DateTime.Now;

                FormsAuthentication.SetAuthCookie(user.U_UserId, false);
                var authTicket = new FormsAuthenticationTicket(1, user.U_UserId, DateTime.Now, DateTime.Now.AddMinutes(20), false, user.U_UserName);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                
                return RedirectToAction("CreateQcOnGrpo", "Home1");
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
            return RedirectToAction("Login", "Login1");
        }
    }
}