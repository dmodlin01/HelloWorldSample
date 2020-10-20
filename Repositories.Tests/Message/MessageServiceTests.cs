using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using CatalogServices;
using DTOs;
using Xunit;

namespace Repositories.Tests.Message
{
    public class MessageServiceTests
    {
        private IMessageRepository _messageRepository;
        
        /// <summary>
        /// Tests RepositoryMessageService with passed in MockRepository
        /// </summary>
        [Fact]
        public void ReturnMessageWithRepositoryMessageService()
        {
            //Arrange
            const string expectedResult = "Hello World";
            _messageRepository = new MockMessageRepository();
            var messageService = new RepositoryMessageService(null, _messageRepository);
            //Act
            MessageDTO message = messageService.GetMessage();
            //Assert
            Assert.NotNull(message);
            Assert.Equal(message.Message, expectedResult);
        }
        /// <summary>
        /// Tests RepositoryMessageService with default repository
        /// </summary>
        [Fact]
        public void ReturnMessageWithRepositoryMessageServiceDefault()
        {
            //Arrange
            const string expectedResult = "Hello World";
           
            var messageService = new RepositoryMessageService(null);
            //Act
            MessageDTO message = messageService.GetMessage();
            //Assert
            Assert.NotNull(message);
            Assert.Equal(message.Message, expectedResult);
        }

        /// <summary>
        /// Test WebApiMessageService with no logger. Expect for the test to fail as the web api is not running
        /// </summary>
        [Fact]
        public void ReturnMessageWithWebApiMessageService()
        {
            //Arrange
            const string expectedErrorText = "No connection could be made because the target machine actively refused it.";
            _messageRepository = new WebApiMessageRepository();
            var messageService = new WebApiMessageService(null,_messageRepository);
            //Act

            //Assert
            var exception = Assert.Throws<AggregateException>(() => messageService.GetMessage()); //expect to get Aggregate exception as the host for web API is off
            var innerException = exception.InnerException;
            Assert.NotNull(innerException); //check that inner exception is not null
            Assert.IsType<HttpRequestException>(innerException); //check that inner exception is of type HttpRequestException
            Assert.Equal(innerException.Message, expectedErrorText);
        }
    }
}
