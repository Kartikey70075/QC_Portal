using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace V_Weave_Qc.Models
{
    public class SapConnection
    {

        public static string gConnStr;
        public static string gHANASERVER;
        public static string gHANAUSER_NAME;
        public static string gHANAUSER_PASSWORD;
        public static string gHANADATABASE;
        public static string gSAPSERVER;
        public static string gSAPUSER_NAME;
        public static string gSAPUSER_PASSWORD;
        public static string gSAPDATABASE;
        public static string strConnectionString = string.Empty;
        public static DbConnection oDbConnection;
        string Is_serverType = string.Empty;
        public Company connecttocompany(ref int ErrorCode, ref string _errorMessage)
        {
            Company _company = new Company();
            if (!_company.Connected)
            {

                #region SAP CONNECTION STRING 
                gSAPSERVER = ConfigurationSettings.AppSettings["SAPSERVER"];
                gSAPDATABASE = ConfigurationSettings.AppSettings["SAPDATABASE"];
                gSAPUSER_NAME = ConfigurationSettings.AppSettings["SAPUSER"];
                gSAPUSER_PASSWORD = ConfigurationSettings.AppSettings["SAPPASSWORD"];
                #endregion
                #region HANA CONNECTION STRING 
                gHANASERVER = ConfigurationSettings.AppSettings["HANASERVER"];
                gHANADATABASE = ConfigurationSettings.AppSettings["HANADATABASENAME"];
                gHANAUSER_NAME = ConfigurationSettings.AppSettings["HANAUSERNAME"];
                gHANAUSER_PASSWORD = ConfigurationSettings.AppSettings["HANAPASSWORD"];
                Is_serverType = ConfigurationSettings.AppSettings["SERVERTYPE"];
                #endregion
                if (Is_serverType == "HANA")
                {
                    strConnectionString = string.Empty;
                    if (IntPtr.Size == 8)
                    {
                        // Do 64-bit stuff
                        strConnectionString = string.Concat(strConnectionString, "Driver={HDBODBC};");
                    }
                    else
                    {
                        // Do 32-bit
                        strConnectionString = string.Concat(strConnectionString, "Driver={HDBODBC32};");
                    }
                    strConnectionString = string.Concat(strConnectionString, "UID=", gHANAUSER_NAME, ";");
                    strConnectionString = string.Concat(strConnectionString, "PWD=", gHANAUSER_PASSWORD, ";");
                    strConnectionString = string.Concat(strConnectionString, "ServerNode=", gHANASERVER, ";");
                    strConnectionString = string.Concat(strConnectionString, "CS=", gHANADATABASE);

                    DbProviderFactory oDbProviderFactory;
                    oDbProviderFactory = DbProviderFactories.GetFactory("System.Data.Odbc");
                    oDbConnection = oDbProviderFactory.CreateConnection();
                    oDbConnection.ConnectionString = strConnectionString;
                    oDbConnection.Open();
                }
                _company.Server = gSAPSERVER;
                _company.CompanyDB = gSAPDATABASE;
                _company.UserName = gSAPUSER_NAME;
                _company.Password = gSAPUSER_PASSWORD;
                _company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                //_company.Server = ConfigurationManager.AppSettings["SAPServer"];
                //_company.DbUserName = ConfigurationManager.AppSettings["SAPDatabaseUser"];
                //_company.DbPassword = ConfigurationManager.AppSettings["SAPDatabasePassword"];
                //_company.LicenseServer = ConfigurationManager.AppSettings["SAPLicenseServer"];
                //_company.CompanyDB = ConfigurationManager.AppSettings["SAPDatabase"];
                //_company.UserName = ConfigurationManager.AppSettings["SAPUser"];
                //_company.Password = ConfigurationManager.AppSettings["SAPPassword"];
                ErrorCode = _company.Connect();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out _errorMessage);
                }
            }
            return _company;
        }

        #region HANA Conneciton by Dilshad A. on 06 Apr 2021
        public void connectCompany()
        {
            int retValue = 0;
            SAPbobsCOM.Company oCompany = null;
            string retMsg = String.Empty;
            DataTable dtSAPDet = new DataTable();
            try
            {
                #region SAP CONNECTION STRING 
                gSAPSERVER = ConfigurationSettings.AppSettings["SAPSERVER"];
                gSAPDATABASE = ConfigurationSettings.AppSettings["SAPDATABASE"];
                gSAPUSER_NAME = ConfigurationSettings.AppSettings["SAPUSER"];
                gSAPUSER_PASSWORD = ConfigurationSettings.AppSettings["SAPPASSWORD"];
                #endregion
                #region HANA CONNECTION STRING 
                gHANASERVER = ConfigurationSettings.AppSettings["HANASERVER"];
                gHANADATABASE = ConfigurationSettings.AppSettings["HANADATABASENAME"];
                gHANAUSER_NAME = ConfigurationSettings.AppSettings["HANAUSERNAME"];
                gHANAUSER_PASSWORD = ConfigurationSettings.AppSettings["HANAPASSWORD"];
                Is_serverType = ConfigurationSettings.AppSettings["SERVERTYPE"];
                #endregion
                if (Is_serverType == "HANA")
                {
                    strConnectionString = string.Empty;
                    if (IntPtr.Size == 8)
                    {
                        // Do 64-bit stuff
                        strConnectionString = string.Concat(strConnectionString, "Driver={HDBODBC};");
                    }
                    else
                    {
                        // Do 32-bit
                        strConnectionString = string.Concat(strConnectionString, "Driver={HDBODBC32};");
                    }
                    strConnectionString = string.Concat(strConnectionString, "UID=", gHANAUSER_NAME, ";");
                    strConnectionString = string.Concat(strConnectionString, "PWD=", gHANAUSER_PASSWORD, ";");
                    strConnectionString = string.Concat(strConnectionString, "ServerNode=", gHANASERVER, ";");
                    strConnectionString = string.Concat(strConnectionString, "CS=", gHANADATABASE);

                    DbProviderFactory oDbProviderFactory;
                    oDbProviderFactory = DbProviderFactories.GetFactory("System.Data.Odbc");
                    oDbConnection = oDbProviderFactory.CreateConnection();
                    oDbConnection.ConnectionString = strConnectionString;
                    oDbConnection.Open();
                }
                oCompany = new SAPbobsCOM.Company();
                oCompany.Server = gSAPSERVER;
                oCompany.CompanyDB = gSAPDATABASE;
                oCompany.UserName = gSAPUSER_NAME;
                oCompany.Password = gSAPUSER_PASSWORD;
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                if (oCompany.Connected)
                {
                    oCompany.Disconnect();
                }
                retValue = oCompany.Connect();
                if (retValue != 0)
                {
                    oCompany.GetLastError(out retValue, out retMsg);
                }
                else
                    retMsg = "Connented";
                #region NOT IN USE
                //oCompany = new SAPbobsCOM.Company();
                //oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                //oCompany.Server = @"103.20.213.209:30015";
                //oCompany.CompanyDB = "VWEAVE_TEST2";
                //oCompany.DbUserName = "SYSTEM";            
                //oCompany.DbPassword = "P@ssw0rd88";
                //oCompany.UserName = "PLM01";
                //oCompany.Password = "manager";
                //oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
                ////oCompany.LicenseServer = "103.20.213.209";
                ////oCompany.UseTrusted = false;
                //retValue = oCompany.Connect();



                //if (retValue != 0)
                //{
                //    oCompany.GetLastError(out retValue, out retMsg);
                //    //MessageBox.Show("In Connect Company: " + retValue + ":" + retMsg);
                //}
                //else
                //    retMsg = "Connented";
                ////MessageBox.Show("Connected");
                #endregion
            }
            catch (Exception ex)
            {
                //MessageBox.Show("In Connect Company. Catch: " + ex.Message);
                throw ex;
            }
        }
        #endregion END HANA Conneciton by Dilshad A. on 06 Apr 2021


    }

}
