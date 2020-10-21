﻿using System;
using System.Runtime.Serialization;

namespace DTOs
{
    [DataContract]
    public class MessageDTO
    {
        [DataMember(Name = "MessageId")] 
        public int MessageId;
        [DataMember(Name = "Message")]
        public string Message { get; set; }
    }
}
