using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SwaggerDemo.WebApp.Models;

namespace SwaggerDemo.WebApp.DTOs
{
    public class PutSessionDto
    {
        [Required]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public SessionState? SessionState { get; set; }

        public string ModeratorUserName { get; set; }
    }
}