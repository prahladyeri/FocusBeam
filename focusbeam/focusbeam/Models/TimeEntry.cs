﻿/**
 * TimeEntry.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Windows.Forms;

namespace focusbeam.Models
{
    public enum TimeEntryStatusLevel {Running, Paused, Completed }
    public class TimeEntry
    {
        public int Id { get; set; }
        public int TaskId { get; set; } // references tasks.id
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; } // Duration in minutes (e.g., 25 for a standard Pomodoro)
        public TimeEntryStatusLevel Status { get; set; }
        public string Notes { get; set; } = "";

        public bool Save()
        {
            int cnt;
            if (Id == 0) //new
            {
                var sql = @"insert into timesheet(task_id, start_time, end_time,
duration, status, notes) values(?, ?, ?, ?, ?, ?)";
                object[] args = { TaskId, StartTime, EndTime, Duration, (int)Status, Notes };
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (cnt>0)
                    Id = Convert.ToInt32(DBAL.ExecuteScalar("SELECT last_insert_rowid()"));
                if (DBAL.LastError.Length > 0)
                    MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                var sql = @"UPDATE timesheet SET
                      task_id = ?, start_time = ?, end_time = ?, duration = ?, status = ?, notes = ?
                    WHERE id = ?";
                object[] args = { TaskId, StartTime, EndTime, Duration, (int)Status, Notes, Id };
                cnt =DBAL.ExecuteNonQuery(sql, args);
                if (DBAL.LastError.Length > 0)
                    MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return (cnt>0);
        }
    }
}
