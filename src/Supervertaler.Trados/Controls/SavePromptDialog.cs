using System.Drawing;
using System.Windows.Forms;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// Simple dialog that asks for a prompt name when saving a generated prompt.
    /// </summary>
    internal class SavePromptDialog : Form
    {
        private TextBox _txtName;

        /// <summary>The prompt name entered by the user.</summary>
        public string PromptName => _txtName?.Text?.Trim() ?? "";

        public SavePromptDialog()
        {
            Text = "Save as Prompt";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 130);
            Font = new Font("Segoe UI", 9f);
            AutoScaleMode = AutoScaleMode.Dpi;

            var lbl = new Label
            {
                Text = "Prompt name:",
                Location = new Point(12, 16),
                AutoSize = true
            };
            Controls.Add(lbl);

            _txtName = new TextBox
            {
                Location = new Point(12, 38),
                Width = 356,
                Text = "Custom Translation Prompt"
            };
            _txtName.SelectAll();
            Controls.Add(_txtName);

            var btnOk = new Button
            {
                Text = "Save",
                DialogResult = DialogResult.OK,
                Location = new Point(212, 80),
                Width = 75
            };
            Controls.Add(btnOk);
            AcceptButton = btnOk;

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(293, 80),
                Width = 75
            };
            Controls.Add(btnCancel);
            CancelButton = btnCancel;
        }
    }
}
