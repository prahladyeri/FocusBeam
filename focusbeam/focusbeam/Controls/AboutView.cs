using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void AboutView_Load(object sender, EventArgs e)
        {
            //tableLayoutPanel1.Dock = DockStyle.Fill;
            //tableLayoutPanel1.ColumnStyles[0].Width = 30;
            //tableLayoutPanel1.ColumnStyles[1].Width = 70;
            lblAppName.Text = Application.ProductName;
            lblVersion.Text = Util.AssemblyInfoHelper.GetVersion();
            lblDescription.Text = Util.AssemblyInfoHelper.GetDescription();
            lblFrameworkVersion.Text = Environment.Version.ToString();
            lblFrameworkVersion.Text = RuntimeEnvironment.GetSystemVersion();
            lblCompany.Text = Util.AssemblyInfoHelper.Company;

            foreach (Control ctrl in tableLayoutPanel1.Controls)
            {
                Font fnt = new Font(ctrl.Font.FontFamily, 15, FontStyle.Regular);
                if (ctrl.Name == "lblAppName")
                {
                    fnt = new Font("Arial Black", 21, FontStyle.Regular);
                }
                else if (ctrl.Name == "lblDescription")
                {
                    fnt = new Font(ctrl.Font.FontFamily, 18, FontStyle.Regular);
                }
                ctrl.Font = fnt;
            }

            var files = Directory.GetFiles("licenses");
            for (int i = 0; i < files.Length; i++)
            {
                var fname = files[i];
                tableLayoutPanel1.RowCount += 1;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                var parts = fname.Split('\\');
                Label lbl = new Label
                {
                    Text = parts[parts.Length - 1].Replace(".txt", ""),
                    TextAlign = ContentAlignment.MiddleRight,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    Dock = DockStyle.None,
                    Height = lblVersion.Height,
                    Font = new Font(this.Font.FontFamily, 15f, FontStyle.Bold),
                };
                LinkLabel lnk = new LinkLabel
                {
                    Text = "Read License",
                    TextAlign = ContentAlignment.MiddleLeft,
                    //Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    AutoSize = true,
                    Dock = DockStyle.None,
                    Height = lblVersion.Height,
                    Font = new Font(this.Font.FontFamily, 15f),
                };
                lnk.Click += (s, ev) => {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "notepad.exe",
                        Arguments = $"\"{fname}\"",
                        UseShellExecute = true
                    });
                };
                tableLayoutPanel1.Controls.Add(lbl, 0, tableLayoutPanel1.RowCount - 1);
                tableLayoutPanel1.Controls.Add(lnk, 1, tableLayoutPanel1.RowCount - 1);
            }
        }

        private void lnkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var url = lnkGithub.Text;
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true // Required for opening links in default browser
            });
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void lnkChangelog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var fname = "changelog.txt";
            Process.Start(new ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = $"\"{fname}\"",
                UseShellExecute = true
            });
        }
    }
}
