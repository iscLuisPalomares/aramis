using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Windows.Forms;
using System.Drawing;

namespace ComprasProject {
    public partial class ApproveCotizacion : Form {
        public ApproveCotizacion() {
            InitializeComponent();
        }

        public string idcot         { get; set; }
        public string usuario       { get; set; }
        public string account       { get; set; }
        public string accountid     { get; set; }
        public double costototal    { get; set; }
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
        private void sendmaildisapproved(string motivo) {
            string creadormail = getcorreo();
            MailMessage mail = new MailMessage("aramis@posey.com", creadormail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Cotizacion: " + idcot + " ha sido desaprobada";
            mail.Body = "Razon de rechazo:\n" + motivo;
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
                    + "where fsid = @idcot;";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlalmacenistas, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@idcot", idcot);
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
                string sqlalmacenistas = "SELECT id FROM accounts where acctnumber = @accountnum;";
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                adapteralmacen.SelectCommand.Parameters.AddWithValue("@accountnum", accountnum);
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
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n"
                    + "UPDATE tbcotizaciones SET fsapprovedate = GETDATE(), fsapprovedby = @usuario, "
                    + "fsstatus = 'Cotizacion Desaprobada' WHERE fsid = @idcot;\n"
                    + "UPDATE materialrequerido SET fsstatus = 'Cotizacion Desaprobada' WHERE fsid IN ("
                    + "SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = @idcot);\n"
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
                    + "SELECT bucketid, sum(fsabsolutodlls) AS 'Total estimado dlls', sum(absolutdllscot) as 'Total cotizado' "
                    + "FROM materialrequerido WHERE fsstatus in ('Requisicion Creada', 'Requisicion Aprobada' "
                    + ", 'Cotizacion Creada', 'Cotizacion Aprobada', 'PO Creado', 'PO Recibiendo'"
                    + ", 'PO Aprobado', 'PO creado') GROUP BY bucketid "
                    + ") asignados ON bucks.id_bucket  = asignados.bucketid "
                    + "update buckets set balance = ajustado - gasto - asignado; \n";
                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.Parameters.AddWithValue("@usuario", usuario);
                ejecucion.Parameters.AddWithValue("@idcot", idcot);
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MotivoCotRechazada mot = new MotivoCotRechazada();
                mot.ShowDialog();
                sendmaildisapproved(mot.motivo);
                MessageBox.Show("Cotizacion Desaprobada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void aprobarcotizacion() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n"
                    + "UPDATE tbcotizaciones SET "
                    + "fsapprovedate = GETDATE(), fsapprovedby = @usuario"
                    + ", fsstatus = 'Cotizacion Aprobada' WHERE fsid = @idcot;\n"
                    + "UPDATE materialrequerido SET fsstatus = 'Cotizacion Aprobada' WHERE fsid IN (" 
                    + "SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido "
                    + "WHERE fsidcotizacion = @idcot);\n"
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
                    + "SELECT bucketid, sum(fsabsolutodlls) AS 'Total estimado dlls', sum(absolutdllscot) as 'Total cotizado' "
                    + "FROM materialrequerido WHERE fsstatus in ('Requisicion Creada', 'Requisicion Aprobada' "
                    + ", 'Cotizacion Creada', 'Cotizacion Aprobada', 'PO Creado', 'PO Recibiendo'"
                    + ", 'PO Aprobado', 'PO creado') GROUP BY bucketid "
                    + ") asignados ON bucks.id_bucket  = asignados.bucketid "
                    + "update buckets set balance = ajustado - gasto - asignado; \n";
                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.Parameters.AddWithValue("@usuario", usuario);
                ejecucion.Parameters.AddWithValue("@idcot", idcot);
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
                numeros += "'" + dr.Cells["Bucket"].Value.ToString() + "',";
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
                    + "ajustado as Ajustado, gasto as Gasto, asignado as Asignado, "
                    + "balance as Balance, periodo as Periodo "
                    + "FROM buckets WHERE id_bucket IN (" +
                    getreqnums() + ")";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                DataTable table3 = new DataTable();
                adapter.Fill(table);
                adapter.Fill(table3);
                dataGridView2.DataSource = table;
                dataGridView3.DataSource = table3;
                foreach (DataGridViewColumn dc in dataGridView2.Columns) {
                    dc.Width = 70;
                }
                foreach (DataGridViewColumn dc in dataGridView3.Columns) {
                    dc.Width = 70;
                }
                conn.Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void actualizarconsumo() {
            try {
                foreach (DataGridViewRow dr1 in dataGridView1.Rows) {
                    double lineaestimado = double.Parse(dr1.Cells["Estimado Dlls"].Value.ToString());
                    string lineabucket = dr1.Cells["Bucket"].Value.ToString();
                    foreach (DataGridViewRow dr2 in dataGridView2.Rows) {
                        if (dr2.Cells[0].Value.ToString() == lineabucket) {
                            int indice = dr2.Index;
                            double bucketasignado = double.Parse(dr2.Cells[5].Value.ToString());
                            bucketasignado = bucketasignado - lineaestimado;
                            double bucketajustado = double.Parse(dr2.Cells[3].Value.ToString());
                            double bucketgasto      = double.Parse(dr2.Cells[4].Value.ToString());
                            double bucketbalance    = double.Parse(dr2.Cells[6].Value.ToString());
                            bucketbalance = bucketajustado - bucketgasto - bucketasignado;
                            dr2.Cells[6].Value = bucketbalance;
                            dr2.Cells[5].Value = bucketasignado;
                            dataGridView3[5, indice].Value = bucketasignado;
                            dataGridView3[6, indice].Value = bucketbalance;
                        }
                    }
                }
                foreach (DataGridViewRow dr1 in dataGridView1.Rows) {
                    double lineacotizado = double.Parse(dr1.Cells["Costo en Dlls"].Value.ToString());
                    string lineabucket = dr1.Cells["Bucket"].Value.ToString();
                    foreach (DataGridViewRow dr3 in dataGridView3.Rows) {
                        if (dr3.Cells[0].Value.ToString() == lineabucket) {
                            double bucketasignado = double.Parse(dr3.Cells[5].Value.ToString());
                            bucketasignado = bucketasignado + lineacotizado;
                            double bucketajustado = double.Parse(dr3.Cells[3].Value.ToString());
                            double bucketgasto = double.Parse(dr3.Cells[4].Value.ToString());
                            double bucketbalance = double.Parse(dr3.Cells[6].Value.ToString());
                            bucketbalance = bucketajustado - bucketgasto - bucketasignado;
                            dr3.Cells[6].Value = bucketbalance;
                            dr3.Cells[5].Value = bucketasignado;
                        }
                    }
                }
            } catch (Exception e) {
                MessageBox.Show(e.ToString());
            }
        }
        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "select matreq.fsid as 'ID', "
                    + "deps.name as 'Departamento', usuario.fulname as 'Usuario', "
                    + "matreq.fscodigo as 'Codigo', matreq.fsdesc as 'Descripcion', "
                    + "matreq.fscantidad as 'Cantidad', matreq.fsunimedida as 'U/M', "
                    + "matreq.fscuenta as 'Cuenta', matreq.bucketid as 'Bucket', "
                    + "matreq.fscostounitario as 'Costo Unitario', matreq.fstotalcost as 'Costo Extendido', "
                    + "matreq.divisacot as 'Divisa Cotizacion', matreq.absolutdllscot as 'Costo en Dlls', "
                    + "matreq.fsabsolutodlls as 'Estimado Dlls', "
                    + "matreq.bucketid as 'Bucket', "
                    + "tbsku.fscategoria_reportesku as 'Categoria para reportes' "
                    + "from materialrequerido matreq "
                    + "join requisiciones req on req.id_req = matreq.fsrequisicion "
                    + "join Deptos deps on deps.id = req.deptoid "
                    + "join Users usuario on usuario.username = req.createdby "
                    + "join sku tbsku on tbsku.sku = fscodigo "
                    + "where fsid in ( "
                    + "select fsidmaterialrequerido "
                    + "from tbcotmaterialrequerido where fsidcotizacion = @idcot"
                    + ");";
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter(sqlquery, conn);
                adapter1.SelectCommand.Parameters.AddWithValue("@idcot", idcot);
                DataTable tb1 = new DataTable();
                adapter1.Fill(tb1);
                dataGridView1.DataSource = tb1;

                dataGridView1.Columns["Descripcion"].Width = 250;
                dataGridView1.Columns["Cantidad"].Width = 70;
                dataGridView1.Columns["U/M"].Width = 70;
                dataGridView1.Columns["Bucket"].Visible = false;
                dataGridView1.Columns["Cuenta"].Width = 150;
                dataGridView1.Columns["Bucket"].Width = 150;
                dataGridView1.Columns["Categoria para reportes"].Width = 200;
                dataGridView1.Columns["Estimado Dlls"].Visible = false;
                dataGridView1.Columns["Bucket1"].Visible = false;
                
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
            setantes();
            actualizarconsumo();
            gettotalendlls();
        }
        private void gettotalendlls() {
            try {
                costototalendlls = 0;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    costototalendlls += double.Parse(dr.Cells["Costo en Dlls"].Value.ToString());
                }
                label3.Text = "Total a aprobar $" + costototalendlls.ToString() + " dlls";
            } catch (Exception) { }
        }
        private string getcotizador() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select usuarios.fulname from tbcotizaciones "
                    + "join users usuarios on tbcotizaciones.createdby = usuarios.id where fsid = @idcot;";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@idcot", idcot);
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
        int rowindex = 0;
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                rowindex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                ContextMenuStrip m = new ContextMenuStrip();
                m.Items.Add("Editar");
                m.ItemClicked += M_ItemClicked;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }
        private void M_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            ToolStripItem btn = e.ClickedItem;
            if (btn.Text == "Editar") {
                try {
                    EditCategoriaReportes el = new EditCategoriaReportes();
                    el.lineaid = dataGridView1[0, rowindex].Value.ToString();
                    el.usuario = usuario;
                    el.ShowDialog();
                    getdata();
                } catch (Exception) { MessageBox.Show("Se presento un problema, intente de nuevo."); }
            }
        }
    }
}