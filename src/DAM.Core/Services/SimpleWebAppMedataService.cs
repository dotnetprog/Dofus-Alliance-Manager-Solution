using DAM.Core.Abstractions.Services;

namespace DAM.Core.Services
{
    public class SimpleWebAppMedataService : IWebAppMetadataService
    {

        private readonly string _baseUrl;

        public SimpleWebAppMedataService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string GetSaisonPlayerUrl(ulong allianceid, Guid SaisonId, string PlayerName)
        {
            return $"{_baseUrl}/saison/{allianceid}/{SaisonId}/Player/{PlayerName}";
        }

        public string GetEditSaisonUrl(ulong allianceid, Guid SaisonId)
        {
            return $"{_baseUrl}/saison/{allianceid}/Edit/{SaisonId}";
        }

        public string GetUrl()
        {
            return _baseUrl;
        }
    }
}
