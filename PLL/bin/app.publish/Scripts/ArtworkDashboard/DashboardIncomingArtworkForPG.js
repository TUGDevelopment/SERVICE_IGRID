$(document).ready(function () {

    $('.cls_page_dashboard .cls_incoming_artwork_for_pg_date_from').val(GetPreviousDate(90));
    $('.cls_page_dashboard .cls_incoming_artwork_for_pg_date_to').val(GetCurrentDate2());

    if ($(".cls_li_incoming_artwork_for_pg").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_incoming_artwork_for_pg thead tr').clone(true).appendTo('#table_incoming_artwork_for_pg thead');
        $('#table_incoming_artwork_for_pg thead tr:eq(1) th').each(function (i) {
            if (i == 0) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '"  data-index="' + i + '" />');
            }
        });

        var table_incoming_artwork_for_pg = $('#table_incoming_artwork_for_pg').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            fixedColumns: {
                leftColumns: 3
            },
            stateSave: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/dashboard/incomingartworkforpg",
                    url: suburl + "/api/dashboard/incomingartworkforpg?"
                        + 'data.get_by_create_date_from=' + $('.cls_incoming_artwork_for_pg_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_incoming_artwork_for_pg_date_to').val(),
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
                { type: 'znatural', targets: 1 },
                { type: 'znatural', targets: 2 },
            ],
            "order": [[9, 'asc']],
            "processing": true,
            "searching": true,
            "lengthChange": false,
            "ordering": true,
            "info": true,
            "scrollX": true,
            dom: 'Bfrtip',
            buttons: [
                {
                    title: 'Incoming_artwork',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                        format: {
                            body: function (data, row, column, node) {
                                //if (column == 8) {
                                //    return myDateTimeMoment(data);
                                //} else if (column == 10) {
                                //    return myDateMoment(data);
                                //} else {
                                return data;
                                //}
                            }
                        }
                    }
                }
            ],
            "columns": [
                {
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { "data": "ARTWORK_REQUEST_NO", "className": "cls_request_no cls_nowrap" },
                { "data": "REQUEST_ITEM_NO", "className": "cls_request_item_no cls_nowrap" },
                { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "COUNTRY_CODE_SET", "className": "cls_nowrap" },
                { "data": "PRODUCT_CODE_SET", "className": "cls_nowrap" },
                //{ "data": "CREATE_DATE_ARTWORK_REQUEST", "className": "cls_create_date_artwork_request cls_nowrap" },
                {
                    className: "cls_create_date_artwork_request cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateTimeMoment(row.CREATE_DATE_ARTWORK_REQUEST);
                    }
                },
                { "data": "CREATE_BY_ARTWORK_REQUEST_DISPLAY_TXT", "className": "cls_nowrap" },
                //{ "data": "REQUEST_DELIVERY_DATE", "className": "cls_rdd cls_nowrap" }
                {
                    className: "cls_rdd cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.REQUEST_DELIVERY_DATE);
                    }
                },
                { "data": "PLANT", "className": "cls_nowrap" },
            ],
            "rowCallback": function (row, data, index) {
                if (data.REQUEST_ITEM_NO === null) {
                    if (data.ARTWORK_REQUEST_NO === null)
                        $(row).find('.cls_request_item_no').html('<a target="_blank" href="' + suburl + '/ArtworkUpload/' + data.ARTWORK_REQUEST_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_DISPLAY_TXT + '</a>');
                    else
                        $(row).find('.cls_request_item_no').html('<a target="_blank" href="' + suburl + '/Artwork/' + data.ARTWORK_REQUEST_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_DISPLAY_TXT + '</a>');
                }
                else {
                    $(row).find('.cls_request_item_no').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_SUB_ID + '" style="text-decoration: underline;">' + data.REQUEST_ITEM_NO + '</a>');
                }
                $(row).find('.cls_request_no').html('<a target="_blank" href="' + suburl + '/Artwork/' + data.ARTWORK_REQUEST_ID + '" style="text-decoration: underline;">' + data.ARTWORK_REQUEST_NO + '</a>');

                //$(row).find('.cls_create_date_artwork_request').html(myDateTimeMoment(data.CREATE_DATE_ARTWORK_REQUEST));
                //$(row).find('.cls_rdd').html(myDateMoment(data.REQUEST_DELIVERY_DATE));
            },
            "drawCallback": function (settings) {
                $('.cls_cnt_incoming_artwork_for_pg').text('(' + this.api().data().count() + ') ');
            },
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_incoming_artwork_for_pg').text('(' + json.recordsTotal + ') ');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_incoming_artwork_for_pg").click(function (e) {
            table_incoming_artwork_for_pg.ajax.reload();
        });

        $("#view_incoming_artwork_for_pg .cls_btn_excel_incoming_artwork_pg").click(function () {
            $("#view_incoming_artwork_for_pg .buttons-excel").click();
        });

        $(table_incoming_artwork_for_pg.table().container()).on('keyup', 'input', function () {
            table_incoming_artwork_for_pg
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_incoming_artwork_for_pg_filter").hide();
        $("#table_incoming_artwork_for_pg_wrapper .dt-buttons").hide();

        table_incoming_artwork_for_pg.on('order.dt search.dt', function () {
            table_incoming_artwork_for_pg.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_incoming_artwork_for_pg']").on('shown.bs.tab', function (e) {
            table_incoming_artwork_for_pg.columns.adjust().draw();
            //if (table_incoming_artwork_for_pg == null)
            //    table_incoming_artwork_for_pg.columns.adjust().draw();
            //else
            //    table_incoming_artwork_for_pg.ajax.reload();
        });

        table_incoming_artwork_for_pg.search('').columns().search('').draw();
    }
});