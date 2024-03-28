var first_load = true;
$(document).ready(function () {
    //bind_lov_no_ajax('.cls_report_endtoend .cls_lov_search_workflow_type', '', '');
    bind_lov('.cls_report_endtoend .cls_lov_search_sold_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_company', '/api/lov/company', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_ship_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_brand', '/api/lov/brand', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_zone', '/api/lov/zone', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_country', '/api/lov/country', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_packaging_type', '/api/lov/packtype', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_current_step', '/api/common/stepmockupandpartwork', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_creator', '/api/lov/user', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_supervised_by', '/api/lov/user', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_endtoend .cls_lov_search_current_assign', '/api/lov/user', 'data.DISPLAY_TXT');

    bind_table_report_endtoend();
    first_load = false;

    $(table_report_endtoend.table().container()).on('keyup', 'input', function () {
        table_report_endtoend
            .column($(this).data('index'))
            .search(this.value)
            .draw(false);
    });

    $(".cls_report_endtoend form").submit(function (e) {
        if ($('.cls_report_endtoend .cls_lov_search_workflow_type').val() == '') 
            alertError2('Please select Workflow type.');
        else if ($('.cls_report_endtoend .cls_dt_search_request_date_from').val() == '' || $('.cls_report_endtoend .cls_dt_search_request_date_to').val() == '')
            alertError2('Please select Request date.');
         else {
            table_report_endtoend.ajax.reload();
            e.preventDefault();
        }
    });

    $(".cls_report_endtoend .cls_btn_clear").click(function () {
        $('.cls_report_endtoend input[type=text]').val('');
        $('.cls_report_endtoend input[type=checkbox]').prop('checked', false);
        $('.cls_report_endtoend textarea').val('');
        $(".cls_report_endtoend .cls_lov_search_workflow_type").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_sold_to").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_company").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_ship_to").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_brand").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_zone").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_country").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_packaging_type").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_current_step").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_creator").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_supervised_by").val('').trigger("change");
        $(".cls_report_endtoend .cls_lov_search_current_assign").val('').trigger("change");

        $('.cls_report_endtoend .cls_dt_search_request_date_from').val(GetFirstDateOfMonth());
        $('.cls_report_endtoend .cls_dt_search_request_date_to').val(GetLastDateOfMonth());
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box

    $(".cls_report_endtoend .cls_row_advance_search").hide();
    $(".cls_report_endtoend .cls_chk_advance_search").change(function () {
        if (this.checked) {
            $(".cls_report_endtoend .cls_row_advance_search").show();
        }
        else {
            $(".cls_report_endtoend .cls_row_advance_search").hide();
        }
    });

    $(".cls_report_endtoend .cls_btn_export_excel").click(function () {
        //$(".cls_report_endtoend .buttons-excel").click();
        var param = getParamReportEndToEnd_Excel();
        window.open(suburl + "/excel/endtoendnewreport" + param, '_blank');
    });

    $('#modal_report_endtoend_att').on('shown.bs.modal', function (e) {
        $('.cls_endtoend_att_workflow_no').val($(e.relatedTarget).data('workflow_number'));
        bindEndToEndReport_File($('.cls_endtoend_att_workflow_no').val());
    });

    $('#modal_report_endtoend_att').on('hidden.bs.modal', function (e) {
        $('.cls_endtoend_att_workflow_no').val('');
    });

    $(".cls_report_endtoend .cls_btn_download_file").click(function () {
        if (table_report_endtoend == undefined) {
            alertError2('Please select at least 1 item.');
        } else {
            var data = table_report_endtoend.rows({ selected: true }).data();
            if (data.length > 0) {
                var dataWF = [];
                var aw_list = "";
                for (i = 0; i < data.length; i++) {
                    if (dataWF.indexOf(data[i].WF_NO) == -1) {
                        dataWF.push(data[i].WF_NO);
                        if (aw_list.length > 0) {
                            aw_list = aw_list + "||" + data[i].WF_NO;
                        }
                        else {
                            aw_list = data[i].WF_NO;
                        }
                    }
                }

                var url = suburl + '/FileUpload/DownloadAWZip?aw_list=' + aw_list;
                window.open(url, '_blank');
            } else {
                alertError2('Please select at least 1 item.');
            }
        }
    });
});

function callback_downloadEndtoEndFile(res) {
    if (res.status == "E" && res.msg != '') {
        alertError(res.msg);
    }
}

var table_report_endtoend;
function bind_table_report_endtoend() {
    var param = getParamReportEndToEnd();
    var groupColumn = 3;
    table_report_endtoend = $('#table_report_endtoend').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        //fixedColumns: {
        //    leftColumns: 3
        //},
        select: {
            'style': 'multi',
            selector: 'td:first-child'
        },
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
                url: suburl + "/api/report/endtoendreport_new" + "?data.first_load=" + first_load + getParamReportEndToEnd(),
                type: 'GET',
                data: data,
                success: function (res) {
                    groupColumn = DData(res).ORDER_COLUMN;
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [{
            "searchable": false,
            "orderable": false,
            "targets": 0
        }],
        "order": [[3, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "scrollY": "450px",
        "scrollCollapse": true,
        //dom: 'Bfrtip',
        //buttons: [
        //    {
        //        title: 'Report EndToEnd',
        //        extend: 'excelHtml5',
        //        exportOptions: {
        //            format: {
        //                body: function (data, row, column, node) {
        //                    //if (column == 4) {
        //                    //    return myDateMoment(data);
        //                    //} else
        //                    if (column == 2) {
        //                        return removeHtmlToComma(data);
        //                    }
        //                    //else if (column == 11) {
        //                    //    return myDateTimeMoment(data);
        //                    //}
        //                    else if (column == 12) {
        //                        return removeHtml(data);
        //                    } else {
        //                        return data;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //],
        "columns": [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            {
                className: 'cls_hide',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                render: function (data, type, row, meta) {
                    return '<a data-toggle="modal" data-workflow_number="' + row.WF_NO + '" title="Add this item" href="#modal_report_endtoend_att"><img src="' + suburl + '/Content/Free-file-icons/16px/_blank.png"></a>';
                }
            },
            { "data": "WF_NO", "className": "cls_nowrap cls_wf_no" },//WORKFLOW_NUMBER
            { "data": "PACKAGING_TYPE", "className": "cls_nowrap" },//PACKING_TYPE_DISPLAY_TXT
            { "data": "WF_STAUTS", "className": "cls_nowrap" },//CURRENT_STEP_DISPLAY_TXT
            { "data": "ORDER_BOM_COMPONENT", "className": "cls_nowrap" },//SALES_ORDER_ITEM_COMPONENT
            { "data": "CURRENT_STEP_NAME_1", "className": "cls_current_step_1 cls_nowrap" },//CURRENT_STATUS_DISPLAY_TXT
            { "data": "CURRENT_USER_NAME_1", "className": "cls_nowrap" },
            { "data": "DUE_DATE_1", "className": "cls_due_date_1 cls_nowrap" },
            { "data": "SALES_ORDER_NO", "className": "cls_nowrap" },//SALES_ORDER_NO
            { "data": "BRAND_NAME", "className": "cls_nowrap" },//BRAND_DISPLAY_TXT
            { "data": "ADDITIONAL_BRAND_NAME", "className": "cls_nowrap" },//ADDITIONAL_BRAND_DISPLAY_TXT
            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },//PRODUCT_CODE_DISPLAY_TXT
            { "data": "PROD_INSP_MEMO", "className": "cls_nowrap" },//PRODUCTION_MEMO_DISPLAY_TXT
            { "data": "REFERENCE_NO", "className": "cls_nowrap" },//RD_NUMBER_DISPLAY_TXT
            { "data": "PLANT", "className": "cls_nowrap" },//PLANT
            { "data": "SOLD_TO", "className": "cls_nowrap" },//SOLD_TO_DISPLAY_TXT
            { "data": "SHIP_TO", "className": "cls_nowrap" },//SHIP_TO_DISPLAY_TXT
            { "data": "COUNTRY", "className": "cls_nowrap" },//COUNTRY
            { "data": "PORT", "className": "cls_nowrap" },//PORT
            { "data": "IN_TRANSIT_TO", "className": "cls_nowrap" },//IN_TRANSIT_TO_DISPLAY_TXT
            { "data": "CREATE_ON", "className": "cls_nowrap" },//SALES_ORDER_CREATE_DATE
            { "data": "RDD", "className": "cls_rdd cls_nowrap" },//RDD_DISPLAY_TXT
            { "data": "REQUEST_NO", "className": "cls_request_no cls_nowrap" },//REQUEST_NUMBER /* REQUEST_FORM_NO*/
            { "data": "PA_NAME", "className": "cls_nowrap" },//PA_OWNER /* CRRUENT_ASSIGN*/
            { "data": "PG_NAME", "className": "cls_nowrap" },//PG_OWNER
            { "data": "CURRENT_STEP_NAME", "className": "cls_current_step cls_nowrap" },//ALL_STEP_DISPLAY_TXT
            { "data": "CURRENT_USER_NAME", "className": "cls_nowrap" },
            { "data": "STEP_CREATE_DATE", "className": "cls_start_date cls_nowrap " },
            { "data": "STEP_END_DATE", "className": "cls_end_date cls_nowrap" },
            { "data": "DURATION", "className": "cls_duration cls_nowrap dt-body-right" },
            { "data": "IS_STEP_DURATION_EXTEND", "className": "cls_nowrap dt-body-center" },
            { "data": "REASON", "className": "cls_td_width_350" },
            { "data": "TOTALDAY", "className": "cls_total_day cls_right" },//TOTAL_DAY_DISPLAY_TXT
            { "data": "MARKETTING", "className": "cls_nowrap" },//MARKETING_NAME
            { "data": "PROJECT_NAME", "className": "cls_nowrap" },//PROJECT_NAME
            { "data": "CREATOR_NAME", "className": "cls_nowrap" },//CREATOR_DISPLAY_TXT
            { "data": "RECEIVER_REASON", "className": "cls_td_width_350" },  //CR#19743 by aof
            { "data": "RECEIVER_COMMENT", "className": "cls_td_width_350" },  //CR#19743 by aof
            { "data": "TERMINATE_REASON", "className": "cls_nowrap" },  //#INC-55439 by aof
            { "data": "TERMINATE_COMMENT", "className": "cls_nowrap" },  //#INC-55439 by aof
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var j = 1;

            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                if (last !== group) {
                    //$(rows).eq(i).before(
                    //    '<tr class="group highlight"><td colspan="39"></td></tr>'
                    //);
                    last = group;
                    j++;
                }
                else {
                    if (groupColumn == 3) {
                        $(rows).eq(i).find('td').eq(0).removeClass('select-checkbox');
                        $(rows).eq(i).find('td').eq(1).text('');
                        $(rows).eq(i).find('td').eq(2).text('');
                        $(rows).eq(i).find('td').eq(3).text('');
                        $(rows).eq(i).find('td').eq(4).text('');
                        $(rows).eq(i).find('td').eq(5).text('');
                        $(rows).eq(i).find('td').eq(6).text('');
                    }
                }
            });
        },
        "rowCallback": function (row, data, index) {
            if (data.WF_SUB_ID != 0) {
                if (data.WF_NO.indexOf('AW-') >= 0) {
                    $(row).find('td').eq(3).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.WF_NO + '</a>');
                    $(row).find('.cls_current_step_1').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_NAME_1 + '</a>');
                    $(row).find('.cls_current_step').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_NAME + '</a>');
                    $(row).find('.cls_request_no').html('<a target="_blank" href="' + suburl + '/Artwork/' + data.REQUEST_ID + '" style="text-decoration: underline;">' + data.REQUEST_NO + '</a>');
                }
                else {
                    $(row).find('td').eq(3).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.WF_NO + '</a>');
                    $(row).find('.cls_current_step_1').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_NAME_1 + '</a>');
                    $(row).find('.cls_current_step').html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.CURRENT_STEP_NAME + '</a>');
                    $(row).find('.cls_request_no').html('<a target="_blank" href="' + suburl + '/Checklist/' + data.REQUEST_ID + '" style="text-decoration: underline;">' + data.REQUEST_NO + '</a>');
                }
            }

            if (!isEmpty(data.STEP_CREATE_DATE)) {
                $(row).find('.cls_start_date').html(myDateTimeMoment(data.STEP_CREATE_DATE));
            }
            if (!isEmpty(data.STEP_END_DATE)) {
                $(row).find('.cls_end_date').html(myDateTimeMoment(data.STEP_END_DATE));
            }
            if (!isEmpty(data.DUE_DATE_1)) {
                $(row).find('.cls_due_date_1').html(myDateTimeMoment(data.DUE_DATE_1));
            }
            if (!isEmpty(data.DURATION_STANDARD)) {
                var useDay = !isEmpty(data.USEDAY) && data.USEDAY != 0 ? data.USEDAY : "0";
                $(row).find('.cls_duration').html(useDay + " [" + data.DURATION_STANDARD + "]");
            }
            if (isEmpty(data.TOTALDAY) || data.USEDAY == 0) {
                $(row).find('.cls_total_day').html("");
            }
            if (isEmpty(data.CURRENT_STEP_NAME_1)) {
                $(row).find('.cls_current_step_1').html("");
            }

            //if (data.IS_STEP_DURATION_EXTEND == "X") {
            //    $(row).find('.cls_is_extended').html("Yes");
            //}
        }
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box
}

function getParamReportEndToEnd() {
    var current_step_wf_type = '';

    if ($('.cls_report_endtoend .cls_lov_search_current_step').select2('data').length > 0) {
        current_step_wf_type = $('.cls_report_endtoend .cls_lov_search_current_step').select2('data')[0].WF_TYPE;
    }
    return "&data.workflow_no=" + $('.cls_report_endtoend .cls_txt_search_workflow_no').val()
        + "&data.workflow_type=" + $('.cls_report_endtoend .cls_lov_search_workflow_type').val()
        + "&data.workflow_no_2=" + $('.cls_report_endtoend .cls_txt_search_request_form_no').val()
        + "&data.REQUEST_DATE_FROM=" + $('.cls_report_endtoend .cls_dt_search_request_date_from').val()
        + "&data.REQUEST_DATE_TO=" + $('.cls_report_endtoend .cls_dt_search_request_date_to').val()
        + "&data.workflow_completed=" + $('.cls_report_endtoend .cls_chk_is_completed').is(":checked")
        + "&data.search_so=" + $('.cls_report_endtoend .cls_txtarea_sales_order').val().replace(/\n/g, ',')
        + "&data.search_order_bom=" + $('.cls_report_endtoend .cls_txtarea_order_bom_component').val().replace(/\n/g, ',')
        + "&data.sold_to_id=" + $('.cls_report_endtoend .cls_lov_search_sold_to').val()
        + "&data.company_id=" + $('.cls_report_endtoend .cls_lov_search_company').val()
        + "&data.ship_to_id=" + $('.cls_report_endtoend .cls_lov_search_ship_to').val()
        + "&data.brand_id=" + $('.cls_report_endtoend .cls_lov_search_brand').val()
        + "&data.zone_txt=" + $('.cls_report_endtoend .cls_lov_search_zone option:selected').text()
        + "&data.country_id=" + $('.cls_report_endtoend .cls_lov_search_country').val()
        + "&data.packaging_type_id=" + $('.cls_report_endtoend .cls_lov_search_packaging_type').val()
        + "&data.primary_size_txt=" + $('.cls_report_endtoend .cls_txt_search_primary_size').val()
        + "&data.project_name=" + $('.cls_report_endtoend .cls_txt_search_project_name').val()
        + "&data.product_code=" + $('.cls_report_endtoend .cls_txtarea_search_product_code').val().replace(/\n/g, ',')
        + "&data.rd_number=" + $('.cls_report_endtoend .cls_txt_search_rd_ref_no').val()
        + "&data.net_weight_txt=" + $('.cls_report_endtoend .cls_txt_search_net_weight').val()
        + "&data.current_step_id=" + $('.cls_report_endtoend .cls_lov_search_current_step').val()
        + "&data.current_step_wf_type=" + current_step_wf_type
        + "&data.creator_id=" + $('.cls_report_endtoend .cls_lov_search_creator').val()
        + "&data.supervised_id=" + $('.cls_report_endtoend .cls_lov_search_supervised_by').val()
        + "&data.current_assign_id=" + $('.cls_report_endtoend .cls_lov_search_current_assign').val()
        + "&data.workflow_overdue=" + $('.cls_report_endtoend .cls_chk_is_overdue').is(":checked")
        + "&data.workflow_process=" + $('.cls_report_endtoend .cls_chk_wf_process').is(":checked")
        + "&data.workflow_terminated=" + $('.cls_report_endtoend .cls_chk_wf_terminated').is(":checked")
        + "&data.ref_wf_no=" + $('.cls_report_endtoend .cls_txt_search_ref_form_no').val()
        + "&data.current_user_id=" + UserID
        + "&data.view=" + 'MK'
        + "&data.PIC=" + ''
    
        //+ "&data.TypeWorkflow=" + $('.cls_report_endtoend input[name=rdo_view_Workflow]:radio:checked').val()
        + "&data.so_create_date_from=" + $('.cls_report_endtoend .cls_dt_search_so_create_date_from').val()
        + "&data.so_create_date_to=" + $('.cls_report_endtoend .cls_dt_search_so_create_date_to').val()
        + "&data.customer_approve_from=" + $('.cls_report_endtoend .cls_txt_search_customer_approve_from').val()
        + "&data.customer_approve_to=" + $('.cls_report_endtoend .cls_txt_search_customer_approve_to').val()
        + "&data.end_to_end_from=" + $('.cls_report_endtoend .cls_txt_search_end_to_end_from').val()
        + "&data.end_to_end_to=" + $('.cls_report_endtoend .cls_txt_search_end_to_end_to').val()
        + "&data.STEP_DATE_FROM=" + $('.cls_report_endtoend .cls_dt_search_step_date_from').val()
        + "&data.STEP_DATE_TO=" + $('.cls_report_endtoend .cls_dt_search_step_date_to').val();
}

function getParamReportEndToEnd_Excel() {
    var current_step_wf_type = '';
    if ($('.cls_report_endtoend .cls_lov_search_current_step').select2('data').length > 0)
        current_step_wf_type = $('.cls_report_endtoend .cls_lov_search_current_step').select2('data')[0].WF_TYPE;

    return "?data.workflow_no=" + $('.cls_report_endtoend .cls_txt_search_workflow_no').val()
        + "&data.workflow_type=" + $('.cls_report_endtoend .cls_lov_search_workflow_type').val()
        + "&data.workflow_no_2=" + $('.cls_report_endtoend .cls_txt_search_request_form_no').val()
        + "&data.REQUEST_DATE_FROM=" + $('.cls_report_endtoend .cls_dt_search_request_date_from').val()
        + "&data.REQUEST_DATE_TO=" + $('.cls_report_endtoend .cls_dt_search_request_date_to').val()
        + "&data.workflow_completed=" + $('.cls_report_endtoend .cls_chk_is_completed').is(":checked")
        + "&data.search_so=" + $('.cls_report_endtoend .cls_txtarea_sales_order').val().replace(/\n/g, ',')
        + "&data.search_order_bom=" + $('.cls_report_endtoend .cls_txtarea_order_bom_component').val().replace(/\n/g, ',')
        + "&data.sold_to_id=" + $('.cls_report_endtoend .cls_lov_search_sold_to').val()
        + "&data.company_id=" + $('.cls_report_endtoend .cls_lov_search_company').val()
        + "&data.ship_to_id=" + $('.cls_report_endtoend .cls_lov_search_ship_to').val()
        + "&data.brand_id=" + $('.cls_report_endtoend .cls_lov_search_brand').val()
        + "&data.zone_txt=" + $('.cls_report_endtoend .cls_lov_search_zone option:selected').text()
        + "&data.country_id=" + $('.cls_report_endtoend .cls_lov_search_country').val()
        + "&data.packaging_type_id=" + $('.cls_report_endtoend .cls_lov_search_packaging_type').val()
        + "&data.primary_size_txt=" + $('.cls_report_endtoend .cls_txt_search_primary_size').val()
        + "&data.project_name=" + $('.cls_report_endtoend .cls_txt_search_project_name').val()
        + "&data.product_code=" + $('.cls_report_endtoend .cls_txtarea_search_product_code').val().replace(/\n/g, ',')
        + "&data.rd_number=" + $('.cls_report_endtoend .cls_txt_search_rd_ref_no').val()
        + "&data.net_weight_txt=" + $('.cls_report_endtoend .cls_txt_search_net_weight').val()
        + "&data.current_step_id=" + $('.cls_report_endtoend .cls_lov_search_current_step').val()
        + "&data.current_step_wf_type=" + current_step_wf_type
        + "&data.creator_id=" + $('.cls_report_endtoend .cls_lov_search_creator').val()
        + "&data.supervised_id=" + $('.cls_report_endtoend .cls_lov_search_supervised_by').val()
        + "&data.current_assign_id=" + $('.cls_report_endtoend .cls_lov_search_current_assign').val()
        + "&data.workflow_overdue=" + $('.cls_report_endtoend .cls_chk_is_overdue').is(":checked")
        + "&data.workflow_process=" + $('.cls_report_endtoend .cls_chk_wf_process').is(":checked")
        + "&data.workflow_terminated=" + $('.cls_report_endtoend .cls_chk_wf_terminated').is(":checked")
        + "&data.ref_wf_no=" + $('.cls_report_endtoend .cls_txt_search_ref_form_no').val()
        + "&data.current_user_id=" + UserID
        + "&data.view=" + 'MK'
        + "&data.PIC=" + ''
        + "&data.GENERATE_EXCEL=X"
        + "&data.so_create_date_from=" + $('.cls_report_endtoend .cls_dt_search_so_create_date_from').val()
        + "&data.so_create_date_to=" + $('.cls_report_endtoend .cls_dt_search_so_create_date_to').val()
        + "&data.customer_approve_from=" + $('.cls_report_endtoend .cls_txt_search_customer_approve_from').val()
        + "&data.customer_approve_to=" + $('.cls_report_endtoend .cls_txt_search_customer_approve_to').val()
        + "&data.end_to_end_from=" + $('.cls_report_endtoend .cls_txt_search_end_to_end_from').val()
        + "&data.end_to_end_to=" + $('.cls_report_endtoend .cls_txt_search_end_to_end_to').val()
        + "&data.step_date_from=" + $('.cls_report_endtoend .cls_dt_search_step_date_from').val()
        + "&data.step_date_to=" + $('.cls_report_endtoend .cls_dt_search_step_date_to').val();
}

function bindEndToEndReport_File(workflow_no) {
    table_report_endtoend_att = $('#table_report_endtoend_att').DataTable();
    table_report_endtoend_att.destroy();
    table_report_endtoend_att = $('#table_report_endtoend_att').DataTable({
        ajax: function (data, callback, settings) {
            var xhr = $.ajax({
                url: suburl + "/api/report/endtoendreport_file?data.WORKFLOW_NUMBER=" + workflow_no,
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
                    return getName_EndtoEndAtt(row.FILE_NAME, 'icon');
                }
            },
            { "data": "TITLE", "className": "cls_nowrap" },
            {
                render: function (data, type, row, meta) {
                    return getName_EndtoEndAtt(row.FILE_NAME, 'file_name');
                }
            },
            {
                render: function (data, type, row, meta) {
                    return getName_EndtoEndAtt(row.FILE_NAME, 'created_by');
                }
            },
            { "data": "CREATED_DATE", "className": "cls_center cls_created_date" },
            {
                render: function (data, type, row, meta) {
                    return '<a href="' + suburl + '/FileUpload/DownloadFile?nodeIdTxt=' + row.NODE_ID_TXT + '" class="cls_hand" target="_blank">Download</a>';
                }
            }
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_created_date').html(moment(data.CREATED_DATE).format('DD/MM/YYYY HH:mm:ss'));
            //$(row).find('td').eq(5).html(moment(data.CREATE_DATE).format('DD/MM/YYYY HH:mm:ss'));

            //if (data.CURRENT_STEP_CODE_DISPLAY_TXT != null) {
            //    if (data.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_RD') {
            //        //SIT Issue log : หน้า QC เวลากดส่งไป RD ไม่ควรส่งกลับ PA ได้ ถ้า RD ยังไม่ส่งกลับมา
            //        if (data.IS_END == null) {
            //            $('#qc_submit_modal').find('.cls_chk_send_pa_qc').attr('disabled', true);
            //        }
            //        else {
            //            $('#qc_submit_modal').find('.cls_chk_send_pa_qc').attr('disabled', false);
            //        }
            //    }
            //}
        },
        "initComplete": function (settings, json) {

        }
    });
}

function getName_EndtoEndAtt(file_name, type) {
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
