using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Repositories.Domain
{
    /// <summary>
    /// Entity (Domain) class for using with EF (code-first)
    /// </summary>
    [Table("Messages", Schema = "dbo")]
    public class MessageEnt
    {
        [Key]
        public int MessageId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Message { get; set; }
        public string MessageBody { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public UserEnt User { get; set; }
    }
}
