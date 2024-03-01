/*
 * Copyright Murray Peterson
 * Date: 8/19/2008
 * See:
 * http://www.c-sharpcorner.com/UploadFile/tameta/PlayingWithDataGrid09052005081718AM/PlayingWithDataGrid.aspx
 * http://msdn.microsoft.com/en-us/library/ms993231.aspx (dataerror event handling)
 * http://www.thescarms.com/dotnet/ColumnStyles.aspx (column widths and styles)
 */
// disable CompareOfFloatsByEqualityOperator
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace RaceBeam
{
	
	
	
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private readonly RaceBeam.SerialIO timer;
		private readonly RaceBeam.RunData timingData;
		
		private readonly BindingSource timergridBindingSource = new BindingSource();
		// -------------------------------------------------------------------
		public MainForm()
		{
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			load_config();
			InitClasses();
			timingData = new RunData(new logMsg(ShowMsg),
			                         configData.GetField("driverDataFile", "Value"),
			                         configData.GetField("backupDataFolder", "Value"),
			                         configData
			                        );
			timer = new SerialIO(new logMsg(ShowMsg), new InvokeTimeEvent(TimeEvent), configData);
			
			SetComboBox.SelectedIndex = 0; // Make Set 1 the starting selection
			
			// Bind the list to the BindingSource.
			this.timergridBindingSource.DataSource = timingData._data;
			// Attach the BindingSource to the DataGridView.
			this.timergrid.DataSource = this.timergridBindingSource;
			timergrid.RowEnter += new DataGridViewCellEventHandler(Timergrid_RowEnter);
			timergrid.RowLeave += new DataGridViewCellEventHandler(Timergrid_RowLeave);
			//timergrid.DataSource = timingData.timingData;
			timergrid.Visible = true;
			timergrid.CellEndEdit += new DataGridViewCellEventHandler(Timergrid_CellEndEdit);
			//timergrid.CellValidating += new DataGridViewCellValidatingEventHandler(Timergrid_CellValidating);
			timergrid.KeyPress += new KeyPressEventHandler(TimergridKeyPress);
			this.Closing += new System.ComponentModel.CancelEventHandler(Form_Closing);
			
			this.tabControl1.Selected += new TabControlEventHandler(TabControl1_Selected);
			scoringList.SetItemChecked(0,false);
			scoringList.SetItemChecked(2,true);		// should be PAX times
			
			set1RadioButton.Checked = true;
			
			timergrid.Columns["Car"].DisplayIndex = 0;
			timergrid.Columns["Penalty"].DisplayIndex = 1;
			timergrid.Columns["Time"].DisplayIndex = 2;
			timergrid.Columns["Run"].DisplayIndex = 3;
			timergrid.Columns["Set"].DisplayIndex = 4;
			timergrid.Columns["Driver"].DisplayIndex = 5;
			timergrid.Columns["Class"].DisplayIndex = 6;
			timergrid.Columns["Description"].DisplayIndex = 7;
			timergrid.Columns["Colour"].DisplayIndex = 8;
			
			timergrid.Columns["Car"].Width = 40;
			timergrid.Columns["Set"].Width = 40;
			timergrid.Columns["Run"].Width = 50;
			timergrid.Columns["Run"].HeaderText = "Run#";
			timergrid.Columns["Penalty"].Width = 60;
			timergrid.Columns["Time"].Width = 70;
			timergrid.Columns["Driver"].Width = 140;
			timergrid.Columns["Class"].Width = 50;
			timergrid.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			// MWP To see internal time stamps and index
			//timergrid.Columns["StartTime"].DisplayIndex = 9;
			//timergrid.Columns["StopTime"].DisplayIndex = 10;
			//timergrid.Columns["Index"].DisplayIndex = 11;
			//timergrid.Columns["Index"].ReadOnly = true;
			//timergrid.Columns["Index"].Width = 45;
			//timergrid.Columns["StopTime"].Width = 70;
			//timergrid.Columns["StartTime"].Width = 70;
			
			timingData.Clear();
			
			// default to today for day1 scoring
			Day1TextBox.Text = DateTime.Now.ToString("yyyy_MM_dd");
			
			// Create data folders if necessary
			if (Directory.Exists(configData.GetField("eventDataFolder", "Value")) == false)
			{
				Directory.CreateDirectory(configData.GetField("eventDataFolder", "Value"));
			}
			if (Directory.Exists(configData.GetField("backupDataFolder", "Value")) == false)
			{
				Directory.CreateDirectory(configData.GetField("backupDataFolder", "Value"));
			}
			// load timing file if it already exists
			if (File.Exists(timingFileName))
			{
				
				timingData.TimingFileName = timingFileName;
				timingData.Clear();
				timingData.LoadData(timingFileName);
				GotoLastRow();
				RefreshView();
			}
			else
			{
				// New file, so try to create it
				try
				{
					timingData.TimingFileName = timingFileName;
					RefreshView();
				}
				catch (System.Exception ex)
				{
					ShowMsg(ex.Message);
					MessageBox.Show("Unable to create timing file: " + timingFileName, "Hostbeam");
					timingData.TimingFileName = "";
					Environment.Exit(0);
				}
			}
			// Make a copy of the driverData file if we haven't already
			string today = DateTime.Now.ToString("yyyy_MM_dd");
			string eventDriverDataFile = configData.GetField("eventDataFolder", "Value") + "\\" + today + "_driverData.csv";
			string backupEventDriverDataFile = configData.GetField("backupDataFolder", "Value") + "\\" + today + "_driverData.csv";
			
			try
			{
				File.Copy(configData.GetField("driverDataFile", "Value"), eventDriverDataFile, true);
				// make copies into backup folder
				File.Copy(configData.GetField("driverDataFile", "Value"), backupEventDriverDataFile, true);
				File.Copy(classDataFile, configData.GetField("backupDataFolder", "Value") + "\\" + Path.GetFileName(classDataFile), true);
			}
			catch
			{
				// not interested in any errors
			}
			
			// Initialize the registration grid
			InitRegistration();
			InitClasses();
		}
		
		// -------------------------------------------------------------------
		[STAThread]
        public static void Main()
        {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			// Exit if we already have an instance of ourself running
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
			{
				return;
			}
			Application.Run(new MainForm());
		}
		// -------------------------------------------------------------------
		/// <summary>
		/// Trap the keystrokes so we can use the function keys
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (keyData == (Keys.F1))
			{
				int idx = tabControl1.SelectedIndex;
				idx += 1;
				if (idx >= 2)
				{
					idx = 0;
					timergrid.Select();
				}
				tabControl1.SelectTab(idx);
			}
			else if (keyData == (Keys.F2))
			{
				int idx = tabControl1.SelectedIndex;
				if (idx >= 5)
				{
					idx = 0;
					timergrid.Select();
				}
				else
				{
					idx = 5;
					
				}
				tabControl1.SelectTab(idx);
			}
			else if (keyData == (Keys.F3))
			{
				DialogResult r = MessageBox.Show("Reset (cancel) the last start trigger?", "Cancel Start", MessageBoxButtons.YesNo);
				if (r == DialogResult.Yes)
				{
					timingData.ResetStart();
					RefreshView();
				}
			}
			else if (keyData == (Keys.F4))
			{
				DialogResult r = MessageBox.Show("Reset (cancel) the last finish trigger?", "Cancel Finish", MessageBoxButtons.YesNo);
				if (r == DialogResult.Yes)
				{
					timingData.ResetStop();
					RefreshView();
				}
			}
			else if (keyData == (Keys.F5))
			{
				DialogResult r = MessageBox.Show("Force a start trigger?", "Trigger Start", MessageBoxButtons.YesNo);
				if (r == DialogResult.Yes)
				{
					timingData.TriggerStart();
					RefreshView();
				}
			}
			else if (keyData == (Keys.F6))
			{
				DialogResult r = MessageBox.Show("Force a finish trigger?", "Trigger Finish", MessageBoxButtons.YesNo);
				if (r == DialogResult.Yes)
				{
					timingData.TriggerStop();
					RefreshView();
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		// ----------------------------------------------------------------------
		// Update the datagrid view and reset the column ordering and visibility
		public void RefreshView()
        {
			timergrid.Refresh();
			return;
		}
		// ----------------------------------------------------------------------
		public delegate void InvokeshowMsg(string msg);
		public void ShowMsg(string msg)
		{
			// Do the invoke here to save everyone else the bother
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new InvokeshowMsg(ShowMsg), new object[] {msg});
				return;
			}
			if (msgTextBox.TextLength > 800)
			{
				msgTextBox.Clear();
			}
			
			if ((msg.StartsWith("Input data:") == true) || (msg.StartsWith("SS:") == true) ||
			    (msg.StartsWith("FE:") == true) || (msg.StartsWith("FS:") == true) ||
			    (msg.StartsWith("FSync:") == true) || (msg.StartsWith("SSync:") == true) ||
			    (msg.StartsWith("FInit:") == true) || (msg.StartsWith("SInit:") == true) ||
			    (msg.StartsWith("FTrip:") == true) || (msg.StartsWith("STrip:") == true) ||
			    (msg.StartsWith("Q") == true)
			   )
			{
				if (configData.GetField("logTimingMessages", "Value").Contains("Y"))
				{
					msgTextBox.AppendText(msg);
				}
			}
			else
			{
				msgTextBox.AppendText(msg);
			}
			//msgTextBox.Focus();
			msgTextBox.SelectionStart = msgTextBox.Text.Length;
			//timergrid.Focus();
		}
		// ----------------------------------------------------------------------
		public void TimeEvent(string type, string time)
		{
			// Do the invoke here to save everyone else the bother
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new InvokeTimeEvent(TimeEvent), new object[] {type,time});
				return;
			}

			if (type == "barcode")
			{
				// find matching barcode
				string carnum = FindBarcode(time);
				if (carnum == null)
				{
					ShowMsg("No driver found with barcode of " + time + "\n");
					carnum = "NOBARCODE";
					
					
				}
				for (int i = 0; i < timingData._data.Count; i++)
				{
					var r = (Runtime)(timingData._data[i]);
					if ((string.IsNullOrEmpty(r._car_number) == true) && (r.Penalty != "RRN"))
					{
						// Do not move focus away from user input
						// timergrid.CurrentCell = timergrid[0,i];
						r.Car = carnum;
						if (carnum == "NOBARCODE")
						{
							// find cell and colour it red
							this.timergrid.Rows[i].Cells["Car"].Style.BackColor = Color.Red;
						}
						else
						{
							// find cell and colour it white
							this.timergrid.Rows[i].Cells["Car"].Style.BackColor = Color.White;
						}
						timingData.Carchange(i, false);
						RefreshView();
						break;
					}
					
				}
			}
			else if (type == "Q")
			{
				// Check for status message and handle it here
				// Format: Sdddddd
				// First digit is start beam status
				// Second digit is stop beam status
				// 0x01 bit: radio good (0) or bad (1)
				// 0x02 bit: sync good (0) or bad (1)
				// 0x04 bit: alignment good (0) or bad (1)
				string startbits = time.Substring(0,1);
                int.TryParse(startbits, out int startStatus);
                if ((startStatus & 0x03) != 0)
				{
					// radio is 0x01 bit and sync is 0x02 bit
					startRadio.BackColor = Color.Red;
				}
				else
				{
					startRadio.BackColor= Color.Lime;
				}
				if ((startStatus & 0x04) != 0)
				{
					startAlign.BackColor = Color.Red;
				}
				else
				{
					startAlign.BackColor= Color.Lime;
				}
				string stopbits = time.Substring(1,1);
                int.TryParse(stopbits, out int stopStatus);
                if ((stopStatus & 0x03) != 0)
				{
					// radio is 0x01 bit and sync is 0x02 bit
					stopRadio.BackColor = Color.Red;
				}
				else
				{
					stopRadio.BackColor= Color.Lime;
				}
				if ((stopStatus & 0x04) != 0)
				{
					stopAlign.BackColor = Color.Red;
				}
				else
				{
					stopAlign.BackColor= Color.Lime;
				}
			}
			else
			{
				// AC4 needs to generate 2 events here -- start with time 000000 and finish with given time
				bool AC4Protocol = false;  // weird device that only sends finish triggers
				string AC4proto = configData.GetField("AC4Protocol", "Value");
				AC4proto = AC4proto.ToUpperInvariant();
				if ((string.IsNullOrEmpty(AC4proto) == false) && (AC4proto.StartsWith("Y") == true))
				{
					AC4Protocol = true;
				}
				else
				{
					AC4Protocol = false;
				}
				if (AC4Protocol == true)
				{
					timingData.TimingEvent("A", "000000");  // Generate a false start trigger with time 0
					type = "B";  // Convert event to a finish trigger
					if (string.IsNullOrEmpty(time) == false)
					{
						char[] charArray = time.ToCharArray();
						Array.Reverse(charArray);
						string revTime = new string(charArray);
						PenaltyAndTime revmsg = timingData.TimingEvent(type, revTime);
						if (double.TryParse(revTime, out double dval) == true)
						{
							timer.DisplayTime(revmsg);
						}
					}
					RefreshView();
				}
				else
				{
					// normal time event (not AC4)
					PenaltyAndTime msg = timingData.TimingEvent(type, time);
					if (string.IsNullOrEmpty(msg.time) == false)
					{
						if (double.TryParse(msg.time, out double dval) == true)
						{
							timer.DisplayTime(msg);
						}
					}
					RefreshView();
				}
			}
		}
		// ----------------------------------------------------------------------
		private void TabControl1_Selected(Object sender, TabControlEventArgs e)
		{
			
			if (e.TabPage.Name == "scoringTabPage")
			{
				GenScores(false);
			}
			else if (e.TabPage.Name == "HeatsTabPage")
			{
				string heatInfo = Heats.doHeatCalcs();
				HeatsTextBox.Clear();
				HeatsTextBox.AppendText(heatInfo);
			}
			else if (e.TabPage.Name == "COMPortsTabPage")
			{
				List<COMPortInfo> comportList = COMPortInfo.GetCOMPortsInfo();
				COMPortsTextBox.Clear();
				foreach (COMPortInfo comPort in comportList)
				{
					if (comPort.Name == "COM1")
					{
						//continue;
					}
					COMPortsTextBox.AppendText(comPort.Description + "\n");
				}
			}
			else if (e.TabPage.Name == "registrationTabPage")
			{
				LoadRegData();
			}
			else if (e.TabPage.Name == "timingTabPage")
			{
				timingData.LoadDriverData();
			}
			else if (e.TabPage.Name == "configurationTabPage")
			{
				
			}
		}
		// ----------------------------------------------------------------------
		void TimerPortButtonClick(object sender, EventArgs e)
		{
			// Check if we have a date rollover
			string today = DateTime.Now.ToString("yyyy_MM_dd");
			if (timingFileName.Contains(today) == false)
			{
				MessageBox.Show("You must exit and restart MJTiming -- date has changed!", "MJ Timing", MessageBoxButtons.OK);
				return;
			}
			
			if (TimerPortButton.Text.Contains("Stopped"))
			{
				if (timer.StartTiming() == false)
				{
					string timerPortName = configData.GetField("timerPort", "Value");
					MessageBox.Show("Unable to open timer port " + timerPortName);
					return;
				}
				TimerPortButton.Text = "Timer Running";
				TimerPortButton.BackColor = Color.Red;
				TimerPortButton.ForeColor = Color.White;
				timingData.CancelTimers();
				this.timergrid.Refresh();
			}
			else
			{
				TimerPortButton.Text = "Timer Stopped";
				TimerPortButton.BackColor = Color.Lime;
				TimerPortButton.ForeColor = Color.Black;
				timer.StopTiming();
				startRadio.BackColor = Color.White;
				startAlign.BackColor = Color.White;
				stopRadio.BackColor = Color.White;
				stopAlign.BackColor = Color.White;
				timingData.CancelTimers();
				this.timergrid.Refresh();
			}
		}
		
		// ----------------------------------------------------------------------
		private void TimergridCellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			//showMsg("Click " + e.ColumnIndex.ToString() + " Row: " + e.RowIndex.ToString());
		}
		// ----------------------------------------------------------------------
		void TimergridKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 12)
			{
				// ctrl-L (go to first empty row)
				GotoLastRow();
			}
			else if (e.KeyChar == 19)
			{
				// ctrl-S (save data)
				timingData.WriteData();
				ShowMsg("CTRL-S -- saving data\n");
			}
			else
			{
				//int charval = (int)e.KeyChar;
				//string msg = "Char: " + charval.ToString() + "\n";
				//showMsg(msg);
			}
			
		}
		// ----------------------------------------------------------------------
		void GotoLastRow()
		{
			// ctrl-L (go to first empty row)
			for (int i = 0; i < timingData._data.Count; i++)
			{
				var r = (Runtime)(timingData._data[i]);
				if ((string.IsNullOrEmpty(r._car_number) == true) && (r.Penalty != "RRN"))
				{
					timergrid.CurrentCell = timergrid[0,i];
					break;
				}
			}
		}
		// ----------------------------------------------------------------------
		// twiddle current column (cannot change row or we call here recursively)
		void Timergrid_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
		}
		// ----------------------------------------------------------------------
		// twiddle current column (cannot change row or we call here recursively)
		void Timergrid_RowLeave(object sender, DataGridViewCellEventArgs e)
		{
		}
		// ----------------------------------------------------------------------
		// Validate user input
		// Also, add a new row if this is the last row in the data array
		void Timergrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			string headerText = timergrid.Columns[e.ColumnIndex].HeaderText;
			
			var x = (Runtime)(timingData._data[e.RowIndex]);
			if (headerText.Equals("Penalty"))
			{
				timingData.Fix_penalty(e.RowIndex);
			}
			else if (headerText.Equals("Car"))
			{
				x.Set = timingData.RunDay;
				if (x.Car != null)
				{
					x.Car = x.Car.Trim();
					x.Car = x.Car.ToUpperInvariant();
				}
				if (x.Car == "")
				{
					x.Car = null;
				}
				timingData.Carchange(e.RowIndex, false);
			}
			else if (headerText.Equals("Set"))
			{
				// TODO Change background colour when set # changes
				timingData.WriteData();
			}
			else if (headerText.Equals("Time"))
			{
                // ensure that it has a decimal part
                if (x.Time.Contains(".") == false)
                {
                    x.Time += ".000";
                }

                if (double.TryParse(x.Time, out double fval) == false)
				{
					x.Time = "999.999";
					fval = 999.999;
					ShowMsg("Not a valid time value.\n");
				}
				x._start_time = "000000";
				x._stop_time = (fval * 1000).ToString();
                var timeData = new PenaltyAndTime
                {
                    time = x._run_time,
                    penalty = x._penalty
                };
                timer.DisplayTime(timeData);
				x.Set = timingData.RunDay;
				timingData.WriteData();
			}
			RefreshView();
			
		}
		// -------------------------------------------------------------------
		void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			DialogResult d = MessageBox.Show("Do you really wish to exit?", "MJ Timing", MessageBoxButtons.YesNo);
			if (d == DialogResult.Yes)
			{
				timer.StopTiming();
				timingData.CancelTimers();
				timingData.StopSaveThread();
				System.Threading.Thread.Sleep(500);
				SaveRegData();
				Save_config();
				SaveClassData();
				// delete driver data and timing data files if no timing data exists
				//TODO don't delete anything until we can do it reliably!
				var r = (Runtime)(timingData._data[0]);
				if ((string.IsNullOrEmpty(r._car_number) == true) &&
				    (string.IsNullOrEmpty(r.Penalty) == true) &&
				    ((string.IsNullOrEmpty(r._run_time) == true))
				   )
				{
					string today = DateTime.Now.ToString("yyyy_MM_dd");
					string eventDriverDataFile = configData.GetField("eventDataFolder", "Value") + "\\" + today + "_driverData.csv";
					if (File.Exists(eventDriverDataFile) == true)
					{
						//File.Delete(eventDriverDataFile);
					}
					string timingDataFile = configData.GetField("eventDataFolder", "Value") + "\\" + today + "_timingData.csv";
					if (File.Exists(timingDataFile) == true)
					{
						//File.Delete(timingDataFile);
					}
				}
			}
			else
				e.Cancel = true;
		}
		
		// -------------------------------------------------------------------
		void TriggerStartButtonClick(object sender, EventArgs e)
		{
			DialogResult r = MessageBox.Show("Force a start trigger?", "Trigger Start", MessageBoxButtons.YesNo);
			if (r == DialogResult.Yes)
			{
				timingData.TriggerStart();
				RefreshView();
			}
		}
		// -------------------------------------------------------------------
		void TriggerStopButtonClick(object sender, EventArgs e)
		{
			DialogResult r = MessageBox.Show("Force a finish trigger?", "Trigger Finish", MessageBoxButtons.YesNo);
			if (r == DialogResult.Yes)
			{
				timingData.TriggerStop();
				RefreshView();
			}
		}
		// ----------------------------------------------------------------------
		void StartResetButtonClick(object sender, EventArgs e)
		{
			DialogResult r = MessageBox.Show("Reset (cancel) the last start trigger?", "Cancel Start", MessageBoxButtons.YesNo);
			if (r == DialogResult.Yes)
			{
				timingData.ResetStart();
				RefreshView();
			}
		}
		// ----------------------------------------------------------------------
		void StopResetButtonClick(object sender, EventArgs e)
		{
			DialogResult r = MessageBox.Show("Reset (cancel) the last finish trigger?", "Cancel Finish", MessageBoxButtons.YesNo);
			if (r == DialogResult.Yes)
			{
				timingData.ResetStop();
				RefreshView();
			}
		}
		// -------------------------------------------------------------------
		void SetComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SetComboBox.SelectedIndex == 0)
			{
				timingData.RunDay = "1";
			}
			else if (SetComboBox.SelectedIndex == 1)
			{
				timingData.RunDay = "2";
			}
			else
			{
				timingData.RunDay = "3";
			}
		}
		// -------------------------------------------------------------------
		void ScoringListSelectedIndexChanged(object sender, EventArgs e)
		{
			GenScores(false);
		}
		void ScoreModifiersSelectedIndexChanged(object sender, EventArgs e)
		{
			GenScores(false);
		}
		void OneDayCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			GenScores(false);
		}
		void Set1RadioButtonCheckedChanged(object sender, EventArgs e)
		{
			GenScores(false);
		}
		void Day2RadioButtonCheckedChanged(object sender, EventArgs e)
		{
			GenScores(false);
		}
		void Day1PlusDay2RadioButtonCheckedChanged(object sender, EventArgs e)
		{
			GenScores(false);
		}
		void BestRunRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			GenScores(false);
		}
		void TabControl1Deselecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPage.Name == "registrationTabPage")
			{
				SaveRegData();
			}
			else if (e.TabPage.Name == "configurationTabPage")
			{
				Save_config();
				LoadRegData();		// might have changed!
				InitClasses();
			}
			else if (e.TabPage.Name == "classesTabPage")
			{
				SaveClassData();
				InitClasses();
			}
		}
		// ----------------------------------------------------------------------
		void ScoreButtonClick(object sender, EventArgs e)
		{
			GenScores(true);
		}
		void GenScores(bool createFiles)
		{
			var args = new scoreArgs();
			
			if (Day1TextBox.Text == "")
			{
				Day1TextBox.Text = DateTime.Now.ToString("yyyy_MM_dd");
			}
			args.day1 = Day1TextBox.Text;
			args.day2 = Day2TextBox.Text;

			if (int.TryParse(OfficialRunsTextBox.Text, out args.maxOfficialRuns) == false)
			{
				args.maxOfficialRuns = 999;
			}
			args.writeCSV = false;
			args.eventFolder = "";
			
			foreach (string item in scoringList.CheckedItems)
			{
				if (item.Contains("Run Times"))
					args.showRunTimes = true;
				else if (item.Contains("Raw Times"))
					args.showRawTimes = true;
				else if (item.Contains("Class Times"))
					args.showClassTimes = true;
				else if (item.Contains("PAX Times"))
					args.showPaxTimes = true;
				else if (item.Contains("Cone"))
					args.showConeCounts = true;
				else if (item.Contains("Team"))
					args.showTeams = true;
			}
			foreach (string item in scoreModifiers.CheckedItems)
			{
				if (item.Contains("Rookie"))
					args.showRookie = true;
			}
			
			if (bestRunRadioButton.Checked == true)
			{
				args.bestSingleRun = true;
			}
			else if (set1PlusSet2RadioButton.Checked == true)
			{
				args.set1PlusSet2 = true;	// score as recorded
			}
			else if (set1RadioButton.Checked == true)
			{
				args.set1Only = true;
			}
			if (set2RadioButton.Checked == true)
			{
				args.set2Only = true;
			}
			
			if (timingData.IsSaveInProgress() == true)
			{
				scoresTextBox.Clear();
				scoresTextBox.AppendText("Save data is in progress -- please try again later\n");
			}
			else
			{
				if (createFiles == true)
				{
					args.writeCSV = true;
				}
				//var tmpArgs = new scoreArgs(args);
				string results = textScores.textScore(args);
				scoresTextBox.Clear();
				scoresTextBox.AppendText(results);
				scoresTextBox.SelectionStart = 0;
				scoresTextBox.ScrollToCaret();
				if (createFiles == true)
				{
					string eventFolder = configData.GetField("eventDataFolder", "Value");

					// We create a script file that will regen these results
					// Usage: rbscore -day1 <day1 date> -day2 <day2 date> -oneday -runtimes -classtimes -classrookie
					// -rawtimes -paxtimes -rookie -conecounts -title <string> -path <path to event data folder>
					string binFolder = Process.GetCurrentProcess().MainModule.FileName;
					binFolder = Path.GetDirectoryName(binFolder);
					string txtcmd = binFolder + "\\mjCmdLineScoring.exe -day1 " + Day1TextBox.Text;
					string htmlcmd = binFolder + "\\htmlScores.exe -day1 " + Day1TextBox.Text;
					string cmd = "";
					if (Day2TextBox.Text != "")
					{
						cmd += " -day2 " + Day2TextBox.Text;
					}
					if (args.showRunTimes == true)
					{
						cmd += " -runtimes";
					}
					if (args.showRawTimes == true)
					{
						cmd += " -rawtimes";
					}
					if (args.showPaxTimes == true)
					{
						cmd += " -paxtimes";
					}
					if (args.showClassTimes == true)
					{
						cmd += " -classtimes";
					}
					if (args.showTeams == true)
					{
						cmd += " -teams";
					}
					if (args.showConeCounts == true)
					{
						cmd += " -conecounts";
					}
					if (args.showRookie == true)
					{
						cmd += " -rookie";
					}
					if (args.set1Only == true)
					{
						cmd += " -set1only";
					}
					if (args.set2Only == true)
					{
						cmd += " -set2only";
					}
					if (args.bestSingleRun == true)
					{
						cmd += " -bestsinglerun";
					}
					if (args.set1PlusSet2 == true)
					{
						cmd += " -set1plusset2";
					}
					if (args.maxOfficialRuns < 99)
					{
						cmd += " -maxofficialruns " + args.maxOfficialRuns.ToString();
					}
					if (titleTextBox.Text != "")
					{
						cmd += " -title \"" + titleTextBox.Text + "\"";
					}
					cmd += " -path . ";
					
					string htmlCmd = cmd;

					string cmdfileName;
					if (Day2TextBox.Text != "")
					{
						cmd += " -out " + args.day2 + "__2-day" + "__scores.txt";
						cmdfileName = eventFolder + "\\" + args.day2 + "_2-day" + "__scoreCMD.bat";
					}
					else
					{
						cmd += " -out " + args.day1 + "__scores.txt";
						cmdfileName =   eventFolder + "\\" + args.day1 + "__scoreCMD.bat";
					}
					try
					{
						TextWriter tw = new StreamWriter(cmdfileName);
						tw.WriteLine(txtcmd + cmd);
						tw.WriteLine(htmlcmd + htmlCmd);
						tw.Close();
						string oldDir = Directory.GetCurrentDirectory();
						Directory.SetCurrentDirectory(eventFolder);
						Process.Start(cmdfileName);  //create scores
						Process.Start(eventFolder);	// Open explorer window to folder
						Directory.SetCurrentDirectory(oldDir);
					}
					catch (Exception ex)
					{
						scoresTextBox.Clear();
						scoresTextBox.AppendText(ex.Message);
					}
				}
			}
		}
	}
	public delegate void logMsg(string msg);
	public delegate void InvokeTimeEvent(string type, string time);
}