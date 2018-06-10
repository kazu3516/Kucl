using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
//using Kucl.Cfg;
using xcfg = Kucl.Xml.XmlCfg;

namespace Kucl.App {

    /// <summary>
    /// アプリケーションのエントリポイントを提供するクラスの基本クラスです。
    /// </summary>
    public class AppMainBase {

        #region 定数
        private const string ErrorLogFileName = "Error.log";
        #endregion

        #region メンバ変数
        private Form m_MainWindow;
        private xcfg.XmlConfigModel m_AppConfigs;
        private xcfg.UseConfigObjectCollection m_UseConfigObjects;

		private AppManager m_AppManager;

        private bool m_UseConfig;
        private string m_ConfigDirectoryName;
        private string m_ConfigDirectory;

        private bool m_UseExDLL;
		private string m_ExDLLDirectoryName;
		private string m_ExDLLDirectory;

        private string m_ErrorLogFileName;

        private bool m_IsConfigSeted;

        private List<string> m_UserPathCollection;

        private bool m_DoWriteLogOnStart;
        #endregion

        #region イベント
        /// <summary>
        /// Configの適用が完了した時に発生するイベントです。
        /// </summary>
        public event EventHandler SetConfigComplete;
        #endregion

        #region プロパティ
        /// <summary>
        /// MainWindowを取得、設定します。
        /// </summary>
        public Form MainWindow {
            get {
                return this.m_MainWindow;
            }
            set {
                this.m_MainWindow = value;
            }
        }
        /// <summary>
        /// AppManagerを取得します。
        /// </summary>
		protected AppManager AppManager {
			get {
				return this.m_AppManager;
			}
		}

        #region Config
        /// <summary>
        /// Configパッケージを取得します。
        /// </summary>
        protected xcfg.XmlConfigModel AppConfigs {
            get {
                return this.m_AppConfigs;
            }
        }
        /// <summary>
        /// Configが設定された後かどうかを取得します。
        /// </summary>
        public bool IsConfigSeted {
            get {
                return this.m_IsConfigSeted;
            }
        }
        /// <summary>
        /// Configを使用するかどうかを取得、設定します。
        /// </summary>
        public bool UseConfig {
            get {
                return this.m_UseConfig;
            }
            set {
                this.m_UseConfig = value;
            }
        }
        /// <summary>
        /// Configが保存されるディレクトリ名を取得、設定します。
        /// </summary>
        public string ConfigDirectoryName {
            get {
                return this.m_ConfigDirectoryName;
            }
            set {
                this.m_ConfigDirectoryName = value;
            }
        }

        /// <summary>
        /// UseConfigObjectsを公開を取得します。
        /// </summary>
        public xcfg.UseConfigObjectCollection UseConfigObjects {
            get {
                return this.m_UseConfigObjects;
            }
        }

        #endregion

        #region ExDLL
        /// <summary>
        /// 追加ライブラリの読み込み機能を使用するかどうかを取得、設定します。
        /// </summary>
        public bool UseExDLL {
            get {
                return this.m_UseExDLL;
            }
            set {
                this.m_UseExDLL = value;
            }
        }
        /// <summary>
        /// 追加ライブラリの保存ディレクトリ名を取得、設定します。
        /// </summary>
        public string ExDLLDirectoryName {
            get {
                return this.m_ExDLLDirectoryName;
            }
            set {
                this.m_ExDLLDirectoryName = value;
            }
        } 
        /// <summary>
        /// 追加ライブラリの保存ディレクトリのフルパスを取得、設定します。
        /// </summary>
		public string ExDLLDirectory {
			get {
				return this.m_ExDLLDirectory;
			}
			set {
				this.m_ExDLLDirectory = value;
			}
		}
        #endregion

        /// <summary>
        /// エラーログファイルのフルパスを取得します。
        /// </summary>
        public string ErrorLogFileFullName {
            get {
                return this.m_ErrorLogFileName;
            }
        }

        /// <summary>
        /// OnStartメソッド内で発生した例外をエラーログファイルに書き込むかどうかを示すBool値を取得または設定します。
        /// </summary>
        public bool DoWriteLogOnStart {
            get {
                return this.m_DoWriteLogOnStart;
            }
            set {
                this.m_DoWriteLogOnStart = value;
            }
        }

        /// <summary>
        /// アプリケーション開始時に作成するディレクトリ名のコレクションです。
        /// </summary>
        protected List<string> UserPathCollection {
            get {
                return this.m_UserPathCollection;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// AppMainBaseを初期化します。
        /// </summary>
        public AppMainBase() {
            this.m_UserPathCollection = new List<string>();
            this.m_ConfigDirectoryName = "config";
			this.m_ExDLLDirectoryName = "dll";
            this.m_UseExDLL = true;
            this.m_UseConfig = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
        #endregion

        #region Startメソッド
        /// <summary>
        /// アプリケーションを開始します。
        /// </summary>
        public void Start() {
            try {
                this.OnStart();
            }
            catch(Exception ex) {
                this.OnErrorAtOnStart(ex);
                return;
            }
            this.LoadConfig();

			this.LoadExDLL();
			Application.Idle += new EventHandler(Application_Idle);
            try {
                this.Run();
            }
            catch(Exception ex) {
                this.OnError(ex);
            }
            finally {
                this.OnEnd();
            }
        }
        #endregion

		#region イベントハンドラ
		private bool m_IsInitializingInAdle;
		void Application_Idle(object sender,EventArgs e) {
			if (!this.m_IsInitializingInAdle) {
				this.m_IsInitializingInAdle = true;
				this.AppManager.InitializeInIdle();
				Application.Idle -= new EventHandler(Application_Idle);
				this.m_IsInitializingInAdle = false;
			}
		} 
		#endregion

        #region RegisterUseConfigObject
        /// <summary>
        /// Configを適用するIUseConfigオブジェクトを登録します。
        /// </summary>
        /// <param name="useConfigObject">登録するIUseConfigオブジェクト</param>
        public void RegisterUseConfigObject(xcfg.IUseConfig useConfigObject) {
            this.m_UseConfigObjects.Add(useConfigObject);
        }
        #endregion

        #region OnStart
        /// <summary>
        /// 初期化処理
        /// Configを読み込むオブジェクトは、
        /// このメソッドが呼び出された瞬間から、このメソッドを抜けるまでにRegisterUseConfigObjectを呼び出す必要があります。
        /// この期間は、このメソッドをオーバーライドした場合、オーバーライドしたメソッドを抜ける前までになります。
        /// </summary>
        protected virtual void OnStart() {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            this.m_IsConfigSeted = false;
            //オブジェクトの構築
            this.m_UseConfigObjects = new xcfg.UseConfigObjectCollection();
            
            //this.m_MainWindow = new MainWindow();
            this.m_AppConfigs = new xcfg.XmlConfigModel();
			this.m_AppManager = new AppManager(false);
            //パスの設定、ディレクトリの確認
            this.InitializePath();
            this.InitializeDirectory();
        }

        /// <summary>
        /// 指定した文字列をUserPathCollectionに追加し、追加された位置を返します。
        /// このメソッドはOnStartメソッド呼び出し前までに呼び出されることを前提としています。
        /// また、OnStartメソッド呼び出し後に、戻り値をindexとしてUserPathCollectionにアクセスすることで、フルパスを取得できます。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected int AddUserPath(string path) {
            this.m_UserPathCollection.Add(path);
            return this.m_UserPathCollection.Count - 1;
        }

        #region パスの設定
        /// <summary>
        /// パスの初期化を行います。このメソッドはOnStartメソッドから呼び出されます。
        /// </summary>
        protected virtual void InitializePath() {
            string path = Application.StartupPath;
            this.m_ErrorLogFileName = Path.Combine(path,AppMainBase.ErrorLogFileName);
            this.m_ConfigDirectory = Path.Combine(path,this.m_ConfigDirectoryName);
			this.m_ExDLLDirectory = Path.Combine(path,this.m_ExDLLDirectoryName);
            //
            for (int i = 0; i < this.m_UserPathCollection.Count; i++) {
                this.m_UserPathCollection[i] = Path.Combine(path, this.m_UserPathCollection[i]);
            }
        }
        #endregion

        #region ディレクトリ構成のチェックと構築
        /// <summary>
        /// 設定されたパスに従い、ディレクトリ構成を初期化します。このメソッドはOnStartメソッドから呼び出されます。
        /// </summary>
        protected virtual void InitializeDirectory() {
            if(this.m_UseConfig) {
                this.CheckAndCreateDirectory(this.m_ConfigDirectory);
            }
            if(this.m_UseExDLL) {
                this.CheckAndCreateDirectory(this.m_ExDLLDirectory);
            }
            //
            for (int i = 0; i < this.m_UserPathCollection.Count; i++) {
                this.CheckAndCreateDirectory(this.m_UserPathCollection[i]);
            }
        }
        /// <summary>
        /// 指定したパスを検査し、存在しないディレクトリであれば、作成します。
        /// </summary>
        /// <param name="path"></param>
		protected void CheckAndCreateDirectory(string path) {
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
		}
        #endregion

        #endregion

		#region LoadConfig
		private void LoadConfig(){
            if(Directory.Exists(this.m_ConfigDirectory)) {
                //設定の読み込み
                this.m_AppConfigs.Load(this.m_ConfigDirectory);
                //Configを適用
                this.ApplyConfig();
                this.m_IsConfigSeted = true;
                //Configは適用した後は使わないのでクリア。
                this.m_AppConfigs.ClearPackage();
            }
		}
		#endregion 

		#region LoadExDLL
		private void LoadExDLL() {
			//追加DLLの読み込み
			//AppDomain内の追加DLL
			this.AppManager.LoadAssemblies();
			//AppDomainに読み込まれていない追加DLL
			if (Directory.Exists(this.m_ExDLLDirectory)) {
				this._LoadDirectory(this.m_ExDLLDirectory);
			}
		}
		private void _LoadDirectory(string directory) {
			string[] subdirectories = Directory.GetDirectories(directory);
			for (int i = 0;i < subdirectories.Length;i++) {
				this._LoadDirectory(subdirectories[i]);
			}
			string[] files = Directory.GetFiles(directory,"*.dll");
			for (int i = 0;i < files.Length;i++) {
				Assembly asm = Assembly.LoadFrom(files[i]);
				this.m_AppManager.LoadAssembly(asm);
			}
		}

		#endregion
        
		#region Run
        /// <summary>
        /// アプリケーションを起動します。
        /// Configに依存する操作は、このメソッドが呼び出された瞬間から有効になります。
        /// このタイミングは、このメソッドをオーバーライドした場合、オーバーライドしたメソッドが呼び出された瞬間になります。
        /// </summary>
        protected virtual void Run() {
            Application.Run(this.m_MainWindow);

        }
        #endregion

        #region Configの適用
        /// <summary>
        /// Configを適用します。
        /// 適用とは保存されたConfigを設定することです。
        /// </summary>
        protected void ApplyConfig() {
            foreach(xcfg.IUseConfig useconfig in this.m_UseConfigObjects) {
                useconfig.ApplyConfig(this.m_AppConfigs);
            }
            if(this.SetConfigComplete != null) {
                this.SetConfigComplete(this,new EventArgs());
            }
        }
        #endregion

        #region Configの更新
        /// <summary>
        /// Configを更新します。
        /// 更新とは変更されたConfigを保存することです。
        /// </summary>
        protected void ReflectConfig() {
            foreach(xcfg.IUseConfig useconfig in this.m_UseConfigObjects) {
                useconfig.ReflectConfig(this.m_AppConfigs);
            }
        }
        #endregion

        #region OnErrorAtOnStart(Exception ex)
        /// <summary>
        /// OnStartメソッドで発生したエラーを処理するメソッドです
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void OnErrorAtOnStart(Exception ex) {
            StringWriter writer = new StringWriter();
            this.Write(ex,writer);
            MessageBox.Show(writer.ToString());
            this.DebugWrite(ex);

            if(this.m_DoWriteLogOnStart) {
                this.WriteErrorLog(ex);
            }
        }
        #endregion

        #region OnError(Exception ex)
        /// <summary>
        /// Startメソッドで発生したエラーを処理するメソッドです
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void OnError(Exception ex) {
            this.WriteErrorLog(ex);
        }
        #endregion

        #region WriteErrorLog
        private void WriteErrorLog(Exception ex) {
            string temp = Path.GetTempFileName();
            using(StreamWriter writer = File.CreateText(temp)) {
                this.Write(ex,writer);
                if(File.Exists(this.m_ErrorLogFileName)) {
                    //以前のファイルが存在する場合、以前のファイルの内容を書き出す
                    using(StreamReader reader = new StreamReader(File.OpenRead(this.m_ErrorLogFileName))) {
                        writer.Write(reader.ReadToEnd());
                    }
                }
            }
            File.Delete(this.m_ErrorLogFileName);
            File.Move(temp,this.m_ErrorLogFileName);
        }
        #endregion

        #region Write(Exception ex,TextWriter writer)
        /// <summary>
        /// エラーログに例外情報を書き出します。
        /// </summary>
        /// <param name="ex">スローされた例外</param>
        /// <param name="writer">エラーログに書き出すTextWriter</param>
        protected void Write(Exception ex,TextWriter writer) {
            writer.WriteLine(DateTime.Now.ToString());
            writer.WriteLine(ex.GetType().FullName);
            writer.WriteLine(ex.Message);
            foreach(DictionaryEntry DE in ex.Data) {
                writer.WriteLine(DE.Key.ToString());
                writer.WriteLine(DE.Value.ToString());
                writer.WriteLine();
            }
            writer.WriteLine(ex.StackTrace);
            writer.WriteLine();
            if(ex.InnerException != null) {
                this.Write(ex.InnerException,writer);
            }
            writer.WriteLine();
            writer.WriteLine();
        }
        #endregion

        #region DebugWrite(Exception ex)
        internal void DebugWrite(Exception ex) {
            StringBuilder bf = new StringBuilder();
            using(StringWriter writer = new StringWriter(bf)) {
                this.Write(ex,writer);
            }
            System.Diagnostics.Debug.Write(bf.ToString());
        }
        #endregion

        #region OnEnd
        /// <summary>
        /// 終了処理
        /// </summary>
        protected virtual void OnEnd() {
            this.ReflectConfig();
            this.m_AppConfigs.Save(this.m_ConfigDirectory);
        }
        #endregion

        #region ThreadExceptionイベントハンドラ
        void Application_ThreadException(object sender,System.Threading.ThreadExceptionEventArgs e) {
            this.OnError(e.Exception);
            this.OnEnd();
            MessageBox.Show("アプリケーションで致命的なエラーが発生したため、終了します。","アプリケーションエラー");
            Application.Exit();
        }
        #endregion

    }

	/// <summary>
	/// この属性が付加されたアセンブリは、AppMainBase.AppManagerによって、追加で読み込まれます。
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class ExDLLAttribute : Attribute {
	}

}