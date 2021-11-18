using System;
using System.Collections.Generic;
using System.Text;

namespace EM.IOC
{
    /// <summary>
    /// Ioc参数
    /// </summary>
    public struct IocOptions
    {
        /// <summary>
        /// 服务目录集合
        /// </summary>
        public string[] ServiceDirectories { get; set; }
        /// <summary>
        /// 待添加的服务集合
        /// </summary>
        public List<(Type, Type)> ServiceAndImplementations { get; set; }
    }
}
