using DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HelloWorldWeb.ViewModels
{
    public class AddMessageVM
    {
        public int MessageId { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string MessageBody { get; set; }
        [Required]
        public string RecipientId { get; set; }

        public List<SelectListItem> Users { get; set; }
    }
}
