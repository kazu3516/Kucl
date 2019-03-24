using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Kucl.Collections;

namespace Kucl.Test {
    public partial class Form1:Form {
		private CustomCollection<Sample> list;
        public Form1() {
            InitializeComponent();
			this.list = new CustomCollection<Sample>();
			this.list.ItemPropertyChanged += new CustomCollection<Sample>.CustomCollectionEventHandler(list_ItemPropertyChanged);
			this.list.ItemChanged += new EventHandler(list_ItemChanged);
			this.list.BeforeClear += new EventHandler(list_BeforeClear);
			this.list.AfterSet += new CustomCollection<Sample>.CustomCollectionEventHandler2(list_AfterSet);
			this.list.AfterRemove += new CustomCollection<Sample>.CustomCollectionEventHandler(list_AfterRemove);
			this.list.AfterInsert += new CustomCollection<Sample>.CustomCollectionEventHandler(list_AfterInsert);
		}

		void list_ItemPropertyChanged(object sender,CustomCollection<Sample>.CustomCollectionEventArgs e) {
			this.textBox2.Text += "\r\nPropertyChanged";
		}

		void list_AfterInsert(object sender,CustomCollection<Sample>.CustomCollectionEventArgs e) {
			this.textBox2.Text += "\r\nAfterInsert";
		}

		void list_AfterRemove(object sender,CustomCollection<Sample>.CustomCollectionEventArgs e) {
			this.textBox2.Text += "\r\nAfterRemove";
		}

		void list_AfterSet(object sender,CustomCollection<Sample>.CustomCollectionEventArgs2 e) {
			this.textBox2.Text += "\r\nAfterSet";
		}

		void list_BeforeClear(object sender,EventArgs e) {
			this.textBox2.Text += "\r\nBeforeClear";
		}

		void list_ItemChanged(object sender,EventArgs e) {
			this.textBox2.Text += "\r\nItemChanged";
		}

		private void button2_Click(object sender,EventArgs e) {
			//追加
			Sample item = new Sample();
			item.Name = this.textBox1.Text;
			this.list.Add(item);
			this.listView1.Items.Add(new ListViewItem(new string[]{(this.listView1.Items.Count + 1).ToString(),this.textBox1.Text}));
		}

		private void button3_Click(object sender,EventArgs e) {
			if (this.listView1.SelectedIndices.Count != 0) {
				//削除
				int i = this.listView1.SelectedIndices[0];
				this.list.RemoveAt(i);
				this.listView1.Items.RemoveAt(i);
			}
		}

		private void button4_Click(object sender,EventArgs e) {
			if (this.listView1.SelectedIndices.Count != 0) {
				//更新
				int i = this.listView1.SelectedIndices[0];
				Sample item = new Sample();
				item.Name = this.textBox1.Text;
				this.list[i] = item;
				this.listView1.Items[i].SubItems[1].Text = this.textBox1.Text;
			}
		}

		private void button1_Click(object sender,EventArgs e) {
			if (this.listView1.SelectedIndices.Count != 0) {
				//Name変更
				int i = this.listView1.SelectedIndices[0];
				Sample item = this.list[i];
				item.Name = this.textBox1.Text;
				this.listView1.Items[i].SubItems[1].Text = this.textBox1.Text;
			}
		}

		private void listView1_SelectedIndexChanged(object sender,EventArgs e) {
			if (this.listView1.SelectedIndices.Count != 0) {
				int i = this.listView1.SelectedIndices[0];
				Sample item = this.list[i];
				this.textBox1.Text = item.Name;
			}
		}
    }
}
