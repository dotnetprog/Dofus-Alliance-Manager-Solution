using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public enum ScreenValidationStatus
    {
        Pending,
        Processing,
        Completed
    }
    public enum ScreenValidationResultStatus
    {

        Valid,
        NotValid,
        ManualyValid,
        ManualyInvalid
    }
    public class ScreenValidation
    {
        public ScreenValidation()
        {

            this.ProcessingState = ScreenValidationStatus.Pending;
        }

        public Guid Id { get; set; }
        public Guid ScreenPostId { get; set; }
        [ForeignKey("ScreenPostId")]
        public ScreenPost ScreenPost { get; set; }
        public string? OCRScreenText { get; set; }

        public Guid? ClosedByMemberId { get; set; }
        [ForeignKey("ClosedByMemberId")]
        public AllianceMember? ClosedBy { get; set; }
        public DateTime? ClosedOn { get; set; }

        public ScreenValidationStatus ProcessingState { get; set; }
        public ScreenValidationResultStatus? ResultState { get; set; }
    }
}
