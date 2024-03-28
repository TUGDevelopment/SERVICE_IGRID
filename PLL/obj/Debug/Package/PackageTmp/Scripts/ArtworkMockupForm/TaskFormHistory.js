var userPG = 0;
$(document).ready(function () {

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_history':
                if (table_taskform_history == null)
                    bind_table_history();
                else
                    table_taskform_history.ajax.reload();
                break;
            default:
                break;
        }
    });
});

var table_taskform_history;
function bind_table_history() {
    table_taskform_history = $('#table_taskform_history').DataTable({
        ajax: function (data, callback, settings) {
            var xhr = $.ajax({
                url: suburl + "/api/taskform/history/info?data.mockup_sub_id=" + MOCKUPSUBID,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[9, 'asc']],
        "processing": true,
        "lengthChange": false,
        //"ordering": false,
        //"info": false,
        "searching": false,
        "pageLength": -1,
        "paging": false,
        "scrollX": true,
        "columnDefs": [
            {
                "max-width": "1px",
                "searchable": false,
                "orderable": false,
                "targets": 1
            }, {
                "searchable": false,
                "orderable": false,
                "targets": 2
            }, {
                "searchable": false,
                "orderable": false,
                "targets": 3
            }, {
                "searchable": false,
                "orderable": false,
                "targets": 4
            }, {
                "searchable": false,
                "orderable": false,
                "targets": 10
            }
        ],
        "columns": [
            {
                render: function (data, type, row, meta) {
                    if (row.IS_END == 'X' && row.REMARK_KILLPROCESS == null) {
                        return '<img title="Completed" style="width:15px;" src="' + suburl + '/Content/img/ico_true.png">';
                    }
                    else if (row.REMARK_KILLPROCESS != null) {
                        return '<img title="Terminated" style="width:15px;" src="' + suburl + '/Content/img/ico_killed.png">';
                    }
                    else {
                        return '';
                    }
                }, "className": "cls_td_width_10"
            },
            {
                render: function (data, type, row, meta) {
                    if (row.IS_CG_OWNER == 'X') {
                        return '<img title="Change owner from : ' + row.OLD_OWNER_CG_OWNER + '<br>Change owner by : ' + row.CG_OWNER_BY + '<br> Change owner reason : ' + row.CG_OWNER_REASON + '" style="width:15px;" src="' + suburl + '/Content/img/cg-owner.png">';
                    }
                    else {
                        return '';
                    }
                }, "className": "cls_td_width_10"
            },
            {
                render: function (data, type, row, meta) {
                    if (row.IS_REASSIGN == 'X') {
                        return '<img title="Re-Assign from : ' + row.OLD_OWNER_REASSIGN + '<br>Re-Assign by : ' + row.REASSIGNBY + '<br> Re-Assign reason : ' + row.REASSIGNREASON + '" style="width:15px;" src="' + suburl +'/Content/img/re-assige.png">';
                    }
                    else {
                        return '';
                    }
                }, "className": "cls_td_width_10"
            },
            {
                render: function (data, type, row, meta) {
                    if (row.IS_REOPEN == 'X') {
                        //return '<img title="' + row.CURRENT_STEP_DISPLAY_TXT + ' has been Re-Open <br> Re-Open by : ' + row.REOPENBY + '<br> Re-Open reason : ' + row.REOPENREASON + '" style="width:15px;" src="' + suburl + '/Content/img/re-assige.png">';
                        return '<img title="Re-Open by : ' + row.REOPENBY + '<br> Re-Open reason : ' + row.REOPENREASON + '" style="width:15px;" src="' + suburl + '/Content/img/re-open.png">';
                    }
                    else {
                        return '';
                    }
                }, "className": "cls_td_width_10"
            },
            {
                render: function (data, type, row, meta) {
                    if (row.IS_DELEGATE_ == 'X') {
                        return '<img title="Delegated from : ' + row.OLD_OWNER_DELEGATE + '<br> Delegate reason : ' + row.DELEGATEREASON + '" style="width:15px;" src="' + suburl + '/Content/img/delegate.png">';
                    }
                    else {
                        return '';
                    }
                }, "className": "cls_td_width_10"
            },
            { "data": "CURRENT_USER_DISPLAY_TXT", "className": "" },
            { "data": "CURRENT_STEP_DISPLAY_TXT", "className": "" },
            { "data": "REMARK" },
            { "data": "REMARK_OTHERS" },
            { "data": "CREATE_DATE", "className": "cls_center cls_td_width_150" },
            { "className": "cls_td_width_10" },
        ],
        "rowCallback": function (row, data, index) {
            if (data.CREATE_DATE != data.UPDATE_DATE && myDateTimeMoment(data.UPDATE_DATE) != '') {
                $(row).find('td').eq(9).html("Create date : " + myDateTimeMoment(data.CREATE_DATE) + "<br/>" + "Update date : " + myDateTimeMoment(data.UPDATE_DATE));
            }
            else {
                $(row).find('td').eq(9).html("Create date : " + myDateTimeMoment(data.CREATE_DATE));
            }
            //$(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE));

            if (data.IS_END == 'X' && data.UPDATE_BY == '-1') {
                $(row).find('td').eq(8).html("[Auto close workitem by system.]");
            }
            
            if (data.CURRENT_STEP_ID == 1) userPG = data.CURRENT_USER_ID;
            if (userPG == CURRENTUSERID && data.IS_END != "X" && data.CURRENT_STEP_ID != 1) {
                //$(row).find('td').eq(4).html('<img title="Terminate" data-toggle="modal" data-target="#popup_killprocess_workflow_mockup" style="cursor:pointer; width: 10px;" src="' + suburl + '/Content/img/terminate.png">');
                $(row).find('td').eq(10).html('<img class="cls_img_end_task_mockup" title="End task" onclick="killprocess(' + data.MOCKUP_SUB_ID + ',' + data.MOCKUP_ID + ')" style="cursor:pointer; width: 10px;" src="' + suburl + '/Content/img/terminate.png">');
            }
            else {
                $(row).find('td').eq(10).html('');
            }
        },
        "initComplete": function (settings, json) {

        }
    });


    $("#popup_killprocess_workflow_mockup .cls_btn_submit_terminate_workflow").click(function (e) {
        var jsonObj = new Object();
        jsonObj.data = {};
        jsonObj.data["MOCKUP_ID"] = $('#popup_killprocess_workflow_mockup .cls_mockup_id_terminate').val();
        jsonObj.data["MOCKUP_SUB_ID"] = $('#popup_killprocess_workflow_mockup .cls_mockup_sub_id_terminate').val();
        jsonObj.data["CREATE_BY"] = UserID;
        jsonObj.data["UPDATE_BY"] = UserID;
        var editor = new Quill('#popup_killprocess_workflow_mockup .cls_txtedt_terminate_workflow');
        jsonObj.data["REMARK_KILLPROCESS"] = editor.root.innerHTML;

        var myurl = '/api/taskform/mockupprocess/killwfmockup';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjaxConfirmSubmit(myurl, mytype, mydata, hide_modal_submit_killprocess);
    });

}

function killprocess(mockup_sub_id_, mockup_id_) {
    $('#popup_killprocess_workflow_mockup').modal({
        backdrop: 'static',
        keyboard: true
    });
    $('#popup_killprocess_workflow_mockup .cls_mockup_sub_id_terminate').val(mockup_sub_id_);
    $('#popup_killprocess_workflow_mockup .cls_mockup_id_terminate').val(mockup_id_);
}

function hide_modal_submit_killprocess() {
    $('#popup_killprocess_workflow_mockup').modal('hide');
    table_taskform_history.ajax.reload();
}