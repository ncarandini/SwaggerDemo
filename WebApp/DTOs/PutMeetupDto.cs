﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerDemo.WebApp.DTOs
{
    public class PutMeetupDto
    {
        [Required]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? StartAt { get; set; }

        public DateTimeOffset? EndAt { get; set; }
    }
}
