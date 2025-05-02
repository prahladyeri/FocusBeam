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

namespace focusbeam.Models
{
    public enum CategoryLevel { Work, Study, Home, Other }
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public CategoryLevel Category { get; set; }
        public string Tags { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }

        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    }
}
