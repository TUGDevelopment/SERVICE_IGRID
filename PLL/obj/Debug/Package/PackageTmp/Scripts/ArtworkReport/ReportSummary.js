var first_load = true;
$(document).ready(function () {
    google.charts.load('current', { 'packages': ['corechart'] });
    $('.cls_report_summary #popup_data_dedtail input:radio[name="requesttype"]').filter('[value="ALL"]').prop('checked', true);
    $('.cls_report_summary .cls_search_so_created_from').val(GetFirstDateOfMonth());
    $('.cls_report_summary .cls_search_so_created_to').val(GetLastDateOfMonth());

    bind_lov('.cls_report_summary .cls_lov_search_current_step', '/api/common/stepmockupandpartwork', 'data.DISPLAY_TXT');

    loadTable_DataByStep();
    loadTable_DataByWorkflowType();
    loadTable_DataDetail();

    $(".dataTables_filter").hide();
    loadChart();

    $(".cls_report_summary .cls_ddl_department").change(function () {
        report_summary_table_data_by_step.ajax.reload();
    });

    $(".cls_report_summary .cls_ddl_workflow_type").change(function () {
        report_summary_table_data_by_workflow_type.ajax.reload();
    });

    $(".cls_report_summary .cls_ddl_workflow_type2").change(function () {
        report_summary_table_data_by_step.ajax.reload();
    });

    $(".cls_report_summary .cls_btn_search").click(function () {
        loadChart();
        report_summary_table_data_by_step.ajax.reload();
        report_summary_table_data_by_workflow_type.ajax.reload();
    });

    $(".cls_report_summary .cls_show_hide_graph_summary").click(function () {
        if ($(".cls_report_summary .cls_row_chart").is(':hidden'))
            $('.cls_report_summary .cls_row_chart').show();
        else $('.cls_report_summary .cls_row_chart').hide();
    });

    $(".cls_report_summary .cls_btn_clr").click(function () {
        $('.cls_report_summary .cls_search_so_created_from').val(GetFirstDateOfMonth());
        $('.cls_report_summary .cls_search_so_created_to').val(GetLastDateOfMonth());
        $(".cls_report_summary .cls_ddl_workflow_status").val('ALL');

        $(".cls_report_summary .cls_ddl_workflow_type").val('ALL');
        $(".cls_report_summary .cls_ddl_department").val('ALL');
    });

    $(document).on("click", ".cls_report_summary td.cls_show_data_detail", function () {
        $('#popup_data_dedtail').modal({
            backdrop: 'static',
            keyboard: true
        });

        $('.cls_report_summary .cls_search_so_created_from_data_detail').val($('.cls_report_summary .cls_search_so_created_from').val());
        $('.cls_report_summary .cls_search_so_created_to_data_detail').val($('.cls_report_summary .cls_search_so_created_to').val());

        if ($(this).hasClass('cls_td_inprocess')) {
            $('.cls_report_summary .cls_ddl_workflow_status_data_detail').val('I');
        }
        else if ($(this).hasClass('cls_td_completed')) {
            $('.cls_report_summary .cls_ddl_workflow_status_data_detail').val('C');
        }
        else {
            $('.cls_report_summary .cls_ddl_workflow_status_data_detail').val('ALL');
        }

        var wf_type = $(this).closest('tr').find('.cls_wf_type').text();
        if (wf_type == 'Artwork new') $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('ARTNEW');
        else if (wf_type == 'Artwork repeat') $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('ARTREPEAT');
        else if (wf_type == 'Artwork repeat R6') $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('ARTR6');
        else if (wf_type == 'Mockup design') $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('MODESIGN');
        else if (wf_type == 'Mockup dieline') $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('MODIELINE');
        else if (wf_type == 'Mockup normal') $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('MONORMAL');
        else $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('ALL');

        //if ($(this).hasClass('cls_td_total_static')) {
        //    $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('NO');
        //}

        var current_step_value = $(this).closest('tr').find('.cls_current_step_value').text();
        var current_step_desc = $(this).closest('tr').find('.cls_current_step_desc').text();
        setValueToDDL('.cls_report_summary .cls_lov_search_current_step', current_step_value, current_step_desc);
        current_step_wf_type = current_step_value;

        if ($(this).hasClass('cls_td_ontime')) $("input[name=requesttype][value='ONTIME']").prop("checked", true);
        else if ($(this).hasClass('cls_td_almost_due')) $("input[name=requesttype][value='ALMOSTDUE']").prop("checked", true);
        else if ($(this).hasClass('cls_td_over_due')) $("input[name=requesttype][value='OVERDUE']").prop("checked", true);
        else $("input[name=requesttype][value='ALL']").prop("checked", true);

        //if ($(this).hasClass('cls_td_inprocess') && $(this).hasClass('cls_td_ontime')) {
        //    LIST_WF_SUB_ID = $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_ONTIME').text();
        //}
        //if ($(this).hasClass('cls_td_inprocess') && $(this).hasClass('cls_td_almost_due')) {
        //    LIST_WF_SUB_ID = $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE').text();
        //}
        //if ($(this).hasClass('cls_td_inprocess') && $(this).hasClass('cls_td_over_due')) {
        //    LIST_WF_SUB_ID = $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_OVERDUE').text();
        //}
        //if ($(this).hasClass('cls_td_completed') && $(this).hasClass('cls_td_ontime')) {
        //    LIST_WF_SUB_ID = $(this).closest('tr').find('.LIST_WF_SUB_ID_COMPLETED_ONTIME').text();
        //}
        //if ($(this).hasClass('cls_td_completed') && $(this).hasClass('cls_td_over_due')) {
        //    LIST_WF_SUB_ID = $(this).closest('tr').find('.LIST_WF_SUB_ID_COMPLETED_OVERDUE').text();
        //}

        //var obj = this;
        //if ($(this).hasClass('cls_show_data_detail_total')) {
        //    LIST_WF_SUB_ID = '';
        //    $('#table2_report_summary > tbody  > tr').each(function () {
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_ONTIME').text();
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE').text();
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_OVERDUE').text();
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_COMPLETED_ONTIME').text();
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_COMPLETED_OVERDUE').text();
        //    });
        //}
        //if ($(this).hasClass('cls_show_data_detail_total_inprocess')) {
        //    LIST_WF_SUB_ID = '';
        //    $('#table2_report_summary > tbody  > tr').each(function () {
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_ONTIME').text();
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE').text();
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_OVERDUE').text();
        //    });  
        //}
        //if ($(this).hasClass('cls_show_data_detail_total_completed')) {
        //    LIST_WF_SUB_ID = '';
        //    $('#table2_report_summary > tbody  > tr').each(function () {
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_COMPLETED_ONTIME').text();
        //        LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_COMPLETED_OVERDUE').text();
        //    }); 
        //}
        //if ($(this).hasClass('cls_td_total')) {
        //    LIST_WF_SUB_ID = '';
        //    $('#table2_report_summary > tbody  > tr').each(function () {
        //        if ($(obj).hasClass('cls_td_inprocess') && $(obj).hasClass('cls_td_ontime')) {
        //            LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_ONTIME').text();
        //        }
        //        if ($(obj).hasClass('cls_td_inprocess') && $(obj).hasClass('cls_td_almost_due')) {
        //            LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE').text();
        //        }
        //        if ($(obj).hasClass('cls_td_inprocess') && $(obj).hasClass('cls_td_over_due')) {
        //            LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_INPROCESS_OVERDUE').text();
        //        }
        //        if ($(obj).hasClass('cls_td_completed') && $(obj).hasClass('cls_td_ontime')) {
        //            LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_COMPLETED_ONTIME').text();
        //        }
        //        if ($(obj).hasClass('cls_td_completed') && $(obj).hasClass('cls_td_over_due')) {
        //            LIST_WF_SUB_ID += $(this).closest('tr').find('.LIST_WF_SUB_ID_COMPLETED_OVERDUE').text();
        //        }
        //    });
        //}

        //if (isEmpty(LIST_WF_SUB_ID)) LIST_WF_SUB_ID = 'NO';
        //else LIST_WF_SUB_ID = "YES";
        LIST_WF_SUB_ID = "YES";
        report_summary_table_data_detail.ajax.reload();
    });

    $(document).on("click", ".cls_report_summary td.cls_show_data_detail_by_step", function () {
        $('#popup_data_dedtail').modal({
            backdrop: 'static',
            keyboard: true
        });

        $('.cls_report_summary .cls_search_so_created_from_data_detail').val($('.cls_report_summary .cls_search_so_created_from').val());
        $('.cls_report_summary .cls_search_so_created_to_data_detail').val($('.cls_report_summary .cls_search_so_created_to').val());

        if ($(this).hasClass('cls_td_inprocess')) {
            $('.cls_report_summary .cls_ddl_workflow_status_data_detail').val('I');
        }
        else if ($(this).hasClass('cls_td_completed')) {
            $('.cls_report_summary .cls_ddl_workflow_status_data_detail').val('C');
        }
        else {
            $('.cls_report_summary .cls_ddl_workflow_status_data_detail').val('ALL');
        }

        $('.cls_report_summary .cls_ddl_workflow_type_data_detail').val('ALL');

        var current_step_value = $(this).closest('tr').find('.cls_current_step_value').text();
        var current_step_desc = $(this).closest('tr').find('.cls_current_step_desc').text();
        setValueToDDL('.cls_report_summary .cls_lov_search_current_step', current_step_value, current_step_desc);
        current_step_wf_type = current_step_value;

        if ($(this).hasClass('cls_td_ontime')) $("input[name=requesttype][value='ONTIME']").prop("checked", true);
        else if ($(this).hasClass('cls_td_almost_due')) $("input[name=requesttype][value='ALMOSTDUE']").prop("checked", true);
        else if ($(this).hasClass('cls_td_over_due')) $("input[name=requesttype][value='OVERDUE']").prop("checked", true);
        else $("input[name=requesttype][value='ALL']").prop("checked", true);

        if ($(this).hasClass('cls_show_data_detail_by_step_total_inprocess')) {
            $("input[name=requesttype][value='ALL']").prop("checked", true);
        }
        if ($(this).hasClass('cls_show_data_detail_by_step_total_complete')) {
            $("input[name=requesttype][value='ALL']").prop("checked", true);
        }

        LIST_WF_SUB_ID = '';
        report_summary_table_data_detail.ajax.reload();
    });

    $(".cls_report_summary .cls_btn_search_data_detail").click(function () {
        report_summary_table_data_detail.ajax.reload();
    });

    $(".cls_report_summary .cls_btn_clr_data_detail").click(function () {
        $(".cls_report_summary #popup_data_dedtail .cls_ddl_workflow_type_data_detail").val('ARTNEW');
        $(".cls_report_summary #popup_data_dedtail .cls_ddl_workflow_status_data_detail").val('ALL');
        $('#popup_data_dedtail input:radio[name="requesttype"]').filter('[value="ALL"]').prop('checked', true);
    });

    $(".cls_report_summary .cls_lov_search_current_step").on("change", function () {
        if ($('.cls_report_summary .cls_lov_search_current_step').select2('data').length > 0)
            current_step_wf_type = $('.cls_report_summary .cls_lov_search_current_step').select2('data')[0].WF_TYPE;
        else current_step_wf_type = '';
    });

    $(".cls_report_summary .cls_btn_export_excel").click(function () {
        window.open("/excel/summaryreport?data.workflow_type=" + $(".cls_report_summary .cls_ddl_workflow_type_data_detail option:selected").val()
            + "&data.date_from=" + $(".cls_report_summary .cls_search_so_created_from_data_detail").val()
            + "&data.date_to=" + $(".cls_report_summary .cls_search_so_created_to_data_detail").val()
            + "&data.current_step=" + current_step_wf_type
            + "&data.LIST_WF_SUB_ID=" + LIST_WF_SUB_ID
            + "&data.workflow_status=" + $('.cls_report_summary .cls_ddl_workflow_status_data_detail').val()
            + "&data.request_type=" + $(".cls_report_summary input[name='requesttype']:checked").val()
            + "&data.department=" + $(".cls_report_summary .cls_ddl_department option:selected").val()
            , '_blank');
    });

    first_load = false;

    $(".cls_report_summary .cls_btn_view_logic_report_summary").click(function () {
        $('#view_logic_program_summary_report').modal({
            backdrop: 'static',
            keyboard: true
        });
    });
});

function loadChart() {
    var myurl = "/api/report/summaryreportchartbystep"
        + "?data.date_from=" + $(".cls_report_summary .cls_search_so_created_from").val()
        + "&data.date_to=" + $(".cls_report_summary .cls_search_so_created_to").val()
        + "&data.first_load=" + first_load
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, drawChartReportSummaryLeft);

    var myurl = "/api/report/summaryreportchartbyworkflow"
        + "?data.date_from=" + $(".cls_report_summary .cls_search_so_created_from").val()
        + "&data.date_to=" + $(".cls_report_summary .cls_search_so_created_to").val()
        + "&data.first_load=" + first_load
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, drawChartReportSummaryRight);
}

function drawChartReportSummaryRight(res) {
    var data = google.visualization.arrayToDataTable([
        ['Genre', 'On time', 'Almost due', 'Over due', { role: 'annotation' }],
        ['Artwork new', res.WF_NEW_ONTIME, res.WF_NEW_ALMOST_DUE, res.WF_NEW_OVER_DUE, ''],
        ['Artwork repeat', res.WF_REPEAT_ONTIME, res.WF_REPEAT_ALMOST_DUE, res.WF_REPEAT_OVER_DUE, ''],
        ['Artwork repeat R6', res.WF_REPEATR6_ONTIME, res.WF_REPEATR6_ALMOST_DUE, res.WF_REPEATR6_OVER_DUE, ''],
        ['Mockup normal', res.WF_MONORMAL_ONTIME, res.WF_MONORMAL_ALMOST_DUE, res.WF_MONORMAL_OVER_DUE, ''],
        ['Mockup design', res.WF_MODESIGN_ONTIME, res.WF_MODESIGN_ALMOST_DUE, res.WF_MODESIGN_OVER_DUE, ''],
        ['Mockup dieline', res.WF_MODIELINE_ONTIME, res.WF_MODIELINE_ALMOST_DUE, res.WF_MODIELINE_OVER_DUE, ''],
    ]);

    var options = {
        legend: { position: 'top', maxLines: 3 },
        bar: { groupWidth: '75%' },
        isStacked: true,
        colors: ['#109618', '#ff9900', '#dc3912'],
        tooltip: { isHtml: true },    // CSS styling affects only HTML tooltips.
    };

    var view = new google.visualization.DataView(data);
    var barchart = new google.visualization.ColumnChart(document.getElementById('div_columnchart_right'));
    barchart.draw(view, options);
}

function drawChartReportSummaryLeft(res) {
    var data = google.visualization.arrayToDataTable([
        ['Genre', 'On time', 'Almost due', 'Over due', { role: 'annotation' }],
        ['PK', res.CNT_PK_ONTIME, res.CNT_PK_ALMOSTDUE, res.CNT_PK_OVERDUE, ''],
        ['MK', res.CNT_MK_ONTIME, res.CNT_MK_ALMOSTDUE, res.CNT_MK_OVERDUE, ''],
        ['QC', res.CNT_QC_ONTIME, res.CNT_QC_ALMOSTDUE, res.CNT_QC_OVERDUE, ''],
        ['RD', res.CNT_RD_ONTIME, res.CNT_RD_ALMOSTDUE, res.CNT_RD_OVERDUE, ''],
        ['WH', res.CNT_WH_ONTIME, res.CNT_WH_ALMOSTDUE, res.CNT_WH_OVERDUE, ''],
        ['PN', res.CNT_PN_ONTIME, res.CNT_PN_ALMOSTDUE, res.CNT_PN_OVERDUE, ''],
        ['CUS', res.CNT_CUS_ONTIME, res.CNT_CUS_ALMOSTDUE, res.CNT_CUS_OVERDUE, ''],
        ['VN', res.CNT_VN_ONTIME, res.CNT_VN_ALMOSTDUE, res.CNT_VN_OVERDUE, '']
    ]);

    var options = {
        legend: { position: 'top', maxLines: 3 },
        bar: { groupWidth: '75%' },
        isStacked: true,
        colors: ['#109618', '#ff9900', '#dc3912'],
        tooltip: { isHtml: true },    // CSS styling affects only HTML tooltips.
    };

    var view = new google.visualization.DataView(data);
    var barchart = new google.visualization.ColumnChart(document.getElementById('div_columnchart_left'));
    barchart.draw(view, options);
}

var report_summary_table_data_by_step;
function loadTable_DataByStep() {

    report_summary_table_data_by_step = $('#table1_report_summary').DataTable({
        orderCellsTop: true,
        stateSave: false,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/summaryreportdatabystep?data.department=" + $(".cls_report_summary .cls_ddl_department option:selected").val()
                    + "&data.date_from=" + $(".cls_report_summary .cls_search_so_created_from").val()
                    + "&data.date_to=" + $(".cls_report_summary .cls_search_so_created_to").val()
                    + "&data.workflow_type=" + $(".cls_report_summary .cls_ddl_workflow_type2 option:selected").val()
                    + "&data.first_load=" + first_load
                ,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "orderable": false, "targets": 0 },
        ],
        "order": [[2, 'asc']],
        "orderFixed": [12, 'asc'],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": true,
        "scrollX": true,
        "autoWidth": false,
        "paging": false,
        columns: [
            {
                className: "cls_td_width_25 cls_row_number",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },

            { data: "STEP_DISPLAY_TEXT", "className": "cls_nowrap cls_current_step_desc" },
            { data: "WORKFLOW_TYPE", "className": "cls_nowrap cls_td_width_100 cls_wf_type" },
            { data: "WF_TOTAL", "className": "cls_nowrap cls_td_width_100 cls_wf_total cls_right cls_show_data_detail_by_step" },
            { data: "INPROCESS_ONTIME", "className": "cls_nowrap cls_right cls_td_boder_left cls_td_width_70 cls_font_green cls_show_data_detail_by_step cls_td_inprocess cls_td_ontime" },
            { data: "INPROCESS_ALMOST_DUE", "className": "cls_nowrap cls_right cls_td_width_70 cls_font_yellow cls_show_data_detail_by_step cls_td_inprocess cls_td_almost_due" },
            { data: "INPROCESS_OVER_DUE", "className": "cls_nowrap cls_right cls_td_width_70 cls_font_red cls_show_data_detail_by_step cls_td_inprocess cls_td_over_due" },
            { data: "WF_TOTAL_INPROCESS_BY_WORKFLOW_STEP", "className": "cls_td_inprocess cls_show_data_detail_by_step cls_show_data_detail_by_step_total_inprocess cls_font_blue cls_right" },
            { data: "COMPLETED_ONTIME", "className": "cls_nowrap cls_right cls_td_boder_left cls_td_width_70 cls_font_green cls_show_data_detail_by_step cls_td_completed cls_td_ontime" },
            { data: "COMPLETED_OVER_DUE", "className": "cls_nowrap cls_right cls_td_width_70 cls_font_red cls_show_data_detail_by_step cls_td_completed cls_td_over_due" },
            { data: "WF_TOTAL_COMPLETE_BY_WORKFLOW_STEP", "className": "cls_td_completed cls_show_data_detail_by_step cls_show_data_detail_by_step_total_complete cls_font_blue cls_right" },

            { data: "CURRENT_STEP_TXT", "className": "cls_hide cls_current_step_value" },
            { data: "ORDERBY", "className": "cls_hide" },
        ],
        "drawCallback": function (settings) {


        },
        "rowCallback": function (row, data, index) {

        },
    });

    report_summary_table_data_by_step.on('order.dt search.dt', function () {
        report_summary_table_data_by_step.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
        $('#table1_report_summary tbody tr:last').prev().find('.cls_row_number').html('');
        $('#table1_report_summary tbody tr:last').find('.cls_row_number').html('');

        $('#table1_report_summary tbody tr:last').prev().find('.cls_wf_type').css('text-align', 'right');
        $('#table1_report_summary tbody tr:last').find('.cls_wf_type').css('text-align', 'right');

        $('#table1_report_summary tbody tr:last').prev().css('font-weight', 'bold').css('background-color', '#f9f9f9');
        $('#table1_report_summary tbody tr:last').css('font-weight', 'bold').css('background-color', '#f9f9f9');

        $('#table1_report_summary tbody tr:last').find('.cls_show_data_detail_by_step').removeClass('cls_show_data_detail_by_step');
        $('#table1_report_summary tbody tr:last').prev().find('td').addClass('cls_td_total');

        $('#table1_report_summary tbody tr:last').find('.cls_show_data_detail_by_step_total_inprocess').html('');
        $('#table1_report_summary tbody tr:last').find('.cls_show_data_detail_by_step_total_complete').html('');

        $('#table1_report_summary tbody tr:last').find('.cls_wf_total').html('');
    });
}

var report_summary_table_data_by_workflow_type;
function loadTable_DataByWorkflowType() {

    report_summary_table_data_by_workflow_type = $('#table2_report_summary').DataTable({
        orderCellsTop: true,
        stateSave: false,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/summaryreportdatabyworkflowtype?data.workflow_type=" + $(".cls_report_summary .cls_ddl_workflow_type option:selected").val()
                    + "&data.date_from=" + $(".cls_report_summary .cls_search_so_created_from").val()
                    + "&data.date_to=" + $(".cls_report_summary .cls_search_so_created_to").val()
                    + "&data.first_load=" + first_load
                ,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "orderable": false, "targets": 0 },
        ],
        "order": [[1, 'asc']],
        "orderFixed": [11, 'asc'],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": true,
        "scrollX": true,
        "autoWidth": false,
        "paging": false,
        columns: [
            {
                className: "cls_td_width_25 cls_row_number",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "WORKFLOW_TYPE", "className": "cls_nowrap cls_wf_type" },
            { data: "CNT_WF", "className": "cls_nowrap cls_show_data_detail cls_show_data_detail_total cls_right" },
            { data: "INPROCESS_ONTIME", "className": "cls_nowrap cls_show_data_detail cls_td_inprocess cls_right cls_td_boder_left cls_td_width_100 cls_font_green cls_td_ontime" },
            { data: "INPROCESS_ALMOST_DUE", "className": "cls_nowrap cls_show_data_detail cls_td_inprocess cls_right cls_td_width_100 cls_font_yellow cls_td_almost_due" },
            { data: "INPROCESS_OVER_DUE", "className": "cls_nowrap cls_show_data_detail cls_td_inprocess cls_right cls_td_width_100 cls_font_red cls_td_over_due" },
            { data: "WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE", "className": "cls_nowrap cls_right cls_td_inprocess cls_show_data_detail cls_show_data_detail_total_inprocess cls_td_width_100 cls_font_blue" },
            { data: "COMPLETED_ONTIME", "className": "cls_nowrap cls_show_data_detail cls_td_completed cls_right cls_td_boder_left cls_td_width_100 cls_td_ontime cls_font_green" },
            { data: "COMPLETED_OVER_DUE", "className": "cls_nowrap cls_show_data_detail cls_td_completed cls_right cls_td_width_100 cls_font_red cls_td_over_due" },
            { data: "COMPLETED_TOTAL", "className": "cls_nowrap cls_right cls_td_completed cls_show_data_detail_total_completed cls_show_data_detail cls_font_bold cls_td_width_100 cls_font_blue cls_completed_toltal" },
            { data: "COMPLETED_PERCEN", "className": "cls_nowrap cls_right cls_td_completed cls_font_bold cls_td_width_100 cls_font_blue cls_completed_percen" },


            { data: "ORDERBY", "className": "cls_hide" },
            { data: "CURRENT_STEP_VALUE", "className": "cls_hide cls_current_step_value" },
            { data: "CURRENT_STEP_DISPLAY_TEXT", "className": "cls_hide cls_current_step_desc" },
            { data: "LIST_WF_SUB_ID_INPROCESS_ONTIME", "className": "cls_hide LIST_WF_SUB_ID_INPROCESS_ONTIME" },
            { data: "LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE", "className": "cls_hide LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE" },
            { data: "LIST_WF_SUB_ID_INPROCESS_OVERDUE", "className": "cls_hide LIST_WF_SUB_ID_INPROCESS_OVERDUE" },
            { data: "LIST_WF_SUB_ID_COMPLETED_ONTIME", "className": "cls_hide LIST_WF_SUB_ID_COMPLETED_ONTIME" },
            { data: "LIST_WF_SUB_ID_COMPLETED_OVERDUE", "className": "cls_hide LIST_WF_SUB_ID_COMPLETED_OVERDUE" },
        ],
        "drawCallback": function (settings) {

        },
        "rowCallback": function (row, data, index) {

        },
    });

    report_summary_table_data_by_workflow_type.on('order.dt search.dt', function () {
        report_summary_table_data_by_workflow_type.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
        $('#table2_report_summary tbody tr:last').prev().find('.cls_row_number').html('');
        $('#table2_report_summary tbody tr:last').find('.cls_row_number').html('');

        $('#table2_report_summary tbody tr:last').prev().find('.cls_wf_type').css('font-weight', 'bold').css('text-align', 'right');
        $('#table2_report_summary tbody tr:last').find('.cls_wf_type').css('font-weight', 'bold').css('text-align', 'right');

        $('#table2_report_summary tbody tr:last').prev().css('font-weight', 'bold').css('background-color', '#f9f9f9');
        $('#table2_report_summary tbody tr:last').css('font-weight', 'bold').css('background-color', '#f9f9f9');

        $('#table2_report_summary tbody tr:last').find('.cls_show_data_detail').removeClass('cls_show_data_detail');
        $('#table2_report_summary tbody tr:last').prev().find('td').addClass('cls_td_total');
        $('#table2_report_summary tbody tr:last').prev().find('td').addClass('cls_td_total_static');
        $('#table2_report_summary tbody tr:last').prev().find('.cls_show_data_detail_total').removeClass('cls_td_total');
        $('#table2_report_summary tbody tr:last').prev().find('.cls_show_data_detail_total').removeClass('cls_td_total');
        $('#table2_report_summary tbody tr:last').prev().find('.cls_show_data_detail_total_inprocess').removeClass('cls_td_total');
        $('#table2_report_summary tbody tr:last').prev().find('.cls_show_data_detail_total_completed').removeClass('cls_td_total');



        $('#table2_report_summary tbody tr:last').find('.cls_show_data_detail_total').html('');
        $('#table2_report_summary tbody tr:last').find('.cls_show_data_detail_total_inprocess').html('');

        $('#table2_report_summary tbody tr:last').find('.cls_completed_toltal').text('');
        $('#table2_report_summary tbody tr:last').find('.cls_completed_percen').text('');
    });
}

var LIST_WF_SUB_ID = '';
var current_step_wf_type = '';
var report_summary_table_data_detail
function loadTable_DataDetail() {
    var groupColumn = 1;
    report_summary_table_data_detail = $('#table_report_summary_data_detail').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedColumns: {
            leftColumns: 2
        },
        ajax: function (data, callback, settings) {

            for (var i = 0, len = data.columns.length; i < len; i++) {
                delete data.columns[i].name;
                delete data.columns[i].data;
                delete data.columns[i].searchable;
                delete data.columns[i].orderable;
                delete data.columns[i].search.regex;
                delete data.search.regex;

                delete data.columns[i].search.value;
            }

            $.ajax({
                url: suburl + "/api/report/summaryreportdatadetail?data.workflow_type=" + $(".cls_report_summary .cls_ddl_workflow_type_data_detail option:selected").val()
                    + "&data.date_from=" + $(".cls_report_summary .cls_search_so_created_from_data_detail").val()
                    + "&data.date_to=" + $(".cls_report_summary .cls_search_so_created_to_data_detail").val()
                    + "&data.current_step=" + current_step_wf_type
                    + "&data.workflow_status=" + $('.cls_report_summary .cls_ddl_workflow_status_data_detail').val()
                    + "&data.request_type=" + $(".cls_report_summary input[name='requesttype']:checked").val()
                    + "&data.LIST_WF_SUB_ID=" + LIST_WF_SUB_ID
                    + "&data.first_load=" + first_load
                    + "&data.department=" + $(".cls_report_summary .cls_ddl_department option:selected").val()
                ,
                type: 'GET',
                data: data,
                success: function (res) {
                    groupColumn = DData(res).ORDER_COLUMN;
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "orderable": false, "targets": 0 },
        ],
        "order": [[1, 'asc']],
        "processing": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "scrollX": true,
        "autoWidth": false,
        columns: [
            {
                className: "cls_td_width_25 cls_row_number",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "WF_NO", "className": "cls_nowrap cls_wf_no" },
            { data: "CURRENT_STEP", "className": "cls_nowrap" },
            { data: "WORKFLOW_STATUS", "className": "cls_nowrap" },
            { data: "CURRENT_ASSIGN", "className": "cls_nowrap" },
            { data: "CUS_OR_VEN_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "CREATE_DATE", "className": "cls_nowrap cls_create_date" },
            { data: "DURATION_STANDARD", "className": "cls_nowrap" },
            { data: "EXTEND_DURATION", "className": "cls_nowrap" },
            { data: "DUE_DATE", "className": "cls_nowrap cls_due_date" },
            { data: "END_DATE", "className": "cls_nowrap cls_end_date" },
            { data: "TOTAL_DAY", "className": "cls_nowrap" },
            { data: "SALES_ORDER_NO", "className": "cls_nowrap" },
            { data: "BRAND", "className": "cls_nowrap" },
            { data: "SOLD_TO", "className": "cls_nowrap" },
            { data: "SHIP_TO", "className": "cls_nowrap" },
            { data: "PACKAGING_TYPE", "className": "cls_nowrap" },
            { data: "PRIMARY_TYPE", "className": "cls_nowrap" },
            { data: "PRODUCT_CODE", "className": "cls_nowrap" },
            { data: "RDD", "className": "cls_nowrap" },
            { data: "PA_NAME", "className": "cls_nowrap" },
            { data: "PG_NAME", "className": "cls_nowrap" },
            { data: "MARKETTING", "className": "cls_nowrap" },
        ],
        "drawCallback": function (settings) {
            //var api = this.api();
            //var rows = api.rows({ page: 'current' }).nodes();
            //var last = null;
            //var j = 1;

            //api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
            //    if (last !== group) {
            //        $(rows).eq(i).before(
            //            '<tr class="group highlight"><td colspan="23"></td></tr>'
            //        );
            //        last = group;
            //        j++;
            //    }
            //});
        },
        "rowCallback": function (row, data, index) {

            if (data.WF_TYPE == 'Artwork')
                $(row).find('.cls_wf_no').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.WF_NO + '</a>');
            else
                $(row).find('.cls_wf_no').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.WF_NO + '</a>');

            if (!isEmpty(data.CREATE_DATE)) {
                $(row).find('.cls_create_date').html(myDateTimeMoment(data.CREATE_DATE));
            }
            if (!isEmpty(data.END_DATE)) {
                $(row).find('.cls_end_date').html(myDateTimeMoment(data.END_DATE));
            }
            if (!isEmpty(data.DUE_DATE)) {
                $(row).find('.cls_due_date').html(myDateTimeMoment(data.DUE_DATE));
            }
        },
    });
}