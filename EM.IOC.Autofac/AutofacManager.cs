using Autofac;
using System;
using System.Collections.Generic;
namespace EM.IOC.Autofac
{
    /// <summary>
    /// 控制反转管理器
    /// </summary>
    public class AutofacManager : IocManager, IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 容器
        /// </summary>
        public IContainer Container { get; private set; }
        /// <summary>
        /// 实例化管理器
        /// </summary>
        /// <param name="iocOptions">ioc参数</param>
        public AutofacManager(IocOptions iocOptions) : base(iocOptions)
        {
            var builder = new ContainerBuilder();
            Register(builder, iocOptions);
            Container = builder.Build(); 
        }

        private void Register(ContainerBuilder builder, IocOptions iocOptions)
        {
            builder.RegisterInstance(this).As<IIocManager>().AsSelf().SingleInstance();//默认注册一个管理器
            if (iocOptions.ServiceDirectories!=null)
            {
                foreach (var directory in iocOptions.ServiceDirectories)
                {
                    if (directory!=null)
                    {
                        builder.RegisterTypes(directory);
                    }
                }
            }
            if (iocOptions.NewServices != null)
            {
                foreach (var item in iocOptions.NewServices)
                {
                    builder.RegisterType(item.ServiceType, item.ImplementationType, item.ServiceLifetime);
                }
            }
        }

        public override IEnumerable<T> GetServices<T>()
        {
            if (Container==null)
            {
                return null;
            }
            else
            {
                return Container.Resolve<IEnumerable<T>>();
            }
        }

        public override T GetService<T>()
        {
            T ret = default;
            if (Container==null)
            {
                throw new NullReferenceException(nameof(Container));
            }
            else
            {
                if (Container.IsRegistered<T>())
                {
                    ret= Container.Resolve<T>();
                }
            }
            return ret;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    if (Container!=null)
                    {
                        Container.Dispose();
                        Container=null;
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
