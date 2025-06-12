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
    public partial class MindMapView : UserControl
    {
        public MindMapView()
        {
            InitializeComponent();
        }

        private void MindMapView_Load(object sender, EventArgs e)
        {
            btnSave.Font = new Font(btnSave.Font, FontStyle.Bold);
        }
    }
}
