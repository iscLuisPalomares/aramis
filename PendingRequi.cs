using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class PendingRequi : Form {
        public PendingRequi() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string depto { get; set; }
        public string user_id { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = ""
                    + "SELECT id_req AS 'ID', "
                    + "creador.fulname AS 'Creada por', "
                    + "cuenta.acctnumber AS 'Cuenta', "
                    + "cuenta.acctdesc AS 'Descripcion', "
                    + "depa.name AS 'Departamento', "
                    + "urgente AS 'Urgente', "
                    + "donde AS 'Lugar' "
                    + "FROM requisiciones "
                    + "JOIN users creador ON requisiciones.createdby = creador.username "
                    + "JOIN Accounts cuenta ON requisiciones.account = cuenta.acctnumber "
                    + "JOIN Deptos depa ON requisiciones.deptoid = depa.id "
                    + "JOIN users manager ON manager.id = depa.gerente "
                    + "WHERE fsstatus = 'Requisicion Creada' AND manager.id = " + user_id + ";";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            Close();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            try {
                ApproveRequi ar = new ApproveRequi();
                ar.idreq = dataGridView1["ID", e.RowIndex].Value.ToString();
                ar.account = dataGridView1["Cuenta", e.RowIndex].Value.ToString();
                ar.FormClosed += Ar_FormClosed;
                Visible = false;
                ar.usuario = usuario;
                ar.ShowDialog();
            } catch (Exception) { }
        }
        private void Ar_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
            getdata();
        }
        private void PendingRequi_Load(object sender, EventArgs e) {
            getdata();
        }
    }
}
