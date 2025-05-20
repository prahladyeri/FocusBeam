/**
 * AddRecordForm.cs - The main form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam
{

    public partial class AddRecordForm : Form
    {
        public AddRecordForm()
        {
            InitializeComponent();
            InitializeTableLayoutPanelColumns();
        }

        private void InitializeTableLayoutPanelColumns()
        {
            // Clear existing styles if any, or ensure it's empty
            tableLayoutPanel1.ColumnStyles.Clear();

            // Column 0: Fixed width for Labels (e.g., 120 pixels)
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // 120 pixels wide

            // Column 1: Fills the remaining space
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Take 100% of remaining space

            // Ensure the TableLayoutPanel has 2 columns defined.
            // If you add more columns dynamically, adjust this.
            tableLayoutPanel1.ColumnCount = 2;
        }

        public Control AddField(string fieldName, object fieldValue) {
            Label label = new Label
            {
                Text = fieldName + ":",
                Dock = DockStyle.Fill,
                Width = 100,
                TextAlign = ContentAlignment.MiddleRight
            };
            //label.Font = new Font(label.Font, FontStyle.Bold);
            Control control = null ;
            if (fieldValue.GetType() == typeof(string))
            {
                control = new TextBox
                {
                    Text = fieldValue.ToString(),
                    Dock = DockStyle.Fill,
                };
            }
            else if (fieldValue != null && fieldValue.GetType().IsEnum)
            {
                var enumType = fieldValue.GetType();
                //var enumNames = Enum.GetNames(enumType);
                var combo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
                foreach (string name in Enum.GetNames(enumType))
                {
                    combo.Items.Add(name);
                    //Console.WriteLine($"{enumNames[i]} = {(int)enumValues.GetValue(i)}");
                }
                combo.Text = fieldValue.ToString();
                control = combo;
            }
            else if (Util.Core.IsNumericType(fieldValue.GetType())) // Helper method to check for numeric types
            {
                var numericUpDown = new NumericUpDown
                {
                    Dock = DockStyle.Fill,
                    Minimum = decimal.MinValue, // Set appropriate min/max for your data
                    Maximum = decimal.MaxValue,
                    Increment = 1m // Default increment
                };
                numericUpDown.Value = Convert.ToDecimal(fieldValue, CultureInfo.InvariantCulture);
                // Adjust DecimalPlaces if it's a floating-point number
                if (fieldValue is float || fieldValue is double || fieldValue is decimal)
                {
                    numericUpDown.DecimalPlaces = 2;
                    // This is a simple heuristic; you might need a more robust way
                    // to determine appropriate decimal places, e.g., from property attributes.
                    //string valueAsString = fieldValue.ToString();
                    //int decimalPointIndex = valueAsString.IndexOf(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                    //if (decimalPointIndex > -1)
                    //{
                    //    numericUpDown.DecimalPlaces = valueAsString.Length - 1 - decimalPointIndex;
                    //}
                    //else
                    //{
                    //    numericUpDown.DecimalPlaces = 0;
                    //}
                }
                control = numericUpDown;
            }
            else
            {
                // Fallback for unhandled types
                control = new Label
                {
                    Text = fieldValue.ToString(), // Just display as a label if unhandled
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.FixedSingle, // Make it look distinct
                    BackColor = System.Drawing.SystemColors.ControlLight // A light background
                };
            }
            // Add a new row to the table layout
            int newRowIndex = tableLayoutPanel1.RowCount;
            tableLayoutPanel1.RowCount += 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(label, 0, newRowIndex);     // Column 0
            tableLayoutPanel1.Controls.Add(control, 1, newRowIndex);   // Column 1
            return control;
        }

        private void AddRecordForm_Load(object sender, EventArgs e)
        {
            Util.FormHelper.SetFocusToFirstEditableControl(this.tableLayoutPanel1);
        }

        private void AddRecordForm_Shown(object sender, EventArgs e)
        {
            //base.OnShown(e);
            //this.SelectNextControl(this, true, true, true, true);
        }
    }
}
