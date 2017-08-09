using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RepGastoDepto : Form {
        public RepGastoDepto() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string user_id { get; set; }
        public string tipo { get; set; }
        public string user_depto { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select depto.id as 'ID', depto.name as 'Departamento', "
                    + "round(sum(matreq.absolutdllscot), 2) as 'Costo Dlls' "
                    + "from materialrequerido matreq "
                    + "join requisiciones req on req.id_req = matreq.fsrequisicion "
                    + "join deptos depto on depto.id = req.deptoid "
                    + "where fsid in ( "
                    + "select fsidlinea "
                    + "from tblrecibos "
                    + "where fsdate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and "
                    + "fsdate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59' "
                    + ") "
                    + "group by depto.id, depto.name "
                    + "order by round(sum(matreq.absolutdllscot), 2) desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Departamento"].Width = 200;
                tabControl1.SelectedIndex = 0;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            string depto = dataGridView1["ID", e.RowIndex].Value.ToString();
            getlineas(depto);
        }
        private void getlineas(string depid) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select matreq.fsid as 'ID', depto.name as 'Departamento', "
                    + "matreq.fscodigo as 'Codigo', matreq.fsdesc as 'Descripcion', "
                    + "matreq.fscantidad as 'Cantidad', matreq.absolutdllscot as 'Costo Dlls' "
                    + "from materialrequerido matreq join requisiciones req on req.id_req = matreq.fsrequisicion "
                    + "join deptos depto on depto.id = req.deptoid where fsid in ("
                    + "select fsidlinea from tblrecibos "
                    + "where fsdate >= '2016-12-01 00:00:00' "
                    + "and fsdate <= '2017-01-04 23:59:59' ) "
                    + "and depto.id = " + depid + " order by matreq.absolutdllscot desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView2.DataSource = table;
                conn.Close();
                tabControl1.SelectedIndex = 1;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void RepGastoDepto_Load(object sender, EventArgs e) {

        }
    }
}
