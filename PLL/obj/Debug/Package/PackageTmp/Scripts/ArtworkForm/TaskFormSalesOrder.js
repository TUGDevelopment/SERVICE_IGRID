var first_load_salesorder = true;
var Bom_sales = "";
$(document).ready(function () {
    $('[data-toggle="popover"]').popover();
    $(".cls_tfartwork_salesorder .cls_btn_tfartwork_salesorder").click(function (e) {
        $('#modal_tfartwork_salesorder .cls_lbl_assign_so_sold_to_name').val($('.cls_artwork_request_form .cls_lov_artwork_sold_to option:selected').text());
        $('#modal_tfartwork_salesorder .cls_lbl_assign_so_ship_to_name').val($('.cls_artwork_request_form .cls_lov_artwork_ship_to option:selected').text());

        if ($('.cls_artwork_request_form .cls_lov_artwork_brand_other').val() == -1) {
            $('#modal_tfartwork_salesorder .cls_lbl_assign_so_brand_name').val($('.cls_artwork_request_form .cls_input_artwork_brand_other').val());
        }
        else {
            $('#modal_tfartwork_salesorder .cls_lbl_assign_so_brand_name').val($('.cls_artwork_request_form .cls_lov_artwork_brand_other option:selected').text());
        }

        var table = $('#table_tfartwork_salesorder').DataTable();
        var tblData = table.rows().data();
        var data = setSelectSalesOrderItem(tblData);
        bindSalesOrderItemPopUp(data);
        bindFOCItemPopUp(data);

        $('#modal_tfartwork_salesorder').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $(".cls_tfartwork_salesorder .cls_btn_tfartwork_salesorder_clear").click(function (e) {
        deleteSalesorderitem('');
    });

    $("#modal_tfartwork_salesorder input[name=filter_so]").click(function (e) {
        if ($("#modal_tfartwork_salesorder input[name=filter_so]:checked").val() == '0') {
            table_tfartwork_salesorder_select.ajax.reload();
            $(".cls_row_foc_select").hide();
            $(".cls_row_salesorder_select").show();
            $(".cls_btn_tfartwork_salesorder_select").show();
            $(".cls_btn_tfartwork_foc_select").hide();
        }
        else {
            table_tfartwork_foc_select.ajax.reload();
            $(".cls_row_salesorder_select").hide();
            $(".cls_row_foc_select").show();
            $(".cls_btn_tfartwork_salesorder_select").hide();
            $(".cls_btn_tfartwork_foc_select").show();
        }
    });

    $(".cls_tfartwork_salesorder .cls_btn_tfartwork_salesorder_viewchange").click(function (e) {
        bindSalesOrderViewChangePopUp();
        $('#modal_tfartwork_salesorder_viewchange').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $("#modal_tfartwork_salesorder_viewchange .cls_btn_tfartwork_salesorder_viewchange_accept").click(function (e) {
        var jsonObj = new Object();
        jsonObj.data = {};
        var item = {};

        item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
        item["ARTWORK_SUB_ID"] = ArtworkSubId;
        item["UPDATE_BY"] = UserID;

        jsonObj.data = item;

        var myurl = '/api/taskform/salesorderitem/acceptchange';
        var mytype = 'POST';
        var mydata = jsonObj;

        myAjaxConfirmSubmit(myurl, mytype, mydata, callback_saleorder_acceptchange);
    });

    $("#modal_tfartwork_salesorder .cls_btn_tfartwork_salesorder_select").click(function (e) {
        var table = $('#table_tfartwork_salesorder_select').DataTable();
        var tblData = table.rows('.selected').data();
        var data = setSelectSalesOrderItem(tblData);
        if (data.length > 0) {
            validateSelectSalesOrderItem(data);
        }
        else {
            alertError2("Please select at least 1 item.");
        }
    });

    $("#modal_tfartwork_salesorder .cls_btn_tfartwork_foc_select").click(function (e) {
        var table = $('#table_tfartwork_foc_select').DataTable();
        var tblData = table.rows('.selected').data();
        var data = setSelectFOC(tblData);
        if (data.length > 0) {
            bindSelectFOC(data);
            $("#modal_tfartwork_salesorder .cls_btn_tfartwork_salesorder_close").click();
        }
        else {
            alertError2("Please select at least 1 item.");
        }
    });

    $('#modal_tfartwork_salesorder').on('hide.bs.modal', function () {
        var table_tfartwork_salesorder_select = $('#table_tfartwork_salesorder_select').DataTable();
        table_tfartwork_salesorder_select.destroy();
        var table_tfartwork_foc_select = $('#table_tfartwork_foc_select').DataTable();
        table_tfartwork_foc_select.destroy();
        first_load_salesorder = false;
    })

    bindTaskFormSalesOrder();
});

var table_tfartwork_salesorder;
function bindTaskFormSalesOrder() {
    // Setup - add a text input to each footer cell
    $('#table_tfartwork_salesorder thead tr').clone(true).appendTo('#table_tfartwork_salesorder thead');
    $('#table_tfartwork_salesorder thead tr:eq(1) th').each(function (i) {
        //if (ReadOnly == "1" || REQUEST_MATERIAL == "1" || CURRENT_STEP_CODE_DISPLAY_TXT != 'SEND_PA') {
        //    if (i == 0) {
        //        $(this).html('');
        //    } else if (i == 11) {
        //        $(this).html('<input type="text" placeholder="dd/mm/yyyy" class="form-control" data-index="' + i + '" />');
        //    } else {
        //        var title = $(this).text();
        //        $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
        //    }
        //}
        //else {
        if (i == 0 || i == 1 || i == 2) {
            $(this).html('');
        } else if (i == 13) {
            $(this).html('<input type="text" placeholder="dd/mm/yyyy" class="form-control" data-index="' + i + '" />');
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
        }
        //}
    });

    table_tfartwork_salesorder = $('#table_tfartwork_salesorder').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        "scrollX": true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/taskform/salesorderitem/get?data.artwork_request_id=" + ARTWORK_REQUEST_ID + "&data.artwork_sub_id=" + MainArtworkSubId,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                    if (res.status == "S") {
                        //callback(res);

                        if (res.data.length == 0) {
                            Bom_sales = "";
                        }
                    }
                }
            });
        },
        "columnDefs": [
            { "orderable": false, "targets": 0 },
            { "orderable": false, "targets": 1 },
            { "orderable": false, "targets": 2 },
        ],
        "order": [[3, 'asc']],
        columns: [
            {
                "className": 'delete-control',
                "orderable": false,
                "data": null,
                "defaultContent": '',
                visible: !(ReadOnly == "1" || CURRENT_STEP_CODE_DISPLAY_TXT != 'SEND_PA')
            },
            {
                "className": 'details-control',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            //{ data: "GROUPING_DISPLAY_TXT", "className": "cls_nowrap", "orderable": false },
            { data: "SO_NUMBER", "className": "cls_nowrap" },
            { data: "SO_ITEM_NO", "className": "cls_nowrap" },
            { data: "MATERIAL_NO", "className": "cls_nowrap" },
            { data: "MATERIAL_DESC", "className": "cls_nowrap" },
            { data: "PRODUCTION_PLANT", "className": "cls_nowrap" },
            { data: "ORDER_BOM_NO", "className": "cls_nowrap" },
            { data: "ORDER_BOM_DESC", "className": "cls_nowrap" },
            { data: "FOC_ITEM", "className": "cls_nowrap" },
            { data: "QUANTITY", "className": "cls_nowrap" },
            { data: "STOCK_PO", "className": "cls_nowrap" },
            { data: "RDD_DISPLAY_TXT", "className": "cls_rdd cls_nowrap" },
            {
                data: "ASSIGN_ID",
                visible: false
            }
        ],
        "rowCallback": function (row, data, index) {
            //Set value for Vendor tab
            if ($('.cls_vendor_txt_order_no').val() != "") {
                $('.cls_vendor_txt_order_no').val($('.cls_vendor_txt_order_no').val() + ", " + data.SALES_ORDER_NO + ' (' + data.SALES_ORDER_ITEM + ')');
            }
            else {
                $('.cls_vendor_txt_order_no').val(data.SALES_ORDER_NO + ' (' + data.SALES_ORDER_ITEM + ')');
            }

            Bom_sales = data.ORDER_BOM_NO;

            var mat5FromWF = $('.cls_txt_header_tfartwork_mat_no').val();
            if (!isEmpty(data.ORDER_BOM_NO) && !isEmpty(mat5FromWF)) {
                if (mat5FromWF != data.ORDER_BOM_NO) {
                    $('.cls_txt_header_tfartwork_mat_no').css('background-color', 'lightskyblue');
                    $('.cls_txt_header_tfartwork_mat_no').css('color', 'black');
                }
                else {
                    $('.cls_txt_header_tfartwork_mat_no').css('background-color', '#F8F8F8');
                    $('.cls_txt_header_tfartwork_mat_no').css('color', '#555555');
                }
            }
        }
    });

    // Add event listener for opening and closing details
    $('#table_tfartwork_salesorder tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = table_tfartwork_salesorder.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            row.child(formatSalesOrderItem(row.data())).show();
            tr.addClass('shown');
        }
    });

    // Add event listener for opening and closing details
    $('#table_tfartwork_salesorder tbody').on('click', 'td.delete-control', function () {
        var tr = $(this).closest('tr');
        var row = table_tfartwork_salesorder.row(tr);
        var d = row.data();

        deleteSalesorderitem(d.ARTWORK_PROCESS_SO_ID);
    });

    $("a[href='#view_salesorder']").on('shown.bs.tab', function (e) {
        table_tfartwork_salesorder.columns.adjust().draw();
    });

    $(table_tfartwork_salesorder.table().container()).on('keyup', 'input', function () {
        table_tfartwork_salesorder
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    table_tfartwork_salesorder.on('order.dt search.dt', function () {
        table_tfartwork_salesorder.column(2, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}

function deleteSalesorderitem(artwork_process_so_id) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    if (artwork_process_so_id != '') {
        item["ARTWORK_PROCESS_SO_ID"] = artwork_process_so_id;
    }
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["UPDATE_BY"] = UserID;
    jsonObj.data = item;

    var myurl = '/api/taskform/salesorderitem/delete';
    var mytype = 'DELETE';
    var mydata = jsonObj;
    myAjaxConfirmDelete(myurl, mytype, mydata, callback_delete_salesorderitem);
}

function callback_delete_salesorderitem(res) {

    if (res.status == "E" && res.msg != '') {
        alertError(res.msg);
    }
    else if (res.status == "S") {
        Bom_sales = "";
        bindTaskformArtwork(ArtworkSubId);
        if (table_tfartwork_salesorder == null)
            bindTaskFormSalesOrder();
        else
            table_tfartwork_salesorder.ajax.reload();
        $('a[data-toggle="tab"][href="#view_salesorder"]').tab('show');
    }
    bindFOC(ArtworkSubId);
}

function callback_saleorder_acceptchange(res) {
    if (res.status == "E" && res.msg != '') {
        alertError(res.msg);
    }
    else if (res.status == "S") {
        bindTaskformArtwork(ArtworkSubId);
        $("#modal_tfartwork_salesorder_viewchange .cls_btn_tfartwork_salesorder_viewchange_close").click();
        if (table_tfartwork_salesorder == null)
            bindTaskFormSalesOrder();
        else
            table_tfartwork_salesorder.ajax.reload();
        $('a[data-toggle="tab"][href="#view_salesorder"]').tab('show');
    }
}

function formatSalesOrderItem(d) {
    // `d` is the original data object for the row
    return '<table cellpadding="12" cellspacing="0" border="0" style="padding-left:50px;">' +
        getTemplateRowSaleOrderItem('Order Qty.', d.ORDER_QTY) +
        getTemplateRowSaleOrderItem('Create Date', d.CREATE_DATE_DISPLAY_TXT) +
        getTemplateRowSaleOrderItem('RDD', d.RDD_DISPLAY_TXT) +
        getTemplateRowSaleOrderItem('ETD Date from', d.ETD_DATE_FROM) +
        getTemplateRowSaleOrderItem('ETD Date to', d.ETD_DATE_TO) +
        getTemplateRowSaleOrderItem('L / C Last shipment Date 1 - 2', d.LC) +
        getTemplateRowSaleOrderItem('Sold - To - Party', d.SOLD_TO_PARTY) +
        getTemplateRowSaleOrderItem('Ship - To - Party', d.SHIP_TO_PARTY) +
        getTemplateRowSaleOrderItem('Material', d.MATERIAL) +
        getTemplateRowSaleOrderItem('Old Material', d.OLD_MATERIAL) +
        getTemplateRowSaleOrderItem('Plant', d.PLANT) +
        getTemplateRowSaleOrderItem('Size / Drain wt.', d.DRAIN_WEIGHT) +
        getTemplateRowSaleOrderItem('Prod / Insp.Memo', d.INSP_MEMO) +
        getTemplateRowSaleOrderItem('Reason for Rejection', d.REASON_REJECTION) +
        getTemplateRowSaleOrderItem('Pack size', d.PACK_SIZE) +
        getTemplateRowSaleOrderItem('Brand', d.BRAND) +
        getTemplateRowSaleOrderItem('Additional Brand', d.BRAND_ADDITIONAL) +
        getTemplateRowSaleOrderItem('Sold - To P.O.', d.SOLD_TO_PO) +
        getTemplateRowSaleOrderItem('Ship - To P.O.', d.SHIP_TO_PO) +
        getTemplateRowSaleOrderItem('Port / Country / Zone', d.PORT) +
        getTemplateRowSaleOrderItem('Via', d.VIA) +
        getTemplateRowSaleOrderItem('In Transit to', d.IN_TRANSIT_TO) +
        getTemplateRowSaleOrderItem('Payment Term', d.PAYMENT_TERM) +
        getTemplateRowSaleOrderItem('L / C No.', d.LC_NO) +
        getTemplateRowSaleOrderItem('Exp.', d.EXP) +
        getTemplateRowSaleOrderItem('Item PKG. & Warehouse Text', d.WAREHOUSE_TEXT) +
        getTemplateRowSaleOrderItem('General Text', d.GENERAL_TEXT) +
        '</table>';
}

function getTemplateRowSaleOrderItem(columnname, columntext) {
    if (columntext == null) {
        columntext = '';
    }
    return ('<tr>' +
        '<td>' + columnname + ' :</td>' +
        '<td>' + columntext + '</td>' +
        '</tr>');
}

//function addSalesOrderItem(data) {
//    var obj = $('.tr_tfartwork_first_salesorder_item').clone().removeClass('tr_tfartwork_first_salesorder_item').removeClass('cls_cn_hide');

//    $('.cls_tfartwork_salesorder .table_tfartwork_salesorder tbody').append(obj);
//    var obj2 = $('.cls_tfartwork_salesorder .table_tfartwork_salesorder tbody tr:last');

//    obj.find('.cls_td_tfartwork_so').text(data.SO_NUMBER == null ? "" : data.SO_NUMBER);
//    obj.find('.cls_td_tfartwork_so_item').text(data.SO_ITEM_NO == null ? "" : data.SO_ITEM_NO);
//    obj.find('.cls_td_tfartwork_material_no').text(data.MATERIAL_NO == null ? "" : data.MATERIAL_NO);
//    obj.find('.cls_td_tfartwork_material_desc').text(data.MATERIAL_DESC == null ? "" : data.MATERIAL_DESC);
//    obj.find('.cls_td_tfartwork_production_plan').text(data.PRODUCTION_PLANT == null ? "" : data.PRODUCTION_PLANT);
//    obj.find('.cls_td_tfartwork_bom_component').text(data.ORDER_BOM_NO == null ? "" : data.ORDER_BOM_NO);
//    obj.find('.cls_td_tfartwork_bom_component_desc').text(data.ORDER_BOM_DESC == null ? "" : data.ORDER_BOM_DESC);
//    obj.find('.cls_td_tfartwork_quantity').text(data.QUANTITY == null ? "" : data.QUANTITY);
//    obj.find('.cls_td_tfartwork_stock_po').text(data.STOCK_PO == null ? "" : data.STOCK_PO);
//    obj.find('.cls_td_tfartwork_delivery_date').text(data.RDD == null ? "" : data.RDD);

//    $(".cls_img_tfartwork_delete_salesorder_item").click(function () {
//        $(this).closest('tr').remove();
//    });
//}

function bindSalesOrderItemPopUp(dataFilter) {
    $('#filter_tfartwork_salesorder').val("");
    if ($(".cls_img_tfartwork")[0] != undefined) {
        //resize image algorithm 
        var maxWidth = 300;
        var maxHeight = 300;
        var ratio = 0;
        var width = $(".cls_img_tfartwork")[0].width;
        var height = $(".cls_img_tfartwork")[0].height;

        if (width > maxWidth) {
            ratio = maxWidth / width;

            $(".cls_img_tfartwork").attr('width', maxWidth);
            $(".cls_img_tfartwork").attr('height', height * ratio);
        }

        else if (height > maxHeight) {
            ratio = maxHeight / height;

            $(".cls_img_tfartwork").attr('width', width * ratio);
            $(".cls_img_tfartwork").attr('height', maxHeight);
        }
    }
    if (first_load_salesorder) {
        // Setup - add a text input to each footer cell
        $('#table_tfartwork_salesorder_select thead tr').clone(true).appendTo('#table_tfartwork_salesorder_select thead');
        $('#table_tfartwork_salesorder_select thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 1 || i == 18) {
                $(this).empty();
            } else if (i == 19) {
                $(this).html('<input type="text" placeholder="dd/mm/yyyy" class="form-control" data-index="' + i + '" />');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });
    }

    var groupColumn = 3;
    table_tfartwork_salesorder_select = $('#table_tfartwork_salesorder_select').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/taskform/salesorderitem/popup?data.artwork_sub_id=" + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "visible": false, "targets": groupColumn },
            {
                "searchable": false,
                "orderable": false,
                "targets": 0
            },
            { "orderable": false, "targets": 1 },
            { "orderable": false, "targets": 18 },
        ],
        columns: [
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_chk_tfsaleorder" type="checkbox">';
                }
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "ASSIGN_ID",
                visible: false
            },
            { data: "GROUPING", "className": "cls_nowrap" },
            { data: "GROUPING_DISPLAY_TXT", "className": "cls_nowrap cls_hide" },
            //{ data: "SOLD_TO_NAME", "className": "cls_nowrap" },
            //{ data: "SHIP_TO_NAME", "className": "cls_nowrap" },
            { data: "SALES_ORG", "className": "cls_nowrap" },
            { data: "SO_NUMBER", "className": "cls_nowrap" },
            { data: "SO_ITEM_NO", "className": "cls_nowrap" },
            //{ data: "BRAND", "className": "cls_nowrap" },
            { data: "RDD", "className": "cls_nowrap cls_rdd" },
            { data: "MATERIAL_NO", "className": "cls_nowrap" },
            { data: "PORT", "className": "cls_nowrap" },
            { data: "PRODUCTION_PLANT", "className": "cls_nowrap" },
            { data: "ORDER_BOM_NO", "className": "cls_nowrap" },
            { data: "ORDER_BOM_DESC", "className": "cls_nowrap" },
            { data: "BOM_ITEM_CUSTOM_1", "className": "cls_nowrap" },
            { data: "QUANTITY", "className": "cls_nowrap" },
            { data: "STOCK_PO", "className": "cls_nowrap" },
            { data: "ORDER_BOM_ID", "className": "cls_nowrap cls_hide" },
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_chk_tfsaleorder" type="checkbox">';
                }
            },
        ],
        "scrollX": true,
        "scrollY": "350px",
        "scrollCollapse": true,
        "paging": false,
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var j = 1;
            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                var str_moredetails = "";
                var str_grouping = "";
                for (var x = 0; x < rows.data().length; x++) {
                    if (rows.data()[x].GROUPING == group) {
                        str_moredetails = rows.data()[x].GROUPING_DISPLAY_TXT;
                        str_grouping = rows.data()[x].GROUPING;
                    }
                }

                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group highlight"><td><input data-group-name="group-' + str_grouping + '" class="cls_chk_tfsaleorder_grouping" type="checkbox"></td>  <td colspan="16"> Group ' + (j) + ' <span title="' + str_moredetails + '" class="cls_hand label label-info">more details</span></td></tr>'
                    );

                    last = group;

                    j++;
                }
            });
        },
        select: {
            'style': 'multi',
            selector: 'td:first-child input,td:last-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_rdd').html(myDateMoment(data.RDD));

            //if (data.MATERIAL_NO.match("^3")) {
            //    // do this if begins with Hello
            //    $(row).addClass('group-' + data.GROUPING);
            //}
            //else {
            //    $(row).find('.cls_chk_tfsaleorder').remove();
            //}

            if (data.MATERIAL_NO.startsWith("5") && !isEmpty(data.ITEM_CUSTOM_1) && data.ITEM_CUSTOM_1 != '0') {
                //FOC
                $(row).find('.cls_chk_tfsaleorder').remove();
            }
            else {
                $(row).addClass('group-' + data.GROUPING);
            }
        },
        //order: [[2, 'asc'], [4, 'asc']],
        order: [6, 'asc'],
        "orderFixed": [3, 'asc'],
        initComplete: function (settings, json) {

        }
    });

    $(document).on('click', '#modal_tfartwork_salesorder .cls_chk_tfsaleorder', function (e) {
        if ($(this).is(':checked')) {
            table_tfartwork_salesorder_select.rows($(this).closest('tr')).select();
        }
        else {
            table_tfartwork_salesorder_select.rows($(this).closest('tr')).deselect();
        }
        $(this).closest('tr').find('.cls_chk_tfsaleorder').prop('checked', this.checked);
    });

    $(table_tfartwork_salesorder_select.table().container()).on('keyup', 'input', function () {
        table_tfartwork_salesorder_select
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    $("#table_tfartwork_salesorder_select_filter").hide();

    table_tfartwork_salesorder_select.on('order.dt search.dt', function () {
        table_tfartwork_salesorder_select.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();

    $(document).on('click', '.cls_chk_tfsaleorder_grouping', function (e) {
        // Get group class name
        var groupName = $(this).data('group-name');
        var tempCheck = this.checked;

        // Select all child rows
        //table_tfartwork_salesorder_select.rows('tr.' + groupName).select(this.checked);
        //$('input', table_tfartwork_salesorder_select.cells('tr.' + groupName, 0).nodes()).prop('checked', this.checked);

        var temp = table_tfartwork_salesorder_select.rows({ search: 'applied' });
        temp.rows('tr.' + groupName).every(function (rowIdx, tableLoop, rowLoop) {
            var rowNode = this.node();
            if ($(rowNode).is(':visible')) {
                table_tfartwork_salesorder_select.row(rowIdx).select(tempCheck);
                $(rowNode).find('.cls_chk_tfsaleorder').prop('checked', tempCheck);
            }
        });
    });

    if (dataFilter.length > 0) {
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                for (i = 0; i < dataFilter.length; i++) {
                    if (data[1] == dataFilter[i].ASSIGN_ID) {
                        return false;
                    }
                }
                return true;
            }
        );
    }
    else {
        $.fn.dataTable.ext.search.pop();
    }

    $('input.filter_tfartwork_salesorder').on('keyup click', function () {
        $('#table_tfartwork_salesorder_select').DataTable().search(
            $('#filter_tfartwork_salesorder').val()
        ).draw();
    });
}

function bindFOCItemPopUp(dataFilter) {
    $('#filter_tfartwork_salesorder').val("");

    if (first_load_salesorder) {
        // Setup - add a text input to each footer cell
        $('#table_tfartwork_foc_select thead tr').clone(true).appendTo('#table_tfartwork_foc_select thead');
        $('#table_tfartwork_foc_select thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 1 || i == 2) {
                $(this).empty();
            }
            else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });
    }

    table_tfartwork_foc_select = $('#table_tfartwork_foc_select').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/taskform/salesorderfoc/popup?data.artwork_sub_id=" + ArtworkSubId,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            {
                "searchable": false,
                "orderable": false,
                "targets": 0
            }, { "orderable": false, "targets": 1 },
        ],
        columns: [
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_chk_tffoc" type="checkbox">';
                }
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "ASSIGN_ID",
                visible: false
            },
            { data: "SALES_ORDER_NO", "className": "cls_nowrap" },
            { data: "SALES_ORDER_ITEM", "className": "cls_nowrap" },
            { data: "MATERIAL_NO", "className": "cls_nowrap" },
            { data: "MATERIAL_DESC", "className": "cls_nowrap" },
        ],
        "scrollX": true,
        "scrollY": "350px",
        "scrollCollapse": true,
        "paging": false,
        select: {
            'style': 'multi',
            selector: 'td:first-child input'
        },
        "processing": true,
        "rowCallback": function (row, data, index) {

        },
        order: [3, 'asc'],
        //"orderFixed": [2, 'asc'],
        initComplete: function (settings, json) {
        }
    });

    $(document).on('click', '#modal_tfartwork_salesorder .cls_chk_tffoc', function (e) {
        if ($(this).is(':checked')) {
            table_tfartwork_foc_select.rows($(this).closest('tr')).select();
        }
        else {
            table_tfartwork_foc_select.rows($(this).closest('tr')).deselect();
        }
        $(this).closest('tr').find('.cls_chk_tffoc').prop('checked', this.checked);
    });

    $(table_tfartwork_foc_select.table().container()).on('keyup', 'input', function () {
        table_tfartwork_foc_select
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    $("#table_tfartwork_foc_select_filter").hide();

    table_tfartwork_foc_select.on('order.dt search.dt', function () {
        table_tfartwork_foc_select.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();

    if (dataFilter.length > 0) {
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                for (i = 0; i < dataFilter.length; i++) {
                    if (data[1] == dataFilter[i].ASSIGN_ID) {
                        return false;
                    }
                }
                return true;
            }
        );
    }
    else {
        $.fn.dataTable.ext.search.pop();
    }

    $('input.filter_tfartwork_salesorder').on('keyup click', function () {
        $('#table_tfartwork_foc_select').DataTable().search(
            $('#filter_tfartwork_salesorder').val()
        ).draw();
    });
}

function bindSalesOrderViewChangePopUp() {
    var table_tfartwork_salesorder_viewchange = $('#table_tfartwork_salesorder_viewchange').DataTable();
    table_tfartwork_salesorder_viewchange.destroy();
    var groupColumn = 0;
    var bomItemNew = 0;

    table_tfartwork_salesorder_viewchange = $('#table_tfartwork_salesorder_viewchange').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/taskform/salesorderitem/showchange?data.artwork_sub_id=" + ArtworkSubId,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "visible": false, "targets": groupColumn }
        ],
        "scrollX": true,
        columns: [
            { data: "GROUPING", "className": "cls_nowrap" },
            { data: "FIELDS_NAME", "className": "cls_nowrap" },
            { data: "OLD_VALUE", "className": "cls_nowrap" },
            { data: "NEW_VALUE", "className": "cls_nowrap" }
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var j = 1;
            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group highlight"><td colspan="4">' + group + '</td></tr>'
                    );

                    last = group;
                    j++;
                }
            });
        },
        //select: {
        //    'style': 'multi'
        //},
        "processing": true,
        "rowCallback": function (row, data, index) {
            if (ARTWORK_NO.indexOf("AW-R") != -1 && data.FIELDS_NAME == "BOM ITEM CUSTOM 1") {
                if (data.NEW_VALUE.indexOf("NEW") != -1) {
                    bomItemNew = bomItemNew + 1;
                }
            }
            //if (data.RDD != "") {
            //    $(row).find('td').eq(16).html(moment(data.RDD).format('DD/MM/YYYY'));
            //}
        },
        //order: [[6, 'asc']],
        initComplete: function (settings, json) {
            //if (json.data.length > 0) {
            //    setValueToDDL('.cls_container_taskform .cls_lov_primary_type_other', json.data[0].PRIMARY_TYPE_ID, json.data[0].PRIMARY_TYPE_DISPLAY_TXT);
            //    setValueToDDL('.cls_container_taskform .cls_lov_primary_size', json.data[0].PRIMARY_SIZE_ID, json.data[0].PRIMARY_SIZE_DISPLAY_TXT);
            //    setValueToDDL('.cls_container_taskform .cls_lov_pack_size', json.data[0].PACK_SIZE_ID, json.data[0].PACK_SIZE_DISPLAY_TXT);
            //    //setValueToDDL('.cls_container_taskform .cls_lov_packing_style', json.data[0].PACKING_STYLE_ID, json.data[0].PACKING_STYLE_DISPLAY_TXT);
            //    setValueToDDL('.cls_container_taskform .cls_lov_pg_packaging_type_search', json.data[0].PACKING_TYPE_ID, json.data[0].PACKAGING_TYPE_DISPLAY_TXT);
            //}
            $(".cls_btn_tfartwork_salesorder_viewchange_accept").prop('disabled', false);
            $(".so_change_msg").addClass("cls_hide");
            if (bomItemNew > 0) {
                $(".cls_btn_tfartwork_salesorder_viewchange_accept").prop('disabled', true);
                $(".so_change_msg").removeClass("cls_hide");
            }
        }
    });
}

function setSelectSalesOrderItem(tblData) {
    var data = [];

    if (tblData.length > 0) {
        for (i = 0; i < tblData.length; i++) {
            var item = {};
            item['SOLD_TO_NAME'] = tblData[i].SOLD_TO_NAME;
            item['SHIP_TO_NAME'] = tblData[i].SHIP_TO_NAME;
            item['SALES_ORG'] = tblData[i].SALES_ORG;
            item['SALES_ORDER_NO'] = tblData[i].SO_NUMBER;
            item['SALES_ORDER_ITEM'] = tblData[i].SO_ITEM_NO;
            item['BRAND'] = tblData[i].BRAND;
            item['MATERIAL_NO'] = tblData[i].MATERIAL_NO;
            item['MATERIAL_DESC'] = tblData[i].MATERIAL_DESC;
            item['PORT'] = tblData[i].PORT;
            item['PRODUCTION_PLANT'] = tblData[i].PRODUCTION_PLANT;
            item['ORDER_BOM_ID'] = tblData[i].ORDER_BOM_ID;
            item['ORDER_BOM_NO'] = tblData[i].ORDER_BOM_NO;
            item['ORDER_BOM_DESC'] = tblData[i].ORDER_BOM_DESC;
            item['QUANTITY'] = tblData[i].QUANTITY;
            item['STOCK_PO'] = tblData[i].STOCK_PO;
            item['RDD'] = tblData[i].RDD;
            item['GROUPING_DISPLAY_TXT'] = tblData[i].GROUPING_DISPLAY_TXT;
            item['ASSIGN_ID'] = tblData[i].ASSIGN_ID;
            item['COUNTRY'] = tblData[i].COUNTRY;
            item['ARTWORK_REQUEST_ID'] = ARTWORK_REQUEST_ID;
            item['ARTWORK_SUB_ID'] = ArtworkSubId;

            item['BOM_ITEM_CUSTOM_1'] = tblData[i].BOM_ITEM_CUSTOM_1;

            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;
            data.push(item);
        }
    }

    return data;
}

function setSelectFOC(tblData) {
    var data = [];

    if (tblData.length > 0) {
        for (i = 0; i < tblData.length; i++) {
            var item = {};
            item['SALES_ORDER_NO'] = tblData[i].SALES_ORDER_NO;
            item['SALES_ORDER_ITEM'] = tblData[i].SALES_ORDER_ITEM;
            item['MATERIAL_NO'] = tblData[i].MATERIAL_NO;
            item['ARTWORK_REQUEST_ID'] = ARTWORK_REQUEST_ID;
            item['ARTWORK_SUB_ID'] = ArtworkSubId;

            item["CREATE_BY"] = UserID;
            item["UPDATE_BY"] = UserID;
            data.push(item);
        }
    }

    return data;
}

function validateSelectSalesOrderItem(data) {
    var jsonObj = new Object();
    jsonObj.data = data;

    var myurl = '/api/taskform/salesorderitem/validate';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callbackValidateSalesOrderItem, "", false, "");
}

function callbackValidateSalesOrderItem(res) {
    if (res.data.length > 0) {
        if (res.data[0].VALIDATE_MESSAGE != "") {
            $(".se-pre-con").fadeOut('fast');
            $(".se-pre-con2").fadeOut('fast');
            alertError2(res.data[0].VALIDATE_MESSAGE);
        } else {
            var table = $('#table_tfartwork_salesorder_select').DataTable();
            var tblData = table.rows('.selected').data();
            var data = setSelectSalesOrderItem(tblData);
            bindSelectSalesOrderItem(data);
        }
    }
}

function bindSelectFOC(data) {
    var jsonObj = new Object();
    jsonObj.data = data;

    var myurl = '/api/taskform/salesorderfoc/save';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjax(myurl, mytype, mydata, callbackBindSelectSalesOrderItem);
}

function bindSelectSalesOrderItem(data) {
    var jsonObj = new Object();
    jsonObj.data = data;

    var myurl = '/api/taskform/salesorderitem/save';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, callbackBindSelectSalesOrderItem);
}

function callbackBindSelectSalesOrderItem(res) {
    if (res.status == "E" && res.msg != '') {
        alertError(res.msg);
    }
    else if (res.status == "S") {
        $("#modal_tfartwork_salesorder .cls_btn_tfartwork_salesorder_close").click();
        if (table_tfartwork_salesorder == null)
            bindTaskFormSalesOrder();
        else
            table_tfartwork_salesorder.ajax.reload();
    }
    bindFOC(ArtworkSubId);
}
