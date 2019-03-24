using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Kucl.Collections;

namespace Kucl.Test {
    public partial class Form2:Form {
		private CustomDictionary<string,Sample> list;
        public Form2() {
            InitializeComponent();
			this.list = new CustomDictionary<string,Sample>();
			this.list.ItemPropertyChanged += new CustomDictionary<string,Sample>.CustomDictionaryEventHandler(list_ItemPropertyChanged);
			this.list.ItemChanged += new EventHandler(list_ItemChanged);
			this.list.BeforeClear += new EventHandler(list_BeforeClear);
			this.list.AfterSet += new CustomDictionary<string,Sample>.CustomDictionaryEventHandler2(list_AfterSet);
			this.list.AfterRemove += new CustomDictionary<string,Sample>.CustomDictionaryEventHandler(list_AfterRemove);
			this.list.AfterInsert += new CustomDictionary<string,Sample>.CustomDictionaryEventHandler(list_AfterInsert);
		}

		void list_ItemPropertyChanged(object sender,CustomDictionary<string,Sample>.CustomDictionaryEventArgs e) {
			this.textBox2.Text += "\r\nPropertyChanged";
		}

		void list_AfterInsert(object sender,CustomDictionary<string,Sample>.CustomDictionaryEventArgs e) {
			this.textBox2.Text += "\r\nAfterInsert";
		}

		void list_AfterRemove(object sender,CustomDictionary<string,Sample>.CustomDictionaryEventArgs e) {
			this.textBox2.Text += "\r\nAfterRemove";
		}

		void list_AfterSet(object sender,CustomDictionary<string,Sample>.CustomDictionaryEventArgs2 e) {
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
			string key = this.textBox3.Text;
			this.list.Add(key,item);
			this.listView1.Items.Add(new ListViewItem(new string[]{key,(this.listView1.Items.Count + 1).ToString(),this.textBox1.Text}));
		}

		private void button3_Click(object sender,EventArgs e) {
			if (this.listView1.SelectedIndices.Count != 0) {
				//削除
				int i = this.listView1.SelectedIndices[0];
				string key = this.listView1.Items[i].Text;
				this.list.Remove(key);
				this.listView1.Items.RemoveAt(i);
			}
		}

		private void button4_Click(object sender,EventArgs e) {
			if (this.listView1.SelectedIndices.Count != 0) {
				//更新
				int i = this.listView1.SelectedIndices[0];
				string key = this.listView1.Items[i].Text;
				Sample item = new Sample();
				item.Name = this.textBox1.Text;
				this.list[key] = item;
				this.listView1.Items[i].SubItems[2].Text = this.textBox1.Text;
			}
		}

		private void button1_Click(object sender,EventArgs e) {
			if (this.listView1.SelectedIndices.Count != 0) {
				//Name変更
				int i = this.listView1.SelectedIndices[0];
				string key = this.listView1.Items[i].Text;
				Sample item = this.list[key];
				item.Name = this.textBox1.Text;
				this.listView1.Items[i].SubItems[2].Text = this.textBox1.Text;
			}
		}

		private void listView1_SelectedIndexChanged(object sender,EventArgs e) {
			if (this.listView1.SelectedIndices.Count != 0) {
				int i = this.listView1.SelectedIndices[0];
				string key = this.listView1.Items[i].Text;
				Sample item = this.list[key];
				this.textBox1.Text = item.Name;
			}
		}

		private void button5_Click(object sender,EventArgs e) {
			KeyValuePair<string,Sample>[] array = new KeyValuePair<string,Sample>[this.list.Count];
			this.list.CopyTo(array,0);
			MessageBox.Show(array.Length.ToString());
			
		}
    }
}
