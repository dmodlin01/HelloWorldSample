using Repositories;
using Microsoft.Extensions.Logging;

namespace CatalogServices
{
    /// <summary>
    /// While the Abstract/base class it providing the base implementation, the intention of this class is to extend (Decorate) it
    /// </summary>
    public class WebApiMessageService: MessageService<WebApiMessageService>
    {
        /// <summary>
        /// Extending the base constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="messageRepository"></param>
        public WebApiMessageService(ILogger<WebApiMessageService> logger, IMessageRepository messageRepository) : base(
            logger, messageRepository)
        {
            Logger.LogInformation("Instantiating WebApiMessageService.");
        }
        /// <summary>
        /// Sample method to extend the base/abstract class
        /// </summary>
        /// <returns></returns>
        public string GetMessageText()
        {
            return GetMessage()?.Message;
        }
    }
}
