using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
//Подключение библиотек Emgu CV
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;

namespace ProjectOfGOD
{
    public partial class Data_grid : Form
    {
        public Data_grid()
        {
            InitializeComponent();
        }
        //назад
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //переименование
        private void button1_Click(object sender, EventArgs e)
        {
            bool b = true;
            if (textBox1.Text != "")
            {
                string str = textBox1.Text;
                foreach (var item in str)
                {
                    if (Char.IsNumber(item) != true)
                    {
                        b = false;
                    }
                }
                if (b == true)
                {
                    if (Convert.ToInt32(textBox1.Text) < File.ReadLines(@"DataBase\Face_List\Face_name.txt").Count() + 1 &&
                        Convert.ToInt32(textBox1.Text) > 0)
                    {
                        int index = Convert.ToInt32(textBox1.Text) - 1;
                        string txt1 = File.ReadLines(@"DataBase\Face_List\Face_name.txt").Skip(index).First();
                        List<string> List = new List<string>();
                        string[] txt2 = File.ReadAllLines(@"DataBase\Face_List\Face_name.txt");
                        for (int i = 0; i < txt2.Length; i++)
                        {
                            if (i != index)
                            {
                                List.Add(txt2[i]);
                            }
                            else
                            {
                                List.Add(textBox2.Text);
                            }
                        }
                        Form1 main = this.Owner as Form1;
                        if (main != null)
                        {
                            main.Face_name.Clear();
                            main.Face_name = List.GetRange(0, List.Count);
                            File.WriteAllLines(@"DataBase\Face_List\Face_name.txt", List);
                        }
                        List = new List<string>();
                        Fill_dataGrid();
                    }
                }
            }
        }
        //удаление из бд
        private void button3_Click(object sender, EventArgs e)
        {
            bool b = true;
            if (textBox1.Text != "")
            {
                string str = textBox1.Text;
                foreach (var item in str)
                {
                    if (Char.IsNumber(item) != true)
                    {
                        b = false;
                    }
                }
                if (b == true)
                {
                    if (b == true)
                    {
                        if (Convert.ToInt32(textBox1.Text) < File.ReadLines(@"DataBase\Face_List\Face_name.txt").Count() + 1 &&
                            Convert.ToInt32(textBox1.Text) > 0)
                        {
                            if (textBox2.Text != null)
                            {
                                int index = Convert.ToInt32(textBox1.Text) - 1;
                                string path = @"DataBase\Face\" + index + @"\";
                                DirectoryInfo di = new DirectoryInfo(path);
                                foreach (FileInfo fi in di.GetFiles())
                                {
                                    try
                                    {
                                        fi.Delete();
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                                path = @"DataBase\Face\" + index;
                                Directory.Delete(path);
                                if (File.Exists(@"DataBase\Face_List\Face_name.txt") == true)
                                {
                                    string txt1 = File.ReadLines(@"DataBase\Face_List\Face_name.txt").Skip(index).First();

                                    List<string> List = new List<string>();
                                    string[] txt2 = File.ReadAllLines(@"DataBase\Face_List\Face_name.txt");

                                    for (int i = 0; i < txt2.Length; i++)
                                    {
                                        if (txt2[i] != txt1)
                                        {
                                            List.Add(txt2[i]);
                                        }
                                    }
                                    string[] Dir_name = Directory.GetDirectories(@"DataBase\Face");
                                    string dir = @"DataBase\Face";
                                    int iter_dir = 0;
                                    foreach (var item in Dir_name)
                                    {
                                        try
                                        {
                                            Directory.Move(item, (dir + @"\" + iter_dir));
                                            iter_dir++;
                                        }
                                        catch (Exception)
                                        {
                                            iter_dir++;
                                        }
                                    }
                                    File.WriteAllLines(@"DataBase\Face_List\Face_name.txt", List);
                                    Form1 main = this.Owner as Form1;
                                    if (main != null)
                                    {
                                        main.Face_name.Clear();
                                        main.Face_name = List.GetRange(0, List.Count);
                                        main.ClassOfImage--;
                                        File.WriteAllText("ClassOfImage.txt", main.ClassOfImage.ToString());
                                        if (main.ClassOfImage.ToString() == "100")
                                        {
                                            File.Delete("recognizer_train.yml");
                                            main.StartTrainBool = false;
                                            main.TrainBool = false;
                                            main.recognizer.Dispose();
                                            main.result.Label = 0;
                                            main.recognizer = new LBPHFaceRecognizer(3, 8, 8, 8, 123.0);
                                        }
                                        else
                                        {
                                            main.StartTrainBool = false;
                                            main.TrainBool = true;
                                            main.recognizer.Dispose();
                                            main.result.Label = 0;
                                            main.recognizer = new LBPHFaceRecognizer(3, 8, 8, 8, 123.0);
                                            main.Train();
                                        }
                                    }
                                }
                                Fill_dataGrid();
                            }
                        }
                    }
                }
            }
        }
        //заполнение таблицы
        public void Fill_dataGrid()
        {
            string path = @"DataBase\Face_List\Face_name.txt";
            if (File.Exists(path) == true)
            {
                string[] str = File.ReadAllLines(path);
                dataGridView1.RowCount = str.Length;
                for (int i = 0; i < str.Length; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = i + 1;
                    dataGridView1.Rows[i].Cells[1].Value = str[i];
                    /*
                    dataGridView1.Rows[i].Cells[2].Value = Image.FromFile("123.jpg");
                    dataGridView1.Rows[i].Height = 100;
                    */
                }
            }
        }

        private void Data_grid_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(186, 216, 225);
            button1.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderSize = 0;
            dataGridView1.BorderStyle = BorderStyle.None;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            button1.BackColor = Color.FromArgb(86, 155, 180);
            button2.BackColor = Color.FromArgb(86, 155, 180);
            button3.BackColor = Color.FromArgb(218, 26, 50);
            button4.BackColor = Color.FromArgb(218, 26, 50);
            button1.ForeColor = Color.FromArgb(228, 236, 237);
            button2.ForeColor = Color.FromArgb(228, 236, 237);
            button3.ForeColor = Color.FromArgb(228, 236, 237);
            button4.ForeColor = Color.FromArgb(228, 236, 237);

            label1.BackColor = Color.FromArgb(228, 236, 237);
            label1.ForeColor = Color.FromArgb(81, 120, 145);
            label2.BackColor = Color.FromArgb(228, 236, 237);
            label2.ForeColor = Color.FromArgb(81, 120, 145);
            label3.BackColor = Color.FromArgb(228, 236, 237);
            label3.ForeColor = Color.FromArgb(81, 120, 145);
            label4.BackColor = Color.FromArgb(81, 120, 145);
            label5.BackColor = Color.FromArgb(81, 120, 145);

            comboBox1.BackColor = Color.FromArgb(86, 155, 180);
            comboBox1.ForeColor = Color.FromArgb(228, 236, 237);

            Fill_dataGrid();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                Delete_SH();
                dataGridView1.Rows.Clear();
            }
        }

        //Удаление шаблона
        public void Delete_SH()
        {
            if (Directory.Exists(@"DataBase\Face\") == true)
            {
                if (File.Exists("recognizer_train.yml") == true)
                {
                    File.Delete("recognizer_train.yml");
                }
                string path = @"DataBase\Face\";
                deleteSub(path);
                Directory.Delete(path);
                if (File.Exists(@"DataBase\Face_List\Face_name.txt") == true)
                {
                    File.Delete(@"DataBase\Face_List\Face_name.txt");
                }
                //File.Create(@"DataBase\Face_List\Face_name.txt");
            }
            Form1 main = this.Owner as Form1;
            if (main != null)
            {
                main.ImageList = new List<Image<Gray, byte>>();
                main.LabelsList = new List<int>();
                main.Face_name = new List<string>();
                main.ClassOfImage = 100;
                File.WriteAllText("ClassOfImage.txt", main.ClassOfImage.ToString());
                main.StartTrainBool = false;
                main.TrainBool = false;
                main.recognizer.Dispose();
                main.result.Label = 0;
                main.recognizer = new LBPHFaceRecognizer(3, 8, 8, 8, 123.0);
            }
        }
        //удаление каталога
        public void deleteSub(string path)
        {
            int index_l = File.ReadAllLines(@"DataBase\Face_List\Face_name.txt").Length;
            for (int i = 0; i < index_l; i++)
            {
                path = @"DataBase\Face\" + i + @"\";
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo fi in di.GetFiles())
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch (Exception)
                    {

                    }
                }
                path = @"DataBase\Face\" + i;
                Directory.Delete(path);
            }
        }
    }
}
