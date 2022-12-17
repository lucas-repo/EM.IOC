using Autofac;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EM.IOC.Autofac
{
    /// <summary>
    /// 服务容器扩展类
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册指定目录，所有程序集中带<seealso cref="InjectableAttribute"/>特性的类型
        /// </summary>
        /// <param name="containerBuilder">服务容器</param>
        /// <param name="directory">目录</param>
        /// <param name="searchPattern">搜索模式</param>
        /// <param name="searchOption">搜索选项</param>
        /// <returns>服务容器</returns>
        public static ContainerBuilder RegisterTypes(this ContainerBuilder containerBuilder, string directory, string searchPattern = "*.dll", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (containerBuilder == null)
            {
                return containerBuilder;
            }
            if (!Directory.Exists(directory))
            {
                return containerBuilder;
            }
            var files = Directory.GetFiles(directory, searchPattern, searchOption);
            if (files.Length == 0)
            {
                return containerBuilder;
            }
            foreach (var file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    if (assembly != null)
                    {
                        containerBuilder.RegisterTypes(assembly);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"读取dll“{file}”失败，{e}");
                }
            }
            return containerBuilder;
        }

        /// <summary>
        /// 注册程序集中带<seealso cref="InjectableAttribute"/>特性的类型
        /// </summary>
        /// <param name="containerBuilder">服务容器</param>
        /// <param name="assembly">程序集</param>
        /// <returns>服务容器</returns>
        public static ContainerBuilder RegisterTypes(this ContainerBuilder containerBuilder, Assembly assembly)
        {
            if (containerBuilder != null && assembly != null )
            {
                var types = assembly.GetTypes();
                foreach (var implementationType in types)
                {
                    RegisterTypeByInjectableAttribute(containerBuilder, implementationType);
                }
            }
            return containerBuilder;
        }
        /// <summary>
        /// 注册带<seealso cref="InjectableAttribute"/>特性的类型
        /// </summary>
        /// <param name="containerBuilder">服务容器</param>
        /// <param name="implementationType">实例类型</param>
        /// <returns>服务容器</returns>
        public static ContainerBuilder RegisterTypeByInjectableAttribute(this ContainerBuilder containerBuilder, Type implementationType)
        {
            if (containerBuilder == null || implementationType == null)
            {
                return containerBuilder;
            }
            var attribute = implementationType.GetCustomAttribute<InjectableAttribute>();
            if (attribute == null)
            {
                return containerBuilder;
            }
            Type destServiceType = attribute.ServiceType;
            if (destServiceType == null)
            {
                return containerBuilder;
            }
            if (implementationType.IsAbstract ||!destServiceType.IsAssignableFrom(implementationType))
            {
                return containerBuilder;
            }
                try
            {
                //根据生命周期注册服务
                switch (attribute.ServiceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        containerBuilder.RegisterType(implementationType).AsSelf().As(destServiceType).SingleInstance();
                        break;
                    case ServiceLifetime.Scoped:
                        containerBuilder.RegisterType(implementationType).AsSelf().As(destServiceType).InstancePerLifetimeScope();
                        break;
                    case ServiceLifetime.Transient:
                        containerBuilder.RegisterType(implementationType).AsSelf().As(destServiceType).InstancePerDependency();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"注册{implementationType}失败，{e}");
            }
            return containerBuilder;
        }
        /// <summary>
        /// 注册实例类型为指定的类型
        /// </summary>
        /// <param name="containerBuilder">服务容器</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementationType">实例类型</param>
        /// <param name="serviceLifetime">生命周期</param>
        /// <returns>服务容器</returns>
        public static ContainerBuilder RegisterType(this ContainerBuilder containerBuilder, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
        {
            if (containerBuilder != null && serviceType != null && implementationType != null)
            {
                try
                {
                    if (!implementationType.IsAbstract && serviceType.IsAssignableFrom(implementationType))
                    {
                        //根据生命周期注册服务
                        switch (serviceLifetime)
                        {
                            case ServiceLifetime.Singleton:
                                containerBuilder.RegisterType(implementationType).AsSelf().As(serviceType).SingleInstance();
                                break;
                            case ServiceLifetime.Scoped:
                                containerBuilder.RegisterType(implementationType).AsSelf().As(serviceType).InstancePerLifetimeScope();
                                break;
                            case ServiceLifetime.Transient:
                                containerBuilder.RegisterType(implementationType).AsSelf().As(serviceType).InstancePerDependency();
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"注册{implementationType}失败，{e}");
                }
            }
            return containerBuilder;
        }
    }
}
