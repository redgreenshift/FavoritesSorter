//Define this symbol to use compareNoPrompt for testing without user interaction.
//Undefine it to use compareWithPrompt which prompts the user for comparisons.
//#define TESTING

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace FavoritesSorter
{
	public partial class Form1 : Form
	{
		private string[] theListToSort = null;

        // Initialize the form and create the memory dictionary used to cache comparison results.
        public Form1()
		{
			InitializeComponent();
			theMemory = new Dictionary<string, bool>();
		}


		private int comparisons = 0;

        /// <summary>
        /// Handles the Click event of the buttonSort control.
        /// Parses the textbox input into a list, runs the sort, and writes the sorted result back.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void buttonSort_Click(object sender, EventArgs e)
		{
			char[] delimiters = {'\r', '\n'};
			theListToSort = textBox1.Text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

			theMemory.Clear();
			comparisons = 0;

			try
			{
				//mergeSortBinary(theListToSort, 0, theListToSort.Length);
				mergeSortLinear(theListToSort, 0, theListToSort.Length);
            }
            catch (Exception ex)
			{
				MessageBox.Show("Sorting was aborted: " + ex.Message, "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
            }
		}

        #region Merge Sort Linear
        /// <summary>
        /// Compares two elements in the list.
        /// </summary>
        /// <param name="theList">The list containing the elements to compare.</param>
        /// <param name="v1">The index of the first element.</param>
        /// <param name="v2">The index of the second element.</param>
        /// <returns>True if theList[v1] sorts before theList[v2]; otherwise, false.</returns>
        private bool compare(string[] theList, int v1, int v2)
		{
			++comparisons;
#if TESTING
            // compareNoPrompt performs a simple alphabetical comparison without prompting the user,
            // which is useful for testing and benchmarking the sorting algorithm without user interaction.
			return compareNoPrompt(theList, v1, v2);
#else
            // compareWithPrompt delegates to compareWithMemory, which records and looks up answers in theMemory dictionary.
            // This avoids redundant pairwise comparisons by using the recorded answers.
            return compareWithPrompt(theList, v1, v2);
#endif
		}

        /// <summary>
        /// Compares two elements in the list using a linear approach,
        /// theList[v1] vs theList[v2] using the cached memory.
        /// </summary>
        /// <param name="theList">The list containing the elements to compare.</param>
        /// <param name="v1">The index of the first element.</param>
        /// <param name="v2">The index of the second element.</param>
        /// <returns>True if theList[v1] sorts before theList[v2]; otherwise, false.</returns>
        private bool compareLinear(string[] theList, int v1, int v2)
		{
			return compareWithMemory(theList, v1, v2);
		}

        /// <summary>
        /// Compares two elements in the list using a binary search approach.
        /// the comparison result is cached for future lookups
        /// </summary>
        /// <param name="theList">The list containing the elements to compare.</param>
        /// <param name="v1">The index of the first element.</param>
        /// <param name="v2">The index of the second element.</param>
        /// <returns>True if theList[v1] sorts before theList[v2]; otherwise, false.</returns>
        private bool compareBinary(string[] theList, int v1, int v2)
        {
			return compareWithMemory(theList, v1, v2);
		}

        /// <summary>
        /// Compares two elements in the list by prompting the user for input, and records the result in memory for future comparisons.
        /// </summary>
        /// <param name="theList">The list containing the elements to compare.</param>
        /// <param name="v1">The index of the first element.</param>
        /// <param name="v2">The index of the second element.</param>
        /// <returns>True if theList[v1] sorts before theList[v2]; otherwise, false.</returns>
        /// <exception cref="OperationCanceledException">Thrown if the user cancels the prompt, indicating that the sort operation should be aborted.</exception>
        private bool compareWithPrompt(string[] theList, int v1, int v2)
		{
			var res = CustomDialogForm.ShowDialog(this, "Which do you prefer?", "Pick One", theList[v1], theList[v2]);
			if (res == CustomDialogResult.Button1)
			{
				return true; /* (first button clicked) */
			}
			else if (res == CustomDialogResult.Button2)
			{
				return false; /* (second button clicked) */
			}
			else
			{
				throw new OperationCanceledException("User aborted the sort.");
			}
		}

        /// <summary>
        /// Compares two elements in the list alphabetically without prompting the user.
        /// </summary>
        /// <param name="theList">The list containing the elements to compare.</param>
        /// <param name="v1">The index of the first element.</param>
        /// <param name="v2">The index of the second element.</param>
        /// <returns>True if theList[v1] sorts before theList[v2]; otherwise, false.</returns>
        private bool compareNoPrompt(string[] theList, int v1, int v2)
        {
			string s1 = theList[v1];
			string s2 = theList[v2];
			return s1.CompareTo(s2) < 0;
		}

		/// <summary>
		/// Merges two sorted sublists into a single sorted list using a linear approach.
		/// </summary>
		/// <param name="theList">The list containing the elements to merge.</param>
		/// <param name="start">The starting index of the first sublist.</param>
		/// <param name="middle">The ending index of the first sublist and starting index of the second sublist.</param>
		/// <param name="end">The ending index of the second sublist.</param>
		private void mergeLinear(string[] theList, int start, int middle, int end)
		{
			int v1, v2;
			v1 = start;
			v2 = middle;

            // The while loop compares elements from the first partition against the element at v2.
            // v1 advances through the left partition, v2 through the right.
            // If v1 catches up to v2, all elements in the first partition are confirmed
            // to be smaller than v2, so the element at v2 is in its correct final position.
            // The second condition (v2 < end) exits if v2 has been placed after all elements.
            while (v1 < v2 && v2 < end)
			{
				if (compareLinear(theList, v1, v2))
				{
					++v1;
				}
				else
				{
					string temp = theList[v2];
					// now move all the values to the right one
					for (int t = v2; t > v1; --t)
					{
						theList[t] = theList[t - 1];
					}
					theList[v1] = temp;
					++v2;
				}
			}
		}

		// sort the elements from start to end (exclusive)

		/// <summary>
		/// Sorts a range of elements in the list using a linear merge sort approach.
		/// </summary>
		/// <param name="theList">The list containing the elements to sort.</param>
		/// <param name="start">The starting index of the range to sort (inclusive).</param>
		/// <param name="end">The ending index of the range to sort (exclusive).</param>
		private void mergeSortLinear(string[] theList, int start, int end)
        {
            // Recursively divide the list in half until base case is reached, then merge the halves back together.
            if (end - start < 2)
				return;

			int middle = (start + end) / 2;

			mergeSortLinear(theList, start, middle);
			mergeSortLinear(theList, middle /* + 1 wrong????*/, end);
			mergeLinear(theList, start, middle, end);
		}
        #endregion // Merge Sort Linear

        private System.Collections.Generic.Dictionary<string, bool> theMemory;

        /// <summary>
        /// Compares two elements in the list using a memory cache to avoid redundant comparisons.
        /// </summary>
        /// <param name="theList">The list containing the elements to compare.</param>
        /// <param name="v1">The index of the first element.</param>
        /// <param name="v2">The index of the second element.</param>
        /// <returns>True if theList[v1] sorts before theList[v2]; otherwise, false.</returns>
        private bool compareWithMemory(string[] theList, int v1, int v2)
        {
			string s1 = theList[v1];
			string s2 = theList[v2];
			int theOrder = s1.CompareTo(s2);
			string key = null;
			// isReversed = true when theOrder >= 0 (s1 == s2 or s1 > s2).
			// Using >= instead of > handles the tie case correctly -- equal strings are treated as "reversed" so binary search works consistently.
			bool isReversed = (theOrder >= 0);

			if (isReversed)
				key = string.Format("{0}...{1}", s2, s1);
			else
				key = string.Format("{0}...{1}", s1, s2);
			// Key is built in sorted order (smaller...larger) regardless of which string was first.
			// This ensures consistent lookups regardless of comparison direction.
			if (theMemory.ContainsKey(key))
			{
                // Previously computed the answer; return from memory.
                if (isReversed)
					return !theMemory[key];
				else
					return theMemory[key];
			}

            // Record the answer so we don't prompt the user a second time for this pair of options (regardless of the order they are compared).
            bool retVal = compare(theList, v1, v2);

            if (isReversed)
				theMemory[key] = !retVal;
			else
				theMemory[key] = retVal;

			return retVal;
		}

        #region Merge Sort BinarySearch

        /// <summary>
        /// Determines the position where the element at index <b>source</b> should be inserted using a binary search within the range [start, end].
        /// </summary>
        /// <param name="theList">The list containing the elements to search.</param>
        /// <param name="start">The starting index of the range to search.</param>
        /// <param name="end">The ending index of the range to search (inclusive).</param>
        /// <param name="source">The source index of the element to move.</param>
        /// <returns>The <b>destination</b> index where the element should be inserted.</returns>
        private int determineWhereToMove(string[] theList, int start, int end, int source)
        {
            if (start == source)
                // source belongs at position 'start', which is the first
                // position not yet confirmed to be smaller than source.
                return start;

            if (start >= end)
			{
                // When start == end, we've checked one position. The element at 'start' is our insertion point.
                // Return start + 1 so the caller can place 'source' after this position.
                if (compareBinary(theList, start, source))
					return start + 1;
				else
					return start;
			}
			else
			{
				// Binary search continues while start < end.
				// The partition boundary 'middle' splits the range into [start..middle-1] and [middle..end].
				int middle = (start + end) / 2;
                // When compareBinary returns true, the element at 'middle' is smaller than 'source',
                // so we know 'source' belongs after 'middle' and we continue in the right half.
                if (compareBinary(theList, middle, source))
				{
					// middle + 1 is always >= start, so the next call always makes progress
					// (either reduces end or advances start).
					return determineWhereToMove(theList, middle + 1, end, source);
				}
				else
				{
                    // When compareBinary returns false, the element at 'middle' is larger than 'source',
                    // so 'source' belongs before 'middle'. If middle - 1 < start, we keep searching in [start..middle-1].
                    return determineWhereToMove(theList, start, middle - 1, source);
				}
			}
		}

        /// <summary>
        /// Merges two sorted subarrays within the specified range [start,end) using a binary search approach.
        /// </summary>
        /// <param name="theList">The list containing the elements to merge.</param>
        /// <param name="start">The starting index of the first subarray (inclusive).</param>
        /// <param name="middle">The starting index of the second subarray.</param>
        /// <param name="end">The ending index of the second subarray (exclusive).</param>
        private void mergeBinary(string[] theList, int start, int middle, int end)
		{
			int v1, v2;
			v1 = start;
			v2 = middle;

			while (v1 < v2 && v2 < end)
			{
                // Search for where v2's element should be inserted among elements from v1 to v2-1.
                // The range v1..v2-1 contains the "next" partition, and we find the correct insertion point.
                int destination = determineWhereToMove(theList, v1, v2 - 1, v2);

				// When destination == v2, it means v2's element belongs at position v2 itself
				// (it's greater than all elements from v1 to v2-1). We can stop merging because
				// all remaining elements in the partition are already confirmed to be smaller.
				if (destination == v2)
				{
					break; // DONE!
				}
				else
				{
					string temp = theList[v2];
					// now move all the values to the right one
					for (int t = v2; t > destination; --t)
					{
						theList[t] = theList[t - 1];
					}
					theList[destination] = temp;
					// v1 = destination is correct (not destination + 1) because after shifting,
					// the element at destination's old position becomes the new v1. We re-examine this
					// position because the shifted element might belong elsewhere relative to v2.
					// v1 = destination + 1; // Was tried but found to be incorrect.
					v1 = destination; // v1 points to where v2 was placed. We re-examine this position in case the shifted element also belongs elsewhere relative to v2.
					++v2;
				}
			}
		}

		// sort the elements from start to end (exclusive)

		/// <summary>
		/// Sorts the elements in the specified range [start,end) using a binary merge sort algorithm.
		/// </summary>
		/// <param name="theList">The list containing the elements to sort.</param>
		/// <param name="start">The starting index of the range to sort (inclusive).</param>
		/// <param name="end">The ending index of the range to sort (exclusive).</param>
		private void mergeSortBinary(string[] theList, int start, int end)
        {
            // Recursive divide-and-conquer sort. Divides the list in half at "middle", sorts both halves, then merges them using binary search to find where each element should go.
            if (end - start < 2)
				return;

			int middle = (start + end) / 2;

			mergeSortBinary(theList, start, middle);
			mergeSortBinary(theList, middle, end);
			mergeBinary(theList, start, middle, end);
		}
        #endregion // Merge Sort BinarySearch

    }


	public enum CustomDialogResult
    {
        None = 0,
        Button1,
        Button2,
        Button3,
        Button4,
        Closed
    }

    public class CustomDialogForm : Form
    {
        private Label messageLabel;
        private FlowLayoutPanel buttonPanel;
        private CustomDialogResult result = CustomDialogResult.None;

        public CustomDialogResult Result => result;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDialogForm"/> class.
        /// </summary>
        /// <param name="message">The message to display in the dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="buttonLabels">An array of button labels (1-4) from left to right.</param>
        /// <exception cref="ArgumentException">Thrown if the number of button labels is not between 1 and 4.</exception>
        public CustomDialogForm(string message, string title, params string[] buttonLabels)
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
            using (var dlg = new CustomDialogForm(message, title, buttonLabels))
            {
                dlg.ShowDialog(owner);
                return dlg.Result;
            }
        }
    }

}
