$(document).ready(function () {

    $('.cls_page_dashboard .cls_pp_view_date_from').val(GetPreviousDate(90));
    $('.cls_page_dashboard .cls_pp_view_date_to').val(GetCurrentDate2());

    if ($(".cls_li_pp_view").is(':visible')) {
        // Setup - add a text input to each footer cell
        $('#table_pp_view thead tr').clone(true).appendTo('#table_pp_view thead');
        $('#table_pp_view thead tr:eq(1) th').each(function (i) {
            if (i == 0) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control cls_txt_search_'+ i +' " placeholder="Search ' + title + '"  data-index="' + i + '" />');
            }
        });
        var groupColumnPPView = 2;

        var table_pp_view = $('#table_pp_view').DataTable({
            "scrollY": "600px",
            "scrollCollapse": true,
            paging: false,
            orderCellsTop: true,
            stateSave: false,
            ajax: function (data, callback, settings) {
                $.ajax({
                    //url: suburl + "/api/taskform/pp/incoming",
                    url: suburl + "/api/taskform/pp/incoming?"
                        + 'data.get_by_create_date_from=' + $('.cls_pp_view_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_pp_view_date_to').val(),
                    type: 'GET',
                    success: function (res) {
                        dtSuccess(res, callback);
                    }
                });
            },
            "columnDefs": [
                { "visible": false, "targets": groupColumnPPView },
                { "orderable": false, "targets": 0 },
            ],
            "order": [[1, 'desc']],
            "processing": true,
            "lengthChange": false,
            select: {
                'style': 'multi',
                selector: 'td:first-child input'
            },
            "ordering": true,
            "info": true,
            "searching": true,
            "scrollX": true,
            "autoWidth": false,
            dom: 'Bfrtip',
            buttons: [
                {
                    title: 'PP view',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16],
                        format: {
                            body: function (data, row, column, node) {
                                //if (column == 4) {
                                //    return myDateMoment(data);
                                //} else
                                if (column == 2) {
                                    return removeHtmlToComma(data);
                                }
                                //else if (column == 11) {
                                //    return myDateTimeMoment(data);
                                //}
                                else if (column == 12) {
                                    return removeHtml(data);
                                } else {
                                    return data;
                                }
                            }
                        }
                    }
                }
            ],
            columns: [
                {
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { data: "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "GROUPING", "className": "cls_nowrap" },
                { data: "SHIP_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "SALES_ORDER", "className": "cls_nowrap" },
                { data: "SALES_ORDER_ITEM", "className": "cls_nowrap" },
                { data: "PLANT", "className": "cls_nowrap" },
                { data: "SALES_ORG", "className": "cls_nowrap" },
                //{ data: "RDD", "className": "cls_nowrap cls_rdd" },
                {
                    className: "cls_rdd cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.RDD);
                    }
                },
                { data: "BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "PKG_TYPE_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "PRODUCT_CODE", "className": "cls_nowrap" },
                { data: "PKG_CODE", "className": "cls_nowrap" },
                { data: "VENDOR_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "WORKFLOW_NO", "className": "cls_nowrap cls_WORKFLOW_NO" },
                //{ data: "RECEIVE_DATE", "className": "cls_nowrap cls_receive_date" },
                {
                    className: "cls_receive_date cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateTimeMoment(row.RECEIVE_DATE);
                    }
                },
                { data: "REMARK_BY_PA", "className": "cls_nowrap" }
            ],
            "drawCallback": function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null; var j = 1;
                api.column(groupColumnPPView, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        var temp = [];
                        var str_grouping = "";
                        for (var x = 0; x < rows.data().length; x++) {
                            if (rows.data()[x].GROUPING === group) {
                                temp.push(rows.data()[x].ARTWORK_SUB_ID);
                            }
                        }
                        if (temp.length > 0)
                            str_grouping = temp.join("|");
                        $(rows).eq(i).before('<tr class="group highlight"><td><input data-group-name="' + str_grouping + '" class="cls_chk_group" type="checkbox"/></td><td colspan="16">Group ' + j +'</td></tr>');
                        last = group;
                        j++;
                    }
                });
                $('.cls_cnt_pp_view').text('(' + this.api().data().count() + ') ');
            },
            "createdRow": function (row, data, index) {
                if (data.STATUS=="1")
                $(row).css("color", "#A20025");
            },
            "rowCallback": function (row, data, index) {
                $(row).find('.cls_WORKFLOW_NO').html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_SUB_ID + '"> ' + data.WORKFLOW_NO + ' </a>');

                //if (!isEmpty(data.RDD))
                //    $(row).find('.cls_rdd').html(myDateMoment(data.RDD));

                //if (!isEmpty(data.RECEIVE_DATE))
                //    $(row).find('.cls_receive_date').html(myDateTimeMoment(data.RECEIVE_DATE));
            },
            //"drawCallback": function (settings) {

            //},
            //"initComplete": function (settings, json) {
            //    $('.cls_cnt_pp_view').text('(' + json.data.length + ') ');
            //}
        });

        $(".cls_page_dashboard .cls_btn_search_pp_view").click(function (e) {
            if ($('.cls_pp_view_date_from').val() == '' || $('.cls_pp_view_date_to').val() == '')
                alertError2('Please select Receiving date.');
            else
                table_pp_view.ajax.reload();
        });

        $(table_pp_view.table().container()).on('keyup', 'input', function () {
            table_pp_view
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        //hide search table and use search global
        $("#table_pp_view_filter").hide();
        $("#table_pp_view_wrapper .dt-buttons").hide();

        table_pp_view.on('order.dt search.dt', function () {
            table_pp_view.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_pp_view']").on('shown.bs.tab', function (e) {
            table_pp_view.columns.adjust().draw();
            //if (table_pp_view == null)
            //    table_pp_view.columns.adjust().draw();
            //else
            //    table_pp_view.ajax.reload();
        });

        table_pp_view.search('').columns().search('').draw();
    }

    $("#view_pp_view .cls_btn_excel_pp_view").click(function () {
        window.open('/excel/ppviewReport?data.get_by_create_date_from=' + $('.cls_pp_view_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_pp_view_date_to').val()
            + '&data.SOLD_TO_DISPLAY_TXT=' + $(".cls_txt_search_1").val()
            + '&data.SHIP_TO_DISPLAY_TXT=' + $(".cls_txt_search_3").val()
            + '&data.SALES_ORDER=' + $(".cls_txt_search_4").val()
            + '&data.SALES_ORDER_ITEM=' + $(".cls_txt_search_5").val()
            + '&data.PLANT=' + $(".cls_txt_search_6").val()
            + '&data.SALES_ORG=' + $(".cls_txt_search_7").val()
            + '&data.RDD=' + $(".cls_txt_search_8").val()
            + '&data.BRAND_DISPLAY_TXT=' + $(".cls_txt_search_9").val()
            + '&data.PKG_TYPE_DISPLAY_TXT=' + $(".cls_txt_search_10").val()
            + '&data.PRODUCT_CODE=' + $(".cls_txt_search_11").val()
            + '&data.PKG_CODE=' + $(".cls_txt_search_12").val()
            + '&data.VENDOR_DISPLAY_TXT=' + $(".cls_txt_search_13").val()
            + '&data.WORKFLOW_NO=' + $(".cls_txt_search_14").val()
            + '&data.RECEIVE_DATE=' + $(".cls_txt_search_15").val()
            + '&data.REMARK_BY_PA=' + $(".cls_txt_search_16").val()
            , '_blank');
        //$("#view_pp_view .buttons-excel").click();
        debugger;


       //var test =  $(".cls_txt_search_1").val();

        //var data = [];
        //$("#view_pp_view TBODY TR").each(function () {
        //    var row = $(this);
        //    var item = {};
        //    if (!row.find("TD").eq(0).html().startsWith("<input")) {
        //        item["SOLD_TO_DISPLAY_TXT"] = row.find("TD").eq(1).html();
        //        item["SHIP_TO_DISPLAY_TXT"] = row.find("TD").eq(2).html();

        //        item["SALES_ORDER"] = row.find("TD").eq(3).html();
        //        item["SALES_ORDER_ITEM"] = row.find("TD").eq(4).html();
        //        item["PLANT"] = row.find("TD").eq(5).html();
        //        item["SALES_ORG"] = row.find("TD").eq(6).html();
        //        item["RDD"] = row.find("TD").eq(7).html();
        //        item["BRAND"] = row.find("TD").eq(8).html();
        //        item["PKG_TYPE"] = row.find("TD").eq(9).html();
        //        item["PRODUCT_CODE"] = row.find("TD").eq(10).html();
        //        item["PKG_CODE"] = row.find("TD").eq(11).html();
        //        item["VENDOR"] = row.find("TD").eq(12).html();
        //        item["WORKFLOW_NO"] = row.find("TD").eq(13).html();
        //        item["RECEIVE_DATE"] = row.find("TD").eq(14).html();
        //        item["REMARK_BY_PA"] = row.find("TD").eq(15).html();
        //        data.push(item);
        //    }
        //});
        //var jsonObj = new Object();
        //jsonObj.data = data;
        //$("#view_pp_view").table2excel({
        //    filename: "Table.xls"
        //});



        //var myurl = '/Excel/ppviewReport';
        //var mytype = 'POST';
        //var mydata = jsonObj;
        //myAjax(myurl, mytype, mydata, callbackSelectSendToPP2);
        //$.ajax({
        //    type: "POST",
        //    url: "/Excel/ppviewReport",
        //    data: JSON.stringify(data),
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    traditional: true,

        //     success: function (results) {
        //        if (results.redirect) {
        //            window.location.href = results.redirect;
        //        }
        //        else {
        //            // some here Ajax Process the expected Data results...
        //        }
        //    },
        //    error: function (xhr, textStatus, errorThrown) {
        //        alert('sorry, Error!  Status = ' + xhr.status);
        //    } 

        //});


        //var myurl = '/api/taskform/pp/exporttoexcel';
        //var mytype = 'POST';
        //var mydata = jsonObj;
        //myAjax(myurl, mytype, mydata, callbackSelectSendToPP2);

        //var url = "/excel/ppviewReport";
        //var name = $("#Name").val();
        //var address = $("#Address").val();
        //$.post(url, { data: jsonObj }, function (data) {
        //    $("#msg").html(data);
        //}); 
        //Send the JSON array to Controller using AJAX.
        //$.ajax({
        //    type: 'POST',
        //    url: suburl + '/api/taskform/pp/exporttoexcel',
        //    data: jsonObj,
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json',
        //    success: function (response) {
        //    }
        //});
    });

    //function callbackSelectSendToPP2(res) {

    //}

    $("#view_pp_view .cls_btn_accept_pp_view").click(function () {
        // Get group class name
        var temp = [];
        $("#table_pp_view .cls_chk_group:checked").each(function () {
            //var a = {};
            //a["group"] = $(this).data("group-name");
            temp.push($(this).data("group-name"));
        });
        if (temp.length == 0) {
            alertError2("Please select at least 1 item.");
        } else {
            var item = {};
            var jsonObj = new Object();
            item["REMARK_OTHERS"] = temp.join("|");
            item["CURRENT_USER_ID"] = UserID;
            item["UPDATE_BY"] = UserID;
            jsonObj.data = item;
            var mydata = jsonObj;
            var myurl = '/api/taskform/pp/accepttask';
            var mytype = 'POST';
            //if (confirm("Do you want to submit?")) {
            //    $("#view_pp_view .buttons-excel").click();
            //    submit(myurl, mytype, mydata);
            //}
            myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSelectSuggestAccept);
        }
    });
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
                $("#view_pp_view .cls_btn_search_pp_view").click();
            }
        }
    });
};

function callbackSelectSuggestAccept(res) {
    if (res.status == 'S') {
        var strAlert = "";
        if (res.data.length > 0) {
            for (var i = 0; i < res.data.length; i++) {
                if (res.data[i].REASON_BY_OTHER != null) {
                    strAlert = strAlert+"<br>"+res.data[i].REASON_BY_OTHER; 
                }
            }
        }
        if (strAlert != "")
            alertError2("Unable accept workflow:" + strAlert);
        //$("#view_pp_view .cls_btn_search_pp_view").click();
    }
};

