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

namespace focusbeam
{
    public static class DBAL
    {
        private static SQLiteConnection conn = null;

        public static void Init() {
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

        public static List<Project> GetAllProjects() {
            var projects = new List<Project>();
            string sql = "SELECT id, title, category, tags, start_date, end_date, notes FROM projects";
            //var cmd = new SQLiteCommand(sql, conn);
            using (var dataAdapter = new SQLiteDataAdapter(sql, conn)) {
                var ds = new DataSet();
                dataAdapter.Fill(ds, "projects");
                foreach (DataRow row in ds.Tables["projects"].Rows) {
                    var project = new Project
                    {
                        Id = row.Field<int>("id"),
                        Title = row.Field<string>("title"),
                        Category = (CategoryLevel)Enum.Parse(typeof(CategoryLevel), row.Field<string>("category"), true),
                        Tags = row.IsNull("tags") ? "" : row.Field<string>("tags"),
                        StartDate = row.Field<DateTime>("start_date"),
                        EndDate = row.Field<DateTime>("end_date"),
                        Notes = row.IsNull("notes") ? "" : row.Field<string>("notes")
                    };
                    projects.Add(project);
                }
            }
            return projects;
        }

    }
}
