var cnt_lov_artwork_country_multiple = 1;
var cnt_lov_artwork_filename_multiple = 1;
var cnt_lov_artwork_plant_multiple = 1;
var cnt_lov_artwork_mailto_multiple = 1;
var cnt_lov_artwork_mailcc_multiple = 1;
var inTaskForm = false;
var defaultArtwork_PrimarySizeData = [];
var defaultArtwork_PackingStyleData = [];
var RDREF = "";
var typeartwork = "";
var typecheck_vap = "";
var msgconfirm_multiplant_sorepeat = "";  // 20230121_3V_SOREPAT INC-93118

$(document).ready(function () {
    bind_lov('.cls_lov_artwork_type_of_product', '/api/lov/typeofproduct', 'data.DISPLAY_TXT');
    typecheck_vap = "load";
    bind_lov('.cls_lov_artwork_company', '/api/lov/company', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_sold_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_ship_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_other_to', '/api/lov/customerother', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_brand_other', '/api/lov/brand', 'data.DISPLAY_TXT', '.cls_input_artwork_brand_other');
    bind_lov('.cls_lov_artwork_container_type_other', '/api/lov/containerType', 'data.DISPLAY_TXT', '.cls_input_artwork_container_type_other');
    bind_lov('.cls_lov_artwork_lid_type_other', '/api/lov/lidtype', 'data.DISPLAY_TXT', '.cls_input_artwork_lid_type_other');
    //--------------------------// by aof  packing style 01/20/2021- -------------------------------
    // bind_lov('.cls_lov_artwork_packing_style_other', '/api/lov/2p', 'data.DISPLAY_TXT', '.cls_input_artwork_packing_style_other');
    bind_lov_param('.cls_lov_artwork_packing_style_other', '/api/lov/2p_new', 'data.DISPLAY_TXT', ["PRIMARY_TYPE_ID"], ['.cls_lov_artwork_primary_type_other'], '.cls_input_artwork_packing_style_other');
    //--------------------------// by aof  packing style 01/20/2021--------------------------------
    bind_lov('.cls_lov_artwork_pack_size_other', '/api/lov/packsize', 'data.DISPLAY_TXT', '.cls_input_artwork_pack_size_other');
    bind_lov('.cls_lov_artwork_primary_size_other', '/api/lov/3p', 'data.DISPLAY_TXT', '.cls_input_artwork_primary_size_other');
    bind_lov('.cls_lov_artwork_primary_type_other', '/api/lov/primarytype', 'data.DISPLAY_TXT', '.cls_input_artwork_primary_type_other');
    bind_lov('.cls_lov_artwork_search', '/api/lov/listofartwork', 'data.DISPLAY_TXT');

    //for modal search check list
    bind_lov('.cls_lov_artwork_search_soldto', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_search_shipto', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_search_brand', '/api/lov/brand', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_search_country', '/api/lov/country', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_search_req_creator', '/api/lov/checklistcreator', 'data.DISPLAY_TXT');
    
    $('input[type=radio][name=rdo_artwork_type_of_artwork_ffc]').change(function () {
        setRequiredFieldArtwork();
    });

    $(document).on("click", ".cls_artwork_request_form .cls_img_lov_artwork_add_country_multiple", function () {
        var obj = $('.tr_artwork_country_multiple').clone();
        obj.show();
        obj.removeClass('tr_artwork_country_multiple');
        obj.find('.cls_lov_artwork_country_multiple').toggleClass('cls_lov_artwork_country_multiple cls_lov_artwork_country_multiple' + cnt_lov_artwork_country_multiple);
        obj.insertAfter($('.tr_artwork_country_multiple_static:last'));

        if (UserPosition != "FFC") {
            $('.cls_lov_artwork_country_multiple' + cnt_lov_artwork_country_multiple).attr("required", true);
        }

        bind_lov('.cls_lov_artwork_country_multiple' + cnt_lov_artwork_country_multiple, '/api/lov/country', 'data.DISPLAY_TXT');

        if (cnt_lov_artwork_country_multiple > 1) {
            obj.find('.td_artwork_country_txt_multiple').text('');
            obj.find('.cls_img_lov_artwork_add_country_multiple').remove();
            obj.find('.cls_img_lov_artwork_delete_country_multiple').show();
        }
        $(obj).find(".cls_img_lov_artwork_delete_country_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_artwork_country_multiple++;
    });
    $(".cls_img_lov_artwork_add_country_multiple").click();

    $(document).on("click", ".cls_artwork_request_form .cls_img_lov_artwork_add_plant_multiple", function () {
        var obj = $('.tr_artwork_plant_multiple').clone();
        obj.show();
        obj.removeClass('tr_artwork_plant_multiple');
        obj.find('.cls_lov_artwork_plant_multiple').toggleClass('cls_lov_artwork_plant_multiple cls_lov_artwork_plant_multiple' + cnt_lov_artwork_plant_multiple);
        obj.insertAfter($('.tr_artwork_plant_multiple_static:last'));

        if (UserPosition != "FFC") {
            $('.cls_lov_artwork_plant_multiple' + cnt_lov_artwork_plant_multiple).attr("required", true);
        }

        bind_lov('.cls_lov_artwork_plant_multiple' + cnt_lov_artwork_plant_multiple, '/api/lov/plant', 'data.DISPLAY_TXT');

        if (cnt_lov_artwork_plant_multiple > 1) {
            obj.find('.td_artwork_plant_txt_multiple').text('');
            obj.find('.cls_img_lov_artwork_add_plant_multiple').remove();
            obj.find('.cls_img_lov_artwork_delete_plant_multiple').show();
            $('.cls_artwork_request_form .cls_display_txt_check_manufacturer').show();
        }
        $(obj).find(".cls_img_lov_artwork_delete_plant_multiple").click(function () {
            cnt_lov_artwork_plant_multiple--;
            if (cnt_lov_artwork_plant_multiple < 3) {
                $('.cls_artwork_request_form .cls_display_txt_check_manufacturer').hide();
            }
            $(this).closest('.row').remove();
            getDataArtworkffc();
        });
        $('.cls_lov_artwork_plant_multiple' + cnt_lov_artwork_plant_multiple).change(function () {
            //check plant duplicate
            var arrayplantList = [];
            arrayplantList = getplantlist();
            //var in_array = plantList.filter(function (item) {
            //    return item.PRODUCTION_PLANT_ID == this.value // look for the item where ID is equal to value
            //});
            var in_array = arrayplantList.filter(x => x.PRODUCTION_PLANT_ID === this.value).length; 
            //var text = in_array[0].length();
            if (in_array > 1) {
                setValueToDDL('.cls_lov_artwork_plant_multiple' + cnt_lov_artwork_plant_multiple, "");
                var obj = $('.cls_lov_artwork_plant_multiple_static:last');
                $(obj).empty();
                alertError2("Production plant is duplicate.");
                return false;
            } else 
            setValueToDDL('.cls_lov_artwork_plant_multiple' + cnt_lov_artwork_plant_multiple, this.value);
            //setDataArtworkffc(this.value);
                getDataArtworkffc();
        });
        cnt_lov_artwork_plant_multiple++;
    });
    $(".cls_img_lov_artwork_add_plant_multiple").click();

    $(document).on("click", ".cls_artwork_request_form .cls_img_lov_artwork_add_mailto_multiple", function () {
        var obj = $('.tr_artwork_mailto_multiple').clone();
        obj.show();
        obj.removeClass('tr_artwork_mailto_multiple');
        obj.find('.cls_lov_artwork_mailto_multiple').toggleClass('cls_lov_artwork_mailto_multiple cls_lov_artwork_mailto_multiple' + cnt_lov_artwork_mailto_multiple);
        obj.insertAfter($('.tr_artwork_mailto_multiple_static:last'));
        $('.cls_lov_artwork_mailto_multiple' + cnt_lov_artwork_mailto_multiple).attr("required", true);

        if (UserPosition == "FFC")
            bind_lov('.cls_lov_artwork_mailto_multiple' + cnt_lov_artwork_mailto_multiple, '/api/checklist/mailtoccffc', 'data.DISPLAY_TXT');
        else if (UserPosition == "T-HOLDING")
            bind_lov('.cls_lov_artwork_mailto_multiple' + cnt_lov_artwork_mailto_multiple, '/api/checklist/mailtocctholding', 'data.DISPLAY_TXT');
        else
            bind_lov_param_filter('.cls_lov_artwork_mailto_multiple' + cnt_lov_artwork_mailto_multiple, '/api/checklist/mailtocustomer', 'data.USER_DISPLAY_TXT', ["SOLD_TO_ID", "SHIP_TO_ID", "CUSTOMER_OTHER_ID"], [".cls_artwork_request_form .cls_lov_artwork_sold_to", ".cls_artwork_request_form .cls_lov_artwork_ship_to", ".cls_artwork_request_form .cls_lov_artwork_other_to"], "FILTER_ID", [".cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static", ".cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static"]);

        if (cnt_lov_artwork_mailto_multiple > 1) {
            obj.find('.td_artwork_mailto_txt_multiple').text('');
            obj.find('.cls_img_lov_artwork_add_mailto_multiple').remove();
            obj.find('.cls_img_lov_artwork_delete_mailto_multiple').show();
        }
        $(obj).find(".cls_img_lov_artwork_delete_mailto_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_artwork_mailto_multiple++;
    });

    $(document).on("click", ".cls_artwork_request_form .cls_img_lov_artwork_add_mailcc_multiple", function () {
        var obj = $('.tr_artwork_mailcc_multiple').clone();
        obj.show();
        obj.removeClass('tr_artwork_mailcc_multiple');
        obj.find('.cls_lov_artwork_mailcc_multiple').toggleClass('cls_lov_artwork_mailcc_multiple cls_lov_artwork_mailcc_multiple' + cnt_lov_artwork_mailcc_multiple);
        obj.insertAfter($('.tr_artwork_mailcc_multiple_static:last'));
        //$('.cls_lov_artwork_mailcc_multiple' + cnt_lov_artwork_mailcc_multiple).attr("required", true);

        if (UserPosition == "FFC")
            bind_lov('.cls_lov_artwork_mailcc_multiple' + cnt_lov_artwork_mailcc_multiple, '/api/checklist/mailtoccffc', 'data.DISPLAY_TXT');
        else if (UserPosition == "T-HOLDING")
            bind_lov('.cls_lov_artwork_mailcc_multiple' + cnt_lov_artwork_mailcc_multiple, '/api/checklist/mailtocctholding', 'data.DISPLAY_TXT');
        else
            bind_lov_param_filter('.cls_lov_artwork_mailcc_multiple' + cnt_lov_artwork_mailcc_multiple, '/api/checklist/mailtocustomer', 'data.USER_DISPLAY_TXT', ["SOLD_TO_ID", "SHIP_TO_ID", "CUSTOMER_OTHER_ID"], [".cls_artwork_request_form .cls_lov_artwork_sold_to", ".cls_artwork_request_form .cls_lov_artwork_ship_to", ".cls_artwork_request_form .cls_lov_artwork_other_to"], "FILTER_ID", [".cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static", ".cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static"]);

        if (cnt_lov_artwork_mailcc_multiple > 1) {
            obj.find('.td_artwork_mailcc_txt_multiple').text('');
            obj.find('.cls_img_lov_artwork_add_mailcc_multiple').remove();
            obj.find('.cls_img_lov_artwork_delete_mailcc_multiple').show();
        }
        $(obj).find(".cls_img_lov_artwork_delete_mailcc_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_artwork_mailcc_multiple++;
    });

    $(".cls_img_lov_artwork_add_mailto_multiple").click();
    $('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static').prop("disabled", true);
    $(".cls_img_lov_artwork_add_mailcc_multiple").click();
    $('.cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static').prop("disabled", true);

    bind_lov_param('.cls_lov_artwork_reviewer', '/api/lov/reviewer', 'data.DISPLAY_TXT', ["type_of_product_id"], ['.cls_lov_type_of_product']);
    if (UserPosition == "FFC") {
        bind_lov_param('.cls_lov_artwork_reviewer', '/api/lov/reviewer_ffc', 'data.DISPLAY_TXT', ["type_of_product_id"], ['.cls_lov_type_of_product']);
        if (!inTaskForm) {
            if (!isEmpty(FFCDefaultReviewer_UserID))
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_reviewer', FFCDefaultReviewer_UserID, FFCDefaultReviewer_UserName);
            if (!isEmpty(FFCDefaultEmailTo_UserID)) {
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static', FFCDefaultEmailTo_UserID, FFCDefaultEmailTo_UserName);
                $(".cls_img_lov_artwork_add_mailto_multiple").hide();
                $(".cls_img_lov_artwork_delete_mailto_multiple").hide();
            }
            if (!isEmpty(FFCDefaultEmailCC_UserID)) {
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static', FFCDefaultEmailCC_UserID, FFCDefaultEmailCC_UserName);
                $(".cls_img_lov_artwork_add_mailcc_multiple").hide();
                $(".cls_img_lov_artwork_delete_mailcc_multiple").hide();
            }
        }
        $('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static:visible').prop("disabled", true);
    }
    else if (UserPosition == "T-HOLDING") {
        bind_lov_param('.cls_lov_artwork_reviewer', '/api/lov/reviewer_tholding', 'data.DISPLAY_TXT', ["type_of_product_id"], ['.cls_lov_type_of_product']);
    }
    else {
        bind_lov_param('.cls_lov_artwork_reviewer', '/api/lov/reviewer', 'data.DISPLAY_TXT', ["type_of_product_id"], ['.cls_lov_artwork_type_of_product']);
    }


    setArtworkProductCodeRefNo('product_code');
    $('input[type=radio][name=req_artwork_product_code_ref_no]').change(function () {
        setArtworkProductCodeRefNo(this.value);
    });

    $('.cls_artwork_request_form .cls_lov_artwork_sold_to').change(function () {
        if (UserPosition == "FFC") {
            resetMailToCcArtwork(true);
        } else {
            resetMailToCcArtwork(false);
        }
    });
    $('.cls_artwork_request_form .cls_lov_artwork_ship_to').change(function () {
        if (UserPosition == "FFC") {
            resetMailToCcArtwork(true);
        } else {
            resetMailToCcArtwork(false);
        }
    });
    $('.cls_artwork_request_form .cls_lov_artwork_other_to').change(function () {
        if (UserPosition == "FFC") {
            resetMailToCcArtwork(true);
        } else {
            resetMailToCcArtwork(false);
        }
    });

    $(".cls_artwork_request_form .cls_btn_artwork_search").click(function (e) {
        $('#modal_artwork_search').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $(".cls_artwork_request_form .cls_btn_artwork_check_salesorder").click(function (e) {
        getSalesOrderArtwork();
    });

    $("#modal_artwork_search .cls_btn_artwork_search").click(function (e) {
        var soldto = $("#modal_artwork_search .cls_lov_artwork_search_soldto").val();
        var shipto = $("#modal_artwork_search .cls_lov_artwork_search_shipto").val();
        var brand = $("#modal_artwork_search .cls_lov_artwork_search_brand").val();
        var country = $("#modal_artwork_search .cls_lov_artwork_search_country").val();
        var creator = $("#modal_artwork_search .cls_lov_artwork_search_req_creator").val();
        if (soldto == null && shipto == null && brand == null && country == null && creator == null) {
            alertError2("Please select at least 1 criteria.");
        } else {
            bindArtworkSearchRequesForm(soldto, shipto, brand, country, creator);
        }
    });

    $("#modal_artwork_search .cls_btn_artwork_clear").click(function (e) {
        $("#modal_artwork_search .cls_lov_artwork_search_soldto").val('').trigger("change");
        $("#modal_artwork_search .cls_lov_artwork_search_shipto").val('').trigger("change");
        $("#modal_artwork_search .cls_lov_artwork_search_brand").val('').trigger("change");
        $("#modal_artwork_search .cls_lov_artwork_search_country").val('').trigger("change");
        $("#modal_artwork_search .cls_lov_artwork_search_req_creator").val('').trigger("change");
    });

    $('#table_artwork_search').DataTable();
    $("#modal_artwork_search .cls_btn_artwork_select").click(function (e) {
        var table = $('#table_artwork_search').DataTable();
        var tblData = table.rows('.selected').data();

        if (tblData.length > 0) {
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_search', tblData[0].ID, tblData[0].REQUEST_FORM_NO);
            $('.cls_artwork_request_form .cls_hid_artwork_copy_ref_type').val(tblData[0].ARTWORK_ID > 0 ? 'ARTWORK' : 'CHECKLIST');
            $('.cls_artwork_request_form .cls_hid_artwork_copy_ref_id').val(tblData[0].ARTWORK_ID > 0 ? tblData[0].ARTWORK_ID : tblData[0].CHECK_LIST_ID);
            $("#modal_artwork_search .cls_btn_artwork_close").click();
        }
    });
});
var click_ref = "";
var click_ref_temp_artwork_request_id = "";
var click_ref_temp_artwork_request_txt = "";
$(document).ready(function () {
    $(".cls_artwork_request_form .cls_btn_artwork_add_product_code").click(function () {
        var productCodes = $(".cls_input_artwork_product_code").val().split(/\n/);
        var strAlert = "";
        var jsonObj = new Object();
        jsonObj.data = [];
        var plantList = [];

        plantList = getplantlist();
        var in_array = plantList.filter(function (item) {
            return item.PRODUCTION_PLANT_ID == 3 // look for the item where ID is equal to value
        });
        var text = plantList.indexOf(in_array[0]) != -1 ? "3" : "";
        var list_err = "";
        var check_ffc = true;
 
        for (var i = 0; i < productCodes.length; i++) {
            productCodes[i] = productCodes[i].trim();
            if (productCodes[i] != "") {
                var isDup = false;
                $('.table_artwork_product_code_item > tbody > tr').each(function () {
                    if (productCodes[i].toLowerCase() == $(this).find('.cls_artwork_product_code').val().toLowerCase()) {
                        isDup = true;
                        strAlert = strAlert + productCodes[i] + " was already" + "<br>";
                        return false;
                    }
                });
                if (!isDup) {

                    if (productCodes[i].substring(2, 1) != "E" && UserPosition == "FFC") {
                        list_err += productCodes[i] + "<br/>";
                        check_ffc = false;
                    }else {
                        var item = {};
                        item["PRODUCT_CODE"] = productCodes[i];
                        item["PRODUCTION_PLANT"] = text;
                        jsonObj.data.push(item);
                    }
                }
            }
        }

        if (plantList.length == 0) {
            strAlert = "Production plant should not be blank";
            $(".cls_input_artwork_product_code").val('');
        } 
        if (check_ffc == false && UserPosition == "FFC") {
            strAlert = list_err + "Product code was not found or mismatch.Please contact your system administrator.";
            $(".cls_input_artwork_product_code").val('');
        }
        if (jsonObj.data.length > 0 && plantList.length > 0) {
            getArtworkProductCode(jsonObj);
            $(".cls_input_artwork_product_code").val('');
        }

        if (strAlert.length > 0) {
            alertError2(strAlert);
        }
    });

    $(".cls_artwork_request_form .cls_btn_artwork_add_reference_no").click(function () {

        $('.cls_artwork_lbl_th_net_weight:visible').html("Net weight");

        var ref_no = $(".cls_input_artwork_reference_no").val().split(/\n/);
        var strAlert = "";
        var jsonObj = new Object();
        jsonObj.data = [];
        var plantList = [];
        plantList = getplantlist();
        for (var i = 0; i < ref_no.length; i++) {
            ref_no[i] = ref_no[i].trim();
            if (ref_no[i] != "") {
                var isDup = false;
                $('.table_artwork_ref_no_item > tbody > tr').each(function () {
                    if (ref_no[i].toLowerCase() == $(this).find('.cls_artwork_ref_no').val().toLowerCase()) {
                        isDup = true;
                        strAlert = strAlert + ref_no[i] + " was already" + "<br>";
                        return false;
                    }
                });

                if (!isDup) {
                    var item = {};
                    item["REFERENCE_NO"] = ref_no[i];
                    item["NET_WEIGHT"] = "";
                    item["DRAINED_WEIGHT"] = "";

                    jsonObj.data.push(item);
                }
            }
        }
        if (plantList.length == 0) {
            strAlert = "Production plant should not be blank";
        }
        if (jsonObj.data.length > 0 && plantList.length > 0) {

            var lbl_spanStar = "<span style=\"color: red;\">*</span>";
            $('.cls_artwork_lbl_th_net_weight:visible').html("Net weight" + lbl_spanStar);

            bindArtworkRefNo(jsonObj.data);
            $(".cls_input_artwork_reference_no").val('');
        }

        if (strAlert.length > 0) {
            alertError2(strAlert);
        }
    });

    $(".cls_input_artwork_salesorder").on('input focus keydown keyup', function () {
        var maxLength = 10;
        var text = $(this).val();
        var lines = text.split(/(\r\n|\n|\r)/gm);
        for (var i = 0; i < lines.length; i++) {
            if (lines[i].length > maxLength) {
                lines[i] = lines[i].substring(0, maxLength);
            }
        }
        $(this).val(lines.join(''));
    });

    $(".cls_input_artwork_product_code").on('input focus keydown keyup', function () {
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

    //$(".cls_input_artwork_reference_no").on('input focus keydown keyup', function () {
    //    var maxLength = 18;
    //    var text = $(this).val();
    //    var lines = text.split(/(\r\n|\n|\r)/gm);
    //    for (var i = 0; i < lines.length; i++) {
    //        if (lines[i].length > maxLength) {
    //            lines[i] = lines[i].substring(0, maxLength);
    //        }
    //    }
    //    $(this).val(lines.join(''));
    //});

    $('.cls_artwork_request_form .cls_btn_artwork_copy').click(function () {
        if (isEmpty($('.cls_lov_artwork_search').val())) {
            alertError2("Please select Copy/Reference request form.");
        }
        else {
            click_ref = "No";
            click_ref_temp_artwork_request_id = '';
            click_ref_temp_artwork_request_txt = '';
            $('.cls_btn_artwork_ref').removeClass('btn-primary').addClass('btn-default');
            bindDataArtworkRequestFormByRequestFormNo('copy');
        }
    })

    $('.cls_artwork_request_form .cls_btn_artwork_ref').click(function () {
        if (isEmpty($('.cls_lov_artwork_search').val())) {
            alertError2("Please select Copy/Reference request form.");
        }
        else {
            click_ref = "Yes";
            click_ref_temp_artwork_request_id = $('.cls_lov_artwork_search').val();
            click_ref_temp_artwork_request_txt = $('.cls_lov_artwork_search option:selected').text();
            $('.cls_btn_artwork_ref').removeClass('btn-default').addClass('btn-primary');
            bindDataArtworkRequestFormByRequestFormNo('ref');
        }
    })

    $('.cls_artwork_request_form .cls_lov_artwork_primary_size_other').on("change", function () {
        var selections = $(this).select2('data');
        var v = $(this)[0].value;
        var isClear = true;
        if (selections.length > 0) {
            if (selections[0].ID != -1 && selections[0].ID != undefined) {
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_container_type_other', selections[0].CONTAINER_TYPE_VALUE, selections[0].CONTAINER_TYPE_DESCRIPTION);
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_lid_type_other', selections[0].LID_TYPE_VALUE, selections[0].LID_TYPE_DESCRIPTION);
                $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').prop("disabled", true);
                $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').prop("disabled", true);
                isClear = false;
            }
            else if (defaultArtwork_PrimarySizeData[v] != undefined) {
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_container_type_other', defaultArtwork_PrimarySizeData[v].CONTAINER_TYPE_ID, defaultArtwork_PrimarySizeData[v].CONTAINER_TYPE_DISPLAY_TXT);
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_lid_type_other', defaultArtwork_PrimarySizeData[v].LID_TYPE_ID, defaultArtwork_PrimarySizeData[v].LID_TYPE_DISPLAY_TXT);
                $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').prop("disabled", true);
                $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').prop("disabled", true);
                isClear = false;
            }
        }

        if (isClear) {
            $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').empty();
            $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').empty();
            $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').prop("disabled", false);
            $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').prop("disabled", false);
        }
    });

    $('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').on("change", function () {
        var selections = $(this).select2('data');
        var v = $(this)[0].value;
        var isClear = true;
        if (selections.length > 0) {
            if (selections[0].ID != -1 && selections[0].ID != undefined) {
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_pack_size_other', selections[0].PACK_SIZE_VALUE, selections[0].PACK_SIZE_DESCRIPTION);
                $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').prop("disabled", true);
                isClear = false;
            }
            else if (defaultArtwork_PackingStyleData[v] != undefined) {
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_pack_size_other', defaultArtwork_PackingStyleData[v].PACK_SIZE_ID, defaultArtwork_PackingStyleData[v].PACK_SIZE_DISPLAY_TXT);
                $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').prop("disabled", true);
                isClear = false;
            }
        }

        if (isClear) {
            $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').empty();
            $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').prop("disabled", false);
        }
    });




    $('.cls_artwork_request_form .cls_btn_artwork_save').click(function () {
        //saveDataArtworkRequest(true);  // ticket 453346  by aof start
        //checkReviewerInGroupTypeOfProduct(true); // ticket 453346  by aof start
        checkArtworkRequestIsComplete(true);  // INC-93118 by aof
    })

    $('.cls_artwork_request_form .cls_btn_artwork_delete').click(function () {
        deleteDataArtworkRequest(ARTWORK_REQUEST_ID);
    });

    $(".cls_artwork_request_form form").submit(function (e) {
        if ($(this).valid()) {
            //saveDataArtworkRequest(false);  // ticket 453346  by aof start
           // checkReviewerInGroupTypeOfProduct(false);  // ticket 453346  by aof start
            checkArtworkRequestIsComplete(false); // INC-93118 by aof
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    if (ARTWORK_REQUEST_ID == 0) {
        tohomepage();
    }
    else {
        bindDataArtworkRequestForm(ARTWORK_REQUEST_ID);
    }

    $('input:radio[name="rdo_artwork_type_of_artwork_ffc"]').filter('[value="NEW"]').attr('checked', true).change();
});
function getDataArtworkffc() {
    var jsonObj = new Object();
    jsonObj.data = [];
    var plantList = [];
    plantList = getplantlist();
    var in_array = plantList.filter(function (item) {
        return item.PRODUCTION_PLANT_ID == 3
    });
    var text = plantList.indexOf(in_array[0]) != -1 ? "3" : "";
    if ($('input[name=req_artwork_product_code_ref_no]:checked').val() == 'ref_no') {
        $('.table_artwork_ref_no_item > tbody > tr').each(function () {
                var ref_no = $(this).find('.cls_artwork_ref_no').val();
                if (ref_no != "") {
                    var item = {};
                    item["REFERENCE_NO"] = ref_no;
                    item["NET_WEIGHT"] = $(this).find('.cls_artwork_net_weight' + 'X').val();
                    item["DRAINED_WEIGHT"] = $(this).find('.cls_artwork_drain_weight').val();
                    jsonObj.data.push(item);
                }
        });
        $('.table_artwork_ref_no_item tbody tr:visible').remove();
        if (jsonObj.data.length > 0 && plantList.length > 0) 
        bindArtworkRefNo(jsonObj.data);
    }
    if ($('input[name=req_artwork_product_code_ref_no]:checked').val() == 'product_code') {
        $('.table_artwork_product_code_item > tbody > tr').each(function () {
            var productCodes = $(this).find('.cls_artwork_product_code').val().toLowerCase();
            if (productCodes != "") {
                var item = {};
                item["PRODUCT_CODE"] = productCodes;
                item["PRODUCTION_PLANT"] = text;
                jsonObj.data.push(item);
            }
        });
        //if (jsonObj.data.length > 0 && plantList.length > 1) {
        //    alertError2("If your product is a VAP, Please select the VAP product flag. Default product flag function is not supported multiple production plants.");
        //}
        $('.table_artwork_product_code_item tbody tr:visible').remove();
        if (jsonObj.data.length > 0 && plantList.length > 0) {
            getArtworkProductCode(jsonObj);
        }
    }
}
function setDataArtworkffc(value) {
    $('.table_artwork_product_code_item > tbody > tr').each(function () {
        var productCodes = $(this).find('.cls_artwork_product_code').val().toLowerCase();
        if (productCodes != "") {
            var obj = $(this);
 
            input = 'cls_artwork_vap';
            obj.find('.' + input).prop('checked', value == 6 ? true : false);
            obj.find('.' + input).prop("disabled", checked_status);
            //var artwork_request_type = $('.cls_artwork_request_form .cls_input_artwork_request_type').val();
            //input = 'cls_artwork_ffc';
            //obj.find('.' + input).prop('checked', value == 6 && artwork_request_type == "FFC" ? true : false);
            //obj.find('.' + input).prop("disabled", true);
        }
    });
}
function bindDataArtworkRequestForm(artwork_req_id) {
    var myurl = '/api/artwork/artworkrequest?data.artwork_request_id=' + artwork_req_id;
    var mytype = 'GET';
    var mydata = null;

    myAjax(myurl, mytype, mydata, callback_get_artworkrequest);
}

function bindDataArtworkRequestFormByRequestFormNo(typeOf) {
    $('.cls_artwork_request_form .cls_hid_artwork_copy_reference').val(typeOf);
    if ($('.cls_artwork_request_form .cls_hid_artwork_copy_ref_type').val() == 'CHECKLIST') {
        var myurl = '/api/checklist/request?data.check_list_no=' + $('.cls_artwork_request_form .cls_lov_artwork_search').val();
        var mytype = 'GET';
        var mydata = null;
        myAjax(myurl, mytype, mydata, callback_get_artworkrequest);
    }
    else {
        var myurl = '/api/artwork/artworkrequest?data.ARTWORK_REQUEST_NO=' + $('.cls_artwork_request_form .cls_lov_artwork_search').val();
        var mytype = 'GET';
        var mydata = null;
        myAjax(myurl, mytype, mydata, callback_get_artworkrequest);
    }
}

function deleteDataArtworkRequest(artwork_request_id) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["ARTWORK_REQUEST_ID"] = artwork_request_id;
    jsonObj.data = item;

    var myurl = '/api/artwork/deleteartworkrequest';
    var mytype = 'DELETE';
    var mydata = jsonObj;
    myAjaxConfirmDelete(myurl, mytype, mydata, tohomepage);
}

function deleteDataArtworkFile(artwork_request_id, node_id, obj) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["ARTWORK_REQUEST_ID"] = artwork_request_id;
    item["REQUEST_FORM_FILE_NODE_ID"] = node_id;
    jsonObj.data = item;
    artworkfile_delete_obj = obj;

    var myurl = '/api/artwork/deletefilerequest';
    var mytype = 'DELETE';
    var mydata = jsonObj;
    myAjaxConfirmDelete(myurl, mytype, mydata, callback_delete_artworkfile);
}

var artworkfile_delete_obj;
function callback_delete_artworkfile(res) {
    if (res.status == 'S') {
        artworkfile_delete_obj.remove();
    }
}

function showBtnArtwork(artwork_request_id) {
    if (ARTWORK_REQUEST_READONLY == "0") {
        $('.cls_artwork_request_form .cls_btn_artwork_save').show();
        $('.cls_artwork_request_form .cls_btn_artwork_submit').show();
        if (artwork_request_id > 0) {
            if (!inTaskForm)
                $('.cls_artwork_request_form .cls_btn_artwork_delete').show();
        }
        if (typeof ArtworkSubId !== 'undefined') {
            $('.cls_artwork_request_form .cls_img_lov_artwork_delete_filename_multiple').hide();
            $('.cls_artwork_request_form .cls_btn_artwork_delete').hide();
        }
    }
    else {
        //$('.cls_artwork_request_form .cls_row_artwork_copy_ref1').hide();
        //$('.cls_artwork_request_form .cls_row_artwork_copy_ref2').hide();
        $('.cls_artwork_request_form .cls_btn_artwork_check_salesorder').hide();
        $('.cls_artwork_request_form .cls_btn_artwork_add_product_code').hide();
        $('.cls_artwork_request_form .cls_btn_artwork_add_reference_no').hide();
        $('.cls_artwork_request_form .cls_img_artwork_delete_product_code_item').hide();
        $('.cls_artwork_request_form .cls_img_artwork_delete_ref_no_item').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_add_plant_multiple').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_delete_plant_multiple').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_add_country_multiple').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_delete_country_multiple').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_add_mailto_multiple').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_delete_mailto_multiple').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_add_mailcc_multiple').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_delete_mailcc_multiple').hide();
        $('.cls_artwork_request_form .cls_img_lov_artwork_delete_filename_multiple').hide();

        $('.cls_artwork_request_form').find('input:visible, textarea:visible, select:visible').attr('disabled', true);
    }
}

function callback_get_artworkrequest(res) {
    $('.cls_artwork_request_form').show();

   

    if (res.data.length > 0) {
        console.log(res.data);
        if (res.data[0].IN_TRANSIT_TO != null)
            $('.cls_artwork_request_form .cls_input_artwork_in_tran').val(res.data[0].IN_TRANSIT_TO);
        if (res.data[0].VIA != null)
            $('.cls_artwork_request_form .cls_input_artwork_via').val(res.data[0].VIA);

        $('.cls_task_form_pg_artwork .cls_hid_artwork_taskform_primary_type').val(res.data[0].PRIMARY_TYPE_ID);
        setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other', res.data[0].PRIMARY_TYPE_ID, res.data[0].PRIMARY_TYPE_DISPLAY_TXT);

        if (inTaskForm) {
            if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_BACK_MK") {
            }
            else {
                $('.cls_row_artwork_copy_ref1 .cls_btn_artwork_search').hide();
                $('.cls_row_artwork_copy_ref2').hide();
            }
        }

        if (ARTWORK_REQUEST_READONLY == "1") {
            $('.cls_artwork_request_form .cls_row_artwork_copy_ref1 .cls_btn_artwork_search').hide();
            $('.cls_artwork_request_form .cls_row_artwork_copy_ref2').hide();
        }

        if (click_ref == '') {
            $('.cls_artwork_request_form .cls_hid_artwork_copy_ref_id').val(res.data[0].REFERENCE_REQUEST_ID);
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_search', res.data[0].REFERENCE_REQUEST_NO, res.data[0].REFERENCE_REQUEST_NO);
            $('.cls_artwork_request_form .cls_hid_artwork_copy_ref_type').val(res.data[0].REFERENCE_REQUEST_TYPE);
        }

        $('.cls_artwork_request_form .cls_input_artwork_project_name').val(res.data[0].PROJECT_NAME);
        setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_type_of_product', res.data[0].TYPE_OF_PRODUCT_ID, res.data[0].TYPE_OF_PRODUCT_DISPLAY_TXT);
        $('.cls_artwork_request_form .cls_hid_artwork_node_id').val(res.data[0].REQUEST_FORM_FOLDER_NODE_ID);

        var saleorder = '';
        $.each(res.data[0].SALES_ORDER, function (index, item) {
            saleorder += item.SALES_ORDER_NO + "\r\n";
        });
        $('.cls_input_artwork_salesorder').val(saleorder);

        if ($('.cls_artwork_request_form .cls_hid_artwork_copy_reference').val() != undefined) {
            if ($('.cls_artwork_request_form .cls_hid_artwork_copy_reference').val().length == 0) {
                if (!isEmpty(res.data[0].TYPE_OF_ARTWORK)) {
                    typeartwork = res.data[0].TYPE_OF_ARTWORK;
                    if (res.data[0].TYPE_OF_ARTWORK == "NEW") {
                        $("input[name=rdo_artwork_type_of_artwork_ffc][value=NEW]").prop('checked', true);
                    }
                    else if (res.data[0].TYPE_OF_ARTWORK == "REPEAT") {
                        $("input[name=rdo_artwork_type_of_artwork_ffc][value=REPEAT]").prop('checked', true);
                        if (UserPosition != "FFC") {
                            var sorepeat = GetRequestParam('so');
                            if (sorepeat != null || (inTaskForm && CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_BACK_MK")) {
                                $('.cls_row_artwork_product_code_rd_ref').hide();
                                $('.cls_row_artwork_copy_ref1').hide();
                                $('.cls_row_artwork_copy_ref2').hide();
                                $('.cls_artwork_request_form .cls_hid_artwork_sorepeat').val('X');
                            }
                        }
                    }
                }

                if (res.data[0].ARTWORK_REQUEST_NO != null) {
                    $('.cls_artwork_request_form .cls_input_artwork_request_form_no').val(res.data[0].ARTWORK_REQUEST_NO);
                }
                if (res.data[0].ARTWORK_REQUEST_TYPE != null) {
                    $('.cls_artwork_request_form .cls_input_artwork_request_type').val(res.data[0].ARTWORK_REQUEST_TYPE);
                } else if (UserPosition == "FFC")
                    $('.cls_artwork_request_form .cls_input_artwork_request_type').val(UserPosition);
                if ($('.cls_artwork_request_form .cls_input_artwork_request_type').val() == "FFC") {
                    $('.cls_artwork_request_form .cls_img_lov_artwork_add_plant_multiple').hide();
                    $('.cls_artwork_request_form .cls_lov_artwork_plant_multiple_static').prop("disabled", true);
                }
                //if (ARTWORK_REQUEST_READONLY != "0") 
                if (res.data[0].CREATOR_NAME != null)
                    $('.cls_artwork_request_form .cls_input_artwork_creator_name').val(res.data[0].CREATOR_NAME);

                $('.cls_artwork_request_form .cls_input_artwork_uploaded_by').val(res.data[0].ARTWORK_UPLOADED_BY_DISPLAY_TXT);
                if (res.data[0].REQUEST_ITEMS != null && res.data[0].REQUEST_ITEMS.length > 0) {
                    $('.tr_artwork_filename_multiple_static:visible').remove();
                    cnt_lov_artwork_filename_multiple = 1;

                    $.each(res.data[0].REQUEST_ITEMS, function (index, item) {
                        var do_work = false;

                        if (inTaskForm) {
                            if (typeof ArtworkSubId !== 'undefined') {
                                if (item.ARTWORK_SUB_ID == MainArtworkSubId) {
                                    do_work = true;
                                }
                            }
                            else {
                                do_work = true;
                            }
                        }
                        else {
                            do_work = true;
                        }

                        if (do_work) {
                            var obj = $('.tr_artwork_filename_multiple').clone();
                            obj.show();
                            obj.removeClass('tr_artwork_filename_multiple');
                            obj.find('.cls_img_lov_artwork_delete_filename_multiple').toggleClass('cls_img_lov_artwork_delete_filename_multiple' + cnt_lov_artwork_filename_multiple);
                            obj.find('.cls_a_artwork_filename_multiple').toggleClass('cls_a_artwork_filename_multiple' + cnt_lov_artwork_filename_multiple);
                            obj.insertAfter($('.tr_artwork_filename_multiple_static:last'));
                            $('.cls_artwork_request_form .cls_a_artwork_filename_multiple' + cnt_lov_artwork_filename_multiple).attr("href", function (i, href) {
                                return href + '?nodeIdTxt=' + item.NODE_ID_TXT;
                            });
                            $('.cls_artwork_request_form .cls_a_artwork_filename_multiple' + cnt_lov_artwork_filename_multiple).html(item.FILE_NAME);
                            $(".cls_artwork_request_form .cls_img_lov_artwork_delete_filename_multiple" + cnt_lov_artwork_filename_multiple).click(function () {
                                deleteDataArtworkFile(res.data[0].ARTWORK_REQUEST_ID, item.REQUEST_FORM_FILE_NODE_ID, $(this).closest('.row'));
                            });
                            if (cnt_lov_artwork_filename_multiple > 1) {
                                obj.find('.td_artwork_filename_txt_multiple').text('');
                            }
                            cnt_lov_artwork_filename_multiple++;
                        }

                    });
                }
            }
        }

        //ticket 453346 by aof start
        if (res.data[0].REVIEWER_ID != null) {
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_reviewer', res.data[0].REVIEWER_ID, res.data[0].REVIEWER_DISPLAY_TXT);
        } else {
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_reviewer', 0, '');
        }
        //setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_reviewer', res.data[0].REVIEWER_ID, res.data[0].REVIEWER_DISPLAY_TXT);  //aof commented
        //ticket 453346 by aof last
        
        setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_company', res.data[0].COMPANY_ID, res.data[0].COMPANY_DISPLAY_TXT);
        setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_sold_to', res.data[0].SOLD_TO_ID, res.data[0].SOLD_TO_DISPLAY_TXT);
        setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_ship_to', res.data[0].SHIP_TO_ID, res.data[0].SHIP_TO_DISPLAY_TXT);
        setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_other_to', res.data[0].CUSTOMER_OTHER_ID, res.data[0].CUSTOMER_OTHER_DISPLAY_TXT);

        $('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static').prop("disabled", false);
        $('.cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static').prop("disabled", false);

        $('.tr_artwork_mailto_multiple_static:visible').remove();
        $('.tr_artwork_mailcc_multiple_static:visible').remove();

        cnt_lov_artwork_mailto_multiple = 1;
        cnt_lov_artwork_mailcc_multiple = 1;
        var obj = $('.cls_lov_artwork_mailto_multiple_static:first');
        obj.closest('.tr_artwork_mailto_multiple_static').find(".cls_img_lov_artwork_add_mailto_multiple").click();
        var obj = $('.cls_lov_artwork_mailcc_multiple_static:first');
        obj.closest('.tr_artwork_mailcc_multiple_static').find(".cls_img_lov_artwork_add_mailcc_multiple").click();
        var str_mail_to_customer = '';
        var str_mail_cc_customer = '';
        $.each(res.data[0].MAIL_TO_CUSTOMER, function (index, item) {
            if (item.MAIL_TO.toLowerCase() == "x") {
                if (cnt_lov_artwork_mailto_multiple > 2) {
                    var obj = $('.cls_lov_artwork_mailto_multiple_static:first');
                    obj.closest('.tr_artwork_mailto_multiple_static').find(".cls_img_lov_artwork_add_mailto_multiple").click();
                }
                else {
                    cnt_lov_artwork_mailto_multiple++;
                }
                var obj = $('.cls_lov_artwork_mailto_multiple_static:last');
                $(obj).empty();
                $(obj).append('<option value="' + item.CUSTOMER_USER_ID + '">' + item.USER_DISPLAY_TXT + '</option>');
                $(obj).val(item.CUSTOMER_USER_ID);

                if (str_mail_to_customer == '')
                    str_mail_to_customer = item.USER_DISPLAY_TXT;
                else
                    str_mail_to_customer = str_mail_to_customer + ', ' + item.USER_DISPLAY_TXT;

            } else if (item.MAIL_CC.toLowerCase() == "x") {
                if (cnt_lov_artwork_mailcc_multiple > 2) {
                    var obj = $('.cls_lov_artwork_mailcc_multiple_static:first');
                    obj.closest('.tr_artwork_mailcc_multiple_static').find(".cls_img_lov_artwork_add_mailcc_multiple").click();
                }
                else {
                    cnt_lov_artwork_mailcc_multiple++;
                }
                var obj = $('.cls_lov_artwork_mailcc_multiple_static:last');
                $(obj).empty();
                $(obj).append('<option value="' + item.CUSTOMER_USER_ID + '">' + item.USER_DISPLAY_TXT + '</option>');
                $(obj).val(item.CUSTOMER_USER_ID);

                if (str_mail_cc_customer == '')
                    str_mail_cc_customer = item.USER_DISPLAY_TXT;
                else
                    str_mail_cc_customer = str_mail_cc_customer + ', ' + item.USER_DISPLAY_TXT;
            }
        });
        if (UserPosition == "FFC") {
            resetMailToCcArtwork(true);
        }

        //Set value for customer popup
        $('.cls_to_customer_cus_pop').val(str_mail_to_customer);
        $('.cls_cc_customer_cus_pop').val(str_mail_cc_customer);

        bindArtworkCountry(res.data[0].COUNTRY);
        bindArtworkProductionPlant(res.data[0].PRODUCTION_PLANT);

        setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_primary_type_other', res.data[0].PRIMARY_TYPE_ID, res.data[0].PRIMARY_TYPE_DISPLAY_TXT);
        setValueToDDLOther('.cls_artwork_request_form .cls_input_artwork_primary_type_other', res.data[0].PRIMARY_TYPE_OTHER);

        setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_brand_other', res.data[0].BRAND_ID, res.data[0].BRAND_DISPLAY_TXT);
        setValueToDDLOther('.cls_artwork_request_form .cls_input_artwork_brand_other', res.data[0].BRAND_OTHER);
        $('.cls_artwork_request_form .cls_txt_artwork_special_req').val(res.data[0].SPECIAL_REQUIREMENT);
        bindArtworkRequestDeliveryDate(res.data[0].REQUEST_DELIVERY_DATE);
        $('.cls_artwork_request_form .cls_txt_artwork_other_requests').val(res.data[0].OTHER_REQUEST);

        $('.table_artwork_product_code_item tbody tr:visible').remove();
        if (res.data[0].PRODUCT != null) {
            if (res.data[0].PRODUCT.length > 0) {
                $("input[name=req_artwork_product_code_ref_no][value=product_code]").prop('checked', true);
                bindArtworkProductCode(res.data[0].PRODUCT);
                setArtworkProductCodeRefNo('product_code');
            }
        }

        $('.table_artwork_ref_no_item tbody tr:visible').remove();
        if (res.data[0].REFERENCE != null) {
            if (res.data[0].REFERENCE.length > 0) {
                $("input[name=req_artwork_product_code_ref_no][value=ref_no]").prop('checked', true);
                bindArtworkRefNo(res.data[0].REFERENCE);
                setArtworkProductCodeRefNo('ref_no');

                var lbl_spanStar = "<span style=\"color: red;\">*</span>";
                $('.cls_artwork_lbl_th_net_weight:visible').html("Net weight" + lbl_spanStar);

                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_primary_size_other', res.data[0].THREE_P_ID, res.data[0].THREE_P_DISPLAY_TXT);
                setValueToDDLOther('.cls_artwork_request_form .cls_input_artwork_primary_size_other', res.data[0].PRIMARY_SIZE_OTHER);
                if ((res.data[0].CONTAINER_TYPE_ID == null) && (res.data[0].CONTAINER_TYPE_DISPLAY_TXT != null)) {
                    setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_container_type_other', -99, res.data[0].CONTAINER_TYPE_DISPLAY_TXT);
                    $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').prop("disabled", true);
                } else {
                    setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_container_type_other', res.data[0].CONTAINER_TYPE_ID, res.data[0].CONTAINER_TYPE_DISPLAY_TXT);
                }
                setValueToDDLOther('.cls_artwork_request_form .cls_input_artwork_container_type_other', res.data[0].CONTAINER_TYPE_OTHER);
                if ((res.data[0].LID_TYPE_ID == null) && (res.data[0].LID_TYPE_DISPLAY_TXT != null)) {
                    setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_lid_type_other', -99, res.data[0].LID_TYPE_DISPLAY_TXT);
                    $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').prop("disabled", true);
                } else {
                    setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_lid_type_other', res.data[0].LID_TYPE_ID, res.data[0].LID_TYPE_DISPLAY_TXT);
                }
                setValueToDDLOther('.cls_artwork_request_form .cls_input_artwork_lid_type_other', res.data[0].LID_TYPE_OTHER);

                if (res.data[0].THREE_P_ID != null) {
                    var defaultArtwork_PrimarySizeDataItem = {};
                    defaultArtwork_PrimarySizeDataItem[res.data[0].THREE_P_ID] = {
                        CONTAINER_TYPE_ID: -99,
                        CONTAINER_TYPE_DISPLAY_TXT: res.data[0].CONTAINER_TYPE_DISPLAY_TXT,
                        LID_TYPE_ID: -99,
                        LID_TYPE_DISPLAY_TXT: res.data[0].LID_TYPE_DISPLAY_TXT
                    };
                    defaultArtwork_PrimarySizeData = defaultArtwork_PrimarySizeDataItem;
                }

                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_packing_style_other', res.data[0].TWO_P_ID, res.data[0].TWO_P_DISPLAY_TXT);
                setValueToDDLOther('.cls_artwork_request_form .cls_input_artwork_packing_style_other', res.data[0].PACKING_STYLE_OTHER);
                if ((res.data[0].PACK_SIZE_ID == null) && (res.data[0].PACK_SIZE_DISPLAY_TXT != null)) {
                    setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_pack_size_other', -99, res.data[0].PACK_SIZE_DISPLAY_TXT);
                    $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').prop("disabled", true);
                } else {
                    setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_pack_size_other', res.data[0].PACK_SIZE_ID, res.data[0].PACK_SIZE_DISPLAY_TXT);
                }
                setValueToDDLOther('.cls_artwork_request_form .cls_input_artwork_pack_size_other', res.data[0].PACK_SIZE_OTHER);

                if (res.data[0].TWO_P_ID != null) {
                    var defaultArtwork_PackingStyleDataItem = {};
                    defaultArtwork_PackingStyleDataItem[res.data[0].TWO_P_ID] = {
                        PACK_SIZE_ID: -99,
                        PACK_SIZE_DISPLAY_TXT: res.data[0].PACK_SIZE_DISPLAY_TXT
                    };
                    defaultArtwork_PackingStyleData = defaultArtwork_PackingStyleDataItem;
                }
            }
        }

        if (click_ref == "No") {
            $('.cls_btn_artwork_ref').removeClass('btn-primary').addClass('btn-default');
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_search', click_ref_temp_artwork_request_id, click_ref_temp_artwork_request_txt);
        }
        else if (click_ref == "Yes") {
            $('.cls_btn_artwork_ref').removeClass('btn-default').addClass('btn-primary');
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_search', click_ref_temp_artwork_request_id, click_ref_temp_artwork_request_txt);
        }
        else {
            if (isEmpty(res.data[0].REFERENCE_REQUEST_NO)) {
                $('.cls_btn_artwork_ref').removeClass('btn-primary').addClass('btn-default');
            }
            else {
                $('.cls_btn_artwork_ref').removeClass('btn-default').addClass('btn-primary');
                setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_search', res.data[0].REFERENCE_REQUEST_NO, res.data[0].REFERENCE_REQUEST_NO);
            }

            showBtnArtwork(res.data[0].ARTWORK_REQUEST_ID);
        }
        click_ref_temp_artwork_request_id = '';
        click_ref_temp_artwork_request_txt = '';
        setRequiredFieldArtwork();
        if (inTaskForm) {
            if (res.data[0].IS_FFC) {
                $('.cls_row_artwork_type_of_artwork_ffc').show();
                //$('.cls_artwork_request_form .cls_img_lov_artwork_add_plant_multiple').hide();
                //$('.cls_artwork_request_form .cls_img_lov_artwork_delete_plant_multiple').hide();
            }
            else {
                $('.cls_row_artwork_type_of_artwork_ffc').hide();
                //$('.cls_artwork_request_form .cls_img_lov_artwork_add_plant_multiple').show();
                //$('.cls_artwork_request_form .cls_img_lov_artwork_delete_plant_multiple').show();
            }
        }
        else {
            if (UserPosition == "FFC") {
                $('.cls_row_artwork_type_of_artwork_ffc').show();
                $('.cls_artwork_request_form .cls_img_lov_artwork_add_plant_multiple').hide();
                $('.cls_artwork_request_form .cls_lov_artwork_plant_multiple_static').prop("disabled", true);
                var itemProduct_Plant = [];
                plantItem = {};
                plantItem["PRODUCTION_PLANT_DISPLAY_TXT"] = "1021:TUM Plant 1";
                plantItem["PRODUCTION_PLANT_ID"] = 3;
                itemProduct_Plant.push(plantItem);
                bindArtworkProductionPlant(itemProduct_Plant);
                
            }
            else {
                $('.cls_row_artwork_type_of_artwork_ffc').hide();
                //$('.cls_artwork_request_form .cls_img_lov_artwork_add_plant_multiple').show();
            }
        }
        //Set value for Customer tab
        $('.cls_cus_proname').val(res.data[0].PROJECT_NAME);

        //var sorepeat = GetRequestParam('so');
        //if (sorepeat != null) {
        //    getSalesOrderArtwork();
        //}




        if (inTaskForm) {
            setRequiredFieldArtwork();
        }
    }
}

function bindArtworkCountry(data) {
    if (data != null) {
        $('.tr_artwork_country_multiple_static:visible').remove();
        cnt_lov_artwork_country_multiple = 1;
        if (data.length == 0) {
            var obj = $('.cls_lov_artwork_country_multiple_static:first');
            obj.closest('.tr_artwork_country_multiple_static').find(".cls_img_lov_artwork_add_country_multiple").click();
        }
        else {
            $.each(data, function (index, item) {
                var obj = $('.cls_lov_artwork_country_multiple_static:first');
                obj.closest('.tr_artwork_country_multiple_static').find(".cls_img_lov_artwork_add_country_multiple").click();

                var obj = $('.cls_lov_artwork_country_multiple_static:last');
                $(obj).empty();
                $(obj).append('<option value="' + item.COUNTRY_ID + '">' + item.COUNTRY_DISPLAY_TXT + '</option>');
                $(obj).val(item.COUNTRY_ID);
            });
        }
    }
}

function bindArtworkProductionPlant(data) {
    if (data != null) {
        $('.tr_artwork_plant_multiple_static:visible').remove();
        cnt_lov_artwork_plant_multiple = 1;
        if (data.length == 0) {
            var obj = $('.cls_lov_artwork_plant_multiple_static:first');
            obj.closest('.tr_artwork_plant_multiple_static').find(".cls_img_lov_artwork_add_plant_multiple").click();
            $('.cls_artwork_request_form .cls_display_txt_check_manufacturer').hide();
        }
        else {
            $.each(data, function (index, item) {
                var obj = $('.cls_lov_artwork_plant_multiple_static:first');
                obj.closest('.tr_artwork_plant_multiple_static').find(".cls_img_lov_artwork_add_plant_multiple").click();
 
                var obj = $('.cls_lov_artwork_plant_multiple_static:last');
                $(obj).empty();
                $(obj).append('<option value="' + item.PRODUCTION_PLANT_ID + '">' + item.PRODUCTION_PLANT_DISPLAY_TXT + '</option>');
                $(obj).val(item.PRODUCTION_PLANT_ID);
            });
        }
    } else {
        $('.cls_artwork_request_form .cls_display_txt_check_manufacturer').hide();
    }
}

function bindArtworkRequestDeliveryDate(data) {
    if (!isEmpty(data)) {
        //var date = new Date(data);
        //var temp = pad(date.getDate(), 2) + '/' + pad((date.getMonth() + 1), 2) + '/' + date.getFullYear();
        //$('.cls_artwork_request_form .cls_dt_artwork_rdd').val(temp);
        $('.cls_artwork_request_form .cls_dt_artwork_rdd').val(myDateMoment(data));
    }
}


 //by aof 20230121_3V_SOREPAT INC-93118
function checkArtworkRequestIsComplete(only_save)
{

    // start by aof 20230121_3V_SOREPAT INC-93118

    if (msgconfirm_multiplant_sorepeat != "") {

        var msg_title = "";
        if (only_save)
        {
            msg_title = "Confirm to save request form? ";
        } else
        {
            msg_title = "Confirm to submit request form? ";
        }

        $.confirm({
            title: msg_title, //'Confirm Dialog',
            content: msgconfirm_multiplant_sorepeat,
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
                        // myAjax(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother)
                        //alertError2("OK");
                        checkReviewerInGroupTypeOfProduct(only_save);
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

    } else
    {
        checkReviewerInGroupTypeOfProduct(only_save);
    }

    // end by aof 20230121_3V_SOREPAT INC-93118

}


// ticket 453346  by aof start
function checkReviewerInGroupTypeOfProduct(only_save)
{
    //checkReviewerInGroupTypeOfProduct
    var reviewer_id = $('.cls_artwork_request_form .cls_lov_artwork_reviewer').val();
    if (reviewer_id == null)
    {
        reviewer_id = 0 
    }
    var myurl = '/api/master/getusertypeofproduct?USER_ID=' + reviewer_id + '&ONLY_SAVE=' + only_save;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_GetReviewerInGroupTypeOfProduct);
}

function callback_GetReviewerInGroupTypeOfProduct(res) {
    var f_valid = true;
 
    if (res.data != null && res.data.length > 0)
    {
        var typeofproduct_id = $('.cls_artwork_request_form .cls_lov_artwork_type_of_product').val();
        if (typeofproduct_id != null && typeofproduct_id >= 0)
        {
            f_valid = false;
            for (i = 0; i < res.data.length; i++) {
                if (res.data[i].TYPE_OF_PRODUCT_ID == typeofproduct_id) {
                    f_valid = true;
                    break;  
                }
            }
        }

    }

    if (f_valid) {
        saveDataArtworkRequest(res.ONLY_SAVE);
    } else {
        $('.cls_artwork_request_form .cls_lov_artwork_reviewer').focus();
        alertError2("Reviewer mismatched with type of product.");
    }

}

// ticket 453346  by aof start last

function saveDataArtworkRequest(only_save) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};

    //if (only_save) { jsonObj.validate = false; }

    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    if ($('.cls_artwork_request_form .cls_lov_artwork_search').val() != null) {
        item["REFERENCE_REQUEST_ID"] = $('.cls_artwork_request_form .cls_hid_artwork_copy_ref_id').val();
        item["REFERENCE_REQUEST_NO"] = $('.cls_artwork_request_form .cls_lov_artwork_search').val();
        item["REFERENCE_REQUEST_TYPE"] = $('.cls_artwork_request_form .cls_hid_artwork_copy_ref_type').val();
    }
    item["ARTWORK_REQUEST_TYPE"] = $('.cls_artwork_request_form .cls_input_artwork_request_type').val();//UserPosition;
    item["TYPE_OF_ARTWORK"] = $('input[name=rdo_artwork_type_of_artwork_ffc]:checked').val() == undefined ? "NEW" : $('input[name=rdo_artwork_type_of_artwork_ffc]:checked').val();
    item["PROJECT_NAME"] = $('.cls_artwork_request_form .cls_input_artwork_project_name').val();
    item["TYPE_OF_PRODUCT_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_type_of_product').val();
    item["ARTWORK_REQUEST_NO"] = $('.cls_artwork_request_form .cls_input_artwork_request_form_no').val();
    item["REVIEWER_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_reviewer').val();
    item["COMPANY_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_company').val();
    item["SOLD_TO_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_sold_to').val();
    item["SHIP_TO_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_ship_to').val();
    item["CUSTOMER_OTHER_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_other_to').val();

    item["PRIMARY_TYPE_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_primary_type_other').val();
    item["PRIMARY_TYPE_OTHER"] = $('.cls_artwork_request_form .cls_input_artwork_primary_type_other').val();
    item["THREE_P_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_primary_size_other').val();
    //item["PRIMARY_SIZE_ID"] = $('.cls_artwork_request_form .cls_lov_primary_size_other').val();
    item["PRIMARY_SIZE_OTHER"] = $('.cls_artwork_request_form .cls_input_artwork_primary_size_other').val();
    if ($('.cls_artwork_request_form .cls_lov_artwork_primary_size_other').val() == -1) {
        item["CONTAINER_TYPE_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').val();
        item["CONTAINER_TYPE_OTHER"] = $('.cls_artwork_request_form .cls_input_artwork_container_type_other').val();
        item["LID_TYPE_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').val();
        item["LID_TYPE_OTHER"] = $('.cls_artwork_request_form .cls_input_artwork_lid_type_other').val();
    }
    item["TWO_P_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').val();
    //item["PACKING_STYLE_ID"] = $('.cls_artwork_request_form .cls_lov_packing_style_other').val();
    item["PACKING_STYLE_OTHER"] = $('.cls_artwork_request_form .cls_input_artwork_packing_style_other').val();
    if ($('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').val() == -1) {
        item["PACK_SIZE_OTHER"] = $('.cls_artwork_request_form .cls_input_artwork_pack_size_other').val();
    }
    item["PACK_SIZE_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').val();
    item["BRAND_ID"] = $('.cls_artwork_request_form .cls_lov_artwork_brand_other').val();
    item["BRAND_OTHER"] = $('.cls_artwork_request_form .cls_input_artwork_brand_other').val();

    //ticket#459465 by aof
    if ($('input[name=req_artwork_product_code_ref_no]:checked').val() == 'product_code')
    {
        item["THREE_P_ID"] = null;
        item["PRIMARY_SIZE_OTHER"] = ""; 
        item["TWO_P_ID"] = null;
        item["PACKING_STYLE_OTHER"] = "";
        item["CONTAINER_TYPE_ID"] = null;
        item["CONTAINER_TYPE_OTHER"] = null;
        item["LID_TYPE_ID"] = null;
        item["LID_TYPE_OTHER"] = null;
        item["PACK_SIZE_ID"] = null;
        item["PACK_SIZE_OTHER"] = null;

    }
    //ticket#459465 by aof

    item["IN_TRANSIT_TO"] = $('.cls_artwork_request_form .cls_input_artwork_in_tran').val();
    item["VIA"] = $('.cls_artwork_request_form .cls_input_artwork_via').val();
    item["REQUEST_FORM_FOLDER_NODE_ID"] = $('.cls_artwork_request_form .cls_hid_artwork_node_id').val();
    if (!isEmpty($('.cls_artwork_request_form .cls_dt_artwork_rdd').datepicker("getDate"))) {
        item["REQUEST_DELIVERY_DATE"] = $('.cls_artwork_request_form .cls_dt_artwork_rdd').datepicker("getDate").addHours(7);
    }
    item["SPECIAL_REQUIREMENT"] = $('.cls_artwork_request_form .cls_txt_artwork_special_req').val();
    item["OTHER_REQUEST"] = $('.cls_artwork_request_form .cls_txt_artwork_other_requests').val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;
    item["CREATOR_ID"] = UserID;

    var saleorderList = [];
    var saleorders = $(".cls_input_artwork_salesorder").val().split(/\n/);
    for (var i = 0; i < saleorders.length; i++) {
        saleorders[i] = saleorders[i].trim();
        if (saleorders[i] != "") {
            saleorderItem = {};
            saleorderItem["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            saleorderItem["SALES_ORDER_NO"] = saleorders[i];
            saleorderItem["CREATE_BY"] = UserID;
            saleorderItem["UPDATE_BY"] = UserID;
            saleorderList.push(saleorderItem);
        }
    }

    var mailCustomerList = [];
    var first = true;
    $(".cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                var mailToItem = {};
                mailToItem["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
                mailToItem["CUSTOMER_USER_ID"] = $(this).val();
                mailToItem["MAIL_TO"] = "X";
                mailToItem["MAIL_CC"] = "";
                mailToItem["CREATE_BY"] = UserID;
                mailToItem["UPDATE_BY"] = UserID;
                mailCustomerList.push(mailToItem);
            }
        }
    });

    first = true;
    $(".cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                var mailCcItem = {};
                mailCcItem["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
                mailCcItem["CUSTOMER_USER_ID"] = $(this).val();
                mailCcItem["MAIL_TO"] = "";
                mailCcItem["MAIL_CC"] = "X";
                mailCcItem["CREATE_BY"] = UserID;
                mailCcItem["UPDATE_BY"] = UserID;
                mailCustomerList.push(mailCcItem);
            }
        }
    });

    var countryList = [];
    first = true;
    $(".cls_artwork_request_form .cls_lov_artwork_country_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                countryItem = {};
                countryItem["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
                countryItem["COUNTRY_ID"] = $(this).val();
                countryItem["CREATE_BY"] = UserID;
                countryItem["UPDATE_BY"] = UserID;
                countryList.push(countryItem);
            }
        }
    });

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

    first_row = true;
    var productList = [];
    var referenceList = [];
    if ($('input[name=req_artwork_product_code_ref_no]:checked').val() == 'product_code') {
        $(".cls_artwork_request_form .table_artwork_product_code_item tbody tr").each(function (index) {
            if (first_row) first_row = false;
            else {
                productItem = {};
                var product_type = "";
                if ($(this).find('.cls_artwork_vap').is(':checked')) {
                    product_type = "VAP";
                } else if ($(this).find('.cls_artwork_ffc').is(':checked'))
                    product_type = "FFC";
                productItem["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
                productItem["PRODUCT_CODE_ID"] = $(this).find('.cls_hid_artwork_productcode_id').val();
                productItem["PRODUCT_TYPE"] = product_type;
                //productItem["DRAINED_WEIGHT"] = $(this).find('.cls_artwork_drain_weight').val();
                //productItem["PRIMARY_SIZE"] = $(this).find('.cls_artwork_primary_size').val();
                //productItem["CONTAINER_TYPE"] = $(this).find('.cls_artwork_container_type').val();
                //productItem["LID_TYPE"] = $(this).find('.cls_artwork_lid_type').val();
                //productItem["PACKING_STYLE"] = $(this).find('.cls_artwork_packing_style').val();
                //productItem["PACK_SIZE"] = $(this).find('.cls_artwork_pack_size').val();
                productItem["CREATE_BY"] = UserID;
                productItem["UPDATE_BY"] = UserID;
                productList.push(productItem);
            }
        });
    } else {
        $(".cls_artwork_request_form .table_artwork_ref_no_item tbody tr").each(function (index) {
            if (first_row) first_row = false;
            else {
                referenceItem = {};
                var reference_type = "";

                if ($(this).find('.cls_artwork_ref_no_vap').is(':checked')) {
                    reference_type = "VAP";
                } else if ($(this).find('.cls_artwork_ref_no_ffc').is(':checked'))
                    reference_type = "FFC";
                referenceItem["PRODUCT_TYPE"] = reference_type;
                referenceItem["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
                referenceItem["REFERENCE_NO"] = $(this).find('.cls_artwork_ref_no').val();
                referenceItem["NET_WEIGHT"] = $(this).find('.cls_artwork_net_weight' + 'X').val();
                referenceItem["DRAINED_WEIGHT"] = $(this).find('.cls_artwork_drain_weight').val();
                referenceItem["CREATE_BY"] = UserID;
                referenceItem["UPDATE_BY"] = UserID;
                referenceList.push(referenceItem);
            }
        });
    }

    if (inTaskForm) {
        if (!only_save) {
            item["ARTWORK_SUB_ID"] = ArtworkSubId;
            item["ENDTASKFORM"] = true;
        }
    }

    jsonObj.data = item;
    jsonObj.data.SALES_ORDER = saleorderList;
    jsonObj.data.COUNTRY = countryList;
    jsonObj.data.PRODUCTION_PLANT = plantList;
    jsonObj.data.PRODUCT = productList;
    jsonObj.data.REFERENCE = referenceList;
    jsonObj.data.MAIL_TO_CUSTOMER = mailCustomerList;

    var myurl = '/api/artwork/artworkrequest';
    var mytype = 'POST';
    var mydata = jsonObj;


    var validateref_no = true;
    if ($('input[name=rdo_artwork_type_of_artwork_ffc]:checked').val() == 'REPEAT' && UserPosition == "FFC") {
        //if (UserPosition == "FFC") {
        validateref_no = false;
    }

    if (only_save)
        myAjax(myurl, mytype, mydata, callbacksaveDataArtworkRequest);
    else {
        if ($('input[name=req_artwork_product_code_ref_no]:checked').val() == 'product_code' && productList.length == 0 && validateref_no) {
            alertError2("Product code must contain at least 1 item");
        } else if ($('input[name=req_artwork_product_code_ref_no]:checked').val() == 'ref_no' && referenceList.length == 0 && validateref_no) {
            alertError2("RD reference No./3V must contain at least 1 item");
        } else {
            myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSubmitDataArtwork, '', false, false);
        }
    }
}
 

function callbacksaveDataArtworkRequest(res) {
    ARTWORK_REQUEST_ID = res.data[0].ARTWORK_REQUEST_ID;
    if (!inTaskForm) $('.cls_btn_artwork_delete').show();
}

function callbackSubmitDataArtwork(res) {
    ARTWORK_REQUEST_ID = res.data[0].ARTWORK_REQUEST_ID;

    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    //item["IS_REPEAT"] = $('.cls_artwork_request_form .cls_hid_artwork_sorepeat').val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;
    item["CREATOR_ID"] = UserID;
    jsonObj.data = item;

    if (typeof ArtworkSubId !== 'undefined') {
        var PROCESS = {};
        PROCESS["ARTWORK_SUB_ID"] = ArtworkSubId;
        PROCESS["ENDTASKFORM"] = true;
        jsonObj.data.PROCESS = PROCESS;
    }

    var myurl = '/api/artwork/submitrequest';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (inTaskForm)
        myAjax(myurl, mytype, mydata, tohomepage);
    else
        myAjaxAlert(myurl, mytype, mydata, "", "", true, true);
}

function getArtworkProductCode(jsonObj) {
    var myurl = '/api/artwork/product';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callback_set_productcode);
}
function getArtworkProduct_vap(jsonObj) {
    var myurl = '/api/artwork/product_vap';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callback_set_product_vap);
}
function callback_set_product_vap(res) {
    if (res.data.length > 0) {
    }
}
function callback_set_productcode(res) {
    if (res.data.length > 0) {
        bindArtworkProductCode(res.data);
        setArtworkProductCodeRefNo('product_code');
    }
    else {
        alertError2("Product code not found");
    }
}

function getSalesOrderArtwork() {
    var salesorders = $(".cls_input_artwork_salesorder").val().split(/\n/);
    var strAlert = "";
    var jsonObj = new Object();
    jsonObj.data = [];
    for (var i = 0; i < salesorders.length; i++) {
        salesorders[i] = salesorders[i].trim();
        if (salesorders[i] != "") {
            var isDup = false;
            for (var j = 0; j < jsonObj.data.length; j++) {
                if (jsonObj.data[j]["SALES_ORDER_NO"].toLowerCase() == salesorders[i]) {
                    isDup = true;
                }
            }

            if (!isDup) {
                var item = {};
                item["SALES_ORDER_NO"] = salesorders[i];
                item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
                item["PRODUCT_TYPE"] = $('.cls_artwork_request_form .cls_input_artwork_request_type').val();;
                jsonObj.data.push(item);
            }
        }
    }

    var myurl = '/api/artwork/salesorderrequest';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callback_get_SalesOrderArtwork);
}

function callback_get_SalesOrderArtwork(res) {
    if (res.data.length > 0) {
        var index = 0;
        for (var i = 0; i < res.data.length; i++) {
            if (res.data[i].PRODUCTION_PLANT != null) {
                var plantList = [];
                plantList = res.data[i].PRODUCTION_PLANT;
                var in_array = plantList.filter(function (item) {
                    return item.PRODUCTION_PLANT_ID == 3  
                });
                var index = plantList.indexOf(in_array[0]);
                if (index == -1) {
                    $('.cls_input_artwork_salesorder').val('');
                }
            }
            //if (index != -1 && UserPosition == "FFC") {

            //    var ProductList = [];
            //    ProductList = res.data[i].PRODUCT;
            //    var array = ProductList.filter(function (item) {
            //        return item.PRODUCT_CODE.substring(2, 1) == "E"
            //    });
            //    var arr_index = ProductList.indexOf(array[0]);
            //}
        }
        //var arr_index = get_vap(res);
        //if (index == -1 && UserPosition == "FFC") {
        //    alertError2("Sales order is mismatch");
        //} else if (arr_index == -1 && UserPosition == "FFC") {
        //    res.status = "E";
        //    alertError2("Product code is not found.Please contact your system administrator.");
        //} else
        if (plantList == undefined) 
            return false;
        //else if (plantList.length > 1 && UserPosition != "FFC") {
        //    alertError2("If your product is a VAP, Please select the VAP product flag. Default product flag function is not supported multiple production plants.<br/>");
        //}
         else {
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_reviewer', res.data[0].REVIEWER_ID, res.data[0].REVIEWER_DISPLAY_TXT);

            //if ($('.cls_artwork_request_form .cls_lov_artwork_sold_to').val() != res.data[0].SOLD_TO_ID && $('.cls_artwork_request_form .cls_lov_artwork_ship_to').val() != res.data[0].SHIP_TO_ID) {
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_sold_to', res.data[0].SOLD_TO_ID, res.data[0].SOLD_TO_DISPLAY_TXT);
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_ship_to', res.data[0].SHIP_TO_ID, res.data[0].SHIP_TO_DISPLAY_TXT);
            $(".cls_artwork_request_form .cls_lov_artwork_ship_to").change();
            //}

            bindArtworkCountry(res.data[0].COUNTRY);
            bindArtworkProductionPlant(res.data[0].PRODUCTION_PLANT);
            setValueToDDL('.cls_artwork_request_form .cls_lov_artwork_brand_other', res.data[0].BRAND_ID, res.data[0].BRAND_DISPLAY_TXT);
            bindArtworkRequestDeliveryDate(res.data[0].REQUEST_DELIVERY_DATE);

            if (res.data[0].IN_TRANSIT != null)
                $('.cls_artwork_request_form .cls_input_artwork_in_tran').val(res.data[0].IN_TRANSIT);
            else
                $('.cls_artwork_request_form .cls_input_artwork_in_tran').val("");

            if (res.data[0].VIA != null)
                $('.cls_artwork_request_form .cls_input_artwork_via').val(res.data[0].VIA);
            else
                $('.cls_artwork_request_form .cls_input_artwork_via').val("");

            var sorepeat = GetRequestParam('so');
            if (sorepeat != null) {
            }
            else {
                $('.table_artwork_product_code_item tbody tr:visible').remove();
                for (var i = 0; i < res.data.length; i++) {
                    if (res.data[i].PRODUCT != null) {
                        if (res.data[i].PRODUCT.length > 0) {
                            $("input[name=req_artwork_product_code_ref_no][value=product_code]").prop('checked', true);
                            bindArtworkProductCode(res.data[i].PRODUCT);
                            setArtworkProductCodeRefNo('product_code');
                        }
                    }
                }
            }

            var saleorder = '';
            $.each(res.data, function (index, item) {
                saleorder += item.SALES_ORDER_NO + "\r\n";
            });
            $('.cls_input_artwork_salesorder').val(saleorder);

            if (res.msg.length == 0) {
                alertError2("Sales orders are inconsistency");
            }
        }
    }
    else {

        alertError2("Sales order not found");
    }
}
function get_vap(res) {
    var ProductList = [];
    ProductList = res.data[0].PRODUCT;
    var array = ProductList.filter(function (item) {
        return item.PRODUCT_TYPE == "VAP"
    });
    var arr_index = ProductList.indexOf(array[0]);
    return arr_index;
}
function bindArtworkProductCode(data) {
    var strAlert = "";
    var msgAlert = "";
    
    for (var i = 0; i < data.length; i++) {
        var isSame = true;
        var v = data[i];
        var str_net_weight = "";
        var str_product_desc = "";
        var str_drain_weight = "";
        var str_primary_size = "";
        var str_container_type = "";
        var str_lid_type = "";
        var str_packing_style = "";
        var str_pack_size = "";

        if (v.NET_WEIGHT != null) {
            str_net_weight = v.NET_WEIGHT;
        }
        if (v.PRODUCT_DESCRIPTION != null) {
            str_product_desc = v.PRODUCT_DESCRIPTION;
        }
        if (v.DRAINED_WEIGHT != null) {
            str_drain_weight = v.DRAINED_WEIGHT;
        }
        if (v.PRIMARY_SIZE != null) {
            str_primary_size = v.PRIMARY_SIZE;
        }
        if (v.CONTAINER_TYPE != null) {
            str_container_type = v.CONTAINER_TYPE;
        }
        if (v.LID_TYPE != null) {
            str_lid_type = v.LID_TYPE;
        }
        if (v.PACKING_STYLE != null) {
            str_packing_style = v.PACKING_STYLE;
        }
        if (v.PACK_SIZE != null) {
            str_pack_size = v.PACK_SIZE;
        }

        var obj = $('.cls_artwork_request_form .table_artwork_product_code_item tbody tr:last');
        if (obj.find('.cls_artwork_product_code').val() != undefined) {
            if (obj.find('.cls_artwork_product_code').val().toLowerCase() != "") {
                if (str_net_weight.toLowerCase() != obj.find('.cls_artwork_net_weight').val().toLowerCase()
                    || str_primary_size.toLowerCase() != obj.find('.cls_artwork_primary_size').val().toLowerCase()
                    || str_container_type.toLowerCase() != obj.find('.cls_artwork_container_type').val().toLowerCase()
                    || str_lid_type.toLowerCase() != obj.find('.cls_artwork_lid_type').val().toLowerCase()
                    || str_packing_style.toLowerCase() != obj.find('.cls_artwork_packing_style').val().toLowerCase()
                    || str_pack_size.toLowerCase() != obj.find('.cls_artwork_pack_size').val().toLowerCase()) {
                    isSame = false;
                    strAlert = strAlert + v.PRODUCT_CODE + "<br>";
                }
            }
        }

        if (isSame) {
            var isDup = false;
            $('.table_artwork_product_code_item > tbody > tr').each(function () {
                if (v.PRODUCT_CODE.toLowerCase() == $(this).find('.cls_artwork_product_code').val().toLowerCase()) {
                    isDup = true;
                }
            });

            if (!isDup) {
                var obj = $('.tr_artwork_product_code_item').clone().removeClass('tr_artwork_product_code_item').removeClass('cls_cn_hide');

                $('.cls_artwork_request_form .table_artwork_product_code_item tbody').append(obj);
                var obj2 = $('.cls_artwork_request_form .table_artwork_product_code_item tbody tr:last');

                $(".cls_artwork_request_form .cls_img_artwork_delete_product_code_item").click(function () {
                    $(this).closest('tr').remove();
                });
                $(".cls_artwork_request_form .cls_artwork_vap").click(function () {
                    if ($(this).is(':checked')) {
                        input = 'cls_artwork_ffc';
                        obj.find('.' + input).prop('checked', false);
                    }
                });
                $(".cls_artwork_request_form .cls_artwork_ffc").click(function () {
                    if ($(this).is(':checked')) {
                        input = 'cls_artwork_vap';
                        obj.find('.' + input).prop('checked', false);
                    }
                });
                input = 'cls_hid_artwork_productcode_id';
                obj.find('.' + input).val(v.PRODUCT_CODE_ID);

                input = 'cls_artwork_product_code';
                obj.find('.' + input).val(v.PRODUCT_CODE);

                input = 'cls_artwork_product_desc';
                obj.find('.' + input).val(v.PRODUCT_DESCRIPTION);

                input = 'cls_artwork_net_weight';
                obj.find('.' + input).val(v.NET_WEIGHT);

                input = 'cls_artwork_drain_weight';
                obj.find('.' + input).val(v.DRAINED_WEIGHT);

                input = 'cls_artwork_primary_size';
                obj.find('.' + input).val(v.PRIMARY_SIZE);

                input = 'cls_artwork_container_type';
                obj.find('.' + input).val(v.CONTAINER_TYPE);

                input = 'cls_artwork_lid_type';
                obj.find('.' + input).val(v.LID_TYPE);

                input = 'cls_artwork_packing_style';
                obj.find('.' + input).val(v.PACKING_STYLE);

                input = 'cls_artwork_pack_size';
                obj.find('.' + input).val(v.PACK_SIZE);

                input = 'cls_artwork_vap';

                var plantList = [];
                plantList = getplantlist();
                var in_array = plantList.filter(function (item) {
                    return item.PRODUCTION_PLANT_ID == 3
                });
                var isenable;
                var artwork_request_type = $('.cls_artwork_request_form .cls_input_artwork_request_type').val();
                var index = plantList.indexOf(in_array[0]);
                //ARTWORK_REQUEST_ID
                //if (v.PRODUCT_CODE_ID != null && v.PRODUCT_TYPE == "VAP") {
                //    obj.find('.' + input).prop('checked', true);
                //    obj.find('.' + input).prop("disabled", false);
                //} else

              //  // start by aof 20230121_3V_SOREPAT INC-93118
              //  var vap_plant_id = 3; // by aof 20230121_3V_SOREPAT INC-93118
              //  if (typeartwork == "REPEAT" && res.data[0].PRODUCT != null && res.data[0].PRODUCTION_PLANT != null) {
              //      if (res.data[0].PRODUCT[0].PRODUCT_TYPE_CHECK != null && res.data[0].PRODUCT[0].PRODUCT_TYPE_CHECK == "VAP") {
              //          if (res.data[0].PRODUCTION_PLANT.length > 1) {
              //              $.each(res.data[0].PRODUCTION_PLANT, function (index, item) {
              //                  if (item.PRODUCTION_PLANT_ID = vap_plant_id) {
              //                      msgconfirm_multiplant_sorepeat = "VAP product with multiple plants. Do you want to confrim?";
              //                  }
              //              });
              //          }
              //      }
              //  }
                msgconfirm_multiplant_sorepeat = "";
              //// end by aof 20230121_3V_SOREPAT INC-93118
  
                if (plantList.length > 1 && v.PRODUCT_TYPE == "VAP") {
                    obj.find('.' + input).prop('checked', false);
                    obj.find('.' + input).prop("disabled", false);
                    if (typeartwork == "REPEAT" && typecheck_vap == "load")
                    {
                        msgconfirm_multiplant_sorepeat = "Default product flag function is not supported multiple production plants. You have to select the VAP product flag If your product is a VAP."; //by aof 20230121_3V_SOREPAT INC - 93118
                    }
                    
                } else if (plantList.length == 1 && v.PRODUCT_TYPE == "VAP") {
                    obj.find('.' + input).prop('checked', true);
                    obj.find('.' + input).prop("disabled", true);
                } else if (v.PRODUCT_TYPE == "VAP_FIX" && index >= 0) {
                    obj.find('.' + input).prop('checked', true);
                    obj.find('.' + input).prop("disabled", true);
                } else if (plantList.length > 0 && index == -1) {
                    obj.find('.' + input).prop("disabled", true);
                }
                else if (plantList.length > 0 && index >= 0)
                    obj.find('.' + input).prop("disabled", true);

                if (obj.find('.cls_artwork_vap').is(':checked') == false
                    && obj.find('.cls_artwork_ffc').is(':checked') == false
                    && plantList.length > 1 && index >= 0 && typecheck_vap=="" && ARTWORK_REQUEST_READONLY == "0") {
                    msgAlert = "If your product is a VAP, Please select the VAP product flag. Default product flag function is not supported multiple production plants.";
                }
                //var checked_status = false;
                //if (artwork_request_type == "FFC") {
                //    checked_status = true;


                input = 'cls_artwork_ffc';
                if (v.PRODUCT_TYPE == "FFC")
                    obj.find('.' + input).prop('checked', true);
                if (artwork_request_type == "FFC")
                    obj.find('.' + input).prop('checked', true);
                obj.find('.' + input).prop("disabled", true);
                
            }
        }
    }
    if (msgAlert.length > 0) {
        alertError2(msgAlert);
    }
    if (strAlert.length > 0) {
        alertError2(strAlert + " not same characteristic value (net weight, primary size, container type, lid type, pack size and packing style)")
    }
    typecheck_vap = "";
}

function bindArtworkRefNo(data) {
    var msgAlert = "";
    for (var i = 0; i < data.length; i++) {
        var v = data[i];
        var obj = $('.tr_artwork_ref_no_item').clone().removeClass('tr_artwork_ref_no_item').removeClass('cls_cn_hide');

        $('.cls_artwork_request_form .table_artwork_ref_no_item tbody').append(obj);
        var obj2 = $('.cls_artwork_request_form .table_artwork_ref_no_item tbody tr:last');

        $(".cls_artwork_request_form .cls_img_artwork_delete_ref_no_item").click(function () {
            $(this).closest('tr').remove();
            if ($('.table_artwork_ref_no_item tbody tr:visible').length == 0) {
                $('.cls_artwork_lbl_th_net_weight:visible').html("Net weight");
            }
        });
        $(".cls_artwork_request_form .cls_artwork_ref_no_vap").click(function () {
            if ($(this).is(':checked')) {
                input = 'cls_artwork_ref_no_ffc';
                obj.find('.' + input).prop('checked', false);
            }
        });
        $(".cls_artwork_request_form .cls_artwork_ref_no_ffc").click(function () {
            if ($(this).is(':checked')) {
                input = 'cls_artwork_ref_no_vap';
                obj.find('.' + input).prop('checked', false);
            }
        });

        input = 'cls_artwork_ref_no';
        obj.find('.' + input).val(v.REFERENCE_NO);

        //if (RDREF == "")
        //    RDREF = v.REFERENCE_NO;
        //else if (RDREF != "")
        //    RDREF += "," + v.REFERENCE_NO;

        input = 'cls_artwork_net_weight';
        obj.find('.' + input).toggleClass(input + ' ' + input + 'X');
        obj.find('.' + input + 'X').val(v.NET_WEIGHT);

        if ($('input[name=rdo_artwork_type_of_artwork_ffc]:checked').val() == 'REPEAT' && UserPosition == "FFC") {
        }
        else {
            $('.' + input + 'X').attr("required", true);
        }
        input = 'cls_artwork_drain_weight';
        obj.find('.' + input).val(v.DRAINED_WEIGHT);

        input = 'cls_artwork_ref_no_vap';
        var plantList = [];
        plantList = getplantlist();
        var in_array = plantList.filter(function (item) {
            return item.PRODUCTION_PLANT_ID == 3
        });
        var artwork_request_type = $('.cls_artwork_request_form .cls_input_artwork_request_type').val();
        var index = plantList.indexOf(in_array[0]);
        var checked_status = false;
        if(v.PRODUCT_TYPE == "VAP")
            obj.find('.' + input).prop('checked',true);
        else
            obj.find('.' + input).prop('checked', false);

        if (artwork_request_type == "FFC") {
            checked_status = true;
        } else if (plantList.length > 0 && index >= 0)
            checked_status = false;
        else if (plantList.length > 0 && index == -1)
            checked_status = true;

        obj.find('.' + input).prop("disabled", checked_status);
 
        input = 'cls_artwork_ref_no_ffc';
        if (v.PRODUCT_TYPE == "FFC")
            obj.find('.' + input).prop('checked', true);
        else if (artwork_request_type == "FFC")
            obj.find('.' + input).prop('checked', true);
        obj.find('.' + input).prop("disabled", true);

        if (obj.find('.cls_artwork_ref_no_vap').is(':checked') == false
            && obj.find('.cls_artwork_ref_no_ffc').is(':checked') == false && plantList.length > 1 && index >= 0 && ARTWORK_REQUEST_READONLY == "0" && typecheck_vap == "") {
                msgAlert = "If your product is a VAP, Please select the VAP product flag. Default product flag function is not supported multiple production plants.< br /> ";
        }
    }
    if (msgAlert.length > 0) {
        alertError2(msgAlert);
    }
    typecheck_vap = "";
    //$('.cls_txt_product_code').val(RDREF);
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
function setArtworkProductCodeRefNo(val) {
    if (val == 'product_code') {
        $('.cls_artwork_request_form .cls_row_artwork_product_code_header').show();
        $('.cls_artwork_request_form .cls_row_artwork_product_code_detail').show();
        $('.cls_artwork_request_form .cls_row_artwork_ref_no_header').hide();
        $('.cls_artwork_request_form .cls_row_artwork_ref_no_detail1').hide();
        $('.cls_artwork_request_form .cls_row_artwork_ref_no_detail2').hide();

        $('.cls_artwork_request_form .cls_lov_artwork_primary_size_other').prop("disabled", true);
        $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').prop("disabled", true);
        $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').prop("disabled", true);
        $('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').prop("disabled", true);
        $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').prop("disabled", true);

        $('.cls_artwork_request_form .cls_artwork_net_weight' + 'X').attr("required", false);
    }
    else if (val == 'ref_no') {
        $('.cls_artwork_request_form .cls_row_artwork_product_code_header').hide();
        $('.cls_artwork_request_form .cls_row_artwork_product_code_detail').hide();
        $('.cls_artwork_request_form .cls_row_artwork_ref_no_header').show();
        $('.cls_artwork_request_form .cls_row_artwork_ref_no_detail1').show();
        $('.cls_artwork_request_form .cls_row_artwork_ref_no_detail2').show();

        $('.cls_artwork_request_form .cls_lov_artwork_primary_size_other').prop("disabled", false);
        if ($('.cls_artwork_request_form .cls_lov_artwork_container_type_other').val() == -99) {
            $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').prop("disabled", true);
        } else {
            $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').prop("disabled", false);
        }
        if ($('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').val() == -99) {
            $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').prop("disabled", true);
        } else {
            $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').prop("disabled", false);
        }
        $('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').prop("disabled", false);
        if ($('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').val() == -99) {
            $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').prop("disabled", true);
        } else {
            $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').prop("disabled", false);
        }

        $('.cls_artwork_request_form .cls_artwork_net_weight' + 'X').attr("required", true);
    }
    setRequiredFieldArtwork();
}

function bindArtworkSearchRequesForm(soldto, shipto, brand, country, creator) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["SOLD_TO_ID"] = soldto;
    item["SHIP_TO_ID"] = shipto;
    item["BRAND_ID"] = brand;
    item["COUNTRY_ID"] = country;
    item["CREATOR_ID"] = creator;
    jsonObj.data = item;

    var table = $('#table_artwork_search').DataTable();
    table.destroy();

    table = $('#table_artwork_search').DataTable({
        "processing": true,
        "scrollX": true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/common/searchrequest",
                type: 'POST',
                data: jsonObj,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            { data: "REQUEST_FORM_NO", "className": "cls_nowrap" },
            { data: "PROJECT_NAME", "className": "cls_nowrap" },
            { data: "COMPANY_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "TYPE", "className": "cls_nowrap" },
            { data: "PRIMARY_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKING_STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACK_SIZE_DISPLAY_TXT", "className": "cls_nowrap" }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: [[1, 'asc']],
        rowCallback: function (row, data, index) {
            if (data.CHECK_LIST_ID > 0) {
                $(row).find('td').eq(1).html('<a href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;" target="_blank">' + data.REQUEST_FORM_NO + '</a>');
            }
            else {
                $(row).find('td').eq(1).html('<a href="' + suburl + '/Artwork/' + data.ARTWORK_ID + '" style="text-decoration: underline;" target="_blank">' + data.REQUEST_FORM_NO + '</a>');
            }
        }
    });
}

function resetMailToCcArtwork(isFFC) {
    if (isFFC) {
        $('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static').prop("disabled", false);
        $('.cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static').prop("disabled", false);
        var frist = true;
        $('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static:visible').each(function (index) {
            if (frist) {
                frist = false;
                $(this).prop("disabled", true);
            }
            else {
                $(this).val('').trigger("change");
                if ($('.cls_artwork_request_form .cls_lov_artwork_sold_to').val() == null && $('.cls_artwork_request_form .cls_lov_artwork_ship_to').val() == null && $('.cls_artwork_request_form .cls_lov_artwork_other_to').val() == null) {
                    $(this).prop("disabled", true);
                }
                else {
                    $(this).prop("disabled", false);
                }
            }
        });
        var frist = true;
        $('.cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static:visible').each(function (index) {
            if (frist) {
                frist = false;
                $(this).prop("disabled", true);
            }
            else {
                $(this).val('').trigger("change");
                if ($('.cls_artwork_request_form .cls_lov_artwork_sold_to').val() == null && $('.cls_artwork_request_form .cls_lov_artwork_ship_to').val() == null && $('.cls_artwork_request_form .cls_lov_artwork_other_to').val() == null) {
                    $(this).prop("disabled", true);
                }
                else {
                    $(this).prop("disabled", false);
                }
            }
        });
    }
    else {
        $('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static').val('').trigger("change");
        $('.cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static').val('').trigger("change");
        if ($('.cls_artwork_request_form .cls_lov_artwork_sold_to').val() == null && $('.cls_artwork_request_form .cls_lov_artwork_ship_to').val() == null && $('.cls_artwork_request_form .cls_lov_artwork_other_to').val() == null) {
            $('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static').prop("disabled", true);
            $('.cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static').prop("disabled", true);
        } else {
            $('.cls_artwork_request_form .cls_lov_artwork_mailto_multiple_static').prop("disabled", false);
            $('.cls_artwork_request_form .cls_lov_artwork_mailcc_multiple_static').prop("disabled", false);
        }
    }
}

function setRequiredFieldArtwork() {
    var lbl_spanStar = "<span style=\"color: red;\">*</span>";

    $('.cls_artwork_request_form .cls_lbl_artwork_other_requests').html("Other requests" + " :");
    $('.cls_artwork_request_form .cls_txt_artwork_other_requests').attr("required", false);

    if (UserPosition == "FFC" || CreateRequestByFFC == "1") {
        if ($('input[name=rdo_artwork_type_of_artwork_ffc]:checked').val() == 'NEW') {
            $('.cls_artwork_request_form .cls_lbl_artwork_packing_style').html("Packing style" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').attr("required", false);

            $('.cls_artwork_request_form .td_artwork_plant_txt_multiple').html("Production plant" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_plant_multiple').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_artwork_brand').html("Brand" + lbl_spanStar + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_brand_other').attr("required", true);

            $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').prop("disabled", false);

            $('.cls_artwork_request_form .cls_lbl_sold_to').html("Sold to" + lbl_spanStar + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_sold_to').attr("required", true);

            $('.cls_artwork_request_form .cls_lbl_ship_to').html("Ship to" + lbl_spanStar + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_ship_to').attr("required", true);

            $('.cls_artwork_request_form .cls_lbl_country').html("Country" + lbl_spanStar + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_country_multiple_static:visible').attr("required", true);

            $('.cls_artwork_request_form .cls_lbl_artwork_primary_type').html("Primary type" + lbl_spanStar + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_primary_type_other').attr("required", true);

            if ($('input[name=req_artwork_product_code_ref_no]:checked').val() == 'ref_no') {
                $('.cls_artwork_request_form .cls_lbl_artwork_primary_size').html("Primary size" + lbl_spanStar + " :");
                $('.cls_artwork_request_form .cls_lov_artwork_primary_size_other').attr("required", true);

                $('.cls_artwork_request_form .cls_lbl_artwork_container_type').html("Container type" + lbl_spanStar + " :");
                $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').attr("required", true);

                $('.cls_artwork_request_form .cls_lbl_artwork_lid_type').html("Lid type" + lbl_spanStar + " :");
                $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').attr("required", true);

                $('.cls_artwork_request_form .cls_lbl_artwork_packing_style').html("Packing style" + " :");
                $('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').attr("required", false);

                $('.cls_artwork_request_form .cls_lbl_artwork_pack_size').html("Pack size" + lbl_spanStar + " :");
                $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').attr("required", true);

                $('.cls_artwork_request_form .cls_artwork_lbl_rd').html("RD reference No./3V" + lbl_spanStar + " :");
                $('.cls_artwork_request_form .cls_artwork_lbl_th_net_weight').html("Net weight" + lbl_spanStar);
                $('.cls_artwork_request_form .cls_artwork_net_weightX').attr("required", true);
            }
            else {
                $('.cls_artwork_request_form .cls_lbl_artwork_primary_size').html("Primary size :");
                $('.cls_artwork_request_form .cls_lov_artwork_primary_size_other').attr("required", false);

                $('.cls_artwork_request_form .cls_lbl_artwork_container_type').html("Container type :");
                $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').attr("required", false);

                $('.cls_artwork_request_form .cls_lbl_artwork_lid_type').html("Lid type :");
                $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').attr("required", false);

                $('.cls_artwork_request_form .cls_lbl_artwork_packing_style').html("Packing style :");
                $('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').attr("required", false);

                $('.cls_artwork_request_form .cls_lbl_artwork_pack_size').html("Pack size :");
                $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').attr("required", false);

                $('.cls_artwork_request_form .cls_artwork_lbl_rd').html("RD reference No./3V :");
                $('.cls_artwork_request_form .cls_artwork_lbl_th_net_weight').html("Net weight");
                $('.cls_artwork_request_form .cls_artwork_net_weightX').attr("required", false);
            }

        }
        else if ($('input[name=rdo_artwork_type_of_artwork_ffc]:checked').val() == 'REPEAT') {
            //$('.cls_artwork_request_form .cls_lbl_artwork_type_of_product').html("Type of product" + " :");
            //$('.cls_artwork_request_form .cls_lov_artwork_type_of_product').attr("required", false);

            //$('.cls_artwork_request_form .cls_lbl_artwork_company').html("Company" + " :");
            //$('.cls_artwork_request_form .cls_lov_artwork_company').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_sold_to').html("Sold to" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_sold_to').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_ship_to').html("Ship to" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_ship_to').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_country').html("Country" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_country_multiple_static').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_artwork_product_code').html("Product code" + " :");

            $('.cls_artwork_request_form .cls_lbl_artwork_primary_type').html("Primary type" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_primary_type_other').attr("required", false);

            $('.cls_artwork_request_form .td_artwork_plant_txt_multiple').html("Production plant" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_plant_multiple').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_artwork_brand').html("Brand" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_brand_other').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_artwork_other_requests').html("Other requests" + lbl_spanStar);
            $('.cls_artwork_request_form .cls_txt_artwork_other_requests:visible').attr("required", true);



            $('.cls_artwork_request_form .cls_lbl_artwork_primary_size').html("Primary size" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_primary_size_other').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_artwork_container_type').html("Container type" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_container_type_other').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_artwork_lid_type').html("Lid type" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_lid_type_other').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_artwork_packing_style').html("Packing style" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_packing_style_other').attr("required", false);

            $('.cls_artwork_request_form .cls_lbl_artwork_pack_size').html("Pack size" + " :");
            $('.cls_artwork_request_form .cls_lov_artwork_pack_size_other').attr("required", false);

            $('.cls_artwork_request_form .cls_artwork_lbl_rd').html("RD reference No./3V" + " :");
            $('.cls_artwork_request_form .cls_artwork_lbl_th_net_weight').html("Net weight");
            $('.cls_artwork_request_form .cls_artwork_net_weightX').attr("required", false);
        }
    }
    else {
        //$('.cls_check_list_form .cls_lbl_remark').html("Remark" + lbl_spanStar);
        //$('.cls_check_list_form .cls_txt_remark:visible').attr("required", true);

        //$('.cls_check_list_form .cls_lbl_product_code').html("Product code");
    }
}