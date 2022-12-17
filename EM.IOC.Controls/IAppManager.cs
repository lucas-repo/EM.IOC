using System.Windows.Controls;

namespace EM.IOC.Controls
{
    /// <summary>
    /// app管理器接口
    /// </summary>
    public interface IAppManager 
    {
        /// <summary>
        /// Ioc管理器
        /// </summary>
        IIocManager IocManager { get; }
        /// <summary>
        /// 顶部控件
        /// </summary>
        Panel Header { get; set; }
        /// <summary>
        /// 内容控件
        /// </summary>
        Panel Content { get; set; }
        /// <summary>
        /// 底部控件
        /// </summary>
        Panel Bottom { get; set; }
    }
}