using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using API.Entities;
using API.DTOs;
using API.Extensions;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser,MemberDto>().ForMember(x=>x.PhotoUrl,y=>y.MapFrom(z=>z.Photos.FirstOrDefault(xx=>xx.IsMain).Url))
                                          .ForMember(dest=>dest.Age,options=>options.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoDto>();

            CreateMap<MemberEditDto,AppUser>();
        }
    }
}