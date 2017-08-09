using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class StatusCot : Form {
        public StatusCot() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string userid { get; set; }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) {
                string idcot = dataGridView1[0, e.RowIndex].Value.ToString();
                ChangeCotStatus ccs = new ChangeCotStatus();
                ccs.idcot = idcot;
                ccs.ShowDialog();
            }
        }

        private void StatusCot_Load(object sender, EventArgs e) {
            getcotizaciones();
        }
        private void getcotizaciones() {
            SqlConnection conn = new SqlConnection(Program.stringconnection);
            conn.Open();
            try {
                string sqlquery = "select top 200 * from tbcotizaciones where fsganadora = 1 order by fsid desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            } catch (Exception) {

            }
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            getcotizaciones();
        }
    }
}
