using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EM.IOC.Controls
{
    /// <summary>
    /// app管理器
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IAppManager))]
    public class AppManager :  IAppManager
    {
        /// <summary>
        /// 顶部控件
        /// </summary>
        public Panel Header { get; set; }
        /// <summary>
        /// 内容控件
        /// </summary>
        public Panel Content { get; set; }
        /// <summary>
        /// 底部控件
        /// </summary>
        public Panel Bottom { get; set; }

        public IIocManager IocManager { get; }
        public AppManager(IIocManager iocManager)
        {
            IocManager=iocManager??throw new ArgumentNullException(nameof(iocManager));
        }
    }
}
