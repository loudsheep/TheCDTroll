using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheCDTrollGUI
{
    // class for executing commands
    public class Actions
    {
        public static readonly Dictionary<string, Func<string[], int>> functions = new Dictionary<string, Func<string[], int>>() 
        {
            // every element represents a command and a method for executing it {command, method},
            {"open", Actions.Open},
            {"close", Actions.Close},
            {"msg", Actions.Msg},
        };

        public static int ExecuteCommand(string command)
        {
            if (command.Length == 0) return -1;

            command = Regex.Replace(command, @"\s+", " ");
            string[] split = command.Split(' ');
            if(split.Length == 1)
            {
                return functions[split[0].ToLower()](new string[] { });
            } 
            else if(split.Length > 1)
            {
                string[] arguments = new string[split.Length - 1];
                Array.Copy(split, 1, arguments, 0, arguments.Length);
                return functions[split[0].ToLower()](arguments);
            }

            return -1;
        }

        public static int Open(string[] args)
        {
            if (CDTray.GetCDDrivesLetters().Length == 0) return -1;

            CDTray.OpenAllCDDrives();
            return 0;
        }

        public static int Close(string[] args)
        {
            if (CDTray.GetCDDrivesLetters().Length == 0) return -1;

            CDTray.CloseAllCDDrives();
            return 0;
        }

        public static int Msg(string[] args)
        {
            if(args.Length > 0)
            {
                Console.WriteLine(string.Join(" ", args));
            }
            else
            {
                Console.WriteLine("message");
            }

            return 0;
        }
    }
}
