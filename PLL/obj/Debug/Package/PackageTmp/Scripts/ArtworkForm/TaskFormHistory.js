var userPA = 0;
$(document).ready(function () {

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_history':
                if (table_artwork_history == null)
                    bind_table_history();
                else {
                    countvnpm = 0; countvnsl = 0;
                    table_artwork_history.ajax.reload();
                }
                break;
            default:
                break;
        }
    });
});

var table_artwork_history;
function bind_table_history() {
    table_artwork_history = $('#table_taskform_history').DataTable({
        ajax: function (data, callback, settings) {
            var xhr = $.ajax({
                url: suburl + "/api/taskform/history/artworkinfo?data.artwork_sub_id=" + ArtworkSubId,
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
                        return '<img title="Re-Assign from : ' + row.OLD_OWNER_REASSIGN + '<br>Re-Assign by : ' + row.REASSIGNBY + '<br> Re-Assign reason : ' + row.REASSIGNREASON + '" style="width:15px;" src="' + suburl + '/Content/img/re-assige.png">';
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
            { "data": "CURRENT_USER_DISPLAY_TXT" },
            { "data": "CURRENT_STEP_DISPLAY_TXT" },
            { "data": "REMARK", "className": "" },
            { "data": "REMARK_OTHERS", "className": "" },  ////ticket#442923 by aof 
            //ticket#442923 by aof start
            //{
            //    render: function (data, type, row, meta) {     
            //        if (row.REMARK_OTHERS != null) {
                        
            //            //var rem = row.REMARK_OTHERS;
            //            //rem = rem.replace(new RegExp('\r?\n', 'g'), '<br/>');
            //            //rem = rem.replace(/\s/g, '&nbsp;');

            //            //prepareHTML(row.REMARK_OTHERS);
                      
            //            //return rem;

            //            return prepareHTML(row.REMARK_OTHERS);
            //        }
            //        else {
            //            return '';
            //        }
            //    }, "className": ""
                //ticket#442923 by aof end
            //},
            { "data": "CREATE_DATE", "className": "cls_center cls_td_width_150" },
            { "className": "cls_td_width_10" },
        ],
        "rowCallback": function (row, data, index) {

            if (data.CURRENT_STEP_ID == 2) userPA = data.CURRENT_USER_ID;
            if (userPA == CURRENTUSERID && data.IS_END != "X" && data.CURRENT_STEP_ID != 2) {
                $(row).find('td').eq(10).html('<img class="cls_img_end_task_artwork" title="End task" onclick="killprocess_aw(' + data.ARTWORK_SUB_ID + ')" style="cursor:pointer; width: 10px;" src="' + suburl + '/Content/img/terminate.png">');
            }
            else {
                $(row).find('td').eq(10).html('');
            }

            if (data.CREATE_DATE != data.UPDATE_DATE && myDateTimeMoment(data.UPDATE_DATE) != '') {
                $(row).find('td').eq(9).html("Create date : " + myDateTimeMoment(data.CREATE_DATE) + "<br/>" + "Update date : " + myDateTimeMoment(data.UPDATE_DATE));
            }
            else {
                $(row).find('td').eq(9).html("Create date : " + myDateTimeMoment(data.CREATE_DATE));
            }
            //$(row).find('td').eq(9).html(myDateTimeMoment(data.CREATE_DATE));

            if (data.CURRENT_STEP_CODE_DISPLAY_TXT != null) {
                if (data.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_RD') {
                    if (data.IS_END == null) {
                        $('#qc_submit_modal').find('.cls_chk_send_pa_qc').attr('disabled', true);
                        $('.cls_art_qc').find('.cls_qc_btn_send_back').attr('disabled', true);
                        $('.cls_art_qc .cls_rd_inprocess').show();
                    }
                    else {
                        $('#qc_submit_modal').find('.cls_chk_send_pa_qc').attr('disabled', false);
                        $('.cls_art_qc').find('.cls_qc_btn_send_back').attr('disabled', false);
                        $('.cls_art_qc .cls_rd_inprocess').hide();
                    }
                }

                if (data.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_PM' && data.REMARK_KILLPROCESS == null)
                    countvnpm = 1;
                if (data.CURRENT_STEP_CODE_DISPLAY_TXT == 'SEND_VN_SL' && data.REMARK_KILLPROCESS == null)
                    countvnsl = 1;
            }
        },
        "initComplete": function (settings, json) {

        }
    });

    $("#modal_tfartwork_history_terminate .cls_btn_tfartwork_history_terminate_submit").click(function (e) {
        var jsonObj = new Object();
        jsonObj.data = {};
        var PROCESS = {};

        PROCESS["ARTWORK_REQUEST_ID"] = ARTWORK_REQUEST_ID;
        PROCESS["ARTWORK_SUB_ID"] = $('#modal_tfartwork_history_terminate .cls_aw_sub_id_terminate').val();
        PROCESS["CREATE_BY"] = UserID;
        PROCESS["UPDATE_BY"] = UserID;
        var editor = new Quill('#modal_tfartwork_history_terminate .cls_txtedt_tfartwork_history_terminate_remark');
        PROCESS["REMARK_KILLPROCESS"] = editor.root.innerHTML;

        jsonObj.data = PROCESS;

        var myurl = '/api/taskform/pa/killprocess';
        var mytype = 'POST';
        var mydata = jsonObj;

        myAjaxConfirmSubmit(myurl, mytype, mydata, hide_modal_submit_killprocess_aw);
    });
}

function killprocess_aw(aw_sub_id_) {
    $('#modal_tfartwork_history_terminate').modal({
        backdrop: 'static',
        keyboard: true
    });
    $('#modal_tfartwork_history_terminate .cls_aw_sub_id_terminate').val(aw_sub_id_);
}

function hide_modal_submit_killprocess_aw() {
    countvnpm = 0; countvnsl = 0;
    $('#modal_tfartwork_history_terminate').modal('hide');
    table_artwork_history.ajax.reload();

    click_retrive_or_request_mat = true;
    bindTaskformArtwork(ArtworkSubId);
}

//ticket#442923 by aof 
function prepareHTML(text) {
    var new_text = "";
    var x = text.split("\n");

    for (i = 0; i < x.length; i++) {
        new_text += "<p>"+ x[i].replace(/\s/g, '&nbsp;'); +"<p/>";
    }
    return new_text;

}
//ticket#442923 by aof 