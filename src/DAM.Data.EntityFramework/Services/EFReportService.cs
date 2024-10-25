using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Data.EntityFramework.Services
{
    public class EFReportService : IReportServiceAsync
    {
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;

        private readonly string _commandText = @"EXEC	[dbo].[spSummaryReport]
		@allianceid,
		@baremeAtk,
		@baremeDef,
		@From,
		@To";

        public EFReportService(IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            this._dbContextBuilder = dbContextBuilder;
        }
       
        public async Task<IEnumerable<SummaryReportRow>> GetReportData(Guid allianceid, Guid baremeAtkid, 
            Guid baremeDefid, DateTime From, DateTime to)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync()){
                try
                {
                    var allianceparam = new SqlParameter("@allianceid", allianceid);
                    var baremeAtkParam = new SqlParameter("@baremeAtk", baremeAtkid);
                    var baremeDefparam = new SqlParameter("@baremeDef", baremeDefid);
                    var Fromparam = new SqlParameter("@From", From);
                    var Toparam = new SqlParameter("@To", to);
                    var rows = dbContext.SummaryReportRows.FromSqlRaw(_commandText,
                        allianceparam,
                        baremeAtkParam,
                        baremeDefparam,
                        Fromparam,
                        Toparam).ToArray();
                    return rows;
                }
                catch(Exception ex)
                {
                    throw;
                }
               

            }
        }
    }
}
