using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Core.Abstractions.Services
{
    public interface IMembreService
    {

        Task<Guid> AddPseudo(AnkamaPseudo pseudo);
        Task<IReadOnlyCollection<AnkamaPseudo>> GetPseudos(Guid AllianceMemberId);
        Task<IReadOnlyCollection<AllianceMember>> RechercheParPseudo(Guid AllianceId,string Pseudo);
        Task<IReadOnlyCollection<AllianceMember>> RechercheParPersonnage(Guid AllianceId,string Personnage,string Serveur);

        Task DeletePseudo(Guid MemberId,string ankamapseudo);
    }
}
