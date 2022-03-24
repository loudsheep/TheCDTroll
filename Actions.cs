using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheCDTrollGUI
{
    // class for executing commands
    public class Actions
    {
        public static readonly Dictionary<string, Func<string[], int>> functions = new Dictionary<string, Func<string[], int>>() 
        {
            // every element represents a command and a method for executing it {command, method},
            { "open", Actions.Open },
            { "close", Actions.Close },
            { "msg", Actions.Msg },
            { "start", Actions.Start },
            { "rickroll", Actions.RickRoll },
            //{ "fish", Actions.Fish }, // doesn't work, idk why, just doesn't work and don't use it
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
            MessageBoxIcon boxIcon = MessageBoxIcon.None;
            string message = "";
            string title = "Title";
            if(args.Length > 0)
            {
                for(int i=0; i<args.Length; i++ )
                {
                    if(args[i].StartsWith("-"))
                    {
                        if (args[i] == "-error") boxIcon = MessageBoxIcon.Error;
                        else if (args[i] == "-warning") boxIcon = MessageBoxIcon.Warning;
                        else if (args[i] == "-information") boxIcon = MessageBoxIcon.Information;
                        else if (args[i] == "-question") boxIcon = MessageBoxIcon.Question;
                        else if(args[i] == "-t")
                        {
                            if(i < args.Length - 1)
                            {
                                title = args[i + 1];
                                i++;
                            }
                        }
                        else
                        {
                            message += " " +  args[i];
                        }
                    }
                    else
                    {
                        message += " " + args[i];
                    }
                }
            }
            else
            {
                message = "Message";
            }


            new Thread(() => MessageBox.Show(message, title, MessageBoxButtons.OK, boxIcon)).Start();

            return 0;
        }

        public static int Start(string[] args)
        {
            if (args.Length == 0) return -1;

            for(int i=0; i<args.Length; i++)
            {
                try
                {
                    Process.Start(args[i]);
                }
                catch(Exception) { }
            }
            return 0;
        }

        public static int RickRoll(string[] args)
        {
            Start(new string[]{"https://www.youtube.com/watch?v=dQw4w9WgXcQ"});

            return 0;
        }
    }
}
