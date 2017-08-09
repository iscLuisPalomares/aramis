using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ChangeCotStatus : Form {
        public ChangeCotStatus() {
            InitializeComponent();
        }
        public string idcot { get; set; }
        private void button3_Click(object sender, EventArgs e) {
            Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            cancelcotizacion();
        }

        private void cancelcotizacion() {
            SqlConnection conn = new SqlConnection(Program.stringconnection);
            conn.Open();
            try {
                string sqlquery = "update tbcotizaciones set fsganadora = 0, fsstatus = 'Cotizacion Cancelada' "
                    + "where fsid = @idcot \n"
                    + "update materialrequerido set fsstatus = 'Cotizacion Cancelada' where fsid in ("
                    + "select fsidmaterialrequerido from tbcotmaterialrequerido where fsidcotizacion = @idcot )";
                SqlCommand execution = new SqlCommand(sqlquery, conn);
                execution.Parameters.AddWithValue("@idcot", idcot);
                execution.ExecuteNonQuery();
                MessageBox.Show("Listo");
            } catch (Exception) {
            }
            conn.Close();
        }
        
        private void button2_Click(object sender, EventArgs e) {
            returncotizacion();
        }

        private void returncotizacion() {
            SqlConnection conn = new SqlConnection(Program.stringconnection);
            conn.Open();
            try {
                string sqlquery = "update tbcotizaciones set fsganadora = 1, fsstatus = 'Cotizacion Creada' "
                    + "where fsid = @idcot \n"
                    + "update materialrequerido set fsstatus = 'Cotizado' where fsid in ("
                    + "select fsidmaterialrequerido from tbcotmaterialrequerido where fsidcotizacion = @idcot )";
                SqlCommand execution = new SqlCommand(sqlquery, conn);
                execution.Parameters.AddWithValue("@idcot", idcot);
                execution.ExecuteNonQuery();
                MessageBox.Show("Listo");
            } catch (Exception) {
            }
            conn.Close();
        }

        private void button4_Click(object sender, EventArgs e) {
            returntorequote();
        }
        private void returntorequote() {
            SqlConnection conn = new SqlConnection(Program.stringconnection);
            conn.Open();
            try {
                string sqlquery = "update tbcotizaciones set fsganadora = 0, fsstatus = 'Cotizacion No Ganadora' "
                    + "where fsid = @idcot \n"
                    + "update materialrequerido set fsstatus = 'Requisicion Aprobada' where fsid in ("
                    + "select fsidmaterialrequerido from tbcotmaterialrequerido where fsidcotizacion = @idcot )";
                SqlCommand execution = new SqlCommand(sqlquery, conn);
                execution.Parameters.AddWithValue("@idcot", idcot);
                execution.ExecuteNonQuery();
                MessageBox.Show("Listo");
            } catch (Exception) {
            }
            conn.Close();
        }
    }
}
