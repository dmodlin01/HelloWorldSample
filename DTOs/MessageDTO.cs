using System;
using System.Runtime.Serialization;

namespace DTOs
{
    [DataContract]
    public class MessageDTO
    {
        [DataMember(Name = "Message")]
        public string Message { get; set; }
    }
}
