using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ctrl = System.Windows.Forms.Control;

namespace Kucl.Forms.Controls.Wizard {

    /// <summary>
    /// TabControlに各ステップのUIを定義できるウィザードコントロールです。
    /// Startメソッドを呼ぶと、TabControlを非表示にし、TabPageの内容を順に表示します。
    /// 
    /// 
    /// <para>Anchorプロパティ等を使用したレイアウトをコントロールするためには、TabPage直下にPanelを定義し、ユーザはPanel上にコントロールを配置してください。</para>
    /// </summary>
    public partial class EditableWizardControl : UserControl {

        //NOTE:InitializePageで各TabPageのコントロールをPanelに移し替えているが、Anchor等のレイアウトが一致しない。手動でPanelを追加し、その上にレイアウトすることで、パネルの移し替えが発生しなくなるため、回避できる。（条件：TabPage.Controls.Count == 1 && TabPage.Controls[0] is Panelであること）

        #region フィールド(メンバ変数、プロパティ、イベント)

        private Panel m_CurrentPanel;


        #region protected BeforePageChangeイベント
        /// <summary>
        /// Pageが変更される前に発生するイベントです。
        /// </summary>
        protected event BeforePageChangeEventHandler BeforePageChange;
        /// <summary>
        /// BeforePageChangeイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnBeforePageChange(BeforePageChangeEventArgs e) {
            this.BeforePageChange?.Invoke(this, e);
        } 
        #endregion

        #region protected AfterPageChangeイベント
        /// <summary>
        /// Pageが変更された後に発生するイベントです。
        /// </summary>
        protected event AfterPageChangeEventHandler AfterPageChange;
        /// <summary>
        /// AfterPageChangeイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnAfterPageChange(AfterPageChangeEventArgs e) {
            this.AfterPageChange?.Invoke(this, e);
        }
        #endregion


        #region WizardFinishedイベント
        /// <summary>
        /// ウィザードが終了した時に発生するイベントです。
        /// </summary>
        public event EventHandler WizardFinished;
        /// <summary>
        /// WizardFinishedイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnWizardFinished(EventArgs e) {
            if (this.WizardFinished != null) {
                this.WizardFinished(this, e);
            }
        }
        #endregion

        #region WizardCanceledイベント
        /// <summary>
        /// ウィザードがキャンセルされたときに発生するイベントです。
        /// </summary>
        public event EventHandler WizardCanceled;
        /// <summary>
        /// WizardCanceledイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnWizardCanceled(EventArgs e) {
            if (this.WizardCanceled != null) {
                this.WizardCanceled(this, e);
            }
        } 
        #endregion



        #region CurrentStepNo
        private int m_CurrentStepNo;
        /// <summary>
        /// CurrentStepNoを取得します。
        /// </summary>
        [Browsable(false)]
        public int CurrentStepNo {
            get {
                return this.m_CurrentStepNo;
            }
        }
        #endregion

        #region Initialized
        private bool m_Initialized;
        /// <summary>
        /// Initializedを取得、設定します。
        /// </summary>
        [Browsable(false)]
        public bool Initialized {
            get {
                return this.m_Initialized;
            }
            set {
                this.m_Initialized = value;
            }
        }
        #endregion

        #region TabPages
        /// <summary>
        /// ウィザードに使用するコントロールを一時的に格納するTabPageのコレクションを取得します。
        /// </summary>
        public TabControl.TabPageCollection TabPages {
            get {
                return this.tabControl1.TabPages;
            }
        }

        #endregion


        #region CanGoNext
        private bool m_CanGoNext;
        /// <summary>
        /// 次へ進めるかどうかを取得します。
        /// </summary>
        [Browsable(false)]
        public bool CanGoNext {
            get {
                return this.m_CanGoNext && this.m_CurrentStepNo < this.tabControl1.TabPages.Count - 1;
            }
            protected set {
                this.m_CanGoNext = value;
                this.ChangeButtonStatus();
            }
        }
        #endregion

        #region CanGoBack
        private bool m_CanGoBack;
        /// <summary>
        /// 前に戻れるかどうかを取得します。
        /// </summary>
        [Browsable(false)]
        public bool CanGoBack {
            get {
                return this.m_CanGoBack && 0 < this.m_CurrentStepNo;
            }
            protected set {
                this.m_CanGoBack = value;
                this.ChangeButtonStatus();
            }
        }

        #endregion

        #region CanCancel
        private bool m_CanCancel;
        /// <summary>
        /// Cancelボタンが有効かどうかを取得します。
        /// </summary>
        public bool CanCancel {
            get {
                return this.m_CanCancel;
            }
            protected set {
                this.m_CanCancel = value;
                this.ChangeButtonStatus();
            }
        }
        #endregion



        #endregion

        #region コンストラクタ

        /// <summary>
        /// EditableWizardControlオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public EditableWizardControl() {
            InitializeComponent();

            this.m_CurrentStepNo = 0;
            this.m_Initialized = false;
            this.m_CanGoNext = true;
            this.m_CanGoBack = true;
            this.m_CanCancel = true;

            this.m_CurrentPanel = null;

        }

        #endregion

        #region イベントハンドラ

        /// <summary>
        /// 次へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goNextButton_Click(object sender, EventArgs e) {
            if (this.CanGoNext) {
                this.GoNext();
            }
            else {
                this.Finish();
            }
        }
        /// <summary>
        /// 戻るボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goBackButton_Click(object sender, EventArgs e) {
            this.GoBack();

        }
        /// <summary>
        /// キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e) {
            this.Cancel();
        }

        #endregion

        #region ウィザードを操作するメソッド

        #region Initialize
        /// <summary>
        /// ウィザードの初期化を行います。
        /// Startメソッドを呼ぶ前にこのメソッドを呼び出す必要があります。
        /// </summary>
        public void Initialize() {
            //TabPageに配置されているコントロールを、Panelにまとめておく
            this.tabControl1.TabPages.Cast<TabPage>().ToList().ForEach(page => this.InitializePage(page));

            this.m_Initialized = true;
        }

        /// <summary>
        /// 各ページの初期化を行います。
        /// </summary>
        /// <param name="page"></param>
        private void InitializePage(TabPage page) {
            if (page.Controls.Count != 1 || (page.Controls.Count == 1 && !(page.Controls[0] is Panel))) {

                //Panelを作成し、コントロールを移動
                Panel panel = new Panel();
                panel.BorderStyle = BorderStyle.None;
                panel.Size = this.Size;

                panel.SuspendLayout();
                panel.Controls.AddRange(page.Controls.Cast<ctrl>().ToArray());
                panel.ResumeLayout();

                //もとのコントロールは移動されるため削除不要
                //page.Controls.Clear();

                page.Controls.Add(panel);
            }
        } 
        #endregion

        /// <summary>
        /// ウィザードを開始します。
        /// このメソッドを呼ぶ前にInitializeメソッドが呼び出されていない場合、例外が発生します。
        /// </summary>
        public void Start() {
            if (!this.Initialized) {
                throw new InvalidOperationException($"Initialize method should be called , before Start method is called.");
            }
            this.tabControl1.Visible = false;

            if (this.ChangePage(-1, 0)) {

                this.OnStart();

            }
        }

        /// <summary>
        /// 1ページ進みます。
        /// </summary>
        public void GoNext() {
            if (!this.CanGoNext) {
                return;
            }

            if (this.ChangePage(this.m_CurrentStepNo, m_CurrentStepNo + 1)){

                this.OnGoNext();

            }
        }

        /// <summary>
        /// 1ページ戻ります。
        /// </summary>
        public void GoBack() {
            if (!this.CanGoBack) {
                return;
            }

            if (this.ChangePage(this.m_CurrentStepNo, m_CurrentStepNo - 1)) {

                this.OnGoBack();

            }
        }
        /// <summary>
        /// ウィザードをキャンセルします。
        /// </summary>
        public void Cancel() {
            if (!this.CanCancel) {
                return;
            }
            
            int canceledStepNo = this.m_CurrentStepNo;

            if (this.ChangePage(this.m_CurrentStepNo, -1)) {

                this.OnCancel(canceledStepNo);

                this.OnWizardCanceled(EventArgs.Empty);

            }
        }

        /// <summary>
        /// ウィザードを終了します。
        /// </summary>
        public void Finish() {

            if (this.ChangePage(this.m_CurrentStepNo, -1)) {

                this.OnFinish();

                this.OnWizardFinished(EventArgs.Empty);

            }
        }


        #region ChangePage
        private bool ChangePage(int prevPageIndex, int nextPageIndex) {
            //nextPageはBeforePageChangeイベントハンドラにて変更される可能性があるため、変数にする
            int nextPageIndex2 = nextPageIndex;

            BeforePageChangeEventArgs e = new BeforePageChangeEventArgs(prevPageIndex, nextPageIndex2);
            this.OnBeforePageChange(e);

            //変更されたNextPageの検証
            bool validNextPage = (0 <= nextPageIndex2 && nextPageIndex2 < this.TabPages.Count);
            //終了判定
            bool finishedPage = (nextPageIndex2 == -1);
            //Page変更しない場合、falseを返す
            if (e.Cancel || (!validNextPage && !finishedPage) ) {
                return false;
            }

            //遷移条件のリセット
            this.m_CanGoBack = true;
            this.m_CanGoNext = true;
            this.m_CanCancel = true;

            //===========================================
            //現在表示しているページを元に戻す
            if (0 <= prevPageIndex) {
                TabPage prevPage = this.TabPages[prevPageIndex];
                prevPage.Controls.Add(this.m_CurrentPanel);
            }

            //===========================================
            if (validNextPage) {
                TabPage nextPage = this.TabPages[nextPageIndex2];
                //次に表示するページの状態確認
                if (nextPage.Controls.Count != 1) {
                    this.InitializePage(nextPage);
                }
                this.m_CurrentPanel = nextPage.Controls[0] as Panel;
                this.m_CurrentStepNo = nextPageIndex2;

                //---------------------------------
                this.SuspendLayout();
                this.MainPanel.SuspendLayout();

                this.m_CurrentPanel.Dock = DockStyle.Fill;

                this.MainPanel.Controls.Add(this.m_CurrentPanel);
                this.MainPanel.ResumeLayout();
                this.ResumeLayout();
                //---------------------------------
            }

            this.ChangeButtonStatus();

            this.OnAfterPageChange(new AfterPageChangeEventArgs(prevPageIndex,nextPageIndex2));

            return true;
        }
        #endregion

        #region ChangeButtonStatus
        /// <summary>
        /// ボタンの状態を変更します。
        /// </summary>
        private void ChangeButtonStatus() {
            if (this.m_CurrentStepNo == this.TabPages.Count - 1) {
                this.SetGoNextButtonText("完了");
                this.goNextButton.Enabled = true;
            }
            else {
                this.SetGoNextButtonText("次へ");
                this.goNextButton.Enabled = this.CanGoNext;
            }
            
            this.goBackButton.Enabled = this.CanGoBack;
            this.cancelButton.Enabled = this.CanCancel;
        }

        /// <summary>
        /// 次へボタンのテキストを設定します。
        /// 派生クラスでオーバーライドされると、次へボタンのテキストを独自に変更することができます。
        /// </summary>
        /// <param name="text"></param>
        protected virtual void SetGoNextButtonText(string text) {
            this.goNextButton.Text = text;
        }

        #endregion

        #endregion

        #region 各ページ遷移のタイミングで呼び出されるvirtualメソッド
        /// <summary>
        /// 派生クラスでオーバーライドされると、Start時の動作を定義します。
        /// このメソッドが呼び出されたタイミングでは、CurrentStepNoプロパティの値は0が設定されています。
        /// </summary>
        protected virtual void OnStart() {
        }
        /// <summary>
        /// 派生クラスでオーバーライドされると、GoNext時の動作を定義します。
        /// このメソッドが呼び出されたタイミングでは、CurrentStepNoプロパティの値は次のページに移っています。
        /// </summary>
        protected virtual void OnGoNext() {
        }
        /// <summary>
        /// 派生クラスでオーバーライドされると、GoBack時の動作を定義します。
        /// このメソッドが呼び出されたタイミングでは、CurrentStepNoプロパティの値は前のページに移っています。
        /// /// </summary>
        protected virtual void OnGoBack() {
        }
        /// <summary>
        /// 派生クラスでオーバーライドされると、Cancel時の動作を定義します。
        /// このメソッドが呼び出されたタイミングでは、CurrentStepNoプロパティの値は-1が設定されています。
        /// キャンセル前のStepNoは引数 canceledStepNoとして渡されます。
        /// </summary>
        /// <param name="canceledStepNo"></param>
        protected virtual void OnCancel(int canceledStepNo) {
        }
        /// <summary>
        /// 派生クラスでオーバーライドされると、Finish時の動作を定義します。
        /// このメソッドが呼び出されたタイミングでは、CurrentStepNoプロパティの値は-1が設定されています。
        /// Finishは必ず最終Stepで実行されます。最終StepNoはTabPages.Count - 1を使用してください。
        /// </summary>
        protected virtual void OnFinish() {
        }
        #endregion

    }

    #region Before/After PageChangeイベント

    #region  EventHandler
    /// <summary>
    /// BeforePageChangeイベントのデリゲートです。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void BeforePageChangeEventHandler(object sender, BeforePageChangeEventArgs e);

    /// <summary>
    /// AfterPageChangeイベントのデリゲートです。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AfterPageChangeEventHandler(object sender, AfterPageChangeEventArgs e);
    
    #endregion

    #region BeforePageChangeEventArgs
    /// <summary>
    /// BeforePageChangeイベントのデータを格納するクラスです。
    /// イベントハンドラにてCancel、NextPageの設定を行うことで、ページ遷移のコントロールを行うことができます。
    /// </summary>
    public class BeforePageChangeEventArgs:EventArgs {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #region PreviousPage
        private int m_PreviousPage;
        /// <summary>
        /// PreviousPageを取得します。
        /// </summary>
        public int PreviousPage {
            get {
                return this.m_PreviousPage;
            }
        }
        #endregion

        #region NextPage
        private int m_NextPage;
        /// <summary>
        /// NextPageを取得、設定します。
        /// </summary>
        public int NextPage {
            get {
                return this.m_NextPage;
            }
            set {
                this.m_NextPage = value;
            }
        }
        #endregion

        #region Cancel
        private bool m_Cancel;
        /// <summary>
        /// Cancelを取得、設定します。
        /// </summary>
        public bool Cancel {
            get {
                return this.m_Cancel;
            }
            set {
                this.m_Cancel = value;
            }
        }
        #endregion


        #endregion

        #region コンストラクタ
        /// <summary>
        /// BeforePageChangeEventArgsオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="prevPage"></param>
        /// <param name="nextPage"></param>
        public BeforePageChangeEventArgs(int prevPage,int nextPage) {
            this.m_PreviousPage = prevPage;
            this.m_NextPage = nextPage;
            this.m_Cancel = false;
        }
        #endregion

    }
    #endregion

    #region AfterPageChangeEventArgs
    /// <summary>
    /// AfterPageChangeイベントのデータを格納するクラスです。
    /// </summary>
    public class AfterPageChangeEventArgs : EventArgs {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #region PreviousPage
        private int m_PreviousPage;
        /// <summary>
        /// PreviousPageを取得します。
        /// </summary>
        public int PreviousPage {
            get {
                return this.m_PreviousPage;
            }
        }
        #endregion

        #region NextPage
        private int m_NextPage;
        /// <summary>
        /// NextPageを取得します。
        /// </summary>
        public int NextPage {
            get {
                return this.m_NextPage;
            }
        }
        #endregion


        #endregion

        #region コンストラクタ
        /// <summary>
        /// AfterPageChangeEventArgsオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="prevPage"></param>
        /// <param name="nextPage"></param>
        public AfterPageChangeEventArgs(int prevPage, int nextPage) {
            this.m_PreviousPage = prevPage;
            this.m_NextPage = nextPage;
        }
        #endregion

    }
    #endregion

    #endregion

}
