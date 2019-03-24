using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kucl.Forms.Dialog {

    /// <summary>
    /// オプションダイアログを表すクラスです。
    /// </summary>
    public partial class OptionDialogBase : Form {


        #region フィールド(メンバ変数、プロパティ、イベント)

        #endregion

        #region コンストラクタ
        /// <summary>
        /// OptionDialogBaseオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public OptionDialogBase() {
            InitializeComponent();
        }
        #endregion

        #region イベントハンドラ

        #endregion

        /// <summary>
        /// コントロールのプロパティとオブジェクトのプロパティの間のデータバインドを設定します。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propertyName"></param>
        /// <param name="bindObject"></param>
        /// <param name="bindPropertyName"></param>
        /// <param name="clearDataBindings">trueを指定すると、既存のデータバインドを全て削除した後に、指定されたデータバインドを設定します。規定値はtrueです。</param>
        protected void AddSingleDataBind(System.Windows.Forms.Control control,string propertyName,object bindObject,string bindPropertyName,bool clearDataBindings = true) {
            control.DataBindings.Clear();
            control.DataBindings.Add(propertyName, bindObject, bindPropertyName, true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
