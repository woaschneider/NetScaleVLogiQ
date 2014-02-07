using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;


namespace HWB.NETSCALE.FRONTEND.WPF
{
    public class HardewareInfo
    {
        private NetworkInterface[] NetworkAdapters = NetworkInterface.GetAllNetworkInterfaces();


        public string GetMacAdresse()
        {
            NetworkInterface[] NetworkAdapters = NetworkInterface.GetAllNetworkInterfaces();
            {
                return NetworkAdapters[0].GetPhysicalAddress().ToString();
            }
        }

        [DllImport("kernel32.dll")]
        private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer,
                                                        UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber,
                                                        ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags,
                                                        StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);

        public string GetVolumeSerialNumber2() // Festplatten-Seriennummer auslesen Methode 2
        {
            uint serNum = 0;
            uint maxCompLen = 0;
            StringBuilder VolLabel = new StringBuilder(256);
            UInt32 VolFlags = new UInt32();
            StringBuilder FSName = new StringBuilder(256); // File System Name

            string strDriveLetter = Application.StartupPath.Substring(0, 3);
            long Ret = GetVolumeInformation(strDriveLetter, VolLabel, (UInt32) VolLabel.Capacity, ref serNum,
                                            ref maxCompLen, ref VolFlags, FSName, (UInt32) FSName.Capacity);
            return Convert.ToString(serNum, 16).ToUpper();
        }
    }
}