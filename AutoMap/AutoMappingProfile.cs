using System;
using AutoMapper;
using DTOs;
using Repositories.Domain;

namespace AutoMap
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<MessageDTO, MessageEnt>()
                .ForMember(m => m.UserId, opt => opt.MapFrom(s => s.RecipientId));
            //.ForMember(x => x.RecipientId, opt => opt.Ignore());

            CreateMap<MessageEnt, MessageDTO>()
                .ForMember(m => m.RecipientId, opt => opt.MapFrom(s => s.UserId));
            //.ForSourceMember(x=>x.RecipientId,opt=>opt.DoNotValidate())

        }
    }
}
