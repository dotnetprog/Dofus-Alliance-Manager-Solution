using DAM.Core.Requests.Commands;
using FluentValidation.Results;

namespace DAM.Core.Abstractions.Validator
{
    public interface ISaisonValidator
    {
        Task<IEnumerable<ValidationFailure>> ValidateCommand(CreateSaisonCommand command);
        Task<IEnumerable<ValidationFailure>> ValidateCommand(UpdateSaisonCommand command);
    }
}
