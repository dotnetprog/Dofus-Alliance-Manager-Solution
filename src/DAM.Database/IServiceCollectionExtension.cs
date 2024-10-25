using Alachisoft.NCache.Client;
using Alachisoft.NCache.EntityFrameworkCore;
using DAM.Database.Contexts;
using DAM.Database.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Database
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection ConfigureDAMDatabase(this IServiceCollection service, IConfiguration Configuration)
        {


            service.AddDbContextFactory<AllianceContext>(
        options =>
        {
        string cacheId = "myClusteredCache";
        NCacheConfiguration.Configure(cacheId, DependencyType.SqlServer);
        NCacheConfiguration.ConfigureLogger();
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), (so) => so.EnableRetryOnFailure(3,TimeSpan.FromSeconds(2),null));
          
        });

           

           
            return service;
        }
    }
}
