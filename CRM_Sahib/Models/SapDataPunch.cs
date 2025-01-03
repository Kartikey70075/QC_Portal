using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAPbobsCOM;
using Sap.Data.Hana;
using V_Weave_Qc.Controllers;
using System.Data;
using System.Runtime.ConstrainedExecution;


namespace V_Weave_Qc.Models
{
    public class SapDataPunch
    {
        Home1Controller hc = new Home1Controller();
        int ErrorCode = 0, count;
        string msg = "";
        StockTransfer _stocktransfer;
        Company _company = null;
        SapConnection connection = null;
        Documents document;



        HanaSQL Sqlhana = new HanaSQL();
        static HanaConnection con = new HanaConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Hana"].ConnectionString);
        public static string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];

        #region inventry transfer by wamique shaikh 23_09_2022
        public string Save_QC_Data(Qcmodeldata qcmodeldata, QcItemtableModel qcitem, string Email)
        {
            try
            {
                Home1Controller home1 = new Home1Controller();
                string responsemsg = string.Empty;
                connection = new SapConnection();
                _company = connection.connecttocompany(ref ErrorCode, ref msg);
                if (ErrorCode == 0)
                {

                    responsemsg = inventtransfer(qcmodeldata, ref ErrorCode, ref msg);
                    //var mail = home1.SentMail(Email, qcmodeldata, qcitem);
                    //if (mail.Data.ToString() != "mail send")
                    //{
                    //    return "mail not send";
                    //}
                }
                else
                {
                    responsemsg = "Can Not Connect To Company : " + msg;
                }
                return responsemsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string inventtransfer(Qcmodeldata qcmodeldata, ref int ErrorCode, ref string msg)
        {
            string TranferDone = string.Empty;
            string passtransfermsg = string.Empty;
            string scraptransfermsg = string.Empty;
            string shortagetransfermsg = string.Empty;
            string holdingtransfermsg = string.Empty;
            string extratransfermsg = string.Empty;
            string reworktransfermsg = string.Empty;
            string rejecttransfermsg = string.Empty;
            string arinvoiceforreworkqty = string.Empty;
            string observationsavemsg = string.Empty;
            string invenmsgreply = string.Empty;
            try
            {
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                _stocktransfer.Series = 27;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcmodeldata.QcItem[0].CardCode;
                _stocktransfer.FromWarehouse = qcmodeldata.QcItem[0].FromWhsCode;
                _stocktransfer.ToWarehouse = qcmodeldata.QcItem[0].QC_PassWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcmodeldata.QcItem[0].DocEntry;
                //invenmsgreply = "OK";
                for (int i = 0; i < qcmodeldata.QcItem.Count; i++)
                {
                    #region Seprate Tranfesr
                    //if (qcmodeldata.QcItem[i].totalpass > 0)
                    //{
                    //    passtransfermsg = PassTransfer(qcmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    //    if (passtransfermsg != "OK" && passtransfermsg != string.Empty)
                    //    {
                    //        invenmsgreply = observationsavemsg;
                    //        //return invenmsgreply;
                    //    }
                    //}

                    //if (qcmodeldata.QcItem[i].shortageqty > 0)
                    //{
                    //    shortagetransfermsg = ShortageTransfer(qcmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    //    if (shortagetransfermsg != "OK" && shortagetransfermsg != string.Empty)
                    //    {
                    //        invenmsgreply = shortagetransfermsg;
                    //        //return invenmsgreply;
                    //    }
                    //}
                    //if (qcmodeldata.QcItem[i].holdingqty > 0)
                    //{
                    //    holdingtransfermsg = HoldingTransfer(qcmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    //    if (holdingtransfermsg != "OK" && holdingtransfermsg != string.Empty)
                    //    {
                    //        invenmsgreply = holdingtransfermsg;
                    //        //return invenmsgreply;
                    //    }
                    //}
                    //if (qcmodeldata.QcItem[i].extraqty > 0)
                    //{
                    //    extratransfermsg = ExtraTransfer(qcmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    //    if (extratransfermsg != "OK" && extratransfermsg != string.Empty)
                    //    {
                    //        invenmsgreply = extratransfermsg;
                    //        //return invenmsgreply;
                    //    }
                    //}

                    //if (qcmodeldata.QcItem[i].reworkqty > 0)
                    //{
                    //    reworktransfermsg = ReworkTransfer(qcmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    //    if (reworktransfermsg != "OK" && reworktransfermsg != string.Empty)
                    //    {
                    //        invenmsgreply = reworktransfermsg;
                    //        //return invenmsgreply;
                    //    }
                    //}


                    //if (qcmodeldata.QcItem[i].rejectqty > 0)
                    //{
                    //    rejecttransfermsg = RejectTransfer(qcmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    //    if (rejecttransfermsg != "OK" && rejecttransfermsg != string.Empty)
                    //    {
                    //        invenmsgreply = rejecttransfermsg;
                    //        //return invenmsgreply;
                    //    }

                    //}
                    #endregion

                    #region New Change By Kartik

                    int LineNum = 0;
                    if (qcmodeldata.QcItem[i].shortageqty > 0)
                    {
                        #region Line Data
                        _stocktransfer.Lines.SetCurrentLine(LineNum);
                        _stocktransfer.Lines.ItemCode = qcmodeldata.QcItem[i].ItemCode;
                        _stocktransfer.Lines.FromWarehouseCode = qcmodeldata.QcItem[i].FromWhsCode;
                        _stocktransfer.Lines.WarehouseCode = qcmodeldata.QcItem[i].QC_ShortageWarehouses;
                        _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcmodeldata.QcItem[i].UnitPrice);
                        _stocktransfer.Lines.Quantity = qcmodeldata.QcItem[i].shortageqty;

                        if (qcmodeldata.QcItem[i].ManBtchNum == "Y")
                        {
                            string Sql = "SELECT * FROM " + "\"" + SCHEMA + "\"" + ".\"IBT1\" \r\nWHERE \"BaseEntry\" = '" + qcmodeldata.QcItem[i].DocEntry + "' \r\nAND \"BaseType\" = '20' \r\nAND \"WhsCode\" = '" + qcmodeldata.QcItem[i].FromWhsCode + "' AND \"BaseLinNum\" = '" + qcmodeldata.QcItem[i].LineNum + "'";
                            DataTable DT = Sqlhana.GetHanaDataSQL(Sql);

                            if (DT.Rows.Count > 0)
                            {
                                for (int batchIndex = 0; batchIndex < DT.Rows.Count; batchIndex++)
                                {
                                    _stocktransfer.Lines.BatchNumbers.SetCurrentLine(batchIndex);

                                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[batchIndex]["BatchNum"].ToString();
                                    _stocktransfer.Lines.BatchNumbers.Quantity = qcmodeldata.QcItem[i].shortageqty;
                                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = LineNum;

                                    _stocktransfer.Lines.BatchNumbers.Add();
                                }
                            }
                            else
                            {
                                Console.WriteLine("No batch numbers found for this item.");
                            }
                        }

                        _stocktransfer.Lines.Add();
                        LineNum = LineNum + 1;
                        #endregion
                    }

                    if (qcmodeldata.QcItem[i].holdingqty > 0)
                    {
                        #region Line Data
                        _stocktransfer.Lines.SetCurrentLine(LineNum);
                        _stocktransfer.Lines.ItemCode = qcmodeldata.QcItem[i].ItemCode;
                        _stocktransfer.Lines.FromWarehouseCode = qcmodeldata.QcItem[i].FromWhsCode;
                        _stocktransfer.Lines.WarehouseCode = qcmodeldata.QcItem[i].QC_HoldWarehouses;
                        _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcmodeldata.QcItem[i].UnitPrice);
                        _stocktransfer.Lines.Quantity = qcmodeldata.QcItem[i].holdingqty;

                        if (qcmodeldata.QcItem[i].ManBtchNum == "Y")
                        {
                            string Sql = "SELECT * FROM " + "\"" + SCHEMA + "\"" + ".\"IBT1\" \r\nWHERE \"BaseEntry\" = '" + qcmodeldata.QcItem[i].DocEntry + "' \r\nAND \"BaseType\" = '20' \r\nAND \"WhsCode\" = '" + qcmodeldata.QcItem[i].FromWhsCode + "' AND \"BaseLinNum\" = '" + qcmodeldata.QcItem[i].LineNum + "'";
                            DataTable DT = Sqlhana.GetHanaDataSQL(Sql);

                            if (DT.Rows.Count > 0)
                            {
                                for (int batchIndex = 0; batchIndex < DT.Rows.Count; batchIndex++)
                                {
                                    _stocktransfer.Lines.BatchNumbers.SetCurrentLine(batchIndex);
                                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[batchIndex]["BatchNum"].ToString();
                                    _stocktransfer.Lines.BatchNumbers.Quantity = qcmodeldata.QcItem[i].holdingqty;
                                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = LineNum;

                                    _stocktransfer.Lines.BatchNumbers.Add();
                                }
                            }
                            else
                            {

                                Console.WriteLine("No batch numbers found for this item.");
                            }
                        }

                        _stocktransfer.Lines.Add();
                        LineNum = LineNum + 1;
                        #endregion
                    }

                    if (qcmodeldata.QcItem[i].extraqty > 0)
                    {
                        #region Line Data
                        _stocktransfer.Lines.SetCurrentLine(LineNum);
                        _stocktransfer.Lines.ItemCode = qcmodeldata.QcItem[i].ItemCode;
                        _stocktransfer.Lines.FromWarehouseCode = qcmodeldata.QcItem[i].FromWhsCode;
                        _stocktransfer.Lines.WarehouseCode = qcmodeldata.QcItem[i].QC_ExtraWherehouses;
                        _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcmodeldata.QcItem[i].UnitPrice);
                        _stocktransfer.Lines.Quantity = qcmodeldata.QcItem[i].extraqty;

                        if (qcmodeldata.QcItem[i].ManBtchNum == "Y")
                        {
                            string Sql = "SELECT * FROM " + "\"" + SCHEMA + "\"" + ".\"IBT1\" \r\nWHERE \"BaseEntry\" = '" + qcmodeldata.QcItem[i].DocEntry + "' \r\nAND \"BaseType\" = '20' \r\nAND \"WhsCode\" = '" + qcmodeldata.QcItem[i].FromWhsCode + "' AND \"BaseLinNum\" = '" + qcmodeldata.QcItem[i].LineNum + "'";
                            DataTable DT = Sqlhana.GetHanaDataSQL(Sql);

                            if (DT.Rows.Count > 0)
                            {
                                for (int batchIndex = 0; batchIndex < DT.Rows.Count; batchIndex++)
                                {
                                    _stocktransfer.Lines.BatchNumbers.SetCurrentLine(batchIndex);

                                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[batchIndex]["BatchNum"].ToString();
                                    _stocktransfer.Lines.BatchNumbers.Quantity = qcmodeldata.QcItem[i].extraqty;
                                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = LineNum;

                                    _stocktransfer.Lines.BatchNumbers.Add();
                                }
                            }
                            else
                            {

                                Console.WriteLine("No batch numbers found for this item.");
                            }
                        }

                        _stocktransfer.Lines.Add();
                        LineNum = LineNum + 1;
                        #endregion
                    }

                    if (qcmodeldata.QcItem[i].reworkqty > 0)
                    {
                        #region Line Data
                        _stocktransfer.Lines.SetCurrentLine(LineNum);
                        _stocktransfer.Lines.ItemCode = qcmodeldata.QcItem[i].ItemCode;
                        _stocktransfer.Lines.FromWarehouseCode = qcmodeldata.QcItem[i].FromWhsCode;
                        _stocktransfer.Lines.WarehouseCode = qcmodeldata.QcItem[i].QC_ReworkWarehouses;
                        _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcmodeldata.QcItem[i].UnitPrice);
                        _stocktransfer.Lines.Quantity = qcmodeldata.QcItem[i].reworkqty;

                        if (qcmodeldata.QcItem[i].ManBtchNum == "Y")
                        {
                            string Sql = "SELECT * FROM " + "\"" + SCHEMA + "\"" + ".\"OIBT\" \r\nWHERE \"BaseEntry\" = '" + qcmodeldata.QcItem[0].DocEntry + "' \r\nAND \"BaseType\" = '20' \r\nAND \"WhsCode\" = '" + qcmodeldata.QcItem[i].FromWhsCode + "' AND \"BaseLinNum\" = '" + qcmodeldata.QcItem[i].LineNum + "'";
                            DataTable DT = Sqlhana.GetHanaDataSQL(Sql);

                            if (DT.Rows.Count > 0)
                            {
                                for (int batchIndex = 0; batchIndex < DT.Rows.Count; batchIndex++)
                                {
                                    _stocktransfer.Lines.BatchNumbers.SetCurrentLine(batchIndex);

                                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[batchIndex]["BatchNum"].ToString();
                                    _stocktransfer.Lines.BatchNumbers.Quantity = qcmodeldata.QcItem[i].reworkqty;
                                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = LineNum;

                                    _stocktransfer.Lines.BatchNumbers.Add();
                                }
                            }
                            else
                            {

                                Console.WriteLine("No batch numbers found for this item.");
                            }
                        }

                        _stocktransfer.Lines.Add();
                        LineNum = LineNum + 1;
                        #endregion
                    }

                    #region MyNew
                    if (qcmodeldata.QcItem[i].totalpass > 0)
                    {
                        #region Line Data
                        _stocktransfer.Lines.SetCurrentLine(LineNum);
                        _stocktransfer.Lines.ItemCode = qcmodeldata.QcItem[i].ItemCode;
                        _stocktransfer.Lines.FromWarehouseCode = qcmodeldata.QcItem[i].FromWhsCode;
                        _stocktransfer.Lines.WarehouseCode = qcmodeldata.QcItem[i].QC_PassWarehouses;
                        _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcmodeldata.QcItem[i].UnitPrice);
                        _stocktransfer.Lines.Quantity = qcmodeldata.QcItem[i].totalpass;

                        if (qcmodeldata.QcItem[i].ManBtchNum == "Y")
                        {
                            string Sql = "SELECT * FROM " + "\"" + SCHEMA + "\"" + ".\"IBT1\" \r\nWHERE \"BaseEntry\" = '" + qcmodeldata.QcItem[i].DocEntry + "' \r\nAND \"BaseType\" = '20' \r\nAND \"WhsCode\" = '" + qcmodeldata.QcItem[i].FromWhsCode + "' AND \"BaseLinNum\" = '" + qcmodeldata.QcItem[i].LineNum + "'";
                            DataTable DT = Sqlhana.GetHanaDataSQL(Sql);

                            if (DT.Rows.Count > 0)
                            {
                                for (int batchIndex = 0; batchIndex < DT.Rows.Count; batchIndex++)
                                {
                                    _stocktransfer.Lines.BatchNumbers.SetCurrentLine(batchIndex);

                                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[batchIndex]["BatchNum"].ToString();
                                    _stocktransfer.Lines.BatchNumbers.Quantity = qcmodeldata.QcItem[i].totalpass;
                                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = LineNum;

                                    _stocktransfer.Lines.BatchNumbers.Add();
                                }
                            }
                            else
                            {

                                Console.WriteLine("No batch numbers found for this item.");
                            }
                        }

                        _stocktransfer.Lines.Add();
                        LineNum = LineNum + 1;
                        #endregion
                    }

                    if (qcmodeldata.QcItem[i].rejectqty > 0)
                    {
                        #region Line Data
                        _stocktransfer.Lines.SetCurrentLine(LineNum);
                        _stocktransfer.Lines.ItemCode = qcmodeldata.QcItem[i].ItemCode;
                        _stocktransfer.Lines.FromWarehouseCode = qcmodeldata.QcItem[i].FromWhsCode;
                        _stocktransfer.Lines.WarehouseCode = qcmodeldata.QcItem[i].QC_RejectWareHouses;
                        _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcmodeldata.QcItem[i].UnitPrice);
                        _stocktransfer.Lines.Quantity = qcmodeldata.QcItem[i].rejectqty;

                        if (qcmodeldata.QcItem[i].ManBtchNum == "Y")
                        {
                            string Sql = "SELECT * FROM " + "\"" + SCHEMA + "\"" + ".\"IBT1\" \r\nWHERE \"BaseEntry\" = '" + qcmodeldata.QcItem[i].DocEntry + "' \r\nAND \"BaseType\" = '20' \r\nAND \"WhsCode\" = '" + qcmodeldata.QcItem[i].FromWhsCode + "' AND \"BaseLinNum\" = '" + qcmodeldata.QcItem[i].LineNum + "'";
                            DataTable DT = Sqlhana.GetHanaDataSQL(Sql);

                            if (DT.Rows.Count > 0)
                            {
                                for (int batchIndex = 0; batchIndex < DT.Rows.Count; batchIndex++)
                                {
                                    _stocktransfer.Lines.BatchNumbers.SetCurrentLine(batchIndex);
                                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[batchIndex]["BatchNum"].ToString();
                                    _stocktransfer.Lines.BatchNumbers.Quantity = qcmodeldata.QcItem[i].rejectqty;
                                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = LineNum;
                                    _stocktransfer.Lines.BatchNumbers.Add();
                                }
                            }
                            else
                            {
                                Console.WriteLine("No batch numbers found for this rejected item.");
                            }
                        }

                        _stocktransfer.Lines.Add();
                        LineNum = LineNum + 1;
                        #endregion
                    }

                    #endregion
                    #endregion
                    ErrorCode = _stocktransfer.Add();
                    if (ErrorCode != 0)
                    {
                        _company.GetLastError(out ErrorCode, out msg);
                        msg = "Error From SAP:" + msg;
                        return msg;
                    }
                    else
                    {
                        invenmsgreply = "OK";
                    }
                    observationsavemsg = ObsSave(qcmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    if (TranferDone != "OK" && TranferDone != string.Empty)
                    {
                        invenmsgreply = observationsavemsg;
                        //return invenmsgreply;
                    }
                }

                if (observationsavemsg != "OK" || invenmsgreply == string.Empty)
                {
                    invenmsgreply = observationsavemsg;
                    return invenmsgreply;
                }
                return observationsavemsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string PassTransfer(QcItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                _stocktransfer.Series = 27;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_PassWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_PassWarehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.totalpass;
                if (qcItemtableModel.ManBtchNum == "Y")
                {
                    string Sql = "SELECT  a.\"InDate\",A.\"ItemCode\", A.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '20' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".OITW B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where a.\"Quantity\" <> 0 " +
                        "and b.\"WhsCode\" = '" + qcItemtableModel.FromWhsCode + "'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", A.\"ItemCode\", A.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";
                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                    {
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString(); //db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    }
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.totalpass;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }

                _stocktransfer.Lines.Add();
                //
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ShortageTransfer(QcItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                //SQLDataContext db = new SQLDataContext();
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                _stocktransfer.Series = 27;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ShortageWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ShortageWarehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.shortageqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT  a.\"InDate\",A.\"ItemCode\", A.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '20' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".OITW B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where a.\"Quantity\" <> 0 " +
                        "and b.\"WhsCode\" = '" + qcItemtableModel.FromWhsCode + "'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", A.\"ItemCode\", A.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    // _stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.shortageqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                //_stocktransfer.UserFields.Fields.Item("U_GRPODocNum").Value = qcItemtableModel.grpodocnum;
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string HoldingTransfer(QcItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                //SQLDataContext db = new SQLDataContext();
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                _stocktransfer.Series = 27;
                //_stocktransfer.Series = Convert.ToInt32(qcItemtableModel.QC_HoldSeries);
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_HoldWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_HoldWarehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.holdingqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT  a.\"InDate\",A.\"ItemCode\", A.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '20' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".OITW B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where a.\"Quantity\" <> 0 " +
                        "and b.\"WhsCode\" = '" + qcItemtableModel.FromWhsCode + "'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", A.\"ItemCode\", A.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString(); //db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.holdingqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                //_stocktransfer.UserFields.Fields.Item("U_GRPODocNum").Value = qcItemtableModel.grpodocnum;
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ExtraTransfer(QcItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                //SQLDataContext db = new SQLDataContext();
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                _stocktransfer.Series = 27;
                //_stocktransfer.Series = Convert.ToInt32(qcItemtableModel.QC_ExtraQtySeries);
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ExtraWherehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ExtraWherehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.extraqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT  a.\"InDate\",A.\"ItemCode\", A.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '20' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".OITW B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where a.\"Quantity\" <> 0 " +
                        "and b.\"WhsCode\" = '" + qcItemtableModel.FromWhsCode + "' and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", A.\"ItemCode\", A.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString(); //db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.extraqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                //_stocktransfer.UserFields.Fields.Item("U_GRPODocNum").Value = qcItemtableModel.grpodocnum;
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RejectTransfer(QcItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {

                //SQLDataContext db = new SQLDataContext();
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                _stocktransfer.Series = 27;
                //_stocktransfer.Series = Convert.ToInt32(qcItemtableModel.QC_RejectSeries);
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_RejectWareHouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_RejectWareHouses;
                //_stocktransfer.Lines.Quantity = Convert.ToInt32(qcItemtableModel.rejectqty);
                _stocktransfer.Lines.Quantity = qcItemtableModel.rejectqty;
                _stocktransfer.Lines.UnitPrice = qcItemtableModel.reworkunitprice;
                if (qcItemtableModel.ManBtchNum == "Y")
                {
                    string Sql = "SELECT  a.\"InDate\",A.\"ItemCode\", A.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '20' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".OITW B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where a.\"Quantity\" <> 0 " +
                        "and b.\"WhsCode\" = '" + qcItemtableModel.FromWhsCode + "'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", A.\"ItemCode\", A.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    // _stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.rejectqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                //_stocktransfer.UserFields.Fields.Item("U_GRPODocNum").Value = qcItemtableModel.grpodocnum;
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ObsSave(QcItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            try
            {

                SAPbobsCOM.CompanyService oCmpSrv;
                SAPbobsCOM.GeneralService oGeneralService;
                SAPbobsCOM.GeneralData oGeneralData;
                SAPbobsCOM.GeneralData oChild;
                SAPbobsCOM.GeneralDataCollection oChildren;
                SAPbobsCOM.GeneralDataParams oGeneralParams;

                oCmpSrv = _company.GetCompanyService();
                oGeneralService = oCmpSrv.GetGeneralService("QualityCheckSave");

                oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                oChildren = oGeneralData.Child("QC_OBSR");
                string ss = oGeneralData.GetXMLSchema();
                oGeneralData.SetProperty("U_ItemCode", qcItemtableModel.ItemCode);
                oGeneralData.SetProperty("U_ItemName", string.IsNullOrEmpty(qcItemtableModel.ItemDescription) ? "" : qcItemtableModel.ItemDescription);
                oGeneralData.SetProperty("U_BaseDocEntry", qcItemtableModel.DocEntry);
                oGeneralData.SetProperty("U_ReceiptChallanNo", string.IsNullOrEmpty(qcItemtableModel.ReceiptChallanNo) ? "" : qcItemtableModel.ReceiptChallanNo);
                oGeneralData.SetProperty("U_Weight", string.IsNullOrEmpty(qcItemtableModel.Weight) ? "" : qcItemtableModel.Weight);
                oGeneralData.SetProperty("U_InvoiceNo", string.IsNullOrEmpty(qcItemtableModel.BpRefrenceNumber) ? "" : qcItemtableModel.BpRefrenceNumber);
                oGeneralData.SetProperty("U_ItemName", qcItemtableModel.Dscription);
                oGeneralData.SetProperty("U_CardName", qcItemtableModel.CardName);
                oGeneralData.SetProperty("U_PendingQC", string.IsNullOrEmpty(qcItemtableModel.PENDINGQTY) ? "" : qcItemtableModel.PENDINGQTY);
                oGeneralData.SetProperty("U_Bales", string.IsNullOrEmpty(qcItemtableModel.Bales) ? "" : qcItemtableModel.Bales);
                oGeneralData.SetProperty("U_Attachment", string.IsNullOrEmpty(qcItemtableModel.postedFile) ? "" : qcItemtableModel.postedFile);
                oGeneralData.SetProperty("U_Material", string.IsNullOrEmpty(qcItemtableModel.MaterialType) ? "" : qcItemtableModel.MaterialType);
                oGeneralData.SetProperty("U_QC_Process", string.IsNullOrEmpty(qcItemtableModel.ItemGroupType) ? "" : qcItemtableModel.ItemGroupType);
                oGeneralData.SetProperty("U_ItemType", qcItemtableModel.ItmsGrpNam);
                //oGeneralData.SetProperty("U_Bundles", qcItemtableModel.Bundles);
                //oGeneralData.SetProperty("U_BranchName", qcItemtableModel.branchid.ToString());
                //oGeneralData.SetProperty("U_BINLocation", qcItemtableModel.BinLoaction);
                //oGeneralData.SetProperty("U_QcType", qcItemtableModel.QcType);
                oGeneralData.SetProperty("U_SampleQty", qcItemtableModel.SampleQuantity.ToString());
                oGeneralData.SetProperty("U_SamplePassQty", qcItemtableModel.samplepass.ToString());
                oGeneralData.SetProperty("U_SampleRejectQty", qcItemtableModel.samplerej.ToString());
                oGeneralData.SetProperty("U_TotalQty", qcItemtableModel.Quantity.ToString());
                oGeneralData.SetProperty("U_Remarks", string.IsNullOrEmpty(qcItemtableModel.BaseRef) ? "" : qcItemtableModel.BaseRef);
                oGeneralData.SetProperty("U_TotalWeightKg", string.IsNullOrEmpty(qcItemtableModel.TotalWeight) ? "" : qcItemtableModel.TotalWeight);
                oGeneralData.SetProperty("Remark", string.IsNullOrEmpty(qcItemtableModel.remarks) ? "" : qcItemtableModel.remarks);
                //oGeneralData.SetProperty("U_GrpoDocEntry", string.IsNullOrEmpty(qcItemtableModel.DocEntry) ? "" : qcItemtableModel.DocEntry);
                oGeneralData.SetProperty("U_TotalPassQty", qcItemtableModel.totalpass.ToString());
                oGeneralData.SetProperty("U_TotalRejectQty", qcItemtableModel.rejectqty.ToString());
                oGeneralData.SetProperty("U_TotalShortageQty", qcItemtableModel.shortageqty.ToString());
                oGeneralData.SetProperty("U_TotalHoldQty", qcItemtableModel.holdingqty.ToString());
                oGeneralData.SetProperty("U_TotalExtraQty", qcItemtableModel.extraqty.ToString());
                oGeneralData.SetProperty("U_ManualDate", qcItemtableModel.ManualDate.ToString());
                oGeneralData.SetProperty("U_UserName", string.IsNullOrEmpty(qcItemtableModel.UserName) ? "" : qcItemtableModel.UserName);
                oGeneralData.SetProperty("U_Status", "PASS");
                oGeneralData.SetProperty("U_CardCord", qcItemtableModel.CardCode);
                if (qcItemtableModel.Parameters != null)
                {

                    for (int i = 0; i < qcItemtableModel.Parameters.Count; i++)
                    {
                        oChild = oChildren.Add();
                        oChild.SetProperty("U_Description", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Description) ? "" : qcItemtableModel.Parameters[i].Description);
                        oChild.SetProperty("U_Maximum", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Maximum) ? "" : qcItemtableModel.Parameters[i].Maximum);
                        oChild.SetProperty("U_Minimum", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Minimum) ? "" : qcItemtableModel.Parameters[i].Minimum);
                        oChild.SetProperty("U_Instrument", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Instrument) ? "" : qcItemtableModel.Parameters[i].Instrument);
                        oChild.SetProperty("U_Tolerance", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Tolerance) ? "" : qcItemtableModel.Parameters[i].Tolerance);
                        oChild.SetProperty("U_Status", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].U_Status) ? "" : qcItemtableModel.Parameters[i].U_Status);

                        oChild.SetProperty("U_Obs1", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(0) != null ? qcItemtableModel.Parameters[i].ObsArr[0].Observ : "-");
                        oChild.SetProperty("U_Obs2", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(1) != null ? qcItemtableModel.Parameters[i].ObsArr[1].Observ : "-");
                        oChild.SetProperty("U_Obs3", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(2) != null ? qcItemtableModel.Parameters[i].ObsArr[2].Observ : "-");
                        oChild.SetProperty("U_Obs4", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(3) != null ? qcItemtableModel.Parameters[i].ObsArr[3].Observ : "-");
                        oChild.SetProperty("U_Obs5", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(4) != null ? qcItemtableModel.Parameters[i].ObsArr[4].Observ : "-");
                        oChild.SetProperty("U_Obs6", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(5) != null ? qcItemtableModel.Parameters[i].ObsArr[5].Observ : "-");
                        oChild.SetProperty("U_Obs7", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(6) != null ? qcItemtableModel.Parameters[i].ObsArr[6].Observ : "-");
                        oChild.SetProperty("U_Obs8", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(7) != null ? qcItemtableModel.Parameters[i].ObsArr[7].Observ : "-");
                        oChild.SetProperty("U_Obs9", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(8) != null ? qcItemtableModel.Parameters[i].ObsArr[8].Observ : "-");
                        oChild.SetProperty("U_Obs10", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(9) != null ? qcItemtableModel.Parameters[i].ObsArr[9].Observ : "-");
                        oChild.SetProperty("U_Obs11", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(10) != null ? qcItemtableModel.Parameters[i].ObsArr[10].Observ : "-");
                        oChild.SetProperty("U_Obs12", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(11) != null ? qcItemtableModel.Parameters[i].ObsArr[11].Observ : "-");
                        oChild.SetProperty("U_Obs13", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(12) != null ? qcItemtableModel.Parameters[i].ObsArr[12].Observ : "-");
                        oChild.SetProperty("U_Obs14", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(13) != null ? qcItemtableModel.Parameters[i].ObsArr[13].Observ : "-");
                        oChild.SetProperty("U_Obs15", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(14) != null ? qcItemtableModel.Parameters[i].ObsArr[14].Observ : "-");
                        oChild.SetProperty("U_Obs16", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(15) != null ? qcItemtableModel.Parameters[i].ObsArr[15].Observ : "-");
                        oChild.SetProperty("U_Obs17", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(16) != null ? qcItemtableModel.Parameters[i].ObsArr[16].Observ : "-");
                        oChild.SetProperty("U_Obs18", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(17) != null ? qcItemtableModel.Parameters[i].ObsArr[17].Observ : "-");
                        oChild.SetProperty("U_Obs19", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(18) != null ? qcItemtableModel.Parameters[i].ObsArr[18].Observ : "-");
                        oChild.SetProperty("U_Obs20", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(19) != null ? qcItemtableModel.Parameters[i].ObsArr[19].Observ : "-");
                        oChild.SetProperty("U_Obs21", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(20) != null ? qcItemtableModel.Parameters[i].ObsArr[20].Observ : "-");
                        oChild.SetProperty("U_Obs22", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(21) != null ? qcItemtableModel.Parameters[i].ObsArr[21].Observ : "-");
                        oChild.SetProperty("U_Obs23", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(22) != null ? qcItemtableModel.Parameters[i].ObsArr[22].Observ : "-");
                        oChild.SetProperty("U_Obs24", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(23) != null ? qcItemtableModel.Parameters[i].ObsArr[23].Observ : "-");
                        oChild.SetProperty("U_Obs25", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(24) != null ? qcItemtableModel.Parameters[i].ObsArr[24].Observ : "-");
                        oChild.SetProperty("U_Obs26", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(25) != null ? qcItemtableModel.Parameters[i].ObsArr[25].Observ : "-");
                        oChild.SetProperty("U_Obs27", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(26) != null ? qcItemtableModel.Parameters[i].ObsArr[26].Observ : "-");
                        oChild.SetProperty("U_Obs28", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(27) != null ? qcItemtableModel.Parameters[i].ObsArr[27].Observ : "-");
                        oChild.SetProperty("U_Obs29", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(28) != null ? qcItemtableModel.Parameters[i].ObsArr[28].Observ : "-");
                        oChild.SetProperty("U_Obs30", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(29) != null ? qcItemtableModel.Parameters[i].ObsArr[29].Observ : "-");
                        oChild.SetProperty("U_Obs31", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(30) != null ? qcItemtableModel.Parameters[i].ObsArr[30].Observ : "-");
                        oChild.SetProperty("U_Obs32", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(31) != null ? qcItemtableModel.Parameters[i].ObsArr[31].Observ : "-");
                        oChild.SetProperty("U_Obs33", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(32) != null ? qcItemtableModel.Parameters[i].ObsArr[32].Observ : "-");
                        oChild.SetProperty("U_Obs34", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(33) != null ? qcItemtableModel.Parameters[i].ObsArr[33].Observ : "-");
                        oChild.SetProperty("U_Obs35", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(34) != null ? qcItemtableModel.Parameters[i].ObsArr[34].Observ : "-");
                        oChild.SetProperty("U_Obs36", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(35) != null ? qcItemtableModel.Parameters[i].ObsArr[35].Observ : "-");
                        oChild.SetProperty("U_Obs37", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(36) != null ? qcItemtableModel.Parameters[i].ObsArr[36].Observ : "-");
                        oChild.SetProperty("U_Obs38", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(37) != null ? qcItemtableModel.Parameters[i].ObsArr[37].Observ : "-");
                        oChild.SetProperty("U_Obs39", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(38) != null ? qcItemtableModel.Parameters[i].ObsArr[38].Observ : "-");
                        oChild.SetProperty("U_Obs40", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(39) != null ? qcItemtableModel.Parameters[i].ObsArr[39].Observ : "-");
                        oChild.SetProperty("U_Obs41", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(40) != null ? qcItemtableModel.Parameters[i].ObsArr[40].Observ : "-");
                        oChild.SetProperty("U_Obs42", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(41) != null ? qcItemtableModel.Parameters[i].ObsArr[41].Observ : "-");
                        oChild.SetProperty("U_Obs43", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(42) != null ? qcItemtableModel.Parameters[i].ObsArr[42].Observ : "-");
                        oChild.SetProperty("U_Obs44", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(43) != null ? qcItemtableModel.Parameters[i].ObsArr[43].Observ : "-");
                        oChild.SetProperty("U_Obs45", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(44) != null ? qcItemtableModel.Parameters[i].ObsArr[44].Observ : "-");
                        oChild.SetProperty("U_Obs46", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(45) != null ? qcItemtableModel.Parameters[i].ObsArr[45].Observ : "-");
                        oChild.SetProperty("U_Obs47", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(46) != null ? qcItemtableModel.Parameters[i].ObsArr[46].Observ : "-");
                        oChild.SetProperty("U_Obs48", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(47) != null ? qcItemtableModel.Parameters[i].ObsArr[47].Observ : "-");
                        oChild.SetProperty("U_Obs49", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(48) != null ? qcItemtableModel.Parameters[i].ObsArr[48].Observ : "-");
                        oChild.SetProperty("U_Obs50", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(49) != null ? qcItemtableModel.Parameters[i].ObsArr[49].Observ : "-");
                        //listedqcr.Add(oChild);
                    }
                }

                oGeneralService.Add(oGeneralData);
                HanaConnection hana = new HanaConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Hana"].ConnectionString);

                int InvtDocEntry = 0;
                string Comand1 = "SELECT  CAST(\"DocEntry\" AS INT) AS \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".OWTR\r\nORDER BY CAST(\"DocEntry\" AS INT) DESC\r\nLIMIT 1";
                DataTable DT2 = Sqlhana.GetHanaDataSQL(Comand1);
                if (DT2.Rows.Count != 0)
                    InvtDocEntry = Convert.ToInt32(DT2.Rows[0]["DocEntry"]);
                hana.Open();
                HanaCommand update = new HanaCommand(
                 "Update " + "\"" + SCHEMA + "\"" + ".\"PDN1\" set \"U_IsQcDone\" =  " + "'" + "Y" + "',\"U_ITS\" =  " + "'" + "Yes" + "',\"U_ITDoc\" =  " + "'" + InvtDocEntry + "' where \"LineNum\" =" + "'" + qcItemtableModel.LineNum + "'and \"DocEntry\" =" + "'" + qcItemtableModel.DocEntry + "'",
                       hana);
                int recordsAffected = update.ExecuteNonQuery();
                hana.Close();
                string DocEntry = string.Empty;
                string Comand = "select top 1 \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_OBSH\"  ORDER BY \"DocEntry\" DESC";
                DataTable DT = Sqlhana.GetHanaDataSQL(Comand);
                if (DT.Rows.Count != 0)
                    DocEntry = DT.Rows[0]["DocEntry"].ToString();
                string s = "QC Done Data Saved with DocEntry " + DocEntry;
                return s;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string ReworkTransfer(QcItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                //SQLDataContext db = new SQLDataContext();
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ReworkWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ReworkWarehouses;
                //_stocktransfer.Lines.Quantity = Convert.ToInt32(qcItemtableModel.reworkqty);
                _stocktransfer.Lines.UnitPrice = qcItemtableModel.reworkunitprice;
                _stocktransfer.Lines.Quantity = qcItemtableModel.reworkqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT  a.\"InDate\",A.\"ItemCode\", A.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                       " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '20' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                       " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".OITW B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                       "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where a.\"Quantity\" <> 0 " +
                       "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                       " t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", A.\"ItemCode\", A.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = Convert.ToInt32(qcItemtableModel.reworkqty);
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                //_stocktransfer.UserFields.Fields.Item("U_GRPODocNum").Value = qcItemtableModel.grpodocnum;
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion end by wamique shaikh 

        #region Create Production QC Punch Data by wamique shaikh 26-09-2022
        public string Save_QC_PROD_Data(QcProdmodeldata qcprodmodeldata, QcPRODItemtableModel qcItemtableModel, QcItemtableModel qcitem, string Email)
        {

            try
            {
                Home1Controller home1 = new Home1Controller();
                string responsemsg = string.Empty;
                connection = new SapConnection();
                _company = connection.connecttocompany(ref ErrorCode, ref msg);
                if (ErrorCode == 0)
                {
                    responsemsg = inventtransfer_qc_prod(qcprodmodeldata, ref ErrorCode, ref msg);
                    var mail = home1.SentMailProduction(qcprodmodeldata, qcitem, Email);
                    if (mail.Data.ToString() != "mail send")
                    {
                        return "mail not send";
                    }


                }
                else
                {
                    responsemsg = "Can Not Connect To Company : " + msg;
                }
                return responsemsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string inventtransfer_qc_prod(QcProdmodeldata qcprodmodeldata, ref int ErrorCode, ref string msg)
        {
            string passtransfermsg = string.Empty;
            string scraptransfermsg = string.Empty;
            string shortagetransfermsg = string.Empty;
            string holdingtransfermsg = string.Empty;
            string reworktransfermsg = string.Empty;
            string arinvoiceforreworkqty = string.Empty;
            string extratransfermsg = string.Empty;
            string observationsavemsg = string.Empty;
            string invenmsgreply = string.Empty;
            try
            {
                invenmsgreply = "OK";
                for (int i = 0; i < qcprodmodeldata.QcItem.Count; i++)
                {

                    if (qcprodmodeldata.QcItem[i].totalpass > 0)
                    {
                        passtransfermsg = PassTransfer_QC_PROD(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (passtransfermsg != "OK" && passtransfermsg != string.Empty)
                        {
                            invenmsgreply = passtransfermsg;
                            //return invenmsgreply;
                        }
                    }
                    if (qcprodmodeldata.QcItem[i].shortageqty > 0)
                    {
                        shortagetransfermsg = ShortageTransfer_QC_PROD(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (shortagetransfermsg != "OK" && shortagetransfermsg != string.Empty)
                        {
                            invenmsgreply = shortagetransfermsg;
                            //return invenmsgreply;
                        }
                    }
                    if (qcprodmodeldata.QcItem[i].holdingqty > 0)
                    {
                        holdingtransfermsg = HoldingTransfer_QC_PROD(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (holdingtransfermsg != "OK" && holdingtransfermsg != string.Empty)
                        {
                            invenmsgreply = holdingtransfermsg;
                            //return invenmsgreply;
                        }
                    }

                    if (qcprodmodeldata.QcItem[i].extraqty > 0)
                    {
                        extratransfermsg = ExtraTransfer_QC_PROD(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (extratransfermsg != "OK" && extratransfermsg != string.Empty)
                        {
                            invenmsgreply = extratransfermsg;
                            //return invenmsgreply;
                        }
                    }
                    if (qcprodmodeldata.QcItem[i].reworkqty > 0)
                    {
                        reworktransfermsg = ReworkTransfer_QC_PROD(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (reworktransfermsg != "OK" && reworktransfermsg != string.Empty)
                        {
                            invenmsgreply = reworktransfermsg;
                            //return invenmsgreply;
                        }
                    }
                    if (qcprodmodeldata.QcItem[i].rejectqty > 0)
                    {
                        reworktransfermsg = RejectTransfer_QC_PROD(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (reworktransfermsg != "OK" && reworktransfermsg != string.Empty)
                        {
                            invenmsgreply = reworktransfermsg;
                            //return invenmsgreply;
                        }

                    }

                    observationsavemsg = ObsSave_QC_PROD(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    if (observationsavemsg != "OK" || invenmsgreply == string.Empty)
                    {
                        invenmsgreply = observationsavemsg;
                        return invenmsgreply;
                    }
                }

                return observationsavemsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string PassTransfer_QC_PROD(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {

                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_PassWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_PassWarehouses;
                _stocktransfer.Lines.UnitPrice = qcItemtableModel.reworkunitprice;
                _stocktransfer.Lines.Quantity = qcItemtableModel.totalpass;
                if (qcItemtableModel.ManBtchNum == "Y")
                {
                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                       " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                       " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                       "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                       "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                       " a.\"Quantity\" <> 0 " +
                       " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";
                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.IGE1_ItemCode).Select(x =>  x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.totalpass;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }

                _stocktransfer.Lines.Add();
                //
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {

                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ShortageTransfer_QC_PROD(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {

                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ShortageWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ShortageWarehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.shortageqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                       " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                       " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                       "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                       "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                       " a.\"Quantity\" <> 0 " +
                       " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";
                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.shortageqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string HoldingTransfer_QC_PROD(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_HoldWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_HoldWarehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.holdingqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                       " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                       " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                       "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                       "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                       " a.\"Quantity\" <> 0 " +
                       " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";


                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    if (DT.Rows.Count != 0)
                        _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.holdingqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ExtraTransfer_QC_PROD(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                //SQLDataContext db = new SQLDataContext();
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                //_stocktransfer.Series = Convert.ToInt32(qcItemtableModel.QC_ExtraQtySeries);
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ExtraWherehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ExtraWherehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.extraqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                        "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " a.\"Quantity\" <> 0 " +
                        " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString(); //db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.extraqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                //_stocktransfer.UserFields.Fields.Item("U_GRPODocNum").Value = qcItemtableModel.grpodocnum;
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RejectTransfer_QC_PROD(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1009;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_RejectWareHouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_RejectWareHouses;
                _stocktransfer.Lines.Quantity = Convert.ToInt32(qcItemtableModel.rejectqty);
                _stocktransfer.Lines.UnitPrice = qcItemtableModel.reworkunitprice;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                        "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " a.\"Quantity\" <> 0 " +
                        " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = Convert.ToInt32(qcItemtableModel.rejectqty);
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ReworkTransfer_QC_PROD(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {

                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    //_stocktransfer.Series = Convert.ToInt32(qcItemtableModel.QC_ReworkSeries);
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ReworkWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ReworkWarehouses;
                _stocktransfer.Lines.UnitPrice = qcItemtableModel.reworkunitprice;
                _stocktransfer.Lines.Quantity = qcItemtableModel.reworkqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                       " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                       " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                       "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                       "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                       " a.\"Quantity\" <> 0 " +
                       " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    _stocktransfer.Lines.BatchNumbers.Quantity = Convert.ToInt32(qcItemtableModel.reworkqty);
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ObsSave_QC_PROD(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            try
            {
                SAPbobsCOM.CompanyService oCmpSrv;
                SAPbobsCOM.GeneralService oGeneralService;
                SAPbobsCOM.GeneralData oGeneralData;
                SAPbobsCOM.GeneralData oChild;
                SAPbobsCOM.GeneralDataCollection oChildren;
                SAPbobsCOM.GeneralDataParams oGeneralParams;

                oCmpSrv = _company.GetCompanyService();
                oGeneralService = oCmpSrv.GetGeneralService("QualityCheckSave");

                oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                oChildren = oGeneralData.Child("QC_OBSR");
                string ss = oGeneralData.GetXMLSchema();

                oGeneralData.SetProperty("U_ItemCode", qcItemtableModel.ItemCode);
                oGeneralData.SetProperty("U_ItemName", string.IsNullOrEmpty(qcItemtableModel.Dscription) ? "" : qcItemtableModel.Dscription);
                oGeneralData.SetProperty("U_BaseDocEntry", qcItemtableModel.DocEntry);
                oGeneralData.SetProperty("U_ReceiptChallanNo", string.IsNullOrEmpty(qcItemtableModel.ReceiptChallanNo) ? "" : qcItemtableModel.ReceiptChallanNo);
                oGeneralData.SetProperty("U_Bales", string.IsNullOrEmpty(qcItemtableModel.Bales) ? "" : qcItemtableModel.Bales);
                oGeneralData.SetProperty("U_InvoiceNo", string.IsNullOrEmpty(qcItemtableModel.InvoicesNumber) ? "" : qcItemtableModel.InvoicesNumber);
                oGeneralData.SetProperty("U_Weight", string.IsNullOrEmpty(qcItemtableModel.Weight) ? "" : qcItemtableModel.Weight);
                oGeneralData.SetProperty("U_Bundles", string.IsNullOrEmpty(qcItemtableModel.Bundles) ? "" : qcItemtableModel.Bundles);
                oGeneralData.SetProperty("U_SampleQty", qcItemtableModel.SampleQuantity.ToString());
                oGeneralData.SetProperty("U_CardName", qcItemtableModel.CardName);
                oGeneralData.SetProperty("U_BINLocation", string.IsNullOrEmpty(qcItemtableModel.BinLoaction) ? "" : qcItemtableModel.BinLoaction);
                oGeneralData.SetProperty("U_ItemType", string.IsNullOrEmpty(qcItemtableModel.ItmsGrpNam) ? "" : qcItemtableModel.ItmsGrpNam);
                oGeneralData.SetProperty("U_SamplePassQty", qcItemtableModel.samplepass.ToString());
                oGeneralData.SetProperty("U_Material", string.IsNullOrEmpty(qcItemtableModel.MaterialType) ? "" : qcItemtableModel.MaterialType);
                oGeneralData.SetProperty("U_QcType", qcItemtableModel.QcType);
                oGeneralData.SetProperty("U_SampleRejectQty", qcItemtableModel.samplerej.ToString());
                oGeneralData.SetProperty("U_BranchName", qcItemtableModel.branchid.ToString());
                oGeneralData.SetProperty("U_TotalQty", qcItemtableModel.Quantity.ToString());
                oGeneralData.SetProperty("U_TotalPassQty", qcItemtableModel.totalpass.ToString());
                oGeneralData.SetProperty("U_TotalRejectQty", qcItemtableModel.rejectqty.ToString());
                oGeneralData.SetProperty("U_TotalShortageQty", qcItemtableModel.shortageqty.ToString());
                oGeneralData.SetProperty("U_TotalHoldQty", qcItemtableModel.holdingqty.ToString());
                oGeneralData.SetProperty("U_TotalReworkQty", qcItemtableModel.reworkqty.ToString());
                oGeneralData.SetProperty("U_TotalExtraQty", qcItemtableModel.extraqty.ToString());
                oGeneralData.SetProperty("U_Remarks", string.IsNullOrEmpty(qcItemtableModel.WorkOrder) ? "" : qcItemtableModel.WorkOrder);
                oGeneralData.SetProperty("U_TotalWeightKg", string.IsNullOrEmpty(qcItemtableModel.TotalWeight) ? "" : qcItemtableModel.TotalWeight);
                oGeneralData.SetProperty("Remark", string.IsNullOrEmpty(qcItemtableModel.remarks) ? "" : qcItemtableModel.remarks);
                oGeneralData.SetProperty("U_UserName", string.IsNullOrEmpty(qcItemtableModel.UserName) ? "" : qcItemtableModel.UserName);
                oGeneralData.SetProperty("U_QC_Process", qcItemtableModel.QC_Process);
                oGeneralData.SetProperty("U_Status", "Pass");
                oGeneralData.SetProperty("U_ManualDate", qcItemtableModel.ManualDate.ToString());
                oGeneralData.SetProperty("U_CardCord", string.IsNullOrEmpty(qcItemtableModel.CardCode) ? "" : qcItemtableModel.CardCode);



                for (int i = 0; i < qcItemtableModel.Parameters.Count; i++)
                {
                    oChild = oChildren.Add();
                    oChild.SetProperty("U_Description", qcItemtableModel.Parameters[i].Description);
                    oChild.SetProperty("U_Maximum", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Maximum) ? "" : qcItemtableModel.Parameters[i].Maximum);
                    oChild.SetProperty("U_Minimum", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Minimum) ? "" : qcItemtableModel.Parameters[i].Minimum);
                    oChild.SetProperty("U_Instrument", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Instrument) ? "" : qcItemtableModel.Parameters[i].Instrument);
                    oChild.SetProperty("U_Tolerance", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Tolerance) ? "" : qcItemtableModel.Parameters[i].Tolerance);
                    oChild.SetProperty("U_Status", qcItemtableModel.Parameters[i].U_Status);

                    oChild.SetProperty("U_Obs1", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(0) != null ? qcItemtableModel.Parameters[i].ObsArr[0].Observ : "-");
                    oChild.SetProperty("U_Obs2", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(1) != null ? qcItemtableModel.Parameters[i].ObsArr[1].Observ : "-");
                    oChild.SetProperty("U_Obs3", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(2) != null ? qcItemtableModel.Parameters[i].ObsArr[2].Observ : "-");
                    oChild.SetProperty("U_Obs4", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(3) != null ? qcItemtableModel.Parameters[i].ObsArr[3].Observ : "-");
                    oChild.SetProperty("U_Obs5", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(4) != null ? qcItemtableModel.Parameters[i].ObsArr[4].Observ : "-");
                    oChild.SetProperty("U_Obs6", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(5) != null ? qcItemtableModel.Parameters[i].ObsArr[5].Observ : "-");
                    oChild.SetProperty("U_Obs7", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(6) != null ? qcItemtableModel.Parameters[i].ObsArr[6].Observ : "-");
                    oChild.SetProperty("U_Obs8", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(7) != null ? qcItemtableModel.Parameters[i].ObsArr[7].Observ : "-");
                    oChild.SetProperty("U_Obs9", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(8) != null ? qcItemtableModel.Parameters[i].ObsArr[8].Observ : "-");
                    oChild.SetProperty("U_Obs10", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(9) != null ? qcItemtableModel.Parameters[i].ObsArr[9].Observ : "-");
                    oChild.SetProperty("U_Obs11", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(10) != null ? qcItemtableModel.Parameters[i].ObsArr[10].Observ : "-");
                    oChild.SetProperty("U_Obs12", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(11) != null ? qcItemtableModel.Parameters[i].ObsArr[11].Observ : "-");
                    oChild.SetProperty("U_Obs13", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(12) != null ? qcItemtableModel.Parameters[i].ObsArr[12].Observ : "-");
                    oChild.SetProperty("U_Obs14", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(13) != null ? qcItemtableModel.Parameters[i].ObsArr[13].Observ : "-");
                    oChild.SetProperty("U_Obs15", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(14) != null ? qcItemtableModel.Parameters[i].ObsArr[14].Observ : "-");
                    oChild.SetProperty("U_Obs16", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(15) != null ? qcItemtableModel.Parameters[i].ObsArr[15].Observ : "-");
                    oChild.SetProperty("U_Obs17", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(16) != null ? qcItemtableModel.Parameters[i].ObsArr[16].Observ : "-");
                    oChild.SetProperty("U_Obs18", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(17) != null ? qcItemtableModel.Parameters[i].ObsArr[17].Observ : "-");
                    oChild.SetProperty("U_Obs19", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(18) != null ? qcItemtableModel.Parameters[i].ObsArr[18].Observ : "-");
                    oChild.SetProperty("U_Obs20", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(19) != null ? qcItemtableModel.Parameters[i].ObsArr[19].Observ : "-");
                    oChild.SetProperty("U_Obs21", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(20) != null ? qcItemtableModel.Parameters[i].ObsArr[20].Observ : "-");
                    oChild.SetProperty("U_Obs22", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(21) != null ? qcItemtableModel.Parameters[i].ObsArr[21].Observ : "-");
                    oChild.SetProperty("U_Obs23", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(22) != null ? qcItemtableModel.Parameters[i].ObsArr[22].Observ : "-");
                    oChild.SetProperty("U_Obs24", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(23) != null ? qcItemtableModel.Parameters[i].ObsArr[23].Observ : "-");
                    oChild.SetProperty("U_Obs25", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(24) != null ? qcItemtableModel.Parameters[i].ObsArr[24].Observ : "-");
                    oChild.SetProperty("U_Obs26", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(25) != null ? qcItemtableModel.Parameters[i].ObsArr[25].Observ : "-");
                    oChild.SetProperty("U_Obs27", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(26) != null ? qcItemtableModel.Parameters[i].ObsArr[26].Observ : "-");
                    oChild.SetProperty("U_Obs28", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(27) != null ? qcItemtableModel.Parameters[i].ObsArr[27].Observ : "-");
                    oChild.SetProperty("U_Obs29", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(28) != null ? qcItemtableModel.Parameters[i].ObsArr[28].Observ : "-");
                    oChild.SetProperty("U_Obs30", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(29) != null ? qcItemtableModel.Parameters[i].ObsArr[29].Observ : "-");
                    oChild.SetProperty("U_Obs31", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(30) != null ? qcItemtableModel.Parameters[i].ObsArr[30].Observ : "-");
                    oChild.SetProperty("U_Obs32", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(31) != null ? qcItemtableModel.Parameters[i].ObsArr[31].Observ : "-");
                    oChild.SetProperty("U_Obs33", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(32) != null ? qcItemtableModel.Parameters[i].ObsArr[32].Observ : "-");
                    oChild.SetProperty("U_Obs34", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(33) != null ? qcItemtableModel.Parameters[i].ObsArr[33].Observ : "-");
                    oChild.SetProperty("U_Obs35", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(34) != null ? qcItemtableModel.Parameters[i].ObsArr[34].Observ : "-");
                    oChild.SetProperty("U_Obs36", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(35) != null ? qcItemtableModel.Parameters[i].ObsArr[35].Observ : "-");
                    oChild.SetProperty("U_Obs37", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(36) != null ? qcItemtableModel.Parameters[i].ObsArr[36].Observ : "-");
                    oChild.SetProperty("U_Obs38", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(37) != null ? qcItemtableModel.Parameters[i].ObsArr[37].Observ : "-");
                    oChild.SetProperty("U_Obs39", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(38) != null ? qcItemtableModel.Parameters[i].ObsArr[38].Observ : "-");
                    oChild.SetProperty("U_Obs40", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(39) != null ? qcItemtableModel.Parameters[i].ObsArr[39].Observ : "-");
                    oChild.SetProperty("U_Obs41", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(40) != null ? qcItemtableModel.Parameters[i].ObsArr[40].Observ : "-");
                    oChild.SetProperty("U_Obs42", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(41) != null ? qcItemtableModel.Parameters[i].ObsArr[41].Observ : "-");
                    oChild.SetProperty("U_Obs43", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(42) != null ? qcItemtableModel.Parameters[i].ObsArr[42].Observ : "-");
                    oChild.SetProperty("U_Obs44", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(43) != null ? qcItemtableModel.Parameters[i].ObsArr[43].Observ : "-");
                    oChild.SetProperty("U_Obs45", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(44) != null ? qcItemtableModel.Parameters[i].ObsArr[44].Observ : "-");
                    oChild.SetProperty("U_Obs46", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(45) != null ? qcItemtableModel.Parameters[i].ObsArr[45].Observ : "-");
                    oChild.SetProperty("U_Obs47", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(46) != null ? qcItemtableModel.Parameters[i].ObsArr[46].Observ : "-");
                    oChild.SetProperty("U_Obs48", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(47) != null ? qcItemtableModel.Parameters[i].ObsArr[47].Observ : "-");
                    oChild.SetProperty("U_Obs49", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(48) != null ? qcItemtableModel.Parameters[i].ObsArr[48].Observ : "-");
                    oChild.SetProperty("U_Obs50", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(49) != null ? qcItemtableModel.Parameters[i].ObsArr[49].Observ : "-");

                }

                oGeneralService.Add(oGeneralData);
                string Comand = "select top 1 \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_OBSH\"  ORDER BY \"DocEntry\" DESC";
                DataTable DT = Sqlhana.GetHanaDataSQL(Comand);
                string DocEntry = DT.Rows[0]["DocEntry"].ToString();
                string s = "QC Done Production Data Saved with DocEntry " + DocEntry;
                return s;
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }

        #endregion by wamique shaikh 26-09-2022

        #region Dayeng Qc save data function and inventry transfer 26-09-2022 
        public string Save_QC_DAYENG_Data(QcProdmodeldata qcprodmodeldata, QcItemtableModel qcitem, string Email)
        {
            try
            {
                Home1Controller home1 = new Home1Controller();
                string responsemsg = string.Empty;
                connection = new SapConnection();
                _company = connection.connecttocompany(ref ErrorCode, ref msg);
                if (ErrorCode == 0)
                {
                    responsemsg = inventtransfer_QC_DAYENG(qcprodmodeldata, ref ErrorCode, ref msg);
                    var mail = home1.SentMails(qcprodmodeldata, qcitem, Email);
                    if (mail.Data.ToString() != "mail send")
                    {
                        return "mail not send";
                    }
                }
                else
                {
                    responsemsg = "Can Not Connect To Company : " + msg;
                }
                return responsemsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string inventtransfer_QC_DAYENG(QcProdmodeldata qcprodmodeldata, ref int ErrorCode, ref string msg)
        {
            string passtransfermsg = string.Empty;
            string scraptransfermsg = string.Empty;
            string shortagetransfermsg = string.Empty;
            string holdingtransfermsg = string.Empty;
            string reworktransfermsg = string.Empty;
            string arinvoiceforreworkqty = string.Empty;
            string extratransfermsg = string.Empty;
            string observationsavemsg = string.Empty;
            string invenmsgreply = string.Empty;
            try
            {
                invenmsgreply = "OK";
                for (int i = 0; i < qcprodmodeldata.QcItem.Count; i++)
                {

                    if (qcprodmodeldata.QcItem[i].totalpass > 0)
                    {
                        passtransfermsg = PassTransfer_QC_DAYENG(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (passtransfermsg != "OK" && passtransfermsg != string.Empty)
                        {
                            invenmsgreply = passtransfermsg;
                        }
                    }
                    if (qcprodmodeldata.QcItem[i].shortageqty > 0)
                    {
                        shortagetransfermsg = ShortageTransfer_QC_DAYENG(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (shortagetransfermsg != "OK" && shortagetransfermsg != string.Empty)
                        {
                            invenmsgreply = shortagetransfermsg;
                            //return invenmsgreply;
                        }
                    }
                    if (qcprodmodeldata.QcItem[i].holdingqty > 0)
                    {
                        holdingtransfermsg = HoldingTransfer_QC_DAYENG(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (holdingtransfermsg != "OK" && holdingtransfermsg != string.Empty)
                        {
                            invenmsgreply = holdingtransfermsg;
                            //return invenmsgreply;
                        }
                    }

                    if (qcprodmodeldata.QcItem[i].extraqty > 0)
                    {
                        extratransfermsg = ExtraTransfer_QC_DAYENG(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (extratransfermsg != "OK" && extratransfermsg != string.Empty)
                        {
                            invenmsgreply = extratransfermsg;
                            //return invenmsgreply;
                        }
                    }
                    if (qcprodmodeldata.QcItem[i].reworkqty > 0)
                    {
                        reworktransfermsg = ReworkTransfer_QC_DAYENG(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (reworktransfermsg != "OK" && reworktransfermsg != string.Empty)
                        {
                            invenmsgreply = reworktransfermsg;
                            //return invenmsgreply;
                        }
                    }

                    if (qcprodmodeldata.QcItem[i].rejectqty > 0)
                    {
                        reworktransfermsg = RejectTransfer_QC_DAYENG(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                        if (reworktransfermsg != "OK" && reworktransfermsg != string.Empty)
                        {
                            invenmsgreply = reworktransfermsg;
                            //return invenmsgreply;
                        }

                    }
                    observationsavemsg = ObsSave_QC_DAYENG(qcprodmodeldata.QcItem[i], ref ErrorCode, ref msg);
                    if (observationsavemsg != "OK" || invenmsgreply == string.Empty)
                    {
                        invenmsgreply = observationsavemsg;
                        return invenmsgreply;
                    }
                }

                return observationsavemsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string PassTransfer_QC_DAYENG(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {

                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_PassWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_PassWarehouses;
                _stocktransfer.Lines.UnitPrice = qcItemtableModel.reworkunitprice;
                _stocktransfer.Lines.Quantity = qcItemtableModel.totalpass;
                if (qcItemtableModel.ManBtchNum == "Y")
                {


                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                         " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                          "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                            "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                             " a.\"Quantity\" <> 0 " +
                            " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";
                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.IGE1_ItemCode).Select(x =>  x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.totalpass;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }

                _stocktransfer.Lines.Add();
                //
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {

                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ShortageTransfer_QC_DAYENG(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ShortageWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ShortageWarehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.shortageqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {


                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                      "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " a.\"Quantity\" <> 0 " +
                         " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";
                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.shortageqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string HoldingTransfer_QC_DAYENG(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                //SQLDataContext db = new SQLDataContext();
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                _stocktransfer.Series = 1007;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_HoldWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_HoldWarehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.holdingqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {


                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                      "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " a.\"Quantity\" <> 0 " +
                         " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.holdingqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ExtraTransfer_QC_DAYENG(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                //SQLDataContext db = new SQLDataContext();
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                //_stocktransfer.Series = Convert.ToInt32(qcItemtableModel.QC_ExtraQtySeries);
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ExtraWherehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ExtraWherehouses;
                _stocktransfer.Lines.UnitPrice = Convert.ToDouble(qcItemtableModel.UnitPrice);
                _stocktransfer.Lines.Quantity = qcItemtableModel.extraqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                        "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " a.\"Quantity\" <> 0 " +
                        " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";

                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString(); //db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = qcItemtableModel.extraqty;
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                //_stocktransfer.UserFields.Fields.Item("U_GRPODocNum").Value = qcItemtableModel.grpodocnum;
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RejectTransfer_QC_DAYENG(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_RejectWareHouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_RejectWareHouses;
                _stocktransfer.Lines.Quantity = Convert.ToInt32(qcItemtableModel.rejectqty);
                _stocktransfer.Lines.UnitPrice = qcItemtableModel.reworkunitprice;
                if (qcItemtableModel.ManBtchNum == "Y")
                {


                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                        " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                        " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                        "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                      "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                        " a.\"Quantity\" <> 0 " +
                         " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";
                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = Convert.ToInt32(qcItemtableModel.rejectqty);
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    //return msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ReworkTransfer_QC_DAYENG(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            string replymsg = string.Empty;
            try
            {
                _stocktransfer = _company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (qcItemtableModel.QcType == "Production")
                    _stocktransfer.Series = 1007;
                else if (qcItemtableModel.QcType == "Sample")
                    _stocktransfer.Series = 1010;
                _stocktransfer.DocDate = DateTime.Now;
                _stocktransfer.CardCode = qcItemtableModel.CardCode;
                _stocktransfer.FromWarehouse = qcItemtableModel.FromWhsCode;
                _stocktransfer.ToWarehouse = qcItemtableModel.QC_ReworkWarehouses;
                _stocktransfer.UserFields.Fields.Item("U_GRPODOCENTRY").Value = qcItemtableModel.DocEntry;
                _stocktransfer.Lines.SetCurrentLine(0);
                _stocktransfer.Lines.ItemCode = qcItemtableModel.ItemCode;
                _stocktransfer.Lines.FromWarehouseCode = qcItemtableModel.FromWhsCode;
                _stocktransfer.Lines.WarehouseCode = qcItemtableModel.QC_ReworkWarehouses;
                //_stocktransfer.Lines.Quantity = Convert.ToInt32(qcItemtableModel.reworkqty);
                _stocktransfer.Lines.UnitPrice = qcItemtableModel.reworkunitprice;
                _stocktransfer.Lines.Quantity = qcItemtableModel.reworkqty;
                if (qcItemtableModel.ManBtchNum == "Y")
                {

                    string Sql = "SELECT distinct a.\"InDate\",c.\"ItemCode\", c.\"ItemName\", A.\"BatchNum\" \"DistNumber\", a.\"Quantity\", b.\"WhsCode\", t2.\"BaseEntry\"" +
                       " FROM " + "\"" + SCHEMA + "\"" + ".IBT1 T2 INNER JOIN " + "\"" + SCHEMA + "\"" + ".OIBT A on a.\"BatchNum\" = t2.\"BatchNum\" and a.\"ItemCode\" = T2.\"ItemCode\"  and a.\"WhsCode\" =T2.\"WhsCode\" and T2.\"BaseType\" = '59' left join " + "\"" + SCHEMA + "\"" + ".OBTN on OBTN.\"DistNumber\" = A.\"BatchNum\"" +
                       " AND OBTN.\"ItemCode\" = A.\"ItemCode\" INNER JOIN" + "\"" + SCHEMA + "\"" + ".IGN1 B ON A.\"ItemCode\" = B.\"ItemCode\" AND " +
                       "A.\"WhsCode\" = B.\"WhsCode\" INNER JOIN " + "\"" + SCHEMA + "\"" + ".OITM C ON B.\"ItemCode\" = C.\"ItemCode\" where b.\"Quantity\" <> 0 " +
                       "and b.\"WhsCode\" = 'INQCV'  and A.\"ItemCode\" = '" + qcItemtableModel.ItemCode + "'  and" +
                       " a.\"Quantity\" <> 0 " +
                       " and t2.\"BaseEntry\" = '" + qcItemtableModel.DocEntry + "'  group by a.\"InDate\", c.\"ItemCode\", c.\"ItemName\",A.\"BatchNum\", a.\"Quantity\", b.\"WhsCode\", T2.\"CardCode\",t2.\"BaseEntry\"";


                    DataTable DT = Sqlhana.GetHanaDataSQL(Sql);
                    _stocktransfer.Lines.BatchNumbers.BatchNumber = DT.Rows[0]["DistNumber"].ToString();
                    //_stocktransfer.Lines.BatchNumbers.BatchNumber = db.OBTNs.Where(x => x.ItemCode == qcItemtableModel.ItemCode).Select(x => x.DistNumber).FirstOrDefault();
                    _stocktransfer.Lines.BatchNumbers.Quantity = Convert.ToInt32(qcItemtableModel.reworkqty);
                    _stocktransfer.Lines.BatchNumbers.BaseLineNumber = 0;
                }
                _stocktransfer.Lines.Add();
                ErrorCode = _stocktransfer.Add();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out msg);
                    msg = "Error From SAP:" + msg;
                    replymsg = msg;
                }
                else
                {
                    replymsg = "OK";
                }
                return replymsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ObsSave_QC_DAYENG(QcPRODItemtableModel qcItemtableModel, ref int ErrorCode, ref string msg)
        {
            try
            {
                SAPbobsCOM.CompanyService oCmpSrv;
                SAPbobsCOM.GeneralService oGeneralService;
                SAPbobsCOM.GeneralData oGeneralData;
                SAPbobsCOM.GeneralData oChild;
                SAPbobsCOM.GeneralDataCollection oChildren;
                SAPbobsCOM.GeneralDataParams oGeneralParams;

                oCmpSrv = _company.GetCompanyService();
                oGeneralService = oCmpSrv.GetGeneralService("QualityCheckSave");

                oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                oChildren = oGeneralData.Child("QC_OBSR");
                string ss = oGeneralData.GetXMLSchema();

                oGeneralData.SetProperty("U_ItemCode", qcItemtableModel.ItemCode);
                oGeneralData.SetProperty("U_ItemName", string.IsNullOrEmpty(qcItemtableModel.Dscription) ? "" : qcItemtableModel.Dscription);
                oGeneralData.SetProperty("U_BaseDocEntry", qcItemtableModel.DocEntry);
                oGeneralData.SetProperty("U_ReceiptChallanNo", string.IsNullOrEmpty(qcItemtableModel.ReceiptChallanNo) ? "" : qcItemtableModel.ReceiptChallanNo);
                oGeneralData.SetProperty("U_Bales", string.IsNullOrEmpty(qcItemtableModel.Bales) ? "" : qcItemtableModel.Bales);
                oGeneralData.SetProperty("U_InvoiceNo", string.IsNullOrEmpty(qcItemtableModel.InvoicesNumber) ? "" : qcItemtableModel.InvoicesNumber);
                oGeneralData.SetProperty("U_Weight", string.IsNullOrEmpty(qcItemtableModel.Weight) ? "" : qcItemtableModel.Weight);
                oGeneralData.SetProperty("U_Bundles", string.IsNullOrEmpty(qcItemtableModel.Bundles) ? "" : qcItemtableModel.Bundles);
                oGeneralData.SetProperty("U_ItemType", string.IsNullOrEmpty(qcItemtableModel.ItmsGrpNam) ? "" : qcItemtableModel.ItmsGrpNam);
                oGeneralData.SetProperty("U_SampleQty", qcItemtableModel.SampleQuantity.ToString());
                oGeneralData.SetProperty("U_CardName", qcItemtableModel.CardName);
                oGeneralData.SetProperty("U_BINLocation", string.IsNullOrEmpty(qcItemtableModel.BinLoaction) ? "" : qcItemtableModel.BinLoaction);
                oGeneralData.SetProperty("U_Material", string.IsNullOrEmpty(qcItemtableModel.MaterialType) ? "" : qcItemtableModel.MaterialType);
                oGeneralData.SetProperty("U_Attachment", string.IsNullOrEmpty(qcItemtableModel.postedFile) ? "" : qcItemtableModel.postedFile);
                oGeneralData.SetProperty("U_QcType", qcItemtableModel.QcType);
                oGeneralData.SetProperty("U_SamplePassQty", qcItemtableModel.samplepass.ToString());
                oGeneralData.SetProperty("U_SampleRejectQty", qcItemtableModel.samplerej.ToString());
                oGeneralData.SetProperty("U_BranchName", qcItemtableModel.branchid.ToString());
                oGeneralData.SetProperty("U_TotalQty", qcItemtableModel.Quantity.ToString());
                oGeneralData.SetProperty("U_TotalPassQty", qcItemtableModel.totalpass.ToString());
                oGeneralData.SetProperty("U_TotalRejectQty", qcItemtableModel.rejectqty.ToString());
                oGeneralData.SetProperty("U_TotalShortageQty", qcItemtableModel.shortageqty.ToString());
                oGeneralData.SetProperty("U_TotalHoldQty", qcItemtableModel.holdingqty.ToString());
                oGeneralData.SetProperty("U_TotalReworkQty", qcItemtableModel.reworkqty.ToString());
                oGeneralData.SetProperty("U_TotalExtraQty", qcItemtableModel.extraqty.ToString());
                oGeneralData.SetProperty("U_Remarks", string.IsNullOrEmpty(qcItemtableModel.WorkOrder) ? "" : qcItemtableModel.WorkOrder);
                oGeneralData.SetProperty("U_TotalWeightKg", string.IsNullOrEmpty(qcItemtableModel.TotalWeight) ? "" : qcItemtableModel.TotalWeight);
                oGeneralData.SetProperty("Remark", string.IsNullOrEmpty(qcItemtableModel.remarks) ? "" : qcItemtableModel.remarks);
                oGeneralData.SetProperty("U_QC_Process", qcItemtableModel.QC_Process);
                oGeneralData.SetProperty("U_Status", "Pass");
                oGeneralData.SetProperty("U_ManualDate", qcItemtableModel.ManualDate.ToString());
                oGeneralData.SetProperty("U_UserName", string.IsNullOrEmpty(qcItemtableModel.UserName) ? "" : qcItemtableModel.UserName);
                oGeneralData.SetProperty("U_CardCord", string.IsNullOrEmpty(qcItemtableModel.CardCode) ? "" : qcItemtableModel.CardCode);

                for (int i = 0; i < qcItemtableModel.Parameters.Count; i++)
                {
                    oChild = oChildren.Add();
                    oChild.SetProperty("U_Description", qcItemtableModel.Parameters[i].Description);
                    oChild.SetProperty("U_Maximum", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Maximum) ? "" : qcItemtableModel.Parameters[i].Maximum);
                    oChild.SetProperty("U_Minimum", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Minimum) ? "" : qcItemtableModel.Parameters[i].Minimum);
                    oChild.SetProperty("U_Instrument", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Instrument) ? "" : qcItemtableModel.Parameters[i].Instrument);
                    oChild.SetProperty("U_Tolerance", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].Tolerance) ? "" : qcItemtableModel.Parameters[i].Tolerance);
                    oChild.SetProperty("U_Status", string.IsNullOrEmpty(qcItemtableModel.Parameters[i].U_Status) ? "" : qcItemtableModel.Parameters[i].U_Status);

                    oChild.SetProperty("U_Obs1", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(0) != null ? qcItemtableModel.Parameters[i].ObsArr[0].Observ : "-");
                    oChild.SetProperty("U_Obs2", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(1) != null ? qcItemtableModel.Parameters[i].ObsArr[1].Observ : "-");
                    oChild.SetProperty("U_Obs3", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(2) != null ? qcItemtableModel.Parameters[i].ObsArr[2].Observ : "-");
                    oChild.SetProperty("U_Obs4", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(3) != null ? qcItemtableModel.Parameters[i].ObsArr[3].Observ : "-");
                    oChild.SetProperty("U_Obs5", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(4) != null ? qcItemtableModel.Parameters[i].ObsArr[4].Observ : "-");
                    oChild.SetProperty("U_Obs6", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(5) != null ? qcItemtableModel.Parameters[i].ObsArr[5].Observ : "-");
                    oChild.SetProperty("U_Obs7", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(6) != null ? qcItemtableModel.Parameters[i].ObsArr[6].Observ : "-");
                    oChild.SetProperty("U_Obs8", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(7) != null ? qcItemtableModel.Parameters[i].ObsArr[7].Observ : "-");
                    oChild.SetProperty("U_Obs9", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(8) != null ? qcItemtableModel.Parameters[i].ObsArr[8].Observ : "-");
                    oChild.SetProperty("U_Obs10", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(9) != null ? qcItemtableModel.Parameters[i].ObsArr[9].Observ : "-");
                    oChild.SetProperty("U_Obs11", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(10) != null ? qcItemtableModel.Parameters[i].ObsArr[10].Observ : "-");
                    oChild.SetProperty("U_Obs12", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(11) != null ? qcItemtableModel.Parameters[i].ObsArr[11].Observ : "-");
                    oChild.SetProperty("U_Obs13", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(12) != null ? qcItemtableModel.Parameters[i].ObsArr[12].Observ : "-");
                    oChild.SetProperty("U_Obs14", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(13) != null ? qcItemtableModel.Parameters[i].ObsArr[13].Observ : "-");
                    oChild.SetProperty("U_Obs15", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(14) != null ? qcItemtableModel.Parameters[i].ObsArr[14].Observ : "-");
                    oChild.SetProperty("U_Obs16", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(15) != null ? qcItemtableModel.Parameters[i].ObsArr[15].Observ : "-");
                    oChild.SetProperty("U_Obs17", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(16) != null ? qcItemtableModel.Parameters[i].ObsArr[16].Observ : "-");
                    oChild.SetProperty("U_Obs18", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(17) != null ? qcItemtableModel.Parameters[i].ObsArr[17].Observ : "-");
                    oChild.SetProperty("U_Obs19", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(18) != null ? qcItemtableModel.Parameters[i].ObsArr[18].Observ : "-");
                    oChild.SetProperty("U_Obs20", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(19) != null ? qcItemtableModel.Parameters[i].ObsArr[19].Observ : "-");
                    oChild.SetProperty("U_Obs21", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(20) != null ? qcItemtableModel.Parameters[i].ObsArr[20].Observ : "-");
                    oChild.SetProperty("U_Obs22", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(21) != null ? qcItemtableModel.Parameters[i].ObsArr[21].Observ : "-");
                    oChild.SetProperty("U_Obs23", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(22) != null ? qcItemtableModel.Parameters[i].ObsArr[22].Observ : "-");
                    oChild.SetProperty("U_Obs24", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(23) != null ? qcItemtableModel.Parameters[i].ObsArr[23].Observ : "-");
                    oChild.SetProperty("U_Obs25", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(24) != null ? qcItemtableModel.Parameters[i].ObsArr[24].Observ : "-");
                    oChild.SetProperty("U_Obs26", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(25) != null ? qcItemtableModel.Parameters[i].ObsArr[25].Observ : "-");
                    oChild.SetProperty("U_Obs27", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(26) != null ? qcItemtableModel.Parameters[i].ObsArr[26].Observ : "-");
                    oChild.SetProperty("U_Obs28", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(27) != null ? qcItemtableModel.Parameters[i].ObsArr[27].Observ : "-");
                    oChild.SetProperty("U_Obs29", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(28) != null ? qcItemtableModel.Parameters[i].ObsArr[28].Observ : "-");
                    oChild.SetProperty("U_Obs30", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(29) != null ? qcItemtableModel.Parameters[i].ObsArr[29].Observ : "-");
                    oChild.SetProperty("U_Obs31", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(30) != null ? qcItemtableModel.Parameters[i].ObsArr[30].Observ : "-");
                    oChild.SetProperty("U_Obs32", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(31) != null ? qcItemtableModel.Parameters[i].ObsArr[31].Observ : "-");
                    oChild.SetProperty("U_Obs33", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(32) != null ? qcItemtableModel.Parameters[i].ObsArr[32].Observ : "-");
                    oChild.SetProperty("U_Obs34", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(33) != null ? qcItemtableModel.Parameters[i].ObsArr[33].Observ : "-");
                    oChild.SetProperty("U_Obs35", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(34) != null ? qcItemtableModel.Parameters[i].ObsArr[34].Observ : "-");
                    oChild.SetProperty("U_Obs36", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(35) != null ? qcItemtableModel.Parameters[i].ObsArr[35].Observ : "-");
                    oChild.SetProperty("U_Obs37", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(36) != null ? qcItemtableModel.Parameters[i].ObsArr[36].Observ : "-");
                    oChild.SetProperty("U_Obs38", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(37) != null ? qcItemtableModel.Parameters[i].ObsArr[37].Observ : "-");
                    oChild.SetProperty("U_Obs39", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(38) != null ? qcItemtableModel.Parameters[i].ObsArr[38].Observ : "-");
                    oChild.SetProperty("U_Obs40", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(39) != null ? qcItemtableModel.Parameters[i].ObsArr[39].Observ : "-");
                    oChild.SetProperty("U_Obs41", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(40) != null ? qcItemtableModel.Parameters[i].ObsArr[40].Observ : "-");
                    oChild.SetProperty("U_Obs42", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(41) != null ? qcItemtableModel.Parameters[i].ObsArr[41].Observ : "-");
                    oChild.SetProperty("U_Obs43", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(42) != null ? qcItemtableModel.Parameters[i].ObsArr[42].Observ : "-");
                    oChild.SetProperty("U_Obs44", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(43) != null ? qcItemtableModel.Parameters[i].ObsArr[43].Observ : "-");
                    oChild.SetProperty("U_Obs45", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(44) != null ? qcItemtableModel.Parameters[i].ObsArr[44].Observ : "-");
                    oChild.SetProperty("U_Obs46", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(45) != null ? qcItemtableModel.Parameters[i].ObsArr[45].Observ : "-");
                    oChild.SetProperty("U_Obs47", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(46) != null ? qcItemtableModel.Parameters[i].ObsArr[46].Observ : "-");
                    oChild.SetProperty("U_Obs48", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(47) != null ? qcItemtableModel.Parameters[i].ObsArr[47].Observ : "-");
                    oChild.SetProperty("U_Obs49", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(48) != null ? qcItemtableModel.Parameters[i].ObsArr[48].Observ : "-");
                    oChild.SetProperty("U_Obs50", qcItemtableModel.Parameters[i].ObsArr.ElementAtOrDefault(49) != null ? qcItemtableModel.Parameters[i].ObsArr[49].Observ : "-");

                }

                oGeneralService.Add(oGeneralData);
                string Comand = "select top 1 \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_OBSH\"  ORDER BY \"DocEntry\" DESC";
                DataTable DT = Sqlhana.GetHanaDataSQL(Comand);
                string DocEntry = DT.Rows[0]["DocEntry"].ToString();
                string s = "QC Done Dying Data Saved with DocEntry " + DocEntry;
                return s;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion
    }
}