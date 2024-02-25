using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackApp.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? UserPassword { get; set; }
        [Required]
        public string? UserEmail { get; set; }
        [Required]
        public string? UserLevel { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        [Required]
        public Boolean UserActive { get; set; }

    }
}
