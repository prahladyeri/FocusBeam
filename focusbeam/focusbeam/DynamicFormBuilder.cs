/**
 * DynamicFormBuilder.cs - The main form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace focusbeam
{

    internal partial class DynamicFormBuilder : Form
    {
        private List<Field> fieldsToGenerate;
        private int yOffset = 20;
        private const int controlHeight = 25;
        private const int labelWidth = 100;
        private const int controlWidth = 200;
        private const int padding = 10;


        public DynamicFormBuilder(List<Field> fieldsToGenerate)
        {
            InitializeComponent();
            this.AutoScroll = true; // Enable scrolling if many fields
            this.fieldsToGenerate = fieldsToGenerate;
            GenerateFormControls();
        }

        private void GenerateFormControls()
        {
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // 120 pixels wide
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Take 100% of remaining space
            tableLayoutPanel1.ColumnCount = 2;
            
            foreach (Field field in fieldsToGenerate) 
            {
                Label label = new Label
                {
                    Text = field.Name + ":",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight
                };
                Control control = null;
                if (field.CustomControl != null)
                {
                    control = field.CustomControl;
                }
                else if (field.ControlType == FieldControlType.TextBox)
                {
                    control = new TextBox
                    {
                        Text = field.Value.ToString(),
                    };
                }
                else if (field.ControlType== FieldControlType.ComboBox) 
                {
                    var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    foreach (string name in field.Items)
                    {
                        combo.Items.Add(name);
                    }
                    combo.Text = field.Value.ToString();
                    control = combo;
                }
                else if (field.ControlType == FieldControlType.NumericUpDown)
                {
                    var numericUpDown = new NumericUpDown
                    {
                        Minimum = decimal.MinValue, // Set appropriate min/max for your data
                        Maximum = decimal.MaxValue,
                        Increment = 1m // Default increment
                    };
                    numericUpDown.Value = Convert.ToDecimal(field.Value, CultureInfo.InvariantCulture);
                    if (field.Value is float || field.Value is double || field.Value is decimal)
                    {
                        numericUpDown.DecimalPlaces = 2;
                    }
                    control = numericUpDown;
                }
                else if (field.Value != null) //Fallback to auto-deduction 
                {
                    if (field.Value.GetType() == typeof(string))
                    {
                        control = new TextBox
                        {
                            Text = field.Value.ToString(),
                        };
                    }
                    else if (field.Value != null && field.Value.GetType().IsEnum)
                    {
                        var enumType = field.Value.GetType();
                        var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                        foreach (string name in Enum.GetNames(enumType))
                        {
                            combo.Items.Add(name);
                        }
                        combo.Text = field.Value.ToString();
                        control = combo;
                    }
                    else if (Util.Core.IsNumericType(field.Value.GetType())) // Helper method to check for numeric types
                    {
                        var numericUpDown = new NumericUpDown
                        {
                            Minimum = decimal.MinValue, // Set appropriate min/max for your data
                            Maximum = decimal.MaxValue,
                            Increment = 1m // Default increment
                        };
                        numericUpDown.Value = Convert.ToDecimal(field.Value, CultureInfo.InvariantCulture);
                        if (field.Value is float || field.Value is double || field.Value is decimal)
                        {
                            numericUpDown.DecimalPlaces = 2;
                        }
                        control = numericUpDown;
                    }
                    else
                    {
                        // Fallback for unhandled types
                        control = new Label
                        {
                            Text = $"[Unhandled: {field.Value.ToString()}]", // Just display as a label if unhandled
                            BorderStyle = BorderStyle.FixedSingle, // Make it look distinct
                            BackColor = System.Drawing.SystemColors.ControlLight // A light background
                        };
                    }
                }
                if (field.Properties.Count > 0)
                {
                    Type theType = control.GetType();
                    foreach (var key in field.Properties.Keys)
                    {
                        PropertyInfo property = theType.GetProperty(key);
                        object value = field.Properties[key];
                        if (property != null && property.CanWrite)
                        {
                            // Handle type conversion if necessary (e.g., value is string, property is int)
                            try
                            {
                                // For properties like 'Location' (Point struct), you'd need to cast 'value' accordingly
                                // or create a new Point based on the value type.
                                // For simple types, direct conversion might work.
                                object convertedValue = Convert.ChangeType(value, property.PropertyType);
                                property.SetValue(control, convertedValue, null);
                            }
                            catch { }
                        }
                    }
                }
                control.Dock = DockStyle.Fill;
                control.Name = "ctrl_" + field.Name;
                int newRowIndex = tableLayoutPanel1.RowCount;
                tableLayoutPanel1.RowCount += 1;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel1.Controls.Add(label, 0, newRowIndex);     // Column 0
                tableLayoutPanel1.Controls.Add(control, 1, newRowIndex);   // Column 1
            }
        }


        private void DynamicFormBuilder_Load(object sender, EventArgs e)
        {
            
        }

        private void DynamicFormBuilder_Shown(object sender, EventArgs e)
        {
            //base.OnShown(e);
            //this.SelectNextControl(this, true, true, true, true);
            Util.FormHelper.SetFocusToFirstEditableControl(this.tableLayoutPanel1);
        }
    }

    internal enum FieldControlType
    {
        Auto, // New value to indicate auto-deduction
        TextBox,
        NumericUpDown,
        ComboBox,
        CheckBox,
        DateTimePicker,
        Custom // For when CustomControl is explicitly set
    }

    internal class Field
    {
        internal string Name { get; set; }
        internal object Value { get; set; }
        internal Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        internal FieldControlType ControlType { get; set; }
        internal bool Required { get; set; } = false;
        internal Control CustomControl { get; set; }
        internal string[] Items { get; set; } = new string[] { }; // for combo box
    }

}
