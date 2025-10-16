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
        Random rnd = new Random(987589);
        private Project _currentProject;
        public TreeView TreeViewControl { get { return this.treeView1; } }
        public TextBox NotesControl { get { return this.txtNotes; } }

        public MindMapView(Project currentProject)
        {
            this._currentProject = currentProject;
            InitializeComponent();
        }

        private void MindMapView_Load(object sender, EventArgs e)
        {
            btnSave.Font = new Font(btnSave.Font, FontStyle.Bold);
        }

        private void SaveNodes(TreeNodeCollection nodes) 
        {
            List<MindMap> _mindmaps = MindMap.GetAll(_currentProject.Id);
            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode node = nodes[i];
                MindMap m;
                if (node.Name.StartsWith("noname")) //new node
                {
                    m = new MindMap();
                    m.Title = node.Text;
                    m.Notes = (node.Tag as MindMap).Notes;
                    m.ProjectId = _currentProject.Id;
                    if (node.Parent != null) // non-root node
                        m.ParentId = int.Parse(node.Parent.Name);
                    m.Save();
                    node.Name = m.Id.ToString();
                }
                else
                {
                    m = _mindmaps.Find(mm => mm.Id == Convert.ToInt32(node.Name));
                    m.Notes = (node.Tag as MindMap).Notes;
                    m.Save();
                }
                SaveNodes(node.Nodes);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //SaveButtonClicked?.Invoke(this, e);
            SaveNodes(treeView1.Nodes);
            MessageBox.Show("Node data saved", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAddNode_Click(object sender, EventArgs e)
        {
            if (txtNode.Text.Trim().Length == 0) {
                txtNode.Focus();
                return;
            }
            TreeNode node = CreateNewMindMap(txtNode.Text);

            node.EnsureVisible();
            treeView1.SelectedNode = node;
            treeView1.Focus();
        }

        private TreeNode CreateNewMindMap(string text, string parentId = "") {
            MindMap mm = new MindMap();
            mm.Title = text;
            TreeNode node = new TreeNode(mm.Title);
            node.Name = "noname" + (rnd).Next(0, 9999).ToString();
            if (parentId != "")
            {
                treeView1.Nodes[parentId].Nodes.Add(node);
                mm.ParentId = int.Parse( parentId);
            }
            treeView1.Nodes.Add(node);
            txtNode.Text = "";
            node.Tag = mm;
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
            (treeView1.SelectedNode.Tag as MindMap).Notes = txtNotes.Text;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is MindMap mm)
            {
                txtNotes.Text = mm.Notes;
                txtNotes.ReadOnly = false;

                // Optional small delay to ensure focus "sticks"
                BeginInvoke((Action)(() => txtNotes.Focus()));
            }
        }
    }
}
