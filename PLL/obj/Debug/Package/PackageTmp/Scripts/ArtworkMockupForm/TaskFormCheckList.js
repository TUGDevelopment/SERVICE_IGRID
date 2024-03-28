$(document).ready(function () {
    //bindDataCheckList(MOCKUPSUBID);
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        switch (target) {
            case '#view_check_list':
                break;
            default:
                break;
        }
    });
});
