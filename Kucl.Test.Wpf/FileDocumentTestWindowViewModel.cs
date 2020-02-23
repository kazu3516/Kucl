using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Kucl;

namespace Kucl.Test.Wpf {
    public class FileDocumentTestWindowViewModel : IDisposable {

        private TestDocument document;

        private CompositeDisposable disposables;

        public ReactiveProperty<string> Text {
            get;
        }

        public ReactiveCommand NewDocumentCommand {
            get;
        }
        public ReactiveCommand OpenFileCommand {
            get;
        }

        public ReactiveCommand SaveFileCommand {
            get;
        }

        public ReactiveCommand SaveAsCommand {
            get;
        }

        public ReactiveCommand CloseCommand {
            get;
        }


        public FileDocumentTestWindowViewModel() {
            this.document = new TestDocument();
            this.disposables = new CompositeDisposable();

            this.Text = new ReactiveProperty<string>().AddTo(this.disposables);
            this.Text.Subscribe(x =>
            {
                this.document.TestData = x;
                this.document.Dirty = true;
            });
            this.NewDocumentCommand = new ReactiveCommand().AddTo(this.disposables)
                .WithSubscribe(() =>
                {
                    if(this.document.CreateNewDocument()) {
                        this.Text.Value = this.document.TestData;
                        this.document.Dirty = false;
                    }
                });
            this.OpenFileCommand = new ReactiveCommand().AddTo(this.disposables)
                .WithSubscribe(() => {
                    if(this.document.OpenDocument()) {
                        this.Text.Value = this.document.TestData;
                        this.document.Dirty = false;
                    }
                });
            this.SaveAsCommand = new ReactiveCommand().AddTo(this.disposables)
                .WithSubscribe(() => {
                    this.document.SaveAsDocument();
                });
            this.SaveFileCommand = new ReactiveCommand().AddTo(this.disposables)
                .WithSubscribe(() => {
                    this.document.SaveDocument();
                });

            this.document.RequestOpenFileDialog += this.Document_RequestOpenFileDialog;
            this.document.RequestSaveFileDialog += this.Document_RequestSaveFileDialog;
            this.document.RequestConfirmCloseMessageDialog += this.Document_RequestConfirmCloseMessageDialog;

            this.document.Dirty = false;
        }

        private void Document_RequestConfirmCloseMessageDialog(object sender,MessageBoxEventArgs e) {
            var service = FileDocumentDialogService.GetService("Kucl.Test.Wpf");
            service.RaiseRequestConfirmCloseMessageDialog(e);
        }

        private void Document_RequestSaveFileDialog(object sender,FileDialogEventArgs e) {
            var service = FileDocumentDialogService.GetService("Kucl.Test.Wpf");
            e.Filter = "テキストファイル(*.txt)|*.txt";
            service.RaiseRequestSaveFileDialog(e);
        }

        private void Document_RequestOpenFileDialog(object sender,FileDialogEventArgs e) {
            var service = FileDocumentDialogService.GetService("Kucl.Test.Wpf");
            e.Filter = "テキストファイル(*.txt)|*.txt";
            service.RaiseRequestOpenFileDialog(e);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    this.disposables.Dispose();
                    this.disposables = null;

                    this.document.Dispose();
                    this.document = null;
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~FileDocumentTestWindowViewModel()
        // {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose() {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }


    public class TestDocument : FileDocumentBase {

        public string TestData {
            get; set;
        }

        public TestDocument() {
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
