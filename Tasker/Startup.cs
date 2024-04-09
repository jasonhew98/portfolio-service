using Tasker.Infrastructure;
using Tasker.Infrastructure.AutofacModules;
using Tasker.Infrastructure.Extensions;
using Tasker.Infrastructure.Middlewares;
using Tasker.Infrastructure.Services;
using Tasker.Seedwork.AesEncryption;
using Autofac;
using FluentValidation.AspNetCore;
using Tasker.Infrastructure.Seedwork;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using System;
using Asp.Versioning;
using Tasker.Infrastructure.Extensions;
using Tasker.Features;
using Autofac.Core;
using Quartz.Impl;
using Tasker.Infrastructure.JobFactory;

namespace Tasker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions().Configure<ServiceConfiguration>(Configuration);
            services.AddOptions().Configure<PortfolioRepositoryOptions>(Configuration.GetSection("PortfolioRepository"));
            services.AddOptions().Configure<AesConfigurationOptions>(Configuration.GetSection("AesConfigurations"));
            services.AddOptions().Configure<JwtAuthorizationConfigurationOptions>(Configuration.GetSection("JwtAuthorizationConfigurations"));
            services.AddOptions().Configure<DirectoryPathConfigurationOptions>(Configuration.GetSection("DirectoryPathConfigurations"));
            services.AddOptions().Configure<BaseAddressConfigurationOptions>(Configuration.GetSection("BaseAddressConfigurations"));
            services.AddOptions().Configure<GoogleAuthServiceConfigurationOptions>(Configuration.GetSection("GoogleAuthServiceConfigurations"));
            services.AddOptions().Configure<MicrosoftAuthServiceConfigurationOptions>(Configuration.GetSection("MicrosoftAuthServiceConfigurations"));
            services.AddOptions().Configure<MicrosoftGraphServiceConfigurationOptions>(Configuration.GetSection("MicrosoftGraphServiceConfigurations"));
            services.AddOptions().Configure<CronJobConfigurationOptions>(Configuration.GetSection("CronJobConfigurations"));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddHttpContextAccessor();

            services.AddMvc(opt => opt.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            services.AddFluentValidationAutoValidation();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages().AddNewtonsoftJson();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddJwtBearer();

            services.AddHttpClient<GoogleAuthService, ApiGoogleAuthService>((serviceProvider, client) =>
            {
                var url = serviceProvider.GetService<IOptions<GoogleAuthServiceConfigurationOptions>>().Value.ServiceUrl;
                client.BaseAddress = new Uri(url);
            });

            services.AddHttpClient<MicrosoftAuthService, ApiMicrosoftAuthService>((serviceProvider, client) =>
            {
                var url = serviceProvider.GetService<IOptions<MicrosoftAuthServiceConfigurationOptions>>().Value.ServiceUrl;
                client.BaseAddress = new Uri(url);
            });

            services.AddHttpClient<OneDriveService, ApiOneDriveService>((serviceProvider, client) =>
            {
                var url = serviceProvider.GetService<IOptions<MicrosoftGraphServiceConfigurationOptions>>().Value.ServiceUrl;
                client.BaseAddress = new Uri(url);
            });

            services.AddSingleton<SyncBalanceJob>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var container = builder;

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration.Get<ServiceConfiguration>()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(b =>
                {
                    b.AllowAnyHeader();
                    b.AllowAnyMethod();
                    b.AllowAnyOrigin();
                });
            }

            app.UseCors();
            app.UseMiddleware<JwtAuthorizationMiddleware>();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            MongoDbConfiguration.RegisterDefault();
        }
    }
}
