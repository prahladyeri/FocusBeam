/**
 * DynamicFormBuilder.cs - The main form
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace focusbeam
{

    internal partial class DynamicFormBuilder : Form
    {
        internal List<Field> FieldsToGenerate;
        private EditMode _editMode;

        internal event EventHandler SaveButtonClicked;

        internal class RecordValidatingEventArgs : CancelEventArgs
        {
            internal List<Field> Fields { get; }

            internal RecordValidatingEventArgs(List<Field> fields)
            {
                Fields = fields;
            }
        }
        internal event EventHandler<RecordValidatingEventArgs> RecordValidating;


        internal DynamicFormBuilder(List<Field> fieldsToGenerate, EditMode editMode)
        {
            _editMode = editMode;
            this.Text = $"{_editMode} Record";
            InitializeComponent();
            this.AutoScroll = true;
            this.FieldsToGenerate = fieldsToGenerate;
            GenerateFormControls();
        }

        internal Field FindField(string fieldName) {
            return FieldsToGenerate.Find(item => item.Name == fieldName);
        }

        internal Control FindControl(string fieldName) {
            return tableLayoutPanel1.Controls[$"ctrl_{fieldName}"];
        }

        private void GenerateFormControls()
        {
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // 120 pixels wide
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Take 100% of remaining space
            tableLayoutPanel1.ColumnCount = 2;

            foreach (Field field in FieldsToGenerate)
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
                else if (field.ControlType == FieldControlType.DateTimePicker) {
                    var dtp = new DateTimePicker
                    {
                        Value = (DateTime)field.Value,
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "yyyy-MM-dd",
                    };
                    control = dtp;
                }
                else if (field.ControlType == FieldControlType.TextBox)
                {
                    control = new TextBox
                    {
                        Text = field.Value.ToString(),
                    };
                }
                else if (field.ControlType == FieldControlType.MultilineTextBox)
                {
                    control = new TextBox
                    {
                        Text = field.Value.ToString(),
                        Multiline = true,
                        Height = 60,
                        ScrollBars = ScrollBars.Vertical
                    };
                }
                else if (field.ControlType == FieldControlType.ComboBox)
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
                    if (field.ValueType == typeof(string))
                    {
                        control = new TextBox
                        {
                            Text = field.Value.ToString(),
                        };
                    }
                    else if (field.Value != null && field.ValueType.IsEnum)
                    {
                        var enumType = field.ValueType;
                        var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                        foreach (string name in Enum.GetNames(enumType))
                        {
                            combo.Items.Add(name);
                        }
                        combo.Text = field.Value.ToString();
                        control = combo;
                    }
                    else if (Util.Helper.IsNumericType(field.ValueType)) // Helper method to check for numeric types
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (Field field in FieldsToGenerate) {
                Control control = tableLayoutPanel1.Controls[$"ctrl_{field.Name}"];
                switch (field.ControlType) {
                    case FieldControlType.Custom:
                        if (field.CustomControlValueProperty.Length > 0) {
                            PropertyInfo prop = control.GetType().GetProperty(field.CustomControlValueProperty,
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (prop != null && prop.CanRead) {
                                field.Value = prop.GetValue(control);
                            }
                        }
                        break;
                    case FieldControlType.TextBox:
                    case FieldControlType.MultilineTextBox:
                    case FieldControlType.ComboBox:
                        if (field.Required && string.IsNullOrWhiteSpace(control.Text)) {
                            MessageBox.Show("Value can't be empty", ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            control.Focus();
                            return;
                        }
                        field.Value = control.Text;
                        break;
                    case FieldControlType.Auto:
                        if (field.Required && control is TextBox tb && string.IsNullOrWhiteSpace(control.Text)) {
                            MessageBox.Show("Value can't be empty", ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            control.Focus();
                            return;
                        }
                        field.Value = control.Text;
                        break;
                    case FieldControlType.DateTimePicker:
                        DateTimePicker dtp= (DateTimePicker)control;
                        field.Value = dtp.Value;
                        break;
                    case FieldControlType.NumericUpDown:
                        field.Value = Convert.ChangeType(control.Text, field.ValueType);
                        break;
                }
                if (field.ControlType == FieldControlType.Auto &&
                    field.ValueType.IsEnum) {
                    field.Value = Enum.Parse(field.ValueType, control.Text);
                }
            }
            var args = new RecordValidatingEventArgs(FieldsToGenerate);
            RecordValidating?.Invoke(this, args);
            if (args.Cancel)
            {
                return; // validation failed
            }
            SaveButtonClicked?.Invoke(this, e);
            this.Close();
        }
    }

    internal enum FieldControlType
    {
        Auto, // New value to indicate auto-deduction
        TextBox,
        MultilineTextBox,
        NumericUpDown,
        ComboBox,
        CheckBox,
        DateTimePicker,
        Custom // For when CustomControl is explicitly set
    }

    internal class Field
    {
        private Type _valueType;

        internal string Name { get; set; }
        internal object Value { get; set; } = "";
        internal Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        internal FieldControlType ControlType { get; set; }
        internal bool Required { get; set; } = false;
        internal Control CustomControl { get; set; }
        /// <summary>
        /// The name of the property to fetch from the custom control using reflection.
        /// Set this to an empty string ("") to ignore the property and skip value extraction.
        /// Default is "Value".
        /// </summary>        
        internal String CustomControlValueProperty { get; set; } = "Value";
        internal string[] Items { get; set; } = new string[] { }; // for combo box
        internal Type ValueType
        {
            get
            {
                if (_valueType == null && Value != null)
                {
                    _valueType = Value.GetType();
                }
                return _valueType;
            }
            set => _valueType = value;
        }
    }

    internal enum EditMode
    {
        Add,
        Edit
    }
}