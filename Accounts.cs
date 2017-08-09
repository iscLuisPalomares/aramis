using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class Accounts : Form {
        public Accounts() {
            InitializeComponent();
        }
        public string usuario       { get; set; }
        public string user_id       { get; set; }
        public string tipo          { get; set; }
        public string user_depto    { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT accounts.id as 'ID', acctnumber as 'Cuenta', "
                    + "acctdesc as 'Descripcion', deps.name as 'Departamento'"
                    + ", accounts.createdate as 'Fecha Creacion' "
                    + "FROM Accounts JOIN Deptos deps on accounts.depto = deps.id";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["ID"].Width = 70;
                dataGridView1.Columns["Cuenta"].Width = 150;
                dataGridView1.Columns["Descripcion"].Width = 150;
                dataGridView1.Columns["Departamento"].Width = 180;
                dataGridView1.Columns["Fecha Creacion"].Width = 150;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata();
            dataGridView1.AllowUserToResizeRows = false;
        }
        private void button2_Click(object sender, EventArgs e) {
            CreateAccount ca = new CreateAccount();
            ca.usuario = usuario;
            ca.FormClosing += Cu_FormClosing;
            ca.ShowDialog();
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
        private void button4_Click(object sender, EventArgs e) {

        }
        private void editarRegistroToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("editar informacion del registro seleccionado");
        }
    }
}
