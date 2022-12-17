using Autofac;
using System;
using System.Collections.Generic;
namespace EM.IOC.Autofac
{
    /// <summary>
    /// 控制反转管理器
    /// </summary>
    public class IocManager : IOC.IocManager, IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 容器
        /// </summary>
        public IContainer Container { get; private set; }
        //public ILifetimeScope LifetimeScope { get; private set; }
        /// <summary>
        /// 实例化管理器（自动创建Host主机）
        /// </summary>
        /// <param name="iocOptions">ioc参数</param>
        public IocManager(IocOptions iocOptions) : base(iocOptions)
        {
            var builder = new ContainerBuilder();
            Register(builder, iocOptions);

            Container = builder.Build();
            //LifetimeScope = Container.BeginLifetimeScope();
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
                        builder.Register<IInjectable>(directory);
                    }
                }
            }
            if (iocOptions.ServiceAndImplementations!=null)
            {
                foreach (var item in iocOptions.ServiceAndImplementations)
                {
                    builder.Register(item.Item1, item.Item2);
                }
            }
        }

        public override IEnumerable<T> GetServices<T>()
        {
            //if (LifetimeScope == null)
            //{
            //    return null;
            //}
            //else
            //{
            //    return LifetimeScope.Resolve<IEnumerable<T>>();
            //}
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
            //if (LifetimeScope==null)
            //{
            //    throw new NullReferenceException(nameof(LifetimeScope));
            //}
            //else
            //{
            //    return LifetimeScope.Resolve<T>();
            //}
            if (Container==null)
            {
                throw new NullReferenceException(nameof(Container));
            }
            else
            {
                return Container.Resolve<T>();
            }
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
                        //if (LifetimeScope!=null)
                        //{
                        //    LifetimeScope.Dispose();
                        //    LifetimeScope=null;
                        //}
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
