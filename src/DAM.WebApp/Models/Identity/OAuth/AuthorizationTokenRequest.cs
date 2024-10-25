namespace DAM.WebApp.Models.Identity.OAuth
{
    public class AuthorizationTokenRequest
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }

    }
}
