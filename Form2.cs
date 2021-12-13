using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace TheCDTrollGUI
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            listBox1.Items.AddRange(Connection.GetLocalAddreses());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("No network selected");
                return;
            }

            if (!(listBox1.SelectedItem is IPAddress))
            {
                MessageBox.Show("Something is wrong with IP");
                return;
            }

            IPAddress ip = (IPAddress)listBox1.SelectedItem;
            string command = textBox1.Text;

            UDPSender.SendBroadcastMessage(ip, 13000, command);
        }
    }
}
