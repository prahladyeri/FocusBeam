using System;
using System.Collections.Generic;
using System.Data;
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
        //public List<string> Tags { get; set; } = new List<string> { "roller", "coaster" };
        public string Notes { get; set; } = "";

        public static List<MindMap> GetAll() {
            List<MindMap> ml = new List<MindMap>();
            foreach (DataRow row in DBAL.Execute("select * from mindmaps order by parentid asc").Rows) {
                ml.Add( DBAL.DataRowToObject<MindMap>(row) );
            }
            return ml;
        }

        public bool Save() {
            //TODO: WIP
            string sql = "";
            if (Id == 0)
            { //new
                sql = DBAL.ObjectToInsertQuery(this);
                object[] args = DBAL.GetParamValues(this);
                DBAL.Execute(sql, args);
            }
            else {
                sql = DBAL.ObjectToUpdateQuery(this);
            }
            return false;
        }
    }
}
