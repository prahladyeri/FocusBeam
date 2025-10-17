/**
 * MindMapView.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class MindMapView : UserControl
    {
        private Random rnd = new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        private System.Windows.Forms.Timer _saveTimer;
        private MindMap _editingMindMap;
        private Project _currentProject;
        public TreeView TreeViewControl { get { return this.treeView1; } }
        public TextBox NotesControl { get { return this.txtNotes; } }

        public MindMapView(Project currentProject)
        {
            this._currentProject = currentProject;
            InitializeComponent();

            _saveTimer = new System.Windows.Forms.Timer();
            _saveTimer.Interval = 2000; // 2 seconds idle delay
            _saveTimer.Tick += _saveTimer_Tick;
        }

        
        private void _saveTimer_Tick(object sender, EventArgs e)
        {
            _saveTimer.Stop(); // prevent multiple triggers
            if (_editingMindMap != null)
            {
                _editingMindMap.Save();
                var mainForm = this.FindForm() as MainForm;
                mainForm.SetStatus($"{_editingMindMap.Title} notes saved.");
            }
        }

        private void MindMapView_Load(object sender, EventArgs e)
        {
            //btnSave.Font = new Font(btnSave.Font, FontStyle.Bold);
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            //SaveButtonClicked?.Invoke(this, e);
            //SaveNodes(treeView1.Nodes);
            //MessageBox.Show("Node data saved", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAddNodeToRoot_Click(object sender, EventArgs e)
        {
            if (txtNode.Text.Trim().Length == 0) {
                txtNode.Focus();
                return;
            }
            TreeNode node = CreateNewMindMap(txtNode.Text, "");

            node.EnsureVisible();
            treeView1.SelectedNode = node;
            treeView1.Focus();
        }

        private TreeNode CreateNewMindMap(string text, string parentId = "") 
        {
            MindMap mm = new MindMap
            {
                Title = text,
                ProjectId = _currentProject.Id
            };
            
            if (!string.IsNullOrEmpty(parentId))
                mm.ParentId = int.Parse( parentId);
            
            mm.Save(); // generate id
            TreeNode node = new TreeNode(mm.Title)
            {
                Name = mm.Id.ToString(),
                Tag = mm
            };
            if (!string.IsNullOrEmpty(parentId))
            {
                mm.Position = treeView1.Nodes[parentId].Nodes.Add(node);
            }
            else {
                mm.Position = treeView1.Nodes.Add(node);
            }
            mm.Save(); // save position
            txtNode.Text = "";
            return node;
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
        }

        private void txtNotes_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtNotes_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtNotes_KeyUp(object sender, KeyEventArgs e)
        {
            if (treeView1.SelectedNode == null) return;
            _editingMindMap = (treeView1.SelectedNode.Tag as MindMap);
            _editingMindMap.Notes = txtNotes.Text;

            _saveTimer.Stop();  // reset timer
            _saveTimer.Start(); // start countdown again
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is MindMap mm)
            {
                txtNotes.Text = mm.Notes;
                txtNotes.ReadOnly = false;

                // Optional small delay to ensure focus "sticks"
                //BeginInvoke((Action)(() => txtNotes.Focus()));
            }
        }

        private void createSubitemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            CreateNewMindMap($"noname{rnd.Next(1, 999999).ToString("D6")}", node.Name).EnsureVisible();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node; // select the node under mouse
            }
        }

        private void DeleteNodes(TreeNodeCollection nodes)
        {
            // iterate backwards when removing nodes from collection
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                DeleteNodes(nodes[i].Nodes); // delete subnodes first
                MindMap mm = nodes[i].Tag as MindMap;
                var sql = "delete from mindmaps where id=?";
                object[] args = { mm.Id };
                DBAL.ExecuteNonQuery(sql, args);
                nodes[i].Remove();
            }
        }


        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            var result = MessageBox.Show(
                $"Are you sure you want to delete '{node.Text}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            //if (node.Nodes.Count > 0)
            DeleteNodes(node.Nodes); // delete subnodes first
            MindMap mm = node.Tag as MindMap;
            var sql = "delete from mindmaps where id=?";
            object[] args = {mm.Id };
            DBAL.ExecuteNonQuery(sql, args);
            node.Remove();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null) return; // user cancelled

            MindMap mm = e.Node.Tag as MindMap;
            var sql = "UPDATE mindmaps SET title=? WHERE id=?";
            object[] args = { e.Label, mm.Id };
            int res = DBAL.ExecuteNonQuery(sql, args);
            if (res == -1) {
                e.CancelEdit = true;
                MessageBox.Show($"Error occurred: {DBAL.LastError}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            mm.Title = e.Label;
        }
    }
}
