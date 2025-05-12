/**
 * Project.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace focusbeam.Models
{
    public enum CategoryLevel { 
        Work = 0, Study = 1, Home = 2, Other = 3
    }
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public CategoryLevel Category { get; set; }
        public string[] Tags { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public void Save()
        {
            var tags = string.Join(",", Tags);
            if (Id == 0) //new
            {
                var sql = @"insert into projects (title, category, tags, start_date, end_date, notes) 
values(?, ?, ?, ?, ?, ?)";
                object[] args = { Title, (int)Category, tags, StartDate, EndDate, Notes };
                DBAL.Execute(sql, args);
                Id = Convert.ToInt32(DBAL.ExecuteScalar("SELECT last_insert_rowid()"));
            }
            else
            {
                var sql = @"UPDATE projects SET title=?, category=?, tags=?, start_date = ?, end_date = ?, notes = ? WHERE id = ?";
                object[] args = { Title, (int)Category, tags, StartDate, EndDate, Notes, Id };
                DBAL.Execute(sql, args);
            }
            Tasks.ForEach(t => {
                t.ProjectId = this.Id;
                t.Save();
                });
        }
    }
}
