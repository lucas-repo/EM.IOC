namespace EM.IOC
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin: IInjectable
    {
        /// <summary>
        /// 控制反转管理器
        /// </summary>
        IIocManager IocManager { get; set; }
        /// <summary>
        /// 插件加载优先级(默认9999，优先级最高设为0)
        /// </summary>
        uint Priority { get; set; } 
        /// <summary>
        /// 是否已加载
        /// </summary>
        bool IsLoaded { get; }
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
