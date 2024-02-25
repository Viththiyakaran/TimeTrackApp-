using System.ComponentModel.DataAnnotations;

namespace TimeTrackApp.Models
{
    public class TaskHistoryModel
    {
        [Key]
        public int HistoryId { get; set; }

        public int TaskId { get; set; }

        public int Id { get; set; }

        public string TaskSubject { get; set; }

        public string TaskDescription { get; set; }

        public string TaskStatus { get; set; }

        public string TaskCreateBy { get; set; }

        public DateTime TaskCreatedDateAndTime { get; set; }

        public DateTime TaskLastModifiedDateAndTime { get; set; }

        public bool TaskAdminApprove { get; set; }

        public int TaskAssignedUserId { get; set; }

        public string ChangeType { get; set; }

        public DateTime ChangeDateTime { get; set; }
    }
}
