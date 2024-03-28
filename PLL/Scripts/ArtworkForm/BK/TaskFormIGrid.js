var TypeOfText = "", fn = "", check_btn = false;
var alpha = "[ A-Za-z]";
var numeric = "[0-9]";
var alphanumeric = "[ A-Za-z0-9;]";
var BRAND_DISPLAY_TEXT = "";
var PRIMARY_SIZE_DISPLAY_TEXT = "";
var STYLE_OF_PRINTING_DISPLAY_TEXT = "";
var PACKING_STYLE_DISPLAY_TEXT = "";
var IsSaveCompleteInfoGroupAlready = false;
var eventSaveCompleteInfoGroup = false;
var IsStep_InfoGroup = false;
var IsApprove_step = false;
var IsStatusApp = 0;
$(document).ready(function () {


   
    $('.cls_txt_pa_plant_register_no').hide();
    $('.fileupload-buttonbar').hide();

    $('.cls_tfigrid_pg .cls_txt_pg_comment').val("");
    $('.cls_tfigrid .cls_txt_pa_comment').val("");
    typecheck_pa_vap = "load";
    $(".cls_container_taskform_igrid .cls_igrid_lbl_link_aw").hide();
    $('a[href="#view_pa"]').tab('show');
    //bindSuggestPackingStylePopUp(false, "0");
    $('.cls_container_taskform_igrid .cls_lbl_pa_packing_style').html('<span style=\"color: red;\">Packing style </span> :');
    $('.cls_container_taskform_igrid .cls_lbl_pa_pack_size').html("Pack size :");
    //bind_lov_param('.cls_tfigrid', '/api/lov/artworkno', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    //bind_lov_param('.cls_tfigrid .cls_lov_pa_type_of', '/api/lov/pa/typeof', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_type_of');
    //bind_lov('.cls_tfigrid .cls_lov_pa_packing_style', '/api/lov/PackStyleIGrid','data.RefStyle');
    
    bind_lov('.cls_container_taskform_igrid .cls_lov_assign', '/api/lov/AssignIGrid', 'data.DISPLAY_TXT');
    //bind_lov('.cls_tfigrid .cls_lov_pa_material_group', '/api/lov/packtypeIGrid', 'data.DISPLAY_TXT');
    bind_lov_confirm('.cls_tfigrid .cls_lov_pa_material_group', '/api/lov/packtypeIGrid', 'data.DISPLAY_TXT', '', 'Are you sure want to change material group?', getIGridPA_MatGroupValue);

    bind_lov('.cls_tfigrid .cls_lov_brand', '/api/lov/brandIGrid2', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_tfigrid .cls_lov_pa_pms_colour', '/api/lov/PMSColourIGrid', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_tfigrid .cls_lov_pa_process_colour', '/api/lov/processcolourIGrid', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_tfigrid .cls_lov_pa_total_colour', '/api/lov/totalcolourIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);

    bind_lov_param('.cls_tfigrid .cls_lov_pa_type_of', '/api/lov/TypeOfIGrid?data.MaterialType=0', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_tfigrid .cls_lov_pa_type_of_two', '/api/lov/typeof2IGrid?data.MaterialType=2', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_tfigrid .cls_lov_pa_style_of_printing', '/api/lov/StyleofPrintingIGrid', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    PA_ProductType = 'X';

    //bind_lov_param('.cls_tfigrid .cls_lov_pa_plant_register_no', '/api/lov/PlantRegisteredIGrid', 'data.DISPLAY_TXT',['plant'], ['.cls_txt_pa_plant_multiple_static']);
    //bind_lov('.cls_tfigrid .cls_lov_pa_company_name', '/api/lov/CompanyAddressIGrid', 'data.DISPLAY_TXT');
    //bind_lov_param('.cls_tfigrid .cls_lov_pa_pms_colour', '/api/lov/pa/pms', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_pms_colour');
    //bind_lov_param('.cls_tfigrid .cls_lov_pa_process_colour', '/api/lov/pa/processcolour', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_process_colour');
    //bind_lov_param('.cls_tfigrid .cls_lov_pa_total_colour', '/api/lov/pa/totalcolour', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_total_colour');
    bind_lov_param('.cls_tfigrid .cls_lov_pa_typeofprimary', '/api/lov/TypeofPrimaryIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    //bind_lov('.cls_tfigrid .cls_lov_pa_typeofprimary', '/api/lov/TypeofPrimaryIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid .cls_lov_pa_catching_period', '/api/lov/CatchingPeriodIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid .cls_lov_pa_scientific_name', '/api/lov/ScientificNameIGrid', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_tfigrid .cls_lov_pa_direction_of_sticker', '/api/lov/DirectionIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    bind_lov('.cls_tfigrid .cls_lov_pa_specie', '/api/lov/SpecieIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid .cls_lov_pa_printing_style_of_primary', '/api/lov/pa/printingstyleofprimary', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_tfigrid .cls_lov_pa_printing_style_of_secondary', '/api/lov/PrintingSystemIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);

    bind_lov('.cls_lov_pg_packaging_type', '/api/lov/packtype', 'data.DISPLAY_TXT');
    $('.cls_container_taskform_igrid .cls_chk_Fixed_Desc').change(function () {
        //debugger;
        var isCheckFixed_Desc = $('.cls_container_taskform_igrid .cls_chk_Fixed_Desc').is(":checked");
        if (isCheckFixed_Desc == false) {
            get_desacription_text();
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').val("");
            setcal();
        }     
    });
    $('.cls_container_taskform_igrid .cls_chk_Fixed_Desc').click(function () {
        var isCheckFixed_Desc = $('.cls_container_taskform_igrid .cls_chk_Fixed_Desc').is(":checked");
        $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').prop('disabled', isCheckFixed_Desc ? false : true);
    });
    
    $('.cls_tfigrid .cls_lov_pa_company_name').on("change", function () {
        var fg = $(".cls_tfigrid .cls_txt_pa_product_code").val();
        var selections = $(this).select2('data');
        var RegisteredNo = $('.cls_tfigrid .cls_lov_pa_plant_register_no option:selected').text();
        if (selections.length > 0) {
            if (selections[0].text == "NO" && RegisteredNo == "NO") {
                var myurl = '/api/taskform/igrid/ProductCodeIGrid?data.PRODUCT_CODE=' + fg + "&data.Address=" + selections[0].text + "&data.RegisteredNo=" + RegisteredNo;
                var mytype = 'GET';
                var mydata = null;
                myAjax(myurl, mytype, mydata, callback_getprd_plant);


            } else {
                get_Production_plant();
            }
        }
    });
    $('.cls_lov_pg_packaging_type').on("change", function () {
        $('.cls_lov_pg_style').val('').trigger("change");
        $('.cls_lov_pg_pim_color').val('').trigger("change");
    });

    bind_lov_param('.cls_lov_pkg_sec_certification_standard', '/api/lov/SustainCertSourcingIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_tfigrid .cls_lov_pa_material_group']);
    bind_lov_param('.cls_lov_pg_style', '/api/lov/style', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type']);
    bind_lov('.cls_lov_pg_pim', '/api/lov/printsystem', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_lov_pg_pim_color', '/api/lov/numberofcolor', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type']);
    bind_lov('.cls_lov_pg_box_color', '/api/lov/boxcolor', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_pg_coating', '/api/lov/coating', 'data.DISPLAY_TXT');

    bind_lov_param('.cls_tfigrid_pg .cls_lov_printing_system', '/api/lov/PrintingSystemIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_lov_grade_of', '/api/lov/GradeofIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_tfigrid .cls_lov_pa_material_group']);
    bind_lov_param('.cls_lov_rsc_di_cut', '/api/lov/dicutIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_tfigrid .cls_lov_pa_material_group']);
    bind_lov_param('.cls_lov_roll_sheet', '/api/lov/RollSheet', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_tfigrid .cls_lov_pa_material_group']);
    bind_lov('.cls_lov_vendor', '/api/lov/vendorhasuser', 'data.vendor_name');
    bind_lov('.cls_lov_pkg_sec_plastic', '/api/lov/SustainPlasticIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid_pg .cls_lov_pkg_sec_material', '/api/lov/SustainMaterialIGrid', 'data.DISPLAY_TXT');
    //bind_lov_param('.cls_lov_pa_printing_style_of_secondary', '/api/lov/SustainCertSourcingIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    
    $('.cls_tfigrid_pg .cls_lov_vendor').prop('disabled', true);
    bind_lov_param('.cls_lov_flute', '/api/lov/FluteIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    setRequireFieldTaskformIGridPA($('.cls_tfigrid .cls_lov_pa_material_group').val());


    $('.cls_tfigrid .cls_lov_brand').on("change", function () {
        setcal();
    });
    $('input[type=radio][name=cls_rdo_haeder_tfartwork_change_point]').change(function () {
        setcal();
    });
    $('.cls_tfigrid .cls_lov_pa_material_group').on("change", function () {
        //setRequireFieldTaskformIGridPA($('.cls_tfigrid .cls_lov_pa_material_group').val());

        //$(".cls_tfigrid .cls_lov_pa_type_of").val('').trigger("change");

        //$(".cls_tfigrid .cls_lov_pa_type_of_two").val('').trigger("change");

       // get_required_field();

      

    });
    $('.cls_tfigrid .cls_lov_pa_type_of').on("change", function () {
        get_desacription_text();
    });


    $('.cls_tfigrid .cls_lov_pa_style_of_printing').on("change", function () {
        setRequireFieldTaskformIGridPA($('.cls_tfigrid .cls_lov_pa_material_group').val());
    });

    $('.cls_tfigrid .cls_lov_brand').on("change", function () {
        var selections = $(this).select2('data');
       // $(this).addClass('borderRed')

        if (BRAND_DISPLAY_TEXT != selections[0].text) {
            $('.cls_lbl_brand').text(BRAND_DISPLAY_TEXT);
        } else
        {
            $('.cls_lbl_brand').text('');
        }

        get_desacription_text();
      
    });



    $(".cls_tfigrid .cls_txt_pa_product_code").on("change", function () {
        var fg = $(".cls_tfigrid .cls_txt_pa_product_code").val();
        //alertError2('fg');      
        getPakcingStyleByFG(fg);
        getproduct_code(fg);
        //debugger;
      
    });

   // $('.cls_tfigrid .cls_txt_pa_first_info').addClass('borderRed');
    //$('.cls_ddl_option_condition').addClass('borderRed');
   // bindIGridUser();
    bindDataIGridSAPMaterial();
    bindDataIGridHistory();
    bindDataAttachIGird();
    //$(document).on('click', '#TableId_igrid_pa_primary .cls_chk_sendtopp', function (e) {
    //    if ($(this).is(':checked')) {
    //        TableId_igrid_pa_primary.rows($(this).closest('tr')).select();
    //    }
    //    else {
    //        TableId_igrid_pa_primary.rows($(this).closest('tr')).deselect();
    //    }
    //    $(this).closest('tr').find('.cls_chk_sendtopp').prop('checked', this.checked);
    //});


    //$(".cls_container_taskform_igrid .cls_txt_header_tfartwork_aw_no").click(function () {
    //    alert('clicked!');
    //});






    $(".cls_tfigrid_pg .cls_chk_Complete_InfoGroup").change(function () {
        if (this.checked) {
            //Do stuff
            eventSaveCompleteInfoGroup = true;
        }
    });


    $(".cls_tfigrid_pg .cls_btn_add_pg_accessories").click(function () {
     
        if ($(".cls_tfigrid_pg .cls_txt_acessories").val() == null || $(".cls_tfigrid_pg .cls_txt_acessories").val() == "" ) {
            alertError2('Please key accessories');
        } else if ($(".cls_tfigrid_pg .cls_txt_acessories").val() != "") {
              
            var accessories = $('.cls_tfigrid_pg .cls_txt_acessories').val();
            if ($('#btn_pg_accessoreis').text() == "Add") {
                addAccessoriesToTable(accessories);
            } else
            {
                editAccessoriesToTable(accessories);
            }

            $('.cls_tfigrid_pg .cls_txt_acessories').val('');
        }
    });

    $("#modal_igrid_pa_plant .cls_btn_igrid_pa_plant_submit").click(function (e) {
        //debugger;
        var table = $('#TableId_igrid_pa_plant').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            const array = [];
            for (i = 0; i < tblData.length; i++) {
                //var three_p_id = tblData[i].Id;
                array.push(tblData[i].Id);
            }
            
            var _a = array.join(';');
            $('.cls_container_taskform_igrid .cls_txt_pa_plant_multiple_static').val(_a);
           
            $("#modal_igrid_pa_plant .cls_btn_igrid_pa_plant_close").click();
        } else {
            alertError2("Please select at least 1 item.");
        }

    });
    $("#modal_igrid_pg_vendor .cls_btn_igrid_pg_vendor_submit").click(function (e) {
        //debugger;
        var table = $('#TableId_igrid_pg_vendor').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            const array = [];
            const array2 = [];
            for (i = 0; i < tblData.length; i++) {
                //var three_p_id = tblData[i].Id;
                array.push(tblData[i].Code);
                array2.push(tblData[i].Code + "-" + tblData[i].Name);
            }

            var _a = array.join(';');
            var _a2 = array2.join(';');
            $('.cls_tfigrid_pg .cls_txt_pg_vendor_multiple_static').val(_a);
            $('.cls_tfigrid_pg .cls_lbl_pg_vendor_name').text(_a2);

            $("#modal_igrid_pg_vendor .cls_btn_igrid_pg_vendor_close").click();
        } else {
            alertError2("Please select at least 1 item.");
        }

    });
    
    $("#modal_igrid_pa_primary .cls_btn_igrid_pa_terminate_submit").click(function (e) {
        //debugger;
        var table = $('#TableId_igrid_pa_primary').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            //var three_p_id = tblData[0].Id;
            var primary_size = tblData[0].Code + ":" + tblData[0].Description;

            if (primary_size.toLowerCase() != PRIMARY_SIZE_DISPLAY_TEXT.toLowerCase()) {
                $('.cls_lbl_primary_size').text(PRIMARY_SIZE_DISPLAY_TEXT);
            } else
            {
                $('.cls_lbl_primary_size').text('');
            }

            $('.cls_container_taskform_igrid .cls_txt_pa_primary_size_id').val(tblData[0].Id);
            $('.cls_container_taskform_igrid .cls_txt_pa_primary_size').val(primary_size);
            //$('.cls_container_taskform_igrid .cls_txt_pa_primary_size').val(tblData[0].Can + ":" + tblData[0].Description);
            $('.cls_container_taskform_igrid .cls_txt_pa_container_type').val(tblData[0].ContainerType);
            //$('.cls_container_taskform_igrid .cls_txt_pa_lid_type').val(tblData[0].LidType + ":" + tblData[0].DescriptionType);
            $('.cls_container_taskform_igrid .cls_txt_pa_lid_type').val(tblData[0].DescriptionType);
            $("#modal_igrid_pa_primary .cls_btn_igrid_pa_terminate_close").click();
            setcal();
        } else {
            alertError2("Please select at least 1 item.");
        }
    
    });
    $('#modal_igrid_pa_catching_area .cls_btn_igrid_pa_catching_area_submit').click(function (e) {
        var table = $('#TableId_igrid_pa_catching_area').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
        const array = [];
        for (i = 0; i < tblData.length; i++) {
            array.push(tblData[i].Description);
        }
            var _a = array.join(';');
            $('.cls_tfigrid .cls_txt_pa_catching_area_multiple_static').val(_a);

            $("#modal_igrid_pa_catching_area .cls_btn_igrid_pa_catching_area_close").click();
        } else {
            alertError2("Please select at least 1 item.");
        }
    });
    $('#modal_igrid_pa_packing_style .cls_btn_igrid_pa_packing_style_submit').click(function (e) {
        var table = $('#TableId_igrid_pa_packing_style').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            var three_p_id = tblData[0].Id;
            //
            $('.cls_tfigrid .cls_txt_pa_packing_style').val(tblData[0].RefStyle);
            $('.cls_tfigrid .cls_txt_pa_pack_size').val(tblData[0].PackSize);
            $("#modal_igrid_pa_packing_style .cls_btn_igrid_pa_packing_style_close").click();

            setRequireFieldTaskformIGridPA($('.cls_tfigrid .cls_lov_pa_material_group').val());
        } else {
            alertError2("Please select at least 1 item.");
        }
    });

    $("#modal_igrid_pa_catching_method .cls_btn_igrid_pa_catching_method_submit").click(function (e) {
        //debugger;
        var table = $('#TableId_igrid_pa_catching_method').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            const array = [];
            for (i = 0; i < tblData.length; i++) {
                array.push(tblData[i].Description);
            }
            var _a = array.join(';');
            $('.cls_tfigrid .cls_txt_pa_catching_method_multiple_static').val(_a);
            $("#modal_igrid_pa_catching_method .cls_btn_igrid_pa_catching_method_close").click();
        } else {
            alertError2("Please select at least 1 item.");
        }
    });
    $("#modal_igrid_pa_symbol .cls_btn_igrid_pa_symbol_submit").click(function (e) {
        //debugger;
        var table = $('#TableId_igrid_pa_symbol').DataTable(); //.search('').columns().search('').draw();



        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            const array = [];
            for (i = 0; i < tblData.length; i++) {
                array.push(tblData[i].Description);
            }
            var _a = array.join(';');
            $('.cls_tfigrid .cls_txt_pa_symbol_multiple_static').val(_a);
            $("#modal_igrid_pa_symbol .cls_btn_igrid_pa_symbol_close").click();
        } else {
            alertError2("Please select at least 1 item.");
        }
    });
    $("#modal_igrid_pa_faozone .cls_btn_igrid_pa_faozone_submit").click(function (e) {
        //debugger;
        var table = $('#TableId_igrid_pa_faozone').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            const array = [];
            for (i = 0; i < tblData.length; i++) {
                array.push(tblData[i].Description);
            }
            var _a = array.join(';');
            $('.cls_tfigrid .cls_txt_pa_fao_zone_multiple_static').val(_a);
            $("#modal_igrid_pa_faozone .cls_btn_igrid_pa_faozone_close").click();
        } else {
            alertError2("Please select at least 1 item.");
        }
    });
    $(".cls_tfigrid .cls_btn_pa_fao_zone_multiple_static").click(function (e) {
        var default_value = $('.cls_tfigrid .cls_txt_pa_fao_zone_multiple_static').val();
        bindSuggestFAOZonePopUp(false, "1", suburl + "/api/lov/FAOZoneIGrid", '#TableId_igrid_pa_faozone', default_value);
        $('#modal_igrid_pa_faozone').modal({ backdrop: 'static', keyboard: true });
    });

    $(".cls_tfigrid .cls_btn_pa_symbol_multiple_static").click(function (e) {
        $('#modal_igrid_pa_symbol .modal-title').html("Symbol");
        var default_value = $('.cls_tfigrid .cls_txt_pa_symbol_multiple_static').val();
        bindSuggestSymbolPopUp(false, "1", default_value);
        $('#modal_igrid_pa_symbol').modal({ backdrop: 'static', keyboard: true });
    });
   
    $(".cls_tfigrid .cls_btn_pa_catching_method_multiple_static").click(function (e) {
        var default_value = $('.cls_tfigrid .cls_txt_pa_catching_method_multiple_static').val();
        bindSuggestCatchingMethodPopUp(false, "1", default_value);
        $('#modal_igrid_pa_catching_method').modal({ backdrop: 'static', keyboard: true });
    });
    $(".cls_tfigrid_pg .cls_btn_pg_vendor_multiple_static").click(function (e) {
        var default_value = $('.cls_tfigrid_pg .cls_txt_pg_vendor_multiple_static').val();
        bindSuggestVendorPopUp(false, "1", default_value);
        $('#modal_igrid_pg_vendor').modal({ backdrop: 'static', keyboard: true });
    });

    $(".cls_tfigrid .cls_btn_pa_plant_multiple_static").click(function (e) {
        url = suburl + "/api/lov/PlantIGrid";
        var default_value = $('.cls_tfigrid .cls_txt_pa_plant_multiple_static').val();
        bindSuggestPlantPopUp(false, "1", url, '#TableId_igrid_pa_plant', default_value);
        $('#modal_igrid_pa_plant').modal({ backdrop: 'static', keyboard: true });
    });
    $(".cls_tfigrid .cls_btn_pa_catching_area_multiple_static").click(function (e) {
        url = suburl + "/api/lov/CatchingAreaIGrid";
        var default_value = $('.cls_tfigrid .cls_txt_pa_catching_area_multiple_static').val();
        bindSuggestCatchingAreaPopUp(false, "1", url, '#TableId_igrid_pa_catching_area', default_value);
        $('#modal_igrid_pa_catching_area').modal({ backdrop: 'static', keyboard: true });
    });
    $(".cls_tfigrid .cls_btn_pa_packing_style").click(function (e) {
        //debugger;
        var pa_typeofprimary = $('.cls_tfigrid .cls_lov_pa_typeofprimary').text();
        var default_value = $('.cls_tfigrid .cls_txt_pa_packing_style').val();

        if (!isEmpty(pa_typeofprimary)) {
            //TableId_igrid_pa_packing_style.destroy();
            //TableId_igrid_pa_packing_style.ajax.reload();
            bindSuggestPackingStylePopUp(false, "1", default_value);
         
            $('#modal_igrid_pa_packing_style').modal({ backdrop: 'static', keyboard: true });
        }
        else {
            alertError2("Please input request TypeofPrimary.");
        }
    });
    
    $('.cls_tfigrid_pg .cls_btn_reject').click(function (e) {

        saveActive('PG_Approve', 5, 'Reject', '/api/taskform/igrid/saveActive');
    });
    $('.cls_tfigrid .cls_btn_pa_remove').click(function (e) {
        var result = confirm("Do you want delete this job?");
        if (result) {
            saveActive('PA_Input', 1, 'Delete', '/api/taskform/igrid/Delete_UnusedJob');
        }
    });
    $('.cls_tfigrid .cls_btn_pa_reject').click(function (e) {
        saveActive('PA_Approve', 5, 'Reject', '/api/taskform/igrid/saveActive');
    });
    $('.cls_tfigrid .cls_btn_pa_submit').click(function (e) {

        if (validateFieldSubmit_PA())
        {
            //debugger;
            if ($('#cls_btn_pa_submit').html() == 'Submit')
                saveActive('PA', 1, 'Approve', '/api/taskform/igrid/saveActive');
            if ($('#cls_btn_pa_submit').html() == 'Approve')
                saveActive('PA_Approve', 1, 'Approve', '/api/taskform/igrid/saveActive');
        }
    });
    $('.cls_tfigrid_pg .cls_btn_submit').click(function (e) {
        //debugger;
        if ($('#cls_btn_pg_submit').html() == 'Submit')
            saveActive('PG', 1, 'Approve', '/api/taskform/igrid/saveActive');
        if ($('#cls_btn_pg_submit').html() == 'Approve')
            saveActive('PG_Approve', 1, 'Approve', '/api/taskform/igrid/saveActive');
    });
    $('.cls_tfigrid_pg .cls_btn_assign').click(function (e) {
        if ($('.cls_container_taskform_igrid .cls_lov_assign').val() == null)
        {
            alertError2("Please input assignee (PG)!")
            return false;
        }
        saveActive('PG_Assign', 1, 'Assign','/api/taskform/igrid/assign');
    });
    $('.cls_tfigrid_pg .cls_btn_save').click(function (e) {
        if ($('.cls_tfigrid_pg .cls_lov_grade_of').val() == null) {
            alertError2("Please input Grade Of!!!")
            return false;
        }


        //if (IsStep_InfoGroup == true && $(".cls_tfigrid_pg .cls_chk_Complete_InfoGroup").is(":checked") == false)
        //{
        //    alertError2("Please Check Complete InfoGroup.")
        //    return false;
        //}


        check_btn = true;

        if (IsStep_InfoGroup == true) {


            //if ($(".cls_tfigrid_pg .cls_chk_Complete_InfoGroup").is(":checked") == true) {
                saveIGridCompleteInfoGroup(true);
            //}


        } else
        {
            saveIGridSAPMaterial(true);
        }


    });


    

    $('.cls_tfigrid .cls_btn_pa_save').click(function (e) {
        //if (validateFieldSave()) // edited by aof
        //{
            check_btn = true;
            saveIGridSAPMaterial(true);
        //}
    });
    $(".cls_tfigrid .cls_btn_pa_primary_size").click(function (e) {

        var default_value = $('.cls_tfigrid .cls_txt_pa_primary_size').val();

        url = suburl + "/api/lov/primarysizeIGrid";
        bindSuggestPrimarySizePopUp(false, "1", url, '#TableId_igrid_pa_primary', '', default_value);
        //table.clear().destroy();
        //builddata(url,table);
        
        $('#modal_igrid_pa_primary').modal({ backdrop: 'static', keyboard: true });
        //table_tfartwork_suggest_material = $('#TableId_igrid_pa_primary').DataTable({
        //    serverSide: serverSide,
        //    ajax: function (data, callback, settings) {
        //        if (serverSide) {
        //            for (var i = 0, len = data.columns.length; i < len; i++) {
        //                delete data.columns[i].name;
        //                delete data.columns[i].data;
        //                delete data.columns[i].searchable;
        //                delete data.columns[i].orderable;
        //                delete data.columns[i].search.regex;
        //                delete data.search.regex;

        //                delete data.columns[i].search.value;
        //            }
        //        }
        //        $.ajax({
        //            url: suburl + "/api/taskform/igrid/primary",
        //            type: 'GET',
        //            data: data,
        //            success: function (res) {
        //                dtSuccess(res, callback);
        //            }
        //        });
        //    },
        //    fixedColumns: {
        //        leftColumns: 1
        //    },
        //    "scrollX": true,
        //    columns: [
        //        {
        //            data: null,
        //            defaultContent: '',
        //            className: 'select-checkbox',
        //            orderable: false
        //        },
        //        { data: "Id", "className": "cls_nowrap" },
        //        { data: "Code", "className": "cls_nowrap" },
        //        { data: "Can", "className": "cls_nowrap" },
        //        { data: "Description", "className": "cls_nowrap" },
        //        { data: "ContainerType", "className": "cls_nowrap" },
        //        { data: "LidType", "className": "cls_nowrap" }
        //        ],
        //    select: {
        //        style: 'os',
        //        selector: 'td:first-child'
        //    },
        //    "processing": true,
        //    "rowCallback": function (row, data, index) {
        //    //    if (!isEmpty(data.START_DATE)) {
        //    //        $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
        //    //    }
        //    //    if (!isEmpty(data.END_DATE)) {
        //    //        $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
        //    //    }
        //    },
        //    order: [[2, 'asc']],
        //    initComplete: function (settings, json) {

        //    },
        //});
        //bindSalesOrderViewChangePopUp();
        //$('#modal_tfartwork_salesorder_viewchange').modal({
        //    backdrop: 'static',
        //    keyboard: true
        //});
    });



    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_history':
                bindDataIGridHistory();
                //if (table_artwork_history == null)
                //    bindDataIGridHistory();
                //else {
                //    countvnpm = 0; countvnsl = 0;
                //    table_artwork_history.ajax.reload();
                //}
                break;
            default:
                break;
        }
    });


    $(".cls_txt_pkg_sec_recycled_content").blur(function () {

        var val = $(".cls_txt_pkg_sec_recycled_content").val();
        if (val > 100) {
            alertError2("Packaging recycled content must be 0-100.");
            $(".cls_txt_pkg_sec_recycled_content").focus();
            $(".cls_txt_pkg_sec_recycled_content").select();

        }
        else {
            $(".cls_txt_pkg_sec_recycled_content").val(setStringToNumber(val, 0));
        }

    });

    $('.cls_txt_pkg_sec_recycled_content').keypress(function (event) {
        return isKeyNumber(event, this)
    });

    //$('.cls_txt_pa_product_code').keypress(function (event) {
    //    return isKeyNumber(event, this)
    //});

    $(".cls_txt_pkg_sec_weight").blur(function () {
        var val = $(".cls_txt_pkg_sec_weight").val();

        if (setStringToNumber(val) >= 1000000000000) {
            alertError2("Packaging weight entered exceeds the maximum length or not in a properly formatted (999,999,999,999.999).");
            $(".cls_txt_pkg_sec_weight").focus();
            $(".cls_txt_pkg_sec_weight").select();
        } else {
            $(".cls_txt_pkg_sec_weight").val(setNumberToStringFormat(val, 3));
        }

        // $(".cls_txt_pkg_sec_weight").val(setStringToNumber(weg));
    });
    $('.cls_txt_pkg_sec_weight').keypress(function (event) {
        return isKeyDeicmal(event, this)
    });



});


function validateFieldSave() // by aof
{

    if ($('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').val().length > 40) {
        alertError2("Description can't more than 40 digits please change!!!");
        return false;
    }


    if ($('.cls_tfigrid .cls_lov_pa_type_of_two').val() == null) {
        // alertError2("Please input type of " + $('.cls_tfigrid .cls_lov_pa_material_group').val());

        var matgroup = $('.cls_tfigrid .cls_lov_pa_material_group').val();

        switch (matgroup) {
            case "N":
            case "R":
            case "F":
            //case "H":
            case "G":
            case "C":
            case "D":
                alertError2("Please input type of 2");
                return false;
        }


    }
    //If Left(txtSAPCode.text, 2) = "5K" Or Left(txtSAPCode.text, 2) = "5L" Or Left(txtSAPCode.text, 2) = "5P" Or Left(txtSAPCode.text, 2) = "5J" Then
    var matgroup = $('.cls_tfigrid .cls_lov_pa_material_group').val();
    if (matgroup == "K" || matgroup == "L" || matgroup == "P" || matgroup == "J") {
    } else {
        if ($('.cls_tfigrid .cls_txt_pa_packing_style').val() != $(".cls_tfigrid .cls_lbl_pa_packing_style_fg").html()) {
            //debugger;
            var result = confirm("This Packing Style conflick with Packing Style from SAP. \n Do you want to continue? \n [Click 'OK' to skip and accept this Packing Style] or \n [Click 'Cancel' for back to recheck/change]");
            if (result == false) {
                return false;
            }
        }
	}

    debugger;
    //if ($('.cls_tfigrid .cls_txt_pa_symbol_multiple_static').val() == null) {
    //    alertError2("Please input SYMBOL!!!")
    //    return false;
    //}
    var Chk_NHC = false, Chk_HC = false, Chk_OTS = false, Chk_NNC = false, Chk_NC = false, Chk_NS = false;
    $('.cls_tfigrid .cls_txt_pa_symbol_multiple_static').val().split(";").forEach(function (item) {
        if (item == "NO HEALTH CLAIM") {
            Chk_NHC = true;
        } else if (item == "HEALTH CLAIM") {
            Chk_HC = true;
        } else if (item == "NO NUTRIENT CLAIM") {
            Chk_NNC = true;
        } else if (item == "NUTRIENT CLAIM") {
            Chk_NC = true;
        } else if (item == "NO SYMBOL") {
            Chk_NS = true;
        } else if (item != "") {
            Chk_OTS = true;
        }
    });
    if (Chk_NHC != true && Chk_HC != true) {
        alertError2("Please choose symbol HEALTH CLAIM or NO HEALTH CLAIM");
        return false;
    }
    else if (Chk_NHC == true && Chk_HC == true) {
        alertError2("Please choose symbol HEALTH CLAIM or NO HEALTH CLAIM can't input in the same time");
        return false;
    }
    else if (Chk_NNC != true && Chk_NC != true) {
        alertError2("Please choose symbol NUTRIENT CLAIM or NO NUTRIENT CLAIM");
        return false;
    }
    else if (Chk_NNC == true && Chk_NC == true) {
        alertError2("Please choose symbol NUTRIENT CLAIM or NO NUTRIENT CLAIM can't input in the same time");
        return false;
    } else if (Chk_NS != true && Chk_OTS != true) {
        alertError2("Please choose NO SYMBOL or Other Symbol");
        return false;
    } else if (Chk_NS == true && Chk_OTS == true) {
        alertError2("Please choose NO SYMBOL or Other Symbol can't input in the same time");
        return false;
    }
    if ($('.cls_tfigrid .cls_lov_brand').val() == null) {
        alertError2("Please input Brand!!!");
        return false;
    }
    if ($('.cls_tfigrid .cls_lov_pa_material_group').val() == null) {
        alertError2("Please input Material Group!!!");
        return false;
    }

    return true;
}


function validateFieldSubmit_PA() // by aof
{

    if (validateFieldSave() == false)
    {
        return false;
    }


    if ($('.cls_tfigrid .cls_lov_pa_typeofprimary').val() == null) {
        alertError2("Please input Type of Primary!!!");
        return false;
    }

    if ($('.cls_tfigrid .cls_lov_pa_type_of').val() == null) {
        alertError2("Please input Type of!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_txt_pa_primary_size').val()) || $('.cls_tfigrid .cls_txt_pa_primary_size').val() == ":" ) {
        alertError2("Please input Primary size!!!");
        return false;
    }

    if ($('.cls_tfigrid .cls_txt_pa_product_code').val() == null || $('.cls_tfigrid .cls_txt_pa_product_code').val() == "") {
        alertError2("Please input Product Code!!!");
        return false;
    }
    

    if ($('.cls_tfigrid .cls_txt_pa_packing_style').val() == null || $('.cls_tfigrid .cls_txt_pa_packing_style').val() == "") {
        // alertError2("Please input type of " + $('.cls_tfigrid .cls_lov_pa_material_group').val());

        var matgroup = $('.cls_tfigrid .cls_lov_pa_material_group').val();

        switch (matgroup) {
            case "N":
            case "R":
            case "F":
            case "H":
            case "G":
            case "C":
            case "D":
            case "M":
                alertError2("Please input Packing style!!!");
                return false;
        }
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_txt_pa_pack_size').val())) {
        // alertError2("Please input type of " + $('.cls_tfigrid .cls_lov_pa_material_group').val());

        var matgroup = $('.cls_tfigrid .cls_lov_pa_material_group').val();

        switch (matgroup) {
            case "N":
            case "R":
            case "F":
            case "H":
            case "G":
            case "C":
            case "D":
            case "M":
                alertError2("Please input Pack size!!!");
                return false;
        }
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_plant_register_no option:selected').text()))
    {
        alertError2("Please input Plant registered No!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_company_name option:selected').text())) {
        alertError2("Please input Company name/Address!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_scientific_name option:selected').text())) {
        alertError2("Please input Scientific name!!!");
        return false;
    }


    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_specie option:selected').text())) {
        alertError2("Please input Specie!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_txt_pa_plant_multiple_static').val())) {
        alertError2("Please input Plant!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_txt_pa_fao_zone_multiple_static').val())) {
        alertError2("Please input FAO Zone!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_txt_pa_catching_area_multiple_static').val())) {
        alertError2("Please input Catching area!!!");
        return false;
    }

    
    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_catching_period option:selected').text())) {
        alertError2("Please input Catching period/Date!!!");
        return false;
    }


    if (isStringNullOrEmpty($('.cls_tfigrid .cls_txt_pa_catching_method_multiple_static').val())) {
        alertError2("Please input Catching method!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_pms_colour option:selected').text())) {
        alertError2("Please input PMS Colour!!!");
        return false;
    }


    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_process_colour option:selected').text())) {
        alertError2("Please input Process colour!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_total_colour option:selected').text())) {
        alertError2("Please input Total colour!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_style_of_printing option:selected').val())) {
        alertError2("Please input Style of printing!!!");
        return false;
    }

    if (isStringNullOrEmpty($('.cls_tfigrid .cls_lov_pa_direction_of_sticker option:selected').val())) {

        switch (matgroup) {
            case "J":      
                alertError2("Please input Direction of sticker!!!");
                return false;
        }
      
    }

    
    return true;
}

function bindDataAttachIGird() {

}
function saveActive(fn, StatusApp, event_log, myurl) {
 
    var form_name = '.cls_tfigrid ';
    //let person = prompt("Comment", "");
    if (fn.match(/PG.*/))
        person = $('.cls_tfigrid_pg .cls_txt_pg_comment').val();
    else if (fn.match(/PA.*/))
        person = $('.cls_tfigrid .cls_txt_pa_comment').val();

    //if (person != null)
    //{
        

        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item['ID'] = IGridSAPMaterialId;
        item['ActiveBy'] = $('.cls_container_taskform_igrid .cls_lov_assign option:selected').val();
        item['event_log'] = event_log;
        item['Description'] = $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').val();
        item['fn'] = fn;
  
        item['StatusApp'] = StatusApp;
        item['Remark'] = person;
        jsonObj.data = item;
        //var myurl = '/api/taskform/igrid/saveActive';
        var mytype = 'POST';
    var mydata = jsonObj;

    if (fn == "PG_Assign" || event_log == "Reject") {

        myAjax(myurl, mytype, mydata, callbackNoSaveIGridApprove, '', false, true);
    } else {
        myAjax(myurl, mytype, mydata, callbackSaveIGridApprove, '', false, true);
    }

    //}


}

function saveIGridCompleteInfoGroup(is_showmsg) {

    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    //if ($('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').is(":checked") == false) {
    //    alertError2("Please Check Complete InfoGroup.");
    //    return false;
    //}
    //if ($('.cls_tfigrid_pg .cls_txt_final_infogroup').val() == null || $('.cls_tfigrid_pg .cls_txt_final_infogroup').val() == "") {
    //    alertError2("Please input request final infogroup.");
    //    return false;
    //}
    if ($('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').is(":checked") == false && (($('.cls_tfigrid_pg .cls_txt_final_infogroup').val() == null || $('.cls_tfigrid_pg .cls_txt_final_infogroup').val() == ""))) {
        alertError2("Please input Final infoGroup Or Check Complete InfoGroup.");
        return false;
    }
    item['Id'] = IGridSAPMaterialId;
    item['FinalInfoGroup'] = $('.cls_tfigrid_pg .cls_txt_final_infogroup').val();
    item['IsSaveCompleteInfoGroup'] = $('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').is(":checked") ? 'X' : '';

    if (item['IsSaveCompleteInfoGroup'] == 'X')
    {
        check_btn = false;  // for close form and redirect link to inbox
    }


    jsonObj.data = item;
    var myurl = '/api/taskform/igrid/savecompleteinfogroup';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjax(myurl, mytype, mydata, callbackSaveIGridSAPMaterial, '', false, is_showmsg);

}


function saveIGridSAPMaterial(is_showmsg) {
    var ischeckproduct = false;
    var product_type = "";
    var form_name = '.cls_tfigrid ';

    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item['MaterialGroup'] = $(form_name + '.cls_lov_pa_material_group').val();

    item['Brand'] = $(form_name + '.cls_lov_brand').val();
    item['Description'] = $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').val();
    item['Primarysize_id'] = $(form_name + '.cls_txt_pa_primary_size_id').val();
    item['FAOZone'] = $(form_name + '.cls_txt_pa_fao_zone_multiple_static').val();
    item['Plant'] = $(form_name + '.cls_txt_pa_plant_multiple_static').val();
    item['Primarysize'] = $(form_name + '.cls_txt_pa_primary_size').val().split(":")[0];
      
    item['Id'] = IGridSAPMaterialId;
    item["ChangePoint"] = $('input[name=cls_rdo_haeder_tfartwork_change_point]:radio:checked').val();
    item["PrintingStyleofPrimary"] = $(form_name + '.cls_lov_pa_printing_style_of_primary option:selected').text();
    item["PrintingStyleofSecondary"] = $(form_name + '.cls_lov_pa_printing_style_of_secondary option:selected').text();
    item["CustomerDesign"] = $('input[name=pa_rdo_customer_design]:radio:checked').val();
    item["CustomerSpec"] = $('input[name=pa_rdo_customer_spec]:radio:checked').val();
    item["CustomerSize"] = $('input[name=pa_rdo_customer_size]:radio:checked').val();
    item["CustomerVendor"] = $('input[name=pa_rdo_customer_nominate]:radio:checked').val();
    item["CustomerColor"] = $('input[name=pa_rdo_customer_nominate_colour]:radio:checked').val();
    item["CustomerScanable"] = $('input[name=pa_rdo_customer_barcode]:radio:checked').val();
    item["CustomerBarcodeSpec"] = $('input[name=pa_rdo_customer_barcode_spec]:radio:checked').val();

    item["CustomersDesignDetail"] = $(form_name + '.cls_txt_pa_customer_design').val();
    item["CustomersSpecDetail"] = $(form_name + '.cls_txt_pa_customer_spec').val();
    item["CustomersSizeDetail"] = $(form_name + '.cls_txt_pa_customer_size').val();
    item["CustomerNominatesVendorDetail"] = $(form_name + '.cls_txt_pa_customer_nominate').val();
    item["CustomerNominatesColorPantoneDetail"] = $(form_name + '.cls_txt_pa_customer_nominate_colour').val();
    item["CustomersBarcodeScanableDetail"] = $(form_name + '.cls_txt_pa_customer_barcode').val();
    item["CustomersBarcodeSpecDetail"] = $(form_name + '.cls_txt_pa_barcode_spec').val();

    item["PackingStyle"] = $(form_name + '.cls_txt_pa_packing_style').val();
    item["Packing"] = $(form_name + '.cls_txt_pa_pack_size').val();
    item["StyleofPrinting"] = $(form_name + '.cls_lov_pa_style_of_printing option:selected').text();
    item["ContainerType"] = $(form_name + '.cls_txt_pa_container_type').val();
    item["LidType"] = $(form_name + '.cls_txt_pa_lid_type').val();
    item["TotalColour"] = $(form_name + '.cls_lov_pa_total_colour option:selected').text();
    item["ProductCode"] = $(form_name + '.cls_txt_pa_product_code').val();
    item["Processcolour"] = $(form_name + '.cls_lov_pa_process_colour option:selected').text();
    item["PlantRegisteredNo"] = $(form_name + '.cls_lov_pa_plant_register_no option:selected').text();
    item["CompanyNameAddress"] = $(form_name + '.cls_lov_pa_company_name option:selected').text();
    item["PMScolour"] = $(form_name + '.cls_lov_pa_pms_colour option:selected').text();
    item["Symbol"] = $(form_name + '.cls_txt_pa_symbol_multiple_static').val();
    item["CatchingArea"] = $(form_name + '.cls_txt_pa_catching_area_multiple_static').val();
    item["CatchingPeriodDate"] = $(form_name + '.cls_lov_pa_catching_period option:selected').text();

    item["Grandof"] = $('.cls_tfigrid_pg .cls_lov_grade_of option:selected').text();
    item["Flute"] = $('.cls_tfigrid_pg .cls_lov_flute option:selected').text();
    item["Vendor"] = $('.cls_tfigrid_pg .cls_txt_pg_vendor_multiple_static').val();
    item["Dimension"] = $('.cls_tfigrid_pg .cls_txt_dimension_of').val();
    item["RSC"] = $('.cls_tfigrid_pg .cls_lov_rsc_di_cut option:selected').text();
    item["Accessories"] = getAccessoriesToTable(); // $('.cls_tfigrid_pg .cls_txt_acessories').val();
    item["SheetSize"] = $('.cls_tfigrid_pg .cls_txt_sheet_size').val();

    item["Typeof"] = $(form_name + '.cls_lov_pa_type_of option:selected').text();
    item["TypeofCarton2"] = $(form_name + '.cls_lov_pa_type_of_two option:selected').text();
    item["TypeofPrimary"] = $(form_name + '.cls_lov_pa_typeofprimary option:selected').text();//$(form_name + '.cls_lov_pa_typeofprimary').text();
    item["PrintingSystem"] = $('.cls_tfigrid_pg .cls_lov_printing_system option:selected').text();
    item["Direction"] = $(form_name + '.cls_lov_pa_direction_of_sticker option:selected').text();
    item["RollSheet"] = $('.cls_tfigrid_pg .cls_lov_roll_sheet option:selected').text();
    item["PlantAddress"] = $(form_name + '.cls_txt_pa_plant_multiple_static').text();
    item["Catching_Method"] = $(form_name + '.cls_txt_pa_catching_method_multiple_static').val();
    item["Scientific_Name"] = $(form_name + '.cls_lov_pa_scientific_name option:selected').text();
    item["Specie"] = $(form_name + '.cls_lov_pa_specie option:selected').text();
    item["Fixed_Desc"] = $('.cls_container_taskform_igrid .cls_chk_Fixed_Desc').is(":checked") ? "1" : "";
    item["SustainMaterial"] = $('.cls_tfigrid_pg .cls_lov_pkg_sec_material option:selected').text();
    item["SustainPlastic"] = $('.cls_tfigrid_pg .cls_lov_pkg_sec_plastic option:selected').text();
    item["SustainReuseable"] = $('input[name=pg_rdo_pkg_sec_reusable]:radio:checked').val();
    item["SustainRecyclable"] = $('input[name=pg_rdo_pkg_sec_recyclable]:radio:checked').val();
    item["SustainComposatable"] = $('input[name=pg_rdo_pkg_sec_composatable]:radio:checked').val();
    item["SustainCertification"] = $('input[name=pg_rdo_pkg_sec_certification]:radio:checked').val();
    item["SustainCertSourcing"] = $('.cls_tfigrid_pg .cls_lov_pkg_sec_certification_standard option:selected').text();
    item["SustainOther"] = $('.cls_tfigrid_pg .cls_txt_pkg_sec_other').val();
    item["SusSecondaryPKGWeight"] = $('.cls_tfigrid_pg .cls_txt_pkg_sec_weight').val();
    item["SusRecycledContent"] = $('.cls_tfigrid_pg .cls_txt_pkg_sec_recycled_content').val();
    item["PlantAddress"] = $(form_name + '.cls_input_pa_sec_pkg_production_plant').val();
    item["FirstInfoGroup"] = $(form_name + '.cls_txt_pa_first_info').val();
    item["PICMkt"] = $(form_name + '.cls_txt_pa_pic_mkt').val();
    item["SO"] = $(form_name + '.cls_txt_pa_so').val();
    item["DMSNo"] = $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_aw_no').val();
    item["Version"] = $('.cls_container_taskform_igrid .cls_txt_version_of_material').val();
    item["SOPlant"] = $(form_name + '.cls_txt_pa_soplant').val();
    item["Destination"] = $(form_name + '.cls_txt_pa_country').val();
    item["Remark"] = $(form_name +'.cls_txt_pa_remark').val();
    item["Note"] = $('.cls_tfigrid_pg .cls_txt_remark_note_of_pg').val();
    item["FinalInfoGroup"] = $('.cls_tfigrid_pg .cls_txt_final_infogroup').val();
    item["IsApprove_step"] = IsApprove_step;
    
    jsonObj.data = item;
    var myurl = '/api/taskform/igrid/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjax(myurl, mytype, mydata, callbackSaveIGridSAPMaterial, '', false, is_showmsg);

}
function callbackNoSaveIGridApprove(res) {
//if (res.data != null) {
        var url = suburl + '/Home/';
        $(location).attr('href', url);
//    }
}
function callbackSaveIGridApprove(res) {
    saveIGridSAPMaterial(true);
    //if (res.data != null) {
    //    var url = suburl + '/Home/';
    //    $(location).attr('href', url);
    //}
}
function callbackSaveIGridSAPMaterial(res) {
    //ARTWORK_SUB_PA_ID = res.data[0].ARTWORK_SUB_PA_ID;
    if (res.data != null) {
        var url = suburl + '/IGrid/' + res.data[0].Id;
        if (check_btn == false) {
            url = suburl + '/Home/';
        }
        $(location).attr('href', url);
    }
    //window.open(suburl + '/IGrid/22824', '_blank');
    //bindDataIGridSAPMaterial();
}
function uploadConfirm() {
    var form_name = '.cls_tfigrid ';

    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item['Name'] = $(form_name + '.cls_lov_pa_material_group').val();
    jsonObj.data = item;
    var myurl = '/api/taskform/igrid/upload';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjax(myurl, mytype, mydata, callbackSaveIGridSAPMaterial, '', false, is_showmsg);
    //$.ajax({
    //    type: 'POST',
    //    contentType: 'application/ json; charset = utf - 8',
    //    url: '@Url.Action("CheckDataBeforeUpload", "NoteUpload")',
    //    data: "{'periodID': '" + $('#selectedlistPeriods').val() +
    //        "' , 'financeID': '" + $('#selectedlistFinance').val() +
    //        "'} ",
    //    success: function (resultSave) {
    //    },
    //    error: function (data) {
    //        alert(data);
    //    }
    //});
}
function setcal() {
    matgroup_value = $('.cls_tfigrid .cls_lov_pa_material_group').val();
    brand = $('.cls_tfigrid .cls_lov_brand').val();
    pkg_size = $('.cls_tfigrid .cls_txt_pa_primary_size').val().split(":")[0];
    changepoint = $('input[name=cls_rdo_haeder_tfartwork_change_point]:radio:checked').val();
    runing = "XXXXXXX";
    var version = $('.cls_container_taskform_igrid .cls_txt_version_of_material').val();
    //debugger;
    if ($('.cls_container_taskform_igrid .cls_txt_header_tfartwork_rf_no').val().substring(0, 1) == '1' && IsStatusApp == 0) {
        $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_mat_no').val(['5', matgroup_value, brand, pkg_size, changepoint == '1' || changepoint == 'C' ? "C" : "N", runing, version].join(""));
    } else if ($('.cls_container_taskform_igrid .cls_ddl_option_condition').val() == "4") {
        var str = $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_mat_no').val();
        var mat = str.substring(0, 8) + (changepoint == '1' || changepoint == 'C' ? "C" : "N") + str.substring(8 + 1);
        $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_mat_no').val(mat);
    }

    if ($('.cls_container_taskform_igrid .cls_chk_Fixed_Desc').is(":checked")==false) {
        //TypeOfText = $('.cls_tfigrid .cls_lov_pa_type_of').val();
        //    DocNo_Gigit1 & DocNo_RightDigits & "-" & TextBrand
        if ($('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').val() == '') {
            var TextBrand = $('.cls_tfigrid .cls_lov_brand option:selected').text();
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').val([TypeOfText, "", "", "-", TextBrand].join(""));
        }
    }
}

function get_Production_plant() {
    var company_name = $('.cls_tfigrid .cls_lov_pa_company_name option:selected').text();
    var RegisteredNo = $('.cls_tfigrid .cls_lov_pa_plant_register_no option:selected').text();
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item['STR_PRODUCT_CODE'] = $(".cls_tfigrid .cls_txt_pa_product_code").val();
    item['RegisteredNo'] = RegisteredNo;
    item['Address'] = company_name;
    jsonObj.data = item;

    $.ajax({
        type: 'GET',
        url: '/api/lov/CompanyAddresIGrid2',
        data: jsonObj,
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        success: function (res) {
            res = DData(res);
            debugger;
            if (res.data.length > 0) {
                for (var i = 0; i < res.data.length; i++) {
                    $('.cls_tfigrid .cls_input_pa_sec_pkg_production_plant').val(res.data[i].Plant);
                }
            }
        }
    });

    //var myurl = '/api/lov/CompanyNameIGrid';//"/api/lov/CompanyNameIGrid?data.Address=" + company_name + "&data.RegisteredNo=" + RegisteredNo;
    //var mytype = 'GET';
    //var mydata = jsonObj;
    //myAjax(myurl, mytype, mydata, callback_getPA_ProductionPlant);
}
function callback_getPA_ProductionPlant(res) {
    debugger;
    if (res.data.length > 0) {
        for (var i = 0; i < res.data.length; i++) {
            $('.cls_tfigrid .cls_input_pa_sec_pkg_production_plant').val(res.data[i].Plant);
        }
    }
}
function get_desacription_text() {
    var material_group = $('.cls_tfigrid .cls_lov_pa_material_group').val();
    var DescriptionText = $('.cls_tfigrid .cls_lov_pa_type_of option:selected').text();
    var Brand = $('.cls_tfigrid .cls_lov_brand').val();
    var myurl = "/api/lov/TypeOfandBrandIGrid?data.MaterialType=0&data.MaterialGroup=" + material_group + "&data.DescriptionText=" + DescriptionText + "&data.Brand="+ Brand;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_desacription_text);
}
function callback_get_desacription_text(res) {
    //debugger;
    TypeOfText = "";
    if (res.data.length > 0) {
        for (var i = 0; i < res.data.length; i++) {
            TypeOfText = res.data[i].DescriptionText;
        }
    }

    $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').val(TypeOfText);

    
}
function setRequireFieldTaskformIGridPA(matgroup_value) {
    //debugger;
    setcal();

    var StyleOfPrintingDiff = "";
    var PackingStyleDiff = "";
    if ($('.cls_tfigrid .cls_lov_pa_style_of_printing option:selected').text() != STYLE_OF_PRINTING_DISPLAY_TEXT)
    {
        StyleOfPrintingDiff = "";//STYLE_OF_PRINTING_DISPLAY_TEXT;
    } else
    {
        StyleOfPrintingDiff = "";
    }

    if ($('.cls_tfigrid .cls_txt_pa_packing_style').val() != PACKING_STYLE_DISPLAY_TEXT)
    {
        PackingStyleDiff = ""; //PACKING_STYLE_DISPLAY_TEXT;
    } else
    {
        PackingStyleDiff = "";
    }

    //debugger;
    //$('.cls_tfigrid .cls_txt_pa_packing_style').val('');//clear change value //20230612
    $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html("Style of printing : " + StyleOfPrintingDiff);
    $('.cls_container_taskform_igrid .cls_lbl_pa_direction_of_sticker').html("Direction of sticker :");
    $('.cls_container_taskform_igrid .cls_lbl_pa_customer_design').html("Customer's design :");

    $('.cls_container_taskform_igrid').find('.cls_btn_pa_submit').attr('disabled', false);
    $('.cls_container_taskform_igrid').find('.cls_btn_pa_save').attr('disabled', false);
    
    var lbl_spanStar = "<span style=\"color: red;\">*</span>";
    var lbl_stylf_of_printing = "<span style=\"color: red;\">Style of printing *</span> : <span style=\"color: red;\">" + StyleOfPrintingDiff +"</span>";
    switch (matgroup_value) {
        case "K":
            $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html(lbl_stylf_of_printing); //"Style of printing " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            $('.cls_container_taskform_igrid .cls_lbl_pa_packing_style').html("<span style=\"color: black;\">Packing style</span>");
            $(".cls_tfigrid .cls_btn_pa_packing_style").hide();
            $('.cls_tfigrid .cls_txt_pa_packing_style').val('').trigger("change");
            $('.cls_tfigrid .cls_txt_pa_pack_size').val('').trigger("change");
            break;
        case "L":
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            $('.cls_container_taskform_igrid .cls_lbl_pa_packing_style').html("<span style=\"color: black;\">Packing style</span>");
            $(".cls_tfigrid .cls_btn_pa_packing_style").hide();
            $('.cls_tfigrid .cls_txt_pa_packing_style').val('').trigger("change");
            $('.cls_tfigrid .cls_txt_pa_pack_size').val('').trigger("change");
            break;
        case "P":
            $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html(lbl_stylf_of_printing); //"Style of printing " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            $('.cls_container_taskform_igrid .cls_lbl_pa_packing_style').html("<span style=\"color: black;\">Packing style</span>");
            $(".cls_tfigrid .cls_btn_pa_packing_style").hide();
            $('.cls_tfigrid .cls_txt_pa_packing_style').val('').trigger("change");
            $('.cls_tfigrid .cls_txt_pa_pack_size').val('').trigger("change");
            break;
        case "J":
            $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html(lbl_stylf_of_printing); //"Style of printing " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lbl_pa_direction_of_sticker').html("Direction of sticker " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            $('.cls_container_taskform_igrid .cls_lbl_pa_packing_style').html("<span style=\"color: black;\">Packing style</span>");
            $(".cls_tfigrid .cls_btn_pa_packing_style").hide();
            $('.cls_tfigrid .cls_txt_pa_packing_style').val('').trigger("change");
            $('.cls_tfigrid .cls_txt_pa_pack_size').val('').trigger("change");
            //$('.cls_container_taskform_igrid .cls_lbl_pa_customer_design').html("Customer's design " + lbl_spanStar + ":");
            break;
        default:
            var span_lbl_packing_style = "<span style=\"color: red;\">Packing style *</span> : <span style=\"color: red;\">" + PackingStyleDiff + "</span>";
            $('.cls_container_taskform_igrid .cls_lbl_pa_packing_style').html(span_lbl_packing_style) //"Packing style " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lbl_pa_pack_size').html("Pack size " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html(lbl_stylf_of_printing); //"Style of printing " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", false);   // by aof to disable packing style
            $(".cls_tfigrid .cls_btn_pa_packing_style").show();
            break;
    }
}

function bindDataIGridHistory() {
    //debugger;
    //table.clear().destroy(); // Clear all rows and remove all columns
    //$('#TableId_igrid_pa_faozone').empty(); // Empty the table element to remove any remaining rows and columns

    var table_igrid = $('#table_igrid_history').DataTable();
    table_igrid.destroy();

    table_igrid = $('#table_igrid_history').DataTable({
        //"scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
                //for (var i = 0, len = data.columns.length; i < len; i++) {
                //    delete data.columns[i].name;
                //    delete data.columns[i].data;
                //    delete data.columns[i].searchable;
                //    delete data.columns[i].orderable;
                //    delete data.columns[i].search.regex;
                //    delete data.search.regex;

                //    delete data.columns[i].search.value;
                //}
            }
            $.ajax({
                url: suburl + '/api/taskform/igrid/history?data.ID=' + IGridSAPMaterialId,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": false,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,

        //"scrollX": true,
        //"bAutoWidth": true,
        //"scrollX": true,
        columns:
            [
                {
                    data: null,
                    defaultContent: '',
                    //className: 'select-checkbox',
                    orderable: false
                },
                { data: "Name", "className": "cls_nowrap" },
                { data: "Result", "className": "cls_nowrap" },
                { data: "ActiveBy", "className": "cls_nowrap" },
                { data: "ModifyOn", "className": "cls_nowrap cls_modifyon" }
            ],
        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        "processing": true,
        "rowCallback": function (row, data, index) {
            if (!isEmpty(data.ModifyOn)) {
                $(row).find('.cls_modifyon').html(myDateMoment(data.ModifyOn));
            }
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        columnDefs: [{
            render: function (data, type, full, meta) {
                return "<div id='dvNotes' style='white-space: normal;width: 590px;'>" + data + "</div>";
            },
            targets: 2
        }],
        order: [[4, 'asc']],
        initComplete: function (settings, json) {

        },
    });


    $("#table_igrid_history_filter").hide();
}

function bindSuggestSymbolPopUp(serverSide, autosearch, default_value) {
    //alert(serverSide);  
   
    var test = "";
    var table_igrid = $('#TableId_igrid_pa_symbol').DataTable()
    table_igrid.destroy();
    table_igrid = $('#TableId_igrid_pa_symbol').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
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
                url: suburl + '/api/lov/SymbolIGrid',
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": false,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": false,
        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns:
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                //{
                //    render: function (data, type, row, meta) {
                //        return '<input class="cls_chk_plant" type="checkbox">';
                //    }
                //},
                { data: "Id", "className": "cls_nowrap cls_hide" },
                { data: "Description", "className": "cls_nowrap" }
            ],
        select: {
            'style': 'multi',
           // selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {

           if (checkDataIsExist(data.Description, default_value)) {
                $(row).addClass('selected')
            }


        },
        "columnDefs": [
            {
                "aTargets": [0],
                "bSortable": false
            }
        ],
        order: [],
        initComplete: function (settings, json) {
            default_value = "";
        },
    });
}

function bindSuggestCatchingMethodPopUp(serverSide, autosearch, default_value) {
    //alert(serverSide);  


    var table_igrid = $('#TableId_igrid_pa_catching_method').DataTable()
    table_igrid.destroy();
    table_igrid = $('#TableId_igrid_pa_catching_method').DataTable({

        "scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
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
                url: suburl + '/api/lov/CatchingMethodIGrid',
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,

        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns:
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                //{
                //    render: function (data, type, row, meta) {
                //        return '<input class="cls_chk_plant" type="checkbox">';
                //    }
                //},
                { data: "Id", "className": "cls_nowrap cls_hide" },
                { data: "Description", "className": "cls_nowrap" }
            ],
        select: {
            'style': 'multi',
            //selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            if (checkDataIsExist(data.Description, default_value)) {
                $(row).addClass('selected')
            }
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {
            default_value = "";
        },
    });
}
function bindSuggestVendorPopUp(serverSide, autosearch, default_value) {
    //alert(serverSide);  
   // $('#TableId_igrid_pg_vendor').DataTable({

    var table_igrid = $('#TableId_igrid_pg_vendor').DataTable()
    table_igrid.destroy();
    table_igrid = $('#TableId_igrid_pg_vendor').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
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
                url: suburl + '/api/lov/VendorIGrid',
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,

        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns:
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                //{
                //    render: function (data, type, row, meta) {
                //        return '<input class="cls_chk_plant" type="checkbox">';
                //    }
                //},
                { data: "Code", "className": "cls_nowrap" },
                { data: "Name", "className": "cls_nowrap" }
            ],
        select: {
            'style': 'multi',
          //  selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            if (checkDataIsExist(data.Code, default_value)) {
                $(row).addClass('selected')
            }
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {
            default_value = "";
        },
    });
}
function bindSuggestFAOZonePopUp(serverSide, autosearch, url, table, default_value) {


    //alert(serverSide);  
    var table_igrid = $(table).DataTable()
    table_igrid.destroy();
    table_igrid = $(table).DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
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
                //url: suburl + '/api/taskform/igrid/primarysize',
                //url: suburl +'/api/taskform/igrid/info?data.Id=' + MainArtworkSubId,
                url: url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,

        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns:
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                //{
                //    render: function (data, type, row, meta) {
                //        return '<input class="cls_chk_plant" type="checkbox">';
                //    }
                //},
                { data: "Id", "className": "cls_nowrap cls_hide" },
                { data: "Description", "className": "cls_nowrap" }
            ],
        select: {
            'style': 'multi',
            //selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            if (checkDataIsExist(data.Description, default_value)) {
                $(row).addClass('selected')
            }
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {
            default_value = "";
        },
    });
}
function bindSuggestPlantPopUp(serverSide, autosearch, url, table, default_val) {
    //alert(serverSide);  
    var table_igrid = $(table).DataTable()
    table_igrid.destroy();
    table_igrid = $(table).DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
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
                //url: suburl + '/api/taskform/igrid/primarysize',
                //url: suburl +'/api/taskform/igrid/info?data.Id=' + MainArtworkSubId,
                url: url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,

        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns: 
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                //{
                //    render: function (data, type, row, meta) {
                //        return '<input class="cls_chk_plant" type="checkbox">';
                //    }
                //},
                { data: "Id", "className": "cls_nowrap cls_hide"},
                { data: "Description", "className": "cls_nowrap" }
        ],
        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        select: {
            'style': 'multi',
            //selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {

            
            if (checkDataIsExist(data.Id, default_val))
            {
                $(row).addClass('selected')
            }
         
         


        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {
            default_value = "";
        },
    });
}

function checkDataIsExist(search,data)
{
    var f_exist = false;

    if (data != null && data != 'undefined')
    {
        var arr = data.split(';');
        $.each(arr, function (i, v) {
            if (search.toLowerCase() == v.toLowerCase())
            {
                f_exist = true;
                return f_exist;
            }
        });
    }

    return f_exist;
}

function bindSuggestCatchingAreaPopUp(serverSide, autosearch, url, table, default_value) {
    //alert(serverSide);  
    var table_igrid = $(table).DataTable()
    table_igrid.destroy();
    table_igrid = $(table).DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
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
                //url: suburl + '/api/taskform/igrid/primarysize',
                //url: suburl +'/api/taskform/igrid/info?data.Id=' + MainArtworkSubId,
                url: url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,

        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns:
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                //{
                //    render: function (data, type, row, meta) {
                //        return '<input class="cls_chk_plant" type="checkbox">';
                //    }
                //},
                { data: "Id", "className": "cls_nowrap cls_hide" },
                { data: "Description", "className": "cls_nowrap" }
            ],
        
        select: {
            'style': 'multi',
            //selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            if (checkDataIsExist(data.Description, default_value)) {
                $(row).addClass('selected')
            }
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {
            default_value = "";
        },
    });
}
function bindSuggestPackingStylePopUp(serverSide, autosearch, default_value) {
    //alert(serverSide);  
    var TableId_igrid_pa_packing_style = $('#TableId_igrid_pa_packing_style').DataTable();
    TableId_igrid_pa_packing_style.destroy();
    TableId_igrid_pa_packing_style = $('#TableId_igrid_pa_packing_style').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
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
                //url: suburl + '/api/taskform/igrid/primarysize',
                url: suburl + "/api/lov/PackStyleIGrid?data.TypeofPrimary=" + $('.cls_tfigrid .cls_lov_pa_typeofprimary option:selected').text(),
                //url: url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns:
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                //{
                //    render: function (data, type, row, meta) {
                //        return '<input class="cls_chk_plant" type="checkbox">';
                //    }
                //},
                { data: "Id", "className": "cls_nowrap cls_hide" },
                { data: "PrimaryCode", "className": "cls_nowrap cls_seleced" },
                { data: "GroupStyle", "className": "cls_nowrap cls_seleced" },
                { data: "PackingStyle", "className": "cls_nowrap cls_seleced" },
                { data: "RefStyle", "className": "cls_nowrap cls_seleced" },
                { data: "PackSize", "className": "cls_nowrap cls_seleced" },
            ],
        select: {
            style: 'os',
           // selector: 'td:first-child'
        },
        //select: {
        //    'style': 'multi',
        //    selector: 'td:first-child input,td:last-child input'
        //},
        "processing": true,
        "rowCallback": function (row, data, index) {
            if (checkDataIsExist(data.RefStyle, default_value)) {
               // $(row).addClass('selected')
                $(row).find('.cls_seleced').addClass('fontRed');
            } else
            {
                $(row).find('.cls_seleced').addClass('fontBlack');
            }
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}
function bindSuggestPrimarySizePopUp(serverSide, autosearch, url, table, cls_txt_pa_primary_size_id, default_value) {
    //alert(serverSide);  
    var table_igrid = $(table).DataTable()
    table_igrid.destroy();
    table_igrid = $(table).DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: false,
        orderCellsTop: true,
        serverSide: false,
        ajax: function (data, callback, settings) {
            if (false) {
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
                //url: suburl + '/api/taskform/igrid/primarysize',
                //url: suburl +'/api/taskform/igrid/info?data.Id=' + MainArtworkSubId,
                url : url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
      
        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns: 
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap cls_hide" },
                { data: "Code", "className": "cls_nowrap cls_code" },
                { data: "Can", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap cls_description" },
                { data: "LidType", "className": "cls_nowrap" },
                { data: "ContainerType", "className": "cls_nowrap" },
             
                { data: "DescriptionType", "className": "cls_nowrap cls_description_type" },
        ],
        select: {
            style: 'os',
            //selector: 'td:first-child'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {

            $(row).find('.cls_description_type').text(data.DescriptionType);

            var dataCheck = data.Code + ":" + data.Description;
            if (checkDataIsExist(dataCheck, default_value)) {
                $(row).find('.cls_code').addClass('fontRed');
                $(row).find('.cls_description').addClass('fontRed');
            } else
            {
                $(row).find('.cls_code').addClass('fontBlack');
                $(row).find('.cls_description').addClass('fontBlack');
            }
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}


function isStringNullOrEmpty(val, type = 'text') {
    var f_empty = false;


    if (type == 'number') {
        if (val == '0' || val == '' || val == 'undefined' || val == null) {
            f_empty = true;
        }
    } else if (type = 'text')
    {
        if (val == '' || val == 'undefined' || val == null) {
            f_empty = true;
        }
    }
  
    return f_empty;
}


function getPakcingStyleByFG(fg)
{
    $(".cls_tfigrid .cls_lbl_pa_packing_style_fg").html('');
    if (!isStringNullOrEmpty(fg)) {
        var myurl = '/api/taskform/igrid/packingstylefg?data.PRODUCT_CODE=' + fg;
        var mytype = 'GET';
        var mydata = null;
        myAjax(myurl, mytype, mydata, callback_getPakcingStyleByFG);
    }
 
}

function getproduct_code(fg) {

    var company_name = $('.cls_tfigrid .cls_lov_pa_company_name option:selected').text();
    var RegisteredNo = $('.cls_tfigrid .cls_lov_pa_plant_register_no option:selected').text();
    bindLOVPlantRegister();
    if (!isStringNullOrEmpty(fg)) {
        var myurl = '/api/taskform/igrid/ProductCodeIGrid?data.PRODUCT_CODE=' + fg + "&data.Address=" + company_name + "&data.RegisteredNo=" + RegisteredNo;
        var mytype = 'GET';
        var mydata = null;
        myAjax(myurl, mytype, mydata, callback_getproduct_code);
    }


}
function callback_getprd_plant(res) {
    if (res.data.length > 0) {
        for (var i = 0; i < res.data.length; i++) {
            $('.cls_tfigrid .cls_input_pa_sec_pkg_production_plant').val(res.data[i].prd_plant);
        }
    }
}
function callback_getproduct_code(res) {
    $(".cls_tfigrid .cls_txt_pa_plant_register_no").val('');
    //bindLOVPlantRegister();
    debugger;
    
    if (res.data[0].Count_RegisteredNo > 0) {
        //$(".cls_tfigrid .cls_lbl_pa_packing_style_fg").html(res.data[0].PACKING_STYLE);
        $(".cls_tfigrid .cls_txt_pa_plant_register_no").val(res.data[0].RegisteredNo);
        bindLOVCompanyAddress();
    } else {
        $(".cls_tfigrid .cls_lov_pa_plant_register_no").val('').change();
    }
    if (res.data[0].Count_Address > 0) {
        $(".cls_tfigrid .cls_lov_pa_company_name").val(res.data[0].Address);
        
    } else {
        $(".cls_tfigrid .cls_lov_pa_company_name").val('').change();
    }
}
function callback_getPakcingStyleByFG(res)
{

   
    if (res.data.length > 0)
    {
        $(".cls_tfigrid .cls_lbl_pa_packing_style_fg").html(res.data[0].PACKING_STYLE);
    }
}

function bindDataIGridSAPMaterial() {
    var myurl = '/api/taskform/igrid/info?data.Id=' + IGridSAPMaterialId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bindDataIGridSAPMaterial);
}
function callback_bindDataIGridSAPMaterial(res) {
    if (res.data.length > 0) {

        var item = res.data[0];
        if (item != null) {

            //STATUSAPP = item.StatusApp;

            if (item.fn == "PA") {
                $('.cls_tfigrid .cls_btn_pa_save').prop('disabled', false);
                $('.cls_tfigrid .cls_btn_pa_submit').prop('disabled', false);
                $('.cls_tfigrid .cls_btn_pa_remove').prop('disabled', true);
                $('.cls_tfigrid .cls_btn_pa_reject').prop('disabled', true);
                $('#cls_btn_pa_submit').html('Submit');
                $('.cls_container_taskform_igrid .cls_lov_assign').prop('disabled', true)
            } else if (item.fn == "PG") {
                $('a[href="#view_pg"]').tab('show');
                $('.cls_tfigrid_pg .cls_btn_pa_remove').prop('disabled', true);
                $('.cls_tfigrid_pg .cls_btn_reject').prop('disabled', true);

                $('.cls_tfigrid_pg .cls_btn_save').prop('disabled', false);
                $('.cls_tfigrid_pg .cls_btn_submit').prop('disabled', false);
                $("#cls_btn_pg_submit").html('Submit');
            }
            $('.fileupload-buttonbar').show();

            var form_name = '.cls_container_taskform_igrid ';
            $('.cls_container_taskform_igrid').find('.cls_btn_pa_submit').attr('disabled', true);
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_rf_no').val(item.DocumentNo);
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_aw_no').val(item.DMSNo);

            if (item.ArtowrkURL != null && item.ArtowrkURL != "") {
                $(".cls_container_taskform_igrid .cls_igrid_lbl_link_aw").show();
                $(".cls_container_taskform_igrid .cls_igrid_lbl_link_aw").attr("href", item.ArtowrkURL);
            } //else
            
            if (item.MaterialGroup != "J") {
                $('.cls_tfigrid_pg .cls_lov_roll_sheet').prop('disabled', true);
                $('.cls_tfigrid_pg .cls_lov_printing_system').prop('disabled', true);
            }
           
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_ref_form_no').val(item.Refnumber);
            $('.cls_tfigrid .cls_txt_pa_primary_size_id').val(item.PrimarySize_id);
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_mat_no').val(item.Material);
            $('.cls_container_taskform_igrid .cls_txt_version_of_material').val(item.Version);
            $('.cls_container_taskform_igrid .cls_ddl_option_condition').val(item.Condition);
            if (item.Condition == "4") {
                $('.cls_tfigrid .cls_lov_pa_material_group').prop('disabled', true);
                $('.cls_tfigrid .cls_lov_brand').prop('disabled', true);
                //$('.cls_tfigrid .cls_lov_pa_typeofprimary').prop('disabled', true);
				//$('.cls_container_taskform_igrid .cls_txt_header_tfartwork_aw_no').prop('disabled', true);
                $('.cls_container_taskform_igrid .cls_ddl_option_condition').prop('disabled', true);
                //$(".cls_tfigrid .cls_btn_pa_primary_size").hide();
                $("#modal_igrid_pa_primary .cls_btn_igrid_pa_terminate_submit").prop('disabled', true);
                
            }
            if (item.Condition == "7") {
                $('.cls_tfigrid .cls_lov_pa_material_group').prop('disabled', true);
                $('.cls_tfigrid .cls_lov_brand').prop('disabled', true);
                //$('.cls_tfigrid .cls_lov_pa_typeofprimary').prop('disabled', true);
				$('input[name=cls_rdo_haeder_tfartwork_change_point]').attr("disabled", true);
				//$('.cls_container_taskform_igrid .cls_txt_header_tfartwork_aw_no').prop('disabled', true);
                $('.cls_container_taskform_igrid .cls_ddl_option_condition').prop('disabled', true);
                //$(".cls_tfigrid .cls_btn_pa_primary_size").hide();
                $("#modal_igrid_pa_primary .cls_btn_igrid_pa_terminate_submit").prop('disabled', true);
                
            }
            $(form_name + '.cls_txt_header_tfartwork_doc').val(item.Description.replace(',', '-'));
            BRAND_DISPLAY_TEXT = item.Brand_TXT;
            setValueToDDL(form_name + '.cls_lov_brand', item.Brand, item.Brand_TXT);
            setValueToDDL(form_name + '.cls_lov_pa_typeofprimary', item.TypeofPrimary, item.TypeofPrimary);


            if (item.ChangePoint == "N")
                $("input[name=cls_rdo_haeder_tfartwork_change_point][value=N]").prop('checked', true);
            else 
                $("input[name=cls_rdo_haeder_tfartwork_change_point][value=C]").prop('checked', true);

            //
            setValueToDDL(form_name + '.cls_lov_pa_type_of_two', item.TypeofCarton2, item.TypeofCarton2);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_type_of', item.Typeof, item.Typeof);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_catching_period', item.CatchingPeriodDate, item.CatchingPeriodDate);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_scientific_name', item.Scientific_Name, item.Scientific_Name);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_process_colour', item.Processcolour, item.Processcolour);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_pms_colour', "99", item.PMScolour);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_total_colour',"99", item.Totalcolour);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_process_colour', "99", item.Processcolour);
            STYLE_OF_PRINTING_DISPLAY_TEXT = item.StyleofPrinting;
            setValueToDDL('.cls_tfigrid .cls_lov_pa_style_of_printing', item.StyleofPrinting, item.StyleofPrinting);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_plant_register_no', item.PlantRegisteredNo, item.PlantRegisteredNo);
            $('.cls_tfigrid .cls_txt_pa_plant_register_no').val(item.PlantRegisteredNo);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_company_name', item.CompanyNameAddress, item.CompanyNameAddress);
            setValueToDDL(form_name + '.cls_lov_pa_material_group', item.MaterialGroup, item.MaterialGroup_TXT);
            setValueToDDL(form_name + '.cls_lov_pa_direction_of_sticker', item.Direction, item.Direction);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_specie', item.Specie, item.Specie);
            //debugger;

            PRIMARY_SIZE_DISPLAY_TEXT = item.PrimarySize
            $('.cls_tfigrid .cls_txt_pa_primary_size').val(item.PrimarySize);
            $('.cls_tfigrid .cls_txt_pa_container_type').val(item.ContainerType);
            $('.cls_tfigrid .cls_txt_pa_lid_type').val(item.LidType);
            $('.cls_tfigrid .cls_txt_pa_product_code').val(item.ProductCode);
            $('.cls_tfigrid .cls_txt_pa_pack_size').val(item.Packing);
            PACKING_STYLE_DISPLAY_TEXT = item.PackingStyle;
            $('.cls_tfigrid .cls_txt_pa_packing_style').val(item.PackingStyle);
            $('.cls_tfigrid .cls_txt_pa_fao_zone_multiple_static').val(item.FAOZone);
            $('.cls_tfigrid .cls_txt_pa_plant_multiple_static').val(item.Plant);
            $('.cls_tfigrid .cls_txt_pa_catching_area_multiple_static').val(item.CatchingArea);
            $('.cls_tfigrid .cls_txt_pa_catching_method_multiple_static').val(item.Catching_Method);
            $('.cls_tfigrid .cls_txt_pa_symbol_multiple_static').val(item.Symbol.replace('|', ';'));
            $('.cls_tfigrid .cls_input_pa_sec_pkg_production_plant').val(item.PlantAddress);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_printing_style_of_primary', item.PrintingStyleofPrimary, item.PrintingStyleofPrimary);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_printing_style_of_secondary', item.PrintingStyleofSecondary, item.PrintingStyleofSecondary);

            //PG
            setValueToDDL('.cls_tfigrid_pg .cls_lov_grade_of', item.Grandof, item.Grandof);
            setValueToDDL('.cls_tfigrid_pg .cls_lov_rsc_di_cut', item.RSC, item.RSC);
            setValueToDDL('.cls_tfigrid_pg .cls_lov_flute', item.Flute, item.Flute);
            setValueToDDL('.cls_tfigrid_pg .cls_lov_roll_sheet', item.RollSheet, item.RollSheet);
            $('.cls_tfigrid_pg .cls_txt_sheet_size').val(item.SheetSize);
            $('.cls_tfigrid_pg .cls_txt_dimension_of').val(item.Dimension);
            $('.cls_tfigrid_pg .cls_txt_pg_vendor_multiple_static').val(item.Vendor);
            $('.cls_tfigrid_pg .cls_lbl_pg_vendor_name').text(item.VendorName);
            setAccessoriesToTable(item.Accessories); // $('.cls_tfigrid_pg .cls_txt_acessories').val(item.Accessories);

            setValueToDDL('.cls_tfigrid_pg .cls_lov_printing_system', item.PrintingSystem, item.PrintingSystem);
            //$('.cls_tfigrid_pg .cls_lov_printing_system').val(item.PrintingSystem);
            $('.cls_container_taskform_igrid .cls_chk_Fixed_Desc').prop('checked', item.Fixed_Desc == 1 ? true : false);
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').prop('disabled', item.Fixed_Desc == 1 ? false : true);
            if (item.CustomerDesign.length > 2) {
                if (item.CustomerDesign.split('|')[0]=='Yes')
                    $("input[name=pa_rdo_customer_design][value='Yes']").prop('checked', true);
                else if (item.CustomerDesign.split('|')[0] == 'No')
                    $("input[name=pa_rdo_customer_design][value='No']").prop('checked', true);
                $('.cls_tfigrid .cls_txt_pa_customer_design').val(item.CustomerDesign.split('|')[1]);
            }
            if (item.CustomerSpec.length > 2) {
                if (item.CustomerSpec.split('|')[0] == 'Yes')
                    $("input[name=pa_rdo_customer_spec][value='Yes']").prop('checked', true);
                else if (item.CustomerSpec.split('|')[0] == 'No')
                    $("input[name=pa_rdo_customer_spec][value='No']").prop('checked', true);
                $('.cls_tfigrid .cls_txt_pa_customer_spec').val(item.CustomerSpec.split('|')[1]);
            }
            if (item.CustomerSize.length > 2) {
                if (item.CustomerSize.split('|')[0] == 'Yes')
                    $("input[name=pa_rdo_customer_size][value='Yes']").prop('checked', true);
                else if (item.CustomerSize.split('|')[0] == 'No')
                    $("input[name=pa_rdo_customer_size][value='No']").prop('checked', true);
                $('.cls_tfigrid .cls_txt_pa_customer_size').val(item.CustomerSize.split('|')[1]);
            }
            if (item.CustomerVendor.length > 2) {
                if (item.CustomerVendor.split('|')[0] == 'Yes')
                    $("input[name=pa_rdo_customer_nominate][value='Yes']").prop('checked', true);
                else if (item.CustomerVendor.split('|')[0] == 'No')
                    $("input[name=pa_rdo_customer_nominate][value='No']").prop('checked', true);
                $('.cls_tfigrid .cls_txt_pa_customer_nominate').val(item.CustomerVendor.split('|')[1]);
            }
            if (item.CustomerColor.length > 2) {
                if (item.CustomerColor.split('|')[0] == 'Yes')
                    $("input[name=pa_rdo_customer_nominate_colour][value='Yes']").prop('checked', true);
                else if (item.CustomerColor.split('|')[0] == 'No')
                    $("input[name=pa_rdo_customer_nominate_colour][value='No']").prop('checked', true);
                $('.cls_tfigrid .cls_txt_pa_customer_nominate_colour').val(item.CustomerColor.split('|')[1]);
            }
            if (item.CustomerScanable.length > 2) {
                if (item.CustomerScanable.split('|')[0] == 'Yes')
                    $("input[name=pa_rdo_customer_barcode][value='Yes']").prop('checked', true);
                else if (item.CustomerScanable.split('|')[0] == 'No')
                    $("input[name=pa_rdo_customer_barcode][value='No']").prop('checked', true);
                $('.cls_tfigrid .cls_txt_pa_customer_barcode').val(item.CustomerScanable.split('|')[1]);
            }
            if (item.CustomerBarcodeSpec.length > 2) {
                if (item.CustomerBarcodeSpec.split('|')[0] == 'Yes')
                    $("input[name=pa_rdo_customer_barcode_spec][value='Yes']").prop('checked', true);
                else if (item.CustomerBarcodeSpec.split('|')[0] == 'No')
                    $("input[name=pa_rdo_customer_barcode_spec][value='No']").prop('checked', true);
                $('.cls_tfigrid .cls_txt_pa_barcode_spec').val(item.CustomerBarcodeSpec.split('|')[1]);
            }
            var form_name = '.cls_tfigrid ';
            $(form_name + '.cls_txt_pa_first_info').val(item.FirstInfoGroup);
            $(form_name + '.cls_txt_pa_pic_mkt').val(item.PICMkt);
            $(form_name + '.cls_txt_pa_so').val(item.SO);
            $(form_name + '.cls_txt_pa_soplant').val(item.SOPlant);
            $(form_name + '.cls_txt_pa_country').val(item.Destination);
            $(form_name + '.cls_txt_pa_remark').val(item.Remark);
            $('.cls_tfigrid_pg .cls_txt_remark_note_of_pg').val(item.Note);
            $('.cls_tfigrid_pg .cls_txt_final_infogroup').val(item.FinalInfoGroup);
            setValueToDDL('.cls_tfigrid_pg .cls_lov_pkg_sec_material', item.SustainMaterial, item.SustainMaterial);
            setValueToDDL('.cls_container_taskform_igrid .cls_lov_assign', item.Assignee, item.Assignee_TXT);
            setValueToDDL('.cls_tfigrid_pg .cls_lov_pkg_sec_plastic', item.SustainPlastic, item.SustainPlastic);
            setValueToDDL('.cls_tfigrid_pg .cls_lov_pkg_sec_certification_standard', item.SustainCertSourcing, item.SustainCertSourcing);
            if (item.SustainReuseable.length != '') {
                if (item.SustainReuseable == 'Yes')
                    $("input[name=pg_rdo_pkg_sec_reusable][value='Yes']").prop('checked', true);
                else if (item.SustainReuseable == 'No')
                    $("input[name=pg_rdo_pkg_sec_reusable][value='No']").prop('checked', true);
            }
            if (item.SustainRecyclable.length != '') {
                if (item.SustainRecyclable == 'Yes')
                    $("input[name=pg_rdo_pkg_sec_recyclable][value='Yes']").prop('checked', true);
                else if (item.SustainRecyclable == 'No')
                    $("input[name=pg_rdo_pkg_sec_recyclable][value='No']").prop('checked', true);
            }
            if (item.SustainComposatable.length != '') {
                if (item.SustainComposatable == 'Yes')
                    $("input[name=pg_rdo_pkg_sec_composatable][value='Yes']").prop('checked', true);
                else if (item.SustainComposatable == 'No')
                    $("input[name=pg_rdo_pkg_sec_composatable][value='No']").prop('checked', true);
            }
            if (item.SustainCertification.length != '') {
                if (item.SustainCertification == 'Yes')
                    $("input[name=pg_rdo_pkg_sec_certification][value='Yes']").prop('checked', true);
                else if (item.SustainCertification == 'No')
                    $("input[name=pg_rdo_pkg_sec_certification][value='No']").prop('checked', true);
            }
            $('.cls_tfigrid_pg .cls_txt_pkg_sec_other').val(item.SustainOther);
            $('.cls_tfigrid_pg .cls_txt_pkg_sec_weight').val(item.SusSecondaryPKGWeight);
            $('.cls_tfigrid_pg .cls_txt_pkg_sec_recycled_content').val(item.SusRecycledContent);

            IsStatusApp = item.StatusApp;
            setRequireFieldTaskformIGridPA(item.MaterialGroup);
            getPakcingStyleByFG(item.ProductCode);
            bindLOVPlantRegister();
            bindLOVCompanyAddress();
            //bindIGridMaterialStep();
            //$('.cls_tfigrid .cls_input_pa_brand').val(item.Brand);
            //bindIGridUser();
        }
    }

    bindIGridUser();
}

//function get_required_field() {
//    var myurl = suburl + "/api/master/getrequiredfield?data.ID='" + $('.cls_tfigrid .cls_lov_pa_material_group').val()+"'";
//    var mytype = 'GET';
//    var mydata = null;
//    myAjax(myurl, mytype, mydata, callback_bindRequiredField);
//}
//function callback_bindRequiredField(res) {
//    if (res.data != null && res.data) {

//    }
//}


//function bindIGridMaterialStep()
//{
//    var myurl = suburl + '/api/taskform/igrid/lov/GetTransApproveIGrid?data.MatDoc=' + IGridSAPMaterialId;
//    var mytype = 'GET';
//    var mydata = null;
//    myAjax(myurl, mytype, mydata, callback_bindIGridMaterialStep);
//}
//function callback_bindIGridMaterialStep(res) {
//    if (res.data != null && res.data.length > 0 )
//    {

//    }
//}

function bindIGridUser() {
    var myurl = suburl + '/api/master/getigriduser?data.Matdoc=' + IGridSAPMaterialId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bindIGridUser);
}

function callback_bindIGridUser(res) {
    IsSaveCompleteInfoGroupAlready = false;
    eventSaveCompleteInfoGroup = false;
    IsStep_InfoGroup = false;
    if (res.data != null && res.data.length > 0 ) 
    {

        // start เชคว่า WF นี้อยู่ Step ไหน
        var IsStep_PA = true;
        var IsStep_PG = false;
        var IsStep_PG_Assign = false;
        var IsStep_PA_Approve = false;
        var IsStep_PG_Approve = false;
        //var IsStep_InfoGroup = false;
        var IsCancel = false;
        var IsComplete = false;
        var Assignee = "";
        var CreateBy = "";
        var CurrentUser = "";
        var StatusApp = "";


        if (res.data[0].ListTransApproves != null && res.data[0].ListTransApproves.length > 0) {
            TransApprove = res.data[0].ListTransApproves[0];
            IsStep_PA = TransApprove.IsStep_PA == "X" ? true : false;
            IsStep_PG = TransApprove.IsStep_PG == "X" ? true : false;
            IsStep_PG_Assign = TransApprove.IsStep_PG_Assign == "X" ? true : false;
            IsStep_PA_Approve = TransApprove.IsStep_PA_Approve == "X" ? true : false;
            IsStep_PG_Approve = TransApprove.IsStep_PG_Approve == "X" ? true : false;
            IsStep_InfoGroup = TransApprove.IsStep_InfoGroup == "X" ? true : false;
            IsCancel = TransApprove.IsCancel == "X" ? true : false;
            IsComplete = TransApprove.IsComplete == "X" ? true : false;
            Assingee = TransApprove.Assingee;
            CreateBy = TransApprove.CreateBy;
            CurrentUser = TransApprove.CurrentUser;
            StatusApp = TransApprove.StatusApp;
            IsSaveCompleteInfoGroupAlready = TransApprove.CntCompleteInfoGroup > 0;

        } else
        {
           
        }
        // end เชคว่า WF นี้อยู่ Step ไหน



        $('.cls_tfigrid_pg .cls_txt_pg_comment').prop('disabled', true);
        $('.cls_tfigrid .cls_txt_pa_comment').prop('disabled', true);
        $('.cls_tfigrid .cls_btn_pa_save').prop('disabled', true);
        //$('.cls_tfigrid .cls_btn_pa_clear').prop('disabled', true);
        $('.cls_tfigrid .cls_btn_pa_submit').prop('disabled', true);
        $('.cls_tfigrid .cls_btn_pa_reject').prop('disabled', true);
        $('.cls_tfigrid .cls_btn_pa_remove').prop('disabled', true);

        $('.cls_tfigrid_pg .cls_btn_save').prop('disabled', true);
        $('.cls_tfigrid_pg .cls_btn_submit').prop('disabled', true);
        $('.cls_tfigrid_pg .cls_btn_assign').prop('disabled', true);
        $('.cls_tfigrid_pg .cls_btn_reject').prop('disabled', true);

        $('.cls_chk_Fixed_Desc').hide();

        $('.cls_container_taskform_igrid .cls_lov_assign').prop('disabled', true)
        $('.cls_tfigrid_pg .cls_div_complete_infogroup').hide();

        var isPA = false;
        var isPG = false;
        var isPA_Approve = false;
        var isPG_Approve = false;

        if (!isEmpty(res.data[0].fn)) {
            var roles = res.data[0].fn.split(",")

            $.each(roles, function (i, v) {
                //isPA = false;
                //isPG = false;
                //isPA_Approve = false;
                //isPG_Approve = false;

                switch (v.trim()) {
                    case 'PA':
                        isPA = true;
                        break;
                    case 'PA_Approve':
                        isPA_Approve = true;
                        break;
                    case 'PG':
                        isPG = true;
                        break;
                    case 'PG_Approve':
                        isPG_Approve = true;
                        break;
                    default:
                        break;
                }
            });
          
        }

        if (IsCancel || IsComplete) {
            setDisablePG();
            setDisablePA();
            setDisableAttachedfiles();
            setDisableHeader();

            $('.cls_tfigrid_pg .cls_div_complete_infogroup').show();
            $('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').prop('disabled', true);
            $('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').prop('checked', true);
        } else if (IsStep_PA && isPA && (CreateBy == CurrentUser || IGridSAPMaterialId == 0)) {
            setButtonStepPA();
            setDisablePG();
            if (isPA_Approve) {
                $('.cls_chk_Fixed_Desc').show();
            }


        } else if (IsStep_PA_Approve && isPA_Approve) {
            setButtonStepPA_Approve();
            setDisablePG();
            $('.cls_chk_Fixed_Desc').show();
        } else if (IsStep_PG && isPG) {
            setButtonStepPG();
            setDisablePA();
            setDisableHeader();


        } else if (IsStep_PG_Assign && isPG_Approve)
        {
            if (isStringNullOrEmpty($('.cls_container_taskform_igrid .cls_lov_assign').val())) {
                $('.cls_container_taskform_igrid .cls_lov_assign').prop('disabled', false);
            }

            setButtonStepPG_Assign();
            setDisablePA();
            setDisableHeader();

            $('.cls_tfigrid_pg .cls_btn_reject').prop('disabled', false);
           // $('.cls_tfigrid_pg .cls_btn_assign').prop('disabled', false);

        } else if (IsStep_PG_Assign && isPG) {
            setButtonStepPG_Assign();
            setDisablePA();
            setDisableHeader();

        } else if (IsStep_PG_Approve && isPG_Approve) {

            if (isStringNullOrEmpty($('.cls_container_taskform_igrid .cls_lov_assign').val()))
            {
                $('.cls_container_taskform_igrid .cls_lov_assign').prop('disabled', false);
            }

           
            setButtonStepPG_Approve();
            setDisablePA();
            setDisableHeader();

        } else if (IsStep_InfoGroup && isPG_Approve) {
            setDisablePG();
            setDisablePA();
            setDisableAttachedfiles();
            setDisableHeader();
            $('a[href="#view_pg"]').tab('show');

            if (StatusApp == "4")  // statusap 4 is created on SAP already.
            {
                $('.cls_tfigrid_pg .cls_div_complete_infogroup').show();
            }

            if (IsSaveCompleteInfoGroupAlready) {
                $('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').prop('disabled', true);
                $('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').prop('checked', true);
            } else
            {
                $('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').prop('disabled', false);
                $('.cls_tfigrid_pg .cls_txt_final_infogroup').prop('disabled', false);
                $('.cls_tfigrid_pg .cls_txt_final_infogroup').prop('readonly', false);
            }
        
          
            $('.cls_tfigrid_pg .cls_btn_save').prop('disabled', false);
        } else
        {
            setDisablePG();
            setDisablePA();
            setDisableAttachedfiles();
            setDisableHeader();
        }

        //if (isPA) {

        //    setDisalbePG()
        //}

        //if (isPG) {
        //    setDisablePA()
        //}

    }
}



function setButtonStepPA()
{

    $('a[href="#view_pa"]').tab('show');
    $('.cls_tfigrid .cls_btn_pa_save').prop('disabled', false);
    //$('.cls_tfigrid .cls_btn_pa_clear').prop('disabled', false);
    $('.cls_tfigrid .cls_btn_pa_submit').prop('disabled', false);
    // $('.cls_tfigrid .cls_btn_pa_reject').prop('disabled', false);

    if (IGridSAPMaterialId != 0)
    {
        $('.cls_tfigrid .cls_btn_pa_remove').prop('disabled', false);
    }
 
    $('.cls_tfigrid_pg .cls_btn_save').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_btn_submit').prop('disabled', true);
    $('#cls_btn_pa_submit').html('Submit');
    //$('.cls_report_overview .btn_over_option').show();

    //$('.cls_container_taskform_igrid .cls_lov_assign').prop('disabled', true)
}

function setButtonStepPA_Approve()
{
    $('a[href="#view_pa"]').tab('show');
    $('.cls_tfigrid .cls_btn_pa_save').prop('disabled', false);
    //$('.cls_tfigrid .cls_btn_pa_clear').prop('disabled', false);
    $('.cls_tfigrid .cls_btn_pa_submit').prop('disabled', false);
    $('.cls_tfigrid .cls_btn_pa_reject').prop('disabled', false);

    $('.cls_tfigrid_pg .cls_btn_save').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_btn_submit').prop('disabled', true);
    //$('.cls_report_overview .cls_over_btn_Remove').show();
    $('#cls_btn_pa_submit').html('Approve');
    IsApprove_step = true;
    //$('.cls_container_taskform_igrid .cls_lov_assign').prop('disabled', true)

    //if (IsStep_PA_Approve == true) {
    //    $('.cls_chk_Fixed_Desc').show();
    //}
}


function setButtonStepPG()
{
    $('a[href="#view_pg"]').tab('show');
    $('.cls_tfigrid .cls_btn_pa_save').prop('disabled', true);
    //$('.cls_tfigrid .cls_btn_pa_clear').prop('disabled', true);
    $('.cls_tfigrid .cls_btn_pa_submit').prop('disabled', true);

    $('.cls_tfigrid_pg .cls_btn_save').prop('disabled', false);
    $('.cls_tfigrid_pg .cls_btn_submit').prop('disabled', false);
    $("#cls_btn_pg_submit").html('Submit');
}


function setButtonStepPG_Assign() {
    $('a[href="#view_pg"]').tab('show');
    if ($('.cls_container_taskform_igrid .cls_lov_assign').val() == null) {
        $('.cls_tfigrid_pg .cls_btn_assign').prop('disabled', false);
    } else {
        $('.cls_tfigrid .cls_btn_pa_save').prop('disabled', true);
        //$('.cls_tfigrid .cls_btn_pa_clear').prop('disabled', true);
        $('.cls_tfigrid .cls_btn_pa_submit').prop('disabled', true);

        $('.cls_tfigrid_pg .cls_btn_save').prop('disabled', false);
        $('.cls_tfigrid_pg .cls_btn_submit').prop('disabled', false);
    }
    $('.cls_tfigrid_pg .cls_btn_reject').prop('disabled', true);
    //$('.cls_tfigrid_pg .cls_btn_pa_submit').val('Approve');
   // $("#cls_btn_pg_submit").html("Approve");
}


function setButtonStepPG_Approve()
{
    $('a[href="#view_pg"]').tab('show');
    if ($('.cls_container_taskform_igrid .cls_lov_assign').val() == null) {
        $('.cls_tfigrid_pg .cls_btn_assign').prop('disabled', false);
    } else {
        $('.cls_tfigrid .cls_btn_pa_save').prop('disabled', true);
        //$('.cls_tfigrid .cls_btn_pa_clear').prop('disabled', true);
        $('.cls_tfigrid .cls_btn_pa_submit').prop('disabled', true);

        $('.cls_tfigrid_pg .cls_btn_save').prop('disabled', false);
        $('.cls_tfigrid_pg .cls_btn_submit').prop('disabled', false);
    }
    $('.cls_tfigrid_pg .cls_btn_reject').prop('disabled', false);
    //$('.cls_tfigrid_pg .cls_btn_pa_submit').val('Approve');
    //debugger;
    $("#cls_btn_pg_submit").html("Approve");
    IsApprove_step = true;
}





function setDisablePA()
{
    $('.cls_tfigrid .form-control').prop('readonly', true);
    $('.cls_tfigrid .cls_lov_pa_material_group').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_type_of').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_type_of_two').prop('disabled', true);

    $('.cls_tfigrid .cls_lov_pa_typeofprimary').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_brand').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_plant_register_no').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_pms_colour').prop('disabled', true);

    $('.cls_tfigrid .cls_lov_pa_company_name').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_process_colour').prop('disabled', true);

    $('.cls_tfigrid .cls_lov_pa_catching_period').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_total_colour').prop('disabled', true);

    $('.cls_tfigrid .cls_lov_pa_scientific_name').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_direction_of_sticker').prop('disabled', true);

    $('.cls_tfigrid .cls_lov_pa_style_of_printing').prop('disabled', true);


    $('.cls_tfigrid .cls_lov_pa_specie').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_printing_style_of_primary').prop('disabled', true);
    $('.cls_tfigrid .cls_lov_pa_printing_style_of_secondary').prop('disabled', true);


    $('input[name=cls_rdo_haeder_tfartwork_change_point]').attr("disabled", true);
    $('input[name=pa_rdo_customer_design]').attr("disabled", true);
    $('input[name=pa_rdo_customer_spec]').attr("disabled", true);
    $('input[name=pa_rdo_customer_size]').attr("disabled", true);
    $('input[name=pa_rdo_customer_nominate]').attr("disabled", true);
    $('input[name=pa_rdo_customer_nominate_colour]').attr("disabled", true);
    $('input[name=pa_rdo_customer_barcode]').attr("disabled", true);
    $('input[name=pa_rdo_customer_barcode_spec]').attr("disabled", true);

    $('.cls_tfigrid_pg .cls_txt_pg_comment').prop('disabled', false);


    $('.cls_tfigrid .btn-warning').hide();
}
function setDisablePG()
{
    $('.cls_tfigrid_pg .form-control').prop('readonly', true);
    $('.cls_tfigrid_pg .cls_lov_grade_of').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_lov_rsc_di_cut').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_lov_printing_system').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_lov_flute').prop('disabled', true);

    $('.cls_tfigrid_pg .cls_lov_roll_sheet').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_chk_Complete_InfoGroup').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_lov_pkg_sec_material').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_lov_pkg_sec_plastic').prop('disabled', true);
    $('.cls_tfigrid_pg .cls_lov_pkg_sec_certification_standard').prop('disabled', true);

    $('input[name=pg_rdo_pkg_sec_certification]').attr("disabled", true);
    $('input[name=pg_rdo_pkg_sec_reusable]').attr("disabled", true);
    $('input[name=pg_rdo_pkg_sec_recyclable]').attr("disabled", true);
    $('input[name=pg_rdo_pkg_sec_composatable]').attr("disabled", true);

    $('.cls_tfigrid_pg .cls_btn_add_pg_accessories').hide();
    $('.cls_tfigrid_pg .cls_btn_pg_vendor_multiple_static').hide();
    $('.cls_tfigrid_pg .cls_img_edit_pg_accessories').hide();
    $('.cls_tfigrid_pg .cls_img_delete_pg_accessories').hide();


    $('.cls_tfigrid .cls_txt_pa_comment').prop('disabled', false);
}

function setDisableAttachedfiles()
{
    $('.fileupload-buttonbar').hide();
}


function setDisableHeader() {
    
    $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_doc').prop('disabled', true);
    $('.cls_container_taskform_igrid .cls_ddl_option_condition').prop('disabled', true);
    $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_aw_no').prop('disabled', true);
}


function editAccessoriesToTable(accessories) {


    $(".cls_tfigrid_pg .table_pg_accessories_mutli_value tbody tr").each(function (index) {

        if ($(this).find('.cls_txt_pg_accessories').hasClass('cls_edit')) {
            $(this).find('.cls_txt_pg_accessories').val(accessories);
            $(this).find('.cls_txt_pg_accessories').removeClass('cls_edit');
            $(this).find('.cls_txt_pg_accessories').removeClass('fontRed');
        }

    });


    $('#btn_pg_accessoreis').text('Add');
}


function addAccessoriesToTable(accessories)
{

    var obj = $('.tr_pg_accessories_mutli_value').clone().removeClass('tr_pg_accessories_mutli_value').removeClass('cls_cn_hide');

    $('.cls_tfigrid_pg .table_pg_accessories_mutli_value tbody').append(obj);
    //var accessories = $('.cls_tfigrid_pg .cls_txt_acessories').val();
    obj.find('.cls_txt_pg_accessories').val(accessories);

    $(".cls_tfigrid_pg .cls_img_delete_pg_accessories").click(function () {
        $(this).closest('tr').remove();
    });


    $(".cls_tfigrid_pg .cls_img_edit_pg_accessories").click(function (e) {
       // $(this).closest('tr').remove();

        // clear class all rows
        $(".cls_tfigrid_pg .table_pg_accessories_mutli_value tbody tr").each(function (index) {
           
            $(this).find('.cls_txt_pg_accessories').removeClass('cls_edit');
            $(this).find('.cls_txt_pg_accessories').removeClass('fontRed');

        });



        // set class for edit row
        var obj2 = $(this).closest('tr').find('.cls_txt_pg_accessories');

        obj2.addClass('fontRed');
        obj2.addClass('cls_edit');
        $('.cls_txt_acessories').val(obj2.val());
        $('#btn_pg_accessoreis').text('Edit');

        //alert(obj2.val());
        //if (obj2.prop("disabled")) {
        //    obj2.prop("disabled", false);
        //} else
        //{
        //    obj2.prop("disabled", true);
        //}

     

    });


}


function setAccessoriesToTable(accessories) {

    if (accessories != null && accessories != "")
    {
        var data = accessories.split(";");

        $.each(data, function (i, v) {
            if (v != "")
            {
                addAccessoriesToTable(v);
            }
        });
    }
}


function getAccessoriesToTable()
{
    var accessories = "";

    $(".cls_tfigrid_pg .table_pg_accessories_mutli_value tbody tr").each(function (index) {

        if ($(this).find('.cls_txt_pg_accessories').val() != "") {
            if (accessories != "") accessories += ";";
            //accessories += $(this).find('.cls_txt_pg_accessories').val().trim();
            accessories += $(this).find('.cls_txt_pg_accessories').val();
        }


    });

    return accessories;

}



function getIGridPA_MatGroupValue(obj) {
    var selections = $(obj).select2('data');
    var v = $(obj)[0].value;
    var matgroup_id = 0;
    var matgroup_value = "";
    if (selections.length > 0) {
        if (selections[0].ID != -1 && selections[0].ID != undefined) {
            setValueToDDL('.cls_tfigrid .cls_lov_pa_material_group', selections[0].ID, selections[0].DISPLAY_TXT);
            matgroup_value = v; //selections[0].VALUE;
            setRequireFieldTaskformIGridPA(matgroup_value);
            matgroup_id = selections[0].ID;
        }
        //else if (defaultPA_MaterialGroupData[v] != undefined) {
        //    setValueToDDL('.cls_tfigrid .cls_lov_pa_material_group', defaultPA_MaterialGroupData[v].ID, defaultPA_MaterialGroupData[v].DISPLAY_TXT);
        //    matgroup_value = defaultPA_MaterialGroupData[v].VALUE;
        //    setRequireFieldTaskformIGridPA(matgroup_value);
        //    matgroup_id = defaultPA_MaterialGroupData[v].ID;
        //}
    }

    $(".cls_tfigrid .cls_lov_pa_type_of").val('').trigger("change"); 
    $(".cls_tfigrid .cls_lov_pa_type_of_two").val('').trigger("change");
 
    
}


$("#modal_igrid_pa_primary .cls_btn_search").click(function (e) {


    var default_value = $('.cls_tfigrid .cls_txt_pa_primary_size').val();
    var where = "";


    where = "&data.Code=" + $('#modal_igrid_pa_primary .cls_txt_primary_size_code').val()
        + "&data.Can=" + $('#modal_igrid_pa_primary .cls_txt_primary_size_can').val()
        + "&data.Description=" + $('#modal_igrid_pa_primary .cls_txt_primary_size_can_desciption').val()
        + "&data.LidType=" + $('#modal_igrid_pa_primary .cls_txt_primary_size_lid_type').val()
        + "&data.ContainerType=" + $('#modal_igrid_pa_primary .cls_txt_primary_size_container_type').val()
        + "&data.DescriptionType=" + $('#modal_igrid_pa_primary .cls_txt_primary_size_description_type').val();


    url = suburl + "/api/lov/primarysizeIGrid2?" + where;
    bindSuggestPrimarySizePopUp(false, "1", url, '#TableId_igrid_pa_primary', '', default_value);
      

});


$("#modal_igrid_pa_primary .cls_btn_clear").click(function (e) {
    $('.cls_div_igrid_parimary_size_search input[type=text]').val('');

});


function bindLOVPlantRegister()
{
    bind_lov_param('.cls_tfigrid .cls_lov_pa_plant_register_no', '/api/taskform/igrid/lov/PlantRegisteredIGrid', 'data.DISPLAY_TXT', ['STR_PRODUCT_CODE'], ['.cls_txt_pa_product_code'], "", callback_LOVPlantRegister);
}

function callback_LOVPlantRegister(res)
{
    if (res != null )
    {
        $('.cls_tfigrid .cls_txt_pa_plant_register_no').val($('.cls_tfigrid .cls_lov_pa_plant_register_no option:selected').text());
        $('.cls_tfigrid .cls_lov_pa_company_name').val('').change();
        bindLOVCompanyAddress();
    }
}

function bindLOVCompanyAddress()
{
    //'.cls_tfigrid .cls_lov_pa_plant_register_no'
    debugger;
    var res = $('.cls_tfigrid .cls_txt_pa_plant_register_no').val();
    var cod = $(".cls_tfigrid .cls_txt_pa_product_code").val();
    bind_lov_param('.cls_tfigrid .cls_lov_pa_company_name', '/api/taskform/igrid/lov/CompanyAddressIGrid?data.ProductCode=' + cod, 'data.DISPLAY_TXT', ['RegisteredNo'], ['.cls_txt_pa_plant_register_no']);
}



function ValidateKey() {
    var key = window.event.keyCode;
    var allowed = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ -_()[]/+&,.1234567890';
    
    return allowed.indexOf(String.fromCharCode(key)) != -1;
}


function onKeyValidate(e, charVal) {
    var keynum;
    var keyChars = /[\x00\x08]/;
    var validChars = new RegExp(charVal);
    if (window.event) {
        keynum = e.keyCode;
    }
    else if (e.which) {
        keynum = e.which;
    }
    var keychar = String.fromCharCode(keynum);
    if (!validChars.test(keychar) && !keyChars.test(keychar)) {
        return false
    } else {
        return keychar;
    }
}

//---------by aof - - start
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
    } else {
        val = 0
    }


    val = val.toFixed(point).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");

    return val
}

  //---------by aof 
