using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace LANChat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        Socket mySocket;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        public string name = "";
        int check = 0;

        private string GetLocalIP()
        {
            IPHostEntry myHost;
            myHost = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in myHost.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] RecievedData = new byte[1500];
                RecievedData = (byte[])aResult.AsyncState;
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string RecievedMessage = aEncoding.GetString(RecievedData);
                if (check == 1)
                {
                    label5.Text = "Last Message: " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "\n";
                }
                if (name == "")
                    ListMessages.Items.Add("Friend: " + RecievedMessage + "\n");
                else
                    ListMessages.Items.Add(name + ": " + RecievedMessage + "\n");
                buffer = new byte[1500];
                mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception)
            {
                MessageBox.Show("Your contact is not connected with you!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            txtLocalIP.Text = GetLocalIP();
            //txtRemoteIP.Text = GetLocalIP();
            txtLocalPort.Text = "80";
            txtRemotePort.Text = "80";
            label5.Text = "";
            ListMessages.ItemHeight = 20;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(txtLocalIP.Text), Convert.ToInt32(txtLocalPort.Text));
                mySocket.Bind(epLocal);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            try
            {
                epRemote = new IPEndPoint(IPAddress.Parse(txtRemoteIP.Text), Convert.ToInt32(txtRemotePort.Text));
                mySocket.Connect(epRemote);
            }
            catch(Exception)
            {
                MessageBox.Show("Your contact is offline.");
                return;
            }
            buffer = new byte[1500];
            mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            BtnConnect.Text = "Connected";
            BtnConnect.Enabled = false;
            check = 1;
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (check == 0)
            {
                MessageBox.Show("Please Connect First!");
                return;
            }
            if (TxtMessage.Text == "")
            {
                return;
            }
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] SendingMessage = new byte[1500];
            SendingMessage = aEncoding.GetBytes(TxtMessage.Text);
            try
            {
                mySocket.Send(SendingMessage);
            }
            catch (Exception)
            {
                MessageBox.Show("");
            }
            label5.Text = "Last Message: " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "\n"; 
            ListMessages.Items.Add("You: " + TxtMessage.Text + "\n");
            TxtMessage.Text = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
            List<string> now = form.list;
            txtRemoteIP.Text = now[2];
            txtRemotePort.Text = now[1];
            name = now[0];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3(txtRemoteIP.Text, txtRemotePort.Text);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListMessages.Items.Clear();
        }
    }
}
