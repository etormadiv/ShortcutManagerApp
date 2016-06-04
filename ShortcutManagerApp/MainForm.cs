using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace ShortcutManagerApp
{
    public partial class MainForm : Form
    {
        string configFile = "ShortcutManager.conf";

        Dictionary<int, string> programPaths = new Dictionary<int, string>(36);

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists(configFile))
            {
                using (StreamWriter sw = new StreamWriter(configFile))
                {
                    sw.WriteLine(
                    #region configFileContent
@"1=
2=
3=
4=
5=
6=
7=
8=
9=
10=
11=
12=
13=
14=
15=
16=
17=
18=
19=
20=
21=
22=
23=
24=
25=
26=
27=
28=
29=
30=
31=
32=
33=
34=
35=
36="
                    #endregion
);
                }
            }
            try
            {
                using (StreamReader sr = new StreamReader(configFile))
                    while (!sr.EndOfStream)
                    {
                        string[] row = sr.ReadLine().Split('=');
                        programPaths.Add(int.Parse(row[0]), row[1]);
                    }
            }
            catch (Exception)
            {
                MessageBox.Show("Error found on " + configFile + ".");
            }

            for (int i = 1; i <= 36; i++)
            {
                FieldInfo field = typeof(MainForm).GetField("pictureBox" + i, BindingFlags.NonPublic | BindingFlags.Instance);

                PictureBox pb = (PictureBox)field.GetValue(this);
                if (programPaths[i] != "")
                {
                    Icon ico = Icon.ExtractAssociatedIcon(programPaths[i]);
                    pb.Image = ico.ToBitmap();
                    pb.Tag = programPaths[i];
                }
            }
        }

        private void allPictureBoxes_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            PictureBox pb = (PictureBox)sender;
            if (pb.Tag != null)
            {
                Process.Start((string)pb.Tag);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Executable files|*.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Icon ico = Icon.ExtractAssociatedIcon(ofd.FileName);
                pb.Image = ico.ToBitmap();
                pb.Tag = ofd.FileName;
                programPaths[int.Parse(pb.Name.Remove(0, 10))] = ofd.FileName;

                using (StreamWriter sw = new StreamWriter(configFile))
                {
                    for (int i = 1; i <= 36; i++)
                    {
                        sw.WriteLine(i + "=" + programPaths[i]);
                    }
                    sw.Flush();
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)((ContextMenuStrip)((ToolStripItem)sender).Owner).SourceControl;
            pb.Image = Properties.Resources.addicon;
            pb.Tag = null;

            programPaths[int.Parse(pb.Name.Remove(0, 10))] = "";

            using (StreamWriter sw = new StreamWriter(configFile))
            {
                for (int i = 1; i <= 36; i++)
                {
                    sw.WriteLine(i + "=" + programPaths[i]);
                }
                sw.Flush();
            }
        }
    }
}
