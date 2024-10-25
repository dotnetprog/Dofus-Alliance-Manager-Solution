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
            $expand: 'ModifiedBy($select=Nickname,Alias)',
            $count: true
        },
        url: '/odata/v1/' + allianceid + '/Saisons/',
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
        "Win": 'bg-success',
        "Lost": 'bg-danger'
    };
    var stateMapper = {
        "Active": 'bg-info',
        "Inactive": 'bg-dark'
    };
    datatable = $('#saisonsListTable').customBootstrapTable({
        datasource: dt_src,

        columns: [
            {
                name: 'Titre',
                field: 'Name',
                sortable: true
            },
            {
                name: 'État',
                field: 'State',
                sortable: true,
                template: function (datarow) {

                    var classBadge = !!datarow.State ? stateMapper[datarow.State] : 'bg-info';
                    var state = datarow.State || 'Actif';
                    var value = `<div class="badge ${classBadge} font-size-12">${state}</div>`;
                    return value;
                }

            },
            {
                name: 'Date de début',
                field: 'StartDate',
                template: function (datarow) {

                    var dateString = moment.utc(datarow.StartDate).format("D/MM/YYYY, H:mm:ss");
                    return dateString + " (UTC)";

                },
                sortable: true
            },
            {
                name: 'Date de fin',
                field: 'EndDate',
                template: function (datarow) {

                    var dateString = moment.utc(datarow.EndDate).format("D/MM/YYYY, H:mm:ss");
                    return dateString + " (UTC)";

                },
                sortable: true
            },
            {
                name: 'Modifié par',
                template: function (datarow) {

                    var username = datarow.ModifiedBy.Nickname || datarow.ModifiedBy.Alias;
                    return username;

                }

            },
            {
                name: 'Date de modification',
                field: 'ModifiedOn',
                sortable: true,
                template: function (datarow) {

                    var dateString = moment.utc(datarow.ModifiedOn).local().format("D/MM/YYYY, H:mm:ss");
                    return dateString;

                }

            },
            {
                name: "Actions",
                actions: [
                    {
                        template: function (datarow) {
                            return `<a href="/saison/${allianceid}/Edit/${datarow.Id}" class="px-2 text-primary"><i class="uil uil-pen font-size-18"></i></a>`
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
            url: `/odata/v1/${allianceid}/Saisons(${id})`,
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
