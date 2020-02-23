using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace Kucl {

    #region FileDocument
    /// <summary>
    /// �h�L�������g�������s�����߂̃N���X�ł��B
    /// <para>���̃N���X��WindowsForms�A�v���P�[�V�����Ŏg�p���邱�Ƃ��z�肳�ꂽ�N���X�ł��BWPF�Ŏg�p����ꍇ�AFileDocumentBase�N���X���p�������N���X���g�p���Ă��������B</para>
    /// </summary>
    public abstract class FileDocument : FileDocumentBase {

        #region �����o�ϐ�

        private string m_Filter;

        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;


        #endregion

        #region �R���X�g���N�^�E�f�X�g���N�^
        /// <summary>
        /// FileDocument�I�u�W�F�N�g�����������܂��B
        /// </summary>
        public FileDocument() {
            this.saveFileDialog = new SaveFileDialog();
            this.openFileDialog = new OpenFileDialog();

            this.RequestOpenFileDialog += this.FileDocument_RequestOpenFileDialog;
            this.RequestSaveFileDialog += this.FileDocument_RequestSaveFileDialog;
            this.RequestConfirmCloseMessageDialog += this.FileDocument_RequestConfirmCloseMessageDialog;
        }

        #endregion
        
        #region �C�x���g�n���h��

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
            this.saveFileDialog.Filter = this.Filter;
            if(this.saveFileDialog.ShowDialog() == DialogResult.OK) {
                e.FileName = this.saveFileDialog.FileName;
            }
            else {
                e.Canceled = true;
            }
        }
        private void FileDocument_RequestOpenFileDialog(object sender,FileDialogEventArgs e) {
            this.openFileDialog.Filter = this.Filter;
            if(this.openFileDialog.ShowDialog() == DialogResult.OK) {
                e.FileName = this.openFileDialog.FileName;
            }
            else {
                e.Canceled = true;
            }
        }
        #endregion

        #region �v���p�e�B

        /// <summary>
        /// �t�@�C��������s�����߂̃t�B���^�[���擾�A�ݒ肵�܂��B
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

    }
    #endregion

    #region FileDocumentBase
    public abstract class FileDocumentBase : IDisposable {

        #region �����o�ϐ�

        private bool m_Dirty;
        private string m_FileName;


        /// <summary>
        /// �h�L�������g����鎞�ɕ\������m�F�_�C�A���O�̃��b�Z�[�W��\���܂��B
        /// </summary>
        protected string ConfirmCloseDocument_Message;
        /// <summary>
        /// �h�L�������g����鎞�ɕ\������m�F�_�C�A���O�̃^�C�g����\���܂��B
        /// </summary>
        protected string ConfirmCloseDocument_Title;
        /// <summary>
        /// �V�K�h�L�������g�̃t�@�C������\���܂��B
        /// </summary>
        protected string NewFileName;




        #endregion

        #region �R���X�g���N�^�E�f�X�g���N�^
        /// <summary>
        /// FileDocument�I�u�W�F�N�g�����������܂��B
        /// </summary>
        public FileDocumentBase() {
            this.ConfirmCloseDocument_Message = "{0}�͕ύX����Ă��܂��B�ۑ����܂����H";
            this.ConfirmCloseDocument_Title = "�v���W�F�N�g�̕ۑ��m�F";
            this.NewFileName = "New File";
        }

        #region IDisposable Support
        private bool disposedValue = false; // �d������Ăяo�������o����ɂ�

        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    // TODO: �}�l�[�W��Ԃ�j�����܂� (�}�l�[�W �I�u�W�F�N�g)�B
                }

                // TODO: �A���}�l�[�W ���\�[�X (�A���}�l�[�W �I�u�W�F�N�g) ��������A���̃t�@�C�i���C�U�[���I�[�o�[���C�h���܂��B
                // TODO: �傫�ȃt�B�[���h�� null �ɐݒ肵�܂��B

                disposedValue = true;
            }
        }

        // TODO: ��� Dispose(bool disposing) �ɃA���}�l�[�W ���\�[�X���������R�[�h���܂܂��ꍇ�ɂ̂݁A�t�@�C�i���C�U�[���I�[�o�[���C�h���܂��B
        // ~FileDocumentBase()
        // {
        //   // ���̃R�[�h��ύX���Ȃ��ł��������B�N���[���A�b�v �R�[�h����� Dispose(bool disposing) �ɋL�q���܂��B
        //   Dispose(false);
        // }

        // ���̃R�[�h�́A�j���\�ȃp�^�[���𐳂��������ł���悤�ɒǉ�����܂����B
        public void Dispose() {
            // ���̃R�[�h��ύX���Ȃ��ł��������B�N���[���A�b�v �R�[�h����� Dispose(bool disposing) �ɋL�q���܂��B
            Dispose(true);
            // TODO: ��̃t�@�C�i���C�U�[���I�[�o�[���C�h�����ꍇ�́A���̍s�̃R�����g���������Ă��������B
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion

        #region �C�x���g

        public event FileDialogEventHandler RequestOpenFileDialog;
        protected void OnRequestOpenFileDialog(FileDialogEventArgs e) {
            this.RequestOpenFileDialog?.Invoke(this,e);
        }


        public event FileDialogEventHandler RequestSaveFileDialog;
        protected void OnRequestSaveFileDialog(FileDialogEventArgs e) {
            this.RequestSaveFileDialog?.Invoke(this,e);
        }


        public event MessageBoxEventHandler RequestConfirmCloseMessageDialog;
        protected void OnRequestConfirmCloseMessageDialog(MessageBoxEventArgs e) {
            this.RequestConfirmCloseMessageDialog?.Invoke(this,e);
        }
        #endregion

        #region �v���p�e�B

        #region FileName
        /// <summary>
        /// �t�@�C�������擾�A�ݒ肵�܂��B
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
        /// FileName�v���p�e�B���ύX���ꂽ���ɔ�������C�x���g�ł��B
        /// </summary>
        public event EventHandler FileNameChanged;
        /// <summary>
        /// FileNameChanged�C�x���g����������Ƃ��ɌĂяo����郁�\�b�h�ł��B
        /// </summary>
        /// <param name="e"></param>
        protected void OnFileNameChanged(EventArgs e) {
            this.FileNameChanged?.Invoke(this,e);
        }
        #endregion

        #region Dirty
        /// <summary>
        /// �h�L�������g���ύX���ꂽ���ǂ������擾�A�ݒ肵�܂��B
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
        /// Dirty�v���p�e�B���ύX���ꂽ���ɔ�������C�x���g�ł��B
        /// </summary>
        public event EventHandler DirtyChanged;
        /// <summary>
        /// DirtyChanged�C�x���g����������Ƃ��ɌĂяo����郁�\�b�h�ł��B
        /// </summary>
        /// <param name="e"></param>
        protected void OnDirtyChanged(EventArgs e) {
            this.DirtyChanged?.Invoke(this,e);
        }
        #endregion

        #endregion

        #region ConfirmCloseDocument���\�b�h
        /// <summary>
        /// �h�L�������g����܂��B
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

        #region SaveDocument���\�b�h
        /// <summary>
        /// �㏑���ۑ����܂��B
        /// </summary>
        /// <returns></returns>
        public virtual bool SaveDocument() {
            if(File.Exists(this.FileName)) {
                return this.SaveAsDocument(this.FileName);
            }
            return this.SaveAsDocument();
        }
        #endregion

        #region SaveAsDocument���\�b�h
        /// <summary>
        /// ���O��t���ĕۑ����܂��B
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
        /// �w�肵�����O��t���ĕۑ����܂��B
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

        #region OpenDocument���\�b�h
        /// <summary>
        /// �t�@�C�����J���܂��B
        /// </summary>
        /// <returns></returns>
        public virtual bool OpenDocument() {

            //�V���O���h�L�������g�̏ꍇ�A���݂̃h�L�������g�����
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
        /// �t�@�C�������w�肵�ăt�@�C�����J���܂��B
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public virtual bool OpenDocument(string filename) {
            return this.OpenDocument(filename,true);
        }
        /// <summary>
        /// �t�@�C�����J���܂��B
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="confirmClose">True�Ȃ�΁AConfirmCloseDocument���Ăяo���t���[�֓���</param>
        /// <returns></returns>
        protected virtual bool OpenDocument(string filename,bool confirmClose) {
            //�V���O���h�L�������g�̏ꍇ�A���݂̃h�L�������g�����
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

        #region CreateNewDocument���\�b�h
        /// <summary>
        /// �V�K�쐬���܂��B
        /// </summary>
        /// <returns></returns>
        public virtual bool CreateNewDocument() {
            //�V���O���h�L�������g�̏ꍇ�A���݂̃h�L�������g�����
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

        #region Close���\�b�h
        /// <summary>
        /// �h�L�������g����܂��B
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

        #region ���ہA���z���\�b�h
        /// <summary>
        /// �h�L�������g��ۑ����܂��B
        /// </summary>
        protected abstract void OnSaveFile();
        /// <summary>
        /// �h�L�������g��ǂݍ��݂܂��B
        /// </summary>
        protected abstract void OnLoadFile();
        /// <summary>
        /// �h�L�������g�̐V�K�쐬���s���܂��B
        /// </summary>
        protected abstract void OnCreateNewDocument();
        /// <summary>
        /// �h�L�������g����܂��B
        /// </summary>
        protected abstract void OnCloseDocument();

        #endregion
    } 
    #endregion


    #region FileDialogEvent

    public delegate void FileDialogEventHandler(object sender,FileDialogEventArgs e);

    #region FileDialogEventArgs
    public class FileDialogEventArgs : EventArgs {

        #region �����o�ϐ�/�v���p�e�B
        public string FileName {
            get; set;
        }
        public bool Canceled {
            get; set;
        }
        public string Filter {
            get;set;
        }
        #endregion

        #region �R���X�g���N�^
        public FileDialogEventArgs() : base() {
        }
        #endregion
    }
    #endregion

    #endregion

    #region MessageBoxEvent

    public delegate void MessageBoxEventHandler(object sender,MessageBoxEventArgs e);

    #region MessageBoxEventArgs
    public class MessageBoxEventArgs : EventArgs {

        #region �����o�ϐ�/�v���p�e�B
        public string Message {
            get; set;
        }
        public string Title {
            get; set;
        }
        public MessageBoxResult Result {
            get; set;
        }
        #endregion

        #region �R���X�g���N�^
        public MessageBoxEventArgs() : base() {

        }
        #endregion
    }
    #endregion

    #endregion

    public enum MessageBoxResult {
        None,
        OK,
        Cancel,
        Yes,
        No,
        Abort,
        Ignore,
        Retry,
    }

    #region FileDocumentDialogService

    public class FileDocumentDialogService {

        private static Dictionary<string,FileDocumentDialogService> table = new Dictionary<string,FileDocumentDialogService>();
        public static FileDocumentDialogService GetService(string identifer) {
            if(!table.ContainsKey(identifer)) {
                table.Add(identifer,new FileDocumentDialogService());
            }
            return table[identifer];
        }

        /// <summary>
        /// �O������̃R���X�g���N�^�Ăяo���͕s�BGetService���\�b�h����Ăяo���B
        /// </summary>
        private FileDocumentDialogService() {
        }

        #region �C�x���g
        /// <summary>
        /// OpenFileDialog�̕\���v���������������ɓ��삷��C�x���g�ł��B
        /// </summary>
        public event FileDialogEventHandler RequestOpenFileDialog;
        /// <summary>
        /// RequestOpenFileDialog�C�x���g�𔭐������܂��B
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestOpenFileDialog(FileDialogEventArgs e) {
            this.RequestOpenFileDialog?.Invoke(this,e);
        }

        /// <summary>
        /// SaveFileDialog�̕\���v���������������ɓ��삷��C�x���g�ł��B
        /// </summary>
        public event FileDialogEventHandler RequestSaveFileDialog;
        /// <summary>
        /// RequestSaveFileDialog�C�x���g�𔭐������܂��B
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestSaveFileDialog(FileDialogEventArgs e) {
            this.RequestSaveFileDialog?.Invoke(this,e);
        }

        /// <summary>
        /// ConfirmCloseMessageDialog�̕\���v���������������ɓ��삷��C�x���g�ł��B
        /// </summary>
        public event MessageBoxEventHandler RequestConfirmCloseMessageDialog;
        /// <summary>
        /// RequestConfirmCloseMessageDialog�C�x���g�𔭐������܂��B
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestConfirmCloseMessageDialog(MessageBoxEventArgs e) {
            this.RequestConfirmCloseMessageDialog?.Invoke(this,e);
        }
        #endregion

        /// <summary>
        /// RequestOpenFileDialog�C�x���g�𔭐������܂��B
        /// </summary>
        /// <param name="e"></param>
        public void RaiseRequestOpenFileDialog(FileDialogEventArgs e) {
            this.OnRequestOpenFileDialog(e);
        }
        /// <summary>
        /// RequestSaveFileDialog�C�x���g�𔭐������܂��B
        /// </summary>
        /// <param name="e"></param>
        public void RaiseRequestSaveFileDialog(FileDialogEventArgs e) {
            this.OnRequestSaveFileDialog(e);
        }
        /// <summary>
        /// RequestConfirmCloseMessageDialog�C�x���g�𔭐������܂��B
        /// </summary>
        /// <param name="e"></param>

        public void RaiseRequestConfirmCloseMessageDialog(MessageBoxEventArgs e) {
            this.OnRequestConfirmCloseMessageDialog(e);
        }

    }
    #endregion

}
