using System;
using System.Collections.Generic;

namespace Kucl {
	/// <summary>
	/// �A�����������𐶐����A�Ǘ�����N���X�ł�
	/// ���̃N���X����������ʂ��ԍ��́A�����Ă��鐔�l������ꍇ�A�����Ă��鐔�l��D��I�ɐ������܂�
	/// </summary>
	public class SequentialNumberGenerator{
        
        #region �����o�ϐ�
        private int m_start;
        private int m_end;
        private List<int> m_list;
        private SequentialNumnerMode m_Mode;
        private Predicate<int> m_ForCondition;
        private int m_Step;
        #endregion

        #region �v���p�e�B
        /// <summary>
        /// ���[�h���擾�A�ݒ肵�܂��B
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

        #region �R���X�g���N�^
        /// <summary>
        /// SequentialNumberGenerator�I�u�W�F�N�g�����������܂��B
        /// ����ł͔ԍ���0����n�܂�܂��B
        /// </summary>
        public SequentialNumberGenerator()
            : this(0,int.MaxValue) {
        }
        /// <summary>
        /// SequentialNumberGenerator�I�u�W�F�N�g�����������܂��B
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
		/// �ʂ��ԍ��̎��̐��l��Ԃ�
		/// </summary>
		/// <returns>�ʂ��ԍ��̎��̐��l</returns>
		public int GetNext(){
            for(int i = this.m_start;this.m_ForCondition.Invoke(i);i += this.m_Step) {
                if(this.m_list.Contains(i)) {
                    continue;
                }
                this.m_list.Add(i);
                return i;
            }
            throw new ApplicationException("�ʂ��ԍ��̎擾�Ɏ��s���܂����B\r\n�擾�ł���ԍ�������܂���B");
		}
		/// <summary>
		/// �ʂ��ԍ��̎��̐��l���ǂ݂���B
		/// </summary>
		/// <returns>�ʂ��ԍ��̎��̐��l</returns>
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
		/// �ʂ��ԍ�����number���폜���܂�
		/// </summary>
		/// <param name="number">�ʂ��ԍ�����폜���鐔�l</param>
		public void RemoveNumber(int number){
            if(this.m_list.Contains(number)) {
                this.m_list.Remove(number);
            }
		}
        /// <summary>
        /// �w�肵���ԍ����擾�ςݔԍ��Ƃ��ēo�^���܂�
        /// </summary>
        /// <param name="number"></param>
        public void RegistNumber(int number) {
            this.m_list.Add(number);
        }

        /// <summary>
        /// �w�肵���ԍ����擾�ς݂��ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool IsRegistered(int number) {
            return this.m_list.Contains(number);
        }
        /// <summary>
        /// �S�Ă̎擾�ςݔԍ������Z�b�g���܂�
        /// </summary>
        public void Reset() {
            this.m_list.Clear();
        }
	}

    /// <summary>
    /// �A�Ԏ擾�̃��[�h�������񋓑̂ł��B
    /// </summary>
    public enum SequentialNumnerMode {
        /// <summary>
        /// ��������
        /// </summary>
        Ascending,
        /// <summary>
        /// ��������
        /// </summary>
        Descending
    }
}
