var mockup_action_att_version = false;
$(document).ready(function () {
    $('.cls_form_upload_file_version').bind('fileuploadsubmit', function (e, data) {
        mockup_action_att_version = true;
        data.formData = {
            mockupSubId: MOCKUPSUBID,
            userId: UserID,
            nodeId: $('.cls_node_id_file_version').val(),
        };
    });

    $(function () {
        $('.cls_form_upload_file_version').fileupload({
            sequentialUploads: true,
            dropZone: $('.cls_form_upload_file_version .dropzone'),
            dataType: 'json',
            url: suburl + '/fileUpload/upload_version',
            uploadTemplateId: 'template-upload_file_version',
            downloadTemplateId: 'template-download_file_version',
        });
    });
    $('#popup_attachment_file_version').on('shown.bs.modal', function (e) {
        mockup_action_att_version = false;
        $('.cls_node_id_file_version').val($(e.relatedTarget).data('node_id_file_version'));
        bindDataAttach_version(MOCKUPSUBID, $('.cls_node_id_file_version').val());
        $('.cls_div_table_version').css('max-height', '300px').css('overflow-y', 'auto');
    })
    $('#popup_attachment_file_version').on('hidden.bs.modal', function (e) {
        $('.cls_node_id_file_version').val('');
        $('.cls_div_table_version').css('max-height', 'none').css('overflow-y', 'unset');
        if (mockup_action_att_version) bindDataAttach(MOCKUPSUBID, '');
    })
    $(document).on("click", ".cls_form_upload_file_version .cls_delete_file_version", function () {
        mockup_action_att_version = true;
        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["NODE_ID"] = $(this).data("node-id");
        item["VERSION"] = $(this).data("version");
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;
        jsonObj.data = item;
        delete_node_id = $(this).data("node-id");
        delete_version = $(this).data("version");
        var myurl = '/api/taskform/attachment/deleteversion';
        var mytype = 'DELETE';
        var mydata = jsonObj;
        myAjaxConfirmDelete(myurl, mytype, mydata, callback_delete_file_complete_version);
    });
});
var delete_version;
var delete_node_id;
function callback_delete_file_complete_version(res) {
    $('.cls_table_attachment_version > tbody  > tr').each(function () {
        if ($(this).find('.cls_delete_file_version').data("node-id") == delete_node_id && $(this).find('.cls_delete_file_version').data("version") == delete_version) {
            $(this).remove();
        }
    });
}

function bindDataAttach_version(mockup_sub_id, nodeId) {
    $('.cls_form_upload_file_version').addClass('fileupload-processing');
    $('.cls_form_upload_file_version .files').empty();
    var myurl = '/api/taskform/attachment/info_version?data.mockup_sub_id=' + MOCKUPSUBID + '&data.mockup_id=' + MOCKUPID + '&data.node_id=' + nodeId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_attach_version22);
}

function callback_get_attach_version22(res) {
    for (var i = 0; i < res.data.length; i++) {
        res.data[i]["thumbnailUrl"] = suburl + "/Content/Free-file-icons/32px/" + res.data[i].EXTENSION + ".png";
        res.data[i]["name"] = res.data[i].FILE_NAME;
        res.data[i]["size"] = res.data[i].SIZE;
        res.data[i]["nodeid"] = res.data[i].NODE_ID;
        res.data[i]["version"] = res.data[i].VERSION;
        res.data[i]["version2"] = res.data[i].VERSION2;
        res.data[i]["NODE_ID_TXT"] = res.data[i].NODE_ID_TXT;
    }
    $(".cls_form_upload_file_version").each(function (index) {
        $(this).fileupload('option', 'done').call($(this), $.Event('done'), { result: { files: res.data } })
        $(this).removeClass('fileupload-processing');
    });
}
