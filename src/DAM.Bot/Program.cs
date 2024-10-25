using AnkamaWebClient.Abstractions;
using AnkamaWebClient.Client;
using DAM.Bot;
using DAM.Bot.Commands;
using DAM.Bot.EventSubscribers.Message;
using DAM.Bot.Services.Discord;
using DAM.Core;
using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Core.Abstractions.Validator;
using DAM.Core.Mappings;
using DAM.Core.Validators;
using DAM.Data.Cached;
using DAM.Data.EntityFramework.Services;
using DAM.Database;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Mapster;
using MapsterMapper;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
    })
    .ConfigureServices((hostContext, services) =>
    {
        var config = new DiscordSocketConfig()
        {
            AlwaysDownloadUsers = true,
            GatewayIntents = GatewayIntents.All,
            LogLevel = LogSeverity.Info,
            UseInteractionSnowflakeDate = false
        };
        var client = new DiscordSocketClient(config);
        var interactionService = new InteractionService(client, new InteractionServiceConfig()
        {
            // Again, log level:
            LogLevel = LogSeverity.Info

        });
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        // scans the assembly and gets the IRegister, adding the registration to the TypeAdapterConfig
        typeAdapterConfig.Scan(typeof(MapsterDAMMapper).Assembly);
        var webappurl = hostContext.Configuration.GetValue<string>("WebAppUrl");
        var mapperConfig = new Mapper(typeAdapterConfig);
        services.AddSingleton<IMapper>(mapperConfig)
        .AddSingleton<IDAMMapper, MapsterDAMMapper>();
        services.ConfigureDAMDatabase(hostContext.Configuration)
                .ConfigureDAMCore(webappurl, typeof(CreateScreenOnMessageCreationSubscriber).Assembly)
                .AddHostedService<Worker>()
                .AddSingleton<DiscordSocketClient>(client)
                .AddSingleton<IDiscordBotService, DiscordSocketService>()
                .AddSingleton<InteractionService>(interactionService)
                .AddSingleton<ICommandHandler, CommandHandler>()
                .AddSingleton<IAllianceManagementServiceAsync, EFAllianceManagementService>()
                .Decorate<IAllianceManagementServiceAsync, CachedAllianceManagementService>()
                .AddTransient<IScreenPostServiceAsync, EFScreenPostService>()
                .AddTransient<IAlertServiceAsync, EFAlertService>()
                .AddTransient<ISaisonServiceAsync, EFSaisonService>()
                .AddTransient<ISaisonValidator, AppSaisonValidator>()
                .AddTransient<IReportServiceAsync, EFReportService>()
                .AddTransient<IAvAService, EFAvaService>()
                .AddTransient<IBaremeServiceAsync, EFBaremeService>()
                .AddSingleton<IAnkamaService, AnkamaWebService>()
                .AddTransient<IMembreService, EFMembreService>();
        services.AddMemoryCache();
    })
    .Build();

await host.RunAsync();
