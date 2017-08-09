using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class FilesAjuste : Form {
        public FilesAjuste() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string user_id { get; set; }
        public string tipo { get; set; }
        public string user_depto { get; set; }
        public string user_deptoid { get; set; }
        public string gerenteid { get; set; }
        public string ajusteid { get; set; }

        public void getdata_for_fileslist() {
            try {
                string query = "SELECT fsfilename FROM tbajustesfiles WHERE fsajusteid = '" + ajusteid + "'";
                listBox1.Items.Clear();
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(query, conn);
                DataTable cuentastable = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);
                DataSet ds = new DataSet();
                foreach (DataRow da in almacentb.Rows) {
                    listBox1.Items.Add(da[0].ToString());
                }
                conn.Close();
            } catch (SqlException e) {
                MessageBox.Show(e.ToString());
            }
        }
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listBox1.Items.Count > 0) {
                try {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "All files (*.*)|*.*";
                    saveFileDialog1.FileName = listBox1.SelectedItem.ToString();
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                        System.IO.File.Copy(@"\\mexfs01\TJTemp\Opardo\FOLIOS\ajustes\" + ajusteid + @"\" + listBox1.SelectedItem.ToString(),
                        saveFileDialog1.FileName, true);
                        MessageBox.Show(saveFileDialog1.FileName.ToString());
                        System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                    }
                } catch (Exception) {
                    MessageBox.Show("Error al abrir archivo");
                }
            }
        }
        private void FilesRequi_Load(object sender, EventArgs e) {
            getdata_for_fileslist();
        }
        private void button1_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
