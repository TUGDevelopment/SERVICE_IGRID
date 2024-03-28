var first_load_new = true;
$(document).ready(function () {

    $('.cls_page_dashboard .cls_so_new_create_date_from').val('');
    $('.cls_page_dashboard .cls_so_new_create_date_to').val('');
    $('.cls_page_dashboard .cls_so_new_rdd_from').val(GetCurrentDate2);
    $('.cls_page_dashboard .cls_so_new_rdd_to').val(GetNextDate(60));

    if ($(".cls_li_incoming_so_new").is(':visible')) {
        $('#table_so_new thead tr').clone(true).appendTo('#table_so_new thead');
        $('#table_so_new thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 1) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });

        var groupColumn = 1;
        var table_so_new = $('#table_so_new').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    url: suburl + "/api/dashboard/incomingsalesordernew?"
                        + 'data.get_by_create_date_from=' + $('.cls_so_new_create_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_so_new_create_date_to').val()
                        + '&data.get_by_rdd_from=' + $('.cls_so_new_rdd_from').val() + '&data.get_by_rdd_to=' + $('.cls_so_new_rdd_to').val()
                        + '&data.get_by_packaging_type=' + encodeURIComponent($('.cls_lov_new_packaging_type option:selected').text())
                        + '&data.get_by_sold_to=' + encodeURIComponent($('.cls_lov_new_sold_to option:selected').text())
                        + '&data.get_by_ship_to=' + encodeURIComponent($('.cls_lov_new_ship_to option:selected').text())
                        + '&data.get_by_brand=' + encodeURIComponent($('.cls_lov_new_brand option:selected').text())
                        + '&data.first_load=' + first_load_new
                        + '&data.grouping=' + $('.cls_lov_so_new_group').val(),
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
                },

                { "visible": false, "targets": groupColumn },
                { "visible": false, "targets": 2 }
            ],
            "scrollX": true,
            "lengthChange": false,
            "scrollY": "350px",
            "scrollCollapse": true,
            "paging": false,
            dom: 'Bfrtip',
            buttons: [
                {
                    title: 'Incoming_sales_order_new',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19],
                        format: {
                            body: function (data, row, column, node) {
                                //if (column == 14 || column == 15) {
                                //    return myDateMoment(data);
                                //} else {
                                return data;
                                //}
                            }
                        }
                    }
                }
            ],
            columns: [
                {
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { data: "GROUPING", "className": "" },
                { data: "GROUPING_DISPLAY_TXT", "className": "" },
                { data: "SALES_ORDER_NO", "className": "cls_nowrap" },
                { data: "ITEM", "className": "cls_nowrap" },
                { data: "PRODUCT_CODE", "className": "cls_nowrap" },
                { data: "SALES_ORG", "className": "cls_nowrap" },
                { data: "PRODUCTION_PLANT", "className": "cls_nowrap" },
                { data: "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "SHIP_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "ADDITIONAL_BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "COUNTRY", "className": "cls_nowrap" },
                { data: "IN_TRANSIT_TO", "className": "cls_nowrap" },
                { data: "COMPONENT_ITEM", "className": "cls_nowrap" },
                { data: "COMPONENT_MATERIAL", "className": "cls_nowrap" },
                { data: "DECRIPTION", "className": "cls_nowrap" },
                { data: "BOM_ITEM_CUSTOM_1", "className": "cls_nowrap" },
                //{ data: "CREATE_ON", "className": "cls_nowrap cls_create_on" },
                {
                    className: "cls_create_on cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.CREATE_ON);
                    }
                },
                //{ data: "RDD", "className": "cls_nowrap cls_rdd" },
                {
                    className: "cls_rdd cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.RDD);
                    }
                },

            ],
            "drawCallback": function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;
                var j = 1;
                //+++++++++++++++++++
                var val = $(".cls_lov_so_new_group").val();
                if (val == "yes") {
                    api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                        var str_moredetails = "";
                        var str_grouping = "";
                        for (var x = 0; x < rows.data().length; x++) {
                            if (rows.data()[x].GROUPING == group) {
                                str_moredetails = replaceHtml(rows.data()[x].GROUPING_DISPLAY_TXT);
                                str_grouping = rows.data()[x].GROUPING;
                            }
                        }

                        if (last !== group) {
                            $(rows).eq(i).before(
                                '<tr class="group highlight"><td colspan="18"> Group ' + (j) + ' <span title="' + str_moredetails + '" class="cls_hand label label-info">more details</span></td></tr>'
                            );
                            last = group;
                            j++;
                        }
                    });
                }
                $('.cls_cnt_incoming_so_new').text('(' + api.rows().data().count() + ') ');
            },
            //select: {
            //    'style': 'multi'
            //},
            "processing": true,
            "rowCallback": function (row, data, index) {
                //if (data.RDD != "") {
                //    $(row).find('.cls_rdd').html(myDateMoment(data.RDD));
                //}
                //if (data.CREATE_ON != "") {
                //    $(row).find('.cls_create_on').html(myDateMoment(data.CREATE_ON));
                //}

            },
            order: [[3, 'asc']],
            "orderFixed": [1, 'asc'],
            //initComplete: function (settings, json) {
            //    $('.cls_cnt_incoming_so_new').text('(' + json.recordsTotal + ') ');
            //}
        });

        $(table_so_new.table().container()).on('keyup', 'input', function () {
            table_so_new
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        $('table').on('click', function (e) {
            if ($('.popoverButton').length > 1)
                $('.popoverButton').popover('hide');
            $(e.target).popover('toggle');
        });

        $("#table_so_new_filter").hide();
        $("#table_so_new_wrapper .dt-buttons").hide();

        table_so_new.on('order.dt search.dt', function () {
            table_so_new.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_so_new']").on('shown.bs.tab', function (e) {
            table_so_new.columns.adjust().draw();
        });

        $(".cls_page_dashboard .cls_btn_search_so_new").click(function (e) {
            table_so_new.ajax.reload();
        });

        $("#view_so_new .cls_btn_excel_so_new").click(function () {
            $("#view_so_new .buttons-excel").click();
        });
    }

    bind_lov('.cls_lov_new_packaging_type', '/api/lov/packtype', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_new_sold_to', '/api/lov/customersoldtoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_new_ship_to', '/api/lov/customershiptoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_new_brand', '/api/lov/brandso', 'data.DISPLAY_TXT');

    first_load_new = false;
});

