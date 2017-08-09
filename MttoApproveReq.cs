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
    public partial class MttoApproveReq : Form {
        public MttoApproveReq() {
            InitializeComponent();
        }
        public string usuario = "";
        public string user_id = "";
        public string mttoreq = "";
        string archivo1 = "";
        string archivo2 = "";
        private void ApproveReqMtto_Load(object sender, EventArgs e) {
            getreqdata();
        }

        private void getreqdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select mtto.fsid as 'ID', convert(varchar, mtto.fsfecha, 105) as 'Fecha de requisición', users.fulname as 'Nombre Completo' "
                    + ", mtto.fsdescripcion as 'Descripción de Trabajo', mtto.fsfile1 as 'File 1', mtto.fsfile2 as 'File 2' "
                    + "from tbmttoreq mtto join users on users.id = mtto.fsidusuario "
                    + "where fsid = @id";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@id", mttoreq);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                textBox1.Text = table.Rows[0][2].ToString();
                textBox2.Text = table.Rows[0][3].ToString();
                if (table.Rows[0][4].ToString().Length > 0) {
                    archivo1 = table.Rows[0][4].ToString();
                } else { button1.Visible = false; }
                if (table.Rows[0][5].ToString().Length > 0) {
                    archivo2 = table.Rows[0][5].ToString();
                } else { button2.Visible = false; }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FileName = archivo1;
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    System.IO.File.Copy(@"\\mexfs01\TJTemp\Opardo\FOLIOS\mttoreq\" + mttoreq + @"\" + archivo1,
                    saveFileDialog1.FileName, true);
                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                }
            } catch (Exception) {
                MessageBox.Show("Error al abrir archivo");
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            try {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FileName = archivo2;
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    System.IO.File.Copy(@"\\mexfs01\TJTemp\Opardo\FOLIOS\mttoreq\" + mttoreq + @"\" + archivo2,
                    saveFileDialog1.FileName, true);
                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                }
            } catch (Exception) {
                MessageBox.Show("Error al abrir archivo");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            approvemttoreq();
        }

        private void approvemttoreq() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION;\n"
                    + "UPDATE tbmttoreq SET fsstatus = 'Requisicion Aprobada', fsaprobadopor = @aprobadopor, fsfechaaprobado = GETDATE() where fsid = @mttoreq "
                    + "COMMIT TRANSACTION;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.Parameters.AddWithValue("@aprobadopor", user_id);
                ejecucion.Parameters.AddWithValue("@mttoreq", mttoreq);
                ejecucion.ExecuteNonQuery();
                conn.Close();
                
                string mail = getcorreo();
                if (mail.Length > 0) {
                    sendmail(mail, "aprobado");
                }
                MessageBox.Show("Requisicion Aprobada", "Listo");
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void sendmail(string correo, string seaprobo_ono) {
            MailMessage mail = new MailMessage("aramis@posey.com", correo);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            if (seaprobo_ono == "aprobado") {
                mail.Subject = "Requisicion de trabajo: " + mttoreq + " aprobada.";
                mail.Body = "Se ha aprobado una requisicion de trabajo:\n"
                    + textBox2.Text + "\n \n"
                    + "Ingrese al Sistema ARAMIS por favor para continuar.";
            }
            if (seaprobo_ono == "desaprobado") {
                mail.Subject = "Requisicion de trabajo: " + mttoreq + " desaprobada.";
                mail.Body = "Se ha desaprobado una requisicion de trabajo:\n"
                    + textBox2.Text + "\n \n"
                    + "Ingrese al Sistema ARAMIS por favor para continuar.";
            }
            try {
                client.Send(mail);
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
        }

        private string getcorreo() {
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

        private void pictureBox2_Click(object sender, EventArgs e) {
            disapprovemttoreq();
        }

        private void disapprovemttoreq() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION;\n"
                    + "UPDATE tbmttoreq SET fsstatus = 'Requisicion Desaprobada', fsdesaprobadopor = @desaprobadopor, fsfechadesaprobado = GETDATE() where fsid = @mttoreq "
                    + "COMMIT TRANSACTION;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.Parameters.AddWithValue("@desaprobadopor", user_id);
                ejecucion.Parameters.AddWithValue("@mttoreq", mttoreq);
                ejecucion.ExecuteNonQuery();
                conn.Close();
                string mail = getcorreo();
                if (mail.Length > 0) {
                    sendmail(mail, "desaprobado");
                }
                MessageBox.Show("Requisicioin Desaprobada", "Listo");
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
