var ARTWORK_SUB_PA_ID = 0;
var defaultPA_MaterialGroupData = [];
var defaultPA_PrimarySizeData = [];
var defaultPA_PackingStyleData = [];
var defaultPA_ProductCode = [];
var defaultPA_RDRef = [];
var cnt_lov_tfartwork_fao_zone_multiple = 1;
var cnt_lov_tfartwork_catching_area_multiple = 1;
var cnt_lov_tfartwork_catching_method_multiple = 1; // // ticke#425737 added by aof
var cnt_lov_tfartwork_symbol_multiple = 1;
var cnt_lov_tfartwork_plant_multiple = 1;
var pg_quill, mk_quill, qc_quill, wh_quill, plan_quill, pp_quill;
var vn_aw_quill, vn_shade_quill;
var cus_print_quill, cus_req_ref_quill, cus_shade_ref_quill, cus_shade_quill;
var pa_sendback_quill, pa_terminate_quill, pa_killprocess_quill;
var countCus = 0;
var countvnpm, countvnsl = 0;
var default_decision;
var productcode_ = "";
var data_product_code = [];
var data_rd_ref = [];
var CHECK_DIF_REQ_RD = false;
var check_vap_type = [];
var check_msg = "", event_val = "";
var typecheck_pa_vap = "";

var prev_packingstyle_val = "";     //  added by aof r CR#IGRID_REIM_SPRINT2
var prev_pakcingstyle_display = "";  //  added by aof r CR#IGRID_REIM_SPRINT2
 
$(document).ready(function () {
    typecheck_pa_vap = "load";
    $('#send_to_customer_modal form').validate({
        rules: {
            cus_form_modal_chk:
            {
                required: true
            },
            cus_reason_modal_chk:
            {
                required: true
            }
        },
        messages: {
            cus_form_modal_chk:
            {
                required: "Please check at least 1 of these fields."
            },
            cus_reason_modal_chk:
            {
                required: "Please check at least 1 of these fields."
            }
        }
    });

    $('#pa_submit_modal form').validate({
        rules: {
            chkWh:
            {
                required: true
            }
        },
        messages: {
            chkWh:
            {
                required: "Please check at least 1 of these fields."
            }
        }
    });

    var pa_submit_modal = "#pa_submit_modal ";
    var send_to_vendor_modal = "#send_to_vendor_modal ";
    var send_to_customer_modal = "#send_to_customer_modal ";

    pg_quill = bind_text_editor(pa_submit_modal + '.cls_txt_send_pg');
    mk_quill = bind_text_editor(pa_submit_modal + '.cls_txt_send_mk');
    qc_quill = bind_text_editor(pa_submit_modal + '.cls_txt_send_qc');
    wh_quill = bind_text_editor(pa_submit_modal + '.cls_txt_send_wh');
    plan_quill = bind_text_editor(pa_submit_modal + '.cls_txt_send_plan');
    pp_quill = bind_text_editor(pa_submit_modal + '.cls_txt_send_pp');

    vn_aw_quill = bind_text_editor(send_to_vendor_modal + '.cls_txt_send_artwork_vendor');
    vn_shade_quill = bind_text_editor(send_to_vendor_modal + '.cls_txt_send_shade');

    cus_print_quill = bind_text_editor(send_to_customer_modal + '.cls_txt_send_print');
    cus_req_ref_quill = bind_text_editor(send_to_customer_modal + '.cls_txt_send_req_ref');
    cus_shade_quill = bind_text_editor(send_to_customer_modal + '.cls_txt_send_shade');
    cus_shade_ref_quill = bind_text_editor(send_to_customer_modal + '.cls_txt_send_shade_ref');

    pa_sendback_quill = bind_text_editor('#modal_tfartwork_pa_sendback .cls_txtedt_tfartwork_pa_sendback_remark');
    pa_terminate_quill = bind_text_editor('#modal_tfartwork_pa_terminate .cls_txtedt_tfartwork_pa_terminate_remark');

    pa_killprocess_quill = bind_text_editor('#modal_tfartwork_history_terminate .cls_txtedt_tfartwork_history_terminate_remark');

    bind_lov_param('.cls_lov_pa_copy_artworkno', '/api/lov/artworkno', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    //bind_lov('.cls_lov_pa_copy_artworkno', '/api/lov/artworkno', 'data.DISPLAY_TXT');

    bind_lov_confirm('.cls_lov_pa_material_group', '/api/lov/packtype', 'data.DISPLAY_TXT', '', 'Are you sure want to change material group?', getPA_MatGroupValue);

    bind_lov_param('.cls_lov_pa_rd_reference_no', '/api/lov/pa/rdreferenceno', 'data.DISPLAY_TXT', ["ARTWORK_REQUEST_ID"], ['.cls_input_artwork_request_id']);
    bind_lov_param('.cls_lov_pa_product_code', '/api/lov/pa/productcode', 'data.DISPLAY_TXT', ["ARTWORK_REQUEST_ID"], ['.cls_input_artwork_request_id']);

    //--------------------------// by aof  packing style 01/20/2021- -------------------------------
    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_packing_style', '/api/lov/2p', 'data.DISPLAY_TXT', '.cls_input_pa_packing_style');
    bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_packing_style', '/api/lov/2p_new', 'data.DISPLAY_TXT', ["PRIMARY_TYPE_ID"], ['.cls_artwork_request_form .cls_lov_artwork_primary_type_other'], '.cls_input_pa_packing_style');
    //--------------------------// by aof  packing style 01/20/2021- -------------------------------

 
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_pack_size', '/api/lov/packsize', 'data.DISPLAY_TXT', '.cls_input_pa_pack_size');
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_primary_size', '/api/lov/3p', 'data.DISPLAY_TXT', '.cls_input_pa_primary_size');  //by aof 202306 for CR#IGRID_REIM_PA include others
    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_primary_size', '/api/lov/3p', 'data.DISPLAY_TXT'); //by aof 202306 for CR#IGRID_REIM_PA exclude others
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_lid_type', '/api/lov/lidtype', 'data.DISPLAY_TXT', '.cls_input_pa_lid_type');
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_container_type', '/api/lov/containerType', 'data.DISPLAY_TXT', '.cls_input_pa_container_type');

    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_fao_zone', '/api/lov/pa/fao', 'data.DISPLAY_TXT', '.cls_input_pa_fao_zone');
    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_plant_register_no', '/api/lov/pa/plantregisteredno', 'data.DISPLAY_TXT', '.cls_input_pa_plant_register_no');

    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_company_name', '/api/lov/pa/companyaddress', 'data.DISPLAY_TXT', '.cls_input_pa_company_name');
    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_catching_area', '/api/lov/pa/catchingarea', 'data.DISPLAY_TXT', '.cls_input_pa_catching_area');
    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_symbol', '/api/lov/pa/shcclaim', 'data.DISPLAY_TXT', '.cls_input_pa_symbol');
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_catching_period', '/api/lov/pa/catchingperiod', 'data.DISPLAY_TXT'); //, '.cls_input_pa_catching_period'); //IGRID_REIM_PA exclude other
   // bind_lov('.cls_tfartwork_pa .cls_lov_pa_catching_method', '/api/lov/pa/catchingmethod', 'data.DISPLAY_TXT', '.cls_input_pa_catching_method');  // ticke#425737 comented by aof 
    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_plant', '/api/lov/plant', 'data.DISPLAY_TXT', '.cls_input_pa_plant');
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_scientific_name', '/api/lov/pa/scientificname', 'data.DISPLAY_TXT'); //, '.cls_input_pa_scientific_name'); //IGRID_REIM_PA exclude other
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_direction_of_sticker', '/api/lov/pa/directionofsticker', 'data.DISPLAY_TXT'); //, '.cls_input_pa_direction_of_sticker'); /IGRID_REIM_PA exclude other
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_specie', '/api/lov/pa/specie', 'data.DISPLAY_TXT'); //, '.cls_input_pa_specie'); /IGRID_REIM_PA exclude other
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_printing_style_of_primary', '/api/lov/pa/printingstyleofprimary', 'data.DISPLAY_TXT', '.cls_input_pa_printing_style_of_primary');
    bind_lov('.cls_tfartwork_pa .cls_lov_pa_printing_style_of_secondary', '/api/lov/pa/printingstyleofsecondary', 'data.DISPLAY_TXT', '.cls_input_pa_printing_style_of_secondary');

    bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_type_of', '/api/lov/pa/typeof', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']); //, '.cls_input_pa_type_of'); //IGRID_REIM_PA exclude other
    bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_type_of_two', '/api/lov/pa/typeof2', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']); //, '.cls_input_pa_type_of_two'); //IGRID_REIM_PA exclude other
    bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_style_of_printing', '/api/lov/pa/styleofprintting', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_style_of_printing');
    bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_pms_colour', '/api/lov/pa/pms', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']); //, '.cls_input_pa_pms_colour');  //IGRID_REIM_PA exclude other
    bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_process_colour', '/api/lov/pa/processcolour', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']) //, '.cls_input_pa_process_colour'); //IGRID_REIM_PA exclude other
    bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_total_colour', '/api/lov/pa/totalcolour', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_total_colour');

    bind_lov('#modal_tfartwork_pa_sendback .cls_lov_marketing', '/api/lov/pa/marketing', 'data.DISPLAY_TXT');
    bind_lov_reason('#modal_tfartwork_pa_terminate .cls_lov_artwork_reason_terminate', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_TERMINATE_REASON&data.IS_ACTIVE=X', 'data.DISPLAY_TXT');


   

    $(document).on("click", ".cls_tfartwork_pa .cls_img_lov_tfartwork_pa_add_fao_zone_multiple", function () {
        var obj = $('.tr_tfartwork_pa_fao_zone_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pa_fao_zone_multiple');
        obj.find('.cls_lov_pa_fao_zone_multiple').toggleClass('cls_lov_pa_fao_zone_multiple cls_lov_pa_fao_zone_multiple' + cnt_lov_tfartwork_fao_zone_multiple);
        //obj.find('.cls_input_pa_fao_zone_multiple').toggleClass('cls_input_pa_fao_zone_multiple cls_input_pa_fao_zone_multiple' + cnt_lov_tfartwork_fao_zone_multiple); //IGRID_REIM_PA exclude other
        obj.insertAfter($('.tr_tfartwork_pa_fao_zone_multiple_static:last'));
        bind_lov('.cls_lov_pa_fao_zone_multiple' + cnt_lov_tfartwork_fao_zone_multiple, '/api/lov/pa/fao', 'data.DISPLAY_TXT');//, '.cls_input_pa_fao_zone_multiple' + cnt_lov_tfartwork_fao_zone_multiple); //IGRID_REIM_PA exclude other

        if (cnt_lov_tfartwork_fao_zone_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pa_add_fao_zone_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_fao_zone_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_fao_zone_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_fao_zone_multiple++;
    });
    $(".cls_img_lov_tfartwork_pa_add_fao_zone_multiple").click();

    $(document).on("click", ".cls_tfartwork_pa .cls_img_lov_tfartwork_pa_add_catching_area_multiple", function () {
        var obj = $('.tr_tfartwork_pa_catching_area_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pa_catching_area_multiple');
        obj.find('.cls_lov_pa_catching_area_multiple').toggleClass('cls_lov_pa_catching_area_multiple cls_lov_pa_catching_area_multiple' + cnt_lov_tfartwork_catching_area_multiple);
        //obj.find('.cls_input_pa_catching_area_multiple').toggleClass('cls_input_pa_catching_area_multiple cls_input_pa_catching_area_multiple' + cnt_lov_tfartwork_catching_area_multiple);  //IGRID_REIM_PA exclude other

        obj.insertAfter($('.tr_tfartwork_pa_catching_area_multiple_static:last'));
        bind_lov('.cls_lov_pa_catching_area_multiple' + cnt_lov_tfartwork_catching_area_multiple, '/api/lov/pa/catchingarea', 'data.DISPLAY_TXT'); //, '.cls_input_pa_catching_area_multiple' + cnt_lov_tfartwork_catching_area_multiple);  //IGRID_REIM_PA exclude other
        if (cnt_lov_tfartwork_catching_area_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pa_add_catching_area_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_catching_area_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_catching_area_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_catching_area_multiple++;
    });
    $(".cls_img_lov_tfartwork_pa_add_catching_area_multiple").click();

   // ticke#425737 added by aof
    $(document).on("click", ".cls_tfartwork_pa .cls_img_lov_tfartwork_pa_add_catching_method_multiple", function () {
        var obj = $('.tr_tfartwork_pa_catching_method_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pa_catching_method_multiple');
        obj.find('.cls_lov_pa_catching_method_multiple').toggleClass('cls_lov_pa_catching_method_multiple cls_lov_pa_catching_method_multiple' + cnt_lov_tfartwork_catching_method_multiple);
       // obj.find('.cls_input_pa_catching_method_multiple').toggleClass('cls_input_pa_catching_method_multiple cls_input_pa_catching_method_multiple' + cnt_lov_tfartwork_catching_method_multiple); //IGRID_REIM_PA exclude other
        obj.insertAfter($('.tr_tfartwork_pa_catching_method_multiple_static:last'));
        bind_lov('.cls_lov_pa_catching_method_multiple' + cnt_lov_tfartwork_catching_method_multiple, '/api/lov/pa/catchingmethod', 'data.DISPLAY_TXT');//, '.cls_input_pa_catching_method_multiple' + cnt_lov_tfartwork_catching_method_multiple); //IGRID_REIM_PA exclude other

        if (cnt_lov_tfartwork_catching_method_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pa_add_catching_method_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_catching_method_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_catching_method_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_catching_method_multiple++;
    });
    $(".cls_img_lov_tfartwork_pa_add_catching_method_multiple").click();

   // ticke#425737 added by aof


    $(document).on("click", ".cls_tfartwork_pa .cls_img_lov_tfartwork_pa_add_symbol_multiple", function () {

        var obj = $('.tr_tfartwork_pa_symbol_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pa_symbol_multiple');
        obj.find('.cls_lov_pa_symbol_multiple').toggleClass('cls_lov_pa_symbol_multiple cls_lov_pa_symbol_multiple' + cnt_lov_tfartwork_symbol_multiple);
       // obj.find('.cls_input_pa_symbol_multiple').toggleClass('cls_input_pa_symbol_multiple cls_input_pa_symbol_multiple' + cnt_lov_tfartwork_symbol_multiple);  //CR#IGRID_REIM_PA exclude other
        obj.insertAfter($('.tr_tfartwork_pa_symbol_multiple_static:last'));

          //start by aof 202306 for CR#IGRID_REIM_PA
        var where_not_in_symbol_id = "";
        $(".cls_tfartwork_pa .tr_tfartwork_pa_symbol_multiple_static").each(function (index) {
            if (!isEmpty($(this).find('.cls_lov_pa_symbol_multiple_static').val())) {
                if (where_not_in_symbol_id != "") {
                    where_not_in_symbol_id = where_not_in_symbol_id + ",";
                }
                where_not_in_symbol_id = where_not_in_symbol_id + $(this).find('.cls_lov_pa_symbol_multiple_static').val();;
            }

            $(".cls_txt_symbol_id").val(where_not_in_symbol_id);
        });
          //end by aof 202306 for CR#IGRID_REIM_PA
    
        bind_lov_param('.cls_lov_pa_symbol_multiple' + cnt_lov_tfartwork_symbol_multiple, '/api/lov/pa/shcclaim', 'data.DISPLAY_TXT', ["WHERE_NOT_IN_CHARACTERISTIC_ID"], ['.cls_txt_symbol_id']);//, '.cls_input_pa_symbol_multiple' + cnt_lov_tfartwork_symbol_multiple);   //by aof 202306 for CR#IGRID_REIM_PA exclude other
        //bind_lov('.cls_lov_pa_symbol_multiple' + cnt_lov_tfartwork_symbol_multiple, '/api/lov/pa/shcclaim', 'data.DISPLAY_TXT', '.cls_input_pa_symbol_multiple' + cnt_lov_tfartwork_symbol_multiple);    //by aof 202306 for CR#IGRID_REIM_PA

        if (cnt_lov_tfartwork_symbol_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pa_add_symbol_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_symbol_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_symbol_multiple").click(function () {
            $(this).closest('.row').remove();
        });

          //start by aof 202306 for CR#IGRID_REIM_PA
        $(obj).find(".cls_lov_pa_symbol_multiple" + cnt_lov_tfartwork_symbol_multiple).on("change", function () {
            var where_not_in_symbol_id = "";
            $(".cls_tfartwork_pa .tr_tfartwork_pa_symbol_multiple_static").each(function (index) {

                if (!isEmpty($(this).find('.cls_lov_pa_symbol_multiple_static').val())) {

                    if (where_not_in_symbol_id != "") {
                        where_not_in_symbol_id = where_not_in_symbol_id + ",";
                    }
                    where_not_in_symbol_id = where_not_in_symbol_id + $(this).find('.cls_lov_pa_symbol_multiple_static').val();
                }

            });
            $(".cls_txt_symbol_id").val(where_not_in_symbol_id);
        });
          //end by aof 202306 for CR#IGRID_REIM_PA

        cnt_lov_tfartwork_symbol_multiple++;

    });
    $(".cls_img_lov_tfartwork_pa_add_symbol_multiple").click();

    $(document).on("click", ".cls_tfartwork_pa .cls_img_lov_tfartwork_pa_add_plant_multiple", function () {
        var obj = $('.tr_tfartwork_pa_plant_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pa_plant_multiple');
        obj.find('.cls_lov_pa_plant_multiple').toggleClass('cls_lov_pa_plant_multiple cls_lov_pa_plant_multiple' + cnt_lov_tfartwork_plant_multiple);
       // obj.find('.cls_input_pa_plant_multiple').toggleClass('cls_input_pa_plant_multiple cls_input_pa_plant_multiple' + cnt_lov_tfartwork_plant_multiple);  // CR#IGRID_REIM_PA exclude other
        obj.insertAfter($('.tr_tfartwork_pa_plant_multiple_static:last'));
        bind_lov('.cls_lov_pa_plant_multiple' + cnt_lov_tfartwork_plant_multiple, '/api/lov/plant', 'data.DISPLAY_TXT'); //, '.cls_input_pa_plant_multiple' + cnt_lov_tfartwork_plant_multiple);  // CR#IGRID_REIM_PA exclude other

        if (cnt_lov_tfartwork_plant_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pa_add_plant_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_plant_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_plant_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_plant_multiple++;
    });
    $(".cls_img_lov_tfartwork_pa_add_plant_multiple").click();

    $('.cls_tfartwork_pa .cls_lov_pa_primary_size').on("change", function () {
        var selections = $(this).select2('data');
        var v = $(this)[0].value;
        var isClear = true;
        if (selections.length > 0) {
            if (selections[0].ID != -1 && selections[0].ID != undefined) {
                setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_container_type', selections[0].CONTAINER_TYPE_VALUE, selections[0].CONTAINER_TYPE_DESCRIPTION);
                setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_lid_type', selections[0].LID_TYPE_VALUE, selections[0].LID_TYPE_DESCRIPTION);
                $('.cls_tfartwork_pa .cls_lov_pa_container_type').prop("disabled", true);
                $('.cls_tfartwork_pa .cls_lov_pa_lid_type').prop("disabled", true);
                $('.cls_tfartwork_pa .cls_input_pa_container_type').hide();  // added by aof r CR#IGRID_REIM
                $('.cls_tfartwork_pa .cls_input_pa_lid_type').hide();  // added by aof r CR#IGRID_REIM
                isClear = false;
            }
            else if (defaultPA_PrimarySizeData[v] != undefined) {
                setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_container_type', defaultPA_PrimarySizeData[v].CONTAINER_TYPE_ID, defaultPA_PrimarySizeData[v].CONTAINER_TYPE_DISPLAY_TXT);
                setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_lid_type', defaultPA_PrimarySizeData[v].LID_TYPE_ID, defaultPA_PrimarySizeData[v].LID_TYPE_DISPLAY_TXT);
                if (v == -1) {
                    $('.cls_tfartwork_pa .cls_lov_pa_container_type').prop("disabled", false);    //start @* by aof 202306 for CR#IGRID_REIM----commented
                    $('.cls_tfartwork_pa .cls_lov_pa_lid_type').prop("disabled", false);     //start @* by aof 202306 for CR#IGRID_REIM----commented
                }
                else {
                    $('.cls_tfartwork_pa .cls_lov_pa_container_type').prop("disabled", true);
                    $('.cls_tfartwork_pa .cls_lov_pa_lid_type').prop("disabled", true);
                    $('.cls_tfartwork_pa .cls_input_pa_container_type').hide();  // added by aof r CR#IGRID_REIM
                    $('.cls_tfartwork_pa .cls_input_pa_lid_type').hide();  // added by aof r CR#IGRID_REIM
                }
                isClear = false;
            }
        }

        if (isClear) {
            $('.cls_tfartwork_pa .cls_lov_pa_container_type').empty();
            $('.cls_tfartwork_pa .cls_lov_pa_lid_type').empty();
            $('.cls_tfartwork_pa .cls_lov_pa_container_type').prop("disabled", false);   //start @* by aof 202306 for CR#IGRID_REIM----commented
            $('.cls_tfartwork_pa .cls_lov_pa_lid_type').prop("disabled", false);   //start @* by aof 202306 for CR#IGRID_REIM----commented
            $('.cls_tfartwork_pa .cls_input_pa_container_type').hide();  // added by aof r CR#IGRID_REIM
            $('.cls_tfartwork_pa .cls_input_pa_lid_type').hide();  // added by aof r CR#IGRID_REIM
        }
    });

    function callbackTest()
    {
        alertDialog("test");
    }
    $('.cls_tfartwork_pa .cls_lov_pa_packing_style').on("change", function () {
        var selections = $(this).select2('data');
        var v = $(this)[0].value;
        var isClear = true;



          // start added by aof r CR#IGRID_REIM_SPRINT2

        if (!confirmPackingStyleDifferentRequestForm()) {
            setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_packing_style', prev_packingstyle_val, prev_packingstyle_display);
            return;
        }
        
        //if (selections.length > 0)
        //{
        //    var packingSytle = "";
        //    var packingStyleRF = $('.cls_tfartwork_pa .cls_input_pa_packing_style_requestform').val();
        //   if (selections[0].ID != -1 && selections[0].ID != undefined) {
        //        packingSytle = selections[0].DISPLAY_TXT;
        //    } else if (defaultPA_PackingStyleData[v] != undefined)
        //    {
        //        packingSytle = selections[0].text;
        //    }

        //    if (packingSytle.indexOf(packingStyleRF) == -1)
        //    {

        //        if (confirm("Please confirm Packing Style different Requet from?")) {
        //             //alert('ok');
        //        }
        //        else {
        //            setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_packing_style', prev_packingstyle_val, prev_packingstyle_display);
        //            return;
        //        }
        //    }

        //}
           // end added by aof r CR#IGRID_REIM_SPRINT2

       

        if (selections.length > 0) {
            if (selections[0].ID != -1 && selections[0].ID != undefined) {
                setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_pack_size', selections[0].PACK_SIZE_VALUE, selections[0].PACK_SIZE_DESCRIPTION);
                $('.cls_tfartwork_pa .cls_lov_pa_pack_size').prop("disabled", true);
                $('.cls_tfartwork_pa .cls_input_pa_packing_style').hide();  // added by aof r CR#IGRID_REIM
                $('.cls_tfartwork_pa .cls_input_pa_pack_size').hide();  // added by aof r CR#IGRID_REIM
                isClear = false;

                prev_packingstyle_val = selections[0].ID;     //  added by aof r CR#IGRID_REIM_SPRINT2
                prev_packingstyle_display = selections[0].DISPLAY_TXT;    //  added by aof r CR#IGRID_REIM_SPRINT2
            }
            else if (defaultPA_PackingStyleData[v] != undefined) {
                setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_pack_size', defaultPA_PackingStyleData[v].PACK_SIZE_ID, defaultPA_PackingStyleData[v].PACK_SIZE_DISPLAY_TXT);
                if (v == -1) {
                    $('.cls_tfartwork_pa .cls_lov_pa_pack_size').prop("disabled", false);
                }
                else {
                    $('.cls_tfartwork_pa .cls_lov_pa_pack_size').prop("disabled", true);
                    $('.cls_tfartwork_pa .cls_input_pa_packing_style').hide();  // added by aof r CR#IGRID_REIM
                    $('.cls_tfartwork_pa .cls_input_pa_pack_size').hide();  // added by aof r CR#IGRID_REIM
                }
                isClear = false;

                prev_packingstyle_val = selections[0].id;  //  added by aof r CR#IGRID_REIM_SPRINT2
                prev_packingstyle_display = selections[0].text;  //  added by aof r CR#IGRID_REIM_SPRINT2
            }
        }

        if (isClear) {
            $('.cls_tfartwork_pa .cls_lov_pa_pack_size').empty();
            $('.cls_tfartwork_pa .cls_lov_pa_pack_size').prop("disabled", false);
            $('.cls_tfartwork_pa .cls_input_pa_packing_style').hide();  // added by aof r CR#IGRID_REIM
            $('.cls_tfartwork_pa .cls_input_pa_pack_size').hide();  // added by aof r CR#IGRID_REIM
                     
        }
    });

    $(".cls_tfartwork_pa .cls_txt_pa_productcode").on('input focus keydown keyup', function () {
        var maxLength = 18;
        var text = $(this).val();
        var lines = text.split(/(\r\n|\n|\r)/gm);
        for (var i = 0; i < lines.length; i++) {
            if (lines[i].length > maxLength) {
                lines[i] = lines[i].substring(0, maxLength);
            }
        }
        $(this).val(lines.join(''));
    });


    //IGRID_REIM #SPLINT2
    $('.cls_tfartwork_pa .cls_lov_pa_type_of').on("change", function () {

        //bindMatDesc_ByMateiralRefMaterialTypeofBrand(); //by aof
        //if ($('.cls_tfartwork_pa .cls_lov_pa_type_of').val() == '' || $('.cls_tfartwork_pa .cls_lov_pa_type_of').val() == null )
        //{
        //    $('.cls_container_taskform_artwork .cls_txt_pa_mat_desc').val('');
        //}

    });
     //IGRID_REIM #SPLINT2
    //$('.cls_tfartwork_pa .cls_lov_pa_material_group').on("change", function () {
    //    var selections = $(this).select2('data');
    //    var v = $(this)[0].value;
    //    var isClear = true;
    //    var matgroup_value = "";
    //    if (selections.length > 0) {
    //        if (selections[0].ID != -1 && selections[0].ID != undefined) {
    //            setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_material_group', selections[0].ID, selections[0].DISPLAY_TXT);
    //            matgroup_value = selections[0].VALUE;
    //            setRequireFieldTaskformArtworkPA(matgroup_value);

    //            isClear = false;
    //        }
    //        else if (defaultPA_MaterialGroupData[v] != undefined) {
    //            setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_material_group', defaultPA_MaterialGroupData[v].ID, defaultPA_MaterialGroupData[v].DISPLAY_TXT);
    //            matgroup_value = defaultPA_MaterialGroupData[v].VALUE;
    //            setRequireFieldTaskformArtworkPA(matgroup_value);
    //            isClear = false;
    //        }
    //    }

    //    if (isClear) {
    //        $('.cls_tfartwork_pa .cls_lov_pa_material_group').empty();
    //        $('.cls_tfartwork_pa .cls_lov_pa_material_group').prop("disabled", false);
    //    }
    //});

    $('.cls_tfartwork_pa .cls_btn_pa_save').click(function (e) {
        saveTaskFormPA(true);
    });

    $('.cls_tfartwork_pa .cls_btn_pa_send_back_workflow').click(function (e) {
        saveTaskFormPA(false);
        $('#modal_tfartwork_pa_sendback').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    //start @* by aof 202306 for CR#IGRID_REIM----add img search primary size"*@

  
    $('.cls_tfartwork_pa .cls_img_search_primary_size').click(function (e) {
        $('.cls_div_igrid_parimary_size_search input[type=text]').val('');
        bindPackingSizePopUp();
        $('#modal_tfartwork_primary_size').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

   
    $("#modal_tfartwork_primary_size .cls_btn_search").click(function (e) {

        
        bindPackingSizePopUp();
     
    });

    $("#modal_tfartwork_primary_size .cls_btn_clear").click(function (e) {

        $('.cls_div_igrid_parimary_size_search input[type=text]').val('');      

    });


    

    $("#modal_tfartwork_primary_size .cls_btn_tfartwork_primary_size_select").click(function (e) {
        var table = $('#table_tfartwork_primary_size').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            //setSelectSuggestMaterial(tblData);
           
            //alertError2(tblData[0].THREE_P_ID);

            if (tblData[0].THREE_P_ID > 0)
            {

                var three_p_id = tblData[0].THREE_P_ID;
                var can_description = tblData[0].DESCRIPTION + ":" + tblData[0].CONTAINERTYPE + ":" + tblData[0].DESCRIPTIONTYPE;
                var running = tblData[0].CODE;
                var defaultPA_PrimarySizeDataItem = {};
                defaultPA_PrimarySizeDataItem[three_p_id] = {
                    CONTAINER_TYPE_ID: -99,
                    CONTAINER_TYPE_DISPLAY_TXT: tblData[0].CONTAINERTYPE,
                    LID_TYPE_ID: -99,
                    LID_TYPE_DISPLAY_TXT: tblData[0].DESCRIPTIONTYPE
                };
                defaultPA_PrimarySizeData = defaultPA_PrimarySizeDataItem;
                setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_primary_size', three_p_id, can_description);
                $(".cls_tfartwork_pa .cls_input_pa_primary_size").hide();
                $(".cls_tfartwork_pa .cls_lov_pa_primary_size").trigger("change");

                $(".cls_tfartwork_pa .cls_input_pa_packing_size_running").val(running);
                $(".cls_tfartwork_pa .cls_lbl_pa_packing_size_running").html(running);
                
                        
                $('#modal_tfartwork_primary_size').modal('hide');
            } else
            {
                alertError2("Please check, Not found THREE_P_ID in the Artwork system.")
            }


        }
        else {
            alertError2("Please select 1 item.");
        }
    });


    $('.cls_tfartwork_pa .cls_lov_pa_container_type').prop("disabled", true);
    $('.cls_tfartwork_pa .cls_lov_pa_lid_type').prop("disabled", true);


    $('.cls_tfartwork_pa .cls_lov_pa_plant_register_no').on("change", function () {
        bindPA_SetData_FFC("change2");
    });

    //end @* by aof 202306 for CR#IGRID_REIM----add img search"*@


    $("#modal_tfartwork_pa_sendback .cls_btn_tfartwork_pa_sendback_submit").click(function (e) {
        debugger;
        var jsonObj = new Object();
        jsonObj.data = {};
        var PROCESS = {};

        PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
        PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
        PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
        //PROCESS["ARTWORK_SUB_ID"] = ArtworkSubId;
        var str_pa_sendback = $('input[name=cls_rdo_pa_sendback]:radio:checked').val();
        if (str_pa_sendback == "0") {
            var usermk = $('#modal_tfartwork_pa_sendback .cls_lov_marketing').val();
            if (usermk != undefined) {
                PROCESS["CURRENT_USER_ID"] = usermk;
            }
            else {
                alertError2("Please select send to.");
                return false;
            }
        }

        PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_BACK_MK').curr_role;
        PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_BACK_MK').curr_step;
        PROCESS["REASON_ID"] = $('#modal_tfartwork_pa_sendback .cls_lov_send_for_reason').val();
        if (PROCESS["REASON_ID"] == DefaultResonId) {
            alertError2("Please select reason for send back");
            return false;
        }
        PROCESS["CREATE_BY"] = UserID;
        PROCESS["UPDATE_BY"] = UserID;
        var editor = new Quill('#modal_tfartwork_pa_sendback .cls_txtedt_tfartwork_pa_sendback_remark');
        PROCESS["REMARK"] = editor.root.innerHTML;

        if (PROCESS["REMARK"] == "<p><br></p>" || PROCESS["REMARK"] == null) {
            alertError2("Please fill comment");
            return false;
        }

        jsonObj.data = PROCESS;

        var myurl = '/api/taskform/pa/sendbackmk';
        var mytype = 'POST';
        var mydata = jsonObj;

        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomesendback);
    });

    $('.cls_tfartwork_pa .cls_btn_pa_request_material').click(function (e) {
        saveTaskFormPA(false);

        var str_changepoint = $('input[name=cls_rdo_haeder_tfartwork_change_point]:radio:checked').val();
        if (str_changepoint == undefined) {
            alertError2("Please select change point.");
        } else if (!confirmPackingStyleDifferentRequestForm())  //  by aof  IGRID_REIM_SPLINT2
        {

        }
        else {
            checkSalesOrderRequestMaterial();
        }
    });
    $('.cls_tfartwork_pa .cls_btn_pa_update_material').click(function (e) {

        if (!confirmPackingStyleDifferentRequestForm()) 
        {
            return;
        }

        saveTaskFormPA(false);
        //DisplayProgressMessage(this);
        var jsonObj = new Object();
        jsonObj.data = {};
        var item = {};

        item["ARTWORK_SUB_ID"] = ArtworkSubId;
        item["RECORD_TYPE"] = "U";
        item["REFERENCE_MATERIAL"] = $('.cls_txt_header_tfartwork_ref_mat').val();;
        item["CHANGE_POINT"] = $('input[name=cls_rdo_haeder_tfartwork_change_point]:radio:checked').val();
        item["ACTION"] = "UPDATE";
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;

        jsonObj.data = item;

        var myurl = '/api/taskform/pa/requestmaterial';
        var mytype = 'POST';
        var mydata = jsonObj;

        myAjaxConfirmSubmit(myurl, mytype, mydata, callback_requestmaterial, '', true, true);
    });

    $('.cls_tfartwork_pa .cls_btn_pa_complete_workflow').click(function (e) {
        var jsonObj = new Object();
        jsonObj.data = {};
        var PROCESS = {};

        PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
        PROCESS["ARTWORK_SUB_ID"] = ArtworkSubId;
        PROCESS["CREATE_BY"] = UserID;
        PROCESS["UPDATE_BY"] = UserID;

        jsonObj.data = PROCESS;

        var myurl = '/api/taskform/pa/complete';
        var mytype = 'POST';
        var mydata = jsonObj;

        myAjaxConfirmSubmit(myurl, mytype, mydata, callback_tf_complete_workflow, '', true, true);
    });

    $('.cls_tfartwork_pa .cls_btn_pa_terminate_workflow').click(function (e) {
        $('#modal_tfartwork_pa_terminate').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $("#modal_tfartwork_pa_terminate .cls_btn_tfartwork_pa_terminate_submit").click(function (e) {
        var jsonObj = new Object();
        jsonObj.data = {};
        var PROCESS = {};

        PROCESS["ARTWORK_SUB_ID"] = ArtworkSubId;
        PROCESS["CREATE_BY"] = UserID;
        PROCESS["UPDATE_BY"] = UserID;
        var editor = new Quill('#modal_tfartwork_pa_terminate .cls_txtedt_tfartwork_pa_terminate_remark');
        PROCESS["REMARK_TERMINATE"] = editor.root.innerHTML;
        PROCESS["TERMINATE_REASON_CODE"] = $(".cls_lov_artwork_reason_terminate").val();

        if (PROCESS["TERMINATE_REASON_CODE"] != DefaultResonId) {
            if ($(".cls_lov_artwork_reason_terminate").select2('data')[0].text.toLowerCase() == "อื่นๆ โปรดระบุ (Others)".toLowerCase() && (PROCESS["REMARK_TERMINATE"] == "<p><br></p>" || PROCESS["REMARK_TERMINATE"] == null)) {
                alertError2("Please fill comment");
                return false;
            }
        }
        else {
            alertError2("Please select reason for terminate");
            return false;
        }

        jsonObj.data = PROCESS;

        var myurl = '/api/taskform/pa/terminate';
        var mytype = 'POST';
        var mydata = jsonObj;

        myAjaxConfirmSubmit(myurl, mytype, mydata, tohometerminate);
    });


    $(".cls_tfartwork_pa .cls_btn_pa_submit").click(function () {
        saveTaskFormPA(false);
        if (typeartwork == "REPEAT")
            $("#pa_submit_modal .cls_req_shade_pa_pop").hide();
        $("#pa_submit_modal").modal({
            backdrop: true,
            show: true,
            keyboard: true
        });

    });

    $(".cls_tfartwork_pa .cls_btn_pa_send_to_customer").click(function () {
        saveTaskFormPA(false);
        $("#send_to_customer_modal").modal({
            backdrop: 'static',
            keyboard: true
        });

    });

    $(".cls_tfartwork_pa .cls_btn_pa_send_to_vendor").click(function () {
        saveTaskFormPA(false);
        checkdielinefiles_artwork();
        $("#send_to_vendor_modal").modal({
            backdrop: 'static',
            keyboard: true
        });

    });

    $(pa_submit_modal + '.cls_chk_send_pg').click(function () {
        if ($(this).prop('checked')) {
            $(pa_submit_modal + '.cls_body_send_pg').show();
        }
        else {
            $(pa_submit_modal + '.cls_body_send_pg').hide();
        }
    });
    $(pa_submit_modal + '.cls_chk_send_mk').click(function () {

        if ($(this).prop('checked')) {
            $(pa_submit_modal + '.cls_body_send_mk').show();
        }
        else {
            $(pa_submit_modal + '.cls_body_send_mk').hide();
        }
    });
    $(pa_submit_modal + '.cls_chk_send_qc').click(function () {
        if ($(this).prop('checked')) {
            $(pa_submit_modal + '.cls_body_send_qc').show();
        }
        else {
            $(pa_submit_modal + '.cls_body_send_qc').hide();
        }
    });
    $(pa_submit_modal + '.cls_chk_send_wh').click(function () {
        if ($(this).prop('checked')) {
            $(pa_submit_modal + '.cls_body_send_wh').show();
        }
        else {
            $(pa_submit_modal + '.cls_body_send_wh').hide();
        }
    });
    $(pa_submit_modal + '.cls_chk_send_plan').click(function () {
        if ($(this).prop('checked')) {
            $(pa_submit_modal + '.cls_body_send_plan').show();
        }
        else {
            $(pa_submit_modal + '.cls_body_send_plan').hide();
        }
    });

    $(pa_submit_modal + '.cls_chk_send_pp').click(function () {
        if ($(this).prop('checked')) {
            $(pa_submit_modal + '.cls_body_send_pp').show();
        }
        else {
            $(pa_submit_modal + '.cls_body_send_pp').hide();
        }
    });

    $(send_to_vendor_modal + '.cls_chk_send_artwork_vendor').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_vendor_modal + '.cls_body_send_artwork_vendor').show();
        }
        else {
            $(send_to_vendor_modal + '.cls_body_send_artwork_vendor').hide();
        }
    });

    $(send_to_vendor_modal + '.cls_chk_send_shade').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_vendor_modal + '.cls_body_send_shade').show();
        }
        else {
            $(send_to_vendor_modal + '.cls_body_send_shade').hide();
        }
    });

    $(send_to_customer_modal + '.cls_chk_send_review_customer').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_send_review_customer').show();
        }
        else {
            $(send_to_customer_modal + '.cls_body_send_review_customer').hide();
        }
    });

    $(send_to_customer_modal + '.cls_chk_formandlabelraw_customer').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_formandlabelraw_customer').show();
            //$(send_to_customer_modal + '.cls_body_adjustment_customer').hide();
        }
        else {
            $(send_to_customer_modal + '.cls_body_formandlabelraw_customer').hide();
        }
    });

    $(send_to_customer_modal + '.cls_chk_qc_changedetails_customer').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_qc_changedetails_customer').show();
            //$(send_to_customer_modal + '.cls_body_noncompliance_customer').hide();
        }
        else {
            $(send_to_customer_modal + '.cls_body_qc_changedetails_customer').hide();
        }
    });

    $(send_to_customer_modal + '.cls_chk_noncompliance_customer').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_noncompliance_customer').show();
            //$(send_to_customer_modal + '.cls_body_qc_changedetails_customer').hide();
        }
        else {
            $(send_to_customer_modal + '.cls_body_noncompliance_customer').hide();
        }
    });

    $(send_to_customer_modal + '.cls_chk_remark_adjustment_customer').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_adjustment_customer').show();
            //$(send_to_customer_modal + '.cls_body_formandlabelraw_customer').hide();
        }
        else {
            $(send_to_customer_modal + '.cls_body_adjustment_customer').hide();
        }
    });

    $(send_to_customer_modal + '.cls_chk_send_req_ref').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_send_req_ref').show();
        }
        else {
            $(send_to_customer_modal + '.cls_body_send_req_ref').hide();
        }
    });

    $(send_to_customer_modal + '.cls_chk_send_approve_print').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_send_approve_print').show();
        }
        else {
            $(send_to_customer_modal + '.cls_body_send_approve_print').hide();
        }
    });

    $(send_to_customer_modal + '.cls_chk_send_approve_shade').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_send_approve_shade').show();
        }
        else {
            $(send_to_customer_modal + '.cls_body_send_approve_shade').hide();
        }
    });


    $(send_to_customer_modal + '.cls_chk_send_shade_ref').click(function () {
        if ($(this).prop('checked')) {
            $(send_to_customer_modal + '.cls_body_send_shade_ref').show();
        }
        else {
            $(send_to_customer_modal + '.cls_body_send_shade_ref').hide();
        }
    });

    $(".cls_tfartwork_pa .cls_btn_pa_clear").click(function (e) {
        $.confirm({
            title: 'Confirm Dialog.',
            content: 'Do you want to clear data?',
            animation: 'none',
            closeAnimation: 'none',
            type: 'red',
            backgroundDismiss: false,
            backgroundDismissAnimation: 'glow',
            buttons: {
                Yes: {
                    text: 'Yes',
                    btnClass: 'btn-danger cls_btn_confirm_ok',
                    action: function () {
                        clearPAData();
                    }
                },
                No: {
                    text: 'No',
                    btnClass: 'btn-default cls_btn_confirm_no',
                    action: function () {
                        //nothing
                    }
                }
            }
        });


    });

    $("#pa_submit_modal form").submit(function (e) {
        if ($(this).valid()) {
            if ($('.cls_header_chk_pa[type="checkbox"]:checked').length > 0) {
                e.preventDefault();
                SubmitDataPop();
            }
            else {
                $(".se-pre-con").fadeOut('fast');
                alertError2("Please select at least 1 item.");
            }
        }
        else if ($('.cls_input_pg_by_pa_other ').is(':visible') && $('.cls_input_pg_by_pa_other ').val() == '') {
            alertError2("Please fill remark reason");
        }
        else if ($('.cls_input_mk_by_pa_other ').is(':visible') && $('.cls_input_mk_by_pa_other ').val() == '') {
            alertError2("Please fill remark reason");
        }
        else if ($('.cls_input_qc_by_pa_other ').is(':visible') && $('.cls_input_qc_by_pa_other ').val() == '') {
            alertError2("Please fill remark reason");
        }
        else if ($('.cls_input_wh_by_pa_other ').is(':visible') && $('.cls_input_wh_by_pa_other ').val() == '') {
            alertError2("Please fill remark reason");
        }
        else if ($('.cls_input_pn_by_pa_other  ').is(':visible') && $('.cls_input_pn_by_pa_other  ').val() == '') {
            alertError2("Please fill remark reason");
        }
        else if ($('.cls_input_pp_by_pa_other ').is(':visible') && $('.cls_input_pp_by_pa_other ').val() == '') {
            alertError2("Please fill remark reason");
        }
        //$(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $("#send_to_vendor_modal .cls_btn_submit_vendor_modal").click(function (e) {
        if ($('.cls_header_chk_vendor[type="checkbox"]:checked').length > 0) {
            if ($('.cls_input_vn_sl_by_pa_other ').is(':visible') && $('.cls_input_vn_sl_by_pa_other ').val() == '') {
                alertError2("Please fill remark reason");
            }
            else if ($('.cls_input_vn_pm_by_pa_other ').is(':visible') && $('.cls_input_vn_pm_by_pa_other ').val() == '') {
                alertError2("Please fill remark reason");
            }
            else {
                SubmitDataPopSendtovendor();
            }
        }
        else {
            $(".se-pre-con").fadeOut('fast');
            alertError2("Please select at least 1 item.");
        }
    });

    $("#send_to_customer_modal form").submit(function (e) {
        if ($(this).valid()) {
            if ($('.cls_header_chk_customer[type="checkbox"]:checked').length > 0) {
                SubmitDataPopSendtocustomer();
            }
            else {
                $(".se-pre-con").fadeOut('fast');
                alertError2("Please select at least 1 item.");
            }
            e.preventDefault();
        }
        $(this).removeAttr("novalidate");
    });


    $(".cls_tfartwork_pa .cls_btn_copy_pa_data").click(function () {
        if ($(".cls_tfartwork_pa .cls_lov_pa_copy_artworkno").val() == null) {
            alertError2('Please selected Artwork No.');
        }
        else {

            //alertError2($(".cls_tfartwork_pa .cls_lov_pa_copy_artworkno").val());

            var jsonObj = new Object();
            jsonObj.data = {};
            var item = {};

            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ARTWORK_ITEM_ID_COPY"] = $(".cls_tfartwork_pa .cls_lov_pa_copy_artworkno").val();
            item["UPDATE_BY"] = UserID;

            jsonObj.data = item;

            var myurl = '/api/taskform/pa/copypadata';
            var mytype = 'POST';
            var mydata = jsonObj;

            var msg = "";
            msg = 'Do you want to copy PA Data ?'; //from Artwork no : ' + $(".cls_tfartwork_pa .cls_lov_pa_copy_artworkno").text();

            //myAjaxConfirmSubmit(myurl, mytype, mydata, callback_copypadata, '', true, true);
            myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback_copypadata, '', true, true, msg);

        }

    });

    function callback_copypadata(res) {
        if (res.status == "S") {

            var copy_aw_no = $(".cls_tfartwork_pa .cls_lov_pa_copy_artworkno").val();

            clearPAData();

            $(".cls_tfartwork_pa .cls_lov_pa_copy_artworkno").val(copy_aw_no).trigger("change");

            bindDataTaskFormPA();
        }
    }


    $(".cls_tfartwork_pa .cls_btn_add_pa_productcode").click(function () {
        event_val = "change";
        typecheck_pa_vap = "change";
        if ($(".cls_tfartwork_pa .cls_lov_pa_product_code").val() == null && $(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no").val() == null) {
            alertError2('Please selected Product code/RD Reference no. before add multiple product code.');
        }else if ($(".cls_txt_pa_productcode").val()!="") {
            var productCodes = $(".cls_txt_pa_productcode").val().split(/\n/);

            var strAlert = "";
            var jsonObj = new Object();
            jsonObj.data = [];
            for (var i = 0; i < productCodes.length; i++) {
                productCodes[i] = productCodes[i].trim();
                if (productCodes[i] != "") {
                    var isDup = false;
                    $('.table_pa_productcode_item > tbody > tr').each(function () {
                        if (productCodes[i].toLowerCase() == $(this).find('.cls_txt_pa_productcode_item_productcode').val().toLowerCase()) {
                            isDup = true;
                            strAlert = strAlert + productCodes[i] + " was already" + "<br>";
                            return false;
                        }
                    });
                    if (!isDup) {
                        var item = {};
                        item["PRODUCT_CODE"] = productCodes[i];
                        var text = "";
                        var first = true;
                        $(".cls_artwork_request_form .cls_lov_artwork_plant_multiple_static").each(function (index) {
                            if (first) first = false;
                            else {
                                if (!isEmpty($(this).val())) {
                                    text += $(this).val() + ";";
                                }
                            }
                        });
                        var artwork_request_type = $('.cls_artwork_request_form .cls_input_artwork_request_type').val();
                        item["PRODUCTION_PLANT"] = text;     
                        item["PRODUCT_TYPE"] = artwork_request_type; 
                        jsonObj.data.push(item);
                    }
                }
            }
            if (jsonObj.data.length > 0) {
                getPA_ProductCode(jsonObj);
            }

            if (strAlert.length > 0) {
                alertError2(strAlert);
            }
        }else if ($(".cls_tfartwork_pa .cls_lov_pa_product_code").val() != null) {
            var selections = $(".cls_tfartwork_pa .cls_lov_pa_product_code").select2('data');
            bindPA_Net_Weight_DrainedWeight(selections, true,"add");
        }
        else if ($(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no").val() != null) {
            var selections = $(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no").select2('data');
            bindPA_Net_Weight_DrainedWeight(selections, false,"add");
        }
    });
    //change by voravut on 2020-08-05
    //$('.cls_tfartwork_pa .cls_lov_pa_rd_reference_no').on("change", function () {
    //    var selections = $(this).select2('data');
    //    bindPA_Net_Weight_DrainedWeight(selections, false);
    //    bindPA_SetData_FFC();
    //});

    //$('.cls_tfartwork_pa .cls_lov_pa_product_code').on("change", function () {
    //    var selections = $(this).select2('data');
    //    bindPA_Net_Weight_DrainedWeight(selections, true);
    //    bindPA_SetData_FFC();
    //});

    $('.cls_tfartwork_pa .cls_input_artwork_request_id').val(ARTWORK_REQUEST_ID);
    if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PA' || CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_MK_VERIFY') {
        bindDataCustomerPopup();
    }
    setRequireFieldTaskformArtworkPA('');
    bindDataTaskFormPA();
});

function checkSalesOrderRequestMaterial() {
    var str_suggest_mat = $('.cls_txt_header_tfartwork_ref_mat').val();

    if (Bom_sales != "" && Bom_sales != null && str_suggest_mat == "" && typeartwork == "NEW") {
        //  myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback_requestmaterial, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please suggest material before request material master from iGrid! (' + Bom_sales + ')</span>');
        dialogConfirmForcheckSalesOrderRequestMaterial('Do you want to submit ?</br > <span style="color:red;font-style: italic;">**Please suggest material before request material master from iGrid! (' + Bom_sales + ')</span>');
    }
    else if (Bom_sales == "") {
        // myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback_requestmaterial, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please assign sales order before request material master from iGrid!</span>');
        dialogConfirmForcheckSalesOrderRequestMaterial('Do you want to submit?</br><span style="color:red;font-style: italic;">**Please assign sales order before request material master from iGrid!</span>');
    }
    else {
        //myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback_requestmaterial, '', true, true, 'Do you want to submit?');
        dialogConfirmForcheckSalesOrderRequestMaterial('Do you want to submit?');
    }

}

function dialogConfirmForcheckSalesOrderRequestMaterial(msg) {
    $.confirm({
        title: 'Confirm Dialog',
        content: msg,
        animation: 'none',
        closeAnimation: 'none',
        type: 'blue',
        backgroundDismiss: false,
        backgroundDismissAnimation: 'glow',
        buttons: {
            Yes: {
                text: 'Yes',
                btnClass: 'btn-primary cls_btn_confirm_ok',
                action: function () {
                    //DisplayProgressMessage(true);
                    CheckBrandRefMaterial_RequestForm();
                }
            },
            No: {
                text: 'No',
                btnClass: 'btn-default cls_btn_confirm_no',
                action: function () {

                }
            }
        }
    });
}

function CheckBrandRefMaterial_RequestForm() {
    var myurl = '/api/taskform/pa/validatebrandrefmatwithrequestform?data.ARTWORK_SUB_ID=' + ArtworkSubId;
    var mytype = 'GET';
    var mydata = null;
    myAjaxNoAlertSuccess(myurl, mytype, mydata, callback_CheckBrandRefMaterial_RequestForm);
}

function callback_CheckBrandRefMaterial_RequestForm(res) {

    var str_changepoint = $('input[name=cls_rdo_haeder_tfartwork_change_point]:radio:checked').val();
    var str_suggest_mat = $('.cls_txt_header_tfartwork_ref_mat').val();
    var jsonObj = new Object();
    jsonObj.data = {};
    var item = {};

    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["RECORD_TYPE"] = "I";
    item["REFERENCE_MATERIAL"] = $('.cls_txt_header_tfartwork_ref_mat').val();
    item["CHANGE_POINT"] = str_changepoint;
    item["ACTION"] = "REQUEST";
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;

    var myurl = '/api/taskform/pa/requestmaterial';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (res.status == "S" && (res.msg != null && res.msg != '')) {

        $.confirm({
            title: res.msg,
            content: 'Do you want to edit?',
            animation: 'none',
            closeAnimation: 'none',
            type: 'red',
            backgroundDismiss: false,
            backgroundDismissAnimation: 'glow',
            buttons: {
                Yes: {
                    text: 'Yes',
                    btnClass: 'btn-primary cls_btn_confirm_ok',
                    action: function () {

                    }
                },
                No: {
                    text: 'No',
                    btnClass: 'btn-default cls_btn_confirm_no',
                    action: function () {

                        myAjax(myurl, mytype, mydata, callback_requestmaterial, true, true);
                       
                    }
                }
            }
        });
    }
    else {
        myAjax(myurl, mytype, mydata, callback_requestmaterial, true, true);
       //myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback_requestmaterial, '', true, true, 'Do you want to submit?');
    }
}

//---------------------------------------------------ticket# 473360 by aof commment and rewrite----------------------------------------------------------
// change function copydielinefiles_artwork to checkdielinefiles_artwork
//function copydielinefiles_artwork() {
//    var jsonObj = new Object();
//    jsonObj.data = {};

//    jsonObj.data["ARTWORK_SUB_ID"] = ArtworkSubId;
//    jsonObj.data["CREATE_BY"] = UserID;
//    jsonObj.data["UPDATE_BY"] = UserID;

//    var myurl = '/api/taskform/internal/pg/copydielinefiles';
//    var mytype = 'POST';
//    var mydata = jsonObj;
//    myAjax(myurl, mytype, mydata, callbackCopydielinefiles_artwork, '', true, false);
//}

//function callbackCopydielinefiles_artwork(res) {
//    if (res.msg.length > 0) {
//        $('.cls_row_txt_copydieline').show();
//        $('.cls_lbl_copydieline').html(res.msg);
//    }
//} 

function checkdielinefiles_artwork() {
    var jsonObj = new Object();
    jsonObj.data = {};

    jsonObj.data["ARTWORK_SUB_ID"] = ArtworkSubId;
    jsonObj.data["CREATE_BY"] = UserID;
    jsonObj.data["UPDATE_BY"] = UserID;

    var myurl = '/api/taskform/internal/pg/checkdielinefiles';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callbackCheckdielinefiles_artwork, '', true, false);
}

function callbackCheckdielinefiles_artwork(res) {
    if (res.msg.length > 0) {
        $('.cls_row_txt_copydieline').show();
        $('.cls_lbl_copydieline').html(res.msg);
    }
} 
//---------------------------------------------------ticket# 473360 by aof commment and rewrite----------------------------------------------------------


function getPA_ProductType() {
    var product_type = "";
    var arrList = [];
    $('.table_pa_productcode_item > tbody > tr').each(function () {
        if ($(this).find('.cls_txt_pa_productcode_item_productcode').val().toLowerCase() != "") {
            if ($(this).find('.cls_chk_pa_productcode_item_vap').is(':checked')) {
                product_type = "VAP";
            } else if ($(this).find('.cls_chk_pa_productcode_item_ffc').is(':checked'))
                product_type = "FFC";
            else
                product_type = "X";

            var in_array = arrList.filter(function (item) {
                return item.ID == product_type
            });
            var index = arrList.indexOf(in_array[0]);
            Item = {};
            if (index == -1) {
                Item["ID"] = product_type;
                arrList.push(Item);
            }
        }
    });
    $('.table_pa_productcode_item2 > tbody > tr').each(function () {
        if ($(this).find('.cls_txt_pa_productcode_item_productcode2').val().toLowerCase() != "") {
            if ($(this).find('.cls_chk_pa_productcode_item_vap2').is(':checked')) {
                product_type = "VAP";
            } else if ($(this).find('.cls_chk_pa_productcode_item_ffc2').is(':checked'))
                product_type = "FFC";
            else
                product_type = "X";

            var in_array = arrList.filter(function (item) {
                return item.ID == product_type
            });
            var index = arrList.indexOf(in_array[0]);
            Item = {};
            if (index == -1) {
                Item["ID"] = product_type;
                arrList.push(Item);
            }
        }
    });
    return arrList;
    //return arrList.length == 1 ?product_type:"X";
}
function getPA_checkData_vap(arrList) {
    if (check_vap_type.length != arrList.length) {
        if (check_vap_type.length > 0)
            check_msg = "Product is changed. Please check plant register No. and company name/address.";
        check_vap_type = arrList;
    }
}
function getPA_MatGroupValue(obj) {
    var selections = $(obj).select2('data');
    var v = $(obj)[0].value;
    var matgroup_id = 0;
    var matgroup_value = "";
    if (selections.length > 0) {
        if (selections[0].ID != -1 && selections[0].ID != undefined) {
            setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_material_group', selections[0].ID, selections[0].DISPLAY_TXT);
            matgroup_value = selections[0].VALUE;
            setRequireFieldTaskformArtworkPA(matgroup_value);
            matgroup_id = selections[0].ID;
        }
        else if (defaultPA_MaterialGroupData[v] != undefined) {
            setValueToDDL('.cls_tfartwork_pa .cls_lov_pa_material_group', defaultPA_MaterialGroupData[v].ID, defaultPA_MaterialGroupData[v].DISPLAY_TXT);
            matgroup_value = defaultPA_MaterialGroupData[v].VALUE;
            setRequireFieldTaskformArtworkPA(matgroup_value);
            matgroup_id = defaultPA_MaterialGroupData[v].ID;
        }
    }

    $(".cls_tfartwork_pa .cls_lov_pa_type_of").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_type_of").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_type_of_two").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_type_of_two").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_style_of_printing").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_style_of_printing").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_pms_colour").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_pms_colour").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_process_colour").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_process_colour").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_total_colour").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_total_colour").val('');

    var myurl = '/api/taskform/pa/changematerialgroup?data.MATERIAL_GROUP_ID=' + matgroup_id + '&data.artwork_sub_id=' + ArtworkSubId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata);
}

function setRequireFieldTaskformArtworkPA(matgroup_value) {
    $('.cls_tfartwork_pa .cls_lbl_pa_style_of_printing').html("Style of printing :");
    $('.cls_tfartwork_pa .cls_lbl_pa_direction_of_sticker').html("Direction of sticker :");
    $('.cls_tfartwork_pa .cls_lbl_pa_customer_design').html("Customer's design :");
    $('.cls_tfartwork_pa .cls_lbl_pa_packing_style').html("Packing style :");
    $('.cls_tfartwork_pa .cls_lbl_pa_pack_size').html("Pack size :");

    $('.cls_tfartwork_pa .cls_lov_pa_direction_of_sticker').prop("disabled", true);   //by aof IGRID_REIM_PA
    

    var lbl_spanStar = "<span style=\"color: red;\">*</span>";
    switch (matgroup_value) {
        case "K":
            $('.cls_tfartwork_pa .cls_lbl_pa_style_of_printing').html("Style of printing " + lbl_spanStar + ":");
            $('.cls_tfartwork_pa .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            break;
        case "L":
            $('.cls_tfartwork_pa .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style     
            break;
        case "P":
            $('.cls_tfartwork_pa .cls_lbl_pa_style_of_printing').html("Style of printing " + lbl_spanStar + ":");
            $('.cls_tfartwork_pa .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style     
            break;
        case "J":
            $('.cls_tfartwork_pa .cls_lbl_pa_style_of_printing').html("Style of printing " + lbl_spanStar + ":");
            $('.cls_tfartwork_pa .cls_lbl_pa_direction_of_sticker').html("Direction of sticker " + lbl_spanStar + ":");
            $('.cls_tfartwork_pa .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style     
            //$('.cls_tfartwork_pa .cls_lbl_pa_customer_design').html("Customer's design " + lbl_spanStar + ":");
            $('.cls_tfartwork_pa .cls_lov_pa_direction_of_sticker').prop("disabled", false);  //by aof IGRID_REIM_PA
            break;
        default:
            $('.cls_tfartwork_pa .cls_lbl_pa_packing_style').html("Packing style " + lbl_spanStar + ":");
            $('.cls_tfartwork_pa .cls_lbl_pa_pack_size').html("Pack size " + lbl_spanStar + ":");
            $('.cls_tfartwork_pa .cls_lbl_pa_style_of_printing').html("Style of printing " + lbl_spanStar + ":");
            $('.cls_tfartwork_pa .cls_lov_pa_packing_style').prop("disabled", false);   // by aof to disable packing style   
            break;
    }
}

function getPA_ProductCode(jsonObj) {
    var myurl = '/api/artwork/product';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callback_set_pa_productcode);
}

function callback_set_pa_productcode(res) {
    bindPA_ProductCode(res.data);
    var productCodes = $(".cls_txt_pa_productcode").val().split(/\n/);
    var item = [];
    for (var i = 0; i < productCodes.length; i++) {
        if (productCodes[i] != "") {
            var text = "";
            var first = true;
            $(".cls_artwork_request_form .cls_lov_artwork_plant_multiple_static").each(function (index) {
                if (first) first = false;
                else {
                    if (!isEmpty($(this).val())) {
                        text += $(this).val() + ";";
                    }
                }
            });
               
            var itemProduct = {
                "PRODUCT_CODE": productCodes[i],
                "PRODUCTION_PLANT" : text
            }
            item.push(itemProduct);
        }
    }
    bindPA_ProductCodeOther(item);
    $(".cls_txt_pa_productcode").val('');

}
function getplantlist() {
    var plantList = [];
    first = true;
    $(".cls_artwork_request_form .cls_lov_artwork_plant_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                plantItem = {};
                plantItem["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
                plantItem["PRODUCTION_PLANT_ID"] = $(this).val();
                plantItem["CREATE_BY"] = UserID;
                plantItem["UPDATE_BY"] = UserID;
                plantList.push(plantItem);
            }
        }
    });
    return plantList;
}

function bindPA_ProductCode(data) {
    for (var i = 0; i < data.length; i++) {
        var v = data[i];
        var isDup = false;
        if (v.PRODUCT_CODE != null) {

            $('.table_pa_productcode_item > tbody > tr').each(function () {
                if (v.PRODUCT_CODE.toLowerCase() == $(this).find('.cls_txt_pa_productcode_item_productcode').val().toLowerCase()) {
                    isDup = true;
                }
            });
            if (!isDup) {
                var obj = $('.tr_pa_productcode_item').clone().removeClass('tr_pa_productcode_item').removeClass('cls_cn_hide');

                $('.cls_tfartwork_pa .table_pa_productcode_item tbody').append(obj);
                var obj2 = $('.cls_tfartwork_pa .table_pa_productcode_item tbody tr:last');

                $(".cls_tfartwork_pa .cls_img_delete_pa_productcode_item").click(function () {
                    //var value = obj.find('.cls_txt_pa_productcode_item_productcode').val();
                    $(this).closest('tr').remove();
                    bindPA_SetData_FFC("change");
                    //var newArray = [];
                    //for (var i = 0; i < data_product_code.length; i++) {
                    //    if (data_product_code[i][0] != value) {
                    //        newArray.push(data_product_code[i]);
                    //    }
                    //}
                    //data_product_code = newArray;
                });
                $(".cls_tfartwork_pa .cls_chk_pa_productcode_item_vap").click(function () {
                    if ($(this).is(':checked')) {
                        input = 'cls_chk_pa_productcode_item_ffc';
                        obj.find('.' + input).prop('checked', false);
                    }
                    bindPA_SetData_FFC("change");
                });
                $(".cls_tfartwork_pa .cls_chk_pa_productcode_item_ffc").click(function () {
                    if ($(this).is(':checked')) {
                        input = 'cls_chk_pa_productcode_item_vap';
                        obj.find('.' + input).prop('checked', false);
                    }
                    bindPA_SetData_FFC("change");
                });
                input = 'cls_hid_pa_productcode_id';
                obj.find('.' + input).val(v.PRODUCT_CODE_ID);

                input = 'cls_txt_pa_productcode_item_productcode';
                obj.find('.' + input).val(v.PRODUCT_CODE);

                input = 'cls_txt_pa_productcode_item_productdescription';
                obj.find('.' + input).val(v.PRODUCT_DESCRIPTION);

                input = 'cls_txt_pa_productcode_item_net_weight';
                obj.find('.' + input).val(v.NET_WEIGHT);
                input = 'cls_txt_pa_productcode_item_drain_weight';
                obj.find('.' + input).val(v.DRAINED_WEIGHT);
                input = 'cls_txt_pa_ref_request_id';
                obj.find('.' + input).val(v.RF_PRODUCT_RD_REF_ID);

                input = 'cls_chk_pa_productcode_item_vap';
                var plantList = [];
                plantList = getplantlist();
                var in_array = plantList.filter(function (item) {
                    return item.PRODUCTION_PLANT_ID == 3
                });
                var artwork_request_type = $('.cls_artwork_request_form .cls_input_artwork_request_type').val();
                var index = plantList.indexOf(in_array[0]);
                var RF_PRODUCT_RD_REF_ID = v.RF_PRODUCT_RD_REF_ID == "" ? 0 : parseInt(v.RF_PRODUCT_RD_REF_ID);
                var DATA_FROM_DB = v.DATA_FROM_DB == "" ? 0 : parseInt(v.DATA_FROM_DB);
                
                if (Number.isInteger(RF_PRODUCT_RD_REF_ID) == true) {
                    RF_PRODUCT_RD_REF_ID = parseInt(RF_PRODUCT_RD_REF_ID);
                }
                if (Number.isInteger(DATA_FROM_DB) == true) {
                    DATA_FROM_DB = parseInt(DATA_FROM_DB);
                }
                if (RF_PRODUCT_RD_REF_ID > 0) {//data from Request form + save in DB
                    if (v.PRODUCT_TYPE == "VAP")
                        obj.find('.' + input).prop('checked', true);
                    else
                        obj.find('.' + input).prop('checked', false);
                    obj.find('.' + input).prop("disabled", true);
                } else if (DATA_FROM_DB > 0) { //save in DB---------------
                    if (v.PRODUCT_TYPE == "NON") {
                        obj.find('.' + input).prop('checked', false);
                        obj.find('.' + input).prop("disabled", true);
                    }
                    else if (v.PRODUCT_TYPE == "VAP_FIX" && index >= 0) {
                        obj.find('.' + input).prop('checked', true);
                        obj.find('.' + input).prop("disabled", true);
                    }
                    else {//group product vap
                        if (v.PRODUCT_TYPE == "VAP")
                            obj.find('.' + input).prop('checked', true);
                        else
                            obj.find('.' + input).prop('checked', false);
                        //------------------------------------------------------//
                        if (plantList.length == 1 && index >= 0)
                            obj.find('.' + input).prop("disabled", true);
                        else if (plantList.length > 1 && index >= 0)
                            obj.find('.' + input).prop("disabled", false);
                        else if (index = -1)
                            obj.find('.' + input).prop("disabled", true);
                    }
                    //----------------------------------------------------
                } else if (plantList.length > 0 && index == 0 && artwork_request_type == "FFC") {
                    obj.find('.' + input).prop("disabled", true);
                } else if (plantList.length > 1 && v.PRODUCT_TYPE == "VAP" && index >= 0) {
                    obj.find('.' + input).prop('checked', false);
                    obj.find('.' + input).prop("disabled", false);
                } else if (v.PRODUCT_TYPE == "VAP" && index == -1 ) {
                    obj.find('.' + input).prop('checked', false);
                    obj.find('.' + input).prop("disabled", true);
                } else if (plantList.length == 1 && v.PRODUCT_TYPE == "VAP" && index >= 0) {
                    obj.find('.' + input).prop('checked', true);
                    obj.find('.' + input).prop("disabled", true);
                } else if (v.PRODUCT_TYPE == "VAP_FIX" && index >= 0) {
                    obj.find('.' + input).prop('checked', true);
                    obj.find('.' + input).prop("disabled", true);
                } else if (artwork_request_type != "FFC" && v.PRODUCT_TYPE == "" && index >= 0) {
                    obj.find('.' + input).prop("disabled", true);
                } else if (artwork_request_type != "FFC" && v.PRODUCT_TYPE == "" && index >= 0 && plantList.length == 1) {
                    obj.find('.' + input).prop("disabled", true);
                } else if (artwork_request_type != "FFC" && v.PRODUCT_TYPE == "" && index >= 0 && plantList.length > 1) {
                    obj.find('.' + input).prop("disabled", false);
                } else if (plantList.length > 0 && index == -1) {
                    obj.find('.' + input).prop("disabled", true);
                }else{
                    obj.find('.' + input).prop("disabled", true);
                //} else if (Number.isInteger(v.PRODUCT_CODE_ID)) {
                //    obj.find('.' + input).prop("disabled", true);
                }

                input = 'cls_chk_pa_productcode_item_ffc';
                if (v.PRODUCT_TYPE == "FFC")
                    obj.find('.' + input).prop('checked', true);
                if (artwork_request_type == "FFC")
                    obj.find('.' + input).prop('checked', true);
                obj.find('.' + input).prop("disabled", true);
                if (v.RF_PRODUCT_RD_REF_ID == undefined && v.PRODUCT_TYPE =="VAP") {
                    if (obj.find('.cls_chk_pa_productcode_item_vap').is(':checked') == false
                        && obj.find('.cls_chk_pa_productcode_item_ffc').is(':checked') == false
                        && plantList.length > 1 && index >= 0 && typecheck_pa_vap == "change") {
                        alertError2("If your product is a VAP, Please select the VAP product flag. Default product flag function is not supported multiple production plants.");
                    }
                }
                if (!isItemInProductCode(data_product_code, v.PRODUCT_CODE)) {
                    data_product_code.push([v.PRODUCT_CODE, v.PRODUCT_DESCRIPTION, v.NET_WEIGHT, v.DRAINED_WEIGHT]);
                }
                bindPA_SetData_FFC(event_val);
                //change by voravut on 2020-08-05
                //if (v.PRODUCT_CODE == $(".cls_tfartwork_pa .cls_lov_pa_product_code option:selected").text() || v.PRODUCT_CODE == $(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no option:selected").text() || CURRENT_STEP_CODE_DISPLAY_TXT != 'SEND_PA') {
                //    input = 'cls_img_delete_pa_productcode_item';
                //    obj.find('.' + input).hide();
                //}
            }
        }
    }
    //typecheck_pa_vap = "";
}


function getStrProductCode() {
    var str_product_code = "";
    $('.table_pa_productcode_item > tbody > tr').each(function () {
        if ($(this).find('.cls_txt_pa_productcode_item_productcode').val().toLowerCase() != "") {
            if (str_product_code != "") str_product_code += "@";                 
            str_product_code += $(this).find('.cls_txt_pa_productcode_item_productcode').val().trim().replace(/[&\/\\#,+()$~%.'":*?<>{}]/g, '_');

            //string = string.replace(/[&\/\\#,+()$~%.'":*?<>{}]/g, '_');
            
        }
    });


    $('.table_pa_productcode_item2 > tbody > tr').each(function () {
        if ($(this).find('.cls_txt_pa_productcode_item_productcode2').val().toLowerCase() != "") {
            if (str_product_code != "") str_product_code += "@";
            str_product_code += $(this).find('.cls_txt_pa_productcode_item_productcode2').val().trim().replace(/[&\/\\#,+()$~%.'":*?<>{}]/g, '_');
        }
    });


    return str_product_code;
    //return arrList.length == 1 ?product_type:"X";
}

function bindPA_SetData_FFC(value) {

    var arrList = [];
    arrList = getPA_ProductType();
    if (value == "") {
        check_vap_type = [];
        check_vap_type = arrList;
    }
    if (value == "change")
        getPA_checkData_vap(arrList);

    var PA_ProductType = arrList.length == 1 ? arrList[0].ID : "X";
    showalertErrorMsg(value);
     
    //start by aof 202306 for CR#IGRID_REIM_PA
    var STR_PRODUCT_CODE = getStrProductCode(); // $('.cls_tfartwork_pa .cls_lov_pa_product_code').val();
    var REGISTER_ID = $('.cls_tfartwork_pa .cls_lov_pa_plant_register_no').val();


    if (REGISTER_ID == null) {
        REGISTER_ID = 0;
    }

    var param = 'data.PRODUCT_TYPE=' + PA_ProductType + '&data.STR_PRODUCT_CODE=' + STR_PRODUCT_CODE + '&data.REGISTER_CHARACTERISTIC_ID=' + REGISTER_ID;
     //end by aof 202306 for CR#IGRID_REIM_PA

    if ($('.cls_tfartwork_pa .cls_lov_pa_plant_register_no').val() != null && value !="") {
        getPA_plantregisteredno(PA_ProductType,param);
    } 
    if ($('.cls_tfartwork_pa .cls_lov_pa_company_name').val() != null && value != "") {
        getPA_companyaddress(PA_ProductType,param);
    }
    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_plant_register_no', '/api/lov/pa/plantregisteredno?data.PRODUCT_TYPE=' + PA_ProductType, 'data.DISPLAY_TXT', '.cls_input_pa_plant_register_no');
    //bind_lov('.cls_tfartwork_pa .cls_lov_pa_company_name', '/api/lov/pa/companyaddress?data.PRODUCT_TYPE=' + PA_ProductType, 'data.DISPLAY_TXT', '.cls_input_pa_company_name');
       
    if (value != "change2")
    {
        bind_lov('.cls_tfartwork_pa .cls_lov_pa_plant_register_no', '/api/lov/pa/plantregisteredno?' + param, 'data.DISPLAY_TXT'); //, '.cls_input_pa_plant_register_no');  exlude other
    }
     bind_lov('.cls_tfartwork_pa .cls_lov_pa_company_name', '/api/lov/pa/companyaddress?' + param, 'data.DISPLAY_TXT'); // , '.cls_input_pa_company_name');  exlude other
    //$('.cls_txt_product_type').val(PA_ProductType);
    //$('.cls_txt_str_product_code').val(STR_PRODUCT_CODE);
    //$('.cls_txt_register_id').val(REGISTER_ID);
    //bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_plant_register_no', '/api/lov/pa/plantregisteredno', 'data.DISPLAY_TXT', ['PRODUCT_TYPE', 'STR_PRODUCT_CODE', 'REGISTER_CHARACTERISTIC_ID'], ['.cls_txt_product_type', '.cls_txt_str_product_code', '.cls_txt_register_id'], '.cls_input_pa_plant_register_no');   //by aof 202306 for CR#IGRID_REIM_PA
    //bind_lov_param('.cls_tfartwork_pa .cls_lov_pa_company_name', '/api/lov/pa/companyaddress', 'data.DISPLAY_TXT', ['PRODUCT_TYPE', 'STR_PRODUCT_CODE', 'REGISTER_CHARACTERISTIC_ID'], ['.cls_txt_product_type', '.cls_txt_str_product_code', '.cls_txt_register_id'], '.cls_input_pa_company_name');   //by aof 202306 for CR#IGRID_REIM_PA
} 
function showalertErrorMsg(value) {
    if (($('.cls_tfartwork_pa .cls_lov_pa_plant_register_no').val() != null || $('.cls_tfartwork_pa .cls_lov_pa_company_name').val() != null) && value != "")
    if (check_msg!="") {
        alertError2(check_msg);
        check_msg = "";
    }
}
function getPA_plantregisteredno(PA_ProductType,param) {
    //var myurl = '/api/lov/pa/plantregisteredno?data.PRODUCT_TYPE=' + PA_ProductType;    //by aof 202306 for CR#IGRID_REIM_PA add param
    var myurl = '/api/lov/pa/plantregisteredno?'+param;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_getPA_plantregisteredno);
}

function callback_getPA_plantregisteredno(res) {
    var form_name = '.cls_tfartwork_pa ';
    if (res.data.length > 0) {
        for (var i = 0; i < res.data.length; i++) {
            var v = res.data[i];
            var test = v.CHARACTERISTIC_ID;
            var p = $(form_name + '.cls_lov_pa_plant_register_no').val();
            if (v.CHARACTERISTIC_ID == p)
                return; 
        }
        check_msg = "";
        var selections = $(form_name + '.cls_lov_pa_plant_register_no').select2('data');
        $(form_name + '.cls_lov_pa_plant_register_no').val('');
        if (selections.length > 0) {
            var t = selections[0].text;
            if (t != "")
                alertError2("Old plant register No. :" + t);
            $(form_name + '.cls_lov_pa_plant_register_no').text('');
        }
    }
}
function getPA_companyaddress(PA_ProductType,param) {
   // var myurl = '/api/lov/pa/companyaddress?data.PRODUCT_TYPE=' + PA_ProductType;  //by aof 202306 for CR#IGRID_REIM_PA add param
    var myurl = '/api/lov/pa/companyaddress?'+param;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_getPA_companyaddress);
}

function callback_getPA_companyaddress(res) {
    if (res.data.length > 0) {
        for (var i = 0; i < res.data.length; i++) {
            var v = res.data[i];
            var test = v.CHARACTERISTIC_ID;
            var p = $('.cls_tfartwork_pa .cls_lov_pa_company_name').val();
            if (v.CHARACTERISTIC_ID == p)
                return;
        }
        check_msg = "";
        var selections = $('.cls_tfartwork_pa .cls_lov_pa_company_name').select2('data');
        $('.cls_tfartwork_pa .cls_lov_pa_company_name').val('');
        if (selections.length > 0) {
            var t = selections[0].text;
            if (t != "")
                alertError2("Old company name / address :" + t);
            $('.cls_tfartwork_pa .cls_lov_pa_company_name').text('');
        }
    }
}
function isItemInProductCode(array, item) {
    for (var i = 0; i < array.length; i++) {
        // This if statement depends on the format of your array
        if (array[i][0] == item) {
            return true;   // Found it
        }
    }
    return false;   // Not found
}


function bindPA_ProductCodeOther(data) {
    for (var i = 0; i < data.length; i++) {
        var v = data[i];
        if (v.PRODUCT_CODE != null) {
            var isExist = false;
            $('.table_pa_productcode_item > tbody > tr').each(function () {
                if (v.PRODUCT_CODE.toLowerCase() == $(this).find('.cls_txt_pa_productcode_item_productcode').val().toLowerCase()) {
                    isExist = true;
                }
            });

            if (!isExist) {
                $('.table_pa_productcode_item > tbody > tr').each(function () {
                    if (v.PRODUCT_CODE.toLowerCase() == $(this).find('.cls_txt_pa_productcode_item_productcode').val().toLowerCase()) {
                        isExist = true;
                    }
                });

                if (!isExist) {
                    var obj = $('.tr_pa_productcode_item2').clone().removeClass('tr_pa_productcode_item2').removeClass('cls_cn_hide');

                    $('.cls_tfartwork_pa .table_pa_productcode_item2 tbody').append(obj);
                    var obj2 = $('.cls_tfartwork_pa .table_pa_productcode_item2 tbody tr:last');

                    $(".cls_tfartwork_pa .cls_img_delete_pa_productcode_item2").click(function () {
                        $(this).closest('tr').remove();
                        bindPA_SetData_FFC("change");
                    });
                    $(".cls_tfartwork_pa .cls_chk_pa_productcode_item_vap2").click(function () {
                        if ($(this).is(':checked')) {
                            input = 'cls_chk_pa_productcode_item_ffc2';
                            obj.find('.' + input).prop('checked', false);
                        }
                        bindPA_SetData_FFC("change");
                    });
                    $(".cls_tfartwork_pa .cls_chk_pa_productcode_item_ffc2").click(function () {
                        if ($(this).is(':checked')) {
                            input = 'cls_chk_pa_productcode_item_vap2';
                            obj.find('.' + input).prop('checked', false);
                        }
                        bindPA_SetData_FFC("change");
                    });
                    input = 'cls_txt_pa_productcode_item_productcode2';
                    obj.find('.' + input).val(v.PRODUCT_CODE);

                    input = 'cls_chk_pa_productcode_item_vap2';
                    if (v.PRODUCT_TYPE == "VAP")
                    obj.find('.' + input).prop('checked',  true );
                    var plantList = [];
                    plantList = getplantlist();
                    var in_array = plantList.filter(function (item) {
                        return item.PRODUCTION_PLANT_ID == 3
                    });
                    var artwork_request_type = $('.cls_artwork_request_form .cls_input_artwork_request_type').val();
                    var index = plantList.indexOf(in_array[0]);
                    var checked_status = false;
                    if (artwork_request_type == "FFC") {
                        checked_status = true;
                    }
                    else if (plantList.length > 0 && index >= 0)
                        checked_status = false;
                    else if (plantList.length > 0 && index == -1) 
                        checked_status = true;

                    obj.find('.' + input).prop("disabled", checked_status);

                    input = 'cls_chk_pa_productcode_item_ffc2';
                    if (v.PRODUCT_TYPE == "FFC")
                        obj.find('.' + input).prop('checked', true);
                    else if (artwork_request_type == "FFC") 
                        obj.find('.' + input).prop('checked', true);
                    obj.find('.' + input).prop("disabled", true);

                    if (!isItemInProductCode(data_product_code, v.PRODUCT_CODE)) {
                        data_product_code.push([v.PRODUCT_CODE, v.NET_WEIGHT, v.DRAINED_WEIGHT]);
                    }
                }
            }
        }
        bindPA_SetData_FFC(event_val);
    }
}
//function bindPA_ProductCodeOther(data) {
//    for (var i = 0; i < data.length; i++) {
//        var v = data[i];
//        if (v.PRODUCT_CODE != null) {
//            var isExist = false;
//            $('.table_pa_productcode_item > tbody > tr').each(function () {
//                if (v.PRODUCT_CODE.toLowerCase() == $(this).find('.cls_txt_pa_productcode_item_productcode').val().toLowerCase()) {
//                    isExist = true;
//                }
//            });

//            if (!isExist) {
//                $('.table_pa_productcode_item > tbody > tr').each(function () {
//                    if (v.PRODUCT_CODE.toLowerCase() == $(this).find('.cls_txt_pa_productcode_item_productcode').val().toLowerCase()) {
//                        isExist = true;
//                    }
//                });

//                if (!isExist) {
//                    var obj = $('.tr_pa_productcode_item2').clone().removeClass('tr_pa_productcode_item2').removeClass('cls_cn_hide');

//                    $('.cls_tfartwork_pa .table_pa_productcode_item2 tbody').append(obj);
//                    var obj2 = $('.cls_tfartwork_pa .table_pa_productcode_item2 tbody tr:last');

//                    $(".cls_tfartwork_pa .cls_img_delete_pa_productcode_item2").click(function () {
//                        $(this).closest('tr').remove();
//                    });

//                    input = 'cls_txt_pa_productcode_item_productcode2';
//                    obj.find('.' + input).val(v.PRODUCT_CODE);

//                    if (!isItemInProductCode(data_product_code, v.PRODUCT_CODE)) {
//                        data_product_code.push([v.PRODUCT_CODE, v.NET_WEIGHT, v.DRAINED_WEIGHT]);
//                    }
//                }
//            }
//        }
//    }
//}

function callback_requestmaterial(res) {
    if (res.status == "S") {
        //bindTaskformArtwork(ArtworkSubId);
        //window.location.reload();
        click_retrive_or_request_mat = true;
        REQUEST_MATERIAL = "1";
        afterRequestMat();
        bindTaskformArtwork(ArtworkSubId);
    }
}

function callback_tf_complete_workflow(res) {
    if (res.status == "S") {
        tohomepage();
    }
    else if (res.msg != '') {
        alertError(res.msg);
    }
}

function bindPA_Net_Weight_DrainedWeight(selections, is_productcode,is_type) {
    //change by voravut 2020-09-10
    if (is_type=="")
    $('.table_pa_productcode_item tbody tr:visible').remove();
    if (selections.length > 0) {
        if (is_productcode) {
            if (selections[0].DISPLAY_TXT == undefined) {
                selections = defaultPA_ProductCode;
            }
            setPA_ProductCodeRDReference(true);
        } else {
            if (selections[0].DISPLAY_TXT == undefined) {
                selections = defaultPA_RDRef;
            }
            setPA_ProductCodeRDReference(false);
        }
        var item = [{
            "PRODUCT_CODE_ID": selections[0].ID,
            "PRODUCT_CODE": selections[0].DISPLAY_TXT,
            "PRODUCT_TYPE": selections[0].PRODUCT_TYPE,
            "PRODUCT_DESCRIPTION": selections[0].PRODUCT_DESCRIPTION,
            "NET_WEIGHT": selections[0].NET_WEIGHT,
            "DRAINED_WEIGHT": selections[0].DRAINED_WEIGHT,
            "RF_PRODUCT_RD_REF_ID": selections[0].ID,
            "DATA_FROM_DB": selections[0].DATA_FROM_DB
        }];
        bindPA_ProductCode(item);
    }
    else if (is_productcode) {
        setPA_ProductCodeRDReference(true);

    }
    else {
        setPA_ProductCodeRDReference(false);
    }
}

function saveTaskFormPA(is_showmsg) {
    var ischeckproduct = false;
    var product_type = "";
    var form_name = '.cls_tfartwork_pa ';
            var temp;
            if ($(form_name + '.cls_lov_pa_product_code').is(":visible"))
                temp = $(form_name + '.cls_lov_pa_product_code option:selected').text();
            else if ($(form_name + '.cls_lov_pa_rd_reference_no').is(":visible"))
                temp = $(form_name + '.cls_lov_pa_rd_reference_no option:selected').text();
    $(".cls_tfartwork_pa .table_pa_productcode_item tbody tr").each(function (index) {
        if (temp == $(this).find('.cls_txt_pa_productcode_item_productcode').val())
            ischeckproduct = true;

        if (temp == $(this).find('.cls_txt_pa_productcode_item_productcode').val()) {
            if ($(this).find('.cls_txt_pa_productcode_item_productcode').val() != "") {
                if ($(this).find('.cls_chk_pa_productcode_item_vap').is(':checked')) {
                    product_type = "VAP";
                } else if ($(this).find('.cls_chk_pa_productcode_item_ffc').is(':checked'))
                    product_type = "FFC";
                else
                    product_type = "";
            }
        }
    });
    if (ischeckproduct == false) {
        alertError2('Please add product code/RD Reference No. from drop-down list before add another product');
        return false;
    }
    var jsonObj = new Object();
    jsonObj.data = [];
    var form_name = '.cls_tfartwork_pa ';
    var item = {};

    if (ARTWORK_SUB_PA_ID != 0) {
        item["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
    }
    item['ARTWORK_SUB_ID'] = ArtworkSubId;

    var readytocreatepo = '';
    if ($('.cls_chk_header_tfartwork_po').prop('checked')) {
        readytocreatepo = 'X';
    }
    item['READY_CREATE_PO'] = readytocreatepo;
    item['PRODUCT_TYPE'] = product_type;
    item['MATERIAL_GROUP_ID'] = $(form_name + '.cls_lov_pa_material_group').val();
    item['TYPE_OF_ID'] = $(form_name + '.cls_lov_pa_type_of').val();
    item['TYPE_OF_OTHER'] = $(form_name + '.cls_input_pa_type_of').val();
    item['TYPE_OF_2_ID'] = $(form_name + '.cls_lov_pa_type_of_two').val();
    item['TYPE_OF_2_OTHER'] = $(form_name + '.cls_input_pa_type_of_two').val();
    item['RD_REFERENCE_NO_ID'] = $(form_name + '.cls_lov_pa_rd_reference_no').val();
    item['THREE_P_ID'] = $(form_name + '.cls_lov_pa_primary_size').val();
    item['PRIMARY_SIZE_OTHER'] = $(form_name + '.cls_input_pa_primary_size').val();
    if ($(form_name + '.cls_lov_pa_primary_size').val() == -1) {
        item['CONTAINER_TYPE_ID'] = $(form_name + '.cls_lov_pa_container_type').val();
        item['CONTAINER_TYPE_OTHER'] = $(form_name + '.cls_input_pa_container_type').val();
        item['LID_TYPE_ID'] = $(form_name + '.cls_lov_pa_lid_type').val();
        item['LID_TYPE_OTHER'] = $(form_name + '.cls_input_pa_lid_type').val();
    }
    item['TWO_P_ID'] = $(form_name + '.cls_lov_pa_packing_style').val();
    item['PACKING_STYLE_OTHER'] = $(form_name + '.cls_input_pa_packing_style').val();
    if ($(form_name + '.cls_lov_pa_packing_style').val() == -1) {
        item['PACK_SIZE_ID'] = $(form_name + '.cls_lov_pa_pack_size').val();
        item['PACK_SIZE_OTHER'] = $(form_name + '.cls_input_pa_pack_size').val();
    }
    item['PRODUCT_CODE_ID'] = $(form_name + '.cls_lov_pa_product_code').val();
    item['PLANT_REGISTERED_ID'] = $(form_name + '.cls_lov_pa_plant_register_no').val();
    item['PLANT_REGISTERED_OTHER'] = $(form_name + '.cls_input_pa_plant_register_no').val();
    //item['FAO_ZONE_ID'] = $(form_name + '.cls_lov_pa_fao_zone').val();
    //item['FAO_ZONE_OTHER'] = $(form_name + '.cls_input_pa_fao_zone').val();
    item["CHANGE_POINT"] = $('input[name=cls_rdo_haeder_tfartwork_change_point]:radio:checked').val();
    var faozoneList = [];
    var first = true;
    $(".cls_tfartwork_pa .tr_tfartwork_pa_fao_zone_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pa_fao_zone_multiple_static').val())) {
                var faozoneItem = {};
                faozoneItem["ARTWORK_SUB_ID"] = ArtworkSubId;
                if (ARTWORK_SUB_PA_ID != 0) {
                    faozoneItem["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
                }

                faozoneItem["FAO_ZONE_ID"] = $(this).find('.cls_lov_pa_fao_zone_multiple_static').val();
                faozoneItem["FAO_ZONE_OTHER"] = $(this).find('.cls_input_pa_fao_zone_multiple_static').val();
                faozoneItem["CREATE_BY"] = UserID;
                faozoneItem["UPDATE_BY"] = UserID;
                faozoneList.push(faozoneItem);
            }
        }
    });

    item['PMS_COLOUR_ID'] = $(form_name + '.cls_lov_pa_pms_colour').val();
    item['PMS_COLOUR_OTHER'] = $(form_name + '.cls_input_pa_pms_colour').val();

    item['COMPANY_ADDRESS_ID'] = $(form_name + '.cls_lov_pa_company_name').val();
    item['COMPANY_ADDRESS_OTHER'] = $(form_name + '.cls_input_pa_company_name').val();
    //item['CATCHING_AREA_ID'] = $(form_name + '.cls_lov_pa_catching_area').val();
    //item['CATCHING_AREA_OTHER'] = $(form_name + '.cls_input_pa_catching_area').val();

    var catching_areaList = [];
    first = true;
    $(".cls_tfartwork_pa .tr_tfartwork_pa_catching_area_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pa_catching_area_multiple_static').val())) {
                var catching_areaItem = {};
                catching_areaItem["ARTWORK_SUB_ID"] = ArtworkSubId;
                if (ARTWORK_SUB_PA_ID != 0) {
                    catching_areaItem["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
                }

                catching_areaItem["CATCHING_AREA_ID"] = $(this).find('.cls_lov_pa_catching_area_multiple_static').val();
                catching_areaItem["CATCHING_AREA_OTHER"] = $(this).find('.cls_input_pa_catching_area_multiple_static').val();
                catching_areaItem["CREATE_BY"] = UserID;
                catching_areaItem["UPDATE_BY"] = UserID;
                catching_areaList.push(catching_areaItem);
            }
        }
    });

    item['PROCESS_COLOUR_ID'] = $(form_name + '.cls_lov_pa_process_colour').val();
    item['PROCESS_COLOUR_OTHER'] = $(form_name + '.cls_input_pa_process_colour').val();
    //item['SYMBOL_ID'] = $(form_name + '.cls_lov_pa_symbol').val();
    //item['SYMBOL_OTHER'] = $(form_name + '.cls_input_pa_symbol').val();

    var symbolList = [];
    first = true;
    $(".cls_tfartwork_pa .tr_tfartwork_pa_symbol_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pa_symbol_multiple_static').val())) {
                var symbolItem = {};
                symbolItem["ARTWORK_SUB_ID"] = ArtworkSubId;
                if (ARTWORK_SUB_PA_ID != 0) {
                    symbolItem["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
                }

                symbolItem["SYMBOL_ID"] = $(this).find('.cls_lov_pa_symbol_multiple_static').val();
                symbolItem["SYMBOL_OTHER"] = $(this).find('.cls_input_pa_symbol_multiple_static').val();
                symbolItem["CREATE_BY"] = UserID;
                symbolItem["UPDATE_BY"] = UserID;
                symbolList.push(symbolItem);
            }
        }
    });

    item['CATCHING_PERIOD_ID'] = $(form_name + '.cls_lov_pa_catching_period').val();
    item['CATCHING_PERIOD_OTHER'] = $(form_name + '.cls_input_pa_catching_period').val();
    item['TOTAL_COLOUR_ID'] = $(form_name + '.cls_lov_pa_total_colour').val();
    item['TOTAL_COLOUR_OTHER'] = $(form_name + '.cls_input_pa_total_colour').val();

   
    //item['CATCHING_METHOD_ID'] = $(form_name + '.cls_lov_pa_catching_method').val();    // ticke#425737 commented by aof 
    //item['CATCHING_METHOD_OTHER'] = $(form_name + '.cls_input_pa_catching_method').val(); // ticke#425737 commented by aof 
    // ticke#425737 added by aof 
    var catching_methodList = [];
    first = true;
    $(".cls_tfartwork_pa .tr_tfartwork_pa_catching_method_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pa_catching_method_multiple_static').val())) {
                var catching_methodItem = {};
                catching_methodItem["ARTWORK_SUB_ID"] = ArtworkSubId;
                if (ARTWORK_SUB_PA_ID != 0) {
                    catching_methodItem["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
                }

                catching_methodItem["CATCHING_METHOD_ID"] = $(this).find('.cls_lov_pa_catching_method_multiple_static').val();
                catching_methodItem["CATCHING_METHOD_OTHER"] = $(this).find('.cls_input_pa_catching_method_multiple_static').val();
                catching_methodItem["CREATE_BY"] = UserID;
                catching_methodItem["UPDATE_BY"] = UserID;
                catching_methodList.push(catching_methodItem);
            }
        }
    });
     // ticke#425737 added by aof 

      

    item['STYLE_OF_PRINTING_ID'] = $(form_name + '.cls_lov_pa_style_of_printing').val();
    item['STYLE_OF_PRINTING_OTHER'] = $(form_name + '.cls_input_pa_style_of_printing').val();
    //item['PLANT_ID'] = $(form_name + '.cls_lov_pa_plant').val();
    //item['PLANT_OTHER'] = $(form_name + '.cls_input_pa_plant').val();

    var plantList = [];
    var first = true;
    $(".cls_tfartwork_pa .tr_tfartwork_pa_plant_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pa_plant_multiple_static').val())) {
                var plantItem = {};
                plantItem["ARTWORK_SUB_ID"] = ArtworkSubId;
                if (ARTWORK_SUB_PA_ID != 0) {
                    plantItem["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
                }

                plantItem["PLANT_ID"] = $(this).find('.cls_lov_pa_plant_multiple_static').val();
                plantItem["PLANT_OTHER"] = $(this).find('.cls_input_pa_plant_multiple_static').val();
                plantItem["CREATE_BY"] = UserID;
                plantItem["UPDATE_BY"] = UserID;
                plantList.push(plantItem);
            }
        }
    });

    item['SCIENTIFIC_NAME_ID'] = $(form_name + '.cls_lov_pa_scientific_name').val();
    item['SCIENTIFIC_NAME_OTHER'] = $(form_name + '.cls_input_pa_scientific_name').val();
    item['DIRECTION_OF_STICKER_ID'] = $(form_name + '.cls_lov_pa_direction_of_sticker').val();
    item['DIRECTION_OF_STICKER_OTHER'] = $(form_name + '.cls_input_pa_direction_of_sticker').val();
    item['SPECIE_ID'] = $(form_name + '.cls_lov_pa_specie').val();
    item['SPECIE_OTHER'] = $(form_name + '.cls_input_pa_specie').val();
    item['PRINTING_STYLE_OF_PRIMARY_ID'] = $(form_name + '.cls_lov_pa_printing_style_of_primary').val();
    item['PRINTING_STYLE_OF_PRIMARY_OTHER'] = $(form_name + '.cls_input_pa_printing_style_of_primary').val();
    item['PRINTING_STYLE_OF_SECONDARY_ID'] = $(form_name + '.cls_lov_pa_printing_style_of_secondary').val();
    item['PRINTING_STYLE_OF_SECONDARY_OTHER'] = $(form_name + '.cls_input_pa_printing_style_of_secondary').val();
    item['PACKAGE_QUANTITY'] = $(form_name + '.cls_txt_pa_package_quantity').val();
    item['CUSTOMER_DESIGN'] = $('input[name=pa_rdo_customer_design]:radio:checked').val();
    item['CUSTOMER_DESIGN_OTHER'] = $(form_name + '.cls_txt_pa_customer_design').val();
    item['FIRST_INFOGROUP_OTHER'] = $('.cls_txt_pa_first_info').val();
    item['WASTE_PERCENT'] = $(form_name + '.cls_txt_pa_waste_percent').val();
    item['CUSTOMER_SPEC'] = $('input[name=pa_rdo_customer_spec]:radio:checked').val();
    item['CUSTOMER_SPEC_OTHER'] = $(form_name + '.cls_txt_pa_customer_spec').val();
    item['PIC_MKT'] = $(form_name + '.cls_txt_pa_pic_mkt').val();
    item['COURIER_NO'] = $(form_name + '.cls_txt_pa_courier_no').val();
    item['CUSTOMER_SIZE'] = $('input[name=pa_rdo_customer_size]:radio:checked').val();
    item['CUSTOMER_SIZE_OTHER'] = $(form_name + '.cls_txt_pa_customer_size').val();

    item['PRODUCTION_EXPIRY_DATE_SYSTEM'] = $(form_name + '.cls_txt_pa_production_expiry_date_system').val();
    item['CUSTOMER_NOMINATES_VENDOR'] = $('input[name=pa_rdo_customer_nominate]:radio:checked').val();
    item['CUSTOMER_NOMINATES_VENDOR_OTHER'] = $(form_name + '.cls_txt_pa_customer_nominate').val();
    item['COMPLETE_INFOGROUP'] = $(form_name + '.cls_txt_pa_complete_infogroup').val();
    item['SERIOUSNESS_OF_COLOR_PRINTING'] = $('input[name=pa_rdo_customer_seriousness]:radio:checked').val();
    item['CUSTOMER_NOMINATES_COLOR'] = $('input[name=pa_rdo_customer_nominate_colour]:radio:checked').val();
    item['CUSTOMER_NOMINATES_COLOR_OTHER'] = $(form_name + '.cls_txt_pa_customer_nominate_colour').val();
    item['NOTE_OF_PA'] = $(form_name + '.cls_txt_pa_remark').val();

    item['NUTRITION_ANALYSIS'] = $('input[name=pa_rdo_customer_ingre]:radio:checked').val();
    item['CUSTOMER_BARCODE_SCANABLE'] = $('input[name=pa_rdo_customer_barcode]:radio:checked').val();
    item['CUSTOMER_BARCODE_SCANABLE_OTHER'] = $(form_name + '.cls_txt_pa_customer_barcode').val();
    item['CUSTOMER_BARCODE_SPEC'] = $('input[name=pa_rdo_customer_barcode_spec]:radio:checked').val();
    item['CUSTOMER_BARCODE_SPEC_OTHER'] = $(form_name + '.cls_txt_pa_barcode_spec').val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    item['PRODICUTION_PLANT_ID'] = -1;
    item['PRODICUTION_PLANT_OTHER'] = $(form_name + '.cls_input_pa_sec_pkg_production_plant').val();

    item['MATERIAL_DESCRIPTION'] = $('.cls_txt_pa_mat_desc').val();  // BY AOF IGRID_REIM_SPRINT2'
    item['RUNNING'] = $(form_name + '.cls_input_pa_packing_size_running').val();

    var first_row = true;
    var productList = [];
    var productOtherList = [];
    $(".cls_tfartwork_pa .table_pa_productcode_item tbody tr").each(function (index) {
       var temp;
        if ($(form_name + '.cls_lov_pa_product_code').is(":visible"))
            temp = $(form_name + '.cls_lov_pa_product_code option:selected').text();
        else if ($(form_name + '.cls_lov_pa_rd_reference_no').is(":visible"))
            temp = $(form_name + '.cls_lov_pa_rd_reference_no option:selected').text();

        if (first_row) first_row = false;
        else if ($(this).find('.cls_txt_pa_productcode_item_productcode').val() != temp) {
            //else if ($(this).find('.cls_hid_pa_productcode_id').val() != $(form_name + '.cls_lov_pa_product_code').val()) {
            if ($(this).find('.cls_hid_pa_productcode_id').val() != "" && $(this).find('.cls_txt_pa_productcode_item_productdescription').val() != "") {
                productItem = {};
                productItem["PRODUCT_CODE_ID"] = $(this).find('.cls_hid_pa_productcode_id').val();
                var product_type = "";
                if ($(this).find('.cls_chk_pa_productcode_item_vap').is(':checked')) {
                    product_type = "VAP";
                } else if ($(this).find('.cls_chk_pa_productcode_item_ffc').is(':checked'))
                    product_type = "FFC";
                productItem["PRODUCT_TYPE"] = product_type;
                if (ARTWORK_SUB_PA_ID != 0) {
                    productItem["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
                }
                productItem["RF_PRODUCT_RD_REF_ID"] = $(this).find('.cls_txt_pa_ref_request_id').val();
                productItem['ARTWORK_SUB_ID'] = ArtworkSubId;
                productItem["CREATE_BY"] = UserID;
                productItem["UPDATE_BY"] = UserID;
                productList.push(productItem);
            }
            else if ($(this).find('.cls_hid_pa_productcode_id').val() != "" && $(this).find('.cls_txt_pa_productcode_item_productdescription').val() == "") {
                productItem = {};
                productItem["PRODUCT_CODE"] = $(this).find('.cls_txt_pa_productcode_item_productcode').val();
                var product_type = "";
                if ($(this).find('.cls_chk_pa_productcode_item_vap').is(':checked')) {
                    product_type = "VAP";
                } else if ($(this).find('.cls_chk_pa_productcode_item_ffc').is(':checked'))
                    product_type = "FFC";
                productItem["PRODUCT_TYPE"] = product_type;
                if (ARTWORK_SUB_PA_ID != 0) {
                    productItem["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
                }
                productItem["RF_PRODUCT_RD_REF_ID"] = $(this).find('.cls_txt_pa_ref_request_id').val();
                productItem['ARTWORK_SUB_ID'] = ArtworkSubId;
                productItem["CREATE_BY"] = UserID;
                productItem["UPDATE_BY"] = UserID;
                productOtherList.push(productItem);
            }
        }
    });

    first_row = true;
    //var productOtherList = [];
    $(".cls_tfartwork_pa .table_pa_productcode_item2 tbody tr").each(function (index) {

        if (first_row) first_row = false;
        else {
            productItem2 = {};
            productItem2["PRODUCT_CODE"] = $(this).find('.cls_txt_pa_productcode_item_productcode2').val();
            var product_type = "";
            if ($(this).find('.cls_chk_pa_productcode_item_vap2').is(':checked')) {
                product_type = "VAP";
            } else if ($(this).find('.cls_chk_pa_productcode_item_ffc2').is(':checked'))
                product_type = "FFC";
            productItem2["PRODUCT_TYPE"] = product_type;
            if (ARTWORK_SUB_PA_ID != 0) {
                productItem2["ARTWORK_SUB_PA_ID"] = ARTWORK_SUB_PA_ID;
            }
            productItem2['ARTWORK_SUB_ID'] = ArtworkSubId;
            productItem2["CREATE_BY"] = UserID;
            productItem2["UPDATE_BY"] = UserID;
            productOtherList.push(productItem2);
        }
    });

    jsonObj.data = item;
    jsonObj.data.PRODUCTS = productList;
    jsonObj.data.PRODUCT_OTHERS = productOtherList;
    jsonObj.data.FAOS = faozoneList;
    jsonObj.data.CATCHING_AREAS = catching_areaList;
    jsonObj.data.SYMBOLS = symbolList;
    jsonObj.data.PLANTS = plantList;
    jsonObj.data.CATCHING_METHODS = catching_methodList;

    var myurl = '/api/taskform/pa/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjax(myurl, mytype, mydata, callbackSaveTaskFormPA, '', false, is_showmsg);
}

function callbackSaveTaskFormPA(res) {
    ARTWORK_SUB_PA_ID = res.data[0].ARTWORK_SUB_PA_ID;
    bindDataTaskFormPA();
}

function callbackReference(res) {
    var item = res.data[0];
    if (res.data.length > 0) {
        $('.cls_tfartwork_pa .cls_input_net_weight').val(item.NET_WEIGHT);
        $('.cls_tfartwork_pa .cls_input_drain_weight').val(item.DRAINED_WEIGHT);
    }
}

function getPA_RDReference() {
    var myurl = '/api/lov/pa/rdreferenceno?data.artwork_request_id=' + ARTWORK_REQUEST_ID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_getPA_RDReference);
}

function callback_getPA_RDReference(res) {
    var item = res.data[0];
    if (res.data.length > 0) {
        setPA_ProductCodeRDReference(false);
    }
    else {
        setPA_ProductCodeRDReference(true);
    }
}

function bindDataTaskFormPA() {
    var myurl = '/api/taskform/pa/info?data.artwork_sub_id=' + MainArtworkSubId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_dataTaskFormPA);
}

function bindDataCustomerPopup() {
    var myurl = '/api/taskform/internal/qc/info?data.artwork_sub_id=' + MainArtworkSubId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_dataCustomerPopup);
}

function callback_bind_dataCustomerPopup(res) {
    if (res.data.length > 0) {
        var item = res.data[0];

        if (item.NUTRITION != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_nutri_cus_pop").show();
            var editor1 = new Quill("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_nutri_txt_comment_cus_pop", {
                modules: { toolbar: toolbarOptions },
                theme: 'snow'
            });
            if (item.NUTRITION_COMMENT != null)
                editor1.clipboard.dangerouslyPasteHTML(item.NUTRITION_COMMENT);
            else
                editor1.clipboard.dangerouslyPasteHTML("<p>No comment</p>");

            editor1.enable(false);
        }
        if (item.INGREDIENTS != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_ingri_cus_pop").show();
            var editor2 = new Quill("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_ingre_txt_comment_cus_pop", {
                modules: { toolbar: toolbarOptions },
                theme: 'snow'
            });
            if (item.INGREDIENTS_COMMENT != null)
                editor2.clipboard.dangerouslyPasteHTML(item.INGREDIENTS_COMMENT);
            else
                editor2.clipboard.dangerouslyPasteHTML("<p>No comment</p>");

            editor2.enable(false);
        }
        if (item.ANALYSIS != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_analysis_cus_pop").show();
            var editor3 = new Quill("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_analysis_txt_comment_cus_pop", {
                modules: { toolbar: toolbarOptions },
                theme: 'snow'
            });
            if (item.ANALYSIS_COMMENT != null)
                editor3.clipboard.dangerouslyPasteHTML(item.ANALYSIS_COMMENT);
            else
                editor3.clipboard.dangerouslyPasteHTML("<p>No comment</p>");

            editor3.enable(false);
        }
        if (item.HEALTH_CLAIM != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_health_cus_pop").show();
            var editor4 = new Quill("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_health_txt_comment_cus_pop", {
                modules: { toolbar: toolbarOptions },
                theme: 'snow'
            });
            if (item.HEALTH_CLAIM_COMMENT != null)
                editor4.clipboard.dangerouslyPasteHTML(item.HEALTH_CLAIM_COMMENT);
            else
                editor4.clipboard.dangerouslyPasteHTML("<p>No comment</p>");

            editor4.enable(false);
        }
        if (item.NUTRIENT_CLAIM != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_nutri_claim_cus_pop").show();
            var editor5 = new Quill("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_nutclaim_txt_comment_cus_pop", {
                modules: { toolbar: toolbarOptions },
                theme: 'snow'
            });
            if (item.NUTRIENT_CLAIM_COMMENT != null)
                editor5.clipboard.dangerouslyPasteHTML(item.NUTRIENT_CLAIM_COMMENT);
            else
                editor5.clipboard.dangerouslyPasteHTML("<p>No comment</p>");

            editor5.enable(false);
        }
        if (item.SPECIES != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_species_cus_pop").show();
            var editor6 = new Quill("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_species_txt_comment_cus_pop", {
                modules: { toolbar: toolbarOptions },
                theme: 'snow'
            });
            if (item.SPECIES_COMMENT != null)
                editor6.clipboard.dangerouslyPasteHTML(item.SPECIES_COMMENT);
            else
                editor6.clipboard.dangerouslyPasteHTML("<p>No comment</p>");

            editor6.enable(false);
        }
        if (item.CATCHING_AREA != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_catch_fao_cus_pop").show();
            var editor7 = new Quill("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_catching_txt_comment_cus_pop", {
                modules: { toolbar: toolbarOptions },
                theme: 'snow'
            });
            if (item.CATCHING_AREA_COMMENT != null)
                editor7.clipboard.dangerouslyPasteHTML(item.CATCHING_AREA_COMMENT);
            else
                editor7.clipboard.dangerouslyPasteHTML("<p>No comment</p>");

            editor7.enable(false);
        }

        if (item.CHECK_DETAIL != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_check_detail_cus_pop").show();
            var editor8 = new Quill("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_check_detail_txt_comment_cus_pop", {
                modules: { toolbar: toolbarOptions },
                theme: 'snow'
            });
            if (item.CHECK_DETAIL_COMMENT != null)
                editor8.clipboard.dangerouslyPasteHTML(item.CHECK_DETAIL_COMMENT);
            else
                editor8.clipboard.dangerouslyPasteHTML("<p>No comment</p>");

            editor8.enable(false);
        }

        if (item.COMMENT != null) {
            countCus++;
            $("#send_to_customer_modal .cls_body_qc_changedetails_customer .cls_comment_qc_cus_pop").show();

            $("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_comment_qc_cus_pop").text(item.COMMENT);
        }

        if (countCus != 0) {
            $('#send_to_customer_modal .cls_chk_formandlabelraw_customer').prop('checked', true);
            $('#send_to_customer_modal .cls_body_formandlabelraw_customer').show();
            $('#send_to_customer_modal .cls_body_qc_changedetails_customer').show();
            $('#send_to_customer_modal .cls_chk_qc_changedetails_customer').prop("checked", true);
        }
    }
}

function callback_bind_dataTaskFormPA(res) {
    if (res.data.length > 0) {

        event_val = "";

     
        defaultPA_RDRef = [];
        defaultPA_ProductCode = [];
        var item = res.data[0];
        if (item != null) {
            var form_name = '.cls_tfartwork_pa ';
            ARTWORK_SUB_PA_ID = item.ARTWORK_SUB_PA_ID;

            $(form_name + '.cls_txt_pa_pa_username').val(item.PA_DISPLAY_TXT);
            $(form_name + '.cls_txt_pa_pg_username').val(item.PG_DISPLAY_TXT);
            $(form_name + '.cls_txt_pa_vendor').val(item.VENDOR_BY_MIGRATION_DISPLAY_TXT);

            if (is_first_load_pa)
            {
                debugger;
                $('.cls_txt_pa_mat_desc').val(item.MATERIAL_DESCRIPTION);   //by aof IGRID_REIM_SPRINT2
            }

            $(form_name + '.cls_input_pa_packing_size_running').val(item.RUNNING);  //by aof IGRID_REIM_SPRINT2
            $(form_name + '.cls_lbl_pa_packing_size_running').html(item.RUNNING);  //by aof IGRID_REIM_SPRINT2
  
            CHECK_DIF_REQ_RD = item.CHECK_DIFFERNT_REQ_PA

            if ((CHECK_DIF_REQ_RD) && (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PA')) {
                $('.cls_row_header_tfartwork_procode_ref_msg').show();
                $('.cls_lbl_header_tfartwork_procode_ref_msg').html('<p>Product code/RD reference in PA data and request form has different please check.</p>');
            }

            if (item.MATERIAL_GROUP_ID != null) {
                var material_group_display_txt = "";
                if (item.MATERIAL_GROUP_FROMSO_DISPLAY_TXT != null) {
                    setValueToDDL(form_name + '.cls_lov_pa_material_group', item.MATERIAL_GROUP_ID, item.MATERIAL_GROUP_FROMSO_DISPLAY_TXT);
                    material_group_display_txt = item.MATERIAL_GROUP_FROMSO_DISPLAY_TXT;
                    $(form_name + '.cls_lov_pa_material_group').prop("disabled", true);
                }
                else {
                    setValueToDDL(form_name + '.cls_lov_pa_material_group', item.MATERIAL_GROUP_ID, item.MATERIAL_GROUP_DISPLAY_TXT);
                    material_group_display_txt = item.MATERIAL_GROUP_DISPLAY_TXT;
                    if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                        $(form_name + '.cls_lov_pa_material_group').prop("disabled", true);
                    }
                    else {
                        $(form_name + '.cls_lov_pa_material_group').prop("disabled", false);
                    }
                }
                var defaultPA_MaterialGroupItem = {
                    ID: item.MATERIAL_GROUP_ID,
                    DISPLAY_TXT: material_group_display_txt,
                    VALUE: item.MATERIAL_GROUP_CODE
                };
                defaultPA_MaterialGroupData.push(defaultPA_MaterialGroupItem);
                setRequireFieldTaskformArtworkPA(item.MATERIAL_GROUP_CODE);
            }

            setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search', item.MATERIAL_GROUP_ID, material_group_display_txt);
            $('.cls_task_form_pg_artwork .cls_hid_artwork_taskform_packaging_type').val(item.MATERIAL_GROUP_ID);

            setValueToDDL(form_name + '.cls_lov_pa_type_of', item.TYPE_OF_ID, item.TYPE_OF_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_pa_input_type_of', item.TYPE_OF_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_type_of_two', item.TYPE_OF_2_ID, item.TYPE_OF_2_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_type_of_two', item.TYPE_OF_2_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_type_of_two', item.TYPE_OF_2_ID, item.TYPE_OF_2_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_type_of_two', item.TYPE_OF_2_OTHER);

            if (item.RD_REFERENCE_NO_ID != null ) {
                $(".cls_tfartwork_pa .cls_lov_pa_product_code").val('').trigger("change");
                setValueToDDL(form_name + '.cls_lov_pa_rd_reference_no', item.RD_REFERENCE_NO_ID, item.RD_REFERENCE_NO_DISPLAY_TXT);
                var selections = $(form_name + '.cls_lov_pa_rd_reference_no').select2('data');
                if (selections != undefined && selections.length != 0) {
                    selections[0]["NET_WEIGHT"] = item.NET_WEIGHT_DISPLAY_TXT;
                    selections[0]["DRAINED_WEIGHT"] = item.DRAIN_WEIGHT_DISPLAY_TXT;
                    selections[0]["PRODUCT_TYPE"] = item.PRODUCT_TYPE;
                    selections[0]["RF_PRODUCT_RD_REF_ID"] = item.RD_REFERENCE_NO_ID;
                    selections[0]["DATA_FROM_DB"] = item.DATA_FROM_DB;
                }
                var defaultPA_RDRefItem = {
                    ID: item.RD_REFERENCE_NO_ID,
                    DISPLAY_TXT: item.RD_REFERENCE_NO_DISPLAY_TXT,
                    NET_WEIGHT: item.NET_WEIGHT_DISPLAY_TXT,
                    DRAINED_WEIGHT: item.DRAIN_WEIGHT_DISPLAY_TXT,
                    PRODUCT_TYPE: item.PRODUCT_TYPE,
                    RF_PRODUCT_RD_REF_ID: item.RD_REFERENCE_NO_ID,
                    DATA_FROM_DB:  item.DATA_FROM_DB
                };
                defaultPA_RDRef.push(defaultPA_RDRefItem);
                bindPA_Net_Weight_DrainedWeight(defaultPA_RDRef, false,"");
            }
            else if (item.PRODUCT_CODE_ID != null) {
                $(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no").val('').trigger("change");
                setValueToDDL(form_name + '.cls_lov_pa_product_code', item.PRODUCT_CODE_ID, item.PRODUCT_CODE_DISPLAY_TXT);

                if (item.IS_LOCK_PRODUCT_CODE != null && item.IS_LOCK_PRODUCT_CODE != "") {
                    $(form_name + '.cls_lov_pa_product_code').prop("disabled", true);
                } else {
                    if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                        $(form_name + '.cls_lov_pa_product_code').prop("disabled", true);
                    }
                    else {
                        $(form_name + '.cls_lov_pa_product_code').prop("disabled", false);
                    }
                }
                var selections = $(form_name + '.cls_lov_pa_product_code').select2('data');
                if (selections != undefined && selections.length != 0) {
                    selections[0]["PRODUCT_DESCRIPTION"] = item.PRODUCT_DESCRIPTION_DISPLAY_TXT;
                    selections[0]["NET_WEIGHT"] = item.NET_WEIGHT_DISPLAY_TXT;
                    selections[0]["DRAINED_WEIGHT"] = item.DRAIN_WEIGHT_DISPLAY_TXT;
                    selections[0]["PRODUCT_TYPE"] = item.PRODUCT_TYPE;
                    selections[0]["RF_PRODUCT_RD_REF_ID"] = item.PRODUCT_CODE_ID; // selections[0]["RF_PRODUCT_RD_REF_ID"] = item.ID; //------- by aof
                    selections[0]["DATA_FROM_DB"] = item.DATA_FROM_DB;
                    selections[0]["DISPLAY_TXT"] = item.PRODUCT_CODE_DISPLAY_TXT;  //------- by aof
                    selections[0]["PRODUCT_CODE_ID"] = item.PRODUCT_CODE_ID;   //------- by aof
                    selections[0]["ID"] = item.PRODUCT_CODE_ID;   //------- by aof
                }
                $('.table_pa_productcode_item tbody tr:visible').remove();
                var defaultPA_ProductCodeItem = {
                    ID: item.PRODUCT_CODE_ID,
                    DISPLAY_TXT: item.PRODUCT_CODE_DISPLAY_TXT,
                    PRODUCT_DESCRIPTION: item.PRODUCT_DESCRIPTION_DISPLAY_TXT,
                    PRODUCT_TYPE: item.PRODUCT_TYPE,
                    NET_WEIGHT: item.NET_WEIGHT_DISPLAY_TXT,
                    DRAINED_WEIGHT: item.DRAIN_WEIGHT_DISPLAY_TXT,
                    PRODUCT_TYPE: item.PRODUCT_TYPE,
                    RF_PRODUCT_RD_REF_ID: item.PRODUCT_CODE_ID,
                    DATA_FROM_DB: item.DATA_FROM_DB
                };
                defaultPA_ProductCode.push(defaultPA_ProductCodeItem);
                bindPA_Net_Weight_DrainedWeight(defaultPA_ProductCode, true,"");

            }
            else if (item.RD_REFERENCE_NO_ID == null && item.PRODUCT_CODE_ID == null) {
                if ($(".cls_tfartwork_pa .cls_lov_pa_product_code").val() != null) {
                    var selections = $(".cls_tfartwork_pa .cls_lov_pa_product_code").select2('data');
                    bindPA_Net_Weight_DrainedWeight(selections, true,"add");
                }
                else if ($(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no").val() != null) {
                    var selections = $(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no").select2('data');
                    bindPA_Net_Weight_DrainedWeight(selections, false,"add");
                }
                else {
                    $(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no").val('').trigger("change");
                    $(".cls_tfartwork_pa .cls_lov_pa_product_code").val('').trigger("change");
                    getPA_RDReference();
                }
            }


            $('.table_pa_productcode_item2 tbody tr:visible').remove();
            if (item.PRODUCT_OTHERS.length > 0) {
                var data_product_Other = [];
                $.each(item.PRODUCT_OTHERS, function (index, data_ref) {
                    if (data_ref.RF_PRODUCT_RD_REF_ID == null) {
                        data_product_Other.push(data_ref);
                    } else {
                        data_ref["PRODUCT_CODE_ID"] = data_ref.RF_PRODUCT_RD_REF_ID;
                        item.PRODUCTS.push(data_ref);
                    }
                })
                if (data_product_Other.length>0)
                    bindPA_ProductCodeOther(data_product_Other);
            }
            if (item.PRODUCTS.length > 0) {
                bindPA_ProductCode(item.PRODUCTS);
            }
            setValueToDDL(form_name + '.cls_lov_pa_primary_size', item.THREE_P_ID, item.THREE_P_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_primary_size', item.PRIMARY_SIZE_OTHER);
            if ((item.CONTAINER_TYPE_ID == null) && (item.CONTAINER_TYPE_DISPLAY_TXT != null) && (item.CONTAINER_TYPE_DISPLAY_TXT != "")) {
                setValueToDDL(form_name + '.cls_lov_pa_container_type', -99, item.CONTAINER_TYPE_DISPLAY_TXT);
                $(form_name + '.cls_lov_pa_container_type').prop("disabled", true);
            } else {
                setValueToDDL(form_name + '.cls_lov_pa_container_type', item.CONTAINER_TYPE_ID, item.CONTAINER_TYPE_DISPLAY_TXT);
            }
            setValueToDDLOther(form_name + '.cls_input_pa_container_type', item.CONTAINER_TYPE_OTHER);

            if ((item.LID_TYPE_ID == null) && (item.LID_TYPE_DISPLAY_TXT != null) && (item.LID_TYPE_DISPLAY_TXT != "")) {
                setValueToDDL(form_name + '.cls_lov_pa_lid_type', -99, item.LID_TYPE_DISPLAY_TXT);
                $(form_name + '.cls_lov_pa_lid_type').prop("disabled", true);
            } else {
                setValueToDDL(form_name + '.cls_lov_pa_lid_type', item.LID_TYPE_ID, item.LID_TYPE_DISPLAY_TXT);
            }
            setValueToDDLOther(form_name + '.cls_input_pa_lid_type', item.LID_TYPE_OTHER);

            if (item.THREE_P_ID != null) {
                var defaultPA_PrimarySizeDataItem = {};
                defaultPA_PrimarySizeDataItem[item.THREE_P_ID] = {
                    CONTAINER_TYPE_ID: -99,
                    CONTAINER_TYPE_DISPLAY_TXT: item.CONTAINER_TYPE_DISPLAY_TXT,
                    LID_TYPE_ID: -99,
                    LID_TYPE_DISPLAY_TXT: item.LID_TYPE_DISPLAY_TXT
                };
                defaultPA_PrimarySizeData = defaultPA_PrimarySizeDataItem;
            }

            setValueToDDL(form_name + '.cls_lov_pa_packing_style', item.TWO_P_ID, item.TWO_P_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_packing_style', item.PACKING_STYLE_OTHER);
            $(form_name + '.cls_lbl_pa_packing_style_requestform').html(item.PACKING_STYLE_REQUESTFORM);
            $(form_name + '.cls_input_pa_packing_style_requestform').val(item.PACKING_STYLE_REQUESTFORM);  // by aof IGRID_REIM_SPINT2
            prev_packingstyle_val = item.TWO_P_ID; // by aof IGRID_REIM_SPINT2
            prev_packingstyle_display = item.TWO_P_DISPLAY_TXT; // by aof IGRID_REIM_SPINT2
            

            if ((res.data[0].PACK_SIZE_ID == null) && (res.data[0].PACK_SIZE_DISPLAY_TXT != null) && (res.data[0].PACK_SIZE_DISPLAY_TXT != "")) {
                setValueToDDL(form_name + '.cls_lov_pa_pack_size', -99, item.PACK_SIZE_DISPLAY_TXT);
                $(form_name + '.cls_lov_pa_pack_size').prop("disabled", true);
            } else {
                setValueToDDL(form_name + '.cls_lov_pa_pack_size', item.PACK_SIZE_ID, item.PACK_SIZE_DISPLAY_TXT);
            }
            setValueToDDLOther(form_name + '.cls_input_pa_pack_size', item.PACK_SIZE_OTHER);

            if (item.TWO_P_ID != null) {
                var defaultPA_PackingStyleDataItem = {};
                defaultPA_PackingStyleDataItem[item.TWO_P_ID] = {
                    PACK_SIZE_ID: -99,
                    PACK_SIZE_DISPLAY_TXT: item.PACK_SIZE_DISPLAY_TXT
                };
                defaultPA_PackingStyleData = defaultPA_PackingStyleDataItem;
            }

            $(form_name + '.cls_input_pa_brand').val(item.BRAND_DISPLAY_TXT);
            $(form_name + '.cls_lbl_pa_brand_warning').html(item.BRAND_WARNING_DISPLAY_TXT);

            $(form_name + '.cls_input_pa_sec_pkg_production_plant').html(item.PRODICUTION_PLANT_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_plant_register_no', item.PLANT_REGISTERED_ID, item.PLANT_REGISTERED_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_plant_register_no', item.PLANT_REGISTERED_OTHER);

            if (res.data[0].FAOS != null) {
                cnt_lov_tfartwork_fao_zone_multiple = 1;
                $('.tr_tfartwork_pa_fao_zone_multiple_static:visible').remove();
                $.each(res.data[0].FAOS, function (index, item) {
                    var obj = $('.cls_lov_pa_fao_zone_multiple_static:first');
                    if (index > 0 || ($('.cls_lov_pa_fao_zone_multiple' + (index + 1)).length == 0)) {
                        obj.closest('.tr_tfartwork_pa_fao_zone_multiple_static').find(".cls_img_lov_tfartwork_pa_add_fao_zone_multiple").click();
                        if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                            $('.cls_img_lov_tfartwork_pa_delete_fao_zone_multiple').hide();
                        }
                    }
                    else {
                        cnt_lov_tfartwork_fao_zone_multiple++;
                    }

                    if (item.FAO_ZONE_ID == -1) {
                        setValueToDDL('.cls_lov_pa_fao_zone_multiple' + (index + 1), item.FAO_ZONE_ID, '');
                        setValueToDDLOther('.cls_input_pa_fao_zone_multiple' + (index + 1), item.FAO_ZONE_OTHER);
                    }
                    else {
                        setValueToDDL('.cls_lov_pa_fao_zone_multiple' + (index + 1), item.FAO_ZONE_ID, item.DISPLAY_TXT);
                    }
                });
            }

            setValueToDDL(form_name + '.cls_lov_pa_pms_colour', item.PMS_COLOUR_ID, item.PMS_COLOUR_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_pms_colour', item.PMS_COLOUR_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_company_name', item.COMPANY_ADDRESS_ID, item.COMPANY_ADDRESS_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_company_name', item.COMPANY_ADDRESS_OTHER);

            if (res.data[0].CATCHING_AREAS != null) {
                cnt_lov_tfartwork_catching_area_multiple = 1;
                $('.tr_tfartwork_pa_catching_area_multiple_static:visible').remove();
                $.each(res.data[0].CATCHING_AREAS, function (index, item) {
                    var obj = $('.cls_lov_pa_catching_area_multiple_static:first');
                    if (index > 0 || ($('.cls_lov_pa_catching_area_multiple' + (index + 1)).length == 0)) {
                        obj.closest('.tr_tfartwork_pa_catching_area_multiple_static').find(".cls_img_lov_tfartwork_pa_add_catching_area_multiple").click();
                        if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                            $('.cls_img_lov_tfartwork_pa_delete_catching_area_multiple').hide();
                        }
                    }
                    else {
                        cnt_lov_tfartwork_catching_area_multiple++;
                    }

                    if (item.CATCHING_AREA_ID == -1) {
                        setValueToDDL('.cls_lov_pa_catching_area_multiple' + (index + 1), item.CATCHING_AREA_ID, '');
                        setValueToDDLOther('.cls_input_pa_catching_area_multiple' + (index + 1), item.CATCHING_AREA_OTHER);
                    }
                    else {
                        setValueToDDL('.cls_lov_pa_catching_area_multiple' + (index + 1), item.CATCHING_AREA_ID, item.DISPLAY_TXT);
                    }
                });
            }

            setValueToDDL(form_name + '.cls_lov_pa_process_colour', item.PROCESS_COLOUR_ID, item.PROCESS_COLOUR_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_process_colour', item.PROCESS_COLOUR_OTHER);

            if (res.data[0].SYMBOLS != null) {
                cnt_lov_tfartwork_symbol_multiple = 1;
                $('.tr_tfartwork_pa_symbol_multiple_static:visible').remove();
                $.each(res.data[0].SYMBOLS, function (index, item) {
                    var obj = $('.cls_lov_pa_symbol_multiple_static:first');
                    if (index > 0 || ($('.cls_lov_pa_symbol_multiple' + (index + 1)).length == 0)) {
                        obj.closest('.tr_tfartwork_pa_symbol_multiple_static').find(".cls_img_lov_tfartwork_pa_add_symbol_multiple").click();
                        if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                            $('.cls_img_lov_tfartwork_pa_delete_symbol_multiple').hide();
                        }
                    }
                    else {
                        cnt_lov_tfartwork_symbol_multiple++;
                    }

                    if (item.SYMBOL_ID == -1) {
                        setValueToDDL('.cls_lov_pa_symbol_multiple' + (index + 1), item.SYMBOL_ID, '');
                        setValueToDDLOther('.cls_input_pa_symbol_multiple' + (index + 1), item.SYMBOL_OTHER);
                    }
                    else {
                        setValueToDDL('.cls_lov_pa_symbol_multiple' + (index + 1), item.SYMBOL_ID, item.DISPLAY_TXT);
                    }
                });
            }

            setValueToDDL(form_name + '.cls_lov_pa_catching_period', item.CATCHING_PERIOD_ID, item.CATCHING_PERIOD_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_catching_period', item.CATCHING_PERIOD_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_total_colour', item.TOTAL_COLOUR_ID, item.TOTAL_COLOUR_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_total_colour', item.TOTAL_COLOUR_OTHER);

            $(form_name + '.cls_lov_pa_sec_pkg_production_plant').val(item.PRODICUTION_PLANT_DISPLAY_TXT);



            
            //setValueToDDL(form_name + '.cls_lov_pa_catching_method', item.CATCHING_METHOD_ID, item.CATCHING_METHOD_DISPLAY_TXT);  // ticke#425737 commented by aof
            //setValueToDDLOther(form_name + '.cls_input_pa_catching_method', item.CATCHING_METHOD_OTHER); // ticke#425737 commented by aof
            // ticke#425737 added by aof 
            if (res.data[0].CATCHING_METHODS != null) {
                cnt_lov_tfartwork_catching_method_multiple = 1;
                $('.tr_tfartwork_pa_catching_method_multiple_static:visible').remove();
                $.each(res.data[0].CATCHING_METHODS, function (index, item) {
                    var obj = $('.cls_lov_pa_catching_method_multiple_static:first');
                    if (index > 0 || ($('.cls_lov_pa_catching_method_multiple' + (index + 1)).length == 0)) {
                        obj.closest('.tr_tfartwork_pa_catching_method_multiple_static').find(".cls_img_lov_tfartwork_pa_add_catching_method_multiple").click();
                        if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                            $('.cls_img_lov_tfartwork_pa_delete_catching_method_multiple').hide();
                        }
                    }
                    else {
                        cnt_lov_tfartwork_catching_method_multiple++;
                    }

                    if (item.CATCHING_METHOD_ID == -1) {
                        setValueToDDL('.cls_lov_pa_catching_method_multiple' + (index + 1), item.CATCHING_METHOD_ID, '');
                        setValueToDDLOther('.cls_input_pa_catching_method_multiple' + (index + 1), item.CATCHING_METHOD_OTHER);
                    }
                    else {
                        setValueToDDL('.cls_lov_pa_catching_method_multiple' + (index + 1), item.CATCHING_METHOD_ID, item.DISPLAY_TXT);
                    }
                });
            }
             // ticke#425737 added by aof

            setValueToDDL(form_name + '.cls_lov_pa_style_of_printing', item.STYLE_OF_PRINTING_ID, item.STYLE_OF_PRINTING_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_style_of_printing', item.STYLE_OF_PRINTING_OTHER);

            if (res.data[0].PLANTS != null) {
                cnt_lov_tfartwork_plant_multiple = 1;
                $('.tr_tfartwork_pa_plant_multiple_static:visible').remove();
                $.each(res.data[0].PLANTS, function (index, item) {
                    var obj = $('.cls_lov_pa_plant_multiple_static:first');
                    if (index > 0 || ($('.cls_lov_pa_plant_multiple' + (index + 1)).length == 0)) {
                        obj.closest('.tr_tfartwork_pa_plant_multiple_static').find(".cls_img_lov_tfartwork_pa_add_plant_multiple").click();
                        if (CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_PG') {
                            $('.cls_img_lov_tfartwork_pa_delete_plant_multiple').hide();
                        }
                    }
                    else {
                        cnt_lov_tfartwork_plant_multiple++;
                    }

                    if (item.PLANT_ID == -1) {
                        setValueToDDL('.cls_lov_pa_plant_multiple' + (index + 1), item.PLANT_ID, '');
                        setValueToDDLOther('.cls_input_pa_plant_multiple' + (index + 1), item.PLANT_OTHER);
                    }
                    else {
                        setValueToDDL('.cls_lov_pa_plant_multiple' + (index + 1), item.PLANT_ID, item.DISPLAY_TXT);
                    }
                });
            }

            setValueToDDL(form_name + '.cls_lov_pa_scientific_name', item.SCIENTIFIC_NAME_ID, item.SCIENTIFIC_NAME_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_scientific_name', item.SCIENTIFIC_NAME_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_direction_of_sticker', item.DIRECTION_OF_STICKER_ID, item.DIRECTION_OF_STICKER_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_direction_of_sticker', item.DIRECTION_OF_STICKER_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_specie', item.SPECIE_ID, item.SPECIE_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_specie', item.SPECIE_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_printing_style_of_primary', item.PRINTING_STYLE_OF_PRIMARY_ID, item.PRINTING_STYLE_OF_PRIMARY_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_printing_style_of_primary', item.PRINTING_STYLE_OF_PRIMARY_OTHER);

            setValueToDDL(form_name + '.cls_lov_pa_printing_style_of_secondary', item.PRINTING_STYLE_OF_SECONDARY_ID, item.PRINTING_STYLE_OF_SECONDARY_DISPLAY_TXT);
            setValueToDDLOther(form_name + '.cls_input_pa_printing_style_of_secondary', item.PRINTING_STYLE_OF_SECONDARY_OTHER);

            $(form_name + '.cls_txt_pa_package_quantity').val(item.PACKAGE_QUANTITY);
            if (item.CUSTOMER_DESIGN != "") {
                $("input[name=pa_rdo_customer_design][value=" + item.CUSTOMER_DESIGN + "]").prop('checked', true);
            }
            $(form_name + '.cls_txt_pa_customer_design').val(item.CUSTOMER_DESIGN_OTHER);
            $(form_name + '.cls_txt_pa_first_info').val(item.FIRST_INFOGROUP_OTHER);
            $(form_name + '.cls_txt_pa_waste_percent').val(item.WASTE_PERCENT);
            if (item.CUSTOMER_SPEC != "") {
                $("input[name=pa_rdo_customer_spec][value=" + item.CUSTOMER_SPEC + "]").prop('checked', true);
            }
            $(form_name + '.cls_txt_pa_customer_spec').val(item.CUSTOMER_SPEC_OTHER);
            $(form_name + '.cls_txt_pa_pic_mkt').val(item.PIC_MKT);
            $(form_name + '.cls_txt_pa_courier_no').val(item.COURIER_NO);
            if (item.CUSTOMER_SIZE != "") {
                $('input[name=pa_rdo_customer_size][value=' + item.CUSTOMER_SIZE + ']').prop('checked', true);
            }
            $(form_name + '.cls_txt_pa_customer_size').val(item.CUSTOMER_SIZE_OTHER);
            $(form_name + '.cls_input_pa_sec_pkg_production_plant').val(item.PRODICUTION_PLANT_DISPLAY_TXT);
            $(form_name + '.cls_txt_pa_country').val(item.COUNTRY);
            $(form_name + '.cls_txt_pa_production_expiry_date_system').val(item.PRODUCTION_EXPIRY_DATE_SYSTEM);
            if (item.CUSTOMER_NOMINATES_VENDOR != "") {
                $("input[name=pa_rdo_customer_nominate][value=" + item.CUSTOMER_NOMINATES_VENDOR + "]").prop('checked', true);
            }
            $(form_name + '.cls_txt_pa_customer_nominate').val(item.CUSTOMER_NOMINATES_VENDOR_OTHER);
            $(form_name + '.cls_txt_pa_complete_infogroup').val(item.COMPLETE_INFOGROUP);
            if (item.SERIOUSNESS_OF_COLOR_PRINTING != "") {
                $("input[name=pa_rdo_customer_seriousness][value=" + item.SERIOUSNESS_OF_COLOR_PRINTING + "]").prop('checked', true);
            }
            if (item.CUSTOMER_NOMINATES_COLOR != "") {
                $("input[name=pa_rdo_customer_nominate_colour][value=" + item.CUSTOMER_NOMINATES_COLOR + "]").prop('checked', true);
            }
            $(form_name + '.cls_txt_pa_customer_nominate_colour').val(item.CUSTOMER_NOMINATES_COLOR_OTHER);
            $(form_name + '.cls_txt_pa_remark').val(item.NOTE_OF_PA);
            if (item.NUTRITION_ANALYSIS != "") {
                $("input[name=pa_rdo_customer_ingre][value=" + item.NUTRITION_ANALYSIS + "]").prop('checked', true);
            }
            if (item.CUSTOMER_BARCODE_SCANABLE != "") {
                $("input[name=pa_rdo_customer_barcode][value=" + item.CUSTOMER_BARCODE_SCANABLE + "]").prop('checked', true);
            }
            $(form_name + '.cls_txt_pa_customer_barcode').val(item.CUSTOMER_BARCODE_SCANABLE_OTHER);
            if (item.CUSTOMER_BARCODE_SPEC != "") {
                $("input[name=pa_rdo_customer_barcode_spec][value=" + item.CUSTOMER_BARCODE_SPEC + "]").prop('checked', true);
            }
            $(form_name + '.cls_txt_pa_barcode_spec').val(item.CUSTOMER_BARCODE_SPEC_OTHER);

            //Set value for Vendor tab
            $('.cls_vendor_txt_brand').val(item.BRAND_DISPLAY_TXT);
            $('.cls_vendor_txt_country').val(item.COUNTRY);
            if (item.PACK_SIZE_OTHER != "" && item.PACK_SIZE_OTHER != null) {
                $('.cls_vendor_txt_pack').val(item.PACK_SIZE_OTHER);
            }
            else {
                $('.cls_vendor_txt_pack').val(item.PACK_SIZE_DISPLAY_TXT);
            }
            if (item.PRIMARY_SIZE_OTHER != "" && item.PRIMARY_SIZE_OTHER != null) {
                $('.cls_vendor_txt_primary_size').val(item.PRIMARY_SIZE_OTHER);
            }
            else {
                $('.cls_vendor_txt_primary_size').val(item.PRIMARY_SIZE_DISPLAY_TXT);
            }
            if (item.STYLE_OF_PRINTING_OTHER != "" && item.STYLE_OF_PRINTING_OTHER != null) {
                $('.cls_vendor_txt_style_printing').val(item.STYLE_OF_PRINTING_OTHER);
            }
            else {
                $('.cls_vendor_txt_style_printing').val(item.STYLE_OF_PRINTING_DISPLAY_TXT);
            }

            //Set value for Customer tab
            $('.cls_cus_brand').val(item.BRAND_DISPLAY_TXT);
            $('.cls_cus_country').val(item.COUNTRY);

            var data_product_code_not_rd = [];
            for (var k = 0; k < data_product_code.length; k++) {
                if (data_product_code[k][0] != item.RD_REFERENCE_NO_DISPLAY_TXT) {
                    data_product_code_not_rd.push(data_product_code[k]);

                    if (productcode_ == "")
                        productcode_ = data_product_code[k][0];
                    else if (productcode_ != "")
                        productcode_ += "\n" + data_product_code[k][0];
                }
            }
            $('.cls_txt_product_code').val(productcode_);
            $('.table_customer_product_code').DataTable({
                "info": false,
                "searching": false,
                "lengthChange": false,
                "paging": false,
                "ordering": false,
                data: data_product_code_not_rd
            });

            if (item.RD_REFERENCE_NO_DISPLAY_TXT != null) {
                data_rd_ref.push([item.RD_REFERENCE_NO_DISPLAY_TXT, item.NET_WEIGHT_DISPLAY_TXT, item.DRAIN_WEIGHT_DISPLAY_TXT]);
                $('.cls_txt_rd_reference').val(item.RD_REFERENCE_NO_DISPLAY_TXT);
            }
            $('.table_customer_rd_ref').DataTable({
                "info": false,
                "searching": false,
                "lengthChange": false,
                "paging": false,
                "ordering": false,
                data: data_rd_ref
            });

            $('.cls_cus_pkg').val(item.MATERIAL_GROUP_DISPLAY_TXT);


            if (item.PACK_SIZE_OTHER != null && item.PACK_SIZE_OTHER != "") {
                $('.cls_cus_pack').val(item.PACK_SIZE_OTHER);
            }
            else {
                $('.cls_cus_pack').val(item.PACK_SIZE_DISPLAY_TXT);
            }
            if (item.PRIMARY_SIZE_OTHER != null && item.PRIMARY_SIZE_OTHER != "") {
                $('.cls_cus_primary_size').val(item.PRIMARY_SIZE_OTHER);
            }
            else {
                $('.cls_cus_primary_size').val(item.PRIMARY_SIZE_DISPLAY_TXT);
            }

            if (item.DECISION_DEFAULT != null)
                default_decision = item.DECISION_DEFAULT;
        } else {
            getPA_RDReference();
        }
        bindPA_SetData_FFC("");
       // bindMatDesc_ByMateiralRefMaterialTypeofBrand(); // by aof IGRID_REIM_SPLINT3
        is_first_load_pa = false;  // by aof 
    }
}

function setPA_ProductCodeRDReference(is_productcode) {
 
    if (is_productcode) {
        $('.cls_tfartwork_pa .col_pa_productcode1').show();
        $('.cls_tfartwork_pa .col_pa_productcode2').show();

        $('.cls_tfartwork_pa .col_pa_rd_reference1').hide();
        $('.cls_tfartwork_pa .col_pa_rd_reference2').hide();
        $('.cls_tfartwork_pa .col_netweight1').hide();
        $('.cls_tfartwork_pa .col_netweight2').hide();
        $('.cls_tfartwork_pa .col_drainweight1').hide();
        $('.cls_tfartwork_pa .col_drainweight2').hide();

    } else {
        $('.cls_tfartwork_pa .col_pa_productcode1').hide();
        $('.cls_tfartwork_pa .col_pa_productcode2').hide();

        $('.cls_tfartwork_pa .col_pa_rd_reference1').show();
        $('.cls_tfartwork_pa .col_pa_rd_reference2').show();
        $('.cls_tfartwork_pa .col_netweight1').show();
        $('.cls_tfartwork_pa .col_netweight2').show();
        $('.cls_tfartwork_pa .col_drainweight1').show();
        $('.cls_tfartwork_pa .col_drainweight2').show();
    }
}

function SubmitDataPop() {
    myAjaxConfirmSubmitBlank(callbackSubmitDataPop);
}


function checkRemarkLenghtOverThan4000(remark)
{
//ticket#472144  this function is writed by aof.
    var f_overthan4000 = false;

    if (!isEmpty(remark))
    {
        if (remark.length > 4000)
        {
            alertError2("Text is more than 4000 characters! ("+remark.length+") ");
            f_overthan4000 = true;
        }
    }


    return f_overthan4000;
}

//submit in pop up
function callbackSubmitDataPop() {
    var send = false;
    if ($('.cls_header_chk_pa[type="checkbox"]:checked').length > 0) {
        var jsonObj = new Object();
        var pa_submit_modal = "#pa_submit_modal ";

        if ($(pa_submit_modal + '.cls_chk_send_pg').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ASK_FOR_DIE_LINE"] = "X";

            //if ($(pa_submit_modal + '.cls_body_send_pg .cls_chk_assign_vn').is(":checked"))
            //    item["FOR_ASSIGN_VENDOR"] = "X"
            //else
            //    item["FOR_ASSIGN_VENDOR"] = "";
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_PG').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_PG').curr_step;
            item.PROCESS["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_pg .cls_lov_send_for_reason').val();
            item["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_pg .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pa_submit_modal + '.cls_body_send_pg .cls_input_pg_by_pa_other').val();
            item["WF_STEP"] = getstepartwork('SEND_PG').curr_step;
            item["IS_SENDER"] = true;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill('.cls_body_send_pg .cls_txt_send_pg');
            item.PROCESS["REMARK"] = editor.root.innerHTML;


            if (!checkRemarkLenghtOverThan4000(editor.root.innerHTML))  //ticket#472144  this function is writed by aof.
            {
                jsonObj.data = item;

                var myurl = '/api/taskform/pa/sendtopg';
                var mytype = 'POST';
                var mydata = jsonObj;

                if (send)
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, false, true);
                else {
                    send = true;
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, true, true);
                }
            }       
        }
        if ($(pa_submit_modal + '.cls_chk_send_mk').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_MK').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_MK').curr_step;
            item.PROCESS["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_mk .cls_lov_send_for_reason').val();
            item["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_mk .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pa_submit_modal + '.cls_body_send_mk .cls_input_mk_by_pa_other').val();
            item["WF_STEP"] = getstepartwork('SEND_MK').curr_step;
            item["IS_SENDER"] = true;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(pa_submit_modal + '.cls_body_send_mk .cls_txt_send_mk');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            if (!checkRemarkLenghtOverThan4000(editor.root.innerHTML))  //ticket#472144  this function is writed by aof.
            {
                jsonObj.data = item;

                var myurl = '/api/taskform/pa/sendtomk';
                var mytype = 'POST';
                var mydata = jsonObj;

                if (send)
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, false, true);
                else {
                    send = true;
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, true, true);
                }
            }
        }
        if ($(pa_submit_modal + '.cls_chk_send_qc').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            if ($(pa_submit_modal + '.cls_body_send_qc .cls_chk_qc_2').is(":checked"))
                item["NUTRITION"] = "X";
            else
                item["NUTRITION"] = "";
            if ($(pa_submit_modal + '.cls_body_send_qc .cls_chk_qc_3').is(":checked"))
                item["INGREDIENTS"] = "X";
            else
                item["INGREDIENTS"] = "";
            if ($(pa_submit_modal + '.cls_body_send_qc .cls_chk_qc_4').is(":checked"))
                item["ANALYSIS"] = "X";
            else
                item["ANALYSIS"] = "";
            if ($(pa_submit_modal + '.cls_body_send_qc .cls_chk_qc_5').is(":checked"))
                item["HEALTH_CLAIM"] = "X";
            else
                item["HEALTH_CLAIM"] = "";
            if ($(pa_submit_modal + '.cls_body_send_qc .cls_chk_qc_6').is(":checked"))
                item["NUTRIENT_CLAIM"] = "X";
            else
                item["NUTRIENT_CLAIM"] = "";
            if ($(pa_submit_modal + '.cls_body_send_qc .cls_chk_qc_7').is(":checked"))
                item["SPECIES"] = "X";
            else
                item["SPECIES"] = "";
            if ($(pa_submit_modal + '.cls_body_send_qc .cls_chk_qc_8').is(":checked"))
                item["CATCHING_AREA"] = "X";
            else
                item["CATCHING_AREA"] = "";
            if ($(pa_submit_modal + '.cls_body_send_qc .cls_chk_qc_9').is(":checked"))
                item["CHECK_DETAIL"] = "X";
            else
                item["CHECK_DETAIL"] = "";

            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_QC').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_QC').curr_step;
            item.PROCESS["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_qc .cls_lov_send_for_reason').val();
            item["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_qc .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pa_submit_modal + '.cls_body_send_qc .cls_input_qc_by_pa_other').val();
            item["WF_STEP"] = getstepartwork('SEND_QC').curr_step;
            item["IS_SENDER"] = true;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(pa_submit_modal + '.cls_body_send_qc .cls_txt_send_qc');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            if (!checkRemarkLenghtOverThan4000(editor.root.innerHTML))  //ticket#472144  this function is writed by aof.
            {
                //alert(item.PROCESS["REMARK"].length);

                jsonObj.data = item;

                var myurl = '/api/taskform/pa/sendtoqc';
                var mytype = 'POST';
                var mydata = jsonObj;

                if (send)
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, false, true);
                else {
                    send = true;
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, true, true);
                }
            }
        }
        if ($(pa_submit_modal + '.cls_chk_send_wh').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            if ($(pa_submit_modal + '.cls_body_send_wh .cls_chk_stamp').is(":checked"))
                item["INKJET_STAMP_AREA"] = "X";
            else
                item["INKJET_STAMP_AREA"] = "";
            if ($(pa_submit_modal + '.cls_body_send_wh .cls_chk_direction').is(":checked"))
                item["ROLL_DIRECTION"] = "X";
            else
                item["ROLL_DIRECTION"] = "";
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;


            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_WH').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_WH').curr_step;
            item.PROCESS["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_wh .cls_lov_send_for_reason').val();
            item["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_wh .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pa_submit_modal + '.cls_body_send_wh .cls_input_wh_by_pa_other').val();
            item["WF_STEP"] = getstepartwork('SEND_WH').curr_step;
            item["IS_SENDER"] = true;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(pa_submit_modal + '.cls_body_send_wh .cls_txt_send_wh');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            if (!checkRemarkLenghtOverThan4000(editor.root.innerHTML))  //ticket#472144  this function is writed by aof.
            {
                jsonObj.data = item;

                var myurl = '/api/taskform/pa/sendtowarehouse';
                var mytype = 'POST';
                var mydata = jsonObj;

                if (send)
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, false, true);
                else {
                    send = true;
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, true, true);
                }
            }        
        }
        if ($(pa_submit_modal + '.cls_chk_send_plan').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_PN').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_PN').curr_step;
            item.PROCESS["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_plan .cls_lov_send_for_reason').val();
            item["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_plan .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pa_submit_modal + '.cls_body_send_plan .cls_input_pn_by_pa_other').val();
            item["WF_STEP"] = getstepartwork('SEND_PN').curr_step;
            item["IS_SENDER"] = true;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(pa_submit_modal + '.cls_body_send_plan .cls_txt_send_plan');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            if (!checkRemarkLenghtOverThan4000(editor.root.innerHTML))  //ticket#472144  this function is writed by aof.
            {
                jsonObj.data = item;

                var myurl = '/api/taskform/pa/sendtoplanning';
                var mytype = 'POST';
                var mydata = jsonObj;

                if (send)
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, false, true);
                else {
                    send = true;
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, true, true);
                }
            }
        }
        if ($(pa_submit_modal + '.cls_chk_send_pp').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            if ($('input[name=pa_rdo_request_shade_limit]:checked').val() == 1)
                item["REQUEST_SHADE_LIMIT"] = "X";
            else if ($('input[name=pa_rdo_request_shade_limit]:checked').val() == 0)
                item["REQUEST_SHADE_LIMIT"] = "";
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_PP').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_PP').curr_step;
            item.PROCESS["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_pp .cls_lov_send_for_reason').val();
            item["REASON_ID"] = $(pa_submit_modal + '.cls_body_send_pp .cls_lov_send_for_reason').val();
            item["REMARK_REASON"] = $(pa_submit_modal + '.cls_body_send_pp .cls_input_pp_by_pa_other').val();
            item["WF_STEP"] = getstepartwork('SEND_PP').curr_step;
            item["IS_SENDER"] = true;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(pa_submit_modal + '.cls_body_send_pp .cls_txt_send_pp');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            if (!checkRemarkLenghtOverThan4000(editor.root.innerHTML))  //ticket#472144  this function is writed by aof.
            {
                jsonObj.data = item;

                var myurl = '/api/taskform/pa/sendtopp';
                var mytype = 'POST';
                var mydata = jsonObj;

                if (send)
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, false, true);
                else {
                    send = true;
                    myAjax(myurl, mytype, mydata, hide_modal_submit, '', true, true, true);
                }
            }
        }
    }
    else {
        $(".se-pre-con").fadeOut('fast');
        alertError2("Please select at least 1 item.");
    }
}
function hide_modal_submit() {
    $('#pa_submit_modal').modal('hide');

    resetDllReason('#pa_submit_modal .cls_lov_send_for_reason');
    $('#pa_submit_modal .cls_lov_search_file_template').val('').trigger('change');
    $('#pa_submit_modal textarea:enabled').val('');
    $('#pa_submit_modal input[type=checkbox]:enabled').prop('checked', false);
    $('#pa_submit_modal input[type=radio]:enabled').prop('checked', false);
    $('#pa_submit_modal .cls_body_send_pg').hide();
    $('#pa_submit_modal .cls_body_send_mk').hide();
    $('#pa_submit_modal .cls_body_send_qc').hide();
    $('#pa_submit_modal .cls_body_send_wh').hide();
    $('#pa_submit_modal .cls_body_send_plan').hide();
    $('#pa_submit_modal .cls_body_send_pp').hide();

    pg_quill = new Quill('#pa_submit_modal .cls_txt_send_pg');
    mk_quill = new Quill('#pa_submit_modal .cls_txt_send_mk');
    qc_quill = new Quill('#pa_submit_modal .cls_txt_send_qc');
    wh_quill = new Quill('#pa_submit_modal .cls_txt_send_wh');
    plan_quill = new Quill('#pa_submit_modal .cls_txt_send_plan');
    pp_quill = new Quill('#pa_submit_modal .cls_txt_send_pp');

    pg_quill.setContents([{ insert: '\n' }]);
    mk_quill.setContents([{ insert: '\n' }]);
    qc_quill.setContents([{ insert: '\n' }]);
    wh_quill.setContents([{ insert: '\n' }]);
    plan_quill.setContents([{ insert: '\n' }]);
    pp_quill.setContents([{ insert: '\n' }]);

    click_retrive_or_request_mat = true;
    bindTaskformArtwork(ArtworkSubId);
    bindDataTaskFormPA();
}

function SubmitDataPopSendtovendor() {
    myAjaxConfirmSubmitBlank(callbackSubmitDataPopSendtovendor, 'Vendor');
}

function callbackSubmitDataPopSendtovendor() {
    var send = false;
    if ($('.cls_header_chk_vendor[type="checkbox"]:checked').length > 0) {
        var jsonObj = new Object();
        var send_to_vendor_modal = "#send_to_vendor_modal ";


        if ($(send_to_vendor_modal + '.cls_chk_send_artwork_vendor').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["SEND_TO_VENDOR_TYPE"] = "REQ_PRINT_MASTER";
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_VN_PM').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_VN_PM').curr_step;

            var reason_ = $(send_to_vendor_modal + '.cls_body_send_artwork_vendor .cls_lov_send_for_reason').val();
            if ((reason_ == default_decision) && countvnpm == 1) {
                alertError2("Please select reason for revise");
                return false;
            }
            else
                item.PROCESS["REASON_ID"] = reason_;

            item["REASON_ID"] = reason_;
            item["REMARK_REASON"] = $(send_to_vendor_modal + '.cls_body_send_artwork_vendor .cls_input_vn_pm_by_pa_other').val();
            item["WF_STEP"] = getstepartwork('SEND_VN_PM').curr_step;
            item["IS_SENDER"] = true;

            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(send_to_vendor_modal + '.cls_body_send_artwork_vendor .cls_txt_send_artwork_vendor');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data = item;

            var myurl = '/api/taskform/pa/sendtovendor';
            var mytype = 'POST';
            var mydata = jsonObj;

            if (send)
                myAjax(myurl, mytype, mydata, hide_modal_send_vendor, '', true, false, true);
            else {
                send = true;
                myAjax(myurl, mytype, mydata, hide_modal_send_vendor, '', true, true, true);
            }

        }
        if ($(send_to_vendor_modal + '.cls_chk_send_shade').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["SEND_TO_VENDOR_TYPE"] = "REQ_SHADE_LIM";
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;


            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_ROLE_ID"] = getstepartwork('SEND_VN_SL').curr_role;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_VN_SL').curr_step;

            var reason_ = $(send_to_vendor_modal + '.cls_body_send_shade .cls_lov_send_for_reason').val();
            if ((reason_ == default_decision) && countvnsl == 1) {
                alertError2("Please select reason for revise");
                return false;
            }
            else
                item.PROCESS["REASON_ID"] = reason_;

            item["REASON_ID"] = reason_;
            item["REMARK_REASON"] = $(send_to_vendor_modal + '.cls_body_send_shade .cls_input_vn_sl_by_pa_other').val();
            item["WF_STEP"] = getstepartwork('SEND_VN_SL').curr_step;
            item["IS_SENDER"] = true;

            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(send_to_vendor_modal + '.cls_body_send_shade .cls_txt_send_shade');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data = item;

            var myurl = '/api/taskform/pa/sendtovendor';
            var mytype = 'POST';
            var mydata = jsonObj;

            if (send)
                myAjax(myurl, mytype, mydata, hide_modal_send_vendor, '', true, false, true);
            else {
                send = true;
                myAjax(myurl, mytype, mydata, hide_modal_send_vendor, '', true, true, true);
            }
        }

    }
    else {
        $(".se-pre-con").fadeOut('fast');
        alertError2("Please select at least 1 item.");
    }
}
function hide_modal_send_vendor() {
    $('#send_to_vendor_modal').modal('hide');

    resetDllReason('#send_to_vendor_modal .cls_lov_send_for_reason');
    $('#send_to_vendor_modal .cls_lov_search_file_template').val('').trigger('change');
    $('#send_to_vendor_modal textarea:enabled').val('');
    $('#send_to_vendor_modal input[type=checkbox]:enabled').prop('checked', false);
    $('#send_to_vendor_modal input[type=radio]:enabled').prop('checked', false);
    $('#send_to_vendor_modal .cls_body_send_artwork_vendor').hide();
    $('#send_to_vendor_modal .cls_body_send_shade').hide();

    vn_aw_quill = new Quill('#send_to_vendor_modal .cls_txt_send_artwork_vendor');
    vn_shade_quill = new Quill('#send_to_vendor_modal .cls_txt_send_shade');

    vn_aw_quill.setContents([{ insert: '\n' }]);
    vn_shade_quill.setContents([{ insert: '\n' }]);
}

function SubmitDataPopSendtocustomer() {
    myAjaxConfirmSubmitBlank(callbackSubmitDataPopSendtocustomer, 'Customer');
}

function callbackSubmitDataPopSendtocustomer() {
    var send = false;

    if ($('.cls_header_chk_customer[type="checkbox"]:checked').length > 0) {
        var jsonObj = new Object();
        var send_to_customer_modal = "#send_to_customer_modal ";


        if ($(send_to_customer_modal + '.cls_chk_send_review_customer').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ENDTASKFORM"] = false;
            item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REVIEW";

            if ($(send_to_customer_modal + '.cls_chk_formandlabelraw_customer').is(":checked"))
                item["IS_FORMLABEL"] = "X";
            if ($(send_to_customer_modal + '.cls_chk_qc_changedetails_customer').is(":checked"))
                item["IS_CHANGEDETAIL"] = "X";
            if ($(send_to_customer_modal + '.cls_chk_noncompliance_customer').is(":checked"))
                item["IS_NONCOMPLIANCE"] = "X";
            if ($(send_to_customer_modal + '.cls_chk_remark_adjustment_customer').is(":checked"))
                item["IS_ADJUST"] = "X";

            if ($(send_to_customer_modal + '.cls_chk_formandlabelraw_customer').is(":checked")) {
                if ($(send_to_customer_modal + '.cls_chk_qc_changedetails_customer').is(":checked")) {

                    var editor1 = new Quill(send_to_customer_modal + '.cls_body_qc_changedetails_customer .cls_remark_nutri_txt_comment_cus_pop');
                    if (editor1.root.innerHTML != "<p><br></p>")
                        item["NUTRITION_COMMENT"] = editor1.root.innerHTML;

                    var editor2 = new Quill(send_to_customer_modal + '.cls_body_qc_changedetails_customer .cls_remark_ingre_txt_comment_cus_pop');
                    if (editor2.root.innerHTML != "<p><br></p>")
                        item["INGREDIENTS_COMMENT"] = editor2.root.innerHTML;

                    var editor3 = new Quill(send_to_customer_modal + '.cls_body_qc_changedetails_customer .cls_remark_analysis_txt_comment_cus_pop');
                    if (editor3.root.innerHTML != "<p><br></p>")
                        item["ANALYSIS_COMMENT"] = editor3.root.innerHTML;

                    var editor4 = new Quill(send_to_customer_modal + '.cls_body_qc_changedetails_customer .cls_remark_health_txt_comment_cus_pop');
                    if (editor4.root.innerHTML != "<p><br></p>")
                        item["HEALTH_CLAIM_COMMENT"] = editor4.root.innerHTML;

                    var editor5 = new Quill(send_to_customer_modal + '.cls_body_qc_changedetails_customer .cls_remark_nutclaim_txt_comment_cus_pop');
                    if (editor5.root.innerHTML != "<p><br></p>")
                        item["NUTRIENT_CLAIM_COMMENT"] = editor5.root.innerHTML;

                    var editor6 = new Quill(send_to_customer_modal + '.cls_body_qc_changedetails_customer .cls_remark_species_txt_comment_cus_pop');
                    if (editor6.root.innerHTML != "<p><br></p>")
                        item["SPECIES_COMMENT"] = editor6.root.innerHTML;

                    var editor7 = new Quill(send_to_customer_modal + '.cls_body_qc_changedetails_customer .cls_remark_catching_txt_comment_cus_pop');
                    if (editor7.root.innerHTML != "<p><br></p>")
                        item["CATCHING_AREA_COMMENT"] = editor7.root.innerHTML;

                    var editor8 = new Quill(send_to_customer_modal + '.cls_body_qc_changedetails_customer .cls_remark_check_detail_txt_comment_cus_pop');
                    if (editor8.root.innerHTML != "<p><br></p>")
                        item["CHECK_DETAIL_COMMENT"] = editor8.root.innerHTML;

                    item["QC_COMMENT"] = $("#send_to_customer_modal  .cls_body_qc_changedetails_customer .cls_remark_comment_qc_cus_pop").text();
                }

                if ($(send_to_customer_modal + '.cls_chk_noncompliance_customer').is(":checked")) {
                    //var editornon = new Quill(send_to_customer_modal + '.cls_body_send_artwork_customer .cls_txt_send_artwork_customer');
                    //item.PROCESS["REMARK"] = editornon.root.innerHTML;
                    item["COMMENT_NONCOMPLIANCE"] = $(send_to_customer_modal + '.cls_body_noncompliance_customer .cls_remark_noncompliance_cus_pop').val();
                }
                item["COMMENT_FORM_LABEL"] = $(send_to_customer_modal + '.cls_body_formandlabelraw_customer .cls_remark_formandlabelraw_cus_pop').val();
            }
            if ($(send_to_customer_modal + '.cls_chk_remark_adjustment_customer').is(":checked")) {
                item["COMMENT_ADJUST"] = $(send_to_customer_modal + '.cls_body_adjustment_customer .cls_remark_adjustment_cus_pop').val();
            }


            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

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

            if (send)
                myAjax(myurl, mytype, mydata, hide_modal_send_customer, '', true, false);
            else {
                send = true;
                myAjax(myurl, mytype, mydata, hide_modal_send_customer, '', true, true);
            }

        }


        if ($(send_to_customer_modal + '.cls_chk_send_approve_print').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ENDTASKFORM"] = false;
            item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_PRINT";
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;


            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_CUS_PRINT').curr_step;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(send_to_customer_modal + '.cls_body_send_approve_print .cls_txt_send_print');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data = item;

            var myurl = '/api/taskform/pa/sendtocustomer';
            var mytype = 'POST';
            var mydata = jsonObj;

            if (send)
                myAjax(myurl, mytype, mydata, hide_modal_send_customer, '', true, false);
            else {
                send = true;
                myAjax(myurl, mytype, mydata, hide_modal_send_customer, '', true, true);
            }
        }

        if ($(send_to_customer_modal + '.cls_chk_send_req_ref').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_MK_VERIFY")
                item["ENDTASKFORM"] = true;
            else
                item["ENDTASKFORM"] = false;
            item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REQ_REF";
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;


            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_CUS_REQ_REF').curr_step;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(send_to_customer_modal + '.cls_body_send_req_ref .cls_txt_send_req_ref');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data = item;

            var myurl = '/api/taskform/pa/sendtocustomer';
            var mytype = 'POST';
            var mydata = jsonObj;

            if (send)
                myAjax(myurl, mytype, mydata, hide_modal_send_customer, '', true, false);
            else {
                send = true;
                myAjax(myurl, mytype, mydata, hide_modal_send_customer, '', true, true);
            }
        }
        if ($(send_to_customer_modal + '.cls_chk_send_approve_shade').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ENDTASKFORM"] = false;
            item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_SHADE";
            item["COURIER_NAME"] = $(send_to_customer_modal + '.cls_body_send_approve_shade .cls_courier_cus_pop').val();
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;


            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_CUS_SHADE').curr_step;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(send_to_customer_modal + '.cls_body_send_approve_shade .cls_txt_send_shade');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data = item;

            var myurl = '/api/taskform/pa/sendtocustomer';
            var mytype = 'POST';
            var mydata = jsonObj;

            if (send)
                myAjax(myurl, mytype, mydata, hide_modal_send_customer, '', true, false);
            else {
                send = true;
                myAjax(myurl, mytype, mydata, hide_modal_send_customer, '', true, true);
            }
        }
        if ($(send_to_customer_modal + '.cls_chk_send_shade_ref').is(":checked")) {
            jsonObj.data = {};
            var item = {};
            item.PROCESS = {};

            item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ENDTASKFORM"] = true;
            item["SEND_TO_CUSTOMER_TYPE"] = "REQ_CUS_REF";
            item["COURIER_NAME"] = $(send_to_customer_modal + '.cls_body_send_shade_ref .cls_courier_cus_pop').val();
            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;

            item.PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            item.PROCESS["ARTWORK_ITEM_ID"] = ARTWORK_ITEM_ID;
            item.PROCESS["PARENT_ARTWORK_SUB_ID"] = ArtworkSubId;
            item.PROCESS["CURRENT_STEP_ID"] = getstepartwork('SEND_CUS_REF').curr_step;
            item.PROCESS["CREATE_BY"] = UserID;
            item.PROCESS["UPDATE_BY"] = UserID;
            var editor = new Quill(send_to_customer_modal + '.cls_body_send_shade_ref .cls_txt_send_shade_ref');
            item.PROCESS["REMARK"] = editor.root.innerHTML;

            jsonObj.data = item;

            var myurl = '/api/taskform/pa/sendtocustomer';
            var mytype = 'POST';
            var mydata = jsonObj;

            if (send)
                myAjax(myurl, mytype, mydata, tohomeshaderef, '', true, false);
            else {
                send = true;
                myAjax(myurl, mytype, mydata, tohomeshaderef, '', true, true);
            }
        }

    }
    else {
        $(".se-pre-con").fadeOut('fast');
        alertError2("Please select at least 1 item.");
    }
}
function hide_modal_send_customer() {
    $('#send_to_customer_modal').modal('hide');

    $('#send_to_customer_modal .cls_lov_search_file_template').val('').trigger('change');
    $('#send_to_customer_modal textarea:enabled').val('');
    $('#send_to_customer_modal input[type=checkbox]:enabled').prop('checked', false);
    $('#send_to_customer_modal input[type=radio]:enabled').prop('checked', false);

    $('#send_to_customer_modal .cls_body_send_review_customer').hide();
    $('#send_to_customer_modal .cls_body_formandlabelraw_customer').hide();
    $('#send_to_customer_modal .cls_body_qc_changedetails_customer').hide();
    $('#send_to_customer_modal .cls_body_noncompliance_customer').hide();
    $('#send_to_customer_modal .cls_body_adjustment_customer').hide();
    $('#send_to_customer_modal .cls_body_send_req_ref').hide();
    $('#send_to_customer_modal .cls_body_send_approve_print').hide();
    $('#send_to_customer_modal .cls_body_send_approve_shade').hide();
    $('#send_to_customer_modal .cls_body_send_shade_ref').hide();

    cus_print_quill = new Quill('#send_to_customer_modal .cls_txt_send_print');
    cus_req_ref_quill = new Quill('#send_to_customer_modal .cls_txt_send_req_ref');
    cus_shade_quill = new Quill('#send_to_customer_modal .cls_txt_send_shade');
    cus_shade_ref_quill = new Quill('#send_to_customer_modal .cls_txt_send_shade_ref');

    cus_print_quill.setContents([{ insert: '\n' }]);
    cus_shade_quill.setContents([{ insert: '\n' }]);
    cus_req_ref_quill.setContents([{ insert: '\n' }]);
    cus_shade_ref_quill.setContents([{ insert: '\n' }]);

    if (countCus != 0) {
        $('#send_to_customer_modal .cls_chk_formandlabelraw_customer').prop('checked', true);
        $(send_to_customer_modal + ' .cls_body_formandlabelraw_customer').show();
        $('#send_to_customer_modal .cls_chk_qc_changedetails_customer').prop('checked', true);
        $(send_to_customer_modal + ' .cls_body_qc_changedetails_customer').show();
    }
}

function clearPAData() {

    $(".cls_tfartwork_pa .cls_lov_pa_copy_artworkno").val('').trigger("change"); ///475099 by aof 
    $(".cls_tfartwork_pa .cls_lov_pa_material_group").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_lov_pa_type_of").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_type_of").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_type_of_two").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_type_of_two").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_rd_reference_no").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_lov_pa_product_code").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_txt_pa_productcode").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_primary_size").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_primary_size").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_container_type").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_container_type").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_lid_type").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_lid_type").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_packing_style").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_packing_style").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_pack_size").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_pack_size").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_plant_register_no").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_plant_register_no").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_fao_zone").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_fao_zone").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_pms_colour").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_pms_colour").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_company_name").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_company_name").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_catching_area").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_catching_area").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_catching_method").val('').trigger("change");    // ticke#425737 added by aof 
    $(".cls_tfartwork_pa .cls_input_pa_catching_method").val('');                   // ticke#425737 added by aof 
    $(".cls_tfartwork_pa .cls_lov_pa_process_colour").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_process_colour").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_symbol").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_symbol").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_catching_period").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_catching_period").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_total_colour").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_total_colour").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_catching_method").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_catching_method").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_style_of_printing").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_style_of_printing").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_plant").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_plant").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_scientific_name").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_scientific_name").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_direction_of_sticker").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_direction_of_sticker").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_specie").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_specie").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_printing_style_of_primary").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_printing_style_of_primary").val('');
    $(".cls_tfartwork_pa .cls_lov_pa_printing_style_of_secondary").val('').trigger("change");
    $(".cls_tfartwork_pa .cls_input_pa_printing_style_of_secondary").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_package_quantity").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_customer_design").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_first_info").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_waste_percent").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_customer_spec").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_pic_mkt").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_courier_no").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_customer_size").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_production_expiry_date_system").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_customer_nominate").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_complete_infogroup").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_customer_nominate_colour").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_remark").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_customer_barcode").val('');
    $(".cls_tfartwork_pa .cls_txt_pa_barcode_spec").val('');
    $(".cls_tfartwork_pa .cls_input_pa_sec_pkg_production_plant").val('');
    $(".cls_tfartwork_pa .cls_lov_send_for_reason").val('');
    $(".cls_tfartwork_pa .cls_lov_send_for_reason").trigger("change");;
    $(".cls_tfartwork_pa .cls_input_pg_by_pa_other").val('');
    $(".cls_tfartwork_pa .cls_input_mk_by_pa_other").val('');
    $(".cls_tfartwork_pa .cls_input_qc_by_pa_other").val('');
    $(".cls_tfartwork_pa .cls_input_wh_by_pa_other").val('');
    $(".cls_tfartwork_pa .cls_input_pn_by_pa_other").val('');
    $(".cls_tfartwork_pa .cls_input_pp_by_pa_other").val('');

    $('input[name="pa_rdo_customer_design"]').prop('checked', false);
    $('input[name="pa_rdo_customer_spec"]').prop('checked', false);
    $('input[name="pa_rdo_customer_size"]').prop('checked', false);
    $('input[name="pa_rdo_customer_nominate"]').prop('checked', false);
    $('input[name="pa_rdo_customer_seriousness"]').prop('checked', false);
    $('input[name="pa_rdo_customer_nominate_colour"]').prop('checked', false);
    $('input[name="pa_rdo_customer_ingre"]').prop('checked', false);
    $('input[name="pa_rdo_customer_barcode"]').prop('checked', false);
    $('input[name="pa_rdo_customer_barcode_spec"]').prop('checked', false);

    $(".cls_tfartwork_pa .tr_tfartwork_pa_symbol_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_tfartwork_pa .cls_lov_pa_symbol_multiple_static").val('').trigger("change");

    $(".cls_tfartwork_pa .tr_tfartwork_pa_fao_zone_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_tfartwork_pa .cls_lov_pa_fao_zone_multiple_static").val('').trigger("change");

    $(".cls_tfartwork_pa .tr_tfartwork_pa_catching_area_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_tfartwork_pa .cls_lov_pa_catching_area_multiple_static").val('').trigger("change");

    $(".cls_tfartwork_pa .tr_tfartwork_pa_plant_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_tfartwork_pa .cls_lov_pa_plant_multiple_static").val('').trigger("change");

    // ticke#425737 added by aof 
    $(".cls_tfartwork_pa .tr_tfartwork_pa_catching_method_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_tfartwork_pa .cls_lov_pa_catching_method_multiple_static").val('').trigger("change");
    // ticke#425737 added by aof 

}

function selectOnlyThisCusReasonmodal(id) {
    for (var i = 1; i <= 2; i++) {
        if ("cus_reason_modal_rdo" + i === id && document.getElementById("cus_reason_modal_rdo" + i).checked === true) {
            document.getElementById("cus_reason_modal_rdo" + i).checked = true;
        } else {
            document.getElementById("cus_reason_modal_rdo" + i).checked = false;
        }
    }
}

function selectOnlyThisCusFormmodal(id) {
    for (var i = 1; i <= 2; i++) {
        if ("cus_form_modal_rdo" + i === id && document.getElementById("cus_form_modal_rdo" + i).checked === true) {
            document.getElementById("cus_form_modal_rdo" + i).checked = true;
        } else {
            document.getElementById("cus_form_modal_rdo" + i).checked = false;
        }
    }
}

function tohometerminate() {
    setTimeout(function () {
        document.location.href = suburl + "/";
    }, 1000);
    //pa_terminate_quill = new Quill('#modal_tfartwork_pa_terminate .cls_txtedt_tfartwork_pa_terminate_remark');
    pa_terminate_quill.setContents([{ insert: '\n' }]);
}

function tohomesendback() {
    //setTimeout(function () {
    //    document.location.href = suburl + "/";
    //}, 1000);
    $('#modal_tfartwork_pa_sendback').modal('hide');
    $('#modal_tfartwork_pa_sendback .cls_lov_marketing').empty();
    $('#modal_tfartwork_pa_sendback input[name=cls_rdo_pa_sendback][value=1]').prop('checked', true);
    //pa_sendback_quill = new Quill('#modal_tfartwork_pa_sendback .cls_txtedt_tfartwork_pa_sendback_remark');
    pa_sendback_quill.setContents([{ insert: '\n' }]);
}

function tohomeshaderef() {
    $('#send_to_customer_modal').modal('hide');
    setTimeout(function () {
        document.location.href = suburl + "/";
    }, 1000);

}
function DisplayProgressMessage(ctl) {
    var matStatus = $('.cls_txt_header_tfartwork_mat_status').val();
    if (matStatus=="")
        $('.cls_tfartwork_pa .cls_btn_pa_request_material').prop("disabled", ctl);
    if (matStatus == "Completed")
        $('.cls_tfartwork_pa .cls_btn_pa_update_material').prop("disabled", ctl);
    //$(".submit-progress").removeClass("hidden");
    return true;
}

 //by aof 202306 for CR#IGRID_REIM_SPLINT2----ADD NEW Function@
function confirmPackingStyleDifferentRequestForm()
{
    var f_confrim = true;
    var selections = $('.cls_tfartwork_pa .cls_lov_pa_packing_style').select2('data');
    var v = $('.cls_tfartwork_pa .cls_lov_pa_packing_style').val();
    if (selections.length > 0) {
        var packingSytle = "";
        var packingStyleRF = $('.cls_tfartwork_pa .cls_input_pa_packing_style_requestform').val();
        if (selections[0].ID != -1 && selections[0].ID != undefined) {
            packingSytle = selections[0].DISPLAY_TXT;
        } else if (defaultPA_PackingStyleData[v] != undefined) {
            packingSytle = selections[0].text;
        }


        var arrData = packingSytle.split(":");
        if (arrData.length > 0) {
            if (arrData[0] != packingStyleRF) {
                //if (packingSytle.indexOf(packingStyleRF) == -1) {
                if (confirm("Please confirm Packing Style PA(" + packingSytle + ") different Request form(" + packingStyleRF + ")?")) {
                    f_confrim = true
                } else {
                    f_confrim = false;
                }

            }
        }

    }


    return f_confrim
}


 //by aof 202306 for CR#IGRID_REIM----ADD NEW Function@
function bindPackingSizePopUp() {
    var serverSide = false;
    var where_criteria = "";
    var table_tfartwork_primary_size = $('#table_tfartwork_primary_size').DataTable();

    where_criteria = "&data.CODE=" + $('#modal_tfartwork_primary_size .cls_txt_primary_size_code').val()
        + "&data.CAN=" + $('#modal_tfartwork_primary_size .cls_txt_primary_size_can').val()
        + "&data.DESCRIPTION=" + $('#modal_tfartwork_primary_size .cls_txt_primary_size_can_desciption').val()
        + "&data.LIDTYPE=" + $('#modal_tfartwork_primary_size .cls_txt_primary_size_lid_type').val()
        + "&data.CONTAINERTYPE=" + $('#modal_tfartwork_primary_size .cls_txt_primary_size_container_type').val()
        + "&data.DESCRIPTIONTYPE=" + $('#modal_tfartwork_primary_size .cls_txt_primary_size_description_type').val();

    table_tfartwork_primary_size.destroy();
    table_tfartwork_suggest_material = $('#table_tfartwork_primary_size').DataTable({
        serverSide: false,
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
                url: suburl + "/api/lov/3p_primarysize_igrid?" + where_criteria,
                // url: suburl + "/api/taskform/pa/suggestmaterial/info?data.artwork_request_id=" + ARTWORK_REQUEST_ID + "&data.artwork_sub_id=" + ArtworkSubId + "&data.all_data=" + cls_chk_suggest_material_no_search_val + "&data.product_code_id=" + product_code_id, //ticket#437764 added by aof on 30/03/2021 PRODUCT_CODE_ID
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        fixedColumns: {
            leftColumns: 0
        },
        "scrollX": true,
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            { data: "CODE", "className": "cls_nowrap" },
            { data: "CAN", "className": "cls_nowrap" },
            { data: "DESCRIPTION", "className": "cls_nowrap" },
            { data: "LIDTYPE", "className": "cls_nowrap" },
            { data: "CONTAINERTYPE", "className": "cls_nowrap" },
            { data: "DESCRIPTIONTYPE", "className": "cls_nowrap cls_descriptiontype" },
            { data: "THREE_P_ID", "className": "cls_nowrap cls_three_p_id" },
        
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_descriptiontype').text(data.DESCRIPTIONTYPE);
            if (data.THREE_P_ID <= 0)
            {
                $(row).find('.cls_three_p_id').text("No data found.");
            }
        },
        order: [[1, 'asc']],
        initComplete: function (settings, json) {

        },
    });

}