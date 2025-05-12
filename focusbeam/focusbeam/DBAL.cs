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

        public static void Dispose() {
            conn.Dispose();
            conn = null;
        }

        public static void Init() {
            string dbPath = "focusbeam.db";
            bool isNew = false;
            if (!File.Exists(dbPath)) isNew = true;
            conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            conn.Open();
            if (isNew) {
                string sql = Util.FileHelper.ReadEmbeddedResource("focusbeam.files.init.sql");
                using (var cmd = new SQLiteCommand(sql, conn)) {
                    cmd.ExecuteNonQuery(); // a bunch of DDL/DML statements.
                }
                //Create default project, task.
                Project project = new Project { 
                    Title = "Default Project",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(90),
                    Tags = new string[] { "Alpha", "Beta", "Epsilon" },
                };
                project.Tasks = new List<TaskItem> {
                    new TaskItem {
                        ProjectId = project.Id,
                        Title = "Default Task",
                        Priority = PriorityLevel.High,
                        Status = StatusLevel.Pending,
                        StartDate = project.StartDate,
                        EndDate = project.EndDate,
                        Tags = new string[] { "Tango", "Charlie" },
                    },
                };
                project.Save();
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
                if (!sql.ToUpperInvariant().StartsWith("SELECT"))
                {
                    cmd.ExecuteNonQuery();
                    return null;
                }
                else {
                    using (var da = new SQLiteDataAdapter(cmd)) {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    } 
                }
            }
        }

        public static object ExecuteScalar(string sql) {
            using (var cmd = new SQLiteCommand(sql, conn)) {
                var result= cmd.ExecuteScalar();
                return result == DBNull.Value ? null : result;
            }
        }

        private static DataTable FetchResult(string sql) {
            //string sql = $"select * from {tableName}";
            DataSet ds;
            using (var da = new SQLiteDataAdapter(sql, conn)) {
                ds = new DataSet();
                da.Fill(ds, "table1");
            }
            return ds.Tables["table1"];
        }

        public static List<Project> GetAllProjects() {
            var projects = new List<Project>();
            foreach (DataRow row in FetchResult("select * from projects order by id desc;").Rows) {
                var project = new Project
                {
                    Id = Convert.ToInt32(row["id"]),
                    Title = row.Field<string>("title"),
                    Category = (CategoryLevel)row.Field<int>("category"),
                    //Tags = row.IsNull("tags") ? "" : row.Field<string>("tags"),
                    StartDate = row.Field<DateTime>("start_date"),
                    EndDate = row.Field<DateTime>("end_date"),
                    Notes = row.IsNull("notes") ? "" : row.Field<string>("notes")
                };
                if (!row.IsNull("tags")) {
                    project.Tags = row.Field<string>("tags").Split(',');
                }
                var items = FetchResult($"select * from tasks where project_id={project.Id}");
                foreach (DataRow taskRow in items.Rows) {
                    TaskItem taskItem = new TaskItem
                    {
                        Id = Convert.ToInt32(taskRow["id"]),
                        ProjectId = project.Id,
                        Title = taskRow.Field<string>("title"),
                        Priority = (PriorityLevel)taskRow.Field<int>("priority"),
                        Status = (StatusLevel)taskRow.Field<int>("status"),
                        StartDate = taskRow.Field<DateTime>("start_date"),
                        EndDate = taskRow.Field<DateTime>("end_date"),
                        PlannedHours = taskRow.Field<int>("planned_hours"),
                        Notes = taskRow.Field<string>("notes"),
                    };
                    if (!taskRow.IsNull("tags"))
                    {
                        taskItem.Tags = taskRow.Field<string>("tags").Split(',');
                    }

                    var titems = FetchResult($"select * from timesheet where task_id={taskItem.Id} order by id;");
                    foreach (DataRow teRow in titems.Rows) {
                        TimeEntry te = new TimeEntry {
                            Id = Convert.ToInt32(teRow["id"]),
                            TaskId = taskItem.Id,
                            StartTime = teRow.Field<DateTime>("start_time"),
                            EndTime = teRow.Field<DateTime>("end_time"),
                            Duration = teRow.Field<int>("duration"),
                            Status = (TimeEntryStatusLevel)teRow.Field<int>("status"),
                            Notes = teRow.Field<string>("notes"),
                        };
                        taskItem.TimeEntries.Add(te);
                    }
                    project.Tasks.Add(taskItem);
                } 
                projects.Add(project);
            }
            return projects;
        }

    }
}
