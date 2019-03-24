using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kucl.Collections {

    #region ObservableList
    /// <summary>
    /// 要素の変更を監視できるコレクションです。
    /// </summary>
    public class ObservableList<T> : IList<T> where T : INotifyPropertyChanged {

        #region 移譲するList<T>
        private List<T> List {
            get;
        }
        #endregion

        #region イベント
        /// <summary>
        /// 要素に変更が加えられた場合に発生するイベントです。
        /// </summary>
        public event EventHandler ItemChanged;
        /// <summary>
        /// Clearメソッドによりコレクションの全要素が削除される前に発生するイベントです。
        /// </summary>
        public event EventHandler BeforeClear;
        /// <summary>
        /// 要素が削除された後に発生するイベントです。
        /// </summary>
        public event ObservableListEventHandler AfterRemove;
        /// <summary>
        /// 要素が追加された後に発生するイベントです。
        /// </summary>
        public event ObservableListEventHandler AfterInsert;
        /// <summary>
        /// 要素が設定された後に発生するイベントです。
        /// </summary>
        public event ObservableListEventHandler2 AfterSet;


        /// <summary>
        /// コレクションに格納されているオブジェクトのPropertyChangedイベントが発生した時に発生するイベントです。
        /// </summary>
        public event PropertyChangedEventHandler CollectionItemPropertyChanged;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// ObservableListオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        public ObservableList() {
            this.List = new List<T>();
            this.Initialize();
        }
        /// <summary>
        /// ObservableListオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="capacity"></param>
        public ObservableList(int capacity) {
            this.List = new List<T>(capacity);
            this.Initialize();
        }
        /// <summary>
        /// ObservableListオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="collection"></param>
        public ObservableList(IEnumerable<T> collection) {
            this.List = new List<T>(collection);
            this.Initialize();
        }

        private void Initialize() {
            this.BeforeClear += this.ObservableList_BeforeClear;
            this.AfterInsert += this.ObservableList_AfterInsert;
            this.AfterRemove += this.ObservableList_AfterRemove;
            this.AfterSet += this.ObservableList_AfterSet;
        }
        #endregion

        #region イベントハンドラ
        private void ObservableList_AfterSet(object sender, ObservableListEventArgs2 e) {
            var item1 = e.OldValue;
            if (item1 != null) {
                item1.PropertyChanged += this.Item_PropertyChanged;
            }
            var item2 = e.NewValue;
            if (item2 != null) {
                item2.PropertyChanged += this.Item_PropertyChanged;
            }
        }

        private void ObservableList_AfterRemove(object sender, ObservableListEventArgs e) {
            var item = e.Value;
            item.PropertyChanged -= this.Item_PropertyChanged;
        }

        private void ObservableList_AfterInsert(object sender, ObservableListEventArgs e) {
            var item = e.Value;
            item.PropertyChanged += this.Item_PropertyChanged;
        }

        private void ObservableList_BeforeClear(object sender, EventArgs e) {
            this.List.ForEach(item => {
                item.PropertyChanged -= this.Item_PropertyChanged;
            });
        }


        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.CollectionItemPropertyChanged?.Invoke(sender, e);
        }
        #endregion

        #region インデクサ
        /// <summary>
        /// indexによって指定したオブジェクトを取得または設定するインデクサです。
        /// </summary>
        /// <param name="index">型 : System.Int32
        ///<para>取得または設定する要素の 0 から始まるインデックス。</para>
        /// </param>
        /// <returns>指定したインデックスにあるオブジェクト。</returns>
        public T this[int index] {
            get {
                return this.List[index];
            }
            set {
                T oldValue = this.List[index];
                this.List[index] = value;
                this.OnSetComplete(index, oldValue, value);
            }
        } 
        #endregion

        #region Add
        /// <summary>
        /// オブジェクトをコレクションに追加します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Add(T value) {
            this.List.Add(value);
            this.OnInsertComplete(this.Count - 1, value);
        }
        /// <summary>
        /// 複数のオブジェクトをコレクションに追加します。
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(IList<T> values) {
            for (int i = 0; i < values.Count; i++) {
                this.Add(values[i]);
            }
        }
        /// <summary>
        /// 位置を指定して、オブジェクトをコレクションに挿入します。
        /// </summary>
        public void Insert(int index, T value) {
            this.List.Insert(index, value);
            this.OnInsertComplete(index, value);
        }
        #endregion

        #region Remove
        /// <summary>
        /// オブジェクトをコレクションから削除します。
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value) {
            int index = this.List.IndexOf(value);
            this.List.Remove(value);
            this.OnRemoveComplete(index, value);
        }
        bool ICollection<T>.Remove(T item) {
            if (this.Contains(item)) {
                this.Remove(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// インデックスを指定して、オブジェクトをコレクションから削除します。
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) {
            T value = this.List[index];
            this.List.RemoveAt(index);
            this.OnRemoveComplete(index, value);
        }
        #endregion

        /// <summary>
        /// このコレクションに格納されているオブジェクトをすべて削除します。
        /// </summary>
        public void Clear() {
            this.OnClear();
            this.List.Clear();
            this.OnClearComplete();
        }

        /// <summary>
        /// オブジェクトがコレクションに含まれているかどうかを返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value) {
            return this.List.Contains(value);
        }
        /// <summary>
        /// 指定したオブジェクトのコレクション内の位置を返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(T value) {
            return this.List.IndexOf(value);
        }
        /// <summary>
        /// コレクション内の全要素を配列にコピーします。
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() {
            T[] array = new T[this.Count()];
            this.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// 指定したindexからオブジェクトを配列にコピーします。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex) {
            this.List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// このコレクションに格納されているオブジェクトの個数を取得します。
        /// </summary>
        public int Count {
            get {
                return this.List.Count;
            }
        }

        bool ICollection<T>.IsReadOnly {
            get {
                return ((IList<T>)this.List).IsReadOnly;
            }
        }

        #region On…

        /// <summary>
        /// Clearメソッドによりコレクションの全要素が削除された後に呼び出されます。
        /// </summary>
        protected virtual void OnClearComplete() {
            this.ItemChanged?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Clearメソッドによりコレクションの全要素が削除される前に呼び出されます。
        /// </summary>
        protected virtual void OnClear() {
            this.BeforeClear?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// 要素が削除された後に呼び出されます。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected virtual void OnRemoveComplete(int index, object value) {
            this.AfterRemove?.Invoke(this, new ObservableListEventArgs((T)value));
            this.ItemChanged?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// 要素が追加された後に呼び出されます。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected virtual void OnInsertComplete(int index, object value) {
            this.AfterInsert?.Invoke(this, new ObservableListEventArgs((T)value));
            this.ItemChanged?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// 要素が設定された後に呼び出されます。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnSetComplete(int index, object oldValue, object newValue) {
            this.AfterSet?.Invoke(this, new ObservableListEventArgs2((T)oldValue, (T)newValue));
            this.ItemChanged?.Invoke(this, new EventArgs());
        }

        #region ObservableListEventArgs
        /// <summary>
        /// イベントデータが格納されているクラスです。 
        /// </summary>
        public class ObservableListEventArgs : EventArgs {

            #region プロパティ
            /// <summary>
            /// イベント対象のオブジェクトを取得します。
            /// </summary>
            public T Value { get; }
            #endregion

            #region コンストラクタ
            /// <summary>
            /// ObservableListEventArgsオブジェクトを初期化します。
            /// </summary>
            /// <param name="value"></param>
            public ObservableListEventArgs(T value)
                : base() {
                this.Value = value;
            }
            #endregion
        }
        #endregion

        #region ObservableListEventArgs2
        /// <summary>
        /// イベントデータが格納されているクラスです。 
        /// </summary>
        public class ObservableListEventArgs2 : EventArgs {


            #region プロパティ
            /// <summary>
            /// イベント対象の変更前オブジェクトを取得します。
            /// </summary>
            public T OldValue { get; }
            /// <summary>
            /// イベント対象の変更後オブジェクトを取得します。
            /// </summary>
            public T NewValue { get; }
            #endregion

            #region コンストラクタ
            /// <summary>
            /// ObservableListEventArgs2オブジェクトを初期化します。
            /// </summary>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
            public ObservableListEventArgs2(T oldValue, T newValue)
                : base() {
                this.OldValue = oldValue;
                this.NewValue = newValue;
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// イベントを処理するメソッドを表します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ObservableListEventHandler(object sender, ObservableListEventArgs e);
        /// <summary>
        /// イベントを処理するメソッドを表します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ObservableListEventHandler2(object sender, ObservableListEventArgs2 e);

        #endregion

        #region GetEnumerator
        /// <summary>
        /// コレクションを反復処理する列挙子を返します。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() {
            return this.List.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.List.GetEnumerator();
        }
        #endregion

    }
    #endregion


}
