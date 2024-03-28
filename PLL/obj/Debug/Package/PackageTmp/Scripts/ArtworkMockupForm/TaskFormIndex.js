var CURRENTUSERID = 0;
var NODE_ID_MOCKUP = 0;
//var CUSTOMER_ID = 0;
var CURRENT_STEP_CODE_DISPLAY_TXT;
var isProjectNoCus = "0";
var stepDurationReasonId;
var stepDurationReasonText;
inTaskForm = true;
$(document).ready(function () {

    bind_lov_template('#pg_master_template .cls_lov_template', '/api/common/template', 'data.DISPLAY_TXT', 'Please select file for download', '', callback_after_select_template);
    $(".cls_pg_master_template .cls_btn_copy_template").click(function (e) {
        copyToClipboard('id_txt_template_data_for_copy_to_clipboard');
    });
    bind_lov_reason('#popup_terminate_workflow_mockup .cls_lov_mockup_reason_terminate', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_TERMINATE_REASON&data.IS_ACTIVE=X', 'data.DISPLAY_TXT');

    bindTaskform(MOCKUPSUBID);
    bindDataPG(MainMockupSubId);
    bind_lov_reason('.cls_row_extend_step_duration .cls_lov_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_' + CURRENT_STEP_CODE_DISPLAY_TXT, 'data.DISPLAY_TXT', '.cls_input_reason_other');

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab

        switch (target) {
            case '#view_artwork':
                break;
            case '#view_so_detail':
                break;
            case '#view_pa':
                break;
            case '#view_pg':
                break;
            case '#view_internal_department':
                break;
            case '#view_check_list':
                break;
            case '#view_attachment':
                bindDataAttach(MOCKUPSUBID);
                break;
            case '#view_vendor':
                break;
            default:
                break;
        }
    });

    $(".cls_tf_btn_accept").click(function () {
        acceptTaskform('SentToPG');
    });

    $(".cls_btn_header_extend_duration").click(function () {
        if ($('.cls_row_extend_step_duration .cls_lov_reason').val() == DefaultResonId) {
            alertError2("Please select reason for extend step duration");
            return false;
        }
        else if ($('.cls_row_extend_step_duration .cls_lov_reason').select2('data')[0].text == "อื่นๆ โปรดระบุ (Others)" && $('.cls_row_extend_step_duration .cls_input_reason_other').val() == '') {
            alertError2("Please input remark reason for extend step duration");
            return false;
        }
        else {
            setEntendStepDuration_TaskformMockup(true);
        }
    });

    $('.cls_header_extend_duration').click(function () {
        setEntendStepDuration_TaskformMockup($(this).prop('checked'));
    });

    //bind_lov('.cls_div_header_update_packing_style .cls_header_lov_packing_style', '/api/lov/packingstyle', 'data.DISPLAY_TXT');
    //bind_lov('.cls_div_header_update_packing_style .cls_header_lov_pack_size', '/api/lov/packsize', 'data.DISPLAY_TXT');
    //$(".cls_div_header_update_packing_style form").submit(function (e) {
    //    if ($(this).valid()) {
    //        var jsonObj = new Object();
    //        var item = {};
    //        item["MOCKUP_ID"] = MOCKUPID;
    //        item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    //        item["PACKING_STYLE_ID"] = $('.cls_div_header_update_packing_style .cls_header_lov_packing_style').val();
    //        item["PACK_SIZE_ID"] = $('.cls_div_header_update_packing_style .cls_header_lov_pack_size').val();
    //        item["UPDATE_BY"] = UserID;
    //        jsonObj.data = item;

    //        var myurl = '/api/taskform/internal/warehouse/packstyle';
    //        var mytype = 'POST';
    //        var mydata = jsonObj;
    //        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
    //    }
    //    $(this).removeAttr("novalidate");
    //    e.preventDefault();	//STOP default action
    //});

    bind_lov_template('#pg_submit_modal_vendor .cls_lov_search_file_template', '/api/lov/filetemplatemockup', 'data.DISPLAY_TXT', 'Please select file for download');
    $('#pg_submit_modal_vendor .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('#pg_submit_modal_vendor .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('#pg_submit_modal_vendor .cls_lov_search_file_template').val());
    });

    bind_lov_template('#pg_submit_modal .cls_lov_search_file_template', '/api/lov/filetemplatemockup', 'data.DISPLAY_TXT', 'Please select file for download');
    $('#pg_submit_modal .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('#pg_submit_modal .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('#pg_submit_modal .cls_lov_search_file_template').val());
    });

    bind_lov_template('.cls_pg_submit_modal_to_customer .cls_lov_search_file_template', '/api/lov/filetemplatemockup', 'data.DISPLAY_TXT', 'Please select file for download');
    $('.cls_pg_submit_modal_to_customer .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('.cls_pg_submit_modal_to_customer .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('.cls_pg_submit_modal_to_customer .cls_lov_search_file_template').val());
    });

    var overdue = '';
    if (OverDue == "1")
        overdue = "X";

    bind_lov_reason('.cls_body_send_rd .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_RD&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_rd_other');
    bind_lov_reason('.cls_rdmu_row .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_RD_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_rdmu_row .cls_input_other');

    bind_lov_reason('.cls_body_send_planning .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_PN&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_planning_other');
    bind_lov_reason('.cls_plnmu_row .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PN_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT','.cls_plnmu_row .cls_input_other');

    bind_lov_reason('.cls_body_send_warehouse .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_WH&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_warehouse_other');
    bind_lov_reason('.cls_whmu_row .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_WH_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_whmu_row .cls_input_other');

    bind_lov_reason('.cls_body_send_supervisor_need_design .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_SUP_SELECT_VENDOR_NEED_DESIGN&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_supervisor_need_design_other');
    bind_lov_reason('.cls_pg_sup_row .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_SUP_SELECT_VENDOR_NEED_DESIGN_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_pg_sup_row .cls_input_other');

    bind_lov_reason('.cls_body_send_approval_match_board .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_MANAGER_APPROVE_MATCH_BOARD&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_approval_match_board_other');
    bind_lov_reason('.cls_manager_approve_matchboard_row .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_MANAGER_APPROVE_MATCH_BOARD_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_manager_approve_matchboard_row .cls_input_other');

    bind_lov_reason('.cls_body_send_supervisor .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_SUP_SELECT_QUO&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_supervisor_other');

    bind_lov_reason('.cls_body_send_pr .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_PR&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_pr_other');
    bind_lov_reason('.cls_body_send_rs .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_rs_other');
    bind_lov_reason('.cls_body_send_mb .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_MB&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_mb_other');
    bind_lov_reason('.cls_body_send_dl .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_DL&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_send_dl_other');

    bind_lov_reason('.cls_pr_vendor .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_VN_PR_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_pr_vendor .cls_input_other');
    bind_lov_reason('.cls_rs_vendor .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_VN_RS_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_rs_vendor .cls_input_other');
    bind_lov_reason('.cls_mb_vendor .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_VN_MB_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_mb_vendor .cls_input_other');
    bind_lov_reason('.cls_dl_vendor .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_VN_DL_SEND_TO_PG&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_dl_vendor .cls_input_other');

    bind_lov_reason('.cls_pg_submit_modal_to_customer .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_CUSTOMER&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_remark_other');
    bind_lov_reason('#popup_send_to_checklist_creator .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_BACK_MK&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');

    bindRemarkReason();
    setValueToDDL('.cls_row_extend_step_duration .cls_lov_reason', stepDurationReasonId, stepDurationReasonText);

});
var linkDownloadFileTemplate = "";


function callback_after_select_template(obj) {
    var myurl = '/api/common/template?data.template_id=' + $(obj).val();
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_after_select_template2);
}

function callback_after_select_template2(res) {
    $('.cls_pg_master_template .cls_txt_template_data').html(res.data[0].DESCRIPTION);
}

function acceptTaskform(step_role) {
    var jsonObj = new Object();
    var item = {};
    item["MOCKUP_ID"] = MOCKUPID;
    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["CURRENT_USER_ID"] = UserID;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;
    var myurl = '/api/taskform/mockupprocess/accepttask';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, callback_accept_taskform, '', false, true);
}

function callback_accept_taskform(res) {
    bindTaskform(MOCKUPSUBID);
    bindDataPG(MainMockupSubId);
}

function bindTaskform(mockup_sub_id) {
    var myurl = '/api/taskform/mockupprocess/process?data.mockup_sub_id=' + mockup_sub_id;
    var mytype = 'GET';
    var mydata = null;
    myAjaxNoSync(myurl, mytype, mydata, callback_get_taskform);
}

function callback_get_taskform(res) {
    if (res.data.length > 0) {
        var v = res.data[0];
        //MOCKUPID = v.MOCKUP_ID;
        CURRENTUSERID = v.CURRENT_USER_ID;
        
        NODE_ID_MOCKUP = v.NODE_ID_MOCKUP;
        CURRENT_STEP_CODE_DISPLAY_TXT = v.CURRENT_STEP_CODE_DISPLAY_TXT;
        $('.cls_header_checklist_no').val(v.CHECK_LIST_NO_DISPLAY_TXT);
        $('.cls_header_mockup_no').val(v.MOCKUP_NO_DISPLAY_TXT);

        if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PP') {
            $('.cls_btn_header_tfartwork_accept').hide();
        }
        if (v.MOCKUP_NO_DISPLAY_TXT.indexOf("MO-N-") >= 0) {
            $('#modal_taskform_pg_search_for_dieline .cls_btn_taskform_pg_dieline_select').hide();
        }

        if (v.IS_STEP_DURATION_EXTEND == 'X') {
            stepDurationReasonId = v.STEP_DURATION_EXTEND_REASON_ID;
            stepDurationReasonText = v.STEP_DURATION_REMARK_REASON;
            //setValueToDDL('.cls_row_extend_step_duration .cls_lov_reason', v.STEP_DURATION_EXTEND_REASON_ID, v.STEP_DURATION_REMARK_REASON);
            $('.cls_row_extend_step_duration .cls_lov_reason').prop('disabled', true);
            $('.cls_btn_header_extend_duration').hide();
            $('.extend_step_duration_warning').hide();
            if (v.STEP_DURATION_EXTEND_REMARK != '') {
                setValueToDDLOther('.cls_row_extend_step_duration .cls_input_reason_other', v.STEP_DURATION_EXTEND_REMARK);
                $('.cls_row_extend_step_duration .cls_input_reason_other').prop('disabled', true);
            }
        }

        if (isEmpty(v.IS_END))
            $('.cls_header_curr_status_txt').val(v.CURRENT_STEP_DISPLAY_TXT);
        else {
            if (isEmpty(v.IS_TERMINATE))
                $('.cls_header_curr_status_txt').val(v.CURRENT_STEP_DISPLAY_TXT + " [Completed]");
            else
                $('.cls_header_curr_status_txt').val(v.CURRENT_STEP_DISPLAY_TXT + " [Terminated]");
        }
        $('.cls_pg_price_template_vendor .cls_qua_remark').val(v.REMARK);

        if (isEmpty(CURRENTUSERID) && v.MOCKUP_NO_DISPLAY_TXT.indexOf('MO-C-') < 0) {
            $('.cls_tf_btn_accept').show();
            if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PN_PRI_PKG') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_RD_PRI_PKG') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_WH_TEST_PACK') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG_SUP_SEL_VENDOR') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_APP_MATCH_BOARD') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_MK_UPD_PACK_STYLE') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_WH_UPD_PACK_STYLE') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
        }
        else {
            $('.cls_tf_btn_accept').hide();
            $('.cls_row_extend_step_duration').show();
            if (v.IS_STEP_DURATION_EXTEND == 'X') {
                $('.cls_header_extend_duration').prop('checked', true);
            }
            else {
                $('.cls_header_extend_duration').prop('checked', false);
            }

            if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                $('a[href="#view_check_list"]').tab('show');
                 var mockup_role = false;
                debugger;
                var mockup_role = false;
                if (v.MOCKUP_NO_DISPLAY_TXT.indexOf('MO-D-') >= 0)
                    mockup_role = true;
                var bool = (getroleuser("MK_CD") || getroleuser("MK_CD_SENIOR") || getroleuser("MK_CD_AM") || getroleuser("MK_GM") || getroleuser("MC_STAFF") || getroleuser("MC_SUPERVISOR") || getroleuser("MC_AM") ||
                    getroleuser("MK_CD_MC_MANAGER") || getroleuser("PMC") || getroleuser("PME")); 
                if (getroleuser("PG_STAFF") || getroleuser("PG_TEAM_LEAD") || getroleuser("PG_SUPPERVISOR") || getroleuser("PG_MANAGER") || getroleuser("ADMINISTRATOR_PK") || getroleuser("ADMINISTRATOR")
                    || getroleuser("PA_STAFF") || getroleuser("PA_TEAM_LEAD") || getroleuser("PA_SUPERVISOR") || getroleuser("PA_ASS_MANAGER") || (mockup_role && bool)) {
                    $('.cls_li_attachment').show();
                }

                if (v.MOCKUP_NO_DISPLAY_TXT.indexOf('MO-D-') >= 0) {
                    $('.cls_li_pg_data').show();
                    $('.cls_li_checklist_data').show();
                    $('.cls_li_history').show();

                    $('.cls_task_form_pg .cls_div_for_mockup_normal_and_project input').prop("disabled", true);
                    $('.cls_task_form_pg .cls_div_for_mockup_normal_and_project textarea').prop("disabled", true);
                    $('.cls_task_form_pg .cls_div_for_mockup_normal_and_project select').prop("disabled", true);
                    $('.cls_task_form_pg .cls_btn_submit').hide();
                    $('.cls_task_form_pg .cls_btn_send_to_vendor').hide();
                    $('.cls_task_form_pg .cls_btn_send_to_customer').hide();
                }
                else {
                    $('.cls_li_pg_data').show();
                    $('.cls_li_quotations').show();
                    $('.cls_pg_price_template .cls_div_select_vendor').show();

                    if (WaitingPGSUPSelectQuo == "1") {
                        $('.cls_pg_price_template .cls_table_user_vendor_price_template').hide();
                        $('.cls_pg_price_template .cls_btn_request_quotation_price_template').hide();
                        $('.cls_pg_price_template .cls_btn_reset_vendor_price_template').hide();
                        $('.cls_pg_price_template .cls_btn_save_vendor_price_template').hide();
                        $('.cls_pg_price_template .cls_row_select_vendor').hide();
                        $('.cls_div_price_compare .cls_row_manual_add_vendor').hide();
                    }
                    else {
                        $('.cls_pg_price_template .cls_table_user_vendor_price_template').show();
                        $('.cls_pg_price_template .cls_div_price_template').show();
                        $('.cls_pg_price_template .cls_btn_request_quotation_price_template').show();
                        $('.cls_pg_price_template .cls_btn_reset_vendor_price_template').show();
                        $('.cls_pg_price_template .cls_btn_save_vendor_price_template').show();
                        $('.cls_pg_price_template .cls_row_select_vendor').show();
                        $('.cls_div_price_compare .cls_row_manual_add_vendor').show();
                    }

                    $('.cls_pg_price_template .cls_div_price_compare').show();

                    //if (getroleuser("PG_STAFF") || getroleuser("PG_TEAM_LEAD") || getroleuser("PG_SUPPERVISOR") || getroleuser("PG_ASS_MANAGER") || getroleuser("PG_MANAGER")) {
                    //    $('.cls_li_attachment').show();
                    //}
                    $('.cls_li_checklist_data').show();
                    $('.cls_li_history').show();

                    $('.cls_li_internal_department').show();
                    $('.cls_rdmu_row').show();
                    $('.cls_plnmu_row').show();
                    $('.cls_whmu_row').show();
                    $('.cls_pg_sup_row').show();
                    $('.cls_manager_approve_matchboard_row').show();

                    $('.cls_li_customer').show();
                    $('.cls_div_customer .cls_div_customer_log').show();

                    $('.cls_li_vendor').show();
                    $('.cls_pr_vendor').show();
                    $('.cls_rs_vendor').show();
                    $('.cls_mb_vendor').show();
                    $('.cls_dl_vendor').show();
                }

                if (inTaskForm) {
                    if (CreateByFFC == "1") {
                        $('.cls_check_list_form .cls_row_copy_ref1 .cls_btn_search_chklist').hide();
                        $('.cls_check_list_form .cls_row_copy_ref2').hide();

                        if (!isEmpty(CURRENTUSERID)) {
                            $('.cls_check_list_form .cls_btn_save').show();

                            $('.cls_check_list_form .cls_lov_rd_person').attr('disabled', false);
                            $('.cls_check_list_form .cls_lov_primary_type_other').attr('disabled', false);
                            $('.cls_check_list_form .cls_input_primary_type_other').attr('disabled', false);

                            $('.cls_check_list_form .cls_lov_primary_size_other').attr('disabled', false);
                            $('.cls_check_list_form .cls_input_primary_size_other').attr('disabled', false);

                            if (isEmpty(res.data[0].THREE_P_ID) || res.data[0].THREE_P_ID == -1) {
                                $('.cls_check_list_form .cls_lov_container_type_other').attr('disabled', false);
                                $('.cls_check_list_form .cls_lov_lid_type_other').attr('disabled', false);
                            }
                            $('.cls_check_list_form .cls_input_container_type_other').attr('disabled', false);
                            $('.cls_check_list_form .cls_input_lid_type_other').attr('disabled', false);

                            $('.cls_check_list_form .cls_lov_packing_style_other').attr('disabled', false);
                            $('.cls_check_list_form .cls_input_packing_style_other').attr('disabled', false);

                            if (isEmpty(res.data[0].TWO_P_ID) || res.data[0].TWO_P_ID == -1) {
                                $('.cls_check_list_form .cls_lov_pack_size_other').attr('disabled', false);
                            }
                            $('.cls_check_list_form .cls_input_pack_size_other').attr('disabled', false);
                        }
                    }
                }


            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PN_PRI_PKG') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_internal_department').show();

                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
                $('.cls_plnmu_row').show();
                $('.cls_plnmu_row .cls_div_only_pn').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_RD_PRI_PKG') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_internal_department').show();
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
                $('.cls_rdmu_row').show();
                $('.cls_rdmu_row .cls_div_only_rd').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_WH_TEST_PACK') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_internal_department').show();
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
                $('.cls_whmu_row').show();
                $('.cls_whmu_row .cls_div_only_wh').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_QUO') {
                $('a[href="#view_price_template_vendor"]').tab('show');
                $('.cls_li_quotations_vendor').show();
                load_pricetemplate_vendor();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG_SUP_SEL_VENDOR') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_quotations').show();

                $('.cls_pg_price_template .cls_div_select_vendor').show();
                $('.cls_pg_price_template .cls_div_price_template').show();
                $('.cls_pg_price_template .cls_div_price_compare').show();
                $('.cls_pg_price_template .cls_div_select_vendor_manager').show();
                //$('.cls_li_attachment').show();
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN') {
                $('a[href="#view_check_list"]').tab('show');

                $('.cls_li_internal_department').show();
                $('.cls_pg_sup_row').show();
                $('.cls_only_pg_sup').show();

                //$('.cls_li_attachment').show();
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_APP' || v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_MK_APP') {
                $('a[href="#view_customer"]').tab('show');
                $('.cls_li_customer').show();
                $('.cls_div_customer .cls_div_only_customer').show();
                bind_data_tab_customer();
                bindSendtoCustomer(MOCKUPSUBID);
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_APP_MATCH_BOARD') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();

                $('.cls_li_internal_department').show();
                $('.cls_manager_approve_matchboard_row').show();
                $('.cls_div_only_manager_approve_matchboard').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_RS') {
                $('a[href="#view_vendor"]').tab('show');
                $('.cls_li_vendor').show();

                $('.cls_div_only_vendor').show();
                load_tab_vendor();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_MB') {
                $('a[href="#view_vendor"]').tab('show');
                $('.cls_li_vendor').show();

                $('.cls_div_only_vendor').show();
                load_tab_vendor();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_DL') {
                $('a[href="#view_vendor"]').tab('show');
                $('.cls_li_vendor').show();

                $('.cls_div_only_vendor').show();
                load_tab_vendor();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_PR') {
                $('a[href="#view_vendor"]').tab('show');
                $('.cls_li_vendor').show();

                $('.cls_div_only_vendor').show();
                load_tab_vendor();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_MK_UPD_PACK_STYLE') {
                //$('a[href="#view_check_list"]').tab('show');
                //$('.cls_li_checklist_data').show();
                //$('.cls_li_history').show();
                //$('.cls_div_header_update_packing_style').show();

                //$('.cls_li_internal_department').show();
                //$('.cls_mk_send_to_warehouse_row').show();

                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();

                $('.cls_check_list_form .cls_btn_save').show();
                $('.cls_check_list_form .cls_btn_submit_by_mk').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_WH_UPD_PACK_STYLE') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
                $('.cls_div_header_update_packing_style').show();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_BACK_MK') {
                $('a[href="#view_check_list"]').tab('show');
                $('.cls_li_checklist_data').show();
                $('.cls_li_history').show();
                $('.cls_li_attachment').show();

                $('.cls_check_list_form .cls_btn_save').show();
                $('.cls_check_list_form .cls_btn_submit_by_mk').show();
            }

        }

    }
}

function setEntendStepDuration_TaskformMockup(is_check) {
    var extendStepDuration = '';
    if (is_check) {
        extendStepDuration = 'X';
    }
    var jsonObj = new Object();
    var item = {};
    item["IS_STEP_DURATION_EXTEND"] = extendStepDuration;
    item["STEP_DURATION_EXTEND_REASON_ID"] = $('.cls_row_extend_step_duration .cls_lov_reason').val();
    item["STEP_DURATION_EXTEND_REMARK"] = $('.cls_row_extend_step_duration .cls_input_reason_other').val();
    item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["STEP_CODE"] = CURRENT_STEP_CODE_DISPLAY_TXT;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;
    var myurl = '/api/taskform/mockupprocess/stepdurationextend';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, function (res) {
        if (res.data.length > 0) {
            if (res.data[0].IS_OVER_DUE == "1") {
                $('.cls_display_txt_over_due').show();
            }
            else {
                $('.cls_display_txt_over_due').hide();
            }

            $('.cls_row_extend_step_duration .cls_lov_reason').prop('disabled', true);
            $('.cls_btn_header_extend_duration').hide();
            $('.extend_step_duration_warning').hide();
            $('.cls_row_extend_step_duration .cls_input_reason_other').prop('disabled', true);
        }
    });
}

function bindRemarkReason() {
    var myurl = '/api/taskform/remarkreason/info?data.MOCKUP_SUB_ID=' + MOCKUPSUBID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_getRemark);
}

function callback_getRemark(res) {
    var item = res.data;
    if (item.length > 0) {
        var currentstepid = getstepmockup(CURRENT_STEP_CODE_DISPLAY_TXT).curr_step;
        for (var i = 0; i < item.length; i++) {
            if (item[i].WF_SUB_ID == MOCKUPSUBID && item[i].WF_STEP == currentstepid) {
                //Internal
                if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_RD_PRI_PKG') setValueToDDLOther('.cls_rdmu_row .cls_input_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_PN_PRI_PKG") setValueToDDLOther('.cls_plnmu_row .cls_input_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_WH_TEST_PACK") setValueToDDLOther('.cls_whmu_row .cls_input_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN") setValueToDDLOther('.cls_pg_sup_row .cls_input_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_APP_MATCH_BOARD") setValueToDDLOther('.cls_manager_approve_matchboard_row .cls_input_other', item[i].REMARK_REASON);


                // -------------------------------------------------rewrited by aof #INC-11265-----------------------------------------------------------------------------------
                //Vendor
                //if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PR") setValueToDDLOther('.cls_pr_vendor .cls_input_other', item[i].REMARK_REASON);
                //if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_RS") setValueToDDLOther('.cls_rs_vendor .cls_input_other', item[i].REMARK_REASON);
                //if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_MB") setValueToDDLOther('.cls_mb_vendor .cls_input_other', item[i].REMARK_REASON);
                //if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_DL") setValueToDDLOther('.cls_dl_vendor .cls_input_other', item[i].REMARK_REASON);

                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PR") {
                    setValueToDDL('.cls_pr_vendor .cls_lov_send_for_reason', item[i].REASON_ID, gOthers);
                    setValueToDDLOther('.cls_pr_vendor .cls_input_other', item[i].REMARK_REASON);
                } 
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_RS") {
                    setValueToDDL('.cls_rs_vendor .cls_lov_send_for_reason', item[i].REASON_ID, gOthers);
                    setValueToDDLOther('.cls_rs_vendor .cls_input_other', item[i].REMARK_REASON);
                }
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_MB") {
                    setValueToDDL('.cls_mb_vendor .cls_lov_send_for_reason', item[i].REASON_ID, gOthers);
                    setValueToDDLOther('.cls_mb_vendor .cls_input_other', item[i].REMARK_REASON);
                } 
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_DL") {
                    setValueToDDL('.cls_dl_vendor .cls_lov_send_for_reason', item[i].REASON_ID, gOthers);
                    setValueToDDLOther('.cls_dl_vendor .cls_input_other', item[i].REMARK_REASON);
                } 
                // -------------------------------------------------rewrited by aof #INC-11265-----------------------------------------------------------------------------------
            }
        }
    }
}