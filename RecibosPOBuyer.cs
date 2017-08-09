using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RecibosPOBuyer : Form {
        public RecibosPOBuyer() {
            InitializeComponent();
        }

        public string usuario   { get; set; }
        public string user_id   { get; set; }
        public string idpo      { get; set; }

        private void getlineas() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT fsid as 'ID', fsrequisicion as 'Requisicion', fscodigo as 'Codigo', "
                    + "fsdesc as 'Descripcion', fscantidad as 'Cantidad', fstotalcost as 'Costo Total', "
                    + "fscostounitario as 'Costo Unitario', saldo as 'Saldo', recibido as 'Recibido', "
                    + "bucketid as 'Bucket', fscuenta as 'Cuenta', fspurchaseorder as 'PO ID', absdllscotuni as 'Costo Dlls Unidad' "
                    + "FROM materialrequerido WHERE fspurchaseorder = '" + idpo + "'";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                dataGridView2.DataSource = tb;

                dataGridView2.Columns["ID"].Width               = 70;
                dataGridView2.Columns["Requisicion"].Width      = 70;
                dataGridView2.Columns["Cantidad"].Width         = 70;
                dataGridView2.Columns["Costo Total"].Width      = 70;
                dataGridView2.Columns["Costo Unitario"].Width   = 70;
                dataGridView2.Columns["Saldo"].Width            = 70;
                dataGridView2.Columns["Recibido"].Width         = 70;
                dataGridView2.Columns["Descripcion"].Width      = 180;
                dataGridView2.Columns["Bucket"].Visible         = false;
                dataGridView2.Columns["Cuenta"].Visible         = false;
                dataGridView2.Columns["PO ID"].Visible          = false;
                
                tabControl1.SelectedIndex = 1;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT fsid as 'ID', asl.suppname as 'Proveedor', fsdaterequired as 'Fecha Requerida', fscomments as 'Comentarios', fsdepto as 'Departamento', "
                    + "fstotalcost as 'Costo Total', fscreatedate as 'Fecha de Creación' FROM tblPurchaseOrders JOIN asl ON tblPurchaseOrders.fsvendor = asl.id WHERE fsstatus = 'PO Aprobado' OR fsstatus = 'PO Recibiendo'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Proveedor"].Width = 200;
                dataGridView1.Columns["Fecha Requerida"].Width = 150;
                dataGridView1.Columns["Fecha de Creación"].Width = 150;
                dataGridView1.Columns["Comentarios"].Width = 350;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata();
            Width = 1000;
            Height = 500;
            CenterToScreen();
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            try {
                idpo = dataGridView1["ID", e.RowIndex].Value.ToString();
                getlineas();
            } catch (Exception) { }
        }
        private void button4_Click(object sender, EventArgs e) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT fsid as 'ID', fscomments as 'Comentarios', fsdepto as 'Departamento', "
                    + "fstotalcost as 'Costo Total' FROM tblPurchaseOrders WHERE fsstatus = 'PO Aprobado' AND fsid='" +
                    textBox1.Text + "'";
                tabControl1.SelectedIndex = 0;
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
    }
}
