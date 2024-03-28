var table_reassign2;
$(document).ready(function () {
    $('#popup_reassign2 form').validate({
        rules: {
            reassign_2:
            {
                required: true
            }

        },
        messages: {
            reassign_2:
            {
                required: "Please fill remark first"
            }
        }
    });
    bind_lov_no_ajax('.cls_form_reassign2 .cls_workflow_type', '', '');
    bind_lov('.cls_form_reassign2 .cls_current_assign', '/api/lov/user', 'data.DISPLAY_TXT');

    $(".cls_form_search form").submit(function (e) {
        table_reassign2.ajax.reload();
        e.preventDefault();
    });

    $(".cls_form_reassign2 .cls_btn_clear").click(function () {
        $('.cls_form_reassign2 input[type=text]').val('');
        $('.cls_form_reassign2 .cls_workflow_type').val('All').change();
    });

    table_reassign2 = $('#table_reassign2').DataTable({
        "serverSide": true,
        "pageLength": 25,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/wfsetting/reassign2/info?data.wf_no=" + $('.cls_form_reassign2 .cls_workflow_number').val()
                    + "&data.wf_type=" + $('.cls_form_reassign2 .cls_workflow_type').val()
                    + "&data.create_date_from=" + $('.cls_form_reassign2 .cls_create_date_from').val()
                    + "&data.create_date_to=" + $('.cls_form_reassign2 .cls_create_date_to').val()
                    + "&data.current_user_id=" + $('.cls_form_reassign2 .cls_current_assign').val()
                    + "&data.user_id=" + UserID
                ,
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
                "searchable": false,
                "orderable": false,
                "targets": 1
            },
            {
                "searchable": false,
                "orderable": false,
                "targets": 4
            },
            {
                "searchable": false,
                "orderable": false,
                "targets": 5
            },
            {
                "searchable": false,
                "orderable": false,
                "targets": 6
            }
        ],
        "order": [[3, 'asc']],
        "processing": true,
        "searching": false,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        stateSave: false,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_chk_reassign_wf" data-wf_id="' + row.REQUEST_ID + '" type="checkbox">';
                }
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "WORKFLOW_TYPE", "className": "cls_nowrap cls_td_wf_type" },
            { "data": "REQUEST_NO", "className": "cls_nowrap" },
            { "data": "WORKFLOW_NO", "className": "cls_nowrap" },
            { "data": "ASSIGN_FROM_USER_DISPLAY_TXT", "className": "" },
            { "data": "ASSIGN_TO_USER_DISPLAY_TXT", "className": "" }
        ],
        "rowCallback": function (row, data, index) {
            var wf_html = "";
            var wf_type = "";
            var rf_type = "";
            if (data.WF_TYPE == "Mockup") {
                wf_type = 'TaskForm';
                rf_type = 'CheckList';
            }
            else {
                wf_type = 'TaskFormArtwork';
                rf_type = 'Artwork';
            }

            $(row).find('td').eq(3).html('<a target="_blank" href="' + suburl + '/' + rf_type + '/' + data.REQUEST_ID + '" style="text-decoration: underline;">' + data.REQUEST_NO + '</a>');
            
            if (data.WORKFLOW_NO.length > 0) {
                var arr_wf = data.WORKFLOW_NO.split('||');
                var wf_format = '<a target="_blank" href="' + suburl + '/' + wf_type + '/{1}" style="text-decoration: underline;">{0}</a>';
                for (var i = 0; i < arr_wf.length; i++) {
                    var arr_wf_no = arr_wf[i].split(',');
                    if (arr_wf_no.length > 1) {
                        if (wf_html.length > 0) {
                            wf_html += '<br>';
                        }
                        wf_html += wf_format.format(arr_wf_no[0], arr_wf_no[1]);
                    }
                }
            }

            $(row).find('td').eq(4).html(wf_html);
        },
    });

    //table_reassign.on('order.dt search.dt', function () {
    //    table_reassign.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
    //        cell.innerHTML = i + 1;
    //    });
    //}).draw();

    bind_lov('#popup_reassign2 .cls_lov_assign_to', '/api/lov/user', 'data.DISPLAY_TXT');

    $("#popup_reassign2 form").submit(function (e) {
        if ($(this).valid()) {
            myAjaxConfirmSubmitBlank(callbackSubmitChangeOwner);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_form_reassign2 .cls_btn_reassign").click(function () {
        if ($('.cls_form_reassign2 .cls_chk_reassign_wf:checked').length > 0) {

            $('.cls_form_reassign2 .cls_input_remark').val('');
            $('.cls_form_reassign2 .cls_lov_assign_to').val(null).trigger('change');

            $('#popup_reassign2').modal({
                backdrop: 'static',
                keyboard: true
            });
        }
        else {
            alertError2("Please select workitem at least 1 item.");
        }
    });
});

function reload_table_reassign2() {
    $('#popup_reassign2').modal('hide');
    table_reassign2.ajax.reload();
}

function callbackSubmitChangeOwner() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    $(".cls_form_reassign2 .cls_chk_reassign_wf:checked").each(function () {
        var item = {};

        item["WORKFLOW_ID"] = $(this).data("wf_id");
        item["WORKFLOW_TYPE"] = $(this).closest('tr').find('.cls_td_wf_type').text();
        item["CURRENT_USER_ID"] = $('.cls_form_reassign2 .cls_lov_assign_to').val();
        item["REMARK_REASSIGN"] = $('.cls_form_reassign2 .cls_input_remark').val();
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/wfsetting/reassign2/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxAlertNoBack(myurl, mytype, mydata, reload_table_reassign2, "", true, true);
}
