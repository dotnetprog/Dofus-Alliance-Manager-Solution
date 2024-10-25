///<reference path="../../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../../libs/jquery/jquery.min.js" />
///<reference path="../../../libs/toastr/build/toastr.min.js" />
///<reference path="../../customlibs/bootstrapcustomdatatable.js" />
///<reference path="../../../libs/moment/min/moment.min.js" />
///<reference path="../../../libs/moment/locale/fr-ca.js" />
var datatable = null;
function LoadDatatable() {
    var parentState = $("#State").text();
    var currentAvaId = $('#Id').val();
    var allianceid = $("#allianceid").val();
    if (!window.JWTManager) {
        setTimeout(LoadDatatable, 50);
        return;
    }
    var dt_src = new BootstrapData.dataSource({
        type: 'odata',

        queryOptions: {
            $expand: 'ValidatedBy($select=Nickname,Alias),Member($select=Nickname,Alias,AvatarUrl)',
            $count: true,
            $filter: 'AvaId eq ' + currentAvaId
        },
        url: '/odata/v1/' + allianceid + '/AvAMembers',
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
    datatable = $('#membersTable').customBootstrapTable({
        datasource: dt_src,

        columns: [
            {
                name: 'Joueur',
                template: function (datarow) {

                    var username = datarow.Member.Nickname || datarow.Member.Alias;
                    let content = `${username}`;

                    if (!!datarow.Member.AvatarUrl) {
                        content = `<img src="${datarow.Member.AvatarUrl}" alt="" class="avatar-xs rounded-circle me-2">` + username;
                    }

                    return content;
                }
            },
            {
                name: 'Screen',
                template: function (datarow) {

                    var html = `<a href="${datarow.ImageUrl}" target="_blank">Lien</a>`;
                    return html;
                }

            },
            {
                name: 'État',
                field: 'ValidationState',
                sortable: true,
                template: function (datarow) {

                    var classBadge = !!datarow.ValidationState ? stateMapper[datarow.ValidationState] : 'bg-soft-warning';
                    var state = datarow.ValidationState || 'Pending';
                    var value = `<div class="badge ${classBadge} font-size-12">${state}</div>`;
                    return value;
                }
            },
            {
                name: 'Validée par',
                template: function (datarow) {
                    if (!datarow.ValidatedBy) {
                        return "";
                    }
                    var username = datarow.ValidatedBy.Nickname || datarow.ValidatedBy.Alias;
                    return username;

                }

            },
            {
                name: 'Montant pépites (Paye)',
                field: 'MontantPepites',
                type: 'number',
                sortable: true
            },
           
            {
                name: "Actions",
                hidden: parentState === 'Closed',
                actions: [
                    {
                        template: function (datarow) {
                            return `<a data-id="${datarow.Id}" href="javascript:void(0);" class="btn btn-success"><i class="fas fa-check-circle fa-lg"></i></a>`
                        },
                        click: function (e, datarow) {
                            console.log('approving ' + datarow.Id);
                           
                            datatable.showRowLoading(datarow.Id, true);
                            $.ajax({
                                contentType: "application/json",
                                async: true,
                                url: `/api/ava/${allianceid}/SetMemberValidationState/${datarow.Id}`,
                                type: "PUT",
                                data: JSON.stringify({ State: 1 }),
                                beforeSend: function (xhr) {
                                    var token = window.JWTManager.getToken();
                                    xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                                },
                                success: function () {
                                    datarow.set('ValidationState', 'Approved');
                                    datatable.showRowLoading(datarow.Id, false);

                                },
                                error: function (e) {

                                    console.error(e);
                                }


                            });


                        }
                       
                    },
                    {
                        template: function (datarow) {
                            return `<a data-id="${datarow.Id}" href="javascript:void(0);" class="btn btn-danger"><i class="fa fa-times-circle fa-lg"></i></a>`
                        },
                       
                        click: function (e,datarow) {
                            console.log('denying ' + datarow.Id);
                           
                            datatable.showRowLoading(datarow.Id, true);

                            $.ajax({
                                contentType: "application/json",
                                async: true,
                                url: `/api/ava/${allianceid}/SetMemberValidationState/${datarow.Id}`,
                                type: "PUT",
                                data: JSON.stringify({ State: 2 }),
                                beforeSend: function (xhr) {
                                    var token = window.JWTManager.getToken();
                                    xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                                },
                                success: function () {
                                    datatable.showRowLoading(datarow.Id, false);
                                    datarow.set('ValidationState', 'Rejected');

                                },
                                error: function (e) {

                                    console.error(e);
                                }


                            });
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



function AttachButtonEvents() {
    var currentAvaId = $('#Id').val();
    var allianceid = $("#allianceid").val();
    var distributeBtn = $('#distributeBtn');
    var closeBtn = $('#closeBtn');
    var openBtn = $('#openBtn');

    if (distributeBtn.length > 0) {

        distributeBtn.on('click', function (e) {
            displayLoading(true);
            $(e.currentTarget).addClass('disabled');
            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/api/ava/${allianceid}/Distribute/${currentAvaId}`,
                type: "PUT",
                beforeSend: function (xhr) {
                    var token = window.JWTManager.getToken();
                    xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                },
                success: function () {
                    displayLoading(false);
                    $(e.currentTarget).removeClass('disabled');
                    datatable.refresh();

                },
                error: function (e) {

                    console.error(e);
                }


            });
        })


    }
    if (closeBtn.length > 0) {
        closeBtn.on('click', function (e) {
            displayLoading(true);
            $(e.currentTarget).addClass('disabled');
            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/api/ava/${allianceid}/Close/${currentAvaId}`,
                type: "PUT",
                beforeSend: function (xhr) {
                    var token = window.JWTManager.getToken();
                    xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                },
                success: function () {
                   
                    $(e.currentTarget).removeClass('disabled');
                    window.location.reload();

                },
                error: function (e) {

                    console.error(e);
                }


            });
        })
    }
    if (openBtn.length > 0) {
        openBtn.on('click', function (e) {
            displayLoading(true);
            $(e.currentTarget).addClass('disabled');
            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/api/ava/${allianceid}/Open/${currentAvaId}`,
                type: "PUT",
                beforeSend: function (xhr) {
                    var token = window.JWTManager.getToken();
                    xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                },
                success: function () {
                   
                    $(e.currentTarget).removeClass('disabled');
                    window.location.reload();

                },
                error: function (e) {

                    console.error(e);
                }


            });
        })
    }







}
function displayLoading(display) {
    if (display) {
        $('#pageLoader').show();
    } else {
        $('#pageLoader').hide();
    }
    
}
$(document).ready(function () {
    displayLoading(false);
    AttachButtonEvents();    
    LoadDatatable();
    

});