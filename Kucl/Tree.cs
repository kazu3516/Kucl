using System;
using System.Collections.Generic;
namespace Kucl {

    #region Tree
    /// <summary>
    /// �C�Ӄf�[�^��ێ�����؍\����\���N���X�ł��B
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable()]
    public class Tree<T> {

        #region �����o�ϐ�
        private TreeItem<T> m_RootItem;
        #endregion

        #region �C�x���g

        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �؍\���f�[�^�̃��[�g�v�f���擾���܂��B
        /// </summary>
        public TreeItem<T> RootItem {
            get {
                return this.m_RootItem;
            }
        }
        #endregion

        #region �R���X�g���N�^
        /// <summary>
        /// Tree�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        public Tree()
            : this(new TreeItem<T>()) {
        }
        /// <summary>
        /// Root���w�肵�āATree�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="root"></param>
        public Tree(TreeItem<T> root) {
            this.m_RootItem = root;
            this.m_RootItem.Level = 0;
        }
        #endregion

        #region Find
        /// <summary>
        /// �w�肵���l�����v�f���������܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TreeItem<T> Find(T value) {
            return this.m_RootItem.Find(value);
        }

        /// <summary>
        /// �w�肳�ꂽ�q��ɂ���Ē�`���ꂽ�����ƈ�v����v�f���������A�ŏ��Ɍ��������v�f��Ԃ��܂��B
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TreeItem<T> Find(Predicate<TreeItem<T>> predicate) {
            return this.m_RootItem.Find(predicate);
        }
        #endregion

        #region FindAll
        /// <summary>
        /// �w�肳�ꂽ�q��ɂ���Ē�`���ꂽ�����ƈ�v����S�Ă̗v�f���擾���܂��B
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
    /// �؍\���̃A�C�e����\���N���X�ł��B
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable()]
    public class TreeItem<T> {

        #region �����o�ϐ�

        private TreeItem<T> m_Parent;
        private TreeItemCollection<T> m_Children;
        private T m_Value;
        private int m_Level;
        #endregion

        #region �C�x���g
        /// <summary>
        /// �e�v�f���ύX���ꂽ�ꍇ�ɔ�������C�x���g�ł��B
        /// </summary>
        public event EventHandler ParentChanged;
        /// <summary>
        /// �e�v�f���ύX���ꂽ�ꍇ�ɌĂяo����܂��B
        /// </summary>
        /// <param name="e"></param>
        protected void OnParentChanged(EventArgs e) {
            if(this.ParentChanged != null) {
                this.ParentChanged(this,e);
            }
        }
        /// <summary>
        /// �l���ύX���ꂽ�ꍇ�ɔ�������C�x���g�ł��B
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// �l���ύX���ꂽ�ꍇ�ɌĂяo����܂��B
        /// </summary>
        /// <param name="e"></param>
        protected void OnValueChanged(EventArgs e) {
            if(this.ValueChanged != null) {
                this.ValueChanged(this,e);
            }
        }
        /// <summary>
        /// �[�����ύX���ꂽ�ꍇ�ɔ�������C�x���g�ł��B
        /// </summary>
        public event EventHandler LevelChanged;
        /// <summary>
        /// �[�����ύX���ꂽ�ꍇ�ɌĂяo����܂��B
        /// </summary>
        /// <param name="e"></param>
        protected void OnLevelChanged(EventArgs e) {
            if(this.LevelChanged != null) {
                this.LevelChanged(this,e);
            }
        }
        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �e�v�f���擾�܂��͐ݒ肵�܂��B
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
        /// �q�v�f�̃R���N�V�������擾���܂��B
        /// </summary>
        public TreeItemCollection<T> Children {
            get {
                return this.m_Children;
            }
        }
        /// <summary>
        /// �ێ�����l���擾�܂��͐ݒ肵�܂��B
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
        /// 0����n�܂�A���̃A�C�e���̐[����\���܂��B
        /// ���[�g�m�[�h��0�ł��B
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

        #region �R���X�g���N�^
        /// <summary>
        /// TreeItem�N���X�̐V�����C���X�^���X�����������܂��B
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
        /// �f�[�^���w�肵��TreeItem�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="value"></param>
        public TreeItem(T value)
            : this() {
            this.m_Value = value;
        }
        #endregion

		#region �C�x���g�n���h��
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
				//�����̐e�ɓo�^����Ă���ꍇ�A�q���e�ƔF�����Ă���ꍇ�𐳂Ƃ���
				if (this.Children[i].Parent == this) {
					this.Children[i].Level = int.MinValue;
				}
			}
		}


		//�e�̃��x�����ύX���ꂽ��A�q�̃��x���͐e�̃��x��+1�ƂȂ�B
		void TreeItem_LevelChanged(object sender,EventArgs e) {
			for (int i = 0;i < this.Children.Count;i++) {
				//�����̐e�ɓo�^����Ă���ꍇ�A�q���e�ƔF�����Ă���ꍇ�𐳂Ƃ���
				if (this.Children[i].Parent == this) {
					this.Children[i].Level = this.Level + 1;
				}
			}
		}

		#endregion

        #region Find
        /// <summary>
        /// �w�肵���f�[�^�����v�f���������܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TreeItem<T> Find(T value) {
            return this.Find(value, int.MaxValue);
        }

        /// <summary>
        /// �w�肵���f�[�^�����v�f���������܂��B
        /// �������I�����郌�x�����w�肷�邱�Ƃ��ł��܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <param name="findTargetLevel"></param>
        /// <returns></returns>
        public TreeItem<T> Find(T value, int findTargetLevel) {
            return this.Find(value, findTargetLevel, true);
        }
        /// <summary>
        /// �w�肵���f�[�^�����v�f���������܂��B
        /// �������I�����郌�x�����w�肷�邱�Ƃ��ł��܂��B
        /// �������g�������Ώۂɂ��邩�ǂ������w�肷�邱�Ƃ��ł��܂��B
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
        /// �w�肳�ꂽ�q��ɂ���Ē�`���ꂽ�����ƈ�v����v�f���������A�ŏ��Ɍ��������v�f��Ԃ��܂��B
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
        /// �w�肳�ꂽ�q��ɂ���Ē�`���ꂽ�����ƈ�v����S�Ă̗v�f���擾���܂��B
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IList<TreeItem<T>> FindAll(Predicate<TreeItem<T>> predicate) {
            List<TreeItem<T>> list = new List<TreeItem<T>>();
            this.FindAll(list, predicate);
            return list;
        }

        /// <summary>
        /// �w�肳�ꂽ�q��ɂ���Ē�`���ꂽ�����ƈ�v����S�Ă̗v�f���擾���A�w�肳�ꂽList�Ɋi�[���܂��B
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
    /// �؍\���̃A�C�e����ێ�����R���N�V�����ł��B
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[Serializable()]
    public class TreeItemCollection<T> : System.Collections.CollectionBase,IList<TreeItem<T>> {

        /// <summary>
        /// index���w�肵��TreeItem�I�u�W�F�N�g���擾�܂��͐ݒ肷��C���f�N�T�ł��B
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
        /// TreeItem�I�u�W�F�N�g���R���N�V�����ɒǉ����܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int Add(TreeItem<T> value) {
			return this.List.Add(value);
		}
        /// <summary>
        /// ������TreeItem�I�u�W�F�N�g���R���N�V�����ɒǉ����܂��B
        /// </summary>
        /// <param name="values"></param>
		public void AddRange(IList<TreeItem<T>> values) {
			for (int i = 0;i < values.Count;i++) {
				this.Add(values[i]);
			}
		}
        /// <summary>
        /// TreeItem�I�u�W�F�N�g���R���N�V��������폜���܂��B
        /// </summary>
        /// <param name="value"></param>
		public void Remove(TreeItem<T> value) {
			this.List.Remove(value);
		}
        /// <summary>
        /// �ʒu���w�肵�āATreeItem�I�u�W�F�N�g���R���N�V�����ɑ}�����܂��B
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
		public void Insert(int index,TreeItem<T> value) {
			this.List.Insert(index,value);
		}
        /// <summary>
        /// TreeItem�I�u�W�F�N�g���R���N�V�����Ɋ܂܂�邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public bool Contains(TreeItem<T> value) {
			return this.List.Contains(value);
		}
        /// <summary>
        /// �w�肵��TreeItem�I�u�W�F�N�g�̃R���N�V�������̈ʒu��Ԃ��܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int IndexOf(TreeItem<T> value) {
			return this.List.IndexOf(value);
		}
        /// <summary>
        /// �w�肵��index����A�z��֗v�f���R�s�[���܂��B
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
		public void CopyTo(Array array,int index) {
			this.List.CopyTo(array,index);
		}
        /// <summary>
        /// �R���N�V�������̑S�v�f��z��ɃR�s�[���܂��B
        /// </summary>
        /// <returns></returns>
		public TreeItem<T>[] ToArray() {
			TreeItem<T>[] array = new TreeItem<T>[this.Count];
			this.CopyTo(array,0);
			return array;
		}

		#region On�c
        /// <summary>
        /// �A�C�e���ɕύX���������ꍇ�ɔ�������C�x���g�ł��B
        /// </summary>
		public event EventHandler ItemChanged;

        /// <summary>
        /// Clear���\�b�h�ɂ��A�S�v�f���폜���ꂽ��ɌĂяo����܂��B
        /// </summary>
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if (this.ItemChanged != null) {
				this.ItemChanged(this,new EventArgs());
			}
		}
        /// <summary>
        /// Clear���\�b�h�ɂ��A�S�v�f���폜�����O�ɔ�������C�x���g�ł��B
        /// </summary>
		public event EventHandler BeforeClear;
        /// <summary>
        /// Clear���\�b�h�ɂ��A�S�v�f���폜�����O�ɌĂяo����܂��B
        /// </summary>
		protected override void OnClear() {
			base.OnClear();
			if (this.BeforeClear != null) {
				this.BeforeClear(this,new EventArgs());
			}
		}
        /// <summary>
        /// TreeItem�I�u�W�F�N�g���폜���ꂽ��ɔ�������C�x���g�ł��B
        /// </summary>
		public event TreeItemCollectionEventHandler AfterRemove;
        /// <summary>
        /// TreeItem�I�u�W�F�N�g���폜���ꂽ��ɌĂяo����܂��B
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
        /// TreeItem�I�u�W�F�N�g���}�����ꂽ��ɔ�������C�x���g�ł��B
        /// </summary>
		public event TreeItemCollectionEventHandler AfterInsert;
        /// <summary>
        /// TreeItem�I�u�W�F�N�g���}�����ꂽ��ɌĂяo����܂��B
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
        /// TreeItem�I�u�W�F�N�g���ݒ肳�ꂽ��ɔ�������C�x���g�ł��B
        /// </summary>
		public event TreeItemCollectionEventHandler2 AfterSet;
        /// <summary>
        /// TreeItem�I�u�W�F�N�g���ݒ肳�ꂽ��ɌĂяo����܂��B
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
        /// TreeItem���ύX���ꂽ�ꍇ�̃C�x���g�f�[�^��\���N���X�ł��B
        /// </summary>
		public class TreeItemCollectionEventArgs : EventArgs {

			#region �����o�ϐ�
			private TreeItem<T> m_Value;
			#endregion

			#region �v���p�e�B
            /// <summary>
            /// �ύX���ꂽTreeItem�I�u�W�F�N�g���擾���܂��B
            /// </summary>
			public TreeItem<T> Value {
				get {
					return this.m_Value;
				}
			}
			#endregion

			#region �R���X�g���N�^
            /// <summary>
            /// TreeItemCollectionEventArgs�N���X�̐V�����C���X�^���X�����������܂��B
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
        /// TreeItem���ύX���ꂽ�ꍇ�̃C�x���g�f�[�^��\���N���X�ł��B
        /// </summary>
		public class TreeItemCollectionEventArgs2 : EventArgs {

			#region �����o�ϐ�
			private TreeItem<T> m_OldValue;
			private TreeItem<T> m_NewValue;
			#endregion

			#region �v���p�e�B
            /// <summary>
            /// �ύX�O��TreeItem�I�u�W�F�N�g���擾���܂��B
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
            /// �ύX���TreeItem�I�u�W�F�N�g���擾���܂��B
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

			#region �R���X�g���N�^
            /// <summary>
            /// TreeItemCollectionEventArgs2�N���X�̐V�����C���X�^���X�����������܂��B
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
        /// �C�x���g���������郁�\�b�h��\���܂��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		public delegate void TreeItemCollectionEventHandler(object sender,TreeItemCollectionEventArgs e);
        /// <summary>
        /// �C�x���g���������郁�\�b�h��\���܂��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TreeItemCollectionEventHandler2(object sender, TreeItemCollectionEventArgs2 e);

		#endregion


		#region ICollection<TreeItem<T>> �����o

		void ICollection<TreeItem<T>>.Add(TreeItem<T> item) {
			this.Add(item);
		}
        /// <summary>
        /// index���w�肵�āA�v�f��z��ɃR�s�[���܂��B
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
		public void CopyTo(TreeItem<T>[] array,int arrayIndex) {
			this.List.CopyTo(array,arrayIndex);
		}
        /// <summary>
        /// �R���N�V�������ǂݎ���p���ǂ������擾���܂��B
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

		#region IEnumerable<TreeItem<T>> �����o
        /// <summary>
        /// �R���N�V�����𔽕���������񋓎q��Ԃ��܂��B 
        /// </summary>
        /// <returns></returns>
		public new IEnumerator<TreeItem<T>> GetEnumerator() {
			return new TreeItemEnumerator(this);
		}

		#region TreeItemEnumerator
		private class TreeItemEnumerator : IEnumerator<TreeItem<T>> {

			#region �����o�ϐ�
			private TreeItemCollection<T> m_List;
			private int m_CurrentIndex;
			private bool m_Changed;
			#endregion

			#region �v���p�e�B

			#endregion

			#region �R���X�g���N�^
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

			#region IEnumerator<TreeItem<T>> �����o

			public TreeItem<T> Current {
				get {
					if (this.m_CurrentIndex == -1 || this.m_List.Count <= this.m_CurrentIndex) {
						throw new InvalidOperationException();
					}
					return this.m_List[this.m_CurrentIndex];
				}
			}

			#endregion

			#region IDisposable �����o

			public void Dispose() {
			}

			#endregion

			#region IEnumerator �����o

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