using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kucl {

    #region CSVReader
    /// <summary>
    /// CSVを解析するクラスです。
    /// </summary>
    public class CSVReader:IDisposable {

        #region メンバ変数
        private TextReader m_Reader;

        private StringBuilder m_Buffer;

        private List<List<string>> m_Results;

        /// <summary>
        /// バッファのサイズ
        /// </summary>
        private int m_BufferSize;
        /// <summary>
        /// バッファ中の解析位置
        /// </summary>
        private int m_BufferPosition;

        /// <summary>
        /// ソース中の解析位置
        /// </summary>
        private int m_Position;
        
        /// <summary>
        /// 現在の解析位置がクォーテーション内であるかどうかを示すフラグ
        /// </summary>
        private bool m_InQuatation;

        private List<ConvertRule> m_RulesInQuot;
        private List<ConvertRule> m_RulesNotInQuot;
        #endregion

        #region プロパティ
        /// <summary>
        /// 内部で使用するTextReaderを取得します。
        /// </summary>
        protected TextReader Reader {
            get {
                return this.m_Reader;
            }
        }
        /// <summary>
        /// 現在位置を示す値を取得または設定します。
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
        /// 値の読み取りに使用するバッファサイズを取得または設定します。
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
        /// 値の読み取りに使用するバッファを取得します。
        /// </summary>
        protected StringBuilder Buffer {
            get {
                return this.m_Buffer;
            }
        }
        /// <summary>
        /// 引用符内の値の変換ルールを取得します。
        /// </summary>
        protected List<ConvertRule> Rules_InQuot {
            get {
                return this.m_RulesInQuot;
            }
        }
        /// <summary>
        /// 引用符外の値の変換ルールを取得します。
        /// </summary>
        protected List<ConvertRule> Rules_NotInQuot {
            get {
                return this.m_RulesNotInQuot;
            }
        }
        #endregion

        #region コンストラクタ
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
        /// ファイル名を指定してCSVReaderクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="filename"></param>
        public CSVReader(string filename)
            : this() {
            this.m_Reader = new StreamReader(filename);
        }
        /// <summary>
        /// TextReaderを指定してCSVReaderクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="reader"></param>
        public CSVReader(TextReader reader)
            : this() {
            this.m_Reader = reader;
        }
        /// <summary>
        /// CSVReaderクラスのインスタンスを破棄します。
        /// </summary>
        ~CSVReader() {
            if(!this.m_Disposed) {
                this.Close();
            }
        }
        #endregion

        #region InitializeRuleメソッド
        /// <summary>
        /// 変換ルールの初期化を行います。
        /// </summary>
        protected virtual void InitializeRule() {
            this.m_RulesInQuot.Add(new ConvertRule("\"\"","\""));
        }
        #endregion

        #region IDisposable メンバ
        private bool m_Disposed;
        /// <summary>
        /// Readerを閉じます。
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

        #region publicメソッド

        #region ReadAllメソッド
        /// <summary>
        /// EOFに到達するまで読み取りを行います。
        /// </summary>
        /// <returns></returns>
        public CSVData ReadAll() {
            while(!this.IsEOF()) {
                this.Read();
            }
            return new CSVData(this.m_Results);
        }
        #endregion

        #region Readメソッド
        /// <summary>
        /// 項目の読込を行います。
        /// </summary>
        /// <returns></returns>
        public string Read() {
            if(this.IsBufferEnd()) {
                this.ReadToBuffer();
            }
            List<string> items = this.m_Results[this.m_Results.Count - 1];
            //先頭の文字が"の場合、クォーテーションモード
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
                    //クォーテーションモード
                    this.ReadInQuatation(buff,c);
                }
                else {
                    //クォーテーションモードでは無い場合
                    end = this.ReadInNoQuatation(buff,c);
                }
                if(end) {
                    break;
                }
            }
            //読み取ったアイテムを保存
            items.Add(buff.ToString());
            return buff.ToString();
        }
        #endregion

        #region ReadInQuatation
        private void ReadInQuatation(StringBuilder buff,char c) {
            //==================================================================================
            //クォーテーション内の場合、クォーテーションが閉じるまでは文字列をそのまま読み込む。
            //==================================================================================
            //一部の文字列はルールに従って変換する。
            ConvertRule rule = this.ApplyRule_InQuote();
            if(rule == null) {
                //ルールが適用されない場合
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
                //ルールが適用された場合
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
                //ルールが適用されない場合
                switch(c) {
                    case ',':
                        end = true;
                        break;
                    case '\r':
                        //改行が\r\nの場合、\nで行の終わりとするため、\rはスルー
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
                //ルールが適用された場合
                buff.Append(rule.After);
                this.m_BufferPosition += rule.Before.Length;
            }
            return end;
		}
		#endregion 

        #region IsEOFメソッド
        /// <summary>
        /// 現在の位置がEOFかどうかを返します。
        /// </summary>
        /// <returns></returns>
        public bool IsEOF() {
            return this.m_Reader.Peek() == -1 && this.IsBufferEnd();
        }
        /// <summary>
        /// 現在の位置からoffset分進んだ位置がEOFかどうかを返します。
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private bool IsEOF(int offset) {
            return this.m_Reader.Peek() == -1 && this.IsBufferEnd(offset);
        }
        #endregion

        #endregion

        #region protected virtualメソッド

        #region IsSeparator
        /// <summary>
        /// 現在位置の文字がセパレータかどうかを返します。
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsSeparator() {
            return this.m_Buffer[this.m_BufferPosition] == ',';
        }
        #endregion

        #endregion

        #region privateメソッド

        #region IsBufferEndメソッド
        /// <summary>
        /// 現在の位置がバッファエンドを超えているかどうかを返します。
        /// </summary>
        /// <returns></returns>
        private bool IsBufferEnd() {
            return this.m_Buffer.Length <= this.m_BufferPosition;
        }
        /// <summary>
        /// 現在の位置からoffsetの数値分進んだ位置がバッファエンドを超えているかどうかを返します。
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private bool IsBufferEnd(int offset) {
            return this.m_Buffer.Length <= this.m_BufferPosition + offset;
        }
        #endregion

        #region ReadToBuffer
        /// <summary>
        /// バッファ読み込みを実行します
        /// </summary>
        private void ReadToBuffer() {
            int new_Position = this.m_Buffer.Length - this.m_BufferPosition;
            //バッファ内の読み込み済みの領域を削除
            if(this.m_BufferPosition <= this.m_Buffer.Length) {
                this.m_Buffer.Remove(0,this.m_BufferPosition);
            }
            //新しく読み込む長さ、読み込み後のバッファ内位置を計算
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
        /// ルールを適用します。
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
    /// 変換ルールを表します。
    /// </summary>
    public class ConvertRule {

        #region メンバ変数
        private string m_Before;
        private string m_After;
        #endregion

        #region プロパティ
        /// <summary>
        /// 変換前文字列を取得します。
        /// </summary>
        public string Before {
            get {
                return this.m_Before;
            }
        }
        /// <summary>
        /// 変換後文字列を取得します。
        /// </summary>
        public string After {
            get {
                return this.m_After;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// ConvertRuleクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public ConvertRule(string before,string after) {
            this.m_After = after;
            this.m_Before = before;
        }
        #endregion

        #region オーバーライド
        /// <summary>
        /// このインスタンスと指定したインスタンスが等しいかどうかを返します。
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
        /// GetHashCodeをオーバーライドします。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return this.m_After.GetHashCode() + this.m_Before.GetHashCode();
        }
        /// <summary>
        /// ToStringをオーバーライドします。
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
    /// CSVとして保存された各項目のデータを表します。
    /// </summary>
    public class CSVData {

        #region メンバ変数
        private List<string[]> m_Rows;

        private int m_ColumnCount;

        #endregion

        #region イベント

        #endregion

        #region イベントを発生させるメソッド

        #endregion

        #region プロパティ
        /// <summary>
        /// 行数を取得します。
        /// </summary>
        public int RowCount {
            get {
                return this.m_Rows.Count;
            }
        }
        /// <summary>
        /// 列の数。列の数は1行目の列の数を示しています。
        /// </summary>
        public int ColumnCount {
            get {
                return this.m_ColumnCount;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// CSVDataクラスの新しいインスタンスを初期化します。
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
        /// 行、列を指定してデータを取得します。
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
    /// CSVDataをファイルに書き込むクラスを表します。
    /// </summary>
    public class CSVWriter {

        #region メンバ変数

        private TextWriter m_Writer;

        #endregion

        #region プロパティ
        /// <summary>
        /// ファイルの書き込みに使用するTextWriterを取得または設定します。
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

        #region コンストラクタ
        /// <summary>
        /// 書き込みに使用するTextWriterを指定して、CSVWriterクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="writer"></param>
        public CSVWriter(TextWriter writer) {
            this.m_Writer = writer;
        }
        #endregion

        /// <summary>
        /// 指定したCSVDataを書き込みます。
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
