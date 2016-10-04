using System.ComponentModel.DataAnnotations;

namespace SwaggerDemo.WebApp.Models
{
    public class SessionVote
    {
        public int Id { get; set; }

        public int Vote { get; set; }

        public int SessionId { get; set; }

        [Required]
        public Session Session { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}