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
using System.Net.Mail;

namespace ComprasProject {
    public partial class MttoAsignarTrabajo : Form {
        public MttoAsignarTrabajo() {
            InitializeComponent();
        }
        public string user_id;
        public string usuario;

        private void MttoAsignarTrabajo_Load(object sender, EventArgs e) {
            getmttopendingreqs();
        }

        private void getmttopendingreqs() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select mtto.fsid as 'ID', convert(varchar, mtto.fsfecha, 105) as 'Fecha de requisición', users.fulname as 'Nombre Completo' "
                    + ", mtto.fsdescripcion as 'Descripción de Trabajo', mtto.fsasignadoa as 'Asignado a', mtto.fsfechaasignado as 'Fecha de asignación' "
                    + "from tbmttoreq mtto join users on users.id = mtto.fsidusuario "
                    + "where fsstatus in ('Requisicion Aprobada', 'Requisicion Asignada')";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
                dataGridView1.Columns["ID"].Width = 70;
                dataGridView1.Columns["Nombre Completo"].Width = 170;
                dataGridView1.Columns["Descripción de Trabajo"].Width = 450;
                dataGridView1.Columns["Asignado a"].Width = 120;
                dataGridView1.Columns["Fecha de asignación"].Width = 150;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            MttoSelectWorker select = new MttoSelectWorker();
            select.mttoreq = dataGridView1[0, e.RowIndex].Value.ToString();
            if (select.ShowDialog() == DialogResult.OK) {
                MessageBox.Show(select.selectedemployee);
                setmttoreqemployee(select.selectedemployee, dataGridView1[0,e.RowIndex].Value.ToString());
            }
        }

        private void setmttoreqemployee(string empleado, string linea) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION;\n"
                    + "UPDATE tbmttoreq SET fsstatus = 'Requisicion Asignada', fsasignadoa = @empleado, fsfechaasignado = GETDATE() where fsid = @mttoreq "
                    + "COMMIT TRANSACTION;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.Parameters.AddWithValue("@empleado", empleado);
                ejecucion.Parameters.AddWithValue("@mttoreq", linea);
                ejecucion.ExecuteNonQuery();
                conn.Close();

                string mail = getcorreo(linea);
                if (mail.Length > 0) {
                    sendmail(mail, "aprobado", linea);
                }
                MessageBox.Show("Requisicion de Trabajo, Asignado", "Listo");
                getmttopendingreqs();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void sendmail(string correo, string seaprobo_ono, string mttoreq) {
            MailMessage mail = new MailMessage("aramis@posey.com", correo);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            if (seaprobo_ono == "aprobado") {
                mail.Subject = "Requisicion de trabajo: " + mttoreq + " asignada.";
                mail.Body = "Se ha asignado una requisicion de trabajo:\n"
                    + "Ingrese al Sistema ARAMIS por favor para continuar.";
            }
            try {
                client.Send(mail);
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
        }

        private string getcorreo(string mttoreq) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "select usuario.correo "
                    + "from tbmttoreq join users usuario on usuario.id = tbmttoreq.fsidusuario "
                    + "where fsid = @mttoreq;";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@mttoreq", mttoreq);
                DataTable tabla = new DataTable();
                adapter.Fill(tabla);
                DataRow dr = tabla.Rows[0];
                conn.Close();
                return dr[0].ToString();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
                return "";
            }
        }
    }
}
