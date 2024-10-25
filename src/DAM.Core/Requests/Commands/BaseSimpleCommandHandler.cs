using DAM.Core.Abstractions.Requests;
using MediatR;

namespace DAM.Core.Requests.Commands
{
    public abstract class BaseSimpleCommandHandler<TRequest> : ICommandHandler<TRequest, Unit> where TRequest : ICommand<Unit>
    {
        public async Task<Unit> Handle(TRequest request, CancellationToken cancellationToken)
        {
            await Run(request, cancellationToken);

            return Unit.Value;
        }

        public abstract Task Run(TRequest request, CancellationToken cancellationToken);

    }
}
