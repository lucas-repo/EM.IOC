using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.IOC.Controls
{
    /// <summary>
    /// 插件
    /// </summary>
    public class Plugin : BasePlugin
    {
        protected readonly IAppManager AppManager;
        public Plugin(IAppManager appManager) : base(appManager?.IocManager)
        {
            AppManager=appManager??throw new ArgumentNullException(nameof(appManager));
        }
    }
}
