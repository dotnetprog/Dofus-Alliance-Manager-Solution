///<reference path="../../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../../libs/jquery/jquery.min.js" />
///<reference path="../../../libs/moment/min/moment.min.js" />
///<reference path="../../../libs/moment/locale/fr-ca.js" />

var QueryData = {
    pageIndex : 1,
    pageSize : 8,
    totalRecords: 0,
    filter:null,
    hasMoreRecord: function () {
        return (this.totalRecords - (this.pageSize * this.pageIndex)) > 0;
    }
}

var PageConfig = {
    allianceid: ""
};

function displaySuccessWithMessage(message) {

    toastr.success(message);

}
function getStateHtml(row) {

    var approvedIcon = 'fas fa-check-circle';
    var deniedIcon = "fas fa-times-circle";
    var pendingIcon = "fas fa-exclamation-circle";
    var icon;

    switch (row.StatutResultatValidation) {
        case "Valid":
        case "ManualyValid":
            icon = approvedIcon;
            break;
        case "NotValid":
        case "ManualyInvalid":
            icon = deniedIcon;
            break;
        default:
            icon = pendingIcon;
            break;
    }
  
    var baseHtml = `<div class="float-end">
                        <i class="${icon}"></i>
                    </span>
                </div>`;
    return baseHtml;
    
}
function getClosedByData(row) {

    var label = '';
    switch (row.StatutResultatValidation) {
        case "Valid":
        case "ManualyValid":
            label = 'Approuvé par:'
            break;
        case "NotValid":
        case "ManualyInvalid":
            label = 'Rejeté par:'
            break;
        default:
            return ''
            break;
    }

    const closedBy = row.ClosedBy;
    if (!closedBy) {
        return '';
    }
    return ` <label><b>${label}</b>${closedBy.Nickname || closedBy.Alias}</label>`;

}

function buildCard(row) {
    var container = document.createElement('div');
    container.classList.add("card");
    container.classList.add("text-center");
    
    var body = document.createElement('div');
    body.classList.add("card-body");
    $(body).html(`<div class="float-start">
                    <span class="text-body font-size-16">
                        <i class="bx bx-shield-alt-2"></i>
                    </span>
                </div>`);
    $(body).append(getStateHtml(row));
    $(body).append('<div class="clearfix"></div>');
    var imgContainer = document.createElement("div");
    imgContainer.className = "mb-6";
    var img = document.createElement('img');
    img.className = 'img-thumbnail';
    img.src = row.ImageUrl;
    var aimg = document.createElement('a');
    aimg.href = row.ImageUrl;
    aimg.title = 'Defense';
    aimg.append(img);
    imgContainer.append(aimg);

    $(imgContainer).magnificPopup({
        delegate: "a",
        type: "image",
        closeOnContentClick: !1,
        closeBtnInside: !1,
        mainClass: "mfp-with-zoom mfp-img-mobile",
        image: {
            verticalFit: !0,
            titleSrc: function (e) {
                return e.el.attr("title") + ' &middot; <a href="' + e.el.src + '" target="_blank">image source</a>'
            }
        },
        gallery: {
            enabled: !0
        },
        zoom: {
            enabled: !0,
            duration: 300,
            opener: function (e) {
                return e.find("img")
            }
        }
    })

    var membernames = row.Members.map(m => `${m.AllianceMember.Nickname || m.AllianceMember.Alias}(${m.CharacterCount})`);
    $(body).append(imgContainer);
    var username = row.CreatedBy.Nickname || row.CreatedBy.Alias;
    var nbAttackants = row.Nombre_Attaquant || 0;
    var nbAlly = row.Nombre_Defenseur || 0;
    var Enemy = !!row.AllianceEnemy ? row.AllianceEnemy.Name : "Aucune";
    var createdon = moment.utc(row.CreatedOn).local().format("D/MM/YYYY, H:mm:ss");


    const body_content = `<div class='row'>
					<div class='col-xl-10 col-sm-12 col-mb-10' style='text-align: left;'>
						<div class='row'>
							<div class='col-xl-12 col-sm-12 col-mb-12'>
							 <div><label><b>Enregistré par :</b>${username}</label></div>
							 <div><label><b>Enregistré le :</b>${createdon}</label></div>
							 <div><label><b>Alliance adverse:</b>${Enemy}</label></div>
							 <div><label><b>Membres:</b> ${membernames.join(',')} </label></div>
							 ${getClosedByData(row)}
							</div>
						</div>
					</div>
					<div class='col-xl-2 col-sm-12 col-mb-2'>
						<p class="mb-2">${nbAlly}vs${nbAttackants}</p>
						<p class="mb-2">${row.Target}</p>
					</div>
				</div>`;



    $(body).append(body_content);
    var buttons = buildActions(row);
    container.append(body);
    container.append(buttons)
    return container;
}
function buildButton(btnText, icon, OnClickEvent) {
    var btn = document.createElement("button");
    btn.classList.add("btn");
    btn.classList.add("btn-outline-light");
    btn.classList.add("text-truncate");

    var iconElement = document.createElement("i");
    iconElement.className = icon;
    iconElement.style.paddingRight = "5px";
    btn.append(iconElement);

    var span = document.createElement("span");
    span.textContent = btnText;


    $(btn).append(span);
    btn.onclick = OnClickEvent;
    return btn;
}
function buildActions(row) {
    /*
     <div class="btn-group" role="group">
                <button type="button" class="btn btn-outline-light text-truncate"><i class="fas fa-check-circle"></i>Approve</button>
                <button type="button" class="btn btn-outline-light text-truncate"><i class="fas fa-times-circle"></i>Reject</button>

            </div>
    */
    var buttonContainer = document.createElement("div");
    buttonContainer.className = "btn-group";

    var denyHandler = (e) => {

           //ajax call to set resultstate to ManuallyInvalid.
           // refresh the title.
        console.log("Denying : " + row.Id);
        var containerTile = $(e.currentTarget).parent().parent().parent();
        setScreenValidationState(containerTile, row.Id, 3).then((container, state) => {
            container.remove();
            displaySuccessWithMessage('Screen refusé');
        });



    };
    var approveHandler = (e) => {

        //ajax call to set resultstate to ManuallyValid.
        // refresh the title.
        console.log("Approving : " + row.Id);
        var containerTile = $(e.currentTarget).parent().parent().parent();
        setScreenValidationState(containerTile, row.Id, 2).then((container, state) => {
            container.remove();
            displaySuccessWithMessage('Screen approuvé');
        });
    };

    switch (row.StatutResultatValidation) {
        case "Valid":
        case "ManualyValid":
            buttonContainer.append(buildButton("Refuser", "fas fa-times-circle", denyHandler));
            break;
        case "NotValid":
        case "ManualyInvalid":
            buttonContainer.append(buildButton("Approuver", "fas fa-check-circle", approveHandler));
            break;
        default:
            buttonContainer.append(buildButton("Approuver", "fas fa-check-circle", approveHandler));
            buttonContainer.append(buildButton("Refuser", "fas fa-times-circle", denyHandler)); 
            break;
    }
    return buttonContainer;


}
function buildTile(row) {
    var container = document.createElement('div');
    container.className = "col-xl-3 col-sm-6";
    var cardBody = buildCard(row);
    container.append(cardBody);

    

    return container;
}





function getScreens(expand,pageSize, currentpage, filter) {
    var promise = new Promise((resolve, reject) => {

        var skip = (currentpage - 1) * pageSize;

        var query = `?$top=${pageSize}`;
        if (skip > 0) {
            query += `&$skip=${skip}`;
        }
        if (!!filter) {
            query += "&$filter=" + filter;
        }
        if (!!expand) {
            query += "&$expand=" + expand;
        }
        query += "&$count=true";
        $.ajax({
            contentType: "application/json",
            async: true,
            url: `/odata/v1/${PageConfig.allianceid}/DefScreensPosts${query || ""}`,
            type: "GET",
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function (data) {
                resolve(data);

            },
            error: function (e) {

                reject(e);
            }


        });



    });
    return promise;

}

function setScreenValidationState(container,screenid, state) {
    var promise = new Promise((resolve, reject) => {
        var data = {
            ScreenId: screenid,
            Result: state
        };

        $.ajax({
            contentType: "application/json",
            async: true,
            url: `/api/alliance/${PageConfig.allianceid}/SetValidationResult`,
            type: "PUT",
            data: JSON.stringify(data),
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function () {
                resolve(container, state);

            },
            error: function (e) {

                reject(e);
            }


        });


    });
    return promise;
   

}

function refreshTileStatus(container,state) {
    

}

function initializeGrid() {
    if (!window.JWTManager) {

        setTimeout(initializeGrid,50);
        return;

    }
    var container = $("#screenGrid");
    container.empty();
    LoadData();
}
function iniFilterPanel() {
    $('#processingstate').select2({
        theme: "bootstrap-5",
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        closeOnSelect: false,
    });//resultstate
    $('#resultstate').select2({
        theme: "bootstrap-5",
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        closeOnSelect: false,
    });
    $('#playersearch').select2({
        ajax: {
            url: `/odata/v1/${PageConfig.allianceid}/AllianceMembers`,
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            data: function (params) {
                var query = {
                    $top: '5',
                    $select: "DiscordId,Nickname,Alias"
                }
                if (!params.term) {
                    return query;
                }
                query.$filter = `contains(Alias,'${params.term}') or contains(Nickname,'${params.term}')`;

                // Query parameters will be ?search=[term]&type=public
                return query;
            },
            processResults: function (data) {
                // Transforms the top-level key of the response object from 'items' to 'results'
                return {
                    results: data.value.map(d => {
                        return {
                            id: d.DiscordId,
                            text: d.Nickname || d.Alias
                        }

                    })
                };
            }
        },
        theme: "bootstrap-5",
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        escapeMarkup: function (markup) {
            return markup;
        },
        closeOnSelect: false,
    });
    $("#submit").on('click', function () {
        var filterObjects = getFilter();
        console.log(filterObjects);
        //build $filter query
        var filter = buildOdataFilterFromFilterObject(filterObjects);
        QueryData.filter = filter;
        QueryData.pageIndex = 1;


        //Query data
        initializeGrid();
    })
}

function LoadData() {
    var container = $("#screenGrid");
    getScreens("ClosedBy($select=Alias,Nickname),CreatedBy($select=Nickname,Alias),AllianceEnemy($select=Name),Members($select=CharacterCount;$expand=AllianceMember($select=Alias,Nickname))", QueryData.pageSize, QueryData.pageIndex, QueryData.filter).then((data) => {
        $("#LoadNext").removeAttr("disabled");
        QueryData.totalRecords = data["@odata.count"];
        if (QueryData.hasMoreRecord()) {
            $("#LoadMoreIndicator").show();
        } else {
            $("#LoadMoreIndicator").hide();
        }
        data.value.forEach((row) => {
            var tile = buildTile(row);
            container.append(tile);
        });

    }).catch((e) => {
        $("#LoadMoreIndicator").hide();
        console.error(e);
    });
}
function buildOdataFilterFromFilterObject(filterObj) {
    var conditions = [];
    if (filterObj.processStates.length > 0) {
        conditions.push(`StatutTraitementValidation in (${filterObj.processStates.map(s => `'${s}'`).join(',')})`);
    }
    if (filterObj.validationStates.length > 0) {
        var subconditions = [];
        if (filterObj.validationStates.indexOf("-1") > -1) {
            subconditions.push("StatutResultatValidation eq null");
        }
        var filteredstates = filterObj.validationStates.filter(function (e) { return e !== '-1' });
        if (filteredstates.length > 0)
            subconditions.push(`StatutResultatValidation in (${filterObj.validationStates.filter(function (e) { return e !== '-1' }).map(s => `'${s}'`).join(',')})`);
        conditions.push(`(${subconditions.join(' or ')})`);
    }
    if (!!filterObj.from) {
        var fromDate = moment(filterObj.from).format("YYYY-MM-DD") + "T00:00:00.000Z";
        //var fromUtc = moment.utc(filterObj.from).format();
        conditions.push(`CreatedOn gt ${fromDate}`);
    }
    if (!!filterObj.to) {
        var toDate = moment(filterObj.to).format("YYYY-MM-DD") + "T00:00:00.000Z";
        // var toUtc = moment.utc(filterObj.to).format();

        //SDateTime gt 2014-06-23T00:00:00.000Z and SDateTime lt 2014-06-26T03:30:00.000Z
        conditions.push(`CreatedOn lt ${toDate}`);
    }

    if (!!filterObj.players) {
        filterObj.players.forEach((id) => {

            conditions.push(`Members/any(m: m/AllianceMember/DiscordId eq '${id}')`);

        })
    }

    return conditions.length > 0 ? conditions.join(' and ') : null;

}
function getFilter() {
    var from = $("#From").data('datepicker');
    var to = $("#To").data('datepicker');
    var fromDate = !!from ? from.getDate() : null;
    var toDate = !!to ? to.getDate() : null;

    var states = $('#processingstate').val();
    var validationStates = $('#resultstate').val();
    var players = $("#playersearch").val();
    return {
        from: fromDate,
        to: toDate,
        processStates: states,
        validationStates: validationStates,
        players: players
    };

}
function AttachLoader() {
    $("#LoadNext").on('click', function (e) {
        $("#LoadNext").attr("disabled", "disabled");
        if (QueryData.hasMoreRecord()) {
            QueryData.pageIndex = QueryData.pageIndex + 1;
            LoadData();
        }

    });
}

$(document).ready(function () {
    PageConfig.allianceid = $("#allianceid").val();
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
    moment.locale("fr-ca");
    iniFilterPanel();
   // initializeGrid();
    AttachLoader();
   


});
