using EM.IOC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EM.IOC
{
    /// <summary>
    /// 服务容器扩展类
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 如果指定目录的程序集中带有<seealso cref="InjectableAttribute"/>特性的服务类型已注册，但未添加其类型实例，则将其添加至服务集合
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="directory">目录</param>
        /// <param name="searchPattern">搜索模式</param>
        /// <param name="searchOption">搜索选项</param>
        public static void TryAddEnumerable(this IServiceCollection services, string directory, string searchPattern = "*.dll", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (services == null)
            {
                return;
            }
            if (!Directory.Exists(directory))
            {
                return;
            }
            var files = Directory.GetFiles(directory, searchPattern, searchOption);
            if (files.Length == 0)
            {
                return;
            }
            foreach (var file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    if (assembly != null)
                    {
                        services.TryAddEnumerable(assembly);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"读取dll“{file}”失败，{e}");
                }
            }
            return;
        }

        /// <summary>
        /// 如果程序集中带有<seealso cref="InjectableAttribute"/>特性的服务类型已注册，但未添加其类型实例，则将其添加至服务集合
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="assembly">程序集</param>
        public static void TryAddEnumerable(this IServiceCollection services, Assembly assembly)
        {
            if (services == null || assembly == null)
            {
                return;
            }
            else
            {
                var types = assembly.GetTypes();
                services.TryAddEnumerable(types);
            }
            return;
        }
        /// <summary>
        /// 如果带有<seealso cref="InjectableAttribute"/>特性的服务类型已注册，但未添加其类型实例，则将其添加至服务集合
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="types">类型集合</param>
        public static void TryAddEnumerable(this IServiceCollection services, IEnumerable<Type> types)
        {
            if (services != null && types?.Count() > 0)
            {
                foreach (var implementationType in types)
                {
                    services.TryAddEnumerable(implementationType);
                }
            }
        }
        /// <summary>
        /// 如果带有<seealso cref="InjectableAttribute"/>特性的服务类型已注册，但未添加其类型实例，则将其添加至服务集合
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="implementationType">实例类型</param>
        public static void TryAddEnumerable(this IServiceCollection services, Type implementationType)
        {
            if (services == null || implementationType == null || implementationType.IsAbstract)
            {
                return;
            }
            var attribute = implementationType.GetCustomAttribute<InjectableAttribute>();
            if (attribute?.ServiceType == null)
            {
                return;
            }
            try
            {
                //根据生命周期注册服务
                switch (attribute.ServiceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        if (attribute.ServiceType.IsAssignableFrom(implementationType))
                        {
                            services.TryAddEnumerable(ServiceDescriptor.Singleton(attribute.ServiceType, implementationType));
                        }
                        break;
                    case ServiceLifetime.Scoped:
                        if (attribute.ServiceType.IsAssignableFrom(implementationType))
                        {
                            services.TryAddEnumerable(ServiceDescriptor.Scoped(attribute.ServiceType, implementationType));
                        }
                        break;
                    case ServiceLifetime.Transient:
                        if (attribute.ServiceType.IsAssignableFrom(implementationType))
                        {
                            services.TryAddEnumerable(ServiceDescriptor.Transient(attribute.ServiceType, implementationType));
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(Register)}失败_ImplementationType:{implementationType}，{e}");
            }
        }
        /// <summary>
        /// 如果带有<seealso cref="InjectableAttribute"/>特性的服务类型尚未注册，则将其添加至服务集合
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="types">类型集合</param>
        public static void TryAdd(this IServiceCollection services, IEnumerable<Type> types)
        {
            if (services != null && types?.Count() > 0)
            {
                foreach (var implementationType in types)
                {
                    services.TryAdd(implementationType);
                }
            }
        }
        /// <summary>
        /// 如果带有<seealso cref="InjectableAttribute"/>特性的服务类型尚未注册，则将其添加至服务集合
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="implementationType">实例类型</param>
        public static void TryAdd(this IServiceCollection services, Type implementationType)
        {
            if (services == null || implementationType == null || implementationType.IsAbstract)
            {
                return;
            }
            var attribute = implementationType.GetCustomAttribute<InjectableAttribute>();
            if (attribute?.ServiceType == null)
            {
                return;
            }
            try
            {
                //根据生命周期注册服务
                switch (attribute.ServiceLifetime)
                {
                    case EM.IOC.ServiceLifetime.Singleton:
                        if (attribute.ServiceType.IsAssignableFrom(implementationType))
                        {
                            services.TryAdd(ServiceDescriptor.Singleton(attribute.ServiceType, implementationType));
                        }
                        break;
                    case EM.IOC.ServiceLifetime.Scoped:
                        if (attribute.ServiceType.IsAssignableFrom(implementationType))
                        {
                            services.TryAdd(ServiceDescriptor.Scoped(attribute.ServiceType, implementationType));
                        }
                        break;
                    case EM.IOC.ServiceLifetime.Transient:
                        if (attribute.ServiceType.IsAssignableFrom(implementationType))
                        {
                            services.TryAdd(ServiceDescriptor.Transient(attribute.ServiceType, implementationType));
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(Register)}失败_ImplementationType:{implementationType}，{e}");
            }
        }
        /// <summary>
        /// 如果服务类型已注册，但未添加其类型实例，则将其添加至服务集合
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementationType">实例类型</param>
        /// <param name="serviceLifetime">服务生命周期</param>
        /// <returns>若成功，则返回服务类型和实例类型，反之返回null</returns>
        public static void TryAddEnumerable(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
        {
            if (services == null || serviceType == null || implementationType == null)
            {
                return ;
            }

            if (implementationType.IsAbstract || !serviceType.IsAssignableFrom(implementationType))
            {
                return ;
            }
            try
            {
                //根据生命周期注册服务
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.TryAddEnumerable(ServiceDescriptor.Singleton(serviceType, implementationType));
                        break;
                    case ServiceLifetime.Scoped:
                        services.TryAddEnumerable(ServiceDescriptor.Scoped(serviceType, implementationType));
                        break;
                    case ServiceLifetime.Transient:
                        services.TryAddEnumerable(ServiceDescriptor.Transient(serviceType, implementationType));
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(Register)}失败_ServiceType:{serviceType}_ImplementationType:{implementationType}，{e}");
            }
            return ;
        }
        /// <summary>
        /// 如果服务类型尚未注册，则将其添加至服务集合
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementationType">实例类型</param>
        /// <param name="serviceLifetime">服务生命周期</param>
        /// <returns>若成功，则返回服务类型和实例类型，反之返回null</returns>
        public static void TryAdd(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
        {
            if (services == null || serviceType == null || implementationType == null)
            {
                return;
            }

            if (implementationType.IsAbstract || !serviceType.IsAssignableFrom(implementationType))
            {
                return;
            }
            try
            {
                //根据生命周期注册服务
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.TryAdd(ServiceDescriptor.Singleton(serviceType, implementationType));
                        break;
                    case ServiceLifetime.Scoped:
                        services.TryAdd(ServiceDescriptor.Scoped(serviceType, implementationType));
                        break;
                    case ServiceLifetime.Transient:
                        services.TryAdd(ServiceDescriptor.Transient(serviceType, implementationType));
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(Register)}失败_ServiceType:{serviceType}_ImplementationType:{implementationType}，{e}");
            }
            return;
        }
        /// <summary>
        /// 注册指定类型
        /// </summary>
        /// <typeparam name="TParent">需要注册的类型</typeparam>
        /// <param name="services">服务容器</param>
        /// <param name="implementationFactory">获取实例匿名方法</param>
        /// <returns>服务容器</returns>
        public static IServiceCollection Register<TParent>(this IServiceCollection services, Func<IServiceProvider, object> implementationFactory)
        {
            if (services != null && implementationFactory != null)
            {
                var instanceType = implementationFactory.Method.ReturnType;
                var parentType = typeof(TParent);
                if (!instanceType.IsAbstract && parentType.IsAssignableFrom(instanceType))
                {
                    try
                    {
                        var serviceLifetime = EM.IOC.ServiceLifetime.Scoped;//默认生命周期为作用域
                        var attribute = instanceType.GetCustomAttribute<InjectableAttribute>();
                        if (attribute != null)
                        {
                            serviceLifetime = attribute.ServiceLifetime;
                        }
                        //根据生命周期注册服务
                        switch (serviceLifetime)
                        {
                            case EM.IOC.ServiceLifetime.Singleton:
                                services.AddSingleton(parentType, implementationFactory);
                                break;
                            case EM.IOC.ServiceLifetime.Scoped:
                                services.AddScoped(parentType, implementationFactory);
                                break;
                            case EM.IOC.ServiceLifetime.Transient:
                                services.AddTransient(parentType, implementationFactory);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"注册{instanceType}失败，{e}");
                    }
                }
            }
            return services;
        }
        /// <summary>
        /// 移除容器中指定程序集中指定类型的服务
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="services">服务容器</param>
        /// <param name="assembly">程序集</param>
        /// <param name="isInherited">是否移除继承的类型</param>
        /// <returns>服务容器</returns>
        public static IServiceCollection Remove<T>(this IServiceCollection services, Assembly assembly, bool isInherited = false)
        {
            if (services != null && assembly != null)
            {
                if (services.Count > 0)
                {
                    var destType = typeof(T);
                    if (isInherited)
                    {
                        for (int i = services.Count - 1; i >= 0; i--)
                        {
                            var type = services[i].ServiceType;
                            if (type.Assembly == assembly && destType.IsAssignableFrom(type))
                            {
                                services.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = services.Count - 1; i >= 0; i--)
                        {
                            if (destType == services[i].ServiceType)
                            {
                                services.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            return services;
        }
        /// <summary>
        /// 移除容器中指定程序集中指定类型的服务
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="services">服务容器</param>
        /// <param name="isInherited">是否移除继承的类型</param>
        /// <returns>服务容器</returns>
        public static IServiceCollection Remove<T>(this IServiceCollection services, bool isInherited = false)
        {
            if (services != null)
            {
                if (services.Count > 0)
                {
                    var destType = typeof(T);
                    if (isInherited)
                    {
                        for (int i = services.Count - 1; i >= 0; i--)
                        {
                            if (destType.IsAssignableFrom(services[i].ServiceType))
                            {
                                services.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = services.Count - 1; i >= 0; i--)
                        {
                            if (destType == services[i].ServiceType)
                            {
                                services.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            return services;
        }
    }
}
