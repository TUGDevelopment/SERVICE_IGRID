var toolbarOptions = [
    ['bold', 'italic', 'underline', 'strike'],          // toggled buttons
    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
    [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
    [{ 'color': [] }, { 'background': [] }],            // dropdown with defaults from theme
    [{ 'align': [] }],
    ['clean']                                         // remove formatting button
];

function bind_text_editor(obj_name) {
    if ($(obj_name).length > 0)
        var editor_remark = new Quill(obj_name, {
            modules: { toolbar: toolbarOptions },
            theme: 'snow'
        });

    return editor_remark;
}