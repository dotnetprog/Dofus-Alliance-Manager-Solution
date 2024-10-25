using DAM.Domain.Entities;

namespace DAM.WebApp.Services
{
    public interface IAllianceMemberIdentityService
    {
        Task<AllianceMember> GetCurrentMemberAsync();
    }
}
