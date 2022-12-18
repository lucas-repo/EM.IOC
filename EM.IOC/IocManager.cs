using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EM.IOC
{
    /// <summary>
    /// 控制反转管理器
    /// </summary>
    public abstract class IocManager : IIocManager
    {
        /// <summary>
        /// ioc参数
        /// </summary>
        protected IocOptions IocOptions { get; }
        /// <summary>
        /// 实例化管理器（自动创建Host主机）
        /// </summary>
        /// <param name="iocOptions">ioc参数</param>
        public IocManager(IocOptions iocOptions)
        {
            IocOptions = iocOptions;
        }

        /// <inheritdoc/>
        public abstract T GetService<T>();
        /// <inheritdoc/>
        public abstract IEnumerable<T> GetServices<T>();

        /// <inheritdoc/>
        public virtual async Task<List<IPlugin>> LoadPlugins()
        {
            var ret = new List<IPlugin>();
            await Task.Factory.StartNew(() =>
            {
                var plugins = GetServices<IPlugin>();
                var plugins1 = plugins.Where(x => !x.IsUnloadable).OrderBy(x => x.Priority);//不允许卸载的扩展
                var unloadablePlugins = plugins.Where(x => x.IsUnloadable).OrderBy(x => x.Priority);//允许卸载的扩展

                foreach (var item in plugins1.Union(unloadablePlugins))
                {
                    if (Load(item))
                    {
                        ret.Add(item);
                    }
                }
            });
            return ret;
        }
        private static bool Load(IPlugin plugin)
        {
            bool ret = false;
            if (plugin == null)
            {
                return ret;
            }
            if (!plugin.IsLoaded)
            {
                try
                {
                    Debug.WriteLine($"加载 {plugin.AssemblyQualifiedName} 中");
                    plugin.Load();
                    ret = true;
                    Debug.WriteLine($"加载 {plugin.AssemblyQualifiedName} 完成");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"加载 {plugin.AssemblyQualifiedName} 失败，{e}");
                }
            }
            return ret;
        }

        /// <inheritdoc/>
        public virtual List<IPlugin> UnloadPlugins()
        {
            var ret = new List<IPlugin>();
            foreach (var item in GetServices<IPlugin>())
            {
                if (item?.Unload() == true)
                {
                    ret.Add(item);
                }
            }
            return ret;
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
            if (assembly != null)
            {
                var types = assembly.GetTypes();
                if (types.Length > 0)
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
            if (assembly != null)
            {
                var types = assembly.GetTypes();
                if (types.Length > 0)
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
                            if (iocManager != null)
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
            IIocManager iocManager = GetIocManager(assembly, options);
            return iocManager;
        }

        public TImplement GetService<TService, TImplement>() where TImplement : TService
        {
            TImplement ret = default;
            var services = GetServices<TService>();
            foreach (var item in services)
            {
                if (item is TImplement implement)
                {
                    ret = implement;
                    break;
                }
            }
            return ret;
        }
    }
}
