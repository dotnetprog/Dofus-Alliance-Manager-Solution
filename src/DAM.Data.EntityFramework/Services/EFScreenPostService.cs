using Alachisoft.NCache.EntityFrameworkCore;
using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DAM.Data.EntityFramework.Services
{
    public class EFScreenPostService : IScreenPostServiceAsync
    {
        private readonly CachingOptions cachingOptions = new CachingOptions
        {
            StoreAs = StoreAs.SeparateEntities
        };
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;
        public EFScreenPostService(IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _dbContextBuilder = dbContextBuilder;
        }
        public async Task<Guid> CreatePost(ScreenPost post, params AllianceMember[] relatedmembers)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                post.Id = Guid.NewGuid();
                if (post.CreatedOn == default(DateTime))
                {
                    post.CreatedOn = DateTime.UtcNow;
                }


                await dbContext.ScreenPosts.AddAsync(post);

                var validation = new ScreenValidation()
                {
                    Id = Guid.NewGuid(),
                    ScreenPostId = post.Id
                };
                await dbContext.ScreenValidations.AddAsync(validation);
                await dbContext.SaveChangesAsync();

                var relations = relatedmembers.GroupBy(x => x.Id).Select(g => new AllianceMember_ScreenPost()
                {
                    AllianceMemberId = g.Key,
                    ScreenPostId = post.Id,
                    CharacterCount = g.Count()
                }).ToList();
                await dbContext.Member_ScreenPosts.AddRangeAsync(relations);
                await dbContext.SaveChangesAsync();

                return post.Id;
            }
        }

        public async Task DeletePost(Guid postId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var post = await dbContext.ScreenPosts.FindAsync(postId);
                if (post == null)
                {
                    return;
                }

                var validations = await dbContext.ScreenValidations.Where(sv => sv.ScreenPostId == postId).ToListAsync();

                if (validations.Any())
                {
                    dbContext.ScreenValidations.RemoveRange(validations);
                }


                dbContext.ScreenPosts.Remove(post);

                await dbContext.SaveChangesAsync();
                dbContext.GetCache().Remove(post);
            }
        }

        public async Task<ScreenPost?> GetPost(Guid postId, bool WithImage)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var query = await dbContext.ScreenPosts
                    .Where(c => c.Id == postId).FromCacheAsync(cachingOptions);

                if (WithImage)
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    var result = query.Select(sp => new ScreenPost
                    {
                        Id = sp.Id,
                        CreatedByMemberId = sp.CreatedByMemberId,
                        CreatedOn = sp.CreatedOn,
                        Description = sp.Description,
                        Type = sp.Type,
                        EnemyCount = sp.EnemyCount,
                        HasOtherWithSameSize = sp.HasOtherWithSameSize,
                        Target = sp.Target,
                        DiscordMessageId = sp.DiscordMessageId,
                        DiscordChannelId = sp.DiscordChannelId
                    });
                    return result.FirstOrDefault();
                }


            }
        }

        public async Task<ScreenPost?> GetPost(ulong MessageId, bool WithImage)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var query = await dbContext.ScreenPosts
                    .Where(c => c.DiscordMessageId == MessageId).FromCacheAsync(cachingOptions);

                if (WithImage)
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    var result = query.Select(sp => new ScreenPost
                    {
                        Id = sp.Id,
                        CreatedByMemberId = sp.CreatedByMemberId,
                        CreatedOn = sp.CreatedOn,
                        Description = sp.Description,
                        Type = sp.Type,
                        EnemyCount = sp.EnemyCount,
                        HasOtherWithSameSize = sp.HasOtherWithSameSize,
                        Target = sp.Target,
                        DiscordMessageId = sp.DiscordMessageId,
                        DiscordChannelId = sp.DiscordChannelId
                    });
                    return result.FirstOrDefault();
                }


            }
        }

        public async Task<IReadOnlyCollection<ScreenPost>> GetPostsByFileSize(int filesize)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var results = await dbContext.ScreenPosts
                    .Where(sp => sp.Filesize == filesize)
                    .Select(sp => new ScreenPost
                    {
                        Id = sp.Id,
                        Filesize = sp.Filesize
                    }).ToListAsync();
                return results;
            }
        }

        public async Task<IReadOnlyCollection<ScreenPost>> GetPostsByMember(Guid MemberId, bool includeImage)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {

                var query = dbContext.ScreenPosts.Where(sp => sp.Members.Any(m => m.AllianceMemberId == MemberId));
                if (includeImage)
                {
                    return await query.ToArrayAsync();
                }
                else
                {
                    return await query.Select(sp => new ScreenPost
                    {
                        Id = sp.Id,
                        CreatedByMemberId = sp.CreatedByMemberId,
                        CreatedOn = sp.CreatedOn,
                        Description = sp.Description,
                        Type = sp.Type
                    }).ToArrayAsync();
                }

            }
        }

        public async Task<IReadOnlyCollection<ScreenPost>> GetScreenPostsByPlayerBetweenTwoDates(Guid AllianceId, Guid MemberId, DateTime Start, DateTime End)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var screens = await (from sp in dbContext.ScreenPosts
                                     where sp.Members.Any(m => m.AllianceMemberId == MemberId) &&
                                           (sp.CreatedOn >= Start && sp.CreatedOn < End)
                                     select sp).Include(sp => sp.ScreenValidations)
                                     .Include(sp => sp.AllianceEnemy)
                                     .Include(sp => sp.Members)
                                     .ThenInclude(m => m.AllianceMember)
                                     .ToArrayAsync();
                return screens;
            }
        }

        public async Task SetScreenValidationResult(Guid PostId, ScreenValidationResultStatus finalState, Guid ClosedByMemberId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var validations = await dbContext.ScreenValidations.Where(w => w.ScreenPostId == PostId).ToListAsync();
                foreach (var v in validations)
                {
                    v.ResultState = finalState;
                    v.ProcessingState = ScreenValidationStatus.Completed;
                    v.ClosedOn = DateTime.UtcNow;
                    v.ClosedByMemberId = ClosedByMemberId;
                }
                dbContext.ScreenValidations.UpdateRange(validations);
                await dbContext.SaveChangesAsync();

            }

        }



        public async Task UpdatePost(ScreenPost post, params AllianceMember[] relatedmembers)
        {
            var existingPost = post.Id == Guid.Empty ?
                await this.GetPost(post.DiscordMessageId.Value, false) :
                await this.GetPost(post.Id, false);
            post.Id = existingPost.Id;
            if (existingPost == null)
            {
                throw new InvalidEntityOperationException(post.Id, nameof(ScreenPost), EntityOperation.Update, $"record does not exist (messageid = {post.DiscordMessageId?.ToString()})");
            }
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                dbContext.Attach(existingPost);
                var entityEntry = dbContext.ScreenPosts.Entry(existingPost);
                entityEntry.CurrentValues.SetValues(post);
                if (!post.AllianceEnemyId.HasValue)
                    entityEntry.Property(navProp => navProp.AllianceEnemyId).IsModified = false;
                if (string.IsNullOrWhiteSpace(post.ImageUrl))
                    entityEntry.Property(navProp => navProp.ImageUrl).IsModified = false;
                if (string.IsNullOrWhiteSpace(post.ImagePrepUrl))
                    entityEntry.Property(navProp => navProp.ImagePrepUrl).IsModified = false;
                if (string.IsNullOrWhiteSpace(post.Description))
                    entityEntry.Property(navProp => navProp.Description).IsModified = false;
                if (!post.DiscordChannelId.HasValue)
                {
                    entityEntry.Property(navProp => navProp.DiscordChannelId).IsModified = false;
                }
                if (!post.DiscordMessageId.HasValue)
                {
                    entityEntry.Property(navProp => navProp.DiscordMessageId).IsModified = false;
                }


                //will never change
                entityEntry.Property(navProp => navProp.CreatedOn).IsModified = false;
                entityEntry.Property(navProp => navProp.CreatedByMemberId).IsModified = false;
                entityEntry.Property(navProp => navProp.Type).IsModified = false;
                entityEntry.Property(navProp => navProp.Target).IsModified = false;
                entityEntry.Property(navProp => navProp.EnemyCount).IsModified = false;


                await dbContext.SaveChangesAsync();
                dbContext.GetCache().Remove(existingPost);

                if (relatedmembers == null || !relatedmembers.Any())
                {
                    return;
                }

                var existingMembers = await dbContext.Member_ScreenPosts.Where(msp => msp.ScreenPostId == existingPost.Id).ExecuteDeleteAsync();

                var relations = relatedmembers.GroupBy(x => x.Id).Select(g => new AllianceMember_ScreenPost()
                {
                    AllianceMemberId = g.Key,
                    ScreenPostId = post.Id,
                    CharacterCount = g.Count()
                }).ToList();
                await dbContext.Member_ScreenPosts.AddRangeAsync(relations);
                await dbContext.SaveChangesAsync();


            }

        }

    }
}
