var first_load = true;
var countheader = 0;
var unlock_remark_quill, obsolete_remark_quill
$(document).ready(function () {

    bind_lov_no_ajax('.cls_report_artworkmatcontrol .cls_lov_search_status', '', '');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_sold_to', '/api/lov/customersoldtoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_ship_to', '/api/lov/customershiptoso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_brand', '/api/lov/brandso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_zone', '/api/lov/zone', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_country', '/api/lov/countryso', 'data.DISPLAY_TXT');
    bind_lov('.cls_report_artworkmatcontrol .cls_lov_search_pic', '/api/lov/userpicso', 'data.DISPLAY_TXT');

    unlock_remark_quill = bind_text_editor('#modal_report_artworkmatcontrol_unlock .cls_unlock_remark');
    obsolete_remark_quill = bind_text_editor('#modal_report_artworkmatcontrol_obsolete .cls_obsolete_remark');

    $('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_from').attr('required', true);
    $('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_to').attr('required', true);

    checkIsRoleView();

    //alert(UserID)

    $('.cls_table_report_artworkmatcontrol thead tr').clone(true).appendTo('.cls_table_report_artworkmatcontrol thead');
    $('.cls_table_report_artworkmatcontrol thead tr:eq(1) th').each(function (i) {
        //if (i == 2 || i == 11 || i == 12) {
        if (i == 0 || i == 13 || i == 14 || i == 15 || i == 16 || i == 17 || i == 18) {
            $(this).html('');
        }
        else {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control cls_table_text_search" placeholder="Search ' + $.trim(title).replace(/\s\s+/g, ' ') + '" data-index="' + i + '" />');
        }

    });




    $('#table_report_artworkmatcontrol').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        //fixedColumns: {
        //    leftColumns: 4
        //},
        lengthChange: false,
        scrollX: true,
        search: false,
        ordering: false,
        searching: false,
    });

    //bind_table_report_artworkmatcontrol();
    //first_load = false;

    //$(table_report_artworkmatcontrol.table().container()).on('keyup', 'input', function () {
    //    table_report_artworkmatcontrol
    //        .column($(this).data('index'))
    //        .search(this.value)
    //        .draw();
    //});




    $(document).on('change', '.cls_report_artworkmatcontrol .cls_chk_group', function (e) {
        // Get group class name
        var groupName = $(this).data('group-name');
        var tempCheck = this.checked;
        //var temp = table_report_artworkmatcontrol.rows({ search: 'applied' });
        //temp.rows('tr.' + groupName).every(function (rowIdx, tableLoop, rowLoop) {
        //    var rowNode = this.node();
        //    table_report_artworkmatcontrol.row(rowIdx).select(tempCheck);
        //    $(rowNode).find('.cls_chk_mat5').prop('checked', tempCheck);
        //});

        if (tempCheck) {
            $(".cls_head_chk_group_" + groupName).addClass("selected");
        }
        else
            $(".cls_head_chk_group_" + groupName).removeClass("selected");
    });



    $('body').on('change', '#mat_select_all', function () {
        if (this.checked)
            $('.cls_report_artworkmatcontrol .cls_chk_group').prop('checked', true);
        else
            $('.cls_report_artworkmatcontrol .cls_chk_group').prop('checked', false);

        $('.cls_report_artworkmatcontrol .cls_chk_group').change();
    });

    $(".cls_report_artworkmatcontrol form").submit(function (e) {

        var isFreezeColumn = $('.cls_report_artworkmatcontrol .cls_chk_freeze_column').is(":checked") ? "X" : "";

        if (isFreezeColumn == "X") {
            bind_table_report_artowrkmatcontrol_serverside();
        } else {
            bind_table_report_artworkmatcontrolV2();
        }


        e.preventDefault();
    });

    $(".cls_report_artworkmatcontrol .cls_btn_clear").click(function () {
        $('.cls_report_artworkmatcontrol input[type=text]').val('');
        $('.cls_report_artworkmatcontrol input[type=checkbox]').prop('checked', false);
        $('.cls_report_artworkmatcontrol textarea').val('');
        $(".cls_report_artworkmatcontrol .cls_lov_search_sold_to").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_ship_to").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_brand").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_zone").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_country").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_pic").val('').trigger("change");
        $(".cls_report_artworkmatcontrol .cls_lov_search_status").val('').trigger("change");
    });

    //$(".dataTables_filter").css("display", "none");  // hiding global search box

    $(".cls_report_artworkmatcontrol .cls_btn_export_excel").click(function () {
        var isCheckAWFile = $('.cls_report_artworkmatcontrol .cls_chk_artwork_file').is(":checked") ? "X" : "";
        window.open("/excel/MaterialLockReportV2?data.SEARCH_SOLD_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_sold_to option:selected').text()
            + "&data.SEARCH_SHIP_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_ship_to option:selected').text()
            + "&data.SEARCH_MATERIAL_NO=" + $('.cls_report_artworkmatcontrol .cls_txtarea_mat5').val().replace(/\n/g, ',')
            + "&data.SEARCH_COUNTRY=" + $('.cls_report_artworkmatcontrol .cls_txtarea_country').val().replace(/\n/g, ',')
            + "&data.SEARCH_BRAND=" + $('.cls_report_artworkmatcontrol .cls_lov_search_brand option:selected').text()
            + "&data.SEARCH_PAOWNER=" + $('.cls_report_artworkmatcontrol .cls_lov_search_pic option:selected').text()
            + "&data.SEARCH_ZONE=" + $('.cls_report_artworkmatcontrol .cls_lov_search_zone option:selected').text()
            + "&data.SEARCH_STATUS=" + $('.cls_report_artworkmatcontrol .cls_lov_search_status option:selected').val()
            + "&data.SEARCH_PRODUCT_CODE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_search_product_code').val().replace(/\n/g, ',')
            + "&data.SEARCH_PKG_TYPE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_pkg').val().replace(/\n/g, ',')
            + "&data.SEARCH_REMARK_UNLOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_unlock').val().replace(/\n/g, ',')
            + "&data.SEARCH_REMARK_LOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_lock').val().replace(/\n/g, ',')
            + "&data.SEARCH_CHECK_ARTWORK_FILE=" + isCheckAWFile
            , '_blank');
        // $(".cls_report_artworkmatcontrol .buttons-excel").click();
    });

    $(".cls_report_artworkmatcontrol .cls_btn_obsolete").click(function () {
        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {

            if (checkMatIsConversion()) {
                alertError2("Please uncheck Conversion Material. It cannot be processed.");
            }
            else {
                $('#modal_report_artworkmatcontrol_obsolete').modal({
                    backdrop: 'static',
                    keyboard: true
                });
            }


        }
        else {
            alertError2("Please select material at least 1 item.");
        }
    });

    $(".cls_report_artworkmatcontrol .cls_btn_cancel_unlock").click(function () {
        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {

            if (checkMatIsConversion()) {
                alertError2("Please uncheck Conversion Material. It cannot be processed.");
            }
            else {
                cancelunlock();
            }


        }
        else {
            alertError2("Please select material at least 1 item.");
        }
    });

    $(".cls_report_artworkmatcontrol .cls_btn_inuse").click(function () {
        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {

            if (checkMatIsConversion()) {
                alertError2("Please uncheck Conversion Material. It cannot be processed.");
            }
            else {
                submitInuse();
            }


        }
        else {
            alertError2("Please select material at least 1 item.");
        }
    });

    $("#modal_report_artworkmatcontrol_unlock form").submit(function (e) {
        if ($(this).valid()) {
            if (unlock_remark_quill.root.innerHTML == "<p><br></p>") {
                alertError2("Please fill in the remark");
                return false;
            }
            else
                SubmitUnlock();
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $("#modal_report_artworkmatcontrol_obsolete form").submit(function (e) {
        if ($(this).valid()) {
            if (obsolete_remark_quill.root.innerHTML == "<p><br></p>") {
                alertError2("Please fill in the remark");
                return false;
            }
            else
                submitObsolete();
        }
        $(this).removeAttr("novalidate");
        e.preventDefault();	//STOP default action
    });

    $(".cls_report_artworkmatcontrol .cls_unlock_btn").click(function () {


        // checkMatIsConversion();

        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {


            if (checkMatIsConversion()) {
                alertError2("Please uncheck Conversion Material. It cannot be processed.");
            } else {

                $(".cls_report_artworkmatcontrol .cls_dt_search_unlock_date_from").val('').trigger("change");
                $(".cls_report_artworkmatcontrol .cls_dt_search_unlock_date_to").val('').trigger("change");

                $('#modal_report_artworkmatcontrol_unlock').modal({
                    backdrop: 'static',
                    keyboard: true
                });
            }

        }
        else {
            alertError2("Please select material at least 1 item.");
        }
    });

    $(".cls_report_artworkmatcontrol .cls_btn_download_file").click(function () {

        if ($('.cls_report_artworkmatcontrol .cls_chk_group:checked').length > 0) {
            var isFreeze = $('.cls_report_artworkmatcontrol .cls_chk_freeze_column').is(":checked") ? true : false;
            var mat;
            var dataMat = [];
            var mat_list = "";

            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {

                if (isFreeze) {
                    rowIndex = $(this).closest('tr').index();
                    mat = table_report_artworkmatcontrol.cell(rowIndex, 1).data();
                } else {
                    mat = $(this).closest('tr').find('td').eq(1).text();
                }

                if (mat_list.length > 0)
                {
                        mat_list = mat_list + "||" + mat;
                }
                else {
                        mat_list = mat;
                }

                //if (dataMat.indexOf(mat) == -1) {
                //    dataMat.push(mat);
                //    //if (data[i].IS_HAS_FILES_DISPLAY_TXT == 'Yes') {
                //    if (mat_list.length > 0) {
                //        mat_list = mat_list + "||" + mat;
                //    }
                //    else {
                //        mat_list = mat;
                //    }
                //}
            });   

            var url = suburl + '/FileUpload/DownloadLockMat?mat_list=' + mat_list;
            window.open(url, '_blank');

            //alertError2(mat_list);
        } else {
            alertError2('Please select at least 1 item.');
        }
    });

});


         //   var data = table_report_artworkmatcontrol.rows({ selected: true }).data();
            //if (data.length > 0) {
            //    var dataMat = [];
            //    var mat_list = "";
            //    for (i = 0; i < data.length; i++) {
            //        if (dataMat.indexOf(data[i].MATERIAL_NO) == -1) {
            //            dataMat.push(data[i].MATERIAL_NO);
            //            //if (data[i].IS_HAS_FILES_DISPLAY_TXT == 'Yes') {
            //            if (mat_list.length > 0) {
            //                mat_list = mat_list + "||" + data[i].MATERIAL_NO;
            //            }
            //            else {
            //                mat_list = data[i].MATERIAL_NO;
            //            }
            //            //}
            //        }
            //    }
            //    //if (mat_list.length == 0)
            //    //    return alertError2('Please select packaging material that has file');

            //    var url = suburl + '/FileUpload/DownloadLockMat?mat_list=' + mat_list;
            //    window.open(url, '_blank');


        //if (table_report_artworkmatcontrol == undefined) {
        //    alertError2('Please select at least 1 item.');
        //} else {
        //    var data = table_report_artworkmatcontrol.rows({ selected: true }).data();
        //    if (data.length > 0) {
        //        var dataMat = [];
        //        var mat_list = "";
        //        for (i = 0; i < data.length; i++) {
        //            if (dataMat.indexOf(data[i].MATERIAL_NO) == -1) {
        //                dataMat.push(data[i].MATERIAL_NO);
        //                //if (data[i].IS_HAS_FILES_DISPLAY_TXT == 'Yes') {
        //                if (mat_list.length > 0) {
        //                    mat_list = mat_list + "||" + data[i].MATERIAL_NO;
        //                }
        //                else {
        //                    mat_list = data[i].MATERIAL_NO;
        //                }
        //                //}
        //            }
        //        }
        //        //if (mat_list.length == 0)
        //        //    return alertError2('Please select packaging material that has file');



        //        var url = suburl + '/FileUpload/DownloadLockMat?mat_list=' + mat_list;
        //        window.open(url, '_blank');
        //    } else {
        //        alertError2('Please select at least 1 item.');
        //    }


function checkIsRoleView() {
  
    var myurl = '/api/report/materiallockreportviwer?data.USER_ID='+ UserID;
    var mytype = 'GET';
    var mydata = null;

    myAjax(myurl, mytype, mydata, callbackCheckIsRoleView);

}

function callbackCheckIsRoleView(res)
{
    if (res.data !=null)
    {
        if (res.data.length>0)
        {
            $('.cls_unlock_btn').addClass('cls_hide');
            $('.cls_btn_cancel_unlock').addClass('cls_hide');
            $('.cls_btn_inuse').addClass('cls_hide');
            $('.cls_btn_obsolete').addClass('cls_hide');
           // $('.cls_btn_download_file').addClass('cls_hide');
        }
    }
}



function getParamReportArtworkMatControlV2() {

    var isCheckAWFile = $('.cls_report_artworkmatcontrol .cls_chk_artwork_file').is(":checked") ? "X" : "";

    return "?data.SEARCH_SOLD_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_sold_to option:selected').text()
        + "&data.SEARCH_SHIP_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_ship_to option:selected').text()
        + "&data.SEARCH_MATERIAL_NO=" + $('.cls_report_artworkmatcontrol .cls_txtarea_mat5').val().replace(/\n/g, ',')
        + "&data.SEARCH_COUNTRY=" + $('.cls_report_artworkmatcontrol .cls_txtarea_country').val().replace(/\n/g, ',')
        + "&data.SEARCH_BRAND=" + $('.cls_report_artworkmatcontrol .cls_lov_search_brand option:selected').text()
        + "&data.SEARCH_PAOWNER=" + $('.cls_report_artworkmatcontrol .cls_lov_search_pic option:selected').text()
        + "&data.SEARCH_ZONE=" + $('.cls_report_artworkmatcontrol .cls_lov_search_zone option:selected').text()
        + "&data.SEARCH_STATUS=" + $('.cls_report_artworkmatcontrol .cls_lov_search_status option:selected').val()
        + "&data.SEARCH_PRODUCT_CODE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_search_product_code').val().replace(/\n/g, ',')
        + "&data.SEARCH_PKG_TYPE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_pkg').val().replace(/\n/g, ',')
        + "&data.SEARCH_REMARK_UNLOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_unlock').val().replace(/\n/g, ',')
        + "&data.SEARCH_REMARK_LOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_lock').val().replace(/\n/g, ',')
        + "&data.SEARCH_CHECK_ARTWORK_FILE=" + isCheckAWFile;

  
    //+ "&data.first_load=" + first_load;
}


function checkMatIsConversion()
{

    var f_found = false;
    var status;
    var isCheckAWFile = $('.cls_report_artworkmatcontrol .cls_chk_artwork_file').is(":checked") ? true : false;
    var isFreeze = $('.cls_report_artworkmatcontrol .cls_chk_freeze_column').is(":checked") ? true : false;

    var index;

    if (isCheckAWFile) {
        index = 4;
    } else
    {
        index = 3;
    }

    var rowIndex = 
    $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
 
       
        //alert(table_report_artworkmatcontrol.cells(rowIndex, 3).data())

        // status = $(this).closest('tr').find('td').eq(index).text();

            if (isFreeze)
            {
                rowIndex = $(this).closest('tr').index();
                status = table_report_artworkmatcontrol.cell(rowIndex, 4).data();
            } else
            {
                status = $(this).closest('tr').find('td').eq(index).text();
            }
        

        if (status == "Conversion")
        {
            //alert("Found Conversion")
            f_found = true;
            return true; 
        }

            
    });


    return f_found;
}


function checkEnterCriteria() {
    var f_enter = false;

    var ship_to = !isEmpty($('.cls_report_artworkmatcontrol .cls_lov_search_sold_to option:selected').text());
    var sld_to = !isEmpty($('.cls_report_artworkmatcontrol .cls_lov_search_ship_to option:selected').text());

    var mat_no = !isEmpty($('.cls_report_artworkmatcontrol .cls_txtarea_mat5').val().replace(/\n/g, ','));
    var product = !isEmpty($('.cls_report_artworkmatcontrol .cls_txtarea_search_product_code').val().replace(/\n/g, ','));

    var country = !isEmpty($('.cls_report_artworkmatcontrol .cls_txtarea_country').val().replace(/\n/g, ','));
    var package = !isEmpty($('.cls_report_artworkmatcontrol .cls_txtarea_pkg').val().replace(/\n/g, ','));

    var remark_unlock = !isEmpty($('.cls_report_artworkmatcontrol .cls_txtarea_remark_unlock').val().replace(/\n/g, ','));
    var remark_lock = !isEmpty($('.cls_report_artworkmatcontrol .cls_txtarea_remark_lock').val().replace(/\n/g, ','));

    var zone = !isEmpty($('.cls_report_artworkmatcontrol .cls_lov_search_zone option:selected').text());
    var brand = !isEmpty($('.cls_report_artworkmatcontrol .cls_lov_search_brand option:selected').text());
   
    var status = !isEmpty($('.cls_report_artworkmatcontrol .cls_lov_search_status option:selected').val());
    var paowner = !isEmpty($('.cls_report_artworkmatcontrol .cls_lov_search_pic option:selected').text());


    if (ship_to || sld_to || mat_no || product || country || package || remark_unlock || remark_lock || zone || brand || status || paowner)
    {
        f_enter = true;
    }
    else
    {
        f_enter = false;
        alertError2("Please enter at least one criteria.");
    }
   
    return f_enter
}


var table_report_artworkmatcontrol;





function bind_table_report_artworkmatcontrolV2() {
    var groupColumn = 1;
    var isCheckAWFile = $('.cls_report_artworkmatcontrol .cls_chk_artwork_file').is(":checked") ? true : false;
    table_report_artworkmatcontrol = $('#table_report_artworkmatcontrol').DataTable()

    $('.cls_table_text_search').val('');

    if (checkEnterCriteria())
    {
        table_report_artworkmatcontrol.destroy();

        table_report_artworkmatcontrol = $('#table_report_artworkmatcontrol').DataTable({
            serverSide: false,
            select: {
                'style': 'multi',
                selector: 'td:first-child input',
                info: false
            },
            orderCellsTop: true,
            fixedHeader: true,
            //fixedColumns: {
            //    leftColumns: 3,
            //    //heightMatch: 'none'
            //},
            //select: true,
            ajax: function (data, callback, settings) {
                $.ajax({
                    url: suburl + "/api/report/materiallockreportv2" + getParamReportArtworkMatControlV2(),
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
  
                {
                    "targets": 3,
                    "visible": isCheckAWFile
                },
   
              //  { "visible": false, "targets": groupColumn }


            ],
            "order": [[1, 'asc']],
            "processing": true,
            "lengthChange": true,
            "ordering": false,
            "info": true,
            "scrollX": true,
            "scrollCollapse": true,
            "scrollY": "500px",
             "scroller": true,
             "searching" : true,
             "stateSave": false,
            //"autoWidth": true,
            "columns": [
                {
                    render: function (data, type, row, meta) {

                        // return '<input class="cls_chk_group cls_td_width_10" data-mat_id="' + row.MATERIAL_LOCK_ID + '" data-group-name="group-' + row.GROUPING +'" type="checkbox">';
                        //if (row.STATUS != "Conversion")
                        //{
                            return '<input class="cls_chk_group cls_nowrap" data-group-name="group-' + row.GROUPING + '" data-mat_id="' + row.MATERIAL_LOCK_ID + '" type="checkbox">';
                        //}
                        
                    }
                },
                //{ "data": "GROUPING", "className": "cls_td_width_25 cls_td_mat5" },
                { "data": "MATERIAL_NO", "className": "cls_td_width_140" },
                { "data": "MATERIAL_DESCRIPTION", "className": "cls_nowrap" },
                { "data": "IS_HAS_FILES", "className": "cls_td_width_50 cls_status" },
                { "data": "STATUS", "className": "cls_td_width_60" },
                { "data": "REQUEST_FORM_NO", "className": "cls_td_width_130" },
                { "data": "ARTWORK_NO", "className": "cls_td_width_130 cls_artwork_no" },
                { "data": "MOCKUP_NO", "className": "cls_td_width_160" },
                //{ "data": "UNLOCK_DATE_FROM", "className": "cls_nowrap cls_lock_date_from" },
                {
                    className: "cls_lock_date_from cls_td_width_130",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.UNLOCK_DATE_FROM);
                    }
                },

                //{ "data": "UNLOCK_DATE_TO", "className": "cls_nowrap cls_lock_date_to" },
                {
                    className: "cls_lock_date_to cls_td_width_130",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.UNLOCK_DATE_TO);
                    }
                },
                { "data": "REMARK_UNLOCK", "className": "cls_td_width_240 cls_remark_unlock" },
                { "data": "REMARK_LOCK", "className": "cls_td_width_240 cls_remark_lock" },
                // { "data": "LOG_DATE", "className": "cls_nowrap cls_logdate" },
                {
                    className: "cls_logdate cls_td_width_130",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateTimeMoment(row.LOG_DATE);
                    }
                },

                { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
                { "data": "SOLD_TO", "className": "cls_nowrap" },
                { "data": "SHIP_TO", "className": "cls_nowrap" },
                { "data": "BRAND", "className": "cls_nowrap" },
                { "data": "COUNTRY", "className": "cls_nowrap" },
                { "data": "ZONE", "className": "cls_nowrap" },


                { "data": "PRIMARY_SIZE", "className": "cls_nowrap" },
                { "data": "PACK_SIZE", "className": "cls_nowrap" },
                { "data": "PACKAGING_STYLE", "className": "cls_nowrap" },

                { "data": "PACKAGING_TYPE", "className": "cls_nowrap" },
                { "data": "PRIMARY_TYPE", "className": "cls_nowrap" },
                { "data": "PA_OWNER", "className": "cls_nowrap" },
                { "data": "PG_OWNER", "className": "cls_nowrap" }
            ],


            "rowCallback": function (row, data, index) {
                $(row).addClass('group-' + data.GROUPING);
                $(row).addClass('head-' + data.GROUPING);
                $(row).addClass('group');
                $(row).addClass('highlight');
                $(row).addClass("cls_head_chk_group_" + "group-" + data.GROUPING);


                $(".cls_report_artworkmatcontrol #mat_select_all").prop('checked', false);



                

                if (isCheckAWFile) {
                    if (!isEmpty(data.IS_HAS_FILES))
                    {
                        if (data.IS_HAS_FILES == "Yes")
                            $('td:eq(3)', row).html('<label style="color:green;margin-bottom:0px;font-weight:normal;">' + data.IS_HAS_FILES + '</label>');
                        else
                            $('td:eq(3)', row).html('<label style="color:red;margin-bottom:0px;font-weight:normal;">' + data.IS_HAS_FILES + '</label>');


                        if (!isEmpty(data.STATUS)) {
                            if (data.STATUS == "In use")
                                $('td:eq(4)', row).html('<label style="color:green;margin-bottom:0px;font-weight:normal;">' + data.STATUS + '</label>');
                            else
                                $('td:eq(4)', row).html('<label style="color:red;margin-bottom:0px;font-weight:normal;">' + data.STATUS + '</label>');
                        }



                        if (!isEmpty(data.REQUEST_FORM_NO)) {
                            $('td:eq(5)', row).html('<a target="_blank" href="' + suburl + '/Artwork/' + data.REQUEST_FORM_ID + '" style="text-decoration: underline;">' + data.REQUEST_FORM_NO + '</a>');
                        }

                        if (!isEmpty(data.ARTWORK_NO)) {
                            $('td:eq(6)', row).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_ID + '" style="text-decoration: underline;">' + data.ARTWORK_NO + '</a>');
                        }


                        if (!isEmpty(data.MOCKUP_NO)) {
                            $('td:eq(7)', row).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                        }


                    }

                } else
                {
                    if (!isEmpty(data.STATUS)) {
                        if (data.STATUS == "In use")
                            $('td:eq(3)', row).html('<label style="color:green;margin-bottom:0px;font-weight:normal;">' + data.STATUS + '</label>');
                        else
                            $('td:eq(3)', row).html('<label style="color:red;margin-bottom:0px;font-weight:normal;">' + data.STATUS + '</label>');
                    }




                    if (!isEmpty(data.REQUEST_FORM_NO)) {
                        $('td:eq(4)', row).html('<a target="_blank" href="' + suburl + '/Artwork/' + data.REQUEST_FORM_ID + '" style="text-decoration: underline;">' + data.REQUEST_FORM_NO + '</a>');
                    }

                    if (!isEmpty(data.ARTWORK_NO)) {
                        $('td:eq(5)', row).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_ID + '" style="text-decoration: underline;">' + data.ARTWORK_NO + '</a>');
                    }


                    if (!isEmpty(data.MOCKUP_NO)) {
                        $('td:eq(6)', row).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                    }


                }

             
            },

            "drawCallback": function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var rows_data = api.rows({ page: 'current' }).data();
                var last = null;

                api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                   //if (last !== group) {
                        //$(rows).eq(i).after('<tr class="group"><td colspan="27">' + group + '1</td></tr>');
                        //$(rows).eq(i).after('<tr class="group"><td colspan="27">' + group + '2</td></tr>');
                    if (rows_data[i].listDETAIL !== null)
                    {
                            for (var x = 0; x < rows_data[i].listDETAIL.length; x++)
                            {


                                if (isCheckAWFile)
                                {

                                    // $(rows).eq(i).after('<tr class="cls_head_chk_group_group-' + rows_data[i].GROUPING + ' head-' + rows_data[i].GROUPING + '"><td><input class= "cls_hide cls_chk_mat5 cls_td_width_10" data-mat_id="' + rows_data[i].MATERIAL_LOCK_ID + '" type = "checkbox">'
                                    $(rows).eq(i).after('<tr class="cls_head_chk_group_group-' + rows_data[i].GROUPING + '"><td>'
                                        // + '</td><td colspan="12">'
                                        + '</td><td> '// + rows_data[i].MATERIAL_NO 
                                        + '</td><td> '// + rows_data[i].MATERIAL_DESCRIPTION 
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td>' + rows_data[i].listDETAIL[x].PRODUCT_CODE
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].SOLD_TO
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].SHIP_TO
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].BRAND
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].COUNTRY
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].ZONE
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].PRIMARY_SIZE
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].PACK_SIZE
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].PACKAGING_STYLE
                                        + '</td><td>'
                                        + '</td><td>'
                                        + '</td><td colspan="2"></td></tr>');
                                }
                                else
                                {
                                    // $(rows).eq(i).after('<tr class="cls_head_chk_group_group-' + rows_data[i].GROUPING + ' head-' + rows_data[i].GROUPING + '"><td><input class= "cls_hide cls_chk_mat5 cls_td_width_10" data-mat_id="' + rows_data[i].MATERIAL_LOCK_ID + '" type = "checkbox">'
                                    $(rows).eq(i).after('<tr class="cls_head_chk_group_group-' + rows_data[i].GROUPING + '"><td>'
                                        // + '</td><td colspan="12">'
                                        + '</td><td> '// + rows_data[i].MATERIAL_NO 
                                        + '</td><td> '// + rows_data[i].MATERIAL_DESCRIPTION                                       
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td> '
                                        + '</td><td>' + rows_data[i].listDETAIL[x].PRODUCT_CODE
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].SOLD_TO
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].SHIP_TO
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].BRAND
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].COUNTRY
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].ZONE
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].PRIMARY_SIZE
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].PACK_SIZE
                                        + '</td><td class="cls_nowrap">' + rows_data[i].listDETAIL[x].PACKAGING_STYLE
                                        + '</td><td>'
                                        + '</td><td>'
                                        + '</td><td colspan="2"></td></tr>');
                                }

                            }

                        }
                       // last = group;
                   // }
                });

                $($.fn.dataTable.tables(true)).DataTable().columns.adjust();      
            },
            dom: 'Bfrtip',
            buttons: ['pageLength',              
            ],

        });
    }

   //new $.fn.dataTable.FixedColumns(table_report_artworkmatcontrol, { leftColumns: 3 });


    //var isCheckAWFile = $('.cls_report_artworkmatcontrol .cls_chk_artwork_file').is(":checked") ? true : false;
    //table_report_artworkmatcontrol.column(3).visible(isCheckAWFile);


    $(table_report_artworkmatcontrol.table().container()).on('keyup', 'input', function () {
        table_report_artworkmatcontrol
            .column($(this).data('index'))
            .search(this.value, true)
            .draw();
    });


    $("#table_report_artworkmatcontrol_filter").hide();
    

   // $("#table_report_artworkmatcontrol .dt-buttons .buttons-excel").hide();

 

}


function convertDateTimeDB(date) {
    if (!isEmpty(date)) {
        return moment(date).format('YYYY-MM-DD HH:mm:ss');
    }
    else
        return "";
}

function SubmitUnlock() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    update_datetime = Date.now();
    submit_type = "unlock";
    $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        var item = {};

        item["MATERIAL_LOCK_ID"] = $(this).data("mat_id");
        item["UNLOCK_DATE_FROM_PARAM"] = $('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_from').val();
        item["UNLOCK_DATE_TO_PARAM"] = $('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_to').val();
        item["REMARK_UNLOCK"] = unlock_remark_quill.root.innerHTML;
        item["UPDATE_BY"] = UserID;
        item["UPDATE_DATE_LOCK_PARAM"] = convertDateTimeDB(update_datetime);
       
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/report/materiallockreport';
    var mytype = 'POST';
    var mydata = jsonObj;

   

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, reload_table_mat, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please inform supervisor for take action</span>');
}




var submit_type;
var update_datetime;

function reload_table_mat() {

    var isCheckAWFile = $('.cls_report_artworkmatcontrol .cls_chk_artwork_file').is(":checked") ? true : false;
    var isServerSide = $('.cls_report_artworkmatcontrol .cls_chk_freeze_column').is(":checked") ? true : false;
    var table = $('#table_report_artworkmatcontrol').DataTable();

    if (isServerSide == false) {
        //if (!isEmpty(submit_type)) {

        //    if (isCheckAWFile) {

        //        if (submit_type == 'unlock') {
        //            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        //                // $(this).closest('tr').find('td').eq(8).html('');
        //                $(this).closest('tr').find('td').eq(8).html($('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_from').val());
        //                $(this).closest('tr').find('td').eq(9).html($('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_to').val());
        //                $(this).closest('tr').find('td').eq(10).html(unlock_remark_quill.root.innerHTML);
        //                $(this).closest('tr').find('td').eq(12).html(myDateTimeMoment(update_datetime));
        //                $(this).prop("checked", false);
        //                $('.cls_report_artworkmatcontrol .cls_chk_group').change();
        //            });
        //        } else if (submit_type == 'cancel_unlcok') {
        //            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {

        //                $(this).closest('tr').find('td').eq(8).html('');
        //                $(this).closest('tr').find('td').eq(9).html('');
        //                $(this).closest('tr').find('td').eq(10).html('');
        //                $(this).closest('tr').find('td').eq(12).html(myDateTimeMoment(update_datetime));
        //                $(this).prop("checked", false);
        //                $('.cls_report_artworkmatcontrol .cls_chk_group').change();
        //            });
        //        } else if (submit_type == 'inuse') {
        //            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        //                $(this).closest('tr').find('td').eq(4).html('<label style="color:green;margin-bottom:0px;font-weight:normal;">In use</label>');  //in use
        //                $(this).closest('tr').find('td').eq(11).html('');  //remark_lock
        //                $(this).closest('tr').find('td').eq(12).html(myDateTimeMoment(update_datetime));  //log_date
        //                $(this).prop("checked", false);
        //                $('.cls_report_artworkmatcontrol .cls_chk_group').change();
        //            });
        //        } else if (submit_type == "obsolete") {
        //            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        //                $(this).closest('tr').find('td').eq(4).html('<label style="color:red;margin-bottom:0px;font-weight:normal;">Obsolete</label>');  //Obsolete
        //                $(this).closest('tr').find('td').eq(11).html(obsolete_remark_quill.root.innerHTML);  //remark_lock
        //                $(this).closest('tr').find('td').eq(12).html(myDateTimeMoment(update_datetime));  //log_date
        //                $(this).prop("checked", false);
        //                $('.cls_report_artworkmatcontrol .cls_chk_group').change();
        //            });
        //        }

        //    }
        //    else {
        //        if (submit_type == 'unlock') {
        //            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        //                // $(this).closest('tr').find('td').eq(8).html('');
        //                $(this).closest('tr').find('td').eq(8 - 1).html($('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_from').val());
        //                $(this).closest('tr').find('td').eq(9 - 1).html($('#modal_report_artworkmatcontrol_unlock .cls_dt_search_unlock_date_to').val());
        //                $(this).closest('tr').find('td').eq(10 - 1).html(unlock_remark_quill.root.innerHTML);
        //                $(this).closest('tr').find('td').eq(12 - 1).html(myDateTimeMoment(update_datetime));
        //                $(this).prop("checked", false);
        //                $('.cls_report_artworkmatcontrol .cls_chk_group').change();
        //            });
        //        } else if (submit_type == 'cancel_unlcok') {
        //            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {

        //                $(this).closest('tr').find('td').eq(8 - 1).html('');
        //                $(this).closest('tr').find('td').eq(9 - 1).html('');
        //                $(this).closest('tr').find('td').eq(10 - 1).html('');
        //                $(this).closest('tr').find('td').eq(12 - 1).html(myDateTimeMoment(update_datetime));
        //                $(this).prop("checked", false);
        //                $('.cls_report_artworkmatcontrol .cls_chk_group').change();
        //            });
        //        } else if (submit_type == 'inuse') {
        //            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        //                $(this).closest('tr').find('td').eq(4 - 1).html('<label style="color:green;margin-bottom:0px;font-weight:normal;">In use</label>');  //in use
        //                $(this).closest('tr').find('td').eq(11 - 1).html('');  //remark_lock
        //                $(this).closest('tr').find('td').eq(12 - 1).html(myDateTimeMoment(update_datetime));  //log_date
        //                $(this).prop("checked", false);
        //                $('.cls_report_artworkmatcontrol .cls_chk_group').change();


        //                //var cell = table.cell($(this).closest('tr').index, 4 - 1);
        //                //cell.data('In use').draw();
                      
        //            });
        //        } else if (submit_type == "obsolete") {
        //            $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        //                $(this).closest('tr').find('td').eq(4 - 1).html('<label style="color:red;margin-bottom:0px;font-weight:normal;">Obsolete</label>');  //Obsolete
        //                $(this).closest('tr').find('td').eq(11 - 1).html(obsolete_remark_quill.root.innerHTML);  //remark_lock
        //                $(this).closest('tr').find('td').eq(12 - 1).html(myDateTimeMoment(update_datetime));  //log_date
        //                $(this).prop("checked", false);
        //                $('.cls_report_artworkmatcontrol .cls_chk_group').change();



        //                //var cell = table.cell($(this).closest('tr').index, 4- 1);
        //                //cell.data('Obsolete').draw();

        //            });
        //        }
        //    }

        //}
        table_report_artworkmatcontrol.ajax.reload();
    } else
    {
        table_report_artworkmatcontrol.ajax.reload();
    }

  
   
    $(".cls_report_artworkmatcontrol #mat_select_all").prop('checked', false);
 
    $('#modal_report_artworkmatcontrol_unlock').modal('hide');
    $('#modal_report_artworkmatcontrol_obsolete').modal('hide');
   // table_report_artworkmatcontrol.ajax.reload();
    unlock_remark_quill.setContents([{ insert: '\n' }]);
    obsolete_remark_quill.setContents([{ insert: '\n' }]);
}


function cancelunlock() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    update_datetime = Date.now();
    submit_type = "cancel_unlcok";
    $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        var item = {};

        item["MATERIAL_LOCK_ID"] = $(this).data("mat_id");
        item["UNLOCK_DATE_FROM_PARAM"] = "";
        item["UNLOCK_DATE_TO_PARAM"] = "";
        item["REMARK_UNLOCK"] = "";
        item["UPDATE_BY"] = UserID;
        item["UPDATE_DATE_LOCK_PARAM"] = convertDateTimeDB(update_datetime);
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/report/materiallockreport';
    var mytype = 'POST';
    var mydata = jsonObj;
 
    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, reload_table_mat, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please inform supervisor for take action</span>');
    //myAjaxConfirmSubmit(myurl, mytype, mydata, reload_table_mat, "", true, true);
}


function submitInuse() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};

    update_datetime = Date.now();
    submit_type = "inuse";
    $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        var item = {};

        item["MATERIAL_LOCK_ID"] = $(this).data("mat_id");
        item["STATUS"] = "I";
        item["REMARK_LOCK"] = "";
        item["UPDATE_BY"] = UserID;
        item["UPDATE_DATE_LOCK_PARAM"] = convertDateTimeDB(update_datetime);
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/report/materiallockreport';
    var mytype = 'POST';
    var mydata = jsonObj;

    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, reload_table_mat, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please inform supervisor for take action</span>');
    //myAjaxConfirmSubmit(myurl, mytype, mydata, reload_table_mat, "", true, true);
}



function submitObsolete() {
    var jsonObj = new Object();
    var data = [];
    jsonObj.data = {};


    update_datetime = Date.now();
    submit_type = "obsolete";

    $(".cls_report_artworkmatcontrol .cls_chk_group:checked").each(function () {
        var item = {};

        item["MATERIAL_LOCK_ID"] = $(this).data("mat_id");
        item["STATUS"] = "O";
        item["REMARK_LOCK"] = obsolete_remark_quill.root.innerHTML;
        item["UPDATE_BY"] = UserID;
        item["UPDATE_DATE_LOCK_PARAM"] = convertDateTimeDB(update_datetime);
        data.push(item);
    });

    jsonObj.data = data;

    var myurl = '/api/report/materiallockreport';
    var mytype = 'POST';
    var mydata = jsonObj;
 
    myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, reload_table_mat, '', true, true, 'Do you want to submit?</br><span style="color:red;font-style: italic;">**Please inform supervisor for take action</span>');
    //myAjaxConfirmSubmit(myurl, mytype, mydata, reload_table_mat, "", true, true);
}




function getParamReportArtworkMatControl_ServerSide() {

    var isCheckAWFile = $('.cls_report_artworkmatcontrol .cls_chk_artwork_file').is(":checked") ? "X" : "";

    return "?data.SEARCH_SOLD_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_sold_to option:selected').text()
        + "&data.SEARCH_SHIP_TO=" + $('.cls_report_artworkmatcontrol .cls_lov_search_ship_to option:selected').text()
        + "&data.SEARCH_MATERIAL_NO=" + $('.cls_report_artworkmatcontrol .cls_txtarea_mat5').val().replace(/\n/g, ',')
        + "&data.SEARCH_COUNTRY=" + $('.cls_report_artworkmatcontrol .cls_txtarea_country').val().replace(/\n/g, ',')
        + "&data.SEARCH_BRAND=" + $('.cls_report_artworkmatcontrol .cls_lov_search_brand option:selected').text()
        + "&data.SEARCH_PAOWNER=" + $('.cls_report_artworkmatcontrol .cls_lov_search_pic option:selected').text()
        + "&data.SEARCH_ZONE=" + $('.cls_report_artworkmatcontrol .cls_lov_search_zone option:selected').text()
        + "&data.SEARCH_STATUS=" + $('.cls_report_artworkmatcontrol .cls_lov_search_status option:selected').val()
        + "&data.SEARCH_PRODUCT_CODE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_search_product_code').val().replace(/\n/g, ',')
        + "&data.SEARCH_PKG_TYPE=" + $('.cls_report_artworkmatcontrol .cls_txtarea_pkg').val().replace(/\n/g, ',')
        + "&data.SEARCH_REMARK_UNLOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_unlock').val().replace(/\n/g, ',')
        + "&data.SEARCH_REMARK_LOCK=" + $('.cls_report_artworkmatcontrol .cls_txtarea_remark_lock').val().replace(/\n/g, ',')
        + "&data.SEARCH_CHECK_ARTWORK_FILE=" + isCheckAWFile
        + "&data.SEARCH_SERVER_SIDE=X" ;


    //+ "&data.first_load=" + first_load;
}


function bind_table_report_artowrkmatcontrol_serverside() {

    var isCheckAWFile = $('.cls_report_artworkmatcontrol .cls_chk_artwork_file').is(":checked") ? true : false;
    table_report_artworkmatcontrol = $('#table_report_artworkmatcontrol').DataTable();


    $('.cls_table_text_search').val('');

    if (checkEnterCriteria())
    {
        table_report_artworkmatcontrol.destroy();
        table_report_artworkmatcontrol = $('#table_report_artworkmatcontrol').DataTable({
            serverSide: true,
            select: {
                style: 'multi',
                selector: 'td:first-child input',
                info: false,
                //className: 'select-checkbox',
            },
            orderCellsTop: true,
            fixedHeader: true,
            fixedColumns: {
                leftColumns: 3,

            },
            ajax: function (data, callback, settings) {

                for (var i = 0, len = data.columns.length; i < len; i++) {
                    delete data.columns[i].name;
                    delete data.columns[i].data;
                    delete data.columns[i].searchable;
                    delete data.columns[i].orderable;
                    delete data.columns[i].search.regex;
                    delete data.search.regex;
                }


                $.ajax({
                    url: suburl + "/api/report/materiallockreportv2" + getParamReportArtworkMatControl_ServerSide(),
                    type: 'GET',
                    data: data,
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

                {
                    "targets": 3,
                    "visible": isCheckAWFile
                },

            ],
            "order": [[2, 'asc']],
            "processing": true,
            "lengthChange": true,
            "ordering": false,
            "info": true,
            "scrollX": true,
            "scrollCollapse": true,
            "scrollY": "500px",
          
            "columns": [
                {
                    render: function (data, type, row, meta) {

                        if (!isEmpty(row.MATERIAL_NO)) {
                            return '<input class="cls_chk_group cls_nowrap" data-mat_id="' + row.MATERIAL_LOCK_ID + '" data-group-name="group-' + row.GROUPING + '" type="checkbox">';
                        } else {
                            // return '<input class="cls_chk_group cls_td_width_10 cls_hide" data-mat_id="' + row.MATERIAL_LOCK_ID + '" data-group-name="group-' + row.GROUPING + '" type="checkbox">';
                        }


                        // return '<input class="cls_chk_group" type="checkbox">';
                    }
                    // "data": "", "className": "cls_nowrap"
                },
                //{ "data": "GROUPING", "className": "cls_td_width_25 cls_td_mat5" },
                { "data": "MATERIAL_NO", "className": "cls_td_width_140" },
                { "data": "MATERIAL_DESCRIPTION", "className": "cls_nowrap" },
                { "data": "IS_HAS_FILES", "className": "cls_td_width_50" },
                { "data": "STATUS", "className": "cls_td_width_60 cls_status" },
                { "data": "REQUEST_FORM_NO", "className": "cls_td_width_130" },
                { "data": "ARTWORK_NO", "className": "cls_td_width_130 cls_artwork_no" },
                { "data": "MOCKUP_NO", "className": "cls_td_width_160" },
                //{ "data": "UNLOCK_DATE_FROM", "className": "cls_nowrap cls_lock_date_from" },
                {
                    className: "cls_lock_date_from cls_td_width_130",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.UNLOCK_DATE_FROM);
                    }
                },

                //{ "data": "UNLOCK_DATE_TO", "className": "cls_nowrap cls_lock_date_to" },
                {
                    className: "cls_lock_date_to cls_td_width_130",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateMoment(row.UNLOCK_DATE_TO);
                    }
                },
                { "data": "REMARK_UNLOCK", "className": "cls_td_width_240 cls_remark_unlock" },
                { "data": "REMARK_LOCK", "className": "cls_td_width_240 cls_remark_lock" },
                // { "data": "LOG_DATE", "className": "cls_nowrap cls_logdate" },
                {
                    className: "cls_logdate cls_td_width_130",
                    type: "date-uk",
                    render: function (data, type, row, meta) {
                        return myDateTimeMoment(row.LOG_DATE);
                    }
                },

                { "data": "PRODUCT_CODE", "className": "cls_nowrap" },
                { "data": "SOLD_TO", "className": "cls_nowrap" },
                { "data": "SHIP_TO", "className": "cls_nowrap" },
                { "data": "BRAND", "className": "cls_nowrap" },
                { "data": "COUNTRY", "className": "cls_nowrap" },
                { "data": "ZONE", "className": "cls_nowrap" },


                { "data": "PRIMARY_SIZE", "className": "cls_nowrap" },
                { "data": "PACK_SIZE", "className": "cls_nowrap" },
                { "data": "PACKAGING_STYLE", "className": "cls_nowrap" },

                { "data": "PACKAGING_TYPE", "className": "cls_nowrap" },
                { "data": "PRIMARY_TYPE", "className": "cls_nowrap" },
                { "data": "PA_OWNER", "className": "cls_nowrap" },
                { "data": "PG_OWNER", "className": "cls_nowrap" }
            ],
            "rowCallback": function (row, data, dataIndex) {
                //var rowId = data[0];

                if (!isEmpty(data.MATERIAL_NO)) {
                    $(row).addClass('highlight');
                    $(row).addClass('group-' + data.GROUPING);
                    //  $(row).addClass('head-' + data.GROUPING);

                }

                $(row).addClass("cls_head_chk_group_group-" + data.GROUPING);



                if (isCheckAWFile) {
                    if (!isEmpty(data.IS_HAS_FILES)) {
                        if (data.IS_HAS_FILES == "Yes")
                            $('td:eq(3)', row).html('<label style="color:green;margin-bottom:0px;font-weight:normal;">' + data.IS_HAS_FILES + '</label>');
                        else
                            $('td:eq(3)', row).html('<label style="color:red;margin-bottom:0px;font-weight:normal;">' + data.IS_HAS_FILES + '</label>');


                        if (!isEmpty(data.STATUS)) {
                            if (data.STATUS == "In use")
                                $('td:eq(4)', row).html('<label style="color:green;margin-bottom:0px;font-weight:normal;">' + data.STATUS + '</label>');
                            else
                                $('td:eq(4)', row).html('<label style="color:red;margin-bottom:0px;font-weight:normal;">' + data.STATUS + '</label>');
                        }



                        if (!isEmpty(data.REQUEST_FORM_NO)) {
                            $('td:eq(5)', row).html('<a target="_blank" href="' + suburl + '/Artwork/' + data.REQUEST_FORM_ID + '" style="text-decoration: underline;">' + data.REQUEST_FORM_NO + '</a>');
                        }

                        if (!isEmpty(data.ARTWORK_NO)) {
                            $('td:eq(6)', row).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_ID + '" style="text-decoration: underline;">' + data.ARTWORK_NO + '</a>');
                        }


                        if (!isEmpty(data.MOCKUP_NO)) {
                            $('td:eq(7)', row).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                        }


                    }

                } else {
                    if (!isEmpty(data.STATUS)) {
                        if (data.STATUS == "In use")
                            $('td:eq(3)', row).html('<label style="color:green;margin-bottom:0px;font-weight:normal;">' + data.STATUS + '</label>');
                        else
                            $('td:eq(3)', row).html('<label style="color:red;margin-bottom:0px;font-weight:normal;">' + data.STATUS + '</label>');
                    }




                    if (!isEmpty(data.REQUEST_FORM_NO)) {
                        $('td:eq(4)', row).html('<a target="_blank" href="' + suburl + '/Artwork/' + data.REQUEST_FORM_ID + '" style="text-decoration: underline;">' + data.REQUEST_FORM_NO + '</a>');
                    }

                    if (!isEmpty(data.ARTWORK_NO)) {
                        $('td:eq(5)', row).html('<a target="_blank" href="' + suburl + '/TaskFormArtwork/' + data.ARTWORK_ID + '" style="text-decoration: underline;">' + data.ARTWORK_NO + '</a>');
                    }


                    if (!isEmpty(data.MOCKUP_NO)) {
                        $('td:eq(6)', row).html('<a target="_blank" href="' + suburl + '/TaskForm/' + data.MOCKUP_ID + '" style="text-decoration: underline;">' + data.MOCKUP_NO + '</a>');
                    }


                }


            },
        });


        table_report_artworkmatcontrol.on('select', function (e, dt, type, indexes) {
            var rowData = table.rows(indexes).data().toArray();

        })
            .on('deselect', function (e, dt, type, indexes) {
                var rowData = table.rows(indexes).data().toArray();

            });

        $(table_report_artworkmatcontrol.table().container()).on('keyup', 'input', function (event) {
            if (event.which == 13) {
                table_report_artworkmatcontrol
                    .column($(this).data('index'))
                    .search(this.value)
                    .draw();
            }

        });

    }

    $("#table_report_artworkmatcontrol_filter").hide();

   // new $.fn.dataTable.FixedColumns(table_report_artworkmatcontrol, { leftColumns: 3 });
}


