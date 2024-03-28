var cnt_lov_checklist_plant_multiple = 1;   //by aof 04/11/2021 for Itail add Production Plant
var cnt_lov_country_multiple = 1;
var cnt_lov_plant_multiple = 1;
var cnt_lov_mailto_multiple = 1;
var cnt_lov_mailcc_multiple = 1;
var inTaskForm = false;

$(document).ready(function () {
    bind_lov('.cls_lov_type_of_product', '/api/lov/typeofproduct', 'data.type_of_product');
    //bind_lov('.cls_lov_reviewer', '/api/lov/reviewer', 'data.DISPLAY_TXT');

    if (UserPosition == "FFC")
        bind_lov_param('.cls_lov_reviewer', '/api/lov/reviewer_ffc', 'data.DISPLAY_TXT', ["type_of_product_id"], ['.cls_lov_type_of_product']);
    else if (UserPosition == "T-HOLDING")
        bind_lov_param('.cls_lov_reviewer', '/api/lov/reviewer_tholding', 'data.DISPLAY_TXT', ["type_of_product_id"], ['.cls_lov_type_of_product']);
    else
        bind_lov_param('.cls_lov_reviewer', '/api/lov/reviewer', 'data.DISPLAY_TXT', ["type_of_product_id"], ['.cls_lov_type_of_product']);

    bind_lov('.cls_check_list_form .cls_lov_company', '/api/lov/company', 'data.DISPLAY_TXT');
    bind_lov('.cls_check_list_form .cls_lov_sold_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_check_list_form .cls_lov_ship_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_check_list_form .cls_lov_other_to', '/api/lov/customerother', 'data.DISPLAY_TXT');
    bind_lov('.cls_check_list_form .cls_lov_brand_other', '/api/lov/brand', 'data.DISPLAY_TXT', '.cls_input_brand_other');
    bind_lov('.cls_check_list_form .cls_lov_container_type_other', '/api/lov/containerType', 'data.DISPLAY_TXT', '.cls_input_container_type_other');
    bind_lov('.cls_check_list_form .cls_lov_lid_type_other', '/api/lov/lidtype', 'data.DISPLAY_TXT', '.cls_input_lid_type_other');
     //--------------------------// by aof  packing style 01/20/2021- -------------------------------
    //bind_lov('.cls_check_list_form .cls_lov_packing_style_other', '/api/lov/2p', 'data.DISPLAY_TXT', '.cls_input_packing_style_other');
    bind_lov_param('.cls_check_list_form .cls_lov_packing_style_other', '/api/lov/2p_new', 'data.DISPLAY_TXT', ["PRIMARY_TYPE_ID"], ['.cls_check_list_form .cls_lov_primary_type_other'], '.cls_input_packing_style_other');
     //--------------------------// by aof  packing style 01/20/2021- -------------------------------
   
    bind_lov('.cls_check_list_form .cls_lov_pack_size_other', '/api/lov/packsize', 'data.DISPLAY_TXT', '.cls_input_pack_size_other');
    bind_lov('.cls_check_list_form .cls_lov_primary_size_other', '/api/lov/3p', 'data.DISPLAY_TXT', '.cls_input_primary_size_other');
    bind_lov('.cls_check_list_form .cls_lov_primary_type_other', '/api/lov/primarytype', 'data.DISPLAY_TXT', '.cls_input_primary_type_other');

    if (UserPosition == "FFC")
        bind_lov_param('.cls_lov_rd_person', '/api/lov/rdperson_ffc', 'data.DISPLAY_TXT', ["company_id"], ['.cls_lov_company']);
    else if (UserPosition == "T-HOLDING")
        bind_lov_param('.cls_lov_rd_person', '/api/lov/rdperson_tholding', 'data.DISPLAY_TXT', ["company_id"], ['.cls_lov_company']);
    else
        bind_lov_param('.cls_lov_rd_person', '/api/lov/rdperson', 'data.DISPLAY_TXT', ["company_id"], ['.cls_lov_company']);

    bind_lov('.cls_lov_brand_oem_other', '/api/lov/brandoem', 'data.DISPLAY_TXT', '.cls_input_brand_oem_other');
    bind_lov('.cls_lov_search_check_list', '/api/lov/listofchecklist', 'data.DISPLAY_TXT');

    //for modal search check list
    bind_lov('.cls_lov_chklist_search_soldto', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_chklist_search_shipto', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_chklist_search_brand', '/api/lov/brand', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_chklist_search_country', '/api/lov/country', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_chklist_search_req_creator', '/api/lov/checklistcreator', 'data.DISPLAY_TXT');

    $(document).on("click", ".cls_check_list_form .cls_img_lov_add_country_multiple", function () {
        var obj = $('.tr_country_multiple').clone();
        obj.show();
        obj.removeClass('tr_country_multiple');
        obj.find('.cls_lov_country_multiple').toggleClass('cls_lov_country_multiple cls_lov_country_multiple' + cnt_lov_country_multiple);
        obj.insertAfter($('.tr_country_multiple_static:last'));
        $('.cls_lov_country_multiple' + cnt_lov_country_multiple).attr("required", true);
        bind_lov('.cls_lov_country_multiple' + cnt_lov_country_multiple, '/api/lov/country', 'data.DISPLAY_TXT');

        if (cnt_lov_country_multiple > 1) {
            obj.find('.td_country_txt_multiple').text('');
            obj.find('.cls_img_lov_add_country_multiple').remove();
            obj.find('.cls_img_lov_delete_country_multiple').show();
        }
        else {
            obj.find('.cls_img_lov_delete_country_multiple').remove();
        }
        $(obj).find(".cls_img_lov_delete_country_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_country_multiple++;
    });
    $(".cls_img_lov_add_country_multiple").click();


     // by aof 04/11/2021 for Itail add Production Plant #start 
    $(document).on("click", ".cls_check_list_form .cls_img_lov_checklist_add_plant_multiple", function () {
        var obj = $('.tr_checklist_plant_multiple').clone();
        obj.show();
        obj.removeClass('tr_checklist_plant_multiple');
        obj.find('.cls_lov_checklist_plant_multiple').toggleClass('cls_lov_checklist_plant_multiple cls_lov_checklist_plant_multiple' + cnt_lov_checklist_plant_multiple);
        obj.insertAfter($('.tr_checklist_plant_multiple_static:last'));

         // aof commented
        //if (UserPosition != "FFC") {     
        //    $('.cls_lov_checklist_plant_multiple' + cnt_lov_checklist_plant_multiple).attr("required", true);   
        //}   
        // aof commented 
        bind_lov('.cls_lov_checklist_plant_multiple' + cnt_lov_checklist_plant_multiple, '/api/lov/plant', 'data.DISPLAY_TXT');

        if (cnt_lov_checklist_plant_multiple > 1) {
            obj.find('.td_checklist_plant_txt_multiple').text('');
            obj.find('.cls_img_lov_checklist_add_plant_multiple').remove();
            obj.find('.cls_img_lov_checklist_delete_plant_multiple').show();
         //   $('.cls_checklist_request_form .cls_display_txt_check_manufacturer').show();  // aof commented
        }
        $(obj).find(".cls_img_lov_checklist_delete_plant_multiple").click(function () {
            cnt_lov_checklist_plant_multiple--;
            // aof commented
            //if (cnt_lov_checklist_plant_multiple < 3) {
            //    $('.cls_checklist_request_form .cls_display_txt_check_manufacturer').hide();
            //}
            // aof commented
            $(this).closest('.row').remove();
            //getDataArtworkffc();   // aof commented
        });
        $('.cls_lov_artwork_checklist_multiple' + cnt_lov_checklist_plant_multiple).change(function () {
            //check plant duplicate
            var arrayplantList = [];
            arrayplantList = getplantlist();
            //var in_array = plantList.filter(function (item) {
            //    return item.PRODUCTION_PLANT_ID == this.value // look for the item where ID is equal to value
            //});
            var in_array = arrayplantList.filter(x => x.PRODUCTION_PLANT_ID === this.value).length;
            //var text = in_array[0].length();
            if (in_array > 1) {
                setValueToDDL('.cls_lov_checklist_plant_multiple' + cnt_lov_checklist_plant_multiple, "");
                var obj = $('.cls_lov_checklist_plant_multiple_static:last');
                $(obj).empty();
                alertError2("Production plant is duplicate.");
                return false;
            } else
                setValueToDDL('.cls_lov_checklist_plant_multiple' + cnt_lov_checklist_plant_multiple, this.value);
            //setDataArtworkffc(this.value);
           // getDataArtworkffc(); // aof commented
        });
        cnt_lov_checklist_plant_multiple++;
    });
    $(".cls_img_lov_checklist_add_plant_multiple").click();
     // by aof 04/11/2021 for Itail add Production Plant #end 

    $(document).on("click", ".cls_check_list_form .cls_img_lov_add_mailto_multiple", function () {
        var obj = $('.tr_mailto_multiple').clone();
        obj.show();
        obj.removeClass('tr_mailto_multiple');
        obj.find('.cls_lov_mailto_multiple').toggleClass('cls_lov_mailto_multiple cls_lov_mailto_multiple' + cnt_lov_mailto_multiple);
        obj.insertAfter($('.tr_mailto_multiple_static:last'));
        $('.cls_lov_mailto_multiple' + cnt_lov_mailto_multiple).attr("required", true);

        if (UserPosition == "FFC")
            bind_lov('.cls_lov_mailto_multiple' + cnt_lov_mailto_multiple, '/api/checklist/mailtoccffc', 'data.DISPLAY_TXT');
        else if (UserPosition == "T-HOLDING")
            bind_lov('.cls_lov_mailto_multiple' + cnt_lov_mailto_multiple, '/api/checklist/mailtocctholding', 'data.DISPLAY_TXT');
        else
            //bind_lov_param('.cls_lov_mailto_multiple' + cnt_lov_mailto_multiple, '/api/checklist/mailtocustomer', 'data.USER_DISPLAY_TXT', ["SOLD_TO_ID", "SHIP_TO_ID", "CUSTOMER_OTHER_ID"], [".cls_check_list_form .cls_lov_sold_to", ".cls_check_list_form .cls_lov_ship_to", ".cls_check_list_form .cls_lov_other_to"]);
            bind_lov_param_filter('.cls_lov_mailto_multiple' + cnt_lov_mailto_multiple, '/api/checklist/mailtocustomer', 'data.USER_DISPLAY_TXT', ["SOLD_TO_ID", "SHIP_TO_ID", "CUSTOMER_OTHER_ID"], [".cls_check_list_form .cls_lov_sold_to", ".cls_check_list_form .cls_lov_ship_to", ".cls_check_list_form .cls_lov_other_to"], "FILTER_ID", [".cls_check_list_form .cls_lov_mailto_multiple_static", ".cls_check_list_form .cls_lov_mailcc_multiple_static"]);

        if (cnt_lov_mailto_multiple > 1) {
            obj.find('.td_mailto_txt_multiple').text('');
            obj.find('.cls_img_lov_add_mailto_multiple').remove();
            obj.find('.cls_img_lov_delete_mailto_multiple').show();
        }
        else {
            obj.find('.cls_img_lov_delete_mailto_multiple').remove();
        }
        $(obj).find(".cls_img_lov_delete_mailto_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_mailto_multiple++;
    });
    $(".cls_img_lov_add_mailto_multiple").click();
    $('.cls_check_list_form .cls_lov_mailto_multiple_static').prop("disabled", true);

    $(document).on("click", ".cls_check_list_form .cls_img_lov_add_mailcc_multiple", function () {
        var obj = $('.tr_mailcc_multiple').clone();
        obj.show();
        obj.removeClass('tr_mailcc_multiple');
        obj.find('.cls_lov_mailcc_multiple').toggleClass('cls_lov_mailcc_multiple cls_lov_mailcc_multiple' + cnt_lov_mailcc_multiple);
        obj.insertAfter($('.tr_mailcc_multiple_static:last'));
        //$('.cls_lov_mailcc_multiple' + cnt_lov_mailcc_multiple).attr("required", true);

        if (UserPosition == "FFC")
            bind_lov('.cls_lov_mailcc_multiple' + cnt_lov_mailcc_multiple, '/api/checklist/mailtoccffc', 'data.DISPLAY_TXT');
        else if (UserPosition == "T-HOLDING")
            bind_lov('.cls_lov_mailcc_multiple' + cnt_lov_mailcc_multiple, '/api/checklist/mailtocctholding', 'data.DISPLAY_TXT');
        else
            //bind_lov_param('.cls_lov_mailcc_multiple' + cnt_lov_mailcc_multiple, '/api/checklist/mailtocustomer', 'data.USER_DISPLAY_TXT', ["SOLD_TO_ID", "SHIP_TO_ID", "CUSTOMER_OTHER_ID"], [".cls_check_list_form .cls_lov_sold_to", ".cls_check_list_form .cls_lov_ship_to", ".cls_check_list_form .cls_lov_other_to"]);
            bind_lov_param_filter('.cls_lov_mailcc_multiple' + cnt_lov_mailcc_multiple, '/api/checklist/mailtocustomer', 'data.USER_DISPLAY_TXT', ["SOLD_TO_ID", "SHIP_TO_ID", "CUSTOMER_OTHER_ID"], [".cls_check_list_form .cls_lov_sold_to", ".cls_check_list_form .cls_lov_ship_to", ".cls_check_list_form .cls_lov_other_to"], "FILTER_ID", [".cls_check_list_form .cls_lov_mailto_multiple_static", ".cls_check_list_form .cls_lov_mailcc_multiple_static"]);

        if (cnt_lov_mailcc_multiple > 1) {
            obj.find('.td_mailcc_txt_multiple').text('');
            obj.find('.cls_img_lov_add_mailcc_multiple').remove();
            obj.find('.cls_img_lov_delete_mailcc_multiple').show();
        }
        else {
            obj.find('.cls_img_lov_delete_mailcc_multiple').remove();
        }
        $(obj).find(".cls_img_lov_delete_mailcc_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_mailcc_multiple++;
    });
    $(".cls_img_lov_add_mailcc_multiple").click();
    $('.cls_check_list_form .cls_lov_mailcc_multiple_static').prop("disabled", true);

    setProductCodeRefNo('product_code');
    $('input[type=radio][name=req_product_code_ref_no]').change(function () {
        setProductCodeRefNo(this.value);
        setRequiredFieldChecklist();
    });

    $('input[type=radio][name=rdo_request_for]').change(function () {
        if ($('input[name=rdo_request_for]:checked').val() == "2") {
            $("input[name=req_product_code_ref_no][value=ref_no]").prop('checked', true);
            setProductCodeRefNo('ref_no');

            $('.cls_check_list_form .cls_customer_email').hide();

            resetMailToCcChecklist(false);
        }
        else {
            $('.cls_check_list_form .cls_customer_email').show();

            $("input[name=req_product_code_ref_no][value=product_code]").prop('checked', true);
            setProductCodeRefNo('product_code');
        }
        setRequiredFieldChecklist();
    });

    $('.cls_check_list_form .cls_lov_sold_to').change(function () {
        if (UserPosition == "FFC") {
            resetMailToCcChecklist(true);
        } else {
            resetMailToCcChecklist(false);
        }
    });
    $('.cls_check_list_form .cls_lov_ship_to').change(function () {
        if (UserPosition == "FFC") {
            resetMailToCcChecklist(true);
        } else {
            resetMailToCcChecklist(false);
        }
    });
    $('.cls_check_list_form .cls_lov_other_to').change(function () {
        if (UserPosition == "FFC") {
            resetMailToCcChecklist(true);
        } else {
            resetMailToCcChecklist(false);
        }
    });

    $(".cls_check_list_form .cls_btn_search_chklist").click(function (e) {
        $('#modal_chklist_search').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $("#modal_chklist_search .cls_btn_chklist_search").click(function (e) {
        var soldto = $("#modal_chklist_search .cls_lov_chklist_search_soldto").val();
        var shipto = $("#modal_chklist_search .cls_lov_chklist_search_shipto").val();
        var brand = $("#modal_chklist_search .cls_lov_chklist_search_brand").val();
        var country = $("#modal_chklist_search .cls_lov_chklist_search_country").val();
        var creator = $("#modal_chklist_search .cls_lov_chklist_search_req_creator").val();
        if (soldto == null && shipto == null && brand == null && country == null && creator == null) {
            alertError2("Please select at least 1 criteria.");
        } else {
            bindSearchRequesForm(soldto, shipto, brand, country, creator);
        }
    });

    $("#modal_chklist_search .cls_btn_chklist_clear").click(function (e) {
        $("#modal_chklist_search .cls_lov_chklist_search_soldto").val('').trigger("change");
        $("#modal_chklist_search .cls_lov_chklist_search_shipto").val('').trigger("change");
        $("#modal_chklist_search .cls_lov_chklist_search_brand").val('').trigger("change");
        $("#modal_chklist_search .cls_lov_chklist_search_country").val('').trigger("change");
        $("#modal_chklist_search .cls_lov_chklist_search_req_creator").val('').trigger("change");
    });

    $('#table_chklist_search').DataTable();
    $("#modal_chklist_search .cls_btn_chklist_select").click(function (e) {
        var table = $('#table_chklist_search').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            setValueToDDL('.cls_check_list_form .cls_lov_search_check_list', tblData[0].ID, tblData[0].REQUEST_FORM_NO);
            $("#modal_chklist_search .cls_btn_chklist_close").click();
        }
    });

    $('#modal_chklist_submit_success').on('shown.bs.modal', function (e) {

    });
    $('#modal_chklist_submit_success').on('hidden.bs.modal', function (e) {
        tohomepage();
    });

});

var click_ref = "";
var click_ref_temp_check_list_id = "";
var click_ref_temp_check_list_txt = "";
$(document).ready(function () {
    var cnt = 0;
    $(".cls_check_list_form .cls_btn_add_line_item").click(function () {

        var obj = $('.tr_mockup_first_line_item').clone().removeClass('tr_mockup_first_line_item').removeClass('cls_cn_hide');

        cnt++;
        $('.table_mockup_line_item tbody').append(obj);
        var obj2 = $('.table_mockup_line_item tbody tr:last');

        var ddlPackagingType = 'cls_lov_packaging_type';
        obj2.find('.' + ddlPackagingType).toggleClass(ddlPackagingType + ' ' + ddlPackagingType + cnt);
        bind_lov('.' + ddlPackagingType + cnt, '/api/lov/packtype', 'data.DISPLAY_TXT');
        $('.' + ddlPackagingType + cnt).attr("required", true);
        $('.' + ddlPackagingType + cnt).on("change", function () {
            $('.cls_lov_style' + cnt).val('').trigger("change");
            $('.cls_lov_pim_color' + cnt).val('').trigger("change");
        });

        var ddl = 'cls_lov_style';
        var input = 'cls_input_style_other';
        obj.find('.' + ddl).toggleClass(ddl + ' ' + ddl + cnt);
        obj.find('.' + input).toggleClass(input + ' ' + input + cnt);
        bind_lov_param('.' + ddl + cnt, '/api/lov/style', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.' + ddlPackagingType + cnt], '.' + input + cnt);
        $('.' + ddl + cnt).attr("required", true);

        ddl = 'cls_lov_pim';
        input = 'cls_input_pim_other';
        obj.find('.' + ddl).toggleClass(ddl + ' ' + ddl + cnt);
        obj.find('.' + input).toggleClass(input + ' ' + input + cnt);
        bind_lov('.' + ddl + cnt, '/api/lov/printsystem', 'data.DISPLAY_TXT', '.' + input + cnt);

        ddl = 'cls_lov_pim_color';
        input = 'cls_input_pim_color_other';
        obj.find('.' + ddl).toggleClass(ddl + ' ' + ddl + cnt);
        obj.find('.' + input).toggleClass(input + ' ' + input + cnt);
        bind_lov_param('.' + ddl + cnt, '/api/lov/numberofcolor', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.' + ddlPackagingType + cnt], '.' + input + cnt);

        ddl = 'cls_lov_box_color';
        input = 'cls_input_box_color_other';
        obj.find('.' + ddl).toggleClass(ddl + ' ' + ddl + cnt);
        obj.find('.' + input).toggleClass(input + ' ' + input + cnt);
        bind_lov('.' + ddl + cnt, '/api/lov/boxcolor', 'data.DISPLAY_TXT', '.' + input + cnt);

        ddl = 'cls_lov_coating';
        input = 'cls_input_coating_other';
        obj.find('.' + ddl).toggleClass(ddl + ' ' + ddl + cnt);
        obj.find('.' + input).toggleClass(input + ' ' + input + cnt);
        bind_lov('.' + ddl + cnt, '/api/lov/coating', 'data.DISPLAY_TXT', '.' + input + cnt);

        var i = 0;
        $('.table_mockup_line_item > tbody  > tr').each(function () {
            $(this).find('.cls_td_running_no').text(i + '.');
            i++;
        });

        $(".cls_img_delete_line_item").click(function () {
            $(this).closest('tr').remove();
            var i = 0;
            $('.table_mockup_line_item > tbody  > tr').each(function () {
                $(this).find('.cls_td_running_no').text(i + '.');
                i++;
            });
        });

        setRequiredFieldChecklist();
        //setRequiredFieldForNotFFC();
    });

    $(".cls_check_list_form .cls_btn_add_product_code").click(function () {
        var productCodes = $(".cls_input_product_code").val().split(/\n/);
        var strAlert = "";
        var jsonObj = new Object();
        jsonObj.data = [];
        for (var i = 0; i < productCodes.length; i++) {
            productCodes[i] = productCodes[i].trim();
            if (productCodes[i] != "") {
                var isDup = false;
                $('.table_product_code_item > tbody > tr').each(function () {
                    if (productCodes[i].toLowerCase() == $(this).find('.cls_product_code').val().toLowerCase()) {
                        isDup = true;
                        strAlert = strAlert + productCodes[i] + " was already" + "<br>";
                        return false;
                    }
                });

                if (!isDup) {
                    var item = {};
                    item["PRODUCT_CODE"] = productCodes[i];
                    jsonObj.data.push(item);
                }
            }
        }

        if (jsonObj.data.length > 0) {
            getCheckListProductCode(jsonObj);
            $(".cls_input_product_code").val('');
        }

        if (strAlert.length > 0) {
            alertError2(strAlert);
        }
    });

    $(".cls_check_list_form .cls_btn_add_reference_no").click(function () {

        $('.cls_lbl_netweight_rd:visible').html("Net weight");

        var ref_no = $(".cls_input_reference_no").val().split(/\n/);
        var strAlert = "";
        var jsonObj = new Object();
        jsonObj.data = [];
        for (var i = 0; i < ref_no.length; i++) {
            ref_no[i] = ref_no[i].trim();
            if (ref_no[i] != "") {
                var isDup = false;
                $('.table_ref_no_item > tbody > tr').each(function () {
                    if (ref_no[i].toLowerCase() == $(this).find('.cls_ref_no').val().toLowerCase()) {
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

        if (jsonObj.data.length > 0) {
            var lbl_spanStar = "<span style=\"color: red;\">*</span>";
            $('.cls_lbl_netweight_rd:visible').html("Net weight" + lbl_spanStar);

            bindCheckListRefNo(jsonObj.data);
            //getCheckListRefNo(jsonObj);
            $(".cls_input_reference_no").val('');
        }

        if (strAlert.length > 0) {
            alertError2(strAlert);
        }
    });

    $(".cls_input_product_code").on('input focus keydown keyup', function () {
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

    //$(".cls_input_reference_no").on('input focus keydown keyup', function () {
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

    if (CheckListID == 0) {
        $(".cls_check_list_form .cls_btn_add_line_item").click();
    }

    $('.cls_check_list_form .cls_btn_copy').click(function () {
        if (isEmpty($('.cls_lov_search_check_list').val())) {
            alertError2("Please select Copy/Reference check list.");
        }
        else {
            click_ref = "No";
            click_ref_temp_check_list_id = '';
            click_ref_temp_check_list_txt = '';
            $('.cls_btn_ref').removeClass('btn-primary').addClass('btn-default');
            bindDataCheckListByCheckListNo($('.cls_lov_search_check_list').val(), 'copy');
        }
    })

    $('.cls_check_list_form .cls_btn_ref').click(function () {
        if (isEmpty($('.cls_lov_search_check_list').val())) {
            alertError2("Please select Copy/Reference check list.");
        }
        else {
            click_ref = "Yes";
            click_ref_temp_check_list_id = $('.cls_lov_search_check_list').val();
            click_ref_temp_check_list_txt = $('.cls_lov_search_check_list option:selected').text();
            $('.cls_btn_ref').removeClass('btn-default').addClass('btn-primary');
            bindDataCheckListByCheckListNo($('.cls_lov_search_check_list').val(), 'ref');
        }
    })

    $('.cls_check_list_form .cls_lov_primary_size_other').on("change", function () {
        get3PbyID($('.cls_lov_primary_size_other').val());
    });

    $('.cls_check_list_form .cls_lov_packing_style_other').on("change", function () {
        get2PbyID($('.cls_lov_packing_style_other').val());
    });

    setSpecialReq(false);
    $('.cls_check_list_form .cls_chk_special_req').click(function () {
        setSpecialReq($(this).prop('checked'));
    });

    $('.cls_check_list_form .cls_btn_save').click(function () {
        saveDataCheckList(true);
    })

    $('.cls_check_list_form .cls_btn_delete').click(function () {
        deleteDataCheckList(CheckListID);
    })

    $(".cls_check_list_form form").submit(function (e) {
        if ($(this).valid()) {
            saveDataCheckList(false);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    //setRequiredFieldForNotFFC();

    if (CheckListID == 0) {
        $('.cls_check_list_form').show();
        if (UserPosition == "FFC") {
            if (!isEmpty(FFCDefaultReviewer_UserID))
                setValueToDDL('.cls_check_list_form .cls_lov_reviewer', FFCDefaultReviewer_UserID, FFCDefaultReviewer_UserName);
            if (!isEmpty(FFCDefaultEmailTo_UserID))
                setValueToDDL('.cls_check_list_form .cls_lov_mailto_multiple_static:visible', FFCDefaultEmailTo_UserID, FFCDefaultEmailTo_UserName);
            if (!isEmpty(FFCDefaultEmailCC_UserID))
                setValueToDDL('.cls_check_list_form .cls_lov_mailcc_multiple_static:visible', FFCDefaultEmailCC_UserID, FFCDefaultEmailCC_UserName);
            $('.cls_check_list_form .cls_lov_mailto_multiple_static:visible').prop("disabled", true);
        }

        //if (UserPosition == "PK") {
        //    $('.cls_check_list_form input[name=rdo_request_for]').attr("disabled", true);
        //}

        showBtn(CheckListID, "");

        if (UserPosition == "FFC") {
            $(".cls_check_list_form input[name=req_product_code_ref_no][value=ref_no]").prop('checked', true).change();
            $('.cls_check_list_form input[name=req_product_code_ref_no]').attr("disabled", true);

            $('.cls_check_list_form input[name=rdo_request_for]').attr("disabled", true);

            $('.cls_check_list_form .cls_lbl_rd_person:visible').html("RD person :");
            $('.cls_check_list_form .cls_lov_rd_person:visible').attr("required", false);

            $('.cls_check_list_form .cls_lbl_primary_type:visible').html("Primary type :");
            $('.cls_check_list_form .cls_lov_primary_type_other:visible').attr("required", false);

            $('.cls_check_list_form .cls_lbl_primary_size:visible').html("Primary size :");
            $('.cls_check_list_form .cls_lov_primary_size_other:visible').attr("required", false);

            $('.cls_check_list_form .cls_lbl_container_type:visible').html("Container type :");
            $('.cls_check_list_form .cls_lov_container_type_other:visible').attr("required", false);

            $('.cls_check_list_form .cls_lbl_lid_type:visible').html("Lid type :");
            $('.cls_check_list_form .cls_lov_lid_type_other:visible').attr("required", false);

            $('.cls_check_list_form .cls_lbl_packing_style:visible').html("Packing style :");
            $('.cls_check_list_form .cls_lov_packing_style_other:visible').attr("required", false);

            $('.cls_check_list_form .cls_lbl_pack_size:visible').html("Pack size :");
            $('.cls_check_list_form .cls_lov_pack_size_other:visible').attr("required", false);
        }
    }
    else {
        if ($('.cls_check_list_form').length == 1)
            bindDataCheckList(CheckListID);
    }
});

function get3PbyID(ID) {
    if (!isEmpty(ID) && ID != -1) {
        var myurl = '/api/lov/3p?data.three_p_id=' + ID;
        var mytype = 'GET';
        var mydata = null;
        myAjax(myurl, mytype, mydata, callback_get3PbyID);
    }
    else {
        setValueToDDL('.cls_check_list_form .cls_lov_container_type_other', '', '');
        setValueToDDL('.cls_check_list_form .cls_lov_lid_type_other', '', '');

        $('.cls_check_list_form .cls_lov_container_type_other').prop("disabled", false);
        $('.cls_check_list_form .cls_lov_lid_type_other').prop("disabled", false);
    }
}
function callback_get3PbyID(res) {
    $('.cls_check_list_form .cls_lov_container_type_other').prop("disabled", true);
    $('.cls_check_list_form .cls_lov_lid_type_other').prop("disabled", true);
    setValueToDDL('.cls_check_list_form .cls_lov_container_type_other', -99, res.data[0].CONTAINER_TYPE_DESCRIPTION);
    setValueToDDL('.cls_check_list_form .cls_lov_lid_type_other', -99, res.data[0].LID_TYPE_DESCRIPTION);

    $('.cls_check_list_form .cls_input_container_type_other').hide();
    $('.cls_check_list_form .cls_input_primary_size_other').hide();
    $('.cls_check_list_form .cls_input_lid_type_other').hide();
}

function get2PbyID(ID) {
    if (!isEmpty(ID) && ID != -1) {
        var myurl = '/api/lov/2p?data.two_p_id=' + ID;
        var mytype = 'GET';
        var mydata = null;
        myAjax(myurl, mytype, mydata, callback_get2PbyID);
    }
    else {
        setValueToDDL('.cls_check_list_form .cls_lov_pack_size_other', '', '');
        $('.cls_check_list_form .cls_lov_pack_size_other ').prop("disabled", false);
    }
}
function callback_get2PbyID(res) {
    $('.cls_check_list_form .cls_lov_pack_size_other').prop("disabled", true);

    setValueToDDL('.cls_check_list_form .cls_lov_pack_size_other', -99, res.data[0].PACK_SIZE_DESCRIPTION);

    $('.cls_check_list_form .cls_input_packing_style_other').hide();
    $('.cls_check_list_form .cls_input_pack_size_other ').hide();

}


function setSpecialReq(isShow) {
    if (isShow) {
        $('.cls_row_special_req').show();
    }
    else {
        $('.cls_row_special_req').hide();
    }
}

function bindDataCheckList(CheckListID) {
    cnt_lov_country_multiple = 1;
    cnt_lov_plant_multiple = 1;
    var myurl = '/api/checklist/request?data.check_list_id=' + CheckListID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_checklistrequest);
}

function bindDataCheckListByCheckListNo(CheckListNo, typeOf) {
    $('.cls_check_list_form .cls_hid_copy_reference').val(typeOf);
    var paramRef = "";
    if (typeOf == 'ref') {
        paramRef = "&data.IS_REFERENCE=X";
    }

    if (CheckListNo.indexOf('CL-') >= 0) {
        var myurl = '/api/checklist/request?data.check_list_no=' + CheckListNo + paramRef;
        var mytype = 'GET';
        var mydata = null;
        myAjax(myurl, mytype, mydata, callback_get_checklistrequest);
    }
    else {
        var myurl = '/api/artwork/artworkrequest?data.ARTWORK_REQUEST_NO=' + CheckListNo + paramRef;
        var mytype = 'GET';
        var mydata = null;
        myAjax(myurl, mytype, mydata, callback_get_artworkrequest);
    }
}

function deleteDataCheckList(CheckListID) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["CHECK_LIST_ID"] = CheckListID;
    jsonObj.data = item;

    var myurl = '/api/checklist/request';
    var mytype = 'DELETE';
    var mydata = jsonObj;
    myAjaxConfirmDelete(myurl, mytype, mydata, tohomepage);
}

function showBtn(CheckListID, CHECK_LIST_NO) {
    if (isEmpty(CHECK_LIST_NO)) {
        $('.cls_check_list_form .cls_btn_save').show();
        $('.cls_check_list_form .cls_btn_submit').show();
        $('.cls_check_list_form .cls_btn_add_line_item').show();

        //by aof 04 / 11 / 2021 for Itail add Production Plant #start
        $('.cls_check_list_form .cls_img_lov_checklist_add_plant_multiple').hide();
        $('.cls_check_list_form .cls_img_lov_checklist_delete_plant_multiple').hide();
        //by aof 04 / 11 / 2021 for Itail add Production Plant #last

        if (CheckListID > 0)
            $('.cls_check_list_form .cls_btn_delete').show();
    }
    else {
        //$('.cls_check_list_form .cls_row_copy_ref1').hide();
        //$('.cls_check_list_form .cls_row_copy_ref2').hide();

        $('.cls_check_list_form .cls_img_delete_product_code_item').hide();
        $('.cls_check_list_form .cls_img_delete_ref_no_item').hide();
        $('.cls_check_list_form .cls_img_delete_line_item').hide();
        //$('.cls_check_list_form .cls_img_lov_add_plant_multiple').hide();
        $('.cls_check_list_form .cls_img_lov_delete_plant_multiple').hide();
        $('.cls_check_list_form .cls_btn_add_product_code').hide();
        $('.cls_check_list_form .cls_btn_add_reference_no').hide();
        $('.cls_check_list_form .cls_img_lov_add_country_multiple').hide();
        $('.cls_check_list_form .cls_img_lov_delete_country_multiple').hide();
        $('.cls_check_list_form .cls_img_lov_add_mailto_multiple').hide();
        $('.cls_check_list_form .cls_img_lov_delete_mailto_multiple').hide();
        $('.cls_check_list_form .cls_img_lov_add_mailcc_multiple').hide();
        $('.cls_check_list_form .cls_img_lov_delete_mailcc_multiple').hide();

        //by aof 04 / 11 / 2021 for Itail add Production Plant #start
        $('.cls_check_list_form .cls_img_lov_checklist_add_plant_multiple').hide();
        $('.cls_check_list_form .cls_img_lov_checklist_delete_plant_multiple').hide();
        //by aof 04 / 11 / 2021 for Itail add Production Plant #last

        $('.cls_check_list_form').find('input:visible, textarea:visible, select:visible').attr('disabled', true);
    }
}

var REQUEST_PHYSICAL_MOCKUP;
function callback_get_checklistrequest(res) {
    $('.cls_check_list_form').show();
    if (res.data[0].CHECK_LIST_ID == 0) {

    }
    else {
        $('.cls_check_list_form .cls_check_list_no').val(res.data[0].CHECK_LIST_NO);
    }

    if (!inTaskForm && !isEmpty(res.data[0].CHECK_LIST_NO)) {
        if (click_ref == '') {
            $('.cls_check_list_form .cls_row_copy_ref1 .cls_btn_search_chklist ').hide();
            $('.cls_check_list_form .cls_row_copy_ref2').hide();
        }
    }

    if (!isEmpty(res.data[0].REQUEST_PHYSICAL_MOCKUP)) {
        REQUEST_PHYSICAL_MOCKUP = res.data[0].REQUEST_PHYSICAL_MOCKUP;
        if (res.data[0].REQUEST_PHYSICAL_MOCKUP == "1") {
            $("input[name=req_physical_mock_up][value=1]").prop('checked', true);
            $(".cls_pg_submit_modal_to_customer input[name=send_to_customer_approve][value=0]").prop('checked', true);
        }
        else if (res.data[0].REQUEST_PHYSICAL_MOCKUP == "0") {
            $("input[name=req_physical_mock_up][value=0]").prop('checked', true);
            $(".cls_pg_submit_modal_to_customer input[name=send_to_customer_approve][value=1]").prop('checked', true);
        }
    }

    if (!isEmpty(res.data[0].CHECK_LIST_FOR_DESIGN) && !isEmpty(res.data[0].REQUEST_FOR_DIE_LINE)) {
        if (res.data[0].CHECK_LIST_FOR_DESIGN == "0" && res.data[0].REQUEST_FOR_DIE_LINE == "0") {
            $("input[name=rdo_request_for][value=1]").prop('checked', true);
        }
        else if (res.data[0].CHECK_LIST_FOR_DESIGN == "1" && res.data[0].REQUEST_FOR_DIE_LINE == "0") {
            $("input[name=rdo_request_for][value=2]").prop('checked', true);
        }
        else if (res.data[0].CHECK_LIST_FOR_DESIGN == "0" && res.data[0].REQUEST_FOR_DIE_LINE == "1") {
            $("input[name=rdo_request_for][value=3]").prop('checked', true);
        }
    }

    if (res.data[0].IN_TRANSIT_TO != null)
        $('.cls_check_list_form .cls_input_checklist_in_tran').val(res.data[0].IN_TRANSIT_TO);
    if (res.data[0].VIA != null)
        $('.cls_check_list_form .cls_input_checklist_via').val(res.data[0].VIA);

    $('.cls_check_list_form .cls_input_trf_ref_no').val(res.data[0].TRF_REF_NO);
    $('.cls_check_list_form .cls_input_project_name').val(res.data[0].PROJECT_NAME);
    setValueToDDL('.cls_check_list_form .cls_lov_type_of_product', res.data[0].TYPE_OF_PRODUCT_ID, res.data[0].TYPE_OF_PRODUCT_DISPLAY_TXT);
    if ($('.cls_check_list_form .cls_hid_copy_reference').val().length == 0) {
        $('.cls_check_list_form .cls_creator_name').val(res.data[0].CREATOR_NAME);
    }
    if (UserPosition != "FFC") setValueToDDL('.cls_check_list_form .cls_lov_reviewer', res.data[0].REVIEWER, res.data[0].REVIEWER_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_company', res.data[0].COMPANY_ID, res.data[0].COMPANY_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_sold_to', res.data[0].SOLD_TO_ID, res.data[0].SOLD_TO_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_ship_to', res.data[0].SHIP_TO_ID, res.data[0].SHIP_TO_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_other_to', res.data[0].CUSTOMER_OTHER_ID, res.data[0].CUSTOMER_OTHER_DISPLAY_TXT);

    if (UserPosition != "FFC") {
        $('.cls_check_list_form .cls_lov_mailto_multiple_static').prop("disabled", false);
        $('.cls_check_list_form .cls_lov_mailcc_multiple_static').prop("disabled", false);
    }

    if (UserPosition != "FFC") {
        $('.tr_mailto_multiple_static:visible').remove();
        $('.tr_mailcc_multiple_static:visible').remove();
    }

    cnt_lov_mailto_multiple = 1;
    cnt_lov_mailcc_multiple = 1;
    if (UserPosition != "FFC") {
        var obj = $('.cls_lov_mailto_multiple_static:first');
        obj.closest('.tr_mailto_multiple_static').find(".cls_img_lov_add_mailto_multiple").click();

        var obj = $('.cls_lov_mailcc_multiple_static:first');
        obj.closest('.tr_mailcc_multiple_static').find(".cls_img_lov_add_mailcc_multiple").click();
    }
    $.each(res.data[0].MAIL_TO_CUSTOMER, function (index, item) {
        if (item.MAIL_TO.toLowerCase() == "x") {
            if (UserPosition != "FFC") {
                if (cnt_lov_mailto_multiple > 2) {
                    var obj = $('.cls_lov_mailto_multiple_static:first');
                    obj.closest('.tr_mailto_multiple_static').find(".cls_img_lov_add_mailto_multiple").click();
                }
                else {
                    cnt_lov_mailto_multiple++;
                }
                var obj = $('.cls_lov_mailto_multiple_static:last');
                $(obj).empty();
                $(obj).append('<option value="' + item.CUSTOMER_USER_ID + '">' + item.USER_DISPLAY_TXT + '</option>');
                $(obj).val(item.CUSTOMER_USER_ID);
            }
        }
        else if (item.MAIL_CC.toLowerCase() == "x") {
            if (UserPosition != "FFC") {
                if (cnt_lov_mailcc_multiple > 2) {
                    var obj = $('.cls_lov_mailcc_multiple_static:first');
                    obj.closest('.tr_mailcc_multiple_static').find(".cls_img_lov_add_mailcc_multiple").click();
                }
                else {
                    cnt_lov_mailcc_multiple++;
                }
                var obj = $('.cls_lov_mailcc_multiple_static:last');
                $(obj).empty();
                $(obj).append('<option value="' + item.CUSTOMER_USER_ID + '">' + item.USER_DISPLAY_TXT + '</option>');
                $(obj).val(item.CUSTOMER_USER_ID);
            }
        }
    });

    $('.tr_country_multiple_static:visible').remove();
    cnt_lov_country_multiple = 1;
    if (res.data[0].COUNTRY.length == 0) {
        var obj = $('.cls_lov_country_multiple_static:first');
        obj.closest('.tr_country_multiple_static').find(".cls_img_lov_add_country_multiple").click();
    }
    else {
        $.each(res.data[0].COUNTRY, function (index, item) {
            var obj = $('.cls_lov_country_multiple_static:first');
            obj.closest('.tr_country_multiple_static').find(".cls_img_lov_add_country_multiple").click();

            var obj = $('.cls_lov_country_multiple_static:last');
            $(obj).empty();
            $(obj).append('<option value="' + item.COUNTRY_ID + '">' + item.COUNTRY_DISPLAY_TXT + '</option>');
            $(obj).val(item.COUNTRY_ID);
        });
    }

    //by aof 04/11/2021 for Itail add Production Plant #start
    $('.tr_checklist_plant_multiple_static:visible').remove();
    cnt_lov_checklist_plant_multiple = 1;
    if (res.data[0].PLANT.length == 0) {
        var obj = $('.cls_lov_checklist_plant_multiple_static:first');
        obj.closest('.tr_checklist_plant_multiple_static').find(".cls_img_lov_checklist_add_plant_multiple").click();
    }
    else {
        $.each(res.data[0].PLANT, function (index, item) {
            var obj = $('.tr_checklist_plant_multiple_static:first');
            obj.closest('.tr_checklist_plant_multiple_static').find(".cls_img_lov_checklist_add_plant_multiple").click();

            var obj = $('.cls_lov_checklist_plant_multiple_static:last');
            $(obj).empty();
            $(obj).append('<option value="' + item.PRODUCTION_PLANT_ID + '">' + item.PLANT_DISPLAY_TXT + '</option>');
            $(obj).val(item.PRODUCTION_PLANT_ID);
        });
    }
     //by aof 04/11/2021 for Itail add Production Plant #end


    $('.cls_check_list_form .cls_input_ref_product_code').val(res.data[0].REF_PRODUCT_CODE);
    setValueToDDL('.cls_check_list_form .cls_lov_rd_person', res.data[0].RD_PERSON_ID, res.data[0].RD_PERSON_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_primary_type_other', res.data[0].PRIMARY_TYPE_ID, res.data[0].PRIMARY_TYPE_DISPLAY_TXT);
    setValueToDDLOther('.cls_check_list_form .cls_input_primary_type_other', res.data[0].PRIMARY_TYPE_OTHER);
    if (res.data[0].PRIMARY_TYPE_ID == -1) $('.cls_check_list_form .cls_input_primary_type_other').show();

    setValueToDDL('.cls_check_list_form .cls_lov_primary_size_other', res.data[0].THREE_P_ID, res.data[0].THREE_P_DISPLAY_TXT);
    setValueToDDLOther('.cls_check_list_form .cls_input_primary_size_other', res.data[0].PRIMARY_SIZE_OTHER);
    if (res.data[0].THREE_P_ID == -1) $('.cls_check_list_form .cls_input_primary_size_other').show();

    if ((res.data[0].CONTAINER_TYPE_ID == null) && (res.data[0].CONTAINER_TYPE_DISPLAY_TXT != null)) {
        setValueToDDL('.cls_check_list_form .cls_lov_container_type_other', -99, res.data[0].CONTAINER_TYPE_DISPLAY_TXT);
        $('.cls_check_list_form .cls_lov_container_type_other').prop("disabled", true);
    } else {
        setValueToDDL('.cls_check_list_form .cls_lov_container_type_other', res.data[0].CONTAINER_TYPE_ID, res.data[0].CONTAINER_TYPE_DISPLAY_TXT);
    }
    setValueToDDLOther('.cls_check_list_form .cls_input_container_type_other', res.data[0].CONTAINER_TYPE_OTHER);
    if (res.data[0].CONTAINER_TYPE_ID == -1) $('.cls_check_list_form .cls_input_container_type_other').show();


    if ((res.data[0].LID_TYPE_ID == null) && (res.data[0].LID_TYPE_DISPLAY_TXT != null)) {
        setValueToDDL('.cls_check_list_form .cls_lov_lid_type_other', -99, res.data[0].LID_TYPE_DISPLAY_TXT);
        $('.cls_check_list_form .cls_lov_lid_type_other').prop("disabled", true);
    } else {
        setValueToDDL('.cls_check_list_form .cls_lov_lid_type_other', res.data[0].LID_TYPE_ID, res.data[0].LID_TYPE_DISPLAY_TXT);
    }
    setValueToDDLOther('.cls_check_list_form .cls_input_lid_type_other', res.data[0].LID_TYPE_OTHER);
    if (res.data[0].LID_TYPE_ID == -1) $('.cls_check_list_form .cls_input_lid_type_other').show();

    setValueToDDL('.cls_check_list_form .cls_lov_packing_style_other', res.data[0].TWO_P_ID, res.data[0].TWO_P_DISPLAY_TXT);
    setValueToDDLOther('.cls_check_list_form .cls_input_packing_style_other', res.data[0].PACKING_STYLE_OTHER);
    if (res.data[0].TWO_P_ID == -1) $('.cls_check_list_form .cls_input_packing_style_other').show();


    if ((res.data[0].PACK_SIZE_ID == null) && (res.data[0].PACK_SIZE_DISPLAY_TXT != null)) {
        setValueToDDL('.cls_check_list_form .cls_lov_pack_size_other', -99, res.data[0].PACK_SIZE_DISPLAY_TXT);
        $('.cls_check_list_form .cls_lov_pack_size_other').prop("disabled", true);
    } else {
        setValueToDDL('.cls_check_list_form .cls_lov_pack_size_other', res.data[0].PACK_SIZE_ID, res.data[0].PACK_SIZE_DISPLAY_TXT);
    }
    if (res.data[0].PACK_SIZE_ID == -1) $('.cls_check_list_form .cls_input_pack_size_other').show();


    setValueToDDLOther('.cls_check_list_form .cls_input_pack_size_other', res.data[0].PACK_SIZE_OTHER);
    setValueToDDL('.cls_check_list_form .cls_lov_brand_other', res.data[0].BRAND_ID, res.data[0].BRAND_DISPLAY_TXT);
    if (res.data[0].BRAND_ID == -1) $('.cls_check_list_form .cls_input_brand_other').show();

    if (!isEmpty(res.data[0].NOMINATED_CONTAINER_VENDOR) || !isEmpty(res.data[0].NOMINATED_SPEC)) {
        $('.cls_check_list_form .cls_chk_special_req').prop('checked', true);
        setSpecialReq(true);
    }
    $('.cls_check_list_form .cls_txt_nominated_container').val(res.data[0].NOMINATED_CONTAINER_VENDOR);
    $('.cls_check_list_form .cls_txt_nominated_spec').val(res.data[0].NOMINATED_SPEC);
    setValueToDDLOther('.cls_check_list_form .cls_input_brand_other', res.data[0].BRAND_OTHER);
    setValueToDDL('.cls_check_list_form .cls_lov_brand_oem_other', res.data[0].BRAND_OEM_ID, res.data[0].BRAND_OEM_DISPLAY_TXT);
    setValueToDDLOther('.cls_check_list_form .cls_input_brand_oem_other', res.data[0].BRAND_OEM_OTHER);
    $('.cls_check_list_form .cls_primary_material').val(res.data[0].PRIMARY_MATERIAL);
    if (!isEmpty(res.data[0].REQUEST_DELIVERY_DATE)) {
        //var date = new Date(res.data[0].REQUEST_DELIVERY_DATE);
        //var temp = pad(date.getDate(), 2) + '/' + pad((date.getMonth() + 1), 2) + '/' + date.getFullYear();
        //$('.cls_check_list_form .cls_rdd').val(temp);
        $('.cls_check_list_form .cls_rdd').val(myDateMoment(res.data[0].REQUEST_DELIVERY_DATE));
    }
    $('.cls_check_list_form .cls_other_requests').val(res.data[0].OTHER_REQUESTS);

    $('.table_mockup_line_item tbody tr:visible').remove();
    $.each(res.data[0].ITEM, function (index, item) {
        var do_work = false;
        if (inTaskForm) {
            if (item.MOCKUP_ID == MOCKUPID) {
                do_work = true;
            }
        }
        else {
            do_work = true;
        }

        if (do_work) {
            $(".cls_check_list_form .cls_btn_add_line_item").click();
            var obj2 = $('.table_mockup_line_item tbody tr:last');
            setValueToDDL(obj2.find('.cls_lov_packaging_type_static'), item.PACKING_TYPE_ID, item.PACKING_TYPE_DISPLAY_TXT);
            setValueToDDL(obj2.find('.cls_lov_pim_static'), item.PRINT_SYSTEM_ID, item.PRINT_SYSTEM_DISPLAY_TXT);
            setValueToDDLOther(obj2.find('.cls_input_pim_other_static'), item.PRINT_SYSTEM_OTHER);
            setValueToDDL(obj2.find('.cls_lov_pim_color_static'), item.NUMBER_OF_COLOR_ID, item.NUMBER_OF_COLOR_DISPLAY_TXT);
            setValueToDDLOther(obj2.find('.cls_input_pim_color_other_static'), item.NUMBER_OF_COLOR_OTHER);
            setValueToDDL(obj2.find('.cls_lov_box_color_static'), item.BOX_COLOR_ID, item.BOX_COLOR_DISPLAY_TXT);
            setValueToDDLOther(obj2.find('.cls_input_box_color_other_static'), item.BOX_COLOR_OTHER);
            setValueToDDL(obj2.find('.cls_lov_coating_static'), item.COATING_ID, item.COATING_DISPLAY_TXT);
            setValueToDDLOther(obj2.find('.cls_input_coating_other_static'), item.COATING_OTHER);
            obj2.find('.cls_purpose_of').val(item.PURPOSE_OF);
            setValueToDDL(obj2.find('.cls_lov_style_static'), item.STYLE_ID, item.STYLE_DISPLAY_TXT);
            setValueToDDLOther(obj2.find('.cls_input_style_other_static'), item.STYLE_OTHER);
            obj2.find('.cls_txt_remark').val(item.REMARK);
        }
    });

    var rdoProductCodeRefNo = 'product_code';
    $('.table_product_code_item tbody tr').not(':first').remove();
    if (res.data[0].PRODUCT.length > 0) {
        $("input[name=req_product_code_ref_no][value=product_code]").prop('checked', true);
        bindCheckListProductCode(res.data[0].PRODUCT);
        setProductCodeRefNo(rdoProductCodeRefNo);
    }

    $('.table_ref_no_item tbody tr').not(':first').remove();
    if (res.data[0].REFERENCE.length > 0) {
        $("input[name=req_product_code_ref_no][value=ref_no]").prop('checked', true);
        bindCheckListRefNo(res.data[0].REFERENCE);
        rdoProductCodeRefNo = 'ref_no';
        setProductCodeRefNo(rdoProductCodeRefNo);

        var lbl_spanStar = "<span style=\"color: red;\">*</span>";
        $('.cls_lbl_netweight_rd:visible').html("Net weight" + lbl_spanStar);
    }

    if (res.data[0].REFERENCE.length == 0 && res.data[0].PRODUCT.length == 0) {
        if ($('input[name=rdo_request_for]:checked').val() == "2") {
            $("input[name=req_product_code_ref_no][value=ref_no]").prop('checked', true);
            bindCheckListRefNo(res.data[0].REFERENCE);
            rdoProductCodeRefNo = 'ref_no';
            setProductCodeRefNo(rdoProductCodeRefNo);
        }
    }

    if (click_ref == "No") {
        $('.cls_btn_ref').removeClass('btn-primary').addClass('btn-default');
        $('.cls_check_list_form .cls_check_list_no').val('');
        setValueToDDL('.cls_check_list_form .cls_lov_search_check_list', click_ref_temp_check_list_id, click_ref_temp_check_list_txt);
    }
    else if (click_ref == "Yes") {
        $('.cls_btn_ref').removeClass('btn-default').addClass('btn-primary');
        $('.cls_check_list_form .cls_check_list_no').val('');
        setValueToDDL('.cls_check_list_form .cls_lov_search_check_list', click_ref_temp_check_list_id, click_ref_temp_check_list_txt);
    }
    else {
        if (isEmpty(res.data[0].REFERENCE_REQUEST_ID)) {
            $('.cls_btn_ref').removeClass('btn-primary').addClass('btn-default');
        }
        else {
            $('.cls_btn_ref').removeClass('btn-default').addClass('btn-primary');
            setValueToDDL('.cls_check_list_form .cls_lov_search_check_list', res.data[0].REFERENCE_REQUEST_NO, res.data[0].REFERENCE_REQUEST_NO);
        }
        showBtn(res.data[0].CHECK_LIST_ID, res.data[0].CHECK_LIST_NO);
    }
    click_ref_temp_check_list_id = '';
    click_ref_temp_check_list_txt = '';

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
        else {
            if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_BACK_MK") {

                $('.cls_check_list_form .cls_btn_submit_by_mk').text('Submit');

                $('.cls_check_list_form').find('input:visible, textarea:visible, select:visible').attr('disabled', false);
                $('.cls_check_list_form').find('.cls_lov_mailto_multiple').attr('disabled', false);
                $('.cls_check_list_form').find('.cls_lov_mailcc_multiple').attr('disabled', false);
                $('.cls_check_list_form').find('.cls_lov_country_multiple').attr('disabled', false);
                
                $('.table_product_code_item').find('input:visible').attr('disabled', true);
                //$('.table_product_code_item input').attr('disabled', true);
                //$('.table_ref_no_item input').attr('disabled', true);

                //$('.cls_check_list_form input[name="req_product_code_ref_no"]').attr('disabled', 'disabled');
                //$('.cls_check_list_form .cls_row_ref_no_header').hide();
                //$('.cls_check_list_form .cls_row_product_code_header').hide();

                $('.cls_check_list_form .cls_btn_add_product_code').show();
                $('.cls_check_list_form .cls_btn_add_reference_no').show();
                $('.cls_check_list_form .cls_img_delete_product_code_item').show();
                $('.cls_check_list_form .cls_img_delete_ref_no_item').show();

                $('.cls_check_list_form .cls_img_delete_line_item').hide();

                $('.cls_check_list_form .cls_creator_name').attr('disabled', 'disabled');
                $('.cls_check_list_form .cls_check_list_no').attr('disabled', 'disabled');
                //$('.cls_check_list_form input[name="req_physical_mock_up"]').attr('disabled', 'disabled');
                $('.cls_check_list_form input[name="rdo_request_for"]').attr('disabled', 'disabled');

                if (isEmpty(res.data[0].THREE_P_ID) || res.data[0].THREE_P_ID == -1) {

                }
                else {
                    $('.cls_check_list_form .cls_lov_container_type_other').attr('disabled', true);
                    $('.cls_check_list_form .cls_lov_lid_type_other').attr('disabled', true);
                }
                if (isEmpty(res.data[0].TWO_P_ID) || res.data[0].TWO_P_ID == -1) {

                }
                else {
                    $('.cls_check_list_form .cls_lov_pack_size_other').attr('disabled', true);
                }

                if (ReadOnly == "1") {
                    $('.cls_check_list_form .cls_img_lov_add_country_multiple').hide();
                    $('.cls_check_list_form .cls_img_lov_delete_country_multiple').hide();
                    $('.cls_check_list_form .cls_img_lov_add_mailto_multiple').hide();
                    $('.cls_check_list_form .cls_img_lov_delete_mailto_multiple').hide();
                    $('.cls_check_list_form .cls_img_lov_add_mailcc_multiple').hide();
                    $('.cls_check_list_form .cls_img_lov_delete_mailcc_multiple').hide();
                    $('.cls_check_list_form .cls_img_delete_product_code_item').hide();
                    $('.cls_check_list_form .cls_img_delete_ref_no_item').hide();

                    $('.cls_check_list_form .cls_img_lov_checklist_add_plant_multiple').hide();     //by aof 04/11/2021 for Itail add Production Plant
                    $('.cls_check_list_form .cls_img_lov_checklist_delete_plant_multiple').hide();  //by aof 04/11/2021 for Itail add Production Plant
                }
                else {
                    $('.cls_check_list_form .cls_img_lov_add_country_multiple').show();
                    $('.cls_check_list_form .cls_img_lov_delete_country_multiple').show();
                    $('.cls_check_list_form .cls_img_lov_add_mailto_multiple').show();
                    $('.cls_check_list_form .cls_img_lov_delete_mailto_multiple').show();
                    $('.cls_check_list_form .cls_img_lov_add_mailcc_multiple').show();
                    $('.cls_check_list_form .cls_img_lov_delete_mailcc_multiple').show();

                    $('.cls_check_list_form .cls_img_lov_checklist_add_plant_multiple').hide();     //by aof 04/11/2021 for Itail add Production Plant
                    $('.cls_check_list_form .cls_img_lov_checklist_delete_plant_multiple').hide();  //by aof 04/11/2021 for Itail add Production Plant
                }


                //#INC - 39521 by aof start.
                $('.cls_check_list_form .cls_btn_copy').hide(); //.attr('disabled', true);
                $('.cls_check_list_form .cls_btn_ref').hide(); //.attr('disabled', true);
                $('.cls_check_list_form .cls_btn_search_chklist').hide(); //.attr('disabled', true);
                $('.cls_check_list_form .cls_lov_search_check_list').attr('disabled', true);
                //#INC - 39521 by aof end.

            }
            else if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_MK_UPD_PACK_STYLE") {

                $('.cls_check_list_form .cls_row_copy_ref1 .cls_btn_search_chklist').hide();
                $('.cls_check_list_form .cls_row_copy_ref2').hide();

                $('.cls_check_list_form .cls_btn_submit_by_mk').text('Complete workflow');

                $('.cls_check_list_form .cls_lov_packing_style_other:visible').attr('disabled', false);
                $('.cls_check_list_form .cls_input_packing_style_other').attr('disabled', false);
                if (isEmpty(res.data[0].TWO_P_ID) || res.data[0].TWO_P_ID == -1) {
                    $('.cls_check_list_form .cls_lov_pack_size_other').attr('disabled', false);
                }
                $('.cls_check_list_form .cls_input_pack_size_other').attr('disabled', false);


            }
            else {
                $('.cls_check_list_form .cls_row_copy_ref1 .cls_btn_search_chklist').hide();
                $('.cls_check_list_form .cls_row_copy_ref2').hide();
                showBtn(res.data[0].CHECK_LIST_ID, res.data[0].CHECK_LIST_NO);
            }
        }
    }

    //if (!inTaskForm) {
    setRequiredFieldChecklist();
    //}
}

function callback_get_artworkrequest(res) {
    //if (res.data[0].CHECK_LIST_ID == 0) {

    //}
    //else {
    //    $('.cls_check_list_form .cls_check_list_no').val(res.data[0].CHECK_LIST_NO);
    //}

    //if (!isEmpty(res.data[0].REQUEST_PHYSICAL_MOCKUP)) {
    //    if (res.data[0].REQUEST_PHYSICAL_MOCKUP == "1") {
    //        $("input[name=req_physical_mock_up][value=1]").prop('checked', true);
    //    }
    //    else if (res.data[0].REQUEST_PHYSICAL_MOCKUP == "0") {
    //        $("input[name=req_physical_mock_up][value=0]").prop('checked', true);
    //    }
    //}
    $("input[name=req_physical_mock_up][value=0]").prop('checked', true);
    $("input[name=rdo_request_for][value=1]").prop('checked', true);
    //if (!isEmpty(res.data[0].CHECK_LIST_FOR_DESIGN) && !isEmpty(res.data[0].REQUEST_FOR_DIE_LINE)) {
    //    if (res.data[0].CHECK_LIST_FOR_DESIGN == "0" && res.data[0].REQUEST_FOR_DIE_LINE == "0") {
    //        $("input[name=rdo_request_for][value=1]").prop('checked', true);
    //    }
    //    else if (res.data[0].CHECK_LIST_FOR_DESIGN == "1" && res.data[0].REQUEST_FOR_DIE_LINE == "0") {
    //        $("input[name=rdo_request_for][value=2]").prop('checked', true);
    //    }
    //    else if (res.data[0].CHECK_LIST_FOR_DESIGN == "0" && res.data[0].REQUEST_FOR_DIE_LINE == "1") {
    //        $("input[name=rdo_request_for][value=3]").prop('checked', true);
    //    }
    //}

    //$('.cls_check_list_form .cls_input_trf_ref_no').val(res.data[0].TRF_REF_NO);
    $('.cls_check_list_form .cls_input_project_name').val(res.data[0].PROJECT_NAME);
    setValueToDDL('.cls_check_list_form .cls_lov_type_of_product', res.data[0].TYPE_OF_PRODUCT_ID, res.data[0].TYPE_OF_PRODUCT_DISPLAY_TXT);
    //if ($('.cls_check_list_form .cls_hid_copy_reference').val().length == 0) {
    //    $('.cls_check_list_form .cls_creator_name').val(res.data[0].CREATOR_NAME);
    //}
    if (UserPosition != "FFC") setValueToDDL('.cls_check_list_form .cls_lov_reviewer', res.data[0].REVIEWER, res.data[0].REVIEWER_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_company', res.data[0].COMPANY_ID, res.data[0].COMPANY_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_sold_to', res.data[0].SOLD_TO_ID, res.data[0].SOLD_TO_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_ship_to', res.data[0].SHIP_TO_ID, res.data[0].SHIP_TO_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_other_to', res.data[0].CUSTOMER_OTHER_ID, res.data[0].CUSTOMER_OTHER_DISPLAY_TXT);
    if (UserPosition != "FFC") {
        $('.cls_check_list_form .cls_lov_mailto_multiple_static').prop("disabled", false);
        $('.cls_check_list_form .cls_lov_mailcc_multiple_static').prop("disabled", false);
    }

    if (UserPosition != "FFC") {
        $('.tr_mailto_multiple_static:visible').remove();
        $('.tr_mailcc_multiple_static:visible').remove();
    }

    cnt_lov_mailto_multiple = 1;
    cnt_lov_mailcc_multiple = 1;
    if (UserPosition != "FFC") {
        var obj = $('.cls_lov_mailto_multiple_static:first');
        obj.closest('.tr_mailto_multiple_static').find(".cls_img_lov_add_mailto_multiple").click();

        var obj = $('.cls_lov_mailcc_multiple_static:first');
        obj.closest('.tr_mailcc_multiple_static').find(".cls_img_lov_add_mailcc_multiple").click();
    }
    $.each(res.data[0].MAIL_TO_CUSTOMER, function (index, item) {
        if (item.MAIL_TO.toLowerCase() == "x") {
            if (UserPosition != "FFC") {
                if (cnt_lov_mailto_multiple > 2) {
                    var obj = $('.cls_lov_mailto_multiple_static:first');
                    obj.closest('.tr_mailto_multiple_static').find(".cls_img_lov_add_mailto_multiple").click();
                }
                else {
                    cnt_lov_mailto_multiple++;
                }
                var obj = $('.cls_lov_mailto_multiple_static:last');
                $(obj).empty();
                $(obj).append('<option value="' + item.CUSTOMER_USER_ID + '">' + item.USER_DISPLAY_TXT + '</option>');
                $(obj).val(item.CUSTOMER_USER_ID);
            }
        }
        else if (item.MAIL_CC.toLowerCase() == "x") {
            if (UserPosition != "FFC") {
                if (cnt_lov_mailcc_multiple > 2) {
                    var obj = $('.cls_lov_mailcc_multiple_static:first');
                    obj.closest('.tr_mailcc_multiple_static').find(".cls_img_lov_add_mailcc_multiple").click();
                }
                else {
                    cnt_lov_mailcc_multiple++;
                }
                var obj = $('.cls_lov_mailcc_multiple_static:last');
                $(obj).empty();
                $(obj).append('<option value="' + item.CUSTOMER_USER_ID + '">' + item.USER_DISPLAY_TXT + '</option>');
                $(obj).val(item.CUSTOMER_USER_ID);
            }
        }
    });

    $('.tr_country_multiple_static:visible').remove();
    cnt_lov_country_multiple = 1;
    if (res.data[0].COUNTRY.length == 0) {
        var obj = $('.cls_lov_country_multiple_static:first');
        obj.closest('.tr_country_multiple_static').find(".cls_img_lov_add_country_multiple").click();
    }
    else {
        $.each(res.data[0].COUNTRY, function (index, item) {
            var obj = $('.cls_lov_country_multiple_static:first');
            obj.closest('.tr_country_multiple_static').find(".cls_img_lov_add_country_multiple").click();

            var obj = $('.cls_lov_country_multiple_static:last');
            $(obj).empty();
            $(obj).append('<option value="' + item.COUNTRY_ID + '">' + item.COUNTRY_DISPLAY_TXT + '</option>');
            $(obj).val(item.COUNTRY_ID);
        });
    }

    //by aof 04/11/2021 for Itail add Production Plant #start
    $('.tr_checklist_plant_multiple_static:visible').remove();
    cnt_lov_checklist_plant_multiple = 1;
    if (res.data[0].PRODUCTION_PLANT.length == 0) {
        var obj = $('.cls_lov_checklist_plant_multiple_static:first');
        obj.closest('.tr_checklist_plant_multiple_static').find(".cls_img_lov_checklist_add_plant_multiple").click();
    }
    else {
        $.each(res.data[0].PRODUCTION_PLANT, function (index, item) {
            var obj = $('.tr_checklist_plant_multiple_static:first');
            obj.closest('.tr_checklist_plant_multiple_static').find(".cls_img_lov_checklist_add_plant_multiple").click();

            var obj = $('.cls_lov_checklist_plant_multiple_static:last');
            $(obj).empty();
            $(obj).append('<option value="' + item.PRODUCTION_PLANT_ID + '">' + item.PRODUCTION_PLANT_DISPLAY_TXT + '</option>');
            $(obj).val(item.PRODUCTION_PLANT_ID);


        });
    }
     //by aof 04/11/2021 for Itail add Production Plant #end




    if (res.data[0].IN_TRANSIT_TO != null)
        $('.cls_check_list_form .cls_input_checklist_in_tran').val(res.data[0].IN_TRANSIT_TO);
    if (res.data[0].VIA != null)
        $('.cls_check_list_form .cls_input_checklist_via').val(res.data[0].VIA);

    $('.cls_check_list_form .cls_input_ref_product_code').val(res.data[0].REF_PRODUCT_CODE);
    setValueToDDL('.cls_check_list_form .cls_lov_rd_person', res.data[0].RD_PERSON_ID, res.data[0].RD_PERSON_DISPLAY_TXT);
    setValueToDDL('.cls_check_list_form .cls_lov_primary_type_other', res.data[0].PRIMARY_TYPE_ID, res.data[0].PRIMARY_TYPE_DISPLAY_TXT);
    setValueToDDLOther('.cls_check_list_form .cls_input_primary_type_other', res.data[0].PRIMARY_TYPE_OTHER);
    if (res.data[0].PRIMARY_TYPE_ID == -1) $('.cls_check_list_form .cls_input_primary_type_other').show();

    setValueToDDL('.cls_check_list_form .cls_lov_primary_size_other', res.data[0].THREE_P_ID, res.data[0].THREE_P_DISPLAY_TXT);
    setValueToDDLOther('.cls_check_list_form .cls_input_primary_size_other', res.data[0].PRIMARY_SIZE_OTHER);
    if (res.data[0].THREE_P_ID == -1) $('.cls_check_list_form .cls_input_primary_size_other').show();

    if ((res.data[0].CONTAINER_TYPE_ID == null) && (res.data[0].CONTAINER_TYPE_DISPLAY_TXT != null)) {
        setValueToDDL('.cls_check_list_form .cls_lov_container_type_other', -99, res.data[0].CONTAINER_TYPE_DISPLAY_TXT);
        $('.cls_check_list_form .cls_lov_container_type_other').prop("disabled", true);
    } else {
        setValueToDDL('.cls_check_list_form .cls_lov_container_type_other', res.data[0].CONTAINER_TYPE_ID, res.data[0].CONTAINER_TYPE_DISPLAY_TXT);
    }
    setValueToDDLOther('.cls_check_list_form .cls_input_container_type_other', res.data[0].CONTAINER_TYPE_OTHER);
    if (res.data[0].CONTAINER_TYPE_ID == -1) $('.cls_check_list_form .cls_input_container_type_other').show();


    if ((res.data[0].LID_TYPE_ID == null) && (res.data[0].LID_TYPE_DISPLAY_TXT != null)) {
        setValueToDDL('.cls_check_list_form .cls_lov_lid_type_other', -99, res.data[0].LID_TYPE_DISPLAY_TXT);
        $('.cls_check_list_form .cls_lov_lid_type_other').prop("disabled", true);
    } else {
        setValueToDDL('.cls_check_list_form .cls_lov_lid_type_other', res.data[0].LID_TYPE_ID, res.data[0].LID_TYPE_DISPLAY_TXT);
    }
    setValueToDDLOther('.cls_check_list_form .cls_input_lid_type_other', res.data[0].LID_TYPE_OTHER);
    if (res.data[0].LID_TYPE_ID == -1) $('.cls_check_list_form .cls_input_lid_type_other').show();


    setValueToDDL('.cls_check_list_form .cls_lov_packing_style_other', res.data[0].TWO_P_ID, res.data[0].TWO_P_DISPLAY_TXT);
    setValueToDDLOther('.cls_check_list_form .cls_input_packing_style_other', res.data[0].PACKING_STYLE_OTHER);
    if (res.data[0].TWO_P_ID == -1) $('.cls_check_list_form .cls_input_packing_style_other').show();

    if ((res.data[0].PACK_SIZE_ID == null) && (res.data[0].PACK_SIZE_DISPLAY_TXT != null)) {
        setValueToDDL('.cls_check_list_form .cls_lov_pack_size_other', -99, res.data[0].PACK_SIZE_DISPLAY_TXT);
        $('.cls_check_list_form .cls_lov_pack_size_other').prop("disabled", true);
    } else {
        setValueToDDL('.cls_check_list_form .cls_lov_pack_size_other', res.data[0].PACK_SIZE_ID, res.data[0].PACK_SIZE_DISPLAY_TXT);
    }
    if (res.data[0].PACK_SIZE_ID == -1) $('.cls_check_list_form .cls_input_pack_size_other').show();

    setValueToDDLOther('.cls_check_list_form .cls_input_pack_size_other', res.data[0].PACK_SIZE_OTHER);
    setValueToDDL('.cls_check_list_form .cls_lov_brand_other', res.data[0].BRAND_ID, res.data[0].BRAND_DISPLAY_TXT);
    if (!isEmpty(res.data[0].NOMINATED_CONTAINER_VENDOR) || !isEmpty(res.data[0].NOMINATED_SPEC)) {
        $('.cls_check_list_form .cls_chk_special_req').prop('checked', true);
        setSpecialReq(true);
    }
    if (res.data[0].BRAND_ID == -1) $('.cls_check_list_form .cls_input_brand_other').show();

    $('.cls_check_list_form .cls_txt_nominated_container').val(res.data[0].NOMINATED_CONTAINER_VENDOR);
    $('.cls_check_list_form .cls_txt_nominated_spec').val(res.data[0].NOMINATED_SPEC);
    setValueToDDLOther('.cls_check_list_form .cls_input_brand_other', res.data[0].BRAND_OTHER);
    setValueToDDL('.cls_check_list_form .cls_lov_brand_oem_other', res.data[0].BRAND_OEM_ID, res.data[0].BRAND_OEM_DISPLAY_TXT);
    setValueToDDLOther('.cls_check_list_form .cls_input_brand_oem_other', res.data[0].BRAND_OEM_OTHER);
    $('.cls_check_list_form .cls_primary_material').val(res.data[0].PRIMARY_MATERIAL);
    if (!isEmpty(res.data[0].REQUEST_DELIVERY_DATE)) {
        //var date = new Date(res.data[0].REQUEST_DELIVERY_DATE);
        //var temp = pad(date.getDate(), 2) + '/' + pad((date.getMonth() + 1), 2) + '/' + date.getFullYear();
        //$('.cls_check_list_form .cls_rdd').val(temp);
        $('.cls_check_list_form .cls_rdd').val(myDateMoment(res.data[0].REQUEST_DELIVERY_DATE));
    }
    $('.cls_check_list_form .cls_other_requests').val(res.data[0].OTHER_REQUEST);

    $('.table_mockup_line_item tbody tr:visible').remove();
    $(".cls_check_list_form .cls_btn_add_line_item").click();

    var rdoProductCodeRefNo = 'product_code';
    $('.table_product_code_item tbody tr:visible').remove();
    if (res.data[0].PRODUCT.length > 0) {
        $("input[name=req_product_code_ref_no][value=product_code]").prop('checked', true);
        bindCheckListProductCode(res.data[0].PRODUCT);
        setProductCodeRefNo(rdoProductCodeRefNo);
    }

    $('.table_ref_no_item tbody tr:visible').remove();
    if (res.data[0].REFERENCE.length > 0) {
        $("input[name=req_product_code_ref_no][value=ref_no]").prop('checked', true);
        bindCheckListRefNo(res.data[0].REFERENCE);
        rdoProductCodeRefNo = 'ref_no';
        setProductCodeRefNo(rdoProductCodeRefNo);

        var lbl_spanStar = "<span style=\"color: red;\">*</span>";
        $('.cls_lbl_netweight_rd:visible').html("Net weight" + lbl_spanStar);
    }

    if (click_ref == "No") {
        $('.cls_btn_ref').removeClass('btn-primary').addClass('btn-default');
        $('.cls_check_list_form .cls_check_list_no').val('');
        setValueToDDL('.cls_check_list_form .cls_lov_search_check_list', click_ref_temp_check_list_id, click_ref_temp_check_list_txt);
    }
    else if (click_ref == "Yes") {
        $('.cls_btn_ref').removeClass('btn-default').addClass('btn-primary');
        $('.cls_check_list_form .cls_check_list_no').val('');
        setValueToDDL('.cls_check_list_form .cls_lov_search_check_list', click_ref_temp_check_list_id, click_ref_temp_check_list_txt);
    }
    else {
        if (isEmpty(res.data[0].REFERENCE_CHECK_LIST_ID)) {
            $('.cls_btn_ref').removeClass('btn-primary').addClass('btn-default');
        }
        else {
            $('.cls_btn_ref').removeClass('btn-default').addClass('btn-primary');
            setValueToDDL('.cls_check_list_form .cls_lov_search_check_list', res.data[0].REFERENCE_CHECK_LIST_ID, res.data[0].REFERENCE_CHECK_LIST_DISPLAY_TXT);
        }

        showBtn(res.data[0].CHECK_LIST_ID, res.data[0].CHECK_LIST_NO);
    }
    click_ref_temp_check_list_id = '';
    click_ref_temp_check_list_txt = '';
    setRequiredFieldChecklist();
    //setProductCodeRefNo(rdoProductCodeRefNo);
}

function saveDataCheckList(only_save) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};

    if (only_save) { jsonObj.validate = false; }

    item["CHECK_LIST_ID"] = CheckListID;
    item["REFERENCE_REQUEST_NO"] = $('.cls_check_list_form .cls_lov_search_check_list option:selected').text();
    item["REQUEST_PHYSICAL_MOCKUP"] = $('input[name=req_physical_mock_up]:checked').val();
    var isForDesign = 0;
    var isDieLineOnly = 0;
    if ($('input[name=rdo_request_for]:checked').val() == "2") {
        isForDesign = 1;
    } else if ($('input[name=rdo_request_for]:checked').val() == "3") {
        isDieLineOnly = 1;
    }
    item["CHECK_LIST_FOR_DESIGN"] = isForDesign;
    item["REQUEST_FOR_DIE_LINE"] = isDieLineOnly;
    item["TRF_REF_NO"] = $('.cls_check_list_form .cls_input_trf_ref_no').val();
    item["PROJECT_NAME"] = $('.cls_check_list_form .cls_input_project_name').val();
    item["TYPE_OF_PRODUCT_ID"] = $('.cls_check_list_form .cls_lov_type_of_product').val();
    item["REVIEWER"] = $('.cls_check_list_form .cls_lov_reviewer').val();
    item["COMPANY_ID"] = $('.cls_check_list_form .cls_lov_company').val();
    item["SOLD_TO_ID"] = $('.cls_check_list_form .cls_lov_sold_to').val();
    item["SHIP_TO_ID"] = $('.cls_check_list_form .cls_lov_ship_to').val();
    item["CUSTOMER_OTHER_ID"] = $('.cls_check_list_form .cls_lov_other_to').val();
    item["REF_PRODUCT_CODE"] = $('.cls_check_list_form .cls_input_ref_product_code').val();
    item["PRIMARY_TYPE_ID"] = $('.cls_check_list_form .cls_lov_primary_type_other').val();
    item["PRIMARY_TYPE_OTHER"] = $('.cls_check_list_form .cls_input_primary_type_other').val();
    item["PRIMARY_MATERIAL"] = $('.cls_check_list_form .cls_primary_material').val();
    if ($('.cls_check_list_form .cls_chk_special_req').prop('checked')) {
        item["NOMINATED_CONTAINER_VENDOR"] = $('.cls_check_list_form .cls_txt_nominated_container').val();
        item["NOMINATED_SPEC"] = $('.cls_check_list_form .cls_txt_nominated_spec').val();
    }
    item["BRAND_ID"] = $('.cls_check_list_form .cls_lov_brand_other').val();
    item["BRAND_OTHER"] = $('.cls_check_list_form .cls_input_brand_other').val();
    item["BRAND_OEM_ID"] = $('.cls_check_list_form .cls_lov_brand_oem_other').val();
    item["BRAND_OEM_OTHER"] = $('.cls_check_list_form .cls_input_brand_oem_other').val();
    if (!isEmpty($('.cls_check_list_form .cls_rdd').datepicker("getDate"))) {
        item["REQUEST_DELIVERY_DATE"] = $('.cls_check_list_form .cls_rdd').datepicker("getDate").addHours(7);
    }
    item["OTHER_REQUESTS"] = $('.cls_check_list_form .cls_other_requests').val();
    item["IN_TRANSIT_TO"] = $('.cls_check_list_form .cls_input_checklist_in_tran').val();
    item["VIA"] = $('.cls_check_list_form .cls_input_checklist_via').val();
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;
    item["CREATOR_ID"] = UserID;

    var mailCustomerList = [];
    var first = true;
    $(".cls_check_list_form .cls_lov_mailto_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                var mailToItem = {};
                mailToItem["CHECK_LIST_ID"] = CheckListID;
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
    $(".cls_check_list_form .cls_lov_mailcc_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                var mailCcItem = {};
                mailCcItem["CHECK_LIST_ID"] = CheckListID;
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
    $(".cls_check_list_form .cls_lov_country_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                countryItem = {};
                countryItem["CHECK_LIST_ID"] = CheckListID;
                countryItem["COUNTRY_ID"] = $(this).val();
                countryItem["CREATE_BY"] = UserID;
                countryItem["UPDATE_BY"] = UserID;
                countryList.push(countryItem);
            }
        }
    });

    var plantList = [];
    first = true;
    $(".cls_check_list_form .cls_lov_checklist_plant_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                plantItem = {};
                plantItem["CHECK_LIST_ID"] = CheckListID;
                plantItem["PRODUCTION_PLANT_ID"] = $(this).val();
                plantItem["CREATE_BY"] = UserID;
                plantItem["UPDATE_BY"] = UserID;
                plantList.push(plantItem);
            }
        }
    });

    var first_row = true;
    var lineItemList = [];
    $(".cls_check_list_form .table_mockup_line_item tbody tr").each(function (index) {
        if (first_row) first_row = false;
        else {
            lineItem = {};
            lineItem["CHECK_LIST_ID"] = CheckListID;
            if (inTaskForm) lineItem["MOCKUP_ID"] = MOCKUPID;
            lineItem["PACKING_TYPE_ID"] = $(this).find('.cls_lov_packaging_type_static').val();
            lineItem["PRINT_SYSTEM_ID"] = $(this).find('.cls_lov_pim_static').val();
            lineItem["PRINT_SYSTEM_OTHER"] = $(this).find('.cls_input_pim_other_static').val();
            lineItem["NUMBER_OF_COLOR_ID"] = $(this).find('.cls_lov_pim_color_static').val();
            lineItem["NUMBER_OF_COLOR_OTHER"] = $(this).find('.cls_input_pim_color_other_static').val();
            lineItem["BOX_COLOR_ID"] = $(this).find('.cls_lov_box_color_static').val();
            lineItem["BOX_COLOR_OTHER"] = $(this).find('.cls_input_box_color_other_static').val();
            lineItem["COATING_ID"] = $(this).find('.cls_lov_coating_static').val();
            lineItem["COATING_OTHER"] = $(this).find('.cls_input_coating_other_static').val();
            lineItem["PURPOSE_OF"] = $(this).find('.cls_purpose_of').val();
            lineItem["STYLE_ID"] = $(this).find('.cls_lov_style_static').val();
            lineItem["STYLE_OTHER"] = $(this).find('.cls_input_style_other_static').val();
            lineItem["REMARK"] = $(this).find('.cls_txt_remark').val();
            lineItem["CREATE_BY"] = UserID;
            lineItem["UPDATE_BY"] = UserID;
            lineItemList.push(lineItem);
        }
    });

    first_row = true;
    var productList = [];
    var referenceList = [];
    if ($('input[name=req_product_code_ref_no]:checked').val() == 'product_code') {
        $(".cls_check_list_form .table_product_code_item tbody tr").each(function (index) {
            if (first_row) first_row = false;
            else {
                productItem = {};
                productItem["CHECK_LIST_ID"] = CheckListID;
                productItem["PRODUCT_CODE"] = $(this).find('.cls_product_code').val();
                productItem["NET_WEIGHT"] = $(this).find('.cls_net_weight').val();
                productItem["DRAINED_WEIGHT"] = $(this).find('.cls_drain_weight').val();
                productItem["PRIMARY_SIZE"] = $(this).find('.cls_primary_size').val();
                productItem["CONTAINER_TYPE"] = $(this).find('.cls_container_type').val();
                productItem["LID_TYPE"] = $(this).find('.cls_lid_type').val();
                productItem["PACKING_STYLE"] = $(this).find('.cls_packing_style').val();
                productItem["PACK_SIZE"] = $(this).find('.cls_pack_size').val();
                productItem["CREATE_BY"] = UserID;
                productItem["UPDATE_BY"] = UserID;
                productList.push(productItem);
            }
        });
    } else {

        item["RD_PERSON_ID"] = $('.cls_check_list_form .cls_lov_rd_person').val();

        item["THREE_P_ID"] = $('.cls_check_list_form .cls_lov_primary_size_other').val();
        //item["PRIMARY_SIZE_ID"] = $('.cls_check_list_form .cls_lov_primary_size_other').val();
        item["PRIMARY_SIZE_OTHER"] = $('.cls_check_list_form .cls_input_primary_size_other').val();
        if ($('.cls_check_list_form .cls_lov_primary_size_other').val() == -1) {
            item["CONTAINER_TYPE_ID"] = $('.cls_check_list_form .cls_lov_container_type_other').val();
            item["CONTAINER_TYPE_OTHER"] = $('.cls_check_list_form .cls_input_container_type_other').val();
            item["LID_TYPE_ID"] = $('.cls_check_list_form .cls_lov_lid_type_other').val();
            item["LID_TYPE_OTHER"] = $('.cls_check_list_form .cls_input_lid_type_other').val();
        }
        item["TWO_P_ID"] = $('.cls_check_list_form .cls_lov_packing_style_other').val();
        //item["PACKING_STYLE_ID"] = $('.cls_check_list_form .cls_lov_packing_style_other').val();
        item["PACKING_STYLE_OTHER"] = $('.cls_check_list_form .cls_input_packing_style_other').val();
        if ($('.cls_check_list_form .cls_lov_packing_style_other').val() == -1 || isEmpty($('.cls_check_list_form .cls_lov_packing_style_other').val())) {
            item["PACK_SIZE_ID"] = $('.cls_check_list_form .cls_lov_pack_size_other').val();
            item["PACK_SIZE_OTHER"] = $('.cls_check_list_form .cls_input_pack_size_other').val();
        }

        $(".cls_check_list_form .table_ref_no_item tbody tr").each(function (index) {
            if (first_row) first_row = false;
            else {
                referenceItem = {};
                referenceItem["CHECK_LIST_ID"] = CheckListID;
                referenceItem["REFERENCE_NO"] = $(this).find('.cls_ref_no').val();
                referenceItem["NET_WEIGHT"] = $(this).find('.cls_net_weight' + 'X').val();
                referenceItem["DRAINED_WEIGHT"] = $(this).find('.cls_drain_weight').val();
                referenceItem["CREATE_BY"] = UserID;
                referenceItem["UPDATE_BY"] = UserID;
                referenceList.push(referenceItem);
            }
        });
    }

    var error = false;
    $.each(productList, function (index, item) {
        $.each(productList, function (index2, item2) {
            if (item.NET_WEIGHT != item2.NET_WEIGHT) {
                error = true;
            }
        });
        $.each(productList, function (index2, item2) {
            if (item.PRIMARY_SIZE != item2.PRIMARY_SIZE) {
                error = true;
            }
        });
        $.each(productList, function (index2, item2) {
            if (item.CONTAINER_TYPE != item2.CONTAINER_TYPE) {
                error = true;
            }
        });
        $.each(productList, function (index2, item2) {
            if (item.LID_TYPE != item2.LID_TYPE) {
                error = true;
            }
        });
        $.each(productList, function (index2, item2) {
            if (item.PACKING_STYLE != item2.PACKING_STYLE) {
                error = true;
            }
        });
        $.each(productList, function (index2, item2) {
            if (item.PACK_SIZE != item2.PACK_SIZE) {
                error = true;
            }
        });
    });
    if (error) { alertError2("Product code's information invalid."); return; }

    var mk_submit = false;
    if (inTaskForm) {
        item["MOCKUP_ID"] = MOCKUPID;
        if (!only_save) {
            mk_submit = true;
            item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
            item["ENDTASKFORM"] = true;
        }
    }

    jsonObj.data = item;
    jsonObj.data.country = countryList;
    jsonObj.data.plant = plantList;
    jsonObj.data.item = lineItemList;
    jsonObj.data.product = productList;
    jsonObj.data.reference = referenceList;
    jsonObj.data.mail_to_customer = mailCustomerList;

    var myurl = '/api/checklist/request';
    var mytype = 'POST';
    var mydata = jsonObj;

    var validateref_no = true;
    if ($('input[name=rdo_request_for]:checked').val() == 2) {
        //project
        if ($('input[name=req_product_code_ref_no]:checked').val() == 'ref_no') {
            validateref_no = false;
        }
    }

    if (mk_submit) {
        var rowCount = $('.cls_check_list_form .table_mockup_line_item tbody tr').length;
        if (rowCount > 1) {
            if ($('input[name=req_product_code_ref_no]:checked').val() == 'product_code' && productList.length == 0) {
                alertError2("Product code must contain at least 1 item");
                $('.cls_input_product_code').focus();
            } else if ($('input[name=req_product_code_ref_no]:checked').val() == 'ref_no' && referenceList.length == 0 && validateref_no) {
                alertError2("RD reference No./3V must contain at least 1 item");
                $('.cls_input_reference_no').focus();
            } else {
                myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
            }
        }
        else {
            alertError2("Please add item.");
        }
    }
    else {
        if (only_save)
            myAjax(myurl, mytype, mydata, callbackSaveDataCheckList);
        else {
            var rowCount = $('.cls_check_list_form .table_mockup_line_item tbody tr').length;
            if (rowCount > 1) {
                if ($('input[name=req_product_code_ref_no]:checked').val() == 'product_code' && productList.length == 0) {
                    alertError2("Product code must contain at least 1 item");
                    $('.cls_input_product_code').focus();
                } else if ($('input[name=req_product_code_ref_no]:checked').val() == 'ref_no' && referenceList.length == 0 && validateref_no) {
                    alertError2("RD reference No./3V must contain at least 1 item");
                    $('.cls_input_reference_no').focus();
                } else {
                    myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSubmitDataCheckList, '', false, false);
                }
            }
            else {
                alertError2("Please add item.");
            }
        }
    }
}

function callbackSaveDataCheckList(res) {
    CheckListID = res.data[0].CHECK_LIST_ID;
    if (!inTaskForm) $('.cls_btn_delete').show();
    //bindDataCheckList(CheckListID);
}

function callbackSubmitDataCheckList(res) {
    CheckListID = res.data[0].CHECK_LIST_ID;

    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["CURRENT_STEP_ID"] = getstepmockup('SEND_PG').curr_step;
    item["CURRENT_ROLE_ID"] = getstepmockup('SEND_PG').curr_role;
    item["CHECK_LIST_ID"] = CheckListID;
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;
    jsonObj.data = item;

    var myurl = '/api/taskform/mockupprocess/checklist';
    var mytype = 'POST';
    var mydata = jsonObj;
    //myAjaxAlert(myurl, mytype, mydata, "", "", true, true);
    myAjaxNoAlertSuccess(myurl, mytype, mydata, callbackSubmitDataCheckListSuccess);
}

function callbackSubmitDataCheckListSuccess(res) {

    if (res.status == "S") {
        var mymodal = $('#modal_chklist_submit_success');
        mymodal.find('.modal-body').html(res.msg);
        mymodal.modal({ backdrop: 'static', keyboard: false });
    }
    else if (res.status == "E") {
        if (res.msg != '')
            alertError(res.msg);
    }
    //  $("#modal_chklist_submit_success").modal("toggle");

}


function getCheckListProductCode(jsonObj) {
    var myurl = '/api/checklist/product';
    var mytype = 'POST2';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callback_set_productcode);
}

function callback_set_productcode(res) {
    if (res.data.length > 0) {
        bindCheckListProductCode(res.data);
        setProductCodeRefNo('product_code');
    }
    else {
        alertError2("Product code not found");
    }
}

function bindCheckListProductCode(data) {
    var strAlert = "";
    for (var i = 0; i < data.length; i++) {
        var isSame = true;
        var v = data[i];

        var str_net_weight = "";
        var str_drain_weight = "";
        var str_primary_size = "";
        var str_container_type = "";
        var str_lid_type = "";
        var str_packing_style = "";
        var str_pack_size = "";
        if (v.NET_WEIGHT != null) {
            str_net_weight = v.NET_WEIGHT;
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

        var obj = $('.cls_check_list_form .table_product_code_item tbody tr:last');
        if (obj.find('.cls_product_code').val() != undefined) {
            if (obj.find('.cls_product_code').val().toLowerCase() != "") {
                if (str_net_weight.toLowerCase() != obj.find('.cls_net_weight').val().toLowerCase()
                    || str_primary_size.toLowerCase() != obj.find('.cls_primary_size').val().toLowerCase()
                    || str_container_type.toLowerCase() != obj.find('.cls_container_type').val().toLowerCase()
                    || str_lid_type.toLowerCase() != obj.find('.cls_lid_type').val().toLowerCase()
                    || str_packing_style.toLowerCase() != obj.find('.cls_packing_style').val().toLowerCase()
                    || str_drain_weight.toLowerCase() != obj.find('.cls_drain_weight').val().toLowerCase()
                    || str_pack_size.toLowerCase() != obj.find('.cls_pack_size').val().toLowerCase())
                {
                    isSame = false;
                    strAlert = strAlert + v.PRODUCT_CODE + "<br>";
                }
            }
        }

        if (isSame) {

            var obj = $('.tr_product_code_item').clone().removeClass('tr_product_code_item').removeClass('cls_cn_hide');

            $('.cls_check_list_form .table_product_code_item tbody').append(obj);
            var obj2 = $('.cls_check_list_form .table_product_code_item tbody tr:last');

            $(".cls_check_list_form .cls_img_delete_product_code_item").click(function () {
                $(this).closest('tr').remove();
            });

            input = 'cls_product_code';
            obj.find('.' + input).val(v.PRODUCT_CODE);

            input = 'cls_product_desc';
            obj.find('.' + input).val(v.PRODUCT_DESCRIPTION);

            input = 'cls_net_weight';
            obj.find('.' + input).val(v.NET_WEIGHT);

            input = 'cls_drain_weight';
            obj.find('.' + input).val(v.DRAINED_WEIGHT);

            input = 'cls_primary_size';
            obj.find('.' + input).val(v.PRIMARY_SIZE);

            input = 'cls_container_type';
            obj.find('.' + input).val(v.CONTAINER_TYPE);

            input = 'cls_lid_type';
            obj.find('.' + input).val(v.LID_TYPE);

            input = 'cls_packing_style';
            obj.find('.' + input).val(v.PACKING_STYLE);

            input = 'cls_pack_size';
            obj.find('.' + input).val(v.PACK_SIZE);

        }

    }

    if (strAlert.length > 0) {
        alertError2(strAlert + " not same characteristic value (net weight, drain weight, primary size, container type, lid type, pack size and packing style)")
    }
}

//function getCheckListRefNo(jsonObj) {
//    var myurl = '/api/checklist/reference';
//    var mytype = 'POST';
//    var mydata = jsonObj;
//    myAjax(myurl, mytype, mydata, callback_set_ref_no);
//}

function callback_set_ref_no(res) {
    if (res.data.length > 0) {
        bindCheckListRefNo(res.data);
        setProductCodeRefNo('ref_no');
        //var v = res.data[0];
        //setValueToDDL('.cls_lov_primary_size_other', v.PRIMARY_SIZE_ID, v.PRIMARY_SIZE_DISPLAY_TXT);
        //setValueToDDL('.cls_lov_container_type_other', v.CONTAINER_TYPE_ID, v.CONTAINER_TYPE_DISPLAY_TXT);
        //setValueToDDL('.cls_lov_lid_type_other', v.LID_TYPE_ID, v.LID_TYPE_DISPLAY_TXT);
        //setValueToDDL('.cls_lov_packing_style_other', v.PACKING_STYLE_ID, v.PACKING_STYLE_DISPLAY_TXT);
        //setValueToDDL('.cls_lov_pack_size_other', v.PACK_SIZE_ID, v.PACK_SIZE_DISPLAY_TXT);
        //setValueToDDL('.cls_lov_brand_other', v.BRAND_ID, v.BRAND_DISPLAY_TXT);
    }
    else {
        alertError2("RD reference No./3V Not found");
    }
}

function bindCheckListRefNo(data) {
    for (var i = 0; i < data.length; i++) {
        var v = data[i];
        var obj = $('.tr_ref_no_item').clone().removeClass('tr_ref_no_item').removeClass('cls_cn_hide');

        $('.cls_check_list_form .table_ref_no_item tbody').append(obj);
        var obj2 = $('.cls_check_list_form .table_ref_no_item tbody tr:last');

        $(".cls_check_list_form .cls_img_delete_ref_no_item").click(function () {
            $(this).closest('tr').remove();

            if ($('.table_ref_no_item tbody tr:visible').length == 0) {
                $('.cls_lbl_netweight_rd:visible').html("Net weight");
            }
        });

        input = 'cls_ref_no';
        obj.find('.' + input).val(v.REFERENCE_NO);

        input = 'cls_net_weight';
        obj.find('.' + input).toggleClass(input + ' ' + input + 'X');
        obj.find('.' + input + 'X').val(v.NET_WEIGHT);
        $('.' + input + 'X').attr("required", true);

        input = 'cls_drain_weight';
        obj.find('.' + input).val(v.DRAINED_WEIGHT);
    }
}

function setProductCodeRefNo(val) {
    if (val == 'product_code') {
        $('.cls_check_list_form .cls_row_product_code_header').show();
        $('.cls_check_list_form .cls_row_product_code_detail').show();
        $('.cls_check_list_form .cls_row_ref_no_header').hide();
        $('.cls_check_list_form .cls_row_ref_no_detail1').hide();
        $('.cls_check_list_form .cls_row_ref_no_detail2').hide();
        $('.cls_check_list_form .cls_row_ref_no_detail3').hide();

        $('.cls_check_list_form .cls_lov_primary_size_other').prop("disabled", true);
        $('.cls_check_list_form .cls_lov_container_type_other').prop("disabled", true);
        $('.cls_check_list_form .cls_lov_lid_type_other').prop("disabled", true);
        $('.cls_check_list_form .cls_lov_packing_style_other').prop("disabled", true);
        $('.cls_check_list_form .cls_lov_pack_size_other').prop("disabled", true);
        $('.cls_check_list_form .cls_lov_rd_person').prop("disabled", true);

        $('.cls_check_list_form .cls_net_weight' + 'X').attr("required", false);
    }
    else if (val == 'ref_no') {
        $('.cls_check_list_form .cls_row_product_code_header').hide();
        $('.cls_check_list_form .cls_row_product_code_detail').hide();
        $('.cls_check_list_form .cls_row_ref_no_header').show();
        $('.cls_check_list_form .cls_row_ref_no_detail1').show();
        $('.cls_check_list_form .cls_row_ref_no_detail2').show();
        $('.cls_check_list_form .cls_row_ref_no_detail3').show();

        $('.cls_check_list_form .cls_lov_primary_size_other').prop("disabled", false);
        if ($('.cls_check_list_form .cls_lov_container_type_other').val() == -99) {
            $('.cls_check_list_form .cls_lov_container_type_other').prop("disabled", true);
        } else {
            $('.cls_check_list_form .cls_lov_container_type_other').prop("disabled", false);
        }
        if ($('.cls_check_list_form .cls_lov_lid_type_other').val() == -99) {
            $('.cls_check_list_form .cls_lov_lid_type_other').prop("disabled", true);
        } else {
            $('.cls_check_list_form .cls_lov_lid_type_other').prop("disabled", false);
        }
        $('.cls_check_list_form .cls_lov_packing_style_other').prop("disabled", false);
        if ($('.cls_check_list_form .cls_lov_pack_size_other').val() == -99) {
            $('.cls_check_list_form .cls_lov_pack_size_other').prop("disabled", true);
        } else {
            $('.cls_check_list_form .cls_lov_pack_size_other').prop("disabled", false);
        }

        $('.cls_check_list_form .cls_lov_rd_person').prop("disabled", false);

        $('.cls_check_list_form .cls_net_weight' + 'X').attr("required", true);
    }
}

function setRequiredFieldChecklist() {

    setRequiredFieldForNotFFC();

    var lbl_spanStar = "<span style=\"color: red;\">*</span>";
    if ($('input[name=rdo_request_for]:checked').val() == 1) {
        //Normal
        $('.cls_check_list_form .cls_lbl_company:visible').html("Company" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_company:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_sold_to:visible').html("Sold to" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_sold_to:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_mailto:visible:first').html("TO" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_mailto_multiple_static:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_country:visible:first').html("Country" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_country_multiple_static:visible:first').attr("required", true);

        $('.cls_check_list_form .cls_lbl_plant:visible:first').html("Production plant" + lbl_spanStar + " :"); //by aof 04 / 11 / 2021 for Itail add Production Plant
        $('.cls_check_list_form .cls_lov_checklist_plant_multiple_static:visible:first').attr("required", true); //by aof 04 / 11 / 2021 for Itail add Production Plant

        $('.cls_check_list_form .cls_lbl_rd_person:visible').html("RD person" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_rd_person:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_container_type:visible').html("Container type" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_container_type_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_lid_type:visible').html("Lid type" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_lid_type_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_packing_style:visible').html("Pack size" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_input_pack_size_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_packing_style:visible').html("Packing style" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_packing_style_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_style:visible').html("Style" + lbl_spanStar);
        $('.cls_check_list_form .cls_lov_style_static:visible').attr("required", true);

        if ($('input[name=req_product_code_ref_no]:checked').val() == 'product_code') {
            $('.cls_lbl_product_code').html("Product code" + lbl_spanStar + " : ");
        }
        else if ($('input[name=req_product_code_ref_no]:checked').val() == 'ref_no') {
            $('.cls_lbl_rd_ref_no_no_star').hide();
            $('.cls_lbl_rd_ref_no').show();
        }

    } else if ($('input[name=rdo_request_for]:checked').val() == 2) {
        //Project

        $('.cls_check_list_form .cls_lbl_company:visible').html("Company" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_company:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_sold_to:visible').html("Sold to" + " :");
        $('.cls_check_list_form .cls_lov_sold_to:visible').attr("required", false);

        $('.cls_check_list_form .cls_lbl_mailto:visible:first').html("TO" + " :");
        $('.cls_check_list_form .cls_lov_mailto_multiple_static').attr("required", false);

        // start #INC-73353 by aof on 31/08/2022
        //$('.cls_check_list_form .cls_lbl_country:visible:first').html("Country" + " :");   
        //$('.cls_check_list_form .cls_lov_country_multiple_static:visible:first').attr("required", false);
        $('.cls_check_list_form .cls_lbl_country:visible:first').html("Country" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_country_multiple_static:visible:first').attr("required", true);
        //end #INC-73353 by aof on 31/08/2022

        $('.cls_check_list_form .cls_lbl_plant:visible:first').html("Production plant" + lbl_spanStar + " :"); //by aof 04 / 11 / 2021 for Itail add Production Plant
        $('.cls_check_list_form .cls_lov_checklist_plant_multiple_static:visible:first').attr("required", true); //by aof 04 / 11 / 2021 for Itail add Production Plant

        $('.cls_check_list_form .cls_lbl_rd_person:visible').html("RD person" + " :");
        $('.cls_check_list_form .cls_lov_rd_person:visible').attr("required", false);

        $('.cls_check_list_form .cls_lbl_container_type:visible').html("Container type" + " :");
        $('.cls_check_list_form .cls_lov_container_type_other:visible').attr("required", false);

        $('.cls_check_list_form .cls_lbl_lid_type:visible').html("Lid type" + " :");
        $('.cls_check_list_form .cls_lov_lid_type_other:visible').attr("required", false);

        $('.cls_check_list_form .cls_lbl_pack_size:visible').html("Pack size" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_input_pack_size_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_packing_style:visible').html("Packing style" + " :");
        $('.cls_check_list_form .cls_lov_packing_style_other:visible').attr("required", false);

        $('.cls_check_list_form .cls_lbl_style:visible').html("Style");
        $('.cls_check_list_form .cls_lov_style_static:visible').attr("required", false);

        if ($('input[name=req_product_code_ref_no]:checked').val() == 'product_code') {
            $('.cls_lbl_product_code').html("Product code" + lbl_spanStar + " :");
        }
        else if ($('input[name=req_product_code_ref_no]:checked').val() == 'ref_no') {
            $('.cls_lbl_rd_ref_no_no_star').show();
            $('.cls_lbl_rd_ref_no').hide();
        }

    } else if ($('input[name=rdo_request_for]:checked').val() == 3) {
        //Dieline
        $('.cls_check_list_form .cls_lbl_company:visible').html("Company" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_company:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_sold_to:visible').html("Sold to" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_sold_to:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_mailto:visible:first').html("TO" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_mailto_multiple_static:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_country:visible:first').html("Country" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_country_multiple_static:visible:first').attr("required", true);

        $('.cls_check_list_form .cls_lbl_plant:visible:first').html("Production plant" + lbl_spanStar + " :"); //by aof 04 / 11 / 2021 for Itail add Production Plant
        $('.cls_check_list_form .cls_lov_checklist_plant_multiple_static:visible:first').attr("required", true); //by aof 04 / 11 / 2021 for Itail add Production Plant

        $('.cls_check_list_form .cls_lbl_rd_person:visible').html("RD person" + " :");
        $('.cls_check_list_form .cls_lov_rd_person:visible').attr("required", false);

        $('.cls_check_list_form .cls_lbl_container_type:visible').html("Container type" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_container_type_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_lid_type:visible').html("Lid type" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_lid_type_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_packing_style:visible').html("Pack size" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_input_pack_size_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_packing_style:visible').html("Packing style" + lbl_spanStar + " :");
        $('.cls_check_list_form .cls_lov_packing_style_other:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_style:visible').html("Style");
        $('.cls_check_list_form .cls_lov_style_static:visible').attr("required", false);

        if ($('input[name=req_product_code_ref_no]:checked').val() == 'product_code') {
            $('.cls_lbl_product_code').html("Product code" + lbl_spanStar + " : ");
        }
        else if ($('input[name=req_product_code_ref_no]:checked').val() == 'ref_no') {
            $('.cls_lbl_rd_ref_no_no_star').hide();
            $('.cls_lbl_rd_ref_no').show();
        }
    }
}

function setRequiredFieldForNotFFC() {
    var lbl_spanStar = "<span style=\"color: red;\">*</span>";
    if (UserPosition != "FFC") {
        $('.cls_check_list_form .cls_lbl_other_requests').html("Other requests" + " :");
        $('.cls_check_list_form .cls_other_requests').attr("required", false);

        $('.cls_check_list_form .cls_lbl_remark').html("Remark");
        $('.cls_check_list_form .cls_txt_remark').attr("required", false);
    }
    else {
        $('.cls_check_list_form .cls_lbl_remark').html("Remark" + lbl_spanStar);
        $('.cls_check_list_form .cls_txt_remark:visible').attr("required", true);

        $('.cls_check_list_form .cls_lbl_product_code').html("Product code");
    }
}

function bindSearchRequesForm(soldto, shipto, brand, country, creator) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["SOLD_TO_ID"] = soldto;
    item["SHIP_TO_ID"] = shipto;
    item["BRAND_ID"] = brand;
    item["COUNTRY_ID"] = country;
    item["CREATOR_ID"] = creator;
    jsonObj.data = item;

    var table = $('#table_chklist_search').DataTable();
    table.destroy();

    table = $('#table_chklist_search').DataTable({
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
        "processing": true,
        "scrollX": true,
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
            if (data.CHECK_LIST_ID > 0)
                $(row).find('td').eq(1).html('<a href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;" target="_blank">' + data.REQUEST_FORM_NO + '</a>');
            else
                $(row).find('td').eq(1).html('<a href="' + suburl + '/Artwork/' + data.ARTWORK_ID + '" style="text-decoration: underline;" target="_blank">' + data.REQUEST_FORM_NO + '</a>');
        }
    });
}

function resetMailToCcChecklist(isFFC) {
    if (isFFC) {
        $('.cls_check_list_form .cls_lov_mailto_multiple_static').prop("disabled", false);
        $('.cls_check_list_form .cls_lov_mailcc_multiple_static').prop("disabled", false);
        var frist = true;
        $('.cls_check_list_form .cls_lov_mailto_multiple_static:visible').each(function (index) {
            if (frist) {
                frist = false;
                $(this).prop("disabled", true);
            }
            else {
                $(this).val('').trigger("change");
                if ($('.cls_check_list_form .cls_lov_sold_to').val() == null && $('.cls_check_list_form .cls_lov_ship_to').val() == null && $('.cls_check_list_form .cls_lov_other_to').val() == null) {
                    $(this).prop("disabled", true);
                }
                else {
                    $(this).prop("disabled", false);
                }
            }
        });
        var frist = true;
        $('.cls_check_list_form .cls_lov_mailcc_multiple_static:visible').each(function (index) {
            if (frist) {
                frist = false;
                $(this).prop("disabled", true);
            }
            else {
                $(this).val('').trigger("change");
                if ($('.cls_check_list_form .cls_lov_sold_to').val() == null && $('.cls_check_list_form .cls_lov_ship_to').val() == null && $('.cls_check_list_form .cls_lov_other_to').val() == null) {
                    $(this).prop("disabled", true);
                }
                else {
                    $(this).prop("disabled", false);
                }
            }
        });
    }
    else {
        $('.cls_check_list_form .cls_lov_mailto_multiple_static').val('').trigger("change");
        $('.cls_check_list_form .cls_lov_mailcc_multiple_static').val('').trigger("change");
        if ($('.cls_check_list_form .cls_lov_sold_to').val() == null && $('.cls_check_list_form .cls_lov_ship_to').val() == null && $('.cls_check_list_form .cls_lov_other_to').val() == null) {
            $('.cls_check_list_form .cls_lov_mailto_multiple_static').prop("disabled", true);
            $('.cls_check_list_form .cls_lov_mailcc_multiple_static').prop("disabled", true);
        } else {
            $('.cls_check_list_form .cls_lov_mailto_multiple_static').prop("disabled", false);
            $('.cls_check_list_form .cls_lov_mailcc_multiple_static').prop("disabled", false);
        }
    }

}

function getplantlist() {
    var plantList = [];
    first = true;
    $(".cls_artwork_request_form .cls_lov_artwork_plant_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).val())) {
                plantItem = {};
                plantItem["CHECK_LIST_ID"] = CheckListID;
                plantItem["PRODUCTION_PLANT_ID"] = $(this).val();
                plantItem["CREATE_BY"] = UserID;
                plantItem["UPDATE_BY"] = UserID;
                plantList.push(plantItem);
            }
        }
    });
    return plantList;
}
