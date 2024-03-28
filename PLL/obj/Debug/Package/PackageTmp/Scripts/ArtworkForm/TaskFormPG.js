var ARTWORK_SUB_PG_ID = 0;

$(document).ready(function () {

    //set dieline popup
    //bind_lov('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other', '/api/lov/primarytype', 'data.DISPLAY_TXT');
    //bind_lov('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search', '/api/lov/packtype', 'data.DISPLAY_TXT');
    //bind_lov('#modal_taskform_pg_search_for_dieline .cls_lov_pack_size', '/api/lov/packsizeXecm', 'data.DISPLAY_TXT');

    bind_text_editor('#modal_tfartwork_pg_submit ' + '.cls_txtedt_tfartwork_pg_submit_remark');
    bind_text_editor('#modal_tfartwork_pg_sendback ' + '.cls_txtedt_tfartwork_pg_sendback_remark');

    var overdue = '';
    bind_lov_reason('#modal_tfartwork_pg_submit .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PG_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT','.cls_input_pg_by_pa_submit_other');
    bind_lov_reason('#modal_tfartwork_pg_sendback .cls_lov_send_for_reason', '/api/lov/decisionreason?data.STEP_CODE=ARTWORK_PG_SEND_TO_PA&data.IS_OVERDUE=' + overdue, 'data.DISPLAY_TXT', '.cls_input_pg_by_pa_sendback_other');

    $(".cls_task_form_pg_artwork .cls_btn_submit_pg_artwork").click(function (e) {
        $('#modal_tfartwork_pg_submit').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $(".cls_task_form_pg_artwork .cls_btn_send_back_pg_artwork").click(function (e) {
        $('#modal_tfartwork_pg_sendback').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $("#modal_tfartwork_pg_submit form").submit(function (e) {
        if ($(this).valid()) {
            var jsonObj = new Object();
            jsonObj.data = {};

            jsonObj.data["REASON_ID"] = $('#modal_tfartwork_pg_submit .cls_lov_send_for_reason').val();
            jsonObj.data["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            jsonObj.data["ARTWORK_SUB_ID"] = ArtworkSubId;
            jsonObj.data["ACTION_CODE"] = 'SUBMIT';
            jsonObj.data["CREATE_BY"] = UserID;
            jsonObj.data["UPDATE_BY"] = UserID;
            var editor = new Quill('#modal_tfartwork_pg_submit .cls_txtedt_tfartwork_pg_submit_remark');
            jsonObj.data["COMMENT"] = editor.root.innerHTML;

            if (jsonObj.data["REASON_ID"] == DefaultResonId && OverDue == 1) {
                alertError2("Please select reason for overdue");
                return false;
            }

            jsonObj.data["REMARK_REASON"] = $("#modal_tfartwork_pg_submit .cls_input_pg_by_pa_submit_other").val();
            jsonObj.data["WF_STEP"] = getstepartwork('SEND_PG').curr_step;

            var myurl = '/api/taskform/artworkprocess/endtaskpg';
            var mytype = 'POST';
            var mydata = jsonObj;
            myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $("#modal_tfartwork_pg_sendback form").submit(function (e) {
        if ($(this).valid()) {
            var jsonObj = new Object();
            jsonObj.data = {};

            jsonObj.data["REASON_ID"] = $('#modal_tfartwork_pg_sendback .cls_lov_send_for_reason').val();
            jsonObj.data["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
            jsonObj.data["ARTWORK_SUB_ID"] = ArtworkSubId;
            jsonObj.data["ACTION_CODE"] = 'SEND_BACK';
            jsonObj.data["CREATE_BY"] = UserID;
            jsonObj.data["UPDATE_BY"] = UserID;
            var editor = new Quill('#modal_tfartwork_pg_sendback .cls_txtedt_tfartwork_pg_sendback_remark');
            jsonObj.data["COMMENT"] = editor.root.innerHTML;

            if (jsonObj.data["REASON_ID"] == DefaultResonId && OverDue == 1) {
                alertError2("Please select reason for overdue");
                return false;
            }
            if (jsonObj.data["REASON_ID"] == DefaultResonId) {
                alertError2("Please select reason for send back");
                return false;
            }

            jsonObj.data["REMARK_REASON"] = $("#modal_tfartwork_pg_sendback .cls_input_pg_by_pa_sendback_other").val();
            jsonObj.data["WF_STEP"] = getstepartwork('SEND_PG').curr_step;


            var myurl = '/api/taskform/artworkprocess/endtaskpg';
            var mytype = 'POST';
            var mydata = jsonObj;
            myAjaxConfirmSubmit(myurl, mytype, mydata, tohomepage, '', true, true, true);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_task_form_pg_artwork .cls_btn_search_for_die_line_pg_artwork").click(function (e) {
        first_load = true; // by aof #INC-4849
        if ($('.cls_artwork_form_task_form .cls_input_artwork_primary_type_other').val() != "") {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_primary_type').text($('.cls_artwork_form_task_form .cls_input_artwork_primary_type_other').val());
        }
        else if ($('.cls_artwork_form_task_form .cls_lov_artwork_primary_type_other option:selected').text() == "") {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_primary_type').text("-");
        }
        else {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_primary_type').text($('.cls_artwork_form_task_form .cls_lov_artwork_primary_type_other option:selected').text());
        }
        if ($('.cls_tfartwork_pa .cls_input_pa_primary_size').val() != "") {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_primary_size').text($('.cls_tfartwork_pa .cls_input_pa_primary_size').val());
            $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size').val($('.cls_tfartwork_pa .cls_input_pa_primary_size').val());
        }
        else {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_primary_size').text($('.cls_tfartwork_pa .cls_lov_pa_primary_size option:selected').text().split(':')[0]);
            $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size').val($('.cls_tfartwork_pa .cls_lov_pa_primary_size option:selected').text().split(':')[0]);
        }
        if ($('.cls_tfartwork_pa .cls_input_pa_pack_size').val() != "") {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_pack_size').text($('.cls_tfartwork_pa .cls_input_pa_pack_size').val());
        }
        else {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_pack_size').text($('.cls_tfartwork_pa .cls_lov_pa_pack_size option:selected').text());
        }
        if ($('.cls_tfartwork_pa .cls_input_pa_packing_style').val() != "") {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_packing_style').text($('.cls_tfartwork_pa .cls_input_pa_packing_style').val());
            $('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val($('.cls_tfartwork_pa .cls_input_pa_packing_style').val());
        }
        else {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_packing_style').text($('.cls_tfartwork_pa .cls_lov_pa_packing_style option:selected').text());
            $('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val($('.cls_tfartwork_pa .cls_lov_pa_packing_style option:selected').text().split(':')[0]);
        }

        $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_packaging_type').text($('.cls_tfartwork_pa .cls_lov_pa_material_group option:selected').text());

        if ($('.cls_tfartwork_pa .cls_input_pa_total_colour').val() != "") {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_total_colour').text($('.cls_tfartwork_pa .cls_input_pa_total_colour').val());
        }
        else {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_total_colour').text($('.cls_tfartwork_pa .cls_lov_pa_total_colour option:selected').text());
        }

        if ($('.cls_tfartwork_pa .cls_input_pa_style_of_printing').val() != "") {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_style_of_printing').text($('.cls_tfartwork_pa .cls_input_pa_style_of_printing').val());
        }
        else {
            $('#modal_taskform_pg_search_for_dieline .cls_lbl_info_style_of_printing').text($('.cls_tfartwork_pa .cls_lov_pa_style_of_printing option:selected').text());
        }

         // by aof #INC-4849

         // by aof #INC-4849




        bindSearchForDielineArtwork();
        first_load = false; // by aof #INC-4849
        $('#modal_taskform_pg_search_for_dieline').modal({
            backdrop: 'static',
            keyboard: true
        });
    });

    $(".cls_task_form_pg_artwork .cls_btn_taskform_pg_dieline_clear").click(function () {
        setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other', '', '');
        setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search', '', '');
        //setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size', '', '');
        setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_pack_size', '', '');
        //setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style', '', '');

        $('#modal_taskform_pg_search_for_dieline .cls_lov_primary_size').val('');
        $('#modal_taskform_pg_search_for_dieline .cls_lov_packing_style').val('');

        $('#modal_taskform_pg_search_for_dieline .cls_lov_dimension_of').val(''); // by aof #INC-4849
        $('#modal_taskform_pg_search_for_dieline .cls_lov_final_info_group').val(''); // by aof #INC-4849
    });

    $(".cls_task_form_pg_artwork .cls_btn_taskform_pg_dieline_search").click(function () {
        bindSearchForDielineArtwork();
    });

    $('.cls_task_form_pg_artwork #modal_taskform_pg_search_for_dieline .cls_lov_primary_size').on('keypress', function (e) {
        if (e.which === 13) {
            bindSearchForDielineArtwork();
        }
    });
    $('.cls_task_form_pg_artwork #modal_taskform_pg_search_for_dieline .cls_lov_packing_style').on('keypress', function (e) {
        if (e.which === 13) {
            bindSearchForDielineArtwork();
        }
    });

    $(".cls_task_form_pg_artwork .cls_btn_taskform_pg_dieline_select").click(function (e) {
        var table = $('#table_taskform_pg_search_for_dieline').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            if (tblData[0].PACKING_TYPE_ID != $('.cls_task_form_pg_artwork .cls_hid_artwork_taskform_packaging_type').val()) {
                alertError2('Packaging type does not match');
            } else {
                saveDataPG_Dieline_Artwork(tblData[0]);
                //bindDataPG_Dieline(tblData[0]);
                $("#modal_taskform_pg_search_for_dieline .cls_btn_taskform_pg_dieline_close").click();
            }
        }
        else {
            alertError2("Please select at least 1 item.");
        }
    });

    bindDataTaskFormPG_Artwork();
    setReadOnlyTaskFormPG();
    //getPA_RDReference();
});

//function callback_submit_PG_Artwork(res) {
//    var jsonObj = new Object();
//    jsonObj.data = {};

//    jsonObj.data["ARTWORK_SUB_ID"] = ArtworkSubId;
//    jsonObj.data["CREATE_BY"] = UserID;
//    jsonObj.data["UPDATE_BY"] = UserID;

//    var myurl = '/api/taskform/internal/pg/copydielinefiles';
//    var mytype = 'POST';
//    var mydata = jsonObj;
//    myAjax(myurl, mytype, mydata, tohomepage);
//}

function setReadOnlyTaskFormPG() {
    $('.cls_task_form_pg').find('input, textarea, select').attr('disabled', true);
    $('.cls_row_pg_button').hide();
}

function bindDataTaskFormPG_Artwork() {
    var myurl = '/api/taskform/internal/pg/info?data.artwork_request_id=' + ARTWORK_REQUEST_ID + '&data.artwork_sub_id=' + ArtworkSubId;
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_bind_dataTaskFormPG_Artwork);
}

function callback_bind_dataTaskFormPG_Artwork(res) {
    //bind_PG
    if (res.data != null) {
        if (res.data.length > 0) {
            var item = res.data[0];
            if (item != null) {
                if (item.DIE_LINE_MOCKUP_SUB_ID != null) {
                    bindDataPG(item.DIE_LINE_MOCKUP_SUB_ID, ArtworkSubId);
                }
                if (item.HISTORIES != null) {
                    bindDataPG_Artwork_Log(item.HISTORIES);
                }


                setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_primary_type_other', item.PRIMARY_TYPE_ID, item.PRIMARY_TYPE_DISPLAY_TXT);
                //$('.cls_task_form_pg_artwork .cls_hid_artwork_taskform_primary_type').val(item.PRIMARY_TYPE_ID);
                //setValueToDDL('#modal_taskform_pg_search_for_dieline .cls_lov_pg_packaging_type_search', item.PACKAGING_TYPE_ID, item.PACKAGING_TYPE_DISPLAY_TXT);
            }
        }
    }
    else {
        $('.cls_task_form_pg .cls_lov_roll_sheet').val('').trigger("change");
    }
}

function bindDataPG_Artwork_Log(v) {
    //$('.cls_task_form_pg .cls_row_pg_dieline').show();
    //$('.cls_task_form_pg .cls_hid_pg_dieline_mockup_id').val(v.MOCKUP_ID);
    var tablePGArtworkLog = $('#table_pg_artwork_log').DataTable({
        "searching": false,
        "ordering": false,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bAutoWidth": false
    });
    tablePGArtworkLog.clear().draw();
    for (i = 0; i < v.length; i++) {
        var dateCommentPA = (v[i].COMMENT_BY_PA == null ? "" : myDateTimeMoment(v[i].CREATE_DATE_BY_PA));
        var dateCommentPG = (v[i].COMMENT_BY_PG == null ? "" : myDateTimeMoment(v[i].CREATE_DATE_BY_PG));
        var rowData = [
            v[i].NO,
            v[i].REASON_BY_PA,
            v[i].COMMENT_BY_PA,
            dateCommentPA,
            v[i].ACTION_NAME,
            v[i].REASON_BY_OTHER,
            v[i].COMMENT_BY_PG,
            dateCommentPG
        ];
        tablePGArtworkLog.row.add(rowData).draw(false);
    }

}


// by aof #INC-4849
function bindSearchForDielineArtwork() {


    $('.cls_table_text_search').val('');
    setparamSearchDieline();
   
    table_search_for_dieline.destroy();

    table_search_for_dieline = $('#table_taskform_pg_search_for_dieline').DataTable({
        serverSide: false,//true,
        orderCellsTop: true,
        fixedHeader: true,
        columnDefs: [{
            "searchable": false,
            "orderable": false,
            "targets": 0
        }],
        //searching: true,
        //stateSave: false,
        ajax: function (data, callback, settings) {

            //for (var i = 0, len = data.columns.length; i < len; i++) {
            //    delete data.columns[i].name;
            //    delete data.columns[i].data;
            //    delete data.columns[i].searchable;
            //    delete data.columns[i].orderable;
            //    delete data.columns[i].search.regex;
            //    delete data.search.regex;

            //    delete data.columns[i].search.value;
            //}

            $.ajax({
                // url: suburl + "/api/taskform/pg/searchdieline2?data.PRIMARY_TYPE_ID=" + primaryTypeId  //by aof
                url: suburl + "/api/taskform/pg/searchdieline_tutuning?data.PRIMARY_TYPE_ID=" + primaryTypeId  //by aof
                    + "&data.PACKING_TYPE_ID=" + packagingTypeId
                    + "&data.PRIMARY_SIZE_DISPLAY_TXT=" + primarySizeId
                    + "&data.PACK_SIZE_DISPLAY_TXT=" + packSizeId    
                    + "&data.PACKING_STYLE_DISPLAY_TXT=" + packagingStyleId
                    + "&data.DIMENSION_OF_DISPLAY_TXT=" + dimension_of
                    + "&data.FINAL_INFO_GROUP_DISPLAY_TXT=" + final_info_group
                    + "&data.FIRST_LOAD=" + first_load,                  
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        fixedColumns: {
            leftColumns: 3
        },
        columns: [
       

            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },

            { data: "CHECK_LIST_NO", "className": "cls_nowrap" },
            { data: "MOCKUP_NO", "className": "cls_nowrap" },
            { data: "PRIMARY_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "CONTAINER_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "LID_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACK_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKING_STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKAGING_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "GRADE_OF_DISPLAY_TXT", "className": "cls_nowrap" },     
            { data: "DIMENSION_OF_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "FINAL_INFO_GROUP_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "SHEET_SIZE", "className": "cls_nowrap" },
            { data: "FLUTE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STATUS_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "NUMBER_OF_COLOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STYLE_OF_PRINTING_DISPLAY_TXT", "className": "cls_nowrap" }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "processing": true,
        "scrollX": true,
        order: [[1, 'asc']],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(1).html('<a target="_blank" href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '"> ' + data.CHECK_LIST_NO + ' </a>');
            $(row).find('td').eq(2).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '"> ' + data.MOCKUP_NO + ' </a>');
        },

      

    });


    $(table_search_for_dieline.table().container()).on('keyup', 'input', function () {
        if (event.which == 13)
        {
            table_search_for_dieline
                .column($(this).data('index'))
                .search(this.value, false)
                .draw();
        }
      
    });

    $("#table_taskform_pg_search_for_dieline_filter").hide();

}
// by aof #INC-4849

function bindSearchForDielineArtwork_old() {

    setparamSearchDieline();
    table_search_for_dieline.destroy();

    table_search_for_dieline = $('#table_taskform_pg_search_for_dieline').DataTable({
        serverSide: true,
        ajax: function (data, callback, settings) {

            for (var i = 0, len = data.columns.length; i < len; i++) {
                delete data.columns[i].name;
                delete data.columns[i].data;
                delete data.columns[i].searchable;
                delete data.columns[i].orderable;
                delete data.columns[i].search.regex;
                delete data.search.regex;

                delete data.columns[i].search.value;
            }

            $.ajax({
                    url: suburl + "/api/taskform/pg/searchdieline2?data.PRIMARY_TYPE_ID=" + primaryTypeId  
             
                    + "&data.PACKING_TYPE_ID=" + packagingTypeId
                    + "&data.PRIMARY_SIZE_DISPLAY_TXT=" + primarySizeId
                    + "&data.PACK_SIZE_ID=" + packSizeId
                    + "&data.PACKING_STYLE_DISPLAY_TXT=" + packagingStyleId
                    + "&data.FIRST_LOAD=" + first_load,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        fixedColumns: {
            leftColumns: 1
        },
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },

            { data: "CHECK_LIST_NO", "className": "cls_nowrap" },
            { data: "MOCKUP_NO", "className": "cls_nowrap" },
            { data: "PRIMARY_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PRIMARY_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "CONTAINER_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "LID_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACK_SIZE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKING_STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "PACKAGING_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "GRADE_OF_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "DIMENSION_OF_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "SHEET_SIZE", "className": "cls_nowrap" },
            { data: "FLUTE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STYLE_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STATUS_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "NUMBER_OF_COLOR_DISPLAY_TXT", "className": "cls_nowrap" },
            { data: "STYLE_OF_PRINTING_DISPLAY_TXT", "className": "cls_nowrap" }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "processing": true,
        "scrollX": true,
        order: [[1, 'asc']],
        "rowCallback": function (row, data, index) {
            $(row).find('td').eq(1).html('<a target="_blank" href="' + suburl + '/CheckList/' + data.CHECK_LIST_ID + '"> ' + data.CHECK_LIST_NO + ' </a>');
            $(row).find('td').eq(2).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_SUB_ID + '"> ' + data.MOCKUP_NO + ' </a>');
        },
    });
}


function saveDataPG_Dieline_Artwork(data) {
    var jsonObj = new Object();
    jsonObj.data = [];

    var item = {};
    item["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
    item["ARTWORK_SUB_ID"] = ArtworkSubId;
    item["DIE_LINE_MOCKUP_ID"] = data.MOCKUP_ID;
    item["CREATE_BY"] = UserID;
    item["UPDATE_BY"] = UserID;

    jsonObj.data = item;

    var myurl = '/api/taskform/internal/pg/info';
    var mytype = 'POST';
    var mydata = jsonObj;
    myAjax(myurl, mytype, mydata, bindDataTaskFormPG_Artwork, '', true, true);
}
