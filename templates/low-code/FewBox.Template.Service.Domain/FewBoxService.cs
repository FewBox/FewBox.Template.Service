using FewBox.Template.Service.Model.Configs;
using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Services;

namespace FewBox.Template.Service.Domain
{
    public class FewBoxService : IFewBoxService
    {
        private LowCodeConfig LowCodeConfig { get; set; }
        public FewBoxService(LowCodeConfig lowCodeConfig)
        {
            this.LowCodeConfig = lowCodeConfig;
        }

        public AuthorDto GetAuthor()
        {
            return new AuthorDto{
                Name = this.LowCodeConfig.Author
            };
        }
    }
}