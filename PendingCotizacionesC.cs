using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class PendingCotizacionesC : Form {
        public PendingCotizacionesC() {
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
                string sqlquery = "SELECT cotz.fsid as 'ID', cotz.createdate as 'Fecha'"
                    + ", cotz.fscomentario as 'Comentario', "
                    + "cotz.fscostototal as 'Costo Total', cotz.fsimpuestos as 'Impuesto %' "
                    + ", cotz.fsdivisa as 'Moneda' , asl.suppname as 'Proveedor' "
                    + "FROM tbcotizaciones cotz join asl on asl.id = cotz.fssupplier "
                    + "WHERE fsstatus = 'Cotizacion Creada'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
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
                CotizacionEdit ar = new CotizacionEdit();
                ar.idcot = dataGridView1[0, e.RowIndex].Value.ToString();
                ar.FormClosed += Ar_FormClosed;
                ar.costototal = double.Parse(dataGridView1[3, e.RowIndex].Value.ToString());
                ar.ShowInTaskbar = false;
                ar.usuario = usuario;
                ar.ShowDialog();
            } catch (Exception) { }
        }
        private void Ar_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }

        private void PendingRequi_Load(object sender, EventArgs e) {
            getdata();
        }
    }
}
