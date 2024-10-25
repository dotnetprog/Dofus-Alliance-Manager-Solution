using DAM.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkamaWebClient.Abstractions
{
    public interface IAnkamaService
    {
        Task<IReadOnlyCollection<AnkamaPseudoData>> SearchAnkadex(string pseudo);

        Task<bool> ValidatePseudo(string pseudo);

        Task<string> ParsePseudo(string pseudo);

    }
}
