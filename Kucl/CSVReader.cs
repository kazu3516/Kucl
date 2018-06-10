using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kucl {

    #region CSVReader
    /// <summary>
    /// CSV����͂���N���X�ł��B
    /// </summary>
    public class CSVReader:IDisposable {

        #region �����o�ϐ�
        private TextReader m_Reader;

        private StringBuilder m_Buffer;

        private List<List<string>> m_Results;

        /// <summary>
        /// �o�b�t�@�̃T�C�Y
        /// </summary>
        private int m_BufferSize;
        /// <summary>
        /// �o�b�t�@���̉�͈ʒu
        /// </summary>
        private int m_BufferPosition;

        /// <summary>
        /// �\�[�X���̉�͈ʒu
        /// </summary>
        private int m_Position;
        
        /// <summary>
        /// ���݂̉�͈ʒu���N�H�[�e�[�V�������ł��邩�ǂ����������t���O
        /// </summary>
        private bool m_InQuatation;

        private List<ConvertRule> m_RulesInQuot;
        private List<ConvertRule> m_RulesNotInQuot;
        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �����Ŏg�p����TextReader���擾���܂��B
        /// </summary>
        protected TextReader Reader {
            get {
                return this.m_Reader;
            }
        }
        /// <summary>
        /// ���݈ʒu�������l���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public int Position {
            get {
                return this.m_Position;
            }
            protected set {
                this.m_Position = value;
            }
        }
        /// <summary>
        /// �l�̓ǂݎ��Ɏg�p����o�b�t�@�T�C�Y���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        protected int BufferSize {
            get {
                return this.m_BufferSize;
            }
            set {
                this.m_BufferSize = value;
            }
        }
        /// <summary>
        /// �l�̓ǂݎ��Ɏg�p����o�b�t�@���擾���܂��B
        /// </summary>
        protected StringBuilder Buffer {
            get {
                return this.m_Buffer;
            }
        }
        /// <summary>
        /// ���p�����̒l�̕ϊ����[�����擾���܂��B
        /// </summary>
        protected List<ConvertRule> Rules_InQuot {
            get {
                return this.m_RulesInQuot;
            }
        }
        /// <summary>
        /// ���p���O�̒l�̕ϊ����[�����擾���܂��B
        /// </summary>
        protected List<ConvertRule> Rules_NotInQuot {
            get {
                return this.m_RulesNotInQuot;
            }
        }
        #endregion

        #region �R���X�g���N�^
        private CSVReader() {
            this.m_Position = 0;
            this.m_BufferPosition = 0;
            this.m_BufferSize = 1024;
            this.m_Buffer = new StringBuilder();
            this.m_Results = new List<List<string>>();
            this.m_Results.Add(new List<string>());
            this.m_RulesInQuot = new List<ConvertRule>();
            this.m_RulesNotInQuot = new List<ConvertRule>();
            this.InitializeRule();
        }
        /// <summary>
        /// �t�@�C�������w�肵��CSVReader�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="filename"></param>
        public CSVReader(string filename)
            : this() {
            this.m_Reader = new StreamReader(filename);
        }
        /// <summary>
        /// TextReader���w�肵��CSVReader�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="reader"></param>
        public CSVReader(TextReader reader)
            : this() {
            this.m_Reader = reader;
        }
        /// <summary>
        /// CSVReader�N���X�̃C���X�^���X��j�����܂��B
        /// </summary>
        ~CSVReader() {
            if(!this.m_Disposed) {
                this.Close();
            }
        }
        #endregion

        #region InitializeRule���\�b�h
        /// <summary>
        /// �ϊ����[���̏��������s���܂��B
        /// </summary>
        protected virtual void InitializeRule() {
            this.m_RulesInQuot.Add(new ConvertRule("\"\"","\""));
        }
        #endregion

        #region IDisposable �����o
        private bool m_Disposed;
        /// <summary>
        /// Reader����܂��B
        /// </summary>
        public void Close() {
            if(this.m_Reader != null) {
                this.m_Reader.Close();
                this.m_Disposed = true;
            }
        }
        void IDisposable.Dispose() {
            if(this.m_Reader != null) {
                this.m_Reader.Dispose();
                this.m_Disposed = true;
            }
        }

        #endregion

        #region public���\�b�h

        #region ReadAll���\�b�h
        /// <summary>
        /// EOF�ɓ��B����܂œǂݎ����s���܂��B
        /// </summary>
        /// <returns></returns>
        public CSVData ReadAll() {
            while(!this.IsEOF()) {
                this.Read();
            }
            return new CSVData(this.m_Results);
        }
        #endregion

        #region Read���\�b�h
        /// <summary>
        /// ���ڂ̓Ǎ����s���܂��B
        /// </summary>
        /// <returns></returns>
        public string Read() {
            if(this.IsBufferEnd()) {
                this.ReadToBuffer();
            }
            List<string> items = this.m_Results[this.m_Results.Count - 1];
            //�擪�̕�����"�̏ꍇ�A�N�H�[�e�[�V�������[�h
            this.m_InQuatation = this.m_Buffer[this.m_BufferPosition] == '"';
            if(this.m_InQuatation && !this.IsEOF()) {
                this.m_BufferPosition++;
            }
            bool end = false;
            StringBuilder buff = new StringBuilder();
            while(!(this.IsEOF())) {
                if(this.IsBufferEnd()) {
                    this.ReadToBuffer();
                }
                char c = this.m_Buffer[this.m_BufferPosition];
                if(this.m_InQuatation) {
                    //�N�H�[�e�[�V�������[�h
                    this.ReadInQuatation(buff,c);
                }
                else {
                    //�N�H�[�e�[�V�������[�h�ł͖����ꍇ
                    end = this.ReadInNoQuatation(buff,c);
                }
                if(end) {
                    break;
                }
            }
            //�ǂݎ�����A�C�e����ۑ�
            items.Add(buff.ToString());
            return buff.ToString();
        }
        #endregion

        #region ReadInQuatation
        private void ReadInQuatation(StringBuilder buff,char c) {
            //==================================================================================
            //�N�H�[�e�[�V�������̏ꍇ�A�N�H�[�e�[�V����������܂ł͕���������̂܂ܓǂݍ��ށB
            //==================================================================================
            //�ꕔ�̕�����̓��[���ɏ]���ĕϊ�����B
            ConvertRule rule = this.ApplyRule_InQuote();
            if(rule == null) {
                //���[�����K�p����Ȃ��ꍇ
                switch(c) {
                    case '"':
                        this.m_InQuatation = false;
                        break;
                    default:
                        buff.Append(c);
                        break;
                }
                this.m_BufferPosition++;
            }
            else {
                //���[�����K�p���ꂽ�ꍇ
                buff.Append(rule.After);
                this.m_BufferPosition += rule.Before.Length;
            }
        } 
        #endregion

		#region ReadInNoQuatation
		private bool ReadInNoQuatation(StringBuilder buff,char c){
            bool end = false;
            ConvertRule rule = this.ApplyRule_NotInQuote();
            if(rule == null) {
                //���[�����K�p����Ȃ��ꍇ
                switch(c) {
                    case ',':
                        end = true;
                        break;
                    case '\r':
                        //���s��\r\n�̏ꍇ�A\n�ōs�̏I���Ƃ��邽�߁A\r�̓X���[
                        if(this.m_Buffer[this.m_BufferPosition + 1] != '\n') {
                            this.m_Results.Add(new List<string>());
                            end = true;
                        }
                        break;
                    case '\n':
                        this.m_Results.Add(new List<string>());
                        end = true;
                        break;
                    default:
                        buff.Append(c);
                        break;
                }
                this.m_BufferPosition++;
            }
            else {
                //���[�����K�p���ꂽ�ꍇ
                buff.Append(rule.After);
                this.m_BufferPosition += rule.Before.Length;
            }
            return end;
		}
		#endregion 

        #region IsEOF���\�b�h
        /// <summary>
        /// ���݂̈ʒu��EOF���ǂ�����Ԃ��܂��B
        /// </summary>
        /// <returns></returns>
        public bool IsEOF() {
            return this.m_Reader.Peek() == -1 && this.IsBufferEnd();
        }
        /// <summary>
        /// ���݂̈ʒu����offset���i�񂾈ʒu��EOF���ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private bool IsEOF(int offset) {
            return this.m_Reader.Peek() == -1 && this.IsBufferEnd(offset);
        }
        #endregion

        #endregion

        #region protected virtual���\�b�h

        #region IsSeparator
        /// <summary>
        /// ���݈ʒu�̕������Z�p���[�^���ǂ�����Ԃ��܂��B
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsSeparator() {
            return this.m_Buffer[this.m_BufferPosition] == ',';
        }
        #endregion

        #endregion

        #region private���\�b�h

        #region IsBufferEnd���\�b�h
        /// <summary>
        /// ���݂̈ʒu���o�b�t�@�G���h�𒴂��Ă��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <returns></returns>
        private bool IsBufferEnd() {
            return this.m_Buffer.Length <= this.m_BufferPosition;
        }
        /// <summary>
        /// ���݂̈ʒu����offset�̐��l���i�񂾈ʒu���o�b�t�@�G���h�𒴂��Ă��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private bool IsBufferEnd(int offset) {
            return this.m_Buffer.Length <= this.m_BufferPosition + offset;
        }
        #endregion

        #region ReadToBuffer
        /// <summary>
        /// �o�b�t�@�ǂݍ��݂����s���܂�
        /// </summary>
        private void ReadToBuffer() {
            int new_Position = this.m_Buffer.Length - this.m_BufferPosition;
            //�o�b�t�@���̓ǂݍ��ݍς݂̗̈���폜
            if(this.m_BufferPosition <= this.m_Buffer.Length) {
                this.m_Buffer.Remove(0,this.m_BufferPosition);
            }
            //�V�����ǂݍ��ޒ����A�ǂݍ��݌�̃o�b�t�@���ʒu���v�Z
            int length = this.m_BufferSize - this.m_Buffer.Length;
            char[] buf = new char[length];
            int count = this.m_Reader.ReadBlock(buf,0,length);
            if(count < length) {
                Array.Resize<char>(ref buf,count);
            }
            this.m_Buffer.Append(buf);
            this.m_BufferPosition = new_Position;
        }
        #endregion

        #region ApplyRules
        /// <summary>
        /// ���[����K�p���܂��B
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        private ConvertRule ApplyRules(List<ConvertRule> rules) {
            for(int i = 0;i < rules.Count;i++) {
                ConvertRule rule = rules[i];
                bool match = true;
                for(int j = 0;j < rule.Before.Length;j++) {
                    if(this.IsBufferEnd(j)) {
                        if(!this.IsEOF(j)) {
                            this.ReadToBuffer();
                        }
                        else {
                            match = false;
                            break;
                        }
                    }
                    if(this.m_Buffer[this.m_BufferPosition + j] != rule.Before[j]) {
                        match = false;
                        break;
                    }
                }
                if(match) {
                    return rule;
                }
            }
            return null;
        }

        private ConvertRule ApplyRule_InQuote() {
            return this.ApplyRules(this.m_RulesInQuot);
        }
        private ConvertRule ApplyRule_NotInQuote() {
            return this.ApplyRules(this.m_RulesNotInQuot);
        }
        #endregion

        #endregion

    } 
    #endregion

    #region ConvertRule
    /// <summary>
    /// �ϊ����[����\���܂��B
    /// </summary>
    public class ConvertRule {

        #region �����o�ϐ�
        private string m_Before;
        private string m_After;
        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �ϊ��O��������擾���܂��B
        /// </summary>
        public string Before {
            get {
                return this.m_Before;
            }
        }
        /// <summary>
        /// �ϊ��㕶������擾���܂��B
        /// </summary>
        public string After {
            get {
                return this.m_After;
            }
        }
        #endregion

        #region �R���X�g���N�^
        /// <summary>
        /// ConvertRule�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public ConvertRule(string before,string after) {
            this.m_After = after;
            this.m_Before = before;
        }
        #endregion

        #region �I�[�o�[���C�h
        /// <summary>
        /// ���̃C���X�^���X�Ǝw�肵���C���X�^���X�����������ǂ�����Ԃ��܂��B
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            ConvertRule rule = obj as ConvertRule;
            if(rule == null) {
                return false;
            }
            return this.m_Before.Equals(rule.m_Before) && this.m_After.Equals(rule.m_After);
        }
        /// <summary>
        /// GetHashCode���I�[�o�[���C�h���܂��B
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return this.m_After.GetHashCode() + this.m_Before.GetHashCode();
        }
        /// <summary>
        /// ToString���I�[�o�[���C�h���܂��B
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "Before : " + this.m_Before + ",After : " + this.m_After;
        }
        #endregion
    } 
    #endregion

    #region CSVData
    /// <summary>
    /// CSV�Ƃ��ĕۑ����ꂽ�e���ڂ̃f�[�^��\���܂��B
    /// </summary>
    public class CSVData {

        #region �����o�ϐ�
        private List<string[]> m_Rows;

        private int m_ColumnCount;

        #endregion

        #region �C�x���g

        #endregion

        #region �C�x���g�𔭐������郁�\�b�h

        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �s�����擾���܂��B
        /// </summary>
        public int RowCount {
            get {
                return this.m_Rows.Count;
            }
        }
        /// <summary>
        /// ��̐��B��̐���1�s�ڂ̗�̐��������Ă��܂��B
        /// </summary>
        public int ColumnCount {
            get {
                return this.m_ColumnCount;
            }
        }
        #endregion

        #region �R���X�g���N�^
        /// <summary>
        /// CSVData�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="csvData"></param>
        public CSVData(List<List<string>> csvData) {
            this.m_Rows = new List<string[]>();
            if(0 < csvData.Count) {
                this.m_ColumnCount = csvData[0].Count;
            }
            for(int i = 0;i < csvData.Count;i++) {
                string[] row = new string[this.m_ColumnCount];
                int count = Math.Min(this.m_ColumnCount,csvData[i].Count);
                for(int j = 0;j < count;j++) {
                    row[j] = csvData[i][j];
                }
                this.m_Rows.Add(row);
            }
        }
        #endregion

        /// <summary>
        /// �s�A����w�肵�ăf�[�^���擾���܂��B
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public string GetValue(int row,int col) {
            return this.m_Rows[row][col];
        }
    }
    #endregion

    #region CSVWriter
    /// <summary>
    /// CSVData���t�@�C���ɏ������ރN���X��\���܂��B
    /// </summary>
    public class CSVWriter {

        #region �����o�ϐ�

        private TextWriter m_Writer;

        #endregion

        #region �v���p�e�B
        /// <summary>
        /// �t�@�C���̏������݂Ɏg�p����TextWriter���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public TextWriter Writer {
            get {
                return this.m_Writer;
            }
            set {
                this.m_Writer = value;
            }
        }

        #endregion

        #region �R���X�g���N�^
        /// <summary>
        /// �������݂Ɏg�p����TextWriter���w�肵�āACSVWriter�N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="writer"></param>
        public CSVWriter(TextWriter writer) {
            this.m_Writer = writer;
        }
        #endregion

        /// <summary>
        /// �w�肵��CSVData���������݂܂��B
        /// </summary>
        /// <param name="data"></param>
        public void Write(CSVData data) {
            StringBuilder buff = new StringBuilder(100);
            for(int i = 0;i < data.RowCount;i++) {
                for(int j = 0;j < data.ColumnCount;j++) {
                    string value =data.GetValue(i,j);
                    value = value.Replace("\"","\"\"");
                    buff.Append(value);
                    if(j != data.ColumnCount - 1) {
                        buff.Append(',');
                    }
                }
                buff.Remove(buff.Length - 1,1);
                if(i != data.RowCount - 1) {
                    buff.Append("\r\n");
                }
            }
            this.m_Writer.Write(buff.ToString());
        }
    }
    #endregion
}
