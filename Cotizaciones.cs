using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {

    public partial class Cotizaciones : Form {
        public Cotizaciones() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_id { get; set; }
        public string user_depto { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM tbcotizaciones";
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
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            FilesCot fc = new FilesCot();
            fc.reqid = dataGridView1[0, e.RowIndex].Value.ToString();
            fc.usuario = usuario;
            fc.ShowDialog();
        }
    }
}
