/**
 * DBAL.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Util;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace focusbeam
{
    public static class DBAL
    {
        private static SQLiteConnection conn = null;
        public static string LastError = "";

        public static void Dispose() {
            conn.Dispose();
            conn = null;
        }

        public static bool Init(string dbPath, string initSql) {
            bool isnew = false; string result;
            if (!File.Exists(dbPath)) isnew = true;
            conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            conn.Open();
            using (var cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
            {
                result = (string)cmd.ExecuteScalar();
            }
            if (!string.Equals(result, "wal", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Failed to set WAL mode on SQLite database.");
            }
            if (DateTime.Now.Date.Day == 1) {
                using (var cmd = new SQLiteCommand("vacuum;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            if (isnew)
            {
                using (var cmd = new SQLiteCommand(initSql, conn))
                {
                    cmd.ExecuteNonQuery(); // a bunch of DDL/DML statements.
                }
            }
            return isnew;
        }

        public static int ExecuteNonQuery(string sql, object[] args = null) 
        {
            try
            {
                LastError = "";
                args = (args == null ? Array.Empty<object>() : args);
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@param{i}", args[i]);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) {
                Logger.WriteLog("DBAL ERROR: " + ex.ToString());
                if (ex is SQLiteException sqliteEx && sqliteEx.ResultCode == SQLiteErrorCode.Constraint)
                {
                    LastError = "Duplicate entry or constraint violation.";
                }
                else {
                    LastError = $"Error occurred: {ex.Message}";
                }
                return -1;
            }
        }

        public static DataTable Execute(string sql, object[] args = null) {
            args = (args == null ? Array.Empty<object>() : args);
            using (var cmd = new SQLiteCommand(sql, conn)) 
            {
                for (int i = 0; i < args.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@param{i}", args[i]);
                }
                using (var da = new SQLiteDataAdapter(cmd)) {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                } 
            }
        }

        public static object ExecuteScalar(string sql) {
            using (var cmd = new SQLiteCommand(sql, conn)) {
                var result= cmd.ExecuteScalar();
                return result == DBNull.Value ? null : result;
            }
        }

        public static DataTable FetchResult(string sql) {
            var dt = new DataTable();
            using (var da = new SQLiteDataAdapter(sql, conn)) {
                da.Fill(dt);
            }
            return dt;
        }
    }
}
