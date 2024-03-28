var first_load_new = true;
$(document).ready(function () {

    //$('.cls_page_dashboard .cls_so_new_create_date_from').val('');
    //$('.cls_page_dashboard .cls_so_new_create_date_to').val('');
    //$('.cls_page_dashboard .cls_so_new_rdd_from').val(GetCurrentDate2);
    //$('.cls_page_dashboard .cls_so_new_rdd_to').val(GetNextDate(60));
    if ($(".cls_li_igird").is(':visible')) {
        $('#table_igrid_view thead tr:eq(1) th').each(function (i) {
            if (i == 0) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control cls_txt_search_' + i + ' " placeholder="Search ' + title + '"  data-index="' + i + '" />');
            }
        });
        var groupColumnPPView = 2;
        //debugger;
        var table_igrid_view = $('#table_igrid_view').DataTable({
            "scrollY": "600px",
            "scrollCollapse": true,
            paging: false,
            orderCellsTop: true,
            stateSave: false,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/taskform/pp/incoming",
                    url: suburl + "/api/taskform/igrid/result",
                    type: 'GET',
                    success: function (res) {
                        dtSuccess(res, callback);
                    }
                });
            },
            "order": [[0, 'desc']],
            "processing": true,
            "lengthChange": false,
            //select: {
            //    style: 'os',
            //    selector: 'td:first-child'
            //},
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
            columns: [
                //{
                //    data: null,
                //    defaultContent: '',
                //    className: 'select-checkbox',
                //    orderable: false
                //},
                //{
                //    render: function (data, type, row, meta) {
                //        return meta.row + meta.settings._iDisplayStart + 1;
                //    }
                //},
                { data: "ID", "className": "cls_nowrap" },
                { data: "STATUSBY", "className": "cls_nowrap" },
                
                { data: "CONDITION", "className": "cls_nowrap" },
                { data: "REQUESTTYPE", "className": "cls_nowrap" },
                { data: "DOCUMENTNO", "className": "cls_nowrap cls_DOCUMENTNO" },
                { data: "DMSNO", "className": "cls_nowrap" },
                { data: "MATERIAL", "className": "cls_nowrap" },
                { data: "DESCRIPTION", "className": "cls_nowrap" },
                
                { data: "MATERIALGROUP", "className": "cls_nowrap" },
                { data: "BRAND", "className": "cls_nowrap" },
                { data: "ASSIGNEE", "className": "cls_nowrap" },
                { data: "CREATEON", "className": "cls_nowrap" },
                { data: "ACTIVEBY", "className": "cls_nowrap" },
                
                { data: "ACTION", "className": "cls_nowrap" }
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
                $('.cls_cnt_igrid_view').text('(' + this.api().data().count() + ') ');
            },
            "createdRow": function (row, data, index) {
                if (data.STATUS == "1")
                    $(row).css("color", "#A20025");
            },
            "rowCallback": function (row, data, index) {
                $(row).find('.cls_DOCUMENTNO').html('<a target="_blank" href="' + suburl + '/IGrid/' + data.ID + '"> ' + data.DOCUMENTNO + ' </a>');

                //if (!isEmpty(data.RDD))
                //    $(row).find('.cls_rdd').html(myDateMoment(data.RDD));

                //if (!isEmpty(data.RECEIVE_DATE))
                //    $(row).find('.cls_receive_date').html(myDateTimeMoment(data.RECEIVE_DATE));
            },
            //"drawCallback": function (settings) {

            //},
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_igrid_view').text('(' + json.data.length + ') ');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_igrid_view").click(function (e) {
            //if ($('.cls_lov_igrid_type').val() == '' || $('.cls_txt_search_keyword').val() == '')
            //    alertError2('Please select Receiving date.');
            //else
                table_igrid_view.ajax.reload();
        });

        $(table_igrid_view.table().container()).on('keyup', 'input', function () {
            table_igrid_view
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_igrid_view_filter").hide();
        $("#table_igrid_view_wrapper .dt-buttons").hide();

        //table_igrid_view.on('order.dt search.dt', function () {
        //    table_igrid_view.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
        //        cell.innerHTML = i + 1;
        //    });
        //}).draw();

        $("a[href='#view_igrid_view']").on('shown.bs.tab', function (e) {
            table_igrid_view.columns.adjust().draw();
            //if (table_igrid_view == null)
            //    table_igrid_view.columns.adjust().draw();
            //else
            //    table_igrid_view.ajax.reload();
        });

        table_igrid_view.search('').columns().search('').draw();
    }
    
    $("#view_igrid .cls_btn_excel_igrid").click(function () {
        window.open('/excel/igridviewReport?data.get_by_type = ' + $('.cls_lov_igrid_type').val() + ' & data.get_by_keywork='+$('.cls_txt_search_keyword').val()
            , '_blank');
    });
    function submit(myurl, mytype, mydata) {
        $.ajax({
            url: suburl + myurl,
            type: mytype,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(mydata),
            success: function (res) {
                res = DData(res);
                if (res.status == 'S') {
                    alertSuccess("Accept task & Export excel successfully.");
                    //show button close after send to pp
                    $("#view_igrid_view .cls_btn_search_igrid_view").click();
                }
            }
        });
    };
    first_load_new = false;
});