using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class Departamentos : Form {
        public Departamentos() {
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
                string sqlquery = "SELECT deptos.id as 'ID', "
                    + "name AS 'Nombre', "
                    + "moddesc AS 'Descripcion', "
                    + "gerente.fulname AS 'Gerente', "
                    + "gerente.id as 'ID Gerente' "
                    + "FROM deptos JOIN users gerente ON deptos.gerente = gerente.id";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Nombre"].Width = 130;
                dataGridView1.Columns["Descripcion"].Width = 130;
                dataGridView1.Columns["Gerente"].Width = 180;
                dataGridView1.Columns["ID Gerente"].Visible = false;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata();
        }
        private void button2_Click(object sender, EventArgs e) {
            CreateDepto cu = new CreateDepto();
            cu.usuario = usuario;
            cu.user_id = user_id;
            cu.ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            try {
                EditDepto ed = new EditDepto();
                ed.user_id = user_id;
                ed.usuario = usuario;
                ed.deptoid = dataGridView1["ID", e.RowIndex].Value.ToString();
                ed.FormClosed += Ed_FormClosed;
                ed.ShowDialog();
            } catch (Exception) { }
        }
        private void Ed_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
    }
}
