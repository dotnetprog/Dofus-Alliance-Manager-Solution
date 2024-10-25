namespace DAM.Core.Abstractions.Services
{
    public interface IWebAppMetadataService
    {
        string GetUrl();
        string GetEditSaisonUrl(ulong allianceid, Guid SaisonId);
        string GetSaisonPlayerUrl(ulong allianceid, Guid SaisonId, string PlayerName);

    }
}
