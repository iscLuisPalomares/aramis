using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ApprovedCotizaciones : Form {
        public ApprovedCotizaciones() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string user_id { get; set; }
        public string tipo { get; set; }
        public string user_depto { get; set; }
        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM tbcotizaciones WHERE fsstatus = 'Cotizacion Aprobada'";
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
        private void Cu_FormClosing(object sender, FormClosingEventArgs e) {
            label1.Text = label1.Text;
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void editarRegistroToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("editar informacion del registro seleccionado");
        }
        private void Rm_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }

    }
}
