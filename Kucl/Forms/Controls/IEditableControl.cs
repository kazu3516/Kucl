using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kucl.Cmd;

namespace Kucl.Forms.Controls {

    /// <summary>
    /// 標準の編集メニューをサポートするコントロールが実装するインターフェースです
    /// </summary>
    public interface IEditableControl {

        /// <summary>
        /// 編集メニューによる操作を記録するCommandHistoryを取得または設定します。
        /// </summary>
        CommandHistory CommandHistory {
            get;
            set;
        }

        /// <summary>
        /// Can...プロパティの値が変更されたことを通知するイベントを表します。
        /// </summary>
        event EventHandler NotifyStatusChanged;
        
        /// <summary>
        /// Cutを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanCut {
            get;
        }
        /// <summary>
        /// Copyを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanCopy {
            get;
        }
        /// <summary>
        /// Pasteを実行可能かどうかを示すbool値を返します。
        /// </summary>
        /// <returns></returns>
        bool CanPaste();
        /// <summary>
        /// Deleteを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanDelete {
            get;
        }
        /// <summary>
        /// SelectAllを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanSelectAll {
            get;
        }

        /// <summary>
        /// Cut操作を行います。
        /// </summary>
        void Cut();
        /// <summary>
        /// コピー操作を行います。
        /// </summary>
        void Copy();
        /// <summary>
        /// 貼り付け操作を行います。
        /// </summary>
        void Paste();
        /// <summary>
        /// 削除操作を行います。
        /// </summary>
        void Delete();
        /// <summary>
        /// すべて選択の操作を行います。
        /// </summary>
        void SelectAll();


    }
}
