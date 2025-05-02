/**
 * DBAL.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace focusbeam
{
    public static class DBAL
    {
        private static SQLiteConnection conn = null;

        public static void init() {
            string dbPath = "focusbeam.db";
            bool isNew = false;
            if (!File.Exists(dbPath)) isNew = true;
            conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            conn.Open();
            if (isNew) {
                //TODO Create new schema if empty
                string sql = Util.FileHelper.ReadEmbeddedResource("focusbeam.files.init.sql");
                var cmd = new SQLiteCommand(sql);
                cmd.ExecuteNonQuery(); // a bunch of DDL/DML statements.
            }
        }

        public List<Project> GetAllProjects() {
            var projects = new List<Project>();
            var cmd = new SQLiteCommand("SELECT id, title, category, tags, start_date, end_date, notes FROM projects", conn);
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    var project = new Project
                    {

                    };
                }
            }



        }

    }
}
