/**
 * TimesheetView.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
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
    public partial class TimesheetView : UserControl
    {
        public DataGridView dgv { 
            get { return this.dgvTasks; }
        }
        public TimesheetView()
        {
            InitializeComponent();
        }
    }
}
