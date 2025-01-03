using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CRM_Sahib.Models
{
    public class SapConnection
    {
        public Company ConnectCompany(ref int ErrorCode, ref string _errorMessage)
        {
            Company _company = new Company();

            if (!_company.Connected)
            {
                _company.DbServerType = (BoDataServerTypes)Convert.ToInt32(ConfigurationManager.AppSettings["databaseserver"]);
                _company.Server = ConfigurationManager.AppSettings["SAPServer"];
                _company.DbUserName = ConfigurationManager.AppSettings["SAPDatabaseUser"];
                _company.DbPassword = ConfigurationManager.AppSettings["SAPDatabasePassword"];
                _company.LicenseServer = ConfigurationManager.AppSettings["SAPLicenseServer"];
                _company.CompanyDB = ConfigurationManager.AppSettings["SAPDatabase"];
                _company.UserName = ConfigurationManager.AppSettings["SAPUser"];
                _company.Password = ConfigurationManager.AppSettings["SAPPassword"];
                ErrorCode = _company.Connect();
                if (ErrorCode != 0)
                {
                    _company.GetLastError(out ErrorCode, out _errorMessage);
                }
            }

            return _company;
        } 
    }
}