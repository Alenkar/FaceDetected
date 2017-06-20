using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.IO;
//Подключение библиотек Emgu CV
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;

using DirectShowLib;
namespace ProjectOfGOD
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        public int _CameraIndex;

        private void Settings_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(186, 216, 225);

            button1.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;

            label1.BackColor = Color.FromArgb(228, 236, 237);
            label1.ForeColor = Color.FromArgb(81, 120, 145);
            label2.BackColor = Color.FromArgb(228, 236, 237);
            label2.ForeColor = Color.FromArgb(81, 120, 145);
            label3.BackColor = Color.FromArgb(81, 120, 145);
            label4.BackColor = Color.FromArgb(81, 120, 145);
            label5.BackColor = Color.FromArgb(228, 236, 237);
            label6.BackColor = Color.FromArgb(228, 236, 237);

            button1.BackColor = Color.FromArgb(86, 155, 180);
            button1.ForeColor = Color.FromArgb(228, 236, 237);
            button2.BackColor = Color.FromArgb(218, 26, 50);
            button2.ForeColor = Color.FromArgb(228, 236, 237);

            comboBox1.BackColor = Color.FromArgb(86, 155, 180);
            comboBox1.ForeColor = Color.FromArgb(228, 236, 237);
            comboBox2.BackColor = Color.FromArgb(86, 155, 180);
            comboBox2.ForeColor = Color.FromArgb(228, 236, 237);

            List<KeyValuePair<int, string>> ListCamerasData = new List<KeyValuePair<int, string>>();
            DsDevice[] SystemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            int DeviceIndex = 0;
            foreach (DsDevice Camera in SystemCameras)
            {
                ListCamerasData.Add(new KeyValuePair<int, string>(DeviceIndex, Camera.Name));
                DeviceIndex++;
            }
            comboBox1.DataSource = null;
            comboBox1.Items.Clear();
            comboBox1.DataSource = new BindingSource(ListCamerasData, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";

            string[] Settings = File.ReadAllLines("Settings.txt");
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = Convert.ToInt32(Settings[0].Split().ToArray()[1]);
            }
            comboBox2.SelectedIndex = comboBox2.FindString(Settings[1].Split().ToArray()[1]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KeyValuePair<int, string> SelectedItem;
            if (comboBox1.Items.Count > 0)
            {
                SelectedItem = (KeyValuePair<int, string>)comboBox1.SelectedItem;
            }
            string[] str = comboBox2.SelectedItem.ToString().Split('x');
            Form1 main = this.Owner as Form1;
            if (main != null)
            {
                main.width = Convert.ToInt32(str[0]);
                main.heihgt = Convert.ToInt32(str[1]);
                main.WebCam_index_2 = comboBox2.SelectedIndex;
                main.pictureBox1.Image = null;
            }
            string[] Settings = File.ReadAllLines("Settings.txt");
            if (comboBox1.Items.Count > 0)
            {
                Settings[0] = Settings[0].Split().ToArray()[0] + " " + comboBox1.SelectedIndex;
            }
            else
            {
                Settings[0] = Settings[0].Split().ToArray()[0] + " 0";
            }
            Settings[1] = Settings[1].Split().ToArray()[0] + " " + comboBox2.SelectedItem.ToString();
            File.WriteAllLines("Settings.txt", Settings);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            comboBox2.SelectedIndex = 0;
            string[] str = comboBox2.SelectedItem.ToString().Split('x');
            Form1 main = this.Owner as Form1;
            if (main != null)
            {
                main.width = Convert.ToInt32(str[0]);
                main.heihgt = Convert.ToInt32(str[1]);
                main.WebCam_index_2 = comboBox1.SelectedIndex;
                main.pictureBox1.Image = null;
            }
            string[] Settings = File.ReadAllLines("Settings.txt"); if (comboBox1.Items.Count > 0)
            {
                Settings[0] = Settings[0].Split().ToArray()[0] + " " + comboBox1.SelectedIndex;
            }
            else
            {
                Settings[0] = Settings[0].Split().ToArray()[0] + " 0";
            }
            Settings[1] = Settings[1].Split().ToArray()[0] + " " + comboBox2.SelectedItem.ToString();
            File.WriteAllLines("Settings.txt", Settings);
        }
    }
}
