using V_Weave_Qc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Globalization;
using Newtonsoft.Json;
using System.Net.Mail;
using Sap.Data.Hana;
using System.Net;
using ClosedXML.Excel;

namespace V_Weave_Qc.Controllers
{


    public class Home1Controller : Controller
    {

        public Home1Controller()
        {

        }
        public ActionResult Welcome()
        {
            return View();

        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateQcOnGrpo()
        {

            return View();
        }
        public ActionResult CreateQCDaing()
        {

            return View();
        }
        public ActionResult CreateQC_on_Production()
        {

            return View();
        }
        public ActionResult XLDownload()
        {

            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateQcOnGrpo(HttpPostedFileBase file)
        {

            Session["file"] = file;
            string _path = string.Empty;
            if (file.ContentLength > 0)
            {
                string _FileName = Path.GetFileName(file.FileName);
                _path = Path.Combine(Server.MapPath("~/Uploads"), _FileName);
                file.SaveAs(_path);
                TempData["Path"] = _path;
                ViewBag.Message = "File Uploaded Successfully!!";
                Session["Path"] = _path;
            }
            else
            {
                ViewBag.Message = "Please Upload File!!";
                TempData["Path"] = "empty";
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateQCDaing(HttpPostedFileBase file)
        {
            Session["file"] = file;
            string _path = string.Empty;
            if (file.ContentLength > 0)
            {
                string _FileName = Path.GetFileName(file.FileName);
                _path = Path.Combine(Server.MapPath("~/Uploads"), _FileName);
                file.SaveAs(_path);
                TempData["Path"] = _path;
                ViewBag.Message = "File Uploaded Successfully!!";
                Session["Path"] = _path;

            }
            else
            {
                ViewBag.Message = "Please Upload File!!";
                TempData["Path"] = "empty";
            }
            return View();

        }
        HanaSQL Sqlhana = new HanaSQL();
        static HanaConnection con = new HanaConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Hana"].ConnectionString);
        public static string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];

        #region row material qc by wamique shaikh 09-23_2022

        [HttpPost]
        public JsonResult SaveQcData(Qcmodeldata qcmodeldata, QcItemtableModel qcitem, _VWAVEQCLOGIN user, string Email)
        {
            try
            {
                string responsemsg = string.Empty;
                SapDataPunch sapDataPunch = new SapDataPunch();
                responsemsg = "";
                responsemsg = sapDataPunch.Save_QC_Data(qcmodeldata, qcitem, Email);

                return Json(responsemsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SentMail(string Email, Qcmodeldata qcmodeldata, QcItemtableModel qcitem)
        {
            try
            {
               
                    string Comand = "select top 1 \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_OBSH\"  ORDER BY \"DocEntry\" DESC";
                    DataTable DT = Sqlhana.GetHanaDataSQL(Comand);
                    string DocEntry = DT.Rows[0]["DocEntry"].ToString();

                    string Comand2 = "select \"SalUnitMsr\" from " + "\"" + SCHEMA + "\"" + ".\"OITM\" where \"ItemCode\" = '" + qcmodeldata.QcItem[0].ItemCode + "'";
                    DataTable DT2 = Sqlhana.GetHanaDataSQL(Comand2);
                    string UoM = DT2.Rows[0]["SalUnitMsr"].ToString();

                    var a = qcmodeldata.QcItem[0].rejectqty;
                    var b = qcmodeldata.QcItem[0].SampleQuantity;
                    var c = qcmodeldata.QcItem[0].totalpass;
                    var d = qcmodeldata.QcItem[0].ItemCode;
                    var e = qcmodeldata.QcItem[0].NumAtCard;
                    var f = qcmodeldata.QcItem[0].holdingqty;
                    var g = qcmodeldata.QcItem[0].Quantity;
                    var h = qcmodeldata.QcItem[0].CardName;
                    var ab = DocEntry;
                    //MailAddress bcc = new MailAddress("Riahabh.katoch@itssquad.com");
                    using (MailMessage mm = new MailMessage("erp@v-weave.com", Email))
                    {
                        mm.Subject = "" + h + " QC No. " + ab;
                        mm.Body = "Dear " + h + ", <br /> We have received your Material " + qcmodeldata.QcItem[0].ItemCode + " - " + qcmodeldata.QcItem[0].Quantity + "." + UoM + " < br /> Description Name  - " + qcmodeldata.QcItem[0].Dscription + "<br />  Base on Bill Number - " + qcmodeldata.QcItem[0].NumAtCard + " and <br />  Manual Date - " + qcmodeldata.QcItem[0].ManualDate + "  and  <br />  PO Number - " + qcmodeldata.QcItem[0].BaseRef + " and QC Department Has given below details:<br /> <br />  QC Approved - "
                         + qcmodeldata.QcItem[0].totalpass + "." + UoM + " <br />  QC Not Approve/Hold - " + qcmodeldata.QcItem[0].holdingqty + "." + UoM + " " +
                         "<br />  QC Extra/Extra - " + qcmodeldata.QcItem[0].extraqty + "." + UoM + " <br />  QC Rejected - " + qcmodeldata.QcItem[0].rejectqty + "." + UoM + " <br />  QC Rework/Rework - " + qcmodeldata.QcItem[0].reworkqty + "." + UoM + " <br /> <br /> <br /> <br />  With Regards <br /> V-WEAVE <br /> QC Department ";
                        //mm.CC.Add(bcc);
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential("erp@v-weave.com", "tfgvaoktfoascutg");
                        smtp.UseDefaultCredentials = true; 
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                        return Json("mail send", JsonRequestBehavior.AllowGet);
                    }
                }
            catch (Exception ex)
            {
                return Json("eroor in mail".ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Create Productin Save And Mail 26-09-2022
        [HttpPost]
        public JsonResult SaveQc_ProdData(QcProdmodeldata qcprodmodeldata, QcPRODItemtableModel qcItemtableModel, Qcmodeldata qcmodeldata, QcItemtableModel qcitem, string Email)
        {
            try
            {
                string responsemsg = string.Empty;
                SapDataPunch sapDataPunch = new SapDataPunch();
                responsemsg = "";
                responsemsg = sapDataPunch.Save_QC_PROD_Data(qcprodmodeldata, qcItemtableModel, qcitem, Email);
                return Json(responsemsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult SentMailProduction(QcProdmodeldata qcprodmodeldata, QcItemtableModel qcitem, string Email)
        {
            try
            {
                if (qcprodmodeldata.QcItem[0].QC_Process == "Weaving")
                {
                    string Comand = "select top 1 \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_OBSH\"  ORDER BY \"DocEntry\" DESC";
                    DataTable DT = Sqlhana.GetHanaDataSQL(Comand);
                    string DocEntry = DT.Rows[0]["DocEntry"].ToString();

                    string Comand2 = "select \"SalUnitMsr\" from" + "\"" + SCHEMA + "\"" + ".\"OITM\" where \"ItemCode\" = '" + qcprodmodeldata.QcItem[0].ItemCode + "'";
                    DataTable DT2 = Sqlhana.GetHanaDataSQL(Comand2);
                    string UoM = DT2.Rows[0]["SalUnitMsr"].ToString();

                    var a = qcprodmodeldata.QcItem[0].rejectqty;
                    var b = qcprodmodeldata.QcItem[0].SampleQuantity;
                    var c = qcprodmodeldata.QcItem[0].totalpass;
                    var d = qcprodmodeldata.QcItem[0].ItemCode;
                    var e = qcprodmodeldata.QcItem[0].NumAtCard;
                    var f = qcprodmodeldata.QcItem[0].holdingqty;
                    var g = qcprodmodeldata.QcItem[0].Quantity;
                    var h = qcprodmodeldata.QcItem[0].CardName;
                    var ab = DocEntry;
                    var x = qcprodmodeldata.QcItem[0].WorkOrder;
                    var z = qcprodmodeldata.QcItem[0].ManualDate;
                    //MailAddress bcc = new MailAddress("Riahabh.katoch@itssquad.com");
                    using (MailMessage mm = new MailMessage("erp@v-weave.com", Email))
                    {
                        mm.Subject = "" + h + " QC No. " + ab;
                        mm.Body = "Dear " + h + ", <br /> We have received your Material " + qcprodmodeldata.QcItem[0].ItemCode + " - " + qcprodmodeldata.QcItem[0].Quantity + "." + UoM + " <br /> Alias Name  - " + qcprodmodeldata.QcItem[0].AliasName + " <br/> Manual Date " + Convert.ToDateTime(qcprodmodeldata.QcItem[0].ManualDate).ToString("dd-MM-yyyy") + " <br/> Work Order Number " + qcprodmodeldata.QcItem[0].WorkOrder + " -  <br />  " +
                           "Base on Bill Number " + qcprodmodeldata.QcItem[0].InvoicesNumber + " - <br /> " +
                           "and QC Department Has given below details:<br /> <br />  QC Approved - "
                         + qcprodmodeldata.QcItem[0].totalpass + "." + UoM + " <br />  QC Not Approve/Hold - " + qcprodmodeldata.QcItem[0].holdingqty + "." + UoM + " <br />  QC Rejected - " + qcprodmodeldata.QcItem[0].rejectqty + "." + UoM + " <br />  QC Rework - " + qcprodmodeldata.QcItem[0].reworkqty + "." + UoM + " <br /> <br /> <br /> <br />  With Regards <br /> V-WEAVE <br /> QC Department ";
                        //mm.CC.Add(bcc);
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential("erp@v-weave.com", "tfgvaoktfoascutg");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                        return Json("mail send", JsonRequestBehavior.AllowGet);

                    }

                }

                else
                {
                    string Comand2 = "select top 1 \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_OBSH\"  ORDER BY \"DocEntry\" DESC";
                    DataTable DT1 = Sqlhana.GetHanaDataSQL(Comand2);
                    string DocEntry = DT1.Rows[0]["DocEntry"].ToString();

                    string Comand3 = "select \"SalUnitMsr\" from " + "\"" + SCHEMA + "\"" + ".\"OITM\" where \"ItemCode\" = '" + qcprodmodeldata.QcItem[0].ItemCode + "'";
                    DataTable DT3 = Sqlhana.GetHanaDataSQL(Comand3);
                    string UoM = DT3.Rows[0]["SalUnitMsr"].ToString();

                    var a = qcprodmodeldata.QcItem[0].rejectqty;
                    var b = qcprodmodeldata.QcItem[0].SampleQuantity;
                    var c = qcprodmodeldata.QcItem[0].totalpass;
                    var d = qcprodmodeldata.QcItem[0].ItemCode;
                    var e = qcprodmodeldata.QcItem[0].NumAtCard;
                    var f = qcprodmodeldata.QcItem[0].holdingqty;
                    var g = qcprodmodeldata.QcItem[0].Quantity;
                    var h = qcprodmodeldata.QcItem[0].CardName;
                    var ab = DocEntry;
                    var x = qcprodmodeldata.QcItem[0].WorkOrder;
                    var z = qcprodmodeldata.QcItem[0].ManualDate;
                    //MailAddress bcc = new MailAddress("Riahabh.katoch@itssquad.com");
                    using (MailMessage mm = new MailMessage("erp@v-weave.com", Email))
                    {
                        mm.Subject = "" + h + " QC No. " + ab;
                        mm.Body = "Dear " + h + ", <br /> We have received your Material " + qcprodmodeldata.QcItem[0].ItemCode + " - " + qcprodmodeldata.QcItem[0].Quantity + "." + UoM + " <br /> Description Name  - " + qcprodmodeldata.QcItem[0].Dscription + " <br/> Manual Date " + Convert.ToDateTime(qcprodmodeldata.QcItem[0].ManualDate).ToString("dd-MM-yyyy") + " <br/> Work Order Number " + qcprodmodeldata.QcItem[0].WorkOrder + " -  <br />  " +
                           "Base on Bill Number " + qcprodmodeldata.QcItem[0].InvoicesNumber + " - <br /> " +
                           "and QC Department Has given below details:<br /> <br />  QC Approved - "
                         + qcprodmodeldata.QcItem[0].totalpass + "." + UoM + " <br />  QC Not Approve/Hold - " + qcprodmodeldata.QcItem[0].holdingqty + "." + UoM + " <br />  QC Rejected - " + qcprodmodeldata.QcItem[0].rejectqty + "." + UoM + " <br />  QC Rework - " + qcprodmodeldata.QcItem[0].reworkqty + "." + UoM + " <br /> <br /> <br /> <br />  With Regards <br /> V-WEAVE <br /> QC Department ";
                        //mm.CC.Add(bcc);
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential("erp@v-weave.com", "tfgvaoktfoascutg");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                        return Json("mail send", JsonRequestBehavior.AllowGet);
                    }

                }
            }

            catch (Exception ex)
            {
                return Json("Error in mail".ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Create save and mail function by wamique shaikh 26-09-2022

        [HttpPost]
        public JsonResult SaveQc_DayengData(QcProdmodeldata qcprodmodeldata, QcItemtableModel qcitem, string Email)
        {
            try
            {
                string responsemsg = string.Empty;
                SapDataPunch sapDataPunch = new SapDataPunch();
                responsemsg = "";
                responsemsg = sapDataPunch.Save_QC_DAYENG_Data(qcprodmodeldata, qcitem, Email);               
                return Json(responsemsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SentMails(QcProdmodeldata qcprodmodeldata, QcItemtableModel qcitem, string Email)      //string VendorName, string Email   ,  string invcnum               
        {
            try
            {

                string Comand = "select top 1 \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_OBSH\"  ORDER BY \"DocEntry\" DESC";
                DataTable DT = Sqlhana.GetHanaDataSQL(Comand);
                string DocEntry = DT.Rows[0]["DocEntry"].ToString();

                string Comand2 = "select \"SalUnitMsr\" from" + "\"" + SCHEMA + "\"" + ".\"OITM\" where \"ItemCode\" = '" + qcprodmodeldata.QcItem[0].ItemCode + "'";
                DataTable DT2 = Sqlhana.GetHanaDataSQL(Comand2);
                string UoM = DT2.Rows[0]["SalUnitMsr"].ToString();

                var a = qcprodmodeldata.QcItem[0].rejectqty;
                var b = qcprodmodeldata.QcItem[0].SampleQuantity;
                var c = qcprodmodeldata.QcItem[0].totalpass;
                var d = qcprodmodeldata.QcItem[0].ItemCode;
                var e = qcprodmodeldata.QcItem[0].NumAtCard;
                var f = qcprodmodeldata.QcItem[0].holdingqty;
                var g = qcprodmodeldata.QcItem[0].Quantity;
                var h = qcprodmodeldata.QcItem[0].CardName;
                var ab = DocEntry;
                //MailAddress bcc = new MailAddress("Riahabh.katoch@itssquad.com");
                using (MailMessage mm = new MailMessage("erp@v-weave.com", Email))
                {
                    mm.Subject = "" + h + " QC No. " + DocEntry;
                    mm.Body = "Dear " + h + ", <br /> We have received your Material " + qcprodmodeldata.QcItem[0].ItemCode + " - " + qcprodmodeldata.QcItem[0].Quantity + "." + UoM + " <br /> Description Name  - " + qcprodmodeldata.QcItem[0].Dscription + "  <br/> Manual Date " + qcprodmodeldata.QcItem[0].ManualDate + " <br/> Work Order Number " + qcprodmodeldata.QcItem[0].WorkOrder + " -  <br />  " +
                       "Base on Bill Number " + qcprodmodeldata.QcItem[0].InvoicesNumber + " - <br /> " +
                       "and QC Department Has given below details:<br /> <br />  QC Approved - "
                     + qcprodmodeldata.QcItem[0].totalpass + "." + UoM + " <br />  QC Not Approve/Hold - " + qcprodmodeldata.QcItem[0].holdingqty + "." + UoM + " <br />  QC Rejected - " + qcprodmodeldata.QcItem[0].rejectqty + "." + UoM + " <br />  QC Rework - " + qcprodmodeldata.QcItem[0].reworkqty + "." + UoM + " <br /> <br /> <br /> <br />  With Regards <br /> V-WEAVE <br /> QC Department ";
                    //mm.CC.Add(bcc);
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("erp@v-weave.com", "tfgvaoktfoascutg");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                    return Json("mail send", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("error in mail".ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion save fuction end by wamique shaikh 26-09-2022
        public ActionResult CreateRepoert(string From, string To, string ItemGroup, int Branch, string Bin)
        {
            try
            {
                if (ItemGroup == "undefined")
                    ItemGroup = null;
                if (Bin == "undefined")
                    Bin = null;
                DateTime FromDate, ToDate;
                DateTime.TryParseExact(From, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out FromDate);
                DateTime.TryParseExact(To, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ToDate);
                HanaConnection hana = new HanaConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Hana"].ConnectionString);
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                hana.Close();
                string test = SCHEMA + ".SITSPL_QCINWARD";
                HanaCommand hanaCommand = new HanaCommand(test, hana);
                hanaCommand.CommandType = CommandType.StoredProcedure;
                hanaCommand.Parameters.AddWithValue("@FromDate", HanaDbType.Integer).Value = FromDate;
                hanaCommand.Parameters.AddWithValue("@ToDate", HanaDbType.NVarChar).Value = ToDate;
                hanaCommand.Parameters.AddWithValue("@ItemGroup", HanaDbType.NVarChar).Value = ItemGroup;
                hanaCommand.Parameters.AddWithValue("@Branch", HanaDbType.NVarChar).Value = Branch;
                hanaCommand.Parameters.AddWithValue("@Bin", HanaDbType.NVarChar).Value = Bin;
                HanaDataAdapter da = new HanaDataAdapter(hanaCommand);
                DataTable Table = new DataTable();
                da.Fill(Table);
                var path = ConfigurationManager.AppSettings["OrderPathExcel"];
                string folderPath = path;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                //Codes for the Closed XML             
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(Table, "Customer");
                    wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                    string myName = Server.UrlEncode("Test" + "_" + DateTime.Now.ToShortDateString() + ".xlsx");
                    MemoryStream stream = GetStream(wb);
                    // The method is defined below             
                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=" + myName);
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();

                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }

    }

}
   