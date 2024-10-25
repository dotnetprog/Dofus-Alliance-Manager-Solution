///<reference path="../../../libs/chart.js/Chart.js" />

$(document).ready(function () {
   
    displayLoading(false);
    LoadDatatable();
});
var chart_initialized = false;
function getDataFromServer(allianceid,rankingid,cb) {

    $.ajax({
        contentType: "application/json",
        async: true,
        url: `/api/ScreenPosts/${allianceid}/GetScreensBySeasonRankingId/${rankingid}`,
        type: "GET",
        //beforeSend: function (xhr) {
        //    var token = window.JWTManager.getToken();
        //    xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        //},
        success: function (data) {
            console.log(data);
            cb({ data: data });
            LoadActivityChart(data);
        },
        error: function (e) {
            alert(e.responseText);
            console.error(e);
        }


    });
}
function getStateHtml(row) {

    var approvedIcon = 'fas fa-check-circle';
    var deniedIcon = "fas fa-times-circle";
    var pendingIcon = "fas fa-exclamation-circle";
    var icon,color;//Completed
    if (row.state !== 2) {
        return `<i style="color:blue;" class="${pendingIcon}"></i>`;
    }
    switch (row.stateResult) {
        case 0:
        case 2:
            icon = approvedIcon;
            color = "green";
            break;
        case 1:
        case 3:
            icon = deniedIcon;
            color = "red";
            break;
        default:
            icon = pendingIcon;
            break;
    }

    var baseHtml = `<i style="color:${color}" class="${icon}"></i>`;
    return baseHtml;

}
function LoadActivityChart(rows) {
    if (chart_initialized) {
        return;
    }

    var parsedrows = rows.map(r => {
        var hour = moment(r.createdOn).hour();
        return { hour: hour };

    });
    var df = Object.groupBy(parsedrows, parsedrow => parsedrow.hour);
    const datarows = [];
   
    const labels = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23];
    for (var i = 0; i < labels.length; i++) {
        if (!df[i]) {
            datarows.push(0);
        }
        else {
            datarows.push(df[i].length);
        }
    }
    const data = {
        labels: labels,
        datasets: [{
            label: 'Screens',
            data: datarows,
            fill: false,
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
        }]
    };
    const config = {
        type: 'line',
        data: data,
        options: {
            scales: {
                y: {
                    ticks: {
                        callback: function (label, index, labels) {
                            // when the floored value is the same as the value we have a whole number
                            if (Math.floor(label) === label) {
                                return label;
                            }

                        },
                    }
                },
                x: {
                    ticks: {
                        callback: function (label, index, labels) {
                            // when the floored value is the same as the value we have a whole number
                            return `${label}:00`;

                        },
                    }
                },
            }
        }
    };
    var containerChart = document.getElementById('activitychart');
    const mychart = new Chart(containerChart, config);
    chart_initialized = true;
}
function LoadDatatable() {
    var allianceid = $('#allianceid').val();
    var rankingid = $("#Id").val();
    let table = $(".datatable").DataTable({
        //  lengthChange: !1,
       
        ajax: function (d, cb) {

            getDataFromServer(allianceid, rankingid, cb);

        },
        "formatNumber": function (toFormat) {
            return toFormat.toLocaleString("en-US");
        },
       // order: [[8, 'desc']],
        columns: [
            {
                data: 'imageUrl',
                orderable:false,
                render: (data, type, row, meta) => {
                    return type === 'display' ?
                        `<a href="${data}" target="_blank">Lien</a>` :
                        data;
                }
            },
            {
                data: 'type',
                render: (data, type, row, meta) => {

                    const display = (v) => {
                        if (v === 0) {
                            //attack
                            return '<i style="color:red;" class="fas fa-fist-raised"></i>';
                        } else {
                            return '<i style="color:blue;" class="bx bx-shield-alt-2"></i>';
                        }
                    };

                    return type === 'display' ?
                        display(data) :
                        data;
                }
            },
            {

                data: "allianceEnemyName",
                render: (data, type, row, meta) => {



                    return !!data ? data : "Aucune";
                        
                }
            },
            {
                data: null,
                render: (data, type, row, meta) => {



                    return `${data.countAlly}v${data.countEnemy}`;
                       
                }
            },
            {
                data: null,
                render: (data, type, row, meta) => {

                    var result = "";
                    switch (type) {
                        case 'display':
                            result = getStateHtml(data);
                            break;
                        case 'sort':
                            result = row.stateResult || 0;
                            break;
                        default:
                            result = data;
                            break;
                    }

                    return result;
                }
            },
            {
                data:"pointsValue"
            },
            {
                data: "createdOn",
                render: $.fn.dataTable.render.moment("YYYY-MM-DDTHH:mm:ss.SSSSSZ", "dddd, DD MMMM YYYY, h:mm:ss a","fr-Ca",true)
            }
         
        ],
    });


}




function displayLoading(display) {
    if (display) {
        $('#pageLoader').show();
    } else {
        $('#pageLoader').hide();
    }

}