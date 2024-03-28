var first_load = true;
var Status_ChgMatDesc;

$(document).ready(function () {
    bind_lov('.cls_report_ImpactedMat_Desc .cls_lov_by', '/api/lov/byIGrid', 'data.DISPLAY_TXT');
    //bind_lov('.cls_report_ImpactedMat_Desc .cls_lov_master', '/api/lov/selectmaster', 'data.DISPLAY_TXT');
    builddata(false, "0");
    $("#table_report_impaced_wrapper .dt-buttons .buttons-excel").hide();
    first_load = false;
    $('.cls_report_ImpactedMat_Desc .cls_lov_condition').on("change", function () {
        var selections = $(this).select2('data');
        var v = $(this)[0].value;
        var isClear = true;
    });
    
    //$('body').on('change', '#mat_select_all', function () {
    //    if (this.checked)
    //        $('.cls_report_ImpactedMat_Desc .cls_chk_group').prop('checked', true);
    //    else
    //        $('.cls_report_ImpactedMat_Desc .cls_chk_group').prop('checked', false);

    //    $('.cls_report_ImpactedMat_Desc .cls_chk_group').change();
    //});



    $(".cls_report_ImpactedMat_Desc .cls_impac_btn_clr").click(function () {
        debugger;
        updatestatus("In process");
    });
    $(".cls_report_ImpactedMat_Desc .cls_impac_btn_Reject").click(function () {

        var reason = prompt("Please input the reason for reject", "");
        if (reason != null)
        {
           // alertError2("OK");
           updatestatus("Reject",reason);
        }
      
    });
    $(".cls_report_ImpactedMat_Desc .cls_impac_btn_Reset_reject").click(function () {

        updatestatus("Reset Reject");
    });
    $(".cls_report_ImpactedMat_Desc .cls_btn_export_excel").click(function () {
        $(".cls_report_ImpactedMat_Desc .buttons-excel").click();
        // $(".cls_report_warehouse .buttons-excel").unbind();
    });
    $(".cls_report_ImpactedMat_Desc .cls_impac_btn_search").click(function (e) {
        //debugger;
        var master = $('.cls_report_ImpactedMat_Desc .cls_lov_master').val();

        //if (!isEmpty(master)) {
            //table_report_impaced.ajax.reload();

        if (checkRequireData() )
        {
            builddata(false, "0");
        }
           
        //    e.preventDefault();
        //}
        //else {
        //    alertError2("Please input request condition.");

        //    e.preventDefault();
        //}
    });
});

function checkRequireData() {
    var f_pass = false;
    

    var FrDt = $('.cls_report_ImpactedMat_Desc .cls_impac_req_from').val();
    var ToDt = $('.cls_report_ImpactedMat_Desc .cls_impac_req_to').val();
    // var columns;
    if (!isEmpty(FrDt) && !isEmpty(ToDt)) {
        f_pass = true;
    }
    else {
        alertError2("Please input Changed date from and Changed date to.");

        //e.preventDefault();
    }
    return f_pass;
}

function updatestatus(Status_ChgMatDesc,reason) {

    
    var table = $('#table_report_impaced').DataTable();
    var tblData = table.rows('.selected').data();
    
    
    //if (tblData.length > 0) {
        //const array = [];
        var jsonObj = new Object();
        jsonObj.data = [];
        var data = [];
    $(".cls_report_ImpactedMat_Desc .cls_chk_group:checked").each(function () {
        //var value = $(this).data("mat_id");
        var value = $(this);
        var col_Status = $(this).closest('tr').children('td').eq(2).text();
        var col_Reason = $(this).closest('tr').children('td').eq(3).text();
        //for (i = 0; i < tblData.length; i++) {
        var item = {};
        //var three_p_id = tblData[i].Id;
        //array.push(tblData[i].Id);
        if (Status_ChgMatDesc == "In process") {
            if (col_Status == "Not Start" || col_Status == "Failed") {
                item["Id"] = $(this).data("mat_id");
                item["Reason"] = "";
                item["Status"] = Status_ChgMatDesc;
                data.push(item);
            }
        } else if (Status_ChgMatDesc == "Reset Reject") {
           // if (col_Reason == "" || col_Reason == null) {
                item["Id"] = $(this).data("mat_id");
                item["Reason"] = "";
                item["Status"] = "Not Start";
                data.push(item);
            //}
        } else if (Status_ChgMatDesc = "Reject") {
            if (col_Status == "Not Start" && (col_Reason == "" || col_Reason == null)) {
                item["Id"] = $(this).data("mat_id");
                item["Reason"] = reason;
                item["Status"] = Status_ChgMatDesc;
                data.push(item);
            }
        }
    });
            jsonObj.data = data;
            var myurl = '/api/report/ImpactedMatDesc';
            var mytype = 'POST';
            var mydata = jsonObj;

            myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSaveTaskFormPA);
}
function callbackSaveTaskFormPA() {
    builddata(false, "0");
}
function builddata(serverSide, autosearch) {
    var groupColumnPPView = 2;
    var FrDt = $('.cls_report_ImpactedMat_Desc .cls_impac_req_from').val();
    var ToDt = $('.cls_report_ImpactedMat_Desc .cls_impac_req_to').val();
    //if (!isEmpty(ToDt)) {
    //    ToDt = FrDt.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");
    //}
    var table_report_impaced = $('#table_report_impaced').DataTable()
    table_report_impaced.destroy();
    table_report_impaced = $('#table_report_impaced').DataTable({
        "scrollY": "600px",
        "scrollCollapse": true,
        paging: true,
        orderCellsTop: true,
        stateSave: false,
        serverSide: false,
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
                url: suburl + "/api/report/ImpactedMatDesc?data.MasterName=" + $('.cls_report_ImpactedMat_Desc .cls_lov_master').val()
                    + "&data.FrDt=" + FrDt
                    + "&data.ToDt=" + ToDt
                    + "&data.Status=" + $('.cls_report_ImpactedMat_Desc .cls_lov_status').val()
                    + "&data.User=" + $('.cls_report_ImpactedMat_Desc .cls_lov_by option:selected').text()
                    + "&data.Keyword=" + $('.cls_report_ImpactedMat_Desc .cls_txt_master_keyword').val()
                    + "&data.Action=" + $('.cls_report_ImpactedMat_Desc .cls_lov_action').val()
                    + "&data.first_load=" + first_load
                ,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "order": [[1, 'desc']],
        "processing": true,
        //"lengthChange": false,
        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        'columnDefs': [{
            'targets': 0,
            'searchable': false,
            'orderable': false,
            'className': 'dt-body-center',
            'render': function (data, type, full, meta) {
                return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(data).html() + '">';
            }
        }],
        "ordering": true,
        "info": true,
        "searching": false,
        "scrollX": true,
        //"autoWidth": false,
        dom: 'Bfrtip',
        buttons: [
            {
                title: 'IGrid List all affected Materials report',
                extend: 'excelHtml5',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6,7,8,9,10,11,12,13,14,15,16],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            }, "pageLength"
        ],
        columns: [
            {
                render: function (data, type, row, meta) {
                    return '<input class="cls_chk_group" data-mat_id="' + row.Id + '" type="checkbox">';
                }
            },
            { data: "Id", "className": "cls_nowrap" },
            { data: "Status", "className": "cls_nowrap" },
            { data: "Reason", "className": "cls_nowrap" },
            { data: "Changed_By", "className": "cls_nowrap" },
            { data: "Changed_On", "className": "cls_nowrap" },
            { data: "Changed_Action", "className": "cls_nowrap cls_action" },
            { data: "Changed_Tabname", "className": "cls_nowrap cls_master" },
           
            //{ data: "Char_NewValue", "className": "cls_nowrap" },      
            { data: "Old_Id", "className": "cls_nowrap" },
            { data: "Old_Description", "className": "cls_nowrap" },
            { data: "Char_Description", "className": "cls_nowrap cls_new_desc" },
            { data: "DMSNo", "className": "cls_nowrap" },
            { data: "Material", "className": "cls_nowrap" },
            { data: "Description", "className": "cls_nowrap" },
            { data: "New_Material", "className": "cls_nowrap" },
            { data: "New_Description", "className": "cls_nowrap" },
            { data: "NewMat_JobId", "className": "cls_nowrap" },
            

        ],
        select: {
            'style': 'multi',
            selector: 'td:first-child input,td:last-child input'
        },
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null; var j = 1;
            //api.column(groupColumnIGridView, { page: 'current' }).data().each(function (group, i) {
            //if (last !== group) {
            //    var temp = [];
            //    var str_grouping = "";
            //    for (var x = 0; x < rows.data().length; x++) {
            //        if (rows.data()[x].GROUPING === group) {
            //            temp.push(rows.data()[x].ARTWORK_SUB_ID);
            //        }
            //    }
            //    if (temp.length > 0)
            //        str_grouping = temp.join("|");
            //    $(rows).eq(i).before('<tr class="group highlight"><td><input data-group-name="' + str_grouping + '" class="cls_chk_group" type="checkbox"/></td><td colspan="16">Group ' + j + '</td></tr>');
            //    last = group;
            //    j++;
            //}
            //});
            //$('.cls_cnt_igrid_view').text('(' + this.api().data().count() + ') ');
        },
        "createdRow": function (row, data, index) {
            //if (data.STATUS == "1")
            //    $(row).css("color", "#A20025");
        },
        "rowCallback": function (row, data, index) {

            //$(".cls_report_ImpactedMat_Desc #mat_select_all").prop('checked', false);

            //if (data.Changed_Action == 'Inactive' || data.Changed_Action == 'Master Inactive') {
            //    if (data.Changed_Tabname == 'MasBrand') {
            //        $(row).find('.cls_action').val('Master Inactive');
            //        $(row).find('.cls_new_desc').val('');

            //    } else {
            //        if (data.Char_NewValue == '') {
            //            $(row).find('.cls_action').val('Master Inactive');
            //            $(row).find('.cls_new_desc').val('');
            //        } else {
            //            $(row).find('.cls_action').val('Update Characteristic Master');
            //            $(row).find('.cls_new_desc').val(data.Char_NewValue);
            //        }

            //    }
            //} else if (data.Changed_Action == 'Re-Active' || data.Changed_Action == 'Master Re-Active') {
            //    $(row).find('.cls_action').val('Master Re-Active');
            //    $(row).find('.cls_new_desc').val('');
            //} else if (data.Changed_Action == 'Update Characteristic Master') {
            //    $(row).find('.cls_new_desc').val(data.Char_NewValue);
            //} else if (data.Changed_Action == 'Update' || data.Changed_Action == 'Update Characteristic Master') {
            //    if (data.Changed_Tabname == 'MasBrand') {
            //        $(row).find('.cls_action').val('Update Characteristic Master');
            //    } else {
            //        $(row).find('.cls_action').val('Update Characteristic Master');
            //    }
            //} else {
            //    $(row).find('.cls_new_desc').val(data.Char_Description);
            //}

        },
        //"drawCallback": function (settings) {

        //},
        //"initComplete": function (settings, json) {
        //    $('.cls_cnt_igrid_view').text('(' + json.data.length + ') ');
        //}
    });
    $("#table_report_impaced_wrapper .dt-buttons .buttons-excel").hide();

    // Handle click on "Select all" control
    $('#mat_select_all').on('click', function () {
        // Get all rows with search applied
        var rows = table_report_impaced.rows({ 'search': 'applied' }).nodes();
        // Check/uncheck checkboxes for all rows in the table
        $('input[type="checkbox"]', rows).prop('checked', this.checked);
    });

    // Handle click on checkbox to set state of "Select all" control
    $('#table_report_impaced tbody').on('change', 'input[type="checkbox"]', function () {
        // If checkbox is not checked
        if (!this.checked) {
            var el = $('#mat_select_all').get(0);
            // If "Select all" control is checked and has 'indeterminate' property
            if (el && el.checked && ('indeterminate' in el)) {
                // Set visual state of "Select all" control
                // as 'indeterminate'
                el.indeterminate = true;
            }
        }
    });

    // Handle form submission event
    $('#frm-example').on('submit', function (e) {
        var form = this;

        // Iterate over all checkboxes in the table
        table_report_impaced.$('input[type="checkbox"]').each(function () {
            // If checkbox doesn't exist in DOM
            if (!$.contains(document, this)) {
                // If checkbox is checked
                if (this.checked) {
                    // Create a hidden element
                    $(form).append(
                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', this.name)
                            .val(this.value)
                    );
                }
            }
        });
    });
}