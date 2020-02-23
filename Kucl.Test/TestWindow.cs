using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kucl.Xml;
using Kucl.Xml.XmlCfg;
namespace Kucl.Test {
    public partial class TestWindow : Form {

        public TestWindow() {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e) {
            FileDocumentTestWindow window = new FileDocumentTestWindow();
            window.Show();
        }

        private void button1_Click(object sender, EventArgs e) {
            XmlContentsModelViewer viewer = new XmlContentsModelViewer();
            XmlConfigModel model = new XmlConfigModel();
            AppMain.g_AppMain.UseConfigObjects.Cast<IUseConfig>().ToList().ForEach(x => x.ReflectConfig(model));
            viewer.TargetModel = model;
            viewer.ShowDialog();
        }
    }

}
