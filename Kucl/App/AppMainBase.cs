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
    /// �A�v���P�[�V�����̃G���g���|�C���g��񋟂���N���X�̊�{�N���X�ł��B
    /// </summary>
    public class AppMainBase {

        #region �萔
        private const string ErrorLogFileName = "Error.log";
        #endregion

        #region �����o�ϐ�
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

        #region �C�x���g
        /// <summary>
        /// Config�̓K�p�������������ɔ�������C�x���g�ł��B
        /// </summary>
        public event EventHandler SetConfigComplete;
        #endregion

        #region �v���p�e�B
        /// <summary>
        /// MainWindow���擾�A�ݒ肵�܂��B
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
        /// AppManager���擾���܂��B
        /// </summary>
		protected AppManager AppManager {
			get {
				return this.m_AppManager;
			}
		}

        #region Config
        /// <summary>
        /// Config�p�b�P�[�W���擾���܂��B
        /// </summary>
        protected xcfg.XmlConfigModel AppConfigs {
            get {
                return this.m_AppConfigs;
            }
        }
        /// <summary>
        /// Config���ݒ肳�ꂽ�ォ�ǂ������擾���܂��B
        /// </summary>
        public bool IsConfigSeted {
            get {
                return this.m_IsConfigSeted;
            }
        }
        /// <summary>
        /// Config���g�p���邩�ǂ������擾�A�ݒ肵�܂��B
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
        /// Config���ۑ������f�B���N�g�������擾�A�ݒ肵�܂��B
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
        /// UseConfigObjects�����J���擾���܂��B
        /// </summary>
        public xcfg.UseConfigObjectCollection UseConfigObjects {
            get {
                return this.m_UseConfigObjects;
            }
        }

        #endregion

        #region ExDLL
        /// <summary>
        /// �ǉ����C�u�����̓ǂݍ��݋@�\���g�p���邩�ǂ������擾�A�ݒ肵�܂��B
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
        /// �ǉ����C�u�����̕ۑ��f�B���N�g�������擾�A�ݒ肵�܂��B
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
        /// �ǉ����C�u�����̕ۑ��f�B���N�g���̃t���p�X���擾�A�ݒ肵�܂��B
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
        /// �G���[���O�t�@�C���̃t���p�X���擾���܂��B
        /// </summary>
        public string ErrorLogFileFullName {
            get {
                return this.m_ErrorLogFileName;
            }
        }

        /// <summary>
        /// OnStart���\�b�h���Ŕ���������O���G���[���O�t�@�C���ɏ������ނ��ǂ���������Bool�l���擾�܂��͐ݒ肵�܂��B
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
        /// �A�v���P�[�V�����J�n���ɍ쐬����f�B���N�g�����̃R���N�V�����ł��B
        /// </summary>
        protected List<string> UserPathCollection {
            get {
                return this.m_UserPathCollection;
            }
        }
        #endregion

        #region �R���X�g���N�^
        /// <summary>
        /// AppMainBase�����������܂��B
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

        #region Start���\�b�h
        /// <summary>
        /// �A�v���P�[�V�������J�n���܂��B
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

		#region �C�x���g�n���h��
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
        /// Config��K�p����IUseConfig�I�u�W�F�N�g��o�^���܂��B
        /// </summary>
        /// <param name="useConfigObject">�o�^����IUseConfig�I�u�W�F�N�g</param>
        public void RegisterUseConfigObject(xcfg.IUseConfig useConfigObject) {
            this.m_UseConfigObjects.Add(useConfigObject);
        }
        #endregion

        #region OnStart
        /// <summary>
        /// ����������
        /// Config��ǂݍ��ރI�u�W�F�N�g�́A
        /// ���̃��\�b�h���Ăяo���ꂽ�u�Ԃ���A���̃��\�b�h�𔲂���܂ł�RegisterUseConfigObject���Ăяo���K�v������܂��B
        /// ���̊��Ԃ́A���̃��\�b�h���I�[�o�[���C�h�����ꍇ�A�I�[�o�[���C�h�������\�b�h�𔲂���O�܂łɂȂ�܂��B
        /// </summary>
        protected virtual void OnStart() {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            this.m_IsConfigSeted = false;
            //�I�u�W�F�N�g�̍\�z
            this.m_UseConfigObjects = new xcfg.UseConfigObjectCollection();
            
            //this.m_MainWindow = new MainWindow();
            this.m_AppConfigs = new xcfg.XmlConfigModel();
			this.m_AppManager = new AppManager(false);
            //�p�X�̐ݒ�A�f�B���N�g���̊m�F
            this.InitializePath();
            this.InitializeDirectory();
        }

        /// <summary>
        /// �w�肵���������UserPathCollection�ɒǉ����A�ǉ����ꂽ�ʒu��Ԃ��܂��B
        /// ���̃��\�b�h��OnStart���\�b�h�Ăяo���O�܂łɌĂяo����邱�Ƃ�O��Ƃ��Ă��܂��B
        /// �܂��AOnStart���\�b�h�Ăяo����ɁA�߂�l��index�Ƃ���UserPathCollection�ɃA�N�Z�X���邱�ƂŁA�t���p�X���擾�ł��܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected int AddUserPath(string path) {
            this.m_UserPathCollection.Add(path);
            return this.m_UserPathCollection.Count - 1;
        }

        #region �p�X�̐ݒ�
        /// <summary>
        /// �p�X�̏��������s���܂��B���̃��\�b�h��OnStart���\�b�h����Ăяo����܂��B
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

        #region �f�B���N�g���\���̃`�F�b�N�ƍ\�z
        /// <summary>
        /// �ݒ肳�ꂽ�p�X�ɏ]���A�f�B���N�g���\�������������܂��B���̃��\�b�h��OnStart���\�b�h����Ăяo����܂��B
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
        /// �w�肵���p�X���������A���݂��Ȃ��f�B���N�g���ł���΁A�쐬���܂��B
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
                //�ݒ�̓ǂݍ���
                this.m_AppConfigs.Load(this.m_ConfigDirectory);
                //Config��K�p
                this.ApplyConfig();
                this.m_IsConfigSeted = true;
                //Config�͓K�p������͎g��Ȃ��̂ŃN���A�B
                this.m_AppConfigs.ClearPackage();
            }
		}
		#endregion 

		#region LoadExDLL
		private void LoadExDLL() {
			//�ǉ�DLL�̓ǂݍ���
			//AppDomain���̒ǉ�DLL
			this.AppManager.LoadAssemblies();
			//AppDomain�ɓǂݍ��܂�Ă��Ȃ��ǉ�DLL
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
        /// �A�v���P�[�V�������N�����܂��B
        /// Config�Ɉˑ����鑀��́A���̃��\�b�h���Ăяo���ꂽ�u�Ԃ���L���ɂȂ�܂��B
        /// ���̃^�C�~���O�́A���̃��\�b�h���I�[�o�[���C�h�����ꍇ�A�I�[�o�[���C�h�������\�b�h���Ăяo���ꂽ�u�ԂɂȂ�܂��B
        /// </summary>
        protected virtual void Run() {
            Application.Run(this.m_MainWindow);

        }
        #endregion

        #region Config�̓K�p
        /// <summary>
        /// Config��K�p���܂��B
        /// �K�p�Ƃ͕ۑ����ꂽConfig��ݒ肷�邱�Ƃł��B
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

        #region Config�̍X�V
        /// <summary>
        /// Config���X�V���܂��B
        /// �X�V�Ƃ͕ύX���ꂽConfig��ۑ����邱�Ƃł��B
        /// </summary>
        protected void ReflectConfig() {
            foreach(xcfg.IUseConfig useconfig in this.m_UseConfigObjects) {
                useconfig.ReflectConfig(this.m_AppConfigs);
            }
        }
        #endregion

        #region OnErrorAtOnStart(Exception ex)
        /// <summary>
        /// OnStart���\�b�h�Ŕ��������G���[���������郁�\�b�h�ł�
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
        /// Start���\�b�h�Ŕ��������G���[���������郁�\�b�h�ł�
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
                    //�ȑO�̃t�@�C�������݂���ꍇ�A�ȑO�̃t�@�C���̓��e�������o��
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
        /// �G���[���O�ɗ�O���������o���܂��B
        /// </summary>
        /// <param name="ex">�X���[���ꂽ��O</param>
        /// <param name="writer">�G���[���O�ɏ����o��TextWriter</param>
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
        /// �I������
        /// </summary>
        protected virtual void OnEnd() {
            this.ReflectConfig();
            this.m_AppConfigs.Save(this.m_ConfigDirectory);
        }
        #endregion

        #region ThreadException�C�x���g�n���h��
        void Application_ThreadException(object sender,System.Threading.ThreadExceptionEventArgs e) {
            this.OnError(e.Exception);
            this.OnEnd();
            MessageBox.Show("�A�v���P�[�V�����Œv���I�ȃG���[�������������߁A�I�����܂��B","�A�v���P�[�V�����G���[");
            Application.Exit();
        }
        #endregion

    }

	/// <summary>
	/// ���̑������t�����ꂽ�A�Z���u���́AAppMainBase.AppManager�ɂ���āA�ǉ��œǂݍ��܂�܂��B
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class ExDLLAttribute : Attribute {
	}

}