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
            List<MindMap> ml = new List<MindMap>();
            DataTable tbl = DBAL.Execute("select * from mindmaps where project_id=? order by parent_id asc", 
                new object[] { projectId });
            foreach (DataRow row in tbl.Rows) {
                ml.Add( DBAL.DataRowToObject<MindMap>(row) );
            }
            return ml;
        }

        public bool Save() {
            string sql = "";
            int cnt;
            if (Id == 0)
            { //new
                sql = DBAL.ObjectToInsertQuery(this);
                object[] args = DBAL.GetParamValues(this);
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (cnt > 0) {
                    Id = Convert.ToInt32(DBAL.ExecuteScalar("SELECT last_insert_rowid()"));
                    if (DBAL.LastError.Length > 0)
                        MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                sql = DBAL.ObjectToUpdateQuery(this);
                object[] args = DBAL.GetParamValues(this);
                cnt = DBAL.ExecuteNonQuery(sql, args);
                if (DBAL.LastError.Length > 0)
                    MessageBox.Show(DBAL.LastError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return cnt > 0;
        }
    }
}
