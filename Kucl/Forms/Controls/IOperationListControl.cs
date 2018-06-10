using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kucl.Cmd;

namespace Kucl.Forms.Controls {

    /// <summary>
    /// リストを操作するコントロールが実装するインターフェースです。
    /// </summary>
    public interface IOperationListControl {

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
        /// MoveUpを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanMoveUp {
            get;
        }
        /// <summary>
        /// MoveDownを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanMoveDown {
            get;
        }
        /// <summary>
        /// CreateNewItemを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanCreateNewItem {
            get;
        }
        /// <summary>
        /// DeleteItemを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanDeleteItem {
            get;
        }
        /// <summary>
        /// EditItemを実行可能かどうかを示すbool値を取得します。
        /// </summary>
        bool CanEditItem {
            get;
        }

        /// <summary>
        /// アクティブなアイテムを上へ移動させます。
        /// </summary>
        void MoveUp();
        /// <summary>
        /// アクティブなアイテムを下へ移動させます。
        /// </summary>
        void MoveDown();

        /// <summary>
        /// アイテムの新規作成を行います。
        /// </summary>
        void CreateNewItem();
        /// <summary>
        /// アクティブなアイテムを削除します。
        /// </summary>
        void DeleteItem();

        /// <summary>
        /// アクティブなアイテムを編集します。
        /// </summary>
        void EditItem();

    }
}
