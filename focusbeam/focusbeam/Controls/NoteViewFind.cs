/**
 * NoteViewFind.cs - UX Class
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class NoteViewFind : Form
    {
        public NoteViewFind()
        {
            InitializeComponent();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void NoteViewFind_Load(object sender, EventArgs e)
        {
            //txtSearch.Focus();
            //txtSearch.SelectAll(); // optional: selects all text
        }
    }
}
