using System;
using System.Collections.Generic;
using System.Text;

namespace EM.IOC
{
    /// <summary>
    /// 服务生命周期
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// 单例
        /// </summary>
        Singleton,
        /// <summary>
        /// 作用域
        /// </summary>
        Scoped,
        /// <summary>
        /// 暂时
        /// </summary>
        Transient
    }
}
