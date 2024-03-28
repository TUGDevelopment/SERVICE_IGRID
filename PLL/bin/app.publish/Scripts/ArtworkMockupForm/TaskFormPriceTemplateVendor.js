$(document).ready(function () {
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_price_template_vendor':
                load_pricetemplate_vendor();
                break;
            default:
                break;
        }
    });

    $(".cls_pg_price_template_vendor .cls_btn_reset_price_template_vendor").click(function () {
        table_pg_price_template_vendor.ajax.reload();
    });

    $(".cls_pg_price_template_vendor .cls_btn_save_price_template_vendor").click(function () {
        save_price_template_vendor2('');
    });

    $(".cls_pg_price_template_vendor .cls_btn_submit_quotation_price_template").click(function () {
        save_price_template_vendor2('endtask');
    });

    $(document).on("focus", ".cls_pg_price_template_vendor .cls_price", function () {
        $(this).select();
    });
});

function load_pricetemplate_vendor() {
    bindDataForQuoVendorTab();
    if (table_pg_price_template_vendor == null)
        bind_table_pg_price_template_vendor();
    else
        table_pg_price_template_vendor.ajax.reload();
}

function save_price_template_vendor2(type) {
    var jsonObj = new Object();
    jsonObj.data = [];
    if (type == 'endtask') jsonObj.ENDTASKFORM = true;
    jsonObj.COMMENT_BY_VENDOR = $('.cls_div_price_template_vendor .cls_remark_qua_vendor').val();
    jsonObj.MOCKUP_ID = MOCKUPID;
    jsonObj.MOCKUP_SUB_ID = MOCKUPSUBID;
    jsonObj.CREATE_BY = UserID;
    jsonObj.UPDATE_BY = UserID;
    var lineItemList = [];
    $(".cls_pg_price_template_vendor .cls_table_pg_price_template_vendor tbody tr").each(function (index) {
        lineItem = {};
        lineItem["PRICE_TEMPLATE_ID"] = $(this).find('.cls_price_template_id').text();
        lineItem["MOCKUP_ID"] = MOCKUPID;
        lineItem["SCALE"] = $(this).find('.cls_scale').autoNumeric('get');
        lineItem["PRICE"] = $(this).find('.cls_price').autoNumeric('get');
        lineItem["MOCKUP_SUB_ID"] = MOCKUPSUBID;
        lineItem["CREATE_BY"] = UserID;
        lineItem["UPDATE_BY"] = UserID;
        lineItemList.push(lineItem);
    });
    jsonObj.data = lineItemList;

    var myurl = '/api/taskform/pg/pricetemplate';
    var mytype = 'POST';
    var mydata = jsonObj;

    if (type == 'endtask') {
        myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', false, false);
    }
    else
        myAjax(myurl, mytype, mydata);
}

var table_pg_price_template_vendor;
function bind_table_pg_price_template_vendor() {
    table_pg_price_template_vendor = $('.cls_table_pg_price_template_vendor').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pg/pricetemplateforvendor?data.mockup_sub_id=' + MOCKUPSUBID,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                    if (res.status == "S") {
                        //callback(res);

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
            "targets": 0
        }],
        "order": [[0, 'asc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": false,
        "columns": [

            {
                render: function (data, type, row, meta) {
                    return '<input disabled class="cls_scale form-control" type="text" value="' + row.SCALE + '">'
                },
                "orderDataType": "dom-text-numeric"
            },
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_price form-control" type="text" value="' + row.PRICE + '">'
                },
                "orderDataType": "dom-text-numeric"
            },
            { "data": "PRICE_TEMPLATE_ID", "className": "cls_hide cls_price_template_id" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(0).find('.cls_scale').autoNumeric('init', { mDec: '0' });
            $(row).find('td').eq(1).find('.cls_price').autoNumeric('init', { mDec: '4' });
            if (isEmpty(data.SCALE)) {
                $(row).find('td').eq(0).find('.cls_scale').autoNumeric('set', 0);
            }
            else {
                $(row).find('td').eq(0).find('.cls_scale').autoNumeric('set', data.SCALE);
            }

            if (isEmpty(data.PRICE)) {
                $(row).find('td').eq(1).find('.cls_price').autoNumeric('set', 0);
            }
            else {
                $(row).find('td').eq(1).find('.cls_price').autoNumeric('set', data.PRICE);
            }
        },
    });
}

function bindDataForQuoVendorTab() {
    var myurl = '/api/taskform/pg/pricetemplatevendor?data.mockup_id=' + MOCKUPID + "&data.user_id=" + UserID;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bindDataForQuoVendorTab);
}

function callback_bindDataForQuoVendorTab(res) {
    var data = res.data[0];
    $('.cls_pg_price_template_vendor .cls_qua_special_color').val(data.SPECIAL_COLOR);
    $('.cls_pg_price_template_vendor .cls_qua_direction_of_sticker').val(data.DIRECTION_OF_STICKER_DISPLAY_TXT);
    $('.cls_pg_price_template_vendor .cls_qua_style_of_printing').val(data.STYLE_OF_PRINTING_DISPLAY_TXT);
    if (!isEmpty(data.STYLE_OF_PRINTING_OTHER_DISPLAY_TXT)) $('.cls_pg_price_template_vendor .cls_qua_style_of_printing').val(data.STYLE_OF_PRINTING_OTHER_DISPLAY_TXT);

    $('.cls_pg_price_template_vendor .cls_qua_est').val(data.SKUS);
    $('.cls_pg_price_template_vendor .cls_qua_est_volume').val(data.VOLUME);
    $('.cls_pg_price_template_vendor .cls_qua_remark').val(data.COMMENT_BY_PG);
}