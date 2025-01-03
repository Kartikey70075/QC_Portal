jQuery.noConflict();

jQuery(function ($) {
    'use strict';
    $(function () {
        var Chart = c3.generate({
            bindto: '#c3-line-chart',
            data: {
                columns: [
                    ['data1', 30, 200, 100, 400, 150, 250],
                    ['data2', 50, 20, 10, 40, 15, 25]
                ]
            },
            color: {
                pattern: ['ff5252', '039be5', '67d7e8']
            },
            padding: {
                top: 0,
                right: 0,
                bottom: 30,
                left: 0,
            }
        });

        setTimeout(function () {
            Chart.load({
                columns: [
                    ['data1', 230, 190, 300, 350, 300, 400]
                ]
            });
        }, 1000);

        setTimeout(function () {
            Chart.load({
                columns: [
                    ['data3', 130, 150, 200, 300, 200, 100]
                ]
            });
        }, 1500);

        setTimeout(function () {
            Chart.load({
                ids: 'data1'
            });
        }, 2000);

        var c3SplineChart = c3.generate({
            bindto: '#c3-spline-chart',
            data: {
                columns: [
                    ['data1', 30, 200, 100, 400, 150, 250],
                    ['data2', 130, 100, 140, 200, 150, 50]
                ],
                type: 'spline'
            },
            color: {
                pattern: ['rgba(88,216,163,1)', 'rgba(237,28,36,0.6)', 'rgba(4,189,254,0.6)']
            },
            padding: {
                top: 0,
                right: 0,
                bottom: 30,
                left: 0,
            }
        });
        var c3BarChart = c3.generate({
            bindto: '#c3-bar-chart',
            data: {
                columns: [
                    ['data1', 30, 200, 100, 400, 150, 250],
                    ['data2', 130, 100, 140, 200, 150, 50]
                ],
                type: 'bar'
            },
            color: {
                pattern: ['rgba(88,216,163,1)', 'rgba(4,189,254,0.6)', 'rgba(237,28,36,0.6)']
            },
            padding: {
                top: 0,
                right: 0,
                bottom: 30,
                left: 0,
            },
            bar: {
                width: {
                    ratio: 0.7 // this makes bar width 50% of length between ticks
                }
            }
        });

        setTimeout(function () {
            c3BarChart.load({
                columns: [
                    ['data3', 130, -150, 200, 300, -200, 100]
                ]
            });
        }, 1000);

        var c3StepChart = c3.generate({
            bindto: '#c3-step-chart',
            data: {
                columns: [
                    ['data1', 300, 350, 300, 0, 0, 100],
                    ['data2', 130, 100, 140, 200, 150, 50]
                ],
                types: {
                    data1: 'step',
                    data2: 'area-step'
                }
            },
            color: {
                pattern: ['rgba(88,216,163,1)', 'rgba(4,189,254,0.6)', 'rgba(237,28,36,0.6)']
            },
            padding: {
                top: 0,
                right: 0,
                bottom: 30,
                left: 0,
            }
        });

        var count = 0;
        //Bill Total starts

        //$.ajax({
        //    url: "/Home/GetBillTotal", type: 'post', success: function (result) {
        //        var BillTotal = result[0].BillTotal;
        //        var SameStateTotal = result[0].SameStateTotal;
        //        var OtherStateTotal = result[0].OtherStateTotal;

        //        var c3PieChart = c3.generate({
        //            bindto: '#c3-pie-chart',
        //            data: {
        //                // iris data from R
        //                columns: [
        //                    ["WithinStateTotal : Rs." + SameStateTotal, SameStateTotal],
        //                    ["OtherStateTotal : Rs." + OtherStateTotal, OtherStateTotal],
        //                ],
        //                type: 'pie',
        //                onclick: function (d, i) {
        //                    console.log("onclick", d, i);
        //                },
        //                onmouseover: function (d, i) {
        //                    console.log("onmouseover", d, i);
        //                },
        //                onmouseout: function (d, i) {
        //                    console.log("onmouseout", d, i);
        //                }
        //            },
        //            color: {
        //                pattern: ['#75bbe4', '#ea96f5', '#8f99da']
        //            },
        //            padding: {
        //                top: 0,
        //                right: 0,
        //                bottom: 30,
        //                left: 0,
        //            }
        //        });
        //        $('#lbl').text('Rs.' + BillTotal);
        //        count++;
        //        if (count === 3) {
        //            $('#loading').hide();
        //        }
        //    }
        //});

        //$('#dtbillfrm,#dtbillto').change(function () {
        //    if ($('#dtbillfrm').val() !== undefined && $('#dtbillfrm').val() !== '' && $('#dtbillto').val() !== undefined && $('#dtbillto').val() !== '') {
        //        var from = new Date($('#dtbillfrm').val().split('/')[1] + '/' + $('#dtbillfrm').val().split('/')[0] + '/' + $('#dtbillfrm').val().split('/')[2]);
        //        var to = new Date($('#dtbillto').val().split('/')[1] + '/' + $('#dtbillto').val().split('/')[0] + '/' + $('#dtbillto').val().split('/')[2]);
        //        if (from > to) {
        //            alert('Invalid Date Range !!');
        //        } else {
        //            $('#loading').show();
        //            $('*').attr('tabindex', '-1');
        //            $.ajax({
        //                url: "/Home/GetBillTotal",
        //                type: 'post',
        //                data: "{'dtbillfrm':'" + $('#dtbillfrm').val() + "','dtbillto':'" + $('#dtbillto').val() + "'}",
        //                contentType: "application/json; charset=utf-8",
        //                dataType: "json",
        //                success: function (result) {
        //                    var BillTotal = result[0].BillTotal;
        //                    var SameStateTotal = result[0].SameStateTotal;
        //                    var OtherStateTotal = result[0].OtherStateTotal;

        //                    var c3PieChart = c3.generate({
        //                        bindto: '#c3-pie-chart',
        //                        data: {
        //                            // iris data from R
        //                            columns: [
        //                                ["WithinStateTotal : Rs." + SameStateTotal, SameStateTotal],
        //                                ["OtherStateTotal : Rs." + OtherStateTotal, OtherStateTotal],
        //                            ],
        //                            type: 'pie',
        //                            onclick: function (d, i) {
        //                                console.log("onclick", d, i);
        //                            },
        //                            onmouseover: function (d, i) {
        //                                console.log("onmouseover", d, i);
        //                            },
        //                            onmouseout: function (d, i) {
        //                                console.log("onmouseout", d, i);
        //                            }
        //                        },
        //                        color: {
        //                            pattern: ['#75bbe4', '#ea96f5', '#8f99da']
        //                        },
        //                        padding: {
        //                            top: 0,
        //                            right: 0,
        //                            bottom: 30,
        //                            left: 0,
        //                        }
        //                    });
        //                    $('#lbl').text('Rs.' + BillTotal);
        //                    $('#loading').hide();
        //                    $('*').removeAttr('tabindex');
        //                    //setTimeout(function () {
        //                    //    c3PieChart.unload({
        //                    //        ids: 'SameStateTotal : Rs.'
        //                    //    });
        //                    //    c3PieChart.unload({
        //                    //        ids: 'OtherStateTotal : Rs.'
        //                    //    });
        //                    //}, 2500);

        //                    //setTimeout(function () {
        //                    //    c3PieChart.load({
        //                    //        columns: [
        //                    //            ["SameStateTotal : Rs." + SameStateTotal, SameStateTotal],
        //                    //            ["OtherStateTotal : Rs." + OtherStateTotal, OtherStateTotal],
        //                    //        ]
        //                    //    });
        //                    //}, 1500);
        //                    //$('#lbl').text('Rs.' + BillTotal);
        //                }
        //            });
        //        }
        //    }
        //});

        //$.ajax({
        //    url: "/Home/GetTaxTotal", type: 'post', success: function (result) {
        //        var Total_Amount = result[0].Total_Amount;
        //        var IGSTAmt = result[0].IGSTAmt;
        //        var CGSTAmt = result[0].CGSTAmt;
        //        var SGSTAmt = result[0].SGSTAmt;
        //        var SGST_CGST_Amt = result[0].SGST_CGST_Amt;

        //        var c3DonutChart = c3.generate({
        //            bindto: '#c3-donut-chart',
        //            data: {
        //                columns: [
        //                    ['IGSTAmt : Rs.' + IGSTAmt, IGSTAmt],
        //                    ['CGSTAmt : Rs.' + CGSTAmt, CGSTAmt],
        //                    ['SGSTAmt : Rs.' + SGSTAmt, SGSTAmt]
        //                ],
        //                type: 'donut',
        //                onclick: function (d, i) {
        //                    console.log("onclick", d, i);
        //                },
        //                onmouseover: function (d, i) {
        //                    console.log("onmouseover", d, i);
        //                },
        //                onmouseout: function (d, i) {
        //                    console.log("onmouseout", d, i);
        //                }
        //            },
        //            color: {
        //                pattern: ['rgba(88,216,163,1)', 'rgba(4,189,254,0.6)', 'rgba(237,28,36,0.6)']
        //            },
        //            padding: {
        //                top: 0,
        //                right: 0,
        //                bottom: 30,
        //                left: 0,
        //            },
        //            donut: {
        //                title: "Tax"
        //            }
        //        });
        //        $('#lbltax').text('Rs.' + Total_Amount);
        //        count++;
        //        if (count === 3) {
        //            $('#loading').hide();
        //        }
        //    }
        //});

        //$('#dttaxfrm,#dttaxto').change(function () {
        //    if ($('#dttaxfrm').val() !== undefined && $('#dttaxfrm').val() !== '' && $('#dttaxto').val() !== undefined && $('#dttaxto').val() !== '') {
        //        var from = new Date($('#dttaxfrm').val().split('/')[1] + '/' + $('#dttaxfrm').val().split('/')[0] + '/' + $('#dttaxfrm').val().split('/')[2]);
        //        var to = new Date($('#dttaxto').val().split('/')[1] + '/' + $('#dttaxto').val().split('/')[0] + '/' + $('#dttaxto').val().split('/')[2]);
        //        if (from > to) {
        //            alert('Invalid Date Range !!');
        //        } else {
        //            $('#loading').show();
        //            $('*').attr('tabindex', '-1');
        //            $.ajax({
        //                url: "/Home/GetTaxTotal",
        //                type: 'post',
        //                data: "{'dttaxfrm':'" + $('#dttaxfrm').val() + "','dttaxto':'" + $('#dttaxto').val() + "'}",
        //                contentType: "application/json; charset=utf-8",
        //                dataType: "json",
        //                success: function (result) {
        //                    var Total_Amount = result[0].Total_Amount;
        //                    var IGSTAmt = result[0].IGSTAmt;
        //                    var CGSTAmt = result[0].CGSTAmt;
        //                    var SGSTAmt = result[0].SGSTAmt;
        //                    var SGST_CGST_Amt = result[0].SGST_CGST_Amt;

        //                    var c3DonutChart = c3.generate({
        //                        bindto: '#c3-donut-chart',
        //                        data: {
        //                            columns: [
        //                                ['IGSTAmt : Rs.' + IGSTAmt, IGSTAmt],
        //                                ['CGSTAmt : Rs.' + CGSTAmt, CGSTAmt],
        //                                ['SGSTAmt : Rs.' + SGSTAmt, SGSTAmt]
        //                            ],
        //                            type: 'donut',
        //                            onclick: function (d, i) {
        //                                console.log("onclick", d, i);
        //                            },
        //                            onmouseover: function (d, i) {
        //                                console.log("onmouseover", d, i);
        //                            },
        //                            onmouseout: function (d, i) {
        //                                console.log("onmouseout", d, i);
        //                            }
        //                        },
        //                        color: {
        //                            pattern: ['rgba(88,216,163,1)', 'rgba(4,189,254,0.6)', 'rgba(237,28,36,0.6)']
        //                        },
        //                        padding: {
        //                            top: 0,
        //                            right: 0,
        //                            bottom: 30,
        //                            left: 0,
        //                        },
        //                        donut: {
        //                            title: "Tax"
        //                        }
        //                    });
        //                    $('#lbltax').text('Rs.' + Total_Amount);
        //                    $('#loading').hide();
        //                    $('*').removeAttr('tabindex');
        //                    //setTimeout(function () {
        //                    //    c3PieChart.unload({
        //                    //        ids: 'SameStateTotal : Rs.'
        //                    //    });
        //                    //    c3PieChart.unload({
        //                    //        ids: 'OtherStateTotal : Rs.'
        //                    //    });
        //                    //}, 2500);

        //                    //setTimeout(function () {
        //                    //    c3PieChart.load({
        //                    //        columns: [
        //                    //            ["SameStateTotal : Rs." + SameStateTotal, SameStateTotal],
        //                    //            ["OtherStateTotal : Rs." + OtherStateTotal, OtherStateTotal],
        //                    //        ]
        //                    //    });
        //                    //}, 1500);
        //                    //$('#lbl').text('Rs.' + BillTotal);
        //                }
        //            });
        //        }
        //    }
        //});
        //var datas = [];
        //if ($('#morris-bar-chart').length > 0)
        //    // Bar Chart
        //    $.ajax({
        //        url: "/Home/GetTaxPercent", type: 'post', success: function (result) {
        //            $('#lbltaxper').text('Rs.' + result[0].TAXSUM);
        //            result.splice(0, 1);
        //            datas = result;
        //            Morris.Bar({
        //                element: 'morris-bar-chart',
        //                data: datas,
        //                xkey: 'Taxpecent',
        //                ykeys: ['TAXSUM'],
        //                labels: [''],
        //                barRatio: 0.4,
        //                xLabelAngle: 35,
        //                pointSize: 1,
        //                barOpacity: 1,
        //                pointStrokeColors: ['#4aa23c'],
        //                behaveLikeLine: true,
        //                grid: false,
        //                gridTextColor: '#878787',
        //                hideHover: 'auto',
        //                barColors: ['#67d7e8'],
        //                resize: true

        //            });
        //            count++;
        //            if (count === 3) {
        //                $('#loading').hide();
        //            }
        //        }
        //    });

        //$('#taxfrom,#taxto').change(function () {
        //    if ($('#taxfrom').val() !== undefined && $('#taxfrom').val() !== '' && $('#taxto').val() !== undefined && $('#taxto').val() !== '') {
        //        var from = new Date($('#taxfrom').val().split('/')[1] + '/' + $('#taxfrom').val().split('/')[0] + '/' + $('#taxfrom').val().split('/')[2]);
        //        var to = new Date($('#taxto').val().split('/')[1] + '/' + $('#taxto').val().split('/')[0] + '/' + $('#taxto').val().split('/')[2]);
        //        if (from > to) {
        //            alert('Invalid Date Range !!');
        //        } else {
        //            $('#loading').show();
        //            $('*').attr('tabindex', '-1');
        //            datas = [];
        //            $('#morris-bar-chart').empty();
        //            $.ajax({
        //                url: "/Home/GetTaxPercent",
        //                type: 'post',
        //                data: "{'taxfrom':'" + $('#taxfrom').val() + "','taxto':'" + $('#taxto').val() + "'}",
        //                contentType: "application/json; charset=utf-8",
        //                dataType: "json",
        //                success: function (result) {
        //                    $('#lbltaxper').text('Rs.' + result[0].TAXSUM);
        //                    result.splice(0, 1);
        //                    datas = result;
        //                    Morris.Bar({
        //                        element: 'morris-bar-chart',
        //                        data: datas,
        //                        xkey: 'Taxpecent',
        //                        ykeys: ['TAXSUM'],
        //                        labels: [''],
        //                        barRatio: 0.4,
        //                        xLabelAngle: 35,
        //                        pointSize: 1,
        //                        barOpacity: 1,
        //                        pointStrokeColors: ['#4aa23c'],
        //                        behaveLikeLine: true,
        //                        grid: false,
        //                        gridTextColor: '#878787',
        //                        hideHover: 'auto',
        //                        barColors: ['#67d7e8'],
        //                        resize: true

        //                    });
        //                    $('#loading').hide();
        //                    $('*').removeAttr('tabindex');

        //                }
        //            });
        //        }
        //    }
        //});


        //$.ajax({
        //    url: "/Home/GetTaxTotal", type: 'post', success: function (result) {
        //        var Total_Amount = result[0].Total_Amount;
        //        var IGSTAmt = result[0].IGSTAmt;
        //        var CGSTAmt = result[0].CGSTAmt;
        //        var SGSTAmt = result[0].SGSTAmt;
        //        var SGST_CGST_Amt = result[0].SGST_CGST_Amt;

        //        setTimeout(function () {
        //            c3DonutChart.load({
        //                columns: [
        //                    ["IGSTAmt : Rs." + IGSTAmt, IGSTAmt],
        //                    ["CGSTAmt : Rs." + CGSTAmt, CGSTAmt],
        //                    ["SGSTAmt : Rs." + SGSTAmt, SGSTAmt],
        //                ]
        //            });
        //        }, 1500);
        //        $('#lbltax').text('Rs.' + Total_Amount);
        //    }
        //});

        //setTimeout(function () {
        //    c3DonutChart.unload({
        //        ids: 'data1'
        //    });
        //    c3DonutChart.unload({
        //        ids: 'data2'
        //    });
        //}, 2500);
    });
});