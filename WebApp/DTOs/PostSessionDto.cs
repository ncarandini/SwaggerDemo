using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SwaggerDemo.WebApp.Models;

namespace SwaggerDemo.WebApp.DTOs
{
    public class PostSessionDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ProponentUserName { get; set; }

        [Required]
        public string ModeratorUserName { get; set; }
    }
}