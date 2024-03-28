var first_load = true;
var columns, url;
var columns_export_excel, rowCallback;
$(document).ready(function () {
    //bind_lov('.cls_report_history .cls_lov_layout', '/api/lov/layout', 'data.DISPLAY_TXT', 'cls_lov_layout');
    //debugger;
    //bind_report_history(false, "0");
    var table_report_history;
    cleardata();
    //$('.cls_table_report_history').removeClass('cls_hide');
    //$("#table_report_history_wrapper .dt-buttons .buttons-excel").hide();
    $('.cls_report_overview .cls_lov_condition').on("change", function () {
        var selections = $(this).select2('data');
        var v = $(this)[0].value;
        var isClear = true;
    });
    $('.cls_lov_layout').on("change", function () {
        //debugger;
       // var layout = $(this).val();
        //$('#table_report_history tr').eq(0).find('th').eq(0).html(layout);
        //var tb  = document.getElementById("Log_PA_ModifyBy");
        //tb.innerHTML = tb.innerHTML.replace("PA", layout).replace("PG", layout);
        
        //var headers = document.getElementById('myTable').getElementsByTagName("th");
        //for (var i = 0; i < headers.length; i++) {
        //    headers[i].innerHTML = headers[i].innerHTML.replace("PA", layout).replace("PG", layout);
        //}





    });


    
    $(".cls_report_history .cls_his_btn_clr").click(function () {
        $('.cls_report_history .cls_txt_master_keyword').val('');

        $('.cls_report_history .cls_his_req_from').val(GetFirstDateOfMonth());
        $('.cls_report_history .cls_his_req_to').val(GetCurrentDate2());


     
    });

    $(".cls_report_history .cls_btn_export_excel").click(function () {



        var FrDt = $('.cls_report_history .cls_his_req_from').val();
        var ToDt = $('.cls_report_history .cls_his_req_to').val();
        var where = "&data.FrDt=" + FrDt + "&data.ToDt=" + ToDt + "&data.first_load=" + '0' + "&data.EXPORT_EXCEL='X'"
           

        if (!$('.cls_table_report_kpi_summary').hasClass('cls_hide'))
        {

            if (checkRequireData())
            {
                var layout = $('.cls_report_history .cls_lov_layout').val();
                window.open("/excel/IGridHistorySummarizeOwnerReport?data.LayOut=" + layout + where, '_blank');                      
            }

         
        } else if (!$('.cls_table_report_summary_approve').hasClass('cls_hide'))
        {
            if (checkRequireData()) {
                var layout = $('.cls_report_history .cls_lov_layout').val();
                var Str_Layout = (layout == "PA") ? "PA_SUMApprove" : "PG_SUMApprove";
                window.open("/excel/IGridHistorySummarizeApproveReport?data.LayOut=" + Str_Layout + where, '_blank');
            }
        }
        else
        {
            var layout = $('.cls_report_history .cls_lov_layout').val();
            if (layout == "PA")
                $(".cls_table_report_history .buttons-excel").click();
            else
                $(".cls_table_report_history_pg .buttons-excel").click();
        }

     
        // $(".cls_report_warehouse .buttons-excel").unbind();
    });
    $('.cls_his_btn_approve').click(function (e) {
        if (checkRequireData()) {
            bind_report_history_approve("0")
        } else {
            bind_report_history_approve("1")
        }
      
    });

    $('.cls_his_btn_owner').click(function (e) {
        if (checkRequireData()) {
            bind_report_history_owner("0")
        } else {
            bind_report_history_owner("1")
        }
       
    });
    
    $('.cls_his_btn_over').click(function (e) {
        $(".cls_his_btn_search").click();
    });


    $('.cls_his_btn_search').click(function (e) {

        if (checkRequireData())
        {
            bind_report_history_main("0")
        } else
        {
            bind_report_history_main("1")
        }

    });

    bind_report_history_main("1")

});



function checkRequireData() {
    var f_pass = false;
    var rdatefrom = $('.cls_report_history .cls_his_req_from').val();
    var rdateto = $('.cls_report_history .cls_his_req_to').val();
    // var columns;
    if (!isEmpty(rdatefrom) && !isEmpty(rdateto)) {
        f_pass = true;
    }
    else {
        alertError2("Please input request date from and request date to.");

        //e.preventDefault();
    }
    return f_pass;
}

function bind_report_history_approve(fist_load)
{

    var layout = $('.cls_report_history .cls_lov_layout').val();

   
    $('.cls_approve_role').html(layout);
    $('.cls_approve_role_fullname').html(layout + ' FullName');

    columns = [
        {
            data: null,
            //defaultContent: '',
            className: 'cls_hide',//'select-checkbox',
            orderable: false
        },

        { "data": "MODIFY_BY_USERNAME", "className": "cls_nowrap" },
        { "data": "MODIFY_BY_FULLNAME", "className": "cls_nowrap" },
        { "data": "CREATE_USERNAME", "className": "cls_nowrap" },
        { "data": "CREATE_FULLNAME", "className": "cls_nowrap" },
        { "data": "", "className": "cls_nowrap" },

        //{ "data": "Log_" + layout + "_ModifyBy", "className": "cls_nowrap" },
        //{ "data": "Log_" + layout + "_ModifyByFullName", "className": "cls_nowrap" },
        //{ "data": "CreateBy", "className": "cls_nowrap" },
        //{ "data": "CreateByFullName", "className": "cls_nowrap" },
        //{ "data": "Count", "className": "cls_nowrap" },
    ];
    var Str_Layout = (layout == "PA") ? "PA_SUMApprove" : "PG_SUMApprove";
    bind_ajax_summary_approve(fist_load, columns, suburl + '/api/report/historyreport_kpisummary_approve?data.LayOut=' + Str_Layout, "#table_report_summary_approve", ".cls_table_report_summary_approve");

}


function bind_report_history_owner(first_load)
{
    var layout = $('.cls_report_history .cls_lov_layout').val();

    $('.cls_owner_role').html(layout);
    $('.cls_owner_role_fullname').html(layout + ' FullName');

    if (layout == "PA")       
        $('.cls_owner_all_recored').html('No. of all created records');
    else
        $('.cls_owner_all_recored').html('No. of all assigned records');

    columns = [
        //{
        //    render: function (data, type, row, meta) {
        //        return meta.row + meta.settings._iDisplayStart + 1;
        //    }
        //},
        {
            data: null,
            //defaultContent: '',
            className: 'cls_hide',//'select-checkbox',
            orderable: false
        },

        //{ "data": "Log_" + layout + "_ModifyBy", "className": "cls_nowrap" },
        //{ "data": "Log_" + layout + "_ModifyByFullName", "className": "cls_nowrap" },
        //{ "data": "CreateBy", "className": "cls_nowrap" },
        //{ "data": "CreateByFullName", "className": "cls_nowrap" },
        //{ "data": "Count", "className": "cls_nowrap" },
        //{ "data": "SAPMat_Count", "className": "cls_nowrap" },
        //{ "data": "Count_All", "className": "cls_nowrap" },

        { "data": "CREATE_USERNAME", "className": "cls_nowrap" },
        { "data": "CREATE_FULLNAME", "className": "cls_nowrap" },
        { "data": "MODIFY_BY_USERNAME", "className": "cls_nowrap" },
        { "data": "MODIFY_BY_FULLNAME", "className": "cls_nowrap" },
        { "data": "MODIFYED_RECOORD", "className": "cls_nowrap" },
        { "data": "CREATED_RECOORD", "className": "cls_nowrap" },
        { "data": "PECENTAGE_ERROR", "className": "cls_nowrap" },

    ];
    bind_ajax_summary_owner(first_load, columns, suburl + '/api/report/historyreport_kpisummary_owner?data.LayOut=' + layout, "#table_report_kpi_summary", ".cls_table_report_kpi_summary");

}




function bind_report_history_main(first_load)
{
    //var rdatefrom = $('.cls_report_history .cls_his_req_from').val();
    //var rdateto = $('.cls_report_history .cls_his_req_to').val();
    var layout = $('.cls_report_history .cls_lov_layout').val();
    var table, divclass;

    //// var columns;
    //if (!isEmpty(rdatefrom) && !isEmpty(rdateto)) {

        //$('.cls_layout').html($('.cls_layout').html().replace('PA', layout).replace('PG', layout));
        //$('.cls_modifyby').html('Log_' + layout + '_ModifyBy');
        //$('.cls_modifybyfirstname').html('Log_' + layout + '_ModifyByFirstName');
        //$('.cls_modifybyfn').html('Log_' + layout + '_ModifyByfn');
        //$('.cls_modifyon').html('Log_' + layout + '_ModifyOn');


    if (layout == "PA") {
        columns_export_excel = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82];
        columns = [

            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox cls_hide',
                orderable: false
            },

            { "data": "Log_Id", "className": "cls_nowrap" },
            { "data": "Log_" + layout + "_ModifyBy", "className": "cls_nowrap" },
            { "data": "Log_" + layout + "_ModifyByFirstName", "className": "cls_nowrap" },
            { "data": "Log_" + layout + "_ModifyByfn", "className": "cls_nowrap" },
            { "data": "Log_" + layout + "_ModifyOn", "className": "cls_nowrap" },
            { "data": "Material", "className": "cls_nowrap" },
            { "data": "Description", "className": "cls_nowrap" },
            { "data": "CreateBy", "className": "cls_nowrap" },
            { "data": "CreateByFirstName", "className": "cls_nowrap" },
            { "data": "CreateByfn", "className": "cls_nowrap" },
            { "data": "CreateOn", "className": "cls_nowrap" },
            { "data": "Final_ApprovedDate", "className": "cls_nowrap" },
            { "data": "Final_ApprovedBy", "className": "cls_nowrap" },
            { "data": "Final_ApprovedName", "className": "cls_nowrap" },
            { "data": "StatusApp", "className": "cls_nowrap" },
            { "data": "Condition", "className": "cls_nowrap" },
            { "data": "Old_Brand", "className": "cls_nowrap" },
            { "data": "Brand", "className": "cls_nowrap" },
            { "data": "Old_ChangePoint", "className": "cls_nowrap" },
            { "data": "ChangePoint", "className": "cls_nowrap" },
            { "data": "Old_MaterialGroup", "className": "cls_nowrap" },
            { "data": "MaterialGroup", "className": "cls_nowrap" },
            { "data": "Old_PrimarySize", "className": "cls_nowrap" },
            { "data": "PrimarySize", "className": "cls_nowrap" },
            { "data": "Old_ContainerType", "className": "cls_nowrap" },
            { "data": "ContainerType", "className": "cls_nowrap" },
            { "data": "Old_LidType", "className": "cls_nowrap cls_Old_LidType" },
            { "data": "LidType", "className": "cls_nowrap cls_LidType" },
            { "data": "Old_PackingStyle", "className": "cls_nowrap" },
            { "data": "PackingStyle", "className": "cls_nowrap" },
            { "data": "Old_Packing", "className": "cls_nowrap" },
            { "data": "Packing", "className": "cls_nowrap" },
            { "data": "Old_StyleofPrinting", "className": "cls_nowrap" },
            { "data": "StyleofPrinting", "className": "cls_nowrap" },
            { "data": "Old_ProductCode", "className": "cls_nowrap" },
            { "data": "ProductCode", "className": "cls_nowrap" },
            { "data": "Old_FAOZone", "className": "cls_nowrap" },
            { "data": "FAOZone", "className": "cls_nowrap" },
            { "data": "Old_Plant", "className": "cls_nowrap" },
            { "data": "Plant", "className": "cls_nowrap" },
            { "data": "Old_PMScolour", "className": "cls_nowrap" },
            { "data": "PMScolour", "className": "cls_nowrap" },
            { "data": "Old_Processcolour", "className": "cls_nowrap" },
            { "data": "Processcolour", "className": "cls_nowrap" },
            { "data": "Old_Totalcolour", "className": "cls_nowrap" },
            { "data": "Totalcolour", "className": "cls_nowrap" },
            { "data": "Old_PlantRegisteredNo", "className": "cls_nowrap" },
            { "data": "PlantRegisteredNo", "className": "cls_nowrap" },
            { "data": "Old_CompanyNameAddress", "className": "cls_nowrap" },
            { "data": "CompanyNameAddress", "className": "cls_nowrap" },
            { "data": "Old_Symbol", "className": "cls_nowrap" },
            { "data": "Symbol", "className": "cls_nowrap" },
            { "data": "Old_CatchingArea", "className": "cls_nowrap" },
            { "data": "CatchingArea", "className": "cls_nowrap" },
            { "data": "Old_CatchingPeriodDate", "className": "cls_nowrap" },
            { "data": "CatchingPeriodDate", "className": "cls_nowrap" },
            { "data": "Old_PrintingStyleofPrimary", "className": "cls_nowrap" },
            { "data": "PrintingStyleofPrimary", "className": "cls_nowrap" },
            { "data": "Old_PrintingStyleofSecondary", "className": "cls_nowrap" },
            { "data": "PrintingStyleofSecondary", "className": "cls_nowrap" },
            { "data": "Old_Typeof", "className": "cls_nowrap" },
            { "data": "Typeof", "className": "cls_nowrap" },
            { "data": "Old_TypeofCarton2", "className": "cls_nowrap" },
            { "data": "TypeofCarton2", "className": "cls_nowrap" },
            { "data": "Old_DMSNo", "className": "cls_nowrap" },
            { "data": "DMSNo", "className": "cls_nowrap" },
            { "data": "Old_TypeofPrimary", "className": "cls_nowrap" },
            { "data": "TypeofPrimary", "className": "cls_nowrap" },
            { "data": "Old_Direction", "className": "cls_nowrap" },
            { "data": "Direction", "className": "cls_nowrap" },
            { "data": "Old_PlantAddress", "className": "cls_nowrap" },
            { "data": "PlantAddress", "className": "cls_nowrap" },

            { "data": "Old_Catching_Method", "className": "cls_nowrap" },
            { "data": "Catching_Method", "className": "cls_nowrap" },
            { "data": "Old_Scientific_Name", "className": "cls_nowrap" },
            { "data": "Scientific_Name", "className": "cls_nowrap" },
            { "data": "Old_Specie", "className": "cls_nowrap" },
            { "data": "Specie", "className": "cls_nowrap" },

            { "data": "Refnumber", "className": "cls_nowrap" },
            { "data": "Remark", "className": "cls_nowrap" },
            { "data": "Note", "className": "cls_nowrap" },
            { "data": "CountTotal", "className": "cls_nowrap" },
        ];
        rowCallback = function (row, data, index) {
            $(row).find('.cls_Old_LidType').text(data.Old_LidType);
            $(row).find('.cls_LidType').text(data.LidType);
        };
        table = "#table_report_history";
        divclass = ".cls_table_report_history";
      
    } else
    {
        columns_export_excel = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40];
        columns = [
           
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox cls_hide',
                orderable: false
            },

            { "data": "Log_Id", "className": "cls_nowrap" },
            { "data": "Log_" + layout + "_ModifyBy", "className": "cls_nowrap" },
            { "data": "Log_" + layout + "_ModifyByFirstName", "className": "cls_nowrap" },
            { "data": "Log_" + layout + "_ModifyByfn", "className": "cls_nowrap" },
            { "data": "Log_" + layout + "_ModifyOn", "className": "cls_nowrap" },
            { "data": "Material", "className": "cls_nowrap" },
            { "data": "Description", "className": "cls_nowrap" },
            { "data": "Assignee", "className": "cls_nowrap" },
            { "data": "AssigneeFirstName", "className": "cls_nowrap" },
            { "data": "CreateBy", "className": "cls_nowrap" },
            { "data": "CreateByFirstName", "className": "cls_nowrap" },
            { "data": "CreateByfn", "className": "cls_nowrap" },
            { "data": "CreateOn", "className": "cls_nowrap" },
            { "data": "Final_ApprovedDate", "className": "cls_nowrap" },
            { "data": "Final_ApprovedBy", "className": "cls_nowrap" },
            { "data": "Final_ApprovedName", "className": "cls_nowrap" },
            { "data": "StatusApp", "className": "cls_nowrap" },
            { "data": "Condition", "className": "cls_nowrap" },

            { "data": "Old_Grandof", "className": "cls_nowrap" },
            { "data": "Grandof", "className": "cls_nowrap" },
            { "data": "Old_SheetSize", "className": "cls_nowrap" }, 
            { "data": "SheetSize", "className": "cls_nowrap" },
            { "data": "Old_Vendor", "className": "cls_nowrap" },
            { "data": "Vendor", "className": "cls_nowrap" },
            { "data": "Old_Flute", "className": "cls_nowrap" },
            { "data": "Flute", "className": "cls_nowrap" },
            { "data": "Old_Dimension", "className": "cls_nowrap" },
            { "data": "Dimension", "className": "cls_nowrap" },
            { "data": "Old_RSC", "className": "cls_nowrap" },
            { "data": "RSC", "className": "cls_nowrap" },
            { "data": "Old_Accessories", "className": "cls_nowrap" },
            { "data": "Accessories", "className": "cls_nowrap" },
            { "data": "Old_PrintingSystem", "className": "cls_nowrap" },
            { "data": "PrintingSystem", "className": "cls_nowrap" },
            { "data": "Old_RollSheet", "className": "cls_nowrap" },
            { "data": "RollSheet", "className": "cls_nowrap" },
          

            { "data": "Refnumber", "className": "cls_nowrap" },
            { "data": "Remark", "className": "cls_nowrap" },
            { "data": "Note", "className": "cls_nowrap" },
            { "data": "CountTotal", "className": "cls_nowrap" },
        ];
        rowCallback = function (row, data, index) {

        };
        table = "#table_report_history_pg";
        divclass = ".cls_table_report_history_pg";
    }
    bind_report_history(false, first_load, columns, suburl + '/api/report/historyreport?data.LayOut=' + layout, table, divclass);

   

}

function cleardata() {
    $('.cls_table_report_history').addClass('cls_hide');
    $('.cls_table_report_history_pg').addClass('cls_hide');
    $('.cls_table_report_kpi_summary').addClass('cls_hide');
    $('.cls_table_report_summary_approve').addClass('cls_hide');
}

function bind_report_history(serverSide, autosearch, columns, url, table, divclass) {
    cleardata();
    $(divclass).removeClass('cls_hide');
    //var layout = $('.cls_report_history .cls_lov_layout').val();
    var FrDt = $('.cls_report_history .cls_his_req_from').val();
    var ToDt = $('.cls_report_history .cls_his_req_to').val();

    //var groupColumn = 1;
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
                    url: url+ "&data.FrDt=" + FrDt
                        + "&data.ToDt=" + ToDt
                    //+ "&data.rd_number=" + $('.cls_report_history .cls_txt_master_keyword').val()
                        + "&data.first_load=" + autosearch
                ,
                //url: suburl + "/api/report/historyreport",
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //"columnDefs": [{
        //    "searchable": false,
        //    "orderable": false,
        //    "targets": 0
        //}],
        "order": [[1, 'asc']],
        "processing": true,
        //"lengthChange": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "scrollX": true,
        "scrollY": "540px",
        "scrollCollapse": true,
        dom: 'Bfrtip',
        buttons: [
            {
               
                title: 'IGrid Overall History Log Report',
                extend: 'excelHtml5',
                exportOptions: {
                    columns: columns_export_excel,
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            },  //"pageLength"
        ],
        
        columns: columns,
        "rowCallback": rowCallback,
        
    });
   // $(table + "_filter").hide();
    $(table + "_wrapper .dt-buttons .buttons-excel").hide();
    if ($('.cls_report_history .cls_txt_master_keyword').val() != "")
        table_report_history.search($('.cls_report_history .cls_txt_master_keyword').val()).draw();
    //debugger;
    //$(table).filterTable({
    //        quickList: [
    //            'NON'
    //        ],
    //        quickListClear: '× clear filter'
    //    });
    
    //$(".cls_report_track_view").change();
}





function bind_ajax_summary_owner(first_load, columns, url, table, divclass) {
    cleardata();
    $(divclass).removeClass('cls_hide');
   
    var FrDt = $('.cls_report_history .cls_his_req_from').val();
    var ToDt = $('.cls_report_history .cls_his_req_to').val();

    var groupColumn = 1;
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
                url: url + "&data.FrDt=" + FrDt
                    + "&data.ToDt=" + ToDt               
                    + "&data.first_load=" + first_load
                ,          
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //"columnDefs": [{
        //    "searchable": false,
        //    "orderable": false,
        //    "targets": 0
        //}],
        // "order": [[1, 'asc']],
        "processing": true,
        //"lengthChange": true,
        //"ordering": false,
        "info": true,
        "searching": true,
        "scrollX": true,
        "scrollY": "540px",
        "scrollCollapse": true,
        //dom: 'Bfrtip',
        //buttons: [
        //    {
        //        title: 'IGrid Summarize by Owner Level Report',
        //        extend: 'excelHtml5',
        //        exportOptions: {
        //            //columns: [1, 2, 3, 4, 5, 6],
        //            format: {
        //                body: function (data, row, column, node) {
        //                    return data;
        //                }
        //            }
        //        }
        //    }, "pageLength"
        //],

        columns: columns,
        "rowCallback": function (row, data, index) {
         

        },

        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var rows_data = api.rows({ page: 'current' }).data();
            var last = null;

       
            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {


                if (rows_data[i].data !== null) {

                    if ($('.cls_report_history .cls_lov_layout').val() == "PA")
                    {
                        $(rows).eq(i).before('<tr style="background-color:lightgray"><td class="cls_hide">'                    
                            + '</td><td class="cls_nowrap">' + 'PA FullName:'
                            + '</td><td class="cls_nowrap">' + rows_data[i].CREATE_FULLNAME
                            + '</td><td class="cls_nowrap">'
                            + '</td><td class="cls_nowrap">'
                            + '</td><td class="cls_nowrap">'
                            + '</td><td class="cls_nowrap">'
                            + '</td><td>'
                            + '</td></tr>');
                    }
                    else
                    {
                        $(rows).eq(i).before('<tr style="background-color:lightgray"><td class="cls_hide">'
                            + '</td><td class="cls_nowrap">' + 'PG FullName:'
                            + '</td><td class="cls_nowrap">' + rows_data[i].CREATE_FULLNAME
                            + '</td><td class="cls_nowrap">'
                            + '</td><td class="cls_nowrap">'
                            + '</td><td class="cls_nowrap">'
                            + '</td><td class="cls_nowrap">'
                            + '</td><td>'
                            + '</td></tr>');
                    }
                  

                    for (var x = 0; x < rows_data[i].data.length; x++) {
                        
                        $(rows).eq(i).before('<tr><td class="cls_hide">'
                             
                                + '</td><td class="cls_nowrap">' + rows_data[i].data[x].CREATE_USERNAME
                                + '</td><td class="cls_nowrap">' + rows_data[i].data[x].CREATE_FULLNAME
                                + '</td><td class="cls_nowrap">' + rows_data[i].data[x].MODIFY_BY_USERNAME
                                + '</td><td class="cls_nowrap">' + rows_data[i].data[x].MODIFY_BY_FULLNAME
                                + '</td><td class="cls_nowrap">' + rows_data[i].data[x].Count
                                + '</td><td class="cls_nowrap">' + rows_data[i].data[x].SAPMat_Count            
                                + '</td><td>'
                                + '</td></tr>');

                    }

                }
            
            });

           // $($.fn.dataTable.tables(true)).DataTable().columns.adjust();
        },

    });
   
    $(table + "_wrapper .dt-buttons .buttons-excel").hide();
    if ($('.cls_report_history .cls_txt_master_keyword').val() != "")
        table_report_history.search($('.cls_report_history .cls_txt_master_keyword').val()).draw();


   
}

function bind_ajax_summary_approve(first_load, columns, url, table, divclass) {
    cleardata();
    $(divclass).removeClass('cls_hide');

    var FrDt = $('.cls_report_history .cls_his_req_from').val();
    var ToDt = $('.cls_report_history .cls_his_req_to').val();

    var groupColumn = 1;
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
                url: url + "&data.FrDt=" + FrDt
                    + "&data.ToDt=" + ToDt
                    + "&data.first_load=" + first_load
                ,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //"columnDefs": [{
        //    "searchable": false,
        //    "orderable": false,
        //    "targets": 0
        //}],
        // "order": [[1, 'asc']],
        "processing": true,
        //"lengthChange": true,
        //"ordering": false,
        "info": true,
        "searching": true,
        "scrollX": true,
        "scrollY": "540px",
        "scrollCollapse": true,
        //dom: 'Bfrtip',
        //buttons: [
        //    {
        //        title: 'IGrid Summarize by Owner Level Report',
        //        extend: 'excelHtml5',
        //        exportOptions: {
        //            //columns: [1, 2, 3, 4, 5, 6],
        //            format: {
        //                body: function (data, row, column, node) {
        //                    return data;
        //                }
        //            }
        //        }
        //    }, "pageLength"
        //],

        columns: columns,
        "rowCallback": function (row, data, index) {
            $(row).addClass('highlight');

        },

        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var rows_data = api.rows({ page: 'current' }).data();
            var last = null;


            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {


                if (rows_data[i].data !== null) {

 
                    for (var x = 0; x < rows_data[i].data.length; x++) {

                        $(rows).eq(i).after('<tr><td class="cls_hide">'

                            + '</td><td class="cls_nowrap">' + rows_data[i].data[x].MODIFY_BY_USERNAME
                            + '</td><td class="cls_nowrap">' + rows_data[i].data[x].MODIFY_BY_FULLNAME
                            + '</td><td class="cls_nowrap">' + rows_data[i].data[x].CREATE_USERNAME
                            + '</td><td class="cls_nowrap">' + rows_data[i].data[x].CREATE_FULLNAME
                            + '</td><td class="cls_nowrap">' + rows_data[i].data[x].Count
                            + '</td></tr>');

                    }

                }

            });

            // $($.fn.dataTable.tables(true)).DataTable().columns.adjust();
        },

    });

    $(table + "_wrapper .dt-buttons .buttons-excel").hide();
    if ($('.cls_report_history .cls_txt_master_keyword').val() != "")
        table_report_history.search($('.cls_report_history .cls_txt_master_keyword').val()).draw();


}
