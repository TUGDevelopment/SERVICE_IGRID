$(document).ready(function () {

    $(document).on("change", ".cls_chk_po_active", function () {
        var jsonObj = new Object();
        jsonObj.data = {};
        var item = {};
        //jsonObj.data = [];
        //$('#table_po_tab_artwork > tbody  > tr').each(function () {
        //    var itemPO = {};
        //    itemPO["ARTWORK_MAPPING_PO_ID"] = $(this).closest('tr').find('.cls_po_id').val();
        //    itemPO["IS_ACTIVE"] = $(this).closest('tr').find(".cls_chk_po_active").is(":checked") ? "X" : "";
        //    jsonObj.data.push(itemPO);
        //});
        item["PO_NO"] = $(this).closest('tr').find('.cls_po_id').val();
        item["IS_ACTIVE"] = $(this).closest('tr').find(".cls_chk_po_active").is(":checked") ? "X" : "";

        jsonObj.data = item;

        var myurl = '/api/taskform/pomappingaw/info';
        var mytype = 'POST';
        var mydata = jsonObj;
        myAjax(myurl, mytype, mydata);
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_po':
                if (table_po_tab_artwork == null)
                    bind_art_po_tab_art();
                else
                    table_po_tab_artwork.ajax.reload();
                break;
            default:
                break;
        }
    });
});


var table_po_tab_artwork;
function bind_art_po_tab_art() {
    table_po_tab_artwork = $('#table_po_tab_artwork').DataTable({
        ajax: function (data, callback, settings) {
            $.ajax({
                url: suburl + '/api/taskform/pomappingaw/info?data.artwork_no=' + ARTWORK_NO,
                type: 'GET',
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        "columnDefs": [{
            "searchable": false,
            "orderable": false,
            "targets": 0
        }],
        "pageLength": 25,
        "order": [[1, 'asc']],
        "processing": true,
        "lengthChange": true,
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": false,
        "scrollX": true,
        "columns": [
            {
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }, "className": "cls_center"
            },
            { "data": "PO_NO", "className": "" },
            { "data": "CURRENCY_DISPLAY_TXT", "className": "" },
            { "data": "PURCHASING_ORG_DISPLAY_TXT", "className": "" },
            { "data": "VENDOR_DISPLAY_TXT", "className": "" },
            { "data": "PURCHASER_DISPLAY_TXT", "className": "" },
            {
                render: function (data, type, row, meta) {
                    return '<a target="_blank" href="' + suburl + '/FileUpload/DownloadPO?po=' + row.PO_NO2 + '" style="text-decoration: underline;"> Download </a>';
                }, "className": "cls_center"
            },
            {
                render: function (data, type, row, meta) {
                    if (row.IS_ACTIVE == 'X') {
                        return "<input class='cls_chk_po_active' checked='checked' type='checkbox'><input type='hidden' class='cls_po_id' value='" + row.PO_NO + "'>";
                    }
                    else {
                        return "<input class='cls_chk_po_active' type='checkbox'><input type='hidden' class='cls_po_id' value='" + row.PO_NO + "'>";
                    }
                }, "className": "cls_td_width_60 cls_center"
            },

        ],
    });

    table_po_tab_artwork.on('order.dt search.dt', function () {
        table_po_tab_artwork.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}
