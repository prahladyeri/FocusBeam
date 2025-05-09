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

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //DBAL.Init();
            //projects = DBAL.GetAllProjects();
            this.Icon = Util.FileHelper.GetEmbeddedIcon("focusbeam.files.logo.png");
            notifyIcon1.Icon = Util.FileHelper.GetEmbeddedIcon("focusbeam.files.logo.png", 16);
            notifyIcon1.Text = Util.AssemblyInfoHelper.Title;
            notifyIcon1.Visible = true;
            //notifyIcon1.Text = AppDomain.CurrentDomain.app
            //this.Text += " " + Util.AssemblyInfoHelper.GetVersion();

            var project = new Project {
                Id = 1,
                Title = "Default Project",
                Category = CategoryLevel.Work,
                Tags = "",
                StartDate = new DateTime(2025, 05, 04),
                EndDate = new DateTime(2025, 05, 31),
                Notes = "",
            };
            project.Tasks = new List<TaskItem> {
                new TaskItem {
                    ProjectId = project.Id,
                    Title = "Apar QRLabel",
                    Priority = PriorityLevel.High,
                    Status = StatusLevel.Pending,
                    Tags = new List<string>{ "Tango", "Charlie" },
                },
                new TaskItem {
                    ProjectId = project.Id,
                    Title = "Wave PHP",
                    Priority = PriorityLevel.High,
                    Status = StatusLevel.Pending,
                    Tags = new List<string>{ "Tango", "Charlie" },
                },
                new TaskItem {
                    ProjectId = project.Id,
                    Title = "Focus Beam",
                    Priority = PriorityLevel.High,
                    Status = StatusLevel.Pending,
                    Tags = new List<string>{ "Tango", "Charlie" },
                },

            };
            _projects = new List<Project> { project};

                        

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
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _isExiting = true;
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isExiting) {
                e.Cancel = true;
                MinimizeToTray();
            }
        }

        private void MinimizeToTray() {
            this.Hide();
            notifyIcon1.BalloonTipTitle = Util.AssemblyInfoHelper.Title;
            notifyIcon1.BalloonTipText = $"{Util.AssemblyInfoHelper.Title} is still running in the background. Right-click the tray icon to exit.";
            notifyIcon1.ShowBalloonTip(3000); // duration in milliseconds
        }
    }
}
