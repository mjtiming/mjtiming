/*
Registration app
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Text.RegularExpressions;
using RaceBeam;

namespace RaceBeam
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		CSVData driverData = new CSVData();
		string driverPath = "";
		private readonly System.Collections.Generic.Dictionary<int, int> dispOrder = new Dictionary<int, int>();
		bool dataModified = false;
		int lastFindIndex = 0;
		string lastFindText = "";
		
		public void InitRegistration()
		{
			this.Closing += new System.ComponentModel.CancelEventHandler(RegForm_Closing);
			drivers.CellValidating += new DataGridViewCellValidatingEventHandler(Drivers_CellValidating);
			drivers.CellValidated += new DataGridViewCellEventHandler(Drivers_CellValidated);
			drivers.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.Drivers_SortCompare);
			drivers.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.Drivers_UserDeletingRow);
			LoadRegData();
			
			DataGridViewColumnCollection cols = drivers.Columns;
			for (int i = 0; i < cols.Count; i++)
			{
				cols[i].MinimumWidth = 40;
			}
			try
			{
				drivers.Columns["Car Model"].MinimumWidth = 150;
				drivers.Columns["Sponsor"].MinimumWidth = 20;
				drivers.Columns["Sponsor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				drivers.Columns["First Name"].MinimumWidth = 100;
				drivers.Columns["Last Name"].MinimumWidth = 100;
			}
			catch
			{
				// do nothing
			}
			drivers.AllowUserToResizeColumns = true;
		}
		// ----------------------------------------------------------------------
		// Save data to driver file
		void SaveRegData()
		{
			string row;
			
			// This path may have changed since we last looked
			driverPath = configData.GetField("driverDataFile", "Value");
			if (driverPath == "")
			{
				MessageBox.Show("Driver data file not defined in config file");
				System.Environment.Exit(0);
			}
			// We must ensure that "Number" is the first column saved.
			// Order of the remainder is unimportant (but leave as user arranged them)
			DataGridViewColumn numCol = drivers.Columns["Number"];
			numCol.DisplayIndex = 0;
			DataGridViewColumnCollection cols = drivers.Columns;
			dispOrder.Clear();
			for (int i = 0; i < cols.Count; i++)
			{
				dispOrder.Add(cols[i].DisplayIndex, cols[i].Index);
			}
			
			try
			{
				TextWriter tw = new StreamWriter(driverPath);
				
				
				// First write out header row
				// Bit of work here -- need to order columns by displayIndex, not column index
				
				row = "";
				for (int i = 0; i < cols.Count; i++)
				{
					if (i == 0)
						row = cols[dispOrder[i]].HeaderText;
					else
						row += "," + cols[dispOrder[i]].HeaderText;
				}
				tw.WriteLine(row);
				
				
				for (int rowindex = 0; rowindex < drivers.RowCount; rowindex++)
				{
					row = "";
					
					for (int colindex = 0; colindex < drivers.ColumnCount; colindex++)
					{
						int dispIndex = dispOrder[colindex];
						if (dispIndex == 0)
						{
							if (drivers[dispIndex,rowindex].Value == null)
							{
								row += "Unknown" + colindex.ToString();
							}
							else
							{
								row += drivers[dispIndex,rowindex].Value.ToString();
							}
						}
						else
						{
							if (drivers[dispIndex,rowindex].Value == null)
							{
								row += ",";
							}
							else
							{
								row += "," + drivers[dispIndex,rowindex].Value.ToString();
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
		// ----------------------------------------------------------------------
		public void LoadRegData()
		{
			driverPath = configData.GetField("driverDataFile", "Value");
			if (driverPath == "")
			{
				MessageBox.Show("Driver data file not defined in config file");
				System.Environment.Exit(0);
			}
			driverData = new CSVData();
			
			string err = driverData.LoadData(driverPath,',',"Number");
			if (err != "")
			{
				MessageBox.Show("Unable to load registration file: " + err);
				System.Environment.Exit(0);
			}
			drivers.Rows.Clear();
			drivers.Columns.Clear();
			List<string> columns = driverData.GetHeaders();
			int i = 0;
			foreach (string col in columns)
			{
				if (drivers.Columns.Contains(col) == true)
				{
					continue;
				}
				drivers.Columns.Add(col, col);
				drivers.Columns[i++].MinimumWidth = 40; //AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			}
			if (drivers.Columns.Contains("Barcode") == false)
			{
				drivers.Columns.Add("Barcode", "Barcode");
			}
			if (drivers.Columns.Contains("Team") == false)
			{
				drivers.Columns.Add("Team", "Team");
			}
			if (drivers.Columns.Contains("XGroup") == false)
			{
				drivers.Columns.Add("XGroup", "XGroup");
			}
			List<string> carNumbers = driverData.getKeys();
			foreach (string carNumber in carNumbers)
			{
				bool doesExist = false;
				int rows = drivers.RowCount;
				for (int index = 0; index < rows; index++)
				{
					if (drivers["Number",index].Value.ToString() == carNumber)
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
					record[i++] = driverData.GetField(carNumber, col);
				}
				drivers.Rows.Add(record);
			}
			drivers.Refresh();
		}
		// ----------------------------------------------------------------------
		// Find a driver that has the given barcode
		// Returns car number or -1 if not found
		public string FindBarcode(string barcode)
		{
			for (int index = 0; index < drivers.RowCount; index++)
			{
				if (drivers["Barcode",index].Value.ToString() == barcode)
				{
					return drivers["Number",index].Value.ToString();
				}
			}
			return null;
		}
		// ----------------------------------------------------------------------
		void FindButtonClick(object sender, EventArgs e)
		{
			bool found = false;
			int foundIndex = lastFindIndex + 1;
			if (lastFindText != findText.Text)
			{
				lastFindIndex = 0;
				lastFindText = findText.Text;
				foundIndex = 0;
			}
			if (lastFindIndex >= drivers.RowCount)
			{
				lastFindIndex = 0;
				foundIndex = 0;
			}
			if (foundIndex >= drivers.RowCount)
			{
				foundIndex = 0;
			}
			
			string findLower = lastFindText.ToLowerInvariant();
			while (found == false)
			{
				string s1 = drivers["First Name",foundIndex].Value.ToString().ToLowerInvariant();
				
				if (s1.Contains(findLower))
				{
					found = true;
					break;
				}
				s1 = drivers["Last Name",foundIndex].Value.ToString().ToLowerInvariant();
				if (s1.Contains(findLower))
				{
					found = true;
					break;
				}
				s1 = drivers["Number",foundIndex].Value.ToString().ToLowerInvariant();
				if (s1.Contains(findLower))
				{
					found = true;
					break;
				}
				foundIndex += 1;
				if (foundIndex >= drivers.RowCount)
				{
					foundIndex = 0;
				}
				if (foundIndex == lastFindIndex)
				{
					break;
				}
			}
			if (found == false)
			{
				lastFindIndex = 0;
				lastFindText = findText.Text;
				return;
			}
			lastFindIndex = foundIndex;
			lastFindText = findText.Text;
			drivers.ClearSelection();
			drivers.FirstDisplayedScrollingRowIndex = foundIndex;
			drivers.Refresh();
			// select the row
			drivers.Rows[foundIndex].Selected = true;
		}
		// ----------------------------------------------------------------------
		private void Drivers_UserDeletingRow( object sender, System.Windows.Forms.DataGridViewRowCancelEventArgs e)
		{
			// Handles user Deleting Row
			if (e.Row.IsNewRow == true)
				return;
			DataGridViewCell num = drivers["Number", e.Row.Index];
			if (num.Value == null)
				return;
			DialogResult result = MessageBox.Show("Really delete driver " + num.Value.ToString() + "?","MJ registration", MessageBoxButtons.YesNo);
			if (result == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		// ----------------------------------------------------------------------
		private void Drivers_SortCompare( object sender, DataGridViewSortCompareEventArgs e )
		{
			int intValue1, intValue2;
			try
			{
				string digits = Regex.Match(e.CellValue1.ToString(), @"\d+").Value;
				if ( !Int32.TryParse( digits, out intValue1 ) )
				{
					e.Handled = false;
					return;
				}
				digits = Regex.Match(e.CellValue2.ToString(), @"\d+").Value;
				if ( !Int32.TryParse(digits, out intValue2 ) )
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
			if ( intValue1 == intValue2 )
				e.SortResult = 0;
			else if ( intValue1 < intValue2 )
				e.SortResult = -1;
			else
				e.SortResult = 1;
			
			e.Handled = true;
		}
		// ----------------------------------------------------------------------
		// Find a free number over 300
		int FindNumber()
		{
			var used = new List<int>();
            var seed = configData.GetField("SeedCarNumber", "Value");
            if (int.TryParse(seed, out int firstFree) == false)
			{
				firstFree = 300;
			}
			for (int i = 0; i < drivers.RowCount; i++)
			{
				if (drivers["Number",i].Value == null)
				{
					continue;
				}
				string sval = drivers["Number",i].Value.ToString();
                if (int.TryParse(sval, out int intval) == true)
                {
                    used.Add(intval);
                }
            }
			used.Sort();
			for (int i = 0; i < used.Count; i++)
			{
				if (used[i] == firstFree)
				{
					firstFree += 1;
				}
			}
			return firstFree;
		}
		// ----------------------------------------------------------------------
		// Go to first empty row, fill in driver number with first free # > 300, and go into edit mode
		void GotoLastRegRow()
		{
			
			
			// go to end of grid
			int index = drivers.RowCount - 1;
			drivers.FirstDisplayedScrollingRowIndex = index;
			drivers.Refresh();
			
			// select the row and go into edit mode
			drivers.CurrentCell = drivers["Number",index];
			drivers.CurrentCell.Value = FindNumber().ToString();
			drivers["First Name", index].Value = "Joe";
			drivers["Last Name", index].Value = "Driver";
			drivers["Car Model", index].Value = "Unknown";
			drivers["Car Color", index].Value = "Unknown";
			drivers["Registered", index].Value = "Yes";
			drivers["Member", index].Value = "No";
			drivers["Rookie", index].Value = "Yes";
			drivers["Class", index].Value = "UNK1";
			drivers.BeginEdit(true);
			drivers.Rows[index].Selected = true;
		}
		// ----------------------------------------------------------------------
		// Look for a driver number (string really)
		// Returns the grid index where it was found or -1 if not found
		private int FindDriverNumber(string number, int skipIndex)
		{
			for (int i = 0; i < drivers.RowCount; i++)
			{
				if (i == skipIndex)
				{
					continue;
				}
				if (drivers["Number",i].Value == null)
				{
					continue;
				}
				if (drivers["Number",i].Value.ToString() == number)
				{
					return i;
				}
			}
			return -1;
		}
		// ----------------------------------------------------------------------
		// do not allow duplicate driver numbers
		// Do not allow commas -- that's our separator
		private void Drivers_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			dataModified = true;  // may not be true, but could be true
			string headerText = drivers.Columns[e.ColumnIndex].HeaderText;
			
			string data = e.FormattedValue.ToString();
			if (data.Contains(","))
			{
				MessageBox.Show("Commas are not allowed");
				e.Cancel = true;
				return;
			}
			// We always trim whitespace
			data = data.Trim();
			
			if ((data == "") && (headerText.Equals("Number")))
			{
				MessageBox.Show("Blank value is not allowed");
				e.Cancel = true;
				return;
			}
			// Abort validation if cell is not in the Number column.
			if (!headerText.Equals("Number")) return;
			
			// Confirm that the value is unique
			string newnum = e.FormattedValue.ToString();
			int dupIndex = FindDriverNumber(newnum, e.RowIndex);
			
			if (dupIndex != -1)
			{
				DialogResult result = MessageBox.Show("Driver number " + newnum + " is already being used by " +
				                                      drivers["First Name",dupIndex].Value.ToString() + " " + drivers["Last Name",dupIndex].Value.ToString() +
				                                      "\n\nDo you want to use that number anyway?",
				                                      "In Use!",
				                                      MessageBoxButtons.YesNo);
				if (result == DialogResult.Yes)
				{
					string altnum = newnum + "x";
					while (FindDriverNumber(altnum, e.RowIndex) >= 0)
					{
						altnum += "x";
					}
					drivers["Number",dupIndex].Value = altnum;
				}
				else
				{
					e.Cancel = true;
				}
				return;
			}
		}
		// ----------------------------------------------------------------------
		// Force member or rookie to be Yes/No
		// Force registered to be Yes or blank
		private void Drivers_CellValidated(object sender, DataGridViewCellEventArgs e)
		{
			string headerText = drivers.Columns[e.ColumnIndex].HeaderText;
			if (drivers[e.ColumnIndex,e.RowIndex].Value == null)
				return;
			
			
			string data = drivers[e.ColumnIndex,e.RowIndex].Value.ToString();
			
			// We always trim whitespace
			data = data.Trim();
			drivers[e.ColumnIndex, e.RowIndex].Value = data;
			if (data == "")
				return;
			
			string ucData = data.ToUpperInvariant();
			
			if ((headerText.Equals("Registered") == true) ||
			    (headerText.Equals("Member") == true) ||
			    (headerText.Equals("Rookie") == true)
			   )
			{
				if ((ucData.Contains("Y") == true))
				{
					drivers[e.ColumnIndex, e.RowIndex].Value = "Yes";
				}
				else
				{
					drivers[e.ColumnIndex, e.RowIndex].Value = "No";
					if (headerText.Equals("Registered") == true)
						drivers[e.ColumnIndex, e.RowIndex].Value = "";
				}
			}
			if (headerText.Equals("Class") == true)
			{
				// look up in class data and verify it exists
				if (classData.GetField(data, "PAX") == "")
				{
					// Not found.  Try uppercase
					if (classData.GetField(ucData, "PAX") == "")
					{
						MessageBox.Show("No such class : " + data);
						drivers[e.ColumnIndex, e.RowIndex].Value = "";
						return;
					}
					else
					{
						drivers[e.ColumnIndex, e.RowIndex].Value = ucData;
					}
				}
			}
			if (headerText.Equals("Number"))
			{
				// Car number field is always uppercase
				drivers[e.ColumnIndex, e.RowIndex].Value = ucData;
			}
			// We do allow an empty group field
			if ((headerText.Equals("Group") == true) && (data != ""))
			{
				if (data.Trim() == "")
				{
					drivers[e.ColumnIndex, e.RowIndex].Value = "";
					return;
				}
				// look up in class data and verify it exists
				if (classData.GetField(data, "PAX") == "")
				{
					// Not found.  Try uppercase
					if (classData.GetField(ucData, "PAX") == "")
					{
						MessageBox.Show("No such group: " + data);
						drivers[e.ColumnIndex, e.RowIndex].Value = "";
						return;
					}
					else
					{
						drivers[e.ColumnIndex, e.RowIndex].Value = ucData;
					}
				}
			}
			// We do allow an empty group field
			if ((headerText.Equals("XGroup") == true) && (data != ""))
			{
				if (data.Trim() == "")
				{
					drivers[e.ColumnIndex, e.RowIndex].Value = "";
					return;
				}
				// look up in class data and verify it exists
				// Xgroup field allows multiple group names separated by ';'
				string[] xgroups = data.Split(';');
				for (int xgindex = 0; xgindex < xgroups.Length; xgindex++)
				{
					if (classData.GetField(xgroups[xgindex], "PAX") == "")
					{
						// Not found.  Try uppercase
						if (classData.GetField(xgroups[xgindex].ToUpper(), "PAX") == "")
						{
							MessageBox.Show("No such XGroup: " + xgroups[xgindex]);
							drivers[e.ColumnIndex, e.RowIndex].Value = "";
							return;
						}
						else
						{
							xgroups[xgindex] = xgroups[xgindex].ToUpper();
							drivers[e.ColumnIndex, e.RowIndex].Value = String.Join(";", xgroups);
						}
					}
				}
			}
		}
		// ----------------------------------------------------------------------
		void AddDriverButtonClick(object sender, EventArgs e)
		{
			dataModified = true;
			drivers.Rows.Add();
			GotoLastRegRow();
		}
		// ----------------------------------------------------------------------
		void RegForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (dataModified == true)
			{
				SaveRegData();
				dataModified = false;
			}
		}
		// ----------------------------------------------------------------------
		void UnregButtonClick(object sender, System.EventArgs e)
		{
			DialogResult result = MessageBox.Show("Really unregister all drivers?","MJ registration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);
			if (result == DialogResult.Yes)
			{
				dataModified = true;
				for (int i = 0; i < drivers.RowCount; i++)
				{
					drivers["Registered",i].Value = "";
				}
			}
		}
		// ----------------------------------------------------------------------
		void ClearNotesButtonClick(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Really clear all notes?","MJ registration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);
			if (result == DialogResult.Yes)
			{
				dataModified = true;
				try
				{
					for (int i = 0; i < drivers.RowCount; i++)
					{
						string note = drivers["Notes",i].Value.ToString();
						if (note.Contains("eason") == false)
						{
							drivers["Notes",i].Value = "";
						}
					}
				}
				catch
				{
					
				}
			}
		}
		// ----------------------------------------------------------------------
		// Create or update an HTML formatted file of all registrants
		public class PaxInfo
		{
			public string carClass;
			public string pax;
			public string description;
			public string group;
			public string displayOrder;
		}
		// ----------------------------------------------------------------------
		// Import/merge registration data from a tab separated report
		void MSRImportButtonClick(object sender, System.EventArgs e)
		{
			ImportData(',', "");
		}
		void DeleteDriversButtonClick(object sender, System.EventArgs e)
		{
			DialogResult result = MessageBox.Show("Really delete all drivers?","MJ registration", MessageBoxButtons.YesNo);
			if (result == DialogResult.No)
			{
				return;
			}
			drivers.Rows.Clear();
		}
		// ----------------------------------------------------------------------
		// Import/merge registration data from a tab separated report
		void KareloImportButtonClick(object sender, EventArgs e)
		{
			ImportData('\t', "Row");
		}
		// ----------------------------------------------------------------------
		void ImportData(char separator, string keyField)
		{
			var report = new CSVData();
			string filePath;
			
			try
			{
                var fd = new OpenFileDialog
                {
                    InitialDirectory = @"C:\"
                };

                if (fd.ShowDialog() != DialogResult.Cancel)
				{
					filePath = fd.FileName;
				}
				else
				{
					filePath = null;
				}
			}
			catch (Exception eX)
			{
				string error = eX.ToString();
				MessageBox.Show(error);
				return;
			}
			if (filePath == null)
			{
				return;
			}
			dataModified = true;
			report.LoadData(filePath,separator,keyField);
			int index = 0;
			for (int dline = 0; dline <= report.Length(); dline++)
			{
				int reportrow;
				reportrow = dline;
				string fname = report.GetField(reportrow.ToString(),"First Name");
				if (string.IsNullOrEmpty(fname) == true)
				{
					continue; // Ignore entire row if first name is blank?
				}
				fname = fname.Trim();
				
				string lname = report.GetField(reportrow.ToString(),"Last Name");
				if (string.IsNullOrEmpty(lname) == true)
				{
					continue;
				}
				lname = lname.Trim();
				try
				{
					bool driverFound = false;
					for (int driver = 0; driver < drivers.RowCount; driver++)
					{
						if (drivers["First Name",driver].Value.ToString() != fname)
						{
							continue;
						}
						if (drivers["Last Name",driver].Value.ToString() != lname)
						{
							continue;
						}
						// found the driver!
						driverFound = true;
						index = driver;
						break;
					}
					if (driverFound == false)
					{
						string carnum = report.GetField(reportrow.ToString(),"Number");
						if (string.IsNullOrEmpty(carnum) == true)
						{
							carnum = report.GetField(reportrow.ToString(),"Car Number");
						}
						if (string.IsNullOrEmpty(carnum) == true)
						{
							carnum = report.GetField(reportrow.ToString(),"Choice 1");
						}
						if (string.IsNullOrEmpty(carnum) == true)
						{
							carnum = report.GetField(reportrow.ToString(),"No.");
						}
						// carnum = carnum.TrimStart('0');	// trim leading zeroes (confusing for timing)
						if (string.IsNullOrEmpty(carnum) == true)
						{
							carnum = FindNumber().ToString();
						}
						// They might want car# plus class
						if (concat_checkbox.Checked == true)
						{
							string driverclass = report.GetField(reportrow.ToString(),"Class");
							carnum += driverclass;
						}
						bool numFound = false;
						for (int driver = 0; driver < drivers.RowCount; driver++)
						{
							if (drivers["Number",driver].Value.ToString() == carnum)
							{
								numFound = true;
								break;
							}
						}
						if (numFound == true)
						{
							// Already in use -- give him a new one
							carnum = FindNumber().ToString();
						}
						
						drivers.Rows.Add();
						index = drivers.RowCount-1;
						drivers["Number",index].Value = carnum;
						drivers["First Name", index].Value = fname;
						drivers["Last Name", index].Value = lname;
					}
					
					// Old or new, update info
					drivers["Registered", index].Value = "Yes";
					drivers["Notes",index].Value = "";
					
					string carclass = report.GetField(reportrow.ToString(),"Vehicle Class");
					if (string.IsNullOrEmpty(carclass) == true)
					{
						carclass = report.GetField(reportrow.ToString(),"Class");
					}
					if ((string.IsNullOrEmpty(carclass) == true) || (carclass.Contains("selected") == true))
					{
						carclass = "AM";  // everyone into "AM" if nothing else
					}
					if (drivers["Class", index].Value == null)
					{
						drivers["Class", index].Value = "";
					}
					// look up in class data and verify it exists
					if (classData.GetField(carclass, "PAX") == "")
					{
						// Not found.  Try uppercase
						if (classData.GetField(carclass.ToUpperInvariant(), "PAX") == "")
						{
							bool classFound = false;
							List<string> classKeys = classData.getKeys();
							foreach (string className in classKeys)
							{
								string group = classData.GetField(className, "Group");
								if (group == carclass)
								{
									drivers["Class", index].Value = className;
									classFound = true;
									break;
								}
							}
							if (classFound == false)
							{
								drivers["Class", index].Value = "AM";
							}
						}
						else
						{
							drivers["Class", index].Value = carclass.ToUpperInvariant();
						}
					}
					else
					{
						// it exists, so use the given class
						drivers["Class", index].Value = carclass;
					}
					
					string year = report.GetField(reportrow.ToString(),"Year");
					if (string.IsNullOrEmpty(year) == true)
					{
						year = "";
					}
					string make = report.GetField(reportrow.ToString(),"Make");
					if (string.IsNullOrEmpty(make) == true)
					{
						make = "";
					}
					string model = report.GetField(reportrow.ToString(),"Model");
					if (string.IsNullOrEmpty(model) == true)
					{
						model = "";
					}
					string carmodel = year + " " + make + " " + model;
					carmodel = carmodel.Trim();
					if (string.IsNullOrEmpty(carmodel) == true)
					{
						carmodel = report.GetField(reportrow.ToString(),"Vehicle Year/Make/Model");
					}
					if (string.IsNullOrEmpty(carmodel) == true)
					{
						carmodel = "Unknown";
					}
					drivers["Car Model", index].Value = carmodel;
					
					string colour = report.GetField(reportrow.ToString(),"Colour");
					if (string.IsNullOrEmpty(colour) == true)
					{
						colour = "";
					}
					colour = report.GetField(reportrow.ToString(),"Color");
					if (string.IsNullOrEmpty(colour) == true)
					{
						colour = "";
					}
					drivers["Car Color", index].Value = colour;
					
					if (drivers.Columns.Contains("Member") == true)
					{
						if ((drivers["Member", index].Value == null) ||
						    (string.IsNullOrEmpty(drivers["Member", index].Value.ToString()) == true))
						{
							drivers["Member", index].Value = "No";
							string fee = report.GetField(reportrow.ToString(),"Fee");
							if (fee == "Members")
							{
								drivers["Member", index].Value = "Yes";
							}
						}
					}
					
					if (drivers.Columns.Contains("Rookie") == true)
					{
						string novice = report.GetField(reportrow.ToString(),"Novice");
						if (novice == "")
						{
							novice = report.GetField(reportrow.ToString(),"Rookie");
						}
						if ((string.IsNullOrEmpty(novice) == true) || (novice.Contains("1")))
						{
							drivers["Rookie", index].Value = "Yes";
						}
						else if (novice.Contains("0"))
						{
							drivers["Rookie", index].Value = "No";
						}
						if ((drivers["Rookie", index].Value == null) ||
						    (string.IsNullOrEmpty(drivers["Rookie", index].Value.ToString()) == true))
						{
							drivers["Rookie", index].Value = "Yes";
						}
					}
					// special case for LADIES group
					string lady = report.GetField(reportrow.ToString(),"Ladies");
					if (lady.Contains("1"))
					{
						drivers["XGroup", index].Value = "LADIES";
					}

					string comment = report.GetField(reportrow.ToString(),"Comment");
					if (string.IsNullOrEmpty(comment) == true)
					{
						comment = "";
					}
					string payment = report.GetField(reportrow.ToString(),"Payment Method");
					if (string.IsNullOrEmpty(payment) == false)
					{
						comment = payment + comment;
					}
					else
					{
						payment = report.GetField(reportrow.ToString(),"Amnt.");
						if (string.IsNullOrEmpty(payment) == false)
						{
							comment = payment + comment;
						}
						else
						{
							payment = report.GetField(reportrow.ToString(),"Total Paid");
							comment = payment + comment;
						}
					}
					
					if (drivers.Columns.Contains("Notes") == true)
					{
						drivers["Notes", index].Value += " " + comment;
						if (driverFound == false)
						{
							drivers["Notes", index].Value += " (new entry)";
						}
					}
					
					// Do a one-to-one copy of matching headers
					List<string> hdrs = report.GetHeaders();
					foreach (string hdr in hdrs)
					{
						// Ignore special case fields
						if ((hdr == "Number") || (hdr == "First Name") || (hdr == "Last Name") ||
						    (hdr == "Class") || (hdr == "Rookie")
						   )
						{
							continue;
						}
						if (drivers.Columns.Contains(hdr))
						{
							string rawVal = report.GetField(reportrow.ToString(), hdr).Trim();
							if (hdr.Contains("WCMA"))
							{
								if (rawVal == "1")
								{
									rawVal = "Yes";
								}
								else
								{
									rawVal = "No";
								}
							}
							drivers[hdr, index].Value = rawVal;
						}
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show("Driver " + fname + "." + lname + "entry error:" + ex.Message);
				}
			}
		}
	}
}

