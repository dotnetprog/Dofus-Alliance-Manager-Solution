
function displaySuccessWithMessage(message) {

    toastr.success(message);

}
$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
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
    };
    var allianceid = $('#allianceid').val();
    var fromDate = $("#From");
    var toDate = $("#To");
    var multiplier = $("#multiplier");
    var selectedBaremeAttaqueId = $("#SelectedBaremeAttaqueId");
    var selectedBaremeDefId = $("#SelectedBaremeDefenseId");
    let table = $(".datatable").DataTable({
      //  lengthChange: !1,
        buttons: ["copy",
            {
                "extend": 'excelHtml5',
                "text": 'Excel',
                "footer": true,
                customizeData: function (data) {
                    for (var i = 0; i < data.body.length; i++) {
                        data.body[i][0] = '\u200C' + data.body[i][0];
                       
                    }
                }
            },
            "csv",
            "pdf",
            "colvis",
            {
                text: 'Publish to discord',
                action: function (e, dt, node, config) {
                    if (dt.data().length === 0) {
                        alert("Aucune ligne à publier.")
                        return;
                    }
                    var fd = fromDate.data('datepicker');
                    var td = toDate.data('datepicker');
                    var baremeAtkId = selectedBaremeAttaqueId.val();
                    var baremeDefId = selectedBaremeDefId.val();
                    var multiplierValue = multiplier.val() || "1";
                    var payload = {
                        From: moment(fd.getDate()).format("YYYY-MM-DD") + "T00:00:00.000Z",
                        To: moment(td.getDate()).format("YYYY-MM-DD") + "T00:00:00.000Z",
                        BaremeAttaqueId: baremeAtkId || null,
                        BaremeDefenseId: baremeDefId || null,
                        Multiplier: parseFloat(multiplierValue)
                    };
                    console.log("publish to discord: ", payload);
                    publishReport(allianceid, payload);
                }
            }],
        ajax: function (d, cb) {
            console.log(d);
            var fd = fromDate.data('datepicker');
            var td = toDate.data('datepicker');
            var multiplierValue = multiplier.val() || "1";
            if (!fd || !td) {

                cb({data:[]});
                return;
            }
            if (!fd.getDate() || !td.getDate()) {
                cb({ data: [] });
                return;
            }
            getDataFromServer(allianceid, fd.getDate(), td.getDate(), parseFloat(multiplierValue), selectedBaremeAttaqueId.val(), selectedBaremeDefId.val(), cb);
           
        },
        "formatNumber": function (toFormat) {
            return toFormat.toLocaleString("en-US");
        },
        order: [[8, 'desc']],
        columns: [
            { data: 'discordId' },
            { data: 'username' },
            { data: 'nombre_defense' },
            { data: 'nombre_attaques' },
            { data: 'nombreParticipationAvA' },
            { data: 'montantDefPepites', render: $.fn.dataTable.render.number(',', '.', 2, '') },
            { data: "montantAtkPepites", render: $.fn.dataTable.render.number(',', '.', 2, '') },
            { data: "montantAvAPepites", render: $.fn.dataTable.render.number(',', '.', 2, '') },
            {
                data: "montantTotalPepites",
                render: $.fn.dataTable.render.number(',', '.', 2, '')
            }
        ],
    });
    table.buttons().container().appendTo("#datatable-buttons_wrapper .col-md-6:eq(0)");
    $(".dataTables_length select").addClass("form-select form-select-sm");

    $("#submit").on("click", (e) => { submitHandler(e, table); } );

});
function setReportAggregateInfo(data) {
    var sumPepiteDef = data.totalPepitesDef || 0;
    var sumPepiteAtk = data.totalPepitesAtk || 0;
    var sumPepiteAvA = data.totalPepitesAvA || 0;
    var sumPepiteTotal = data.totalPepites || 0;
    var average = data.moyennePepiteParJoueur || 0;
    $('#totalpepitesAtk').text(sumPepiteAtk.toLocaleString("en-US"));
    $('#totalpepitesDef').text(sumPepiteDef.toLocaleString("en-US"));
    $('#totalpepiteAvA').text(sumPepiteAvA.toLocaleString("en-US"));
    $('#averagepepite').text(average.toLocaleString("en-US", {
        maximumFractionDigits: 2
    }));
    $('#totalpepites').text((sumPepiteTotal).toLocaleString("en-US"));
}
function publishReport(allianceid, payload) {
    $.ajax({
        contentType: "application/json",
        async: true,
        data: JSON.stringify(payload),
        url: `/api/Alliance/${allianceid}/PublishReport`,
        type: "POST",
        beforeSend: function (xhr) {
            var token = window.JWTManager.getToken();
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            displaySuccessWithMessage("Publication du rapport réussi !");
          

        },
        error: function (e) {
            alert(e.responseText);
            console.error(e);
        }


    });
}
function getDataFromServer(allianceid, fromDate, toDate,multiplerValue,baremeAtkId,baremeDefId ,cb) {

    var from = moment(fromDate);
    var to = moment(toDate);
    var payload = {
        From: from.format("YYYY-MM-DD") + "T00:00:00.000Z",
        To: to.format("YYYY-MM-DD") + "T00:00:00.000Z",
        BaremeAttaqueId: baremeAtkId || null,
        BaremeDefenseId: baremeDefId || null,
        Multiplier: multiplerValue
    };

    $.ajax({
        contentType: "application/json",
        async: true,
        data: JSON.stringify(payload),
        url: `/api/Alliance/${allianceid}/SummaryReportData`,
        type: "POST",
        beforeSend: function (xhr) {
            var token = window.JWTManager.getToken();
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            console.log(data);
            setReportAggregateInfo(data);
            cb(data);

        },
        error: function (e) {
            alert(e.responseText);
            console.error(e);
        }


    });
}


function submitHandler(event,parentTable) {

    parentTable.ajax.reload();
}