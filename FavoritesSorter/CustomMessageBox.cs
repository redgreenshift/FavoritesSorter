using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FavoritesSorter
{

    public enum CustomDialogResult
    {
        None = 0,
        Button1,
        Button2,
        Button3,
        Button4,
        Closed
    }

    /// <summary>
    /// A custom message dialog form for displaying messages with 1-4 action buttons. Implements keyboard shortcuts:
    /// Escape closes the dialog, Enter activates the primary button.
    /// </summary>
    public class CustomMessageBox : Form
    {
        private Label messageLabel;
        private FlowLayoutPanel buttonPanel;
        private CustomDialogResult result = CustomDialogResult.None;

        public CustomDialogResult Result => result;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMessageBox"/> class.
        /// </summary>
        /// <param name="message">The message to display in the dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="buttonLabels">An array of button labels (1-4) from left to right.</param>
        /// <exception cref="ArgumentException">Thrown if the number of button labels is not between 1 and 4.</exception>
        public CustomMessageBox(string message, string title, params string[] buttonLabels)
        {
            if (buttonLabels == null || buttonLabels.Length == 0 || buttonLabels.Length > 4)
                throw new ArgumentException("Provide 1 to 4 button labels.", nameof(buttonLabels));

            Text = title ?? "";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Padding = new Padding(12);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MinimumSize = new Size(280, 0);
            KeyPreview = true;

            // Message label
            messageLabel = new Label
            {
                AutoSize = true,
                Text = message ?? "",
                MaximumSize = new Size(420, 0),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            // Button panel
            buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                Margin = new Padding(0, 12, 0, 0)
            };

            // Container that participates in PreferredSize calculation
            var container = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0),
            };
            container.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            container.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            container.Controls.Add(messageLabel, 0, 0);
            container.Controls.Add(buttonPanel, 0, 1);

            Controls.Add(container);

            // Create buttons and add to buttonPanel
            // Create buttons, insert from right to left, so that the first button label ends up on the leftmost side of the dialog.
            for (int i = buttonLabels.Length - 1; i >= 0; --i)
            {
                var idx = i; // capture
                var btn = new Button
                {
                    Text = buttonLabels[i],
                    AutoSize = true,
                    Margin = new Padding(6, 0, 0, 0),
                    Tag = idx + 1,
                    MinimumSize = new Size(75, 0)
                };
                btn.Click += (s, e) =>
                {
                    result = (CustomDialogResult)btn.Tag;
                    DialogResult = DialogResult.OK;
                    Close();
                };
                buttonPanel.Controls.Add(btn);
            }

            // Ensure layout is up-to-date, then set a stable size (respecting padding)
            SuspendLayout();
            container.PerformLayout();
            buttonPanel.PerformLayout();
            messageLabel.PerformLayout();
            ResumeLayout(false);

            // Compute preferred size from container (TableLayoutPanel reports PreferredSize correctly)
            var desired = container.PreferredSize;
            desired.Width += Padding.Left + Padding.Right;
            desired.Height += Padding.Top + Padding.Bottom;
            desired.Width = Math.Max(desired.Width, MinimumSize.Width);

            // Apply as ClientSize so borders are accounted for
            this.ClientSize = new Size(desired.Width, desired.Height);

            // Closing / keyboard handling
            FormClosing += (s, e) =>
            {
                if (result == CustomDialogResult.None)
                    result = CustomDialogResult.Closed;
            };

            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    result = CustomDialogResult.Closed;
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
                else if (e.KeyCode == Keys.Enter && buttonPanel.Controls.Count > 0)
                {
                    // activate the right-most (primary) button
                    var first = buttonPanel.Controls[buttonPanel.Controls.Count - 1] as Button;
                    first?.PerformClick();
                }
            };
        }

        /// <summary>
        /// Static helper
        /// Displays a custom dialog with the specified message, title, and buttons.
        /// </summary>
        /// <param name="owner">The owner window of the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="buttonLabels">An array of button labels (1-4) from left to right.</param>
        /// <returns>The result of the dialog.</returns>
        public static CustomDialogResult ShowDialog(IWin32Window owner, string message, string title, params string[] buttonLabels)
        {
            using (var dlg = new CustomMessageBox(message, title, buttonLabels))
            {
                dlg.ShowDialog(owner);
                return dlg.Result;
            }
        }
    }
}
