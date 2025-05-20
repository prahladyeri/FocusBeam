/**
 * Program.cs - Main entry point and other utilities
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam
{
    static class Program
    {
        public static AppSettings Settings;

        public static void StyleGrid(DataGridView dgv) {
            dgv.RowHeadersVisible = false;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(
                dgv.Font.FontFamily,
                dgv.Font.Size,
                FontStyle.Bold
            );
        }

        [STAThread]
        static void Main()
        {
            Settings = AppSettings.Load();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
