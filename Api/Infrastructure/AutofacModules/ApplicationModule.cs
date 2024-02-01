using Api.Application.Events;
using Api.Infrastructure.Services;
using Api.Seedwork.AesEncryption;
using Autofac;
using Autofac.Features.ResolveAnything;
using Domain.AggregatesModel.UserAggregate;
using Infrastructure.Repositories;
using Infrastructure.Seedwork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using Api.Infrastructure.Authorization;
using Api.Infrastructure.Helpers;

namespace Api.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        private readonly ServiceConfiguration _serviceConfiguration;

        public ApplicationModule(ServiceConfiguration serviceConfiguration)
        {
            _serviceConfiguration = serviceConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PortfolioMongoDbContext>().As<IMongoContext>().InstancePerLifetimeScope()
                .WithParameter((pi, ctx) => pi.Name == "mongoDbUrl",
                    (p, c) => c.Resolve<IOptions<PortfolioRepositoryOptions>>().Value.MongoDbUrl)
                .WithParameter((pi, ctx) => pi.Name == "database",
                    (p, c) => c.Resolve<IOptions<PortfolioRepositoryOptions>>().Value.Database);
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            builder.RegisterType<CurrentUserAccessor>().As<ICurrentUserAccessor>();

            Func<ParameterInfo, IComponentContext, bool> parameterSelector = (pi, ctx) => pi.Name == "collectionName";

            builder.RegisterType<MongoDbUserRepository>().As<IUserRepository>().InstancePerLifetimeScope()
                .WithParameter(parameterSelector,
                    (p, c) => c.Resolve<IOptions<PortfolioRepositoryOptions>>().Value.UserCollectionName);

            builder.RegisterType<AesSecurity>().As<IAesSecurity>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ApiGoogleDriveService>().As<GoogleDriveService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ApiFileService>().As<FileService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AuthHelper>().As<IAuthHelper>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ServiceHelper>().As<IServiceHelper>()
                .InstancePerLifetimeScope();

            builder.Register((c, p) =>
            {
                var accessor = c.Resolve<IHttpContextAccessor>();
                var logger = c.Resolve<ILogger<ContainerBuilder>>();

                var headerAuthorization =
                    accessor.HttpContext.Request.Headers["authorization"].ToString() ?? string.Empty;
                var token = "";

                if (!string.IsNullOrEmpty(headerAuthorization))
                {
                    logger.LogInformation("Request authorization header value: {value}", headerAuthorization);
                    token = headerAuthorization.Replace("Bearer", "").Trim();
                }

                return true;
            }).InstancePerDependency();

            builder.RegisterSource(
                new AnyConcreteTypeNotAlreadyRegisteredSource(type => type.Namespace != null && !type.Namespace.Contains("Microsoft"))
                .WithRegistrationsAs(b => b.InstancePerDependency()));
        }
    }
}
