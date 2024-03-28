$(document).ready(function () {
    $('.cls_page_dashboard .cls_inbox_date_from').val(GetPreviousDate(90));
    $('.cls_page_dashboard .cls_inbox_date_to').val(GetCurrentDate2());

    get_user_role();
   
    //start #TSK-1511 #SR-70695 by aof in 09/2022
    var col_export_excel_wf_status = 2;  
    var f_vis_col_wf_status = false; 
    if (isQCPosition("QC"))
    {
        col_export_excel_wf_status = 6;
        f_vis_col_wf_status = true;
    }
    //end #TSK-1511 #SR-70695 by aof in 09/2022
    

    if ($(".cls_li_inbox").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_incoming_inbox thead tr').clone(true).appendTo('#table_incoming_inbox thead');
        $('#table_incoming_inbox thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 1) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });

        var table_incoming_inbox = $('#table_incoming_inbox').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            fixedColumns: {
                leftColumns: 6
            },
            dom: 'rBfrtip',
            select: true,
            buttons: [
                {
                    text: 'Approve selected (GM Marketing)',
                    className: 'cls_margin_left_5 btn-success cls_btn_gmmk_approveselected cls_hide',
                    action: function () {
                        var data = table_incoming_inbox.rows({ selected: true }).data();
                        approveForm_byGM(data, 'MK', 'APPROVE');
                    }
                }, {
                    text: 'Approve selected (GM QC)',
                    className: 'cls_margin_left_5 btn-success cls_btn_gmqc_approveselected cls_hide',
                    action: function () {
                        var data = table_incoming_inbox.rows({ selected: true }).data();
                        approveForm_byGM(data, 'QC', 'APPROVE');
                    }
                }, {
                    text: 'Not approve selected (GM Marketing)',
                    className: 'cls_margin_left_5 btn-danger cls_btn_gmmk_not_approveselected cls_hide',
                    action: function () {
                        var data = table_incoming_inbox.rows({ selected: true }).data();
                        approveForm_byGM(data, 'MK', 'NOTAPPROVE');
                    }
                }, {
                    text: 'Not approve selected (GM QC)',
                    className: 'cls_margin_left_5 btn-danger cls_btn_gmqc_not_approveselected cls_hide',
                    action: function () {
                        var data = table_incoming_inbox.rows({ selected: true }).data();
                        approveForm_byGM(data, 'QC', 'NOTAPPROVE');
                    }
                }
            ],
            select: {
                'style': 'multi'
            },
            stateSave: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/dashboard/inbox",
                    url: suburl + "/api/dashboard/inbox?"
                        + 'data.get_by_create_date_from=' + $('.cls_inbox_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_inbox_date_to').val(),
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
                    title: 'Inbox',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [2, 3, 4, 5, col_export_excel_wf_status, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23], //#TSK-1511 #SR-70695 by aof in 09/2022 for adding 23
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
                    className: 'select-checkbox',
                    orderable: false
                },
                {
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_current_step cls_td_width_140" },
                { "data": "CHECK_LIST_NO", "className": "cls_checklist_no cls_nowrap" },
                { "data": "REFERENCE_REQUEST_NO", "className": "cls_ref_request_no cls_td_width_130" },
                { "data": "MOCKUP_NO", "className": "cls_mockup_no cls_td_width_160" },
                { "data": "WF_STATUS", "className": "cls_wf_status cls_td_width_110" },   //#TSK-1511 #SR-70695 by aof in 09/2022 
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
                { "data": "CURRENT_ASSIGN", "className": "cls_nowrap" },
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

                if (data.PP_SEND_BACK) {
                    $(row).find('td').addClass('yellow-highlight');
                    $(row).find('td').prop('title', 'This item has been send back from PP team : ' + data.PP_SEND_BACK_COMMENT);
                }
            },
            "drawCallback": function (settings) {
                $('.cls_cnt_incoming_inbox').text('(' + this.api().data().count() + ') ');
            },
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_incoming_inbox').text('(' + json.recordsTotal + ') ');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_inbox").click(function (e) {
            $(".cls_badge_warning_so_change").hide();
            table_incoming_inbox.ajax.reload();
        });

        $(".cls_page_dashboard .cls_btn_refresh_inbox").click(function (e) {
            refresh_SO_update();
        });

        $(table_incoming_inbox.table().container()).on('keyup', 'input', function () {
            table_incoming_inbox
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_incoming_inbox_filter").hide();
        $("#table_incoming_inbox_wrapper .dt-buttons").hide();

        table_incoming_inbox.on('order.dt search.dt', function () {
            table_incoming_inbox.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_incoming_inbox']").on('shown.bs.tab', function (e) {
            table_incoming_inbox.columns.adjust().draw();
            //if (table_incoming_inbox == null)
            //    table_incoming_inbox.columns.adjust().draw();
            //else
            //    table_incoming_inbox.ajax.reload();
        });

        table_incoming_inbox.search('').columns().search('').draw();

        table_incoming_inbox.columns('.cls_wf_status').visible(f_vis_col_wf_status);   //#TSK-1511 #SR-70695 by aof in 09/2022

        //approve all of gm qa and customer
        if (!(getroleuser("GM_QC") || getroleuser("MK_GM"))) {
            $('.cls_btn_gmmk_approveselected').parent().hide();
            table_incoming_inbox.columns('.select-checkbox').visible(false);
            //table_incoming_inbox.columns('.cls_request_no').visible(false);
            table_incoming_inbox.select.style('api');
        } else {
            if (getroleuser("GM_QC")) {
                $('.cls_btn_gmmk_approveselected').parent().show();
                table_incoming_inbox.columns('.select-checkbox').visible(true);
                //table_incoming_inbox.columns('.cls_request_no').visible(true);
                table_incoming_inbox.select.style('multi');
                $('.cls_btn_gmqc_approveselected').show();
                $('.cls_btn_gmqc_not_approveselected').show();
            }
            if (getroleuser("MK_GM")) {
                $('.cls_btn_gmmk_approveselected').parent().show();
                table_incoming_inbox.columns('.select-checkbox').visible(true);
                //table_incoming_inbox.columns('.cls_request_no').visible(true);
                table_incoming_inbox.select.style('multi');
                $('.cls_btn_gmmk_approveselected').show();
                $('.cls_btn_gmmk_not_approveselected').show();
            }
        }

        $("#view_incoming_inbox .cls_btn_excel_inbox").click(function () {
            $("#view_incoming_inbox .buttons-excel").click();
        });
    }
});

function approveForm_byGM(data, roletype, action_code) {
    if (data.length > 0) {
        var req_form_no = "";
        var current_step_id = "";
        var sold_to = "";
        var ship_to = "";
        var isvalid = true;
        var dataSendTo = [];
        for (i = 0; i < data.length; i++) {
            var item = {};
            if (i == 0) {
                req_form_no = data[i].REQUEST_FORM_NO;
                current_step_id = data[i].CURRENT_STEP_ID;
                sold_to = data[i].SOLD_TO_DISPLAY_TXT;
                ship_to = data[i].SHIP_TO_DISPLAY_TXT;
            }
            else if (current_step_id != data[i].CURRENT_STEP_ID) {
                alertError2('Cannot select different step of items.');
                isvalid = false;
                return false;
            }
            //else if (req_form_no != data[i].REQUEST_FORM_NO) {
            //    alertError2('Cannot select different request form of items.');
            //    isvalid = false;
            //    return false;
            //}
            else if (sold_to != data[i].SOLD_TO_DISPLAY_TXT || ship_to != data[i].SHIP_TO_DISPLAY_TXT) {
                alertError2('Cannot select different sold to and ship to of items.');
                isvalid = false;
                return false;
            }

            item["ARTWORK_REQUEST_ID"] = data[i].CHECK_LIST_ID;
            item["ARTWORK_ITEM_ID"] = data[i].MOCKUP_ID;
            item["ARTWORK_SUB_ID"] = data[i].MOCKUP_SUB_ID;
            item["ACTION_CODE"] = action_code;
            item["UPDATE_BY"] = UserID;
            item["ENDTASKFORM"] = true;
            dataSendTo.push(item);
        }
        if (isvalid) {
            var jsonObj = new Object();
            jsonObj.data = dataSendTo;

            var myurl = '/api/taskform/qc/sendtomkbygmqcmulti';
            if (roletype == 'MK') {
                myurl = '/api/taskform/mk/sendtopabygmmkmulti';
            }

            var mytype = 'POST';
            var mydata = jsonObj;

            myAjaxConfirmSubmit(myurl, mytype, mydata, callback_approveForm_byGM);
        }
    }
    else {
        alertError2('Please select at least 1 item.');
    }
}

function callback_approveForm_byGM(res) {
    if (res.status == "E" && res.msg != '') {
        alertError(res.msg);
    }
    else if (res.status == "S") {
        tohomepage();
    }
}

function refresh_SO_update() {
    var table = $('#table_incoming_inbox').DataTable();
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
                $(".cls_badge_warning_so_change").text(res.data.length).show();
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

function get_user_role() {
    $('.cls_btn_refresh_inbox').hide();
    $('.cls_btn_refresh_inbox_sent_pp').hide();
    $.ajax({
        url: suburl + '/api/master/userrole',
        type: 'GET',
        data: null,
        success: function (res) {
            res = DData(res);
            $.each(res.data, function (index, value) {
                if (value.ROLE_CODE.search("PA") != -1) {
                    $('.cls_btn_refresh_inbox').show();
                    $('.cls_btn_refresh_inbox_sent_pp').show();
                }
            });
        }
    });
}