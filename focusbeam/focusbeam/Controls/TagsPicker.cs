using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace focusbeam.Controls
{
    public partial class TagsPicker : UserControl
    {
        public List<string> Value { get; set; } = new List<string>();
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The background color used for individual tag labels.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TagBackColor { get; set; } = Color.Cornsilk;


        public TagsPicker()
        {
            InitializeComponent();
        }

        private void RemoveLink_Click(object sender, EventArgs e)
        {
            if (sender is Label lbl && lbl.Tag is Control tagPanel)
            {
                flowLayoutPanel1.Controls.Remove(tagPanel);
                int idx = Value.IndexOf(tagPanel.Controls[0].Text);
                Value.RemoveAt(idx);
                tagPanel.Dispose();
            }
        }

        private void CreateTagLabel(string tag) {
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
            foreach (string tag in Value) {
                CreateTagLabel(tag);
            }
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newTag = txtTag.Text.Trim();
            if (string.IsNullOrEmpty(newTag))
            {
                MessageBox.Show("Tag can't be empty.");
                txtTag.Focus();
                return;
            }
            else if (Value.Contains(newTag))
            {
                MessageBox.Show("This tag already exists.");
                return;
            }
            Value.Add(newTag);
            CreateTagLabel(newTag);
            txtTag.Text = "";
            txtTag.Focus();
        }
    }
}
