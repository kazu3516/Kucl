using System;
using System.Collections.Generic;
using System.Text;

namespace Kucl.View.ViewCore {

    #region ViewInfo
    public class ViewInfo {

        #region メンバ変数
        private IView m_View;
        private string m_FileName;

        #endregion

        #region プロパティ
        public IView View {
            get {
                return this.m_View;
            }
        }
        public string FileName {
            get {
                return this.m_FileName;
            }
        }
        #endregion

        #region コンストラクタ
        public ViewInfo(IView view,string filename) {
            this.m_FileName = filename;
            this.m_View = view;
        }
        #endregion
    }
    #endregion

}
