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


    $('.cls_report_tracking .cls_chk_wf_completed').on('change', function () {
        var checked = this.checked
        if (checked == true)
        {
            $(".cls_report_tracking .cls_chk_wf_in_process").prop("checked", false);
        }
      
    });


    $('.cls_report_tracking .cls_chk_wf_in_process').on('change', function () {
        var checked = this.checked
        if (checked == true) {
            $(".cls_report_tracking .cls_chk_wf_completed").prop("checked", false);
        }

    });

   bind_report_track_pg();
   first_load = false;

    //$('#table_report_pg').DataTable({
    //    orderCellsTop: true,
    //    fixedHeader: true,
    //    lengthChange: false,
    //    scrollX: true,
    //    search: false,
    //});


    //$(".cls_trk_wf_type").change(function () {

    //    if ($(this).val() == "AW") {
    //       // alert("A1");
    //        $('.cls_trk_wf_type_sub').empty(); 
    //        $('.cls_trk_wf_type_sub').append('<option selected = "selected" value="">All</option>'); 
    //        $('.cls_trk_wf_type_sub').append('<option value="AW-N-">(AW-N) New</option>');
    //        $('.cls_trk_wf_type_sub').append('<option value="AW-R-">(AW-R) Repeat</option>');
    //        $('.cls_trk_wf_type_sub').append('<option value="AW-R6-">(AW-R6) Repeat over 6 months</option>');
    //    }
    //    if ($(this).val() == "MC") {
    //       // alert("M1");
    //        $('.cls_trk_wf_type_sub').empty();   
    //        $('.cls_trk_wf_type_sub').append('<option selected = "selected" value="">All</option>'); 
    //        $('.cls_trk_wf_type_sub').append('<option value="MO-N-">(MO-N) Normal</option>');
    //        $('.cls_trk_wf_type_sub').append('<option value="MO-P-">(MO-P) Project</option>');
    //        $('.cls_trk_wf_type_sub').append('<option value="MO-D-">(MO-D) Dieline</option>');
    //    }
    // });

    //$(".cls_trk_wf_type_sub").change(function () {

    //    alert($(this).val());
       
    //});


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
        $('.cls_report_tracking .cls_trk_wf_type').val("AW-").change();
    });

    $(".cls_report_tracking .cls_trk_btn_search").click(function (e) {
        try {

            var rdatefrom = $('.cls_report_tracking .cls_trk_req_from').val();
            var rdateto = $('.cls_report_tracking .cls_trk_req_to').val();

            if (!isEmpty(rdatefrom) && !isEmpty(rdateto))
            {
                if ($('.cls_report_tracking .cls_lov_cur_step').select2('data').length > 0) {
                    current_step_wf_type = $('.cls_report_tracking .cls_lov_cur_step').select2('data')[0].WF_TYPE;
                    current_step_id = $('.cls_report_tracking .cls_lov_cur_step').select2('data')[0].STEP_ID;
                }

                else {
                    current_step_wf_type = '';
                    current_step_id = 0;
                }


                if ($('.cls_report_tracking .cls_trk_wf_type').val().indexOf("AW-") != -1) {
                    wf_type = "AW";
                }
                else {
                    wf_type = "MO";
                }

                table_reprot_tracking_pg.ajax.reload();

                e.preventDefault();
            }
            else
            {
                alertError2("Please input request date from and request date to.");

                e.preventDefault();
            }


          
        }
        catch (err) { alert(err.message); }

        //bind_report_track_pg();
        //e.preventDefault();
    });

    $(".cls_report_tracking .cls_btn_export_excel").click(function () {

        if ($('.cls_report_tracking .cls_lov_cur_step').select2('data').length > 0) {
            current_step_wf_type = $('.cls_report_tracking .cls_lov_cur_step').select2('data')[0].WF_TYPE;
            current_step_id = $('.cls_report_tracking .cls_lov_cur_step').select2('data')[0].STEP_ID;
        }

        else {
            current_step_wf_type = '';
            current_step_id = 0;
        }


        if ($('.cls_report_tracking .cls_trk_wf_type').val().indexOf("AW-") != -1) {
            wf_type = "AW";
        }
        else {
            wf_type = "MO";
        }


        window.open("/excel/trackingnewreportV3?data.SEARCH_WF_TYPE=" + $('.cls_report_tracking .cls_trk_req_type').val()

            + "&data.SEARCH_REQUEST_DATE_FROM=" + $('.cls_report_tracking .cls_trk_req_from').val()
            + "&data.SEARCH_REQUEST_DATE_TO=" + $('.cls_report_tracking .cls_trk_req_to').val()
            + "&data.SEARCH_WF_NO=" + $('.cls_report_tracking .cls_trk_wf_no').val()
            + "&data.SEARCH_REQUEST_NO=" + $('.cls_report_tracking .cls_trk_wf_no_2').val()
            + "&data.SEARCH_REFERENCE_FORM_NO=" + $('.cls_report_tracking .cls_trk_ref_wf_no').val()
            + "&data.SEARCH_WF_IS_COMPLETED=" + $('.cls_report_tracking .cls_chk_wf_completed').is(":checked")
            + "&data.SEARCH_WF_IN_PROCESS=" + $('.cls_report_tracking .cls_chk_wf_in_process').is(":checked")

            + "&data.SEARCH_SO_NO=" + $('.cls_report_tracking .cls_search_so').val().replace(/\n/g, ',')
            + "&data.SEARCH_SO_MATERIAL=" + $('.cls_report_tracking .cls_search_order_bom').val().replace(/\n/g, ',')
            + "&data.SEARCH_SO_CREATE_DATE_FROM=" + $('.cls_report_tracking .cls_trk_rdd_from').val()
            + "&data.SEARCH_SO_CREATE_DATE_TO=" + $('.cls_report_tracking .cls_trk_rdd_to').val()

            + "&data.SEARCH_COMPANY_ID=" + $('.cls_report_tracking .cls_lov_company').val()
            + "&data.SEARCH_SOLD_TO_ID=" + $('.cls_report_tracking .cls_lov_sold_to').val()
            + "&data.SEARCH_SHIP_TO_ID=" + $('.cls_report_tracking .cls_lov_ship_to').val()
            + "&data.SEARCH_BRAND_ID=" + $('.cls_report_tracking .cls_lov_brand').val()

            + "&data.SEARCH_ZONE=" + $('.cls_report_tracking .cls_lov_zone option:selected').text()
            + "&data.SEARCH_COUNTRY_ID=" + $('.cls_report_tracking .cls_lov_country').val()
            + "&data.SEARCH_PROJECT_NAME=" + $('.cls_report_tracking .cls_input_project_name').val()

            + "&data.SEARCH_PACKAGING_TYPE=" + $('.cls_report_tracking .cls_lov_packaging_type option:selected').text()

            + "&data.SEARCH_PRODUCT=" + $('.cls_report_tracking .cls_input_product_code').val().replace(/\n/g, ',')
            + "&data.SEARCH_PRIMARY_SIZE=" + $('.cls_report_tracking .cls_lov_primary_size').val()

            + "&data.SEARCH_REFERENCE_NO=" + $('.cls_report_tracking .cls_input_rd_number').val()
            + "&data.SEARCH_NET_WEIGHT=" + $('.cls_report_tracking .cls_input_net_weight').val()


            + "&data.SEARCH_CURRENT_STEP_ID=" + current_step_id  //$('.cls_report_tracking .cls_lov_cur_step').val()
            + "&data.SEARCH_CREATOR_ID=" + $('.cls_report_tracking .cls_lov_creator').val()
            + "&data.SEARCH_SUPERVISED_BY_ID=" + $('.cls_report_tracking .cls_lov_supervised').val()
            + "&data.SEARCH_CURRENT_ASSING_ID=" + $('.cls_report_tracking .cls_lov_cur_assign').val()
            + "&data.SEARCH_WORKING_GROUP_ID=" + $('.cls_report_tracking .cls_lov_cur_working_group').val()
            + "&data.SEARCH_WORKFLOW_IS_OVERDUE=" + $('.cls_report_tracking .cls_chk_wf_overdue').is(":checked")
            + "&data.SEARCH_ACTION_BY_ME=" + $('.cls_report_tracking .cls_chk_wf_action_by_me').is(":checked")
            + "&data.SEARCH_LOGIN_USER_ID=" + UserID

            + "&data.SEARCH_WF_TYPE_X=" + wf_type //$('.cls_report_tracking .cls_trk_wf_type').val()
            + "&data.SEARCH_WF_SUB_TYPE=" + $('.cls_report_tracking .cls_trk_wf_type').val()

            + "&data.VIEW=" + $('.cls_report_tracking .cls_report_track_view').val()
            + "&data.GENERATE_EXCEL=X"
            , '_blank');
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box

    table_reprot_tracking_pg.column(3).visible(false);
    table_reprot_tracking_pg.column(11).visible(false);
});


function bind_report_track_pg() {
    var groupColumn = 1;

    //var table_reprot_tracking_pg = $('#table_report_pg').DataTable()
    //table_reprot_tracking_pg.destroy();
 
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
                url: suburl + "/api/report/trackingreport_v3?data.SEARCH_WF_TYPE=" + $('.cls_report_tracking .cls_trk_req_type').val()
                
                    + "&data.SEARCH_REQUEST_DATE_FROM=" + $('.cls_report_tracking .cls_trk_req_from').val()
                    + "&data.SEARCH_REQUEST_DATE_TO=" + $('.cls_report_tracking .cls_trk_req_to').val()
                    + "&data.SEARCH_WF_NO=" + $('.cls_report_tracking .cls_trk_wf_no').val()
                    + "&data.SEARCH_REQUEST_NO=" + $('.cls_report_tracking .cls_trk_wf_no_2').val()
                    + "&data.SEARCH_REFERENCE_FORM_NO=" + $('.cls_report_tracking .cls_trk_ref_wf_no').val()
                    + "&data.SEARCH_WF_IS_COMPLETED=" + $('.cls_report_tracking .cls_chk_wf_completed').is(":checked")
                    + "&data.SEARCH_WF_IN_PROCESS=" + $('.cls_report_tracking .cls_chk_wf_in_process').is(":checked")

                    + "&data.SEARCH_SO_NO=" + $('.cls_report_tracking .cls_search_so').val().replace(/\n/g, ',')
                    + "&data.SEARCH_SO_MATERIAL=" + $('.cls_report_tracking .cls_search_order_bom').val().replace(/\n/g, ',')
                    + "&data.SEARCH_SO_CREATE_DATE_FROM=" + $('.cls_report_tracking .cls_trk_rdd_from').val()
                    + "&data.SEARCH_SO_CREATE_DATE_TO=" + $('.cls_report_tracking .cls_trk_rdd_to').val()

                    + "&data.SEARCH_COMPANY_ID=" + $('.cls_report_tracking .cls_lov_company').val()
                    + "&data.SEARCH_SOLD_TO_ID=" + $('.cls_report_tracking .cls_lov_sold_to').val()
                    + "&data.SEARCH_SHIP_TO_ID=" + $('.cls_report_tracking .cls_lov_ship_to').val()
                    + "&data.SEARCH_BRAND_ID=" + $('.cls_report_tracking .cls_lov_brand').val()

                    + "&data.SEARCH_ZONE=" + $('.cls_report_tracking .cls_lov_zone option:selected').text()
                    + "&data.SEARCH_COUNTRY_ID=" + $('.cls_report_tracking .cls_lov_country').val()
                    + "&data.SEARCH_PROJECT_NAME=" + $('.cls_report_tracking .cls_input_project_name').val()

                    + "&data.SEARCH_PACKAGING_TYPE=" + $('.cls_report_tracking .cls_lov_packaging_type option:selected').text()

                    + "&data.SEARCH_PRODUCT=" + $('.cls_report_tracking .cls_input_product_code').val().replace(/\n/g, ',')
                    + "&data.SEARCH_PRIMARY_SIZE=" + $('.cls_report_tracking .cls_lov_primary_size').val()

                    + "&data.SEARCH_REFERENCE_NO=" + $('.cls_report_tracking .cls_input_rd_number').val()
                    + "&data.SEARCH_NET_WEIGHT=" + $('.cls_report_tracking .cls_input_net_weight').val()


                    + "&data.SEARCH_CURRENT_STEP_ID=" + current_step_id  //$('.cls_report_tracking .cls_lov_cur_step').val()
                    + "&data.SEARCH_CREATOR_ID=" + $('.cls_report_tracking .cls_lov_creator').val()
                    + "&data.SEARCH_SUPERVISED_BY_ID=" + $('.cls_report_tracking .cls_lov_supervised').val()
                    + "&data.SEARCH_CURRENT_ASSING_ID=" + $('.cls_report_tracking .cls_lov_cur_assign').val()
                    + "&data.SEARCH_WORKING_GROUP_ID=" + $('.cls_report_tracking .cls_lov_cur_working_group').val()
                    + "&data.SEARCH_WORKFLOW_IS_OVERDUE=" + $('.cls_report_tracking .cls_chk_wf_overdue').is(":checked")
                    + "&data.SEARCH_ACTION_BY_ME=" + $('.cls_report_tracking .cls_chk_wf_action_by_me').is(":checked")
                    + "&data.SEARCH_LOGIN_USER_ID=" + UserID

                    + "&data.SEARCH_WF_TYPE_X=" + wf_type //$('.cls_report_tracking .cls_trk_wf_type').val()
                    + "&data.SEARCH_WF_SUB_TYPE=" + $('.cls_report_tracking .cls_trk_wf_type').val()

                    + "&data.first_load=" + first_load
                ,
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
            { "data": "WF_STATUS", "className": "cls_wf_status cls_nowrap" },//CURRENT_STEP_DISPLAY_TXT  4
            { "data": "CURRENT_STEP", "className": "cls_current_step_1 cls_nowrap" },//CURRENT_STATUS_DISPLAY_TXT
            { "data": "CURRENT_ASSING", "className": "cls_nowrap" },
            
            { "data": "CURRENT_DURATION", "className": "cls_nowrap" },  //7
            { "data": "CURRENT_DUE_DATE", "className": "cls_due_date_1 cls_nowrap" },//8
            { "data": "SOLD_TO", "className": "cls_nowrap" },//SOLD_TO_DISPLAY_TXT
            { "data": "SHIP_TO", "className": "cls_nowrap" },//SHIP_TO_DISPLAY_TXT
            { "data": "PORT", "className": "cls_nowrap" },//11
            { "data": "IN_TRANSIT_TO", "className": "cls_nowrap" },//IN_TRANSIT_TO_DISPLAY_TXT 12
            { "data": "SO_NO", "className": "cls_nowrap" },//SALES_ORDER_NO 13
            { "data": "SO_CREATE_DATE", "className": "cls_nowrap" },//SALES_ORDER_CREATE_DATE 14
            { "data": "BRAND_NAME", "className": "cls_nowrap" },//BRAND_DISPLAY_TXT 15
            { "data": "ADDITIONAL_BRAND", "className": "cls_nowrap" },//ADDITIONAL_BRAND_DISPLAY_TXT 16
            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },//PRODUCT_CODE_DISPLAY_TXT 17
            { "data": "PROD_INSP_MEMO", "className": "cls_nowrap" },//PRODUCTION_MEMO_DISPLAY_TXT 18
            { "data": "REFERENCE_NO", "className": "cls_nowrap" },//RD_NUMBER_DISPLAY_TXT 19
            { "data": "RDD", "className": "cls_rdd cls_nowrap" },//RDD_DISPLAY_TXT 20
            { "data": "VENDOR_RFQ", "className": "cls_nowrap" }, //21
            { "data": "SELECTED_VENDOR", "className": "cls_nowrap" }, //22
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
                    $(row).find('.cls_wf_no').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_PA_ID + '" style="text-decoration: underline;">' + data.WF_NO + '</a>');
                    $(row).find('.cls_current_step_1').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP + '</a>');
                }
                else {
                    $(row).find('.cls_wf_no').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_PA_ID + '" style="text-decoration: underline;">' + data.WF_NO + '</a>');
                    $(row).find('.cls_current_step_1').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP + '</a>');
                }
            }
            if (isEmpty(data.CURRENT_STEP)) {
                $(row).find('.cls_current_step_1').html("");
            }
            if (!isEmpty(data.CURRENT_DUE_DATE))
                $(row).find('.cls_due_date_1').html(myDateTimeMoment(data.CURRENT_DUE_DATE));


        },
    });

    //$(".cls_report_track_view").change();
}
var current_step_wf_type = '';
var current_step_id = 0;

var wf_type = "";
