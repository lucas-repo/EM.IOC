using EM.IOC.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EM.IOC.Luncher
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
