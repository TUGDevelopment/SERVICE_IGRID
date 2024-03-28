//const { bind } = require("angular");
$(document).ready(function () {
    typecheck_pa_vap = "load";
    $('a[href="#view_pa"]').tab('show');
    //bindSuggestPackingStylePopUp(false, "0");
    $('.cls_container_taskform_igrid .cls_lbl_pa_packing_style').html("Packing style :");
    $('.cls_container_taskform_igrid .cls_lbl_pa_pack_size').html("Pack size :");
    //bind_lov_param('.cls_tfigrid', '/api/lov/artworkno', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    //bind_lov_param('.cls_tfigrid .cls_lov_pa_type_of', '/api/lov/pa/typeof', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_type_of');
    //bind_lov('.cls_tfigrid .cls_lov_pa_packing_style', '/api/lov/PackStyleIGrid','data.RefStyle');
    bind_lov('.cls_tfigrid .cls_lov_pa_material_group', '/api/lov/packtypeIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid .cls_lov_brand', '/api/lov/brandIGrid', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_tfigrid .cls_lov_pa_pms_colour', '/api/lov/PMSColourIGrid', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_tfigrid .cls_lov_pa_process_colour', '/api/lov/processcolourIGrid', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_tfigrid .cls_lov_pa_total_colour', '/api/lov/pa/totalcolourIGrid', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);

    bind_lov_param('.cls_tfigrid .cls_lov_pa_type_of', '/api/lov/TypeOfIGrid?data.MaterialType=0', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_tfigrid .cls_lov_pa_type_of_two', '/api/lov/typeof2IGrid?data.MaterialType=2', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    bind_lov_param('.cls_tfigrid .cls_lov_pa_style_of_printing', '/api/lov/StyleofPrintingIGrid', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group']);
    PA_ProductType = 'X';

    bind_lov_param('.cls_tfigrid .cls_lov_pa_plant_register_no', '/api/lov/PlantRegisteredIGrid', 'data.DISPLAY_TXT',['plant'], ['.cls_txt_pa_plant_multiple_static']);
    bind_lov('.cls_tfigrid .cls_lov_pa_company_name', '/api/lov/CompanyAddressIGrid', 'data.DISPLAY_TXT');
    //bind_lov_param('.cls_tfigrid .cls_lov_pa_pms_colour', '/api/lov/pa/pms', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_pms_colour');
    //bind_lov_param('.cls_tfigrid .cls_lov_pa_process_colour', '/api/lov/pa/processcolour', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_process_colour');
    //bind_lov_param('.cls_tfigrid .cls_lov_pa_total_colour', '/api/lov/pa/totalcolour', 'data.DISPLAY_TXT', ["MATERIAL_GROUP_ID"], ['.cls_lov_pa_material_group'], '.cls_input_pa_total_colour');

    bind_lov('.cls_tfigrid .cls_lov_pa_typeofprimary', '/api/lov/TypeofPrimaryIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid .cls_lov_pa_catching_period', '/api/lov/CatchingPeriodIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid .cls_lov_pa_scientific_name', '/api/lov/ScientificNameIGrid', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_tfigrid .cls_lov_pa_direction_of_sticker', '/api/lov/DirectionIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    bind_lov('.cls_tfigrid .cls_lov_pa_specie', '/api/lov/SpecieIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid .cls_lov_pa_printing_style_of_primary', '/api/lov/pa/printingstyleofprimary', 'data.DISPLAY_TXT');
    bind_lov('.cls_tfigrid .cls_lov_pa_printing_style_of_secondary', '/api/lov/PrintingSystemIGrid', 'data.DISPLAY_TXT');

    bind_lov('.cls_lov_pg_packaging_type', '/api/lov/packtype', 'data.DISPLAY_TXT');
    $('.cls_lov_pg_packaging_type').on("change", function () {
        $('.cls_lov_pg_style').val('').trigger("change");
        $('.cls_lov_pg_pim_color').val('').trigger("change");
    });
    bind_lov_param('.cls_lov_pg_style', '/api/lov/style', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type']);
    bind_lov('.cls_lov_pg_pim', '/api/lov/printsystem', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_lov_pg_pim_color', '/api/lov/numberofcolor', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.cls_lov_pg_packaging_type']);
    bind_lov('.cls_lov_pg_box_color', '/api/lov/boxcolor', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_pg_coating', '/api/lov/coating', 'data.DISPLAY_TXT');

    bind_lov_param('.cls_lov_grade_of', '/api/lov/GradeofIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_tfigrid .cls_lov_pa_material_group']);
    bind_lov_param('.cls_lov_rsc_di_cut', '/api/lov/dicutIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_tfigrid .cls_lov_pa_material_group']);
    bind_lov_param('.cls_lov_roll_sheet', '/api/lov/RollSheet', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_tfigrid .cls_lov_pa_material_group']);
    bind_lov('.cls_lov_vendor', '/api/lov/vendorhasuser', 'data.vendor_name');
    bind_lov('.cls_lov_pkg_sec_plastic', '/api/lov/SustainPlasticIGrid', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_pkg_sec_material', '/api/lov/SustainMaterialIGrid', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_lov_pa_printing_style_of_secondary', '/api/lov/SustainCertSourcingIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    
    $('.cls_task_form_pg .cls_lov_vendor').prop('disabled', true);
    bind_lov_param('.cls_lov_flute', '/api/lov/FluteIGrid', 'data.DISPLAY_TXT', ["MaterialGroup"], ['.cls_lov_pa_material_group']);
    $('.cls_tfigrid .cls_lov_pa_material_group').on("change", function () {
        setRequireFieldTaskformArtworkPA($('.cls_tfigrid .cls_lov_pa_material_group').val());
    });
    
    bindDataIGridSAPMaterial();
    bindDataIGridHistory();
    //$(document).on('click', '#TableId_igrid_pa_primary .cls_chk_sendtopp', function (e) {
    //    if ($(this).is(':checked')) {
    //        TableId_igrid_pa_primary.rows($(this).closest('tr')).select();
    //    }
    //    else {
    //        TableId_igrid_pa_primary.rows($(this).closest('tr')).deselect();
    //    }
    //    $(this).closest('tr').find('.cls_chk_sendtopp').prop('checked', this.checked);
    //});
    
    $("#modal_igrid_pa_plant .cls_btn_igrid_pa_plant_submit").click(function (e) {
        debugger;
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
        debugger;
        var table = $('#TableId_igrid_pg_vendor').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            const array = [];
            for (i = 0; i < tblData.length; i++) {
                //var three_p_id = tblData[i].Id;
                array.push(tblData[i].Code);
            }

            var _a = array.join(';');
            $('.cls_task_form_pg .cls_txt_pg_vendor_multiple_static').val(_a);

            $("#modal_igrid_pg_vendor .cls_btn_igrid_pg_vendor_close").click();
        } else {
            alertError2("Please select at least 1 item.");
        }

    });
    
    $("#modal_igrid_pa_primary .cls_btn_igrid_pa_terminate_submit").click(function (e) {
        debugger;
        var table = $('#TableId_igrid_pa_primary').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            //var three_p_id = tblData[0].Id;
            $('.cls_container_taskform_igrid .cls_txt_pa_primary_size_id').val(tblData[0].Id);
            $('.cls_container_taskform_igrid .cls_txt_pa_primary_size').val(tblData[0].Can + ":" + tblData[0].Description);
            $('.cls_container_taskform_igrid .cls_txt_pa_container_type').val(tblData[0].ContainerType);
            $('.cls_container_taskform_igrid .cls_txt_pa_lid_type').val(tblData[0].LidType);
            $("#modal_igrid_pa_primary .cls_btn_igrid_pa_terminate_close").click();
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
        } else {
            alertError2("Please select at least 1 item.");
        }
    });

    $("#modal_igrid_pa_catching_method .cls_btn_igrid_pa_catching_method_submit").click(function (e) {
        debugger;
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
        debugger;
        var table = $('#TableId_igrid_pa_symbol').DataTable();
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
        debugger;
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
        bindSuggestFAOZonePopUp(false, "1", suburl + "/api/lov/FAOZoneIGrid", '#TableId_igrid_pa_faozone');
        $('#modal_igrid_pa_faozone').modal({ backdrop: 'static', keyboard: true });
    });

    $(".cls_tfigrid .cls_btn_pa_symbol_multiple_static").click(function (e) {
        $('#modal_igrid_pa_symbol .modal-title').html("Symbol");
        bindSuggestSymbolPopUp(false, "1");
        $('#modal_igrid_pa_symbol').modal({ backdrop: 'static', keyboard: true });
    });
   
    $(".cls_tfigrid .cls_btn_pa_catching_method_multiple_static").click(function (e) {
        bindSuggestCatchingMethodPopUp(false, "1");
        $('#modal_igrid_pa_catching_method').modal({ backdrop: 'static', keyboard: true });
    });
    $(".cls_task_form_pg .cls_btn_pg_vendor_multiple_static").click(function (e) {
        bindSuggestVendorPopUp(false, "1");
        $('#modal_igrid_pg_vendor').modal({ backdrop: 'static', keyboard: true });
    });

    $(".cls_tfigrid .cls_btn_pa_plant_multiple_static").click(function (e) {
        url = suburl + "/api/lov/PlantIGrid";
        bindSuggestPlantPopUp(false, "1", url, '#TableId_igrid_pa_plant');
        $('#modal_igrid_pa_plant').modal({ backdrop: 'static', keyboard: true });
    });
    $(".cls_tfigrid .cls_btn_pa_catching_area_multiple_static").click(function (e) {
        url = suburl + "/api/lov/CatchingPeriodIGrid";
        bindSuggestCatchingAreaPopUp(false, "1", url, '#TableId_igrid_pa_catching_area');
        $('#modal_igrid_pa_catching_area').modal({ backdrop: 'static', keyboard: true });
    });
    $(".cls_tfigrid .cls_btn_pa_packing_style").click(function (e) {
        debugger;
        var pa_typeofprimary = $('.cls_tfigrid .cls_lov_pa_typeofprimary').text();
        if (!isEmpty(pa_typeofprimary)) {
            //TableId_igrid_pa_packing_style.destroy();
            //TableId_igrid_pa_packing_style.ajax.reload();
            bindSuggestPackingStylePopUp(false, "1");
            
            $('#modal_igrid_pa_packing_style').modal({ backdrop: 'static', keyboard: true });
        }
        else {
            alertError2("Please input request TypeofPrimary.");
        }
    });
    $('.cls_tfigrid .cls_btn_pa_save').click(function (e) {
        saveIGridSAPMaterial(true);
    });
    $(".cls_tfigrid .cls_btn_pa_primary_size").click(function (e) {
       
        url = suburl + "/api/lov/primarysizeIGrid";
        bindSuggestMaterialPopUp(false, "1", url, '#TableId_igrid_pa_primary');
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
});

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
    item['Primarysize'] = $(form_name + '.cls_input_pa_primary_size').val();

    jsonObj.data = item;
    var myurl = '/api/taskform/igrid/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjax(myurl, mytype, mydata, callbackSaveIGridSAPMaterial, '', false, is_showmsg);

}
function callbackSaveIGridSAPMaterial(res) {
    //ARTWORK_SUB_PA_ID = res.data[0].ARTWORK_SUB_PA_ID;
    bindDataIGridSAPMaterial();
}


function setRequireFieldTaskformArtworkPA(matgroup_value) {
    $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html("Style of printing :");
    $('.cls_container_taskform_igrid .cls_lbl_pa_direction_of_sticker').html("Direction of sticker :");
    $('.cls_container_taskform_igrid .cls_lbl_pa_customer_design').html("Customer's design :");

    
    var lbl_spanStar = "<span style=\"color: red;\">*</span>";
    switch (matgroup_value) {
        case "K":
            $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html("Style of printing " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            break;
        case "L":
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            break;
        case "P":
            $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html("Style of printing " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            break;
        case "J":
            $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html("Style of printing " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lbl_pa_direction_of_sticker').html("Direction of sticker " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", true);   // by aof to disable packing style
            //$('.cls_container_taskform_igrid .cls_lbl_pa_customer_design').html("Customer's design " + lbl_spanStar + ":");
            break;
        default:
            $('.cls_container_taskform_igrid .cls_lbl_pa_packing_style').html("Packing style " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lbl_pa_pack_size').html("Pack size " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lbl_pa_style_of_printing').html("Style of printing " + lbl_spanStar + ":");
            $('.cls_container_taskform_igrid .cls_lov_pa_packing_style').prop("disabled", false);   // by aof to disable packing style
            break;
    }
}

function bindDataIGridHistory() {
    //debugger;
    //table.clear().destroy(); // Clear all rows and remove all columns
    //$('#TableId_igrid_pa_faozone').empty(); // Empty the table element to remove any remaining rows and columns
    table_igrid = $('#table_taskform_history').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
                url: '/api/taskform/igrid/History?data.Id=' + IGridSAPMaterialId,
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
        //"scrollX": true,
        columns:
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                { data: "Name", "className": "cls_nowrap" },
                { data: "Result", "className": "cls_nowrap" },
                { data: "ActiveBy", "className": "cls_nowrap" },
                { data: "ModifyOn", "className": "cls_nowrap" }
            ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });

    //$.ajax({
    //    url: '/api/lov/ToGenerate?data.Id=' + MainArtworkSubId,
    //    //url: url,
    //    dataSrc: 'data',
    //    success: function (data) {
    //        var columns = [];
    //        //$("#TableId_igrid_pa_faozone").DataTable().ajax.reload();
    //        //build the DataTable dynamically.
    //        // json return:  {"Table":[{"ImportID":121,"DeptName":"Ag Commissioner","FTE":48.15,"EmployeeCount":50}...
    //        columnNames = Object.keys(data.Table[0]); //.Table[0]] refers to the propery name of the returned json
    //        for (var i in columnNames) {
    //            columns.push({
    //                data: columnNames[i],
    //                title: columnNames[i]
    //            });
    //        }            
    //        $("#table_taskform_history").DataTable({
           
    //            dom: 'frtip',
    //            data: data.Table,
    //            rowId: 'ImportID',
    //            scrollX: true,
    //            columns: columns,
  
    //        })
    //    }

    //});
 
    //$("#TableId_igrid_pa_faozone").DataTable({
    //    ajax: {
    //        url: "../ServiceCS.asmx/Getjson2",
    //        data: myjson,
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",
    //        processData: false,
    //        dataType: "json",
    //        success: function (data) {
    //            alert(data.d);
    //        },
    //    },
    //    columns: [
    //        { data: "Id" },
    //        { data: "Description" },
    //    ],  
    //    error: function (xhr, status, error) {
    //        alert('Error Occured -'+ error)
    //    }
    //});
}

function bindSuggestSymbolPopUp(serverSide, autosearch) {
    //alert(serverSide);  
    $('#TableId_igrid_pa_symbol').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,

        //"bAutoWidth": true,
        //"scrollX": true,
        columns:
            [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                {
                    render: function (data, type, row, meta) {
                        return '<input class="cls_chk_plant" type="checkbox">';
                    }
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap" }
            ],
        select: {
            'style': 'multi',
            selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}

function bindSuggestCatchingMethodPopUp(serverSide, autosearch) {
    //alert(serverSide);  

    $('#TableId_igrid_pa_catching_method').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
        //"scrollX": true,
        columns:
            [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                {
                    render: function (data, type, row, meta) {
                        return '<input class="cls_chk_plant" type="checkbox">';
                    }
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap" }
            ],
        select: {
            'style': 'multi',
            selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}
function bindSuggestVendorPopUp(serverSide, autosearch) {
    //alert(serverSide);  
    $('#TableId_igrid_pg_vendor').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
        //"scrollX": true,
        columns:
            [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                {
                    render: function (data, type, row, meta) {
                        return '<input class="cls_chk_plant" type="checkbox">';
                    }
                },
                { data: "Code", "className": "cls_nowrap" },
                { data: "Name", "className": "cls_nowrap" }
            ],
        select: {
            'style': 'multi',
            selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}
function bindSuggestFAOZonePopUp(serverSide, autosearch, url, table) {
    //alert(serverSide);  
    $(table).DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
        //"scrollX": true,
        columns:
            [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                {
                    render: function (data, type, row, meta) {
                        return '<input class="cls_chk_plant" type="checkbox">';
                    }
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap" }
            ],
        select: {
            'style': 'multi',
            selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}
function bindSuggestPlantPopUp(serverSide, autosearch, url, table) {
    //alert(serverSide);  
    table_igrid = $(table).DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
        //"scrollX": true,
        columns: 
            [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                {
                    render: function (data, type, row, meta) {
                        return '<input class="cls_chk_plant" type="checkbox">';
                    }
                },
                { data: "Id", "className": "cls_nowrap"},
                { data: "Description", "className": "cls_nowrap" }
        ],
        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        select: {
            'style': 'multi',
            selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}
function bindSuggestCatchingAreaPopUp(serverSide, autosearch, url, table) {
    //alert(serverSide);  
    table_igrid = $(table).DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
        //"scrollX": true,
        columns:
            [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                {
                    render: function (data, type, row, meta) {
                        return '<input class="cls_chk_plant" type="checkbox">';
                    }
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap" }
            ],
        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        select: {
            'style': 'multi',
            selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}
function bindSuggestPackingStylePopUp(serverSide, autosearch) {
    //alert(serverSide);  


    var TableId_igrid_pa_packing_style = $('#TableId_igrid_pa_packing_style').DataTable();
    TableId_igrid_pa_packing_style.destroy();
    TableId_igrid_pa_packing_style = $('#TableId_igrid_pa_packing_style').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
                url: suburl + "/api/lov/PackStyleIGrid?data.TypeofPrimary=" + $('.cls_tfigrid .cls_lov_pa_typeofprimary').text(),
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
        //"scrollX": true,
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
                { data: "Id", "className": "cls_nowrap" },
                { data: "PrimaryCode", "className": "cls_nowrap" },
                { data: "GroupStyle", "className": "cls_nowrap" },
                { data: "PackingStyle", "className": "cls_nowrap" },
                { data: "RefStyle", "className": "cls_nowrap" },
                { data: "PackSize", "className": "cls_nowrap" },
            ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        //select: {
        //    'style': 'multi',
        //    selector: 'td:first-child input,td:last-child input'
        //},
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
}
function bindSuggestMaterialPopUp(serverSide, autosearch, url, table) {
    //alert(serverSide);  
    table_igrid = $(table).DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
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
        //"scrollX": true,
        columns: 
            [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
            { data: "Id", "className": "cls_nowrap" },
            { data: "Code", "className": "cls_nowrap" },
            { data: "Can", "className": "cls_nowrap" },
            { data: "Description", "className": "cls_nowrap" },
            { data: "ContainerType", "className": "cls_nowrap" },
            { data: "LidType", "className": "cls_nowrap" }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            //if (!isEmpty(data.START_DATE)) {
            //    $(row).find('.cls_startdate').html(myDateMoment(data.START_DATE));
            //}
            //if (!isEmpty(data.END_DATE)) {
            //    $(row).find('.cls_enddate').html(myDateMoment(data.END_DATE));
            //}
        },
        order: [[2, 'asc']],
        initComplete: function (settings, json) {

        },
    });
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
            
            var form_name = '.cls_container_taskform_igrid ';
            $('.cls_container_taskform_igrid').find('.cls_btn_pa_submit').attr('disabled', true);
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_rf_no').val(item.DocumentNo);
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_aw_no').val(item.DMSNo);
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_ref_form_no').val(item.ReferenceMaterial);
            $('.cls_tfigrid .cls_txt_pa_primary_size_id').val(item.PrimarySize_id);
            $('.cls_container_taskform_igrid .cls_txt_header_tfartwork_mat_no').val(item.Material);
            $(form_name + '.cls_txt_header_tfartwork_doc').val(item.Description);
            setValueToDDL(form_name + '.cls_lov_brand', item.Brand, item.Brand_TXT);
            setValueToDDL(form_name + '.cls_lov_pa_typeofprimary', item.TypeofPrimary, item.TypeofPrimary);


            if (item.ChangePoint == "N")
                $("input[name=cls_rdo_haeder_tfartwork_change_point][value=0]").prop('checked', true);
            else 
                $("input[name=cls_rdo_haeder_tfartwork_change_point][value=1]").prop('checked', true);

            //
            setValueToDDL(form_name + '.cls_lov_pa_type_of_two', item.TypeofCarton2, item.TypeofCarton2);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_type_of', item.Typeof, item.Typeof);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_catching_period', item.CatchingPeriodDate, item.CatchingPeriodDate);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_scientific_name', item.Scientific_Name, item.Scientific_Name);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_process_colour', item.Processcolour, item.Processcolour);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_pms_colour', item.PMScolour, item.PMScolour);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_total_colour', item.Totalcolour, item.Totalcolour);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_process_colour', item.Processcolour, item.Processcolour);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_style_of_printing', item.StyleofPrinting, item.StyleofPrinting);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_plant_register_no', item.PlantRegisteredNo, item.PlantRegisteredNo);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_company_name', item.CompanyNameAddress, item.CompanyNameAddress);
            setValueToDDL(form_name + '.cls_lov_pa_material_group', item.MaterialGroup, item.MaterialGroup_TXT);
            setValueToDDL(form_name + '.cls_lov_pa_direction_of_sticker', item.Direction, item.Direction);
            setValueToDDL('.cls_tfigrid .cls_lov_pa_specie', item.Specie, item.Specie);


            $('.cls_tfigrid .cls_txt_pa_primary_size').val(item.PrimarySize);
            $('.cls_tfigrid .cls_txt_pa_container_type').val(item.ContainerType);
            $('.cls_tfigrid .cls_txt_pa_lid_type').val(item.LidType);
            $('.cls_tfigrid .cls_txt_pa_product_code').val(item.ProductCode);
            $('.cls_tfigrid .cls_txt_pa_pack_size').val(item.Packing);
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
            setValueToDDL('.cls_task_form_pg .cls_lov_grade_of', item.Grandof, item.Grandof);
            setValueToDDL('.cls_task_form_pg .cls_lov_rsc_di_cut', item.RSC, item.RSC);
            setValueToDDL('.cls_task_form_pg .cls_lov_flute', item.Flute, item.Flute);
            setValueToDDL('.cls_task_form_pg .cls_lov_roll_sheet', item.RollSheet, item.RollSheet);
            $('.cls_task_form_pg .cls_txt_pg_vendor_multiple_static').val(item.Vendor);
            $('.cls_task_form_pg .cls_txt_acessories').val(item.Accessories);
            $('.cls_task_form_pg .cls_txt_printing_system').val(item.PrintingSystem);


            setRequireFieldTaskformArtworkPA(item.MaterialGroup);
            //$('.cls_tfigrid .cls_input_pa_brand').val(item.Brand);
        }
    }
}

