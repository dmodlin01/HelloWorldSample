using System.Runtime.Serialization;

namespace DTOs
{
    [DataContract]
    public class UserDTO
    {
        [DataMember(Name = "UserId")]
        public int UserId { get; set; }
        [DataMember(Name = "UserName")]
        public string UserName { get; set; }
        [DataMember(Name = "FullName")]
        public string FullName { get; set; }
    }
}
