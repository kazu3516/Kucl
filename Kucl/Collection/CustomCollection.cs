using System;
using System.Collections.Generic;
using System.Text;

namespace Kucl.Collection {

	#region CustomCollection

    /// <summary>
    /// 要素の変更を監視するコレクションです。
    /// </summary>
    /// <typeparam name="T">INotifyインターフェースを実装する型</typeparam>
	[Serializable()]
    public class CustomCollection<T> : System.Collections.CollectionBase,IList<T> where T:INotify {


		#region イベント
        /// <summary>
        /// 格納している要素のPropertyが変更された時に発生するイベントです。
        /// このイベントは格納されている要素のINotify.Changedイベントによって発生します。
        /// </summary>
		public event CustomCollectionEventHandler ItemPropertyChanged;
        /// <summary>
        /// ItemPropertyChangedイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="e"></param>
		protected void OnItemPropertyChanged(CustomCollectionEventArgs e) {
			if (this.ItemPropertyChanged != null) {
				this.ItemPropertyChanged(this,e);
			}
		}
        /// <summary>
        /// 格納している要素が変更された時に発生するイベントです。
        /// このイベントは、格納している要素に増減があった時、
        /// および、格納している要素のプロパティに変更があった時に発生します。
        /// </summary>
		public event EventHandler ItemChanged;
		/// <summary>
		/// ItemChangedイベントが発生するときに呼び出されるメソッドです。
		/// </summary>
		/// <param name="e"></param>
        protected void OnItemChanged(EventArgs e) {
			if (this.ItemChanged != null) {
				this.ItemChanged(this,e);
			}
		}
        /// <summary>
        /// 格納している要素に増減があった時に発生するイベントです。
        /// 要素自体の変更があった場合も本イベントが発生します。
        /// インデクサによる要素の変更等が該当します。
        /// </summary>
        public event EventHandler ItemCountChanged;
        /// <summary>
        /// ItemCountChangedイベントが発生する時に呼び出されるメソッドです。
        /// </summary>
        /// <param name="e"></param>
        protected void OnItemCountChanged(EventArgs e) {
            if (this.ItemCountChanged != null) {
                this.ItemCountChanged(this, e);
            }
        }
		#endregion

		#region インデクサ
        /// <summary>
        /// 指定した位置の要素を取得または設定します。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] {
			get {
				return ((T)(this.List[index]));
			}
			set {
				this.List[index] = value;
			}
		}

		#endregion

		#region メソッド

        /// <summary>
        /// コレクションに要素を追加します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int Add(T value) {
			return this.List.Add(value);
		}
        /// <summary>
        /// コレクションに複数の要素を追加します。
        /// </summary>
        /// <param name="values"></param>
		public void AddRange(IList<T> values) {
			for (int i = 0;i < values.Count;i++) {
				this.Add(values[i]);
			}
		}
        /// <summary>
        /// コレクションから要素を削除します。
        /// </summary>
        /// <param name="value"></param>
		public void Remove(T value) {
			this.List.Remove(value);
		}
        /// <summary>
        /// コレクションの指定した位置に要素を挿入します。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		public void Insert(int index,T value) {
			this.List.Insert(index,value);
		}
        /// <summary>
        /// コレクションに指定した要素が含まれているかどうかを返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public bool Contains(T value) {
			return this.List.Contains(value);
		}
        /// <summary>
        /// 指定した要素がコレクションのどの位置に格納されているかを返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int IndexOf(T value) {
			return this.List.IndexOf(value);
		}
        /// <summary>
        /// コレクションの要素を配列にコピーします。コピーは指定した位置から開始されます。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
		public void CopyTo(Array array,int index) {
			this.List.CopyTo(array,index);
		}
        /// <summary>
        /// コレクションを配列に変換します。
        /// </summary>
        /// <returns></returns>
		public T[] ToArray() {
			T[] array = new T[this.Count];
			this.CopyTo(array,0);
			return array;
		}
				
		#endregion

		#region イベントハンドラ
        /// <summary>
        /// 要素が実装したINotifyインターフェースのChangedイベントを受け取り、ItemChangedイベントを発生させます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void item_Changed(object sender,EventArgs e) {
			this.OnItemPropertyChanged(new CustomCollectionEventArgs((T)sender));
			this.OnItemChanged(e);
		}

		#endregion

		#region On…
        /// <summary>
        /// Clearメソッドによって要素が削除された後に発生するイベントです。
        /// </summary>
		public event EventHandler AfterClear;
        /// <summary>
        /// AfterClearイベントが発生する時に呼び出されるメソッドです。
        /// </summary>
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if (this.AfterClear != null) {
				this.AfterClear(this,new EventArgs());
			}
            this.OnItemCountChanged(new EventArgs());
			this.OnItemChanged(new EventArgs());
		}
        /// <summary>
        /// Clearメソッドによって要素が削除される前に発生するイベントです。
        /// </summary>
		public event EventHandler BeforeClear;
        /// <summary>
        /// BeforeClearイベントが発生する時に呼び出されるメソッドです。
        /// </summary>
		protected override void OnClear() {
			base.OnClear();
			foreach (T item in this) {
				item.Changed -= new EventHandler(item_Changed);
			}
			if (this.BeforeClear != null) {
				this.BeforeClear(this,new EventArgs());
			}
		}
        /// <summary>
        /// Removeメソッド等により要素が削除された後に発生するイベントです。
        /// </summary>
		public event CustomCollectionEventHandler AfterRemove;
        /// <summary>
        /// AfterRemoveイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		protected override void OnRemoveComplete(int index,object value) {
			base.OnRemoveComplete(index,value);
			T item = (T)value;
			item.Changed -= new EventHandler(item_Changed);
			if (this.AfterRemove != null) {
				this.AfterRemove(this,new CustomCollectionEventArgs(item));
			}
            this.OnItemCountChanged(new EventArgs());
            this.OnItemChanged(new EventArgs());
		}
        /// <summary>
        /// Addメソッド等により要素が追加された後に発生するイベントです。
        /// </summary>
		public event CustomCollectionEventHandler AfterInsert;
        /// <summary>
        /// AfterInsertイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		protected override void OnInsertComplete(int index,object value) {
			base.OnInsert(index,value);
			T item = (T)value;
			item.Changed += new EventHandler(item_Changed);
			if (this.AfterInsert != null) {
				this.AfterInsert(this,new CustomCollectionEventArgs(item));
			}
            this.OnItemCountChanged(new EventArgs());
            this.OnItemChanged(new EventArgs());
		}
        /// <summary>
        /// インデクサ等により要素が変更された後に発生するイベントです。
        /// </summary>
		public event CustomCollectionEventHandler2 AfterSet;
        /// <summary>
        /// AfterSetイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
		protected override void OnSetComplete(int index,object oldValue,object newValue) {
			base.OnSet(index,oldValue,newValue);
			T oldItem = (T)oldValue;
			T newItem = (T)newValue;
			oldItem.Changed -= new EventHandler(item_Changed);
			newItem.Changed += new EventHandler(item_Changed);
			if (this.AfterSet != null) {
				this.AfterSet(this,new CustomCollectionEventArgs2(oldItem,newItem));
			}
            this.OnItemCountChanged(new EventArgs());
            this.OnItemChanged(new EventArgs());
		}

		#region CustomCollectionEventArgs
        /// <summary>
        /// 単一の要素に関するイベント引数を表すクラスです。
        /// </summary>
		public class CustomCollectionEventArgs : EventArgs {

			#region メンバ変数
			private T m_Value;
			#endregion

			#region プロパティ
            /// <summary>
            /// 対象となる要素を取得します。
            /// </summary>
			public T Value {
				get {
					return this.m_Value;
				}
			}
			#endregion

			#region コンストラクタ
            /// <summary>
            /// 対象となる要素を指定して、CustomCollectionEventArgsクラスのインスタンスを初期化します。
            /// </summary>
            /// <param name="value"></param>
			public CustomCollectionEventArgs(T value)
				: base() {
				this.m_Value = value;
			}
			#endregion
		}
		#endregion

		#region CustomCollectionEventArgs2
        /// <summary>
        /// 二つの要素に関するイベント引数を表すクラスです。
        /// </summary>
		public class CustomCollectionEventArgs2 : EventArgs {

			#region メンバ変数
			private T m_OldValue;
			private T m_NewValue;
			#endregion

			#region プロパティ
            /// <summary>
            /// 二つの要素のうち、古い要素を取得または設定します。
            /// </summary>
			public T OldValue {
				get {
					return this.m_OldValue;
				}
				set {
					this.m_OldValue = value;
				}
			}
            /// <summary>
            /// 二つの要素のうち、新しい要素を取得または設定します。
            /// </summary>
			public T NewValue {
				get {
					return this.m_NewValue;
				}
				set {
					this.m_NewValue = value;
				}
			}
			#endregion

			#region コンストラクタ
            /// <summary>
            /// 二つの要素を指定してCustomCollectionEventArgs2クラスのインスタンスを初期化します。
            /// </summary>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
			public CustomCollectionEventArgs2(T oldValue,T newValue)
				: base() {
				this.m_OldValue = oldValue;
				this.m_NewValue = newValue;
			}
			#endregion
		}
		#endregion

        /// <summary>
        /// 単一の要素に関するイベントデリゲートを表します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		public delegate void CustomCollectionEventHandler(object sender,CustomCollectionEventArgs e);
        /// <summary>
        /// 二つの要素に関するイベントデリゲートを表します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		public delegate void CustomCollectionEventHandler2(object sender,CustomCollectionEventArgs2 e);

		#endregion

		#region ICollection<T> メンバ

		void ICollection<T>.Add(T item) {
			this.Add(item);
		}
        /// <summary>
        /// コレクションの要素を配列にコピーします。コピーは指定した位置から開始されます。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
		public void CopyTo(T[] array,int arrayIndex) {
			this.List.CopyTo(array,arrayIndex);
		}
        /// <summary>
        /// コレクションか読み取り専用かどうかを取得します。
        /// </summary>
		public bool IsReadOnly {
			get {
				return this.List.IsReadOnly;
			}
		}

		bool ICollection<T>.Remove(T item) {
			if (this.Contains(item)) {
				this.Remove(item);
				return true;
			}
			return false;
		}

		#endregion

		#region IEnumerable<T> メンバ

        /// <summary>
        /// コレクションの列挙子を取得します。
        /// </summary>
        /// <returns></returns>
		public new IEnumerator<T> GetEnumerator() {
			return new TEnumerator(this);
		}

		#region TEnumerator
		private class TEnumerator : IEnumerator<T> {

			#region メンバ変数
			private CustomCollection<T> m_List;
			private int m_CurrentIndex;
			private bool m_Changed;
			#endregion

			#region プロパティ

			#endregion

			#region コンストラクタ
			public TEnumerator(CustomCollection<T> list) {
				list.AfterInsert += new CustomCollectionEventHandler(AfterInsert);
				list.AfterRemove += new CustomCollectionEventHandler(AfterRemove);
				list.AfterSet += new CustomCollectionEventHandler2(AfterSet);
				list.BeforeClear += new EventHandler(BeforeClear);
				this.m_Changed = false;
				this.m_List = list;
				this.m_CurrentIndex = -1;
			}

			void BeforeClear(object sender,EventArgs e) {
				this.m_Changed = true;
			}
			void AfterSet(object sender,CustomCollectionEventArgs2 e) {
				this.m_Changed = true;
			}
			void AfterRemove(object sender,CustomCollectionEventArgs e) {
				this.m_Changed = true;
			}
			void AfterInsert(object sender,CustomCollectionEventArgs e) {
				this.m_Changed = true;
			}
			#endregion

			#region IEnumerator<T> メンバ

			public T Current {
				get {
					if (this.m_CurrentIndex == -1 || this.m_List.Count <= this.m_CurrentIndex) {
						throw new InvalidOperationException();
					}
					return this.m_List[this.m_CurrentIndex];
				}
			}

			#endregion

			#region IDisposable メンバ

			public void Dispose() {
				this.m_List = null;
			}

			#endregion

			#region IEnumerator メンバ

			object System.Collections.IEnumerator.Current {
				get {
					return this.Current;
				}
			}

			public bool MoveNext() {
				if (this.m_Changed) {
					throw new InvalidOperationException();
				}
				if (this.m_List.Count - 1 <= this.m_CurrentIndex) {
					return false;
				}
				this.m_CurrentIndex++;
				return true;
			}

			public void Reset() {
				if (this.m_Changed) {
					throw new InvalidOperationException();
				}
				this.m_CurrentIndex = -1;
			}

			#endregion
		}
		#endregion

		#endregion
	}
	#endregion

	#region INotify
	/// <summary>
	/// 変更通知を行うChangedイベントを実装します。
	/// </summary>
	public interface INotify {
        /// <summary>
        /// 変更を通知するイベントです。
        /// </summary>
		event EventHandler Changed;
	}

	#endregion

}
