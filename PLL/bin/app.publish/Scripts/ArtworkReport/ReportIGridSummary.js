
$(document).ready(function () {


    $(".cls_report_igridsummary .cls_igrids_btn_clr").click(function () {
        $('.cls_report_igridsummary .cls_txt_master_keyword').val('');
    });


    $('#table_report_igrids_all').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        lengthChange: false,
        scrollX: true,
        search: false,
    });


    $('#table_report_igrids_infogroup thead tr').clone(true).appendTo('#table_report_igrids_infogroup thead');
    $('#table_report_igrids_infogroup thead tr:eq(1) th').each(function (i) {
        if (i == 0) {
            $(this).html('');
        }
        else
        {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });


    $('#table_report_igrids_infogroup').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        lengthChange: false,
        scrollX: true,
        search: false,
    });

    $('#table_report_igrids thead tr').clone(true).appendTo('#table_report_igrids thead');
    $('#table_report_igrids thead tr:eq(1) th').each(function (i) {
        if (i == 0) {
            $(this).html('');
        } else if (i == 1 || i == 2) {
            $(this).addClass('cls_hide');      
        } else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    $('#table_report_igrids').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        lengthChange: false,
        scrollX: true,
        search: false,
    });

    //$('#table_report_igrids_all thead tr').clone(true).appendTo('#table_report_igrids_all thead');
    //$('#table_report_igrids_all thead tr:eq(1) th').each(function (i) {
    //    if (i == 0) {
    //        $(this).html('');
    //    } else {
    //        var title = $(this).text();
    //        $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
    //    }
    //});


   // $('.cls_report_igridsummary .cls_txt_master_keyword').prop('placeholder', 'Material No, Material Description, Document No, Brand');



    cleardata();
    first_load = false;
    $('.cls_report_igridsummary .cls_lov_action').on("change", function () {
        //var selections = $(this).select2('data');
        //var v = $(this)[0].value;
        //var isClear = true;
        var condition = $('.cls_report_igridsummary .cls_lov_action').val();
        switch (condition)
        {
            case "All":
                $('.cls_report_igridsummary .cls_lbl_mater_keywork_description').html('Material No, Material Description, Document No, Brand');
                break;
            case "Complete":
                $('.cls_report_igridsummary .cls_lbl_mater_keywork_description').html('Material No, Material Description, Document No, Brand');
                break;
            case "Info_group":           
                $('.cls_report_igridsummary .cls_lbl_mater_keywork_description').html('Material No, Material Description, Document No, Info Group');
                break;
        }
    });
   
    
    $(".cls_report_igridsummary .cls_btn_export_excel").click(function () {


        if (!$('.cls_table_report_igrids_all').hasClass('cls_hide')) {
            var criteria = "data.where=" + $('.cls_report_igridsummary .cls_txt_master_keyword').val() + "&data.first_load=0&data.export_excel=X";
            window.open("/excel/IGridSummaryAllReport?" + criteria, '_blank');
        } else if (!$('.cls_table_report_igrids').hasClass('cls_hide')) {
            $(".cls_table_report_igrids .buttons-excel").click();
        } else
        {
            $(".cls_table_report_igrids_infogroup .buttons-excel").click();
        }

        // $(".cls_report_warehouse .buttons-excel").unbind();
    });
    $(".cls_report_igridsummary .cls_igrids_btn_search").click(function (e) {
        //debugger;
        var columns, url;
        var condition = $('.cls_report_igridsummary .cls_lov_action').val();
        if (isEmpty($('.cls_report_igridsummary .cls_txt_master_keyword').val()) && condition== "All") {
            alertError2("Please input keyword.");

            e.preventDefault();
        }
        else {
            if (condition == "All") {
                //table_report_overview.ajax.reload();

                columns = [
                    //{
                    //    render: function (data, type, row, meta) {
                    //        return meta.row + meta.settings._iDisplayStart + 1;
                    //    }
                    //},
                    {
                        data: null,
                        defaultContent: '',
                        className: 'cls_hide',//'select-checkbox',
                        orderable: false
                    },

                    //{ data: "ID", "className": "cls_nowrap" },
                    //{ data: "Condition", "className": "cls_nowrap" },
                    //{ data: "RequestType", "className": "cls_nowrap" },
                    //{ data: "DocumentNo", "className": "cls_nowrap" },
                    //{ data: "DMSNo", "className": "cls_nowrap" },
                    //{ data: "Material", "className": "cls_nowrap" },
                    //{ data: "Description", "className": "cls_nowrap" },
                    //{ data: "MaterialGroup", "className": "cls_nowrap" },
                    //{ data: "Brand", "className": "cls_nowrap" },
                    //{ data: "fn", "className": "cls_nowrap" },
                    //{ data: "CreateOn", "className": "cls_nowrap" },
                    //{ data: "ActiveBy", "className": "cls_nowrap" },
                    //{ data: "StatusApp", "className": "cls_nowrap" },

                    { data: "ID", "className": "cls_td_width_50" },
                    { data: "Condition", "className": "cls_td_width_130" },
                    { data: "RequestType", "className": "cls_td_width_130" }, //cls_td_width_100
                    { data: "DocumentNo", "className": "cls_td_width_130" },
                    { data: "DMSNo", "className": "cls_td_width_130" },
                    { data: "Material", "className": "cls_td_width_150" }, //cls_td_width_240
                    { data: "Description", "className": "cls_td_width_200" }, //cls_td_width_240
                    { data: "MaterialGroup", "className": "cls_td_width_70" }, //cls_td_width_50
                    { data: "Brand", "className": "cls_td_width_150" },
                    { data: "fn", "className": "cls_td_width_130" }, //cls_td_width_100
                    { data: "Submitdate", "className": "cls_td_width_130" },
                    { data: "ActiveBy", "className": "cls_td_width_200" },
                    { data: "StatusApp", "className": "cls_td_width_130" }, //cls_td_width_100
                    //{ data: "Remark", "className": "cls_nowrap" },

                ];
                // url = suburl + "/api/report/igridsummaryreport?data.where=" + $('.cls_report_igridsummary .cls_txt_master_keyword').val()
                url = suburl + "/api/report/igridsummarygroupreport?data.where=" + $('.cls_report_igridsummary .cls_txt_master_keyword').val()
                    + "&data.first_load=" + first_load;
                bindDataReportAll("0", columns, url, "#table_report_igrids_all", ".cls_table_report_igrids_all");
                //builddata(false, "0", columns, url, "#table_report_igrids", ".cls_table_report_igrids");
                //e.preventDefault();
            } else if (condition == "Complete") {
                columns = [
                    //{
                    //    render: function (data, type, row, meta) {
                    //        return meta.row + meta.settings._iDisplayStart + 1;
                    //    }
                    //},
                    //{
                    //    data: null,
                    //    defaultContent: '',
                    //    className: 'cls_hide',//'select-checkbox',
                    //    orderable: false
                    //},
                    { data: "Id", "className": "cls_nowrap" },
                    { data: "Title", "className": "cls_nowrap cls_hide" },
                    { data: "RequestType", "className": "cls_nowrap cls_hide" },
                    { data: "DocumentNo", "className": "cls_nowrap cls_DOCUMENTNO" },
                    { data: "DMSNo", "className": "cls_nowrap" },
                    { data: "Material", "className": "cls_nowrap" },
                    { data: "Description", "className": "cls_nowrap" },
                    { data: "MaterialGroup", "className": "cls_nowrap" },
                    { data: "Brand", "className": "cls_nowrap" },
                    { data: "CreateOn", "className": "cls_nowrap" },
                    { data: "CreateBy", "className": "cls_nowrap" },
                    { data: "Assignee", "className": "cls_nowrap" },
                    { data: "FinalInfoGroup", "className": "cls_nowrap" },
                    //{ data: "Remark", "className": "cls_nowrap" },
                ];
                url = suburl + "/api/report/igridcompletereport?data.where=" + $('.cls_report_igridsummary .cls_txt_master_keyword').val()
                    + "&data.first_load=" + first_load;
                // builddata(false, "0", columns, url, "#table_report_igrids", ".cls_table_report_igrids");
                bindDataReportComplete("0", columns, url, "#table_report_igrids", ".cls_table_report_igrids");
                //e.preventDefault();
            }
            else if (condition == "Info_group")
            {
                columns = [

                    //{
                    //    data: null,
                    //    defaultContent: '',
                    //   // className: 'cls_hide',//'select-checkbox',
                    //    orderable: false
                    //},
                    { data : "ID", "className": "cls_nowrap" },
                    { data: "CONDITION", "className": "cls_nowrap" },
                    { data: "REQUESTTYPE", "className": "cls_nowrap" },
                    { data: "DOCUMENTNO", "className": "cls_nowrap cls_DOCUMENTNO" },
                    { data: "DMSNO", "className": "cls_nowrap" },
                    { data: "MATERIAL", "className": "cls_nowrap" },
                    { data: "DESCRIPTION", "className": "cls_nowrap" },
                    { data: "MATERIALGROUP", "className": "cls_nowrap" },
                    { data: "BRAND", "className": "cls_nowrap" },
                    { data: "ASSIGN", "className": "cls_nowrap" },
                    { data: "CREATEON", "className": "cls_nowrap" },
                    { data: "CREATEBY", "className": "cls_nowrap" },
                    { data: "FINALINFOGROUP", "className": "cls_nowrap" },
                    //{ "data": "REFERENCEMATERIAL", "className": "cls_nowrap" },
                    //{ "data": "VENDER", "className": "cls_nowrap" },
                    //{ "data": "VENDERDESCRIPTION", "className": "cls_nowrap" },
                ];
                url = suburl + "/api/report/igridinfogroupreport?data.where=" + $('.cls_report_igridsummary .cls_txt_master_keyword').val()
                    + "&data.first_load=" + false,           
                    bindDataReportInfoGroup(false, columns, url, "#table_report_igrids_infogroup", ".cls_table_report_igrids_infogroup");
            }
        }
    });
    bind_report_igridsummary_main("1")
    //builddata(false, "0");
});


function bind_report_igridsummary_main(first_load) {

    columns = [
        //{
        //    render: function (data, type, row, meta) {
        //        return meta.row + meta.settings._iDisplayStart + 1;
        //    }
        //},
        {
            data: null,
            defaultContent: '',
            className: 'select-checkbox',
            orderable: false
        },
        { data: "ID", "className": "cls_nowrap" },
        { data: "Condition", "className": "cls_nowrap" },
        { data: "RequestType", "className": "cls_nowrap" },
        { data: "DocumentNo", "className": "cls_nowrap" },
        { data: "DMSNo", "className": "cls_nowrap" },
        { data: "Material", "className": "cls_nowrap" },
        { data: "Description", "className": "cls_nowrap" },
        { data: "MaterialGroup", "className": "cls_nowrap" },
        { data: "Brand", "className": "cls_nowrap" },
        { data: "Assignee", "className": "cls_nowrap" },
        { data: "CreateOn", "className": "cls_nowrap" },
        { data: "ActiveBy", "className": "cls_nowrap" },
        { data: "StatusApp", "className": "cls_nowrap" },
        { data: "Remark", "className": "cls_nowrap" },

    ];
    url = suburl + "/api/report/igridsummaryreport?data.where=" + $('.cls_report_igridsummary .cls_txt_master_keyword').val()
        + "&data.first_load=" + first_load;
    builddata(false, "0", columns, url, "#table_report_igrids_all", ".cls_table_report_igrids_all");
        //e.preventDefault();
}
function cleardata() {
    $('.cls_table_report_igrids_all').addClass('cls_hide');
    $('.cls_table_report_igrids').addClass('cls_hide');
    $('.cls_table_report_igrids_infogroup').addClass('cls_hide');
}
function builddata(serverSide, autosearch, columns, url, table, divclass ) {
    var groupColumn = 4;
    cleardata();
    $(divclass).removeClass('cls_hide');
    var table_report_overview = $(table).DataTable()
    table_report_overview.destroy();
    table_report_overview = $(table).DataTable({

        "scrollY": "540px",
        "scrollCollapse": true,
        paging: false,
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
                url: url,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //"columnDefs": [
        //    { "visible": false, "targets": groupColumnPPView },
        //    { "orderable": false, "targets": 0 },
        //],
        "order": [[1, 'desc']],
        "processing": true,
        "lengthChange": false,
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "ordering": true,
        "info": true,
        "searching": false,
        "scrollX": true,
        "autoWidth": false,
        dom: 'Bfrtip',
        buttons: [
            {
                title: 'IGrid view',
                extend: 'excelHtml5',
                exportOptions: {
                    //columns: [1, 2, 3, 4, 5, 6],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            }
        ],
        columns: columns,
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null; var j = 1;
            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
            if (last !== group) {
                var temp = [];
                var str_grouping = "";
                for (var x = 0; x < rows.data().length; x++) {
                    if (rows.data()[x].GROUPING === group) {
                        temp.push(rows.data()[x].DocumentNo);
                    }
                }
                if (temp.length > 0)
                    str_grouping = temp.join("|");
                $(rows).eq(i).before('<tr class="group highlight"><td><input data-group-name="' + str_grouping + '" class="cls_chk_group" type="checkbox"/></td><td colspan="16"> ' + group + '</td></tr>');
                last = group;
                j++;
            }
            });
            $('.cls_cnt_igrid_view').text('(' + this.api().data().count() + ') ');
        },
        "createdRow": function (row, data, index) {
            if (data.STATUS == "1")
                $(row).css("color", "#A20025");
        },
        //"drawCallback": function (settings) {

        //},
        //"initComplete": function (settings, json) {
        //    $('.cls_cnt_igrid_view').text('(' + json.data.length + ') ');
        //}
    });
    $(table + "_wrapper .dt-buttons .buttons-excel").hide();
}



function bindDataReportAll(f_fistload, columns, url, table, divclass) {
    
    cleardata();
    $(divclass).removeClass('cls_hide');

    var groupColumn = 4;
    var table_report_history = $(table).DataTable()
    table_report_history.destroy();
    table_report_history = $(table).DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHaeder: true,
        paging: false,  
        //stateSave: false,
        ajax: function (data, callback, settings) {

            $.ajax({
                url: url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
   
        "processing": true,
     
        "info": true,
        "searching": false,
        "scrollX": true,
        "scrollY": "540px",
        "scrollCollapse": true,
      
        columns: columns,
        "rowCallback": function (row, data, index) {
            $(row).addClass('highlight');
            $('td:eq(2)', row).html(data.DocumentNo);

            //$('td:eq(2)', row).html('<a target="_blank" href="' + suburl + '/IGrid/' + data.ID + '"> ' + data.DocumentNo + ' </a>');
            //$('td:eq(4)', row).html('<a target="_blank" href="' + suburl + '/IGrid/' + data.ID + '"> ' + data.DocumentNo + ' </a>');



        },

        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var rows_data = api.rows({ page: 'current' }).data();
            var last = null;


            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {


                if (rows_data[i].data !== null) {


                    for (var x = 0; x < rows_data[i].data.length; x++) {

                        $(rows).eq(i).after('<tr><td class="cls_hide cls_nowrap">'
                            + '</td><td class="cls_td_width_50">' + rows_data[i].data[x].Id
                            + '</td><td class="cls_td_width_130">' + rows_data[i].data[x].Condition
                            + '</td><td class="cls_td_width_130">' + rows_data[i].data[x].RequestType
                            + '</td><td class="cls_td_width_130">' + '<a target="_blank" href="' + suburl + '/IGrid/' + rows_data[i].data[x].Id + '"> ' + rows_data[i].data[x].DocumentNo + ' </a>'  //rows_data[i].data[x].DocumentNo
                            + '</td><td class="cls_td_width_130">' + rows_data[i].data[x].DMSNo
                            + '</td><td class="cls_td_width_150">' + rows_data[i].data[x].Material
                            + '</td><td class="cls_td_width_200">' + rows_data[i].data[x].Description
                            + '</td><td class="cls_td_width_70">' + rows_data[i].data[x].MaterialGroup
                            + '</td><td class="cls_td_width_150">' + rows_data[i].data[x].Brand
                            + '</td><td class="cls_td_width_130">' + rows_data[i].data[x].fn
                            + '</td><td class="cls_td_width_130">' + rows_data[i].data[x].Submitdate
                            + '</td><td class="cls_td_width_200">' + rows_data[i].data[x].ActiveBy
                            + '</td><td class="cls_td_width_130">' + rows_data[i].data[x].StatusApp
                           // + '</td><td class="cls_td_width_130">' //+ rows_data[i].data[x].Remark
                            + '</td></tr>');


                     

                    }

                }

            });

           // api.table.column.adjust().draw();
         
        },

    });

    $(table + "_wrapper .dt-buttons .buttons-excel").hide();
    //$(table + "_filter").hide();

    //$(table_report.table().container()).on('keyup', 'input', function () {
    //    table_report
    //        .column($(this).data('index'))
    //        .search(this.value, true)
    //        .draw();
    //});


    //$(table).on('draw.dt', function () {
    //    ////var info = table.page.info();
    //    ////$('#pageInfo').html('Showing page: ' + info.page + ' of ' + info.pages);
    //    //table_report_history.columns.adjust().draw();
    //    //table.columns.adjust().draw();
      
    //});

  table_report_history.columns.adjust().draw();

}


function bindDataReportComplete(f_fistload, columns, url, table, divclass) {

    cleardata();
    $(divclass).removeClass('cls_hide');

   
    var table_report = $(table).DataTable()
    table_report.destroy();
    table_report = $(table).DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHaeder: true,
        paging: false,  
        //stateSave: false,
        ajax: function (data, callback, settings) {

            $.ajax({
                url: url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },

        "processing": true,

        "info": true,
        "searching": true,
        "scrollX": true,
        "scrollY": "540px",
        "scrollCollapse": true,

        columns: columns,
        dom: 'Bfrtip',
        buttons: [
            {
                title: 'IGrid Summary Complete Report',
                extend: 'excelHtml5',
                exportOptions: {
                    //columns: [0,1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            }, //"pageLength"
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_DOCUMENTNO').html('<a target="_blank" href="' + suburl + '/IGrid/' + data.Id + '"> ' + data.DocumentNo + ' </a>');


            //$(row).addClass('highlight');
            //$('td:eq(2)', row).html(data.DocumentNo);

        },

    });

    $(table + "_wrapper .dt-buttons .buttons-excel").hide();
    $(table + "_filter").hide();

    $(table_report.table().container()).on('keyup', 'input', function () {
        table_report
            .column($(this).data('index'))
            .search(this.value, true)
            .draw();
    });

    table_report.columns.adjust().draw();


}


function bindDataReportInfoGroup(f_fistload, columns, url, table, divclass) {

    cleardata();
    $(divclass).removeClass('cls_hide');


    var table_report = $(table).DataTable()
    table_report.destroy();

  
    table_report = $(table).DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHaeder: true,
        paging: false,  
        //stateSave: false,
        ajax: function (data, callback, settings) {

            $.ajax({
                url: url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },

        "processing": true,

        "info": true,
        "searching": true,
        "scrollX": true,
        "scrollY": "540px",
        "scrollCollapse": true,

        columns: columns,
        dom: 'Bfrtip',
        buttons: [
            {
                title: 'IGrid Summary Infogroup Report',
                extend: 'excelHtml5',
                exportOptions: {
                    //columns: [0,1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            },// "pageLength"
        ],
        "rowCallback": function (row, data, index) {
            $(row).find('.cls_DOCUMENTNO').html('<a target="_blank" href="' + suburl + '/IGrid/' + data.ID + '"> ' + data.DOCUMENTNO + ' </a>');
            //$('td:eq(2)', row).html(data.DocumentNo);

        },

    });

    $(table + "_wrapper .dt-buttons .buttons-excel").hide();
    $(table + "_filter").hide();

    $(table_report.table().container()).on('keyup', 'input', function () {
        table_report
            .column($(this).data('index'))
            .search(this.value, true)
            .draw();
    });

   table_report.columns.adjust().draw();

    //table_report.columns.adjust().responsive.recalc();
    //table_report.fixedHeader.adjust();


}