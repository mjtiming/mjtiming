using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace RaceBeam
{
	/// <summary>
	/// Class to hold data loaded from csv file
	/// Requires that the csv file have a header line
	/// Read-only
	/// </summary>
	public class CSVData
	{
		// We use a dictionary containing other dictionaries
		// Key is first column in CSV file, it keys to dict of header names and values
		// Access via data["driver number"]["header name"]
		
		private readonly Dictionary<string, Dictionary<string,string>> _data = new Dictionary<string, Dictionary<string, string>>();
		private List<string> headers = new List<string>();
		
		// Return the array of headers
		public List<string> GetHeaders()
		{
			return headers;
		}
		// Return a list of keys
		public List<string> getKeys()
		{
			var list = new List<string>(_data.Keys);
			return list;
		}
		
		/// <summary>
		/// Gets the field value at the specified record index for the supplied field name
		/// </summary>
		/// <param name="key">Car number</param>
		/// <param name="fieldName">Field name</param>
		/// <returns>Value string or empty string</returns>
		public string GetField(string key, string fieldName)
		{
			if (key == null)
			{
				return "";
			}
			if ((_data.ContainsKey(key)) && (_data[key].ContainsKey(fieldName)))
			{
				return _data[key][fieldName];
			}
			else
			{
				return "";
			}
		}
		/// <summary>
		/// Returns number of keyed elements (lines in CSV file)
		/// </summary>
		/// 
		public int Length()
		{
			return _data.Count;
		}
		
		/// <param name="filePath"></param>
		/// <summary>
		/// Reads in the driver or timing data from the specified csv file
		/// Dictionary key is first column heading name
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <param name="separator">character separator</param>
		/// <param name="keyField">NAme of key field (must be unique)</param>
		public string LoadData(string filePath, char separator, string keyField)
		{
			_data.Clear();
			bool addedHeader = false;
			int numColumns;
			int myKey = 0;	// when we invent a key field
			try
			{
				var file = new System.IO.StreamReader(
					File.Open(filePath,FileMode.Open,
						FileAccess.Read,
						FileShare.ReadWrite));
				using (TextFieldParser parser = new TextFieldParser(file))
				{
					if (separator == ',')
					{
						parser.SetDelimiters(",");
					}
					else
					{
						parser.SetDelimiters("\t");
					}
					parser.TextFieldType = FieldType.Delimited;
					
					while (!parser.EndOfData)
					{
						string[] columns;
						try
						{
							columns = parser.ReadFields();
						}
						catch
						{
							// not a valid delimited line - log, terminate, or ignore
							continue;
						}
						
						if ((columns.Length < 3) || ((columns.Length == 3) && (string.IsNullOrEmpty(columns[2]) == true)))
						{
							// didn't work -- skip this line
							continue;
						}
						for (int i = 0; i < columns.Length; i++)
						{
							columns[i] = columns[i].Replace(",",";");
							columns[i] = columns[i].Replace("\t"," ");
						}
						if (!addedHeader)
						{
							headers = new List<string>();
							numColumns = columns.Length;
							for (int i = 0; i < columns.Length; i++)
							{
								if (string.IsNullOrEmpty(columns[i]) == true)
								{
									columns[i] = "col" + i.ToString();
								}
								headers.Add(columns[i]);
								addedHeader = true;
							}
							if (keyField == "")
							{
								headers.Add("myKey");
							}
							continue;
						}
					
						var record = new Dictionary<string, string>();
					
						for (int i = 0; i < columns.Length; i++)
						{
							if (i >= headers.Count)
							{
								continue;
							}
							if (string.IsNullOrEmpty(columns[i]) == true)
							{
								//columns[i] = "col" + i.ToString();
							}
							if (record.ContainsKey(headers[i]))
							{
								continue;
							}
							record.Add(headers[i], columns[i]);
						}
						if (keyField == "")
						{
							string nextKey = myKey.ToString();
							myKey += 1;
							if (record.ContainsKey(nextKey))
							{
								continue;
							}
							record.Add("myKey", nextKey);
						}
						string key = keyField;
						if (keyField == "")
						{
							key = "myKey";
						}
						if (record.ContainsKey(key))
						{
							if (_data.ContainsKey(record[key]))
							{
								continue;
							}
							_data.Add(record[key], record);
						}
					}
				}
				file.Close();
				file = null;
			}
			catch (Exception e)
			{
				return(e.Message);
			}
			return "";
		}
	}
}
