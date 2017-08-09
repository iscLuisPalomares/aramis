using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class Suppliers : Form {
        public Suppliers() {
            InitializeComponent();
        }
        public string usuario       { get; set; }
        public string tipo          { get; set; }
        public string user_id       { get; set; }
        public string user_depto    { get; set; }

        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM asl";
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
        private void button2_Click(object sender, EventArgs e) {
            
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            ChangeFlag ca = new ChangeFlag();
            ca.aslid = dataGridView1[0, e.RowIndex].Value.ToString();
            string bandera = dataGridView1["flag", e.RowIndex].Value.ToString();
            string nombre = dataGridView1["suppname", e.RowIndex].Value.ToString();

            ca.flag = dataGridView1["flag", e.RowIndex].Value.ToString();
            ca.suppname = dataGridView1["suppname", e.RowIndex].Value.ToString();
            ca.suppcity = dataGridView1["suppcity", e.RowIndex].Value.ToString();
            ca.suppcontactname = dataGridView1["suppcontactname", e.RowIndex].Value.ToString();
            ca.suppemail = dataGridView1["suppemail", e.RowIndex].Value.ToString();
            ca.suppphone = dataGridView1["suppphone", e.RowIndex].Value.ToString();
            ca.pais = dataGridView1["Pais", e.RowIndex].Value.ToString();

            ca.FormClosed += Ca_FormClosed;
            ca.ShowDialog();
        }
        private void Ca_FormClosed(object sender, FormClosedEventArgs e) {
            
        }
        private void button2_Click_1(object sender, EventArgs e) {
            CreateASL cu = new CreateASL();
            cu.usuario = usuario;
            cu.ShowDialog();
        }
    }
}
