using DAM.Domain.Entities;

namespace DAM.Core.Abstractions.Services
{
    public interface ISaisonServiceAsync
    {
        public Task<Guid> Create(Saison saison);
        public Task Update(Saison saison);
        public Task Delete(Guid SaisonId);
        public Task<Saison> GetById(Guid AllianceId, Guid Id);
        public Task<IReadOnlyCollection<Saison>> GetList(Guid AllianceId);

        public Task<IReadOnlyCollection<SaisonRanking>> GetRankings(Guid AllianceId, Guid SaisonId, bool IncludeMembers = false);

        public Task ClearRankings(Guid SaisonId);
        public Task CreateRankings(IEnumerable<SaisonRanking> rankings);

        public Task<SaisonRanking> GetRanking(Guid AllianceId, Guid SaisonRankingId);
        public Task<SaisonRanking> GetRanking(Guid AllianceId, Guid SaisonId, string PlayerAlias);
        public Task UpdateRanking(SaisonRanking rankings);


    }
}
