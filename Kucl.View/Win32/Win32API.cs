using System;
using System.Runtime.InteropServices;

namespace Kucl.View.Win32 {
    /// <summary>
    /// Win32API�̃��b�p�[�N���X�ł��B
    /// </summary>
    public static class Win32API{

		#region �萔
		public const int WS_EX_TOOLWINDOW = 0x00000080;
		public const int WS_APPWINDOW = 0x00040000;
		#endregion

		#region API�錾
		[DllImportAttribute("user32.dll",EntryPoint="SetWindowLong")]
		private static extern int SetWindowLong(IntPtr hWnd,short nIndex,int dwNewLong);

		[DllImport("user32.dll",EntryPoint="GetWindowLong")]
		private static extern int GetWindowLong(IntPtr hWnd,short nIndex);

		[DllImport("user32.dll",EntryPoint="SendMessage")]
		private static extern IntPtr SendMessage(IntPtr hWnd,uint Msg,IntPtr wParam,IntPtr lParam);
		#endregion

		#region GetWindowLong
		public static int GetWindowLong(IntPtr hWnd,WindowLongIndex nIndex){
			return Win32API.GetWindowLong(hWnd,(short)nIndex);
		}
		#endregion

		#region SetWindowLong
		public static int SetWindowLong(IntPtr hWnd,WindowLongIndex nIndex,int newValue){
			return Win32API.SetWindowLong(hWnd,(short)nIndex,newValue);
		}
		#endregion

		#region GetWindowLong,SetWindowLong�Ŏg�p����񋓑�
		public enum WindowLongIndex{
			EX_STYLE = -20
		}
		#endregion

		#region SendMessage
		public static IntPtr SendMessage(IntPtr hWnd,WindowsMessage Msg,IntPtr wParam,IntPtr lParam){
			return Win32API.SendMessage(hWnd,(uint)Msg,wParam,lParam);
		}
		#endregion

    }

    #region SystemCommands
    public enum SystemCommands{
		SC_MINIMIZE = 0xF020,
        SC_MAXIMIZE = 0xF030,
    }
    #endregion

    #region HitTestRegion
    public enum HitTestRegion{
		HTCLIENT = 1,
		HTCAPTION = 2,
        //�T�C�Y�ύX
        HTLEFT = 10,
        HTRIGHT = 11,
        HTTOP = 12,
        HTTOPLEFT = 13,
        HTTOPRIGHT = 14,
        HTBOTTOM = 15,
        HTBOTTOMLEFT = 16,
        HTBOTTOMRIGHT = 17,
	}
	#endregion

	#region MouseState
	public enum MouseState{
		None = 0,
		MK_LBUTTON = 0x0001,
		MK_RBUTTON = 0x0002,
		MK_SHIFT = 0x0004,
		MK_CONTROL = 0x0008,
		MK_MBUTTON = 0x0010
	}
	#endregion

	#region WindowsMessage
	public enum WindowsMessage:uint{
		//�q�b�g�e�X�g
		WM_NCHITTEST = 0x0084,

		//�N���C�A���g�G���A

		WM_MOUSEMOVE = 0x0200,
		//���{�^��
		WM_LBUTTONDOWN = 0x0201,
		WM_LBUTTONUP = 0x0202,
		//�E�{�^��
		WM_RBUTTONDOWN = 0x0204,
		WM_RBUTTONUP = 0x0205,

		//��N���C�A���g�G���A

		WM_NCMOUSEMOVE = 0x00A0,
		//�}�E�X���{�^��
		WM_NCLBUTTONDOWN = 0x00A1,
		WM_NCLBUTTONUP = 0x00A2,
		//�}�E�X�E�{�^��
		WM_NCRBUTTONUP = 0x00A5,
		WM_NCRBUTTONDOWN = 0x00A4,

		//�V�X�e���R�}���h
		WM_SYSCOMMAND = 0x0112
	}

	#endregion

}
