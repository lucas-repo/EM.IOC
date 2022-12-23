using EM.Bases;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace EM.IOC
{
    /// <summary>
    /// 插件抽象类
    /// </summary>
    public abstract class Plugin : AssemblyInformation, IPlugin
    {
        private FileVersionInfo _file;
        public virtual uint Priority { get; } = 9999;

        public bool IsLoaded { get; private set; }

        public virtual bool IsUnloadable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        public virtual bool OnLoad() => true;
        /// <inheritdoc/>
        public bool Load()
        {
            bool ret = false;
            if (!IsLoaded)
            {
                IsLoaded = OnLoad();
                if (IsLoaded)
                {
                    ret = true;
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
        /// <inheritdoc/>
        public bool Unload()
        {
            bool ret = false;
            if (IsLoaded)
            {
                if (OnUnload())
                {
                    IsLoaded = false;
                    ret = true;
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
