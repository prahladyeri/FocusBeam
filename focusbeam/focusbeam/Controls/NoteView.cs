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
        public event EventHandler SaveButtonClicked;

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

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveButtonClicked?.Invoke(this, e);
        }

        private void NoteView_Load(object sender, EventArgs e)
        {
            btnSave.Font = new System.Drawing.Font(btnSave.Font, System.Drawing.FontStyle.Bold);
            //txtNote.Focus();
            this.BeginInvoke((MethodInvoker)(() =>
            {
                txtNote.Focus();
                txtNote.SelectionStart = txtNote.Text.Length; // Move cursor to end
                txtNote.SelectionLength = 0;                  // No text selected
            }));
        }
    }
}
