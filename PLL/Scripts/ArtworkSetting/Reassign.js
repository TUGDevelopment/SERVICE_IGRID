var table_reassign;
var first_load = true;   // by aof reassign
$(document).ready(function () {


    //by aof on 01/04/2022 start
    $('body').on('change', '#chk_select_all', function () {
        if (this.checked)
            $('.cls_form_reassign .cls_chk_reassign_wf').prop('checked', true);
        else
            $('.cls_form_reassign .cls_chk_reassign_wf').prop('checked', false);

        $('.cls_form_reassign .cls_chk_reassign_wf').change();
    });
     //by aof on 01/04/2022 end


    $('#popup_reassign form').validate({
        rules: {
            reassign_:
            {
                required: true
            }

        },
        messages: {
            reassign_:
            {
                required: "Please fill remark first"
            }
        }
    });


    bind_lov_no_ajax('.cls_form_reassign .cls_workflow_type', '', '');
    bind_lov('.cls_form_reassign .cls_current_assign', '/api/lov/user', 'data.DISPLAY_TXT');
    bind_lov('.cls_form_reassign .cls_current_step', '/api/common/stepmockupandpartwork', 'data.DISPLAY_TXT');  //by aof on 01/04/2022

    $(".cls_form_search form").submit(function (e) {

        if ($('.cls_form_reassign .cls_current_step').select2('data').length > 0) {
            current_step_wf_type = $('.cls_form_reassign .cls_current_step').select2('data')[0].WF_TYPE;
            current_step_id = $('.cls_form_reassign .cls_current_step').select2('data')[0].STEP_ID;
        }

        else {
            current_step_wf_type = '';
            current_step_id = 0;
        }

        table_reassign.ajax.reload();
        e.preventDefault();
    });

    $(".cls_form_reassign .cls_btn_clear").click(function () {
        $('.cls_form_reassign input[type=text]').val('');
        $('.cls_form_reassign .cls_workflow_type').val('All').change();
    });

    table_reassign = $('#table_reassign').DataTable({
        "serverSide": true,
        "pageLength": 25,
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/wfsetting/reassign/info?data.wf_no=" + $('.cls_form_reassign .cls_workflow_number').val().replace(/\n/g, ',') //+ $('.cls_form_reassign .cls_workflow_number').val()
                    + "&data.wf_type=" + $('.cls_form_reassign .cls_workflow_type').val()
                    + "&data.create_date_from=" + $('.cls_form_reassign .cls_create_date_from').val()
                    + "&data.create_date_to=" + $('.cls_form_reassign .cls_create_date_to').val()
                    + "&data.current_user_id=" + $('.cls_form_reassign .cls_current_assign').val()
                    + "&data.remark=" + current_step_wf_type   // by aof on 01/04/2022 to use remark for keeping step wf type ( A1,M1) A=AW,M=MO and 1 is step_id
                    + "&data.user_id=" + UserID
                    + "&data.first_load=" + first_load  // by aof reassign
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
            { "data": "SOLD_TO_NAME", "className": "" },      //BY AOF #INC-130655 
            { "data": "SHIP_TO_NAME", "className": "" },  //BY AOF #INC-130655 
            { "data": "COUNTRY", "className": "cls_nowrap" },   //BY AOF #INC-130655 
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
        },
    });

    //table_reassign.on('order.dt search.dt', function () {
    //    table_reassign.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
    //        cell.innerHTML = i + 1;
    //    });
    //}).draw();


    
  
    bind_lov('#popup_reassign .cls_lov_assign_to', '/api/lov/userreassign', 'data.DISPLAY_TXT');

    $("#popup_reassign form").submit(function (e) {
        if ($(this).valid()) {
            myAjaxConfirmSubmitBlank(callbackSubmitReAssign);
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_form_reassign .cls_btn_reassign").click(function () {
        if ($('.cls_form_reassign .cls_chk_reassign_wf:checked').length > 0) {

            $('.cls_form_reassign .cls_input_remark').val('');
            $('.cls_form_reassign .cls_lov_assign_to').val(null).trigger('change');

            $('#popup_reassign').modal({
                backdrop: 'static',
                keyboard: true
            });
        }
        else {
            alertError2("Please select workitem at least 1 item.");
        }
    });

   // by aof reassign
    var UserName = UserNameDisplay + " (" + UserPositionName + ")";
    //$('.cls_form_reassign .cls_current_assign').select2('item', { ID: UserID, DISPLAY_TXT: "Maneerat Ketsuwan (Packaging)" });
    $('.cls_form_reassign .cls_current_assign').append('<option selected = "selected" value="' + UserID + '">' + UserName +'</option>'); 
    first_load = false;   // by aof reassign
});

function reload_table_reassign() {
    $('#popup_reassign').modal('hide');
    table_reassign.ajax.reload();
}

function callbackSubmitReAssign() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    $(".cls_form_reassign .cls_chk_reassign_wf:checked").each(function () {
        var item = {};
        item["WORKFLOW_SUB_ID"] = $(this).data("wf_sub_id");
        item["WORKFLOW_TYPE"] = $(this).closest('tr').find('.cls_td_wf_type').text();
        item["CURRENT_USER_ID"] = $('.cls_form_reassign .cls_lov_assign_to').val();
        item["REMARK_REASSIGN"] = $('.cls_form_reassign .cls_input_remark').val();
        item["CREATE_BY"] = UserID;
        item["UPDATE_BY"] = UserID;
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/wfsetting/reassign/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxAlertNoBack(myurl, mytype, mydata, reload_table_reassign, "", true, true);

    $('#chk_select_all').prop('checked', false);   //by aof on 01/04/2022 
    $('#chk_select_all').change();  //by aof on 01/04/2022 
}

var current_step_wf_type = '';    //by aof on 01/04/2022 
var current_step_id = 0;   //by aof on 01/04/2022 