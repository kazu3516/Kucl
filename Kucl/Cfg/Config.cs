using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

//互換性のために残されています。
//Ver1.0.7.0以降ではXmlConfigを使用してください。

namespace Kucl.Cfg {

    #region ConfigPath
    /// <summary>
    /// Configのパスを表すクラスです。
    /// Configのパスはパッケージ名とパス名に分割され、
    /// それぞれ階層を持ちます。
    /// (例)package1.package2.package3:path1.path2.path3…
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class ConfigPath {
        private string[] m_PackageName;
        private string[] m_Path;
        /// <summary>
        /// ConfigPathオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="package"></param>
        /// <param name="items"></param>
        public ConfigPath(string[] package,string[] items) {
            this.m_PackageName = package;
            this.m_Path = items;
        }
        /// <summary>
        /// パッケージ名を取得します。
        /// </summary>
        public string[] PackageName {
            get {
                return m_PackageName;
            }
        }
        /// <summary>
        /// パス名を取得します。
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
    /// 文字列からConfigPathを生成するクラスです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public static class ConfigPathProvider {
        /// <summary>
        /// 階層を区切る文字列'.'を表します。
        /// </summary>
        public const char DelimiterChar = '.';
        /// <summary>
        /// ConfigとPackageを区切る文字列':'を表します。
        /// </summary>
        public const char PackageDelimiterChar = ':';
        /// <summary>
        /// パス形式の文字列を生成します。
        /// 文字列配列はDelimiterCharで連結されます。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathString(string[] path) {
            return string.Join(ConfigPathProvider.DelimiterChar.ToString(),path);
        }
        /// <summary>
        /// 完全パスからConfigPathオブジェクトを生成します。
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
        /// Configから生成される不完全パスからConfigPathを生成。
        /// Packagenameは考慮しない。
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
    /// ConfigPackageを管理する最上位のConfigオブジェクトを表すクラスです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class Configs {

        #region メンバ変数
        private Dictionary<string,ConfigPackage> m_Packages;

        private bool m_DoClearFileOnSave;


        #endregion

        #region プロパティ
        /// <summary>
        /// ファイル保存前に、ディレクトリ内のファイルを全て削除するかどうかを取得または設定します。
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
        /// 名前を指定してConfigPackageの取得または設定を行うインデクサです。
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
        /// 名前とConfigPackageを関連付けたハッシュテーブルを取得します。
        /// </summary>
        public Dictionary<string,ConfigPackage> Packages {
            get {
                return m_Packages;
            }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// Configsオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public Configs(){
            this.m_Packages = new Dictionary<string,ConfigPackage>();
        }
        #endregion

        #region AddPackage
        /// <summary>
        /// ConfigPackageを追加します。
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
        /// 指定した名前を持つConfigaPackageを削除します。
        /// </summary>
        /// <param name="name"></param>
        public void RemovePackage(string name) {
            this.m_Packages.Remove(name);
        }
        #endregion

        #region ClearPackage
        /// <summary>
        /// ConfigPackageを全て削除します。
        /// </summary>
        public void ClearPackage() {
            this.m_Packages.Clear();
        }
        #endregion

        #region GetPackage
        /// <summary>
        /// 指定した名前を持つConfigPackageを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigPackage GetPackage(string name) {
            return this.m_Packages[name];
        }
        #endregion

        #region GetConfig
        /// <summary>
        /// 指定した名前を持つConfigItemを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigItem GetConfig(string name) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            return this.GetConfig(path);
        }
        /// <summary>
        /// 指定したConfigPathを持つConfigItemを返します。
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
            throw new ArgumentException("指定された項目は存在しません。");
        }
        #endregion

        #region AddConfig
        /// <summary>
        /// 名前を指定して、設定にbool値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConfig(string name,bool value) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,new BoolConfigItem(this.GetItemName(path),value));
        }
        /// <summary>
        /// 名前を指定して、設定に文字列を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConfig(string name,string value) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,new StringConfigItem(this.GetItemName(path),value));
        }
        /// <summary>
        /// 名前を指定して、設定に整数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConfig(string name,int value) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,new IntConfigItem(this.GetItemName(path),value));
        }
        /// <summary>
        /// 名前を指定して、設定に実数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConfig(string name,double value) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,new DoubleConfigItem(this.GetItemName(path),value));
        }
        /// <summary>
        /// 名前を指定して設定にConfigItemを追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void AddConfig(string name,ConfigItem item) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            this.AddConfig(path,item);
        }
        /// <summary>
        /// ConfigPathを指定して、設定にConfigItemを追加します。
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
        /// 指定した名前を持つConfigが存在するかどうかを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsConfig(string name) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(name);
            return this.ContainsConfig(path);
        }
        /// <summary>
        /// 指定したConfigPathを持つConfigが存在するかどうかを返します。
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

        #region ファイル入出力
        /// <summary>
        /// 指定したディレクトリからConfigをロードします。
        /// </summary>
        /// <param name="dirName"></param>
        public void Load(string dirName) {
            this.OnLoad(dirName,"");
        }
		/// <summary>
		/// 指定したディレクトリからConfigをロードします。
		/// </summary>
		/// <param name="dirName">Configをロードするディレクトリ</param>
		/// <param name="packageName">指定したディレクトリが含まれるパッケージ。ルートパッケージの場合は空文字列です。</param>
        private void OnLoad(string dirName,string packageName) {
            if(!Directory.Exists(dirName)) {
                throw new DirectoryNotFoundException(string.Format("指定したディレクトリは存在しません。\r\n{0}\r\n",dirName));
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
        /// ディレクトリ名を指定して、Condfigをすべて保存します。
        /// </summary>
        /// <param name="dirName"></param>
        public void Save(string dirName) {
			//すべて削除してから保存(不要ファイルが残るため)
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
			//"Setting.Application.Kucl:Application.FastSetup"の場合、Setting\Application\Kucl.xmlが作られる
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
    /// Configパッケージを表すクラスです。
    /// 1つのConfigパッケージは1つのファイルに対応します。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class ConfigPackage{

		#region メンバ変数
		private Dictionary<string,Config> m_Configs;
		private string m_Name;

		#endregion

		#region コンストラクタ
        /// <summary>
        /// ConfigPackageオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
		public ConfigPackage(){
			this.m_Name = "";
			this.m_Configs = new Dictionary<string,Config>();
		}
        /// <summary>
        /// ConfigPackageオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
		public ConfigPackage(string name){
			this.m_Name = name;
            this.m_Configs = new Dictionary<string,Config>();
		}
		#endregion
		
		#region プロパティ
        /// <summary>
        /// 指定した名前をもつConfigを取得するインデクサです。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public Config this[string name]{
			get{
				return this.m_Configs[name];
			}
		}
        /// <summary>
        /// 名前とConfigを関連付けたハッシュテーブルを取得します。
        /// </summary>
        public Dictionary<string,Config> Configs {
			get{
				return this.m_Configs;
			}
		}
        /// <summary>
        /// パッケージ名を取得または設定します。
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

		#region Configの取得、追加、削除
        /// <summary>
        /// 指定した名前のConfigが存在するかどうかを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name) {
            return this.m_Configs.ContainsKey(name);
        }
        /// <summary>
        /// Configを追加します。
        /// </summary>
        /// <param name="value"></param>
		public void AddConfig(Config value){
			this.m_Configs.Add(value.Name,value);
		}
        /// <summary>
        /// 名前を指定してConfigを取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public Config GetConfig(string name){
			return this.m_Configs[name];
		}
        /// <summary>
        /// 名前を指定してConfigを削除します。
        /// </summary>
        /// <param name="name"></param>
		public void Remove(string name){
			this.m_Configs.Remove(name);
		}
		#endregion

		#region ファイル入出力
        /// <summary>
        /// ファイル名を指定してConfigパッケージをファイルに保存します。
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
        /// ファイル名とパッケージ名を指定してConfigパッケージをファイルから読み込みます。
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
    /// 設定を表すクラスです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class Config{
		
		#region メンバ変数
		private string m_Name;
		private ContainerConfigItem m_Root;
		#endregion

		#region コンストラクタ
        /// <summary>
        /// Configオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
		public Config(string name){
			this.m_Name = name;
			this.m_Root = new ContainerConfigItem(name);
		}
        private Config(){
            //パラメータ無しのコンストラクタは、Config.LoadFileで必要
        }
		#endregion

		#region プロパティ
        /// <summary>
        /// Configオブジェクトの名前を取得します。
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
        /// 指定した完全パスを持つ設定項目が存在するかどうかを返します。
        /// </summary>
        /// <param name="CompletePath"></param>
        /// <returns></returns>
		public bool Contains(string CompletePath){
            ConfigPath path = ConfigPathProvider.GetConfigPath(CompletePath);
            return this.Contains(path);
        }
        /// <summary>
        /// 指定したパConfigPathを持つ設定項目が存在するかどうかを返します。
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
            //Listに格納されたパスを順にチェック
			if(parent.Items.ContainsKey(path[0])){
				ConfigItem item = parent.Items[path[0]];
				if(path.Count == 1){
                    //最後までマッチしたらtrue
					return true;
				}
				else if(item.Type == ConfigItemType.Container){
                    //マッチするたびに削除して下の階層へ降りる
					path.RemoveAt(0);
					return this.Contains((ContainerConfigItem)item,path);
				}
			}
			return false;
		}
		#endregion

		#region GetConfig
        /// <summary>
        /// パスを指定してConfigItemを取得するインデクサです。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
		public ConfigItem this[string path]{
			get{
				return this.GetConfig(path);
			}
		}
        /// <summary>
        /// 完全パスを指定してConfigItemを取得します。
        /// </summary>
        /// <param name="completePath"></param>
        /// <returns></returns>
        public ConfigItem GetConfig(string completePath) {
            ConfigPath path = ConfigPathProvider.GetConfigPath(completePath);
            return this.GetConfig(path);
        }
        /// <summary>
        /// ConfigPathを指定してConfigIteを取得します。
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
		/// 名前を指定して、設定にbool値を追加します。
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void AddConfig(string name,bool value){
			this.AddConfig(name,new BoolConfigItem(this.GetItemName(name),value));
		}
        /// <summary>
        /// 名前を指定して、設定に文字列を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void AddConfig(string name,string value){
			this.AddConfig(name,new StringConfigItem(this.GetItemName(name),value));
		}
        /// <summary>
        /// 名前を指定して、設定に整数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void AddConfig(string name,int value){
			this.AddConfig(name,new IntConfigItem(this.GetItemName(name),value));
		}
        /// <summary>
        /// 名前を指定して、設定に実数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void AddConfig(string name,double value){
			this.AddConfig(name,new DoubleConfigItem(this.GetItemName(name),value));
		}
        /// <summary>
        /// 名前を指定して設定にConfigItemを追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
		public void AddConfig(string name,ConfigItem item){
            ConfigPath path = ConfigPathProvider.GetConfigPathWithoutPackageName(name);
            this.AddConfig(path,item);
        }
        /// <summary>
        /// ConfigPathを指定して、設定にConfigItemを追加します。
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
				throw new ArgumentException("間違ったパスルートが指定されました。");
			}
			this.AddConfig(this.m_Root,path,item);
		}

		private void AddConfig(ContainerConfigItem parent,List<String> path,ConfigItem item){
			if(path.Count == 1){
				//parentは追加すべきアイテムの直の親
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
		/// 完全パスを指定してConfigItemを削除します。
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
					throw new ArgumentException("不正なパスです。");
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
        /// 設定値を全て削除します。
        /// </summary>
		public void ClearConfig(){
			this.m_Root.Items.Clear();
		}
		#endregion

		#region ファイル入出力

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
    /// 全ての設定項目を表すConfigItemの基本クラスです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public abstract class ConfigItem{
		private ConfigItemType m_Type;
		private string m_Name;
        /// <summary>
        /// 型名と名前を指定してConfigItemオブジェクトを初期化します。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
		protected ConfigItem(ConfigItemType type,string name){
			this.m_Type = type;
			this.m_Name = name;
		}
        /// <summary>
        /// 設定項目名を取得します。
        /// </summary>
		public string Name{
			get{
				return this.m_Name;
			}
		}
        /// <summary>
        /// Config値の型名を取得します。
        /// </summary>
		public ConfigItemType Type{
			get{
				return this.m_Type;
			}
		}
        /// <summary>
        /// Config値を取得、設定します。
        /// </summary>
		public abstract object Value{get;set;}
        /// <summary>
        /// Config値をXMLに書き出します。
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
        /// Condfig値をXMLから読み出します。
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
        /// 派生クラスでオーバーライドされると、Config値をXMLに書き出す処理を実行します。
        /// </summary>
        /// <param name="writer"></param>
		protected abstract void OnSave(XmlTextWriter writer);
        /// <summary>
        /// 派生クラスでオーバーライドされると、Config値をXMLから読み出す処理を実行します。
        /// </summary>
        /// <param name="reader"></param>
		protected abstract void OnLoad(XmlTextReader reader);
        /// <summary>
        /// ValueプロパティのToString()メソッドで返される結果をXMLに書き込みます。
        /// ValueプロパティのToString()メソッドが、適切なConfig値を返す場合、
        /// OnSaveメソッドではWriteDefaultValueメソッドのみを呼び出します。
        /// </summary>
        /// <param name="writer"></param>
		protected void WriteDefaultValue(XmlTextWriter writer){
			writer.WriteString(this.Value.ToString());
		}
	}
    #endregion

    #region ConfigItemType
    /// <summary>
    /// Config値の型名を示します。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public enum ConfigItemType{
        /// <summary>
        /// 複数のConfigを内部に格納するアイテムです。
        /// </summary>
		Container,
        /// <summary>
        /// 真偽値を保持するアイテムです。
        /// </summary>
		Bool,
        /// <summary>
        /// 文字列を保持するアイテムです。
        /// </summary>
		String,
        /// <summary>
        /// 整数値を保持するアイテムです。
        /// </summary>
		Int,
        /// <summary>
        /// 実数値を保持するアイテムです。
        /// </summary>
		Double
	}
    #endregion

    #region ConfigItemの派生クラス(基本型)

    #region ContainerConfigItem
    /// <summary>
    /// 他のConfigItemを包含するConfigItemです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class ContainerConfigItem:ConfigItem{
		
        #region コンストラクタ
        /// <summary>
        /// ContainerConfigItemクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public ContainerConfigItem(string name)
            : base(ConfigItemType.Container, name) {
            this.m_Children = new Dictionary<string, ConfigItem>();
        } 
        #endregion

        #region プロパティ
        private Dictionary<string, ConfigItem> m_Children;
        /// <summary>
        /// 保持するConfigItemのハッシュテーブルを取得します。
        /// </summary>
        public Dictionary<string, ConfigItem> Items {
            get {
                return this.m_Children;
            }
        }
        /// <summary>
        /// 保持するConfigItemの個数を取得します。
        /// 設定処理は無効です。
        /// </summary>
        public override object Value {
            get {
                return this.m_Children.Count;
            }
            set {
            }
        } 
        #endregion

        #region シリアル化
        /// <summary>
        /// ContainerConfigItemオブジェクトをXMLに書き込みます。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            writer.WriteElementString("Count", this.m_Children.Count.ToString());
            foreach (ConfigItem item in this.m_Children.Values) {
                ConfigItem.Save(writer, item);
            }
        }
        /// <summary>
        /// ContainerConfigItemオブジェクトをXMLから読み出します。
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
    /// Bool値を持つConfigItemです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class BoolConfigItem:ConfigItem{
        
        #region コンストラクタ
        /// <summary>
        /// BoolConfigItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public BoolConfigItem(string name) : this(name, true) {
        }
        /// <summary>
        /// BoolConfigItemオブジェクトの新しいインスタンスを初期化します。
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
        /// BoolConfigItemオブジェクトの値を取得または設定します。
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

        #region シリアル化
        /// <summary>
        /// BoolConfigItemオブジェクトをXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// BoolConfigItemオブジェクトをXMLから読み出します。
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
    /// 文字列を持つConfigItemです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class StringConfigItem:ConfigItem{
        
        #region コンストラクタ
        /// <summary>
        /// StringConfigItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public StringConfigItem(string name) : this(name, "") {
        }
        /// <summary>
        /// StringConfigItemオブジェクトの新しいインスタンスを初期化します。
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
        /// StringConfigItemオブジェクトの値を取得または設定します。
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

        #region シリアル化
        /// <summary>
        /// StringConfigItemオブジェクトをXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// StringConfigItemオブジェクトをXMLから読み出します。
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
    /// 整数値を持つConfigItemです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class IntConfigItem:ConfigItem{
        
        #region コンストラクタ
        /// <summary>
        /// IntConfigItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public IntConfigItem(string name) : this(name, 0) {
        }
        /// <summary>
        /// IntConfigItemオブジェクトの新しいインスタンスを初期化します。
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
        /// IntConfigItemオブジェクトの値を取得または設定します。
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

        #region シリアル化
        /// <summary>
        /// IntConfigItemオブジェクトをXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// IntConfigItemオブジェクトをXMLから読み出します。
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
    /// 実数値を持つConfigItemです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class DoubleConfigItem:ConfigItem{
        
        #region コンストラクタ
        /// <summary>
        /// DoubleConfigItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public DoubleConfigItem(string name) : this(name, 0.0) {
        }
        /// <summary>
        /// DoubleConfigItemオブジェクトの新しいインスタンスを初期化します。
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
        /// DoubleConfigItemオブジェクトの値を取得または設定します。
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

        #region シリアル化
        /// <summary>
        /// DoubleConfigItemオブジェクトをXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// DoubleConfigItemオブジェクトをXMLから読み出します。
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
    /// Configを使用するオブジェクトを示すインターフェースです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public interface IUseConfig{
        /// <summary>
        /// 既定値を示すConfigを作成します。
        /// </summary>
        /// <returns></returns>
		Configs CreateDefaultConfig();
		/// <summary>
		/// configを適用します
		/// </summary>
		/// <param name="config"></param>
		void ApplyConfig(Configs config);
        /// <summary>
        /// configを更新します。
        /// </summary>
        /// <param name="config"></param>
		void ReflectConfig(Configs config);
        //string PackageName{get;}
        //string ConfigName{get;}
        /// <summary>
        /// 指定したConfig値が既定値かどうかを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		bool IsDefaultValue(string name,ConfigItem value);
	}
    #endregion

    #region UseConfigObjectCollection
    /// <summary>
    /// IUseConfigインターフェースを実装したオブジェクトのコレクションを表します。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class UseConfigObjectCollection : System.Collections.CollectionBase {
        /// <summary>
        /// IUseConfigオブジェクトを取得、設定するインデクサです。
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
        /// IUseConfigオブジェクトをコレクションに追加します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int Add(IUseConfig value) {
			return this.List.Add(value);
		}
        /// <summary>
        /// IUseConfigオブジェクトをコレクションから削除します。
        /// </summary>
        /// <param name="value"></param>
		public void Remove(IUseConfig value) {
			this.List.Remove(value);
		}
        /// <summary>
        /// 位置を指定して、IUseConfigオブジェクトをコレクションに挿入します。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		public void Insert(int index, IUseConfig value) {
			this.List.Insert(index, value);
		}
        /// <summary>
        /// IUseConfigオブジェクトがコレクションに含まれているかどうかを返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public bool Contains(IUseConfig value) {
			return this.List.Contains(value);
		}
        /// <summary>
        /// 指定したIUseConfigオブジェクトのコレクション内の位置を返します。
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
    /// IUseConfigを実装するオブジェクトに対してのヘルパメソッドを提供するクラスです。
    /// </summary>
    [Obsolete("Kucl.Xml.Xlcfg名前空間のクラスを使用してください。")]
    public class UseConfigHelper {

        #region メンバ変数
        private Configs m_DefaultConfig;
		private Configs m_Config;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// UseConfigHelperオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="defaultConfig"></param>
        public UseConfigHelper(Configs defaultConfig){
			this.m_DefaultConfig = defaultConfig;
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 対象となるConfigsオブジェクトを取得または設定します。
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
        /// 名前を指定してConfigItemが変更されていないかどうかを返します。
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
        /// 指定した名前を持つConfigItemを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public ConfigItem GetConfigItem(string name){
			ConfigItem item;
			if(!this.m_Config.ContainsConfig(name)){
#if DEBUG
				System.Diagnostics.Debug.WriteLine("存在しないConfig\r\n" + name);
#endif
				//存在しない設定の場合、デフォルトの設定から取得
				item = this.m_DefaultConfig.GetConfig(name);
				if(item == null){
					throw new ArgumentException(string.Format("{0}は存在しません",name));
				}
			}
			else{
				item = this.m_Config.GetConfig(name);
			}
			return item;
		}
		#endregion

        #region 特定の型の値の取得

        #region GetIntFromConfig
        /// <summary>
        /// 指定した名前を持つ整数値を返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIntFromConfig(string name) {
            return (int)this.GetConfigItem(name).Value;
        }
        #endregion

        #region GetDoubleFromConfig
        /// <summary>
        /// 指定した名前を持つ実数値を取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double GetDoubleFromConfig(string name) {
            return (double)this.GetConfigItem(name).Value;
        }
        #endregion

        #region GetStringFromConfig
        /// <summary>
        /// 指定した名前を持つ文字列を取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetStringFromConfig(string name) {
            return (string)this.GetConfigItem(name).Value;
        }
        #endregion

        #region GetBoolFromConfig
        /// <summary>
        /// 指定した名前を持つbool値を取得します。
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
