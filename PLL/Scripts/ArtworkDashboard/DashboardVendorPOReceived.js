var table_vendor_po_received;
$(document).ready(function () {

    $('.cls_page_dashboard .cls_vendor_po_received_date_from').val(GetPreviousDate(90));
    $('.cls_page_dashboard .cls_vendor_po_received_date_to').val(GetCurrentDate2());

    if ($(".cls_li_vendor_po_received").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_vendor_po_received thead tr').clone(true).appendTo('#table_vendor_po_received thead');
        $('#table_vendor_po_received thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 2 || i == 3) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '"  data-index="' + i + '" />');
            }
        });

        table_vendor_po_received = $('#table_vendor_po_received').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            //dom: 'rBfrtip',
            //buttons: [
            //    {
            //        text: 'Confirm selected PO',
            //        className: 'btn-success cls_btn_vendor_po_selected',
            //        action: function () {
            //            var data = table_vendor_po_received.rows({ selected: true }).data();
            //            confirmSelectedPO(data);
            //        }
            //    }
            //],
            select: {
                'style': 'multi'
            },
            stateSave: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/dashboard/povendor",
                    url: suburl + "/api/dashboard/povendor?"
                        + 'data.get_by_create_date_from=' + $('.cls_vendor_po_received_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_vendor_po_received_date_to').val(),
                    type: 'GET',
                    success: function (res) {
                        dtSuccess(res, callback);
                    }
                });
            },
            "columnDefs": [
                {
                    "searchable": false,
                    "orderable": false,
                    "targets": 0
                },
                { type: 'znatural', targets: 2 }
            ],
            "order": [[1, 'asc']],
            "processing": true,
            "lengthChange": false,
            "ordering": true,
            "info": true,
            "searching": true,
            "scrollX": true,
            "autoWidth": false,
            "columns": [
                {
                    data: null,
                    defaultContent: '',
                    className: 'select-checkbox',
                    orderable: false
                },
                { "data": "PURCHASE_ORDER_NO" },
                { "data": "PURCHASING_ORG", "className": "cls_view_po" },
                { "data": "PURCHASER", "className": "cls_download_all_artwork" }
            ],
            "rowCallback": function (row, data, index) {
                $(row).find('.cls_view_po').html('<a target="_blank" href="' + suburl + '/FileUpload/DownloadPO?po=' + data.PO_NO2 + '" style="text-decoration: underline;">View PO</a>');
                $(row).find('.cls_download_all_artwork').html('<a target="_blank" href="' + suburl + '/FileUpload/DownloadArtworkVendor?po=' + data.PO_NO2 + '" style="text-decoration: underline;">Download all artwork</a>');
            },
            "drawCallback": function (settings) {
                $('.cls_cnt_vendor_po_received').text('(' + this.api().data().count() + ') ');
            },
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_vendor_po_received').text('(' + json.recordsTotal + ') ');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_vendor_po_received").click(function (e) {
            table_vendor_po_received.ajax.reload();
        });

        $(".cls_page_dashboard .cls_btn_vendor_po_selected").click(function (e) {
            var data = table_vendor_po_received.rows({ selected: true }).data();
            confirmSelectedPO(data);
        });

        $(table_vendor_po_received.table().container()).on('keyup', 'input', function () {
            table_vendor_po_received
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_vendor_po_received_filter").hide();

        //table_vendor_po_received.on('order.dt search.dt', function () {
        //    table_vendor_po_received.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
        //        cell.innerHTML = i + 1;
        //    });
        //}).draw();

        $("a[href='#view_vendor_po_received']").on('shown.bs.tab', function (e) {
            table_vendor_po_received.columns.adjust().draw();
        });

        table_vendor_po_received.search('').columns().search('').draw();
    }
});

var dataPO = [];
function confirmSelectedPO(data) {

    dataPO = [];
    if (data.length > 0) {
        for (i = 0; i < data.length; i++) {
            var item = {};
            item["ACTION_CODE"] = 'APPROVE';
            item["PURCHASE_ORDER_NO"] = data[i].PURCHASE_ORDER_NO;
            item["UPDATE_BY"] = UserID;
            item["CONFIRM_PO"] = 'X';
            item["ENDTASKFORM"] = true;
            dataPO.push(item);
        }

        var jsonObj = new Object();
        jsonObj.data = dataPO;
        var myurl = '/api/taskform/pp/vendor/porelateartwork';
        var mytype = 'POST';
        var mydata = jsonObj;

        myAjaxNoAlertSuccess(myurl, mytype, mydata, callbackGetPORelateArtwork);
    }
    else {
        alertError2('Please select at least 1 item.');
    }
}

var aw_encrypt = "";
function callbackGetPORelateArtwork(res) {

    dataPO = [];
    if (res.data.length > 0) {
        for (i = 0; i < res.data[0].LIST_CONFIRM_PO_DISPLAY_TXT.length; i++) {
            var item = {};
            item["ACTION_CODE"] = 'APPROVE';
            item["PURCHASE_ORDER_NO"] = res.data[0].LIST_CONFIRM_PO_DISPLAY_TXT[i];
            item["UPDATE_BY"] = UserID;
            item["CONFIRM_PO"] = 'X';
            item["ENDTASKFORM"] = true;
            dataPO.push(item);
        }

        aw_encrypt = "";
        aw_encrypt = res.data[0].AW_ENCRYPT;

        var jsonObj = new Object();
        jsonObj.data = dataPO;
        var myurl = '/api/taskform/pp/vendor/multiconfirm';
        var mytype = 'POST';
        var mydata = jsonObj;
        // myAjaxConfirmSubmit(myurl, mytype, mydata, callback_confirmSelectedPO);

        var content = 'Do you want to confirm related PO and download all of PO attachment as below? <br><br>' + res.data[0].CONFIRM_PO_DISPLAY_TXT;
        myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback_confirmSelectedPO, '', true, true, content);

      
    }
}

function callback_confirmSelectedPO(res) {
    if (res.status == "E" && res.msg != '') {
        alertError(res.msg);
    }
    else if (res.status == "S") {
        table_vendor_po_received.ajax.reload();
        $('.cls_cnt_vendor_po_received').text('(' + table_vendor_po_received.data().count() + ') ');

        DownloadPOByAW(aw_encrypt);
    }
}

function DownloadPOByAW(data) {
    
    var url = suburl + '/FileUpload/DownloadPOByAW?artworkNo=' + data;
    window.open(url, '_blank');
}


