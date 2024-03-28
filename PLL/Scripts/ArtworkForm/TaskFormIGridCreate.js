
$(document).ready(function () {
    var t = 100;


    $(".cls_form_igrid_create .cls_btn_igrid_create").click(function (e) {
     
        createDocument(true);
    });


});


function createDocument(is_showmsg)
{

    var jsonObj = new Object();
    jsonObj.data = [];
    var item = {};

    item['Id'] = 0;

    jsonObj.data = item;
    var myurl = '/api/taskform/igrid/info';
    var mytype = 'POST';
    var mydata = jsonObj;

    //myAjax(myurl, mytype, mydata, callbackCreateDocument, '', false, is_showmsg);
    myAjaxConfirmSubmit(myurl, mytype, mydata, callbackCreateDocument, '', false, is_showmsg);

  
}


function callbackCreateDocument(res) {
    //ARTWORK_SUB_PA_ID = res.data[0].ARTWORK_SUB_PA_ID;
    if (res.data != null) {
        var url = suburl + '/IGrid/' + res.data[0].Id;
       
        $(location).attr('href', url);
    }
  
}