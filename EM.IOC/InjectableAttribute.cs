using System;

namespace EM.IOC
{
    /// <summary>
    /// 可注入特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class InjectableAttribute : Attribute
    {
        /// <summary>
        /// 生命周期
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; set; }
        /// <summary>
        /// 服务类型
        /// </summary>
        public Type ServiceType { get; set; }
    }
}
