$(document).ready(function () {
    if (!isNaN(UserID)) {
        var myurl = '/api/common/stepmockup';
        var mytype = 'GET';
        var mydata = null;
        myAjaxNoSync(myurl, mytype, mydata, callback_get_stepmockup);
    }

    if (!isNaN(UserID)) {
        var myurl = '/api/common/stepartwork';
        var mytype = 'GET';
        var mydata = null;
        myAjaxNoSync(myurl, mytype, mydata, callback_get_stepartwork);
    }
});

var STEPANDROLE = [];
function callback_get_stepmockup(res) {
    var jsonObj = [];

    $.each(res.data, function (index, item) {
        var s = {};
        s["step_code"] = item.STEP_MOCKUP_CODE;
        s["curr_step"] = item.STEP_MOCKUP_ID;
        s["curr_role"] = item.ROLE_ID_RESPONSE;
        jsonObj.push(s);
    });

    STEPANDROLE = jsonObj;
}

function getstepmockup(step_code) {
    var res =
        {
            "step_code": '',
            "curr_step": 0,
            "curr_role": 0,
        };

    var found = false;
    $.each(STEPANDROLE, function (index, item) {
        if (item.step_code == step_code) {
            found = true;
            res =
                {
                    "step_code": item.step_code,
                    "curr_step": item.curr_step,
                    "curr_role": item.curr_role,
                };
        }
    });

    if (found == false) {
        alertError("Please config step for this action.");
    }

    return res;
}

var STEPANDROLEARTWORK = [];
function callback_get_stepartwork(res) {
    var jsonObj = [];

    $.each(res.data, function (index, item) {
        var s = {};
        s["step_code"] = item.STEP_ARTWORK_CODE;
        s["curr_step"] = item.STEP_ARTWORK_ID;
        s["curr_role"] = item.ROLE_ID_RESPONSE;
        jsonObj.push(s);
    });

    STEPANDROLEARTWORK = jsonObj;
}

function getstepartwork(step_code) {
    var res =
    {
        "step_code": '',
        "curr_step": 0,
        "curr_role": 0,
    };

    var found = false;
    $.each(STEPANDROLEARTWORK, function (index, item) {
        if (item.step_code == step_code) {
            found = true;
            res =
                {
                    "step_code": item.step_code,
                    "curr_step": item.curr_step,
                    "curr_role": item.curr_role,
                };
        }
    });

    if (found == false) {
        alertError("Please config step for this action.");
    }

    return res;
}