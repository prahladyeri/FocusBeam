/**
 * DBAL.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam
{
    public static class DBAL
    {
        private static SQLiteConnection conn = null;

        public static void Dispose() {
            conn.Dispose();
            conn = null;
        }

        public static void Init(string dbPath) {
            bool isNew = false;
            if (!File.Exists(dbPath)) isNew = true;
            conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            conn.Open();
            using (var cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
            {
                cmd.ExecuteNonQuery();
            }
            if (isNew) {
                string sql = Util.FileHelper.ReadEmbeddedResource("focusbeam.files.init.sql");
                using (var cmd = new SQLiteCommand(sql, conn)) {
                    cmd.ExecuteNonQuery(); // a bunch of DDL/DML statements.
                }
                //Create default project, task.
                Project project = new Project { 
                    Title = "First Project",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(90),
                };
                project.Tasks = new List<TaskItem> {
                    new TaskItem {
                        ProjectId = project.Id,
                        Title = "First Task",
                        Priority = PriorityLevel.High,
                        Status = StatusLevel.Pending,
                        StartDate = project.StartDate,
                        EndDate = project.EndDate,
                    },
                };
                project.Save();
            }
        }

        public static int ExecuteNonQuery(string sql, object[] args = null) 
        {
            try
            {
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
                if (ex is SQLiteException sqliteEx && sqliteEx.ResultCode == SQLiteErrorCode.Constraint)
                {
                    MessageBox.Show("Duplicate entry or constraint violation.", Application.ProductName,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else {
                    MessageBox.Show($"Error occurred: {ex.Message}", Application.ProductName,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
