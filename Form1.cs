using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using System.IO;

namespace WallpaperChanger
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        UInt32 SPI_SETWALL = 0x14;
        UInt32 SPIF_UPDATE = 0x01;
        UInt32 SPIF_SWEDINI = 0x2;

        // creates time object
        DateTime now = DateTime.Now;
        Random random = new Random();
        string hours = "0";
        String getFile;
        string folderpath;
        string completepath;
        string choice;
        int imageCount = 0;
        int intervals = 0;
        bool photosloaded = false;
        bool folderloaded = false;
        bool unlocked = true;

        Dictionary<string, string> dict = new Dictionary<string, string>();

        
        public Form1()
        {
            InitializeComponent();
        }

        // setup and runs application
        private void button2_Click(object sender, EventArgs e)
        {
            if (photosloaded == true)
            {
                hours = now.Hour.ToString();
                textBox1.Text = hours;
                // Sets wallpaper
                SystemParametersInfo(SPI_SETWALL, 0, dict[hours], SPIF_UPDATE | SPIF_SWEDINI);
                timer1.Enabled = true;
            }

            else if (folderloaded == true)
            {
                if (radioButton1.Checked == true)
                {
                    // 5 mins
                    intervals = 300000;
                }
                if (radioButton2.Checked == true)
                {
                    // 30 mins
                    intervals = 1800000;
                }
                if (radioButton3.Checked == true)
                {
                    // 1 hour
                    intervals = 3600000;
                }

                timer2.Interval = intervals;
                // get directory
                var filepath = Directory.GetFiles(completepath);
                // gets random file from directory
                choice = filepath[random.Next(filepath.Length)].ToString();
                pictureBox1.Image = Image.FromFile(choice);
                // Sets wallpaper
                SystemParametersInfo(SPI_SETWALL, 0, choice, SPIF_UPDATE | SPIF_SWEDINI);
                timer2.Enabled = true;
            }
            else
            {
                textBox1.Text = "Please Load Photo Libary.";
            }
        }

        // 24hr timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            now = DateTime.Now;
            hours = now.Hour.ToString();
            textBox1.Text = "Running...";
            // Sets wallpaper
            SystemParametersInfo(SPI_SETWALL, 0, dict[hours], SPIF_UPDATE | SPIF_SWEDINI);
        }

        // uncapped timer
        private void timer2_Tick(object sender, EventArgs e)
        {
            var filepath = Directory.GetFiles(completepath);
            choice = filepath[random.Next(filepath.Length)].ToString();
            textBox1.Text = "Running...";
            // Sets wallpaper
            SystemParametersInfo(SPI_SETWALL, 0, choice, SPIF_UPDATE | SPIF_SWEDINI);
        }


        // stores image into dictionary
        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filelist = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string done = filelist[0];
            pictureBox1.Image = Image.FromFile(done);
            if (imageCount <= 24)
            {
                textBox1.Text = imageCount.ToString();
                dict.Add(imageCount.ToString(), done);
                imageCount++;
            }
            else
            {
                textBox1.Text = "Photos Ready!";
                photosloaded = true;

            }
        }

        // gets path from draged file
        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect= DragDropEffects.None;
            }
        }

        // saves photo
        private void save_button_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            // searalization is the process of converting info into bytes to save / transmit them
            File.WriteAllText(saveFileDialog1.FileName + ".txt", new JavaScriptSerializer().Serialize(dict));
            textBox1.Text = "Photo Libary Saved!!";
            
        }

        // loads photo libary
        private void load_button_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                getFile = openFileDialog1.FileName;
                // deserialization rebuilds the information back to its original state
                var dict_load = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(getFile));
                textBox1.Text = "Photo Libary Loaded!!";
                dict = dict_load;
                pictureBox1.Image = Image.FromFile(dict["0"]);
                photosloaded = true;
                folderloaded = false;
            }
        }

        // loads photo folder
        private void LoadFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderpath = folderBrowserDialog1.SelectedPath;
                completepath = folderpath;
                folderloaded = true;
                photosloaded = false;
                textBox1.Text = "Folder Loaded!";
            }
        }

        // switches between 24hr and dynamic wallpaper modes
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (unlocked == true)
            {
                UI_Changer.Text = "24hr";
                textBox1.Text = "Load Photo Folder.";
                LoadFolder.Visible = true;
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                radioButton3.Visible = true;
                load_button.Visible = false;
                save_button.Visible = false;
                timer1.Enabled = false;
                timer2.Enabled = false;
            }
            if (unlocked == false)
            {
                UI_Changer.Text = "Uncapped";
                textBox1.Text = "Load or Create Photo Libary.\r\n";
                LoadFolder.Visible = false;
                radioButton1.Visible = false;
                radioButton2.Visible = false;
                radioButton3.Visible = false;
                load_button.Visible = true;
                save_button.Visible = true;
                timer1.Enabled = false;
                timer2.Enabled = false;
            }
            unlocked = !unlocked;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
