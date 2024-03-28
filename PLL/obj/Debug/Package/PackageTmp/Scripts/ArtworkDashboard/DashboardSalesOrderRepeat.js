var first_load_repeat = true;
var pp_quill;
var listComment;
var f_tu_tuning = false; // by aof for tuning create so repeat


$(document).ready(function () {
    $('.cls_page_dashboard .cls_so_repeat_create_date_from').val('');
    $('.cls_page_dashboard .cls_so_repeat_create_date_to').val('');
    $('.cls_page_dashboard .cls_so_repeat_rdd_from').val(GetCurrentDate2());
    $('.cls_page_dashboard .cls_so_repeat_rdd_to').val(GetNextDate(60));
    //pp_quill = bind_text_editor(pp_submit_modal + '.cls_txt_send_pp');
    if ($(".cls_li_incoming_so_repeat").is(':visible')) {
        $('#table_so_repeat thead tr').clone(true).appendTo('#table_so_repeat thead');
        $('#table_so_repeat thead tr:eq(1) th').each(function (i) {
            if (i == 0 || i == 1 || i == 23) {
                $(this).html('');
            } else {
                var title = $(this).text();
                $(this).html('<input type="text" class="form-control" placeholder="Search ' + title + '" data-index="' + i + '" />');
            }
        });

        var groupColumn = 2;
        var table_so_repeat = $('#table_so_repeat').DataTable({
            orderCellsTop: true,
            fixedHeader: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    url: suburl + "/api/dashboard/incomingsalesorderrepeat?"
                        + 'data.get_by_create_date_from=' + $('.cls_so_repeat_create_date_from').val() + '&data.get_by_create_date_to=' + $('.cls_so_repeat_create_date_to').val()
                        + '&data.get_by_rdd_from=' + $('.cls_so_repeat_rdd_from').val() + '&data.get_by_rdd_to=' + $('.cls_so_repeat_rdd_to').val()
                        + '&data.get_by_packaging_type=' + encodeURIComponent($('.cls_lov_artwork_packaging_type option:selected').text())
                        + '&data.get_by_sold_to=' + encodeURIComponent($('.cls_lov_artwork_sold_to option:selected').text())
                        + '&data.get_by_ship_to=' + encodeURIComponent($('.cls_lov_artwork_ship_to option:selected').text())
                        + '&data.first_load=' + first_load_repeat
                        + '&data.get_by_brand=' + encodeURIComponent($('.cls_lov_artwork_brand option:selected').text())
                        + '&data.grouping=' + $('.cls_lov_so_repeat_group').val(),
                    type: 'GET',
                    success: function (res) {
                        dtSuccess(res, callback);
                    }
                });
            },
            "columnDefs": [
                {
                    "orderable": false, "targets": 0
                },
                { "orderable": false, "targets": 1 },
                { "visible": false, "targets": groupColumn },
                { "visible": false, "targets": 3 },
                { "orderable": false, "targets": 23 },
            ],
            "scrollX": true,
            "lengthChange": false,
            select: {
                'style': 'multi',
                selector: 'td:first-child input,td:last-child input'
            },
            "scrollY": "350px",
            "scrollCollapse": true,
            "paging": false,
            dom: 'Bfrtip',
            buttons: [
                {
                    title: 'Incoming_sales_order_repeat',
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: [4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23],
                        format: {
                            body: function (data, row, column, node) {
                                //if (column == 15 || column == 16) {
                                //    return myDateMoment(data);
                                //} else {
                                return data;
                                //}
                            }
                        }
                    }
                }
            ],
            columns: [
                {
                    render: function (data, type, row, meta) {
                        return '<input class="cls_chk_so_repeat" type="checkbox">';
                    }
                },
                {
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { data: "GROUPING", "className": "" },
                { data: "GROUPING_DISPLAY_TXT", "className": "" },
                //{
                //    visible: false,
                //    className: "cls_td_width_80",
                //    render: function (data, type, row, meta) {
                //        var min = row.GROUP_MIN_ROW;
                //        var max = row.GROUP_MAX_ROW;
                //        var result = '';
                //        if (row.PRODUCT_CODE.startsWith("5") && !isEmpty(row.ITEM_CUSTOM_1) && row.ITEM_CUSTOM_1 != '0') {

                //        }
                //        else {
                //            result = '<select class="cls_lov_group" style="width:50px;height:20px !important;"><option value="0" ></option>';
                //            for (var i = 1; i <= 10; i++) {
                //                result = result + '<option value="' + i + '" >' + i + '</option>';
                //            }
                //            result = result + '</select>';
                //        }

                //        return result;
                //    }
                //},
                { data: "SALES_ORDER_NO", "className": "cls_nowrap" },
                { data: "ITEM", "className": "cls_nowrap" },
                { data: "PRODUCT_CODE", "className": "cls_nowrap" },
                { data: "SALES_ORG", "className": "cls_nowrap" },
                { data: "PRODUCTION_PLANT", "className": "cls_nowrap" },
                { data: "SOLD_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "SHIP_TO_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "ADDITIONAL_BRAND_DISPLAY_TXT", "className": "cls_nowrap" },
                { data: "COUNTRY", "className": "cls_nowrap" },
                { data: "IN_TRANSIT_TO", "className": "cls_nowrap" },
                { data: "COMPONENT_ITEM", "className": "cls_nowrap" },
                { data: "COMPONENT_MATERIAL", "className": "cls_nowrap" },
                { data: "DECRIPTION", "className": "cls_nowrap" },
                //{ data: "BOM_ITEM_CUSTOM_1", "className": "cls_nowrap" },
                { data: "BOM_STOCK", "className": "cls_nowrap" },
                { data: "QUANTITY", "className": "cls_nowrap" },
                { data: "ACTION", "className": "cls_nowrap" },
                //{ data: "CREATE_ON", "className": "cls_nowrap cls_create_on" },
                {
                    className: "cls_create_on cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.CREATE_ON);
                    }
                },
                //{ data: "RDD", "className": "cls_nowrap cls_rdd" },
                {
                    className: "cls_rdd cls_nowrap",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.RDD);
                    }
                },

                { data: "RECHECK_ARTWORK", "className": "cls_nowrap cls_center cls_recheck_artwork" },
                {
                    render: function (data, type, row, meta) {
                        return '<input class="cls_chk_so_repeat" type="checkbox">';
                    }
                },
            ],
            "drawCallback": function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;
                var j = 1;
                //+++++++++++++++++++
                var val = $(".cls_lov_so_repeat_group").val();
                if (val == "yes") {
                    api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                        var str_moredetails = "";
                        var str_grouping = "";
                        for (var x = 0; x < rows.data().length; x++) {
                            if (rows.data()[x].GROUPING == group) {
                                str_moredetails = replaceHtml(rows.data()[x].GROUPING_DISPLAY_TXT);
                                str_grouping = rows.data()[x].GROUPING;
                            }
                        }

                        if (last !== group) {

                            $(rows).eq(i).before(
                                '<tr class="group highlight"><td><input data-group-name="' + "group-" + str_grouping + '" class="cls_chk_group" type="checkbox"/></td>  <td colspan="23"> Group ' + (j) + ' <span title="' + str_moredetails + '" class="cls_hand label label-info">more details</span></td></tr>'
                            );
                            last = group;
                            j++;
                        }
                    });
                } 
                $('.cls_btn_select_so_repeat').prop("disabled", val == "no" ? true : false);
                //+++++++++++++++++
                $('.cls_cnt_incoming_so_repeat').text('(' + api.rows().data().count() + ') ');
            },
            "processing": true,
            "rowCallback": function (row, data, index) {
                //if (data.RDD != "") {
                //    $(row).find('.cls_rdd').html(myDateMoment(data.RDD));
                //}
                //if (data.CREATE_ON != "") {
                //    $(row).find('.cls_create_on').html(myDateMoment(data.CREATE_ON));
                //}

                if (data.RECHECK_ARTWORK == "Yes") {
                    $(row).find('.cls_recheck_artwork').css('color', 'red');
                }

                if (data.PRODUCT_CODE.startsWith("5") && !isEmpty(data.ITEM_CUSTOM_1) && data.ITEM_CUSTOM_1 != '0') {
                    //FOC
                    $(row).find('.cls_chk_so_repeat').remove();
                }
                else {
                    $(row).addClass('group-' + data.GROUPING);
                }
            },
            order: [4, 'asc'],
            "orderFixed": [2, 'asc'],
            //initComplete: function (settings, json) {
            //    $('.cls_cnt_incoming_so_repeat').text('(' + json.recordsTotal + ') ');
            //}
        });

        $(document).on('click', '.cls_page_dashboard .cls_chk_group', function (e) {
            // Get group class name
            var groupName = $(this).data('group-name');
            var tempCheck = this.checked;

            var temp = table_so_repeat.rows({ search: 'applied' });
            temp.rows('tr.' + groupName).every(function (rowIdx, tableLoop, rowLoop) {
                var rowNode = this.node();

                //if ($(".cls_lov_submit_type option:selected").val() != "submit") {
                //    var group = $(rowNode).find('.cls_lov_group');
                //    group.val("1");
                //}
                //else {
                if ($(rowNode).is(':visible')) {
                    table_so_repeat.row(rowIdx).select(tempCheck);
                    $(rowNode).find('.cls_chk_so_repeat').prop('checked', tempCheck);
                }
                //}
            });
        });

        $(document).on('click', '#table_so_repeat .cls_chk_so_repeat', function (e) {
            if ($(this).is(':checked')) {
                table_so_repeat.rows($(this).closest('tr')).select();
            }
            else {
                table_so_repeat.rows($(this).closest('tr')).deselect();
            }
            $(this).closest('tr').find('.cls_chk_so_repeat').prop('checked', this.checked);
        });

        $(table_so_repeat.table().container()).on('keyup', 'input', function () {
            table_so_repeat
                .column($(this).data('index'))
                .search(this.value)
                .draw();
        });

        $('table').on('click', function (e) {
            if ($('.popoverButton').length > 1)
                $('.popoverButton').popover('hide');
            $(e.target).popover('toggle');
        });

        $("#table_so_repeat_filter").hide();
        $("#table_so_repeat_wrapper .dt-buttons").hide();

        table_so_repeat.on('order.dt search.dt', function () {
            table_so_repeat.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $("a[href='#view_so_repeat']").on('shown.bs.tab', function (e) {
            if (first_click_tab_so_repeat) {
                first_click_tab_so_repeat = false;
                table_so_repeat.columns.adjust().draw();
            }
        });

        $(".cls_page_dashboard .cls_btn_search_so_repeat").click(function (e) {
            table_so_repeat.ajax.reload();
        });

        $("#view_so_repeat .cls_btn_excel_so_repeat").click(function () {
            $("#view_so_repeat .buttons-excel").click();
        });


        

        $("#view_so_repeat .cls_btn_select_so_repeat_tuning").click(function () {
            var data = table_so_repeat.rows({ selected: true }).data();



            selectedSORepeat_TUTuning(data);
            f_tu_tuning = true;

            //if (f_tu_tuning) {
            //    selectedSORepeat_TUTuning(data);
            //    // selectedSORepeat_tuning(data);
            //} else {
            //    selectedSORepeat(data);
            //}        
        });

        $(".cls_page_dashboard .cls_btn_select_so_repeat").click(function (e) {
            var data = table_so_repeat.rows({ selected: true }).data();
            selectedSORepeat(data);
            f_tu_tuning = false;
            //if ($(".cls_lov_submit_type").val() == "submit") {
            //    let listGroup = selectGroup.map(a => a.GROUPING).filter((v, i, a) => a.indexOf(v) === i);

            //    if (listGroup.length) {

            //    }
            //}
            //else {
            //    data = [];

            //    //get data all group
            //    var selectGroup = table_so_repeat.rows({ selected: true }).data();

            //    //filter distinct group


            //    var table = $('#table_so_repeat').DataTable();
            //    var temp = table.rows();
            //    temp.rows().every(function (rowIdx, tableLoop, rowLoop) {
            //        var rowNode = this.node();
            //        var group = $(rowNode).find('.cls_lov_group');
            //        var selectGroup = group.find('option:selected').val();
            //        if (selectGroup != 0) {
            //            this.data().SELECTED_GROUP = selectGroup;
            //            data.push(this.data());
            //        }
            //    });
            //}

           
         
            //if (f_tu_tuning)
            //{
            //    selectedSORepeat_TUTuning(data);
            //    // selectedSORepeat_tuning(data);
            //} else {
            //    selectedSORepeat(data); 
            //}        
          
        });

        $(".cls_page_dashboard .cls_btn_unselect_so_repeat").click(function (e) {
            clearSelected();
        });

        //$("#dialog").dialog({
        //    autoOpen: false,
        //    modal: true
        //});
    }

    bind_lov('.cls_lov_artwork_packaging_type', '/api/lov/packtype', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_sold_to', '/api/lov/customersoldtoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_ship_to', '/api/lov/customershiptoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_lov_artwork_brand', '/api/lov/brandso', 'data.DISPLAY_TXT');

    first_load_repeat = false;

    var tablerepeat = $('#table_so_repeat').DataTable();
    //$(".cls_lov_submit_type")
    //    .change(function () {
    //        $(".cls_lov_submit_type option:selected").each(function () {
    //            if ($(this).val() == "submit") {
    //                tablerepeat.column(4).visible(false);
    //                tablerepeat.column(0).visible(true);
    //            }
    //            else {
    //                tablerepeat.column(4).visible(true);
    //                tablerepeat.column(0).visible(false);
    //            }
    //            clearSelected();
    //        });
    //    });

    $("#modal_for_information .cls_btn_submit_send_pp").click(function (e) {

        if (f_tu_tuning) {

            submitSendToPP_TUTuning();
        } else
        {
            $.confirm({
                title: 'Confirm Dialog',
                content: 'Do you want to submit?',
                animation: 'none',
                closeAnimation: 'none',
                type: 'blue',
                backgroundDismiss: false,
                backgroundDismissAnimation: 'glow',
                buttons: {
                    Yes: {
                        text: 'Yes',
                        btnClass: 'btn-primary cls_btn_confirm_ok',
                        action: function () {
                            if (listComment.length > 0) {
                                var countGroup = 0;
                                for (var i = 0; i < listComment.length; i++) {
                                    //find comment by cls name                
                                    var editor = new Quill('#modal_for_information .cls_comment' + listComment[i].data.CONTROL_NAME);
                                    listComment[i].data.COMMENT = editor.root.innerHTML;

                                    var mydata = listComment[i];
                                    var myurl = '/api/dashboard/selectedrepeatso';
                                    var mytype = 'POST';
                                    $.ajax({
                                        url: suburl + myurl,
                                        type: mytype,
                                        contentType: 'application/json; charset=utf-8',
                                        data: JSON.stringify(mydata),
                                        success: function (res) {
                                            res = DData(res);
                                            var cls = res.param.data.SALES_ORDER_REPEAT[0].SALES_ORDER_NO + res.param.data.SALES_ORDER_REPEAT[0].SALES_ORDER_ITEM + res.param.data.SALES_ORDER_REPEAT[0].COMPONENT_ITEM;
                                            var cls_name_return = '.cls_' + cls;

                                            //if (res.status == "S") {
                                            //    $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color:green;">Completed : <br/>' + res.WF_NO + '</span>');
                                            //}
                                            //else 
                                            if (res.status == "E") {
                                                $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color: red;">Error : <br/>' + res.msg + '</span>');
                                            }
                                            countGroup++;

                                            if (countGroup == listComment.length) {
                                                alertSuccess("Send to PP successfully.");
                                                //show button close after send to pp
                                                //$("#modal_for_information .cls_btn_close_send_pp").removeClass("cls_hide");
                                                $("#modal_for_information .cls_btn_submit_send_pp").addClass("cls_hide");
                                            }
                                        }
                                    });
                                }
                            }
                        }
                    },
                    No: {
                        text: 'No',
                        btnClass: 'btn-default cls_btn_confirm_no',
                        action: function () {

                        }
                    }
                }
            });
        }

     
    });
});
function ViewAssignedRequests() {
    //$('.grid-mvc').gridmvc();
    //$('#RequestGrid').reload();
    $.ajax({
        type: "GET",
        url: "/api/dashboard/SORepeat",   
        data: { xEdit: "" }
    }).done(function () {
        alert('Added');
    });
}


//---------------------------- tuning performance 2022 by aof---------------------------------//


var listRequestSORepeat = [];
function selectedSORepeat_TUTuning(data)
{

    if (data.length > 0) {

        listRequestSORepeat = [];
        listComment = [];

        if ($(".cls_lov_submit_type").val() == "submit") {
            //alertError("test submit");
            // summit only
            $.confirm({
                title: 'Confirm Dialog',
                content: 'Do you want to submit?',
                animation: 'none',
                closeAnimation: 'none',
                type: 'blue',
                backgroundDismiss: false,
                backgroundDismissAnimation: 'glow',
                buttons: {
                    Yes: {
                        text: 'Yes',
                        btnClass: 'btn-primary cls_btn_confirm_ok',
                        action: function () {

                            var sonumber = '';
                            var saleorderList = [];
                            var saleorderRepeatList = [];
                            for (i = 0; i < data.length; i++) {
                                if (sonumber.indexOf(data[i].SALES_ORDER_NO) == -1) {
                                    sonumber += '|' + data[i].SALES_ORDER_NO;

                                    var saleorderItem = {};
                                    saleorderItem["ARTWORK_REQUEST_ID"] = 0;
                                    //saleorderItem["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                    saleorderItem["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                    saleorderItem["CREATE_BY"] = UserID;
                                    saleorderItem["UPDATE_BY"] = UserID;
                                    saleorderItem["RECHECK_ARTWORK"] = data[i].RECHECK_ARTWORK;
                                    saleorderList.push(saleorderItem);
                                }

                                var saleorderRepeat = {};
                                saleorderRepeat["ARTWORK_REQUEST_ID"] = 0;
                                saleorderRepeat["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                saleorderRepeat["SALES_ORDER_ITEM"] = data[i].ITEM;
                                saleorderRepeat["PRODUCT_CODE"] = data[i].PRODUCT_CODE;
                                saleorderRepeat["COMPONENT_ITEM"] = data[i].COMPONENT_ITEM;
                                saleorderRepeat["COMPONENT_MATERIAL"] = data[i].COMPONENT_MATERIAL;
                                saleorderRepeat["SOLD_TO_DISPLAY_TXT"] = data[i].SOLD_TO_DISPLAY_TXT;
                                saleorderRepeat["SHIP_TO_DISPLAY_TXT"] = data[i].SHIP_TO_DISPLAY_TXT;
                                saleorderRepeat["BRAND_DISPLAY_TXT"] = data[i].BRAND_DISPLAY_TXT;
                                saleorderRepeat["CREATE_BY"] = UserID;
                                saleorderRepeat["UPDATE_BY"] = UserID;
                                saleorderRepeat["RECHECK_ARTWORK"] = data[i].RECHECK_ARTWORK;
                                saleorderRepeat["PRODUCTION_PLANT_DISPLAY_TXT"] = data[i].PRODUCTION_PLANT;  //by aof 20230121_3V_SOREPAT INC-93118
                                saleorderRepeatList.push(saleorderRepeat);


                            }


                            var recipientsList = [];
                            var recipientsItem = {};
                            recipientsItem["RECIPIENT_USER_ID"] = UserID;
                            recipientsItem["CREATE_BY"] = UserID;
                            recipientsItem["UPDATE_BY"] = UserID;
                            recipientsList.push(recipientsItem);

                            var item = {};

                            item["ARTWORK_REQUEST_ID"] = 0;
                            item["TYPE_OF_ARTWORK"] = 'REPEAT';
                            item["CREATE_BY"] = UserID;
                            item["UPDATE_BY"] = UserID;
                            item["CREATOR_ID"] = UserID;

                            item.SALES_ORDER = saleorderList;
                            item.REQUEST_RECIPIENT = recipientsList;
                            item.SALES_ORDER_REPEAT = saleorderRepeatList;
                            item.IS_COMPLETE = $(".cls_lov_submit_type").val() == "submit_complete" ? true : false;
                            item.IS_SEND_TO_PP = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? true : false;
                            item.CONTROL_NAME = saleorderRepeatList[0].SALES_ORDER_NO + saleorderRepeatList[0].SALES_ORDER_ITEM + saleorderRepeatList[0].COMPONENT_ITEM + "SUBMIT";
                            item.ARTWORK_SUB_ID = 0;

                            listRequestSORepeat.push(item)

                            var jsonObj2 = new Object();
                            jsonObj2.data = listRequestSORepeat;
                            var mydata = jsonObj2;
                            var myurl = '/api/dashboard/selectedrepeatso_tuning';
                            var mytype = 'POST';
                            myAjax(myurl, mytype, mydata, callbackSubmitDataUploadArtworkSO_TUTuning);

                        }
                    },
                    No: {
                        text: 'No',
                        btnClass: 'btn-default cls_btn_confirm_no',
                        action: function () {

                        }
                    }
                }
            });

        }
        else {
            // submit and sendPP and complete
            //var listRequest = [];
            var countGroup = 0;
            let listGroup = data.map(a => a.GROUPING).filter((v, i, a) => a.indexOf(v) === i);
            if (listGroup.length <= 15) {//max group = 15
                $('#modal_for_information .cls_display_information').html("<b>Submit type : " + $(".cls_lov_submit_type option:selected").text() + "</b>");
                var columnComment = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? '<div class="col-md-4">Comment</div>' : "";
                $('#modal_for_information .cls_display_information').append('<div class="row header"><div class="col-md-1">SO Order</div><div class="col-md-2">Product code</div><div class="col-md-2">Order bom</div><div class="col-md-3">Process status</div>' + columnComment + '</div>');
                //$('#modal_for_information .close').addClass("cls_hide");


                for (var j = 0; j < listGroup.length; j++) {
                    if (listGroup[j] != undefined) {
                        var listFilter = data.filter(obj => {
                            return obj.GROUPING === listGroup[j];
                        })
                        var strSO = '<b>Group ' + (j + 1) + '</b><br/>';
                        var strProductCode = '';
                        var strMaterial = '';
                        var sonumber = '';
                        var saleorderList = [];
                        var saleorderRepeatList = [];
                        for (i = 0; i < listFilter.length; i++) {
                            if (sonumber.indexOf(listFilter[i].SALES_ORDER_NO) == -1) {
                                sonumber += '|' + listFilter[i].SALES_ORDER_NO;

                                var saleorderItem = {};
                                saleorderItem["ARTWORK_REQUEST_ID"] = 0;
                                saleorderItem["SALES_ORDER_NO"] = listFilter[i].SALES_ORDER_NO;
                                saleorderItem["CREATE_BY"] = UserID;
                                saleorderItem["UPDATE_BY"] = UserID;
                                saleorderItem["RECHECK_ARTWORK"] = listFilter[i].RECHECK_ARTWORK;
                                saleorderList.push(saleorderItem);
                            }

                            var saleorderRepeat = {};
                            saleorderRepeat["ARTWORK_REQUEST_ID"] = 0;
                            saleorderRepeat["SALES_ORDER_NO"] = listFilter[i].SALES_ORDER_NO;
                            saleorderRepeat["SALES_ORDER_ITEM"] = listFilter[i].ITEM;
                            saleorderRepeat["PRODUCT_CODE"] = listFilter[i].PRODUCT_CODE;
                            saleorderRepeat["COMPONENT_ITEM"] = listFilter[i].COMPONENT_ITEM;
                            saleorderRepeat["COMPONENT_MATERIAL"] = listFilter[i].COMPONENT_MATERIAL;
                            saleorderRepeat["SOLD_TO_DISPLAY_TXT"] = listFilter[i].SOLD_TO_DISPLAY_TXT;
                            saleorderRepeat["SHIP_TO_DISPLAY_TXT"] = listFilter[i].SHIP_TO_DISPLAY_TXT;
                            saleorderRepeat["BRAND_DISPLAY_TXT"] = listFilter[i].BRAND_DISPLAY_TXT;
                            saleorderRepeat["CREATE_BY"] = UserID;
                            saleorderRepeat["UPDATE_BY"] = UserID;
                            saleorderRepeat["RECHECK_ARTWORK"] = listFilter[i].RECHECK_ARTWORK;
                            saleorderRepeat["PRODUCTION_PLANT_DISPLAY_TXT"] = data[i].PRODUCTION_PLANT;  //by aof 20230121_3V_SOREPAT INC-93118
                            saleorderRepeatList.push(saleorderRepeat);

                            strSO = strSO + listFilter[i].SALES_ORDER_NO + "(" + listFilter[i].ITEM + ")<br/>";
                            var product = strProductCode.indexOf(listFilter[i].PRODUCT_CODE) == -1 ? listFilter[i].PRODUCT_CODE : "";
                            strProductCode = strProductCode + product + "<br/>";

                            var tempCOMPONENT_MATERIAL = '';
                            if (!isEmpty(listFilter[i].COMPONENT_MATERIAL)) {
                                tempCOMPONENT_MATERIAL = listFilter[i].COMPONENT_MATERIAL
                            }

                            var material = strMaterial.indexOf(tempCOMPONENT_MATERIAL) == -1 ? tempCOMPONENT_MATERIAL : "";
                            if (strMaterial == '') {
                                strMaterial = strMaterial + material;
                            }
                            else {
                                strMaterial = strMaterial + '<br/>' + material;
                            }
                        }

                        var recipientsList = [];
                        var recipientsItem = {};
                        recipientsItem["RECIPIENT_USER_ID"] = UserID;
                        recipientsItem["CREATE_BY"] = UserID;
                        recipientsItem["UPDATE_BY"] = UserID;
                        recipientsList.push(recipientsItem);

                        //var jsonObj = new Object();
                        //jsonObj.data = [];
                        var item = {};

                        item["ARTWORK_REQUEST_ID"] = 0;
                        item["TYPE_OF_ARTWORK"] = 'REPEAT';
                        item["CREATE_BY"] = UserID;
                        item["UPDATE_BY"] = UserID;
                        item["CREATOR_ID"] = UserID;

                        item.SALES_ORDER = saleorderList;
                        item.REQUEST_RECIPIENT = recipientsList;
                        item.SALES_ORDER_REPEAT = saleorderRepeatList;
                        item.IS_COMPLETE = $(".cls_lov_submit_type").val() == "submit_complete" ? true : false;
                        item.IS_SEND_TO_PP = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? true : false;
                        item.CONTROL_NAME = saleorderRepeatList[0].SALES_ORDER_NO + saleorderRepeatList[0].SALES_ORDER_ITEM + saleorderRepeatList[0].COMPONENT_ITEM;
                        item.ARTWORK_SUB_ID = 0;

                        //jsonObj.data = item;
                        //jsonObj.data.SALES_ORDER = saleorderList;
                        //jsonObj.data.REQUEST_RECIPIENT = recipientsList;
                        //jsonObj.data.SALES_ORDER_REPEAT = saleorderRepeatList;
                        //jsonObj.data.IS_COMPLETE = $(".cls_lov_submit_type").val() == "submit_complete" ? true : false;
                        //jsonObj.data.IS_SEND_TO_PP = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? true : false;
                        //jsonObj.data.CONTROL_NAME = saleorderRepeatList[0].SALES_ORDER_NO + saleorderRepeatList[0].SALES_ORDER_ITEM + saleorderRepeatList[0].COMPONENT_ITEM;
                        //jsonObj.data.ARTWORK_SUB_ID = 0;

                        $('#modal_for_information').modal({
                            backdrop: 'static',
                            keyboard: false,

                        });

                        var cls_name = item.CONTROL_NAME;
                        var comment = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? '<div class="col-md-4"><div class="cls_comment' + cls_name + '" style="min-height: 80px;"></div></div>' : "";
                        // $('#modal_for_information .cls_display_information').append('<div class="row detail"><div class="col-md-1">' + strSO + '</div><div class="col-md-2">' + strProductCode + '</div><div class="col-md-2">' + strMaterial + '</div><div class="col-md-3"><span class="cls_' + cls_name + '" style="color:orange;">Waiting processing...</span></div>' + comment + '</div>'); 
                        $('#modal_for_information .cls_display_information').append('<div class="row detail"><div class="col-md-1">' + strSO + '</div><div class="col-md-2">' + strProductCode + '</div><div class="col-md-2">' + strMaterial + '</div><div class="col-md-3"> <div class="cls_handle_loading"></div><div><span class="cls_' + cls_name + '" style="color:orange;"></span></div></div>' + comment + '</div>');

                        //
                        if ($(".cls_lov_submit_type").val() == "submit_send_to_pp") {
                            bind_text_editor('#modal_for_information .cls_comment' + cls_name);
                        }

                        listRequestSORepeat.push(item)


                    }
                }
                $('#modal_for_information .cls_display_information').append("</table>");
                $('#modal_for_information .cls_btn_submit_send_pp').removeClass("cls_hide");

                //$('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("cls_loading");
                //$('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("col-md-3");

            }
            else {
                alertError2('Max group is 15 group.');
            }


        }

    }
    else
    {
        alertError2('Please select at least 1 item.');
    }

}

function callbackSubmitDataUploadArtworkSO_TUTuning(res) {
    if (res.data.length > 0) {
        window.open(suburl + "/" + 'Artwork/' + res.data[0].ARTWORK_REQUEST_ID + '?so=sorepeat');
    }
}

function submitSendToPP_TUTuning()
{

    $.confirm({
        title: 'Confirm Dialog',
        content: 'Do you want to submit?',
        animation: 'none',
        closeAnimation: 'none',
        type: 'blue',
        backgroundDismiss: false,
        backgroundDismissAnimation: 'glow',
        buttons: {
            Yes: {
                text: 'Yes',
                btnClass: 'btn-primary cls_btn_confirm_ok',
                action: function () {
                    if (listRequestSORepeat.length > 0) {
                      
                        var countGroup = 0;
                        for (var i = 0; i < listRequestSORepeat.length; i++) {

                            //find comment by cls name
                            var cls = listRequestSORepeat[i].CONTROL_NAME;
                            var cls_name_status = '.cls_' + cls;

                            if ($(".cls_lov_submit_type").val() == "submit_send_to_pp") {
                                var editor = new Quill('#modal_for_information .cls_comment' + cls);
                                listRequestSORepeat[i].COMMENT = editor.root.innerHTML;
                            }

                            $('#modal_for_information .cls_display_information').find(cls_name_status).html('<span style="color:orange;">&nbsp;&nbsp;Waiting processing...</span>');
                        }

                        $("#modal_for_information .cls_btn_submit_send_pp").addClass("cls_hide");
                        $('#modal_for_information .close').addClass("cls_hide");
                        $('#modal_for_information .cls_display_information .cls_handle_loading').addClass("col-md-1");
                        $('#modal_for_information .cls_display_information .cls_handle_loading').addClass("cls_loading");
                     

                        //-----------------------------------------------call ajax-----------------------------------------
                        var jsonObj2 = new Object();
                        jsonObj2.data = listRequestSORepeat;
                        var mydata = jsonObj2;
                        var myurl = '/api/dashboard/selectedrepeatso_tuning';
                        var mytype = 'POST';

                        $.ajax({
                            url: suburl + myurl,
                            type: mytype,
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify(mydata),
                            beforeSend: function (xhr) {
                                // $("<div class='loader' id='searching-loader'></div>").appendTo("#modal_for_information");
                                //$("#modal_for_information").animate({ scrollTop: $("#modal_for_information").height() }, 100);
                            },
                            success: function (res) {
                                res = DData(res);
                                if (res.status == "S") {

                                    callback_submitSendToPP_TUTuning(res);

                                }
                                else if (res.status == "E") {
                                    alertError2(res.msg);

                                    callback_submitSendToPP_TUTuning(res);              
                                    //$('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color: red;">Error : <br/>' + res.msg + '</span>');
                                }



                                //$('#searching-loader').remove();

                                $('#modal_for_information .close').removeClass("cls_hide");
                                $('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("cls_loading");
                                $('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("col-md-1");
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                $('#modal_for_information .close').removeClass("cls_hide");
                                $('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("cls_loading");
                                $('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("col-md-1");

                                alert("Status: " + textStatus); alert("Error: " + errorThrown);
                            }
                        });
                                //-----------------------------------------------call ajax-----------------------------------------

                    }
                }
            },
            No: {
                text: 'No',
                btnClass: 'btn-default cls_btn_confirm_no',
                action: function () {

                }
            }
        }
    });

}

function callback_submitSendToPP_TUTuning(res)
{
    if (res.data.length > 0) {
        for (i = 0; i < res.data.length; i++) {
            {
                var cls = res.data[i].CONTROL_NAME;
                var cls_name_return = '.cls_' + cls;

                if (res.data[i].RESULT_CREATE_WF_STATUS == "S") {
                    $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color:green;">Completed : <br/>' + res.data[i].RESULT_CREATE_WF_WFNO + '</span>');  
                }
                else if (res.data[i].RESULT_CREATE_WF_STATUS == "E") {
                    $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color: red;">Error : <br/>' + res.data[i].RESULT_CREATE_WF_MESSAGE + '</span>');
                } else {
                    $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color: blue;">Information : <br/>Have not been processed.</span>');
                }

            }
        }
    }
}

function selectedSORepeat_tuning_testing(data) {
    $('#modal_for_information .cls_display_information').html("<b>Submit type : " + $(".cls_lov_submit_type option:selected").text() + "</b>");
    if (data.length > 0) {
        $.confirm({
            title: 'Confirm Dialog',
            content: 'Do you want to submit?',
            animation: 'none',
            closeAnimation: 'none',
            type: 'blue',
            backgroundDismiss: false,
            backgroundDismissAnimation: 'glow',
            buttons: {
                Yes: {
                    text: 'Yes',
                    btnClass: 'btn-primary cls_btn_confirm_ok',
                    action: function () {

                        //case submit only
                        if ($(".cls_lov_submit_type").val() == "submit") {
                            var sonumber = '';
                            var saleorderList = [];
                            var saleorderRepeatList = [];
                            for (i = 0; i < data.length; i++) {
                                if (sonumber.indexOf(data[i].SALES_ORDER_NO) == -1) {
                                    sonumber += '|' + data[i].SALES_ORDER_NO;

                                    var saleorderItem = {};
                                    saleorderItem["ARTWORK_REQUEST_ID"] = 0;
                                    //saleorderItem["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                    saleorderItem["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                    saleorderItem["CREATE_BY"] = UserID;
                                    saleorderItem["UPDATE_BY"] = UserID;
                                    saleorderItem["RECHECK_ARTWORK"] = data[i].RECHECK_ARTWORK;
                                    saleorderList.push(saleorderItem);

                                }

                                var saleorderRepeat = {};
                                saleorderRepeat["ARTWORK_REQUEST_ID"] = 0;
                                saleorderRepeat["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                saleorderRepeat["SALES_ORDER_ITEM"] = data[i].ITEM;
                                saleorderRepeat["PRODUCT_CODE"] = data[i].PRODUCT_CODE;
                                saleorderRepeat["COMPONENT_ITEM"] = data[i].COMPONENT_ITEM;
                                saleorderRepeat["COMPONENT_MATERIAL"] = data[i].COMPONENT_MATERIAL;
                                saleorderRepeat["BRAND_DISPLAY_TXT"] = data[i].BRAND_DISPLAY_TXT;
                                saleorderRepeat["CREATE_BY"] = UserID;
                                saleorderRepeat["UPDATE_BY"] = UserID;
                                saleorderRepeat["RECHECK_ARTWORK"] = data[i].RECHECK_ARTWORK;
                                saleorderRepeatList.push(saleorderRepeat);
                            }

                            var recipientsList = [];
                            var recipientsItem = {};
                            recipientsItem["RECIPIENT_USER_ID"] = UserID;
                            recipientsItem["CREATE_BY"] = UserID;
                            recipientsItem["UPDATE_BY"] = UserID;
                            recipientsList.push(recipientsItem);

                            var jsonObj = new Object();
                            jsonObj.data = [];
                            var item = {};

                            item["ARTWORK_REQUEST_ID"] = 0;
                            item["TYPE_OF_ARTWORK"] = 'REPEAT';
                            item["CREATE_BY"] = UserID;
                            item["UPDATE_BY"] = UserID;


                            jsonObj.data = item;
                            jsonObj.data.SALES_ORDER = saleorderList;
                            jsonObj.data.REQUEST_RECIPIENT = recipientsList;
                            jsonObj.data.SALES_ORDER_REPEAT = saleorderRepeatList;




                            var myurl = '/api/dashboard/selectedrepeatso';
                            var mytype = 'POST';
                            var mydata = jsonObj;
                            //new_tab = window.open("");
                            myAjax(myurl, mytype, mydata, callbackSubmitDataUploadArtworkSO);

                        }
                        //case submit and complete or submit and sent pp
                        else {
                            listComment = [];
                            var listRequest = [];
                            var countGroup = 0;
                            let listGroup = data.map(a => a.GROUPING).filter((v, i, a) => a.indexOf(v) === i);
                            if (listGroup.length <= 15) {//max group = 15
                                var columnComment = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? '<div class="col-md-4">Comment</div>' : "";
                                $('#modal_for_information .cls_display_information').append('<div class="row header"><div class="col-md-1">SO Order</div><div class="col-md-2">Product code</div><div class="col-md-2">Order bom</div><div class="col-md-3">Process status</div>' + columnComment + '</div>');
                                $('#modal_for_information .close').addClass("cls_hide");

                                for (var j = 0; j < listGroup.length; j++) {
                                    if (listGroup[j] != undefined) {
                                        var listFilter = data.filter(obj => {
                                            return obj.GROUPING === listGroup[j];
                                        })
                                        var strSO = '';
                                        var strProductCode = '';
                                        var strMaterial = '';
                                        var sonumber = '';
                                        var saleorderList = [];
                                        var saleorderRepeatList = [];
                                        for (i = 0; i < listFilter.length; i++) {
                                            if (sonumber.indexOf(listFilter[i].SALES_ORDER_NO) == -1) {
                                                sonumber += '|' + listFilter[i].SALES_ORDER_NO;

                                                var saleorderItem = {};
                                                saleorderItem["ARTWORK_REQUEST_ID"] = 0;
                                                saleorderItem["SALES_ORDER_NO"] = listFilter[i].SALES_ORDER_NO;
                                                saleorderItem["CREATE_BY"] = UserID;
                                                saleorderItem["UPDATE_BY"] = UserID;
                                                saleorderItem["RECHECK_ARTWORK"] = listFilter[i].RECHECK_ARTWORK;
                                                saleorderList.push(saleorderItem);
                                            }

                                            var saleorderRepeat = {};
                                            saleorderRepeat["ARTWORK_REQUEST_ID"] = 0;
                                            saleorderRepeat["SALES_ORDER_NO"] = listFilter[i].SALES_ORDER_NO;
                                            saleorderRepeat["SALES_ORDER_ITEM"] = listFilter[i].ITEM;
                                            saleorderRepeat["PRODUCT_CODE"] = listFilter[i].PRODUCT_CODE;
                                            saleorderRepeat["COMPONENT_ITEM"] = listFilter[i].COMPONENT_ITEM;
                                            saleorderRepeat["COMPONENT_MATERIAL"] = listFilter[i].COMPONENT_MATERIAL;
                                            saleorderRepeat["SOLD_TO_DISPLAY_TXT"] = listFilter[i].SOLD_TO_DISPLAY_TXT;
                                            saleorderRepeat["SHIP_TO_DISPLAY_TXT"] = listFilter[i].SHIP_TO_DISPLAY_TXT;
                                            saleorderRepeat["BRAND_DISPLAY_TXT"] = listFilter[i].BRAND_DISPLAY_TXT;
                                            saleorderRepeat["CREATE_BY"] = UserID;
                                            saleorderRepeat["UPDATE_BY"] = UserID;
                                            saleorderRepeat["RECHECK_ARTWORK"] = listFilter[i].RECHECK_ARTWORK;
                                            saleorderRepeatList.push(saleorderRepeat);

                                            strSO = strSO + listFilter[i].SALES_ORDER_NO + "(" + listFilter[i].ITEM + ")" + "<br/>";
                                            var product = strProductCode.indexOf(listFilter[i].PRODUCT_CODE) == -1 ? listFilter[i].PRODUCT_CODE : "";
                                            strProductCode = strProductCode + product + "<br/>";

                                            var tempCOMPONENT_MATERIAL = '';
                                            if (!isEmpty(listFilter[i].COMPONENT_MATERIAL)) {
                                                tempCOMPONENT_MATERIAL = listFilter[i].COMPONENT_MATERIAL
                                            }

                                            var material = strMaterial.indexOf(tempCOMPONENT_MATERIAL) == -1 ? tempCOMPONENT_MATERIAL : "";
                                            if (strMaterial == '') {
                                                strMaterial = strMaterial + material;
                                            }
                                            else {
                                                strMaterial = strMaterial + '<br/>' + material;
                                            }
                                        }

                                        var recipientsList = [];
                                        var recipientsItem = {};
                                        recipientsItem["RECIPIENT_USER_ID"] = UserID;
                                        recipientsItem["CREATE_BY"] = UserID;
                                        recipientsItem["UPDATE_BY"] = UserID;
                                        recipientsList.push(recipientsItem);

                                        //var jsonObj = new Object();
                                        //jsonObj.data = [];
                                        var item = {};

                                        item["ARTWORK_REQUEST_ID"] = 0;
                                        item["TYPE_OF_ARTWORK"] = 'REPEAT';
                                        item["CREATE_BY"] = UserID;
                                        item["UPDATE_BY"] = UserID;
                                        item["CREATOR_ID"] = UserID;

                                        item.SALES_ORDER = saleorderList;
                                        item.REQUEST_RECIPIENT = recipientsList;
                                        item.SALES_ORDER_REPEAT = saleorderRepeatList;
                                        item.IS_COMPLETE = $(".cls_lov_submit_type").val() == "submit_complete" ? true : false;
                                        item.IS_SEND_TO_PP = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? true : false;
                                        item.CONTROL_NAME = saleorderRepeatList[0].SALES_ORDER_NO + saleorderRepeatList[0].SALES_ORDER_ITEM + saleorderRepeatList[0].COMPONENT_ITEM;
                                        item.ARTWORK_SUB_ID = 0;

                                        //jsonObj.data = item;
                                        //jsonObj.data.SALES_ORDER = saleorderList;
                                        //jsonObj.data.REQUEST_RECIPIENT = recipientsList;
                                        //jsonObj.data.SALES_ORDER_REPEAT = saleorderRepeatList;
                                        //jsonObj.data.IS_COMPLETE = $(".cls_lov_submit_type").val() == "submit_complete" ? true : false;
                                        //jsonObj.data.IS_SEND_TO_PP = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? true : false;
                                        //jsonObj.data.CONTROL_NAME = saleorderRepeatList[0].SALES_ORDER_NO + saleorderRepeatList[0].SALES_ORDER_ITEM + saleorderRepeatList[0].COMPONENT_ITEM;
                                        //jsonObj.data.ARTWORK_SUB_ID = 0;

                                        $('#modal_for_information').modal({
                                            backdrop: 'static',
                                            keyboard: false,
                                           
                                        });

                                        var cls_name = item.CONTROL_NAME;
                                        var comment = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? '<div class="col-md-4"><div class="cls_comment' + cls_name + '" style="min-height: 80px;"></div></div>' : "";
                                      // $('#modal_for_information .cls_display_information').append('<div class="row detail"><div class="col-md-1">' + strSO + '</div><div class="col-md-2">' + strProductCode + '</div><div class="col-md-2">' + strMaterial + '</div><div class="col-md-3"><span class="cls_' + cls_name + '" style="color:orange;">Waiting processing...</span></div>' + comment + '</div>'); 
                                        $('#modal_for_information .cls_display_information').append('<div class="row detail"><div class="col-md-1">(' + (j+1) + ').' + strSO + '</div><div class="col-md-2">' + strProductCode + '</div><div class="col-md-2">' + strMaterial + '</div><div class="col-md-3 cls_handle_loading cls_loading"></div><div class="col-md-3"><span class="cls_' + cls_name + '" style="color:orange;">Waiting processing...</span></div>' + comment + '</div>');

                                        listRequest.push(item)                            
                                    }
                                }
                                $('#modal_for_information .cls_display_information').append("</table>");

                                //-----------------------------------------------call ajax-----------------------------------------
                                var jsonObj2 = new Object();
                                jsonObj2.data = listRequest;
                                var mydata = jsonObj2;
                                var myurl = '/api/dashboard/selectedrepeatso_tuning';
                                var mytype = 'POST';
                                $.ajax({
                                    url: suburl + myurl,
                                    type: mytype,
                                    contentType: 'application/json; charset=utf-8',
                                    data: JSON.stringify(mydata),
                                    beforeSend: function (xhr) {
                                       // $("<div class='loader' id='searching-loader'></div>").appendTo("#modal_for_information");
                                       //$("#modal_for_information").animate({ scrollTop: $("#modal_for_information").height() }, 100);
                                    },
                                    success: function (res) {
                                        res = DData(res);
                                        if (res.status == "S") {

                                            if (res.data.length > 0) {
                                                for (i = 0; i < res.data.length; i++) {
                                                    {
                                                        var cls = res.data[i].CONTROL_NAME;
                                                        var cls_name_return = '.cls_' + cls;

                                                        if (res.data[i].RESULT_CREATE_WF_STATUS == "S") {
                                                            $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color:green;">Completed : <br/>' + res.data[i].RESULT_CREATE_WF_WFNO + '</span>');
                                                        }
                                                        else if (res.data[i].RESULT_CREATE_WF_STATUS == "E") {
                                                            $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color: red;">Error : <br/>' + res.data[i].RESULT_CREATE_WF_MESSAGE + '</span>');
                                                        }


                                                    }
                                                }
                                            }

                                        }
                                        else if (res.status == "E")
                                        {
                                            alertError2(res.msg);
                                            //$('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color: red;">Error : <br/>' + res.msg + '</span>');
                                        }



                                        //$('#searching-loader').remove();

                                        $('#modal_for_information .close').removeClass("cls_hide");
                                        $('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("cls_loading");
                                        $('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("col-md-3");
                                    },
                                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                                        $('#modal_for_information .close').removeClass("cls_hide");
                                        $('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("cls_loading");
                                        $('#modal_for_information .cls_display_information .cls_handle_loading').removeClass("col-md-3");

                                        alert("Status: " + textStatus); alert("Error: " + errorThrown);
                                    } 
                                });
                                //-----------------------------------------------call ajax-----------------------------------------

                            }
                            else {
                                alertError2('Max group is 15 group.');
                            }
                        }
                    }
                },
                No: {
                    text: 'No',
                    btnClass: 'btn-default cls_btn_confirm_no',
                    action: function () {

                    }
                }
            }
        });
    }
}
//---------------------------- tuning performance 2022 by aof---------------------------------//



var first_click_tab_so_repeat = true;
var new_tab;
function selectedSORepeat(data) {
    $('#modal_for_information .cls_display_information').html("<b>Submit type : " + $(".cls_lov_submit_type option:selected").text() + "</b>");
    if (data.length > 0) {
        $.confirm({
            title: 'Confirm Dialog',
            content: 'Do you want to submit?',
            animation: 'none',
            closeAnimation: 'none',
            type: 'blue',
            backgroundDismiss: false,
            backgroundDismissAnimation: 'glow',
            buttons: {
                Yes: {
                    text: 'Yes',
                    btnClass: 'btn-primary cls_btn_confirm_ok',
                    action: function () {

                        //case submit only
                        if ($(".cls_lov_submit_type").val() == "submit") {
                            var sonumber = '';
                            var saleorderList = [];
                            var saleorderRepeatList = [];
                            for (i = 0; i < data.length; i++) {
                                if (sonumber.indexOf(data[i].SALES_ORDER_NO) == -1) {
                                    sonumber += '|' + data[i].SALES_ORDER_NO;

                                    var saleorderItem = {};
                                    saleorderItem["ARTWORK_REQUEST_ID"] = 0;
                                    //saleorderItem["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                    saleorderItem["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                    saleorderItem["CREATE_BY"] = UserID;
                                    saleorderItem["UPDATE_BY"] = UserID;
                                    saleorderItem["RECHECK_ARTWORK"] = data[i].RECHECK_ARTWORK;
                                    saleorderList.push(saleorderItem);

                                }

                                var saleorderRepeat = {};
                                saleorderRepeat["ARTWORK_REQUEST_ID"] = 0;
                                saleorderRepeat["SALES_ORDER_NO"] = data[i].SALES_ORDER_NO;
                                saleorderRepeat["SALES_ORDER_ITEM"] = data[i].ITEM;
                                saleorderRepeat["PRODUCT_CODE"] = data[i].PRODUCT_CODE;
                                saleorderRepeat["COMPONENT_ITEM"] = data[i].COMPONENT_ITEM;
                                saleorderRepeat["COMPONENT_MATERIAL"] = data[i].COMPONENT_MATERIAL;
                                saleorderRepeat["BRAND_DISPLAY_TXT"] = data[i].BRAND_DISPLAY_TXT;
                                saleorderRepeat["CREATE_BY"] = UserID;
                                saleorderRepeat["UPDATE_BY"] = UserID;
                                saleorderRepeat["RECHECK_ARTWORK"] = data[i].RECHECK_ARTWORK;
                             

                                saleorderRepeatList.push(saleorderRepeat);
                            }

                            var recipientsList = [];
                            var recipientsItem = {};
                            recipientsItem["RECIPIENT_USER_ID"] = UserID;
                            recipientsItem["CREATE_BY"] = UserID;
                            recipientsItem["UPDATE_BY"] = UserID;
                            recipientsList.push(recipientsItem);

                            var jsonObj = new Object();
                            jsonObj.data = [];
                            var item = {};

                            item["ARTWORK_REQUEST_ID"] = 0;
                            item["TYPE_OF_ARTWORK"] = 'REPEAT';
                            item["CREATE_BY"] = UserID;
                            item["UPDATE_BY"] = UserID;
                            

                            jsonObj.data = item;
                            jsonObj.data.SALES_ORDER = saleorderList;
                            jsonObj.data.REQUEST_RECIPIENT = recipientsList;
                            jsonObj.data.SALES_ORDER_REPEAT = saleorderRepeatList;


                          
                        
                            var myurl = '/api/dashboard/selectedrepeatso';
                            var mytype = 'POST';
                            var mydata = jsonObj;
                            //new_tab = window.open("");
                            myAjax(myurl, mytype, mydata, callbackSubmitDataUploadArtworkSO);
                           
                        }
                        //case submit and complete or submit and sent pp
                        else {
                            listComment = [];
                            var listRequest = [];
                            var countGroup = 0;
                            let listGroup = data.map(a => a.GROUPING).filter((v, i, a) => a.indexOf(v) === i);
                            if (listGroup.length <= 15) {//max group = 15
                                var columnComment = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? '<div class="col-md-4">Comment</div>' : "";
                                $('#modal_for_information .cls_display_information').append('<div class="row header"><div class="col-md-1">SO Order</div><div class="col-md-2">Product code</div><div class="col-md-2">Order bom</div><div class="col-md-3">Process status</div>' + columnComment + '</div>');

                                for (var j = 0; j < listGroup.length; j++) {
                                    if (listGroup[j] != undefined) {
                                        var listFilter = data.filter(obj => {
                                            return obj.GROUPING === listGroup[j];
                                        })
                                        var strSO = '';
                                        var strProductCode = '';
                                        var strMaterial = '';
                                        var sonumber = '';
                                        var saleorderList = [];
                                        var saleorderRepeatList = [];
                                        for (i = 0; i < listFilter.length; i++) {
                                            if (sonumber.indexOf(listFilter[i].SALES_ORDER_NO) == -1) {
                                                sonumber += '|' + listFilter[i].SALES_ORDER_NO;

                                                var saleorderItem = {};
                                                saleorderItem["ARTWORK_REQUEST_ID"] = 0;
                                                saleorderItem["SALES_ORDER_NO"] = listFilter[i].SALES_ORDER_NO;
                                                saleorderItem["CREATE_BY"] = UserID;
                                                saleorderItem["UPDATE_BY"] = UserID;
                                                saleorderItem["RECHECK_ARTWORK"] = listFilter[i].RECHECK_ARTWORK;
                                                saleorderList.push(saleorderItem);
                                            }

                                            var saleorderRepeat = {};
                                            saleorderRepeat["ARTWORK_REQUEST_ID"] = 0;
                                            saleorderRepeat["SALES_ORDER_NO"] = listFilter[i].SALES_ORDER_NO;
                                            saleorderRepeat["SALES_ORDER_ITEM"] = listFilter[i].ITEM;
                                            saleorderRepeat["PRODUCT_CODE"] = listFilter[i].PRODUCT_CODE;
                                            saleorderRepeat["COMPONENT_ITEM"] = listFilter[i].COMPONENT_ITEM;
                                            saleorderRepeat["COMPONENT_MATERIAL"] = listFilter[i].COMPONENT_MATERIAL;
                                            saleorderRepeat["BRAND_DISPLAY_TXT"] = listFilter[i].BRAND_DISPLAY_TXT;
                                            saleorderRepeat["CREATE_BY"] = UserID;
                                            saleorderRepeat["UPDATE_BY"] = UserID;
                                            saleorderRepeat["RECHECK_ARTWORK"] = listFilter[i].RECHECK_ARTWORK;
                                            saleorderRepeatList.push(saleorderRepeat);

                                            strSO = strSO + listFilter[i].SALES_ORDER_NO + "(" + listFilter[i].ITEM + ")" + "<br/>";
                                            var product = strProductCode.indexOf(listFilter[i].PRODUCT_CODE) == -1 ? listFilter[i].PRODUCT_CODE : "";
                                            strProductCode = strProductCode + product + "<br/>";

                                            var tempCOMPONENT_MATERIAL = '';
                                            if (!isEmpty(listFilter[i].COMPONENT_MATERIAL)) {
                                                tempCOMPONENT_MATERIAL = listFilter[i].COMPONENT_MATERIAL
                                            }

                                            var material = strMaterial.indexOf(tempCOMPONENT_MATERIAL) == -1 ? tempCOMPONENT_MATERIAL : "";
                                            if (strMaterial == '') {
                                                strMaterial = strMaterial + material;
                                            }
                                            else {
                                                strMaterial = strMaterial + '<br/>' + material;
                                            }
                                        }

                                        var recipientsList = [];
                                        var recipientsItem = {};
                                        recipientsItem["RECIPIENT_USER_ID"] = UserID;
                                        recipientsItem["CREATE_BY"] = UserID;
                                        recipientsItem["UPDATE_BY"] = UserID;
                                        recipientsList.push(recipientsItem);

                                        var jsonObj = new Object();
                                        jsonObj.data = [];
                                        var item = {};

                                        item["ARTWORK_REQUEST_ID"] = 0;
                                        item["TYPE_OF_ARTWORK"] = 'REPEAT';
                                        item["CREATE_BY"] = UserID;
                                        item["UPDATE_BY"] = UserID;
                                      

                                        jsonObj.data = item;
                                        jsonObj.data.SALES_ORDER = saleorderList;
                                        jsonObj.data.REQUEST_RECIPIENT = recipientsList;
                                        jsonObj.data.SALES_ORDER_REPEAT = saleorderRepeatList;
                                        jsonObj.data.IS_COMPLETE = $(".cls_lov_submit_type").val() == "submit_complete" ? true : false;
                                        jsonObj.data.IS_SEND_TO_PP = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? true : false;
                                        jsonObj.data.CONTROL_NAME = saleorderRepeatList[0].SALES_ORDER_NO + saleorderRepeatList[0].SALES_ORDER_ITEM + saleorderRepeatList[0].COMPONENT_ITEM;
                                        jsonObj.data.ARTWORK_SUB_ID = 0;

                                        $('#modal_for_information').modal({
                                            backdrop: 'static',
                                            keyboard: true,
                                        });

                                        var cls_name = jsonObj.data.CONTROL_NAME;
                                        var comment = $(".cls_lov_submit_type").val() == "submit_send_to_pp" ? '<div class="col-md-4"><div class="cls_comment' + cls_name + '" style="min-height: 80px;"></div></div>' : "";
                                        $('#modal_for_information .cls_display_information').append('<div class="row detail"><div class="col-md-1">' + strSO + '</div><div class="col-md-2">' + strProductCode + '</div><div class="col-md-2">' + strMaterial + '</div><div class="col-md-3"><span class="cls_' + cls_name + '" style="color:orange;">Waiting processing...</span></div>' + comment + '</div>');

                                        listRequest.push(jsonObj);

                                        var mydata = jsonObj;
                                        var myurl = '/api/dashboard/selectedrepeatso';
                                        var mytype = 'POST';
                                        $.ajax({
                                            url: suburl + myurl,
                                            type: mytype,
                                            contentType: 'application/json; charset=utf-8',
                                            data: JSON.stringify(mydata),
                                            success: function (res) {
                                                res = DData(res);
                                                var cls = res.param.data.SALES_ORDER_REPEAT[0].SALES_ORDER_NO + res.param.data.SALES_ORDER_REPEAT[0].SALES_ORDER_ITEM + res.param.data.SALES_ORDER_REPEAT[0].COMPONENT_ITEM;
                                                var cls_name_return = '.cls_' + cls;

                                                if (res.status == "S") {
                                                    if ($(".cls_lov_submit_type").val() == "submit") {
                                                        var taga = '';
                                                        if (res.data.length > 0) {
                                                            taga = '<a target="_blank" href="' + suburl + "/" + 'Artwork/' + res.data[0].ARTWORK_REQUEST_ID + '?so=sorepeat' + '">Click here</a>'
                                                            window.open(suburl + "/" + 'Artwork/' + res.data[0].ARTWORK_REQUEST_ID + '?so=sorepeat');
                                                        }

                                                        $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color:green;">Completed : ' + taga + '</span>');
                                                    }
                                                    else {
                                                        if ($(".cls_lov_submit_type").val() == "submit_send_to_pp") {
                                                            bind_text_editor('#modal_for_information .cls_comment' + cls);

                                                            //keep data success
                                                            var resultSeccess = listRequest.filter(obj => {
                                                                return obj.data.CONTROL_NAME === cls;
                                                            })

                                                            resultSeccess[0].data.ARTWORK_SUB_ID = res.ARTWORK_SUB_ID;
                                                            listComment.push(resultSeccess[0]);
                                                        }
                                                        $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color:green;">Completed : <br/>' + res.WF_NO + '</span>');
                                                    }
                                                }
                                                else if (res.status == "E") {
                                                    $('#modal_for_information .cls_display_information').find(cls_name_return).html('<span style="color: red;">Error : <br/>' + res.msg + '</span>');
                                                }
                                                countGroup++;

                                                if (countGroup == listGroup.length) {
                                                    if (listComment.length > 0) {
                                                        //show button submit send to pp
                                                        $("#modal_for_information .cls_btn_submit_send_pp").removeClass("cls_hide");
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                                $('#modal_for_information .cls_display_information').append("</table>");
                            }
                            else {
                                alertError2('Max group is 15 group.');
                            }
                        }
                    }
                },
                No: {
                    text: 'No',
                    btnClass: 'btn-default cls_btn_confirm_no',
                    action: function () {

                    }
                }
            }
        });
    }
    else {
        alertError2('Please select at least 1 item.');
    }
}

function callbackSubmitDataUploadArtworkSO(res) {
    if (res.data.length > 0) {
      
        window.open(suburl + "/" + 'Artwork/' + res.data[0].ARTWORK_REQUEST_ID + '?so=sorepeat');

    
       // new_tab.location.href = suburl + "/" + 'Artwork/' + res.data[0].ARTWORK_REQUEST_ID + '?so=sorepeat';

        // document.location.href = suburl + "/" + 'Artwork/' + res.data[0].ARTWORK_REQUEST_ID + '?so=sorepeat';
       // window.location.href = suburl + "/" + 'Artwork/' + res.data[0].ARTWORK_REQUEST_ID + '?so=sorepeat';
    }
}



function clearSelected() {
    var table_so_repeat = $('#table_so_repeat').DataTable();
    $('#table_so_repeat .cls_chk_group').prop('checked', false);
    //if ($(".cls_lov_submit_type").val() == "submit") {
    table_so_repeat.rows({ selected: true }).deselect();
    $('#table_so_repeat .cls_chk_so_repeat').prop('checked', false);
    //}
    //else {
    //    data = [];
    //    var temp = table_so_repeat.rows();
    //    temp.rows().every(function (rowIdx, tableLoop, rowLoop) {
    //        var rowNode = this.node();
    //        var group = $(rowNode).find('.cls_lov_group');
    //        group.val("0");
    //    });
    //}
}
