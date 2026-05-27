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
		/// <returns>True if the first element is considered better than the second; otherwise, false.</returns>
		private bool compare(string[] theList, int v1, int v2)
		{
			++comparisons;
#if TESTING
			return compareNoPrompt(theList, v1, v2);
#else
			return compareWithPrompt(theList, v1, v2);
#endif
		}

		/// <summary>
		/// Compares two elements in the list using a linear approach.
		/// </summary>
		/// <param name="theList">The list containing the elements to compare.</param>
		/// <param name="v1">The index of the first element.</param>
		/// <param name="v2">The index of the second element.</param>
		/// <returns>True if the first element is considered better than the second; otherwise, false.</returns>
        private bool compareLinear(string[] theList, int v1, int v2)
		{
			return compareWithMemory(theList, v1, v2);
		}

		/// <summary>
		/// Compares two elements in the list using a binary search approach.
		/// </summary>
		/// <param name="theList">The list containing the elements to compare.</param>
		/// <param name="v1">The index of the first element.</param>
		/// <param name="v2">The index of the second element.</param>
		/// <returns>True if the first element is considered better than the second; otherwise, false.</returns>
		private bool compareBinary(string[] theList, int v1, int v2)
        {
			return compareWithMemory(theList, v1, v2);
		}

		/// <summary>
		/// Compares two elements in the list by prompting the user for input.
		/// </summary>
		/// <param name="theList">The list containing the elements to compare.</param>
		/// <param name="start">The index of the first element.</param>
		/// <param name="end">The index of the second element.</param>
		/// <returns>True if the first element is considered better than the second; otherwise, false.</returns>
		private bool compareWithPrompt(string[] theList, int start, int end)
		{
			return DialogResult.Yes == MessageBox.Show(string.Format("{0} is better than {1}", theList[start], theList[end]), "Which do you prefer?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
		}

        /// <summary>
        /// Compares two elements in the list alphabetically without prompting the user.
        /// </summary>
        /// <param name="theList">The list containing the elements to compare.</param>
        /// <param name="v1">The index of the first element.</param>
        /// <param name="v2">The index of the second element.</param>
        /// <returns>True if the first element is considered better than the second; otherwise, false.</returns>
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

		/// <summary>
		/// Sorts a range of elements in the list using a linear merge sort approach.
		/// </summary>
		/// <param name="theList">The list containing the elements to sort.</param>
		/// <param name="start">The starting index of the range to sort.</param>
		/// <param name="end">The ending index of the range to sort (exclusive).</param>
		private void mergeSortLinear(string[] theList, int start, int end)
        {
			if (end - start < 2)
				return;

			int middle = (start + end) / 2;

			mergeSortLinear(theList, start, middle);
			mergeSortLinear(theList, middle, end);
			mergeLinear(theList, start, middle, end);
		}
#endregion

		#region Merge Sort BinarySearch

		private System.Collections.Generic.Dictionary<string, bool> theMemory;

		/// <summary>
		/// Compares two elements in the list using a memory cache to avoid redundant comparisons.
		/// </summary>
		/// <param name="theList">The list containing the elements to compare.</param>
		/// <param name="v1">The index of the first element.</param>
		/// <param name="v2">The index of the second element.</param>
		/// <returns>True if the first element is considered better than the second; otherwise, false.</returns>
		private bool compareWithMemory(string[] theList, int v1, int v2)
        {
			string s1 = theList[v1];
			string s2 = theList[v2];
			int theOrder = s1.CompareTo(s2);
			string key = null;
			bool isReversed = (theOrder >= 0);

			if (isReversed)
				key = string.Format("{0}...{1}", s2, s1);
			else
				key = string.Format("{0}...{1}", s1, s2);
			if (theMemory.ContainsKey(key))
			{
				if (isReversed)
					return !theMemory[key];
				else
					return theMemory[key];
			}

			bool retVal = compare(theList, v1, v2);

			if (isReversed)
				theMemory[key] = !retVal;
			else
				theMemory[key] = retVal;

			return retVal;
		}

        /// <summary>
        /// Determines the position where the element at index `source` should be inserted within the specified range using a binary search approach.
        /// </summary>
        /// <param name="theList">The list containing the elements to search.</param>
        /// <param name="start">The starting index of the range to search.</param>
        /// <param name="end">The ending index of the range to search (inclusive).</param>
        /// <param name="source">The source index of the element to move.</param>
        /// <returns>The destination index where the element should be inserted.</returns>
        private int determineWhereToMove(string[] theList, int start, int end, int source)
        {
            if (start == source)
                return start;

			if (start >= end)
			{
                if (compareBinary(theList, start, source))
					return start + 1;
				else
					return start;
			}
			else
			{
				int middle = (start + end) / 2;
                if (compareBinary(theList, middle, source))
				{
					return determineWhereToMove(theList, middle + 1, end, source);
				}
				else
				{
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


			// I think I want to see which list is longest, then take the first element of the OTHER list, and do a binary search to find where it belongs.
			// Once I move that position, then the remaining longest list goes next

			while (v1 < v2 && v2 < end)
			{
				int destination = determineWhereToMove(theList, v1, v2 - 1, v2);

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
					// v1 = destination + 1; // Was tried but found to be incorrect.
					v1 = destination;
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
			if (end - start < 2)
				return;

			int middle = (start + end) / 2;

			mergeSortBinary(theList, start, middle);
			mergeSortBinary(theList, middle, end);
			mergeBinary(theList, start, middle, end);
		}
        #endregion

    }
}
