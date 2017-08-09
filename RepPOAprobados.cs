using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RepPOAprobados : Form {
        public RepPOAprobados() {
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
                string sqlquery = "select fsid as 'ID', tbcotizaciones.createdate as 'Fecha de Creacion', usuario.fulname as 'Creador', "
                    + "fsstatus as 'Status', fsapprovedate as 'Fecha Aprobacion', fscostototal as 'Sub Total', "
                    + "fsimpuestos as 'Impuestos %', round(((fsimpuestos / 100) + 1) * fscostototal, 2) as 'Costo Total', "
                    + "fsdivisa as 'Divisa', fsisuniquevendor as 'Es Proveedor Unico (Comodato)' "
                    + "from tbcotizaciones "
                    + "join Users usuario on tbcotizaciones.createdby = usuario.id "
                    + "where ("
                    + "fsapprovedate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00.000' "
                    + "and fsapprovedate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59.999' "
                    + ") "
                    + "and fsstatus = 'PO Aprobado' "
                    + "order by fscostototal desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Status"].Visible = false;
                sqlquery = "select pos.fsid as 'ID', usuarios.fulname as 'Creador', "
                    + "vendor.suppname as 'Vendor', pos.fsterms as 'Terminos', "
                    + "pos.fsdaterequired as 'Fecha requerida', pos.fsobservaciones as 'Observaciones', "
                    + "pos.fsstatus as 'Status', pos.fsdepto as 'Departamento', "
                    + "pos.fstotalcost as 'Subtotal', pos.fsimpuestos as 'Impuestos %', "
                    + "round(fstotalcost * ((fsimpuestos / 100) + 1), 2) as 'Costo Total' "
                    + "from tblPurchaseOrders pos join users usuarios on usuarios.id = pos.fscreatedby "
                    + "join asl vendor on vendor.id = pos.fsvendor "
                    + "where fsid >= 1000 and ("
                    + "fscreatedate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00.000' "
                    + "and fscreatedate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59.999'"
                    + ") order by pos.fstotalcost desc";
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery, conn);
                DataTable table2 = new DataTable();
                adapter2.Fill(table2);
                dataGridView2.DataSource = table2;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Cu_FormClosing(object sender, FormClosingEventArgs e) {
            label1.Text = label1.Text;
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void editarRegistroToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("editar informacion del registro seleccionado");
        }
        private void button2_Click(object sender, EventArgs e) {
            getdata();
        }
    }
}
