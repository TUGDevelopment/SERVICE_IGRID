var first_load = true;
var countheader = 0;
var unlock_remark_quill, obsolete_remark_quill
$(document).ready(function () {

    bind_lov_no_ajax('.cls_report_artworkmatcontrol .cls_lov_search_status', '', '');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_sold_to', '/api/lov/customersoldtoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_ship_to', '/api/lov/customershiptoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_brand', '/api/lov/brandso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_zone', '/api/lov/zone', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_country', '/api/lov/countryso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_pic', '/api/lov/userpicso', 'data.DISPLAY_TXT');

    unlock_remark_quill = bind_text_editor('#modal_report_artworkmatcontrol_unlock .cls_unlock_remark');
    obsolete_remark_quill = bind_text_editor('#modal_report_artworkmatcontrol_obsolete .cls_obsolete_remark');

    $('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_from').attr('required', true);
    $('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_to').attr('required', true);

    $('.cls_table_report_artworkmatcontrol thead tr').clone(true).appendTo('.cls_table_report_artworkmatcontrol thead');
    $('.cls_table_report_artworkmatcontrol thead tr:eq(1) th').each(function (i) {
        //if (i == 2 || i == 11 || i == 12) {
        if (i == 2) {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
        else
            $(this).html('');
    });

    bind_table_report_artworkmatcontrol();
    first_load = false;

    $(table_report_artworkmatcontrol.table().container()).on('keyup', 'input', function () {
        table_report_artworkmatcontrol
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    $(document).on('change', '.cls_report_artworkmatcontrol .cls_chk_group', function (e) {
        // Get group class name
        var groupName = $(this).data('group-name');
        var tempCheck = this.checked;
        var temp = table_report_artworkmatcontrol.rows({ search: 'applied' });
        temp.rows('tr.' + groupName).every(function (rowIdx, tableLoop, rowLoop) {
            var rowNode = this.node();
            table_report_artworkmatcontrol.row(rowIdx).select(tempCheck);
            $(rowNode).find('.cls_chk_mat5').prop('checked', tempCheck);
        });
        if (tempCheck) {
            $(".cls_head_chk_group_" + groupName).addClass("selected");
        }
        else
            $(".cls_head_chk_group_" + groupName).removeClass("selected");
    });

    $('body').on('change', '#mat_select_all', function () {
        if (this.checked)
            $('.cls_report_artworkmatcontrol .cls_chk_group').prop('checked', true);
        else
            $('.cls_report_artworkmatcontrol .cls_chk_group').prop('checked', false);

        $('.cls_report_artworkmatcontrol .cls_chk_group').change();
    });

    $(".cls_report_artworkmatcontrol form").submit(function (e) {
        table_report_artworkmatcontrol.ajax.reload();
        e.preventDefault();
    });

    $(".cls_report_artworkmatcontrol .cls_btn_clear").click(function () {
        $('.cls_report_artworkmatcontrol input[type=text]').val('');
        $('.cls_report_artworkmatcontrol input[type=checkbox]').prop('checked', false);
        $('.cls_report_artworkmatcontrol textarea').val('');
        $(".cls_report_artworkmatcontrol .cls_lov_search_sold_to").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_ship_to").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_brand").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_zone").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_country").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_pic").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_status").val('').trigger("change");
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box

    $(".cls_report_artworkmatcontrol .cls_btn_export_excel").click(function () {
        window.open("/excel/MaterialLockReport?data.GENERATE_EXCEL=X&data.SEARCH_SOLD_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_sold_to option:selected').text()
            + "&data.SEARCH_SHIP_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_ship_to option:selected').text()
            + "&data.SEARCH_MATERIAL_NO=" + $('.cls_report_artworkmatcontrol .cls_txtarea_mat5').val().replace(/\n/g, ',')
            + "&data.SEARCH_COUNTRY=" + $('.cls_report_artworkmatcontrol .cls_txtarea_country').val().replace(/\n/g, ',')
            + "&data.SEARCH_BRAND=" + $('.cls_report_artworkmatcontrol .cls_lov_search_brand option:selected').text()
            + "&data.SEARCH_PIC=" + $('.cls_report_artworkmatcontrol .cls_lov_search_pic option:selected').text()
            + "&data.SEARCH_ZONE=" + $('.cls_report_artworkmatcontrol .cls_lov_search_zone option:selected').text()
            + "&data.SEARCH_STATUS=" + $('.cls_report_artworkmatcontrol .cls_lov_search_status option:selected').val()
            + "&data.SEARCH_PRODUCT_CODE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_search_product_code').val().replace(/\n/g, ',')
            + "&data.SEARCH_PKG_TYPE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_pkg').val().replace(/\n/g, ',')
            + "&data.REMARK_LOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_lock').val().replace(/\n/g, ',')
            + "&data.REMARK_UNLOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_unlock').val().replace(/\n/g, ',')
            , '_blank');
    });

    $(".cls_report_artworkmatcontrol .cls_btn_obsolete").click(function () {
        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {
            $('#modal_report_artworkmatcontrol_obsolete').modal({
                backdrop: 'static',
                keyboard: true
            });
        }
        else {
            alertError2("Please select material at least 1 item.");
        }
    });

    $(".cls_report_artworkmatcontrol .cls_btn_cancel_unlock").click(function () {
        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {
            cancelunlock();
        }
        else {
            alertError2("Please select material at least 1 item.");
        }
    });

    $(".cls_report_artworkmatcontrol .cls_btn_inuse").click(function () {
        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {
            submitInuse();
        }
        else {
            alertError2("Please select material at least 1 item.");
        }
    });

    $("#modal_report_artworkmatcontrol_unlock form").submit(function (e) {
        if ($(this).valid()) {
            if (unlock_remark_quill.root.innerHTML == "<p><br></p>") {
                alertError2("Please fill in the remark");
                return false;
            }
            else
                SubmitUnlock();
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $("#modal_report_artworkmatcontrol_obsolete form").submit(function (e) {
        if ($(this).valid()) {
            if (obsolete_remark_quill.root.innerHTML == "<p><br></p>") {
                alertError2("Please fill in the remark");
                return false;
            }
            else
                submitObsolete();
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_report_artworkmatcontrol .cls_unlock_btn").click(function () {
        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {

            $(".cls_report_artworkmatcontrol .cls_dt_search_unlock_date_from").val('').trigger("change");
            $(".cls_report_artworkmatcontrol .cls_dt_search_unlock_date_to").val('').trigger("change");

            $('#modal_report_artworkmatcontrol_unlock').modal({
                backdrop: 'static',
                keyboard: true
            });
        }
        else {
            alertError2("Please select material at least 1 item.");
        }
    });

    $(".cls_report_artworkmatcontrol .cls_btn_download_file").click(function () {
        if (table_report_artworkmatcontrol == undefined) {
            alertError2('Please select at least 1 item.');
        } else {
            var data = table_report_artworkmatcontrol.rows({ selected: true }).data();
            if (data.length > 0) {
                var dataMat = [];
                var mat_list = "";
                for (i = 0; i < data.length; i++) {
                    if (dataMat.indexOf(data[i].MATERIAL_NO) == -1) {
                        dataMat.push(data[i].MATERIAL_NO);
                        //if (data[i].IS_HAS_FILES_DISPLAY_TXT == 'Yes') {
                        if (mat_list.length > 0) {
                            mat_list = mat_list + "||" + data[i].MATERIAL_NO;
                        }
                        else {
                            mat_list = data[i].MATERIAL_NO;
                        }
                        //}
                    }
                }
                //if (mat_list.length == 0)
                //    return alertError2('Please select packaging material that has file');

                var url = suburl + '/FileUpload/DownloadLockMat?mat_list=' + mat_list;
                window.open(url, '_blank');
            } else {
                alertError2('Please select at least 1 item.');
            }
        }
    });
});

function SubmitUnlock() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    $(".cls_report_artworkmatcontrol .cls_chk_mat5:checked").each(function () {
        var item = {};

        item["MATERIAL_LOCK_ID"] = $(this).data("mat_id");
        item["UNLOCK_DATE_FROM_PARAM"] = $('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_from').val();
        item["UNLOCK_DATE_TO_PARAM"] = $('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_to').val();
        item["REMARK_UNLOCK"] = unlock_remark_quill.root.innerHTML;
        item["UPDATE_BY"] = UserID;
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/report/materiallockreport';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, reload_table_mat, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please inform supervisor for take action</span>');
}

function cancelunlock() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    $(".cls_report_artworkmatcontrol .cls_chk_mat5:checked").each(function () {
        var item = {};

        item["MATERIAL_LOCK_ID"] = $(this).data("mat_id");
        item["UNLOCK_DATE_FROM_PARAM"] = "";
        item["UNLOCK_DATE_TO_PARAM"] = "";
        item["REMARK_UNLOCK"] = "";
        item["UPDATE_BY"] = UserID;
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/report/materiallockreport';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, reload_table_mat, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please inform supervisor for take action</span>');
    //myAjaxConfirmSubmit(myurl, mytype, mydata, reload_table_mat, "", true, true);
}

function reload_table_mat() {
    $('#modal_report_artworkmatcontrol_unlock').modal('hide');
    $('#modal_report_artworkmatcontrol_obsolete').modal('hide');
    table_report_artworkmatcontrol.ajax.reload();
    unlock_remark_quill.setContents([{ insert: '\n' }]);
    obsolete_remark_quill.setContents([{ insert: '\n' }]);
}
function submitObsolete() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    $(".cls_report_artworkmatcontrol .cls_chk_mat5:checked").each(function () {
        var item = {};

        item["MATERIAL_LOCK_ID"] = $(this).data("mat_id");
        item["STATUS"] = "O";
        item["REMARK_LOCK"] = obsolete_remark_quill.root.innerHTML;
        item["UPDATE_BY"] = UserID;
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/report/materiallockreport';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, reload_table_mat, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please inform supervisor for take action</span>');
    //myAjaxConfirmSubmit(myurl, mytype, mydata, reload_table_mat, "", true, true);
}

function submitInuse() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    $(".cls_report_artworkmatcontrol .cls_chk_mat5:checked").each(function () {
        var item = {};

        item["MATERIAL_LOCK_ID"] = $(this).data("mat_id");
        item["STATUS"] = "I";
        item["REMARK_LOCK"] = "";
        item["UPDATE_BY"] = UserID;
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/report/materiallockreport';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, reload_table_mat, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please inform supervisor for take action</span>');
    //myAjaxConfirmSubmit(myurl, mytype, mydata, reload_table_mat, "", true, true);
}

var table_report_artworkmatcontrol;
function bind_table_report_artworkmatcontrol() {
    var groupColumn = 1;
    table_report_artworkmatcontrol = $('#table_report_artworkmatcontrol').DataTable({
        serverSide: true,
        select: {
            'style': 'multi',
            selector: 'td:first-child input',
            info: false
        },
        orderCellsTop: true,
        fixedHeader: true,
        ajax: function (data, callback, settings) {

            for (var i = 0, len = data.columns.length; i < len; i++) {
                delete data.columns[i].name;
                delete data.columns[i].data;
                delete data.columns[i].searchable;
                delete data.columns[i].orderable;
                delete data.columns[i].search.regex;
                delete data.search.regex;
            }


            $.ajax({
                url: suburl + "/api/report/materiallockreport" + getParamReportArtworkMatControl(),
                type: 'GET',
                data: data,
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
            {
                "searchable": true,
                "orderable": true,
                "targets": 2
            },
            {
                "searchable": false,
                "orderable": false,
                "targets": 4
            },

            { "visible": false, "targets": groupColumn }
        ],
        "order": [[2, 'asc']],
        "processing": true,
        "lengthChange": true,
        "ordering": false,
        "info": true,
        "scrollX": true,
        "scrollCollapse": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_hide cls_chk_mat5 cls_td_width_10" data-mat_id="' + row.MATERIAL_LOCK_ID + '" type="checkbox">';
                }
            },
            { "data": "GROUPING", "className": "cls_nowrap cls_td_mat5" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },

            { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
            { "data": "SOLD_TO", "className": "cls_nowrap" },
            { "data": "SHIP_TO", "className": "cls_nowrap" },
            { "data": "BRAND", "className": "cls_nowrap" },
            { "data": "COUNTRY", "className": "cls_nowrap" },
            { "data": "ZONE", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" },
            { "data": "", "className": "cls_nowrap" }
        ],
        "rowCallback": function (row, data, index) {
            $(row).addClass('group-' + data.GROUPING);
            $(row).addClass('head-' + data.GROUPING);
            if (isEmpty(data.PRODUCT_CODE)
                && isEmpty(data.SOLD_TO)
                && isEmpty(data.SHIP_TO)
                && isEmpty(data.BRAND)
                && isEmpty(data.COUNTRY)
                && isEmpty(data.ZONE)
                && isEmpty(data.PACKAGING_TYPE)
                && isEmpty(data.PRIMARY_TYPE)
                && isEmpty(data.PRIMARY_SIZE)
                && isEmpty(data.PACK_SIZE)
                && isEmpty(data.PACKAGING_STYLE)
                && isEmpty(data.PIC_DISPLAY_TXT)
                && isEmpty(data.PG_OWNER_DISPLAY_TXT)) {
                $(row).addClass('cls_hide');
            }
               

            $(".cls_report_artworkmatcontrol #mat_select_all").prop('checked', false);
        },
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            countheader = 0;
            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                var str_grouping = "";
                var str_mat5 = "";
                var str_matdes = "";
                var str_req_no = "";
                var str_art_no = "";
                var str_mock_no = "";
                var str_update_from = "";
                var str_update_to = "";
                var str_primary_type = "";
                var str_primary_size = "";
                var str_pack_size = "";
                var str_pkg_style = "";
                var str_pic = "";
                var str_pg_owner = "";
                var str_pkg_type = "";
                var str_status = "";
                var str_aw_status = "";
                var str_remark_unlock = "";
                var str_remark_lock = "";
                var str_log_date = "";

                for (var x = 0; x < rows.data().length; x++) {
                    if (rows.data()[x].GROUPING == group) {
                        str_grouping = rows.data()[x].GROUPING;

                        if (rows.data()[x].MATERIAL_NO != null) {
                            str_mat5 = rows.data()[x].MATERIAL_NO;
                        }
                        if (rows.data()[x].MATERIAL_DESCRIPTION != null) {
                            str_matdes = rows.data()[x].MATERIAL_DESCRIPTION;
                        }
                        if (rows.data()[x].ARTWORK_NO != null) {
                            //str_art_no = rows.data()[x].ARTWORK_NO;
                            if (rows.data()[x].ARTWORK_ID != null)
                                str_art_no = '<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + rows.data()[x].ARTWORK_ID + '" style="text-decoration: underline;">' + rows.data()[x].ARTWORK_NO + '</a>';
                            else
                                str_art_no = rows.data()[x].ARTWORK_NO;
                        }
                        if (rows.data()[x].MOCKUP_NO != null) {
                            //str_mock_no = rows.data()[x].MOCKUP_NO;
                            if (rows.data()[x].MOCKUP_ID != null)
                                str_mock_no = '<a target="_blank" href="' + suburl + '/TaskForm/' + rows.data()[x].MOCKUP_ID + '" style="text-decoration: underline;">' + rows.data()[x].MOCKUP_NO + '</a>';
                            else
                                str_mock_no = rows.data()[x].MOCKUP_NO;
                        }
                        if (rows.data()[x].REQUEST_FORM_NO != null) {
                            //str_req_no = rows.data()[x].REQUEST_FORM_NO;
                            if (rows.data()[x].REQUEST_FORM_ID != null)
                                str_req_no = '<a target="_blank" href="' + suburl + '/Artwork/' + rows.data()[x].REQUEST_FORM_ID + '" style="text-decoration: underline;">' + rows.data()[x].REQUEST_FORM_NO + '</a>';
                            else
                                str_req_no = rows.data()[x].REQUEST_FORM_NO;
                        }
                        if (rows.data()[x].PRIMARY_TYPE != null) {
                            str_primary_type = rows.data()[x].PRIMARY_TYPE;
                        }
                        if (rows.data()[x].PRIMARY_SIZE != null) {
                            str_primary_size = rows.data()[x].PRIMARY_SIZE;
                        }
                        if (rows.data()[x].PACKAGING_STYLE != null) {
                            str_pkg_style = rows.data()[x].PACKAGING_STYLE;
                        }
                        if (rows.data()[x].PACK_SIZE != null) {
                            str_pack_size = rows.data()[x].PACK_SIZE;
                        }
                        if (rows.data()[x].PIC_DISPLAY_TXT != null) {
                            str_pic = rows.data()[x].PIC_DISPLAY_TXT;
                        }
                        if (rows.data()[x].PG_OWNER_DISPLAY_TXT != null) {
                            str_pg_owner = rows.data()[x].PG_OWNER_DISPLAY_TXT;
                        }
                        if (rows.data()[x].PACKAGING_TYPE != null) {
                            str_pkg_type = rows.data()[x].PACKAGING_TYPE;
                        }
                        if (rows.data()[x].STATUS_DISPLAY_TXT != null) {
                            str_status = '';
                            if (rows.data()[x].STATUS_DISPLAY_TXT == "In use")
                                str_status = '<label style="color:green;margin-bottom:0px;font-weight:normal;">' + rows.data()[x].STATUS_DISPLAY_TXT + '</label>';
                            else
                                str_status = '<label style="color:red;margin-bottom:0px;font-weight:normal;">' + rows.data()[x].STATUS_DISPLAY_TXT + '</label>';
                        }
                        if (rows.data()[x].IS_HAS_FILES_DISPLAY_TXT != null) {
                            if (rows.data()[x].IS_HAS_FILES_DISPLAY_TXT == "Yes")
                                str_aw_status = '<label style="color:green;margin-bottom:0px;font-weight:normal;">' + rows.data()[i].IS_HAS_FILES_DISPLAY_TXT + '</label>';
                            else
                                str_aw_status = '<label style="color:red;margin-bottom:0px;font-weight:normal;">' + rows.data()[i].IS_HAS_FILES_DISPLAY_TXT + '</label>';
                        }
                        if (rows.data()[x].UNLOCK_DATE_FROM != null) {
                            str_update_from = myDateMoment(rows.data()[x].UNLOCK_DATE_FROM);
                        }
                        if (rows.data()[x].UNLOCK_DATE_TO != null) {
                            str_update_to = myDateMoment(rows.data()[x].UNLOCK_DATE_TO);
                        }
                        if (rows.data()[x].REMARK_UNLOCK != null) {
                            str_remark_unlock = rows.data()[x].REMARK_UNLOCK;
                        }
                        if (rows.data()[x].REMARK_LOCK != null) {
                            str_remark_lock = rows.data()[x].REMARK_LOCK;
                        }
                        //if (rows.data()[x].LOG_DATE != null && (rows.data()[x].REMARK_LOCK != null || rows.data()[x].REMARK_UNLOCK != null)) {
                            str_log_date = rows.data()[x].LOG_DATE;
                        //} 
                    }

                }
                //colspan = "22"  data-mat_id="' + rows.data()[x].MATERIAL_LOCK_ID + '"
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group highlight cls_head_chk_group_' + "group-" + str_grouping
                        + '"><td><input data-group-name="' + "group-" + str_grouping
                        + '" class="cls_chk_group" type="checkbox"/></td>  <td> ' + str_mat5
                        + '</td><td class="cls_td_width_300"> ' + str_matdes
                        + '</td><td> ' + str_aw_status
                        + '</td><td> ' + str_status
                        + '</td><td> ' + str_req_no
                        + '</td><td> ' + str_art_no
                        + '</td><td> ' + str_mock_no
                        + '</td><td> ' + str_update_from
                        + '</td><td> ' + str_update_to
                        + '</td><td class="cls_td_width_240"> ' + str_remark_unlock
                        + '</td><td class="cls_td_width_240"> ' + str_remark_lock
                        + '</td><td class="cls_td_width_130"> ' + myDateTimeMoment(str_log_date)
                        + '</td><td colspan = "6"></td>'
                        + '</td><td> ' + str_pkg_type
                        + '</td><td> ' + str_primary_type
                        + '</td><td> ' + str_primary_size
                        + '</td><td> ' + str_pack_size
                        + '</td><td> ' + str_pkg_style
                        + '</td><td> ' + str_pic
                        + '</td><td> ' + str_pg_owner
                        + '</td></tr>'
                    );
                    last = group;
                    countheader++;
                }

            });

            //$('#table_report_artworkmatcontrol_info').html("Showing " + countheader + " entries");
        },
    });
    var aa = '';

}

function getParamReportArtworkMatControl() {
    return "?data.SEARCH_SOLD_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_sold_to option:selected').text()
        + "&data.SEARCH_SHIP_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_ship_to option:selected').text()
        + "&data.SEARCH_MATERIAL_NO=" + $('.cls_report_artworkmatcontrol .cls_txtarea_mat5').val().replace(/\n/g, ',')
        + "&data.SEARCH_COUNTRY=" + $('.cls_report_artworkmatcontrol .cls_txtarea_country').val().replace(/\n/g, ',')
        + "&data.SEARCH_BRAND=" + $('.cls_report_artworkmatcontrol .cls_lov_search_brand option:selected').text()
        + "&data.SEARCH_PIC=" + $('.cls_report_artworkmatcontrol .cls_lov_search_pic option:selected').text()
        + "&data.SEARCH_ZONE=" + $('.cls_report_artworkmatcontrol .cls_lov_search_zone option:selected').text()
        + "&data.SEARCH_STATUS=" + $('.cls_report_artworkmatcontrol .cls_lov_search_status option:selected').val()
        + "&data.SEARCH_PRODUCT_CODE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_search_product_code').val().replace(/\n/g, ',')
        + "&data.SEARCH_PKG_TYPE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_pkg').val().replace(/\n/g, ',')
        + "&data.REMARK_LOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_lock').val().replace(/\n/g, ',')
        + "&data.REMARK_UNLOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_unlock').val().replace(/\n/g, ',')
        + "&data.first_load=" + first_load;
}
