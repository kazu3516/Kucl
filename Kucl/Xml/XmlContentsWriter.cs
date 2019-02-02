using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace Kucl.Xml {

    #region XmlContentsWriter
    /// <summary>
    /// XmlContentsのWriterの基本クラスです。
    /// </summary>
    public abstract class XmlContentsWriter {


        #region フィールド(メンバ変数、プロパティ、イベント)
        private Dictionary<XmlContentsItemType, Func<XmlContentsItemWriter>> m_CreateItemWriterTable;

        /// <summary>
        /// このXmlContentsWriterのバージョンを取得します。
        /// </summary>
        protected virtual string Version {
            get {
                return "";
            }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsWriterオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        protected XmlContentsWriter() {
            this.m_CreateItemWriterTable = new Dictionary<XmlContentsItemType, Func<XmlContentsItemWriter>>() {
                { XmlContentsItemType.Container ,this.CreateContainerItemWriter},
                { XmlContentsItemType.Bool      ,this.CreateBoolItemWriter},
                { XmlContentsItemType.String    ,this.CreateStringItemWriter},
                { XmlContentsItemType.Int       ,this.CreateIntItemWriter},
                { XmlContentsItemType.Double    ,this.CreateDoubleItemWriter},
            };
        }
        #endregion

        /// <summary>
        /// 派生クラスでオーバーライドされると、Packageの保存を行うメソッドを定義します。
        /// </summary>
        /// <param name="info"></param>
        public abstract void SavePackage(XmlContentsPackageWriteInfo info);
        /// <summary>
        /// 派生クラスでオーバーライドされると、Contentsの保存を行うメソッドを定義します。
        /// </summary>
        /// <param name="info"></param>
        public abstract void SaveContents(XmlContentsWriteInfo info);
        /// <summary>
        /// 派生クラスでオーバーライドされると、ContentsItemの保存を行うメソッドを定義します。
        /// </summary>
        /// <param name="info"></param>
        public abstract void SaveContentsItem(XmlContentsItemWriteInfo info);

        /// <summary>
        /// XmlContentsItemの保存を行うクラスです。
        /// </summary>
        protected abstract class XmlContentsItemWriter {
            /// <summary>
            /// このWriterの親となるXmlContentsWriterを取得します。
            /// </summary>
            protected XmlContentsWriter ParentWriter {
                get;
            }
            /// <summary>
            /// XmlContentsItemWriterオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentWriter"></param>
            protected XmlContentsItemWriter(XmlContentsWriter parentWriter) {
                this.ParentWriter = parentWriter;
            }
            /// <summary>
            /// 派生クラスでオーバーライドされると、ContentsItemの保存を行うメソッドを定義します。
            /// </summary>
            /// <param name="info"></param>
            public abstract void SaveContentsItem( XmlContentsItemWriteInfo info);
        }

        /// <summary>
        /// 指定したタイプに対応するXmlContentsItemWriterを返します。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected XmlContentsItemWriter CreateItemWriter(XmlContentsItemType type) {
            return this.m_CreateItemWriterTable[type]();
        }
        /// <summary>
        /// ContainerItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemWriter CreateContainerItemWriter();
        /// <summary>
        /// BoolItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemWriter CreateBoolItemWriter();
        /// <summary>
        /// StringItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemWriter CreateStringItemWriter();
        /// <summary>
        /// IntItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemWriter CreateIntItemWriter();
        /// <summary>
        /// DoubleItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected abstract XmlContentsItemWriter CreateDoubleItemWriter();

    }
    #endregion


    #region XmlContentsPackageWriteInfo
    /// <summary>
    /// XmlContentsPackageの書き込み情報を表すクラスです。
    /// </summary>
    public class XmlContentsPackageWriteInfo {

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
        /// XmlContentsPackageWriteInfoオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsPackageWriteInfo() {
        }
        #endregion

    }
    #endregion

    #region XmlContentsWriteInfo
    /// <summary>
    /// XmlContentsの書き込み情報を表すクラスです。
    /// </summary>
    public class XmlContentsWriteInfo {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #region Writer
        private XmlTextWriter m_Writer;
        /// <summary>
        /// Writerを取得、設定します。
        /// </summary>
        public XmlTextWriter Writer {
            get {
                return this.m_Writer;
            }
            set {
                this.m_Writer = value;
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
        /// XmlContentsWriteInfoオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsWriteInfo() {
        }
        #endregion

    }
    #endregion

    #region XmlContentsItemWriteInfo
    /// <summary>
    /// XmlContentsItemの書き込み情報を表すクラスです。
    /// </summary>
    public class XmlContentsItemWriteInfo {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #region Writer
        private XmlTextWriter m_Writer;
        /// <summary>
        /// Writerを取得、設定します。
        /// </summary>
        public XmlTextWriter Writer {
            get {
                return this.m_Writer;
            }
            set {
                this.m_Writer = value;
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

        #region Item
        private XmlContentsItem m_Item;
        /// <summary>
        /// Itemを取得、設定します。
        /// </summary>
        public XmlContentsItem Item {
            get {
                return this.m_Item;
            }
            set {
                this.m_Item = value;
            }
        }
        #endregion

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsItemWriteInfoオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsItemWriteInfo() {
        }
        #endregion

    }
    #endregion


    #region XmlContentsWriterFactory
    /// <summary>
    /// XmlContentsWriterを生成するファクトリメソッドを提供するクラスです。
    /// </summary>
    public static class XmlContentsWriterFactory {

        /// <summary>
        /// XmlContentsWriterを生成します。
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static XmlContentsWriter Create(string typeName, string version) {
            switch (typeName) {
                case "":
                    switch (version) {
                        case "":
                        case "0.0":
                            return new XmlContentsWriter_00();
                        case "1.0":
                            return new XmlContentsWriter_01();
                    }
                    break;
            }
            throw new ArgumentException("不明なtypeNameとversionが指定されました");
        }
    }
    #endregion


    #region XmlContentsWriter_00
    /// <summary>
    /// XmlContentsWriterのVersion0を表すクラスです。
    /// </summary>
    public class XmlContentsWriter_00 : XmlContentsWriter {

        #region フィールド(メンバ変数、プロパティ、イベント)
        /// <summary>
        /// Package書き込みバージョンを表します。
        /// このプロパティをオーバーライドすることでバージョンの切り替えを行います。
        /// </summary>
        protected override string Version => "0.0";

        #endregion

        #region コンストラクタ
        /// <summary>
        /// XmlContentsWriter_00オブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsWriter_00() {
        }
        #endregion

        #region SavePackage
        /// <summary>
        /// Packageを保存します。
        /// </summary>
        /// <param name="info"></param>
        public override void SavePackage(XmlContentsPackageWriteInfo info) {
            string filename = info.FileName;
            XmlContentsPackage package = info.Package;
            XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8) {
                Formatting = Formatting.Indented,
                Indentation = 4
            };
            try {
                writer.WriteStartDocument();
                writer.WriteStartElement(package.PackageRootElement);
                writer.WriteAttributeString(package.PackageRootCountAttribute, package.XmlContentsTable.Count.ToString());
                writer.WriteAttributeString(package.PackageRootVersionAttribute, this.Version);
                foreach (XmlContents contents in package.XmlContentsTable.Values) {
                    XmlContentsWriteInfo contentsInfo = new XmlContentsWriteInfo() {
                        Writer = writer,
                        Contents = contents
                    };
                    this.SaveContents(contentsInfo);
                    //contents.SaveFile(writer);
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
        #endregion

        #region SaveContents
        /// <summary>
        /// Contentsを保存します。
        /// </summary>
        /// <param name="info"></param>
        public override void SaveContents(XmlContentsWriteInfo info) {
            XmlTextWriter writer = info.Writer;
            XmlContents contents = info.Contents;
            writer.WriteStartElement(contents.ContentsRootElement);
            writer.WriteAttributeString(contents.ContentsRootNameAttribute, contents.Name);
            writer.WriteAttributeString(contents.ContentsRootCountAttribute, contents.RootItem.Items.Count.ToString());
            foreach (XmlContentsItem item in contents.RootItem.Items.Values) {
                XmlContentsItemWriteInfo itemInfo = new XmlContentsItemWriteInfo() {
                    Writer = writer,
                    ItemProvider = contents.ItemProvider,
                    Item = item
                };
                this.SaveContentsItem(itemInfo);
                //this.m_ItemProvider.Save(writer, item);
            }
            writer.WriteEndElement();
        }
        #endregion

        #region SaveContentsItem
        /// <summary>
        /// ContentsItemを保存します。
        /// </summary>
        /// <param name="info"></param>
        public override void SaveContentsItem(XmlContentsItemWriteInfo info) {
            XmlTextWriter writer = info.Writer;
            XmlContentsItemProvider provider = info.ItemProvider;
            XmlContentsItem item = info.Item;

            writer.WriteStartElement(provider.ItemRootElement);
            writer.WriteAttributeString(provider.ItemRootNameAttribute, item.Name);
            writer.WriteAttributeString(provider.ItemRootTypeAttribute, item.Type.ToString());
            //TODO:ItemのSave
            //item.Save(writer);
            XmlContentsItemWriter itemWriter = this.CreateItemWriter(item.Type);
            itemWriter.SaveContentsItem(info);


            writer.WriteEndElement();
        }
        #endregion

        #region XmlContentsItemWriterの継承とインスタンス生成

        /// <summary>
        /// ContainerItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemWriter CreateContainerItemWriter() {
            return new ContainerXmlContentsItemWriter(this);
        }

        /// <summary>
        /// BoolItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemWriter CreateBoolItemWriter() {
            return new BoolXmlContentsItemWriter(this);
        }

        /// <summary>
        /// StringItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemWriter CreateStringItemWriter() {
            return new StringXmlContentsItemWriter(this);
        }

        /// <summary>
        /// IntItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemWriter CreateIntItemWriter() {
            return new IntXmlContentsItemWriter(this);
        }

        /// <summary>
        /// DoubleItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemWriter CreateDoubleItemWriter() {
            return new DoubleXmlContentsItemWriter(this);
        }

        #region ContainerXmlContentsItemWriterクラス
        /// <summary>
        /// ContainerXmlContentsItemの保存を行うクラスです。
        /// </summary>
        protected class ContainerXmlContentsItemWriter : XmlContentsItemWriter {
            /// <summary>
            /// ContainerXmlContentsItemWriterオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentWriter"></param>
            public ContainerXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            /// <summary>
            /// ContentsItemの保存を行います。
            /// </summary>
            /// <param name="info"></param>
            public override void SaveContentsItem(XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                ContainerXmlContentsItem container = (ContainerXmlContentsItem)item;
                writer.WriteElementString(container.ItemCountElement, container.Items.Count.ToString());
                foreach (XmlContentsItem item1 in container.Items.Values) {
                    XmlContentsItemWriteInfo itemInfo = new XmlContentsItemWriteInfo() {
                        Writer = info.Writer,
                        ItemProvider = info.ItemProvider,
                        Item = item1
                    };
                    this.ParentWriter.SaveContentsItem(itemInfo);
                    //this.m_ItemProvider.Save(writer, item1);
                }
            }
        }
        #endregion

        #region BoolXmlContentsItemWriterクラス
        /// <summary>
        /// BoolXmlContentsItemの保存を行うクラスです。
        /// </summary>
        protected class BoolXmlContentsItemWriter : XmlContentsItemWriter {
            /// <summary>
            /// BoolXmlContentsItemWriterオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentWriter"></param>
            public BoolXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            /// <summary>
            /// ContentsItemの保存を行います。
            /// </summary>
            /// <param name="info"></param>
            public override void SaveContentsItem(XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                writer.WriteString(item.Value.ToString());
            }
        }
        #endregion

        #region StringXmlContentsItemWriterクラス
        /// <summary>
        /// StringXmlContentsItemの保存を行うクラスです。
        /// </summary>
        protected class StringXmlContentsItemWriter : XmlContentsItemWriter {
            /// <summary>
            /// StringXmlContentsItemWriterオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentWriter"></param>
            public StringXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            /// <summary>
            /// ContentsItemの保存を行います。
            /// </summary>
            /// <param name="info"></param>
            public override void SaveContentsItem(XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                writer.WriteString(item.Value.ToString());
            }
        }
        #endregion

        #region IntXmlContentsItemWriterクラス
        /// <summary>
        /// IntXmlContentsItemの保存を行うクラスです。
        /// </summary>
        protected class IntXmlContentsItemWriter : XmlContentsItemWriter {
            /// <summary>
            /// IntXmlContentsItemWriterオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentWriter"></param>
            public IntXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            /// <summary>
            /// ContentsItemの保存を行います。
            /// </summary>
            /// <param name="info"></param>
            public override void SaveContentsItem(XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                writer.WriteString(item.Value.ToString());
            }
        }
        #endregion

        #region DoubleXmlContentsItemWriterクラス
        /// <summary>
        /// DoubleXmlContentsItemの保存を行うクラスです。
        /// </summary>
        protected class DoubleXmlContentsItemWriter : XmlContentsItemWriter {
            /// <summary>
            /// DoubleXmlContentsItemWriterオブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentWriter"></param>
            public DoubleXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }

            /// <summary>
            /// ContentsItemの保存を行います。
            /// </summary>
            /// <param name="info"></param>
            public override void SaveContentsItem(XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                writer.WriteString(item.Value.ToString());
            }
        } 
        #endregion

        #endregion
    }
    #endregion

    #region XmlContentsWriter_01

    /// <summary>
    /// XmlContentsWriter Ver1を表すクラスです。
    /// </summary>
    public class XmlContentsWriter_01 : XmlContentsWriter_00 {

        //*******************************************************
        // 変更点
        //
        // Ver1はContainerの子Elementの個数の保持方法の変更。
        // Count要素⇒Count属性とする。
        //
        //*******************************************************

        /// <summary>
        /// Package書き込みバージョンを表します。
        /// このプロパティをオーバーライドすることでバージョンの切り替えを行います。
        /// </summary>
        protected override string Version => "1.0";

        /// <summary>
        /// ContainerItemWriterを返します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemWriter CreateContainerItemWriter() {
            return new ContainerXmlContentsItemWriter_01(this);
        }
        /// <summary>
        /// ContainerXmlContentsItemの保存を行うクラスです。
        /// </summary>
        protected class ContainerXmlContentsItemWriter_01 : ContainerXmlContentsItemWriter {
            /// <summary>
            /// ContainerXmlContentsItemWriter_01オブジェクトの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="parentWriter"></param>
            public ContainerXmlContentsItemWriter_01(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            /// <summary>
            /// ContentsItemの保存を行います。
            /// </summary>
            /// <param name="info"></param>
            public override void SaveContentsItem(XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                ContainerXmlContentsItem container = (ContainerXmlContentsItem)item;
                //writer.WriteElementString(container.ItemCountElement, container.Items.Count.ToString());
                writer.WriteAttributeString(container.ItemCountElement, container.Items.Count.ToString());
                foreach (XmlContentsItem item1 in container.Items.Values) {
                    XmlContentsItemWriteInfo itemInfo = new XmlContentsItemWriteInfo() {
                        Writer = info.Writer,
                        ItemProvider = info.ItemProvider,
                        Item = item1
                    };
                    this.ParentWriter.SaveContentsItem(itemInfo);
                    //this.m_ItemProvider.Save(writer, item1);
                }
            }
        }
    }
    #endregion

}
