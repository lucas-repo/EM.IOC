using System.Collections.Generic;

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
        /// <typeparam name="T">泛型</typeparam>
        /// <returns>服务</returns>
        T GetService<T>();
        /// <summary>
        /// 获取容器中的服务集
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <returns>服务集</returns>
        IEnumerable<T> GetServices<T>();
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <returns>返回成功个数</returns>
        int LoadPlugins();
        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <returns>返回成功个数</returns>
        int UnloadPlugins();
    }
}