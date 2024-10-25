using Cocona;
using DAM.Core;
using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Core.Abstractions.Validator;
using DAM.Core.Mappings;
using DAM.Core.Validators;
using DAM.Data.EntityFramework.Services;
using DAM.Database;
using DAM.Tool.DataMigrations.Commands;
using DAM.Tool.DataMigrations.Migrations;
using DAM.Tool.DataMigrations.Services;
using Discord;
using Discord.Rest;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var prod_token = "";
            var client = new DiscordRestClient();
            await client.LoginAsync(TokenType.Bot, prod_token);
            var builder = CoconaApp.CreateBuilder();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddSingleton<DiscordRestClient>(client);
            builder.Services.ConfigureDAMDatabase(builder.Configuration).ConfigureDAMCore(string.Empty);

            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            // scans the assembly and gets the IRegister, adding the registration to the TypeAdapterConfig
            typeAdapterConfig.Scan(typeof(MapsterDAMMapper).Assembly);
            // register the mapper as Singleton service for my application
            var mapperConfig = new Mapper(typeAdapterConfig);
            builder.Services.AddSingleton<IMapper>(mapperConfig)
                    .AddSingleton<IDAMMapper, MapsterDAMMapper>();
            builder.Services.AddTransient<IAllianceManagementServiceAsync, EFAllianceManagementService>()
                            .AddTransient<IScreenPostServiceAsync, EFScreenPostService>()
                            .AddTransient<ISaisonServiceAsync, EFSaisonService>().AddTransient<IDiscordBotService, RestDiscordBotService>();
            builder.Services.AddTransient<IReportServiceAsync, EFReportService>().AddTransient<ISaisonValidator, AppSaisonValidator>();


            var app = builder.Build();
            app.AddCommands<MigrateDiscordMessagesToDAM>();
            app.AddCommands<CustomLadderCommands>();
            await app.RunAsync();


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }



}




