using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Kucl.View.Win32;

namespace Kucl.View.ViewCore {

    #region MouseMessageSender
    public class MouseMessageSender {

        #region メンバ変数
        private Control m_Host;
        #endregion

        #region プロパティ
        public Control Host {
            get {
                return this.m_Host;
            }
            set {
                this.m_Host = value;
            }
        }
        #endregion

        #region コンストラクタ
        public MouseMessageSender(Control host) {
            this.m_Host = host;
        }
        #endregion

        #region マウスメッセージをMainWindowに送る

        #region SendMouseDownMessageToMainWindow
        public void SendMouseDownMessageToMainWindow(MouseEventArgs e) {
            this.SendMouseDownMessageToMainWindow(e.Button,e.X,e.Y);
        }
        public void SendMouseDownMessageToMainWindow(MouseButtons button,int x,int y) {
            WindowsMessage msg = WindowsMessage.WM_LBUTTONDOWN;
            MouseState wParam = MouseState.MK_LBUTTON;
            if(button == MouseButtons.Left) {
                msg = WindowsMessage.WM_LBUTTONDOWN;
                wParam = MouseState.MK_LBUTTON;
            }
            else if(button == MouseButtons.Right) {
                msg = WindowsMessage.WM_RBUTTONDOWN;
                wParam = MouseState.MK_RBUTTON;
            }
            this.SendMouseMessageToMainWindow(msg,wParam,x,y);
        }
        #endregion

        #region SendMouseMoveMessageToMainWindow
        public void SendMouseMoveMessageToMainWindow(MouseEventArgs e) {
            this.SendMouseMoveMessageToMainWindow(e.Button,e.X,e.Y);
        }
        public void SendMouseMoveMessageToMainWindow(MouseButtons button,int x,int y) {
            WindowsMessage msg = WindowsMessage.WM_MOUSEMOVE;
            MouseState wParam = MouseState.MK_LBUTTON;
            if(button == MouseButtons.Left) {
                wParam = MouseState.MK_LBUTTON;
            }
            else if(button == MouseButtons.Right) {
                wParam = MouseState.MK_RBUTTON;
            }
            else {
                wParam = MouseState.None;
            }
            this.SendMouseMessageToMainWindow(msg,wParam,x,y);
        }
        #endregion

        #region SendMouseUpMessageToMainWindow
        public void SendMouseUpMessageToMainWindow(MouseEventArgs e) {
            this.SendMouseUpMessageToMainWindow(e.Button,e.X,e.Y);
        }
        public void SendMouseUpMessageToMainWindow(MouseButtons button,int x,int y) {
            WindowsMessage msg = WindowsMessage.WM_LBUTTONUP;
            MouseState wParam = MouseState.MK_LBUTTON;
            if(button == MouseButtons.Left) {
                msg = WindowsMessage.WM_LBUTTONUP;
                wParam = MouseState.MK_LBUTTON;
            }
            else if(button == MouseButtons.Right) {
                msg = WindowsMessage.WM_RBUTTONUP;
                wParam = MouseState.MK_RBUTTON;
            }
            this.SendMouseMessageToMainWindow(msg,wParam,x,y);
        }
        #endregion

        private void SendMouseMessageToMainWindow(WindowsMessage msg,MouseState wParam,int x,int y) {
            Message m = new Message();
            m.Msg = (int)msg;
            m.WParam = (IntPtr)wParam;
            m.LParam = (IntPtr)(x | y << 16);
            m.HWnd = this.m_Host.Handle;
            this.SendMouseMessageToMainWindow(m);
        }
        private void SendMouseMessageToMainWindow(Message m) {
            Form form = this.m_Host.FindForm();
            int p = m.LParam.ToInt32();
            Point point = new Point(p);
            //MainWindowのクライアント座標に変換
            point = this.m_Host.PointToScreen(point);
            point = form.PointToClient(point);
            p = point.X | (point.Y << 16);
            Win32API.SendMessage(form.Handle,(WindowsMessage)m.Msg,m.WParam,(IntPtr)p);
        }
        #endregion

    }
    #endregion

}
