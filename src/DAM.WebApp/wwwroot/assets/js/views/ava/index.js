///<reference path="../../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../../libs/jquery/jquery.min.js" />
///<reference path="../../../libs/toastr/build/toastr.min.js" />
///<reference path="../../customlibs/bootstrapcustomdatatable.js" />
///<reference path="../../../libs/moment/min/moment.min.js" />
///<reference path="../../../libs/moment/locale/fr-ca.js" />

var datatable = null;
function LoadDataTable() {
    if (!window.JWTManager) {
        setTimeout(LoadDataTable, 50);
        return;
    }
    var allianceid = $("#allianceid").val();
    var dt_src = new BootstrapData.dataSource({
        type: 'odata',

        queryOptions: {
            $expand: 'CreatedBy($select=Nickname,Alias)',
            $count:true
        },
        url: '/odata/v1/' + allianceid + '/AvAs/',
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
    var resultMapper = {
        "Win":'bg-success',
        "Lost":'bg-danger'
    };
    var stateMapper = {
        "Open": 'bg-info',
        "Closed": 'bg-dark'
    };
    datatable = $('#avaListTable').customBootstrapTable({
        datasource: dt_src,

        columns: [
            {
                name: 'Titre',
                field: 'Titre',
                sortable: true
            },
            {
                name: 'État',
                field: 'ResultState',
                sortable: true,
                template: function (datarow) {

                    var classBadge = !!datarow.ResultState ? resultMapper[datarow.ResultState] : 'bg-info';
                    var state = datarow.ResultState || 'In Progress';
                    var value = `<div class="badge ${classBadge} font-size-12">${state}</div>`;
                    return value;
                }

            },
            {
                name: 'Statut',
                field: 'State',
                sortable: true,
                template: function (datarow) {

                    var classBadge = !!datarow.State ? stateMapper[datarow.State] : stateMapper.Open;
                    var state = datarow.State || 'Open';
                    var value = `<div class="badge ${classBadge} font-size-12">${state}</div>`;
                    return value;
                }

            },
            {
                name: 'Montant total',
                field: 'MontantPepitesTotal',
                type: 'number',
                sortable: true
            },
            {
                name: 'Montant Obtenu',
                field: 'MontantPepitesObtenu',
                type: 'number',
                sortable: true
            },
            {
                name: 'Créé par',
                template: function (datarow) {

                    var username = datarow.CreatedBy.Nickname || datarow.CreatedBy.Alias;
                    return username;

                }

            },
            {
                name: 'Date de l\'évènement',
                field: 'CreatedOn',
                sortable: true,
                template: function (datarow) {

                    var dateString = moment.utc(datarow.CreatedOn).local().format("D/MM/YYYY, H:mm:ss");
                    return dateString;

                }

            },
            {
                name: "Actions",
                actions: [
                    {
                        template: function (datarow) {
                            return `<a href="/ava/${allianceid}/Summary/${datarow.Id}" class="px-2 text-primary"><i class="uil uil-pen font-size-18"></i></a>`
                        }
                    },
                    {
                        template: function (datarow) {
                            //<a href="#" recordid="@bareme.Id" class="px-2 text-danger deleteBaremebtn"><i class="uil uil-trash-alt font-size-18"></i></a>

                            return `<a data-id="${datarow.Id}"  data-bs-toggle="modal" data-bs-target="#deleteConfirmModal" href="#" class="px-2 text-danger"><i class="uil uil-trash-alt font-size-18"></i></a>`
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

function SetupDeleteConfirm() {
    var allianceid = $("#allianceid").val();
    $('#deleteConfirmModal').on('show.bs.modal', function (e) {
        $(this).find('.btn-danger').data('id', $(e.relatedTarget).data('id'));
    });
    $('#deleteConfirmModal').on('click', '.btn-danger', function (e) {

        var $modalDiv = $(e.delegateTarget);
        var id = $(this).data('id');

        $modalDiv.addClass('loading');
        console.log('removing ' + id);

        $.ajax({
            contentType: "application/json",
            async: true,
            url: `/odata/v1/${allianceid}/AvAs(${id})`,
            type: "DELETE",
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function () {
                datatable.removeRow(id);
                $modalDiv.modal('hide').removeClass('loading');

            },
            error: function (e) {

                console.error(e);
            }

        });


    });
}

$(document).ready(function () {

    LoadDataTable();
    SetupDeleteConfirm();
    
});
