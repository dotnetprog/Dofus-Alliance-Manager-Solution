using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Core.Abstractions.Services
{
    public interface IReportServiceAsync
    {
        Task<IEnumerable<SummaryReportRow>> GetReportData(Guid allianceid, Guid baremeAtkid,Guid baremeDefif,DateTime From,DateTime to);
    }
}
