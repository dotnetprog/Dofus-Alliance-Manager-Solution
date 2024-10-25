///<reference path="../../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../../libs/jquery/jquery.min.js" />
///<reference path="../../../libs/toastr/build/toastr.min.js" />



function displaySuccess(nameObject) {
    
    toastr.success(nameObject+" Sauvegardé");
    
}
function displaySuccessWithMessage(message) {

    toastr.success(message);

}

function bindBaremeDeleteEvents() {
    var allianceid = $('#allianceid').val();
    $("a.deleteBaremebtn").on("click", function (e) {
        var id = $(e.currentTarget).attr("recordid");
        var rowElement = $(e.currentTarget).closest('tr');
        console.log("delete baremeid" + id);

        $.ajax({
            async: true,
            url: `/api/Alliance/${allianceid}/Bareme/${id}`,
            type: "DELETE",
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function () {
                rowElement.remove();
                console.log("success");
                displaySuccessWithMessage("Barème effacé avec succès.");
            },
            error: function (e) {

                console.error(e);
            }


        });


        

    });
}


function buildAutoMapConfigurationInterface(parentContainer, data, onConfigChangeHandler) {
    parentContainer.empty();
    const options = $('#templateChannels > option');
    let channels = options.map(idx => {
        const el = $(options[idx]);
        return {
            id: el.val(),
            name: el.text()
        }
    });
    channels = [{id:"-1",name:'select'},...channels];

    const buildChannelSelector = (selectid,channelid) => {

        let selectElement = document.createElement("select");
        selectElement.className = "form-select";
        selectElement.id = selectid;

        for (var i = 0; i < channels.length; i++) {
            const channel = channels[i];
            const optionElement = document.createElement("option");
            optionElement.value = channel.id;
            optionElement.innerHTML = channel.name;

            if (channel.id == channelid) {
                optionElement.selected = true;
            }

            selectElement.append(optionElement);
        }

        return selectElement;


    }

    const selectOnChangeHandler = (element,event) => {

        var config = {};
        const nodefChannelId = $('#autoMapAtkChannelCount0').val();
        config['nodefchannelenemy'] = !!nodefChannelId && nodefChannelId !== "-1" ? nodefChannelId : null;
        for (var i = 0; i < 5; i++) {
            const enemyindex = i + 1;
            const defchannelid = $("#autoMapDefChannelCount" + enemyindex).val();
            const atkchannelid = $("#autoMapAtkChannelCount" + enemyindex).val();
            config['defchannelenemy' + enemyindex] = !!defchannelid && defchannelid !== "-1" ? defchannelid : null;
            config['atkchannelenemy' + enemyindex] = !!atkchannelid && atkchannelid !== "-1" ? atkchannelid : null;
        }
        !!onConfigChangeHandler && onConfigChangeHandler(config);




    };


    const buildNoDefRow = () => {
        const NoDefSelectId = "autoMapAtkChannelCount0";
        let rowContainer = document.createElement('div');
        rowContainer.className = 'col-12';
        const label = document.createElement('label');
        label.className = "form-label";
        label.innerHTML = "Aucune défense";
        rowContainer.append(label);

        const nodefselectElement = buildChannelSelector(NoDefSelectId, data['nodefchannelenemy']);
        nodefselectElement.addEventListener('change', selectOnChangeHandler);
        rowContainer.append(nodefselectElement);
        return rowContainer;
    };

    parentContainer.append(buildNoDefRow());
    for (var i = 0; i < 5; i++) {

        const enemyindex = i + 1;
        const Defselectid = "autoMapDefChannelCount" + enemyindex;
        const Atkselectid = "autoMapAtkChannelCount" + enemyindex;

        const rowContainer = document.createElement('div');
        rowContainer.className = 'col-12';
       
        const label = document.createElement('label');
        label.className = "form-label";
        label.innerHTML = "Nombre ennemie: " + enemyindex;
        label.htmlFor = Defselectid;
        rowContainer.append(label);
        const div_row = document.createElement('div');
        div_row.className = 'row';
        const divselectdefwrapper = document.createElement('div');
        divselectdefwrapper.className = 'col-6';
        const labeldef = document.createElement('label');
        labeldef.className = "form-label";
        labeldef.htmlFor = Defselectid;
        labeldef.innerHTML = "Défense";
        divselectdefwrapper.append(labeldef);
        const divselectatkwrapper = document.createElement('div');
        divselectatkwrapper.className = 'col-6';
        const labelatk = document.createElement('label');
        labelatk.className = "form-label";
        labelatk.htmlFor = Atkselectid;
        labelatk.innerHTML = "Attaque";
        divselectatkwrapper.append(labelatk);

       
        const defselectElement = buildChannelSelector(Defselectid, data['defchannelenemy' + enemyindex]);
        divselectdefwrapper.append(defselectElement);
        const atkselectElement = buildChannelSelector(Atkselectid, data['atkchannelenemy' + enemyindex]);
        divselectatkwrapper.append(atkselectElement);
        //Handle onChange
        defselectElement.addEventListener('change', selectOnChangeHandler);
        atkselectElement.addEventListener('change', selectOnChangeHandler);
        //Build Json everytime anyconfig changes

        div_row.append(divselectdefwrapper);
        div_row.append(divselectatkwrapper);
        rowContainer.append(div_row);
        
        parentContainer.append(rowContainer);



    }


}


function bindEnemyDeleteEvents() {
    var allianceid = $('#allianceid').val();
    $("a.deleteEnemybtn").on("click", function (e) {
        var id = $(e.currentTarget).attr("recordid");
        var rowElement = $(e.currentTarget).closest('tr');
        console.log("delete enemyid" + id);

        $.ajax({
            async: true,
            url: `/api/Alliance/${allianceid}/Enemy/${id}`,
            type: "DELETE",
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function () {
                rowElement.remove();
                console.log("success");
                displaySuccessWithMessage("Alliance adverse effacé avec succès.");
            },
            error: function (e) {

                console.error(e);
            }


        });




    });
}
async function renderConfigInterface(behaviorType) {
    const parentcontainer = $("#behaviorConfigContainer");
    const dataJson = $("#BehaviorScreenConfigJSONData").val();
    const data = !!dataJson ? JSON.parse(dataJson) : {};
    switch (behaviorType) {
        default:
        case 1: //Commands
            parentcontainer.empty();
            parentcontainer.parent().hide();
            break;
        case 2://AutoChannelEnemy
            buildAutoMapConfigurationInterface(parentcontainer, data, (config) => {
                $("#BehaviorScreenConfigJSONData").val(JSON.stringify(config));
                console.log("config changed", { config });

            });
            parentcontainer.parent().show();
            break;


    }

}

function bindInterfaceRenderingToBehaviorType() {
    $("#BotScreenBehaviorType").change((e) => {
        renderConfigInterface(parseInt($(e.currentTarget).val()));
    });
}

function formatValueToPayload(value) {
    return value || null;
}
function validateJsonBehaviorConfig() {
    const behaviorType = parseInt($("#BotScreenBehaviorType").val());
    const dataJson = $("#BehaviorScreenConfigJSONData").val();
    switch (behaviorType) {
        default:
        case 1: //Commands
            return true;
            break;
        case 2://AutoChannelEnemy
            if (!dataJson) {
                return false;
            }
            var config = JSON.parse(dataJson);
            var values = [];
            for (var k in config) {
                if (!config[k]) {
                    continue;
                }
                values.push(config[k]);
            }

            let Duplicates = values.filter((item, index) => values.indexOf(item) !== index);
            return Duplicates.length == 0;
         
            break;


    }
}
$(document).ready(function () {
    renderConfigInterface(parseInt($("#BotScreenBehaviorType").val()));
    bindInterfaceRenderingToBehaviorType();
    bindBaremeDeleteEvents();
    bindEnemyDeleteEvents();
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
    var atkAttribute = $("#AtkScreen_DiscordChannelId");
    var defAttribute = $("#DefScreen_DiscordChannelId");
    var reportAttribute = $("#Rapport_DiscordChannelId");
    var canalSeasonRankingAttribute = $("#DefaultSeasonRankingChannelId");
    var configIdAttribute = $('#Id');
    var allianceidAttribute = $('#allianceid');
    var screenprepaAttribute = $('#IsScreenPrepaRequired');
    var allianceEnemyAttribute = $('#IsAllianceEnemyRequired');
    var avaforumAttribute = $('#Ava_DiscordForumChannelId');
    var baremeAtk = $("#DefaultAttaqueBareme");
    var baremeDef = $("#DefaultDefBareme");
    var behaviorConfig = $("#BehaviorScreenConfigJSONData");
    var behaviorType = $("#BotScreenBehaviorType");
    var autovalidatenodef = $("#AutoValidateNodef");//AllowSeasonOverlap
    var allowSeasonOverlap = $("#AllowSeasonOverlap");//AllowSeasonOverlap
    //role_approval
    var roleApproval = $("#ScreenApproverRoleId");
    $('#saveConfig').on('click', function () {
        if (!validateJsonBehaviorConfig()) {
            alert("Les canaux discord configuré pour les nombre ennemies doivent être tous unique.");
            return;
        }
        console.log(atkAttribute.val())
        console.log(defAttribute.val());
        console.log(roleApproval.val());
        console.log(allianceidAttribute.val());
        console.log(configIdAttribute.val());
        console.log(behaviorConfig.val());
        var payload = {
            Id: configIdAttribute.val(),
            AtkScreen_DiscordChannelId: formatValueToPayload(atkAttribute.val()),
            DefScreen_DiscordChannelId: formatValueToPayload(defAttribute.val()),
            ScreenApproverRoleId: formatValueToPayload(roleApproval.val()),
            Rapport_DiscordChannelId: formatValueToPayload(reportAttribute.val()),
            IsScreenPrepaRequired: screenprepaAttribute.is(':checked'),
            IsAllianceEnemyRequired: allianceEnemyAttribute.is(':checked'),
            Ava_DiscordForumChannelId: formatValueToPayload(avaforumAttribute.val()),
            DefaultSeasonRankingChannelId: formatValueToPayload(canalSeasonRankingAttribute.val()),
            DefaultBaremeAttaqueId: formatValueToPayload(baremeAtk.val()),
            DefaultBaremeDefenceId: formatValueToPayload(baremeDef.val()),
            BotScreenBehaviorType: !!behaviorType.val() ? parseInt($("#BotScreenBehaviorType").val()): 1,
            BehaviorScreenConfigJSONData: !!behaviorConfig.val() ? behaviorConfig.val() : null,
            AutoValidateNoDef: autovalidatenodef.is(':checked'),
            AllowSeasonOverlap: allowSeasonOverlap.is(':checked')
        };
        $.ajax({
            contentType: "application/json",
            async: true,
            data: JSON.stringify(payload),
            url: `/api/Alliance/${allianceidAttribute.val()}/UpdateConfiguration`,
            type: "PUT",
            beforeSend: function (xhr) {
                var token = window.JWTManager.getToken();
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            },
            success: function () {

                console.log("success");
                displaySuccess("Configuration");
            },
            error: function (e) {

                console.error(e);
            }


        });

    });


});