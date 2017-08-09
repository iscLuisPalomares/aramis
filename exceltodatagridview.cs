using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Linq;

namespace ComprasProject {
    public partial class exceltodatagridview : Form {
        public exceltodatagridview() {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e) {
            try {
                PasteClipboard();
            } catch (Exception) { MessageBox.Show("Fatal Error"); }
        }

        private void PasteClipboard() {
            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.Text)) {
                if (dataGridView1.RowCount > 0)
                    dataGridView1.Rows.Clear();

                string[] pastedRows = Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
                int j = 0;
                int columnas = int.Parse(pastedRows[0].Split(new char[] { '\t' }).Length.ToString());
                MessageBox.Show(columnas.ToString() + " " + dataGridView1.ColumnCount.ToString());
                while (dataGridView1.ColumnCount < columnas) {
                    dataGridView1.Columns.Add("added" + dataGridView1.ColumnCount.ToString(), "Columna nueva");
                }
                
                foreach (string pastedRow in pastedRows) {

                    string[] pastedRowCells = pastedRow.Split(new char[] { '\t' });
                    dataGridView1.Rows.Add();
                    int myRowIndex = dataGridView1.Rows.Count - 1;

                    using (DataGridViewRow myDataGridViewRow = dataGridView1.Rows[j]) {
                        for (int i = 0;i < pastedRowCells.Length;i++)
                            myDataGridViewRow.Cells[i].Value = pastedRowCells[i];
                    }
                    j++;
                }
            }
        }
        private void exceltodatagridview_Load(object sender, EventArgs e) {
            dataGridView1.Columns.Add("first", "Erste");
            dataGridView1.Columns.Add("second", "Zweite");
            dataGridView1.Columns.Add("third", "Dritte");
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                ContextMenuStrip m = new ContextMenuStrip();
                m.Items.Add("Paste");
                m.ItemClicked += M_ItemClicked;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        private void M_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            ToolStripItem btn = e.ClickedItem;
            if (btn.Text == "Paste") {
                try {
                    PasteClipboard();
                } catch (Exception) { MessageBox.Show("Fatal Error"); }
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.PrintPreviewControl.Zoom = 0.90;
            printPreviewDialog1.Height = 500;
            printPreviewDialog1.Width = 900;
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            Image img = Properties.Resources.PoseyLogo;
            //instanciar fuentes para el documento
            Font titulo = new Font("Arial", 18, FontStyle.Bold);
            Font subtitulo = new Font("Arial", 14, FontStyle.Regular);
            Font texto = new Font("Arial", 8, FontStyle.Regular);
            Font textobold = new Font("Arial", 8, FontStyle.Bold);
            //TITULO
            e.Graphics.DrawString("ORDEN DE COMPRA\n PURCHASE ORDER", titulo, Brushes.Black, 300f, 20f);
            //Imagen logo posey
            e.Graphics.DrawImage(img, 20, 20, 250, 100);
            //Informacion primaria
            e.Graphics.DrawString("Cam. Atingua a Tecate #16760 Interior 32-33", texto, Brushes.Black, 300f, 80f);
            e.Graphics.DrawString("Colonia Niños Heroes Este", texto, Brushes.Black, 300f, 92f);
            e.Graphics.DrawString("Tijuana B.C. Mexico", texto, Brushes.Black, 300f, 104f);
            e.Graphics.DrawString("TEL. (664) 969-87-37 FAX (664) 689 - 7167", texto, Brushes.Black, 300f, 116f);
            e.Graphics.DrawString("R.F.C. POS-980803-SE5", textobold, Brushes.Black, 300f, 128f);
            string textito = textBox4.Text;
            float stringsize = 0f;
            stringsize = e.Graphics.MeasureString(textito, subtitulo).Width;
            e.Graphics.DrawString(stringsize.ToString(), subtitulo, Brushes.BlueViolet, 30f, 100f);
            e.Graphics.DrawString(textito, subtitulo, Brushes.BlueViolet, 30f, 150f);
            //Numero de orden
            e.Graphics.DrawString("ORDEN No.", texto, Brushes.Black, 600f, 34f);
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e) {

        }

        private void button3_Click(object sender, EventArgs e) {
            MessageBox.Show(IsAuthenticated("", textBox1.Text, textBox2.Text).ToString());
        }
        public bool IsAuthenticated(string srvr, string usr, string pwd) {
            bool authenticated = false;

            try {
                DirectoryEntry entry = new DirectoryEntry(@"", @"POSEY\" + usr, pwd);
                object nativeObject = entry.NativeObject;
                authenticated = true;
            } catch (DirectoryServicesCOMException cex) {
                MessageBox.Show(cex.Message);
                //not authenticated; reason why is in cex
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                //not authenticated due to some other exception [this is optional]
            }
            return authenticated;
        }

        private void button4_Click(object sender, EventArgs e) {
            try {
                string[] arr = Split(textBox4.Text, int.Parse(textBox3.Text)).ToArray();
                string fin = "";
                foreach (string div in arr) {
                    fin += div + "\n";
                }
                MessageBox.Show(fin);
            } catch (Exception) {

            }
            
        }
        static IEnumerable<string> Split(string str, int chunkSize) {
            return Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        private void button5_Click(object sender, EventArgs e) {

        }
    }
}
