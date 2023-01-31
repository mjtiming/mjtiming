// Class for handling the timing data
// Used as the source for the datagridview
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Media;
using System.Globalization;
using System.Windows.Forms;
using RaceBeam;
namespace RaceBeam
{
	// Our class for defining the visible grid
	public class Runtime
	{
		public int index;
		public string _datetime;
		public string _datetime_stop;
		public string _set;
		public string _car_number;
		public string _start_time;
		public string _split_time;
		public string _stop_time;
		public string _run_time;
		public string _penalty;
		public string _run_number;
		// not stored on disk
		public string _car_class;
		public string _driver;
		public string _car_description;
		public string _car_colour;
		public uint   _lap_count;
		public Runtime()
		{
			_lap_count = 0;
		}
		// Names that the gridview will show (and in the same order)
		public string Car
		{
			get { return _car_number; }
			set { _car_number = value; }
		}
		public string Penalty
		{
			get { return _penalty; }
			set { _penalty = value; }
		}
		public string Time
		{
			get
			{
				if (_run_time == null)
					_run_time = "";
				return _run_time;
			}
			set
			{
				_run_time = value;
			}
		}
		public string Run
		{
			get { return _run_number; }
		}
		public string Set
		{
			get { return _set; }
			set { _set = value; }
		}
		
		public string Driver
		{
			get { return _driver; }
		}
		public string Class
		{
			get { return _car_class; }
		}
		public string Description
		{
			get { return _car_description; }
		}
		public string Colour
		{
			get { return _car_colour; }
		}
		
		#if MWP // to see internal time stamps
		public string StartTime
		{
			get { return _start_time; }
		}
		public string StopTime
		{
			get { return _stop_time; }
		}
		public int Index
		{
			get { return index; }
		}
		#endif
	}
	
	public class RunData
	{
		// Where all of our timing data is stored
		// We expose this to allow the gridview to display it
		public ArrayList _data = new ArrayList();
		private CSVData driverData = new CSVData();
		private readonly string driverDataPath = "";
		private readonly CSVData configData = new CSVData();

        // We use this to display messages to the user
        readonly logMsg showMsg = null;
		private static string filename = null;
		private static string backupFolder = null;        // Folder where we make copies of our timing data
		private static int backupNumber = 1;          // we make backups with a number to distinguish them
		private string _runDay = "1";
		private const int maxRunData = 1500;		// max runs in a day
		private static Semaphore TriggerSema;
		private static bool exitRequested = false;
		private static bool exitDone = false;
		private static bool saveInProgress = false;
		// -------------------------------------------------------------------
		// constructor
		public RunData(logMsg showMsgDelegate, string driverDataLocation, string backup, CSVData _configData)
		{
			backupFolder = backup;
			driverDataPath = driverDataLocation;
			configData = _configData;
			
			// So we can display messages to the user
			showMsg = showMsgDelegate;
			for (int i = 0; i < maxRunData; i++)
			{
                Runtime x = new Runtime
                {
                    index = i
                };
                _data.Add(x);
			}
			// Read in default driverData file if it exists
			LoadDriverData();
			// Create data save thread and the semaphore
			TriggerSema = new Semaphore(0,1);

            var thread = new Thread(new ThreadStart(SaveDataThread))
            {
                IsBackground = true
            };
            thread.Start();
		}
		// -------------------------------------------------------------------
		public void LoadDriverData()
		{
			// Read in default driverData file if it exists
			try
			{
				driverData = new CSVData();
				string err = driverData.LoadData(driverDataPath,',',"Number");
				if (err != "")
				{
					showMsg(err + "\n");
				}
			}
			catch
			{
				
			}
		}
		// -------------------------------------------------------------------
		public string TimingFileName
		{
			get { return filename; }
			set { filename = value; }
		}
		// -------------------------------------------------------------------
		public string RunDay
		{
			set { _runDay = value; }
			get { return _runDay; }
		}
		// -------------------------------------------------------------------
		// Wipe out all data and start fresh
		public void Clear()
		{
			for (int i = 0; i < maxRunData; i++)
			{
                var x = new Runtime
                {
                    index = i
                };
                _data[i] = x;
			}
		}
		// -------------------------------------------------------------------
		public void StopSaveThread()
		{
			// Request thread to exit
			exitRequested = true;
			// Wake him up
			try
			{
				TriggerSema.Release();
			}
			catch
			{
				
			}
			while (exitDone == false)
				System.Threading.Thread.Sleep(10);
		}
		// -------------------------------------------------------------------
		public bool IsSaveInProgress()
		{
			return saveInProgress;
		}
		
		// -------------------------------------------------------------------
		public void SaveDataThread()
		{
			// Create a data array the same size as the real one
			var SaveData = new ArrayList();
			for (int i = 0; i < maxRunData; i++)
			{
                var x = new Runtime
                {
                    index = i
                };
                SaveData.Add(x);
			}
			while(exitRequested == false)
			{
				// Wait for trigger to start saving
				saveInProgress = false;
				TriggerSema.WaitOne();
				if (exitRequested == true)
				{
					exitDone = true;
					return;
				}
				// Make copy of data
				saveInProgress = true;
				try
				{
					for (int i = 0; i < maxRunData; i++)
					{
						var dst = (Runtime) SaveData[i];
						var src = (Runtime) _data[i];
						dst.index = i;
						dst._datetime = src._datetime;
						dst._datetime_stop = src._datetime_stop;
						dst._set = src._set;
						if (dst._set == "")
						{
							dst._set = "3";
							dst.Penalty = "RRN";
						}
						dst._car_number = src._car_number;
						dst._start_time = src._start_time;
						dst._split_time = src._split_time;
						dst._stop_time = src._stop_time;
						dst._run_time = src._run_time;
						dst._penalty = src._penalty;
						dst._run_number = src._run_number;
					}
				}
				catch
				{
					// oops -- something went wrong.  Do not write out bogus data
					showMsg("Exception while reading data -- skipping this save!\n");
					saveInProgress = false;
					continue;
				}
				// Now we can write our own copy of the data in our own time
				try
				{
					TextWriter tw = new StreamWriter(TimingFileName);
					tw.WriteLine("index,datetime,datetime_stop,day,car_number,start_time,split_time,stop_time,run_time,penalty,run_number");
					for (int i = 0; i < SaveData.Count; i++)
					{
						var x = (Runtime)(SaveData[i]);
						if ((string.IsNullOrEmpty(x._run_time) == true) && (x._penalty != "RRN"))
						{
							break;
						}
						tw.WriteLine(x.index.ToString() +
						             "," + x._datetime +
						             "," + x._datetime_stop +
						             "," + x._set +
						             "," + x._car_number +
						             "," + x._start_time +
						             "," + x._split_time +
						             "," + x._stop_time +
						             "," + x._run_time +
						             "," + x._penalty +
						             "," + x._run_number);
						// Sleep a tiny bit to ensure main thread gets to run
						if ((i % 20) == 0)
						{
							System.Threading.Thread.Sleep(1);
						}
					}
					
					tw.Close();
					// Make a backup copy
					if (backupFolder != null)
					{
						// Copy timing file without name change
						File.Copy(TimingFileName, backupFolder + "\\" + Path.GetFileName(TimingFileName), true);
						
						string backupFilename = backupFolder + "\\timingdata_" + backupNumber.ToString() + ".csv";
						File.Copy(TimingFileName,backupFilename,true);
						// We back up the previous 20 changes
						backupNumber += 1;
						if (backupNumber > 19)
							backupNumber = 0;
					}
					//showMsg("Data saved at " + DateTime.Now.ToString("hh:mm:ss") + "\n");
				}
				catch (Exception ex)
				{
					showMsg("Unable to write data to file: " +  ex.Message + "\n");
				}
				saveInProgress = false;
			}
			exitDone = true;
		}
		// -------------------------------------------------------------------
		// Write out run data in CSV format to a file
		public void WriteData()
		{
			// He might already be busy, so catch this exception
			try
			{
				TriggerSema.Release();
			}
			catch
			{
				
			}
		}
		// -------------------------------------------------------------------
		// Reads in the timing data from the given file
		public void LoadData(string filename)
		{
            string sval;

            new CSVData().LoadData(filename, ',',"index");
			_data.Clear();
			for (int i = 0; i < new CSVData().Length(); i++)
			{
                var x = new Runtime
                {
                    index = i
                };
                string indexS = i.ToString();
				x._datetime = new CSVData().GetField(indexS, "datetime");
				x._datetime_stop = new CSVData().GetField(indexS, "datetime_stop");
				x._car_number = new CSVData().GetField(indexS, "car_number");
				
				sval = new CSVData().GetField(indexS, "start_time");
                if (UInt32.TryParse(sval, out _) == true)
				{
					x._start_time = sval;
				}
				sval = new CSVData().GetField(indexS, "split_time");
				if (UInt32.TryParse(sval, out _) == true)
				{
					x._split_time = sval;
				}
				sval = new CSVData().GetField(indexS, "stop_time");
				if (UInt32.TryParse(sval, out _) == true)
				{
					x._stop_time = sval;
				}
				sval = new CSVData().GetField(indexS, "run_time");
				sval = sval.Trim();
				if (sval != "")
				{
                    if (Double.TryParse(sval, out _) == false)
                    {
                        sval = "0";
                    }
                }
				x._run_time = sval;
				x._penalty = new CSVData().GetField(indexS, "penalty");
				x._run_number = new CSVData().GetField(indexS, "run_number");
				x._set = new CSVData().GetField(indexS, "set");
				if (x._set == "")
				{
					x._set = new CSVData().GetField(indexS, "day");
				}
				if (x._set == "")
				{
					x._set = "3";
					x._penalty = "RRN";
				}
				_data.Add(x);
				if (string.IsNullOrEmpty(x._car_number) == false)
				{
					Carchange(x.index, true);
				}
			}
			
			for (int i = _data.Count; i < maxRunData; i++)
			{
                var x = new Runtime
                {
                    index = i
                };
                _data.Add(x);
			}
		}
		// -------------------------------------------------------------------
		// An incoming timing event
		// Returns time string to send to display, empty string if nothing to show
		
		public static bool isReversed = false;
		
		public PenaltyAndTime TimingEvent(string type, string time)
		{
			PenaltyAndTime returnTime = new PenaltyAndTime();
			PenaltyAndTime noTime =  new PenaltyAndTime();
			int index;
			bool found = false;
			bool playSounds = false;
			string reverse = configData.GetField("reverseStartFinish", "Value").ToUpperInvariant();
			string sounds = configData.GetField("PlaySounds", "Value").ToUpperInvariant();
			string starthold = configData.GetField("StartHoldoffMS", "Value").ToUpperInvariant();
            string stophold = configData.GetField("StopHoldoffMS", "Value").ToUpperInvariant();
            string lapCount = configData.GetField("LapCount", "Value").ToUpperInvariant();

			returnTime.penalty = "";
            returnTime.time = "";
			
			noTime.penalty = "";
			noTime.time = "";
			

			string start = "A";
			string finish = "B";
			//showMsg("Timing event: " + type + "\n");
			if (type == "S")
			{
				ResetStart();
				showMsg("Reset start received\n");
				return noTime;
			}
			if (type == "R")
			{
				if (time != "000000")
				{
					return noTime;
				}
				ResetStop();
				showMsg("Reset stop received\n");
				return noTime;
			}

			if (UInt32.TryParse(starthold, out uint startHoldoffMS) == false)
			{
				startHoldoffMS = 0;
			}
			// Do we allow more than one stop trigger before calling it a true finish?
			if (UInt32.TryParse(lapCount, out uint lapCountInt) == false)
			{
				lapCountInt = 1;
			}
			
			if (UInt32.TryParse(stophold, out uint stopHoldoffMS) == false)
			{
				stopHoldoffMS = 0;
			}
			
			if ((reverse.Contains("Y") == true) || (isReversed == true))
			{
				start = "B";
				finish = "A";
			}

			if (sounds.Contains("Y") == true)
			{
				playSounds = true;
			}
			if (type == start)
			{
				// A start timer event
				// find the first empty start slot
				for (index = 0; index < _data.Count; index++)
				{
					var x = (Runtime)(_data[index]);
					if ((x._start_time == null))
					{
						// Ensure holdoff delay has elapsed since last start
						if ((index > 0) && (startHoldoffMS > 0))
						{
                            if (UInt32.TryParse(time, out uint cur_time) == false)
                            {
                                showMsg("Received invalid start time string: " + time + "\n");
                                return noTime;
                            }
                            var prev = (Runtime)(_data[index-1]);
							if (prev._start_time != null)
							{
                                if (UInt32.TryParse(prev._start_time, out uint prev_start_time) == true)
                                {
                                    uint delaytime = unchecked(cur_time - prev_start_time);
                                    // handle rollover at 999999
                                    if (prev_start_time > cur_time)
                                    {
                                        delaytime = unchecked(1000000 - prev_start_time + cur_time);
                                    }
                                    if (delaytime < startHoldoffMS)
                                    {
                                        showMsg("Received start trigger before the holdoff period of: " + starthold + "\n");
                                        return noTime;
                                    }
                                }
                            }
						}
						x._start_time = time;
						x._run_time = "Running";
						x._datetime = DateTime.Now.ToString("dd-hh:mm:ss");
						if (playSounds == true)
						{
							SystemSounds.Beep.Play();
						}
						return noTime;
					}
				}
				if (found == false)
				{
					if (playSounds == true)
					{
						SystemSounds.Hand.Play();
					}
					showMsg("No free slot found!\n");
					return noTime;
				}
			}
			if (type == finish)
			{
				
				// Stop timer event
				// find the appropriate start slot
				found = false;
				for (index = 0; index < _data.Count; index++)
				{
					var x = (Runtime)(_data[index]);
					if ((x._start_time != null) && (x._stop_time == null))
					{
                        if (UInt32.TryParse(time, out uint stop_time) == false)
                        {
                            showMsg("Received invalid stop time string: " + time + "\n");
                        }

                        // Ensure holdoff delay has elapsed since last stop
                        if ((index > 0) && (stopHoldoffMS > 0))
						{
							var prev = (Runtime)(_data[index-1]);
							if (prev._stop_time != null)
							{
                                if (UInt32.TryParse(prev._stop_time, out uint prev_stop_time) == true)
                                {
                                    uint delaytime = unchecked(stop_time - prev_stop_time);
                                    // handle rollover at 999999
                                    if (prev_stop_time > stop_time)
                                    {
                                        delaytime = unchecked(1000000 - prev_stop_time + stop_time);
                                    }
                                    if (delaytime < stopHoldoffMS)
                                    {
                                        showMsg("Received stop time trigger before the holdoff period of: " + stophold + "\n");
                                        return noTime;
                                    }
                                }
                            }
						}
						// Check if we have finished the required number of laps
						if (lapCountInt > 1)
						{
							x._lap_count += 1;
							if (x._lap_count < lapCountInt)
							{
								return noTime;
							}
						}
						
						x._stop_time = time;        // valid -- put it into data record

                        if (UInt32.TryParse(x._start_time, out uint start_time) == false)
                        {
                            showMsg("Invalid start time string: " + x._start_time + "\n");
                            x._start_time = "000000";
                            x._penalty = "RRN";
                            start_time = 0;
                        }
                        uint rtime = unchecked(stop_time - start_time);
						// handle rollover at 999999
						if (start_time > stop_time)
						{
							rtime = unchecked(1000000 - start_time + stop_time);
						}
						string stime = (rtime / 1000).ToString();
						string frac = (rtime%1000).ToString();
						while (frac.Length < 3)
						{
							frac = "0" + frac;
						}
						stime += "." + frac;
						x._run_time = stime;
						x._set = RunDay;
						x._datetime_stop = DateTime.Now.ToString("dd-hh:mm:ss");
						_data[index] = x;
						WriteData();
						if (playSounds == true)
						{
							SystemSounds.Hand.Play();
						}
						returnTime.time = stime;
						returnTime.penalty = x._penalty;
						return returnTime;
					}
				}
				if (found == false)
				{
					if (playSounds == true)
					{
						SystemSounds.Hand.Play();
					}
					showMsg("Finish triggered with no matching start trigger\n");
					return noTime;
				}
			}
			
			if (type == "C")
			{
				// split time event
				found = false;
				for (index = 0; index < _data.Count; index++)
				{
					var x = (Runtime)(_data[index]);
					if ((x._start_time != null) && (x._split_time == null))
					{
						x._split_time = time;
						_data[index] = x;
						if (playSounds == true)
						{
							SystemSounds.Beep.Play();
						}
						returnTime.time = time;
						returnTime.penalty = x._penalty;
						return returnTime;
					}
				}
				if (found == false)
				{
					if (playSounds == true)
					{
						SystemSounds.Hand.Play();
					}
					showMsg("Split triggered with no matching start trigger\n");
					return noTime;
				}
			}
			showMsg("Unknown type of timing message: " + type + "\n");
			return noTime;
		}
		// -------------------------------------------------------------------
		// Clean up the _penalty value and update run numbers if required
		public void Fix_penalty(int row)
		{
			if ((row < 0)  || (row >= _data.Count))
			{
				return;
			}
			var x = (Runtime)(_data[row]);
			x._set = RunDay;
			if (x.Penalty == null)
			{
				UpdateRunNumbers();
				return;
			}
			x.Penalty = x.Penalty.Trim();
			if (string.IsNullOrEmpty(x.Penalty))
			{
				x.Penalty = null;
				UpdateRunNumbers();
				return;
			}
			
			x.Penalty = x.Penalty.ToLowerInvariant();
			// Do a few shortcuts
			if ((x.Penalty.StartsWith("x")) || (x.Penalty.StartsWith("d")))
			{
				x.Penalty = "DNF";
			}
			else if (x.Penalty.StartsWith("r"))
			{
				x.Penalty = "RRN";
			}
			else
			{
                if (int.TryParse(x.Penalty, out int result) == false)
                {
                    x.Penalty = null;
                }
                if (result == 0)
				{
					x.Penalty = null;
				}
			}
			UpdateRunNumbers();
			if (x._stop_time != null)
			{
				WriteData();
			}
		}
		// -------------------------------------------------------------------
		// Car number changed -- update data in child fields
		public void Carchange(int row, bool suppressWrite)
		{
			if ((row < 0)  || (row >= _data.Count))
			{
				return;
			}
			var x = (Runtime)(_data[row]);
			if (string.IsNullOrEmpty(x._car_number))
			{
				x._car_number = null;
				x._run_number = null;
				x._car_class = null;
				x._driver = null;
				x._car_description = null;
				x._car_colour = null;
				x._set = RunDay;
				
				UpdateRunNumbers();
				if ((x._stop_time != null) && (suppressWrite == false))
				{
					WriteData();
				}
				return;
			}
			
			x._car_class = driverData.GetField(x.Car, "Class");
			x._driver = driverData.GetField(x.Car,"First Name") + " " + driverData.GetField(x.Car,"Last Name");
			x._car_description = driverData.GetField(x.Car,"Car Model");
			x._car_colour = driverData.GetField(x.Car,"Car Color");
			UpdateRunNumbers();
			if ((suppressWrite == false) && (x._stop_time != null)) WriteData();
		}
		// -------------------------------------------------------------------
		// Update run numbers for all cars
		public void UpdateRunNumbers()
		{
			var runs = new Dictionary<string, int>();
			for (int i = 0; i < _data.Count; i++)
			{
				var r = (Runtime)(_data[i]);
				if ((string.IsNullOrEmpty(r._car_number) == false) && (r.Penalty != "RRN"))
				{
					if (runs.ContainsKey(r._car_number) == false)
					{
						runs.Add(r._car_number, 1);
						r._run_number = "1";
					}
					else
					{
						int run = runs[r._car_number];
						run += 1;
						r._run_number = run.ToString();;
						runs[r._car_number] += 1;
					}
				}
			}
		}
		// -------------------------------------------------------------------
		// Fake a start trigger
		// Always mark the penalty as RRN because the time will be incorrect
		public void TriggerStart()
		{
			int index;
			bool found = false;
			
			// find the first empty start slot
			for (index = 0; index < _data.Count; index++)
			{
				var x = (Runtime)(_data[index]);
				if ((x._start_time == null))
				{
					found = true;
					x._start_time = "000000";
					x._run_time = "Running";
					x._datetime = DateTime.Now.ToString("dd-hh:mm:ss");
					x._penalty = "RRN";
					break;
				}
			}
			if (found == false)
			{
				showMsg("No free slot found!\n");
				return;
			}
		}
		// -------------------------------------------------------------------
		// Fake a stop trigger
		// Always mark the penalty as RRN because the time will be incorrect
		public void TriggerStop()
		{
			bool found = false;
			for (int index = 0; index < _data.Count; index++)
			{
				var x = (Runtime)(_data[index]);
				if ((x._start_time != null) && (x._stop_time == null))
				{
					found = true;
					x._stop_time = "999999";
					x._run_time = "999.999";
					x._datetime_stop = DateTime.Now.ToString("dd-hh:mm:ss");
					x._penalty = "RRN";
					x._set = RunDay;
					x._lap_count = 0;
					UpdateRunNumbers();
					WriteData();
					break;
				}
			}
			if (found == false)
			{
				showMsg("No running timer found!\n");
				return;
			}
		}
		// -------------------------------------------------------------------
		// Reset (cancel) last start timer
		public void ResetStart()
		{
			for (int i = _data.Count-1; i >= 0; i--)
			{
				var x = (Runtime)(_data[i]);
				if ((x._start_time != null) && (x._stop_time == null))
				{
					x._run_time = "";
					x._start_time = null;
					x._penalty = "";
					x._lap_count = 0;
					_data[i] = x;
					break;
				}
			}
		}
		// -------------------------------------------------------------------
		// Reset (cancel) last stop event
		public void ResetStop()
		{
			for (int i = _data.Count-1; i >= 0; i--)
			{
				var x = (Runtime)(_data[i]);
				if ((x._start_time != null) && (x._stop_time != null))
				{
					x._stop_time = null;
					x._run_time = "Running";
					if (x._lap_count > 0)
					{
						x._lap_count -= 1;
					}
					_data[i] = x;
					WriteData();
					break;
				}
			}
		}
		// -------------------------------------------------------------------
		// Reset (cancel) any running timers
		public void CancelTimers()
		{
			for (int i = _data.Count-1; i >= 0; i--)
			{
				var x = (Runtime)(_data[i]);
				if ((x._start_time != null) && (x._stop_time != null))
				{
					break;
				}
				if ((x._start_time != null) && (x._stop_time == null))
				{
					x._run_time = "";
					x._start_time = null;
					x._lap_count = 0;
					x._penalty = "";
					//_data[i] = x;
					// writeData();		//Not important to save data here
				}
			}
			
		}
	}
}