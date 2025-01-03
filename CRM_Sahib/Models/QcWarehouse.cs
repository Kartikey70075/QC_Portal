using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace V_Weave_Qc.Models
{
    public class QcWarehouse
    {
        public string QC_PassWarehouses { get; set; }
        public string PassSeries { get; set; }
        public string RejectWareHouse { get; set; }
        public string RejectSeries { get; set; }
        public string HoldWareHouse { get; set; }
        public string HoldSeries { get; set; }
        public string ShortageWareHouse { get; set; }
        public string ShortageSeries { get; set; }
        public string ExtraWarehouse { get; set; }
        public string ExtraQtySeries { get; set; }
        public string ReworkWarehouse { get; set; }
        public string ReworkSeries { get; set; }

    }
}