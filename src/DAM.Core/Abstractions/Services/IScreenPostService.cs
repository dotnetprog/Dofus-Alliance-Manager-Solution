using DAM.Domain.Entities;

namespace DAM.Core.Abstractions.Services
{
    public interface IScreenPostServiceAsync
    {
        Task<Guid> CreatePost(ScreenPost post, params AllianceMember[] relatedmembers);


        Task UpdatePost(ScreenPost post, params AllianceMember[] relatedmembers);

        Task<IReadOnlyCollection<ScreenPost>> GetPostsByFileSize(int filesize);
        Task<ScreenPost?> GetPost(ulong MessageId, bool WIthImage);
        Task DeletePost(Guid postId);

        Task<ScreenPost?> GetPost(Guid postId, bool WithImage);

        Task<IReadOnlyCollection<ScreenPost>> GetPostsByMember(Guid MemberId, bool includeImage);

        Task SetScreenValidationResult(Guid PostId, ScreenValidationResultStatus finalState, Guid ClosedByMemberId);

        Task<IReadOnlyCollection<ScreenPost>> GetScreenPostsByPlayerBetweenTwoDates(Guid AllianceId, Guid MemberId, DateTime Start, DateTime End);


    }
}
