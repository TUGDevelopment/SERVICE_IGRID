var MOCKUPSUBPGID = 0;
var table_search_for_dieline = $('#table_taskform_pg_search_for_dieline').DataTable();
var first_load = true;
$(document).ready(function () {


    $('#table_taskform_pg_search_for_dieline thead tr').clone(true).appendTo('#table_taskform_pg_search_for_dieline thead');
    $('#table_taskform_pg_search_for_dieline thead tr:eq(1) th').each(function (i) {
        if (i == 0) {
            $(this).html('');
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control cls_table_text_search" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });


    

    //$('#table_taskform_pg_search_for_dieline').DataTable({
    //    orderCellsTop: true,
    //    fixedHeader: true,
    //    lengthChange: false,
    //    scrollX: true,
    //    search: false,
    //    columnDefs: [{
    //        "searchable": false,
    //        "orderable": false,
    //        "targets": [0]
    //    }],

    //});


    $('#pg_submit_modal form').validate({
        rules: {
            send_to_warehouse:
            {
                required: true
            }
        },
        messages: {
            send_to_warehouse:
            {
                required: "Please fill at least 1 of these fields."
            }
        }
    });

    bind_lov('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other', '/api/lov/primarytype', 'data.DISPLAY_TXT');
    bind_lov('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search', '/api/lov/packtype', 'data.DISPLAY_TXT');
    bind_lov('#modal_taskform_pg_search_for_dieline .cls_lov_pack_size', '/api/lov/packsizeXecm', 'data.DISPLAY_TXT');

    $("a[href='#view_pg']").on('shown.bs.tab', function (e) {
        if (tablDieline != null)
            tablDieline.columns.adjust().draw();
    });

    var pg_submit_modal = "#pg_submit_modal ";
    var text_editor_pn = bind_text_editor(pg_submit_modal + '.cls_txtedt_send_planning');
    var text_editor_rd = bind_text_editor(pg_submit_modal + '.cls_txtedt_send_rd');
    var text_editor_supervisor = bind_text_editor(pg_submit_modal + '.cls_txtedt_send_supervisor');
    var text_editor_approval_match_board = bind_text_editor(pg_submit_modal + '.cls_txtedt_send_approval_match_board');
    var text_editor_wh = bind_text_editor(pg_submit_modal + '.cls_txtedt_send_warehouse');
    var text_editor_supervisor_need_design = bind_text_editor(pg_submit_modal + '.cls_txtedt_send_supervisor_need_design');
    var text_editor_send_customer = bind_text_editor('#pg_submit_modal_to_customer ' + '.cls_txtedt_send_customer');
    var text_editor_send_back_mk = bind_text_editor('#popup_send_to_checklist_creator ' + '.cls_txtedt_send_back_mk');
    var text_editor_terminate_workflow = bind_text_editor('#popup_terminate_workflow_mockup ' + '.cls_txtedt_terminate_workflow');
    var text_editor_killprocess_workflow = bind_text_editor('#popup_killprocess_workflow_mockup ' + '.cls_txtedt_terminate_workflow');

    bind_lov('.cls_lov_pg_packaging_type', '/api/lov/packtype', 'data.DISPLAY_TXT');
    $('.cls_lov_pg_packaging_type').on("change", function () {
        $('.cls_lov_pg_style').val('').trigger("change");
        $('.cls_lov_pg_pim_color').val('').trigger("change");
        // by aof 202306 for CR#IGRID_REIM
       // alertError2($(".cls_lov_pg_packaging_type  option:selected").val());
        $('.cls_lov_rsc_di_cut').val('').trigger("change");
        setFieldDisablePackagingType();
        bind_lov_param('.cls_lov_rsc_di_cut', '/api/lov/dicut', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type'], '');
     // by aof 202306 for CR#IGRID_REIM

    });
    bind_lov_param('.cls_lov_pg_style', '/api/lov/style', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type'], '.cls_input_pg_style_other');
    bind_lov('.cls_lov_pg_pim', '/api/lov/printsystem', 'data.DISPLAY_TXT', '.cls_input_pg_pim_other');
    bind_lov_param('.cls_lov_pg_pim_color', '/api/lov/numberofcolor', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type'], '.cls_input_pg_pim_color_other');
    bind_lov('.cls_lov_pg_box_color', '/api/lov/boxcolor', 'data.DISPLAY_TXT', '.cls_input_pg_box_color_other');
    bind_lov('.cls_lov_pg_coating', '/api/lov/coating', 'data.DISPLAY_TXT', '.cls_input_pg_coating_other');
    bind_lov_param('.cls_lov_grade_of', '/api/lov/gradeof', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type'], '.cls_input_grade_of');
     //bind_lov('.cls_lov_rsc_di_cut', '/api/lov/dicut', 'data.DISPLAY_TXT', '.cls_input_rsc_di_cut');  // by aof 202306 for CR#IGRID_REIM
    bind_lov_param('.cls_lov_rsc_di_cut', '/api/lov/dicut', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type'], '');  // by aof 202306 for CR#IGRID_REIM
    bind_lov('.cls_lov_vendor', '/api/lov/vendorhasuser', 'data.vendor_name', '');
    $('.cls_task_form_pg .cls_lov_vendor').prop('disabled', true);
    bind_lov_param('.cls_lov_flute', '/api/lov/flute', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type'], '.cls_input_flute');

    bind_lov_no_ajax('.cls_lov_roll_sheet', '', '.cls_input_roll_sheet');
    bind_lov_no_ajax('.cls_whmu_lov_test_pack_result_reject', 'Please select rejection code...', '');

    //---------by aof 20220118 for CR sustain-- - start
    bind_lov('.cls_lov_pkg_sec_material', '/api/lov/packaging_sec_material', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_pkg_sec_plastic', '/api/lov/packaging_sec_plastic_type', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_pkg_sec_certification_standard', '/api/lov/packaging_sec_cert_source', 'data.DISPLAY_TXT');

    
    $('.cls_txt_acessories').attr("required", true); // by aof 202306 for CR#IGRID_REIM


    $(".cls_lov_pkg_sec_material").change(function () {
       // alert($(".cls_lov_pkg_sec_material").text());
        var lbl_spanStar = "<span style=\"color: red;\">*</span> :";
        if ($(".cls_lov_pkg_sec_material  option:selected").text() == 'Plastic')
        {
            $('.cls_lbl_pkg_sec_plastic').html('Type of plastic packaging' + lbl_spanStar);
            $('.cls_lov_pkg_sec_plastic').attr("required", true);
            $('.cls_lov_pkg_sec_plastic').attr("disabled", false);
        } else
        {
            $('.cls_lbl_pkg_sec_plastic').html('Type of plastic packaging :');
            $('.cls_lov_pkg_sec_plastic').attr("required", false);
            $('.cls_lov_pkg_sec_plastic').attr("disabled", true);
            setValueToDDL('.cls_lov_pkg_sec_plastic','', '');
        }

        if ($(".cls_lov_pkg_sec_material").prop("disabled"))
        {
            $('.cls_lov_pkg_sec_plastic').attr("disabled", true);
        }


    });


    $(".cls_txt_pkg_sec_other").blur(function () {
        var val = $(".cls_txt_pkg_sec_other").val();
        if (val.length > 30) {
            alertError2("Other entered exceeds the maximum length (30 characters).");
            $(".cls_txt_pkg_sec_other").focus();
            $(".cls_txt_pkg_sec_other").select();
        }
        // $(".cls_txt_pkg_sec_weight").val(setStringToNumber(weg));
    });

    $(".cls_txt_pkg_sec_weight").blur(function () {
        var val = $(".cls_txt_pkg_sec_weight").val();
       
        if (setStringToNumber(val) >= 1000000000000) {
            alertError2("Packaging weight entered exceeds the maximum length or not in a properly formatted (999,999,999,999.999).");
            $(".cls_txt_pkg_sec_weight").focus();
            $(".cls_txt_pkg_sec_weight").select();
        } else
        {
            $(".cls_txt_pkg_sec_weight").val(setNumberToStringFormat(val, 3));
        }
       
       // $(".cls_txt_pkg_sec_weight").val(setStringToNumber(weg));
    });
    $('.cls_txt_pkg_sec_weight').keypress(function (event) {
        return isKeyDeicmal(event, this)
    });
    $(".cls_txt_pkg_sec_recycled_content").blur(function () {

        var val = $(".cls_txt_pkg_sec_recycled_content").val();
        if (val > 100)
        {
            alertError2("Packaging recycled content must be 0-100.");
            $(".cls_txt_pkg_sec_recycled_content").focus();
            $(".cls_txt_pkg_sec_recycled_content").select();
             
        }
        else
        {
            $(".cls_txt_pkg_sec_recycled_content").val(setStringToNumber(val, 0));
        }
       
    });

    $('.cls_txt_pkg_sec_recycled_content').keypress(function (event) {
        return isKeyNumber(event, this)
    });
   


    //---------by aof 20220118 for CR sustain-- - end

    //bind_lov('.cls_lov_pg_packaging_type_search', '/api/lov/packtype', 'data.description');
    //bind_lov('.cls_lov_primary_size', '/api/lov/primarysizeXecm', 'data.DISPLAY_TXT');
    //bind_lov('.cls_lov_pack_size', '/api/lov/packsizeXecm', 'data.DISPLAY_TXT');
    //bind_lov('.cls_lov_packing_style', '/api/lov/packingstyle', 'data.DISPLAY_TXT');

    //bind_lov('.cls_task_form_pg .cls_lov_pg_brand', '/api/lov/brand', 'data.DISPLAY_TXT');
    //bind_lov('.cls_task_form_pg .cls_lov_pg_primary_type', '/api/lov/primarytype', 'data.DISPLAY_TXT');
    bind_lov('.cls_task_form_pg .cls_lov_pg_primary_size', '/api/lov/3p', 'data.DISPLAY_TXT');
    bind_lov('.cls_task_form_pg .cls_lov_pg_pack_size', '/api/lov/packsize', 'data.DISPLAY_TXT');

    $(pg_submit_modal + '.cls_chk_send_planning').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal + '.cls_body_send_planning').show();
        }
        else {
            $(pg_submit_modal + '.cls_body_send_planning').hide();
        }
    });
    $(pg_submit_modal + '.cls_chk_send_rd').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal + '.cls_body_send_rd').show();
        }
        else {
            $(pg_submit_modal + '.cls_body_send_rd').hide();
        }
    });
    $(pg_submit_modal + '.cls_chk_send_supervisor').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal + '.cls_body_send_supervisor').show();
        }
        else {
            $(pg_submit_modal + '.cls_body_send_supervisor').hide();
        }
    });
    $(pg_submit_modal + '.cls_chk_send_approval_match_board').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal + '.cls_body_send_approval_match_board').show();
        }
        else {
            $(pg_submit_modal + '.cls_body_send_approval_match_board').hide();
        }
    });
    $(pg_submit_modal + '.cls_chk_send_supervisor_need_design').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal + '.cls_body_send_supervisor_need_design').show();
        }
        else {
            $(pg_submit_modal + '.cls_body_send_supervisor_need_design').hide();
        }
    });
    $(pg_submit_modal + '.cls_chk_send_warehouse').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal + '.cls_body_send_warehouse').show();
            $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_supplier_primary_container').prop('required', true);
            $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_supplier_primary_lid').prop('required', true);
            $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_remark').prop('required', true);
        }
        else {
            $(pg_submit_modal + '.cls_body_send_warehouse').hide();
            $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_supplier_primary_container').prop('required', false);
            $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_supplier_primary_lid').prop('required', false);
            $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_remark').prop('required', false);
        }
    });

    
    

     //---------by aof 20220118 for CR sustain-- start
    function checkFieldRequiredSustain()
    {
        var f_req = false ;

        if ($('.cls_lov_pkg_sec_plastic').prop("required") && !($('.cls_lov_pkg_sec_plastic').val() > 0))
        {
            $(".se-pre-con").fadeOut('fast');
            alertError2("Please input Type of plastic packaging.");
            $(".cls_lov_pkg_sec_plastic").focus();
            $(".cls_lov_pkg_sec_plastic").select();
            f_req = true;
        }

        return f_req;
    }
     //---------by aof 20220118 for CR sustain-- end


    $(".cls_task_form_pg .cls_btn_save").click(function () {
        //---------by aof 20220118 for CR sustain-- start
        if (checkFieldRequiredSustain())
        {
            return false;
        }
        //---------by aof 20220118 for CR sustain-- end

        saveDataPG(true, true);
    });

    $(".cls_task_form_pg .cls_btn_submit").click(function () {

        //---------by aof 20220118 for CR sustain-- start
        if (checkFieldRequiredSustain()) {
            return false;
        }
         //---------by aof 20220118 for CR sustain-- end

        var error_send_to_pf_sup_qua = false;
        $(".cls_div_select_vendor form :input:disabled").each(function () {
            if ($(this).prop('required')) {
                if (isEmpty($(this).val())) {
                    error_send_to_pf_sup_qua = true
                }
            }
        });

        if (error_send_to_pf_sup_qua) {
            $('#pg_submit_modal .cls_chk_send_supervisor').attr("disabled", true);
            $('#pg_submit_modal .cls_lbl_submit_to_supervisor').text('Send to supervisor (Please input all required field in PG tab.)');
            $('#pg_submit_modal .cls_lbl_submit_to_supervisor').css('color', 'red');
        }
        else {
            $('#pg_submit_modal .cls_chk_send_supervisor').attr("disabled", false);
            $('#pg_submit_modal .cls_lbl_submit_to_supervisor').text('Send to supervisor');
            $('#pg_submit_modal .cls_lbl_submit_to_supervisor').css('color', 'black');
        }
        saveDataPG(true, false);
        $(pg_submit_modal).modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $(pg_submit_modal + "form").submit(function (e) {

        //---------by aof 20220118 for CR sustain-- start
        if (checkFieldRequiredSustain()) {
            return false;
        }
         //---------by aof 20220118 for CR sustain-- end

        //$(pg_submit_modal + ".cls_btn_submit").click(function (e) {
        if ($(this).valid()) {
            saveDataPG(false, true);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_task_form_pg .cls_btn_send_to_customer").click(function (e) {
        if (REQUEST_PHYSICAL_MOCKUP == "1") {
            $(".cls_pg_submit_modal_to_customer input[name=send_to_customer_approve][value=0]").prop('checked', true);
        }
        else if (REQUEST_PHYSICAL_MOCKUP == "0") {
            $(".cls_pg_submit_modal_to_customer input[name=send_to_customer_approve][value=1]").prop('checked', true);
        }

        $('#pg_submit_modal_to_customer').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    //Vendor
    var pg_submit_modal_vendor = "#pg_submit_modal_vendor ";
    var text_editor_vn_send_dl = bind_text_editor(pg_submit_modal_vendor + '.cls_txt_send_dl');
    var text_editor_vn_send_mb = bind_text_editor(pg_submit_modal_vendor + '.cls_txt_send_mb');
    var text_editor_vn_send_pr = bind_text_editor(pg_submit_modal_vendor + '.cls_txt_send_pr');
    var text_editor_vn_send_rs = bind_text_editor(pg_submit_modal_vendor + '.cls_txt_send_rs');

    $(pg_submit_modal_vendor + '.cls_chk_send_dl').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal_vendor + '.cls_body_send_dl').show();
        }
        else {
            $(pg_submit_modal_vendor + '.cls_body_send_dl').hide();
        }
    });
    $(pg_submit_modal_vendor + '.cls_chk_send_mb').click(function () {
        if ($(this).prop('checked')) {
            $(pg_submit_modal_vendor + '.cls_body_send_mb').show();
        }
        else {
            $(pg_submit_modal_vendor + '.cls_body_send_mb').hide();
        }
    });

    $(".cls_task_form_pg .cls_btn_send_to_vendor").click(function () {
        $(pg_submit_modal_vendor).modal({
            backdrop: 'static',
            keyboard: true
        });
    });


    $(".cls_task_form_pg .cls_btn_complete_workflow").click(function (e) {

        //---------by aof 20220118 for CR sustain-- start
        if (checkFieldRequiredSustain()) {
            return false;
        }
         //---------by aof 20220118 for CR sustain-- end

        var jsonObj = new Object();
        jsonObj.data = {};
        jsonObj.data["MOCKUP_ID"] = MOCKUPID;
        jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        jsonObj.data["CREATE_BY"] = UserID;
        jsonObj.data["UPDATE_BY"] = UserID;

        var myurl = '/api/taskform/mockupprocess/pgcompletewf';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
    });

    $(".cls_task_form_pg .cls_btn_send_to_mk").click(function (e) {
        $('#popup_send_to_checklist_creator').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $(".cls_task_form_pg .cls_btn_terminate_workflow").click(function (e) {
        $('#popup_terminate_workflow_mockup').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $("#popup_send_to_checklist_creator .cls_btn_send_to_mk_submit").click(function (e) {
        var jsonObj = new Object();
        jsonObj.data = {};
        jsonObj.data["MOCKUP_ID"] = MOCKUPID;
        jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        jsonObj.data["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
        jsonObj.data["CURRENT_STEP_ID"] = getstepmockup('SEND_BACK_MK').curr_step;
        jsonObj.data["CREATE_BY"] = UserID;
        jsonObj.data["UPDATE_BY"] = UserID;
        var editor = new Quill('#popup_send_to_checklist_creator .cls_txtedt_send_back_mk');
        jsonObj.data["REMARK"] = editor.root.innerHTML;

        if (jsonObj.data["REMARK"] == "<p><br></p>" || jsonObj.data["REMARK"] == null) {
            alertError2("Please fill comment");
            return false;
        }

        var myurl = '/api/taskform/mockupprocess/pgsendbackmk';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxConfirmSubmit(myurl, mytype, mydata, callback_submit_sendback_to_mk);
    });

    $("#popup_terminate_workflow_mockup .cls_btn_submit_terminate_workflow").click(function (e) {
        var jsonObj = new Object();
        jsonObj.data = {};
        jsonObj.data["MOCKUP_ID"] = MOCKUPID;
        jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        jsonObj.data["TERMINATE_REASON_CODE"] = $(".cls_lov_mockup_reason_terminate").val();
        jsonObj.data["CREATE_BY"] = UserID;
        jsonObj.data["UPDATE_BY"] = UserID;
        var editor = new Quill('#popup_terminate_workflow_mockup .cls_txtedt_terminate_workflow');
        jsonObj.data["REMARK_TERMINATE"] = editor.root.innerHTML;

        if (jsonObj.data["TERMINATE_REASON_CODE"] != DefaultResonId) {
            if ($(".cls_lov_mockup_reason_terminate").select2('data')[0].text.toLowerCase() == "อื่นๆ โปรดระบุ (Others)".toLowerCase() && (jsonObj.data["REMARK_TERMINATE"] == "<p><br></p>" || jsonObj.data["REMARK_TERMINATE"] == null)) {
                alertError2("Please fill comment");
                return false;
            }
        }
        else {
            alertError2("Please select reason for terminate");
            return false;
        }

        var myurl = '/api/taskform/mockupprocess/terminatewfmockup';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
    });

    $(".cls_task_form_pg .cls_btn_search_for_die_line").click(function (e) {

        first_load = true; // by aof #INC-4849
        $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_primary_type').text($('.cls_task_form_pg .cls_txt_pg_primary_type').val());
        $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_primary_size').text($('.cls_task_form_pg .cls_txt_pg_primary_size').val());
        $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_pack_size').text($('.cls_task_form_pg .cls_txt_pg_pack_size').val());

        if ($('.cls_check_list_form_task_form .cls_lov_packing_style_other option:selected').val() > 0) {
            $('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val($('.cls_check_list_form_task_form .cls_lov_packing_style_other option:selected').text().split(':')[0]);
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_packing_style').text($('.cls_check_list_form_task_form .cls_lov_packing_style_other option:selected').text().split(':')[0]);
        }
        else {
            $('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val($('.cls_check_list_form_task_form .cls_input_packing_style_other').val());
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_packing_style').text($('.cls_check_list_form_task_form .cls_input_packing_style_other').val());
        }

        $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_packaging_type').text($('.cls_task_form_pg .cls_lov_pg_packaging_type option:selected').text());
        $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_total_colour').text($('.cls_tfartwork_pa .cls_lov_pg_pim_color_static option:selected').text());
        $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_style_of_printing').text($('.cls_tfartwork_pa .cls_lov_pa_style_of_printing option:selected').text());

        $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size').val($('.cls_task_form_pg .cls_txt_pg_primary_size').val());

        binddatapopupSearchDieline();
        setparamSearchDieline();
        bindSearchForDieline();
        first_load = false; // by aof #INC-4849
        $('#modal_taskform_pg_search_for_dieline').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $(".cls_container_taskform .cls_btn_taskform_pg_dieline_select").click(function (e) {
        var table = $('#table_taskform_pg_search_for_dieline').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {

            var v = res_pg_request.data[0];
            //if (tblData[0]['PRIMARY_TYPE_ID'] !== v.CHECKLIST.PRIMARY_TYPE_ID) {
            //    alertError('Primary type : ' + tblData[0]['PRIMARY_TYPE_DISPLAY_TXT'].toString() + ' ไม่สามารถเลือกข้อมูลนี้ได้');
            //    return;
            //}
            if (tblData[0]['PACKING_TYPE_ID'] != v.CHECKLIST_PG.PACKING_TYPE_ID) {
                //alertError('Packaging type : ' + tblData[0]['PACKAGING_TYPE_DISPLAY_TXT'].toString() + ' ไม่สามารถเลือกข้อมูลนี้ได้');
                alertError2('Packaging type does not match');
                return;
            }
        }
        else {
            alertError2("Please select at least 1 item.");
        }

        var table = $('#table_taskform_pg_search_for_dieline').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            bindDataPG_Dieline(tblData[0]);
            $("#modal_taskform_pg_search_for_dieline .cls_btn_taskform_pg_dieline_close").click();
            saveDataPG(true, true);
        }
    });

    $(".cls_container_taskform .cls_btn_taskform_pg_dieline_clear").click(function () {
        setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other', '', '');
        setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search', '', '');
        //setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size', '', '');
        setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_pack_size', '', '');
        //setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style', '', '');
        $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size').val('');
        $('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val('');
    });

    $(".cls_container_taskform .cls_btn_taskform_pg_dieline_search").click(function () {
        bindSearchForDieline();
    });

    $('.cls_view_pg_mockup #modal_taskform_pg_search_for_dieline .cls_lov_primary_size').on('keypress', function (e) {
        if (e.which === 13) {
            bindSearchForDielineArtwork();
        }
    });
    $('.cls_view_pg_mockup #modal_taskform_pg_search_for_dieline .cls_lov_packing_style').on('keypress', function (e) {
        if (e.which === 13) {
            bindSearchForDielineArtwork();
        }
    });
    //$(".cls_container_taskform .cls_lov_primary_type_other").change(function (e) {
    //    if ($('.cls_container_taskform .cls_lov_primary_type_other option:selected').select2()[0] == null) {
    //        return
    //    }
    //    primaryTypeId = e.currentTarget.value;
    //});
    //$('.cls_container_taskform .cls_lov_primary_size').change(function (e) {
    //    if ($('.cls_container_taskform .cls_lov_primary_size option:selected').select2()[0] == null) {
    //        return
    //    }
    //    primarySizeId = e.currentTarget.value;
    //});
    //$('.cls_container_taskform .cls_lov_pack_size').change(function (e) {
    //    if ($('.cls_container_taskform .cls_lov_pack_size option:selected').select2()[0] == null) {
    //        return
    //    }
    //    packSizeId = e.currentTarget.value;
    //});
    //$('.cls_container_taskform .cls_lov_packing_style').change(function (e) {
    //    if ($('.cls_container_taskform .cls_lov_packing_style option:selected').select2()[0] == null) {
    //        return
    //    }
    //    packagingStyleId = e.currentTarget.value;
    //});
    //$('.cls_container_taskform .cls_lov_pg_packaging_type_search').change(function (e) {
    //    if ($('.cls_container_taskform .cls_lov_pg_packaging_type_search option:selected').select2()[0] == null) {
    //        return
    //    }
    //    packagingTypeId = e.currentTarget.value;
    //});


    $(".cls_lov_pkg_sec_material").trigger("change");     //---------by aof 20220118 for CR sustain-- - start
});

function callback_submit_sendback_to_mk(res) {
    resetDllReason('#popup_send_to_checklist_creator .cls_lov_send_for_reason');
    var text_editor_send_back_mk = new Quill('#popup_send_to_checklist_creator ' + '.cls_txtedt_send_back_mk');
    text_editor_send_back_mk.setContents([{ insert: '\n' }]);
    $('#popup_send_to_checklist_creator').modal('hide');
}



function saveDataPG(only_save, is_showmsg) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var form_name = '.cls_task_form_pg ';
    var item = {};





    if ($('.cls_header_chk[type="checkbox"]:checked').length > 0 || only_save == true) {
        if (MOCKUPSUBPGID != 0) {
            item["MOCKUP_SUB_PG_ID"] = MOCKUPSUBPGID;
        }
        item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        //item["REMARK"] = $(form_name + '.cls_txt_remark').val();

        var itemCheckList_PG = {};
        itemCheckList_PG["CHECK_LIST_PG_ID"] = $(form_name + '.cls_hid_check_list_pg_id').val();
        itemCheckList_PG["CHECK_LIST_ID"] = CheckListID;
        itemCheckList_PG["MOCKUP_ID"] = MOCKUPID;
        itemCheckList_PG["PACKING_TYPE_ID"] = $(form_name + '.cls_lov_pg_packaging_type_static').val();
        itemCheckList_PG["PRINT_SYSTEM_ID"] = $(form_name + '.cls_lov_pg_pim_static').val();
        itemCheckList_PG["PRINT_SYSTEM_OTHER"] = $(form_name + '.cls_input_pg_pim_other_static').val();
        itemCheckList_PG["NUMBER_OF_COLOR_ID"] = $(form_name + '.cls_lov_pg_pim_color_static').val();
        itemCheckList_PG["NUMBER_OF_COLOR_OTHER"] = $(form_name + '.cls_input_pg_pim_color_other_static').val();
        itemCheckList_PG["BOX_COLOR_ID"] = $(form_name + '.cls_lov_pg_box_color_static').val();
        itemCheckList_PG["BOX_COLOR_OTHER"] = $(form_name + '.cls_input_pg_box_color_other_static').val();
        itemCheckList_PG["COATING_ID"] = $(form_name + '.cls_lov_pg_coating_static').val();
        itemCheckList_PG["COATING_OTHER"] = $(form_name + '.cls_input_pg_coating_other_static').val();
        itemCheckList_PG["PURPOSE_OF"] = $(form_name + '.cls_txt_pg_purpose_of').val();
        itemCheckList_PG["STYLE_ID"] = $(form_name + '.cls_lov_pg_style_static').val();
        itemCheckList_PG["STYLE_OTHER"] = $(form_name + '.cls_input_pg_style_other_static').val();
        itemCheckList_PG["REMARK"] = $(form_name + '.cls_txt_pg_remark').val();
        item["CHECKLIST_PG"] = itemCheckList_PG;

        //var itemCheckList = {};
        //itemCheckList["THREE_P_ID"] = $(form_name + '.cls_lov_pg_primary_size').val();
        //itemCheckList["PACK_SIZE_ID"] = $(form_name + '.cls_lov_pg_pack_size').val();
        //itemCheckList["UPDATE_BY"] = UserID;
        //item["CHECKLIST"] = itemCheckList;

        item["DIE_LINE_MOCKUP_ID"] = $(form_name + '.cls_hid_pg_dieline_mockup_id').val();
        if (!isEmpty($('.cls_header_mockup_no').val()) && $('.cls_header_mockup_no').val().indexOf('MO-D-') >= 0)
        {
 
        }
        else {
            item["GRADE_OF"] = $(form_name + '.cls_lov_grade_of').val();
            item["GRADE_OF_OTHER"] = $(form_name + '.cls_input_grade_of').val();
            item["DI_CUT"] = $(form_name + '.cls_lov_rsc_di_cut').val();
            item["DI_CUT_OTHER"] = $(form_name + '.cls_input_rsc_di_cut').val();
            item["SHEET_SIZE"] = $(form_name + '.cls_txt_sheet_size').val();
            item["ACCESSORIES"] = $(form_name + '.cls_txt_acessories').val();
            item["VENDOR"] = $(form_name + '.cls_lov_vendor').val();
            item["VENDOR_OTHER"] = $(form_name + '.cls_input_vendor').val();
            item["PRINT_SYSTEM"] = $(form_name + '.cls_txt_printing_system').val();
            item["FLUTE"] = $(form_name + '.cls_lov_flute').val();
            item["FLUTE_OTHER"] = $(form_name + '.cls_input_flute').val();
            item["ROLL_SHEET"] = $(form_name + '.cls_lov_roll_sheet').val();
            item["ROLL_SHEET_OTHER"] = $(form_name + '.cls_input_roll_sheet').val();
            item["DIMENSION_OF"] = $(form_name + '.cls_txt_dimension_of').val();
            item["FINAL_INFO"] = $(form_name + '.cls_txt_final_infogroup').val();
            item["REMARK_PG"] = $(form_name + '.cls_txt_remark_note_of_pg').val();
            item["ID_MM"] = $(form_name + '.cls_txt_id_mm').val();
            //item["CUSTOMER_DESIGN"] = $('input[name=pg_rdo_customer_design]:radio:checked').val();
            //item["CUSTOMER_DESIGN_REMARK"] = $(form_name + '.cls_txt_customer_design').val();
            item["CUSTOMER_SPEC"] = $('input[name=pg_rdo_customer_spec]:radio:checked').val();
            item["CUSTOMER_SPEC_REMARK"] = $(form_name + '.cls_txt_customer_spec').val();
            item["CUSTOMER_SIZE"] = $('input[name=pg_rdo_customer_size]:radio:checked').val();
            item["CUSTOMER_SIZE_REMARK"] = $(form_name + '.cls_txt_customer_size').val();
            item["CUSTOMER_NOMINATES_VENDOR"] = $('input[name=pg_rdo_customer_nominates_vendor]:radio:checked').val();
            item["CUSTOMER_NOMINATES_VENDOR_REMARK"] = $(form_name + '.cls_txt_nominates_vendor').val();

            //---------by aof 20220118 for CR sustain-- - start
            item["SUSTAIN_MATERIAL"] = $(form_name + '.cls_lov_pkg_sec_material').val();
            item["PLASTIC_TYPE"] = $(form_name + '.cls_lov_pkg_sec_plastic').val();
            item["REUSEABLE"] = $('input[name=pg_rdo_pkg_sec_reusable]:radio:checked').val();
            item["RECYCLABLE"] = $('input[name=pg_rdo_pkg_sec_recyclable]:radio:checked').val();
            item["COMPOSATABLE"] = $('input[name=pg_rdo_pkg_sec_composatable]:radio:checked').val();
            item["RECYCLE_CONTENT"] = $(form_name + '.cls_txt_pkg_sec_recycled_content').val();
            item["CERT"] = $('input[name=pg_rdo_pkg_sec_certification]:radio:checked').val();
            item["CERT_SOURCE"] = $(form_name + '.cls_lov_pkg_sec_certification_standard').val();
            item["PKG_WEIGHT"] = setStringToNumber($(form_name + '.cls_txt_pkg_sec_weight').val());
            item["SUSTAIN_OTHER"] = $(form_name + '.cls_txt_pkg_sec_other').val();

            //---------by aof 20220118 for CR sustain-- - end
        }


        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;

        jsonObj.data = item;

        var myurl = '/api/taskform/pg/info';
        var mytype = 'POST';
        var mydata = jsonObj;
        if (only_save)
            myAjax(myurl, mytype, mydata, callbackSaveDataPG, '', false, is_showmsg);
        else {
            myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSubmitDataPG, '', false, false);
        }
    }
    else {
        $(".se-pre-con").fadeOut('fast');
        alertError2("Please select at least 1 item.");
    }
}

function bindDataPG(mockup_sub_id, artwork_sub_id) {
    var param_artwork_sub_id = '';
    if (artwork_sub_id != undefined) {
        param_artwork_sub_id = '&data.artwork_sub_id=' + artwork_sub_id;
    }
    var myurl = '/api/taskform/pg/info?data.mockup_sub_id=' + mockup_sub_id + param_artwork_sub_id;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_pgrequest);
}

var res_pg_request;
function callback_get_pgrequest(res) {
    res_pg_request = res;
    if (res.data.length > 0) {
        var form_name = '.cls_task_form_pg ';
        var v = res.data[0];
        MOCKUPSUBPGID = v.MOCKUP_SUB_PG_ID;
        $(form_name + '.cls_txt_pa_username').val(v.PA_USER_DISPLAY_TXT);
        $(form_name + '.cls_txt_pg_username').val(v.PG_USER_DISPLAY_TXT);

        var tempREFERENCE_REQUEST_NO = "&nbsp;";
        if (!isEmpty(v.CHECKLIST.REFERENCE_REQUEST_NO)) {
            tempREFERENCE_REQUEST_NO = v.CHECKLIST.REFERENCE_REQUEST_NO;
        }
        $('.cls_header_ref_no').html('<a target="_blank" href="/CheckList/' + v.CHECKLIST.REFERENCE_REQUEST_ID + '">' + tempREFERENCE_REQUEST_NO + '</a>');

        if (v.CHECKLIST.CHECK_LIST_FOR_DESIGN == 1) {
            //if (v.CHECKLIST.CHECK_LIST_FOR_DESIGN == 1 && isEmpty(v.CHECKLIST.SOLD_TO_ID) && isEmpty(v.CHECKLIST.SHIP_TO_ID)) {
            $('.cls_btn_send_to_customer').text('Send to marketing');
            $('.cls_pg_submit_modal_to_customer #myModalLabel').text('Send to marketing');
            $('.cls_li_customer a').text('Marketing');
            isProjectNoCus = "1";
        }
        else if (v.CHECKLIST.REQUEST_FOR_DIE_LINE == 1) {
            $('.cls_task_form_pg .cls_btn_complete_workflow').text('Complete workflow and send dieline');
        }

        var CHECKLIST_NET_WEIGHT = "";
        if (v.CHECKLIST.REFERENCE.length > 0) {
            $.each(v.CHECKLIST.REFERENCE, function (index, item) {
                var found = false;
                var arr = CHECKLIST_NET_WEIGHT.split(',');
                $.each(arr, function (index, value) {
                    if (value == item.NET_WEIGHT) {
                        found = true;
                    }
                });
                if (!found) {
                    if (CHECKLIST_NET_WEIGHT.length > 0) {

                        CHECKLIST_NET_WEIGHT += "," + item.NET_WEIGHT;
                    }
                    else {
                        CHECKLIST_NET_WEIGHT += item.NET_WEIGHT;
                    }
                }
            });
        } else if (v.CHECKLIST.PRODUCT.length > 0) {
            $.each(v.CHECKLIST.PRODUCT, function (index, item) {
                var found = false;
                var arr = CHECKLIST_NET_WEIGHT.split(',');
                $.each(arr, function (index, value) {
                    if (value == item.NET_WEIGHT) {
                        found = true;
                    }
                });
                if (!found) {
                    if (CHECKLIST_NET_WEIGHT.length > 0) {
                        CHECKLIST_NET_WEIGHT += "," + item.NET_WEIGHT;
                    }
                    else {
                        CHECKLIST_NET_WEIGHT += item.NET_WEIGHT;
                    }
                }
            });
        }

        var CHECKLIST_BRAND = "";
        if (v.CHECKLIST.BRAND_ID == -1) {
            CHECKLIST_BRAND = v.CHECKLIST.BRAND_OTHER;
        } else {
            CHECKLIST_BRAND = v.CHECKLIST.BRAND_DISPLAY_TXT;
        }
        var CHECKLIST_PRIMARY_TYPE = "";
        if (v.CHECKLIST.PRIMARY_TYPE_ID == -1) {
            CHECKLIST_PRIMARY_TYPE = v.CHECKLIST.PRIMARY_TYPE_OTHER;
        } else {
            CHECKLIST_PRIMARY_TYPE = v.CHECKLIST.PRIMARY_TYPE_DISPLAY_TXT;
        }

        var CHECKLIST_PG_PACKANGING_TYPE = "";
        var CHECKLIST_PG_PACKANGING_STYLE = "";
        var CHECKLIST_PG_PRINTING_SYSTEM = "";
        var CHECKLIST_PG_PURPOSE_OF = "";
        var CHECKLIST_PG_NUMBER_OF_COLOR = "";
        var CHECKLIST_PG_COATING = "";
        //Tab : PG Data : Item
        if (v.CHECKLIST_PG != null) {
            $(form_name + '.cls_hid_check_list_pg_id').val(v.CHECKLIST_PG.CHECK_LIST_PG_ID);
            setValueToDDL(form_name + '.cls_lov_pg_packaging_type_static', v.CHECKLIST_PG.PACKING_TYPE_ID, v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT);
            setValueToDDL(form_name + '.cls_lov_pg_pim_static', v.CHECKLIST_PG.PRINT_SYSTEM_ID, v.CHECKLIST_PG.PRINT_SYSTEM_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pg_pim_other_static', v.CHECKLIST_PG.PRINT_SYSTEM_OTHER);
            setValueToDDL(form_name + '.cls_lov_pg_pim_color_static', v.CHECKLIST_PG.NUMBER_OF_COLOR_ID, v.CHECKLIST_PG.NUMBER_OF_COLOR_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pg_pim_color_other_static', v.CHECKLIST_PG.NUMBER_OF_COLOR_OTHER);
            setValueToDDL(form_name + '.cls_lov_pg_box_color_static', v.CHECKLIST_PG.BOX_COLOR_ID, v.CHECKLIST_PG.BOX_COLOR_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pg_box_color_other_static', v.CHECKLIST_PG.BOX_COLOR_OTHER);
            setValueToDDL(form_name + '.cls_lov_pg_coating_static', v.CHECKLIST_PG.COATING_ID, v.CHECKLIST_PG.COATING_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pg_coating_other_static', v.CHECKLIST_PG.COATING_OTHER);
            $(form_name + '.cls_txt_pg_purpose_of').val(v.CHECKLIST_PG.PURPOSE_OF);
            setValueToDDL(form_name + '.cls_lov_pg_style_static', v.CHECKLIST_PG.STYLE_ID, v.CHECKLIST_PG.STYLE_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pg_style_other_static', v.CHECKLIST_PG.STYLE_OTHER);
            $(form_name + '.cls_txt_pg_remark').val(v.CHECKLIST_PG.REMARK);

            CHECKLIST_PG_PACKANGING_TYPE = v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT;
            if (v.CHECKLIST_PG.STYLE_ID == -1) {
                CHECKLIST_PG_PACKANGING_STYLE = v.CHECKLIST_PG.STYLE_OTHER;
            } else {
                CHECKLIST_PG_PACKANGING_STYLE = v.CHECKLIST_PG.STYLE_DISPLAY_TXT;
            }
            if (v.CHECKLIST_PG.PRINT_SYSTEM_ID == -1) {
                CHECKLIST_PG_PRINTING_SYSTEM = v.CHECKLIST_PG.PRINT_SYSTEM_OTHER;
            } else {
                CHECKLIST_PG_PRINTING_SYSTEM = v.CHECKLIST_PG.PRINT_SYSTEM_DISPLAY_TXT;
            }
            CHECKLIST_PG_PURPOSE_OF = v.CHECKLIST_PG.PURPOSE_OF;
            if (v.CHECKLIST_PG.NUMBER_OF_COLOR_ID == -1) {
                CHECKLIST_PG_NUMBER_OF_COLOR = v.CHECKLIST_PG.NUMBER_OF_COLOR_OTHER;
            } else {
                CHECKLIST_PG_NUMBER_OF_COLOR = v.CHECKLIST_PG.NUMBER_OF_COLOR_DISPLAY_TXT;
            }
            if (v.CHECKLIST_PG.COATING_ID == -1) {
                CHECKLIST_PG_COATING = v.CHECKLIST_PG.COATING_OTHER;
            } else {
                CHECKLIST_PG_COATING = v.CHECKLIST_PG.COATING_DISPLAY_TXT;
            }
        }

        if (v.DIE_LINE != null) {
            $(form_name + '.cls_hid_pg_dieline_mockup_id').val(v.DIE_LINE_MOCKUP_ID);
            bindDataPG_Dieline(v.DIE_LINE);
        }

        var PG_GRADE_OF = "";
        if (v.GRADE_OF == -1) {
            PG_GRADE_OF = v.GRADE_OF_OTHER;
        } else {
            PG_GRADE_OF = v.GRADE_OF_DISPLAY_TXT;
        }
        var PG_FLUTE = "";
        if (v.FLUTE == -1) {
            PG_FLUTE = v.FLUTE_OTHER;
        } else {
            PG_FLUTE = v.FLUTE_DISPLAY_TXT;
        }

        var CHECKLIST_PRIMARY_TYPE = "";
        if (v.CHECKLIST.PRIMARY_TYPE_ID == -1) {
            CHECKLIST_PRIMARY_TYPE = v.CHECKLIST.PRIMARY_TYPE_OTHER;
        } else {
            CHECKLIST_PRIMARY_TYPE = v.CHECKLIST.PRIMARY_TYPE_DISPLAY_TXT;
        }

        $(form_name + '.cls_txt_pg_brand').val(CHECKLIST_BRAND);
        $(form_name + '.cls_txt_pg_primary_type').val(CHECKLIST_PRIMARY_TYPE);

        var tempPRIMARY_SIZE_DISPLAY_TXT = '';
        if (!isEmpty(v.CHECKLIST.PRIMARY_SIZE_DISPLAY_TXT))
            tempPRIMARY_SIZE_DISPLAY_TXT = v.CHECKLIST.PRIMARY_SIZE_DISPLAY_TXT;
        else if (!isEmpty(v.CHECKLIST.PRIMARY_SIZE_OTHER))
            tempPRIMARY_SIZE_DISPLAY_TXT = v.CHECKLIST.PRIMARY_SIZE_OTHER;
        else if (v.CHECKLIST.PRODUCT.length > 0)
            tempPRIMARY_SIZE_DISPLAY_TXT = v.CHECKLIST.PRODUCT[0].PRIMARY_SIZE;

        var tempPACK_SIZE_DISPLAY_TXT = '';
        if (!isEmpty(v.CHECKLIST.PACK_SIZE_DISPLAY_TXT))
            tempPACK_SIZE_DISPLAY_TXT = v.CHECKLIST.PACK_SIZE_DISPLAY_TXT;
        if (!isEmpty(v.CHECKLIST.PACK_SIZE_OTHER))
            tempPACK_SIZE_DISPLAY_TXT = v.CHECKLIST.PACK_SIZE_OTHER;
        else if (v.CHECKLIST.PRODUCT.length > 0)
            tempPACK_SIZE_DISPLAY_TXT = v.CHECKLIST.PRODUCT[0].PACK_SIZE;

        $(form_name + '.cls_txt_pg_primary_size').val(tempPRIMARY_SIZE_DISPLAY_TXT);
        $(form_name + '.cls_txt_pg_pack_size').val(tempPACK_SIZE_DISPLAY_TXT);
        $(form_name + '.cls_txt_pg_style_of_printing').val(v.STYLE_OF_PRINTING_DISPLAY_TXT);

        if (!isEmpty($('.cls_header_mockup_no').val()) && $('.cls_header_mockup_no').val().indexOf('MO-D-') >= 0 && v.DIE_LINE != null) {
            setValueToDDL(form_name + '.cls_lov_flute', v.DIE_LINE.FLUTE, v.DIE_LINE.FLUTE_DISPLAY_TXT);
            if (v.DIE_LINE.FLUTE == -1) setValueToDDLOther(form_name + '.cls_input_flute', v.DIE_LINE.FLUTE_DISPLAY_TXT);

            setValueToDDL(form_name + '.cls_lov_rsc_di_cut', v.DIE_LINE.DI_CUT, v.DIE_LINE.DI_CUT_DISPLAY_TXT);
            if (v.DIE_LINE.DI_CUT == -1) setValueToDDLOther(form_name + '.cls_input_rsc_di_cut', v.DIE_LINE.DI_CUT_DISPLAY_TXT);

            setValueToDDL(form_name + '.cls_lov_grade_of', v.DIE_LINE.GRADE_OF, v.DIE_LINE.GRADE_OF_DISPLAY_TXT);
            if (v.DIE_LINE.GRADE_OF == -1) setValueToDDLOther(form_name + '.cls_input_grade_of', v.DIE_LINE.GRADE_OF_DISPLAY_TXT);

            $(form_name + '.cls_txt_printing_system').val(v.DIE_LINE.PRINT_SYSTEM);
            $(form_name + '.cls_txt_sheet_size').val(v.DIE_LINE.SHEET_SIZE);
            $(form_name + '.cls_txt_acessories').val(v.DIE_LINE.ACCESSORIES);
            $(form_name + '.cls_txt_dimension_of').val(v.DIE_LINE.DIMENSION_OF);
            $(form_name + '.cls_txt_final_infogroup').val(v.DIE_LINE.FINAL_INFO);
            $(form_name + '.cls_txt_remark_note_of_pg').val(v.DIE_LINE.REMARK_PG);
            $(form_name + '.cls_txt_id_mm').val(v.DIE_LINE.ID_MM);

            $(form_name + '.cls_lov_roll_sheet').val(v.DIE_LINE.ROLL_SHEET).trigger("change");

            if (!isEmpty(v.DIE_LINE.CUSTOMER_SPEC)) {
                if (v.DIE_LINE.CUSTOMER_SPEC == "1") {
                    $("input[name=pg_rdo_customer_spec][value=1]").prop('checked', true);
                }
                else if (v.DIE_LINE.CUSTOMER_SPEC == "0") {
                    $("input[name=pg_rdo_customer_spec][value=0]").prop('checked', true);
                }
            }
            $(form_name + '.cls_txt_customer_spec').val(v.DIE_LINE.CUSTOMER_SPEC_REMARK);
            if (!isEmpty(v.DIE_LINE.CUSTOMER_SIZE)) {
                if (v.DIE_LINE.CUSTOMER_SIZE == "1") {
                    $("input[name=pg_rdo_customer_size][value=1]").prop('checked', true);
                }
                else if (v.DIE_LINE.CUSTOMER_SIZE == "0") {
                    $("input[name=pg_rdo_customer_size][value=0]").prop('checked', true);
                }
            }
            $(form_name + '.cls_txt_customer_size').val(v.DIE_LINE.CUSTOMER_SIZE_REMARK);
            if (!isEmpty(v.DIE_LINE.CUSTOMER_NOMINATES_VENDOR)) {
                if (v.DIE_LINE.CUSTOMER_NOMINATES_VENDOR == "1") {
                    $("input[name=pg_rdo_customer_nominates_vendor][value=1]").prop('checked', true);
                }
                else if (v.DIE_LINE.CUSTOMER_NOMINATES_VENDOR == "0") {
                    $("input[name=pg_rdo_customer_nominates_vendor][value=0]").prop('checked', true);
                }
            }
            $(form_name + '.cls_txt_nominates_vendor').val(v.DIE_LINE.CUSTOMER_NOMINATES_VENDOR_REMARK);

            //---------by aof 20220118 for CR sustain-- - start
            setValueToDDL(form_name + '.cls_lov_pkg_sec_material', v.DIE_LINE.SUSTAIN_MATERIAL, v.DIE_LINE.SUSTAIN_MATERIAL_DISPLAY_TXT);
            setValueToDDL(form_name + '.cls_lov_pkg_sec_plastic', v.DIE_LINE.PLASTIC_TYPE, v.DIE_LINE.PLASTIC_TYPE_DISPLAY_TXT);
            setValueToDDL(form_name + '.cls_lov_pkg_sec_certification_standard', v.DIE_LINE.CERT_SOURCE, v.DIE_LINE.CERT_SOURCE_DISPLAY_TXT);
            if (v.DIE_LINE.REUSEABLE == "1") {
                $("input[name=pg_rdo_pkg_sec_reusable][value=1]").prop('checked', true);
            }
            else if (v.DIE_LINE.REUSEABLE == "0") {
                $("input[name=pg_rdo_pkg_sec_reusable][value=0]").prop('checked', true);
            }

            if (v.DIE_LINE.RECYCLABLE == "1") {
                $("input[name=pg_rdo_pkg_sec_recyclable][value=1]").prop('checked', true);
            }
            else if (v.DIE_LINE.RECYCLABLE == "0") {
                $("input[name=pg_rdo_pkg_sec_recyclable][value=0]").prop('checked', true);
            }

            if (v.DIE_LINE.COMPOSATABLE == "1") {
                $("input[name=pg_rdo_pkg_sec_composatable][value=1]").prop('checked', true);
            }
            else if (v.DIE_LINE.COMPOSATABLE == "0") {
                $("input[name=pg_rdo_pkg_sec_composatable][value=0]").prop('checked', true);
            }

            if (v.DIE_LINE.CERT == "1") {
                $("input[name=pg_rdo_pkg_sec_certification][value=1]").prop('checked', true);
            }
            else if (v.DIE_LINE.CERT == "0") {
                $("input[name=pg_rdo_pkg_sec_certification][value=0]").prop('checked', true);
            }
            $(form_name + '.cls_txt_pkg_sec_recycled_content').val(v.DIE_LINE.RECYCLE_CONTENT);
            $(form_name + '.cls_txt_pkg_sec_weight').val(setNumberToStringFormat(v.DIE_LINE.PKG_WEIGHT, 3));
            $(form_name + '.cls_txt_pkg_sec_other').val(v.DIE_LINE.SUSTAIN_OTHER);

            //---------by aof 20220118 for CR sustain-- - end


        }
        else {

            setValueToDDL(form_name + '.cls_lov_grade_of', v.GRADE_OF, v.GRADE_OF_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_grade_of', v.GRADE_OF_OTHER);
            setValueToDDL(form_name + '.cls_lov_rsc_di_cut', v.DI_CUT, v.DI_CUT_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_rsc_di_cut', v.DI_CUT_OTHER);
            $(form_name + '.cls_txt_sheet_size').val(v.SHEET_SIZE);
            $(form_name + '.cls_txt_acessories').val(v.ACCESSORIES);
            if (!isEmpty(v.VENDOR_DISPLAY_TXT)) setValueToDDL(form_name + '.cls_lov_vendor', v.VENDOR, v.VENDOR_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_vendor', v.VENDOR_OTHER);
            $(form_name + '.cls_txt_printing_system').val(v.PRINT_SYSTEM);
            setValueToDDL(form_name + '.cls_lov_flute', v.FLUTE, v.FLUTE_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_flute', v.FLUTE_OTHER);
            $(form_name + '.cls_lov_roll_sheet').val(v.ROLL_SHEET).trigger("change");
            setValueToDDLOther(form_name + '.cls_input_roll_sheet', v.ROLL_SHEET_OTHER);
            $(form_name + '.cls_txt_dimension_of').val(v.DIMENSION_OF);
            $(form_name + '.cls_txt_final_infogroup').val(v.FINAL_INFO);
            $(form_name + '.cls_txt_remark_note_of_pg').val(v.REMARK_PG);
            $(form_name + '.cls_txt_id_mm').val(v.ID_MM);
            if (!isEmpty(v.CUSTOMER_SPEC)) {
                if (v.CUSTOMER_SPEC == "1") {
                    $("input[name=pg_rdo_customer_spec][value=1]").prop('checked', true);
                }
                else if (v.CUSTOMER_SPEC == "0") {
                    $("input[name=pg_rdo_customer_spec][value=0]").prop('checked', true);
                }
            }
            $(form_name + '.cls_txt_customer_spec').val(v.CUSTOMER_SPEC_REMARK);
            if (!isEmpty(v.CUSTOMER_SIZE)) {
                if (v.CUSTOMER_SIZE == "1") {
                    $("input[name=pg_rdo_customer_size][value=1]").prop('checked', true);
                }
                else if (res.data[0].CUSTOMER_SIZE == "0") {
                    $("input[name=pg_rdo_customer_size][value=0]").prop('checked', true);
                }
            }
            $(form_name + '.cls_txt_customer_size').val(v.CUSTOMER_SIZE_REMARK);
            if (!isEmpty(v.CUSTOMER_NOMINATES_VENDOR)) {
                if (v.CUSTOMER_NOMINATES_VENDOR == "1") {
                    $("input[name=pg_rdo_customer_nominates_vendor][value=1]").prop('checked', true);
                }
                else if (v.CUSTOMER_NOMINATES_VENDOR == "0") {
                    $("input[name=pg_rdo_customer_nominates_vendor][value=0]").prop('checked', true);
                }
            }
            $(form_name + '.cls_txt_nominates_vendor').val(v.CUSTOMER_NOMINATES_VENDOR_REMARK);

                 //---------by aof 20220118 for CR sustain-- - start
            setValueToDDL(form_name + '.cls_lov_pkg_sec_material', v.SUSTAIN_MATERIAL, v.SUSTAIN_MATERIAL_DISPLAY_TXT);
            setValueToDDL(form_name + '.cls_lov_pkg_sec_plastic', v.PLASTIC_TYPE, v.PLASTIC_TYPE_DISPLAY_TXT);
            setValueToDDL(form_name + '.cls_lov_pkg_sec_certification_standard', v.CERT_SOURCE, v.CERT_SOURCE_DISPLAY_TXT);

            

            if (v.REUSEABLE == "1") {
                $("input[name=pg_rdo_pkg_sec_reusable][value=1]").prop('checked', true);
            }
            else if (v.REUSEABLE == "0") {
                $("input[name=pg_rdo_pkg_sec_reusable][value=0]").prop('checked', true);
            }

            if (v.RECYCLABLE == "1") {
                $("input[name=pg_rdo_pkg_sec_recyclable][value=1]").prop('checked', true);
            }
            else if (v.RECYCLABLE == "0") {
                $("input[name=pg_rdo_pkg_sec_recyclable][value=0]").prop('checked', true);
            }

            if (v.COMPOSATABLE == "1") {
                $("input[name=pg_rdo_pkg_sec_composatable][value=1]").prop('checked', true);
            }
            else if (v.COMPOSATABLE == "0") {
                $("input[name=pg_rdo_pkg_sec_composatable][value=0]").prop('checked', true);
            }

            if (v.CERT == "1") {
                $("input[name=pg_rdo_pkg_sec_certification][value=1]").prop('checked', true);
            }
            else if (v.CERT == "0") {
                $("input[name=pg_rdo_pkg_sec_certification][value=0]").prop('checked', true);
            }
            $(form_name + '.cls_txt_pkg_sec_recycled_content').val(v.RECYCLE_CONTENT);
            $(form_name + '.cls_txt_pkg_sec_weight').val(setNumberToStringFormat(v.PKG_WEIGHT,3));
            $(form_name + '.cls_txt_pkg_sec_other').val(v.SUSTAIN_OTHER);

            //---------by aof 20220118 for CR sustain-- - end

        }

        //Tab : PG Data : Submit
        $('.cls_body_send_warehouse .cls_wh_customer').val(v.CHECKLIST.SOLD_TO_DISPLAY_TXT);
        if (isEmpty(v.CHECKLIST.SOLD_TO_DISPLAY_TXT))
            $('.cls_body_send_warehouse .cls_wh_customer').val(v.CHECKLIST.SHIP_TO_DISPLAY_TXT);
        if (isEmpty(v.CHECKLIST.SHIP_TO_DISPLAY_TXT))
            $('.cls_body_send_warehouse .cls_wh_customer').val(v.CHECKLIST.CUSTOMER_OTHER_DISPLAY_TXT);

        $('.cls_body_send_warehouse .cls_wh_brand').val(CHECKLIST_BRAND);
        $('.cls_body_send_warehouse .cls_wh_primary_type').val(CHECKLIST_PRIMARY_TYPE);
        $('.cls_body_send_warehouse .cls_wh_primary_size').val(tempPRIMARY_SIZE_DISPLAY_TXT);
        $('.cls_body_send_warehouse .cls_wh_pack_size').val(tempPACK_SIZE_DISPLAY_TXT);
        $('.cls_body_send_warehouse .cls_wh_packaging_type').val(CHECKLIST_PG_PACKANGING_TYPE);

        var PACKING_STYLE_DISPLAY_TXT = '';
        if (!isEmpty(v.CHECKLIST.PACKING_STYLE_DISPLAY_TXT))
            PACKING_STYLE_DISPLAY_TXT = v.CHECKLIST.PACKING_STYLE_DISPLAY_TXT;
        if (!isEmpty(v.CHECKLIST.PACKING_STYLE_OTHER))
            PACKING_STYLE_DISPLAY_TXT = v.CHECKLIST.PACKING_STYLE_OTHER;
        else if (v.CHECKLIST.PRODUCT.length > 0)
            PACKING_STYLE_DISPLAY_TXT = v.CHECKLIST.PRODUCT[0].PACKING_STYLE;
        $('.cls_body_send_warehouse .cls_wh_packaging_style').val(PACKING_STYLE_DISPLAY_TXT);
        $('.cls_body_send_warehouse .cls_wh_net_weight').val(CHECKLIST_NET_WEIGHT);
        $('.cls_body_send_warehouse .cls_wh_grade').val(PG_GRADE_OF);
        $('.cls_body_send_warehouse .cls_wh_dimension_of').val(v.DIMENSION_OF);

        //Tab : PG Data : Send to vendor[button]
        $('.cls_vn_packaging_type').val(CHECKLIST_PG_PACKANGING_TYPE);
        $('.cls_vn_brand').val(CHECKLIST_BRAND);
        $('.cls_vn_primary_type').val(CHECKLIST_PRIMARY_TYPE);
        $('.cls_vn_primary_size').val(tempPRIMARY_SIZE_DISPLAY_TXT);
        $('.cls_vn_pack_size').val(tempPACK_SIZE_DISPLAY_TXT);
        $('.cls_vn_spec').val(PG_GRADE_OF);
        $('.cls_vn_acc').val(v.ACCESSORIES);
        $('.cls_vn_dimension_of').val(v.DIMENSION_OF);
        $('.cls_vn_flute').val(PG_FLUTE);

        //Tab : PG Data : Send to customer[button]
        var country = '';
        $.each(v.CHECKLIST.COUNTRY, function (index, item) {
            if (country == '')
                country = item.COUNTRY_DISPLAY_TXT;
            else
                country += ", " + item.COUNTRY_DISPLAY_TXT;
        });

        $('.cls_pg_submit_modal_to_customer .cls_c_customer').val(v.CHECKLIST.SOLD_TO_DISPLAY_TXT);
        if (isEmpty(v.CHECKLIST.SOLD_TO_DISPLAY_TXT))
            $('.cls_pg_submit_modal_to_customer .cls_c_customer').val(v.CHECKLIST.SHIP_TO_DISPLAY_TXT);
        if (isEmpty(v.CHECKLIST.SHIP_TO_DISPLAY_TXT))
            $('.cls_pg_submit_modal_to_customer .cls_c_customer').val(v.CHECKLIST.CUSTOMER_OTHER_DISPLAY_TXT);

        //$('.cls_pg_submit_modal_to_customer .cls_c_customer').val(v.CHECKLIST.SOLD_TO_DISPLAY_TXT);
        $('.cls_pg_submit_modal_to_customer .cls_c_brand').val(CHECKLIST_BRAND);
        $('.cls_pg_submit_modal_to_customer .cls_c_country').val(country);
        $('.cls_pg_submit_modal_to_customer .cls_c_project_name').val(v.CHECKLIST.PROJECT_NAME);
        $('.cls_pg_submit_modal_to_customer .cls_c_mkt_name').val(v.CHECKLIST.CREATOR_NAME);
        $('.cls_pg_submit_modal_to_customer .cls_c_primary_type').val(CHECKLIST_PRIMARY_TYPE);
        $('.cls_pg_submit_modal_to_customer .cls_c_primary_size').val(tempPRIMARY_SIZE_DISPLAY_TXT);
        $('.cls_pg_submit_modal_to_customer .cls_c_net_weight').val(CHECKLIST_NET_WEIGHT);
        $('.cls_pg_submit_modal_to_customer .cls_c_pack_size').val(tempPACK_SIZE_DISPLAY_TXT);
        $('.cls_pg_submit_modal_to_customer .cls_c_packaging_type').val(CHECKLIST_PG_PACKANGING_TYPE);
        $('.cls_pg_submit_modal_to_customer .cls_c_packing_style').val(v.CHECKLIST.PACKING_STYLE_DISPLAY_TXT);
        $('.cls_pg_submit_modal_to_customer .cls_c_packaging_style').val(CHECKLIST_PG_PACKANGING_STYLE);
        $('.cls_pg_submit_modal_to_customer .cls_c_printing_system').val(CHECKLIST_PG_PRINTING_SYSTEM);
        $('.cls_pg_submit_modal_to_customer .cls_c_purpose_of').val(CHECKLIST_PG_PURPOSE_OF);
        $('.cls_pg_submit_modal_to_customer .cls_txt_display_to_customer').val(v.CHECKLIST.TO_DISPLAY_TXT);
        $('.cls_pg_submit_modal_to_customer .cls_txt_display_cc_customer').val(v.CHECKLIST.CC_DISPLAY_TXT);

        //Tab : Quotations
        $('.cls_pg_price_template .cls_qua_packaging_type').val(CHECKLIST_PG_PACKANGING_TYPE);
        $('.cls_pg_price_template .cls_qua_printing_system').val(CHECKLIST_PG_PRINTING_SYSTEM);
        $('.cls_pg_price_template .cls_qua_brand').val(CHECKLIST_BRAND);
        $('.cls_pg_price_template .cls_qua_primary_type').val(CHECKLIST_PRIMARY_TYPE);
        $('.cls_pg_price_template .cls_qua_primary_size').val(tempPRIMARY_SIZE_DISPLAY_TXT);
        $('.cls_pg_price_template .cls_qua_pack_size').val(tempPACK_SIZE_DISPLAY_TXT);
        $('.cls_pg_price_template .cls_qua_dimension_of').val(v.DIMENSION_OF);
        $('.cls_pg_price_template .cls_qua_accessories').val(v.ACCESSORIES);
        $('.cls_pg_price_template .cls_qua_specification').val(PG_GRADE_OF);
        $('.cls_pg_price_template .cls_qua_flute').val(PG_FLUTE);
        $('.cls_pg_price_template .cls_qua_roll_sheet').val(v.ROLL_SHEET);
        $('.cls_pg_price_template .cls_qua_color').val(CHECKLIST_PG_NUMBER_OF_COLOR);
        $('.cls_pg_price_template .cls_qua_coating').val(CHECKLIST_PG_COATING);

        //Tab : Customer
        $('.cls_div_customer .cls_c_customer').val(v.CHECKLIST.SOLD_TO_DISPLAY_TXT);
        $('.cls_div_customer .cls_c_brand').val(CHECKLIST_BRAND);
        $('.cls_div_customer .cls_c_country').val(country);
        $('.cls_div_customer .cls_c_project_name').val(v.CHECKLIST.PROJECT_NAME);
        $('.cls_div_customer .cls_c_mkt_name').val(v.CHECKLIST.CREATOR_NAME);
        $('.cls_div_customer .cls_c_primary_type').val(CHECKLIST_PRIMARY_TYPE);
        $('.cls_div_customer .cls_c_primary_size').val(tempPRIMARY_SIZE_DISPLAY_TXT);
        $('.cls_div_customer .cls_c_net_weight').val(CHECKLIST_NET_WEIGHT);
        $('.cls_div_customer .cls_c_pack_size').val(tempPACK_SIZE_DISPLAY_TXT);
        $('.cls_div_customer .cls_c_packaging_type').val(CHECKLIST_PG_PACKANGING_TYPE);
        $('.cls_div_customer .cls_c_packaging_style').val(CHECKLIST_PG_PACKANGING_STYLE);
        $('.cls_div_customer .cls_c_printing_system').val(CHECKLIST_PG_PRINTING_SYSTEM);

        //Tab : Internal department
        $('.cls_task_form_intdep .cls_wh_customer').val(v.CHECKLIST.SOLD_TO_DISPLAY_TXT);
        $('.cls_task_form_intdep .cls_wh_brand').val(CHECKLIST_BRAND);
        $('.cls_task_form_intdep .cls_wh_primary_type').val(CHECKLIST_PRIMARY_TYPE);
        $('.cls_task_form_intdep .cls_wh_primary_size').val(tempPRIMARY_SIZE_DISPLAY_TXT);
        $('.cls_task_form_intdep .cls_wh_pack_size').val(tempPACK_SIZE_DISPLAY_TXT);
        $('.cls_task_form_intdep .cls_wh_packaging_type').val(CHECKLIST_PG_PACKANGING_TYPE);
        $('.cls_task_form_intdep .cls_wh_packaging_style').val(CHECKLIST_PG_PACKANGING_STYLE);
        $('.cls_task_form_intdep .cls_wh_grade').val(PG_GRADE_OF);
        $('.cls_task_form_intdep .cls_wh_net_weight').val(CHECKLIST_NET_WEIGHT);
        $('.cls_task_form_intdep .cls_wh_dimension_of').val(v.DIMENSION_OF);

        //Tab : Vendor
        $('.cls_pg_price_template_vendor .cls_qua_packaging_type').val(CHECKLIST_PG_PACKANGING_TYPE);
        $('.cls_pg_price_template_vendor .cls_qua_printing_system').val(CHECKLIST_PG_PRINTING_SYSTEM);
        $('.cls_pg_price_template_vendor .cls_qua_brand').val(CHECKLIST_BRAND);
        $('.cls_pg_price_template_vendor .cls_qua_primary_type').val(CHECKLIST_PRIMARY_TYPE);
        $('.cls_pg_price_template_vendor .cls_qua_primary_size').val(tempPRIMARY_SIZE_DISPLAY_TXT);
        $('.cls_pg_price_template_vendor .cls_qua_pack_size').val(tempPACK_SIZE_DISPLAY_TXT);
        $('.cls_pg_price_template_vendor .cls_qua_dimension_of').val(v.DIMENSION_OF);
        $('.cls_pg_price_template_vendor .cls_qua_accessories').val(v.ACCESSORIES);
        $('.cls_pg_price_template_vendor .cls_qua_specification').val(PG_GRADE_OF);
        $('.cls_pg_price_template_vendor .cls_qua_flute').val(PG_FLUTE);
        $('.cls_pg_price_template_vendor .cls_qua_roll_sheet').val(v.ROLL_SHEET);
        $('.cls_pg_price_template_vendor .cls_qua_color').val(CHECKLIST_PG_NUMBER_OF_COLOR);
        $('.cls_pg_price_template_vendor .cls_qua_coating').val(CHECKLIST_PG_COATING);

        //search die line
        binddatapopupSearchDieline();

        $(form_name + '.cls_lov_pkg_sec_material').trigger("change");     //---------by aof 20220118 for CR sustain-- - start
    }

    var lbl_dot = " :";
    var lbl_spanStar = "<span style=\"color: red;\">*</span> :";
    requiredALL();
    requiredALLVendor();
    $('.cls_div_select_vendor .lbl_qua_direction_of_sticker').html('Direction of sticker' + lbl_dot);
    $('.cls_div_select_vendor .lbl_qua_roll_sheet').html('Roll/sheet' + lbl_dot);
    $('.cls_div_select_vendor .cls_qua_roll_sheet').attr("required", false);
    $('.cls_div_select_vendor .cls_qua_direction_of_sticker').attr("required", false);

    $('.cls_task_form_pg .lbl_pg_roll_sheet').html('Roll/sheet' + lbl_dot);
    if (v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("M:") >= 0) {
        //Plactic
        $('.cls_div_select_vendor .lbl_qua_printing_system').html('Printing system' + lbl_dot);
        $('.cls_div_select_vendor .lbl_qua_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
        $('.cls_div_select_vendor .lbl_qua_special_color').html('Special color' + lbl_dot);
        $('.cls_div_select_vendor .lbl_qua_coating').html('Coating' + lbl_dot);
        $('.cls_div_select_vendor .lbl_qua_flute').html('Flute' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_printing_system').attr("required", false);
        $('.cls_div_select_vendor .cls_qua_accessories').attr("required", false);
        $('.cls_div_select_vendor .cls_qua_special_color').attr("required", false);
        $('.cls_div_select_vendor .cls_qua_coating').attr("required", false);
        $('.cls_div_select_vendor .cls_qua_flute').attr("required", false);

        $('#pg_submit_modal_vendor .cls_lbl_vn_acc').html('Accessories (Grad,Qty)' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_lbl_vn_flute').html('Flute' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_vn_acc').attr("required", false);
        $('#pg_submit_modal_vendor .cls_vn_flute').attr("required", false);

        $('.cls_task_form_pg .lbl_pg_printing_system').html('Printing system');
        $('.cls_task_form_pg .lbl_pg_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
        $('.cls_task_form_pg .lbl_pg_flute').html('Flute' + lbl_dot);
        $('.cls_task_form_pg .lbl_pg_coating').html('Coating');
    }
    if (v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("G:") >= 0
        || v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("F:") >= 0
        || v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("N:") >= 0
        || v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("D:") >= 0) {
        //Tray
        //Carton
        //Inner
        //Displayer
    }
    if (v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("C:") >= 0) {
        //Cardboard
        $('.cls_div_select_vendor .lbl_qua_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_accessories').attr("required", false);

        $('#pg_submit_modal_vendor .cls_lbl_vn_acc').html('Accessories (Grad,Qty)' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_vn_acc').attr("required", false);

        $('.cls_task_form_pg .lbl_pg_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
    }
    if (v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("R:") >= 0) {
        //Inner non
        $('.cls_div_select_vendor .lbl_qua_flute').html('Flute' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_flute').attr("required", false);

        $('#pg_submit_modal_vendor .cls_lbl_vn_flute').html('Flute' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_vn_flute').attr("required", false);

        $('.cls_task_form_pg .lbl_pg_flute').html('Flute' + lbl_dot);
    }
    if (v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("H:") >= 0
        || v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("P:") >= 0
        || v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("L:") >= 0) {
        //Sleeve box
        //Insert
        //Leaflet
        $('.cls_div_select_vendor .lbl_qua_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_accessories').attr("required", false);
        $('.cls_div_select_vendor .lbl_qua_flute').html('Flute' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_flute').attr("required", false);

        $('#pg_submit_modal_vendor .cls_lbl_vn_acc').html('Accessories (Grad,Qty)' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_lbl_vn_flute').html('Flute' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_vn_acc').attr("required", false);
        $('#pg_submit_modal_vendor .cls_vn_flute').attr("required", false);

        $('.cls_task_form_pg .lbl_pg_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
        $('.cls_task_form_pg .lbl_pg_flute').html('Flute' + lbl_dot);
    }
    if (v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("K:") >= 0) {
        //Label
        $('.cls_div_select_vendor .lbl_qua_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_accessories').attr("required", false);
        $('.cls_div_select_vendor .lbl_qua_dimension_of').html('Dimension of (LXWXH)(mm.)' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_dimension_of').attr("required", false);

        $('#pg_submit_modal_vendor .cls_lbl_vn_acc').html('Accessories (Grad,Qty)' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_lbl_vn_flute').html('Flute' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_vn_acc').attr("required", false);
        $('#pg_submit_modal_vendor .cls_vn_flute').attr("required", false);
        $('#pg_submit_modal_vendor .cls_lbl_vn_id_mm').html('Dimension of (LXWXH)(mm.)' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_vn_id_mm').attr("required", false);

        $('.cls_task_form_pg .lbl_pg_id_mm').html('Dimension of (LXWXH)(mm.)' + lbl_dot);
        $('.cls_task_form_pg .lbl_pg_accessories').html('Accessories (Grad,Qty)' + lbl_dot);

        $('.cls_task_form_pg .lbl_pg_flute').html('Flute' + lbl_dot);
        $('.cls_div_select_vendor .lbl_qua_flute').html('Flute' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_flute').attr("required", false);
    }
    if (v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT.indexOf("J:") >= 0) {
        //Sticker
        $('.cls_div_select_vendor .lbl_qua_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_accessories').attr("required", false);
        $('.cls_div_select_vendor .lbl_qua_flute').html('Flute' + lbl_dot);
        $('.cls_div_select_vendor .cls_qua_flute').attr("required", false);
        $('.cls_div_select_vendor .lbl_qua_direction_of_sticker').html('Direction of sticker' + lbl_spanStar);
        $('.cls_div_select_vendor .lbl_qua_roll_sheet').html('Roll/sheet' + lbl_spanStar);
        $('.cls_div_select_vendor .cls_qua_roll_sheet').attr("required", true);
        $('.cls_div_select_vendor .cls_qua_direction_of_sticker').attr("required", true);

        $('#pg_submit_modal_vendor .cls_lbl_vn_acc').html('Accessories (Grad,Qty)' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_lbl_vn_flute').html('Flute' + lbl_dot);
        $('#pg_submit_modal_vendor .cls_vn_acc').attr("required", false);
        $('#pg_submit_modal_vendor .cls_vn_flute').attr("required", false);

        $('.cls_task_form_pg .lbl_pg_accessories').html('Accessories (Grad,Qty)' + lbl_dot);
        $('.cls_task_form_pg .lbl_pg_flute').html('Flute' + lbl_dot);
        $('.cls_task_form_pg .lbl_pg_roll_sheet').html('Roll/sheet' + lbl_spanStar);
    }

    setFieldDisablePackagingType();       // by aof 202306 for CR#IGRID_REIM

}

//---------by aof 202306 for CR#IGRID_REIM -----start
function setFieldDisablePackagingType() {

    var isMockup_MO_D; //= $('.cls_header_mockup_no').val().indexOf('MO-D-');


    if (!isEmpty($('.cls_header_mockup_no').val()) && $('.cls_header_mockup_no').val().indexOf('MO-D-') >= 0) {
        isMockup_MO_D = true;
    }
    else
    {
        isMockup_MO_D = false;
    }
   

    if (!isMockup_MO_D)
    {
        if (!$('.cls_lov_pg_packaging_type').prop('disabled')) {

            if ($(".cls_lov_pg_packaging_type  option:selected").text() == "N:Inner Corrugated") {
                $('.cls_lov_rsc_di_cut').attr("disabled", false);
            } else if ($(".cls_lov_pg_packaging_type  option:selected").text() == "F:Carton") {
                $('.cls_lov_rsc_di_cut').attr("disabled", false);
            } else if ($(".cls_lov_pg_packaging_type  option:selected").text() == "G:Tray") {

                $('.cls_lov_rsc_di_cut').attr("disabled", false);
            } else {
                $('.cls_lov_rsc_di_cut').attr("disabled", true);
            }
        }
    }
}
    //---------by aof 202306 for CR#IGRID_REIM -----end


function requiredALL() {
    var lbl_spanStar = "<span style=\"color: red;\">*</span> :";
    $('.cls_div_select_vendor .lbl_qua_printing_system').html('Printing system' + lbl_spanStar);
    $('.cls_div_select_vendor .lbl_qua_dimension_of').html('Dimension of (LXWXH)(mm.)' + lbl_spanStar);
    $('.cls_div_select_vendor .lbl_qua_accessories').html('Accessories (Grad,Qty)' + lbl_spanStar);
    $('.cls_div_select_vendor .lbl_qua_flute').html('Flute' + lbl_spanStar);
    $('.cls_div_select_vendor .lbl_qua_roll_sheet').html('Roll/sheet' + lbl_spanStar);
    $('.cls_div_select_vendor .lbl_qua_direction_of_sticker').html('Direction of sticker' + lbl_spanStar);
    $('.cls_div_select_vendor .lbl_qua_special_color').html('Special color' + lbl_spanStar);
   // $('.cls_div_select_vendor .lbl_qua_coating').html('Coating' + lbl_spanStar);   //@*by aof 202306 for CR#IGRID_REIM---commented@

    $('.cls_div_select_vendor .cls_qua_printing_system').attr("required", true);
    $('.cls_div_select_vendor .cls_qua_dimension_of').attr("required", true);
    $('.cls_div_select_vendor .cls_qua_accessories').attr("required", true);
    $('.cls_div_select_vendor .cls_qua_flute').attr("required", true);
    $('.cls_div_select_vendor .cls_qua_roll_sheet').attr("required", true);
    $('.cls_div_select_vendor .cls_qua_direction_of_sticker').attr("required", true);
    $('.cls_div_select_vendor .cls_qua_special_color').attr("required", true);
    //$('.cls_div_select_vendor .cls_qua_coating').attr("required", true);  //@*by aof 202306 for CR#IGRID_REIM---commented@

    //tab pg data
    $('.cls_task_form_pg .lbl_pg_printing_system').html('Printing system' + '<span style=\"color: red;\">*</span>');
    $('.cls_task_form_pg .lbl_pg_dimension_of').html('Dimension of (LXWXH)(mm.)' + lbl_spanStar);
    $('.cls_task_form_pg .lbl_pg_accessories').html('Accessories (Grad,Qty)' + lbl_spanStar);
    $('.cls_task_form_pg .lbl_pg_flute').html('Flute' + lbl_spanStar);
    $('.cls_task_form_pg .lbl_pg_roll_sheet').html('Roll/sheet' + lbl_spanStar);
   // $('.cls_task_form_pg .lbl_pg_coating').html('Coating' + '<span style=\"color: red;\">*</span>'); //  @*by aof 202306 for CR#IGRID_REIM---commented@
}
function requiredALLVendor() {
    var lbl_spanStar = "<span style=\"color: red;\">*</span> :";
    $('#pg_submit_modal_vendor .cls_lbl_vn_packaging_type').html('Packaging type' + lbl_spanStar);
    //$('#pg_submit_modal_vendor .cls_lbl_vn_brand').html('Brand' + lbl_spanStar);
    $('#pg_submit_modal_vendor .cls_lbl_vn_primary_type').html('Primary type' + lbl_spanStar);
    $('#pg_submit_modal_vendor .cls_lbl_vn_primary_size').html('Primary size' + lbl_spanStar);
    $('#pg_submit_modal_vendor .cls_lbl_vn_pack_size').html('Pack size' + lbl_spanStar);
    $('#pg_submit_modal_vendor .cls_lbl_vn_dimension_of').html('Dimension of (LXWXH)(mm.)' + lbl_spanStar);
    $('#pg_submit_modal_vendor .cls_lbl_vn_spec').html('Specification (Grade of)' + lbl_spanStar);
    $('#pg_submit_modal_vendor .cls_lbl_vn_acc').html('Accessories (Grad,Qty)' + lbl_spanStar);
    $('#pg_submit_modal_vendor .cls_lbl_vn_flute').html('Flute' + lbl_spanStar);
    $('#pg_submit_modal_vendor .cls_lbl_vn_amount').html('จำนวนตัวอย่างที่ขอ' + lbl_spanStar);

    $('#pg_submit_modal_vendor .cls_vn_packaging_type').attr("required", true);
    //$('#pg_submit_modal_vendor .cls_vn_brand').attr("required", true);
    $('#pg_submit_modal_vendor .cls_vn_primary_type').attr("required", true);
    $('#pg_submit_modal_vendor .cls_vn_primary_size').attr("required", true);
    $('#pg_submit_modal_vendor .cls_vn_pack_size').attr("required", true);
    $('#pg_submit_modal_vendor .cls_vn_dimension_of').attr("required", true);
    $('#pg_submit_modal_vendor .cls_vn_spec').attr("required", true);
    $('#pg_submit_modal_vendor .cls_vn_acc').attr("required", true);
    $('#pg_submit_modal_vendor .cls_vn_flute').attr("required", true);
    $('#pg_submit_modal_vendor .cls_vn_amount').attr("required", true);
}


// by aof #INC-4849
function setparamSearchDieline() {
    primaryTypeId = $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other').val();
    packagingTypeId = $('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search').val();
    primarySizeId = $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size').val();
    packSizeId = $('#modal_taskform_pg_search_for_dieline .cls_lov_pack_size option:selected').text();
    packagingStyleId = $('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val();

    dimension_of = $('#modal_taskform_pg_search_for_dieline .cls_lov_dimension_of').val();
    final_info_group = $('#modal_taskform_pg_search_for_dieline .cls_lov_final_info_group').val();
}
// by aof #INC-4849

//function setparamSearchDieline_old() {
//    primaryTypeId = $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other').val();
//    packagingTypeId = $('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search').val();
//    primarySizeId = $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size').val();
//    packSizeId = $('#modal_taskform_pg_search_for_dieline .cls_lov_pack_size').val();
//    packagingStyleId = $('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val();
//}

function binddatapopupSearchDieline() {
    var v = res_pg_request.data[0];
    if (v.CHECKLIST_PG != null) {
        setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search', v.CHECKLIST_PG.PACKING_TYPE_ID, v.CHECKLIST_PG.PACKING_TYPE_DISPLAY_TXT);
    }
    setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other', v.CHECKLIST.PRIMARY_TYPE_ID, v.CHECKLIST.PRIMARY_TYPE_DISPLAY_TXT);
    //$('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val(v.CHECKLIST.PACKING_STYLE_DISPLAY_TXT);
}

function callbackSaveDataPG(res) {
    MOCKUPSUBPGID = res.data[0].MOCKUP_SUB_PG_ID;
    bindDataPG(MainMockupSubId);
}

var send_to_sup_select_quo = false;
function callbackSubmitDataPG(res) {
    send_to_sup_select_quo = false;
    MOCKUPSUBPGID = res.data[0].MOCKUP_SUB_PG_ID;
    var pg_submit_modal = "#pg_submit_modal ";
    var jsonObj = new Object();
    jsonObj.data = [];
    var data = [];
    for (var i = 0; i < 7; i++) {
        var item = {};
        var reasonObj;
        var remarkReasonObj;
        var step_mockup_id = 0;
        var curr_role_id = 0;
        var cls_txtedt = "";
        var cls_chk = "";
        switch (i) {
            case 1:
                reasonObj = ".cls_body_send_planning .cls_lov_send_for_reason";
                remarkReasonObj = ".cls_body_send_planning .cls_input_send_planning_other";
                cls_txtedt = ".cls_txtedt_send_planning";
                cls_chk = ".cls_chk_send_planning";
                step_mockup_id = getstepmockup('SEND_PN_PRI_PKG').curr_step;
                curr_role_id = getstepmockup('SEND_PN_PRI_PKG').curr_role;
                break;
            case 2:
                reasonObj = ".cls_body_send_rd .cls_lov_send_for_reason";
                remarkReasonObj = ".cls_body_send_rd .cls_input_send_rd_other";
                cls_txtedt = ".cls_txtedt_send_rd";
                cls_chk = ".cls_chk_send_rd";
                step_mockup_id = getstepmockup('SEND_RD_PRI_PKG').curr_step;
                curr_role_id = getstepmockup('SEND_RD_PRI_PKG').curr_role;
                break;
            case 3:
                reasonObj = ".cls_body_send_supervisor .cls_lov_send_for_reason";
                remarkReasonObj = ".cls_body_send_supervisor .cls_input_send_supervisor_other";
                cls_txtedt = ".cls_txtedt_send_supervisor";
                cls_chk = ".cls_chk_send_supervisor";
                step_mockup_id = getstepmockup('SEND_PG_SUP_SEL_VENDOR').curr_step;
                curr_role_id = getstepmockup('SEND_PG_SUP_SEL_VENDOR').curr_role;
                break;
            case 4:
                reasonObj = ".cls_body_send_approval_match_board .cls_lov_send_for_reason";
                remarkReasonObj = ".cls_body_send_approval_match_board .cls_input_send_approval_match_board_other";
                cls_txtedt = ".cls_txtedt_send_approval_match_board";
                cls_chk = ".cls_chk_send_approval_match_board";
                step_mockup_id = getstepmockup('SEND_APP_MATCH_BOARD').curr_step;
                curr_role_id = getstepmockup('SEND_APP_MATCH_BOARD').curr_role;
                break;
            case 5:
                reasonObj = ".cls_body_send_supervisor_need_design .cls_lov_send_for_reason";
                remarkReasonObj = ".cls_body_send_supervisor_need_design .cls_input_send_supervisor_need_design_other";
                cls_txtedt = ".cls_txtedt_send_supervisor_need_design";
                cls_chk = ".cls_chk_send_supervisor_need_design";
                step_mockup_id = getstepmockup('SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN').curr_step;
                curr_role_id = getstepmockup('SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN').curr_role;
                break;
            default:
                break;
        }
        if ($(pg_submit_modal + cls_chk).is(":checked")) {

            if (cls_chk == '.cls_chk_send_supervisor') {
                send_to_sup_select_quo = true;
            }

            item["REASON_ID"] = $(reasonObj).val();
            item["REMARK_REASON"] = $(remarkReasonObj).val();
            item["WF_STEP"] = step_mockup_id;
            item["MOCKUP_ID"] = MOCKUPID;
            item["WF_SUB_ID"] = MOCKUPSUBID;
            item["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item["CURRENT_STEP_ID"] = step_mockup_id;
            item["CURRENT_ROLE_ID"] = curr_role_id;
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;
            item["IS_SENDER"] = true;
            var editor = new Quill(pg_submit_modal + cls_txtedt);
            item["REMARK"] = editor.root.innerHTML;
            data.push(item);
        }
    }

    if ($('.cls_header_chk[type="checkbox"]:checked').length > 0) {
        if (data.length > 0) {
            showalertwh = false;
            jsonObj.data = {};
            jsonObj.data = data;
            var myurl = '/api/taskform/mockupprocess/routing';
            var mytype = 'POST';
            var mydata = jsonObj;
            myAjax(myurl, mytype, mydata, callbackPGSubmit,'',true,true,true);
        }
        else {
            savewhbypg();
        }
    }
    else {
        $(".se-pre-con").fadeOut('fast');
        alertError2("Please select at least 1 item.");
    }
}

var showalertwh = true;
function savewhbypg() {
    var pg_submit_modal = "#pg_submit_modal ";

    if ($(pg_submit_modal + '.cls_chk_send_warehouse').is(":checked")) {
        var jsonObj = new Object();
        jsonObj.data = {};
        jsonObj.data.PROCESS = {};

        jsonObj.data["MOCKUP_ID"] = MOCKUPID;
        jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        jsonObj.data["TEST_PACK_SIZING"] = $('.cls_body_send_warehouse .cls_chk_confirm_size').is(":checked") ? "X" : "";
        jsonObj.data["TEST_PACK_HARD_EASY"] = $('.cls_body_send_warehouse .cls_chk_easy').is(":checked") ? "X" : "";
        jsonObj.data["CREATE_BY"] = UserID;
        jsonObj.data["UPDATE_BY"] = UserID;
        jsonObj.data["SUPPLIER_PRIMARY_CONTAINER"] = $('.cls_body_send_warehouse .cls_wh_supplier_primary_container').val();
        jsonObj.data["SUPPLIER_PRIMARY_LID"] = $('.cls_body_send_warehouse .cls_wh_supplier_primary_lid').val();
        jsonObj.data["SHIP_TO_FACTORY"] = $('.cls_body_send_warehouse .cls_wh_remark').val();

        jsonObj.data.PROCESS["REASON_ID"] = $('.cls_body_send_warehouse .cls_lov_send_for_reason').val();
        jsonObj.data.PROCESS["MOCKUP_ID"] = MOCKUPID;
        jsonObj.data.PROCESS["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;
        jsonObj.data.PROCESS["CURRENT_STEP_ID"] = getstepmockup('SEND_WH_TEST_PACK').curr_step;
        jsonObj.data.PROCESS["CURRENT_ROLE_ID"] = getstepmockup('SEND_WH_TEST_PACK').curr_role;
        jsonObj.data.PROCESS["CREATE_BY"] = UserID;
        jsonObj.data.PROCESS["UPDATE_BY"] = UserID;
        var editor = new Quill('.cls_body_send_warehouse .cls_txtedt_send_warehouse');
        jsonObj.data.PROCESS["REMARK"] = editor.root.innerHTML;

        var myurl = '/api/taskform/pg/sendtowarehouse';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxAlertError2(myurl, mytype, mydata, callbackPGSubmitForWH, '', true, showalertwh);
        showalertwh = true;
    }
}

function callbackPGSubmit(res) {
    savewhbypg();

    var pg_submit_modal = "#pg_submit_modal ";
    $(pg_submit_modal).modal('hide');

    if (send_to_sup_select_quo) {
        $('.cls_pg_price_template .cls_table_user_vendor_price_template').hide();
        $('.cls_pg_price_template .cls_btn_request_quotation_price_template').hide();
        $('.cls_pg_price_template .cls_btn_reset_vendor_price_template').hide();
        $('.cls_pg_price_template .cls_btn_save_vendor_price_template').hide();
        $('.cls_pg_price_template .cls_row_select_vendor').hide();
        $('.cls_pg_price_template .cls_div_price_template').hide();
        $('.cls_div_price_compare .cls_row_manual_add_vendor').hide();
    }


    resetDllReason(pg_submit_modal + ' .cls_lov_send_for_reason');
    $(pg_submit_modal + ' .cls_lov_search_file_template').val('').trigger('change');
    var text_editor_pn = new Quill(pg_submit_modal + '.cls_txtedt_send_planning');
    var text_editor_rd = new Quill(pg_submit_modal + '.cls_txtedt_send_rd');
    var text_editor_supervisor = new Quill(pg_submit_modal + '.cls_txtedt_send_supervisor');
    var text_editor_approval_match_board = new Quill(pg_submit_modal + '.cls_txtedt_send_approval_match_board');
    var text_editor_wh = new Quill(pg_submit_modal + '.cls_txtedt_send_warehouse');
    var text_editor_supervisor_need_design = new Quill(pg_submit_modal + '.cls_txtedt_send_supervisor_need_design');
    text_editor_pn.setContents([{ insert: '\n' }]);
    text_editor_rd.setContents([{ insert: '\n' }]);
    text_editor_supervisor.setContents([{ insert: '\n' }]);
    text_editor_approval_match_board.setContents([{ insert: '\n' }]);
    text_editor_wh.setContents([{ insert: '\n' }]);
    text_editor_supervisor_need_design.setContents([{ insert: '\n' }]);
    $(pg_submit_modal + ' input:text:enabled').val('');
    $(pg_submit_modal + ' input:checkbox').prop('checked', false);
    $(pg_submit_modal + '.cls_body_send_planning').hide();
    $(pg_submit_modal + '.cls_body_send_rd').hide();
    $(pg_submit_modal + '.cls_body_send_supervisor').hide();
    $(pg_submit_modal + '.cls_body_send_approval_match_board').hide();
    $(pg_submit_modal + '.cls_body_send_supervisor_need_design').hide();
    $(pg_submit_modal + '.cls_body_send_warehouse').hide();
    $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_supplier_primary_container').prop('required', false);
    $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_supplier_primary_lid').prop('required', false);
    $(pg_submit_modal + '.cls_body_send_warehouse .cls_wh_remark').prop('required', false);

}
function callbackPGSubmitForWH(res) {
    var pg_submit_modal = "#pg_submit_modal ";
    $(pg_submit_modal).modal('hide');
}

var primaryTypeId;
var packagingTypeId;
var primarySizeId;
var packSizeId;
var packagingStyleId;
var dimension_of;  //by aof #INC-4849
var final_info_group; //by aof  #INC-4849



function bindSearchForDieline() {

    $('.cls_table_text_search').val('');
    setparamSearchDieline();
    table_search_for_dieline.destroy();

    table_search_for_dieline = $('#table_taskform_pg_search_for_dieline').DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHeader: true,
        columnDefs: [{
            "searchable": false,
            "orderable": false,
            "targets": 0
        }],
        ajax: function (data, callback, settings) {

            //for (var i = 0, len = data.columns.length; i < len; i++) {
            //    delete data.columns[i].name;
            //    delete data.columns[i].data;
            //    delete data.columns[i].searchable;
            //    delete data.columns[i].orderable;
            //    delete data.columns[i].search.regex;
            //    delete data.search.regex;

            //    delete data.columns[i].search.value;
            //}

            $.ajax({
               // url: suburl + "/api/taskform/pg/searchdieline2?data.check_list_id=" + CheckListID + "&data.mockup_id=" + MOCKUPID + "&data.PRIMARY_TYPE_ID=" + primaryTypeId + "&data.PACKING_TYPE_ID=" + packagingTypeId + "&data.PRIMARY_SIZE_DISPLAY_TXT=" + primarySizeId + "&data.PACK_SIZE_ID=" + packSizeId + "&data.PACKING_STYLE_DISPLAY_TXT=" + packagingStyleId,
                url: suburl + "/api/taskform/pg/searchdieline_tutuning?data.check_list_id=" + CheckListID + "&data.mockup_id=" + MOCKUPID
                    + "&data.PRIMARY_TYPE_ID=" + primaryTypeId  //by aof
                    + "&data.PACKING_TYPE_ID=" + packagingTypeId
                    + "&data.PRIMARY_SIZE_DISPLAY_TXT=" + primarySizeId
                    + "&data.PACK_SIZE_DISPLAY_TXT=" + packSizeId
                    + "&data.PACKING_STYLE_DISPLAY_TXT=" + packagingStyleId
                    + "&data.DIMENSION_OF_DISPLAY_TXT=" + dimension_of
                    + "&data.FINAL_INFO_GROUP_DISPLAY_TXT=" + final_info_group
                    + "&data.FIRST_LOAD=" + first_load,   

                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        fixedColumns: {
            leftColumns: 3
        },
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            { data: "CHECK_LIST_NO", "className": "cls_nowrap" },
            { data: "MOCKUP_NO", "className": "cls_nowrap" },
            { data: "PRIMARY_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "CONTAINER_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "LID_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACK_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKING_STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKAGING_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "GRADE_OF_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "DIMENSION_OF_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "FINAL_INFO_GROUP_DISPLAY_TXT", "className": "cls_nowrap" },  //by aof
            { data: "SHEET_SIZE", "className": "cls_nowrap" },
            { data: "FLUTE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STATUS_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "NUMBER_OF_COLOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STYLE_OF_PRINTING_DISPLAY_TXT", "className": "cls_nowrap" }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "processing": true,
        "scrollX": true,
        order: [[1, 'asc']],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(1).html('<a target="_blank" href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '"> ' + data.CHECK_LIST_NO + ' </a>');
            $(row).find('td').eq(2).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '"> ' + data.MOCKUP_NO + ' </a>');
        },
        initComplete: function (settings, json) {

        }
    });


    $(table_search_for_dieline.table().container()).on('keyup', 'input', function () {
        if (event.which == 13) {
            table_search_for_dieline
                .column($(this).data('index'))
                .search(this.value, false)
                .draw();
        }       
    });

    $("#table_taskform_pg_search_for_dieline_filter").hide();
}




function bindSearchForDieline_old() {

    setparamSearchDieline();
    table_search_for_dieline.destroy();

    table_search_for_dieline = $('#table_taskform_pg_search_for_dieline').DataTable({
        serverSide: true,
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
                url: suburl + "/api/taskform/pg/searchdieline2?data.check_list_id=" + CheckListID + "&data.mockup_id=" + MOCKUPID + "&data.PRIMARY_TYPE_ID=" + primaryTypeId + "&data.PACKING_TYPE_ID=" + packagingTypeId + "&data.PRIMARY_SIZE_DISPLAY_TXT=" + primarySizeId + "&data.PACK_SIZE_ID=" + packSizeId + "&data.PACKING_STYLE_DISPLAY_TXT=" + packagingStyleId,
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
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            { data: "CHECK_LIST_NO", "className": "cls_nowrap" },
            { data: "MOCKUP_NO", "className": "cls_nowrap" },
            { data: "PRIMARY_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "CONTAINER_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "LID_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACK_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKING_STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKAGING_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "GRADE_OF_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "DIMENSION_OF_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "FINAL_INFO_GROUP_DISPLAY_TXT", "className": "cls_nowrap" },  //by aof
            { data: "SHEET_SIZE", "className": "cls_nowrap" },
            { data: "FLUTE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STATUS_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "NUMBER_OF_COLOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STYLE_OF_PRINTING_DISPLAY_TXT", "className": "cls_nowrap" }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "processing": true,
        "scrollX": true,
        order: [[1, 'asc']],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(1).html('<a target="_blank" href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '"> ' + data.CHECK_LIST_NO + ' </a>');
            $(row).find('td').eq(2).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '"> ' + data.MOCKUP_NO + ' </a>');
        },
        initComplete: function (settings, json) {

        }
    });
}

var tablDieline;
function bindDataPG_Dieline(v) {
    $('.cls_task_form_pg .cls_row_pg_dieline').show();
    $('.cls_task_form_pg .cls_hid_pg_dieline_mockup_id').val(v.MOCKUP_ID);
    tablDieline = $('#table_chk_list_pg_dieline').DataTable({
        "searching": false,
        "ordering": false,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bAutoWidth": false,
        "scrollX": true,
    });
    tablDieline.clear().draw();
    tablDieline.row.add([
        '<a target="_blank" href="' + suburl + '/CheckList/' + v.CHECK_LIST_ID + '">' + v.CHECK_LIST_NO + '</a>',
        '<a target="_blank" href="' + suburl + '/TaskForm/' + v.MOCKUP_SUB_ID + '">' + v.MOCKUP_NO + '</a>',
        v.STATUS_DISPLAY_TXT,
    ]).draw(false);
}


  //---------by aof 20220118 for CR sustain-- - start
// THE SCRIPT THAT CHECKS IF THE KEY PRESSED IS A NUMERIC OR DECIMAL VALUE.
function isKeyDeicmal(evt, element) {

    var charCode = (evt.which) ? evt.which : event.keyCode

    if (
        //(charCode != 45 || $(element).val().indexOf('-') != -1) &&      // “-” CHECK MINUS, AND ONLY ONE.
        (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
        (charCode < 48 || charCode > 57))
        return false;

    return true;
}
function isKeyNumber(evt, element) {

    var charCode = (evt.which) ? evt.which : event.keyCode

    if (
        //(charCode != 45 || $(element).val().indexOf('-') != -1) &&      // “-” CHECK MINUS, AND ONLY ONE.
        //(charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
        (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function setStringToNumber(val) {
    val = val.replace(/,/g, ''); // strip out commas
    if (!isEmpty(val)) {
        val = parseFloat(val);
        if (isNaN(val)) {
            val = 0
        }
    } else {
        val = 0
    }
    return val
}

function setNumberToStringFormat(val, point) {
    if (val != null) {
        if (!$.isNumeric(val)) {
            val = val.replace(/,/g, ''); // strip out commas
        }
        if (!isEmpty(val)) {
            val = parseFloat(val);
            if (isNaN(val)) {
                val = 0
            }

            //$(".cls_txt_pkg_sec_weight").val(format_number(weg));
        } else {
            val = 0
        }
    } else
    {
        val = 0 
    }


    val = val.toFixed(point).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");

    return val
}

  //---------by aof 20220118 for CR sustain-- -end
