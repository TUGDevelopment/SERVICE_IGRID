$(document).ready(function () {
    bind_lov_no_ajax('.cls_report_endtoend .cls_lov_search_workflow_type', '', '');
    
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

    $('.cls_table_report_endtoend thead tr').clone(true).appendTo('.cls_table_report_endtoend thead');
    $('.cls_table_report_endtoend thead tr:eq(1) th').each(function (i) {
        if (i == 0) {
            $(this).html('');
        } else if (i == 25) {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" style="" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    
    $('#table_report_endtoend').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        "scrollX": true,
        "scrollY": "350px",
        "scrollCollapse": true,
    });
    //bind_table_report_endtoend();

    //$(".cls_report_endtoend .cls_btn_search").click(function () {
    //    bind_table_report_endtoend();
    //});

    $(".cls_report_endtoend form").submit(function (e) {
        bind_table_report_endtoend();
        e.preventDefault();
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

    //$(".cls_report_endtoend form").submit(function (e) {
    //    table_report_endtoend.ajax.reload();
    //    e.preventDefault();
    //});

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
        var param = getParamReportEndToEnd_Execel();
        window.open(suburl + "/excel/endtoendreport" + param, '_blank');
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
                    if (dataWF.indexOf(data[i].WORKFLOW_NUMBER) == -1) {
                        dataWF.push(data[i].WORKFLOW_NUMBER);
                        if (aw_list.length > 0) {
                            aw_list = aw_list + "||" + data[i].WORKFLOW_NUMBER;
                        }
                        else {
                            aw_list = data[i].WORKFLOW_NUMBER;
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
    table_report_endtoend = $('#table_report_endtoend').DataTable();
    table_report_endtoend.destroy();
    table_report_endtoend = $('#table_report_endtoend').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        //fixedColumns: {
        //    leftColumns: 3
        //},
        select: {
            'style': 'multi'
        },
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/endtoendreport" + param,
                type: 'GET',
                data: data,
                success: function (res) {
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
            {
                render: function (data, type, row, meta) {
                    return '<a data-toggle="modal" data-workflow_number="' + row.WORKFLOW_NUMBER + '" title="Add this item" href="#modal_report_endtoend_att"><img src="' + suburl + '/Content/Free-file-icons/16px/_blank.png"></a>';
                }},
            { "data": "WORKFLOW_NUMBER", "className": "cls_td_width_140" },
            { "data": "PACKING_TYPE_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "SALES_ORDER_ITEM_COMPONENT", "className": "cls_td_width_140" },
            { "data": "CURRENT_STATUS_DISPLAY_TXT", "className": "cls_nowrap" },

            { "data": "SALES_ORDER_NO", "className": "cls_td_width_200" },
            //{ "data": "SALES_ORDER_ITEM", "className": "cls_td_width_200" },
            { "data": "BRAND_DISPLAY_TXT", "className": "cls_td_width_140" },

            { "data": "ADDITIONAL_BRAND_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "PRODUCT_CODE_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "PRODUCTION_MEMO_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "RD_NUMBER_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "PLANT", "className": "cls_td_width_200" },
            { "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
            { "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_td_width_200" },
            { "data": "COUNTRY", "className": "cls_td_width_200" },
            { "data": "PORT", "className": "cls_td_width_200" },
            { "data": "IN_TRANSIT_TO_DISPLAY_TXT", "className": "cls_td_width_140" },
            { "data": "SALES_ORDER_CREATE_DATE", "className": "cls_td_width_140" },
            { "data": "RDD_DISPLAY_TXT", "className": "cls_rdd cls_td_width_100" },
            //{ "data": "", "className": "cls_td_width_200" },/* CRRUENT_ASSIGN*/
            { "data": "REQUEST_NUMBER", "className": "cls_td_width_200" },/* REQUEST_FORM_NO*/
            { "data": "PA_OWNER", "className": "cls_td_width_200" },/* CRRUENT_ASSIGN*/
            { "data": "PG_OWNER", "className": "cls_td_width_200" },
            { "data": "ALL_STEP_DISPLAY_TXT", "className": "cls_td_width_200" },
            { "data": "TOTAL_DAY_DISPLAY_TXT", "className": "cls_right" },
            { "data": "MARKETING_NAME", "className": "cls_td_width_140" },
            //{ "data": "", "className": "cls_td_width_140" },
            { "data": "PROJECT_NAME", "className": "cls_td_width_140" },
            { "data": "CREATOR_DISPLAY_TXT", "className": "cls_td_width_140" },
         

          

        ],
        "rowCallback": function (row, data, index) {
            if (data.MOCKUP_SUB_ID != 0) {
                if (data.WORKFLOW_NUMBER.indexOf('AW-') >= 0)
                    $(row).find('td').eq(3).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.WORKFLOW_NUMBER + '</a>');
                else
                    $(row).find('td').eq(3).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + data.WORKFLOW_NUMBER + '</a>');
            }
            if (!isEmpty(data.RDD))
                $(row).find('.cls_rdd').html(moment(data.RDD).format('DD/MM/YYYY'));

            //bindTooltip(row);
        },
    });

    $(table_report_endtoend.table().container()).on('keyup', 'input', function () {
        table_report_endtoend
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box
}

function getParamReportEndToEnd() {
    var current_step_wf_type = '';
    if ($('.cls_report_endtoend .cls_lov_search_current_step').select2('data').length > 0)
        current_step_wf_type = $('.cls_report_endtoend .cls_lov_search_current_step').select2('data')[0].WF_TYPE;
    return "?data.workflow_no=" + $('.cls_report_endtoend .cls_txt_search_workflow_no').val()
        + "&data.workflow_type=" + $('.cls_report_endtoend .cls_lov_search_workflow_type').val()
        + "&data.workflow_no_2=" + $('.cls_report_endtoend .cls_txt_search_request_form_no').val()
        + "&data.request_date_from=" + $('.cls_report_endtoend .cls_dt_search_request_date_from').val()
        + "&data.request_date_to=" + $('.cls_report_endtoend .cls_dt_search_request_date_to').val()
        + "&data.workflow_completed=" + $('.cls_report_endtoend .cls_chk_is_completed').is(":checked")

        + "&data.search_so=" + $('.cls_report_endtoend .cls_txtarea_sales_order').val().replace(/\n/g, ',')
        + "&data.search_order_bom=" + $('.cls_report_endtoend .cls_txtarea_order_bom_component').val().replace(/\n/g, ',')
        //SO Create date from
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
        //Customer approve process(Days) from:
        + "&data.supervised_id=" + $('.cls_report_endtoend .cls_lov_search_supervised_by').val()
        //End to end process (Days) from :
        + "&data.current_assign_id=" + $('.cls_report_endtoend .cls_lov_search_current_assign').val()
        //+ "&data.working_group_id=" + $('.cls_report_tracking .cls_lov_cur_working_group').val()
        //+ "&data.workflow_overdue=" + $('.cls_report_tracking .cls_chk_wf_overdue').is(":checked")
        + "&data.workflow_overdue=" + $('.cls_report_endtoend .cls_chk_is_overdue').is(":checked")
        + "&data.ref_wf_no=" + $('.cls_report_endtoend .cls_txt_search_ref_form_no').val()
        + "&data.current_user_id=" + UserID
        + "&data.view=" + 'MK'
        + "&data.PIC=" + '';
}

function getParamReportEndToEnd_Execel() {
    var current_step_wf_type = '';
    if ($('.cls_report_endtoend .cls_lov_search_current_step').select2('data').length > 0)
        current_step_wf_type = $('.cls_report_endtoend .cls_lov_search_current_step').select2('data')[0].WF_TYPE;

    return "?data.workflow_no=" + $('.cls_report_endtoend .cls_txt_search_workflow_no').val()
        + "&data.workflow_type=" + $('.cls_report_endtoend .cls_lov_search_workflow_type').val()
        + "&data.workflow_no_2=" + $('.cls_report_endtoend .cls_txt_search_request_form_no').val()
        + "&data.request_date_from=" + $('.cls_report_endtoend .cls_dt_search_request_date_from').val()
        + "&data.request_date_to=" + $('.cls_report_endtoend .cls_dt_search_request_date_to').val()
        + "&data.workflow_completed=" + $('.cls_report_endtoend .cls_chk_is_completed').is(":checked")

        + "&data.search_so=" + $('.cls_report_endtoend .cls_txtarea_sales_order').val().replace(/\n/g, ',')
        + "&data.search_order_bom=" + $('.cls_report_endtoend .cls_txtarea_order_bom_component').val().replace(/\n/g, ',')
        //SO Create date from
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
        //Customer approve process(Days) from:
        + "&data.supervised_id=" + $('.cls_report_endtoend .cls_lov_search_supervised_by').val()
        //End to end process (Days) from :
        + "&data.current_assign_id=" + $('.cls_report_endtoend .cls_lov_search_current_assign').val()
        //+ "&data.working_group_id=" + $('.cls_report_tracking .cls_lov_cur_working_group').val()
        //+ "&data.workflow_overdue=" + $('.cls_report_tracking .cls_chk_wf_overdue').is(":checked")
        + "&data.workflow_overdue=" + $('.cls_report_endtoend .cls_chk_is_overdue').is(":checked")
        + "&data.ref_wf_no=" + $('.cls_report_endtoend .cls_txt_search_ref_form_no').val()
        + "&data.current_user_id=" + UserID
        + "&data.view=" + 'MK'
        + "&data.PIC=" + ''
        + "&data.GENERATE_EXCEL=X";
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
