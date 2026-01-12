/**
 * NoteView.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class NoteView : UserControl
    {
        //public event EventHandler SaveButtonClicked;
        public event EventHandler KeyUp;
        private string lastSearchText = "";
        private int lastFindIndex = -1;
        private bool _suppressKey = false;

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
            if (e.Control && e.KeyCode == Keys.F) {
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }
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
            var result= dialog.ShowDialog(this.FindForm());
            if (result == DialogResult.OK) {
                FindText((dialog.Controls["txtSearch"] as TextBox).Text, false);
            }
        }

        private void txtNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                _suppressKey = true;
                ShowFindDialog(); // Ctrl+F
            }
            else if (e.KeyCode == Keys.F3)
            {
                FindText(lastSearchText, matchCase: false);
                e.Handled = true;
            }
            else if (!e.Shift && e.KeyCode == Keys.Tab)
            {
                txtNote.SelectedText = "\t";  // Inserts without resetting scroll
                e.SuppressKeyPress = true;    // Prevent focus change
                e.Handled = true;
            }
            else if (e.Shift && e.KeyCode == Keys.Tab)
            {
                int selStart = txtNote.SelectionStart;

                // Find start of the current line
                int lineIndex = txtNote.GetLineFromCharIndex(selStart);
                int lineStart = txtNote.GetFirstCharIndexFromLine(lineIndex);

                // Check if line starts with a tab
                if (txtNote.Text.Length > lineStart && txtNote.Text[lineStart] == '\t')
                {
                    // Remove the tab without resetting scroll
                    txtNote.SelectionStart = lineStart;
                    txtNote.SelectionLength = 1;
                    txtNote.SelectedText = "";

                    // Restore caret relative to previous position
                    txtNote.SelectionStart = selStart > lineStart ? selStart - 1 : lineStart;
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
            }

        }

        private void txtNote_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_suppressKey)
            {
                e.Handled = true;
                _suppressKey = false;
            }

        }
    }
}
