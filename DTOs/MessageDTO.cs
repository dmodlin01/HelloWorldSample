using System;
using System.Runtime.Serialization;

namespace DTOs
{
    [DataContract]
    public class MessageDTO
    {
        [DataMember(Name = "MessageId")]
        public int MessageId { get; set; }

        [DataMember(Name = "Message")]
        public string Message { get; set; }
        [DataMember(Name = "MessageBody")]
        public string MessageBody { get; set; }
        [DataMember(Name = "RecipientId")]
        public int? RecipientId { get; set; }
    }

}
