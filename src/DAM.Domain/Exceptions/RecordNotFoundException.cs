
namespace DAM.Domain.Exceptions
{
    public class RecordNotFoundException : InvalidEntityOperationException
    {
        public RecordNotFoundException(Guid EntityId, string EntityType) : base(EntityId, EntityType, EntityOperation.Retrieve, "record do not exist.")
        {
        }
    }
}
