using System;
using System.Net.Http;
using DTOs;
using Xunit;

namespace Repositories.Tests.Message
{
    public class MessageRepositoryTests
    {
        private IMessageRepository _messageRepository;
        /// <summary>
        /// Test MockMessageRepository with no logger
        /// </summary>
        [Fact]
        public void ReturnMessageWithMockMessageRepository()
        {
            //Arrange
            const string expectedResult = "Hello World";
            _messageRepository = new MockMessageRepository();
            //Act
            MessageDTO message = _messageRepository.GetMessage();
            //Assert
            Assert.NotNull(message);
            Assert.Equal(message.Message, expectedResult);
        }
        /// <summary>
        /// Test WebApiMessageRepository with no logger. Expect for the test to fail as the web api is not running
        /// </summary>
        [Fact]
        public void ReturnMessageWithWebApiMessageRepository()
        {
            //Arrange
            const string expectedErrorText = "No connection could be made because the target machine actively refused it.";
            _messageRepository = new WebApiMessageRepository();
            //Act
            
            //Assert
            var exception = Assert.Throws<AggregateException>(() => _messageRepository.GetMessage()); //expect to get Aggregate exception as the host for web API is off
            var innerException = exception.InnerException;
            Assert.NotNull(innerException); //check that inner exception is not null
            Assert.IsType<HttpRequestException>(innerException); //check that inner exception is of type HttpRequestException
            Assert.Equal(innerException.Message, expectedErrorText);
        }
    }
}
