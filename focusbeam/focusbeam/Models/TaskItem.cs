/**
 * TaskItem.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace focusbeam.Models
{
    public enum PriorityLevel { Low, Medium, High, Critical }
    public enum StatusLevel { Pending, Completed}

    public class TaskItem
    {
        public override string ToString()
        {
            return $"{Title} [{Status}] ({PlannedHours}h)";
        }

        public int Id { get; set; }
        public int ProjectId { get; set; } // references projects.id
        public string Title { get; set; }
        public PriorityLevel Priority { get; set; }
        public StatusLevel Status { get; set; }
        public List<string> Tags { get; set; } = new List<string> { "high-priority", "research" }; 
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; } = DateTime.Now.AddMonths(1);
        public int PlannedHours { get; set; } = 120;
        public string Notes { get; set; } = "";

        public List<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

        public int GetTotalLogged() {
            return this.TimeEntries.Sum(te => te.Duration);
        }

        public bool SaveNotesOnly() {
            int cnt = 0;
            if (Id > 0)
            {
                var sql = @"UPDATE tasks SET notes = ? WHERE id = ?";
                object[] args = { Notes, Id };
                cnt = DBAL.ExecuteNonQuery(sql, args);
            }
            return (cnt > 0);
        }

        public bool Save()
        {
            int cnt = 0;
            var tags = string.Join(",", Tags);
            bool isNew = (Id == 0);

            if (isNew) //new
            {
                var sql = @"insert into tasks (project_id, title, priority,
status, start_date, end_date, tags, planned_hours, notes) values(?, ?, ?, ?, ?, ?, ?, ?, ?)";
                object[] args = { ProjectId, Title, (int)Priority, (int)Status, StartDate, EndDate, tags,
                    PlannedHours, Notes };
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (cnt>0)
                    Id = Convert.ToInt32(DBAL.ExecuteScalar("SELECT last_insert_rowid()"));
                if (DBAL.LastError.Length > 0)
                    MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var sql = @"UPDATE tasks SET title=?, priority=?, status=?, start_date = ?, end_date = ?, 
tags= ?, planned_hours = ?, notes = ? WHERE id = ?";
                object[] args = { Title, (int)Priority, (int)Status, StartDate, EndDate, tags, PlannedHours, Notes, Id };
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (DBAL.LastError.Length > 0)
                    MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (cnt > 0 && isNew) {
                TimeEntries.ForEach(te => {
                    te.TaskId = this.Id;
                    te.Save();
                });
            }
            return (cnt > 0);
        }
    }
}
