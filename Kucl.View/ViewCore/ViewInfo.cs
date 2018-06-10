using System;
using System.Collections.Generic;
using System.Text;

namespace Kucl.View.ViewCore {

    #region ViewInfo
    public class ViewInfo {

        #region �����o�ϐ�
        private IView m_View;
        private string m_FileName;

        #endregion

        #region �v���p�e�B
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

        #region �R���X�g���N�^
        public ViewInfo(IView view,string filename) {
            this.m_FileName = filename;
            this.m_View = view;
        }
        #endregion
    }
    #endregion

}
