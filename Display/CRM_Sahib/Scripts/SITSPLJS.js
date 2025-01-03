var app = angular.module('MyApp', ['ngTable', 'angucomplete-alt', 'moment-picker', 'cp.ngConfirm', 'ngFileUpload', 'ngMessages', 'directive.contextMenu']);

app.controller('CustomerCtrl', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {
    //--------------------------------------Dashboard Page------------------------     

    /*==============================================================================*                                                                   
     *                                                                              
     *                                SALES ORDER  For Customer                                 
     *                                
     * =============================================================================*/

    $scope.CustomerInit = function () {
        angular.element('#loading').hide();
    }

    // Get Customer Details on Index
    $scope.GetCustomerDetails = function () {
        $http.get("/api/WebAPI/GetCustomerDetails").then(function (res) {
            $scope.CustomerDetails = res.data;
            if ($scope.CustomerDetails !== undefined && $scope.CustomerDetails !== '' && $scope.CustomerDetails !== null) {                
                angular.element('#SOCustomerCode_value').val($scope.CustomerDetails.CardCode);
                angular.element('#SOCustomerName_value').val($scope.CustomerDetails.CardName);
                $scope.SalesOrderHeader.CustomerCode = $scope.CustomerDetails.CardCode;
                $scope.SalesOrderHeader.CustomerName = $scope.CustomerDetails.CardName;                
                $("#SOCustomerCode").prop("disabled", true);
                $("#SOCustomerName").prop("disabled", true);
            }
        });
    };

    // Bind BP at the time Customet Login 
    $scope.GetBPByCardCode = function () {
        $http.get('/api/WebAPI/GetBPByCardCode').then(function (res) {
            $scope.BP = res.data;
        });
    };

    $scope.SaleOrderINIT = function () {
        try {

            $scope.SalesOrderHeader = {};
            $scope.SalesOrderRow = {};
            $scope.SalesOrderArray = [];
            $scope.SalesOrderRow.HideButton = false;
            $scope.SalesOrderHeader.DeliveryDate = moment();
            $scope.SalesOrderHeader.PostingDate = moment();
            $scope.SalesOrderHeader.DocDate = moment();
            $scope.GetCustomerDetails();
            var data = localStorage.getItem("SaleQuotation");
            var saleQuotationData = JSON.parse(data);
            $http.get('/api/WebAPI/SOINIT').then(function (response) {
                angular.element('#loading').hide();
                $scope.OcrdData = response.data.OCRD;
                $scope.OCPRdata = response.data.OCPR;
                $scope.OITMData = response.data.OITM;
                $scope.OSTCData = response.data.OSTC;
                $scope.OBPLData = response.data.OBPL;
                $scope.SeriesData = response.data.Series;
                $scope.SalesOrderHeader.DocNumber = response.data.ORDR;
                $scope.OSCN = response.data.OSCN;

                $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
                $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);

                ////if (saleQuotationData != undefined) {
                ////    $scope.SalesOrderHeader = saleQuotationData;
                ////    $scope.SalesOrderArray = $scope.SalesOrderHeader.Row;
                ////    $scope.$broadcast('angucomplete-alt:changeInput', 'SOCustomerCode', $scope.SalesOrderHeader.CustomerCode);
                ////    $scope.$broadcast('angucomplete-alt:changeInput', 'SOCustomerName', $scope.SalesOrderHeader.CustomerName);
                ////    $scope.SalesOrderHeader.DocNumber = response.data.ORDR;
                ////    $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
                ////    $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
                ////    var dateObj = new Date($scope.SalesOrderHeader.DocDate);
                ////    var dateobj2 = new Date($scope.SalesOrderHeader.PostingDate);
                ////    $scope.SalesOrderHeader.DocDate = moment(dateObj);
                ////    $scope.SalesOrderHeader.PostingDate = moment(dateobj2);
                ////}

                $scope.HideLoader();
            }).catch(function (response) {
                $scope.HideLoader();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            })

        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }

    };

    $scope.afterSelectSOseries = function (seriesCode) {
        $scope.SalesOrderHeader.BranchCode = ($scope.SeriesData.find(x => x.Series == parseInt(seriesCode)).BPLId).toString();
    };

    $scope.AfterSelectedSOBP = function (selected) {
        try {
            angular.element('#SOCustomerCode_value').val(selected.originalObject.CardCode);
            angular.element('#SOCustomerName_value').val(selected.originalObject.CardName);
            $scope.SalesOrderHeader.CustomerCode = selected.originalObject.CardCode;
            $scope.SalesOrderHeader.CustomerName = selected.originalObject.CardName;
            $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
            $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.AfterSelectSOItem = function (selected) {
        try {
            angular.element('#SOCatno_value').val(selected.originalObject.ItemCode);
            angular.element('#SOItemName_value').val(selected.originalObject.ItemName);
            $scope.SalesOrderRow.BpCatNo = selected.originalObject.CatNo;
            $scope.SalesOrderRow.ItemCode = selected.originalObject.ItemCode;
            $scope.SalesOrderRow.ItemName = selected.originalObject.ItemName;
            $scope.GetSubCatNumByItemCode($scope.SalesOrderRow.ItemCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };

    
    $scope.GetSubCatNumByItemCode = function (ItemCode) {
        $http.get("/api/WebApi/GetSubCatNumByItemCode?ItemCode=" + ItemCode).then(function (res) {
            $scope.SalesOrderRow.SubCatName = res.data;
        });
    }

    $scope.SOAddNewItemRow = function () {
        if ($scope.SOValidateItemRow()) {
            $scope.SalesOrderRow.TaxName = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Name;
            $scope.SalesOrderRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Rate;
            $scope.SalesOrderArray.push($scope.SalesOrderRow);
            if ($scope.SalesOrderArray.length > 0) {
                $scope.SalesOrderRow = {};
                $scope.SalesOrderRow.HideButton = false;
                $scope.$broadcast('angucomplete-alt:clearInput', 'SOCatno');
                $scope.$broadcast('angucomplete-alt:clearInput', 'SOItemCode');
                $scope.SOcalculationoftotal();
            }

        }
        else
            $scope.alert('Alert !!', 'Please Fill Required Fields', 'btn-danger', 'red');
    };

    $scope.SOEditItemRow = function (index) {
        try {
            if ($scope.SalesOrderArray.length > 0) {
                $scope.$broadcast('angucomplete-alt:changeInput', 'SOCatno', $scope.SalesOrderArray[index].BpCatNo);
                $scope.$broadcast('angucomplete-alt:changeInput', 'SOItemName', $scope.SalesOrderArray[index].ItemName);
                $scope.SalesOrderRow = $scope.SalesOrderArray[index];
                $scope.showSavebutton = true;
                for (var i = 0; i < $scope.SalesOrderArray.length; i++) {
                    if (i == index)
                        $scope.SalesOrderArray[i].HideButton = true;
                    else
                        $scope.SalesOrderArray[i].HideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.SOUpdateItemRow = function (index) {
        try {
            $scope.$broadcast('angucomplete-alt:clearInput', 'SOCatno');
            $scope.$broadcast('angucomplete-alt:clearInput', 'SOItemCode');
            $scope.SalesOrderArray[index].TaxName = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Name;
            $scope.SalesOrderArray[index].TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Rate;
            $scope.SalesOrderArray[index] = $scope.SalesOrderRow;
            for (var i = 0; i < $scope.SalesOrderArray.length; i++) {
                $scope.SalesOrderArray[i].HideButton = false;
            }

            $scope.SalesOrderRow = {};
            $scope.showSavebutton = false;
            $scope.SOcalculationoftotal();
            if ($scope.SalesOrderHeader.BillDiscount != undefined)
                $scope.SOpendingcalulation();
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.SODeleteItemRow = function (index) {
        if ($scope.SalesOrderArray.length > 0) {
            $scope.SalesOrderArray.splice(index, 1);
            $scope.SOcalculationoftotal();
            if ($scope.SalesOrderHeader.BillDiscount != undefined)
                $scope.SOpendingcalulation();
        }
    };

    $scope.SOcalculationoftotal = function () {
        $scope.SalesOrderHeader.TotalBF = 0;
        $scope.SalesOrderHeader.TaxAmount = 0;
        for (var i = 0; i < $scope.SalesOrderArray.length; i++) {
            $scope.SalesOrderHeader.TotalBF = $scope.SalesOrderArray[i].RowTotal + $scope.SalesOrderHeader.TotalBF;
            $scope.SalesOrderHeader.TaxAmount = $scope.SalesOrderArray[i].TaxAmount + $scope.SalesOrderHeader.TaxAmount;
        }
        $scope.SalesOrderHeader.BillTotal = $scope.SalesOrderHeader.TotalBF;
    };

    $scope.SOpendingcalulation = function () {
        var GtotalWithDiscount = $scope.SalesOrderHeader.BillTotal * (parseInt($scope.SalesOrderHeader.BillDiscount) / 100);
        $scope.SalesOrderHeader.BillTotal = $scope.SalesOrderHeader.BillTotal - GtotalWithDiscount;
    };

    $scope.SORowTotalCaluc = function () {
        if ($scope.SalesOrderRow.Discount == undefined) {
            var RowTotal = $scope.SalesOrderRow.Qty * parseInt($scope.SalesOrderRow.UnitPrice);
            $scope.SalesOrderRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Rate;
            $scope.SalesOrderRow.TaxAmount = RowTotal * $scope.SalesOrderRow.TaxRate / 100;
            $scope.SalesOrderRow.RowTotal = $scope.SalesOrderRow.TaxAmount + RowTotal;
        }
        else {
            $scope.SalesOrderRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Rate;
            var DiscountPercent = $scope.SalesOrderRow.Discount / 100;
            var RowTotal = $scope.SalesOrderRow.Qty * parseInt($scope.SalesOrderRow.UnitPrice);
            var DiscountPrice = RowTotal * DiscountPercent;
            var TotalBeforeDiscount = RowTotal - DiscountPrice;
            $scope.SalesOrderRow.TaxAmount = TotalBeforeDiscount * $scope.SalesOrderRow.TaxRate / 100;
            $scope.SalesOrderRow.RowTotal = $scope.SalesOrderRow.TaxAmount + TotalBeforeDiscount;
        }
    };

    $scope.SOValidateItemRow = function () {
        if ($scope.SalesOrderRow.ItemCode != undefined && $scope.SalesOrderRow.ItemName != undefined && $scope.SalesOrderRow.Qty != undefined
            && $scope.SalesOrderRow.Taxcode != undefined
        )
            return true;
        else
            return false;
    };

    $scope.SubmitSalesOrder = function () {
        try {
            $scope.SalesOrderHeader.Row = [];
            if ($scope.SalesOrderArray.length > 0) {
                $scope.SalesOrderHeader.Row = $scope.SalesOrderArray;
                $scope.showloader();
                $http.post('/api/WebAPI/PunchSalesOrder', $scope.SalesOrderHeader).then(function (response) {
                    if (response.data.startsWith('Sales')) {
                        $scope.HideLoader();
                        // localStorage.clear();
                        localStorage.removeItem("SaleQuotation");
                        $scope.alert('Success !!', response.data, 'btn-success', 'green');

                    }
                    else {
                        $scope.HideLoader();
                        $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                    }

                }).catch(function (response) {
                    $scope.HideLoader();
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response.data);
                });
            }
            else {
                $scope.alert('Alert !!', 'Kindly Add Item !!', 'btn-danger', 'red');
            }
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    //-------------------------------------Custom Function---------------------------

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
                },
                OK: {
                    text: 'Copy To Sales Order',
                    btnClass: 'btn-success',
                    action: function () {
                        $scope.CopyQuotationToOrder();
                    }
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

app.controller('MyCtrl', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {
    //--------------------------------------Dashboard Page------------------------

    $scope.BusinessPartnerInit = function () {
        angular.element('#loading').hide();
    }

    //----------------------------------------Business Partner-------------------
    $scope.CreateBusinessPartnerINIT = function () {
        try {
            $scope.BusinessPartnerList = {};
            $scope.ContactPersonlist = {};
            $scope.BillToPayTolist = {};
            $scope.ShipToShipFromlist = {};
            $scope.ContactPersonArray = [];
            $scope.BillToPayToArray = [];
            $scope.ShipToShipFromArray = [];
            $scope.phonenumbervalidation = /^[6789]\d{9}/;
            $scope.emailvalidation = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            $scope.ContactPersonlist.HideButton = false;
            $scope.BillToPayTolist.BillHideButton = false;
            $scope.ShipToShipFromlist.ShipHideButton = false;
            $http.get("../api/WebAPI/GetBusinessPartnerINIT").then(function (response) {
                $scope.CurrencyData = response.data.Currency;
                $scope.GroupData = response.data.Group;
                $scope.CountryData = response.data.Country;
                $scope.StateData = response.data.State;
                $scope.seriesData = response.data.Series;
                $scope.HideLoader();
            }).catch(function (response) {
                $scope.HideLoader();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            });
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.getcuscode = function () {
        var data = $scope.seriesData.find(x => x.Series == $scope.BusinessPartnerList.Series);
        var x = data.BeginStr;
        var y = (data.NextNumber).toString();
        var y1 = y.length;
        var z = data.NumSize;
        var zero = z - y1;
        if (data == null) {
            $scope.BusinessPartnerList.CustomerCode = undefined;
        }
        if (zero == 0) {
            var value = x + y;
            $scope.BusinessPartnerList.CustomerCode = value;

        }
        if (zero == 1) {
            var value = x + "0" + y;
            $scope.BusinessPartnerList.CustomerCode = value;

        } if (zero == 2) {
            var value = x + "00" + y;
            $scope.BusinessPartnerList.CustomerCode = value;

        } if (zero == 3) {
            var value = x + "000" + y;
            $scope.BusinessPartnerList.CustomerCode = value;

        }
        if (zero == 4) {
            var value = x + "0000" + y;
            $scope.BusinessPartnerList.CustomerCode = value;

        }
        if (zero == 5) {
            var value = x + "00000" + y;
            $scope.BusinessPartnerList.CustomerCode = value;
        }
    };

    //-----------------------------------------------Contact Person Function-------------------
    $scope.AddContactPerson = function () {
        try {
            if ($scope.ValidateContactPersonList()) {
                $scope.ContactPersonArray.push($scope.ContactPersonlist);
                if ($scope.ContactPersonArray.length > 0) {
                    $scope.ContactPersonlist = {};
                    $scope.ContactPersonlist.HideButton = false;
                }
            }
            else
                $scope.alert('Alert !!', 'Kindly Fill Required Fields', 'btn-danger', 'red');
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.EditConactPerson = function (index) {

        try {
            if ($scope.ContactPersonArray.length > 0) {
                $scope.ContactPersonlist = $scope.ContactPersonArray[index];
                $scope.showSavebutton = true;
                for (var i = 0; i < $scope.ContactPersonArray.length; i++) {
                    if (i == index)
                        $scope.ContactPersonArray[i].HideButton = true;
                    else
                        $scope.ContactPersonArray[i].HideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.UpdateConactPerson = function (index) {

        try {
            $scope.ContactPersonArray[index] = $scope.ContactPersonlist;
            for (var i = 0; i < $scope.ContactPersonArray.length; i++) {
                $scope.ContactPersonArray[i].HideButton = false;
            }
            $scope.ContactPersonlist = {};
            $scope.showSavebutton = false;
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.DeleteConactPerson = function (index) {
        try {
            if ($scope.ContactPersonArray.length > 0)
                $scope.ContactPersonArray.splice(index, 1);
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.ValidateContactPersonList = function () {
        if ($scope.ContactPersonlist.ContactID != undefined && $scope.ContactPersonlist.gender != undefined
            && $scope.ContactPersonlist.FirstName != undefined && $scope.ContactPersonlist.LastName != undefined
            && $scope.ContactPersonlist.mobile != undefined && $scope.ContactPersonlist.Email != undefined
        )
            return true;
        else
            return false;
    };

    /*=========================================Addresses=======================================*/
    //--------------------------------Pay to Bill Address------------------------------------
    $scope.AddPayToBillToAddress = function () {
        try {
            if ($scope.ValidateBillToPayToList()) {
                $scope.BillToPayToArray.push($scope.BillToPayTolist);
                if ($scope.BillToPayToArray.length > 0) {
                    $scope.BillToPayTolist = {};
                    $scope.BillToPayTolist.BillHideButton = false;
                }
            }
            else
                $scope.alert('Alert !!', 'Kindly Fill Required Fields', 'btn-danger', 'red');
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.EditPayToBillToAddress = function (index) {
        try {
            if ($scope.BillToPayToArray.length > 0) {
                $scope.BillToPayTolist = $scope.BillToPayToArray[index];
                $scope.BillshowSavebutton = true;
                for (var i = 0; i < $scope.BillToPayToArray.length; i++) {
                    if (i == index)
                        $scope.BillToPayToArray[i].BillHideButton = true;
                    else
                        $scope.BillToPayToArray[i].BillHideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.UpdatePayToBillToAddress = function (index) {
        $scope.Hide
        try {
            $scope.BillToPayToArray[index] = $scope.BillToPayTolist;
            for (var i = 0; i < $scope.BillToPayToArray.length; i++) {
                $scope.BillToPayToArray[i].BillHideButton = false;
            }
            $scope.BillToPayTolist = {};
            $scope.BillshowSavebutton = false;
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.DeletePayToBillToAddress = function (index) {
        try {
            if ($scope.BillToPayToArray.length > 0)
                $scope.BillToPayToArray.splice(index, 1);
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.ValidateBillToPayToList = function () {
        if ($scope.BillToPayTolist.AddressID != undefined && $scope.BillToPayTolist.City != undefined
            && $scope.BillToPayTolist.Zipcode != undefined && $scope.BillToPayTolist.Country != undefined
            && $scope.BillToPayTolist.State != undefined
        )
            return true;
        else
            return false;
    };

    $scope.ischeckedfun = function () {
        if ($scope.IsChecked) {
            $scope.ShipToShipFromlist = $scope.BillToPayTolist;
        }
        else {
            $scope.ShipToShipFromlist = {};
        }
    };

    //---------------------------------------------------------------------------------------


    //-------------------------------------Ship To Ship From --------------------------------
    $scope.AddShipToShipFromAddress = function () {
        try {
            if ($scope.ValidateShipToShipFromList()) {
                $scope.ShipToShipFromArray.push($scope.ShipToShipFromlist);
                if ($scope.ShipToShipFromArray.length > 0) {
                    $scope.ShipToShipFromlist = {};
                    $scope.ShipToShipFromlist.ShipHideButton = false;
                }
            }
            else
                $scope.alert('Alert !!', 'Kindly Fill Required Fields', 'btn-danger', 'red');
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.EditShipToShipFromAddress = function (index) {
        try {
            if ($scope.ShipToShipFromArray.length > 0) {
                $scope.ShipToShipFromlist = $scope.ShipToShipFromArray[index];
                $scope.ShipshowSavebutton = true;
                for (var i = 0; i < $scope.ShipToShipFromArray.length; i++) {
                    if (i == index)
                        $scope.ShipToShipFromArray[i].ShipHideButton = true;
                    else
                        $scope.ShipToShipFromArray[i].ShipHideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.UpdateShipToShipFromAddress = function (index) {
        try {
            $scope.ShipToShipFromArray[index] = $scope.ShipToShipFromlist;
            for (var i = 0; i < $scope.ShipToShipFromArray.length; i++) {
                $scope.ShipToShipFromArray[i].ShipHideButton = false;
            }
            $scope.ShipToShipFromlist = {};
            $scope.ShipshowSavebutton = false;
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.DeleteShipToShipFromAddress = function (index) {
        try {
            if ($scope.ShipToShipFromArray.length > 0)
                $scope.ShipToShipFromArray.splice(index, 1);
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.ValidateShipToShipFromList = function () {
        if ($scope.ShipToShipFromlist.AddressID != undefined && $scope.ShipToShipFromlist.City != undefined
            && $scope.ShipToShipFromlist.Zipcode != undefined && $scope.ShipToShipFromlist.Country != undefined
            && $scope.ShipToShipFromlist.State != undefined
        )
            return true;
        else
            return false;
    };

    //---------------------------------------------------------------------------------------

    $scope.PunchBusinessPartner = function () {
        try {
            $scope.BusinessPartnerList.ContactPerson = [];
            $scope.BusinessPartnerList.BillToAdd = [];
            $scope.BusinessPartnerList.ShipToAdd = [];

            if ($scope.ContactPersonArray.length > 0)
                $scope.BusinessPartnerList.ContactPerson = $scope.ContactPersonArray;

            if ($scope.BillToPayToArray.length > 0)
                $scope.BusinessPartnerList.BillToAdd = $scope.BillToPayToArray;

            if ($scope.ShipToShipFromArray.length > 0)
                $scope.BusinessPartnerList.ShipToAdd = $scope.ShipToShipFromArray;
            $scope.showloader();
            $http.post('/api/WebAPI/PunchBusinessPartner', $scope.BusinessPartnerList).then(function (response) {
                if (response.data.startsWith('Business')) {
                    $scope.HideLoader();
                    $scope.alert('Success !!', response.data, 'btn-success', 'green');
                }
                else {
                    $scope.HideLoader();
                    $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                }

            }).catch(function (response) {
                $scope.HideLoader();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            });
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.AfterSelectedBP = function (selected) {
        try {
            angular.element('#CardCode_value').val(selected.originalObject.CardCode);
            angular.element('#CardName_value').val(selected.originalObject.CardName);
            $scope.OpportunityList.CardCode = selected.originalObject.CardCode;
            $scope.OpportunityList.CardName = selected.originalObject.CardName;
            $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.OpportunityList.CardCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };


    $scope.IndexPageInit = function () {
        angular.element('#loading').hide();
    }

    $scope.InsertUser = function () {
        var type = document.getElementById("insertUser").getAttribute("value");
        if (type == "Submit") {
            $scope.C_USERDETAILS = {};
            //$scope.Customer.CustomerCode = $scope.CustomerCode;
            //$scope.Customer.CustomerName = $scope.CustomerName;
            $scope.C_USERDETAILS.U_UserId = $scope.U_UserId;
            $scope.C_USERDETAILS.U_UserName = $scope.U_UserName;
            $scope.C_USERDETAILS.U_Name = $scope.U_Name;
            $scope.C_USERDETAILS.U_Designation = $scope.U_Designation;
            $scope.C_USERDETAILS.U_BranchId = $scope.U_BranchId;
            $scope.C_USERDETAILS.U_Mobile1 = $scope.U_Mobile1;
            $scope.C_USERDETAILS.U_ReportingHead = $scope.U_ReportingHead;
            $scope.C_USERDETAILS.U_UserStatus = $scope.U_UserStatus;
            $scope.C_USERDETAILS.U_EmailId = $scope.U_EmailId;
            $scope.C_USERDETAILS.Name = $scope.Name;
            //$scope.Customer.BranchId = $scope.BranchId;
            $scope.C_USERDETAILS.U_Locked = $scope.U_Locked;
            $scope.C_USERDETAILS.U_Mobile2 = $scope.U_Mobile2;
            $scope.C_USERDETAILS.U_UserType = $scope.U_UserType;          
            $http({
                method: "post",
                url: "/Home/InsertRecord",
                //datatype: "json",
                data: JSON.stringify($scope.C_USERDETAILS)
            }).then(function (response) {
                alert(response.data);
                location.reload();
                //$scope.CustomerCode = " ";
                //$scope.CustomerName = " ";


                //$scope.FirstName = $scope.LastName = $scope.BranchCode ="";
                //$scope.LastName = " ";
                //$scope.Designation = 0;
                //$scope.BranchCode = " ";
                //$scope.Mobile1 = " ";
                //$scope.Reportinghead = " ";
                //$scope.ddlstatus = " ";
                //$scope.Emailid = " ";
                //$scope.BranchId = " ";
                //$scope.Locked = " ";
                //$scope.mobile2 = " ";
                //$scope.UserType = " ";
                //$scope.Isdeleted = " ";


            });

        }


    };










   



    //-------------------------------------Custom Function---------------------------

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
                },
                OK: {
                    text: 'Copy To Sales Order',
                    btnClass: 'btn-success',
                    action: function () {
                        $scope.CopyQuotationToOrder();
                    }
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


});

app.controller('OpportunityCtrl', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {

    $scope.OpportunityINIT = function () {
        try {
            angular.element('#loading').show();
            $scope.OpportunityList = {};
            $scope.StagesArray = [];
            $scope.StagesList = {};
            $scope.DivisionList = {};
            $scope.DivisionArray = [];
            $scope.ItemList = {};
            $scope.OpportunityList.StartDate = moment();
            $scope.StagesList.HideButton = false;
            $scope.DivisionList.HideButton = false;
            $http.get('/api/WebAPI/OpporunityPageLoad').then(function (response) {
                $scope.OcrdData = response.data.OCRD;
                $scope.SalesPersonData = response.data.SalesPerson;
                $scope.StategeData = response.data.Stages;
                $scope.ActivityData = response.data.Activity;
                $scope.OCPRdata = response.data.OCPR;
                $scope.DivisionData = response.data.Divsion;
                $scope.TypeOPP = response.data.Division;
                $scope.ItemList.ItemCode = response.data.ItemCode;
                $scope.OpportunityList.OppNumber = response.data.OPPNO;

                $scope.HideLoader();
            }).catch(function (response) {
                $scope.HideLoader();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            })
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.AfterSelectedBP = function (selected) {
        try {
            angular.element('#CardCode_value').val(selected.originalObject.CardCode);
            angular.element('#CardName_value').val(selected.originalObject.CardName);
            $scope.OpportunityList.CardCode = selected.originalObject.CardCode;
            $scope.OpportunityList.CardName = selected.originalObject.CardName;
            $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.OpportunityList.CardCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.OnChangeOppType = function () {
        try {
            $http.post('/api/WebAPI/GetDocumentNumber', JSON.stringify($scope.OpportunityList.OppType)).then(function (response) {
                $scope.DocData = response.data;
            }).catch(function (response) {
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            })
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    //--------------------------------------------------Division(Types of opp)------------
    $scope.AddTypeOfOpp = function () {
        if ($scope.ValidateOppType()) {
            $scope.ShowTable = true;
            $scope.DivisionList.DivisionName = $scope.TypeOPP.find(x => x.U_Division === $scope.DivisionList.DivisionID).Descr;
            $scope.DivisionArray.push($scope.DivisionList);
            if ($scope.DivisionArray.length > 0) {
                $scope.DivisionList = {};
                $scope.DivisionList.HideButton = false;
            }

        }
        else
            $scope.alert('Alert !!', 'Please Fill Required Fields', 'btn-danger', 'red');
    };

    $scope.EditTypeOfOpp = function (index) {
        try {
            if ($scope.DivisionArray.length > 0) {
                $scope.DivisionList = $scope.DivisionArray[index];
                $scope.showSavebutton = true;
                for (var i = 0; i < $scope.DivisionArray.length; i++) {
                    if (i == index)
                        $scope.DivisionArray[i].HideButton = true;
                    else
                        $scope.DivisionArray[i].HideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.UpdateTypeOfOpp = function (index) {
        try {
            $scope.DivisionArray[index] = $scope.DivisionList;
            for (var i = 0; i < $scope.DivisionArray.length; i++) {
                $scope.DivisionArray[i].HideButton = false;
            }
            $scope.DivisionList = {};
            $scope.showSavebutton = false;
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.DeleteTypeOfOpp = function (index) {
        if ($scope.DivisionArray.length > 0) {
            $scope.ShowTable = false;
            $scope.DivisionArray.splice(index, 1);
        }
    };

    $scope.ValidateOppType = function () {
        if ($scope.DivisionList.DivisionID != undefined && $scope.DivisionList.SubDivsion != undefined && $scope.DivisionList.Cateogry != undefined)
            return true;
        else
            return false;
    };

    //$scope.SubmitOpp = function () {
    $scope.OpportunityAdd = function () {
        try {
            $scope.OpportunityList.Stage = [];
            $scope.OpportunityList.Divisionlist = [];
            //if ($scope.StagesArray.length > 0 && $scope.DivisionArray.length > 0) { // Commented on 18 Mar 2021 by Dilshad A.
            if ($scope.StagesArray.length > 0) {
                $scope.OpportunityList.Stage = $scope.StagesArray;
                $scope.OpportunityList.Divisionlist = $scope.DivisionArray;

                $scope.showloader();
                $http.post('/Home/AddPunchOpportunity', { OppList: $scope.OpportunityList }).then(function (response) {
                    if (response.data.startsWith('Opportunity')) {
                        $scope.HideLoader();
                        $scope.alert('Success !!', response.data, 'btn-success', 'green');
                    }
                    else {
                        $scope.HideLoader();
                        $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                    }

                }).catch(function (response) {
                    $scope.HideLoader();
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response.data);
                });
            }
            else {
                $scope.alert('Alert !!', 'Kindly Add Divison And Stages !!', 'btn-danger', 'red');
            }
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.OpportunityUpdate = function () {
        try {
            $scope.OpportunityList.Stage = [];
            $scope.OpportunityList.Divisionlist = [];
            //if ($scope.StagesArray.length > 0 && $scope.DivisionArray.length > 0) {
            if ($scope.StagesArray.length > 0) {
                $scope.OpportunityList.Stage = $scope.StagesArray;
                $scope.OpportunityList.Divisionlist = $scope.DivisionArray;

                $scope.showloader();
                $http.post('/api/WebAPI/PunchOpportunity', $scope.OpportunityList).then(function (response) {
                    if (response.data.startsWith('Opportunity')) {
                        $scope.HideLoader();
                        $scope.alert('Success !!', response.data, 'btn-success', 'green');
                    }
                    else {
                        $scope.HideLoader();
                        $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                    }

                }).catch(function (response) {
                    $scope.HideLoader();
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response.data);
                });
            }
            else {
                $scope.alert('Alert !!', 'Kindly Add Divison And Stages !!', 'btn-danger', 'red');
            }
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.AddStagesRow = function () {
        try {
            if ($scope.ValidateStage()) {
                $scope.StagesList.SalesPersonName = $scope.SalesPersonData.find(x => x.SlpCode == $scope.StagesList.SalesPersonCode.SlpCode).SlpName;
                $scope.StagesList.ActivityName = $scope.ActivityData.find(x => x.FldValue == $scope.StagesList.Activity).Descr;
                $scope.StagesList.StatgeName = $scope.StategeData.find(x => x.StepId == $scope.StagesList.StageID).Descript;
                $scope.StagesList.StartDateStr = $scope.StagesList.StartDate.format('DD-MM-YYYY');
                $scope.StagesList.ClosingDateStr = $scope.StagesList.ClosingDate.format('DD-MM-YYYY');
                $scope.StagesList.SlpCode = $scope.StagesList.SalesPersonCode.SlpCode;

                $scope.StagesArray.push($scope.StagesList);
                if ($scope.StagesArray.length > 0) {
                    $scope.StagesList = {};
                    $scope.StagesList.HideButton = false;
                }
            }
            else
                $scope.alert('Alert !!', 'Please Fill Required Fields', 'btn-danger', 'red');

        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.EditStagesRow = function (index) {
        try {
            if ($scope.StagesArray.length > 0) {

                var data = {};
                //data.SlpCode = $scope.StagesArray[index].SalesPersonCode[0];
                data.SplCode = parseInt($scope.StagesArray[index].SalesPersonCode || $scope.StagesArray[index].SlpCode);
                data.SlpName = $scope.StagesArray[index].SalesPersonName;
                $scope.StagesArray[index].SalesPersonCode = data;
                $scope.StagesList = $scope.StagesArray[index];
                $scope.StagesList.DocEntry = $scope.StagesList.DocEntry.toString();
                $scope.StategeData[index].SalesPersonCode = data;

                //$scope.StagesList.SalesPersonCode = {};                    
                //$scope.StagesList.SalesPersonCode.SlpCode  = $scope.StagesArray[index].SalesPersonCode[0];
                //$scope.StagesList.SalesPersonCode.SlpName = $scope.StagesArray[index].SalesPersonName;
                //$scope.StagesList.SalesPersonCode = { SlpName: $scope.StagesArray[index].SalesPersonCode, SlpName: $scope.StagesArray[index].SalesPersonName }

                $scope.showSavebutton = true;
                for (var i = 0; i < $scope.StagesArray.length; i++) {
                    if (i == index)
                        $scope.StagesArray[i].HideButton = true;
                    else
                        $scope.StagesArray[i].HideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.updateStagesRow = function (index) {

        try {
            $scope.StagesArray[index] = $scope.StagesList;
            $scope.StagesArray[index].StartDateStr = moment($scope.StagesList.StartDate).format("DD-MM-YYYY");
            $scope.StagesArray[index].ClosingDateStr = moment($scope.StagesList.ClosingDate).format("DD-MM-YYYY");
            $scope.StagesArray[index].SalesPersonName = $scope.StagesList.SalesPersonCode.SlpName;
            $scope.StagesArray[index].SalesPersonCode = $scope.StagesArray[index].SalesPersonCode.SlpCode;
            //$scope.StagesArray[index].SalesPersonCode = $scope.StagesList.SalesPersonCode.SlpCode;
            for (var i = 0; i < $scope.StagesArray.length; i++) {
                $scope.StagesArray[i].HideButton = false;
            }
            $scope.StagesList = {};
            $scope.showSavebutton = false;
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.DeleteStagesRow = function (index) {
        if ($scope.StagesArray.length > 0) {
            $scope.StagesArray.splice(index, 1);
        }
    };

    $scope.ValidateStage = function () {
        try {
            if ($scope.StagesList.StartDate != undefined && $scope.StagesList.ClosingDate != undefined && $scope.StagesList.SalesPersonCode != undefined
                && $scope.StagesList.StageID != undefined && $scope.StagesList.StatgePercentage != undefined && $scope.StagesList.PotentialAmount != undefined
                && $scope.StagesList.WeightAmount != undefined && $scope.StagesList.DocType != undefined && $scope.StagesList.DocEntry != undefined
                && $scope.StagesList.Activity != undefined)
                return true;
            else
                return false;
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

   
    //$scope.showFindOportunity = function () {
    //    $("#popupFindOpportunity").modal('show');
    //};

   
    $scope.GetOpportunityByCardCode = function (CardCode) {
        if (CardCode !== undefined && CardCode !== "" && CardCode !== null) {
            $scope.CardCode = CardCode;
            $http.get("/api/WebAPI/GetOpportunityByCardCode?CardCode=" + $scope.CardCode).then(function (res) {
                $scope.OpportunityData = res.data;
            });
        }
        else {
            alert("BP Code is required");
        }
    };

    // Get Single Oppertunity by DocEntry from BP 
    $scope.GetOpportunityFromList = function (DocEntry) {
        var data = $scope.OpportunityData.filter(x => x.DocEntry == DocEntry);

        if (data[0].OpprType === "R") {
            $scope.OpportunityList.OppType = "C"
        } else {
            $scope.OpportunityList.OppType = "S";
        }
        $scope.OpportunityList.OppNumber = data[0].OpprId;
        var date = moment(data[0].OpenDate).format('DD-MM-YYYY');
        $scope.OpportunityList.MomentStartDate = date;
        $scope.OpportunityList.OppName = data[0].Name;
        $scope.OpportunityList.ContactPerson = data[0].CprCode;// Contact Person
        $scope.OpportunityList.SalesPersonCode = data[0].SlpCode;// Sales Person
        $scope.OpportunityList.PotentialAmount = data[0].MaxSumLoc;
        $scope.OpportunityList.PredictedCloseDate = data[0].CloseDate;
        $scope.OpportunityList.OpprId = data[0].OpprId;

        $scope.GetOpprStageByOpprId(data[0].OpprId);
    }

    // Bind Stage Details BY OpperId 
    $scope.GetOpprStageByOpprId = function (OpprId) {
        $http.get("/api/WebAPI/GetOpprStageByOpprId?OpprId=" + OpprId).then(function (res) {
            $scope.OpprStage = res.data;
            var length = $scope.OpprStage.length;
            for (var i = 0; i < length; i++) {
                var objType = "";
                var DocNum = "";
                if ($scope.OpprStage[i].d1.ObjType === "-1") {
                    objType = "";
                }
                else if ($scope.OpprStage[i].d1.ObjType === "23") {
                    objType = "Sales Quotations";
                }
                else if ($scope.OpprStage[i].d1.ObjType === "540000006") {
                    objType = "Purchase Quotation";
                }

                $scope.OnChangeOppType();

                $scope.StagesArray.push({
                    StartDateStr: moment($scope.OpprStage[i].d1.OpenDate).format("DD-MM-YYYY"), ClosingDateStr: moment($scope.OpprStage[i].d1.CloseDate).format("DD-MM-YYYY"),
                    StartDate: moment($scope.OpprStage[i].d1.OpenDate), ClosingDate: moment($scope.OpprStage[i].d1.CloseDate),
                    SalesPersonName: $scope.OpprStage[i].SlpName, SalesPersonCode: $scope.OpprStage[i].SlpCode.toString(),
                    StatgeName: $scope.OpprStage[i].Descript, StageID: $scope.OpprStage[i].StepId.toString(),
                    StatgePercentage: $scope.OpprStage[i].d1.ClosePrcnt, PotentialAmount: $scope.OpprStage[i].d1.MaxSumLoc,
                    WeightAmount: $scope.OpprStage[i].d1.WtSumLoc, DocType: objType, DocEntry: DocNum = $scope.OpprStage[i].d1.DocId,
                    DocNumber: $scope.OpprStage[i].d1.DocEntry, Activity: $scope.OpprStage[i].d1.U_act1,
                });
                //DocNumber: $scope.OpprStage[i].d1.DocNumber
            }
        });
    };
});

app.controller('SaleCtrl', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {

    // Get Customer Details 
    $scope.GetCustomerDetails = function () {
        $http.get("/api/WebAPI/GetCustomerDetails").then(function (res) {
            $scope.CustomerDetails = res.data;
            if ($scope.CustomerDetails !== undefined && $scope.CustomerDetails !== '' && $scope.CustomerDetails !== null) {
                $scope.OpportunityList.CardCode = $scope.CustomerDetails.CardCode;
                $scope.OpportunityList.CardName = $scope.CustomerDetails.CardName;
            }
        });
    };

    // Get Customer Details on Index Page 
    $scope.GetBPByCardCode = function () {
        $http.get("/api/WebAPI/GetBPByCardCode").then(function (res) {
            $scope.CustomerDetails = res.data;
            if ($scope.CustomerDetails !== undefined && $scope.CustomerDetails !== '' && $scope.CustomerDetails !== null) {
                $scope.SalesOrderHeader.CustomerCode = $scope.CustomerDetails.CardCode;
                $scope.SalesOrderHeader.CustomerName = $scope.CustomerDetails.CardName;
            }
        });
    };

    //--------------------------------Sale Quotation-----------------------------

    $scope.SaleQuotationINIT = function () {
        try {

            $scope.SalesQuotationHeader = {};
            $scope.SalesQuotationRow = {};
            $scope.SalesQuotationArray = [];
            $scope.SalesQuotationRow.HideButton = false;
            $scope.SalesQuotationHeader.ValidDate = moment();
            $scope.SalesQuotationHeader.PostingDate = moment();
            $scope.SalesQuotationHeader.DocDate = moment();
            var data = localStorage.getItem("QuotationFromAssign");
            $scope.AssginQuoteData = JSON.parse(data);
            $http.get('/api/WebAPI/SQINIT').then(function (response) {
                $scope.OcrdData = response.data.OCRD;
                $scope.OCPRdata = response.data.OCPR;
                $scope.OITMData = response.data.OITM;
                $scope.OSTCData = response.data.OSTC;
                $scope.OBPLData = response.data.OBPL;
                $scope.SeriesData = response.data.Series;
                $scope.OSCN = response.data.OSCN;
                $scope.SalesQuotationHeader.DocNumber = response.data.OQUT;

                //var PageName = angular.element('#hdnPageId').val();
                //if ($scope.AssginQuoteData != undefined && PageName == "PunchQuoteInDb") {
                //$scope.$broadcast('angucomplete-alt:changeInput', 'CustomerCode', $scope.AssginQuoteData[0].BpCode);
                //$scope.$broadcast('angucomplete-alt:changeInput', 'CustomerName', $scope.AssginQuoteData[0].BpName);
                $scope.SalesQuotationHeader.CustomerCode = $scope.AssginQuoteData[0].BpCode;
                $scope.SalesQuotationHeader.CustomerName = $scope.AssginQuoteData[0].BpName;
                $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.SalesQuotationHeader.CustomerCode);
                $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.SalesQuotationHeader.CustomerCode);
                $scope.SalesQuotationHeader.DocNumber = response.data.NoChargeDocNum;
                $scope.PunchQuote = true;
                //}
                //else {
                //    $scope.SalesQuotationHeader.DocNumber = response.data.OQUT;
                //    $scope.PunchQuote = false;
                //}
                $scope.HideLoader();
            }).catch(function (response) {
                $scope.HideLoader();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            })
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }

    };

    $scope.afterSelectSQseries = function (seriesCode) {
        $scope.SalesQuotationHeader.BranchCode = ($scope.SeriesData.find(x => x.Series == parseInt(seriesCode)).BPLId).toString();
    };

    $scope.AfterSelectedBP = function (selected) {
        try {
            angular.element('#CustomerCode_value').val(selected.originalObject.CardCode);
            angular.element('#CustomerName_value').val(selected.originalObject.CardName);
            $scope.SalesQuotationHeader.CustomerCode = selected.originalObject.CardCode;
            $scope.SalesQuotationHeader.CustomerName = selected.originalObject.CardName;
            $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.SalesQuotationHeader.CustomerCode);
            $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.SalesQuotationHeader.CustomerCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.AfterSelectItem = function (selected) {
        try {
            angular.element('#ItemName_value').val(selected.originalObject.ItemName);
            $scope.SalesQuotationRow.ItemCode = selected.originalObject.ItemCode;
            //$scope.SalesQuotationRow.BpCatNo = $scope.OSCN.find(x => x.ItemCode == $scope.SalesQuotationRow.ItemCode && x.CardCode == $scope.SalesQuotationHeader.CustomerCode).CatNo;
            //angular.element('#Catno_value').val($scope.SalesQuotationRow.BpCatNo);
            $scope.SalesQuotationRow.ItemName = selected.originalObject.ItemName;
        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.AfterSelectBpCat = function (selected) {
        angular.element('#ItemName_value').val(selected.originalObject.ItemName);
        angular.element('#Catno_value').val(selected.originalObject.CatNo);
        $scope.SalesQuotationRow.ItemCode = selected.originalObject.ItemCode;
        $scope.SalesQuotationRow.ItemName = selected.originalObject.ItemName;
    }

    $scope.AddNewItemRow = function () {
        if ($scope.ValidateItemRow()) {
            if (!$scope.PunchQuote) {
                $scope.SalesQuotationRow.TaxName = $scope.OSTCData.find(x => x.Code === $scope.SalesQuotationRow.Taxcode).Name;
                $scope.SalesQuotationRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesQuotationRow.Taxcode).Rate;
            }
            $scope.SalesQuotationArray.push($scope.SalesQuotationRow);
            if ($scope.SalesQuotationArray.length > 0) {
                $scope.SalesQuotationRow = {};
                $scope.SalesQuotationRow.HideButton = false;
                $scope.$broadcast('angucomplete-alt:clearInput', 'Catno');
                $scope.$broadcast('angucomplete-alt:clearInput', 'ItemCode');
                $scope.calculationoftotal();
            }

        }
        else
            $scope.alert('Alert !!', 'Please Fill Required Fields', 'btn-danger', 'red');
    };

    $scope.EditItemRow = function (index) {
        try {
            if ($scope.SalesQuotationArray.length > 0) {
                $scope.$broadcast('angucomplete-alt:changeInput', 'Catno', $scope.SalesQuotationArray[index].BpCatNo);
                $scope.$broadcast('angucomplete-alt:changeInput', 'ItemName', $scope.SalesQuotationArray[index].ItemName);
                $scope.SalesQuotationRow = $scope.SalesQuotationArray[index];
                $scope.showSavebutton = true;
                for (var i = 0; i < $scope.SalesQuotationArray.length; i++) {
                    if (i == index)
                        $scope.SalesQuotationArray[i].HideButton = true;
                    else
                        $scope.SalesQuotationArray[i].HideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.UpdateItemRow = function (index) {

        try {
            $scope.$broadcast('angucomplete-alt:clearInput', 'Catno');
            $scope.$broadcast('angucomplete-alt:clearInput', 'ItemCode');
            $scope.SalesQuotationArray[index].TaxName = $scope.OSTCData.find(x => x.Code === $scope.SalesQuotationRow.Taxcode).Name;
            $scope.SalesQuotationArray[index].TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesQuotationRow.Taxcode).Rate;
            $scope.SalesQuotationArray[index] = $scope.SalesQuotationRow;
            for (var i = 0; i < $scope.SalesQuotationArray.length; i++) {
                $scope.SalesQuotationArray[i].HideButton = false;
            }
            $scope.calculationoftotal();
            $scope.SalesQuotationRow = {};
            $scope.showSavebutton = false;

            if ($scope.SalesQuotationHeader.BillDiscount != undefined)
                $scope.pendingcalulation();

        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.DeleteItemRow = function (index) {
        if ($scope.SalesQuotationArray.length > 0) {
            $scope.SalesQuotationArray.splice(index, 1);
            $scope.calculationoftotal();
            if ($scope.SalesQuotationHeader.BillDiscount != undefined)
                $scope.pendingcalulation();
        }
    };

    $scope.calculationoftotal = function () {
        $scope.SalesQuotationHeader.TotalBF = 0;
        $scope.SalesQuotationHeader.TaxAmount = 0;
        for (var i = 0; i < $scope.SalesQuotationArray.length; i++) {
            $scope.SalesQuotationHeader.TotalBF = $scope.SalesQuotationArray[i].RowTotal + $scope.SalesQuotationHeader.TotalBF;
            $scope.SalesQuotationHeader.TaxAmount = $scope.SalesQuotationArray[i].TaxAmount + $scope.SalesQuotationHeader.TaxAmount;
        }
        $scope.SalesQuotationHeader.BillTotal = $scope.SalesQuotationHeader.TotalBF;
    };

    $scope.pendingcalulation = function () {
        var GtotalWithDiscount = $scope.SalesQuotationHeader.BillTotal * (parseInt($scope.SalesQuotationHeader.BillDiscount) / 100);
        $scope.SalesQuotationHeader.BillTotal = $scope.SalesQuotationHeader.BillTotal - GtotalWithDiscount;
    };

    $scope.RowTotalCaluc = function () {
        if ($scope.PunchQuote) {
            if ($scope.SalesQuotationRow.Discount == undefined) {
                var RowTotal = $scope.SalesQuotationRow.Qty * parseInt($scope.SalesQuotationRow.UnitPrice);
                $scope.SalesQuotationRow.RowTotal = RowTotal;
            }
            else {
                var DiscountPercent = $scope.SalesQuotationRow.Discount / 100;
                var RowTotal = $scope.SalesQuotationRow.Qty * parseInt($scope.SalesQuotationRow.UnitPrice);
                var DiscountPrice = RowTotal * DiscountPercent;
                var TotalBeforeDiscount = RowTotal - DiscountPrice;
                $scope.SalesQuotationRow.RowTotal = TotalBeforeDiscount
            }
        }
        else {
            if ($scope.SalesQuotationRow.Discount == undefined) {
                var RowTotal = $scope.SalesQuotationRow.Qty * parseInt($scope.SalesQuotationRow.UnitPrice);
                $scope.SalesQuotationRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesQuotationRow.Taxcode).Rate;
                $scope.SalesQuotationRow.TaxAmount = RowTotal * $scope.SalesQuotationRow.TaxRate / 100;
                $scope.SalesQuotationRow.RowTotal = $scope.SalesQuotationRow.TaxAmount + RowTotal;
            }
            else {
                $scope.SalesQuotationRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesQuotationRow.Taxcode).Rate;
                var DiscountPercent = $scope.SalesQuotationRow.Discount / 100;
                var RowTotal = $scope.SalesQuotationRow.Qty * parseInt($scope.SalesQuotationRow.UnitPrice);
                var DiscountPrice = RowTotal * DiscountPercent;
                var TotalBeforeDiscount = RowTotal - DiscountPrice;
                $scope.SalesQuotationRow.TaxAmount = TotalBeforeDiscount * $scope.SalesQuotationRow.TaxRate / 100;
                $scope.SalesQuotationRow.RowTotal = $scope.SalesQuotationRow.TaxAmount + TotalBeforeDiscount;
            }
        }
    };

    $scope.ValidateItemRow = function () {
        if ($scope.PunchQuote) {
            if ($scope.SalesQuotationRow.ItemCode != undefined && $scope.SalesQuotationRow.ItemName != undefined && $scope.SalesQuotationRow.Qty != undefined
            )
                return true;
            else
                return false;
        }
        else {
            if ($scope.SalesQuotationRow.ItemCode != undefined && $scope.SalesQuotationRow.ItemName != undefined && $scope.SalesQuotationRow.Qty != undefined
                && $scope.SalesQuotationRow.Taxcode != undefined
            )
                return true;
            else
                return false;
        }
    };

    $scope.SubmitSalesQuotation = function () {
        try {
            $scope.SalesQuotationHeader.Row = [];
            if ($scope.SalesQuotationArray.length > 0) {
                //$scope.SalesQuotationHeader.CreatedByCode = angular.element('hdnuserId').val();
                //$scope.SalesQuotationHeader.CreatedByName = angular.element('hdnUserName').val();
                $scope.SalesQuotationHeader.Row = $scope.SalesQuotationArray;

                $scope.showloader();
                $http.post('/api/WebAPI/PunchSalesQuotation', $scope.SalesQuotationHeader).then(function (response) {
                    if (response.data.startsWith('Sales')) {
                        $scope.HideLoader();
                        $scope.AlertFun('Success !!', response.data, 'btn-success', 'green');
                    }
                    else {
                        $scope.HideLoader();
                        $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                    }

                }).catch(function (response) {
                    $scope.HideLoader();
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response.data);
                });
            }
            else {
                $scope.alert('Alert !!', 'Kindly Add Item !!', 'btn-danger', 'red');
            }
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.CopyQuotationToOrder = function () {

        localStorage.setItem("SaleQuotation", JSON.stringify($scope.SalesQuotationHeader));
        location.href = '/Home/SalesOrder'

    };

    /*==============================================================================*                                                                   
     *                                                                              
     *                                SALES ORDER                                   
     *                                
     * =============================================================================*/

    $scope.SaleOrderINIT = function () {
        try {

            $scope.SalesOrderHeader = {};
            $scope.SalesOrderRow = {};
            $scope.SalesOrderArray = [];
            $scope.SalesOrderRow.HideButton = false;
            $scope.SalesOrderHeader.DeliveryDate = moment();
            $scope.SalesOrderHeader.PostingDate = moment();
            $scope.SalesOrderHeader.DocDate = moment();
            var data = localStorage.getItem("SaleQuotation");
            var saleQuotationData = JSON.parse(data);
            $http.get('/api/WebAPI/SOINIT').then(function (response) {
                $scope.OcrdData = response.data.OCRD;
                $scope.OCPRdata = response.data.OCPR;
                $scope.OITMData = response.data.OITM;
                $scope.OSTCData = response.data.OSTC;
                $scope.OBPLData = response.data.OBPL;
                $scope.SeriesData = response.data.Series;
                $scope.SalesOrderHeader.DocNumber = response.data.ORDR;
                $scope.OSCN = response.data.OSCN;

                if (saleQuotationData != undefined) {
                    $scope.SalesOrderHeader = saleQuotationData;
                    $scope.SalesOrderArray = $scope.SalesOrderHeader.Row;
                    $scope.$broadcast('angucomplete-alt:changeInput', 'SOCustomerCode', $scope.SalesOrderHeader.CustomerCode);
                    $scope.$broadcast('angucomplete-alt:changeInput', 'SOCustomerName', $scope.SalesOrderHeader.CustomerName);
                    $scope.SalesOrderHeader.DocNumber = response.data.ORDR;
                    $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
                    $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
                    var dateObj = new Date($scope.SalesOrderHeader.DocDate);
                    var dateobj2 = new Date($scope.SalesOrderHeader.PostingDate);
                    $scope.SalesOrderHeader.DocDate = moment(dateObj);
                    $scope.SalesOrderHeader.PostingDate = moment(dateobj2);
                }

                $scope.HideLoader();
            }).catch(function (response) {
                $scope.HideLoader();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            })

        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }

    };

    $scope.afterSelectSOseries = function (seriesCode) {
        $scope.SalesOrderHeader.BranchCode = ($scope.SeriesData.find(x => x.Series == parseInt(seriesCode)).BPLId).toString();
    };

    $scope.AfterSelectedSOBP = function (selected) {
        try {
            angular.element('#SOCustomerCode_value').val(selected.originalObject.CardCode);
            angular.element('#SOCustomerName_value').val(selected.originalObject.CardName);
            $scope.SalesOrderHeader.CustomerCode = selected.originalObject.CardCode;
            $scope.SalesOrderHeader.CustomerName = selected.originalObject.CardName;
            $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
            $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.SalesOrderHeader.CustomerCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.AfterSelectSOItem = function (selected) {
        try {
            angular.element('#SOCatno_value').val(selected.originalObject.ItemCode);
            angular.element('#SOItemName_value').val(selected.originalObject.ItemName);
            $scope.SalesOrderRow.BpCatNo = selected.originalObject.CatNo;
            $scope.SalesOrderRow.ItemCode = selected.originalObject.ItemCode;
            $scope.SalesOrderRow.ItemName = selected.originalObject.ItemName;
            $scope.GetSubCatNumByItemCode($scope.SalesOrderRow.ItemCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };

    //  on 24 Mar 2021
    $scope.GetSubCatNumByItemCode = function (ItemCode) {
        $http.get("/api/WebApi/GetSubCatNumByItemCode?ItemCode=" + ItemCode).then(function (res) {
            $scope.SalesOrderRow.SubCatName = res.data;
        });
    }

    $scope.SOAddNewItemRow = function () {
        if ($scope.SOValidateItemRow()) {
            $scope.SalesOrderRow.TaxName = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Name;
            $scope.SalesOrderRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Rate;
            $scope.SalesOrderArray.push($scope.SalesOrderRow);
            if ($scope.SalesOrderArray.length > 0) {
                $scope.SalesOrderRow = {};
                $scope.SalesOrderRow.HideButton = false;
                $scope.$broadcast('angucomplete-alt:clearInput', 'SOCatno');
                $scope.$broadcast('angucomplete-alt:clearInput', 'SOItemCode');
                $scope.SOcalculationoftotal();
            }

        }
        else
            $scope.alert('Alert !!', 'Please Fill Required Fields', 'btn-danger', 'red');
    };

    $scope.SOEditItemRow = function (index) {
        try {
            if ($scope.SalesOrderArray.length > 0) {
                $scope.$broadcast('angucomplete-alt:changeInput', 'SOCatno', $scope.SalesOrderArray[index].BpCatNo);
                $scope.$broadcast('angucomplete-alt:changeInput', 'SOItemName', $scope.SalesOrderArray[index].ItemName);
                $scope.SalesOrderRow = $scope.SalesOrderArray[index];
                $scope.showSavebutton = true;
                for (var i = 0; i < $scope.SalesOrderArray.length; i++) {
                    if (i == index)
                        $scope.SalesOrderArray[i].HideButton = true;
                    else
                        $scope.SalesOrderArray[i].HideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.SOUpdateItemRow = function (index) {
        try {
            $scope.$broadcast('angucomplete-alt:clearInput', 'SOCatno');
            $scope.$broadcast('angucomplete-alt:clearInput', 'SOItemCode');
            $scope.SalesOrderArray[index].TaxName = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Name;
            $scope.SalesOrderArray[index].TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Rate;
            $scope.SalesOrderArray[index] = $scope.SalesOrderRow;
            for (var i = 0; i < $scope.SalesOrderArray.length; i++) {
                $scope.SalesOrderArray[i].HideButton = false;
            }

            $scope.SalesOrderRow = {};
            $scope.showSavebutton = false;
            $scope.SOcalculationoftotal();
            if ($scope.SalesOrderHeader.BillDiscount != undefined)
                $scope.SOpendingcalulation();
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.SODeleteItemRow = function (index) {
        if ($scope.SalesOrderArray.length > 0) {
            $scope.SalesOrderArray.splice(index, 1);
            $scope.SOcalculationoftotal();
            if ($scope.SalesOrderHeader.BillDiscount != undefined)
                $scope.SOpendingcalulation();
        }
    };

    $scope.SOcalculationoftotal = function () {
        $scope.SalesOrderHeader.TotalBF = 0;
        $scope.SalesOrderHeader.TaxAmount = 0;
        for (var i = 0; i < $scope.SalesOrderArray.length; i++) {
            $scope.SalesOrderHeader.TotalBF = $scope.SalesOrderArray[i].RowTotal + $scope.SalesOrderHeader.TotalBF;
            $scope.SalesOrderHeader.TaxAmount = $scope.SalesOrderArray[i].TaxAmount + $scope.SalesOrderHeader.TaxAmount;
        }
        $scope.SalesOrderHeader.BillTotal = $scope.SalesOrderHeader.TotalBF;
    };

    $scope.SOpendingcalulation = function () {
        var GtotalWithDiscount = $scope.SalesOrderHeader.BillTotal * (parseInt($scope.SalesOrderHeader.BillDiscount) / 100);
        $scope.SalesOrderHeader.BillTotal = $scope.SalesOrderHeader.BillTotal - GtotalWithDiscount;
    };

    $scope.SORowTotalCaluc = function () {
        if ($scope.SalesOrderRow.Discount == undefined) {
            var RowTotal = $scope.SalesOrderRow.Qty * parseInt($scope.SalesOrderRow.UnitPrice);
            $scope.SalesOrderRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Rate;
            $scope.SalesOrderRow.TaxAmount = RowTotal * $scope.SalesOrderRow.TaxRate / 100;
            $scope.SalesOrderRow.RowTotal = $scope.SalesOrderRow.TaxAmount + RowTotal;
        }
        else {
            $scope.SalesOrderRow.TaxRate = $scope.OSTCData.find(x => x.Code === $scope.SalesOrderRow.Taxcode).Rate;
            var DiscountPercent = $scope.SalesOrderRow.Discount / 100;
            var RowTotal = $scope.SalesOrderRow.Qty * parseInt($scope.SalesOrderRow.UnitPrice);
            var DiscountPrice = RowTotal * DiscountPercent;
            var TotalBeforeDiscount = RowTotal - DiscountPrice;
            $scope.SalesOrderRow.TaxAmount = TotalBeforeDiscount * $scope.SalesOrderRow.TaxRate / 100;
            $scope.SalesOrderRow.RowTotal = $scope.SalesOrderRow.TaxAmount + TotalBeforeDiscount;
        }
    };

    $scope.SOValidateItemRow = function () {
        if ($scope.SalesOrderRow.ItemCode != undefined && $scope.SalesOrderRow.ItemName != undefined && $scope.SalesOrderRow.Qty != undefined
            && $scope.SalesOrderRow.Taxcode != undefined
        )
            return true;
        else
            return false;
    };

    $scope.SubmitSalesOrder = function () {
        try {
            $scope.SalesOrderHeader.Row = [];
            if ($scope.SalesOrderArray.length > 0) {
                $scope.SalesOrderHeader.Row = $scope.SalesOrderArray;
                $scope.showloader();
                $http.post('/api/WebAPI/PunchSalesOrder', $scope.SalesOrderHeader).then(function (response) {
                    if (response.data.startsWith('Sales')) {
                        $scope.HideLoader();
                        // localStorage.clear();
                        localStorage.removeItem("SaleQuotation");
                        $scope.alert('Success !!', response.data, 'btn-success', 'green');

                    }
                    else {
                        $scope.HideLoader();
                        $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                    }

                }).catch(function (response) {
                    $scope.HideLoader();
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response.data);
                });
            }
            else {
                $scope.alert('Alert !!', 'Kindly Add Item !!', 'btn-danger', 'red');
            }
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    /*------------------------------------------------------------------------------------------------------------------
    
                                            AR Invoice (POS)
    
     -------------------------------------------------------------------------------------------------------------------*/
    $scope.ARInvoiceINIT = function () {
        try {

            $scope.ARInvoiceHeader = {};
            $scope.ARInvoiceRow = {};
            $scope.ARInvoiceArray = [];
            $scope.ARInvoiceRow.HideButton = false;
            $scope.ARInvoiceHeader.ValidDate = moment();
            $scope.ARInvoiceHeader.PostingDate = moment();
            $scope.ARInvoiceHeader.DocDate = moment();
            var data = localStorage.getItem("QuotationFromAssign");
            $scope.AssginQuoteData = JSON.parse(data);
            $http.get('/api/WebAPI/ARInvoiceINIT').then(function (response) {
                $scope.OcrdData = response.data.OCRD;
                $scope.OCPRdata = response.data.OCPR;
                $scope.OITMData = response.data.OITM;
                $scope.OSTCData = response.data.OSTC;
                $scope.OBPLData = response.data.OBPL;
                $scope.SeriesData = response.data.Series;
                $scope.OSCN = response.data.OSCN;
                $scope.ARInvoiceHeader.DocNumber = response.data.DocNum;
                $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.ARInvoiceHeader.CustomerCode);
                $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.ARInvoiceHeader.CustomerCode);
                $scope.OSAC = response.data.OSAC;

                $scope.PunchQuote = true;
                $scope.HideLoader();
            }).catch(function (response) {
                $scope.HideLoader();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            })
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }

    };

    $scope.afterSelectARseries = function (seriesCode) {
        $scope.ARInvoiceHeader.BranchCode = ($scope.SeriesData.find(x => x.Series == parseInt(seriesCode)).BPLId).toString();
    };

    $scope.AfterSelectedARBP = function (selected) {
        try {
            angular.element('#ARCustomerCode_value').val(selected.originalObject.CardCode);
            angular.element('#ARCustomerName_value').val(selected.originalObject.CardName);
            $scope.ARInvoiceHeader.CustomerCode = selected.originalObject.CardCode;
            $scope.ARInvoiceHeader.CustomerName = selected.originalObject.CardName;
            $scope.OCPR = $scope.OCPRdata.filter(x => x.CardCode === $scope.ARInvoiceHeader.CustomerCode);
            $scope.OSCNData = $scope.OSCN.filter(x => x.CardCode === $scope.ARInvoiceHeader.CustomerCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.AfterSelectARSAC = function (selected) {
        try {
            angular.element('#SOCatno_value').val(selected.originalObject.ServCode);
            angular.element('#ARSAC_value').val(selected.originalObject.ServName);
            $scope.ARInvoiceRow.BpCatNo = selected.originalObject.CatNo;
            $scope.ARInvoiceRow.ServCode = selected.originalObject.ServCode;
            $scope.ARInvoiceRow.ServName = selected.originalObject.ServName;
            //$scope.GetSubCatNumByItemCode($scope.ARInvoiceRow.ItemCode);
        }
        catch (ex) {
            console.log(ex);
        }
    };

    //-------------------------------------Custom Function---------------------------

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
                },
                OK: {
                    text: 'Copy To Sales Order',
                    btnClass: 'btn-success',
                    action: function () {
                        $scope.CopyQuotationToOrder();
                    }
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


});
/*===========================================END Sales Oppotunity ==============================*/

app.controller('BOM', function ($rootScope, $scope, $http, NgTableParams, $ngConfirm, $q, Upload) {

    $scope.BOMINIT = function () {
        try {
            $scope.BOMHeader = {};
            $scope.BOMRow = {};
            $scope.BOMArray = [];
            $scope.BOMRow.HideButton = false;
            $http.get('/api/WebAPI/BOMINIT').then(function (response) {
                $scope.OITMData = response.data.OITM;
                $scope.OWHSData = response.data.OWHS;
                $scope.OPLNdata = response.data.OPLN;
                $scope.HideLoader();
            }).catch(function (response) {
                $scope.HideLoader();
                $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                console.log(response.data);
            })

        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.AfterSelectProduct = function (selected) {
        try {
            angular.element('#ProductCode_value').val(selected.originalObject.ItemCode);
            angular.element('#ProductDes_value').val(selected.originalObject.ItemName);
            $scope.BOMHeader.ProductCode = selected.originalObject.ItemCode;
            $scope.BOMHeader.ProductDescription = selected.originalObject.ItemName;
            //   $scope.OCPR = $scope.OITMData.find(x => x.ItemCode === $scope.BOMHeader.ItemCode).UgpEntry;
        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.AfterSelectItem = function (selected) {
        try {
            angular.element('#Itemno_value').val(selected.originalObject.ItemCode);
            angular.element('#ItemName_value').val(selected.originalObject.ItemName);
            $scope.BOMRow.ItemCode = selected.originalObject.ItemCode;
            $scope.BOMRow.ItemName = selected.originalObject.ItemName;
            // $scope.BOMRow.UOM = $scope.OITMData.find(x => x.ItemCode === $scope.BOMRow.ItemCode).UgpEntry;
        }
        catch (ex) {
            console.log(ex);
        }
    };

    $scope.AddNewItemRow = function () {
        if ($scope.ValidateItemRow()) {

            $scope.BOMArray.push($scope.BOMRow);
            if ($scope.BOMArray.length > 0) {
                $scope.BOMRow = {};
                $scope.BOMRow.HideButton = false;
                $scope.$broadcast('angucomplete-alt:clearInput', 'Itemno');
                $scope.$broadcast('angucomplete-alt:clearInput', 'ItemName');
                // $scope.calculationoftotal();
            }
        }
        else
            $scope.alert('Alert !!', 'Please Fill Required Fields', 'btn-danger', 'red');
    };

    $scope.ValidateItemRow = function () {
        if ($scope.BOMRow.ItemCode != undefined && $scope.BOMRow.ItemName != undefined && $scope.BOMRow.ItemQty != undefined && $scope.BOMRow.Warehouse != undefined) {
            return true;
        }
        else
            return false;
    };

    $scope.EditItemRow = function (index) {
        try {
            if ($scope.BOMArray.length > 0) {
                $scope.$broadcast('angucomplete-alt:changeInput', 'Itemno', $scope.BOMArray[index].ItemCode);
                $scope.$broadcast('angucomplete-alt:changeInput', 'ItemName', $scope.BOMArray[index].ItemName);
                $scope.BOMRow = $scope.BOMArray[index];
                $scope.showSavebutton = true;
                for (var i = 0; i < $scope.BOMArray.length; i++) {
                    if (i == index)
                        $scope.BOMArray[i].HideButton = true;
                    else
                        $scope.BOMArray[i].HideButton = false;
                }
            }
        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.UpdateItemRow = function (index) {

        try {
            $scope.$broadcast('angucomplete-alt:clearInput', 'Itemno');
            $scope.$broadcast('angucomplete-alt:clearInput', 'ItemName');
            $scope.BOMArray[index] = $scope.BOMRow;
            for (var i = 0; i < $scope.BOMArray.length; i++) {
                $scope.BOMArray[i].HideButton = false;
            }
            $scope.BOMRow = {};
            $scope.showSavebutton = false;

        }
        catch (ex) {
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };

    $scope.DeleteItemRow = function (index) {
        if ($scope.BOMArray.length > 0) {
            $scope.BOMArray.splice(index, 1);
        }
    };

    $scope.SubmitBom = function () {
        try {
            $scope.BOMHeader.Row = [];
            if ($scope.BOMArray.length > 0) {
                $scope.BOMHeader.Row = $scope.BOMArray;
                $scope.showloader();
                $http.post('/api/WebAPI/PunchBom', $scope.BOMHeader).then(function (response) {
                    if (response.data.startsWith('Bill')) {
                        $scope.HideLoader();
                        $scope.alert('Success !!', response.data, 'btn-success', 'green');

                    }
                    else {
                        $scope.HideLoader();
                        $scope.alert('Alert !!', response.data, 'btn-danger', 'red');
                    }

                }).catch(function (response) {
                    $scope.HideLoader();
                    $scope.alert('Alert !!', response.data.ExceptionMessage, 'btn-danger', 'red');
                    console.log(response.data);
                });
            }
            else {
                $scope.alert('Alert !!', 'Kindly Add Item !!', 'btn-danger', 'red');
            }
        }
        catch (ex) {
            $scope.HideLoader();
            $scope.alert('Alert !!', ex, 'btn-danger', 'red');
            console.log(ex);
        }
    };
    //-----------------------------------------------------------Custom Function-------------------------------
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
                },
                OK: {
                    text: 'Copy To Sales Order',
                    btnClass: 'btn-success',
                    action: function () {
                        $scope.CopyQuotationToOrder();
                    }
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

});

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