using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Practices.Unity;
using Testura.Mutation.Application.Commands.Project.OpenProject;
using Testura.Mutation.Application.Infrastructure;

namespace Testura.Mutation.Application.Extensions
{
    public static class UnityContainerExtensionMethods
    {
        public static IUnityContainer RegisterMediator(this IUnityContainer container, LifetimeManager lifetimeManager)
        {
            container.RegisterType(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            container.RegisterType(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>), "RequestValidationBehavior");
            container.RegisterMediatorValidators(Assembly.GetAssembly(typeof(RequestValidationBehavior<,>)));

            container.RegisterType<IMediator, Mediator>(lifetimeManager)
                .RegisterInstance<ServiceFactory>(type =>
                {
                    var enumerableType = type
                        .GetInterfaces()
                        .Concat(new[] { type })
                        .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                    return enumerableType != null
                        ? container.ResolveAll(enumerableType.GetGenericArguments()[0])
                        : container.IsRegistered(type)
                            ? container.Resolve(type)
                            : null;
                });

            container.RegisterMediatorHandlers(Assembly.GetAssembly(typeof(OpenProjectCommandHandler)));

            return container;
        }

        public static IUnityContainer RegisterMediatorHandlers(this IUnityContainer container, Assembly assembly)
        {
            return container.RegisterTypesImplementingType(assembly, typeof(IRequestHandler<,>))
                            .RegisterNamedTypesImplementingType(assembly, typeof(INotificationHandler<>));
        }

        public static IUnityContainer RegisterMediatorValidators(this IUnityContainer container, Assembly assembly)
        {
            var validators = AssemblyScanner.FindValidatorsInAssembly(assembly);
            validators.ForEach(validator => container.RegisterType(validator.InterfaceType, validator.ValidatorType));
            return container;
        }

        public static IUnityContainer RegisterTypesImplementingType(this IUnityContainer container, Assembly assembly, Type type)
        {
            foreach (var implementation in assembly.GetTypes().Where(t => t.GetInterfaces().Any(implementation => IsSubclassOfRawGeneric(type, implementation))))
            {
                var interfaces = implementation.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    container.RegisterType(@interface, implementation);
                }
            }

            return container;
        }

        public static IUnityContainer RegisterNamedTypesImplementingType(this IUnityContainer container, Assembly assembly, Type type)
        {
            foreach (var implementation in assembly.GetTypes().Where(t => t.GetInterfaces().Any(implementation => IsSubclassOfRawGeneric(type, implementation))))
            {
                var interfaces = implementation.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    container.RegisterType(@interface, implementation, implementation.FullName);
                }
            }

            return container;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var currentType = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == currentType)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }
    }
}
