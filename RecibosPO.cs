using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RecibosPO : Form {
        public RecibosPO() {
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
                    + "FROM materialrequerido WHERE fspurchaseorder = '" + idpo + "' AND (fsstatus = 'PO Creado' OR fsstatus = 'PO Recibiendo')";
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
                string sqlquery = "SELECT fsid as 'ID', fscreatedate as 'Fecha de Creación', fscomments as 'Comentarios', fsdepto as 'Departamento', "
                    + "fstotalcost as 'Costo Total' FROM tblPurchaseOrders WHERE fsstatus = 'PO Aprobado' OR fsstatus = 'PO Recibiendo'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = table;
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
        private void dataGridView2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            
            RecibosAddRecibo rr = new RecibosAddRecibo();
            rr.user_id = user_id;
            rr.usuario = usuario;
            rr.idlinea = dataGridView2["ID", e.RowIndex].Value.ToString();
            rr.saldo = dataGridView2["Saldo", e.RowIndex].Value.ToString();
            rr.recibido = dataGridView2["Recibido", e.RowIndex].Value.ToString();
            rr.qty = dataGridView2["Cantidad", e.RowIndex].Value.ToString();
            rr.cuenta = dataGridView2["Cuenta", e.RowIndex].Value.ToString();
            rr.idpo = dataGridView2["PO ID", e.RowIndex].Value.ToString();
            rr.costounidad = dataGridView2["Costo Dlls Unidad", e.RowIndex].Value.ToString();
            rr.requisicionid = dataGridView2["Requisicion", e.RowIndex].Value.ToString();
            rr.ShowInTaskbar = false;
            rr.FormClosed += Rr_FormClosed;
            rr.ShowDialog();
        }
        private void Rr_FormClosed(object sender, FormClosedEventArgs e) {
            getlineas();
            updatewithsaldo();
        }
        private void updatewithsaldo() {
            try {
                double saldo = 0;
                foreach (DataGridViewRow dr in dataGridView2.Rows) {
                    try {
                        saldo += double.Parse(dr.Cells["Saldo"].Value.ToString());
                    } catch (Exception) {
                        return;
                    }
                }
                if (saldo == 0) {
                    string connectionstring = Program.stringconnection;
                    SqlConnection conn = new SqlConnection(connectionstring);
                    conn.Open();
                    string sqlquery = "UPDATE tblPurchaseOrders SET fsstatus = 'PO Recibido' WHERE fsid = " + idpo + ";";
                    tabControl1.SelectedIndex = 0;
                    SqlCommand ejecucion = new SqlCommand();
                    ejecucion.Connection = conn;
                    ejecucion.CommandType = CommandType.Text;
                    ejecucion.CommandText = sqlquery;
                    ejecucion.ExecuteNonQuery();
                    conn.Close();
                    getdata();
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
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
