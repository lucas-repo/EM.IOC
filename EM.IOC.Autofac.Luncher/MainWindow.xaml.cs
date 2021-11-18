using EM.IOC.Controls;
using System;
using System.Windows;

namespace EM.IOC.Autofac.Luncher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAppManager AppManager;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(IAppManager appManager) : this()
        {
            AppManager = appManager ?? throw new NullReferenceException(nameof(appManager));
            AppManager.Header = header;
            AppManager.Content = content;
            AppManager.Bottom = bottom;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppManager?.IocManager.LoadPlugins();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AppManager?.IocManager.UnloadPlugins();
        }
    }
}
