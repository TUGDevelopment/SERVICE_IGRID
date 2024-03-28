var MOCKUPSUBVNID = 0;
var fmvendor = '';
var STEPMOCKUPCODE = '';
$(document).ready(function () {
    //bind_pack_detail(MOCKUPSUBID);
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_vendor':
                load_tab_vendor();
                break;
            default:
                break;
        }
    });
    var pg_submit_modal_vendor = "#pg_submit_modal_vendor ";


    $(pg_submit_modal_vendor + '.cls_chk_send_pr').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal_vendor + '.cls_body_send_pr').show();
            $(pg_submit_modal_vendor + '.cls_body_send_rs').hide();
        }
        else {
            $(pg_submit_modal_vendor + '.cls_body_send_pr').hide();
        }
    });
    $(pg_submit_modal_vendor + '.cls_chk_send_rs').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal_vendor + '.cls_body_send_rs').show();
            $(pg_submit_modal_vendor + '.cls_body_send_pr').hide();
        }
        else {
            $(pg_submit_modal_vendor + '.cls_body_send_rs').hide();
        }
    });

    $("#pg_submit_modal_vendor .cls_btn_submit_send_vendor").click(function (e) {

        var error = false;
        var errorPR = false;
        var errorSample = false;
        var errorMatchboard = false;
        var errorDieline = false;
        var errorReason = false;

        var strH = '<span style="font-weight:bold"> Please input data in PG tab.</span><br/><span>The following field is required.</span>';
        var strPR = '<br/>&nbsp;&nbsp;&nbsp;Send primary and request sample';
        var strSample = '<br/>&nbsp;&nbsp;&nbsp;Request for sample';
        var strMatchboard = '<br/>&nbsp;&nbsp;&nbsp;Request for match board';
        var strDieline = '<br/>&nbsp;&nbsp;&nbsp;Request for die-line';

        $(".cls_body_send_pr :input:visible").each(function () {
            if ($(this).prop('required')) {
                if (isEmpty($(this).val())) {
                    error = true; errorPR = true;
                    strPR += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + $.trim($(this).closest('div').prev().text());
                }
            }

            if ($(this).hasClass('cls_lov_send_for_reason')) {
                if ($(".cls_body_send_pr .cls_lov_send_for_reason option:selected").val() == DefaultResonId) {
                    error = true; errorPR = true;
                    strPR += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + $.trim($(this).closest('div').prev().text());
                }
            }
        });

        $(".cls_body_send_rs :input:visible").each(function () {
            if ($(this).prop('required')) {
                if (isEmpty($(this).val())) {
                    error = true; errorSample = true;
                    strSample += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + $.trim($(this).closest('div').prev().text());
                }
            }

            if ($(this).hasClass('cls_lov_send_for_reason')) {
                if ($(".cls_body_send_rs .cls_lov_send_for_reason option:selected").val() == DefaultResonId) {
                    error = true; errorSample = true;
                    strSample += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + $.trim($(this).closest('div').prev().text());
                }
            }
        });

        $(".cls_body_send_mb :input:visible").each(function () {
            if ($(this).hasClass('cls_lov_send_for_reason')) {
                if ($(".cls_body_send_mb .cls_lov_send_for_reason option:selected").val() == DefaultResonId) {
                    error = true; errorMatchboard = true;
                    strMatchboard += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + $.trim($(this).closest('div').prev().text());
                }
            }
        });

        $(".cls_body_send_dl :input:visible").each(function () {
            if ($(this).prop('required')) {
                if (isEmpty($(this).val())) {
                    error = true; errorDieline = true;
                    strDieline += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + $.trim($(this).closest('div').prev().text());
                }
            }

            if ($(this).hasClass('cls_lov_send_for_reason')) {
                if ($(".cls_body_send_dl .cls_lov_send_for_reason option:selected").val() == DefaultResonId) {
                    error = true; errorDieline = true;
                    strDieline += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + $.trim($(this).closest('div').prev().text());
                }
            }
        });

        if ($('.cls_input_send_rs_other').is(':visible') && $('.cls_input_send_rs_other').val() == '') {
            error = true; errorReason = true;
            alertError2("Please fill remark reason");
        }
        if ($('.cls_input_send_pr_other').is(':visible') && $('.cls_input_send_pr_other').val() == '') {
            error = true; errorReason = true;
            alertError2("Please fill remark reason");
        }
        if ($('.cls_input_send_mb_other').is(':visible') && $('.cls_input_send_mb_other').val() == '') {
            error = true; errorReason = true;
            alertError2("Please fill remark reason");
        }
        if ($('.cls_input_send_dl_other').is(':visible') && $('.cls_input_send_dl_other').val() == '') {
            error = true; errorReason = true;
            alertError2("Please fill remark reason");
        }

        if (!error) {
            SubmitDataVendorPop();
        }
        else {
            var allStr = strH;
            if (errorPR) allStr += strPR;
            if (errorSample) allStr += strSample;
            if (errorMatchboard) allStr += strMatchboard;
            if (errorDieline) allStr += strDieline;

            if (!errorReason)
                alertError(allStr);
        }
    });

    $(".cls_btn_submit_vendor").click(function () {
        vendor_send_sumbmit('SUBMIT', true);
    });
    $(".cls_btn_send_back_vendor").click(function () {
        vendor_send_sumbmit('SEND_BACK', true);
    });

    //$(".cls_btn_upload_file_vendor").click(function () {
    //    var $form = null;
    //    $(function () {
    //        $form = $('#fileupload').fileupload({
    //            dataType: 'json',
    //            formData: { mockupSubId: MOCKUPSUBID, userId: UserID },
    //            url: suburl + '/FileUpload/Upload',
    //        });
    //    });
    //});

});

function load_tab_vendor() {
    if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_PR') {
        bind_detail_vn();
        if (table_vendor_log_pr == null)
            bind_vendor_pr();
        else
            table_vendor_log_pr.ajax.reload();
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_RS') {
        bind_detail_vn();
        if (table_vendor_log_rs == null)
            bind_vendor_rs();
        else
            table_vendor_log_rs.ajax.reload();
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_MB') {
        bind_detail_vn();
        if (table_vendor_log_mb == null)
            bind_vendor_mb();
        else
            table_vendor_log_mb.ajax.reload();
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_DL') {
        bind_detail_vn();
        if (table_vendor_log_dl == null)
            bind_vendor_dl();
        else
            table_vendor_log_dl.ajax.reload();
    }
    else {
        bind_pack_detail();
        if (table_vendor_log_pr == null)
            bind_vendor_pr();
        else
            table_vendor_log_pr.ajax.reload();
        if (table_vendor_log_rs == null)
            bind_vendor_rs();
        else
            table_vendor_log_rs.ajax.reload();
        if (table_vendor_log_mb == null)
            bind_vendor_mb();
        else
            table_vendor_log_mb.ajax.reload();
        if (table_vendor_log_dl == null)
            bind_vendor_dl();
        else
            table_vendor_log_dl.ajax.reload();
    }
}

function bind_detail_vn() {

    var myurl = '/api/taskform/pg/sendtovendor?data.mockup_sub_id=' + MOCKUPSUBID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_pack_detail);
}

function bind_pack_detail() {

    var myurl = '/api/taskform/pg/sendtovendor?data.mockup_id=' + MOCKUPID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_pack_detail);
}

function callback_bind_pack_detail(res) {

    if (res.data.length > 0) {
        for (var i = 0; i < res.data.length; i++) {
            var v = res.data[i];
            if (v.SEND_TO_VENDOR_TYPE == 'REQ_PRI') {
                fmvendor = '.cls_pr_vendor';
                $('.cls_pr_vendor').show();
                $('.cls_pr_vendor .cls_vn_amount').val(v.SAMPLE_AMOUNT_FOR_SEND_PRIMARY);
            }
            else if (v.SEND_TO_VENDOR_TYPE == 'REQ_SAM') {
                fmvendor = '.cls_rs_vendor';
                $('.cls_rs_vendor').show();
                $('.cls_rs_vendor .cls_vn_amount').val(v.SAMPLE_AMOUNT_FOR_REQ_SAMPLE_DIELINE);
            }
            else if (v.SEND_TO_VENDOR_TYPE == 'REQ_MATCH') {
                fmvendor = '.cls_mb_vendor';
                $('.cls_mb_vendor').show();
            }
            else if (v.SEND_TO_VENDOR_TYPE == 'REQ_DIELINE') {
                fmvendor = '.cls_dl_vendor';
                $('.cls_dl_vendor').show();
                $('.cls_dl_vendor .cls_vn_amount').val(v.SAMPLE_AMOUNT_FOR_REQ_SAMPLE_DIELINE);
            }


            //------------------------------------------by aof #INC-11265------------------------------------------------------------
            $(fmvendor + ' .cls_vendor_txt_comment').val(v.PROCESS_VENDOR.COMMENT);     
            setValueToDDL(fmvendor + ' .cls_lov_send_for_reason', v.PROCESS_VENDOR.REASON_ID, v.PROCESS_VENDOR.REASON_BY_OTHER);
           //------------------------------------------by aof #INC-11265------------------------------------------------------------
        }
    }
}

//submit in pop up
function SubmitDataVendorPop() {

    var pg_submit_modal_vendor = "#pg_submit_modal_vendor ";
    var jsonObj = new Object();
    jsonObj.data = [];
    var data = [];
    for (var i = 1; i < 5; i++) {
        var item = {};
        var step_mockup_id = 0;
        var curr_role_id = 0;
        var cls_txtedt = "";
        var cls_chk = "";
        var remarkReasonObj;
        switch (i) {

            case 1:
                cls_txtedt = ".cls_txt_send_pr";
                cls_chk = ".cls_chk_send_pr";
                step_mockup_id = getstepmockup('SEND_VN_PR').curr_step;
                curr_role_id = getstepmockup('SEND_VN_PR').curr_role;
                remarkReasonObj = ".cls_body_send_pr .cls_input_send_pr_other";
                break;
            case 2:
                cls_txtedt = ".cls_txt_send_rs";
                cls_chk = ".cls_chk_send_rs";
                step_mockup_id = getstepmockup('SEND_VN_RS').curr_step;
                curr_role_id = getstepmockup('SEND_VN_RS').curr_role;
                remarkReasonObj = ".cls_body_send_rs .cls_input_send_rs_other";
                break;
            case 3:
                cls_txtedt = ".cls_txt_send_mb";
                cls_chk = ".cls_chk_send_mb";
                step_mockup_id = getstepmockup('SEND_VN_MB').curr_step;
                curr_role_id = getstepmockup('SEND_VN_MB').curr_role;
                remarkReasonObj = ".cls_body_send_mb .cls_input_send_mb_other";
                break;
            case 4:
                cls_txtedt = ".cls_txt_send_dl";
                cls_chk = ".cls_chk_send_dl";
                step_mockup_id = getstepmockup('SEND_VN_DL').curr_step;
                curr_role_id = getstepmockup('SEND_VN_DL').curr_role;
                remarkReasonObj = ".cls_body_send_dl .cls_input_send_dl_other";

                break;
            default:
                break;
        }
        if ($(pg_submit_modal_vendor + cls_chk).is(":checked")) {
            item["MOCKUP_ID"] = MOCKUPID;
            item["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item["CURRENT_STEP_ID"] = step_mockup_id;
            item["CURRENT_ROLE_ID"] = curr_role_id;
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;
            item["WF_SUB_ID"] = MOCKUPSUBID;
            item["REMARK_REASON"] = $(remarkReasonObj).val();
            item["IS_SENDER"] = true;
            var editor = new Quill(pg_submit_modal_vendor + cls_txtedt);
            item["REMARK"] = editor.root.innerHTML;
            data.push(item);
        }
    }
    if ($('.cls_header_chk_vendor[type="checkbox"]:checked').length > 0) {
        var jsonObj = new Object();
        jsonObj.data = [];

        if ($(pg_submit_modal_vendor + '.cls_chk_send_pr').is(":checked")) {
            var item = {};
            item.PROCESS = {};
            item["SEND_TO_VENDOR_TYPE"] = "REQ_PRI";
            item["MOCKUP_ID"] = MOCKUPID;
            item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item["SAMPLE_AMOUNT_FOR_SEND_PRIMARY"] = $(pg_submit_modal_vendor + '.cls_body_send_pr .cls_vn_amount').val();
            //item["WF_SUB_ID"] = MOCKUPSUBID;  //rewrited by aof #INC-11265
            item["REASON_ID"] = $(pg_submit_modal_vendor + '.cls_body_send_pr .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pg_submit_modal_vendor + '.cls_body_send_pr .cls_input_send_pr_other').val();
            item["IS_SENDER"] = true;
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["MOCKUP_ID"] = MOCKUPID;
            item.PROCESS["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item.PROCESS["CURRENT_STEP_ID"] = getstepmockup('SEND_VN_PR').curr_step;
            item.PROCESS["REASON_ID"] = $(pg_submit_modal_vendor + '.cls_body_send_pr .cls_lov_send_for_reason').val();
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill('.cls_body_send_pr .cls_txt_send_pr');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data.push(item);
        }
        if ($(pg_submit_modal_vendor + '.cls_chk_send_rs').is(":checked")) {
            var item = {};
            item.PROCESS = {};

            item["SEND_TO_VENDOR_TYPE"] = "REQ_SAM";
            item["MOCKUP_ID"] = MOCKUPID;
            item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item["SAMPLE_AMOUNT_FOR_REQ_SAMPLE_DIELINE"] = $(pg_submit_modal_vendor + '.cls_body_send_rs .cls_vn_amount').val();
            //item["WF_SUB_ID"] = MOCKUPSUBID;  //rewrited by aof #INC-11265
            item["REASON_ID"] = $(pg_submit_modal_vendor + '.cls_body_send_rs .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pg_submit_modal_vendor + '.cls_body_send_rs .cls_input_send_rs_other').val();
            item["IS_SENDER"] = true;
            item["WF_SUB_ID"] = MOCKUPSUBID;
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["MOCKUP_ID"] = MOCKUPID;
            item.PROCESS["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item.PROCESS["CURRENT_STEP_ID"] = getstepmockup('SEND_VN_RS').curr_step;
            item.PROCESS["REASON_ID"] = $(pg_submit_modal_vendor + '.cls_body_send_rs .cls_lov_send_for_reason').val();
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill('.cls_body_send_rs .cls_txt_send_rs');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data.push(item);
        }
        if ($(pg_submit_modal_vendor + '.cls_chk_send_mb').is(":checked")) {
            var item = {};
            item.PROCESS = {};
            item["SEND_TO_VENDOR_TYPE"] = "REQ_MATCH";
            item["MOCKUP_ID"] = MOCKUPID;
            item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
            //item["WF_SUB_ID"] = MOCKUPSUBID;  //rewrited by aof #INC-11265
            item["REASON_ID"] = $(pg_submit_modal_vendor + '.cls_body_send_mb .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pg_submit_modal_vendor + '.cls_body_send_mb .cls_input_send_mb_other').val();
            item["IS_SENDER"] = true;
            item["WF_SUB_ID"] = MOCKUPSUBID;
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["MOCKUP_ID"] = MOCKUPID;
            item.PROCESS["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item.PROCESS["CURRENT_STEP_ID"] = getstepmockup('SEND_VN_MB').curr_step;
            item.PROCESS["REASON_ID"] = $(pg_submit_modal_vendor + '.cls_body_send_mb .cls_lov_send_for_reason').val();
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill('.cls_body_send_mb .cls_txt_send_mb');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data.push(item);
        }
        if ($(pg_submit_modal_vendor + '.cls_chk_send_dl').is(":checked")) {
            var item = {};
            item.PROCESS = {};
            item["SEND_TO_VENDOR_TYPE"] = "REQ_DIELINE";
            item["MOCKUP_ID"] = MOCKUPID;
            item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
            //item["SAMPLE_AMOUNT_FOR_REQ_SAMPLE_DIELINE"] = $(pg_submit_modal_vendor + '.cls_body_send_dl .cls_vn_amount').val()
            // item["WF_SUB_ID"] = MOCKUPSUBID;  //rewrited by aof #INC-11265
            item["REASON_ID"] = $(pg_submit_modal_vendor + '.cls_body_send_dl .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pg_submit_modal_vendor + '.cls_body_send_dl .cls_input_send_dl_other').val();
            item["IS_SENDER"] = true;
            item["WF_SUB_ID"] = MOCKUPSUBID;
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["MOCKUP_ID"] = MOCKUPID;
            item.PROCESS["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item.PROCESS["CURRENT_STEP_ID"] = getstepmockup('SEND_VN_DL').curr_step;
            item.PROCESS["REASON_ID"] = $(pg_submit_modal_vendor + '.cls_body_send_dl .cls_lov_send_for_reason').val();
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill('.cls_body_send_dl .cls_txt_send_dl');
            item.PROCESS["REMARK"] = editor.root.innerHTML;
            jsonObj.data.push(item);
        }

        var myurl = '/api/taskform/pg/sendtovendor';
        var mytype = 'POST';
        var mydata = jsonObj;

        if (data.length > 0)
            myAjaxConfirmSubmit(myurl, mytype, mydata, hide_modalto_vendor, '', true, true, true);
        else
            myAjax(myurl, mytype, mydata);


    }
    else {
        $(".se-pre-con").fadeOut('fast');
        alertError2("Please select at least 1 item.");
    }
}
function hide_modalto_vendor() {

    var pg_submit_modal_vendor = "#pg_submit_modal_vendor ";
    resetDllReason(pg_submit_modal_vendor + ' .cls_lov_send_for_reason');
    $(pg_submit_modal_vendor + ' .cls_lov_search_file_template').val('').trigger('change');

    var text_editor_vn_send_dl = new Quill(pg_submit_modal_vendor + '.cls_txt_send_dl');
    var text_editor_vn_send_mb = new Quill(pg_submit_modal_vendor + '.cls_txt_send_mb');
    var text_editor_vn_send_pr = new Quill(pg_submit_modal_vendor + '.cls_txt_send_pr');
    var text_editor_vn_send_rs = new Quill(pg_submit_modal_vendor + '.cls_txt_send_rs');
    text_editor_vn_send_pr.setContents([{ insert: '\n' }]);
    text_editor_vn_send_rs.setContents([{ insert: '\n' }]);
    text_editor_vn_send_dl.setContents([{ insert: '\n' }]);
    text_editor_vn_send_mb.setContents([{ insert: '\n' }]);

    $(pg_submit_modal_vendor + ' input:checkbox').prop('checked', false);
    $(pg_submit_modal_vendor + ' input:text:enabled').val('');
    $(pg_submit_modal_vendor + '.cls_body_send_dl').hide();
    $(pg_submit_modal_vendor + '.cls_body_send_mb').hide();
    $(pg_submit_modal_vendor + '.cls_body_send_pr').hide();
    $(pg_submit_modal_vendor + '.cls_body_send_rs').hide();

    $('#pg_submit_modal_vendor').modal('hide');
}
function callbackVNPopSubmit(res) {
    var pg_submit_modal_vendor = "#pg_submit_modal_vendor ";
    $(pg_submit_modal_vendor).modal('hide');
}

function selectOnlyThis(id) {
    for (var i = 1; i <= 2; i++) {
        if ("vn_rdo_rs" + i === id && document.getElementById("vn_rdo_rs" + i).checked === true) {
            document.getElementById("vn_rdo_rs" + i).checked = true;
        } else {
            document.getElementById("vn_rdo_rs" + i).checked = false;
        }
    }
}


//commented by aof #INC-11265 
//function vendor_send_sumbmit(ACTION_CODE, EndTaskForm) {
//    var jsonObj = new Object();
//    jsonObj.data = {};
//    jsonObj.data["MOCKUP_ID"] = MOCKUPID;
//    jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
//    jsonObj.data["ENDTASKFORM"] = EndTaskForm;
//    jsonObj.data["ACTION_CODE"] = ACTION_CODE;
//    jsonObj.data["COMMENT"] = $(fmvendor + ' .cls_vendor_txt_comment').val();
//    jsonObj.data["CREATE_BY"] = UserID;
//    jsonObj.data["UPDATE_BY"] = UserID;
//    jsonObj.data["REASON_ID"] = $(fmvendor + ' .cls_lov_send_for_reason').val();
//    jsonObj.data["STEP_MOCKUP_CODE"] = CURRENT_STEP_CODE_DISPLAY_TXT;

//    if (jsonObj.data["REASON_ID"] == DefaultResonId && OverDue == 1 && EndTaskForm) {
//        alertError2("Please select reason for overdue");
//        return false;
//    }
//    if (jsonObj.data["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
//        alertError2("Please select reason for send back");
//        return false;
//    }

//    var myurl = '/api/taskform/vendor/info';
//    var mytype = 'POST';
//    var mydata = jsonObj;
//    myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
//}

//rewrited by aof #INC-11265 
function vendor_send_sumbmit(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    jsonObj.data = [];


    item["MOCKUP_ID"] = MOCKUPID;
    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["ENDTASKFORM"] = EndTaskForm;
    item["ACTION_CODE"] = ACTION_CODE;
    item["COMMENT"] = $(fmvendor + ' .cls_vendor_txt_comment').val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;
    item["REASON_ID"] = $(fmvendor + ' .cls_lov_send_for_reason').val();
    item["STEP_MOCKUP_CODE"] = CURRENT_STEP_CODE_DISPLAY_TXT;

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1 && EndTaskForm) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    //-------------------------- appended by aof #INC-11265 ------------------------------------
    item["WF_SUB_ID"] = MOCKUPSUBID;
    item["REMARK_REASON"] = $(fmvendor + ' .cls_input_other').val();
    item["WF_STEP"] = getstepmockup(CURRENT_STEP_CODE_DISPLAY_TXT).curr_step;
    //-------------------------- appended by aof #INC-11265 ------------------------------------

    jsonObj.data.push(item);

    var myurl = '/api/taskform/vendor/info';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
}

var table_vendor_log_pr;
function bind_vendor_pr() {
    table_vendor_log_pr = $('#table_vendor_log_pr').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/vendor/info?data.mockup_id=' + MOCKUPID + '&data.step_mockup_code=SEND_VN_PR&data.send_to_vendor_type=REQ_PRI',
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
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "" },

        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(5).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));
        },
    });

    table_vendor_log_pr.on('order.dt search.dt', function () {
        table_vendor_log_pr.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_vendor_log_rs;
function bind_vendor_rs() {
    table_vendor_log_rs = $('#table_vendor_log_rs').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/vendor/info?data.mockup_id=' + MOCKUPID + '&data.step_mockup_code=SEND_VN_RS&data.send_to_vendor_type=REQ_SAM',
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
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(5).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));
        },
    });

    table_vendor_log_rs.on('order.dt search.dt', function () {
        table_vendor_log_rs.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_vendor_log_mb;
function bind_vendor_mb() {
    table_vendor_log_mb = $('#table_vendor_log_mb').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/vendor/info?data.mockup_id=' + MOCKUPID + '&data.step_mockup_code=SEND_VN_MB&data.send_to_vendor_type=REQ_MATCH',
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
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "" },
            //{ "data": "CREATE_BY_DISPLAY_TXT", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(5).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));
        },
    });

    table_vendor_log_mb.on('order.dt search.dt', function () {
        table_vendor_log_mb.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_vendor_log_dl;
function bind_vendor_dl() {
    table_vendor_log_dl = $('#table_vendor_log_dl').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/vendor/info?data.mockup_id=' + MOCKUPID + '&data.step_mockup_code=SEND_VN_DL&data.send_to_vendor_type=REQ_DIELINE',
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
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE));

            $(row).find('td').eq(5).html(myDateTimeMoment(data.CREATE_DATE_BY_PG));
        },
    });

    table_vendor_log_dl.on('order.dt search.dt', function () {
        table_vendor_log_dl.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}