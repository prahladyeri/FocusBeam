/**
 * NoteView.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class NoteView : UserControl
    {
        //public event EventHandler SaveButtonClicked;
        public event EventHandler KeyUp;
        private string lastSearchText = "";
        private int lastFindIndex = -1;


        public override string Text
        {
            get => txtNote.Text;
            set => txtNote.Text = value;
        }

        public NoteView()
        {
            InitializeComponent();
        }

        public NoteView(string initialText) : this()
        {
            txtNote.Text = initialText;
        }

        //private void btnSave_Click(object sender, EventArgs e)
        //{
        //    SaveButtonClicked?.Invoke(this, e);
        //}




        private void NoteView_Load(object sender, EventArgs e)
        {
            //btnSave.Font = new System.Drawing.Font(btnSave.Font, System.Drawing.FontStyle.Bold);
            //txtNote.Focus();
            this.BeginInvoke((MethodInvoker)(() =>
            {
                txtNote.Focus();
                txtNote.SelectionStart = txtNote.Text.Length; // Move cursor to end
                txtNote.SelectionLength = 0;                  // No text selected
            }));
        }

        private void txtNote_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUp.Invoke(this, e);
        }


        private void FindText(string search, bool matchCase = false)
        {
            string text = txtNote.Text;
            string searchText = search;
            lastSearchText = searchText;

            if (!matchCase)
            {
                text = text.ToLower();
                searchText = search.ToLower();
            }

            int startIndex = txtNote.SelectionStart + txtNote.SelectionLength;

            int index = text.IndexOf(searchText, startIndex);
            if (index == -1)
            {
                // Optional: wrap around
                index = text.IndexOf(searchText, 0);
            }

            if (index != -1)
            {
                txtNote.Select(index, search.Length);
                txtNote.ScrollToCaret();
                txtNote.Focus();
                lastFindIndex = index;
            }
            else
            {
                MessageBox.Show("Text not found", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void ShowFindDialog() {
            NoteViewFind dialog = new NoteViewFind();
            var result= dialog.ShowDialog();
            if (result == DialogResult.OK) {
                FindText((dialog.Controls["txtSearch"] as TextBox).Text, false);
            }
        }

        private void txtNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                ShowFindDialog(); // Ctrl+F
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                // You could store the last searched text somewhere globally
                FindText(lastSearchText, matchCase: false);
                e.Handled = true;
            }
        }
    }
}
