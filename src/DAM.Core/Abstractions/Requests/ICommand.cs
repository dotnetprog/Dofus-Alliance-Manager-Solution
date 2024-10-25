using MediatR;

namespace DAM.Core.Abstractions.Requests
{
    public interface ICommand<T> : IRequest<T>
    {
    }
}
