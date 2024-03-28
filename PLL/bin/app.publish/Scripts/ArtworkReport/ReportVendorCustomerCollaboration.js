$(document).ready(function () {

    $(".cls_report_vendor_customer_collaboration .cls_btn_clr").click(function () {
        location.reload();
    });

    $(".cls_report_vendor_customer_collaboration .cls_btn_search").click(function (e) {
        if ($('.cls_dt_search_request_date_from').val() == '') {
            alertError2('Please select request date from.');
        }
        else if ($('.cls_dt_search_request_date_to').val() == '') {
            alertError2('Please select request date to.');
        }
        else {
            var reqType = $("#frm_vendor_customer_collaboration input[name='requesttype']:checked").val();
            var viewof = $("#frm_vendor_customer_collaboration input[name='viewof']:checked").val();

            clear_table();

            //Mockup
            if (reqType == "mockup") {
                if (viewof == "customer") {
                    bind_table_report_customer_mockup();
                    $(".cls_table_report_customer_mockup").show();
                    $(".cls_table_report_customer_artwork").hide();
                    $(".cls_table_report_vendor_artwork").hide();
                    $(".cls_table_report_vendor_mockup").hide();
                }
                else {
                    bind_table_report_vendor_mockup();
                    $(".cls_table_report_vendor_mockup").show();
                    $(".cls_table_report_customer_artwork").hide();
                    $(".cls_table_report_customer_mockup").hide();
                    $(".cls_table_report_vendor_artwork").hide();
                }
            }
            //Artwork
            else {
                if (viewof == "customer") {
                    bind_table_report_customer_artwork();
                    $(".cls_table_report_customer_artwork").show();
                    $(".cls_table_report_customer_mockup").hide();
                    $(".cls_table_report_vendor_artwork").hide();
                    $(".cls_table_report_vendor_mockup").hide();
                }
                else {
                    bind_table_report_vendor_artwork();
                    $(".cls_table_report_vendor_artwork").show();
                    $(".cls_table_report_vendor_mockup").hide();
                    $(".cls_table_report_customer_artwork").hide();
                    $(".cls_table_report_customer_mockup").hide();
                }
            }
        }
        //e.preventDefault();
    });

    $(".cls_report_vendor_customer_collaboration .cls_btn_export_excel").click(function () {
        var reqType = $("#frm_vendor_customer_collaboration input[name='requesttype']:checked").val();
        var viewof = $("#frm_vendor_customer_collaboration input[name='viewof']:checked").val();
        var reportType = "";
        //Mockup
        if (reqType == "mockup") {
            if (viewof == "customer") {
                reportType = "customermockupreport";
            }
            else {
                reportType = "vendormockupreport";
            }
        }
        //Artwork
        else {
            if (viewof == "customer") {
                reportType = "customerartworkreport";
            }
            else {
                reportType = "vendorartworkreport";
            }
        }
        var param = getParamReportCustomerVendor_Execel();
        window.open(suburl + "/excel/" + reportType + param, '_blank');
    });

    bind_lov('.cls_report_vendor_customer_collaboration .cls_lov_search_customer', '/api/lov/customer', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_search_vendor', '/api/lov/vendorhasuser_bymatgroup', 'data.DISPLAY_TXT');

    $(".cls_report_vendor_customer_collaboration .search_vendor").addClass("cls_hide");

    $('#frm_vendor_customer_collaboration input[type=radio]').change(function () {
        if ($(this).val() == "customer") {
            $(".search_customer").removeClass("cls_hide");
            $(".search_vendor").addClass("cls_hide");
        }
        else if ($(this).val() == "vendor") {
            $(".search_customer").addClass("cls_hide");
            $(".search_vendor").removeClass("cls_hide");
        }
    })

    $(".cls_table_report_customer_artwork").hide();
    $(".cls_table_report_customer_mockup").hide();
    $(".cls_table_report_vendor_artwork").hide();
    $(".cls_table_report_vendor_mockup").hide();

    generate_header_filter();
});

function getParamReportCustomerVendor_Execel() {
    var viewof = $("#frm_vendor_customer_collaboration input[name='viewof']:checked").val();
    var criteria = "";
    if (viewof == "customer") {
        criteria = "&data.CUSTOMER_ID=" + $('.cls_report_vendor_customer_collaboration .cls_lov_search_customer').val();
    }
    else if (viewof == "vendor") {
        criteria = "&data.VENDOR_ID=" + $('.cls_report_vendor_customer_collaboration .cls_lov_search_vendor').val();
    }
    return "?data.DATE_FROM=" + $('.cls_dt_search_request_date_from').val()
        + "&data.DATE_TO=" + $('.cls_dt_search_request_date_to').val()
        + criteria;
}

function clear_table() {
    table_report_customer_artwork = $('#table_report_customer_artwork').DataTable();
    table_report_customer_artwork.destroy();

    table_report_customer_mockup = $('#table_report_customer_mockup').DataTable();
    table_report_customer_mockup.destroy();

    table_report_vendor_artwork = $('#table_report_vendor_artwork').DataTable();
    table_report_vendor_artwork.destroy();

    table_report_vendor_mockup = $('#table_report_vendor_mockup').DataTable();
    table_report_vendor_mockup.destroy();
}

function generate_header_filter() {
    clear_table();

    //customer_artwork
    $('.cls_table_report_customer_artwork thead tr:eq(2)').after('<tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th></tr>');
    $('.cls_table_report_customer_artwork thead tr:eq(3) th').each(function (i) {
        if (i == 0) {
            $(this).addClass("hide_sorting_asc");
            $(this).html('');
        }
        else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    //customer_mockup
    $('.cls_table_report_customer_mockup thead tr:eq(3)').after('<tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th></tr>');
    $('.cls_table_report_customer_mockup thead tr:eq(4) th').each(function (i) {
        if (i == 0) {
            $(this).addClass("hide_sorting_asc");
            $(this).html('');
        }
        else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    //vendor_artwork
    $('.cls_table_report_vendor_artwork thead tr:eq(2)').after('<tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th></tr>');
    $('.cls_table_report_vendor_artwork thead tr:eq(3) th').each(function (i) {
        if (i == 0) {
            $(this).addClass("hide_sorting_asc");
            $(this).html('');
        }
        else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });

    //vendor_mockup
    $('.cls_table_report_vendor_mockup thead tr:eq(2)').after('<tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th></tr>');
    $('.cls_table_report_vendor_mockup thead tr:eq(3) th').each(function (i) {
        if (i == 0) {
            $(this).addClass("hide_sorting_asc");
            $(this).html('');
        }
        else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }
    });
}

var table_report_customer_artwork;
function bind_table_report_customer_artwork() {
    var filter = "?data.DATE_FROM=" + $('.cls_dt_search_request_date_from').val() + "&data.DATE_TO=" + $('.cls_dt_search_request_date_to').val() + "&data.CUSTOMER_ID=" + $('.cls_report_vendor_customer_collaboration .cls_lov_search_customer').val();

    //bind data
    table_report_customer_artwork = $('#table_report_customer_artwork').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        fixedColumns: {
            leftColumns: 2
        },
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/customerartworkreport" + filter,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [
            { "searchable": false, "orderable": false, "targets": [0], "className": 'dt-body-center' }
        ],
        "order": [[1, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "scrollY": "450px",
        "scrollCollapse": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "Customer", "className": "cls_nowrap" },
            { "data": "Total", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_ChangeOption", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_WantToAdjust", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_IncorrectArtwork", "className": "cls_nowrap dt-body-right" },
            { "data": "Approve_Artwork", "className": "cls_nowrap dt-body-right" },
            { "data": "Approve_ShadeLimit", "className": "cls_nowrap dt-body-right" },
            { "data": "Cancel", "className": "cls_nowrap dt-body-right" },
        ],
        "rowCallback": function (row, data, index) {

        },
    });

    $(table_report_customer_artwork.table().container()).on('keyup', 'input', function () {
        table_report_customer_artwork
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    //hiding global search box
    $(".dataTables_filter").css("display", "none");
}

var table_report_customer_mockup;
function bind_table_report_customer_mockup() {
    var filter = "?data.DATE_FROM=" + $('.cls_dt_search_request_date_from').val() + "&data.DATE_TO=" + $('.cls_dt_search_request_date_to').val() + "&data.CUSTOMER_ID=" + $('.cls_report_vendor_customer_collaboration .cls_lov_search_customer').val();

    //bind data    
    table_report_customer_mockup = $('#table_report_customer_mockup').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        fixedColumns: {
            leftColumns: 2
        },
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/customermockupreport" + filter,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [{ "searchable": false, "orderable": false, "targets": [0], "className": 'dt-body-center' }],
        "order": [[1, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "scrollY": "450px",
        "scrollCollapse": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "CustomerCode", "className": "cls_nowrap" },
            { "data": "Total", "className": "cls_nowrap dt-body-right" },
            { "data": "ApproveDieLine_NoArtwork", "className": "cls_nowrap dt-body-right" },
            { "data": "ApproveDieLine_Artwork", "className": "cls_nowrap dt-body-right" },
            { "data": "ApprovePhysical_Mockup", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_WanttoAdjust", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_IncorrectMockup", "className": "cls_nowrap dt-body-right" },
            { "data": "Cancel", "className": "cls_nowrap dt-body-right" },
        ],
        "rowCallback": function (row, data, index) {

        },
    });

    $(table_report_customer_mockup.table().container()).on('keyup', 'input', function () {
        table_report_customer_mockup
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    //hiding global search box
    $(".dataTables_filter").css("display", "none");
}

var table_report_vendor_artwork;
function bind_table_report_vendor_artwork() {
    var filter = "?data.DATE_FROM=" + $('.cls_dt_search_request_date_from').val() + "&data.DATE_TO=" + $('.cls_dt_search_request_date_to').val() + "&data.VENDOR_ID=" + $('.cls_report_vendor_customer_collaboration .cls_lov_search_vendor').val();

    //bind data
    table_report_vendor_artwork = $('#table_report_vendor_artwork').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        fixedColumns: {
            leftColumns: 2
        },
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/vendorartworkreport" + filter,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [{ "searchable": false, "orderable": false, "targets": [0], "className": 'dt-body-center' }],
        "order": [[1, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "scrollY": "450px",
        "scrollCollapse": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "Vendor", "className": "cls_nowrap" },
            { "data": "Total", "className": "cls_nowrap dt-body-right" },
            { "data": "Approve", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_PA", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_PG", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_QC", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Customer", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Marketing", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Vendor", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Warehouse", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Planning", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Send_Print_All", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Send_Print_Ontime", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Confirm_PO_All", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Confirm_PO_Ontime", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Send_Shade_All", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Send_Shade_Ontime", "className": "cls_nowrap dt-body-right" },
        ],
        "rowCallback": function (row, data, index) {

        },
    });

    $(table_report_vendor_artwork.table().container()).on('keyup', 'input', function () {
        table_report_vendor_artwork
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    //hiding global search box
    $(".dataTables_filter").css("display", "none");
}

var table_report_vendor_mockup;
function bind_table_report_vendor_mockup() {
    var filter = "?data.DATE_FROM=" + $('.cls_dt_search_request_date_from').val() + "&data.DATE_TO=" + $('.cls_dt_search_request_date_to').val() + "&data.VENDOR_ID=" + $('.cls_report_vendor_customer_collaboration .cls_lov_search_vendor').val();

    //bind data
    table_report_vendor_mockup = $('#table_report_vendor_mockup').DataTable({
        serverSide: true,
        orderCellsTop: true,
        fixedHeader: true,
        fixedColumns: {
            leftColumns: 2
        },
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + "/api/report/vendormockuppreport" + filter,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [{ "searchable": false, "orderable": false, "targets": [0], "className": 'dt-body-center' }],
        "order": [[1, 'asc']],
        "processing": true,
        "searching": true,
        "lengthChange": true,
        "ordering": true,
        "info": true,
        "scrollX": true,
        "scrollY": "450px",
        "scrollCollapse": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "Vendor", "className": "cls_nowrap" },
            { "data": "Total", "className": "cls_nowrap dt-body-right" },
            { "data": "Approve", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Vendor", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_PG", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Customer", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Warehouse", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Marketing", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_Planning", "className": "cls_nowrap dt-body-right" },
            { "data": "Revise_RD", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Quotations_All", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Quotations_Ontime", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Mockup_All", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Mockup_Ontime", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Dieline_All", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Dieline_Ontime", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Matchboard_All", "className": "cls_nowrap dt-body-right" },
            { "data": "Day_Matchboard_Ontime", "className": "cls_nowrap dt-body-right" },
        ],
        "rowCallback": function (row, data, index) {

        },
    });

    $(table_report_vendor_mockup.table().container()).on('keyup', 'input', function () {
        table_report_vendor_mockup
            .column($(this).data('index'))
            .search(this.value)
            .draw();
    });

    //hiding global search box
    $(".dataTables_filter").css("display", "none");
}

