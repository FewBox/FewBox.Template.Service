using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;

namespace FewBox.Template.Service.AutoMapperProfiles
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<App, AppDto>();
            CreateMap<AppPersistantDto, App>();
        }
    }
}