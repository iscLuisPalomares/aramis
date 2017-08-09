using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ApproveApAjuste : Form {
        public ApproveApAjuste() {
            InitializeComponent();
        }
        public string usuario           { get; set; }
        public string tipo              { get; set; }
        public string user_id           { get; set; }
        public string user_depto        { get; set; }
        public string deptoid           { get; set; }
        public string user_deptoid      { get; set; }
        public string user_gerenteid    { get; set; }
        public string gerenteid         { get; set; }
        public string ajusteid          { get; set; }
        public string account           { get; set; }
        public string accountid         { get; set; }
        string creator;
        List<string> files = new List<string>();
        List<string> codigos = new List<string>();
        List<string> unidadesdemedida = new List<string>();
        
        private string getaccountid() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT id FROM accounts where acctnumber = @text3;";
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                adapteralmacen.SelectCommand.Parameters.AddWithValue("@text3", textBox3.Text);
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
        private string getcorreo() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlalmacenistas = "select usuario.correo "
                    + "from tbajustes join users usuario on usuario.username = fscreatedby "
                    + "where fsid = @ajusteid;";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlalmacenistas, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@ajusteid", ajusteid);
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
        private void getbag() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT balance, budget, ajustado, gasto, asignado, id_bucket FROM " + 
                    " buckets WHERE id_cuenta = @getaccountid AND periodo = @date1;";
                string sqlquery2 = "select Deptos.name from accounts left join deptos " + 
                    "on accounts.depto = Deptos.id where acctnumber = @account;";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@getaccountid", getaccountid());
                adapter.SelectCommand.Parameters.AddWithValue("@date1", DateTime.Now.ToString("yyyy-MM"));
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery2, conn);
                adapter2.SelectCommand.Parameters.AddWithValue("@account", account);
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
        private void disapproveajuste() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                double totalarecuperar = 0;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    totalarecuperar += double.Parse(dr.Cells["Valor Inventario"].Value.ToString());
                }
                string disapproverequiquery = "UPDATE tbajustes SET " +
                    "fsapprovedate = GETDATE(), " +
                    "fsapprovedby = @usuario, " +
                    "fsstatus = 'Ajuste Desaprobado' WHERE fsid = @ajusteid; \n"
                    + "UPDATE tbajusteslineas SET fsstatus = 'Ajuste Desaprobado' " +
                    "WHERE fsajusteid = @ajusteid;"
                    + "update "
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
                    + "SELECT bucketid, sum(fsabsolutodlls) AS 'Total estimado dlls'"
                    + ", sum(absolutdllscot) as 'Total cotizado' "
                    + "FROM materialrequerido WHERE fsstatus in ('Requisicion Creada', 'Requisicion Aprobada' "
                    + ", 'Cotizacion Creada', 'Cotizacion Aprobada', 'PO Creado', 'PO Recibiendo'"
                    + ", 'PO Aprobado', 'PO creado') GROUP BY bucketid "
                    + ") asignados ON bucks.id_bucket  = asignados.bucketid "
                    + "update buckets set balance = ajustado - gasto - asignado \n";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = disapproverequiquery;
                ejecucion.Parameters.AddWithValue("@usuario", usuario);
                ejecucion.Parameters.AddWithValue("@ajusteid", ajusteid);
                ejecucion.ExecuteNonQuery();
                sendmaildisapprove();
                conn.Close();

                MessageBox.Show("Ajuste Desaprobado", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void aprobarajuste() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION;\n"
                    + "UPDATE tbajustes SET fsapprovedate = GETDATE(), fsapprovedby = @usuario, " +
                    "fsstatus = 'Ajuste Preaprobado' WHERE fsid= @ajusteid;\n"
                    + "UPDATE tbajusteslineas SET fsstatus = 'Ajuste Preaprobado' "
                    + "WHERE fsajusteid = @ajusteid;\n"
                    + "COMMIT TRANSACTION;";
                SqlCommand ejecucion    = new SqlCommand();
                ejecucion.Connection    = conn;
                ejecucion.CommandType   = CommandType.Text;
                ejecucion.CommandText   = sqlquery;
                ejecucion.Parameters.AddWithValue("@usuario", usuario);
                ejecucion.Parameters.AddWithValue("@ajusteid", ajusteid);
                ejecucion.ExecuteNonQuery();
                
                conn.Close();
                sendmail();
                MessageBox.Show("Ajuste Aprobado", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void sendmail() {
            string vpmail = "amartinez@posey.com";
            if (Program.stringconnection == "Data Source=MEXATLAS\\REQUI;Database=testing;User ID=sa;Password=R3qu1@16;") {
                vpmail = "lpalomares@posey.com";
            }

            MailMessage mail = new MailMessage("aramis@posey.com", vpmail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Solicitud de Ajuste de inventario";
            mail.Body = "Se generó una nueva solicitud de ajuste, ingrese al Sistema ARAMIS por favor para continuar.";
            try {
                client.Send(mail);
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
        }
        private void sendmaildisapprove() {
            string creadormail = getcorreo();
            MailMessage mail = new MailMessage("aramis@posey.com", creadormail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Ajuste desaprobado";
            mail.Body = "Se ha denegado un ajuste, ingrese al Sistema ARAMIS por favor para más detalle.";
            try {
                client.Send(mail);
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
        }
        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT * FROM tbajustes WHERE fsid = @ajusteid;";
                string sqlquery3 = "SELECT name FROM Deptos LEFT JOIN accounts ON deptos.id = Accounts.depto "
                    + "WHERE accounts.acctnumber = @account;";
                string sqlquery4 = "select fsid as 'ID', fsajusteid as 'Ajuste ID', sku as 'SKU', "
                    + "fscantidadensistema as 'Qty Sistema', fscantidadfisica as 'Qty Fisica', "
                    + "fsdiferencia as 'Diferencia', fscostounitario as 'Costo unitario', "
                    + "fscostoext as 'Costo Extendido', fsvalorencontra as 'Valor en Contra', "
                    + "fsvalorafavor as 'Valor a Favor', fsrazondetail as 'Razon', fsresponsable as 'Responsable', "
                    + "fsbucketid as 'Bucket ID', fsstatus as 'Status', fslocacion as 'Locacion', "
                    + "fsconfiabilidad as 'Confiabilidad', fsfecha as 'Fecha', fsconto as 'Conto', "
                    + "fsvalorabsoluto as 'Absoluto', fsvalorinventario as 'Valor Inventario', fsqty as 'Qty' "
                    + "from tbajusteslineas where fsajusteid = @ajusteid;";
                conn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@ajusteid", ajusteid);
                SqlDataAdapter adapter3 = new SqlDataAdapter(sqlquery3, conn);
                adapter3.SelectCommand.Parameters.AddWithValue("@account", account);
                SqlDataAdapter adapter4 = new SqlDataAdapter(sqlquery4, conn);
                adapter4.SelectCommand.Parameters.AddWithValue("@ajusteid", ajusteid);
                DataTable tb = new DataTable();
                DataTable tb3 = new DataTable();
                DataTable tb4 = new DataTable();
                adapter.Fill(tb);
                adapter3.Fill(tb3);
                adapter4.Fill(tb4);
                
                dataGridView1.DataSource = tb4;
                dataGridView1.Columns["Qty"].Visible = false;
                dataGridView1.Columns["Bucket ID"].Visible = false;
                dataGridView1.Columns["Status"].Visible = false;
                dataGridView1.Columns["Confiabilidad"].Visible = false;
                try {
                    DataRow dr = tb.Rows[0];
                    creator = dr[2].ToString();
                    textBox5.Text = creator;
                    textBox2.Text = dr[6].ToString();
                    label9.Text = "Departamento:        " + tb3.Rows[0][0].ToString();
                    textBox3.Text = account;
                    textBox1.Text = dr[11].ToString();
                } catch (Exception e) {
                    label7.Text = "Se presento un error";
                    MessageBox.Show(e.ToString());
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        private void getrazonescount() {
            try {
                string connectionstring = Program.stringconnection;

                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "select fsrazondetail, count(fsrazondetail) as suma, "
                    + "sum(fsvalorinventario) as neto, "
                    + "sum(fsvalorafavor) as ganancia, "
                    + "sum(fsvalorencontra) as perdida "
                    + "from tbajusteslineas where fsajusteid = @ajusteid group by fsrazondetail";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@ajusteid", ajusteid);
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                dataGridView2.DataSource = tb;
                label29.Text = "";
                label30.Text = "";
                label31.Text = "";
                label36.Text = "";
                foreach (DataGridViewRow dr in dataGridView2.Rows) {
                    label29.Text += dr.Cells[0].Value.ToString() + "\n";
                    label36.Text += double.Parse(dr.Cells[3].Value.ToString()) + double.Parse(dr.Cells[4].Value.ToString()) + "\n";
                    label30.Text += dr.Cells[3].Value.ToString() + "\n";
                    label31.Text += dr.Cells[4].Value.ToString() + "\n";
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        private void bucketantes() {
            try {
                double totalajuste = 0;
                double cbalance = double.Parse(label16.Text);
                double cbudget = double.Parse(label17.Text);
                double cgasto = double.Parse(label18.Text);
                double cajustado = double.Parse(label19.Text);
                double casignado = double.Parse(label20.Text);
                double totalperdidas = 0;
                double totalganacias = 0;

                double tabsoluto = 0;
                double tinventario = 0;

                if (dataGridView1.RowCount >= 1) {
                    foreach (DataGridViewRow dr in dataGridView1.Rows) {
                        if (!dr.IsNewRow) {
                            totalajuste += double.Parse(dr.Cells["Costo Extendido"].Value.ToString());
                            totalganacias += double.Parse(dr.Cells["Valor a Favor"].Value.ToString());
                            totalperdidas += double.Parse(dr.Cells["Valor en Contra"].Value.ToString());
                            tinventario += double.Parse(dr.Cells["Valor Inventario"].Value.ToString());
                            tabsoluto += double.Parse(dr.Cells["Absoluto"].Value.ToString());
                            casignado = casignado - double.Parse(dr.Cells["Valor Inventario"].Value.ToString());
                            cbalance = cajustado - cgasto - casignado;
                        }
                    }
                } else { cbalance = cajustado - cgasto - casignado; }

                label26.Text = "Costo de Ajuste: $" + (totalperdidas + totalganacias);
                label27.Text = "Perdidas: $" + totalperdidas;
                label28.Text = "Ganancias: $" + totalganacias;
                label25.Text = cbalance.ToString();
                label24.Text = cbudget.ToString();
                label23.Text = cgasto.ToString();
                label22.Text = cajustado.ToString();
                label21.Text = casignado.ToString();
                label37.Text = "Discrepancia: " + Math.Abs(Math.Round(tabsoluto / totalajuste, 4) * 100) + "%";
                label38.Text = "Veracidad: " + Math.Round((1 - Math.Abs(tabsoluto / totalajuste)) * 100, 2) + "%";
                
            } catch (Exception) {
                label25.Text = "";
                label24.Text = "";
                label23.Text = "";
                label22.Text = "";
                label21.Text = "";
                MessageBox.Show("Error");
            }
        }
        
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void button4_Click(object sender, EventArgs e) {
            FilesAjuste fr = new FilesAjuste();
            fr.ajusteid = ajusteid;
            fr.usuario = usuario;
            fr.ShowInTaskbar = false;
            fr.ShowDialog();
        }
        private void CreateRequi_Load(object sender, EventArgs e) {
            getdata();
            getbag();
            bucketantes();
            getrazonescount();
            textBox5.Text = usuario;
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            getbag();
        }
        
        private void pictureBox1_Click(object sender, EventArgs e) {
            aprobarajuste();
            Close();
        }
        private void pictureBox2_Click(object sender, EventArgs e) {
            disapproveajuste();
            Close();
        }
    }
}