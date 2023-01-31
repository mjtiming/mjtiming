/*
 * 
 * Created by Murray Peterson
 * 
 */

// reverted back to text instead of html

// ASN trophy rules:
// 1-3 competitors = 1 trophy
// 4-6 competitors = 2 trophies
// 7-9 competitors = 3 trophies
// 1 trophy for every four additional competitors
// Basically, 3,3,3,4,4,4,4,...
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;


// disable CompareOfFloatsByEqualityOperator
namespace RaceBeam
{
	class Champ
	{
		// ---------------------------------------------------------------------------
		// All the scores for a given event
		public class EventScore
		{
			public string 	eventName = "";
			public double 	RAWScore;
			public double 	rookieRAWScore;
			public double 	PAXScore;
			public double 	rookiePAXScore;
			public double	groupScore;
			public double	groupRookieScore;
			public int		coneCount;
		}
		// A group and the event scores withion that group
		public class GroupScore
		{
			public string	groupName = "";
			public double totalPAX;
			public double totalRookiePAX;
			public Dictionary<string,EventScore> eventScores = new Dictionary<string,EventScore>();
		}

		// Info about a single driver (we use first name + last name as key) and overall raw/pax scores
		class DriverData
		{
			public string number;
			public string driver;	// first name + last name
			public string firstName;
			public string lastName;
			public string member;
			public string rookie;
			public double totalPAX;
			public double totalRookiePAX;
			public double totalRAW;
			public double totalRookieRAW;
			public int    totalCones;
			public int    totalEvents;
			// dictionary of overall pax and raw scores for each attended event
			public Dictionary<string,EventScore> overallScores = new Dictionary<string,EventScore>();
			// dictionary of groups in which this driver competed.  Each group contains a list of events where that group was used
			public Dictionary<string,GroupScore> groupScores  = new Dictionary<string,GroupScore>();
		}
		public class PaxInfo
		{
			public string carClass;
			public string pax;
			public string description;
			public string group;
			public string displayOrder;
		}
		// ---------------------------------------------------------------------------
		const string separator = "--------------------------------------------------------------------------------\r\n";
		static string eventFolder = "";
		static string classfile = "";
		static string title = "";
		static int numDays = 8;
		static int numRookieDays = 7;
		static bool doRookie = true;
		static bool membersOnly = false;
		static bool doAttendance = false;
		static readonly Dictionary<string,DriverData> scores = new Dictionary<string,DriverData>();
		static public List<string> eventNames = new List<string>();
		static readonly CSVData classData = new CSVData();
		static readonly CSVData configData = new CSVData();
		// ---------------------------------------------------------------------------
		public static void Usage()
		{
			Console.WriteLine("Usage: overalls -title <title string> -norookie -membersonly -attendance -best <num> -rookiebest <num> -path <path to event data folder> -classfile <path to class.csv file>");
			Environment.Exit(0);
		}
		// ---------------------------------------------------------------------------
		public static void Main(string[] args)
		{
			// parse command line arguments
			for (int i = 0; i < args.Length; i++)
			{
				if ((args[i] == "-h") | (args[i] == "-help") | (args[i] == "-?"))
				{
					Usage();
				}
				else if (args[i] == "-path")
				{
					i += 1;
					eventFolder = args[i];
				}
				else if (args[i] == "-classfile")
				{
					i += 1;
					classfile = args[i];
				}
				else if (args[i] == "-best")
				{
					i += 1;
					if (int.TryParse(args[i], out numDays) == false)
					{
						Usage();
					}
				}
				else if (args[i] == "-rookiebest")
				{
					i += 1;
					if (int.TryParse(args[i], out numRookieDays) == false)
					{
						Usage();
					}
				}
				else if (args[i] == "-title")
				{
					i += 1;
					title = args[i];
					Console.WriteLine(title);
				}
				else if (args[i] == "-norookie")
				{
					doRookie = false;		// no rookie scores
				}
				else if (args[i] == "-membersonly")
				{
					membersOnly = true;		// only display members
				}
				else if (args[i] == "-attendance")
				{
					doAttendance = true;
				}
				else
				{
					Usage();
				}
			}
			if (eventFolder == "")
			{
				Usage();
			}
			
			string configFolder = Process.GetCurrentProcess().MainModule.FileName;
			configFolder = Path.GetDirectoryName(configFolder);
			string configFilename =  configFolder + "\\..\\config\\configData.csv";
			string err = configData.LoadData(configFilename,',',"Parameter");
			if (err != "")
			{
				Console.Error.WriteLine("Unable to load config file: " + err);
				Usage();
			}
			
			string classDataFile = configData.GetField("classDataFile", "Value");
			if (classDataFile == "")
			{
				Console.Error.WriteLine("Class data file not defined in config file");
			}
			if (classfile != "")
			{
				classDataFile = classfile;
			}
			
			// Read in class data file
			err = classData.LoadData(classDataFile,',',"Class");
			if (err != "")
			{
				Console.Error.WriteLine(err);
			}
			
			ReadData();
			eventNames.Sort(delegate (string ev1, string ev2)
			                {
			                	string ev1a = Regex.Replace(ev1, "_", "");
			                	string ev2a = Regex.Replace(ev2, "_", "");
			                	return ev1a.CompareTo(ev2a);
			                }
			               );
			
			// raw data, everyone
			CalcData(false,false);
			PrintData(false,false);

			// raw data, rookie
			if (doRookie == true)
			{
				
				CalcData(false,true);
				Console.WriteLine("");
				PrintData(false,true);
			}
			
			// pax data, everyone
			Console.WriteLine("");
			CalcData(true,false);
			Console.WriteLine("");
			PrintData(true,false);
			
			// pax data, rookie
			if (doRookie == true)
			{
				CalcData(true,true);
				Console.WriteLine("");
				PrintData(true,true);
			}
			Console.WriteLine("");
			// class scores
			CalcData(true,false);
			Console.WriteLine(ClassScores(false)); // everyone
			if (doRookie == true)
			{
				CalcData(true,true);
				Console.WriteLine(ClassScores(true)); // rookie
			}
			Console.WriteLine("");

			PrintCones();
			if (doAttendance)
			{
				Attendance();
			}
			
		}
		// ---------------------------------------------------------------------------
		// print out cone counts
		public static void PrintCones()
		{
			var myList = new List<KeyValuePair<string, DriverData>>(scores);
			// Sort drivers by cone count
			myList.Sort(delegate(KeyValuePair<string, DriverData> firstPair,
			                     KeyValuePair<string, DriverData> nextPair)
			            {
			            	return nextPair.Value.totalCones.CompareTo(firstPair.Value.totalCones);
			            }
			           );
			
			// print header line
			Console.WriteLine(separator + "Overall cone count");
			Console.Write("Pos Driver        ");
			Console.WriteLine("    Total cones");
			
			// Now print out drivers, their event scores and their total
			int position = 1;
			int totCones = 0;
			foreach (KeyValuePair<string, DriverData> driver in myList)
			{
				if (driver.Value.totalCones <= 0)
				{
					break;
				}
				if ((membersOnly == true) && (driver.Value.member.ToUpperInvariant().Contains("Y") == false))
				{
					continue;
				}
				string line = String.Format("{0,3} {1,-14}",
				                            position++,
				                            driver.Value.driver
				                           );
				Console.Write(line);
				line = String.Format("  {0,10:#0}", driver.Value.totalCones);
				Console.WriteLine(line);
			}
			Console.WriteLine("Total cones: " + totCones);
		}
		// ---------------------------------------------------------------------------
		// print out attendance stats
		public static void Attendance()
		{
			var myList = new List<KeyValuePair<string, DriverData>>(scores);
			var attend = new SortedDictionary<int, int>();
			var rookieattend = new SortedDictionary<int, int>();
			
			// print header line
			Console.WriteLine(separator + "Attendance");

			for (int i = 1; i <= eventNames.Count; i++)
			{
				attend.Add(i,0);
				rookieattend.Add(i,0);
			}

			// Go through driver list and add his events to the dict
			foreach (KeyValuePair<string, DriverData> driver in myList)
			{
				attend[driver.Value.totalEvents] += 1;
				if (driver.Value.rookie.Contains("Y"))
				{
					rookieattend[driver.Value.totalEvents] += 1;
				}
			}

			// Print out results
			Console.WriteLine(separator);
			Console.WriteLine("{0,8} {1,8}", "Events", "Drivers");
			int totalDrivers = 0;
			foreach(int n in attend.Keys)
			{
				string line = String.Format("{0,8} {1,8}\r\n",
				                            n,
				                            attend[n]
				                           );
				totalDrivers += attend[n];
				Console.Write(line);
			}
			Console.WriteLine("Total drivers: " + totalDrivers.ToString());


			int totalRookieDrivers = 0;
			Console.WriteLine("\r\nRookie-only:");
			Console.WriteLine("{0,8} {1,8}", "Events", "Drivers");
			foreach(int n in rookieattend.Keys)
			{
				string line = String.Format("{0,8} {1,8}\r\n",
				                            n,
				                            rookieattend[n]
				                           );
				totalRookieDrivers += rookieattend[n];
				Console.Write(line);
			}
			Console.WriteLine("Total rookie drivers: " + totalRookieDrivers.ToString());
		}
		// ---------------------------------------------------------------------------
		public static void PrintData(bool isPAX, bool isRookie)
		{
			var myList = new List<KeyValuePair<string, DriverData>>(scores);
			
			// Sort drivers by PAX or RAW
			myList.Sort(delegate(KeyValuePair<string, DriverData> firstPair,
			                     KeyValuePair<string, DriverData> nextPair)
			            {
			            	if (isPAX == true)
			            	{
			            		if (isRookie == true)
			            		{
			            			return nextPair.Value.totalRookiePAX.CompareTo(firstPair.Value.totalRookiePAX);
			            		}
			            		else
			            		{
			            			return nextPair.Value.totalPAX.CompareTo(firstPair.Value.totalPAX);
			            		}
			            	}
			            	else
			            	{
			            		if (isRookie == true)
			            		{
			            			return nextPair.Value.totalRookieRAW.CompareTo(firstPair.Value.totalRookieRAW);
			            		}
			            		else
			            		{
			            			return nextPair.Value.totalRAW.CompareTo(firstPair.Value.totalRAW);
			            		}
			            	}
			            }
			           );
			
			// print header line
			Console.Write(separator);
			if (isPAX == true)
			{
				if (isRookie == true)
				{
					Console.WriteLine("Rookie-only PAX scores");
				}
				else
				{
					Console.WriteLine("Overall PAX scores");
				}
			}
			else
			{
				if (isRookie == true)
				{
					Console.WriteLine("Rookie-only RAW scores");
				}
				else
				{
					Console.WriteLine("Overall RAW scores");
				}
			}
			Console.Write("Pos Driver        ");

			foreach (string name in eventNames)
			{
				Console.Write("{0,10} ", name);
			}
			if (isRookie == true)
				Console.WriteLine("    Best {0}", numRookieDays.ToString());
			else
				Console.WriteLine("    Best {0}", numDays.ToString());
			
			// Now print out drivers, their event scores and their total
			int position = 1;
			foreach (KeyValuePair<string, DriverData> driver in myList)
			{
				string rookie = driver.Value.rookie.ToUpperInvariant();
				if ((isRookie == true) && (rookie.Contains("Y") == false))
				{
					continue;
				}
				// don't print out people with 0 scores (say 1 event with a DNS)
				if (isPAX == true)
				{
					if ((isRookie == true) && (driver.Value.totalRookiePAX <= 0.0))
					{
						break;
					}
					else if (driver.Value.totalPAX <= 0.0)
					{
						break;
					}
				}
				else
				{
					if (driver.Value.totalRAW <= 0.0)
					{
						break;
					}
				}
				string member = driver.Value.member.ToUpperInvariant();
				if ((membersOnly == true) && (member.Contains("Y") == false))
				{
					continue;
				}
				string line = String.Format("{0,3} {1,-13}",
				                            position++,
				                            driver.Value.driver
				                           );
				Console.Write(line);
				
				foreach (string eventName in eventNames)
				{
					if (driver.Value.overallScores.ContainsKey(eventName) == false)
					{
						// never attended
						line = String.Format(" {0,10:#0.000}", 0.00);
					}
					else
					{
						if (isPAX == true)
						{
							if (isRookie == true)
							{
								line = String.Format(" {0,10:#0.000}", driver.Value.overallScores[eventName].rookiePAXScore);
							}
							else
							{
								line = String.Format(" {0,10:#0.000}", driver.Value.overallScores[eventName].PAXScore);
							}
						}
						else
						{
							if (isRookie == true)
							{
								line = String.Format(" {0,10:#0.000}", driver.Value.overallScores[eventName].rookieRAWScore);
							}
							else
							{
								line = String.Format(" {0,10:#0.000}", driver.Value.overallScores[eventName].RAWScore);
							}
						}
					}
					Console.Write(line);
				}
				if (isPAX == true)
				{
					if (isRookie == true)
					{
						line = String.Format("  {0,10:#0.000}", driver.Value.totalRookiePAX);
					}
					else
					{
						line = String.Format("  {0,10:#0.000}", driver.Value.totalPAX);
					}
				}
				else
				{
					if (isRookie == true)
					{
						line = String.Format("  {0,10:#0.000}", driver.Value.totalRookieRAW);
					}
					else
					{
						line = String.Format("  {0,10:#0.000}", driver.Value.totalRAW);
					}
				}
				
				Console.WriteLine(line);
			}
		}
		// ---------------------------------------------------------------------------
		public static void CalcData(bool isPAX, bool isRookie)
		{
			// First go through and calculate best total scores
			foreach (string driverName in scores.Keys)
			{
				DriverData driver = scores[driverName];
				driver.totalPAX = 0.0;
				driver.totalRookiePAX = 0.0;
				driver.totalRAW = 0.0;
				driver.totalRookieRAW = 0.0;

				// Sort by PAX score, descending order
				var myList = new List<EventScore>(driver.overallScores.Values);
				
				// Sort drivers by PAX or RAW
				myList.Sort(delegate(EventScore first,
				                     EventScore next)
				            {
				            	if (isPAX == true)
				            	{
				            		if (isRookie == true)
				            		{
				            			return next.rookiePAXScore.CompareTo(first.rookiePAXScore);
				            		}
				            		else
				            		{
				            			return next.PAXScore.CompareTo(first.PAXScore);
				            		}
				            	}
				            	else
				            	{
				            		if (isRookie == true)
				            		{
				            			return next.rookieRAWScore.CompareTo(first.rookieRAWScore);
				            		}
				            		else
				            		{
				            			return next.RAWScore.CompareTo(first.RAWScore);
				            		}
				            	}
				            }
				           );
				
				// Add up scores until we hit the "best N" count
				int bestCount = numDays;
				if (isRookie == true)
				{
					bestCount = numRookieDays;
				}
				foreach (EventScore evScore in myList)
				{
					driver.totalPAX += evScore.PAXScore;
					driver.totalRAW += evScore.RAWScore;
					driver.totalRookiePAX += evScore.rookiePAXScore;
					driver.totalRookieRAW += evScore.rookieRAWScore;
					bestCount -= 1;
					if (bestCount <= 0)
					{
						break;
					}
				}
				
				// Deal with Group scores
				foreach (string grp in driver.groupScores.Keys)
				{
					bestCount = numDays;
					if (isRookie == true)
					{
						bestCount = numRookieDays;
					}
					// convert event dict into a list
					myList = new List<EventScore>(driver.groupScores[grp].eventScores.Values);
					// sort in descending order by group score
					myList.Sort(delegate(EventScore first,
					                     EventScore next)
					            {
					            	if (isRookie == true)
					            	{
					            		return next.groupRookieScore.CompareTo(first.groupRookieScore);
					            	}
					            	else
					            	{
					            		return next.groupScore.CompareTo(first.groupScore);
					            	}
					            }
					           );

					if (isRookie == true)
					{
						bestCount = numRookieDays;
						driver.groupScores[grp].totalRookiePAX = 0.0;
					}
					else
					{
						bestCount = numDays;
						driver.groupScores[grp].totalPAX = 0.0;
					}
					foreach (EventScore evScore in myList)
					{
						if (isRookie == true)
						{
							driver.groupScores[grp].totalRookiePAX += evScore.groupRookieScore;
						}
						else
						{
							driver.groupScores[grp].totalPAX += evScore.groupScore;
						}

						bestCount -= 1;
						if (bestCount <= 0)
						{
							break;
						}
					}
				}
			}
		}
		// ---------------------------------------------------------------------------
		public static string ReadData()
		{
			string[] filePaths = Directory.GetFiles(eventFolder, "*_CSVData.csv");
			// Read in timing data
			foreach (string scorefile in filePaths)
			{
				//Console.Error.WriteLine("Adding data for " + scorefile);
				
				// parse out the event name from something like c:\blah\12_02_23_CSVData.csv
				//Regex reg = new Regex(@".*(event[0-9]+).*");
				var reg = new Regex(@".*(\d\d_\d\d_\d\d.*)_CSVData.csv");
				Match m;
				m = reg.Match(scorefile);
				if (m.Success == false)
				{
					Console.WriteLine("Unable to parse:" + scorefile);
					return "";
				}
				GroupCollection g = m.Groups;
				string eventName = g[1].Value;
				if (eventNames.Contains(eventName) == false)
				{
					eventNames.Add(eventName);
				}
				
				var ev = new CSVData();
				ev.LoadData(scorefile,',',"Car#");
				foreach (string carnum in ev.getKeys())
				{
					DriverData driver;
					// Each event is keyed by car number, but we will key the overalls by the driver's full name
					string driverName = ev.GetField(carnum, "First Name") + "_" + ev.GetField(carnum, "Last Name");
					// Check for registered but didn't compete
					string px = ev.GetField(carnum, "PAX Score");
                    double.TryParse(px, out double pxScore);
                    if (pxScore == 0.0)
					{
						continue;
					}
					if (scores.ContainsKey(driverName) == false)
					{
                        driver = new DriverData
                        {
                            number = carnum,
                            driver = ev.GetField(carnum, "Driver"),
                            firstName = ev.GetField(carnum, "First Name"),
                            lastName = ev.GetField(carnum, "Last Name"),
                            member = "N",
                            totalEvents = 1
                        };
                        if (ev.GetField(carnum, "Mbr").ToUpperInvariant().Contains("Y") == true)
						{
							driver.member = "Y";
						}
						driver.rookie = "N";
						string rk = ev.GetField(carnum, "Rky").ToUpperInvariant();
						if ((rk.Contains("Y") == true) || (rk.Contains("TRUE")))
						{
							driver.rookie = "Y";
						}
						scores.Add(driverName, driver);
					}
					else
					{
						// Complain about mis-spelled driver names
						// If the first letter of the first name is different, assume it's just a re-used number
						// by two completely different people
						driver = scores[driverName];
						driver.totalEvents += 1;
						string mem = "N";
						if (ev.GetField(carnum, "Mbr").ToUpperInvariant().Contains("Y") == true)
						{
							mem = "Y";
						}
						if (driver.member != mem)
						{
							TextWriter errorWriter = Console.Error;
							errorWriter.WriteLine("Membership change for " + driverName +
							                      " in event " + scorefile +
							                      "( " + driver.member + " to " + mem + " )");
						}
						string rook = "N";
						if (ev.GetField(carnum, "Rky").ToUpperInvariant().Contains("Y") == true)
						{
							rook = "Y";
						}
						if (driver.rookie != rook)
						{
							TextWriter errorWriter = Console.Error;
							errorWriter.WriteLine("Rookie change for " + driverName +
							                      " in event " + scorefile +
							                      "( " + driver.rookie + " to " + rook + " )");
						}
					}
					driver = scores[driverName];
					// Copy overall scores into driver's data
					var evScores = new EventScore();
					string pax = ev.GetField(carnum, "PAX Score");
					double.TryParse(pax, out evScores.PAXScore);
					string rookiepax = ev.GetField(carnum, "Rookie PAX Score");
					double.TryParse(rookiepax, out evScores.rookiePAXScore);
					string raw = ev.GetField(carnum, "RAW Score");
					double.TryParse(raw, out evScores.RAWScore);
					string rookieraw = ev.GetField(carnum, "Rookie RAW Score");
					double.TryParse(rookieraw, out evScores.rookieRAWScore);
					string cones = ev.GetField(carnum, "Cones");
					int.TryParse(cones, out evScores.coneCount);
					
					if (driver.overallScores.ContainsKey(eventName) == false)
					{
						driver.overallScores.Add(eventName, evScores);
					}
					// Add to total cone count for this driver
					driver.totalCones += evScores.coneCount;

					// multiple groups, so go through and parse each one
					for (int grpnum = 1; grpnum < 1000; grpnum++)
					{
						string hdr = String.Format("Xgroup-{0}:Name", grpnum);
						
						string grpname = ev.GetField(carnum, hdr);
						if (String.IsNullOrEmpty(grpname))
						{
							break;
						}
						// Get groupScore ptr (or create one if this is the first occurrence)
						GroupScore grpscore;
						if (driver.groupScores.ContainsKey(grpname) == false)
						{
                            grpscore = new GroupScore
                            {
                                groupName = grpname
                            };
                            driver.groupScores.Add(grpname, grpscore);
						}
						else
						{
							grpscore = driver.groupScores[grpname];
						}
						// Now create an eventScore for this event
						var scoresThisEvent = new EventScore();
						hdr = String.Format("Xgroup-{0}:Score", grpnum);
						string tmpstr = ev.GetField(carnum, hdr);
                        double.TryParse(tmpstr, out double tmp);
                        scoresThisEvent.groupScore = tmp;
						
						hdr = String.Format("Xgroup-{0}:Rookie Score", grpnum);
						tmpstr = ev.GetField(carnum, hdr);
						double.TryParse(tmpstr, out tmp);
						scoresThisEvent.groupRookieScore = tmp;

						grpscore.eventScores.Add(eventName, scoresThisEvent);
						
					}
				}
			}
			
			return "";
		}
		// ---------------------------------------------------------------------------
		public static string ClassScores(bool isRookie)
		{
			string results = separator;
			
			if (isRookie == true)
			{
				results += "Rookie-only group/class scores\r\n";
			}
			else
			{
				results += "Overall group/class scores\r\n";
			}

			// First build a sorted class list
			// Sort order is given in the csv file
			var sortedClassList = new SortedDictionary<int,PaxInfo>();
			List<string> classList = classData.getKeys();
			foreach (string className in classList)
			{
                var p = new PaxInfo
                {
                    carClass = className,
                    pax = classData.GetField(className, "PAX"),
                    description = classData.GetField(className, "Description"),
                    group = classData.GetField(className, "Group"),
                    displayOrder = classData.GetField(className, "Display Order")
                };
                if (int.TryParse(p.displayOrder, out int orderVal) == false)
                {
                    results += String.Format("Display order value for class " + p.carClass + " is not a valid integer\r\n");
                }
                else
                {
                    try
                    {
                        sortedClassList.Add(orderVal, p);
                    }
                    catch
                    {
                        results += String.Format("Multiple entries for class " + p.carClass + "\r\n");
                    }
                }
            }

			// Each driver has a list of event scores, with each event possibly in a different class
			// For each class, we need to make a new driver list, with all drivers that ever ran in that class
			// If they didn't run in that class, then the event score (raw) is 0.0
			// Then sort by score totals for each driver
			
			foreach (KeyValuePair<int, PaxInfo> classInfo in sortedClassList)
			{
				PaxInfo curClass = classInfo.Value;
				int rank = 1;

				// make a new driver list, with all drivers that ever ran in this class
				var drvList = new List<DriverData>();
				foreach (string driverName in scores.Keys)
				{
					DriverData driver = scores[driverName];
					if ((isRookie == true) && (driver.rookie.ToUpperInvariant().Contains("Y") == false))
					{
						continue;
					}
					if (driver.groupScores.ContainsKey(curClass.carClass))
					{
						drvList.Add(driver);
					}
				}
				// skip group if nobody competed there
				if (drvList.Count == 0)
				{
					continue;
				}

				// Now sort drivers by their overall score in this group
				drvList.Sort(delegate(DriverData first, DriverData next)
				             {
				             	if (isRookie == true)
				             	{
				             		return next.groupScores[curClass.carClass].totalRookiePAX.CompareTo(first.groupScores[curClass.carClass].totalRookiePAX);
				             	}
				             	else
				             	{
				             		return next.groupScores[curClass.carClass].totalPAX.CompareTo(first.groupScores[curClass.carClass].totalPAX);
				             	}
				             }
				            );

				foreach (DriverData driver in drvList)
				{
					if ((isRookie == true) && (driver.rookie.ToUpperInvariant().Contains("Y") == false))
					{
						continue;
					}
					if ((membersOnly == true) && (driver.member.ToUpperInvariant().Contains("Y") == false))
					{
						continue;
					}
					
					if (rank == 1)
					{
						// OK -- print header
						results += String.Format("\r\n");
						results += String.Format("Group/Class: " + curClass.carClass + " (" + curClass.description + ")\r\n");
						results += String.Format("{0,4} {1,-16}",
						                         "Rank","Driver");
						foreach (string name in eventNames)
						{
							results += String.Format(" {0,10}", name);
						}
						results += "       Best " + numDays.ToString() + "\r\n";
					}
					
					string driverName = driver.firstName + " " + driver.lastName.Substring(0,1);
					if (configData.GetField("ShowLastName","Value").Contains("Y"))
					{
						driverName = driver.firstName + " " + driver.lastName;
					}
					
					results += String.Format("{0,3} {1,-16}",
					                         rank++,
					                         driverName
					                        );
					
					// Driver score dict has gaps -- not all events attended
					foreach (string eventName in eventNames)
					{
						if (driver.groupScores[curClass.carClass].eventScores.ContainsKey(eventName) == false)
						{
							results += String.Format(" {0,10:#0.000}", 0.00);
						}
						else
						{
							if (isRookie == true)
							{
								results += String.Format(" {0,10:#0.000}", driver.groupScores[curClass.carClass].eventScores[eventName].groupRookieScore);
							}
							else
							{
								results += String.Format(" {0,10:#0.000}", driver.groupScores[curClass.carClass].eventScores[eventName].groupScore);
							}
						}
					}
					// Now the best N event total
					if (isRookie == true)
					{
						results += String.Format(" {0,10:#0.000}", driver.groupScores[curClass.carClass].totalRookiePAX);
					}
					else
					{
						results += String.Format(" {0,10:#0.000}", driver.groupScores[curClass.carClass].totalPAX);
					}
					results += "\r\n";
				}
			}
			return results;
		}
		// ---------------------------------------------------------------------------
	}
}