// Compile command:
// C:\Windows\Microsoft.NET\Framework\v3.5\Csc.exe /reference:c:\mjtiming\bin\mjcommon.dll /platform:x86 sample.cs
// Note that the resulting sample.exe will not work unless it is copied into mjtiming\bin

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RaceBeam  // this is required to get easy reference to the datatypes
{
	class paxscore
	{
		// returned list of all driver's score data
		public static Dictionary<string,scoreCalcs.driverScoreData> scores;
		// Returned list of team scores
		public static List<scoreCalcs.teamData> teamScores;
		// Returned list of all class data, sorted by sort order given in file
		public static SortedDictionary<int,scoreCalcs.paxInfo> sortedClassList;
		// Returned statistics
		public static scoreCalcs.statsDataClass stats;
		
	
		// ---------------------------------------------------------------------------
		public static void usage()
		{
			Console.WriteLine("Usage: score <event date>\n");
			Environment.Exit(0);
		}
		// ---------------------------------------------------------------------------
		
		public static void Main(string[] args)
		{
			// default to 1 day scoring, today's date
			
			
			var argblock = new scoreArgs();		// parameters passed to the scoring module
			string day1Name = DateTime.Now.ToString("yyyy_MM_dd");
			argblock.day1 = day1Name;
			argblock.set1Only = true;
			// Where the config file resides
			argblock.configFolder = "C:\\mjtiming\\config";
			// this overrides the eventfolder specified in the config file
			argblock.eventFolder = "C:\\mjtiming\\eventdata";
			
			// Do the scoring calcs
			string err = scoreCalcs.doScore(argblock, out scores, out teamScores, out stats, out sortedClassList);
			
			if (string.IsNullOrEmpty(err) == false)
			{
				Console.WriteLine(err);
				return;
			}
			string results = paxTimes(argblock);
			Console.WriteLine(results);
		}
		
		// ---------------------------------------------------------------------------
		// Return a printable text string for PAX data
		public static string paxTimes(scoreArgs args)
		{
			string results = "";
			// List of all drivers and their score data
			var myList = new List<KeyValuePair<string, scoreCalcs.driverScoreData>>(scores);
			
			// Sort by pax time
			myList.Sort(delegate(KeyValuePair<string, scoreCalcs.driverScoreData> firstPair,
				KeyValuePair<string, scoreCalcs.driverScoreData> nextPair)
				{
					return firstPair.Value.scoreData.bestPAX.CompareTo(nextPair.Value.scoreData.bestPAX);
				}
			);
			
			results += String.Format("Overall ranking by PAX:\r\n");
			string hdr = String.Format("{0,4} {1,4} {2,3} {3,3} {4,5} {5,-16} {6,-22} {7,8}  {8,5} {9,9} {10,7}\r\n",
				"Rank","Car#","Mbr","Rky","Class","Driver","Car","Raw Time","PAX #", "PAX Time", "Score");
			results += hdr;
			
			foreach (KeyValuePair<string, scoreCalcs.driverScoreData> driver in myList)
			{
				
				results += String.Format("{0,4} {1,4} {2,3} {3,3} {4,5} {5,-16} {6,-22} {7,8:#.000}  {8,5:#0.000} {9,9:#.000} {10,7:#0.000}\r\n",
					driver.Value.scoreData.PAXrank,
					driver.Value.number,
					driver.Value.member,
					driver.Value.rookie?"Y":"N",
					driver.Value.carClass,
					driver.Value.firstName + " " + driver.Value.lastName.Substring(0,1),
					driver.Value.carDescription,
					driver.Value.scoreData.bestRAW < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestRAW.ToString("#.000") : "DNS",
					driver.Value.pax,
					driver.Value.scoreData.bestPAX < scoreCalcs.DNFvalue ? driver.Value.scoreData.bestPAX.ToString("#.000") : "DNS",
					driver.Value.scoreData.PAXscore
				);
			}
			return results;
		}
	}
}