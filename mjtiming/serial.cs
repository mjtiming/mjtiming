// Class for handling the serial ports
// Need to suppress the finalizer here, or it throws exceptions on exit if the port has gone away
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using RaceBeam;

/*
 On all items below,
	"R" is used to show 0x80 bytes
	"v" shows digits in reverse order
	"d" shows digits in normal order


JAC Chrono:
Start: Adddddd<13><10>
Reset start: Sdddddd<13><10>
Finish: Rvvvvvv<13><10>Bdddddd<13><10>
Reset finish: R000000<13><10>

R000000 == reset stop
R001702 == 207.100 (elapsed time with digits reversed)
S094937 == reset start (dddddd is start time stamp)
A000000 == start time
B000000 == stop time


Tag Heur:
<eye> hh:mm:ss.fff000<13>
T           01 00:03:08.313000
T           02 00:04:12.218000
      (Subtraction yields 01:04.-95) = 63.905 seconds
Reset (start or finish):
T-          <previous trigger time stamp (start or finish)>

JAC Normal:
Start: No start trigger
Finish: Rdddddd<13> Milliseconds in reverse order
Reset: R000000<13>

AC4 timer:
Start: not reported
Finish: 0x80[hex80]fffsss[cr][lf] or [hex80]fffssm[cr][lf]
We convert this to an R (reset) event and let the TimeEvent handler sort it out
 */
namespace RaceBeam
{
	
	public class PenaltyAndTime
	{
		public string time;
		public string penalty;
	}
	public class SerialIO
	{
		SerialPort TimerComPort = null;
		SerialPort DisplayComPort = null;
		SerialPort secondaryDisplayComPort = null;
		SerialPort barcodeComPort = null;
		string DisplayPortName = "";
		string secondaryDisplayPortName = "";
		string TimerPortName = "";
		string barcodePortName = "";
		bool reverseDisplayDigits = false;
		

		// Do we send a penalty to the display (MJTiming display only handles this)
		readonly bool sendPenalty = false;
        readonly CSVData configData = null;

        // We use this to display messages to the user
        readonly logMsg showMsg = null;
        readonly InvokeTimeEvent timingEvent = null;
		
		// -------------------------------------------------------------------
		// constructor
		public SerialIO(logMsg show_a_message, InvokeTimeEvent timerEventHandler, CSVData config)
		{
			// So we can display messages to the user
			showMsg = show_a_message;
			timingEvent = timerEventHandler;
			configData = config;

			string displayOption = configData.GetField("SendPenaltyToDisplay", "Value").ToUpperInvariant();
			if (displayOption.Contains("Y"))
			{
				sendPenalty = true;
			}
			StartBarcode();
		}
		// -------------------------------------------------------------------
		public bool StartTiming()
		{
			try
			{
				TimerPortName = configData.GetField("timerPort", "Value");
				DisplayPortName = configData.GetField("primaryDisplayPort", "Value");
				secondaryDisplayPortName = configData.GetField("secondaryDisplayPort", "Value");
				string reverseDigits = configData.GetField("reverseDisplayDigits", "Value");
				reverseDigits = reverseDigits.ToUpperInvariant();
				if ((string.IsNullOrEmpty(reverseDigits) == false) && (reverseDigits.StartsWith("Y") == true))
				{
					reverseDisplayDigits = true;
				}
				else
				{
					reverseDisplayDigits = false;
				}
				
				string timeoutString = configData.GetField("serialTimeoutMS", "Value");
                if (Int32.TryParse(timeoutString, out int readTimeout) == false)
                {
                    readTimeout = 100;
                }
                else
                {
                    if (readTimeout < 100)
                    {
                        readTimeout = 100;
                    }
                }

                TimerComPort = new SerialPort(TimerPortName, 9600, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.None,
                    DtrEnable = false,  // stop arduino reset
                    NewLine = "\r",
                    ReadTimeout = readTimeout   // Enough time for quite a few characters
                };
                TimerComPort.DataReceived += OnSerialDataReceived;
				TimerComPort.Open();
				//The workaround for unhandled finalizer expections
				GC.SuppressFinalize(TimerComPort.BaseStream);
				
				return true;
			}
			catch
			{
				return false;
			}
		}
		// -------------------------------------------------------------------
		public void StopTiming()
		{
			try
			{
				if ((TimerComPort != null) && (TimerComPort.IsOpen == true))
				{
					TimerComPort.Close();
					TimerComPort = null;
				}
				if ((DisplayComPort != null) && (DisplayComPort.IsOpen == true))
				{
					DisplayComPort.Close();
					DisplayComPort = null;
				}
			}
			catch
			{
				
			}
		}
		// -------------------------------------------------------------------
		// Send a time string to the display
		public void DisplayTime(PenaltyAndTime timeData)
		{
			string timestr = timeData.time;
			
			// Parse up time string and get into a 6 digit form
			// Display sign wants 0x80 followed by 6 digits in reverse: 0x80FFF.SSS
			if (timestr.Contains(".") == false)
			{
				timestr += ".000";
			}
			string[] parts = timestr.Split('.');
			string seconds = parts[0];
			if (seconds.Length > 3)
			{
				seconds = seconds.Substring(0,3);
			}
			while (seconds.Length < 3)
			{
				seconds = "0" + seconds;
			}
			string fraction = parts[1];
			if (fraction.Length > 3)
			{
				fraction = fraction.Substring(0,3);
			}
			while (fraction.Length < 3)
			{
				fraction += "0";
			}
			string dtime = seconds + fraction;
			
			char[] timeArr = dtime.ToCharArray();
			Array.Reverse(timeArr);
			
			// check for need to reverse digits for this display
			if (reverseDisplayDigits == true)
			{
				Array.Reverse(timeArr);
			}
			// Convert penalty into a single byte
			// 0-8 is cone count, 9 is a DNF
			string penaltyString;
			if (string.IsNullOrEmpty(timeData.penalty) == true)
			{
				penaltyString = "0";
			}
			else
			{
				penaltyString = timeData.penalty.ToUpperInvariant();
			}
			// Default to no penalty (ascii '0')
			var penalty = System.Text.Encoding.UTF8.GetBytes("0");
			if (string.IsNullOrEmpty(penaltyString) == false)
			{
				if (penaltyString.Contains("D"))
				{
					penalty[0] += 9;
				}
                if (Int32.TryParse(penaltyString, out int iVal) == true)
                {
                    if (iVal > 8)
                    {
                        iVal = 8;
                    }
                    penalty[0] += (byte)iVal;
                }
            }


			
			//convert the string to a byte array here:
			var dbuf = System.Text.Encoding.UTF8.GetBytes(timeArr);
			var header = new byte[1];
			var trailer = new byte[2];
			if (sendPenalty == false)
			{
				header[0] = 0x80;  //Race America header
			}
			else
			{
				header[0] = 0x88; // MJTiming header (0x88 or ';')
			}
			trailer[0] = 0x0d;
			trailer[1] = 0x0a;
			// Primary display first
			try
			{
				if (string.IsNullOrEmpty(DisplayPortName) == true)
				{
					return;
				}
				// If not the same port, then we need to do all the serial IO crud
				if ((DisplayPortName != TimerPortName) && (DisplayPortName != ""))
				{
					// First see if we have to open the display port
					if ((DisplayComPort == null) || (DisplayComPort.IsOpen == false))
					{
                        DisplayComPort = new SerialPort(DisplayPortName, 9600, Parity.None, 8, StopBits.One)
                        {
                            Handshake = Handshake.None,
                            DtrEnable = false,  // stop arduino reset
                            ReadTimeout = 50    // Enough time for quite a few characters
                        };
                        DisplayComPort.Open();
						//The workaround for unhandled finalizer expections
						GC.SuppressFinalize(DisplayComPort.BaseStream);
					}
					// Port is open, so send the message
					//showMsg("Sending time out display port: " + timestr + "\n");
					DisplayComPort.Write(header,0,header.Length);
					DisplayComPort.Write(dbuf,0,dbuf.Length);
					if (sendPenalty)
					{
						TimerComPort.Write(penalty,0,penalty.Length);
					}
					DisplayComPort.Write(trailer,0,trailer.Length);
				}
				else
				{
					// Same port as timer -- we don't play with that serial port here
					if ((TimerComPort != null) && (TimerComPort.IsOpen == true))
					{
						// Port is open, so send the message
						TimerComPort.Write(header,0,header.Length);
						TimerComPort.Write(dbuf,0,dbuf.Length);
						if (sendPenalty)
						{
							TimerComPort.Write(penalty,0,penalty.Length);
						}
						TimerComPort.Write(trailer,0,trailer.Length);
					}
					else
					{
						//showMsg("Display port is not open\n");
					}
				}
			}
			catch (Exception e)
			{
				showMsg(e.Message + "\n");
			}
			// Secondary display
			try
			{
				if (string.IsNullOrEmpty(secondaryDisplayPortName) == true)
				{
					return;
				}
				// If not the same port, then we need to do all the serial IO crud
				if ((secondaryDisplayPortName != TimerPortName) && (secondaryDisplayPortName != ""))
				{
					// First see if we have to open the display port
					if ((secondaryDisplayComPort == null) || (secondaryDisplayComPort.IsOpen == false))
					{
                        secondaryDisplayComPort = new SerialPort(secondaryDisplayPortName, 9600, Parity.None, 8, StopBits.One)
                        {
                            Handshake = Handshake.None,
                            DtrEnable = false,  // stop arduino reset
                            ReadTimeout = 50    // Enough time for quite a few characters
                        };
                        secondaryDisplayComPort.Open();
						//The workaround for unhandled finalizer expections
						GC.SuppressFinalize(secondaryDisplayComPort.BaseStream);
					}
					// Port is open, so send the message
					//showMsg("Sending time out display port: " + timestr + "\n");
					secondaryDisplayComPort.Write(header,0,header.Length);
					secondaryDisplayComPort.Write(dbuf,0,dbuf.Length);
					if (sendPenalty == true)
					{
						TimerComPort.Write(penalty,0,penalty.Length);
					}
					secondaryDisplayComPort.Write(trailer,0,trailer.Length);
				}
				else
				{
					// Same port as timer -- we don't play with that serial port here
					if ((TimerComPort != null) && (TimerComPort.IsOpen == true))
					{
						// Port is open, so send the message
						TimerComPort.Write(header,0,header.Length);
						TimerComPort.Write(dbuf,0,dbuf.Length);
						if (sendPenalty == true)
						{
							TimerComPort.Write(penalty,0,penalty.Length);
						}
						TimerComPort.Write(trailer,0,trailer.Length);
					}
					else
					{
						//showMsg("Display port is not open\n");
					}
				}
			}
			catch (Exception e)
			{
				showMsg(e.Message + "\n");
			}
		}
		// -------------------------------------------------------------------
		// This is our timer data handler
		// Caution -- separate thread running this!
		public void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs args)
		{
			string inputBuffer = "";
			var nextbyte = new byte[1];
			System.Text.Encoding encode = System.Text.Encoding.UTF8;
			try
			{
				showMsg("Input: ");
				while (true)
				{
					nextbyte[0] = (byte)TimerComPort.ReadByte();
					if ((nextbyte[0] < 0x20) || (nextbyte[0] > 0x7e))
					{
						showMsg("(0x" + BitConverter.ToString(nextbyte) + ")");
					}
					else
					{
						showMsg(encode.GetString(nextbyte));
					}
					if ((nextbyte[0] == 10) || (nextbyte[0] == 13))
					{
						showMsg("\n");
						break;
					}
					if (nextbyte[0] == 128)
					{
						inputBuffer += "R";
					}
					else
					{
						inputBuffer += encode.GetString(nextbyte);
					}
				} ;
			}
			catch (Exception e)
			{
				showMsg(e.Message + "\n");
				return;
			}

			// First discard anything too short
			if (inputBuffer.Length < 6)
			{
				return;
			}

			// showMsg("\nInput:" + inputBuffer + "\n");
			
			// Ignore T-Link battery messages
			// Battery format is ZB <bA><bB><bC><bD><bE><bF><bG><bZ> cr
			if (inputBuffer.Contains("ZB"))
			{
				return;
			}
			// Check for ZW message from T-Link (RF strengths)
			// The format is ZW <rA><rB><rC><rD><rE><rF><rG> 0 cr
			if (inputBuffer.Contains("ZW"))
			{
				string radioA = "0";
				string radioB = "0";
				int indexA = inputBuffer.IndexOf("ZW");
				indexA += 2;
				if (indexA < inputBuffer.Length)
				{
					string byteval = inputBuffer.Substring(indexA,1);
					// simpler and safer to just do some comparisons instead of math
					if ((byteval == "0") || (byteval == "1") || (byteval == "2") || (byteval == "3"))
					{
						radioA = "7";
					}
					else
					{
						radioA = "0";
					}
					indexA += 1;  // B radio signal
					byteval = inputBuffer.Substring(indexA,1);
					// simpler and safer to just do some comparisons instead of math
					if ((byteval == "0") || (byteval == "1") || (byteval == "2") || (byteval == "3"))
					{
						radioB = "7";
					}
					else
					{
						radioB = "0";
					}
					
				}
				// Build a status message
				// Format: Sdddddd
				// First digit is start beam status
				// Second digit is stop beam status
				// 0x01 bit: radio good (0) or bad (1)
				// 0x02 bit: sync good (0) or bad (1)
				// 0x04 bit: alignment good (0) or bad (1)
				string newMSg = radioA + radioB + "0000";
				timingEvent("Q", newMSg);
				return;
			}
			
			// Match Adddddd or Bdddddd up to Zdddddd
			var reg = new Regex(@"^([A-Z])([0-9]+)$");
			Match m;
			
			// Extract any start or stop events in buffer
			m = reg.Match(inputBuffer);
			if (m.Success == false)
			{
				// Probably an info message from the receiver (we get a bunch)
				if (configData.GetField("logTimingMessages", "Value").Contains("Y") == true)
				{
					showMsg(inputBuffer + "\n");
				}
				return;
			}
			
			GroupCollection g = m.Groups;
			string sval = g[2].Value;
            if (UInt32.TryParse(sval, out uint time) == false)
            {
                showMsg("Received invalid time string: " + inputBuffer + "\n");
                return;
            }
            while (sval.Length > 6)
			{
				// T-Link comes in with a 9 digit timestamp, so chop it down to the last 6 digits
				sval = sval.Substring(1);
			}
			timingEvent(g[1].Value, sval);
		}
		// -------------------------------------------------------------------
		// This is our timer data handler
		// Caution -- separate thread running this!
		public void BarcodeDataReceived(object sender, SerialDataReceivedEventArgs args)
		{
			string inputBuffer = "";
			var nextbyte = new byte[1];
			System.Text.Encoding encode = System.Text.Encoding.UTF8;
			try
			{
				// Keep reading until we time out
				while (true)
				{
					nextbyte[0] = (byte)barcodeComPort.ReadByte();
					inputBuffer += encode.GetString(nextbyte);
				} ;
			}
			catch (TimeoutException)
			{
				// just fall through
			}
			catch (Exception e)
			{
				showMsg(e.Message + "\n");
				return;
			}
			
			inputBuffer = inputBuffer.Trim();
			if (configData.GetField("logTimingMessages", "Value").Contains("Y") == true)
			{
				showMsg("barcode data: " + inputBuffer + "\n");
			}
			timingEvent("barcode", inputBuffer);
		}
		// -------------------------------------------------------------------
		public bool StartBarcode()
		{
			try
			{
				barcodePortName = configData.GetField("barcodeReaderPort", "Value");
                barcodeComPort = new SerialPort(barcodePortName, 9600, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.None,
                    DtrEnable = false,  // stop arduino reset
                    NewLine = "\r",
                    ReadTimeout = 50    // Enough time for quite a few characters
                };
                barcodeComPort.DataReceived += BarcodeDataReceived;
				barcodeComPort.Open();
				//The workaround for unhandled finalizer expections
				GC.SuppressFinalize(barcodeComPort.BaseStream);
				showMsg("barcode reading on port: " + barcodePortName + "\n");
				return true;
			}
			catch
			{
				if (string.IsNullOrEmpty(barcodePortName) == false)
				{
					showMsg("Unable to open barcode port: " + barcodePortName + "\n");
				}
				return false;
			}
		}
		// -------------------------------------------------------------------
		public void StopBarcode()
		{
			try
			{
				if ((barcodePortName != null) && (barcodeComPort.IsOpen == true))
				{
					barcodeComPort.Close();
					barcodeComPort = null;
				}
			}
			catch
			{
				
			}
		}
	}
	
}