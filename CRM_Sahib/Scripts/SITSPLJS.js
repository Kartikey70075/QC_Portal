
var app = angular.module('MyApp', ['ngTable', 'angucomplete-alt', 'moment-picker', 'cp.ngConfirm', 'ngFileUpload', 'ngMessages', 'directive.contextMenu']);

app.controller('MyCtrl', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {
    //wamique shaikh
    $scope.alert = function (title, msg, btnclss, typeclr) {
        $ngConfirm({
            title: title,
            content: msg,
            type: typeclr,
            typeAnimated: true,
            buttons: {
                close: {
                    text: 'OK',
                    btnClass: btnclss,
                    action: function () {
                        if (title === 'Success !!')
                            location.reload();
                    }
                }
            }
        });
    };
});
app.controller('Reporting', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {
    $scope.CreateRepoert = function () {
        try {
            $scope.StartDate = moment($scope.Report.From).format('DD-MM-YYYY');
            $scope.EndDate = moment($scope.Report.To).format('DD-MM-YYYY');
            window.open('/Home1/CreateRepoert?From=' + $scope.StartDate + '&To=' + $scope.EndDate + '&Branch=' + $scope.BranchSelected + '&ItemGroup=' + $scope.ItemGroup + '&Bin=' + $scope.BinLoaction); /*+ { qcmodeldata: data }*/        //+ '&totalp=' + $scope.d.totalpass + '&totalpas=' + $scope.totalpass           //+ '&invcnum=' + $scope.InvoicesNumber);
        }
        catch (ex) {
            console.log(ex);
        }
    };
    $scope.QC_Itemgroup = function () {
        angular.element('#loading').show();
        $http.get('/api/QCAPI/Get_Data_QC_ItemGroup').then(function (response) {
            $scope.ItemGrp = response.data.list2;
            $scope.BranchData = response.data.list3;
            angular.element('#loading').hide();
        }).catch(function (response) {
            angular.element('#loading').hide();
            $scope.alert('Alert !!', response.Message, 'btn-danger', 'red');
            console.log(response);
        });
    };
});
app.controller('QcPage', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {
    $scope.QC_Page_init = function () {
        try {
            $scope.Disable_Branch_Selection = false;
            angular.element('#loading').show();
            $scope.selecteditem = [];
            $scope.GRPOQC = {};
            $scope.Isdiable = false;
            $scope.show = false;
            $scope.today = moment().format('DD-MM-YYYY');
            $scope.GRPOQC.InspectionDate = moment();
            $http.get('/api/QCAPI/CreatQCfromGRPOINT').then(function (response) {
                $scope.OCRDdata = response.data.OCRD;
                $scope.UserName = document.getElementById("sessionUSerId").value;
                $scope.GRPOQC.Email = document.getElementById("sessionEmailId").value;
                angular.element('#loading').hide();
            }).catch(function (response) {
                angular.element('#loading').hide();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);
            });
        }
        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
    $scope.Get_Warehouses_Data_init = function () {
        try {
            $http.get('/api/QCAPI/Get_WareHouses_QCInit2?ItmsGrpCod=' + $scope.ItmsGrpCod).then(function (response) {
                $scope.QC_WareHouses = response.data.WareHouses;
                $scope.QC_PassWarehouses = response.data.QC_Warehouses_Data[0].QC_PassWarehouses;
                $scope.QC_RejectWareHouses = response.data.QC_Warehouses_Data[0].RejectWareHouse;
                $scope.QC_HoldWarehouses = response.data.QC_Warehouses_Data[0].HoldWareHouse;
                $scope.QC_ShortageWarehouses = response.data.QC_Warehouses_Data[0].ShortageWareHouse;
                $scope.QC_ExtraWarehouses = response.data.QC_Warehouses_Data[0].ExtraWarehouse;
                $scope.QC_ReworkWarehouse = response.data.QC_Warehouses_Data[0].ReworkWarehouse;
                $scope.QC_PassSeries = response.data.QC_Warehouses_Data[0].PassSeries;
                $scope.QC_RejectSeries = response.data.QC_Warehouses_Data[0].RejectSeries;
                $scope.QC_HoldSeries = response.data.QC_Warehouses_Data[0].HoldSeries;
                $scope.QC_ShortageSeries = response.data.QC_Warehouses_Data[0].ShortageSeries;
                $scope.QC_ExtraQtySeries = response.data.QC_Warehouses_Data[0].ExtraQtySeries;
                $scope.QC_ReworkSeries = response.data.QC_Warehouses_Data[0].ReworkSeries;
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);

            });
        }
        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
    $scope.AfterSelectedVedorCodeGRPOQC = function (selected) {
        angular.element('#JoworkerCode_value').val(selected.originalObject.CardCode);
        angular.element('#JoworkerName_value').val(selected.originalObject.CardName);
        /*angular.element('#Email_value').val(selected.originalObject.E_Mail);*/
        $scope.GRPOQC.VendorCode = selected.originalObject.CardCode;
        $scope.GRPOQC.VendorName = selected.originalObject.CardName;
        /*$scope.GRPOQC.Email = selected.originalObject.E_Mail;*/
        try {
            let combinedstring = $scope.GRPOQC.VendorCode + '<' + $scope.BranchSelected + '<' + $scope.QC_Process;
            $http.get('/api/QCAPI/GetGRPONumber?combstring=' + combinedstring).then(function (response) {
                $scope.GRPOData = response.data;
                //$scope.NumAtCard = response.data[0].NumAtCard;

            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);


            });
        }
        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };
    $scope.sumOfQuantity = 0;
    $scope.GRPOdocChange = function () {
        try {
            $scope.GRPOQC.DocEntry;
            $scope.GRPOQC.WhsCode = $scope.GRPOData.find(x => x.DocEntry == $scope.GRPOQC.DocEntry).WhsCode;
            if ($scope.GRPOQC.WhsCode.includes("QC")) {
                $http.get('/api/QCAPI/GetGRPORowDetails?DocEntry=' + $scope.GRPOQC.DocEntry + "&WhsCode=" + $scope.GRPOQC.WhsCode).then(function (response) {
                    if (response.data.length > 0) {
                        $scope.GRPORowItemDetails = response.data;
                        $scope.BaseRef = response.data[0].BaseRef;
                        $scope.NumAtCard = response.data[0].NumAtCard;
                        $scope.ItmsGrpCod = response.data[0].ItmsGrpCod;
                        $scope.TotalSum = 0;
                        for (var i = 0; i < $scope.GRPORowItemDetails.length; i++) {
                            $scope.sumOfQuantity += parseInt($scope.GRPORowItemDetails[i].Quantity);
                        }
                        $scope.Get_Warehouses_Data_init($scope.GRPOQC.WhsCode);
                    }
                    else {
                        $scope.alert('Success !!', 'This Grpo Number QC AlReady Done', 'btn-success', 'green');
                    }
                }).catch(function (response) {
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response);
                }); 
            }
            else {
                $scope.alert1('Alert !!', "This GRPO Item Wharehouse are Not Valid for QC !!", 'btn-danger', 'red');
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }

    };
    $scope.ItemSelected = function (ind, d) {
        try {
            if ($scope.GRPORowItemDetails[ind].IsItemSelected) {
                let arrlength = $scope.selecteditem.length;
                let b = { LineNum: $scope.GRPORowItemDetails[ind].LineNum,ItemCode: $scope.GRPORowItemDetails[ind].ItemCode, ItmsGrpNam: $scope.GRPORowItemDetails[ind].ItmsGrpNam, U_RMTYPE: $scope.GRPORowItemDetails[ind].U_RMTYPE, QCPercentage: 5, Quantity: $scope.GRPORowItemDetails[ind].Quantity, PENDINGQTY: $scope.GRPORowItemDetails[ind].PENDINGQTY, OnHand: $scope.GRPORowItemDetails[ind].OnHand, Dscription: $scope.GRPORowItemDetails[ind].Dscription, DocEntry: $scope.GRPORowItemDetails[ind].DocEntry, ItemGroupType: $scope.GRPORowItemDetails[ind].ItemGroupType, MaterialType: $scope.GRPORowItemDetails[ind].MaterialType, DocNum: $scope.GRPORowItemDetails[ind].WorkOrder, UserName: angular.element("#UserName").val(), UserEmail: angular.element("#UserEmail").val(), FromWhsCode: $scope.GRPORowItemDetails[ind].WhsCode, CardCode: $scope.GRPOQC.VendorCode, CardName: $scope.GRPOQC.VendorName, ManBtchNum: $scope.GRPORowItemDetails[ind].ManBtchNum };
                if (arrlength > 0) {
                    $scope.selecteditem.splice(arrlength, 0, b);
                }
                else {
                    $scope.selecteditem.splice(arrlength, 0, b);
                }
                let combinedstring = $scope.GRPORowItemDetails[ind].ItemCode + '<' + $scope.GRPORowItemDetails[ind].ItmsGrpNam + '<' + $scope.GRPORowItemDetails[ind].U_RMTYPE;
                $scope.Get_QC_Params(combinedstring, arrlength);

            } else {
                if ($scope.GRPORowItemDetails[ind].IsItemSelected === false) {
                    let index = $scope.selecteditem.findIndex(x => x.ItemCode === $scope.GRPORowItemDetails[ind].ItemCode);
                    $scope.selecteditem.splice(index, 1);

                }
            }
        } catch (ex) {
            console.log(ex);
        }
    }

    $scope.ShowAll = function () {
        if($scope.SampleQuantity != 0 && $scope.SampleQuantity != undefined && $scope.SampleQuantity != '') {
            $scope.show = true;
        } else {
            $scope.show = false;
        }
    }

    $scope.Get_QC_Params = function (combinedstring, index) {
        try {
            $scope.selecteditem[index].SampleQuantity = $scope.SampleQuantity;
            if ($scope.selecteditem[index].SampleQuantity == undefined || $scope.selecteditem[index].SampleQuantity === '0') {
                $scope.alert1('Alert !!', "Plz Fill Sample Quantity > 0", 'btn-danger', 'red');
            }
            else {
                $http.get('/api/QCAPI/Get_Item_Params?CombinedString=' + combinedstring)
                    .then(function (response) {
                        $scope.selecteditem[index].QC_PassWarehouses = $scope.QC_PassWarehouses;
                        $scope.selecteditem[index].QC_RejectWareHouses = $scope.QC_RejectWareHouses;
                        $scope.selecteditem[index].QC_ShortageWarehouses = $scope.QC_ShortageWarehouses;
                        $scope.selecteditem[index].QC_HoldWarehouses = $scope.QC_HoldWarehouses;
                        $scope.selecteditem[index].QC_PassSeries = $scope.QC_PassSeries;
                        $scope.selecteditem[index].QC_RejectSeries = $scope.QC_RejectSeries;
                        $scope.selecteditem[index].QC_HoldSeries = $scope.QC_HoldSeries;
                        $scope.selecteditem[index].QC_ShortageSeries = $scope.QC_ShortageSeries;
                        $scope.selecteditem[index].QC_ExtraWherehouses = $scope.QC_ExtraWarehouses;
                        $scope.selecteditem[index].QC_ExtraQtySeries = $scope.QC_ExtraQtySeries;
                        $scope.selecteditem[index].QC_ReworkWarehouses = $scope.QC_ReworkWarehouse;
                        $scope.selecteditem[index].QC_ReworkSeries = $scope.QC_ReworkSeries;
                        $scope.selecteditem[index].branchid = parseInt($scope.BranchSelected);
                        $scope.selecteditem[index].Parameters = response.data;
                        if (response.data.length == 0) {
                            $scope.alert1('Alert !!', "Plz Give parameters to this ItemCode:" + $scope.selecteditem[index].ItemCode, 'btn-danger', 'red');
                        }
                        else {
                            for (let i = 0; i < $scope.selecteditem[index].Parameters.length; i++) {
                                if ($scope.selecteditem[index].Parameters[i].Minimum.includes('.') === true) {
                                    $scope.selecteditem[index].Parameters[i].Minimum = ($scope.selecteditem[index].Parameters[i].Minimum);
                                    $scope.selecteditem[index].Parameters[i].Maximum = ($scope.selecteditem[index].Parameters[i].Maximum);

                                }
                                else {
                                    $scope.selecteditem[index].Parameters[i].Minimum = ($scope.selecteditem[index].Parameters[i].Minimum);
                                    $scope.selecteditem[index].Parameters[i].Maximum = ($scope.selecteditem[index].Parameters[i].Maximum);
                                }
                            }
                            for (let i = 0; i < $scope.selecteditem[index].Parameters.length; i++) {

                                $scope.selecteditem[index].Parameters[i].ObsArr = [];
                                for (let j = 0; j < $scope.selecteditem[index].SampleQuantity; j++) {
                                    let obj = {};

                                    obj.Observ = ($scope.selecteditem[index].Parameters[i].Minimum);
                                    $scope.selecteditem[index].Parameters[i].ObsArr.push(obj);
                                }
                            }
                        }
                    }).catch(function (response) {
                        $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                        console.log(response);
                    });

            }
        } catch (exception) {
            console.log(exception);
        }
    }
    $scope.CreateGRPOQC = function () {
        try {
            angular.element('#loading').show();
            /* var a = document.getElementById("Path").value;*/
            for (let k = 0; k < $scope.selecteditem.length; k++) {
                $scope.Parameters = $scope.selecteditem[k].Parameters;
                if ($scope.Parameters[0].U_Status == "PASS" || $scope.Parameters[0].U_Status == "REJECT") {
                    if ($scope.selecteditem[k].QC_PassWarehouses != undefined && $scope.selecteditem[k].QC_RejectWareHouses != undefined && $scope.selecteditem[k].QC_HoldWarehouses != undefined && $scope.selecteditem[k].QC_ExtraWherehouses != undefined) {
                        for (let i = 0; i < $scope.selecteditem.length; i++) {
                            $scope.selecteditem[i].Weight = $scope.Weight;
                            $scope.selecteditem[i].Bales = $scope.Bales;
                            $scope.selecteditem[i].NumAtCard = $scope.NumAtCard;
                            $scope.selecteditem[i].ReceiptChallanNo = $scope.ReceiptChallanNo;
                            $scope.selecteditem[i].ManualDate = $scope.ManualDate;
                            $scope.selecteditem[i].UserName = $scope.UserName;
                            $scope.selecteditem[i].BaseRef = $scope.BaseRef;
                            $scope.selecteditem[i].TotalWeight = $scope.TotalWeight;
                            $scope.selecteditem[i].Remarks = $scope.Remarks;
                            $scope.selecteditem[i].BinLoaction = $scope.BinLoaction;
                            $scope.selecteditem[i].QcType = $scope.QcType;
                            $scope.selecteditem[i].QcType = $scope.QcType;
                            /*$scope.selecteditem[i].postedFile = a;*/
                        }
                    }
                    else {
                        angular.element('#loading').hide();
                        $scope.alert1('Alert !!', "Plz Select WareHouses For Itemcode:" + $scope.selecteditem[k].ItemCode, 'btn-danger', 'red');
                    }
                }
                else {
                    angular.element('#loading').hide();
                    $scope.alert1('Alert !!', "Plz Select Status For Itemcode:" + $scope.selecteditem[k].ItemCode, 'btn-danger', 'red');
                }
            }
            var data = { QcItem: $scope.selecteditem };
            var newdata = $scope.selecteditem;
            $http.post('/Home1/SaveQcData', { qcmodeldata: data, Email: $scope.GRPOQC.Email })
                .then(function (response) {
                    if (response.data.startsWith("QC")) {
                        angular.element('#loading').hide();
                        $scope.AlertFun('Success !!', response.data, 'btn-success', 'green');
                    }
                    else {
                        angular.element('#loading').hide();
                        $scope.alert1('Alert !!', response.data, 'btn-danger', 'red');
                    }
                }).catch(function (response) {
                    angular.element('#loading').hide();
                    $scope.alert1('Alert !!', response.data, 'btn-danger', 'red');
                    console.log(response.data);
                });
        }
        catch (ex) {
            $scope.alert1('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
    /*---------------------------------------------Custom File------------------------------------------------------------*/
    $scope.alert = function (title, msg, btnclss, typeclr) {
        $ngConfirm({
            title: title,
            content: msg,
            type: typeclr,
            typeAnimated: true,
            buttons: {
                close: {
                    text: 'OK',
                    btnClass: btnclss,
                    action: function () {
                        if (title === 'Alert !!')
                            location.reload();
                    }
                }
            }
        });
    };
    $scope.alert1 = function (title, msg, btnclss, typeclr) {
        $ngConfirm({
            title: title,
            content: msg,
            type: typeclr,
            typeAnimated: true,
            buttons: {
                close: {
                    text: 'OK',
                    btnClass: btnclss,
                    // action: function () {
                    //    if (title === 'Alert !!')
                    //        location.reload();
                    //}
                }
            }
        });
    };
    $scope.AlertFun = function (title, msg, btnclss, typeclr) {
        $ngConfirm({
            title: title,
            content: msg,
            type: typeclr,
            typeAnimated: true,
            buttons: {
                close: {
                    text: 'OK',
                    btnClass: btnclss,
                    action: function () {
                        if (title === 'Success !!')
                            location.reload();
                    }
                    //},
                    //OK: {
                    //    text: 'Copy To Sales Order',
                    //    btnClass: 'btn-success',
                    //    action: function () {
                    //        $scope.CopyQuotationToOrder();
                    //    }
                    //}
                }
            }

        });
    };
    $scope.HideLoader = function () {
        angular.element('#loading').hide();
    };
    $scope.showloader = function () {
        angular.element('#loading').show();
    };
    /*------------------------------------------------------------------------------------------------------------------*/
});
app.controller('QcProd', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {
    $scope.ProductionQInit = function () {
        try {
            $scope.Disable_Branch_Selection = false;
            angular.element('#loading').show();
            $scope.QC_Prod_Item_SelectedData = [];
            $http.get('/api/QCAPI/Get_Data_QC_init_Prod').then(function (response) {
                $scope.BranchData = response.data;
                angular.element('#loading').hide();
            }).catch(function (response) {
                angular.element('#loading').hide();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);
            });
        }
        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
    $scope.QC_Pro_BranchSelection = function () {
        $scope.UserName = document.getElementById("sessionUSerId").value;
        if ($scope.BranchSelected !== null) {
            $scope.QC_pro_Page_After_BranchSelection();
        }
        else {
            $scope.alert('Alert !!', 'Select Branch for Qc Process', 'btn-danger', 'red');
        }
    }
    $scope.QC_pro_Page_After_BranchSelection = function () {
        angular.element('#loading').hide();
        try {
            $scope.selecteditem = [];
            $scope.GRPOQC = {};
            $scope.Isdiable = false;
            $scope.today = moment().format('DD-MM-YYYY');
            $scope.QC_InspectionDate = moment();
            $http.get('/api/QCAPI/CreatQCfromGRPOINT2').then(function (response) {
                $scope.OCRDdata = response.data.OCRD;
                $scope.ProcessListData = response.data.OIGN;
                $scope.Prod_Order_List = response.data.OIGN;
                angular.element('#loading').hide();

            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);
            });
        }
        catch (ex) {
            console.log(ex);
        }
    };
    $scope.AfterSelectedworkerCode = function (selected) {
        angular.element('#JoworkerCode_value').val(selected.originalObject.CardCode);
        angular.element('#JoworkerName_value').val(selected.originalObject.CardName);
        $scope.GRPOQC.VendorCode = selected.originalObject.CardCode;
        $scope.GRPOQC.VendorName = selected.originalObject.CardName;
        $scope.GRPOQC.Email = selected.originalObject.E_Mail;
        $scope.Process = $scope.QC_Process;

        try {
            let combinedstring = $scope.GRPOQC.VendorCode + '<' + $scope.BranchSelected + '<' + $scope.Process;
            $http.get('/api/QCAPI/DocNumber?combstring=' + combinedstring).then(function (response) {
                $scope.GRPOData = response.data;
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);

            });
        }
        catch (ex) {
            console.log(ex);
        }

    };
    $scope.QC_Prod_Item_Selected = function (ind, d) {
        try {
            if ($scope.GRPORowItemDetails[ind].QCPROD_IsItemSelected) {
                let arrlength = $scope.QC_Prod_Item_SelectedData.length;
                //let b = { IGE1_BaseRef: $scope.QC_Prod_ItemsList[ind].IGE1_BaseRef, IGE1_DocEntry: $scope.QC_Prod_ItemsList[ind].IGE1_DocEntry, IGE1_ItemCode: $scope.QC_Prod_ItemsList[ind].IGE1_ItemCode, IGE1_ItemDescription: $scope.QC_Prod_ItemsList[ind].IGE1_ItemDescription, IGE1_Quantity: $scope.QC_Prod_ItemsList[ind].IGE1_Quantity, OIGE_DocNum: $scope.QC_Prod_ItemsList[ind].OIGE_DocNum, OIGE_JobWorkerCode: $scope.QC_Prod_ItemsList[ind].OIGE_JobWorkerCode, OIGE_JobWorkerName: $scope.QC_Prod_ItemsList[ind].OIGE_JobWorkerName, OIGE_Ref2: $scope.QC_Prod_ItemsList[ind].OIGE_Ref2, OWOR_DocEntry: $scope.QC_Prod_ItemsList[ind].OWOR_DocEntry, OWOR_Process: $scope.QC_Prod_ItemsList[ind].OWOR_Process, IGE1_FromWhsCode: $scope.QC_Prod_ItemsList[ind].IGE1_FromWhsCode };
                let b = { ItemCode: $scope.GRPORowItemDetails[ind].ItemCode, Dscription: $scope.GRPORowItemDetails[ind].Dscription, Quantity: $scope.GRPORowItemDetails[ind].Quantity, PENDINGQTY: $scope.GRPORowItemDetails[ind].PENDINGQTY, CmpltQty: $scope.GRPORowItemDetails[ind].CmpltQty, DocEntry: $scope.GRPORowItemDetails[ind].DocEntry, MaterialType: $scope.GRPORowItemDetails[ind].MaterialType, AliasName: $scope.GRPORowItemDetails[ind].AliasName, UserName: angular.element("#UserName").val(), UserEmail: angular.element("#UserEmail").val(), FromWhsCode: $scope.GRPORowItemDetails[ind].WhsCode, CardCode: $scope.GRPOQC.VendorCode, WorkOrder: $scope.GRPORowItemDetails[ind].Ref2, CardName: $scope.GRPOQC.VendorName, ManBtchNum: $scope.GRPORowItemDetails[ind].ManBtchNum, ItmsGrpNam: $scope.GRPORowItemDetails[ind].ItmsGrpNam };
                let ItmCode = $scope.GRPORowItemDetails[ind].ItemCode;
                if (arrlength > 0) {

                    $scope.QC_Prod_Item_SelectedData.splice(arrlength, 0, b);
                }
                else {
                    $scope.QC_Prod_Item_SelectedData.splice(arrlength, 0, b);
                }
                $scope.Get_QC_Prod_Parameter(ItmCode, arrlength);
            } else {
                if ($scope.QC_Prod_ItemsList[ind].QCPROD_IsItemSelected === false) {
                    let index = $scope.QC_Prod_Item_SelectedData.findIndex(x => x.ItemCode === $scope.QC_Prod_ItemsList[ind].ItemCode);
                    $scope.QC_Prod_Item_SelectedData.splice(index, 1);

                }
            }

        } catch (ex) {
            console.log(ex);
        }
    }
    $scope.Get_QC_Prod_Parameter = function (ItemCode, index) {
        try {
            $http.get('/api/QCAPI/Get_Prod_Item_Parameter?ItemCode=' + ItemCode)
                .then(function (response) {

                    if (response.data === null || response.data.length <= 0) {
                        alert("No data available");
                        return false;
                    }
                    var len = $scope.QC_Prod_Item_SelectedData.length;
                    $scope.QC_Prod_Item_SelectedData[index].QC_PassWarehouses = $scope.QC_PassWarehouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_RejectWareHouses = $scope.QC_RejectWareHouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ShortageWarehouses = $scope.QC_ShortageWarehouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_HoldWarehouses = $scope.QC_HoldWarehouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ExtraWarehouses = $scope.QC_ExtraWarehouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ReworkWarehouses = $scope.QC_ReworkWarehouse;
                    $scope.QC_Prod_Item_SelectedData[index].QC_PassSeries = $scope.QC_PassSeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_RejectSeries = $scope.QC_RejectSeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_HoldSeries = $scope.QC_HoldSeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ExtraQtySeries = $scope.ExtraQtySeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ShortageSeries = $scope.QC_ShortageSeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ReworkSeries = $scope.QC_ReworkSeries;
                    $scope.QC_Prod_Item_SelectedData[index].branchid = parseInt($scope.BranchSelected);
                    console.log(response.data);
                    $scope.QC_Prod_Item_SelectedData[index].Parameters = response.data.params_data;

                    if ($scope.QC_Process == "Dying") {
                        $scope.QC_Prod_Item_SelectedData[index].SampleQuantity = $scope.QC_Prod_Item_SelectedData.SampleQuantity;
                    }
                    else {
                        $scope.QC_Prod_Item_SelectedData[index].SampleQuantity = $scope.QC_Prod_Item_SelectedData.SampleQuantity;
                    }

                    var regExp = /[a-zA-Z]/g;
                    for (let i = 0; i < $scope.QC_Prod_Item_SelectedData[index].Parameters.length; i++) {

                        if (regExp.test($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum)) {
                            $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum).toString();
                            $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum).toString();
                        }
                        else {
                            if ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum.includes('.') === true) {
                                $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum);
                                $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum);
                            }
                            else {
                                $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum);
                                $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum);
                            }
                        }
                    }

                    for (let i = 0; i < $scope.QC_Prod_Item_SelectedData[index].Parameters.length; i++) {
                        $scope.QC_Prod_Item_SelectedData[index].Parameters[i].ObsArr = [];
                        for (let j = 0; j < $scope.QC_Prod_Item_SelectedData[index].SampleQuantity; j++) {
                            let obj = {};
                            obj.Observ = $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum;
                            $scope.QC_Prod_Item_SelectedData[index].Parameters[i].ObsArr.push(obj);
                        }
                    }
                }).catch(function (response) {
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response);
                });



        } catch (exception) {
            console.log(exception);
        }
    }
    $scope.OnPOLinkChange = function (QC_Prod_Order) {
        try {
            let VendorCode = $scope.GRPOQC.VendorCode;
            for (let i = 0; i < $scope.GRPOData.length; i++) {
                if ($scope.GRPOData[i].WorkOrder == QC_Prod_Order) {
                    $scope.InvoicesNumber = $scope.GRPOData[i].U_WO;
                    $scope.ReceiptChallanNo = $scope.GRPOData[i].U_VH;
                }
            }
            $http.get('/api/QCAPI/OnPOLinkChangeProd?QC_Prod_Order=' + QC_Prod_Order + "&VendorCode=" + VendorCode).then(function (response) {
                $scope.GRPORowItemDetails = response.data;
                $scope.sumOfQuantity = 0;
                for (var i = 0; i < $scope.GRPORowItemDetails.length; i++) {
                    if ($scope.GRPORowItemDetails[i].Quantity >= 0 && $scope.GRPORowItemDetails[i].Quantity <= 100) {
                        $scope.GRPORowItemDetails[i].Checking = '20';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 100 && $scope.GRPORowItemDetails[i].Quantity <= 500) {
                        $scope.GRPORowItemDetails[i].Checking = '100';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 500 && $scope.GRPORowItemDetails[i].Quantity <= 1000) {
                        $scope.GRPORowItemDetails[i].Checking = '150';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 1000 && $scope.GRPORowItemDetails[i].Quantity <= 2000) {
                        $scope.GRPORowItemDetails[i].Checking = '200';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity > 2000) {
                        $scope.GRPORowItemDetails[i].Checking = '30';
                    }
                    $scope.sumOfQuantity += $scope.GRPORowItemDetails[i].Quantity;
                }
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);
            });
        }
        catch (ex) {
            console.log(ex);
        }
    };
    $scope.OnPOLinkChangeProd = function (Doc_No) {
        try {
            let VendorCode = $scope.GRPOQC.VendorCode;
            for (let i = 0; i < $scope.GRPOData.length; i++) {
                if ($scope.GRPOData[i].DocNum == Doc_No) {
                    $scope.QC_Prod_Order = $scope.GRPOData[i].WorkOrder
                    $scope.InvoicesNumber = $scope.GRPOData[i].U_WO;
                    $scope.ReceiptChallanNo = $scope.GRPOData[i].U_VH;
                }
            }

            $http.get('/api/QCAPI/OnPOLinkChangeProd?QC_Prod_Order=' + $scope.QC_Prod_Order + "&VendorCode=" + $scope.GRPOQC.VendorCode + "&DocNum=" + Doc_No).then(function (response) {
                $scope.GRPORowItemDetails = response.data;
                $scope.sumOfQuantity = 0;
                for (var i = 0; i < $scope.GRPORowItemDetails.length; i++) {
                    if ($scope.GRPORowItemDetails[i].Quantity >= 0 && $scope.GRPORowItemDetails[i].Quantity <= 100) {
                        $scope.GRPORowItemDetails[i].Checking = '20';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 100 && $scope.GRPORowItemDetails[i].Quantity <= 500) {
                        $scope.GRPORowItemDetails[i].Checking = '100';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 500 && $scope.GRPORowItemDetails[i].Quantity <= 1000) {
                        $scope.GRPORowItemDetails[i].Checking = '150';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 1000 && $scope.GRPORowItemDetails[i].Quantity <= 2000) {
                        $scope.GRPORowItemDetails[i].Checking = '200';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity > 2000) {
                        $scope.GRPORowItemDetails[i].Checking = '30';
                    }
                    $scope.sumOfQuantity += $scope.GRPORowItemDetails[i].Quantity;
                }
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);
            });
        }
        catch (ex) {
            console.log(ex);
        }
    };
    $scope.Get_Warehouses_Data_init1 = function () {
        try {
            $scope.Disable_Branch_Selection = true;
            let Process = $scope.QC_Process;
            $http.get('/api/QCAPI/Get_WareHouses_QCInitProd?Branch=' + $scope.BranchSelected + "&process=" + Process).then(function (response) {
                $scope.QC_WareHouses = response.data.WareHouses;
                $scope.QC_PassWarehouses = response.data.QC_Warehouses_Data[0].QC_PassWarehouses;
                $scope.QC_RejectWareHouses = response.data.QC_Warehouses_Data[0].RejectWareHouse;
                $scope.QC_HoldWarehouses = response.data.QC_Warehouses_Data[0].HoldWareHouse;
                $scope.QC_ShortageWarehouses = response.data.QC_Warehouses_Data[0].ShortageWareHouse;
                $scope.QC_ExtraWarehouses = response.data.QC_Warehouses_Data[0].ExtraWarehouse;
                $scope.QC_ReworkWarehouse = response.data.QC_Warehouses_Data[0].ReworkWarehouse;
                $scope.QC_PassSeries = response.data.QC_Warehouses_Data[0].PassSeries;
                $scope.QC_RejectSeries = response.data.QC_Warehouses_Data[0].RejectSeries;
                $scope.QC_HoldSeries = response.data.QC_Warehouses_Data[0].HoldSeries;
                $scope.QC_ShortageSeries = response.data.QC_Warehouses_Data[0].ShortageSeries;
                $scope.QC_ExtraQtySeries = response.data.QC_Warehouses_Data[0].ExtraQtySeries;
                $scope.QC_ReworkSeries = response.data.QC_Warehouses_Data[0].ReworkSeries;
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);

            });
        }
        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
    $scope.Create_Prod_QC_PageSave = function () {
        try {
            angular.element('#loading').show();
            for (let i = 0; i < $scope.QC_Prod_Item_SelectedData.length; i++) {
                $scope.QC_Prod_Item_SelectedData[i].InvoicesNumber = $scope.InvoicesNumber;
                $scope.QC_Prod_Item_SelectedData[i].BinLoaction = $scope.BinLoaction;
                $scope.QC_Prod_Item_SelectedData[i].ReceiptChallanNo = $scope.ReceiptChallanNo;
                $scope.QC_Prod_Item_SelectedData[i].Bales = $scope.Bales;
                $scope.QC_Prod_Item_SelectedData[i].Remark = $scope.Remark;
                $scope.QC_Prod_Item_SelectedData[i].ManualDate = $scope.ManualDate;
                $scope.QC_Prod_Item_SelectedData[i].QC_Process = $scope.QC_Process;
                $scope.QC_Prod_Item_SelectedData[i].Weight = $scope.Weight;
                $scope.QC_Prod_Item_SelectedData[i].TotalWeight = $scope.TotalWeight;
                $scope.QC_Prod_Item_SelectedData[i].Remarks = $scope.Remarks;
                $scope.QC_Prod_Item_SelectedData[i].QcType = $scope.QcType;
                $scope.QC_Prod_Item_SelectedData[i].ManualQuantity = $scope.ManualQuantity;
                $scope.QC_Prod_Item_SelectedData[i].UserName = $scope.UserName;

            }
            /*=======New Code By wamique A. on 17 Feb 2021*/
            var data = { QcItem: $scope.QC_Prod_Item_SelectedData };
            var newdata = $scope.QC_Prod_Item_SelectedData;

            $http.post('/Home1/SaveQc_ProdData', { qcprodmodeldata: data, Email: $scope.GRPOQC.Email })
                .then(function (response) {

                    if (response.data.startsWith("QC")) {
                        angular.element('#loading').hide();

                        $scope.alert('Success !!', response.data, 'btn-success', 'green');

                    }
                    else {
                        angular.element('#loading').hide();
                        $scope.alert('Alert', response.data, 'btn-danger', 'red');
                    }
                }).catch(function (response) {
                    angular.element('#loading').hide();
                    $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                    console.log(response.data);
                });
        }
        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
});
app.controller('QcPageDying', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {
    $scope.DAINGQInit = function () {
        try {
            $scope.Disable_Branch_Selection = false;
            angular.element('#loading').show();
            $scope.QC_Prod_Item_SelectedData = [];
            $http.get('/api/QCAPI/Get_Data_QC_init_Dying').then(function (response) {
                $scope.BranchData = response.data;
                angular.element('#loading').hide();
            }).catch(function (response) {
                angular.element('#loading').hide();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);
            });
        }

        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
    $scope.QC_Pro_BranchSelection = function () {
        $scope.UserName = document.getElementById("sessionUSerId").value;
        if ($scope.BranchSelected !== null) {
            $scope.QC_pro_Page_After_BranchSelection();
        }
        else {
            $scope.alert('Alert !!', 'Select Branch for Qc Process', 'btn-danger', 'red');
        }
    }
    $scope.QC_pro_Page_After_BranchSelection = function () {
        angular.element('#loading').hide();
        try {
            $scope.selecteditem = [];
            $scope.GRPOQC = {};
            $scope.Isdiable = false;
            $scope.today = moment().format('DD-MM-YYYY');
            $scope.QC_InspectionDate = moment();
            $http.get('/api/QCAPI/CreatQCfromGRPOINTDying').then(function (response) {
                $scope.OCRDdata = response.data.OCRD;
                $scope.ProcessListData = response.data.OIGN;
                $scope.Prod_Order_List = response.data.OIGN;
                //$scope.QC_Process = response.data.U_Operation;
                $scope.QC_Process = "Dying";
                $scope.Get_Warehouses_Data_init();
                angular.element('#loading').hide();
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);
            });
        }
        catch (ex) {
            console.log(ex);
        }
    };
    $scope.OnPOLinkChange = function (Doc_Number) {
        try {
            let VendorCode = $scope.GRPOQC.VendorCode;
            for (let i = 0; i < $scope.GRPOData.length; i++) {
                if ($scope.GRPOData[i].DocNum == Doc_Number) {
                    $scope.QC_Prod_Order = $scope.GRPOData[i].WorkOrder
                    $scope.InvoicesNumber = $scope.GRPOData[i].U_WO;
                    $scope.ReceiptChallanNo = $scope.GRPOData[i].U_VH;
                }
            }

            $http.get('/api/QCAPI/OnPOLinkChangeDying?QC_Prod_Order=' + $scope.QC_Prod_Order + "&VendorCode=" + $scope.GRPOQC.VendorCode + "&DocNum=" + Doc_Number).then(function (response) {
                $scope.GRPORowItemDetails = response.data;
                //console.log($scope.GRPORowItemDetails);
                $scope.sumOfQuantity = 0;
                // Apply condition for Checking by Dilshad a. on 11 Feb 2021
                for (var i = 0; i < $scope.GRPORowItemDetails.length; i++) {
                    if ($scope.GRPORowItemDetails[i].Quantity >= 0 && $scope.GRPORowItemDetails[i].Quantity <= 100) {
                        $scope.GRPORowItemDetails[i].Checking = '20';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 100 && $scope.GRPORowItemDetails[i].Quantity <= 500) {
                        $scope.GRPORowItemDetails[i].Checking = '100';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 500 && $scope.GRPORowItemDetails[i].Quantity <= 1000) {
                        $scope.GRPORowItemDetails[i].Checking = '150';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity >= 1000 && $scope.GRPORowItemDetails[i].Quantity <= 2000) {
                        $scope.GRPORowItemDetails[i].Checking = '200';
                    }
                    else if ($scope.GRPORowItemDetails[i].Quantity > 2000) {
                        $scope.GRPORowItemDetails[i].Checking = '30';
                    }
                    $scope.sumOfQuantity += $scope.GRPORowItemDetails[i].Quantity;
                }
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);
            });
        }
        catch (ex) {
            console.log(ex);
        }
    };
    $scope.Get_Warehouses_Data_init = function () {
        try {
            $scope.Disable_Branch_Selection = true;
            let Process = $scope.QC_Process;
            $http.get('/api/QCAPI/Get_WareHouses_QCInitDying?Branch=' + $scope.BranchSelected + "&process=" + Process).then(function (response) {
                $scope.QC_WareHouses = response.data.WareHouses;
                $scope.QC_PassWarehouses = response.data.QC_Warehouses_Data[0].QC_PassWarehouses;
                $scope.QC_RejectWareHouses = response.data.QC_Warehouses_Data[0].RejectWareHouse;
                $scope.QC_HoldWarehouses = response.data.QC_Warehouses_Data[0].HoldWareHouse;
                $scope.QC_ShortageWarehouses = response.data.QC_Warehouses_Data[0].ShortageWareHouse;
                $scope.QC_ExtraWarehouses = response.data.QC_Warehouses_Data[0].ExtraWarehouse;
                $scope.QC_ReworkWarehouse = response.data.QC_Warehouses_Data[0].ReworkWarehouse;
                $scope.QC_PassSeries = response.data.QC_Warehouses_Data[0].PassSeries;
                $scope.QC_RejectSeries = response.data.QC_Warehouses_Data[0].RejectSeries;
                $scope.QC_HoldSeries = response.data.QC_Warehouses_Data[0].HoldSeries;
                $scope.QC_ShortageSeries = response.data.QC_Warehouses_Data[0].ShortageSeries;
                $scope.QC_ExtraQtySeries = response.data.QC_Warehouses_Data[0].ExtraQtySeries;
                $scope.QC_ReworkSeries = response.data.QC_Warehouses_Data[0].ReworkSeries;
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);

            });
        }
        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
    $scope.AfterSelectedworkerCode = function (selected) {
        angular.element('#JoworkerCode_value').val(selected.originalObject.CardCode);
        angular.element('#JoworkerName_value').val(selected.originalObject.CardName);
        $scope.GRPOQC.VendorCode = selected.originalObject.CardCode;
        $scope.GRPOQC.VendorName = selected.originalObject.CardName;
        $scope.GRPOQC.Email = selected.originalObject.E_Mail;
        $scope.Process = $scope.QC_Process;
        try {
            let combinedstring = $scope.GRPOQC.VendorCode + '<' + $scope.BranchSelected + '<' + $scope.Process;
            $http.get('/api/QCAPI/DocNumberDying?combstring=' + combinedstring).then(function (response) {
                $scope.GRPOData = response.data;
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response);

            });
        }
        catch (ex) {
            console.log(ex);
        }

    };
    $scope.QC_Dying_Item_Selected = function (ind, d) {
        try {
            if ($scope.GRPORowItemDetails[ind].QCPROD_IsItemSelected) {
                let arrlength = $scope.QC_Prod_Item_SelectedData.length;
                //let b = { IGE1_BaseRef: $scope.QC_Prod_ItemsList[ind].IGE1_BaseRef, IGE1_DocEntry: $scope.QC_Prod_ItemsList[ind].IGE1_DocEntry, IGE1_ItemCode: $scope.QC_Prod_ItemsList[ind].IGE1_ItemCode, IGE1_ItemDescription: $scope.QC_Prod_ItemsList[ind].IGE1_ItemDescription, IGE1_Quantity: $scope.QC_Prod_ItemsList[ind].IGE1_Quantity, OIGE_DocNum: $scope.QC_Prod_ItemsList[ind].OIGE_DocNum, OIGE_JobWorkerCode: $scope.QC_Prod_ItemsList[ind].OIGE_JobWorkerCode, OIGE_JobWorkerName: $scope.QC_Prod_ItemsList[ind].OIGE_JobWorkerName, OIGE_Ref2: $scope.QC_Prod_ItemsList[ind].OIGE_Ref2, OWOR_DocEntry: $scope.QC_Prod_ItemsList[ind].OWOR_DocEntry, OWOR_Process: $scope.QC_Prod_ItemsList[ind].OWOR_Process, IGE1_FromWhsCode: $scope.QC_Prod_ItemsList[ind].IGE1_FromWhsCode };
                let b = { ItemCode: $scope.GRPORowItemDetails[ind].ItemCode, Dscription: $scope.GRPORowItemDetails[ind].Dscription, Quantity: $scope.GRPORowItemDetails[ind].Quantity, PENDINGQTY: $scope.GRPORowItemDetails[ind].PENDINGQTY, ManualQuantity: $scope.GRPORowItemDetails[ind].ManualQuantity, PlannedQty: $scope.GRPORowItemDetails[ind].PlannedQty, CmpltQty: $scope.GRPORowItemDetails[ind].CmpltQty, DocEntry: $scope.GRPORowItemDetails[ind].DocEntry, MaterialType: $scope.GRPORowItemDetails[ind].MaterialType, AliasName: $scope.GRPORowItemDetails[ind].AliasName, UserName: angular.element("#UserName").val(), UserEmail: angular.element("#UserEmail").val(), FromWhsCode: $scope.GRPORowItemDetails[ind].WhsCode, CardCode: $scope.GRPOQC.VendorCode, WorkOrder: $scope.GRPORowItemDetails[ind].Ref2, CardName: $scope.GRPOQC.VendorName, ManBtchNum: $scope.GRPORowItemDetails[ind].ManBtchNum, ItmsGrpNam: $scope.GRPORowItemDetails[ind].ItmsGrpNam };
                let ItmCode = $scope.GRPORowItemDetails[ind].ItemCode;
                if ($scope.QC_Process == "Dying") {
                    $scope.QC_Prod_Item_SelectedData.QC_PassWarehouses = "DYING-BCM";
                }
                if (arrlength > 0) {

                    $scope.QC_Prod_Item_SelectedData.splice(arrlength, 0, b);
                }
                else {
                    $scope.QC_Prod_Item_SelectedData.splice(arrlength, 0, b);
                }
                $scope.Get_QC_Dying_Parameter(ItmCode, arrlength);
            } else {
                if ($scope.QC_Prod_ItemsList[ind].QCPROD_IsItemSelected === false) {
                    let index = $scope.QC_Prod_Item_SelectedData.findIndex(x => x.ItemCode === $scope.QC_Prod_ItemsList[ind].ItemCode);
                    $scope.QC_Prod_Item_SelectedData.splice(index, 1);

                }

            }


        } catch (ex) {
            console.log(ex);
        }
    }
    $scope.Get_QC_Dying_Parameter = function (ItemCode, index) {
        try {
            $http.get('/api/QCAPI/Get_Dying_Item_Parameter?ItemCode=' + ItemCode)
                .then(function (response) {
                    if (response.data === null || response.data.length <= 0) {
                        alert("No data available");
                        return false;
                    }
                    var len = $scope.QC_Prod_Item_SelectedData.length;
                    $scope.QC_Prod_Item_SelectedData[index].QC_PassWarehouses = $scope.QC_PassWarehouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_RejectWareHouses = $scope.QC_RejectWareHouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ShortageWarehouses = $scope.QC_ShortageWarehouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_HoldWarehouses = $scope.QC_HoldWarehouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ExtraWarehouses = $scope.QC_ExtraWarehouses;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ReworkWarehouses = $scope.QC_ReworkWarehouse;
                    $scope.QC_Prod_Item_SelectedData[index].QC_PassSeries = $scope.QC_PassSeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_RejectSeries = $scope.QC_RejectSeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_HoldSeries = $scope.QC_HoldSeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ExtraQtySeries = $scope.ExtraQtySeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ShortageSeries = $scope.QC_ShortageSeries;
                    $scope.QC_Prod_Item_SelectedData[index].QC_ReworkSeries = $scope.QC_ReworkSeries;
                    $scope.QC_Prod_Item_SelectedData[index].branchid = parseInt($scope.BranchSelected);
                    console.log(response.data);
                    $scope.QC_Prod_Item_SelectedData[index].Parameters = response.data.params_data;
                    if ($scope.QC_Process == "Dying") {
                        $scope.QC_Prod_Item_SelectedData[index].SampleQuantity = $scope.QC_Prod_Item_SelectedData.SampleQuantity;
                    }
                    else {
                        $scope.QC_Prod_Item_SelectedData[index].SampleQuantity = $scope.QC_Prod_Item_SelectedData.SampleQuantity;
                    }


                    // extra code just for test  here i am going to convert the string values to double or int format //
                    var regExp = /[a-zA-Z]/g;
                    for (let i = 0; i < $scope.QC_Prod_Item_SelectedData[index].Parameters.length; i++) {
                        if (regExp.test($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum)) {
                            $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum).toString();
                            $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum).toString();
                        }
                        else {
                            if ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum.includes('.') === true) {
                                $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum);
                                $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum);
                            }
                            else {
                                $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum);
                                $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum = ($scope.QC_Prod_Item_SelectedData[index].Parameters[i].Maximum);
                            }
                        }
                    }

                    for (let i = 0; i < $scope.QC_Prod_Item_SelectedData[index].Parameters.length; i++) {

                        $scope.QC_Prod_Item_SelectedData[index].Parameters[i].ObsArr = [];
                        for (let j = 0; j < $scope.QC_Prod_Item_SelectedData[index].SampleQuantity; j++) {
                            let obj = {};
                            obj.Observ = $scope.QC_Prod_Item_SelectedData[index].Parameters[i].Minimum;
                            $scope.QC_Prod_Item_SelectedData[index].Parameters[i].ObsArr.push(obj);
                        }

                    }
                }).catch(function (response) {
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response);

                });
        } catch (exception) {
            console.log(exception);
        }
    }
    $scope.Create_dyieng_QC_PageSave = function () {
        try {
            angular.element('#loading').show();
            var a = document.getElementById("Path").value;
            for (let i = 0; i < $scope.QC_Prod_Item_SelectedData.length; i++) {
                $scope.QC_Prod_Item_SelectedData[i].InvoicesNumber = $scope.InvoicesNumber;
                $scope.QC_Prod_Item_SelectedData[i].BinLoaction = $scope.BinLoaction;
                $scope.QC_Prod_Item_SelectedData[i].ReceiptChallanNo = $scope.ReceiptChallanNo;
                $scope.QC_Prod_Item_SelectedData[i].Bales = $scope.Bales;
                $scope.QC_Prod_Item_SelectedData[i].Remark = $scope.Remark;
                $scope.QC_Prod_Item_SelectedData[i].ManualDate = $scope.ManualDate;
                $scope.QC_Prod_Item_SelectedData[i].QC_Process = $scope.QC_Process;
                $scope.QC_Prod_Item_SelectedData[i].QcType = $scope.QcType;
                $scope.QC_Prod_Item_SelectedData[i].Weight = $scope.Weight;
                $scope.QC_Prod_Item_SelectedData[i].TotalWeight = $scope.TotalWeight;
                $scope.QC_Prod_Item_SelectedData[i].Remarks = $scope.Remarks;
                $scope.QC_Prod_Item_SelectedData[i].UserName = $scope.UserName;
                $scope.QC_Prod_Item_SelectedData[i].postedFile = a;

            }

            var data = { QcItem: $scope.QC_Prod_Item_SelectedData };
            var newdata = $scope.QC_Prod_Item_SelectedData;

            $http.post('/Home1/SaveQc_DayengData', { qcprodmodeldata: data, Email: $scope.GRPOQC.Email })
                .then(function (response) {
                    if (response.data.startsWith("QC")) {
                        angular.element('#loading').hide();
                        $scope.alert('Success !!', response.data, 'btn-success', 'green');
                    }
                    else {
                        angular.element('#loading').hide();
                        $scope.alert('Alert', response.data, 'btn-danger', 'red');
                    }

                }).catch(function (response) {
                    angular.element('#loading').hide();
                    $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                    console.log(response.data);
                });
        }

        catch (ex) {
            $scope.alert('Alert', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    }
});
/*===========================================END Customer Vender ==============================*/
//-----------------------------------Directive For app----------------------------
app.directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text.replace(/[^0-9]/g, '');

                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return undefined;
            }
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
}).directive('amountOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text.replace(/[^0-9.-]/g, '');

                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return undefined;
            }
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
}).directive('compareTo', function () {
    return {
        require: 'ngModel',
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue === scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
}).directive("ngRightClick", function ($parse) {
    return function (scope, element, attrs) {
        var fn = $parse(attrs.ngRightClick);
        element.bind("contextmenu", function (event) {
            scope.$apply(function () {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
    };
}).filter('unique', function () {
    return function (collection, keyname) {
        var output = [],
            keys = [];
        angular.forEach(collection, function (item) {
            var key = item[keyname];
            if (keys.indexOf(key) === -1) {
                keys.push(key);
                output.push(item);
            }
        });
        return output;
    };
});

(function (angular) {
    var ngContextMenu = angular.module("directive.contextMenu", []);
    ngContextMenu.directive("cellHighlight", function () {
        return {
            restrict: "C",
            link: function postLink(scope, iElement, iAttrs) {
                iElement.find("td").mouseover(function () {
                    $(this).parent("tr").css("opacity", "0.7");
                }).mouseout(function () {
                    $(this).parent("tr").css("opacity", "1.0");
                });
            }
        };
    });

    ngContextMenu.directive("context", [
        function () {
            return {
                restrict: 'A',
                scope: "@&",
                compile: function compile(tElement, tAttrs, transclude) {
                    return {
                        post: function postLink(scope, iElement, iAttrs, controller) {
                            var ul = $('#' + iAttrs.context),
                                last = null;
                            ul.css({
                                "display": "none"
                            });
                            $(iElement).bind("contextmenu", function (event) {
                                event.preventDefault();
                                ul.css({
                                    position: "fixed",
                                    display: "block",
                                    left: event.clientX + "px",
                                    top: event.clientY + "px"
                                });
                                last = event.timeStamp;
                            });

                            $(document).click(function (event) {
                                var target = $(event.target);
                                if (!target.is(".popover") && !target.parents().is(".popover")) {
                                    if (last === event.timeStamp)
                                        return;
                                    ul.css({
                                        "display": "none"
                                    });
                                }
                            });
                        }
                    };
                }
            };
        }
    ]);
})(window.angular);

$(window).on("unload", function (e) {
    localStorage.removeItem('bpcode');
    localStorage.removeItem('bpname');
    //console.log(localStorage.bpcode);
});

(function (angular) {
    "use strict";

    //function mainCtrl() {
    //    var vm = this;
    //    vm.numericValue = 12345678;
    //}

    function sgNumberInput($filter, $locale) {
        //#region helper methods
        function getCaretPosition(inputField) {
            // Initialize
            var position = 0;
            // IE Support
            if (document.selection) {
                inputField.focus();
                // To get cursor position, get empty selection range
                var emptySelection = document.selection.createRange();
                // Move selection start to 0 position
                emptySelection.moveStart('character', -inputField.value.length);
                // The caret position is selection length
                position = emptySelection.text.length;
            }
            else if (inputField.selectionStart || inputField.selectionStart === 0) {
                position = inputField.selectionStart;
            }
            return position;
        }
        function setCaretPosition(inputElement, position) {
            if (inputElement.createTextRange) {
                var range = inputElement.createTextRange();
                range.move('character', position);
                range.select();
            }
            else {
                if (inputElement.selectionStart) {
                    inputElement.focus();
                    inputElement.setSelectionRange(position, position);
                }
                else {
                    inputElement.focus();
                }
            }
        }
        function countNonNumericChars(value) {
            return (value.match(/[^a-z0-9]/gi) || []).length;
        }
        //#endregion helper methods



        return {
            require: "ngModel",
            restrict: "A",
            link: function ($scope, element, attrs, ctrl) {
                var fractionSize = parseInt(attrs['fractionSize']) || 0;
                var numberFilter = $filter('number');
                //format the view value
                ctrl.$formatters.push(function (modelValue) {
                    var retVal = numberFilter(modelValue, fractionSize);
                    var isValid = !isNaN(modelValue);
                    ctrl.$setValidity(attrs.name, isValid);
                    return retVal;
                });
                //parse user's input
                ctrl.$parsers.push(function (viewValue) {
                    var caretPosition = getCaretPosition(element[0]), nonNumericCount = countNonNumericChars(viewValue);
                    viewValue = viewValue || '';
                    //Replace all possible group separators
                    var trimmedValue = viewValue.trim().replace(/,/g, '').replace(/`/g, '').replace(/'/g, '').replace(/\u00a0/g, '').replace(/ /g, '');
                    //If numericValue contains more decimal places than is allowed by fractionSize, then numberFilter would round the value up
                    //Thus 123.109 would become 123.11
                    //We do not want that, therefore I strip the extra decimal numbers
                    var separator = $locale.NUMBER_FORMATS.DECIMAL_SEP;
                    var arr = trimmedValue.split(separator);
                    var decimalPlaces = arr[1];
                    if (decimalPlaces !== null && decimalPlaces.length > fractionSize) {
                        //Trim extra decimal places
                        decimalPlaces = decimalPlaces.substring(0, fractionSize);
                        trimmedValue = arr[0] + separator + decimalPlaces;
                    }
                    var numericValue = parseFloat(trimmedValue);
                    var isEmpty = numericValue === null || viewValue.trim() === "";
                    var isRequired = attrs.required || false;
                    var isValid = true;
                    if (isEmpty && isRequired || !isEmpty && isNaN(numericValue)) {
                        isValid = false;
                    }
                    ctrl.$setValidity(attrs.name, isValid);
                    if (!isNaN(numericValue) && isValid) {
                        var newViewValue = numberFilter(numericValue, fractionSize);
                        element.val(newViewValue);
                        var newNonNumbericCount = countNonNumericChars(newViewValue);
                        var diff = newNonNumbericCount - nonNumericCount;
                        var newCaretPosition = caretPosition + diff;
                        if (nonNumericCount === 0 && newCaretPosition > 0) newCaretPosition--;
                        setCaretPosition(element[0], newCaretPosition);
                    }
                    return !isNaN(numericValue) ? numericValue : null;
                });
            } //end of link function
        };
    }

    sgNumberInput.$inject = ["$filter", "$locale"];

    //angular
    //    .module("a", [])
    //    .controller("mainCtrl", mainCtrl)
    //    .directive("sgNumberInput", sgNumberInput);
    app.directive("sgNumberInput", sgNumberInput);
})(angular);
