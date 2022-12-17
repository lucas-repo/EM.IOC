using System;
using System.Collections.Generic;
using System.Text;

namespace EM.IOC
{
    /// <summary>
    /// 容器参数
    /// </summary>
    public class IocOptions
    {
        /// <summary>
        /// 要注册的服务所在目录集合
        /// </summary>
        public List<string> ServiceDirectories { get; } = new List<string>();
        /// <summary>
        /// 待添加的服务集合
        /// </summary>
        public List<(Type ServiceType, Type ImplementationType, ServiceLifetime ServiceLifetime)> NewServices { get; } = new List<(Type ServiceType, Type ImplementationType, ServiceLifetime ServiceLifetime)>();
    }
}
