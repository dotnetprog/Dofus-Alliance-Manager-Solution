///<reference path="../../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../../libs/jquery/jquery.min.js" />
///<reference path="../../../libs/moment/min/moment.min.js" />
///<reference path="../../../libs/apexcharts/apexcharts.min.js" />
///<reference path="../../../libs/toastr/build/toastr.min.js" />

/*var options = {
    fill: {
        colors: ["#34c38f"]
    },
    series: [70],
    chart: {
        type: "radialBar",
        width: 45,
        height: 45,
        sparkline: {
            enabled: !0
        }
    },
    dataLabels: {
        enabled: !1
    },
    plotOptions: {
        radialBar: {
            hollow: {
                margin: 0,
                size: "60%"
            },
            track: {
                margin: 0
            },
            dataLabels: {
                show: !1
            }
        }
    }
}
  , chart = new ApexCharts(document.querySelector("#orders-chart"),options);
chart.render();*/
var PageConfig = {
    allianceid: null,

    getAllianceId: function () {
        if (!this.allianceid) {
            this.allianceid = $("#allianceid").val();
        }
        return this.allianceid;
    },
    charts: {
        defenseCounter: null,
        atkCounter:null,
        defenseResultStates:null
    }
};
var AtkPieChart = null;
var DefPieChart = null;
var activityChart = null;

var ApiClient = {

    getDefScreensData: function(fq)  {
        var promise = new Promise((resolve, reject) => {
            var allianceid = PageConfig.getAllianceId();
            

            var query = `?$apply=${fq || ""}groupby((StatutTraitementValidation),aggregate(Id with countdistinct as total))`;
           
           
            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/odata/v1/${allianceid}/DefScreensPosts${query || ""}`,
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
    },
    getDefScreensByResultStateData: function (fq) {
        var promise = new Promise((resolve, reject) => {
            var allianceid = PageConfig.getAllianceId();


            var query = `?$apply=${fq || ""}groupby((StatutResultatValidation),aggregate(Id with countdistinct as total))`;


            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/odata/v1/${allianceid}/DefScreensPosts${query || ""}`,
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
    },
    getAtkScreens : function (fq) {

        var promise = new Promise((resolve, reject) => {
            var allianceid = PageConfig.getAllianceId();
            var query = `?$select=CreatedOn${fq}`;
            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/odata/v1/${allianceid}/AtkScreensPosts${query || ""}`,
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
        

    },
    getDefScreens : function (fq) {

        var promise = new Promise((resolve, reject) => {
            var allianceid = PageConfig.getAllianceId();
            var query = `?$select=CreatedOn${fq}`;
            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/odata/v1/${allianceid}/DefScreensPosts${query || ""}`,
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


    },
    getAtkScreensData: function (fq) {
        var promise = new Promise((resolve, reject) => {
            var allianceid = PageConfig.getAllianceId();


            var query = `?$apply=${fq || ""}groupby((StatutTraitementValidation),aggregate(Id with countdistinct as total))`;
           
            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/odata/v1/${allianceid}/AtkScreensPosts${query || ""}`,
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
    },
    getAtkScreensByResultStateData: function (fq) {
        var promise = new Promise((resolve, reject) => {
            var allianceid = PageConfig.getAllianceId();


            var query = `?$apply=${fq || ""}groupby((StatutResultatValidation),aggregate(Id with countdistinct as total))`;


            $.ajax({
                contentType: "application/json",
                async: true,
                url: `/odata/v1/${allianceid}/AtkScreensPosts${query || ""}`,
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
    },



};
function getFilterRawFromType(type) {

    //var fromUtc = moment.utc(filterObj.from).format();
    //filter(cast(CreatedOn,%27Edm.DateTimeOffset%27)
    var today = moment();

    var queryFilter = ``;

    switch (type) {
        case 'weekly':
            const from_week_date = today.startOf('week');
            var fromWeekDate = from_week_date.format("YYYY-MM-DD") + "T00:00:00.000Z";
            queryFilter = `CreatedOn gt ${fromWeekDate}`;
            break;
        case 'monthly':
            const from_month_date = today.startOf('month');
            var frommonthDate = from_month_date.format("YYYY-MM-DD") + "T00:00:00.000Z";
            queryFilter = `CreatedOn gt ${frommonthDate}`;
            break;
        case 'all':
        default:
            return "";
            break;
    }
    return `&$filter=${queryFilter}`;

}
function getFilterFromType(type) {
    
    //var fromUtc = moment.utc(filterObj.from).format();
    //filter(cast(CreatedOn,%27Edm.DateTimeOffset%27)
    var today = moment();
   
    var queryFilter = ``;

    switch (type) {
        case 'weekly':
            const from_week_date = today.startOf('week');
            var fromWeekDate = from_week_date.format("YYYY-MM-DD") + "T00:00:00.000Z";
            queryFilter = `CreatedOn gt ${fromWeekDate}`;
            break;
        case 'monthly':
            const from_month_date = today.startOf('month');
            var frommonthDate = from_month_date.format("YYYY-MM-DD") + "T00:00:00.000Z";
            queryFilter = `CreatedOn gt ${frommonthDate}`;
            break;
        case 'all':
        default:
            return "";
            break;
    }
    return `filter(${queryFilter})/`;

}

async function renderDefCounter(filter) {
    var data = await ApiClient.getDefScreensData(filter);

    var pendingValue = data.value.find(v => v.StatutTraitementValidation === 'Pending') || { total:0};

    $('#defcount').text(pendingValue.total);
    var all = data.value.map(v => v.total);

    var sum = all.reduce((partialSum, a) => partialSum + a, 0);

    var pourcentage = (pendingValue.total / sum) * 100;
    var options = {
        fill: {
            colors: ["#0086ff"]
        },
        labels: ['En attente'],
        series: [pourcentage],
        chart: {
            type: "radialBar",
            width: 45,
            height: 45,
            sparkline: {
                enabled: !0
            }
        },
        dataLabels: {

            enabled: true,
            formatter: function (val) {
                return val + "%"
            }
        },
        plotOptions: {
            radialBar: {
                hollow: {
                    margin: 0,
                    size: "60%"
                },
                track: {
                    margin: 0
                },
                dataLabels: {
                    show: false
                }
            }
        }
    };
    if (!PageConfig.charts.defenseCounter) {
        PageConfig.charts.defenseCounter = new ApexCharts(document.querySelector("#total-pending-def"), options);
        PageConfig.charts.defenseCounter.render();
    } else {
        PageConfig.charts.defenseCounter.updateOptions(options);
    }
    
}

async function renderAtkCounter(filter) {
    var data = await ApiClient.getAtkScreensData(filter);

    var pendingValue = data.value.find(v => v.StatutTraitementValidation === 'Pending') || { total: 0 };
    $('#atkcount').text(pendingValue.total);
    var all = data.value.map(v => v.total);

    var sum = all.reduce((partialSum, a) => partialSum + a, 0);

    var pourcentage = (pendingValue.total / sum) * 100;
    var options = {
        fill: {
            colors: ["#0086ff"]
        },
        labels: ['En attente'],
        series: [pourcentage],
        chart: {
            type: "radialBar",
            width: 45,
            height: 45,
            sparkline: {
                enabled: !0
            }
        },
        dataLabels: {

            enabled: true,
            formatter: function (val) {
                return val + "%"
            }
        },
        plotOptions: {
            radialBar: {
                hollow: {
                    margin: 0,
                    size: "60%"
                },
                track: {
                    margin: 0
                },
                dataLabels: {
                    show: false
                }
            }
        }
    };
    if (!PageConfig.charts.atkCounter) {
        PageConfig.charts.atkCounter = new ApexCharts(document.querySelector("#total-pending-atk"), options);
        PageConfig.charts.atkCounter.render();
    } else {
        PageConfig.charts.atkCounter.updateOptions(options);
    }
       
}

async function AttachDropDownCounterEvents() {
    $("#counterDropdownMenu").on('click', 'li', function () {
        displayLoading(true);
        var mainDd = $('#counterDropdown');
        var txt = $(this).text();
        var value = $(this).attr('data-value');

        mainDd.children('span')[0].childNodes[0].textContent = txt;
        mainDd.attr('data-value', value);
        var filter = getFilterFromType(value);
        var defaultScreenQuery = getFilterRawFromType(value);
        fetchData(filter, defaultScreenQuery).then(result => {
            Promise.all([renderDefCounter(filter),
                renderAtkCounter(filter),
                renderPieChartDefense(result.defdataset),
                renderPieChartAtk(result.atkdataset),
                LoadActivityChart(result.allscreens)
            ]).then(() => displayLoading(false));
        });

       

    });
   
}

function LoadActivityChart(rows) {
    var parsedrows = rows.map(r => {
        var hour = moment(r.CreatedOn).hour();
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
    if (activityChart == null) {
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
                responsive: true,
                maintainAspectRatio: false,
                
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
        var containerChart = document.getElementById('activitylinechart');
        activityChart = new Chart(containerChart, config);
    } else {
        activityChart.data.datasets[0].data = datarows;
        activityChart.update();
    }
}
async function fetchData(filter,screenfilter) {
    var defs = await ApiClient.getDefScreensByResultStateData(filter);
    var attaques = await ApiClient.getAtkScreensByResultStateData(filter);
    var screen_atk = await ApiClient.getAtkScreens(screenfilter);
    var screen_def = await ApiClient.getDefScreens(screenfilter);



    return {
        atkdataset: attaques,
        defdataset: defs,
        screenatkset: screen_atk.value,
        screendefset: screen_def.value,
        allscreens: [].concat(screen_atk.value).concat(screen_def.value)
    };
}

async function renderPieChartDefense(data) {



    var manualyValid = data.value.find((d) => d.StatutResultatValidation === 'ManualyValid')|| { total:0};
    var manualyInvalid = data.value.find((d) => d.StatutResultatValidation === 'ManualyInvalid') || { total: 0 };
    var pending = data.value.find((d) => d.StatutResultatValidation === null) || { total: 0 };
    if (DefPieChart == null) {
        const options = {
            labels: ["Approuvé", "Refusé", "En attente"],
            datasets: [{
                label: 'Screen Défenses',
                data: [manualyValid.total, manualyInvalid.total, pending.total],
                backgroundColor: ["#228e68", "#f46a6a", "#d9a245"],
                hoverOffset: 4
            }]
        };
        const config = {
            type: 'pie',
            data: options,
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        };
        var containerChart = document.getElementById('pie_chart_defense');
        DefPieChart = new Chart(containerChart, config);
    } else {
        DefPieChart.data.datasets[0].data = [manualyValid.total, manualyInvalid.total, pending.total];
        DefPieChart.update();
    }
   
}
async function renderPieChartAtk(data) {

    

    var manualyValid = data.value.find((d) => d.StatutResultatValidation === 'ManualyValid') || { total: 0 };
    var manualyInvalid = data.value.find((d) => d.StatutResultatValidation === 'ManualyInvalid') || { total: 0 };
    var pending = data.value.find((d) => d.StatutResultatValidation === null) || { total: 0 };
    if (AtkPieChart == null) {
        const options = {
            labels: ["Approuvé", "Refusé", "En attente"],
            datasets: [{
                label: 'Screen Attaques',
                data: [manualyValid.total, manualyInvalid.total, pending.total],
                backgroundColor: ["#228e68", "#f46a6a", "#d9a245"],
                hoverOffset: 4
            }]
        };
        const config = {
            type: 'pie',
            data: options,
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        };
        var containerChart = document.getElementById('pie_chart_attaque');
        AtkPieChart = new Chart(containerChart, config);
    } else {
        AtkPieChart.data.datasets[0].data = [manualyValid.total, manualyInvalid.total, pending.total];
        AtkPieChart.update();
    }
  
  
   
}

function displayLoading(display) {
    if (display) {
        $('#pageLoader').show();
    } else {
        $('#pageLoader').hide();
    }

}
function LoadUi() {
    if (!window.JWTManager) {
        setTimeout(LoadUi, 50);
        return;
    }
    displayLoading(true);
    var defaultQuery = getFilterFromType('weekly');
    var defaultScreenQuery = getFilterRawFromType('weekly');
    fetchData(defaultQuery, defaultScreenQuery).then(result => {
        Promise.all([AttachDropDownCounterEvents(),
            renderPieChartDefense(result.defdataset),
        renderDefCounter(defaultQuery),
            renderAtkCounter(defaultQuery),
            renderPieChartAtk(result.atkdataset),
            LoadActivityChart(result.allscreens)
        ]).then(() => displayLoading(false));
    })
   
}

$(document).ready(LoadUi);