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
using System.IO;

namespace ComprasProject {
    public partial class MttoRequisicion : Form {
        public MttoRequisicion() {
            InitializeComponent();
        }
        public string user_id = "";
        public string usuario = "";

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            setnuevamttoreq();
        }

        private void setnuevamttoreq() {
            string connectionstring = Program.stringconnection;
            SqlConnection conn = new SqlConnection(connectionstring);
            conn.Open();
            string sqlquery = "set xact_abort on \n BEGIN TRANSACTION \n"
            + "DECLARE @mttoreqid INTEGER; \n"
            + "INSERT INTO tbmttoreq (fsidusuario, fsfecha, fsstatus, fsfile1, fsfile2, fsdescripcion) VALUES (@usuario, GETDATE(), @status, @file1, @file2, @desc); \n"
            + "SELECT @mttoreqid = SCOPE_IDENTITY(); \n"
            + "SELECT @mttoreqid; \n"
            + "COMMIT TRANSACTION;";
            SqlCommand ejecucion = new SqlCommand();
            ejecucion.Connection = conn;
            ejecucion.CommandType = CommandType.Text;
            ejecucion.Parameters.AddWithValue("@usuario", user_id);
            ejecucion.Parameters.AddWithValue("@status", "Requisicion Creada");
            ejecucion.Parameters.AddWithValue("@file1", System.IO.Path.GetFileName(textBox3.Text));
            ejecucion.Parameters.AddWithValue("@file2", System.IO.Path.GetFileName(textBox4.Text));
            ejecucion.Parameters.AddWithValue("@desc", textBox2.Text);
            ejecucion.CommandText = sqlquery;
            string id = ejecucion.ExecuteScalar().ToString();
            if (!System.IO.Directory.Exists(@"\\mexfs01\TJTemp\Opardo\FOLIOS\mttoreq\" + id.ToString())) {
                System.IO.Directory.CreateDirectory(@"\\mexfs01\TJTemp\Opardo\FOLIOS\mttoreq\" + id.ToString());
            }
            if (textBox3.Text.Length > 0) {
                System.IO.File.Copy(textBox3.Text, @"\\mexfs01\TJTemp\Opardo\FOLIOS\mttoreq\" + id.ToString() + @"\" + System.IO.Path.GetFileName(textBox3.Text), true);
            }
            if (textBox4.Text.Length > 0) {
                System.IO.File.Copy(textBox4.Text, @"\\mexfs01\TJTemp\Opardo\FOLIOS\mttoreq\" + id.ToString() + @"\" + System.IO.Path.GetFileName(textBox4.Text), true);
            }

            conn.Close();
            MessageBox.Show("Requisicion creada", "Listo");
            sendmail();
            Close();
        }
        private void sendmail() {
            string gerentemail = getgerente();
            MailMessage mail = new MailMessage("aramis@posey.com", gerentemail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Nueva Requisicion de trabajo";
            mail.Body = "Se ha generado una nueva requisicion: \n"
                + textBox2.Text + "\n \n"
                + "Ingrese al Sistema ARAMIS por favor para continuar.";
            try {
                client.Send(mail);
                MessageBox.Show("Correo Enviado");
            } catch (Exception) {
                MessageBox.Show("Se presento un problema al enviar el correo");
            }
        }

        private string getgerente() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlalmacenistas = "select correo from users where " +
                    "id = (select gerente from deptos where " +
                    "id = (select depto from users where id = " + user_id + "))";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable tabla = new DataTable();
                adapter.Fill(tabla);
                DataRow dr = tabla.Rows[0];
                conn.Close();
                return dr[0].ToString();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
                return "";
            }
        }

        private void RequisicionMtto_Load(object sender, EventArgs e) {
            textBox1.Text = usuario;
        }

        private void button3_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK) {
                FileInfo fi = new FileInfo(dialog.FileName);
                long fileSize = fi.Length;
                if (fileSize <= 3000000) {
                    textBox3.Text = dialog.FileName;
                } else { MessageBox.Show("El archivo es demasiado extenso."); }
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK) {
                FileInfo fi = new FileInfo(dialog.FileName);
                long fileSize = fi.Length;
                if (fileSize <= 3000000) {
                    textBox4.Text = dialog.FileName;
                } else { MessageBox.Show("El archivo es demasiado extenso."); }
            }
        }
    }
}
