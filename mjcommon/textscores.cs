/*
 * Generates score output in text format
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
	public static class textScores
	{
		// returned list of all driver's score data
		public static Dictionary<string,scoreCalcs.driverScoreData> scores;
		// Returned list of team scores
		public static List<scoreCalcs.teamData> teamScores;
		// Returned list of all class data, sorted by sort order given in file
		public static SortedDictionary<int,scoreCalcs.paxInfo> sortedClassList;
		// Returned statistics
		public static scoreCalcs.statsDataClass stats;
		// The arguments to tell us what to show
		public static scoreArgs args;
		
		public static CSVData configData = new CSVData();
		public static bool showLastName = false;
		
		private const string separator = "\r\n--------------------------------------------------------------------------------\r\n";
		public static string textScore(scoreArgs myArgs)
		{
			args = myArgs;
			string configFolder = args.configFolder;
			string configFilename;
			if (configFolder == "")
			{
				configFolder = Process.GetCurrentProcess().MainModule.FileName;
				configFolder = Path.GetDirectoryName(configFolder);
				configFolder =  configFolder + "\\..\\config";
			}
			configFilename =  configFolder + "\\configData.csv";
			string err = configData.LoadData(configFilename,',',"Parameter");
			if (err != "")
			{
				return "Unable to load config file: " + err;
			}
			if (configData.GetField("ShowLastName", "Value").Contains("Y"))
			{
				showLastName = true;
			}
			// Do some scoring calcs
			err = scoreCalcs.doScore(args, out scores, out teamScores, out stats, out sortedClassList);
			
			if (string.IsNullOrEmpty(err) == false)
			{
				return err;
			}
			string results = "";
			
			if (args.showRunTimes == true)
			{
				results += separator + runTimes(args);
			}
			if (args.showRawTimes == true)
			{
				results += separator + rawTimes(args);
			}
			if (args.showPaxTimes == true)
			{
				results += separator + paxTimes(args);
			}
			if (args.showClassTimes == true)
			{
				results += separator + classTimes(args);
			}
			if (args.showTeams == true)
			{
				results += separator + Teams(args);
			}
			if (args.showConeCounts == true)
			{
				results += separator;
				results += coneCounts(args);
			}
			results += separator;
			if (stats.day1.set1TimeOfFirstRun != "")
			{
				results += String.Format("Set1: First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                         stats.day1.set1TimeOfFirstRun, stats.day1.set1TimeOfLastRun, stats.day1.set1NumberOfRuns, stats.day1.set1TotalTime / stats.day1.set1NumberOfRuns);
			}
			if (stats.day1.set2TimeOfFirstRun != "")
			{
				results += String.Format("Set2: First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                         stats.day1.set2TimeOfFirstRun, stats.day1.set2TimeOfLastRun, stats.day1.set2NumberOfRuns, stats.day1.set2TotalTime / stats.day1.set2NumberOfRuns);
			}
			if (stats.day1.set3TimeOfFirstRun != "")
			{
				results += String.Format("Fun:  First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                         stats.day1.set3TimeOfFirstRun, stats.day1.set3TimeOfLastRun, stats.day1.set3NumberOfRuns, stats.day1.set3TotalTime / stats.day1.set3NumberOfRuns);
			}
			// Day 2 if we have one
			if (stats.day2.set1TimeOfFirstRun != "")
			{
				results += String.Format("Set1: First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                         stats.day2.set1TimeOfFirstRun, stats.day2.set1TimeOfLastRun, stats.day2.set1NumberOfRuns, stats.day2.set1TotalTime / stats.day2.set1NumberOfRuns);
			}
			if (stats.day2.set2TimeOfFirstRun != "")
			{
				results += String.Format("Set2: First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                         stats.day2.set2TimeOfFirstRun, stats.day2.set2TimeOfLastRun, stats.day2.set2NumberOfRuns, stats.day2.set2TotalTime / stats.day2.set2NumberOfRuns);
			}
			if (stats.day2.set3TimeOfFirstRun != "")
			{
				results += String.Format("Fun:  First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                         stats.day2.set3TimeOfFirstRun, stats.day2.set3TimeOfLastRun, stats.day2.set3NumberOfRuns, stats.day2.set3TotalTime / stats.day2.set3NumberOfRuns);
			}
			return results;
		}
		// ---------------------------------------------------------------------------
		public static string textScoreSplit(scoreArgs myArgs,
		                                    out string runtimes,
		                                    out string rawtimes,
		                                    out string paxtimes,
		                                    out string classtimes,
		                                    out string teamtimes,
		                                    out string cones,
		                                    out string statistics
		                                   )
		{
			args = myArgs;
			
			runtimes = "";
			rawtimes = "";
			paxtimes = "";
			classtimes = "";
			teamtimes = "";
			cones = "";
			statistics = "";
			
			string configFolder = args.configFolder;
			string configFilename;
			if (configFolder == "")
			{
				configFolder = Process.GetCurrentProcess().MainModule.FileName;
				configFolder = Path.GetDirectoryName(configFolder);
				configFolder =  configFolder + "\\..\\config";
			}
			configFilename =  configFolder + "\\configData.csv";
			string err = configData.LoadData(configFilename,',',"Parameter");
			if (err != "")
			{
				return "Unable to load config file: " + err;
			}
			if (configData.GetField("ShowLastName", "Value").Contains("Y"))
			{
				showLastName = true;
			}
			
			// Do some scoring calcs
			err = scoreCalcs.doScore(args, out scores, out teamScores, out stats, out sortedClassList);
			
			if (string.IsNullOrEmpty(err) == false)
			{
				return err;
			}
			
			if (args.showRunTimes == true)
			{
				runtimes = runTimes(args);
			}
			if (args.showRawTimes == true)
			{
				rawtimes = rawTimes(args);
			}
			if (args.showPaxTimes == true)
			{
				paxtimes = paxTimes(args);
			}
			if (args.showClassTimes == true)
			{
				classtimes = classTimes(args);
			}
			if (args.showTeams == true)
			{
				teamtimes = Teams(args);
			}
			if (args.showConeCounts == true)
			{
				cones = coneCounts(args);
			}
			
			if (stats.day1.set1TimeOfFirstRun != "")
			{
				statistics += String.Format("Set1: First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                            stats.day1.set1TimeOfFirstRun, stats.day1.set1TimeOfLastRun, stats.day1.set1NumberOfRuns, stats.day1.set1TotalTime / stats.day1.set1NumberOfRuns);
			}
			if (stats.day1.set2TimeOfFirstRun != "")
			{
				statistics += String.Format("Set2: First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                            stats.day1.set2TimeOfFirstRun, stats.day1.set2TimeOfLastRun, stats.day1.set2NumberOfRuns, stats.day1.set2TotalTime / stats.day1.set2NumberOfRuns);
			}
			if (stats.day1.set3TimeOfFirstRun != "")
			{
				statistics += String.Format("Fun:  First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                            stats.day1.set3TimeOfFirstRun, stats.day1.set3TimeOfLastRun, stats.day1.set3NumberOfRuns, stats.day1.set3TotalTime / stats.day1.set3NumberOfRuns);
			}
			// Day 2 if we have one
			if (stats.day2.set1TimeOfFirstRun != "")
			{
				statistics += String.Format("Set1: First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                            stats.day2.set1TimeOfFirstRun, stats.day2.set1TimeOfLastRun, stats.day2.set1NumberOfRuns, stats.day2.set1TotalTime / stats.day2.set1NumberOfRuns);
			}
			if (stats.day2.set2TimeOfFirstRun != "")
			{
				statistics += String.Format("Set2: First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                            stats.day2.set2TimeOfFirstRun, stats.day2.set2TimeOfLastRun, stats.day2.set2NumberOfRuns, stats.day2.set2TotalTime / stats.day2.set2NumberOfRuns);
			}
			if (stats.day2.set3TimeOfFirstRun != "")
			{
				statistics += String.Format("Fun:  First run: {0} Last run: {1} Number of runs: {2} Avg run time: {3,5:#0.00}\r\n",
				                            stats.day2.set3TimeOfFirstRun, stats.day2.set3TimeOfLastRun, stats.day2.set3NumberOfRuns, stats.day2.set3TotalTime / stats.day2.set3NumberOfRuns);
			}
			return "";
		}
		// ---------------------------------------------------------------------------
		// Times > DNFvalue denote a DNS
		public static string timeOrDNS(double time)
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
		public static string rawTimes(scoreArgs args)
		{
			string results = "";
			string rookieResults = separator;
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			// Sort by raw time
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestRAW.CompareTo(nextPair.Value.scoreData.bestRAW);
			            }
			           );
			
			
			rookieResults += String.Format("Rookie ranking by raw time:\r\n");
			results += String.Format("Overall ranking by raw time:\r\n");
			string hdr = String.Format("{0,4} {1,8} {2,11} {3,-16} {4,-22} {5,8} {6,7}\r\n",
			                           "Rank","Car#","Class","Driver","Car","Raw Time", "Score");
			results += hdr;
			rookieResults += hdr;
			
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
					if (driverName.Length > 16)
					{
						driverName = driverName.Substring(0,16);
					}
				}
				string line = String.Format("{0,4} {1,8} {2,11} {3,-16} {4,-22} {5,8:#.000} {6,7:#0.000}\r\n",
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
					line = String.Format("{0,4} {1,8} {2,11} {3,-16} {4,-22} {5,8:#.000} {6,7:#0.000}\r\n",
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
		public static string paxTimes(scoreArgs args)
		{
			string results = "";
			string rookieResults = separator;
			
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			// Sort by pax time
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestPAX.CompareTo(nextPair.Value.scoreData.bestPAX);
			            }
			           );
			
			results += String.Format("Overall ranking by PAX:\r\n");
			rookieResults += String.Format("Rookie ranking by PAX:\r\n");
            string hdr = String.Format("{0,4} {1,8} {2,11} {3,-16} {4,-22} {5,8}  {6,5} {7,9} {8,7} {9,8} {10,8}\r\n",
                                       "Rank", "Car#", "Class", "Driver", "Car", "Raw Time", "PAX #", "PAX Time", "Score", "RAWtoNext", "RAWtoFirst");

            results += hdr;
			rookieResults += hdr;

            foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
			{
				string driverName = driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1);
				if (showLastName)
				{
					driverName = driver.Value.firstName + " " + driver.Value.lastName;
					if (driverName.Length > 16)
					{
						driverName = driverName.Substring(0,16);
					}
				}
                results += String.Format("{0,4} {1,8} {2,11} {3,-16} {4,-22} {5,8:#.000}  {6,5:#0.000} {7,9:#.000} {8,7:#0.000} {9,8:#.000} {10,11:#.000}\r\n",
				                         driver.Value.scoreData.PAXrank,
				                         driver.Value.number,
				                         driver.Value.carClass,
				                         driverName,
				                         driver.Value.carDescription,
				                         driver.Value.scoreData.bestRAW < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestRAW.ToString("#.000") : "DNS",
				                         driver.Value.pax,
				                         driver.Value.scoreData.bestPAX < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestPAX.ToString("#.000") : "DNS",
				                         driver.Value.scoreData.PAXscore,
										 driver.Value.scoreData.RAWtoNext,
										 driver.Value.scoreData.RAWtoFirst
										);
				if (driver.Value.rookie)
				{
					rookieResults += String.Format("{0,4} {1,8} {2,11} {3,-16} {4,-22} {5,8:#.000}  {6,5:#0.000} {7,9:#.000} {8,7:#0.000} {9,8:#.000} {10,11:#.000}\r\n",
					                               driver.Value.scoreData.PAXRookieRank,
					                               driver.Value.number,
					                               driver.Value.carClass,
					                               driverName,
					                               driver.Value.carDescription,
					                               driver.Value.scoreData.bestRAW < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestRAW.ToString("#.000") : "DNS",
					                               driver.Value.pax,
					                               driver.Value.scoreData.bestPAX < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestPAX.ToString("#.000") : "DNS",
					                               driver.Value.scoreData.PAXrookieScore,
												   driver.Value.scoreData.RookieRAWtoNext,
												   driver.Value.scoreData.RookieRAWtoFirst
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
		// print out times, groups by Group, ordered within group by PAX time
		public static string classTimes(scoreArgs args)
		{
			string results = "";
			string rookieResults = separator;
			
			// Sort drivers by PAX time
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	return firstPair.Value.scoreData.bestPAX.CompareTo(nextPair.Value.scoreData.bestPAX);
			            }
			           );
			rookieResults += String.Format("Rookie ranking by Group:\r\n");
			results += String.Format("Overall ranking by Group:\r\n");
			
			foreach (KeyValuePair<int, scoreCalcs.paxInfo> classInfo in sortedClassList)
			{
				var curClass = classInfo.Value;
				string curClassGroup = curClass.group;
				
				double bestTime = 0.0;
				
				foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
				{
					bool driverIsInXgroup = false;
					foreach(scoreCalcs.groupscore grp in driver.Value.scoreData.groupScores)
					{
						if (grp.groupName == curClass.carClass)
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

					line += String.Format("\r\nGroup: " + curClass.carClass + " (" + curClass.description + ")\r\n");

					line += String.Format("{0,4} {1,8} {2,11} {3,-16} {4,-22} {5,8}  {6,5} {7,9} {8,6}",
					                      "Rank","Car#","Class","Driver","Car","Raw Time","PAX #", "PAX Time", "Score");
					
					line += String.Format("\r\n");

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
					if (driver.Value.carDescription.Length > 20)
					{
						driver.Value.carDescription = driver.Value.carDescription.Substring(0,20);
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
						if (driverName.Length > 16)
						{
							driverName = driverName.Substring(0,16);
						}
					}
					results += String.Format("{0,-1}{1,3} {2,8} {3,11} {4,-16} {5,-22} {6,8:#.000} {7,6:#.000} {8,9:#0.000} {9,8:#0.000} ",
					                         trophyIndicator,
					                         grpPtr.groupRank,
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
						rookieResults += String.Format("{0,-1}{1,3} {2,8} {3,11} {4,-16} {5,-22} {6,8:#.000} {7,6:#.000} {8,9:#0.000} {9,8:#0.000} ",
						                               rookieTrophyIndicator,
						                               grpPtr.groupRookieRank,
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
		// Team scores
		// We group rAW and PAX if given in the driver's group filed
		// If the group is something else, then we ignore the group and show the team for both raw and pax
		public static string Teams(scoreArgs args)
		{
			string results = "";
			results += "RAW Team scores\r\n";
			
			// We sort our teams by raw time
			teamScores.Sort(delegate(scoreCalcs.teamData first, scoreCalcs.teamData next)
			                {
			                	// disable ConvertToLambdaExpression
			                	return first.rawTotal.CompareTo(next.rawTotal);
			                }
			               );
			// now go through the sorted list and print out raw team results
			results = "RAW Team scores\r\n\r\n";
			results += string.Format("{0,4}", "Rank");
			int rank = 1;
			foreach (scoreCalcs.teamData tm in teamScores)
			{
				if (tm.teamType.StartsWith("PAX") == true)
				{
					continue;
				}
				
				results += string.Format("\r\n{0,4} Team:{1,6} Total: RAW:{2,8:#.000} PAX:{3,8:#.000} Cones: {4}\r\n",
				                         rank.ToString(),
				                         tm.team,
				                         timeOrDNS(tm.rawTotal),
				                         timeOrDNS(tm.paxTotal),
				                         tm.coneTotal
				                        );
				rank += 1;
				foreach (scoreCalcs.driverScoreData driver in tm.teamDrivers)
				{
					string driverName = driver.firstName + " " + driver.lastName.Substring(0,1);
					if (showLastName)
					{
						driverName = driver.firstName + " " + driver.lastName;
						if (driverName.Length > 16)
						{
							driverName = driverName.Substring(0,16);
						}
					}
					results += string.Format("\tDriver {0,3}: {1,-16}  RAW:{2, 8:#.000}  PAX:{3, 8:#.000}\r\n",
					                         driver.number,
					                         driverName,
					                         timeOrDNS(driver.scoreData.bestRAW),
					                         timeOrDNS(driver.scoreData.bestPAX)
					                        );
				}
			}
			
			// Do the same thing for PAX teams
			// Now we sort our teams by PAX time
			teamScores.Sort(delegate(scoreCalcs.teamData first, scoreCalcs.teamData next)
			                {
			                	return first.paxTotal.CompareTo(next.paxTotal);
			                }
			               );
			results += separator + "PAX Team scores\r\n\r\n";
			results += string.Format("{0,4}", "Rank");
			rank = 1;
			foreach (scoreCalcs.teamData tm in teamScores)
			{
				if (tm.teamType.StartsWith("RAW") == true)
				{
					continue;
				}
				
				results += string.Format("\r\n{0,4} Team:{1,6} Total:  RAW:{2, 8:#.000} PAX:{3, 8:#.000} Cones: {4}\r\n",
				                         rank.ToString(),
				                         tm.team,
				                         timeOrDNS(tm.rawTotal),
				                         timeOrDNS(tm.paxTotal),
				                         tm.coneTotal
				                        );
				rank += 1;
				foreach (scoreCalcs.driverScoreData driver in tm.teamDrivers)
				{
					string driverName = driver.firstName + " " + driver.lastName.Substring(0,1);
					if (showLastName)
					{
						driverName = driver.firstName + " " + driver.lastName;
						if (driverName.Length > 16)
						{
							driverName = driverName.Substring(0,16);
						}
					}
					results += string.Format("\tDriver {0,3}: {1,-16}  RAW:{2, 8:#.000}  PAX:{3, 8:#.000}\r\n",
					                         driver.number,
					                         driverName,
					                         timeOrDNS(driver.scoreData.bestRAW),
					                         timeOrDNS(driver.scoreData.bestPAX)
					                        );
				}
			}
			return results;
		}
		// ---------------------------------------------------------------------------
		// Return a printable text string for cone counts
		public static string coneCounts(scoreArgs args)
		{
			string results = "";
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			// Sort by cone counts
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	return nextPair.Value.coneCount.CompareTo(firstPair.Value.coneCount);
			            }
			           );
			
			results += String.Format("Cone counts:\r\n");
			
			results += String.Format("{0,4} {1,8} {2,-16} {3,4}\r\n",
			                         "Rank","Car#","Driver","Cones");
			
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
					if (driverName.Length > 16)
					{
						driverName = driverName.Substring(0,16);
					}
				}
				results += String.Format("{0,4} {1,8} {2,-16} {3,4}\r\n",
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
		public static string printSet(scoreCalcs.singleSetData setData, int dayNumber, int setNumber)
		{
			string results = String.Format("{0,19}Day {1,1} Set {2,1}"," ", dayNumber,setNumber);
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
					results += "Fun-->";
				}
				if (run.penalty == "DNF")
				{
					if (run.time >= scoreCalcs.DNFvalue)
					{
						// didn't even run
						results += String.Format("{0,8:#.000}{1,-4}","DNS   ","    ");
					}
					else
					{
						results += String.Format("{0,8:#.000}{1,-4}",run.time,"+DNF");
					}
				}
				else if (run.penalty == "")
				{
					if (run == setData.bestRun)
					{
						results += String.Format("{0,8: (#.000}{1,-4}", run.time,")  ");
					}
					else
					{
						results += String.Format("{0,8:#.000}{1,-4}", run.time,"    ");
					}
				}
				else
				{
					if (run == setData.bestRun)
					{
						results += String.Format("{0,8: (#.000}{1,-4}", run.time, "+" + run.penalty + ")");
					}
					else
					{
						results += String.Format("{0,8:#.000}{1,-4}", run.time, "+" + run.penalty);
					}
				}
			}
			if (hasRuns == false)
			{
				results += String.Format("{0,8:#.000}{1,-4}","DNS   ","    ");
			}
			results += String.Format("\r\n");
			return results;
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
		// print out run data
		public static string runTimes(scoreArgs args)
		{
			string results = "";
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			// Sort by car number
			// TODO sort properly, even if number has trailing letters (delegate in mjcommon?)
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
			                     KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
			            {
			            	int d1Number,d2Number;
			            	d1Number = GetLeadingInt(firstPair.Value.number);
			            	d2Number = GetLeadingInt(nextPair.Value.number);
			            	return d1Number.CompareTo(d2Number);
			            }
			           );
			
			results += "Run data (ordered by car number):\r\n";
			// see http://blog.stevex.net/string-formatting-in-csharp/ for a C# formatting info
			results += String.Format("{0,8} {1,3} {2,3} {3,15} {4,-16} {5,-22} {6,-22}\r\n",
			                         "Car#","Mbr","Rky","Class","Driver","Car","Sponsor");
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
					if (driverName.Length > 16)
					{
						driverName = driverName.Substring(0,16);
					}
				}
				string line = String.Format("{0,8} {1,3} {2,3} {3,15} {4,-16} {5,-22} {6,-22}\r\n",
				                            driver.Value.number,
				                            driver.Value.member,
				                            driver.Value.rookie?"Yes":"No",
				                            driver.Value.carClass,
				                            driverName,
				                            driver.Value.carDescription,
				                            driver.Value.sponsor
				                           );
				results += String.Format(line);
				if (day1NumSets >= 1)
				{
					results += printSet(driver.Value.Day1Set1,1,1);
				}
				if (day1NumSets >= 2)
				{
					results += printSet(driver.Value.Day1Set2,1,2);
				}
				if (day1NumSets >= 3)
				{
					results += printSet(driver.Value.Day1Set3,1,3);
				}
				if (day2NumSets >= 1)
				{
					results += printSet(driver.Value.Day2Set1,2,1);
				}
				if (day2NumSets >= 2)
				{
					results += printSet(driver.Value.Day2Set2,2,2);
				}
				if (day2NumSets >= 3)
				{
					results += printSet(driver.Value.Day2Set3,2,3);
				}
				
				if (args.set1Only == true)
				{
					results += String.Format("{0,35}","Total (set1 only):");
				}
				else if (args.set2Only == true)
				{
					results += String.Format("{0,35}","Total (set2 only):");
				}
				else if (args.bestSingleRun == true)
				{
					results += String.Format("{0,35}","Total (single best run):");
				}
				else
				{
					results += String.Format("{0,35}","Total (both sets):");
				}
				
				
				if (driver.Value.scoreData.bestRAW >= scoreCalcs.DNFvalue)
				{
					results += String.Format("{0,8}\r\n","DNS");
				}
				else
				{
					results += String.Format("{0,8:#.000}\r\n",driver.Value.scoreData.bestRAW);
				}
			}
			return results;
		}
		// ---------------------------------------------------------------------------
	} // class
} // namespace