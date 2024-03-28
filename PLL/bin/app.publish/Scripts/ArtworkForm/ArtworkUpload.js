var DefaultRecipientData = [];
$(document).ready(function () {
    //$(".cls_div_uploadartwork").find("*").prop("disabled", true);
    $('.cls_form_artwork_upload_file').bind('fileuploadsubmit', function (e, data) {
        data.formData = {
            userId: UserID,
            requestFormId: $('.cls_hid_artwork_request_form_id').val()
        };
    });

    $(function () {
        $('.cls_form_artwork_upload_file').fileupload({
            sequentialUploads: true,
            dataType: 'json',
            url: suburl + '/fileUpload/upload_artwork',
            uploadTemplateId: 'template-upload_file_artwork',
            downloadTemplateId: 'template-download_file_artwork',
            start: function (e) {
                myPleaseWait.show();
            },
            stop: function (e) {
                if (typeSend == 'cus') {
                    alertSuccessDialog(resUpload.msg);
                }
                else if (typeSend == 'mk') {
                    document.location.href = suburl + "/" + 'Artwork/' + resUpload.data[0].ARTWORK_REQUEST_ID;
                }
                myPleaseWait.hide();
            },
        });
    });

    $(document).on("click", ".cls_form_artwork_upload_file .cls_delete_file", function () {
        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["REQUEST_FORM_FILE_NODE_ID"] = $(this).data("node-id");
        jsonObj.data = item;
        delete_node_id = $(this).data("node-id");
        var myurl = '/api/artwork/deleteuploadfile';
        var mytype = 'DELETE';
        var mydata = jsonObj;
        myAjaxConfirmDelete(myurl, mytype, mydata, callback_delete_file_complete);
    });

    var cntRecipient = 0;
    $(".cls_form_artwork_upload_file .cls_btn_add_recipient").click(function () {

        var obj = $('.cls_form_artwork_upload_file .table_recipient .tr_recipient').clone().removeClass('tr_recipient').removeClass('cls_cn_hide');

        cntRecipient++;
        $('.cls_form_artwork_upload_file .table_recipient tbody').append(obj);
        var obj2 = $('.cls_form_artwork_upload_file .table_recipient tbody tr:last');

        var ddl = 'cls_lov_recipient';
        obj2.find('.' + ddl).toggleClass(ddl + ' ' + ddl + cntRecipient);
        bind_lov_param('.' + ddl + cntRecipient, '/api/lov/recipient', 'data.DISPLAY_TXT', ["IS_HF", "IS_PF"], ['.cls_chk_type_of_product_hf', '.cls_chk_type_of_product_pf']);
        $('.' + ddl + cntRecipient).attr("required", true);
        //if (cntRecipient > 1) {
        //    obj.find('.cls_img_delete_recipient').show();
        //}
        $('.' + ddl + cntRecipient).on("change", function () {
            var selections = $(this).select2('data');
            var isClear = true;
            if (selections.length > 0) {
                //input = 'cls_txt_email';
                //if (selections[0].EMAIL == undefined) {
                //    obj.find('.' + input).val(DefaultRecipientData[selections[0].id].EMAIL);
                //} else {
                //    obj.find('.' + input).val(selections[0].EMAIL);
                //}

                input = 'cls_txt_position';
                if (selections[0].POSITION_DISPLAY_TXT == undefined) {
                    obj.find('.' + input).val(DefaultRecipientData[selections[0].id].POSITION_DISPLAY_TXT);
                } else {
                    obj.find('.' + input).val(selections[0].POSITION_DISPLAY_TXT);
                }
            }
            else {
                //input = 'cls_txt_email';
                //obj.find('.' + input).val('');

                input = 'cls_txt_position';
                obj.find('.' + input).val('');
            }
        });

        $(".cls_form_artwork_upload_file .table_recipient .cls_img_delete_recipient").click(function () {
            $(this).closest('tr').remove();
        });
    });
    $(".cls_form_artwork_upload_file .cls_btn_add_recipient").click();

    $(".cls_form_artwork_upload_file .cls_btn_save").click(function (e) {
        saveDataUploadArtwork(true);
    });

    $('.cls_form_artwork_upload_file .cls_btn_cancel').click(function () {
        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["ARTWORK_REQUEST_ID"] = $('.cls_hid_artwork_request_form_id').val();
        jsonObj.data = item;

        var myurl = '/api/artwork/deleteuploadform';
        var mytype = 'DELETE';
        var mydata = jsonObj;
        myAjaxConfirmDelete(myurl, mytype, mydata, tohomepage);
    })

    $(".cls_form_artwork_upload_file form").submit(function (e) {
        if ($(this).valid()) {
            saveDataUploadArtwork(false);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    if (ARTWORK_REQUEST_ID == 0) {
        if (UserPosition == 'PK' || UserPosition == 'MK' || UserPosition == 'T-HOLDING' || UserPosition == 'MK_SUPPORT' || UserPosition == 'FFC') {
            //If marketing or PA or FFC or T-Holding upload artwork file by himself, recipient and email address and position will be defaulted person who upload artwork file.
            var data = [{
                RECIPIENT_USER_ID: UserID,
                RECIPIENT_USER_DISPLAY_TXT: UserNameDisplay + ' (' + UserEmail + ')',
                RECIPIENT_EMAIL: UserEmail,
                RECIPIENT_POSITION_DISPLAY_TXT: UserPositionName
            }];

            bindDataArtworkUploadRecipient(data);
        }
    }
    else {
        bindDataUploadArtwork(ARTWORK_REQUEST_ID);
    }
});

function bindDataUploadArtwork(ArtworkRequestId) {
    var myurl = '/api/artwork/upload?data.artwork_request_id=' + ArtworkRequestId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_get_uploadArtwork);
}

function callback_get_uploadArtwork(res) {
    if (res.data.length > 0 && res.data[0].ARTWORK_REQUEST_NO == null) {
        $('.cls_hid_artwork_request_form_id').val(res.data[0].ARTWORK_REQUEST_ID);
        $('.cls_form_artwork_upload_file .cls_btn_submit').show();
        $('.cls_form_artwork_upload_file .cls_btn_cancel').show();
        $(".cls_div_uploadartwork").find("*").prop("disabled", false);

        for (var i = 0; i < res.data[0].REQUEST_ITEMS.length; i++) {
            res.data[0].REQUEST_ITEMS[i]["thumbnailUrl"] = suburl + "/Content/Free-file-icons/32px/" + res.data[0].REQUEST_ITEMS[i].EXTENSION + ".png";
            res.data[0].REQUEST_ITEMS[i]["name"] = res.data[0].REQUEST_ITEMS[i].FILE_NAME;
            res.data[0].REQUEST_ITEMS[i]["size"] = res.data[0].REQUEST_ITEMS[i].FILE_SIZE;
            res.data[0].REQUEST_ITEMS[i]["nodeid"] = res.data[0].REQUEST_ITEMS[i].NODE_ID;
            res.data[0].REQUEST_ITEMS[i]["canDelete"] = true;
            res.data[0].REQUEST_ITEMS[i]["create_date_display_txt"] = res.data[0].REQUEST_ITEMS[i].CREATE_DATE;
            res.data[0].REQUEST_ITEMS[i]["create_by_display_txt"] = res.data[0].REQUEST_ITEMS[i].CREATE_BY_DISPLAY_TXT;
        }
        $(".cls_form_artwork_upload_file").each(function (index) {
            $(this).fileupload('option', 'done').call($(this), $.Event('done'), { result: { files: res.data[0].REQUEST_ITEMS } })
            $(this).removeClass('fileupload-processing');
        });

        bindDataArtworkUploadRecipient(res.data[0].REQUEST_RECIPIENT);
    }
    else {
        tohomepage();
    }
}

function bindDataArtworkUploadRecipient(data) {
    var recipientDataitem = {};
    //$('.cls_form_artwork_upload_file .table_recipient tbody tr:visible').remove();
    $.each(data, function (index, item) {
        if (index > 0) {
            $(".cls_form_artwork_upload_file .cls_btn_add_recipient").click();
        }
        var obj2 = $('.cls_form_artwork_upload_file .table_recipient tbody tr:last');
        //obj2.find('.cls_lov_recipient_static').select2("data", { id: item.RECIPIENT_USER_ID, text: item.RECIPIENT_USER_DISPLAY_TXT, EMAIL: item.RECIPIENT_EMAIL, POSITION_DISPLAY_TXT: item.RECIPIENT_POSITION_DISPLAY_TXT });
        //obj2.find('.cls_lov_recipient_static').val(item.RECIPIENT_USER_ID).trigger('change.select2');
        setValueToDDL(obj2.find('.cls_lov_recipient_static'), item.RECIPIENT_USER_ID, item.RECIPIENT_USER_DISPLAY_TXT);
        //obj2.find('.cls_txt_email').val(item.RECIPIENT_EMAIL);
        obj2.find('.cls_txt_position').val(item.RECIPIENT_POSITION_DISPLAY_TXT);

        recipientDataitem[item.RECIPIENT_USER_ID] = {
            NAME_EMAIL_DISPLAY_TXT: item.NAME_EMAIL_DISPLAY_TXT,
            EMAIL: item.RECIPIENT_EMAIL,
            POSITION_DISPLAY_TXT: item.RECIPIENT_POSITION_DISPLAY_TXT
        };
    });
    DefaultRecipientData = recipientDataitem;
}

var delete_node_id;
function callback_delete_file_complete(res) {
    $('.cls_table_artwork_upload_file > tbody  > tr').each(function () {
        if ($(this).find('.cls_delete_file').data("node-id") == delete_node_id) {
            $(this).remove();
        }
    });
}

function saveDataUploadArtwork(only_save) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};

    if ($('.cls_hid_artwork_request_form_id').val() != '') {
        item["ARTWORK_REQUEST_ID"] = $('.cls_hid_artwork_request_form_id').val();
    } else {
        item["CREATE_BY"] = UserID;
        item["UPLOAD_BY"] = UserID;
    }
    item["UPDATE_BY"] = UserID;

    var isValidRecipient = false;
    var isValidFile = false;
    var first_row = true;
    var recipientsList = [];
    var arrRecipient = [];
    $(".cls_form_artwork_upload_file .table_recipient tbody tr").each(function (index) {
        if (first_row) first_row = false;
        else if ($(this).find('.cls_lov_recipient_static').val() != null) {
            recipientsItem = {};
            recipientsItem["RECIPIENT_USER_ID"] = $(this).find('.cls_lov_recipient_static').val();
            recipientsItem["CREATE_BY"] = UserID;
            recipientsItem["UPDATE_BY"] = UserID;
            recipientsList.push(recipientsItem);
            arrRecipient.push($(this).find('.cls_lov_recipient_static').val());
            isValidRecipient = true;
        }
    });

    if ($(".cls_form_artwork_upload_file .cls_table_artwork_upload_file tbody tr").length > 0) {
        isValidFile = true;
    }

    if (find_duplicates(arrRecipient).length == 0) {
        jsonObj.data = item;
        jsonObj.data.REQUEST_RECIPIENT = recipientsList;
        typeSend = '';
        if (only_save) {
            if (isValidRecipient || isValidFile) {
                var myurl = '/api/artwork/upload';
                var mytype = 'POST';
                var mydata = jsonObj;

                myAjax(myurl, mytype, mydata, callbackSaveDataUploadArtwork);
            }
            else {
                alertError2("Please select recipient or add file at least 1 item");
            }
        }
        else {
            if (isValidRecipient && isValidFile) {
                var myurl = '/api/artwork/submitupload';
                var mytype = 'POST';
                var mydata = jsonObj;

                if (UserPosition == 'PK' || UserPosition == 'MK' || UserPosition == 'T-HOLDING' || UserPosition == 'MK_SUPPORT' || UserPosition == 'FFC') {
                    typeSend = 'mk';
                    myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSubmitDataUploadArtwork);
                }
                else {
                    typeSend = 'cus';
                    myAjaxConfirmSubmitAlert(myurl, mytype, mydata, callbackSubmitDataUploadArtwork, '', true, false);
                }
            }
            else {
                alertError2("Please select recipient and add file at least 1 item");
            }
        }
    }
    else {
        alertError2("Please select a unique recipient");
    }
}

function callbackSaveDataUploadArtwork(res) {
    $('.cls_hid_artwork_request_form_id').val(res.data[0].ARTWORK_REQUEST_ID);
    $('.cls_form_artwork_upload_file .cls_btn_cancel').show();
    if ($(".cls_form_artwork_upload_file .cls_table_artwork_upload_file tbody tr").length > 0) {
        $(".cls_form_artwork_upload .start").click();

    }
}

var typeSend = '';
var resUpload;
function callbackSubmitDataUploadArtwork(res) {
    resUpload = res;
    $('.cls_hid_artwork_request_form_id').val(res.data[0].ARTWORK_REQUEST_ID);
    if ($(".cls_form_artwork_upload_file .cls_table_artwork_upload_file tbody tr").length > 0) {
        $(".cls_form_artwork_upload .start").click();
    }

    if ($('.btn.btn-warning.cancel:visible').length == 0) {
        if (typeSend == 'cus') {
            alertSuccessDialog(resUpload.msg);
        }
        else if (typeSend == 'mk') {
            document.location.href = suburl + "/" + 'Artwork/' + resUpload.data[0].ARTWORK_REQUEST_ID;
        }
    }
}
