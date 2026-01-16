/**
 * MindMapView.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class MindMapView : UserControl
    {
        private Random rnd = new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        //private Timer _saveTimer;
        private bool _isInitializing = true;
        private MainForm _mainForm;
        private Project _currentProject;
        public TreeView TreeViewControl { get { return this.treeView1; } }
        public TextBox NotesControl { get { return this.txtNotes; } }
        private int _editVersion;

        public MindMapView(Project currentProject)
        {
            this._currentProject = currentProject;
            InitializeComponent();
        }

        private void MindMapView_Load(object sender, EventArgs e)
        {
            _mainForm = this.FindForm() as MainForm;
            _mainForm.SetStatus($"Editing mindmaps for project {_currentProject.Title}");
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
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
                mm.ParentId = int.Parse( parentId.Substring(1));
            
            mm.Save(); // generate id
            TreeNode node = new TreeNode(mm.Title)
            {
                Name = $"n{mm.Id}",
                Tag = mm
            };
            if (string.IsNullOrEmpty(parentId))
            {
                node.ImageKey = "folder";
                node.SelectedImageKey = "folder";
                mm.Position = treeView1.Nodes.Add(node);
            }
            else {
                node.ImageKey = "file";
                node.SelectedImageKey = "file";
                TreeNode[] matches = treeView1.Nodes.Find($"{parentId}", true);
                mm.Position = matches[0].Nodes.Add(node);
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

        private async void txtNotes_TextChanged(object sender, EventArgs e)
        {
            if (_isInitializing) return;
            if (treeView1.SelectedNode == null) return;

            int version = Interlocked.Increment(ref _editVersion);
            MindMap mm = treeView1.SelectedNode.Tag as MindMap;
            string snapshot = txtNotes.Text;
            await Task.Run(() =>
            {
                Thread.Sleep(150);
                if (version != _editVersion) return;

                mm.Notes = snapshot;
                var sql = "UPDATE mindmaps SET notes=? WHERE id=?";
                object[] args = { mm.Notes, mm.Id };
                int res = DBAL.ExecuteNonQuery(sql, args);
                if (res == -1)
                {
                    //_mainForm.SetStatus($"Error occurred: {DBAL.LastError}");
                    return;
                }
                //_mainForm.SetStatus($"Saved notes for {mm.Title}");
                _mainForm.BeginInvoke((Action)(() =>
                {
                    _mainForm.SetStatus($"Saved notes for {mm.Title}");
                }));

            });
        }

        private void txtNotes_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is MindMap mm)
            {
                txtNotes.ReadOnly = true;
                _isInitializing = true;
                Interlocked.Increment(ref _editVersion); // invalidate first
                txtNotes.Text = mm.Notes;// then mutate text
                _isInitializing = false;
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
