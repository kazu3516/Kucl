using System;
using System.Collections.Generic;

namespace Kucl {
	/// <summary>
	/// 連続した整数を生成し、管理するクラスです
	/// このクラスが生成する通し番号は、抜けている数値がある場合、抜けている数値を優先的に生成します
	/// </summary>
	public class SequentialNumberGenerator{
        
        #region メンバ変数
        private int m_start;
        private int m_end;
        private List<int> m_list;
        private SequentialNumnerMode m_Mode;
        private Predicate<int> m_ForCondition;
        private int m_Step;
        #endregion

        #region プロパティ
        /// <summary>
        /// モードを取得、設定します。
        /// </summary>
        public SequentialNumnerMode Mode {
            get {
                return this.m_Mode;
            }
            set {
                this.m_Mode = value;
                this.OnSetMode();
            }
        }
        private void OnSetMode() {
            int start = this.m_start;
            int end = this.m_end;
            switch(this.m_Mode) {
                case SequentialNumnerMode.Ascending:
                    this.m_ForCondition = delegate(int i) {
                        return i <= this.m_end;
                    };
                    this.m_Step = 1;
                    this.m_start = Math.Min(start,end);
                    this.m_end = Math.Max(start,end);
                    break;
                case SequentialNumnerMode.Descending:
                    this.m_ForCondition = delegate(int i) {
                        return i >= this.m_end;
                    };
                    this.m_Step = -1;
                    this.m_end = Math.Min(start,end);
                    this.m_start = Math.Max(start,end);
                    break;
                default:
                    throw new ApplicationException();
            }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// SequentialNumberGeneratorオブジェクトを初期化します。
        /// 既定では番号は0から始まります。
        /// </summary>
        public SequentialNumberGenerator()
            : this(0,int.MaxValue) {
        }
        /// <summary>
        /// SequentialNumberGeneratorオブジェクトを初期化します。
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public SequentialNumberGenerator(int start,int end) {
            if(start < end) {
                this.Mode = SequentialNumnerMode.Ascending;
            }
            else {
                this.Mode = SequentialNumnerMode.Descending;
            }
            this.m_list = new List<int>();
            this.m_start = start;
            this.m_end = end;
        } 
        #endregion

		/// <summary>
		/// 通し番号の次の数値を返す
		/// </summary>
		/// <returns>通し番号の次の数値</returns>
		public int GetNext(){
            for(int i = this.m_start;this.m_ForCondition.Invoke(i);i += this.m_Step) {
                if(this.m_list.Contains(i)) {
                    continue;
                }
                this.m_list.Add(i);
                return i;
            }
            throw new ApplicationException("通し番号の取得に失敗しました。\r\n取得できる番号がありません。");
		}
		/// <summary>
		/// 通し番号の次の数値を先読みする。
		/// </summary>
		/// <returns>通し番号の次の数値</returns>
		public int PeekNext(){
            for(int i = this.m_start;this.m_ForCondition.Invoke(i);i += this.m_Step) {
                if(this.m_list.Contains(i)) {
                    continue;
                }
                return i;
            }
            return -1;
		}
		/// <summary>
		/// 通し番号からnumberを削除します
		/// </summary>
		/// <param name="number">通し番号から削除する数値</param>
		public void RemoveNumber(int number){
            if(this.m_list.Contains(number)) {
                this.m_list.Remove(number);
            }
		}
        /// <summary>
        /// 指定した番号を取得済み番号として登録します
        /// </summary>
        /// <param name="number"></param>
        public void RegistNumber(int number) {
            this.m_list.Add(number);
        }

        /// <summary>
        /// 指定した番号が取得済みかどうかを返します。
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool IsRegistered(int number) {
            return this.m_list.Contains(number);
        }
        /// <summary>
        /// 全ての取得済み番号をリセットします
        /// </summary>
        public void Reset() {
            this.m_list.Clear();
        }
	}

    /// <summary>
    /// 連番取得のモードを示す列挙体です。
    /// </summary>
    public enum SequentialNumnerMode {
        /// <summary>
        /// 増加方向
        /// </summary>
        Ascending,
        /// <summary>
        /// 減少方向
        /// </summary>
        Descending
    }
}
