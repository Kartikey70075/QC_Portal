using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace V_Weave_Qc.Models
{
    public class Structure
    {
        public string VendorCode { get; set; }
        // public string InspectionDate { get; set; }
        public string VendorName { get; set; }
        public string PoDocEntry { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string PENDINGQTY { get; set; }
        public string Email { get; set; }
        public string NumAtCard { get; set; }
        public string BpRefrenceNumber { get; set; }
        public string UserName { get; set; }
        public string DocNum { get; set; }
        public string DocEntry { get; set; }
        public int Quantity { get; set; }
        public int ItemGroupType { get; set; }
        public int MaterialType { get; set; }
        public int? PassQuantity { get; set; }
        public int? RejectedQuantity { get; set; }
        public DateTime InspectionDate { get; set; }
        public int rejectqty { get; set; }
        public int reworkqty { get; set; }
        public int totalpass { get; set; }
        public int holdingqty { get; set; }
        public int extraqty { get; set; }
        public string FileUpload { get; set; }
        public List<RowData> Row { get; set; }
        public List<QcClass> QcRow { get; set; }
        public List<RejectQc> RejectedRow { get; set; }
    }
    public class QcClass
    {
        public string U_QCDES { get; set; }
        public string U_QCMAX { get; set; }
        public string U_QCMIN { get; set; }
        public string InspectionSample { get; set; }
        public string QcRemarks { get; set; }
        public string QcObservation { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
    }

    public class RejectQc
    {
        public string U_QCDES { get; set; }
        public string U_QCMAX { get; set; }
        public string U_QCMIN { get; set; }
        public string InspectionSample { get; set; }
        public string QcRemarks { get; set; }
        public string QcObservation { get; set; }
    }
    public class RowData
    {
        public string ItemCode { get; set; }
        public string PENDINGQTY { get; set; }
        public string Dscription { get; set; }
        public string ItemName { get; set; }
        public string Quality { get; set; }
        public int? Quantity { get; set; }
        public int PassQty { get; set; }
        public int RejectedQty { get; set; }
        public int ReworkQty { get; set; }
        public int OnHold { get; set; }
        public int OnExtra { get; set; }
        public string Observation { get; set; }
        public string Remarks { get; set; }
        public string Remark { get; set; }
        public string BaseRef { get; set; }
        //public int? DocNum { get; set; }
        public string DocEntry { get; set; }
        public string WhsCodePass { get; set; }
        public string WhsCodeHold { get; set; }
        public string WhsCodeExtra { get; set; }
        public string WhsCodeReject { get; set; }
        public string WhsCodeShortage { get; set; }
        public string WhsCoderework { get; set; }
        public string Weight { get; set; }
        public string TotalWeight { get; set; }
        public string Rework { get; set; }
        public string DocNum { get; set; }
        public int ItemGroupType { get; set; }
        public int MaterialType { get; set; }
        public int? SuppilerOrderNo { get; set; }
        public List<QcClass> QcRow { get; set; }

    }

    public class Row
    {
        public string ItemCode { get; set; }
        public string PENDINGQTY { get; set; }
        public string Dscription { get; set; }
        public string ItemName { get; set; }
        public string Quality { get; set; }
        public int? Quantity { get; set; }
        public int PassQty { get; set; }
        public int RejectedQty { get; set; }
        public int ReworkQty { get; set; }
        public int OnHold { get; set; }
        public int OnExtra { get; set; }
        public string Observation { get; set; }
        public string Remarks { get; set; }
        public string Remark { get; set; }
        public string BaseRef { get; set; }
        //public int? DocNum { get; set; }
        public string DocEntry { get; set; }
        public string Rework { get; set; }
        public DateTime ManualDate { get; set; }
        public string BinLoaction { get; set; }
        public string ItemGroup { get; set; }
        public string QcType { get; set; }
        public string DocNum { get; set; }
        public string NumAtCard { get; set; }
        public string BpRefrenceNumber { get; set; }
        public string postedFile { get; set; }
        public string WorkOrder { get; set; }
        public int ItemGroupType { get; set; }
        public int MaterialType { get; set; }
        public int? SuppilerOrderNo { get; set; }
    }

    public class ReportList
    {

        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string QcDate { get; set; }
        public string OrderNo { get; set; }
        public string PlanQty { get; set; }
        public string IssueChallan { get; set; }
        public string IssueQty { get; set; }
        public string ReceiptChallan { get; set; }
        public int? PassQty { get; set; }
        public int? RejectQty { get; set; }
        public string ReworkQty { get; set; }
        public string OnHoldQty { get; set; }
        public string ExtraQty { get; set; }

    }
    public class Params
    {
        public string Description { get; set; }
        public string Instrument { get; set; }
        public string Maximum { get; set; }
        public string Minimum { get; set; }
        public string Tolerance { get; set; }
    }
    public class Qcmodeldata
    {
        private List<QcItemtableModel> _qcitem;
        public List<QcItemtableModel> QcItem { get { return _qcitem; } set { _qcitem = value; } }

        public object rejectqty { get; internal set; }
        public object totalpass { get; internal set; }
        public object holdingqty { get; internal set; }
        public object extraqty { get; internal set; }
        public object reworkqty { get; internal set; }
    }

    public class QcItemtableModel
    {
        private string _heatno;
        private bool _checked;
        private string _itemcode;
        private string _Dscription;
        private string _LineNum;
        private string _PENDINGQTY;
        private string _itemdescription;
        private int _openquantity;
        private List<Parameters> _parameter;
        private double _quantity;
        private double _samplequantity;
        private string _passwarehouse;
        private double _reworkqty;
        private double _rejectqty;
        private double _reworkunitprice;
        private string _reworkwarehouse;
        private double _reworktotal;
        private string _rejectwhscode;
        private string _holdingwhscode;
        private string _extrawhscode;
        private double _samplepass;
        private double _samplerej;
        private double _scrapqty;
        private double _shortageqty;
        private double _holdingqty;
        private double _extraqty;
        private string _shortagewhscode;
        private double _totalpass;
        private double _totalquantity;
        private List<ClientData> _clientdata;
        private docseries _docseries;
        private string _fromwhscode;
        private string _unitprice;
        private string _taxcode;
        private string _grpodocnum;
        private int _branchid;
        private string _userid;
        private string _remarks;
        private string _vendor;
        private string _cardcode;
        private string _docentry;
        private string _customremarksname;
        private string _username;
        private string _useremail;
        private string _passseries;
        private string _rejectseries;
        private string _holdingseries;
        private string _shortageseries;
        private string _extraqtyseries;
        private string _cardname;
        private string _itmsgrpnam;
        private string _u_rmtype;
        private string _ManBtchNum;
        private string _reworkseries;
        private string _weight;
        private string _TotalWeight;
        private string _Rework;
        private string _bales;
        private string _remark;
        private string _baseRef;
        private string _bundles;
        private string _NumAtCard;
        private string _BpRefrenceNumber;
        private string _postedFile;
        private string _receiptChallanNo;
        private DateTime _ManualDate;
        private string _BinLoaction;
        private string _QcType;
        private string _ItemGroup;
        private string _U_ItemTyoe;
        private string _MaterialType;
        private string _ItemGroupType;
        private string _DocNum;
        private string _WorkOrder;

        public DateTime ManualDate { get { return _ManualDate; } set { _ManualDate = value; } }
        public string BinLoaction { get { return _BinLoaction; } set { _BinLoaction = value; } }
        public string ItemGroup { get { return _ItemGroup; } set { _ItemGroup = value; } }
        public string QcType { get { return _QcType; } set { _QcType = value; } }
        public string Weight { get { return _weight; } set { _weight = value; } }
        public string TotalWeight { get { return _TotalWeight; } set { _TotalWeight = value; } }
        public string Rework { get { return _Rework; } set { _Rework = value; } }
        public string Bales { get { return _bales; } set { _bales = value; } }
        public string Remark { get { return _remark; } set { _remark = value; } }
        public string BaseRef { get { return _baseRef; } set { _baseRef = value; } }
        public string NumAtCard { get { return _NumAtCard; } set { _NumAtCard = value; } }
        public string BpRefrenceNumber { get { return _BpRefrenceNumber; } set { _BpRefrenceNumber = value; } }
        public string DocNum { get { return _DocNum; } set { _DocNum = value; } }
        public string postedFile { get { return _postedFile; } set { { _postedFile = value; } } }
        public string ReceiptChallanNo { get { return _receiptChallanNo; } set { _receiptChallanNo = value; } }
        public string Bundles { get { return _bundles; } set { _bundles = value; } }
        public string ManBtchNum { get { return _ManBtchNum; } set { _ManBtchNum = value; } }
        public string U_RMTYPE { get { return _u_rmtype; } set { _u_rmtype = value; } }
        public string ItmsGrpNam { get { return _itmsgrpnam; } set { _itmsgrpnam = value; } }
        public string UserName { get { return _username; } set { _username = value; } }
        public string UserEmail { get { return _useremail; } set { _useremail = value; } }
        public string U_ItemTyoe { get { return _U_ItemTyoe; } set { _U_ItemTyoe = value; } }
        public string MaterialType { get { return _MaterialType; } set { _MaterialType = value; } }
        public string ItemGroupType { get { return _ItemGroupType; } set { _ItemGroupType = value; } }
        public string WorkOrder { get { return _WorkOrder; } set { _WorkOrder = value; } }
        //public string DocNum { get { return _DocNum; } set { _DocNum = value; } }

        public string customremarksname { get { return _customremarksname; } set { _customremarksname = value; } }
        public string heatno { get { return _heatno; } set { _heatno = value; } }
        public string DocEntry { get { return _docentry; } set { _docentry = value; } }
        public string CardCode { get { return _cardcode; } set { _cardcode = value; } }
        public string CardName { get { return _cardname; } set { _cardname = value; } }
        public string vendor { get { return _vendor; } set { _vendor = value; } }
        public string remarks { get { return _remarks; } set { _remarks = value; } }

        public string userid { get { return _userid; } set { _userid = value; } }
        public string grpodocnum { get { return _grpodocnum; } set { _grpodocnum = value; } }
        public int branchid { get { return _branchid; } set { _branchid = value; } }
        public string taxcode { get { return _taxcode; } set { _taxcode = value; } }
        public string UnitPrice { get { return _unitprice; } set { _unitprice = value; } }
        public string FromWhsCode { get { return _fromwhscode; } set { _fromwhscode = value; } }
        public docseries docseries { get { return _docseries; } set { _docseries = value; } }
        public List<ClientData> ClientData { get { return _clientdata; } set { _clientdata = value; } }
        public double reworktotal { get { return _reworktotal; } set { _reworktotal = value; } }
        public double reworkunitprice { get { return _reworkunitprice; } set { _reworkunitprice = value; } }
        public double totalquantity { get { return _totalquantity; } set { _totalquantity = value; } }
        public double totalpass { get { return _totalpass; } set { _totalpass = value; } }
        public string QC_ShortageWarehouses { get { return _shortagewhscode; } set { _shortagewhscode = value; } }
        public double shortageqty { get { return _shortageqty; } set { _shortageqty = value; } }
        public double holdingqty { get { return _holdingqty; } set { _holdingqty = value; } }
        public double extraqty { get { return _extraqty; } set { _extraqty = value; } }
        //public string scrapwhscode { get { return _scrapwhscode; } set { _scrapwhscode = value; } }
        public double scrapqty { get { return _scrapqty; } set { _scrapqty = value; } }
        public double samplerej { get { return _samplerej; } set { _samplerej = value; } }
        public double samplepass { get { return _samplepass; } set { _samplepass = value; } }
        public string QC_RejectWareHouses { get { return _rejectwhscode; } set { _rejectwhscode = value; } }
        public string QC_HoldWarehouses { get { return _holdingwhscode; } set { _holdingwhscode = value; } }
        public string QC_ExtraWherehouses { get { return _extrawhscode; } set { _extrawhscode = value; } }
        public string QC_ReworkWarehouses { get { return _reworkwarehouse; } set { _reworkwarehouse = value; } }
        public double rejectqty { get { return _rejectqty; } set { _rejectqty = value; } }
        public double reworkqty { get { return _reworkqty; } set { _reworkqty = value; } }
        public string QC_PassWarehouses { get { return _passwarehouse; } set { _passwarehouse = value; } }
        public double SampleQuantity { get { return _samplequantity; } set { _samplequantity = value; } }
        public double Quantity { get { return _quantity; } set { _quantity = value; } }
        public List<Parameters> Parameters { get { return _parameter; } set { _parameter = value; } }
        public int OpenQuantity { get { return _openquantity; } set { _openquantity = value; } }
        public string ItemDescription { get { return _itemdescription; } set { _itemdescription = value; } }
        public string ItemCode { get { return _itemcode; } set { _itemcode = value; } }
        public string Dscription { get { return _Dscription; } set { _Dscription = value; } }
        public string LineNum { get { return _LineNum; } set { _LineNum = value; } }
        public string PENDINGQTY { get { return _PENDINGQTY; } set { _PENDINGQTY = value; } }
        public bool Checked { get { return _checked; } set { _checked = value; } }
        public string QC_PassSeries { get { return _passseries; } set { _passseries = value; } }
        public string QC_HoldSeries { get { return _holdingseries; } set { _holdingseries = value; } }
        public string QC_RejectSeries { get { return _rejectseries; } set { _rejectseries = value; } }
        public string QC_ShortageSeries { get { return _shortageseries; } set { _shortageseries = value; } }
        public string QC_ExtraQtySeries { get { return _extraqtyseries; } set { _extraqtyseries = value; } }
        public string QC_ReworkSeries { get { return _reworkseries; } set { _reworkseries = value; } }


        //public string ScrapSeries { get { return _scrapseries; } set { _scrapseries = value; } }
    }

    public class docseries
    {
        //PassSeries: "452", ScrapSeries: "454", ReworkSeries: "455", ShortageSeries: "453" GLAccount  SACCode

        private string _glaccount;
        private string _saccode;

        public string GLAccount { get { return _glaccount; } set { _glaccount = value; } }
        public string SACCode { get { return _saccode; } set { _saccode = value; } }

    }

    public class Parameters
    {
        private List<ObservData> _obsarr;
        private int _samplequantity;
        private string _description;
        private string _tolerance;
        private string _maximum;
        private string _minimum;
        private string _instrument;
        private string _u_status;

        public string U_Status { get { return _u_status; } set { _u_status = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string Tolerance { get { return _tolerance; } set { _tolerance = value; } }
        public string Maximum { get { return _maximum; } set { _maximum = value; } }
        public string Minimum { get { return _minimum; } set { _minimum = value; } }
        public string Instrument { get { return _instrument; } set { _instrument = value; } }
        public int SampleQuantity { get { return _samplequantity; } set { _samplequantity = value; } }
        public List<ObservData> ObsArr { get { return _obsarr; } set { _obsarr = value; } }
    }
    public class ObservData
    {
        private string _observ;
        public string Observ { get { return _observ; } set { _observ = value; } }
    }
    public class ClientData
    {
        private string _cardcode;
        private string _cardname;
        private string _contactno;
        public string CardCode { get { return _cardcode; } set { _cardcode = value; } }
        public string CardName { get { return _cardname; } set { _cardname = value; } }
        public string ContactNo { get { return _contactno; } set { _contactno = value; } }
    }
    public class QC_PROD_TabData
    {
        public int OIGE_DocNum { get; set; }
        public string OIGE_Ref2 { get; set; }
        public string OIGE_JobWorkerCode { get; set; }
        public string OIGE_JobWorkerName { get; set; }
        public int IGE1_DocEntry { get; set; }
        public string IGE1_BaseRef { get; set; }
        public int IGE1_Quantity { get; set; }
        public string IGE1_ItemCode { get; set; }
        public string IGE1_ItemDescription { get; set; }
        public int OWOR_DocEntry { get; set; }
        public string OWOR_Process { get; set; }
        public string IGE1_FromWhsCode { get; set; }
        public string InvoicesNumber { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string ItemName { get; set; }
        public string Quality { get; set; }
        public int? Quantity { get; set; }
        public string ManualQuantity { get; set; }
        public int PassQty { get; set; }
        public int RejectedQty { get; set; }
        public int ReworkQty { get; set; }
        public string Observation { get; set; }
        public string Remarks { get; set; }
        public string Remark { get; set; }
        public string BaseRef { get; set; }
        public int? DocNum { get; set; }
        public string DocEntry { get; set; }
        public string Weight { get; set; }
        public string TotalWeight { get; set; }
        public string Rework { get; set; }
        public DateTime ManualDate { get; set; }
        public string BinLoaction { get; set; }
        public string ItemGroup { get; set; }
        public string QcType { get; set; }
        public string MaterialType { get; set; }
        public string AliasName { get; set; }
        public string NumAtCard { get; set; }
        public string postedFile { get; set; }
        public int? SuppilerOrderNo { get; set; }
        public string QC_Process { get; set; }
        public string Email { get; set; }
        public string ItmsGrpNam { get; set; }
        public string ItmsGrpCod { get; set; }
        public string WorkOrder { get; set; }
    }

    public class QcProdmodeldata
    {
        private List<QcPRODItemtableModel> _qcitem;
        public List<QcPRODItemtableModel> QcItem { get { return _qcitem; } set { _qcitem = value; } }
    }
    public class QcPRODItemtableModel
    {
        private string _heatno;
        private string _dscription;
        private string _QC_Process;
        private bool _checked;
        private string _itemcode;
        private string _itemdescription;
        private int _openquantity;
        private List<Parameters> _parameter;
        private double _quantity;
        private double _samplequantity;
        private string _passwarehouse;

        private double _reworkunitprice;
        private double _reworktotal;

        private string _holdingwhscode;
        //private string _extrawhscode;
        private double _samplepass;
        private double _samplerej;
        private double _scrapqty;
        //private string _scrapwhscode;
        private double _shortageqty;
        private double _holdingqty;
        private double _extraqty;
        private string _shortagewhscode;
        private string _extrawhscode;
        private double _totalpass;
        private double _totalquantity;
        private List<ClientData> _clientdata;
        private docseries _docseries;
        private string _fromwhscode;
        private string _unitprice;
        private string _taxcode;
        private string _grpodocnum;
        private int _branchid;
        private string _userid;
        private string _remarks;
        private string _vendor;
        private string _cardcode;
        private string _docentry;
        private string _customremarksname;
        private string _username;
        private string _useremail;
        private string _passseries;
        //private string _scrapseries;
        private string _rejectseries;
        private string _holdingseries;
        private string _extraseries;
        private string _shortageseries;
        private string _cardname;
        private string _itmsgrpnam;
        private string _u_rmtype;
        private string _ManBtchNum;
        private string _ItmsGrpCod;

        private string _weight;
        private string _TotalWeight;
        private string _Rework;
        private string _bales;
        private string _remark;
        private string _baseRef;
        private string _bundles;
        private string _NumAtCard;
        private string _postedFile;
        private double _reworkqty;
        private string _reworkseries;
        private string _reworkwarehouse;
        private double _rejectqty;
        private string _rejectwhscode;
        private DateTime _ManualDate;
        private string _BinLoaction;
        private string _QcType;
        private string _InvoicesNumber;
        private string _receiptChallanNo;
        private string _ManualQuantity;
        private string _ItemGroup;
        private string _ItemTyoe;
        private string _MaterialType;
        private string _AliasName;
        private string _WorkOrder;



        public string IGE1_FromWhsCode { get; set; }
        public string IGE1_BaseRef { get; set; }
        public int IGE1_DocEntry { get; set; }
        public string IGE1_ItemCode { get; set; }
        public string IGE1_ItemDescription { get; set; }
        public int IGE1_Quantity { get; set; }
        public string OIGE_DocNum { get; set; }
        public string OIGE_JobWorkerCode { get; set; }
        public string OIGE_JobWorkerName { get; set; }
        public string OIGE_Ref2 { get; set; }
        public int OWOR_DocEntry { get; set; }
        public string OWOR_Process { get; set; }
        public string Weight { get { return _weight; } set { _weight = value; } }
        public string TotalWeight { get { return _TotalWeight; } set { _TotalWeight = value; } }
        public string Rework { get { return _Rework; } set { _Rework = value; } }
        public DateTime ManualDate { get { return _ManualDate; } set { _ManualDate = value; } }
        public string BinLoaction { get { return _BinLoaction; } set { _BinLoaction = value; } }
        public string ItemGroup { get { return _ItemGroup; } set { _ItemGroup = value; } }
        public string Dscription { get { return _dscription; } set { _dscription = value; } }
        public string QcType { get { return _QcType; } set { _QcType = value; } }
        public string Bales { get { return _bales; } set { _bales = value; } }
        public string Remark { get { return _remark; } set { _remark = value; } }
        public string QC_Process { get { return _QC_Process; } set { _QC_Process = value; } }
        public string BbaseRef { get { return _baseRef; } set { _baseRef = value; } }
        public string ReceiptChallanNo { get { return _receiptChallanNo; } set { _receiptChallanNo = value; } }
        public string Bundles { get { return _bundles; } set { _bundles = value; } }
        public string ManualQuantity { get { return _ManualQuantity; } set { _ManualQuantity = value; } }
        public string NumAtCard { get { return _NumAtCard; } set { _NumAtCard = value; } }
        public string postedFile { get { return _postedFile; } set { { _postedFile = value; } } }
        public string ManBtchNum { get { return _ManBtchNum; } set { _ManBtchNum = value; } }
        public string U_RMTYPE { get { return _u_rmtype; } set { _u_rmtype = value; } }
        public string ItmsGrpNam { get { return _itmsgrpnam; } set { _itmsgrpnam = value; } }
        public string UserName { get { return _username; } set { _username = value; } }
        public string UserEmail { get { return _useremail; } set { _useremail = value; } }
        public string InvoicesNumber { get { return _InvoicesNumber; } set { _InvoicesNumber = value; } }
        public string U_ItemTyoe { get { return _ItemTyoe; } set { _ItemTyoe = value; } }
        public string MaterialType { get { return _MaterialType; } set { _MaterialType = value; } }
        public string AliasName { get { return _AliasName; } set { _AliasName = value; } }
        public string ItmsGrpCod { get { return _ItmsGrpCod; } set { _ItmsGrpCod = value; } }
        public string WorkOrder { get { return _WorkOrder; } set { _WorkOrder = value; } }

        public string customremarksname { get { return _customremarksname; } set { _customremarksname = value; } }
        public string heatno { get { return _heatno; } set { _heatno = value; } }
        public string DocEntry { get { return _docentry; } set { _docentry = value; } }
        public string CardCode { get { return _cardcode; } set { _cardcode = value; } }
        public string CardName { get { return _cardname; } set { _cardname = value; } }
        public string vendor { get { return _vendor; } set { _vendor = value; } }
        public string remarks { get { return _remarks; } set { _remarks = value; } }

        public string userid { get { return _userid; } set { _userid = value; } }
        public string grpodocnum { get { return _grpodocnum; } set { _grpodocnum = value; } }
        public int branchid { get { return _branchid; } set { _branchid = value; } }
        public string taxcode { get { return _taxcode; } set { _taxcode = value; } }
        public string UnitPrice { get { return _unitprice; } set { _unitprice = value; } }
        public string FromWhsCode { get { return _fromwhscode; } set { _fromwhscode = value; } }
        public docseries docseries { get { return _docseries; } set { _docseries = value; } }
        public List<ClientData> ClientData { get { return _clientdata; } set { _clientdata = value; } }
        public double reworktotal { get { return _reworktotal; } set { _reworktotal = value; } }
        public double reworkunitprice { get { return _reworkunitprice; } set { _reworkunitprice = value; } }
        public double totalquantity { get { return _totalquantity; } set { _totalquantity = value; } }
        public double totalpass { get { return _totalpass; } set { _totalpass = value; } }
        public string QC_ShortageWarehouses { get { return _shortagewhscode; } set { _shortagewhscode = value; } }
        public string QC_EXTRAQTY { get { return _extrawhscode; } set { _extrawhscode = value; } }
        public double shortageqty { get { return _shortageqty; } set { _shortageqty = value; } }
        public double holdingqty { get { return _holdingqty; } set { _holdingqty = value; } }
        public double extraqty { get { return _extraqty; } set { _extraqty = value; } }
        public double reworkqty { get { return _reworkqty; } set { _reworkqty = value; } }
        //public string scrapwhscode { get { return _scrapwhscode; } set { _scrapwhscode = value; } }
        public double scrapqty { get { return _scrapqty; } set { _scrapqty = value; } }
        public double samplerej { get { return _samplerej; } set { _samplerej = value; } }
        public double samplepass { get { return _samplepass; } set { _samplepass = value; } }
        public string QC_RejectWareHouses { get { return _rejectwhscode; } set { _rejectwhscode = value; } }
        public string QC_HoldWarehouses { get { return _holdingwhscode; } set { _holdingwhscode = value; } }
        public string QC_ExtraWherehouses { get { return _extrawhscode; } set { _extrawhscode = value; } }
        public double rejectqty { get { return _rejectqty; } set { _rejectqty = value; } }
        public string QC_PassWarehouses { get { return _passwarehouse; } set { _passwarehouse = value; } }
        public string QC_ReworkWarehouses { get { return _reworkwarehouse; } set { _reworkwarehouse = value; } }
        public double SampleQuantity { get { return _samplequantity; } set { _samplequantity = value; } }
        public double Quantity { get { return _quantity; } set { _quantity = value; } }
        public List<Parameters> Parameters { get { return _parameter; } set { _parameter = value; } }
        public int OpenQuantity { get { return _openquantity; } set { _openquantity = value; } }
        public string ItemDescription { get { return _itemdescription; } set { _itemdescription = value; } }
        public string ItemCode { get { return _itemcode; } set { _itemcode = value; } }
        public bool Checked { get { return _checked; } set { _checked = value; } }
        public string QC_PassSeries { get { return _passseries; } set { _passseries = value; } }
        public string QC_HoldSeries { get { return _holdingseries; } set { _holdingseries = value; } }
        public string QC_RejectSeries { get { return _rejectseries; } set { _rejectseries = value; } }
        public string QC_ShortageSeries { get { return _shortageseries; } set { _shortageseries = value; } }
        public string QC_ExtraSeries { get { return _extraseries; } set { _extraseries = value; } }
        public string QC_ReworkSeries { get { return _reworkseries; } set { _reworkseries = value; } }

    }
}
