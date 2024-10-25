using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.Domain.Enums;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Models.Alliance;
using DAM.WebApp.OAuth.Discord.Services;
using Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAM.WebApp.Controllers
{
    [RequireHttps]
    [Authorize(AuthenticationSchemes = "Discord")]
    [TypeFilter(typeof(CheckAlliianceRegistration), Order = 2)]
    [TypeFilter(typeof(AllianceDataFilter), Order = 1)]
    public class AllianceController : Controller
    {
        private readonly IUserServiceAsync _userService;
        private readonly IDiscordBotService _discordService;
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IBaremeServiceAsync _baremeService;
        public AllianceController(IUserServiceAsync userService,
            IDiscordBotService botService,
            IAllianceManagementServiceAsync allianceService,
            IBaremeServiceAsync baremeService
            )
        {
            this._userService = userService;
            this._discordService = botService;
            this._allianceService = allianceService;
            this._baremeService = baremeService;
        }
        [HttpGet]
        [Route("{controller}/{allianceid?}")]
        public async Task<IActionResult> Index(ulong? allianceid)
        {
            ViewBag.Alliance = null;
            if (allianceid.HasValue)
            {
                return RedirectToAction("Dashboard", new { allianceid = allianceid.Value });

            }
            var guilds = await _userService.GetGuilds();
            var alliances = guilds.Select(g => new AllianceViewModel
            {
                DiscordId = g.Id,
                DisplayName = g.Name,
                ImageUrl = g.IconUrl
            });


            return View(alliances);
        }


        [HttpGet]
        [Route("{controller}/{allianceid}/Dashboard")]
        public async Task<IActionResult> Dashboard(ulong allianceid)
        {
            return View();

        }
        [HttpGet]
        [Route("{controller}/{allianceid}/Enemy/Add")]
        public async Task<IActionResult> AddEnemy(ulong allianceid)
        {
            return View();
        }
        [HttpGet]
        [Route("{controller}/{allianceid}/Enemy/Edit/{EnemyId}")]
        public async Task<IActionResult> EditEnemy(ulong allianceid, Guid EnemyId)
        {
            var alliance = ViewBag.allianceContext as Alliance;

            var enemy = await _allianceService.GetEnemy(alliance.Id, EnemyId);
            return View(enemy);
        }
        [HttpGet]
        [Route("{controller}/{allianceid}/Bareme/Add")]
        public async Task<IActionResult> AddBareme(ulong allianceid)
        {
            var alliance = ViewBag.allianceContext as Alliance;
            ViewBag.AllEnemies = await _allianceService.GetEnemies(alliance.Id);
            return View();
        }
        [HttpGet]
        [Route("{controller}/{allianceid}/Bareme/Edit/{BaremeId}")]
        public async Task<IActionResult> EditBareme(ulong allianceid, Guid BaremeId)
        {

            var alliance = ViewBag.allianceContext as Alliance;
            ViewBag.AllEnemies = await _allianceService.GetEnemies(alliance.Id);
            var bareme = await _baremeService.GetBareme(alliance.Id, BaremeId);

            var enemies = bareme.Type == BaremeType.Attaque ? bareme.EnemiesAttaque?.Select(e => e.Id) : bareme.EnemiesDefense?.Select(e => e.Id);
            var viewmodel = new BaremeViewModel
            {
                Id = bareme.Id,
                Name = bareme.Name,
                BaremeType = bareme.Type,
                Enemies = bareme.Type.HasValue && enemies != null ? enemies : new List<Guid>(),
                baremeDetails = bareme.Details.Select(d => new BaremeDetailViewModel
                {
                    Id = d.Id,
                    AllyCount = d.NbAllie,
                    EnemyCount = d.NbEnemie,
                    NbPepites = d.NbPepite
                }).ToList()
            };



            return View(viewmodel);
        }
        [HttpGet]
        [Route("{controller}/{allianceid}/Configuration")]
        public async Task<IActionResult> Configuration(ulong allianceid)
        {


            var alliance = ViewBag.allianceContext as Alliance;

            if (alliance.AllianceConfiguration == null)
            {
                return View();
            }
            var model = new AllianceConfigurationBaremeViewModel
            {
                Id = alliance.AllianceConfiguration.Id,
                AtkScreen_DiscordChannelId = alliance.AllianceConfiguration.AtkScreen_DiscordChannelId,
                DefScreen_DiscordChannelId = alliance.AllianceConfiguration.DefScreen_DiscordChannelId,
                ScreenApproverRoleId = alliance.AllianceConfiguration.ScreenApproverRoleId ?? 0,
                Rapport_DiscordChannelId = alliance.AllianceConfiguration.Rapport_DiscordChannelId,
                IsAllianceEnemyRequired = alliance.AllianceConfiguration.IsAllianceEnemyRequired ?? false,
                IsScreenPrepaRequired = alliance.AllianceConfiguration.IsScreenPrepaRequired ?? false,
                Ava_DiscordForumChannelId = alliance.AllianceConfiguration.Ava_DiscordForumChannelId,
                DefaultAttaqueBareme = alliance.AllianceConfiguration.DefaultBaremeAttaqueId,
                DefaultDefBareme = alliance.AllianceConfiguration.DefaultBaremeDefenseId,
                BotScreenBehaviorType = alliance.AllianceConfiguration.BotScreenBehaviorType ?? BotScreenBehaviorType.Commands,
                BehaviorScreenConfigJSONData = alliance.AllianceConfiguration.BehaviorScreenConfigJSONData,
                DefaultSeasonRankingChannelId = alliance.AllianceConfiguration.DefaultSeasonRankingChannelId,
                AutoValidateNodef = alliance.AllianceConfiguration.AutoValidateNoDef ?? false,
                AllowSeasonOverlap = alliance.AllianceConfiguration.AllowSeasonOverlap ?? false
            };

            var baremes = await this._baremeService.GetBaremes(alliance.Id);
            model.Baremes = baremes;

            var enemies = await this._allianceService.GetEnemies(alliance.Id);
            model.Enemies = enemies;

            var channels = await _discordService.GetChannels(allianceid);
            ViewBag.TextChannels = channels;

            var forums = await _discordService.GetChannels(allianceid, ChannelType.Forum);
            ViewBag.Forums = forums;

            var roles = await _discordService.GetRoles(allianceid);
            ViewBag.Roles = roles;




            return View(model);

        }
        [HttpGet]
        [Route("{controller}/{allianceid}/Reports")]
        public async Task<IActionResult> Reports(ulong allianceid)
        {

            var alliance = ViewBag.allianceContext as Alliance;
            var baremes = await _baremeService.GetBaremes(alliance.Id);

            var vm = new SummaryReportViewModel
            {
                SelectedBaremeDefenseId = alliance.AllianceConfiguration.DefaultBaremeDefenseId,
                SelectedBaremeAttaqueId = alliance.AllianceConfiguration.DefaultBaremeAttaqueId,
                Baremes = baremes.Select(b => new BaremeViewModel { Id = b.Id, Name = b.Name, BaremeType = b.Type }).ToArray()
            };


            return View(vm);
        }
        [HttpGet]
        [Route("{controller}/{allianceid}/ScreenDefenses")]

        public async Task<IActionResult> ScreenDefenses()
        {


            return View();
        }
        [HttpGet]
        [Route("{controller}/{allianceid}/ScreenAttaques")]

        public async Task<IActionResult> ScreenAttaques()
        {
            return View();
        }

    }
}
