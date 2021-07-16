/*
 * 
 * Created by Murray Peterson
 * 
 */
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
namespace RaceBeam
{
	class champ
	{
		// ---------------------------------------------------------------------------
		class scoreClass
		{
			public string eventName;
			public string carClass;
			public double RAWScore;
			public double PAXScore;
			public double RookiePAXScore;
			public double classScore;
			public int	  coneCount;
			public scoreClass()
			{
				eventName = "";
				carClass = "";
				RAWScore = 0.0;
				PAXScore = 0.0;
				RookiePAXScore = 0.0;
				classScore = 0.0;
				coneCount = 0;
			}
		}
		class driverDataClass
		{
			public string number;
			public string driver;
			public string firstName;
			public string lastName;
			public string carDescription;
			public string member;
			public string rookie;
			public string carClass;
			public string sponsor;
			public double totalPAX;
			public double totalRookiePAX;
			public double totalRAW;
			public double totalClass;
			public int    totalCones;
			public int	  classEventCount;
			public List<scoreClass> scores = new List<scoreClass>();

			public driverDataClass()
			{
				number = "";
				driver = "";
				firstName = "";
				lastName = "";
				carDescription = "";
				member = "";
				rookie = "";
				carClass = "";
				sponsor = "";
				totalCones = 0;
				totalPAX = 0.0;
				totalRAW = 0.0;
				totalClass = 0.0;
				totalRookiePAX = 0.0;
				classEventCount = 0;
			}
		}
		public class paxInfo
		{
			public string carClass;
			public string pax;
			public string description;
			public string group;
			public string displayOrder;
		}
		// ---------------------------------------------------------------------------
		static string eventFolder = "";
		static string title = "";
		static int numDays = 8;
		static int numRookieDays = 7;
		static Dictionary<string,driverDataClass> scores = new Dictionary<string,driverDataClass>();
		static public List<string> eventNames = new List<string>();
		static CSVData classData = new CSVData();
		// ---------------------------------------------------------------------------
		public static void usage()
		{
			Console.WriteLine("Usage: overalls -best <num> -rookiebest <num> -path <path to event data folder>");
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
					usage();
				}
				else if (args[i] == "-path")
				{
					i += 1;
					eventFolder = args[i];
				}
				else if (args[i] == "-best")
				{
					i += 1;
					if (int.TryParse(args[i], out numDays) == false)
					{
						usage();
					}
				}
				else if (args[i] == "-rookiebest")
				{
					i += 1;
					if (int.TryParse(args[i], out numRookieDays) == false)
					{
						usage();
					}
				}
				else if (args[i] == "-title")
				{
					i += 1;
					title = args[i];
				}
				else
				{
					usage();
				}
			}
			if (eventFolder == "")
			{
				usage();
			}
			CSVData configData = new CSVData();
			string configFolder = Process.GetCurrentProcess().MainModule.FileName;
			configFolder = Path.GetDirectoryName(configFolder);
			string configFilename =  configFolder + "\\configData.csv";
			string err = configData.loadData(configFilename,',');
			if (err != "")
			{
				Console.Error.WriteLine("Unable to load config file: " + err);
			}
			
			string classDataFile = configData.getField("classDataFile", "Value");
			if (classDataFile == "")
			{
				Console.Error.WriteLine("Class data file not defined in config file");
			}
			// Read in class data file
			
			err = classData.loadData(classDataFile,',');
			if (err != "")
			{
				Console.Error.WriteLine(err);
			}
			
			string results = "";
			if (title != "")
			{
				results = title + "\r\n";
			}
			readData();
			addEvents();
			eventNames.Sort(delegate (string ev1, string ev2)
			                {
			                	string ev1num = Regex.Match(ev1, @"\d+$").Value;
			                	int ev1val;
			                	Int32.TryParse(ev1num, out ev1val);
			                	string ev2num = Regex.Match(ev2, @"\d+$").Value;
			                	int ev2val;
			                	Int32.TryParse(ev2num, out ev2val);
			                	return ev1val.CompareTo(ev2val);
			                }
			               );

			// raw data, everyone
			calcData(false,false);
			printData(false,false);

			// pax data, everyone
			Console.WriteLine("");
			calcData(true,false);
			printData(true,false);

			// pax data, rookie
			Console.WriteLine("");
			calcData(true,true);
			printData(true,true);
			Console.WriteLine("");
			Console.WriteLine(classScores());
			Console.WriteLine("");
			printCones();
			
		}
		// ---------------------------------------------------------------------------
		// print out cone counts
		public static void printCones()
		{
			List<KeyValuePair<string, driverDataClass>> myList = new List<KeyValuePair<string, driverDataClass>>(scores);
			// Sort drivers by cone count
			myList.Sort(delegate(KeyValuePair<string, driverDataClass> firstPair,
			                     KeyValuePair<string, driverDataClass> nextPair)
			            {
			            	return nextPair.Value.totalCones.CompareTo(firstPair.Value.totalCones);
			            }
			           );

			// print header line
			Console.WriteLine("Overall cone count");
			Console.Write("Pos Car# Driver        ");
			//eventNames.Sort();
			foreach (string name in eventNames)
			{
				Console.Write("{0,9}", name);
			}
			Console.WriteLine("    Total cones");

			// Now print out drivers, their event scores and their total
			int position = 1;
			foreach (KeyValuePair<string, driverDataClass> driver in myList)
			{
				if (driver.Value.totalCones <= 0)
				{
					break;
				}
				string line = String.Format("{0,3} {1,4} {2,-14}",
				                            position++,
				                            driver.Value.number,
				                            driver.Value.driver
				                           );
				Console.Write(line);

				// Sort score list by event name
				driver.Value.scores.Sort(delegate(scoreClass first, scoreClass next)
				                         {
				                         	string ev1num = Regex.Match(first.eventName, @"\d+$").Value;
				                         	int ev1val;
				                         	Int32.TryParse(ev1num, out ev1val);
				                         	string ev2num = Regex.Match(next.eventName, @"\d+$").Value;
				                         	int ev2val;
				                         	Int32.TryParse(ev2num, out ev2val);
				                         	return ev1val.CompareTo(ev2val);
				                         }
				                        );

				foreach (scoreClass eventScores in driver.Value.scores)
				{
					line = String.Format(" {0,8:#0}", eventScores.coneCount);
					Console.Write(line);
				}
				line = String.Format("  {0,8:#0}", driver.Value.totalCones);
				Console.WriteLine(line);
			}
		}
		// ---------------------------------------------------------------------------
		public static void printData(bool isPAX, bool isRookie)
		{
			List<KeyValuePair<string, driverDataClass>> myList = new List<KeyValuePair<string, driverDataClass>>(scores);
			// Sort drivers by PAX or RAW
			myList.Sort(delegate(KeyValuePair<string, driverDataClass> firstPair,
			                     KeyValuePair<string, driverDataClass> nextPair)
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
			            		return nextPair.Value.totalRAW.CompareTo(firstPair.Value.totalRAW);
			            }
			           );

			// print header line
			if (isPAX == true)
			{
				Console.Write("Overall PAX scores ");
			}
			else
			{
				Console.Write("Overall RAW scores ");
			}
			if (isRookie == true)
			{
				Console.WriteLine("for rookies");
			}
			else
			{
				Console.WriteLine("for all");
			}
			
			Console.Write("Pos Car# Mbr Rky Class Driver        ");
			//eventNames.Sort();
			foreach (string name in eventNames)
			{
				Console.Write("{0,9}", name);
			}
			if (isRookie == true)
				Console.WriteLine("    Best {0}", numRookieDays.ToString());
			else
				Console.WriteLine("    Best {0}", numDays.ToString());

			// Now print out drivers, their event scores and their total
			int position = 1;
			foreach (KeyValuePair<string, driverDataClass> driver in myList)
			{
				string rookie = driver.Value.rookie.ToLowerInvariant();
				if ((isRookie == true) && (rookie.Contains("y") == false))
				{
					continue;
				}
				string line = String.Format("{0,3} {1,4} {2,3} {3,3} {4,5} {5,-14}",
				                            position++,
				                            driver.Value.number,
				                            driver.Value.member,
				                            driver.Value.rookie,
				                            driver.Value.carClass,
				                            driver.Value.driver
				                           );
				Console.Write(line);

				// Sort score list by event name
				driver.Value.scores.Sort(delegate(scoreClass first, scoreClass next)
				                         {
				                         	string ev1num = Regex.Match(first.eventName, @"\d+$").Value;
				                         	int ev1val;
				                         	Int32.TryParse(ev1num, out ev1val);
				                         	string ev2num = Regex.Match(next.eventName, @"\d+$").Value;
				                         	int ev2val;
				                         	Int32.TryParse(ev2num, out ev2val);
				                         	return ev1val.CompareTo(ev2val);
				                         }
				                        );

				foreach (scoreClass eventScores in driver.Value.scores)
				{
					if (isPAX == true)
					{
						if (isRookie == true)
						{
							line = String.Format(" {0,8:#0.000}", eventScores.RookiePAXScore);
						}
						else
						{
							line = String.Format(" {0,8:#0.000}", eventScores.PAXScore);
						}
					}
					else
						line = String.Format(" {0,8:#0.000}", eventScores.RAWScore);
					Console.Write(line);
				}
				if (isPAX == true)
				{
					if (isRookie == true)
					{
						line = String.Format("  {0,8:#0.000}", driver.Value.totalRookiePAX);
					}
					else
					{
						line = String.Format("  {0,8:#0.000}", driver.Value.totalPAX);
					}
				}
				else
					line = String.Format("  {0,8:#0.000}", driver.Value.totalRAW);
				Console.WriteLine(line);
			}
		}
		// ---------------------------------------------------------------------------
		public static void calcData(bool isPAX, bool isRookie)
		{
			// First go through and calculate best total scores
			foreach (string driverName in scores.Keys)
			{
				driverDataClass driver = scores[driverName];
				driver.totalPAX = 0.0;
				driver.totalRookiePAX = 0.0;
				driver.totalRAW = 0.0;
				driver.totalCones = 0;
				// Sort by PAX score, descending order
				driver.scores.Sort(delegate(scoreClass first, scoreClass next)
				                   {
				                   	if (isPAX == true)
				                   	{
				                   		if (isRookie == true)
				                   			return next.RookiePAXScore.CompareTo(first.RookiePAXScore);
				                   		else
				                   			return next.PAXScore.CompareTo(first.PAXScore);
				                   	}
				                   	else
				                   		return next.RAWScore.CompareTo(first.RAWScore);
				                   }
				                  );
				// Add up scores until we hit the "best N" count
				int bestCount = numDays;
				if (isRookie == true)
				{
					bestCount = numRookieDays;
				}
				foreach (scoreClass eventScores in driver.scores)
				{
					driver.totalPAX += eventScores.PAXScore;
					driver.totalRAW += eventScores.RAWScore;
					driver.totalRookiePAX += eventScores.RookiePAXScore;
					bestCount -= 1;
					if (bestCount <= 0)
					{
						break;
					}
				}
				// Add up cone counts from all events
				foreach (scoreClass eventScores in driver.scores)
				{
					driver.totalCones += eventScores.coneCount;
				}

			}
		}
		// ---------------------------------------------------------------------------
		// Now we need to go back through the data and add an event record for each driver
		// This way, the print/calc routine won't have nearly as much work to do
		public static void addEvents()
		{
			//eventNames.Sort();
			foreach (string driverName in scores.Keys)
			{
				driverDataClass driver = scores[driverName];
				driver.totalPAX = 0.0;
				driver.totalRookiePAX = 0.0;
				driver.totalRAW = 0.0;
				driver.totalCones = 0;
				foreach (string evname in eventNames)
				{
					bool found = false;
					foreach (scoreClass eventScores in driver.scores)
					{
						if (eventScores.eventName == evname)
						{
							found = true;
							break;
						}
					}
					if (found == false)
					{
						scoreClass fake = new scoreClass();
						fake.eventName = evname;
						fake.PAXScore = 0.0;
						fake.RookiePAXScore = 0.0;
						fake.RAWScore = 0.0;
						fake.carClass = "";
						fake.classScore = 0.0;
						driver.totalCones = 0;
						driver.scores.Add(fake);
					}
				}
			}
			// Go through and check for name changes under a car number
			// May not be an error, but complain anyway
			Dictionary<string, driverDataClass> carnums = new Dictionary<string, driverDataClass>();
			
			foreach (string driverName in scores.Keys)
			{
				driverDataClass driver = scores[driverName];
				string num = driver.number;
				if (carnums.ContainsKey(num) == false)
				{
					carnums.Add(num,driver);
				}
				else
				{
					driverDataClass olddriver = carnums[num];
					string oname = olddriver.firstName + "_" + olddriver.lastName;
					string fname = driver.firstName + "_" + driver.lastName;
					if ((oname.Substring(0,1) == fname.Substring(0,1)) && (oname != fname))
					{
						Console.Error.WriteLine("Name change for car #" + num + ": " + oname + "." + "-->" + fname);
						Console.Error.Write("\t" + oname +  ": ");
						foreach (scoreClass eventScores in olddriver.scores)
						{
							if (eventScores.RAWScore != 0.0)
							{
								Console.Error.Write(" " + eventScores.eventName);
							}
						}
						Console.Error.Write("\n\t" + fname + ": ");
						foreach (scoreClass eventScores in driver.scores)
						{
							if (eventScores.RAWScore != 0.0)
							{
								Console.Error.Write(" " + eventScores.eventName);
							}
						}
						Console.Error.WriteLine("");
					}
				}
				
			}
			
		}
		// ---------------------------------------------------------------------------
		public static string readData()
		{
			string[] filePaths = Directory.GetFiles(eventFolder, "*_CSVData.csv");
			// Read in timing data
			foreach (string scorefile in filePaths)
			{
				//Console.Error.WriteLine("Adding data for " + scorefile);
				
				CSVData ev = new CSVData();
				ev.loadData(scorefile,',');
				foreach (string carnum in ev.getKeys())
				{
					driverDataClass driver;
					// Each event is keyed by car number, but we will key the overalls by the driver's full name
					string driverName = ev.getField(carnum, "First Name") + "_" + ev.getField(carnum, "Last Name");
					if (scores.ContainsKey(driverName) == false)
					{
						driver = new driverDataClass();
						driver.number = carnum;
						driver.carClass = ev.getField(carnum, "Class");
						driver.carDescription = ev.getField(carnum, "Car");
						driver.driver = ev.getField(carnum, "Driver");
						driver.firstName = ev.getField(carnum, "First Name");
						driver.lastName = ev.getField(carnum, "Last Name");
						driver.member = "N";
						if (ev.getField(carnum, "Mbr").ToUpperInvariant().Contains("Y") == true)
						{
							driver.member = "Y";
						}
						driver.rookie = "N";
						if (ev.getField(carnum, "Rky").ToUpperInvariant().Contains("Y") == true)
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
						string mem = "N";
						if (ev.getField(carnum, "Mbr").ToUpperInvariant().Contains("Y") == true)
						{
							mem = "Y";
						}
						if (driver.member != mem)
						{
							TextWriter errorWriter = Console.Error;
							errorWriter.WriteLine("Membership change for " + driverName +
							                      " in event " + scorefile +
							                      "( " + driver.member + " to " + mem);
						}
						string rook = "N";
						if (ev.getField(carnum, "Rky").ToUpperInvariant().Contains("Y") == true)
						{
							rook = "Y";
						}
						if (driver.rookie != rook)
						{
							TextWriter errorWriter = Console.Error;
							errorWriter.WriteLine("Rookie change for " + driverName +
							                      " in event " + scorefile +
							                      "( " + driver.rookie + " to " + rook);
						}
					}
					driver = scores[driverName];
					scoreClass evScores = new scoreClass();

					string pax = ev.getField(carnum, "PAX Score");
					double.TryParse(pax, out evScores.PAXScore);
					string rookiepax = ev.getField(carnum, "Rookie PAX Score");
					double.TryParse(rookiepax, out evScores.RookiePAXScore);
					string raw = ev.getField(carnum, "RAW Score");
					double.TryParse(raw, out evScores.RAWScore);
					string cones = ev.getField(carnum, "Cones");
					int.TryParse(cones, out evScores.coneCount);
					evScores.carClass = ev.getField(carnum, "Class");
					string classScore = ev.getField(carnum, "Class Score");
					double.TryParse(classScore, out evScores.classScore);

					// parse out the event name from something like c:\blah\_scores_event3_CSVData.csv
					Regex reg = new Regex(@".*(event[0-9]+).*");
					Match m;
					m = reg.Match(scorefile);
					if (m.Success == false)
					{
						Console.WriteLine("Unable to parse:" + scorefile);
						return "";
					}
					GroupCollection g = m.Groups;
					evScores.eventName = g[1].Value;
					driver.scores.Add(evScores);
					
					if (eventNames.Contains(evScores.eventName) == false)
					{
						eventNames.Add(evScores.eventName);
					}
				}
			}

			return "";
		}
		// ---------------------------------------------------------------------------
		public static string classScores()
		{
			string results = "Overall Class scores\r\n";
			results += "\tNote that you must have attended at least " + numDays.ToString() + " events to obtain a class score\r\n";
			
			// First build a sorted class list
			// Sort order is given in the csv file
			SortedDictionary<int,paxInfo> sortedClassList = new SortedDictionary<int,paxInfo>();
			List<string> classList = classData.getKeys();
			foreach (string className in classList)
			{
				if (className.StartsWith("UNK") == true)
				{
					continue;
				}
				paxInfo p = new paxInfo();
				p.carClass = className;
				p.pax = classData.getField(className, "PAX");
				p.description = classData.getField(className, "Description");
				p.group = classData.getField(className, "Group");
				p.displayOrder = classData.getField(className, "Display Order");
				int orderVal;
				if (int.TryParse(p.displayOrder, out orderVal) == false)
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
			// If the number of events of the winner doesn't meet the minimum, then on to next
			
			foreach (KeyValuePair<int, paxInfo> classInfo in sortedClassList)
			{
				paxInfo curClass = classInfo.Value;
				string curClassGroup = curClass.group;
				int rank = 1;
				int trophies = 0;
				// make a new driver list, with all drivers that ever ran in this class
				List<KeyValuePair<string, driverDataClass>> myList = new List<KeyValuePair<string, driverDataClass>>(scores);
				
				// Sum up class scores for each driver that ran in this class
				foreach (KeyValuePair<string, driverDataClass> driver in myList)
				{
					//mwp -----------------------
					// Sort by class score, descending order
					driver.Value.scores.Sort(delegate(scoreClass first, scoreClass next)
					                         {
					                         	return next.classScore.CompareTo(first.classScore);
					                         }
					                        );
					// Add up scores until we hit the "best N" count
					int bestCount = numDays;
					driver.Value.totalClass= 0.0;
					driver.Value.classEventCount = 0;
					foreach (scoreClass eventScores in driver.Value.scores)
					{
						if (eventScores.carClass == curClass.carClass)
						{
							driver.Value.totalClass += eventScores.classScore;
							driver.Value.classEventCount += 1;
							bestCount -= 1;
							if (bestCount <= 0)
							{
								break;
							}
						}
					}
				}
				// Now sort the drivers by their total score in this class
				myList.Sort(delegate(KeyValuePair<string, driverDataClass> firstPair,
				                     KeyValuePair<string, driverDataClass> nextPair)
				            {
				            	return nextPair.Value.totalClass.CompareTo(firstPair.Value.totalClass);
				            }
				           );
				// We have to go through and calc the trophy count for this class
				int numCars = 0;
				foreach (KeyValuePair<string, driverDataClass> driver in myList)
				{
					// First check if the driver has enough events
					// If not, we are done with this entire class
					if (driver.Value.classEventCount < numDays)
					{
						break;
					}
					numCars += 1;
				}
				trophies = trophyCount(numCars);
				foreach (KeyValuePair<string, driverDataClass> driver in myList)
				{
					// First check if the driver has enough events
					// If not, we are done with this entire class
					if (driver.Value.classEventCount < numDays)
					{
						break;
					}
					if (driver.Value.totalClass <= 0.0)
					{
						break;
					}
					// Sort score list by event name
					driver.Value.scores.Sort(delegate(scoreClass first, scoreClass next)
					                         {
					                         	string ev1num = Regex.Match(first.eventName, @"\d+$").Value;
					                         	int ev1val;
					                         	Int32.TryParse(ev1num, out ev1val);
					                         	string ev2num = Regex.Match(next.eventName, @"\d+$").Value;
					                         	int ev2val;
					                         	Int32.TryParse(ev2num, out ev2val);
					                         	return ev1val.CompareTo(ev2val);
					                         }
					                        );
					if (rank == 1)
					{
						// OK -- print header
						results += String.Format("\r\n");
						results += String.Format("Class: " + curClass.carClass + "\r\n");
						results += String.Format("{0,4} {1,4} {2,3} {3,3} {4,-16}",
						                         "Rank","Car#","Mbr","Rky","Driver");
						foreach (string name in eventNames)
						{
							results += String.Format("{0,9}", name);
						}
						results += "     Best 8\r\n";
					}

					string trophyIndicator = "T";
					if (trophies-- <= 0) trophyIndicator = "";
					results += String.Format("{0,-1}{1,3} {2,4} {3,3} {4,3} {5,-16}",
					                         trophyIndicator,
					                         rank++,
					                         driver.Value.number,
					                         driver.Value.member,
					                         driver.Value.rookie,
					                         driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1)
					                        );
					foreach (scoreClass eventScores in driver.Value.scores)
					{
						if (eventScores.carClass == curClass.carClass)
						{
							results += String.Format(" {0,8:#0.000}", eventScores.classScore);
						}
						else
						{
							results += String.Format(" {0,8:#0.000}", 0.0);
						}
					}
					results += String.Format("   {0,8:#0.000}\r\n",driver.Value.totalClass);
				}
			}
			return results;
		}
		// ---------------------------------------------------------------------------
		// Calculate the number of trophy spots in a class
		public static int trophyCount(int numDrivers)
		{
			if (numDrivers == 0) return 0;
			if (numDrivers <= 3) return 1;
			if (numDrivers <= 6) return 2;
			if (numDrivers <= 9) return 3;
			// one more trophy for every additional four competitors
			if (numDrivers <= 13) return 4;
			if (numDrivers <= 17) return 5;
			if (numDrivers <= 21) return 6;
			if (numDrivers <= 25) return 7;
			if (numDrivers <= 29) return 8;
			if (numDrivers <= 33) return 9;
			if (numDrivers <= 37) return 10;
			return 11; // 41 in a class should do...
			
		}
		// ---------------------------------------------------------------------------
	}
}