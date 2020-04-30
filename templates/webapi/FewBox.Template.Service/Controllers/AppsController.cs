using AutoMapper;
using FewBox.Template.Service.Model.Entities;
using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace FewBox.Template.Service.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class AppsController : MapperController
    {
        private IAppRepository AppRepository { get; set; }

        public AppsController(IAppRepository appRepository, IMapper mapper) : base(mapper)
        {
            this.AppRepository = appRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<AppDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<AppDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<App>, IEnumerable<AppDto>>(this.AppRepository.FindAll())
            };
        }

        [HttpGet("paging/{pageIndex}/{pageRange}")]
        public PayloadResponseDto<PagingDto<AppDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<AppDto>>
            {
                Payload = new PagingDto<AppDto>
                {
                    Items = this.Mapper.Map<IEnumerable<App>, IEnumerable<AppDto>>(this.AppRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.AppRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<AppDto> Get(Guid id)
        {
            return new PayloadResponseDto<AppDto>
            {
                Payload = this.Mapper.Map<App, AppDto>(this.AppRepository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]AppPersistantDto appDto)
        {
            var app = this.Mapper.Map<AppPersistantDto, App>(appDto);
            Guid appId = this.AppRepository.Save(app);
            return new PayloadResponseDto<Guid> {
                Payload = appId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public PayloadResponseDto<int> Put(Guid id, [FromBody]AppPersistantDto appDto)
        {
            int effect;
            var app = this.Mapper.Map<AppPersistantDto, App>(appDto);
            app.Id = id;
            var updateApp = this.AppRepository.FindOne(id);
            effect = this.AppRepository.Update(app);
            return new PayloadResponseDto<int>{
                Payload = effect
            };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public PayloadResponseDto<int> Delete(Guid id)
        {
            return new PayloadResponseDto<int>{
                Payload = this.AppRepository.Recycle(id)
            };
        }
    }
}
