/**
 * MainForm.cs - The main form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Controls;
using focusbeam.Helpers;
using focusbeam.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                tableLayoutPanel1,
                new object[] { true });

            this.Icon = FileHelper.GetEmbeddedIcon("focusbeam.files.logo.png");
            notifyIcon1.Icon = FileHelper.GetEmbeddedIcon("focusbeam.files.logo.png", 16);
            notifyIcon1.Text = AssemblyInfoHelper.Title;
            notifyIcon1.Visible = true;
            FormHelper.CreateTooltip(btnTaskNotes, "Task Notes");
            FormHelper.CreateTooltip(btnStart, "Start Tracking");
            bool isnew = DBAL.Init("focusbeam.db",
                FileHelper.ReadEmbeddedResource(typeof(Program).Namespace + ".files.init.sql"));
            if (isnew)
            {
                //Create default project, task, etc.
                Project project = new Project
                {
                    Title = "First Project",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(90),
                };
                project.Tasks = new List<TaskItem> {
                    new TaskItem {
                        ProjectId = project.Id,
                        Title = "First Task",
                        Priority = PriorityLevel.High,
                        Status = StatusLevel.Pending,
                        StartDate = project.StartDate,
                        EndDate = project.EndDate,
                    },
                };
                project.Save();
            }

            _projects = Project.GetAll();
            foreach (Project proj in _projects) {
                this.rpkProject.Items.Add(proj.Title);
            }
            dashboardToolStripMenuItem_Click_1(this, new EventArgs());
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
            RefreshView();
        }

        private void RefreshView() {
            Control view;  decimal totLogged = 0;
            if (_currentProject == null) return;
            switch (_view) {
                case TimesheetView tv:
                    tv.dgv.Rows.Clear();
                    _currentProject.Tasks.ForEach(task => {
                        totLogged += task.GetTotalLogged();
                        addTaskToGrid(task);
                    });
                    lblLoggedHours.Text = (totLogged / 60).ToString("F2") + " hrs logged.";
                    break;
                case NoteView nv:
                    nv.Text = _currentTask.Notes;
                    break;
            }
        }

        private void rpkTaskItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string title = rpkTaskItem.SelectedItem.ToString();
            //currentProject = projects.FirstOrDefault(p => p.Title == title);
            _currentTask = _currentProject.Tasks.FirstOrDefault(t => t.Title == title);
            lblStatus.Text = $"Current task set to {_currentProject.Title} => {title}";
            RefreshView();
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

        public void SetStatus(string text)
        {
            lblStatus.Text = text;
        }

        private void MinimizeToTray() {
            this.Hide();
            if (!_isMinimizedTrayWarningShown) { 
                notifyIcon1.BalloonTipTitle = AssemblyInfoHelper.Title;
                notifyIcon1.BalloonTipText = $"{AssemblyInfoHelper.Title} is still running in the background. Right-click the tray icon to exit.";
                notifyIcon1.ShowBalloonTip(3000); // duration in milliseconds
                _isMinimizedTrayWarningShown = true;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //TODO: Play custom sound before/after this.
            if (!_isTracking) {
                notifyIcon1.ShowBalloonTip(3000, 
                    AssemblyInfoHelper.Title, 
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
                    AssemblyInfoHelper.Title, 
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
                FormHelper.CreateTooltip(lblTracker, $"Stopped tracking at {DateTime.Now.ToShortTimeString()}");
                RefreshView();
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
            //TODO: change color of row-header of added row depending on Priority:
            var row = timesheetView.dgv.Rows[0];
            switch (task.Priority) {
                case PriorityLevel.Critical:
                    row.HeaderCell.Style.BackColor = Color.Red;
                    break;
                case PriorityLevel.High:
                    row.HeaderCell.Style.BackColor = Color.Orange;
                    break;
                case PriorityLevel.Low:
                    row.HeaderCell.Style.BackColor = Color.Yellow;
                    break;
                case PriorityLevel.Medium:
                    row.HeaderCell.Style.BackColor = Color.Green;
                    break;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //🕒 00:00:00
            //tableLayoutPanel1.SuspendLayout();
            TimeSpan ts = DateTime.Now.Subtract(_trackingStartedAt);
            lblTracker.Text = "🕒" + ts.ToString(@"hh\:mm\:ss");
            //tableLayoutPanel1.ResumeLayout();

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
                EntityMapper.MapFieldsToEntity(builder.FieldsToGenerate, clone);
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
                EntityMapper.MapFieldsToEntity(builder.FieldsToGenerate, project);
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
                    Value = task.PlannedHours,
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
                EntityMapper.MapFieldsToEntity(builder.FieldsToGenerate, task);
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
                RefreshView();
            };

            builder.ShowDialog();
        }

        private void rpkTaskItem_EditButtonClicked(object sender, EventArgs e)
        {
            //TaskItem task = _currentProject.Tasks.Find(ti => ti.Title == rpkTaskItem.Text);
            TaskItem task = _currentTask;
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
                TaskItem clone =  Helper.DeepClone(task);
                EntityMapper.MapFieldsToEntity(builder.FieldsToGenerate, clone);
                bool success = clone.Save();
                if (!success)
                {
                    ev.Cancel = true;
                    return;
                }
                task = clone;
                // Update in collection
                int index = _currentProject.Tasks.FindIndex(t => t.Id == task.Id);
                if (index != -1)
                {
                    _currentProject.Tasks[index] = task;
                    _currentTask = task;
                }
                //_currentProject.Tasks.Add(task);
                rpkTaskItem.Items[taskidx] = task.Title;
                //rpkTaskItem.Text = task.Title;
                MessageBox.Show(FormHelper.RecordSaveMessage(task), Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (_view is TimesheetView)
                {
                    RefreshView();
                }
                else if (_view is NoteView) {
                    _view.Text = _currentTask.Notes;
                }
            };
            builder.ShowDialog();
        }

        private void setView(Control theView) {
            this.panelMain.Controls.Clear();
            theView.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(theView);
            this.Text = ProductName + " - " + theView.Name.TrimEnd("View".ToCharArray());
            _view = theView;
            switch (theView) {
                case TimesheetView tv:
                    RefreshView();
                    break;
            }
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            setView(new AboutView());
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dashboardToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            TimesheetView view = new TimesheetView();
            view.dgv.CellContentClick += (object s, DataGridViewCellEventArgs ev) => {
                if (ev.RowIndex >= 0 && view.dgv.Columns[ev.ColumnIndex].Name == "timesheet")
                {
                    string taskTitle = view.dgv.Rows[ev.RowIndex].Cells["Title"].Value?.ToString();
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
            setView(view);
            view.dgv.RowHeadersVisible = true;
            view.dgv.RowHeadersWidth = 10;
            rpkProject.SelectedIndex = 0;
        }

        private void mindMapsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            List<MindMap> _mindmaps = MindMap.GetAll(_currentProject.Id);
            MindMapView view = new MindMapView(_currentProject);
            ImageList treeImages = new ImageList();
            treeImages.Images.Add("folder", Properties.Resources.folder_icon);
            treeImages.Images.Add("file", Properties.Resources.file_icon);
            view.TreeViewControl.ImageList = treeImages;

            for (int i = 0; i < _mindmaps.Count; i++) 
            {
                MindMap mm = _mindmaps[i];
                TreeNode node = new TreeNode 
                {
                    Name = $"n{mm.Id}",
                    Text = mm.Title,
                    Tag = mm
                };
                //node.ContextMenuStrip = (view.Controls["contextMenuStrip1"] as ContextMenuStrip);
                if (mm.ParentId == 0)
                {
                    node.ImageKey = "folder";
                    node.SelectedImageKey = "folder";
                    view.TreeViewControl.Nodes.Add(node);
                    node.Expand();
                }
                else {
                    node.ImageKey = "file";
                    node.SelectedImageKey = "file";
                    TreeNode[] matches = view.TreeViewControl.Nodes.Find($"n{mm.ParentId}", true);
                    matches[0].Nodes.Add(node);
                }
            }
            setView(view);
        }

        private void mCQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Under Construction", Application.ProductName,
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void settingsToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            setView(new SettingsView());
        }

        private void btnTaskNotes_Click(object sender, EventArgs e)
        {
            NoteView view = new NoteView(_currentTask.Notes);
            view.SaveButtonClicked += (s, ev) => {
                _currentTask.Notes = view.Text;
                _currentTask.Save();
                MessageBox.Show("Notes saved.", ProductName,
                    MessageBoxButtons.OK,  MessageBoxIcon.Information);
            };
            setView(view);
        }
    }
}
