using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ComprasProject {
    public partial class MttoMyReqs : Form {
        public MttoMyReqs() {
            InitializeComponent();
        }
        public string user_id;
        public string usuario;

        private void MttoApprovedReqs_Load(object sender, EventArgs e) {
            getapprovedmttoreqs();
        }

        private void getapprovedmttoreqs() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select mtto.fsid as 'ID', convert(varchar, mtto.fsfecha, 105) as 'Fecha de requisición', users.fulname as 'Nombre Completo' "
                    + ", mtto.fsdescripcion as 'Descripción de Trabajo', mtto.fsstatus as 'Status' "
                    + "from tbmttoreq mtto join users on users.id = mtto.fsidusuario "
                    + "where fsidusuario = @usuario";
                string querypararecibir = "select mtto.fsid as 'ID', convert(varchar, mtto.fsfecha, 105) as 'Fecha de requisición', users.fulname as 'Nombre Completo' "
                    + ", mtto.fsdescripcion as 'Descripción de Trabajo', mtto.fsstatus as 'Status' "
                    + "from tbmttoreq mtto join users on users.id = mtto.fsidusuario "
                    + "where fsidusuario = @usuario and fsstatus = 'Requisicion Asignada'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapterrecibir = new SqlDataAdapter(querypararecibir, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@usuario", user_id);
                adapterrecibir.SelectCommand.Parameters.AddWithValue("@usuario", user_id);
                DataTable table = new DataTable();
                DataTable tablarecibir = new DataTable();
                adapter.Fill(table);
                adapterrecibir.Fill(tablarecibir);
                dataGridView1.DataSource = table;
                dataGridView2.DataSource = tablarecibir;
                conn.Close();
                dataGridView1.Columns["Nombre Completo"].Width = 200;
                dataGridView1.Columns["Descripción de Trabajo"].Width = 450;
                dataGridView1.Columns["Status"].Width = 250;
                dataGridView2.Columns["Nombre Completo"].Width = 200;
                dataGridView2.Columns["Descripción de Trabajo"].Width = 450;
                dataGridView2.Columns["Status"].Width = 250;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }

        private void dataGridView2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            MttoRecibirTrabajo recibir = new MttoRecibirTrabajo();
            recibir.usuario = usuario;
            recibir.user_id = user_id;
            if (recibir.ShowDialog() == DialogResult.OK) {
                mttorecibirlinea(dataGridView2[0, e.RowIndex].Value.ToString());
            }
        }
        private void mttorecibirlinea(string linea) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION \n"
                + "UPDATE tbmttoreq SET fsfechaentregado = GETDATE(), fsstatus = @recibidopor WHERE fsid = @linea;\n"
                + "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.Parameters.AddWithValue("@linea", linea);
                ejecucion.Parameters.AddWithValue("@recibidopor", "Recibido por " + usuario);
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Requisicion Recibida", "Listo");
                getapprovedmttoreqs();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
