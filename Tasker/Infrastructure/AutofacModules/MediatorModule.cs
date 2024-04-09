using Autofac;
using FluentValidation;
using MediatR;
using System.Linq;
using System.Reflection;

namespace Tasker.Infrastructure.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            var apiAssembly = Assembly.Load("Tasker");
            builder.RegisterAssemblyTypes(apiAssembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
            builder.RegisterAssemblyTypes(apiAssembly).AsClosedTypesOf(typeof(INotificationHandler<>));
            builder.RegisterAssemblyTypes(apiAssembly).Where(t => t.IsClosedTypeOf(typeof(IValidator<>))).AsImplementedInterfaces();
        }
    }
}
