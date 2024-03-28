var first_load = true;
$(document).ready(function () {

    bind_lov_no_ajax('.cls_report_tracking .cls_trk_req_type', '', '');
    bind_lov_no_ajax_not_allow_clear('.cls_report_tracking .cls_report_track_view', '', '');
    bind_lov('.cls_report_tracking .cls_lov_sold_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_company', '/api/lov/company', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_ship_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_brand', '/api/lov/brand', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_country', '/api/lov/country', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_packaging_type', '/api/lov/packtype', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_cur_step', '/api/common/stepmockupandpartwork', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_creator', '/api/lov/user', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_supervised', '/api/lov/user', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_cur_assign', '/api/lov/user', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_cur_working_group', '/api/lov/user', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_tracking .cls_lov_zone', '/api/lov/zone', 'data.DISPLAY_TXT');

    $('.cls_report_tracking .cls_reprot_tracking_advance_search').hide();
    $(".cls_report_tracking .cls_chk_reprot_tracking_advance_search").click(function () {
        if ($('.cls_reprot_tracking_advance_search').is(':visible'))
            $('.cls_reprot_tracking_advance_search').hide();
        else
            $('.cls_reprot_tracking_advance_search').show();
    });

    bind_report_track_pg();
    first_load = false;

    $(".cls_report_track_view").change(function () {
        if ($(this).val() == "pg") {
            table_reprot_tracking_pg.column(3).visible(false);
            table_reprot_tracking_pg.column(11).visible(false);
            table_reprot_tracking_pg.column(21).visible(true);
            table_reprot_tracking_pg.column(22).visible(true);
        }
        if ($(this).val() == "mk") {
            table_reprot_tracking_pg.column(3).visible(true);
            table_reprot_tracking_pg.column(11).visible(true);
            table_reprot_tracking_pg.column(21).visible(false);
            table_reprot_tracking_pg.column(22).visible(false);
        }
     
        table_reprot_tracking_pg.ajax.reload();
    });

    $(".cls_report_tracking .cls_trk_btn_clr").click(function () {
        $('.cls_div_body_search_criteria input[type=text]').val('');
        $('.cls_div_body_search_criteria textarea').val('');
        $('.cls_div_body_search_criteria input[type=checkbox]').not('.cls_chk_reprot_tracking_advance_search').prop('checked', false);
        $('.cls_div_body_search_criteria select').not('.cls_report_track_view').val('').change();

        $('.cls_report_tracking .cls_trk_req_from').val(GetFirstDateOfMonth());
        $('.cls_report_tracking .cls_trk_req_to').val(GetLastDateOfMonth());
    });

    $(".cls_report_tracking form").submit(function (e) {
        try {
            if ($('.cls_report_tracking .cls_lov_cur_step').select2('data').length > 0)
                current_step_wf_type = $('.cls_report_tracking .cls_lov_cur_step').select2('data')[0].WF_TYPE;
            else
                current_step_wf_type = '';

            table_reprot_tracking_pg.ajax.reload();

            e.preventDefault();
        }
        catch (err) { alert(err.message); }
    });

    $(".cls_report_tracking .cls_btn_export_excel").click(function () {

        window.open("/excel/trackingnewreport?data.workflow_no=" + $('.cls_report_tracking .cls_trk_wf_no').val()
            + "&data.GENERATE_EXCEL=X"
            + "&data.workflow_type=" + $('.cls_report_tracking .cls_trk_req_type').val()
            + "&data.workflow_no_2=" + $('.cls_report_tracking .cls_trk_wf_no_2').val()
            + "&data.request_date_from=" + $('.cls_report_tracking .cls_trk_req_from').val()
            + "&data.request_date_to=" + $('.cls_report_tracking .cls_trk_req_to').val()
            + "&data.workflow_completed=" + $('.cls_report_tracking .cls_chk_wf_completed').is(":checked")
            + "&data.workflow_in_process=" + $('.cls_report_tracking .cls_chk_wf_in_process').is(":checked")

            + "&data.sold_to_id=" + $('.cls_report_tracking .cls_lov_sold_to').val()
            + "&data.ship_to_id=" + $('.cls_report_tracking .cls_lov_ship_to').val()
            + "&data.country_id=" + $('.cls_report_tracking .cls_lov_country').val()
            + "&data.brand_id=" + $('.cls_report_tracking .cls_lov_brand').val()
            + "&data.company_id=" + $('.cls_report_tracking .cls_lov_company').val()
            + "&data.project_name=" + $('.cls_report_tracking .cls_input_project_name').val()

            + "&data.product_code=" + $('.cls_report_tracking .cls_input_product_code').val().replace(/\n/g, ',')
            + "&data.rd_number=" + $('.cls_report_tracking .cls_input_rd_number').val()
            + "&data.packaging_type_id=" + $('.cls_report_tracking .cls_lov_packaging_type').val()
            + "&data.primary_size_txt=" + $('.cls_report_tracking .cls_lov_primary_size').val()
            + "&data.net_weight_txt=" + $('.cls_report_tracking .cls_input_net_weight').val()

            + "&data.current_step_id=" + $('.cls_report_tracking .cls_lov_cur_step').val()
            + "&data.current_step_wf_type=" + current_step_wf_type

            + "&data.creator_id=" + $('.cls_report_tracking .cls_lov_creator').val()
            + "&data.supervised_id=" + $('.cls_report_tracking .cls_lov_supervised').val()
            + "&data.current_assign_id=" + $('.cls_report_tracking .cls_lov_cur_assign').val()
            + "&data.working_group_id=" + $('.cls_report_tracking .cls_lov_cur_working_group').val()
            + "&data.workflow_overdue=" + $('.cls_report_tracking .cls_chk_wf_overdue').is(":checked")
            + "&data.workflow_action_by_me=" + $('.cls_report_tracking .cls_chk_wf_action_by_me').is(":checked")
            + "&data.ref_wf_no=" + $('.cls_report_tracking .cls_trk_ref_wf_no').val()

            + "&data.search_so=" + $('.cls_report_tracking .cls_search_so').val().replace(/\n/g, ',')
            + "&data.search_order_bom=" + $('.cls_report_tracking .cls_search_order_bom').val().replace(/\n/g, ',')
            + "&data.zone_txt=" + $('.cls_report_tracking .cls_lov_zone option:selected').text()

            + "&data.current_user_id=" + UserID
            + "&data.PIC=" + ''

            + "&data.view=" + $('.cls_report_tracking .cls_report_track_view').val()

            , '_blank');
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box

    table_reprot_tracking_pg.column(3).visible(false);
    table_reprot_tracking_pg.column(11).visible(false);
});

var table_reprot_tracking_pg;
function bind_report_track_pg() {
    var groupColumn = 1;
    table_reprot_tracking_pg = $('#table_report_pg').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        //fixedColumns: {
        //    leftColumns: 2
        //},
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
                url: suburl + "/api/report/trackingreport_new?data.workflow_no=" + $('.cls_report_tracking .cls_trk_wf_no').val()
                    + "&data.workflow_type=" + $('.cls_report_tracking .cls_trk_req_type').val()
                    + "&data.workflow_no_2=" + $('.cls_report_tracking .cls_trk_wf_no_2').val()
                    + "&data.request_date_from=" + $('.cls_report_tracking .cls_trk_req_from').val()
                    + "&data.request_date_to=" + $('.cls_report_tracking .cls_trk_req_to').val()
                    + "&data.workflow_completed=" + $('.cls_report_tracking .cls_chk_wf_completed').is(":checked")
                    + "&data.workflow_in_process=" + $('.cls_report_tracking .cls_chk_wf_in_process').is(":checked")
                    + "&data.sold_to_id=" + $('.cls_report_tracking .cls_lov_sold_to').val()
                    + "&data.ship_to_id=" + $('.cls_report_tracking .cls_lov_ship_to').val()
                    + "&data.country_id=" + $('.cls_report_tracking .cls_lov_country').val()
                    + "&data.brand_id=" + $('.cls_report_tracking .cls_lov_brand').val()
                    + "&data.company_id=" + $('.cls_report_tracking .cls_lov_company').val()
                    + "&data.project_name=" + $('.cls_report_tracking .cls_input_project_name').val()

                    + "&data.product_code=" + $('.cls_report_tracking .cls_input_product_code').val().replace(/\n/g, ',')
                    + "&data.rd_number=" + $('.cls_report_tracking .cls_input_rd_number').val()
                    + "&data.packaging_type_id=" + $('.cls_report_tracking .cls_lov_packaging_type').val()
                    + "&data.primary_size_txt=" + $('.cls_report_tracking .cls_lov_primary_size').val()
                    + "&data.net_weight_txt=" + $('.cls_report_tracking .cls_input_net_weight').val()

                    + "&data.current_step_id=" + $('.cls_report_tracking .cls_lov_cur_step').val()
                    + "&data.current_step_wf_type=" + current_step_wf_type

                    + "&data.creator_id=" + $('.cls_report_tracking .cls_lov_creator').val()
                    + "&data.supervised_id=" + $('.cls_report_tracking .cls_lov_supervised').val()
                    + "&data.current_assign_id=" + $('.cls_report_tracking .cls_lov_cur_assign').val()
                    + "&data.working_group_id=" + $('.cls_report_tracking .cls_lov_cur_working_group').val()
                    + "&data.workflow_overdue=" + $('.cls_report_tracking .cls_chk_wf_overdue').is(":checked")
                    + "&data.workflow_action_by_me=" + $('.cls_report_tracking .cls_chk_wf_action_by_me').is(":checked")
                    + "&data.ref_wf_no=" + $('.cls_report_tracking .cls_trk_ref_wf_no').val()

                    + "&data.search_so=" + $('.cls_report_tracking .cls_search_so').val().replace(/\n/g, ',')
                    + "&data.search_order_bom=" + $('.cls_report_tracking .cls_search_order_bom').val().replace(/\n/g, ',')
                    + "&data.zone_txt=" + $('.cls_report_tracking .cls_lov_zone option:selected').text()

                    + "&data.first_load=" + first_load

                    + "&data.current_user_id=" + UserID
                    //+ "&data.view=" + 'PG'
                    + "&data.PIC=" + '',
                type: 'GET',
                data: data,
                success: function (res) {
                    groupColumn = DData(res).ORDER_COLUMN;
                    dtSuccess(res, callback);
                }
            });
        },
        //"columnDefs": [{
        //    "searchable": false,
        //    "orderable": false,
        //    "targets": 0
        //}],
        "order": [[1, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "scrollY": "450px",
        "scrollCollapse": true,
        "columns": [
            { "data": "CREATOR_NAME", "className": "cls_creator_name cls_nowrap" },//CREATOR_DISPLAY_TXT
            { "data": "WF_NO", "className": "cls_nowrap cls_wf_no" },//WORKFLOW_NUMBER
            { "data": "PACKAGING_TYPE", "className": "cls_packaging_type cls_nowrap" },//PACKING_TYPE_DISPLAY_TXT
            { "data": "PRIMARY_TYPE_TXT", "className": "cls_primary_type cls_nowrap" },//
            { "data": "WF_STAUTS", "className": "cls_wf_status cls_nowrap" },//CURRENT_STEP_DISPLAY_TXT
            { "data": "CURRENT_STEP_NAME_1", "className": "cls_current_step_1 cls_nowrap" },//CURRENT_STATUS_DISPLAY_TXT
            { "data": "CURRENT_USER_NAME_1", "className": "cls_nowrap" },
            
            { "data": "DURATION_1", "className": "cls_nowrap" },
            { "data": "DUE_DATE_1", "className": "cls_due_date_1 cls_nowrap" },
            { "data": "SOLD_TO", "className": "cls_nowrap" },//SOLD_TO_DISPLAY_TXT
            { "data": "SHIP_TO", "className": "cls_nowrap" },//SHIP_TO_DISPLAY_TXT
            { "data": "PORT", "className": "cls_nowrap" },//
            { "data": "IN_TRANSIT_TO", "className": "cls_nowrap" },//IN_TRANSIT_TO_DISPLAY_TXT
            { "data": "SALES_ORDER_NO", "className": "cls_nowrap" },//SALES_ORDER_NO
            { "data": "CREATE_ON", "className": "cls_nowrap" },//SALES_ORDER_CREATE_DATE
            { "data": "BRAND_NAME", "className": "cls_nowrap" },//BRAND_DISPLAY_TXT
            { "data": "ADDITIONAL_BRAND_NAME", "className": "cls_nowrap" },//ADDITIONAL_BRAND_DISPLAY_TXT
            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },//PRODUCT_CODE_DISPLAY_TXT
            { "data": "PROD_INSP_MEMO", "className": "cls_nowrap" },//PRODUCTION_MEMO_DISPLAY_TXT
            { "data": "REFERENCE_NO", "className": "cls_nowrap" },//RD_NUMBER_DISPLAY_TXT
            { "data": "RDD", "className": "cls_rdd cls_nowrap" },//RDD_DISPLAY_TXT
            { "data": "VENDOR_RFQ", "className": "cls_nowrap" },
            { "data": "SELECTED_VENDOR", "className": "cls_nowrap" },
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var j = 1;
           
            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                if (last !== group) {
                    //$(rows).eq(i).before(
                    //    '<tr class="group highlight"><td colspan="23"></td></tr>'
                    //);
                    last = group;
                    j++;
                }
                else {
                    if (groupColumn == 1) {
                        $(rows).eq(i).find('.cls_creator_name').text('');
                        $(rows).eq(i).find('.cls_wf_no').text('');
                        $(rows).eq(i).find('.cls_packaging_type').text('');
                        $(rows).eq(i).find('.cls_primary_type').text('');
                        $(rows).eq(i).find('.cls_wf_status').text('');
                    }
                }
            });
        },
        "rowCallback": function (row, data, index) {
            if (data.WF_SUB_ID != 0) {
                if (data.WF_NO.indexOf('AW-') >= 0) {
                    $(row).find('.cls_wf_no').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.WF_NO + '</a>');
                    $(row).find('.cls_current_step_1').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_NAME_1 + '</a>');
                }
                else {
                    $(row).find('.cls_wf_no').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.WF_NO + '</a>');
                    $(row).find('.cls_current_step_1').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_NAME_1 + '</a>');
                }
            }
            if (isEmpty(data.CURRENT_STEP_NAME_1)) {
                $(row).find('.cls_current_step_1').html("");
            }
            if (!isEmpty(data.DUE_DATE_1))
                $(row).find('.cls_due_date_1').html(myDateTimeMoment(data.DUE_DATE_1));


        },
    });
}
var current_step_wf_type = '';
