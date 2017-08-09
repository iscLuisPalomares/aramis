using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ImprimirListaPOs : Form {
        public ImprimirListaPOs() {
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
                string sqlquery = "select tpos.fsid as 'ID', tcot.fsapprovedate as 'Fecha Aprobacion' "
                    + ", approvers.fulname as 'Aprobado por', fstotalcost as 'Costo Total' "
                    + ", tpos.fsimpuestos as 'Impuestos', comprador.fulname as 'Comprador' "
                    + ", fsbuyer as 'Comprador ID', fscomments as 'Comentarios' "
                    + ", fsvendor as 'Proveedor', fscotizacionid as 'Cotizacion ID' "
                    + ", fsterms as 'Terminos', fsdaterequired as 'Fecha Requerida' "
                    + ", fsobservaciones as 'Observaciones' "
                    + "from tblPurchaseOrders tpos "
                    + "join users comprador on comprador.id = fsbuyer "
                    + "join tbcotizaciones tcot on tcot.fsid = fscotizacionid "
                    + "join users approvers on approvers.username = tcot.fsapprovedby "
                    + "where tpos.fsstatus = 'PO Aprobado' or tpos.fsstatus = 'PO Recibiendo' or tpos.fsstatus = 'PO Recibido'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = table;
                foreach (DataGridViewColumn dc in dataGridView1.Columns) {
                    if (dc.Name == "ID") { dc.Width = 70; }
                    if (dc.Name == "Fecha Requerida") { dc.Width = 170; }
                }
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
                ImprimirPO ap = new ImprimirPO();
                ap.idpo = dataGridView1["ID", e.RowIndex].Value.ToString();
                ap.subtotal = dataGridView1["Costo Total", e.RowIndex].Value.ToString();
                ap.impuestos = dataGridView1["Impuestos", e.RowIndex].Value.ToString();
                ap.compradorid = dataGridView1["Comprador ID", e.RowIndex].Value.ToString();
                ap.comentario = dataGridView1["Comentarios", e.RowIndex].Value.ToString();
                ap.proveedorid = dataGridView1["Proveedor", e.RowIndex].Value.ToString();
                ap.cotizacionid = dataGridView1["Cotizacion ID", e.RowIndex].Value.ToString();
                ap.terminos = dataGridView1["Terminos", e.RowIndex].Value.ToString();
                ap.fecharequerida = dataGridView1["Fecha Requerida", e.RowIndex].Value.ToString();
                ap.observaciones = dataGridView1["Observaciones", e.RowIndex].Value.ToString();
                ap.usuario = usuario;
                ap.user_id = user_id;
                ap.ShowInTaskbar = false;
                ap.ShowDialog();
            } catch (Exception) { }
        }
        private void Ar_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
        private void PendingRequi_Load(object sender, EventArgs e) {
            getdata();
        }

        private void button2_Click(object sender, EventArgs e) {
            searchbypo();
        }

        private void searchbypo() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select tpos.fsid as 'ID', tcot.fsapprovedate as 'Fecha Aprobacion' "
                    + ", approvers.fulname as 'Aprobado por', fstotalcost as 'Costo Total' "
                    + ", tpos.fsimpuestos as 'Impuestos', comprador.fulname as 'Comprador' "
                    + ", fsbuyer as 'Comprador ID', fscomments as 'Comentarios' "
                    + ", fsvendor as 'Proveedor', fscotizacionid as 'Cotizacion ID' "
                    + ", fsterms as 'Terminos', fsdaterequired as 'Fecha Requerida' "
                    + ", fsobservaciones as 'Observaciones' "
                    + "from tblPurchaseOrders tpos "
                    + "join users comprador on comprador.id = fsbuyer "
                    + "join tbcotizaciones tcot on tcot.fsid = fscotizacionid "
                    + "join users approvers on approvers.username = tcot.fsapprovedby "
                    + "where tpos.fsid = @idpo";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@idpo", textBox1.Text);
                DataTable table = new DataTable();
                adapter.Fill(table);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = table;
                foreach (DataGridViewColumn dc in dataGridView1.Columns) {
                    if (dc.Name == "ID") { dc.Width = 70; }
                    if (dc.Name == "Fecha Requerida") { dc.Width = 170; }
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
