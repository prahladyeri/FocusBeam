/**
 * RefPicker.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Util;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    [DefaultEvent("SelectedIndexChanged")]
    public partial class RefPicker : UserControl
    {

        public ComboBox.ObjectCollection Items
        {
            get { return comboBox1.Items; }
        }


        public override string Text
        {
            get { return comboBox1.Text; }
            set { comboBox1.Text = value; }
        }

        public int SelectedIndex
        {
            get { return comboBox1.SelectedIndex; }
            set { comboBox1.SelectedIndex = value; }
        }

        public object SelectedItem
        {
            get { return comboBox1.SelectedItem; }
            set { comboBox1.SelectedItem = value; }
        }

        public event EventHandler SelectedIndexChanged;
        public event EventHandler AddButtonClicked;
        public event EventHandler EditButtonClicked;
        public event EventHandler DeleteButtonClicked;

        public RefPicker()
        {
            InitializeComponent();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            FormHelper.CreateTooltip(btnNew, "New Record");
            FormHelper.CreateTooltip(btnEdit, "Edit Record");
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditButtonClicked?.Invoke(this, e);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddButtonClicked?.Invoke(this, e);
        }
    }
}
