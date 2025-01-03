using CRM_Sahib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Sahib.Controllers
{
    

   public class HomeController : Controller
    {
        SahibDataContext con = null;
        SapPunch sapPunch = null;
        string errorMsg = "";
        int errorCode = 0;
        public HomeController()
        {
            con = new SahibDataContext();
            sapPunch = new SapPunch();
        }
        public ActionResult Index()
        {
            if (Session["UserType"] != null)
            {
                if (Session["UserType"].ToString().ToUpper() == "ADMIN")
                {
                    return View();
                }
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Login", "Login");
        }

        public ActionResult Welcome()
        {
            if (Session["UserType"] != null)
            {
                if (Session["UserType"].ToString().ToUpper() == "ADMIN")
                {
                    return View();
                }
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Login", "Login");
        }






       [HttpPost]
        public string InsertRecord(_USERDETAIL ab)
        {
            try
            {
                var code = con._USERDETAILs.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();


                if (!string.IsNullOrEmpty(code))
                {
                    ab.Code = (Convert.ToInt32(code) + 1).ToString();
                }
                else
                {
                    ab.Code = "1";
                }

                con._USERDETAILs.InsertOnSubmit(ab);
                con.SubmitChanges();
                return "Student Added Sucessfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
               
        }

        [HttpPost]
        public JsonResult AddPunchOpportunity(OpportunityList OppList)
        {
            try
           {
                sapPunch.PunchOpp(OppList, ref errorCode, ref errorMsg);

                if (errorCode == 0)
                {
                    
                        if (OppList.OpprId > 0)
                        {
                            return Json("Opportunity is updated with No. " + errorMsg,JsonRequestBehavior.AllowGet);
                        }
                        return Json("Opportunity is Created with No. " + errorMsg, JsonRequestBehavior.AllowGet);
                    
                }
                else
                    return Json(errorMsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}