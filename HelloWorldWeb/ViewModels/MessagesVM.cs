using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;

namespace HelloWorldWeb.ViewModels
{
    public class MessagesVM
    {
        public MessageDTO LatestMessage;
        public List<MessageDTO> RemainingMessages;
        public AddMessageVM addMessageVM;
    }
}
