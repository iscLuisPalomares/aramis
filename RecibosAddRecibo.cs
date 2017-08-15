using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class RecibosAddRecibo : Form {
        public RecibosAddRecibo() {
            InitializeComponent();
        }
        public string idlinea       { get; set; }
        public string qty           { get; set; }
        public string usuario       { get; set; }
        public string user_id       { get; set; }
        public string saldo         { get; set; }
        public string recibido      { get; set; }
        public string cuenta        { get; set; }
        public string costounidad   { get; set; }
        public string idpo          { get; set; }
        public string accountid     { get; set; }
        public string requisicionid { get; set; }
        public string bucketid      { get; set; }
        public double costorecibo;

        private void getaccountid() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT id FROM Accounts WHERE acctnumber = '" + cuenta + "'";

                conn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                DataRow dr = tb.Rows[0];
                accountid = dr[0].ToString();

                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void setnewreceipt() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                //crear nuevo recibo
                sqlquery += "INSERT INTO tblrecibos (fsidpo, fsidlinea, fsdate, fsuserid, fscantidad, fscoments," +
                    "fscostoarecibir, fscuentaid, fsalias) VALUES ('" + idpo + "','" + idlinea + "','" + DateTime.Now.ToString() + "', " +
                    "'" + user_id + "', " + numericUpDown1.Value.ToString() + ", '" + textBox2.Text + "','" + textBox3.Text + "','" + accountid + "'," + 
                    "'alias');\n";
                //si el saldo de la linea esta en blanco y el recibido esta en blanco
                if (saldo == "" && recibido == "") {
                    double nuevosaldo = double.Parse(qty) - double.Parse(numericUpDown1.Value.ToString());
                    double nuevorecibido = double.Parse(numericUpDown1.Value.ToString());
                    if (nuevosaldo == 0) {
                        sqlquery += "UPDATE materialrequerido SET "
                            + "saldo = '" + nuevosaldo + "', "
                            + "recibido = '" + nuevorecibido + "', "
                            + "fsstatus = 'PO Recibido'"
                            + "WHERE fsid = '" + idlinea + "';\n";
                    } else {
                        sqlquery += "UPDATE materialrequerido SET "
                            + "saldo = '" + nuevosaldo + "', "
                            + "recibido = '" + nuevorecibido + "', "
                            + "fsstatus = 'PO Recibiendo' "
                            + "WHERE fsid = '" + idlinea + "';\n";
                    }
                    if (nuevosaldo == 0) {
                        sqlquery += "UPDATE tblpurchaseorders SET fsstatus = 'PO Recibido' WHERE fsid = " + idpo + ";\n";
                    } else {
                        sqlquery += "UPDATE tblpurchaseorders SET fsstatus = 'PO Recibiendo' WHERE fsid = " + idpo + ";\n";
                    }
                } else {
                    double nuevorecibido = double.Parse(recibido) + double.Parse(numericUpDown1.Value.ToString());
                    double nuevosaldo = double.Parse(qty) - nuevorecibido;
                    if (nuevosaldo == 0) {
                        sqlquery += "UPDATE materialrequerido SET "
                        + "saldo = '" + nuevosaldo + "', "
                        + "recibido = '" + nuevorecibido + "', "
                        + "fsstatus = 'PO Recibido'"
                        + "WHERE fsid = '" + idlinea + "';\n";
                    } else {
                        sqlquery += "UPDATE materialrequerido SET "
                        + "saldo = '" + nuevosaldo + "', "
                        + "recibido = '" + nuevorecibido + "', "
                        + "fsstatus = 'PO Recibiendo' "
                        + "WHERE fsid = '" + idlinea + "';\n";
                    }
                    if (nuevosaldo == 0) {
                        sqlquery += "UPDATE tblpurchaseorders SET fsstatus = 'PO Recibido' WHERE fsid = " + idpo + ";\n";
                    } else {
                        sqlquery += "UPDATE tblpurchaseorders SET fsstatus = 'PO Recibiendo' WHERE fsid = " + idpo + ";\n";
                    }
                }
                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                
                actualizarbuckets();
                enviarcorreo(idlinea);
                MessageBox.Show("Actualizado", "Listo");
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void enviarcorreo(string idlinea) {
            string requisitormail = getrequisitor(idlinea);
            MailMessage mail = new MailMessage("aramis@posey.com", requisitormail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Nueva Requisicion para Aprobar";
            mail.Body = "Se ha generado una nueva requisicion, ingrese al Sistema ARAMIS por favor para continuar.";
            try {
                client.Send(mail);
                MessageBox.Show("Correo Enviado");
            } catch (Exception) {
                MessageBox.Show("Se presento un problema al enviar el correo");
            }
        }

        private string getrequisitor(string idlinea) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "select top 1 correo from users where " +
                    "id = (select gerente from deptos where " +
                    "id = (select depto from users where id = " + user_id + "))";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
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

        private void getbucketid() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                string requidatequery = "select createdate from requisiciones where id_req = '" + requisicionid + "'";

                SqlDataAdapter adapter = new SqlDataAdapter(requidatequery, conn);
                

                DataTable table = new DataTable();
                adapter.Fill(table);
                string datestring = table.Rows[0][0].ToString();
                DateTime dtfecha = Convert.ToDateTime(datestring);
                string bucketidquery = "select id_bucket from buckets where id_cuenta = " +
                    "(select id from accounts where acctnumber = '" + cuenta + "') and " +
                    "periodo = '" + dtfecha.ToString("yyyy-MM") + "'";
                
                SqlDataAdapter adapter2 = new SqlDataAdapter(bucketidquery, conn);
                
                DataTable table2 = new DataTable();
                adapter2.Fill(table2);
                bucketid = table2.Rows[0][0].ToString();
                
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void actualizarbuckets() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string bucketaffected = "SELECT * FROM buckets WHERE id_bucket = " + bucketid + "";
                SqlDataAdapter adapter = new SqlDataAdapter(bucketaffected, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                
                double gasto = double.Parse(table.Rows[0][4].ToString());
                double asignado = double.Parse(table.Rows[0][5].ToString());
                double costorecibo = double.Parse(textBox3.Text);
                gasto = gasto + costorecibo;
                asignado = asignado - costorecibo;

                string sqlqueryfinal = "update "
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
                ejecucion.CommandText = sqlqueryfinal;
                ejecucion.ExecuteNonQuery();

                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void RecibosAddRecibo_Load(object sender, EventArgs e) {
            getaccountid();
            getbucketid();
        }
        private void button1_Click(object sender, EventArgs e) {
            if (recibido == null || recibido == "") {
                recibido = "0";
            }
            if (double.Parse(recibido) + double.Parse(numericUpDown1.Value.ToString()) > double.Parse(qty)) {
                MessageBox.Show("No se puede recibir esta cantidad");
            } else {
                setnewreceipt();
                Close();
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            try {
                costorecibo = double.Parse(costounidad) * double.Parse(numericUpDown1.Value.ToString());
                textBox3.Text = costorecibo.ToString();

            } catch (Exception) {
                textBox3.Text = "";
            }
        }
        private void sendmail() {
            string gerentemail = getgerente();
            MailMessage mail = new MailMessage("aramis@posey.com", gerentemail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Se recibio/recibieron linea(s) de requisicion";
            mail.Body = "Llegaron lineas de requisicion, ingrese al Sistema ARAMIS por favor para verificar.";
            //client.Send(mail);
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
                MessageBox.Show(ex.Message);
                Close();
                return "";
            }
        }
    }
}
