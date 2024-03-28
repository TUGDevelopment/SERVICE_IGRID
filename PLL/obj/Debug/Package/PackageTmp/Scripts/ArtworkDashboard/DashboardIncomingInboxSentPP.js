$(document).ready(function () {
    $('.cls_page_dashboard .cls_inbox_date_from_sent_pp').val(GetPreviousDate(90));
    $('.cls_page_dashboard .cls_inbox_date_to_sent_pp').val(GetCurrentDate2());

    if ($(".cls_li_inbox_sent_pp").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_incoming_inbox_sent_pp thead tr').clone(true).appendTo('#table_incoming_inbox_sent_pp thead');
        $('#table_incoming_inbox_sent_pp thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 1) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });

        var table_incoming_inbox_sent_pp = $('#table_incoming_inbox_sent_pp').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            fixedColumns: {
                leftColumns: 6
            },
            dom: 'rBfrtip',
            //select: true,
            //buttons: [
            //    {
            //        text: 'Approve selected (GM Marketing)',
            //        className: 'cls_margin_left_5 btn-success cls_btn_gmmk_approveselected cls_hide',
            //        action: function () {
            //            var data = table_incoming_inbox.rows({ selected: true }).data();
            //            approveForm_byGM(data, 'MK', 'APPROVE');
            //        }
            //    }, {
            //        text: 'Approve selected (GM QC)',
            //        className: 'cls_margin_left_5 btn-success cls_btn_gmqc_approveselected cls_hide',
            //        action: function () {
            //            var data = table_incoming_inbox.rows({ selected: true }).data();
            //            approveForm_byGM(data, 'QC', 'APPROVE');
            //        }
            //    }, {
            //        text: 'Not approve selected (GM Marketing)',
            //        className: 'cls_margin_left_5 btn-danger cls_btn_gmmk_not_approveselected cls_hide',
            //        action: function () {
            //            var data = table_incoming_inbox.rows({ selected: true }).data();
            //            approveForm_byGM(data, 'MK', 'NOTAPPROVE');
            //        }
            //    }, {
            //        text: 'Not approve selected (GM QC)',
            //        className: 'cls_margin_left_5 btn-danger cls_btn_gmqc_not_approveselected cls_hide',
            //        action: function () {
            //            var data = table_incoming_inbox.rows({ selected: true }).data();
            //            approveForm_byGM(data, 'QC', 'NOTAPPROVE');
            //        }
            //    }
            //],
            //select: {
            //    'style': 'multi'
            //},
            stateSave: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/dashboard/inbox",
                    url: suburl + "/api/dashboard/inbox?"
                        + 'data.get_by_create_date_from=' + $('.cls_inbox_date_from_sent_pp').val() + '&data.get_by_create_date_to=' + $('.cls_inbox_date_to_sent_pp').val() + '&data.sent_pp=true',
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
                    "targets": 1
                },
                { type: 'znatural', targets: 3 },

            ],
            "order": [[5, 'asc']],
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
                    title: 'Inbox_sent_pp',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21],    //#INC-78189 by aof added 21 in the basket.
                        format: {
                            body: function (data, row, column, node) {
                                //if (column == 4) {
                                //    return myDateTimeMoment(data);
                                //} else if (column == 13) {
                                //    return myDateMoment(data);
                                //} else if (column == 14) {
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
                    data: null,
                    defaultContent: '',
                    className: 'cls_hide',
                    orderable: false,
                },
                {
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_current_step cls_td_width_140" },
                { "data": "CHECK_LIST_NO", "className": "cls_checklist_no cls_nowrap" },
                { "data": "REFERENCE_REQUEST_NO", "className": "cls_ref_request_no cls_td_width_150" },
                { "data": "MOCKUP_NO", "className": "cls_mockup_no cls_td_width_160" },
                //{ "data": "CREATE_DATE_PROCESS", "className": "cls_create_date_process cls_td_width_140" },
                {
                    className: "cls_create_date_process cls_td_width_140",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateTimeMoment(row.CREATE_DATE_PROCESS);
                    }
                },
                { "data": "PLANT", "className": "cls_nowrap" },
                { "data": "PA_PG_DISPLAY_TXT", "className": "cls_nowrap" },

                { "data": "BRAND_DISPLAY_TXT", "className": "cls_td_width_140" },

                { "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_td_width_350" },
                { "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_td_width_350" },
                { "data": "DESTINATION", "className": "cls_td_width_200" },
                { "data": "PACKING_TYPE_DISPLAY_TXT", "className": "cls_td_width_140" },
                { "data": "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_td_width_140" },
                { "data": "DUEDATE_DISPLAY_TXT", "className": "cls_td_width_140" },
                //{ "data": "REQUEST_DELIVERY_DATE", "className": "cls_rdd cls_td_width_70" },
                {
                    className: "cls_rdd cls_td_width_70",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.REQUEST_DELIVERY_DATE);
                    }
                },

                //{ "data": "LAST_UPDATE_DATE_WF", "className": "cls_last_update_date_wf cls_td_width_140" },
                {
                    className: "cls_last_update_date_wf cls_td_width_140",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateTimeMoment(row.LAST_UPDATE_DATE_WF);
                    }
                },
                { "data": "SO_AND_ITEM_NO", "className": "cls_nowrap" },
                { "data": "SALES_ORG", "className": "cls_nowrap" },
                { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
                { "data": "MAT5", "className": "cls_nowrap" },
            ],
            "rowCallback": function (row, data, index) {
                if (data.WORKFLOW_TYPE == "Artwork") {
                    if (data.MOCKUP_SUB_ID == 0) {

                        if (data.CHECK_LIST_NO == "Draft")
                            $(row).find('.cls_checklist_no').html('<a target="_blank" href="' + suburl + '/ArtworkUpload/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.CHECK_LIST_NO + '</a>');
                        else {
                            $(row).find('.cls_checklist_no').html('<a target="_blank" href="' + suburl + '/Artwork/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.CHECK_LIST_NO + '</a>');
                            if (data.REFERENCE_REQUEST_NO != null) {
                                $(row).find('.cls_ref_request_no').html('<a target="_blank" href="' + suburl + '/Artwork/' + data.REFERENCE_REQUEST_ID + '" style="text-decoration: underline;">' + data.REFERENCE_REQUEST_NO + '</a>');
                            }
                        }
                    }
                    else {
                        $(row).find('.cls_checklist_no').html('<a target="_blank" href="' + suburl + '/Artwork/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.CHECK_LIST_NO + '</a>');
                        $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                        if (data.REFERENCE_REQUEST_NO != null) {
                            $(row).find('.cls_ref_request_no').html('<a target="_blank" href="' + suburl + '/Artwork/' + data.REFERENCE_REQUEST_ID + '" style="text-decoration: underline;">' + data.REFERENCE_REQUEST_NO + '</a>');
                            //$(row).find('.cls_ref_request_no').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.REFERENCE_REQUEST_NO + '</a>');
                        }
                    }
                }
                else {
                    if (data.MOCKUP_SUB_ID == 0) {
                        $(row).find('.cls_checklist_no').html('<a target="_blank" href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.CHECK_LIST_NO + '</a>');
                        if (data.REFERENCE_REQUEST_NO != null) {
                            $(row).find('.cls_ref_request_no').html('<a target="_blank" href="' + suburl + '/CheckList/' + data.REFERENCE_REQUEST_ID + '" style="text-decoration: underline;">' + data.REFERENCE_REQUEST_NO + '</a>');
                        }
                    }
                    else {
                        $(row).find('.cls_checklist_no').html('<a target="_blank" href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + data.CHECK_LIST_NO + '</a>');
                        $(row).find('.cls_mockup_no').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                        if (data.REFERENCE_REQUEST_NO != null) {
                            $(row).find('.cls_ref_request_no').html('<a target="_blank" href="' + suburl + '/CheckList/' + data.REFERENCE_REQUEST_ID + '" style="text-decoration: underline;">' + data.REFERENCE_REQUEST_NO + '</a>');
                            //$(row).find('.cls_ref_request_no').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.REFERENCE_REQUEST_NO + '</a>');
                        }
                    }
                }

                //if (!isEmpty(data.CREATE_DATE_PROCESS))
                //    $(row).find('.cls_create_date_process').html(myDateTimeMoment(data.CREATE_DATE_PROCESS));

                //if (!isEmpty(data.LAST_UPDATE_DATE_WF))
                //    $(row).find('.cls_last_update_date_wf').html(myDateTimeMoment(data.LAST_UPDATE_DATE_WF));

                //if (!isEmpty(data.REQUEST_DELIVERY_DATE))
                //    $(row).find('.cls_rdd').html(myDateMoment(data.REQUEST_DELIVERY_DATE));

                if (data.CNT_TOTAL_SUB_WF_NOT_END > 0) {
                    var label_txt = '<span style="cursor: default;" title="' + data.WAITING_STEP + '" class="label label-info">'
                        + data.CNT_TOTAL_SUB_WF_NOT_END
                        + '</span>';

                    if (data.CNT_TOTAL_SUB_WF_NOT_END > 0) {
                        $(row).find('.cls_mockup_no').html($(row).find('.cls_mockup_no').html() + ' ' + label_txt);
                    }
                }

                if (data.CNT_TOTAL_SUB_WF_END > 0) {
                    var label_txt = '<span style="cursor: default;" title="' + data.END_STEP + '" class="label label-success">'
                        + data.CNT_TOTAL_SUB_WF_END
                        + '</span>';

                    if (data.CNT_TOTAL_SUB_WF_END > 0) {
                        $(row).find('.cls_mockup_no').html($(row).find('.cls_mockup_no').html() + ' ' + label_txt);
                    }
                }

                if ($(row).find('.cls_checklist_no').text() == $(row).find('.cls_ref_request_no').text()) {
                    $(row).find('.cls_ref_request_no').text("");
                }
            },
            "drawCallback": function (settings) {
                $('.cls_cnt_incoming_inbox_sent_pp').text('(' + this.api().data().count() + ') ');
            },
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_incoming_inbox').text('(' + json.recordsTotal + ') ');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_inbox_sent_pp").click(function (e) {
            $(".cls_badge_warning_so_change_sent_pp").hide();
            table_incoming_inbox_sent_pp.ajax.reload();
        });

        $(".cls_page_dashboard .cls_btn_refresh_inbox_sent_pp").click(function (e) {
            refresh_SO_update_sent_pp();
        });

        $(table_incoming_inbox_sent_pp.table().container()).on('keyup', 'input', function () {
            table_incoming_inbox_sent_pp
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_incoming_inbox_sent_pp_filter").hide();
        $("#table_incoming_inbox_sent_pp_wrapper .dt-buttons").hide();

        table_incoming_inbox_sent_pp.on('order.dt search.dt', function () {
            table_incoming_inbox_sent_pp.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_incoming_inbox_sent_pp']").on('shown.bs.tab', function (e) {
            table_incoming_inbox_sent_pp.columns.adjust().draw();
            //if (table_incoming_inbox == null)
            //    table_incoming_inbox.columns.adjust().draw();
            //else
            //    table_incoming_inbox.ajax.reload();
        });

        table_incoming_inbox_sent_pp.search('').columns().search('').draw();

        //approve all of gm qa and customer
        //if (!(getroleuser("GM_QC") || getroleuser("MK_GM"))) {
        //    $('.cls_btn_gmmk_approveselected').parent().hide();
        //    table_incoming_inbox.columns('.select-checkbox').visible(false);
        //    //table_incoming_inbox.columns('.cls_request_no').visible(false);
        //    table_incoming_inbox.select.style('api');
        //} else {
        //    if (getroleuser("GM_QC")) {
        //        $('.cls_btn_gmmk_approveselected').parent().show();
        //        table_incoming_inbox.columns('.select-checkbox').visible(true);
        //        //table_incoming_inbox.columns('.cls_request_no').visible(true);
        //        table_incoming_inbox.select.style('multi');
        //        $('.cls_btn_gmqc_approveselected').show();
        //        $('.cls_btn_gmqc_not_approveselected').show();
        //    }
        //    if (getroleuser("MK_GM")) {
        //        $('.cls_btn_gmmk_approveselected').parent().show();
        //        table_incoming_inbox.columns('.select-checkbox').visible(true);
        //        //table_incoming_inbox.columns('.cls_request_no').visible(true);
        //        table_incoming_inbox.select.style('multi');
        //        $('.cls_btn_gmmk_approveselected').show();
        //        $('.cls_btn_gmmk_not_approveselected').show();
        //    }
        //}

        $("#view_incoming_inbox_sent_pp .cls_btn_excel_inbox_sent_pp").click(function () {
            $("#view_incoming_inbox_sent_pp .buttons-excel").click();
        });
    }
});

function refresh_SO_update_sent_pp() {
    var table = $('#table_incoming_inbox_sent_pp').DataTable();
    var listArtworkSubId = [];

    //get all artworkSubId from datatable
    $.each(table.data(), function (index, value) {
        if (value.CURRENT_STEP_DISPLAY_TXT == 'Assigned PA') {
            listArtworkSubId.push(value.MOCKUP_SUB_ID);
        }
    });

    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item["LIST_ARTWORK_SUB_ID"] = listArtworkSubId;
    jsonObj.data = item;

    //call api    
    if ($('.se-pre-con').is(':visible')) { }
    else { $(".se-pre-con").show(); }

    var cntColumn = table.columns(':visible').count();

    $.ajax({
        url: suburl + '/api/dashboard/inbox/issoupdate',
        type: 'POST',
        data: jsonObj,
        success: function (res) {
            res = DData(res);

            if (res.data.length > 0) {
                $(".cls_badge_warning_so_change_sent_pp").text(res.data.length).show();
                table.cells().nodes().to$().removeClass('isupdate-highlight');
                table.cells().nodes().to$().attr('title', '');
                $.each(res.data, function (indexResult, valueResult) {
                    $.each(table.data(), function (index, value) {
                        if (valueResult.MOCKUP_SUB_ID == value.MOCKUP_SUB_ID) {
                            var ii = 0;
                            while (ii <= cntColumn) {
                                table.rows(index).cell(index, ii).nodes().to$().addClass('isupdate-highlight');
                                table.rows(index).cell(index, ii).nodes().to$().attr('title', 'SO changed');
                                ii++;
                            }
                            return;
                        }
                    });
                });

                table.draw();
            }

            $(".se-pre-con").fadeOut('fast');
        }
    });
}

