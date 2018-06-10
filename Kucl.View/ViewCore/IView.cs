using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Kucl.Xml.XmlCfg;

namespace Kucl.View.ViewCore {
    public interface IView:IUseConfig {

        #region プロパティ
        IHost Host {
            get;
            set;
        }
        Form MainWindow {
            get;
        }
        string Name {
            get;
        }
        #endregion

        #region メソッド
        void RefreshView();
        #endregion

    }
}
