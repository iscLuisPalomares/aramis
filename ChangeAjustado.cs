using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ChangeAjustado : Form {
        public ChangeAjustado() {
            InitializeComponent();
        }
        public string id_bucket { get; set; }
        public string ajustado_actual { get; set; }
        public string ajustado { get; set; }
        public string gasto { get; set; }
        public string asignado { get; set; }
        public string balance { get; set; }

        public void setadjustment() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "update "
                    + "buckets set buckets.gasto = gastos.[Total cotizado dlls], buckets.asignado = 0 FROM buckets bucks "
                    + "INNER JOIN(SELECT bucketid, sum(fsabsolutodlls) AS 'Total estimado dlls' "
                    + ", sum(absolutdllscot) AS 'Total cotizado dlls', fsstatus FROM materialrequerido "
                    + "WHERE fsstatus = 'PO Recibido' GROUP BY fsstatus, bucketid) gastos "
                    + "ON bucks.id_bucket = gastos.bucketid update "
                    + "buckets set buckets.asignado = ("
                    + "CASE WHEN asignados.[Total cotizado] is null OR asignados.[Total cotizado] = 0 "
                    + "then asignados.[Total estimado dlls] "
                    + "else asignados.[Total cotizado] end) "
                    + "FROM buckets bucks INNER JOIN("
                    + "SELECT bucketid, sum(fsabsolutodlls) AS 'Total estimado dlls', sum(absolutdllscot) as 'Total cotizado' "
                    + "FROM materialrequerido WHERE fsstatus in ('Requisicion Creada', 'Requisicion Aprobada' "
                    + ", 'Cotizacion Creada', 'Cotizacion Aprobada', 'PO Creado', 'PO Recibiendo'"
                    + ", 'PO Aprobado', 'PO creado') GROUP BY bucketid "
                    + ") asignados ON bucks.id_bucket  = asignados.bucketid "
                    + "update buckets set balance = ajustado - gasto - asignado \n";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                updatebucket();
                MessageBox.Show("Actualizado", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public void updatebucket() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlcuentas = "SELECT * FROM buckets WHERE id_bucket = '" + id_bucket + "'";
                SqlDataAdapter adaptercuentas = new SqlDataAdapter(sqlcuentas, conn);
                DataTable cuentastable = new DataTable();
                adaptercuentas.Fill(cuentastable);
                //3-4-5 = 6
                double tres = double.Parse(cuentastable.Rows[0][3].ToString());
                double cuatro = double.Parse(cuentastable.Rows[0][4].ToString());
                double cinco = double.Parse(cuentastable.Rows[0][5].ToString());
                double seis = tres - cuatro - cinco;
                
                //asignar nuevos valores 
                string sqlquery = "UPDATE buckets SET ajustado = '" + 
                   tres.ToString() + "', gasto = '" + cuatro.ToString() + "', asignado = '" +
                   cinco.ToString() +"', balance = '" + seis.ToString() + "' WHERE id_bucket='" + id_bucket + "'";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                //MessageBox.Show("Actualizado", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public string usuario { get; set; }
        public string user_id { get; set; }

        private void CreateUser_Load(object sender, EventArgs e) {
            textBox1.Text = ajustado_actual;
        }

        private void button1_Click(object sender, EventArgs e) {
            setadjustment();
        }

        private void CreateUser_SizeChanged(object sender, EventArgs e) {

        }
        private void textBox4_TextChanged(object sender, EventArgs e) {

        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
