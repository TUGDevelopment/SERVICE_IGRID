var artwork_action_att_version = false;
$(document).ready(function () {
    $('.cls_form_upload_file_version_artwork').bind('fileuploadsubmit', function (e, data) {
        artwork_action_att_version = true;
        data.formData = {
            artworkSubId: ArtworkSubId,
            userId: UserID,
            nodeId: $('.cls_node_id_file_version_artwork').val(),
        };
    });

    $(function () {
        $('.cls_form_upload_file_version_artwork').fileupload({
            sequentialUploads: true,
            dropZone: $('.cls_form_upload_file_version_artwork .dropzone'),
            dataType: 'json',
            url: suburl + '/fileUpload/upload_artwork_version',
            uploadTemplateId: 'template-upload_file_version_artwork',
            downloadTemplateId: 'template-download_file_version_artwork',
        });
    });
    $('#popup_attachment_file_version_artwork').on('shown.bs.modal', function (e) {
        artwork_action_att_version = false;
        $('.cls_node_id_file_version_artwork').val($(e.relatedTarget).data('node_id_file_version'));
        bindDataAttachArtwork_version($('.cls_node_id_file_version_artwork').val());
        $('.cls_div_table_version_artwork').css('max-height', '300px').css('overflow-y', 'auto');
    })
    $('#popup_attachment_file_version_artwork').on('hidden.bs.modal', function (e) {
        $('.cls_node_id_file_version_artwork').val('');
        $('.cls_div_table_version_artwork').css('max-height', 'none').css('overflow-y', 'unset');
        if (artwork_action_att_version)  bindDataAttachArtwork();
    })
    $(document).on("click", ".cls_form_upload_file_version_artwork .cls_delete_file_version_artwork", function () {
        artwork_action_att_version = true;
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
        var myurl = '/api/taskform/attachment/artwork/deleteversion';
        var mytype = 'DELETE';
        var mydata = jsonObj;
        myAjaxConfirmDelete(myurl, mytype, mydata, callback_delete_file_complete_version_artwork);
    });
});
var delete_version;
var delete_node_id;
function callback_delete_file_complete_version_artwork(res) {
    $('.cls_table_attachment_version_artwork > tbody  > tr').each(function () {
        if ($(this).find('.cls_delete_file_version_artwork').data("node-id") == delete_node_id && $(this).find('.cls_delete_file_version_artwork').data("version") == delete_version) {
            $(this).remove();
        }
    });
}

function bindDataAttachArtwork_version(nodeId) {
    $('.cls_form_upload_file_version_artwork').addClass('fileupload-processing');
    $('.cls_form_upload_file_version_artwork .files').empty();
    var myurl = '/api/taskform/attachment/artwork/info_version?data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.node_id=' + nodeId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_attach_version_artwork);
}

function callback_get_attach_version_artwork(res) {
    for (var i = 0; i < res.data.length; i++) {
        res.data[i]["thumbnailUrl"] = suburl + "/Content/Free-file-icons/32px/" + res.data[i].EXTENSION + ".png";
        res.data[i]["name"] = res.data[i].FILE_NAME;
        res.data[i]["size"] = res.data[i].SIZE;
        res.data[i]["nodeid"] = res.data[i].NODE_ID;
        res.data[i]["version"] = res.data[i].VERSION;
        res.data[i]["version2"] = res.data[i].VERSION2;
        res.data[i]["NODE_ID_TXT"] = res.data[i].NODE_ID_TXT;
        res.data[i]["IS_SYSTEM"] = (res.data[i].IS_SYSTEM == null ? "" : res.data[i].IS_SYSTEM);
    }
    $(".cls_form_upload_file_version_artwork").each(function (index) {
        $(this).fileupload('option', 'done').call($(this), $.Event('done'), { result: { files: res.data } })
        $(this).removeClass('fileupload-processing');
    });
}
