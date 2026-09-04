using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace FavoritesSorter
{
    public partial class Form1 : Form
    {
        private string[] theListToSort = null;
        private bool userAborted = false;

        // Initialize the form and create the memory dictionary used to cache comparison results.
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Displays a custom message box dialog to let the user choose between two specified options.
        /// </summary>
        /// <param name="v1">The first option presented to the user in the dialog.</param>
        /// <param name="v2">The second option presented to the user in the dialog.</param>
        /// <returns>-1 if the user selected the first button, or 1 if the user selected the second button.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the user cancels the selection process.</exception>
        int PromptTheUser(string v1, string v2)
        {
            var res = CustomMessageBox.ShowDialog(this, "Which do you prefer?", "Pick One", v1, v2);
            if (res == CustomDialogResult.Button1)
            {
                return -1; /* (first button clicked) */
            }
            else if (res == CustomDialogResult.Button2)
            {
                return 1; /* (second button clicked) */
            }
            else
            {
                userAborted = true;
                throw new OperationCanceledException("User aborted the sort.");
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonSort control.
        /// Parses the textbox input into a list, runs the sort, and writes the sorted result back.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void buttonSort_Click(object sender, EventArgs e)
        {
            char[] delimiters = { '\r', '\n' };
            theListToSort = textBox1.Text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            userAborted = false;

            try
            {
                InteractiveStringComparer comparer = new InteractiveStringComparer(PromptTheUser);
                Array.Sort(theListToSort, comparer);
            }
            catch (Exception ex)
            {
                string message;
                if (userAborted)
                    message = "User aborted the sort.";
                else
                    message = ex.Message;

                MessageBox.Show("Sorting was aborted: " + message, "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Thread.Sleep(100);

            textBox1.Text = string.Join(Environment.NewLine, theListToSort, 0, theListToSort.Length);
        }
    }
}
