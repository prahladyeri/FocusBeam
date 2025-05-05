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
        private List<Project> projects = null;
        private Project currentProject = null;
        private Task currentTask = null;

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
            notifyIcon1.Text = Util.AssemblyInfoHelper.GetTitle();
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
                    Title = "Default Task",
                    Priority = PriorityLevel.High,
                    Status = StatusLevel.Pending,
                    Tags = new List<string>{ "Tango", "Charlie" },
                }
            };
            projects = new List<Project> { project};

            rpkProject.cmbMain.SelectedIndexChanged += CmbMain_SelectedIndexChanged;

            foreach (Project proj in projects) {
                this.rpkProject.cmbMain.Items.Add(proj.Title);
            }

            rpkProject.cmbMain.SelectedIndex = 0;

            
        }

        private void CmbMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            string title = rpkProject.cmbMain.SelectedItem.ToString();
            currentProject = projects.FirstOrDefault(p => p.Title == title);
            rpkTaskItem.cmbMain.Items.Clear();
            foreach (TaskItem task in currentProject.Tasks) {
                rpkTaskItem.cmbMain.Items.Add(task.Title);
            }
            rpkTaskItem.cmbMain.SelectedIndex = 0;
            // TODO: update the view
            lblStatus.Text = $"{title} loaded";
        }
    }
}
