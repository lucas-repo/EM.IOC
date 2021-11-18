using EM.IOC.Controls;

namespace EM.IOC.Plugins.Headers
{
    public class HeaderPlugin : Plugin
    {
        UserControl1 _userControl;

        public HeaderPlugin(IAppManager appManager) : base(appManager)
        {
        }

        public override bool OnLoad()
        {
            bool ret = false;
            if (_userControl == null)
            {
                _userControl = new UserControl1();
            }
            if (!AppManager.Header.Children.Contains(_userControl))
            {
                AppManager.Header.Children.Add(_userControl);
                ret=true;
            }
            return ret;
        }

        public override bool OnUnload()
        {
            bool ret = false;
            if (_userControl != null && AppManager.Header.Children.Contains(_userControl))
            {
                AppManager.Header.Children.Remove(_userControl);
                ret=true;
            }
            return ret;
        }
    }
}
