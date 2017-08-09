using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ImprimirPOdev : Form {
        public ImprimirPOdev() {
            InitializeComponent();
        }
        public string idpo;
        public string user_id;
        public string usuario;
        private void button1_Click(object sender, EventArgs e) {
            try {
                pictureBox1.Image.Dispose();
            } catch (Exception) {

            }
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "") {
                return;
            }
            string filePathedit = @"C:\Users\opardo\MOR\logoposeyedit.jpg";
            Bitmap bitmap = null;
            using (var stream = File.OpenRead(textBox1.Text)) {
                bitmap = (Bitmap)Bitmap.FromStream(stream);
            }
            using (bitmap)
            using (var graphics = Graphics.FromImage(bitmap))
            using (var font = new Font("Arial", 10, FontStyle.Regular)) {
                graphics.DrawString(textBox4.Text, font, Brushes.Black, float.Parse(textBox2.Text), float.Parse(textBox3.Text));
                try {
                    bitmap.Save(filePathedit);
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
                cargarimagen();
            }
        }
        private void imgbtnfile_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG|*.png";
            if (dialog.ShowDialog() == DialogResult.OK) {
                List<string> files = new List<string>();
                string file;
                files = dialog.FileNames.ToList();
                file = dialog.FileName;
                textBox1.Text = "";
                foreach (string onefile in files) {
                    textBox1.Text += onefile;
                }
            }
        }
        private void button1_Click_1(object sender, EventArgs e) {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += PrintPage;
            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.Document = pd;
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                pd.Print();
            }
        }
        private void PrintPage(object o, PrintPageEventArgs e) {
            try {
                if (File.Exists(@"C:\Users\opardo\MOR\logoposeyedit.jpg")) {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(@"C:\Users\opardo\MOR\logoposeyedit.jpg");
                    Rectangle m = e.MarginBounds;
                    if ((double)img.Width / (double)img.Height > (double)m.Width / (double)m.Height) {
                        m.Height = (int)((double)img.Height / (double)img.Width * (double)m.Width);
                    } else {
                        m.Width = (int)((double)img.Width / (double)img.Height * (double)m.Height);
                    }
                    e.Graphics.DrawImage(img, m);
                }
            } catch (Exception) {

            }
        }
        private void cargarimagen() {
            pictureBox1.Image = Image.FromFile(@"C:\Users\opardo\MOR\logoposeyedit.jpg");
            
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }
    }
}
