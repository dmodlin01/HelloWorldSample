using System;
using System.Collections.Generic;
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
    [Table("Users", Schema = "dbo")]
    public class UserEnt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        [ForeignKey("UserId")]
        public List<MessageEnt> Messages;

    }
}
