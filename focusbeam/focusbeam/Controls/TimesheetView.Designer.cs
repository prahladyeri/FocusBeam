
namespace focusbeam.Controls
{
    partial class TimesheetView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvTimesheet = new System.Windows.Forms.DataGridView();
            this.start_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimesheet)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTimesheet
            // 
            this.dgvTimesheet.AllowUserToDeleteRows = false;
            this.dgvTimesheet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimesheet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.start_time,
            this.end_time,
            this.duration,
            this.status,
            this.notes});
            this.dgvTimesheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTimesheet.Location = new System.Drawing.Point(0, 0);
            this.dgvTimesheet.Name = "dgvTimesheet";
            this.dgvTimesheet.RowHeadersVisible = false;
            this.dgvTimesheet.Size = new System.Drawing.Size(576, 390);
            this.dgvTimesheet.TabIndex = 1;
            // 
            // start_time
            // 
            this.start_time.HeaderText = "Start Time";
            this.start_time.Name = "start_time";
            this.start_time.ReadOnly = true;
            this.start_time.Width = 150;
            // 
            // end_time
            // 
            this.end_time.HeaderText = "End Time";
            this.end_time.Name = "end_time";
            this.end_time.ReadOnly = true;
            this.end_time.Width = 150;
            // 
            // duration
            // 
            this.duration.HeaderText = "Duration";
            this.duration.Name = "duration";
            this.duration.ReadOnly = true;
            this.duration.Width = 150;
            // 
            // status
            // 
            this.status.HeaderText = "Status";
            this.status.Name = "status";
            this.status.ReadOnly = true;
            this.status.Width = 150;
            // 
            // notes
            // 
            this.notes.HeaderText = "Notes";
            this.notes.Name = "notes";
            this.notes.ReadOnly = true;
            this.notes.Width = 150;
            // 
            // TimesheetView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvTimesheet);
            this.Name = "TimesheetView";
            this.Size = new System.Drawing.Size(576, 390);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimesheet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvTimesheet;
        private System.Windows.Forms.DataGridViewTextBoxColumn start_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn end_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn duration;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.DataGridViewTextBoxColumn notes;
    }
}
