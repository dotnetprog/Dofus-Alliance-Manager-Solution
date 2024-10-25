function showInfo(message) {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-full-width",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": 300,
        "hideDuration": 1000,
        "timeOut": 5000,
        "extendedTimeOut": 1000,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    toastr.info(message);
}
$(document).ready(function () {
    var name = $("#Name");
    var allianceidAttribute = $('#allianceid');

    $("#saveBtn").on("click", function (e) {
        var t = document.getElementsByClassName("needs-validation");
        Array.prototype.filter.call(t, function (e) {
            e.classList.add("was-validated");
        })
        var validationName = name.parsley("validate");
       
        if (!validationName.isValid()) {
            e.preventDefault();
            e.stopPropagation();
            return;
        }
        showInfo("Traitement en cours..")
        var payload = {
            Name: name.val()
        };

        $.ajax({
            contentType: "application/json",
            async: true,
            data: JSON.stringify(payload),
            url: `/api/Alliance/${allianceidAttribute.val()}/CreateEnemy`,
            type: "POST",
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function () {

                window.location.href = `/Alliance/${allianceidAttribute.val()}/Configuration`;
            },
            error: function (e) {

                console.error(e);
            }


        });

    });




});