using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace Kucl {
    /// <summary>
    /// �h�L�������g�������s�����߂̃N���X�ł��B
    /// </summary>
    public abstract class FileDocument:IDisposable {

        #region �����o�ϐ�

        private bool m_Dirty;
        private string m_FileName;
        private string m_Filter;

        private bool m_Disposed;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;

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
        public FileDocument() {
            this.m_Disposed = false;
            this.ConfirmCloseDocument_Message = "{0}�͕ύX����Ă��܂��B�ۑ����܂����H";
            this.ConfirmCloseDocument_Title = "�v���W�F�N�g�̕ۑ��m�F";
            this.NewFileName = "New File";
            this.saveFileDialog = new SaveFileDialog();
            this.openFileDialog = new OpenFileDialog();
        }
        /// <summary>
        /// FileDocument�I�u�W�F�N�g��j�����܂��B�B
        /// </summary>
        ~FileDocument() {
            if(!this.m_Disposed) {
                this.Dispose();
            }
        }
        /// <summary>
        /// FileDocument�I�u�W�F�N�g��j�����郁�\�b�h�ł��B
        /// </summary>
        public void Dispose() {
            this.OnDispose();
        }
        /// <summary>
        /// �h���N���X�ŃI�[�o�[���C�h�����ƁA�I�u�W�F�N�g�̔j�����������܂��B
        /// </summary>
        protected virtual void OnDispose() {
            this.saveFileDialog.Dispose();
            this.openFileDialog.Dispose();
            this.m_Disposed = true;
        }
        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �t�@�C�������擾�A�ݒ肵�܂��B
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
        /// FileName�v���p�e�B���ύX���ꂽ���ɔ�������C�x���g�ł��B
        /// </summary>
        public event EventHandler FileNameChanged;
        /// <summary>
        /// FileNameChanged�C�x���g����������Ƃ��ɌĂяo����郁�\�b�h�ł��B
        /// </summary>
        /// <param name="e"></param>
        protected void OnFileNameChanged(EventArgs e) {
            if (this.FileNameChanged != null) {
                this.FileNameChanged(this, e);
            }
        }

        /// <summary>
        /// �h�L�������g���ύX���ꂽ���ǂ������擾�A�ݒ肵�܂��B
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
        /// Dirty�v���p�e�B���ύX���ꂽ���ɔ�������C�x���g�ł��B
        /// </summary>
        public event EventHandler DirtyChanged;
        /// <summary>
        /// DirtyChanged�C�x���g����������Ƃ��ɌĂяo����郁�\�b�h�ł��B
        /// </summary>
        /// <param name="e"></param>
        protected void OnDirtyChanged(EventArgs e) {
            if (this.DirtyChanged != null) {
                this.DirtyChanged(this, e);
            }
        }

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

        #region ConfirmCloseDocument���\�b�h
        /// <summary>
        /// �h�L�������g����܂��B
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
            this.saveFileDialog.Filter = this.Filter;
            if(this.saveFileDialog.ShowDialog() == DialogResult.OK) {
                return this.SaveAsDocument(this.saveFileDialog.FileName);
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
            this.openFileDialog.Filter = this.Filter;
            //�V���O���h�L�������g�̏ꍇ�A���݂̃h�L�������g�����
            if(!this.ConfirmCloseDocument()) {
                return false;
            }
            if(this.openFileDialog.ShowDialog() == DialogResult.OK) {
                return this.OpenDocument(this.openFileDialog.FileName,false);
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
            if (!ConfirmCloseDocument()) {
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

}
