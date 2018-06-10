using System;
using System.Collections.Generic;


namespace Kucl.Cmd {

    #region CommandHistory
    /// <summary>
    /// コマンドデータのリストを表現するクラスです
    /// </summary>
    public class CommandHistory {

        #region メンバ変数
        private Stack<CommandBase> m_undostack;
        private Stack<CommandBase> m_redostack;

        private bool m_IsDefaultPoint;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// CommandHistoryクラスの新しいインスタンスを初期化します
        /// </summary>
        public CommandHistory() {
            this.m_undostack = new Stack<CommandBase>();
            this.m_redostack = new Stack<CommandBase>();
            this.m_IsDefaultPoint = true;
        }
        #endregion

        #region RegisterCommand
        /// <summary>
        /// 指定したコマンドを登録します。
        /// </summary>
        /// <param name="command"></param>
        public void RegisterCommand(CommandBase command) {
            this.m_undostack.Push(command);
            this.m_redostack.Clear();
            if(this.IsDefaultPoint) {
                command.UndoToDefault = true;
                this.IsDefaultPoint = false;
            }
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// Undo可能かどうかを取得します。
        /// </summary>
        public bool CanUndo {
            get {
                return this.m_undostack.Count != 0;
            }
        }
        /// <summary>
        /// Redo可能かどうかを取得します。
        /// </summary>
        public bool CanRedo {
            get {
                return this.m_redostack.Count != 0;
            }
        }
        /// <summary>
        /// DefaultPointかどうかを取得します。
        /// DefaultPointはシリアル化された状態を表し、Undo or Redoにより変更された、とされる状態です。
        /// </summary>
		public bool IsDefaultPoint {
			get {
				return this.m_IsDefaultPoint;
			}
            protected set {
                if(this.m_IsDefaultPoint != value) {
                    this.m_IsDefaultPoint = value;
                    this.OnIsDefaultPointChanged(new EventArgs());
                }
            }
		}
        /// <summary>
        /// IsDefaultPointプロパティの値が変更されたときに発生するイベントです。
        /// </summary>
        public event EventHandler IsDefaultPointChanged;
        /// <summary>
        /// IsDefaultPointプロパティの値が変更されたときに呼び出されます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnIsDefaultPointChanged(EventArgs e) {
            if(this.IsDefaultPointChanged != null) {
                this.IsDefaultPointChanged(this,e);
            }
        }
		#endregion

        #region Undo
        /// <summary>
        /// 元に戻します
        /// </summary>
        public void Undo() {
            CommandBase command = this.m_undostack.Pop();
            this.m_redostack.Push(command);
            command.Undo();
            this.IsDefaultPoint = command.UndoToDefault;
        }
        #endregion

        #region Redo
        /// <summary>
        /// やり直しをします。
        /// </summary>
        public void Redo() {
            CommandBase command = this.m_redostack.Pop();
            this.m_undostack.Push(command);
            command.Redo();
            this.IsDefaultPoint = command.RedoToDefault;
        }
        #endregion

        #region ClearUndo
        /// <summary>
        /// Undoリストをクリアします。
        /// </summary>
        public void ClearUndo() {
            this.m_undostack.Clear();
        }
        #endregion

        #region ClearRedo
        /// <summary>
        /// Redoリストをクリアします。
        /// </summary>
        public void ClearRedo() {
            this.m_redostack.Clear();
        }
        #endregion

        #region ClearCommand
        /// <summary>
        /// Undo/Redoリストをクリアします。
        /// </summary>
        public void ClearCommand() {
            this.m_undostack.Clear();
            this.m_redostack.Clear();
        }
        #endregion

        #region PeekNextCommand
        /// <summary>
        /// Undoコマンドを先読みします。
        /// </summary>
        /// <returns></returns>
        public CommandBase PeekNextUndoCommand() {
            if(this.m_undostack.Count != 0) {
                return this.m_undostack.Peek();
            }
            return null;
        }
        /// <summary>
        /// Redoコマンドを先読みします。
        /// </summary>
        /// <returns></returns>
        public CommandBase PeekNextRedoCommand() {
            if(this.m_redostack.Count != 0) {
                return this.m_redostack.Peek();
            }
            return null;
        }
        #endregion

        #region CreateDefaultPoint
        /// <summary>
        /// 現在の状態をDefaultPointに設定します。
        /// 通常、保存時に設定し、Dirtyフラグなどの復帰に用います。
        /// </summary>
        public void CreateDefaultPoint() {
            //DefaultPointのリセット
            foreach(CommandBase command in this.m_undostack) {
                command.UndoToDefault = false;
                command.RedoToDefault = false;
            }
            foreach(CommandBase command in this.m_redostack) {
                command.UndoToDefault = false;
                command.RedoToDefault = false;
            }
            //DefaultPointの設定
            if(this.CanUndo) {
                //次のUndoコマンドはRedoするとDefaultPointとなる。
                CommandBase undo = this.PeekNextUndoCommand();
                undo.RedoToDefault = true;
            }
            if(this.CanRedo) {
                //次のRedoコマンドはUndoするとDefaultPointとなる。
                CommandBase redo = this.PeekNextRedoCommand();
                redo.UndoToDefault = true;
            }
            this.IsDefaultPoint = true;
        }


        #endregion
    }
    #endregion

    #region CommandBase
    /// <summary>
    /// CommandHistoryで管理されるCommandの基本クラスです。
    /// </summary>
    public abstract class CommandBase {
        private bool m_UndoToDefault;
        private bool m_RedoToDefault;
		/// <summary>
		/// Undoされた後、DefaultPointに戻るかどうかを表します。
		/// </summary>
        internal bool UndoToDefault {
            get {
                return this.m_UndoToDefault;
            }
            set {
                this.m_UndoToDefault = value;
            }
        }
		/// <summary>
        /// Redoされた後、DefaultPointに戻るかどうかを表します。
		/// </summary>
        internal bool RedoToDefault {
            get {
                return this.m_RedoToDefault;
            }
            set {
                this.m_RedoToDefault = value;
            }
        }

        /// <summary>
        /// 派生クラスでオーバーライドされると、元に戻す動作を定義します。
        /// </summary>
        public abstract void Undo();
        /// <summary>
        /// 派生クラスでオーバーライドされると、やり直しの動作を定義します。
        /// </summary>
        public abstract void Redo();
    }
    #endregion

    #region CombinedCommand
    /// <summary>
    /// 複数のコマンドをひとまとめにしたCombinedCommandを表すクラスです。
    /// </summary>
    public class CombinedCommand : CommandBase {


        #region フィールド(メンバ変数、プロパティ、イベント)
        private List<CommandBase> m_Commands;
        /// <summary>
        /// Commandsを取得します。
        /// </summary>
        public List<CommandBase> Commands {
            get {
                return this.m_Commands;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// CombinedCommandオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public CombinedCommand() {
            this.m_Commands = new List<CommandBase>();
        }

        #endregion

        #region CommandBaseの実装
        /// <summary>
        /// 最後に登録されたコマンドから順にUndoメソッドを呼び出し、元に戻します。
        /// </summary>
        public override void Undo() {
            for (int i = this.m_Commands.Count - 1; i >= 0; i--) {
                this.m_Commands[i].Undo();
            }
        }

        /// <summary>
        /// 最初に登録されたコマンドから順にRedoメソッドを呼び出し、やり直します。
        /// </summary>
        public override void Redo() {
            for (int i = 0; i < this.m_Commands.Count; i++) {
                this.m_Commands[i].Redo();
            }
        }
        #endregion

    }
    #endregion

}