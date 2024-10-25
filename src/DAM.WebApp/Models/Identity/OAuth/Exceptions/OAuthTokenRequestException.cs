namespace DAM.WebApp.Models.Identity.OAuth.Exceptions
{

    public abstract class OAuthTokenRequestException : Exception
    {
        public string error { get; set; }

        protected OAuthTokenRequestException(string error, string message) : base(message)
        {
            this.error = error;
        }


    }


    public class OauthInvalidRequestException : OAuthTokenRequestException
    {
        public OauthInvalidRequestException(string message) : base("invalid_request", message)
        {

        }
    }
    public class OauthInvalidClientException : OAuthTokenRequestException
    {
        public OauthInvalidClientException(string message) : base("invalid_client", message)
        {

        }
    }
    public class OauthUnsupportedGrantTypeException : OAuthTokenRequestException
    {
        public OauthUnsupportedGrantTypeException(string message) : base("unsupported_grant_type", message)
        {

        }

    }


}
