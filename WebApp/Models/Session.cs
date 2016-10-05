using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerDemo.WebApp.Models
{
    public class Session
    {
        public Session()
        {
            Votes = new List<SessionVote>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public SessionState SessionState { get; set; }

        public DateTimeOffset ProposedAt { get; set; }

        public DateTimeOffset? CancelledAt { get; set; }

        public int? MeetupId { get; set; }

        public Meetup Meetup { get; set; }

        [ForeignKey("Proponent")]
        public string ProponentId { get; set; }

        [Required]
        public ApplicationUser Proponent { get; set; }

        [ForeignKey("Moderator")]
        public string ModeratorId { get; set; }

        [Required]
        public ApplicationUser Moderator { get; set; }

        public ICollection<SessionVote> Votes { get; set; }

        public void CancelSession(DateTimeOffset cancelledAt)
        {
            CancelledAt = cancelledAt;
            SessionState = SessionState.Cancelled;
            Meetup = null;
        }

        public void ScheduleSession(Meetup meetup)
        {
            MeetupId = meetup.Id;
            SessionState = SessionState.Skeduled;
        }

        public void CompleteSession()
        {
            SessionState = SessionState.Done;
        }

    }
}
