using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackApp.Models
{
    public class TaskModel
    {
        public Guid Id { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        [Required(ErrorMessage = "TaskSubject is required")]
        public string? TaskSubject { get; set; }
        [Required(ErrorMessage = "TaskDescription is required")]
        public string? TaskDescription { get; set; }
        [Required(ErrorMessage = "TaskStatus is required")]
        public string? TaskStatus { get; set; }
        //[Required(ErrorMessage = "TaskCreateBy is required")]
        public string? TaskCreateBy { get; set; }

        public DateTime TaskCreatedDateAndTime { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "TaskLastModifiedDateAndTime is required")]
        public DateTime TaskLastModifiedDateAndTime { get; set; }
        public Boolean TaskAdminApprove { get; set; } = false;
        [Required(ErrorMessage = "TaskAssignedUserId is required")]
        public int TaskAssignedUserId { get; set; }

        // Calculated property for hours difference
        public double HoursDifference => (TaskLastModifiedDateAndTime - TaskCreatedDateAndTime).TotalHours;

    }
}
