using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kucl.App;
using Kucl.Xml;
using Kucl.Xml.XmlCfg;

using log4net;

namespace Kucl.Test {

    /// <summary>
    /// アプリケーションのエントリポイントを提供するクラスです。
    /// </summary>
    public class AppMain : AppMainBase {

        #region Main
        /// <summary>
        /// AppMainクラスの既定のインスタンスです。
        /// </summary>
        public static AppMain g_AppMain;
        /// <summary>
        /// アプリケーションで作成するLoggerオブジェクトです。
        /// </summary>
        public static readonly ILog logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppMain.g_AppMain = new AppMain {
                DoWriteLogOnStart = true,
                UseConfig = true,
                UseExDLL = false
            };
            AppMain.g_AppMain.Start();

        }

        #endregion


        #region フィールド(メンバ変数、プロパティ、イベント)

        #region TemporaryFolderPath
        private string m_TemporaryFolderPath;
        /// <summary>
        /// TemporaryFolderPathを取得、設定します。
        /// </summary>
        public string TemporaryFolderPath {
            get {
                return this.m_TemporaryFolderPath;
            }
            set {
                this.m_TemporaryFolderPath = value;
            }
        }
        #endregion

        #region AppInfo
        private AppInfo m_AppInfo;
        /// <summary>
        /// AppInfoを取得します。
        /// </summary>
        public AppInfo AppInfo {
            get {
                return this.m_AppInfo;
            }
        }
        #endregion

        #endregion

        #region OnStart
        /// <summary>
        /// アプリケーションの開始時の動作をオーバーライドします。
        /// </summary>
        protected override void OnStart() {
            AppMain.logger.Info("Kucl.Test:Start");

            //フォルダ構造の初期化
            int logFolderIndex = this.AddUserPath("log");   //フルパス不要だが、念のためindexを保存
            //int tempFolderIndex = this.AddUserPath("temp");

            base.OnStart();

            //初期化したフォルダのフルパスを取得
            //this.m_TemporaryFolderPath = this.UserPathCollection[tempFolderIndex];

            //***************************
            //初期化

            //AppInfo
            this.m_AppInfo = new AppInfo();

            //Window
            XmlConfigTestWindow form = new XmlConfigTestWindow();
            this.MainWindow = form;


            this.RegisterUseConfigObject(this.m_AppInfo);
            
        }
        #endregion

        #region OnRun
        /// <summary>
        /// アプリケーションの実行時の動作をオーバーライドします。
        /// </summary>
        protected override void Run() {
            //Configを使用した動作はここから。

            //
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) {
                AppMain.logger.Info($"Args({i}:{args[i]}");
            }
            //ファイルを開く / 新規ドキュメント
            //MainWindow form = (MainWindow)this.MainWindow;
            //bool openFile = args.Length > 1;
            //openFile = true;
            //if (openFile) {
            //    string filename = args[1];
            //    //string filename = string.Format("{0}\\test1.xml", this.m_DataDirectory);

            //    form.OpenDocument(filename);
            //}
            //else {
            //    //CreateNewDocument内でConfigを使用するため、Runメソッド内で呼び出す。（OnStart内はConfig読み込み前のため不可）
            //    form.CreateNewDocument();
            //}


            base.Run();
        }

        #endregion

        #region OnEnd
        /// <summary>
        /// アプリケーション終了時の動作をオーバーライドします。
        /// </summary>
        protected override void OnEnd() {
            base.OnEnd();

            AppMain.logger.Info("Kucl.Test:End\r\n");
        }
        #endregion

    }
}
