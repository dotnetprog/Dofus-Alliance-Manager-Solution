using DAM.Domain.Ïdentity;

namespace DAM.Core.Abstractions.Identity
{
    public interface IAppUserIdentityManagerAsync
    {
        Task<AppUser> GetUser(Guid ClientId);
        Task<AppUser> RegisterAsync(string Username, Guid AllianceId);
        Task<AppUser> SignInAsync(Guid ClientId, string ClientSecret);

        Task<IReadOnlyCollection<AppUser>> GetAllusers(Guid AllianceId);

        Task UpdateAsync(AppUser user);
        Task DeleteAsync(Guid ClientId);

        Task UpdateClientSecret(Guid ClientId, string ClientSecret);
    }
}
