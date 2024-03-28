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
    //$(".cls_report_tracking .cls_div_search_criteria").click(function () {
    //    if ($('.cls_div_body_search_criteria').is(':visible'))
    //        $('.cls_div_body_search_criteria').hide();
    //    else
    //        $('.cls_div_body_search_criteria').show();
    //});

    $('.cls_table_report_pg thead tr').clone(true).appendTo('.cls_table_report_pg thead');
    $('.cls_table_report_pg thead tr:eq(1) th').each(function (i) {
        if (i == 0) {
            $(this).html('');
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });
    $('.cls_table_report_mk thead tr').clone(true).appendTo('.cls_table_report_mk thead');
    $('.cls_table_report_mk thead tr:eq(1) th').each(function (i) {
        if (i == 0) {
            $(this).html('');
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    bind_report_track_pg();
    bind_report_track_mk();
    first_load = false;

    $(table_reprot_tracking_pg.table().container()).on('keyup', 'input', function () {
        table_reprot_tracking_pg
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });
    $(table_reprot_tracking_mk.table().container()).on('keyup', 'input', function () {
        table_reprot_tracking_mk
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    $(".cls_report_track_view").change(function () {
        $('.cls_table_report_pg').hide();
        $('.cls_table_report_mk').hide();

        if ($(this).val() == "pg") {
            table_reprot_tracking_pg.ajax.reload();
            $('.cls_table_report_pg').show();
        }
        if ($(this).val() == "mk") {
            table_reprot_tracking_mk.ajax.reload();
            $('.cls_table_report_mk').show();
        }
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

            if ($(".cls_report_track_view").val() == "pg")
                table_reprot_tracking_pg.ajax.reload();
            else
                table_reprot_tracking_mk.ajax.reload();
            e.preventDefault();
        }
        catch (err) { alert(err.message); }
    });

    $(".cls_report_tracking .cls_btn_export_excel").click(function () {
       
        window.open("/excel/trackingreport?data.workflow_no=" + $('.cls_report_tracking .cls_trk_wf_no').val()

            + "&data.workflow_type=" + $('.cls_report_tracking .cls_trk_req_type').val()
            + "&data.workflow_no_2=" + $('.cls_report_tracking .cls_trk_wf_no_2').val()
            + "&data.request_date_from=" + $('.cls_report_tracking .cls_trk_req_from').val()
            + "&data.request_date_to=" + $('.cls_report_tracking .cls_trk_req_to').val()
            + "&data.workflow_completed=" + $('.cls_report_tracking .cls_chk_wf_completed').is(":checked")

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
});

var table_reprot_tracking_pg;
function bind_report_track_pg() {
    table_reprot_tracking_pg = $('#table_report_pg').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        fixedColumns: {
            leftColumns: 3
        },
        ajax: function (data, callback, settings) {

            $.ajax({
                url: suburl + "/api/report/trackingreport?data.workflow_no=" + $('.cls_report_tracking .cls_trk_wf_no').val()
                    + "&data.workflow_type=" + $('.cls_report_tracking .cls_trk_req_type').val()
                    + "&data.workflow_no_2=" + $('.cls_report_tracking .cls_trk_wf_no_2').val()
                    + "&data.request_date_from=" + $('.cls_report_tracking .cls_trk_req_from').val()
                    + "&data.request_date_to=" + $('.cls_report_tracking .cls_trk_req_to').val()
                    + "&data.workflow_completed=" + $('.cls_report_tracking .cls_chk_wf_completed').is(":checked")

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
                    + "&data.view=" + 'PG'
                    + "&data.PIC=" + '',
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [{
            "searchable": false,
            "orderable": false,
            "targets": 0
        }],
        "order": [[2, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "scrollY": "450px",
        "scrollCollapse": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "PIC_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "WORKFLOW_NUMBER", "className": "cls_td_width_140" },
            { "data": "PACKING_TYPE_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "CURRENT_STATUS_DISPLAY_TXT", "className": "cls_nowrap" },

            { "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
            { "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
            { "data": "IN_TRANSIT_TO_DISPLAY_TXT", "className": "cls_td_width_140" },

            { "data": "SALES_ORDER_NO", "className": "cls_td_width_140" },
            { "data": "SALES_ORDER_CREATE_DATE", "className": "cls_td_width_140" },
           // { "data": "SALES_ORDER_ITEM", "className": "cls_td_width_140" },
            { "data": "BRAND_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "ADDITIONAL_BRAND_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "PRODUCT_CODE_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "PRODUCTION_MEMO_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "RD_NUMBER_DISPLAY_TXT", "className": "cls_td_width_140" },

            { "data": "RDD", "className": "cls_rdd cls_td_width_100" },

            { "data": "VENDOR_RFQ", "className": "cls_td_width_200" },
            { "data": "SELECTED_VENDOR", "className": "cls_td_width_200" },
        ],
        "rowCallback": function (row, data, index) {
            if (data.MOCKUP_SUB_ID != 0) {
                if (data.WORKFLOW_NUMBER.indexOf('AW-') >= 0)
                    $(row).find('td').eq(2).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.WORKFLOW_NUMBER + '</a>');
                else
                    $(row).find('td').eq(2).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.WORKFLOW_NUMBER + '</a>');
            }
            if (!isEmpty(data.RDD))
                $(row).find('.cls_rdd').html(moment(data.RDD).format('DD/MM/YYYY'));

            //bindTooltip(row);
        },
    });
}

var current_step_wf_type = '';

var table_reprot_tracking_mk;
function bind_report_track_mk() {
    table_reprot_tracking_mk = $('#table_report_mk').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        fixedColumns: {
            leftColumns: 3
        },
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/trackingreport?data.workflow_no=" + $('.cls_report_tracking .cls_trk_wf_no').val()
                    + "&data.workflow_type=" + $('.cls_report_tracking .cls_trk_req_type').val()
                    + "&data.workflow_no_2=" + $('.cls_report_tracking .cls_trk_wf_no_2').val()
                    + "&data.request_date_from=" + $('.cls_report_tracking .cls_trk_req_from').val()
                    + "&data.request_date_to=" + $('.cls_report_tracking .cls_trk_req_to').val()
                    + "&data.workflow_completed=" + $('.cls_report_tracking .cls_chk_wf_completed').is(":checked")

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
                    + "&data.view=" + 'MK'
                    + "&data.PIC=" + '',
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [{
            "searchable": false,
            "orderable": false,
            "targets": 0
        }],
        "order": [[2, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "PIC_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "WORKFLOW_NUMBER", "className": "cls_td_width_140" },
            { "data": "PACKING_TYPE_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "PRIMARY_TYPE_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "CURRENT_STATUS_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
            { "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
            { "data": "ROUTE_DESC_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "IN_TRANSIT_TO_DISPLAY_TXT", "className": "cls_td_width_140" },




            { "data": "SALES_ORDER_NO", "className": "cls_td_width_140" },
            { "data": "SALES_ORDER_CREATE_DATE", "className": "cls_td_width_140" },
          //  { "data": "SALES_ORDER_ITEM", "className": "cls_td_width_140" },
            { "data": "BRAND_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "ADDITIONAL_BRAND_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "PRODUCT_CODE_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "PRODUCTION_MEMO_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "RD_NUMBER_DISPLAY_TXT", "className": "cls_td_width_140" },

            { "data": "RDD", "className": "cls_td_width_100 cls_rdd" },



        ],
        "rowCallback": function (row, data, index) {
            if (data.MOCKUP_SUB_ID != 0) {
                if (data.WORKFLOW_NUMBER.indexOf('AW-') >= 0)
                    $(row).find('td').eq(2).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.WORKFLOW_NUMBER + '</a>');
                else
                    $(row).find('td').eq(2).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.WORKFLOW_NUMBER + '</a>');
            }
            if (!isEmpty(data.RDD))
                $(row).find('.cls_rdd').html(moment(data.RDD).format('DD/MM/YYYY'));

            //bindTooltip(row);
        },
    });
}