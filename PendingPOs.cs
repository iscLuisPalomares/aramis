using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class PendingPOs : Form {
        public PendingPOs() {
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
                string sqlquery = "SELECT * FROM tblPurchaseOrders WHERE fsstatus = 'PO Creado'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
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
            ApprovePO ap = new ApprovePO();
            ap.idpo = dataGridView1[0, e.RowIndex].Value.ToString();
            ap.usuario = usuario;
            ap.user_id = user_id;
            ap.ShowInTaskbar = false;
            ap.ShowDialog();
        }
        private void Ar_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
        private void PendingRequi_Load(object sender, EventArgs e) {
            getdata();
        }
    }
}
