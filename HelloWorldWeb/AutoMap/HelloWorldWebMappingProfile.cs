using AutoMap;
using DTOs;
using HelloWorldWeb.Models;

namespace HelloWorldWeb.AutoMap
{
    public class HelloWorldWebMappingProfile:AutoMappingProfile
    {
        public HelloWorldWebMappingProfile()
        {
            CreateMap<AddressDTO, AddressVM>();
        }
        
    }
}
