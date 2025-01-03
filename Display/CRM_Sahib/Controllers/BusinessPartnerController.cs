using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Sahib.Controllers
{
    public class BusinessPartnerController : Controller
    {
        // GET: BusinessPartner
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateBP()
        {
            if(Session["UserType"] != null)
            {
                if(Session["UserType"].ToString().ToUpper() == "ADMIN")
                {
                    return View();
                }
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Login", "Login");

        }
    }
}