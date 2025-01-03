using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM_Sahib.Models
{
    // Created By Dilshad A. on 20 Mar 2021
    public class SalesQuotationHeader
    {
        public string CreatedByCode { get; set; }
        public string CreatedByName { get; set; }
        public string CustomerCode { get; set; }
        public int DocNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime PostingDate { get; set; }
        public string ContactPerson { get; set; }
        public DateTime ValidDate { get; set; }
        public int BranchCode { get; set; }
        public int series { get; set; }
        public DateTime DocDate { get; set; }

        public List<SalesQuotationRow> Row { get; set; }

    }

    public class SalesQuotationRow
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public string Taxcode { get; set; }
        public double Discount { get; set; }
        public double RowTotal { get; set; }
        public double UnitPrice { get; set; }

        //----------------------Sample Advised Sheet-------------------
        public string QuotationDocNum { get; set; }
        public string Division { get; set; }
        public string DivionID { get; set; }
        public string SubDivision { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public int OppNum { get; set; }
        public int RowDocEntry { get; set; }
        public int LineId { get; set; }
    }

    public class SalesOrderHeader
    {
        public string CreatedByCode { get; set; }
        public string CreatedByName { get; set; }
        public string CustomerCode { get; set; }
        public int DocNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime PostingDate { get; set; }
        public string ContactPerson { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int BranchCode { get; set; }
        public int series { get; set; }
        public DateTime DocDate { get; set; }
        public List<SalesOrderRow> Row { get; set; }

    }

    public class SalesOrderRow
    {

        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public string Taxcode { get; set; }
        public double Discount { get; set; }
        public double RowTotal { get; set; }
        public double UnitPrice { get; set; }
        public string SubCatName { get; set; }

    }

}