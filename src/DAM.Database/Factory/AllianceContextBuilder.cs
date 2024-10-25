using DAM.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Database.Factory
{
    public class AllianceContextBuilder : DbContextBuilder<AllianceContext>
    {
        public AllianceContextBuilder(string? connectionString) : base(connectionString)
        {
        }

        public override AllianceContext BuildContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AllianceContext>();
            optionsBuilder.UseSqlServer(this.ConnectionString);

            return new AllianceContext(optionsBuilder.Options);
        }
    }
}
