/**
 * MainForm.cs - The main form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Controls;
using focusbeam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam
{
    public partial class MainForm : Form
    {
        private List<Project> _projects = null;
        private Project _currentProject = null;
        private TaskItem _currentTask = null;
        private bool _isExiting = false;
        private bool _isTracking = false;
        private bool _isMinimizedTrayWarningShown = false;
        private DateTime _trackingStartedAt;
        private Control _view = new Control(); //current view

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Util.FileHelper.GetEmbeddedIcon("focusbeam.files.logo.png");
            notifyIcon1.Icon = Util.FileHelper.GetEmbeddedIcon("focusbeam.files.logo.png", 16);
            notifyIcon1.Text = Util.AssemblyInfoHelper.Title;
            notifyIcon1.Visible = true;
            DBAL.Init();
            _projects = Project.GetAll();
            foreach (Project proj in _projects) {
                this.rpkProject.cmbMain.Items.Add(proj.Title);
            }
            btnDashboard_Click(this, new EventArgs());
            rpkProject.cmbMain.SelectedIndex = 0;
        }


        private void rpkProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            string title = rpkProject.cmbMain.SelectedItem.ToString();
            _currentProject = _projects.FirstOrDefault(p => p.Title == title);
            rpkTaskItem.cmbMain.Items.Clear();
            foreach (TaskItem task in _currentProject.Tasks)
            {
                rpkTaskItem.cmbMain.Items.Add(task.Title);
            }
            rpkTaskItem.cmbMain.SelectedIndex = 0;
            // update the view
            
            if (_view.GetType() == typeof(TimesheetView)) {
                RefreshTimesheetGrid();
            }
        }

        private void RefreshTimesheetGrid() {
            var timesheetView = (TimesheetView)_view;
            timesheetView.dgv.Rows.Clear();
            _currentProject.Tasks.ForEach(task => {
                addTaskToGrid(task);
            });
        }

        private void rpkTaskItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string title = rpkTaskItem.cmbMain.SelectedItem.ToString();
            //currentProject = projects.FirstOrDefault(p => p.Title == title);
            _currentTask = _currentProject.Tasks.FirstOrDefault(t => t.Title == title);
            lblStatus.Text = $"Current task set to {_currentProject.Title} => {title}";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _isExiting = true;
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                MinimizeToTray();
            }
            else if (_isTracking)
            {
                e.Cancel = true;
                _isExiting = false;
                MessageBox.Show("Tracking is still in progress.");
            }
            else {
                DBAL.Dispose();
            }
        }

        private void MinimizeToTray() {
            this.Hide();
            if (!_isMinimizedTrayWarningShown) { 
                notifyIcon1.BalloonTipTitle = Util.AssemblyInfoHelper.Title;
                notifyIcon1.BalloonTipText = $"{Util.AssemblyInfoHelper.Title} is still running in the background. Right-click the tray icon to exit.";
                notifyIcon1.ShowBalloonTip(3000); // duration in milliseconds
                _isMinimizedTrayWarningShown = true;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!_isTracking) {
                //Start logic
                notifyIcon1.ShowBalloonTip(3000, Util.AssemblyInfoHelper.Title, "Started Tracking Time", ToolTipIcon.Info);
                btnStart.Text = "⏸️ Stop";
                _isTracking = true;
                _trackingStartedAt = DateTime.Now;
                timer1.Enabled = true;
            }
            else
            {
                //Stop logic
                notifyIcon1.ShowBalloonTip(3000, Util.AssemblyInfoHelper.Title, "Stopped Tracking Time", ToolTipIcon.Info);
                btnStart.Text = "▶️ Start" ;
                _isTracking = false;
                timer1.Enabled = false;
                var endTime = DateTime.Now;
                TimeSpan ts = endTime - _trackingStartedAt;
                var te = new TimeEntry
                {
                    TaskId = _currentTask.Id,
                    StartTime = _trackingStartedAt,
                    EndTime =endTime,
                    Duration = (int)Math.Ceiling(ts.TotalMinutes),
                    Status = TimeEntryStatusLevel.Completed,
                };
                te.Save();
                _currentTask.TimeEntries.Add(te);
            }
        }

        private void addTaskToGrid(TaskItem task) {
            var timesheetView = (TimesheetView)_view;
            timesheetView.dgv.Rows.Insert(0,
                task.Title,
                task.Priority,
                task.Status,
                ((double)task.GetTotalLogged() / 60).ToString("0.00")
            );
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //🕒 00:00:00
            TimeSpan ts = DateTime.Now.Subtract(_trackingStartedAt);
            lblTracker.Text = "🕒" + ts.ToString(@"hh\:mm\:ss");
        }

        private void rpkTaskItem_Load(object sender, EventArgs e)
        {

        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            this.panelMain.Controls.Clear();
            SettingsView theView = new SettingsView();
            theView.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(theView);
            _view = theView;
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            this.panelMain.Controls.Clear();
            TimesheetView theView = new TimesheetView();
            theView.dgv.CellContentClick += (object s, DataGridViewCellEventArgs ev) => {
                if (ev.RowIndex >= 0 && theView.dgv.Columns[ev.ColumnIndex].Name == "timesheet")
                {
                    string taskTitle = theView.dgv.Rows[ev.RowIndex].Cells["Title"].Value?.ToString();
                    MessageBox.Show($"Timesheet for task: {taskTitle}");
                }
            };
            theView.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(theView);
            _view = theView;
            rpkProject.cmbMain.SelectedIndex = 0;
            RefreshTimesheetGrid();
        }

        private void rpkProject_AddButtonClicked(object sender, EventArgs e)
        {
            DynamicFormBuilder dialog = new DynamicFormBuilder();
            dialog.AddField("Title", "");
            dialog.AddField("Category", CategoryLevel.Home);
            dialog.AddField("Tags", "");
            dialog.AddField("Price", 0.00, new Dictionary<string, object> {
                { "Minimum", 1 },
            });
            dialog.AddField("Notes", "", new Dictionary<string, object> {
                { "Multiline", true },
                { "Height", 60}
            });

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                MessageBox.Show("Save button clicked.");
            }
            else if (result == DialogResult.Abort) {
                MessageBox.Show("Delete button clicked.");
            }
            else if (result == DialogResult.Cancel) {
                MessageBox.Show("Cancel button clicked.");
            }
        }
    }
}
