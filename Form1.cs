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

namespace TheCDTrollGUI
{
    public partial class Form1 : Form
    {
        private Thread listeningThread;

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

        private void CheckCDDrives()
        {
            // if pc does not have any cd trays then terminate app
            if (CDTray.GetCDDrivesLetters().Length == 0)
            {
                Close();
                return;
            }
        }

        private void InitThread()
        {
            var IPs = Connection.GetLocalAddreses();
            listBox1.Items.Clear();

            for (int i = 0; i < IPs.Length; i++)
            {
                listBox1.Items.Add(IPs[i]);
            }

            Thread listeningThread = new Thread(Connection.ListenOnAddressUDP)
            {
                IsBackground = true
            };
            listeningThread.Start(new Connection.ListenData() { ip = null, func = Actions.ExecuteCommand });
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

        private void AbortThread()
        {
            try
            {
                listeningThread.Abort();
            }
            catch (Exception) { }
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
            //CheckCDDrives();
        }

        private void ExitItemClick(object Sender, EventArgs e)
        {
            this.Close();
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
    }
}
