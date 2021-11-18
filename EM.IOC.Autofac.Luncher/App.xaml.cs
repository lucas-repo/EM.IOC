using EM.IOC.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace EM.IOC.Autofac.Luncher
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            string[] serviceDirectories = new string[]{ AppDomain.CurrentDomain.BaseDirectory,
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins")
            };//当前目录与插件目录
            IocOptions iocOptions = new IocOptions()
            {
                ServiceDirectories=serviceDirectories
            };
            var assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EM.IOC.Autofac.dll");
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            IIocManager iocManager = BaseIocManager.GetIocManager(assembly, iocOptions);
            IAppManager appManager = iocManager.GetService<IAppManager>();
            //此处可设置优先启动登录窗体
            MainWindow window = new MainWindow(appManager);//在主窗体中加载插件
            if (!(window.ShowDialog() ?? false))
            {
                Shutdown();
                return;
            }
            ShutdownMode = ShutdownMode.OnLastWindowClose;
            base.OnStartup(e);
        }
    }
}
