/**
 * MainForm.cs - The main form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Controls;
using focusbeam.Models;
using focusbeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //this.Controls.SetChildIndex(menuStrip1, 0);          // menuStrip goes at top visually
            //this.Controls.SetChildIndex(tableLayoutPanel1, 1);   // tableLayoutPanel comes below it
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Util.FileHelper.GetEmbeddedIcon("focusbeam.files.logo.png");
            notifyIcon1.Icon = Util.FileHelper.GetEmbeddedIcon("focusbeam.files.logo.png", 16);
            notifyIcon1.Text = Util.AssemblyInfoHelper.Title;
            notifyIcon1.Visible = true;
            DBAL.Init("focusbeam.db");
            _projects = Project.GetAll();
            foreach (Project proj in _projects) {
                this.rpkProject.Items.Add(proj.Title);
            }
            dashboardToolStripMenuItem_Click_1(this, new EventArgs());
            rpkProject.SelectedIndex = 0;
        }


        private void rpkProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            string title = rpkProject.SelectedItem.ToString();
            _currentProject = _projects.FirstOrDefault(p => p.Title == title);
            rpkTaskItem.Items.Clear();
            foreach (TaskItem task in _currentProject.Tasks)
            {
                rpkTaskItem.Items.Add(task.Title);
            }
            rpkTaskItem.SelectedIndex = 0;
            // update the view
            
            if (_view.GetType() == typeof(TimesheetView)) {
                RefreshTimesheetGrid();
            }
        }

        private void RefreshTimesheetGrid() {
            if (_view.GetType().Name != "TimesheetView") return;
            var timesheetView = (TimesheetView)_view;
            timesheetView.dgv.Rows.Clear();
            decimal totLogged = 0;
            _currentProject.Tasks.ForEach(task => {
                totLogged += task.GetTotalLogged();
                addTaskToGrid(task);
            });
            lblLoggedHours.Text = (totLogged / 60).ToString("F2") + " hrs logged.";
        }

        private void rpkTaskItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string title = rpkTaskItem.SelectedItem.ToString();
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
                MessageBox.Show("Tracking is still in progress.", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            //TODO: Play custom sound before/after this.
            if (!_isTracking) {
                notifyIcon1.ShowBalloonTip(3000, 
                    Util.AssemblyInfoHelper.Title, 
                    "Started Tracking Time", 
                    ToolTipIcon.Info);
                btnStart.Text = "⏸️ Stop";
                _isTracking = true;
                _trackingStartedAt = DateTime.Now;
                FormHelper.CreateTooltip(lblTracker, $"Started tracking since {_trackingStartedAt.ToShortTimeString()}");
                timer1.Enabled = true;
            }
            else
            {
                //Stop logic
                notifyIcon1.ShowBalloonTip(3000, 
                    Util.AssemblyInfoHelper.Title, 
                    "Stopped Tracking Time", 
                    ToolTipIcon.Info);
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
                RefreshTimesheetGrid();
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

        private void rpkProject_EditButtonClicked(object sender, EventArgs e)
        {
            DynamicFormBuilder builder = new DynamicFormBuilder(new List<Field> {
                new Field{Name = "Title", Value = _currentProject.Title,
                    ControlType=FieldControlType.TextBox,
                    Required = true},
                new Field{Name = "Category",
                    Value = _currentProject.Category,  Required = true},
                new Field{
                    Name = "Tags",
                    ControlType = FieldControlType.Custom,
                    CustomControl = new TagsPicker {
                        Value = _currentProject.Tags
                    },
                },
                new Field {
                    Name = "StartDate",
                    ControlType = FieldControlType.DateTimePicker,
                    Value = _currentProject.StartDate,
                },
                new Field{
                    Name = "EndDate",
                    ControlType = FieldControlType.DateTimePicker,
                    Value = _currentProject.EndDate,
                },
                new Field {
                    Name = "Notes",  Value =_currentProject.Notes,
                    ControlType=FieldControlType.MultilineTextBox,
                }
            }, EditMode.Edit);
            builder.RecordValidating += (s, ev) => {
                //TagsPicker tp = (builder.FindControl("Tags") as TagsPicker);
                //if (tp.Value.Count == 0)
                //{
                //    MessageBox.Show("At least one tag must be added.");
                //    ev.Cancel = true;
                //    return;
                //}
                Project clone = Helper.DeepClone( _currentProject);
                Util.EntityMapper.MapFieldsToEntity(builder.FieldsToGenerate, clone);
                bool success = clone.Save();
                if (!success) {
                    ev.Cancel = true;
                    return;
                }
                _currentProject = clone;
                // Update in collection
                int index = _projects.FindIndex(p => p.Id == _currentProject.Id);
                if (index != -1)
                {
                    _projects[index] = _currentProject;
                }
                string oldTitle = rpkProject.Text;
                string newTitle = _currentProject.Title;
                if (!string.Equals(oldTitle, newTitle, StringComparison.Ordinal))
                {
                    if (rpkProject.Items.Contains(oldTitle))
                    {
                        rpkProject.Items.Remove(oldTitle);
                    }
                    if (!rpkProject.Items.Contains(newTitle))
                    {
                        rpkProject.Items.Add(newTitle);
                    }
                    rpkProject.Text = newTitle;
                }
                MessageBox.Show(FormHelper.RecordSaveMessage(_currentProject), ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            DialogResult result = builder.ShowDialog();
        }

        private void rpkProject_AddButtonClicked(object sender, EventArgs e)
        {
            //List<string> theTags = new List<string> { "PHP", "Java", "dotnet" };
            Project project = new Project();
            DynamicFormBuilder builder = new DynamicFormBuilder(new List<Field> {
                new Field{Name = "Title", Value = "",
                    ControlType=FieldControlType.TextBox,  
                    Required = true},
                new Field{Name = "Category", 
                    ControlType=FieldControlType.Auto , 
                    Value = CategoryLevel.Home,  Required = true},
                new Field{
                    Name = "Tags",
                    ControlType = FieldControlType.Custom,
                    CustomControl = new TagsPicker { 
                        Value = project.Tags
                    },
                },
                new Field{
                    Name = "StartDate",
                    ControlType = FieldControlType.DateTimePicker,
                    Value = project.StartDate,
                },
                new Field{
                    Name = "EndDate",
                    ControlType = FieldControlType.DateTimePicker,
                    Value = project.EndDate,
                },
                new Field { 
                    Name = "Notes",  Value = "",
                    ControlType=FieldControlType.MultilineTextBox,
                }
            }, EditMode.Add);
            builder.RecordValidating += (s, ev) => {
                //TagsPicker tp = (builder.FindControl("Tags") as TagsPicker);
                //if (tp.Value.Count == 0)
                //{
                //    MessageBox.Show("At least one tag must be added.");
                //    ev.Cancel = true;
                //    return;
                //}
                Util.EntityMapper.MapFieldsToEntity(builder.FieldsToGenerate, project);
                bool success = project.Save();
                if (!success) {
                    ev.Cancel = true;
                    return;
                }
                TaskItem task = new TaskItem
                {
                    ProjectId = project.Id,
                    Title = "Default Task",
                    Priority = PriorityLevel.High,
                    Status = StatusLevel.Pending,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                };
                project.Tasks.Add(task);
                task.Save();
                _projects.Add(project);
                rpkProject.Items.Add(project.Title);
                rpkProject.Text = project.Title;
                MessageBox.Show(FormHelper.RecordSaveMessage(project), Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            builder.ShowDialog();
        }

        private void rpkTaskItem_AddButtonClicked(object sender, EventArgs e)
        {
            TaskItem task = new TaskItem { ProjectId = _currentProject.Id };
            DynamicFormBuilder builder = new DynamicFormBuilder(new List<Field> {
                new Field {
                    Name = "Title",
                    Required = true,
                },
                new Field{
                    Name = "Priority",
                    Value = PriorityLevel.Critical,
                },
                new Field{
                    Name = "Status",
                    Value = StatusLevel.Pending,
                },
                new Field{
                    Name = "Tags",
                    ControlType = FieldControlType.Custom,
                    CustomControl = new TagsPicker {
                        Value = task.Tags,
                    }
                },
                new Field {
                    Name = "StartDate",
                    ControlType = FieldControlType.DateTimePicker,
                    Value = task.StartDate,
                },
                new Field {
                    Name = "EndDate",
                    ControlType = FieldControlType.DateTimePicker,
                    Value = task.EndDate,
                },
                new Field {
                    Name = "PlannedHours",
                    ControlType = FieldControlType.NumericUpDown,
                    Value = 1,
                    Properties = {
                        {"Minimum", 1 },
                    }
                },
                new Field {
                    Name = "Notes",
                    ControlType = FieldControlType.MultilineTextBox,
                    Value = "",
                },
            }, EditMode.Add);
            builder.RecordValidating += (s, ev) => {
                Util.EntityMapper.MapFieldsToEntity(builder.FieldsToGenerate, task);
                bool success = task.Save();
                if (!success)
                {
                    ev.Cancel = true;
                    return;
                }
                _currentProject.Tasks.Add(task);
                rpkTaskItem.Items.Add(task.Title);
                rpkTaskItem.Text = task.Title;
                MessageBox.Show(FormHelper.RecordSaveMessage(task), Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshTimesheetGrid();
            };

            builder.ShowDialog();
        }

        private void rpkTaskItem_EditButtonClicked(object sender, EventArgs e)
        {
            TaskItem task = _currentProject.Tasks.Find(ti => ti.Title == rpkTaskItem.Text);
            int taskidx = rpkTaskItem.Items.IndexOf(task.Title);
            //TaskItem task = new TaskItem { ProjectId = _currentProject.Id };
            DynamicFormBuilder builder = new DynamicFormBuilder(new List<Field> {
                new Field {
                    Name = "Title",
                    Required = true,
                    Value = task.Title,
                },
                new Field{
                    Name = "Priority",
                    Value = task.Priority,
                },
                new Field{
                    Name = "Status",
                    Value = task.Status,
                },
                new Field{
                    Name = "Tags",
                    ControlType = FieldControlType.Custom,
                    CustomControl = new TagsPicker {
                        Value = task.Tags,
                    }
                },
                new Field {
                    Name = "StartDate",
                    ControlType = FieldControlType.DateTimePicker,
                    Value = task.StartDate,
                },
                new Field {
                    Name = "EndDate",
                    ControlType = FieldControlType.DateTimePicker,
                    Value = task.EndDate,
                },
                new Field {
                    Name = "PlannedHours",
                    ControlType = FieldControlType.NumericUpDown,
                    Value = task.PlannedHours,
                    Properties = {
                        {"Minimum", 1 },
                    }
                },
                new Field {
                    Name = "Notes",
                    ControlType = FieldControlType.MultilineTextBox,
                    Value = task.Notes,
                },
            }, EditMode.Add);
            builder.RecordValidating += (s, ev) => {
                Util.EntityMapper.MapFieldsToEntity(builder.FieldsToGenerate, task);
                bool success = task.Save();
                if (!success)
                {
                    ev.Cancel = true;
                    return;
                }
                //_currentProject.Tasks.Add(task);
                rpkTaskItem.Items[taskidx] = task.Title;
                //rpkTaskItem.Text = task.Title;
                MessageBox.Show(FormHelper.RecordSaveMessage(task), Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshTimesheetGrid();
            };
            builder.ShowDialog();
        }


        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.panelMain.Controls.Clear();
            AboutView theView = new AboutView();
            theView.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(theView);
            _view = theView;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dashboardToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.panelMain.Controls.Clear();
            TimesheetView theView = new TimesheetView();
            theView.dgv.CellContentClick += (object s, DataGridViewCellEventArgs ev) => {
                if (ev.RowIndex >= 0 && theView.dgv.Columns[ev.ColumnIndex].Name == "timesheet")
                {
                    string taskTitle = theView.dgv.Rows[ev.RowIndex].Cells["Title"].Value?.ToString();
                    TimesheetForm dialog = new TimesheetForm();
                    DataGridView dgv = dialog.Controls["dgvEntries"] as DataGridView;
                    List<TimeEntry> entries = _currentProject.Tasks.Find(t => t.Title == taskTitle).TimeEntries;
                    if (entries.Count == 0)
                    {
                        MessageBox.Show($"No entries found.", ProductName,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    entries.ForEach(te =>
                    {
                        dgv.Rows.Add(te.StartTime.ToString(), te.EndTime.ToString(),
                            te.Status.ToString(), te.Duration, te.Notes);
                    });
                    dialog.ShowDialog();
                }
            };
            theView.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(theView);
            _view = theView;
            rpkProject.SelectedIndex = 0;
            RefreshTimesheetGrid();
        }

        private void mindMapsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void mCQToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            this.panelMain.Controls.Clear();
            SettingsView theView = new SettingsView();
            theView.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(theView);
            _view = theView;
        }

    }
}
