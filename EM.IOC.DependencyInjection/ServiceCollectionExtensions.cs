using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EM.IOC.DependencyInjection
{
    /// <summary>
    /// 服务容器扩展类
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册指定目录中所有实现了泛型的类型
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="services">服务容器</param>
        /// <param name="directory">目录</param>
        /// <param name="searchPattern">搜索模式</param>
        /// <param name="searchOption">搜索选项</param>
        /// <returns>服务容器</returns>
        public static IServiceCollection Register<T>(this IServiceCollection services, string directory, string searchPattern = "*.dll", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (services == null)
            {
                return services;
            }
            if (!Directory.Exists(directory))
            {
                return services;
            }
            var files = Directory.GetFiles(directory, searchPattern, searchOption);
            if (files.Length == 0)
            {
                return services;
            }
            var destType = typeof(T);
            foreach (var file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    if (assembly != null)
                    {
                        services.Register(assembly, destType);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"读取dll“{file}”失败，{e}");
                }
            }
            return services;
        }

        /// <summary>
        /// 注册程序集中所有实现了指定类型的类型
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="services">服务容器</param>
        /// <param name="assembly">程序集</param>
        /// <returns>服务容器</returns>
        public static IServiceCollection Register<T>(this IServiceCollection services, Assembly assembly)
        {
            if (services != null)
            {
                Type destType = typeof(T);
                services.Register(assembly, destType);
            }
            return services;
        }

        /// <summary>
        /// 注册程序集中所有实现了指定类型的类型
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="assembly">程序集</param>
        /// <param name="serviceType">服务类型</param>
        /// <returns>服务容器</returns>
        public static IServiceCollection Register(this IServiceCollection services, Assembly assembly, Type serviceType)
        {
            if (services != null && assembly != null && serviceType != null)
            {
                var types = assembly.GetTypes();
                foreach (var implementationType in types)
                {
                    Register(services, serviceType, implementationType);
                }
            }
            return services;
        }
        /// <summary>
        /// 注册程序集中所有实现了指定类型的类型
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="serviceType">服务类型(首选InjectableAttribute.ServiceType，其次为服务类型参数)</param>
        /// <param name="implementationType">实例类型</param>
        /// <returns>服务容器</returns>
        public static IServiceCollection Register(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            if (services != null && serviceType != null && implementationType != null)
            {
                try
                {
                    if (!implementationType.IsAbstract && serviceType.IsAssignableFrom(implementationType))
                    {
                        Type destServiceType = null;
                        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped;//默认生命周期为作用域
                        var attribute = implementationType.GetCustomAttribute<InjectableAttribute>();
                        if (attribute != null)
                        {
                            destServiceType = attribute.ServiceType;
                            serviceLifetime = attribute.ServiceLifetime;
                        }
                        if (destServiceType==null)
                        {
                            destServiceType=serviceType;
                        }
                        //根据生命周期注册服务
                        switch (serviceLifetime)
                        {
                            case ServiceLifetime.Singleton:
                                services.TryAddEnumerable(ServiceDescriptor.Singleton(destServiceType, implementationType));
                                break;
                            case ServiceLifetime.Scoped:
                                services.TryAddEnumerable(ServiceDescriptor.Scoped(destServiceType, implementationType));
                                break;
                            case ServiceLifetime.Transient:
                                services.TryAddEnumerable(ServiceDescriptor.Transient(destServiceType, implementationType));
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"注册{implementationType}失败，{e}");
                }
            }
            return services;
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
                        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped;//默认生命周期为作用域
                        var attribute = instanceType.GetCustomAttribute<InjectableAttribute>();
                        if (attribute != null)
                        {
                            serviceLifetime = attribute.ServiceLifetime;
                        }
                        //根据生命周期注册服务
                        switch (serviceLifetime)
                        {
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(parentType, implementationFactory);
                                break;
                            case ServiceLifetime.Scoped:
                                services.AddScoped(parentType, implementationFactory);
                                break;
                            case ServiceLifetime.Transient:
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
