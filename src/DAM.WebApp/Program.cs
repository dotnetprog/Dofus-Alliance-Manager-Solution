using Asp.Versioning;
using DAM.Core;
using DAM.Core.Abstractions.Identity;
using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Core.Abstractions.Validator;
using DAM.Core.Mappings;
using DAM.Core.Validators;
using DAM.Data.EntityFramework.Identity;
using DAM.Data.EntityFramework.Services;
using DAM.Database;
using DAM.Domain.Entities;
using DAM.Domain.Ïdentity;
using DAM.WebApp.Middlewares;
using DAM.WebApp.OAuth.Discord;
using DAM.WebApp.OAuth.Discord.Services;
using DAM.WebApp.Requests.Commands.Reports;
using DAM.WebApp.Services;
using DAM.WebApp.Swagger;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


static IEdmModel CreateODataModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<DefScreensPost>("DefScreensPosts");
    builder.EntitySet<AtkScreensPost>("AtkScreensPosts");
    builder.EntitySet<AllianceMember>("AllianceMembers");
    builder.EntitySet<AllianceMember_ScreenPost>("AllianceMemberScreenPosts");
    builder.EntitySet<AvA>("AvAs");
    builder.EntitySet<AvaMember>("AvAMembers");
    builder.EntitySet<Saison>("Saisons");
    builder.EntitySet<Saison>("Saisons").EntityType.Action("PublishRanking");
    builder.EntitySet<Saison>("Saisons").EntityType.Action("GenerateRankings");
    builder.EntitySet<SaisonRanking>("SaisonRankings");
    builder.EntitySet<AppUser>("AppUsers").EntityType.Ignore(ui => ui.HashedClientSecret);
    builder.EntitySet<AppUser>("AppUsers").EntityType.Action("SetClientSecret").Returns<string>();
    return builder.GetEdmModel();
}
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var issuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
var Audience = builder.Configuration.GetValue<string>("Jwt:Audiance");
var encryptkey = builder.Configuration.GetValue<string>("Jwt:EncryptionKey");
var ClientId = builder.Configuration.GetValue<string>("Discord:ClientId");
var Secret = builder.Configuration.GetValue<string>("Discord:ClientSecret");
var botToken = builder.Configuration.GetValue<string>("Discord:BotToken");

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.ConfigureDAMDatabase(builder.Configuration).ConfigureDAMCore(issuer, typeof(PublishReportCommandHandler).Assembly);

builder.Services.AddSingleton<IDiscordBotService, DiscordBotService>((sp) =>
{
    var client = new DiscordBotService(botToken);
    return client;
});
var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
// scans the assembly and gets the IRegister, adding the registration to the TypeAdapterConfig
typeAdapterConfig.Scan(Assembly.GetExecutingAssembly(), typeof(MapsterDAMMapper).Assembly);
// register the mapper as Singleton service for my application
var mapperConfig = new Mapper(typeAdapterConfig);
builder.Services.AddSingleton<IMapper>(mapperConfig)
        .AddSingleton<IDAMMapper, MapsterDAMMapper>();
builder.Services.AddTransient<IAllianceManagementServiceAsync, EFAllianceManagementService>()
                .AddTransient<IScreenPostServiceAsync, EFScreenPostService>()
                .AddTransient<ISaisonServiceAsync, EFSaisonService>()
                .AddTransient<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>()
                .AddTransient<IAppUserIdentityManagerAsync, EFAppUsersManagerAsync>()
                .AddTransient<IBaremeServiceAsync, EFBaremeService>();
builder.Services.AddTransient<IReportServiceAsync, EFReportService>();
builder.Services.AddTransient<IAvAService, EFAvaService>();
builder.Services.AddScoped<IAllianceMemberIdentityService, HttpContextAllianceMemberIdentityService>();
builder.Services.Decorate<IDiscordBotService, CachedDiscordBotService>().AddTransient<ISaisonValidator, AppSaisonValidator>();

builder.Services.AddControllersWithViews().AddOData(options =>
{
    options.Select().Filter().Count().OrderBy().Expand().EnableQueryFeatures();

    options.AddRouteComponents("odata/v1/{allianceid}", CreateODataModel());
    options.RouteOptions.EnableKeyInParenthesis = true;
    options.RouteOptions.EnableNonParenthesisForEmptyParameterFunction = true;
    options.RouteOptions.EnableQualifiedOperationCall = false;
    options.RouteOptions.EnableUnqualifiedOperationCall = true;


});
builder.Services.AddSingleton<IEdmModel>(CreateODataModel());
builder.Services.AddMvcCore().AddApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader()
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;   // Removes v{version} routes from swagger

});

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUserServiceAsync, DiscordUserService>();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddSwaggerGen(c =>
{

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

});
builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//"Discord";
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie()
.AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptkey))
    };
}).AddOAuth("Discord", options =>
{
    options.AuthorizationEndpoint = "https://discord.com/oauth2/authorize";
    options.Scope.Add(DiscordScopes.Identify);
    options.Scope.Add(DiscordScopes.Guilds);

    options.CallbackPath = new PathString("/auth/oauthCallback");
    options.ClientId = ClientId;
    options.ClientSecret = Secret;
    options.TokenEndpoint = "https://discord.com/api/oauth2/token";
    options.UserInformationEndpoint = "https://discord.com/api/users/@me";
    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
    options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "global_name");
    options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
    {
        var avatar = user.GetString("avatar");
        if (string.IsNullOrWhiteSpace(avatar))
        {
            return string.Empty;
        }
        return string.Format(
        CultureInfo.InvariantCulture,
        "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
        user.GetString("id"),
        avatar,
        avatar.StartsWith("a_") ? "gif" : "png");
    });

    options.AccessDeniedPath = "/Home/DiscordAuthFailed";
    options.SaveTokens = true;
    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var resultObject = JsonDocument.Parse(json).RootElement;
            context.RunClaimActions(resultObject);

        }

    };

});



var useSwaggerRaw = builder.Configuration.GetValue<string>("Swagger:Enabled");

var app = builder.Build();

// Configure the db context and user manager to use a single instance per request

// Configure the HTTP request pipeline.

if (!string.IsNullOrWhiteSpace(useSwaggerRaw) && bool.TryParse(useSwaggerRaw, out bool useSwagger) && useSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        //var descriptions = app.DescribeApiVersions();


        //foreach (var description in descriptions)
        //{
        //    options.SwaggerEndpoint(
        //        $"/swagger/{description.GroupName}/swagger.json",
        //        description.GroupName.ToUpperInvariant());
        //}
        // Add UI dropdown to select all Swagger doc versions
        options.SwaggerEndpoint(
               $"/swagger/v1/swagger.json",
               "V1");
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseODataRouteDebug();
    app.UseMigrationsEndPoint();

}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseAuthorization();
//app.MapControllerRoute(name: "alliance",
//                pattern: "alliance/{guildid?:int}/{action}",
//                defaults: new { controller = "Alliance", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();
app.UseStatusCodePagesWithReExecute("/Error/HandleError/{0}");

app.Run();

