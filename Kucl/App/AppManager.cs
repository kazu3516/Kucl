using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
namespace Kucl.App {

    /// <summary>
    /// DLLの処理、アイドルタイム処理を管理するAppManagerクラスです。
    /// </summary>
	public class AppManager {
		private List<MethodInfo> m_LoadInIdles;
		private bool m_FastSetUp;
        private bool m_Initialized;

		#region プロパティ
        /// <summary>
        /// 時間のかかる処理をアイドルタイムに行うかどうかを取得、設定します。
        /// </summary>
        public bool FastSetUp {
            get{
                return this.m_FastSetUp;
            }
            set {
                this.m_FastSetUp = value;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// AppManagerを初期化します。
        /// </summary>
		public AppManager(bool fastsetup) {
            this.m_Initialized = false;
            this.FastSetUp = fastsetup;
			this.m_LoadInIdles = new List<MethodInfo>();
            Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e) {
            this.InitializeInIdle();
        }
        #endregion

		#region LoadAssemblies
		/// <summary>
		/// AppDomain.CurrentDomain.GetAssemblies()を用いて取得される全てのアセンブリを検索し、
        /// ExDLLAttributeAttributeを持つアセンブリからモデルを読み込みます。
		/// 参照設定していても、使用していないアセンブリは読み込まれないため、
		/// LoadAssemblyメソッドを使用して明示的に読み込んでください。
		/// </summary>
		public void LoadAssemblies() {
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly asm in asms) {
				//Console.WriteLine(asm.FullName);
				object[] attrs = asm.GetCustomAttributes(typeof(ExDLLAttribute),false);
				if (attrs.Length != 0) {
					this.LoadAssembly(asm);
				}
			}
		}
		#endregion 

		#region LoadAssembly
        /// <summary>
        /// Assemblyを読み込み、LoadInIdleAttributeが付与されたアイドルタイム初期化を登録します。
        /// </summary>
        /// <param name="asm"></param>
		public void LoadAssembly(Assembly asm) {
			Type attr = typeof(LoadInIdleAttribute);
			if (!this.IsContainAttribute(asm,attr)) {
				return;
			}
			Type[] types = asm.GetTypes();
			foreach (Type t in types) {
				if (!this.IsContainAttribute(t,attr)) {
					continue;
				}
				MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
				foreach (MethodInfo m in methods) {
					if (!this.IsContainAttribute(m,attr)) {
						continue;
					}
					//初期化処理の読み込み
					this.m_LoadInIdles.Add(m);
				}
			}
		}

		private bool IsContainAttribute(ICustomAttributeProvider target,Type attribute) {
			object[] attrs = target.GetCustomAttributes(attribute,false);
			return attrs.Length != 0;
		} 
		#endregion

		#region InitializeInAdle
		/// <summary>
		/// 時間のかかる初期化処理をアプリケーションのアイドルタイムに行います。
        /// Application.Idleイベント内でこのメソッドを呼び出してください。
		/// </summary>
		public void InitializeInIdle() {
            if (this.m_FastSetUp && !this.m_Initialized) {
                foreach (MethodInfo m in this.m_LoadInIdles) {
                    m.Invoke(null, null);
                }
                this.m_Initialized = true;
            }
		}
		#endregion

        private void InitializeStart() {
            this.m_Initialized = false;
        }

	}

	/// <summary>
	/// アイドルタイムを利用して初期化処理を行います。
	/// この属性は、処理を行うメソッド、メソッド定義を含むクラス、アセンブリの3箇所に記述する必要があります。
	/// Methodに付加された場合、ModelManager.InitializeInIdleで処理されるメソッドを表します。
	/// Class,Assemblyに付加された場合、ModelManager.InitializeInIdleで処理されるメソッドを含んでいることを表します。
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
	public sealed class LoadInIdleAttribute : Attribute {
	}

}
