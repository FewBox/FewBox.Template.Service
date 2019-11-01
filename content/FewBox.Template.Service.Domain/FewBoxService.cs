using FewBox.Template.Service.Model.Configs;
using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Services;

namespace FewBox.Template.Service.Domain
{
    public class FewBoxService : IFewBoxService
    {
        private FewBoxConfig FewBoxConfig { get; set; }
        public FewBoxService(FewBoxConfig fewBoxConfig)
        {
            this.FewBoxConfig = fewBoxConfig;
        }

        public AuthorDto GetAuthor()
        {
            return new AuthorDto{
                Name = this.FewBoxConfig.Author
            };
        }
    }
}