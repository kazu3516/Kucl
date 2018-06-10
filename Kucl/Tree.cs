using System;
using System.Collections.Generic;
namespace Kucl {

    #region Tree
    /// <summary>
    /// 任意データを保持する木構造を表すクラスです。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable()]
    public class Tree<T> {

        #region メンバ変数
        private TreeItem<T> m_RootItem;
        #endregion

        #region イベント

        #endregion

        #region プロパティ
        /// <summary>
        /// 木構造データのルート要素を取得します。
        /// </summary>
        public TreeItem<T> RootItem {
            get {
                return this.m_RootItem;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// Treeクラスの新しいインスタンスを初期化します。
        /// </summary>
        public Tree()
            : this(new TreeItem<T>()) {
        }
        /// <summary>
        /// Rootを指定して、Treeクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="root"></param>
        public Tree(TreeItem<T> root) {
            this.m_RootItem = root;
            this.m_RootItem.Level = 0;
        }
        #endregion

        #region Find
        /// <summary>
        /// 指定した値を持つ要素を検索します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TreeItem<T> Find(T value) {
            return this.m_RootItem.Find(value);
        }

        /// <summary>
        /// 指定された述語によって定義された条件と一致する要素を検索し、最初に見つかった要素を返します。
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TreeItem<T> Find(Predicate<TreeItem<T>> predicate) {
            return this.m_RootItem.Find(predicate);
        }
        #endregion

        #region FindAll
        /// <summary>
        /// 指定された述語によって定義された条件と一致する全ての要素を取得します。
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IList<TreeItem<T>> FindAll(Predicate<TreeItem<T>> predicate) {
            return this.m_RootItem.FindAll(predicate);
        }

        #endregion
    }
    #endregion

    #region TreeItem
    /// <summary>
    /// 木構造のアイテムを表すクラスです。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable()]
    public class TreeItem<T> {

        #region メンバ変数

        private TreeItem<T> m_Parent;
        private TreeItemCollection<T> m_Children;
        private T m_Value;
        private int m_Level;
        #endregion

        #region イベント
        /// <summary>
        /// 親要素が変更された場合に発生するイベントです。
        /// </summary>
        public event EventHandler ParentChanged;
        /// <summary>
        /// 親要素が変更された場合に呼び出されます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnParentChanged(EventArgs e) {
            if(this.ParentChanged != null) {
                this.ParentChanged(this,e);
            }
        }
        /// <summary>
        /// 値が変更された場合に発生するイベントです。
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// 値が変更された場合に呼び出されます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnValueChanged(EventArgs e) {
            if(this.ValueChanged != null) {
                this.ValueChanged(this,e);
            }
        }
        /// <summary>
        /// 深さが変更された場合に発生するイベントです。
        /// </summary>
        public event EventHandler LevelChanged;
        /// <summary>
        /// 深さが変更された場合に呼び出されます。
        /// </summary>
        /// <param name="e"></param>
        protected void OnLevelChanged(EventArgs e) {
            if(this.LevelChanged != null) {
                this.LevelChanged(this,e);
            }
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 親要素を取得または設定します。
        /// </summary>
        public TreeItem<T> Parent {
            get {
                return this.m_Parent;
            }
            set {
                if(this.m_Parent != value) {
                    this.m_Parent = value;
                    this.OnParentChanged(new EventArgs());
                }
            }
        }
        /// <summary>
        /// 子要素のコレクションを取得します。
        /// </summary>
        public TreeItemCollection<T> Children {
            get {
                return this.m_Children;
            }
        }
        /// <summary>
        /// 保持する値を取得または設定します。
        /// </summary>
        public T Value {
            get {
                return this.m_Value;
            }
            set {
                if((this.m_Value == null && value != null) || (this.m_Value != null && !this.m_Value.Equals(value))) {
                    this.m_Value = value;
                    this.OnValueChanged(new EventArgs());
                }
            }
        }
        /// <summary>
        /// 0から始まる、このアイテムの深さを表します。
        /// ルートノードは0です。
        /// </summary>
        public int Level {
            get {
                return this.m_Level;
            }
            set {
                if(this.m_Level != value) {
                    this.m_Level = value;
                    this.OnLevelChanged(new EventArgs());
                }
            }
        }


        #endregion

        #region コンストラクタ
        /// <summary>
        /// TreeItemクラスの新しいインスタンスを初期化します。
        /// </summary>
        public TreeItem() {
            this.m_Children = new TreeItemCollection<T>();
			this.m_Children.AfterInsert += new TreeItemCollection<T>.TreeItemCollectionEventHandler(m_Children_AfterInsert);
			this.m_Children.AfterRemove += new TreeItemCollection<T>.TreeItemCollectionEventHandler(m_Children_AfterRemove);
			this.m_Children.AfterSet += new TreeItemCollection<T>.TreeItemCollectionEventHandler2(m_Children_AfterSet);
			this.m_Children.BeforeClear += new EventHandler(m_Children_BeforeClear);
			this.LevelChanged += new EventHandler(TreeItem_LevelChanged);
        }

        /// <summary>
        /// データを指定してTreeItemクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="value"></param>
        public TreeItem(T value)
            : this() {
            this.m_Value = value;
        }
        #endregion

		#region イベントハンドラ
		void m_Children_AfterInsert(object sender,TreeItemCollection<T>.TreeItemCollectionEventArgs e) {
			TreeItem<T> item = e.Value;
			item.Parent = this;
			item.Level = this.Level + 1;
		}
		void m_Children_AfterRemove(object sender,TreeItemCollection<T>.TreeItemCollectionEventArgs e) {
			TreeItem<T> item = e.Value;
			item.Level = int.MinValue;
			item.Parent = null;
		}
		void m_Children_AfterSet(object sender,TreeItemCollection<T>.TreeItemCollectionEventArgs2 e) {
			TreeItem<T> oldItem = e.OldValue;
			TreeItem<T> newItem = e.NewValue;
			oldItem.Level = int.MinValue;
			newItem.Level = this.Level + 1;
			oldItem.Parent = null;
			newItem.Parent = this;
		}

		void m_Children_BeforeClear(object sender,EventArgs e) {
			for (int i = 0;i < this.Children.Count;i++) {
				//複数の親に登録されている場合、子が親と認識している場合を正とする
				if (this.Children[i].Parent == this) {
					this.Children[i].Level = int.MinValue;
				}
			}
		}


		//親のレベルが変更されたら、子のレベルは親のレベル+1となる。
		void TreeItem_LevelChanged(object sender,EventArgs e) {
			for (int i = 0;i < this.Children.Count;i++) {
				//複数の親に登録されている場合、子が親と認識している場合を正とする
				if (this.Children[i].Parent == this) {
					this.Children[i].Level = this.Level + 1;
				}
			}
		}

		#endregion

        #region Find
        /// <summary>
        /// 指定したデータを持つ要素を検索します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TreeItem<T> Find(T value) {
            return this.Find(value, int.MaxValue);
        }

        /// <summary>
        /// 指定したデータを持つ要素を検索します。
        /// 検索を終了するレベルを指定することができます。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="findTargetLevel"></param>
        /// <returns></returns>
        public TreeItem<T> Find(T value, int findTargetLevel) {
            return this.Find(value, findTargetLevel, true);
        }
        /// <summary>
        /// 指定したデータを持つ要素を検索します。
        /// 検索を終了するレベルを指定することができます。
        /// 自分自身を検索対象にするかどうかを指定することができます。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="findTargetLevel"></param>
        /// <param name="containParent"></param>
        /// <returns></returns>
        public TreeItem<T> Find(T value, int findTargetLevel, bool containParent) {
            if (containParent && this.m_Value != null && this.m_Value.Equals(value)) {
                return this;
            }
            if (this.m_Level < findTargetLevel) {
                for (int i = 0; i < this.m_Children.Count; i++) {
                    TreeItem<T> result = this.m_Children[i].Find(value, findTargetLevel);
                    if (result != null) {
                        return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 指定された述語によって定義された条件と一致する要素を検索し、最初に見つかった要素を返します。
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TreeItem<T> Find(Predicate<TreeItem<T>> predicate) {
            if (predicate(this)) {
                return this;
            }
            foreach (TreeItem<T> child in this.m_Children) {
                TreeItem<T> result = child.Find(predicate);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }
        #endregion

        #region FindAll
        /// <summary>
        /// 指定された述語によって定義された条件と一致する全ての要素を取得します。
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IList<TreeItem<T>> FindAll(Predicate<TreeItem<T>> predicate) {
            List<TreeItem<T>> list = new List<TreeItem<T>>();
            this.FindAll(list, predicate);
            return list;
        }

        /// <summary>
        /// 指定された述語によって定義された条件と一致する全ての要素を取得し、指定されたListに格納します。
        /// </summary>
        /// <param name="resultList"></param>
        /// <param name="predicate"></param>
        protected void FindAll(List<TreeItem<T>> resultList, Predicate<TreeItem<T>> predicate) {
            if (predicate(this)) {
                resultList.Add(this);
            }
            foreach (TreeItem<T> child in this.m_Children) {
                child.FindAll(resultList, predicate);
            }
        }

        #endregion
    }
    #endregion

	#region TreeItemCollection
    /// <summary>
    /// 木構造のアイテムを保持するコレクションです。
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[Serializable()]
    public class TreeItemCollection<T> : System.Collections.CollectionBase,IList<TreeItem<T>> {

        /// <summary>
        /// indexを指定してTreeItemオブジェクトを取得または設定するインデクサです。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public TreeItem<T> this[int index] {
			get {
				return ((TreeItem<T>)(this.List[index]));
			}
			set {
				this.List[index] = value;
			}
		}
        /// <summary>
        /// TreeItemオブジェクトをコレクションに追加します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int Add(TreeItem<T> value) {
			return this.List.Add(value);
		}
        /// <summary>
        /// 複数のTreeItemオブジェクトをコレクションに追加します。
        /// </summary>
        /// <param name="values"></param>
		public void AddRange(IList<TreeItem<T>> values) {
			for (int i = 0;i < values.Count;i++) {
				this.Add(values[i]);
			}
		}
        /// <summary>
        /// TreeItemオブジェクトをコレクションから削除します。
        /// </summary>
        /// <param name="value"></param>
		public void Remove(TreeItem<T> value) {
			this.List.Remove(value);
		}
        /// <summary>
        /// 位置を指定して、TreeItemオブジェクトをコレクションに挿入します。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		public void Insert(int index,TreeItem<T> value) {
			this.List.Insert(index,value);
		}
        /// <summary>
        /// TreeItemオブジェクトがコレクションに含まれるかどうかを返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public bool Contains(TreeItem<T> value) {
			return this.List.Contains(value);
		}
        /// <summary>
        /// 指定したTreeItemオブジェクトのコレクション内の位置を返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int IndexOf(TreeItem<T> value) {
			return this.List.IndexOf(value);
		}
        /// <summary>
        /// 指定したindexから、配列へ要素をコピーします。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
		public void CopyTo(Array array,int index) {
			this.List.CopyTo(array,index);
		}
        /// <summary>
        /// コレクション内の全要素を配列にコピーします。
        /// </summary>
        /// <returns></returns>
		public TreeItem<T>[] ToArray() {
			TreeItem<T>[] array = new TreeItem<T>[this.Count];
			this.CopyTo(array,0);
			return array;
		}

		#region On…
        /// <summary>
        /// アイテムに変更が加えられ場合に発生するイベントです。
        /// </summary>
		public event EventHandler ItemChanged;

        /// <summary>
        /// Clearメソッドにより、全要素が削除された後に呼び出されます。
        /// </summary>
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if (this.ItemChanged != null) {
				this.ItemChanged(this,new EventArgs());
			}
		}
        /// <summary>
        /// Clearメソッドにより、全要素が削除される前に発生するイベントです。
        /// </summary>
		public event EventHandler BeforeClear;
        /// <summary>
        /// Clearメソッドにより、全要素が削除される前に呼び出されます。
        /// </summary>
		protected override void OnClear() {
			base.OnClear();
			if (this.BeforeClear != null) {
				this.BeforeClear(this,new EventArgs());
			}
		}
        /// <summary>
        /// TreeItemオブジェクトが削除された後に発生するイベントです。
        /// </summary>
		public event TreeItemCollectionEventHandler AfterRemove;
        /// <summary>
        /// TreeItemオブジェクトが削除された後に呼び出されます。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		protected override void OnRemoveComplete(int index,object value) {
			base.OnRemoveComplete(index,value);
			if (this.AfterRemove != null) {
				this.AfterRemove(this,new TreeItemCollectionEventArgs((TreeItem<T>)value));
			}
			if (this.ItemChanged != null) {
				this.ItemChanged(this,new EventArgs());
			}
		}
        /// <summary>
        /// TreeItemオブジェクトが挿入された後に発生するイベントです。
        /// </summary>
		public event TreeItemCollectionEventHandler AfterInsert;
        /// <summary>
        /// TreeItemオブジェクトが挿入された後に呼び出されます。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		protected override void OnInsertComplete(int index,object value) {
			base.OnInsert(index,value);
			if (this.AfterInsert != null) {
				this.AfterInsert(this,new TreeItemCollectionEventArgs((TreeItem<T>)value));
			}
			if (this.ItemChanged != null) {
				this.ItemChanged(this,new EventArgs());
			}
		}
        /// <summary>
        /// TreeItemオブジェクトが設定された後に発生するイベントです。
        /// </summary>
		public event TreeItemCollectionEventHandler2 AfterSet;
        /// <summary>
        /// TreeItemオブジェクトが設定された後に呼び出されます。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
		protected override void OnSetComplete(int index,object oldValue,object newValue) {
			base.OnSet(index,oldValue,newValue);
			if (this.AfterSet != null) {
				this.AfterSet(this,new TreeItemCollectionEventArgs2((TreeItem<T>)oldValue,(TreeItem<T>)newValue));
			}
			if (this.ItemChanged != null) {
				this.ItemChanged(this,new EventArgs());
			}
		}

		#region TreeItemCollectionEventArgs
        /// <summary>
        /// TreeItemが変更された場合のイベントデータを表すクラスです。
        /// </summary>
		public class TreeItemCollectionEventArgs : EventArgs {

			#region メンバ変数
			private TreeItem<T> m_Value;
			#endregion

			#region プロパティ
            /// <summary>
            /// 変更されたTreeItemオブジェクトを取得します。
            /// </summary>
			public TreeItem<T> Value {
				get {
					return this.m_Value;
				}
			}
			#endregion

			#region コンストラクタ
            /// <summary>
            /// TreeItemCollectionEventArgsクラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="value"></param>
			public TreeItemCollectionEventArgs(TreeItem<T> value)
				: base() {
				this.m_Value = value;
			}
			#endregion
		}
		#endregion

		#region TreeItemCollectionEventArgs2
        /// <summary>
        /// TreeItemが変更された場合のイベントデータを表すクラスです。
        /// </summary>
		public class TreeItemCollectionEventArgs2 : EventArgs {

			#region メンバ変数
			private TreeItem<T> m_OldValue;
			private TreeItem<T> m_NewValue;
			#endregion

			#region プロパティ
            /// <summary>
            /// 変更前のTreeItemオブジェクトを取得します。
            /// </summary>
            public TreeItem<T> OldValue {
				get {
					return this.m_OldValue;
				}
				set {
					this.m_OldValue = value;
				}
			}
            /// <summary>
            /// 変更後のTreeItemオブジェクトを取得します。
            /// </summary>
            public TreeItem<T> NewValue {
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
            /// TreeItemCollectionEventArgs2クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
			public TreeItemCollectionEventArgs2(TreeItem<T> oldValue,TreeItem<T> newValue)
				: base() {
				this.m_OldValue = oldValue;
				this.m_NewValue = newValue;
			}
			#endregion
		}
		#endregion

        /// <summary>
        /// イベントを処理するメソッドを表します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		public delegate void TreeItemCollectionEventHandler(object sender,TreeItemCollectionEventArgs e);
        /// <summary>
        /// イベントを処理するメソッドを表します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TreeItemCollectionEventHandler2(object sender, TreeItemCollectionEventArgs2 e);

		#endregion


		#region ICollection<TreeItem<T>> メンバ

		void ICollection<TreeItem<T>>.Add(TreeItem<T> item) {
			this.Add(item);
		}
        /// <summary>
        /// indexを指定して、要素を配列にコピーします。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
		public void CopyTo(TreeItem<T>[] array,int arrayIndex) {
			this.List.CopyTo(array,arrayIndex);
		}
        /// <summary>
        /// コレクションが読み取り専用かどうかを取得します。
        /// </summary>
		public bool IsReadOnly {
			get {
				return this.List.IsReadOnly;
			}
		}

		bool ICollection<TreeItem<T>>.Remove(TreeItem<T> item) {
			if (this.Contains(item)) {
				this.Remove(item);
				return true;
			}
			return false;
		}

		#endregion

		#region IEnumerable<TreeItem<T>> メンバ
        /// <summary>
        /// コレクションを反復処理する列挙子を返します。 
        /// </summary>
        /// <returns></returns>
		public new IEnumerator<TreeItem<T>> GetEnumerator() {
			return new TreeItemEnumerator(this);
		}

		#region TreeItemEnumerator
		private class TreeItemEnumerator : IEnumerator<TreeItem<T>> {

			#region メンバ変数
			private TreeItemCollection<T> m_List;
			private int m_CurrentIndex;
			private bool m_Changed;
			#endregion

			#region プロパティ

			#endregion

			#region コンストラクタ
			public TreeItemEnumerator(TreeItemCollection<T> list) {
				list.AfterInsert += new TreeItemCollectionEventHandler(this.AfterInsert);
				list.AfterRemove += new TreeItemCollectionEventHandler(this.AfterRemove);
				list.AfterSet += new TreeItemCollectionEventHandler2(this.AfterSet);
				list.BeforeClear += new EventHandler(this.BeforeClear);
				this.m_Changed = false;
				this.m_List = list;
				this.m_CurrentIndex = -1;
			}

			void BeforeClear(object sender,EventArgs e) {
				this.m_Changed = true;
			}
			void AfterSet(object sender,TreeItemCollectionEventArgs2 e) {
				this.m_Changed = true;
			}
			void AfterRemove(object sender,TreeItemCollectionEventArgs e) {
				this.m_Changed = true;
			}
			void AfterInsert(object sender,TreeItemCollectionEventArgs e) {
				this.m_Changed = true;
			}
			#endregion

			#region IEnumerator<TreeItem<T>> メンバ

			public TreeItem<T> Current {
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

}