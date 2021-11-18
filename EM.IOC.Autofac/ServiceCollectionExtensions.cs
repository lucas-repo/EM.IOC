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
        /// 注册指定目录中所有实现了泛型的类型
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="containerBuilder">服务容器</param>
        /// <param name="directory">目录</param>
        /// <param name="searchPattern">搜索模式</param>
        /// <param name="searchOption">搜索选项</param>
        /// <returns>服务容器</returns>
        public static ContainerBuilder Register<T>(this ContainerBuilder containerBuilder, string directory, string searchPattern = "*.dll", SearchOption searchOption = SearchOption.TopDirectoryOnly)
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
            var destType = typeof(T);
            foreach (var file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    if (assembly != null)
                    {
                        containerBuilder.Register(assembly, destType);
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
        /// 注册程序集中所有实现了指定类型的类型
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="containerBuilder">服务容器</param>
        /// <param name="assembly">程序集</param>
        /// <returns>服务容器</returns>
        public static ContainerBuilder Register<T>(this ContainerBuilder containerBuilder, Assembly assembly)
        {
            if (containerBuilder != null)
            {
                Type destType = typeof(T);
                containerBuilder.Register(assembly, destType);
            }
            return containerBuilder;
        }

        /// <summary>
        /// 注册程序集中所有实现了指定类型的类型
        /// </summary>
        /// <param name="containerBuilder">服务容器</param>
        /// <param name="assembly">程序集</param>
        /// <param name="serviceType">服务类型</param>
        /// <returns>服务容器</returns>
        public static ContainerBuilder Register(this ContainerBuilder containerBuilder, Assembly assembly, Type serviceType)
        {
            if (containerBuilder != null && assembly != null && serviceType != null)
            {
                var types = assembly.GetTypes();
                foreach (var implementationType in types)
                {
                    Register(containerBuilder, serviceType, implementationType);
                }
            }
            return containerBuilder;
        }
        /// <summary>
        /// 注册程序集中所有实现了指定类型的类型
        /// </summary>
        /// <param name="containerBuilder">服务容器</param>
        /// <param name="serviceType">服务类型(首选InjectableAttribute.ServiceType，其次为服务类型参数)</param>
        /// <param name="implementationType">实例类型</param>
        /// <returns>服务容器</returns>
        public static ContainerBuilder Register(this ContainerBuilder containerBuilder, Type serviceType, Type implementationType)
        {
            if (containerBuilder != null && serviceType != null && implementationType != null)
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
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"注册{implementationType}失败，{e}");
                }
            }
            return containerBuilder;
        }
      
        ///// <summary>
        ///// 移除容器中指定程序集中指定类型的服务
        ///// </summary>
        ///// <typeparam name="T">泛型</typeparam>
        ///// <param name="containerBuilder">服务容器</param>
        ///// <param name="assembly">程序集</param>
        ///// <param name="isInherited">是否移除继承的类型</param>
        ///// <returns>服务容器</returns>
        //public static ContainerBuilder Remove<T>(this ContainerBuilder containerBuilder, Assembly assembly, bool isInherited = false)
        //{
        //    if (containerBuilder != null && assembly != null)
        //    {
        //        if (containerBuilder.Count > 0)
        //        {
        //            var destType = typeof(T);
        //            if (isInherited)
        //            {
        //                for (int i = containerBuilder.Count - 1; i >= 0; i--)
        //                {
        //                    var type = containerBuilder[i].ServiceType;
        //                    if (type.Assembly == assembly && destType.IsAssignableFrom(type))
        //                    {
        //                        containerBuilder.RemoveAt(i);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                for (int i = containerBuilder.Count - 1; i >= 0; i--)
        //                {
        //                    if (destType == containerBuilder[i].ServiceType)
        //                    {
        //                        containerBuilder.RemoveAt(i);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return containerBuilder;
        //}
        ///// <summary>
        ///// 移除容器中指定程序集中指定类型的服务
        ///// </summary>
        ///// <typeparam name="T">泛型</typeparam>
        ///// <param name="containerBuilder">服务容器</param>
        ///// <param name="isInherited">是否移除继承的类型</param>
        ///// <returns>服务容器</returns>
        //public static ContainerBuilder Remove<T>(this ContainerBuilder containerBuilder, bool isInherited = false)
        //{
        //    if (containerBuilder != null)
        //    {
        //        if (containerBuilder.Count > 0)
        //        {
        //            var destType = typeof(T);
        //            if (isInherited)
        //            {
        //                for (int i = containerBuilder.Count - 1; i >= 0; i--)
        //                {
        //                    if (destType.IsAssignableFrom(containerBuilder[i].ServiceType))
        //                    {
        //                        containerBuilder.RemoveAt(i);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                for (int i = containerBuilder.Count - 1; i >= 0; i--)
        //                {
        //                    if (destType == containerBuilder[i].ServiceType)
        //                    {
        //                        containerBuilder.RemoveAt(i);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return containerBuilder;
        //}
    }
}
