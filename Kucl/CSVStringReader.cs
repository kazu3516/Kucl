using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kucl {

    /// <summary>
    /// CSV読み込みを行うクラスです。
    /// </summary>
    public class CSVStringReader {
        /// <summary>
        /// CSVの1行の読み込みを行います。
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="items"></param>
        public static void ReadCSV(StreamReader reader,List<string> items) {
            StringBuilder value = new StringBuilder();
            char c;
            do {
                //**************************************************
                //一文字目処理開始
                //**************************************************
                if (reader.EndOfStream) break;
                value.Clear();
                c = (char)reader.Read();
                //値なし
                if (c == ',') {
                    items.Add("");
                    continue;
                }
                if (c == '\r') {
                    reader.Read();  //\nを読み捨て
                    break;
                }
                if (c == '\n') break;
                //
                bool quateStart = false;
                if (c == '"') {
                    quateStart = true;
                    c = (char)reader.Read();
                }
                //**************************************************
                //一文字目処理終了
                //**************************************************
                while (true) {
                    if (reader.EndOfStream) break;
                    if (quateStart && c == '"') {
                        //エスケープ または 終了
                        if (quateStart && (reader.EndOfStream)) {
                            items.Add(value.ToString());
                            break;
                        }
                        c = (char)reader.Read();
                        if (quateStart && (c == ',' || c == '\r' || c == '\n')) {
                            //終了
                            items.Add(value.ToString());
                            break;
                        }
                        else if (c == '"') {
                            //エスケープ
                        }
                        else {
                            //それ以外はエラー
                            throw new ApplicationException();
                        }
                    }
                    if (!quateStart && (c == ',' || c == '\r' || c == '\n')) {
                        //終了
                        items.Add(value.ToString());
                        break;
                    }
                    value.Append(c);
                    c = (char)reader.Read();
                }
            } while (true);
        }
    }
}
