var table_reassign;
$(document).ready(function () {
    $('#popup_reassign form').validate({
        rules: {
            recall_:
            {
                required: true
            }

        },
        messages: {
            recall_:
            {
                required: "Please fill remark first"
            }
        }
    });


    bind_lov_no_ajax('.cls_form_recall .cls_workflow_type', '', '');
    bind_lov('.cls_form_recall .cls_current_assign', '/api/lov/user', 'data.DISPLAY_TXT');

    $(".cls_form_search form").submit(function (e) {
        table_reassign.ajax.reload();
        e.preventDefault();
    });

    $(".cls_form_recall .cls_btn_clear").click(function () {
        $('.cls_form_recall input[type=text]').val('');
        $('.cls_form_recall .cls_workflow_type').val('All').change();
    });

    table_reassign = $('#table_reassign').DataTable({
        "serverSide": true,
        "pageLength": 25,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/wfsetting/recall/info?data.wf_no=" + $('.cls_form_recall .cls_workflow_number').val()
                    + "&data.wf_type=" + $('.cls_form_recall .cls_workflow_type').val()
                    + "&data.create_date_from=" + $('.cls_form_recall .cls_create_date_from').val()
                    + "&data.create_date_to=" + $('.cls_form_recall .cls_create_date_to').val()
                    + "&data.current_user_id=" + $('.cls_form_recall .cls_current_assign').val()
                    + "&data.creator_id=" + UserID
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
            }
        ],
        "order": [[4, 'asc']],
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
                    return '<input class="cls_chk_reassign_wf" data-wf_sub_id="' + row.WORKFLOW_SUB_ID + '" type="checkbox">';
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
            { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "CURRENT_USER_DISPLAY_TXT", "className": "" },
        ],
        "rowCallback": function (row, data, index) {
            ////$(row).find('td').eq(1).html('<a href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '" style="text-decoration: underline;">' + ((data.CHECK_LIST_NO === null) ? 'Draft' : data.CHECK_LIST_NO) + '</a>');
            //$(row).find('td').eq(2).html('<a href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '" style="text-decoration: underline;">' + ((data.MOCKUP_NO === null) ? 'Draft' : data.MOCKUP_NO) + '</a>');
            if (data.WF_TYPE == "Mockup") {
                $(row).find('td').eq(3).html('<a target="_blank" href="' + suburl + '/CheckList/' + data.REQUEST_ID + '" style="text-decoration: underline;">' + data.REQUEST_NO + '</a>');
                $(row).find('td').eq(4).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.WORKFLOW_NO + '</a>');
            }

            if (data.WF_TYPE == "Artwork") {
                $(row).find('td').eq(3).html('<a target="_blank" href="' + suburl + '/Artwork/' + data.REQUEST_ID + '" style="text-decoration: underline;">' + data.REQUEST_NO + '</a>');
                $(row).find('td').eq(4).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.WF_SUB_ID + '" style="text-decoration: underline;">' + data.WORKFLOW_NO + '</a>');
            }

            if (isEmpty(data.CURRENT_USER_ID)) {
                $(row).find('td').eq(0).find('.cls_chk_reassign_wf').prop("disabled", true);
            }
        },
    });

    //table_reassign.on('order.dt search.dt', function () {
    //    table_reassign.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
    //        cell.innerHTML = i + 1;
    //    });
    //}).draw();

    bind_lov('#popup_reassign .cls_lov_assign_to', '/api/lov/user', 'data.DISPLAY_TXT');

    $("#popup_reassign form").submit(function (e) {
        if ($(this).valid()) {
            myAjaxConfirmSubmitBlank(callbackSubmitReCall);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_form_recall .cls_btn_reassign").click(function () {
        if ($('.cls_form_recall .cls_chk_reassign_wf:checked').length > 0) {

            $('.cls_form_recall .cls_input_remark').val('');
            $('.cls_form_recall .cls_lov_assign_to').val(null).trigger('change');

            $('#popup_reassign').modal({
                backdrop: 'static',
                keyboard: true
            });
        }
        else {
            alertError2("Please select workitem at least 1 item.");
        }
    });
});

function reload_table_reassign() {
    $('#popup_reassign').modal('hide');
    table_reassign.ajax.reload();
}

function callbackSubmitReCall() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    $(".cls_form_recall .cls_chk_reassign_wf:checked").each(function () {
        var item = {};
        item["WORKFLOW_SUB_ID"] = $(this).data("wf_sub_id");
        item["WORKFLOW_TYPE"] = $(this).closest('tr').find('.cls_td_wf_type').text();
        item["CURRENT_USER_ID"] = $('.cls_form_recall .cls_lov_assign_to').val();
        item["REMARK_REASSIGN"] = $('.cls_form_recall .cls_input_remark').val();
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/wfsetting/recall/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxAlertNoBack(myurl, mytype, mydata, reload_table_reassign, "", true, true);
}