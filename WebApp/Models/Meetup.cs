using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerDemo.WebApp.Models
{
    public class Meetup
    {
        public Meetup()
        {
            Sessions = new List<MeetupSession>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTimeOffset StartAt { get; set; }

        [Required]
        public DateTimeOffset EndAt { get; set; }

        public ICollection<MeetupSession> Sessions { get; set; }
    }
}
