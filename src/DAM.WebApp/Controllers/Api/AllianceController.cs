using DAM.Core.Abstractions.Services;
using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Models.Alliance;
using DAM.WebApp.Requests.Commands.Reports;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAM.WebApp.Controllers.Api
{

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [TypeFilter(typeof(ApiUserAllianceAccess))]
    public class AllianceController : ControllerBase
    {
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IBaremeServiceAsync _baremeService;
        private readonly IReportServiceAsync _reportService;
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;
        private readonly IScreenPostServiceAsync _screenPostService;
        private readonly AllianceContext _dbContext;
        private readonly ISender _sender;
        public AllianceController(IAllianceManagementServiceAsync allianceService,
            IBaremeServiceAsync baremenService,
            IReportServiceAsync reportService,
            AllianceContext dbContextBuilder,
            IScreenPostServiceAsync screenPostService,
            ISender sender
            )
        {

            this._allianceService = allianceService;
            this._baremeService = baremenService;
            this._reportService = reportService;
            this._dbContext = dbContextBuilder;
            this._screenPostService = screenPostService;
            this._sender = sender;
        }
        [HttpPut]
        [Route("api/[controller]/{allianceid}/[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task UpdateConfiguration(ulong allianceid, [FromBody] AllianceConfigurationViewModel configuration)
        {

            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");
            }
            var config = new AllianceConfiguration
            {
                Id = configuration.Id,
                AtkScreen_DiscordChannelId = ulong.Parse(configuration.AtkScreen_DiscordChannelId),
                DefScreen_DiscordChannelId = ulong.Parse(configuration.DefScreen_DiscordChannelId),
                ScreenApproverRoleId = ulong.Parse(configuration.ScreenApproverRoleId),
                Rapport_DiscordChannelId = ulong.Parse(configuration.Rapport_DiscordChannelId),
                Ava_DiscordForumChannelId = string.IsNullOrWhiteSpace(configuration.Ava_DiscordForumChannelId) ? null : ulong.Parse(configuration.Ava_DiscordForumChannelId),
                IsScreenPrepaRequired = configuration.IsScreenPrepaRequired,
                IsAllianceEnemyRequired = configuration.IsAllianceEnemyRequired,
                DefaultBaremeAttaqueId = configuration.DefaultBaremeAttaqueId,
                DefaultBaremeDefenseId = configuration.DefaultBaremeDefenceId,
                BotScreenBehaviorType = configuration.BotScreenBehaviorType,
                DefaultSeasonRankingChannelId = string.IsNullOrWhiteSpace(configuration.DefaultSeasonRankingChannelId) ? null : ulong.Parse(configuration.DefaultSeasonRankingChannelId),
                BehaviorScreenConfigJSONData = configuration.BehaviorScreenConfigJSONData,
                AutoValidateNoDef = configuration.AutoValidateNoDef,
                AllowSeasonOverlap = configuration.AllowSeasonOverlap ?? false
            };
            await _allianceService.UpdateAllianceConfiguration(alliance.Id, config);

        }


        [HttpPost]
        [Route("api/[controller]/{allianceid}/[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task CreateEnemy(ulong allianceid, [FromBody] AddAllianceEnemyViewModel Enemy)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            var newEnemy = new AllianceEnemy
            {
                AllianceId = alliance.Id,
                Name = Enemy.Name
            };

            newEnemy.Id = await this._allianceService.AddEnemy(newEnemy);


        }
        [HttpDelete]
        [Route("api/[controller]/{allianceid}/Enemy/{EnemyId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task DeleteEnemy(ulong allianceid, Guid EnemyId)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());
            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            await this._allianceService.DeleteEnemy(alliance.Id, EnemyId);
        }
        [HttpPut]
        [Route("api/[controller]/{allianceid}/Enemy")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task UpdateEnemy(ulong allianceid, [FromBody] EnemyViewModel viewmodel)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var enemy = new AllianceEnemy
            {
                Id = viewmodel.Id,
                Name = viewmodel.Name,
                AllianceId = alliance.Id
            };
            await this._allianceService.UpdateEnemy(enemy);


        }

        [HttpDelete]
        [Route("api/[controller]/{allianceid}/Bareme/{BaremeId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task DeleteBareme(ulong allianceid, Guid BaremeId)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            await _baremeService.DeleteBareme(BaremeId);

        }
        [HttpPut]
        [Route("api/[controller]/{allianceid}/Bareme")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task UpdateBareme(ulong allianceid, [FromBody] BaremeViewModel viewmodel)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var Bareme = new Bareme
            {
                Id = viewmodel.Id,
                Name = viewmodel.Name,
                AllianceId = alliance.Id,
                Details = viewmodel.baremeDetails.Select(d => new BaremeDetail
                {
                    Id = d.Id,
                    BaremeId = viewmodel.Id,
                    NbPepite = d.NbPepites
                }).ToList(),
                Type = viewmodel.BaremeType
            };
            var enemylist = viewmodel.Enemies?.Select(e => new AllianceEnemy { Id = e });
            if (enemylist != null)
            {
                switch (Bareme.Type)
                {
                    case BaremeType.Attaque:
                        Bareme.EnemiesAttaque = enemylist.ToList();
                        Bareme.EnemiesDefense.Clear();

                        break;
                    case BaremeType.Defense:
                        Bareme.EnemiesDefense = enemylist.ToList();
                        Bareme.EnemiesAttaque.Clear();
                        break;
                    default:
                        break;
                }
            }



            //Todo update enemies list handling;


            await _baremeService.UpdateBareme(Bareme);


        }

        [HttpPut]
        [Route("api/[controller]/{allianceid}/[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task SetValidationResult(ulong allianceid, [FromBody] ScreenValidationViewModel viewmodel)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }


            var discorduserid = ulong.Parse(User.Claims.FirstOrDefault(c => c.Type == "discordId").Value);


            await _sender.Send(new UpdateScreenPostValidationStatusCommand
            {
                ClosedByDiscordUserId = discorduserid.ToString(),
                DiscordGuildId = allianceid,
                Screenkey = viewmodel.ScreenId.ToString(),
                Status = viewmodel.Result
            });




        }

        [HttpPost]
        [Route("api/[controller]/{allianceid}/[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task CreateBareme(ulong allianceid, [FromBody] AddBaremeViewModel Viewmodel)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var bareme = new Bareme
            {
                AllianceId = alliance.Id,
                Name = Viewmodel.Name,
                Type = Viewmodel.BaremeType
            };



            bareme.Id = await _baremeService.CreateBareme(bareme);

            var details = Viewmodel.Details.Select(d =>
            new BaremeDetail
            {
                BaremeId = bareme.Id,
                NbAllie = d.AllyCount,
                NbEnemie = d.EnemyCount,
                NbPepite = d.NbPepites
            }).ToList();
            foreach (var d in details)
            {
                await _baremeService.CreateBaremeDetail(d);
            }
            if (Viewmodel.Enemies != null)
            {
                foreach (var eid in Viewmodel.Enemies)
                {
                    var enemy = await _allianceService.GetEnemy(alliance.Id, eid);
                    switch (bareme.Type)
                    {
                        case BaremeType.Attaque:
                            enemy.BaremeAttaqueId = bareme.Id;

                            break;
                        case BaremeType.Defense:
                            enemy.BaremeDefenseId = bareme.Id;
                            break;
                        default:
                            continue;

                    }

                    await _allianceService.UpdateEnemy(enemy);

                }
            }



        }


        [HttpPost]
        [Route("api/[controller]/{allianceid}/[action]")]
        public async Task<SummaryReport> SummaryReportData(ulong allianceid, [FromBody] SummaryReportRequest payload)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            if (!alliance.AllianceConfiguration.DefaultBaremeAttaqueId.HasValue ||
                !alliance.AllianceConfiguration.DefaultBaremeDefenseId.HasValue)
            {
                throw new BadHttpRequestException("La configuration de l'alliance doit possèder un barème d'attaque par défaut ainsu qu'un barème de défense par défaut.");
            }

            var baremeAtk = payload.BaremeAttaqueId ?? alliance.AllianceConfiguration.DefaultBaremeAttaqueId.Value;
            var baremeDef = payload.BaremeDefenseId ?? alliance.AllianceConfiguration.DefaultBaremeDefenseId.Value;

            var multiplier = payload.Multiplier ?? 1;
            var rows = await _reportService.GetReportData(alliance.Id, baremeAtk, baremeDef, payload.From.ToUniversalTime(), payload.To.ToUniversalTime());
            var vmRows = rows.Select(r => new SummaryReportRowViewModel(r)).ToList();
            var report = new SummaryReport
            {
                TotalPepitesAtk = vmRows.Sum(r => r.MontantAtkPepites) * multiplier,
                TotalPepitesDef = vmRows.Sum(r => r.MontantDefPepites) * multiplier,
                TotalPepitesAvA = vmRows.Sum(r => r.MontantAvAPepites) * multiplier,
                TotalPepites = vmRows.Sum(r => r.MontantTotalPepites) * multiplier,
                MoyennePepiteParJoueur = vmRows.Any() ? vmRows.Average(r => r.MontantTotalPepites) * multiplier : 0,
                data = vmRows.OrderByDescending(r => r.MontantTotalPepites).Select(c =>
                {
                    c.MontantDefPepites = c.MontantDefPepites * multiplier;
                    c.MontantAvAPepites = c.MontantAvAPepites * multiplier;
                    c.MontantAtkPepites = c.MontantAtkPepites * multiplier;
                    c.MontantTotalPepites = c.MontantTotalPepites * multiplier;
                    return c;

                }),
            };
            return report;


        }
        [HttpPost]
        [Route("api/[controller]/{allianceid}/[action]")]
        public async Task PublishReport(ulong allianceid, [FromBody] SummaryReportRequest payload)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            if (!alliance.AllianceConfiguration.Rapport_DiscordChannelId.HasValue)
            {
                throw new BadHttpRequestException($"L'alliance n'a pas de canal rapport pépite de configuré");
            }

            var command = new PublishReportCommand
            {
                AllianceId = alliance.Id,
                From = payload.From,
                To = payload.To,
                DiscordChannelId = alliance.AllianceConfiguration.Rapport_DiscordChannelId.Value,
                BaremeAttaqueId = payload.BaremeAttaqueId ?? alliance.AllianceConfiguration.DefaultBaremeAttaqueId.Value,
                BaremeDefenseId = payload.BaremeDefenseId ?? alliance.AllianceConfiguration.DefaultBaremeDefenseId.Value,
                Multipler = payload.Multiplier ?? 1

            };

            await _sender.Send(command);
        }





    }
}
