var MOCKUPSUBVNID = 0;
var fmvendor = '';
var STEPMOCKUPCODE = '';

$(document).ready(function () {
    //bind_detail_vendor(ArtworkSubId);
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_vendor':
                load_tab_vendor_artwork();
                break;
            default:
                break;
        }
    });
    //var pg_submit_modal_vendor = "#pg_submit_modal_vendor ";
    //bind_text_editor(pg_submit_modal_vendor + '.cls_txt_send_pr');
    //bind_text_editor(pg_submit_modal_vendor + '.cls_txt_send_rs');


    //$(pg_submit_modal_vendor + '.cls_chk_send_pr').click(function () {
    //    if ($(this).prop('checked')) {
    //        $(pg_submit_modal_vendor + '.cls_body_send_pr').show();
    //        $(pg_submit_modal_vendor + '.cls_body_send_rs').hide();


    //    }
    //    else {
    //        $(pg_submit_modal_vendor + '.cls_body_send_pr').hide();
    //    }
    //});
    //$(pg_submit_modal_vendor + '.cls_chk_send_rs').click(function () {
    //    if ($(this).prop('checked')) {
    //        $(pg_submit_modal_vendor + '.cls_body_send_rs').show();
    //        $(pg_submit_modal_vendor + '.cls_body_send_pr').hide();
    //    }
    //    else {
    //        $(pg_submit_modal_vendor + '.cls_body_send_rs').hide();
    //    }
    //});

    //$("#pg_submit_modal_vendor .cls_btn_submit_send_vendor").click(function (e) {
    //    e.preventDefault();
    //    SubmitDataVendorPop();
    //});

    $(".cls_vendor_printing_master .cls_btn_submit_vendor").click(function () {
        if ($('.cls_input_pm_other').is(':visible') && $('.cls_input_pm_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            vendor_send_submit('SUBMIT', true);
    });
    $(".cls_vendor_printing_master .cls_btn_send_back_vendor").click(function () {
        if ($('.cls_input_pm_other').is(':visible') && $('.cls_input_pm_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            vendor_send_submit('SEND_BACK', true);
    });

    $(".cls_vendor_request_shade_limit .cls_btn_submit_vendor").click(function () {
        if ($('.cls_input_sl_other').is(':visible') && $('.cls_input_sl_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            vendor_send_submit('SUBMIT', true);
    });
    $(".cls_vendor_request_shade_limit .cls_btn_send_back_vendor").click(function () {
        if ($('.cls_input_sl_other').is(':visible') && $('.cls_input_sl_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            vendor_send_submit('SEND_BACK', true);
    });

    $(".cls_send_po_vendor .cls_btn_submit_vendor").click(function () {
        if ($('.cls_input_po_other').is(':visible') && $('.cls_input_po_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            vendor_po_submit('SUBMIT', true);
    });
    $(".cls_send_po_vendor .cls_btn_send_back_vendor").click(function () {
        if ($('.cls_input_po_other').is(':visible') && $('.cls_input_po_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            vendor_po_submit('SEND_BACK', true);
    });

    $(".cls_btn_upload_file_vendor").click(function () {
        var $form = null;
        $(function () {
            $form = $('#fileupload').fileupload({
                dataType: 'json',
                formData: { mockupSubId: MOCKUPSUBID, userId: UserID },
                url: suburl + '/FileUpload/Upload',
            });
        });
    });

});

function bindSaleOrder_() {
    var myurl = '/api/taskform/salesorderitem/get?data.artwork_request_id=' + ARTWORK_REQUEST_ID
    var mytype = 'GET';
    var mydata = null;
    myAjaxNoSync(myurl, mytype, mydata, callback_get_saleordervendor, callback_get_saleordervendor);
}

function callback_get_saleordervendor(res) {
    if (res.data.length > 0) {
        for (var i = 0; i < res.data.length; i++) {
            var v = res.data[i];
            //Set value for Vendor tab
            if ($('.cls_vendor_txt_order_no').val() != "") {
                $('.cls_vendor_txt_order_no').val($('.cls_vendor_txt_order_no').val() + ", " + v.SALES_ORDER_NO + ' (' + v.SALES_ORDER_ITEM + ')');
            }
            else {
                $('.cls_vendor_txt_order_no').val(v.SALES_ORDER_NO + ' (' + v.SALES_ORDER_ITEM + ')');
            }
        }
    }
}

function load_tab_vendor_artwork() {
    bindSaleOrder_();
    if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_PM') {
        $('.cls_vendor_printing_master').show();
        if (table_vendor_log_vn_pm == null)
            bind_vendor_vn_pm();
        else
            table_vendor_log_vn_pm.ajax.reload();
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_SL') {
        $('.cls_vendor_request_shade_limit').show();
        if (table_vendor_log_vn_sl == null)
            bind_vendor_vn_sl();
        else
            table_vendor_log_vn_sl.ajax.reload();
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_PO') {
        $('.cls_send_po_vendor').show();
        if (table_vendor_log_vn_po == null)
            bind_vendor_vn_po();
        else
            table_vendor_log_vn_po.ajax.reload();
    }
    else {
        $('.cls_vendor_printing_master').show();
        $('.cls_vendor_request_shade_limit').show();
        $('.cls_send_po_vendor').show();
        $('.cls_div_only_vendor').hide();
        if (table_vendor_log_vn_pm == null)
            bind_vendor_vn_pm();
        else
            table_vendor_log_vn_pm.ajax.reload();
        if (table_vendor_log_vn_sl == null)
            bind_vendor_vn_sl();
        else
            table_vendor_log_vn_sl.ajax.reload();
        if (table_vendor_log_vn_po == null)
            bind_vendor_vn_po();
        else
            table_vendor_log_vn_po.ajax.reload();
    }
}

function vendor_send_submit(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    jsonObj.data = {};
    jsonObj.data["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    jsonObj.data["ARTWORK_SUB_ID"] = ArtworkSubId;
    jsonObj.data["ENDTASKFORM"] = EndTaskForm;
    if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_PM') {
        jsonObj.data["REASON_ID"] = $('.cls_vendor_printing_master .cls_lov_send_for_reason').val();
        jsonObj.data["COMMENT"] = $('.cls_vendor_printing_master .cls_vendor_txt_comment').val();
        jsonObj.data["REMARK_REASON"] = $(".cls_vendor_printing_master .cls_input_pm_other").val();
        jsonObj.data["WF_STEP"] = getstepartwork('SEND_VN_PM').curr_step;
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_SL') {
        jsonObj.data["REASON_ID"] = $('.cls_vendor_request_shade_limit .cls_lov_send_for_reason').val();
        jsonObj.data["COMMENT"] = $('.cls_vendor_request_shade_limit .cls_vendor_txt_comment').val();
        jsonObj.data["REMARK_REASON"] = $(".cls_vendor_request_shade_limit .cls_input_sl_other").val();
        jsonObj.data["WF_STEP"] = getstepartwork('SEND_VN_SL').curr_step;
    }

    if (jsonObj.data["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    jsonObj.data["ACTION_CODE"] = ACTION_CODE;

    jsonObj.data["CREATE_BY"] = UserID;
    jsonObj.data["UPDATE_BY"] = UserID;

    if (jsonObj.data["REASON_ID"] == DefaultResonId && OverDue == 1) {
        alertError2("Please select reason for overdue");
        return false;
    }

    var myurl = '/api/taskform/pa/vendor/info';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);

}

function vendor_po_submit(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    jsonObj.data = {};
    jsonObj.data["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    jsonObj.data["ARTWORK_SUB_ID"] = ArtworkSubId;
    jsonObj.data["ENDTASKFORM"] = EndTaskForm;
    jsonObj.data["REASON_ID"] = $('.cls_send_po_vendor .cls_lov_send_for_reason').val();
    jsonObj.data["COMMENT"] = $('.cls_send_po_vendor .cls_vendor_txt_comment').val();

    if (jsonObj.data["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }
    jsonObj.data["REMARK_REASON"] = $(".cls_send_po_vendor .cls_input_po_other").val();
    jsonObj.data["WF_STEP"] = getstepartwork('SEND_VN_PO').curr_step;

    var str_is_confirm = '';
    if ($('.cls_send_po_vendor .cls_chk_confirm').prop('checked')) {
        str_is_confirm = 'X';
    }
    jsonObj.data["CONFIRM_PO"] = str_is_confirm;
    jsonObj.data["ACTION_CODE"] = ACTION_CODE;

    jsonObj.data["CREATE_BY"] = UserID;
    jsonObj.data["UPDATE_BY"] = UserID;

    if (jsonObj.data["REASON_ID"] == DefaultResonId && OverDue == 1) {
        alertError2("Please select reason for overdue");
        return false;
    }

    var myurl = '/api/taskform/pp/vendor/info';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
}


//function selectOnlyThis(id) {
//    for (var i = 1; i <= 2; i++) {
//        if ("vn_rdo_rs" + i === id && document.getElementById("vn_rdo_rs" + i).checked === true) {
//            document.getElementById("vn_rdo_rs" + i).checked = true;
//        } else {
//            document.getElementById("vn_rdo_rs" + i).checked = false;
//        }
//    }
//}

var table_vendor_log_vn_pm;
function bind_vendor_vn_pm() {
    table_vendor_log_vn_pm = $('#table_vendor_log_vn_pm').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtovendor?data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.step_artwork_code=SEND_VN_PM&data.send_to_vendor_type=REQ_PRINT_MASTER',
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
        "order": [[5, 'desc']],
        "vn_pmocessing": true,
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
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "REASON_BY_PA", "className": "cls_nowrap" },
            { "data": "REMARK_REASON_BY_PA", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },

        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(8).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(5).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_vendor_printing_master .cls_vendor_txt_comment_sender').html(data.COMMENT_BY_PA);
                $('.cls_vendor_printing_master .cls_vendor_txt_comment').html(data.COMMENT);
                $('.cls_vendor_printing_master .cls_vendor_txt_req_date').val(moment(data.CREATE_DATE_BY_PA).add('days', 1).format('DD/MM/YYYY'));
                 //------------------------------------------by aof #INC-11265------------------------------------------------------------
                setValueToDDL('.cls_vendor_printing_master .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);   
                 //------------------------------------------by aof #INC-11265------------------------------------------------------------
                
            }
        },
    });

    table_vendor_log_vn_pm.on('order.dt search.dt', function () {
        table_vendor_log_vn_pm.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_vendor_log_vn_sl;
function bind_vendor_vn_sl() {
    table_vendor_log_vn_sl = $('#table_vendor_log_vn_sl').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtovendor?data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.step_artwork_code=SEND_VN_SL&data.send_to_vendor_type=REQ_SHADE_LIM',
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
        "order": [[5, 'desc']],
        "vn_slocessing": true,
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
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "REASON_BY_PA", "className": "cls_nowrap" },
            { "data": "REMARK_REASON_BY_PA", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },

        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(8).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(5).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_vendor_request_shade_limit .cls_vendor_txt_comment_sender').html(data.COMMENT_BY_PA);
                $('.cls_vendor_request_shade_limit .cls_vendor_txt_comment').html(data.COMMENT);
                $('.cls_vendor_request_shade_limit .cls_vendor_txt_req_date').val(moment(data.CREATE_DATE_BY_PA).add('days', 1).format('DD/MM/YYYY'));
                //------------------------------------------by aof #INC-11265------------------------------------------------------------
                setValueToDDL('.cls_vendor_request_shade_limit .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
                //------------------------------------------by aof #INC-11265------------------------------------------------------------

            }
        },
    });

    table_vendor_log_vn_sl.on('order.dt search.dt', function () {
        table_vendor_log_vn_sl.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_vendor_log_vn_po;
function bind_vendor_vn_po() {
    table_vendor_log_vn_po = $('#table_vendor_log_vn_po').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pp/vendor/info?data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.step_artwork_code=SEND_VN_PO',
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
        "order": [[5, 'desc']],
        "vn_poocessing": true,
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
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "REASON_BY_PP", "className": "cls_nowrap" },
            { "data": "REMARK_REASON_BY_PP", "className": "cls_nowrap" },
            { "data": "COMMENT_BY_PP", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PP", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "cls_nowrap" },
            { "data": "CONFIRM_PO", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },

        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(5).html(myDateTimeMoment(data.CREATE_DATE_BY_PP));

            if (data.CONFIRM_PO == 'X')
                $(row).find('td').eq(6).html('Yes');
            else
                $(row).find('td').eq(6).html('');

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_send_po_vendor .cls_vendor_txt_comment_sender').html(data.COMMENT_BY_PP);
                $('.cls_send_po_vendor .cls_vendor_txt_req_date').val(moment(data.CREATE_DATE_BY_PP).add('days', 1).format('DD/MM/YYYY'));

                
                if (data.REQUEST_SHADE_LIMIT_REFERENCE == 'Yes') // if (data.REQUEST_SHADE_LIMIT_REFERENCE_DISPLAY_TXT == 'Yes') / ticket#19383 by aof
                    $('.cls_send_po_vendor input:radio[name=rdo_request_for_ref]').filter('[value=1]').prop('checked', true);
                else
                    $('.cls_send_po_vendor input:radio[name=rdo_request_for_ref]').filter('[value=2]').prop('checked', true); // ticket#19383 by aof change form value=0 to value=2

                // ticket#19383 by aof
                if (data.CONFIRM_PO == 'X')
                    $('.cls_send_po_vendor .cls_chk_confirm').prop('checked', true);
                else
                    $('.cls_send_po_vendor .cls_chk_confirm').prop('checked', false);
                 // ticket#19383 by aof

                //------------------------------------------by aof #INC-11265------------------------------------------------------------
                setValueToDDL('.cls_send_po_vendor .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
                //------------------------------------------by aof #INC-11265------------------------------------------------------------


            }
        },
    });

    table_vendor_log_vn_po.on('order.dt search.dt', function () {
        table_vendor_log_vn_po.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}
