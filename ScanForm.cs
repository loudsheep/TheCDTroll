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
using static System.Windows.Forms.ListView;

namespace TheCDTrollGUI
{
    public partial class ScanForm : Form
    {
        private int timeout = 1000;
        private static List<string[]> scanData = new List<string[]>();

        public ScanForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scanData.Clear();
            IPAddress[] ips = Connection.GetLocalAddreses();
            if (ips == null || ips.Length == 0)
            {
                MessageBox.Show("No network cards");
                return;
            }

            UDPSender.SendBroadcastMessage(ips[0], 13000, "hostdiscovery");
            Thread.Sleep(timeout);

            DataTable dt = new DataTable();
            dt.Columns.Add("IP");
            dt.Columns.Add("Host name");
            dt.Columns.Add("MAC");

            foreach(string[] data in scanData)
            {
                DataRow dataRow = dt.NewRow();
                dataRow.ItemArray = data;

                dt.Rows.Add(dataRow);
            }

            dataGridView1.DataSource = dt;

            for (int i=0; i<dataGridView1.Rows.Count; i++)
            {
                dataGridView1.AutoResizeRow(i, DataGridViewAutoSizeRowMode.AllCells);
            }
            for(int i=0; i<dataGridView1.Columns.Count; i++)
            {
                dataGridView1.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
            }
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
                response[i] = response[i].Replace(",", "\n");
            }

            scanData.Add(response);
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }
    }
}
