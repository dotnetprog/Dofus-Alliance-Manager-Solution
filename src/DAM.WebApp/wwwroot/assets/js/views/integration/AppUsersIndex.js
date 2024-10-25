///<reference path="../../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../../libs/jquery/jquery.min.js" />
///<reference path="../../../libs/toastr/build/toastr.min.js" />
///<reference path="../../customlibs/bootstrapcustomdatatable.js" />
///<reference path="../../../libs/moment/min/moment.min.js" />
///<reference path="../../../libs/moment/locale/fr-ca.js" />

var datatable = null;
var cachedsecrets = {};
function LoadDataTable() {
    if (!window.JWTManager) {
        setTimeout(LoadDataTable, 50);
        return;
    }
    var allianceid = $("#allianceid").val();
    var dt_src = new BootstrapData.dataSource({
        type: 'odata',

        queryOptions: {
            $count: true
        },
        url: '/odata/v1/' + allianceid + '/AppUsers',
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
    datatable = $('#appuserListTable').customBootstrapTable({
        datasource: dt_src,

        columns: [
            {
                name: 'App name',
                field: 'Username',
                sortable: true
            },
            {
                name: 'Client Id',
                field: 'Id',
                sortable: true,
            },
            {
                name: 'Client Secret',
                actions: [{
                    template: function (datarow) {
                        return `<a data-id="${datarow.Id}" href="javascript:void(0);" class="btn btn-success"><i class="uil-sync font-size-18"></i>Générer un nouveau client secret</a>`
                    },
                    click: function (e, datarow) {
                        console.log('Generate Token ' + datarow.Id);

                        if (!!cachedsecrets[datarow.id]) {

                            navigator.clipboard.writeText(cachedsecrets[datarow.id]);
                            alert('Client Secret Saved to clipboard');
                            return;

                        }

                        datatable.showRowLoading(datarow.Id, true);
                        $.ajax({
                            //contentType: "application/json",
                            async: true,
                            url: `/odata/v1/${allianceid}/AppUsers(${datarow.Id})/SetClientSecret`,
                            type: "POST",
                            beforeSend: function (xhr) {
                                var token = window.JWTManager.getToken();

                                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                            },
                            success: function (secret) {
                                //display tooltip
                                //copy in clipboard the token.
                                cachedsecrets[datarow.id] = secret.value;
                               
                                navigator.clipboard.writeText(secret.value);
                                datatable.showRowLoading(datarow.Id, false);
                                alert('Client Secret Saved to clipboard');

                            },
                            error: function (e) {

                                console.error(e);
                            }


                        });


                    }
                }
                ]
            },
            {
                name: "Actions",
                actions: [
                    {
                        template: function (datarow) {
                            return `<a href="/Integration/${allianceid}/EditAppUser/${datarow.Id}" class="px-2 text-primary"><i class="uil uil-pen font-size-18"></i></a>`
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
            url: `/odata/v1/${allianceid}/AppUsers(${id})`,
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
