/**
 * MindMap.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

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

        public static List<MindMap> GetAll(int projectId) {
            List<MindMap> mmaps = new List<MindMap>();
            DataTable tbl = DBAL.Execute("select * from mindmaps where project_id=? order by parent_id asc", 
                new object[] { projectId });
            foreach (DataRow row in tbl.Rows) {
                //ml.Add( DBAL.DataRowToObject<MindMap>(row) );
                var mm = new MindMap {
                    Id = Convert.ToInt32(row["id"]),
                    Title = row.Field<string>("title"),
                    ProjectId = row.Field<int>("project_id"),
                    ParentId = row.Field<int>("parent_id"),
                    Notes = row.Field<string>("notes"),
                };
                mmaps.Add(mm);
            }
            return mmaps;
        }

        public bool Save() {
            string sql = "";
            int cnt;
            if (Id == 0)
            { //new
                //sql = DBAL.ObjectToInsertQuery(this, "mindmaps");
                sql = "INSERT INTO mindmaps (parent_id, project_id, title, notes) VALUES (?, ?, ?, ?)";
                //object[] args = DBAL.GetParamValues(this, false);
                object[] args = { this.ParentId, this.ProjectId, Title, Notes };
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (cnt > 0) 
                    Id = Convert.ToInt32(DBAL.ExecuteScalar("SELECT last_insert_rowid()"));
                if (DBAL.LastError.Length > 0)
                        MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                //sql = DBAL.ObjectToUpdateQuery(this, "mindmaps");
                sql = "update mindmaps set title=?, notes=? where id=?";
                //object[] args = DBAL.GetParamValues(this);
                object[] args = { Title, Notes, Id };
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (DBAL.LastError.Length > 0)
                    MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return cnt > 0;
        }
    }
}
