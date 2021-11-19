using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace EM.IOC
{
    /// <summary>
    /// 控制反转管理器
    /// </summary>
    public abstract class BaseIocManager : IIocManager
    {
        /// <summary>
        /// ioc参数
        /// </summary>
        protected IocOptions IocOptions { get; }
        /// <summary>
        /// 实例化管理器（自动创建Host主机）
        /// </summary>
        /// <param name="iocOptions">ioc参数</param>
        public BaseIocManager(IocOptions iocOptions)
        {
            IocOptions=iocOptions;
        }

        public abstract T GetService<T>();
        public abstract IEnumerable<T> GetServices<T>();

        public virtual int LoadPlugins()
        {
            int count = 0;
            foreach (var item in GetServices<IInjectable>().Cast<IPlugin>().OrderBy(x => x.Priority))
            {
                if (item.Load())
                {
                    count++;
                }
            }
            return count;
        }

        public virtual int UnloadPlugins()
        {
            int count = 0;
            foreach (var item in GetServices<IInjectable>().Cast<IPlugin>())
            {
                if (item.Unload())
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 获取程序集中的Ioc管理器集合
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="args">参数</param>
        /// <returns>Ioc管理器集合</returns>
        public static List<IIocManager> GetIocManagers(Assembly assembly, params object[] args)
        {
            List<IIocManager> iocManagers = new List<IIocManager>();
            if (assembly!=null)
            {
                var types = assembly.GetTypes();
                if (types.Length>0)
                {
                    var destType = typeof(IIocManager);
                    foreach (var item in types)
                    {
                        if (!item.IsAbstract && destType.IsAssignableFrom(item))
                        {
                            try
                            {
                                IIocManager iocManager = (IIocManager)Activator.CreateInstance(item, args);
                                iocManagers.Add(iocManager);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine($"创建类型{item}失败：{e}");
                            }
                        }
                    }
                }
            }
            return iocManagers;
        }
        /// <summary>
        /// 获取程序集中的Ioc管理器
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="args">参数</param>
        /// <returns>Ioc管理器</returns>
        public static IIocManager GetIocManager(Assembly assembly, params object[] args)
        {
            IIocManager iocManager = null;
            if (assembly!=null)
            {
                var types = assembly.GetTypes();
                if (types.Length>0)
                {
                    var destType = typeof(IIocManager);
                    foreach (var item in types)
                    {
                        if (!item.IsAbstract && destType.IsAssignableFrom(item))
                        {
                            try
                            {
                                iocManager = (IIocManager)Activator.CreateInstance(item, args);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine($"创建类型{item}失败：{e}");
                            }
                            if (iocManager!=null)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return iocManager;
        }
        /// <summary>
        /// 获取程序集中的Ioc管理器
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="options">Ioc参数</param>
        /// <returns>Ioc管理器</returns>
        public static IIocManager GetIocManagerByOptions(Assembly assembly, IocOptions options)
        {
            IIocManager iocManager = GetIocManager(assembly,options);
            return iocManager;
        }
    }
}
