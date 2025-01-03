using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Sahib.Controllers
{
    public class SalesOrderController : Controller
    {
        // GET: SalesOrder
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateSalesOrder()
        {
            if (Session["UserType"] != null)
            {
                if (Session["UserType"].ToString().ToUpper() == "ADMIN" || Session["UserType"].ToString().ToUpper() == "CUSTOMER")
                {
                    return View();
                }
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Login", "Login");
        }
    }
}