using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Repositories.Domain
{
    /// <summary>
    /// Entity (Domain) class for using with EF (code-first)
    /// </summary>
    public class MessageEnt
    {
        [Key]
        public int MessageId { get; set; }
        public string Message { get; set; }
    }
}
