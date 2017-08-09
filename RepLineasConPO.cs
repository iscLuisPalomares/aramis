using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RepLineasConPO : Form {
        public RepLineasConPO() {
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
                string sqlquery = "select * from materialrequerido matreq "
                    + "join tblPurchaseOrders pos on matreq.fspurchaseorder = pos.fsid "
                    + "where fspurchaseorder >= 1000 and pos.fscreatedate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' "
                    + "and pos.fscreatedate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "'";
                sqlquery = "select matreq.fsid as 'ID', pos.fsid as 'PO ID', matreq.fscantidad as 'Cantidad', "
                    + "matreq.fscodigo as 'Codigo', matreq.fsdesc as 'Descripcion', "
                    + "matreq.absolutdllscot as 'Costo Dlls', matreq.absdllscotuni as 'Costo Unidad' "
                    + "from materialrequerido matreq "
                    + "join tblPurchaseOrders pos on matreq.fspurchaseorder = pos.fsid "
                    + "where fspurchaseorder >= 1000 and pos.fscreatedate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' "
                    + "and pos.fscreatedate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Descripcion"].Width = 300;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            
        }
        private void button2_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            
        }
        private void getlineas(string vendorid) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid as 'ID', fsrequisicion as 'Requisicion', fscantidad as 'Cantidad', "
                    + "fsunimedida as 'U/M', fscodigo as 'Codigo', fsdesc as 'Descripcion', "
                    + "fspurchaseorder as 'Orden de Compra', saldo as 'Saldo', recibido as 'Recibido', "
                    + "comentario as 'Comentario', absolutdllscot as 'Costo Dlls' from materialrequerido where fspurchaseorder in ( "
                    + "select fsid from tblPurchaseOrders where fsstatus = 'PO Recibido' and fsid >= 1000 "
                    + "and(tblPurchaseOrders.fsdaterequired >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") +" 00:00:00.000' "
                    + "and tblPurchaseOrders.fsdaterequired <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59.999' "
                    + "and tblPurchaseOrders.fsvendor = " + vendorid + ")"
                    + ")";
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
    }
}
