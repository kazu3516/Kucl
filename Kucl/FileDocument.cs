using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace Kucl {

    #region FileDocument
    /// <summary>
    /// ドキュメント処理を行うためのクラスです。
    /// <para>このクラスはWindowsFormsアプリケーションで使用することが想定されたクラスです。WPFで使用する場合、FileDocumentBaseクラスを継承したクラスを使用してください。</para>
    /// </summary>
    public abstract class FileDocument : FileDocumentBase {

        #region メンバ変数

        private string m_Filter;

        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;


        #endregion

        #region コンストラクタ・デストラクタ
        /// <summary>
        /// FileDocumentオブジェクトを初期化します。
        /// </summary>
        public FileDocument() {
            this.saveFileDialog = new SaveFileDialog();
            this.openFileDialog = new OpenFileDialog();

            this.RequestOpenFileDialog += this.FileDocument_RequestOpenFileDialog;
            this.RequestSaveFileDialog += this.FileDocument_RequestSaveFileDialog;
            this.RequestConfirmCloseMessageDialog += this.FileDocument_RequestConfirmCloseMessageDialog;
        }

        #endregion
        
        #region イベントハンドラ

        private void FileDocument_RequestConfirmCloseMessageDialog(object sender,MessageBoxEventArgs e) {
            DialogResult result = MessageBox.Show(e.Message,e.Title,MessageBoxButtons.YesNoCancel,MessageBoxIcon.Information);
            switch(result) {
                case DialogResult.Yes:
                    e.Result = MessageBoxResult.Yes;
                    break;
                case DialogResult.No:
                    e.Result = MessageBoxResult.No;
                    break;
                case DialogResult.Cancel:
                    e.Result = MessageBoxResult.Cancel;
                    break;
                default:
                    e.Result = MessageBoxResult.None;
                    break;
            }
        }

        private void FileDocument_RequestSaveFileDialog(object sender,FileDialogEventArgs e) {
            this.saveFileDialog.Filter = e.Filter;
            this.saveFileDialog.FilterIndex = e.FilterIndex;
            if(this.saveFileDialog.ShowDialog() == DialogResult.OK) {
                e.FileName = this.saveFileDialog.FileName;
            }
            else {
                e.Canceled = true;
            }
        }
        private void FileDocument_RequestOpenFileDialog(object sender,FileDialogEventArgs e) {
            this.openFileDialog.Filter = e.Filter;
            this.openFileDialog.FilterIndex = e.FilterIndex;
            if(this.openFileDialog.ShowDialog() == DialogResult.OK) {
                e.FileName = this.openFileDialog.FileName;
            }
            else {
                e.Canceled = true;
            }
        }
        #endregion

        #region プロパティ

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
        /// <summary>
        /// FilterIndexを取得または設定します。
        /// </summary>
        public int FilterIndex {
            get;set;
        }
        #endregion

        /// <summary>
        /// OnRequestOpenFileDialogメソッドをオーバーライドします。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRequestOpenFileDialog(FileDialogEventArgs e) {
            e.Filter = this.Filter;
            e.FilterIndex = this.FilterIndex;
            base.OnRequestOpenFileDialog(e);
        }
        /// <summary>
        /// OnRequestSaveFileDialogメソッドをオーバーライドします。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRequestSaveFileDialog(FileDialogEventArgs e) {
            e.Filter = this.Filter;
            e.FilterIndex = this.FilterIndex;
            base.OnRequestSaveFileDialog(e);
        }
    }
    #endregion

    #region FileDocumentBase
    /// <summary>
    /// ドキュメント処理を行うための機能を提供するクラスです。
    /// <para>WindowsFormアプリケーションで使用する場合、FileDocumentクラスを使用してください。
    /// WPFで使用する場合、このクラスを継承し、DialogServiceを実装してください。
    /// </para>
    /// </summary>
    public abstract class FileDocumentBase : IDisposable {

        #region メンバ変数

        private bool m_Dirty;
        private string m_FileName;


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
        public FileDocumentBase() {
            this.ConfirmCloseDocument_Message = "{0}は変更されています。保存しますか？";
            this.ConfirmCloseDocument_Title = "プロジェクトの保存確認";
            this.NewFileName = "New File";
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには
        /// <summary>
        /// このオブジェクトが保持するリソースを破棄します。
        /// </summary>
        /// <param name="disposing">マネージドリソースを破棄する場合true。アンマネージドリソースのみを破棄する場合falseを指定します。</param>
        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~FileDocumentBase()
        // {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        /// <summary>
        /// このオブジェクトが保持するリソースを破棄します。
        /// </summary>
        public void Dispose() {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion

        #region イベント
        /// <summary>
        /// OpenFileDialogの表示要求が発生した時に動作するイベントです。
        /// </summary>
        public event FileDialogEventHandler RequestOpenFileDialog;
        /// <summary>
        /// RequestOpenFileDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestOpenFileDialog(FileDialogEventArgs e) {
            this.RequestOpenFileDialog?.Invoke(this,e);
        }

        /// <summary>
        /// SaveFileDialogの表示要求が発生した時に動作するイベントです。
        /// </summary>
        public event FileDialogEventHandler RequestSaveFileDialog;
        /// <summary>
        /// RequestSaveFileDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestSaveFileDialog(FileDialogEventArgs e) {
            this.RequestSaveFileDialog?.Invoke(this,e);
        }

        /// <summary>
        /// ConfirmCloseMessageDialogの表示要求が発生した時に動作するイベントです。
        /// </summary>
        public event MessageBoxEventHandler RequestConfirmCloseMessageDialog;
        /// <summary>
        /// RequestConfirmCloseMessageDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestConfirmCloseMessageDialog(MessageBoxEventArgs e) {
            this.RequestConfirmCloseMessageDialog?.Invoke(this,e);
        }
        #endregion

        #region プロパティ

        #region FileName
        /// <summary>
        /// ファイル名を取得、設定します。
        /// </summary>
        public string FileName {
            get {
                return m_FileName;
            }
            set {
                if(this.m_FileName != value) {
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
            this.FileNameChanged?.Invoke(this,e);
        }
        #endregion

        #region Dirty
        /// <summary>
        /// ドキュメントが変更されたかどうかを取得、設定します。
        /// </summary>
        public virtual bool Dirty {
            get {
                return m_Dirty;
            }
            set {
                if(this.m_Dirty != value) {
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
            this.DirtyChanged?.Invoke(this,e);
        }
        #endregion

        #endregion

        #region ConfirmCloseDocumentメソッド
        /// <summary>
        /// ドキュメントを閉じます。
        /// </summary>
        /// <returns></returns>
        protected bool ConfirmCloseDocument() {
            if(this.Dirty) {
                MessageBoxEventArgs e = new MessageBoxEventArgs()
                {
                    Message = string.Format(this.ConfirmCloseDocument_Message,this.FileName),
                    Title = this.ConfirmCloseDocument_Title
                };
                this.OnRequestConfirmCloseMessageDialog(e);
                switch(e.Result) {
                    case MessageBoxResult.Yes:
                        return this.SaveDocument();
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Cancel:
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
            FileDialogEventArgs e = new FileDialogEventArgs()
            {
                FileName = this.FileName
            };
            this.OnRequestSaveFileDialog(e);
            if(!e.Canceled) {
                return this.SaveAsDocument(e.FileName);
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

            //シングルドキュメントの場合、現在のドキュメントを閉じる
            if(!this.ConfirmCloseDocument()) {
                return false;
            }
            FileDialogEventArgs e = new FileDialogEventArgs()
            {
                FileName = this.FileName
            };
            this.OnRequestOpenFileDialog(e);
            if(!e.Canceled) {
                return this.OpenDocument(e.FileName,false);
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
            if(!ConfirmCloseDocument()) {
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
    #endregion


    #region FileDialogEvent
    /// <summary>
    /// FileDialogEventで使用するDelegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FileDialogEventHandler(object sender,FileDialogEventArgs e);

    #region FileDialogEventArgs
    /// <summary>
    /// FileDialogEventで使用するEventArgs
    /// </summary>
    public class FileDialogEventArgs : EventArgs {

        #region メンバ変数/プロパティ
        /// <summary>
        /// FileNameを取得、または設定します。
        /// </summary>
        public string FileName {
            get; set;
        }
        /// <summary>
        /// Canceledを取得または設定します。
        /// </summary>
        public bool Canceled {
            get; set;
        }
        /// <summary>
        /// Filterを取得または設定します。
        /// </summary>
        public string Filter {
            get;set;
        }
        /// <summary>
        /// FilterIndexを取得または設定します。
        /// </summary>
        public int FilterIndex {
            get;set;
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// FileDialogEventArgsクラスの新しいインスタンスを初期化します。
        /// </summary>
        public FileDialogEventArgs() : base() {
        }
        #endregion
    }
    #endregion

    #endregion

    #region MessageBoxEvent
    /// <summary>
    /// MessageBoxEventで使用するDelegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MessageBoxEventHandler(object sender,MessageBoxEventArgs e);

    #region MessageBoxEventArgs
    /// <summary>
    /// MessageBoxEventで使用するEventArgs
    /// </summary>
    public class MessageBoxEventArgs : EventArgs {

        #region メンバ変数/プロパティ
        /// <summary>
        /// Messageを取得または設定します。
        /// </summary>
        public string Message {
            get; set;
        }
        /// <summary>
        /// Titleを取得または設定します。
        /// </summary>
        public string Title {
            get; set;
        }
        /// <summary>
        /// Resultを取得または設定します。
        /// </summary>
        public MessageBoxResult Result {
            get; set;
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// MessageBoxEventArgsクラスの新しいインスタンスを初期化します。
        /// </summary>
        public MessageBoxEventArgs() : base() {

        }
        #endregion
    }
    #endregion

    #endregion

    #region MessageBoxResult
    /// <summary>
    /// MessageBoxの結果を表す列挙型です。
    /// </summary>
    public enum MessageBoxResult {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// OK
        /// </summary>
        OK,
        /// <summary>
        /// Cancel
        /// </summary>
        Cancel,
        /// <summary>
        /// Yes(はい)
        /// </summary>
        Yes,
        /// <summary>
        /// No(いいえ)
        /// </summary>
        No,
        /// <summary>
        /// Abort(中止)
        /// </summary>
        Abort,
        /// <summary>
        /// Ignore(無視)
        /// </summary>
        Ignore,
        /// <summary>
        /// Retry(再試行)
        /// </summary>
        Retry,
    } 
    #endregion

    #region FileDocumentDialogService

    /// <summary>
    /// WPFでFileDocumentBaseを使用するために、ViewModelとViewの通信を行うServiceクラスです。
    /// </summary>
    public class FileDocumentDialogService {

        private static Dictionary<string,FileDocumentDialogService> table = new Dictionary<string,FileDocumentDialogService>();
        /// <summary>
        /// 識別子を指定してServiceを取得します。
        /// </summary>
        /// <param name="identifer"></param>
        /// <returns></returns>
        public static FileDocumentDialogService GetService(string identifer) {
            if(!table.ContainsKey(identifer)) {
                table.Add(identifer,new FileDocumentDialogService());
            }
            return table[identifer];
        }

        /// <summary>
        /// 外部からのコンストラクタ呼び出しは不可。GetServiceメソッドから呼び出す。
        /// </summary>
        private FileDocumentDialogService() {
        }

        #region イベント
        /// <summary>
        /// OpenFileDialogの表示要求が発生した時に動作するイベントです。
        /// </summary>
        public event FileDialogEventHandler RequestOpenFileDialog;
        /// <summary>
        /// RequestOpenFileDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestOpenFileDialog(FileDialogEventArgs e) {
            this.RequestOpenFileDialog?.Invoke(this,e);
        }

        /// <summary>
        /// SaveFileDialogの表示要求が発生した時に動作するイベントです。
        /// </summary>
        public event FileDialogEventHandler RequestSaveFileDialog;
        /// <summary>
        /// RequestSaveFileDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestSaveFileDialog(FileDialogEventArgs e) {
            this.RequestSaveFileDialog?.Invoke(this,e);
        }

        /// <summary>
        /// ConfirmCloseMessageDialogの表示要求が発生した時に動作するイベントです。
        /// </summary>
        public event MessageBoxEventHandler RequestConfirmCloseMessageDialog;
        /// <summary>
        /// RequestConfirmCloseMessageDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestConfirmCloseMessageDialog(MessageBoxEventArgs e) {
            this.RequestConfirmCloseMessageDialog?.Invoke(this,e);
        }
        #endregion

        /// <summary>
        /// RequestOpenFileDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        public void RaiseRequestOpenFileDialog(FileDialogEventArgs e) {
            this.OnRequestOpenFileDialog(e);
        }
        /// <summary>
        /// RequestSaveFileDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        public void RaiseRequestSaveFileDialog(FileDialogEventArgs e) {
            this.OnRequestSaveFileDialog(e);
        }
        /// <summary>
        /// RequestConfirmCloseMessageDialogイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>

        public void RaiseRequestConfirmCloseMessageDialog(MessageBoxEventArgs e) {
            this.OnRequestConfirmCloseMessageDialog(e);
        }

    }
    #endregion

}
