/**
 * TaskItem.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace focusbeam.Models
{
    public enum PriorityLevel { Low, Medium, High, Critical }
    public enum StatusLevel { Pending, Completed}
    public enum KanbanStatusLevel { ToDo, InProgress, Review, Done}

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
        public KanbanStatusLevel KanbanStatus { get; set; }
        public List<string> Tags { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PlannedHours { get; set; }
        public string Notes { get; set; }

        public List<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    }
}
