///<reference path="../../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../../libs/jquery/jquery.min.js" />
///<reference path="../../../libs/toastr/build/toastr.min.js" />
///<reference path="../../../libs/moment/min/moment.min.js" />
///<reference path="../../../libs/moment/locale/fr-ca.js" />
window.Parsley.addValidator("requiredIf", {
    validateString: function (value, requirement) {

        var inputElement = jQuery(requirement);
        let inputval = jQuery(requirement).val();
        if (inputElement.attr('type') === "number") {


            if (!!inputval && !isNaN(inputval)) {
                let numberVal = parseInt(inputval);
                if (numberVal === 0) {
                    return true;
                }

            } else if (!inputval) {
                return true;
            }
                

            return !!value;
        }
       
       


        if (jQuery(requirement).val()) {
            return !!value;
        }

        return true;
    },
    priority: 33
})
var datatable;
var dataSource;
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
    attachSaveEvent();
    attachGenerateRankingsEvent();
    attachPublishEvent();
    displayLoading(false);
    $('#form').parsley();
    LoadDatatable();
    SetupBonusModal();
   
});
function displayLoading(display) {
    if (display) {
        $('#pageLoader').show();
    } else {
        $('#pageLoader').hide();
    }

}
function SetupBonusModal() {
    var allianceid = $("#allianceid").val();
    $('#bonusModal').on('show.bs.modal', function (e) {

        var relatedrowid = $(e.relatedTarget).data('id');

        var ranking = dataSource.findRowFromCacheByKey(relatedrowid);

        var displayname = ranking.Member.Nickname || ranking.Member.Alias;

      
        $(this).find("#bonusModalLabel").text('Bonus pour ' + displayname);
        $(this).find("#bonus").val(ranking.BonusPepite);
        $(this).find("#reason-text").val(ranking.bonusReason);
        $(this).find("#saisonrankingid").val(relatedrowid);
    });
    $('#bonusModal').on('click', '.btn-primary', function (e) {

        var $modalDiv = $(e.delegateTarget);
        $modalDiv.addClass('loading');
        let reasonInput = $modalDiv.find("#reason-text");
        let bonus = $modalDiv.find("#bonus").val();
        const reason = reasonInput.val();
        const rowid = $modalDiv.find("#saisonrankingid").val();

        const bonusValidate = $modalDiv.find("#bonus").parsley("validate");
        const reasonValidate = reasonInput.parsley("validate");

     
        if (!reasonValidate.isValid()) {
            reasonInput.addClass("is-invalid");
        } else {
            reasonInput.removeClass("is-invalid");
        }

        if (!reasonValidate.isValid() || !bonusValidate.isValid()) {
            return;
        }


        if (!isNaN(bonus)) {
            bonus = parseInt(bonus);
        }
        $modalDiv.addClass('loading');
        var payload = {
            bonusReason: reason || "",
            BonusPepite: bonus || 0
        };
        console.log("updating ranking", payload);
        datatable.showRowLoading(rowid, true);
        $.ajax({
            contentType: "application/json",
            async: true,
            url: `/odata/v1/${allianceid}/SaisonRankings(${rowid})`,
            type: "PATCH",
            data: JSON.stringify(payload),
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function (row) {
                reasonInput.removeClass("is-invalid");
              
                $modalDiv.modal('hide').removeClass('loading');
                setTimeout(() => {
                    var ranking = dataSource.findRowFromCacheByKey(rowid);
                    ranking.set('bonusReason', row.bonusReason);
                    ranking.set('BonusPepite', row.BonusPepite);                  
                    ranking.set('MontantTotalPepite', row.MontantTotalPepite);
                    datatable.showRowLoading(rowid, false);
                    datatable.refresh();
                }, 100);
               
                

            },
            error: function (e) {
                datatable.showRowLoading(rowid, false);
                console.error(e);
            }

        });


    });
}
function LoadDatatable() {
    var parentState = $("#State").text();
    var currentSeasonId = $('#Id').val();
    var allianceid = $("#allianceid").val();
    if (!window.JWTManager) {
        setTimeout(LoadDatatable, 50);
        return;
    }
    dataSource = new BootstrapData.dataSource({
        type: 'odata',

        queryOptions: {
            $expand: 'Member($select=Nickname,Alias,AvatarUrl),ModifiedBy($select=Nickname,Alias,AvatarUrl)',
            $count: true,
            $filter: 'SaisonId eq ' + currentSeasonId
        },
        url: '/odata/v1/' + allianceid + '/SaisonRankings',
        async: true,
        ajax: {
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            }
        },
        schema: {
            key: 'Id'
        },
    });
    var stateMapper = {
        "Approved": 'bg-success',
        "Rejected": 'bg-danger'
    };
    datatable = $('#leaderboardTable').customBootstrapTable({
        datasource: dataSource,

        columns: [
            {
                name: 'Position',
                sortable: true,
                type: 'number',
                field: 'PositionRank',
                template: function (datarow) {

                    var diff = !!datarow.Previous_PositionRank ? datarow.Previous_PositionRank - datarow.PositionRank : null;
                    var diffString = "";
                    if (!!diff && diff !== 0) {
                        var up = `<span class="text-success me-1"><i class="mdi mdi-arrow-up-bold me-1"></i>+${diff}</span>`;
                        var down = `<span class="text-danger me-1"><i class="mdi mdi-arrow-down-bold me-1"></i>${diff}</span>`;
                        diffString = diff > 0 ? up : down;
                    }
                   
                    return `${datarow.PositionRank}${diffString}`;
                }
            },
            {
                name: 'Membre',
                template: function (datarow) {

                    var username = datarow.Member.Nickname || datarow.Member.Alias;
                    let content = `<a href="/saison/${allianceid}/DetailRanking/${datarow.Id}" class="text-body">${username}</a>`;

                    if (!!datarow.Member.AvatarUrl) { 
                        content = `<img src="${datarow.Member.AvatarUrl}" alt="" class="avatar-xs rounded-circle me-2">` + content;
                    }
                   
                    return content;
                }
            },
            {
                name: 'Nombre d\'attaques',
                field: 'Nombre_attaques',
                type: 'number',
                sortable: true
            },
            {
                name: 'Nombre de défences',
                field: 'Nombre_defense',
                type: 'number',
                sortable: true
            },
            {
                name: 'Nombre d\'AvA participées',
                field: 'NombreParticipationAvA',
                type: 'number',
                sortable: true
            },
            {
                name: 'Bonus',
                field: 'BonusPepite',
                template: (dr) => {
                    let reasonUi = "";
                    if (!!dr.bonusReason)
                        reasonUi = `<i data-toggle="tooltip" class="fas fa-exclamation-circle" title="${dr.bonusReason}"></i>`;

                    if (!dr.BonusPepite) {
                        return "";
                    }

                    return `<span  style="margin-right: 3px;">${dr.BonusPepite || ""}</span>${reasonUi}`;

                },
                sortable: true
            },
            {
                name: 'Nombre total de points',
                field: 'MontantTotalPepite',
                type: 'number',
                sortable: true
            },
            {
                name: "Actions",
                actions: [
                    {
                        template: function (datarow) {
                            return `<a data-id="${datarow.Id}"  data-bs-toggle="modal" data-bs-target="#bonusModal" href="#" class="px-2 text-primary"><i class="uil uil-pen font-size-18"></i></a>`
                        }

                    },
                    {
                        template: function (datarow) {
                            
                            return `<a href="/saison/${allianceid}/DetailRanking/${datarow.Id}" class="px-2 text-primary"><i class="fas fa-eye font-size-18"></i></a>`
                        }
                    }
                ]
            }
        ],
        paging: {
            displayCount: true,//shows the count in footer , if datasource type = odata, counturl on odata config of datasource must be set
            size: 30 //determines how many records per page.
        },
        selectable: false,
        IsReadOnly: false,

    });


}
function displaySuccessWithMessage(message) {

    toastr.success(message);

}



function buildErrorAlert(message) {
    var content = `<div class="alert alert-border alert-border-danger alert-dismissible fade show" role="alert">
            <i class="uil uil-exclamation-octagon font-size-16 text-danger me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>`

    var btn = document.createElement('button');
    btn.type = 'button';
    btn.className = "btn-close"
    $(btn).attr('data-bs-dismiss', "alert");
    $(btn).attr('aria-label', "Close");
    $(btn).on('click', function (e) {

        $(e.target).parent().remove();

    });
    var alertElement = $(content);
    alertElement.append($(btn));
    return alertElement;

}


function displayServerErrors(container,errors) {
    console.log(errors);
    for (let key in errors) { 
        const messages = errors[key];
        for (let i = 0; i < messages.length; i++) {
            const errorElement = buildErrorAlert(messages[i]);
            container.append(errorElement);

        }
    }

}

function saveForm(formContext) {

    formContext.errorsContainer.empty();
    var t = document.getElementsByClassName("needs-validation");
    Array.prototype.filter.call(t, function (e) {
        e.classList.add("was-validated");
    })


    var formdata = formContext.getData();
    if (!formContext.isValid()) {
        return;
    }
    displayLoading(true);



    var data = `{
                 "Name":"${formdata.name}",
                 "Description":"${formdata.description}",
                 "StartDate":"${formdata.startdate}",
                 "EndDate":"${formdata.enddate}",
                 "SeasonRankingChannelId":${formdata.channelid},
                 "BaremeAttackId": "${formdata.baremeAttackId}",
                 "BaremeDefenseId": "${formdata.baremeDefenseId}"
                }`;

   

    $.ajax({
        contentType: "application/json",
        async: true,
        data: data,
        url: `/odata/v1/${formdata.allianceid}/Saisons/${formdata.id}`,
        type: "PATCH",
        beforeSend: function (xhr) {
            var token = window.JWTManager.getToken();
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            displayLoading(false);
            displaySuccessWithMessage("Sauvegarde réussi !");

        },
        error: function (e) {
            displayLoading(false);
            toastr.error('Sauvegarde en échec !');
            //afficher validation
            if (!!e.responseJSON && !!e.responseJSON.errors)
                displayServerErrors(formContext.errorsContainer,e.responseJSON.errors);
            console.error(e);
        }


    });
}
function publishRankings(formContext) {
    formContext.errorsContainer.empty();
    displayLoading(true);
    var formdata = formContext.getData();
    $.ajax({
        contentType: "application/json",
        async: true,
        url: `/odata/v1/${formdata.allianceid}/Saisons(${formdata.id})/PublishRanking`,
        type: "POST",
        beforeSend: function (xhr) {
            var token = window.JWTManager.getToken();
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            displayLoading(false);
            displaySuccessWithMessage("Publication du classement réussi !");
          
        },
        error: function (e) {
            displayLoading(false);
            toastr.error('Publication du classement en échec !');
            //afficher validation
            if (!!e.responseJSON && (!!e.responseJSON.errors || !!e.responseJSON.detail)) {

                displayServerErrors(formContext.errorsContainer, e.responseJSON.errors || { issue: [e.responseJSON.detail] });
            }

            console.error(e);
        }


    });
}
function refreshRankings(formContext) {
    formContext.errorsContainer.empty();
    displayLoading(true);
    var formdata = formContext.getData();
    $.ajax({
        contentType: "application/json",
        async: true,
        url: `/odata/v1/${formdata.allianceid}/Saisons(${formdata.id})/GenerateRankings`,
        type: "POST",
        beforeSend: function (xhr) {
            var token = window.JWTManager.getToken();
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            displayLoading(false);
            displaySuccessWithMessage("Mise à jour du classement réussi !");
            datatable.refresh();
        },
        error: function (e) {
            displayLoading(false);
            toastr.error('Mise à jour du classement en échec !');
            //afficher validation
            if (!!e.responseJSON && (!!e.responseJSON.errors || !!e.responseJSON.detail)){

                displayServerErrors(formContext.errorsContainer, e.responseJSON.errors || { issue: [e.responseJSON.detail] } );
            }
               
            console.error(e);
        }


    });
}
function attachSaveEvent() {

    var formContext = getFormObject();
    formContext.saveButton.on('click', (e) => {
        e.preventDefault();
        saveForm(formContext);
    });

}

function attachGenerateRankingsEvent() {
    var formContext = getFormObject();
    formContext.rankingButton.on('click', (e) => {
        refreshRankings(formContext);
    });
}

function attachPublishEvent() {
    var formContext = getFormObject();
    formContext.publishBtn.on('click', (e) => {
        publishRankings(formContext);
    });
}


function getFormObject() {

    var id = $("#Id");
    var name = $("#Name");
    var desc = $("#Description");
    var startdate = $("#StartDate");
    var endDate = $("#EndDate");
    var saveButton = $('#saveBtn');
    var rankingButton = $('#refreshBtn');
    var alliance = $("#allianceid");
    var errorsContainer = $('#serverErrors');
    var publishBtn = $("#publishDiscordBtn");
    var channelSelect = $("#SelectedChannelId");
    var baremeDefenseId = $("#BaremeDefenseId");
    var baremeAttackId = $("#BaremeAttackId");

    const formObject = {
        id,
        name,
        desc,
        startdate,
        endDate,
        alliance,
        channelSelect,
        saveButton,
        rankingButton,
        errorsContainer,
        publishBtn,
        isValid: function () {

            const nameValidate = name.parsley("validate");
            const descValidate = desc.parsley("validate");
            const startdateValidate = startdate.parsley("validate");
            const endDateValidate = endDate.parsley("validate");
            const channelValidate = channelSelect.parsley("validate");

            return channelValidate.isValid() &&
                nameValidate.isValid() &&
                descValidate.isValid() &&
                startdateValidate.isValid() &&
                endDateValidate.isValid();


        },
        getData: function () {

        

            return {
                id: id.val(),
                name: name.val(),
                description: desc.val(),
                startdate: startdate.val() +'T00:00:00.000Z',
                enddate: endDate.val() + 'T23:59:59.999Z',
                allianceid: alliance.val(),
                channelid: channelSelect.val(),
                baremeDefenseId: baremeDefenseId.val(),
                baremeAttackId: baremeAttackId.val()

            };

        }
    };
    return formObject;

}


