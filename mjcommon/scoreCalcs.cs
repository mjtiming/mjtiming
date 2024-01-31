/*
 * 
 * Created by Murray Peterson
 * 
3.5.1. Results Format
All results for National events shall meet the following requirements:
Results should be structured in category (Stock, Super Stock, Street Prepared, Modified), with Classes listed alphabetically in each Category;
Class winners shall be listed in order of fastest to slowest.
All times are to be displayed as the time plus the number of pylons, and the corrected time (e.g. –competitor A: 65.25 sec + 2 pylons = 69.25 sec);
Ladies Classes will be listed separately from each regular class;
Competitors with sponsors shall have their sponsors names listed alongside the competitors name in the results under a heading of driver sponsors;
A separate heading shall be used to list the top 10 competitors overall;
A separate listing of all competitors w/ indexed times showing the Overall National Champion rankings.
An indication of the total number of competitors at the event.

3.5.2. Final Results -Presentation Format
All final results for National events shall meet the following requirements and shall be sent to ASN Canada FIA office, all territories, stewards, members of the ASN National SoloSport Committee, sponsors, etc.:
i) Details concerning the event (name of event, name of organizing club, date of event, status of event, permit number);
ii) Acknowledgement of sponsors, stewards, organizers, etc;

3.5.3. ASN Canada FIA Canadian AutoSlalom Championship Awards
1 trophy for 1 to 3 competitors in a class;
2 trophies for 4 to 6 competitors;
3 trophies for 7 to 9 competitors;
1 additional trophy for every four additional competitors.
Basically, 3,3,3,4,4,4,4,.
 */
// disable CompareOfFloatsByEqualityOperator
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace RaceBeam
{
	public class scoreArgs
	{
		public string configFolder;		// If blank, we use <current dir>\..\config
		public string eventFolder;		// If blank, we use config-->eventFolder
		public string classFile;		// If blank, we use config-->classFile
		public string day1;
		public string day2;
		public int maxOfficialRuns;
		public bool set1Only;
		public bool set2Only;
		public bool set1PlusSet2;
		public bool bestSingleRun;
		public bool showRunTimes;
		public bool showRawTimes;
		public bool showPaxTimes;
		public bool showClassTimes;
		public bool showConeCounts;
		public bool showTeams;
		public bool showRookie;			// Applies to raw/pax/class
		public bool writeCSV;
		public string title;
		public string outFile;
		public scoreArgs()
		{
			eventFolder = "";
			configFolder = "";
			classFile = "";
			day1 = "";
			day2 = "";
			maxOfficialRuns = 99;
			set1Only = false;
			set2Only = false;
			set1PlusSet2 = false;
			bestSingleRun = false;
			showRunTimes = false;
			showRawTimes = false;
			showPaxTimes = false;
			showClassTimes = false;
			showConeCounts = false;
			showTeams = false;
			showRookie = false;
			writeCSV = false;
			title = "";
			outFile = string.Empty;
		}
		// copy constructor
		public scoreArgs(scoreArgs src)
		{
			day1 = src.day1;
			day2 = src.day2;
			maxOfficialRuns = src.maxOfficialRuns;
			set1Only = src.set1Only;
			set2Only = src.set2Only;
			set1PlusSet2 = src.set1PlusSet2;
			bestSingleRun = src.bestSingleRun;
			showRunTimes = src.showRunTimes;
			showRawTimes = src.showRawTimes;
			showPaxTimes = src.showPaxTimes;
			showClassTimes = src.showClassTimes;
			showConeCounts = src.showConeCounts;
			showTeams = src.showTeams;
			showRookie = src.showRookie;
			writeCSV = src.writeCSV;
			eventFolder = src.eventFolder;
			title = src.title;
		}
	}
	// ---------------------------------------------------------------------------
	
	/// <summary>
	/// Perform all of the scoring functions and return a string which can be printed out to a text file or displayed
	/// in a window.
	/// </summary>
	public static class scoreCalcs
	{
		public static double DNFvalue = 9999.0;
		
		// All the info about a single run
		public class singleRunData
		{
			public int 		runNumber;
			public double 	time;
			public string	penalty;
			public double	adjustedRawTime;  	// 1000 for DNF
			public double	PAXtime;			// 1000 for DNF
			public int		coneCount;
			public string	startTime;
			public string	stopTime;
		}
		// All the run data for a single set
		public class singleSetData
		{
			public double			bestRAW = DNFvalue;
			public double			bestPAX = DNFvalue;
			public double			bestSUM = DNFvalue;
			public int				cones = 0;
			public singleRunData 	bestRun;
			public List<singleRunData> runs = new List<singleRunData>();
		}
		public class groupscore
		{
			public string	groupName;
			public int		groupRank;
			public int		groupRookieRank;
			public double	groupScore;
			public double	groupRookieScore;
			public bool		groupTrophy;
			public bool		groupRookieTrophy;
		}
		// Calculated score data for one type of score (PAX, RAW, class, etc)
		public class scoreDataClass
		{
			public double	bestFTD = DNFvalue;	// best raw time, any day, any set
			public double	bestRAW;
			public double	bestPAX;
			public double	bestSUM;
			
			
			public double	FTDpax;		// used for match scoring
			public int 		RAWrank;
			public int		RAWrookieRank;
			public double	RAWscore;
			public double	RAWrookieScore;
			public bool		RAWtrophy;
			public bool		RAWrookieTrophy;
			
			public int		PAXrank;
			public int		PAXRookieRank;
			public double	PAXscore;
			public double	PAXrookieScore;
			public bool		PAXtrophy;
			public bool		PAXrookieTrophy;
			public double	RAWtoNext;
			public double	RAWtoFirst;
			public double	RookieRAWtoNext;
			public double	RookieRAWtoFirst;

			// For Group and XGroup (multiple groups here, with ones inXgroup separated by a ';')
			public List<groupscore> groupScores = new List<groupscore>();
		}
		
		// Everything we know and calculate about a single driver
		public class driverScoreData
		{
			public string	number;
			public string	firstName;
			public string	lastName;
			public string	carDescription;
			public string	member;
			public bool		rookie;
			public string	carGroup;			// Primary group
			public string	carGroupDescription;
			public string	carXGroup;			// Xtra group
			public string	carXGroupDescription;
			public int		maxOfficialRuns;	// we use this to limit max runs that count for this driver
			public string	carClass;
			public string	team;
			public double	pax;
			public string	sponsor;
			public paxInfo	classInfo;
			
			public int		coneCount = 0;		// Total for all sets, all days
			
			public string   Day1Set1Info = "";	// Run info packed into a string
			public string   Day1Set2Info = "";
			public string   Day1Set3Info = "";
			public string   Day2Set1Info = "";
			public string   Day2Set2Info = "";
			public string   Day2Set3Info = "";
			
			public singleSetData Day1Set1 = new singleSetData();
			public singleSetData Day1Set2 = new singleSetData();
			public singleSetData Day1Set3 = new singleSetData();
			public singleSetData Day2Set1 = new singleSetData();
			public singleSetData Day2Set2 = new singleSetData();
			public singleSetData Day2Set3 = new singleSetData();
			public scoreDataClass scoreData = new scoreDataClass();
		}
		
		public class paxInfo
		{
			public string carClass;
			public string pax;
			public string description;
			public string group;
			public string displayOrder;
		}
		public class teamData
		{
			public string team;
			public string teamType;
			public double rawTotal;
			public int    rawRank;
			public double rawScore;
			public double paxTotal;
			public int    paxRank;
			public double paxScore;
			public int    coneTotal;
			public int    driversTotalled;
			public List<driverScoreData> teamDrivers;
			public teamData()
			{
				team = "";
				teamType = "";
				rawTotal = 0.0;
				rawScore = 0.0;
				paxTotal = 0.0;
				paxScore = 0.0;
				coneTotal = 0;
				driversTotalled = 0;
				teamDrivers = new List<driverScoreData>();
			}
		}
		public class dailyStats
		{
			public string 	set1TimeOfFirstRun = "";
			public string 	set1TimeOfLastRun = "";
			public int 		set1NumberOfRuns = 0;
			public double 	set1TotalTime = 0.0;
			public string 	set2TimeOfFirstRun = "";
			public string 	set2TimeOfLastRun = "";
			public int 		set2NumberOfRuns = 0;
			public double 	set2TotalTime = 0.0;
			public string 	set3TimeOfFirstRun = "";
			public string 	set3TimeOfLastRun = "";
			public int 		set3NumberOfRuns = 0;
			public double 	set3TotalTime = 0.0;
			public dailyStats()
			{
				set1NumberOfRuns = 0;
				set1TimeOfFirstRun = "";
				set1TimeOfLastRun = "";
				set1TotalTime = 0.0;
				set2NumberOfRuns = 0;
				set2TimeOfFirstRun = "";
				set2TimeOfLastRun = "";
				set2TotalTime = 0.0;
				set3NumberOfRuns = 0;
				set3TimeOfFirstRun = "";
				set3TimeOfLastRun = "";
				set3TotalTime = 0.0;
			}
		}
		public class statsDataClass
		{
			public dailyStats day1 = new dailyStats();
			public dailyStats day2 = new dailyStats();
		}
		// ---------------------------------------------------------------------------
		public static string doScore(scoreArgs args,
		                             out Dictionary<string,scoreCalcs.driverScoreData> driverScoreData,
		                             out List<scoreCalcs.teamData> teamScoreData,
		                             out statsDataClass statistics,
		                             out SortedDictionary<int,paxInfo> sortedClassDict
		                            )
		{
			driverScoreData = null;	// assume things go bad
			teamScoreData = null;
			statistics = null;
			sortedClassDict = null;
			
			driverData = new CSVData();
			timingDataDay1 = new CSVData();
			timingDataDay2 = new CSVData();
			classData = new CSVData();
			configData = new CSVData();
			scores = new Dictionary<string,driverScoreData>();
			driverDataFile = "";
			classDataFile = "";
			timingFileName = "";
			string configFolder = args.configFolder;
			string configFilename = "";
			if (string.IsNullOrEmpty(configFolder))
			{
				configFolder = Process.GetCurrentProcess().MainModule.FileName;
				configFolder = Path.GetDirectoryName(configFolder);
				configFilename = configFolder + "\\..\\config\\configData.csv";
			}
			else
			{
				configFilename =  configFolder + "\\configData.csv";
			}
			string err = configData.LoadData(configFilename,',',"Parameter");
			if (err != "")
			{
				return("Unable to load config file: " + err);
			}
			if (configData.GetField("ConesGetPAXed", "Value") == "Yes")
			{
				ConesGetPAXed = true;
			}
			
			driverDataFile = configData.GetField("driverDataFile", "Value");
			if (driverDataFile == "")
			{
				return("Driver data file not defined in config file");
			}
			
			classDataFile = configData.GetField("classDataFile", "Value");
			if (classDataFile == "")
			{
				return("Class data file not defined in config file");
			}
			if (string.IsNullOrEmpty(args.classFile) == false)
			{
				classDataFile = args.classFile;
			}
			
			string eventFolder = configData.GetField("eventDataFolder", "Value");
			if (eventFolder == "")
			{
				return("Timing data folder not defined in config file");
			}
			if (string.IsNullOrEmpty(args.eventFolder) == false)
			{
				eventFolder = args.eventFolder;
			}
			timingFileName = eventFolder + "\\" + args.day1 + "_timingData.csv";
			
			// Read in timing data
			err = timingDataDay1.LoadData(timingFileName,',',"index");
			if (err != "")
			{
				return(timingFileName + ": " + err);
			}
			// If day 2 data given, then read it in too
			if (args.day2 != "")
			{
				string filename = eventFolder + "\\" + args.day2 + "_timingData.csv";
				err = timingDataDay2.LoadData(filename,',',"index");
				if (err != "")
				{
					return(filename + ": " + err);
				}
			}
			
			// Make a copy of the driverData file if we haven't already
			string eventDriverDataFile = eventFolder + "\\" + args.day1 + "_driverData.csv";
			try
			{
				// Update copy of driver DataFile if we are working on today's data
				string today = DateTime.Now.ToString("yyyy_MM_dd");
				if ((configData.GetField("eventDataFolder", "Value") == eventFolder) &&	(today == args.day1))
				{
					File.Delete(eventDriverDataFile);
				}
				File.Copy(driverDataFile, eventDriverDataFile);
			}
			catch
			{
				// not interested in any errors
			}
			
			// Read in driverData file(s) for this event
			if (args.day1 != "")
			{
				string filename = eventFolder + "\\" + args.day1 + "_driverData.csv";
				err = driverData.LoadData(filename,',',"Number");
				if (err != "")
				{
					return(filename + ": " + err);
				}
			}
			
			// If day 2 data given, then read it in too
			if (args.day2 != "")
			{
				string filename = eventFolder + "\\" + args.day2 + "_driverData.csv";
				err = driverData.LoadData(filename,',',"Number");
				if (err != "")
				{
					return(filename + ": " + err);
				}
			}
			
			// Read in class data file
			err = classData.LoadData(classDataFile,',',"Class");
			if (err != "")
			{
				return(err);
			}
			// First build a sorted class and group list
			// Sort order is given in the csv file
			if (sortedClassList == null)
			{
				sortedClassList = new SortedDictionary<int,paxInfo>();
				
				List<string> classList = classData.getKeys();
				foreach (string className in classList)
				{
					var p = new paxInfo();
					p.carClass = className;
					p.pax = classData.GetField(className, "PAX");
					p.description = classData.GetField(className, "Description");
					p.group = classData.GetField(className, "Group");
					p.displayOrder = classData.GetField(className, "Display Order");
					int orderVal;
					if (int.TryParse(p.displayOrder, out orderVal) == false)
					{
						return String.Format("Display order value for class " + p.carClass + " is not a valid integer\r\n");
					}
					else
					{
						try
						{
							sortedClassList.Add(orderVal, p);
						}
						catch
						{
							return String.Format("Multiple sort order entries for class " + p.carClass + "\r\n");
						}
					}
				}
			}
			if (args.day2 != "")
			{
				// Ignore day flags in each file
				parseRunData(args, timingDataDay1, 1);
				parseRunData(args, timingDataDay2, 2);
			}
			else
			{
				// Use day flags inside timing file
				parseRunData(args, timingDataDay1, 1);
			}
			
			// Do some calcs and fixups on the run data
			calcRunTimes(args);
			rawTimes(args);
			paxTimes(args);


			List<scoreCalcs.teamData> teamScores = teamTimes(args);
			err = classTimes(args);
			if (err != "")
				return err;
			
			statistics = stats;
			driverScoreData = scores;
			teamScoreData = teamScores;
			sortedClassDict = sortedClassList;
			
			if (args.writeCSV == true)
			{
				// Write out csv data
				string csvresults = doCSV(args);
				string csvFileName;
				
				if (args.day2 == "")
				{
					csvFileName = eventFolder + "\\" + args.day1 + "_CSVData.csv";
				}
				else
				{
					csvFileName = eventFolder + "\\" + args.day2 + "__2-day__CSVData.csv";
				}
				try
				{
					TextWriter tw = new StreamWriter(csvFileName);
					tw.WriteLine(csvresults);
					tw.Close();
				}
				catch
				{
					
				}
			}
			return "";
		}
		// ---------------------------------------------------------------------------
		private static CSVData driverData = new CSVData();
		private static CSVData timingDataDay1 = new CSVData();
		private static CSVData timingDataDay2 = new CSVData();
		private static CSVData classData = new CSVData();
		private static CSVData configData = new CSVData();
		
		private static Dictionary<string,driverScoreData> scores = new Dictionary<string,driverScoreData>();
		private static SortedDictionary<int,paxInfo> sortedClassList = null;
		private static statsDataClass stats = new scoreCalcs.statsDataClass();
		
		private static string driverDataFile = "";
		private static string classDataFile = "";
		private static string timingFileName = "";
		private static bool ConesGetPAXed = false;   // Do we apply pax to time+cones?
		// ---------------------------------------------------------------------------
		// calculate raw scores
		public static void rawTimes(scoreArgs args)
		{
			var myList = new List<KeyValuePair<string, driverScoreData>>(scores);
			// Sort by raw time
			myList.Sort(delegate(KeyValuePair<string, driverScoreData> firstPair,
			                     KeyValuePair<string, driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestRAW.CompareTo(nextPair.Value.scoreData.bestRAW);
			            }
			           );
			
			int rank = 1;
			int rookieRank = 1;
			double bestTime = 0.0;
			double rookieBestTime = 0.0;
			foreach (KeyValuePair<string, driverScoreData> driver in myList)
			{
				if (driver.Value.rookie == false)
				{
					driver.Value.scoreData.RAWrookieScore = 0.0;
					driver.Value.scoreData.RAWrookieRank = 0;
				}
				
				if (rank == 1)
				{
					bestTime = driver.Value.scoreData.bestRAW;
					driver.Value.scoreData.RAWscore = 100.00;
					driver.Value.scoreData.RAWrank = rank;
				}
				if ((rookieRank == 1) && (driver.Value.rookie == true))
				{
					rookieBestTime = driver.Value.scoreData.bestRAW;
					driver.Value.scoreData.RAWscore = 100.0;
					driver.Value.scoreData.RAWrank = rank;
				}
				
				if (rank != 1)	// include rookies in overall
				{
					driver.Value.scoreData.RAWscore = bestTime / driver.Value.scoreData.bestRAW * 100.00;
					driver.Value.scoreData.RAWrank = rank;
					if (driver.Value.rookie == true)
					{
						driver.Value.scoreData.RAWrookieScore = rookieBestTime / driver.Value.scoreData.bestRAW * 100.00;
						driver.Value.scoreData.RAWrookieRank = rookieRank;
					}
					if (driver.Value.scoreData.bestRAW >= DNFvalue)
					{
						driver.Value.scoreData.RAWscore = 0.0;
						driver.Value.scoreData.RAWrookieScore = 0.0;
					}
				}
				rank += 1;
				if (driver.Value.rookie == true)
				{
					rookieRank += 1;
				}
			}
		}
		// ---------------------------------------------------------------------------
		// Calculate PAX scores
		public static void paxTimes(scoreArgs args)
		{
			int rank = 1;
			int rookieRank = 1;
			double bestTime = 0.0;
			double rookieBestTime = 0.0;
			double prevTime = 0.0;
			double prevRookieTime = 0.0;

			var myList = new List<KeyValuePair<string, driverScoreData>>(scores);
			// Sort by pax time
			myList.Sort(delegate(KeyValuePair<string, driverScoreData> firstPair,
			                     KeyValuePair<string, driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestPAX.CompareTo(nextPair.Value.scoreData.bestPAX);
			            }
			           );
			
			foreach (KeyValuePair<string, driverScoreData> driver in myList)
			{
				if (driver.Value.rookie == false)
				{
					driver.Value.scoreData.PAXrookieScore = 0.0;
				}
				if (rank == 1)
				{
					bestTime = driver.Value.scoreData.bestPAX;
					prevTime = bestTime;
					driver.Value.scoreData.PAXscore = 100.00;
					driver.Value.scoreData.PAXrank = 1;
					driver.Value.scoreData.RAWtoFirst = 0.0;
					driver.Value.scoreData.RAWtoNext = 0.0;
				}
				if ((rookieRank == 1) && (driver.Value.rookie == true))
				{
					rookieBestTime = driver.Value.scoreData.bestPAX;
                    prevRookieTime = rookieBestTime;
					driver.Value.scoreData.PAXrookieScore = 100.0;
					driver.Value.scoreData.PAXRookieRank = 1;
					driver.Value.scoreData.RookieRAWtoFirst = 0.0;
					driver.Value.scoreData.RookieRAWtoNext = 0.0;
				}
				if (rank != 1)	// include rookies in overall
				{
					driver.Value.scoreData.PAXscore = bestTime / driver.Value.scoreData.bestPAX * 100.00;
					if (driver.Value.scoreData.bestPAX >= DNFvalue)
					{
						driver.Value.scoreData.PAXscore = 0.0;
						//calc raw from pax difference
						if (bestTime >= DNFvalue)
                        {
							driver.Value.scoreData.RAWtoFirst = 0.0;
						}
                        else
						{
							driver.Value.scoreData.RAWtoFirst = bestTime / driver.Value.pax;
						}
						if (prevTime >= DNFvalue)
						{
							driver.Value.scoreData.RAWtoNext = 0.0;
						}
						else
						{
							driver.Value.scoreData.RAWtoNext = prevTime / driver.Value.pax;
						}
					}
					else
                    {
						//calc raw from pax difference
						driver.Value.scoreData.RAWtoFirst = (driver.Value.scoreData.bestPAX - bestTime) /driver.Value.pax;
						driver.Value.scoreData.RAWtoNext = (driver.Value.scoreData.bestPAX - prevTime) / driver.Value.pax;
					}
					driver.Value.scoreData.PAXrank = rank;
					if (driver.Value.rookie == true)
					{
						driver.Value.scoreData.PAXrookieScore = rookieBestTime / driver.Value.scoreData.bestPAX * 100.00;
						if (driver.Value.scoreData.bestPAX >= DNFvalue)
						{
							driver.Value.scoreData.PAXrookieScore = 0.0;
							if (rookieBestTime >= DNFvalue)
							{
								driver.Value.scoreData.RookieRAWtoFirst = 0.0;
							}
							else
							{
								driver.Value.scoreData.RookieRAWtoFirst = rookieBestTime / driver.Value.pax;
							}
							if (prevRookieTime >= DNFvalue)
							{
								driver.Value.scoreData.RookieRAWtoNext = 0.0;
							}
							else
							{
								driver.Value.scoreData.RookieRAWtoNext = prevRookieTime / driver.Value.pax;
							}
						}
						else
                        {
							//calc raw from pax difference
							driver.Value.scoreData.RookieRAWtoFirst = (driver.Value.scoreData.bestPAX - rookieBestTime) / driver.Value.pax;
							driver.Value.scoreData.RookieRAWtoNext = (driver.Value.scoreData.bestPAX - prevRookieTime) / driver.Value.pax;
						}
						driver.Value.scoreData.PAXRookieRank = rookieRank;
					}
				}
				if (driver.Value.scoreData.bestPAX < DNFvalue)
				{
					prevTime = driver.Value.scoreData.bestPAX;
				}
				rank += 1;
				if (driver.Value.rookie == true)
				{
					rookieRank += 1;
					if (driver.Value.scoreData.bestPAX < DNFvalue)
					{
						prevRookieTime = driver.Value.scoreData.bestPAX;
					}
				}
			}
		}
		
		// ---------------------------------------------------------------------------
		// print out times ordered within group and Xgroup by PAX time
		public static string classTimes(scoreArgs args)
		{
			
			// Sort drivers by PAX time
			var myList = new List<KeyValuePair<string, driverScoreData>>(scores);
			myList.Sort(delegate(KeyValuePair<string, driverScoreData> firstPair,
			                     KeyValuePair<string, driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestPAX.CompareTo(nextPair.Value.scoreData.bestPAX);
			            }
			           );
			
			foreach (KeyValuePair<int, paxInfo> classInfo in sortedClassList)
			{
				paxInfo curClass = classInfo.Value;
				string curClassGroup = curClass.group;
				int rank = 1;
				int rookieRank = 1;
				double bestTime = 0.0;
				double rookieBestTime = 0.0;
				int trophies = 0;
				int rookieTrophies = 0;
				int carsInClass = 0;
				int rookieCarsInClass = 0;
				
				// First go through and get a total count of cars in this class so we can allocate trophies
				foreach (KeyValuePair<string, driverScoreData> driver in myList)
				{
					string origXgrps = driver.Value.carXGroup;
					origXgrps += ";" + driver.Value.carGroup;

					string sval = configData.GetField("ClassAsGroup", "Value");
					sval = sval.ToUpperInvariant();
					if (sval.Contains("Y"))
					{
						origXgrps += ";" + driver.Value.carClass;
					}
					string[] xgroups = origXgrps.Split(';');
					bool driverIsInXgroup = false;
					
					foreach (string xg in xgroups)
					{
						if (xg == curClass.carClass)
						{
							driverIsInXgroup = true;
							break;
						}
					}
					if (driverIsInXgroup == false)
					{
						continue;
					}

					if (driver.Value.rookie == true)
					{
						rookieCarsInClass += 1;
					}
					carsInClass += 1;
				}
				trophies = trophyCount(carsInClass);
				rookieTrophies = trophyCount(rookieCarsInClass);
				
				
				foreach (KeyValuePair<string, driverScoreData> driver in myList)
				{
					string origXgrps = driver.Value.carXGroup;
					origXgrps += ";" + driver.Value.carGroup;

					string sval = configData.GetField("ClassAsGroup", "Value");
					sval = sval.ToUpperInvariant();
					if (sval.Contains("Y"))
					{
						origXgrps += ";" + driver.Value.carClass;
					}
					string[] xgroups = origXgrps.Split(';');
					bool driverIsInXgroup = false;
					
					foreach (string xg in xgroups)
					{
						if (xg == curClass.carClass)
						{
							driverIsInXgroup = true;
							break;
						}
					}
					if (driverIsInXgroup == false)
					{
						continue;
					}
					// Clear out all the existing scores for this driver and group
					for (int index = 0; index < driver.Value.scoreData.groupScores.Count; index++)
					{
						if (driver.Value.scoreData.groupScores[index].groupName == curClass.carClass)
						{
							driver.Value.scoreData.groupScores.RemoveAt(index);
							break;
						}
					}
					var grp = new groupscore();
					grp.groupName = curClass.carClass;
					driver.Value.scoreData.groupScores.Add(grp);

					if (rank == 1)
					{
						grp.groupScore = 100.0;
						grp.groupRank = rank;
						bestTime = driver.Value.scoreData.bestPAX;
					}
					if ((rookieRank == 1) && (driver.Value.rookie == true))
					{
						grp.groupRookieScore = 100.0;
						grp.groupRookieRank = rookieRank;
						rookieBestTime = driver.Value.scoreData.bestPAX;
					}
					if (rank != 1)
					{
						if (driver.Value.rookie == true)
						{
							grp.groupRookieScore = rookieBestTime / driver.Value.scoreData.bestPAX * 100.00;
							if (driver.Value.scoreData.bestPAX >= DNFvalue)
							{
								grp.groupRookieScore = 0.0;
							}
							grp.groupRookieRank = rookieRank;
						}
						// rookies are included in the overall scores
						grp.groupScore = bestTime / driver.Value.scoreData.bestPAX * 100.00;
						if (driver.Value.scoreData.bestPAX >= DNFvalue)
						{
							grp.groupScore = 0.0;
						}
						grp.groupRank = rank;
					}
					
					if (trophies-- <= 0)
					{
						grp.groupTrophy = false;
					}
					else
					{
						grp.groupTrophy = true;
					}
					rank += 1;
					if (driver.Value.rookie == true)
					{
						if (rookieTrophies-- <= 0)
						{
							grp.groupRookieTrophy = false;
						}
						else
						{
							grp.groupRookieTrophy = true;
						}
						rookieRank += 1;
					}
				}
			}
			return "";
		}
		// ---------------------------------------------------------------------------
		public static List<scoreCalcs.teamData> teamTimes(scoreArgs args)
		{
			var teams = new List<scoreCalcs.teamData>();
			
			// go through and collate into teams (based on team name)
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	string firstTeam = firstPair.Value.team;
			            	string secondTeam = nextPair.Value.team;
			            	if (secondTeam == null)
			            		return 1;
			            	if (firstTeam == null)
			            		return -1;
			            	return firstTeam.CompareTo(nextPair.Value.team);
			            }
			           );
			string sval = configData.GetField("teamNumScores", "Value");
			int teamNumScores = 4;
			if (Int32.TryParse(sval, out teamNumScores) == false)
			{
				teamNumScores = 4;
			}
			
			scoreCalcs.teamData curTeam = null;
			foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
			{
				if (driver.Value.team == null)
				{
					continue;
				}
				if ((curTeam == null) || (driver.Value.team != curTeam.team))
				{
					// Not the same team, so create a new one
					curTeam = new scoreCalcs.teamData();
					curTeam.team = driver.Value.team;
					curTeam.teamType = driver.Value.carGroup;	// Only works for RAW/PAX
					teams.Add(curTeam);
				}
				// add driver to team
				curTeam.teamDrivers.Add(driver.Value);

				// Everybody gets to add to the cone count
				curTeam.coneTotal += driver.Value.coneCount;
			}
			
			// Go through teams (twice) and assign a PAX score and a RAW score
			// Add all drivers and sort by raw time, then use best n
			foreach (var team in teams)
			{
				// first sort each driver by his by RAW time
				team.teamDrivers.Sort(delegate(driverScoreData first, driverScoreData next)
				                      {
				                      	var firstRaw = first.scoreData.bestRAW;
				                      	var nextRaw = next.scoreData.bestRAW;
				                      	return firstRaw.CompareTo(nextRaw);
				                      }
				                     );
				// Add driver's RAW totals to team score, but only the best <n> drivers are used
				team.driversTotalled = 0;
				foreach (driverScoreData driver in team.teamDrivers)
				{
					if (team.driversTotalled < teamNumScores)
					{
						team.driversTotalled += 1;
						team.rawTotal += driver.scoreData.bestRAW;
					}
				}
				// now sort each driver by his by PAX time
				team.teamDrivers.Sort(delegate(driverScoreData first, driverScoreData next)
				                      {
				                      	var firstPax = first.scoreData.bestPAX;
				                      	var nextPax = next.scoreData.bestPAX;
				                      	return firstPax.CompareTo(nextPax);
				                      }
				                     );
				
				// Add driver's PAX totals to team score, but only the best <n> drivers are used
				team.driversTotalled = 0;
				foreach (driverScoreData driver in team.teamDrivers)
				{
					if (team.driversTotalled < teamNumScores)
					{
						team.driversTotalled += 1;
						team.paxTotal += driver.scoreData.bestPAX;
					}
				}
			}
			
			// First sort by RAW
			teams.Sort(delegate(scoreCalcs.teamData firstTeam, teamData nextTeam)
			           {
			           	return firstTeam.rawTotal.CompareTo(nextTeam.rawTotal);
			           }
			          );
			double bestTime = 0;
			int rank = 1;
			foreach (var team in teams)
			{
				if (rank == 1)
				{
					bestTime = team.rawTotal;
					team.rawScore = 100.00;
					team.rawRank = rank;
				}
				else
				{
					team.rawScore = bestTime / team.rawTotal * 100.00;
					team.rawRank = rank;
				}
				rank += 1;
			}
			// Now sort by PAX totals
			teams.Sort(delegate(scoreCalcs.teamData firstTeam, teamData nextTeam)
			           {
			           	return firstTeam.paxTotal.CompareTo(nextTeam.paxTotal);
			           }
			          );
			rank = 1;
			foreach (var team in teams)
			{
				if (rank == 1)
				{
					bestTime = team.paxTotal;
					team.paxScore = 100.00;
					team.paxRank = rank;
				}
				else
				{
					team.paxScore = bestTime / team.paxTotal * 100.00;
					team.paxRank = rank;
				}
				rank += 1;
			}
			return teams;
		}
		// ---------------------------------------------------------------------------
		public static int GetLeadingInt(string input)
		{
			int i = 0;
			input = input.Trim();
			while (i < input.Length && char.IsDigit(input[i])) i++;

			input = input.Substring(0, i);
			int value = 0;
			if (int.TryParse(input, out value) == false)
			{
				return 0;
			}
			return value;
		}
		// ---------------------------------------------------------------------------
		// Return CSV format data -- won't work unless raw and pax calcs have been done previously
		public static string doCSV(scoreArgs args)
		{
			bool wasrookie = args.showRookie;
			args.showRookie = true;	// force rookie calcs
			rawTimes(args);
			paxTimes(args);
			classTimes(args);
			args.showRookie = wasrookie;
			classTimes(args);

			string results = "";
			var myList = new List<KeyValuePair<string, driverScoreData>>(scores);
			// Sort by car number
			myList.Sort(delegate(KeyValuePair<string, driverScoreData> firstPair,
			                     KeyValuePair<string, driverScoreData> nextPair)
			            {
			            	int d1Number,d2Number;
			            	d1Number = GetLeadingInt(firstPair.Value.number);
			            	d2Number = GetLeadingInt(nextPair.Value.number);
			            	return d1Number.CompareTo(d2Number);
			            }
			           );
			// see http://blog.stevex.net/string-formatting-in-csharp/ for a C# formatting info
			
			// Go through the xgroup lists for each driver and create a new list containing all of the xgroup names
			// We also calc the max xgroups for any driver

			var maxXgroups = 0;
			foreach (KeyValuePair<string, driverScoreData> driver in myList)
			{
				string origXgrps = driver.Value.carXGroup;
				if (string.IsNullOrEmpty(origXgrps))
				{
					origXgrps = driver.Value.carGroup;
				}
				else
				{
					origXgrps += ";" + driver.Value.carGroup;
				}
				string sval = configData.GetField("ClassAsGroup", "Value");
				sval = sval.ToUpperInvariant();
				if (sval.Contains("Y"))
				{
					if (string.IsNullOrEmpty(origXgrps))
					{
						origXgrps = driver.Value.carClass;
					}
					else
					{
						origXgrps += ";" + driver.Value.carClass;
					}
				}
				string[] xgrps = origXgrps.Split(';');
				if (xgrps.Length > maxXgroups)
				{
					maxXgroups = xgrps.Length;
				}
			}
			
			// We can do xgroup headers now that we know how many xgroups exist
			results += String.Format("Car#,Mbr,Rky,Group,XGroup,Class,Team,PAX,Driver,First Name,Last Name,Car,Sponsor,RAW time,PAX time,PAX Score,Rookie PAX Score,RAW Score,Rookie RAW Score,Cones,Day1Set1 Runs,Day1Set2 Runs,Day1Set3 Runs");
			for (int xgcount = 1; xgcount <= maxXgroups; xgcount++)
			{
				results += String.Format(",Xgroup-{0}:Name,Xgroup-{1}:Score,Xgroup-{2}:Rookie Score",xgcount,xgcount,xgcount);
			}
			results += "\r\n";
			
			foreach (KeyValuePair<string, driverScoreData> driver in myList)
			{
				string driverName = driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1);
				if (configData.GetField("ShowLastName", "Value").Contains("Y"))
				{
					driverName = driver.Value.firstName + " " + driver.Value.lastName;
				}
				string line = String.Format("{0},{1},{2},{3},{4},{5},{6},{7,8:#.000},{8},{9},{10},{11},{12,8:#.000},{13,8:#.000},{14,8:#.000},{15,8:#.000},{16,8:#.000},{17,8:#.000},{18,8:#.000},{19},{20},{21},{22}",
				                            driver.Value.number,
				                            driver.Value.member,
				                            driver.Value.rookie?"Y":"N",
				                            driver.Value.carGroup,
				                            driver.Value.carXGroup,
				                            driver.Value.carClass,
				                            driver.Value.team,
				                            driver.Value.pax,
				                            driverName,
				                            driver.Value.firstName,
				                            driver.Value.lastName,
				                            driver.Value.carDescription,
				                            driver.Value.sponsor,
				                            driver.Value.scoreData.bestRAW,
				                            driver.Value.scoreData.bestPAX,
				                            driver.Value.scoreData.PAXscore,
				                            driver.Value.scoreData.PAXrookieScore,
				                            driver.Value.scoreData.RAWscore,
				                            driver.Value.scoreData.RAWrookieScore,
				                            driver.Value.coneCount.ToString(),
				                            driver.Value.Day1Set1Info,
				                            driver.Value.Day1Set2Info,
				                            driver.Value.Day1Set3Info
				                           );
				// emit all the xgroup scores (we know we have enough xgroup headers to hold the list)
				foreach(var grp in driver.Value.scoreData.groupScores)
				{
					line += String.Format(",{0},{1},{2}",
					                      grp.groupName,
					                      grp.groupScore,
					                      grp.groupRookieScore
					                     );
				}
				line += "\r\n";
				results += String.Format(line);
			}
			return results;
		}
		// ---------------------------------------------------------------------------
		// Parse up the run data and get it into our dictionary
		// Do a few calcs on the way
		public static void parseRunData(scoreArgs args, CSVData rdata, int dayNumber)
		{
			
			// Refresh statistics
			stats = new statsDataClass();
			
			for (int i = 0; i < rdata.Length(); i++)
			{
				driverScoreData drv;
				var run = new singleRunData();
				double fval;
				int ival;
				string sval;
				string indexS = i.ToString();
				
				// we key everything by car number
				string number = rdata.GetField(indexS, "car_number");
				if (string.IsNullOrEmpty(number) == true)
				{
					continue;
				}
				string penalty = rdata.GetField(indexS, "penalty");
				if (penalty == "RRN")
				{
					continue;
				}
				if (scores.ContainsKey(number))
				{
					drv = scores[number];
				}
				else
				{
					drv = new driverScoreData();
				}
				drv.number = number;
				if (drv.firstName == null)
				{
					drv.firstName = driverData.GetField(number, "First Name");
					if (drv.firstName == "")
					{
						drv.firstName = "Unknown";
						drv.lastName = "Driver";
						drv.carDescription = "Unknown car";
						drv.member = "No";
						drv.rookie = false;
						drv.carClass = "AM";
						drv.carGroup = "AM";
						drv.carXGroup = "";
						drv.carGroupDescription = "Unregistered";
						drv.pax = 1.0;
						//Console.WriteLine("Missing data for car #" + number.ToString());
					}
				}
				if (drv.lastName == null)
				{
					drv.lastName = driverData.GetField(number, "Last Name");
					if (drv.lastName == "")
					{
						drv.lastName = "Unknown";
					}
				}
				if (drv.carDescription == null)
				{
					drv.carDescription = driverData.GetField(number, "Car Model");
				}
				if (drv.member == null)
				{
					drv.member = driverData.GetField(number, "Member");
					if (drv.member.ToUpperInvariant().Contains("Y") == true)
					{
						drv.member = "Yes";
					}
					else
					{
						drv.member = "No";
					}
				}
				if (drv.sponsor == null)
				{
					drv.sponsor = driverData.GetField(number, "Sponsor");
				}
				if (drv.team == null)
				{
					drv.team = driverData.GetField(number, "Team");
				}
				
				if (drv.carDescription.Length > 20)
				{
					drv.carDescription = drv.carDescription.Substring(0,20);
				}
				
				string rookie = driverData.GetField(number, "Rookie");
				if (rookie.ToUpperInvariant().Contains("Y") == true)
				{
					drv.rookie = true;
				}
				else
				{
					drv.rookie = false;
				}
				if (drv.carClass == null)
				{
					drv.carClass = driverData.GetField(number, "Class");
					string realClass = drv.carClass;
					sval = classData.GetField(realClass, "PAX");
					if (double.TryParse(sval, out fval) == true)
					{
						drv.pax = fval;
					}
					else
					{
						drv.pax = 1.0;
					}
				}
				if (drv.carGroup == null)
				{
					drv.carGroup = driverData.GetField(number, "Group");
					if (string.IsNullOrEmpty(drv.carGroup) == true)
					{
						drv.carGroup = drv.carClass;
					}
					drv.carGroupDescription = classData.GetField(drv.carGroup, "Description");
					// Use group PAX as a max official runs value (when it is > 2)
					drv.maxOfficialRuns = args.maxOfficialRuns;
					int intVal = 0;
					sval = classData.GetField(drv.carGroup, "PAX");
					if (int.TryParse(sval, out intVal) == true)
					{
						if (intVal > 2)
						{
							drv.maxOfficialRuns = intVal;
						}
					}
				}
				if (drv.carXGroup == null)
				{
					drv.carXGroup = driverData.GetField(number, "XGroup");
					if (string.IsNullOrEmpty(drv.carXGroup) == true)
					{
						drv.carXGroup = "";
						drv.carXGroupDescription = "";
					}
					else
					{
						drv.carXGroupDescription = classData.GetField(drv.carXGroup, "Description");
						// Use group PAX as a max official runs value (when it is > 2)
						// If both group and xgroup specify a value, use the lowest one
						int intVal = 0;
						sval = classData.GetField(drv.carXGroup, "PAX");
						if (int.TryParse(sval, out intVal) == true)
						{
							if ((intVal > 2) && (intVal < drv.maxOfficialRuns))
							{
								drv.maxOfficialRuns = intVal;
							}
						}
					}
				}
				string stopstr = rdata.GetField(indexS, "stop_time");
				string startstr = rdata.GetField(indexS, "start_time");
				double stopint,startint;
				double runt = 0.0;
				if ((stopstr != null) && (double.TryParse(stopstr, out stopint) == true) && (double.TryParse(startstr, out startint) == true))
				{
					if (stopint > startint)
					{
						runt = unchecked(stopint - startint);
					}
					else
					{
						runt = unchecked(1000000 - startint + stopint);
					}
					runt = runt / 1000;
				}
				sval = rdata.GetField(indexS, "run_time");
				
				if (double.TryParse(sval, out fval) == true)
				{
					run.time = fval;
					if ((fval != runt) && (runt != 0.0))
					{
						run.time = runt;
					}
					run.penalty = rdata.GetField(indexS, "penalty");
					run.adjustedRawTime = adjustTime(run.time, run.penalty, 1.0);
					run.coneCount = calcCones(run.penalty);
					if (run.adjustedRawTime == DNFvalue)
					{
						run.PAXtime = DNFvalue;
					}
					else
					{
						run.PAXtime = adjustTime(run.time, run.penalty, drv.pax);
					}
					sval = rdata.GetField(indexS, "run_number");
					run.runNumber = 0;
					if (Int32.TryParse(sval, out ival) == true)
					{
						run.runNumber = ival;
					}
					// Handle old style "day" instead of "set"
					string setFlag = rdata.GetField(indexS, "set");
					if (setFlag == "")
					{
						setFlag = rdata.GetField(indexS, "day");
					}
					int set = 3;
					if (Int32.TryParse(setFlag, out ival) == true)
					{
						set = ival;
					}
					if (set > 3)
					{
						set = 3;	// Make it a fun run
					}
					run.startTime = rdata.GetField(indexS, "datetime");
					if (run.startTime.Length > 3)
					{
						run.startTime = run.startTime.Substring(3);
					}
					run.stopTime = rdata.GetField(indexS, "datetime_stop");
					if (run.stopTime.Length > 3)
					{
						run.stopTime = run.stopTime.Substring(3);
					}
					// update this driver's raw FTD
					if (drv.scoreData.bestFTD > run.adjustedRawTime)
					{
						drv.scoreData.bestFTD = run.adjustedRawTime;
					}
					if (set == 1)
					{
						if (dayNumber == 1)
						{
							if (drv.Day1Set1.bestSUM == DNFvalue)
								drv.Day1Set1.bestSUM = 0.0;	// Actually exists, so reset from DNF
							
							// some interesting stats
							if (stats.day1.set1TimeOfFirstRun == "")
							{
								stats.day1.set1TimeOfFirstRun = run.startTime;
							}
							stats.day1.set1TimeOfLastRun = run.startTime;
							stats.day1.set1NumberOfRuns += 1;
							stats.day1.set1TotalTime += run.time;
							drv.Day1Set1Info += String.Format("{0,5:#.000}", run.time);
							if (string.IsNullOrEmpty(run.penalty) == false)
							{
								drv.Day1Set1Info += "+" + run.penalty;
							}
							drv.Day1Set1Info += ";";
							
							drv.Day1Set1.runs.Add(run);	// Add this run to the driver's list

							if ((drv.Day1Set1.runs.Count <= drv.maxOfficialRuns) && (run.adjustedRawTime < drv.Day1Set1.bestRAW))
							{
								drv.Day1Set1.bestRAW = run.adjustedRawTime;
								drv.Day1Set1.bestRun = run;
							}
							if ((drv.Day1Set1.runs.Count <= drv.maxOfficialRuns) && (run.PAXtime < drv.Day1Set1.bestPAX))
							{
								drv.Day1Set1.bestPAX = run.PAXtime;
							}

							drv.coneCount += run.coneCount;
							if (run.adjustedRawTime >= DNFvalue)
							{
								string dnfSeconds = configData.GetField("secondsForDNF", "Value");
								int DNFpenalty;
								if (int.TryParse(dnfSeconds, out DNFpenalty) == false)
								{
									DNFpenalty = 200;
								}
								drv.Day1Set1.bestSUM += DNFpenalty;	// We count a DNF as 200 seconds
							}
							else
							{
								drv.Day1Set1.bestSUM += run.adjustedRawTime;
							}
						}
						else	// day 2
						{
							if (drv.Day2Set1.bestSUM == DNFvalue)
								drv.Day2Set1.bestSUM = 0.0;	// Actually exists, so reset from DNF
							
							// some interesting stats
							if (stats.day2.set1TimeOfFirstRun == "")
							{
								stats.day2.set1TimeOfFirstRun = run.startTime;
							}
							stats.day2.set1TimeOfLastRun = run.startTime;
							stats.day2.set1NumberOfRuns += 1;
							stats.day2.set1TotalTime += run.time;
							drv.Day2Set1Info += String.Format("{0,5:#.000}", run.time);
							if (string.IsNullOrEmpty(run.penalty) == false)
							{
								drv.Day2Set1Info += "+" + run.penalty;
							}
							drv.Day2Set1Info += ";";
							
							drv.Day2Set1.runs.Add(run);	// Add this run to the driver's list
							
							if ((drv.Day2Set1.runs.Count <= drv.maxOfficialRuns) && (run.adjustedRawTime < drv.Day2Set1.bestRAW))
							{
								drv.Day2Set1.bestRAW = run.adjustedRawTime;
								drv.Day2Set1.bestRun = run;
							}
							if ((drv.Day2Set1.runs.Count <= drv.maxOfficialRuns) && (run.PAXtime < drv.Day2Set1.bestPAX))
							{
								drv.Day2Set1.bestPAX = run.PAXtime;
							}
							drv.coneCount += run.coneCount;
							if (run.adjustedRawTime >= DNFvalue)
							{
								string dnfSeconds = configData.GetField("secondsForDNF", "Value");
								int DNFpenalty;
								if (int.TryParse(dnfSeconds, out DNFpenalty) == false)
								{
									DNFpenalty = 200;
								}
								drv.Day2Set1.bestSUM += DNFpenalty;	// We count a DNF as 200 seconds
							}
							else
							{
								drv.Day2Set1.bestSUM += run.adjustedRawTime;
							}
						}
					}
					else if (set == 2)
					{
						if (dayNumber == 1)
						{
							if (drv.Day1Set2.bestSUM == DNFvalue)
								drv.Day1Set2.bestSUM = 0.0;	// Actually exists, so reset from DNF
							
							// some interesting stats
							if (stats.day1.set2TimeOfFirstRun == "")
							{
								stats.day1.set2TimeOfFirstRun = run.startTime;
							}
							stats.day1.set2TimeOfLastRun = run.startTime;
							stats.day1.set2NumberOfRuns += 1;
							stats.day1.set2TotalTime += run.time;
							drv.Day1Set2Info += String.Format("{0,5:#.000}", run.time);
							if (string.IsNullOrEmpty(run.penalty) == false)
							{
								drv.Day1Set2Info += "+" + run.penalty;
							}
							drv.Day1Set2Info += ";";
							
							drv.Day1Set2.runs.Add(run);	// Add this run to the driver's list
							
							if ((drv.Day1Set2.runs.Count <= drv.maxOfficialRuns) && (run.adjustedRawTime < drv.Day1Set2.bestRAW))
							{
								drv.Day1Set2.bestRAW = run.adjustedRawTime;
								drv.Day1Set2.bestRun = run;
							}
							if ((drv.Day1Set2.runs.Count <= drv.maxOfficialRuns) && (run.PAXtime < drv.Day1Set2.bestPAX))
							{
								drv.Day1Set2.bestPAX = run.PAXtime;
							}
							drv.coneCount += run.coneCount;
							if (run.adjustedRawTime >= DNFvalue)
							{
								string dnfSeconds = configData.GetField("secondsForDNF", "Value");
								int DNFpenalty;
								if (int.TryParse(dnfSeconds, out DNFpenalty) == false)
								{
									DNFpenalty = 200;
								}
								drv.Day1Set2.bestSUM += DNFpenalty;	// We count a DNF as 200 seconds
							}
							else
							{
								drv.Day1Set2.bestSUM += run.adjustedRawTime;
							}
						}
						else	// day 2
						{
							if (drv.Day2Set2.bestSUM == DNFvalue)
								drv.Day2Set2.bestSUM = 0.0;	// Actually exists, so reset from DNF
							
							// some interesting stats
							if (stats.day2.set2TimeOfFirstRun == "")
							{
								stats.day2.set2TimeOfFirstRun = run.startTime;
							}
							stats.day2.set2TimeOfLastRun = run.startTime;
							stats.day2.set2NumberOfRuns += 1;
							stats.day2.set2TotalTime += run.time;
							drv.Day2Set2Info += String.Format("{0,5:#.000}", run.time);
							if (string.IsNullOrEmpty(run.penalty) == false)
							{
								drv.Day2Set2Info += "+" + run.penalty;
							}
							drv.Day2Set2Info += ";";
							
							drv.Day2Set2.runs.Add(run);	// Add this run to the driver's list
							
							if ((drv.Day2Set2.runs.Count <= drv.maxOfficialRuns) && (run.adjustedRawTime < drv.Day2Set2.bestRAW))
							{
								drv.Day2Set2.bestRAW = run.adjustedRawTime;
								drv.Day2Set2.bestRun = run;
							}
							if ((drv.Day2Set2.runs.Count <= drv.maxOfficialRuns) && (run.PAXtime < drv.Day2Set2.bestPAX))
							{
								drv.Day2Set2.bestPAX = run.PAXtime;
							}
							drv.coneCount += run.coneCount;
							if (run.adjustedRawTime >= DNFvalue)
							{
								string dnfSeconds = configData.GetField("secondsForDNF", "Value");
								int DNFpenalty;
								if (int.TryParse(dnfSeconds, out DNFpenalty) == false)
								{
									DNFpenalty = 200;
								}
								drv.Day2Set2.bestSUM += DNFpenalty;	// We count a DNF as 200 seconds
							}
							else
							{
								drv.Day2Set2.bestSUM += run.adjustedRawTime;
							}
						}
					}
					else	// set 3 or higher
					{
						if (dayNumber == 1)
						{
							if (drv.Day1Set3.bestSUM == DNFvalue)
								drv.Day1Set3.bestSUM = 0.0;	// Actually exists, so reset from DNF
							
							// some interesting stats
							if (stats.day1.set3TimeOfFirstRun == "")
							{
								stats.day1.set3TimeOfFirstRun = run.startTime;
							}
							stats.day1.set3TimeOfLastRun = run.startTime;
							stats.day1.set3NumberOfRuns += 1;
							stats.day1.set3TotalTime += run.time;
							drv.Day1Set3Info += String.Format("{0,5:#.000}", run.time);
							if (string.IsNullOrEmpty(run.penalty) == false)
							{
								drv.Day1Set3Info += "+" + run.penalty;
							}
							drv.Day1Set3Info += ";";
							
							drv.Day1Set3.runs.Add(run);	// Add this run to the driver's list
							
							if ((drv.Day1Set3.runs.Count <= drv.maxOfficialRuns) && (run.adjustedRawTime < drv.Day1Set3.bestRAW))
							{
								drv.Day1Set3.bestRAW = run.adjustedRawTime;
								drv.Day1Set3.bestRun = run;
							}
							if ((drv.Day1Set3.runs.Count <= drv.maxOfficialRuns) && (run.PAXtime < drv.Day1Set3.bestPAX))
							{
								drv.Day1Set3.bestPAX = run.PAXtime;
							}
							drv.coneCount += run.coneCount;
							if (run.adjustedRawTime >= DNFvalue)
							{
								string dnfSeconds = configData.GetField("secondsForDNF", "Value");
								int DNFpenalty;
								if (int.TryParse(dnfSeconds, out DNFpenalty) == false)
								{
									DNFpenalty = 200;
								}
								drv.Day1Set3.bestSUM += DNFpenalty;	// We count a DNF as 200 seconds
							}
							else
							{
								drv.Day1Set3.bestSUM += run.adjustedRawTime;
							}
						}
						else	// day 2
						{
							if (drv.Day2Set3.bestSUM == DNFvalue)
								drv.Day2Set3.bestSUM = 0.0;	// Actually exists, so reset from DNF
							
							// some interesting stats
							if (stats.day2.set3TimeOfFirstRun == "")
							{
								stats.day2.set3TimeOfFirstRun = run.startTime;
							}
							stats.day2.set3TimeOfLastRun = run.startTime;
							stats.day2.set3NumberOfRuns += 1;
							stats.day2.set3TotalTime += run.time;
							drv.Day2Set3Info += String.Format("{0,5:#.000}", run.time);
							if (string.IsNullOrEmpty(run.penalty) == false)
							{
								drv.Day2Set3Info += "+" + run.penalty;
							}
							drv.Day2Set3Info += ";";
							
							drv.Day2Set3.runs.Add(run);	// Add this run to the driver's list
							
							if ((drv.Day2Set3.runs.Count <= drv.maxOfficialRuns) && (run.adjustedRawTime < drv.Day2Set3.bestRAW))
							{
								drv.Day2Set3.bestRAW = run.adjustedRawTime;
								drv.Day1Set3.bestRun = run;
							}
							if ((drv.Day2Set3.runs.Count <= drv.maxOfficialRuns) && (run.PAXtime < drv.Day2Set3.bestPAX))
							{
								drv.Day2Set3.bestPAX = run.PAXtime;
							}
							drv.coneCount += run.coneCount;
							if (run.adjustedRawTime >= DNFvalue)
							{
								string dnfSeconds = configData.GetField("secondsForDNF", "Value");
								int DNFpenalty;
								if (int.TryParse(dnfSeconds, out DNFpenalty) == false)
								{
									DNFpenalty = 200;
								}
								drv.Day2Set3.bestSUM += DNFpenalty;	// We count a DNF as 200 seconds
							}
							else
							{
								drv.Day2Set3.bestSUM += run.adjustedRawTime;
							}
						}
					}
				}
				// Add to the dictionary
				if (scores.ContainsKey(number))
				{
					scores[number] = drv;
				}
				else
				{
					scores.Add(number,drv);
				}
				
			}
		}
		// ---------------------------------------------------------------------------
		// Fix up run data
		public static void calcRunTimes(scoreArgs args)
		{
			var myList = new List<KeyValuePair<string, driverScoreData>>(scores);
			
			foreach (KeyValuePair<string, driverScoreData> driver in myList)
			{
				if (driver.Value.lastName == "")
					driver.Value.lastName = "Unknown";
				if (driver.Value.carDescription.Length > 20)
				{
					driver.Value.carDescription = driver.Value.carDescription.Substring(0,20);
				}
				
				if (args.set1Only == true)
				{
					// We use both days if they exist
					if ((driver.Value.Day2Set1.runs.Count <= 0) && (string.IsNullOrEmpty(args.day2)))
					{
						driver.Value.scoreData.bestRAW = driver.Value.Day1Set1.bestRAW;
						driver.Value.scoreData.bestPAX = driver.Value.Day1Set1.bestPAX;
						driver.Value.scoreData.bestSUM = driver.Value.Day1Set1.bestRAW;
					}
					else
					{
						driver.Value.scoreData.bestRAW = driver.Value.Day1Set1.bestRAW + driver.Value.Day2Set1.bestRAW;
						driver.Value.scoreData.bestPAX = driver.Value.Day1Set1.bestPAX + driver.Value.Day2Set1.bestPAX;
						driver.Value.scoreData.bestSUM = driver.Value.Day1Set1.bestRAW + driver.Value.Day2Set1.bestRAW;
					}
				}
				else if (args.set2Only == true)
				{
					// We use both days if they exist
					if ((driver.Value.Day2Set2.runs.Count <= 0) && (string.IsNullOrEmpty(args.day2)))
					{
						driver.Value.scoreData.bestRAW = driver.Value.Day1Set2.bestRAW;
						driver.Value.scoreData.bestPAX = driver.Value.Day1Set2.bestPAX;
						driver.Value.scoreData.bestSUM = driver.Value.Day1Set2.bestRAW;
					}
					else
					{
						driver.Value.scoreData.bestRAW = driver.Value.Day1Set2.bestRAW + driver.Value.Day2Set2.bestRAW;
						driver.Value.scoreData.bestPAX = driver.Value.Day1Set2.bestPAX + driver.Value.Day2Set2.bestPAX;
						driver.Value.scoreData.bestSUM = driver.Value.Day1Set2.bestRAW + driver.Value.Day2Set2.bestRAW;
					}
				}
				else if (args.set1PlusSet2 == true)
				{
					// both sets added together
					// We use both days if they exist
					if ((driver.Value.Day2Set1.runs.Count <= 0) && (string.IsNullOrEmpty(args.day2)))
					{
						driver.Value.scoreData.bestRAW = driver.Value.Day1Set1.bestRAW + driver.Value.Day1Set2.bestRAW;
						driver.Value.scoreData.bestPAX = driver.Value.Day1Set1.bestPAX + driver.Value.Day1Set2.bestPAX;
						driver.Value.scoreData.bestSUM = driver.Value.Day1Set1.bestRAW + driver.Value.Day1Set2.bestRAW;
					}
					else
					{
						// two days, 4 sets
						driver.Value.scoreData.bestRAW = driver.Value.Day1Set1.bestRAW + driver.Value.Day1Set2.bestRAW +
							driver.Value.Day2Set1.bestRAW + driver.Value.Day2Set2.bestRAW;
						driver.Value.scoreData.bestPAX = driver.Value.Day1Set1.bestPAX + driver.Value.Day1Set2.bestPAX +
							driver.Value.Day2Set1.bestPAX + driver.Value.Day2Set2.bestPAX;
						driver.Value.scoreData.bestSUM = driver.Value.Day1Set1.bestRAW + driver.Value.Day1Set2.bestRAW +
							driver.Value.Day2Set1.bestRAW + driver.Value.Day2Set2.bestRAW;
					}
				}
				else if (args.bestSingleRun == true)
				{
					// We use the best run of the official sets
					if ((driver.Value.Day2Set1.runs.Count <= 0) && (string.IsNullOrEmpty(args.day2)))
					{
						// best run of the two sets
						if (driver.Value.Day1Set1.bestPAX < driver.Value.Day1Set2.bestPAX)
						{
							driver.Value.scoreData.bestPAX = driver.Value.Day1Set1.bestPAX;
						}
						else
						{
							driver.Value.scoreData.bestPAX = driver.Value.Day1Set2.bestPAX;
						}
						if (driver.Value.Day1Set1.bestRAW < driver.Value.Day1Set2.bestRAW)
						{
							driver.Value.scoreData.bestRAW = driver.Value.Day1Set1.bestRAW;
						}
						else
						{
							driver.Value.scoreData.bestRAW = driver.Value.Day1Set2.bestRAW;
						}
						if (driver.Value.Day1Set1.bestSUM < driver.Value.Day1Set2.bestSUM)
						{
							driver.Value.scoreData.bestSUM = driver.Value.Day1Set1.bestSUM;
						}
						else
						{
							driver.Value.scoreData.bestSUM = driver.Value.Day1Set2.bestSUM;
						}
					}
					else
					{
						// 2 day event, so we total the best run from each day
						// best run of the two sets
						if (driver.Value.Day1Set1.bestPAX < driver.Value.Day1Set2.bestPAX)
						{
							driver.Value.scoreData.bestPAX = driver.Value.Day1Set1.bestPAX;
							if (driver.Value.Day2Set1.bestPAX < driver.Value.Day2Set2.bestPAX)
							{
								driver.Value.scoreData.bestPAX += driver.Value.Day2Set1.bestPAX;
							}
							else
							{
								driver.Value.scoreData.bestPAX += driver.Value.Day2Set2.bestPAX;
							}
						}
						else
						{
							driver.Value.scoreData.bestPAX = driver.Value.Day1Set2.bestPAX;
							if (driver.Value.Day2Set1.bestPAX < driver.Value.Day2Set2.bestPAX)
							{
								driver.Value.scoreData.bestPAX += driver.Value.Day2Set1.bestPAX;
							}
							else
							{
								driver.Value.scoreData.bestPAX += driver.Value.Day2Set2.bestPAX;
							}
						}
						if (driver.Value.Day1Set1.bestRAW < driver.Value.Day1Set2.bestRAW)
						{
							driver.Value.scoreData.bestRAW = driver.Value.Day1Set1.bestRAW;
							if (driver.Value.Day2Set1.bestRAW < driver.Value.Day2Set2.bestRAW)
							{
								driver.Value.scoreData.bestRAW += driver.Value.Day2Set1.bestRAW;
							}
							else
							{
								driver.Value.scoreData.bestRAW += driver.Value.Day2Set2.bestRAW;
							}
						}
						else
						{
							driver.Value.scoreData.bestRAW = driver.Value.Day1Set2.bestRAW;
							if (driver.Value.Day2Set1.bestRAW < driver.Value.Day2Set2.bestRAW)
							{
								driver.Value.scoreData.bestRAW += driver.Value.Day2Set1.bestRAW;
							}
							else
							{
								driver.Value.scoreData.bestRAW += driver.Value.Day2Set2.bestRAW;
							}
						}
					}
				}
				// Adjust PAX for DNS values
				if (driver.Value.scoreData.bestRAW >= DNFvalue)
				{
					driver.Value.scoreData.bestRAW = DNFvalue;
				}
			}
		}
		// ---------------------------------------------------------------------------
		// Add penalties and return adjusted time
		// DNF is 1000
		// Use PAX of 1.0 to get adjusted raw time
		public static double adjustTime(double rawtime, string penalty, double PAX)
		{
			if (penalty == "DNF")
			{
				return DNFvalue;
			}
			int conePenalty = 2;
			string conePen = configData.GetField("ConePenalySeconds", "Value");
			if (Int32.TryParse(conePen, out conePenalty) == false)
			{
				conePenalty = 2;
			}

			int cones;
			double totalTime = rawtime * PAX;
			if (int.TryParse(penalty, out cones) == true)
			{
				if (ConesGetPAXed == true)
				{
					totalTime = (rawtime + (cones * conePenalty)) * PAX;
				}
				else
				{
					totalTime = (rawtime * PAX) + (cones * conePenalty);
				}
			}
			return totalTime;
		}
		// ---------------------------------------------------------------------------
		public static int calcCones(string penalty)
		{
			int cones;
			if (int.TryParse(penalty, out cones) == true)
			{
				return cones;
			}
			return 0;
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
		// Calculate the number of points given the place (1..<n>)
		public static int calcClassPoints(int position)
		{
			string pointAwards = configData.GetField("PointAwards", "Value");
			if (pointAwards.Length > 0)
			{
				string[] points = pointAwards.Split(';');
				position -= 1; // make it 0 based
				if (position >= points.Length)
				{
					return 1;
				}
				string pointString = points[position];
				int pointValue = 1;
				if (int.TryParse(pointString, out pointValue) == false)
				{
					return 1;
				}
				return pointValue;
			}
			// No config -- just use these commonly used values
			if (position == 1) return 20;
			if (position == 2) return 16;
			if (position == 3) return 14;
			if (position == 4) return 12;
			if (position == 5) return 11;
			if (position == 6) return 10;
			if (position == 7) return 9;
			if (position == 8) return 8;
			if (position == 9) return 7;
			if (position == 10) return 6;
			if (position == 11) return 5;
			if (position == 12) return 4;
			if (position == 13) return 3;
			if (position == 14) return 2;
			if (position == 15) return 1;
			return 0;
		}
		
	}
}