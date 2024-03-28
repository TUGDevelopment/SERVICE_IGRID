$(document).ready(function () {

    $(".cls_report_matstatus .cls_matstatus_btn_search").click(function (e) {
        //debugger;

        if ($(".cls_report_matstatus .cls_lov_by_status").val() == "InActive Material") {
            $('.cls_report_matstatus .cls_row_active').removeClass('cls_hide');
            // $('.cls_table_report_matstatus .cls_row_active').prop('disabled',false);

        } else {
            $('.cls_report_matstatus .cls_row_active').addClass('cls_hide');
            // $('.cls_table_report_matstatus .cls_row_active').prop('disabled', true);

        }


        bindMatStatusReport('');
        e.preventDefault();
    });


    $(".cls_report_matstatus .cls_btn_export_excel").click(function () {
        $(".cls_report_matstatus .buttons-excel").click();
      
    });

    $(".cls_report_matstatus .cls_matstatus_btn_clr").click(function (e) {

        
        $('.cls_report_matstatus .cls_txt_master_keyword').val('').trigger('change');


    });



    $(".cls_report_matstatus .cls_btn_reactive").click(function (e) {


        var table = $('#table_report_matstatus').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            //setSelectSuggestMaterial(tblData);

            if (tblData[0].STATUS == "InActive Material") {
                var jsonObj = new Object();
                jsonObj.data = {};
                var MATSTATUS = {};

                MATSTATUS["MATERIAL"] = tblData[0].MATERIAL;


                jsonObj.data = MATSTATUS;

                var myurl = '/api/report/igrid_matstatus_reactive';
                var mytype = 'POST';
                var mydata = jsonObj;

                myAjaxConfirmSubmit(myurl, mytype, mydata, callback_matstatus_reactive);

            } else
            {
                alertError2("Status is not InActive Material.");
            }        

        }
        else {
            alertError2("Please select 1 item.");
        }

    });


    //$(".cls_report_matstatus .cls_lov_by_status").click(function () {
    //    if ($(".cls_report_matstatus .cls_lov_by_status").val() == "InActive Material") {
    //        $('.cls_report_matstatus .cls_row_active').removeClass('cls_hide');
    //       // $('.cls_table_report_matstatus .cls_row_active').prop('disabled',false);

    //    } else
    //    {
    //        $('.cls_report_matstatus .cls_row_active').addClass('cls_hide');
    //       // $('.cls_table_report_matstatus .cls_row_active').prop('disabled', true);

    //    }
    //});


    bindMatStatusReport('X');

});


function callback_matstatus_reactive()
{
    bindMatStatusReport('');
}


function bindMatStatusReport(first_load) {

    var table_report = $('#table_report_matstatus').DataTable()
    table_report.destroy();

    table_report = $('#table_report_matstatus').DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHaeder: true,
        paging: false,  
        //stateSave: false,
        ajax: function (data, callback, settings) {

            $.ajax({
                url: suburl + "/api/report/igrid_matstatus_report?data.first_load=" + first_load 
                    + "&data.SEARCH_BY_STATUS=" + $('.cls_report_matstatus .cls_lov_by_status').val()
                    + "&data.SEARCH_KEYWORD=" + $('.cls_report_matstatus .cls_txt_master_keyword').val()
                ,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },

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
                title: 'IGrid Material Status Report',
                extend: 'excelHtml5',
                exportOptions: {
                    columns: [1, 2, 3, 4],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            },// "pageLength"
        ],
        fixedColumns: {
            leftColumns: 1
        },
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox cls_checkbox',
                orderable: false
            },
            { data: "ID", "className": "cls_nowrap" },
            { data: "MATERIAL", "className": "cls_nowrap" },
            { data: "DESCRIPTION", "className": "cls_nowrap" },
            { data: "STATUS", "className": "cls_nowrap" },
        
        ],
        columnDefs: [
            {
                "targets": 0,
                "orderable": false,
                "createdCell": function (td, cellData, rowData, row, col) {
                    if ($(".cls_report_matstatus .cls_lov_by_status").val() != "InActive Material") {
                        $(td).removeClass('select-checkbox');
                    } else {
                        $(td).addClass('select-checkbox');
                    }
                }
            }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        "drawCallback": function (settings) {

        },
    
    });

    $("#table_report_matstatus_wrapper .dt-buttons .buttons-excel").hide();


    //if ($(".cls_report_matstatus .cls_lov_by_status").val() != "InActive Material")
    //{
    //    $('.cls_table_report_matstatus .cls_checkbox').prop('disabled', true);
    //}


}
