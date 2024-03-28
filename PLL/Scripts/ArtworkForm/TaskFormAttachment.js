$(document).ready(function () {
    $('.cls_form_upload_file_artwork').bind('fileuploadsubmit', function (e, data) {
        var roleId = '';
        if (!isEmpty($(this).closest('.cls_task_form_attachment_artwork').find('.cls_go_to_step').val())) {
            roleId = getstepartwork($(this).closest('.cls_task_form_attachment_artwork').find('.cls_go_to_step').val()).curr_step;
        }
        var IS_VENDOR = '';
        var IS_INTERNAL = 'X';
        var IS_CUSTOMER = '';
        var go_to_step = $(this).closest('.cls_task_form_attachment_artwork').find('.cls_go_to_step').val();

        if (IsVendor == "0") {
            if (go_to_step == go_to_step == 'SEND_VN_PO' || go_to_step == 'SEND_VN_SL' || go_to_step == 'SEND_VN_PM') {
                IS_VENDOR = 'X'
            }
        }
        
        if (IsCustomer == "0") {
            if (go_to_step == 'SEND_CUS_REVIEW' || go_to_step == 'SEND_CUS_REQ_REF' || go_to_step == 'SEND_CUS_SHADE' || go_to_step == 'SEND_CUS_PRINT' || go_to_step == 'SEND_CUS_REF') {
                IS_CUSTOMER = 'X'
            }
        } else if (IsCustomer == "1")
        {
            //INC-109585 by aof in this else
            IS_CUSTOMER = "X";
        }

        //----------------- CR#19440 by aof --------------------------------------------
        if (IsVendor == "1")
        {
            if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PM")
            {
                IS_VENDOR = 'X';
                IS_CUSTOMER = '';
                IS_INTERNAL = '';
            
            }
        }
        //----------------- CR#19440 by aof --------------------------------------------

        data.formData = {
            artworkSubId: ArtworkSubId,
            userId: UserID,
            roldId: roleId,
            IS_VENDOR: IS_VENDOR,
            IS_INTERNAL: IS_INTERNAL,
            IS_CUSTOMER: IS_CUSTOMER
        };
    });

    $(function () {
        $('.cls_form_upload_file_artwork').fileupload({
            sequentialUploads: true,
            dropZone: $('.cls_form_upload_file_artwork .dropzone'),
            dataType: 'json',
            url: suburl + '/fileUpload/upload_artwork_attachment',
        });
    });
    $('#popup_attachment_artwork').on('shown.bs.modal', function (e) {
        $('.cls_go_to_step').val($(e.relatedTarget).data('go_to_step'));
        var roleId = getstepartwork($(e.relatedTarget).data('go_to_step')).curr_step;
        bindDataAttachArtwork(roleId);
        $('.cls_div_table_artwork').css('max-height', '300px').css('overflow-y', 'auto');
    })
    $('#popup_attachment_artwork').on('hidden.bs.modal', function (e) {
        $('.cls_go_to_step').val('');
        $('.cls_div_table_artwork').css('max-height', 'none').css('overflow-y', 'unset');
    })

    $(document).on("click", ".cls_form_upload_file_artwork .cls_delete_file_artwork", function () {
        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["NODE_ID"] = $(this).data("node-id");
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;
        jsonObj.data = item;
        delete_node_id = $(this).data("node-id");
        var myurl = '/api/taskform/attachment/artwork/delete';
        var mytype = 'DELETE';
        var mydata = jsonObj;
        myAjaxConfirmDelete(myurl, mytype, mydata, callback_delete_file_complete_artwork);
    });

    $(document).on("change", ".cls_chk_att_visibility_artwork", function () {
        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["NODE_ID"] = $(this).closest('tr').data("node-id");
        item["IS_CUSTOMER"] = $(this).closest('tr').find(".cls_chk_att_visibility_customer_artwork").is(":checked") ? "X" : "";
        item["IS_INTERNAL"] = $(this).closest('tr').find(".cls_chk_att_visibility_internal_artwork").is(":checked") ? "X" : "";
        item["IS_VENDOR"] = $(this).closest('tr').find(".cls_chk_att_visibility_vendor_artwork").is(":checked") ? "X" : "";
        jsonObj.data = item;
        var myurl = '/api/taskform/attachment/artwork/visibility';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjax(myurl, mytype, mydata);
    });
});

var delete_node_id;
function callback_delete_file_complete_artwork(res) {
    $('.cls_table_attachment_artwork > tbody  > tr').each(function () {
       
        if ($(this).find('.cls_delete_file_artwork').data("node-id") == delete_node_id) {
             // #INC-36800 by aof.
            if ($(this).find("td:eq(4)").text() == 'Develop print master for customer')
            {
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PM") {
                    if ($(".cls_fileupload_buttonbar_artwork").is(":hidden")) {
                        $('.cls_fileupload_buttonbar_artwork').show();
                    }
                }  
            }
             // #INC-36800 by aof
            $(this).remove();          
        }
    });
}

function bindDataAttachArtwork(roleId) {
    if (isEmpty(roleId)) roleId = '';

    $('.cls_form_upload_file_artwork').addClass('fileupload-processing');
    $('.cls_form_upload_file_artwork .files').empty();
    var myurl = '/api/taskform/attachment/artwork/info?data.artwork_sub_id=' + ArtworkSubId + '&data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.role_id=' + roleId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_attach_artwork);
}

function callback_get_attach_artwork(res) {
    if (res.data != null) {

        var f_hide_fileupload_buttonbar = false;   // #INC-36800 by aof.
        for (var i = 0; i < res.data.length; i++) {
            res.data[i]["thumbnailUrl"] = suburl + "/Content/Free-file-icons/32px/" + res.data[i].EXTENSION + ".png";
            res.data[i]["name"] = res.data[i].FILE_NAME;
            res.data[i]["size"] = res.data[i].SIZE;
            res.data[i]["nodeid"] = res.data[i].NODE_ID;
            res.data[i]["version2"] = res.data[i].VERSION2;
            res.data[i]["IS_CUSTOMER"] = res.data[i].IS_CUSTOMER;
            res.data[i]["IS_INTERNAL"] = res.data[i].IS_INTERNAL;
            res.data[i]["IS_VENDOR"] = res.data[i].IS_VENDOR;
            res.data[i]["IS_SYSTEM"] = (res.data[i].IS_SYSTEM == null ? "" : res.data[i].IS_SYSTEM);
            res.data[i]["NODE_ID_TXT"] = res.data[i].NODE_ID_TXT;

             // #INC-36800 by aof start code.
            res.data[i]["step_code"] = res.data[i].step_code;
            if (CURRENT_STEP_CODE_DISPLAY_TXT == res.data[i].step_code)
            {
                if (CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PM")
                {
                    //if (res.data[i].IS_VENDOR == "X")
                    //{
                    f_hide_fileupload_buttonbar = true;
                //}
                }
           
            }
             // #INC-36800 by aof start end.
        }

        // #INC-36800 by aof start.
        if (f_hide_fileupload_buttonbar)
        {
            $('.cls_fileupload_buttonbar_artwork').hide();
        }
        else
        {
            if (ReadOnly == "0")
            {
                $('.cls_fileupload_buttonbar_artwork').show();
            } 
        }    
              // $('.cls_fileupload_buttonbar_artwork').hide();
         // #INC-36800 by aof end.
 
        $(".cls_form_upload_file_artwork").each(function (index) {
            $(this).fileupload('option', 'done').call($(this), $.Event('done'), { result: { files: res.data } })
            $(this).removeClass('fileupload-processing');
        });
    }
}
