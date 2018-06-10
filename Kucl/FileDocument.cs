using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace Kucl {
    /// <summary>
    /// ドキュメント処理を行うためのクラスです。
    /// </summary>
    public abstract class FileDocument:IDisposable {

        #region メンバ変数

        private bool m_Dirty;
        private string m_FileName;
        private string m_Filter;

        private bool m_Disposed;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;

        /// <summary>
        /// ドキュメントを閉じる時に表示する確認ダイアログのメッセージを表します。
        /// </summary>
        protected string ConfirmCloseDocument_Message;
        /// <summary>
        /// ドキュメントを閉じる時に表示する確認ダイアログのタイトルを表します。
        /// </summary>
        protected string ConfirmCloseDocument_Title;
        /// <summary>
        /// 新規ドキュメントのファイル名を表します。
        /// </summary>
        protected string NewFileName;


        #endregion

        #region コンストラクタ・デストラクタ
        /// <summary>
        /// FileDocumentオブジェクトを初期化します。
        /// </summary>
        public FileDocument() {
            this.m_Disposed = false;
            this.ConfirmCloseDocument_Message = "{0}は変更されています。保存しますか？";
            this.ConfirmCloseDocument_Title = "プロジェクトの保存確認";
            this.NewFileName = "New File";
            this.saveFileDialog = new SaveFileDialog();
            this.openFileDialog = new OpenFileDialog();
        }
        /// <summary>
        /// FileDocumentオブジェクトを破棄します。。
        /// </summary>
        ~FileDocument() {
            if(!this.m_Disposed) {
                this.Dispose();
            }
        }
        /// <summary>
        /// FileDocumentオブジェクトを破棄するメソッドです。
        /// </summary>
        public void Dispose() {
            this.OnDispose();
        }
        /// <summary>
        /// 派生クラスでオーバーライドされると、オブジェクトの破棄を実装します。
        /// </summary>
        protected virtual void OnDispose() {
            this.saveFileDialog.Dispose();
            this.openFileDialog.Dispose();
            this.m_Disposed = true;
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// ファイル名を取得、設定します。
        /// </summary>
        public string FileName {
            get {
                return m_FileName;
            }
            set {
                if (this.m_FileName != value) {
                    m_FileName = value;
                    this.OnFileNameChanged(new EventArgs());
                }
            }
        }
        /// <summary>
        /// FileNameプロパティが変更された時に発生するイベントです。
        /// </summary>
        public event EventHandler FileNameChanged;
        /// <summary>
        /// FileNameChangedイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="e"></param>
        protected void OnFileNameChanged(EventArgs e) {
            if (this.FileNameChanged != null) {
                this.FileNameChanged(this, e);
            }
        }

        /// <summary>
        /// ドキュメントが変更されたかどうかを取得、設定します。
        /// </summary>
        public virtual bool Dirty {
            get {
                return m_Dirty;
            }
            set {
                if (this.m_Dirty != value) {
                    m_Dirty = value;
                    this.OnDirtyChanged(new EventArgs());
                }
            }
        }
        /// <summary>
        /// Dirtyプロパティが変更された時に発生するイベントです。
        /// </summary>
        public event EventHandler DirtyChanged;
        /// <summary>
        /// DirtyChangedイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="e"></param>
        protected void OnDirtyChanged(EventArgs e) {
            if (this.DirtyChanged != null) {
                this.DirtyChanged(this, e);
            }
        }

        /// <summary>
        /// ファイル操作を行うためのフィルターを取得、設定します。
        /// </summary>
        public string Filter {
            get {
                return m_Filter;
            }
            set {
                m_Filter = value;
            }
        }

        #endregion

        #region ConfirmCloseDocumentメソッド
        /// <summary>
        /// ドキュメントを閉じます。
        /// </summary>
        /// <returns></returns>
        protected bool ConfirmCloseDocument() {
            if(this.Dirty) {
                DialogResult result = MessageBox.Show(string.Format(this.ConfirmCloseDocument_Message,this.FileName),
                    this.ConfirmCloseDocument_Title,MessageBoxButtons.YesNoCancel,MessageBoxIcon.Information);
                switch(result) {
                    case DialogResult.Yes:
                        return this.SaveDocument();
                    case DialogResult.No:
                        return true;
                    case DialogResult.Cancel:
                        return false;
                }
            }
            return true;
        }
        #endregion

        #region SaveDocumentメソッド
        /// <summary>
        /// 上書き保存します。
        /// </summary>
        /// <returns></returns>
        public virtual bool SaveDocument() {
            if(File.Exists(this.FileName)) {
                return this.SaveAsDocument(this.FileName);
            }
            return this.SaveAsDocument();
        }
        #endregion

        #region SaveAsDocumentメソッド
        /// <summary>
        /// 名前を付けて保存します。
        /// </summary>
        /// <returns></returns>
        public virtual bool SaveAsDocument() {
            this.saveFileDialog.Filter = this.Filter;
            if(this.saveFileDialog.ShowDialog() == DialogResult.OK) {
                return this.SaveAsDocument(this.saveFileDialog.FileName);
            }
            return false;
        }
        /// <summary>
        /// 指定した名前を付けて保存します。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public virtual bool SaveAsDocument(string filename) {
            this.FileName = filename;
            this.OnSaveFile();
            this.Dirty = false;
            return true;
        }
        #endregion

        #region OpenDocumentメソッド
        /// <summary>
        /// ファイルを開きます。
        /// </summary>
        /// <returns></returns>
        public virtual bool OpenDocument() {
            this.openFileDialog.Filter = this.Filter;
            //シングルドキュメントの場合、現在のドキュメントを閉じる
            if(!this.ConfirmCloseDocument()) {
                return false;
            }
            if(this.openFileDialog.ShowDialog() == DialogResult.OK) {
                return this.OpenDocument(this.openFileDialog.FileName,false);
            }
            return false;
        }
        /// <summary>
        /// ファイル名を指定してファイルを開きます。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public virtual bool OpenDocument(string filename) {
            return this.OpenDocument(filename,true);
        }
        /// <summary>
        /// ファイルを開きます。
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="confirmClose">Trueならば、ConfirmCloseDocumentを呼び出すフローへ入る</param>
        /// <returns></returns>
        protected virtual bool OpenDocument(string filename,bool confirmClose) {
            //シングルドキュメントの場合、現在のドキュメントを閉じる
            if(confirmClose && !this.ConfirmCloseDocument()) {
                return false;
            }
            this.OnCloseDocument();
            this.FileName = filename;
            this.OnLoadFile();
            this.Dirty = false;
            return true;
        }

        #endregion

        #region CreateNewDocumentメソッド
        /// <summary>
        /// 新規作成します。
        /// </summary>
        /// <returns></returns>
        public virtual bool CreateNewDocument() {
            //シングルドキュメントの場合、現在のドキュメントを閉じる
            if(!this.ConfirmCloseDocument()) {
                return false;
            }
            this.OnCloseDocument();
            this.FileName = this.NewFileName;
            this.OnCreateNewDocument();
            this.Dirty = false;
            return true;
        }
        #endregion

        #region Closeメソッド
        /// <summary>
        /// ドキュメントを閉じます。
        /// </summary>
        /// <returns></returns>
        public virtual bool Close() {
            if (!ConfirmCloseDocument()) {
                return false;
            }
            this.OnCloseDocument();
            this.FileName = "";
            this.Dirty = false;
            return true;
        }
        #endregion

        #region 抽象、仮想メソッド
        /// <summary>
        /// ドキュメントを保存します。
        /// </summary>
        protected abstract void OnSaveFile();
        /// <summary>
        /// ドキュメントを読み込みます。
        /// </summary>
        protected abstract void OnLoadFile();
        /// <summary>
        /// ドキュメントの新規作成を行います。
        /// </summary>
        protected abstract void OnCreateNewDocument();
        /// <summary>
        /// ドキュメントを閉じます。
        /// </summary>
        protected abstract void OnCloseDocument();
        #endregion

    }

}
