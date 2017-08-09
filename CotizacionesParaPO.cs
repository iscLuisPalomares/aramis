using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CotizacionesParaPO : Form {
        public CotizacionesParaPO() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_id { get; set; }
        public string user_depto { get; set; }
        public string deptoid { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid as 'ID', fscostototal as 'Costo Total', "
                    + "proveedor.suppname as 'Proveedor', fssupplier as 'Supplier ID', fsimpuestos as 'Impuestos' "
                    + "from tbcotizaciones join asl proveedor on proveedor.id = tbcotizaciones.fssupplier "
                    + "where fsstatus = 'Cotizacion Aprobada'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata();
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            try {
                CreateAllPOrder cpo = new CreateAllPOrder();
                cpo.usuario = usuario;
                cpo.user_id = user_id;
                cpo.cotizacion = dataGridView1["ID", e.RowIndex].Value.ToString();
                cpo.vendorname = dataGridView1["Supplier ID", e.RowIndex].Value.ToString();
                cpo.totalcost = dataGridView1["Costo Total", e.RowIndex].Value.ToString();
                cpo.impuestos = dataGridView1["Impuestos", e.RowIndex].Value.ToString();
                cpo.ShowInTaskbar = false;
                cpo.FormClosed += Cpo_FormClosed;
                cpo.ShowDialog();
            } catch (Exception) { }
        }
        private void Cpo_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
        private void pictureBox2_Click(object sender, EventArgs e) {
            getdata();
        }
    }
}
