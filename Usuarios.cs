using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class Usuarios : Form {
        public Usuarios() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_depto { get; set; }
        public string user_id { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT Users.id as 'ID', "
                    + "username as 'Nombre de Usuario', "
                    + "fulname as 'Nombre Completo', "
                    + "depas.name as 'Departamento', "
                    + "tipo as 'Tipo', "
                    + "users.createdby as 'Creado por', "
                    + "users.createdate as 'Fecha de creacion' FROM Users JOIN Deptos depas ON users.depto = depas.id";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["ID"].Width = 60;
                dataGridView1.Columns["Nombre de Usuario"].Width = 125;
                dataGridView1.Columns["Nombre Completo"].Width = 190;
                dataGridView1.Columns["Departamento"].Width = 120;
                dataGridView1.Columns["Tipo"].Width = 100;
                dataGridView1.Columns["Fecha de creacion"].Width = 160;
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
            CreateUser cu = new CreateUser();
            cu.usuario = usuario;
            cu.FormClosing += Cu_FormClosing;
            cu.ShowDialog();
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
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            try {
                string userid = dataGridView1["ID", e.RowIndex].Value.ToString();
                EditUser eu = new EditUser();
                eu.user_id = userid;
                eu.FormClosed += Eu_FormClosed;
                eu.ShowDialog();
                
            } catch (Exception) {
                MessageBox.Show("Se presento un problema, intente de nuevo");
            }
        }
        private void Eu_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
    }
}
