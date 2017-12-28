using Community.CsharpSqlite.SQLiteClient;
using QuizzApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuizzApp.Core.Db
{
    public class DBHelper : IDisposable
    {
        public static string DB_FILE_NAME_EXTENSION = ".sqlite";

        protected SqliteConnection connection;

        protected string dbPath;
        protected string dbFileName;
        protected string dbFolderPath;


        public DBHelper(string path)
        {
            this.dbPath = path;

            if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists(path))
            {
                Debug.WriteLine("Database not found : " + path);
            }
            dbFileName = path.Split(Path.DirectorySeparatorChar).Last().Replace(DB_FILE_NAME_EXTENSION, string.Empty);
            dbFolderPath = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
            this.OpenDatabase();
        }


        public bool OpenDatabase()
        {
            try
            {
                if (this.connection != null && this.connection.State == System.Data.ConnectionState.Open)
                    return true;

                this.connection = new SqliteConnection("Version=3,uri=file:" + dbPath);
                this.connection.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception opening database : ", ex);
                return false;
            }
            return true;
        }

        public bool CloseDatabase()
        {
            try
            {
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception closing database : ", ex);
                return false;
            }
            return true;
        }

        public void Dispose()
        {
            try
            {
                this.CloseDatabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception Dispose : ", ex);
            }
        }


        public void BeginTransaction()
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "begin transaction;";
                cmd.ExecuteNonQuery();
            }
        }

        public void Commit()
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "commit;";
                cmd.ExecuteNonQuery();
            }
        }

        public void Rollback()
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "rollback";
                cmd.ExecuteNonQuery();
            }
        }

        public string Escape(string data)
        {
            data = data.Replace("'", "''");
            data = data.Replace("\\", "\\\\");
            return data;
        }

        public void Insert(string tableName, Dictionary<string, object> dic)
        {
            StringBuilder sbCol = new System.Text.StringBuilder();
            StringBuilder sbVal = new System.Text.StringBuilder();

            foreach (KeyValuePair<string, object> kv in dic)
            {
                if (sbCol.Length == 0)
                {
                    sbCol.Append("insert into ");
                    sbCol.Append(tableName);
                    sbCol.Append("(");
                }
                else
                {
                    sbCol.Append(",");
                }

                sbCol.Append("`");
                sbCol.Append(kv.Key);
                sbCol.Append("`");

                if (sbVal.Length == 0)
                {
                    sbVal.Append(" values(");
                }
                else
                {
                    sbVal.Append(", ");
                }

                sbVal.Append("@v");
                sbVal.Append(kv.Key);
            }

            sbCol.Append(") ");
            sbVal.Append(");");

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = sbCol.ToString() + sbVal.ToString();

                foreach (KeyValuePair<string, object> kv in dic)
                {
                    cmd.Parameters.Add("@v" + kv.Key, kv.Value);
                }

                cmd.ExecuteNonQuery();
            }
        }

        public void Update(string tableName, Dictionary<string, object> dicData, string colCond, object varCond)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic[colCond] = varCond;
            Update(tableName, dicData, dic);
        }

        public void Update(string tableName, Dictionary<string, object> dicData, Dictionary<string, object> dicCond)
        {
            if (dicData.Count == 0)
                throw new Exception("dicData is empty.");

            StringBuilder sbData = new System.Text.StringBuilder();

            Dictionary<string, object> _dicTypeSource = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> kv1 in dicData)
            {
                _dicTypeSource[kv1.Key] = null;
            }

            foreach (KeyValuePair<string, object> kv2 in dicCond)
            {
                if (!_dicTypeSource.ContainsKey(kv2.Key))
                    _dicTypeSource[kv2.Key] = null;
            }

            sbData.Append("update `");
            sbData.Append(tableName);
            sbData.Append("` set ");

            bool firstRecord = true;

            foreach (KeyValuePair<string, object> kv in dicData)
            {
                if (firstRecord)
                    firstRecord = false;
                else
                    sbData.Append(",");

                sbData.Append("`");
                sbData.Append(kv.Key);
                sbData.Append("` = ");

                sbData.Append("@v");
                sbData.Append(kv.Key);
            }

            sbData.Append(" where ");

            firstRecord = true;

            foreach (KeyValuePair<string, object> kv in dicCond)
            {
                if (firstRecord)
                    firstRecord = false;
                else
                {
                    sbData.Append(" and ");
                }

                sbData.Append("`");
                sbData.Append(kv.Key);
                sbData.Append("` = ");

                sbData.Append("@c");
                sbData.Append(kv.Key);
            }

            sbData.Append(";");

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = sbData.ToString();
                foreach (KeyValuePair<string, object> kv in dicData)
                {
                    cmd.Parameters.Add("@v" + kv.Key, kv.Value);
                }
                foreach (KeyValuePair<string, object> kv in dicCond)
                {
                    cmd.Parameters.Add("@c" + kv.Key, kv.Value);
                }
                cmd.ExecuteNonQuery();
            }
        }

        public long LastInsertRowId()
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "select last_insert_rowid();";
                return long.Parse(cmd.ExecuteScalar().ToString());
            }
        }


        public void RenameTable(string tableFrom, string tableTo)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = string.Format("alter table `{0}` rename to `{1}`;", tableFrom, tableTo);
                cmd.ExecuteNonQuery();
            }
        }

        public void DropTable(string table)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = string.Format("drop table if exists `{0}`", table);
                cmd.ExecuteNonQuery();
            }
        }
    }

}
