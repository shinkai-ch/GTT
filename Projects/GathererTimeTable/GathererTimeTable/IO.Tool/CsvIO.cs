﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace GathererTimeTable.IO.Tool {
    class CsvIO {

        public CsvIO() {

        }

        public static void SaveDataTableToCSV(DataGridView _DataGridView,string _FileName) {
            // 保存用のファイルを開く
            using(StreamWriter writer = new StreamWriter(_FileName,false,Encoding.GetEncoding("shift_jis"))) {

                int rowCount = _DataGridView.Rows.Count;

                // ユーザによる行追加が許可されている場合は、最後に新規入力用の1行分を差し引く
                if(_DataGridView.AllowUserToAddRows) {
                    rowCount = rowCount - 1;
                }

                //行頭コメント出力
                writer.WriteLine("#0:bool:アラームONOFF , 1:string:職 , 2:string:アラーム時刻(str) , 3:string:アイテムネーム , 4:string:採集場所 , 5:string:採集段数 , 6:string:ノート");


                for(int i = 0;i < rowCount;i++) {
                    // リストの初期化
                    List<String> strList = new List<String>();

                    // 要素をリストへ登録する
                    for(int j = 0;j < _DataGridView.Columns.Count;j++) {
                        strList.Add("\"" + Convert.ToString(_DataGridView[j,i].Value) + "\"");
                    }

                    // CSV 形式に変換
                    String strCsvData = String.Join(",",strList);

                    writer.WriteLine(strCsvData);
                }
            }
        }

        public static void loadCSVToDataGridView(DataGridView _DataGridView,string _FileName) {
            using(TextFieldParser parser = new TextFieldParser(_FileName,Encoding.GetEncoding("Shift_JIS"))) {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(","); // 区切り文字はコンマ
                parser.CommentTokens = new string[] { "#" }; //#から始まる行をコメント行として設定
                //""コメント内のコンマは無視する
                parser.HasFieldsEnclosedInQuotes = true;

                // データをすべてクリア
                _DataGridView.Rows.Clear();
                //
                while(!parser.EndOfData) {
                    string[] row = parser.ReadFields(); // 1行読み込み
                    // 読み込んだデータ(1行をDataGridViewに表示する)
                    _DataGridView.Rows.Add(row);
                }
            }
        }

        public static void loadCSVToControlList(string _TimeString,string _FileName,List<string> _LBCT,List<string> _LLCT,HashSet<string> _IsTimeSet) {
            using(TextFieldParser parser = new TextFieldParser(_FileName,Encoding.GetEncoding("Shift_JIS"))) {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(","); // 区切り文字はコンマ
                parser.HasFieldsEnclosedInQuotes = true; //""コメント内のコンマは無視する
                parser.CommentTokens = new string[] { "#" }; //#から始まる行をコメント行として設定

                _IsTimeSet.Clear();
                while(!parser.EndOfData) {
                    string[] row = parser.ReadFields(); // 1行読み込み
                    if(row[0] == "True" && row[2] == _TimeString) {
                        string _labelText;
                        if(_FileName == "FisherCollection.csv") {
                            //ボタン側のテキストリストにアイテム名、改行、採集時間 - 採集終了時間を追加
                            _LBCT.Add(row[4] + "\r\n" + row[2] + " - " + row[3]);
                            //ラベル側のテキストリストに"採集場所、採集段数、改行、メモを追加
                            _labelText = row[5] + " / " + row[6] + " / " + row[7] + " / " + row[8] + "\r\n" + row[9];
                            
                        }
                        else {
                            //ボタン側のテキストリストにアイテム名を追加
                            _LBCT.Add(row[3]);
                            //ラベル側のテキストリストに"採集場所、採集段数、改行、メモを追加
                            _labelText = row[4] + " , " + row[5] + "\r\n" + row[6];

                        }
                        _LLCT.Add(_labelText);

                    }
                    _IsTimeSet.Add(row[0] == "True" ? row[2] : null);
                }
            }
        }

        public static void loadCSVIsCheckBoxTrue(string _FileName,HashSet<string> _IsTimeSet) {
            using(TextFieldParser parser = new TextFieldParser(_FileName,Encoding.GetEncoding("Shift_JIS"))) {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(","); // 区切り文字はコンマ
                parser.HasFieldsEnclosedInQuotes = true; //""コメント内のコンマは無視する
                parser.CommentTokens = new string[] { "#" }; //#から始まる行をコメント行として設定

                _IsTimeSet.Clear();
                while(!parser.EndOfData) {
                    string[] row = parser.ReadFields(); // 1行読み込み
                    _IsTimeSet.Add(row[0] == "True" ? row[2] : null);
                }
            }
        }
    }
}
