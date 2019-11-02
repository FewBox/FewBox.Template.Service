using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Utility.Net;
using FewBox.Core.Utility.Formatter;
using FewBox.Core.Web.Log;
using FewBox.Core.Web.Notification;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Error;
using FewBox.Core.Web.Filter;
using FewBox.Core.Web.Orm;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Token;
using FewBox.Template.Service.Domain;
using FewBox.Template.Service.Model.Configs;
// using FewBox.Template.Service.Model.Repositories;
using FewBox.Template.Service.Model.Services;
// using FewBox.Template.Service.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NSwag;
using NSwag.SwaggerGeneration.Processors.Security;
using Dapper;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace FewBox.Template.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            this.Configuration = configuration;
            this.HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // SqlMapper.AddTypeHandler(new SQLiteGuidTypeHandler()); // Note: SQLite
            RestfulUtility.IsCertificateNeedValidate = false;
            RestfulUtility.IsLogging = true; // Todo: Need to remove.
            JsonUtility.IsCamelCase = true;
            JsonUtility.IsNullIgnore = true;
            HttpUtility.IsCertificateNeedValidate = false;
            HttpUtility.IsEnsureSuccessStatusCode = false;
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc(options=>{
                if (this.HostingEnvironment.EnvironmentName != "Test")
                {
                    options.Filters.Add<ExceptionAsyncFilter>();
                }
                if (this.HostingEnvironment.EnvironmentName == "Development")
                {
                    options.Filters.Add(new AllowAnonymousFilter());
                }
                // options.Filters.Add<TransactionAsyncFilter>();
                options.Filters.Add<TraceAsyncFilter>();
            })
            .AddJsonOptions(options=>{
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins("https://fewbox.com").AllowAnyMethod().AllowAnyHeader();
                        });
                    options.AddPolicy("all",
                        builder =>
                        {
                            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                        });

                });
            services.AddAutoMapper();
            services.AddMemoryCache();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSingleton<IExceptionProcessorService, ExceptionProcessorService>();
            // Used for Config.
            // Used for [Authorize(Policy="JWTRole_ControllerAction")].
            var jwtConfig = this.Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            services.AddSingleton(jwtConfig);
            var securityConfig = this.Configuration.GetSection("SecurityConfig").Get<SecurityConfig>();
            services.AddSingleton(securityConfig);
            var healthyConfig = this.Configuration.GetSection("HealthyConfig").Get<HealthyConfig>();
            services.AddSingleton(healthyConfig);
            var logConfig = this.Configuration.GetSection("LogConfig").Get<LogConfig>();
            services.AddSingleton(logConfig);
            var notificationConfig = this.Configuration.GetSection("NotificationConfig").Get<NotificationConfig>();
            services.AddSingleton(notificationConfig);
            var fewBoxConfig = this.Configuration.GetSection("FewBoxConfig").Get<FewBoxConfig>();
            services.AddSingleton(fewBoxConfig);
            // Used for RBAC AOP.
            services.AddScoped<IAuthorizationHandler, RoleHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();
            services.AddScoped<IAuthService, RemoteAuthService>();
            // Used for ORM.
            // services.AddScoped<IOrmConfiguration, AppSettingOrmConfiguration>();
            // services.AddScoped<IOrmSession, MySqlSession>(); // Note: MySql
            // services.AddScoped<IOrmSession, SQLiteSession>(); // Note: SQLite
            // services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
            // Used for Application.
            services.AddScoped<IFewBoxService, FewBoxService>();
            // Used for Exception&Log AOP.
            // services.AddScoped<ILogHandler, ConsoleLogHandler>();
            // services.AddScoped<INotificationHandler, ConsoleNotificationHandler>();
            services.AddScoped<ILogHandler, ServiceLogHandler>();
            services.AddScoped<INotificationHandler, ServiceNotificationHandler>();
            // Used for IHttpContextAccessor&IActionContextAccessor context.
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Used for JWT.
            services.AddScoped<ITokenService, JWTToken>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false, // Remove SigningKey validation, change to Auth service.
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });
            // Used for Swagger Open Api Document.
            services.AddOpenApiDocument(config => {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "FewBox FB_SERVICENAME Api";
                    document.Info.Description = "FewBox FB_SERVICENAME, for more information please visit the 'https://fewbox.com'";
                    document.Info.TermsOfService = "https://fewbox.com/terms";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "FewBox",
                        Email = "support@fewbox.com",
                        Url = "https://fewbox.com/support"
                    };
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "Use under license",
                        Url = "https://raw.githubusercontent.com/FewBox/FewBox.Template.Service/master/LICENSE"
                    };
                };
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                config.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT", new List<string>{"API"}, new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Bearer [Token]",
                        In = SwaggerSecurityApiKeyLocation.Header
                    })
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSwagger();
            if (env.IsDevelopment() || env.IsStaging())  
            {
                app.UseCors("all");
                app.UseSwaggerUi3();  
            }
            else
            {
                app.UseCors("all");
                app.UseReDoc();
            }
            app.UseMvc();
        }
    }
}