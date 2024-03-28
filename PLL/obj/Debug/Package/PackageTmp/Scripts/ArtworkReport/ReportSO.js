var first_load = true;
var currtap = 'assign';
$(document).ready(function () {

    bind_table_report_so();
    bind_table_report_sapso();
    bind_table_report_idoc();
    bind_table_report_mat3();
    bind_table_report_mat5();
    bind_table_report_syslog();
    bind_table_report_bom_master();

    $("a[href='#view_assignso_view']").on('shown.bs.tab', function (e) {
        currtap = 'assign';
        $('.cls_report_so .cls_search_so_no').val('');
        $(".cls_search_so").show();
        $(".cls_so_item").hide();
        table_report_so.columns.adjust();
    });
    $("a[href='#view_sapso_view']").on('shown.bs.tab', function (e) {
        currtap = 'sap';
        $('.cls_report_so .cls_search_so_no').val('');
        $(".cls_search_so").show();
        $(".cls_so_item").show();
        table_report_sapso.columns.adjust();
    });
    $("a[href='#view_idoc_view']").on('shown.bs.tab', function (e) {
        currtap = 'idoc';
        $('.cls_report_so .cls_search_so_no').val('');
        $(".cls_search_so").show();
        $(".cls_so_item").hide();
        table_report_idoc.columns.adjust();
    });
    $("a[href='#view_mat3_view']").on('shown.bs.tab', function (e) {
        currtap = 'mat3';
        $('.cls_report_so .cls_search_so_no').val('');
        $(".cls_search_so").show();
        $(".cls_so_item").hide();
        table_report_mat3.columns.adjust();
    });
    $("a[href='#view_mat5_view']").on('shown.bs.tab', function (e) {
        currtap = 'mat5';
        $('.cls_report_so .cls_search_so_no').val('');
        $(".cls_search_so").show();
        $(".cls_so_item").hide();
        table_report_mat5.columns.adjust();
    });
    $("a[href='#view_syslog_view']").on('shown.bs.tab', function (e) {
        currtap = 'syslog';
        $('.cls_report_so .cls_search_so_no').val('');
        $(".cls_search_so").show();
        $(".cls_so_item").hide();
        table_report_syslog.columns.adjust();
    });
    $("a[href='#view_bom_master']").on('shown.bs.tab', function (e) {
        currtap = 'bom_master';
        $('.cls_report_so .cls_search_so_no').val('');
        $(".cls_search_so").show();
        $(".cls_so_item").hide();
        table_report_bom_master.columns.adjust();
    });

    first_load = false;

    $(".cls_report_so .cls_btn_clr").click(function () {
        $('.cls_report_so input[type=text]').val('');
        $('.cls_report_so textarea').val('');
    });

    $(".cls_report_so form").submit(function (e) {
        try {
            if (currtap == 'assign')
                table_report_so.ajax.reload();
            else if (currtap == 'sap')
                table_report_sapso.ajax.reload();
            else if (currtap == 'idoc')
                table_report_idoc.ajax.reload();
            else if (currtap == 'mat3')
                table_report_mat3.ajax.reload();
            else if (currtap == 'mat5')
                table_report_mat5.ajax.reload();
            else if (currtap == 'syslog')
                table_report_syslog.ajax.reload();
            else if (currtap == 'bom_master')
                table_report_bom_master.ajax.reload();
            e.preventDefault();
        }
        catch (err) {
            alert(err);
        }
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box
});

var table_report_so;
function bind_table_report_so() {
    table_report_so = $('#table_report_so').DataTable({
        //serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/soreport?data.SALES_ORDER_NO=" + $('.cls_report_so .cls_search_so_no').val().replace(/\n/g, ',') + "&data.first_load=" + first_load,
                type: 'GET',
                data: function (data) {
                    for (var i = 0, len = data.columns.length; i < len; i++) {
                        if (!data.columns[i].search.value) delete data.columns[i].search;
                        if (data.columns[i].searchable === true) delete data.columns[i].searchable;
                        if (data.columns[i].orderable === true) delete data.columns[i].orderable;
                        if (data.columns[i].data === data.columns[i].name) delete data.columns[i].name;
                    }
                    delete data.search.regex;
                },
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[1, 'asc']],
        "processing": true,
        "searching": false,
        "lengthChange": true,
        "ordering": false,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "ASSIGN_SO_HEADER_ID", "className": "cls_nowrap" },
            { "data": "ARTWORK_PROCESS_SO_ID", "className": "cls_nowrap" },
            { "data": "ARTWORK_REQUEST_ID", "className": "cls_nowrap" },
            { "data": "ARTWORK_SUB_ID", "className": "cls_nowrap" },
            { "data": "SALES_ORDER_NO", "className": "cls_nowrap" },
            { "data": "SOLD_TO", "className": "cls_nowrap" },
            { "data": "SOLD_TO_NAME", "className": "cls_nowrap" },
            { "data": "LAST_SHIPMENT_DATE", "className": "cls_nowrap" },
            { "data": "DATE_1_2", "className": "cls_nowrap" },
            { "data": "CREATE_ON", "className": "cls_nowrap cls_rdd" },
            { "data": "RDD", "className": "cls_nowrap cls_create_on" },
            { "data": "PAYMENT_TERM", "className": "cls_nowrap" },
            { "data": "LC_NO", "className": "cls_nowrap" },
            { "data": "EXPIRED_DATE", "className": "cls_nowrap" },
            { "data": "SHIP_TO", "className": "cls_nowrap" },
            { "data": "SHIP_TO_NAME", "className": "cls_nowrap" },
            { "data": "SOLD_TO_PO", "className": "cls_nowrap" },
            { "data": "SHIP_TO_PO", "className": "cls_nowrap" },
            { "data": "SALES_GROUP", "className": "cls_nowrap" },
            { "data": "MARKETING_CO", "className": "cls_nowrap" },
            { "data": "MARKETING_CO_NAME", "className": "cls_nowrap" },
            { "data": "MARKETING", "className": "cls_nowrap" },
            { "data": "MARKETING_NAME", "className": "cls_nowrap" },
            { "data": "MARKETING_ORDER_SAP", "className": "cls_nowrap" },
            { "data": "MARKETING_ORDER_SAP_NAME", "className": "cls_nowrap" },
            { "data": "SALES_ORG", "className": "cls_nowrap" },
            { "data": "DISTRIBUTION_CHANNEL", "className": "cls_nowrap" },
            { "data": "DIVITION", "className": "cls_nowrap" },
            { "data": "SALES_ORDER_TYPE", "className": "cls_nowrap" },
            { "data": "HEADER_CUSTOM_1", "className": "cls_nowrap" },
            { "data": "HEADER_CUSTOM_2", "className": "cls_nowrap" },
            { "data": "HEADER_CUSTOM_3", "className": "cls_nowrap" },
            { "data": "ASSIGN_SO_ITEM_ID", "className": "cls_nowrap" },
            { "data": "ITEM", "className": "cls_nowrap" },
            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
            { "data": "MATERIAL_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "NET_WEIGHT", "className": "cls_nowrap" },
            { "data": "ORDER_QTY", "className": "cls_nowrap" },
            { "data": "ORDER_UNIT", "className": "cls_nowrap" },
            { "data": "ETD_DATE_FROM", "className": "cls_nowrap" },
            { "data": "ETD_DATE_TO", "className": "cls_nowrap" },
            { "data": "PLANT", "className": "cls_nowrap" },
            { "data": "OLD_MATERIAL_CODE", "className": "cls_nowrap" },
            { "data": "PACK_SIZE", "className": "cls_nowrap" },
            { "data": "VALUME_PER_UNIT", "className": "cls_nowrap" },
            { "data": "VALUME_UNIT", "className": "cls_nowrap" },
            { "data": "SIZE_DRAIN_WT", "className": "cls_nowrap" },
            { "data": "PROD_INSP_MEMO", "className": "cls_nowrap" },
            { "data": "REJECTION_CODE", "className": "cls_nowrap" },
            { "data": "REJECTION_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "PORT", "className": "cls_nowrap" },
            { "data": "VIA", "className": "cls_nowrap" },
            { "data": "IN_TRANSIT_TO", "className": "cls_nowrap" },
            { "data": "BRAND_ID", "className": "cls_nowrap" },
            { "data": "BRAND_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "ADDITIONAL_BRAND_ID", "className": "cls_nowrap" },
            { "data": "ADDITIONAL_BRAND_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "PRODUCTION_PLANT", "className": "cls_nowrap" },
            { "data": "ZONE", "className": "cls_nowrap" },
            { "data": "COUNTRY", "className": "cls_nowrap" },
            { "data": "PRODUCTION_HIERARCHY", "className": "cls_nowrap" },
            { "data": "MRP_CONTROLLER", "className": "cls_nowrap" },
            { "data": "STOCK", "className": "cls_nowrap" },
            { "data": "ITEM_CUSTOM_1", "className": "cls_nowrap" },
            { "data": "ITEM_CUSTOM_2", "className": "cls_nowrap" },
            { "data": "ITEM_CUSTOM_3", "className": "cls_nowrap" },
            { "data": "ASSIGN_SO_ITEM_COMPONENT_ID", "className": "cls_nowrap" },
            { "data": "COMPONENT_ITEM", "className": "cls_nowrap" },
            { "data": "COMPONENT_MATERIAL", "className": "cls_nowrap" },
            { "data": "DECRIPTION", "className": "cls_nowrap" },
            { "data": "QUANTITY", "className": "cls_nowrap" },
            { "data": "UNIT", "className": "cls_nowrap" },
            { "data": "STOCK_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "BOM_ITEM_CUSTOM_1", "className": "cls_nowrap" },
            { "data": "BOM_ITEM_CUSTOM_2", "className": "cls_nowrap" },
            { "data": "BOM_ITEM_CUSTOM_3", "className": "cls_nowrap" },
            { "data": "REQUEST_ITEM_NO", "className": "cls_nowrap cls_wf_number" },
            { "data": "CURRENT_STEP_ID", "className": "cls_nowrap" },
            { "data": "STEP_ARTWORK_CODE", "className": "cls_nowrap" },
            { "data": "STEP_ARTWORK_NAME", "className": "cls_nowrap" },
            { "data": "STEP_ARTWORK_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "ROLE_ID_RESPONSE", "className": "cls_nowrap" },
            { "data": "DURATION", "className": "cls_nowrap" },
            { "data": "TITLE", "className": "cls_nowrap" },
            { "data": "FIRST_NAME", "className": "cls_nowrap" },
            { "data": "LAST_NAME", "className": "cls_nowrap" },
            { "data": "EMAIL", "className": "cls_nowrap" },
            { "data": "USERNAME", "className": "cls_nowrap" },
            { "data": "READY_CREATE_PO_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "IS_END", "className": "cls_nowrap" },
            { "data": "REMARK", "className": "cls_nowrap" },
            { "data": "REASON_ID", "className": "cls_nowrap" },
            { "data": "ARTWORK_FOLDER_NODE_ID", "className": "cls_nowrap" },
            { "data": "IS_TERMINATE", "className": "cls_nowrap" },
            { "data": "REMARK_TERMINATE", "className": "cls_nowrap" },
            { "data": "IS_DELEGATE", "className": "cls_nowrap" },
            { "data": "CURRENT_CUSTOMER_ID", "className": "cls_nowrap" },
            { "data": "CURRENT_VENDOR_ID", "className": "cls_nowrap" },
            { "data": "CURRENT_ROLE_ID", "className": "cls_nowrap" },
            { "data": "CURRENT_USER_ID", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap cls_creat_date" },
            { "data": "CREATE_BY", "className": "cls_nowrap" },
            { "data": "UPDATE_DATE", "className": "cls_nowrap cls_update_date" },
            { "data": "UPDATE_BY", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_wf_number').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_SUB_ID + '" style="text-decoration: underline;">' + data.REQUEST_ITEM_NO + '</a>');
            $(row).find('.cls_create_on').html(myDateMoment(data.CREATE_ON));
            $(row).find('.cls_rdd').html(myDateMoment(data.RDD));
            $(row).find('.cls_creat_date').html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('.cls_update_date').html(myDateTimeMoment(data.UPDATE_DATE));
        },
    });
}

var table_report_idoc;
function bind_table_report_idoc() {
    table_report_idoc = $('#table_report_idoc').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/idocreport?data.PURCHASE_ORDER_NO=" + $('.cls_report_so .cls_search_so_no').val().replace(/\n/g, ',') + "&data.first_load=" + first_load,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[1, 'asc']],
        "processing": true,
        "searching": false,
        "lengthChange": true,
        "ordering": false,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "PO_ITEM_NO", "className": "cls_nowrap" },
            { "data": "PO_IDOC_ID", "className": "cls_nowrap" },
            { "data": "PURCHASE_ORDER_NO", "className": "cls_nowrap" },
            { "data": "CURRENCY", "className": "cls_nowrap" },
            { "data": "DATE", "className": "cls_nowrap" },
            { "data": "TIME", "className": "cls_nowrap" },
            { "data": "PURCHASING_ORG", "className": "cls_nowrap" },
            { "data": "COMPANY_CODE", "className": "cls_nowrap" },
            { "data": "VENDOR_NO", "className": "cls_nowrap" },
            { "data": "VENDOR_NAME", "className": "cls_nowrap" },
            { "data": "PURCHASER", "className": "cls_nowrap" },
            { "data": "PO_IDOC_ITEM_ID", "className": "cls_nowrap" },
            { "data": "PO_ITEM_NO", "className": "cls_nowrap" },
            { "data": "RECORD_TYPE", "className": "cls_nowrap" },
            { "data": "DELETION_INDICATO", "className": "cls_nowrap" },
            { "data": "QUANTITY", "className": "cls_nowrap" },
            { "data": "ORDER_UNIT", "className": "cls_nowrap" },
            { "data": "ORDER_PRICE_UNIT", "className": "cls_nowrap" },
            { "data": "NET_ORDER_PRICE", "className": "cls_nowrap" },
            { "data": "PRICE_UNIT", "className": "cls_nowrap" },
            { "data": "AMOUNT", "className": "cls_nowrap" },
            { "data": "MATERIAL_GROUP", "className": "cls_nowrap" },
            { "data": "DENOMINATOR_QUANTITY_CONVERSION", "className": "cls_nowrap" },
            { "data": "NUMERATOR_QUANTITY_CONVERSION", "className": "cls_nowrap" },
            { "data": "PLANT", "className": "cls_nowrap" },
            { "data": "METERIAL_NUMBER", "className": "cls_nowrap" },
            { "data": "SHORT_TEXT", "className": "cls_nowrap" },
            { "data": "DELIVERY_DATE", "className": "cls_nowrap" },
            { "data": "SALES_DOCUMENT_NO", "className": "cls_nowrap" },
            { "data": "SALES_DOCUMENT_ITEM", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap cls_creat_date" },
            { "data": "CREATE_BY", "className": "cls_nowrap" },
            { "data": "UPDATE_DATE", "className": "cls_nowrap cls_update_date" },
            { "data": "UPDATE_BY", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_creat_date').html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('.cls_update_date').html(myDateTimeMoment(data.UPDATE_DATE));
        },
    });
}

var table_report_sapso;
function bind_table_report_sapso() {
    table_report_sapso = $('#table_report_sapso').DataTable({
        //serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/sapsoreport?data.item=" + $('.cls_report_so .cls_search_so_item').val() + "&data.SALES_ORDER_NO=" + $('.cls_report_so .cls_search_so_no').val().replace(/\n/g, ',') + "&data.first_load=" + first_load,
                type: 'GET',
                data: function (data) {
                    for (var i = 0, len = data.columns.length; i < len; i++) {
                        if (!data.columns[i].search.value) delete data.columns[i].search;
                        if (data.columns[i].searchable === true) delete data.columns[i].searchable;
                        if (data.columns[i].orderable === true) delete data.columns[i].orderable;
                        if (data.columns[i].data === data.columns[i].name) delete data.columns[i].name;
                    }
                    delete data.search.regex;
                },
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[1, 'asc']],
        "processing": true,
        "searching": false,
        "lengthChange": true,
        "ordering": false,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "PO_COMPLETE_SO_HEADER_ID", "className": "cls_nowrap" },
            { "data": "SALES_ORDER_NO", "className": "cls_nowrap" },
            { "data": "SOLD_TO", "className": "cls_nowrap" },
            { "data": "SOLD_TO_NAME", "className": "cls_nowrap" },
            { "data": "LAST_SHIPMENT_DATE", "className": "cls_nowrap" },
            { "data": "DATE_1_2", "className": "cls_nowrap" },
            { "data": "CREATE_ON", "className": "cls_nowrap cls_create_on" },
            { "data": "RDD", "className": "cls_nowrap cls_sapso_rdd" },
            { "data": "PAYMENT_TERM", "className": "cls_nowrap" },
            { "data": "LC_NO", "className": "cls_nowrap" },
            { "data": "EXPIRED_DATE", "className": "cls_nowrap" },
            { "data": "SHIP_TO", "className": "cls_nowrap" },
            { "data": "SHIP_TO_NAME", "className": "cls_nowrap" },
            { "data": "SOLD_TO_PO", "className": "cls_nowrap" },
            { "data": "SHIP_TO_PO", "className": "cls_nowrap" },
            { "data": "SALES_GROUP", "className": "cls_nowrap" },
            { "data": "MARKETING_CO", "className": "cls_nowrap" },
            { "data": "MARKETING_CO_NAME", "className": "cls_nowrap" },
            { "data": "MARKETING", "className": "cls_nowrap" },
            { "data": "MARKETING_NAME", "className": "cls_nowrap" },
            { "data": "MARKETING_ORDER_SAP", "className": "cls_nowrap" },
            { "data": "MARKETING_ORDER_SAP_NAME", "className": "cls_nowrap" },
            { "data": "SALES_ORG", "className": "cls_nowrap" },
            { "data": "DISTRIBUTION_CHANNEL", "className": "cls_nowrap" },
            { "data": "DIVITION", "className": "cls_nowrap" },
            { "data": "SALES_ORDER_TYPE", "className": "cls_nowrap" },
            { "data": "HEADER_CUSTOM_1", "className": "cls_nowrap" },
            { "data": "HEADER_CUSTOM_2", "className": "cls_nowrap" },
            { "data": "HEADER_CUSTOM_3", "className": "cls_nowrap" },
            { "data": "IS_MIGRATION", "className": "cls_nowrap" },
            { "data": "PO_COMPLETE_SO_ITEM_ID", "className": "cls_nowrap" },
            { "data": "ITEM", "className": "cls_nowrap" },
            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
            { "data": "MATERIAL_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "NET_WEIGHT", "className": "cls_nowrap" },
            { "data": "ORDER_QTY", "className": "cls_nowrap" },
            { "data": "ORDER_UNIT", "className": "cls_nowrap" },
            { "data": "ETD_DATE_FROM", "className": "cls_nowrap cls_etdfrom" },
            { "data": "ETD_DATE_TO", "className": "cls_nowrap cls_etdto" },
            { "data": "PLANT", "className": "cls_nowrap" },
            { "data": "OLD_MATERIAL_CODE", "className": "cls_nowrap" },
            { "data": "PACK_SIZE", "className": "cls_nowrap" },
            { "data": "VALUME_PER_UNIT", "className": "cls_nowrap" },
            { "data": "VALUME_UNIT", "className": "cls_nowrap" },
            { "data": "SIZE_DRAIN_WT", "className": "cls_nowrap" },
            { "data": "PROD_INSP_MEMO", "className": "cls_nowrap" },
            { "data": "REJECTION_CODE", "className": "cls_nowrap" },
            { "data": "REJECTION_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "PORT", "className": "cls_nowrap" },
            { "data": "VIA", "className": "cls_nowrap" },
            { "data": "IN_TRANSIT_TO", "className": "cls_nowrap" },
            { "data": "BRAND_ID", "className": "cls_nowrap" },
            { "data": "BRAND_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "ADDITIONAL_BRAND_ID", "className": "cls_nowrap" },
            { "data": "ADDITIONAL_BRAND_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "PRODUCTION_PLANT", "className": "cls_nowrap" },
            { "data": "ZONE", "className": "cls_nowrap" },
            { "data": "COUNTRY", "className": "cls_nowrap" },
            { "data": "PRODUCTION_HIERARCHY", "className": "cls_nowrap" },
            { "data": "MRP_CONTROLLER", "className": "cls_nowrap" },
            { "data": "STOCK", "className": "cls_nowrap" },
            { "data": "ITEM_CUSTOM_1", "className": "cls_nowrap" },
            { "data": "ITEM_CUSTOM_2", "className": "cls_nowrap" },
            { "data": "ITEM_CUSTOM_3", "className": "cls_nowrap" },
            { "data": "PO_COMPLETE_SO_ITEM_COMPONENT_ID", "className": "cls_nowrap" },
            { "data": "COMPONENT_ITEM", "className": "cls_nowrap" },
            { "data": "COMPONENT_MATERIAL", "className": "cls_nowrap" },
            { "data": "DECRIPTION", "className": "cls_nowrap" },
            { "data": "QUANTITY", "className": "cls_nowrap" },
            { "data": "UNIT", "className": "cls_nowrap" },
            { "data": "BOM_STOCK", "className": "cls_nowrap" },
            { "data": "BOM_ITEM_CUSTOM_1", "className": "cls_nowrap" },
            { "data": "BOM_ITEM_CUSTOM_2", "className": "cls_nowrap" },
            { "data": "BOM_ITEM_CUSTOM_3", "className": "cls_nowrap" },
            { "data": "BOM_IS_ACTIVE", "className": "cls_nowrap" },
            { "data": "SO_ITEM_IS_ACTIVE", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap cls_creat_date" },
            { "data": "CREATE_BY", "className": "cls_nowrap" },
            { "data": "UPDATE_DATE", "className": "cls_nowrap cls_update_date" },
            { "data": "UPDATE_BY", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_creat_date').html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('.cls_update_date').html(myDateTimeMoment(data.UPDATE_DATE));
            $(row).find('.cls_create_on').html(myDateMoment(data.CREATE_ON));
            $(row).find('.cls_sapso_rdd').html(myDateMoment(data.RDD));
            $(row).find('.cls_etdfrom').html(myDateMoment(data.ETD_DATE_FROM));
            $(row).find('.cls_etdto').html(myDateMoment(data.ETD_DATE_TO));
        },
    });
}

var table_report_mat3;
function bind_table_report_mat3() {
    table_report_mat3 = $('#table_report_mat3').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
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
                url: suburl + "/api/report/mat3report?data.PRODUCT_CODE=" + $('.cls_report_so .cls_search_so_no').val().replace(/\n/g, ',') + "&data.first_load=" + first_load,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[1, 'asc']],
        "processing": true,
        "searching": false,
        "lengthChange": true,
        "ordering": false,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "XECM_PRODUCT_ID", "className": "cls_nowrap" },
            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
            { "data": "PRODUCT_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "NET_WEIGHT", "className": "cls_nowrap" },
            { "data": "DRAINED_WEIGHT", "className": "cls_nowrap" },
            { "data": "PRIMARY_SIZE", "className": "cls_nowrap" },
            { "data": "CONTAINER_TYPE", "className": "cls_nowrap" },
            { "data": "LID_TYPE", "className": "cls_nowrap" },
            { "data": "PACKING_STYLE", "className": "cls_nowrap" },
            { "data": "PACK_SIZE", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap cls_creat_date" },
            { "data": "CREATE_BY", "className": "cls_nowrap" },
            { "data": "UPDATE_DATE", "className": "cls_nowrap cls_update_date" },
            { "data": "UPDATE_BY", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_creat_date').html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('.cls_update_date').html(myDateTimeMoment(data.UPDATE_DATE));
        },
    });
}

var table_report_mat5;
function bind_table_report_mat5() {
    table_report_mat5 = $('#table_report_mat5').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
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
                url: suburl + "/api/report/mat5report?data.PRODUCT_CODE=" + $('.cls_report_so .cls_search_so_no').val().replace(/\n/g, ',') + "&data.first_load=" + first_load,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[1, 'asc']],
        "processing": true,
        "searching": false,
        "lengthChange": true,
        "ordering": false,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "XECM_PRODUCT5_ID", "className": "cls_nowrap" },
            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
            { "data": "PRODUCT_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "NET_WEIGHT", "className": "cls_nowrap" },
            { "data": "DRAINED_WEIGHT", "className": "cls_nowrap" },
            { "data": "PRIMARY_SIZE", "className": "cls_nowrap" },
            { "data": "CONTAINER_TYPE", "className": "cls_nowrap" },
            { "data": "LID_TYPE", "className": "cls_nowrap" },
            { "data": "PACKING_STYLE", "className": "cls_nowrap" },
            { "data": "PACK_SIZE", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap cls_creat_date" },
            { "data": "CREATE_BY", "className": "cls_nowrap" },
            { "data": "UPDATE_DATE", "className": "cls_nowrap cls_update_date" },
            { "data": "UPDATE_BY", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_creat_date').html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('.cls_update_date').html(myDateTimeMoment(data.UPDATE_DATE));
        },
    });
}

var table_report_syslog;
function bind_table_report_syslog() {
    table_report_syslog = $('#table_report_syslog').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
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
                url: suburl + "/api/report/syslogreport?data.ACTION=" + $('.cls_report_so .cls_search_so_no').val().replace(/\n/g, ',') + "&data.first_load=" + first_load,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[1, 'asc']],
        "processing": true,
        "searching": false,
        "lengthChange": true,
        "ordering": false,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "LOG_ID", "className": "cls_nowrap" },
            { "data": "TABLE_NAME", "className": "cls_nowrap" },
            { "data": "ACTION", "className": "cls_nowrap" },
            { "data": "NEW_VALUE", "className": "cls_nowrap" },
            { "data": "OLD_VALUE", "className": "cls_nowrap" },
            { "data": "ERROR_MSG", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap cls_creat_date" },
            { "data": "CREATE_BY", "className": "cls_nowrap" },
            { "data": "UPDATE_DATE", "className": "cls_nowrap cls_update_date" },
            { "data": "UPDATE_BY", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_creat_date').html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('.cls_update_date').html(myDateTimeMoment(data.UPDATE_DATE));
        },
    });
}

var table_report_bom_master;
function bind_table_report_bom_master() {
    table_report_bom_master = $('#table_report_bom_master').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
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
                url: suburl + "/api/report/bommaster?data.MATERIAL=" + $('.cls_report_so .cls_search_so_no').val().replace(/\n/g, ',') + "&data.first_load=" + first_load,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[1, 'asc']],
        "processing": true,
        "searching": false,
        "lengthChange": true,
        "ordering": false,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "ORDER_BOM_ID", "className": "cls_nowrap" },
            { "data": "CHANGE_TYPE", "className": "cls_nowrap" },
            { "data": "DATE", "className": "cls_nowrap" },
            { "data": "TIME", "className": "cls_nowrap" },
            { "data": "COUNTER", "className": "cls_nowrap" },
            { "data": "MATERIAL", "className": "cls_nowrap" },
            { "data": "SOLD_TO_PARTY", "className": "cls_nowrap" },
            { "data": "SHIP_TO_PARTY", "className": "cls_nowrap" },
            { "data": "BRAND_ID", "className": "cls_nowrap" },
            { "data": "ADDITIONAL_BRAND_ID", "className": "cls_nowrap" },
            { "data": "ROUTE", "className": "cls_nowrap" },
            { "data": "INTRANSIT_PORT", "className": "cls_nowrap" },
            { "data": "SALES_ORGANIZATION", "className": "cls_nowrap" },
            { "data": "PLANT", "className": "cls_nowrap" },
            { "data": "MATERIAL_NUMBER", "className": "cls_nowrap" },
            { "data": "COUNTRY_KEY", "className": "cls_nowrap" },
            { "data": "PACKAGING_QUANTITY", "className": "cls_nowrap" },
            { "data": "PACKAGING_UNIT", "className": "cls_nowrap" },
            { "data": "FG_QUANTITY", "className": "cls_nowrap" },
            { "data": "FG_UNIT", "className": "cls_nowrap" },
            { "data": "START_DATE", "className": "cls_nowrap cls_start_date" },
            { "data": "END_DATE", "className": "cls_nowrap cls_end_date" },
            { "data": "WASTE_PERCENT", "className": "cls_nowrap" },
            { "data": "COUNTER_REFERENCE", "className": "cls_nowrap" },
            { "data": "CREATE_DATE", "className": "cls_nowrap cls_creat_date" },
            { "data": "CREATE_BY", "className": "cls_nowrap" },
            { "data": "UPDATE_DATE", "className": "cls_nowrap cls_update_date" },
            { "data": "UPDATE_BY", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_start_date').html(myDateMoment(data.START_DATE));
            $(row).find('.cls_end_date').html(myDateMoment(data.END_DATE));
            $(row).find('.cls_creat_date').html(myDateTimeMoment(data.CREATE_DATE));
            $(row).find('.cls_update_date').html(myDateTimeMoment(data.UPDATE_DATE));
        },
    });
}