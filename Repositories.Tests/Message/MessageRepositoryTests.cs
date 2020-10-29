using System;
using System.IO;
using System.Net.Http;
using AutoMap;
using AutoMapper;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace Repositories.Tests.Message
{
    public class MessageRepositoryTests
    {
        private IMessageRepository _messageRepository;
        private IConfiguration _configuration= GetConfiguration();
        private  const string _expectedResult = "Hello World";
        #region Configuration wiring

        /// <summary>
        /// Wiring for configuration retrieval (ability to retrieve from appsettings.json)
        /// </summary>
        /// <param name="configurationBuilder"></param>
        static void BuildConfig(IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //connection to the appsettings.json
        }
        /// <summary>
        /// Uses configuration builder to build out the configuration
        /// </summary>
        /// <returns>Configuration collection</returns>
        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            return builder.Build();
        }

        #endregion
        /// <summary>
        /// Test MockMessageRepository
        /// </summary>
        [Fact]
        public void ReturnMessageWithMockMessageRepository()
        {
            //Arrange
            
            _messageRepository = new MockMessageRepository();
            //Act
            MessageDTO message = _messageRepository.GetMessage();
            //Assert
            Assert.NotNull(message);
            Assert.Equal(message.Message, _expectedResult);
        }

        /// <summary>
        /// Test EF repository with entity to DTO mapping
        /// </summary>
        [Fact]
        public void ReturnMessageWithEFMessageRepository()
        {
            //Arrange
            //build context options
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_configuration.GetConnectionString("HelloWorldConnection"))
                //.UseInMemoryDatabase(databaseName: "HelloWorld")
                .Options;
            //create AutoMapper instance //Used by EF Repository
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMappingProfile>();
            });

            var mapper = config.CreateMapper();
            
            using (var context = new AppDbContext(options))
            {
                //Act
                 var repository = new EfMessageRepository(context, mapper,null);
                 var message = repository.GetMessage();
                 //Assert
                 Assert.NotNull(message);
                 Assert.Equal(message.Message, _expectedResult);

            }

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
