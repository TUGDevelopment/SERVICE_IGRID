$(document).ready(function () {

    $('.cls_page_dashboard .cls_inbox_vn_date_from').val(GetPreviousDate(90));
    $('.cls_page_dashboard .cls_inbox_vn_date_to').val(GetCurrentDate2());

    if ($(".cls_li_inbox_vendor").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_incoming_inbox_vendor thead tr').clone(true).appendTo('#table_incoming_inbox_vendor thead');
        $('#table_incoming_inbox_vendor thead tr:eq(1) th').each(function (i) {
            if (i == 0) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '"  data-index="' + i + '" />');
            }
        });

        var table_incoming_inbox_vendor = $('#table_incoming_inbox_vendor').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            fixedColumns: {
                leftColumns: 3
            },
            stateSave: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/dashboard/inbox",
                    url: suburl + "/api/dashboard/inbox?"
                        + 'data.get_by_create_date_from=' + $('.cls_inbox_vn_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_inbox_vn_date_to').val(),
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
                { type: 'znatural', targets: 2 }
            ],
            "order": [[3, 'asc']],
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
                    title: 'Inbox',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [1, 2, 3, 4, 5, 6, 7, 8],
                        format: {
                            body: function (data, row, column, node) {
                                //if (column == 2) {
                                //    return myDateTimeMoment(data);
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
                //{ "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_td_width_140" },
                //{ "data": "MOCKUP_NO", "className": "cls_mockup_no cls_td_width_150" },
                //{ "data": "CREATE_DATE_PROCESS", "className": "cls_create_date_process cls_td_width_140" },
                ////{ "data": "BRAND_DISPLAY_TXT", "className": "cls_td_width_100" },
                ////{ "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
                ////{ "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
                ////{ "data": "DESTINATION", "className": "cls_td_width_200" },
                //{ "data": "PACKING_TYPE_DISPLAY_TXT", "className": "cls_td_width_140" },
                //{ "data": "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_td_width_140" },
                ////{ "data": "WAITING_STEP", "className": "cls_td_width_400" },
                //{ "data": "WORKFLOW_TYPE", "className": "cls_td_width_110" },
                //{ "data": "DUEDATE_DISPLAY_TXT", "className": "cls_td_width_180" },
                //{ "data": "REQUEST_DELIVERY_DATE", "className": "cls_rdd cls_td_width_70" }

                { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "MOCKUP_NO", "className": "cls_mockup_no cls_nowrap" },
                //{ "data": "CREATE_DATE_PROCESS", "className": "cls_create_date_process cls_nowrap" },
                {
                    className: "cls_create_date_process cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateTimeMoment(row.CREATE_DATE_PROCESS);
                    }
                },
                { "data": "PA_PG_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
                //{ "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
                //{ "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
                //{ "data": "DESTINATION", "className": "cls_td_width_200" },
                { "data": "PACKING_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
                { "data": "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
                //{ "data": "WAITING_STEP", "className": "cls_td_width_400" },
                //{ "data": "WORKFLOW_TYPE", "className": "cls_nowrap" },
                { "data": "DUEDATE_DISPLAY_TXT", "className": "cls_nowrap" },
                //{ "data": "REQUEST_DELIVERY_DATE", "className": "cls_rdd cls_nowrap" }
            ],
            "rowCallback": function (row, data, index) {
                if (data.WORKFLOW_TYPE == "Artwork") {
                    if (data.MOCKUP_SUB_ID == 0) {
                        if (isEmpty(data.REQUEST_FORM_NO))
                            $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/ArtworkUpload/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_DISPLAY_TXT + '</a>');
                        else {
                            $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/Artwork/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                        }
                    }
                    else {
                        $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                    }
                }
                else {
                    if (data.MOCKUP_SUB_ID == 0) {
                        $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                    }
                    else {
                        $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                    }
                }

                //if (!isEmpty(data.CREATE_DATE_PROCESS)) $(row).find('.cls_create_date_process').html(myDateTimeMoment(data.CREATE_DATE_PROCESS));
                //if (!isEmpty(data.REQUEST_DELIVERY_DATE)) $(row).find('.cls_rdd').html(myDateMoment(data.REQUEST_DELIVERY_DATE));
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
                $('.cls_cnt_incoming_inbox_vendor').text('(' + this.api().data().count() + ') ');
            },
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_incoming_inbox_vendor').text('(' + json.recordsTotal + ')');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_inbox_vn").click(function (e) {
            table_incoming_inbox_vendor.ajax.reload();
        });

        $("#view_incoming_inbox_vendor .cls_btn_excel_inbox_vendor").click(function () {
            $("#view_incoming_inbox_vendor .buttons-excel").click();
        });

        $(table_incoming_inbox_vendor.table().container()).on('keyup', 'input', function () {
            table_incoming_inbox_vendor
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_incoming_inbox_vendor_filter").hide();
        $("#table_incoming_inbox_vendor_wrapper .dt-buttons").hide();

        table_incoming_inbox_vendor.on('order.dt search.dt', function () {
            table_incoming_inbox_vendor.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_incoming_inbox_vendor']").on('shown.bs.tab', function (e) {
            table_incoming_inbox_vendor.columns.adjust().draw();
            //if (table_incoming_inbox_vendor == null)
            //    table_incoming_inbox_vendor.columns.adjust().draw();
            //else
            //    table_incoming_inbox_vendor.ajax.reload();
        });

        table_incoming_inbox_vendor.search('').columns().search('').draw();
    }
});
