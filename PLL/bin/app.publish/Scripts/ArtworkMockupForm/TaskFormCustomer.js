$(document).ready(function () {

    $('.cls_pg_submit_modal_to_customer form').validate({
        rules: {
            send_to_customer_approve:
            {
                required: true
            }
        },
        messages: {
            send_to_customer_approve:
            {
                required: "Please fill at least 1 of these fields."
            }
        }
    });

    $('.cls_div_customer form').validate({
        rules: {
            customer_decision:
            {
                required: true
            }
        },
        messages: {
            customer_decision:
            {
                required: "Please fill at least 1 of these fields."
            }
        }
    });

    $(".cls_pg_submit_modal_to_customer form").submit(function (e) {
        if ($(this).valid()) {
            save_send_to_customer();
        }
        else if ($('.cls_pg_submit_modal_to_customer .cls_input_remark_other').is(':visible') && $('.cls_pg_submit_modal_to_customer .cls_input_remark_other').val() == '') {
            alertError2("Please fill remark reason");
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_div_customer form").submit(function (e) {
        if ($(this).valid()) {
            customer_send_wf('SUBMIT');
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });
    $(".cls_div_customer .cls_send_back").click(function () {
        customer_send_wf('SEND_BACK');
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_customer':
                bind_data_tab_customer();
                break;
            default:
                break;
        }
    });
    bind_lov('.cls_div_customer .cls_lov_decision_cancel', '/api/common/desisioncancel?data.STEP_CODE=MOCK_UP_CUSTOMER_SEND_TO_PG_CANCEL', 'data.description');
    bind_lov('.cls_div_customer .cls_lov_decision_revise', '/api/common/desisionrevise?data.STEP_CODE=MOCK_UP_CUSTOMER_SEND_TO_PG_REVISE', 'data.description');

    $('.cls_div_customer input[type=radio][name=customer_decision]').change(function () {
        if (this.value == 'APPROVE') {
            $('.cls_div_customer .cls_lov_decision_revise').attr("required", false);
            $('.cls_div_customer .cls_lov_decision_cancel').attr("required", false);
        }
        else if (this.value == 'REVISE') {
            $('.cls_div_customer .cls_lov_decision_revise').attr("required", true);
            $('.cls_div_customer .cls_lov_decision_cancel').attr("required", false);
        }
        else if (this.value == 'CANCEL') {
            $('.cls_div_customer .cls_lov_decision_revise').attr("required", false);
            $('.cls_div_customer .cls_lov_decision_cancel').attr("required", true);
        }
    });
});

function bind_data_tab_customer() {
    if (table_cus_log == null)
        bind_cus_log();
    else
        table_cus_log.ajax.reload();
}

function customer_send_wf(action) {
    var jsonObj = new Object();
    jsonObj.data = {};

    jsonObj.data["REASON_ID"] = $('.cls_pg_submit_modal_to_customer .cls_lov_send_for_reason').val();
    jsonObj.data["MOCKUP_ID"] = MOCKUPID;
    jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    jsonObj.data["ENDTASKFORM"] = true;
    jsonObj.data["ACTION_CODE"] = action;
    jsonObj.data["COMMENT"] = $('.cls_div_customer .cls_c_commect').val();
    jsonObj.data["DECISION"] = $(".cls_div_customer input[name='customer_decision']:checked").val();
    jsonObj.data["CREATE_BY"] = UserID;
    jsonObj.data["UPDATE_BY"] = UserID;

    if (jsonObj.data["REASON_ID"] == DefaultResonId && action == "SEND_BACK") {
        alertError2("Please select reason for send back");
        return false;
    }

    if ($(".cls_div_customer input[name='customer_decision']:checked").val() == "REVISE") {
        jsonObj.data["REVISE_ID"] = $('.cls_div_customer .cls_lov_decision_revise').val();
    }
    if ($(".cls_div_customer input[name='customer_decision']:checked").val() == "CANCEL") {
        jsonObj.data["CANCEL_ID"] = $('.cls_div_customer .cls_lov_decision_cancel').val();
    }

    var myurl = '/api/taskform/customer/info';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage);
}

function save_send_to_customer() {
    var jsonObj = new Object();
    var item = {};
    jsonObj.data = [];

    item["MOCKUP_ID"] = MOCKUPID;
    //jsonObj.data["MOCKUP_SUB_ID"] = MOCKUPSUBID;
    item["PACKING_STYLE"] = $('.cls_pg_submit_modal_to_customer .cls_c_packing_style').val();
    //jsonObj.data["PURPOSE_OF"] = $('.cls_pg_submit_modal_to_customer .cls_c_purpose_of').val();
    item["PURPOSE_OF"] = '';
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;
    item["IS_SEND_DIE_LINE"] = $('.cls_pg_submit_modal_to_customer .cls_chk_send_die_line').is(":checked") ? "X" : "";
    item["IS_SEND_PHYSICAL"] = $('.cls_pg_submit_modal_to_customer .cls_chk_send_physical').is(":checked") ? "X" : "";
    item["REASON_ID"] = $('.cls_pg_submit_modal_to_customer .cls_lov_send_for_reason').val();
    item["REMARK_REASON"] = $('.cls_pg_submit_modal_to_customer .cls_input_remark_other').val();
    item["WF_SUB_ID"] = MOCKUPSUBID;
    item["IS_SENDER"] = true;
    item.PROCESS = {};
    item.PROCESS["REASON_ID"] = $('.cls_pg_submit_modal_to_customer .cls_lov_send_for_reason').val();
    item.PROCESS["MOCKUP_ID"] = MOCKUPID;
    item.PROCESS["PARENT_MOCKUP_SUB_ID"] = MOCKUPSUBID;

    var stepId;
    if (isProjectNoCus == "1") {
        stepId = getstepmockup('SEND_MK_APP').curr_step;
        item.PROCESS["CURRENT_STEP_ID"] = stepId;
        item["WF_STEP"] = stepId;
    }
    else {
        stepId = getstepmockup('SEND_CUS_APP').curr_step;
        item.PROCESS["CURRENT_STEP_ID"] = stepId;
        item["WF_STEP"] = stepId;
    }

    item["isProjectNoCus"] = isProjectNoCus;
    //jsonObj.data.PROCESS["CURRENT_CUSTOMER_ID"] = CUSTOMER_ID;
    item.PROCESS["CREATE_BY"] = UserID;
    item.PROCESS["UPDATE_BY"] = UserID;
    var editor = new Quill('.cls_pg_submit_modal_to_customer .cls_txtedt_send_customer');
    item.PROCESS["REMARK"] = editor.root.innerHTML;

    jsonObj.data.push(item);

    var myurl = '/api/taskform/pg/sendtocustomer';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmit(myurl, mytype, mydata, hide_modalto_customer, '', true, true, true);
}

function hide_modalto_customer() {

    resetDllReason('#pg_submit_modal_to_customer .cls_lov_send_for_reason');
    $('#pg_submit_modal_to_customer .cls_lov_search_file_template').val('').trigger('change');

    var text_editor_send_customer = new Quill('#pg_submit_modal_to_customer ' + '.cls_txtedt_send_customer');
    text_editor_send_customer.setContents([{ insert: '\n' }]);

    $('#pg_submit_modal_to_customer input:text:enabled').val('');

    $('.cls_pg_submit_modal_to_customer').modal('hide');
}

function bindSendtoCustomer(mockup_sub_id) {
    var myurl = '/api/taskform/pg/sendtocustomer?data.mockup_sub_id=' + mockup_sub_id;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_sendtocustomer);
}

function callback_get_sendtocustomer(res) {
    if (res.data.length > 0) {
        var v = res.data[0];

        $('.cls_div_customer .cls_lbl_die_line').hide();
        $('.cls_div_customer .cls_lbl_physical').hide();
        if (v.IS_SEND_DIE_LINE == 'X') $('.cls_div_customer .cls_lbl_die_line').show();
        if (v.IS_SEND_PHYSICAL == 'X') $('.cls_div_customer .cls_lbl_physical ').show();
        //$('.cls_div_customer .cls_c_purpose_of').val(v.PURPOSE_OF);
        $('.cls_div_customer .cls_c_packing_style').val(v.PACKING_STYLE);
    }
}

var table_cus_log;
function bind_cus_log() {
    table_cus_log = $('#table_customer_log').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/customer/info?data.mockup_id=' + MOCKUPID + '&data.isProjectNoCus=' + isProjectNoCus + '&data.current_user_id=' + CURRENTUSERID,
                type: 'GET',
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
        "order": [[6, 'desc']],
        "processing": true,
        "lengthChange": false,
        "ordering": true,
        "info": false,
        "searching": false,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "CUSTOMER_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "TOPIC_DISPLAY_TXT", "className": "cls_nowrap" },
            { "data": "PACKING_STYLE", "className": "cls_nowrap" },
            //{ "data": "PURPOSE_OF", "className": "" },
            { "data": "REASON_BY_PG", "className": "" },
            { "data": "REMARK_REASON_BY_PG", "className": "" },
            { "data": "COMMENT_BY_PG", "className": "" },
            { "data": "CREATE_DATE_BY_PG", "className": "cls_td_create_date_by_pg" },
            { "data": "ACTION_NAME", "className": "" },

            { "data": "COMMENT", "className": "" },
            { "data": "DECISION_DISPLAY_TXT", "className": "" },
            { "data": "CREATE_DATE", "className": "cls_td_create_date" },
            //{ "data": "CREATE_BY_DISPLAY_TXT", "className": "cls_nowrap" },
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_td_create_date').html(myDateTimeMoment(data.CREATE_DATE));

            $(row).find('.cls_td_create_date_by_pg').html(myDateTimeMoment(data.CREATE_DATE_BY_PG));

            if (data.MOCKUP_SUB_ID == 0) {
                $('.cls_div_customer .cls_c_commect_by_pg').html(data.COMMENT_BY_PG);
            }

            if (data.MOCKUP_SUB_ID == MOCKUPSUBID) {
                $('.cls_div_customer .cls_c_commect').val(data.COMMENT);

                if (data.DECISION == "APPROVE")
                    $(".cls_div_customer input[name=customer_decision][value=APPROVE]").prop('checked', true);
                else if (data.DECISION == "REVISE") {
                    $(".cls_div_customer input[name=customer_decision][value=REVISE]").prop('checked', true);
                    setValueToDDL('.cls_div_customer .cls_lov_decision_revise', data.REVISE_ID, data.DECISION_DISPLAY_TXT2);
                }
                else if (data.DECISION == "CANCEL") {
                    $(".cls_div_customer input[name=customer_decision][value=CANCEL]").prop('checked', true);
                    setValueToDDL('.cls_div_customer .cls_lov_decision_cancel', data.CANCEL_ID, data.DECISION_DISPLAY_TXT2);
                }
            }
        }
    });

    table_cus_log.on('order.dt search.dt', function () {
        table_cus_log.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}