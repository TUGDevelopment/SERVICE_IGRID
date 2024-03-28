$(document).ready(function () {

    $('.cls_page_dashboard .cls_inbox_cus_date_from').val(GetPreviousDate(90));
    $('.cls_page_dashboard .cls_inbox_cus_date_to').val(GetCurrentDate2());

    if ($(".cls_li_inbox_customer").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_incoming_inbox_customer thead tr').clone(true).appendTo('#table_incoming_inbox_customer thead');
        $('#table_incoming_inbox_customer thead tr:eq(1) th').each(function (i) {
            if (i == 0) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });

        var table_incoming_inbox_customer = $('#table_incoming_inbox_customer').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            fixedColumns: {
                leftColumns: 3
            },
            select: true,
            stateSave: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/dashboard/inbox",
                    url: suburl + "/api/dashboard/inbox?"
                        + 'data.get_by_create_date_from=' + $('.cls_inbox_cus_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_inbox_cus_date_to').val(),
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
                { type: 'znatural', targets: 3 }
            ],
            "order": [[2, 'asc']],
            "processing": true,
            "lengthChange": false,
            "ordering": true,
            "info": true,
            "searching": true,
            "scrollX": true,
            "autoWidth": false,
            dom: 'Bfrtip',
            buttons: [
                {
                    title: 'Inbox Customer',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [1, 2, 3, 4, 5, 6, 7, 8],
                        format: {
                            body: function (data, row, column, node) {
                                return data;
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
                { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_td_width_350" },
                { "data": "MOCKUP_NO", "className": "cls_mockup_no cls_td_width_150" },
                { "data": "PACKING_TYPE_DISPLAY_TXT", "className": "cls_td_width_140" },
                { "data": "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_td_width_140" },
                { "data": "BRAND_DISPLAY_TXT", "className": "cls_td_width_140" },
                { "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_td_width_350" },
                { "data": "DESTINATION", "className": "cls_td_width_200" },
                { "data": "DUEDATE_DISPLAY_TXT", "className": "cls_td_width_180" }
            ],
            "rowCallback": function (row, data, index) {
                if (data.WORKFLOW_TYPE == "Artwork") {
                    if (data.MOCKUP_SUB_ID != 0) {
                        $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                    }
                    else if (data.CHECK_LIST_NO == "Draft") {
                        $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/ArtworkUpload/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.CHECK_LIST_NO + '</a>');
                    }
                }
                else {
                    if (data.MOCKUP_SUB_ID != 0) {
                        $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                    }
                }

                var y = data.CNT_TOTAL_SUB_WF - data.CNT_TOTAL_SUB_WF_END;
                if (y > 0) {
                    var label_txt = '<span title="' + data.WAITING_STEP + '" class="cls_hand label label-info">'
                        + y
                        + '</span>';

                    if (data.CNT_TOTAL_SUB_WF > 0) {
                        $(row).find('.cls_mockup_no').html($(row).find('.cls_mockup_no').html() + ' ' + label_txt);
                    }
                }
            },
            "drawCallback": function (settings) {
                $('.cls_cnt_incoming_inbox_customer').text('(' + this.api().data().count() + ') ');
            },
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_incoming_inbox_customer').text('(' + json.recordsTotal + ') ');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_inbox_cus").click(function (e) {
            table_incoming_inbox_customer.ajax.reload();
        });

        $("#view_incoming_inbox_customer .cls_btn_excel_inbox_cust").click(function () {
           // ExportToExcel(); // by aof #INC-95891 commented
            $("#view_incoming_inbox_customer .buttons-excel").click();  //by aof #INC-95891 uncommmented
        });

        $(table_incoming_inbox_customer.table().container()).on('keyup', 'input', function () {
            table_incoming_inbox_customer
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_incoming_inbox_customer_filter").hide();
        $("#table_incoming_inbox_customer_wrapper .dt-buttons").hide();

        table_incoming_inbox_customer.on('order.dt search.dt', function () {
            table_incoming_inbox_customer.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_incoming_inbox_customer']").on('shown.bs.tab', function (e) {
            table_incoming_inbox_customer.columns.adjust().draw();
            //if (table_incoming_inbox == null)
            //    table_incoming_inbox.columns.adjust().draw();
            //else
            //    table_incoming_inbox.ajax.reload();
        });

        table_incoming_inbox_customer.search('').columns().search('').draw();
    }
});
function ExportToExcel() {
    $("#table_incoming_inbox_customer").table2excel({
        filename: "Table.xls"
    });
} 