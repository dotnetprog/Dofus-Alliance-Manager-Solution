using DAM.Core.Abstractions.Identity;
using DAM.Database.Contexts;
using DAM.Domain.Ïdentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAM.Data.EntityFramework.Identity
{
    public class EFAppUsersManagerAsync : IAppUserIdentityManagerAsync
    {
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        public EFAppUsersManagerAsync(IDbContextFactory<AllianceContext> dbContextBuilder, IPasswordHasher<AppUser> passwordHasher)
        {
            _dbContextBuilder = dbContextBuilder;
            _passwordHasher = passwordHasher;
        }

        public async Task DeleteAsync(Guid ClientId)
        {
            using var context = await this._dbContextBuilder.CreateDbContextAsync();

            var founduser = await context.AppUsers.FindAsync(ClientId);
            if (founduser == null)
            {
                return;
            }

            context.AppUsers.Remove(founduser);
            await context.SaveChangesAsync();


        }

        public async Task<IReadOnlyCollection<AppUser>> GetAllusers(Guid AllianceId)
        {
            using var context = await this._dbContextBuilder.CreateDbContextAsync();


            return await context.AppUsers.Where(u => u.AllianceId == AllianceId).ToArrayAsync();


        }

        public async Task<AppUser> GetUser(Guid ClientId)
        {
            using var context = await this._dbContextBuilder.CreateDbContextAsync();
            var user = await context.AppUsers.FindAsync(ClientId);
            return user;
        }

        public async Task<AppUser> RegisterAsync(string Username, Guid AllianceId)
        {

            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new ArgumentException("Cannot be empty nor null", nameof(Username));
            }
            if (AllianceId == Guid.Empty)
            {
                throw new ArgumentException("Cannot be empty", nameof(AllianceId));
            }

            using var context = await this._dbContextBuilder.CreateDbContextAsync();

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                Username = Username,
                AllianceId = AllianceId
            };
            await context.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<AppUser> SignInAsync(Guid ClientId, string ClientSecret)
        {
            using var context = await this._dbContextBuilder.CreateDbContextAsync();

            // encrypt clientsecret

            var founduser = await context.AppUsers.Include(a => a.Alliance).FirstOrDefaultAsync(a => a.Id == ClientId);

            if (founduser == null)
                return null;


            var result = this._passwordHasher.VerifyHashedPassword(null, founduser.HashedClientSecret, ClientSecret);

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return founduser;
        }

        public async Task UpdateAsync(AppUser user)
        {
            using var context = await this._dbContextBuilder.CreateDbContextAsync();
            var founduser = await context.AppUsers.FindAsync(user.Id);

            if (founduser.Username != user.Username)
            {
                founduser.Username = user.Username;
            }
            if (context.Entry<AppUser>(founduser).State == EntityState.Unchanged)
            {
                return;
            }
            await context.SaveChangesAsync();


        }

        public async Task UpdateClientSecret(Guid ClientId, string ClientSecret)
        {
            using var context = await this._dbContextBuilder.CreateDbContextAsync();
            var founduser = await context.AppUsers.FindAsync(ClientId);


            var hashedpassword = this._passwordHasher.HashPassword(null, ClientSecret);
            founduser.HashedClientSecret = hashedpassword;
            await context.SaveChangesAsync();

        }
    }
}
