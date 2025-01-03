using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM_Sahib.Models
{
    public class OpportunityList
    {
        public int OpprId { get; set; }// Added By Dilshad on 18 Mar 2021
        public string OppType { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string OppName { get; set; }
        public int OppNumber { get; set; }
        public string ContactPerson { get; set; }
        public int SalesPersonCode { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime PredictedCloseDate { get; set; }
        public double PotentialAmount { get; set; }

        public int Division { get; set; }
        public List<StagesList> Stage { get; set; }

        public List<DivisionList> Divisionlist { get; set; }
    }

    public class StagesList
    {
        public DateTime StartDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public int SalesPersonCode { get; set; }
        public int SlpCode { get; set; }// for single Sales Persone Code on every Stage
        public int StageID { get; set; }
        public double StatgePercentage { get; set; }
        public double PotentialAmount { get; set; }
        public double WeightAmount { get; set; }
        public string DocType { get; set; }
        public int DocEntry { get; set; }
        public string Activity { get; set; }
        public string ActivityRemarks { get; set; }

    }

    public class DivisionList
    {
        public string DivisionID { get; set; }
        public string SubDivsion { get; set; }
        public string Cateogry { get; set; }
        public string DivisionName { get; set; }
        public string ApplicationOrder { get; set; }
        public decimal? Price { get; set; }
        public bool Matching { get; set; }
        public bool Offered { get; set; }
        public string DocNum { get; set; }
        public string DivisionRemarks { get; set; }
    }

    public class AssginHead
    {
        public string AssginTo { get; set; }
        public List<AssginRow> Row { get; set; }
    }

    public class AssginRow
    {
        public int LineId { get; set; }
        public string OppNum { get; set; }
        public string DocNum { get; set; }
        public string Division { get; set; }
        public string SubDivision { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }
        public int RowDocEntry { get; set; }

    }

    public class ItemList
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

    }
}