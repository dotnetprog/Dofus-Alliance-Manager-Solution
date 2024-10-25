using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Core.Abstractions.Services
{
    public interface IBaremeServiceAsync
    {

        Task<Bareme?> GetBareme(Guid AllianceId, Guid BaremeId);
        Task<IReadOnlyCollection<Bareme>> GetBaremes(Guid AllianceId);

        Task<Guid> CreateBareme(Bareme bareme);

        Task UpdateBareme(Bareme bareme);
        Task<Guid> CreateBaremeDetail(BaremeDetail detail);
        Task DeleteBaremeDetail(Guid detail);

        Task DeleteBareme(Guid BaremeId);

    }
}
