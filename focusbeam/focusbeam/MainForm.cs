/**
 * MainForm.cs - The main form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
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
        private DateTime _trackingStartedAt;

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
            //notifyIcon1.Text = AppDomain.CurrentDomain.app
            //this.Text += " " + Util.AssemblyInfoHelper.GetVersion();
            DBAL.Init();
            _projects = DBAL.GetAllProjects();

            foreach (Project proj in _projects) {
                this.rpkProject.cmbMain.Items.Add(proj.Title);
            }

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
        }


        private void rpkTaskItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string title = rpkTaskItem.cmbMain.SelectedItem.ToString();
            //currentProject = projects.FirstOrDefault(p => p.Title == title);
            _currentTask = _currentProject.Tasks.FirstOrDefault(t => t.Title == title);
            lblStatus.Text = $"Current task set to {title}";
            // TODO: update the view
            timesheetView1.dgv.Rows.Clear();
            _currentTask.TimeEntries.ForEach(te => {
                timesheetView1.dgv.Rows.Add(
                    te.StartTime,
                    te.EndTime,
                    te.Duration,
                    te.Status
                );
            });
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
                MessageBox.Show("Tracking is still in progress.");
            }
        }

        private void MinimizeToTray() {
            this.Hide();
            notifyIcon1.BalloonTipTitle = Util.AssemblyInfoHelper.Title;
            notifyIcon1.BalloonTipText = $"{Util.AssemblyInfoHelper.Title} is still running in the background. Right-click the tray icon to exit.";
            notifyIcon1.ShowBalloonTip(3000); // duration in milliseconds
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
                //TODO: Add te to database
                _currentTask.TimeEntries.Add(te);
                addToTimesheet(te);
            }
        }

        private void addToTimesheet(TimeEntry te) {
            timesheetView1.dgv.Rows.Insert(0,
                te.StartTime,
                te.EndTime,
                te.Duration.ToString("D2"),
                te.Status
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
    }
}
