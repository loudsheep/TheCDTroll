using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;

namespace TheCDTrollGUI
{
    public partial class Form1 : Form
    {
        private Thread listeningThread;
        private bool ignoreCommands = false;
        private bool respondToOwnCommands = true;

        public Form1()
        {
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);

            InitializeComponent();
            SetUpNotify();
            InitThread();
        }

        static void AddressChangedCallback(object sender, EventArgs e)
        {
            Restart();            
        }

        private static void Restart()
        {
            Application.Restart();
            Environment.Exit(0);
        }

        private int ExecuteCommandWhenListening(string command, IPAddress senderAddress)
        {
            if (command.StartsWith("hostdiscovery") || command.StartsWith("hostresponse"))
            {
                Actions.ExecuteCommand(command);
                return 0;
            }

            if (!respondToOwnCommands)
            {
                foreach (var add in Connection.GetLocalAddreses())
                {
                    if (add.Equals(senderAddress)) return 1;
                }
            }

            if(!ignoreCommands)
            {
                Actions.ExecuteCommand(command);
                return 0;
            }

            return 1;
        }

        private void InitThread()
        {
            var IPs = Connection.GetLocalAddreses();
            listBox1.Items.Clear();

            for (int i = 0; i < IPs.Length; i++)
            {
                listBox1.Items.Add(IPs[i]);
            }

            listeningThread = new Thread(Connection.ListenOnAddressUDP)
            {
                IsBackground = true
            };
            listeningThread.Start(new Connection.ListenData() { ip = null, func = this.ExecuteCommandWhenListening });
        }

        private void SetUpNotify()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item1 = new MenuItem();
            item1.Index = 0;
            item1.Text = "Open";
            item1.Click += OpenItemClick;
            menu.MenuItems.Add(item1);

            MenuItem item2 = new MenuItem();
            item2.Index = 1;
            item2.Text = "Exit";
            item2.Click += ExitItemClick;
            menu.MenuItems.Add(item2);

            notifyIcon1.ContextMenu = menu;
        }

        private string SMTH(string msg)
        {
            string log = "(" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + ")   " + msg;
            if(listBox2.InvokeRequired)
            {
                listBox2.Invoke(new Action(() => listBox2.Items.Add(log)));
            }
            else
            {
                listBox2.Items.Add(log);
            }

            if(msg == "open")
            {
                CDTray.OpenAllCDDrives();
            }
            else if(msg == "close")
            {
                CDTray.CloseAllCDDrives();
            }

            return "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Hide();
            notifyIcon1.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
            this.WindowState = FormWindowState.Minimized;
            Hide();
            notifyIcon1.Visible = true;
        }

        private void ExitItemClick(object Sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Hide();
            notifyIcon1.Visible = false;
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        private const uint SW_RESTORE = 0x09;

        public static void Restore(Form form)
        {
            if (form.WindowState == FormWindowState.Minimized)
            {
                ShowWindow(form.Handle, SW_RESTORE);
            }
        }

        private void OpenItemClick(object Sender, EventArgs e)
        {
            //Restore(this);
            Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
            //Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if(Control.ModifierKeys == Keys.Control)
            {
                ignoreCommands = !ignoreCommands;
                this.BackColor = ignoreCommands ? Color.Red : SystemColors.Control;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.respondToOwnCommands = checkBox1.Checked;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            ScanForm f = new ScanForm();
            f.ShowDialog();
        }
    }
}
