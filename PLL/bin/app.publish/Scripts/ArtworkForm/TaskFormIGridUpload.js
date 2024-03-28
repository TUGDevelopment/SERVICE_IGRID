
$(document).ready(function () {


    //bindDataAttachIGird();

    //$(".cls_div_uploadartwork").find("*").prop("disabled", true);
    $('.cls_form_igrid_upload_file').bind('fileuploadsubmit', function (e, data) {
        data.formData = {
            userId: UserID,
            SapMaterialId: IGridSAPMaterialId// $('.cls_hid_artwork_request_form_id').val()
        };
    });

    //$(function () {
    //    $('.cls_form_igrid_upload_file').fileupload({
    //        sequentialUploads: true,
    //        dataType: 'json',
    //        url: suburl + '/fileUpload/upload_artwork',
    //        uploadTemplateId: 'template-upload_file_igrid',
    //        downloadTemplateId: 'template-download_file_igrid',
    //        start: function (e) {
    //            myPleaseWait.show();
    //        },
    //        stop: function (e) {
    //            if (typeSend == 'cus') {
    //                alertSuccessDialog(resUpload.msg);
    //            }
    //            else if (typeSend == 'mk') {
    //                document.location.href = suburl + "/" + 'Artwork/' + resUpload.data[0].ARTWORK_REQUEST_ID;
    //            }
    //            myPleaseWait.hide();
    //        },
    //    });
    //});


    $(function () {
        $('.cls_form_igrid_upload_file').fileupload({
            sequentialUploads: true,
            dropZone: $('.cls_form_igrid_upload .dropzone'),
            dataType: 'json',
            url: suburl + '/fileUpload/upload_igrid_attachment',
            //uploadTemplateId: 'template-upload_file_igrid',
            //downloadTemplateId: 'template-download_file_igrid',
        });
    });



    $(document).on("click", ".cls_form_igrid_upload_file .cls_delete_file", function () {
        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["id"] = $(this).data("node-id");
        jsonObj.data = item;
        delete_node_id = $(this).data("node-id");
        var myurl = '/api/taskform/igrid/attachment/delete';
        var mytype = 'DELETE';
        var mydata = jsonObj;
        myAjaxConfirmDelete(myurl, mytype, mydata, callback_delete_file_complete);
    });




});


var delete_node_id;
function callback_delete_file_complete(res) {


    $('.cls_table_igrid_upload_file > tbody  > tr').each(function () {

        if ($(this).find('.cls_delete_file').data("node-id") == delete_node_id) {
           
            $(this).remove();
        }
    });
}

function bindDataAttachIGird() {
   
    $('.cls_form_igrid_upload_file').addClass('fileupload-processing');
    $('.cls_form_igrid_upload_file .files').empty();
    var myurl = '/api/taskform/igrid/attachment/info?data.matdoc=' + IGridSAPMaterialId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_attach_igrid);
}

function callback_get_attach_igrid(res) {
    if (res.data != null) {
        debugger;
      
        for (var i = 0; i < res.data.length; i++) {

            res.data[i]["thumbnailUrl"] = suburl + "/Content/Free-file-icons/32px/" + res.data[i].extension + ".png";
            res.data[i]["name"] = res.data[i].name;
            res.data[i]["size"] = res.data[i].size;
            res.data[i]["nodeid"] = res.data[i].id;
            res.data[i]["NODE_ID_TXT"] = res.data[i].id;
            res.data[i]["create_date_display_txt"] = res.data[i].id;
            
   
        }

        
        $(".cls_form_igrid_upload_file").each(function (index) {
            $(this).fileupload('option', 'done').call($(this), $.Event('done'), { result: { files: res.data } })
            $(this).removeClass('fileupload-processing');
        });
    }
}