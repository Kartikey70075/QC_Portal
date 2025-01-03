using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM_Sahib.Models
{
    public class ARInvoiceHeader
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
        public List<ARInvoiceRow> Row { get; set; }
    }
    public class ARInvoiceRow
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