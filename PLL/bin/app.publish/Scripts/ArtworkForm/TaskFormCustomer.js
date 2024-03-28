
var MOCKUPSUBVNID = 0;
var fmcustomer = '';
var STEPMOCKUPCODE = '';
var isRepeat = false;
$(document).ready(function () {
    $('.cls_art_cus_print form').validate({
        rules: {
            rdoShade_cus:
            {
                required: true
            },
            rdoChange_cus_11:
            {
                required: true
            },
            txt_cus_pm:
            {
                required: {
                    depends: function (element) {
                        if ($(".cls_art_cus_print input[name=rdoChange_cus_11][value=1]").is(":checked"))
                            return true;
                        else if ($(".cls_art_cus_print input[name=rdoChange_cus_11][value=2]").is(":checked"))
                            return true;
                    }
                }
            },

        },
        messages: {
            rdoShade_cus:
            {
                required: "Please select type of request shade limit"
            },
            rdoChange_cus_11:
            {
                required: "Please select decision"
            },
            txt_cus_pm:
            {
                required: function (element) {
                    if ($(".cls_art_cus_print input[name=rdoChange_cus_11][value=1]").is(":checked"))
                        return "Please fill comment about revise";
                    else if ($(".cls_art_cus_print input[name=rdoChange_cus_11][value=2]").is(":checked"))
                        return "Please fill comment about cancel";
                }
            }

        }
    });

    $('.cls_art_cus_shade form').validate({
        rules: {
            rdoChange_cus_11:
            {
                required: true
            },
            txt_cus_sl:
            {
                required: {
                    depends: function (element) {
                        if ($(".cls_art_cus_shade input[name=rdoChange_cus_11][value=1]").is(":checked"))
                            return true;
                        else if ($(".cls_art_cus_shade input[name=rdoChange_cus_11][value=2]").is(":checked"))
                            return true;
                    }
                }
            }
        },
        messages: {
            rdoChange_cus_11:
            {
                required: "Please select decision"
            },
            txt_cus_sl:
            {
                required: function (element) {
                    if ($(".cls_art_cus_shade input[name=rdoChange_cus_11][value=1]").is(":checked"))
                        return "Please fill comment about revise";
                    else if ($(".cls_art_cus_shade input[name=rdoChange_cus_11][value=2]").is(":checked"))
                        return "Please fill comment about cancel";
                }
            }
        }
    });

    $('.cls_art_cus_review form').validate({
        rules: {
            rdoChange_cus_11:
            {
                required: true
            },
            rdoChange_cus_12:
            {
                required: true
            },
            rdoChange_cus_2:
            {
                required: true
            },
        },
        messages: {
            rdoChange_cus_11:
            {
                required: "Please choose at least 1 decision."
            },
            rdoChange_cus_12:
            {
                required: "Please choose at least 1 decision."
            },
            rdoChange_cus_2:
            {
                required: "Please choose at least 1 decision."
            },
        }
    });

    //bind_detail(MOCKUPSUBID);
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_customer':
                //bindDataTaskFormCustomerReview();
                load_tab_customer_artwork();
                break;
            default:
                break;
        }
    });
    $('.cls_art_cus_print .cls_lov_revise_reason').prop("disabled", true);
    $('.cls_art_cus_print .cls_lov_cancel_reason').prop("disabled", true);


    $(".cls_art_cus_print input[name=rdoChange_cus_11]").click(function () {
        //alert($(".cls_art_cus_print input[name=rdoChange_cus_11]").val());
        if ($(".cls_art_cus_print .cls_approve_shade_radio").prop('checked')) {
            if (!isRepeat)
                $(".cls_art_cus_print .cls_shade_print").show();
        }
        else
            $(".cls_art_cus_print .cls_shade_print").hide();

    });

    $(".cls_art_cus_review form").submit(function (e) {
        if ($(this).valid()) {
            customer_send_sumbmit('SUBMIT', true);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });
    $(".cls_art_cus_review .cls_cus_btn_send_back").click(function () {
        customer_send_sumbmit('SEND_BACK', true);
    });

    $(".cls_art_cus_print form").submit(function (e) {
        if ($(this).valid()) {
            customer_send_sumbmit('SUBMIT', true);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_art_cus_print .cls_cus_btn_send_back").click(function () {
        customer_send_sumbmit('SEND_BACK', true);
    });

    $(".cls_art_cus_shade form").submit(function (e) {
        if ($(this).valid()) {
            customer_send_sumbmit('SUBMIT', true);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });
    //$(".cls_art_cus_shade .cls_cus_btn_submit").click(function () {
    //    customer_send_sumbmit('SUBMIT', true);
    //});
    $(".cls_art_cus_shade .cls_cus_btn_send_back").click(function () {
        customer_send_sumbmit('SEND_BACK', true);
    });

    $(".cls_art_cus_ref .cls_cus_btn_submit").click(function () {
        customer_send_sumbmit('SUBMIT', true);
    });
    $(".cls_art_cus_ref .cls_cus_btn_send_back").click(function () {
        customer_send_sumbmit('SEND_BACK', true);
    });

    $(".cls_art_cus_req_ref .cls_cus_btn_submit").click(function () {
        customer_send_sumbmit('SUBMIT', true);
    });
    $(".cls_art_cus_req_ref .cls_cus_btn_send_back").click(function () {
        customer_send_sumbmit('SEND_BACK', true);
    });


    $(".cls_art_cus_print .cls_chk_agree").click(function () {
        debugger;
        if ($(this).prop('checked')) {
            $('.cls_art_cus_print .cls_cus_btn_send_back').prop("disabled", false);
            $('.cls_art_cus_print .cls_cus_btn_submit').prop("disabled", false);
            $('.cls_art_cus_print .cls_lov_revise_reason').prop("disabled", false);
            $('.cls_art_cus_print .cls_lov_cancel_reason').prop("disabled", false);
            $('.cls_art_cus_print .cls_cus_txt_comment').prop("disabled", false);
            $('.cls_art_cus_print input[name=rdoChange_cus_11]').prop("disabled", false);
            $('.cls_art_cus_print input[name=rdoShade_cus]').prop("disabled", false);
        }
        else {
            $('.cls_art_cus_print .cls_cus_btn_send_back').prop("disabled", true);
            $('.cls_art_cus_print .cls_cus_btn_submit').prop("disabled", true);
            $('.cls_art_cus_print .cls_lov_revise_reason').prop("disabled", true);
            $('.cls_art_cus_print .cls_lov_cancel_reason').prop("disabled", true);
            $('.cls_art_cus_print .cls_cus_txt_comment').prop("disabled", true);
            $('.cls_art_cus_print input[name=rdoChange_cus_11]').prop("disabled", true);
            $('.cls_art_cus_print input[name=rdoShade_cus]').prop("disabled", true);
        }
    });




    $(".cls_btn_upload_file_customer").click(function () {
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


function bindDataTaskFormCustomerReview() {
    var myurl = '/api/taskform/pa/sendtocustomer?data.step_artwork_code=SEND_CUS_REVIEW&data.send_to_customer_type=REQ_CUS_REVIEW&data.artwork_sub_id=' + MainArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.current_user_id=' + CURRENTUSERID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_dataTaskFormCustomerReview);
}



function callback_bind_dataTaskFormCustomerReview(res) {
    if (res.data.length > 0) {
        var item = res.data[0];


        var NumDetailQC = 0;
        if (item.NUTRITION_COMMENT_DISPLAY != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_nutri_txt_comment_cus').html(item.NUTRITION_COMMENT_DISPLAY);
        }
        else
            $('.cls_art_cus_req_ref .cls_nutri_cus').hide();
        if (item.INGREDIENTS_COMMENT_DISPLAY != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_ingre_txt_comment_cus').html(item.INGREDIENTS_COMMENT_DISPLAY);
        }
        else
            $('.cls_art_cus_req_ref .cls_ingri_cus').hide();
        if (item.ANALYSIS_COMMENT_DISPLAY != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_analysis_txt_comment_cus').html(item.ANALYSIS_COMMENT_DISPLAY);
        }
        else
            $('.cls_art_cus_req_ref .cls_analysis_cus').hide();
        if (item.HEALTH_CLAIM_COMMENT_DISPLAY != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_health_txt_comment_cus').html(item.HEALTH_CLAIM_COMMENT_DISPLAY);
        }
        else
            $('.cls_art_cus_req_ref .cls_health_cus').hide();
        if (item.NUTRIENT_CLAIM_COMMENT_DISPLAY != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_nutclaim_txt_comment_cus').html(item.NUTRIENT_CLAIM_COMMENT_DISPLAY);
        }
        else
            $('.cls_art_cus_req_ref .cls_nutri_claim_cus').hide();
        if (item.SPECIES_COMMENT_DISPLAY != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_species_txt_comment_cus').html(item.SPECIES_COMMENT_DISPLAY);
        }
        else
            $('.cls_art_cus_req_ref .cls_species_cus').hide();
        if (item.CATCHING_AREA_COMMENT_DISPLAY != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_catching_txt_comment_cus').html(item.CATCHING_AREA_COMMENT_DISPLAY);
        }
        else
            $('.cls_art_cus_req_ref .cls_catch_fao_cus').hide();

        if (item.CHECK_DETAIL_COMMENT_DISPLAY != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_check_detail_txt_comment_cus').html(item.CHECK_DETAIL_COMMENT_DISPLAY);
        }
        else
            $('.cls_art_cus_req_ref .cls_check_detail_cus').hide();

        if (item.QC_COMMENT != "-") {
            NumDetailQC++;
            $('.cls_art_cus_req_ref .cls_remark_comment_qc_cus').text(item.QC_COMMENT);
        }
        else
            $('.cls_art_cus_req_ref .cls_qccomment_cus').hide();

        if (NumDetailQC == 0)
            $('.cls_art_cus_req_ref .cls_body_detailqc').hide();


        $('.cls_art_cus_req_ref .cls_cus_txt_comment_11').text(item.COMMENT_CHANGE_DETAIL);
        $('.cls_art_cus_req_ref .cls_cus_txt_comment_12').text(item.COMMENT_NONCOMPLIANCE);
        $('.cls_art_cus_req_ref .cls_cus_txt_comment_2').text(item.COMMENT_ADJUST);

        if (item.DECISION__CHANGE_DETAIL == "1")
            $(".cls_art_cus_req_ref input[name=rdoChange_cus_11][value=1]").prop('checked', true);
        else
            $(".cls_art_cus_req_ref input[name=rdoChange_cus_11][value=0]").prop('checked', true);

        if (item.DECISION__NONCOMPLIANCE == "1")
            $(".cls_art_cus_req_ref input[name=rdoChange_cus_12][value=1]").prop('checked', true);
        else
            $(".cls_art_cus_req_ref input[name=rdoChange_cus_12][value=0]").prop('checked', true);

        if (item.DECISION__ADJUST == "1")
            $(".cls_art_cus_req_ref input[name=rdoChange_cus_2][value=1]").prop('checked', true);
        else
            $(".cls_art_cus_req_ref input[name=rdoChange_cus_2][value=0]").prop('checked', true);

    }
}

function load_tab_customer_artwork() {
    if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REVIEW') {
        $('.cls_art_cus_review').show();
        if (table_customer_log_review == null)
            bind_customer_review();
        else
            table_customer_log_review.ajax.reload();
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_PRINT') {
        $('.cls_art_cus_print').show();
        if (table_customer_log_print == null)
            bind_customer_print();
        else
            table_customer_log_print.ajax.reload();
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_SHADE') {
        $('.cls_art_cus_shade').show();
        if (table_customer_log_shade == null)
            bind_customer_shade();
        else
            table_customer_log_shadeajax.reload();
    }
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REF') {
        $('.cls_art_cus_ref').show();
        if (table_customer_log_ref == null)
            bind_customer_ref();
        else
            table_customer_log_ref.ajax.reload();
    }

    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REQ_REF') {
        $('.cls_art_cus_req_ref').show();
        if (table_customer_log_req_ref == null)
            bind_customer_req_ref();
        else
            table_customer_log_req_ref.ajax.reload();
    }
    else {
        $('.cls_art_cus_review').show();
        $('.cls_art_cus_print').show();
        $('.cls_art_cus_shade').show();
        //$('.cls_art_cus_ref').show();
        $('.cls_art_cus_req_ref').show();
        $('.cls_div_only_cus').hide();

        if (table_customer_log_review == null)
            bind_customer_review();
        else
            table_customer_log_review.ajax.reload();
        if (table_customer_log_print == null)
            bind_customer_print();
        else
            table_customer_log_print.ajax.reload();
        if (table_customer_log_shade == null)
            bind_customer_shade();
        else
            table_customer_log_shade.ajax.reload();

        if (table_customer_log_req_ref == null)
            bind_customer_req_ref();
        else
            table_customer_log_req_ref.ajax.reload();

        //$('.cls_div_only_cus').hide();
    }
}

function customer_send_sumbmit(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    jsonObj.data = {};
    jsonObj.data["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    jsonObj.data["ARTWORK_SUB_ID"] = ArtworkSubId;
    jsonObj.data["ENDTASKFORM"] = EndTaskForm;

    if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REVIEW') {
        jsonObj.data["DECISION__CHANGE_DETAIL"] = $('.cls_art_cus_review input[name=rdoChange_cus_11]:radio:checked').val();
        jsonObj.data["COMMENT_CHANGE_DETAIL"] = $('.cls_art_cus_review .cls_cus_txt_comment_11').val();
        jsonObj.data["DECISION__NONCOMPLIANCE"] = $('.cls_art_cus_review input[name=rdoChange_cus_12]:radio:checked').val();
        jsonObj.data["COMMENT_NONCOMPLIANCE"] = $('.cls_art_cus_review .cls_cus_txt_comment_12').val();
        jsonObj.data["DECISION__ADJUST"] = $('.cls_art_cus_review input[name=rdoChange_cus_2]:radio:checked').val();
        jsonObj.data["COMMENT_ADJUST"] = $('.cls_art_cus_review .cls_cus_txt_comment_2').val();
        jsonObj.data["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REVIEW";
    }

    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_PRINT') {
        var DECISION_ACTION_PRINT = $('.cls_art_cus_print input[name=rdoChange_cus_11]:radio:checked').val();

        jsonObj.data["DECISION_ACTION"] = DECISION_ACTION_PRINT;
        if (DECISION_ACTION_PRINT == 1) {
            jsonObj.data["REASON_ID"] = $('.cls_art_cus_print .cls_lov_revise_reason').val();
            jsonObj.data["REMARK_REASON"] = $(".cls_art_cus_print .cls_input_cus_print_revise_other").val();
        }
        else if (DECISION_ACTION_PRINT == 2) {
            jsonObj.data["REASON_ID"] = $('.cls_art_cus_print .cls_lov_cancel_reason').val();
            jsonObj.data["REMARK_REASON"] = $(".cls_art_cus_print .cls_input_cus_print_cancel_other").val();
        }

        jsonObj.data["APPROVE_SHADE_LIMIT"] = $('.cls_art_cus_print input[name=rdoShade_cus]:radio:checked').val();
        jsonObj.data["COMMENT"] = $('.cls_art_cus_print .cls_cus_txt_comment').val();
        jsonObj.data["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_PRINT";

    }

    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_SHADE') {
        var DECISION_ACTION_SHADE = $('.cls_art_cus_shade input[name=rdoChange_cus_11]:radio:checked').val();

        jsonObj.data["DECISION_ACTION"] = DECISION_ACTION_SHADE;
        if (DECISION_ACTION_SHADE == 1) {
            jsonObj.data["REASON_ID"] = $('.cls_art_cus_shade .cls_lov_revise_reason').val();
            jsonObj.data["REMARK_REASON"] = $(".cls_art_cus_shade .cls_input_cus_shade_revise_other").val();
        }
        else if (DECISION_ACTION_SHADE == 2) {
            jsonObj.data["REASON_ID"] = $('.cls_art_cus_shade .cls_lov_cancel_reason').val();
            jsonObj.data["REMARK_REASON"] = $(".cls_art_cus_shade .cls_input_cus_shade_cancel_other").val();
        }
        jsonObj.data["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_SHADE";
        jsonObj.data["COMMENT"] = $('.cls_art_cus_shade .cls_cus_txt_comment').val();
    }
    //Shade with ref
    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REF') {
        jsonObj.data["COMMENT"] = $('.cls_art_cus_ref .cls_cus_txt_comment_ref').val();
        jsonObj.data["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REF";
    }

    else if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REQ_REF') {
        jsonObj.data["COMMENT"] = $('.cls_art_cus_req_ref .cls_cus_txt_comment_req_ref').val();
        jsonObj.data["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REQ_REF";
    }

    jsonObj.data["WF_STEP"] = getstepartwork(CURRENT_STEP_CODE_DISPLAY_TXT).curr_step;
    jsonObj.data["ACTION_CODE"] = ACTION_CODE;
    jsonObj.data["CREATE_BY"] = UserID;
    jsonObj.data["UPDATE_BY"] = UserID;

    if (jsonObj.data["REASON_ID"] == DefaultResonId && OverDue == 1) {
        alertError2("Please select reason for overdue");
        return false;
    }

    var myurl = '/api/taskform/pa/customer/info';
    //var myurl = '';
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

var table_customer_log_review;
function bind_customer_review() {
    table_customer_log_review = $('#table_customer_log_review').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtocustomer?data.step_artwork_code=SEND_CUS_REVIEW&data.send_to_customer_type=REQ_CUS_REVIEW&data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.main_artwork_sub_id=' + MainArtworkSubId + '&data.current_user_id=' + CURRENTUSERID,
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
        "order": [[15, 'desc']],
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
            { "data": "CUSTOMER_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "COMMENT_FORMLABEL_DISPLAY", "className": "cls_nowrap" },
            { "data": "NUTRITION_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "INGREDIENTS_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "ANALYSIS_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "HEALTH_CLAIM_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "NUTRIENT_CLAIM_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "SPECIES_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "CATCHING_AREA_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "CHECK_DETAIL_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "QC_COMMENT", "className": "cls_nowrap" },
            { "data": "COMMENT_NONCOMPLIANCE_DISPLAY", "className": "cls_nowrap" },
            { "data": "COMMENT_ADJUST_DISPLAY", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },

            { "data": "ACTION_NAME", "className": "cls_nowrap" },
            { "data": "DECISION_FORMLABEL_DISPLAY", "className": "cls_nowrap" },
            { "data": "COMMENT_CHANGE_DETAIL", "className": "cls_nowrap" },
            { "data": "DECISION_NONCOMPLIANCE_DISPLAY", "className": "cls_nowrap" },
            { "data": "COMMENT_NONCOMPLIANCE", "className": "cls_nowrap" },
            { "data": "DECISION_ADJUST_DISPLAY", "className": "cls_nowrap" },
            { "data": "COMMENT_ADJUST", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(23).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(15).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {

                if (data.DECISION_FORMLABEL_DISPLAY == "Confirm to change")
                    $(".cls_art_cus_review input[name=rdoChange_cus_11][value=1]").prop('checked', true);
                else if (data.DECISION_FORMLABEL_DISPLAY == "Do not change")
                    $(".cls_art_cus_review input[name=rdoChange_cus_11][value=0]").prop('checked', true);

                if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Confirm to change")
                    $(".cls_art_cus_review input[name=rdoChange_cus_12][value=1]").prop('checked', true);
                else if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Do not change")
                    $(".cls_art_cus_review input[name=rdoChange_cus_12][value=0]").prop('checked', true);

                if (data.DECISION_ADJUST_DISPLAY == "Confirm to change")
                    $(".cls_art_cus_review input[name=rdoChange_cus_2][value=1]").prop('checked', true);
                else if (data.DECISION_ADJUST_DISPLAY == "Do not change")
                    $(".cls_art_cus_review input[name=rdoChange_cus_2][value=0]").prop('checked', true);

            

                if (data.NUTRITION_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_review .cls_remark_nutri_txt_comment_cus').html(data.NUTRITION_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_review .cls_nutri_cus').hide();
                if (data.INGREDIENTS_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_review .cls_remark_ingre_txt_comment_cus').html(data.INGREDIENTS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_review .cls_ingri_cus').hide();
                if (data.ANALYSIS_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_review .cls_remark_analysis_txt_comment_cus').html(data.ANALYSIS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_review .cls_analysis_cus').hide();
                if (data.HEALTH_CLAIM_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_review .cls_remark_health_txt_comment_cus').html(data.HEALTH_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_review .cls_health_cus').hide();
                if (data.NUTRIENT_CLAIM_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_review .cls_remark_nutclaim_txt_comment_cus').html(data.NUTRIENT_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_review .cls_nutri_claim_cus').hide();
                if (data.SPECIES_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_review .cls_remark_species_txt_comment_cus').html(data.SPECIES_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_review .cls_species_cus').hide();
                if (data.CATCHING_AREA_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_review .cls_remark_catching_txt_comment_cus').html(data.CATCHING_AREA_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_review .cls_catch_fao_cus').hide();

                if (data.CHECK_DETAIL_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_review .cls_remark_check_detail_txt_comment_cus').html(data.CHECK_DETAIL_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_review .cls_check_detail_cus').hide();

                if (data.QC_COMMENT != "-") {
                    $('.cls_art_cus_review .cls_remark_comment_qc_cus').html(data.QC_COMMENT);
                }
                else
                    $('.cls_art_cus_review .cls_qccomment_cus').hide();


                if (data.IS_CHANGEDETAIL == "X") {
                    $('.cls_art_cus_review .cls_body_detailqc').show();
                }
                else
                    $('.cls_art_cus_review .cls_body_detailqc').hide();

                if (data.IS_NONCOMPLIANCE == "X") {
                    $('.cls_art_cus_review .cls_customer_txt_comment_sender_12').val(data.COMMENT_NONCOMPLIANCE_DISPLAY);
                    $('.cls_art_cus_review .cls_cus_txt_comment_12').val(data.COMMENT_NONCOMPLIANCE);
                    $('.cls_art_cus_review .cls_body_noncompliance').show();
                }
                else
                    $('.cls_art_cus_review .cls_body_noncompliance').hide();

                if (data.IS_FORMLABEL == "X") {
                    $('.cls_art_cus_review .cls_customer_txt_comment_sender_1').val(data.COMMENT_FORMLABEL_DISPLAY);
                    $('.cls_art_cus_review .cls_cus_txt_comment_11').val(data.COMMENT_CHANGE_DETAIL);
                    $('.cls_art_cus_review .cls_body_form_label').show();
                }
                else
                    $('.cls_art_cus_review .cls_body_form_label').hide();

                if (data.IS_ADJUST == "X") {
                    $('.cls_art_cus_review .cls_customer_txt_comment_sender_2').val(data.COMMENT_ADJUST_DISPLAY);
                    $('.cls_art_cus_review .cls_cus_txt_comment_12').val(data.COMMENT_NONCOMPLIANCE);
                    $('.cls_art_cus_review .cls_cus_txt_comment_2').val(data.COMMENT_ADJUST);  // ticket#119383 by aof on 25/10/2021
                    $('.cls_art_cus_review .cls_body_adjust').show();
                }
                else
                    $('.cls_art_cus_review .cls_body_adjust').hide();

                //Set value for Customer tab
                $('.cls_art_cus_review .cls_cus_sold').val(data.SOLD_TO_PO);
                $('.cls_art_cus_review .cls_cus_ship').val(data.SHIP_TO_PO);
            }
        }
    });

    table_customer_log_review.on('order.dt search.dt', function () {
        table_customer_log_review.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}


var table_customer_log_print;
function bind_customer_print() {
    table_customer_log_print = $('#table_customer_log_print').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtocustomer?data.step_artwork_code=SEND_CUS_PRINT&data.send_to_customer_type=REQ_CUS_PRINT&data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.main_artwork_sub_id=' + MainArtworkSubId + '&data.current_user_id=' + CURRENTUSERID,
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
            { "data": "CUSTOMER_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "COMMENT_BY_PA", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "cls_nowrap" },
            { "data": "DECISION_ACTION_DISPLAY", "className": "cls_nowrap" },
            { "data": "REASON_BY_OTHER", "className": "cls_nowrap" },
            { "data": "REMARK_REASON_BY_OTHER", "className": "cls_nowrap" },
            { "data": "APPROVE_SHADE_LIMIT_DISPLAY", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(3).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_cus_print .cls_comment_approve').html(data.COMMENT_BY_PA);
                $('.cls_art_cus_print .cls_cus_txt_comment ').html(data.COMMENT);

                if (data.IS_REPEAT == "X")
                    isRepeat = true;
                if (data.DECISION_ACTION != null) {
                    $(".cls_art_cus_print input[name=rdoChange_cus_11][value=" + data.DECISION_ACTION + "]").prop('checked', true);
                    $(".cls_art_cus_print .cls_chk_agree").prop('checked', true);
                    if (data.DECISION_ACTION == "0" && isRepeat != true) {
                        $(".cls_art_cus_print .cls_shade_print").show();
                        $(".cls_art_cus_print input[name=rdoShade_cus][value=" + data.APPROVE_SHADE_LIMIT + "]").prop('checked', true);
                    }
                    else if (data.DECISION_ACTION == "1") {
                        setValueToDDL('.cls_art_cus_print .cls_lov_revise_reason', data.REASON_ID, data.REASON_BY_OTHER);
                        setValueToDDLOther('.cls_art_cus_print  .cls_input_cus_print_revise_other', data.REMARK_REASON_BY_OTHER);
                    }
                    else if (data.DECISION_ACTION == "2") {
                        setValueToDDL('.cls_art_cus_print .cls_lov_cancel_reason', data.REASON_ID, data.REASON_BY_OTHER);
                        setValueToDDLOther('.cls_art_cus_print  .cls_input_cus_print_cancel_other', data.REMARK_REASON_BY_OTHER);
                    }
                }


                //Set value for Customer tab
                $('.cls_art_cus_print .cls_cus_sold').val(data.SOLD_TO_PO);
                $('.cls_art_cus_print .cls_cus_ship').val(data.SHIP_TO_PO);
            }
        }
    });

    table_customer_log_print.on('order.dt search.dt', function () {
        table_customer_log_print.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_customer_log_shade;
function bind_customer_shade() {
    table_customer_log_shade = $('#table_customer_log_shade').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtocustomer?data.step_artwork_code=SEND_CUS_SHADE&data.send_to_customer_type=REQ_CUS_SHADE&data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.main_artwork_sub_id=' + MainArtworkSubId + '&data.current_user_id=' + CURRENTUSERID,
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
            { "data": "CUSTOMER_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "COURIER_NUMBER", "className": "cls_nowrap" },
            { "data": "COMMENT_BY_PA", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "cls_nowrap" },
            { "data": "DECISION_ACTION_DISPLAY", "className": "cls_nowrap" },
            { "data": "REASON_BY_OTHER", "className": "cls_nowrap" },
            { "data": "REMARK_REASON_BY_OTHER", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_cus_shade .cls_cus_courier').html(data.COURIER_NUMBER);
                $('.cls_art_cus_shade .cls_cus_comment').html(data.COMMENT_BY_PA);
                $('.cls_art_cus_shade .cls_cus_txt_comment').html(data.COMMENT); // ticket#19383 by aof on 25/10/2021
                $(".cls_art_cus_shade input[name=rdoChange_cus_11][value=" + data.DECISION_ACTION + "]").prop('checked', true);
                if (data.DECISION_ACTION == "1") {
                    setValueToDDL('.cls_art_cus_shade .cls_lov_revise_reason', data.REASON_ID, data.REASON_BY_OTHER);
                    setValueToDDLOther('.cls_art_cus_shade  .cls_input_cus_shade_revise_other', data.REMARK_REASON_BY_OTHER);
                }
                if (data.DECISION_ACTION == "2") {
                    setValueToDDL('.cls_art_cus_shade .cls_lov_cancel_reason', data.REASON_ID, data.REASON_BY_OTHER);
                    setValueToDDLOther('.cls_art_cus_shade  .cls_input_cus_shade_cancel_other', data.REMARK_REASON_BY_OTHER);
                }

                //Set value for Customer tab
                $('.cls_art_cus_shade .cls_cus_sold').val(data.SOLD_TO_PO);
                $('.cls_art_cus_shade .cls_cus_ship').val(data.SHIP_TO_PO);
            }
        }
    });

    table_customer_log_shade.on('order.dt search.dt', function () {
        table_customer_log_shade.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_customer_log_ref;
function bind_customer_ref() {
    table_customer_log_ref = $('#table_customer_log_ref').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtocustomer?data.step_artwork_code=SEND_CUS_REF&data.send_to_customer_type=REQ_CUS_REF&data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.main_artwork_sub_id=' + MainArtworkSubId + '&data.current_user_id=' + CURRENTUSERID,
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
            { "data": "CUSTOMER_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "COURIER_NUMBER", "className": "cls_nowrap" },
            { "data": "COMMENT_BY_PA", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_cus_ref .cls_cus_txt_comment_ref').html(data.COMMENT_BY_PA);
                $('.cls_art_cus_ref .cls_cus_txt_courier').html(data.COURIER_NUMBER);

                //Set value for Customer tab
                $('.cls_art_cus_ref .cls_cus_sold').val(data.SOLD_TO_PO);
                $('.cls_art_cus_ref .cls_cus_ship').val(data.SHIP_TO_PO);
            }
        }
    });

    table_customer_log_ref.on('order.dt search.dt', function () {
        table_customer_log_ref.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_customer_log_req_ref;
function bind_customer_req_ref() {
    table_customer_log_req_ref = $('#table_customer_log_req_ref').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtocustomerreqref?data.step_artwork_code=SEND_CUS_REQ_REF&data.send_to_customer_type=REQ_CUS_REQ_REF&data.artwork_sub_id=' +
                    ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.current_user_id=' + CURRENTUSERID,
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
        "order": [[15, 'desc']],
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
            { "data": "CUSTOMER_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "COMMENT_FORMLABEL_DISPLAY", "className": "cls_nowrap" },
            { "data": "NUTRITION_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "INGREDIENTS_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "ANALYSIS_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "HEALTH_CLAIM_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "NUTRIENT_CLAIM_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "SPECIES_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "CATCHING_AREA_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "CHECK_DETAIL_COMMENT_DISPLAY", "className": "cls_nowrap" },
            { "data": "QC_COMMENT", "className": "cls_nowrap" },
            { "data": "COMMENT_NONCOMPLIANCE_DISPLAY", "className": "cls_nowrap" },
            { "data": "COMMENT_ADJUST_DISPLAY", "className": "cls_nowrap" },
            { "data": "COMMENT_BY_PA", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },

            { "data": "ACTION_NAME", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(18).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(15).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {

                $('.cls_art_cus_req_ref .cls_cus_txt_comment_req_ref').html(data.COMMENT);

                if (data.NUTRITION_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_nutri_txt_comment_cus').html(data.NUTRITION_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_req_ref .cls_nutri_cus').hide();
                if (data.INGREDIENTS_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_ingre_txt_comment_cus').html(data.INGREDIENTS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_req_ref .cls_ingri_cus').hide();
                if (data.ANALYSIS_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_analysis_txt_comment_cus').html(data.ANALYSIS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_req_ref .cls_analysis_cus').hide();
                if (data.HEALTH_CLAIM_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_health_txt_comment_cus').html(data.HEALTH_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_req_ref .cls_health_cus').hide();
                if (data.NUTRIENT_CLAIM_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_nutclaim_txt_comment_cus').html(data.NUTRIENT_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_req_ref .cls_nutri_claim_cus').hide();
                if (data.SPECIES_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_species_txt_comment_cus').html(data.SPECIES_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_req_ref .cls_species_cus').hide();
                if (data.CATCHING_AREA_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_catching_txt_comment_cus').html(data.CATCHING_AREA_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_req_ref .cls_catch_fao_cus').hide();

                if (data.CHECK_DETAIL_COMMENT_DISPLAY != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_check_detail_txt_comment_cus').html(data.CHECK_DETAIL_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_cus_req_ref .cls_check_detail_cus').hide();

                if (data.QC_COMMENT != "-") {
                    $('.cls_art_cus_req_ref .cls_remark_comment_qc_cus').html(data.QC_COMMENT);
                }
                else
                    $('.cls_art_cus_req_ref .cls_qccomment_cus').hide();

                if (data.IS_CHANGEDETAIL == "X") {
                    $('.cls_art_cus_req_ref .cls_body_detailqc').show();
                }
                else
                    $('.cls_art_cus_req_ref .cls_body_detailqc').hide();

                if (data.IS_NONCOMPLIANCE == "X") {
                    $('.cls_art_cus_req_ref .cls_customer_txt_comment_sender_12').val(data.COMMENT_NONCOMPLIANCE);
                    $('.cls_art_cus_req_ref .cls_cus_txt_comment_12').val(data.COMMENT_NONCOMPLIANCE_DISPLAY);
                    $('.cls_art_cus_req_ref .cls_body_noncompliance').show();
                }
                else
                    $('.cls_art_cus_req_ref .cls_body_noncompliance').hide();

                if (data.IS_FORMLABEL == "X") {
                    $('.cls_art_cus_req_ref .cls_customer_txt_comment_sender_1').val(data.COMMENT_CHANGE_DETAIL);
                    $('.cls_art_cus_req_ref .cls_cus_txt_comment_11').val(data.COMMENT_FORMLABEL_DISPLAY);
                    $('.cls_art_cus_req_ref .cls_body_form_label').show();
                }
                else
                    $('.cls_art_cus_req_ref .cls_body_form_label').hide();

                if (data.IS_ADJUST == "X") {
                    $('.cls_art_cus_req_ref .cls_customer_txt_comment_sender_2').val(data.COMMENT_ADJUST);
                    $('.cls_art_cus_req_ref .cls_cus_txt_comment_2').val(data.COMMENT_ADJUST_DISPLAY);
                    $('.cls_art_cus_req_ref .cls_body_adjust').show();
                }
                else
                    $('.cls_art_cus_req_ref .cls_body_adjust').hide();



                if (data.DECISION_FORMLABEL_DISPLAY == "Confirm to change")
                    $(".cls_art_cus_req_ref input[name=rdoChange_cus_11][value=1]").prop('checked', true);
                else if (data.DECISION_FORMLABEL_DISPLAY == "Do not change")
                    $(".cls_art_cus_req_ref input[name=rdoChange_cus_11][value=0]").prop('checked', true);

                if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Confirm to change")
                    $(".cls_art_cus_req_ref input[name=rdoChange_cus_12][value=1]").prop('checked', true);
                else if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Do not change")
                    $(".cls_art_cus_req_ref input[name=rdoChange_cus_12][value=0]").prop('checked', true);

                if (data.DECISION_ADJUST_DISPLAY == "Confirm to change")
                    $(".cls_art_cus_req_ref input[name=rdoChange_cus_2][value=1]").prop('checked', true);
                else if (data.DECISION_ADJUST_DISPLAY == "Do not change")
                    $(".cls_art_cus_req_ref input[name=rdoChange_cus_2][value=0]").prop('checked', true);

                $('.cls_art_cus_req_ref .cls_cus_txt_comment_sender').html(data.COMMENT_BY_PA);


                //Set value for Customer tab
                $('.cls_art_cus_req_ref .cls_cus_sold').val(data.SOLD_TO_PO);
                $('.cls_art_cus_req_ref .cls_cus_ship').val(data.SHIP_TO_PO);

            }
        }
    });

    table_customer_log_req_ref.on('order.dt search.dt', function () {
        table_customer_log_req_ref.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}