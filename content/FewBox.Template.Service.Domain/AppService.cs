using FewBox.Template.Service.Model.Configs;
using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Services;

namespace FewBox.Template.Service.Domain
{
    public class AppService : IAppService
    {
        private HealthyConfig HealthyConfig { get; set; }
        public AppService(HealthyConfig healthyConfig)
        {
            this.HealthyConfig = healthyConfig;
        }

        public HealthyDto GetHealtyInfo()
        {
            return new HealthyDto{
                Version = this.HealthyConfig.Version
            };
        }
    }
}