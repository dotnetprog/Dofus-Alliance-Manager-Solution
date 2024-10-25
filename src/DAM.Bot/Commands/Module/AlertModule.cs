using DAM.Bot.Common.Extensions;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DAM.Bot.Commands.Module
{
    public class AlertModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IAlertServiceAsync _alertService;
        private readonly ILogger _logger;
        public AlertModule(IAllianceManagementServiceAsync allianceManagementService,
                           IAlertServiceAsync alertService,
                           ILogger<AlertModule> logger)
        {
            this._allianceManagementService = allianceManagementService;
            this._alertService = alertService;
            this._logger = logger;
        }
        private Embed BuildErrorMessage(string message)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Une erreur est survenue.")
                .WithDescription(message);

            builder.WithColor(Color.Red);
            return builder.Build();
        }
        private Embed GetInProgressMessage()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Traitement en cours..");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }
        private async Task<AllianceMember> GetOrCreateAllianceMember(Alliance alliance, IGuildUser user)
        {
            try
            {
                var member = new AllianceMember()
                {
                    Alias = user.Username,
                    DiscordId = user.Id.ToString(),
                    AllianceId = alliance.Id,
                    Nickname = user.Nickname,
                    DonotAlert = false
                };
                member.Id = await _allianceManagementService.AddMember(member);
                return member;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                throw;
            }
        }
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("alert_everyone", "Cette commande sert à envoyer un message sur tous les membres su serveur.", false, RunMode.Async)]
        public async Task Run(string message)
        {
            await this.RespondAsync(text: "Cette commande ne fait rien pour l'instant", ephemeral: true);
            return;
            await this.RespondAsync(embed: GetInProgressMessage(), ephemeral: true);

            var users = this.Context.Guild.Users.Where(u => !u.IsBot).ToList();

            var alliance = await this._allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());
            if (alliance == null)
            {
                return;
            }

            var mostRecentAlert = await this._alertService.GetMostRecentAlert(alliance.Id);
            if (mostRecentAlert != null && mostRecentAlert.CreatedOn.Date == DateTime.UtcNow.Date)
            {
                await this.FollowupAsync(embed: BuildErrorMessage("Une seule alerte par jour est permise."), ephemeral: true);
                return;
            }

            var RegisteredMembers = await this._allianceManagementService.GetAllMembers(alliance.Id);
            var createdBy = await GetOrCreateAllianceMember(alliance, this.Context.User as SocketGuildUser);

            var donotalertUsers = RegisteredMembers.Where(a => a.DonotAlert == true).Select(a => ulong.Parse(a.DiscordId)).ToList();


            var usersToAlert = users.Where(u => !donotalertUsers.Any(du => du == u.Id)).ToList();

            var alert = await this._alertService.CreateAlert(message, createdBy.Id, alliance.Id, usersToAlert.Count);


            //foreach(var time in Enumerable.Range(0, 30))
            //{
            //    var tag = RandomString(5);
            //    await this.Context.User.SendMessageAsync($"{message} - {tag}");
            //    await Task.Delay(2000);
            //}

            var batches = usersToAlert.Batch(25);
            var counter = 0;
            try
            {
                foreach (var batch in batches)
                {
                    foreach (var user in batch)
                    {
                        var tag = RandomString(5);
                        await user.SendMessageAsync($"{message} - {tag}");
                        counter++;
                        await Task.Delay(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{counter} users has been notified");
                throw;

            }

            await this.ReplyAsync(embed: BuildEmbed(alert));
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        protected Embed BuildEmbed(Alert data)
        {
            var embed = new EmbedBuilder()
            {
                Title = $"Alerte effectué",
                Description = $"{Context.User.Mention} {data.AudienceCount ?? 0} utilisateurs ont été contactés"

            };

            embed.WithAuthor(Context.User)
                .AddField("Message: ", data.Message)
                .AddField("Identifiant de l'alerte: ", data.Id.ToString());

            embed.WithColor(Color.DarkPurple)
                .WithCurrentTimestamp();
            return embed.Build();

        }

    }
}
