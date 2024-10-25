using DAM.Core.Abstractions.Services;
using DAM.Core.Abstractions.Validator;
using DAM.Core.Requests.Commands;
using DAM.Domain.Entities;
using FluentValidation.Results;

namespace DAM.Core.Validators
{
    public class AppSaisonValidator : ISaisonValidator
    {
        private readonly ISaisonServiceAsync _saisonServiceAsync;
        public AppSaisonValidator(ISaisonServiceAsync saisonServiceAsync)
        {
            _saisonServiceAsync = saisonServiceAsync;
        }


        private IEnumerable<ValidationFailure> ValidateDates(DateTime StartDate, DateTime EndDate, bool AllowOverlap, IReadOnlyCollection<Saison> existingSeasons)
        {
            var faillures = new List<ValidationFailure>();



            if (StartDate >= EndDate)
            {
                faillures.Add(new ValidationFailure(nameof(EndDate), "La date de fin doit être plus grande que la date de début."));
            }
            if (AllowOverlap)
            {
                return faillures;
            }
            if (existingSeasons.Any(es => es.StartDate <= StartDate && StartDate <= es.EndDate))
            {
                faillures.Add(new ValidationFailure(nameof(StartDate), "La date de début est déjà prise en compte dans une autre saison. Veuillez la changer."));
            }
            if (existingSeasons.Any(es => es.StartDate <= EndDate && EndDate <= es.EndDate))
            {
                faillures.Add(new ValidationFailure(nameof(EndDate), "La date de fin est déjà prise en compte dans une autre saison. Veuillez la changer."));
            }
            return faillures;
        }

        public async Task<IEnumerable<ValidationFailure>> ValidateCommand(CreateSaisonCommand command)
        {
            var existingSeasons = await this._saisonServiceAsync.GetList(command.AllianceId);
            var faillures = ValidateDates(command.StartDate, command.EndDate, command.AllowOverlap, existingSeasons);
            return faillures;
        }

        public async Task<IEnumerable<ValidationFailure>> ValidateCommand(UpdateSaisonCommand command)
        {
            var existingSeasons = await this._saisonServiceAsync.GetList(command.AllianceId);
            var faillures = ValidateDates(command.StartDate, command.EndDate, command.AllowOverlap, existingSeasons.Where(e => e.Id != command.Id).ToArray());
            return faillures;
        }
    }
}
