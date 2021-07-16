using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

/*
 * - Total participation days (1132 in 2017 and 885 in 2016)
- The total amount of drivers to participate in our events (last year was 332)
- How many cones we hit total (last year was 2470)
- How many runs we did total (11,863 last year)
*/

namespace RaceBeam
{
	class Statistics
	{
		public static int[] buckets = new int[] {0, 10, 15, 20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39, 40, 45, 50, 55, 60,120,180,240,600,900,1200,18000};
		
		public static Dictionary<string, int> driverNums = new Dictionary<string, int>();
		public static void Usage()
		{
			Console.WriteLine("Usage: stats <event folder>");
			Environment.Exit(0);
		}
		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Usage();
			}
			string[] filePaths = Directory.GetFiles(args[0], "*_timingData.csv");
			int totalEvents = 0;
			int totalRuns = 0;
			int totalReruns = 0;
			int totalDNFs = 0;
			int totalCones = 0;
			int totalDrivers = 0;
			foreach (string timingFileName in filePaths)
			{
				var rdata = new CSVData();
				// Read in timing data
				string err = rdata.LoadData(timingFileName,',',"index");
				if (err != "")
				{
					Console.WriteLine(timingFileName + ": " + err);
					Usage();
				}
				totalEvents += 1;
				driverNums = new Dictionary<string, int>();
				
				for (int i = 1; i < rdata.Length(); i++)
				{
					totalRuns += 1;
					string iStart = i.ToString();
					string iLastStart = (i-1).ToString();
					string carNum = rdata.GetField(iStart, "car_number");
					if (driverNums.ContainsKey(carNum) == false)
					{
						driverNums.Add(carNum, 1);
					}
						
					string penalty = rdata.GetField(iLastStart, "penalty");
					if (penalty == "RRN")
					{
						totalReruns += 1;
					}
					if (penalty == "DNF")
					{
						totalDNFs += 1;
					}
                    if (int.TryParse(penalty, out int cones) == true)
                    {
                        totalCones += cones;
                    }
                }
				totalDrivers += driverNums.Count;
			}
			Console.WriteLine("Events: {0} Runs: {1}  Reruns: {2} DNFs: {3} Cones: {4} Drivers: {5}",
			                  totalEvents, totalRuns, totalReruns, totalDNFs, totalCones, totalDrivers);

			return;
		}
	}
}