var first_load = false;
$(document).ready(function () {


    if ($(".cls_li_igird_infogroup").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_igrid_infogroup_view thead tr').clone(true).appendTo('#table_igrid_infogroup_view thead');
        $('#table_igrid_infogroup_view thead tr:eq(1) th').each(function (i) {
            if (i == 0) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });

        var table_igrid_view = $('#table_igrid_infogroup_view').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            fixedColumns: {
                leftColumns: 3
            },
            stateSave: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    url: suburl + "/api/taskform/igrid/infogroup",
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
                //{ type: 'znatural', targets: 3 },
            ],
            "order": [[3, 'asc']],
            "processing": true,
            "searching": true,
            "lengthChange": false,
            "ordering": true,
            "info": true,
            "scrollX": true,
            paging: false,
            "scrollY": "380px",
            "scrollCollapse": true,
            // "autoWidth": false,
            "columns": [
                //{ "data": "PLANT", "className": "cls_nowrap" },
                { "data": "ID", "className": "cls_nowrap" },
         
                { "data": "CONDITION", "className": "cls_nowrap" },
                { "data": "REQUESTTYPE", "className": "cls_nowrap" },
                { "data": "DOCUMENTNO", "className": "cls_nowrap cls_DOCUMENTNO" },
                { "data": "DMSNO", "className": "cls_nowrap" },
                { "data": "MATERIAL", "className": "cls_nowrap" },
                { "data": "DESCRIPTION", "className": "cls_nowrap" },
                { "data": "MATERIALGROUP", "className": "cls_nowrap" },
                { "data": "BRAND", "className": "cls_nowrap" },
                { "data": "ASSIGNEE", "className": "cls_nowrap" },
                { "data": "CREATEON", "className": "cls_nowrap" },
                { "data": "ACTIVEBY", "className": "cls_nowrap" },
                { "data": "FINALINFOGROUP", "className": "cls_nowrap" },
                { "data": "REFERENCEMATERIAL", "className": "cls_nowrap" },
                { "data": "VENDER", "className": "cls_nowrap" },
                { "data": "VENDERDESCRIPTION", "className": "cls_nowrap" },
             
            ],
            dom: 'Bfrtip',
            buttons: [
                {
                    title: 'Inbox',
                    extend: 'excelHtml5',
                    exportOptions: {
                        // columns: [2, 3, 4, 5, col_export_excel_wf_status, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23], //#TSK-1511 #SR-70695 by aof in 09/2022 for adding 23
                        format: {
                            body: function (data, row, column, node) {

                                return data;
                                //}
                            }
                        }
                    }
                }
            ],

            "rowCallback": function (row, data, index) {
                $(row).find('.cls_DOCUMENTNO').html('<a target="_blank" href="' + suburl + '/IGrid/' + data.ID + '"> ' + data.DOCUMENTNO + ' </a>');
            },
            "drawCallback": function (settings) {
                $('.cls_cnt_igrid_infogroup_view').text('(' + this.api().data().count() + ') ');
            },

        });

        $(".cls_page_dashboard .cls_btn_search_igrid_info").click(function (e) {

            table_igrid_view.ajax.reload();
        });


        $("#view_igrid_infogroup .cls_btn_excel_igrid_info").click(function () {
            $("#view_igrid_infogroup .buttons-excel").click();
        });

        $(table_igrid_view.table().container()).on('keyup', 'input', function () {
            table_igrid_view
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_igrid_infogroup_view_filter").hide();
        $("#table_igrid_infogroup_view_wrapper .dt-buttons").hide();

        //table_igrid_view.on('order.dt search.dt', function () {
        //    table_igrid_view.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
        //        cell.innerHTML = i + 1;
        //    });
        //}).draw();

        $("a[href='#view_igrid_infogroup']").on('shown.bs.tab', function (e) {
            table_igrid_view.columns.adjust().draw();
            //if (table_incoming_inbox == null)
            //    table_incoming_inbox.columns.adjust().draw();
            //else
            //    table_incoming_inbox.ajax.reload();
        });

        table_igrid_view.search('').columns().search('').draw();


     

    }

    first_load = false;
});