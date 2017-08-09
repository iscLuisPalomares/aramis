using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CotizacionEdit : Form {
        public CotizacionEdit() {
            InitializeComponent();
        }

        public string idcot { get; set; }
        public string usuario { get; set; }
        public string account { get; set; }
        public string accountid { get; set; }
        public double costototal { get; set; }
        public double costototalendlls { get; set; }
        private string comentarios;

        private void sendmailapproved() {
            string creadormail = getcorreo();
            MailMessage mail = new MailMessage("aramis@posey.com", creadormail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Cotizacion aprobada";
            mail.Body = "Se ha aprobado una nueva cotizacion.";
            try {
                client.Send(mail);
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
        }
        private void sendmaildisapproved() {
            string creadormail = getcorreo();
            MailMessage mail = new MailMessage("aramis@posey.com", creadormail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Cotizacion desaprobada";
            mail.Body = "Se ha desaprobado una nueva cotizacion.";
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
                string sqlalmacenistas = "select usuario.correo "
                    + "from tbcotizaciones join users usuario on usuario.id = tbcotizaciones.createdby "
                    + "where fsid = " + idcot + ";";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlalmacenistas, conn);
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
        private string getaccountid(string accountnum) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT id FROM accounts where acctnumber = '" + accountnum + "'";
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);
                string number = "";
                DataRow da = almacentb.Rows[0];
                number = da[0].ToString();
                conn.Close();
                return number;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
                return "";
            }
        }
        private void disapprovecot() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                sqlquery += "UPDATE tbcotizaciones SET fsapprovedate = " + "GETDATE()" + ", fsapprovedby = '" + usuario + "', " +
                    "fsstatus = 'Cotizacion Desaprobada' WHERE fsid = '" + idcot + "';\n";
                sqlquery += "UPDATE materialrequerido SET fsstatus = 'Cotizacion Desaprobada' WHERE fsid IN (" +
                    "SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = '" + idcot + "');\n";

                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Cotizacion Desaprobada", "Listo");
                sendmaildisapproved();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void aprobarcotizacion() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                sqlquery += "UPDATE tbcotizaciones SET fsapprovedate = " + "GETDATE()" + ", fsapprovedby = '" + usuario + "', " +
                    "fsstatus = 'Cotizacion Aprobada' WHERE fsid = '" + idcot + "';\n";
                sqlquery += "UPDATE materialrequerido SET fsstatus = 'Cotizacion Aprobada' WHERE fsid IN (" +
                    "SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = '" + idcot + "');\n";
                sqlquery += "";

                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();

                conn.Close();
                MessageBox.Show("Cotizacion aprobada", "Listo");
                sendmailapproved();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private string getreqnums() {
            string numeros = "";
            foreach (DataGridViewRow dr in dataGridView1.Rows) {
                numeros += "'" + dr.Cells["Cuenta"].Value.ToString() + "',";
            }
            string nuevo = numeros.Remove(numeros.Length - 1, 1);
            return nuevo;
        }
        private void setantes() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT id_bucket as Bucket, id_cuenta as Cuenta, budget as Budget, "
                    + "ajustado as Ajustado, gasto as Gasto, asignado as Asignado, balance as Balance, periodo as Periodo "
                    + "FROM buckets WHERE id_cuenta IN (" +
                    "SELECT id FROM accounts WHERE acctnumber IN (" +
                    getreqnums() + "))" + " AND periodo = '" + DateTime.Now.ToString("yyyy-MM") + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                DataTable table3 = new DataTable();
                adapter.Fill(table);
                adapter.Fill(table3);

                conn.Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "select fsid as 'ID',  fscodigo as 'Codigo', fsdesc as 'Descripcion', "
                    + "fscantidad as 'Cantidad', fsunimedida as 'U/M', fscuenta as 'Cuenta', bucketid as 'Bucket',"
                    + "fscostounitario as 'Costo Unitario', fstotalcost as 'Costo Extendido', "
                    + "divisacot as 'Divisa Cotizacion', absolutdllscot as 'Costo en Dlls', fsabsolutodlls as 'Estimado Dlls' "
                    + "from materialrequerido where fsid in (select fsidmaterialrequerido from tbcotmaterialrequerido where fsidcotizacion = '" + idcot + "')";
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter(sqlquery, conn);
                DataTable tb1 = new DataTable();
                adapter1.Fill(tb1);
                dataGridView1.DataSource = tb1;

                dataGridView1.Columns["Descripcion"].Width = 250;
                dataGridView1.Columns["Cantidad"].Width = 70;
                dataGridView1.Columns["U/M"].Width = 70;
                dataGridView1.Columns["Bucket"].Visible = false;
                dataGridView1.Columns["Cuenta"].Width = 150;
                dataGridView1.Columns["Estimado Dlls"].Visible = false;

                DataRow accountnumberrow = tb1.Rows[0];
                account = accountnumberrow["Cuenta"].ToString();
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            aprobarcotizacion();
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void button4_Click(object sender, EventArgs e) {
            FilesCot fr = new FilesCot();
            fr.reqid = idcot;
            fr.usuario = usuario;
            fr.ShowInTaskbar = false;
            fr.ShowDialog();
        }
        private void CreateRequi_Load(object sender, EventArgs e) {
            getdata();
            textBox5.Text = getcotizador();
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            gettotalendlls();
        }
        private void gettotalendlls() {
            try {
                costototalendlls = 0;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    costototalendlls += double.Parse(dr.Cells["Costo en Dlls"].Value.ToString());
                }
                label3.Text = "Total a aprobar $" + costototalendlls.ToString() + " dlls";
            } catch (Exception) {

            }
        }
        private string getcotizador() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select usuarios.fulname from tbcotizaciones join users usuarios on tbcotizaciones.createdby = usuarios.id where fsid = " + idcot;
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                return table.Rows[0][0].ToString();
            } catch (Exception) {
                return "";
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e) {
            aprobarcotizacion();
            Close();
        }
        private void pictureBox2_Click(object sender, EventArgs e) {
            disapprovecot();
            Close();
        }
        private void button1_Click_1(object sender, EventArgs e) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select motivo from requisiciones where id_req in "
                    + "(select distinct(materialrequerido.fsrequisicion) from materialrequerido where fsid in (";
                foreach (DataGridViewRow dir in dataGridView1.Rows) {
                    sqlquery += dir.Cells[0].Value.ToString() + ", ";
                }
                sqlquery = sqlquery.Remove(sqlquery.Length - 2);
                sqlquery += "))";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();

                adapter.Fill(table);
                comentarios = "";
                foreach (DataRow dr in table.Rows) {
                    comentarios += dr[0].ToString() + "\n";
                }
                MessageBox.Show(comentarios);
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                List<string> files = new List<string>();
                files = dialog.FileNames.ToList();
                textBox1.Text = "";
                foreach (string onefile in files) {
                    textBox1.Text += onefile + "|";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            set_uploadfiles(int.Parse(idcot), textBox1.Text.Split('|').ToList());
        }
        private void set_uploadfiles(int id, List<string> arch) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                try {
                    foreach (string onefile in arch) {
                        if (onefile != "") {
                            string sqlquery = "INSERT INTO tbcotfiles (fsidcotizacion, fsfilename, fsdate) VALUES ('" +
                            id.ToString() + "','" +
                            System.IO.Path.GetFileName(onefile) + "'," +
                            "GETDATE()" + ")";
                            SqlCommand ejecucion = new SqlCommand();
                            ejecucion.Connection = conn;
                            ejecucion.CommandType = CommandType.Text;
                            ejecucion.CommandText = sqlquery;
                            ejecucion.ExecuteNonQuery();
                            if (!System.IO.Directory.Exists(@"\\mexfs01\TJTemp\Opardo\FOLIOS\cotizaciones\" + id.ToString())) {
                                System.IO.Directory.CreateDirectory(@"\\mexfs01\TJTemp\Opardo\FOLIOS\cotizaciones\" + id.ToString());
                            }
                            System.IO.File.Copy(onefile, @"\\mexfs01\TJTemp\Opardo\FOLIOS\cotizaciones\" + id.ToString() + @"\" + System.IO.Path.GetFileName(onefile), true);
                        }
                    }
                } catch (SqlException e) {
                    MessageBox.Show(e.ToString());
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}