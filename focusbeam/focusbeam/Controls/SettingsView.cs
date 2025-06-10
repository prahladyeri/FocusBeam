/**
 * SettingsView.cs - The main form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Util;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace focusbeam.Controls
{
    public partial class SettingsView : UserControl
    {
        private void LoadSettingsToGrid(DataGridView dgv, AppSettings settings)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Property",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Resizable = DataGridViewTriState.False,
                Width = 250
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Value",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Resizable = DataGridViewTriState.False,
                Width = 250
            });

            var props = typeof(AppSettings).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;

                DataGridViewCell valueCell;
                var rowIndex = dgv.Rows.Add();
                var row = dgv.Rows[rowIndex];
                row.Cells["Property"].Value = prop.Name;
                object value = prop.GetValue(settings);

                if (prop.PropertyType == typeof(bool))
                {
                    valueCell = new DataGridViewComboBoxCell
                    {
                        DataSource = new[] { "True", "False" },
                        Value = value.ToString()
                    };
                }
                else if (prop.PropertyType.IsEnum)
                {
                    valueCell = new DataGridViewComboBoxCell
                    {
                        DataSource = Enum.GetNames(prop.PropertyType),
                        Value = value.ToString()
                    };
                }
                else
                {
                    valueCell = new DataGridViewTextBoxCell
                    {
                        Value = value?.ToString()
                    };
                }
                row.Cells["Value"] = valueCell;
            }
        }

        private void SaveSettingsFromGrid(DataGridView dgv, AppSettings settings)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue; // Skip new/empty row

                var propName = row.Cells["Property"].Value?.ToString();
                var valueCell = row.Cells["Value"];

                if (string.IsNullOrEmpty(propName))
                    continue;

                var propInfo = typeof(AppSettings).GetProperty(propName);
                if (propInfo == null || !propInfo.CanWrite)
                    continue;

                try
                {
                    var targetType = propInfo.PropertyType;
                    var cellValue = valueCell.Value;

                    object convertedValue;

                    if (targetType == typeof(bool))
                    {
                        // Handles "True"/"False" strings or checkboxes
                        convertedValue = Convert.ToBoolean(cellValue);
                    }
                    else if (targetType.IsEnum)
                    {
                        convertedValue = Enum.Parse(targetType, cellValue.ToString());
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(cellValue, targetType);
                    }

                    propInfo.SetValue(settings, convertedValue);
                }
                catch (Exception ex)
                {
                    // Optional: log or report conversion error
                    MessageBox.Show($"Error saving setting '{propName}': {ex.Message}",Application.ProductName,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            AppSettings.Save(settings);
        }

        public SettingsView()
        {
            InitializeComponent();
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
            //              ControlStyles.AllPaintingInWmPaint |
            //              ControlStyles.UserPaint, true);
            //this.UpdateStyles();
            //typeof(DataGridView)
            //    .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            //    .SetValue(dataGridView1, true, null);


        }

        private void SettingsView_Load(object sender, EventArgs e)
        {
            if (!DesignMode) {
                Program.StyleGrid(dataGridView1);
                //dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AllowUserToResizeRows = false;
                LoadSettingsToGrid(this.dataGridView1, Program.Settings);
                dataGridView1.ColumnWidthChanged += (s, ev) =>
                {
                    btnSave.Invalidate();
                };
            }
            btnSave.Font = new Font(btnSave.Font, FontStyle.Bold);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettingsFromGrid(this.dataGridView1, Program.Settings);
            MessageBox.Show("Settings are saved.\nSome changes may require application restart to take effect."
                , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
