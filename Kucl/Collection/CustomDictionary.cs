using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Kucl.Collections {

	#region CustomDictionary

    /// <summary>
    /// 要素の変更を管理するハッシュテーブルです。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue">INotifyインターフェースを実装する型</typeparam>
    [Serializable()]    
    public class CustomDictionary<TKey, TValue> : System.Collections.DictionaryBase, IDictionary<TKey, TValue> where TValue : INotify {

		#region イベント

        /// <summary>
        /// 格納している要素のPropertyが変更された時に発生するイベントです。
        /// このイベントは格納されている要素のINotify.Changedイベントによって発生します。
        /// </summary>
        public event CustomDictionaryEventHandler ItemPropertyChanged;
        /// <summary>
        /// ItemPropertyChangedイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="e"></param>
        protected void OnItemPropertyChanged(CustomDictionaryEventArgs e) {
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
		#endregion

		#region プロパティ

		#endregion

		#region コンストラクタ
        /// <summary>
        /// CustomDictionaryクラスの新しいインスタンスを初期化します。
        /// </summary>
		public CustomDictionary() {
		}
		#endregion

		#region イベントハンドラ
		private void item_Changed(object sender,EventArgs e) {
			TKey key = default(TKey);
			foreach (KeyValuePair<TKey,TValue> pair in this) {
				TValue value1 = (TValue)sender;
				if (pair.Value.Equals(value1)) {
					key = pair.Key;
				}
			}
			if (key.Equals(default(TKey))) {
				//コレクションに含まれる要素ではない場合、イベントは捨てる。
				return;
			}
			this.OnItemPropertyChanged(new CustomDictionaryEventArgs(key,(TValue)sender));
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
			foreach (KeyValuePair<TKey,TValue> item in this) {
				item.Value.Changed -= new EventHandler(item_Changed);
			}
			if (this.BeforeClear != null) {
				this.BeforeClear(this,new EventArgs());
			}
		}

        /// <summary>
        /// Removeメソッド等により要素が削除された後に発生するイベントです。
        /// </summary>
        public event CustomDictionaryEventHandler AfterRemove;
        /// <summary>
        /// AfterRemoveイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected override void OnRemoveComplete(object key,object value) {
			base.OnRemoveComplete(key,value);
			TValue item = (TValue)value;
			TKey key1 = (TKey)key;
			item.Changed -= new EventHandler(item_Changed);
			if (this.AfterRemove != null) {
				this.AfterRemove(this,new CustomDictionaryEventArgs(key1,item));
			}
			this.OnItemChanged(new EventArgs());
		}
        /// <summary>
        /// Addメソッド等により要素が追加された後に発生するイベントです。
        /// </summary>
        public event CustomDictionaryEventHandler AfterInsert;
        /// <summary>
        /// AfterInsertイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected override void OnInsertComplete(object key,object value) {
			base.OnInsert(key,value);
			TKey key1 = (TKey)key;
			TValue item = (TValue)value;
			item.Changed += new EventHandler(item_Changed);
			if (this.AfterInsert != null) {
				this.AfterInsert(this,new CustomDictionaryEventArgs(key1,item));
			}
			this.OnItemChanged(new EventArgs());
		}
        /// <summary>
        /// インデクサ等により要素が変更された後に発生するイベントです。
        /// </summary>
        public event CustomDictionaryEventHandler2 AfterSet;
        /// <summary>
        /// AfterSetイベントが発生するときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnSetComplete(object key,object oldValue,object newValue) {
			base.OnSet(key,oldValue,newValue);
			TKey key1 = (TKey)key;
			TValue oldItem = (TValue)oldValue;
			TValue newItem = (TValue)newValue;
			oldItem.Changed -= new EventHandler(item_Changed);
			newItem.Changed += new EventHandler(item_Changed);
			if (this.AfterSet != null) {
				this.AfterSet(this,new CustomDictionaryEventArgs2(key1,oldItem,newItem));
			}
			this.OnItemChanged(new EventArgs());
		}

		#region CustomDictionaryEventArgs
        /// <summary>
        /// 単一の要素に関するイベント引数を表すクラスです。
        /// </summary>
        public class CustomDictionaryEventArgs:EventArgs {

			#region メンバ変数
			private TKey m_Key;
			private TValue m_Value;
			#endregion

			#region プロパティ
            /// <summary>
            /// 対象となるキーを取得します。
            /// </summary>
            public TKey Key {
				get {
					return this.m_Key;
				}
			}
            /// <summary>
            /// 対象となる要素を取得します。
            /// </summary>
            public TValue Value {
				get {
					return this.m_Value;
				}
			}
			#endregion

			#region コンストラクタ
            #region コンストラクタ

            /// <summary>
            /// 対象となる要素tとキーを指定して、CustomDictionaryEventArgsクラスのインスタンスを初期化します。
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public CustomDictionaryEventArgs(TKey key,TValue value)
				: base() {
				this.m_Key = key;
				this.m_Value = value;
			}
			#endregion
		}
		#endregion

        #endregion

        #region CustomDictionaryEventArgs2
        /// <summary>
        /// 二つの要素に関するイベント引数を表すクラスです。
        /// </summary>
        public class CustomDictionaryEventArgs2:EventArgs {

			#region メンバ変数
			private TKey m_Key;
			private TValue m_OldValue;
			private TValue m_NewValue;
			#endregion

			#region プロパティ
            /// <summary>
            /// 対象となるキーを取得または設定します。
            /// </summary>
            public TKey Key {
				get {
					return this.m_Key;
				}
			}
            /// <summary>
            /// 二つの要素のうち、古い要素を取得または設定します。
            /// </summary>
            public TValue OldValue {
				get {
					return this.m_OldValue;
				}
			}
            /// <summary>
            /// 二つの要素のうち、新しい要素を取得または設定します。
            /// </summary>
            public TValue NewValue {
				get {
					return this.m_NewValue;
				}
			}
			#endregion

			#region コンストラクタ
            /// <summary>
            /// 二つの要素とキーを指定してCustomDictionaryEventArgs2クラスのインスタンスを初期化します。
            /// </summary>
            /// <param name="key"></param>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
            public CustomDictionaryEventArgs2(TKey key,TValue oldValue,TValue newValue)
				: base() {
				this.m_Key = key;
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
        public delegate void CustomDictionaryEventHandler(object sender,CustomDictionaryEventArgs e);
        /// <summary>
        /// 二つの要素に関するイベントデリゲートを表します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CustomDictionaryEventHandler2(object sender,CustomDictionaryEventArgs2 e);

		#endregion


		#region IDictionary<TKey,TValue> メンバ
        /// <summary>
        /// キーを指定して要素を追加します。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
		public void Add(TKey key,TValue value) {
			this.Dictionary.Add(key,value);
		}
        /// <summary>
        /// 指定したキーが含まれているかどうかを返します。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public bool ContainsKey(TKey key) {
			return this.Dictionary.Contains(key);
		}
        /// <summary>
        /// 全てのキーのコレクションを取得します。
        /// </summary>
		public ICollection<TKey> Keys {
			get {
				ICollection<TKey> keys = new List<TKey>();
				foreach (TKey key in this.Dictionary.Keys) {
					keys.Add(key);
				}
				return keys;
			}
		}
        /// <summary>
        /// キーを指定して要素の削除を行います。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public bool Remove(TKey key) {
			if (this.ContainsKey(key)) {
				this.Dictionary.Remove(key);
				return true;
			}
			return false;
		}
        /// <summary>
        /// キーを指定して要素を取得します。
        /// 存在しないキーを指定した場合、falseを返し、valueには既定値がセットされます。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		public bool TryGetValue(TKey key,out TValue value) {
			if (key == null) {
				throw new ArgumentNullException();
			}
			if (this.ContainsKey(key)) {
				value = (TValue)this.Dictionary[key];
				return true;
			}
			value = default(TValue);
			return false;
		}
        /// <summary>
        /// 要素のコレクションを取得します。
        /// </summary>
		public ICollection<TValue> Values {
			get {
				ICollection<TValue> values = new List<TValue>();
				foreach (TValue value in this.Dictionary.Values) {
					values.Add(value);
				}
				return values;
			}
		}
        /// <summary>
        /// キーを指定して要素を取得または設定するインデクサです。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public TValue this[TKey key] {
			get {
				return (TValue)this.Dictionary[key];
			}
			set {
				this.Dictionary[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> メンバ

        /// <summary>
        /// キーを指定して要素を追加します。
        /// </summary>
        /// <param name="item"></param>
		public void Add(KeyValuePair<TKey,TValue> item) {
			this.Add(item.Key,item.Value);
		}
        /// <summary>
        /// 指定したキーと要素が服荒れているかどうかを返します。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
		public bool Contains(KeyValuePair<TKey,TValue> item) {
			return this.ContainsKey(item.Key);
		}
        /// <summary>
        /// キーと要素を配列にコピーします。コピーは指定した位置から開始されます。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
		public void CopyTo(KeyValuePair<TKey,TValue>[] array,int arrayIndex) {
			object[] array1 = new object[this.Dictionary.Count - arrayIndex];
			this.Dictionary.CopyTo(array1,arrayIndex);
			for (int i = 0;i < array1.Length;i++) {
				DictionaryEntry DE = (DictionaryEntry)array1[i];
				array[i] = new KeyValuePair<TKey,TValue>((TKey)DE.Key,(TValue)DE.Value);
			}

		}

        /// <summary>
        /// コレクションか読み取り専用かどうかを取得します。
        /// </summary>
        public bool IsReadOnly {
			get {
				return this.Dictionary.IsReadOnly;
			}
		}
        /// <summary>
        /// キーと要素を削除します。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
		public bool Remove(KeyValuePair<TKey,TValue> item) {
			if (this.ContainsKey(item.Key)) {
				this.Remove(item.Key);
				return true;
			}
			return false;
		}

		#endregion


		#region IEnumerable<KeyValuePair<TKey,TValue>> メンバ
        /// <summary>
        /// 列挙子を返します。
        /// </summary>
        /// <returns></returns>
		public new IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator() {
			return new T2Enumerator(this);
		}

		#endregion

		#region T2Enumerator
		private class T2Enumerator : IEnumerator<KeyValuePair<TKey,TValue>> {

			#region メンバ変数
			private CustomDictionary<TKey,TValue> m_List;
			private TKey[] m_Keys;
			private int m_CurrentIndex;
			private bool m_Changed;
			#endregion

			#region プロパティ

			#endregion

			#region コンストラクタ
			public T2Enumerator(CustomDictionary<TKey,TValue> list) {
				list.AfterInsert += new CustomDictionaryEventHandler(AfterInsert);
				list.AfterRemove += new CustomDictionaryEventHandler(AfterRemove);
				list.AfterSet += new CustomDictionaryEventHandler2(AfterSet);
				list.BeforeClear += new EventHandler(BeforeClear);
				this.m_Changed = false;
				this.m_List = list;
				this.m_CurrentIndex = -1;
				this.m_Keys = new TKey[this.m_List.Keys.Count];
				this.m_List.Keys.CopyTo(this.m_Keys,0);
			}

			void BeforeClear(object sender,EventArgs e) {
				this.m_Changed = true;
			}
			void AfterSet(object sender,CustomDictionaryEventArgs2 e) {
				this.m_Changed = true;
			}
			void AfterRemove(object sender,CustomDictionaryEventArgs e) {
				this.m_Changed = true;
			}
			void AfterInsert(object sender,CustomDictionaryEventArgs e) {
				this.m_Changed = true;
			}
			#endregion

			#region IEnumerator<KeyValuePair<Tkey,Tvalue>> メンバ

			public KeyValuePair<TKey,TValue> Current {
				get {
					if (this.m_CurrentIndex == -1 || this.m_List.Count <= this.m_CurrentIndex) {
						throw new InvalidOperationException();
					}
					TKey key = this.m_Keys[this.m_CurrentIndex];
					TValue value = this.m_List[key];
					return new KeyValuePair<TKey,TValue>(key,value);
				}
			}

			#endregion

			#region IDisposable メンバ

			public void Dispose() {
				this.m_List = null;
				this.m_Keys = null;
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

	}
	#endregion

}
