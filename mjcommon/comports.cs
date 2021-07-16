// Lots of work to get the COM port list in friendly form
using System.Management;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;

namespace RaceBeam
{
    internal static class ProcessConnection
    {

        public static ConnectionOptions ProcessConnectionOptions()
        {
            var options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;
            return options;
        }

        public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options, string path)
        {
            var connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + machineName + path);
            connectScope.Options = options;
            connectScope.Connect();
            return connectScope;

        }
    }
    public class COMPortInfo
    {
        public string Name;
        public string Description;

        public static List<COMPortInfo> GetCOMPortsInfo()
        {
        	// Should be able to use this instead of the WMI query
			// string[] ports = SerialPort.GetPortNames();
			// However, it appears to not have any description available

        	var comPortInfoList = new List<COMPortInfo>();
            ConnectionOptions options = ProcessConnection.ProcessConnectionOptions();
            ManagementScope connectionScope = ProcessConnection.ConnectionScope(Environment.MachineName, options, @"\root\CIMV2");

            var objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");

            var comPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);

            using (comPortSearcher)
            {
                string caption = null;
                foreach (var obj in comPortSearcher.Get())
                {
                    if (obj != null)
                    {
                        object captionObj = obj["Caption"];
                        if (captionObj != null)
                        {
                            caption = captionObj.ToString();
                            if (caption.Contains("(COM"))
                            {
                                var comPortInfo = new COMPortInfo();
                                comPortInfo.Name = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);
                                comPortInfo.Description = caption;
                                comPortInfoList.Add(comPortInfo);
                            }
                        }
                    }
                }
            }
            return comPortInfoList;
        }
    }
}