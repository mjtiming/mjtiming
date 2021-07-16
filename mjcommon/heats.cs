using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace RaceBeam
{
	/// <summary>
	/// Description of heats.
	/// </summary>
	public static class Heats
	{
		private class classCount
		{
			public string name;
			public int count;
			public List<string> drivers = new List<string>();
		}
		// --------------------------------------------------------------------
		public static string doHeatCalcs()
		{
			string heatInfo = "";
			var driverData = new CSVData();
			var classData = new CSVData();
			var configData = new CSVData();
			string configFolder = Process.GetCurrentProcess().MainModule.FileName;
			configFolder = Path.GetDirectoryName(configFolder);
			string configFilename =  configFolder + "\\..\\config\\configData.csv";
			string err = configData.LoadData(configFilename,',', "Parameter");
			if (err != "")
			{
				return("Unable to load config file: " + err);
			}
			
			string driverDataFile = configData.GetField("driverDataFile", "Value");
			if (driverDataFile == "")
			{
				return("Driver data file not defined in config file");
			}
			
			string classDataFile = configData.GetField("classDataFile", "Value");
			if (classDataFile == "")
			{
				return("Class data file not defined in config file");
			}
			// Read in driverData file for this event
			string day1Name = DateTime.Now.ToString("yyyy_MM_dd");
			err = driverData.LoadData(driverDataFile, ',', "Number");
			if (err != "")
			{
				return(driverDataFile + ": " + err);
			}
			
			// Read in class data file
			err = classData.LoadData(classDataFile,',',"Class");
			if (err != "")
			{
				return(err);
			}
			List<string> driverList = driverData.getKeys();
			
			var groupCounts = new Dictionary<string, classCount>();
			int registeredDrivers = 0;
			foreach (string driver in driverList)
			{
				// We need to order by the driver class
				string className = driverData.GetField(driver, "Class");
				if (className == "")
				{
					className = "AM";
				}
				string registered = driverData.GetField(driver, "Registered");
				string driverGroupName = driverData.GetField(driver, "Group");
				registered = registered.ToUpperInvariant();
				if (registered.Contains("Y") == false)
				{
					continue;
				}
				registeredDrivers += 1;

				// A driver group can be one member of a larger class grouping
				// e.g. BS belongs to "Stock"
				//string classGroupName = classData.getField(className, "Group");
				
				// We either group by class or group, depending on config
				string runheats = configData.GetField("runheats", "Value");
				runheats = runheats.ToLowerInvariant();
				bool heatByGroup = true;	// default to group
				if (runheats.Contains("class"))
				{
					heatByGroup = false;
				}
				
				string heatName = driverGroupName;
				if (heatByGroup == false)
				{
					heatName = className;
				}
				if (heatName == "")
				{
					// Fall back to class
					heatName = className;
				}

				if (groupCounts.ContainsKey(heatName))
				{
					groupCounts[heatName].count += 1;
					groupCounts[heatName].drivers.Add(driver);
				}
				else
				{
					var c = new classCount();
					c.name = heatName;
					c.count = 1;
					c.drivers.Add(driver);
					groupCounts.Add(c.name, c);
				}
			}
			
			// sort group list in inverse order of size
			var groupList = new List<classCount>(groupCounts.Values);
			groupList.Sort(delegate(classCount first, classCount next)
			               {
			               	return next.count.CompareTo(first.count);
			               }
			              );
			heatInfo += "Registered drivers: " + registeredDrivers + "\n";
			var heat1Of2 = new List<string>();
			int heat1Of2Count = 0;
			var heat2Of2 = new List<string>();
			int heat2Of2Count = 0;
			var heat1Of3 = new List<string>();
			int heat1Of3Count = 0;
			var heat2Of3 = new List<string>();
			int heat2Of3Count = 0;
			var heat3Of3 = new List<string>();
			int heat3Of3Count = 0;
			
			
			
			foreach (classCount classGroup in groupList)
			{
				heatInfo += classGroup.count.ToString("#00") + " : " + classGroup.name +"\n";
				// Allocate assignments for two heats
				if (heat1Of2Count > heat2Of2Count)
				{
					heat2Of2.Add(classGroup.name);
					heat2Of2Count += classGroup.count;
				}
				else
				{
					heat1Of2.Add(classGroup.name);
					heat1Of2Count += classGroup.count;
				}

				// Allocate assignments for three heats
				if ((heat1Of3Count <= heat2Of3Count) && (heat1Of3Count <= heat3Of3Count))
				{
					// heat 1 is smallest
					heat1Of3.Add(classGroup.name);
					heat1Of3Count += classGroup.count;
				}
				else if ((heat2Of3Count <= heat1Of3Count) && (heat2Of3Count <= heat3Of3Count))
				{
					// heat 2 is smallest
					heat2Of3.Add(classGroup.name);
					heat2Of3Count += classGroup.count;
				}
				else
				{
					// heat 3 is smallest
					heat3Of3.Add(classGroup.name);
					heat3Of3Count += classGroup.count;
				}
			}
			
			heatInfo += ("\nHeat 1 of 2: (" + heat1Of2Count.ToString("#00") + "):");
			foreach (string name in heat1Of2)
			{
				heatInfo += (name + ", ");
			}
			heatInfo += ("\nHeat 2 of 2: (" + heat2Of2Count.ToString("#00") + "):");
			foreach (string name in heat2Of2)
			{
				heatInfo += (name + ", ");
			}
			
			heatInfo += ("\n\nHeat 1 of 3: (" + heat1Of3Count.ToString("#00") + "):");
			foreach (string name in heat1Of3)
			{
				heatInfo += (name + ", ");
			}
			heatInfo += ("\nHeat 2 of 3: (" + heat2Of3Count.ToString("#00") + "):");
			foreach (string name in heat2Of3)
			{
				heatInfo += (name + ", ");
			}
			heatInfo += ("\nHeat 3 of 3: (" + heat3Of3Count.ToString("#00") + "):");
			foreach (string name in heat3Of3)
			{
				heatInfo += (name + ", ");
			}
			// Randomly generate heat orders
			var runOrder = new List<KeyValuePair<int,string>>();
			var rnd = new Random();
			runOrder.Add(new KeyValuePair<int, string>(rnd.Next(1, 100), "1"));
			runOrder.Add(new KeyValuePair<int, string>(rnd.Next(1, 100), "2"));
			runOrder.Sort(delegate(KeyValuePair<int, string> firstPair, KeyValuePair<int, string> nextPair)
			              {
			              	return (firstPair.Key.CompareTo(nextPair.Key));
			              }
			             );
			heatInfo += "\n\nRandom run order for 2 Heats: ";
			foreach (var ht in runOrder)
			{
				heatInfo += ht.Value.ToString() + ", ";
			}
			runOrder.Clear();
			runOrder.Add(new KeyValuePair<int, string>(rnd.Next(1, 100), "1"));
			runOrder.Add(new KeyValuePair<int, string>(rnd.Next(1, 100), "2"));
			runOrder.Add(new KeyValuePair<int, string>(rnd.Next(1, 100), "3"));
			runOrder.Sort(delegate(KeyValuePair<int, string> firstPair, KeyValuePair<int, string> nextPair)
			              {
			              	return (firstPair.Key.CompareTo(nextPair.Key));
			              }
			             );
			heatInfo += "\nRandom run order for 3 Heats: ";
			foreach (var ht in runOrder)
			{
				heatInfo += ht.Value.ToString() + ", ";
			}
			heatInfo += "\n";
			
			
			// Display the team assignments
			var teamCounts = new Dictionary<string, classCount>();
			foreach (string driver in driverList)
			{
				// We need to order by the driver class
				string teamName = driverData.GetField(driver, "Team");
				string registered = driverData.GetField(driver, "Registered");
				registered = registered.ToUpperInvariant();
				if (registered.Contains("Y") == false)
					continue;
				
				if (teamCounts.ContainsKey(teamName))
				{
					teamCounts[teamName].count += 1;
				}
				else
				{
					var c = new classCount();
					c.name = teamName;
					c.count = 1;
					teamCounts.Add(c.name, c);
				}
			}
			// Sort teams by name
			var teamList = new List<classCount>(teamCounts.Values);
			teamList.Sort(delegate(classCount first, classCount next)
			              {
			              	return first.name.CompareTo(next.name);
			              }
			             );
			heatInfo += "\n-------------------------------------------------------\n";
			heatInfo += "Teams:\n";
			
			foreach (var team in teamList)
			{
				if (string.IsNullOrEmpty(team.name))
				{
					team.name = "Not named";
				}
				heatInfo += String.Format("      {0,-25}{1,3:#0}\r\n", team.name, team.count);
			}
			
			// Listing of each driver
			heatInfo += "-------------------------------------------------------\n";
			heatInfo += "Listing of drivers for two heats\n";
			heatInfo += listheat("Heat 1 of 2", heat1Of2, groupCounts, driverData);
			heatInfo += listheat("Heat 2 of 2", heat2Of2, groupCounts, driverData);
			heatInfo += "\n-------------------------------------------------------\n";
			heatInfo += "Listing of drivers for three heats\n";
			heatInfo += listheat("Heat 1 of 3", heat1Of3, groupCounts, driverData);
			heatInfo += listheat("Heat 2 of 3", heat2Of3, groupCounts, driverData);
			heatInfo += listheat("Heat 3 of 3", heat3Of3, groupCounts, driverData);
			return heatInfo;
		}
		// --------------------------------------------------------------------
		private static string listheat(string title, List<string> heat, Dictionary<string, classCount> groupCounts, CSVData driverData)
		{
			string output = "";
			
			output += String.Format("\n\n{0}:", title);
			foreach (string name in heat)
			{
				groupCounts[name].drivers.Sort(delegate(string first, string next)
				                               {
				                               	int d1Number,d2Number;
				                               	if (int.TryParse(first, out d1Number) == false)
				                               	{
				                               		return first.CompareTo(next);
				                               	}
				                               	if (int.TryParse(next, out d2Number) == false)
				                               	{
				                               		return first.CompareTo(next);
				                               	}
				                               	return d1Number.CompareTo(d2Number);
				                               });
				output += String.Format("\n\t{0,-25}\n", name);
				foreach (string carnum in groupCounts[name].drivers)
				{
					string firstName = driverData.GetField(carnum, "First Name");
					string lastName = driverData.GetField(carnum, "Last Name");
					string group = driverData.GetField(carnum, "Group");
					string carclass = driverData.GetField(carnum, "Class");
					
					output += String.Format("{0,30} {1,-10} {2,-4} {3,-10} {4,-15}\n", carnum, group, carclass, firstName, lastName);
				}
			}
			return output;
		}
	}
}
