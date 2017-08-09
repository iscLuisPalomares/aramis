using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RepLineasRecibidas : Form {
        public RepLineasRecibidas() {
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
                string sqlquery = "select fsid as 'ID', fspurchaseorder as 'Numero PO', "
                    + "fscodigo as 'Codigo', fsdesc as 'Descripcion', fscantidad as 'Cantidad', "
                    + "absdllscotuni as 'Costo Unidad Dlls', absolutdllscot as 'Costo Dlls' "
                    + "from materialrequerido "
                    + "where fsid in ("
                    + "select fsidlinea from tblrecibos "
                    + "where fsdate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' "
                    + "and fsdate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59'"
                    + ")"
                    + "order by absolutdllscot desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Descripcion"].Width = 390;
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
