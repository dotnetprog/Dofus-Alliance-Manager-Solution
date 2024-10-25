using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Core.Requests.Commands;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Models.Saison;
using DAM.WebApp.OAuth.Discord.Services;
using Discord;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAM.WebApp.Controllers
{

    [RequireHttps]
    [Authorize(AuthenticationSchemes = "Discord")]
    [TypeFilter(typeof(CheckAlliianceRegistration), Order = 2)]
    [TypeFilter(typeof(AllianceDataFilter), Order = 1)]
    [Route("{controller}/{allianceid}/{action}")]
    public class SaisonController : Controller
    {
        private readonly IUserServiceAsync _userService;
        private readonly IDiscordBotService _discordService;
        private readonly IBaremeServiceAsync _baremeServiceAsync;
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly ISaisonServiceAsync _saisonService;
        private readonly ISender _sender;
        private readonly IDAMMapper _mapper;
        public SaisonController(IUserServiceAsync userService,
            IDiscordBotService botService,
            IAllianceManagementServiceAsync allianceService,
            ISaisonServiceAsync saisonService,
            ISender sender,
            IDAMMapper mapper,
            IBaremeServiceAsync baremeServiceAsync
            )
        {
            this._userService = userService;
            this._discordService = botService;
            this._allianceService = allianceService;
            this._saisonService = saisonService;
            this._baremeServiceAsync = baremeServiceAsync;
            this._sender = sender;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var alliance = ViewBag.allianceContext as Alliance;
            var channels = await _discordService.GetChannels(ulong.Parse(alliance.DiscordGuildId), ChannelType.Text);
            ViewBag.TextChannels = channels;

            ViewBag.Baremes = await _baremeServiceAsync.GetBaremes(alliance.Id);


            var model = new CreateSaisonViewModel()
            {
                SelectedChannelId = alliance.AllianceConfiguration.DefaultSeasonRankingChannelId?.ToString(),
                BaremeAttackId = alliance.AllianceConfiguration.DefaultBaremeAttaqueId,
                BaremeDefenseId = alliance.AllianceConfiguration.DefaultBaremeDefenseId
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(CreateSaisonViewModel model)
        {
            var alliance = ViewBag.allianceContext as Alliance;
            var channels = await _discordService.GetChannels(ulong.Parse(alliance.DiscordGuildId), ChannelType.Text);
            ViewBag.TextChannels = channels;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userCurrentDiscordId = ulong.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var discordGuildUser = await _discordService.GetGuildUser(ulong.Parse(alliance.DiscordGuildId), userCurrentDiscordId);

            var member = await _allianceService.GetOrCreateAllianceMember(alliance.Id, discordGuildUser);

            var request = _mapper.Map<CreateSaisonCommand, CreateSaisonViewModel>(model);
            request.CreatedByUserId = member.Id;
            request.AllianceId = alliance.Id;
            request.AllowOverlap = alliance.AllianceConfiguration.AllowSeasonOverlap ?? false;
            request.EndDate = request.EndDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMicroseconds(999000);
            try
            {
                var SaisonId = await _sender.Send(request);
                return RedirectToAction("Edit", new { Id = SaisonId });
            }
            catch (ValidationException error)
            {

                foreach (var errorResult in error.Errors)
                {
                    ModelState.AddModelError(errorResult.PropertyName, errorResult.ErrorMessage);
                }
                ViewBag.Baremes = await _baremeServiceAsync.GetBaremes(alliance.Id);
                return View(model);
            }





        }


        [Route("{Id}")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            //TODO: IMPLEMENT VIEW
            //IMPLEMENT GRID RANKING - Partially Done
            //IMPLEMENT RANKING GENERATION 
            //IMPLEMENT PUBLISHING
            //IMPLEMENT VIEW FOR A MEMBER TO SEE ALL RELATED SCREENS TO HIS CALCULATION

            //Refactor using Mediatr and Mapping

            var alliance = ViewBag.allianceContext as Alliance;
            var season = await this._saisonService.GetById(alliance.Id, Id);
            var channels = await _discordService.GetChannels(ulong.Parse(alliance.DiscordGuildId), ChannelType.Text);
            ViewBag.TextChannels = channels;
            ViewBag.Baremes = await _baremeServiceAsync.GetBaremes(alliance.Id);
            var vm = new EditSaisonViewModel
            {
                Id = Id,
                Description = season.Description,
                EndDate = season.EndDate,
                StartDate = season.StartDate,
                Name = season.Name,
                SelectedChannelId = season.SeasonRankingChannelId?.ToString(),
                BaremeAttackId = season.BaremeAttackId,
                BaremeDefenseId = season.BaremeDefenseId
            };

            return View(vm);
        }

        [AllowAnonymous]
        [Route("{Id}")]
        [HttpGet]
        public async Task<IActionResult> DetailRanking(Guid Id)
        {
            var alliance = ViewBag.allianceContext as Alliance;

            var rankingInfo = await _saisonService.GetRanking(alliance.Id, Id);
            if (rankingInfo == null)
            {
                return NotFound();
            }
            var saisonInfo = await _saisonService.GetById(alliance.Id, rankingInfo.SaisonId);

            ViewBag.SaisonInfo = saisonInfo;


            return View(rankingInfo);


        }

        [AllowAnonymous]
        [Route("~/{controller}/{allianceid}/{SaisonId}/{action}/{playerName}")]
        [HttpGet]
        public async Task<IActionResult> Player(Guid SaisonId, string playerName)
        {
            var alliance = ViewBag.allianceContext as Alliance;


            var ranking = await _saisonService.GetRanking(alliance.Id, SaisonId, playerName);
            if (ranking == null)
            {
                return NotFound();
            }
            return RedirectToAction("DetailRanking", new { allianceid = alliance.DiscordGuildId, Id = ranking.Id });
        }


    }
}
