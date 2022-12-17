namespace EM.IOC
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin:IAssemblyInformation
    {
        /// <summary>
        /// 插件加载优先级(默认9999，优先级最高设为0)
        /// </summary>
        uint Priority { get;  }

        /// <summary>
        /// 是否已加载
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// 是否允许卸载
        /// </summary>
        bool IsUnloadable { get; }

        /// <summary>
        /// 加载插件
        /// </summary>
        /// <returns>成功true反之false</returns>
        bool Load();
        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <returns>成功true反之false</returns>
        bool Unload();


    }
}
