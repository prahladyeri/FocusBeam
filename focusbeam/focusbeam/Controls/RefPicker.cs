/**
 * RefPicker.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Util;
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
    [DefaultEvent("SelectedIndexChanged")]
    public partial class RefPicker : UserControl
    {
        public ComboBox cmbMain {
            get { return this.comboBox1; }
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
