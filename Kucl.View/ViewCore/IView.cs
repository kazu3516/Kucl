using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Kucl.Xml.XmlCfg;

namespace Kucl.View.ViewCore {
    public interface IView:IUseConfig {

        #region �v���p�e�B
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

        #region ���\�b�h
        void RefreshView();
        #endregion

    }
}
