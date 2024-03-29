﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EM.IOC
{
    /// <summary>
    /// 控制反转管理器接口
    /// </summary>
    public interface IIocManager 
    {
        /// <summary>
        /// 获取指定类型的服务
        /// </summary>
        /// <typeparam name="T">注册的服务类型</typeparam>
        /// <returns>服务</returns>
        T GetService<T>();
        /// <summary>
        /// 获取指定类型的服务
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplement">实例类型</typeparam>
        /// <returns>服务</returns>
        TImplement GetService<TService, TImplement>() where TImplement:TService;
        /// <summary>
        /// 获取容器中的服务集
        /// </summary>
        /// <typeparam name="T">注册的服务类型</typeparam>
        /// <returns>服务集</returns>
        IEnumerable<T> GetServices<T>();
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <returns>返回成功的扩展</returns>
        List<IPlugin> LoadPlugins();
        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <returns>返回成功的扩展</returns>
        List<IPlugin> UnloadPlugins();
    }
}