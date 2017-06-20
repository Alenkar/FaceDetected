using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows;
using System.Runtime.InteropServices;
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
    public partial class Form1 : Form
    {
        public bool Inicialisation_webcam = false;
        public bool datagrid = true;
        public bool save = false;
        public bool taskA = true;
        public bool taskB = false;
        public string HistCalc = "0";
        static int WebCam_index = 0;
        public int WebCam_index_2;
        public int width = 320;
        public int heihgt = 240;

        //public FaceRecognizer recognizer = new LBPHFaceRecognizer(3, 8, 8, 8, 123.0);
        public FaceRecognizer recognizer = new LBPHFaceRecognizer(3, 8, 8, 8, 123.0);

        public FaceRecognizer.PredictionResult result;
        public CascadeClassifier face = new CascadeClassifier(@"cf\haarcascade_frontalface_default.xml");
        Rectangle[] facesDetected;
        public HistogramBox histo = new HistogramBox();
        static VideoCapture video = new VideoCapture(null);
        static int NumberOfTrain = 5;
        public int Integer = 0;
        public int PorogClassOfImage = 100;

        public int ClassOfImage;
        public List<string> Face_name = new List<string>();

        public int check = 0;
        public string No_Of_Human = "0";
        public string Name_Of_Human = "0";
        public List<Image<Gray, Byte>> ImageList = new List<Image<Gray, Byte>>();
        public List<int> LabelsList = new List<int>();
        public bool StartTrainBool = false;
        public bool TrainBool = false;

        public Form1()
        {
            InitializeComponent();
        }
        //инициализация начального кадра
        static Bitmap bb = (Bitmap)Image.FromFile("Clear.png");
        static Image<Bgr, Byte> ImageFrame = new Image<Bgr, Byte>(bb);
        static Image<Gray, Byte> ImageFrameGray = ImageFrame.Copy().Convert<Gray, Byte>();
        static Image<Gray, Byte> RecImg = ImageFrameGray.Copy();
        Image<Gray, Byte> CopyImageRectangle = RecImg.Copy();
        public Image<Gray, byte> ImageLBP = ImageFrameGray;

        //Обновление элементов формы
        public void RefreshElement()
        {
            //обновление картинки            
            pictureBox1.Image = ImageFrame.ToBitmap();
            pictureBox1.Refresh();
        }
        //Создание элементов формы
        public void CreateElement()
        {
            //инициализация нового кадра
            if (video.Grab() == true)
            {
                ImageFrame = new Image<Bgr, Byte>(video.QueryFrame().Bitmap).Flip(FlipType.Horizontal).Resize(width, heihgt, Inter.Cubic);
            }
            //конвертация кадра в серую палитру
            ImageFrameGray = ImageFrame.Convert<Gray, Byte>().Resize(width, heihgt, Inter.Cubic);
        }
        public bool Get_webCam()
        {
            bool b;
            List<KeyValuePair<int, string>> ListCamerasData = new List<KeyValuePair<int, string>>();
            DsDevice[] SystemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            int DeviceIndex = 0;
            foreach (DsDevice Camera in SystemCameras)
            {
                ListCamerasData.Add(new KeyValuePair<int, string>(DeviceIndex, Camera.Name));
                DeviceIndex++;
            }
            if (ListCamerasData.Count == 0)
            {
                b = false;
                Inicialisation_webcam = true;
            }
            else
            {
                b = true;
            }
            return b;
        }
        //Запуск таймера
        public void button1_Click(object sender, EventArgs e)
        {
            if (Get_webCam() == true)
            {
                if (Inicialisation_webcam == true)
                {
                    video.Dispose();
                    video = new VideoCapture(WebCam_index);
                    Inicialisation_webcam = false;
                }
                timer1.Start();
            }
        }
        //Остановка таймера
        public void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
        //Функции на тике таймера
        public void timer1_Tick(object sender, EventArgs e)
        {
            //Классы обработки на таймере
            CreateElement();
            FaceDetection();
            RefreshElement();

        }
        //Нахождение лица
        public void FaceDetection()
        {
            //Фильтры
            CvInvoke.GaussianBlur(ImageFrameGray, ImageFrameGray, new Size(5, 5), 1.0);
            CvInvoke.EqualizeHist(ImageFrameGray, ImageFrameGray);
            ImageFrame = ImageFrame.Resize(width, heihgt, Inter.Cubic);

            facesDetected = face.DetectMultiScale(ImageFrameGray.Resize(width, heihgt, Inter.Cubic), 1.1, 3, new Size(60, 60));

            dataGridView1.RowCount = facesDetected.Length;
            int i = 1;
            int j = 0;
            foreach (Rectangle face1 in facesDetected)
            {
                RecImg = CopyImage(ImageFrameGray, face1).Resize(100, 100, Inter.Cubic);
                if (TrainBool == true)
                {
                    result = recognizer.Predict(RecImg);
                }
                if (StartTrainBool == true)
                {
                    if (result.Label < PorogClassOfImage)
                    {
                        if (datagrid == true)
                        {
                            Face_name.Add(textBox_Name.Text);
                            datagrid = false;
                        }
                        string dir = @"DataBase\Face\" + (ClassOfImage - 100);
                        Directory.CreateDirectory(dir);
                        string name = dir + @"\" + Integer + ".png";
                        RecImg.Save(name);
                        TrainRecognizer(RecImg, Convert.ToInt32(No_Of_Human));
                    }
                }
                if (result.Label == 0)
                {
                    Name_Of_Human = "Unknown face " + i.ToString();
                    i++;
                }
                else if (result.Label > PorogClassOfImage - 1)
                {
                    Name_Of_Human = Face_name[result.Label - 100];
                }
                //отрисовка на кадре найденных лиц
                ImageFrame.Draw(face1, new Bgr(Color.Aqua), 2);
                ImageFrame.Draw(Name_Of_Human, new Point(face1.X - 25, face1.Y - 5),
                    FontFace.HersheyComplex, 0.85, new Bgr(Color.Aqua),
                    2, LineType.AntiAlias, false);
                //вывод таблицы имён
                dataGridView1.Rows[j].Cells[0].Value = j + 1;
                dataGridView1.Rows[j].Cells[1].Value = Name_Of_Human;
                j++;
            }
            //вывод таблицы
            if (StartTrainBool == false)
            {
                datagrid = true;
            }
        }
        //Копирование изображения
        public Image<Gray, Byte> CopyImage(Image<Gray, Byte> ImageFrame, Rectangle face1)
        {
            CopyImageRectangle = ImageFrame.Copy(face1);
            return CopyImageRectangle;
        }
        //Кнопка вызывающая тренировку алгоритма
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox_Name.Text != "")
            {
                if (textBox_Name.Text != "Enter name")
                {
                    StartTrainBool = true;
                }
            }
        }
        //Метод тренировки
        public void TrainRecognizer(Image<Gray, Byte> ImageToList, int LabelToList)
        {
            //запись данных для тренинга
            if (Integer < NumberOfTrain)
            {
                ImageList.Add(ImageToList);
                LabelsList.Add(ClassOfImage);
                Integer++;
            }
            //тренировка
            else if (Integer == NumberOfTrain)
            {
                File.WriteAllLines(@"DataBase\Face_List\Face_name.txt", Face_name);
                Integer = 0;
                //recognizer.Train(ImageList.ToArray(), LabelsList.ToArray());                
                Train();
                File.WriteAllText("ClassOfImage.txt", ClassOfImage.ToString());
                recognizer.Save("recognizer_train.yml");
                StartTrainBool = false;
                TrainBool = true;
            }
        }
        //Сохранение шаблона
        public void Save_SH()
        {
            timer1.Stop();
            recognizer.Save("recognizer_train.yml");
            File.WriteAllLines(@"DataBase\Face_List\Face_name.txt", Face_name);
            timer1.Start();
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
                File.Create(@"DataBase\Face_List\Face_name.txt");
            }
            ImageList = new List<Image<Gray, byte>>();
            LabelsList = new List<int>();
            Face_name = new List<string>();
            ClassOfImage = 100;
            File.WriteAllText("ClassOfImage.txt", ClassOfImage.ToString());
            StartTrainBool = false;
            TrainBool = false;
            recognizer.Dispose();
            result.Label = 0;
            recognizer = new LBPHFaceRecognizer(3, 8, 8, 8, 123.0);
        }
        //обучение
        public void Train()
        {
            if (File.Exists(@"DataBase\Face\0\0.png") == true)
            {
                if (File.Exists(@"DataBase\Face_List\Face_name.txt"))
                {
                    int index = 100;
                    string[] NameDir = Directory.GetDirectories(@"DataBase\Face");
                    ImageList = new List<Image<Gray, byte>>();
                    LabelsList = new List<int>();
                    List<List<string>> NameFile = new List<List<string>>();
                    foreach (var item in NameDir)
                    {
                        NameFile.Add(Directory.GetFiles(item).ToList());
                    }
                    foreach (var item1 in NameFile)
                    {
                        foreach (var item2 in item1)
                        {
                            Bitmap bim = (Bitmap)Image.FromFile(item2);
                            Image<Gray, byte> Image_to_list = new Image<Gray, byte>(bim);
                            ImageList.Add(Image_to_list);
                            LabelsList.Add(Convert.ToInt32(index));
                        }
                        index++;
                    }
                    ClassOfImage = index;
                    File.WriteAllText("ClassOfImage.txt", ClassOfImage.ToString());
                    string[] face_list = File.ReadAllLines(@"DataBase\Face_List\Face_name.txt");
                    Face_name = new List<string>();
                    foreach (var item in face_list)
                    {
                        Face_name.Add(item);
                    }
                    StartTrainBool = false;
                    TrainBool = true;
                    recognizer.Dispose();
                    result.Label = 0;
                    recognizer = new LBPHFaceRecognizer(3, 8, 8, 8, 123.0);
                    recognizer.Train(ImageList.ToArray(), LabelsList.ToArray());
                    recognizer.Save("recognizer_train.yml");
                }
            }
        }
        //переход к редактированию бд
        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Data_grid dg = new Data_grid();
            dg.Owner = this;
            dg.ShowDialog();
        }
        //Удаление шаблона
        private void button_Delete_SH_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Delete_SH();
            timer1.Start();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(186, 216, 225);
            button1.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderSize = 0;
            button4.FlatAppearance.BorderSize = 0;
            button5.FlatAppearance.BorderSize = 0;
            button6.FlatAppearance.BorderSize = 0;
            dataGridView1.BorderStyle = BorderStyle.None;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            button2.BackColor = Color.FromArgb(218, 26, 50);
            button1.BackColor = Color.FromArgb(212, 239, 84);
            button3.ForeColor = Color.FromArgb(86, 155, 180);
            button4.BackColor = Color.FromArgb(86, 155, 180);
            button4.ForeColor = Color.FromArgb(228, 236, 237);
            button5.BackColor = Color.FromArgb(86, 155, 180);
            button5.ForeColor = Color.FromArgb(228, 236, 237);            
            button6.BackColor = Color.FromArgb(86, 155, 180);
            button6.ForeColor = Color.FromArgb(228, 236, 237);
            label1.BackColor = Color.FromArgb(81, 120, 145);
            label2.BackColor = Color.FromArgb(81, 120, 145);
            label3.BackColor = Color.FromArgb(228, 236, 237);
            label3.ForeColor = Color.FromArgb(81, 120, 145);
            label4.BackColor = Color.FromArgb(228, 236, 237);
            textBox_Name.BackColor = Color.FromArgb(228, 236, 237);
            if (File.Exists("ClassOfImage.txt") == true)
            {
                ClassOfImage = Convert.ToInt32(File.ReadAllText("ClassOfImage.txt"));
            }
            else
            {
                ClassOfImage = 100;
                File.WriteAllText("ClassOfImage.txt", ClassOfImage.ToString());
            }
            Directory.CreateDirectory(@"DataBase\Face\");
            Directory.CreateDirectory(@"DataBase\Face_List");
            if (File.Exists(@"DataBase\Face_List\Face_name.txt") == true)
            {
                string[] Face_str = File.ReadAllLines(@"DataBase\Face_List\Face_name.txt");
                foreach (var item in Face_str)
                {
                    Face_name.Add(item);
                }
            }
            else
            {
                File.Create(@"DataBase\Face_List\Face_name.txt");
            }
            if (File.Exists("recognizer_train.yml") == true)
            {
                recognizer.Load("recognizer_train.yml");
                StartTrainBool = false;
                TrainBool = true;
            }
            List<KeyValuePair<int, string>> ListCamerasData = new List<KeyValuePair<int, string>>();
            DsDevice[] SystemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            int DeviceIndex = 0;
            foreach (DsDevice Camera in SystemCameras)
            {
                ListCamerasData.Add(new KeyValuePair<int, string>(DeviceIndex, Camera.Name));
                DeviceIndex++;
            }
            if (ListCamerasData.Count == 0)
            {
                video = new VideoCapture();
            }
            else
            {
                WebCam_index = WebCam_index_2;
                video.Dispose();
                video = new VideoCapture(WebCam_index);
            }
            if (File.Exists("Settings.txt"))
            {
                string[] Settings = File.ReadAllLines("Settings.txt");
                string[] str = Settings[1].Split().ToArray()[1].Split('x'); ;
                width = Convert.ToInt32(str[0]);
                heihgt = Convert.ToInt32(str[1]);
            }
            else
            {
                string[] str = { "Index_camera 0", "Index_resolution 320x240" };
                File.WriteAllLines("Settings.txt", str);
            }
        }
        //открытие окна информации
        private void button3_Click(object sender, EventArgs e)
        {
            Help h = new Help();
            h.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Settings st = new Settings();
            st.Owner = this;
            st.ShowDialog();

        }
    }
}
