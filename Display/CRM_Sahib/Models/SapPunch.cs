using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CRM_Sahib.Models
{
    /*============================================================
     Created By     : Dilshad A
     Created On     : 08 Mar 2021
     Description    : Punch Data into sap...
      ============================================================*/
    public class SapPunch
    {
        Company company;
        Documents doc;
        BusinessPartners bp;
        SapConnection connection;
        SalesOpportunities sales;
        SahibDataContext db = null;

        public void PunchBusinessPartner(BusinessPartnerList BusinessList, ref string errmsg, ref int errcode)
        {            
            connection = new SapConnection();
            company = connection.ConnectCompany(ref errcode, ref errmsg);            
            
            if (errcode == 0)
            {
                bp = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                if (BusinessList != null)
                {
                    if (BusinessList.Type == 'C')
                    {
                        bp.CardType = BoCardTypes.cCustomer;
                    }
                    if (BusinessList.Type == 'S')
                    {
                        bp.CardType = BoCardTypes.cSupplier;
                    }
                }

                bp.CardCode = BusinessList.CustomerCode;
                bp.Series = BusinessList.Series;
                bp.CardName = BusinessList.CustomerName;
                bp.GroupCode = BusinessList.Group;
                bp.Currency = BusinessList.Currency;
                bp.Phone1 = BusinessList.PhoneNum;
                bp.EmailAddress = BusinessList.Email;
                bp.Password = BusinessList.Password;
                bp.Website = BusinessList.Website == null ? "" : BusinessList.Website;
                bp.PayTermsGrpCode = -1;

                #region User Details on 06 Apr 2021
                _USERLOGIN user = new _USERLOGIN();
                user.Name = BusinessList.CustomerName;
                user.U_Name = BusinessList.CustomerName;
                user.U_UserID = BusinessList.CustomerCode;
                user.U_UserType = "Customer";
                user.U_UserPassword = BusinessList.Password;
                user.U_UserPhone = BusinessList.PhoneNum;
                user.U_UserEmail = BusinessList.Email;
                #endregion END User Details

                //foreach (var x in BusinessList.ContactPerson)
                int Count = BusinessList.ContactPerson.Count;
                for (int i = 0; i < Count; i++)
                {
                    bp.ContactEmployees.SetCurrentLine(i);
                    if (BusinessList.ContactPerson[i].gender == "male")
                    {
                        bp.ContactEmployees.Gender = BoGenderTypes.gt_Male;
                    }

                    if (BusinessList.ContactPerson[i].gender == "female")
                    {
                        bp.ContactEmployees.Gender = BoGenderTypes.gt_Female;
                    }

                    if (BusinessList.ContactPerson[i].gender == "others")
                    {
                        bp.ContactEmployees.Gender = BoGenderTypes.gt_Undefined;
                    }

                    bp.ContactEmployees.Name = BusinessList.ContactPerson[i].ContactID;
                    bp.ContactEmployees.FirstName = BusinessList.ContactPerson[i].FirstName;
                    bp.ContactEmployees.MiddleName = BusinessList.ContactPerson[i].MiddleName == null ? "" : BusinessList.ContactPerson[i].MiddleName;
                    bp.ContactEmployees.LastName = BusinessList.ContactPerson[i].LastName;
                    bp.ContactEmployees.Title = BusinessList.ContactPerson[i].Tittle == null ? "" : BusinessList.ContactPerson[i].Tittle;
                    bp.ContactEmployees.Position = BusinessList.ContactPerson[i].Position == null ? "" : BusinessList.ContactPerson[i].Position;
                    bp.ContactEmployees.Address = BusinessList.ContactPerson[i].Address == null ? "" : BusinessList.ContactPerson[i].Address;
                    bp.ContactEmployees.MobilePhone = BusinessList.ContactPerson[i].mobile;
                    bp.ContactEmployees.E_Mail = BusinessList.ContactPerson[i].Email;
                    //bp.ContactEmployees.DateOfBirth = x.dob;                    
                    bp.ContactEmployees.Add();
                }

                Count = BusinessList.BillToAdd.Count;
                //foreach (var y in BusinessList.BillToAdd)
                for (int i = 0; i < Count; i++)
                {
                    bp.Addresses.SetCurrentLine(i);
                    bp.Addresses.AddressType = BoAddressType.bo_BillTo;
                    bp.Addresses.AddressName = BusinessList.BillToAdd[i].AddressID;
                    bp.Addresses.AddressName2 = BusinessList.BillToAdd[i].Address1 == null ? "" : BusinessList.BillToAdd[i].Address1;
                    bp.Addresses.AddressName3 = BusinessList.BillToAdd[i].Address2 ?? "";
                    bp.Addresses.Street = BusinessList.BillToAdd[i].StreetPoBox ?? "";
                    bp.Addresses.Block = BusinessList.BillToAdd[i].Block ?? "";
                    bp.Addresses.City = BusinessList.BillToAdd[i].City;
                    bp.Addresses.Country = BusinessList.BillToAdd[i].Country;
                    bp.Addresses.State = BusinessList.BillToAdd[i].State;
                    bp.Addresses.StreetNo = BusinessList.BillToAdd[i].StreetNo ?? "";
                    bp.Addresses.BuildingFloorRoom = BusinessList.BillToAdd[i].Building_Floor_Room ?? "";
                    bp.Addresses.Add();
                }

                //foreach (var y in BusinessList.ShipToAdd)
                Count = BusinessList.ShipToAdd.Count;
                for (int i = 0; i < Count; i++)
                {
                    bp.Addresses.SetCurrentLine(i);
                    bp.Addresses.AddressType = BoAddressType.bo_ShipTo;
                    bp.Addresses.AddressName = BusinessList.ShipToAdd[i].AddressID;
                    bp.Addresses.AddressName2 = BusinessList.ShipToAdd[i].Address1 == null ? "" : BusinessList.ShipToAdd[i].Address1;
                    bp.Addresses.AddressName3 = BusinessList.ShipToAdd[i].Address2 ?? "";
                    bp.Addresses.Street = BusinessList.ShipToAdd[i].StreetPoBox ?? "";
                    bp.Addresses.Block = BusinessList.ShipToAdd[i].Block ?? "";
                    bp.Addresses.City = BusinessList.ShipToAdd[i].City;
                    bp.Addresses.Country = BusinessList.ShipToAdd[i].Country;
                    bp.Addresses.State = BusinessList.ShipToAdd[i].State;
                    bp.Addresses.StreetNo = BusinessList.ShipToAdd[i].StreetNo ?? "";
                    bp.Addresses.BuildingFloorRoom = BusinessList.ShipToAdd[i].Building_Floor_Room ?? "";
                    bp.Addresses.Add();
                }

                errcode = bp.Add();

                if (errcode != 0)
                {                    
                    company.GetLastError(out errcode, out errmsg);
                    errmsg = "Error from SAP: " + errmsg;
                }
                else
                {
                    AddUser(user);
                    errmsg = company.GetNewObjectKey();
                }

            }
        }

        #region Add Customer BY Dilshad A. on 06 Apr 2021
        public void AddUser(_USERLOGIN user)
        {
            try
            {
                db = new SahibDataContext();
                if(user != null)
                {
                    var code = db._USERLOGINs.OrderByDescending(x=>x.Code).Select(x=>x.Code).FirstOrDefault();
                    if (!string.IsNullOrEmpty(code))
                    {
                        user.Code = (Convert.ToInt32(code) + 1).ToString();                        
                    }
                    else
                    {
                        user.Code = "1";
                    }
                    db._USERLOGINs.InsertOnSubmit(user);
                    db.SubmitChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Add Customer BY Dilshad A. on 06 Apr 2021

        public void PunchOpp(OpportunityList OppList, ref int errcode, ref string errmsg)
        {

            connection = new SapConnection();
            company = connection.ConnectCompany(ref errcode, ref errmsg);
            if (errcode == 0)
            {
                sales = company.GetBusinessObject(BoObjectTypes.oSalesOpportunities);

                sales.OpportunityType = OppList.OppType == "S" ? OpportunityTypeEnum.boOpPurchasing : OpportunityTypeEnum.boOpSales;
                sales.CardCode = OppList.CardCode;
                sales.OpportunityName = OppList.OppName ?? "";
                sales.SalesPerson = OppList.SalesPersonCode;
                sales.StartDate = OppList.StartDate;
                sales.PredictedClosingDate = OppList.PredictedCloseDate;
                sales.TotalAmountLocal = OppList.PotentialAmount;

                int stgLine = 0;
                if (OppList.Stage != null)
                {
                    foreach (var x in OppList.Stage)
                    {
                        sales.Lines.SetCurrentLine(stgLine);
                        sales.Lines.StartDate = x.StartDate;
                        sales.Lines.ClosingDate = x.ClosingDate;
                        sales.Lines.SalesPerson = x.SalesPersonCode;
                        sales.Lines.SalesPerson = x.SlpCode;
                        sales.Lines.StageKey = x.StageID;
                        sales.Lines.PercentageRate = x.StatgePercentage;
                        sales.Lines.MaxLocalTotal = x.PotentialAmount;
                        // sales.Lines.DocumentType = BoAPARDocumentTypes.bodt_PurchaseQutation;

                        sales.Lines.DocumentType = x.DocType == "Purchase Quotation" ? BoAPARDocumentTypes.bodt_PurchaseQutation : BoAPARDocumentTypes.bodt_Quotation;
                        sales.Lines.DocumentNumber = x.DocEntry;
                        //sales.Lines.UserFields.Fields.Item("U_act1").Value = x.Activity;
                        //sales.Lines.UserFields.Fields.Item("U_ActRemarks1").Value = x.ActivityRemarks ?? "";
                        sales.Lines.Add();
                        stgLine++;
                    }
                }

                errcode = sales.Add();

                if (errcode != 0)
                {
                    company.GetLastError(out errcode, out errmsg);
                    errmsg = "Error from SAP: " + errmsg;
                }
                else
                    errmsg = company.GetNewObjectKey();
            }
            else
            {
                company.GetLastError(out errcode, out errmsg);
                errmsg = "No Connection Made: " + errmsg;
            }
        }

        public void PunchSalesQuotation(SalesQuotationHeader sales, ref int errcode, ref string errmsg)
        {
            connection = new SapConnection();
            company = connection.ConnectCompany(ref errcode, ref errmsg);
            if (errcode == 0)
            {
                doc = company.GetBusinessObject(BoObjectTypes.oQuotations);
                doc.CardCode = sales.CustomerCode;
                doc.DocDate = sales.DocDate;
                doc.RequriedDate = sales.ValidDate;
                doc.BPL_IDAssignedToInvoice = sales.BranchCode;
                doc.Series = sales.series;

                int count = 0;
                foreach (var x in sales.Row)
                {
                    doc.Lines.ItemCode = x.ItemCode;
                    doc.Lines.ItemDescription = x.ItemName;
                    doc.Lines.Quantity = x.Qty;
                    doc.Lines.DiscountPercent = x.Discount;
                    doc.Lines.UnitPrice = x.UnitPrice;
                    doc.Lines.TaxCode = x.Taxcode;
                    doc.Lines.SetCurrentLine(count);
                    doc.Lines.Add();
                    count++;
                }

                errcode = doc.Add();
                if (errcode != 0)
                {
                    company.GetLastError(out errcode, out errmsg);
                    errmsg = "Error from SAP: " + errmsg;
                }
                else
                    errmsg = company.GetNewObjectKey();
            }
            else
            {
                company.GetLastError(out errcode, out errmsg);
                errmsg = "No Connection Made: " + errmsg;
            }
        }

        public void OpportunityUpdate(OpportunityList OppList, ref int errcode, ref string errmsg)
        {
            connection = new SapConnection();
            company = connection.ConnectCompany(ref errcode, ref errmsg);
            if (errcode == 0)
            {
                sales = company.GetBusinessObject(BoObjectTypes.oSalesOpportunities);

                if (sales.GetByKey(OppList.OppNumber))
                {
                    sales.OpportunityType = OppList.OppType == "S" ? OpportunityTypeEnum.boOpPurchasing : OpportunityTypeEnum.boOpSales;
                    sales.CardCode = OppList.CardCode;
                    sales.OpportunityName = OppList.OppName ?? "";
                    sales.SalesPerson = OppList.SalesPersonCode;
                    //sales.StartDate = OppList.StartDate.Date;
                    sales.PredictedClosingDate = OppList.PredictedCloseDate;
                    sales.TotalAmountLocal = OppList.PotentialAmount;

                    if (OppList.PotentialAmount > 0)
                    {
                        sales.Lines.MaxLocalTotal = OppList.PotentialAmount;
                    }


                    sales.ContactPerson = Convert.ToInt32(OppList.ContactPerson);

                    //if (!string.IsNullOrEmpty(OppList.predictedClosingDays))
                    //{
                    //    DateTime.TryParseExact(OppList.predictedClosingDays, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime Preddate);

                    //}
                    sales.PredictedClosingDate = OppList.PredictedCloseDate;

                    //sales.Status = (BoSoOsStatus)Convert.ToInt32(OppList.opportunityStatus);


                    int Count = 0;

                    if (OppList.Stage != null)
                    {
                        int lastactualcount = OppList.Stage.Count();
                        int lastcount = 0;
                        if (lastactualcount > 0)
                        {
                            lastcount = lastactualcount - 1;
                        }
                        foreach (var d in OppList.Stage)
                        {
                            sales.Lines.SetCurrentLine(Count);

                            if (d.StartDate != null)
                            {
                                // DateTime.TryParseExact(d.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);
                                sales.Lines.StartDate = d.StartDate;
                            }
                            if (d.ClosingDate != null)
                            {
                                // DateTime.TryParseExact(d.ClosingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime closeDate);
                                sales.Lines.ClosingDate = d.ClosingDate;
                            }

                            sales.Lines.SalesPerson = d.SalesPersonCode;

                            //if (d.stageStageName != 0)
                            //    sales.Lines.StageKey = d.stageStageName;
                            sales.Lines.StageKey = d.StageID;

                            if (OppList.PotentialAmount > 0)
                            {
                                if (lastcount == Count)
                                {
                                    sales.Lines.MaxLocalTotal = OppList.PotentialAmount;
                                }
                                else
                                {
                                    sales.Lines.MaxLocalTotal = d.PotentialAmount;
                                }
                            }
                            if (d.DocType == "1")
                            {
                                sales.Lines.DocumentType = BoAPARDocumentTypes.bodt_Quotation;
                                sales.Lines.DocumentNumber = d.DocEntry;
                            }
                            else if (d.DocType == "2")
                            {
                                sales.Lines.DocumentType = BoAPARDocumentTypes.bodt_Order;
                                sales.Lines.DocumentNumber = d.DocEntry;
                            }
                            else if (d.DocType == "3")
                            {
                                sales.Lines.DocumentType = BoAPARDocumentTypes.bodt_DeliveryNote;
                                sales.Lines.DocumentNumber = d.DocEntry;
                            }
                            else if (d.DocType == "4")
                            {
                                sales.Lines.DocumentType = BoAPARDocumentTypes.bodt_Invoice;
                                sales.Lines.DocumentNumber = d.DocEntry;
                            }
                            else if (d.DocType == "23")
                            {                                
                                sales.Lines.DocumentNumber = d.DocEntry;
                            }

                            sales.Lines.Add();
                            Count++;
                        }
                    }


                    //=====================================================================
                    ////sales = company.GetBusinessObject(BoObjectTypes.oSalesOpportunities);

                    ////if (sales.GetByKey(OppList.OpprId))
                    ////{
                    ////    sales.OpportunityType = OppList.OppType == "S" ? OpportunityTypeEnum.boOpPurchasing : OpportunityTypeEnum.boOpSales;
                    ////    sales.CardCode = OppList.CardCode;
                    ////    sales.OpportunityName = OppList.OppName ?? "";
                    ////    sales.SalesPerson = OppList.SalesPersonCode;
                    ////    sales.StartDate = OppList.StartDate;
                    ////    sales.PredictedClosingDate = OppList.PredictedCloseDate;
                    ////    sales.TotalAmountLocal = OppList.PotentialAmount;
                    ////    // sales.UserFields.Fields.Item("U_Division").Value = OppList.Division;
                    ////    int stgLine = 0;
                    ////    foreach (var x in OppList.Stage)
                    ////    {
                    ////        if (OppList.OpprId > 0)
                    ////        {
                    ////            sales.Lines.SetCurrentLine(stgLine);
                    ////        }
                    ////        sales.Lines.StartDate = x.StartDate;
                    ////        sales.Lines.ClosingDate = x.ClosingDate;
                    ////        sales.Lines.SalesPerson = x.SalesPersonCode;
                    ////        sales.Lines.StageKey = x.StageID;
                    ////        sales.Lines.PercentageRate = x.StatgePercentage;
                    ////        sales.Lines.MaxLocalTotal = x.PotentialAmount;
                    ////        sales.Lines.DocumentType = x.DocType == "Purchase Quotation" ? BoAPARDocumentTypes.bodt_PurchaseQutation : BoAPARDocumentTypes.bodt_Quotation;
                    ////        sales.Lines.DocumentNumber = x.DocEntry;
                    ////        sales.Lines.UserFields.Fields.Item("U_act1").Value = x.Activity;
                    ////        sales.Lines.UserFields.Fields.Item("U_ActRemarks1").Value = x.ActivityRemarks ?? "";
                    ////        if (OppList.OpprId > 0)
                    ////        {
                    ////            sales.Lines.Add();
                    ////            stgLine++;
                    ////        }
                    ////        else
                    ////        {
                    ////            sales.Lines.Add();
                    ////        }
                    ////    }

                    errcode = sales.Update();

                    if (errcode != 0)
                    {
                        company.GetLastError(out errcode, out errmsg);
                        errmsg = "Error from SAP: " + errmsg;
                    }
                    else
                        errmsg = company.GetNewObjectKey();

                    company = null;
                }
            }
        }

        public void PunchSalesOrder(SalesOrderHeader SO, ref int errcode, ref string errmsg)
        {
            connection = new SapConnection();
            company = connection.ConnectCompany(ref errcode, ref errmsg);
            if (errcode == 0)
            {
                doc = company.GetBusinessObject(BoObjectTypes.oOrders);
                doc.CardCode = SO.CustomerCode;
                doc.DocDate = SO.DocDate;
                doc.DocDueDate = SO.DeliveryDate;
                doc.BPL_IDAssignedToInvoice = SO.BranchCode;
                doc.Series = SO.series;
                int soCount = 0;
                foreach (var x in SO.Row)
                {
                    doc.Lines.ItemCode = x.ItemCode;
                    doc.Lines.ItemDescription = x.ItemName;
                    doc.Lines.Quantity = x.Qty;
                    doc.Lines.DiscountPercent = x.Discount;
                    doc.Lines.UnitPrice = x.UnitPrice;
                    doc.Lines.TaxCode = x.Taxcode;
                    doc.Lines.SetCurrentLine(soCount);
                    doc.Lines.Add();
                }

                errcode = doc.Add();
                if (errcode != 0)
                {
                    company.GetLastError(out errcode, out errmsg);
                    errmsg = "Error from SAP: " + errmsg;
                }
                else
                    errmsg = company.GetNewObjectKey();
            }
            else
            {
                company.GetLastError(out errcode, out errmsg);
                errmsg = "No Connection Made: " + errmsg;
            }

        }
    }
}