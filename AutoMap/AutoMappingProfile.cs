using System;
using AutoMapper;
using DTOs;
using Repositories.Domain;

namespace AutoMap
{
    public class AutoMappingProfile:Profile 
    {
        public AutoMappingProfile()
        {
            CreateMap<MessageDTO, MessageEnt>();
            CreateMap<MessageEnt, MessageDTO>();
        }
    }
}
