// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal2 .modal-body').html(res);
            $('#form-modal2 .modal-title').html(title);
            $('#form-modal2').modal('show');
        }
    })
}

jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res2) {
                if (res2.isValid) {
                    $('#view-all').html(res2.html)
                    $('#form-modal2 .modal-body').html('');
                    $('#form-modal2 .modal-title').html('');
                    $('#form-modal2').modal('hide');
                }
                else
                    $('#form-modal2 .modal-body').html(res2.html);
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}
