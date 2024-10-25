using DAM.Core.Abstractions.Identity;
using DAM.WebApp.Models.Identity.OAuth;
using DAM.WebApp.Models.Identity.OAuth.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DAM.WebApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAppUserIdentityManagerAsync _appUserIdentityManager;

        public LoginController(IConfiguration configuration, IAppUserIdentityManagerAsync appUserIdentityManager)
        {

            _configuration = configuration;
            _appUserIdentityManager = appUserIdentityManager;

        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Discord")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object Token()
        {
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var issuer = _configuration.GetValue<string>("Jwt:Issuer");
            var Audience = _configuration.GetValue<string>("Jwt:Audiance");
            var encryptkey = _configuration.GetValue<string>("Jwt:EncryptionKey");

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptkey));
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("discordId", userId));
            permClaims.Add(new Claim("type", "discord"));

            var token = new JwtSecurityToken(
                issuer,
                Audience,
                permClaims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return new
            {
                ApiToken = jwt_token
            };
        }

        [HttpPost]
        public async Task<AuthorizationTokenResponse> Token([FromBody] AuthorizationTokenRequest request)
        {

            if (string.IsNullOrWhiteSpace(request.client_secret) ||
                           string.IsNullOrWhiteSpace(request.grant_type)
                           )
            {
                throw new OauthInvalidRequestException("The request is missing parameters.No parameters should be empty.");
            }
            Guid clientId;
            if (string.IsNullOrEmpty(request.client_id) || !Guid.TryParse(request.client_id, out clientId))
            {
                throw new OauthInvalidRequestException("The request is incorrect.client_id is not in a valid format.");
            }


            if (request.grant_type != "client_credentials")
            {
                throw new OauthUnsupportedGrantTypeException($"the grant type '{request.grant_type}' is not supported");
            }




            var user = await _appUserIdentityManager.SignInAsync(clientId, request.client_secret);
            if (user == null)
            {
                throw new OauthInvalidClientException($"The clientid and/or the client secret are incorrect.");
            }


            var userId = user.Id;
            var issuer = _configuration.GetValue<string>("Jwt:Issuer");
            var Audience = _configuration.GetValue<string>("Jwt:Audiance");
            var encryptkey = _configuration.GetValue<string>("Jwt:EncryptionKey");

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptkey));
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("username", user.Username));
            permClaims.Add(new Claim("alliancediscordid", user.Alliance.DiscordGuildId));
            permClaims.Add(new Claim("allianceid", user.AllianceId.ToString()));
            permClaims.Add(new Claim("type", "oauth"));


            var timespan = new TimeSpan(7, 0, 0, 0);

            var token = new JwtSecurityToken(
                issuer,
                Audience,
                permClaims,
                expires: DateTime.Now.Add(timespan),
                signingCredentials: credentials
            );

            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthorizationTokenResponse
            {
                access_token = jwt_token,
                token_type = "Bearer",
                expires_in = timespan.TotalSeconds
            };
        }


    }
}
