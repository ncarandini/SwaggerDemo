using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SwaggerDemo.WebApp.DTOs
{
    public class PostSessionVote
    {

        [Required]
        public int SessionId { get; set; }

        [Required]
        public string UserName { get; set; }

    }
}