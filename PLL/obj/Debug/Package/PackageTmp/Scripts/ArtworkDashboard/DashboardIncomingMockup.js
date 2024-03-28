$(document).ready(function () {

    $('.cls_page_dashboard .cls_incoming_mockup_date_from').val(GetPreviousDate(90));
    $('.cls_page_dashboard .cls_incoming_mockup_date_to').val(GetCurrentDate2());

    if ($(".cls_li_incoming_mockup").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_incoming_mockup thead tr').clone(true).appendTo('#table_incoming_mockup thead');
        $('#table_incoming_mockup thead tr:eq(1) th').each(function (i) {
            if (i == 0) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '"/>');
            }
        });

        var table_incoming_mockup = $('#table_incoming_mockup').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            fixedColumns: {
                leftColumns: 3
            },
            stateSave: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/dashboard/incomingmockup",
                    url: suburl + "/api/dashboard/incomingmockup?"
                        + 'data.get_by_create_date_from=' + $('.cls_incoming_mockup_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_incoming_mockup_date_to').val(),
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
                { type: 'znatural', targets: 2 }
            ],
            dom: 'Bfrtip',
            buttons: [
                {
                    title: 'Incoming_mockup',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                        format: {
                            body: function (data, row, column, node) {
                                //if (column == 9) {
                                //    return myDateTimeMoment(data);
                                //} else {
                                return data;
                                //}
                            }
                        }
                    }
                }
            ],
            "order": [[10, 'asc']],
            "processing": true,
            "searching": true,
            "lengthChange": false,
            "ordering": true,
            "info": true,
            "scrollX": true,
            "columns": [
                {
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { "data": "CHECK_LIST_NO", "className": "cls_checklist_no cls_nowrap" },
                { "data": "MOCKUP_NO", "className": "cls_mockup_no cls_nowrap" },
                //{ "data": "", "className": "cls_type cls_nowrap" },
                { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "COUNTRY_CODE_SET", "className": "cls_nowrap" },
                { "data": "BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "PACKING_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "PRODUCT_CODE_SET", "className": "cls_nowrap" },
                //{ "data": "CREATE_DATE_CHECK_LIST", "className": "cls_create_date_check_list cls_nowrap" },
                {
                    className: "cls_create_date_check_list cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateTimeMoment(row.CREATE_DATE_CHECK_LIST);
                    }
                },
                { "data": "CREATE_BY_CHECK_LIST_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "PLANT", "className": "cls_nowrap" }
            ],
            "rowCallback": function (row, data, index) {
                $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + ((data.MOCKUP_NO === null) ? data.CURRENT_STEP_DISPLAY_TXT : data.MOCKUP_NO) + '</a>');
                //$(row).find('.cls_type').html('From check list');
                //$(row).find('.cls_create_date_check_list').html(myDateTimeMoment(data.CREATE_DATE_CHECK_LIST));

                $(row).find('.cls_checklist_no').html('<a target="_blank" href="' + suburl + '/Checklist/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.CHECK_LIST_NO + '</a>');

            },
            "drawCallback": function (settings) {
                $('.cls_cnt_incoming_mockup').text('(' + this.api().data().count() + ') ');
            },
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_incoming_mockup').text('(' + json.recordsTotal + ') ');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_incoming_mockup").click(function (e) {
            table_incoming_mockup.ajax.reload();
        });

        $("#view_incoming_mockup .cls_btn_excel_incoming_mockup").click(function () {
            $("#view_incoming_mockup .buttons-excel").click();
        });

        $(table_incoming_mockup.table().container()).on('keyup', 'input', function () {
            table_incoming_mockup
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_incoming_mockup_filter").hide();
        $("#table_incoming_mockup_wrapper .dt-buttons").hide();

        table_incoming_mockup.on('order.dt search.dt', function () {
            table_incoming_mockup.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_incoming_mockup']").on('shown.bs.tab', function (e) {
            table_incoming_mockup.columns.adjust().draw();
            //if (table_incoming_mockup == null)
            //    table_incoming_mockup.columns.adjust().draw();
            //else
            //    table_incoming_mockup.ajax.reload();
        });

        table_incoming_mockup.search('').columns().search('').draw();
    }
});