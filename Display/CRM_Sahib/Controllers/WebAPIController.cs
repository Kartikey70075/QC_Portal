using CRM_Sahib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CRM_Sahib.Controllers
{
    //[Route("api/[controller]")]
    public class WebAPIController : ApiController
    {
        SahibDataContext con = null;
        SapPunch sapPunch = null;
        string errorMsg = "";
        int errorCode = 0;
        public WebAPIController()
        {
            con = new SahibDataContext();
            sapPunch = new SapPunch();
        }
        [HttpGet]
        [Route("GetName")]
        public IHttpActionResult GetName()
        {
            return Ok("Dilshad");
        }

        #region BusinessPartner 
        [HttpGet]
        public IHttpActionResult GetBusinessPartnerINIT()
        {
            try
            {
                var Series = (from x in con.NNM1s where x.ObjectCode == "2" && x.Locked == 'N' select new { x.SeriesName, x.Series, x.DocSubType, x.NextNumber, x.BeginStr, x.NumSize }).ToList();
                var Group = (from t in con.OCRGs select new { t.GroupName, t.GroupCode }).ToList();
                var Currency = (from z in con.OCRNs select new { z.CurrName, z.CurrCode }).ToList();
                var Country = (from w in con.OCRies select new { w.Code, w.Name }).ToList();
                var State = (from v in con.OCSTs select new { v.Code, v.Name }).ToList();
                var list = new { Series, Group, Currency, Country, State };
                return Ok(list);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion Business Partner

        #region Punch Business 
        [HttpPost]
        public IHttpActionResult PunchBusinessPartner(BusinessPartnerList businessPartnerList)
        {
            try
            {
                //return null;
                sapPunch.PunchBusinessPartner(businessPartnerList, ref errorMsg, ref errorCode);

                if (errorCode == 0)
                    return Ok("Business Partner is Created with Code- " + errorMsg);
                else
                    return Ok(errorMsg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Punch Business Partner 

        //        #region Opporunity
        //        [HttpGet]
        //        public IHttpActionResult OpporunityPageLoad()
        //        {
        //            try
        //            {
        //                var list = new
        //                {
        //                    OCRD = OCRD(),
        //                    OPPNO = con.OOPRs.OrderByDescending(x => x.OpprId).Select(x => x.OpprId).FirstOrDefault() + 1,
        //                    SalesPerson = (from z in con.OSLPs where z.Locked == 'N' select new { z.SlpCode, z.SlpName }).ToList(),
        //                    Stages = (from t in con.OOSTs where t.Canceled == 'N' select new { t.Descript, t.StepId, t.CloPrcnt }).ToList(),
        //                    Activity = (from a in con.UFD1s where a.TableID == "OPR1" select a).ToList(),
        //                    Divsion = con.UFD1s.Where(x => x.TableID == "OOPR" && x.FieldID == 1).Select(x => x).ToList(),
        //                    OCPR = OCPR(),
        //                    //ItemCode = "Sam00" + con._ITEMMASTER_HEADERs.Select(x => x.DocEntry).OrderByDescending(x => x).FirstOrDefault(),
        //                    Division = GetDivision(),

        //                };

        //                return Ok(list);
        //            }
        //            catch (Exception ex)
        //            {
        //                return InternalServerError(ex);
        //            }
        //        }

        //        [HttpPost]
        //        public IHttpActionResult GetDocumentNumber([FromBody] string DocType)
        //        {
        //            var list = DocType == "S" ? (from x in con.OPQTs where x.DocStatus == 'O' select new { x.DocNum, x.DocEntry }).ToList() : (from x in con.OQUTs where x.DocStatus == 'O' select new { x.DocNum, x.DocEntry }).ToList();
        //            return Ok(list);
        //        }
        //        #endregion END Opportunity

        //        #region Punch Oppertunity        
        //        [HttpPost]
        //        public IHttpActionResult PunchOpportunity([FromBody] OpportunityList OppList)
        //        {
        //            try
        //            {
        //                if (OppList.OpprId > 0)
        //                {
        //                    sapPunch.OpportunityUpdate(OppList, ref errorCode, ref errorMsg);
        //                }
        //                else
        //                {
        //                    sapPunch.PunchOpp(OppList, ref errorCode, ref errorMsg);
        //                }
        //                if (errorCode == 0)
        //                {
        //                    string str = PunchDivisionDB(OppList);
        //                    if (str == "OK")
        //                    {
        //                        if (OppList.OpprId > 0)
        //                        {
        //                            return Ok("Opportunity is updated with No. " + errorMsg);
        //                        }
        //                        return Ok("Opportunity is Created with No. " + errorMsg);
        //                    }
        //                    else
        //                        return Ok(str);
        //                }
        //                else
        //                    return Ok(errorMsg);
        //            }
        //            catch (Exception ex)
        //            {
        //                return InternalServerError(ex);
        //            }

        //        }
        //        #endregion END Punch Oppertunity

        //        #region Get Opportunity      
        //        public IHttpActionResult GetOpportunityByCardCode([FromUri] string CardCode)
        //        {
        //            try
        //            {
        //                if (!string.IsNullOrEmpty(CardCode))
        //                {
        //                    var data = (from d in con.OOPRs
        //                                where d.CardCode == CardCode
        //                                select d).ToList();

        //                    return Ok(data);
        //                }
        //                return Ok();
        //            }
        //            catch (Exception ex)
        //            {
        //                return InternalServerError(ex);
        //            }
        //        }
        //        #endregion END Get Opportunity By CardCode 

        //        #region GetOpprStageByOpprId 
        //        public IHttpActionResult GetOpprStageByOpprId([FromUri] int OpprId)
        //        {
        //            try
        //            {
        //                if (OpprId > 0)
        //                {
        //                    var data = (from d in con.OOPRs
        //                                join d1 in con.OPR1s on d.OpprId equals d1.OpprId
        //                                join slp in con.OSLPs on d.SlpCode equals slp.SlpCode
        //                                join stg in con.OOSTs on d.StepLast equals stg.StepId
        //                                where d.OpprId == OpprId
        //                                select new
        //                                {
        //                                    slp.SlpCode,
        //                                    slp.SlpName,
        //                                    stg.StepId,
        //                                    stg.Descript,
        //                                    d,
        //                                    d1
        //                                }).ToList();

        //                    return Ok(data);
        //                }
        //                return Ok();
        //            }
        //            catch (Exception ex)
        //            {
        //                return InternalServerError(ex);
        //            }
        //        }
        //        #endregion

        //        private string PunchDivisionDB(OpportunityList opplist)
        //        {
        //            try
        //            {
        //                _OPPTYPE_HEADER header = new _OPPTYPE_HEADER();
        //                List<_OPPTYPE_ROW> Rowlist = new List<_OPPTYPE_ROW>();
        //                _OPPTYPE_ROW row;
        //                header.DocEntry = con._OPPTYPE_HEADERs.OrderByDescending(x => x.DocEntry).Select(x => x.DocEntry).FirstOrDefault() == 0 ? 1 : con._OPPTYPE_HEADERs.OrderByDescending(x => x.DocEntry).Select(x => x.DocEntry).FirstOrDefault() + 1;
        //                header.DocNum = header.DocEntry;
        //                header.U_Opp_No = opplist.OppNumber;
        //                header.U_BpCode = opplist.CardCode;
        //                header.U_BpName = opplist.CardName;
        //                header.U_OppName = opplist.OppName ?? "";
        //                con._OPPTYPE_HEADERs.InsertOnSubmit(header);
        //                for (int i = 0; i < opplist.Divisionlist.Count; i++)
        //                {
        //                    if (opplist.Divisionlist[i].DocNum == null)
        //                    {
        //                        opplist.Divisionlist[i].DocNum = header.U_Opp_No.ToString() + "-" + opplist.Divisionlist[i].DivisionID + (i + 1);
        //                        for (int j = i + 1; j < opplist.Divisionlist.Count; j++)
        //                        {

        //                            if (opplist.Divisionlist[i].DivisionID == opplist.Divisionlist[j].DivisionID)
        //                            {
        //                                opplist.Divisionlist[j].DocNum = opplist.Divisionlist[i].DocNum;
        //                            }

        //                        }
        //                    }
        //                }
        //                for (int i = 0; i < opplist.Divisionlist.Count; i++)
        //                {
        //                    row = new _OPPTYPE_ROW();
        //                    row.DocEntry = header.DocEntry;
        //                    row.LineId = i;
        //                    row.U_DivisionId = opplist.Divisionlist[i].DivisionID;
        //                    row.U_DivisionName = opplist.Divisionlist[i].DivisionName;
        //                    row.U_SubDivision = opplist.Divisionlist[i].SubDivsion;
        //                    row.U_Category = opplist.Divisionlist[i].Cateogry;
        //                    row.U_ApplicationOrder = opplist.Divisionlist[i].ApplicationOrder;
        //                    row.U_Price = opplist.Divisionlist[i].Price;
        //                    row.U_Matching = opplist.Divisionlist[i].Matching ? 1 : 0;
        //                    row.U_Offered = opplist.Divisionlist[i].Offered ? 1 : 0;
        //                    row.U_DocNum = opplist.Divisionlist[i].DocNum;
        //                    row.U_Remarks = opplist.Divisionlist[i].DivisionRemarks;
        //                    row.U_Status = 0;
        //                    Rowlist.Add(row);
        //                }
        //                con._OPPTYPE_ROWs.InsertAllOnSubmit(Rowlist);
        //                con.SubmitChanges();
        //                return "OK";
        //            }
        //            catch (Exception ex)
        //            {
        //                return ex.ToString();
        //            }
        //        }

        //        #region Sales Quotation

        //        [HttpGet]
        //        public IHttpActionResult SQINIT()
        //        {
        //            try
        //            {
        //                var list = new
        //                {
        //                    OCRD = OCRD(),
        //                    OCPR = OCPR(),
        //                    OQUT = con.OQUTs.OrderByDescending(x => x.DocEntry).Select(x => x.DocNum).FirstOrDefault() == 0 ? 1 : con.OQUTs.OrderByDescending(x => x.DocEntry).Select(x => x.DocNum).FirstOrDefault() + 1,
        //                    OITM = OITM(),
        //                    OSTC = OSTC(),
        //                    OBPL = OBPL(),
        //                    //NoChargeDocNum = NochargeSheetDocNum(),
        //                    Series = con.NNM1s.Where(x => x.ObjectCode == "23").Select(y => y).ToList(),

        //                    OSCN = OSCN()
        //                };
        //                return Ok(list);
        //            }
        //            catch (Exception ex)
        //            {
        //                return InternalServerError(ex);
        //            }
        //        }

        //        [HttpPost]
        //        public IHttpActionResult PunchSalesQuotation([FromBody] SalesQuotationHeader Sales)
        //        {
        //            try
        //            {
        //                sapPunch.PunchSalesQuotation(Sales, ref errorCode, ref errorMsg);
        //                if (errorCode == 0)
        //                {
        //                    return Ok("Sales Quotations Created With DocNum- " + errorMsg);
        //                }
        //                else
        //                    return Ok(errorMsg);
        //            }
        //            catch (Exception ex)
        //            {
        //                return InternalServerError(ex);
        //            }
        //        }
        //        #endregion

        #region Sale Order

        [HttpGet]
        public IHttpActionResult SOINIT()
        {
            try
            {
                var list = new
                {
                    OCRD = OCRD(),
                    OCPR = OCPR(),
                    ORDR = con.ORDRs.OrderByDescending(x => x.DocEntry).Select(x => x.DocNum).FirstOrDefault() == 0 ? 1 : con.ORDRs.OrderByDescending(x => x.DocEntry).Select(x => x.DocNum).FirstOrDefault() + 1,
                    OITM = OITM(),
                    OSTC = OSTC(),
                    OBPL = OBPL(),
                    Series = con.NNM1s.Where(x => x.ObjectCode == "17").Select(y => y).ToList(),
                    OSCN = OSCN()
                };
                return Ok(list);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]

        public IHttpActionResult PunchSalesOrder([FromBody]SalesOrderHeader SO)
        {
            try
            {
                sapPunch.PunchSalesOrder(SO, ref errorCode, ref errorMsg);
                if (errorCode == 0)
                {
                    return Ok("Sales Order Created With DocNum- " + errorMsg);
                }
                else
                    return Ok(errorMsg);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
        #region Get Catalogue Num(SubCatNum) 
        public IHttpActionResult GetSubCatNumByItemCode(string ItemCode)
        {
            try
            {
                var data = (from d in con.OSCNs where d.ItemCode == ItemCode select d.Substitute).SingleOrDefault();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Get Catalogue Num(SubCatNum) 

        //        //=======================================AR Invoice .=====================================
        //        #region AR Invoice
        //        [HttpGet]
        //        public IHttpActionResult ARInvoiceINIT()
        //        {
        //            try
        //            {
        //                var list = new
        //                {
        //                    OCRD = OCRD(),
        //                    OCPR = OCPR(),
        //                    ORDR = con.ORDRs.OrderByDescending(x => x.DocEntry).Select(x => x.DocNum).FirstOrDefault() == 0 ? 1 : con.ORDRs.OrderByDescending(x => x.DocEntry).Select(x => x.DocNum).FirstOrDefault() + 1,
        //                    OITM = OITM(),
        //                    OSTC = OSTC(),
        //                    OBPL = OBPL(),
        //                    Series = con.NNM1s.Where(x => x.ObjectCode == "13" && x.Indicator == "2020-21" && x.DocSubType == "GA").
        //                    Select(y => y).Distinct().ToList(),
        //                    DocNum = con.OINVs.OrderByDescending(x => x.DocEntry).Select(x => x.DocNum).FirstOrDefault() == 0 ? 1 : con.OINVs.OrderByDescending(x => x.DocEntry).Select(x => x.DocNum).FirstOrDefault() + 1,
        //                    OSCN = OSCN(),
        //                    OSAC= OSAC()

        //                };
        //                return Ok(list);
        //            }
        //            catch (Exception ex)
        //            {
        //                return InternalServerError(ex);
        //            }
        //        }

        //        #endregion END AR Invoice

        #region Common Function
        //private object NochargeSheetDocNum()
        //{
        //    return con._NOCHARGESHEET_HEADs.Select(x => x.DocEntry).FirstOrDefault() == 0 ? 1 : con._NOCHARGESHEET_HEADs.Select(x => x.DocEntry).OrderByDescending(x => x).FirstOrDefault() + 1;
        //}
        private object OCRD()
        {
            return con.OCRDs.Select(x => x).ToList();
        }

        private object OCPR()
        {
            return con.OCPRs.Select(x => x).ToList();
        }

        private object OITM()
        {
            var list = con.OITMs.Select(x => x).ToList();
            return list;
        }

        private object OSTC()
        {
            return con.OSTCs.Select(x => x).ToList();
        }

        private object OBPL()
        {

            return con.OBPLs.Where(x => x.BPLId != 2).Select(x => x).ToList();
        }

        private object OSCN()
        {
            var list = (from t0 in con.OSCNs
                        join t1 in con.OITMs on t0.ItemCode equals t1.ItemCode
                        join t2 in con.OCRDs on t0.CardCode equals t2.CardCode
                        select new
                        {
                            ItemCode = t0.ItemCode,
                            ItemName = t1.ItemName,
                            CardCode = t0.CardCode,
                            CardName = t2.CardName,
                            CatNo = t0.Substitute

                        }).ToList();
            return list;
        }


        //private object OSAC()
        //{
        //    return con.OSACs.ToList();
        //}

//private object GetDivision()
//{
//    var list = (from t0 in con._DIVISION_HEADERs
//                join t1 in con._DIVISION_ROWs on t0.DocEntry equals t1.DocEntry
//                join t2 in con.UFD1s on t0.U_Division equals t2.FldValue
//                where t2.TableID == "@Division_Header"
//                select new
//                {
//                    t0.U_Division,
//                    t2.Descr,
//                    t1.U_SubDivision,
//                    t1.U_Category,
//                    t1.U_ApplicationOrder

//                }).ToList();

//    return list;
//}

#endregion END Common Function

//        //============================================= Customer API ==========================================

//        #region Get Customer Details by User_ID(VendorCode)     
//        public IHttpActionResult GetCustomerDetails()
//        {
//            try
//            {                
//                string CardCode = System.Web.HttpContext.Current.Session["Userid"].ToString();

//                var data = (from d in con.OCRDs where d.CardCode == CardCode select d).FirstOrDefault();

//                return Ok(data);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion END Get Customer Details by User_ID(VendorCode)

//        #region Bind CardCode and CardName based on CardCode
//        [HttpGet]
//        public IHttpActionResult GetBPByCardCode()
//        {
//            try
//            {

//                string UserType = System.Web.HttpContext.Current.Session["UserType"].ToString();
//                if(UserType == "Customer")
//                {
//                    string CardCode = System.Web.HttpContext.Current.Session["Userid"].ToString();
//                    var data = (from d in con.OCRDs
//                                where d.CardCode == CardCode
//                                select new
//                                {
//                                    d.CardCode,
//                                    d.CardName
//                                }).SingleOrDefault();
//                    return Ok(data);
//                }

//                return Ok();
//            }
//            catch (Exception   ex)
//            {
//                return InternalServerError(ex);
//            }
//        }
//        #endregion END Bind CardCode and CardName based on CardCode 
   }
}
