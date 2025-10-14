/**
 * FormHelper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System.Linq;
using System.Windows.Forms;

namespace focusbeam.Helpers
{

    internal static class FormHelper
    {

        private static ToolTip sharedToolTip = new ToolTip
        {
            AutoPopDelay = 5000,
            InitialDelay = 200,
            ReshowDelay = 500,
            ShowAlways = true
        };

        internal static string RecordSaveMessage(object obj)
        {
            return $"{obj.GetType().Name} saved.";
        }


        internal static void CreateTooltip(Control ctrl, string text)
        {
            sharedToolTip.SetToolTip(ctrl, text);
        }

        internal static void SetFocusToFirstEditableControl(TableLayoutPanel tlp)
        {
            // Get all controls on the form and order them by TabIndex
            // Filtering for controls that are visible, enabled, and can receive focus.
            var editableControls = tlp.Controls.Cast<Control>()
                .Where(c => c.Visible && c.Enabled && c.CanSelect)
                .OrderBy(c => c.TabIndex) // Order by TabIndex for logical flow
                .ToList(); // Convert to list to iterate

            foreach (Control control in editableControls)
            {
                // Check common editable control types
                if (control is TextBox ||
                    control is ComboBox ||
                    control is NumericUpDown ||
                    control is DateTimePicker ||
                    control is MaskedTextBox)
                {
                    control.Select(); // Give focus to this control
                    return; // Exit after finding and focusing the first one
                }
                // If the control is a container (like GroupBox or Panel),
                // you might want to recursively check its children.
                // For simplicity, this example only checks top-level controls.
                // If you need recursive checking, it gets more complex.
            }

            // Fallback: If no specific editable control is found, try to focus the first selectable one.
            var firstSelectableControl = tlp.Controls.Cast<Control>()
                .Where(c => c.Visible && c.Enabled && c.CanSelect)
                .OrderBy(c => c.TabIndex)
                .FirstOrDefault();

            firstSelectableControl?.Select();
        }
    }
}
