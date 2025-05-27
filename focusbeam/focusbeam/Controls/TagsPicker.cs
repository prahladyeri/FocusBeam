using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class TagsPicker : UserControl
    {
        internal List<string> Tags { get; set; }
        internal Color TagBackColor { get; set; } = Color.Cornsilk;

        public TagsPicker()
        {
            InitializeComponent();
        }

        private void RemoveLink_Click(object sender, EventArgs e)
        {
            if (sender is Label link && link.Tag is Control tagPanel)
            {
                flowLayoutPanel1.Controls.Remove(tagPanel);
                tagPanel.Dispose();
                // Optional: update Tags array or raise an event
                Tags.Remove(link.Text);
                
            }
        }

        private void AddTag(string tag) {
            //tableLayoutPanel1.ColumnCount += 1;
            FlowLayoutPanel tagPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 0),
                Padding = new Padding(0, 0, 0, 0),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
            };

            Label tagLabel = new Label
            {
                Text = tag,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                Padding = new Padding(2),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = TagBackColor,
            };

            Label removeLink = new Label
            {
                Text = "❌",
                AutoSize = true,
                TabStop = false,
                ForeColor = Color.Red,
                Cursor = Cursors.Hand,
                Margin = new Padding(2,0,0,0),
                Padding = new Padding(0),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = TagBackColor,
                Tag = tagPanel // Use Tag to refer back to the panel
            };
            removeLink.Click += RemoveLink_Click;
            tagLabel.Dock = DockStyle.Left;
            removeLink.Dock = DockStyle.Right;
            tagPanel.Controls.Add(tagLabel);
            tagPanel.Controls.Add(removeLink);
            //tableLayoutPanel1.Controls.Add(tagPanel, tableLayoutPanel1.ColumnCount - 1, 0);
            flowLayoutPanel1.Controls.Add(tagPanel);
        }


        private void TagsPicker_Load(object sender, EventArgs e)
        {
            foreach (string tag in Tags) {
                AddTag(tag);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (txtTag.Text.Trim() == "")
            {
                MessageBox.Show("Tag can't be empty.");
                txtTag.Focus();
                return;
            }
            AddTag(txtTag.Text.Trim());
            txtTag.Text = "";
            txtTag.Focus();

        }
    }
}
