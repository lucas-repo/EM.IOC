using System;
using System.Diagnostics;

namespace EM.IOC
{
    /// <summary>
    /// 插件抽象类
    /// </summary>
    public abstract class BasePlugin : BaseInjectable, IPlugin
    {
        public IIocManager IocManager { get; set; }
        public uint Priority { get; set; } = 9999;

        public bool IsLoaded { get; private set; }

        public BasePlugin(IIocManager iocManager)
        {
            IocManager = iocManager??throw new ArgumentNullException(nameof(iocManager));
        }
        /// <summary>
        /// 加载
        /// </summary>
        public virtual bool OnLoad() => true;
        public bool Load()
        {
            bool ret = false;
            if (!IsLoaded)
            {
                IsLoaded= OnLoad();
                if (IsLoaded)
                {
                    ret=true;
                    Debug.WriteLine($"加载{GetType()}成功！");
                }
                else
                {
                    Debug.WriteLine($"加载{GetType()}失败！");
                }
            }
            return ret;
        }
        /// <summary>
        /// 卸载
        /// </summary>
        public virtual bool OnUnload() => true;
        public bool Unload()
        {
            bool ret = false;
            if (IsLoaded)
            {
                if (OnUnload())
                {
                    IsLoaded=false;
                    ret=true;
                    Debug.WriteLine($"卸载{GetType()}成功！");
                }
                else
                {
                    Debug.WriteLine($"卸载{GetType()}失败！");
                }
            }
            return ret;
        }
    }
}
