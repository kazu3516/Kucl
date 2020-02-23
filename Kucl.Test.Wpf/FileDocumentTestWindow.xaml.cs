using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
namespace Kucl.Test.Wpf {
    /// <summary>
    /// FileDocumentTestWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FileDocumentTestWindow : Window {
        public FileDocumentTestWindow() {
            InitializeComponent();

            var service = FileDocumentDialogService.GetService("Kucl.Test.Wpf");
            service.RequestConfirmCloseMessageDialog += this.Service_RequestConfirmCloseMessageDialog;
            service.RequestOpenFileDialog += this.Service_RequestOpenFileDialog;
            service.RequestSaveFileDialog += this.Service_RequestSaveFileDialog;
        }

        private void Service_RequestSaveFileDialog(object sender,FileDialogEventArgs e) {
            var dialog = new SaveFileDialog();
            dialog.Filter = e.Filter;
            dialog.FileName = e.FileName;
            if(dialog.ShowDialog() == true) {
                e.FileName = dialog.FileName;
            }
            else {
                e.Canceled = true;
            }
        }

        private void Service_RequestOpenFileDialog(object sender,FileDialogEventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.Filter = e.Filter;
            dialog.FileName = e.FileName;
            if(dialog.ShowDialog() == true) {
                e.FileName = dialog.FileName;
            }
            else {
                e.Canceled = true;
            }
        }

        private void Service_RequestConfirmCloseMessageDialog(object sender,MessageBoxEventArgs e) {
            System.Windows.MessageBoxResult result = MessageBox.Show(e.Message,e.Title,MessageBoxButton.YesNoCancel,MessageBoxImage.Information);
            switch(result) {
                case System.Windows.MessageBoxResult.Yes:
                    e.Result = MessageBoxResult.Yes;
                    break;
                case System.Windows.MessageBoxResult.No:
                    e.Result = MessageBoxResult.No;
                    break;
                case System.Windows.MessageBoxResult.Cancel:
                    e.Result = MessageBoxResult.Cancel;
                    break;
                default:
                    e.Result = MessageBoxResult.None;
                    break;
            }
        }
    }
}
