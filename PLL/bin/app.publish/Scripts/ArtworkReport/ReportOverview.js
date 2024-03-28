var first_load = true;
var isPA = false;
var isPA_Approve = false;
$(document).ready(function () {
    //bind_lov('.cls_report_overview .cls_lov_condition', '/api/lov/condition', 'data.DISPLAY_TXT');
    first_load = false;

    getIGirdOverviewRole();

   

    bindDataOverviewReport(false, "1");
   


    $('.btn_over_option').click(function (e) {
        var table = $('#table_report_overview').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            $("[name=cls_rdo_igrid_pa_overview_option]").removeAttr("checked");


            $('.cls_igrid_pa_overview_material').val(tblData[0].Material);
       
            $('#modal_igrid_pa_overview').modal({
                backdrop: 'static',
                keyboard: true
            });
        } else
        {
            alertError2("Please select material at least 1 item.");
        }
    });


    $('#modal_igrid_pa_overview .cls_btn_save').click(function (e) {
       // alertError2($('input[name="cls_rdo_igrid_pa_overview_option"]:checked').val());

        var option = $('input[name="cls_rdo_igrid_pa_overview_option"]:checked').val();
        var material = $('#modal_igrid_pa_overview .cls_igrid_pa_overview_material').val();
        switch(option)
        {
            case "4":
                submitCreateDocument(material, option, "Are you sure you want to Change version?");
                break;
            case "1":
                submitCreateDocument(material, option, "Are you sure you want to Copy Template?");
                break;
            case "7":
                submitCreateDocument(material, option, "Are you sure you want to Change Classification?");
                break;
            default:
                alertError2("Please choose option.");
                break;
        }


    });



    $('.cls_report_overview .cls_lov_condition').on("change", function () {
        //var selections = $(this).select2('data');
        //var v = $(this)[0].value;
        //var isClear = true;
    });
    $('.cls_report_overview .cls_over_btn_Remove').click(function (e) {
        var table = $('#table_report_overview').DataTable();
        var tblData = table.rows('.selected').data();
        if (tblData.length > 0) {
            //const array = [];
            for (i = 0; i < tblData.length; i++) {
                //var three_p_id = tblData[i].Id;
                //array.push(tblData[i].Id);
                var jsonObj = new Object();
                jsonObj.data = [];
                var item = {};
                item["Material"] = tblData[i].Material;
                jsonObj.data = item;
                var myurl = suburl + '/api/report/overviewreport';
                var mytype = 'POST';
                var mydata = jsonObj;

                myAjaxConfirmSubmit(myurl, mytype, mydata, callbackInactive);
            }
        }
    });

    $(".cls_report_overview .cls_over_btn_clr").click(function ()
    {
      
        $('.cls_report_overview .cls_txt_master_keyword').val('');
        $('.cls_report_overview .cls_lov_condition').val('All');
    });

    $(".cls_report_overview .cls_btn_export_excel").click(function () {
        $(".cls_report_overview .buttons-excel").click();
        // $(".cls_report_warehouse .buttons-excel").unbind();
    });
    $(".cls_report_overview .cls_over_btn_search").click(function (e) {
        //debugger;
        var keyword = $('.cls_report_overview .cls_txt_master_keyword').val();

        if (!isEmpty(keyword)) {
            //table_report_overview.ajax.reload();
            bindDataOverviewReport(false, "0");
            e.preventDefault();
        }
        else {
            alertError2("Please input request keyword.");

            e.preventDefault();
        }
    });
});
function bindDataOverviewReport(serverSide, autosearch) {
    var groupColumnPPView = 2;
    var table_report_overview = $('#table_report_overview').DataTable()
    table_report_overview.destroy();
    table_report_overview = $('#table_report_overview').DataTable({
        
        "scrollY": "600px",
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
                url: suburl + "/api/report/overviewreport?data.Condition=" + $('.cls_report_overview .cls_lov_condition').val()
                    + "&data.Material=" + $('.cls_report_overview .cls_txt_master_keyword').val()
                    + "&data.first_load=" + autosearch
                ,
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
                title: 'IGrid Overview',
                extend: 'excelHtml5',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61,62,63],
                    format: {
                        body: function (data, row, column, node) {
                            return data;
                        }
                    }
                }
            }
        ],
        columns: [
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
            { data: "DocumentNo", "className": "cls_nowrap" },
            { data: "Material", "className": "cls_nowrap" },
            { data: "Description", "className": "cls_nowrap" },
            { data: "Brand", "className": "cls_nowrap" },
            { data: "MaterialGroup", "className": "cls_nowrap" },
            { data: "PrimarySize", "className": "cls_nowrap" },
            { data: "Version", "className": "cls_nowrap" },
            { data: "ChangePoint", "className": "cls_nowrap" },
            { data: "SheetSize", "className": "cls_nowrap" },
            { data: "Assignee", "className": "cls_nowrap" },
            { data: "PackingStyle", "className": "cls_nowrap" },
            { data: "Packing", "className": "cls_nowrap" },
            { data: "StyleofPrinting", "className": "cls_nowrap" },
            { data: "ContainerType", "className": "cls_nowrap" },
            { data: "LidType", "className": "cls_nowrap cls_lidtype" },
            { data: "Condition", "className": "cls_nowrap" },
            { data: "ModifyBy", "className": "cls_nowrap" },
            { data: "ModifyOn", "className": "cls_nowrap" },
            { data: "ProductCode", "className": "cls_nowrap" },
            { data: "FAOZone", "className": "cls_nowrap" },
            { data: "Plant", "className": "cls_nowrap" },
            { data: "Totalcolour", "className": "cls_nowrap" },
            { data: "Processcolour", "className": "cls_nowrap" },
            { data: "PlantRegisteredNo", "className": "cls_nowrap" },
            { data: "CompanyNameAddress", "className": "cls_nowrap" },
            { data: "PMScolour", "className": "cls_nowrap" },
            { data: "Symbol", "className": "cls_nowrap" },
            { data: "CatchingArea", "className": "cls_nowrap" },
            { data: "CatchingPeriodDate", "className": "cls_nowrap" },
            { data: "Grandof", "className": "cls_nowrap" },
            { data: "Flute", "className": "cls_nowrap" },
            { data: "Vendor", "className": "cls_nowrap" },
            { data: "Dimension", "className": "cls_nowrap" },
            { data: "RSC", "className": "cls_nowrap" },
            { data: "Accessories", "className": "cls_nowrap" },
            { data: "PrintingStyleofPrimary", "className": "cls_nowrap" },
            { data: "PrintingStyleofSecondary", "className": "cls_nowrap" },
            { data: "CustomerDesign", "className": "cls_nowrap" },
            { data: "CustomerSpec", "className": "cls_nowrap" },
            { data: "CustomerSize", "className": "cls_nowrap" },
            { data: "CustomerVendor", "className": "cls_nowrap" },
            { data: "CustomerColor", "className": "cls_nowrap" },
            { data: "CustomerScanable", "className": "cls_nowrap" },
            { data: "CustomerBarcodeSpec", "className": "cls_nowrap" },
            { data: "FirstInfoGroup", "className": "cls_nowrap" },
            { data: "SO", "className": "cls_nowrap" },
            { data: "PICMkt", "className": "cls_nowrap" },
            { data: "SOPlant", "className": "cls_nowrap" },
            { data: "Destination", "className": "cls_nowrap" },
            { data: "Remark", "className": "cls_nowrap" },
            { data: "GrossWeight", "className": "cls_nowrap" },
            { data: "FinalInfoGroup", "className": "cls_nowrap" },
            { data: "Note", "className": "cls_nowrap" },
            { data: "Typeof", "className": "cls_nowrap" },
            { data: "TypeofCarton2", "className": "cls_nowrap" },
            { data: "DMSNo", "className": "cls_nowrap" },
            { data: "TypeofPrimary", "className": "cls_nowrap" },
            { data: "PrintingSystem", "className": "cls_nowrap" },
            { data: "Direction", "className": "cls_nowrap" },
            { data: "RollSheet", "className": "cls_nowrap" },
            { data: "RequestType", "className": "cls_nowrap" },
            { data: "PlantAddress", "className": "cls_nowrap" },

        ],
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
        "rowCallback": function (row, data, index) {
            if (!isEmpty(data.LidType)) {
                $(row).find('.cls_lidtype').text(data.LidType);
            }
          
        },
        "createdRow": function (row, data, index) {
            ////if (data.STATUS == "1")
            ////    $(row).css("color", "#A20025");
        },
        //"drawCallback": function (settings) {

        //},
        //"initComplete": function (settings, json) {
        //    $('.cls_cnt_igrid_view').text('(' + json.data.length + ') ');
        //}
    });
    $("#table_report_overview_wrapper .dt-buttons .buttons-excel").hide();
}
function callbackInactive() {
    //ARTWORK_SUB_PA_ID = res.data[0].ID;
    bindDataOverviewReport(false, "0");
}

function submitCreateDocument(material,option,message_confirm) {
    var jsonObj = new Object();
    var item = {};

    jsonObj.data = {};
    item["Material"] = material;
    item["Condition"] = option;
   
    jsonObj.data = item;

    var myurl = suburl + '/api/report/overviewreport_createdoc';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback_submitCreateDocument, '', true, true, message_confirm);
 
}

function callback_submitCreateDocument(res)
{
    $('#modal_igrid_pa_overview').modal('hide');
    bindDataOverviewReport(false, "0");

    if (res.data != null && res.data[0].ID != null)
    {
        var url = suburl + '/IGrid/' + res.data[0].ID;
        window.open(url, '_blank')
    }
   

}

function getIGirdOverviewRole() {

    //var jsonObj = new Object();
    //var item = {};

    //jsonObj.data = {};
    //item["Id"] = CURRENTUSERID;
    //jsonObj.data = item;


    var myurl = suburl + '/api/report/overviewreport_getrole'
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callback_getIGirdOverviewRole);
}

function callback_getIGirdOverviewRole(res)
{
    debugger;
    isPA = false;
    isPA_Approve = false;

    if (res.data != null && res.data.length > 0)
    {
        if (!isEmpty(res.data[0].fn))
        {
            var roles = res.data[0].fn.split(",")
            $.each(roles, function (i, v) {
                switch(v.trim())
                {
                    case 'PA':
                        isPA = true;
                        $('.cls_report_overview .btn_over_option').show();
                        break;
                    case 'PA_Approve':
                        isPA_Approve = true;
                        $('.cls_report_overview .cls_over_btn_Remove').show();
                        break;
                    default:
                        break;
                }
            });
        }

    }

    if (isPA) {
       
    }

    if (isPA_Approve) {
     
    }

}