using DAM.Bot.Commands.ComplexTypes;
using DAM.Bot.Common.Helper;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DAM.Bot.Commands.Module.Screen
{
    public abstract class T5ScreenBaseModule<T> : InteractionModuleBase<SocketInteractionContext> where T : ScreenCommandInput
    {
        protected readonly IScreenPostServiceAsync _screenService;
        protected readonly IAllianceManagementServiceAsync _allianceManagementService;
        protected readonly ILogger _logger;
        protected readonly ScreenPostType _screentype;
        protected T5ScreenBaseModule(ScreenPostType screenType,
            IScreenPostServiceAsync ScreenService,
            IAllianceManagementServiceAsync allianceManagementService,
            ILogger logger)
        {
            _screentype = screenType;
            _screenService = ScreenService;
            _allianceManagementService = allianceManagementService;
            _logger = logger;
        }
        protected abstract Embed[] BuildEmbed(T data, Guid screenId);
        protected Embed BuildErrorMessage(string message)
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

        public virtual async Task<bool> OnPreValidationExecute(Alliance alliance, T data)
        {
            return await Task.FromResult(true);
        }
        public virtual async Task Run([ComplexParameter] T data)
        {
            await RespondAsync(embed: GetInProgressMessage(), ephemeral: true);




            var alliance = await ConfigureAlliance();
            if (alliance.AllianceConfiguration != null)
            {
                var canalConfig = _screentype == ScreenPostType.Attack ?
                    alliance.AllianceConfiguration.AtkScreen_DiscordChannelId :
                    alliance.AllianceConfiguration.DefScreen_DiscordChannelId;
                if (Context.Channel.Id != canalConfig)
                {
                    try
                    {
                        await FollowupAsync(embed: BuildErrorMessage($"Impossible d'enregistrer votre screen dans ce canal."), ephemeral: true);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        return;
                    }

                }
                var isEnemyRequired = alliance.AllianceConfiguration.IsAllianceEnemyRequired ?? false;
                if (isEnemyRequired && string.IsNullOrWhiteSpace(data.AllianceEnnemi))
                {
                    await FollowupAsync(embed: BuildErrorMessage($"L'alliance ennemie est obligatoire."), ephemeral: true);
                    return;
                }

            }
            if (!await OnPreValidationExecute(alliance, data))
            {
                return;
            }
            var member = await this._allianceManagementService.GetOrCreateAllianceMember(alliance.Id, Context.User as SocketGuildUser);



            var post = data.ConvertToScreenPost();
            post.CreatedByMemberId = member.Id;
            post.Type = _screentype;

            if (!string.IsNullOrEmpty(data.AllianceEnnemi) && Guid.TryParse(data.AllianceEnnemi, out Guid enemyId))
            {
                var enemy = await _allianceManagementService.GetEnemy(alliance.Id, enemyId);
                if (enemy == null)
                {
                    await FollowupAsync(embed: BuildErrorMessage($"L'alliance ennemie sélectionné est invalide ou n'existe plus"), ephemeral: true);
                    return;
                }
                data.Enemy = enemy;
                post.AllianceEnemyId = enemyId;
            }



            var members = new List<AllianceMember>();
            var attaquants = data.GetAttaquants().ToList();
            foreach (var attaquant in attaquants)
            {
                members.Add(await _allianceManagementService.GetOrCreateAllianceMember(alliance.Id, attaquant));
            }
            try
            {

                post.Id = await _screenService.CreatePost(post, members.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }


            var userMessage = await FollowupAsync(embeds: BuildEmbed(data, post.Id));


            if (attaquants.Count < data.NombreEnemies)
            {
                await userMessage.AddReactionAsync(EmojiHelper.Muscle);
            }
            post.DiscordChannelId = Context.Channel.Id;
            post.DiscordMessageId = userMessage.Id;


            await _screenService.UpdatePost(post);





        }
        private async Task<Alliance> ConfigureAlliance()
        {
            var alliance = await _allianceManagementService.GetAlliance(Context.Guild.Id.ToString());
            if (alliance == null)
            {
                alliance = await _allianceManagementService.RegisterAlliance(Context.Guild.Id.ToString(), Context.Guild.Name);
            }

            return alliance;
        }

    }
}
