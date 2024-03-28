//const { bind } = require("angular");
var str_table = "";
var str_charname = "";
var str_old_description = "";
var str_Description = "";
var str_matgroup = "";
var str_oldgroup = "";
var str_mattype = "";
var str_descriptiontext = "";
var str_value = "";
var Max_Item = 0;
var Max_Id = 0;
var str_item = "";
var str_PrimaryCode = "";
var str_GroupStyle = "";
var str_PackingStyle = "";
var str_RefStyle = "";
var str_Packsize = "";
var str_old_Packsize = "";
var str_BaseUnit = "";
var str_TypeofPrimary = "";
var str_old_id = "";
var str_id = "";
var str_old_Can = "";
var str_Running = "";

var str_Can = "";
var str_old_LidType = "";
var str_LidType = "";
var str_old_ContainerType = "";
var str_ContainerType = "";
var str_DescriptionType = "";
var str_username = "";
var str_fn = "";
var str_FirstName = "";
var str_LastName = "";
var str_Email = "";
var str_RegisteredNo = "";
var str_Address = "";
var str_Plant = "";
var str_old_vendorcode = "";
var str_Authorize_ChangeMaster = "";
var str_Product_Group = "";
var str_Product_GroupDesc = "";
var str_PRD_Plant = "";
var str_WHNumber = "";
var str_StorageType = "";
var str_LE_Qty = "";
var str_Storage_UnitType = "";
var Action = "";
var str_Active = "";
var Changed_Reason = "";
var SAPUsername = "";
var str_old_vendorcode = "";



var columns, url, CurrentUser;

var columns_export_excel;
var user_has_autrolize_change_edit;

//var table_primary_view;
var master;
var table, rowCallback;

$(document).ready(function () {
 
    clearAllData();
    checkAutrolizeEditMaster();
    bind_lov('.cls_Update_characteristic .cls_lov_master_name', '/api/report/igrid_overall_get_master', 'data.DISPLAY_TXT');



    $('.cls_igrid_btn_Insert_only').click(function (e) {
        if ($('.cls_Update_characteristic .cls_lov_master_name').val() == null) {
            alertError2('Please selecct master_name');

        }
        else {
            Action = "Insert";
            var dataTableHeaderElements = $(table).DataTable().columns().header();
            var headers = [];
            var classnames = [];
            for (var i = 0; i < dataTableHeaderElements.length; i++) {
                headers.push($(dataTableHeaderElements[i]).text());
                classnames.push($(dataTableHeaderElements[i]).context.className);
            }


            //by aof

            //clearAllData();
            master = $('.cls_Update_characteristic .cls_lov_master_name').val();

            for (let i = 1; i <= 9; i++) {

                $('.Text_M' + i + '_2').prop("disabled", false);
                $('.Text_M' + i + '_2').removeClass('cls_inactive');
                $('.Text_M' + i + '_2').removeClass('cls_id');
                $('.Text_M' + i + '_2').removeClass('cls_require');
                $('.Text_M' + i + '_2').removeClass('cls_fixkey_0_2');
                $('.Text_M' + i + '_2').removeClass('cls_isnumber');
                $('.Text_M' + i + '_2').removeClass('cls_fixkey_X_Y');
                $('.Text_M' + i + '_2').removeClass('cls_fixkey_role');

            }
            // by aof



            $.each(headers, function (index, val) {
                 //-----------------------by aof comment----------------------------------------------------
                ////val2 = val.replace(/\s/g, '');
                ////if (val2.length > 12) {
                ////    $('.Label_M' + index).html(val2.substring(0, 12) + '...');
                ////    $('.Label_M' + index + '_2').html(val2.substring(0, 12) + '...');
                ////} else {
                //    $('.Label_M' + index).html(val);
                //    $('.Label_M' + index + '_2').html(val);
                ////}
                //$('.cls_container_igrid_master .Text_M' + index).val("");
                //$('.cls_container_igrid_master .Text_M' + index + '_2').val("");
                //if ($(".Label_M" + index).is(':hidden')) {
                //    $('.Label_M' + index).show();
                //}
                //if ($(".Text_M" + index).is(':hidden')) {
                //    $('.Text_M' + index).show();
                //}
                //if ($(".Label_M" + index + "_2").is(':hidden')) {
                //    $('.Label_M' + index + '_2').show();
                //}
                //if ($(".Text_M" + index + "_2").is(':hidden')) {
                //    $('.Text_M' + index + '_2').show();
                //}
                //-----------------------by aof comment----------------------------------------------------

                //-----------------------by aof----------------------------------------------------
                var inx =index;
                var hdr =val;
                var classname = classnames[inx];

                $('.cls_container_igrid_master .Text_M' + index).val("");
                $('.cls_container_igrid_master .Text_M' + index + '_2').val("");


                $('.Text_M' + inx + '_2').prop("disabled", false);
                $('.Text_M' + inx + '_2').removeClass('cls_inactive');
                $('.Text_M' + inx + '_2').removeClass('cls_id');
                $('.Text_M' + inx + '_2').removeClass('cls_require');
                $('.Text_M' + inx + '_2').removeClass('cls_fixkey_0_2');
                $('.Text_M' + inx + '_2').removeClass('cls_isnumber');
                $('.Text_M' + inx + '_2').removeClass('cls_fixkey_X_Y');
                $('.Text_M' + inx + '_2').removeClass('cls_fixkey_role');
                //hdr2 = hdr.replace(/\s/g, '');
                //if (hdr2.length > 12) {
                //    $('.Label_M' + inx).html(hdr2.substring(0, 12) + '...');
                //    $('.Label_M' + inx + '_2').html(hdr2.substring(0, 12) + '...');
                //} else {
                $('.Label_M' + inx).html(hdr);
                $('.Label_M' + inx + '_2').html(hdr);
                //}

                if ($(".Label_M" + inx).is(':hidden')) {
                    $('.Label_M' + inx).show();
                }
                if ($(".Text_M" + inx).is(':hidden')) {
                    $('.Text_M' + inx).show();
                }
                if ($(".Label_M" + inx + "_2").is(':hidden')) {
                    $('.Label_M' + inx + '_2').show();
                }
                if ($(".Text_M" + inx + "_2").is(':hidden')) {
                    $('.Text_M' + inx + '_2').show();
                    if (master != "ulogin" || master != "ProductGroup")
                        setRequireField(classname, '.Text_M' + inx + '_2');
                }

                if (hdr == "Inactive") {
                    $('.Text_M' + inx + '_2').prop("disabled", true);
                    $('.Text_M' + inx + '_2').addClass('cls_inactive');

                    //if ($('.Text_M' + inx + '_2').val() == "X") {
                    //    $('.cls_igrid_btn_Remove').html('Re-Active');
                    //} else {
                    //    $('.cls_igrid_btn_Remove').html('Inactive');
                    //}

                }
                if (hdr == "Id") {

                    $('.Text_M' + inx + '_2').prop("disabled", true);
                    $('.Text_M' + inx + '_2').addClass('cls_id');
                }
                //-----------------------by aof----------------------------------------------------




            });
            
            insertMaster();
            $('.lbl_action').html('Action: Insert');
            $('.cls_igrid_btn_Insert').prop('disabled', true);
            $('.cls_igrid_btn_Copy').prop('disabled', true);
            $('.cls_igrid_btn_Remove').prop('disabled', true);
            $('#modal_igrid_popup').modal({ backdrop: 'static', keyboard: true });
        }
    });


    // bind_lov('.cls_Update_characteristic .cls_lov_master_name', '/api/lov/selectmaster', 'data.DISPLAY_TXT');

    $('.cls_igrid_btn_Remove').click(function (e) {

        inactiveMaster();
        $('.lbl_action').html('Action: ' + Action);
    });

    $('.cls_igrid_btn_Insert').click(function (e) {
       
        insertMaster();
        $('.lbl_action').html('Action: Insert');
    });
    $('.cls_igrid_btn_Copy').click(function (e) {
       
        copyMaster();
        $('.lbl_action').html('Action: Copy');
    });
    
    $('.cls_igrid_btn_save').click(function (e) {
        saveMaster();

    });

   
   


    $('.cls_Update_characteristic .cls_lov_master_name').on("change", function () {
        $('.cls_igrid_btn_Insert_only').prop('disabled', true);
        if (user_has_autrolize_change_edit == "Y") {
            if ($('.cls_Update_characteristic .cls_lov_master_name').val().match(/Sustain.*/)) {
                $('.cls_igrid_btn_Insert_only').prop('disabled', true);
            } else {
                $('.cls_igrid_btn_Insert_only').prop('disabled', false);
            }
        }
        selectMasterData();
    });
});


function checkAutrolizeEditMaster()
{
    var myurl = '/api/lov/authrolizeeditmaster?'
    var mytype = 'GET';
    var mydata = null;
    myAjax(myurl, mytype, mydata, callbackAutrolizeEditMaster);
}

function callbackAutrolizeEditMaster(res)
{
    user_has_autrolize_change_edit = res.haveAuthrolizeEditMaster;
}

function clearAllData() {
    $('.Text_M1').hide();
    $(".Label_M1").hide();
    $('.Text_M2').hide();
    $(".Label_M2").hide();
    $('.Text_M3').hide();
    $(".Label_M3").hide();
    $('.Text_M4').hide();
    $(".Label_M4").hide();
    $('.Text_M5').hide();
    $(".Label_M5").hide();
    $('.Text_M6').hide();
    $(".Label_M6").hide();
    $('.Text_M7').hide();
    $(".Label_M7").hide();
    $('.Text_M8').hide();
    $(".Label_M8").hide();
    $('.Text_M9').hide();
    $(".Label_M9").hide();


    $('.Text_M1_2').hide();
    $(".Label_M1_2").hide();
    $('.Text_M2_2').hide();
    $(".Label_M2_2").hide();
    $('.Text_M3_2').hide();
    $(".Label_M3_2").hide();
    $('.Text_M4_2').hide();
    $(".Label_M4_2").hide();
    $('.Text_M5_2').hide();
    $(".Label_M5_2").hide();
    $('.Text_M6_2').hide();
    $(".Label_M6_2").hide();
    $('.Text_M7_2').hide();
    $(".Label_M7_2").hide();
    $('.Text_M8_2').hide();
    $(".Label_M8_2").hide();
    $('.Text_M9_2').hide();
    $(".Label_M9_2").hide();

    $('.lbl_change_reason').hide();
    $('.txt_change_reason').hide();

    $('.div_table_primary_size').addClass('cls_hide');
    $('.div_table_brand').addClass('cls_hide');
    $('.div_table_faozone').addClass('cls_hide');

    $('.div_table_primarytype').addClass('cls_hide');
    $('.div_table_catchingarea').addClass('cls_hide');
    $('.div_table_catchingperiod').addClass('cls_hide');
    $('.div_table_pmscolour').addClass('cls_hide');
    $('.div_table_processcolour').addClass('cls_hide');
    $('.div_table_totalcolour').addClass('cls_hide');
    $('.div_table_styleofprinting').addClass('cls_hide');
    $('.div_table_typeof').addClass('cls_hide');
    $('.div_table_totalcolour').addClass('cls_hide');
    $('.div_table_symbol').addClass('cls_hide');
    //$('.div_table_sustainplastic').addClass('cls_hide');
    $('.div_table_packstyle').addClass('cls_hide');
    $('.div_table_plantregistered').addClass('cls_hide');
    $('.div_table_catchingmethod').addClass('cls_hide');
    $('.div_table_scientificname').addClass('cls_hide');
    $('.div_table_specie').addClass('cls_hide');
    $('.div_table_gradeof').addClass('cls_hide');
    $('.div_table_flute').addClass('cls_hide');
    $('.div_table_ulogin').addClass('cls_hide');
    $('.div_table_vendor').addClass('cls_hide');
    $('.div_table_productgroup').addClass('cls_hide');
    $('.div_table_whmanagement').addClass('cls_hide');
    $('.div_table_sustain').addClass('cls_hide');

}

function bindMasterViewData(columns, url, table, divclass, _rowCallback) {
   
    clearAllData();
    $(divclass).removeClass('cls_hide');

   
    var table_primary_view = $(table).DataTable()
    table_primary_view.destroy();


    table_primary_view = $(table).DataTable({
        serverSide: false,
        orderCellsTop: true,
        fixedHaeder: true,
        paging : false,
        ajax: function (data, callback, settings) {
          
        //    $.ajax({
        //        /*url: suburl + "/api/lov/primarysizeIGrid",*/
        //        url: url,
        //        type: 'GET',
        //        success: function (res) {
        //            dtSuccess(res, callback);
        //        }
        //    });
        //},
      
        ////select: {
        ////    style: 'os',
        ////    selector: 'td:first-child'
        ////},
        //"order": [[1, 'asc']],
        //"processing": true,
        ////"lengthChange": true,
        //"ordering": true,
        //"info": true,
        //"searching": true,
        //"scrollX": true,
        //"scrollY": "400px",
        //"scrollCollapse": true,
        //dom: 'Bfrtip',
        //buttons: [
        //    {
        //        title: 'IGrid Master ' + master,
        //        extend: 'excelHtml5',
        //        exportOptions: {
        //            columns: columns_export_excel,//[1, 2, 3, 4, 5, 6],
        //            format: {
        //                body: function (data, row, column, node) {
        //                    return data;
        //                }
        //            }
        //        }
        //    }//, "pageLength"
        //],
        //columns : columns,
            $.ajax({
                //url: suburl + '/api/taskform/igrid/primarysize',
                //url: suburl +'/api/taskform/igrid/info?data.Id=' + MainArtworkSubId,
                url: url,
                type: 'GET',
                data: data,
                success: function (res) {
                    dtSuccess(res, callback);
                }
            });
        },
        //fixedColumns: {
        //    leftColumns: 1
        //},
        "searching": true,
        "ordering": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,

        //"bAutoWidth": true,
        "scrollX": true,
        "scrollY": "400px",
        "scrollCollapse": true,
        columns: columns,
        select: {
            style: 'os',
            //selector: 'td:first-child'
        },
        "processing": true,
        "drawCallback": function (settings) {
          
        },
        "rowCallback": _rowCallback,
        "createdRow": function (row, data, index) {
            //if (data.STATUS == "1")
            //    $(row).css("color", "#A20025");
        },
      
    });

   
}


function callbackSaveMasterData(res) {

    $('#modal_igrid_popup').modal('hide');
    selectMasterData();
}


function inactiveMaster()
{
    //Action = "Inactive"



    for (let i = 1; i <= 9; i++) {

       

        if (!$('.cls_container_igrid_master .Text_M' + i + '_2').is(':disabled')) {
            $('.cls_container_igrid_master .Text_M' + i + '_2').val($('.cls_container_igrid_master .Text_M' + i).val());
            $('.cls_container_igrid_master .Text_M' + i + '_2').addClass('cls_disable');
            $('.cls_container_igrid_master .Text_M' + i + '_2').prop("disabled", true);
        } else
        {
        }

    }



    if ($('.cls_inactive').val() == "") {
        $('.cls_inactive').val('X');
        $('.cls_igrid_btn_Remove').html('Re-Active');
        Action = "Inactive"
        //str_Active = "X";
    } else
    {
        $('.cls_inactive').val('');
        $('.cls_igrid_btn_Remove').html('Inactive');
        Action = "Re-Active"
        //str_Active = "";
    }




}

function insertMaster()
{

    Action = "Insert";

    //var selections = $('.cls_lov_master_name').select2('data');

    $('.cls_igrid_btn_Copy').prop('disabled', false);
    $('.cls_igrid_btn_save').prop('disabled', false);
    $('.cls_id').val('');
    $('.lbl_change_reason').hide();
    $('.txt_change_reason').hide();
    $('.txt_change_reason').val('');


    if (master == "Brand")
    {
        $('.cls_id').prop('disabled', false);
    }
   

    for (let i = 0; i <= 9; i++) {

        $('.cls_container_igrid_master .Text_M' + i + '_2').val("");

        if ($('.cls_container_igrid_master .Text_M' + i + '_2').hasClass('cls_disable'))
        {
            $('.cls_container_igrid_master .Text_M' + i + '_2').removeClass('cls_disable');
            $('.cls_container_igrid_master .Text_M' + i + '_2').prop("disabled", false);
        }


    }
    //switch (master) {
    //    case "ulogin":
    //        for (let i = 0; i <= 7; i++) {
    //            $('.cls_container_igrid_master .Text_M' + i + '_2').val("");
    //        }
    //        break;
    //    default:
    //        $('.cls_container_igrid_master .Text_M2_2').val("");
    //        break;
    //}
}

function copyMaster()
{
    Action = "Insert";

    $('.lbl_change_reason').hide();
    $('.txt_change_reason').hide();
    $('.txt_change_reason').val('');

    for (let i = 0; i <= 9; i++) {
        if (!$('.cls_container_igrid_master .Text_M' + i + '_2').is(':disabled')) {
            $('.cls_container_igrid_master .Text_M' + i + '_2').val($('.cls_container_igrid_master .Text_M' + i).val());
        } else
        {
            $('.cls_container_igrid_master .Text_M' + i + '_2').val('');
        }
      
    }
    //switch (master) {
    //    case "ulogin":
    //        for (let i = 0; i <= 8; i++) {
    //            $('.cls_container_igrid_master .Text_M' + i + '_2').val($('.cls_container_igrid_master .Text_M' + i).val());
    //        }
    //    default:
    //        $('.cls_container_igrid_master .Text_M2_2').val($('.cls_container_igrid_master .Text_M2').val());
    //        break;
    //}
}


function saveMaster() {


    var f_can_save = true;

    var ctrl_name;
    var lbl_name;
    var data;
    for (let i = 1; i <= 9; i++) {

        ctrl_name = '.cls_container_igrid_master .Text_M' + i + '_2';
        lbl_name = $('.cls_container_igrid_master .Label_M' + i + '_2').html();
        data = $(ctrl_name).val();
        if ($(ctrl_name).hasClass('cls_require'))
        {
            if (data == '')
            {
                alertError2('Require field {' + lbl_name + '}.');
                f_can_save = false;
            }

        }

        if (f_can_save == true)
        {
            if ($(ctrl_name).hasClass('cls_fixkey_0_2'))
            {
                if (data != '0' && data != "2")
                {
                    alertError2('Please input material type only 0 or 2.{' + lbl_name + '}.');
                    f_can_save = false;
                }
            }
        }

        if (f_can_save == true) {
            if ($(ctrl_name).hasClass('cls_isnumber')) {
                if (!$.isNumeric(data)) {
                    alertError2('For {' + lbl_name + '} can only input the number.');
                    f_can_save = false;
                }
            }
        }


        if (f_can_save == true) {
            if ($(ctrl_name).hasClass('cls_fixkey_X_Y')) {
                if (data.toLowerCase() != 'n' && data.toLowerCase() != "y") {
                    alertError2('Please input an {' + lbl_name + '} to change master ( Y or N ).');
                    f_can_save = false;
                }
            }
        }

        if (f_can_save == true) {
            if ($(ctrl_name).hasClass('cls_fixkey_role')) {
                if (data.toUpperCase() != "PA" && data.toUpperCase() != "PA_APPROVE" && data.toUpperCase() != "PG" && data.toUpperCase() != "PG_APPROVE" && data.toUpperCase() != "PA,PA_APPROVE" && data.toUpperCase() != "PA_APPROVE,PA" && data.toUpperCase() != "PG,PG_APPROVE" && data.toUpperCase() != "PG_APPROVE,PG") {
                    alertError2('Please input correct role for this user.{' + lbl_name + '}.');
                    f_can_save = false;
                }
            }
        }
        
    }

    
    
    
    
    
    
    

    //cls_require

    if (f_can_save)
    {

        str_Active = "X"
        str_old_id = "";
        str_old_description = "";
        Changed_Reason = "";

        if (Action == "Update" || Action == "Inactive" || Action == "Re-Active") {
            str_old_id = $('.cls_container_igrid_master .Text_M1').val();
            str_old_description = $('.cls_container_igrid_master .Text_M2').val();

            if ($('.txt_change_reason').val() == '') {
                alertError2('Please input the reason for Update this record.');
                f_can_save = false;
                //break;
            } else {
                Changed_Reason = $('.txt_change_reason').val();
            }

        }
    }


    if (f_can_save)
    {
        switch (master) {
            case "Brand":
                str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_table = "MasBrand";
                str_charname = "ZPKG_SEC_BRAND";
                break;
            case "CatchingArea":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_table = "MasCatchingArea";
                str_charname = "ZPKG_SEC_CATCHING_AREA";
                break;
            case "CatchingMethod":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_table = "MasCatchingMethod";
                str_charname = "ZPKG_SEC_CATCHING_METHOD";
                break;
            case "CatchingPeriod":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_table = "MasCatchingperiodDate";
                str_charname = "ZPKG_SEC_CATCHING_PERIOD";
                break;
            case "FAOZone":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_table = "MasFAOZone";
                str_charname = "ZPKG_SEC_FAO";
                break;
            case "Flute":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_matgroup = $('.cls_container_igrid_master .Text_M3_2').val();
                str_table = "MasFlute";
                str_charname = "";
                break;
            case "Gradeof":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_matgroup = $('.cls_container_igrid_master .Text_M3_2').val();
                str_table = "MasGradeofCarton";
                str_charname = "";
                break
            case "PackStyle":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_PrimaryCode = $('.cls_container_igrid_master .Text_M2_2').val();
                str_GroupStyle = $('.cls_container_igrid_master .Text_M3_2').val();
                str_PackingStyle = $('.cls_container_igrid_master .Text_M4_2').val();
                str_RefStyle = $('.cls_container_igrid_master .Text_M5_2').val();
                str_Packsize = $('.cls_container_igrid_master .Text_M6_2').val();
                str_BaseUnit = $('.cls_container_igrid_master .Text_M7_2').val();
                str_TypeofPrimary = $('.cls_container_igrid_master .Text_M8_2').val();
                str_matgroup = "";
                str_table = "MasPackingStyle";
                str_charname = "ZPKG_SEC_PACKING";


                if (Action != "Insert") {
                   
                    str_old_description = str_GroupStyle;
                   // save TransMaster in case new pack only
                }
              

                break;
            case "PlantRegistered":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_RegisteredNo = $('.cls_container_igrid_master .Text_M2_2').val();
                str_Address = $('.cls_container_igrid_master .Text_M3_2').val();
                str_Plant = $('.cls_container_igrid_master .Text_M4_2').val();
                str_table = "MasPlantRegisteredNo";
                str_charname = "";
                break;
            case "PMSColour":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_matgroup = $('.cls_container_igrid_master .Text_M3_2').val();
                str_table = "MasPMSColour";
                str_charname = "";
                break;
            case "PrimaryType":
               // str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();             
                str_id = str_Description;
                str_table = "MasTypeofPrimary";
                str_charname = "ZPKG_SEC_PRIMARY_TYPE";

                break;
            case "PrimarySize":          
                str_item = "";
                str_id = $('.cls_container_igrid_master .Text_M2_2').val();
                str_Can = $('.cls_container_igrid_master .Text_M3_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M4_2').val();
                str_LidType = $('.cls_container_igrid_master .Text_M5_2').val();
                str_ContainerType = $('.cls_container_igrid_master .Text_M6_2').val();
                str_DescriptionType = $('.cls_container_igrid_master .Text_M7_2').val();
                str_matgroup = "";
                str_table = "MasPrimarySize";
                str_charname = "";

                if (Action == "Update" || Action == "Inactive" || Action == "Re-Active") {
                    str_old_id = $('.cls_container_igrid_master .Text_M1').val();
                    str_old_description = str_Can;
                }

                break;
            case "ProcessColour":
               // str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_matgroup = $('.cls_container_igrid_master .Text_M3_2').val();
                str_table = "MasProcessColour";
                str_charname = "";
                break;
            case "ProductGroup":

               // str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Product_Group = $('.cls_container_igrid_master .Text_M2_2').val();
                str_Product_GroupDesc = $('.cls_container_igrid_master .Text_M3_2').val();
                str_PRD_Plant = $('.cls_container_igrid_master .Text_M4_2').val();
                str_table = "MasProductGroup";
                str_charname = "";
                // no function in store for inactive/re-active
                break;
            case 'ScientificName':
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_table = "MasScientificName";
                str_charname = "ZPKG_SEC_SCIENTIFIC_NAME";
                break;     
            case "Specie":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_table = "MasSpecie";
                str_charname = "ZPKG_SEC_SPECIE";
                break;
            case "StyleofPrinting":
               // str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_matgroup = $('.cls_container_igrid_master .Text_M3_2').val();


                str_table = "MasStyleofPrinting";
                str_charname = "";
                break;
            case "Symbol":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_table = "MasSymbol";
                str_charname = "ZPKG_SEC_SYMBOL";
                break;
            case "TotalColour":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_matgroup = $('.cls_container_igrid_master .Text_M3_2').val();
                str_table = "MasTotalColour";
                str_charname = "";
                break;
            case "TypeOf":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_matgroup = $('.cls_container_igrid_master .Text_M3_2').val();
                str_mattype = $('.cls_container_igrid_master .Text_M4_2').val();
                str_descriptiontext = $('.cls_container_igrid_master .Text_M5_2').val();
                str_table = "MasTypeofCarton";
                str_charname = "";
                break;
            case "Vendor":

              
                str_id = $('.cls_container_igrid_master .Text_M2_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M3_2').val();

                str_table = "MasVendor";
                str_charname = "ZPKG_SEC_VENDOR";

                if (Action == "Update" || Action == "Inactive" || Action == "Re-Active") {
                    str_old_id = $('.cls_container_igrid_master .Text_M1').val();
                    str_old_vendorcode = $('.cls_container_igrid_master .Text_M2').val();
                    str_old_description = $('.cls_container_igrid_master .Text_M3').val();
                }
                break;
            case "ulogin":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_username = $('.cls_container_igrid_master .Text_M2_2').val();
                str_fn = $('.cls_container_igrid_master .Text_M3_2').val();
                str_FirstName = $('.cls_container_igrid_master .Text_M4_2').val();
                str_LastName = $('.cls_container_igrid_master .Text_M5_2').val();
                str_Email = $('.cls_container_igrid_master .Text_M6_2').val();
                str_Authorize_ChangeMaster = $('.cls_container_igrid_master .Text_M7_2').val();
                SAPUsername = $('.cls_container_igrid_master .Text_M8_2').val();
                str_table = "ulogin";
                str_charname = "";
                break;
            case "WHManagement":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Product_Group = $('.cls_container_igrid_master .Text_M2_2').val();
                str_Product_GroupDesc = $('.cls_container_igrid_master .Text_M3_2').val();
                str_PRD_Plant = $('.cls_container_igrid_master .Text_M4_2').val();
                str_WHNumber = $('.cls_container_igrid_master .Text_M5_2').val();
                str_StorageType = $('.cls_container_igrid_master .Text_M6_2').val();
                str_LE_Qty = $('.cls_container_igrid_master .Text_M7_2').val();
                str_Storage_UnitType = $('.cls_container_igrid_master .Text_M8_2').val();
                str_table = "MasLogistics";
                str_charname = "";
                if (Action == "Update" || Action == "Inactive" || Action == "Re-Active") {
                   // str_old_id = $('.cls_container_igrid_master .Text_M1').val();
                    str_old_description = str_Product_Group + '-' + str_PRD_Plant;
                }

                break;
            // no function in store for inactive/re-active
            case "SustainCertSourcing":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_value = $('.cls_container_igrid_master .Text_M3_2').val();
                str_matgroup = $('.cls_container_igrid_master .Text_M4_2').val();
                str_table = "MasSustainCertSourcing";
                str_charname = "ZPKG_SEC_CERT_SOURCE";
                break;
            case "SustainMaterial":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_value = $('.cls_container_igrid_master .Text_M3_2').val();
                str_matgroup = $('.cls_container_igrid_master .Text_M4_2').val();
                str_table = "MasSustainMaterial";
                str_charname = "ZPKG_SEC_MATERIAL";
                break;
            case "SustainPlastic":
                //str_id = $('.cls_container_igrid_master .Text_M1_2').val();
                str_Description = $('.cls_container_igrid_master .Text_M2_2').val();
                str_id = str_Description;
                str_value = $('.cls_container_igrid_master .Text_M3_2').val();
                str_matgroup = $('.cls_container_igrid_master .Text_M4_2').val();
                str_table = "MasSustainPlastic";
                str_charname = "ZPKG_SEC_PLASTIC";
                break;
        }
        var form_name = '.cls_tfigrid ';

        var jsonObj = new Object();
        jsonObj.data = [];
        var item = {};
        item["Changed_Tabname"] = str_table;
        item["Changed_Charname"] = str_charname;
        item["Old_Id"] = str_old_id;
        item["Id"] = str_id;
        item["Old_Description"] = str_old_description;
        item["Description"] = str_Description;
        item["Changed_Action"] = Action;
        item["Changed_By"] = CurrentUser;
        item["Active"] = str_Active;
        item["Material_Group"] = str_matgroup;
        item["Material_Type"] = str_mattype;
        item["DescriptionText"] = str_descriptiontext;
        item["Can"] = str_Can;
        item["LidType"] = str_LidType;
        item["ContainerType"] = str_ContainerType;
        item["DescriptionType"] = str_DescriptionType;
        item["user_name"] = str_username;
        item["fn"] = str_fn;
        item["FirstName"] = str_FirstName;
        item["LastName"] = str_LastName;
        item["Email"] = str_Email;
        item["Authorize_ChangeMaster"] = str_Authorize_ChangeMaster;
        item["PrimaryCode"] = str_PrimaryCode;
        item["GroupStyle"] = str_GroupStyle;
        item["PackingStyle"] = str_PackingStyle;
        item["RefStyle"] = str_RefStyle;
        item["Packsize"] = str_Packsize;
        item["BaseUnit"] = str_BaseUnit;
        item["TypeofPrimary"] = str_TypeofPrimary;
        item["RegisteredNo"] = str_RegisteredNo;
        item["Address"] = str_Address;
        item["Plant"] = str_Plant;
        item["Product_Group"] = str_Product_Group;
        item["Product_GroupDesc"] = str_Product_GroupDesc;
        item["PRD_Plant"] = str_PRD_Plant;
        item["WHNumber"] = str_WHNumber;
        item["StorageType"] = str_StorageType;
        item["LE_Qty"] = str_LE_Qty;
        item["Storage_UnitType"] = str_Storage_UnitType;
        item["Changed_Reason"] = Changed_Reason;
        item["SAP_EDPUsername"] = SAPUsername;
        item["Value"] = str_value;
        jsonObj.data = item;
        var myurl = '/api/lov/savemaster';
        var mytype = 'POST';
        var mydata = jsonObj;

        myAjaxConfirmSubmit(myurl, mytype, mydata, callbackSaveMasterData);
    }
}


function selectMasterData()
{

   // var selections = $(this).select2('data');
    //var v = $(this)[0].value;
    //var isClear = true;

    var f_can_manage_master_data = true;

    master = $('.cls_Update_characteristic .cls_lov_master_name').val();
    rowCallback = function (row, data, index) {
    }
    $('#popup_attachment_label').html(master);

    switch (master) {
        case 'PrimarySize':
            columns_export_excel = [1, 2, 3, 4, 5, 6, 7, 8];
            //columns =
            //[
            //    {
            //        data: null,
            //        defaultContent: '',
            //        className: 'select-checkbox',
            //        orderable: false
            //    },
            //    { data: "Id", "className": "cls_nowrap cls_hide" },
            //    { data: "Code", "className": "cls_nowrap cls_code" },
            //    { data: "Can", "className": "cls_nowrap" },
            //    { data: "Description", "className": "cls_nowrap cls_description" },
            //    { data: "LidType", "className": "cls_nowrap" },
            //    { data: "ContainerType", "className": "cls_nowrap" },

            //    { data: "DescriptionType", "className": "cls_nowrap cls_description_type" },
            //];
            rowCallback = function (row, data, index) {

                $(row).find('.cls_description_type').text(data.DescriptionType);

                //var dataCheck = data.Code + ":" + data.Description;
                //if (checkDataIsExist(dataCheck, default_value)) {
                //    $(row).find('.cls_code').addClass('fontRed');
                //    $(row).find('.cls_description').addClass('fontRed');
                //} else {
                //    $(row).find('.cls_code').addClass('fontBlack');
                //    $(row).find('.cls_description').addClass('fontBlack');
                //}
            }
            columns = [
                {
                    //render: function (data, type, row, meta) {
                    //    return meta.row + meta.settings._iDisplayStart + 1;
                    //}
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className : 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Code", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Can", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "LidType", "className": "cls_nowrap cls_len_030 cls_require" },
                //{ data: "ContainerType", "className": "cls_nowrap" },
                { data: "ContainerType", "className": "cls_nowrap cls_len_030 cls_require" },
                //{ data: "DescriptionType", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "DescriptionType", "className": "cls_nowrap cls_description_type" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/primarysizeIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster="+ user_has_autrolize_change_edit;
            table = '#table_primary_size_view';
            divclass = '.div_table_primary_size';
            break;
        case 'FAOZone':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    //render: function (data, type, row, meta) {
                    //    return '<input class="cls_chk_sendtopp" type="checkbox">';
                    //}
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/FAOZoneIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_faozone_view';
            divclass = '.div_table_faozone';
            break;
        
        case 'Symbol':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/SymbolIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_symbol_view';
            divclass = '.div_table_symbol';
            break;
        case 'TypeOf':
            columns_export_excel = [1, 2, 3,4,5,6];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "MaterialType", "className": "cls_nowrap cls_fixkey_0_2 cls_len_001" },
                { data: "DescriptionText", "className": "cls_nowrap cls_len_030" },
                { data: "Inactive", "className": "cls_nowrap" },

            ]
            url = suburl + "/api/lov/TypeOfIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_typeof_view';
            divclass = '.div_table_typeof';
            break;
        case 'StyleofPrinting':
            columns_export_excel = [1, 2, 3, 4];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },

            ]
            url = suburl + "/api/lov/StyleofPrintingIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_styleofprinting_view';
            divclass = '.div_table_styleofprinting';
            break;
        case 'TotalColour':
            columns_export_excel = [1, 2, 3, 4];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_020 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/TotalColourIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_totalcolour_view';
            divclass = '.div_table_totalcolour';
            break;
        case 'ProcessColour':
            columns_export_excel = [1, 2, 3, 4];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_002 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/ProcessColourIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_processcolour_view';
            divclass = '.div_table_processcolour';
            break;
        case 'PMSColour':
            columns_export_excel = [1, 2, 3, 4];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_002 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/PMSColourIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_pmscolour_view';
            divclass = '.div_table_pmscolour';
            break;
        case 'CatchingPeriod':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/CatchingPeriodIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_catchingperiod_view';
            divclass = '.div_table_catchingperiod';
            break;
        case 'CatchingArea':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/CatchingAreaIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_catchingarea_view';
            divclass = '.div_table_catchingarea';
            break;
        case 'PackStyle':
            columns_export_excel = [1, 2, 3, 4,5,6,7,8,9];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "PrimaryCode", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "GroupStyle", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "PackingStyle", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "RefStyle", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "PackSize", "className": "cls_nowrap cls_len_010 cls_require cls_isnumber" },
                { data: "BaseUnit", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "TypeofPrimary", "className": "cls_nowrap cls_len_030" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/PackStyleIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_packstyle_view';
            divclass = '.div_table_packstyle';
            break;
        case 'CatchingMethod':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/CatchingMethodIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_catchingmethod_view';
            divclass = '.div_table_catchingmethod';
            break;
        case 'Specie':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/SpecieIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_specie_view';
            divclass = '.div_table_specie';
            break;
        case 'ulogin':
            columns_export_excel = [1, 2, 3,4,5,6,7,8,9];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "user_name", "className": "cls_nowrap cls_nowrap cls_len_030 cls_require" },
                { data: "fn", "className": "cls_nowrap cls_nowrap cls_len_030 cls_require cls_fixkey_role" },
                { data: "FirstName", "className": "cls_nowrap cls_nowrap cls_len_030 cls_require" },
                { data: "LastName", "className": "cls_nowrap cls_nowrap cls_len_030 cls_require" },
                { data: "Email", "className": "cls_nowrap cls_nowrap cls_len_225 cls_require" },
                { data: "Authorize_ChangeMaster", "className": "cls_nowrap cls_nowrap cls_len_001 cls_fixkey_X_Y" },
                { data: "SAP_EDPUsername", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/uloginIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_ulogin_view';
            divclass = '.div_table_ulogin';
            break;
        case 'Vendor':
            columns_export_excel = [1, 2, 3, 4];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Code", "className": "cls_nowrap cls_len_008 cls_require cls_isnumber" },
                { data: "Name", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/VendorIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_vendor_view';
            divclass = '.div_table_vendor';
            break;
        case 'ProductGroup':
            columns_export_excel = [1, 2, 3, 4, 5];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Product_Group", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Product_GroupDesc", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "PRD_Plant", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/ProductGroupIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_productgroup_view';
            divclass = '.div_table_productgroup';
            break;
        case 'WHManagement':
            columns_export_excel = [1, 2, 3, 4, 5, 6, 7, 8, 9];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "ProductGroup", "className": "cls_nowrap cls_len_001 cls_require" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Plant", "className": "cls_nowrap cls_len_004 cls_require" },
                { data: "WHNumber", "className": "cls_nowrap cls_len_003 cls_require" },
                { data: "StorageType", "className": "cls_nowrap cls_len_003 cls_require" },
                { data: "LE_Qty", "className": "cls_nowrap cls_len_008 cls_require cls_isnumber" },
                { data: "Storage_UnitType", "className": "cls_nowrapcls_len_003 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/WHManagementIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_whmanagement_view';
            divclass = '.div_table_whmanagement';
            break;
        case 'Flute':
            columns_export_excel = [1, 2, 3, 4];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_002 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/FluteIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_flute_view';
            divclass = '.div_table_flute';
            break;
        case 'Gradeof':
            columns_export_excel = [1, 2, 3, 4];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/GradeofIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_gradeof_view';
            divclass = '.div_table_gradeof';
            break;
        case 'ScientificName':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/ScientificNameIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_scientificname_view';
            divclass = '.div_table_scientificname';
            break;
        case 'PlantRegistered':
            columns_export_excel = [1, 2, 3, 4, 5];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap" },
                { data: "RegisteredNo", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Address", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Plant", "className": "cls_nowrap cls_len_030" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/PlantRegisteredIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_plantregistered_view';
            divclass = '.div_table_plantregistered';
            break;
        case 'PrimaryType':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "Id", "className": "cls_nowrap" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/PrimaryTypeIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_primarytype_view';
            divclass = '.div_table_primarytype';
            break;
        case 'Brand':
            columns_export_excel = [1, 2, 3];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", "className": "cls_nowrap cls_len_003 cls_require" },
                { data: "DISPLAY_TXT", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ];
            url = suburl + "/api/lov/brandIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_brand_view';
            divclass = '.div_table_brand';
            break;
        case 'SustainPlastic':
            columns_export_excel = [1, 2, 3, 4, 5];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", title: "Id" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "value", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/SustainPlasticIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_sustain_view';
            divclass = '.div_table_sustain';
            f_can_manage_master_data = false;
            break;
        case 'SustainMaterial':
            columns_export_excel = [1, 2, 3, 4, 5];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", title: "Id" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "value", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/SustainMaterialIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_sustain_view';
            divclass = '.div_table_sustain';
            f_can_manage_master_data = false;
            break;
        case 'SustainCertSourcing':
            columns_export_excel = [1, 2, 3, 4, 5];
            columns = [
                {
                    data: null,
                    //title: "checkbox",
                    defaultContent: '',
                    //className: 'select-checkbox',
                    className: 'cls_hide',
                    orderable: false
                },
                { data: "ID", title: "Id" },
                { data: "Description", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "value", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "MaterialGroup", "className": "cls_nowrap cls_len_030 cls_require" },
                { data: "Inactive", "className": "cls_nowrap" },
            ]
            url = suburl + "/api/lov/SustainCertSourcingIGrid?data.IsCheckAuthorize=X&data.Authorize_ChangeMaster=" + user_has_autrolize_change_edit;
            table = '#table_sustain_view';
            divclass = '.div_table_sustain';
            f_can_manage_master_data = false;
            break;
        default:
            f_can_manage_master_data = false;
            break;
    }

    bindMasterViewData(columns, url, table, divclass, rowCallback);

    if (user_has_autrolize_change_edit == "Y" && f_can_manage_master_data)
    {

        $(document).on('click', table + ' tr', function () {
            str_Active = "";
            Action = "Update";

            for (let i = 1; i <= 9; i++) {

                $('.Text_M' + i + '_2').prop("disabled", false);
                $('.Text_M' + i + '_2').removeClass('cls_inactive');
                $('.Text_M' + i + '_2').removeClass('cls_id');
                $('.Text_M' + i + '_2').removeClass('cls_require');
                $('.Text_M' + i + '_2').removeClass('cls_fixkey_0_2');
                $('.Text_M' + i + '_2').removeClass('cls_isnumber');
                $('.Text_M' + i + '_2').removeClass('cls_fixkey_X_Y');
                $('.Text_M' + i + '_2').removeClass('cls_fixkey_role');

            }

            var tableData = $(this).children("td").map(function () {
                //debugger;
                var inx = $(this).index();
                var hdr = $(this).closest('table').find('th').eq(inx).text();

                $('.Text_M' + inx + '_2').prop("disabled", false);
                $('.Text_M' + inx + '_2').removeClass('cls_inactive');
                $('.Text_M' + inx + '_2').removeClass('cls_id');
                $('.Text_M' + inx + '_2').removeClass('cls_require');
                $('.Text_M' + inx + '_2').removeClass('cls_fixkey_0_2');
                $('.Text_M' + inx + '_2').removeClass('cls_isnumber');
                $('.Text_M' + inx + '_2').removeClass('cls_fixkey_X_Y');
                $('.Text_M' + inx + '_2').removeClass('cls_fixkey_role');
                //hdr2 = hdr.replace(/\s/g, '');
                //if (hdr2.length > 12) {
                //    $('.Label_M' + inx).html(hdr2.substring(0, 12) + '...');
                //    $('.Label_M' + inx + '_2').html(hdr2.substring(0, 12) + '...');
                //} else {
                    $('.Label_M' + inx).html(hdr);
                    $('.Label_M' + inx + '_2').html(hdr);
                //}

                if ($(".Label_M" + inx).is(':hidden')) {
                    $('.Label_M' + inx).show();
                }
                if ($(".Text_M" + inx).is(':hidden')) {
                    $('.Text_M' + inx).show();
                }
                if ($(".Label_M" + inx + "_2").is(':hidden')) {
                    $('.Label_M' + inx + '_2').show();
                }
                if ($(".Text_M" + inx + "_2").is(':hidden')) {
                    $('.Text_M' + inx + '_2').show();
                    setRequireField($(this).closest('table').find('th').eq(inx).context.className, '.Text_M' + inx + '_2');
                }

                if (hdr == "Inactive") {
                    $('.Text_M' + inx + '_2').prop("disabled", true);
                    $('.Text_M' + inx + '_2').addClass('cls_inactive');

                    //if ($('.Text_M' + inx + '_2').val() == "X") {
                    //    $('.cls_igrid_btn_Remove').html('Re-Active');
                    //} else {
                    //    $('.cls_igrid_btn_Remove').html('Inactive');
                    //}

                }
                if (hdr == "Id") {

                    $('.Text_M' + inx + '_2').prop("disabled", true);
                    $('.Text_M' + inx + '_2').addClass('cls_id');
                }

                return $(this).text();
            }).get();

            for (var i = 0; i < tableData.length; i++) {
                $('.cls_container_igrid_master .Text_M' + i).val($.trim(tableData[i]));
                $('.cls_container_igrid_master .Text_M' + i + '_2').val($.trim(tableData[i]));


                if ($('.cls_container_igrid_master .Text_M' + i + '_2').hasClass('cls_inactive'))
                {
                    if ($('.cls_container_igrid_master .Text_M' + i + '_2').val() == "X") {
                        $('.cls_igrid_btn_Remove').html('Re-Active');
                    } else {
                        $('.cls_igrid_btn_Remove').html('Inactive');
                    }

                }

            }

            $('.lbl_change_reason').show();
            $('.txt_change_reason').show();
            $('.txt_change_reason').val('');
            $('.lbl_action').html('Action: Update');


          // disable button Inactrive/Reactive
         


            switch (master) {
                case 'PrimarySize__SKIP':
                    $('.cls_igrid_btn_popup_master').prop('disabled', false);
                    $('.cls_igrid_btn_Copy').prop('disabled', true);
                    $('.cls_igrid_btn_Insert').prop('disabled', false);
                    $('.cls_igrid_btn_save').prop('disabled', true);
                    $('.cls_igrid_btn_Remove').prop('disabled', true);
                    break;
                default:
                    $('.cls_igrid_btn_popup_master').prop('disabled', false);
                    $('.cls_igrid_btn_Copy').prop('disabled', true);
                    $('.cls_igrid_btn_Insert').prop('disabled', false);
                    $('.cls_igrid_btn_save').prop('disabled', false);
                    $('.cls_igrid_btn_Remove').prop('disabled', false);
                    break;
            }



         
            switch (table) {
                case '#table_productgroup_view':
                case '#table_whmanagement_view':
                    $('.cls_igrid_btn_popup_master').prop('disabled', true);
                    break;
            }



            //alert("Your data is: " + $.trim(tableData[0]) + " , " + $.trim(tableData[1]));
            //Label_M1
            //if ($(".Label_M1").is(':hidden')) {
            //    $('.Label_M1').show();
            //}
            //if ($(".Text_M1").is(':hidden')) {
            //    $('.Text_M1').show();
            //}
            //if ($(".Label_M1_2").is(':hidden')) {
            //    $('.Label_M1_2').show();
            //}
            //if ($(".Text_M1_2").is(':hidden')) {
            //    $('.Text_M1_2').show();
            //}

            //if ($(".Label_M2").is(':hidden')) {
            //    $('.Label_M2').show();
            //}
            //if ($(".Text_M2").is(':hidden')) {
            //    $('.Text_M2').show();
            //}
            //if ($(".Label_M2_2").is(':hidden')) {
            //    $('.Label_M2_2').show();
            //}
            //if ($(".Text_M2_2").is(':hidden')) {
            //    $('.Text_M2_2').show();
            //}

            //$('.Label_M1').html('ID');
            //$('.Label_M1_2').html('ID');
            //$('.cls_container_igrid_master .Text_M1').val($.trim(tableData[1]));
            //$('.cls_container_igrid_master .Text_M1_2').val($.trim(tableData[1]));

            //$('.Label_M2').html('Description');
            //$('.Label_M2_2').html('Description');
            //$('.cls_container_igrid_master .Text_M2').val($.trim(tableData[2]));
            //$('.cls_container_igrid_master .Text_M2_2').val($.trim(tableData[2]));

            $('#modal_igrid_popup').modal({ backdrop: 'static', keyboard: true });
        });
    }

}

function setRequireField(clsname, input) {
    var indexStr = clsname.indexOf('cls_len');
    if (indexStr != - 1) {
        var maxlenght = clsname.substr(indexStr + 8, 3);


        if ($.isNumeric(maxlenght)) {
            $(input).attr("maxlength", parseInt(maxlenght));
        }


    }

    indexStr = clsname.indexOf('cls_require');
    if (indexStr != - 1) {
        $(input).addClass('cls_require');
    }

    indexStr = clsname.indexOf('cls_fixkey_0_2');
    if (indexStr != - 1) {
        $(input).addClass('cls_fixkey_0_2');
    }

    indexStr = clsname.indexOf('cls_isnumber');
    if (indexStr != - 1) {
        $(input).addClass('cls_isnumber');
    }

    indexStr = clsname.indexOf('cls_fixkey_X_Y');
    if (indexStr != - 1) {
        $(input).addClass('cls_fixkey_X_Y');
    }

    indexStr = clsname.indexOf('cls_fixkey_role');
    if (indexStr != - 1) {
        $(input).addClass('cls_fixkey_role');
    }


}