using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
namespace Kucl.Xml {

    #region XmlContentsReader
    /// <summary>
    /// XmlContentsのReaderの基本クラスです。
    /// </summary>
    public abstract class XmlContentsReader {

        #region フィールド(メンバ変数、プロパティ、イベント)
        private Dictionary<XmlContentsItemType, Func<XmlContentsItemReader>> m_CreateItemReaderTable;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsReaderオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        protected XmlContentsReader() {
            this.m_CreateItemReaderTable = new Dictionary<XmlContentsItemType, Func<XmlContentsItemReader>>() {
                { XmlContentsItemType.Container ,this.CreateContainerItemReader},
                { XmlContentsItemType.Bool      ,this.CreateBoolItemReader},
                { XmlContentsItemType.String    ,this.CreateStringItemReader},
                { XmlContentsItemType.Int       ,this.CreateIntItemReader},
                { XmlContentsItemType.Double    ,this.CreateDoubleItemReader},
            };
        }
        #endregion

        /// <summary>
        /// 派生クラスでオーバーライドされると、パッケージの読み込みを行うメソッドを定義します。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract XmlContentsPackage LoadPackage(  XmlContentsPackageReadInfo info);
        /// <summary>
        /// 派生クラスでオーバーライドされると、XmlContentsの読み込みを行うメソッドを定義します。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract XmlContents LoadContents(  XmlContentsReadInfo info);
        /// <summary>
        /// 派生クラスでオーバーライドされると、XmlContentsItemの読み込みを行うメソッドを定義します。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract XmlContentsItem LoadContentsItem(  XmlContentsItemReadInfo info);


        #region protected XmlContentsItemReader
        /// <summary>
        /// 個別のアイテムを読み込むクラスです。
        /// </summary>
        protected abstract class XmlContentsItemReader {
            /// <summary>
            /// このXmlContentsItemReaderを保持するXmlContentsReaderへの参照を取得します。
            /// </summary>
            protected XmlContentsReader ParentReader {
                get;
            }

            /// <summary>
            /// このXmlContentsItemReaderが読み取るContentsItemが属性を持つかどうかを取得します。
            /// 既定ではFalseを返します。Trueを返す必要がある場合、派生クラスのコンストラクタで値を設定します。
            /// </summary>
            public bool HaveAttribute {
                get;
                protected set;
            }
            /// <summary>
            /// XmlContentsItemReaderオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentReader"></param>
            protected XmlContentsItemReader(XmlContentsReader parentReader) {
                this.HaveAttribute = false;
                this.ParentReader = parentReader;
            }
            /// <summary>
            /// 指定したXmlContentsItemの読み込みを行います。
            /// </summary>
            /// <param name="item"></param>
            /// <param name="info"></param>
            public abstract void LoadContentsItem(XmlContentsItem item, XmlContentsItemReadInfo info);
        } 
        #endregion

        /// <summary>
        /// 指定したTypeに対応するXmlContentsItemReaderを返します。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected XmlContentsItemReader CreateItemReader(XmlContentsItemType type) {
            return this.m_CreateItemReaderTable[type]();
        }

        /// <summary>
        /// ContainerItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemReader CreateContainerItemReader();
        /// <summary>
        /// BoolItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemReader CreateBoolItemReader();
        /// <summary>
        /// StringItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemReader CreateStringItemReader();
        /// <summary>
        /// IntItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemReader CreateIntItemReader();
        /// <summary>
        /// DoubleItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemReader CreateDoubleItemReader();
    }
    #endregion


    #region XmlContentsPackageReadInfo 
    /// <summary>
    /// XmlContentsPackageを読み込むために必要な情報を表します。
    /// </summary>
    public class XmlContentsPackageReadInfo {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #region FileName
        private string m_FileName;
        /// <summary>
        /// FileNameを取得、設定します。
        /// </summary>
        public string FileName {
            get {
                return this.m_FileName;
            }
            set {
                this.m_FileName = value;
            }
        }
        #endregion

        #region PackageName
        private string m_PackageName;
        /// <summary>
        /// PackageNameを取得、設定します。
        /// </summary>
        public string PackageName {
            get {
                return this.m_PackageName;
            }
            set {
                this.m_PackageName = value;
            }
        }
        #endregion

        #region Owner
        private XmlContentsModel m_Owner;
        /// <summary>
        /// Ownerを取得、設定します。
        /// </summary>
        public XmlContentsModel Owner {
            get {
                return this.m_Owner;
            }
            set {
                this.m_Owner = value;
            }
        }
        #endregion

        #region PreLoadedPackage
        /// <summary>
        /// Version判定のために事前に読み込まれたPackageを表します。
        /// </summary>
        public XmlContentsPackage PreLoadedPackage {
            get;
            set;
        }
        #endregion

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsPackageReadInfoオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsPackageReadInfo() {
        }
        #endregion

    }
    #endregion

    #region XmlContentsReadInfo
    /// <summary>
    /// XmlContentsを読み込むために必要な情報を表します。
    /// </summary>
    public class XmlContentsReadInfo {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #region Reader
        private XmlTextReader m_Reader;
        /// <summary>
        /// Readerを取得、設定します。
        /// </summary>
        public XmlTextReader Reader {
            get {
                return this.m_Reader;
            }
            set {
                this.m_Reader = value;
            }
        }
        #endregion

        #region Package
        private XmlContentsPackage m_Package;
        /// <summary>
        /// Packageを取得、設定します。
        /// </summary>
        public XmlContentsPackage Package {
            get {
                return this.m_Package;
            }
            set {
                this.m_Package = value;
            }
        }
        #endregion

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsReadInfoオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsReadInfo() {
        }
        #endregion
        
    }
    #endregion

    #region XmlContentsItemReadInfo
    /// <summary>
    /// XmlContentsItemを読み込むために必要な情報を表します。
    /// </summary>
    public class XmlContentsItemReadInfo {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #region Reader
        private XmlTextReader m_Reader;
        /// <summary>
        /// Readerを取得、設定します。
        /// </summary>
        public XmlTextReader Reader {
            get {
                return this.m_Reader;
            }
            set {
                this.m_Reader = value;
            }
        }
        #endregion

        #region ItemProvider
        private XmlContentsItemProvider m_ItemProvider;
        /// <summary>
        /// ItemProviderを取得、設定します。
        /// </summary>
        public XmlContentsItemProvider ItemProvider {
            get {
                return this.m_ItemProvider;
            }
            set {
                this.m_ItemProvider = value;
            }
        }
        #endregion

        #region Contents
        private XmlContents m_Contents;
        /// <summary>
        /// Contentsを取得、設定します。
        /// </summary>
        public XmlContents Contents {
            get {
                return this.m_Contents;
            }
            set {
                this.m_Contents = value;
            }
        }
        #endregion




        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsItemReadInfoオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsItemReadInfo() {
        }
        #endregion

        
    }
    #endregion



    #region PackageVersionReader
    /// <summary>
    /// PackageのVersionを先読みするためのクラスです。
    /// </summary>
    public class PackageVersionReader {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #endregion

        #region コンストラクタ
        /// <summary>
        /// PackageVersionReaderオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public PackageVersionReader() {
        }
        #endregion

        /// <summary>
        /// PackageのVersionを読み込みます。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="package"></param>
        /// <returns></returns>
        public string ReadVersion(XmlContentsPackageReadInfo info,out XmlContentsPackage package) {
            package = info.Owner.CreateXmlContentsPackage(info.PackageName);
            string filename = info.FileName;
            XmlTextReader reader = new XmlTextReader(filename);
            reader.WhitespaceHandling = WhitespaceHandling.Significant;
            try {
                while (reader.Read()) {
                    if (reader.IsStartElement(package.PackageRootElement)) {
                        string version = reader.GetAttribute(package.PackageRootVersionAttribute);
                        return version ?? "";
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
            return "";
        }

    }
    #endregion

    #region XmlContentsReaderFactory
    /// <summary>
    /// XmlContentsReaderを生成するファクトリメソッドを提供するクラスです。
    /// </summary>
    public static class XmlContentsReaderFactory {

        /// <summary>
        /// XmlContentsReaderを生成します。
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static XmlContentsReader Create(string typeName, string version) {
            switch (typeName) {
                case "":
                    switch (version) {
                        case "":
                        case "0.0":
                            return new XmlContentsReader_00();
                        case "1.0":
                            return new XmlContentsReader_01();
                    }
                    break;
            }
            throw new ArgumentException("不明なtypeNameとversionが指定されました");
        }
    }
    #endregion


    #region XmlContentsReader_00
    /// <summary>
    /// 既定のXmlContentsReaderを表します。
    /// </summary>
    public class XmlContentsReader_00 : XmlContentsReader {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsReader_00オブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsReader_00() : base() {
        }
        #endregion

        #region LoadPackage
        /// <summary>
        /// XmlContentsPackageを読み込みます。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override XmlContentsPackage LoadPackage(XmlContentsPackageReadInfo info) {
            //XmlContentsPackage package = info.Owner.CreateXmlContentsPackage(info.PackageName);
            //Version判定のために先読みしたPackageを使用する
            XmlContentsPackage package = info.PreLoadedPackage;
            string filename = info.FileName;
            XmlTextReader reader = new XmlTextReader(filename);
            reader.WhitespaceHandling = WhitespaceHandling.Significant;
            try {
                while (reader.Read()) {
                    if (reader.IsStartElement(package.PackageRootElement)) {
                        int count = int.Parse(reader.GetAttribute(package.PackageRootCountAttribute));
                        for (int i = 0; i < count; i++) {
                            XmlContentsReadInfo contentsInfo = new XmlContentsReadInfo() {
                                Reader = reader,
                                Package = package
                            };
                            XmlContents contents = this.LoadContents(contentsInfo);
                            package.AddXmlContents(contents);
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
            return package;
        }
        #endregion

        #region LoadContents
        /// <summary>
        /// XmlContentsを読み込みます。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override XmlContents LoadContents(XmlContentsReadInfo info) {
            XmlContents contents = info.Package.CreateXmlContents("");
            XmlTextReader reader = info.Reader;
            while (reader.Read()) {
                if (reader.IsStartElement(contents.ContentsRootElement)) {
                    string name = reader.GetAttribute(contents.ContentsRootNameAttribute);
                    contents.Name = name;
                    int count = int.Parse(reader.GetAttribute(contents.ContentsRootCountAttribute));
                    reader.Read();
                    for (int i = 0; i < count; i++) {
                        XmlContentsItemReadInfo itemInfo = new XmlContentsItemReadInfo() {
                            Reader = reader,
                            ItemProvider = contents.ItemProvider,
                            Contents = contents
                        };
                        XmlContentsItem item = this.LoadContentsItem(itemInfo);

                        contents.AddXmlContentsItem(name + '.' + item.Name, item);
                    }
                    break;
                }
            }
            return contents;
        }
        #endregion

        #region LoadContentsItem
        /// <summary>
        /// XmlContentsItemを読み込みます。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override XmlContentsItem LoadContentsItem(XmlContentsItemReadInfo info) {
            XmlTextReader reader = info.Reader;
            XmlContentsItemProvider provider = info.ItemProvider;
            try {
                if (reader.IsStartElement(provider.ItemRootElement)) {
                    string name = reader.GetAttribute(provider.ItemRootNameAttribute);
                    string typeName = reader.GetAttribute(provider.ItemRootTypeAttribute);
                    XmlContentsItemType type = (XmlContentsItemType)Enum.Parse(typeof(XmlContentsItemType), typeName);
                    bool isEmpty = reader.IsEmptyElement;
                    XmlContentsItem item = provider.GenerateItem(type, name);
                    if (item != null) {
                        XmlContentsItemReader itemReader = this.CreateItemReader(type);

                        if (!itemReader.HaveAttribute) {
                            //読み取るItemが属性を持っていない場合、Readerを読み進める。
                            //属性を持っている場合、itemReader.LoadContentsItemメソッド内で属性値を読み取った後にReaderを読み進める。
                            reader.Read();
                        }

                        itemReader.LoadContentsItem(item, info);
                        //item.Load(reader);
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

        #region XmlContentsItemReaderの継承とインスタンス生成
        /// <summary>
        /// ContainerItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemReader CreateContainerItemReader() {
            return new ContainerXmlContentsItemReader(this);
        }
        /// <summary>
        /// BoolItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemReader CreateBoolItemReader() {
            return new BoolXmlContentsItemReader(this);
        }
        /// <summary>
        /// StringItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemReader CreateStringItemReader() {
            return new StringXmlContentsItemReader(this);
        }
        /// <summary>
        /// IntItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemReader CreateIntItemReader() {
            return new IntXmlContentsItemReader(this);
        }
        /// <summary>
        /// DoubleItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemReader CreateDoubleItemReader() {
            return new DoubleXmlContentsItemReader(this);
        }

        #region ContainerXmlContentsItemReaderクラス
        /// <summary>
        /// ContainerXmlContentsItemを読み込むリーダーを表すクラスです。
        /// </summary>
        protected class ContainerXmlContentsItemReader : XmlContentsItemReader {
            /// <summary>
            /// ContainerXmlContentsItemReaderオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentReader"></param>
            public ContainerXmlContentsItemReader(XmlContentsReader parentReader) : base(parentReader) {
            }
            /// <summary>
            /// ContentsItemを読み込みます。
            /// </summary>
            /// <param name="item"></param>
            /// <param name="info"></param>
            public override void LoadContentsItem(XmlContentsItem item, XmlContentsItemReadInfo info) {
                XmlTextReader reader = info.Reader;
                ContainerXmlContentsItem container = (ContainerXmlContentsItem)item;
                try {
                    reader.ReadStartElement(container.ItemCountElement);
                    int count = int.Parse(reader.ReadString());
                    reader.Read();
                    for (int i = 0; i < count; i++) {
                        //XmlContentsItem item1 = container.ItemProvider.Load(reader);
                        XmlContentsItem item1 = this.ParentReader.LoadContentsItem(info);
                        container.Items.Add(item1.Name, item1);
                    }
                }
                catch (XmlException) {// ex){
                                      //AppMain.g_AppMain.DebugWrite(ex);
                    throw;
                }
            }
        }
        #endregion

        #region BoolXmlContentsItemReaderクラス
        /// <summary>
        /// BoolXmlContentsItemを読み込むリーダーを表すクラスです。
        /// </summary>
        protected class BoolXmlContentsItemReader : XmlContentsItemReader {
            /// <summary>
            /// BoolXmlContentsItemReaderオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentReader"></param>
            public BoolXmlContentsItemReader(XmlContentsReader parentReader) : base(parentReader) {
            }
            /// <summary>
            /// ContentsItemを読み込みます。
            /// </summary>
            /// <param name="item"></param>
            /// <param name="info"></param>
            public override void LoadContentsItem(XmlContentsItem item, XmlContentsItemReadInfo info) {
                XmlTextReader reader = info.Reader;
                if (reader.HasValue) {
                    item.Value = bool.Parse(reader.ReadString());
                }
            }
        }
        #endregion

        #region StringXmlContentsItemReaderkクラス
        /// <summary>
        /// StringXmlContentsItemを読み込むリーダーを表すクラスです。
        /// </summary>
        protected class StringXmlContentsItemReader : XmlContentsItemReader {
            /// <summary>
            /// StringXmlContentsItemReaderオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentReader"></param>
            public StringXmlContentsItemReader(XmlContentsReader parentReader) : base(parentReader) {
            }
            /// <summary>
            /// ContentsItemを読み込みます。
            /// </summary>
            /// <param name="item"></param>
            /// <param name="info"></param>
            public override void LoadContentsItem(XmlContentsItem item, XmlContentsItemReadInfo info) {
                XmlTextReader reader = info.Reader;
                if (reader.HasValue) {
                    item.Value = reader.ReadString();
                }
            }
        }
        #endregion

        #region IntXmlContentsItemReaderクラス
        /// <summary>
        /// IntXmlContentsItemを読み込むリーダーを表すクラスです。
        /// </summary>
        protected class IntXmlContentsItemReader : XmlContentsItemReader {
            /// <summary>
            /// IntXmlContentsItemReaderオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentReader"></param>
            public IntXmlContentsItemReader(XmlContentsReader parentReader) : base(parentReader) {
            }
            /// <summary>
            /// ContentsItemを読み込みます。
            /// </summary>
            /// <param name="item"></param>
            /// <param name="info"></param>
            public override void LoadContentsItem(XmlContentsItem item, XmlContentsItemReadInfo info) {
                XmlTextReader reader = info.Reader;
                if (reader.HasValue) {
                    item.Value = int.Parse(reader.ReadString());
                }
            }
        }
        #endregion

        #region DoubleXmlContentsItemReaderクラス
        /// <summary>
        /// DoubleXmlContentsItemを読み込むリーダーを表すクラスです。
        /// </summary>
        protected class DoubleXmlContentsItemReader : XmlContentsItemReader {
            /// <summary>
            /// DoubleXmlContentsItemReaderオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentReader"></param>
            public DoubleXmlContentsItemReader(XmlContentsReader parentReader) : base(parentReader) {
            }
            /// <summary>
            /// ContentsItemを読み込みます。
            /// </summary>
            /// <param name="item"></param>
            /// <param name="info"></param>
            public override void LoadContentsItem(XmlContentsItem item, XmlContentsItemReadInfo info) {
                XmlTextReader reader = info.Reader;
                if (reader.HasValue) {
                    item.Value = double.Parse(reader.ReadString());
                }
            }
        } 
        #endregion

        #endregion

    }
    #endregion

    #region XmlContentsReader_01
    /// <summary>
    /// XmlContentsReader Version01を表します。
    /// </summary>
    public class XmlContentsReader_01 : XmlContentsReader_00 {

        //*******************************************************
        // 変更点
        //
        // Ver1はContainerの子Elementの個数の保持方法の変更。
        // Count要素⇒Count属性とする。
        //
        //*******************************************************

        /// <summary>
        /// ContainerItemReaderの生成メソッドを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemReader CreateContainerItemReader() {
            return new ContainerXmlContentsItemReader_01(this);
        }

        /// <summary>
        /// ContainerXmlContentsItemReaderのVersion1を表します。
        /// </summary>
        protected class ContainerXmlContentsItemReader_01 : ContainerXmlContentsItemReader {

            /// <summary>
            /// ContainerXmlContentsItemReader_01オブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentReader"></param>
            public ContainerXmlContentsItemReader_01(XmlContentsReader parentReader) : base(parentReader) {
                this.HaveAttribute = true;
            }

            /// <summary>
            /// ContentsItemを読み込みます。
            /// </summary>
            /// <param name="item"></param>
            /// <param name="info"></param>
            public override void LoadContentsItem(XmlContentsItem item, XmlContentsItemReadInfo info) {
                XmlTextReader reader = info.Reader;
                ContainerXmlContentsItem container = (ContainerXmlContentsItem)item;
                try {
                    //reader.ReadStartElement(container.ItemCountElement);
                    //int count = int.Parse(reader.ReadString());
                    //reader.Read();
                    //NOTE:HaveAttribute=Trueによりreader.Readが必要になったが、Count要素が無くなったため、reader.Readが不要となり、相殺された。
                    int count = int.Parse(reader.GetAttribute(container.ItemCountElement));
                    reader.Read();
                    for (int i = 0; i < count; i++) {
                        //XmlContentsItem item1 = container.ItemProvider.Load(reader);
                        XmlContentsItem item1 = this.ParentReader.LoadContentsItem(info);
                        container.Items.Add(item1.Name, item1);
                    }
                }
                catch (XmlException) {// ex){
                                      //AppMain.g_AppMain.DebugWrite(ex);
                    throw;
                }
            }
        }
    }

    #endregion
}
