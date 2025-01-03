using V_Weave_Qc.Models;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAPbobsCOM;

namespace V_Weave_Qc.Controllers
{
    public class QCAPIController : ApiController
    {
        HanaConnection con;
        HanaSQL Sqlhana;

        public QCAPIController()
        {
            con = new HanaConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Hana"].ConnectionString);

            Sqlhana = new HanaSQL();
        }

        #region row material qc by wamique shaikh 09_23_2022
        [HttpGet]
        public object Get_Data_QC_init()
        {
            try
            {

                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select \"BPLId\" AS \"BranchId\" ,\"BPLName\" AS \"BranchName\",\"BPLFrName\" AS \"BPLFrName\" from" + "\"" + SCHEMA + "\"" + ".\"OBPL\"";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OBPL> list = new List<OBPL>();
                list = HanaSQL.ConvertDataTable<OBPL>(DT);
                return list;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IHttpActionResult Get_WareHouses_QCInit2(string ItmsGrpCod)
        {
            try
            {
                var ab = 1;
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select \"WhsCode\" AS \"WhsCode\" ,\"WhsName\" AS \"WhsName\" from" + "\"" + SCHEMA + "\"" + ".\"OWHS\"";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OWHS> list = new List<OWHS>();
                list = HanaSQL.ConvertDataTable<OWHS>(DT);
                var WareHouses = list;
                string command2 = "select f.\"U_PassWareHouse\" AS \"QC_PassWarehouses\" ," +
                    "f.\"U_PassSeries\" AS \"PassSeries\" ," +
                    "f.\"U_RejectWareHouse\" AS \"RejectWareHouse\" " +
                    ",f.\"U_RejectSeries\" AS \"RejectSeries\" " +
                    ",f.\"U_HoldWarehouse\" AS \"HoldWareHouse\" ," +
                    "f.\"U_HoldSeries\" AS \"HoldSeries\" ," +
                    "f.\"U_ShortageWarehouse\" AS \"ShortageWareHouse\" " +
                    ",f.\"U_ShortageSeries\" AS \"ShortageSeries\" " +
                    ",f.\"U_ExtraWarehouse\" AS \"ExtraWarehouse\" ," +
                    "f.\"U_ExtraQtySeries\" AS \"ExtraQtySeries\" " +
                    ",f.\"U_ReworkWarehouse\" AS \"ReworkWarehouse\" , " +
                    "f.\"U_ReworkSeries\" AS \"ReworkSeries\" " +
                    "from" + "\"" + SCHEMA + "\"" + ".\"@QC_WORK_DATAH\" e LEFT OUTER JOIN"
                    + "\"" + SCHEMA + "\"" + ".\"@QC_WORK_DATAR\"  f ON e.\"DocEntry\" = f.\"DocEntry\" " +
                    "where e.\"U_ItemGroup\"=" + "'" + ItmsGrpCod + "'";
                DataTable DT2 = Sqlhana.GetHanaDataSQL(command2);
                List<QcWarehouse> list2 = new List<QcWarehouse>();
                list2 = HanaSQL.ConvertDataTable<QcWarehouse>(DT2);
                var QC_Warehouses_Data = list2;
                var data = new { WareHouses, QC_Warehouses_Data };
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IHttpActionResult GetGRPONumber(string combstring)
        {
            try
            {
                string CardCode = combstring.Split('<')[0];
                string Branch = combstring.Split('<')[1];
                string process = combstring.Split('<')[2];
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                //string command = "select \"NumAtCard\" AS \"NumAtCard\", \"DocNum\" AS \"DocNum\" ,\"DocEntry\" AS \"DocEntry\" from" + "\"" + SCHEMA + "\"" + ".\"OPDN\" where  \"CardCode\"=" + "'" + CardCode + "'" + " and \"DocStatus\"='O'" +
                //  "and \"Series\" = '135'";
                string command = "SELECT DISTINCT OPDN.\"DocNum\", OPDN.\"DocEntry\", OPDN.\"DocStatus\", OPDN.\"NumAtCard\", IBT1.\"WhsCode\" FROM " + "\"" + SCHEMA + "\"" + ".\"OPDN\"" +
 " LEFT JOIN " + "\"" + SCHEMA + "\"" + ".\"PDN1\" ON PDN1.\"DocEntry\" = OPDN.\"DocEntry\"" +
 "LEFT JOIN " + "\"" + SCHEMA + "\"" + ". \"IBT1\" ON IBT1.\"BaseEntry\" = PDN1.\"DocEntry\" " +
 "and IBT1.\"ItemCode\" = PDN1.\"ItemCode\" " +
 "and IBT1.\"BaseType\" = PDN1.\"ObjType\" " +
 "and PDN1.\"WhsCode\" = IBT1.\"WhsCode\" " +
 "where IBT1.\"Quantity\" <> 0 and OPDN.\"CardCode\"='" + CardCode + "' and OPDN.\"DocStatus\" = 'O' and OPDN.\"U_QS\" = 'N'";
                    DataTable DT = Sqlhana.GetHanaDataSQL(command);
                    List<OPDN> GRPOData = new List<OPDN>();
                    GRPOData = HanaSQL.ConvertDataTable<OPDN>(DT);
                    return Ok(GRPOData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public IHttpActionResult GetGRPORowDetails(string DocEntry, string WhsCode)
        {
            try
            {
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];

                string command = "select DISTINCT T0.\"DocNum\" AS\"WorkOrder\", T0.\"NumAtCard\" As \"NumAtCard\", T1.\"BaseRef\", T2.\"ItmsGrpCod\",T1.\"ItemCode\" AS \"ItemCode\",T1.\"LineNum\" AS \"LineNum\" ," +
                    "T1.\"Dscription\" AS \"Dscription\" ,T1.\"Quantity\" AS \"Quantity\" ," +
                    "T0.\"DocEntry\", '' as \"U_PARTNo\", T2.\"ItmsGrpCod\", T3.\"ItmsGrpNam\", " +
                    "CASE WHEN T3.\"ItmsGrpNam\" LIKE 'RM%' THEN 'Raw Material' else 'Packing' end as \"ItemGroupType\", " +
                    "T2.\"U_RMTYPE\", T2.\"U_Count\", T2.\"U_PLY\",  1 AS \"U_QCPercentage\",T1.\"WhsCode\",T2.\"ManBtchNum\"," +
                    " q.\"ItmsGrpNam\" as \"MaterialType\", u.\"UomName\" FROM " + "\"" + SCHEMA + "\"" + ".\"OPDN\" T0 " +
                    "INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"PDN1\" T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" " +
                    "INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"OITM\" T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                    "left JOIN  " + "\"" + SCHEMA + "\"" + ".\"IBT1\" T6 ON T6.\"BaseEntry\" = T1.\"DocEntry\" and T6.\"BaseLinNum\" = T1.\"LineNum\"" +
                   " left JOIN  " + "\"" + SCHEMA + "\"" + ".\"OIBT\" A on a.\"BatchNum\" = T6.\"BatchNum\" and  a.\"WhsCode\" = T1.\"WhsCode\"" +
                    "INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"OITB\" T3 ON T3.\"ItmsGrpCod\" = T2.\"ItmsGrpCod\"" +
                    "INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"OITW\" T4 ON T4.\"ItemCode\" = T1.\"ItemCode\" " +
                    "LEFT JOIN " + "\"" + SCHEMA + "\"" + ".\"OUOM\" u on u.\"UomEntry\" = t2.\"UgpEntry\" " +
                    "LEFT JOIN  " + "\"" + SCHEMA + "\"" + ".\"OITG\" q ON CASE " +
                    "WHEN IFNULL(T2.\"QryGroup1\", '') = 'Y' THEN 1 " +
                    "WHEN IFNULL(T2.\"QryGroup2\", '') = 'Y' THEN 2 " +
                    "WHEN IFNULL(T2.\"QryGroup3\", '') = 'Y' THEN 3 " +
                    "WHEN IFNULL(T2.\"QryGroup4\", '') = 'Y' THEN 4 " +
                    "WHEN IFNULL(T2.\"QryGroup5\", '') = 'Y' THEN 5 " +
                    "WHEN IFNULL(T2.\"QryGroup6\", '') = 'Y' THEN 6 " +
                    "WHEN IFNULL(T2.\"QryGroup7\", '') = 'Y' THEN 7 " +
                    "WHEN IFNULL(T2.\"QryGroup8\", '') = 'Y' THEN 8 " +
                    "WHEN IFNULL(T2.\"QryGroup9\", '') = 'Y' THEN 9 " +
                    "WHEN IFNULL(T2.\"QryGroup10\", '') = 'Y' THEN 10 " +
                    "WHEN IFNULL(T2.\"QryGroup11\", '') = 'Y' THEN 11 " +
                    "WHEN IFNULL(T2.\"QryGroup12\", '') = 'Y' THEN 12 " +
                    "WHEN IFNULL(T2.\"QryGroup13\", '') = 'Y' THEN 13 " +
                    "WHEN IFNULL(T2.\"QryGroup14\", '') = 'Y' THEN 14 " +
                    "WHEN IFNULL(T2.\"QryGroup15\", '') = 'Y' THEN 15 " +
                    "WHEN IFNULL(T2.\"QryGroup16\", '') = 'Y' THEN 16 " +
                    "WHEN IFNULL(T2.\"QryGroup17\", '') = 'Y' THEN 17 " +
                    "WHEN IFNULL(T2.\"QryGroup18\", '') = 'Y' THEN 18 " +
                    "WHEN IFNULL(T2.\"QryGroup19\", '') = 'Y' THEN 19 " +
                    "WHEN IFNULL(T2.\"QryGroup20\", '') = 'Y' THEN 20 END = \"ItmsTypCod\" " +
                    "WHERE T0.\"DocEntry\"=" + Convert.ToInt32(DocEntry) + " and T4.\"WhsCode\" ='"+ WhsCode + "' and T1.\"U_IsQcDone\" ='" + "N" + "' and a.\"Quantity\" <> 0 ";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OPDN1> list = new List<OPDN1>();
                list = HanaSQL.ConvertDataTable<OPDN1>(DT);

                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public object Get_Item_Params(string CombinedString)
        {
            try
            {
                string ItemCode = CombinedString.Split('<')[0];
                string ItemType = CombinedString.Split('<')[1];
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select T1.\"U_Description\" AS \"Description\" ,T1.\"U_Instrument\" AS \"Instrument\", " +
                    " T1.\"U_Maximum\" as \"Maximum\",T1.\"U_Minimum\" as \"Minimum\",T1.\"U_Tolerance\" as \"Tolerance\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_PAH\" T0 " +
                    " INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"@QC_PAR\" T1 ON T1.\"DocEntry\"=T0.\"DocEntry\" WHERE T0.\"U_ItemCode\"='" + ItemCode + "'";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<QC1> list = new List<QC1>();
                list = HanaSQL.ConvertDataTable<QC1>(DT);
                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public IHttpActionResult CreatQCfromGRPOINT()
        {
            try
            {
                var data = new
                {
                    OCRD = OCRD(),
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object OCRD()
        {
            string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
            string command = "select \"CardCode\" AS \"CardCode\" ,\"E_Mail\" AS \"E_Mail\" ,\"CardName\" AS \"CardName\" ,\"CardType\" AS \"CardType\" from" + "\"" + SCHEMA + "\"" + ".\"OCRD\"";
            string command2 = "select \"CardCode\" AS \"CardCode\", \"NumAtCard\" AS \"NumAtCard\" from" + "\"" + SCHEMA + "\"" + ".\"OPDN\"";
            DataTable DT2 = Sqlhana.GetHanaDataSQL(command2);
            List<OPDN> list2 = new List<OPDN>();
            list2 = HanaSQL.ConvertDataTable<OPDN>(DT2);
            DataTable DT = Sqlhana.GetHanaDataSQL(command);
            List<OCRD> list = new List<OCRD>();
            list = HanaSQL.ConvertDataTable<OCRD>(DT);
            return list;
        }
        #endregion

        #region Create Qc ON Production By Wamique Shaikh 26-09-2022
        [HttpGet]
        public object Get_Data_QC_init_Prod()
        {
            try
            {
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select \"BPLId\" AS \"BranchId\" ,\"BPLName\" AS \"BranchName\",\"BPLFrName\" AS \"BPLFrName\" from" + "\"" + SCHEMA + "\"" + ".\"OBPL\"";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OBPL> list = new List<OBPL>();
                list = HanaSQL.ConvertDataTable<OBPL>(DT);
                return list;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public object Get_WareHouses_QCInitProd(string Branch, string process)
        {
            try
            {
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select \"WhsCode\" AS \"WhsCode\" ,\"WhsName\" AS \"WhsName\" from" + "\"" + SCHEMA + "\"" + ".\"OWHS\"";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OWHS> list = new List<OWHS>();
                list = HanaSQL.ConvertDataTable<OWHS>(DT);
                var WareHouses = list;
                string command2 = "select f.\"U_PassWareHouse\" AS \"QC_PassWarehouses\" ," +
                    "f.\"U_PassSeries\" AS \"PassSeries\" ," +
                    "f.\"U_RejectWareHouse\" AS \"RejectWareHouse\" " +
                    ",f.\"U_RejectSeries\" AS \"RejectSeries\" " +
                    ",f.\"U_HoldWarehouse\" AS \"HoldWareHouse\" ," +
                    "f.\"U_HoldSeries\" AS \"HoldSeries\" ," +
                    "f.\"U_ShortageWarehouse\" AS \"ShortageWareHouse\" " +
                    ",f.\"U_ShortageSeries\" AS \"ShortageSeries\" " +
                    ",f.\"U_ExtraWarehouse\" AS \"ExtraWarehouse\" ," +
                    "f.\"U_ExtraQtySeries\" AS \"ExtraQtySeries\" " +
                    ",f.\"U_ReworkWarehouse\" AS \"ReworkWarehouse\" , " +
                    "f.\"U_ReworkSeries\" AS \"ReworkSeries\" " +
                    "from" + "\"" + SCHEMA + "\"" + ".\"@QC_WORK_DATAH\" e LEFT OUTER JOIN"
                    + "\"" + SCHEMA + "\"" + ".\"@QC_WORK_DATAR\"  f ON e.\"DocEntry\" = f.\"DocEntry\" " +
                    "where e.\"U_BranchID\"=" + "'" + Branch + "'and e.\"U_ItemGroup\"=" + "'" + process + "'";
                DataTable DT2 = Sqlhana.GetHanaDataSQL(command2);
                List<QcWarehouse> list2 = new List<QcWarehouse>();
                list2 = HanaSQL.ConvertDataTable<QcWarehouse>(DT2);
                var QC_Warehouses_Data = list2;
                var data = new { WareHouses, QC_Warehouses_Data };
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IHttpActionResult GetWorkorderNumber(string combstring)
        {
            try
            {
                string CardCode = combstring.Split('<')[0];
                string Branch = combstring.Split('<')[1];
                string Process = combstring.Split('<')[2];
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "SELECT DISTINCT b.\"Ref2\"  as \"WorkOrder\", b.\"Series\" ,b.\"U_VH\", b.\"U_WO\" from " + "\"" + SCHEMA + "\"" + ".\"OIGN\" b " +
                "where b.\"U_PARTYNAME\" =" + "'" + CardCode + "'" + " and b.\"U_Operation\" = " + "'" + Process + "'" + " and \"Series\" = '766'" +
                  "OR \"Series\" = '767'";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OIGN> list = new List<OIGN>();
                list = HanaSQL.ConvertDataTable<OIGN>(DT);

                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IHttpActionResult DocNumber(string combstring)
        {
            try
            {
                string CardCode = combstring.Split('<')[0];
                string Branch = combstring.Split('<')[1];
                string Process = combstring.Split('<')[2];
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "SELECT DISTINCT b.\"Ref2\"  as \"WorkOrder\", b.\"Series\" ,b.\"U_VH\", b.\"U_WO\", b.\"DocNum\" from " + "\"" + SCHEMA + "\"" + ".\"OIGN\" b " +
                "where b.\"U_PARTYNAME\" =" + "'" + CardCode + "'" + " and b.\"U_Operation\" = " + "'" + Process + "'" + " and \"Series\" = '991'" +
                "OR \"Series\" = '992'";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OIGN> list = new List<OIGN>();
                list = HanaSQL.ConvertDataTable<OIGN>(DT);

                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IHttpActionResult OnPOLinkChangeProd(string QC_Prod_Order, string VendorCode, string DocNum)
        {
            try
            {

                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select distinct T1.\"ItemCode\" , T1.\"Dscription\",(select sum(IBT1.\"Quantity\") from  " + "\"" + SCHEMA + "\"" + ".\"IBT1\" where  " + "\"" + SCHEMA + "\"" + ".\"IBT1\".\"ItemCode\" = T1.\"ItemCode\" and " + "\"" + SCHEMA + "\"" + ".\"IBT1\".\"BaseEntry\" = T1.\"DocEntry\" and " + "\"" + SCHEMA + "\"" + ".\"IBT1\".\"BaseType\" = T1.\"ObjType\") as \"Quantity\"  ," +
                   "T2.\"ManBtchNum\",T1.\"DocEntry\",T1.\"WhsCode\", q.\"ItmsGrpNam\", T0.\"Ref2\", p.\"ItmsGrpNam\" as \"MaterialType\", T2.\"U_AliasName\" as \"AliasName\"," +
                   " T0.\"U_PARTYNAME\",u.\"UomName\" " + "from  " + "\"" + SCHEMA + "\"" + ".\"IGN1\" T1" +
                   " INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"OIGN\" T0 ON T0.\"DocEntry\" = T1.\"DocEntry\" " +
                   "left JOIN  " + "\"" + SCHEMA + "\"" + ".\"OITM\" T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                   "left JOIN  " + "\"" + SCHEMA + "\"" + ".\"IBT1\" T3 ON T3.\"BaseEntry\" = T1.\"DocEntry\" and T3.\"BaseLinNum\" = T1.\"LineNum\"" +
                   " left JOIN  " + "\"" + SCHEMA + "\"" + ".\"OIBT\" A on a.\"BatchNum\" = t3.\"BatchNum\" and  a.\"WhsCode\" = T1.\"WhsCode\"" +
                   "Left JOIN  " + "\"" + SCHEMA + "\"" + ".\"OITB\" q on q.\"ItmsGrpCod\" = T2.\"ItmsGrpCod\" " +
                   "LEFT JOIN  " + "\"" + SCHEMA + "\"" + ".\"OUOM\" u on u.\"UomEntry\" = t2.\"UgpEntry\" " +
                    "LEFT JOIN  " + "\"" + SCHEMA + "\"" + ".\"OITG\" p ON CASE " +
                    "WHEN IFNULL(T2.\"QryGroup1\", '') = 'Y' THEN 1 " +
                    "WHEN IFNULL(T2.\"QryGroup2\", '') = 'Y' THEN 2 " +
                    "WHEN IFNULL(T2.\"QryGroup3\", '') = 'Y' THEN 3 " +
                    "WHEN IFNULL(T2.\"QryGroup4\", '') = 'Y' THEN 4 " +
                    "WHEN IFNULL(T2.\"QryGroup5\", '') = 'Y' THEN 5 " +
                    "WHEN IFNULL(T2.\"QryGroup6\", '') = 'Y' THEN 6 " +
                    "WHEN IFNULL(T2.\"QryGroup7\", '') = 'Y' THEN 7 " +
                    "WHEN IFNULL(T2.\"QryGroup8\", '') = 'Y' THEN 8 " +
                    "WHEN IFNULL(T2.\"QryGroup9\", '') = 'Y' THEN 9 " +
                    "WHEN IFNULL(T2.\"QryGroup10\", '') = 'Y' THEN 10 " +
                    "WHEN IFNULL(T2.\"QryGroup11\", '') = 'Y' THEN 11 " +
                    "WHEN IFNULL(T2.\"QryGroup12\", '') = 'Y' THEN 12 " +
                    "WHEN IFNULL(T2.\"QryGroup13\", '') = 'Y' THEN 13 " +
                    "WHEN IFNULL(T2.\"QryGroup14\", '') = 'Y' THEN 14 " +
                    "WHEN IFNULL(T2.\"QryGroup15\", '') = 'Y' THEN 15 " +
                    "WHEN IFNULL(T2.\"QryGroup16\", '') = 'Y' THEN 16 " +
                    "WHEN IFNULL(T2.\"QryGroup17\", '') = 'Y' THEN 17 " +
                    "WHEN IFNULL(T2.\"QryGroup18\", '') = 'Y' THEN 18 " +
                    "WHEN IFNULL(T2.\"QryGroup19\", '') = 'Y' THEN 19 " +
                    "WHEN IFNULL(T2.\"QryGroup20\", '') = 'Y' THEN 20 END = \"ItmsTypCod\" " +
                   " and a.\"WhsCode\" = T3.\"WhsCode\" " +
                   "and T3.\"BaseType\" = '59' " +
                   "WHERE T0.\"Ref2\" = '" + Convert.ToInt32(QC_Prod_Order) + "' " +
                   "and T0.\"U_PARTYNAME\" = '" + VendorCode + "' and T0.\"DocNum\" = '"+ DocNum + "' and T1.\"WhsCode\" = 'INQCV' and a.\"Quantity\" <> 0";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OIGN> list = new List<OIGN>();
                list = HanaSQL.ConvertDataTable<OIGN>(DT);

                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public object Get_Prod_Item_Parameter(string ItemCode)
        {
            try
            {

                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select T1.\"U_Description\" AS \"Description\" ,T1.\"U_Instrument\" AS \"Instrument\", " +
                    " T1.\"U_Maximum\" as \"Maximum\",T1.\"U_Minimum\" as \"Minimum\",T1.\"U_Tolerance\" as \"Tolerance\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_PAH\" T0 " +
                    " INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"@QC_PAR\" T1 ON T1.\"DocEntry\"=T0.\"DocEntry\" WHERE T0.\"U_ItemCode\"='" + ItemCode + "'";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<QC1> params_data = new List<QC1>();
                params_data = HanaSQL.ConvertDataTable<QC1>(DT);
                var data = new { params_data };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public IHttpActionResult CreatQCfromGRPOINT2()

        {
            try
            {
                var data = new
                {
                    OCRD = OCRD(),

                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region QC On Dying by wamique shaikh 26-09-2022

        [HttpGet]
        public object Get_Data_QC_init_Dying()
        {
            try
            {

                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select \"BPLId\" AS \"BranchId\" ,\"BPLName\" AS \"BranchName\",\"BPLFrName\" AS \"BPLFrName\" from" + "\"" + SCHEMA + "\"" + ".\"OBPL\"";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OBPL> list = new List<OBPL>();
                list = HanaSQL.ConvertDataTable<OBPL>(DT);
                return list;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public object Get_WareHouses_QCInitDying(string Branch, string process)
        {
            try
            {
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select \"WhsCode\" AS \"WhsCode\" ,\"WhsName\" AS \"WhsName\" from" + "\"" + SCHEMA + "\"" + ".\"OWHS\"";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OWHS> list = new List<OWHS>();
                list = HanaSQL.ConvertDataTable<OWHS>(DT);
                //string Process = process.ToUpper();
                var WareHouses = list;
                string command2 = "select f.\"U_PassWareHouse\" AS \"QC_PassWarehouses\" ," +
                    "f.\"U_PassSeries\" AS \"PassSeries\" ," +
                    "f.\"U_RejectWareHouse\" AS \"RejectWareHouse\" " +
                    ",f.\"U_RejectSeries\" AS \"RejectSeries\" " +
                    ",f.\"U_HoldWarehouse\" AS \"HoldWareHouse\" ," +
                    "f.\"U_HoldSeries\" AS \"HoldSeries\" ," +
                    "f.\"U_ShortageWarehouse\" AS \"ShortageWareHouse\" " +
                    ",f.\"U_ShortageSeries\" AS \"ShortageSeries\" " +
                    ",f.\"U_ExtraWarehouse\" AS \"ExtraWarehouse\" ," +
                    "f.\"U_ExtraQtySeries\" AS \"ExtraQtySeries\" " +
                    ",f.\"U_ReworkWarehouse\" AS \"ReworkWarehouse\" , " +
                    "f.\"U_ReworkSeries\" AS \"ReworkSeries\" " +
                    "from" + "\"" + SCHEMA + "\"" + ".\"@QC_WORK_DATAH\" e LEFT OUTER JOIN"
                    + "\"" + SCHEMA + "\"" + ".\"@QC_WORK_DATAR\"  f ON e.\"DocEntry\" = f.\"DocEntry\" " +
                    "where e.\"U_BranchID\"=" + "'" + Branch + "'and e.\"U_ItemGroup\"=" + "'" + process + "'";
                DataTable DT2 = Sqlhana.GetHanaDataSQL(command2);
                List<QcWarehouse> list2 = new List<QcWarehouse>();
                list2 = HanaSQL.ConvertDataTable<QcWarehouse>(DT2);
                var QC_Warehouses_Data = list2;
                var data = new { WareHouses, QC_Warehouses_Data };
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IHttpActionResult CreatQCfromGRPOINTDying()

        {
            try
            {
                var data = new
                {
                    OCRD = OCRD(),
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IHttpActionResult OnPOLinkChangeDying(string QC_Prod_Order, string VendorCode,string DocNum)
        {
            try
            {

                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select distinct T1.\"ItemCode\" , T1.\"Dscription\",(select sum(IBT1.\"Quantity\") from  " + "\"" + SCHEMA + "\"" + ".\"IBT1\" where  " + "\"" + SCHEMA + "\"" + "." +
                    "\"IBT1\".\"ItemCode\" = T1.\"ItemCode\" and " + "\"" + SCHEMA + "\"" + ".\"IBT1\".\"BaseEntry\" = T1.\"DocEntry\" and " + "\"" + SCHEMA + "\"" + ".\"IBT1\".\"BaseType\" = T1.\"ObjType\" and " + "\"" + SCHEMA + "\"" + ".\"IBT1\".\"WhsCode\" = T1.\"WhsCode\" and " + "\"" + SCHEMA + "\"" + ".\"IBT1\".\"ItemCode\" = T1.\"ItemCode\" ) as \"Quantity\"  ," +
                   "T2.\"ManBtchNum\",T1.\"DocEntry\",T1.\"WhsCode\", q.\"ItmsGrpNam\",u.\"UomName\" ,T0.\"Ref2\", p.\"ItmsGrpNam\" as \"MaterialType\", T2.\"U_AliasName\" as \"AliasName\", " +
                   "T0.\"U_PARTYNAME\" " + "from  " + "\"" + SCHEMA + "\"" + ".\"IGN1\" " +
                   "T1 INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"OIGN\" T0 ON T0.\"DocEntry\" = T1.\"DocEntry\" " +
                   "Left JOIN  " + "\"" + SCHEMA + "\"" + ".\"OITM\" T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                   "left JOIN  " + "\"" + SCHEMA + "\"" + ".\"IBT1\" T3 ON T3.\"BaseEntry\" = T1.\"DocEntry\" and T3.\"BaseLinNum\" = T1.\"LineNum\" " +
                   " left JOIN  " + "\"" + SCHEMA + "\"" + ".\"OIBT\" A on a.\"BatchNum\" = t3.\"BatchNum\"  and  a.\"WhsCode\" = T1.\"WhsCode\" and a.\"ItemCode\" =  T1.\"ItemCode\" " +
                   "LEFT JOIN  " + "\"" + SCHEMA + "\"" + ".\"OITB\" q on q.\"ItmsGrpCod\" = T2.\"ItmsGrpCod\" " +
                   "LEFT JOIN  " + "\"" + SCHEMA + "\"" + ".\"OUOM\" u on u.\"UomEntry\" = t2.\"UgpEntry\" " +
                    "LEFT JOIN  " + "\"" + SCHEMA + "\"" + ".\"OITG\" p ON CASE " +
                    "WHEN IFNULL(T2.\"QryGroup1\", '') = 'Y' THEN 1 " +
                    "WHEN IFNULL(T2.\"QryGroup2\", '') = 'Y' THEN 2 " +
                    "WHEN IFNULL(T2.\"QryGroup3\", '') = 'Y' THEN 3 " +
                    "WHEN IFNULL(T2.\"QryGroup4\", '') = 'Y' THEN 4 " +
                    "WHEN IFNULL(T2.\"QryGroup5\", '') = 'Y' THEN 5 " +
                    "WHEN IFNULL(T2.\"QryGroup6\", '') = 'Y' THEN 6 " +
                    "WHEN IFNULL(T2.\"QryGroup7\", '') = 'Y' THEN 7 " +
                    "WHEN IFNULL(T2.\"QryGroup8\", '') = 'Y' THEN 8 " +
                    "WHEN IFNULL(T2.\"QryGroup9\", '') = 'Y' THEN 9 " +
                    "WHEN IFNULL(T2.\"QryGroup10\", '') = 'Y' THEN 10 " +
                    "WHEN IFNULL(T2.\"QryGroup11\", '') = 'Y' THEN 11 " +
                    "WHEN IFNULL(T2.\"QryGroup12\", '') = 'Y' THEN 12 " +
                    "WHEN IFNULL(T2.\"QryGroup13\", '') = 'Y' THEN 13 " +
                    "WHEN IFNULL(T2.\"QryGroup14\", '') = 'Y' THEN 14 " +
                    "WHEN IFNULL(T2.\"QryGroup15\", '') = 'Y' THEN 15 " +
                    "WHEN IFNULL(T2.\"QryGroup16\", '') = 'Y' THEN 16 " +
                    "WHEN IFNULL(T2.\"QryGroup17\", '') = 'Y' THEN 17 " +
                    "WHEN IFNULL(T2.\"QryGroup18\", '') = 'Y' THEN 18 " +
                    "WHEN IFNULL(T2.\"QryGroup19\", '') = 'Y' THEN 19 " +
                    "WHEN IFNULL(T2.\"QryGroup20\", '') = 'Y' THEN 20 END = \"ItmsTypCod\" " +
                   " and a.\"WhsCode\" = T3.\"WhsCode\" and T3.\"BaseType\" = '59' WHERE T0.\"Ref2\" = '" + Convert.ToInt32(QC_Prod_Order) + "' " +
                   "and T0.\"U_PARTYNAME\" = '" + VendorCode + "' and T0.\"DocNum\" = '" + DocNum + "' and T1.\"WhsCode\" = 'INQCV' and a.\"Quantity\" <> 0";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OIGN> list = new List<OIGN>();
                list = HanaSQL.ConvertDataTable<OIGN>(DT);
                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public object Get_Dying_Item_Parameter(string ItemCode)
        {
            try
            {

                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select T1.\"U_Description\" AS \"Description\" ,T1.\"U_Instrument\" AS \"Instrument\", " +
                    " T1.\"U_Maximum\" as \"Maximum\",T1.\"U_Minimum\" as \"Minimum\",T1.\"U_Tolerance\" as \"Tolerance\" from" + "\"" + SCHEMA + "\"" + ".\"@QC_PAH\" T0 " +
                    " INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"@QC_PAR\" T1 ON T1.\"DocEntry\"=T0.\"DocEntry\" WHERE T0.\"U_ItemCode\"='" + ItemCode + "'";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<QC1> params_data = new List<QC1>();
                params_data = HanaSQL.ConvertDataTable<QC1>(DT);
                var data = new { params_data };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public IHttpActionResult DocNumberDying(string combstring)
        {
            try
            {
                string CardCode = combstring.Split('<')[0];
                string Branch = combstring.Split('<')[1];
                string Process = combstring.Split('<')[2];
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "SELECT DISTINCT b.\"Ref2\"  as \"WorkOrder\", b.\"Series\" ,b.\"U_VH\", b.\"U_WO\", b.\"DocNum\" from " + "\"" + SCHEMA + "\"" + ".\"OIGN\" b " +
                "where b.\"U_PARTYNAME\" =" + "'" + CardCode + "'" + " and b.\"U_Operation\" = " + "'" + Process + "'" + " and \"Series\" = '991'" +
                 "OR \"Series\" = '992'";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OIGN> list = new List<OIGN>();
                list = HanaSQL.ConvertDataTable<OIGN>(DT);

                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion fatch data complete by wamique shaikh 26-09-2022
        [HttpGet]
        public object Get_Data_QC_ItemGroup()
        {
            try
            {
                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                string command = "select distinct t1.\"ItmsGrpCod\", t2.\"ItmsGrpNam\"  from" + "\"" +
                SCHEMA + "\"" + ".\"OITM\" t1 " +
                 " INNER JOIN " + "\"" + SCHEMA + "\"" + ".\"OITB\" t2 ON t2.\"ItmsGrpCod\"=t1.\"ItmsGrpCod\" ";
                DataTable DT = Sqlhana.GetHanaDataSQL(command);
                List<OIGN> list2 = new List<OIGN>();
                list2 = HanaSQL.ConvertDataTable<OIGN>(DT);
                var list3 = Get_Data_QC_init();
                var data = new { list2, list3 };
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
