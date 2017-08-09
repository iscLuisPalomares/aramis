using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class PendingAjustesB : Form {
        public PendingAjustesB() {
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
                string sqlquery = "SELECT * FROM tbajustes WHERE fsstatus = 'Ajuste Creado' ORDER BY fsid";
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
            ApproveAjuste ar = new ApproveAjuste();
            ar.ajusteid = dataGridView1[0, e.RowIndex].Value.ToString();
            ar.account = dataGridView1[3, e.RowIndex].Value.ToString();
            ar.FormClosed += Ar_FormClosed;
            Visible = false;
            ar.usuario = usuario;
            ar.ShowDialog();
        }
        private void Ar_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
            getdata();
        }
        private void PendingRequi_Load(object sender, EventArgs e) {
            if (userhaspermit()) {
                getdata();
            } else {
                MessageBox.Show("No tienes autorizacion para aprobar ajustes");
                Close();
            }
        }
        private bool userhaspermit() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM tbdelegados WHERE fsuserid = " + user_id + " AND fsapproveajustes = 1 AND "
                    + "fsvencimiento > SYSDATETIME();";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                conn.Close();
                DataTable table = new DataTable();
                adapter.Fill(table);
                if (table.Rows.Count > 0) {
                    return true;
                } else {
                    return false;
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
