using DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HelloWorldWeb.ViewModels
{
    public class MessageVM
    {
        public int MessageId { get; set; }

        public string Message { get; set; }

        public string MessageBody { get; set; }

        public string RecipientId { get; set; }

        public UserDTO Recipient { get; set; }
    }
}
