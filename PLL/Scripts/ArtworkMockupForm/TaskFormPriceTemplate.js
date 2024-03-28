$(document).ready(function () {
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_price_template':
                //bindDataPG(MOCKUPSUBID);
                bind_header_quo();
                vendor_selected = false;
                bind_price_compare2();
                if (table_user_vendor_price_template == null)
                    bind_user_vendor_price_template();
                else {
                    //table_user_vendor_price_template.ajax.reload();
                }
                if (table_pg_price_template == null)
                    bind_table_pg_price_template();
                else
                    table_pg_price_template.ajax.reload();
                if (table_vendor == null)
                    bind_vendor();
                else
                    table_vendor.ajax.reload();
                break;
            default:
                break;
        }
    });

    $(".cls_pg_price_template .cls_btn_select_vendor_by_manager").click(function () {
        if (isEmpty($("input[name='select_vendor_by_manager']:checked").val())) {
            alertError2("Please select vendor at least 1 item.");
        }
        else {
            var jsonObj = new Object();
            jsonObj.data = {};

            jsonObj.data["ACTION_CODE"] = 'SUBMIT';
            jsonObj.data["MOCKUP_ID"] = MOCKUPID;
            jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
            jsonObj.data["MOCKUPSUBID2"] = $("input[name='select_vendor_by_manager']:checked").val();
            jsonObj.data["CREATE_BY"] = UserID;
            jsonObj.data["UPDATE_BY"] = UserID;
            jsonObj.data["USER_ID"] = $("input[name='select_vendor_by_manager']:checked").closest('tr').find('.cls_user_id').text();
            jsonObj.data["VENDOR_ID"] = $("input[name='select_vendor_by_manager']:checked").closest('tr').find('.cls_vendor_id').text();
            jsonObj.data["COMMENT_BY_PG_SUP"] = $('.cls_div_select_vendor_manager .cls_remark_pg_sub_select_vendor').val();
            var myurl = '/api/taskform/pg/selectvendor';
            var mytype = 'POST';
            var mydata = jsonObj;
            myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
        }
    });

    $(".cls_pg_price_template .cls_btn_send_back_select_vendor_by_manager").click(function () {
        var jsonObj = new Object();
        jsonObj.data = {};

        jsonObj.data["ACTION_CODE"] = 'SEND_BACK';
        jsonObj.data["MOCKUP_ID"] = MOCKUPID;
        jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        //jsonObj.data["MOCKUPSUBID2"] = $("input[name='select_vendor_by_manager']:checked").val();
        jsonObj.data["CREATE_BY"] = UserID;
        jsonObj.data["UPDATE_BY"] = UserID;
        //jsonObj.data["USER_ID"] = $("input[name='select_vendor_by_manager']:checked").closest('tr').find('.cls_user_id').text();
        //jsonObj.data["VENDOR_ID"] = $("input[name='select_vendor_by_manager']:checked").closest('tr').find('.cls_vendor_id').text();
        jsonObj.data["COMMENT_BY_PG_SUP"] = $('.cls_div_select_vendor_manager .cls_remark_pg_sub_select_vendor').val();
        var myurl = '/api/taskform/pg/selectvendor';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
    });

    $('.cls_table_pg_price_template tbody').on('blur', 'input', function () {
        var obj = $(this).closest('tr');
        var d = table_pg_price_template.row(obj).data();
        d.SCALE = removeComma($(obj).find('.cls_scale').val());
        table_pg_price_template
            .row(obj)
            .data(d)
            .draw(false);
    });

    $(".cls_pg_price_template .cls_btn_new_row_price_template").click(function () {
        add_row_price_template(0);
    });

    $(".cls_pg_price_template .cls_btn_reset_price_template").click(function () {
        table_pg_price_template.ajax.reload();
    });

    $(".cls_pg_price_template .cls_btn_reset_vendor_price_template").click(function () {
        table_user_vendor_price_template.ajax.reload();
    });

    $(".cls_pg_price_template .cls_btn_save_price_template").click(function () {
        save_price_template('');
    });

    $(document).on("click", ".cls_pg_price_template .cls_img_lov_delete_row_price_template", function () {
        table_pg_price_template
            .row($(this).parents('tr'))
            .remove()
            .draw();
    });
    $(document).on("click", ".cls_pg_price_template .cls_img_lov_delete_row_user_vendor", function () {
        table_user_vendor_price_template
            .row($(this).parents('tr'))
            .remove()
            .draw();
    });

    //$(".cls_pg_price_template .cls_btn_request_quotation_price_template").click(function () {
    //    save_price_template_vendor('rounting');
    //});
    $(".cls_div_select_vendor form").submit(function (e) {
        var error = false;
        if ($(this).valid()) {
            var str = '<span style="font-weight:bold"> Please input data in PG tab.</span><br/><span>The following field is required.</span>';
            $(".cls_div_select_vendor form :input").each(function () {
                if ($(this).prop('required')) {
                    if (isEmpty($(this).val())) {
                        error = true;
                        str += "<br/>&nbsp;&nbsp;&nbsp;" + $.trim($(this).closest('div').prev().text());
                    }
                }
            });

            if (!error) {
                save_price_template_vendor('rounting');
            }
            else {
                alertError(str);
            }
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_pg_price_template .cls_btn_save_vendor_price_template").click(function () {
        save_price_template_vendor('');
    });

    bind_lov_param('.cls_lov_search_vendor_price_template', '/api/lov/vendorhasuser_bymatgroup', 'data.DISPLAY_TXT', ["MATGROUP_ID"], ['.cls_task_form_pg .cls_lov_pg_packaging_type'], '', callback_lov_select);
    bind_lov_param('.cls_lov_search_vendor_price_template_manual', '/api/lov/vendorhasuser_bymatgroup', 'data.DISPLAY_TXT', ["MATGROUP_ID"], ['.cls_task_form_pg .cls_lov_pg_packaging_type']);

    //bind_lov('.cls_lov_search_vendor_price_template', '/api/lov/vendorhasuser_bymatgroup', 'data.vendor_name', '', callback_lov_select);
    //bind_lov('.cls_lov_search_vendor_price_template_manual', '/api/lov/vendorhasuser_bymatgroup', 'data.vendor_name', '');

    $(".cls_div_price_compare .cls_btn_save_vendor_price_template_manual").click(function () {
        if (isEmpty($('.cls_div_price_compare .cls_lov_search_vendor_price_template_manual').val())) {
            alertError2("Please select vendor.");
            return;
        }
        if (table_pg_price_template.data().count() == 0) {
            alertError2("Please set price template at least 1 item.");
            return;
        }
        var jsonObj = new Object();
        jsonObj.data = [];
        var lineItemList = [];
        $(".cls_table_pg_price_compare .cls_price_manual:enabled").each(function (index) {
            lineItem = {};
            lineItem["MOCKUP_ID"] = MOCKUPID;
            lineItem["SCALE"] = removeComma($(this).closest('tr').find('.cls_scale').text());
            lineItem["PRICE"] = $(this).autoNumeric('get');
            lineItem["MOCKUP_SUB_ID"] = $(this).data('mockup-sub-id');

            var tdObj = $(this).closest('td');
            var rowIndex = $(tdObj).parent().index('.cls_table_pg_price_compare tbody tr');
            var tdIndex = $(tdObj).index('.cls_table_pg_price_compare tbody tr:eq(' + rowIndex + ') td');
            lineItem["COMMENT_BY_VENDOR"] = $(this).closest('tbody').find("tr:last").find('td').eq(tdIndex).find('input').val();

            lineItem["CREATE_BY"] = UserID;
            lineItem["UPDATE_BY"] = '-1';
            lineItemList.push(lineItem);
        });
        jsonObj.data = lineItemList;

        var myurl = '/api/taskform/pg/pricetemplatemanualprice';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxConfirmSubmit(myurl, mytype, mydata, save_price_template2, '', false, false);

        //save_price_template('manual');
    });

    $(".cls_div_price_compare .cls_btn_save_vendor_price_template_manual_price").click(function () {
        var jsonObj = new Object();
        jsonObj.data = [];
        var lineItemList = [];
        $(".cls_table_pg_price_compare .cls_price_manual:enabled").each(function (index) {
            lineItem = {};
            lineItem["MOCKUP_ID"] = MOCKUPID;
            lineItem["SCALE"] = removeComma($(this).closest('tr').find('.cls_scale').text());
            lineItem["PRICE"] = $(this).autoNumeric('get');
            lineItem["MOCKUP_SUB_ID"] = $(this).data('mockup-sub-id');

            var tdObj = $(this).closest('td');
            var rowIndex = $(tdObj).parent().index('.cls_table_pg_price_compare tbody tr');
            var tdIndex = $(tdObj).index('.cls_table_pg_price_compare tbody tr:eq(' + rowIndex + ') td');
            lineItem["COMMENT_BY_VENDOR"] = $(this).closest('tbody').find("tr:last").find('td').eq(tdIndex).find('input').val();

            lineItem["CREATE_BY"] = UserID;
            lineItem["UPDATE_BY"] = '-1';
            lineItemList.push(lineItem);
        });
        jsonObj.data = lineItemList;

        var myurl = '/api/taskform/pg/pricetemplatemanualprice';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxConfirmSubmit(myurl, mytype, mydata, bind_price_compare2, '', false, false);
    });

    $(document).on("click", ".cls_div_price_compare .cls_img_delete_price_template_manual", function () {
        var jsonObj = new Object();
        jsonObj.data = {};
        jsonObj.data["MOCKUP_SUB_ID"] = $(this).data('mockup-sub-id');
        jsonObj.data["CREATE_BY"] = UserID;
        jsonObj.data["UPDATE_BY"] = UserID;

        var myurl = '/api/taskform/pg/pricetemplatemanualprice';
        var mytype = 'DELETE';
        var mydata = jsonObj;
        myAjaxConfirmDelete(myurl, mytype, mydata, bind_price_compare2, '', false, false);
    });

    $(document).on("focus", ".cls_div_price_compare .cls_price_manual, .cls_div_price_template .cls_scale", function () {
        $(this).select();
    });

    var ddlPackagingType = 'table_pg_mockup_line_item .cls_lov_pg_packaging_type';
    bind_lov_param('.cls_div_select_vendor .cls_qua_direction_of_sticker', '/api/lov/distick', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.' + ddlPackagingType]);
    bind_lov_param('.cls_div_select_vendor .cls_qua_style_of_printing', '/api/lov/styleprinting', 'data.DISPLAY_TXT', ["PACKAGING_TYPE_ID"], ['.' + ddlPackagingType], '.cls_input_qua_style_of_printing');
});

function save_price_template_manual() {
    var jsonObj = new Object();
    jsonObj.data = {};

    jsonObj.data["MOCKUP_ID"] = MOCKUPID;
    jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    jsonObj.data["CREATE_BY"] = UserID;
    jsonObj.data["UPDATE_BY"] = UserID;
    jsonObj.data["CURRENT_VENDOR_ID"] = $('.cls_div_price_compare .cls_lov_search_vendor_price_template_manual').val();
    jsonObj.data["CURRENT_STEP_ID"] = getstepmockup('SEND_VN_QUO').curr_step;
    jsonObj.data["CURRENT_USER_ID"] = UserID;

    var myurl = '/api/taskform/pg/pricetemplatemanual';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, bind_price_compare2, '', false, false);

}

function save_price_template_rounting() {
    save_price_template('rounting');
}

function save_price_template(type) {
    if (table_pg_price_template.data().count() == 0) {
        alertError2("Please set price template at least 1 item.");
        return;
    }

    var jsonObj = new Object();
    jsonObj.data = [];
    var lineItemList = [];
    $(".cls_pg_price_template .cls_table_pg_price_template tbody tr:visible").each(function (index) {
        lineItem = {};
        lineItem["PRICE_TEMPLATE_ID"] = $(this).find('.cls_price_template_id').text();
        lineItem["MOCKUP_ID"] = MOCKUPID;
        lineItem["SCALE"] = $(this).find('.cls_scale').autoNumeric('get');
        lineItem["MOCKUP_SUB_ID"] = MainMockupSubId;
        lineItem["CREATE_BY"] = UserID;
        lineItem["UPDATE_BY"] = UserID;
        lineItemList.push(lineItem);
    });
    jsonObj.data = lineItemList;

    var myurl = '/api/taskform/pg/pricetemplate';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (type == 'rounting') {
        myAjax(myurl, mytype, mydata, callback_save_rounting, '', false, false);
    }
    else if (type == 'manual') {
        myAjaxConfirmSubmit(myurl, mytype, mydata, save_price_template_manual, '', false, false);
    }
    else
        myAjax(myurl, mytype, mydata);
}
function save_price_template2() {
    if (table_pg_price_template.data().count() == 0) {
        alertError2("Please set price template at least 1 item.");
        return;
    }

    var jsonObj = new Object();
    jsonObj.data = [];
    var lineItemList = [];
    $(".cls_pg_price_template .cls_table_pg_price_template tbody tr:visible").each(function (index) {
        lineItem = {};
        lineItem["PRICE_TEMPLATE_ID"] = $(this).find('.cls_price_template_id').text();
        lineItem["MOCKUP_ID"] = MOCKUPID;
        lineItem["SCALE"] = $(this).find('.cls_scale').autoNumeric('get');
        lineItem["MOCKUP_SUB_ID"] = MainMockupSubId;
        lineItem["CREATE_BY"] = UserID;
        lineItem["UPDATE_BY"] = UserID;
        lineItemList.push(lineItem);
    });
    jsonObj.data = lineItemList;

    var myurl = '/api/taskform/pg/pricetemplate';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjax(myurl, mytype, mydata, save_price_template_manual, '', false, false);
}

function save_price_template_vendor(type) {

    if (type == 'rounting')
        if (table_user_vendor_price_template.data().count() == 0) {
            alertError2("Please select vendor at least 1 item.");
            return;
        }

    var jsonObj = new Object();
    jsonObj.data = [];
    $(".cls_table_user_vendor_price_template tbody tr:visible").each(function (index) {
        var item = {};
        item["MOCKUP_ID"] = MOCKUPID;
        item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        //item["VENDOR_ID"] = $(this).find('.cls_vendor_id').text();
        //item["USER_ID"] = $(this).find('.cls_user_id').text();
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;
        item["COMMENT_BY_PG"] = $('.cls_div_select_vendor .cls_qua_remark').val();

        item["SPECIAL_COLOR"] = $('.cls_div_select_vendor .cls_qua_special_color').val();
        item["DIRECTION_OF_STICKER"] = $('.cls_div_select_vendor .cls_qua_direction_of_sticker').val();
        item["STYLE_OF_PRINTING"] = $('.cls_div_select_vendor .cls_qua_style_of_printing').val();
        if ($('.cls_div_select_vendor .cls_qua_style_of_printing').val() == -1)
            item["STYLE_OF_PRINTING_OTHER"] = $('.cls_div_select_vendor .cls_input_qua_style_of_printing').val();
        item["SKUS"] = $('.cls_div_select_vendor .cls_qua_est').val();
        item["VOLUME"] = $('.cls_div_select_vendor .cls_qua_est_volume').val();

        //item["PRICE_TEMPLATE_ID"] = $(this).find('.cls_price_template_id').text();
        jsonObj.data.push(item);
    });

    var myurl = '/api/taskform/pg/pricetemplatevendor';
    var mytype = 'POST';
    var mydata = jsonObj;
    if (type == '')
        myAjax(myurl, mytype, mydata);
    else {
        myAjaxConfirmSubmit(myurl, mytype, mydata, save_price_template_rounting, '', false, false);
    }
}

function callback_save_rounting() {
    if ($(".cls_table_user_vendor_price_template tbody tr:visible").length == 0) {
        alertError2("Please select vendor at least 1 item.");
        return;
    }

    var jsonObj = new Object();
    jsonObj.data = [];
    $(".cls_table_user_vendor_price_template tbody tr:visible").each(function (index) {
        var item = {};
        item["MOCKUP_ID"] = MOCKUPID;
        item["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        item["PARENT_MOCKUP_SUB_ID"] = MainMockupSubId;
        item["CURRENT_STEP_ID"] = getstepmockup('SEND_VN_QUO').curr_step;
        item["CURRENT_USER_ID"] = $(this).find('.cls_user_id').text();
        item["CURRENT_VENDOR_ID"] = $(this).find('.cls_vendor_id').text();
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;
        item["REMARK"] = $('.cls_div_select_vendor .cls_qua_remark').val();
        jsonObj.data.push(item);
    });

    var myurl = '/api/taskform/mockupprocess/routingandpricetemplate';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callback_routingandpricetemplate, '', true, true);
}

function callback_routingandpricetemplate() {
    table_user_vendor_price_template.ajax.reload(); bind_price_compare2();
}

function callback_lov_select(obj) {
    bind_table_vendor_user($(obj).val());
    $('.cls_row_select_vendor .cls_lov_search_vendor_price_template').val(null).trigger('change');
}

function bind_table_vendor_user(vendorID) {
    var myurl = '/api/lov/vendoruser_quo?data.vendor_id=' + vendorID + '&data.mockup_id=' + MOCKUPID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, add_row_user_vendor_price_template);
}

function bind_header_quo() {
    var myurl = '/api/taskform/pg/pricetemplatevendor?data.mockup_id=' + MOCKUPID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_header_quo);
}

function callback_bind_header_quo(res) {
    if (res.data.length > 0) {
        if (!isEmpty(res.data[0].SPECIAL_COLOR)) $('.cls_div_select_vendor .cls_qua_special_color').val(res.data[0].SPECIAL_COLOR);
        if (!isEmpty(res.data[0].DIRECTION_OF_STICKER)) setValueToDDL('.cls_div_select_vendor .cls_qua_direction_of_sticker', res.data[0].DIRECTION_OF_STICKER, res.data[0].DIRECTION_OF_STICKER_DISPLAY_TXT);
        if (!isEmpty(res.data[0].STYLE_OF_PRINTING)) setValueToDDL('.cls_div_select_vendor .cls_qua_style_of_printing', res.data[0].STYLE_OF_PRINTING, res.data[0].STYLE_OF_PRINTING_DISPLAY_TXT);
        if (!isEmpty(res.data[0].STYLE_OF_PRINTING_OTHER)) setValueToDDLOther('.cls_div_select_vendor .cls_input_qua_style_of_printing', res.data[0].STYLE_OF_PRINTING_OTHER);
        if (!isEmpty(res.data[0].SKUS)) $('.cls_div_select_vendor .cls_qua_est').val(res.data[0].SKUS);
        if (!isEmpty(res.data[0].VOLUME)) $('.cls_div_select_vendor .cls_qua_est_volume').val(res.data[0].VOLUME);
        if (!isEmpty(res.data[0].COMMENT_BY_PG)) $('.cls_div_select_vendor .cls_qua_remark').val(res.data[0].COMMENT_BY_PG);
    }
}

function add_row_empty_price_compare() {
    var cnt_col = $('.cls_table_pg_price_compare th').length;
    if ($('.cls_table_pg_price_compare tbody tr:visible').length == 0) {
        var row_html = '<tr class="cls_tr_empty"><td style="text-align:center;" colspan="' + cnt_col + '">No data available in table</td></tr>';
        $('.cls_table_pg_price_compare tbody').append(row_html);
    }
}
//function bind_price_compare() {
//    $('.cls_table_pg_price_compare thead tr').remove();
//    $('.cls_table_pg_price_compare tbody tr').remove();

//    var myurl = '/api/taskform/pg/pricetemplatevendor?data.mockup_id=' + MOCKUPID;
//    var mytype = 'GET';
//    var mydata = null;
//    myAjax(myurl, mytype, mydata, callback_bind_price_compare);
//}
//var cnt_td = 1;
//function callback_bind_price_compare(res) {
//    var vendor = '';
//    var row_html = '';
//    cnt_td = 1;
//    row_html = '<tr>';
//    row_html += '<th>Scale</td>';
//    $.each(res.data, function (index, item) {
//        if (vendor.indexOf(item.VENDOR_ID) == -1) {
//            row_html += '<th class="cls_th_vendor_name"><span style="display:none;" class="cls_vendor_id">' + item.VENDOR_ID + '</span>' + item.VENDOR_DISPLAY_TXT + '</th>';
//            vendor = item.VENDOR_ID + '-';
//            cnt_td++;
//        }
//    });
//    row_html += '</tr>';

//    $(row_html).appendTo('.cls_table_pg_price_compare thead')
//    bind_price_compare2();
//}
function bind_price_compare2() {
    var myurl = '/api/taskform/pg/pricetemplatecompare?data.mockup_sub_id=' + MOCKUPSUBID + "&data.mockup_id=" + MOCKUPID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_price_compare2);
}
function callback_bind_price_compare2(res) {
    if (res.data.length > 0)
        $('.cls_div_price_compare .cls_remark_pg_sub_select_vendor_display').val(res.data[0].COMMENT_BY_PG_SUP);

    var vendor_selected_id = 0;
    $.each(res.data, function (index, item) {
        if (item.SELECTED == "X") {
            vendor_selected = true;
            vendor_selected_id = item.VENDOR_ID;
        }
    });

    $('.cls_table_pg_price_compare').remove();
    $('.cls_for_table').append('<table class="cls_table_pg_price_compare"><thead></thead><tbody></tbody></table>');

    //$('.cls_table_pg_price_compare thead tr').remove();
    //$('.cls_table_pg_price_compare tbody tr').remove();

    var MOCKUP_SUB_ID = '';
    var row_html = '';
    cnt_td = 1;
    row_html = '<tr>';
    row_html += '<th>Scale</td>';
    $.each(res.data, function (index, item) {
        if (MOCKUP_SUB_ID.indexOf(item.MOCKUP_SUB_ID) == -1) {
            row_html += '<th class="cls_th_vendor_name"><span style="display:none;" class="cls_round">' + item.ROUND + '</span><span style="display:none;" class="cls_vendor_id">' + item.VENDOR_ID + '</span>' + item.VENDOR_DISPLAY_TXT + " (" + item.ROUND + ")" + '<img style="padding-left:5px;" data-mockup-sub-id="' + item.MOCKUP_SUB_ID + '" class="cls_hand cls_img_delete_price_template_manual" title = "Delete" style="cursor:pointer;" src = "/Content/img/ico_delete.png">' + '</th>';
            MOCKUP_SUB_ID += item.MOCKUP_SUB_ID + '-';
            cnt_td++;
        }
    });
    row_html += '</tr>';

    $(row_html).appendTo('.cls_table_pg_price_compare thead');

    $.each(res.data, function (index, item) {

        var found = false;
        $(".cls_table_pg_price_compare tbody .cls_scale").each(function () {
            if ($(this).text() == setCurrency0(item.SCALE)) { found = true };
        });

        if (!found) {
            var row_html = '<tr>';
            row_html += '<td class="cls_scale">' + setCurrency0(item.SCALE) + '</td>';
            var i = 1;
            while (i < cnt_td) {
                row_html += '<td></td>';
                i++;
            }
            row_html += '</tr>';
            $(row_html).appendTo('.cls_table_pg_price_compare tbody');
        }
    });

    var i = 0;
    $('.cls_table_pg_price_compare thead th').each(function () {
        if (i > 0) {
            var vendor_id = $(this).find('.cls_vendor_id').text();
            var round = $(this).find('.cls_round').text();
            $('.cls_table_pg_price_compare tbody tr').each(function () {
                $(this).find('td').eq(i).text(vendor_id + '-' + round);
            });
        }
        i++;
    });

    $('.cls_table_pg_price_compare tbody tr').each(function () {
        var obj = $(this);
        var scale = $(this).find('.cls_scale').text();
        $(obj).find('td').not(':first').each(function () {
            var obj_td = $(this);
            $.each(res.data, function (index, item) {
                if (setCurrency0(item.SCALE) == scale && item.VENDOR_ID + '-' + item.ROUND == $(obj_td).text()) {
                    $(obj_td).text('');
                    $(obj_td).append("<input data-mockup-sub-id='" + item.MOCKUP_SUB_ID + "' class='form-control cls_price_manual'/>");
                    $(obj_td).find('input').autoNumeric('init', { mDec: '4' });
                    $(obj_td).find('input').autoNumeric('set', item.PRICE);
                    if (item.IS_MANUAL != 'X') {
                        $(obj_td).find('input').attr("disabled", true);
                    }
                }
            });
        });
    });

    $('.cls_table_pg_price_compare tbody tr').each(function () {
        var obj = $(this);
        var scale = $(this).find('.cls_scale').text();
        $(obj).find('td').not(':first').each(function () {
            var obj_td = $(this);
            $.each(res.data, function (index, item) {
                if (item.VENDOR_ID + '-' + item.ROUND == $(obj_td).text()) {
                    $(obj_td).text('');
                    $(obj_td).append("<input data-mockup-sub-id='" + item.MOCKUP_SUB_ID + "' class='form-control cls_price_manual'/>");
                    $(obj_td).find('input').autoNumeric('init', { mDec: '4' });
                    $(obj_td).find('input').autoNumeric('set', 0);
                    if (item.IS_MANUAL != 'X') {
                        $(obj_td).find('input').attr("disabled", true);
                    }
                }
            });
        });
    });

    var vals;
    $('.cls_table_pg_price_compare tbody tr').each(function () {
        var vals = $('td,th', this).map(function () {
            return parseFloat(removeComma($(this).find('input').val()), 10) ? parseFloat(removeComma($(this).find('input').val()), 10) : null;
        }).get();
        // then find their minimum
        var min = Math.min.apply(Math, vals);

        // tag any cell matching the min value
        $('td,th', this).not(':first').filter(function () {
            return parseFloat(removeComma($(this).find('input').val()), 10) === min;
        }).css('background-color', '#80ffaa').find('input').css('background-color', '#80ffaa').css('border-color', '#80ffaa');
    });

    if (res.data.length > 0) {
        var row_html2 = '<tr style="font-weight: bold;"><td>Comment</td>';
        var old_vendor_id = '';
        $.each(res.data, function (index, item) {
            if (!isEmpty(item.VENDOR_ID)) {
                if (old_vendor_id != item.VENDOR_ID + '-' + item.ROUND) {
                    if (!isEmpty(item.COMMENT_BY_VENDOR)) {
                        if (item.IS_MANUAL == 'X') {
                            row_html2 += '<td><input type="text" class="cls_input_comment_manual form-control" value="' + item.COMMENT_BY_VENDOR + '" /></td>';
                        }
                        else if (item.IS_MANUAL != 'X') {
                            row_html2 += '<td><input style="background-color:transparent;border-color:transparent;cursor:default;box-shadow:none;" disabled="disabled" type="text" class="form-control" value="' + item.COMMENT_BY_VENDOR + '" /></td>';
                        }
                    }
                    else {
                        if (item.IS_MANUAL == 'X') {
                            row_html2 += '<td><input type="text" class="cls_input_comment_manual form-control" value="' + '' + '" /></td>';
                        }
                        else if (item.IS_MANUAL != 'X') {
                            row_html2 += '<td><input style="background-color:transparent;border-color:transparent;cursor:default;box-shadow:none;" disabled="disabled" type="text" class="form-control" value="' + '' + '" /></td>';
                        }
                    }
                    old_vendor_id = item.VENDOR_ID + '-' + item.ROUND;
                }
            }
        });
        row_html2 += '</tr>';
        $(row_html2).appendTo('.cls_table_pg_price_compare tbody');
    }

    $('.cls_table_pg_price_compare').DataTable({
        columnDefs: [
            { "orderable": true, type: 'sort-numbers-ignore-text', targets: 0 },
            { "orderable": false, targets: '_all' },
        ],
        "info": false,
        "searching": false,
        "paging": false,
    });

    table_vendor.ajax.reload();
    //add_row_empty_price_compare();

    var cntTh = $('.cls_table_pg_price_compare thead th').length;
    $('.cls_table_pg_price_compare thead th').each(function () {
        $(this).css('width', 100 / cntTh);
    });

    if (vendor_selected) {
        $("th .cls_vendor_id").each(function (index) {
            if ($(this).text() == vendor_selected_id) {
                $(this).closest('th').append('<img title="Selected" class="cls_sleected_vendor_img" style="width:15px;" src="/Content/img/ico_true.png">');
            }
        });

        $(".cls_sleected_vendor_img:not(:last)").each(function (index) {
            $(this).remove();
        });

        //$('.cls_table_pg_price_compare .cls_input_comment_manual').prop('disabled', true);
        //$('.cls_div_price_compare .cls_price_manual').prop('disabled', true);
        //$('.cls_div_price_compare .cls_img_delete_price_template_manual').hide();
        //$('.cls_div_price_compare .cls_row_manual_add_vendor').hide();

        //$('.cls_table_user_vendor_price_template').hide();

        //$('.cls_pg_price_template .cls_div_price_template').hide();

        $('.cls_pg_price_template .cls_btn_request_quotation_price_template').hide();
        $('.cls_pg_price_template .cls_btn_reset_vendor_price_template').hide();
        //$('.cls_pg_price_template .cls_btn_save_vendor_price_template').hide();
        $('.cls_pg_price_template .cls_row_select_vendor').hide();
    }
}

var table_vendor;
function bind_vendor() {
    table_vendor = $('.cls_table_select_vendor_manager').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pg/pricetemplatevendortran?data.mockup_id=' + MOCKUPID,
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
        },
        {
            "searchable": false,
            "orderable": false,
            "targets": 1
        }],
        "order": [[2, 'asc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": false,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    if (row.CANSELECT == "0") return '';
                    else return '<input type="radio" value="' + row.MOCKUPSUBID2 + '" name="select_vendor_by_manager">'
                }
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "USER_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "EMAIL", "className": "cls_nowrap" },
            { "data": "USER_ID", "className": "cls_hide cls_user_id" },
            { "data": "VENDOR_ID", "className": "cls_hide cls_vendor_id" },
        ],
        "rowCallback": function (row, data, index) {

        },
    });

    table_vendor.on('order.dt search.dt', function () {
        table_vendor.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

var table_pg_price_template;
function bind_table_pg_price_template() {
    table_pg_price_template = $('.cls_table_pg_price_template').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pg/pricetemplate?data.mockup_id=' + MOCKUPID,
                type: 'GET',
                success: function (res) {
                    res = DData(res);
                    if (res.status == "E") {
                        $('.dataTables_processing').hide();
                        if (res.msg != '')
                            alertError(res.msg);
                    }
                    else if (res.status == "S") {
                        callback(res);

                        if (res.data.length == 0) {
                            add_row_price_template(1000);
                            add_row_price_template(3000);
                            add_row_price_template(5000);
                            add_row_price_template(7000);
                            add_row_price_template(10000);
                            add_row_price_template(30000);
                            add_row_price_template(50000);
                            add_row_price_template(70000);
                            add_row_price_template(100000);
                        }
                    }
                }
            });
        },
        "columnDefs": [{
            "searchable": false,
            "orderable": false,
            "targets": 0, "width": 10
        }],
        "order": [[1, 'asc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": false,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return '<img class="cls_img_lov_delete_row_price_template" title="Delete" style="cursor:pointer;" src="/Content/img/ico_delete.png" />'
                },
            },
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_scale form-control" type="text" value="' + row.SCALE + '">'
                },
                "orderDataType": "dom-text-numeric",
            },
            { "data": "PRICE_TEMPLATE_ID", "className": "cls_hide cls_price_template_id" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(1).find('.cls_scale').autoNumeric('init', { mDec: '0' });
            if (isEmpty(data.SCALE)) {
                $(row).find('td').eq(1).find('.cls_scale').autoNumeric('set', 0);
            }
            else {
                $(row).find('td').eq(1).find('.cls_scale').autoNumeric('set', data.SCALE);
            }
        },
        "initComplete": function (settings, json) {

        }
    });
}
function add_row_price_template(SCALE) {
    table_pg_price_template.rows.add([{
        "SCALE": SCALE
    }]).draw(false);
}

var table_user_vendor_price_template;
function bind_user_vendor_price_template() {
    table_user_vendor_price_template = $('.cls_table_user_vendor_price_template').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pg/pricetemplatevendortran?data.mockup_id=' + -1,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [{
            "searchable": false,
            "orderable": false,
            "targets": 0, "width": 10
        }, {
            "searchable": false,
            "orderable": false,
            "targets": 1, "width": 20
        }],
        "order": [[2, 'asc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": false,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return '<img class="cls_img_lov_delete_row_user_vendor" title="Delete" style="cursor:pointer;" src="/Content/img/ico_delete.png" />'
                }
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "USER_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "EMAIL", "className": "cls_nowrap" },
            { "data": "USER_ID", "className": "cls_hide cls_user_id" },
            { "data": "VENDOR_ID", "className": "cls_hide cls_vendor_id" },
            { "data": "SELECTED", "className": "cls_hide cls_selected_vendor" },
        ],
        "rowCallback": function (row, data, index) {

        },
        "drawCallback": function (settings, json) {

        }
    });

    table_user_vendor_price_template.on('order.dt search.dt', function () {
        table_user_vendor_price_template.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}
var vendor_selected = false;
function add_row_user_vendor_price_template(res) {
    if (res.data.length > 0) {
        $.each(res.data, function (index, item) {
            var doWork = true;
            $('.cls_table_user_vendor_price_template  > tbody  > tr').each(function () {
                if ($(this).find('.cls_user_id').text() == item.USER_ID) {
                    doWork = false;
                }
            });

            if (doWork) {
                table_user_vendor_price_template.rows.add([{
                    "VENDOR_DISPLAY_TXT": item.VENDOR_DISPLAY_TXT,
                    "USER_DISPLAY_TXT": item.USER_DISPLAY_TXT,
                    "EMAIL": item.EMAIL,
                    "USER_ID": item.USER_ID,
                    "VENDOR_ID": item.VENDOR_ID,
                }]).draw(false);
            }
        });
    }
}