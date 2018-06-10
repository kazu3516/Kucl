using System;
using System.Collections.Generic;
using System.Text;

namespace Kucl.View.ViewCore{
    public interface IHost {

        void ShowOptionDialog();
        void ShowViewList();
        void ExitApplication();
        //void EditTaskBook();

        //TaskBook TaskBook {
        //    get;
        //}
        IView CurrentView {
            get;
        }
    }
}
