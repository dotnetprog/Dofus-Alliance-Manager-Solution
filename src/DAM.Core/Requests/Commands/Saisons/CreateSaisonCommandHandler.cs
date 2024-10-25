using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Requests;
using DAM.Core.Abstractions.Services;
using DAM.Core.Abstractions.Validator;
using DAM.Domain.Entities;
using FluentValidation;

namespace DAM.Core.Requests.Commands
{
    public class CreateSaisonCommandHandler : ICommandHandler<CreateSaisonCommand, Guid>
    {
        private readonly IDAMMapper _mapper;
        private readonly ISaisonServiceAsync _saisonService;
        private readonly ISaisonValidator _saisonValidator;
        public CreateSaisonCommandHandler(IDAMMapper mapper, ISaisonServiceAsync saisonService, ISaisonValidator saisonValidator)
        {
            _mapper = mapper;
            _saisonService = saisonService;
            _saisonValidator = saisonValidator;
        }
        public async Task<Guid> Handle(CreateSaisonCommand request, CancellationToken cancellationToken)
        {

            var faillures = await _saisonValidator.ValidateCommand(request);
            if (faillures.Any())
            {
                throw new ValidationException(faillures);
            }

            var saison = _mapper.Map<Saison, CreateSaisonCommand>(request);
            saison.Id = await _saisonService.Create(saison);
            return saison.Id;

        }

    }
}
