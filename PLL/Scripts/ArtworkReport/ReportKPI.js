var first_load = true;
var vendor_selected = false;
$(document).ready(function () {

    //bind_lov('.cls_report_kpi .cls_lov_kpi_employee', '/api/lov/user', 'data.DISPLAY_TXT');
    bind_lov_param('.cls_report_kpi .cls_lov_kpi_employee', '/api/lov/userwithparam', 'data.DISPLAY_TXT', ["ROLE_CODE"], ['.cls_role_code_']);

    $(".cls_report_kpi .cls_btn_clr").click(function () {
        $('.cls_div_body_search_criteria input[type=text]').val('');
        $('.cls_div_body_search_criteria textarea').val('');
        $('.cls_div_body_search_criteria input[type=checkbox]').prop('checked', false);
        $('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]').filter('[value="reject"]').prop("checked", true);
        $(".cls_report_kpi .cls_lov_kpi_employee").val('').trigger("change");

        $('.cls_report_kpi .cls_kpi_from').val(GetFirstDateOfMonth());
        $('.cls_report_kpi .cls_kpi_to').val(GetLastDateOfMonth());
    });

    $(".cls_report_kpi .cls_chk_kpi_all_dep").click(function () {
        if ($('.cls_chk_kpi_all_dep').is(':checked')) {
            $(".cls_report_kpi .cls_lov_kpi_employee").prop('disabled', true);
            //$(".cls_report_kpi .cls_btn_data_kpi_export_excel").show();
        }
        else {
            $(".cls_report_kpi .cls_lov_kpi_employee").prop('disabled', false);
            //$(".cls_report_kpi .cls_btn_data_kpi_export_excel").hide();
        }
    });

    $('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]').change(function () {
        var q = $('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val();
        $(".cls_report_kpi .cls_lov_kpi_employee").prop('disabled', false);
        $(".cls_report_kpi .cls_chk_kpi_all_dep").prop('disabled', false);
        $(".cls_report_kpi .cls_name_emp").show();
        $(".cls_report_kpi .cls_grading").show();
        if (q == "reject") {
            $(".cls_report_kpi .cls_role_code_").val('PG_');
            $(".cls_report_kpi .cls_header_grade1").html('Grade 1 >');
            $(".cls_report_kpi .cls_grade1").val('9.49');
            $(".cls_report_kpi .cls_header_grade2").html('Grade 2 <=');
            $(".cls_report_kpi .cls_grade2").val('9.49');
            $(".cls_report_kpi .cls_header_grade3").html('Grade 3 <=');
            $(".cls_report_kpi .cls_grade3").val('6.49');
            $(".cls_report_kpi .cls_header_grade4").html('Grade 4 <=');
            $(".cls_report_kpi .cls_grade4").val('3.5');
            $(".cls_report_kpi .cls_header_grade5").html('Grade 5 <=');
            $(".cls_report_kpi .cls_grade5").val('0.5');
        }
        else if (q == "mockstand") {
            $(".cls_report_kpi .cls_role_code_").val('PG_');
            $(".cls_report_kpi .cls_header_grade1").html('Grade 1 <');
            $(".cls_report_kpi .cls_grade1").val('66.00');
            $(".cls_report_kpi .cls_header_grade2").html('Grade 2 >=');
            $(".cls_report_kpi .cls_grade2").val('66.00');
            $(".cls_report_kpi .cls_header_grade3").html('Grade 3 >=');
            $(".cls_report_kpi .cls_grade3").val('75.00');
            $(".cls_report_kpi .cls_header_grade4").html('Grade 4 >=');
            $(".cls_report_kpi .cls_grade4").val('84.00');
            $(".cls_report_kpi .cls_header_grade5").html('Grade 5 >=');
            $(".cls_report_kpi .cls_grade5").val('93.00');
        }
        else if (q == "sendquostand") {
            $(".cls_report_kpi .cls_role_code_").val('PG_');
            $(".cls_report_kpi .cls_header_grade1").html('Grade 1 <');
            $(".cls_report_kpi .cls_grade1").val('66.00');
            $(".cls_report_kpi .cls_header_grade2").html('Grade 2 >=');
            $(".cls_report_kpi .cls_grade2").val('66.00');
            $(".cls_report_kpi .cls_header_grade3").html('Grade 3 >=');
            $(".cls_report_kpi .cls_grade3").val('75.00');
            $(".cls_report_kpi .cls_header_grade4").html('Grade 4 >=');
            $(".cls_report_kpi .cls_grade4").val('84.00');
            $(".cls_report_kpi .cls_header_grade5").html('Grade 5 >=');
            $(".cls_report_kpi .cls_grade5").val('93.00');
        }
        else if (q == "artworkstand") {
            $(".cls_report_kpi .cls_role_code_").val('PA_');
            $(".cls_report_kpi .cls_header_grade1").html('Grade 1 <');
            $(".cls_report_kpi .cls_grade1").val('90.00');
            $(".cls_report_kpi .cls_header_grade2").html('Grade 2 >=');
            $(".cls_report_kpi .cls_grade2").val('90.00');
            $(".cls_report_kpi .cls_header_grade3").html('Grade 3 >=');
            $(".cls_report_kpi .cls_grade3").val('93.00');
            $(".cls_report_kpi .cls_header_grade4").html('Grade 4 >=');
            $(".cls_report_kpi .cls_grade4").val('96.00');
            $(".cls_report_kpi .cls_header_grade5").html('Grade 5 >=');
            $(".cls_report_kpi .cls_grade5").val('99.00');
        }
        else if (q == "postand") {
            $(".cls_report_kpi .cls_role_code_").val('PP_');
            $(".cls_report_kpi .cls_header_grade1").html('Grade 1 <');
            $(".cls_report_kpi .cls_grade1").val('79.00');
            $(".cls_report_kpi .cls_header_grade2").html('Grade 2 >=');
            $(".cls_report_kpi .cls_grade2").val('79.00');
            $(".cls_report_kpi .cls_header_grade3").html('Grade 3 >=');
            $(".cls_report_kpi .cls_grade3").val('83.00');
            $(".cls_report_kpi .cls_header_grade4").html('Grade 4 >=');
            $(".cls_report_kpi .cls_grade4").val('86.99');
            $(".cls_report_kpi .cls_header_grade5").html('Grade 5 >=');
            $(".cls_report_kpi .cls_grade5").val('90.99');
        }
        else {
            $(".cls_report_kpi .cls_role_code_").val('');
            $(".cls_report_kpi .cls_lov_kpi_employee").prop('disabled', true);
            $(".cls_report_kpi .cls_chk_kpi_all_dep").prop('disabled', true);
            $(".cls_report_kpi .cls_name_emp").hide();
            $(".cls_report_kpi .cls_grading").hide();
        }
        $(".cls_report_kpi .cls_lov_kpi_employee").val('').trigger("change");
        $(".cls_report_kpi .cls_chk_kpi_all_dep").prop('checked', false);
    });

    $(".cls_report_kpi form").submit(function (e) {
        if ($('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val() == "saving") {
            $(".cls_report_kpi .cls_table_report_kpi").hide();
            $(".cls_report_kpi .cls_for_kpi_table").show();
            bind_kpi_price_compare();
            $('.cls_report_kpi .cls_kpi_saving_count').show();
        }
        else {
            $(".cls_report_kpi .cls_table_report_kpi").show();
            $(".cls_report_kpi .cls_for_kpi_table").hide();
            table_report_kpi.ajax.reload();
        }
        e.preventDefault();
    });

    $(".cls_report_kpi .cls_btn_kpi_export_excel").click(function () {
        if ($('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val() != "saving")
            window.open("/excel/kpireport?data.DATE_FROM=" + $('.cls_kpi_from').val()
                + "&data.DATE_TO=" + $('.cls_kpi_to').val()
                + "&data.KPI_TYPE=" + $('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val()
                + "&data.userid=" + ($('.cls_chk_kpi_all_dep').is(':checked') ? -1 : $('.cls_lov_kpi_employee').val())
                + "&data.score1=" + $('.cls_report_kpi .cls_grade1').val()
                + "&data.score2=" + $('.cls_report_kpi .cls_grade2').val()
                + "&data.score3=" + $('.cls_report_kpi .cls_grade3').val()
                + "&data.score4=" + $('.cls_report_kpi .cls_grade4').val()
                + "&data.score5=" + $('.cls_report_kpi .cls_grade5').val()
                + "&data.GENERATE_EXCEL=X"
                , '_blank');
        else
            window.open("/excel/kpipricecomparereport?data.DATE_FROM=" + $('.cls_kpi_from').val()
                + "&data.DATE_TO=" + $('.cls_kpi_to').val()
                , '_blank');

    });

    $(".cls_report_kpi .cls_btn_data_kpi_export_excel").click(function () {
        if ($('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val() != "saving")
            window.open("/excel/DataKPIReport?data.DATE_FROM=" + $('.cls_kpi_from').val()
                + "&data.DATE_TO=" + $('.cls_kpi_to').val()
                + "&data.KPI_TYPE=" + $('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val()
                + "&data.userid=" + ($('.cls_chk_kpi_all_dep').is(':checked') ? -1 : $('.cls_lov_kpi_employee').val())
                + "&data.score1=" + $('.cls_report_kpi .cls_grade1').val()
                + "&data.score2=" + $('.cls_report_kpi .cls_grade2').val()
                + "&data.score3=" + $('.cls_report_kpi .cls_grade3').val()
                + "&data.score4=" + $('.cls_report_kpi .cls_grade4').val()
                + "&data.score5=" + $('.cls_report_kpi .cls_grade5').val()
                + "&data.GENERATE_EXCEL=X"
                , '_blank');

    });

    //generate_header_filter();
    $(".cls_report_kpi .cls_lov_kpi_employee").prop('required', true);

    bind_kpi_price_compare();
    bind_table_report_kpi();

    first_load = false;
});
$(function () {
    $('.month-picker-from').datepicker({
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'dd/mm/yy',
        onClose: function (dateText, inst) {
            $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
        }
    })
});
$(function () {
    $('.month-picker-to').datepicker({
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'dd/mm/yy',
        onClose: function (dateText, inst) {
            $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth + 1, 0));
        }
    })
});

function generate_header_filter() {
    //clear table
    table_report_kpi = $('#table_report_kpi').DataTable();

    //kpi
    $('.cls_table_report_kpi thead tr:eq(2)').after('<tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th></tr>');
    $('.cls_table_report_kpi thead tr:eq(3) th').each(function (i) {
        if (i == 0) {
            $(this).html('');
        }
        else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });
}

function bind_kpi_price_compare() {
    var myurl = '/api/report/kpisavingreport?data.first_load=' + first_load + '&data.DATE_FROM=' + $('.cls_kpi_from').val() + '&data.DATE_TO=' + $('.cls_kpi_to').val();
    //var myurl = '/api/report/kpisavingreport?data.DATE_FROM=01/11/2018&data.DATE_TO=10/11/2018' + "&data.first_load=" + first_load;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_kpi_price_compare);
}
function callback_bind_kpi_price_compare(res) {
    $('.cls_for_kpi_table').remove();
    $('.cls_head_for_kpi').append('<div class="cls_for_kpi_table"></div>');
    var vendor_selected_id = 0;
    var temp = res.data.length;
    var temp1 = 0;

    if (first_load) {
        temp = 1;
    }
    else if (temp == 0 || temp == 1) {
        $('.cls_label_total_wf').html('Total : ' + temp + ' Workflow');
    }

    for (var e = 0; e < temp; e++) {
        if (res.data[e].WF_NO != "NO SELECTED") {
            temp1++;
            vendor_selected = false;
            vendor_selected_id = 0;
            vendor_round_selected_id = 0;
            pg_sup_manual = false;
            vendor_manual_id = 0;
            vendor_round_manual_id = 0;
            $.each(res.data[e].ALL_PRICE, function (index, item) {
                if (item.SELECTED == "X") {
                    vendor_selected = true;
                    vendor_selected_id = item.VENDOR_ID;
                    vendor_round_selected_id = item.ROUND;
                }
                if (item.IS_MANUAL == "X") {
                    pg_sup_manual = true;
                    vendor_manual_id = item.VENDOR_ID;
                    vendor_round_manual_id = item.ROUND;
                }
            });


            $('.cls_for_kpi_table').append('<table class="cls_table_kpi_price_compare' + e + '"><thead></thead><tbody></tbody></table>');

            var MOCKUP_SUB_ID = '';
            var row_html = '';
            cnt_td = 1;
            row_html = '<tr>';
            row_html += '<th>WF NO</th>';
            row_html += '<th>Final info</th>';
            row_html += '<th>Scale</td>';
            $.each(res.data[e].ALL_PRICE, function (index, item) {
                if (MOCKUP_SUB_ID.indexOf(item.MOCKUP_SUB_ID) == -1) {
                    row_html += '<th class="cls_th_vendor_name"><span style="display:none;" class="cls_round">' + item.ROUND + '</span>'
                        + '<span style="display:none;" class="cls_vendor_id">' + item.VENDOR_ID + '</span>'
                        + item.VENDOR_DISPLAY_TXT + " (" + item.ROUND
                        + ")</th>";
                    MOCKUP_SUB_ID += item.MOCKUP_SUB_ID + '-';
                    cnt_td++;
                }

            });
            row_html += '</tr>';
            $(row_html).appendTo('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' thead');
            var count = 0;
            $.each(res.data[e].ALL_PRICE, function (index, item) {
                var found = false;
                $(".cls_report_kpi .cls_table_kpi_price_compare" + e + " tbody .cls_scale").each(function () {
                    if ($(this).text() == setCurrency0(item.SCALE)) { found = true };
                });

                if (!found) {
                    var row_html = '<tr>';
                    if (count == 0) {
                        row_html += '<td class="cls_wf_no_">' + item.WF_NO + '</td>';
                        row_html += '<td class="cls_final_info_">' + item.FINAL_INFO + '</td>';
                    }
                    else {
                        row_html += '<td></td>';
                        row_html += '<td></td>';
                    }
                    row_html += '<td class="cls_scale">' + setCurrency0(item.SCALE) + '</td>';
                    var i = 1;
                    while (i < cnt_td) {
                        row_html += '<td></td>';
                        i++;
                    }
                    row_html += '</tr>';
                    $(row_html).appendTo('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' tbody');
                }
                count++;

            });

            var i = 0;
            $('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' thead th').each(function () {
                if (i > 2) {
                    var vendor_id = $(this).find('.cls_vendor_id').text();
                    var round = $(this).find('.cls_round').text();
                    $('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' tbody tr').each(function () {
                        $(this).find('td').eq(i).text(vendor_id + '-' + round);
                    });
                }
                i++;
            });
            $('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' tbody tr').each(function () {
                var obj = $(this);
                var scale = $(this).find('.cls_scale').text();
                $(obj).find('td').not(':first').each(function () {
                    var obj_td = $(this);
                    $.each(res.data[e].ALL_PRICE, function (index, item) {
                        if (setCurrency0(item.SCALE) == scale && item.VENDOR_ID + '-' + item.ROUND == $(obj_td).text()) {
                            $(obj_td).text('');
                            $(obj_td).append("<input data-mockup-sub-id='" + item.MOCKUP_SUB_ID + "' class='form-control cls_price_manual'/>");
                            $(obj_td).find('input').autoNumeric('init', { mDec: '4' });
                            $(obj_td).find('input').autoNumeric('set', item.PRICE);
                            $(obj_td).find('input').attr("disabled", true);
                        }

                    });
                });
            });
            $('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' tbody tr').each(function () {
                var obj = $(this);
                var scale = $(this).find('.cls_scale').text();
                $(obj).find('td').not(':first').each(function () {
                    var obj_td = $(this);
                    $.each(res.data[e].ALL_PRICE, function (index, item) {
                        if (item.VENDOR_ID + '-' + item.ROUND == $(obj_td).text()) {
                            $(obj_td).text('');
                            $(obj_td).append("<input data-mockup-sub-id='" + item.MOCKUP_SUB_ID + "' class='form-control cls_price_manual'/>");
                            $(obj_td).find('input').autoNumeric('init', { mDec: '4' });
                            $(obj_td).find('input').autoNumeric('set', 0);
                            $(obj_td).find('input').attr("disabled", true);
                        }

                    });
                });
            });

            var vals;
            $('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' tbody tr').each(function () {
                var vals = $('td,th', this).map(function () {
                    return parseFloat(removeComma($(this).find('input').val()), 10) ? parseFloat(removeComma($(this).find('input').val()), 10) : null;
                }).get();
                // then find their minimum
                var min = Math.min.apply(Math, vals);

                // tag any cell matching the min value
                $('td,th', this).not(':first').filter(function () {
                    return parseFloat(removeComma($(this).find('input').val()), 10) === min;
                }).css('background-color', '#80ffaa').find('input').css('background-color', '#80ffaa').css('border-color', '#80ffaa');
            });

            $('.cls_report_kpi .cls_table_kpi_price_compare' + e).DataTable({
                columnDefs: [
                    { "orderable": true, type: 'sort-numbers-ignore-text', targets: 0 },
                    { "orderable": false, targets: '_all' },
                ],
                "info": false,
                "searching": false,
                "paging": false,
            });

            var cntTh = $('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' thead th').length;
            $('.cls_report_kpi .cls_table_kpi_price_compare' + e + ' thead th').each(function () {
                if ($(this).text() == "WF NO") {
                    $(this).addClass("col-md-2");
                    $(this).removeAttr("style");
                }
                else if ($(this).text() == "Scale") {
                    $(this).addClass("col-md-1");
                    $(this).removeAttr("style");
                }
                else if ($(this).text() == "Final info") {
                    $(this).addClass("col-md-1");
                    $(this).removeAttr("style");
                }
                else
                    $(this).css('width', 100 / (cntTh - 3));
            });

            if (vendor_selected) {
                $('.cls_table_kpi_price_compare' + e + ' thead th ').each(function (index) {
                    if ($(this).find('.cls_vendor_id').text() == vendor_selected_id && $(this).find('.cls_round').text() == vendor_round_selected_id) {
                        $(this).closest('th').append('<img title="Selected" class="cls_sleected_vendor_img" style="width:15px;" src="/Content/img/ico_true.png">');
                    }
                });

                $('.cls_table_kpi_price_compare' + e + ' .cls_sleected_vendor_img:not(:last)').each(function (index) {
                    $(this).remove();
                });

                $('.cls_report_kpi .cls_btn_request_quotation_price_template').hide();
                $('.cls_report_kpi .cls_btn_reset_vendor_price_template').hide();
                $('.cls_report_kpi .cls_row_select_vendor').hide();
            }
            if (pg_sup_manual) {
                $('.cls_table_kpi_price_compare' + e + ' thead th ').each(function (index) {
                    if ($(this).find('.cls_vendor_id').text() == vendor_manual_id && $(this).find('.cls_round').text() == vendor_round_manual_id) {
                        $(this).closest('th').append(' <label style="font-weight: lighter;">[ Final price ]</label> ');
                    }
                });
            }
        }
    }
    $('.cls_label_total_wf').html('Total : ' + temp1 + ' Workflows');
}


var table_report_kpi;
function bind_table_report_kpi() {

    //bind data    
    table_report_kpi = $('#table_report_kpi').DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/kpireport?data.DATE_FROM=" + $('.cls_kpi_from').val()
                    + "&data.DATE_TO=" + $('.cls_kpi_to').val()
                    + "&data.KPI_TYPE=" + $('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val()
                    + "&data.userid=" + ($('.cls_chk_kpi_all_dep').is(':checked') ? -1 : $('.cls_lov_kpi_employee').val())
                    + "&data.score1=" + $('.cls_report_kpi .cls_grade1').val()
                    + "&data.score2=" + $('.cls_report_kpi .cls_grade2').val()
                    + "&data.score3=" + $('.cls_report_kpi .cls_grade3').val()
                    + "&data.score4=" + $('.cls_report_kpi .cls_grade4').val()
                    + "&data.score5=" + $('.cls_report_kpi .cls_grade5').val()
                    + "&data.first_load=" + first_load,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "searchable": false, "orderable": false, "targets": [0], "className": 'dt-body-center' }
        ],
        "order": [[0, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "scrollY": "450px",
        "scrollCollapse": true,
        "paging": false,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "EMPLOYEE_ID_DISPLAY_TEXT", "className": "cls_nowrap" },
            { "data": "EMPLOYEE_NAME_DISPLAY_TEXT", "className": "cls_nowrap" },
            { "data": "POSITION_DISPLAY_TEXT", "className": "cls_nowrap" },
            { "data": "Month1", "className": "cls_nowrap dt-body-right" },
            { "data": "Month2", "className": "cls_nowrap dt-body-right" },
            { "data": "Month3", "className": "cls_nowrap dt-body-right" },
            { "data": "Month4", "className": "cls_nowrap dt-body-right" },
            { "data": "Month5", "className": "cls_nowrap dt-body-right" },
            { "data": "Month6", "className": "cls_nowrap dt-body-right" },
            { "data": "Month7", "className": "cls_nowrap dt-body-right" },
            { "data": "Month8", "className": "cls_nowrap dt-body-right" },
            { "data": "Month9", "className": "cls_nowrap dt-body-right" },
            { "data": "Month10", "className": "cls_nowrap dt-body-right" },
            { "data": "Month11", "className": "cls_nowrap dt-body-right" },
            { "data": "Month12", "className": "cls_nowrap dt-body-right" },
            { "data": "AVG1", "className": "cls_nowrap dt-body-right" },
            { "data": "GRADE1", "className": "cls_nowrap dt-body-right" },
            { "data": "AVG2", "className": "cls_nowrap dt-body-right" },
            { "data": "GRADE2", "className": "cls_nowrap dt-body-right" },
        ],
        "rowCallback": function (row, data, index) {
            if (data.AVG1 != null && parseInt(data.AVG1) < data.TARGET && parseInt(data.GRADE1) != null)
                $(row).find('td').eq(17).html('<span style=\"color: red;\">' + data.GRADE1 + '</span>');
            if (data.AVG2 != null && parseInt(data.AVG2) < data.TARGET && parseInt(data.GRADE2) != null)
                $(row).find('td').eq(19).html('<span style=\"color: red;\">' + data.GRADE2 + '</span>');
            if ($('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val() == 'reject' && data.AVG1 != null && parseFloat(data.AVG1) > (data.TARGET) && parseInt(data.GRADE1) != null)
                $(row).find('td').eq(17).html('<span style=\"color: red;\">' + data.GRADE1 + '</span>');
            if ($('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val() == 'reject' && data.AVG2 != null && parseFloat(data.AVG2) > (data.TARGET) && parseInt(data.GRADE2) != null)
                $(row).find('td').eq(19).html('<span style=\"color: red;\">' + data.GRADE2 + '</span>');
            if ($('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val() == 'reject' && data.AVG1 != null && parseFloat(data.AVG1) < parseFloat(data.TARGET) && parseInt(data.GRADE1) != null)
                $(row).find('td').eq(17).html('<span">' + data.GRADE1 + '</span>');
            if ($('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val() == 'reject' && data.AVG2 != null && parseFloat(data.AVG2) < parseFloat(data.TARGET) && parseInt(data.GRADE2) != null)
                $(row).find('td').eq(19).html('<span">' + data.GRADE2 + '</span>');

            $('.cls_table_report_kpi thead tr:eq(1) th').each(function (i) {
                if (i >= 0 && i < 12) {
                    var param = getParamReportKPI(moment(data.MONTH_FROM).add(i, 'months').format('DD/MM/YYYY'), moment(data.MONTH_FROM).add(i + 1, 'months').add(-1, 'days').format('DD/MM/YYYY'));
                    $(this).html(moment(data.MONTH_FROM).add(i, 'months').format('MMM </br> YYYY')
                        + '</br><a target="_blank" href="/' + param + '" style="text-decoration: underline;"> <img title="Export data" style="width:15px;" src="/Content/Free-file-icons/16px/dotx.png"> </a>');
                }
                else if (i == 12)
                    $(this).html(moment(data.MONTH_FROM).add(0, 'months').format('MMM YYYY') + "-" + moment(data.MONTH_FROM).add(5, 'months').format('MMM YYYY'));
                else if (i == 13)
                    $(this).html(moment(data.MONTH_FROM).add(6, 'months').format('MMM YYYY') + "-" + moment(data.MONTH_FROM).add(11, 'months').format('MMM YYYY'));
            });
        },
    });

    $(table_report_kpi.table().container()).on('keyup', 'input', function () {
        table_report_kpi
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    //hiding global search box
    $(".dataTables_filter").css("display", "none");
}

function getParamReportKPI(datefrom, dateto) {
    return "excel/DataKPIReport?data.DATE_FROM=" + datefrom
        + "&data.DATE_TO=" + dateto
        + "&data.KPI_TYPE=" + $('.cls_div_body_search_criteria input:radio[name=requesttype_kpi]:radio:checked').val()
        + "&data.userid=" + ($('.cls_chk_kpi_all_dep').is(':checked') ? -1 : $('.cls_lov_kpi_employee').val())
        + "&data.score1=" + $('.cls_report_kpi .cls_grade1').val()
        + "&data.score2=" + $('.cls_report_kpi .cls_grade2').val()
        + "&data.score3=" + $('.cls_report_kpi .cls_grade3').val()
        + "&data.score4=" + $('.cls_report_kpi .cls_grade4').val()
        + "&data.score5=" + $('.cls_report_kpi .cls_grade5').val();
}
