using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CoinExchangeService
{
    public class DbHelper
    {
        public static void CreateDb(string dbName)
        {
            if (File.Exists(dbName))
                return;
            SQLiteConnection.CreateFile(dbName);
            string sqlString = "CREATE TABLE TransData (CoinType TEXT NOT NULL,Height INTEGER NOT NULL,Txid TEXT NOT NULL,Address TEXT NOT NULL,Value REAL NOT NULL,ConfirmCount INTEGER NOT NULL,UpdateTime TEXT NOT NULL);" +
                               "CREATE TABLE Address (CoinType TEXT NOT NULL,Address TEXT NOT NULL,DateTime TEXT NOT NULL);" +
                               "CREATE TABLE ParseIndex (CoinType TEXT PRIMARY KEY NOT NULL,Height INTEGER NOT NULL,DateTime TEXT NOT NULL)";
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

        /// <summary>
        /// 保存监控地址
        /// </summary>
        /// <param name="json"></param>
        public static void SaveAddress(JObject json)
        {
            var sql = $"insert into Address (CoinType,Address,DateTime) values ('{json["type"]}','{json["address"]}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            ExecuteSql(sql);
        }

        /// <summary>
        /// 保存交易信息
        /// </summary>
        /// <param name="transRspList"></param>
        public static void SaveTransInfo(List<TransResponse> transRspList)
        {
            StringBuilder sbSql = new StringBuilder();
            foreach (var tran in transRspList)
            {
                if (tran.confirmcount == 1)
                {
                    sbSql.Append(
                        $"insert into TransData (CoinType,Height,Txid,Address,Value,ConfirmCount,UpdateTime) values ('{tran.coinType}',{tran.height},'{tran.txid}','{tran.address}',{tran.value},{tran.confirmcount},'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');");
                }
                else
                {
                    sbSql.Append(
                        $"update TransData set ConfirmCount={tran.confirmcount},UpdateTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' where Txid='{tran.txid}';");
                }

            }
            ExecuteSql(sbSql.ToString());
        }

        public static List<TransResponse> GetRspList(ref List<TransResponse> TransRspList, int count, string type)
        {
            var sql = $"select CoinType,Height,Txid,Address,Value,ConfirmCount from TransData where CoinType = '{type}' and ConfirmCount < {count}";
            var table = ExecuSqlToDataTable(sql);
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var trans = new TransResponse();
                    trans.coinType = table.Rows[i]["CoinType"].ToString();
                    trans.address = table.Rows[i]["Address"].ToString();
                    trans.txid = table.Rows[i]["Txid"].ToString();
                    trans.confirmcount = Convert.ToInt32(table.Rows[i]["ConfirmCount"]);
                    trans.height = Convert.ToInt32(table.Rows[i]["Height"]);
                    trans.value = Convert.ToDecimal(table.Rows[i]["Value"]);
                    TransRspList.Add(trans);
                }
            }
            return TransRspList;
        }

        public static List<string> GetBtcAddr()
        {
            var list = new List<string>();
            var sql = "select Address from Address where CoinType='btc' ";
            var table = ExecuSqlToDataTable(sql);
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    list.Add(table.Rows[i][0].ToString());
                }
            }

            return list;
        }

        public static List<string> GetEthAddr()
        {
            var list = new List<string>();
            var sql = "select Address from Address where CoinType='eth' ";
            var table = ExecuSqlToDataTable(sql);
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    list.Add(table.Rows[i][0].ToString());
                }
            }

            return list;
        }

        public static void SaveIndex(int i, string type)
        {
            var sql =
                $"Replace into ParseIndex (CoinType,Height,DateTime) values ('{type}',{i},'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            ExecuteSql(sql);
        }

        public static int GetBtcIndex()
        {
            var sql = "select Height from ParseIndex where CoinType='btc' ";
            var table = ExecuSqlToDataTable(sql);
            if (table.Rows.Count > 0 && !string.IsNullOrEmpty(table.Rows[0][0].ToString()))
                return Convert.ToInt32(table.Rows[0][0]);
            return 1;
        }

        public static int GetEthIndex()
        {
            var sql = "select Height from ParseIndex where CoinType='eth' ";
            var table = ExecuSqlToDataTable(sql);
            if (table.Rows.Count > 0 && !string.IsNullOrEmpty(table.Rows[0][0].ToString()))
                return Convert.ToInt32(table.Rows[0][0]);
            return 1;
        }

        private static void ExecuteSql(string sql)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = MonitorData.db");
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
                var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ";
                Console.WriteLine(time + "DbError:" + ex.ToString());
                File.WriteAllText("saveErrLog.txt", ex.ToString());
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
            SQLiteConnection conn = new SQLiteConnection("Data Source = MonitorData.db");
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataAdapter sqliteDa = new SQLiteDataAdapter(cmd);
            conn.Open();
            sqliteDa.Fill(table);
            conn.Close();
            return table;
           
        }      
    }
}
