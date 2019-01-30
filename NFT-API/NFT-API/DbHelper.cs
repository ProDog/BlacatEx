using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;

namespace NFT_API
{
    public class DbHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string dbName = "TransRecord.db";
        public static void CreateDb()
        {
            if (File.Exists(dbName))
                return;
            SQLiteConnection.CreateFile(dbName);
            string sqlString = "CREATE TABLE Transactions (CoinType TEXT NOT NULL,TransKey TEXT NOT NULL,Txid TEXT NOT NULL, ToAddress TEXT,Value REAL NOT NULL,UpdateTime TEXT NOT NULL,PRIMARY KEY (\"CoinType\", \"Txid\",\"TransKey\"));" +
                "CREATE TABLE OpRecord (OpType TEXT NOT NULL,TransKey TEXT NOT NULL,Txid TEXT NOT NULL,UpdateTime TEXT NOT NULL,PRIMARY KEY (\"OpType\", \"Txid\",\"TransKey\"));";
            SQLiteConnection conn = new SQLiteConnection();
            conn.ConnectionString = "DataSource = " + dbName;
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn)
            {
                CommandText = sqlString
            };
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static string GetSendMoneyTxid(JObject json)
        {
            var sql = $"select Txid from Transactions where CoinType= '{json["coinType"]}' and TransKey='{json["key"]}'";
            var table = ExecuSqlToDataTable(sql);
            var Txid = string.Empty;
            if (table.Rows.Count > 0)
            {
                return table.Rows[0]["Txid"].ToString();
            }
            return Txid;
        }

        public static string GetOpRecordTxid(string opType, string key)
        {
            var sql = $"select Txid from OpRecord where OpType= '{opType}' and TransKey='{key}'";
            var table = ExecuSqlToDataTable(sql);
            var Txid = string.Empty;
            if (table.Rows.Count > 0)
            {
                return table.Rows[0]["Txid"].ToString();
            }
            return Txid;
        }

        public static void SaveOpRecordResult(string opType, string key, string txid)
        {
            var sql = $"Insert into OpRecord (OpType, TransKey, Txid, UpdateTime) values ('{opType}','{key}', '{txid}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            ExecuteSql(sql);
        }

        public static void SaveSendMoneyResult(string coinType, string key, string txid, string toAddress, decimal value)
        {
            var sql = $"Insert into Transactions (CoinType, TransKey, Txid, ToAddress, Value, UpdateTime) values ('{coinType}','{key}', '{txid}', '{toAddress}', {value}, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            ExecuteSql(sql);
        }

        private static void ExecuteSql(string sql)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = " + dbName);
            conn.Open();
            //事务操作
            SQLiteTransaction trans = conn.BeginTransaction();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = trans;
            cmd.CommandText = sql.ToString();
            try
            {
                cmd.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                trans.Rollback();
            }
            finally
            {
                conn.Close();
            }
        }

        private static DataTable ExecuSqlToDataTable(string sql)
        {
            DataTable table = new DataTable();
            SQLiteConnection conn = new SQLiteConnection("Data Source = " + dbName);
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataAdapter sqliteDa = new SQLiteDataAdapter(cmd);
            conn.Open();
            sqliteDa.Fill(table);
            conn.Close();
            return table;

        }

    }
}
