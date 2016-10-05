using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SwaggerDemo.WebApp.Models;

namespace SwaggerDemo.WebApp.DTOs
{
    public class GetSessionDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public SessionState SessionState { get; set; }

        public DateTimeOffset ProposedAt { get; set; }

        public DateTimeOffset? CancelledAt { get; set; }

        public int? MeetupId { get; set; }

        public string ProponentFullName { get; set; }

        public string ModeratorFullName { get; set; }

        public int NumOfVotes { get; set; }

    }
}