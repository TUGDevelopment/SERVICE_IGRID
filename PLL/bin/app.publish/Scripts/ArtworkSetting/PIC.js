var upload_action = '';
var cnt_lov_tfartwork_sold_to_multiple = 1;
var cnt_lov_tfartwork_ship_to_multiple = 1;
var cnt_lov_tfartwork_zone_multiple = 1;
var cnt_lov_tfartwork_country_multiple = 1;
var cnt_lov_tfartwork_person_multiple = 1;
var resUpload;

$(document).ready(function () {
    //Sold To
    $(document).on("click", ".cls_img_lov_tfartwork_pic_add_sold_to_multiple", function () {
        var obj = $('.tr_tfartwork_pic_sold_to_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pic_sold_to_multiple');
        obj.find('.cls_lov_pic_sold_to_multiple').toggleClass('cls_lov_pic_sold_to_multiple cls_lov_pic_sold_to_multiple' + cnt_lov_tfartwork_sold_to_multiple);
        obj.find('.cls_input_pic_sold_to_multiple').toggleClass('cls_input_pic_sold_to_multiple cls_input_pic_sold_to_multiple' + cnt_lov_tfartwork_sold_to_multiple);
        obj.insertAfter($('.tr_tfartwork_pic_sold_to_multiple_static:last'));
        bind_lov_nonOther('.cls_lov_pic_sold_to_multiple' + cnt_lov_tfartwork_sold_to_multiple, '/api/lov/customer', 'data.DISPLAY_TXT', '.cls_input_pic_sold_to_multiple' + cnt_lov_tfartwork_sold_to_multiple);

        if (cnt_lov_tfartwork_sold_to_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pic_add_sold_to_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_sold_to_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_sold_to_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_sold_to_multiple++;
    });
    $(".cls_img_lov_tfartwork_pic_add_sold_to_multiple").click();

    //Ship To
    $(document).on("click", ".cls_img_lov_tfartwork_pic_add_ship_to_multiple", function () {
        var obj = $('.tr_tfartwork_pic_ship_to_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pic_ship_to_multiple');
        obj.find('.cls_lov_pic_ship_to_multiple').toggleClass('cls_lov_pic_ship_to_multiple cls_lov_pic_ship_to_multiple' + cnt_lov_tfartwork_ship_to_multiple);
        obj.find('.cls_input_pic_ship_to_multiple').toggleClass('cls_input_pic_ship_to_multiple cls_input_pic_ship_to_multiple' + cnt_lov_tfartwork_ship_to_multiple);
        obj.insertAfter($('.tr_tfartwork_pic_ship_to_multiple_static:last'));
        bind_lov_nonOther('.cls_lov_pic_ship_to_multiple' + cnt_lov_tfartwork_ship_to_multiple, '/api/lov/customer', 'data.DISPLAY_TXT', '.cls_input_pic_ship_to_multiple' + cnt_lov_tfartwork_ship_to_multiple);

        if (cnt_lov_tfartwork_ship_to_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pic_add_ship_to_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_ship_to_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_ship_to_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_ship_to_multiple++;
    });
    $(".cls_img_lov_tfartwork_pic_add_ship_to_multiple").click();

    //Zone
    $(document).on("click", ".cls_img_lov_tfartwork_pic_add_zone_multiple", function () {
        var obj = $('.tr_tfartwork_pic_zone_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pic_zone_multiple');
        obj.find('.cls_lov_pic_zone_multiple').toggleClass('cls_lov_pic_zone_multiple cls_lov_pic_zone_multiple' + cnt_lov_tfartwork_zone_multiple);
        obj.find('.cls_input_pic_zone_multiple').toggleClass('cls_input_pic_zone_multiple cls_input_pic_zone_multiple' + cnt_lov_tfartwork_zone_multiple);
        obj.insertAfter($('.tr_tfartwork_pic_zone_multiple_static:last'));
        bind_lov_nonOther('.cls_lov_pic_zone_multiple' + cnt_lov_tfartwork_zone_multiple, '/api/lov/zone', 'data.DISPLAY_TXT', '.cls_input_pic_zone_multiple' + cnt_lov_tfartwork_zone_multiple);

        if (cnt_lov_tfartwork_zone_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pic_add_zone_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_zone_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_zone_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_zone_multiple++;
    });
    $(".cls_img_lov_tfartwork_pic_add_zone_multiple").click();

    //Country
    $(document).on("click", ".cls_img_lov_tfartwork_pic_add_country_multiple", function () {
        var obj = $('.tr_tfartwork_pic_country_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pic_country_multiple');
        obj.find('.cls_lov_pic_country_multiple').toggleClass('cls_lov_pic_country_multiple cls_lov_pic_country_multiple' + cnt_lov_tfartwork_country_multiple);
        obj.find('.cls_input_pic_country_multiple').toggleClass('cls_input_pic_zone_multiple cls_input_pic_country_multiple' + cnt_lov_tfartwork_country_multiple);
        obj.insertAfter($('.tr_tfartwork_pic_country_multiple_static:last'));

        //bind_lov_nonOther('.cls_lov_pic_country_multiple' + cnt_lov_tfartwork_country_multiple, 'api/lov/pic/country', 'data.DISPLAY_TXT', '.cls_input_pic_country_multiple' + cnt_lov_tfartwork_country_multiple);

        bind_lov_param_filter_list('.cls_lov_pic_country_multiple' + cnt_lov_tfartwork_country_multiple,
            'api/lov/pic/country',
            'data.DISPLAY_TXT',
            ["ZONE"],
            [".cls_lov_pic_zone_multiple1", ".cls_lov_pic_zone_multiple2", ".cls_lov_pic_zone_multiple3", ".cls_lov_pic_zone_multiple4", ".cls_lov_pic_zone_multiple5", ".cls_lov_pic_zone_multiple6", ".cls_lov_pic_zone_multiple7", ".cls_lov_pic_zone_multiple8", ".cls_lov_pic_zone_multiple9", ".cls_lov_pic_zone_multiple10"],
            "FILTER_ID",
            ["cls_lov_pic_country_multiple_static"]);

  
        if (cnt_lov_tfartwork_country_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pic_add_country_multiple').remove();   
            obj.find('.cls_img_lov_tfartwork_pa_delete_country_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_country_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_country_multiple++;
    });
    $(".cls_img_lov_tfartwork_pic_add_country_multiple").click();

    //Person
    $(document).on("click", ".cls_img_lov_tfartwork_pic_add_person_multiple", function () {
        var obj = $('.tr_tfartwork_pic_person_multiple').clone();
        obj.show();
        obj.removeClass('tr_tfartwork_pic_person_multiple');
        obj.find('.cls_lov_pic_person_multiple').toggleClass('cls_lov_pic_person_multiple cls_lov_pic_person_multiple' + cnt_lov_tfartwork_person_multiple);
        obj.find('.cls_input_pic_person_multiple').toggleClass('cls_input_pic_person_multiple cls_input_pic_person_multiple' + cnt_lov_tfartwork_person_multiple);
        obj.insertAfter($('.tr_tfartwork_pic_person_multiple_static:last'));
        bind_lov_nonOther('.cls_lov_pic_person_multiple' + cnt_lov_tfartwork_person_multiple, '/api/lov/userpic', 'data.DISPLAY_TXT', '.cls_input_pic_person_multiple' + cnt_lov_tfartwork_person_multiple);

        if (cnt_lov_tfartwork_person_multiple > 1) {
            obj.find('.cls_img_lov_tfartwork_pic_add_person_multiple').remove();
            obj.find('.cls_img_lov_tfartwork_pa_delete_person_multiple').show();
        }
        $(obj).find(".cls_img_lov_tfartwork_pa_delete_person_multiple").click(function () {
            $(this).closest('.row').remove();
        });
        cnt_lov_tfartwork_person_multiple++;
    });
    $(".cls_img_lov_tfartwork_pic_add_person_multiple").click();



    bind_lov_nonOther('.cls_lov_pic_edit_sold_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov_nonOther('.cls_lov_pic_edit_ship_to', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov_nonOther('.cls_lov_pic_edit_zone', '/api/lov/zone', 'data.DISPLAY_TXT');
    //bind_lov_nonOther('.cls_lov_pic_edit_country', 'api/lov/pic/country', 'data.DISPLAY_TXT');
    bind_lov_nonOther('.cls_lov_pic_edit_user', '/api/lov/userpic', 'data.DISPLAY_TXT');

    bind_lov_param('.cls_lov_pic_edit_country', 'api/lov/pic/country', 'data.DISPLAY_TXT', ["ZONE"], ['.cls_lov_pic_edit_zone']);

    $('.cls_table_pic_data thead tr').clone(true).appendTo('.cls_table_pic_data thead');
    $('.cls_table_pic_data thead tr:eq(1) th').each(function (i) {
        if (i == 0 || i == 1 || i == 12 || i == 13) {
            $(this).html('');
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    $('#table_pic_data').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        "scrollX": true
    });

    bind_table_pic_data();

    $(".cls_pic_data form").submit(function (e) {

        //bind_table_pic_data();
        table_pic_data.ajax.reload();
        e.preventDefault();
    });


    $('body').on('change', '#pic_select_all', function () {
        if (this.checked)
            $('.cls_chk_pic').prop('checked', true);
        else
            $('.cls_chk_pic').prop('checked', false);

        $('.cls_chk_pic').change();
    });


    $('.cls_form_pic_upload_file').bind('fileuploadsubmit', function (e, data) {
        data.formData = {
            userId: UserID,
            action: upload_action
        };
    });

   
    $(function () {
        $('.cls_form_pic_upload_file').fileupload({
            sequentialUploads: true,
            dataType: 'json',
            url: suburl + '/fileUpload/Upload_PIC',
            uploadTemplateId: 'template-upload_file_pic',
            downloadTemplateId: 'template-download_file_pic',
            start: function (e) { 
                myPleaseWait.show();
            },
            stop: function (e) {

                myPleaseWait.hide();
                // $(".cls_upload_close").click();
            },

        });
    });

    $(".dataTables_filter").css("display", "none");  // hiding global search box

    $(".cls_btn_pic_delete").click(function () {
        if ($('.cls_chk_pic:checked').length > 0) {
            deletePICList();
        }
        else {
            alertError2("Please select PIC at least 1 item.");
        }
    });
});



$(".cls_pic_data .cls_btn_clear").click(function () {
    $('.cls_pic_data input[type=text]').val('');
    $('.cls_pic_data input[type=checkbox]').prop('checked', false);
    $('.cls_pic_data textarea').val('');
    $(".cls_pic_data .cls_lov_sales_organization").val('').trigger("change");
    $(".cls_pic_data .cls_lov_ship_to").val('').trigger("change");
    $(".cls_pic_data .cls_lov_brand").val('').trigger("change");
    $(".cls_pic_data .cls_lov_purchasing_organization").val('').trigger("change");
    $(".cls_pic_data .cls_lov_packaging_material").val('').trigger("change");
    $(".cls_pic_data .cls_lov_company").val('').trigger("change");
});

$(".dataTables_filter").css("display", "none");  // hiding global search box

$(".cls_btn_pic_export").click(function () {
    var param = getParamPIC('X');
    window.open("/excel/PICMaster" + param
        , '_blank');
});


$('#modal_pic_edit').on('shown.bs.modal', function (e) {
    upload_action = 'EDIT';
   
    $('.cls_pic_id_edit').val($(e.relatedTarget).data('pic_id'));

    removeAlert();

    bindDataPICEdit($('.cls_pic_id_edit').val());
    //bindReportWarehouse_SOAtt($('.cls_pic_id_edit').val());
});

$('#modal_pic_edit').on('hidden.bs.modal', function (e) {
    $('.cls_pic_id_edit').val('');
    upload_action = '';
  
    removeAlert();
});


$(".cls_btn_pic_edit_save").click(function (e) {

    removeAlert();

    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    var msg = "";

    item['PIC_ID'] = $('.cls_pic_id_edit').val();
    item['SOLD_TO_ID'] = $('.cls_lov_pic_edit_sold_to').val();
    item['SHIP_TO_ID'] = $('.cls_lov_pic_edit_ship_to').val();
    item['ZONE_ID'] = $('.cls_lov_pic_edit_zone').val();
    item['COUNTRY_ID'] = $('.cls_lov_pic_edit_country').val();
    item['USER_ID'] = $('.cls_lov_pic_edit_user').val();
    item["UPDATE_BY"] = UserID;

    if (item['SOLD_TO_ID'] == null) {
        msg += "Sold-to, ";
    }

    if (item['SHIP_TO_ID'] == null) {
        msg += "Ship-to, ";
    }

    if (item['ZONE_ID'] == null) {
        msg += "Zone, ";
    }

    if (item['USER_ID'] == null) {
        msg += "Person in charge, ";
    }

    if (msg != "") {
        msg = msg.substring(0, msg.length - 2);
        msg = msg + " is empty.";

        $("#response").animate({
            height: '+=72px'
        }, 300);

        $('<div class="alert alert-danger" id="myAlert">' +
            '<button type="button" class="close" data-dismiss="alert">' +
            '&times;</button>' + msg + '</div>').hide().appendTo('#response').fadeIn(1000);

        //alert(msg);
    } else {

        savePICEdit();
    }
});


$("#alert").on('closed.bs.alert', function () {
    removeAlert();
});


$(".cls_btn_pic_edit_delete").click(function (e) {
    deletePICEdit();
});

$('.cls_btn_pic_upload_edit').click(function (e) {
    upload_action = 'EDIT'; 
    $('.modal-title').text("Upload file for edit PIC");

    $('#modal_pic_upload_edit').modal({
        backdrop: 'static',
        keyboard: true,
    });

    $('#modal_pic_upload_edit').on('hidden.bs.modal', function (e) {

        $('.cls_table_pic_upload_file > tbody  > tr').each(function () {
            $(this).remove();
        });
    });

    $('.fileinput-button').click(function (e) {
        $('.cls_table_pic_upload_file > tbody  > tr').each(function () {
            $(this).remove();
        });
    });

});

$('.cls_btn_pic_upload_add').click(function (e) {
    upload_action = 'ADD'; 
    $('.modal-title').text("Upload file for add PIC");
    
    $('#modal_pic_upload_edit').modal({
        backdrop: 'static',
        keyboard: true,
    });

    $('#modal_pic_upload_edit').on('hidden.bs.modal', function (e) {

        $('.cls_table_pic_upload_file > tbody  > tr').each(function () {
            $(this).remove();
        });
    });

    $('.fileinput-button').click(function (e) {
        $('.cls_table_pic_upload_file > tbody  > tr').each(function () {
            $(this).remove();
        });
    });

});

$(".cls_btn_pic_clear").click(function (e) {
    clearSearch();
});


var table_pic_data;
function bind_table_pic_data() {

    table_pic_data = $('#table_pic_data').DataTable();
    table_pic_data.destroy();
    table_pic_data = $('#table_pic_data').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        //fixedColumns: {
        //    leftColumns: 3
        //},

        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/master/pic" + getParamPIC(''),
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
                "targets": 13
            },
        ],
        "order": [[2, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_chk_pic" data-pic_id="' + row.PIC_ID + '" type="checkbox">';
                }
            },
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "USER_DISPLAY_TXT", "className": "cls_nowrap cls_td_width_75" },
            { "data": "FIRST_NAME_DISPLAY_TXT", "className": "cls_nowrap cls_td_width_150" },
            { "data": "LAST_NAME_DISPLAY_TXT", "className": "cls_nowrap cls_td_width_150" },
            { "data": "SOLD_TO_CODE", "className": "cls_nowrap cls_td_width_110" },
            { "data": "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap cls_td_width_150" },
            { "data": "SHIP_TO_CODE", "className": "cls_nowrap cls_td_width_110" },
            { "data": "SHIP_TO_DISPLAY_TXT", "className": "cls_nowrap cls_td_width_150" },
            { "data": "ZONE", "className": "cls_nowrap cls_td_width_75" },
            { "data": "COUNTRY", "className": "cls_nowrap cls_td_width_75" },
            { "data": "COUNTRY_DISPLAY_TXT", "className": "cls_nowrap cls_td_width_150" },
            { "data": "PIC_ID", "className": "cls_nowrap cls_hide" },
            //{ "data": "Edit", "className": "cls_item_edit cls_nowrap" },
            {
                render: function (data, type, row, meta) {
                    if (row.PIC_ID != null) {
                        return '<a data-toggle="modal" data-pic_id="' + row.PIC_ID + '"   href="#modal_pic_edit">Edit</a>&nbsp&nbsp|&nbsp&nbsp<a class="cls_delete_item"   style="color:red; cursor: pointer;" onClick = "deletePICRecord(' + row.PIC_ID + ');">Delete</a>';
                    }
                }
            },
        ],
        "rowCallback": function (row, data, index) {
            //$(row).find('.cls_item_edit').html('<a target="_blank" href="" style="text-decoration: underline;">Edit</a>');
            $("#pic_select_all").prop('checked', false);
        },
       
    });


    // $("#table_pic_data_filter").hide();

    $(table_pic_data.table().container()).on('keyup', 'input', function () {
        table_pic_data
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });


 
}

function getParamPIC(exportExcel) {

    var soldtolist = "";
    var shiptolist = "";
    var zonelist = "";
    var countrylist = "";
    var personlist = "";
    var first = true;
    $(".tr_tfartwork_pic_sold_to_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pic_sold_to_multiple_static').val())) {
                soldtolist += $(this).find('.cls_lov_pic_sold_to_multiple_static').val() + ",";
            }
        }
    });

    if (soldtolist != "") {
        soldtolist = soldtolist.substring(0, soldtolist.length - 1);
    }

    first = true;
    $(".tr_tfartwork_pic_ship_to_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pic_ship_to_multiple_static').val())) {
                shiptolist += $(this).find('.cls_lov_pic_ship_to_multiple_static').val() + ",";
            }
        }
    });

    if (shiptolist != "") {
        shiptolist = shiptolist.substring(0, shiptolist.length - 1);
    }

    first = true;
    $(".tr_tfartwork_pic_zone_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pic_zone_multiple_static').val())) {
                //zonelist += $(this).find('.cls_lov_pic_zone_multiple_static').val() + ",";
                zonelist += $(this).find('.cls_lov_pic_zone_multiple_static option:selected').text() + ",";
            }
        }
    });

    if (zonelist != "") {
        zonelist = zonelist.substring(0, zonelist.length - 1);
    }

    first = true;
    $(".tr_tfartwork_pic_country_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pic_country_multiple_static').val())) {
                countrylist += $(this).find('.cls_lov_pic_country_multiple_static').val() + ",";
            }
        }
    });

    if (countrylist != "") {
        countrylist = countrylist.substring(0, countrylist.length - 1);
    }

    first = true;
    $(".tr_tfartwork_pic_person_multiple_static").each(function (index) {
        if (first) first = false;
        else {
            if (!isEmpty($(this).find('.cls_lov_pic_person_multiple_static').val())) {
                personlist += $(this).find('.cls_lov_pic_person_multiple_static').val() + ",";
            }
        }
    });

    if (personlist != "") {
        personlist = personlist.substring(0, personlist.length - 1);
    }

    return "?data.LIST_SOLD_TO=" + soldtolist
        + "&data.LIST_SHIP_TO=" + shiptolist
        + "&data.LIST_ZONE=" + zonelist
        + "&data.LIST_COUNTRY=" + countrylist
        + "&data.LIST_PERSON=" + personlist
        + "&data.EXPORT_EXCEL=" + exportExcel

}

function bindDataPICEdit(pic_id) {

    var myurl = 'api/master/pic/edit?data.pic_id=' + pic_id;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_DataPICEdit);
}

function callback_bind_DataPICEdit(res) {

    if (res.data.length > 0) {
        var item = res.data[0];
        if (item != null) {

            setValueToDDL('.cls_lov_pic_edit_sold_to', item.SOLD_TO_ID, item.SOLD_TO_DISPLAY_TXT);
            setValueToDDL('.cls_lov_pic_edit_ship_to', item.SHIP_TO_ID, item.SHIP_TO_DISPLAY_TXT);
            setValueToDDL('.cls_lov_pic_edit_zone', item.ZONE_ID, item.ZONE);
            setValueToDDL('.cls_lov_pic_edit_country', item.COUNTRY_ID, item.COUNTRY_DISPLAY_TXT);
            setValueToDDL('.cls_lov_pic_edit_user', item.USER_ID, item.USER_DISPLAY_TXT);
        }
    }
}

function savePICEdit() {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    var msg = "";

    item['PIC_ID'] = $('.cls_pic_id_edit').val();
    item['SOLD_TO_ID'] = $('.cls_lov_pic_edit_sold_to').val();
    item['SHIP_TO_ID'] = $('.cls_lov_pic_edit_ship_to').val();
    item['ZONE_ID'] = $('.cls_lov_pic_edit_zone').val();
    item['COUNTRY_ID'] = $('.cls_lov_pic_edit_country').val();
    item['USER_ID'] = $('.cls_lov_pic_edit_user').val();
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;

    var myurl = 'api/master/pic/edit';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjax(myurl, mytype, mydata, callbackSavePICEdit, '', true, false);



}

function callbackSavePICEdit(res) {
    table_pic_data.ajax.reload();
    // bind_table_pic_data();
    $('#modal_pic_edit').modal('hide');
}

function deletePICList() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    $(".cls_chk_pic:checked").each(function () {
        var item = {};

        item["PIC_ID"] = $(this).data("pic_id");
        //item["UPDATE_BY"] = UserID;
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = 'api/master/pic/deletelist';
    var mytype = 'DELETE';
    var mydata = jsonObj;

    myAjaxConfirmDeleteWithContent(myurl, mytype, mydata, callbackDeletePICEdit, 'Do you want to delete selected item?');
    //myAjaxConfirmSubmit(myurl, mytype, mydata, reload_table_unlock, "", true, true);
}

function deletePICEdit() {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item['PIC_ID'] = $('.cls_pic_id_edit').val();

    jsonObj.data = item;

    var myurl = 'api/master/pic/delete';
    var mytype = 'DELETE';
    var mydata = jsonObj;
    myAjaxConfirmDeleteWithContent(myurl, mytype, mydata, callbackDeletePICEdit, "Do you want to delete this item?");

}

function deletePICRecord(pic_id) {
    deletePICItem(pic_id);
}

function deletePICItem(pic_id) {
    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};
    item['PIC_ID'] = pic_id;

    jsonObj.data = item;

    var myurl = 'api/master/pic/delete';
    var mytype = 'DELETE';
    var mydata = jsonObj;
    myAjaxConfirmDeleteWithContent(myurl, mytype, mydata, callbackDeletePICEdit, "Do you want to delete this item?");

}



function callbackDeletePICEdit(res) {
    table_pic_data.ajax.reload();
}

function removeAlert() {
    $(".alert").fadeOut(
        "normal",
        function () {
            $(this).remove();
        });

    $("#response").animate({
        height: '0px'
    }, 300);
}

function clearSearch() {
    //

    $(".tr_tfartwork_pic_sold_to_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_lov_pic_sold_to_multiple_static").val('').trigger("change");

    $(".tr_tfartwork_pic_ship_to_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_lov_pic_ship_to_multiple_static").val('').trigger("change");

    $(".tr_tfartwork_pic_zone_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_lov_pic_zone_multiple_static").val('').trigger("change");

    $(".tr_tfartwork_pic_country_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_lov_pic_country_multiple_static").val('').trigger("change");

    $(".tr_tfartwork_pic_person_multiple_static:visible").not(":eq(0)").remove();
    $(".cls_lov_pic_person_multiple_static").val('').trigger("change");
}

