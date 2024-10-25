using DAM.Domain.Entities;
using MediatR;

namespace DAM.Core.Events.Screens
{
    public class ScreenCreatedEvent : INotification
    {
        public ScreenPost ScreenPost { get; set; }
    }
}
