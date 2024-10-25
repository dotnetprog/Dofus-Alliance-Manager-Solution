using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Models.AvA;
using DAM.WebApp.OAuth.Discord.Services;
using DAM.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAM.WebApp.Controllers
{
    [RequireHttps]
    [Authorize(AuthenticationSchemes = "Discord")]
    [TypeFilter(typeof(CheckAlliianceRegistration), Order = 2)]
    [TypeFilter(typeof(AllianceDataFilter), Order = 1)]
    [Route("{controller}/{allianceid}/{action}")]
    public class AvaController : Controller
    {
        private readonly IUserServiceAsync _userService;
        private readonly IDiscordBotService _discordService;
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IAvAService _avaService;
        public AvaController(IUserServiceAsync userService,
            IDiscordBotService botService,
            IAllianceManagementServiceAsync allianceService,
            IAvAService avaService
            )
        {
            this._userService = userService;
            this._discordService = botService;
            this._allianceService = allianceService;
            this._avaService = avaService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        [Route("{Id}")]
        public async Task<IActionResult> Summary(Guid Id)
        {
            var alliance = ViewBag.allianceContext as Alliance;
            var ava = await _avaService.GetAvA(alliance.Id, Id,true);

            var nbParticipants = ava.Members.Where(m => m.ValidationState != AvAValidationState.Rejected).Count();

            var IsFixe = ava.MontantPayeFixe.HasValue;
            var parjoueur = 0;
            if (ava.ResultState.HasValue)
            {
                if (!IsFixe )
                {
                    var percentage = (ava.PourcentagePaye.Value / 100);
                    var totalRedistrib = ava.MontantPepitesObtenu.HasValue ? Math.Round(ava.MontantPepitesObtenu.Value * percentage, 0, MidpointRounding.ToNegativeInfinity): 0;
                    parjoueur = nbParticipants > 0 && totalRedistrib > 0? Convert.ToInt32(Math.Round(totalRedistrib / nbParticipants, 0, MidpointRounding.ToNegativeInfinity)):0;

                }
                else
                {
                    parjoueur = ava.MontantPayeFixe.Value;

                }
            }


            var viewmodel = new AvAViewModel
            {
                Id = ava.Id,
                ClosedById = ava.ClosedById,
                ClosedByName = ava.ClosedBy?.Nickname ?? ava.ClosedBy?.Alias,
                ClosedOn = ava.ClosedOn,
                CreatedById = ava.CreatedById,
                CreatedByName = ava.CreatedBy.Nickname ?? ava.CreatedBy.Alias,
                CreatedOn = ava.CreatedOn,
                title = ava.Titre,
                MontantTotal = ava.MontantPepitesTotal,
                MontantObtenu = ava.MontantPepitesObtenu,
                NbParticipants = nbParticipants,
                Statut = ava.ResultState,
                MontantParJoueur = parjoueur,
                Pourcentage = ava.PourcentagePaye,
                State = ava.State ?? AvaState.Open

            };

            


            return View(viewmodel);
        }
    }
}
