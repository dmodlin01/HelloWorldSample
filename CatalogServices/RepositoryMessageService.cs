using System;
using System.Collections.Generic;
using System.Text;
using DTOs;
using Microsoft.Extensions.Logging;
using Repositories;

namespace CatalogServices
{
    public class RepositoryMessageService : MessageService<RepositoryMessageService>
    {

        public RepositoryMessageService(ILogger<RepositoryMessageService> logger, IMessageRepository messageRepository) : base(
            logger, messageRepository)
        {
            Logger.LogInformation("Instantiating WebApiMessageService.");
        }

        public RepositoryMessageService(ILogger<RepositoryMessageService> logger) : base(logger, new MockMessageRepository()){}
        


    }
}