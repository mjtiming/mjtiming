/*
 * Created by Murray Peterson
 * 
 * Compile command:
 * C:\Windows\Microsoft.NET\Framework\v3.5\Csc.exe /reference:c:\mjtiming\bin\mjcommon.dll /platform:x86 htmlScores.cs
 * You will need to copy the resulting htmlScores.exe into mjtiming\bin before it will work.
 */

// Compile command:
// C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe /reference:c:\mjtiming\bin\mjcommon.dll /platform:x86 sample.cs
// Note that the resulting sample.exe will not work unless it is copied into mjtiming\bin

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace RaceBeam  // this is required to get easy reference to the datatypes
{
	static class Htmlscore
	{
		// returned list of all driver's score data
		public static Dictionary<string,scoreCalcs.driverScoreData> scores;
		// Returned list of team scores
		public static List<scoreCalcs.teamData> teamScores;
		// Returned list of all class data, sorted by sort order given in file
		public static SortedDictionary<int,scoreCalcs.paxInfo> sortedClassList;
		// Returned statistics
		public static scoreCalcs.statsDataClass stats;
		
		public static CSVData configData = new CSVData();
		public static bool showLastName = false;
		private static string htmlStyle = "";
		private static string htmlHeader = "<html><head>%STYLE%</head><body>";
		private const string htmlFooter = @"</body></html>";
		private const string htmlTableHeader = @"
<table width=""100%"" cellpadding=""3"" style=""border-collapse: collapse"" border=""1"" align=""left"">
<th class=hdr1 colspan=""99"" nowrap align=""center"">%TABLENAME%</th>
";
		private const string htmlTableFooter = @"</table>";
		// ---------------------------------------------------------------------------
		public static void Usage()
		{
			Console.WriteLine("Usage: htmlscore -day1 <day1 date> -day2 <day2 date> -bestsinglerun -set1only -set2only -runtimes -rawtimes -paxtimes -teams -conecounts -classtimes -xgrouptimes -rookie -maxofficialruns <# runs> -classfile <path to class.csv file> -title <string> -path <path to event data folder>");
			Environment.Exit(0);
		}
		// ---------------------------------------------------------------------------
		public static void Main(string[] args)
		{
            // parse command line arguments
            // default to 1 day scoring, today's date, first set only
            scoreArgs scoreArgs = new scoreArgs();
            var argblock = scoreArgs;
			argblock.day1 = DateTime.Now.ToString("yyyy_MM_dd");
			string configFilename;
			string configFolder = argblock.configFolder;

			configFolder = Process.GetCurrentProcess().MainModule.FileName;
			configFolder = Path.GetDirectoryName(configFolder);
			configFolder = configFolder + "\\..\\config";

			configFilename =  configFolder + "\\configData.csv";
			string err = configData.LoadData(configFilename,',',"Parameter");
			if (err != "")
			{
				Console.WriteLine("Unable to load config file: " + err);
				Usage();
			}
			
			if (configData.GetField("ShowLastName", "Value").Contains("Y"))
			{
				showLastName = true;
			}
			for (int i = 0; i < args.Length; i++)
			{
				if ((args[i] == "-h") | (args[i] == "-help") | (args[i] == "-?"))
				{
					Usage();
				}
				else if (args[i] == "-day1")
				{
					i += 1;
					argblock.day1 = args[i];
				}
				else if (args[i] == "-day2")
				{
					i += 1;
					argblock.day2 = args[i];
				}
				else if (args[i] == "-classfile")
				{
					i += 1;
					argblock.classFile = args[i];
				}
				else if (args[i] == "-title")
				{
					i += 1;
					argblock.title = @"<h1 align=""center"">" + args[i] + "</h1>";
				}
				else if (args[i] == "-path")
				{
					i += 1;
					argblock.eventFolder = args[i];
				}
				else if (args[i] == "-set1only")
				{
					argblock.set1Only = true;
				}
				else if (args[i] == "-set2only")
				{
					argblock.set2Only = true;
				}
				else if (args[i] == "-bestsinglerun")
				{
					argblock.bestSingleRun = true;
				}
				else if (args[i] == "-set1plusset2")
				{
					argblock.set1PlusSet2 = true;
				}
				else if (args[i] == "-runtimes")
				{
					argblock.showRunTimes = true;
				}
				else if (args[i] == "-rawtimes")
				{
					argblock.showRawTimes = true;
				}
				else if (args[i] == "-paxtimes")
				{
					argblock.showPaxTimes = true;
				}
				else if (args[i] == "-rookie")
				{
					argblock.showRookie = true;
				}
				else if (args[i] == "-classtimes")
				{
					argblock.showClassTimes = true;
				}
				else if (args[i] == "-teams")
				{
					argblock.showTeams = true;
				}
				else if (args[i] == "-conecounts")
				{
					argblock.showConeCounts = true;
				}
				else if (args[i] == "-eventname")
				{
					// do nothing
				}
				else if (args[i] == "-maxofficialruns")
				{
					i += 1;
					if (int.TryParse(args[i], out argblock.maxOfficialRuns) == false)
					{
						argblock.maxOfficialRuns = 999;
					}
				}
				else
				{
					Usage();
				}
			}
			// Do the scoring calcs ad get the results back
			err = scoreCalcs.doScore(argblock, out scores, out teamScores, out stats, out sortedClassList);
			
			if (string.IsNullOrEmpty(err) == false)
			{
				Console.WriteLine(err);
				return;
			}
			// Now generate results in format of our choice
			// Let the cmd script redirect to the file
			
			string styleFilename =  configFolder + "\\_scoreStyles.css";
			try
			{
				htmlStyle = File.ReadAllText(styleFilename);
			}
			catch
			{
				// No style sheet there, so don't use  style sheet
				htmlStyle = "";
			}
			htmlHeader = htmlHeader.Replace("%STYLE%", htmlStyle);
			
			string results;
			string pathStart;
			if (String.IsNullOrEmpty(argblock.day2))
			{
				pathStart = argblock.eventFolder + "\\" + argblock.day1;
			}
			else
			{
				pathStart = argblock.eventFolder + "\\" + argblock.day2 + "__2-day";
			}
			if (argblock.showRawTimes)
			{
				results = htmlHeader + argblock.title + RawTimes(argblock) + htmlFooter;
				string path = pathStart + "__htmlRAWScores.html";
				System.IO.File.WriteAllText(path, results);
			}
			
			if (argblock.showPaxTimes)
			{
				results = htmlHeader + argblock.title + PaxTimes(argblock) + htmlFooter;
				string path = pathStart + "__htmlPAXScores.html";
				System.IO.File.WriteAllText(path, results);
			}
			
			if (argblock.showClassTimes)
			{
				results = htmlHeader + argblock.title + ClassTimes(argblock) + htmlFooter;
				string path = pathStart + "__htmlClassScores.html";
				System.IO.File.WriteAllText(path, results);
			}
			if (argblock.showRunTimes)
			{
				results = htmlHeader + argblock.title + RunTimes(argblock) + htmlFooter;
				// Always show statistics
				results += Statistics(argblock);
				string path = pathStart + "__htmlRunTimes.html";
				System.IO.File.WriteAllText(path, results);
			}
			if (argblock.showTeams)
			{
				results = htmlHeader + argblock.title + TeamTimes(argblock) + htmlFooter;
				string path = pathStart + "__htmlTeamScores.html";
				System.IO.File.WriteAllText(path, results);
			}
			if (argblock.showConeCounts)
			{
				results = htmlHeader + argblock.title + ConeCounts(argblock) + htmlFooter;
				string path = pathStart + "__htmlConeCounts.html";
				System.IO.File.WriteAllText(path, results);
			}
			
		}
		// ---------------------------------------------------------------------------
		public static string RawTimes(scoreArgs args)
		{
			string results = htmlTableHeader;
			results = results.Replace("%TABLENAME%", "Overall ranking by RAW time");
			results += @"<tr class=""hdr2""><td>Rank</td><td>Car#</td><td>Class</td><td>Driver</td><td>Car</td><td>Raw Time</td><td>Score</td></tr>";
			
			string rookieResults = htmlTableHeader;
			rookieResults = rookieResults.Replace("%TABLENAME%", "Rookie ranking by RAW time");
			rookieResults += @"<tr class=""hdr2""><td>Rank</td><td>Car#</td><td>Class</td><td>Driver</td><td>Car</td><td>Raw Time</td><td>Score</td></tr>";
			
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			// Sort by raw time
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestRAW.CompareTo(nextPair.Value.scoreData.bestRAW);
			            }
			           );
			
			foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
			{
				string driverRAW = "";
				driverRAW = driver.Value.scoreData.bestRAW.ToString("#0.000");
				if (driver.Value.scoreData.bestRAW >= scoreCalcs.DNFvalue)
				{
					driverRAW = "DNS";
				}

				string driverName = driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1);
				if (showLastName)
				{
					driverName = driver.Value.firstName + " " + driver.Value.lastName;
				}
				string line = String.Format("<tr class=data><td>{0,4}</td><td>{1,4}</td><td>{2,3}</td><td>{3,3}</td><td>{4,5}</td><td>{5,8:#.000}</td><td>{6,7:#0.000}</td>\r\n",
				                            driver.Value.scoreData.RAWrank,
				                            driver.Value.number,
				                            driver.Value.carClass,
				                            driverName,
				                            driver.Value.carDescription,
				                            driverRAW,
				                            driver.Value.scoreData.RAWscore
				                           );
				results += String.Format(line);
				if (driver.Value.rookie)
				{
					line = String.Format("<tr class=data><td>{0,4}</td><td>{1,4}</td><td>{2,3}</td><td>{3,3}</td><td>{4,5}</td><td>{5,8:#.000}</td><td>{6,7:#0.000}</td>\r\n",
					                     driver.Value.scoreData.RAWrookieRank,
					                     driver.Value.number,
					                     driver.Value.carClass,
					                     driverName,
					                     driver.Value.carDescription,
					                     driverRAW,
					                     driver.Value.scoreData.RAWrookieScore
					                    );
					
					rookieResults += line;
				}
			}
			results += htmlTableFooter;
			rookieResults += htmlTableFooter;
			if (args.showRookie)
			{
				return results + rookieResults;
			}
			else
			{
				return results;
			}
		}
		// ---------------------------------------------------------------------------
		// Return a printable text string for PAX data
		public static string PaxTimes(scoreArgs args)
		{
			string results = htmlTableHeader;
			results = results.Replace("%TABLENAME%", "Overall ranking by PAX time");
			results += @"<tr class=""hdr2""><td>Rank</td><td>Car#</td><td>Class</td><td>Driver</td><td>Car</td><td>Raw Time</td><td>PAX #</td><td>PAX Time</td><td>Score</td></tr>";
			
			string rookieResults = htmlTableHeader;
			rookieResults = rookieResults.Replace("%TABLENAME%", "Rookie ranking by PAX time");
			rookieResults += @"<tr class=""hdr2""><td>Rank</td><td>Car#</td><td>Class</td><td>Driver</td><td>Car</td><td>Raw Time</td><td>PAX #</td><td>PAX Time</td><td>Score</td></tr>";
			
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			// Sort by pax time
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestPAX.CompareTo(nextPair.Value.scoreData.bestPAX);
			            }
			           );
			
			
			foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
			{
				string driverName = driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1);
				if (showLastName)
				{
					driverName = driver.Value.firstName + " " + driver.Value.lastName;
				}
				results += String.Format("<tr class=data><td>{0,4}</td><td>{1,4}</td><td>{2,3}</td><td>{3,3}</td><td>{4,5}</td><td>{5,5:#.000}</td><td>{6,5:#.000}</td><td> {7,5:#.000}</td><td>{8,5:#0.000}</td>\r\n",
				                         driver.Value.scoreData.PAXrank,
				                         driver.Value.number,
				                         driver.Value.carClass,
				                         driverName,
				                         driver.Value.carDescription,
				                         driver.Value.scoreData.bestRAW < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestRAW.ToString("#.000") : "DNS",
				                         driver.Value.pax,
				                         driver.Value.scoreData.bestPAX < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestPAX.ToString("#.000") : "DNS",
				                         driver.Value.scoreData.PAXscore
				                        );
				if (driver.Value.rookie)
				{
					rookieResults += String.Format("<tr class=data><td>{0,4}</td><td>{1,4}</td><td>{2,3}</td><td>{3,3}</td><td>{4,5}</td><td>{5,5:#.000}</td><td>{6,5:#.000}</td><td>{7,5:#.000}</td><td>{8,5:#0.000}</td>\r\n",
					                               driver.Value.scoreData.PAXRookieRank,
					                               driver.Value.number,
					                               driver.Value.carClass,
					                               driverName,
					                               driver.Value.carDescription,
					                               driver.Value.scoreData.bestRAW < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestRAW.ToString("#.000") : "DNS",
					                               driver.Value.pax,
					                               driver.Value.scoreData.bestPAX < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestPAX.ToString("#.000") : "DNS",
					                               driver.Value.scoreData.PAXrookieScore
					                              );
				}
			}
			if (args.showRookie)
			{
				return results + rookieResults;
			}
			else
			{
				return results;
			}
		}
		// ---------------------------------------------------------------------------
		public static int GetLeadingInt(string input)
		{
			var i = 0;
			input = input.Trim();
			while (i < input.Length && char.IsDigit(input[i])) i++;

			input = input.Substring(0, i);
            if (int.TryParse(input, out int value) == false)
            {
                return 0;
            }
            return value;
		}
		// ---------------------------------------------------------------------------
		// print out run data
		public static string RunTimes(scoreArgs args)
		{
			string results = htmlTableHeader;
			results = results.Replace("%TABLENAME%", "Run times (ordered by car number)");
			results += @"<tr class=""hdr2""><td>Car #</td><td>Member</td><td>Rookie</td><td>Class</td><td>Driver</td><td>Car</td><td>Sponsor</td></tr>";
			
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			// Sort by car number
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	int d1Number,d2Number;
			            	d1Number = GetLeadingInt(firstPair.Value.number);
			            	d2Number = GetLeadingInt(nextPair.Value.number);
			            	return d1Number.CompareTo(d2Number);
			            }
			           );
			
			// First we need to cycle through all our drivers and decide how many sets there were on each day
			// We need to do this because a single driver may have skipped a day or a set
			int numDays = 1;
			int day1NumSets = 1;	// min # of sets in a day
			int day2NumSets = 0;
			if (string.IsNullOrEmpty(args.day2) == false)
			{
				numDays = 2;
				day2NumSets = 1;	// min # of sets in a day
			}
			foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
			{
				if ((driver.Value.Day1Set2.runs.Count > 0) && (day1NumSets < 2))
				{
					day1NumSets = 2;
				}
				if ((driver.Value.Day1Set3.runs.Count > 0) && (day1NumSets < 3))
				{
					day1NumSets = 3;
				}
				if ((numDays == 2) && (driver.Value.Day2Set2.runs.Count > 0) && (day2NumSets < 2))
				{
					day2NumSets = 2;
				}
				if ((numDays == 2) && (driver.Value.Day2Set3.runs.Count > 0) && (day2NumSets < 3))
				{
					day2NumSets = 3;
				}
			}
			
			foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
			{
				string driverName = driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1);
				if (showLastName)
				{
					driverName = driver.Value.firstName + " " + driver.Value.lastName;
				}
				string line = String.Format("<tr class=data><td>{0,4}</td><td>{1,4}</td><td>{2,3}</td><td>{3,3}</td><td>{4,5}</td><td>{5,5}</td><td>{6,5}</td></tr>\r\n",
				                            driver.Value.number,
				                            driver.Value.member,
				                            driver.Value.rookie?"Yes":"No",
				                            driver.Value.carClass,
				                            driverName,
				                            driver.Value.carDescription,
				                            driver.Value.sponsor
				                           );
				results += String.Format(line);
				results += "<td colspan=\"1\"></td><td colspan=\"99\"><table width=\"100%\" cellpadding=\"2\" style=\"border-collapse: collapse\" border=\"1\" align=\"left\">";
				if (day1NumSets >= 1)
				{
					results += PrintSet(args,driver.Value.Day1Set1,1,1);
				}
				if (day1NumSets >= 2)
				{
					results += PrintSet(args,driver.Value.Day1Set2,1,2);
				}
				if (day1NumSets >= 3)
				{
					results += PrintSet(args,driver.Value.Day1Set3,1,3);
				}
				if (day2NumSets >= 1)
				{
					results += PrintSet(args,driver.Value.Day2Set1,2,1);
				}
				if (day2NumSets >= 2)
				{
					results += PrintSet(args,driver.Value.Day2Set2,2,2);
				}
				if (day2NumSets >= 3)
				{
					results += PrintSet(args,driver.Value.Day2Set3,2,3);
				}
				
				
				if (args.set1Only == true)
				{
					results += String.Format("<tr><td class=\"hdr3\">{0,35}</td>","Total (set1 only):");
				}
				else if (args.set2Only == true)
				{
					results += String.Format("<tr><td class=\"hdr3\">{0,35}</td>","Total (set2 only):");
				}
				else if (args.bestSingleRun == true)
				{
					results += String.Format("<tr><td class=\"hdr3\">{0,35}</td>","Total (single best run):");
				}
				else
				{
					// two day scoring
					results += String.Format("<tr><td class=\"hdr3\">{0,35}</td>","Total (both sets):");
				}
				
				
				if (driver.Value.scoreData.bestRAW >= scoreCalcs.DNFvalue)
				{
					results += String.Format("<td class=\"highlight\">{0,8}</td>\r\n","DNS");
				}
				else
				{
					results += String.Format("<td class=\"highlight\">{0,8:#.000}</td>\r\n",driver.Value.scoreData.bestRAW);
				}
				results += "</table>";
			}
			results += "</table>";
			return results;
		}
		// ---------------------------------------------------------------------------
		public static string PrintSet(scoreArgs args, scoreCalcs.singleSetData setData, int dayNumber, int setNumber)
		{
			string results = String.Format("<tr><td class=\"hdr3\">Day {0,1} Set {1,1}</td>",dayNumber,setNumber);
			bool hasRuns = false;
			int startRunNumber = -1;
			foreach (var run in setData.runs)
			{
				hasRuns = true;
				if (startRunNumber < 0)
				{
					// Note that run numbers don't start at 1 -- they increment across sets
					startRunNumber = run.runNumber;
				}
				if ((run.runNumber - startRunNumber) == args.maxOfficialRuns)
				{
					results += "<td class=\"data2\">Fun--></td>";
				}
				if (run.penalty == "DNF")
				{
					if (run.time >= scoreCalcs.DNFvalue)
					{
						// didn't even run
						results += String.Format("<td class=\"data2\">{0,8:#.000}</td>","DNS");
					}
					else
					{
						results += String.Format("<td class=\"data2\">{0,8:#.000}{1,-4}</td>",run.time,"+DNF");
					}
				}
				else if (run.penalty == "")
				{
					if (run == setData.bestRun)
					{
						results += String.Format("<td class=\"highlight\">{0,8:#.000}</td>", run.time);
					}
					else
					{
						results += String.Format("<td class=\"data2\">{0,8:#.000}{1,-4}</td>", run.time,"    ");
					}
				}
				else
				{
					if (run == setData.bestRun)
					{
						results += String.Format("<td class=\"data2\">{0,8: (#.000}{1,-4}</td>", run.time, "+" + run.penalty + ")");
					}
					else
					{
						results += String.Format("<td class=\"data2\">{0,8:#.000}{1,-4}</td>", run.time, "+" + run.penalty);
					}
				}
			}
			if (hasRuns == false)
			{
				results += String.Format("<td class=\"data2\">{0,8:#.000}{1,-4}</td>","DNS   ","    ");
			}
			results += String.Format("\r\n");
			return results;
		}
		// ---------------------------------------------------------------------------
		// print out times, groups by Group, ordered within group by PAX time
		public static string ClassTimes(scoreArgs args)
		{
			string results = htmlTableHeader;
			results = results.Replace("%TABLENAME%", "Overall ranking by Group");
			
			string rookieResults = htmlTableHeader;
			rookieResults = rookieResults.Replace("%TABLENAME%", "Rookie ranking by Group");
			
			// Sort drivers by PAX time
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestPAX.CompareTo(nextPair.Value.scoreData.bestPAX);
			            }
			           );
			
			foreach (KeyValuePair<int, scoreCalcs.paxInfo> classInfo in sortedClassList)
			{
				var curClass = classInfo.Value;
				string curClassGroup = curClass.group;
				
				double bestTime = 0.0;
				
				foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
				{
					string origXgrps = driver.Value.carXGroup;
					origXgrps += ";" + driver.Value.carGroup;

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
					scoreCalcs.groupscore grpPtr = null;
					foreach (scoreCalcs.groupscore grp in driver.Value.scoreData.groupScores)
					{
						if (grp.groupName == curClass.carClass)
						{
							grpPtr = grp;
							break;
						}
					}
					if (grpPtr == null)
					{
						continue;
					}
					
					string line = "";
					
					
					line += String.Format("<tr class=\"hdr3\" nowrap align=\"center\"><td colspan=\"999\">" + curClass.carClass + " (" + curClass.description + ")</td></tr>\r\n");
					line += @"<tr class=""hdr2""><td>Rank</td><td>Car#</td><td>Class</td><td>Driver</td><td>Car</td><td>Raw Time</td><td>PAX #</td><td>PAX time</td><td>Score</td>";
					
					line += String.Format("</tr>\r\n");

					if (grpPtr.groupRank == 1)
					{
						results += line;
					}
					if ((grpPtr.groupRookieRank == 1) && (driver.Value.rookie))
					{
						rookieResults += line;
					}
					
					if (driver.Value.lastName == "")
					{
						driver.Value.lastName = "Unknown";
					}
					if (grpPtr.groupRank == 1)
					{
						bestTime = driver.Value.scoreData.bestPAX;
					}
					
					string driverRAW;
					driverRAW = driver.Value.scoreData.bestRAW.ToString("#0.000");
					if (driver.Value.scoreData.bestRAW >= scoreCalcs.DNFvalue)
					{
						driverRAW = "DNS";
					}

					string driverPAX;
					driverPAX = driver.Value.scoreData.bestPAX.ToString("#0.000");

					if (driver.Value.scoreData.bestRAW >= scoreCalcs.DNFvalue)
					{
						driverPAX = "DNS";
					}
					
					string trophyIndicator = "T";
					string rookieTrophyIndicator = "T";
					if (grpPtr.groupTrophy == true)
						trophyIndicator = "T";
					else
						trophyIndicator = "";
					if (grpPtr.groupRookieTrophy == true)
						rookieTrophyIndicator = "T";
					else
						rookieTrophyIndicator = "";
					string driverName = driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1);
					if (showLastName)
					{
						driverName = driver.Value.firstName + " " + driver.Value.lastName;
					}
					results += String.Format("<tr class=data><td>{0,4}</td><td>{1,4}</td><td>{2,3}</td><td>{3,3}</td><td>{4,5}</td><td>{5,8:#.000}</td><td>{6,9:#.000}</td><td>{7,9:#.000}</td><td>{8,7:#0.000}</td>\r\n",
					                         trophyIndicator + grpPtr.groupRank,
					                         driver.Value.number,
					                         driver.Value.carClass,
					                         driverName,
					                         driver.Value.carDescription,
					                         driverRAW,
					                         driver.Value.pax,
					                         driverPAX,
					                         grpPtr.groupScore
					                        );
					results += String.Format("\r\n");

					if (driver.Value.rookie)
					{
						rookieResults += String.Format("<tr class=data><td>{0,4}</td><td>{1,4}</td><td>{2,3}</td><td>{3,3}</td><td>{4,5}</td><td>{5,8:#.000}</td><td>{6,9:#.000}</td><td>{7,9:#.000}</td><td>{8,7:#0.000}</td>\r\n",
						                               rookieTrophyIndicator + grpPtr.groupRookieRank,
						                               driver.Value.number,
						                               driver.Value.carClass,
						                               driverName,
						                               driver.Value.carDescription,
						                               driverRAW,
						                               driver.Value.pax,
						                               driverPAX,
						                               grpPtr.groupRookieScore
						                              );
						rookieResults += String.Format("\r\n");
					}
				}
			}
			if (args.showRookie)
			{
				return results + rookieResults;
			}
			else
			{
				return results;
			}
		}
		// ---------------------------------------------------------------------------
		// Return a printable text string for cone counts
		public static string ConeCounts(scoreArgs args)
		{
			string results = htmlTableHeader;
			results = results.Replace("%TABLENAME%", "Cone counts");
			if (args == null)
            {
				return "";
            }
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			// Sort by cone counts
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	return nextPair.Value.coneCount.CompareTo(firstPair.Value.coneCount);
			            }
			           );
			
			results += @"<tr class=""hdr2""><td>Rank</td><td>Car#</td><td>Driver</td><td>Cones</td></tr>";
			
			int rank = 0;
			int lastCount = 9999;
			foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
			{
				int tcones = 0;
				tcones = driver.Value.coneCount;
				
				if (tcones <= 0)
				{
					break;
				}
				if (tcones < lastCount)
				{
					rank += 1;
				}
				string driverName = driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1);
				if (showLastName)
				{
					driverName = driver.Value.firstName + " " + driver.Value.lastName;
				}
				results += String.Format("<tr class=data><td>{0,4}</td><td>{1,4}</td><td>{2,35}</td><td>{3,4}</td></tr>\r\n",
				                         rank,
				                         driver.Value.number,
				                         driverName,
				                         tcones.ToString()
				                        );
				lastCount = tcones;
			}
			return results;
		}
		// ---------------------------------------------------------------------------
		// Team scores
		// We group rAW and PAX if given in the driver's group filed
		// If the group is something else, then we ignore the group and show the team for both raw and pax
		public static string TeamTimes(scoreArgs args)
		{
			if (args == null)
			{
				return "";
			}
			string RAWresults = htmlTableHeader;
			RAWresults = RAWresults.Replace("%TABLENAME%", "RAW team scores");
			
			string PAXresults = htmlTableHeader;
			PAXresults = PAXresults.Replace("%TABLENAME%", "PAX team scores");
			
			// We sort our teams by raw time
			teamScores.Sort(delegate(scoreCalcs.teamData first, scoreCalcs.teamData next)
			                {
			                	// disable ConvertToLambdaExpression
			                	return first.rawTotal.CompareTo(next.rawTotal);
			                }
			               );
			// now go through the sorted list and print out raw team results
			RAWresults += @"<tr class=""hdr2""><td>Rank</td><td>Team</td><td>Total RAW</td><td>Total PAX</td><td>Cones</td></tr>";
			int rank = 1;
			foreach (scoreCalcs.teamData tm in teamScores)
			{
				if (tm.teamType.StartsWith("PAX") == true)
				{
					continue;
				}
				
				RAWresults += string.Format("\r\n<tr class=data><td class=hdr2>{0,4}</td><td>{1,6}</td><td>RAW:{2,8:#.000}</td><td>PAX:{3,8:#.000}</td><td>Cones: {4}</td></tr>\r\n",
				                            rank.ToString(),
				                            tm.team,
				                            TimeOrDNS(tm.rawTotal),
				                            TimeOrDNS(tm.paxTotal),
				                            tm.coneTotal
				                           );
				RAWresults += "<td colspan=\"1\"></td><td colspan=\"99\"><table width=\"100%\" cellpadding=\"2\" style=\"border-collapse: collapse\" border=\"1\" align=\"left\">";
				rank += 1;
				foreach (scoreCalcs.driverScoreData driver in tm.teamDrivers)
				{
					string driverName = driver.firstName + " " + driver.lastName.Substring(0,1);
					if (showLastName)
					{
						driverName = driver.firstName + " " + driver.lastName;
					}
					RAWresults += string.Format("<tr class=data2><td>Driver {0,3}</td><td>{1,-14}</td><td>RAW:{2, 8:#.000}</td><td>PAX:{3, 8:#.000}</td></tr>\r\n",
					                            driver.number,
					                            driverName,
					                            TimeOrDNS(driver.scoreData.bestRAW),
					                            TimeOrDNS(driver.scoreData.bestPAX)
					                           );
				}
				RAWresults += "</table>";
			}
			RAWresults += "</table>";
			// Do the same thing for PAX teams
			// Now we sort our teams by PAX time
			teamScores.Sort(delegate(scoreCalcs.teamData first, scoreCalcs.teamData next)
			                {
			                	return first.paxTotal.CompareTo(next.paxTotal);
			                }
			               );
			
			PAXresults += @"<tr class=""hdr2""><td>Rank</td><td>Team</td><td>Total RAW</td><td>Total PAX</td><td>Cones</td></tr>";
			
			rank = 1;
			foreach (scoreCalcs.teamData tm in teamScores)
			{
				if (tm.teamType.StartsWith("RAW") == true)
				{
					continue;
				}
				
				PAXresults += string.Format("\r\n<tr class=data><td class=hdr2>{0,4}</td><td>{1,6}</td><td>RAW:{2,8:#.000}</td><td>PAX:{3,8:#.000}</td><td>Cones: {4}</td></tr>\r\n",
				                            rank.ToString(),
				                            tm.team,
				                            TimeOrDNS(tm.rawTotal),
				                            TimeOrDNS(tm.paxTotal),
				                            tm.coneTotal
				                           );
				PAXresults += "<td colspan=\"1\"></td><td colspan=\"99\"><table width=\"100%\" cellpadding=\"2\" style=\"border-collapse: collapse\" border=\"1\" align=\"left\">";
				rank += 1;
				foreach (scoreCalcs.driverScoreData driver in tm.teamDrivers)
				{
					string driverName = driver.firstName + " " + driver.lastName.Substring(0,1);
					if (showLastName)
					{
						driverName = driver.firstName + " " + driver.lastName;
					}
					PAXresults += string.Format("<tr class=data2><td>Driver {0,3}</td><td>{1,-14}</td><td>RAW:{2, 8:#.000}</td><td>PAX:{3, 8:#.000}</td></tr>\r\n",
					                            driver.number,
					                            driverName,
					                            TimeOrDNS(driver.scoreData.bestRAW),
					                            TimeOrDNS(driver.scoreData.bestPAX)
					                           );
				}
				PAXresults += "</table>";
			}
			PAXresults += "</table>";
			return RAWresults + PAXresults;
		}
        // ---------------------------------------------------------------------------
#pragma warning disable IDE0060 // Remove unused parameter
        public static string Statistics(scoreArgs args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
			string results = htmlTableHeader;
			results = results.Replace("%TABLENAME%", "Statistics");
			if (stats.day1.set1NumberOfRuns > 0)
			{
				results += String.Format("<tr class=data><td>Day1, Set1: </td><td>First run: {0} </td><td>Last run: {1} </td><td>Number of runs: {2} </td><td>Avg run time: {3,5:#0.00}</td>\r\n",
				                         stats.day1.set1TimeOfFirstRun, stats.day1.set1TimeOfLastRun, stats.day1.set1NumberOfRuns, stats.day1.set1TotalTime / stats.day1.set1NumberOfRuns);
			}
			if (stats.day1.set2NumberOfRuns > 0)
			{
				results += String.Format("<tr class=data><td>Day1, Set2: </td><td>First run: {0} </td><td>Last run: {1} </td><td>Number of runs: {2} </td><td>Avg run time: {3,5:#0.00}</td>\r\n",
				                         stats.day1.set2TimeOfFirstRun, stats.day1.set2TimeOfLastRun, stats.day1.set2NumberOfRuns, stats.day1.set2TotalTime / stats.day1.set2NumberOfRuns);
			}
			if (stats.day1.set3NumberOfRuns > 0)
			{
				results += String.Format("<tr class=data><td>Day1, Fun runs: </td><td>First run: {0} </td><td>Last run: {1} </td><td>Number of runs: {2} </td><td>Avg run time: {3,5:#0.00}</td>\r\n",
				                         stats.day1.set3TimeOfFirstRun, stats.day1.set3TimeOfLastRun, stats.day1.set3NumberOfRuns, stats.day1.set3TotalTime / stats.day1.set3NumberOfRuns);
			}
			// Day 2 if we have one
			if (stats.day2.set1NumberOfRuns > 0)
			{
				results += String.Format("<tr class=data><td>Day2, Set1: </td><td>First run: {0} </td><td>Last run: {1} </td><td>Number of runs: {2} </td><td>Avg run time: {3,5:#0.00}</td>\r\n",
				                         stats.day2.set1TimeOfFirstRun, stats.day2.set1TimeOfLastRun, stats.day2.set1NumberOfRuns, stats.day2.set1TotalTime / stats.day2.set1NumberOfRuns);
			}
			if (stats.day2.set2NumberOfRuns > 0)
			{
				results += String.Format("<tr class=data><td>Day2, Set2: </td><td>First run: {0} </td><td>Last run: {1} </td><td>Number of runs: {2} </td><td>Avg run time: {3,5:#0.00}</td>\r\n",
				                         stats.day2.set2TimeOfFirstRun, stats.day2.set2TimeOfLastRun, stats.day2.set2NumberOfRuns, stats.day2.set2TotalTime / stats.day2.set2NumberOfRuns);
			}
			if (stats.day2.set3NumberOfRuns > 0)
			{
				results += String.Format("<tr class=data><td>Day2, Fun runs: </td><td>First run: {0} </td><td>Last run: {1} </td><td>Number of runs: {2} </td><td>Avg run time: {3,5:#0.00}</td>\r\n",
				                         stats.day2.set3TimeOfFirstRun, stats.day2.set3TimeOfLastRun, stats.day2.set3NumberOfRuns, stats.day2.set3TotalTime / stats.day2.set3NumberOfRuns);
			}
			return results;
		}
		// ---------------------------------------------------------------------------
		// Times > DNFvalue denote a DNS
		public static string TimeOrDNS(double time)
		{
			if (time >= scoreCalcs.DNFvalue)
			{
				// didn't even run
				return "DNS   ";
			}
			else
			{
				return String.Format("{0,8:#.000}",time);
			}
		}
		// ---------------------------------------------------------------------------
	}
}