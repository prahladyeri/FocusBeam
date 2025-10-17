namespace focusbeam.Controls
{
    partial class MindMapView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
                _saveTimer?.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MindMapView));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtNode = new System.Windows.Forms.TextBox();
            this.btnAddNodeToRoot = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createSubitemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(0, 34);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(263, 346);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.Click += new System.EventHandler(this.treeView1_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(271, 3);
            this.txtNotes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(543, 377);
            this.txtNotes.TabIndex = 2;
            this.txtNotes.TextChanged += new System.EventHandler(this.txtNotes_TextChanged);
            this.txtNotes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNotes_KeyDown);
            this.txtNotes.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtNotes_KeyUp);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(0, 378);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(814, 35);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "💾 Save Notes";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtNode
            // 
            this.txtNode.Location = new System.Drawing.Point(0, 3);
            this.txtNode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtNode.Name = "txtNode";
            this.txtNode.Size = new System.Drawing.Size(159, 23);
            this.txtNode.TabIndex = 4;
            // 
            // btnAddNodeToRoot
            // 
            this.btnAddNodeToRoot.Image = ((System.Drawing.Image)(resources.GetObject("btnAddNodeToRoot.Image")));
            this.btnAddNodeToRoot.Location = new System.Drawing.Point(167, 1);
            this.btnAddNodeToRoot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddNodeToRoot.Name = "btnAddNodeToRoot";
            this.btnAddNodeToRoot.Size = new System.Drawing.Size(97, 27);
            this.btnAddNodeToRoot.TabIndex = 5;
            this.btnAddNodeToRoot.Text = "Add Root";
            this.btnAddNodeToRoot.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddNodeToRoot.UseVisualStyleBackColor = true;
            this.btnAddNodeToRoot.Click += new System.EventHandler(this.btnAddNodeToRoot_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createSubitemToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(156, 48);
            // 
            // createSubitemToolStripMenuItem
            // 
            this.createSubitemToolStripMenuItem.Name = "createSubitemToolStripMenuItem";
            this.createSubitemToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.createSubitemToolStripMenuItem.Text = "Create Subitem";
            this.createSubitemToolStripMenuItem.Click += new System.EventHandler(this.createSubitemToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // MindMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAddNodeToRoot);
            this.Controls.Add(this.txtNode);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.treeView1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MindMapView";
            this.Size = new System.Drawing.Size(814, 413);
            this.Load += new System.EventHandler(this.MindMapView_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtNode;
        private System.Windows.Forms.Button btnAddNodeToRoot;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem createSubitemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}
