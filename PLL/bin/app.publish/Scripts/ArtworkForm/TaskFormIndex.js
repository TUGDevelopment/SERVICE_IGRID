var CURRENTUSERID = 0;
var NODE_ID_MOCKUP = 0;
//var CUSTOMER_ID = 0;
var CURRENT_STEP_CODE_DISPLAY_TXT;
var ARTWORK_NO;
var stepDurationReasonId;
var stepDurationReasonText;
inTaskForm = true;

var IS_SO_CHANGE = false;  // by aof show btn so change for admin
var is_first_load_pa = true; // IGRID_REIM
$(document).ready(function () {

    bind_lov('#modal_tfartwork_suggest_material .cls_lov_ship_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('#modal_tfartwork_suggest_material .cls_lov_sold_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('#modal_tfartwork_suggest_material .cls_lov_brand', '/api/lov/brand', 'data.DISPLAY_TXT');

    bind_lov('.cls_pa_master_template .cls_lov_template', '/api/common/template', 'data.DISPLAY_TXT', '', callback_after_select_template_artwork);
    $(".cls_pa_master_template .cls_btn_copy_template").click(function (e) {
        copyToClipboard('id_txt_template_data_for_copy_to_clipboard');
    });

 
    bindFOC(ArtworkSubId);
    bindTaskformArtwork(ArtworkSubId);
    //bindDataPG(MainMockupSubId);
    bind_lov_reason('.cls_row_extend_step_duration .cls_lov_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_' + CURRENT_STEP_CODE_DISPLAY_TXT, 'data.DISPLAY_TXT', '.cls_input_reason_other');
    //setValueToDDL('.cls_row_extend_step_duration .cls_lov_reason', STEP_DURATION_REASON_ID);

    bindRemarkReason();

    bind_lov_template('#send_to_vendor_modal .cls_lov_search_file_template', '/api/lov/filetemplateartwork', 'data.DISPLAY_TXT', 'Please select file for download');
    $('#send_to_vendor_modal .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('#send_to_vendor_modal .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('#send_to_vendor_modal .cls_lov_search_file_template').val());
    });

    bind_lov_template('#send_to_customer_modal .cls_lov_search_file_template', '/api/lov/filetemplateartwork', 'data.DISPLAY_TXT', 'Please select file for download');
    $('#send_to_customer_modal .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('#send_to_customer_modal .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('#send_to_customer_modal .cls_lov_search_file_template').val());
    });

    bind_lov_template('#send_to_customer_modal_mk .cls_lov_search_file_template', '/api/lov/filetemplateartwork', 'data.DISPLAY_TXT', 'Please select file for download');
    $('#send_to_customer_modal_mk .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('#send_to_customer_modal_mk .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('#send_to_vendor_modal .cls_lov_search_file_template').val());
    });

    bind_lov_template('#mk_submit_modal .cls_lov_search_file_template', '/api/lov/filetemplateartwork', 'data.DISPLAY_TXT', 'Please select file for download');
    $('#mk_submit_modal .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('#mk_submit_modal .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('#send_to_vendor_modal .cls_lov_search_file_template').val());
    });

    bind_lov_template('#qc_submit_modal .cls_lov_search_file_template', '/api/lov/filetemplateartwork', 'data.DISPLAY_TXT', 'Please select file for download');
    $('#qc_submit_modal .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('#qc_submit_modal .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('#send_to_vendor_modal .cls_lov_search_file_template').val());
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_artwork':
                break;
            case '#view_salesorder':
                break;
            case '#view_pa':
                bindDataTaskFormPA();
                break;
            case '#view_pg':
                bindDataTaskFormPG_Artwork();
                break;
            case '#view_internal':
                break;
            case '#view_check_list':
                break;
            case '#view_attachment':
                bindDataAttachArtwork();
                break;
            case '#view_vendor':
                break;
            case '#view_customer':
                break;
            case '#view_po':
                bind_art_po_tab_art();
                break;
            default:
                break;
        }
    });

    $(".cls_btn_header_tfartwork_accept").click(function () {
        acceptTaskformArtwork();
    });

    $(".cls_btn_header_tfartwork_retrieve").click(function () {
        click_retrive_or_request_mat = true;
        retrieveTaskformArtwork();
    });

    $(".cls_btn_header_tfartwork_extend_duration").click(function () {
        if ($('.cls_row_extend_step_duration .cls_lov_reason').val() == DefaultResonId) {
            alertError2("Please select reason for extend step duration");
            return false;
        }
        else if ($('.cls_row_extend_step_duration .cls_lov_reason').select2('data')[0].text == "อื่นๆ โปรดระบุ (Others)" && $('.cls_row_extend_step_duration .cls_input_reason_other').val() == '') {
            alertError2("Please input remark reason for extend step duration");
            return false;
        }
        else {
            setEntendStepDuration_TaskformArtwork(true);
        }
    });

    $('.cls_chk_header_tfartwork_3v').click(function () {
        if ($(this).prop('checked')) {
            $('.cls_container_taskform_artwork').find('.cls_txt_header_tfartwork_mat_no').attr('disabled', false);
            $('.cls_container_taskform_artwork').find('.cls_btn_header_tfartwork_retrieve').attr('disabled', false);
        }
        else {
            $('.cls_container_taskform_artwork').find('.cls_txt_header_tfartwork_mat_no').attr('disabled', true);
            $('.cls_container_taskform_artwork').find('.cls_btn_header_tfartwork_retrieve').attr('disabled', true);
        }
    });

    $('.cls_chk_header_tfartwork_po').click(function () {
        setReadyToCreatePO_TaskformArtwork($(this).prop('checked'));
    });

    $('.cls_chk_header_tfartwork_extend_duration').click(function () {
        setEntendStepDuration_TaskformArtwork($(this).prop('checked'));
    });

    $('.cls_chk_header_tfartwork_shadelimit').click(function () {
        setReceivedShadeLimit_TaskformArtwork($(this).prop('checked'));
    });

    $('input[type=radio][name=cls_rdo_haeder_tfartwork_change_point]').change(function () {
        setChangePoint_TaskformArtwork(this.value);
    });

    $('input[type=radio][name=cls_rdo_haeder_tfartwork_shadelimit]').change(function () {
        setShadeLimit_TaskformArtwork(this.value);
    });

    $(".cls_container_taskform_artwork .cls_btn_header_tfartwork_ref_mat").click(function (e) {
       //// alert($('.cls_tfartwork_pa .cls_lov_pa_product_code').val());  //ticket#437764 test by aof

        //ticket#437764 by aof commented start
       // $('#modal_tfartwork_suggest_material .cls_chk_suggest_material_no_search').prop('checked', false);
       // bindSuggestMaterialPopUp(false);
       // $('#modal_tfartwork_suggest_material').modal({
       //     backdrop: 'static',
       //     keyboard: true
       // });
       //ticket#437764 by aof commented finish
        checkProductCodeIsExistPAData();  //ticket#437764 by aof
    });

    $(".cls_container_taskform_artwork .cls_btn_header_tfartwork_clear_ref_mat").click(function (e) {
        deleteSuggestMaterial();

    });


  
     //ticket#437764 by aof in below function
    $("#modal_tfartwork_suggest_material .cls_btn_search").click(function (e) {
        if (checkEnterCriteria())
        {
            bindSuggestMaterialPopUp(false, "1");  
        }
       
    });

    function checkEnterCriteria() {
        var f_enter = false;

        var brand = !isEmpty($('#modal_tfartwork_suggest_material .cls_lov_brand option:selected').text());
        var ship_to = !isEmpty($('#modal_tfartwork_suggest_material .cls_lov_ship_to option:selected').text());
        var slod_to = !isEmpty($('#modal_tfartwork_suggest_material .cls_lov_sold_to option:selected').text());

        var pk = !isEmpty($('#modal_tfartwork_suggest_material .cls_txt_material_pk').val().replace(/\n/g, ','));
        var fg = !isEmpty($('#modal_tfartwork_suggest_material .cls_txt_material_fg').val().replace(/\n/g, ','));


     


        if (ship_to || slod_to || brand || pk || fg)
        {
            f_enter = true;
        }
        else {
            f_enter = false;
            alertError2("Please enter at least one criteria.");
        }

        return f_enter
    }


     //ticket#437764 by aof in below function
    $("#modal_tfartwork_suggest_material .cls_btn_clear").click(function (e) {
        clearSearchSuggestMaterial();
    });

    function clearSearchSuggestMaterial()
    {
        $('#modal_tfartwork_suggest_material  input[type=text]').val('');

        // $(".cls_report_warehouse .cls_lov_sales_organization").val('').trigger("change");
        $("#modal_tfartwork_suggest_material .cls_lov_ship_to").val('').trigger("change");
        $("#modal_tfartwork_suggest_material .cls_lov_sold_to").val('').trigger("change");
        $("#modal_tfartwork_suggest_material .cls_lov_brand").val('').trigger("change");

    }

     //ticket#437764 by aof in below function
    function checkProductCodeIsExistPAData()
    {
        
        var myurl = "/api/taskform/pa/suggestmaterial/getartworkprocesspa?data.artwork_sub_id=" + ArtworkSubId;
        var mytype = "GET";
        var mydata = null;

        myAjax(myurl, mytype, mydata, callbackCheckProductCodeIsExistPAData);     
    }

    //ticket#437764 by aof in below function
    function callbackCheckProductCodeIsExistPAData(res)
    {
        var f_found = false;
        var product_code = "";
        var matgroup_code = "";
        if (res.data != null && res.data.length > 0)
        {
            if (res.data[0].PRODUCT_CODE_ID > 0)
            {
                f_found = true;
                product_code = res.data[0].PRODUCT_CODE_DISPLAY_TXT;
            }

            if (res.data[0].MATERIAL_GROUP_CODE != null)
            {
                matgroup_code = res.data[0].MATERIAL_GROUP_CODE;
            }

            if (res.data[0].PRODUCTS != null && res.data[0].PRODUCTS.length > 0)
            {
                for (i = 0; i < res.data[0].PRODUCTS.length; i++) {
                    if (res.data[0].PRODUCTS[i].PRODUCT_CODE_ID > 0) {
                      
                        if (product_code.indexOf(res.data[0].PRODUCTS[i].PRODUCT_CODE) == -1) {
                            if (product_code.length > 0) {
                                product_code += ",";
                            }
                            product_code += res.data[0].PRODUCTS[i].PRODUCT_CODE;
                        }                  
                        f_found = true;
                    }   
                }
            }
        }

       
        if (f_found) {

            clearSearchSuggestMaterial(); //ticket#437764 by aof
            $('#modal_tfartwork_suggest_material .cls_chk_suggest_material_no_search').prop('checked', false);  
            $('#modal_tfartwork_suggest_material .cls_txt_material_fg').val(product_code);   //.val($('.cls_tfartwork_pa .cls_lov_pa_product_code').text());  //ticket#437764 by aof
            $('#modal_tfartwork_suggest_material .cls_txt_material_pk').val(matgroup_code);
            setValueToDDL('#modal_tfartwork_suggest_material .cls_lov_sold_to', $('.cls_artwork_request_form .cls_lov_artwork_sold_to').val(), $('.cls_artwork_request_form .cls_lov_artwork_sold_to').text());
            setValueToDDL('#modal_tfartwork_suggest_material .cls_lov_ship_to', $('.cls_artwork_request_form .cls_lov_artwork_ship_to').val(), $('.cls_artwork_request_form .cls_lov_artwork_ship_to').text());
            setValueToDDL('#modal_tfartwork_suggest_material .cls_lov_brand', $('.cls_artwork_request_form .cls_lov_artwork_brand_other').val(), $('.cls_artwork_request_form .cls_lov_artwork_brand_other').text());


            bindSuggestMaterialPopUp(false, "0");
            $('#modal_tfartwork_suggest_material').modal({
                backdrop: 'static',
                keyboard: true
            });
        } else
        {
            alertError2("Product code not found. Please ensure product code is filled and saved.")
        }
    }


    $("#modal_tfartwork_suggest_material .cls_chk_suggest_material_no_search").click(function (e) {
       bindSuggestMaterialPopUp($("#modal_tfartwork_suggest_material .cls_chk_suggest_material_no_search").is(":checked"));
       // bindSuggestMaterialPopUp(false);
    });

    $("#modal_tfartwork_suggest_material .cls_btn_tfartwork_suggest_material_select").click(function (e) {
        var table = $('#table_tfartwork_suggest_material').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            setSelectSuggestMaterial(tblData);
        }
        else {
            alertError2("Please select 1 item.");
        }
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

    bind_lov_template('#pa_submit_modal .cls_lov_search_file_template', '/api/lov/filetemplateartwork', 'data.DISPLAY_TXT', 'Please select file for download');
    $('#pa_submit_modal .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('#pa_submit_modal .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('#pa_submit_modal .cls_lov_search_file_template').val());
    });

    bind_lov_template('.cls_pa_submit_modal_to_customer .cls_lov_search_file_template', '/api/lov/filetemplateartwork', 'data.DISPLAY_TXT', 'Please select file for download');
    $('.cls_pa_submit_modal_to_customer .cls_link_load_file_template').click(function () {
        if (linkDownloadFileTemplate == '') linkDownloadFileTemplate = this.href;
        if (isEmpty($('.cls_pa_submit_modal_to_customer .cls_lov_search_file_template').val())) {
            alertError2("Please select file before download.");
            return false;
        }
        else
            $(this).attr("href", linkDownloadFileTemplate + "?nodeId=" + $('.cls_pa_submit_modal_to_customer .cls_lov_search_file_template').val());
    });

    //BY PA
    var overdue = '';
    if (OverDue == "1")
        overdue = "X";

    bind_lov_reason('.cls_body_send_pg .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_TO_PG_ASSIGN_DIELINE&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_pg_by_pa_other');
    bind_lov_reason('#pa_submit_modal .cls_body_send_mk .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_TO_MK&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '#pa_submit_modal .cls_body_send_mk .cls_input_mk_by_pa_other');
    bind_lov_reason('#modal_tfartwork_pa_sendback .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_BACK_MK&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    bind_lov_reason('.cls_body_send_wh .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_TO_WH&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_wh_by_pa_other');
    bind_lov_reason('.cls_body_send_plan .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_TO_PN&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_pn_by_pa_other');
    bind_lov_reason('.cls_body_send_qc .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_TO_QC&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_qc_by_pa_other');
    bind_lov_reason('.cls_body_send_pp .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_TO_PP&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_pp_by_pa_other');

    //BY INTERNAL
    //bind_lov_reason('.cls_art_qc_after_cus .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    //bind_lov_reason('.cls_art_mk_after_cus .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');

    bind_lov_reason('.cls_task_form_intdep_artwork .cls_art_mk .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_MK_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_mk_pa_other');
    bind_lov_reason('.cls_task_form_intdep_artwork .cls_art_qc .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_QC_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_qc_pa_other');
    bind_lov_reason('.cls_task_form_intdep_artwork .cls_art_rd .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_RD_SEND_TO_QC&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_rd_pa_other');
    bind_lov_reason('.cls_task_form_intdep_artwork .cls_art_wh .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_WH_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_wh_pa_other');
    bind_lov_reason('.cls_task_form_intdep_artwork .cls_art_plan .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PN_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_plan_pa_other');
    bind_lov_reason('.cls_task_form_intdep_artwork .cls_art_pp .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PP_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_pp_pa_other');
    bind_lov_reason('.cls_task_form_intdep_artwork .cls_art_gm_mk .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_DL&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    bind_lov_reason('.cls_task_form_intdep_artwork .cls_art_gm_qc .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_DL&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');

    bind_lov_reason('#send_to_vendor_modal .cls_body_send_artwork_vendor .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_TO_VENDOR&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_vn_pm_by_pa_other');
    bind_lov_reason('#send_to_vendor_modal .cls_body_send_shade .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PA_SEND_TO_VENDOR_SHADE_LIMIT&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_vn_sl_by_pa_other');

    //bind_lov_reason('#qc_submit_modal .cls_body_send_pa .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    //bind_lov_reason('#qc_submit_modal .cls_body_send_mk .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    //bind_lov_reason('#qc_submit_modal .cls_body_send_gm_qc .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    bind_lov_reason('#qc_submit_modal .cls_body_send_rd .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_QC_SEND_TO_RD&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_rd_by_qc_other');

    bind_lov_reason('#mk_submit_modal .cls_body_send_pa .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_MK_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    //bind_lov_reason('#mk_submit_modal .cls_body_send_qc .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    //bind_lov_reason('#mk_submit_modal .cls_body_send_gm_mk .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');

    //bind_lov_reason('.cls_art_cus_review .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=MOCK_UP_PG_SEND_TO_VN_RS&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT');
    bind_lov_reason('.cls_art_cus_print .cls_lov_revise_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_CUSTOMER_PRINT_MASTER_SEND_TO_PA_REVISE&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_cus_print_revise_other');
    bind_lov_reason('.cls_art_cus_print .cls_lov_cancel_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_CUSTOMER_PRINT_MASTER_SEND_TO_PA_CANCEL&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_cus_print_cancel_other');
    bind_lov_reason('.cls_art_cus_shade .cls_lov_revise_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_CUSTOMER_SHADE_LIMIT_SEND_TO_PA_REVISE&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_cus_shade_revise_other');
    bind_lov_reason('.cls_art_cus_shade .cls_lov_cancel_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_CUSTOMER_SHADE_LIMIT_SEND_TO_PA_CANCEL&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_cus_shade_cancel_other');

    //BY VENDOR
    bind_lov_reason('.cls_vendor_printing_master .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_VENDOR_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_pm_other');
    bind_lov_reason('.cls_vendor_request_shade_limit .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_VENDOR_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_sl_other');

    bind_lov_reason('.cls_send_po_vendor .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_VENDOR_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_po_other');
    setValueToDDL('.cls_row_extend_step_duration .cls_lov_reason', stepDurationReasonId, stepDurationReasonText);

    bind_table_history();



    //start by aof IGRID_REIM_SPLINT2
    $('.cls_chk_pa_change_mat_desc').change(function () {
        if ($(this).is(":checked")) {
            $('.cls_txt_pa_mat_desc').prop('disabled', false);
        } else {
            $('.cls_txt_pa_mat_desc').prop('disabled', true);
        }
     
    });

    $('.cls_img_pa_refresh_mat_desc').click(function (e) {

        if ($(".cls_tfartwork_pa .cls_lov_pa_type_of").val() > 0 && $('.cls_tfartwork_pa .cls_input_pa_brand').val() != "" && ReadOnly == "0")
        {
            //alertError2('test');
            getMatDesc_ByMateiralRefMaterialTypeofBrand($(".cls_tfartwork_pa .cls_lov_pa_type_of").val(), $('.cls_tfartwork_pa .cls_input_pa_brand').val());
        }

    });

     //end by aof IGRID_REIM_SPLINT2


});
var linkDownloadFileTemplate = "";


function bindMatDesc_ByMateiralRefMaterialTypeofBrand() {
    // aof IGRID_REIM_SPLINT2

    var typeof_id = $(".cls_tfartwork_pa .cls_lov_pa_type_of").val();
    var brand = $('.cls_tfartwork_pa .cls_input_pa_brand').val()
    var material = $('.cls_container_taskform_artwork .cls_txt_header_tfartwork_mat_no').val();
    var ref_material = $('.cls_container_taskform_artwork .cls_txt_header_tfartwork_ref_mat').val();
     

    var param = 'data.ID=' + typeof_id + '&data.BRAND_DESCRIPTION=' + brand + '&data.MATERIAL_NO=' + material + '&data.REF_MATERIAL_NO=' + ref_material + '&data.WHERE_NOT_IN_CHARACTERISTIC_ID='+ ArtworkSubId ;

    var myurl = '/api/lov/pa/typeofandbrand?' + param;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bindMatDesc_ByMateiralRefMaterialTypeofBrand);
}

function callback_bindMatDesc_ByMateiralRefMaterialTypeofBrand(res) {
    // aof IGRID_REIM_SPLINT2
    //if ($('.cls_container_taskform_artwork .cls_txt_header_tfartwork_mat_no').val() != "") {

    //}
    //else
    if (res.data.length > 0 && $('.cls_txt_header_tfartwork_mat_status').val() != "Completed") {
        debugger;
        var matdesc = res.data[0].DISPLAY_TXT;
        $('.cls_txt_pa_mat_desc').val(matdesc);
    }
}



function callback_after_select_template_artwork(obj) {
    var myurl = '/api/common/template?data.template_id=' + $(obj).val();
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_after_select_template_artwork2);
}

function callback_after_select_template_artwork2(res) {
    $('.cls_pa_master_template .cls_txt_template_data').html(res.data[0].DESCRIPTION);
}

function acceptTaskformArtwork() {
    var jsonObj = new Object();
    var item = {};
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["CURRENT_USER_ID"] = UserID;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;
    var myurl = '/api/taskform/artworkprocess/accepttask';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (getroleuser("PG_STAFF")) {
        //myAjaxConfirmSubmit(myurl, mytype, mydata, callback_accept_taskformartwork, '', true, true);
        var urlArtWorkReq = '/api/artwork/artworkrequestbyartworkrequestno?data.ARTWORK_REQUEST_NO=' + $('.cls_txt_header_tfartwork_rf_no').val();
        var urlIncomingArtWork = '/api/dashboard/myincomingartworkforpg?data.ARTWORK_REQUEST_NO=' + $('.cls_txt_header_tfartwork_rf_no').val();
        var typeArtWorkReq = 'GET';
        var dataArtWorkReq = null;
        var countAllItem = 0;
        var countMyItem = 0;

        //get all item by request no.
        myAjax(urlArtWorkReq, typeArtWorkReq, dataArtWorkReq, function (res) {
            if (res.data.length > 0) {
                countAllItem = res.data[0].REQUEST_ITEMS.length;

                //get my item by request no.
                myAjax(urlIncomingArtWork, typeArtWorkReq, dataArtWorkReq, function (response) {
                    countMyItem = response.data.length;

                    var color = countAllItem != countMyItem ? "red" : "green";

                    //This workflow is from RF-xxxxx.
                    //All workflows under this request from: 10
                    //PA has already submited: 3
                    //Do you to continue?
                    var content = '<p style="color:' + color + '">This workflow is from ' + $('.cls_txt_header_tfartwork_rf_no').val() + '.<br/>All workflows under this request form : ' + countAllItem + '<br/>PA has already submited : ' + countMyItem + '</p><br/>Do you to continue?';

                    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback_accept_taskformartwork, '', true, true, content);
                });
            }
        });
    }
    else {
        myAjaxConfirmSubmit(myurl, mytype, mydata, callback_accept_taskformartwork, '', true, true);
    }
}

function callback_accept_taskformartwork(res) {
    bindTaskformArtwork(ArtworkSubId);
    //bindDataPG(MainMockupSubId);
}

function retrieveTaskformArtwork() {
    var jsonObj = new Object();
    var item = {};
    item["ARTWORK_NO"] = $('.cls_txt_header_tfartwork_aw_no').val();
    item["MATERIAL_NO"] = $('.cls_txt_header_tfartwork_mat_no').val();
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;
    var myurl = '/api/taskform/pa/retrivematerial';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, callbackRetrieveTaskformArtwork);
}

function callbackRetrieveTaskformArtwork(res) {
    if (res.status == 'S') {
        bindDataTaskFormPA();
        bindTaskformArtwork(ArtworkSubId);
        $('.cls_chk_header_tfartwork_3v').click();
    }
}

function setReadyToCreatePO_TaskformArtwork(is_check) {
    var readytocreatepo = '';
    if (is_check) {
        readytocreatepo = 'X';
    }
    var jsonObj = new Object();
    var item = {};
    item["READY_CREATE_PO"] = readytocreatepo;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;
    var myurl = '/api/taskform/pa/readycreatepo';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata);
}

function setEntendStepDuration_TaskformArtwork(is_check) {
    var extendStepDuration = '';
    if (is_check) {
        extendStepDuration = 'X';
    }
    var jsonObj = new Object();
    var item = {};
    item["IS_STEP_DURATION_EXTEND"] = extendStepDuration;
    item["STEP_DURATION_EXTEND_REASON_ID"] = $('.cls_row_extend_step_duration .cls_lov_reason').val();
    item["STEP_DURATION_EXTEND_REMARK"] = $('.cls_row_extend_step_duration .cls_input_reason_other').val();
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["UPDATE_BY"] = UserID;
    item["STEPNAME"] = CURRENT_STEP_CODE_DISPLAY_TXT;

    jsonObj.data = item;
    var myurl = '/api/taskform/artworkprocess/stepdurationextend';
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
            $('.cls_btn_header_tfartwork_extend_duration').hide();
            $('.extend_step_duration_warning').hide();
            $('.cls_row_extend_step_duration .cls_input_reason_other').prop('disabled', true);
        }
    });
}

function setReceivedShadeLimit_TaskformArtwork(is_check) {
    var receivedshadelimit = '';
    if (is_check) {
        receivedshadelimit = 'X';
    }
    var jsonObj = new Object();
    var item = {};
    item["RECEIVE_SHADE_LIMIT"] = receivedshadelimit;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;
    var myurl = '/api/taskform/pa/receiveshadelimit';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata);
}

function setShadeLimit_TaskformArtwork(v) {
    var jsonObj = new Object();
    var item = {};
    item["SHADE_LIMIT"] = v;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;
    var myurl = '/api/taskform/pa/shadelimit';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata);
}

function setChangePoint_TaskformArtwork(v) {
    var jsonObj = new Object();
    var item = {};
    item["CHANGE_POINT"] = v;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;
    var myurl = '/api/taskform/pa/changepoint';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, '', '', true, false);
}

function bindRemarkReason() {
    var myurl = '/api/taskform/remarkreason/info?data.artwork_sub_id=' + ArtworkSubId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_getRemark);
}

function callback_getRemark(res) {
    var item = res.data;
    if (item.length > 0) {
        var currentstepid = getstepartwork(CURRENT_STEP_CODE_DISPLAY_TXT).curr_step;
        for (var i = 0; i < item.length; i++) {
            if (item[i].WF_SUB_ID == ArtworkSubId && item[i].WF_STEP == currentstepid) {
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_QC") setValueToDDLOther('.cls_art_qc  .cls_input_qc_pa_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_RD") setValueToDDLOther('.cls_art_rd  .cls_input_rd_pa_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_WH") setValueToDDLOther('.cls_art_wh  .cls_input_wh_pa_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_PN") setValueToDDLOther('.cls_art_plan  .cls_input_plan_pa_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_MK") setValueToDDLOther('.cls_art_mk  .cls_input_mk_pa_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_PP") setValueToDDLOther('.cls_art_pp  .cls_input_pp_pa_other', item[i].REMARK_REASON);


                //---------------------------------changed by aof #INC-11265------------------------------------------------------------------------------
                 //if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PM") setValueToDDLOther('.cls_art_pp  .cls_input_pm_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PM") {
                    setValueToDDL('.cls_vendor_printing_master .cls_lov_send_for_reason', item[i].REASON_ID , gOthers);
                    setValueToDDLOther('.cls_vendor_printing_master  .cls_input_pm_other', item[i].REMARK_REASON); 
                }
                    
                //if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_SL") setValueToDDLOther('.cls_art_pp  .cls_input_sl_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_SL") {
                    setValueToDDL('.cls_vendor_request_shade_limit .cls_lov_send_for_reason', item[i].REASON_ID, gOthers);
                    setValueToDDLOther('.cls_vendor_request_shade_limit  .cls_input_sl_other', item[i].REMARK_REASON);
                    
                } 
                //if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PO") setValueToDDLOther('.cls_art_pp  .cls_input_po_other', item[i].REMARK_REASON);
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PO") {
                    setValueToDDL('.cls_send_po_vendor .cls_lov_send_for_reason', item[i].REASON_ID, gOthers);
                    setValueToDDLOther('.cls_send_po_vendor  .cls_input_po_other', item[i].REMARK_REASON);
                }
                //---------------------------------changed by aof #INC-11265------------------------------------------------------------------------------

            }
        }
    }
}

function bindTaskformArtwork(artwork_sub_id) {
    var myurl = '/api/taskform/artworkprocess/process?data.artwork_sub_id=' + artwork_sub_id;
    var mytype = 'GET';
    var mydata = null;
    myAjaxNoSync(myurl, mytype, mydata, callback_get_taskformartwork, callback_get_taskformartwork);
}

function bindFOC(artwork_sub_id) {
    var myurl = '/api/taskform/salesorderfoc/popup?data.artwork_sub_id=' + artwork_sub_id;
    var mytype = 'GET';
    var mydata = null;
    myAjaxNoSync(myurl, mytype, mydata, callback_get_foc, callback_get_foc);
}

function callback_get_foc(res) {
    if (res.data.length > 0)
        $('.cls_badge_warning_foc_viewchange').show();
    else
        $('.cls_badge_warning_foc_viewchange').hide();
}

var click_retrive_or_request_mat = false;
function callback_get_taskformartwork(res) {

    if (res.data.length > 0) {
        var v = res.data[0];
        //        //MOCKUPID = v.MOCKUP_ID;
        CURRENTUSERID = v.CURRENT_USER_ID;

        ARTWORK_NO = v.ARTWORK_NO_DISPLAY_TXT;
        //        NODE_ID_MOCKUP = v.NODE_ID_MOCKUP;
        CURRENT_STEP_CODE_DISPLAY_TXT = v.CURRENT_STEP_CODE_DISPLAY_TXT;
        $('.cls_txt_header_tfartwork_rf_no').val(v.ARTWORK_REQUEST_FORM_NO_DISPLAY_TXT);
        $('.cls_txt_header_tfartwork_aw_no').val(v.ARTWORK_NO_DISPLAY_TXT);
        $('.cls_txt_header_tfartwork_mu_no').val(v.MOCKUP_NO_DISPLAY_TXT);
        $('.cls_txt_header_tfartwork_ref_mat').val(v.REFERENCE_MATERIAL);
        $('.cls_txt_header_tfartwork_mat_no').val(v.MATERIAL_NO);

        $('.cls_txt_header_tfartwork_mat_status').val(v.MATERIAL_STATUS);
        $('.cls_txt_header_tfartwork_ref_form_no').val(v.ARTWORK_REFERENCE_REQUEST_NO_DISPLAY_TXT);
        //        $('.cls_pg_price_template_vendor .cls_qua_remark').val(v.REMARK);

        if (v.IS_STEP_DURATION_EXTEND == 'X') {
            stepDurationReasonId = v.STEP_DURATION_EXTEND_REASON_ID;
            stepDurationReasonText = v.STEP_DURATION_REMARK_REASON;
            //setValueToDDL('.cls_row_extend_step_duration .cls_lov_reason', stepDurationReasonId, stepDurationReasonText);
            $('.cls_row_extend_step_duration .cls_lov_reason').prop('disabled', true);
            $('.cls_btn_header_tfartwork_extend_duration').hide();
            $('.extend_step_duration_warning').hide();
            if (v.STEP_DURATION_EXTEND_REMARK != '') {
                setValueToDDLOther('.cls_row_extend_step_duration .cls_input_reason_other', v.STEP_DURATION_EXTEND_REMARK);
                $('.cls_row_extend_step_duration .cls_input_reason_other').prop('disabled', true);
            }
        }

        if (isEmpty(v.IS_END)) {
            $('.cls_header_curr_status_txt').val(v.CURRENT_STEP_DISPLAY_TXT);
        }
        else if (!isEmpty(v.IS_TERMINATE)) {
            $('.cls_header_curr_status_txt').val(v.CURRENT_STEP_DISPLAY_TXT + " [Terminated]");
        }
        else {
            $('.cls_header_curr_status_txt').val(v.CURRENT_STEP_DISPLAY_TXT + " [Completed]");
        }

        $('.cls_chk_header_tfartwork_shadelimit').prop('checked', (v.RECEIVE_SHADE_LIMIT == 'X'));

        if (v.READY_CREATE_PO == 'X') {
            $('.cls_chk_header_tfartwork_po').prop('checked', true);
        }
        else {
            $('.cls_chk_header_tfartwork_po').prop('checked', false);

        }

        if (v.IS_STEP_DURATION_EXTEND == 'X') {
            $('.cls_chk_header_tfartwork_extend_duration').prop('checked', true);
        }
        else {
            $('.cls_chk_header_tfartwork_extend_duration').prop('checked', false);
        }

        if (v.IS_READY_CREATE_PO == 'X') {
            $('.cls_container_taskform_artwork').find('.cls_chk_header_tfartwork_po').attr('disabled', false);
        }
        else {
            $('.cls_container_taskform_artwork').find('.cls_chk_header_tfartwork_po').attr('disabled', true);
           // $('.cls_chk_header_tfartwork_po').prop('checked', false);
        }

        if (isEmpty(v.READY_CREATE_PO_VALIDATE_MSG)) {
            $('#pa_submit_modal').find('.cls_chk_send_pp').attr('disabled', false);
        }
        else {
            $('#pa_submit_modal').find('.cls_chk_send_pp').attr('disabled', true);
        }

        $('.cls_row_header_tfartwork_po_msg').hide();
        $('.cls_lbl_header_tfartwork_po_msg').html('');
        if (!isEmpty(v.READY_CREATE_PO_VALIDATE_MSG) && (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PA')) {
            //if (!isEmpty(v.READY_CREATE_PO_VALIDATE_MSG) && (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PA') && (v.READY_CREATE_PO != 'X')) {
            $('.cls_row_header_tfartwork_po_msg').show();
            $('.cls_lbl_header_tfartwork_po_msg').html(v.READY_CREATE_PO_VALIDATE_MSG);
        }

         //start ticket 445558 by aof add cls_row_header_tfartwork_mk_msg, cls_lbl_header_tfartwork_mk_msg
        if (isEmpty(v.CHECK_SO_REPEAT_IS_NOT_SEND_BACK_MK)) {
            $('#pa_submit_modal').find('.cls_chk_send_mk').attr('disabled', false);
        }
        else {
            $('#pa_submit_modal').find('.cls_chk_send_mk').attr('disabled', true);
        }
        
        $('.cls_row_header_tfartwork_mk_msg').hide();
        $('.cls_lbl_header_tfartwork_mk_msg').html('');
        if (!isEmpty(v.CHECK_SO_REPEAT_IS_NOT_SEND_BACK_MK)) {
            $('.cls_row_header_tfartwork_mk_msg').show();
            $('.cls_lbl_header_tfartwork_mk_msg').html(v.CHECK_SO_REPEAT_IS_NOT_SEND_BACK_MK);
        }
        //last ticket 445558 by aof add cls_row_header_tfartwork_mk_msg, cls_lbl_header_tfartwork_mk_msg

        $("input[name=cls_rdo_haeder_tfartwork_change_point][value=" + v.CHANGE_POINT + "]").prop('checked', true);
        $("input[name=cls_rdo_haeder_tfartwork_shadelimit][value=" + v.SHADE_LIMIT + "]").prop('checked', true);
        if (v.SHADE_LIMIT == 1)
            $("input[name=pa_rdo_request_shade_limit][value=0]").prop('checked', true);
        else if (v.SHADE_LIMIT == 0)
            $("input[name=pa_rdo_request_shade_limit][value=1]").prop('checked', true);

        $('.cls_a_tfartwork_filename').attr("href", function (i, href) {
            return href + '?nodeIdTxt=' + v.NODE_ID_TXT;
        });
        $(".cls_img_tfartwork").attr("src", function () {
            if (v.FILE_EXTENSION.toUpperCase() == "PNG" || v.FILE_EXTENSION.toUpperCase() == "JPG" || v.FILE_EXTENSION.toUpperCase() == "JPEG") {
                this.src = suburl + "/FileUpload/Download_Artwork?nodeIdTxt=" + v.NODE_ID_TXT;
            } else {
                $('.cls_a_tfartwork_filename').html(v.FILE_NAME);
            }
        });

        if (isEmpty(v.IS_SO_CHANGE)) {
            $('.cls_badge_warning_salesorder_viewchange').hide();
            $('.cls_btn_tfartwork_salesorder_viewchange').hide();
            IS_SO_CHANGE = false;   // by aof show btn so change for admin
        }
        else {
            $('.cls_badge_warning_salesorder_viewchange').show();
            $('.cls_btn_tfartwork_salesorder_viewchange').show();
            IS_SO_CHANGE = true;   // by aof show btn so change for admin
        }

        //$('.cls_img_tfartwork').each(function () {
        //    var foo = $(this).parent().attr('href').split('=');
        //    
        //    $(this).attr('src', '/' + foo[1] + '.jpg');
        //})

        //$('.cls_img_tfartwork').attr('src', '@Url.Action("Download_Artwork","FileUpload", new { nodeId = '+v.NODE_ID+' })');
        setPermissionOfHeaderTaskFormArtwork(true);
        if (isEmpty(CURRENTUSERID)) {
            $('.cls_btn_header_tfartwork_accept').show();
            $('a[href="#view_artwork"]').tab('show');
            $('.cls_li_artwork_data').show();
            $('.cls_li_history_data').show();
        }
        else {
            $('.cls_row_extend_step_duration').show();
            $('.cls_btn_header_tfartwork_accept').hide();
            if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PA') {

                setPermissionOfHeaderTaskFormArtwork(false);

                if (click_retrive_or_request_mat == false)
                    $('a[href="#view_artwork"]').tab('show');

                click_retrive_or_request_mat == false;

                $('.cls_li_artwork_data').show();
                $('.cls_li_salesorder_data').show();
                $('.cls_li_pa_data').show();
                $('.cls_tfartwork_pa .btn').show();
                $('.cls_tfartwork_pa select').prop("disabled", false);
                //$('.cls_tfartwork_pa input').prop("disabled", false);
                $('.cls_tfartwork_pa textarea').prop("disabled", false);
                $('.cls_img_lov_tfartwork_pa_add_fao_zone_multiple').show();
                $('.cls_img_lov_tfartwork_pa_add_catching_area_multiple').show();
                $('.cls_img_lov_tfartwork_pa_add_catching_method_multiple').show();  // ticke#425737 added by aof 
                $('.cls_img_lov_tfartwork_pa_add_symbol_multiple').show();
                $('.cls_img_lov_tfartwork_pa_add_plant_multiple').show();
               

                $('.cls_li_pg_data').show();
                $('.cls_task_form_pg_artwork .btn').hide();

                $('.cls_tfartwork_pa .cls_txt_pa_pa_username').prop("disabled", true);
                $('.cls_tfartwork_pa .cls_txt_pa_pg_username').prop("disabled", true);
                $('.cls_tfartwork_pa .cls_txt_pa_vendor').prop("disabled", true);

                $('.cls_tfartwork_pa .cls_input_pa_brand').prop("disabled", true);
                $('.cls_tfartwork_pa .cls_input_pa_sec_pkg_production_plant').prop("disabled", true);
                $('.cls_tfartwork_pa .cls_txt_pa_country').prop("disabled", true);
                $("input[name=pa_rdo_customer_shade_limit]").prop('disabled', true);
                $('.cls_txt_pa_productcode_item_productcode').prop("disabled", true);
                $('.cls_txt_pa_productcode_item_productdescription').prop("disabled", true);
                $('.cls_txt_pa_productcode_item_net_weight').prop("disabled", true);
                $('.cls_txt_pa_productcode_item_drain_weight').prop("disabled", true);
                $('.cls_txt_pa_productcode_item_productcode2').prop("disabled", true);

                $('.cls_li_po_data').show();
                $('.cls_li_vendor_data').show();
                $('.cls_li_customer_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                if (getroleuser("PG_STAFF") || getroleuser("PG_TEAM_LEAD") || getroleuser("PG_SUPPERVISOR") || getroleuser("PG_MANAGER") || getroleuser("ADMINISTRATOR_PK") || getroleuser("ADMINISTRATOR")
                    || getroleuser("PA_STAFF") || getroleuser("PA_TEAM_LEAD") || getroleuser("PA_SUPERVISOR") || getroleuser("PA_ASS_MANAGER")) {
                    $('.cls_li_attachment_data').show();
                }

                $('.cls_task_form_intdep_artwork .cls_art_mk').show();
                $('.cls_task_form_intdep_artwork .cls_art_qc').show();
                $('.cls_task_form_intdep_artwork .cls_art_wh').show();
                $('.cls_task_form_intdep_artwork .cls_art_plan').show();
                $('.cls_task_form_intdep_artwork .cls_art_rd').show();
                $('.cls_task_form_intdep_artwork .cls_art_pp').show();
                $('.cls_task_form_intdep_artwork .cls_art_mk_after_cus').show();
                $('.cls_task_form_intdep_artwork .cls_art_qc_after_cus').show();
                $('.cls_task_form_intdep_artwork .cls_art_gm_qc').show();
                $('.cls_task_form_intdep_artwork .cls_art_gm_mk').show();


                if (v.MATERIAL_STATUS != null && v.MATERIAL_STATUS != '') {

                    $('.cls_container_taskform_artwork').find('.cls_btn_pa_update_material').attr('disabled', false);

                    if (v.MATERIAL_STATUS == "Completed") {
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_request_material').attr('disabled', true);
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_update_material').attr('disabled', false);
                    }
                    else if ((v.MATERIAL_NO == null || v.MATERIAL_NO == "") && v.MATERIAL_STATUS == "Canceled") {
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_request_material').attr('disabled', false);
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_update_material').attr('disabled', true);

                    }
                    else if (v.MATERIAL_STATUS == "Canceled") {
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_request_material').attr('disabled', true);
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_update_material').attr('disabled', false);
                    }
                    else if (v.MATERIAL_STATUS == "Waiting for approval") {

                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_update_material').attr('disabled', false);
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_request_material').attr('disabled', false);
                    }
                    else {
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_request_material').attr('disabled', false);
                        $('.cls_container_taskform_artwork').find('.cls_btn_pa_update_material').attr('disabled', true);
                    }
                }
                else if (v.MATERIAL_STATUS == "Waiting for approval") {

                    $('.cls_container_taskform_artwork').find('.cls_btn_pa_update_material').attr('disabled', false);
                    $('.cls_container_taskform_artwork').find('.cls_btn_pa_request_material').attr('disabled', false);
                }
                else {

                    $('.cls_container_taskform_artwork').find('.cls_btn_pa_update_material').attr('disabled', true);
                    $('.cls_container_taskform_artwork').find('.cls_btn_pa_request_material').attr('disabled', false);
                }
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                $('.cls_pg_position').show();
                $('.cls_btn_header_tfartwork_ref_mat').hide();
                $('.cls_btn_header_tfartwork_clear_ref_mat').hide();

                if (getroleuser("PG_STAFF") || getroleuser("PG_TEAM_LEAD") || getroleuser("PG_SUPPERVISOR") || getroleuser("PG_MANAGER") || getroleuser("ADMINISTRATOR_PK") || getroleuser("ADMINISTRATOR")
                    || getroleuser("PA_STAFF") || getroleuser("PA_TEAM_LEAD") || getroleuser("PA_SUPERVISOR") || getroleuser("PA_ASS_MANAGER")) {
                    $('.cls_li_attachment_data').show();
                }

                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_history_data').show();
                $('.cls_li_pg_data').show();
                $('.cls_tfartwork_pa .btn').hide();
                $('.cls_tfartwork_pa select').prop("disabled", true);
                //$('.cls_tfartwork_pa input').prop("disabled", true);
                $('.cls_tfartwork_pa textarea').prop("disabled", true);
                $('.cls_img_lov_tfartwork_pa_add_fao_zone_multiple').hide();
                $('.cls_img_lov_tfartwork_pa_add_catching_area_multiple').hide();
                $('.cls_img_lov_tfartwork_pa_add_catching_method_multiple').hide(); // ticke#425737 added by aof 
                $('.cls_img_lov_tfartwork_pa_add_symbol_multiple').hide();
                $('.cls_img_lov_tfartwork_pa_add_plant_multiple').hide();
                $('.cls_img_lov_tfartwork_pa_delete_fao_zone_multiple').hide();
                $('.cls_img_lov_tfartwork_pa_delete_catching_area_multiple').hide();
                $('.cls_img_lov_tfartwork_pa_delete_catching_method_multiple').hide(); // ticke#425737 added by aof 
                $('.cls_img_lov_tfartwork_pa_delete_symbol_multiple').hide();
                $('.cls_img_lov_tfartwork_pa_delete_plant_multiple').hide();

                $('.cls_li_pa_data').show();
                if (getroleuser("PA_STAFF") || getroleuser("PA_TEAM_LEAD") || getroleuser("PA_SUPERVISOR") || getroleuser("PA_ASS_MANAGER")) {
                    $('.cls_li_attachment_data').show();
                }
                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();

            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_MK') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_mk').show();

                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_QC') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_qc').show();
                $('.cls_task_form_intdep_artwork .cls_art_rd').show();

                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_RD') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_rd').show();

                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_WH') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_wh').show();

                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();
            }
            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PN') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_plan').show();

                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_GM_QC') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_gm_qc').show();
                $('.cls_task_form_intdep_artwork .cls_art_gm_qc .cls_div_only_gm_qc').show();

                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_GM_MK') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_gm_mk').show();
                $('.cls_task_form_intdep_artwork .cls_art_gm_mk .cls_div_only_gm_mk').show();

                $('.cls_li_salesorder_data').show();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_QC_VERIFY') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_qc_after_cus').show();
                $('.cls_task_form_intdep_artwork .cls_art_qc_after_cus .cls_div_only_qc_after_cus').show();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_MK_VERIFY') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_mk_after_cus').show();
                $('.cls_task_form_intdep_artwork .cls_art_mk_after_cus .cls_div_only_mk_after_cus').show();

                $('#send_to_customer_modal .cls_for_pa_only').hide();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PA_VERIFY') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_pa_data').show();
                $('.cls_li_customer_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_pa_after_cus').show();
                $('.cls_task_form_intdep_artwork .cls_art_pa_after_cus .cls_div_only_pa_after_cus').show();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_BACK_MK') {
                ARTWORK_REQUEST_READONLY = 0;
                showBtnArtwork(ARTWORK_REQUEST_ID);
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_mk').show();

                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_PM' || v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_SL' || v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_PO') {
                $('a[href="#view_vendor"]').tab('show');
                $('.cls_li_vendor_data').show();
                $('.cls_li_pg_data').show();
                load_tab_vendor_artwork();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REVIEW' || v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_SHADE' || v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_PRINT' || v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REF' || v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_CUS_REQ_REF') {
                $('a[href="#view_customer"]').tab('show');
                $('.cls_li_customer_data').show();
                load_tab_customer_artwork();
            }

            else if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PP') {
                $('a[href="#view_artwork"]').tab('show');
                $('.cls_li_artwork_data').show();
                $('.cls_li_internal_data').show();
                $('.cls_li_history_data').show();
                $('.cls_li_po_data').show();
                $('.cls_task_form_intdep_artwork .cls_art_pp').show();
                $('.cls_task_form_intdep_artwork .cls_art_pp .cls_div_only_pp').show();
                $('.cls_li_salesorder_data').show();
                $('.cls_tfartwork_salesorder .btn').hide();
            }
        }
        if (v.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PP') {
            $('.cls_btn_header_tfartwork_accept').hide();
        }


        bindMatDesc_ByMateiralRefMaterialTypeofBrand(); // by aof IGRID_REIM_SPLINT3
    }
}

function setPermissionOfHeaderTaskFormArtwork(is_disabled) {
    if (is_disabled) {
        $('.cls_only_pa_position').hide();
    }
    else {
        $('.cls_only_pa_position').show();
    }

    var matStatus = $('.cls_txt_header_tfartwork_mat_status').val();
    var matNo = $('.cls_txt_header_tfartwork_mat_no').val();
    if (matStatus == 'Waiting for approval' || matStatus == 'Completed') {
        is_disabled = true;
    }

    $('.cls_container_taskform_artwork').find('.cls_btn_header_tfartwork_ref_mat').attr('disabled', is_disabled);
    $('.cls_container_taskform_artwork').find('.cls_btn_header_tfartwork_clear_ref_mat').attr('disabled', is_disabled);
    $('.cls_container_taskform_artwork').find('.cls_chk_header_tfartwork_3v').attr('disabled', is_disabled);
    //$('.cls_container_taskform_artwork').find('.cls_chk_header_tfartwork_shadelimit').attr('disabled', is_disabled);
    //$('.cls_container_taskform_artwork').find('.cls_chk_header_tfartwork_po').attr('disabled', is_disabled);
    $('.cls_container_taskform_artwork input[name="cls_rdo_haeder_tfartwork_change_point"]').attr('disabled', is_disabled);

  
    if (matStatus == 'Waiting for approval')
    {
        $('.cls_container_taskform_artwork').find('.cls_chk_pa_change_mat_desc').attr('disabled', true);
        $('.cls_container_taskform_artwork').find('.cls_img_pa_refresh_mat_desc').hide();
    } else
    {
        $('.cls_container_taskform_artwork').find('.cls_img_pa_refresh_mat_desc').show();
    }

}

function deleteSuggestMaterial() {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["UPDATE_BY"] = UserID;
    jsonObj.data = item;

    var myurl = '/api/taskform/pa/suggestmaterial/delete';
    var mytype = 'DELETE';
    var mydata = jsonObj;
    myAjaxConfirmDeleteWithContent(myurl, mytype, mydata, callback_delete_suggestmaterial, "Do you want to clear suggest material?");
}

function callback_delete_suggestmaterial(res) {
    if (res.status == 'S') {
        bindTaskformArtwork(ArtworkSubId);
    }
}




function bindSuggestMaterialPopUp(serverSide,autosearch) {

  
    var table_tfartwork_suggest_material = $('#table_tfartwork_suggest_material').DataTable();
    table_tfartwork_suggest_material.destroy();

     //ticket#437764 added by aof commented on 30/03/2021
    var cls_chk_suggest_material_no_search_val = autosearch;  //auto="0", manual="1"
    //var cls_chk_suggest_material_no_search_val = "0";
    //var cls_chk_suggest_material_no_search = $('#modal_tfartwork_suggest_material .cls_chk_suggest_material_no_search').is(":checked");
    //if (cls_chk_suggest_material_no_search) cls_chk_suggest_material_no_search_val = "1";
    //ticket#437764 added by aof commented on 30/03/2021

    //ticket#437764 added by aof on 30/03/2021 start
    //var product_code_id = $('.cls_tfartwork_pa .cls_lov_pa_product_code').val();   //ticket#437764 added by aof on 30/03/2021
    //if (!$.isNumeric(product_code_id))
    //    product_code_id = "0";

    var where_criteria = "";
    if (autosearch=="1")
    {
        where_criteria = "&data.SEARCH_MAT_FG=" + $('#modal_tfartwork_suggest_material .cls_txt_material_fg').val()
            + "&data.SEARCH_MAT_PK=" + $('#modal_tfartwork_suggest_material .cls_txt_material_pk').val()
            + "&data.SEARCH_BRAND_ID=" + $('#modal_tfartwork_suggest_material .cls_lov_brand').val()
            + "&data.SEARCH_SOLD_TO_ID=" + $('#modal_tfartwork_suggest_material .cls_lov_sold_to').val()
            + "&data.SEARCH_SHIP_TO_ID=" + $('#modal_tfartwork_suggest_material .cls_lov_ship_to').val();       
    }

    //ticket#437764 added by aof on 30/03/2021 end
    table_tfartwork_suggest_material = $('#table_tfartwork_suggest_material').DataTable({
        serverSide: serverSide,
        ajax: function (data, callback, settings) {
            if (serverSide) {
                for (var i = 0, len = data.columns.length; i < len; i++) {
                    delete data.columns[i].name;
                    delete data.columns[i].data;
                    delete data.columns[i].searchable;
                    delete data.columns[i].orderable;
                    delete data.columns[i].search.regex;
                    delete data.search.regex;

                    delete data.columns[i].search.value;
                }
            }
            $.ajax({
                url: suburl + "/api/taskform/pa/suggestmaterial/info?data.artwork_request_id=" + ARTWORK_REQUEST_ID + "&data.artwork_sub_id=" + ArtworkSubId + '&data.all_data=' + cls_chk_suggest_material_no_search_val + where_criteria, //ticket#437764 commented by aof on 30/03/2021
               // url: suburl + "/api/taskform/pa/suggestmaterial/info?data.artwork_request_id=" + ARTWORK_REQUEST_ID + "&data.artwork_sub_id=" + ArtworkSubId + "&data.all_data=" + cls_chk_suggest_material_no_search_val + "&data.product_code_id=" + product_code_id, //ticket#437764 added by aof on 30/03/2021 PRODUCT_CODE_ID
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        fixedColumns: {
            leftColumns: 1
        },
        "scrollX": true,
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            { data: "MATERIAL", "className": "cls_nowrap" },
            { data: "MATERIAL_NUMBER", "className": "cls_nowrap" },
            { data: "BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "SHIP_TO_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "ADDITIONAL_BRAND_ID", "className": "cls_nowrap" },
            { data: "ROUTE", "className": "cls_nowrap" },
            { data: "INTRANSIT_PORT", "className": "cls_nowrap" },
            { data: "SALES_ORGANIZATION", "className": "cls_nowrap" },
            { data: "PLANT", "className": "cls_nowrap" },
            { data: "COUNTRY_KEY", "className": "cls_nowrap" },
            { data: "START_DATE", "className": "cls_nowrap cls_startdate" },
            { data: "END_DATE", "className": "cls_nowrap cls_enddate" }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            if (!isEmpty(data.START_DATE)) {
                $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            }
            if (!isEmpty(data.END_DATE)) {
                $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            }
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });

}

function setSelectSuggestMaterial(tblData) {
    var item = {};
    if (tblData.length > 0) {
        item["MATERIAL_NUMBER"] = tblData[0].MATERIAL_NUMBER;
        item["ARTWORK_SUB_ID"] = ArtworkSubId;
        item["UPDATE_BY"] = UserID;
    }
    var jsonObj = new Object();
    jsonObj.data = item;

    var myurl = '/api/taskform/pa/suggestmaterial/selected';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSelectSuggestMaterial);
}

function callbackSelectSuggestMaterial(res) {
    if (res.status == 'S') {
        $("#modal_tfartwork_suggest_material .cls_btn_tfartwork_suggest_material_close").click();
        if (res.data.length > 0) {
            $('.cls_txt_header_tfartwork_ref_mat').val(res.data[0].MATERIAL_NUMBER);

            var result = res.data[0].MATERIAL_NUMBER.substring(8, 9);
            if (result == 'N') {
                $("input[name=cls_rdo_haeder_tfartwork_change_point][value=0]").prop('checked', true);
                setChangePoint_TaskformArtwork(0);
            }
            else if (result == 'C') {
                $("input[name=cls_rdo_haeder_tfartwork_change_point][value=1]").prop('checked', true);
                setChangePoint_TaskformArtwork(1);
            }
        }
        bindDataTaskFormPA();
    }
}


function ValidateKey() {
    var key = window.event.keyCode;
    var allowed = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ -_()[]/+&,.1234567890';

    return allowed.indexOf(String.fromCharCode(key)) != -1;
}
