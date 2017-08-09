using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {

    public partial class MonitorAjustes : Form {
        public MonitorAjustes() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_id { get; set; }
        public string user_depto { get; set; }
        private string selectedajuste;

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM tbajusteslineas WHERE fsajusteid IN " + 
                    "(SELECT fsid FROM tbajustes WHERE fscreatedby = '" + usuario + "')";
                sqlquery = "SELECT * FROM tbajusteslineas ORDER BY fsid";
                sqlquery = "SELECT * FROM tbajustes WHERE fsid >= 99 ORDER BY fsid";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata();
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            try {
                selectedajuste = dataGridView1[0, e.RowIndex].Value.ToString();
                filllineas(selectedajuste);
            } catch (Exception) {

            }
        }
        private void filllineas(string ajusteid) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM tbajusteslineas WHERE fsajusteid = " + ajusteid + ";";
                
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                conn.Close();
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView2.DataSource = table;
                tabControl1.SelectedIndex = 1;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
