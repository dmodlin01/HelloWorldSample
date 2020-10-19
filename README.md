# HelloWorldSample
sample .NET Core Console/Web API solution utilizing Factory, Decorator, Repository, Facade, IOC Template and Proxy patterns.

Brief overview:
The example retrieves a "Hello World" message from one of two repositories, Mock and Web API, and writes it to a console. 

Solution consists of 5 projects:
##### HelloWorldConsole - Console application used to write out the message DTO, retrieved from the Services layer.
  - Wired with Dependency Injection, Logging (Serilog) and ability to read configuration from json file.
  - Includes appsettings.json for storing Logging-specific configuration, the service source (to use for data retrieval).
  - Dependency Injection was used to inject the Logger into each layer, the repository type and the service type.
##### CatalogServices - Services layer used as a Facade/Proxy to the Repository
- Consists of an abstact class that templates the behaviour for the WebAPI and Repository service instances. The two provide a facade to the Repositories.
*Note: while the two services could have easily been combined into one, they were left separately with future enhancements in mind (where the behaviour of the two could deviate).*
 ##### Repositories - Repository layer used to retrieve the data (message) from a given Repository
  - Consists of two Repository classes, one to retrieve mocked up data, and onether to retrieve data from web api.
  - Injected with Logger implementation (Serilog)
 ##### HelloWorldWebAPI - Web API used to expose the message
  - Uses dependency injection to inject repository and logging
 ##### DTOs - data transfer object layer used to transport the serializable data across layers (and externally via Web API)
  - Designed as a proxy to the data (with future consideration to other repositories such as EF and output)
  
  ##### Other Notes:
  - Serilog is configured to write to file and console. The log files, HelloWorldConsole.log and HelloWorldWebAPI.log) are written to [Temp]\helloWorld\ folder *(e.g. C:\Users\\[user]\AppData\Local\Temp\helloWorld\HelloWorldConsole.log)*
  
