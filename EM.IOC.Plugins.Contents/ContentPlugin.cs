using EM.IOC.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.IOC.Plugins.Contents
{
    public class ContentPlugin : Controls.Plugin
    {
        UserControl1 _userControl;

        public ContentPlugin(IAppManager appManager) : base(appManager)
        {
        }
         
        public override bool OnLoad()
        {
            bool ret = false;
            if (_userControl == null)
            {
                _userControl = new UserControl1();
            }
            if (!AppManager.Content.Children.Contains(_userControl))
            {
                AppManager.Content.Children.Add(_userControl);
                ret=true;
            }
            return ret;
        }

        public override bool OnUnload()
        {
            bool ret = false;
            if (_userControl != null && AppManager.Content.Children.Contains(_userControl))
            {
                AppManager.Content.Children.Remove(_userControl);
                ret=true;
            }
            return ret;
        }
    }
}
