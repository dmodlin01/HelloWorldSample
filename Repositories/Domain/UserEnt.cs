using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Domain
{
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
