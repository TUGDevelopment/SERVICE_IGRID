var pp_quill;
//Search
function filterGlobal() {
    $('#table_incoming_inbox').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_incoming_artwork').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_igrid_view').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_igrid_infogroup_view').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_incoming_artwork_for_pg').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_incoming_mockup').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_incoming_inbox_vendor').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_incoming_inbox_customer').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_so_new').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_so_repeat').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_vendor_po_received').DataTable().search(
        $('#global_filter').val()
    ).draw();
    $('#table_pp_view').DataTable().search(
        $('#global_filter').val()
    ).draw();
}
//Chart
function drawChart() {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Effort');
    data.addColumn('number', 'Amount given');
    data.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });
    data.addRows([
        ['Incoming', cntIncoming, cntIncoming_txt],
        ['In progress', cntInProgress, cntInProgress_txt],
        ['Pool', cntPool, cntPool_txt]
    ]);

    var piechart_options = {
        fontSize: 13,
        height: 300,
        title: 'Total of workflow',
        colors: ['#109618', '#3366cc', '#ff9900'],
        chartArea: { left: 20, top: 50, width: "100%" },
        pieSliceText: 'value',
        tooltip: { isHtml: true } // This MUST be set to true for your chart to show.
    };

    var chart = new google.visualization.PieChart(document.getElementById('div_piechart'));

    if ((cntIncoming + cntInProgress + cntPool) > 0)
        chart.draw(data, piechart_options);

    if (UserPosition == "CUSTOMER" || UserPosition == "VENDOR") {
    }
    else {
        var dataColumnChart = new google.visualization.DataTable();
        dataColumnChart.addColumn('string', 'Effort');
        dataColumnChart.addColumn('number', 'Amount');
        dataColumnChart.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });
        dataColumnChart.addRows([
            ['Waiting for primary PKG', cntWaitingPKG, cntWaitingPKG_txt],
            ['Waiting for quotation', cntWaitingQuo, cntWaitingQuo_txt],
            ['Waiting for sample', cntWaitingSample, cntWaitingSample_txt],
            ['Waiting for customer approval (mockup)', cntWaitingCustomer, cntWaitingCustomer_txt],
        ]);

        var view = new google.visualization.DataView(dataColumnChart);
        view.setColumns([0, 1,
            {
                calc: "stringify",
                sourceColumn: 1,
                type: "string",
                role: "annotation"
            },
            2]);

        var total = parseInt(cntWaitingPKG) + parseInt(cntWaitingQuo) + parseInt(cntWaitingSample) + parseInt(cntWaitingCustomer);
        var columnchart_options = {
            tooltip: { isHtml: true },// This MUST be set to true for your chart to show.
            fontSize: 13,
            title: 'Count by workflow status ' + '(Total ' + total + ')',
            height: 300,
            legend: 'none',
            vAxis: {
                gridlineColor: '#fff',
                textPosition: 'none'
            },
            bar: { groupWidth: "30%" },
            chartArea: { left: 20, top: 50, width: "100%" }
        };
        var barchart = new google.visualization.ColumnChart(document.getElementById('div_columnchart_pg'));
        barchart.draw(view, columnchart_options);

        chart_pa();
        chart_mk();

        $(".cls_ddl_chart_view").change();
    }
}

function chart_pa() {
    //var dataColumnChart = google.visualization.arrayToDataTable([
    //    ['Element', 'Total'],
    //    // RGB value
    //    ['Waiting for QC confirmation', cntWaitingQCConfirmation],
    //    ['Waiting for print master', 0],
    //    ['Waiting for customer approval print master', 0],
    //    ['Waiting for shade limit (vendor)', 0],
    //    ['Waiting for customer approval shade limit', 0],
    //]);

    var dataColumnChart = new google.visualization.DataTable();
    dataColumnChart.addColumn('string', 'Effort');
    dataColumnChart.addColumn('number', 'Amount');
    dataColumnChart.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });
    dataColumnChart.addRows([
        ['Waiting for QC confirmation', cntWaitingQCConfirmation, cntWaitingQCConfirmation_txt],
        ['Waiting for print master', cntWaitingPrintmaster, cntWaitingPrintmaster_txt],
        ['Waiting for customer approval print master', cntWaitingCustomerApprovePrintMaster, cntWaitingCustomerApprovePrintMaster_txt],
        ['Waiting for shade limit (vendor)', cntWaitingShadeLimit, cntWaitingShadeLimit_txt],
        ['Waiting for customer approval shade limit', cntWaitingCustomerApproveShadeLimit, cntWaitingCustomerApproveShadeLimit_txt],
    ]);

    var view = new google.visualization.DataView(dataColumnChart);
    view.setColumns([0, 1,
        {
            calc: "stringify",
            sourceColumn: 1,
            type: "string",
            role: "annotation"
        },
        2]);

    var total = parseInt(cntWaitingQCConfirmation) + parseInt(cntWaitingPrintmaster) + parseInt(cntWaitingCustomerApprovePrintMaster) + parseInt(cntWaitingShadeLimit) + parseInt(cntWaitingCustomerApproveShadeLimit);
    var columnchart_options = {
        tooltip: { isHtml: true },// This MUST be set to true for your chart to show.
        fontSize: 13,
        title: 'Count by workflow status ' + '(Total ' + total + ')',
        height: 300,
        legend: 'none',
        vAxis: {
            gridlineColor: '#fff',
            textPosition: 'none'
        },
        bar: { groupWidth: "40%" },
        chartArea: { left: 20, top: 50, width: "100%" }
    };
    var barchart = new google.visualization.ColumnChart(document.getElementById('div_columnchart_pa'));
    barchart.draw(view, columnchart_options);
    $('.cls_columnchart_pa').hide();
}

function chart_mk() {
    //var dataColumnChart = google.visualization.arrayToDataTable([
    //    ['Element', 'Total'],
    //    // RGB value
    //    ['Waiting for customer approval (mockup)', 0],
    //    ['Waiting for customer approval print master', 0],
    //    ['Waiting for customer approval shade limit', 0],
    //    ['Waiting for approval (GM MK)', 0],
    //    ['Waiting for customer review', 0],
    //]);

    var dataColumnChart = new google.visualization.DataTable();
    dataColumnChart.addColumn('string', 'Effort');
    dataColumnChart.addColumn('number', 'Amount');
    dataColumnChart.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });
    dataColumnChart.addRows([
        ['Waiting for customer approval (mockup)', cntWaitingCustomer, cntWaitingCustomer_txt],
        ['Waiting for customer approval print master', cntWaitingCustomerApprovePrintMaster, cntWaitingCustomerApprovePrintMaster_txt],
        ['Waiting for customer approval shade limit', cntWaitingCustomerApproveShadeLimit, cntWaitingCustomerApproveShadeLimit_txt],
        ['Waiting for approval (GM MK)', cntWaitingGMMK, cntWaitingGMMK_txt],
        ['Waiting for customer review', cntWaitingCustomerReview, cntWaitingCustomerReview_txt],
    ]);

    var total = parseInt(cntWaitingCustomer) + parseInt(cntWaitingCustomerApprovePrintMaster) + parseInt(cntWaitingCustomerApproveShadeLimit) + parseInt(cntWaitingGMMK) + parseInt(cntWaitingCustomerReview);
    var view = new google.visualization.DataView(dataColumnChart);
    view.setColumns([0, 1,
        {
            calc: "stringify",
            sourceColumn: 1,
            type: "string",
            role: "annotation"
        },
        2]);

    var columnchart_options = {
        tooltip: { isHtml: true },// This MUST be set to true for your chart to show.
        fontSize: 13,
        title: 'Count by workflow status ' + '(Total ' + total + ')',
        height: 300,
        legend: 'none',
        vAxis: {
            gridlineColor: '#fff',
            textPosition: 'none'
        },
        bar: { groupWidth: "40%" },
        chartArea: { left: 20, top: 50, width: "100%" }
    };
    var barchart = new google.visualization.ColumnChart(document.getElementById('div_columnchart_mk'));
    barchart.draw(view, columnchart_options);
    $('.cls_columnchart_mk').hide();
}

var first_load_sendtopp = true;
var first_load_sendtovendor = true;
$(document).ready(function () {


    $('.cls_btn_create_igrid').hide();

    $(".cls_show_hide_graph").click(function () {
        if ($(".cls_row_chart").is(':hidden')) {
            $('.cls_row_chart').show();

            var myurl = '/api/dashboard/countmockupandartwork';
            var mytype = 'GET';
            var mydata = null;
            myAjax(myurl, mytype, mydata, callback_get_countmockupandartwork);
        }
        else $('.cls_row_chart').hide();
    });
    $(".cls_ddl_chart_view").change(function () {
        $('.cls_columnchart_pg').hide();
        $('.cls_columnchart_pa').hide();
        $('.cls_columnchart_mk').hide();

        if ($(this).val() == "pg") {
            $('.cls_columnchart_pg').show();
        }
        if ($(this).val() == "pa") {
            $('.cls_columnchart_pa').show();
        }
        if ($(this).val() == "mk") {
            $('.cls_columnchart_mk').show();
        }
    });
    //debugger;
    if (UserPosition != "NOT_FOUND") {

        if (UserPosition == "CUSTOMER") {
            $('.cls_li_inbox_customer').show();
            $('a[href="#view_incoming_inbox_customer"]').tab('show');
            $('.cls_home_chart_pie').show();
            $('.cls_btn_home_page').show();
            
            $('.cls_btn_upload_file_artwork').css('display', 'inline-block');
        }
        else if (UserPosition == "VENDOR") {
            $('.cls_home_chart_pie').show();
            $('.cls_li_inbox_vendor').show();
            $('.cls_li_vendor_po_received').show();
            $('.cls_btn_home_page').show();
            $('a[href="#view_incoming_inbox_vendor"]').tab('show');
        }
        else {
            $('.cls_home_chart_pie').show();
            // $('.cls_home_chart_bar').show(); $('.cls_chart_view').show(); ticket#429878 by aof
            $('.cls_li_inbox').show();
            $('a[href="#view_incoming_inbox"]').tab('show');


            if (getiGridUserFN("PA") || getiGridUserFN("PA_Approve"))  //  IGRID REIM by aof in 08/2023
            {
                $('.cls_btn_create_igrid').show();
            }


            if (getroleuser("PG_STAFF") || getroleuser("PG_TEAM_LEAD") || getroleuser("PG_SUPPERVISOR") || getroleuser("PG_ASS_MANAGER") || getroleuser("PG_MANAGER")) {
                $('.cls_li_incoming_mockup').show();
                $('.cls_li_incoming_artwork_for_pg').show();
                $('.cls_btn_home_page').show();
                $('.cls_btn_create_check_list').css('display', 'inline-block');
                $(".cls_ddl_chart_view").val('pg');
                $('.cls_home_chart_bar').show(); $('.cls_chart_view').show();  //ticket#429878 by aof
            }
            if (getroleuser("PA_STAFF") || getroleuser("PA_TEAM_LEAD") || getroleuser("PA_SUPERVISOR") || getroleuser("PA_ASS_MANAGER") || getroleuser("PG_STAFF") || getroleuser("PG_TEAM_LEAD") || getroleuser("PG_SUPPERVISOR")
                || getroleuser("PG_MANAGER") || getroleuser("PG_ASS_MANAGER")) {
                $('.cls_li_igird').show();
            }
            if (getroleuser("PG_TEAM_LEAD") || getroleuser("PG_SUPPERVISOR") || getroleuser("PG_ASS_MANAGER") || getroleuser("PG_MANAGER")) {
                $('.cls_li_igird_infogroup').show();
            }
            if (getroleuser("PA_STAFF") || getroleuser("PA_TEAM_LEAD") || getroleuser("PA_SUPERVISOR") || getroleuser("PA_ASS_MANAGER")) {
                //debugger;
                $('.cls_li_incoming_artwork').show();
                $('.cls_li_incoming_so_new').show();
                $('.cls_li_incoming_so_repeat').show();
                //$('.cls_li_igird').show();
                
                $('.cls_btn_home_page').show();
                $('.cls_btn_upload_file_artwork').css('display', 'inline-block');
          
                $('.cls_btn_sendtopp').show();
                $(".cls_ddl_chart_view").val('pa');
                $('.cls_li_inbox_sent_pp').show();
                $('.cls_home_chart_bar').show(); $('.cls_chart_view').show();  //ticket#429878 by aof
            }
            if (getroleuser("MK_CD") || getroleuser("MK_CD_SENIOR") || getroleuser("MK_CD_AM")
                //by aof start code.
                //|| getroleuser("MC_STAFF") || getroleuser("MC_SUPERVISOR") || getroleuser("MC_AM") || getroleuser("MK_CD_MC_MANAGER")
                //|| getroleuser("MARKETING_SUPPORT_MANAGER") || getroleuser("PMC") || getroleuser("PME")) {
                //-----------------------------------------------------------------------------------------------------
                || getroleuser("MC_STAFF") || getroleuser("MCSS_MCSP") || getroleuser("MC_AM") || getroleuser("MK_CD_MC_MANAGER")
                || getroleuser("MARKETING_SUPPORT_MANAGER") || getroleuser("PMC") || getroleuser("PME_PMS") || getroleuser("PM_MS_AM")) {
                //by aof end code

                $('.cls_btn_home_page').show();
                $('.cls_btn_create_check_list').css('display', 'inline-block');
                $('.cls_btn_upload_file_artwork').css('display', 'inline-block');
                $(".cls_ddl_chart_view").val('mk');
                $('.cls_home_chart_bar').show(); $('.cls_chart_view').show();   //ticket#429878 by aof
            }
            if (getroleuser("PP_STAFF")) {
                $('.cls_btn_home_page').show();
                $('.cls_li_pp_view').show();
                $('.cls_btn_sendtovendor').show();
            }
        }
    }
    else {
        $('.dataTables_filter_dashboard').hide();
        $('.nav-tabs').hide();
    }

    //Search
    $('input.global_filter').on('keyup click', function () {
        filterGlobal();
    });

    $('.cls_row_chart').hide();
    //var myurl = '/api/dashboard/countmockupandartwork?data.first_load=true';
    //var mytype = 'GET';
    //var mydata = null;
    //myAjax(myurl, mytype, mydata, callback_get_countmockupandartwork);

    //Chart
    // Load Charts and the corechart and barchart packages.
    google.charts.load('current', { 'packages': ['corechart'] });
    // Draw the pie chart and bar chart when Charts is loaded.

    pp_quill = bind_text_editor('.cls_txt_send_pp');

    $(".cls_btn_home_page .cls_btn_sendtopp").click(function (e) {
        bindSendToPPPopUp();
        $('#modal_dashboard_sendtopp').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $('#modal_dashboard_sendtopp').on('hide.bs.modal', function () {
        var table_dashboard_sendtopp_select = $('#table_dashboard_sendtopp_select').DataTable();
        table_dashboard_sendtopp_select.destroy();
        first_load_sendtopp = false;

        pp_quill = new Quill('.cls_txt_send_pp');
        pp_quill.setContents([{ insert: '\n' }]);
    })

    $(".cls_btn_home_page .cls_btn_sendtovendor").click(function (e) {
        bindSendToVendorPopUp();
        $('#modal_dashboard_sendtovendor').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $('#modal_dashboard_sendtovendor').on('hide.bs.modal', function () {
        var table_dashboard_sendtovendor_select = $('#table_dashboard_sendtovendor_select').DataTable();
        table_dashboard_sendtovendor_select.destroy();
        first_load_sendtovendor = false;
    })

    $("#modal_dashboard_sendtopp .cls_btn_dashboard_sendtopp_select").click(function (e) {
        var table = $('#table_dashboard_sendtopp_select').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            setSelectSendToPP(tblData);
        }
        else {
            alertError2("Please select at least 1 item.");
        }
    });

    $("#modal_dashboard_sendtovendor .cls_btn_dashboard_sendtovendor_select").click(function (e) {
        var table = $('#table_dashboard_sendtovendor_select').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            setSelectSendToVendor(tblData);
        }
        else {
            alertError2("Please select at least 1 item.");
        }
    });
});

function setSelectSendToPP(tblData) {
    var data = [];
    var str_aw = "";
    if (tblData.length > 0) {
        for (i = 0; i < tblData.length; i++) {
            if (tblData[i].IS_SALES_ORDER_CHANGE != "X") {
                var item = {};
                item.PROCESS = {};
                item["ARTWORK_REQUEST_ID"] = tblData[i].ARTWORK_REQUEST_ID;
                item["ARTWORK_ITEM_ID"] = tblData[i].ARTWORK_ITEM_ID;
                item["ARTWORK_SUB_ID"] = tblData[i].ARTWORK_SUB_ID;
                item["CREATE_BY"] = UserID;
                item["UPDATE_BY"] = UserID;

                item.PROCESS["ARTWORK_REQUEST_ID"] = tblData[i].ARTWORK_REQUEST_ID;
                item.PROCESS["ARTWORK_ITEM_ID"] = tblData[i].ARTWORK_ITEM_ID;
                item.PROCESS["PARENT_ARTWORK_SUB_ID"] = tblData[i].ARTWORK_SUB_ID;
                item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_PP').curr_role;
                item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_PP').curr_step;
                //item.PROCESS["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_pp .cls_lov_send_for_reason').val();
                item.PROCESS["CREATE_BY"] = UserID;
                item.PROCESS["UPDATE_BY"] = UserID;
                var editor = new Quill('.cls_txt_send_pp');
                item.PROCESS["REMARK"] = editor.root.innerHTML;
                data.push(item);
            }
            else {
                if (str_aw.length > 0) {
                    str_aw += "," + tblData[i].WORKFLOW_NO;
                }
                else {
                    str_aw = tblData[i].WORKFLOW_NO;
                }
            }
        }
    }

    if (data.length > 0) {
        var jsonObj = new Object();
        jsonObj.data = data;

        var myurl = '/api/taskform/pa/sendpp/submit';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSelectSendToPP);
    }
    else {
        alertError2("Please accept change sales order(" + str_aw + ") before sent to vendor");
    }

}

function callbackSelectSendToPP(res) {
    if (res.status == 'S') {
        $("#modal_dashboard_sendtopp .cls_btn_dashboard_sendtopp_close").click();
    }
}

function setSelectSendToVendor(tblData) {
    var data = [];
    if (tblData.length > 0) {
        for (i = 0; i < tblData.length; i++) {
            var item = {};
            item["ARTWORK_REQUEST_ID"] = tblData[i].ARTWORK_REQUEST_ID;
            item["ARTWORK_SUB_ID"] = tblData[i].ARTWORK_SUB_ID;
            item["VENDOR_ID"] = tblData[i].VENDOR_ID;
            item["ENDTASKFORM"] = true;
            item["CONFIRM_PO"] = 'X';
            item["ACTION_CODE"] = 'SUBMIT';
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;
            item.PROCESS = {};
            item.PROCESS["ARTWORK_REQUEST_ID"] = tblData[i].ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = tblData[i].ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = tblData[i].ARTWORK_SUB_ID;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_VN_PO').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_VN_PO').curr_step;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            data.push(item);
        }
    }
    var jsonObj = new Object();
    jsonObj.data = data;

    var myurl = '/api/taskform/pp/multisendtovendor';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSelectSendToVendor);
}

function callbackSelectSendToVendor(res) {
    if (res.status == 'S') {
        $("#modal_dashboard_sendtovendor .cls_btn_dashboard_sendtovendor_close").click();
    }
}

var cntIncoming = 0;
var cntInProgress = 0;
var cntPool = 0;
var cntIncoming_txt = 0;
var cntInProgress_txt = 0;
var cntPool_txt = 0;

var cntWaitingPKG = 0;
var cntWaitingQuo = 0;
var cntWaitingSample = 0;
var cntWaitingCustomer = 0;

var cntWaitingPKG_txt = '';
var cntWaitingQuo_txt = '';
var cntWaitingSample_txt = '';
var cntWaitingCustomer_txt = '';

var cntWaitingQCConfirmation = 0;
var cntWaitingPrintmaster = 0;
var cntWaitingCustomerApprovePrintMaster = 0;
var cntWaitingShadeLimit = 0;
var cntWaitingCustomerApproveShadeLimit = 0;
var cntWaitingGMMK = 0;
var cntWaitingCustomerReview = 0;

var cntWaitingQCConfirmation_txt = '';
var cntWaitingPrintmaster_txt = '';
var cntWaitingCustomerApprovePrintMaster_txt = '';
var cntWaitingShadeLimit_txt = '';
var cntWaitingCustomerApproveShadeLimit_txt = '';
var cntWaitingGMMK_txt = '';
var cntWaitingCustomerReview_txt = '';

function callback_get_countmockupandartwork(res) {
    cntWaitingPKG = res.cntWaitingPKG;
    cntWaitingQuo = res.cntWaitingQuo;
    cntWaitingSample = res.cntWaitingSample;
    cntWaitingCustomer = res.cntWaitingCustomer;
    cntWaitingPKG_txt = res.cntWaitingPKG_txt;
    cntWaitingQuo_txt = res.cntWaitingQuo_txt;
    cntWaitingSample_txt = res.cntWaitingSample_txt;
    cntWaitingCustomer_txt = res.cntWaitingCustomer_txt;

    cntWaitingQCConfirmation = res.cntWaitingQCConfirmation;
    cntWaitingPrintmaster = res.cntWaitingPrintmaster;
    cntWaitingCustomerApprovePrintMaster = res.cntWaitingCustomerApprovePrintMaster;
    cntWaitingShadeLimit = res.cntWaitingShadeLimit;
    cntWaitingCustomerApproveShadeLimit = res.cntWaitingCustomerApproveShadeLimit;
    cntWaitingGMMK = res.cntWaitingGMMK;
    cntWaitingCustomerReview = res.cntWaitingCustomerReview;

    cntWaitingQCConfirmation_txt = res.cntWaitingQCConfirmation_txt;
    cntWaitingPrintmaster_txt = res.cntWaitingPrintmaster_txt;
    cntWaitingCustomerApprovePrintMaster_txt = res.cntWaitingCustomerApprovePrintMaster_txt;
    cntWaitingShadeLimit_txt = res.cntWaitingShadeLimit_txt;
    cntWaitingCustomerApproveShadeLimit_txt = res.cntWaitingCustomerApproveShadeLimit_txt;
    cntWaitingGMMK_txt = res.cntWaitingGMMK_txt;
    cntWaitingCustomerReview_txt = res.cntWaitingCustomerReview_txt;

    cntIncoming = res.cntIncoming;
    cntInProgress = res.cntInProgress;
    cntPool = res.cntPool;
    cntIncoming_txt = res.cntIncoming_txt;
    cntInProgress_txt = res.cntInProgress_txt;
    cntPool_txt = res.cntPool_txt;

    google.charts.setOnLoadCallback(drawChart);
}

function bindSendToPPPopUp() {
    if (first_load_sendtopp) {
        // Setup - add a text input to each footer cell
        $('#table_dashboard_sendtopp_select thead tr').clone(true).appendTo('#table_dashboard_sendtopp_select thead');
        $('#table_dashboard_sendtopp_select thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 1 || i == 2) {
                $(this).html('');
            } else if (i == 6) {
                $(this).html('<input type="text" placeholder="dd/mm/yyyy" class="form-control" data-index="' + i + '" />');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });
    }

    var groupColumn = 2;
    var table_dashboard_sendtopp_select = $('#table_dashboard_sendtopp_select').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/taskform/pa/sendpp/info",
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "orderable": false, "targets": 0 },
            { "orderable": false, "targets": 1 },
            { "visible": false, "targets": groupColumn },

        ],
        "scrollX": true,
        "scrollY": "350px",
        "scrollCollapse": true,
        "paging": false,
        columns: [
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_chk_sendtopp" type="checkbox">';
                }
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "GROUPING", "className": "cls_nowrap" },
            { data: "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "SHIP_TO_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "SALES_ORDER_ITEM", "className": "cls_nowrap" },
            { data: "RDD", "className": "cls_nowrap cls_sendtopp_rdd" },
            { data: "BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PKG_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PRODUCT_CODE", "className": "cls_nowrap" },
            { data: "PKG_CODE", "className": "cls_nowrap" },
            { data: "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "WORKFLOW_NO", "className": "cls_nowrap" },
            { data: "RECEIVE_DATE", "className": "cls_nowrap cls_sendtopp_receive" }
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var j = 1;
            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                var str_grouping = "";
                for (var x = 0; x < rows.data().length; x++) {
                    if (rows.data()[x].GROUPING == group) {
                        str_grouping = rows.data()[x].GROUPING;
                    }
                }
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group highlight"><td><input data-group-name="group-' + str_grouping + '" class="cls_chk_sendtopp_grouping" type="checkbox"></td>  <td colspan="12"> Group ' + (j) + ' </td></tr>'
                    );

                    last = group;
                    j++;
                }
            });
        },
        select: {
            'style': 'multi',
            selector: 'td:first-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(11).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_SUB_ID + '"> ' + data.WORKFLOW_NO + ' </a>');
            if (data.IS_LOCK == 'X') {
                $(row).find('.cls_chk_sendtopp').remove();
            }
            else {
                $(row).addClass('group-' + data.GROUPING);
            }

            if (!isEmpty(data.RDD)) {
                $(row).find('.cls_sendtopp_rdd').html(myDateMoment(data.RDD));
            }

            if (!isEmpty(data.RECEIVE_DATE)) {
                $(row).find('.cls_sendtopp_receive').html(myDateTimeMoment(data.RECEIVE_DATE));
            }
        },
        //order: [[1, 'asc'], [2, 'asc']],
        order: [3, 'asc'],
        "orderFixed": [2, 'asc'],
        initComplete: function (settings, json) {
            //if (json.data.length > 0) {
            //    setValueToDDL('.cls_container_taskform .cls_lov_primary_type_other', json.data[0].PRIMARY_TYPE_ID, json.data[0].PRIMARY_TYPE_DISPLAY_TXT);
            //    setValueToDDL('.cls_container_taskform .cls_lov_primary_size', json.data[0].PRIMARY_SIZE_ID, json.data[0].PRIMARY_SIZE_DISPLAY_TXT);
            //    setValueToDDL('.cls_container_taskform .cls_lov_pack_size', json.data[0].PACK_SIZE_ID, json.data[0].PACK_SIZE_DISPLAY_TXT);
            //    //setValueToDDL('.cls_container_taskform .cls_lov_packing_style', json.data[0].PACKING_STYLE_ID, json.data[0].PACKING_STYLE_DISPLAY_TXT);
            //    setValueToDDL('.cls_container_taskform .cls_lov_pg_packaging_type_search', json.data[0].PACKING_TYPE_ID, json.data[0].PACKAGING_TYPE_DISPLAY_TXT);
            //}
        }
    });

    $(table_dashboard_sendtopp_select.table().container()).on('keyup', 'input', function () {
        table_dashboard_sendtopp_select
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    $(document).on('click', '.cls_chk_sendtopp_grouping', function (e) {
        var groupName = $(this).data('group-name');
        var tempCheck = this.checked;

        var temp = table_dashboard_sendtopp_select.rows({ search: 'applied' });
        temp.rows('tr.' + groupName).every(function (rowIdx, tableLoop, rowLoop) {
            var rowNode = this.node();
            if ($(rowNode).is(':visible')) {
                table_dashboard_sendtopp_select.row(rowIdx).select(tempCheck);
                $(rowNode).find('.cls_chk_sendtopp').prop('checked', tempCheck);
            }
        });
    });

    $(document).on('click', '#table_dashboard_sendtopp_select .cls_chk_sendtopp', function (e) {
        if ($(this).is(':checked')) {
            table_dashboard_sendtopp_select.rows($(this).closest('tr')).select();
        }
        else {
            table_dashboard_sendtopp_select.rows($(this).closest('tr')).deselect();
        }
        $(this).closest('tr').find('.cls_chk_sendtopp').prop('checked', this.checked);
    });

    table_dashboard_sendtopp_select.on('order.dt search.dt', function () {
        table_dashboard_sendtopp_select.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function bindSendToVendorPopUp() {
    if (first_load_sendtovendor) {
        // Setup - add a text input to each footer cell
        $('#table_dashboard_sendtovendor_select thead tr').clone(true).appendTo('#table_dashboard_sendtovendor_select thead');
        $('#table_dashboard_sendtovendor_select thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 1 || i == 2) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });
    }

    var groupColumn = 2;
    var table_dashboard_sendtovendor_select = $('#table_dashboard_sendtovendor_select').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/taskform/pp/sendtovendorview",
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "orderable": false, "targets": 0 },
            { "orderable": false, "targets": 1 },
            { "visible": false, "targets": groupColumn }
        ],
        "scrollX": true,
        "scrollY": "350px",
        "scrollCollapse": true,
        "paging": false,
        columns: [
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_chk_sendtovendor" type="checkbox">';
                }
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "GROUPING", "className": "cls_nowrap" },
            { data: "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PO", "className": "cls_nowrap" },
            { data: "PKG_CODE", "className": "cls_nowrap" },
            { data: "SALES_ORDER_NO", "className": "cls_nowrap" },
            { data: "RDD", "className": "cls_nowrap cls_sendtovendor_rdd" },
            { data: "BRAND", "className": "cls_nowrap" },
            { data: "PRODUCT_CODE", "className": "cls_nowrap" },
            { data: "WORKFLOW_NO", "className": "cls_nowrap cls_workflowno" }
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var j = 1;
            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                var str_grouping = "";
                for (var x = 0; x < rows.data().length; x++) {
                    if (rows.data()[x].GROUPING == group) {
                        str_grouping = rows.data()[x].GROUPING;
                    }
                }
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group highlight"><td><input data-group-name="group-' + str_grouping + '" class="cls_chk_sendtovendor_grouping" type="checkbox"></td>  <td colspan="10"> Group ' + (j) + ' </td></tr>'
                    );

                    last = group;
                    j++;
                }
            });
        },
        select: {
            'style': 'multi',
            selector: 'td:first-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_workflowno').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_SUB_ID + '"> ' + data.WORKFLOW_NO + ' </a>');
            if (data.IS_LOCK == 'X') {
                $(row).find('.cls_chk_sendtovendor').remove();
            }
            else {
                $(row).addClass('group-' + data.GROUPING);
            }

            if (!isEmpty(data.RDD)) {
                $(row).find('.cls_sendtovendor_rdd').html(myDateMoment(data.RDD));
            }
        },
        order: [3, 'asc'],
        "orderFixed": [2, 'asc'],
        initComplete: function (settings, json) {

        }
    });

    $(table_dashboard_sendtovendor_select.table().container()).on('keyup', 'input', function () {
        table_dashboard_sendtovendor_select
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    $(document).on('click', '.cls_chk_sendtovendor_grouping', function (e) {
        var groupName = $(this).data('group-name');
        var tempCheck = this.checked;

        var temp = table_dashboard_sendtovendor_select.rows({ search: 'applied' });
        temp.rows('tr.' + groupName).every(function (rowIdx, tableLoop, rowLoop) {
            var rowNode = this.node();
            if ($(rowNode).is(':visible')) {
                table_dashboard_sendtovendor_select.row(rowIdx).select(tempCheck);
                $(rowNode).find('.cls_chk_sendtovendor').prop('checked', tempCheck);
            }
        });
    });

    $(document).on('click', '#table_dashboard_sendtovendor_select .cls_chk_sendtovendor', function (e) {
        if ($(this).is(':checked')) {
            table_dashboard_sendtovendor_select.rows($(this).closest('tr')).select();
        }
        else {
            table_dashboard_sendtovendor_select.rows($(this).closest('tr')).deselect();
        }
        $(this).closest('tr').find('.cls_chk_sendtovendor').prop('checked', this.checked);
    });

    table_dashboard_sendtovendor_select.on('order.dt search.dt', function () {
        table_dashboard_sendtovendor_select.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}