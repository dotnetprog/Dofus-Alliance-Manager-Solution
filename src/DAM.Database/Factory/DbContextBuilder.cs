using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Database.Factory
{
    public abstract class DbContextBuilder<T> where T : DbContext
    {

        protected readonly string? ConnectionString;
        public DbContextBuilder(string? connectionString)
        {
            ConnectionString = connectionString;
        }

        public abstract T BuildContext();



    }
}
