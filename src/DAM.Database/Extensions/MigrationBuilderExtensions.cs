using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Database.Extensions
{
    public static class MigrationBuilderExtensions
    {
        public static void SqlFromFile(this MigrationBuilder builder, string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Required parameter is missing.", nameof(filename));

            FileInfo assemblyInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string fullPath = $"{assemblyInfo.Directory.FullName}/{filename}";

            if (File.Exists(fullPath) )
            {
                string sql = File.ReadAllText(fullPath);
                string lf = Environment.NewLine; // For compilation on Windows or Linux
                var sqlraw = $"EXEC sp_executesql N'{lf}{sql.Replace("'", "''")}{lf}'";
                builder.Sql(sqlraw);
            }
            else
            {
                throw new FileNotFoundException("File not found.", fullPath);
            }
        }

        public static void CreateProcedure(this MigrationBuilder builder, string name, string schema, string filename, bool dropIfExists = true)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Required parameter is missing.", nameof(name));

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Required parameter is missing.", nameof(filename));

            if (dropIfExists) DropProcedure(builder, name, schema);

            SqlFromFile(builder, filename);
        }

        public static void DropProcedure(this MigrationBuilder builder, string name, string schema = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Required parameter is missing.", nameof(name));

            if (string.IsNullOrEmpty(schema))
                schema = "dbo";

            builder.Sql($"IF OBJECT_ID('[{schema}].[{name}]', 'P') IS NOT NULL DROP PROCEDURE [{schema}].[{name}]");
        }
    }
}
