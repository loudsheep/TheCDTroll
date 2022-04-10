using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheCDTrollGUI
{
    public partial class ScanForm : Form
    {
        private int timeout = 1000;
        private static List<ListViewItem> hosts = new List<ListViewItem>();

        public ScanForm()
        {
            InitializeComponent();
            listView1.Anchor = AnchorStyles.Right | AnchorStyles.Left;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hosts.Clear();

            IPAddress[] ips = Connection.GetLocalAddreses();
            if (ips == null || ips.Length == 0)
            {
                MessageBox.Show("No network cards");
                return;
            }

            UDPSender.SendBroadcastMessage(ips[0], 13000, "hostdiscovery");
            Thread.Sleep(timeout);

            listView1.Items.Clear();
            listView1.Items.AddRange(hosts.ToArray());
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timeout = (int) numericUpDown1.Value;
        }

        public static void RegisterHostResponse(string[] response)
        {
            if(response == null || response.Length == 0)
            {
                response = new string[] {"-", "-", "-"};
            } else if(response.Length == 1)
            {
                response = response[0].Split(';');
            }

            for(int i=0; i<response.Length; i++)
            {
                response[i] = response[i].Replace(",", " ");
            }

            ListViewItem item = new ListViewItem(response);
            hosts.Add(item);
        }
    }
}
