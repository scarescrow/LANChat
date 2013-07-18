using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net.NetworkInformation;

namespace LANChat
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public List<string> list
        {
            get;
            set;
        }
        List<person> people = new List<person>();    

        private void Form2_Load(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(path + "\\LANChat"))
            {
                Directory.CreateDirectory(path + "\\LANChat");
            }
            path = path + "\\LANChat";
            if (!File.Exists(path + "\\settings.xml"))
            {
                XmlTextWriter xw = new XmlTextWriter(path + "\\settings.xml", Encoding.UTF8);
                xw.WriteStartElement("People");
                xw.WriteEndElement();
                xw.Close();
            }
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(path + "\\settings.xml");
            foreach (XmlNode xnode in xdoc.SelectNodes("People/Person"))
            {
                person p = new person();
                p.Name = xnode.SelectSingleNode("Name").InnerText;
                p.IP = xnode.SelectSingleNode("IP").InnerText;
                p.Port = xnode.SelectSingleNode("Port").InnerText;
                people.Add(p);
                ListViewItem li = new ListViewItem();
                if (pinger(p.IP) == "Online")
                    li.ForeColor = Color.Green;
                else
                    li.ForeColor = Color.Red;
                li.Text = p.Name;
                listView1.Items.Add(li);
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            list = new List<string>();
            if (listView1.SelectedItems.Count > 0)
            {
                list.Add(people[listView1.SelectedItems[0].Index].Name);
                list.Add(people[listView1.SelectedItems[0].Index].IP);
                list.Add(people[listView1.SelectedItems[0].Index].Port);
            }
            else
            {
                list.Add("");
                list.Add("");
                list.Add("");
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LANChat";
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(path + "\\settings.xml");
            XmlNode xnode = xdoc.SelectSingleNode("People");
            xnode.RemoveAll();
            foreach (person p in people)
            {
                XmlNode xtop = xdoc.CreateElement("Person");
                XmlNode xname = xdoc.CreateElement("Name");
                XmlNode xip = xdoc.CreateElement("IP");
                XmlNode xport = xdoc.CreateElement("Port");
                xname.InnerText = p.Name;
                xip.InnerText = p.IP;
                xport.InnerText = p.Port;
                xtop.AppendChild(xname);
                xtop.AppendChild(xip);
                xtop.AppendChild(xport);
                xdoc.DocumentElement.AppendChild(xtop);
            }
            xdoc.Save(path + "/settings.xml");
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                try
                {
                    if (MessageBox.Show("You are about to delete a contact. Are you sure?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        string q = people[listView1.SelectedItems[0].Index].IP;
                        listView1.Items.Remove(listView1.SelectedItems[0]);
                        foreach (person p in people.Reverse<person>())
                        {
                            if (p.IP == q)
                            {
                                people.Remove(p);
                                break;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string pinger(string ad)
        {
            var ping = new Ping();
            var options = new PingOptions { DontFragment = true };

            //just need some data. this sends 10 bytes.
            var buffer = Encoding.ASCII.GetBytes(new string('z', 10));
            var host = ad;

            try
            {
                var reply = ping.Send(host, 60, buffer, options);
                if (reply == null)
                {
                    return "Offline";
                }

                if (reply.Status == IPStatus.Success)
                {
                    return "Online";
                }
                else
                {
                    return "Offline";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }
    }

    class person
    {
        public string Name
        {
            get;
            set;
        }
        public string IP
        {
            get;
            set;
        }
        public string Port
        {
            get;
            set;
        }
    }
}
