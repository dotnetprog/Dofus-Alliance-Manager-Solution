using MediatR;

namespace DAM.Core.Abstractions.Requests
{
    public interface ICommandHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
    }
}
