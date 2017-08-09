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
    public partial class EliminarLinea : Form {
        public EliminarLinea() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string user_id { get; set; }
        public string descripcion { get; set; }
        public string lineaid { get; set; }
        public string costo { get; set; }
        public string bucket { get; set; }

        public void removeline() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on\n";
                sqlquery += "BEGIN TRANSACTION\n";
                sqlquery += "UPDATE materialrequerido SET fsstatus = 'Eliminado de PO' WHERE fsid = " + lineaid + ";\n";
                sqlquery += "update "
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
                sqlquery += "COMMIT TRANSACTION;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                //MessageBox.Show(sqlquery);
                conn.Close();
                MessageBox.Show("Actualizado", "Listo");
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateUser_Load(object sender, EventArgs e) {
            label2.Text = descripcion;
        }
        private void button1_Click(object sender, EventArgs e) {
            removeline();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
