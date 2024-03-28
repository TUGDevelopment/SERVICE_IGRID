var mk_pa_quill, mk_gm_mk_quill, mk_qc_quill;
var mk_check_case, gmmk_check_case = false;
var mk_check_prev, qc_check_prev;
var editor1, editor2, editor3, editor4, editor5, editor6, editor7, editor8, editorRD
var is_check_verify = "";// ticket 150109 by voravut

var pp_pa_quill; // ticket442923 by aof
$(document).ready(function () {        

    // ticket442923 by aof start
    if (ReadOnly == "1") {
        $('.cls_art_pp .cls_pp_txt_comment').attr("style", "border:1px solid lightgray;padding:5px;background-color:#f8f8f8;border-color: #fff;min-height:100px;");

    } else {   
        pp_pa_quill = bind_text_editor('.cls_art_pp .cls_pp_txt_comment');  
    }
    // ticket442923 by aof last
   
    bind_text_editor('#qc_submit_modal .cls_txt_send_pa');
    bind_text_editor('#qc_submit_modal .cls_txt_send_mk');
    bind_text_editor('#qc_submit_modal .cls_txt_send_gm_qc');
    editorRD = bind_text_editor('#qc_submit_modal .cls_txt_send_rd');

    bind_text_editor('#mk_submit_modal .cls_txt_send_pa');
    bind_text_editor('#mk_submit_modal .cls_txt_send_gm_mk');
    bind_text_editor('#mk_submit_modal .cls_txt_send_qc');

    var qc_submit_modal = '#qc_submit_modal ';

    $('.cls_art_qc form').validate({
        rules: {
            rdoConfirm_qc:
                {
                    required: {
                        depends: function (element) {
                            return $(".cls_chk_send_pa_qc").is(":checked")
                        }
                    }
                },
            chk_result_qc:
                {
                    required: "#rdoNotConfirm_qc:checked"
                }

        },
        messages: {
            rdoConfirm_qc:
                {
                    required: "Please fill at least 1 of these fields."
                },
            chk_result_qc:
                {
                    required: "Please check at least 1 of these fields."
                }
        }
    });

    $('.cls_art_wh form').validate({
        rules: {
            rdo_roll_direc:
                {
                    required: true
                },
        },
        messages: {
            rdo_roll_direc:
                {
                    required: "Please check at least 1 of these fields."
                }
        }
    });

    $('.cls_art_rd form').validate({
        rules: {
            rdoConfirm_rd:
                {
                    required: true
                },
            chk_result_rd:
                {
                    required: "#rdoNotConfirm_rd:checked"
                }
        },
        messages: {
            rdoConfirm_rd:
                {
                    required: "Please fill at least 1 of these fields."
                },
            chk_result_rd:
                {
                    required: "Please check at least 1 of these fields."
                }
        }
    });

    $('.cls_art_mk_after_cus form').validate({
        rules: {
            rdoApprove_mk:
                {
                    required: true
                }
        },
        messages: {
            rdoApprove_mk:
                {
                    required: "Please check at least 1 of these fields."
                }
        }
    });

    $('.cls_art_qc_after_cus form').validate({
        rules: {
            rdoApprove_qc:
                {
                    required: true
                }
        },
        messages: {
            rdoApprove_qc:
                {
                    required: "Please check at least 1 of these fields."
                }
        }
    });


    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_internal':
                if ($(".cls_art_mk").is(':visible')) {
                    if (table_mk_log_art == null)
                        bind_art_mk_art();
                    else
                        table_mk_log_art.ajax.reload();
                }
                if ($(".cls_art_qc_after_cus").is(':visible')) {
                    if (table_qc_after_cus_log_art == null)
                        bind_art_qc_after_cus_art();
                    else
                        table_qc_after_cus_log_art.ajax.reload();
                }
                if ($(".cls_art_mk_after_cus").is(':visible')) {
                    if (table_mk_after_cus_log_art == null) {
                        bind_art_mk_after_cus_art();
                    }
                    else
                        table_mk_after_cus_log_art.ajax.reload();
                }
                if ($(".cls_art_qc").is(':visible')) {
                    if (table_qc_log_art == null)
                        bind_art_qc_art();
                    else
                        table_qc_log_art.ajax.reload();
                }
                if ($(".cls_art_rd").is(':visible')) {
                    if (table_rd_log_art == null)
                        bind_art_rd_art();
                    else
                        table_rd_log_art.ajax.reload();
                }
                if ($(".cls_art_wh").is(':visible')) {
                    if (table_wh_log_art == null)
                        bind_art_wh_art();
                    else
                        table_wh_log_art.ajax.reload();
                }
                if ($(".cls_art_plan").is(':visible')) {
                    if (table_pn_log_art == null)
                        bind_art_pn_art();
                    else
                        table_pn_log_art.ajax.reload();
                }
                if ($(".cls_art_gm_mk").is(':visible')) {
                    if (table_gm_mk_log_art == null)
                        bind_art_gm_mk_art();
                    else
                        table_gm_mk_log_art.ajax.reload();
                }
                if ($(".cls_art_gm_qc").is(':visible')) {
                    if (table_gm_qc_log_art == null)
                        bind_art_gm_qc_art();
                    else
                        table_gm_qc_log_art.ajax.reload();
                }
                if ($(".cls_art_pp").is(':visible')) {
                    if (table_pp_log_art == null)
                        bind_art_pp_art();
                    else
                        table_pp_log_art.ajax.reload();
                }
                break;

            default:
                break;
        }
    });
    $('.cls_art_qc input[name=chk_result_qc]').prop("disabled", true);
    $('.cls_art_rd input[name=chk_result_rd]').prop("disabled", true);

    $('.cls_art_rd #rdoConfirm_rd').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd input[name=chk_result_rd]').prop("disabled", true);
        }
    });

    $('.cls_art_rd #rdoNotConfirm_rd').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd input[name=chk_result_rd]').prop("disabled", false);
        }
    });

    $('.cls_art_qc #rdoConfirm_qc').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc input[name=chk_result_qc]').prop("disabled", true);
        }
    });

    $('.cls_art_qc #rdoNotConfirm_qc').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc input[name=chk_result_qc]').prop("disabled", false);
        }
    });

    $('.cls_art_qc #rdoConfirm_qc').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc input[name=chk_result_qc]').prop("checked", false);
            $('.cls_art_qc .cls_row_nutri').hide();
            $('.cls_art_qc .cls_row_ingre').hide();
            $('.cls_art_qc .cls_row_analysis').hide();
            $('.cls_art_qc .cls_row_health').hide();
            $('.cls_art_qc .cls_row_nutclaim').hide();
            $('.cls_art_qc .cls_row_species').hide();
            $('.cls_art_qc .cls_row_catching').hide();
            $('.cls_art_qc .cls_row_check_detail').hide();

        }
    });

    $(".cls_art_mk .cls_mk_btn_save").click(function () {
        save_art_mk('SAVE', false);
    });

    $(".cls_art_mk .cls_mk_btn_submit_from_pa").click(function () {
        if ($('.cls_input_mk_pa_other').is(':visible') && $('.cls_input_mk_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_art_mk('SUBMIT', true);
    });

    $(".cls_art_mk .cls_mk_btn_send_back").click(function () {
        if ($('.cls_input_mk_pa_other').is(':visible') && $('.cls_input_mk_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_art_mk('SEND_BACK', true);
    });

    $(".cls_art_qc .cls_qc_btn_save").click(function () {
        save_art_qc('SAVE', false, false);
    });

    $(".cls_art_qc form").submit(function (e) {
        if ($(this).valid()) {
            save_art_qc('SEND_BACK', true, false);
        }
        else if ($('.cls_input_qc_pa_other').is(':visible') && $('.cls_input_qc_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_art_mk_after_cus .cls_mk_btn_send_back").click(function () {
        sendback_art_mk_after_cus('SEND_BACK', true);
    });

    $(".cls_art_mk_after_cus form").submit(function (e) {
        if ($(this).valid()) {
            if (mk_check_case) {
                if (mk_check_prev == 'QC') {
                    if ($('.cls_art_mk_after_cus #rdoApprove_mk').is(":checked"))
                        SubmitAutoMK_ReqRef();
                    else if ($('.cls_art_mk_after_cus #rdoNotApprove_mk').is(":checked"))
                        SubmitAutoMK_Review();
                }
                else if (mk_check_prev == 'GM QC') {
                    SubmitAutoMK_GMMK();
                }
                else if (mk_check_prev == 'GM MK') {
                    SubmitAutoMK_ReqRef();
                }
                else if (mk_check_prev == 'GM MK Send back') {
                    SubmitAutoMK_GMMK();
                }
            }
            else {
                if (mk_check_prev == 'GM QC') {
                    SubmitAutoMK_GMMK();
                }
                else
                    SubmitAutoMK_PA();
            }
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action

    });

    $(".cls_art_qc_after_cus form").submit(function (e) {
        if ($(this).valid()) {
            if (qc_check_prev == 'REQ_CUS_REVIEW') {
                SubmitAutoQC_ToMK();
            }
            else if (qc_check_prev == 'REQ_CUS_REQ_REF') {
                SubmitAutoQC_ToGMQC();
            }
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });


    $(".cls_art_rd .cls_rd_btn_save").click(function () {
        save_art_rd('SAVE', false);
    });

    $(".cls_art_rd form").submit(function (e) {
        if ($(this).valid()) {
            save_art_rd('SUBMIT', true);
        }
        else if ($('.cls_input_rd_pa_other').is(':visible') && $('.cls_input_rd_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $('.cls_art_rd #rdoConfirm_rd').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd input[name=chk_result_rd]').prop("checked", false);
            $('.cls_art_rd .cls_row_nutri').hide();
            $('.cls_art_rd .cls_row_ingre').hide();
            $('.cls_art_rd .cls_row_analysis').hide();
            $('.cls_art_rd .cls_row_health').hide();
            $('.cls_art_rd .cls_row_nutclaim').hide();
            $('.cls_art_rd .cls_row_species').hide();
            $('.cls_art_rd .cls_row_catching').hide();
            $('.cls_art_rd .cls_row_check_detail').hide();

        }
    });

    $(".cls_art_rd .cls_rd_btn_send_back").click(function (e) {
        if ($('.cls_input_rd_pa_other').is(':visible') && $('.cls_input_rd_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_art_rd('SEND_BACK', true);
    });

    $(".cls_art_wh .cls_wh_btn_save").click(function () {
        save_art_wh('SAVE', false);
    });

    $(".cls_art_wh form").submit(function (e) {
        if ($(this).valid()) {
            save_art_wh('SUBMIT', true);
        }
        else if ($('.cls_input_wh_pa_other').is(':visible') && $('.cls_input_wh_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_art_wh .cls_wh_btn_send_back").click(function () {
        if ($('.cls_input_wh_pa_other').is(':visible') && $('.cls_input_wh_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_art_wh('SEND_BACK', true);
    });

    $(".cls_art_plan .cls_plan_btn_save").click(function () {
        save_art_pn('SAVE', false);
    });

    $(".cls_art_plan .cls_plan_btn_send_back").click(function () {
        if ($('.cls_input_plan_pa_other').is(':visible') && $('.cls_input_plan_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_art_pn('SEND_BACK', true);
    });

    $(".cls_art_plan .cls_plan_btn_submit").click(function () {
        if ($('.cls_input_plan_pa_other').is(':visible') && $('.cls_input_plan_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_art_pn('SUBMIT', true);
    });

    $(".cls_art_pp .cls_pp_btn_save").click(function () {
        save_art_pp('SAVE', false);
    });

    $(".cls_art_pp .cls_pp_btn_send_back").click(function () {
        if ($('.cls_input_pp_pa_other').is(':visible') && $('.cls_input_pp_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_art_pp('SEND_BACK', true);
    });

    $(".cls_art_pp .cls_pp_btn_submit").click(function () {
        if ($('.cls_input_pp_pa_other').is(':visible') && $('.cls_input_pp_pa_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        else
            save_art_pp('SENDTO_VENDOR', true);
    });

    //GM
    $(".cls_art_gm_mk .cls_btn_approve_gm_mk").click(function () {
        save_art_gm_mk('APPROVE', true);
    });
    $(".cls_art_gm_mk .cls_btn_not_approve_gm_mk").click(function () {
        save_art_gm_mk('NOTAPPROVE', true);
    });
    $(".cls_art_gm_mk .cls_btn_send_back_gm_mk").click(function () {
        save_art_gm_mk('SEND_BACK', true);
    });
    $(".cls_art_gm_mk .cls_btn_save_gm_mk").click(function () {
        save_art_gm_mk('SAVE', false);
    });

    $(".cls_art_gm_qc .cls_btn_approve_gm_qc").click(function () {
        save_art_gm_qc('APPROVE', true);
    });
    $(".cls_art_gm_qc .cls_btn_not_approve_gm_qc").click(function () {
        save_art_gm_qc('NOTAPPROVE', true);
    });

    $(".cls_art_gm_qc .cls_btn_send_back_gm_qc ").click(function () {
        sendback_art_gm_qc('SEND_BACK', true);
    });

    $(".cls_art_gm_qc .cls_btn_save_gm_qc").click(function () {
        save_art_gm_qc('SAVE', false);
    });


    //QC
    $('.cls_art_qc #chNut').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc .cls_row_nutri').show();
        }
        else {
            $('.cls_art_qc .cls_row_nutri').hide();
        }
    });
    $('.cls_art_qc #chIng').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc .cls_row_ingre').show();
        }
        else {
            $('.cls_art_qc .cls_row_ingre').hide();
        }
    });
    $('.cls_art_qc #chResultAnalysis').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc .cls_row_analysis').show();
        }
        else {
            $('.cls_art_qc .cls_row_analysis').hide();
        }
    });
    $('.cls_art_qc #chResultHealthClaim').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc .cls_row_health').show();
        }
        else {
            $('.cls_art_qc .cls_row_health').hide();
        }
    });
    $('.cls_art_qc #chResultNutrientClaim').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc .cls_row_nutclaim').show();
        }
        else {
            $('.cls_art_qc .cls_row_nutclaim').hide();
        }
    });
    $('.cls_art_qc #chResultSpecies').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc .cls_row_species').show();
        }
        else {
            $('.cls_art_qc .cls_row_species').hide();
        }
    });
    $('.cls_art_qc #chResultCatching').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc .cls_row_catching').show();
        }
        else {
            $('.cls_art_qc .cls_row_catching').hide();
        }
    });
    $('.cls_art_qc #chResultCheckdetail').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_qc .cls_row_check_detail').show();
        }
        else {
            $('.cls_art_qc .cls_row_check_detail').hide();
        }
    });

    $('#qc_submit_modal .cls_chk_send_rd_qc').click(function () {
        if ($(this).prop('checked')) {
            $('#qc_submit_modal .cls_body_send_rd').show();
        }
        else {
            $('#qc_submit_modal .cls_body_send_rd').hide();
        }
    });
    $('#qc_submit_modal .cls_chk_send_pa_qc').click(function () {
        if ($(this).prop('checked')) {
            $('#qc_submit_modal .cls_body_send_pa').show();
            $('#qc_submit_modal .cls_body_send_rd').hide();
        }
        else {
            $('#qc_submit_modal .cls_body_send_pa').hide();
        }
    });

    //R&D
    $('.cls_art_rd #chNut').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd .cls_row_nutri').show();
        }
        else {
            $('.cls_art_rd .cls_row_nutri').hide();
        }
    });
    $('.cls_art_rd #chIng').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd .cls_row_ingre').show();
        }
        else {
            $('.cls_art_rd .cls_row_ingre').hide();
        }
    });
    $('.cls_art_rd #chResultAnalysis').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd .cls_row_analysis').show();
        }
        else {
            $('.cls_art_rd .cls_row_analysis').hide();
        }
    });
    $('.cls_art_rd #chResultHealthClaim').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd .cls_row_health').show();
        }
        else {
            $('.cls_art_rd .cls_row_health').hide();
        }
    });
    $('.cls_art_rd #chResultNutrientClaim').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd .cls_row_nutclaim').show();
        }
        else {
            $('.cls_art_rd .cls_row_nutclaim').hide();
        }
    });
    $('.cls_art_rd #chResultSpecies').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd .cls_row_species').show();
        }
        else {
            $('.cls_art_rd .cls_row_species').hide();
        }
    });
    $('.cls_art_rd #chResultCatching').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd .cls_row_catching').show();
        }
        else {
            $('.cls_art_rd .cls_row_catching').hide();
        }
    });
    $('.cls_art_rd #chResultCheckdetail').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_art_rd .cls_row_check_detail').show();
        }
        else {
            $('.cls_art_rd .cls_row_check_detail').hide();
        }
    });

    $("#qc_submit_modal form").submit(function (e) {
        if ($(this).valid()) {
            if ($('.cls_art_qc form').valid() && $('.cls_art_qc_after_cus form').valid()) {
                QCSubmitDataPop();
            }
            else if ($('#qc_submit_modal .cls_chk_send_rd_qc').is(":checked")) {
                QCSubmitDataPop();
            }
            else {
                hide_modal_submit_qc_error();
                //$('.cls_art_qc form').removeAttr("novalidate");
                //$('.cls_art_qc_after_cus form').removeAttr("novalidate");
                if ($('.cls_input_qc_pa_other').is(':visible') && $('.cls_input_qc_pa_other').val() == '')
                    alertError2("Please fill remark reason");
            }
        }
        else if ($('.cls_input_rd_by_qc_other').is(':visible') && $('.cls_input_rd_by_qc_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        e.preventDefault();	//STOP default action
    });

    if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_MK") {
        $('.cls_art_mk .cls_div_only_mk').show();
    }

    if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_MK_VERIFY") {
        $('.cls_art_mk_after_cus  .cls_div_only_mk_after_cus').show();
    }

    if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_QC") {
        $('.cls_art_qc .cls_div_only_qc').show();

        $('#qc_submit_modal .cls_from_pa_only').show();
        $('#qc_submit_modal .cls_from_customer_only').hide();
    }

    if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_QC_VERIFY") {
        $('.cls_art_mk_after_cus .cls_div_only_mk_after_cus').show();

        $('#qc_submit_modal .cls_from_pa_only').hide();
        $('#qc_submit_modal .cls_from_customer_only').show();
    }

    if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_RD") {
        $('.cls_art_rd .cls_div_only_rd').show();
    }

    if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_WH") {
        $('.cls_art_wh .cls_div_only_wh').show();
    }

    if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_PN") {
        $('.cls_art_plan .cls_div_only_pn').show();
    }


    $('#send_to_customer_modal_mk .cls_chk_send_review_customer').click(function () {
        if ($(this).prop('checked')) {
            $('#send_to_customer_modal_mk .cls_body_send_review_customer').show();
        }
        else {
            $('#send_to_customer_modal_mk .cls_body_send_review_customer').hide();
        }
    });


    //$(".cls_art_qc_after_cus .cls_qc_btn_submit").click(function () {
    //    $("#qc_submit_modal").modal({
    //        backdrop: 'static',
    //        keyboard: true
    //    });

    //    $(qc_submit_modal + ' .cls_lov_search_file_template').val('').trigger('change');
    //    $(qc_submit_modal + ' input[type=checkbox]').prop('checked', false);

    //});

    $(".cls_art_qc .cls_qc_btn_submit").click(function () {
        $("#qc_submit_modal").modal({
            backdrop: 'static',
            keyboard: true
        });

        $(qc_submit_modal + ' .cls_lov_search_file_template').val('').trigger('change');
        resetDllReason(qc_submit_modal + ' .cls_lov_send_for_reason');

        editorRD.setContents([{ insert: '\n' }]);

        $(qc_submit_modal + " .cls_input_rd_by_qc_other").val('').hide();
        $(qc_submit_modal + ' input[type=checkbox]').prop('checked', false);
        $('#qc_submit_modal .cls_body_send_rd').hide();

    });

    //$("#send_to_customer_modal_mk .cls_btn_submit_customer_modal").click(function () {
    //    SubmitDataPopSendtocustomerByMK();
    //});


    //$("#mk_submit_modal form").submit(function (e) {
    //    if ($('.cls_art_mk_after_cus form').valid()) {
    //        MKSubmitDataPop();
    //    }
    //    else {
    //        hide_modal_submit_mk_error();
    //        $('.cls_art_mk_after_cus form').removeAttr("novalidate");
    //    }
    //    e.preventDefault();

    //});

    //$(".cls_art_mk_after_cus .cls_mk_btn_submit").click(function () {
    //    $("#mk_submit_modal").modal({
    //        backdrop: 'static',
    //        keyboard: true
    //    });

    //    $(mk_submit_modal + ' .cls_lov_search_file_template').val('').trigger('change');
    //    $(mk_submit_modal + ' input[type=checkbox]').prop('checked', false);

    //});

    //$(".cls_art_mk_after_cus .cls_cus_mk_btn_submit").click(function () {
    //    $("#send_to_customer_modal_mk").modal({
    //        backdrop: 'static',
    //        keyboard: true
    //    });

    //    $(send_to_customer_modal_mk + ' .cls_lov_search_file_template').val('').trigger('change');
    //    $(send_to_customer_modal_mk + ' input[type=checkbox]:enabled').prop('checked', false);
    //    $(send_to_customer_modal_mk + ' .cls_body_send_review_customer ').hide();

    //});
});

function SubmitAutoQC_ToMK() {
    var jsonObj = new Object();
    jsonObj.data = {};
    var item = {};
    item.PROCESS = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = "SUBMIT";
    item["ENDTASKFORM"] = true;
    item["COMMENT"] = $('.cls_art_qc_after_cus .cls_qc_txt_comment').val();
    if ($('.cls_art_qc_after_cus #rdoApprove_qc').is(":checked"))
        item["APPROVE"] = "1";
    else if ($('.cls_art_qc_after_cus #rdoNotApprove_qc').is(":checked"))
        item["APPROVE"] = "0";
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
    item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_MK_VERIFY').curr_role;
    item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_MK_VERIFY').curr_step;
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;
    item.PROCESS["REMARK"] = $('.cls_art_qc_after_cus .cls_qc_txt_comment').val();

    msgbox = 'Do you want to submit to MK?';
    if (is_check_verify != "")
        msgbox = 'Do you want to submit ?';


    jsonObj.data = item;

    var myurl = '/api/taskform/qc/sendtomk';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, msgbox);
}

function SubmitAutoQC_ToGMQC() {
    var jsonObj = new Object();
    jsonObj.data = {};
    var item = {};
    item.PROCESS = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ENDTASKFORM"] = true;
    item["ACTION_CODE"] = "SUBMIT";
    if ($('.cls_art_qc_after_cus #rdoApprove_qc').is(":checked"))
        item["APPROVE"] = "1";
    else if ($('.cls_art_qc_after_cus #rdoNotApprove_qc').is(":checked"))
        item["APPROVE"] = "0";
    item["COMMENT"] = $('.cls_art_qc_after_cus .cls_qc_txt_comment').val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
    item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_GM_QC').curr_role;
    item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_GM_QC').curr_step;
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;
    item.PROCESS["REMARK"] = $('.cls_art_qc_after_cus .cls_qc_txt_comment').val();


    jsonObj.data = item;

    var myurl = '/api/taskform/qc/sendtogmqc';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, 'Do you want to submit to GM QC?');
}

function SubmitAutoMK_Review() {
    var jsonObj = new Object();
    jsonObj.data = {};
    var item = {};
    item.PROCESS = {};


    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ENDTASKFORM"] = true;
    item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REVIEW";
    if ($('.cls_art_mk_after_cus #rdoApprove_mk').is(":checked"))
        item["APPROVE"] = "1";
    else if ($('.cls_art_mk_after_cus #rdoNotApprove_mk').is(":checked"))
        item["APPROVE"] = "0";
    item["ACTION_CODE"] = "SUBMIT";
    item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();

    if ($('#send_to_customer_modal_mk .cls_chk_formandlabelraw_customer').is(":checked")) {
        item["IS_FORMLABEL"] = "X";
        item["COMMENT_FORM_LABEL"] = $('#send_to_customer_modal_mk .cls_body_formandlabelraw_customer .cls_remark_formandlabelraw_cus_pop').val();
    }
    if ($('#send_to_customer_modal_mk .cls_chk_qc_changedetails_customer').is(":checked"))
        item["IS_CHANGEDETAIL"] = "X";
    if ($('#send_to_customer_modal_mk .cls_chk_noncompliance_customer').is(":checked")) {
        item["IS_NONCOMPLIANCE"] = "X";
        item["COMMENT_NONCOMPLIANCE"] = $('#send_to_customer_modal_mk .cls_body_noncompliance_customer .cls_remark_noncompliance_cus_pop').val();
    }
    if ($('#send_to_customer_modal_mk  .cls_chk_remark_adjustment_customer').is(":checked")) {
        item["IS_ADJUST"] = "X";
        item["COMMENT_ADJUST"] = $('#send_to_customer_modal_mk .cls_body_adjustment_customer .cls_remark_adjustment_cus_pop').val();
    }


    if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_nutri_txt_comment_cus_pop').html() != "")
        item["NUTRITION_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_nutri_txt_comment_cus_pop').html();
    if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_ingre_txt_comment_cus_pop').html() != "")
        item["INGREDIENTS_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_ingre_txt_comment_cus_pop').html();
    if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_analysis_txt_comment_cus_pop').html() != "")
        item["ANALYSIS_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_analysis_txt_comment_cus_pop').html();
    if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_health_txt_comment_cus_pop').html() != "")
        item["HEALTH_CLAIM_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_health_txt_comment_cus_pop').html();
    if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_nutclaim_txt_comment_cus_pop').html() != "")
        item["NUTRIENT_CLAIM_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_nutclaim_txt_comment_cus_pop').html();
    if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_species_txt_comment_cus_pop').html() != "")
        item["SPECIES_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_species_txt_comment_cus_pop').html();
    if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_catching_txt_comment_cus_pop').html() != "")
        item["CATCHING_AREA_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_catching_txt_comment_cus_pop').html();
    debugger;
    //if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_check_detail_txt_comment_cus_pop').html() != "")
    //    item["CHECK_DETAIL_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_check_detail_txt_comment_cus_pop').html();
    //if ($('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_comment_qc_cus').html() != "")
    //    item["QC_COMMENT"] = $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer .cls_remark_comment_qc_cus').html();

    var editor8 = new Quill('#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_check_detail_txt_comment_cus_pop');
    if (editor8.root.innerHTML != "<p><br></p>")
        item["CHECK_DETAIL_COMMENT"] = editor8.root.innerHTML;
    item["QC_COMMENT"] = $("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_comment_qc_cus_pop").text();

    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    item.PROCESS["REMARK"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();
    item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
    item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_CUS_REVIEW').curr_step;
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;

    jsonObj.data = item;

    var myurl = '/api/taskform/pa/sendtocustomer';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, 'Do you want to send to customer for review artwork?');
}

function SubmitAutoMK_ReqRef() {
    var jsonObj = new Object();
    jsonObj.data = {};
    var item = {};
    item.PROCESS = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ENDTASKFORM"] = true;
    item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REQ_REF";
    if ($('.cls_art_mk_after_cus #rdoApprove_mk').is(":checked"))
        item["APPROVE"] = "1";
    else if ($('.cls_art_mk_after_cus #rdoNotApprove_mk').is(":checked"))
        item["APPROVE"] = "0";
    item["ACTION_CODE"] = "SUBMIT";
    item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;


    item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
    item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_CUS_REQ_REF').curr_step;
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;
    item.PROCESS["REMARK"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();

    jsonObj.data = item;

    var myurl = '/api/taskform/pa/sendtocustomer';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, 'Do you want to send to customer for request reference letter?');
}

function SubmitAutoMK_GMMK() {
    var jsonObj = new Object();
    jsonObj.data = {};
    var item = {};
    item.PROCESS = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ENDTASKFORM"] = true;
    item["ACTION_CODE"] = "SUBMIT";
    item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
    item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_GM_MK').curr_role;
    item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_GM_MK').curr_step;
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;
    item.PROCESS["REMARK"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();


    jsonObj.data = item;

    var myurl = '/api/taskform/mk/sendtogmmk';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, 'Do you want to submit to GM MK?');
}

function SubmitAutoMK_PA() {
    var jsonObj = new Object();
    jsonObj.data = {};
    var item = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = "SUBMIT";
    if ($('.cls_art_mk_after_cus #rdoApprove_mk').is(":checked"))
        item["APPROVE"] = "1";
    else if ($('.cls_art_mk_after_cus #rdoNotApprove_mk').is(":checked"))
        item["APPROVE"] = "0";
    item["ENDTASKFORM"] = true;
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;
    item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();


    jsonObj.data = item;

    var myurl = '/api/taskform/mk/sendtopa';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, 'Do you want to submit to PA?');
}

function hide_modal_send_customer_mk() {
    $('#send_to_customer_modal_mk').modal('hide');
}

var table_mk_log_art;
function bind_art_mk_art() {
    table_mk_log_art = $('#table_mk_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/internal/mk/info?data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "paging": false,
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
            { "data": "REASON_BY_PA", "className": "" },
            { "data": "REMARK_REASON_BY_PA", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON_BY_OTHER", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_mk .cls_mk_txt_comment_sender').html(data.COMMENT_BY_PA);
                $('.cls_art_mk .cls_mk_txt_comment').val(data.COMMENT);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_art_mk .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
            }

        },
    });

    table_mk_log_art.on('order.dt search.dt', function () {
        table_mk_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_mk_after_cus_log_art;
function bind_art_mk_after_cus_art() {
    table_mk_after_cus_log_art = $('#table_mk_after_cus_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/qc/sendtomk?data.artwork_item_id=' + ARTWORK_ITEM_ID + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "order": [[11, 'desc']],
        "processing": true,
        "lengthChange": false,
        "paging": false,
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
            { "data": "PREV_STEP_SHOW", "className": "" },
            { "data": "DECISION_FORMLABEL_DISPLAY", "className": "" },
            { "data": "COMMENT_FORMLABEL_DISPLAY", "className": "" },
            { "data": "DECISION_NONCOMPLIANCE_DISPLAY", "className": "" },
            { "data": "COMMENT_NONCOMPLIANCE_DISPLAY", "className": "" },
            { "data": "DECISION_ADJUST_DISPLAY", "className": "" },
            { "data": "COMMENT_ADJUST_DISPLAY", "className": "" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "APPROVE_BY_QC", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
            { "data": "ACTION_NAME_BY_OTHER", "className": "" },
            { "data": "APPROVE_BY_OTHER", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE_BY_OTHER", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(15).html(myDateTimeMoment(data.CREATE_DATE_BY_OTHER));
            $(row).find('td').eq(11).html(myDateTimeMoment(data.CREATE_DATE));
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {

                if (data.PREV_STEP_DISPLAY == "GM QC") {
                    $('.cls_art_mk_after_cus .cls_no_gm_mk').hide();
                    $('.cls_art_mk_after_cus .cls_cus_btn_submit_auto').show();
                    $('.cls_art_mk_after_cus .cls_cus_mk_btn_submit').hide();
                    mk_check_prev = 'GM QC';
                }
                else if (data.PREV_STEP_DISPLAY == "GM MK") {
                    $('.cls_art_mk_after_cus .cls_no_gm_mk').hide();

                    if (data.ACTION_NAME == 'Send back') {
                        $('.cls_art_mk_after_cus .cls_cus_btn_submit_auto').show();
                        $('.cls_art_mk_after_cus .cls_cus_mk_btn_submit').hide();
                        mk_check_prev = 'GM MK Send back';
                    }
                    else if (data.ACTION_NAME == 'Not approve') {
                        $('.cls_art_mk_after_cus .cls_cus_btn_submit_auto').hide();
                        $('.cls_art_mk_after_cus .cls_cus_mk_btn_submit').show();
                        mk_check_prev = 'GM MK';
                    }
                    else {
                        $('.cls_art_mk_after_cus .cls_cus_btn_submit_auto').hide();
                        $('.cls_art_mk_after_cus .cls_cus_mk_btn_submit').show();
                        mk_check_case = 'GM MK'
                    }
                }
                else {
                    if (data.APPROVE_BY_QC == "Approve")
                        $(".cls_art_mk_after_cus input[name=rdoApprove_mk][value=1]").prop('checked', true);
                    else if (data.APPROVE_BY_QC == "Not Approve")
                        $(".cls_art_mk_after_cus input[name=rdoApprove_mk][value=0]").prop('checked', true);
                    $(".cls_art_mk_after_cus input[name=rdoApprove_mk]").prop('disabled', true);
                    $('.cls_art_mk_after_cus .cls_no_gm_mk').show();
                    $('.cls_art_mk_after_cus .cls_cus_mk_btn_submit').show();
                    $('.cls_art_mk_after_cus .cls_cus_btn_submit_auto').hide();
                    mk_check_prev = 'QC';
                }

                if (data.IS_FORMLABEL == 'X') {
                    //Case B
                    $('#send_to_customer_modal_mk .cls_chk_formandlabelraw_customer').prop('checked', true);
                    $('.cls_art_mk_after_cus .cls_customer_txt_comment_sender_11').html(data.COMMENT_FORMLABEL_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_formandlabelraw_cus_pop').val(data.COMMENT_CHANGE_DETAIL);
                    $('#send_to_customer_modal_mk .cls_body_formandlabelraw_customer').show();
                    mk_check_case = true;
                }
                else $('#send_to_customer_modal_mk .cls_body_formandlabelraw_customer').hide();
                if (data.IS_CHANGEDETAIL == 'X') {
                    $('#send_to_customer_modal_mk .cls_chk_qc_changedetails_customer').prop('checked', true);
                    $('.cls_art_mk_after_cus .cls_body_detailqc').show();
                    $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer').show();
                }
                else $('#send_to_customer_modal_mk .cls_body_qc_changedetails_customer').hide();
                if (data.IS_NONCOMPLIANCE == 'X') {
                    $('#send_to_customer_modal_mk .cls_chk_noncompliance_customer').prop('checked', true);
                    $('.cls_art_mk_after_cus .cls_customer_txt_comment_sender_12').html(data.COMMENT_NONCOMPLIANCE_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_noncompliance_cus_pop').val(data.COMMENT_NONCOMPLIANCE);
                    $('#send_to_customer_modal_mk .cls_body_noncompliance_customer').show();
                }
                else $('#send_to_customer_modal_mk .cls_body_noncompliance_customer').hide();
                if (data.IS_ADJUST == 'X') {

                    //Case C
                    $('#send_to_customer_modal_mk .cls_chk_remark_adjustment_customer').prop('checked', true);
                    $('.cls_art_mk_after_cus .cls_customer_txt_comment_sender_2').html(data.COMMENT_ADJUST_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_adjustment_cus_pop').val(data.COMMENT_ADJUST);
                    $('#send_to_customer_modal_mk .cls_body_adjustment_customer').show();
                    $('.cls_art_mk_after_cus .cls_cus_mk_btn_submit').hide();
                    $('.cls_art_mk_after_cus .cls_cus_btn_submit_auto').show();
                    mk_check_case = false;
                }
                else $('#send_to_customer_modal_mk .cls_body_adjustment_customer').hide();


                if (data.NUTRITION_COMMENT_DISPLAY != "-" && data.NUTRITION_COMMENT_DISPLAY != null) {
                    $('.cls_art_mk_after_cus .cls_remark_nutri_txt_comment_cus').html(data.NUTRITION_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_nutri_txt_comment_cus_pop').html(data.NUTRITION_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_nutri_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_nutri_mk').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_nutri_mk').hide();

                if (data.INGREDIENTS_COMMENT_DISPLAY != "-" && data.INGREDIENTS_COMMENT_DISPLAY != null) {
                    $('.cls_art_mk_after_cus .cls_remark_ingre_txt_comment_cus').html(data.INGREDIENTS_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_ingre_txt_comment_cus_pop').html(data.INGREDIENTS_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_ingri_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_ingri_mk').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_ingri_mk').hide();

                if (data.ANALYSIS_COMMENT_DISPLAY != "-" && data.ANALYSIS_COMMENT_DISPLAY != null) {
                    $('.cls_art_mk_after_cus .cls_remark_analysis_txt_comment_cus').html(data.ANALYSIS_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_analysis_txt_comment_cus_pop').html(data.ANALYSIS_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_analysis_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_analysis_mk').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_analysis_mk').hide();

                if (data.HEALTH_CLAIM_COMMENT_DISPLAY != "-" && data.HEALTH_CLAIM_COMMENT_DISPLAY != null) {
                    $('.cls_art_mk_after_cus .cls_remark_health_txt_comment_cus').html(data.HEALTH_CLAIM_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_health_txt_comment_cus_pop').html(data.HEALTH_CLAIM_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_health_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_health_mk').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_health_mk').hide();

                if (data.NUTRIENT_CLAIM_COMMENT_DISPLAY != "-" && data.NUTRIENT_CLAIM_COMMENT_DISPLAY != null) {
                    $('.cls_art_mk_after_cus .cls_remark_nutclaim_txt_comment_cus').html(data.NUTRIENT_CLAIM_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_nutclaim_txt_comment_cus_pop').html(data.NUTRIENT_CLAIM_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_nutri_claim_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_nutri_claim_mk').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_nutri_claim_mk').hide();

                if (data.SPECIES_COMMENT_DISPLAY != "-" && data.SPECIES_COMMENT_DISPLAY != null) {
                    $('.cls_art_mk_after_cus .cls_remark_species_txt_comment_cus').html(data.SPECIES_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_species_txt_comment_cus_pop').html(data.SPECIES_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_species_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_species_mk').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_species_mk').hide();

                if (data.CATCHING_AREA_COMMENT_DISPLAY != "-" && data.CATCHING_AREA_COMMENT_DISPLAY != null) {
                    $('.cls_art_mk_after_cus .cls_remark_catching_txt_comment_cus').html(data.CATCHING_AREA_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_catching_txt_comment_cus_pop').html(data.CATCHING_AREA_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_catch_fao_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_catch_fao_mk').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_catch_fao_mk').hide();

                if (data.CHECK_DETAIL_COMMENT_DISPLAY != "-" && data.CHECK_DETAIL_COMMENT_DISPLAY != null) {
                    $('.cls_art_mk_after_cus .cls_remark_check_detail_txt_comment_cus').html(data.CHECK_DETAIL_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_remark_check_detail_txt_comment_cus_pop').html(data.CHECK_DETAIL_COMMENT_DISPLAY);
                    $('#send_to_customer_modal_mk .cls_check_detail_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_check_detail_mk').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_check_detail_mk').hide();

                if (data.QC_COMMENT != "-" && data.QC_COMMENT != null) {
                    $('.cls_art_mk_after_cus .cls_qccomment_mk .cls_remark_comment_qc_cus').text(data.QC_COMMENT);
                    $('#send_to_customer_modal_mk .cls_remark_comment_qc_cus').text(data.QC_COMMENT);
                    $('#send_to_customer_modal_mk .cls_qccomment_cus_pop').show();
                    $('.cls_art_mk_after_cus .cls_qccomment_cus_pop').show();
                }
                else
                    $('.cls_art_mk_after_cus .cls_qccomment_cus_pop').hide();



                if (data.COMMENT_CHANGE_DETAIL != "-")
                    $('.cls_art_mk_after_cus .cls_customer_txt_comment_sender_1_pa').text(data.COMMENT_CHANGE_DETAIL);
                else
                    $('.cls_art_mk_after_cus .cls_body_form_label').hide();
                if (data.COMMENT_NONCOMPLIANCE != "-")
                    $('.cls_art_mk_after_cus .cls_customer_txt_comment_sender_12_pa').text(data.COMMENT_NONCOMPLIANCE);
                else
                    $('.cls_art_mk_after_cus .cls_body_noncompliance').hide();
                if (data.COMMENT_ADJUST != "-")
                    $('.cls_art_mk_after_cus .cls_customer_txt_comment_sender_2_pa').text(data.COMMENT_ADJUST);
                else
                    $('.cls_art_mk_after_cus .cls_body_adjust').hide();


                if (data.DECISION_FORMLABEL_DISPLAY == "Confirm to change")
                    $(".cls_art_mk_after_cus input[name=rdoChange_mk_11][value=1]").prop('checked', true);
                else if (data.DECISION_FORMLABEL_DISPLAY == "Do not change")
                    $(".cls_art_mk_after_cus input[name=rdoChange_mk_11][value=0]").prop('checked', true);

                if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Confirm to change")
                    $(".cls_art_mk_after_cus input[name=rdoChange_mk_12][value=1]").prop('checked', true);
                else if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Do not change")
                    $(".cls_art_mk_after_cus input[name=rdoChange_mk_12][value=0]").prop('checked', true);

                if (data.DECISION_ADJUST_DISPLAY == "Confirm to change")
                    $(".cls_art_mk_after_cus input[name=rdoChange_mk_2][value=1]").prop('checked', true);
                else if (data.DECISION_ADJUST_DISPLAY == "Do not change")
                    $(".cls_art_mk_after_cus input[name=rdoChange_mk_2][value=0]").prop('checked', true);

                if (data.APPROVE_BY_OTHER == "Approve")
                    $(".cls_art_mk_after_cus input[name=rdoApprove_mk][value=1]").prop('checked', true);
                else if (data.APPROVE_BY_OTHER == "Not Approve")
                    $(".cls_art_mk_after_cus input[name=rdoApprove_mk][value=0]").prop('checked', true);

                $('.cls_art_mk_after_cus .cls_mk_txt_comment_from_qc').val(data.COMMENT_BY_PA);
                $('.cls_art_mk_after_cus .cls_mk_txt_comment').val(data.COMMENT);
            }

        },
    });

    table_mk_after_cus_log_art.on('order.dt search.dt', function () {
        table_mk_after_cus_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}


function save_art_mk(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;

    if (CURRENT_STEP_CODE_DISPLAY_TXT !== 'SEND_MK') {
        var editor = new Quill('#mk_submit_modal .cls_txt_send_pa');
        item["COMMENT"] = editor.root.innerHTML;
        item["REASON_ID"] = $("#mk_submit_modal .cls_lov_send_for_reason").val();
    }
    else {
        item["COMMENT"] = $('.cls_art_mk .cls_mk_txt_comment').val();
        item["REASON_ID"] = $(".cls_art_mk .cls_lov_send_for_reason").val();
    }
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["ENDTASKFORM"] = EndTaskForm;

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }
    item["REMARK_REASON"] = $(".cls_art_mk .cls_input_mk_pa_other").val();
    item["WF_STEP"] = getstepartwork('SEND_MK').curr_step;

    jsonObj.data = item;

    var myurl = '/api/taskform/internal/mk/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}

function sendback_art_mk_after_cus(ACTION_CODE, EndTaskForm) {

    var jsonObj = new Object();
    var item = {};
    item.PROCESS = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;
    item["ENDTASKFORM"] = EndTaskForm;
    if ($('.cls_art_mk_after_cus #rdoApprove_mk').is(":checked"))
        item["APPROVE"] = "1";
    else if ($('.cls_art_mk_after_cus #rdoNotApprove_mk').is(":checked"))
        item["APPROVE"] = "0";
    item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;

    item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
    if (mk_check_prev == 'GM MK') {
        item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_GM_MK').curr_role;
        item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_GM_MK').curr_step;
    }
    else if (mk_check_prev != 'GM QC') {
        item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_QC_VERIFY').curr_role;
        item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_QC_VERIFY').curr_step;
    }
    else if (mk_check_prev == 'GM QC') {
        item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_GM_QC').curr_role;
        item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_GM_QC').curr_step;
    }
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;
    item.PROCESS["REMARK"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();

    jsonObj.data = item;

    var myurl = '/api/taskform/mk/sendtoqc';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
    else
        myAjax(myurl, mytype, mydata);
}

function sendback_art_gm_qc(ACTION_CODE, EndTaskForm) {

    var jsonObj = new Object();
    var item = {};
    item.PROCESS = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;
    item["ENDTASKFORM"] = EndTaskForm;
    item["COMMENT"] = $('.cls_art_gm_qc .cls_gm_qc_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;

    item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
    item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_QC_VERIFY').curr_role;
    item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_QC_VERIFY').curr_step;
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;
    item.PROCESS["REMARK"] = $('.cls_art_gm_qc .cls_gm_qc_txt_comment').val();

    jsonObj.data = item;

    var myurl = '/api/taskform/qc/sendtomkbygmqc';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
    else
        myAjax(myurl, mytype, mydata);
}


var table_qc_log_art;
function bind_art_qc_art() {
    table_qc_log_art = $('#table_qc_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtoqc?data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "paging": false,
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
            { "data": "REASON_BY_PA", "className": "" },
            { "data": "REMARK_REASON_BY_PA", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON_BY_OTHER", "className": "" },
            { "data": "IS_CONFIRED_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "NUTRITION_QC_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "NUTRITION_COMMENT", "className": "" },
            { "data": "HEALTH_CLAIM_QC_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "HEALTH_CLAIM_COMMENT", "className": "" },
            { "data": "CATCHING_AREA_QC_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "CATCHING_AREA_COMMENT", "className": "" },
            { "data": "INGREDIENTS_QC_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "INGREDIENTS_COMMENT", "className": "" },
            { "data": "NUTRIENT_CLAIM_QC_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "NUTRIENT_CLAIM_COMMENT", "className": "" },
            { "data": "ANALYSIS_QC_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "ANALYSIS_COMMENT", "className": "" },
            { "data": "SPECIES_QC_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "SPECIES_COMMENT", "className": "" },
            { "data": "CHECK_QC_DETAIL_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "CHECK_DETAIL_COMMENT", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },

        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(26).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));

            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_qc .cls_qc_txt_remark').html(data.COMMENT_BY_PA);
                //$('.cls_art_qc .cls_qc_txt_comment').val(data.COMMENT);   //#INC-77873 by aof on 20/09/2022
             
                //alert("QC"+ data.ARTWORK_SUB_ID);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_art_qc .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);

                if (data.NUTRITION_DISPLAY_TXT == 'Yes') $('.cls_art_qc .cls_chk_qc_2').prop('checked', true);
                if (data.INGREDIENTS_DISPLAY_TXT == 'Yes') $('.cls_art_qc .cls_chk_qc_3').prop('checked', true);
                if (data.ANALYSIS_DISPLAY_TXT == 'Yes') $('.cls_art_qc .cls_chk_qc_4').prop('checked', true);
                if (data.HEALTH_CLAIM_DISPLAY_TXT == 'Yes') $('.cls_art_qc .cls_chk_qc_5').prop('checked', true);
                if (data.NUTRIENT_CLAIM_DISPLAY_TXT == 'Yes') $('.cls_art_qc .cls_chk_qc_6').prop('checked', true);
                if (data.SPECIES_DISPLAY_TXT == 'Yes') $('.cls_art_qc .cls_chk_qc_7').prop('checked', true);
                if (data.CATCHING_AREA_DISPLAY_TXT == 'Yes') $('.cls_art_qc .cls_chk_qc_8').prop('checked', true);
                if (data.CHECK_DETAIL_DISPLAY_TXT == 'Yes') $('.cls_art_qc .cls_chk_qc_9').prop('checked', true);


                //ticket 462433 by aof start
                // aof start comment

                //$('.cls_art_qc input:radio[name=rdoConfirm_qc]').filter('[value=' + data.IS_CONFIRED + ']').prop('checked', true);

                //if (data.NUTRITION == 'X') {
                //    if (editor1 == null)
                //        editor1 = new Quill('.cls_art_qc .cls_remark_nutri_txt_comment_qc', {
                //            modules: { toolbar: toolbarOptions },
                //            theme: 'snow'
                //        });
                //    if (data.NUTRITION_COMMENT != null)
                //        editor1.clipboard.dangerouslyPasteHTML(data.NUTRITION_COMMENT);

                //    $('.cls_art_qc #chNut').prop('checked', true);
                //    $('.cls_art_qc .cls_row_nutri').show();
                //}
                //else
                //    if (editor1 == null)
                //        editor1 = bind_text_editor('.cls_art_qc .cls_remark_nutri_txt_comment_qc');

                //if (data.INGREDIENTS == 'X') {
                //    if (editor2 == null)
                //        editor2 = new Quill('.cls_art_qc .cls_remark_ingre_txt_comment_qc', {
                //            modules: { toolbar: toolbarOptions },
                //            theme: 'snow'
                //        });
                //    if (data.INGREDIENTS_COMMENT != null)
                //        editor2.clipboard.dangerouslyPasteHTML(data.INGREDIENTS_COMMENT);

                //    $('.cls_art_qc #chIng').prop('checked', true);
                //    $('.cls_art_qc .cls_row_ingre').show();
                //}
                //else
                //    if (editor2 == null)
                //        editor2 = bind_text_editor('.cls_art_qc .cls_remark_ingre_txt_comment_qc');

                //if (data.ANALYSIS == 'X') {
                //    if (editor3 == null)
                //        editor3 = new Quill('.cls_art_qc .cls_remark_analysis_txt_comment_qc', {
                //            modules: { toolbar: toolbarOptions },
                //            theme: 'snow'
                //        });
                //    if (data.ANALYSIS_COMMENT != null)
                //        editor3.clipboard.dangerouslyPasteHTML(data.ANALYSIS_COMMENT);

                //    $('.cls_art_qc #chResultAnalysis').prop('checked', true);
                //    $('.cls_art_qc .cls_row_analysis').show();
                //}
                //else
                //    if (editor3 == null)
                //        editor3 = bind_text_editor('.cls_art_qc .cls_remark_analysis_txt_comment_qc');

                //if (data.HEALTH_CLAIM == 'X') {
                //    if (editor4 == null)
                //        editor4 = new Quill('.cls_art_qc .cls_remark_health_txt_comment_qc', {
                //            modules: { toolbar: toolbarOptions },
                //            theme: 'snow'
                //        });
                //    if (data.HEALTH_CLAIM_COMMENT != null)
                //        editor4.clipboard.dangerouslyPasteHTML(data.HEALTH_CLAIM_COMMENT);

                //    $('.cls_art_qc #chResultHealthClaim').prop('checked', true);
                //    $('.cls_art_qc .cls_row_health').show();
                //}
                //else
                //    if (editor4 == null)
                //        editor4 = bind_text_editor('.cls_art_qc .cls_remark_health_txt_comment_qc');

                //if (data.NUTRIENT_CLAIM == 'X') {
                //    if (editor5 == null)
                //        editor5 = new Quill('.cls_art_qc .cls_remark_nutclaim_txt_comment_qc', {
                //            modules: { toolbar: toolbarOptions },
                //            theme: 'snow'
                //        });
                //    if (data.NUTRIENT_CLAIM_COMMENT != null)
                //        editor5.clipboard.dangerouslyPasteHTML(data.NUTRIENT_CLAIM_COMMENT);

                //    $('.cls_art_qc #chResultNutrientClaim').prop('checked', true);
                //    $('.cls_art_qc .cls_row_nutclaim').show();
                //}
                //else
                //    if (editor5 == null)
                //        editor5 = bind_text_editor('.cls_art_qc .cls_remark_nutclaim_txt_comment_qc');

                //if (data.SPECIES == 'X') {
                //    if (editor6 == null)
                //        editor6 = new Quill('.cls_art_qc .cls_remark_species_txt_comment_qc', {
                //            modules: { toolbar: toolbarOptions },
                //            theme: 'snow'
                //        });
                //    if (data.SPECIES_COMMENT != null)
                //        editor6.clipboard.dangerouslyPasteHTML(data.SPECIES_COMMENT);

                //    $('.cls_art_qc #chResultSpecies').prop('checked', true);
                //    $('.cls_art_qc .cls_row_species').show();
                //}
                //else
                //    if (editor6 == null)
                //        editor6 = bind_text_editor('.cls_art_qc .cls_remark_species_txt_comment_qc');

                //if (data.CATCHING_AREA == 'X') {
                //    if (editor7 == null)
                //        editor7 = new Quill('.cls_art_qc .cls_remark_catching_txt_comment_qc', {
                //            modules: { toolbar: toolbarOptions },
                //            theme: 'snow'
                //        });
                //    if (data.CATCHING_AREA_COMMENT != null)
                //        editor7.clipboard.dangerouslyPasteHTML(data.CATCHING_AREA_COMMENT);

                //    $('.cls_art_qc #chResultCatching').prop('checked', true);
                //    $('.cls_art_qc .cls_row_catching').show();
                //}
                //else
                //    if (editor7 == null)
                //        editor7 = bind_text_editor('.cls_art_qc .cls_remark_catching_txt_comment_qc');

                //if (data.CHECK_DETAIL == 'X') {
                //    if (editor8 == null)
                //        editor8 = new Quill('.cls_art_qc .cls_remark_check_detail_txt_comment_qc', {
                //            modules: { toolbar: toolbarOptions },
                //            theme: 'snow'
                //        });
                //    if (data.CHECK_DETAIL_COMMENT != null)
                //        editor8.clipboard.dangerouslyPasteHTML(data.CHECK_DETAIL_COMMENT);

                //    $('.cls_art_qc #chResultCheckdetail').prop('checked', true);
                //    $('.cls_art_qc .cls_row_check_detail').show();
                //}
                //else
                //    if (editor8 == null)
                //        editor8 = bind_text_editor('.cls_art_qc .cls_remark_check_detail_txt_comment_qc');
                // aof finish comment

                // aof start rewrite
                $('.cls_art_qc input:radio[name=rdoConfirm_qc]').filter('[value=' + data.DEFALUT_QC_IS_CONFIRED + ']').prop('checked', true);
               


                if (data.DEFALUT_QC_IS_CONFIRED == 0)
                {
                    $('.cls_art_qc input[name=chk_result_qc]').prop("disabled", false);
                }

              
                //alert(ReadOnly);


                if (data.DEFALUT_QC_NUTRITION == 'X') {
                    if (editor1 == null)
                        editor1 = new Quill('.cls_art_qc .cls_remark_nutri_txt_comment_qc', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.DEFALUT_QC_NUTRITION_COMMENT != null)
                        editor1.clipboard.dangerouslyPasteHTML(data.DEFALUT_QC_NUTRITION_COMMENT);

                    $('.cls_art_qc #chNut').prop('checked', true);
                    $('.cls_art_qc .cls_row_nutri').show();
                   // editor1.enable(false);
                   
                }
                else
                    if (editor1 == null)
                        editor1 = bind_text_editor('.cls_art_qc .cls_remark_nutri_txt_comment_qc');

                if (data.DEFALUT_QC_INGREDIENTS == 'X') {
                    if (editor2 == null)
                        editor2 = new Quill('.cls_art_qc .cls_remark_ingre_txt_comment_qc', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.DEFALUT_QC_INGREDIENTS_COMMENT != null)
                        editor2.clipboard.dangerouslyPasteHTML(data.DEFALUT_QC_INGREDIENTS_COMMENT);

                    $('.cls_art_qc #chIng').prop('checked', true);
                    $('.cls_art_qc .cls_row_ingre').show();
                }
                else
                    if (editor2 == null)
                        editor2 = bind_text_editor('.cls_art_qc .cls_remark_ingre_txt_comment_qc');

                if (data.DEFALUT_QC_ANALYSIS == 'X') {
                    if (editor3 == null)
                        editor3 = new Quill('.cls_art_qc .cls_remark_analysis_txt_comment_qc', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.DEFALUT_QC_ANALYSIS_COMMENT != null)
                        editor3.clipboard.dangerouslyPasteHTML(data.DEFALUT_QC_ANALYSIS_COMMENT);

                    $('.cls_art_qc #chResultAnalysis').prop('checked', true);
                    $('.cls_art_qc .cls_row_analysis').show();
                }
                else
                    if (editor3 == null)
                        editor3 = bind_text_editor('.cls_art_qc .cls_remark_analysis_txt_comment_qc');

                if (data.DEFALUT_QC_HEALTH_CLAIM == 'X') {
                    if (editor4 == null)
                        editor4 = new Quill('.cls_art_qc .cls_remark_health_txt_comment_qc', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.DEFALUT_QC_HEALTH_CLAIM_COMMENT != null)
                        editor4.clipboard.dangerouslyPasteHTML(data.DEFALUT_QC_HEALTH_CLAIM_COMMENT);

                    $('.cls_art_qc #chResultHealthClaim').prop('checked', true);
                    $('.cls_art_qc .cls_row_health').show();
                }
                else
                    if (editor4 == null)
                        editor4 = bind_text_editor('.cls_art_qc .cls_remark_health_txt_comment_qc');

                if (data.DEFALUT_QC_NUTRIENT_CLAIM == 'X') {
                    if (editor5 == null)
                        editor5 = new Quill('.cls_art_qc .cls_remark_nutclaim_txt_comment_qc', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.DEFALUT_QC_NUTRIENT_CLAIM_COMMENT != null)
                        editor5.clipboard.dangerouslyPasteHTML(data.DEFALUT_QC_NUTRIENT_CLAIM_COMMENT);

                    $('.cls_art_qc #chResultNutrientClaim').prop('checked', true);
                    $('.cls_art_qc .cls_row_nutclaim').show();
                }
                else
                    if (editor5 == null)
                        editor5 = bind_text_editor('.cls_art_qc .cls_remark_nutclaim_txt_comment_qc');

                if (data.DEFALUT_QC_SPECIES == 'X') {
                    if (editor6 == null)
                        editor6 = new Quill('.cls_art_qc .cls_remark_species_txt_comment_qc', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.DEFALUT_QC_SPECIES_COMMENT != null)
                        editor6.clipboard.dangerouslyPasteHTML(data.DEFALUT_QC_SPECIES_COMMENT);

                    $('.cls_art_qc #chResultSpecies').prop('checked', true);
                    $('.cls_art_qc .cls_row_species').show();
                }
                else
                    if (editor6 == null)
                        editor6 = bind_text_editor('.cls_art_qc .cls_remark_species_txt_comment_qc');

                if (data.DEFALUT_QC_CATCHING_AREA == 'X') {
                    if (editor7 == null)
                        editor7 = new Quill('.cls_art_qc .cls_remark_catching_txt_comment_qc', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.DEFALUT_QC_CATCHING_AREA_COMMENT != null)
                        editor7.clipboard.dangerouslyPasteHTML(data.DEFALUT_QC_CATCHING_AREA_COMMENT);

                    $('.cls_art_qc #chResultCatching').prop('checked', true);
                    $('.cls_art_qc .cls_row_catching').show();
                }
                else
                    if (editor7 == null)
                        editor7 = bind_text_editor('.cls_art_qc .cls_remark_catching_txt_comment_qc');

                if (data.DEFALUT_QC_CHECK_DETAIL == 'X') {
                    if (editor8 == null)
                        editor8 = new Quill('.cls_art_qc .cls_remark_check_detail_txt_comment_qc', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.DEFALUT_QC_CHECK_DETAIL_COMMENT != null)
                        editor8.clipboard.dangerouslyPasteHTML(data.DEFALUT_QC_CHECK_DETAIL_COMMENT);

                    $('.cls_art_qc #chResultCheckdetail').prop('checked', true);
                    $('.cls_art_qc .cls_row_check_detail').show();
                }
                else
                    if (editor8 == null)
                        editor8 = bind_text_editor('.cls_art_qc .cls_remark_check_detail_txt_comment_qc');


                //start #INC-77873 by aof on 20/09/2022
                // $('.cls_art_qc .cls_qc_txt_comment').val(data.DEFALUT_QC_COMMENT); 
                if (data.DEFALUT_QC_COMMENT != null && data.DEFALUT_QC_COMMENT != "")
                {
                    $('.cls_art_qc .cls_qc_txt_comment').val(data.DEFALUT_QC_COMMENT);
                }
                //end #INC-77873 by aof on 20/09/2022

                if (ReadOnly == "1") {
                    editor1.enable(false);
                    editor2.enable(false);
                    editor3.enable(false);
                    editor4.enable(false);
                    editor5.enable(false);
                    editor6.enable(false);
                    editor7.enable(false);
                    editor8.enable(false);
                }

                // aof finish rewrite
                //ticket 462433 by aof finish
                

            }
        },
    });

    table_qc_log_art.on('order.dt search.dt', function () {
        table_qc_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_qc_after_cus_log_art;
function bind_art_qc_after_cus_art() {
    table_qc_after_cus_log_art = $('#table_qc_after_cus_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/mk/sendtoqc?data.artwork_item_id=' + ARTWORK_ITEM_ID + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "paging": false,
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
            { "data": "PREV_STEP_DISPLAY", "className": "" },
            { "data": "DECISION_FORMLABEL_DISPLAY", "className": "" },
            { "data": "COMMENT_FORMLABEL_DISPLAY", "className": "" },
            { "data": "DECISION_NONCOMPLIANCE_DISPLAY", "className": "" },
            { "data": "COMMENT_NONCOMPLIANCE_DISPLAY", "className": "" },
            { "data": "DECISION_ADJUST_DISPLAY", "className": "" },
            { "data": "COMMENT_ADJUST_DISPLAY", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "cls_nowrap" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "APPROVE", "className": "cls_nowrap" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(13).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));
            debugger;
            is_check_verify = data.IS_CHECK_VERIFY;
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {

                if (data.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REQ_REF" || data.PREV_STEP_DISPLAY == "GM QC review customer's reference letter") {
                    $('.cls_art_qc_after_cus .cls_no_gm_qc').hide();
                    qc_check_prev = 'REQ_CUS_REQ_REF';
                }
                else {
                    $('.cls_art_qc_after_cus .cls_no_gm_qc').show();
                    $('.cls_art_qc_after_cus .cls_qc_after_for_review').hide();
                    qc_check_prev = 'REQ_CUS_REVIEW';
                }


                if (data.IS_FORMLABEL == 'X') {
                    if (data.COMMENT_CHANGE_DETAIL != "-")
                        $('.cls_art_qc_after_cus .cls_customer_txt_comment_sender_1_pa').text(data.COMMENT_CHANGE_DETAIL);
                    $('.cls_art_qc_after_cus .cls_customer_txt_comment_sender_11').html(data.COMMENT_FORMLABEL_DISPLAY);
                    $('.cls_art_qc_after_cus .cls_body_form_label').show();
                }
                else
                    $('.cls_art_qc_after_cus .cls_body_form_label').hide();

                if (data.IS_CHANGEDETAIL == 'X') {
                    $('.cls_art_qc_after_cus .cls_body_detailqc').show();
                }
                else
                    $('.cls_art_qc_after_cus .cls_body_detailqc').hide();

                if (data.IS_NONCOMPLIANCE == 'X') {
                    if (data.COMMENT_NONCOMPLIANCE != "-")
                        $('.cls_art_qc_after_cus .cls_customer_txt_comment_sender_12_pa').text(data.COMMENT_NONCOMPLIANCE);
                    $('.cls_art_qc_after_cus .cls_customer_txt_comment_sender_12').html(data.COMMENT_NONCOMPLIANCE_DISPLAY);
                    $('.cls_art_qc_after_cus .cls_body_noncompliance').show();
                }
                else
                    $('.cls_art_qc_after_cus .cls_body_noncompliance').hide();

                if (data.IS_ADJUST == 'X') {
                    if (data.COMMENT_ADJUST != "-")
                        $('.cls_art_qc_after_cus .cls_customer_txt_comment_sender_2_pa').text(data.COMMENT_ADJUST);
                    $('.cls_art_qc_after_cus .cls_customer_txt_comment_sender_2').html(data.COMMENT_ADJUST_DISPLAY);
                    $('.cls_art_qc_after_cus .cls_body_adjust').show();
                }
                else
                    $('.cls_art_qc_after_cus .cls_body_adjust').hide();


                if (data.NUTRITION_COMMENT_DISPLAY != "-" && data.NUTRITION_COMMENT_DISPLAY != null) {
                    $('.cls_art_qc_after_cus .cls_remark_nutri_txt_comment_cus').html(data.NUTRITION_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_qc_after_cus .cls_nutri_qc').hide();
                if (data.INGREDIENTS_COMMENT_DISPLAY != "-" && data.INGREDIENTS_COMMENT_DISPLAY != null) {
                    $('.cls_art_qc_after_cus .cls_remark_ingre_txt_comment_cus').html(data.INGREDIENTS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_qc_after_cus .cls_ingri_qc').hide();
                if (data.ANALYSIS_COMMENT_DISPLAY != "-" && data.ANALYSIS_COMMENT_DISPLAY != null) {
                    $('.cls_art_qc_after_cus .cls_remark_analysis_txt_comment_cus').html(data.ANALYSIS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_qc_after_cus .cls_analysis_qc').hide();
                if (data.HEALTH_CLAIM_COMMENT_DISPLAY != "-" && data.HEALTH_CLAIM_COMMENT_DISPLAY != null) {
                    $('.cls_art_qc_after_cus .cls_remark_health_txt_comment_cus').html(data.HEALTH_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_qc_after_cus .cls_health_qc').hide();
                if (data.NUTRIENT_CLAIM_COMMENT_DISPLAY != "-" && data.NUTRIENT_CLAIM_COMMENT_DISPLAY != null) {
                    $('.cls_art_qc_after_cus .cls_remark_nutclaim_txt_comment_cus').html(data.NUTRIENT_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_qc_after_cus .cls_nutri_claim_qc').hide();
                if (data.SPECIES_COMMENT_DISPLAY != "-" && data.SPECIES_COMMENT_DISPLAY != null) {
                    $('.cls_art_qc_after_cus .cls_remark_species_txt_comment_cus').html(data.SPECIES_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_qc_after_cus .cls_species_qc').hide();
                if (data.CATCHING_AREA_COMMENT_DISPLAY != "-" && data.CATCHING_AREA_COMMENT_DISPLAY != null) {
                    $('.cls_art_qc_after_cus .cls_remark_catching_txt_comment_cus').html(data.CATCHING_AREA_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_qc_after_cus .cls_catch_fao_qc').hide();

                if (data.CHECK_DETAIL_COMMENT_DISPLAY != "-" && data.CHECK_DETAIL_COMMENT_DISPLAY != null) {
                    $('.cls_art_qc_after_cus .cls_remark_check_detail_txt_comment_cus').html(data.CHECK_DETAIL_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_qc_after_cus .cls_check_detail_qc').hide();

                if (data.QC_COMMENT != "-" && data.QC_COMMENT != null) {
                    $('.cls_art_qc_after_cus .cls_remark_comment_qc_cus').text(data.QC_COMMENT);
                }
                else
                    $('.cls_art_qc_after_cus .cls_qccomment_qc').hide();


                if (data.DECISION_FORMLABEL_DISPLAY == "Confirm to change")
                    $(".cls_art_qc_after_cus input[name=rdoChange_qc_11][value=1]").prop('checked', true);
                else if (data.DECISION_FORMLABEL_DISPLAY == "Do not change")
                    $(".cls_art_qc_after_cus input[name=rdoChange_qc_11][value=0]").prop('checked', true);

                if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Confirm to change")
                    $(".cls_art_qc_after_cus input[name=rdoChange_qc_12][value=1]").prop('checked', true);
                else if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Do not change")
                    $(".cls_art_qc_after_cus input[name=rdoChange_qc_12][value=0]").prop('checked', true);

                if (data.DECISION_ADJUST_DISPLAY == "Confirm to change")
                    $(".cls_art_qc_after_cus input[name=rdoChange_qc_2][value=1]").prop('checked', true);
                else if (data.DECISION_ADJUST_DISPLAY == "Do not change")
                    $(".cls_art_qc_after_cus input[name=rdoChange_qc_2][value=0]").prop('checked', true);

                if (data.APPROVE == "Approve")
                    $(".cls_art_qc_after_cus input[name=rdoApprove_qc][value=1]").prop('checked', true);
                else if (data.APPROVE == "Not Approve")
                    $(".cls_art_qc_after_cus input[name=rdoApprove_qc][value=0]").prop('checked', true);

                $('.cls_art_qc_after_cus .cls_customer_txt_remark_sender').html(data.COMMENT_BY_PA);
                $('.cls_art_qc_after_cus .cls_qc_txt_comment').val(data.COMMENT);
            }
        },
    });

    table_qc_after_cus_log_art.on('order.dt search.dt', function () {
        table_qc_after_cus_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_art_qc(ACTION_CODE, EndTaskForm, NoNeedSubmit) {
    var jsonObj = new Object();
    var item = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;

    if ($(".cls_art_qc input[name='rdoConfirm_qc']").is(":checked"))
        item["IS_CONFIRED"] = $(".cls_art_qc input[name='rdoConfirm_qc']:checked").val();
    if ($('.cls_art_qc #chNut').is(":checked")) {
        item["NUTRITION"] = "X";
        //item["NUTRITION_COMMENT"] = $('.cls_art_qc .cls_remark_nutri_txt_comment').val();
        var editor_qc_nutri = new Quill('.cls_art_qc .cls_remark_nutri_txt_comment_qc');
        item["NUTRITION_COMMENT"] = editor_qc_nutri.root.innerHTML;
    }
    if ($('.cls_art_qc #chIng').is(":checked")) {
        item["INGREDIENTS"] = "X";
        //item["INGREDIENTS_COMMENT"] = $('.cls_art_qc .cls_remark_ingre_txt_comment').val();
        var editor_qc_ingri = new Quill('.cls_art_qc .cls_remark_ingre_txt_comment_qc');
        item["INGREDIENTS_COMMENT"] = editor_qc_ingri.root.innerHTML;
    }
    if ($('.cls_art_qc #chResultAnalysis').is(":checked")) {
        item["ANALYSIS"] = "X";
        //item["ANALYSIS_COMMENT"] = $('.cls_art_qc .cls_remark_analysis_txt_comment').val();
        var editor_qc_analysis = new Quill('.cls_art_qc .cls_remark_analysis_txt_comment_qc');
        item["ANALYSIS_COMMENT"] = editor_qc_analysis.root.innerHTML;
    }
    if ($('.cls_art_qc #chResultHealthClaim').is(":checked")) {
        item["HEALTH_CLAIM"] = "X";
        //item["HEALTH_CLAIM_COMMENT"] = $('.cls_art_qc .cls_remark_health_txt_comment').val();
        var editor_qc_health = new Quill('.cls_art_qc .cls_remark_health_txt_comment_qc');
        item["HEALTH_CLAIM_COMMENT"] = editor_qc_health.root.innerHTML;
    }
    if ($('.cls_art_qc #chResultNutrientClaim').is(":checked")) {
        item["NUTRIENT_CLAIM"] = "X";
        //item["NUTRIENT_CLAIM_COMMENT"] = $('.cls_art_qc .cls_remark_nutclaim_txt_comment').val();
        var editor_qc_nutclaim = new Quill('.cls_art_qc .cls_remark_nutclaim_txt_comment_qc');
        item["NUTRIENT_CLAIM_COMMENT"] = editor_qc_nutclaim.root.innerHTML;
    }
    if ($('.cls_art_qc #chResultSpecies').is(":checked")) {
        item["SPECIES"] = "X";
        //item["SPECIES_COMMENT"] = $('.cls_art_qc .cls_remark_species_txt_comment').val();
        var editor_qc_species = new Quill('.cls_art_qc .cls_remark_species_txt_comment_qc');
        item["SPECIES_COMMENT"] = editor_qc_species.root.innerHTML;
    }
    if ($('.cls_art_qc #chResultCatching').is(":checked")) {
        item["CATCHING_AREA"] = "X";
        //item["CATCHING_AREA_COMMENT"] = $('.cls_art_qc .cls_remark_catching_txt_comment').val();
        var editor_qc_catching = new Quill('.cls_art_qc .cls_remark_catching_txt_comment_qc');
        item["CATCHING_AREA_COMMENT"] = editor_qc_catching.root.innerHTML;
    }

    if ($('.cls_art_qc #chResultCheckdetail').is(":checked")) {
        item["CHECK_DETAIL"] = "X";
        //item["CHECK_DETAIL_COMMENT"] = $('.cls_art_qc .cls_remark_catching_txt_comment').val();
        var editor_qc_check_detail = new Quill('.cls_art_qc .cls_remark_check_detail_txt_comment_qc');
        item["CHECK_DETAIL_COMMENT"] = editor_qc_check_detail.root.innerHTML;
    }

    item["REASON_ID"] = $(".cls_art_qc .cls_lov_send_for_reason").val();
    item["REMARK_REASON"] = $(".cls_art_qc .cls_input_qc_pa_other").val();
    item["WF_STEP"] = getstepartwork('SEND_QC').curr_step;

    item["COMMENT"] = $('.cls_art_qc .cls_qc_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["ENDTASKFORM"] = EndTaskForm;

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    jsonObj.data = item;

    var myurl = '/api/taskform/pa/sendqctopa';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (NoNeedSubmit)
        myAjax(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else if (EndTaskForm)
         // tikcet 437531 by aof
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
        //myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage); 
        // tikcet 437531 by aof
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}

var table_rd_log_art;
function bind_art_rd_art() {
    var comment_RD;
    table_rd_log_art = $('#table_rd_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/qc/sendtord?data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "paging": false,
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
            { "data": "REASON_BY_PA", "className": "" },
            { "data": "REMARK_REASON_BY_PA", "className": "" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON_BY_OTHER", "className": "" },
            { "data": "IS_CONFIRED_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "NUTRITION_RD_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "NUTRITION_COMMENT", "className": "" },
            { "data": "HEALTH_CLAIM_RD_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "HEALTH_CLAIM_COMMENT", "className": "" },
            { "data": "CATCHING_AREA_RD_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "CATCHING_AREA_COMMENT", "className": "" },
            { "data": "INGREDIENTS_RD_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "INGREDIENTS_COMMENT", "className": "" },
            { "data": "NUTRIENT_CLAIM_RD_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "NUTRIENT_CLAIM_COMMENT", "className": "" },
            { "data": "ANALYSIS_RD_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "ANALYSIS_COMMENT", "className": "" },
            { "data": "SPECIES_RD_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "SPECIES_COMMENT", "className": "" },
            { "data": "CHECK_DETAIL_RD_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "CHECK_DETAIL_COMMENT", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(25).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(3).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_rd .cls_rd_txt_remark').html(data.COMMENT_BY_PA);
                $('.cls_art_rd .cls_rd_txt_comment').val(data.COMMENT);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_art_rd .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);

                if (data.NUTRITION_DISPLAY_TXT == 'Yes') $('.cls_art_rd .cls_chk_rd_2').prop('checked', true);
                if (data.INGREDIENTS_DISPLAY_TXT == 'Yes') $('.cls_art_rd .cls_chk_rd_3').prop('checked', true);
                if (data.ANALYSIS_DISPLAY_TXT == 'Yes') $('.cls_art_rd .cls_chk_rd_4').prop('checked', true);
                if (data.HEALTH_CLAIM_DISPLAY_TXT == 'Yes') $('.cls_art_rd .cls_chk_rd_5').prop('checked', true);
                if (data.NUTRIENT_CLAIM_DISPLAY_TXT == 'Yes') $('.cls_art_rd .cls_chk_rd_6').prop('checked', true);
                if (data.SPECIES_DISPLAY_TXT == 'Yes') $('.cls_art_rd .cls_chk_rd_7').prop('checked', true);
                if (data.CATCHING_AREA_DISPLAY_TXT == 'Yes') $('.cls_art_rd .cls_chk_rd_8').prop('checked', true);
                if (data.CHECK_DETAIL_DISPLAY_TXT == 'Yes') $('.cls_art_rd .cls_chk_rd_9').prop('checked', true);

                $('.cls_art_rd input:radio[name=rdoConfirm_rd]').filter('[value=' + data.IS_CONFIRED + ']').prop('checked', true);

                if (data.NUTRITION == 'X') {
                    if (editor1 == null)
                        editor1 = new Quill('.cls_art_rd .cls_remark_nutri_txt_comment_rd', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.NUTRITION_COMMENT != null)
                        editor1.clipboard.dangerouslyPasteHTML(data.NUTRITION_COMMENT);

                    $('.cls_art_rd #chNut').prop('checked', true);
                    $('.cls_art_rd .cls_row_nutri').show();
                }
                else
                    if (editor1 == null)
                        editor1 = bind_text_editor('.cls_art_rd .cls_remark_nutri_txt_comment_rd');

                if (data.INGREDIENTS == 'X') {
                    if (editor2 == null)
                        editor2 = new Quill('.cls_art_rd .cls_remark_ingre_txt_comment_rd', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.INGREDIENTS_COMMENT != null)
                        editor2.clipboard.dangerouslyPasteHTML(data.INGREDIENTS_COMMENT);

                    $('.cls_art_rd #chIng').prop('checked', true);
                    $('.cls_art_rd .cls_row_ingre').show();
                }
                else
                    if (editor2 == null)
                        editor2 = bind_text_editor('.cls_art_rd .cls_remark_ingre_txt_comment_rd');

                if (data.ANALYSIS == 'X') {
                    if (editor3 == null)
                        editor3 = new Quill('.cls_art_rd .cls_remark_analysis_txt_comment_rd', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.ANALYSIS_COMMENT != null)
                        editor3.clipboard.dangerouslyPasteHTML(data.ANALYSIS_COMMENT);

                    $('.cls_art_rd #chResultAnalysis').prop('checked', true);
                    $('.cls_art_rd .cls_row_analysis').show();
                }
                else
                    if (editor3 == null)
                        editor3 = bind_text_editor('.cls_art_rd .cls_remark_analysis_txt_comment_rd');

                if (data.HEALTH_CLAIM == 'X') {
                    if (editor4 == null)
                        editor4 = new Quill('.cls_art_rd .cls_remark_health_txt_comment_rd', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.HEALTH_CLAIM_COMMENT != null)
                        editor4.clipboard.dangerouslyPasteHTML(data.HEALTH_CLAIM_COMMENT);

                    $('.cls_art_rd #chResultHealthClaim').prop('checked', true);
                    $('.cls_art_rd .cls_row_health').show();
                }
                else
                    if (editor4 == null)
                        editor4 = bind_text_editor('.cls_art_rd .cls_remark_health_txt_comment_rd');

                if (data.NUTRIENT_CLAIM == 'X') {
                    if (editor5 == null)
                        editor5 = new Quill('.cls_art_rd .cls_remark_nutclaim_txt_comment_rd', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.NUTRIENT_CLAIM_COMMENT != null)
                        editor5.clipboard.dangerouslyPasteHTML(data.NUTRIENT_CLAIM_COMMENT);

                    $('.cls_art_rd #chResultNutrientClaim').prop('checked', true);
                    $('.cls_art_rd .cls_row_nutclaim').show();
                }
                else
                    if (editor5 == null)
                        editor5 = bind_text_editor('.cls_art_rd .cls_remark_nutclaim_txt_comment_rd');

                if (data.SPECIES == 'X') {
                    if (editor6 == null)
                        editor6 = new Quill('.cls_art_rd .cls_remark_species_txt_comment_rd', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.SPECIES_COMMENT != null)
                        editor6.clipboard.dangerouslyPasteHTML(data.SPECIES_COMMENT);

                    $('.cls_art_rd #chResultSpecies').prop('checked', true);
                    $('.cls_art_rd .cls_row_species').show();
                }
                else
                    if (editor6 == null)
                        editor6 = bind_text_editor('.cls_art_rd .cls_remark_species_txt_comment_rd');

                if (data.CATCHING_AREA == 'X') {
                    if (editor7 == null)
                        editor7 = new Quill('.cls_art_rd .cls_remark_catching_txt_comment_rd', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.CATCHING_AREA_COMMENT != null)
                        editor7.clipboard.dangerouslyPasteHTML(data.CATCHING_AREA_COMMENT);

                    $('.cls_art_rd #chResultCatching').prop('checked', true);
                    $('.cls_art_rd .cls_row_catching').show();
                }
                else
                    if (editor7 == null)
                        editor7 = bind_text_editor('.cls_art_rd .cls_remark_catching_txt_comment_rd');

                if (data.CHECK_DETAIL == 'X') {
                    if (editor8 == null)
                        editor8 = new Quill('.cls_art_rd .cls_remark_check_detail_txt_comment_rd', {
                            modules: { toolbar: toolbarOptions },
                            theme: 'snow'
                        });
                    if (data.CHECK_DETAIL_COMMENT != null)
                        editor8.clipboard.dangerouslyPasteHTML(data.CHECK_DETAIL_COMMENT);

                    $('.cls_art_rd #chResultCheckdetail').prop('checked', true);
                    $('.cls_art_rd .cls_row_check_detail').show();
                }
                else
                    if (editor8 == null)
                        editor8 = bind_text_editor('.cls_art_rd .cls_remark_check_detail_txt_comment_rd');

            }
            if (data.ARTWORK_SUB_ID == data.ARTWORK_SUB_ID_RD && CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_QC" && data.PARENT_RD_ID == ArtworkSubId) {

                //ticket 462433 by aof on 20210306  start
                //$('.cls_art_qc input:radio[name=rdoConfirm_qc]').filter('[value=' + data.IS_CONFIRED + ']').prop('checked', true);
                //if (data.IS_CONFIRED == 0)
                //    $('.cls_art_qc input[name=chk_result_qc]').prop("disabled", false);

                //if (data.NUTRITION != null) {

                //    $('.cls_art_qc #chNut').prop('checked', true);
                //    $('.cls_art_qc .cls_row_nutri').show();
                //    if (data.NUTRITION_COMMENT != null) {
                //        if (editor1 == null)
                //            editor1 = new Quill('.cls_art_qc .cls_remark_nutri_txt_comment_qc', {
                //                modules: { toolbar: toolbarOptions },
                //                theme: 'snow'
                //            });
                //        editor1.clipboard.dangerouslyPasteHTML(data.NUTRITION_COMMENT);
                //    }
                //}
                //if (data.INGREDIENTS != null) {

                //    $('.cls_art_qc #chIng').prop('checked', true);
                //    $('.cls_art_qc .cls_row_ingre').show();
                //    if (data.INGREDIENTS_COMMENT != null) {
                //        if (editor2 == null)
                //            editor2 = new Quill('.cls_art_qc .cls_remark_ingre_txt_comment_qc', {
                //                modules: { toolbar: toolbarOptions },
                //                theme: 'snow'
                //            });
                //        editor2.clipboard.dangerouslyPasteHTML(data.INGREDIENTS_COMMENT);
                //    }
                //}
                //if (data.ANALYSIS != null) {

                //    $('.cls_art_qc #chResultAnalysis').prop('checked', true);
                //    $('.cls_art_qc .cls_row_analysis').show();
                //    if (data.ANALYSIS_COMMENT != null) {
                //        if (editor3 == null)
                //            editor3 = new Quill('.cls_art_qc .cls_remark_analysis_txt_comment_qc', {
                //                modules: { toolbar: toolbarOptions },
                //                theme: 'snow'
                //            });
                //        editor3.clipboard.dangerouslyPasteHTML(data.ANALYSIS_COMMENT);
                //    }
                //}
                //if (data.HEALTH_CLAIM != null) {

                //    $('.cls_art_qc #chResultHealthClaim').prop('checked', true);
                //    $('.cls_art_qc .cls_row_health').show();
                //    if (data.HEALTH_CLAIM_COMMENT != null) {
                //        if (editor4 == null)
                //            editor4 = new Quill('.cls_art_qc .cls_remark_health_txt_comment_qc', {
                //                modules: { toolbar: toolbarOptions },
                //                theme: 'snow'
                //            });
                //        editor4.clipboard.dangerouslyPasteHTML(data.HEALTH_CLAIM_COMMENT);
                //    }
                //}
                //if (data.NUTRIENT_CLAIM != null) {

                //    $('.cls_art_qc #chResultNutrientClaim').prop('checked', true);
                //    $('.cls_art_qc .cls_row_nutclaim').show();
                //    if (data.NUTRIENT_CLAIM_COMMENT != null) {
                //        if (editor5 == null)
                //            editor5 = new Quill('.cls_art_qc .cls_remark_nutclaim_txt_comment_qc', {
                //                modules: { toolbar: toolbarOptions },
                //                theme: 'snow'
                //            });
                //        editor5.clipboard.dangerouslyPasteHTML(data.NUTRIENT_CLAIM_COMMENT);
                //    }
                //}
                //if (data.SPECIES != null) {

                //    $('.cls_art_qc #chResultSpecies').prop('checked', true);
                //    $('.cls_art_qc .cls_row_species').show();
                //    if (data.SPECIES_COMMENT != null) {
                //        if (editor6 == null)
                //            editor6 = new Quill('.cls_art_qc .cls_remark_species_txt_comment_qc', {
                //                modules: { toolbar: toolbarOptions },
                //                theme: 'snow'
                //            });
                //        editor6.clipboard.dangerouslyPasteHTML(data.SPECIES_COMMENT);
                //    }
                //}
                //if (data.CATCHING_AREA != null) {

                //    $('.cls_art_qc #chResultCatching').prop('checked', true);
                //    $('.cls_art_qc .cls_row_catching').show();
                //    if (data.CATCHING_AREA_COMMENT != null) {
                //        if (editor7 == null)
                //            editor7 = new Quill('.cls_art_qc .cls_remark_catching_txt_comment_qc', {
                //                modules: { toolbar: toolbarOptions },
                //                theme: 'snow'
                //            });
                //        editor7.clipboard.dangerouslyPasteHTML(data.CATCHING_AREA_COMMENT);
                //    }
                //}
                //if (data.CHECK_DETAIL != null) {

                //    $('.cls_art_qc #chResultCheckdetail').prop('checked', true);
                //    $('.cls_art_qc .cls_row_check_detail').show();
                //    if (data.CHECK_DETAIL_COMMENT != null) {
                //        if (editor8 == null)
                //            editor8 = new Quill('.cls_art_qc .cls_remark_check_detail_txt_comment_qc', {
                //                modules: { toolbar: toolbarOptions },
                //                theme: 'snow'
                //            });
                //        editor8.clipboard.dangerouslyPasteHTML(data.CHECK_DETAIL_COMMENT);
                //    }
                //}
     
                //if (data.COMMENT != null) {
                //    $('.cls_art_qc .cls_qc_txt_comment').val(data.COMMENT);
                //}
                //ticket 462433 by aof on 20210306  start
            }
        },




    });

    table_rd_log_art.on('order.dt search.dt', function () {
        table_rd_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_art_rd(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;

    if (ACTION_CODE != "SEND_BACK") {
        if ($(".cls_art_rd input[name='rdoConfirm_rd']").is(":checked"))
            item["IS_CONFIRED"] = $(".cls_art_rd input[name='rdoConfirm_rd']:checked").val();
        if ($('.cls_art_rd #chNut').is(":checked")) {
            item["NUTRITION"] = "X";
            var editor_rd_nutri = new Quill('.cls_art_rd .cls_remark_nutri_txt_comment_rd');
            item["NUTRITION_COMMENT"] = editor_rd_nutri.root.innerHTML;
        }
        if ($('.cls_art_rd #chIng').is(":checked")) {
            item["INGREDIENTS"] = "X";
            //item["INGREDIENTS_COMMENT"] = $('.cls_art_rd .cls_remark_ingre_txt_comment').val();
            var editor_rd_ingri = new Quill('.cls_art_rd .cls_remark_ingre_txt_comment_rd');
            item["INGREDIENTS_COMMENT"] = editor_rd_ingri.root.innerHTML;
        }
        if ($('.cls_art_rd #chResultAnalysis').is(":checked")) {
            item["ANALYSIS"] = "X";
            //item["ANALYSIS_COMMENT"] = $('.cls_art_rd .cls_remark_analysis_txt_comment').val();
            var editor_rd_analysis = new Quill('.cls_art_rd .cls_remark_analysis_txt_comment_rd');
            item["ANALYSIS_COMMENT"] = editor_rd_analysis.root.innerHTML;
        }
        if ($('.cls_art_rd #chResultHealthClaim').is(":checked")) {
            item["HEALTH_CLAIM"] = "X";
            //item["HEALTH_CLAIM_COMMENT"] = $('.cls_art_rd .cls_remark_health_txt_comment').val();
            var editor_rd_health = new Quill('.cls_art_rd .cls_remark_health_txt_comment_rd');
            item["HEALTH_CLAIM_COMMENT"] = editor_rd_health.root.innerHTML;
        }
        if ($('.cls_art_rd #chResultNutrientClaim').is(":checked")) {
            item["NUTRIENT_CLAIM"] = "X";
            //item["NUTRIENT_CLAIM_COMMENT"] = $('.cls_art_rd .cls_remark_nutclaim_txt_comment').val();
            var editor_rd_nutclaim = new Quill('.cls_art_rd .cls_remark_nutclaim_txt_comment_rd');
            item["NUTRIENT_CLAIM_COMMENT"] = editor_rd_nutclaim.root.innerHTML;
        }
        if ($('.cls_art_rd #chResultSpecies').is(":checked")) {
            item["SPECIES"] = "X";
            //item["SPECIES_COMMENT"] = $('.cls_art_rd .cls_remark_species_txt_comment').val();
            var editor_rd_species = new Quill('.cls_art_rd .cls_remark_species_txt_comment_rd');
            item["SPECIES_COMMENT"] = editor_rd_species.root.innerHTML;
        }
        if ($('.cls_art_rd #chResultCatching').is(":checked")) {
            item["CATCHING_AREA"] = "X";
            //item["CATCHING_AREA_COMMENT"] = $('.cls_art_rd .cls_remark_catching_txt_comment').val();
            var editor_rd_catching = new Quill('.cls_art_rd .cls_remark_catching_txt_comment_rd');
            item["CATCHING_AREA_COMMENT"] = editor_rd_catching.root.innerHTML;
        }

        if ($('.cls_art_rd #chResultCheckdetail').is(":checked")) {
            item["CHECK_DETAIL"] = "X";
            //item["CHECK_DETAIL_COMMENT"] = $('.cls_art_rd .cls_remark_catching_txt_comment').val();
            var editor_rd_check_detail = new Quill('.cls_art_rd .cls_remark_check_detail_txt_comment_rd');
            item["CHECK_DETAIL_COMMENT"] = editor_rd_check_detail.root.innerHTML;
        }
    }

    item["REASON_ID"] = $(".cls_art_rd .cls_lov_send_for_reason").val();
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    item["REMARK_REASON"] = $(".cls_art_rd .cls_input_rd_pa_other").val();
    item["WF_STEP"] = getstepartwork('SEND_RD').curr_step;

    item["COMMENT"] = $('.cls_art_rd .cls_rd_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["ENDTASKFORM"] = EndTaskForm;

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1) {
        alertError2("Please select reason for overdue");
        return false;
    }

    jsonObj.data = item;

    var myurl = '/api/taskform/qc/sendtoqc';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}

var table_wh_log_art;
function bind_art_wh_art() {
    table_wh_log_art = $('#table_wh_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtowarehouse?data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "order": [[8, 'desc']],
        "processing": true,
        "lengthChange": false,
        "paging": false,
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
            { "data": "INKJET_STAMP_AREA_DISPLAY_TXT", "className": "" },
            { "data": "INKJET_STAMP_AREA", "className": "" },
            { "data": "ROLL_DIRECTION_DISPLAY_TXT", "className": "" },
            {
                render: function (data, type, row, meta) {
                    if (row.ROLL_DIRECTION != null)
                        return '<img title="Completed" style="width:100px;" src="/Content/img/roll_direction/roll_direction_' + row.ROLL_DIRECTION + '.jpg">';
                }
            },
            { "data": "REASON_BY_PA", "className": "" },
            { "data": "REMARK_REASON_BY_PA", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON_BY_OTHER", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(13).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(8).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_wh .cls_wh_sender_txt_comment').html(data.COMMENT_BY_PA);
                $('.cls_art_wh .cls_wh_txt_comment').val(data.COMMENT);
                $('.cls_art_wh .cls_chk_ink_stamp_result').val(data.INKJET_STAMP_AREA);
                $('.cls_art_wh input:radio[name=rdo_roll_direc]').filter('[value=' + data.ROLL_DIRECTION + ']').prop('checked', true);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_art_wh .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
                if (data.INKJET_STAMP_AREA_DISPLAY_TXT == "Yes")
                    $('.cls_art_wh .cls_chk_ink_stamp').show();
                if (data.ROLL_DIRECTION_DISPLAY_TXT == "Yes")
                    $('.cls_art_wh .cls_roll_direction').show();
            }
        },
    });

    table_wh_log_art.on('order.dt search.dt', function () {
        table_wh_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_art_wh(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;
    item["INKJET_STAMP_AREA"] = $('.cls_art_wh .cls_chk_ink_stamp_result').val();
    item["ROLL_DIRECTION"] = $(".cls_art_wh input[name='rdo_roll_direc']:checked").val();

    item["REASON_ID"] = $(".cls_art_wh .cls_lov_send_for_reason").val();
    item["REMARK_REASON"] = $(".cls_art_wh .cls_input_wh_pa_other").val();
    item["WF_STEP"] = getstepartwork('SEND_WH').curr_step;
    item["COMMENT"] = $('.cls_art_wh .cls_wh_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["ENDTASKFORM"] = EndTaskForm;

    if (item["REASON_ID"] == DefaultResonId && OverDue == 1) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    jsonObj.data = item;

    var myurl = '/api/taskform/internal/wh/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}


var table_pn_log_art;
function bind_art_pn_art() {
    table_pn_log_art = $('#table_pn_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtoplanning?data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "paging": false,
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
            { "data": "REASON_BY_PA", "className": "" },
            { "data": "REMARK_REASON_BY_PA", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON_BY_OTHER", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(4).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_plan .cls_plan_sender_txt_comment').html(data.COMMENT_BY_PA);
                $('.cls_art_plan .cls_plan_txt_comment').val(data.COMMENT);
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_art_plan .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
            }

        },
    });

    table_pn_log_art.on('order.dt search.dt', function () {
        table_pn_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_art_pn(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;
    item["COMMENT"] = $('.cls_art_plan .cls_plan_txt_comment').val();
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["ENDTASKFORM"] = EndTaskForm;
    item["REASON_ID"] = $(".cls_art_plan .cls_lov_send_for_reason").val();
    item["REMARK_REASON"] = $(".cls_art_plan .cls_input_plan_pa_other").val();
    item["WF_STEP"] = getstepartwork('SEND_PN').curr_step;
    if (item["REASON_ID"] == DefaultResonId && OverDue == 1) {
        alertError2("Please select reason for overdue");
        return false;
    }
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    jsonObj.data = item;

    var myurl = '/api/taskform/internal/planning/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}

var table_gm_mk_log_art;
function bind_art_gm_mk_art() {
    table_gm_mk_log_art = $('#table_gm_mk_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/mk/sendtogmmk?data.artwork_item_id=' + ARTWORK_ITEM_ID + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "order": [[11, 'desc']],
        "processing": true,
        "lengthChange": false,
        "paging": false,
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
            { "data": "DECISION_FORMLABEL_DISPLAY", "className": "" },
            { "data": "COMMENT_FORMLABEL_DISPLAY", "className": "" },
            { "data": "DECISION_NONCOMPLIANCE_DISPLAY", "className": "" },
            { "data": "COMMENT_NONCOMPLIANCE_DISPLAY", "className": "" },
            { "data": "DECISION_ADJUST_DISPLAY", "className": "" },
            { "data": "COMMENT_ADJUST_DISPLAY", "className": "" },
            { "data": "APPROVAL_GM_QC_DISPLAY", "className": "" },
            { "data": "COMMENT_GM_QC_DISPLAY", "className": "" },
            { "data": "ACTION_NAME_MK", "className": "" },
            { "data": "COMMENT_BY_MK", "className": "" },
            { "data": "CREATE_DATE_BY_MK", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(14).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(11).html(myDateTimeMoment(data.CREATE_DATE_BY_MK));
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {

                if (data.IS_FORMLABEL == 'X') {
                    if (data.COMMENT_CHANGE_DETAIL != "-")
                        $('.cls_art_gm_mk .cls_customer_txt_comment_sender_1_pa').text(data.COMMENT_CHANGE_DETAIL);
                    $('.cls_art_gm_mk .cls_customer_txt_comment_sender_11').html(data.COMMENT_FORMLABEL_DISPLAY);
                    $('.cls_art_gm_mk .cls_body_form_label').show();
                }
                else
                    $('.cls_art_gm_mk .cls_body_form_label').hide();

                if (data.IS_CHANGEDETAIL == 'X') {
                    $('.cls_art_gm_mk .cls_body_detailqc').show();
                }
                else
                    $('.cls_art_gm_mk .cls_body_detailqc').hide();

                if (data.IS_NONCOMPLIANCE == 'X') {
                    if (data.COMMENT_NONCOMPLIANCE != "-")
                        $('.cls_art_gm_mk .cls_customer_txt_comment_sender_12_pa').text(data.COMMENT_NONCOMPLIANCE);
                    $('.cls_art_gm_mk .cls_customer_txt_comment_sender_12').html(data.COMMENT_NONCOMPLIANCE_DISPLAY);
                    $('.cls_art_gm_mk .cls_body_noncompliance').show();
                }
                else
                    $('.cls_art_gm_mk .cls_body_noncompliance').hide();

                if (data.IS_ADJUST == 'X') {
                    if (data.COMMENT_ADJUST != "-")
                        $('.cls_art_gm_mk .cls_customer_txt_comment_sender_2_pa').text(data.COMMENT_ADJUST);
                    $('.cls_art_gm_mk .cls_customer_txt_comment_sender_2').html(data.COMMENT_ADJUST_DISPLAY);
                    $('.cls_art_gm_mk .cls_body_adjust').show();
                    gmmk_check_case = true;
                }
                else
                    $('.cls_art_gm_mk .cls_body_adjust').hide();


                if (data.NUTRITION_COMMENT_DISPLAY != "-" && data.NUTRITION_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_mk .cls_remark_nutri_txt_comment_cus').html(data.NUTRITION_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_mk .cls_nutri_qc').hide();
                if (data.INGREDIENTS_COMMENT_DISPLAY != "-" && data.INGREDIENTS_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_mk .cls_remark_ingre_txt_comment_cus').html(data.INGREDIENTS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_mk .cls_ingri_qc').hide();
                if (data.ANALYSIS_COMMENT_DISPLAY != "-" && data.ANALYSIS_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_mk .cls_remark_analysis_txt_comment_cus').html(data.ANALYSIS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_mk .cls_analysis_qc').hide();
                if (data.HEALTH_CLAIM_COMMENT_DISPLAY != "-" && data.HEALTH_CLAIM_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_mk .cls_remark_health_txt_comment_cus').html(data.HEALTH_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_mk .cls_health_qc').hide();
                if (data.NUTRIENT_CLAIM_COMMENT_DISPLAY != "-" && data.NUTRIENT_CLAIM_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_mk .cls_remark_nutclaim_txt_comment_cus').html(data.NUTRIENT_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_mk .cls_nutri_claim_qc').hide();
                if (data.SPECIES_COMMENT_DISPLAY != "-" && data.SPECIES_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_mk .cls_remark_species_txt_comment_cus').html(data.SPECIES_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_mk .cls_species_qc').hide();
                if (data.CATCHING_AREA_COMMENT_DISPLAY != "-" && data.CATCHING_AREA_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_mk .cls_remark_catching_txt_comment_cus').html(data.CATCHING_AREA_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_mk .cls_catch_fao_qc').hide();

                if (data.CHECK_DETAIL_COMMENT_DISPLAY != "-" && data.CHECK_DETAIL_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_mk .cls_remark_check_detail_txt_comment_cus').html(data.CHECK_DETAIL_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_mk .cls_check_detail_qc').hide();

                if (data.QC_COMMENT != "-" && data.QC_COMMENT != null) {
                    $('.cls_art_gm_mk .cls_remark_comment_qc_cus').text(data.QC_COMMENT);
                }
                else
                    $('.cls_art_gm_mk .cls_qccomment_qc').hide();


                if (data.DECISION_FORMLABEL_DISPLAY == "Confirm to change")
                    $(".cls_art_gm_mk input[name=rdoChange_gmmk_11][value=1]").prop('checked', true);
                else if (data.DECISION_FORMLABEL_DISPLAY == "Do not change")
                    $(".cls_art_gm_mk input[name=rdoChange_gmmk_11][value=0]").prop('checked', true);

                if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Confirm to change")
                    $(".cls_art_gm_mk input[name=rdoChange_gmmk_12][value=1]").prop('checked', true);
                else if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Do not change")
                    $(".cls_art_gm_mk input[name=rdoChange_gmmk_12][value=0]").prop('checked', true);

                if (data.DECISION_ADJUST_DISPLAY == "Confirm to change")
                    $(".cls_art_gm_mk input[name=rdoChange_gmmk_2][value=1]").prop('checked', true);
                else if (data.DECISION_ADJUST_DISPLAY == "Do not change")
                    $(".cls_art_gm_mk input[name=rdoChange_gmmk_2][value=0]").prop('checked', true);

                if (data.APPROVE == "Approve")
                    $(".cls_art_gm_mk input[name=rdoApprove_gmmk][value=1]").prop('checked', true);
                else if (data.APPROVE == "Not Approve")
                    $(".cls_art_gm_mk input[name=rdoApprove_gmmk][value=0]").prop('checked', true);

                $('.cls_art_gm_mk .cls_gm_mk_sender_txt_comment').html(data.COMMENT_BY_MK);
                $('.cls_art_gm_mk .cls_gm_mk_txt_comment').val(data.COMMENT);
            }

        },
    });

    table_gm_mk_log_art.on('order.dt search.dt', function () {
        table_gm_mk_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_art_gm_mk(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;
    if (gmmk_check_case)
        item["IS_ADJUST"] = "X";
    item["ENDTASKFORM"] = EndTaskForm;
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;
    item["COMMENT"] = $('.cls_art_gm_mk .cls_gm_mk_txt_comment').val();


    jsonObj.data = item;

    var myurl = '/api/taskform/mk/sendtopabygmmk';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm) {
        if (item["ACTION_CODE"] == "NOTAPPROVE" && item["IS_ADJUST"] != "X")
            myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, 'Do you want to submit to MK?');
        else if (item["ACTION_CODE"] == "SEND_BACK")
            myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
        else
            myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, 'Do you want to submit to PA?');
    }
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}


var table_gm_qc_log_art;
function bind_art_gm_qc_art() {
    table_gm_qc_log_art = $('#table_gm_qc_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/qc/sendtogmqc?data.artwork_item_id=' + ARTWORK_ITEM_ID + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "order": [[10, 'desc']],
        "processing": true,
        "lengthChange": false,
        "paging": false,
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
            { "data": "PREV_STEP", "className": "" },
            { "data": "DECISION_FORMLABEL_DISPLAY", "className": "" },
            { "data": "COMMENT_FORMLABEL_DISPLAY", "className": "" },
            { "data": "DECISION_NONCOMPLIANCE_DISPLAY", "className": "" },
            { "data": "COMMENT_NONCOMPLIANCE_DISPLAY", "className": "" },
            { "data": "DECISION_ADJUST_DISPLAY", "className": "" },
            { "data": "COMMENT_ADJUST_DISPLAY", "className": "" },
            { "data": "ACTION_NAME_SENDER", "className": "" },
            { "data": "COMMENT_BY_QC", "className": "" },
            { "data": "CREATE_DATE_BY_QC", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(13).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE_BY_QC));
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {

                if (data.IS_FORMLABEL == 'X') {
                    if (data.COMMENT_CHANGE_DETAIL != "-")
                        $('.cls_art_gm_qc .cls_customer_txt_comment_sender_1_pa').text(data.COMMENT_CHANGE_DETAIL);
                    $('.cls_art_gm_qc .cls_customer_txt_comment_sender_11').html(data.COMMENT_FORMLABEL_DISPLAY);
                    $('.cls_art_gm_qc .cls_body_form_label').show();
                }
                else
                    $('.cls_art_gm_qc .cls_body_form_label').hide();

                if (data.IS_CHANGEDETAIL == 'X') {
                    $('.cls_art_gm_qc .cls_body_detailqc').show();
                }
                else
                    $('.cls_art_gm_qc .cls_body_detailqc').hide();

                if (data.IS_NONCOMPLIANCE == 'X') {
                    if (data.COMMENT_NONCOMPLIANCE != "-")
                        $('.cls_art_gm_qc .cls_customer_txt_comment_sender_12_pa').text(data.COMMENT_NONCOMPLIANCE);
                    $('.cls_art_gm_qc .cls_customer_txt_comment_sender_12').html(data.COMMENT_NONCOMPLIANCE_DISPLAY);
                    $('.cls_art_gm_qc .cls_body_noncompliance').show();
                }
                else
                    $('.cls_art_gm_qc .cls_body_noncompliance').hide();

                if (data.IS_ADJUST == 'X') {
                    if (data.COMMENT_ADJUST != "-")
                        $('.cls_art_gm_qc .cls_customer_txt_comment_sender_2_pa').text(data.COMMENT_ADJUST);
                    $('.cls_art_gm_qc .cls_customer_txt_comment_sender_2').html(data.COMMENT_ADJUST_DISPLAY);
                    $('.cls_art_gm_qc .cls_body_adjust').show();
                }
                else
                    $('.cls_art_gm_qc .cls_body_adjust').hide();


                if (data.NUTRITION_COMMENT_DISPLAY != "-" && data.NUTRITION_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_qc .cls_remark_nutri_txt_comment_cus').html(data.NUTRITION_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_qc .cls_nutri_qc').hide();
                if (data.INGREDIENTS_COMMENT_DISPLAY != "-" && data.INGREDIENTS_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_qc .cls_remark_ingre_txt_comment_cus').html(data.INGREDIENTS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_qc .cls_ingri_qc').hide();
                if (data.ANALYSIS_COMMENT_DISPLAY != "-" && data.ANALYSIS_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_qc .cls_remark_analysis_txt_comment_cus').html(data.ANALYSIS_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_qc .cls_analysis_qc').hide();
                if (data.HEALTH_CLAIM_COMMENT_DISPLAY != "-" && data.HEALTH_CLAIM_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_qc .cls_remark_health_txt_comment_cus').html(data.HEALTH_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_qc .cls_health_qc').hide();
                if (data.NUTRIENT_CLAIM_COMMENT_DISPLAY != "-" && data.NUTRIENT_CLAIM_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_qc .cls_remark_nutclaim_txt_comment_cus').html(data.NUTRIENT_CLAIM_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_qc .cls_nutri_claim_qc').hide();
                if (data.SPECIES_COMMENT_DISPLAY != "-" && data.SPECIES_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_qc .cls_remark_species_txt_comment_cus').html(data.SPECIES_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_qc .cls_species_qc').hide();
                if (data.CATCHING_AREA_COMMENT_DISPLAY != "-" && data.CATCHING_AREA_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_qc .cls_remark_catching_txt_comment_cus').html(data.CATCHING_AREA_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_qc .cls_catch_fao_qc').hide();

                if (data.CHECK_DETAIL_COMMENT_DISPLAY != "-" && data.CHECK_DETAIL_COMMENT_DISPLAY != null) {
                    $('.cls_art_gm_qc .cls_remark_check_detail_txt_comment_cus').html(data.CHECK_DETAIL_COMMENT_DISPLAY);
                }
                else
                    $('.cls_art_gm_qc .cls_check_detail_qc').hide();

                if (data.QC_COMMENT != "-" && data.QC_COMMENT != null) {
                    $('.cls_art_gm_qc .cls_remark_comment_qc_cus').html(data.QC_COMMENT);
                }
                else
                    $('.cls_art_gm_qc .cls_qccomment_qc').hide();


                if (data.DECISION_FORMLABEL_DISPLAY == "Confirm to change")
                    $(".cls_art_gm_qc input[name=rdoChange_gmqc_11][value=1]").prop('checked', true);
                else if (data.DECISION_FORMLABEL_DISPLAY == "Do not change")
                    $(".cls_art_gm_qc input[name=rdoChange_gmqc_11][value=0]").prop('checked', true);

                if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Confirm to change")
                    $(".cls_art_gm_qc input[name=rdoChange_gmqc_12][value=1]").prop('checked', true);
                else if (data.DECISION_NONCOMPLIANCE_DISPLAY == "Do not change")
                    $(".cls_art_gm_qc input[name=rdoChange_gmqc_12][value=0]").prop('checked', true);

                if (data.DECISION_ADJUST_DISPLAY == "Confirm to change")
                    $(".cls_art_gm_qc input[name=rdoChange_gmqc_2][value=1]").prop('checked', true);
                else if (data.DECISION_ADJUST_DISPLAY == "Do not change")
                    $(".cls_art_gm_qc input[name=rdoChange_gmqc_2][value=0]").prop('checked', true);

                if (data.APPROVE == "Approve")
                    $(".cls_art_gm_qc input[name=rdoApprove_gmqc][value=1]").prop('checked', true);
                else if (data.APPROVE == "Not Approve")
                    $(".cls_art_gm_qc input[name=rdoApprove_gmqc][value=0]").prop('checked', true);

                $('.cls_art_gm_qc .cls_gm_qc_sender_txt_comment').html(data.COMMENT_BY_QC);
                $('.cls_art_gm_qc .cls_gm_qc_txt_comment').val(data.COMMENT);
            }
        },
    });

    table_gm_qc_log_art.on('order.dt search.dt', function () {
        table_gm_qc_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_art_gm_qc(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    jsonObj.data = {};
    var item = {};
    item.PROCESS = {};

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;
    item["COMMENT"] = $('.cls_art_gm_qc .cls_gm_qc_txt_comment').val();
    item["ENDTASKFORM"] = EndTaskForm;
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
    item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_MK_VERIFY').curr_role;
    item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_MK_VERIFY').curr_step;
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;
    item.PROCESS["REMARK"] = $('.cls_art_gm_qc .cls_gm_qc_txt_comment').val();


    jsonObj.data = item;

    var myurl = '/api/taskform/qc/sendtomkbygmqc';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, tohomepage, '', true, true, 'Do you want to submit to MK?');
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}
var table_pp_log_art;
function bind_art_pp_art() {
    table_pp_log_art = $('#table_pp_log_art').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pa/sendtopp?data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId,
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
        "processing": true,
        "lengthChange": false,
        "paging": false,
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
            { "data": "REQUEST_SHADE_LIMIT_DISPLAY_TXT", "className": "" },
            { "data": "REASON_BY_PA", "className": "" },
            { "data": "REMARK_REASON_BY_PA", "className": "" },
            { "data": "COMMENT_BY_PA", "className": "" },
            { "data": "CREATE_DATE_BY_PA", "className": "cls_nowrap" },
            { "data": "ACTION_NAME", "className": "" },
            { "data": "REASON_BY_OTHER", "className": "" },
            { "data": "REMARK_REASON_BY_OTHER", "className": "" },
            { "data": "COMMENT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(10).html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('td').eq(5).html(myDateTimeMoment(data.CREATE_DATE_BY_PA));
            if (data.ARTWORK_SUB_ID == ArtworkSubId) {
                $('.cls_art_pp .cls_pp_sender_txt_comment').html(data.COMMENT_BY_PA);

                //ticket442923  by aof start
                // $('.cls_art_pp .cls_pp_txt_comment').val(data.COMMENT);   //aof commment
                if (ReadOnly == "1") {
                    $('.cls_art_pp .cls_pp_txt_comment').html(data.COMMENT);
                } else {
                    pp_pa_quill.clipboard.dangerouslyPasteHTML(data.COMMENT); 
                }
                 //ticket442923  by aof last
               
               
                if (data.REASON_ID != null)
                    setValueToDDL('.cls_art_pp .cls_lov_send_for_reason', data.REASON_ID, data.REASON_BY_OTHER);
                if (data.REQUEST_SHADE_LIMIT_REFERENCE_DISPLAY_TXT == 'Yes')
                    $('.cls_art_pp input:radio[name=rdo_shade]').filter('[value=1]').prop('checked', true);
                else
                    $('.cls_art_pp input:radio[name=rdo_shade]').filter('[value=0]').prop('checked', true);
            }
        },
    });

    table_pp_log_art.on('order.dt search.dt', function () {
        table_pp_log_art.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function save_art_pp(ACTION_CODE, EndTaskForm) {
    var jsonObj = new Object();
    var item = {};
    item.PROCESS = {};
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["ACTION_CODE"] = ACTION_CODE;
    var _editor = new Quill('.cls_art_pp .cls_pp_txt_comment');  // ticket#442923  by aof
    item["COMMENT"] = _editor.root.innerHTML; // $('.cls_art_pp .cls_pp_txt_comment').val();  // ticket#442923  by aof
    item["UPDATE_BY"] = UserID;
    item["CREATE_BY"] = UserID;
    item["ENDTASKFORM"] = EndTaskForm;
    item["REASON_ID"] = $(".cls_art_pp .cls_lov_send_for_reason").val();
    item["REMARK_REASON"] = $(".cls_art_pp .cls_input_pp_pa_other").val();
    item["WF_STEP"] = getstepartwork('SEND_PP').curr_step;
    if (item["REASON_ID"] == DefaultResonId && ACTION_CODE == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    if (ACTION_CODE == 'SENDTO_VENDOR') {
        item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
        item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
        item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
        item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_VN_PO').curr_role;
        item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_VN_PO').curr_step;
        item.PROCESS["REASON_ID"] = $('.cls_art_pp .cls_lov_send_for_reason').val();
        item.PROCESS["CREATE_BY"] = UserID;
        item.PROCESS["UPDATE_BY"] = UserID;
        item.PROCESS["REMARK"] = _editor.root.innerHTML //$('.cls_art_pp .cls_pp_txt_comment').val();  // ticket#442923  by aof
    }

    jsonObj.data = item;

    var myurl = '/api/taskform/internal/pp/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (EndTaskForm)
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
    else
        myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, '', '', true, true, true);
}

//function MKSubmitDataPop() {
//    if ($('.cls_header_chk_mk[type="checkbox"]:checked').length > 0)
//        myAjaxConfirmSubmitBlank(callbackMKSubmitDataPop);
//    else {
//        $(".se-pre-con").fadeOut('fast');
//        alertError2("Please select at least 1 item.");
//    }
//}

function QCSubmitDataPop() {
    if ($('.cls_header_chk_qc[type="checkbox"]:checked').length > 0) {
        myAjaxConfirmSubmitBlank(callbackQCSubmitDataPop_);
    }
    else {
        $(".se-pre-con").fadeOut('fast');
        alertError2("Please select at least 1 item.");
    }
}

////MK submit in pop up
//function callbackMKSubmitDataPop() {

//    if ($('.cls_header_chk_mk[type="checkbox"]:checked').length > 0) {
//        var jsonObj = new Object();
//        var mk_submit_modal = "#mk_submit_modal ";

//        if ($(mk_submit_modal + '.cls_chk_send_pa_mk').is(":checked")) {
//            jsonObj.data = {};
//            var item = {};

//            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
//            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
//            item["ARTWORK_SUB_ID"] = ArtworkSubId;
//            item["ACTION_CODE"] = "SUBMIT";
//            if ($('.cls_art_mk_after_cus #rdoApprove_mk').is(":checked"))
//                item["APPROVE"] = "1";
//            else if ($('.cls_art_mk_after_cus #rdoNotApprove_mk').is(":checked"))
//                item["APPROVE"] = "0";
//            item["ENDTASKFORM"] = true;
//            item["CREATE_BY"] = UserID;
//            item["UPDATE_BY"] = UserID;
//            item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();


//            jsonObj.data = item;

//            var myurl = '/api/taskform/mk/sendtopa';
//            var mytype = 'POST';
//            var mydata = jsonObj;

//            myAjax(myurl, mytype, mydata, hide_modal_submit_mk);
//        }


//        if ($(mk_submit_modal + '.cls_chk_send_gm_mk').is(":checked")) {
//            jsonObj.data = {};
//            var item = {};
//            item.PROCESS = {};

//            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
//            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
//            item["ARTWORK_SUB_ID"] = ArtworkSubId;
//            item["ENDTASKFORM"] = true;
//            item["ACTION_CODE"] = "SUBMIT";
//            item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();
//            item["CREATE_BY"] = UserID;
//            item["UPDATE_BY"] = UserID;

//            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
//            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
//            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
//            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_GM_MK').curr_role;
//            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_GM_MK').curr_step;
//            item.PROCESS["CREATE_BY"] = UserID;
//            item.PROCESS["UPDATE_BY"] = UserID;
//            item.PROCESS["REMARK"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();


//            jsonObj.data = item;

//            var myurl = '/api/taskform/mk/sendtogmmk';
//            var mytype = 'POST';
//            var mydata = jsonObj;

//            myAjax(myurl, mytype, mydata, hide_modal_submit_mk);

//        }

//    }
//    else {
//        $(".se-pre-con").fadeOut('fast');
//        alertError2("Please select at least 1 item.");
//    }
//}


//QC submit in pop up
function callbackQCSubmitDataPop_() {
    if ($('.cls_header_chk_qc[type="checkbox"]:checked').length > 0) {
        var jsonObj = new Object();
        var qc_submit_modal = "#qc_submit_modal ";

        if ($(qc_submit_modal + '.cls_chk_send_pa_qc').is(":checked")) {
            save_art_qc('SUBMIT', true, true);
        }

        if ($(qc_submit_modal + '.cls_chk_send_mk').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ACTION_CODE"] = "SUBMIT";
            item["ENDTASKFORM"] = true;
            item["COMMENT"] = $('.cls_art_qc_after_cus .cls_qc_txt_comment').val();
            if ($('.cls_art_qc_after_cus #rdoApprove_qc').is(":checked"))
                item["APPROVE"] = "1";
            else if ($('.cls_art_qc_after_cus #rdoNotApprove_qc').is(":checked"))
                item["APPROVE"] = "0";
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_MK_VERIFY').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_MK_VERIFY').curr_step;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            item.PROCESS["REMARK"] = $('.cls_art_qc_after_cus .cls_qc_txt_comment').val();



            jsonObj.data = item;

            var myurl = '/api/taskform/qc/sendtomk';
            var mytype = 'POST';
            var mydata = jsonObj;

            myAjax(myurl, mytype, mydata, tohomepage);
        }

        if ($(qc_submit_modal + '.cls_chk_send_gm_qc').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ENDTASKFORM"] = true;
            item["ACTION_CODE"] = "SUBMIT";
            if ($('.cls_art_qc_after_cus #rdoApprove_qc').is(":checked"))
                item["APPROVE"] = "1";
            else if ($('.cls_art_qc_after_cus #rdoNotApprove_qc').is(":checked"))
                item["APPROVE"] = "0";
            item["COMMENT"] = $('.cls_art_qc_after_cus .cls_qc_txt_comment').val();
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_GM_QC').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_GM_QC').curr_step;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            item.PROCESS["REMARK"] = $('.cls_art_qc_after_cus .cls_qc_txt_comment').val();


            jsonObj.data = item;

            var myurl = '/api/taskform/qc/sendtogmqc';
            var mytype = 'POST';
            var mydata = jsonObj;

            myAjax(myurl, mytype, mydata, hide_modal_submit_qc);

        }

        if ($(qc_submit_modal + '.cls_chk_send_rd_qc').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;

            if ($('.cls_art_qc .cls_chk_qc_2').is(":checked"))
                item["NUTRITION"] = "X";
            else
                item["NUTRITION"] = "";
            if ($('.cls_art_qc .cls_chk_qc_3').is(":checked"))
                item["INGREDIENTS"] = "X";
            else
                item["INGREDIENTS"] = "";
            if ($('.cls_art_qc .cls_chk_qc_4').is(":checked"))
                item["ANALYSIS"] = "X";
            else
                item["ANALYSIS"] = "";
            if ($('.cls_art_qc .cls_chk_qc_5').is(":checked"))
                item["HEALTH_CLAIM"] = "X";
            else
                item["HEALTH_CLAIM"] = "";
            if ($('.cls_art_qc .cls_chk_qc_6').is(":checked"))
                item["NUTRIENT_CLAIM"] = "X";
            else
                item["NUTRIENT_CLAIM"] = "";
            if ($('.cls_art_qc .cls_chk_qc_7').is(":checked"))
                item["SPECIES"] = "X";
            else
                item["SPECIES"] = "";
            if ($('.cls_art_qc .cls_chk_qc_8').is(":checked"))
                item["CATCHING_AREA"] = "X";
            else
                item["CATCHING_AREA"] = "";
            if ($('.cls_art_qc .cls_chk_qc_9').is(":checked"))
                item["CHECK_DETAIL"] = "X";
            else
                item["CHECK_DETAIL"] = "";

            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_RD').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_RD').curr_step;
            item.PROCESS["REASON_ID"] = $(qc_submit_modal + '.cls_body_send_rd .cls_lov_send_for_reason').val();
            item["REASON_ID"] = $(qc_submit_modal + '.cls_body_send_rd .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(".cls_body_send_rd .cls_input_rd_by_qc_other").val();
            item["WF_STEP"] = getstepartwork('SEND_RD').curr_step;
            item["IS_SENDER"] = true;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(qc_submit_modal + '.cls_body_send_rd .cls_txt_send_rd');
            item.PROCESS["REMARK"] = editor.root.innerHTML;


            jsonObj.data = item;

            var myurl = '/api/taskform/qc/sendtord';
            var mytype = 'POST';
            var mydata = jsonObj;

            myAjax(myurl, mytype, mydata, hide_modal_submit_qc_rd, '', true, true, true);

        }

    }
    else {
        $(".se-pre-con").fadeOut('fast');
        alertError2("Please select at least 1 item.");
    }
}


function hide_modal_submit_qc() {
    $('#qc_submit_modal').modal('hide');
    setTimeout(function () {
        document.location.href = suburl + "/";
    }, 1000);
}

function hide_modal_submit_qc_rd() {
    $('#qc_submit_modal').modal('hide');
    setTimeout(function () {
        window.location.reload()
    }, 1000);
}

//function SubmitDataPopSendtocustomerByMK() {
//    myAjaxConfirmSubmitBlank(callbackSubmitDataPopSendtocustomerByMK);
//}

//function callbackSubmitDataPopSendtocustomerByMK() {
//    var send = false;

//    if ($('.cls_header_chk_customer_by_mk[type="checkbox"]:checked').length > 0) {
//        var jsonObj = new Object();
//        var send_to_customer_modal_mk = "#send_to_customer_modal_mk ";


//        if ($(send_to_customer_modal_mk + '.cls_chk_send_review_customer').is(":checked")) {
//            jsonObj.data = {};
//            var item = {};
//            item.PROCESS = {};

//            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
//            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
//            item["ARTWORK_SUB_ID"] = ArtworkSubId;
//            item["ENDTASKFORM"] = true;
//            item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REVIEW";
//            if ($('.cls_art_mk_after_cus #rdoApprove_mk').is(":checked"))
//                item["APPROVE"] = "1";
//            else if ($('.cls_art_mk_after_cus #rdoNotApprove_mk').is(":checked"))
//                item["APPROVE"] = "0";
//            item["ACTION_CODE"] = "SUBMIT";
//            item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();

//            if ($(send_to_customer_modal_mk + '.cls_chk_formandlabelraw_customer').is(":checked")) {
//                item["IS_FORMLABEL"] = "X";
//                item["COMMENT_FORM_LABEL"] = $(send_to_customer_modal_mk + '.cls_body_formandlabelraw_customer .cls_remark_formandlabelraw_cus_pop').val();
//            }
//            if ($(send_to_customer_modal_mk + '.cls_chk_qc_changedetails_customer').is(":checked"))
//                item["IS_CHANGEDETAIL"] = "X";
//            if ($(send_to_customer_modal_mk + '.cls_chk_noncompliance_customer').is(":checked")) {
//                item["IS_NONCOMPLIANCE"] = "X";
//                item["COMMENT_NONCOMPLIANCE"] = $(send_to_customer_modal_mk + '.cls_body_noncompliance_customer .cls_remark_noncompliance_cus_pop').val();
//            }
//            if ($(send_to_customer_modal_mk + '.cls_chk_remark_adjustment_customer').is(":checked")) {
//                item["IS_ADJUST"] = "X";
//                item["COMMENT_ADJUST"] = $(send_to_customer_modal_mk + '.cls_body_adjustment_customer .cls_remark_adjustment_cus_pop').val();
//            }


//            if ($(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_nutri_txt_comment_cus_pop').html() != "")
//                item["NUTRITION_COMMENT"] = $(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_nutri_txt_comment_cus_pop').html();
//            if ($(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_ingre_txt_comment_cus_pop').html() != "")
//                item["INGREDIENTS_COMMENT"] = $(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_ingre_txt_comment_cus_pop').html();
//            if ($(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_analysis_txt_comment_cus_pop').html() != "")
//                item["ANALYSIS_COMMENT"] = $(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_analysis_txt_comment_cus_pop').html();
//            if ($(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_health_txt_comment_cus_pop').html() != "")
//                item["HEALTH_CLAIM_COMMENT"] = $(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_health_txt_comment_cus_pop').html();
//            if ($(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_nutclaim_txt_comment_cus_pop').html() != "")
//                item["NUTRIENT_CLAIM_COMMENT"] = $(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_nutclaim_txt_comment_cus_pop').html();
//            if ($(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_species_txt_comment_cus_pop').html() != "")
//                item["SPECIES_COMMENT"] = $(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_species_txt_comment_cus_pop').html();
//            if ($(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_catching_txt_comment_cus_pop').html() != "")
//                item["CATCHING_AREA_COMMENT"] = $(send_to_customer_modal_mk + '.cls_body_qc_changedetails_customer .cls_remark_catching_txt_comment_cus_pop').html();


//            item["CREATE_BY"] = UserID;
//            item["UPDATE_BY"] = UserID;

//            item.PROCESS["REMARK"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();
//            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
//            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
//            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
//            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_CUS_REVIEW').curr_step;
//            item.PROCESS["CREATE_BY"] = UserID;
//            item.PROCESS["UPDATE_BY"] = UserID;

//            jsonObj.data = item;

//            var myurl = '/api/taskform/pa/sendtocustomer';
//            var mytype = 'POST';
//            var mydata = jsonObj;

//            if (send)
//                myAjax(myurl, mytype, mydata, tohomepage, '', true, false);
//            else {
//                send = true;
//                myAjax(myurl, mytype, mydata, tohomepage, '', true, true);
//            }

//        }


//        if ($(send_to_customer_modal_mk + '.cls_chk_send_req_ref').is(":checked")) {
//            jsonObj.data = {};
//            var item = {};
//            item.PROCESS = {};

//            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
//            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
//            item["ARTWORK_SUB_ID"] = ArtworkSubId;
//            item["ENDTASKFORM"] = true;
//            item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REQ_REF";
//            if ($('.cls_art_mk_after_cus #rdoApprove_mk').is(":checked"))
//                item["APPROVE"] = "1";
//            else if ($('.cls_art_mk_after_cus #rdoNotApprove_mk').is(":checked"))
//                item["APPROVE"] = "0";
//            item["ACTION_CODE"] = "SUBMIT";
//            item["COMMENT"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();
//            item["CREATE_BY"] = UserID;
//            item["UPDATE_BY"] = UserID;


//            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
//            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
//            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
//            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_CUS_REQ_REF').curr_step;
//            item.PROCESS["CREATE_BY"] = UserID;
//            item.PROCESS["UPDATE_BY"] = UserID;
//            item.PROCESS["REMARK"] = $('.cls_art_mk_after_cus .cls_mk_txt_comment').val();

//            jsonObj.data = item;

//            var myurl = '/api/taskform/pa/sendtocustomer';
//            var mytype = 'POST';
//            var mydata = jsonObj;

//            if (send)
//                myAjax(myurl, mytype, mydata, tohomepage, '', true, false);
//            else {
//                send = true;
//                myAjax(myurl, mytype, mydata, tohomepage, '', true, true);
//            }
//        }

//    }
//    else {
//        $(".se-pre-con").fadeOut('fast');
//        alertError2("Please select at least 1 item.");
//    }
//}

function hide_modal_submit_mk() {
    $('#mk_submit_modal').modal('hide');
    setTimeout(function () {
        document.location.href = suburl + "/";
    }, 1000);
}

function hide_modal_submit_qc_error() {
    $('#qc_submit_modal').modal('hide');
}

function hide_modal_submit_mk_error() {
    $('#mk_submit_modal').modal('hide');
}

function selectOnlyThisQCmodal(id) {
    for (var i = 1; i <= 2; i++) {
        if ("qc_modal_rdo" + i === id && document.getElementById("qc_modal_rdo" + i).checked === true) {
            document.getElementById("qc_modal_rdo" + i).checked = true;
        } else {
            document.getElementById("qc_modal_rdo" + i).checked = false;
        }
    }
}