/**
 * MindMapView.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class MindMapView : UserControl
    {
        public event EventHandler SaveButtonClicked;
        public TreeView TreeViewControl { get { return this.treeView1; } }
        public TextBox NotesControl { get { return this.txtNotes; } }

        public MindMapView()
        {
            InitializeComponent();
        }

        private void MindMapView_Load(object sender, EventArgs e)
        {
            btnSave.Font = new Font(btnSave.Font, FontStyle.Bold);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveButtonClicked?.Invoke(this, e);
        }

        private void btnAddNode_Click(object sender, EventArgs e)
        {
            MindMap mm = new MindMap();
            mm.Title = txtNode.Text;
            TreeNode node = new TreeNode(mm.Title);
            node.Name = "noname" + (new Random()).Next(100, 999).ToString();
            if (treeView1.SelectedNode != null)
            {
                mm.ParentId = (treeView1.SelectedNode.Tag as MindMap).Id;
                treeView1.SelectedNode.Nodes.Add(node);
            }
            else {
                treeView1.Nodes.Add(node);
            }
            txtNode.Text = "";
            node.Tag = mm;
            node.EnsureVisible();
            treeView1.SelectedNode = node;
            treeView1.Focus();
        }
    }
}
