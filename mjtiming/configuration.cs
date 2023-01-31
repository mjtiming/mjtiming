using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using RaceBeam;

namespace RaceBeam
{
	public partial class MainForm : Form
	{
		public string timingFileName = null;
		public string configFolder;
		public string mjFolder;
		public CSVData configData = new CSVData();
		string configFilename = "";
		public void load_config()
		{
			mjFolder = Process.GetCurrentProcess().MainModule.FileName;
			mjFolder = Path.GetDirectoryName(mjFolder);
			mjFolder += "\\..";
			configFilename =  mjFolder + "\\config\\configData.csv";
			configFolder = Path.GetDirectoryName(configFilename);
			
			// First see if we need to copy our template files for a new install
			if (Directory.Exists(configFolder) == false)
			{
				Directory.CreateDirectory(configFolder);
			}
			string templateFolder = mjFolder + "\\config_templates";
			if (File.Exists(configFilename) == false)
			{
				File.Copy(templateFolder + "\\configData.csv", configFolder + "\\configData.csv", false);
			}
			// Copy the others too
			if (File.Exists(configFolder + "\\_driverData.csv") == false)
			{
				File.Copy(templateFolder + "\\_driverData.csv", configFolder + "\\_driverData.csv", false);
			}
			if (File.Exists(configFolder + "\\_classData.csv") == false)
			{
				File.Copy(templateFolder + "\\_classData.csv", configFolder + "\\_classData.csv", false);
			}
			if (File.Exists(configFolder + "\\_webStyle.txt") == false)
			{
				File.Copy(templateFolder + "\\_webStyle.txt", configFolder + "\\_webStyle.txt", false);
			}
			// Check for scoring style file
			if (File.Exists(configFolder + "\\_scoreStyles.css") == false)
			{
				File.Copy(templateFolder + "\\_scoreStyles.css", configFolder + "\\_scoreStyles.css", false);
			}

			string err = configData.LoadData(configFilename,',',"Parameter");
			if (err != "")
			{
				MessageBox.Show("Unable to load config file: " + err, "MJTiming");
				Environment.Exit(0);
			}
			configurationDataGridView.Rows.Clear();
			List<string> columns = configData.GetHeaders();
			foreach (string col in columns)
			{
				if (configurationDataGridView.Columns.Contains(col) == true)
				{
					continue;
				}
				configurationDataGridView.Columns.Add(col, col);
			}
			configurationDataGridView.Columns["Parameter"].ReadOnly = true;
			configurationDataGridView.Columns["Parameter"].Width = 200;
			configurationDataGridView.Columns["Description"].ReadOnly = true;
			configurationDataGridView.Columns["Description"].Width = 400;
			configurationDataGridView.Columns["Value"].Width = 300;
			configurationDataGridView.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			configurationDataGridView.Columns["Description"].MinimumWidth = 10;
			configurationDataGridView.CellValidating += new DataGridViewCellValidatingEventHandler(Config_CellValidating);
			configurationDataGridView.CellDoubleClick += new DataGridViewCellEventHandler(Config_CellContentClick);
			List<string> parameters = configData.getKeys();
			foreach (string parm in parameters)
			{
				var record = new string[columns.Count];
				int i = 0;
				foreach (string col in columns)
				{
					record[i++] = configData.GetField(parm, col);
				}
				configurationDataGridView.Rows.Add(record);
			}
			string today = DateTime.Now.ToString("yyyy_MM_dd");
			timingFileName = configData.GetField("eventDataFolder", "Value");
			if (timingFileName == "")
			{
				MessageBox.Show("Timing data folder not defined in config file");
				Environment.Exit(0);
			}
			timingFileName += "\\" + today + "_timingData.csv";
			//showMsg("Timing data: " + timingFileName + "\n");
			string timerPortName = configData.GetField("timerPort", "Value");
			if (timerPortName == "")
			{
				MessageBox.Show("Timer port not defined in config file");
			}
		}
		// ----------------------------------------------------------------------
		public void Save_config()
		{
			try
			{
				TextWriter tw = new StreamWriter(configFilename);
				
				// First write out header row
				string row = "Parameter,Value,Description";
				tw.WriteLine(row);
				
				for (int rowindex = 0; rowindex < configurationDataGridView.RowCount; rowindex++)
				{
					if (configurationDataGridView["Parameter",rowindex].Value == null)
					{
						continue;
					}
					row = "";
					row = configurationDataGridView["Parameter",rowindex].Value.ToString();
					if (string.IsNullOrEmpty(row) == true)
					{
						continue;
					}
					if (configurationDataGridView["Value",rowindex].Value == null)
					{
						row += ",";
					}
					else
					{
						row += "," + configurationDataGridView["Value",rowindex].Value.ToString();
					}
					if (configurationDataGridView["Description",rowindex].Value == null)
					{
						row += ",";
					}
					else
					{
						row += "," + configurationDataGridView["Description",rowindex].Value.ToString();
					}
					tw.WriteLine(row);
				}
				tw.Close();
				// now re-load all the data
				configurationDataGridView.Rows.Clear();
				load_config();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + ex.StackTrace);
			}
		}
		// ----------------------------------------------------------------------
		private void Config_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			
			string headerText = configurationDataGridView.Columns[e.ColumnIndex].HeaderText;
			if (headerText != "Value")
			{
				return;
			}
			return;
		}
		// ----------------------------------------------------------------------
		// do not allow folder entries that do not exist
		// Do not allow commas -- that's our separator
		private void Config_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			dataModified = true;  // may not be true, but could be true
			string headerText = configurationDataGridView.Columns[e.ColumnIndex].HeaderText;
			string data = e.FormattedValue.ToString();
			if (data.Contains(","))
			{
				MessageBox.Show("Commas are not allowed");
				e.Cancel = true;
				return;
			}
			
			// Abort edit if cell is not in the Value column.
			if (headerText.Equals("Value") == false)
			{
				//e.Cancel = true;
				return;
			}
			
			string param = configurationDataGridView[0, e.RowIndex].Value.ToString();
			
			// Confirm that the value is valid if it is a path
			if ((param == "driverDataFile") || (param == "classDataFile"))
			{
				if (File.Exists(data) == false)
				{
					MessageBox.Show("No such file");
					e.Cancel = true;
					return;
				}
				// don't overwrite a new driverdatafile with loaded contents from previous one
				if (param == "driverDataFile")
				{
					LoadRegData();
				}
				if (param == "classDataFile")
				{
					InitClasses();
				}
				
				return;
			}
			// Confirm that the value is valid if it is a folder
			if ((param == "eventDataFolder") || (param == "backupDataFolder"))
			{
				if (Directory.Exists(data) == false)
				{
					MessageBox.Show("No such folder");
					e.Cancel = true;
					return;
				}
				// It exists, so fine by us
				return;
			}
		}
	}
}