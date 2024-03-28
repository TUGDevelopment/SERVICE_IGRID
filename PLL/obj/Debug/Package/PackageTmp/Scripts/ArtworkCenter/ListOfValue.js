$(document).ready(function () {
});
$.fn.modal.Constructor.prototype.enforceFocus = function () { }; 
function bind_lov(obj_name, api_url, search_name, param_input_other, callback_selelct_lov) {
    $(obj_name).select2({
        dropdownAutoWidth: true,
        //templateResult: function (data) {
        //    if (data.text.indexOf(':') >= 0) {
        //        var r = data.text.split(':');
        //        var $result = $(
        //            '<div class="row" style="padding:0px;margin:0px;">' +
        //            '<div class="col-md-3" style="padding-left:0px;">' + r[0] + '</div>' +
        //            '<div class="col-md-9">' + r[1] + '</div>' +
        //            '</div>'
        //        );
        //        return $result;
        //    }
        //    else {
        //        var r = data.text;
        //        var $result = $(
        //            '<div class="row" style="padding:0px;margin:0px;">' +
        //            '<div class="col-md-12" style="padding-left:0px;">' + r + '</div>' +
        //            '</div>'
        //        );
        //        return $result;
        //    }
        //},
        allowClear: true,
        placeholder: '',
        ajax: {
            url: suburl + api_url,
            dataType: 'json',
            delay: 500, // wait 250 milliseconds before triggering the request
            data: function (params) {
                //var obj = { page: params.page || 1 };//add paging// 
                var obj = {};
                if (params.term != "") {
                    obj[search_name] = params.term;
                }
                return obj;
            },
            processResults: function (res, params) {
                params.page = params.page || 1;//add paging
                var pageSize = 50;//add paging
                res = DData(res);
                if (res.status == 'S') {
                    if (!isEmpty(param_input_other) && (params.term == undefined || params.term == "")) {
                        res.data.splice(0, 0, { ID: "-1", DISPLAY_TXT: "- - - Other - - -" });
                    }
                    data = $.map(res.data, function (item) {
                        item.id = item.ID;
                        item.text = item.DISPLAY_TXT;
                        return item;
                    });
                    return {
                        results: data.slice((params.page - 1) * pageSize, params.page * pageSize),
                        //add paging
                        pagination: {
                            more: data.length >= params.page * pageSize
                        }
                    };
                }
                else if (res.status == "E") {
                    if (res.msg != '')
                        alertError(res.msg);
                }
            },
        }
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
        if (!isEmpty(param_input_other)) {
            $(param_input_other).hide();
            $(param_input_other).val('');
        }
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        //$('.select2-search__field').css("cssText", "font-size: 12px !important;");
        //$('.select2-search__field').css("font-style", "italic");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    $(obj_name).on('select2:select', function (e) {
        if (typeof callback_selelct_lov === 'function')
            callback_selelct_lov(obj_name);
    });


    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_nonOther(obj_name, api_url, search_name, param_input_other, callback_selelct_lov) {
    $(obj_name).select2({
        dropdownAutoWidth: true,

        allowClear: true,
        placeholder: '',
        ajax: {
            url: suburl + api_url,
            dataType: 'json',
            delay: 500, // wait 250 milliseconds before triggering the request
            data: function (params) {
                //var obj = { page: params.page || 1 };//add paging// 
                var obj = {};
                if (params.term != "") {
                    obj[search_name] = params.term;
                }
                return obj;
            },
            processResults: function (res, params) {
                params.page = params.page || 1;//add paging
                var pageSize = 50;//add paging
                res = DData(res);
                if (res.status == 'S') {

                    data = $.map(res.data, function (item) {
                        item.id = item.ID;
                        item.text = item.DISPLAY_TXT;
                        return item;
                    });
                    return {
                        results: data.slice((params.page - 1) * pageSize, params.page * pageSize),
                        //add paging
                        pagination: {
                            more: data.length >= params.page * pageSize
                        }
                    };
                }
                else if (res.status == "E") {
                    if (res.msg != '')
                        alertError(res.msg);
                }
            },
        }
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
        if (!isEmpty(param_input_other)) {
            $(param_input_other).hide();
            $(param_input_other).val('');
        }
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        //$('.select2-search__field').css("cssText", "font-size: 12px !important;");
        //$('.select2-search__field').css("font-style", "italic");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    $(obj_name).on('select2:select', function (e) {
        if (typeof callback_selelct_lov === 'function')
            callback_selelct_lov(obj_name);
    });


    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_confirm(obj_name, api_url, search_name, param_input_other, msg_confirm, callback_selelct_lov) {
    $(obj_name).select2({
        dropdownAutoWidth: true,
        allowClear: false,
        placeholder: '',
        ajax: {
            url: suburl + api_url,
            dataType: 'json',
            delay: 500, // wait 250 milliseconds before triggering the request
            data: function (params) {
                //var obj = { page: params.page || 1 };//add paging// 
                var obj = {};
                if (params.term != "") {
                    obj[search_name] = params.term;
                }
                return obj;
            },
            processResults: function (res, params) {
                params.page = params.page || 1;//add paging
                var pageSize = 50;//add paging
                res = DData(res);
                if (res.status == 'S') {
                    if (!isEmpty(param_input_other) && (params.term == undefined || params.term == "")) {
                        res.data.splice(0, 0, { ID: "-1", DISPLAY_TXT: "- - - Other - - -" });
                    }
                    data = $.map(res.data, function (item) {
                        item.id = item.ID;
                        item.text = item.DISPLAY_TXT;
                        return item;
                    });
                    return {
                        results: data.slice((params.page - 1) * pageSize, params.page * pageSize),
                        //add paging
                        pagination: {
                            more: data.length >= params.page * pageSize
                        }
                    };
                }
                else if (res.status == "E") {
                    if (res.msg != '')
                        alertError(res.msg);
                }
            },
        }
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
        if (!isEmpty(param_input_other)) {
            $(param_input_other).hide();
            $(param_input_other).val('');
        }
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        //$('.select2-search__field').css("cssText", "font-size: 12px !important;");
        //$('.select2-search__field').css("font-style", "italic");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    var is_select;
    $(obj_name).on('select2:selecting', function (e) {
        var curentValue = e.val;
        var previousValue = $(this).val();

        if (previousValue != null) {
            $.confirm({
                title: 'Confirm Dialog',
                content: msg_confirm,
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
                            if (typeof callback_selelct_lov === 'function')
                                callback_selelct_lov(obj_name);
                            is_select = false;
                        }
                    },
                    No: {
                        text: 'No',
                        btnClass: 'btn-default cls_btn_confirm_no',
                        action: function () {
                            $(obj_name).val(previousValue).trigger("change");
                            is_select = false;
                        }
                    }
                }
            });
        }
        else {
            is_select = true;
        }
    });

    $(obj_name).on('select2:select', function (e) {
        if (is_select) {
            if (typeof callback_selelct_lov === 'function')
                callback_selelct_lov(obj_name);
            is_select = false;
        }
    });

    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_template(obj_name, api_url, search_name, placeholder, param_input_other, callback_selelct_lov) {
    $(obj_name).select2({
        dropdownAutoWidth: true,
        allowClear: true,
        placeholder: placeholder,
        ajax: {
            url: suburl + api_url,
            dataType: 'json',
            delay: 500, // wait 250 milliseconds before triggering the request
            data: function (params) {
                //var obj = { page: params.page || 1 };//add paging// 
                var obj = {};
                if (params.term != "") {
                    obj[search_name] = params.term;
                }
                return obj;
            },
            processResults: function (res, params) {
                params.page = params.page || 1;//add paging
                var pageSize = 50;//add paging
                res = DData(res);
                if (res.status == 'S') {
                    if (!isEmpty(param_input_other) && (params.term == undefined || params.term == "")) {
                        res.data.splice(0, 0, { ID: "-1", DISPLAY_TXT: "- - - Other - - -" });
                    }
                    data = $.map(res.data, function (item) {
                        item.id = item.ID;
                        item.text = item.DISPLAY_TXT;
                        return item;
                    });
                    return {
                        results: data.slice((params.page - 1) * pageSize, params.page * pageSize),
                        //add paging
                        pagination: {
                            more: data.length >= params.page * pageSize
                        }
                    };
                }
                else if (res.status == "E") {
                    if (res.msg != '')
                        alertError(res.msg);
                }
            },
        }
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
        if (!isEmpty(param_input_other)) {
            $(param_input_other).hide();
            $(param_input_other).val('');
        }
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    $(obj_name).on('select2:select', function (e) {
        if (typeof callback_selelct_lov === 'function')
            callback_selelct_lov(obj_name);
    });


    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_no_ajax(obj_name, place_holder, param_input_other) {
    $(obj_name).select2({
        dropdownAutoWidth: true,
        allowClear: true,
        placeholder: place_holder
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_no_ajax_not_allow_clear(obj_name, place_holder, param_input_other) {
    $(obj_name).select2({
        dropdownAutoWidth: true,
        allowClear: false,
        placeholder: place_holder
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_param(obj_name, api_url, search_name, param_key, param_name, param_input_other, callback_selelct_lov) {
    //debugger;
    $(obj_name).select2({
        dropdownAutoWidth: true,
        allowClear: true,
        placeholder: '',
        ajax: {
            url: suburl + api_url,
            dataType: 'json',
            delay: 500, // wait 250 milliseconds before triggering the request
            data: function (params) {
                //var obj = { page: params.page || 1 };//add paging// 
                var obj = {};
                if (params.term != "" && params.term != undefined) {
                    obj[search_name] = params.term;
                }
                if (param_key.length == param_name.length) {
                    for (var i = 0; i < param_key.length; i++) {
                        var type = $(param_name[i]).attr("type");
                        if (type == "checkbox" || type == "radio") {
                            obj["data." + param_key[i]] = $(param_name[i]).is(":checked");
                        }
                        else {
                            if ($(param_name[i]).val() != null) {
                                obj["data." + param_key[i]] = $(param_name[i]).val();
                            }
                            //else {
                            //    return false;
                            //}
                        }
                    }
                }
                return obj;
            },
            processResults: function (res, params) {
                params.page = params.page || 1;//add paging
                var pageSize = 50;//add paging
                res = DData(res);
                if (res.status == 'S') {
                    if (!isEmpty(param_input_other) && (params.term == undefined || params.term == "")) {
                        res.data.splice(0, 0, { ID: "-1", DISPLAY_TXT: "- - - Other - - -" });
                    }
                    data = $.map(res.data, function (item) {
                        item.id = item.ID;
                        item.text = item.DISPLAY_TXT;
                        return item;
                    });
                    return {
                        results: data.slice((params.page - 1) * pageSize, params.page * pageSize),
                        //add paging
                        pagination: {
                            more: data.length >= params.page * pageSize
                        }
                    };
                }
                else if (res.status == "E") {
                    if (res.msg != '')
                        alertError(res.msg);
                }
            }
        }
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
        if (!isEmpty(param_input_other)) {
            $(param_input_other).hide();
            $(param_input_other).val('');
        }
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        //$('.select2-search__field').css("cssText", "font-size: 12px !important;");
        //$('.select2-search__field').css("font-style", "italic");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    $(obj_name).on('select2:select', function (e) {
        if (typeof callback_selelct_lov === 'function')
            callback_selelct_lov(obj_name);
    });


    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_param_filter(obj_name, api_url, search_name, param_key, param_name, filter_key, filter_name, param_input_other, callback_selelct_lov) {
    $(obj_name).select2({
        dropdownAutoWidth: true,
        allowClear: true,
        placeholder: '',
        ajax: {
            url: suburl + api_url,
            dataType: 'json',
            delay: 500, // wait 250 milliseconds before triggering the request
            data: function (params) {
                //var obj = { page: params.page || 1 };//add paging// 
                var obj = {};
                if (params.term != "") {
                    obj[search_name] = params.term;
                }
                if (param_key.length == param_name.length) {
                    for (var i = 0; i < param_key.length; i++) {
                        var type = $(param_name[i]).attr("type");
                        if (type == "checkbox" || type == "radio") {
                            obj["data." + param_key[i]] = $(param_name[i]).is(":checked");
                        }
                        else {
                            if ($(param_name[i]).length > 1) {
                                var str_val = "";
                                $(param_name[i]).each(function (index) {
                                    var v = $(this).val();
                                    if (v != null) {
                                        if (str_val != "") {
                                            str_val += "||" + v;
                                        }
                                        else {
                                            str_val = v;
                                        }
                                    }
                                });
                                obj["data." + param_key[i]] = str_val;
                            } else {
                                obj["data." + param_key[i]] = $(param_name[i]).val();
                            }
                        }
                    }
                }
                var str_val = "";
                for (var i = 0; i < filter_name.length; i++) {
                    var type = $(filter_name[i]).attr("type");
                    if (type == "checkbox" || type == "radio") {
                        obj["data." + filter_key] = $(filter_name[i]).is(":checked");
                    }
                    else {
                        if ($(filter_name[i]).length > 1) {

                            $(filter_name[i]).each(function (index) {
                                var v = $(this).val();
                                if (v != null) {
                                    if (str_val != "") {
                                        str_val += "||" + v;
                                    }
                                    else {
                                        str_val = v;
                                    }
                                }
                            });

                        } else {
                            str_val = $(filter_name[i]).val();
                        }
                    }
                }
                obj["data." + filter_key] = str_val;
                return obj;
            },
            processResults: function (res, params) {
                params.page = params.page || 1;//add paging
                var pageSize = 50;//add paging
                res = DData(res);
                if (res.status == 'S') {
                    if (!isEmpty(param_input_other) && (params.term == undefined || params.term == "")) {
                        res.data.splice(0, 0, { ID: "-1", DISPLAY_TXT: "- - - Other - - -" });
                    }
                    data = $.map(res.data, function (item) {
                        item.id = item.ID;
                        item.text = item.DISPLAY_TXT;
                        return item;
                    });
                    return {
                        results: data.slice((params.page - 1) * pageSize, params.page * pageSize),
                        //add paging
                        pagination: {
                            more: data.length >= params.page * pageSize
                        }
                    };
                }
                else if (res.status == "E") {
                    if (res.msg != '')
                        alertError(res.msg);
                }
            }
        }
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
        if (!isEmpty(param_input_other)) {
            $(param_input_other).hide();
            $(param_input_other).val('');
        }
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        //$('.select2-search__field').css("cssText", "font-size: 12px !important;");
        //$('.select2-search__field').css("font-style", "italic");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    $(obj_name).on('select2:select', function (e) {
        if (typeof callback_selelct_lov === 'function')
            callback_selelct_lov(obj_name);
    });


    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_param_filter_list(obj_name, api_url, search_name, param_key, param_name, filter_key, filter_name, param_input_other, callback_selelct_lov) {
    $(obj_name).select2({
        dropdownAutoWidth: true,
        allowClear: true,
        placeholder: '',
        ajax: {
            url: suburl + api_url,
            dataType: 'json',
            delay: 500, // wait 250 milliseconds before triggering the request
            data: function (params) {
                //var obj = { page: params.page || 1 };//add paging// 
                var obj = {};
                if (params.term != "") {
                    obj[search_name] = params.term;
                }

                for (var i = 0; i < param_name.length; i++) { 

                    if (i == 0) {
                        obj["data." + param_key[0]] =  $(param_name[i]).val();
                    }
                    else {
                        obj["data." + param_key[0]] += "||"+ $(param_name[i]).val() ;
                    }
                       
                }

                var str_val = "";
                for (var i = 0; i < filter_name.length; i++) {
                    var type = $(filter_name[i]).attr("type");
                    if (type == "checkbox" || type == "radio") {
                        obj["data." + filter_key] = $(filter_name[i]).is(":checked");
                    }
                    else {
                        if ($(filter_name[i]).length > 1) {

                            $(filter_name[i]).each(function (index) {
                                var v = $(this).val();
                                if (v != null) {
                                    if (str_val != "") {
                                        str_val += "||" + v;
                                    }
                                    else {
                                        str_val = v;
                                    }
                                }
                            });

                        } else {
                            str_val = $(filter_name[i]).val();
                        }
                    }
                }
                obj["data." + filter_key] = str_val;
                return obj;
            },
            processResults: function (res, params) {
                params.page = params.page || 1;//add paging
                var pageSize = 50;//add paging
                res = DData(res);
                if (res.status == 'S') {
                    if (!isEmpty(param_input_other) && (params.term == undefined || params.term == "")) {
                        res.data.splice(0, 0, { ID: "-1", DISPLAY_TXT: "- - - Other - - -" });
                    }
                    data = $.map(res.data, function (item) {
                        item.id = item.ID;
                        item.text = item.DISPLAY_TXT;
                        return item;
                    });
                    return {
                        results: data.slice((params.page - 1) * pageSize, params.page * pageSize),
                        //add paging
                        pagination: {
                            more: data.length >= params.page * pageSize
                        }
                    };
                }
                else if (res.status == "E") {
                    if (res.msg != '')
                        alertError(res.msg);
                }
            }
        }
    }).on("select2:unselecting", function (e) {
        $(this).data('unselecting', true);
        if (!isEmpty(param_input_other)) {
            $(param_input_other).hide();
            $(param_input_other).val('');
        }
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        //$('.select2-search__field').css("cssText", "font-size: 12px !important;");
        //$('.select2-search__field').css("font-style", "italic");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    $(obj_name).on('select2:select', function (e) {
        if (typeof callback_selelct_lov === 'function')
            callback_selelct_lov(obj_name);
    });


    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name).val() == '-1') {
                $(param_input_other).show();
                if ($(obj_name).attr('required')) {
                    $(param_input_other).attr("required", true);
                }
                $(param_input_other).focus();
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }
}

function bind_lov_reason(obj_name, api_url, search_name, param_input_other, callback_selelct_lov) {
    $(obj_name).select2({
        dropdownAutoWidth: true,
        ajax: {
            url: suburl + api_url,
            dataType: 'json',
            delay: 500, // wait 250 milliseconds before triggering the request
            data: function (params) {
                //var obj = { page: params.page || 1 };//add paging// 
                var obj = {};
                if (params.term != "") {
                    obj[search_name] = params.term;
                }
                return obj;
            },
            processResults: function (res, params) {
                params.page = params.page || 1;//add paging
                var pageSize = 50;//add paging
                res = DData(res);
                if (res.status == 'S') {
                    //if (!isEmpty(param_input_other) && (params.term == undefined || params.term == "")) {
                    //    res.data.splice(0, 0, { ID: "-1", DISPLAY_TXT: "- - - Other - - -"  });
                    //}
                    data = $.map(res.data, function (item) {
                        item.id = item.ID;
                        item.text = item.DISPLAY_TXT;
                        return item;
                    });
                    return {
                        results: data.slice((params.page - 1) * pageSize, params.page * pageSize),
                        //add paging
                        pagination: {
                            more: data.length >= params.page * pageSize
                        }
                    };
                }
                else if (res.status == "E") {
                    if (res.msg != '')
                        alertError(res.msg);
                }
            },
        },
    }).on('select2:open', function (e) {
        //$('.select2-search__field').attr("placeholder", "Type here to search");
        if ($(this).data('unselecting')) {
            $(this).select2('close').removeData('unselecting');
        }
    });

    if (!isEmpty(param_input_other)) {
        $(obj_name).on('select2:select', function (e) {
            if ($(obj_name + ' option:selected').text() == gOthers) {
                $(param_input_other).show();
                //if ($(obj_name).attr('required')) {
                //    $(param_input_other).attr("required", true);
                //}
                $(param_input_other).focus();
                $(param_input_other).attr("required", true);
            }
            else {
                $(param_input_other).hide();
                $(param_input_other).val('');
                $(param_input_other).attr("required", false);
            }
        });
    }

    var data = {
        id: DefaultResonId,
        text: DefaultResonTxt,
    };
    var newOption = new Option(data.text, data.id, true, true);
    $(obj_name).append(newOption).trigger('change');
}

function resetDllReason(obj_name) {
    var data = {
        id: DefaultResonId,
        text: DefaultResonTxt,
    };
    var newOption = new Option(data.text, data.id, true, true);
    $(obj_name).append(newOption).trigger('change');
}