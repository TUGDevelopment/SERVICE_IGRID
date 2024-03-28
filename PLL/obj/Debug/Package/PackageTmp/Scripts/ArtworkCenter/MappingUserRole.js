$(document).ready(function () {
    if (!isNaN(UserID)) {
        var myurl = '/api/master/userrole';
        var mytype = 'GET';
        var mydata = null;
        myAjaxNoSync(myurl, mytype, mydata, callback_get_roleid);
    }
});

var ROLEUSER = [];
function callback_get_roleid(res) {
    var jsonObj = [];

    $.each(res.data, function (index, item) {
        var s = {};
        s["role_code"] = item.ROLE_CODE;
        s["role_id"] = item.ROLE_ID;
        s["user_id"] = item.USER_ID;
        s["position_code"] = item.POSITION_CODE; //#TSK-1511 #SR-70695 by aof in 09/2022
        s["igrid_user_fn"] = item.IGRID_USER_FN;  //  IGRID REIM by aof in 08/2023
        jsonObj.push(s); 
    });

    ROLEUSER = jsonObj;
}
function getroleuser(role_code) {
    var foundrole = false;
    $.each(ROLEUSER, function (index, item) {
        if (item.role_code == role_code) {
            foundrole = true;
        }
    });

    return foundrole;
}


function getiGridUserFN(fn) {
    //  IGRID REIM by aof in 08/2023
    var foundrole = false;
    $.each(ROLEUSER, function (index, item) {
        if (item.role_code.indexOf(fn) > -1) {
            foundrole = true;
        }
    });

    return foundrole;
}


//#TSK-1511 #SR-70695 by aof in 09/2022
function isQCPosition(position_code) {
    var f_found = false;

    $.each(ROLEUSER, function (index, item) {
        if (item.position_code == position_code) {
            f_found = true;
        }
    });

    return f_found;
}