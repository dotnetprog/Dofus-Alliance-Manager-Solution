using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Core.Abstractions.Services
{
    public interface IAlertServiceAsync
    {
        Task<IReadOnlyCollection<Alert>> GetAlerts(Guid AllianceId);
        Task<Alert?> GetMostRecentAlert(Guid AllianceId);
        Task<Alert?> GetAlert(Guid AlertId);
        Task<Alert> CreateAlert(string message,Guid CreatedBy,Guid AllianceId,int audienceCount);

    }
}
