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

namespace LANChat
{
    public partial class Form3 : Form
    {
        public Form3(string one, string two)
        {
            InitializeComponent();
            textBox2.Text = one;
            textBox3.Text = two;
        }

        List<person> people = new List<person>();

        private void button1_Click(object sender, EventArgs e)
        {
            person p = new person();
            p.Name = textBox1.Text;
            p.IP = textBox3.Text;
            p.Port = textBox2.Text;
            people.Add(p);
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
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

        private void Form3_Load(object sender, EventArgs e)
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
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
