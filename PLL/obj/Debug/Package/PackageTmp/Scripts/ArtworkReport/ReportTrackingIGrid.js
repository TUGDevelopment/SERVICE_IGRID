var first_load = true;
$(document).ready(function () {
    bind_lov_param('.cls_report_tracking_igrid .cls_lov_by', '/api/lov/by', 'data.DISPLAY_TXT', ["ROLE"], ['.cls_lov_status']);
    bindTrakcingIGidReport(false, "1");
    //Chart
    // Load Charts and the corechart and barchart packages.

    google.charts.load('current', { 'packages': ['corechart'] });
    $('.cls_home_chart_pie').show();
    $("#table_report_tracking_igrid_wrapper .dt-buttons .buttons-excel").hide();
    first_load = false;
    $('.cls_report_overview .cls_lov_condition').on("change", function () {
        var selections = $(this).select2('data');
        var v = $(this)[0].value;
        var isClear = true;
    });
    $(".cls_report_tracking_igrid .cls_btn_export_excel").click(function () {
        $(".cls_report_tracking_igrid .buttons-excel").click();
        // $(".cls_report_warehouse .buttons-excel").unbind();
    });

    $(".cls_report_tracking_igrid .cls_tracking_btn_clr").click(function () {
        $('.cls_report_tracking_igrid .cls_txt_master_keyword').val('');
        //$('.cls_report_tracking_igrid .cls_track_req_from').val('');

        $('.cls_report_tracking_igrid .cls_track_req_from').val(GetFirstDateOfMonth());
        $('.cls_report_tracking_igrid .cls_track_req_to').val(GetLastDateOfMonth());
        $('.cls_report_tracking_igrid .cls_lov_by').val('');
        $('.cls_report_tracking_igrid .cls_lov_by').text('');
        $('.cls_report_tracking_igrid .cls_lov_role').val('PA');
        $('.cls_report_tracking_igrid .cls_lov_status').val('All');

    });

    
    $('.cls_tracking_btn_re').click(function (e) {
        var table = $('#table_report_tracking_igrid').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            //const array = [];
            for (i = 0; i < tblData.length; i++) {
                //var three_p_id = tblData[i].Id;
                //array.push(tblData[i].Id);
                var jsonObj = new Object();
                jsonObj.data = [];
                var item = {};
                item["ID"] = tblData[i].ID;
                jsonObj.data = item;
                var myurl = '/api/report/trackingigridreport';
                var mytype = 'POST';
                var mydata = jsonObj;

                myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSaveTaskFormPA);
            }
        }
    });
    $(".cls_report_tracking_igrid .cls_tracking_btn_search").click(function (e) {
        //debugger;

        if (checkRequireDataIGridTracking()) 
        {
            var condition = $('.cls_report_tracking_igrid .cls_lov_role').val();
            bindTrakcingIGidReport(false, "0");
        }
     
        //var FrDt = $('.cls_report_tracking_igrid .cls_track_req_from').val();
        //var ToDt = $('.cls_report_tracking_igrid .cls_track_req_to').val();
        //var myurl = '/api/report/counttrackingigrid?data.Role=' + $('.cls_report_tracking_igrid.cls_lov_role').val()
        //    + "&data.FrDt=" + FrDt
        //    + "&data.ToDt=" + ToDt
        //    + "&data.Status=" + $('.cls_report_tracking_igrid .cls_lov_status').val()
        //    + "&data.By=" + $('.cls_report_tracking_igrid .cls_lov_by').val()
        //    + "&data.Keyword=" + $('.cls_report_tracking_igrid .cls_txt_master_keyword').val()
        //    + "&data.first_load=" + first_load;
        //var mytype = 'GET';
        //var mydata = null;
        //myAjax(myurl, mytype, mydata, callback_get_countmockupandartwork);
        //-----------------------------------------------------------------------
        //if ($(".cls_row_chart").is(':hidden')) {
        //    $('.cls_row_chart').show();
        
        //createpie();     

        //google.charts.setOnLoadCallback(drawChart);
        //}
        //else $('.cls_row_chart').hide();
        //if (!isEmpty(condition)) {
        //    table_report_tracking_igrid.ajax.reload();
        //    e.preventDefault();
        //}
        //else {
        //    alertError2("Please input request condition.");
        //    e.preventDefault();
        //}
    });
});

function checkRequireDataIGridTracking() {
    var f_pass = false;
    var rdatefrom = $('.cls_report_tracking_igrid .cls_track_req_from').val();
    var rdateto = $('.cls_report_tracking_igrid .cls_track_req_to').val();
    // var columns;
    if (!isEmpty(rdatefrom) && !isEmpty(rdateto)) {
        f_pass = true;
    }
    else {
        alertError2("Please input request date from and request date to.");

        //e.preventDefault();
    }
    return f_pass;
}




var cntFailed = 0, cntCanceled = 0, cntInProcess = 0, cntCompleted = 0;
function callbackSaveTaskFormPA() {

}
function callback_get_countmockupandartwork(res) {
    if (res.status == 'S') {
        if (res.data.length > 0) {
            for (var i = 0; i < res.data.length; i++) {

                if (res.data[i].Status_upd == 'Fail' || res.data[i].Status_upd== "Re-Uploading" )
                    cntFailed = res.data[i].count_Status_upd;
                else if (res.data[i].Status_upd == 'In Process')
                    cntInProcess = res.data[i].count_Status_upd;
                else if (res.data[i].Status_upd == 'Completed')
                    cntCompleted = res.data[i].count_Status_upd;
                else if (res.data[i].Status_upd == 'Canceled')
                    cntCanceled = res.data[i].count_Status_upd;
            }
        }
    }
    //var dataColumnChart = new google.visualization.DataTable();
    //dataColumnChart.addColumn('string', 'Effort');
    //dataColumnChart.addColumn('number', 'Amount');
    //dataColumnChart.addColumn({ type: 'string', role: 'style', 'p': { 'html': true } });
    //dataColumnChart.addRows([
    //    ['Failed', cntFailed, '#b87333'],
    //    ['Canceled', cntCanceled, cntCanceled_txt],
    //    ['In Process', cntInProcess, cntInProcess_txt],
    //    ['Completed', cntCompleted, cntCompleted_txt],
    //    ['Total Requests', cntTotalRequests, 'color: #e5e4e2'],
    //]);

    //var view = new google.visualization.DataView(dataColumnChart);
    //view.setColumns([0, 1,
    //    {
    //        calc: "stringify",
    //        sourceColumn: 1,
    //        type: "string",
    //        role: "annotation"
    //    },
    //    2]);

    var total = parseInt(cntFailed) + parseInt(cntCanceled) + parseInt(cntInProcess) + parseInt(cntCompleted);
    var columnchart_options = {
        tooltip: { isHtml: true },// This MUST be set to true for your chart to show.
        fontSize: 13,
        title: 'Summarize ' + '(Total ' + total + ')',
        height: 300,
        legend: 'none',
        //vAxis: {
        //    gridlineColor: '#fff',
        //    textPosition: 'none'
        //},
        bar: { groupWidth: "90%" },
        chartArea: { left: 150, top: 50, width: "100%" }
    };
    
    //var barchart = new google.visualization.BarChart(document.getElementById('div_piechart'));
    //barchart.draw(view, columnchart_options);
    var data = google.visualization.arrayToDataTable([
        ["Element", "Density", { role: "style" }],
        ['Failed', cntFailed, '#b87333'],
        ['Canceled', cntCanceled, "red"],
        ['In Process', cntInProcess, "gold"],
        ['Completed', cntCompleted, "silver"],
        ['Total Requests', total, 'color: #e5e4e2'],
    ]);

    var view = new google.visualization.DataView(data);
    view.setColumns([0, 1,
        {
            calc: "stringify",
            sourceColumn: 1,
            type: "string",
            role: "annotation"
        },
        2]);
    var chart = new google.visualization.BarChart(document.getElementById("div_piechart"));
    chart.draw(view, columnchart_options);
}




function drawChart() {
    //debugger;
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
}

function bindTrakcingIGidReport(serverSide, autosearch) {
    var groupColumnPPView = 2;
    var FrDt = $('.cls_report_tracking_igrid .cls_track_req_from').val();
    var ToDt = $('.cls_report_tracking_igrid .cls_track_req_to').val();
    //if (!isEmpty(ToDt)) {
    //    ToDt = FrDt.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");
    //}
    var table_report_tracking_igrid = $('#table_report_tracking_igrid').DataTable()
    table_report_tracking_igrid.destroy();
    table_report_tracking_igrid = $('#table_report_tracking_igrid').DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHaeder: true,
        ajax: function (data, callback, settings) {
            //for (var i = 0, len = data.columns.length; i < len; i++) {
            //    delete data.columns[i].name;
            //    delete data.columns[i].data;
            //    delete data.columns[i].searchable;
            //    delete data.columns[i].orderable;
            //    delete data.columns[i].search.regex;
            //    delete data.search.regex;

            //    delete data.columns[i].search.value;
            //}
            $.ajax({
                url: suburl + "/api/report/trackingigridreport?data.Role=" + $('.cls_report_tracking_igrid .cls_lov_role').val()
                    + "&data.FrDt=" + FrDt
                    + "&data.ToDt=" + ToDt
                    + "&data.Status=" + $('.cls_report_tracking_igrid .cls_lov_status').val()
                    + "&data.By=" + $('.cls_report_tracking_igrid .cls_lov_by').val()
                    + "&data.Keyword=" + $('.cls_report_tracking_igrid .cls_txt_master_keyword').val()
                    + "&data.first_load=" + autosearch
                    + "&data.Name=" + $('.cls_report_tracking_igrid .cls_lov_by option:selected').text()
                ,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);


                    res = DData(res);
                    callbackTest(res);
                    //if (res.status == "E") {
                    //    $('.dataTables_processing').hide();
                    //    if (res.msg != '')
                    //        alertError(res.msg);
                    //}
                    //else if (res.status == "S") {
                    //    callbackTest(res);
                    //}

                }
            });
        },
        "order": [[1, 'desc']],
        "processing": true,
       // "lengthChange": false,
        "paging": false,
        select: {
            style: 'os',
            selector: 'td:first-child'
        },

      
        "ordering": true,
        "info": true,
        "searching": false,
        "scrollX": true,
        "scrollY": "540px",
        "scrollCollapse": true,
        dom: 'Bfrtip',
        buttons: [
            {
                title: 'IGrid Tracking Report',
                extend: 'excelHtml5',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            },//"pageLength"
        ],
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            //{
            //    render: function (data, type, row, meta) {
            //        return meta.row + meta.settings._iDisplayStart + 1;
            //    }
            //},
            { data: "ID", "className": "cls_nowrap" },
            { data: "DocumentNo", "className": "cls_nowrap" },
            { data: "Material", "className": "cls_nowrap" },
            { data: "Description", "className": "cls_nowrap" },
            { data: "Status_upd", "className": "cls_nowrap" },
            { data: "PA_InputBy", "className": "cls_nowrap" },
            { data: "PA_Actdate", "className": "cls_nowrap" },
            { data: "PA_SubmitDate", "className": "cls_nowrap" },
            { data: "PA_InputDate", "className": "cls_nowrap" },
            { data: "PAStart_to_PA", "className": "cls_nowrap" },
            { data: "PA_ApproveBy", "className": "cls_nowrap" },
            { data: "PA_ApproveDate", "className": "cls_nowrap" },
            { data: "PA_to_PAApprove", "className": "cls_nowrap" },
            { data: "PG_AssignTo", "className": "cls_nowrap" },
            { data: "PG_AssignDate", "className": "cls_nowrap" },
            { data: "PA_Submit_to_PGAssign", "className": "cls_nowrap" },
            { data: "PG_InputBy", "className": "cls_nowrap" },
            { data: "PG_InputDate", "className": "cls_nowrap" },
            { data: "PGAssign_to_PGInput", "className": "cls_nowrap" },
            { data: "PG_ApproveBy", "className": "cls_nowrap" },
            { data: "PG_ApproveDate", "className": "cls_nowrap" },
            { data: "PGInput_to_PGApprove", "className": "cls_nowrap" },
            { data: "Final_ApprovedBy", "className": "cls_nowrap" },
            { data: "Final_ApprovedName", "className": "cls_nowrap" },
            { data: "Final_ApprovedDate", "className": "cls_nowrap" },
            { data: "PA_to_FinalApprove", "className": "cls_nowrap" },
            { data: "InfoGroupBy", "className": "cls_nowrap" },
            { data: "InfoGroupDate", "className": "cls_nowrap" },
            { data: "Brand", "className": "cls_nowrap" },
            { data: "MaterialGroup", "className": "cls_nowrap" },
            { data: "Name", "className": "cls_nowrap" },
            { data: "Version", "className": "cls_nowrap" },
            { data: "ChangePoint", "className": "cls_nowrap" },
            { data: "SheetSize", "className": "cls_nowrap" },
            { data: "PackingStyle", "className": "cls_nowrap" },
            { data: "Packing", "className": "cls_nowrap" },
            { data: "StyleofPrinting", "className": "cls_nowrap" },
            { data: "ContainerType", "className": "cls_nowrap" },
            { data: "LidType", "className": "cls_nowrap" },
            { data: "Condition", "className": "cls_nowrap" },
            { data: "ProductCode", "className": "cls_nowrap" },
            { data: "FAOZone", "className": "cls_nowrap" },
            { data: "Plant", "className": "cls_nowrap" },
            { data: "Totalcolour", "className": "cls_nowrap" },
            { data: "Processcolour", "className": "cls_nowrap" },
            { data: "PlantRegisteredNo", "className": "cls_nowrap" },
            { data: "CompanyNameAddress", "className": "cls_nowrap" },
            { data: "PMScolour", "className": "cls_nowrap" },
            { data: "Symbol", "className": "cls_nowrap" },
            { data: "CatchingArea", "className": "cls_nowrap" },
            { data: "CatchingPeriodDate", "className": "cls_nowrap" },
            { data: "Grandof", "className": "cls_nowrap" },
            { data: "Flute", "className": "cls_nowrap" },
            { data: "Vendor", "className": "cls_nowrap" },
            { data: "Dimension", "className": "cls_nowrap" },
            { data: "RSC", "className": "cls_nowrap" },
            { data: "Accessories", "className": "cls_nowrap" },
            { data: "PrintingStyleofPrimary", "className": "cls_nowrap" },
            { data: "PrintingStyleofSecondary", "className": "cls_nowrap" },
            { data: "CustomerDesign", "className": "cls_nowrap" },
            { data: "CustomerSpec", "className": "cls_nowrap" },
            { data: "CustomerSize", "className": "cls_nowrap" },
            { data: "CustomerVendor", "className": "cls_nowrap" },
            { data: "CustomerColor", "className": "cls_nowrap" },
            { data: "CustomerScanable", "className": "cls_nowrap" },
            { data: "CustomerBarcodeSpec", "className": "cls_nowrap" },
            { data: "FirstInfoGroup", "className": "cls_nowrap" },
            { data: "SO", "className": "cls_nowrap" },
            { data: "PICMkt", "className": "cls_nowrap" },
            { data: "SOPlant", "className": "cls_nowrap" },
            { data: "Destination", "className": "cls_nowrap" },
            { data: "Remark", "className": "cls_nowrap" },
            { data: "GrossWeight", "className": "cls_nowrap" },
            { data: "FinalInfoGroup", "className": "cls_nowrap" },
            { data: "Note", "className": "cls_nowrap" },
            { data: "Typeof", "className": "cls_nowrap" },
            { data: "TypeofCarton2", "className": "cls_nowrap" },
            { data: "DMSNo", "className": "cls_nowrap" },
            { data: "TypeofPrimary", "className": "cls_nowrap" },
            { data: "PrintingSystem", "className": "cls_nowrap" },
            { data: "Direction", "className": "cls_nowrap" },
            { data: "RollSheet", "className": "cls_nowrap" },
            { data: "RequestType", "className": "cls_nowrap" },
            { data: "PlantAddress", "className": "cls_nowrap" },

        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null; var j = 1;
            //api.column(groupColumnIGridView, { page: 'current' }).data().each(function (group, i) {
            //if (last !== group) {
            //    var temp = [];
            //    var str_grouping = "";
            cntFailed = 0;
            cntCompleted = 0;
            for (var x = 0; x < rows.data().length; x++) {
                
            //        if (rows.data()[x].GROUPING === group) {
            //            temp.push(rows.data()[x].ARTWORK_SUB_ID);
            //        }
            //    }
            //    if (temp.length > 0)
            //        str_grouping = temp.join("|");
            //    $(rows).eq(i).before('<tr class="group highlight"><td><input data-group-name="' + str_grouping + '" class="cls_chk_group" type="checkbox"/></td><td colspan="16">Group ' + j + '</td></tr>');
            //    last = group;
            //    j++;
            }
            //createpie();
            //});
            //$('.cls_cnt_igrid_view').text('(' + this.api().data().count() + ') ');
        },
        "rowCallback": function (row, data, dataIndex) {
            
        },
        "createdRow": function (row, data, index) {
            if (data.STATUS == "1")
                $(row).css("color", "#A20025");
        },
        //"drawCallback": function (settings) {

        //},
        //"initComplete": function (settings, json) {
        //    $('.cls_cnt_igrid_view').text('(' + json.data.length + ') ');
        //}
    });
    $("#table_report_tracking_igrid_wrapper .dt-buttons .buttons-excel").hide();
}

function callbackTest(res)
{
   // var a = 10;


    var total = res.Completed_Record + res.Failed_Record + res.Canceled_Record + res.InProcess_Record;
    var columnchart_options = {
        tooltip: { isHtml: true },// This MUST be set to true for your chart to show.
        fontSize: 12,
        title: 'Summarize ' + '(Total ' + total + ')',
        height: 200,
        legend: 'none',
        //vAxis: {
        //    gridlineColor: '#fff',
        //    textPosition: 'none'
        //},
        bar: { groupWidth: "90%" },
        chartArea: { left: 150, top: 50, width: "100%" }
    };

    //var barchart = new google.visualization.BarChart(document.getElementById('div_piechart'));
    //barchart.draw(view, columnchart_options);
    var data = google.visualization.arrayToDataTable([
        ["Element", "Density", { role: "style" }],
        ['Failed', res.Failed_Record, 'red '],
        ['Canceled', res.Canceled_Record, "cyan"],
        ['In Process', res.InProcess_Record, "yellow"],
        ['Completed', res.Completed_Record, "lime"],
        ['Total Requests', total, 'gray'],
    ]);

    var view = new google.visualization.DataView(data);
    view.setColumns([0, 1,
        {
            calc: "stringify",
            sourceColumn: 1,
            type: "string",
            role: "annotation"
        },
        2]);
    var chart = new google.visualization.BarChart(document.getElementById("div_piechart"));
    chart.draw(view, columnchart_options);

}