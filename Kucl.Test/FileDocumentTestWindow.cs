using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kucl.Test {
    public partial class FileDocumentTestWindow : Form {

        private TestFileDocument document;

        public FileDocumentTestWindow() {
            InitializeComponent();

            this.document = new TestFileDocument();
        }

        private void 新規作成NToolStripMenuItem_Click(object sender,EventArgs e) {
            if(this.document.CreateNewDocument()) {
                this.richTextBox1.Text = this.document.TestData;
                this.document.Dirty = false;
            }
        }

        private void 開くOToolStripMenuItem_Click(object sender,EventArgs e) {
            if(this.document.OpenDocument()) {
                this.richTextBox1.Text = this.document.TestData;
                this.document.Dirty = false;
            }
        }

        private void 上書き保存SToolStripMenuItem_Click(object sender,EventArgs e) {
            this.document.SaveDocument();
        }

        private void 名前を付けて保存AToolStripMenuItem_Click(object sender,EventArgs e) {
            this.document.SaveAsDocument();
        }

        private void 終了XToolStripMenuItem_Click(object sender,EventArgs e) {
            this.Close();
        }

        private void richTextBox1_TextChanged(object sender,EventArgs e) {
            this.document.Dirty = true;
            this.document.TestData = this.richTextBox1.Text;
        }

        private void FileDocumentTestWindow_FormClosing(object sender,FormClosingEventArgs e) {
            if(!this.document.Close()) {
                e.Cancel = true;
            }
        }
    }

    public class TestFileDocument : FileDocument {

        public string TestData {
            get; set;
        }

        protected override void OnSaveFile() {
            using(StreamWriter writer = File.CreateText(this.FileName)) {
                writer.Write(this.TestData);
            }
        }

        protected override void OnLoadFile() {
            using(StreamReader reader = File.OpenText(this.FileName)) {
                this.TestData = reader.ReadToEnd();
            }
        }

        protected override void OnCreateNewDocument() {
            this.TestData = "";
        }

        protected override void OnCloseDocument() {
            this.TestData = "";
        }
    }


}
