namespace DAM.Domain.Exceptions
{
    public enum EntityOperation
    {
        Create,
        Update,
        Delete,
        Retrieve
    }
    public class InvalidEntityOperationException : Exception
    {
        public InvalidEntityOperationException(Guid EntityId,
                                               string EntityType,
                                               EntityOperation operation,
                                               string Reason
            ) : base($"Cannot {operation.ToString()} {EntityType}(Id = {EntityId}) because {Reason}")
        {
            this.EntityId = EntityId.ToString();
            this.EntityType = EntityType;
            this.Operation = operation;
            this.Reason = Reason;
        }
        public InvalidEntityOperationException(string EntityId,
                                              string EntityType,
                                              EntityOperation operation,
                                              string Reason
           ) : base($"Cannot {operation.ToString()} {EntityType}(Id = {EntityId}) because {Reason}")
        {
            this.EntityId = EntityId;
            this.EntityType = EntityType;
            this.Operation = operation;
            this.Reason = Reason;
        }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public EntityOperation Operation { get; set; }

        public string Reason { get; set; }
    }
}
