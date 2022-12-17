namespace EM.IOC
{
    /// <summary>
    /// 程序集信息接口
    /// </summary>
    public interface IAssemblyInformation
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 类型限定名称
        /// </summary>
        string AssemblyQualifiedName { get; }
        /// <summary>
        /// 作者
        /// </summary>
        string Author { get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        string BuildDate { get; }
        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 版本
        /// </summary>
        string Version { get; }
    }
}