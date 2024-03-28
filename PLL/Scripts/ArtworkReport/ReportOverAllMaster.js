var rowCallback;
$(document).ready(function () {
    bind_lov('.cls_report_OverallMaster .cls_lov_master', '/api/report/igrid_overall_get_master', 'data.DISPLAY_TXT');



    $(".cls_report_OverallMaster .cls_overall_btn_search").click(function (e) {
        //debugger;

        if (!isStringNullOrEmpty($(".cls_report_OverallMaster .cls_lov_master").val())) {
            bindOverallMasterReport('');
            e.preventDefault();
        } else
        {
            alertError2("Please select master.");
            e.preventDefault();
        }

       
    });



    $(".cls_report_OverallMaster .cls_btn_export_excel").click(function () {
        if (!isStringNullOrEmpty(div_group))
        {
            $(div_group + " .buttons-excel").click();
        }
       
    });



    $(".cls_report_OverallMaster .cls_lov_master").change(function () {
        //alertError2($(".cls_report_OverallMaster .cls_lov_master").val());
    });



    bindOverallMasterReport('X');

});


function isStringNullOrEmpty(val)
{
    var f_empty = false;

    if (val == '0' || val == '' || val == 'undefined' || val == null)
    {
        f_empty = true;
    }

    return f_empty;
}

var column_export_excel;
var column;
var table_group;
var div_group;
function setColumns(master)
{
    //$(".cls_table_report_overall_master thead tr").empty()
    //$(".cls_table_report_overall_master thead tr").each(function () {
    //    $(this).find("th").remove();
    //});


    $(".cls_report_OverallMaster .cls_table_hide").hide();
    rowCallback = function (row, data, index) {
    }

    switch (master) {
        case "Flute":
        case "Gradeof":
        case "StyleofPrinting":
        case "TotalColour":
        case "ProcessColour":
        case "PMSColour": //02
            table_group = '#table_report_overall_master_02';
            div_group = '.cls_table_report_overall_master_02';
            
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "DESCRIPTION", "className": "cls_nowrap" },
                { data: "MATERIALGROUP", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;
        case "TypeOf":  //03
            table_group = '#table_report_overall_master_03';
            div_group = '.cls_table_report_overall_master_03';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "DESCRIPTION", "className": "cls_nowrap" },
                { data: "MATERIALGROUP", "className": "cls_nowrap" },
                { data: "MATERIALTYPE", "className": "cls_nowrap" }, 
                { data: "DESCRIPTIONTEXT", "className": "cls_nowrap" }, 
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;
        case "PrimarySize":  //04
            table_group = '#table_report_overall_master_04';
            div_group = '.cls_table_report_overall_master_04';
            rowCallback = function (row, data, index) {
                $(row).find('.cls_description_type').text(data.DESCRIPTIONTYPE);
            }
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "CODE", "className": "cls_nowrap",title : "Running" },
                { data: "CAN", "className": "cls_nowrap",title : "Code Primary (Digit 9-11)" },
                { data: "DESCRIPTION", "className": "cls_nowrap",title : "Primary size" },
                { data: "LIDTYPE", "className": "cls_nowrap",title : "Code Container&Lid (Digit 12-13)" },
                { data: "CONTRAINERTYPE", "className": "cls_nowrap", title : "Container Type"},
                { data: "DESCRIPTIONTYPE", "className": "cls_nowrap cls_description_type", title : "Lid Type" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;
        case "PackStyle":  //05
            table_group = '#table_report_overall_master_05';
            div_group = '.cls_table_report_overall_master_05';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "PRIMARYCODE", "className": "cls_nowrap" },
                { data: "GROUPSTYLE", "className": "cls_nowrap" },
                { data: "PACKINGSTYLE", "className": "cls_nowrap" }, 
                { data: "REFSTYLE", "className": "cls_nowrap" },
                { data: "PACKSIZE", "className": "cls_nowrap" },
                { data: "BASEUNIT", "className": "cls_nowrap" },
                { data: "TYPEOFPRIMARY", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;
        case "PlantRegistered":  //06
            table_group = '#table_report_overall_master_06';
            div_group = '.cls_table_report_overall_master_06';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "REGISTEREDNO", "className": "cls_nowrap" },
                { data: "ADDRESS", "className": "cls_nowrap" },
                { data: "PLANT", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;
        case "Vendor":  //07
            table_group = '#table_report_overall_master_07';
            div_group = '.cls_table_report_overall_master_07';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "CODE", "className": "cls_nowrap" ,title : "Vendor Code"},
                { data: "NAME", "className": "cls_nowrap" }, 
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;
        case "ProductGroup":  //08
            table_group = '#table_report_overall_master_08';
            div_group = '.cls_table_report_overall_master_08';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "PRODUCT_GROUP", "className": "cls_nowrap" },
                { data: "PRODUCT_GROUPDESC", "className": "cls_nowrap" },
                { data: "PRD_PLANT", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;

        case "ulogin":  //09
            table_group = '#table_report_overall_master_09';
            div_group = '.cls_table_report_overall_master_09';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "USER_NAME", "className": "cls_nowrap" },
                { data: "FN", "className": "cls_nowrap" },
                { data: "FIRSTNAME", "className": "cls_nowrap" },
                { data: "LASTNAME", "className": "cls_nowrap" },
                { data: "EMAIL", "className": "cls_nowrap" },
                { data: "AUTHORIZE_CHANGEMASTER", "className": "cls_nowrap" },
                { data: "SAP_EDPUSERNAME", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;

        case "WHManagement":  //10
            table_group = '#table_report_overall_master_10';
            div_group = '.cls_table_report_overall_master_10';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "PRODUCTGROUP", "className": "cls_nowrap" },
                { data: "DESCRIPTION", "className": "cls_nowrap" },
                { data: "PLANT", "className": "cls_nowrap" },
                { data: "WHNUMBER", "className": "cls_nowrap" },
                { data: "STORAGETYPE", "className": "cls_nowrap" },
                { data: "LE_QTY", "className": "cls_nowrap" },
                { data: "STORAGE_UNITTYPE", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;

        case "SustainMaterial": //11
        case "SustainPlastic": //11
        case "SustainCertSourcing": //11
            table_group = '#table_report_overall_master_11';
            div_group = '.cls_table_report_overall_master_11';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap" },
                { data: "DESCRIPTION", "className": "cls_nowrap" },
                { data: "VALUE", "className": "cls_nowrap" },
                { data: "MATERIALGROUP", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;

        case "Brand":
            table_group = '#table_report_overall_master_01';
            div_group = '.cls_table_report_overall_master_01';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap",title : "Brand Id" },
                { data: "DESCRIPTION", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;
        default:  //01
            table_group = '#table_report_overall_master_01';
            div_group = '.cls_table_report_overall_master_01';
            column = [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                { data: "IDS", "className": "cls_nowrap",title : "ID" },
                { data: "DESCRIPTION", "className": "cls_nowrap" },
                { data: "INACTIVE", "className": "cls_nowrap" }]
            break;
    }

   
    $(div_group).show();


     //var column = [
    //    {
    //        data: null,
    //        defaultContent: '',
    //        className: 'select-checkbox',
    //        orderable: false
    //    },
    //    { data: "ID", "className": "cls_nowrap" }, { data: "DESCRIPTION", "className": "cls_nowrap" }, { data: "INACTIVE", "className": "cls_nowrap" }];

}



function bindOverallMasterReport(first_load) {

    debugger;
  
    var master = $('.cls_report_OverallMaster .cls_lov_master').val();
    setColumns(master)


    var table_report = $(table_group).DataTable()
    table_report.destroy();

    
   

    table_report = $(table_group).DataTable({

        serverSide: false,
        orderCellsTop: true,
        fixedHaeder: true,
        paging: false,  
        //stateSave: false,
        ajax: function (data, callback, settings) {

            $.ajax({
                url: suburl + "/api/report/igrid_overall_master_report?data.first_load=" + first_load
                    + "&data.SEARCH_MASTER=" + master
                    + "&data.SEARCH_KEYWORD=" + $('.cls_report_OverallMaster .cls_txt_master_keyword').val()
                    + "&data.SEARCH_STATUS=" + $('.cls_report_OverallMaster .cls_lov_status').val()
                ,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },

        "order": [[0, 'asc']],
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
                title: 'IGrid Overall Master Report',
                extend: 'excelHtml5',
                exportOptions: {
                    //columns: [1, 2, 3, 4, 5, 6],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            },// "pageLength"
        ],
        //fixedColumns: {
        //    leftColumns: 1
        //},
        columns : column,
        //columns: [
        //    {
        //        data: null,
        //        defaultContent: '',
        //        className: 'select-checkbox',
        //        orderable: false
        //    },
        //    { data: "ID", "className": "cls_nowrap" },
        //    { data: "CODE", "className": "cls_nowrap" },
        //    { data: "CAN", "className": "cls_nowrap" },
        //    { data: "PRODUCTGROUP", "className": "cls_nowrap" },
        //    { data: "DESCRIPTION", "className": "cls_nowrap" },
        //    { data: "MATERIALGROUP", "className": "cls_nowrap" },
        //    { data: "PRIMARYCODE", "className": "cls_nowrap" },
        //    { data: "GROUPSTYLE", "className": "cls_nowrap" },
        //    { data: "PAKCINGSTYLE", "className": "cls_nowrap" },
        //    { data: "REFSTYLE", "className": "cls_nowrap" },
        //    { data: "PACKSIZE", "className": "cls_nowrap" },
        //    { data: "BASEUNIT", "className": "cls_nowrap" },
        //    { data: "TYPEOFPRIMARY", "className": "cls_nowrap" },
        //    { data: "REGISTEREDNO", "className": "cls_nowrap" },
        //    { data: "ADDRESS", "className": "cls_nowrap" },
        //    { data: "PLANT", "className": "cls_nowrap" },
        //    { data: "LIDTYPE", "className": "cls_nowrap" },
        //    { data: "CONTRAINERTYPE", "className": "cls_nowrap" },
        //    { data: "DESCRIPTIONTYPE", "className": "cls_nowrap" },
        //    { data: "MATERIALTYPE", "className": "cls_nowrap" },
        //    { data: "DESCRIPTIONTEXT", "className": "cls_nowrap" },
        //    { data: "USER_NAME", "className": "cls_nowrap" },
        //    { data: "PASSWORD", "className": "cls_nowrap" },
        //    { data: "USERLEVEL", "className": "cls_nowrap" },
        //    { data: "FIRSTNAME", "className": "cls_nowrap" },
        //    { data: "LASTNAME", "className": "cls_nowrap" },
        //    { data: "EMIAL", "className": "cls_nowrap" },
        //    { data: "AUTHORIZE_CHANGEMASTER", "className": "cls_nowrap" },
        //    { data: "SAP_EDPUSERNAME", "className": "cls_nowrap" },
        //    { data: "NAME", "className": "cls_nowrap" },
        //    { data: "WHNUMBER", "className": "cls_nowrap" },
        //    { data: "STORAGETYPE", "className": "cls_nowrap" },
        //    { data: "LE_QTY", "className": "cls_nowrap" },
        //    { data: "STORAGE_UNITTYPE", "className": "cls_nowrap" },
        //    { data: "INACTIVE", "className": "cls_nowrap" },

        //],
        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        "rowCallback": rowCallback,
        "drawCallback": function (settings) {

        },

    });

    $(table_group + "_wrapper .dt-buttons .buttons-excel").hide();
}

