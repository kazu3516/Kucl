using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Kucl;
using Kucl.App;
using Kucl.Cfg;
namespace Kucl.App {


    #region ContextAppMainBase
    /// <summary>
    /// ApplicationContextを使用したAppMianBaseクラスの派生クラスです。
    /// </summary>
    public class ContextAppMainBase : AppMainBase {

        #region メンバ変数
        private ApplicationContext m_Context;

        #endregion

        #region プロパティ
        /// <summary>
        /// 対象とするApplicationContextを取得します。
        /// </summary>
        public ApplicationContext Context {
            get {
                return this.m_Context;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// ContextAppMainBaseクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="context"></param>
        public ContextAppMainBase(ApplicationContext context)
            : base() {
            this.m_Context = context;
        }
        #endregion

        #region Run
        /// <summary>
        /// Runメソッドをオーバーライドします。
        /// このインスタンスが保持するApplicationContextを使用して、メッセージループを開始します。
        /// </summary>
        protected override void Run() {
            Application.Run(this.m_Context);
        }
        #endregion

        #region Exit
        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        public void Exit() {
            this.m_Context.ExitThread();
        }
        #endregion

    }
    #endregion
}