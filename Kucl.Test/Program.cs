using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Kucl.Collections;

namespace Kucl.Test {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }
    }





	public class Sample:INotify {
		#region Name
		private string m_Name;
		public event EventHandler NameChanged;
		protected void OnNameChanged(EventArgs e){
			if(this.NameChanged != null){
				this.NameChanged(this,e);
			}
		}
		public string Name{
			get{
				return this.m_Name;
			}
			set{
				if(this.m_Name != value){
					this.m_Name = value;
					this.OnNameChanged(new EventArgs());
					this.OnChanged(new EventArgs());
				}
			}
		}
		#endregion


		#region INotify メンバ


		public event EventHandler Changed;
		protected void OnChanged(EventArgs e) {
			if (this.Changed != null) {
				this.Changed(this,e);
			}
		}
		#endregion
	}
}
