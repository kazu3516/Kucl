using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kucl.Forms.Controls {

    /// <summary>
    /// Listを操作するメソッドを提供します。
    /// </summary>
    public static class ListOperation{

        #region MoveUp/MoveDown
        /// <summary>
        /// 上へ移動が可能かどうかを返します。
        /// </summary>
        /// <param name="targetIndices">対象インデックスのコレクション。（例：ListView.SelectedIndices）</param>
        /// <returns></returns>
        public static bool CanMoveUp(IList targetIndices) {
            int index0 = -1;
            for (int i = 0; i < targetIndices.Count; i++) {
                int index = (int)targetIndices[i];
                if (index - index0 > 1) {
                    //一番上、もしくは選択されているアイテムの間に一つ以上間があった場合、canMoveUP=trueとする
                    return true;
                }
                index0 = index;
            }
            return false;
        }
        /// <summary>
        /// 下へ移動が可能かどうかを返します。
        /// </summary>
        /// <param name="allItems">全アイテムのコレクション。(例：ListView.Items）</param>
        /// <param name="targetIndices">対象インデックスのコレクション。（例：ListView.SelectedIndices）</param>
        /// <returns></returns>
        public static bool CanMoveDown(IList allItems, System.Collections.IList targetIndices) {
            int index0 = allItems.Count;
            for (int i = targetIndices.Count - 1; i >= 0; i--) {
                int index = (int)targetIndices[i];
                if (index0 - index > 1) {
                    //一番下、もしくは選択されているアイテムの間に一つ以上間があった場合、canMoveUP=trueとする
                    return true;
                }
                index0 = index;
            }
            return false;
        }
        /// <summary>
        /// 上へ移動を実装します。
        /// </summary>
        /// <param name="allItems">全アイテムのコレクション。(例：ListView.Items）</param>
        /// <param name="targetIndices">対象インデックスのコレクション。（例：ListView.SelectedIndices）</param>
        public static  void MoveUp(IList allItems, IList targetIndices) {
            if (!ListOperation.CanMoveUp(targetIndices)) {
                return;
            }
            int index0 = -1;
            for (int i = 0; i < targetIndices.Count; i++) {
                int index = (int)targetIndices[i];
                if (index - index0 > 1) {
                    object item = allItems[index];
                    allItems.RemoveAt(index);
                    allItems.Insert(index - 1, item);
                    //移動に成功した場合、indexは-1する。(次の選択indexが連続していた場合でも移動できるようにするため)
                    //ex)2,3が選択されていた場合、2⇒1になるので、3⇒2になるはず。
                    index--;
                }
                index0 = index;
            }
        }

        /// <summary>
        /// 下へ移動を実装します。
        /// </summary>
        /// <param name="allItems">全アイテムのコレクション。(例：ListView.Items）</param>
        /// <param name="targetIndices">対象インデックスのコレクション。（例：ListView.SelectedIndices）</param>
        public static void MoveDown(IList allItems, IList targetIndices) {
            if (!ListOperation.CanMoveDown(allItems, targetIndices)) {
                return;
            }
            int index0 = allItems.Count;
            for (int i = targetIndices.Count - 1; i >= 0; i--) {
                int index = (int)targetIndices[i];
                if (index0 - index > 1) {
                    object item = allItems[index];
                    allItems.RemoveAt(index);
                    allItems.Insert(index + 1, item);
                    //移動に成功した場合、indexは+1する。(次の選択indexが連続していた場合でも移動できるようにするため)
                    //ex)2,3が選択されていた場合、3⇒4になるので、2⇒3になるはず。
                    index++;
                }
                index0 = index;
            }
        }
        #endregion

    }
}
