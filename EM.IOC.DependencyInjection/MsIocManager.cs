using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace EM.IOC.DependencyInjection
{
    /// <summary>
    /// 控制反转管理器
    /// </summary>
    public class MsIocManager :IocManager, IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 主机
        /// </summary>
        public IHost Host { get; private set; }
        /// <summary>
        /// 实例化管理器（自动创建Host主机）
        /// </summary>
        /// <param name="iocOptions">ioc参数</param>
        public MsIocManager(IocOptions iocOptions) : base(iocOptions)
        {
            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
            hostBuilder.ConfigureServices((services) => AddServices(services, iocOptions));//注册继承了IInjectable的类型
            Host = hostBuilder.Build(); 
            Host.RunAsync();
        }
        /// <summary>
        /// 实例化管理器（自动创建Host主机）
        /// </summary>
        /// <param name="hostBuilder">宿主构造器</param>
        /// <param name="iocOptions">容器初始化参数</param>
        public MsIocManager(IHostBuilder hostBuilder, IocOptions iocOptions) : base(iocOptions)
        {
            if (hostBuilder == null)
            {
                throw new ArgumentNullException(nameof(hostBuilder));
            }
            hostBuilder.ConfigureServices((services) => AddServices(services, iocOptions));//注册继承了IInjectable的类型
            Host = hostBuilder.Build();
            Host.RunAsync();
        }
        private void AddServices(IServiceCollection services, IocOptions iocOptions)
        {
            RegisterIocManager(services);//默认注册一个管理器
            if (iocOptions.ServiceDirectories!=null)
            {
                foreach (var directory in iocOptions.ServiceDirectories)
                {
                    if (directory!=null)
                    {
                        services.TryAddEnumerable(directory);
                    }
                }
            }
            foreach (var item in iocOptions.SingleServices)
            {
                services.TryAdd(item.ServiceType, item.ImplementationType, item.ServiceLifetime);
            }
            foreach (var item in iocOptions.IenumerableServices)
            {
                services.TryAddEnumerable(item.ServiceType, item.ImplementationType, item.ServiceLifetime);
            }
        }

        /// <summary>
        /// 添加管理器
        /// </summary>
        protected virtual IServiceCollection RegisterIocManager(IServiceCollection services)
        {
            services.AddSingleton<IIocManager>(this);//默认注册一个管理器为单例服务
            return services;
        }

        public override IEnumerable<T> GetServices<T>()
        {
            if (Host==null)
            {
                return null;
            }
            else
            {
                return Host.Services.GetServices<T>();
            }
        }

        public override T GetService<T>()
        {
            if (Host==null)
            {
                throw new NullReferenceException(nameof(Host));
            }
            else
            {
                return Host.Services.GetService<T>();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    if (Host!=null)
                    {
                        Host.Dispose();
                        Host=null;
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue=true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~IocManager()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
