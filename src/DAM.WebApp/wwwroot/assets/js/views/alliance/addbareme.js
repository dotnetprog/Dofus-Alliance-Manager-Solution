///<reference path="../../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../../libs/jquery/jquery.min.js" />
///<reference path="../../../libs/toastr/build/toastr.min.js" />
///<reference path="../../../libs/parsleyjs/parsley.min.js" />
///<reference path="../../../libs/table-edits/build/table-edits.min.js" />


function getDataFromMatrix() {
    var inputs = $("table > tbody > tr > td > input");

    var data = [];

    for (var i = 0; i < inputs.length; i++) {

        var input = $(inputs[i]);
        var eCount = parseInt(input.attr("enemy"));
        var aCount = parseInt(input.attr("ally"));
        var pepites = parseInt(input.val());
        var detail = {
            EnemyCount: isNaN(eCount) ? 0 : eCount ,
            AllyCount: isNaN(aCount) ? 0 : aCount,
            NbPepites: isNaN(pepites) ? 0 : pepites
        };
        data.push(detail);

    }
    return data;


}
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

function loadMultiSelectAllianceEnemy() {
    $('#Enemies').select2({
        theme: "bootstrap-5",
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        closeOnSelect: false
    });
}
$(document).ready(function () {
    var name = $("#Name");
    var allianceidAttribute = $('#allianceid');
    loadMultiSelectAllianceEnemy();
    var type = $('#type');
    var typeParsley = type.parsley();
    var enemies = $('#Enemies');
    $("#saveBtn").on("click", function (e) {
        var t = document.getElementsByClassName("needs-validation");
        Array.prototype.filter.call(t, function (e) {
            e.classList.add("was-validated");
        })

        var validationName = name.parsley("validate");
        var validationType = type.parsley("validate");
        var matrixInstances = $("table > tbody > tr > td > input").parsley("validate");
        var invalidCellCount = matrixInstances.filter(m => !m.isValid());
        if (!validationName.isValid() || !validationType.isValid() || invalidCellCount.length > 0) {
            e.preventDefault();
            e.stopPropagation();
            return;
        }
        showInfo("Traitement en cours..")
        var payload = {
            Name: name.val(),
            Details: getDataFromMatrix(),
            BaremeType: parseInt(type.val()),
            Enemies: enemies.val()
        };

        $.ajax({
            contentType: "application/json",
            async: true,
            data: JSON.stringify(payload),
            url: `/api/Alliance/${allianceidAttribute.val()}/CreateBareme`,
            type: "POST",
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function () {

                window.location.href =  `/Alliance/${allianceidAttribute.val()}/Configuration`;
            },
            error: function (e) {

                console.error(e);
            }


        });

    });

    


});