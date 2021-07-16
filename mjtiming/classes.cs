using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using RaceBeam;

namespace RaceBeam
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private RaceBeam.CSVData classData;
		string classDataFile = null;
		bool firstTime = true;
		void InitClasses()
		{
			classDataFile = configData.GetField("classDataFile", "Value");

			// Read in class data file
			classData = new CSVData();
			string err = classData.LoadData(classDataFile,',',"Class");
			if (err != "")
			{
				MessageBox.Show("Unable to load class data");
				return;
			}

			classDataGridView.Rows.Clear();
			List<string> columns = classData.GetHeaders();
			int i = 0;
			foreach (string col in columns)
			{
				if (classDataGridView.Columns.Contains(col) == true)
				{
					continue;
				}
				classDataGridView.Columns.Add(col, col);
				classDataGridView.Columns[i++].MinimumWidth = 50; //AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			}
			List<string> classNames = classData.getKeys();
			foreach (string className in classNames)
			{
				bool doesExist = false;
				int rows = classDataGridView.RowCount;
				for (int index = 1; index < rows; index++)
				{
					if (classDataGridView["Class",index].Value == null)
					{
						break;
					}
					if (classDataGridView["Class",index].Value.ToString() == className)
					{
						doesExist = true;
						break;
					}
				}
				if (doesExist == true)
				{
					continue;
				}
				var record = new string[columns.Count];
				i= 0;
				foreach (string col in columns)
				{
					record[i++] = classData.GetField(className, col);
				}
				classDataGridView.Rows.Add(record);
			}
			if (firstTime == true)
			{
				firstTime = false;
				try
				{
					classDataGridView.Columns["Class"].MinimumWidth = 100;
					classDataGridView.Columns["PAX"].MinimumWidth = 20;
					classDataGridView.Columns["Description"].MinimumWidth = 200;
					classDataGridView.Columns["Group"].MinimumWidth = 200;
					classDataGridView.Columns["Display Order"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
					classDataGridView.Columns["Display Order"].MinimumWidth = 10;
				}
				catch
				{
					// do nothing
				}
				classDataGridView.AllowUserToAddRows = false;
				classDataGridView.AllowUserToResizeColumns = true;
				classDataGridView.ReadOnly = false;
				classDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.ClassDataGridView_SortCompare);
				classDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.ClassDataGridView_UserDeletingRow);
				classDataGridView.CellValidating += new DataGridViewCellValidatingEventHandler(Classes_CellValidating);
			}
			
			classDataGridView.Refresh();
		}
		// ----------------------------------------------------------------------
		private void ClassDataGridView_UserDeletingRow( object sender, System.Windows.Forms.DataGridViewRowCancelEventArgs e)
		{
			// Handles user Deleting Row
			if (e.Row.IsNewRow == true)
				return;
			DataGridViewCell num = classDataGridView["Class", e.Row.Index];
			if (num.Value == null)
				return;
			DialogResult result = MessageBox.Show("Really delete class " + num.Value.ToString() + "?","MJ registration", MessageBoxButtons.YesNo);
			if (result == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		// ----------------------------------------------------------------------
		// Sort numerically if the column is numeric
		private void ClassDataGridView_SortCompare( object sender, DataGridViewSortCompareEventArgs e )
		{
			float Value1, Value2;
			try
			{
				if ( !float.TryParse( e.CellValue1.ToString(), out Value1 ) )
				{
					e.Handled = false;
					return;
				}
				if ( !float.TryParse( e.CellValue2.ToString(), out Value2 ) )
				{
					e.Handled = false;
					return;
				}
			}
			catch
			{
				e.Handled = false;
				return;
			}
			if ( Value1 == Value2 )
				e.SortResult = 0;
			else if ( Value1 < Value2 )
				e.SortResult = -1;
			else
				e.SortResult = 1;

			e.Handled = true;
		}
		// ----------------------------------------------------------------------
		
		// do not allow duplicate classes
		// Do not allow commas -- that's our separator
		// Verify that sort order and PAX are both proper numbers
		private void Classes_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			dataModified = true;  // may not be true, but could be true
			string headerText = classDataGridView.Columns[e.ColumnIndex].HeaderText;

			string data = e.FormattedValue.ToString();
			if (data.Contains(","))
			{
				MessageBox.Show("Commas are not allowed");
				e.Cancel = true;
				return;
			}
			if (headerText.Equals("Class") == true)
			{
				// Confirm that the value is unique
				string newClass = e.FormattedValue.ToString();
				for (int i = 0; i < classDataGridView.RowCount; i++)
				{
					if (i == e.RowIndex)
					{
						continue;
					}
					if (classDataGridView["Class",i].Value == null)
					{
						continue;
					}
					if (classDataGridView["Class",i].Value.ToString() == newClass)
					{
						MessageBox.Show("Class " + newClass + " is already being used");
						e.Cancel = true;
						return;
					}
				}
			}
			else if (headerText.Equals("PAX") == true)
			{
				// Make sure that the PAX value can be parsed as a number
				string newpax = e.FormattedValue.ToString();
                if (double.TryParse(newpax, out _) == false)
                {
                    MessageBox.Show("PAX value is not a valid number");
                    e.Cancel = true;
                    return;
                }
            }
			else if (headerText.Equals("Sort Order") == true)
			{
				// Make sure that the sort order is a valid number (floats are OK)
				string newsort = e.FormattedValue.ToString();
                if (double.TryParse(newsort, out _) == false)
                {
                    MessageBox.Show("Sort value is not a valid number");
                    e.Cancel = true;
                    return;
                }
            }
		}
		// ----------------------------------------------------------------------
		void AddClassButtonClick(object sender, EventArgs e)
		{
			classDataGridView.Rows.Add();
			// go to end of grid
			int index = classDataGridView.RowCount - 1;
			classDataGridView.FirstDisplayedScrollingRowIndex = index;
			classDataGridView.Refresh();

			// select the row and go into edit mode
			classDataGridView.CurrentCell = classDataGridView["Class",index];
			classDataGridView.CurrentCell.Value = "Unknown";
			classDataGridView["PAX", index].Value = "1.0";
			classDataGridView["Display Order", index].Value = "99.0";
			classDataGridView.BeginEdit(true);
			classDataGridView.Rows[index].Selected = true;
		}
		// ----------------------------------------------------------------------
		// Save data to driver file
		void SaveClassData()
		{
			string row;
			var displayOrder = new Dictionary<int, int>();
			// This path may have changed since we last looked
			classDataFile = configData.GetField("classDataFile", "Value");
			if (classDataFile == "")
			{
				MessageBox.Show("Class data file not defined in config file");
				System.Environment.Exit(0);
			}
			// We must ensure that "Class" is the first column saved.
			// Order of the remainder is unimportant (but leave as user arranged them)
			DataGridViewColumn numCol = classDataGridView.Columns["Class"];
			numCol.DisplayIndex = 0;
			DataGridViewColumnCollection cols = classDataGridView.Columns;
			displayOrder.Clear();
			for (int i = 0; i < cols.Count; i++)
			{
				displayOrder.Add(cols[i].DisplayIndex, cols[i].Index);
			}
			
			try
			{
				TextWriter tw = new StreamWriter(classDataFile);
				// First write out header row
				// Bit of work here -- need to order columns by displayIndex, not column index
				
				row = "";
				for (int i = 0; i < cols.Count; i++)
				{
					if (i == 0)
						row = cols[displayOrder[i]].HeaderText;
					else
						row += "," + cols[displayOrder[i]].HeaderText;
				}
				tw.WriteLine(row);
				
				
				for (int rowindex = 0; rowindex < classDataGridView.RowCount; rowindex++)
				{
					row = "";
					
					for (int colindex = 0; colindex < classDataGridView.ColumnCount; colindex++)
					{
						int dispIndex = displayOrder[colindex];
						if (dispIndex == 0)
						{
							if (classDataGridView[dispIndex,rowindex].Value == null)
							{
								row += "Unknown" + rowindex.ToString();
							}
							else
							{
								row += classDataGridView[dispIndex,rowindex].Value.ToString();
							}
						}
						else
						{
							if (classDataGridView[dispIndex,rowindex].Value == null)
							{
								row += ",";
							}
							else
							{
								row += "," + classDataGridView[dispIndex,rowindex].Value.ToString();
							}
						}
					}
					tw.WriteLine(row);
				}
				tw.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			// Make a copy of the driverData file if we haven't already
			string today = DateTime.Now.ToString("yyyy_MM_dd");
			string eventDriverDataFile = configData.GetField("eventDataFolder", "Value") + "\\" + today + "_driverData.csv";
			try
			{
				File.Copy(configData.GetField("driverDataFile", "Value"), eventDriverDataFile, true);
			}
			catch
			{
				// not interested in any errors
			}
		}
	}
}
