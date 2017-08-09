using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RecotizarList : Form {
        public RecotizarList() {
            InitializeComponent();
        }
        public string usuario   { get; set; }
        public string tipo      { get; set; }
        public string depto     { get; set; }
        public string user_id   { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM tbcotizaciones WHERE fsid IN ("+
                    "SELECT fsidcotizacion FROM tbcotmaterialrequerido WHERE fsidmaterialrequerido IN ("+
                        "SELECT fsid FROM materialrequerido WHERE fsstatus = 'Cotizado')) AND fsganadora = '1'";


                sqlquery = "SELECT * FROM materialrequerido WHERE fsstatus = 'PO Cancelado'";
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
            
        }
        private void Ar_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }

        private void PendingRequi_Load(object sender, EventArgs e) {
            getdata();
        }
    }
}
