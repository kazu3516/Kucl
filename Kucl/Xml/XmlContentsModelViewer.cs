using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kucl.Xml {

    /// <summary>
    /// XmlContentsModelを表示するViewerを表します。
    /// </summary>
    public partial class XmlContentsModelViewer:Form {

        #region フィールド(メンバ変数、プロパティ、イベント)

        private XmlContentsModel m_TargetModel;
        /// <summary>
        /// 表示するXmlContentsModelが変更されたときに発生します。
        /// </summary>
        public event EventHandler TargetModelChanged;
        /// <summary>
        /// 表示するXmlContentsModelが変更されたときに呼び出されます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnTargetModelChanged(EventArgs e) {
            if(this.TargetModelChanged != null) {
                this.TargetModelChanged(this,e);
            }
        }
        /// <summary>
        /// 表示するXmlContentsModelを取得または設定します。
        /// </summary>
        public XmlContentsModel TargetModel {
            get {
                return this.m_TargetModel;
            }
            set {
                if(this.m_TargetModel != value) {
                    this.m_TargetModel = value;
                    this.OnTargetModelChanged(new EventArgs());
                }
            }
        }


        #endregion

        #region コンストラクタ
        
        /// <summary>
        /// XmlContentsModelViewerクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlContentsModelViewer() {
            InitializeComponent();
            this.Initialize();
        }
        private void Initialize() {
            this.m_TargetModel = new XmlContentsModel();

            this.TargetModelChanged += XmlContentsModelViewer_TargetModelChanged;
        }


        
        #endregion

        #region イベントハンドラ
        void XmlContentsModelViewer_TargetModelChanged(object sender,EventArgs e) {
            this.RefreshView();
        }
        private void comboBox1_SelectedIndexChanged(object sender,EventArgs e) {
            this.RefreshListView();
        }
        #endregion


        private void RefreshView() {
            this.comboBox1.BeginUpdate();
            this.comboBox1.Items.Clear();
            foreach(string packageName in this.m_TargetModel.Packages.Keys) {
                this.comboBox1.Items.Add(packageName);
            }
            this.comboBox1.EndUpdate();

            this.listView1.BeginUpdate();
            this.listView1.Items.Clear();

            this.listView1.EndUpdate();
        }

        private void RefreshListView() {
            this.listView1.Items.Clear();

            string packageName = this.comboBox1.Text;
            if(!this.m_TargetModel.Packages.ContainsKey(packageName)) {
                return;
            }

            XmlContentsPackage package = this.m_TargetModel.Packages[packageName];

            foreach(XmlContents contents in package.XmlContentsTable.Values) {
                this.AddItemToListView(packageName + ":",contents.RootItem);
            }
        }
        private void AddItemToListView(string name,XmlContentsItem item) {
            if(item.Type == XmlContentsItemType.Container) {
                foreach(XmlContentsItem child in ((ContainerXmlContentsItem)item).Items.Values) {
                    this.AddItemToListView(name + item.Name + ".",child);
                }
            }
            else {
                this.listView1.Items.Add(new ListViewItem(new string[] { name + item.Name,item.Value.ToString() }));
            }
        }

        private void 名前を付けて保存SToolStripMenuItem_Click(object sender,EventArgs e) {
            this.folderBrowserDialog1.Description = "保存先フォルダを選択してください。";
            if(this.folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                this.m_TargetModel.Save(this.folderBrowserDialog1.SelectedPath);
            }
        }

        private void 終了QToolStripMenuItem_Click(object sender,EventArgs e) {
            this.Close();
        }
    }
}
