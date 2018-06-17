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

        #endregion

        #region コンストラクタ
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

        public abstract void SavePackage(XmlContentsPackageWriteInfo info);
        public abstract void SaveContents(XmlContentsWriteInfo info);
        public abstract void SaveContentsItem(XmlContentsItemWriteInfo info);

        protected abstract class XmlContentsItemWriter {
            protected XmlContentsWriter ParentWriter {
                get;
            }
            protected XmlContentsItemWriter(XmlContentsWriter parentWriter) {
                this.ParentWriter = parentWriter;
            }
            public abstract void SaveContentsItem( XmlContentsItemWriteInfo info);
        }

        protected XmlContentsItemWriter CreateItemWriter(XmlContentsItemType type) {
            return this.m_CreateItemWriterTable[type]();
        }
        protected abstract XmlContentsItemWriter CreateContainerItemWriter();
        protected abstract XmlContentsItemWriter CreateBoolItemWriter();
        protected abstract XmlContentsItemWriter CreateStringItemWriter();
        protected abstract XmlContentsItemWriter CreateIntItemWriter();
        protected abstract XmlContentsItemWriter CreateDoubleItemWriter();

    }
    #endregion


    #region XmlContentsPackageWriteInfo
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
        public XmlContentsPackageWriteInfo() {
        }
        #endregion

    }
    #endregion

    #region XmlContentsWriteInfo
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
        public XmlContentsWriteInfo() {
        }
        #endregion

    }
    #endregion

    #region XmlContentsItemWriteInfo
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
                    }
                    break;
            }
            throw new ArgumentException("不明なtypeNameとversionが指定されました");
        }
    }
    #endregion

    public class XmlContentsWriter_00 : XmlContentsWriter {

        #region フィールド(メンバ変数、プロパティ、イベント)

        #endregion

        #region コンストラクタ
        public XmlContentsWriter_00() {
        }
        #endregion

        public override void SavePackage(XmlContentsPackageWriteInfo info) {
            string filename = info.FileName;
            XmlContentsPackage package = info.Package;
            XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            try {
                writer.WriteStartDocument();
                writer.WriteStartElement(package.PackageRootElement);
                writer.WriteAttributeString(package.PackageRootCountAttribute, package.XmlContentsTable.Count.ToString());
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

        #region XmlContentsItemWriterの継承とインスタンス生成

        protected override XmlContentsItemWriter CreateContainerItemWriter() {
            return new ContainerXmlContentsItemWriter(this);
        }

        protected override XmlContentsItemWriter CreateBoolItemWriter() {
            return new BoolXmlContentsItemWriter(this);
        }

        protected override XmlContentsItemWriter CreateStringItemWriter() {
            return new StringXmlContentsItemWriter(this);
        }

        protected override XmlContentsItemWriter CreateIntItemWriter() {
            return new IntXmlContentsItemWriter(this);
        }

        protected override XmlContentsItemWriter CreateDoubleItemWriter() {
            return new DoubleXmlContentsItemWriter(this);
        }

        protected class ContainerXmlContentsItemWriter : XmlContentsItemWriter {
            public ContainerXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            public override void SaveContentsItem(  XmlContentsItemWriteInfo info) {
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
        protected class BoolXmlContentsItemWriter : XmlContentsItemWriter {
            public BoolXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            public override void SaveContentsItem(  XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                writer.WriteString(item.Value.ToString());
            }
        }
        protected class StringXmlContentsItemWriter : XmlContentsItemWriter {
            public StringXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            public override void SaveContentsItem(  XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                writer.WriteString(item.Value.ToString());
            }
        }
        protected class IntXmlContentsItemWriter : XmlContentsItemWriter {
            public IntXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            public override void SaveContentsItem( XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                writer.WriteString(item.Value.ToString());
            }
        }
        protected class DoubleXmlContentsItemWriter : XmlContentsItemWriter {
            public DoubleXmlContentsItemWriter(XmlContentsWriter parentWriter) : base(parentWriter) {
            }
            public override void SaveContentsItem(  XmlContentsItemWriteInfo info) {
                XmlTextWriter writer = info.Writer;
                XmlContentsItem item = info.Item;
                writer.WriteString(item.Value.ToString());
            }
        }


        #endregion
    }
}
