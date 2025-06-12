using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace focusbeam.Models
{
    internal class MindMap
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int ProjectId { get; set; } // references projects.id
        public string Title { get; set; } = "Leaf#" + (new Random().Next(1000, 9999).ToString());
        public List<string> Tags { get; set; } = new List<string> { "roller", "coaster" };
        public string Notes { get; set; } = "";
    }
}
