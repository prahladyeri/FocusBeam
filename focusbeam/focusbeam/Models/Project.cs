/**
 * Project.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace focusbeam.Models
{
    public enum CategoryLevel { 
        Work = 0, Study = 1, Home = 2, Other = 3
    }
    public class Project
    {
        //TODO: Generate a unique ID
        //"TX" + Guid.NewGuid().ToString("N").Substring(13)
        public int Id { get; set; }
        public string Title { get; set; }
        public CategoryLevel Category { get; set; }
        public List<string> Tags { get; set; } = new List<string> { "urgent", "bug" };
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);
        public string Notes { get; set; } = "";
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public static List<Project> GetAll()
        {
            var projects = new List<Project>();
            foreach (DataRow row in DBAL.FetchResult("select * from projects order by id desc;").Rows)
            {
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
                if (!row.IsNull("tags"))
                {
                    project.Tags = row.Field<string>("tags").Split(',').ToList();
                }
                var items = DBAL.FetchResult($"select * from tasks where project_id={project.Id}");
                foreach (DataRow taskRow in items.Rows)
                {
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
                        taskItem.Tags = taskRow.Field<string>("tags").Split(',').ToList();
                    }

                    var titems = DBAL.FetchResult($"select * from timesheet where task_id={taskItem.Id} order by id;");
                    foreach (DataRow teRow in titems.Rows)
                    {
                        TimeEntry te = new TimeEntry
                        {
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


        public bool Save()
        {
            int cnt;
            var tags = string.Join(",", Tags);
            if (Id == 0) //new
            {
                var sql = @"insert into projects (title, category, tags, start_date, end_date, notes) 
values(?, ?, ?, ?, ?, ?)";
                object[] args = { Title, (int)Category, tags, StartDate, EndDate, Notes };
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (cnt > 0)
                    Id = Convert.ToInt32(DBAL.ExecuteScalar("SELECT last_insert_rowid()"));
                if (DBAL.LastError.Length > 0)
                    MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var sql = @"UPDATE projects SET title=?, category=?, tags=?, start_date = ?, end_date = ?, notes = ? WHERE id = ?";
                object[] args = { Title, (int)Category, tags, StartDate, EndDate, Notes, Id };
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (DBAL.LastError.Length > 0)
                    MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (cnt > 0) 
            {
                Tasks.ForEach(t => {
                    t.ProjectId = this.Id;
                    t.Save();
                });
            }
            return (cnt > 0);
        }
    }
}
