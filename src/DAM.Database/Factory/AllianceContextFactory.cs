using DAM.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAM.Database.Factory
{
    public class AllianceContextFactory : IDesignTimeDbContextFactory<AllianceContext>
    {
        private const string ASPCORE_ENV_NAME = "ASPNETCORE_ENVIRONMENT";
        public AllianceContextFactory()
        {

        }

        private readonly IConfiguration Configuration;
        public AllianceContextFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public AllianceContext CreateDbContext(string[] args)
        {

            Console.WriteLine("Inputs:");
            Console.WriteLine(string.Join("|", args));
            var environment = Environment.GetEnvironmentVariable(ASPCORE_ENV_NAME);
            var indexEnv = Array.IndexOf(args, "--environment");
            if (indexEnv > -1)
            {
                environment = args[indexEnv + 1];
            }


            return Create(
                Directory.GetCurrentDirectory(),
                environment,
                "DefaultConnection");
        }
        private AllianceContext Create(string basePath, string environmentName, string connectionStringName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config.GetConnectionString(connectionStringName);
            Console.WriteLine($"Environment: {environmentName}");

            if (string.IsNullOrWhiteSpace(connstr))
            {
                throw new InvalidOperationException(
                    $"Could not find a connection string named '{connectionStringName}'.");
            }
            else
            {
                var optionsBuilder = new DbContextOptionsBuilder<AllianceContext>();
                optionsBuilder.UseSqlServer(connstr);
                return new AllianceContext(optionsBuilder.Options);
            }
        }
    }

}
