var first_load = true;
$(document).ready(function () {

    bind_lov('.cls_report_outstanding .cls_lov_search_company', '/api/lov/company', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_outstanding .cls_lov_search_sold_to', '/api/lov/customersoldtoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_outstanding .cls_lov_search_ship_to', '/api/lov/customershiptoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_outstanding .cls_lov_search_brand', '/api/lov/brandso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_outstanding .cls_lov_search_country', '/api/lov/countryso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_outstanding .cls_lov_search_product_code', '/api/lov/productso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_outstanding .cls_lov_search_order_bom_component', '/api/lov/bomso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_outstanding .cls_lov_search_pic', '/api/lov/userpackaging', 'data.DISPLAY_TXT');

    $('.cls_table_report_outstanding thead tr').clone(true).appendTo('.cls_table_report_outstanding thead');
    $('.cls_table_report_outstanding thead tr:eq(1) th').each(function (i) {
        if (i == 0 || i == 17 || i == 18) {
            $(this).html('');
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    bind_table_report_outstanding();
    first_load = false;

    $(table_report_outstanding.table().container()).on('keyup', 'input', function () {
        table_report_outstanding
            .column($(this).data('index'))
            .search(this.value)
            .draw(false);
    });

    $(".cls_report_outstanding .cls_btn_clr").click(function () {
        $('.cls_report_outstanding input[type=text]').val('');
        $('.cls_report_outstanding input[type=checkbox]').prop('checked', false);
        $('.cls_report_outstanding textarea').val('');
        $('.cls_report_outstanding select').not('.dataTables_length select').val('').change();
    });

    $(".cls_report_outstanding form").submit(function (e) {
        table_report_outstanding.ajax.reload();
        e.preventDefault();
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box

    $(".cls_report_outstanding .cls_chk_send_to_pp, .cls_report_outstanding .cls_chk_flag_send_to_pp, .cls_report_outstanding .cls_chk_po_created").change(function () {
        if (this.checked) {
            $(".cls_report_outstanding .cls_chk_wf_created").prop("disabled", true);
            $(".cls_report_outstanding .cls_chk_wf_created").prop('checked', true);
        }
        else {
            if (!$('.cls_report_outstanding .cls_chk_po_created').is(":checked")
                && !$('.cls_report_outstanding .cls_chk_flag_send_to_pp').is(":checked")
                && !$('.cls_report_outstanding .cls_chk_send_to_pp').is(":checked")) {
                $(".cls_report_outstanding .cls_chk_wf_created").prop("disabled", false);
            }
        }
    });

    $(".cls_report_outstanding .cls_btn_export_excel").click(function () {
        window.open("/excel/outstandingreport?data.sales_order_no=" + $('.cls_report_outstanding .cls_search_so_no').val().replace(/\n/g, ',')
            + "&data.workflow_created=" + $('.cls_report_outstanding .cls_chk_wf_created').is(":checked")
            + "&data.flag_send_to_pp=" + $('.cls_report_outstanding .cls_chk_flag_send_to_pp').is(":checked")
            + "&data.send_to_pp=" + $('.cls_report_outstanding .cls_chk_send_to_pp').is(":checked")
            + "&data.po_created=" + $('.cls_report_outstanding .cls_chk_po_created').is(":checked")
            + "&data.search_brand_name=" + $('.cls_report_outstanding .cls_lov_search_brand option:selected').text()
            + "&data.search_country_name=" + $('.cls_report_outstanding .cls_lov_search_country option:selected').text()
            + "&data.search_sold_to_name=" + $('.cls_report_outstanding .cls_lov_search_sold_to option:selected').text()
            + "&data.search_ship_to_name=" + $('.cls_report_outstanding .cls_lov_search_ship_to option:selected').text()
            + "&data.search_product_code=" + $('.cls_report_outstanding .cls_lov_search_product_code option:selected').text()
            + "&data.search_bom_component=" + $('.cls_report_outstanding .cls_lov_search_order_bom_component option:selected').text()
            + "&data.search_pic=" + $('.cls_report_outstanding .cls_lov_search_pic option:selected').text()
            + "&data.search_pic_id=" + $('.cls_report_outstanding .cls_lov_search_pic').val()

            + "&data.search_rdd_date_from=" + $('.cls_report_outstanding .cls_search_rdd_from').val()
            + "&data.search_rdd_date_to=" + $('.cls_report_outstanding .cls_search_rdd_to').val()
            + "&data.search_so_create_date_from=" + $('.cls_report_outstanding .cls_search_so_created_from').val()
            + "&data.search_so_create_date_to=" + $('.cls_report_outstanding .cls_search_so_created_to').val()
            + "&data.search_so_item_from=" + $('.cls_report_outstanding .cls_search_so_item_from').val()
            + "&data.search_so_item_to=" + $('.cls_report_outstanding .cls_search_so_item_to').val()
            + "&data.search_company=" + (isEmpty($('.cls_report_outstanding .cls_lov_search_company').val()) ? '' : $('.cls_report_outstanding .cls_lov_search_company').val())
            + "&data.GENERATE_EXCEL=X"

            , '_blank');
    });
});

var table_report_outstanding;
function bind_table_report_outstanding() {
    table_report_outstanding = $('#table_report_outstanding').DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHeader: true,
        //fixedColumns: {
        //    leftColumns: 3
        //},

        ajax: function (data, callback, settings) {

            //for (var i = 0, len = data.columns.length; i < len; i++) {
            //    delete data.columns[i].name;
            //    delete data.columns[i].data;
            //    delete data.columns[i].searchable;
            //    delete data.columns[i].orderable;
            //    delete data.columns[i].search.regex;
            //    delete data.search.regex;
            //}

            $.ajax({
                url: suburl + "/api/report/outstandingreport?data.sales_order_no=" + $('.cls_report_outstanding .cls_search_so_no').val().replace(/\n/g, ',')
                    + "&data.workflow_created=" + $('.cls_report_outstanding .cls_chk_wf_created').is(":checked")
                    + "&data.flag_send_to_pp=" + $('.cls_report_outstanding .cls_chk_flag_send_to_pp').is(":checked")
                    + "&data.send_to_pp=" + $('.cls_report_outstanding .cls_chk_send_to_pp').is(":checked")
                    + "&data.po_created=" + $('.cls_report_outstanding .cls_chk_po_created').is(":checked")
                    + "&data.search_brand_name=" + $('.cls_report_outstanding .cls_lov_search_brand option:selected').text()
                    + "&data.search_country_name=" + $('.cls_report_outstanding .cls_lov_search_country option:selected').text()
                    + "&data.search_sold_to_name=" + $('.cls_report_outstanding .cls_lov_search_sold_to option:selected').text()
                    + "&data.search_ship_to_name=" + $('.cls_report_outstanding .cls_lov_search_ship_to option:selected').text()
                    + "&data.search_product_code=" + $('.cls_report_outstanding .cls_lov_search_product_code option:selected').text()
                    + "&data.search_bom_component=" + $('.cls_report_outstanding .cls_lov_search_order_bom_component option:selected').text()
                    + "&data.search_pic=" + $('.cls_report_outstanding .cls_lov_search_pic option:selected').text()
                    + "&data.search_pic_id=" + $('.cls_report_outstanding .cls_lov_search_pic').val()
                    + "&data.first_load=" + first_load
                    + "&data.search_rdd_date_from=" + $('.cls_report_outstanding .cls_search_rdd_from').val()
                    + "&data.search_rdd_date_to=" + $('.cls_report_outstanding .cls_search_rdd_to').val()
                    + "&data.search_so_create_date_from=" + $('.cls_report_outstanding .cls_search_so_created_from').val()
                    + "&data.search_so_create_date_to=" + $('.cls_report_outstanding .cls_search_so_created_to').val()
                    + "&data.search_so_item_from=" + $('.cls_report_outstanding .cls_search_so_item_from').val()
                    + "&data.search_so_item_to=" + $('.cls_report_outstanding .cls_search_so_item_to').val()
                    + "&data.search_company=" + (isEmpty($('.cls_report_outstanding .cls_lov_search_company').val()) ? '' : $('.cls_report_outstanding .cls_lov_search_company').val())
                ,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "searchable": false, "orderable": false, "targets": 0 },
            { "searchable": false, "orderable": false, "targets": 17 },
            { "searchable": false, "orderable": false, "targets": 18 }
        ],
        "order": [[1, 'asc']],
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
            { "data": "SALES_ORDER_NO", "className": "cls_nowrap" },
            { "data": "ITEM", "className": "cls_nowrap" },
            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
            { "data": "MATERIAL_DESCRIPTION", "className": "cls_nowrap" },
            { "data": "ORDER_QTY", "className": "cls_nowrap" },
            { "data": "ORDER_UNIT", "className": "cls_nowrap" },
            { "data": "REJECTION_CODE", "className": "cls_nowrap" },
            { "data": "PRODUCTION_PLANT", "className": "cls_nowrap" },
            { "data": "COMPONENT_ITEM", "className": "cls_nowrap" },
            { "data": "COMPONENT_MATERIAL", "className": "cls_nowrap" },
            { "data": "DECRIPTION", "className": "cls_nowrap" },
            { "data": "BOM_ITEM_CUSTOM_1", "className": "cls_nowrap" },
            { "data": "QUANTITY_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "CURRENT_WF_STATUS", "className": "cls_nowrap" },
            { "data": "REQUEST_ITEM_NO", "className": "cls_wf_number cls_nowrap" },

            { "data": "READY_CREATE_PO_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "SEND_TO_PP_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "PO_CREATED_DISPLAY_TXT", "className": "cls_nowrap" },

            { "data": "STOCK_DISPLAY_TXT", "className": "cls_nowrap" },

            { "data": "CREATE_ON", "className": "cls_create_on cls_nowrap" },
            { "data": "RDD", "className": "cls_rdd cls_nowrap" },

            { "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "PIC_DISPLAY_TXT", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_wf_number').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_SUB_ID + '" style="text-decoration: underline;">' + data.REQUEST_ITEM_NO + '</a>');
            $(row).find('.cls_create_on').html(myDateMoment(data.CREATE_ON));
            $(row).find('.cls_rdd').html(myDateMoment(data.RDD));

            //if (data.CHECK_WF == "1") {
            //    $(row).find('td').addClass('isupdate-highlight');
            //    var msg = "Please check this workflow";
            //    $(row).find('td').prop('title', msg)
            //    $(row).find('.cls_wf_number').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_SUB_ID + '" style="text-decoration: underline;">' + data.REQUEST_ITEM_NO + '<span class="badge badge-error" title="' + msg + '">!</span>' + '</a>');
            //}
        },
    });
}

