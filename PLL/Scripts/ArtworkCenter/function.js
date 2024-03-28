$(document).ready(function () {
    $(document).bind('dragover', function (e) {
        var dropZones = $('.dropzone'),
            timeout = window.dropZoneTimeout;
        if (timeout) {
            clearTimeout(timeout);
        } else {
            dropZones.addClass('in');
        }
        var hoveredDropZone = $(e.target).closest(dropZones);
        dropZones.not(hoveredDropZone).removeClass('hover');
        hoveredDropZone.addClass('hover');
        window.dropZoneTimeout = setTimeout(function () {
            window.dropZoneTimeout = null;
            dropZones.removeClass('in hover');
        }, 100);
    });

    $(document).bind('drop dragover', function (e) {
        e.preventDefault();
    });

    jQuery.validator.methods.date = function (value, element) {
        var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
        if (isChrome) {
            var d = new Date();
            return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(value)));
        } else {
            return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
        }
    };

    $(".cls_my_datepicker").datepicker({ dateFormat: 'dd/mm/yy' }).val();
    $(".cls_my_datepicker").on('change keyup paste', function () {
        if ($(this).val().length == 10) {
            if (!isValidDate($(this).val())) {
                alertError2('Invalid date (dd/mm/yyyy).');
                $(this).val('');
            }
        }
    });
    $(".cls_my_datepicker").on('blur', function () {
        if ($(this).val().length > 0) {
            if (!isValidDate($(this).val())) {
                alertError2('Invalid date (dd/mm/yyyy).');
                $(this).val('');
            }
        }
    });

    $(".cls_my_datepicker_mindate_today").datepicker({ dateFormat: 'dd/mm/yy', minDate: 0 }).val();
    $(".cls_my_datepicker_mindate_today").on('change keyup paste', function () {
        if ($(this).val().length == 10) {
            if (!isValidDate($(this).val())) {
                alertError2('Invalid date (dd/mm/yyyy).');
                $(this).val('');
            }
            else {
                var dateObject = $(this).datepicker('getDate');
                if (dateObject < GetCurrentDateWithoutTime()) {
                    $(this).val('');
                    alertError2('Please input date equal or more than today.');
                }
            }
        }
    });
    $(".cls_my_datepicker_mindate_today").on('blur', function () {
        if ($(this).val().length > 0) {
            if (!isValidDate($(this).val())) {
                alertError2('Invalid date (dd/mm/yyyy).');
                $(this).val('');
            }
        }
    });

    $(".se-pre-con").hide();
    $(".se-pre-con2").hide();

    $(document).ajaxStart(function () {
    }).ajaxSuccess(function () {
    }).ajaxError(function (event, xhr, options, exc) {
        if (exc == "abort") { }
        else {
            if (xhr.status == 0)
                alertError("The connection having problems!");
            else if (xhr.status == 404)
                alertError("Error message : " + xhr.responseJSON.Message);
            else if (xhr.status == 401)
                alertError("Error message : " + "Your session expired due to inactivity." + "<br/>" + "Please refresh your browser and login to system again.");
            else if (!isEmpty(xhr.responseJSON))
                alertError("Error message : " + xhr.responseJSON.ExceptionMessage);
            else
                alertError("Error message : " + xhr.status + ' ' + xhr.statusText);
            //alertError("There was a global ajax error!");
        }
    });

    $.extend($.fn.dataTable.defaults, {
        language: {
            "processing": "Please wait. Loading....."
        },
    });

    $(document).tooltip({
        content: function () {
            return $(this).prop('title');
        },
        items: ":not(.select2-selection__rendered)",
        position: {
            my: "center bottom", // the "anchor point" in the tooltip element
            at: "right+5 top-5", // the position of that anchor point relative to selected element
        }
    });
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        //"positionClass": "toast-top-right",
        "positionClass": "toast-container",
        "preventDuplicates": false,
        "onclick": function () { alertDialog($('.toast-message').text()); },
        "showDuration": "400",
        "hideDuration": "1400",
        "timeOut": "4000",
        "extendedTimeOut": "1400",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    $(".cls_btn_close ").click(function () {
        document.location.href = "/";
    });

    //disable the warning message
    $.fn.dataTable.ext.errMode = 'none';

    $(document).click(function () {
        $('.ui-tooltip').remove();
    });

    $('.modal').on('show.bs.modal', function () {
        $("body").css('overflow', 'hidden');
    }).on("hidden.bs.modal", function () {
        if ($('.modal-backdrop').length == 0) {
            $("body").css('overflow', 'auto');
        }
    });

    $('.modal').on('shown.bs.modal', function () {
        var i = 1;
        $(".modal-backdrop").each(function (index) {
            if (i == 1) { }
            else if (i == 2) { $(this).css('z-index', '1410'); }
            else
                $(this).css('z-index', '1510');
            i++;
        });
    });

    $.validator.messages.required = '';

    $.fn.dataTable.ext.order['dom-checkbox'] = function (settings, col) {
        return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
            return $('input', td).prop('checked') ? '1' : '0';
        });
    }

    $("body").fadeIn();
});

function dtSuccess(res, callback) {
    res = DData(res);
    if (res.status == "E") {
        $('.dataTables_processing').hide();
        if (res.msg != '')
            alertError(res.msg);
    }
    else if (res.status == "S") {
        callback(res);
    }
}

function isValidDate(dateString) {
    // First check for the pattern
    var regex_date = /^\d{1,2}\/\d{1,2}\/\d{4}$/;

    if (!regex_date.test(dateString)) {
        return false;
    }

    // Parse the date parts to integers
    var parts = dateString.split("/");
    var day = parseInt(parts[0], 10);
    var month = parseInt(parts[1], 10);
    var year = parseInt(parts[2], 10);

    // Check the ranges of month and year
    if (year < 1000 || year > 3000 || month == 0 || month > 12) {
        return false;
    }

    var monthLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    // Adjust for leap years
    if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0)) {
        monthLength[1] = 29;
    }

    // Check the range of the day
    return day > 0 && day <= monthLength[month - 1];
}

function replaceHtml(mystring) {
    if (isEmpty(mystring))
        return "";
    else
        return mystring.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;").replace(/"/g, "&quot;");
}

function removeHtml(mystring) {
    if (isEmpty(mystring))
        return "";
    else
        return mystring.replace(/(<([^>]+)>)/ig, "   ");;
}
function removeHtmlToComma(mystring) {
    if (isEmpty(mystring))
        return "";
    else
        return mystring.replace(/(<([^>]+)>)/ig, ",");;
}


function myDateTimeMoment(date) {
    if (!isEmpty(date)) {
        if (moment(date).format('DD/MM/YYYY') == '01/01/0001')
            return "";
        else
            return moment(date).format('DD/MM/YYYY HH:mm:ss');
    }
    else
        return "";
}

//function myDateTimeMoment(date) {
//    if (!isEmpty(date))
//        return moment(date).format('DD/MM/YYYY HH:mm:ss');
//    else
//        return "";
//}

function myDateMoment(date) {
    if (!isEmpty(date))
        if (moment(date).format('DD/MM/YYYY') == '01/01/0001')
            return "";
        else
            return moment(date).format('DD/MM/YYYY');
    else
        return "";
}

function myAjaxConfirmSubmitBlank(callback, type) {

    $.confirm({
        title: 'Confirm Dialog',
        content: function () {
            if (type != null)
                return 'Do you want to submit? <br> <div style="color: red;font-style: italic;">**Please review visibility in Attachment tab before submit</div>';
            else
                return 'Do you want to submit?';
        },
        //content: 'Do you want to submit?',
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
                    if (typeof callback === 'function')
                        callback();
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

function myAjaxConfirmSubmitWithContent(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, content_) {
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;
    $.confirm({
        title: 'Confirm Dialog',
        content: content_,
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
                    myAjax(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg)
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

function myAjaxConfirmSubmit(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother) {
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;
    if (reasonother == null) reasonother = false;
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
                    myAjax(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother)
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

//function myAjaxConfirmSubmitNoSync(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother) {
//    if (hide_loading == null) hide_loading = true;
//    if (show_msg == null) show_msg = true;
//    if (reasonother == null) reasonother = false;

//    myAjaxWait(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother);

//}

function myAjaxConfirmSaveAndSendInfo(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother) {
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;
    if (reasonother == null) reasonother = false;

    $.confirm({
        title: 'Confirm Dialog',
        content: 'Please confirm to update information.',
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
                    myAjax(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother)
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

function myAjaxConfirmSubmitAlert(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg) {
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;
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
                    myAjaxAlert(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg)
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

function myAjaxConfirmDelete(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg) {
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;
    $.confirm({
        title: 'Confirm Dialog',
        content: 'Do you want to delete?',
        animation: 'none',
        closeAnimation: 'none',
        type: 'red',
        backgroundDismiss: false,
        backgroundDismissAnimation: 'glow',
        buttons: {
            Yes: {
                text: 'Yes',
                btnClass: 'btn-danger cls_btn_confirm_ok',
                action: function () {
                    myAjax(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg)
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

function myAjaxConfirmDeleteWithContent(myurl, mytype, mydata, callback, content_msg, callbackerror, hide_loading, show_msg) {
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;
    $.confirm({
        title: 'Confirm Dialog',
        content: content_msg,
        animation: 'none',
        closeAnimation: 'none',
        type: 'red',
        backgroundDismiss: false,
        backgroundDismissAnimation: 'glow',
        buttons: {
            Yes: {
                text: 'Yes',
                btnClass: 'btn-danger cls_btn_confirm_ok',
                action: function () {
                    myAjax(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg)
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
 
var timeoutHandle = null;
function myAjax(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother) {
    var tempData = injectData(mydata, mytype);
    var tempDataRemark = injectDataRemark(mydata, mytype);
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;
    if (reasonother == null) reasonother = false;

    if (mytype.toUpperCase() == "GET" || mytype.toUpperCase() == "POST2") {
        if (mytype.toUpperCase() == "POST2")
            mytype = "POST";

        if (timeoutHandle == null)
            timeoutHandle = setTimeout(function () {
                if ($('.se-pre-con2').is(':visible')) { }
                else {
                    $(".se-pre-con2").show();
                }
            }, 700);
    }
    else {
        $(".se-pre-con").show();
    }

    $.ajax({
        url: suburl + myurl,
        type: mytype,
        contentType: 'application/json; charset=utf-8',
        data: tempData,
        success: function (res) {
            res = DData(res);
            if (res.status == "S") {
                if (reasonother) {
                    if (typeof MOCKUPSUBID != 'undefined') {
                        $.ajax({
                            url: suburl + "/api/taskform/remarkreasonmockup/info",
                            type: mytype,
                            contentType: 'application/json; charset=utf-8',
                            data: tempDataRemark,
                        });
                    }
                    if (typeof ArtworkSubId != 'undefined') {
                        $.ajax({
                            url: suburl + "/api/taskform/remarkreasonaw/info",
                            type: mytype,
                            contentType: 'application/json; charset=utf-8',
                            data: tempDataRemark,
                        });
                    }
                }
            }

            if (hide_loading) {
                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }
            }
            if (res.status == "E") {

                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }
                //debugger;
                if (res.msg != '') {
                    //var matStatus = $('.cls_txt_header_tfartwork_mat_status').val();
                    //DisplayProgressMessage(false);
                    alertError(res.msg);
                }
                if (typeof callbackerror === 'function')
                    callbackerror(res);
            }
            else if (res.status == "S") {
                if (show_msg) {
                    if (res.msg != '')
                        alertSuccess(res.msg);
                }
                if (typeof callback === 'function')
                    callback(res);
            }
        },
        error: function (error) {
            $(".se-pre-con").fadeOut('fast');
            $(".se-pre-con2").fadeOut('fast');
            if (timeoutHandle != null) {
                clearTimeout(timeoutHandle);
                timeoutHandle = null;
            }

            //alertError(error.status + ' ' + error.statusText);
        }
    });
}

//function myAjaxWait(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother) {
//    var tempData = injectData(mydata, mytype);
//    var tempDataRemark = injectDataRemark(mydata, mytype);
//    if (hide_loading == null) hide_loading = true;
//    if (show_msg == null) show_msg = true;
//    if (reasonother == null) reasonother = false;

//    if (mytype.toUpperCase() == "GET" || mytype.toUpperCase() == "POST2") {
//        if (mytype.toUpperCase() == "POST2")
//            mytype = "POST";

//        if (timeoutHandle == null)
//            timeoutHandle = setTimeout(function () {
//                if ($('.se-pre-con2').is(':visible')) { }
//                else {
//                    $(".se-pre-con2").show();
//                }
//            }, 700);
//    }
//    else {
//        $(".se-pre-con").show();

//    }

//    $.ajax({
//        url: suburl + myurl,
//        type: mytype,
//        contentType: 'application/json; charset=utf-8',
//        data: tempData,
//        //async: false,
//        success: function (res) {
//            res = DData(res);
//            if (res.status == "S") {
//                if (reasonother) {
//                    if (typeof MOCKUPSUBID != 'undefined') {
//                        $.ajax({
//                            url: suburl + "/api/taskform/remarkreasonmockup/info",
//                            type: mytype,
//                            contentType: 'application/json; charset=utf-8',
//                            data: tempDataRemark,
//                        });
//                    }
//                    if (typeof ArtworkSubId != 'undefined') {
//                        $.ajax({
//                            url: suburl + "/api/taskform/remarkreasonaw/info",
//                            type: mytype,
//                            contentType: 'application/json; charset=utf-8',
//                            data: tempDataRemark,
//                        });
//                    }
//                }
//            }


//            if (hide_loading) {
//                $(".se-pre-con").fadeOut('fast');
//                $(".se-pre-con2").fadeOut('fast');
//                if (timeoutHandle != null) {
//                    clearTimeout(timeoutHandle);
//                    timeoutHandle = null;
//                }
//            }
//            if (res.status == "E") {

//                $(".se-pre-con").fadeOut('fast');
//                $(".se-pre-con2").fadeOut('fast');
//                if (timeoutHandle != null) {
//                    clearTimeout(timeoutHandle);
//                    timeoutHandle = null;
//                }

//                if (res.msg != '')
//                    alertError(res.msg);
//                if (typeof callbackerror === 'function')
//                    callbackerror(res);
//            }
//            else if (res.status == "S") {
//                if (show_msg) {
//                    if (res.msg != '')
//                        alertSuccess(res.msg);
//                }
//                if (typeof callback === 'function')
//                    callback(res);
//            }
//        },
//        error: function (error) {
//            $(".se-pre-con").fadeOut('fast');
//            $(".se-pre-con2").fadeOut('fast');
//            if (timeoutHandle != null) {
//                clearTimeout(timeoutHandle);
//                timeoutHandle = null;
//            }

//            //alertError(error.status + ' ' + error.statusText);
//        }
//    });
//}

function myAjaxNoAlertSuccess(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg, reasonother) {
    var tempData = injectData(mydata, mytype);
    var tempDataRemark = injectDataRemark(mydata, mytype);
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;
    if (reasonother == null) reasonother = false;

    if (mytype.toUpperCase() == "GET" || mytype.toUpperCase() == "POST2") {
        if (mytype.toUpperCase() == "POST2")
            mytype = "POST";

        if (timeoutHandle == null)
            timeoutHandle = setTimeout(function () {
                if ($('.se-pre-con2').is(':visible')) { }
                else {
                    $(".se-pre-con2").show();
                }
            }, 700);
    }
    else {
        $(".se-pre-con").show();
    }

    $.ajax({
        url: suburl + myurl,
        type: mytype,
        contentType: 'application/json; charset=utf-8',
        data: tempData,
        success: function (res) {
            if (res.status == "S") {
                if (reasonother) {
                    if (typeof MOCKUPSUBID != 'undefined') {
                        $.ajax({
                            url: suburl + "/api/taskform/remarkreasonmockup/info",
                            type: mytype,
                            contentType: 'application/json; charset=utf-8',
                            data: tempDataRemark,
                        });
                    }
                    if (typeof ArtworkSubId != 'undefined') {
                        $.ajax({
                            url: suburl + "/api/taskform/remarkreasonaw/info",
                            type: mytype,
                            contentType: 'application/json; charset=utf-8',
                            data: tempDataRemark,
                        });
                    }
                }
            }
            res = DData(res);
            if (hide_loading) {
                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }
            }
            if (res.status == "E") {

                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }

                if (res.msg != '')
                    alertError(res.msg);
                if (typeof callbackerror === 'function')
                    callbackerror(res);
            }
            else if (res.status == "S") {

                if (typeof callback === 'function')
                    callback(res);
            }
        },
        error: function (error) {
            $(".se-pre-con").fadeOut('fast');
            $(".se-pre-con2").fadeOut('fast');
            if (timeoutHandle != null) {
                clearTimeout(timeoutHandle);
                timeoutHandle = null;
            }

            //alertError(error.status + ' ' + error.statusText);
        }
    });
}

function myAjaxAlert(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg) {
    var tempData = injectData(mydata, mytype);
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;

    if (mytype.toUpperCase() == "GET") {
        if (timeoutHandle == null)
            timeoutHandle = setTimeout(function () {
                if ($('.se-pre-con2').is(':visible')) { }
                else {
                    $(".se-pre-con2").show();
                }
            }, 700);
    }
    else {
        $(".se-pre-con").show();
    }

    $.ajax({
        url: suburl + myurl,
        type: mytype,
        contentType: 'application/json; charset=utf-8',
        data: tempData,
        success: function (res) {
            res = DData(res);
            if (hide_loading) {
                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }
            }
            if (res.status == "E") {

                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }

                if (res.msg != '')
                    alertError(res.msg);
                if (typeof callbackerror === 'function')
                    callbackerror(res);
            }
            else if (res.status == "S") {
                if (show_msg) {
                    if (res.msg != '')
                        alertSuccessDialog(res.msg);
                }
                if (typeof callback === 'function')
                    callback(res);
            }
        },
        error: function (error) {
            $(".se-pre-con").fadeOut('fast');
            $(".se-pre-con2").fadeOut('fast');
            if (timeoutHandle != null) {
                clearTimeout(timeoutHandle);
                timeoutHandle = null;
            }

            //alertError(error.status + ' ' + error.statusText);
        }
    });
}

function myAjaxAlertNoBack(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg) {
    var tempData = injectData(mydata, mytype);
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;

    if (mytype.toUpperCase() == "GET") {
        if (timeoutHandle == null)
            timeoutHandle = setTimeout(function () {
                if ($('.se-pre-con2').is(':visible')) { }
                else {
                    $(".se-pre-con2").show();
                }
            }, 700);
    }
    else {
        $(".se-pre-con").show();
    }

    $.ajax({
        url: suburl + myurl,
        type: mytype,
        contentType: 'application/json; charset=utf-8',
        data: tempData,
        success: function (res) {
            res = DData(res);
            if (hide_loading) {
                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }
            }
            if (res.status == "E") {

                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }

                if (res.msg != '')
                    alertError(res.msg);
                if (typeof callbackerror === 'function')
                    callbackerror(res);
            }
            else if (res.status == "S") {
                if (show_msg) {
                    if (res.msg != '')
                        alertSuccessDialogNoBack(res.msg);
                }
                if (typeof callback === 'function')
                    callback(res);
            }
        },
        error: function (error) {
            $(".se-pre-con").fadeOut('fast');
            $(".se-pre-con2").fadeOut('fast');
            if (timeoutHandle != null) {
                clearTimeout(timeoutHandle);
                timeoutHandle = null;
            }

            //alertError(error.status + ' ' + error.statusText);
        }
    });
}

function myAjaxNoSync(myurl, mytype, mydata, callback, callbackerror) {
    var tempData = injectData(mydata, mytype);
    $.ajax({
        url: suburl + myurl,
        type: mytype,
        contentType: 'application/json; charset=utf-8',
        data: tempData,
        async: false,
        success: function (res) {
            res = DData(res);
            if (res.status == "E") {
                if (res.msg != '')
                    alertError(res.msg);
                if (typeof callbackerror === 'function')
                    callbackerror(res);
            }
            else if (res.status == "S") {
                if (res.msg != '')
                    alertSuccess(res.msg);
                if (typeof callback === 'function')
                    callback(res);
            }
        },
        error: function (error) {
            //alertError(error.status + ' ' + error.statusText);
        }
    });
}

function myAjaxAlertError2(myurl, mytype, mydata, callback, callbackerror, hide_loading, show_msg) {
    var tempData = injectData(mydata, mytype);
    if (hide_loading == null) hide_loading = true;
    if (show_msg == null) show_msg = true;

    if (mytype.toUpperCase() == "GET") {
        if (timeoutHandle == null)
            timeoutHandle = setTimeout(function () {
                if ($('.se-pre-con2').is(':visible')) { }
                else {
                    $(".se-pre-con2").show();
                }
            }, 700);
    }
    else {
        $(".se-pre-con").show();
    }

    $.ajax({
        url: suburl + myurl,
        type: mytype,
        contentType: 'application/json; charset=utf-8',
        data: tempData,
        success: function (res) {
            res = DData(res);
            if (hide_loading) {
                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }
            }
            if (res.status == "E") {

                $(".se-pre-con").fadeOut('fast');
                $(".se-pre-con2").fadeOut('fast');
                if (timeoutHandle != null) {
                    clearTimeout(timeoutHandle);
                    timeoutHandle = null;
                }

                if (res.msg != '')
                    alertError2(res.msg);
                if (typeof callbackerror === 'function')
                    callbackerror(res);
            }
            else if (res.status == "S") {
                if (show_msg) {
                    if (res.msg != '')
                        alertSuccess(res.msg);
                }
                if (typeof callback === 'function')
                    callback(res);
            }
        },
        error: function (error) {
            $(".se-pre-con").fadeOut('fast');
            $(".se-pre-con2").fadeOut('fast');
            if (timeoutHandle != null) {
                clearTimeout(timeoutHandle);
                timeoutHandle = null;
            }

            //alertError(error.status + ' ' + error.statusText);
        }
    });
}

function injectData(mydata, mytype) {
    if (mydata != null) {
        if (typeof MOCKUPSUBID != 'undefined')
            if (!isEmpty(MOCKUPSUBID))
                if (mytype.toUpperCase() != "GET")
                    mydata["MOCKUP_SUB_ID_CHECK"] = MOCKUPSUBID;

        if (typeof ArtworkSubId != 'undefined')
            if (!isEmpty(ArtworkSubId))
                if (mytype.toUpperCase() != "GET")
                    mydata["ARTWORK_SUB_ID_CHECK"] = ArtworkSubId;

        tempData = JSON.stringify(mydata);
        return tempData;
    }
}

function injectDataRemark(mydata, mytype) {
    if (mydata != null) {
        if (typeof MOCKUPSUBID != 'undefined')
            if (!isEmpty(MOCKUPSUBID))
                if (mytype.toUpperCase() != "GET")
                    mydata["MOCKUP_SUB_ID_CHECK"] = 0;

        if (typeof ArtworkSubId != 'undefined')
            if (!isEmpty(ArtworkSubId))
                if (mytype.toUpperCase() != "GET")
                    mydata["ARTWORK_SUB_ID_CHECK"] = 0;

        tempData = JSON.stringify(mydata);
        return tempData;
    }
}

function GetCurrentDateWithoutTime() {
    var d = new Date();
    d.setHours(0);
    d.setMinutes(0);
    d.setSeconds(0);
    d.setMilliseconds(0);
    return d;
}

function GetCurrentDate() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }
    today = yyyy + '-' + mm + '-' + dd;
    //today = dd + '/' + mm + '/' + yyyy;
    return today;
}

function GetCurrentDate2() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }
    //today = yyyy + '-' + mm + '-' + dd;
    today = dd + '/' + mm + '/' + yyyy;
    return today;
}

function GetFirstDateOfMonth() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }
    //today = yyyy + '-' + mm + '-' + dd;
    today = '01' + '/' + mm + '/' + yyyy;
    return today;
}

function GetLastDateOfMonth() {
    var today = new Date();

    today = new Date((new Date(today.getFullYear(), today.getMonth() + 1, 1)) - 1);
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }
    //today = yyyy + '-' + mm + '-' + dd;
    today = dd + '/' + mm + '/' + yyyy;
    return today;
}

function GetPreviousDate(num) {
    var today = new Date();
    today.setDate(today.getDate() - num);
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }
    //today = yyyy + '-' + mm + '-' + dd;
    today = dd + '/' + mm + '/' + yyyy;
    return today;
}

function GetNextDate(num) {
    var today = new Date();
    today.setDate(today.getDate() + num);
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }
    //today = yyyy + '-' + mm + '-' + dd;
    today = dd + '/' + mm + '/' + yyyy;
    return today;
}

function alertError(msg) {
    $.dialog({
        closeIcon: true,
        //closeIconClass: 'fa fa-close',
        title: 'Encountered an error!',
        content: msg,
        type: 'red',
        animation: 'none',
        closeAnimation: 'none',
        backgroundDismiss: true,
        //backgroundDismissAnimation: 'glow',
    });
}

function alertDialog(msg) {
    $.dialog({
        title: 'Message Dialog.',
        content: msg,

        closeIcon: true,


        type: 'dark',
        animation: 'none',
        closeAnimation: 'none',
        backgroundDismiss: true,


    });
}

function alertConfirmDialog(msg,callback) {

    $.confirm({
        title: 'Confirm Dialog',
        content: msg,
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
                    callback
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

function alertSuccessDialog(msg) {
    $.confirm({
        //closeIcon: true,
        //closeIconClass: 'fa fa-close',
        title: 'Completed',
        content: msg,
        type: 'green',
        animation: 'none',
        closeAnimation: 'none',
        backgroundDismiss: true,
        //backgroundDismissAnimation: 'glow',
        buttons: {
            Close: function () {
                tohomepage();
            },
        },
        onClose: function () {
            tohomepage();
        },
    });
}

function alertSuccessDialogNoBack(msg) {
    $.confirm({
        title: 'Completed',
        content: msg,
        type: 'green',
        animation: 'none',
        closeAnimation: 'none',
        backgroundDismiss: true,
        //backgroundDismissAnimation: 'glow',
        buttons: {
            Close: function () {
                //tohomepage();
            },
        },
        onClose: function () {
            //tohomepage();
        },
    });
}

function alertError2(msg) {
    toastr.error(msg);
}

function alertSuccess(msg) {
    toastr.success(msg);
}

function setValueToInputOther(obj, val, txt) {
    if (val == -1)
        $(obj).val('- - - Other - - -');
    else if (val == 0) { }
    else
        $(obj).val(txt);
}

function setValueToDDL(obj, val, txt) {
    if (val != null) {
        $(obj).empty();
        if (val == -1)
            $(obj).append('<option value="' + '-1' + '">' + '- - - Other - - -' + '</option>');
        else if (val == 0) { }
        else if (txt != null) {
            txt = txt.replace(/</g, "&lt;").replace(/>/g, "&gt;");
            $(obj).append('<option value="' + val + '">' + txt + '</option>');
        }

        $(obj).val(val);
    }
}

function setValueToDDLOther(obj, txt) {
    if (txt != null && txt != '') {
        $(obj).show();
        $(obj).closest('.row').show();
        $(obj).val(txt);
        $(obj).attr("required", true);
    }
}

Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}

function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}

function isEmpty(str) {

    if (str == null) {
        return true;
    }
    if (!str) {
        return true;
    }
    return false;
}

$.fn.dataTable.ext.order['dom-text-numeric'] = function (settings, col) {
    return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
        return Number($('input', td).val().replace(/[^0-9\.-]+/g, "")) * 1;
    });
}

function sortNumbersIgnoreText(a, b, high) {
    var reg = /[+-]?((\d+(\.\d*)?)|\.\d+)([eE][+-]?[0-9]+)?/;

    a = a.replace(/[^\d\-\.]/g, "");
    b = b.replace(/[^\d\-\.]/g, "");

    a = a.match(reg);
    a = a !== null ? parseFloat(a[0]) : high;
    b = b.match(reg);
    b = b !== null ? parseFloat(b[0]) : high;
    return ((a < b) ? -1 : ((a > b) ? 1 : 0));
}
jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "sort-numbers-ignore-text-asc": function (a, b) {
        return sortNumbersIgnoreText(a, b, Number.POSITIVE_INFINITY);
    },
    "sort-numbers-ignore-text-desc": function (a, b) {
        return sortNumbersIgnoreText(a, b, Number.NEGATIVE_INFINITY) * -1;
    }
});

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "date-uk-pre": function (a) {
        if (isEmpty(a)) { return false; }
        else {
            if (isEmpty(a.split(' ')[1])) {
                var ukDatea = a.split(' ')[0].split('/');
                return (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
            }
            else {
                var ukDatea = a.split(' ')[0].split('/');
                var tempTime = a.split(' ')[1].split(':');
                return (ukDatea[2] + ukDatea[1] + ukDatea[0] + tempTime[0] + tempTime[1] + tempTime[2]) * 1;
            }
        }
    },

    "date-uk-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },

    "date-uk-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});


function bindTooltip(row) {
    $(row).find('.cls_td_tooltip').each(function (index) {
        $(this).attr('title', $(this).html());
    });
}

function setCurrency2(txt) {
    return (txt).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

function setCurrency0(txt) {
    return (txt).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function removeComma(txt) {
    if (isEmpty(txt)) return "";
    else return txt.replace(/,/g, '');
}

function tohomepage() {
    setTimeout(function () {
        document.location.href = suburl + "/";
    }, 1000);
}

function copyToClipboard(elem) {
    var el = document.getElementById(elem);
    var range = document.createRange();
    range.selectNodeContents(el);
    var sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);
    document.execCommand('copy');
    alertSuccess("Contents copied to clipboard.");
    return false;
}

function find_duplicates(arr) {
    var len = arr.length,
        out = [],
        counts = {};

    for (var i = 0; i < len; i++) {
        var item = arr[i];
        counts[item] = counts[item] >= 1 ? counts[item] + 1 : 1;
        if (counts[item] === 2) {
            out.push(item);
        }
    }
    return out;
}

function GetRequestParam(param) {
    var res = null;
    try {
        var qs = decodeURIComponent(window.location.search.substring(1));//get everything after then '?' in URI
        var ar = qs.split('&');
        $.each(ar, function (a, b) {
            var kv = b.split('=');
            if (param === kv[0]) {
                res = kv[1];
                return false;//break loop
            }
        });
    } catch (e) { }
    return res;
}

function GetYMD(mystring) {
    //from format dd/mm/yyyy return yyyy-mm-dd
    var arr_value = mystring.split('/');
    var str_date = "";
    if (arr_value.length > 2) {
        if (arr_value[2] != "") {
            str_date = arr_value[2];
        }
        if (arr_value[1] != "") {
            if (str_date.length > 0) {
                str_date += '-' + arr_value[1];
            }
            else {
                str_date = arr_value[1];
            }
        }
        if (arr_value[0] != "") {
            if (str_date.length > 0) {
                str_date += '-' + arr_value[0];
            }
            else {
                str_date = arr_value[0];
            }
        }
    }
    else if (arr_value.length > 1) {
        if (arr_value[1] != "") {
            str_date = arr_value[1];
        }
        if (arr_value[0] != "") {
            if (str_date.length > 0) {
                str_date += '-' + arr_value[0];
            }
            else {
                str_date = arr_value[0];
            }
        }
    } else if (arr_value.length > 0) {
        str_date = arr_value[0];
    }
    return str_date;
}

var myPleaseWait;
myPleaseWait = myPleaseWait || (function () {
    var pleaseWaitDiv = $('<div class="modal hide" id="pleaseWaitDialog" data-backdrop="static" data-keyboard="false"><div class="modal-header"><h1>Processing...</h1></div><div class="modal-body"></div></div>');
    return {
        show: function () {
            pleaseWaitDiv.modal();
        },
        hide: function () {
            pleaseWaitDiv.modal('hide');
        },

    };
})();

String.prototype.format = function () {
    a = this;
    for (k in arguments) {
        a = a.replace("{" + k + "}", arguments[k])
    }
    return a
}

function DData(res) {
    if (EJ == "TRUE") {
        var position = 11;
        var temp = res.str.substring(0, position - 1) + res.str.substring(position, res.str.length);
        return jQuery.parseJSON(b64DecodeUnicode(temp));
    }
    else {
        return res;
    }
}

function b64DecodeUnicode(str) {
    // Going backwards: from bytestream, to percent-encoding, to original string.
    return decodeURIComponent(atob(str).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
}