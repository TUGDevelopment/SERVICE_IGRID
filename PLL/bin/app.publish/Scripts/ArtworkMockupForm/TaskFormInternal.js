$(document).ready(function () {

    $('.cls_whmu_row form').validate({
        rules: {
            whmu_rdo_test_pack_result:
            {
                required: true
            },
            wh_difficult:
            {
                required: true
            },
            wh_need_to_com:
            {
                required: true
            }

        },
        messages: {
            whmu_rdo_test_pack_result:
            {
                required: "Please select at least 1 of these fields."
            },
            wh_difficult:
            {
                required: "Please select at least 1 of these fields."
            },
            wh_need_to_com:
            {
                required: "Please select at least 1 of these fields."
            }
        }
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_internal_department':
                if ($(".cls_rdmu_row").is(':visible')) {
                    if (table_rd_log == null)
                        bind_rd();
                    else
                        table_rd_log.ajax.reload();
                }
                if ($(".cls_plnmu_row").is(':visible')) {
                    if (table_pn_log == null)
                        bind_pn();
                    else
                        table_pn_log.ajax.reload();
                }
                if ($(".cls_whmu_row").is(':visible')) {
                    if (table_wh_log == null)
                        bind_wh();
                    else
                        table_wh_log.ajax.reload();

                    table_wh_log.columns.adjust().draw();
                }

                //bind_user_vendor_pg_sup();
                //if (table_user_vendor_pg_sup == null)
                //    bind_user_vendor_pg_sup();
                //else
                //    table_user_vendor_pg_sup.ajax.reload();
                if ($(".cls_manager_approve_matchboard_row").is(':visible')) {
                    if (table_approve_matchboard_log == null)
                        bind_approve_matchboard_log();
                    else
                        table_approve_matchboard_log.ajax.reload();
                }
                if ($(".cls_pg_sup_row").is(':visible')) {
                    if (table_pg_sup_log == null)
                        bind_pg_sup();
                    else
                        table_pg_sup_log.ajax.reload();
                }

                break;

            default:
                break;
        }
    });

    $(".cls_rdmu_row .cls_rdmu_btn_save").click(function () {
        save_rd('SAVE', false);
    });

    $(".cls_rdmu_row .cls_rdmu_btn_send_primary").click(function () {
        if ($('.cls_rdmu_row .cls_input_other').is(':visible') && $('.cls_rdmu_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_rd('SEND_PRI', true);
    });

    $(".cls_rdmu_row .cls_rdmu_btn_send_back").click(function () {
        if ($('.cls_rdmu_row .cls_input_other').is(':visible') && $('.cls_rdmu_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_rd('SEND_BACK', true);
    });

    $(".cls_plnmu_row .cls_plnmu_btn_save").click(function () {
        save_pn('SAVE', false);
    });

    $(".cls_plnmu_row .cls_plnmu_btn_send_primary").click(function () {
        if ($('.cls_plnmu_row  .cls_input_other').is(':visible') && $('.cls_plnmu_row  .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_pn('SEND_PRI', true);
    });

    $(".cls_plnmu_row .cls_plnmu_btn_send_back").click(function () {
        if ($('.cls_plnmu_row .cls_input_other').is(':visible') && $('.cls_plnmu_row  .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_pn('SEND_BACK', true);
    });

    $(".cls_whmu_row .cls_whmu_btn_save").click(function () {
        save_wh('SAVE', false);
    });

    $(".cls_whmu_row form").submit(function (e) {
        if ($(this).valid()) {
            save_wh('SUBMIT', true);
        }
        else if ($('.cls_whmu_row .cls_input_other').is(':visible') && $('.cls_whmu_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_whmu_row .cls_whmu_btn_send_back").click(function () {
        if ($('.cls_whmu_row .cls_input_other').is(':visible') && $('.cls_whmu_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_wh('SEND_BACK', true);
    });

    bind_lov('.cls_whmu_row .cls_whmu_lov_test_pack_result_reject', '/api/common/desisiontestpackfail?data.STEP_CODE=MOCK_UP_WH_SEND_TO_PG_TEST_PACK_FAIL', 'data.description');
    //bind_lov('.cls_pg_sup_row .cls_lov_search_vendor_pg_sup', '/api/lov/vendorhasuser_bymatgroup', 'data.vendor_name', '');

    bind_lov_param('.cls_pg_sup_row .cls_lov_search_vendor_pg_sup', '/api/lov/vendorhasuser_bymatgroup', 'data.DISPLAY_TXT', ["MATGROUP_ID"], ['.cls_task_form_pg .cls_lov_pg_packaging_type']);


    //$(document).on("click", ".cls_pg_sup_row .cls_img_lov_delete_row_user_vendor", function () {
    //    table_user_vendor_pg_sup
    //        .row($(this).parents('tr'))
    //        .remove()
    //        .draw();
    //});

    $(".cls_pg_sup_row .cls_btn_save_vendor_pg_sup").click(function () {
        save_pg_select_vendor('SAVE');
    });

    $(".cls_pg_sup_row .cls_btn_send_back_vendor_pg_sup").click(function () {
        if ($('.cls_pg_sup_row .cls_input_other').is(':visible') && $('.cls_pg_sup_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_pg_select_vendor('SEND_BACK');
    });

    $(".cls_pg_sup_row form").submit(function (e) {
        if ($(this).valid()) {
            save_pg_select_vendor('SUBMIT');
        }
        else if ($('.cls_pg_sup_row .cls_input_other').is(':visible') && $('.cls_pg_sup_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });
    //$(".cls_pg_sup_row .cls_btn_submit_vendor_pg_sup").click(function () {
    //    save_pg_select_vendor('SUBMIT');
    //});

    //$(".cls_pg_sup_row .cls_btn_reset_vendor_pg_sup").click(function () {
    //    table_user_vendor_pg_sup.ajax.reload();
    //});

    $(".cls_manager_approve_matchboard_row .cls_btn_approve").click(function () {
        if ($('.cls_manager_approve_matchboard_row .cls_input_other').is(':visible') && $('.cls_manager_approve_matchboard_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_approve_matchboard_log('APPROVE', true);
    });
    $(".cls_manager_approve_matchboard_row .cls_btn_not_approve").click(function () {
        if ($('.cls_manager_approve_matchboard_row .cls_input_other').is(':visible') && $('.cls_manager_approve_matchboard_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_approve_matchboard_log('NOTAPPROVE', true);
    });
    $(".cls_manager_approve_matchboard_row .cls_btn_send_back").click(function () {
        if ($('.cls_manager_approve_matchboard_row .cls_input_other').is(':visible') && $('.cls_manager_approve_matchboard_row .cls_input_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_approve_matchboard_log('SEND_BACK', true);
    });
    $(".cls_manager_approve_matchboard_row .cls_btn_save").click(function () {
        save_approve_matchboard_log('SAVE', false);
    });



    //$(".cls_mk_send_to_warehouse_row .cls_btn_mk_send_to_warehouse").click(function () {
    //    var jsonObj = new Object();
    //    var item = {};
    //    item["MOCKUP_ID"] = MOCKUPID;
    //    item["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
    //    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    //    item["CURRENT_STEP_ID"] = getstepmockup('SEND_WH_UPD_PACK_STYLE').curr_step;;
    //    item["CURRENT_ROLE_ID"] = getstepmockup('SEND_WH_UPD_PACK_STYLE').curr_role;
    //    item["REMARK"] = $(".cls_mk_send_to_warehouse_row .cls_txt_mk_send_to_warehouse").val();
    //    item["CREATE_BY"] = UserID;
    //    item["UPDATE_BY"] = UserID;
    //    jsonObj.data = item;

    //    var myurl = '/api/taskform/mockupprocess/mksendtowh';
    //    var mytype = 'POST';
    //    var mydata = jsonObj;

    //    myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
    //});

    $('.cls_pg_sup_row .cls_lov_search_vendor_pg_sup').attr("required", true);

    $('.cls_whmu_row input[type=radio][name=whmu_rdo_test_pack_result]').change(function () {
        if (this.value == '1') {
            $('.cls_whmu_row .cls_whmu_lov_test_pack_result_reject').attr("required", false);
        }
        else if (this.value == '0') {
            $('.cls_whmu_row .cls_whmu_lov_test_pack_result_reject').attr("required", true);
        }
    });
});

var table_rd_log;
function bind_rd() {
    table_rd_log = $('#table_rd_log').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/internal/rd?data.mockup_id=' + MOCKUPID,
                type: 'GET',
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
        "order": [[4, 'desc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "IS_FINAL_SAMPLE_DISPLAY_TXT", "className": "" },
            { "data": "TYPE_OF_SAMPLE_DISPLAY_TXT", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(11).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));
            if (data.MOCKUP_SUB_ID == MOCKUPSUBID) {
                $('.cls_rdmu_row .cls_rdmu_txt_comment').val(data.COMMENT);
                $('.cls_rdmu_row input:radio[name=rd_final_sample]').filter('[value=' + data.IS_FINAL_SAMPLE + ']').prop('checked', true);
                $('.cls_rdmu_row input:radio[name=rd_type_of_sample]').filter('[value=' + data.TYPE_OF_SAMPLE + ']').prop('checked', true);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_rdmu_row .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
            }
        },
        "fnDrawCallback": function (oSettings) {

        },
    });

    table_rd_log.on('order.dt search.dt', function () {
        table_rd_log.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_rd(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    jsonObj.data = [];

    item["MOCKUP_ID"] = MOCKUPID;
    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["ACTION_CODE"] = ACTION_CODE;
    item["COMMENT"] = $('.cls_rdmu_row .cls_rdmu_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["ENDTASKFORM"] = EndTaskForm;
    item["IS_FINAL_SAMPLE"] = $(".cls_rdmu_row input[name='rd_final_sample']:checked").val();
    item["TYPE_OF_SAMPLE"] = $(".cls_rdmu_row input[name='rd_type_of_sample']:checked").val();
    item["REASON_ID"] = $(".cls_rdmu_row .cls_lov_send_for_reason").val();

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1 && EndTaskForm) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }
    item["WF_SUB_ID"] = MOCKUPSUBID;
    item["REMARK_REASON"] = $(".cls_rdmu_row .cls_input_other").val();
    item["WF_STEP"] = getstepmockup('SEND_RD_PRI_PKG').curr_step;

    jsonObj.data.push(item);

    var myurl = '/api/taskform/internal/rd';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}

var table_pn_log;
function bind_pn() {
    table_pn_log = $('#table_pn_log').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/internal/planning?data.mockup_id=' + MOCKUPID,
                type: 'GET',
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
        "order": [[4, 'desc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));
            if (data.MOCKUP_SUB_ID == MOCKUPSUBID) {
                $('.cls_plnmu_row .cls_plnmu_txt_comment').val(data.COMMENT);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_plnmu_row .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
            }
        },
    });

    table_pn_log.on('order.dt search.dt', function () {
        table_pn_log.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_pn(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    jsonObj.data = [];

    var item = {};
    item["MOCKUP_ID"] = MOCKUPID;
    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["ACTION_CODE"] = ACTION_CODE;
    item["COMMENT"] = $('.cls_plnmu_row .cls_plnmu_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["EndTaskForm"] = EndTaskForm;
    item["REASON_ID"] = $(".cls_plnmu_row .cls_lov_send_for_reason").val();

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1 && EndTaskForm) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    item["WF_SUB_ID"] = MOCKUPSUBID;
    item["REMARK_REASON"] = $(".cls_plnmu_row .cls_input_other").val();
    item["WF_STEP"] = getstepmockup('SEND_PN_PRI_PKG').curr_step;

    jsonObj.data.push(item);

    var myurl = '/api/taskform/internal/planning';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}


var table_wh_log;
function bind_wh() {
    table_wh_log = $('#table_wh_log').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/internal/warehouse?data.mockup_id=' + MOCKUPID,
                type: 'GET',
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
        "order": [[9, 'desc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },

            { "data": "TEST_PACK_SIZING_DISPLAY_TXT", "className": "" },
            { "data": "TEST_PACK_HARD_EASY_DISPLAY_TXT", "className": "" },
            { "data": "SUPPLIER_PRIMARY_CONTAINER", "className": "" },
            { "data": "SUPPLIER_PRIMARY_LID", "className": "" },
            { "data": "SHIP_TO_FACTORY", "className": "" },

            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "RECEIVE_PHYSICAL_DISPLAY_TXT", "className": "" },
            { "data": "TEST_PACK_RESULT_DISPLAY_TXT", "className": "" },
            { "data": "NEED_COMMISSIONING_DISPLAY_TXT", "className": "" },
            { "data": "IS_DIFFICULT_DISPLAY_TXT", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(18).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));

            if (data.MOCKUP_SUB_ID == MOCKUPSUBID) {

                $('.cls_whmu_row .cls_whmu_txt_comment').val(data.COMMENT);
                $('.cls_whmu_row .cls_whmu_chk_received').prop('checked', false);
                if (data.RECEIVE_PHYSICAL == 'X') $('.cls_whmu_row .cls_whmu_chk_received').prop('checked', true);
                $('.cls_whmu_row input:radio[name=whmu_rdo_test_pack_result]').filter('[value=' + data.TEST_PACK_RESULT + ']').prop('checked', true);
                $('.cls_whmu_row input:radio[name=wh_need_to_com]').filter('[value=' + data.NEED_COMMISSIONING + ']').prop('checked', true);
                $('.cls_whmu_row input:radio[name=wh_difficult]').filter('[value=' + data.IS_DIFFICULT + ']').prop('checked', true);

                setValueToDDL('.cls_whmu_row .cls_whmu_lov_test_pack_result_reject', data.TEST_PACK_FAIL_ID, data.TEST_PACK_FAIL_DISPLAY_TXT);

                if (data.REASON_ID != null)
                    setValueToDDL('.cls_whmu_row .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);

                if (data.TEST_PACK_SIZING == 'X') $('.cls_whmu_row .cls_chk_confirm_size').prop('checked', true);
                if (data.TEST_PACK_HARD_EASY == 'X') $('.cls_whmu_row .cls_chk_easy').prop('checked', true);

                $('.cls_whmu_row .cls_wh_supplier_primary_container').val(data.SUPPLIER_PRIMARY_CONTAINER);
                $('.cls_whmu_row .cls_wh_supplier_primary_lid').val(data.SUPPLIER_PRIMARY_LID);
                $('.cls_whmu_row .cls_wh_remark').val(data.SHIP_TO_FACTORY);



            }
        },
    });

    table_wh_log.on('order.dt search.dt', function () {
        table_wh_log.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_wh(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    jsonObj.data = [];

    item["MOCKUP_ID"] = MOCKUPID;
    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["ACTION_CODE"] = ACTION_CODE;
    item["COMMENT"] = $('.cls_whmu_row .cls_whmu_txt_comment').val();
    item["TEST_PACK_RESULT"] = $(".cls_whmu_row input[name='whmu_rdo_test_pack_result']:checked").val();
    item["NEED_COMMISSIONING"] = $(".cls_whmu_row input[name='wh_need_to_com']:checked").val();
    item["RECEIVE_PHYSICAL"] = $(".cls_whmu_row .cls_whmu_chk_received").is(":checked") ? "X" : "";
    item["IS_DIFFICULT"] = $(".cls_whmu_row input[name='wh_difficult']:checked").val();
    if ($(".cls_whmu_row input[name='whmu_rdo_test_pack_result']:checked").val() == '0') {
        item["TEST_PACK_FAIL_ID"] = $('.cls_whmu_row .cls_whmu_lov_test_pack_result_reject').val();
    }
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["ENDTASKFORM"] = EndTaskForm;
    item["REASON_ID"] = $(".cls_whmu_row .cls_lov_send_for_reason").val();

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1 && EndTaskForm) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    item["WF_SUB_ID"] = MOCKUPSUBID;
    item["REMARK_REASON"] = $(".cls_whmu_row .cls_input_other").val();
    item["WF_STEP"] = getstepmockup('SEND_WH_TEST_PACK').curr_step;

    jsonObj.data.push(item);

    var myurl = '/api/taskform/internal/warehouse';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}
//function bind_user_vendor_pg_sup() {
//    var myurl = '/api/taskform/pg/selectvendor_noqua?data.mockup_id=' + MOCKUPID;
//    var mytype = 'GET';
//    var mydata = null;
//    myAjaxNoSync(myurl, mytype, mydata, callback_bind_user_vendor_pg_sup);
//}

//function callback_bind_user_vendor_pg_sup(res) {
//    $('.cls_vendor_name_sup_select').val(res.data[0].VENDOR_DISPLAY_TXT);
//}

//var table_user_vendor_pg_sup;
//function bind_user_vendor_pg_sup() {
//    table_user_vendor_pg_sup = $('.cls_table_user_vendor_pg_sup').DataTable({
//        ajax: function (data, callback, settings) {
//            $.ajax({
//                url: suburl + '/api/taskform/pg/selectvendor_noqua?data.mockup_id=' + MOCKUPID,
//                type: 'GET',
//                success: function (res) {
//                  dtSuccess(res, callback);
//                }
//            });
//        },
//        "columnDefs": [{
//            "searchable": false,
//            "orderable": false,
//            "targets": 0
//        }],
//        "order": [[2, 'asc']],
//        "processing": true,
//        "lengthChange": false,
//        "ordering": true,
//        "info": false,
//        "searching": false,
//        "paging": false,
//        "scrollX": true,
//        "columns": [
//            {
//                "className": "cls_td_delete_row",
//                render: function (data, type, row, meta) {
//                    return '<img class="cls_img_lov_delete_row_user_vendor" title="Delete" style="cursor:pointer;" src="/Content/img/ico_delete.png" />'
//                }
//            },
//            {
//                render: function (data, type, row, meta) {
//                    return meta.row + meta.settings._iDisplayStart + 1;
//                }
//            },
//            { "data": "VENDOR_DISPLAY_TXT", "className": "" },
//            { "data": "USER_DISPLAY_TXT", "className": "" },
//            { "data": "EMAIL", "className": "" },
//            { "data": "USER_ID", "className": "cls_hide cls_user_id" },
//            { "data": "VENDOR_ID", "className": "cls_hide cls_vendor_id" },
//        ],
//        "rowCallback": function (row, data, index) {


//        },
//        "fnDrawCallback": function (oSettings) {
//            if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN") {
//                $(".cls_td_delete_row").each(function (index) {
//                    $(this).show();
//                });
//            }
//            else {
//                $(".cls_td_delete_row").each(function (index) {
//                    $(this).hide();
//                });
//            }
//        },
//        "initComplete": function (settings, json) {

//        }
//    });

//    table_user_vendor_pg_sup.on('order.dt search.dt', function () {
//        table_user_vendor_pg_sup.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
//            cell.innerHTML = i + 1;
//        });
//    }).draw();
//}
//function add_row_user_vendor_pg_sup(res) {
//    table_user_vendor_pg_sup.rows.add([{
//        "VENDOR_DISPLAY_TXT": res.data[0].VENDOR_DISPLAY_TXT,
//        "USER_DISPLAY_TXT": res.data[0].USER_DISPLAY_TXT,
//        "EMAIL": res.data[0].EMAIL,
//        "USER_ID": res.data[0].USER_ID,
//        "VENDOR_ID": res.data[0].VENDOR_ID,
//    }]).draw(false);
//}
//function callback_lov_select_pg_sup(obj) {
//    bind_table_vendor_user_pg_sup($(obj).val());
//}
//function bind_table_vendor_user_pg_sup(vendorID) {
//    var myurl = '/api/lov/vendoruser?data.vendor_id=' + vendorID;
//    var mytype = 'GET';
//    var mydata = null;
//    myAjax(myurl, mytype, mydata, add_row_user_vendor_pg_sup);
//}
function save_pg_select_vendor(type) {

    //if (table_user_vendor_pg_sup.data().count() == 0) {
    //    alertError2("Please select vendor at least 1 item.");
    //    return;
    //}
    //if (table_user_vendor_pg_sup.data().count() > 1) {
    //    alertError2("Please select 1 vendor.");
    //    return;
    //}

    var jsonObj = new Object();
    jsonObj.data = [];

    if (type == 'SUBMIT' || type == 'SEND_BACK') {
        jsonObj["ENDTASKFORM"] = true;
    }
    //$(".cls_table_user_vendor_pg_sup tbody tr:visible").each(function (index) {
    var item = {};
    item["MOCKUP_ID"] = MOCKUPID;
    item["ACTION_CODE"] = type;
    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["VENDOR_ID"] = $('.cls_pg_sup_row .cls_lov_search_vendor_pg_sup').val();
    //item["USER_ID"] = $(this).find('.cls_user_id').text();
    item["REMARK"] = $('.cls_pg_sup_row .cls_only_pg_sup .cls_txt_remark_select_vendor_sup').val();
    item["REASON_ID"] = $(".cls_pg_sup_row .cls_lov_send_for_reason").val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1 && type != "SAVE") {
        alertError2("Please select reason for overdue");
        return false;
    }

    if (item["REASON_ID"] == DefaultResonId && type == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    item["WF_SUB_ID"] = MOCKUPSUBID;
    item["REMARK_REASON"] = $(".cls_pg_sup_row .cls_input_other").val();
    item["WF_STEP"] = getstepmockup('SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN').curr_step;

    jsonObj.data.push(item);
    //});

    var myurl = '/api/taskform/pg/selectvendor_noqua';
    var mytype = 'POST';
    var mydata = jsonObj;
    if (type == 'SAVE')
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
    else {
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    }
}


var table_approve_matchboard_log;
function bind_approve_matchboard_log() {
    table_approve_matchboard_log = $('#table_approve_matchboard_log').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/internal/approvematchboard?data.mockup_id=' + MOCKUPID,
                type: 'GET',
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
        "order": [[4, 'desc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));
            if (data.MOCKUP_SUB_ID == MOCKUPSUBID) {
                $('.cls_manager_approve_matchboard_row .cls_manager_approve_matchboard_txt_comment').val(data.COMMENT);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_manager_approve_matchboard_row .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
            }
        },
    });

    table_approve_matchboard_log.on('order.dt search.dt', function () {
        table_approve_matchboard_log.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}
function save_approve_matchboard_log(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    jsonObj.data = [];

    item["MOCKUP_ID"] = MOCKUPID;
    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["ACTION_CODE"] = ACTION_CODE;
    item["COMMENT"] = $('.cls_manager_approve_matchboard_row .cls_manager_approve_matchboard_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["EndTaskForm"] = EndTaskForm;
    item["REASON_ID"] = $(".cls_manager_approve_matchboard_row .cls_lov_send_for_reason").val();

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1 && EndTaskForm) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    item["WF_SUB_ID"] = MOCKUPSUBID;
    item["REMARK_REASON"] = $(".cls_manager_approve_matchboard_row .cls_input_other").val();
    item["WF_STEP"] = getstepmockup('SEND_APP_MATCH_BOARD').curr_step;

    jsonObj.data.push(item);

    var myurl = '/api/taskform/internal/approvematchboard';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}


var table_pg_sup_log;
function bind_pg_sup() {
    table_pg_sup_log = $('.cls_table_user_vendor_pg_sup_log').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pg/selectvendor_noqua_log?data.mockup_id=' + MOCKUPID,
                type: 'GET',
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
        "order": [[3, 'desc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "VENDOR_DISPLAY_TXT", "className": "" },
            { "data": "REMARK", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));

            if (data.MOCKUP_SUB_ID == MOCKUPSUBID) {
                $('.cls_pg_sup_row .cls_txt_remark_select_vendor_sup').val(data.REMARK);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_pg_sup_row .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
                setValueToDDL('.cls_pg_sup_row .cls_lov_search_vendor_pg_sup', data.VENDOR_ID, data.VENDOR_DISPLAY_TXT);
            }
        },
    });

    table_pg_sup_log.on('order.dt search.dt', function () {
        table_pg_sup_log.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}