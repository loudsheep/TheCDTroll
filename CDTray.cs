using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace TheCDTrollGUI
{
    public class CDTray
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendString")]
        public static extern int mciSendStringA(string lpstrCommand, string lpstrReturnString,
                            int uReturnLength, int hwndCallback);

        public static string[] GetDrivesLetters()
        {
            return Directory.GetLogicalDrives();
        }

        public static string[] GetCDDrivesLetters()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> letters = new List<string>();

            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.CDRom)
                {
                    letters.Add(drive.Name);
                }
            }
            return letters.ToArray();
        }

        public static bool IsDriveACDRom(string letter)
        {
            string[] letters = GetCDDrivesLetters();
            foreach (string i in letters)
            {
                if (i.StartsWith(letter)) return true;
            }
            return false;
        }

        public static void OpenAllCDDrives()
        {
            string[] drives = GetCDDrivesLetters();
            foreach(var drive in drives)
            {
                OpenDrive(drive);
            }

            if(drives.Length == 0)
            {
                Console.WriteLine("No CD Drives Found");
            }
        }

        public static void CloseAllCDDrives()
        {
            string[] drives = GetCDDrivesLetters();
            foreach (var drive in drives)
            {
                CloseDrive(drive);
            }

            if (drives.Length == 0)
            {
                Console.WriteLine("No CD Drives Found");
            }
        }

        public static void OpenDrive(string letter)
        {
            string returnString = "";
            if (IsDriveACDRom(letter))
            {
                mciSendStringA("open " + letter + ": type CDaudio alias drive" + letter,
                     returnString, 0, 0);
                mciSendStringA("set drive" + letter + " door open", returnString, 0, 0);
            }
            else
            {
                Console.WriteLine("No CD drives found");
            }
        }

        public static void CloseDrive(string letter)
        {
            string returnString = "";
            if (IsDriveACDRom(letter))
            {
                mciSendStringA("open " + letter + ": type CDaudio alias drive" + letter,
                     returnString, 0, 0);
                mciSendStringA("set drive" + letter + " door closed", returnString, 0, 0);
            }
            else
            {
                Console.WriteLine("No CD drives found");
            }
        }
    }
}
