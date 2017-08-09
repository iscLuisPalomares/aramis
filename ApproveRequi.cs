using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Net.Mail;

namespace ComprasProject {
    public partial class ApproveRequi : Form {
        public ApproveRequi() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_id { get; set; }
        public string user_depto { get; set; }
        public string deptoid { get; set; }
        public string user_deptoid { get; set; }
        public string user_gerenteid { get; set; }
        public string gerenteid { get; set; }
        public string idreq { get; set; }
        public string account { get; set; }
        public string accountid { get; set; }
        List<string> files = new List<string>();
        List<string> codigos = new List<string>();
        List<string> unidadesdemedida = new List<string>();

        public string getaccountid() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT id FROM accounts where acctnumber = '" + textBox3.Text + "'";
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
        public void getbag() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT balance, budget, ajustado, gasto, asignado, id_bucket FROM " +
                    " buckets WHERE id_cuenta = '" + getaccountid() + "' AND periodo = '" +
                    DateTime.Now.ToString("yyyy-MM") + "'";
                string sqlquery2 = "select Deptos.name from accounts left join deptos " +
                    "on accounts.depto = Deptos.id where acctnumber = '" + account + "'";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery2, conn);
                DataTable tb = new DataTable();
                DataTable datatable2 = new DataTable();
                adapter.Fill(tb);
                adapter2.Fill(datatable2);
                try {
                    DataRow dr = datatable2.Rows[0];
                    label9.Text = "Departamento: " + dr[0].ToString();
                    DataRow da = tb.Rows[0];

                    label20.Text = da[4].ToString();
                    label19.Text = da[2].ToString();
                    label18.Text = da[3].ToString();
                    label17.Text = da[1].ToString();
                    label16.Text = da[0].ToString();
                } catch (Exception) {
                    label7.Text = "Balance: ";
                }

                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void aprobarrequi() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                sqlquery += "UPDATE requisiciones SET dateapproved = " + "GETDATE()" + ", approvedby = '" + usuario + "', " +
                    "fsstatus = 'Requisicion Aprobada' WHERE id_req = '" + idreq + "';\n";
                sqlquery += "UPDATE materialrequerido SET fsstatus = 'Requisicion Aprobada' WHERE fsrequisicion = '" + idreq + "'\n";
                sqlquery += wipebuckets();
                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Requisicion aprobada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private string wipebuckets() {
            string query = "update "
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
            return query;
        }
        private void disapproverequi() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                double totalarecuperar = 0;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    totalarecuperar += double.Parse(dr.Cells["Costo en Dlls"].Value.ToString());
                }
                string disapproverequi = "UPDATE requisiciones SET " +
                    "dateapproved = " + "GETDATE()" + ", " +
                    "approvedby = '" + usuario + "', " +
                    "fsstatus = 'Requisicion Desaprobada' WHERE id_req = '" + idreq + "';";
                string bucketdata = "SELECT * FROM buckets WHERE id_bucket = " +
                    dataGridView1["Bucket", 0].Value.ToString();
                string disapprovematerial = "UPDATE materialrequerido SET fsstatus = 'Requisicion Desaprobada' WHERE fsrequisicion = '" + idreq + "'\n";
                string bucketupdate = "";

                SqlDataAdapter adapter = new SqlDataAdapter(bucketdata, conn);
                DataTable tabla = new DataTable();
                adapter.Fill(tabla);
                double balance = double.Parse(tabla.Rows[0][6].ToString());
                double asignado = double.Parse(tabla.Rows[0][5].ToString());
                balance = balance + totalarecuperar;
                asignado = asignado - totalarecuperar;
                bucketupdate = "update "
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
                bucketupdate += wipebuckets();
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                //disapprove requisition
                ejecucion.CommandText = disapproverequi;
                ejecucion.ExecuteNonQuery();
                //disapprove material
                ejecucion.CommandText = disapprovematerial;
                ejecucion.ExecuteNonQuery();
                //restore bucket
                ejecucion.CommandText = bucketupdate;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MotivoCotRechazada mot = new MotivoCotRechazada();
                mot.ShowDialog();
                sendmaildisapproved(mot.motivo);
                MessageBox.Show("Requisicion Desaprobada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void sendmaildisapproved(string motivo) {
            string creadormail = getcorreo();
            if (creadormail.Length == 0) {
                MessageBox.Show("No user email available.");
            } else {
                MailMessage mail = new MailMessage("aramis@posey.com", creadormail);
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "mail.posey.com";
                mail.Subject = "Requisicion: " + idreq + " ha sido desaprobada";
                mail.Body = "Razon de rechazo:\n" + motivo;
                try {
                    client.Send(mail);
                } catch (Exception) {
                    MessageBox.Show("Problema al enviar correo");
                }
            }
        }
        private string getcorreo() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlalmacenistas = "select usuario.correo "
                    + "from requisiciones join users usuario on usuario.username = requisiciones.createdby "
                    + "where requisiciones.id_req = @idreq;";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlalmacenistas, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@idreq", idreq);
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
        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT * FROM requisiciones WHERE id_req = '" + idreq.ToString() + "'";
                string sqlquery3 = "SELECT name FROM Deptos LEFT JOIN accounts ON deptos.id = Accounts.depto WHERE accounts.acctnumber = '" + account + "'";
                string sqlquery4 = "select fsid as 'ID', fscodigo as 'Codigo', "
                    + "fsdesc as 'Descripcion', fsunimedida as 'U/M', fscantidad as 'Cantidad', "
                    + "fsmoneda as 'Moneda', fsabsolutodlls as 'Costo en Dlls', bucketid as 'Bucket' "
                    + "from materialrequerido "
                    + "where fsrequisicion = " + idreq + ";";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter3 = new SqlDataAdapter(sqlquery3, conn);
                SqlDataAdapter adapter4 = new SqlDataAdapter(sqlquery4, conn);
                
                DataTable tb = new DataTable();
                DataTable tb3 = new DataTable();
                DataTable tb4 = new DataTable();
                adapter.Fill(tb);
                adapter3.Fill(tb3);
                adapter4.Fill(tb4);
                dataGridView1.DataSource = tb4;
                dataGridView1.Columns["Bucket"].Visible = false;
                dataGridView1.Columns["Descripcion"].Width = 250;
                try {
                    DataRow dr = tb.Rows[0];
                    textBox2.Text = dr[6].ToString();
                    label9.Text = "Departamento: " + tb3.Rows[0][0].ToString();
                    textBox3.Text = account;
                    textBox1.Text = dr[10].ToString();
                    if (dr[9].ToString() == "true") {
                        checkBox1.Checked = true;
                    } else {
                        checkBox1.Checked = false;
                    }
                } catch (Exception) {
                    label7.Text = "Se presento un error";
                }
                conn.Close();
                sumallarticles();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void bucketantes() {
            try {
                double cbalance = double.Parse(label16.Text);
                double cbudget = double.Parse(label17.Text);
                double cgasto = double.Parse(label18.Text);
                double cajustado = double.Parse(label19.Text);
                double casignado = double.Parse(label20.Text);
                if (dataGridView1.RowCount >= 1) {
                    foreach (DataGridViewRow dr in dataGridView1.Rows) {
                        if (!dr.IsNewRow) {
                            casignado = casignado - double.Parse(dr.Cells["Costo en Dlls"].Value.ToString());
                            cbalance = cajustado - cgasto - casignado;
                        }
                    }
                } else {
                    cbalance = cajustado - cgasto - casignado;
                }

                label25.Text = Math.Round(cbalance, 2).ToString();
                label24.Text = Math.Round(cbudget, 2).ToString();
                label23.Text = Math.Round(cgasto, 2).ToString();
                label22.Text = Math.Round(cajustado, 2).ToString();
                label21.Text = Math.Round(casignado, 2).ToString();
            } catch (Exception) {
                label25.Text = "";
                label24.Text = "";
                label23.Text = "";
                label22.Text = "";
                label21.Text = "";
                MessageBox.Show("error");
            }
        }
        
        private void button1_Click(object sender, EventArgs e) {
            aprobarrequi();
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void CreateRequi_Load(object sender, EventArgs e) {
            getdata();
            getbag();
            bucketantes();
            textBox5.Text = usuario;
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            getbag();
        }
        private void button4_Click(object sender, EventArgs e) {
            FilesRequi fr = new FilesRequi();
            fr.reqid = idreq;
            fr.usuario = usuario;
            fr.ShowInTaskbar = false;
            fr.ShowDialog();
        }
        private void pictureBox1_Click(object sender, EventArgs e) {
            aprobarrequi();
            Close();
        }
        private void pictureBox2_Click(object sender, EventArgs e) {
            disapproverequi();
            Close();
        }
        private void sumallarticles() {
            try {
                double acumulado = 0;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    acumulado += double.Parse(dr.Cells["Costo en Dlls"].Value.ToString());
                }
                acumulado = Math.Round(acumulado, 2);
                label28.Text = "Total a Aprobar: $" + acumulado.ToString() + " Dlls";
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}