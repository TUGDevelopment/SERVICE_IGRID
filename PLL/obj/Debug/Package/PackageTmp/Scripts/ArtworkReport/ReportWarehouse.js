$(document).ready(function () {
    bind_lov('.cls_report_warehouse .cls_lov_sales_organization', '/api/master/salesorg', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_warehouse .cls_lov_ship_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_warehouse .cls_lov_brand', '/api/lov/brand', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_warehouse .cls_lov_purchasing_organization', '/api/master/purchasingorg', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_warehouse .cls_lov_packaging_material', '/api/master/packagingmaterial', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_warehouse .cls_lov_company', '/api/lov/company', 'data.DISPLAY_TXT');

    $('.cls_table_report_warehouse thead tr').clone(true).appendTo('.cls_table_report_warehouse thead');
    $('.cls_table_report_warehouse thead tr:eq(1) th').each(function (i) {
        if (i == 0 || i == 4 || i == 5 || i == 20) {
            $(this).html('');
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    $('#table_report_warehouse').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        "scrollX": true
    });
    //bind_table_report_warehouse();

    //$(".cls_report_warehouse .cls_btn_search").click(function () {
    //    bind_table_report_warehouse();
    //});

    $(".cls_report_warehouse form").submit(function (e) {
        bind_table_report_warehouse();
        e.preventDefault();
    });

    $(".cls_report_warehouse .cls_btn_clear").click(function () {
        $('.cls_report_warehouse input[type=text]').val('');
        $('.cls_report_warehouse input[type=checkbox]').prop('checked', false);
        $('.cls_report_warehouse textarea').val('');
        $(".cls_report_warehouse .cls_lov_sales_organization").val('').trigger("change");
        $(".cls_report_warehouse .cls_lov_ship_to").val('').trigger("change");
        $(".cls_report_warehouse .cls_lov_brand").val('').trigger("change");
        $(".cls_report_warehouse .cls_lov_purchasing_organization").val('').trigger("change");
        $(".cls_report_warehouse .cls_lov_packaging_material").val('').trigger("change");
        $(".cls_report_warehouse .cls_lov_company").val('').trigger("change");
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box

    $(".cls_report_warehouse .cls_btn_export_excel").click(function () {
        window.open(suburl + "/excel/warehousereport?&data.GENERATE_EXCEL=X&data.SALES_ORDER_NO=" + $('.cls_report_warehouse .cls_txt_sales_order').val().replace(/\n/g, ',')
            + "&data.SALES_ORG=" + ($('.cls_report_warehouse .cls_lov_sales_organization').val() == null ? "" : $('.cls_report_warehouse .cls_lov_sales_organization').val())
            + "&data.SALES_ORDER_ITEM=" + $('.cls_report_warehouse .cls_txt_sales_order_item').val().replace(/\n/g, ',')
            + "&data.SHIP_TO_ID=" + $('.cls_report_warehouse .cls_lov_ship_to').val()
            + "&data.BRAND_ID=" + $('.cls_report_warehouse .cls_lov_brand').val()
            + "&data.PROJECT_NAME=" + $('.cls_report_warehouse .cls_txt_project_name').val()
            + "&data.PURCHASE_ORDER_NO=" + $('.cls_report_warehouse .cls_txt_purchase_order').val()
            + "&data.PURCHASING_ORG=" + ($('.cls_report_warehouse .cls_lov_purchasing_organization').val() == null ? "" : $('.cls_report_warehouse .cls_lov_purchasing_organization').val())
            + "&data.PO_ITEM_NO=" + $('.cls_report_warehouse .cls_txt_purchase_order_item').val()
            + "&data.MATERIAL_CODE=" + ($('.cls_report_warehouse .cls_lov_packaging_material').val() == null ? "" : $('.cls_report_warehouse .cls_lov_packaging_material').val())
            + "&data.COMPANY_ID=" + $('.cls_report_warehouse .cls_lov_company').val()
            + "&data.DOC_DATE=" + $('.cls_report_warehouse .cls_dt_document_date').val()

            , '_blank');
    });

    $('#modal_report_warehouse_so_att').on('shown.bs.modal', function (e) {
        $('.cls_so_att_salesorderno').val($(e.relatedTarget).data('sales_order_no'));
        bindReportWarehouse_SOAtt($('.cls_so_att_salesorderno').val());
    });

    $('#modal_report_warehouse_so_att').on('hidden.bs.modal', function (e) {
        $('.cls_so_att_salesorderno').val('');
    });

    $('#modal_report_warehouse_po_att').on('shown.bs.modal', function (e) {
        $('.cls_po_att_pono').val($(e.relatedTarget).data('po_no'));
        bindReportWarehouse_POAtt($('.cls_po_att_pono').val());
    });

    $('#modal_report_warehouse_po_att').on('hidden.bs.modal', function (e) {
        $('.cls_po_att_pono').val('');
    });

    $('#modal_report_warehouse_aw_att').on('shown.bs.modal', function (e) {
        $('.cls_aw_att_mat_no').val($(e.relatedTarget).data('mat_no'));
        $('.cls_aw_att_mat_desc').val($(e.relatedTarget).data('mat_desc'));
        bindReportWarehouse_AWAtt($('.cls_aw_att_mat_no').val(), $('.cls_aw_att_mat_desc').val());
    });

    $('#modal_report_warehouse_aw_att').on('hidden.bs.modal', function (e) {
        $('.cls_aw_att_mat_no').val('');
        $('.cls_aw_att_mat_desc').val('');
    });
});

var table_report_warehouse;
function bind_table_report_warehouse() {
    var salesorg = $('.cls_report_warehouse .cls_lov_sales_organization').val();
    var salesorder = $('.cls_report_warehouse .cls_txt_sales_order').val();
    var po = $('.cls_report_warehouse .cls_txt_purchase_order').val();

    if (!isEmpty(salesorder) || !isEmpty(salesorg) || !isEmpty(po)) {
        var param = getParamReportWarehouse();
        table_report_warehouse = $('#table_report_warehouse').DataTable();
        table_report_warehouse.destroy();
        table_report_warehouse = $('#table_report_warehouse').DataTable({
            serverSide: true,
            orderCellsTop: true,
            fixedHeader: true,
            //fixedColumns: {
            //    leftColumns: 3
            //},

            ajax: function (data, callback, settings) {
                for (var i = 0, len = data.columns.length; i < len; i++) {
                    delete data.columns[i].name;
                    delete data.columns[i].data;
                    delete data.columns[i].searchable;
                    delete data.columns[i].orderable;
                    delete data.columns[i].search.regex;
                    delete data.search.regex;
                }

                $.ajax({
                    url: suburl + "/api/report/warehousereport" + param,
                    type: 'GET',
                    data: data,
                    success: function (res) {
                        dtSuccess(res, callback);
                    }
                });
            },
            "columnDefs": [{
                "searchable": false,
                "orderable": false,
                "targets": [0, 4, 5, 20]
            }],
            "order": [[2, 'asc']],
            "processing": true,
            "searching": true,
            "lengthChange": true,
            "ordering": true,
            "info": true,
            "scrollX": true,
            "columns": [
                {
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { "data": "SALES_ORG", "className": "cls_nowrap" },
                { "data": "SALES_ORDER_NO", "className": "cls_nowrap" },
                { "data": "SALES_ORDER_ITEM", "className": "cls_nowrap" },
                {
                    render: function (data, type, row, meta) {
                        if (row.SALES_ORDER_NO != null) {
                            return '<a data-toggle="modal" data-sales_order_no="' + row.SALES_ORDER_NO + '" title="click" href="#modal_report_warehouse_so_att"><img src="' + suburl + '/Content/Free-file-icons/16px/_blank.png"></a>';
                        }
                    }
                },
                {
                    //className: 'dt-body-center',
                    render: function (data, type, row, meta) {
                        var downloadPrintMaster = '';
                        if (row.IS_SHOW_FILE_PRINT_MASTER == true) {
                            downloadPrintMaster = '<a target="_blank" href="' + suburl + '/FileUpload/DownloadArtworkPrintMaster?workflowNo=' + row.REQUEST_ITEM_NO + '" ><img src="' + suburl + '/Content/Free-file-icons/16px/_page.png" title="Download print master" ></a>';
                        }
                        if (row.MATERIAL_CODE != null && row.MATERIAL_DECRIPTION != null) {
                            return '<div style="width: 60%;float: left;"><span>&nbsp;</span>' + downloadPrintMaster + '</div><div style="width: 40%;float: left;"><a data-toggle="modal" data-mat_no="' + row.MATERIAL_CODE + '" data-mat_desc="' + row.MATERIAL_DECRIPTION + '" title="click" href="#modal_report_warehouse_aw_att"><img src="' + suburl + '/Content/Free-file-icons/16px/_blank.png"></a></div>';
                        }
                        else {
                            return downloadPrintMaster;
                        }
                    }
                },
                { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
                { "data": "MATERIAL_CODE", "className": "cls_nowrap" },
                { "data": "MATERIAL_DECRIPTION", "className": "cls_nowrap" },
                { "data": "PACKAGING_TYPE_NAME", "className": "cls_nowrap" },
                { "data": "REQUEST_ITEM_NO", "className": "cls_nowrap cls_wf_number" },
                { "data": "BRAND_NAME", "className": "cls_nowrap" },
                { "data": "PROJECT_NAME", "className": "cls_nowrap" },
                { "data": "SOLD_TO", "className": "cls_nowrap" },
                { "data": "SOLD_TO_NAME", "className": "cls_nowrap" },
                { "data": "SHIP_TO", "className": "cls_nowrap" },
                { "data": "SHIP_TO_NAME", "className": "cls_nowrap" },
                { "data": "PORT", "className": "cls_nowrap" },
                { "data": "PURCHASE_ORDER_NO", "className": "cls_nowrap" },
                { "data": "PO_ITEM_NO", "className": "cls_nowrap" },
                {
                    render: function (data, type, row, meta) {
                        if (row.PURCHASE_ORDER_NO != null) {
                            return '<a data-toggle="modal" data-po_no="' + row.PURCHASE_ORDER_NO + '" title="click" href="#modal_report_warehouse_po_att"><img src="' + suburl + '/Content/Free-file-icons/16px/_blank.png"></a>';
                        }
                    }
                },
                { "data": "PURCHASING_ORG", "className": "cls_nowrap" },
                { "data": "VENDOR_NO", "className": "cls_nowrap" },
                { "data": "VENDOR_NAME", "className": "cls_nowrap" },
                { "data": "DOC_DATE", "className": "cls_docdate cls_nowrap" },
                { "data": "DELIVERY_DATE", "className": "cls_deliverydate cls_nowrap" },
                {
                    "data": "QUANTITY", "className": "cls_nowrap",
                    render: $.fn.dataTable.render.number(',', '.', 2, '')
                },
                { "data": "ORDER_UNIT", "className": "cls_nowrap" }
            ],
            "rowCallback": function (row, data, index) {
                if (data.ARTWORK_SUB_ID != null) {
                    $(row).find('.cls_wf_number').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_SUB_ID + '" style="text-decoration: underline;">' + data.REQUEST_ITEM_NO + '</a>');
                }
                if (data.DOC_DATE != null) {
                    $(row).find('.cls_docdate').html(myDateMoment(data.DOC_DATE));
                }
                if (data.DELIVERY_DATE != null) {
                    $(row).find('.cls_deliverydate').html(myDateMoment(data.DELIVERY_DATE));
                }


                if (!isEmpty(data.REJECTION_CODE)) {
                    $(row).find('td').addClass('isupdate-highlight');
                    $(row).find('td').prop('title', 'This item has been rejected : ' + data.REJECTION_DESC + ' (' + data.REJECTION_CODE + ')');
                }
            }
        });

        $("#table_report_warehouse_filter").hide();

        $(table_report_warehouse.table().container()).on('keyup', 'input', function () {
            table_report_warehouse
                .column($(this).data('index'))
                .search(this.value)
                .draw(false);
        });
    }
    else {
        alertError2("Please input sales order or select sales organization or purchase order.");
    }
}

function getParamReportWarehouse() {
    return "?data.SALES_ORDER_NO=" + $('.cls_report_warehouse .cls_txt_sales_order').val().replace(/\n/g, ',')
        + "&data.SALES_ORG=" + ($('.cls_report_warehouse .cls_lov_sales_organization').val() == null ? "" : $('.cls_report_warehouse .cls_lov_sales_organization').val())
        + "&data.SALES_ORDER_ITEM=" + $('.cls_report_warehouse .cls_txt_sales_order_item').val().replace(/\n/g, ',')
        + "&data.SHIP_TO_ID=" + $('.cls_report_warehouse .cls_lov_ship_to').val()
        + "&data.BRAND_ID=" + $('.cls_report_warehouse .cls_lov_brand').val()
        + "&data.PROJECT_NAME=" + $('.cls_report_warehouse .cls_txt_project_name').val()
        + "&data.PURCHASE_ORDER_NO=" + $('.cls_report_warehouse .cls_txt_purchase_order').val()
        + "&data.PURCHASING_ORG=" + ($('.cls_report_warehouse .cls_lov_purchasing_organization').val() == null ? "" : $('.cls_report_warehouse .cls_lov_purchasing_organization').val())
        + "&data.PO_ITEM_NO=" + $('.cls_report_warehouse .cls_txt_purchase_order_item').val()
        + "&data.MATERIAL_CODE=" + ($('.cls_report_warehouse .cls_lov_packaging_material').val() == null ? "" : $('.cls_report_warehouse .cls_lov_packaging_material').val())
        + "&data.COMPANY_ID=" + $('.cls_report_warehouse .cls_lov_company').val()
        + "&data.DOC_DATE=" + $('.cls_report_warehouse .cls_dt_document_date').val();
}

function bindReportWarehouse_SOAtt(sales_order_no) {
    table_report_warehouse_so_att = $('#table_report_warehouse_so_att').DataTable();
    table_report_warehouse_so_att.destroy();
    table_report_warehouse_so_att = $('#table_report_warehouse_so_att').DataTable({
        ajax: function (data, callback, settings) {
            var xhr = $.ajax({
                url: suburl + "/api/report/warehousereport_soatt?data.sales_order_no=" + sales_order_no,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[2, 'asc']],
        "processing": true,
        "lengthChange": false,
        //"ordering": false,
        //"info": false,
        "searching": false,
        "pageLength": -1,
        "paging": false,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'icon');
                }
            },
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'title');
                }
            },
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'file_name');
                }
            },
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'created_by');
                }
            },
            {
                render: function (data, type, row, meta) {
                    if (row.CREATED_DATE != null) {
                        return moment(row.CREATED_DATE).format('DD/MM/YYYY HH:mm:ss');
                    }
                }
            },
            {
                render: function (data, type, row, meta) {
                    return '<a href="' + suburl + '/FileUpload/DownloadFile?nodeIdTxt=' + row.NODE_ID_TXT + '" class="cls_hand" target="_blank">Download</a>';
                }
            }
        ]
    });
}

function bindReportWarehouse_POAtt(po_no) {
    table_report_warehouse_po_att = $('#table_report_warehouse_po_att').DataTable();
    table_report_warehouse_po_att.destroy();
    table_report_warehouse_po_att = $('#table_report_warehouse_po_att').DataTable({
        ajax: function (data, callback, settings) {
            var xhr = $.ajax({
                url: suburl + "/api/report/warehousereport_poatt?data.purchase_order_no=" + po_no,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[2, 'asc']],
        "processing": true,
        "lengthChange": false,
        //"ordering": false,
        //"info": false,
        "searching": false,
        "pageLength": -1,
        "paging": false,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'icon');
                }
            },
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'title');
                }
            },
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'file_name');
                }
            },
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'created_by');
                }
            },
            {
                render: function (data, type, row, meta) {
                    if (row.CREATED_DATE != null) {
                        return moment(row.CREATED_DATE).format('DD/MM/YYYY HH:mm:ss');
                    }
                }
            },
            {
                render: function (data, type, row, meta) {
                    return '<a href="' + suburl + '/FileUpload/DownloadFile?nodeIdTxt=' + row.NODE_ID_TXT + '" class="cls_hand" target="_blank">Download</a>';
                }
            }
        ]
    });
}

function bindReportWarehouse_AWAtt(mat_no, mat_desc) {
    table_report_warehouse_aw_att = $('#table_report_warehouse_aw_att').DataTable();
    table_report_warehouse_aw_att.destroy();
    table_report_warehouse_aw_att = $('#table_report_warehouse_aw_att').DataTable({
        ajax: function (data, callback, settings) {
            var xhr = $.ajax({
                //url: suburl + "/api/report/warehousereport_awatt?data.MATERIAL_CODE=" + mat_no + "&data.MATERIAL_DECRIPTION=" + mat_desc,
                url: suburl + "/api/report/warehousereport_awatt?data.material_code=" + mat_no,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[2, 'asc']],
        "processing": true,
        "lengthChange": false,
        //"ordering": false,
        //"info": false,
        "searching": false,
        "pageLength": -1,
        "paging": false,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'icon');
                }
            },
            { "data": "TITLE", "className": "cls_nowrap" },
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'file_name');
                }
            },
            {
                render: function (data, type, row, meta) {
                    return getName_Att(row.FILE_NAME, 'created_by');
                }
            },
            {
                render: function (data, type, row, meta) {
                    if (row.CREATED_DATE != null) {
                        return moment(row.CREATED_DATE).format('DD/MM/YYYY HH:mm:ss');
                    }
                }
            },
            {
                render: function (data, type, row, meta) {
                    return '<a href="' + suburl + '/FileUpload/DownloadFile?nodeIdTxt=' + row.NODE_ID_TXT + '" class="cls_hand" target="_blank">Download</a>';
                }
            }
        ]
    });
}

function getName_Att(file_name, type) {
    var arrTitle = file_name.split('---');
    var arrExtension = file_name.split('.');
    switch (type) {
        case "title":
            if (arrTitle.length > 2) {
                return arrTitle[0];
            }
            break;
        case "file_name":
            if (arrTitle.length > 2) {
                return arrTitle[2];
            } else if (arrTitle.length > 0) {
                return arrTitle[0];
            }
            break;
        case "icon":
            if (arrExtension.length > 1) {
                return "<img src=" + suburl + "/Content/Free-file-icons/32px/" + arrExtension[arrExtension.length - 1] + ".png>";
            }
            else {
                return "<img src=" + suburl + "/Content/Free-file-icons/32px/pdf.png>";
            }
            break;
        case "created_by":
            if (arrTitle.length > 2) {
                return arrTitle[1];
            }
            break;
    }
    return "";
}