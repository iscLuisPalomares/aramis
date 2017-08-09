using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class FilesPO : Form {
        public FilesPO() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string user_id { get; set; }
        public string tipo { get; set; }
        public string user_depto { get; set; }
        public string user_deptoid { get; set; }
        public string gerenteid { get; set; }
        public string poid { get; set; }

        public void getdata_for_fileslist() {
            try {
                string query = "SELECT fsname FROM po_files WHERE fscotid = '" + poid + "'";
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
                    //MessageBox.Show(@"\\mexfs01\TJTemp\Opardo\FOLIOS\requisiciones\" + reqid + @"\" + listBox1.SelectedItem.ToString());
                    if (!System.IO.Directory.Exists(@"C:\aramisproject\porders\" + poid)) {
                        System.IO.Directory.CreateDirectory(@"C:\aramisproject\porders\" + poid);
                    }
                    System.IO.File.Copy(@"\\mexfs01\TJTemp\Opardo\FOLIOS\porders\" + poid + @"\" + listBox1.SelectedItem.ToString(),
                        @"C:\aramisproject\porders\" + poid + @"\" + listBox1.SelectedItem.ToString(), true);
                    System.Diagnostics.Process.Start(@"C:\aramisproject\porders\" + poid + @"\" + listBox1.SelectedItem.ToString());
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
