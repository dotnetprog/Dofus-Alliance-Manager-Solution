using DAM.Bot.Commands;
using DAM.Bot.EventHandlers;
using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Discord.Message;
using Discord;
using Discord.WebSocket;
using MediatR;

namespace DAM.Bot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICommandHandler commandHandler;
        private readonly DiscordSocketClient _client;
        private IConfiguration _configuration;
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IAvAService _avaService;
        private readonly IScreenPostServiceAsync _screenService;
        private readonly IPublisher _publisher;
        public Worker(IAvAService AvaService, IAllianceManagementServiceAsync allianceService,
                      IScreenPostServiceAsync screenService,
            ILogger<Worker> logger, ICommandHandler cmdHandler, DiscordSocketClient client, IConfiguration configuration, IPublisher publisher)
        {
            _logger = logger;
            this._avaService = AvaService;
            this.commandHandler = cmdHandler;
            this._client = client;
            _configuration = configuration;
            this._screenService = screenService;
            this._allianceService = allianceService;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {




            _client.Ready += async () =>
            {
                await Task.Run(() => _logger.LogInformation($"Bot is connected and ready!"));
                await commandHandler.InitializeAsync();

            };
            _client.MessageReceived += async (sm) =>
            {
                await _publisher.Publish(new OnMessageCreatedEvent() { SocketMessage = sm });
            };
            _client.MessageUpdated += async (cm, sm, c) =>
            {
                var message = cm.HasValue ? cm.Value : sm;
                await _publisher.Publish(new OnMessageUpdatedEvent() { Message = message });
            };
            _client.MessageDeleted += async (m, c) =>
            {

                await _publisher.Publish(new OnMessageDeletedEvent()
                {
                    DiscordMessageId = m.Id,
                    ChannelId = c.Id
                });
            };
            _client.ReactionAdded += async (m, c, r) =>
            {
                var user = r.User.GetValueOrDefault() as IGuildUser;
                if (user == null)
                {
                    return;
                }
                if (!r.User.Value.IsBot && !r.User.Value.IsWebhook)
                {

                    await Task.WhenAll(
                         _publisher.Publish(new OnMessageReactEvent()
                         {
                             Message = await m.GetOrDownloadAsync(),
                             Reaction = r
                         }),
                        OnMessageReaction.Handle(_client, r, _avaService, _logger, _allianceService),
                        OnMessageReaction.Handle(_client,
                        r, _screenService, _logger, _allianceService));
                }
            };
            // Login and connect.

            var token = _configuration.GetRequiredSection("Settings")["DiscordBotToken"];
            if (string.IsNullOrWhiteSpace(token))
            {
                //await Logger.Log(LogSeverity.Error, $"{nameof(Program)} | {nameof(MainAsync)}", "Token is null or empty.");
                _logger.LogError($"{nameof(Worker)} | {nameof(ExecuteAsync)}", "Token is null or empty.");
                return;
            }

            await _client.LoginAsync(TokenType.Bot, token, true);
            await _client.StartAsync();

            _client.Log += async (msg) =>
            {

                await Task.Run(() => _logger.LogDebug(msg.ToString()));
            };

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite).WaitAsync(stoppingToken);

        }
    }
}