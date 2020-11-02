using AutoMap;
using DTOs;
using HelloWorldWeb.Models;
using HelloWorldWeb.ViewModels;

namespace HelloWorldWeb.AutoMap
{
    public class HelloWorldWebMappingProfile:AutoMappingProfile
    {
        public HelloWorldWebMappingProfile()
        {
            CreateMap<AddressDTO, AddressVM>();
            CreateMap<AddMessageVM, MessageDTO>();

        }

    }
}
