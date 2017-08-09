using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RepDelegados : Form {
        public RepDelegados() {
            InitializeComponent();
        }

        public string usuario       { get; set; }
        public string user_id       { get; set; }
        public string tipo          { get; set; }
        public string user_depto    { get; set; }
        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select pos.fsid as 'ID',  usuario.fulname as 'Aprobado por', "
                    + "cotz.fsapprovedate as 'Fecha aprobada', round(pos.fstotalcost, 2) as 'Costo', "
                    + "pos.fsimpuestos as 'Impuestos %', "
                    + "round(((pos.fsimpuestos / 100) + 1) * pos.fstotalcost, 2) as 'Total', "
                    + "cotz.fsdivisa as 'Moneda' from tblPurchaseOrders pos "
                    + "join tbcotizaciones cotz on cotz.fsid = pos.fscotizacionid "
                    + "join users usuario on usuario.username = cotz.fsapprovedby "
                    + "where fscotizacionid in ( "
                    + "select fsid from tbcotizaciones where fsapprovedby = ("
                    + "select username from users where id = " + (comboBox1.SelectedItem as ComboboxItem).Value + ") "
                    + "and (fsapprovedate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00.000' "
                    + "and fsapprovedate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59.999') "
                    + ") and pos.fsid >= 1000";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public void getdatatodos() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select pos.fsid as 'ID',  usuario.fulname as 'Aprobado por', "
                    + "cotz.fsapprovedate as 'Fecha aprobada', round(pos.fstotalcost, 2) as 'Costo', "
                    + "pos.fsimpuestos as 'Impuestos %', "
                    + "round(((pos.fsimpuestos / 100) + 1) * pos.fstotalcost, 2) as 'Total', "
                    + "cotz.fsdivisa as 'Moneda' "
                    + "from tblPurchaseOrders pos "
                    + "join tbcotizaciones cotz on cotz.fsid = pos.fscotizacionid "
                    + "join users usuario on usuario.username = cotz.fsapprovedby "
                    + "where fscotizacionid in ( "
                    + "select fsid from tbcotizaciones where fsapprovedby in ("
                    + "select username from users where id <> 61) "
                    + "and(fsapprovedate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00.000' "
                    + "and fsapprovedate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59.999' "
                    + ")) and pos.fsid >= 1000";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public void getlineas(string poid) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select * from materialrequerido where fspurchaseorder = " + poid + "";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView2.DataSource = table;
                tabControl1.SelectedIndex = 1;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void getapprovers() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select id as 'ID', fulname as 'Nombre' from users where username in ("
                    + "select distinct(fsapprovedby) "
                    + "from tbcotizaciones where fsstatus <> 'Cotizacion Desaprobada' "
                    + "and fsapprovedby is not null "
                    + "and fsapprovedby <> 'almartinez' "
                    + "and fsapprovedby <> 'amartinez')";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                comboBox1.Items.Clear();
                comboBox1.Items.Add("Todos");
                foreach (DataRow dr in table.Rows) {
                    ComboboxItem cbitem = new ComboboxItem();
                    cbitem.Text = dr["Nombre"].ToString();
                    cbitem.Value = dr["ID"].ToString();
                    comboBox1.Items.Add(cbitem);
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getapprovers();
        }
        private void Cu_FormClosing(object sender, FormClosingEventArgs e) {
            label1.Text = label1.Text;
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            if (comboBox1.SelectedItem.ToString() == "Todos") {
                getdatatodos();
            } else {
                getdata();
            }
        }
        private void editarRegistroToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("editar informacion del registro seleccionado");
        }
        private void Rm_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            try {
                string idpo = dataGridView1["ID", e.RowIndex].Value.ToString();
                getlineas(idpo);
            } catch (Exception) { }
            
        }
    }
}
