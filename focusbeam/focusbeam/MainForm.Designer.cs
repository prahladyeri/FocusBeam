
namespace focusbeam
{
    partial class MainForm
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblTracker = new System.Windows.Forms.Label();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnMCQ = new System.Windows.Forms.Button();
            this.btnMindMaps = new System.Windows.Forms.Button();
            this.btnNotes = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.rpkProject = new focusbeam.Controls.RefPicker();
            this.rpkTaskItem = new focusbeam.Controls.RefPicker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnSettings, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnStart, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblTracker, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnAbout, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnMCQ, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnMindMaps, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnNotes, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDashboard, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rpkProject, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.rpkTaskItem, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(794, 51);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnSettings
            // 
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSettings.Location = new System.Drawing.Point(0, 27);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(158, 24);
            this.btnSettings.TabIndex = 5;
            this.btnSettings.Text = "⚙️ Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnStart
            // 
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStart.Location = new System.Drawing.Point(474, 27);
            this.btnStart.Margin = new System.Windows.Forms.Padding(0);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(158, 24);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "▶️ Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblTracker
            // 
            this.lblTracker.AutoSize = true;
            this.lblTracker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTracker.Location = new System.Drawing.Point(632, 27);
            this.lblTracker.Margin = new System.Windows.Forms.Padding(0);
            this.lblTracker.Name = "lblTracker";
            this.lblTracker.Size = new System.Drawing.Size(162, 24);
            this.lblTracker.TabIndex = 11;
            this.lblTracker.Text = "🕒 00:00:00";
            this.lblTracker.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAbout
            // 
            this.btnAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAbout.Location = new System.Drawing.Point(632, 0);
            this.btnAbout.Margin = new System.Windows.Forms.Padding(0);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(162, 27);
            this.btnAbout.TabIndex = 4;
            this.btnAbout.Text = "❓ About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnMCQ
            // 
            this.btnMCQ.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMCQ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMCQ.Location = new System.Drawing.Point(474, 0);
            this.btnMCQ.Margin = new System.Windows.Forms.Padding(0);
            this.btnMCQ.Name = "btnMCQ";
            this.btnMCQ.Size = new System.Drawing.Size(158, 27);
            this.btnMCQ.TabIndex = 3;
            this.btnMCQ.Text = "💡 MCQ";
            this.btnMCQ.UseVisualStyleBackColor = true;
            // 
            // btnMindMaps
            // 
            this.btnMindMaps.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMindMaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMindMaps.Location = new System.Drawing.Point(316, 0);
            this.btnMindMaps.Margin = new System.Windows.Forms.Padding(0);
            this.btnMindMaps.Name = "btnMindMaps";
            this.btnMindMaps.Size = new System.Drawing.Size(158, 27);
            this.btnMindMaps.TabIndex = 2;
            this.btnMindMaps.Text = "🧠 Mind Maps";
            this.btnMindMaps.UseVisualStyleBackColor = true;
            // 
            // btnNotes
            // 
            this.btnNotes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNotes.Location = new System.Drawing.Point(158, 0);
            this.btnNotes.Margin = new System.Windows.Forms.Padding(0);
            this.btnNotes.Name = "btnNotes";
            this.btnNotes.Size = new System.Drawing.Size(158, 27);
            this.btnNotes.TabIndex = 1;
            this.btnNotes.Text = "💬 Notes";
            this.btnNotes.UseVisualStyleBackColor = true;
            // 
            // btnDashboard
            // 
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDashboard.Location = new System.Drawing.Point(0, 0);
            this.btnDashboard.Margin = new System.Windows.Forms.Padding(0);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(158, 27);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "🚀 Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // rpkProject
            // 
            this.rpkProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rpkProject.Location = new System.Drawing.Point(158, 27);
            this.rpkProject.Margin = new System.Windows.Forms.Padding(0);
            this.rpkProject.MaximumSize = new System.Drawing.Size(500, 33);
            this.rpkProject.Name = "rpkProject";
            this.rpkProject.Size = new System.Drawing.Size(158, 24);
            this.rpkProject.TabIndex = 12;
            this.rpkProject.SelectedIndexChanged += new System.EventHandler(this.rpkProject_SelectedIndexChanged);
            this.rpkProject.AddButtonClicked += new System.EventHandler(this.rpkProject_AddButtonClicked);
            // 
            // rpkTaskItem
            // 
            this.rpkTaskItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rpkTaskItem.Location = new System.Drawing.Point(316, 27);
            this.rpkTaskItem.Margin = new System.Windows.Forms.Padding(0);
            this.rpkTaskItem.MaximumSize = new System.Drawing.Size(500, 33);
            this.rpkTaskItem.Name = "rpkTaskItem";
            this.rpkTaskItem.Size = new System.Drawing.Size(158, 24);
            this.rpkTaskItem.TabIndex = 13;
            this.rpkTaskItem.SelectedIndexChanged += new System.EventHandler(this.rpkTaskItem_SelectedIndexChanged);
            this.rpkTaskItem.Load += new System.EventHandler(this.rpkTaskItem_Load);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.toolStripStatusLabel1,
            this.progressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 601);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(794, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Ready";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(656, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(86, 16);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(94, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 51);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(794, 550);
            this.panelMain.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 623);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Corbel", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(810, 662);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Focus Beam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnMCQ;
        private System.Windows.Forms.Button btnMindMaps;
        private System.Windows.Forms.Button btnNotes;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Label lblTracker;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private Controls.RefPicker rpkProject;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private Controls.RefPicker rpkTaskItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panelMain;
    }
}

