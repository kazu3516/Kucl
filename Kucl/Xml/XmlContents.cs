using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;

namespace Kucl.Xml {

    //TODO:ファイルフォーマットのバージョンを含めることができるように変更する
    //TODO:ファイルフォーマットのバージョンにより入出力を切り替えできるようにする


    #region XmlContentsPath
    /// <summary>
    /// XmlContentsのパスを表すクラスです。
    /// XmlContentsのパスはパッケージ名とパス名に分割され、
    /// それぞれ階層を持ちます。
    /// (例)package1.package2.package3:path1.path2.path3…
    /// </summary>
    public class XmlContentsPath {

        #region メンバ変数、プロパティ

        #region PackageName
        private string[] m_PackageName;
        /// <summary>
        /// パッケージ名を取得します。
        /// </summary>
        public string[] PackageName {
            get {
                return m_PackageName;
            }
        }
        #endregion

        #region Path
        private string[] m_Path;
        /// <summary>
        /// パス名を取得します。
        /// </summary>
        public string[] Path {
            get {
                return m_Path;
            }
        }
        #endregion

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsPathオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="package"></param>
        /// <param name="items"></param>
        public XmlContentsPath(string[] package, string[] items) {
            this.m_PackageName = package;
            this.m_Path = items;
        }
        #endregion

        /// <summary>
        /// PackageNameを返します。
        /// </summary>
        /// <returns></returns>
        public string GetPackageNameString() {
            return XmlContentsPathProvider.GetPathString(this.m_PackageName);
        }

        /// <summary>
        /// RootPathを返します。
        /// </summary>
        /// <returns></returns>
        public string GetContentsRootPath() {
            return this.m_Path[0];
        }

        /// <summary>
        /// Item名を返します。
        /// </summary>
        /// <returns></returns>
        public string GetItemName() {
            return this.m_Path[this.m_Path.Length - 1];
        }
    }

    #endregion

    #region XmlContentsPathProvider
    /// <summary>
    /// 文字列からXmlContentsPathを生成するクラスです。
    /// </summary>
    public static class XmlContentsPathProvider {

        #region 定数
        /// <summary>
        /// 階層を区切る文字列'.'を表します。
        /// </summary>
        public const char DelimiterChar = '.';
        /// <summary>
        /// XmlContentsとPackageを区切る文字列':'を表します。
        /// </summary>
        public const char PackageDelimiterChar = ':';

        #endregion

        #region GetPathString
        /// <summary>
        /// パス形式の文字列を生成します。
        /// 文字列配列はDelimiterCharで連結されます。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathString(string[] path) {
            return string.Join(XmlContentsPathProvider.DelimiterChar.ToString(), path);
        }
        #endregion

        #region GetXmlContentsPath
        /// <summary>
        /// 完全パスからXmlContentsPathオブジェクトを生成します。
        /// パッケージ名を含まないパスを指定された場合、
        /// GetXmlContentsPathWithoutPackageNameを内部で呼び出し、
        /// パス名のみのXmlContentsPathオブジェクトを生成します。
        /// </summary>
        /// <param name="completePath"></param>
        /// <returns></returns>
        public static XmlContentsPath GetXmlContentsPath(string completePath) {
            if (!completePath.Contains(XmlContentsPathProvider.PackageDelimiterChar.ToString())) {
                //パッケージ名が含まれない不完全パスを指定された場合、パス名のみを返す。
                return XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(completePath);
            }
            string[] path = completePath.Split(new char[] { XmlContentsPathProvider.PackageDelimiterChar }, 2);
            string[] path1 = path[0].Split(XmlContentsPathProvider.DelimiterChar);
            string[] path2 = path[1].Split(XmlContentsPathProvider.DelimiterChar);
            return new XmlContentsPath(path1, path2);
        }
        #endregion

        #region GetXmlContentsPathWithoutPackageName
        /// <summary>
        /// XmlContentsから生成される不完全パスからXmlContentsPathを生成。
        /// Packagenameは考慮しない。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlContentsPath GetXmlContentsPathWithoutPackageName(string path) {
            string targetPath = path;
            if (path.Contains(XmlContentsPathProvider.PackageDelimiterChar.ToString())) {
                //packageNameが含まれている場合、パス名のみ抽出
                targetPath = path.Substring(path.IndexOf(XmlContentsPathProvider.PackageDelimiterChar) + 1);
            }
            string[] path1 = targetPath.Split(XmlContentsPathProvider.DelimiterChar);
            return new XmlContentsPath(new string[] { }, path1);
        }
        #endregion

    }
    #endregion



    #region XmlContentsModel
    /// <summary>
    /// XmlContentsPackageを管理する最上位のXmlContentsModelオブジェクトを表すクラスです。
    /// </summary>
    public class XmlContentsModel {

        #region メンバ変数
        private Dictionary<string, XmlContentsPackage> m_Packages;

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
        /// 名前を指定してXmlContentsPackageの取得または設定を行うインデクサです。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlContentsPackage this[string name] {
            get {
                return this.m_Packages[name];
            }
            set {
                this.m_Packages[name] = value;
            }
        }
        /// <summary>
        /// 名前とXmlContentsPackageを関連付けたハッシュテーブルを取得します。
        /// </summary>
        public Dictionary<string, XmlContentsPackage> Packages {
            get {
                return m_Packages;
            }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsModelクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsModel() {
            this.m_Packages = new Dictionary<string, XmlContentsPackage>();
        }
        #endregion

        #region AddPackage
        /// <summary>
        /// XmlContentsPackageを追加します。
        /// </summary>
        /// <param name="package"></param>
        public void AddPackage(XmlContentsPackage package) {
            if (this.m_Packages.ContainsKey(package.Name)) {
                this.m_Packages[package.Name] = package;
            }
            else {
                this.m_Packages.Add(package.Name, package);
            }
        }
        #endregion

        #region RemovePackage
        /// <summary>
        /// 指定した名前を持つXmlContentsaPackageを削除します。
        /// </summary>
        /// <param name="name"></param>
        public void RemovePackage(string name) {
            this.m_Packages.Remove(name);
        }
        #endregion

        #region ClearPackage
        /// <summary>
        /// XmlContentsPackageを全て削除します。
        /// </summary>
        public void ClearPackage() {
            this.m_Packages.Clear();
        }
        #endregion

        #region GetPackage
        /// <summary>
        /// 指定した名前を持つXmlContentsPackageを返します。
        /// 存在しない名前を指定した場合、新しくパッケージを作成します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlContentsPackage GetPackage(string name) {
            if (this.m_Packages.ContainsKey(name)) {
                return this.m_Packages[name];
            }
            else {
                XmlContentsPackage package = this.CreateXmlContentsPackage(name);
                this.AddPackage(package);
                return package;
            }
        }
        #endregion

        #region GetXmlContentsItem
        /// <summary>
        /// 指定した名前を持つXmlContentsItemを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlContentsItem GetXmlContentsItem(string name) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(name);
            return this.GetXmlContentsItem(path);
        }
        /// <summary>
        /// 指定したXmlContentsPathを持つXmlContentsItemを返します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public XmlContentsItem GetXmlContentsItem(XmlContentsPath path) {
            string packageName = path.GetPackageNameString();
            string contentsName = path.GetContentsRootPath();
            if (this.m_Packages.ContainsKey(packageName)) {
                if (this.m_Packages[packageName].Contains(contentsName)) {
                    try {
                        return this.m_Packages[packageName][contentsName].GetXmlContentsItem(path);
                    }
                    catch {
                        //例外発生時は何もせず抜けて、メソッド末尾でArgumentExceptionを発生させる
                    }
                }
            }
            throw new ArgumentException("指定された項目は存在しません。");
        }
        #endregion

        #region AddXmlContentsItem
        /// <summary>
        /// 名前を指定して、設定にbool値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, bool value) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(name);
            string packageName = path.GetPackageNameString();
            XmlContentsPackage package = this.GetPackage(packageName);
            package.AddXmlContentsItem(name, value);
        }
        /// <summary>
        /// 名前を指定して、設定に文字列を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, string value) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(name);
            string packageName = path.GetPackageNameString();
            XmlContentsPackage package = this.GetPackage(packageName);
            package.AddXmlContentsItem(name, value);
        }
        /// <summary>
        /// 名前を指定して、設定に整数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, int value) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(name);
            string packageName = path.GetPackageNameString();
            XmlContentsPackage package = this.GetPackage(packageName);
            package.AddXmlContentsItem(name, value);
        }
        /// <summary>
        /// 名前を指定して、設定に実数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, double value) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(name);
            string packageName = path.GetPackageNameString();
            XmlContentsPackage package = this.GetPackage(packageName);
            package.AddXmlContentsItem(name, value);
        }

        /// <summary>
        /// 名前を指定して設定にXmlContentsItemを追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void AddXmlContentsItem(string name, XmlContentsItem item) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(name);
            this.AddXmlContentsItem(path, item);
        }
        /// <summary>
        /// XmlContentsPathを指定して、設定にXmlContentsItemを追加します。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        public void AddXmlContentsItem(XmlContentsPath path, XmlContentsItem item) {
            string packageName = path.GetPackageNameString();
            XmlContentsPackage package = this.GetPackage(packageName);
            package.AddXmlContentsItem(path, item);
        }

        #endregion

        #region ContainsXmlContentsItem
        /// <summary>
        /// 指定した名前を持つXmlContentsItemが存在するかどうかを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsXmlContentsItem(string name) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(name);
            return this.ContainsXmlContentsItem(path);
        }
        /// <summary>
        /// 指定したXmlContentsPathを持つXmlContentsが存在するかどうかを返します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ContainsXmlContentsItem(XmlContentsPath path) {
            string packageName = path.GetPackageNameString();
            string contentsName = path.GetContentsRootPath();
            return this.m_Packages.ContainsKey(packageName) &&
                this.m_Packages[packageName].Contains(contentsName) &&
                this.m_Packages[packageName][contentsName].Contains(path);
        }
        #endregion

        #region ファイル入出力
        /// <summary>
        /// 指定したディレクトリからXmlContentsをロードします。
        /// </summary>
        /// <param name="dirName"></param>
        public virtual void Load(string dirName) {
            this.OnLoad(dirName, "");
        }
        /// <summary>
        /// 指定したディレクトリからXmlContentsをロードします。
        /// </summary>
        /// <param name="dirName">XmlContentsをロードするディレクトリ</param>
        /// <param name="packageName">指定したディレクトリが含まれるパッケージ。ルートパッケージの場合は空文字列です。</param>
        private void OnLoad(string dirName, string packageName) {
            if (!Directory.Exists(dirName)) {
                throw new DirectoryNotFoundException(string.Format("指定したディレクトリは存在しません。\r\n{0}\r\n", dirName));
            }
            //packageのVersionを先読みするためのReader
            PackageVersionReader versionReader = new PackageVersionReader();

            //xmlファイルごとに読み込み
            foreach (string filename in Directory.GetFiles(dirName, "*.xml")) {
                string pName = packageName != "" ? packageName + "." : "";
                pName += Path.GetFileNameWithoutExtension(filename);

                //NOTE:XmlContentsReaderの導入により廃止
                //XmlContentsPackage package = this.CreateXmlContentsPackage(pName);
                //package.Load(Path.Combine(dirName, filename));

                //TODO:XmlContentsReaderFactoryを使用したインスタンス生成に変更する。バージョン判断のためのPreLoadが必要。
                XmlContentsPackageReadInfo info = new XmlContentsPackageReadInfo() {
                    FileName = filename,
                    PackageName = pName,
                    Owner = this
                };
                string version = versionReader.ReadVersion(info, out XmlContentsPackage package);
                info.PreLoadedPackage = package;
                //XmlContentsReader reader = new XmlContentsReader_01();
                XmlContentsReader reader = XmlContentsReaderFactory.Create("", version);
                package = reader.LoadPackage(info);

                //読み込んだXmlContentsPackageを追加
                this.AddPackage(package);
            }
            foreach (string dir in Directory.GetDirectories(dirName)) {
                string pName = packageName != "" ? "." : "";
                pName += Path.GetFileNameWithoutExtension(dir);
                this.OnLoad(dir, pName);
            }
        }

        /// <summary>
        /// ディレクトリ名を指定して、XmlContentsをすべて保存します。
        /// </summary>
        /// <param name="dirName"></param>
        public virtual void Save(string dirName) {
            //すべて削除してから保存(不要ファイルが残るため)
            if (this.m_DoClearFileOnSave) {
                foreach (string filename in Directory.GetFiles(dirName)) {
                    File.Delete(filename);
                }
                foreach (string subdir in Directory.GetDirectories(dirName)) {
                    Directory.Delete(subdir, true);
                }
            }
            foreach (XmlContentsPackage package in this.m_Packages.Values) {
                List<string> packageName = new List<string>(package.Name.Split('.'));
                this.OnSave(dirName, packageName, package);
            }
        }
        private void OnSave(string dirName, List<string> packageName, XmlContentsPackage package) {
            //"Setting.Application.Kucl:Application.FastSetup"の場合、Setting\Application\Kucl.xmlが作られる
            if (!Directory.Exists(dirName)) {
                Directory.CreateDirectory(dirName);
            }
            if (packageName.Count == 1) {
                string filename = Path.Combine(dirName, packageName[0]) + ".xml";

                //NOTE:XmlContentsWriterの導入により廃止
                //package.Save(filename);

                //XmlContentsWriter writer = new XmlContentsWriter_00();

                //基本、書き込みは最新バージョンでOK
                //（必ず読み込みとセットになるため、Ver1.0で書き込みしたアプリケーションはVer1.0の読み込みができる。）
                string version = "1.0";
                XmlContentsWriter writer = XmlContentsWriterFactory.Create("", version);
                XmlContentsPackageWriteInfo info = new XmlContentsPackageWriteInfo() {
                    FileName = filename,
                    Package = package
                };
                writer.SavePackage(info);
            }
            else {
                string subdir = packageName[0];
                packageName.RemoveAt(0);
                this.OnSave(Path.Combine(dirName, subdir), packageName, package);
            }

        }
        #endregion

        #region virtualメソッド
        /// <summary>
        /// このXmlContentsModelのインスタンスで使用するXmlContentsPackageクラスのインスタンスを生成します。
        /// 派生クラスで実装された場合、XmlContentsPackageクラスの派生クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public virtual XmlContentsPackage CreateXmlContentsPackage(string packageName) {
            return new XmlContentsPackage(packageName);
        }
        #endregion
    }
    #endregion

    #region XmlContentsPackage
    /// <summary>
    /// XmlContentsパッケージを表すクラスです。
    /// 1つのXmlContentsパッケージは1つのファイルに対応します。
    /// </summary>
    public class XmlContentsPackage {

        #region 定数扱いのプロパティ(派生クラスのみ変更可)
        /// <summary>
        /// シリアル化する際のルートエレメント名を取得します。
        /// </summary>
        public virtual string PackageRootElement {
            get {
                return "Package";
            }
        }
        /// <summary>
        /// シリアル化する際のルートエレメントが持つ属性名を取得します。
        /// この属性は子エレメントの個数を表します。
        /// </summary>
        public virtual string PackageRootCountAttribute {
            get {
                return "Count";
            }
        }
        /// <summary>
        /// シリアル化する際のルートエレメントが持つ属性名を取得します。
        /// この属性はパッケージのシリアル化方法のVersionを表します。
        /// </summary>
        public virtual string PackageRootVersionAttribute {
            get {
                return "Version";
            }
        }
        #endregion

        #region メンバ変数
        private Dictionary<string, XmlContents> m_ContentsList;
        private string m_Name;

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsPackageクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsPackage() {
            this.m_Name = "";
            this.m_ContentsList = new Dictionary<string, XmlContents>();
        }
        /// <summary>
        /// XmlContentsPackageクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public XmlContentsPackage(string name) {
            this.m_Name = name;
            this.m_ContentsList = new Dictionary<string, XmlContents>();
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 指定した名前をもつXmlContentsを取得するインデクサです。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlContents this[string name] {
            get {
                return this.m_ContentsList[name];
            }
        }
        /// <summary>
        /// 名前とXmlContentsを関連付けたハッシュテーブルを取得します。
        /// </summary>
        public Dictionary<string, XmlContents> XmlContentsTable {
            get {
                return this.m_ContentsList;
            }
        }
        /// <summary>
        /// パッケージ名を取得または設定します。
        /// </summary>
        public string Name {
            get {
                return this.m_Name;
            }
            set {
                this.m_Name = value;
            }
        }
        #endregion

        #region XmlContentsの取得、追加、削除
        /// <summary>
        /// 指定した名前のXmlContentsが存在するかどうかを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name) {
            return this.m_ContentsList.ContainsKey(name);
        }
        /// <summary>
        /// XmlContentsを追加します。
        /// </summary>
        /// <param name="value"></param>
        public void AddXmlContents(XmlContents value) {
            this.m_ContentsList.Add(value.Name, value);
        }


        #region GetXmlContents
        /// <summary>
        /// 名前を指定してXmlContentsを取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlContents GetXmlContents(string name) {
            if (this.Contains(name)) {
                return this[name];
            }
            else {
                XmlContents contents = this.CreateXmlContents(name);
                this.AddXmlContents(contents);
                return contents;
            }
        }
        /// <summary>
        /// パスを指定してXmlContentsを取得します。
        /// パスが存在しない場合、新しくXmlContentsを作成します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public XmlContents GetXmlContents(XmlContentsPath path) {
            string contentsName = path.GetContentsRootPath();
            return this.GetXmlContents(contentsName);
        }
        #endregion

        /// <summary>
        /// 名前を指定してXmlContentsを削除します。
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name) {
            this.m_ContentsList.Remove(name);
        }
        /// <summary>
        /// このパッケージが保持するXmlContentsをすべて削除します。
        /// </summary>
        public void ClearXmlContents() {
            this.m_ContentsList.Clear();
        }
        #endregion

        #region AddXmlContentsItem
        /// <summary>
        /// 名前を指定して、設定にbool値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, bool value) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(name);
            string contentsName = path.GetContentsRootPath();
            XmlContents contents = this.GetXmlContents(contentsName);
            contents.AddXmlContentsItem(name, value);
        }
        /// <summary>
        /// 名前を指定して、設定に文字列を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, string value) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(name);
            string contentsName = path.GetContentsRootPath();
            XmlContents contents = this.GetXmlContents(contentsName);
            contents.AddXmlContentsItem(name, value);
        }
        /// <summary>
        /// 名前を指定して、設定に整数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, int value) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(name);
            string contentsName = path.GetContentsRootPath();
            XmlContents contents = this.GetXmlContents(contentsName);
            contents.AddXmlContentsItem(name, value);
        }
        /// <summary>
        /// 名前を指定して、設定に実数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, double value) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(name);
            string contentsName = path.GetContentsRootPath();
            XmlContents contents = this.GetXmlContents(contentsName);
            contents.AddXmlContentsItem(name, value);
        }

        /// <summary>
        /// 名前を指定して設定にXmlContentsItemを追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void AddXmlContentsItem(string name, XmlContentsItem item) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(name);
            this.AddXmlContentsItem(path, item);
        }
        /// <summary>
        /// XmlContentsPathを指定して、設定にXmlContentsItemを追加します。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        public void AddXmlContentsItem(XmlContentsPath path, XmlContentsItem item) {
            string contentsName = path.GetContentsRootPath();
            XmlContents contents = this.GetXmlContents(contentsName);
            contents.AddXmlContentsItem(path, item);
        }

        #endregion

        #region GetXmlContentsItem
        /// <summary>
        /// 指定した名前を持つXmlContentsItemを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlContentsItem GetXmlContentsItem(string name) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(name);
            return this.GetXmlContentsItem(path);
        }
        /// <summary>
        /// 指定したXmlContentsPathを持つXmlContentsItemを返します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public XmlContentsItem GetXmlContentsItem(XmlContentsPath path) {
            string contentsName = path.GetContentsRootPath();
            if (this.Contains(contentsName)) {
                return this[contentsName].GetXmlContentsItem(path);
            }
            throw new ArgumentException("指定された項目は存在しません。");
        }
        #endregion

        #region ContainsXmlContentsItem
        /// <summary>
        /// 指定した名前を持つXmlContentsItemが存在するかどうかを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsXmlContentsItem(string name) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(name);
            return this.ContainsXmlContentsItem(path);
        }
        /// <summary>
        /// 指定したXmlContentsPathを持つXmlContentsが存在するかどうかを返します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ContainsXmlContentsItem(XmlContentsPath path) {
            string contentsName = path.GetContentsRootPath();
            return this.Contains(contentsName) && this[contentsName].Contains(path);
        }
        #endregion

        #region ファイル入出力
        //NOTE:ファイルフォーマットのバージョンを導入するため、Packageに直接Saveメソッドを持たせる設計は廃止。XmlContentsReader/Writerを使用する。

        /// <summary>
        /// ファイル名を指定してXmlContentsパッケージをファイルに保存します。
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename) {
            XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            try {
                writer.WriteStartDocument();
                writer.WriteStartElement(PackageRootElement);
                writer.WriteAttributeString(PackageRootCountAttribute, this.m_ContentsList.Count.ToString());
                foreach (XmlContents contents in this.m_ContentsList.Values) {
                    contents.SaveFile(writer);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            finally {
                if (writer != null) {
                    writer.Close();
                }
            }
        }
        /// <summary>
        /// ファイル名とパッケージ名を指定してXmlContentsパッケージをファイルから読み込みます。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public void Load(string filename) {
            XmlTextReader reader = new XmlTextReader(filename);
            reader.WhitespaceHandling = WhitespaceHandling.Significant;
            try {
                while (reader.Read()) {
                    if (reader.IsStartElement(PackageRootElement)) {
                        int count = int.Parse(reader.GetAttribute(PackageRootCountAttribute));
                        for (int i = 0; i < count; i++) {
                            //仮に名前空でインスタンスを作成(LoadFile内で自分で設定する。)
                            XmlContents contents = this.CreateXmlContents("");
                            contents.LoadFile(reader);
                            this.AddXmlContents(contents);
                        }
                        break;
                    }
                }
            }
            catch (XmlException ex) {
                System.Diagnostics.Debug.WriteLine(ex.GetType().FullName + "\r\n" + ex.Message);
                throw;
            }
            finally {
                if (reader != null) {
                    reader.Close();
                }
            }
        }
        #endregion

        #region virtualメソッド
        /// <summary>
        /// このXmlContentsPackageのインスタンスで使用するXmlContentsクラスのインスタンスを生成します。
        /// 派生クラスで実装された場合、XmlContentsクラスの派生クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual XmlContents CreateXmlContents(string name) {
            return new XmlContents(name);
        }
        #endregion
    }
    #endregion

    #region XmlContents
    /// <summary>
    /// 設定を表すクラスです。
    /// </summary>
    public class XmlContents {

        #region 定数扱いのプロパティ(派生クラスのみ変更可)
        /// <summary>
        /// シリアル化する際のルートエレメント名を取得します。
        /// </summary>
        public virtual string ContentsRootElement {
            get {
                return "Contents";
            }
        }
        /// <summary>
        /// シリアル化する際のルートエレメントが持つ属性名を取得します。
        /// この属性はこのエレメントの名前を表します。
        /// </summary>
        public virtual string ContentsRootNameAttribute {
            get {
                return "Name";
            }
        }
        /// <summary>
        /// シリアル化する際のルートエレメントが持つ属性名を取得します。
        /// この属性は子エレメントの個数を表します。
        /// </summary>
        public virtual string ContentsRootCountAttribute {
            get {
                return "Count";
            }
        }
        #endregion

        #region メンバ変数、プロパティ
        private string m_Name;
        /// <summary>
        /// XmlContentsオブジェクトの名前を取得します。
        /// </summary>
        public string Name {
            get {
                return this.m_Name;
            }
            set {
                this.m_Name = value;
                this.m_Root.Name = value;
            }
        }
        private ContainerXmlContentsItem m_Root;
        internal ContainerXmlContentsItem RootItem {
            get {
                return this.m_Root;
            }
        }

        private XmlContentsItemProvider m_ItemProvider;
        /// <summary>
        /// このインスタンスで使用するXmlContentsItemProviderを取得します。
        /// </summary>
        public virtual XmlContentsItemProvider ItemProvider {
            get {
                return this.m_ItemProvider;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContents() {
            this.m_Name = "";
            this.m_ItemProvider = this.CreateXmlContentsItemProvider();
            this.m_Root = this.m_ItemProvider.CreateContainerXmlContentsItem("");
        }
        /// <summary>
        /// XmlContentsクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public XmlContents(string name) {
            this.m_Name = name;
            this.m_ItemProvider = this.CreateXmlContentsItemProvider();
            this.m_Root = this.m_ItemProvider.CreateContainerXmlContentsItem(name);
        }
        #endregion


        #region Contains
        /// <summary>
        /// 指定した完全パスを持つ設定項目が存在するかどうかを返します。
        /// </summary>
        /// <param name="CompletePath"></param>
        /// <returns></returns>
        public bool Contains(string CompletePath) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(CompletePath);
            return this.Contains(path);
        }
        /// <summary>
        /// 指定したパXmlContentsPathを持つ設定項目が存在するかどうかを返します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Contains(XmlContentsPath path) {
            string contentsName = path.GetContentsRootPath();
            //rootを除いたパス文字列のリスト
            List<String> list = new List<string>(path.Path.Skip(1));
            return this.Contains(contentsName, list);
        }
        private bool Contains(string root, List<String> path) {
            if (root != "" && this.m_Name != root) {
                return false;
            }
            return this.Contains(this.m_Root, path);
        }

        private bool Contains(ContainerXmlContentsItem parent, List<String> path) {
            //Listに格納されたパスを順にチェック
            if (parent.Items.ContainsKey(path[0])) {
                XmlContentsItem item = parent.Items[path[0]];
                if (path.Count == 1) {
                    //最後までマッチしたらtrue
                    return true;
                }
                else if (item.Type == XmlContentsItemType.Container) {
                    //マッチするたびに削除して下の階層へ降りる
                    path.RemoveAt(0);
                    return this.Contains((ContainerXmlContentsItem)item, path);
                }
            }
            return false;
        }
        #endregion

        #region GetXmlContentsItem
        /// <summary>
        /// パスを指定してXmlContentsItemを取得するインデクサです。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public XmlContentsItem this[string path] {
            get {
                return this.GetXmlContentsItem(path);
            }
        }
        /// <summary>
        /// 完全パスを指定してXmlContentsItemを取得します。
        /// </summary>
        /// <param name="completePath"></param>
        /// <returns></returns>
        public XmlContentsItem GetXmlContentsItem(string completePath) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(completePath);
            return this.GetXmlContentsItem(path);
        }
        /// <summary>
        /// XmlContentsPathを指定してXmlContentsIteを取得します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public XmlContentsItem GetXmlContentsItem(XmlContentsPath path) {
            string contentsName = path.GetContentsRootPath();
            //rootを除いたパス文字列のリスト
            List<String> list = new List<string>(path.Path.Skip(1));
            //list.RemoveAt(0);
            XmlContentsItem item = this.GetXmlContentsItem(contentsName, list);

            #region DEBUG
#if DEBUG
            if (item == null) {
                Debug.WriteLine("Null XmlContents");
                Debug.WriteLine(path);
            }
#endif
            #endregion

            return item;
        }

        private XmlContentsItem GetXmlContentsItem(string root, List<String> path) {
            if (root != "" && this.m_Name != root) {
                return null;
            }
            return this.GetXmlContentsItem(this.m_Root, path);
        }

        private XmlContentsItem GetXmlContentsItem(ContainerXmlContentsItem parent, List<String> path) {
            if (parent.Items.ContainsKey(path[0])) {
                XmlContentsItem item = parent.Items[path[0]];
                if (path.Count == 1) {
                    return item;
                }
                else if (item.Type == XmlContentsItemType.Container) {
                    path.RemoveAt(0);
                    return this.GetXmlContentsItem((ContainerXmlContentsItem)item, path);
                }
            }
            return null;
        }
        #endregion

        #region AddXmlContentsItem
        private string GetItemName(string completePath) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(completePath);
            return path.GetItemName();
        }

        /// <summary>
        /// 名前を指定して、設定にbool値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, bool value) {
            string itemName = this.GetItemName(name);
            BoolXmlContentsItem item = this.m_ItemProvider.CreateBoolXmlContentsItem(itemName, value);
            this.AddXmlContentsItem(name, item);
        }
        /// <summary>
        /// 名前を指定して、設定に文字列を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, string value) {
            string itemName = this.GetItemName(name);
            StringXmlContentsItem item = this.m_ItemProvider.CreateStringXmlContentsItem(itemName, value);
            this.AddXmlContentsItem(name, item);
        }
        /// <summary>
        /// 名前を指定して、設定に整数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, int value) {
            string itemName = this.GetItemName(name);
            IntXmlContentsItem item = this.m_ItemProvider.CreateIntXmlContentsItem(itemName, value);
            this.AddXmlContentsItem(name, item);
        }
        /// <summary>
        /// 名前を指定して、設定に実数値を追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddXmlContentsItem(string name, double value) {
            string itemName = this.GetItemName(name);
            DoubleXmlContentsItem item = this.m_ItemProvider.CreateDoubleXmlContentsItem(itemName, value);
            this.AddXmlContentsItem(name, item);
        }
        /// <summary>
        /// 名前を指定して設定にXmlContentsItemを追加します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void AddXmlContentsItem(string name, XmlContentsItem item) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPathWithoutPackageName(name);
            this.AddXmlContentsItem(path, item);
        }
        /// <summary>
        /// XmlContentsPathを指定して、設定にXmlContentsItemを追加します。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        public void AddXmlContentsItem(XmlContentsPath path, XmlContentsItem item) {
            string contentsName = path.GetContentsRootPath();
            //ContentsNameを除いたPathリストを作成
            List<string> list = new List<string>(path.Path.Skip(1));
            this.AddXmlContentsItem(contentsName, list, item);
        }

        private void AddXmlContentsItem(string root, List<String> path, XmlContentsItem item) {
            if (root != "" && this.m_Name != root) {
                throw new ArgumentException("間違ったパスルートが指定されました。");
            }
            this.AddXmlContentsItem(this.m_Root, path, item);
        }

        private void AddXmlContentsItem(ContainerXmlContentsItem parent, List<String> path, XmlContentsItem item) {
            if (path.Count == 1) {
                //parentは追加すべきアイテムの直の親
                if (!parent.Items.ContainsKey(path[0])) {
                    parent.Items.Add(path[0], item);
                }
                else {
                    parent.Items[path[0]] = item;
                }
                return;
            }
            else {
                ContainerXmlContentsItem nextParent;
                if (!parent.Items.ContainsKey(path[0])) {
                    nextParent = this.m_ItemProvider.CreateContainerXmlContentsItem(path[0]);
                    parent.Items.Add(path[0], nextParent);
                }
                else {
                    nextParent = (ContainerXmlContentsItem)parent.Items[path[0]];
                }
                path.RemoveAt(0);
                this.AddXmlContentsItem(nextParent, path, item);
            }
        }
        #endregion

        #region RemoveXmlContentsItem
        /// <summary>
        /// 完全パスを指定してXmlContentsItemを削除します。
        /// </summary>
        /// <param name="completePath"></param>
        public void RemoveXmlContentsItem(string completePath) {
            XmlContentsPath path = XmlContentsPathProvider.GetXmlContentsPath(completePath);
            string contentsName = path.GetContentsRootPath();
            //rootを除いたパス文字列のリスト
            List<String> list = new List<string>(path.Path.Skip(1));
            this.RemoveXmlContentsItem(contentsName, list);
        }

        private void RemoveXmlContentsItem(string root, List<String> path) {
            //GetXmlContentsItemメソッドはpathリストを削除しながら動作するため、pathの複製を渡す。
            XmlContentsItem item = this.GetXmlContentsItem(root, path.ToList());
            if (item != null) {
                string target = path[path.Count - 1];
                path.RemoveAt(path.Count - 1);
                //GetXmlContentsItemメソッドはpathリストを削除しながら動作するため、pathの複製を渡す。
                item = this.GetXmlContentsItem(root, path.ToList());
                if (item.Type != XmlContentsItemType.Container) {
                    throw new ArgumentException("不正なパスです。");
                }
                ContainerXmlContentsItem parent = (ContainerXmlContentsItem)item;
                parent.Items.Remove(target);
                if (parent.Items.Count == 0) {
                    this.RemoveXmlContentsItem(root, path);
                }
            }
        }
        #endregion

        #region ClearXmlContentsItems
        /// <summary>
        /// 設定値を全て削除します。
        /// </summary>
        public void ClearXmlContentsItems() {
            this.m_Root.Items.Clear();
        }
        #endregion

        #region ファイル入出力
        //NOTE:ファイルフォーマットのバージョンを導入するため、Packageに直接Saveメソッドを持たせる設計は廃止。XmlContentsReader/Writerを使用する。

        internal void SaveFile(XmlTextWriter writer) {
            writer.WriteStartElement(ContentsRootElement);
            writer.WriteAttributeString(ContentsRootNameAttribute, this.Name);
            writer.WriteAttributeString(ContentsRootCountAttribute, this.m_Root.Items.Count.ToString());
            foreach (XmlContentsItem item in this.m_Root.Items.Values) {
                this.m_ItemProvider.Save(writer, item);
            }
            writer.WriteEndElement();
        }
        internal void LoadFile(XmlTextReader reader) {
            while (reader.Read()) {
                if (reader.IsStartElement(ContentsRootElement)) {
                    string name = reader.GetAttribute(ContentsRootNameAttribute);
                    this.m_Name = name;
                    this.m_Root = this.m_ItemProvider.CreateContainerXmlContentsItem(name);
                    int count = int.Parse(reader.GetAttribute(ContentsRootCountAttribute));
                    reader.Read();
                    for (int i = 0; i < count; i++) {
                        XmlContentsItem item = this.m_ItemProvider.Load(reader);

                        this.AddXmlContentsItem(name + '.' + item.Name, item);
                    }
                    break;
                }
            }
        }
        #endregion

        #region virtualメソッド

        /// <summary>
        /// このXmlContentsで使用するXmlContentsItemProviderクラスのインスタンスを生成します。
        /// </summary>
        /// <returns></returns>
        protected virtual XmlContentsItemProvider CreateXmlContentsItemProvider() {
            return new XmlContentsItemProvider();
        }

        #endregion
    }
    #endregion

    #region XmlContentsItemProvider
    /// <summary>
    /// XmlContentsItemの型名解決を仲介するクラスです。
    /// </summary>
    public class XmlContentsItemProvider {

        #region 定数
        /// <summary>
        /// シリアル化する際のルートエレメント名を取得します。
        /// </summary>
        public virtual string ItemRootElement {
            get {
                return "Item";
            }
        }
        /// <summary>
        /// シリアル化する際のルートエレメントが持つ属性名を取得します。
        /// この属性はこのエレメントの名前を表します。
        /// </summary>
        public virtual string ItemRootNameAttribute {
            get {
                return "Name";
            }
        }
        /// <summary>
        /// シリアル化する際のルートエレメントが持つ属性名を取得します。
        /// この属性はこのエレメントの型名を表します。
        /// </summary>
        public virtual string ItemRootTypeAttribute {
            get {
                return "Type";
            }
        }
        #endregion

        #region フィールド(メンバ変数、プロパティ、イベント)
        /// <summary>
        /// XmlContentsItemを生成するメソッドのテーブルを表します。
        /// </summary>
        protected Dictionary<XmlContentsItemType, Func<string, XmlContentsItem>> m_GenerateItemTable;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsItemProviderクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsItemProvider() {
            //型解決テーブルの初期化
            this.m_GenerateItemTable = new Dictionary<XmlContentsItemType, Func<string, XmlContentsItem>>() {
                { XmlContentsItemType.Container,this.CreateContainerXmlContentsItem },
                { XmlContentsItemType.Bool,this.CreateBoolXmlContentsItem },
                { XmlContentsItemType.String,this.CreateStringXmlContentsItem},
                { XmlContentsItemType.Int,this.CreateIntXmlContentsItem},
                { XmlContentsItemType.Double,this.CreateDoubleXmlContentsItem},
            };
        }
        #endregion

        #region ファイル入出力
        //Saveは継承による型解決ができるため実際は不要(Loadとの対称性のために本クラスで実装)
        //Loadは型名を判定し、対応するインスタンスを作成する必要があるため、
        //仲介するクラスとしてXmlContentsItemProviderクラスを使用する。

        /// <summary>
        /// TextWriterにXmlContentsItemを書き込みます。
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="item"></param>
        public void Save(XmlTextWriter writer, XmlContentsItem item) {
            writer.WriteStartElement(ItemRootElement);
            writer.WriteAttributeString(ItemRootNameAttribute, item.Name);
            writer.WriteAttributeString(ItemRootTypeAttribute, item.Type.ToString());
            item.Save(writer);
            writer.WriteEndElement();
        }


        /// <summary>
        /// TextReaderからXmlContentsItemを読み取り、オブジェクトを返します。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public XmlContentsItem Load(XmlTextReader reader) {
            try {
                if (reader.IsStartElement(ItemRootElement)) {
                    string name = reader.GetAttribute(ItemRootNameAttribute);
                    string typeName = reader.GetAttribute(ItemRootTypeAttribute);
                    XmlContentsItemType type = (XmlContentsItemType)Enum.Parse(typeof(XmlContentsItemType), typeName);
                    bool isEmpty = reader.IsEmptyElement;
                    reader.Read();
                    XmlContentsItem item = this.GenerateItem(type, name);
                    if (item != null) {
                        item.Load(reader);
                    }
                    if (!isEmpty) {
                        reader.Read();
                    }
                    return item;
                }
                return null;
            }
            catch (XmlException) {
                throw;
            }
            catch (Exception) {// ex){
                //AppMain.g_AppMain.DebugWrite(ex);
                throw;
            }
        }

        #endregion


        #region virtualメソッド

        #region CreateContainerXmlContentsItem
        /// <summary>
        /// ContainerXmlContentsItemを生成するメソッドです。
        /// 派生クラスで実装する場合、ContainerXmlContentsItemクラスの派生クラスのインスタンスを生成します。
        /// 生成されるインスタンスのItemProviderプロパティにItemProvider自身のインスタンスを設定する必要があります。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ContainerXmlContentsItem CreateContainerXmlContentsItem(string name) {
            return new ContainerXmlContentsItem(name) {
                ItemProvider = this
            };
        }

        #endregion

        #region CreateBoolXmlContentsItem
        /// <summary>
        /// BoolXmlContentsItemを生成するメソッドです。
        /// 派生クラスで実装する場合、BoolXmlContentsItemクラスの派生クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual BoolXmlContentsItem CreateBoolXmlContentsItem(string name, bool value) {
            return new BoolXmlContentsItem(name, value);
        }
        private BoolXmlContentsItem CreateBoolXmlContentsItem(string name) {
            return this.CreateBoolXmlContentsItem(name, false);
        }
        #endregion

        #region CreateStringXmlContentsItem
        /// <summary>
        /// StringXmlContentsItemを生成するメソッドです。
        /// 派生クラスで実装する場合、StringXmlContentsItemクラスの派生クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual StringXmlContentsItem CreateStringXmlContentsItem(string name, string value) {
            return new StringXmlContentsItem(name, value);
        }
        private StringXmlContentsItem CreateStringXmlContentsItem(string name) {
            return this.CreateStringXmlContentsItem(name, "");
        }

        #endregion

        #region CreateIntXmlContentsItem
        /// <summary>
        /// IntXmlContentsItemを生成するメソッドです。
        /// 派生クラスで実装する場合、IntXmlContentsItemクラスの派生クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual IntXmlContentsItem CreateIntXmlContentsItem(string name, int value) {
            return new IntXmlContentsItem(name, value);
        }
        private IntXmlContentsItem CreateIntXmlContentsItem(string name) {
            return this.CreateIntXmlContentsItem(name, 0);
        }

        #endregion

        #region CreateDoubleXmlContentsItem
        /// <summary>
        /// DoubleXmlContentsItemを生成するメソッドです。
        /// 派生クラスで実装する場合、DoubleXmlContentsItemクラスの派生クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual DoubleXmlContentsItem CreateDoubleXmlContentsItem(string name, double value) {
            return new DoubleXmlContentsItem(name, value);
        }
        private DoubleXmlContentsItem CreateDoubleXmlContentsItem(string name) {
            return this.CreateDoubleXmlContentsItem(name, 0.0);
        }

        #endregion

        #endregion

        #region 型解決
        /// <summary>
        /// 型名を示すXmlContentsItemTypeと名前を指定してXmlContentsItemの型名解決を行い、生成したインスタンスを返します。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlContentsItem GenerateItem(XmlContentsItemType type, string name) {
            if (this.m_GenerateItemTable.ContainsKey(type)) {
                return this.m_GenerateItemTable[type](name);
            }
            throw new ArgumentException("不明なXmlContentsItemTypeが指定されました。");
        }
        #endregion

    }
    #endregion

    #region XmlContentsItem
    /// <summary>
    /// 全ての設定項目を表すXmlContentsItemの基本クラスです。
    /// </summary>
    public abstract class XmlContentsItem {

        #region メンバ変数
        private XmlContentsItemType m_Type;
        private string m_Name;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// 型名と名前を指定してXmlContentsItemオブジェクトを初期化します。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        protected XmlContentsItem(XmlContentsItemType type, string name) {
            this.m_Type = type;
            this.m_Name = name;
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 設定項目名を取得します。
        /// </summary>
        public string Name {
            get {
                return this.m_Name;
            }
            set {
                this.m_Name = value;
            }
        }
        /// <summary>
        /// XmlContents値の型名を取得します。
        /// </summary>
        public XmlContentsItemType Type {
            get {
                return this.m_Type;
            }
        }
        /// <summary>
        /// XmlContents値を取得、設定します。
        /// </summary>
        public abstract object Value {
            get;
            set;
        }
        #endregion

        #region ファイル入出力
        /// <summary>
        /// XmlContents値をXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        public void Save(XmlTextWriter writer) {
            this.OnSave(writer);
        }
        /// <summary>
        /// Condfig値をXMLから読み出します。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void Load(XmlTextReader reader) {
            this.OnLoad(reader);
        }
        /// <summary>
        /// 派生クラスでオーバーライドされると、XmlContents値をXMLに書き出す処理を実行します。
        /// </summary>
        /// <param name="writer"></param>
        protected abstract void OnSave(XmlTextWriter writer);
        /// <summary>
        /// 派生クラスでオーバーライドされると、XmlContents値をXMLから読み出す処理を実行します。
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void OnLoad(XmlTextReader reader);
        /// <summary>
        /// ValueプロパティのToString()メソッドで返される結果をXMLに書き込みます。
        /// ValueプロパティのToString()メソッドが、適切なXmlContents値を返す場合、
        /// OnSaveメソッドではWriteDefaultValueメソッドのみを呼び出します。
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteDefaultValue(XmlTextWriter writer) {
            writer.WriteString(this.Value.ToString());
        }
        #endregion
    }
    #endregion

    #region XmlContentsItemType
    /// <summary>
    /// XmlContents値の型名を示します。
    /// </summary>
    public enum XmlContentsItemType {
        /// <summary>
        /// 複数のXmlContentsを内部に格納するアイテムです。
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

    #region XmlContentsItemの派生クラス(基本型)

    #region ContainerXmlContentsItem
    /// <summary>
    /// 他のXmlContentsItemを包含するXmlContentsItemです。
    /// </summary>
    public class ContainerXmlContentsItem : XmlContentsItem {

        #region 定数
        /// <summary>
        /// シリアル化する際の子エレメント名を取得します。
        /// 子エレメントの値はその他の子エレメントの個数を表します。
        /// </summary>
        public virtual string ItemCountElement {
            get {
                return "Count";
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// ContainerXmlContentsItemクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public ContainerXmlContentsItem(string name)
            : base(XmlContentsItemType.Container, name) {
            this.m_Children = new Dictionary<string, XmlContentsItem>();
        }
        #endregion

        #region プロパティ
        private XmlContentsItemProvider m_ItemProvider;
        /// <summary>
        /// このContainerXmlContentsItemクラスで使用するXmlContentsItemProviderクラスのインスタンスを取得または設定します。
        /// </summary>
        public virtual XmlContentsItemProvider ItemProvider {
            get {
                return this.m_ItemProvider;
            }
            set {
                this.m_ItemProvider = value;
            }
        }

        private Dictionary<string, XmlContentsItem> m_Children;
        /// <summary>
        /// 保持するXmlContentsItemのハッシュテーブルを取得します。
        /// </summary>
        public Dictionary<string, XmlContentsItem> Items {
            get {
                return this.m_Children;
            }
        }
        /// <summary>
        /// 保持するXmlContentsItemの個数を取得します。
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
        /// ContainerXmlContentsItemオブジェクトをXMLに書き込みます。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            writer.WriteElementString(ItemCountElement, this.m_Children.Count.ToString());
            foreach (XmlContentsItem item in this.m_Children.Values) {
                this.m_ItemProvider.Save(writer, item);
            }
        }
        /// <summary>
        /// ContainerXmlContentsItemオブジェクトをXMLから読み出します。
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            try {
                reader.ReadStartElement(ItemCountElement);
                int count = int.Parse(reader.ReadString());
                reader.Read();
                for (int i = 0; i < count; i++) {
                    XmlContentsItem item = this.m_ItemProvider.Load(reader);
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

    #region BoolXmlContentsItem
    /// <summary>
    /// Bool値を持つXmlContentsItemです。
    /// </summary>
    public class BoolXmlContentsItem : XmlContentsItem {

        #region コンストラクタ
        /// <summary>
        /// BoolXmlContentsItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public BoolXmlContentsItem(string name)
            : this(name, true) {
        }
        /// <summary>
        /// BoolXmlContentsItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public BoolXmlContentsItem(string name, bool value)
            : base(XmlContentsItemType.Bool, name) {
            this.m_Value = value;
        }
        #endregion

        #region Value
        private bool m_Value;
        /// <summary>
        /// BoolXmlContentsItemオブジェクトの値を取得または設定します。
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
        /// BoolXmlContentsItemオブジェクトをXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// BoolXmlContentsItemオブジェクトをXMLから読み出します。
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            if (reader.HasValue) {
                this.m_Value = bool.Parse(reader.ReadString());
            }
        }
        #endregion

    }
    #endregion

    #region StringXmlContentsItem
    /// <summary>
    /// 文字列を持つXmlContentsItemです。
    /// </summary>
    public class StringXmlContentsItem : XmlContentsItem {

        #region コンストラクタ
        /// <summary>
        /// StringXmlContentsItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public StringXmlContentsItem(string name)
            : this(name, "") {
        }
        /// <summary>
        /// StringXmlContentsItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public StringXmlContentsItem(string name, string value)
            : base(XmlContentsItemType.String, name) {
            this.m_Value = value;
        }
        #endregion

        #region Value
        private string m_Value;
        /// <summary>
        /// StringXmlContentsItemオブジェクトの値を取得または設定します。
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
        /// StringXmlContentsItemオブジェクトをXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            //nullはシリアル化時にから文字列に変換する
            if (this.m_Value == null) {
                this.m_Value = "";
            }
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// StringXmlContentsItemオブジェクトをXMLから読み出します。
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            if (reader.HasValue) {
                this.m_Value = reader.ReadString();
            }
        }
        #endregion

    }
    #endregion

    #region IntXmlContentsItem
    /// <summary>
    /// 整数値を持つXmlContentsItemです。
    /// </summary>
    public class IntXmlContentsItem : XmlContentsItem {

        #region コンストラクタ
        /// <summary>
        /// IntXmlContentsItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public IntXmlContentsItem(string name)
            : this(name, 0) {
        }
        /// <summary>
        /// IntXmlContentsItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public IntXmlContentsItem(string name, int value)
            : base(XmlContentsItemType.Int, name) {
            this.m_Value = value;
        }
        #endregion

        #region Value
        private int m_Value;
        /// <summary>
        /// IntXmlContentsItemオブジェクトの値を取得または設定します。
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
        /// IntXmlContentsItemオブジェクトをXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// IntXmlContentsItemオブジェクトをXMLから読み出します。
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            if (reader.HasValue) {
                this.m_Value = int.Parse(reader.ReadString());
            }
        }
        #endregion

    }
    #endregion

    #region DoubleXmlContentsItem
    /// <summary>
    /// 実数値を持つXmlContentsItemです。
    /// </summary>
    public class DoubleXmlContentsItem : XmlContentsItem {

        #region コンストラクタ
        /// <summary>
        /// DoubleXmlContentsItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public DoubleXmlContentsItem(string name)
            : this(name, 0.0) {
        }
        /// <summary>
        /// DoubleXmlContentsItemオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public DoubleXmlContentsItem(string name, double value)
            : base(XmlContentsItemType.Double, name) {
            this.m_Value = value;
        }
        #endregion

        #region Value
        private double m_Value;
        /// <summary>
        /// DoubleXmlContentsItemオブジェクトの値を取得または設定します。
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
        /// DoubleXmlContentsItemオブジェクトをXMLに書き出します。
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnSave(XmlTextWriter writer) {
            this.WriteDefaultValue(writer);
        }
        /// <summary>
        /// DoubleXmlContentsItemオブジェクトをXMLから読み出します。
        /// </summary>
        /// <param name="reader"></param>
        protected override void OnLoad(XmlTextReader reader) {
            if (reader.HasValue) {
                this.m_Value = double.Parse(reader.ReadString());
            }
        }
        #endregion

    }
    #endregion

    #endregion

}
