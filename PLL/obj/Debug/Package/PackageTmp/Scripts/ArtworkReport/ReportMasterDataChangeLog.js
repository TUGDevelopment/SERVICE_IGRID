var columns, url, divclass, table, master;
$(document).ready(function () {
    bindLOVMasterName();
    bind_lov('.cls_report_master_change_log .cls_lov_change_by_name', '/api/report/igrid_user_master_data_report', 'data.DISPLAY_TXT');

    setValueToDDL('.cls_report_master_change_log .cls_lov_master_name', 'All', 'All');
    setValueToDDL('.cls_report_master_change_log .cls_lov_change_by_name', 'All', 'All');


    $(".cls_report_master_change_log .cls_btn_search_master_data_change_log").click(function (e) {
        //debugger;
      

        if ($('.cls_div_body_search_criteria .cls_master_change_log_from').val() == null || $('.cls_div_body_search_criteria .cls_master_change_log_from').val() == "")
        {
            alertError2("Please input date from.");
            e.preventDefault();
           
        } else if ($('.cls_div_body_search_criteria .cls_master_change_log_to').val() == null || $('.cls_div_body_search_criteria .cls_master_change_log_to').val() == "") {
            alertError2("Please input date To.");
            e.preventDefault();
        }
        else {
            debugger;
            var first_load = "";
            if ($('.cls_div_body_search_criteria .cls_lov_layout').val() == "TransChanged") {
                var url =  suburl + "/api/report/igrid_master_data_change_log_report?data.first_load=" + first_load
                    + "&data.SEARCH_DATE_FROM=" + $('.cls_div_body_search_criteria .cls_master_change_log_from').val()
                    + "&data.SEARCH_DATE_TO=" + $('.cls_div_body_search_criteria .cls_master_change_log_to').val()
                    + "&data.SEARCH_USER=" + $('.cls_div_body_search_criteria .cls_lov_change_by_name').val()
                    + "&data.SEARCH_TYPE=" + $('.cls_div_body_search_criteria .cls_lov_layout').val()
                    + "&data.SEARCH_MASTER=" + $('.cls_div_body_search_criteria .cls_lov_master_name').val()
                    + "&data.SEARCH_KEYWORD=" + $('.cls_div_body_search_criteria .cls_txt_master_keyword').val()
                master = $('.cls_report_master_change_log .cls_lov_master_name').val();
                switch (master) {
                    case 'Primary Size':
                        //columns_export_excel = [1, 2, 3, 4, 5, 6, 7, 8];
                        columns = [
                            {
                                //render: function (data, type, row, meta) {
                                //    return meta.row + meta.settings._iDisplayStart + 1;
                                //}
                                data: null,
                                //title: "checkbox",
                                defaultContent: '',
                                //className: 'select-checkbox',
                                className: 'cls_hide',
                                orderable: false
                            },
                            { data: "CHANGED_ID", "className": "cls_nowrap" },
                            { data: "SHORTNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_TABNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_CHARNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_ACTION", "className": "cls_nowrap" },
                            { data: "ID", "className": "cls_nowrap" },
                            { data: "Code", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "Can", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "DESCRIPTION", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "LidType", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "ContainerType", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "DescriptionType", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "Inactive", "className": "cls_nowrap" },
                            { data: "CHANGED_BY", "className": "cls_nowrap" },
                            { data: "CHANGED_ON", "className": "cls_nowrap" },
                        ];
                        table = '#table_primary_size_transchanged_view';
                        divclass = '.div_table_primary_size';
                        break;
                    case 'Plant Register Address':
                        //columns_export_excel = [1, 2, 3, 4, 5];
                        columns = [
                            {
                                data: null,
                                //title: "checkbox",
                                defaultContent: '',
                                //className: 'select-checkbox',
                                className: 'cls_hide',
                                orderable: false
                            },

                            { data: "CHANGED_ID", "className": "cls_nowrap" },
                            { data: "SHORTNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_TABNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_CHARNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_ACTION", "className": "cls_nowrap" },
                            { data: "ID", "className": "cls_nowrap" },
                            { data: "RegisteredNo", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "Address", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "Plant", "className": "cls_nowrap cls_len_030" },
                            { data: "Inactive", "className": "cls_nowrap" },
                            { data: "CHANGED_BY", "className": "cls_nowrap" },
                            { data: "CHANGED_ON", "className": "cls_nowrap" },
                        ];
                        table = '#table_plantregistered_transchanged_view';
                        divclass = '.div_table_plantregistered';
                        break;
                    default:
                        //columns_export_excel = [1, 2, 3, 4, 5, 6, 7, 8, 9];
                        columns = [
                            {
                                data: null,
                                //title: "checkbox",
                                defaultContent: '',
                                //className: 'select-checkbox',
                                className: 'cls_hide',
                                orderable: false
                            },
                            { data: "CHANGED_ID", "className": "cls_nowrap" },
                            { data: "SHORTNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_TABNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_CHARNAME", "className": "cls_nowrap" },
                            { data: "CHANGED_ACTION", "className": "cls_nowrap" },
                            { data: "ID", "className": "cls_nowrap" },
                            { data: "PrimaryCode", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "GroupStyle", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "PackingStyle", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "RefStyle", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "PackSize", "className": "cls_nowrap cls_len_010 cls_require cls_isnumber" },
                            { data: "BaseUnit", "className": "cls_nowrap cls_len_030 cls_require" },
                            { data: "TypeofPrimary", "className": "cls_nowrap cls_len_030" },
                            { data: "Inactive", "className": "cls_nowrap" },
                            { data: "CHANGED_BY", "className": "cls_nowrap" },
                            { data: "CHANGED_ON", "className": "cls_nowrap" },
                        ];
                        table = '#table_packstyle_transchanged_view';
                        divclass = '.div_table_packstyle';
                        break;

                }
                bindMasterViewData(columns, url, table, divclass);
            } else {
                divclass = '.cls_div_master_data_change_log';
                bindMasterDataChangeLogReport('');
            }
            
            e.preventDefault();
        }
    });

    $('.cls_div_body_search_criteria .cls_lov_layout').on("change", function () {
        
        var obj = $('.cls_div_body_search_criteria .cls_lov_master_name:last');
        $(obj).empty();
        var v = $(this)[0].value == "TransChanged" ? "Packing Style" : "All";
        bindLOVMasterName();
        setValueToDDL('.cls_report_master_change_log .cls_lov_master_name', v, v);
         
    });

    $(".cls_div_body_search_criteria .cls_btn_export_excel_master_data_change_log").click(function () {
        $(divclass + " .buttons-excel").click();
        // $(".cls_report_warehouse .buttons-excel").unbind();
    });

    $(".cls_report_master_change_log .cls_btn_clear_master_data_change_log").click(function (e) {

        //$('.cls_div_body_search_criteria .cls_master_change_log_from').val(GetFirstDateOfMonth());
        //$('.cls_div_body_search_criteria .cls_master_change_log_to').val(GetLastDateOfMonth());

        $('.cls_div_body_search_criteria .cls_master_change_log_from').val(GetCurrentDate2());
        $('.cls_div_body_search_criteria .cls_master_change_log_to').val(GetCurrentDate2());

        $('.cls_div_body_search_criteria .cls_lov_change_by_name').val('All').trigger('change');
        $('.cls_div_body_search_criteria .cls_lov_master_name').val('All').trigger('change');
        $('.cls_div_body_search_criteria .cls_txt_master_keyword').val('').trigger('change');

   
    });

    bindMasterDataChangeLogReport('X');

});
function bindLOVMasterName() {
    //debugger;
    bind_lov_param('.cls_report_master_change_log .cls_lov_master_name', '/api/report/igrid_master_data_report', 'data.DISPLAY_TXT', ["SEARCH_TYPE"], ['.cls_div_body_search_criteria .cls_lov_layout']);
    //$('.cls_div_body_search_criteria .cls_lov_master_name').innerHTML = $('.cls_div_body_search_criteria .cls_lov_master_name').prop("selectedIndex", 0).val();
    //var v = $('.cls_div_body_search_criteria .cls_lov_master_name option:nth-child(1)').val();
}
function clearAllData() {
    $('.div_table_primary_size').addClass('cls_hide');
    $('.div_table_packstyle').addClass('cls_hide');
    $('.div_table_plantregistered').addClass('cls_hide');
    $('.cls_div_master_data_change_log').addClass('cls_hide');
}
function bindMasterViewData(columns, url, table, divclass) {

    clearAllData();
    $(divclass).removeClass('cls_hide');




    var table_primary_view = $(table).DataTable()
    table_primary_view.destroy();


    table_primary_view = $(table).DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHaeder: true,
        paging: false,
        ajax: function (data, callback, settings) {

            $.ajax({
                /*url: suburl + "/api/lov/primarysizeIGrid",*/
                url: url,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },

        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        "order": [[1, 'asc']],
        "processing": true,
        //"lengthChange": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        dom: 'Bfrtip',
        buttons: [
            {
                title: 'IGrid Master ' + master,
                extend: 'excelHtml5',
                exportOptions: {
                    //columns: columns_export_excel,//[1, 2, 3, 4, 5, 6],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            }//, "pageLength"
        ],
        columns: columns,

        "drawCallback": function (settings) {

        },
        "createdRow": function (row, data, index) {
            //if (data.STATUS == "1")
            //    $(row).css("color", "#A20025");
        },

    });
    $(table + "_wrapper .dt-buttons .buttons-excel").hide();


}
function bindMasterDataChangeLogReport(first_load) {

    clearAllData();
    $('.cls_div_master_data_change_log').removeClass('cls_hide');
    var table_report = $('#table_master_data_change_log').DataTable()
    table_report.destroy();

    table_report = $('#table_master_data_change_log').DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHaeder : true,
        paging: false,  
        //stateSave: false,
        ajax: function (data, callback, settings) {
           
            $.ajax({
                url: suburl + "/api/report/igrid_master_data_change_log_report?data.first_load=" + first_load
                    + "&data.SEARCH_DATE_FROM=" + $('.cls_div_body_search_criteria .cls_master_change_log_from').val()
                    + "&data.SEARCH_DATE_TO=" + $('.cls_div_body_search_criteria .cls_master_change_log_to').val()
                    + "&data.SEARCH_USER=" + $('.cls_div_body_search_criteria .cls_lov_change_by_name').val()
                    + "&data.SEARCH_TYPE=" + $('.cls_div_body_search_criteria .cls_lov_layout').val()
                    + "&data.SEARCH_MASTER=" + $('.cls_div_body_search_criteria .cls_lov_master_name').val()
                    + "&data.SEARCH_KEYWORD=" + $('.cls_div_body_search_criteria .cls_txt_master_keyword').val()
                ,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
     
        "order": [[1, 'desc']],
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
                title: 'IGrid Master Data Change Log Report',
                extend: 'excelHtml5',
                exportOptions: {
                    //columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            }//,"pageLength"
        ],
        columns: [

            { data: "CHANGED_ID", "className": "cls_nowrap" },
            { data: "SHORTNAME", "className": "cls_nowrap" },
            { data: "CHANGED_TABNAME", "className": "cls_nowrap" },
            { data: "CHANGED_CHARNAME", "className": "cls_nowrap" },
            { data: "CHANGED_ACTION", "className": "cls_nowrap" },
            { data: "OLD_ID", "className": "cls_nowrap" },
            { data: "ID", "className": "cls_nowrap" },
            { data: "DESCRIPTION", "className": "cls_nowrap" },
            { data: "CHANGED_BY", "className": "cls_nowrap" },
            { data: "CHANGED_ON", "className": "cls_nowrap" },
        
        ],
        "drawCallback": function (settings) {
            
        },

      

    });

    $("#table_master_data_change_log_wrapper .dt-buttons .buttons-excel").hide();
}
