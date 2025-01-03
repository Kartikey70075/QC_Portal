using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace V_Weave_Qc.Models
{
    public class OPDN
    {
        public string NumAtCard { get; set; }
        public string DocNum { get; set; }
        public string DocEntry { get; set; }
        public string U_WO { get; set; }
        public string WhsCode { get; set; }
        public string DocStatus { get; set; }

    }

    public class OPDN1
    {
        public string ItemCode { get; set; }
        public string PENDINGQTY { get; set; }
        public string Quantity { get; set; }
        public string Dscription { get; set; }
        public string DocEntry { get; set; }
        public string U_PARTNo { get; set; }
        public string ItmsGrpCod { get; set; }
        public string ItmsGrpNam { get; set; }
        public string U_RMTYPE { get; set; }
        public string U_COUNT { get; set; }
        public string U_PLY { get; set; }
        public string U_QCPercentage { get; set; }
        public string WhsCode { get; set; }
        public string ManBtchNum { get; set; }
        public string OnHand { get; set; }
        public string BaseRef { get; set; }
        public string DocNum { get; set; }
        public string WorkOrder { get; set; }
        public string ItemGroupType { get; set; }
        public string MaterialType { get; set; }
        public string UomName { get; set; }
        public string NumAtCard { get; set; }
        public string LineNum { get; set; }
    }
    public class QC1
    {
        public string Description { get; set; }
        public string Instrument { get; set; }
        public string Maximum { get; set; }
        public string Minimum { get; set; }
        public string Tolerance { get; set; }
    }
    public class OCRD
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string E_Mail { get; set; }
        
    }
}