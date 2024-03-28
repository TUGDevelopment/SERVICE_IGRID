var countFiles = 0;

$(document).ready(function () {
    $('.cls_form_upload_file_checklist').bind('fileuploadsubmit', function (e, data) {
        var roleId = '';
        var MOCKUPSUBID = 0;
        if (!isEmpty($(this).closest('.cls_task_form_attachment').find('.cls_go_to_step').val())) {
            roleId = getstepmockup($(this).closest('.cls_task_form_attachment').find('.cls_go_to_step').val()).curr_step;
        }

        MOCKUPSUBID = $(this).closest('.cls_task_form_attachment').find('.cls_mockup_sub_id').val();

        var IS_VENDOR = '';
        var IS_INTERNAL = 'X';
        var IS_CUSTOMER = '';
        var go_to_step = $(this).closest('.cls_task_form_attachment').find('.cls_go_to_step').val();

        if (IsVendor == "0") {
            if (go_to_step == 'SEND_VN_MB' || go_to_step == 'SEND_VN_DL' || go_to_step == 'SEND_VN_RS' || go_to_step == 'SEND_VN_PR' || go_to_step == 'SEND_VN_QUO') {
                IS_VENDOR = 'X'
            }
        }
        else {
            if (go_to_step == 'SEND_VN_DL') {
                IS_CUSTOMER = 'X'
            }
        }

        if (IsCustomer == "0") {
            if (go_to_step == 'SEND_CUS_APP') {
                IS_CUSTOMER = 'X'
            }
        }
        data.formData = {
            mockupSubId: MOCKUPSUBID,
            userId: UserID,
            roldId: roleId,
            IS_VENDOR: IS_VENDOR,
            IS_INTERNAL: IS_INTERNAL,
            IS_CUSTOMER: IS_CUSTOMER
        };
    });

    $(function () {
        $('.cls_form_upload_file_checklist').fileupload({
            sequentialUploads: true,
            dropZone: $('.cls_form_upload_file_checklist .dropzone'),
            dataType: 'json',
            url: suburl + '/fileUpload/upload',
        });
    });
    $('#popup_attachment_checklist').on('shown.bs.modal', function (e) {
        $('.cls_go_to_step').val($(e.relatedTarget).data('go_to_step'));
        $('.cls_mockup_sub_id').val($(e.relatedTarget).data('mockup_sub_id'));
        $('.cls_mockup_id').val($(e.relatedTarget).data('mockup_id'));
        $('#popup_attachment_label_checklist').text('Attachment ( ' + $(e.relatedTarget).data('mockup_no') + ' )');

        var roleId = getstepmockup($(e.relatedTarget).data('go_to_step')).curr_step;
        var mockupId = $(e.relatedTarget).data('mockup_id');
        var mockupSubId = $(e.relatedTarget).data('mockup_sub_id');
        countFiles = 0;

        bindDataAttachChecklist(mockupId, mockupSubId, roleId);
        $('.cls_div_table').css('max-height', '300px').css('overflow-y', 'auto');
    })
    $('#popup_attachment_checklist').on('hidden.bs.modal', function (e) {

        var lblCountFiles = '#cls_count_files' + $('.cls_mockup_sub_id').val();
        countFiles = $('.cls_table_attachment >tbody >tr').length;
        $(lblCountFiles).text(countFiles + ' Files');

        $('.cls_go_to_step').val('');
        $('.cls_mockup_sub_id').val('');
        $('.cls_mockup_id').val('');
        $('#popup_attachment_label_checklist').text('');
        countFiles = 0;

        $('.cls_div_table').css('max-height', 'none').css('overflow-y', 'unset');
    })

    $(document).on("click", ".cls_form_upload_file_checklist .cls_delete_file", function () {
        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["NODE_ID"] = $(this).data("node-id");
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;
        jsonObj.data = item;
        delete_node_id = $(this).data("node-id");
        var myurl = '/api/taskform/attachment/delete';
        var mytype = 'DELETE';
        var mydata = jsonObj;
        myAjaxConfirmDelete(myurl, mytype, mydata, callback_delete_file_complete);
    });

    $(document).on("change", ".cls_chk_att_visibility", function () {
        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["NODE_ID"] = $(this).closest('tr').data("node-id");
        item["IS_CUSTOMER"] = $(this).closest('tr').find(".cls_chk_att_visibility_customer").is(":checked") ? "X" : "";
        item["IS_INTERNAL"] = $(this).closest('tr').find(".cls_chk_att_visibility_internal").is(":checked") ? "X" : "";
        item["IS_VENDOR"] = $(this).closest('tr').find(".cls_chk_att_visibility_vendor").is(":checked") ? "X" : "";
        jsonObj.data = item;
        var myurl = '/api/taskform/attachment/visibility';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjax(myurl, mytype, mydata);
    });
});

var delete_node_id;
function callback_delete_file_complete(res) {
    $('.cls_table_attachment > tbody  > tr').each(function () {
        if ($(this).find('.cls_delete_file').data("node-id") == delete_node_id) {
            $(this).remove();
        }
    });

    countFiles =  $('.cls_table_attachment >tbody >tr').length;
}

function bindDataAttachChecklist(mockup_id, mockup_sub_id, roleId) {
    if (isEmpty(roleId)) roleId = '';

    $('.cls_form_upload_file_checklist').addClass('fileupload-processing');
    $('.cls_form_upload_file_checklist .files').empty();
    var myurl = '/api/taskform/attachment/info?data.mockup_sub_id=' + mockup_sub_id + '&data.mockup_id=' + mockup_id + '&data.role_id=' + roleId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_attach_checklist);
}

function callback_get_attach_checklist(res) {
    countFiles = res.data.length;

    for (var i = 0; i < res.data.length; i++) {
        res.data[i]["thumbnailUrl"] = suburl + "/Content/Free-file-icons/32px/" + res.data[i].EXTENSION + ".png";
        res.data[i]["name"] = res.data[i].FILE_NAME;
        res.data[i]["size"] = res.data[i].SIZE;
        res.data[i]["nodeid"] = res.data[i].NODE_ID;
        res.data[i]["version2"] = res.data[i].VERSION2;
        res.data[i]["IS_CUSTOMER"] = res.data[i].IS_CUSTOMER;
        res.data[i]["IS_INTERNAL"] = res.data[i].IS_INTERNAL;
        res.data[i]["IS_VENDOR"] = res.data[i].IS_VENDOR;
        res.data[i]["NODE_ID_TXT"] = res.data[i].NODE_ID_TXT;
    }
    $(".cls_form_upload_file_checklist").each(function (index) {
        $(this).fileupload('option', 'done').call($(this), $.Event('done'), { result: { files: res.data } })
        $(this).removeClass('fileupload-processing');
    });

    countFiles = $('.cls_table_attachment >tbody >tr').length;
}
