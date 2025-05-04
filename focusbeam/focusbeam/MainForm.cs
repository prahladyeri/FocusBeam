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
        List<Project> projects = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //DBAL.Init();
            //projects = DBAL.GetAllProjects();
            projects = new List<Project>();
            projects.Add(new Project {
                Id = 1,
                Title = "Default Project",
                Category = CategoryLevel.Work,
                Tags = "",
                StartDate = new DateTime(2025, 05, 04),
                EndDate = new DateTime(2025, 05, 31),
                Notes = ""
            });
        }
    }
}
