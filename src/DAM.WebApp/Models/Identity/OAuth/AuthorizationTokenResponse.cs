namespace DAM.WebApp.Models.Identity.OAuth
{
    public class AuthorizationTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public double expires_in { get; set; }
    }
}
