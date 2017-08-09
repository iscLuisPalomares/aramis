using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class Delegados : Form {
        public Delegados() {
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
                string sqlquery = "select delegados.fsid as 'ID', "
                    + "usuarios.fulname as 'Nombre Completo', "
                    + "usuarios.username as 'Nombre de usuario', "
                    + "usuarios.id as 'User ID', "
                    + "delegados.fsvencimiento as 'Vencimiento', "
                    + "delegados.fsapproveajustes as 'Permiso de ajustes' "
                    + "from tbdelegados delegados join users usuarios on delegados.fsuserid = usuarios.id "
                    + "WHERE fsapproveajustes = 1 AND delegados.fsvencimiento > SYSDATETIME();";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Permiso de ajustes"].Visible = false;
                dataGridView1.Columns["Nombre Completo"].Width = 180;
                dataGridView1.Columns["Nombre de usuario"].Width = 140;
                dataGridView1.Columns["Vencimiento"].Width = 150;

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
        private void button2_Click(object sender, EventArgs e) {
            AddDelegado ad = new AddDelegado();
            ad.user_id = user_id;
            ad.usuario = usuario;
            ad.FormClosed += Ad_FormClosed;
            ad.ShowDialog();
        }
        private void Ad_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            try {
                EditDelegado ed = new EditDelegado();
                ed.FormClosed += Ed_FormClosed;
                ed.delegadoid = dataGridView1["ID", e.RowIndex].Value.ToString();
                ed.delegadoname = dataGridView1["Nombre Completo", e.RowIndex].Value.ToString();
                ed.ShowDialog();
            } catch (Exception) { }
        }
        private void Ed_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
    }
}
