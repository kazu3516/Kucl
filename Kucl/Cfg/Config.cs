using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

//�݊����̂��߂Ɏc����Ă��܂��B
//Ver1.0.7.0�ȍ~�ł�XmlConfig���g�p���Ă��������B

namespace Kucl.Cfg {

    #region ConfigPath
    /// <summary>
    /// Config�̃p�X��\���N���X�ł��B
    /// Config�̃p�X�̓p�b�P�[�W���ƃp�X���ɕ�������A
    /// ���ꂼ��K�w�������܂��B
    /// (��)package1.package2.package3:path1.path2.path3�c
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class ConfigPath {
        private string[] m_PackageName;
        private string[] m_Path;
        /// <summary>
        /// ConfigPath�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="package"></param>
        /// <param name="items"></param>
        public ConfigPath(string[] package,string[] items) {
            this.m_PackageName = package;
            this.m_Path = items;
        }
        /// <summary>
        /// �p�b�P�[�W�����擾���܂��B
        /// </summary>
        public string[] PackageName {
            get {
                return m_PackageName;
            }
        }
        /// <summary>
        /// �p�X�����擾���܂��B
        /// </summary>
        public string[] Path {
            get {
                return m_Path;
            }
        }
    }

    #endregion

    #region ConfigPathProvider
    /// <summary>
    /// �����񂩂�ConfigPath�𐶐�����N���X�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public static class ConfigPathProvider {
        /// <summary>
        /// �K�w����؂镶����'.'��\���܂��B
        /// </summary>
        public const char DelimiterChar = '.';
        /// <summary>
        /// Config��Package����؂镶����':'��\���܂��B
        /// </summary>
        public const char PackageDelimiterChar = ':';
        /// <summary>
        /// �p�X�`���̕�����𐶐����܂��B
        /// ������z���DelimiterChar�ŘA������܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathString(string[] path) {
            return string.Join(ConfigPathProvider.DelimiterChar.ToString(),path);
        }
        /// <summary>
        /// ���S�p�X����ConfigPath�I�u�W�F�N�g�𐶐����܂��B
        /// </summary>
        /// <param name="completePath"></param>
        /// <returns></returns>
        public static ConfigPath GetConfigPath(string completePath) {
            string[] path = completePath.Split(new char[] { ConfigPathProvider.PackageDelimiterChar },2);
            string[] path1 = path[0].Split(ConfigPathProvider.DelimiterChar);
            string[] path2 = path[1].Split(ConfigPathProvider.DelimiterChar);
            return new ConfigPath(path1,path2);
        }

        /// <summary>
        /// Config���琶�������s���S�p�X����ConfigPath�𐶐��B
        /// Packagename�͍l�����Ȃ��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ConfigPath GetConfigPathWithoutPackageName(string path) {
            string[] path1 = path.Split(ConfigPathProvider.DelimiterChar);
            return new ConfigPath(new string[]{},path1);
        }
    }
    #endregion

    #region Configs
    /// <summary>
    /// ConfigPackage���Ǘ�����ŏ�ʂ�Config�I�u�W�F�N�g��\���N���X�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class Configs {

        #region �����o�ϐ�
        private Dictionary<string,ConfigPackage> m_Packages;

        private bool m_DoClearFileOnSave;


        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �t�@�C���ۑ��O�ɁA�f�B���N�g�����̃t�@�C����S�č폜���邩�ǂ������擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public bool DoClearFileOnSave {
            get {
                return this.m_DoClearFileOnSave;
            }
            set {
                this.m_DoClearFileOnSave = value;
            }
        }
        /// <summary>
        /// ���O���w�肵��ConfigPackage�̎擾�܂��͐ݒ���s���C���f�N�T�ł��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigPackage this[string name] {
            get {
                return this.m_Packages[name];
            }
            set {
                this.m_Packages[name] = value;
            }
        }
        /// <summary>
        /// ���O��ConfigPackage���֘A�t�����n�b�V���e�[�u�����擾���܂��B
        /// </summary>
        public Dictionary<string,ConfigPackage> Packages {
            get {
                return m_Packages;
            }
        }

        #endregion

        #region �R���X�g���N�^
        /// <summary>
        /// Configs�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        public Configs(){
            this.m_Packages = new Dictionary<string,ConfigPackage>();
        }
        #endregion

        #region AddPackage
        /// <summary>
        /// ConfigPackage��ǉ����܂��B
        /// </summary>
        /// <param name="package"></param>
        public void AddPackage(ConfigPackage package) {
            if(this.m_Packages.ContainsKey(package.Name)) {
                this.m_Packages[package.Name] = package;
            }
            else {
                this.m_Packages.Add(package.Name,package);
            }
        }
        #endregion

        #region RemovePackage
        /// <summary>
        /// �w�肵�����O������ConfigaPackage���폜���܂��B
        /// </summary>
        /// <param name="name"></param>
        public void RemovePackage(string name) {
            this.m_Packages.Remove(name);
        }
        #endregion

        #region ClearPackage
        /// <summary>
        /// ConfigPackage��S�č폜���܂��B
        /// </summary>
        public void ClearPackage() {
            this.m_Packages.Clear();
        }
        #endregion

        #region GetPackage
        /// <summary>
        /// �w�肵�����O������ConfigPackage��Ԃ��܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigPackage GetPackage(string name) {
            return this.m_Packages[name];
        }
        #endregion

        #region GetConfig
        /// <summary>
        /// �w�肵�����O������ConfigItem��Ԃ��܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigItem GetConfig(string name) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            return this.GetConfig(path);
        }
        /// <summary>
        /// �w�肵��ConfigPath������ConfigItem��Ԃ��܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ConfigItem GetConfig(ConfigPath path) {
            string package = ConfigPathProvider.GetPathString(path.PackageName);
            if(this.m_Packages.ContainsKey(package)) {
                if(this.m_Packages[package].Contains(path.Path[0])){
                    return this.m_Packages[package][path.Path[0]].GetConfig(path);
                }
            }
            throw new ArgumentException("�w�肳�ꂽ���ڂ͑��݂��܂���B");
        }
        #endregion

        #region AddConfig
        /// <summary>
        /// ���O���w�肵�āA�ݒ��bool�l��ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConfig(string name,bool value) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,new BoolConfigItem(this.GetItemName(path),value));
        }
        /// <summary>
        /// ���O���w�肵�āA�ݒ�ɕ������ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConfig(string name,string value) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,new StringConfigItem(this.GetItemName(path),value));
        }
        /// <summary>
        /// ���O���w�肵�āA�ݒ�ɐ����l��ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConfig(string name,int value) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,new IntConfigItem(this.GetItemName(path),value));
        }
        /// <summary>
        /// ���O���w�肵�āA�ݒ�Ɏ����l��ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConfig(string name,double value) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,new DoubleConfigItem(this.GetItemName(path),value));
        }
        /// <summary>
        /// ���O���w�肵�Đݒ��ConfigItem��ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void AddConfig(string name,ConfigItem item) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,item);
        }
        /// <summary>
        /// ConfigPath���w�肵�āA�ݒ��ConfigItem��ǉ����܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        public void AddConfig(ConfigPath path,ConfigItem item) {
            ConfigPackage package;
            string packageName = ConfigPathProvider.GetPathString(path.PackageName);
            if(this.m_Packages.ContainsKey(packageName)) {
                package = this.m_Packages[packageName];
            }
            else {
                package = new ConfigPackage(packageName);
                this.AddPackage(package);
            }
            Config config;
            if(package.Contains(path.Path[0])) {
                config = package[path.Path[0]];
            }
            else {
                config = new Config(path.Path[0]);
                package.AddConfig(config);
            }
            config.AddConfig(path,item);
        }
        private string GetItemName(ConfigPath path) {
            return path.Path[path.Path.Length - 1];
        }
        #endregion

        #region ContainsConfig
        /// <summary>
        /// �w�肵�����O������Config�����݂��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsConfig(string name) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            return this.ContainsConfig(path);
        }
        /// <summary>
        /// �w�肵��ConfigPath������Config�����݂��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ContainsConfig(ConfigPath path) {
            string packageName = ConfigPathProvider.GetPathString(path.PackageName);
            return this.m_Packages.ContainsKey(packageName) &&
                this.m_Packages[packageName].Contains(path.Path[0]) &&
                this.m_Packages[packageName][path.Path[0]].Contains(path);
        }
        #endregion

        #region �t�@�C�����o��
        /// <summary>
        /// �w�肵���f�B���N�g������Config�����[�h���܂��B
        /// </summary>
        /// <param name="dirName"></param>
        public void Load(string dirName) {
            this.OnLoad(dirName,"");
        }
		/// <summary>
		/// �w�肵���f�B���N�g������Config�����[�h���܂��B
		/// </summary>
		/// <param name="dirName">Config�����[�h����f�B���N�g��</param>
		/// <param name="packageName">�w�肵���f�B���N�g�����܂܂��p�b�P�[�W�B���[�g�p�b�P�[�W�̏ꍇ�͋󕶎���ł��B</param>
        private void OnLoad(string dirName,string packageName) {
            if(!Directory.Exists(dirName)) {
                throw new DirectoryNotFoundException(string.Format("�w�肵���f�B���N�g���͑��݂��܂���B\r\n{0}\r\n",dirName));
            }
            foreach(string filename in Directory.GetFiles(dirName,"*.xml")) {
                string pName = packageName != "" ? packageName + "." : "";
                pName += Path.GetFileNameWithoutExtension(filename);
                ConfigPackage package = ConfigPackage.Load(Path.Combine(dirName,filename),pName);
                this.AddPackage(package);
            }
            foreach(string dir in Directory.GetDirectories(dirName)) {
                string pName = packageName != "" ? "." : "";
                pName += Path.GetFileNameWithoutExtension(dir);
                this.OnLoad(dir,pName);
            }
        }
        /// <summary>
        /// �f�B���N�g�������w�肵�āACondfig�����ׂĕۑ����܂��B
        /// </summary>
        /// <param name="dirName"></param>
        public void Save(string dirName) {
			//���ׂč폜���Ă���ۑ�(�s�v�t�@�C�����c�邽��)
            if(this.m_DoClearFileOnSave) {
                foreach(string filename in Directory.GetFiles(dirName)) {
                    File.Delete(filename);
                }
                foreach(string subdir in Directory.GetDirectories(dirName)) {
                    Directory.Delete(subdir,true);
                }
            }
            foreach(ConfigPackage package in this.m_Packages.Values) {
                List<string> packageName = new List<string>(package.Name.Split('.'));
                this.OnSave(dirName,packageName,package);
            }
        }
        private void OnSave(string dirName,List<string> packageName,ConfigPackage package) {
			//"Setting.Application.Kucl:Application.FastSetup"�̏ꍇ�ASetting\Application\Kucl.xml�������
            if(!Directory.Exists(dirName)) {
                Directory.CreateDirectory(dirName);
            }
            if(packageName.Count == 1) {
                ConfigPackage.Save(Path.Combine(dirName,packageName[0]) + ".xml",package);
            }
            else {
                string subdir = packageName[0];
                packageName.RemoveAt(0);
                this.OnSave(Path.Combine(dirName,subdir),packageName,package);
            }
            
        }
        #endregion

    }
    #endregion

    #region ConfigPackage
    /// <summary>
    /// Config�p�b�P�[�W��\���N���X�ł��B
    /// 1��Config�p�b�P�[�W��1�̃t�@�C���ɑΉ����܂��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class ConfigPackage{

		#region �����o�ϐ�
		private Dictionary<string,Config> m_Configs;
		private string m_Name;

		#endregion

		#region �R���X�g���N�^
        /// <summary>
        /// ConfigPackage�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
		public ConfigPackage(){
			this.m_Name = "";
			this.m_Configs = new Dictionary<string,Config>();
		}
        /// <summary>
        /// ConfigPackage�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
		public ConfigPackage(string name){
			this.m_Name = name;
            this.m_Configs = new Dictionary<string,Config>();
		}
		#endregion
		
		#region �v���p�e�B
        /// <summary>
        /// �w�肵�����O������Config���擾����C���f�N�T�ł��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public Config this[string name]{
			get{
				return this.m_Configs[name];
			}
		}
        /// <summary>
        /// ���O��Config���֘A�t�����n�b�V���e�[�u�����擾���܂��B
        /// </summary>
        public Dictionary<string,Config> Configs {
			get{
				return this.m_Configs;
			}
		}
        /// <summary>
        /// �p�b�P�[�W�����擾�܂��͐ݒ肵�܂��B
        /// </summary>
		public string Name{
			get{
				return this.m_Name;
			}
			set{
				this.m_Name = value;
			}
		}
		#endregion

		#region Config�̎擾�A�ǉ��A�폜
        /// <summary>
        /// �w�肵�����O��Config�����݂��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name) {
            return this.m_Configs.ContainsKey(name);
        }
        /// <summary>
        /// Config��ǉ����܂��B
        /// </summary>
        /// <param name="value"></param>
		public void AddConfig(Config value){
			this.m_Configs.Add(value.Name,value);
		}
        /// <summary>
        /// ���O���w�肵��Config���擾���܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public Config GetConfig(string name){
			return this.m_Configs[name];
		}
        /// <summary>
        /// ���O���w�肵��Config���폜���܂��B
        /// </summary>
        /// <param name="name"></param>
		public void Remove(string name){
			this.m_Configs.Remove(name);
		}
		#endregion

		#region �t�@�C�����o��
        /// <summary>
        /// �t�@�C�������w�肵��Config�p�b�P�[�W���t�@�C���ɕۑ����܂��B
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="package"></param>
		public static void Save(string filename,ConfigPackage package){
			XmlTextWriter writer = new XmlTextWriter(filename,System.Text.Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 4;
			try{
				writer.WriteStartDocument();
				writer.WriteStartElement("configs");
				writer.WriteAttributeString("Count",package.m_Configs.Count.ToString());
				foreach(Config config in package.m_Configs.Values){
					Config.SaveFile(writer,config);
				}
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
			finally{
				if(writer != null){
					writer.Close();
				}
			}
		}
        /// <summary>
        /// �t�@�C�����ƃp�b�P�[�W�����w�肵��Config�p�b�P�[�W���t�@�C������ǂݍ��݂܂��B
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
		public static ConfigPackage Load(string filename,string packageName){
			ConfigPackage package = new ConfigPackage(packageName);
			XmlTextReader reader = new XmlTextReader(filename);
			reader.WhitespaceHandling = WhitespaceHandling.Significant;
            try {
                while(reader.Read()) {
                    if(reader.IsStartElement("configs")) {
                        int count = int.Parse(reader.GetAttribute("Count"));
                        for(int i = 0;i < count;i++) {
                            Config config = Config.LoadFile(reader);
                            package.AddConfig(config);
                        }
                        break;
                    }
                }
            }
            catch(XmlException ex) {
                System.Diagnostics.Debug.WriteLine(ex.GetType().FullName + "\r\n" + ex.Message);
                throw;
            }
			finally{
				if(reader != null){
					reader.Close();
				}
			}
			return package;
		}
		#endregion
	}
    #endregion

    #region Config
    /// <summary>
    /// �ݒ��\���N���X�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class Config{
		
		#region �����o�ϐ�
		private string m_Name;
		private ContainerConfigItem m_Root;
		#endregion

		#region �R���X�g���N�^
        /// <summary>
        /// Config�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
		public Config(string name){
			this.m_Name = name;
			this.m_Root = new ContainerConfigItem(name);
		}
        private Config(){
            //�p�����[�^�����̃R���X�g���N�^�́AConfig.LoadFile�ŕK�v
        }
		#endregion

		#region �v���p�e�B
        /// <summary>
        /// Config�I�u�W�F�N�g�̖��O���擾���܂��B
        /// </summary>
		public string Name{
			get{
				return this.m_Name;
			}
		}
		#endregion

		#region CloneList
		private List<String> CloneList(List<String> list){
			List<String> list2 = new List<String>();
			foreach(string item in list){
				list2.Add(item);
			}
			return list2;
		}
		#endregion

		#region Contains
        /// <summary>
        /// �w�肵�����S�p�X�����ݒ荀�ڂ����݂��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="CompletePath"></param>
        /// <returns></returns>
		public bool Contains(string CompletePath){
            ConfigPath path = ConfigPathProvider.GetConfigPath(CompletePath);
            return this.Contains(path);
        }
        /// <summary>
        /// �w�肵���pConfigPath�����ݒ荀�ڂ����݂��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Contains(ConfigPath path){
            List<String> list = new List<string>(path.Path);
            list.RemoveAt(0);
			return this.Contains(path.Path[0],list);
		}
		private bool Contains(string root,List<String> path){
			if(root != "" && this.m_Name != root){
				return false;
			}
			return this.Contains(this.m_Root,path);
		}

		private bool Contains(ContainerConfigItem parent,List<String> path){
            //List�Ɋi�[���ꂽ�p�X�����Ƀ`�F�b�N
			if(parent.Items.ContainsKey(path[0])){
				ConfigItem item = parent.Items[path[0]];
				if(path.Count == 1){
                    //�Ō�܂Ń}�b�`������true
					return true;
				}
				else if(item.Type == ConfigItemType.Container){
                    //�}�b�`���邽�тɍ폜���ĉ��̊K�w�֍~���
					path.RemoveAt(0);
					return this.Contains((ContainerConfigItem)item,path);
				}
			}
			return false;
		}
		#endregion

		#region GetConfig
        /// <summary>
        /// �p�X���w�肵��ConfigItem���擾����C���f�N�T�ł��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
		public ConfigItem this[string path]{
			get{
				return this.GetConfig(path);
			}
		}
        /// <summary>
        /// ���S�p�X���w�肵��ConfigItem���擾���܂��B
        /// </summary>
        /// <param name="completePath"></param>
        /// <returns></returns>
        public ConfigItem GetConfig(string completePath) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(completePath);
            return this.GetConfig(path);
        }
        /// <summary>
        /// ConfigPath���w�肵��ConfigIte���擾���܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ConfigItem GetConfig(ConfigPath path) {
            List<String> list = new List<string>(path.Path);
            list.RemoveAt(0);
			ConfigItem item = this.GetConfig(path.Path[0],list);

            #region DEBUG
#if DEBUG
			if(item == null){
				Debug.WriteLine("Null Config");
				Debug.WriteLine(path);
			}
#endif
            #endregion

            return item;
		}

		private ConfigItem GetConfig(string root,List<String> path){
			if(root != "" && this.m_Name != root){
				return null;
			}
			return this.GetConfig(this.m_Root,path);
		}

		private ConfigItem GetConfig(ContainerConfigItem parent,List<String> path){
			ConfigItem item = parent.Items[path[0]];
			if(path.Count == 1){
				return item;
			}
			else if(item.Type == ConfigItemType.Container){
				path.RemoveAt(0);
				return this.GetConfig((ContainerConfigItem)item,path);
			}
			return null;
		}
		#endregion

		#region AddConfig
		private string GetItemName(string completePath){
            ConfigPath path = ConfigPathProvider.GetConfigPathWithoutPackageName(completePath);
            return this.GetItemName(path);
        }
        private string GetItemName(ConfigPath path){
			return path.Path[path.Path.Length - 1];
		}

		/// <summary>
		/// ���O���w�肵�āA�ݒ��bool�l��ǉ����܂��B
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void AddConfig(string name,bool value){
			this.AddConfig(name,new BoolConfigItem(this.GetItemName(name),value));
		}
        /// <summary>
        /// ���O���w�肵�āA�ݒ�ɕ������ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void AddConfig(string name,string value){
			this.AddConfig(name,new StringConfigItem(this.GetItemName(name),value));
		}
        /// <summary>
        /// ���O���w�肵�āA�ݒ�ɐ����l��ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void AddConfig(string name,int value){
			this.AddConfig(name,new IntConfigItem(this.GetItemName(name),value));
		}
        /// <summary>
        /// ���O���w�肵�āA�ݒ�Ɏ����l��ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void AddConfig(string name,double value){
			this.AddConfig(name,new DoubleConfigItem(this.GetItemName(name),value));
		}
        /// <summary>
        /// ���O���w�肵�Đݒ��ConfigItem��ǉ����܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
		public void AddConfig(string name,ConfigItem item){
            ConfigPath path = ConfigPathProvider.GetConfigPathWithoutPackageName(name);
            this.AddConfig(path,item);
        }
        /// <summary>
        /// ConfigPath���w�肵�āA�ݒ��ConfigItem��ǉ����܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        public void AddConfig(ConfigPath path,ConfigItem item){
            List<string> list = new List<string>(path.Path);
            list.RemoveAt(0);
			this.AddConfig(path.Path[0],list,item);
		}

		private void AddConfig(string root,List<String> path,ConfigItem item){
			if(root != "" && this.m_Name != root){
				throw new ArgumentException("�Ԉ�����p�X���[�g���w�肳��܂����B");
			}
			this.AddConfig(this.m_Root,path,item);
		}

		private void AddConfig(ContainerConfigItem parent,List<String> path,ConfigItem item){
			if(path.Count == 1){
				//parent�͒ǉ����ׂ��A�C�e���̒��̐e
				if(!parent.Items.ContainsKey(path[0])){
					parent.Items.Add(path[0],item);
				}
				else{
					parent.Items[path[0]] = item;
				}
				return;
			}
			else{
				ContainerConfigItem nextParent;
				if(!parent.Items.ContainsKey(path[0])){
					nextParent = new ContainerConfigItem(path[0]);
					parent.Items.Add(path[0],nextParent);
				}
				else{
					nextParent = (ContainerConfigItem)parent.Items[path[0]];
				}
				path.RemoveAt(0);
				this.AddConfig(nextParent,path,item);
			}
		}
		#endregion

		#region RemoveConfig
		/// <summary>
		/// ���S�p�X���w�肵��ConfigItem���폜���܂��B
		/// </summary>
		/// <param name="completePath"></param>
		public void RemoveConfig(string completePath){
            ConfigPath path = ConfigPathProvider.GetConfigPath(completePath);
            List<String> list = new List<string>(path.Path);
            list.RemoveAt(0);
			this.RemoveConfig(path.Path[0],list);
		}

		private void RemoveConfig(string root,List<String> path){
			ConfigItem item = this.GetConfig(root,this.CloneList(path));
			if(item != null){
				string target = path[path.Count - 1];
				path.RemoveAt(path.Count - 1);
				item = this.GetConfig(root,this.CloneList(path));
				if(item.Type != ConfigItemType.Container){
					throw new ArgumentException("�s���ȃp�X�ł��B");
				}
				ContainerConfigItem parent = (ContainerConfigItem)item;
				parent.Items.Remove(target);
				if(parent.Items.Count == 0){
					this.RemoveConfig(root,path);
				}
			}
		}
		#endregion

		#region ClearConfig
        /// <summary>
        /// �ݒ�l��S�č폜���܂��B
        /// </summary>
		public void ClearConfig(){
			this.m_Root.Items.Clear();
		}
		#endregion

		#region �t�@�C�����o��

		internal static void SaveFile(XmlTextWriter writer,Config config){
			writer.WriteStartElement("config");
			writer.WriteAttributeString("Name",config.Name);
			writer.WriteAttributeString("Count",config.m_Root.Items.Count.ToString());
			foreach(ConfigItem item in config.m_Root.Items.Values){
				ConfigItem.Save(writer,item);
			}
			writer.WriteEndElement();
		}
		internal static Config LoadFile(XmlTextReader reader){
			Config config = new Config();
			while(reader.Read()){
				if(reader.IsStartElement("config")){
					string name = reader.GetAttribute("Name");
					config.m_Name = name;
					config.m_Root = new ContainerConfigItem(name);
					int count = int.Parse(reader.GetAttribute("Count"));
					reader.Read();
					for(int i = 0;i < count;i++){
						ConfigItem item = ConfigItem.Load(reader);
						config.AddConfig(name + '.' + item.Name,item);
					}
					break;
				}
			}
			return config;
		}
		#endregion

	}
    #endregion

    #region ConfigItem
    /// <summary>
    /// �S�Ă̐ݒ荀�ڂ�\��ConfigItem�̊�{�N���X�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public abstract class ConfigItem{
		private ConfigItemType m_Type;
		private string m_Name;
        /// <summary>
        /// �^���Ɩ��O���w�肵��ConfigItem�I�u�W�F�N�g�����������܂��B
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
		protected ConfigItem(ConfigItemType type,string name){
			this.m_Type = type;
			this.m_Name = name;
		}
        /// <summary>
        /// �ݒ荀�ږ����擾���܂��B
        /// </summary>
		public string Name{
			get{
				return this.m_Name;
			}
		}
        /// <summary>
        /// Config�l�̌^�����擾���܂��B
        /// </summary>
		public ConfigItemType Type{
			get{
				return this.m_Type;
			}
		}
        /// <summary>
        /// Config�l���擾�A�ݒ肵�܂��B
        /// </summary>
		public abstract object Value{get;set;}
        /// <summary>
        /// Config�l��XML�ɏ����o���܂��B
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="item"></param>
		public static void Save(XmlTextWriter writer,ConfigItem item){
			writer.WriteStartElement("ConfigItem");
			writer.WriteAttributeString("Name",item.m_Name);
			writer.WriteAttributeString("Type",item.m_Type.ToString());
			item.OnSave(writer);
			writer.WriteEndElement();
		}
        /// <summary>
        /// Condfig�l��XML����ǂݏo���܂��B
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
		public static ConfigItem Load(XmlTextReader reader){
			try{
				if(reader.IsStartElement("ConfigItem")){
					string name = reader.GetAttribute("Name");
					ConfigItemType type = (ConfigItemType)Enum.Parse(typeof(ConfigItemType),reader.GetAttribute("Type"));
					reader.Read();
					ConfigItem item = null;
					switch(type){
						case ConfigItemType.Container:
							item = new ContainerConfigItem(name);
							break;
						case ConfigItemType.Bool:
							item = new BoolConfigItem(name);
							break;
						case ConfigItemType.String:
							item = new StringConfigItem(name);
							break;
						case ConfigItemType.Int:
							item = new IntConfigItem(name);
							break;
						case ConfigItemType.Double:
							item = new DoubleConfigItem(name);
							break;
					}
					if(item != null){
						item.OnLoad(reader);
					}
					reader.Read();
					return item;
				}
				return null;
			}
			catch(XmlException){
				throw;
			}
			catch(Exception){// ex){
                //AppMain.g_AppMain.DebugWrite(ex);
				throw;
			}
		}
        /// <summary>
        /// �h���N���X�ŃI�[�o�[���C�h�����ƁAConfig�l��XML�ɏ����o�����������s���܂��B
        /// </summary>
        /// <param name="writer"></param>
		protected abstract void OnSave(XmlTextWriter writer);
        /// <summary>
        /// �h���N���X�ŃI�[�o�[���C�h�����ƁAConfig�l��XML����ǂݏo�����������s���܂��B
        /// </summary>
        /// <param name="reader"></param>
		protected abstract void OnLoad(XmlTextReader reader);
        /// <summary>
        /// Value�v���p�e�B��ToString()���\�b�h�ŕԂ���錋�ʂ�XML�ɏ������݂܂��B
        /// Value�v���p�e�B��ToString()���\�b�h���A�K�؂�Config�l��Ԃ��ꍇ�A
        /// OnSave���\�b�h�ł�WriteDefaultValue���\�b�h�݂̂��Ăяo���܂��B
        /// </summary>
        /// <param name="writer"></param>
		protected void WriteDefaultValue(XmlTextWriter writer){
			writer.WriteString(this.Value.ToString());
		}
	}
    #endregion

    #region ConfigItemType
    /// <summary>
    /// Config�l�̌^���������܂��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public enum ConfigItemType{
        /// <summary>
        /// ������Config������Ɋi�[����A�C�e���ł��B
        /// </summary>
		Container,
        /// <summary>
        /// �^�U�l��ێ�����A�C�e���ł��B
        /// </summary>
		Bool,
        /// <summary>
        /// �������ێ�����A�C�e���ł��B
        /// </summary>
		String,
        /// <summary>
        /// �����l��ێ�����A�C�e���ł��B
        /// </summary>
		Int,
        /// <summary>
        /// �����l��ێ�����A�C�e���ł��B
        /// </summary>
		Double
	}
    #endregion

    #region ConfigItem�̔h���N���X(��{�^)

    #region ContainerConfigItem
    /// <summary>
    /// ����ConfigItem���܂���ConfigItem�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class ContainerConfigItem:ConfigItem{
		
        #region �R���X�g���N�^
        /// <summary>
        /// ContainerConfigItem�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        public ContainerConfigItem(string name)
            : base(ConfigItemType.Container, name) {
            this.m_Children = new Dictionary<string, ConfigItem>();
        } 
        #endregion

        #region �v���p�e�B
        private Dictionary<string, ConfigItem> m_Children;
        /// <summary>
        /// �ێ�����ConfigItem�̃n�b�V���e�[�u�����擾���܂��B
        /// </summary>
        public Dictionary<string, ConfigItem> Items {
            get {
                return this.m_Children;
            }
        }
        /// <summary>
        /// �ێ�����ConfigItem�̌����擾���܂��B
        /// �ݒ菈���͖����ł��B
        /// </summary>
        public override object Value {
            get {
                return this.m_Children.Count;
            }
            set {
            }
        } 
        #endregion

        #region �V���A����
        /// <summary>
        /// ContainerConfigItem�I�u�W�F�N�g��XML�ɏ������݂܂��B
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            writer.WriteElementString("Count", this.m_Children.Count.ToString());
            foreach (ConfigItem item in this.m_Children.Values) {
                ConfigItem.Save(writer, item);
            }
        }
        /// <summary>
        /// ContainerConfigItem�I�u�W�F�N�g��XML����ǂݏo���܂��B
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            try {
                reader.ReadStartElement("Count");
                int count = int.Parse(reader.ReadString());
                reader.Read();
                for (int i = 0; i < count; i++) {
                    ConfigItem item = ConfigItem.Load(reader);
                    this.m_Children.Add(item.Name, item);
                }
            }
            catch (XmlException) {// ex){
                //AppMain.g_AppMain.DebugWrite(ex);
                throw;
            }
        } 
        #endregion
	
    }
    #endregion

    #region BoolConfigItem
    /// <summary>
    /// Bool�l������ConfigItem�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class BoolConfigItem:ConfigItem{
        
        #region �R���X�g���N�^
        /// <summary>
        /// BoolConfigItem�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        public BoolConfigItem(string name) : this(name, true) {
        }
        /// <summary>
        /// BoolConfigItem�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public BoolConfigItem(string name, bool value)
            : base(ConfigItemType.Bool, name) {
            this.m_Value = value;
        } 
        #endregion
        
        #region Value
        private bool m_Value;
        /// <summary>
        /// BoolConfigItem�I�u�W�F�N�g�̒l���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public override object Value {
            get {
                return this.m_Value;
            }
            set {
                this.m_Value = (bool)value;
            }
        } 
        #endregion

        #region �V���A����
        /// <summary>
        /// BoolConfigItem�I�u�W�F�N�g��XML�ɏ����o���܂��B
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// BoolConfigItem�I�u�W�F�N�g��XML����ǂݏo���܂��B
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            this.m_Value = bool.Parse(reader.ReadString());
        } 
        #endregion

	}
    #endregion

    #region StringConfigItem
    /// <summary>
    /// �����������ConfigItem�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class StringConfigItem:ConfigItem{
        
        #region �R���X�g���N�^
        /// <summary>
        /// StringConfigItem�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        public StringConfigItem(string name) : this(name, "") {
        }
        /// <summary>
        /// StringConfigItem�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public StringConfigItem(string name, string value)
            : base(ConfigItemType.String, name) {
            this.m_Value = value;
        } 
        #endregion

        #region Value
        private string m_Value;
        /// <summary>
        /// StringConfigItem�I�u�W�F�N�g�̒l���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public override object Value {
            get {
                return this.m_Value;
            }
            set {
                this.m_Value = (string)value;
            }
        } 
        #endregion

        #region �V���A����
        /// <summary>
        /// StringConfigItem�I�u�W�F�N�g��XML�ɏ����o���܂��B
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// StringConfigItem�I�u�W�F�N�g��XML����ǂݏo���܂��B
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            this.m_Value = reader.ReadString();
        } 
        #endregion

	}
    #endregion

    #region IntConfigItem
    /// <summary>
    /// �����l������ConfigItem�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class IntConfigItem:ConfigItem{
        
        #region �R���X�g���N�^
        /// <summary>
        /// IntConfigItem�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        public IntConfigItem(string name) : this(name, 0) {
        }
        /// <summary>
        /// IntConfigItem�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public IntConfigItem(string name, int value)
            : base(ConfigItemType.Int, name) {
            this.m_Value = value;
        } 
        #endregion
        
        #region Value
        private int m_Value;
        /// <summary>
        /// IntConfigItem�I�u�W�F�N�g�̒l���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public override object Value {
            get {
                return this.m_Value;
            }
            set {
                this.m_Value = (int)value;
            }
        } 
        #endregion

        #region �V���A����
        /// <summary>
        /// IntConfigItem�I�u�W�F�N�g��XML�ɏ����o���܂��B
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// IntConfigItem�I�u�W�F�N�g��XML����ǂݏo���܂��B
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            this.m_Value = int.Parse(reader.ReadString());
        } 
        #endregion

	}
    #endregion

    #region DoubleConfigItem
    /// <summary>
    /// �����l������ConfigItem�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class DoubleConfigItem:ConfigItem{
        
        #region �R���X�g���N�^
        /// <summary>
        /// DoubleConfigItem�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        public DoubleConfigItem(string name) : this(name, 0.0) {
        }
        /// <summary>
        /// DoubleConfigItem�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public DoubleConfigItem(string name, double value)
            : base(ConfigItemType.Double, name) {
            this.m_Value = value;
        } 
        #endregion

        #region Value
        private double m_Value;
        /// <summary>
        /// DoubleConfigItem�I�u�W�F�N�g�̒l���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public override object Value {
            get {
                return this.m_Value;
            }
            set {
                this.m_Value = (double)value;
            }
        } 
        #endregion

        #region �V���A����
        /// <summary>
        /// DoubleConfigItem�I�u�W�F�N�g��XML�ɏ����o���܂��B
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// DoubleConfigItem�I�u�W�F�N�g��XML����ǂݏo���܂��B
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            this.m_Value = double.Parse(reader.ReadString());
        } 
        #endregion

	}
    #endregion

    #endregion

    #region IUseConfig
    /// <summary>
    /// Config���g�p����I�u�W�F�N�g�������C���^�[�t�F�[�X�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public interface IUseConfig{
        /// <summary>
        /// ����l������Config���쐬���܂��B
        /// </summary>
        /// <returns></returns>
		Configs CreateDefaultConfig();
		/// <summary>
		/// config��K�p���܂�
		/// </summary>
		/// <param name="config"></param>
		void ApplyConfig(Configs config);
        /// <summary>
        /// config���X�V���܂��B
        /// </summary>
        /// <param name="config"></param>
		void ReflectConfig(Configs config);
        //string PackageName{get;}
        //string ConfigName{get;}
        /// <summary>
        /// �w�肵��Config�l������l���ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		bool IsDefaultValue(string name,ConfigItem value);
	}
    #endregion

    #region UseConfigObjectCollection
    /// <summary>
    /// IUseConfig�C���^�[�t�F�[�X�����������I�u�W�F�N�g�̃R���N�V������\���܂��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class UseConfigObjectCollection : System.Collections.CollectionBase {
        /// <summary>
        /// IUseConfig�I�u�W�F�N�g���擾�A�ݒ肷��C���f�N�T�ł��B
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public IUseConfig this[int index] {
			get {
				return ((IUseConfig)(this.List[index]));
			}
			set {
				this.List[index] = value;
			}
		}
        /// <summary>
        /// IUseConfig�I�u�W�F�N�g���R���N�V�����ɒǉ����܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int Add(IUseConfig value) {
			return this.List.Add(value);
		}
        /// <summary>
        /// IUseConfig�I�u�W�F�N�g���R���N�V��������폜���܂��B
        /// </summary>
        /// <param name="value"></param>
		public void Remove(IUseConfig value) {
			this.List.Remove(value);
		}
        /// <summary>
        /// �ʒu���w�肵�āAIUseConfig�I�u�W�F�N�g���R���N�V�����ɑ}�����܂��B
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		public void Insert(int index, IUseConfig value) {
			this.List.Insert(index, value);
		}
        /// <summary>
        /// IUseConfig�I�u�W�F�N�g���R���N�V�����Ɋ܂܂�Ă��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public bool Contains(IUseConfig value) {
			return this.List.Contains(value);
		}
        /// <summary>
        /// �w�肵��IUseConfig�I�u�W�F�N�g�̃R���N�V�������̈ʒu��Ԃ��܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int IndexOf(IUseConfig value) {
			return this.List.IndexOf(value);
		}
	}
    #endregion

    #region UseConfigHelper
    /// <summary>
    /// IUseConfig����������I�u�W�F�N�g�ɑ΂��Ẵw���p���\�b�h��񋟂���N���X�ł��B
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg���O��Ԃ̃N���X���g�p���Ă��������B")]
    public class UseConfigHelper {

        #region �����o�ϐ�
        private Configs m_DefaultConfig;
		private Configs m_Config;
        #endregion

        #region �R���X�g���N�^
        /// <summary>
        /// UseConfigHelper�I�u�W�F�N�g�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="defaultConfig"></param>
        public UseConfigHelper(Configs defaultConfig){
			this.m_DefaultConfig = defaultConfig;
        }
        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �ΏۂƂȂ�Configs�I�u�W�F�N�g���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public Configs Config{
			get{
				return this.m_Config;
			}
			set{
				this.m_Config = value;
			}
        }
        #endregion

        #region IsDefaultValue
        /// <summary>
        /// ���O���w�肵��ConfigItem���ύX����Ă��Ȃ����ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsDefaultValue(string name,ConfigItem value){
			ConfigItem item = this.m_DefaultConfig.GetConfig(name);
			if(item != null && item.Type != ConfigItemType.Container){
				return item.Value.Equals(value.Value);
			}
			return false;
		}
		#endregion

		#region GetConfigItem
        /// <summary>
        /// �w�肵�����O������ConfigItem��Ԃ��܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public ConfigItem GetConfigItem(string name){
			ConfigItem item;
			if(!this.m_Config.ContainsConfig(name)){
#if DEBUG
				System.Diagnostics.Debug.WriteLine("���݂��Ȃ�Config\r\n" + name);
#endif
				//���݂��Ȃ��ݒ�̏ꍇ�A�f�t�H���g�̐ݒ肩��擾
				item = this.m_DefaultConfig.GetConfig(name);
				if(item == null){
					throw new ArgumentException(string.Format("{0}�͑��݂��܂���",name));
				}
			}
			else{
				item = this.m_Config.GetConfig(name);
			}
			return item;
		}
		#endregion

        #region ����̌^�̒l�̎擾

        #region GetIntFromConfig
        /// <summary>
        /// �w�肵�����O���������l��Ԃ��܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIntFromConfig(string name) {
            return (int)this.GetConfigItem(name).Value;
        }
        #endregion

        #region GetDoubleFromConfig
        /// <summary>
        /// �w�肵�����O���������l���擾���܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double GetDoubleFromConfig(string name) {
            return (double)this.GetConfigItem(name).Value;
        }
        #endregion

        #region GetStringFromConfig
        /// <summary>
        /// �w�肵�����O������������擾���܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetStringFromConfig(string name) {
            return (string)this.GetConfigItem(name).Value;
        }
        #endregion

        #region GetBoolFromConfig
        /// <summary>
        /// �w�肵�����O������bool�l���擾���܂��B
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetBoolFromConfig(string name) {
            return (bool)this.GetConfigItem(name).Value;
        }
        #endregion

        #endregion

    }
	#endregion

}
