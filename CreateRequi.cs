using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CreateRequi : Form {
        public CreateRequi() {
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
        public string idbucket { get; set; }

        List<string> deptos = new List<string>();
        List<string> files = new List<string>();
        List<string> codigos = new List<string>();
        List<string> unidadesdemedida = new List<string>();

        double casignado = 0;
        double cbalance = 0;
        double cajustado = 0;
        double cgasto = 0;
        double cbudget = 0;

        public void createreq() {
            try {
                double tres = double.Parse(label25.Text);
                double cuatro = double.Parse(label26.Text);
                double cinco = double.Parse(label24.Text);
                double seis = tres - cuatro - cinco;

                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION \n";
                sqlquery += "DECLARE @reqid INTEGER; \n";
                sqlquery += "INSERT INTO requisiciones (createdate, createdby, account, motivo, deptoid, gerenteid, urgente, donde, fsstatus) VALUES (" +
                    "GETDATE()" + ",'" + usuario + "','" +
                    comboBox2.SelectedItem.ToString() + "','" +
                    textBox2.Text + "','" +
                    deptoid + "','" +
                    gerenteid + "','" +
                    urgente() + "','" +
                    comboBox1.SelectedItem.ToString() + "','" +
                    "Requisicion Creada" + "'); SELECT @reqid = SCOPE_IDENTITY();\n";
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
                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    if (!row.IsNewRow) {
                        try {
                            sqlquery += "INSERT INTO materialrequerido (fsrequisicion, " +
                                "fscantidad, fsunimedida, fscodigo, fsdesc, fscostoestimado, " +
                                "fsmoneda, fsabsolutodlls, fsstatus, fscuenta, bucketid, comentario) VALUES ("
                                + "@reqid" + ",'" +
                                row.Cells[3].Value.ToString() + "','" +
                                row.Cells[4].Value.ToString() + "','" +
                                row.Cells[1].Value.ToString() + "','" +
                                row.Cells[2].Value.ToString() + "','" +
                                row.Cells[5].Value.ToString() + "','" +
                                row.Cells[6].Value.ToString() + "','" +
                                row.Cells[9].Value.ToString() + "','" +
                                "Requisicion Creada', '" +
                                comboBox2.SelectedItem.ToString() + "','" +
                                idbucket + "','" +
                                row.Cells[10].Value.ToString() + "');\n";
                        } catch (Exception es) {
                            MessageBox.Show(es.ToString());
                        }
                    }
                }
                List<string> archivos = new List<string>();
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    try {
                        archivos = dr.Cells[8].Value.ToString().Split('|').ToList();
                        foreach (string onefile in archivos) {
                            if (onefile != "") {
                                sqlquery += "INSERT INTO req_files (_name, _date, _userid, _reqid) VALUES ('" +
                                System.IO.Path.GetFileName(onefile) + "'," +
                                "GETDATE()" + ",'" +
                                user_id + "',@reqid);\n";
                            }
                        }
                    } catch (Exception) { }
                }
                sqlquery += wipebuckets();
                sqlquery += "SELECT @reqid;\n COMMIT TRANSACTION;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                string id = ejecucion.ExecuteScalar().ToString();
                foreach (string onefile in archivos) {
                    if (onefile != "") {
                        if (!System.IO.Directory.Exists(@"\\mexfs01\TJTemp\Opardo\FOLIOS\requisiciones\" + id.ToString())) {
                            System.IO.Directory.CreateDirectory(@"\\mexfs01\TJTemp\Opardo\FOLIOS\requisiciones\" + id.ToString());
                        }
                        System.IO.File.Copy(onefile, @"\\mexfs01\TJTemp\Opardo\FOLIOS\requisiciones\" + id.ToString() + @"\" + System.IO.Path.GetFileName(onefile), true);
                    }
                }
                conn.Close();
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MessageBox.Show("Requisicion creada", "Listo");
                sendmail();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
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
        public string urgente() {
            if (checkBox1.Checked) { return "true"; } else { return "false"; }
        }
        public void getcuentas() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select * from accounts left join buckets on Accounts.id = buckets.id_cuenta where depto = " +
                    user_deptoid + " AND periodo = '" + DateTime.Now.ToString("yyyy-MM") + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                if (tb.Rows.Count > 0) {
                    foreach (DataRow da in tb.Rows) {
                        comboBox2.Items.Add(da[1].ToString());
                    }
                    comboBox2.SelectedIndex = 0;
                } else {
                    MessageBox.Show("No Existen Cuentas asignadas a su departamento, Revise con Finanzas", "Error");
                    Close();
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        public void get_locaciones() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlalmacenistas = "SELECT * FROM locacionuso";
                conn.Open();
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);

                comboBox1.Items.Clear();
                foreach (DataRow da in almacentb.Rows) {
                    comboBox1.Items.Add(da[1].ToString());
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        public string getaccountid() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT id FROM accounts where acctnumber = '" + comboBox2.SelectedItem.ToString() + "'";
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
                MessageBox.Show(ex.ToString());
                Close();
                return "";
            }
        }
        public void getbag() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string query1 = "SELECT balance, budget, ajustado, gasto, asignado, id_bucket FROM buckets WHERE id_cuenta = '" + getaccountid() + "' AND periodo = '" + DateTime.Now.ToString("yyyy-MM") + "'";
                string sqlquery2 = "select Deptos.name from accounts left join deptos on accounts.depto = Deptos.id where acctnumber = '" + comboBox2.SelectedItem.ToString() + "'";
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter(query1, conn);
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery2, conn);
                
                DataTable datatabletb = new DataTable();
                DataTable datatable2 = new DataTable();
                adapter1.Fill(datatabletb);
                adapter2.Fill(datatable2);
                try {
                    DataRow dr = datatable2.Rows[0];
                    label9.Text = "Departamento: " + dr[0].ToString();
                    DataRow da = datatabletb.Rows[0];
                    label23.Text = da[0].ToString();
                    label22.Text = da[1].ToString();
                    label21.Text = da[3].ToString();
                    label20.Text = da[2].ToString();
                    label19.Text = da[4].ToString();
                    actualizarconsumo();
                    idbucket = da[5].ToString();
                } catch (Exception) {
                    label23.Text = "-";
                    label22.Text = "-";
                    label21.Text = "-";
                    label20.Text = "-";
                    label19.Text = "-";
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        public void actualizarconsumo() {
            try {
                cbalance = double.Parse(label23.Text);
                cbudget = double.Parse(label22.Text);
                cgasto = double.Parse(label21.Text);
                cajustado = double.Parse(label20.Text);
                casignado = double.Parse(label19.Text);
                if (dataGridView1.RowCount >= 1) {
                    foreach (DataGridViewRow dr in dataGridView1.Rows) {
                        if (!dr.IsNewRow) {
                            casignado = casignado + double.Parse(dr.Cells[9].Value.ToString());
                            cbalance = cajustado - cgasto - casignado;
                        }
                    }
                } else {
                    cbalance = cajustado - cgasto - casignado;
                }
                label28.Text = cbalance.ToString();
                label27.Text = cbudget.ToString();
                label26.Text = cgasto.ToString();
                label25.Text = cajustado.ToString();
                label24.Text = casignado.ToString();
            } catch (Exception) {
                label28.Text = "";
                label27.Text = "";
                label26.Text = "";
                label25.Text = "";
                label24.Text = "";
            }
        }
        public void fillcodigos(string categoria, int row) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT id, sku, skudesc FROM sku WHERE category = '" + categoria + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                codigos.Clear();
                foreach (DataRow da in tb.Rows) {
                    codigos.Add(da[0].ToString() + ";" + da[1].ToString() + ";" + da[2].ToString());
                }
                conn.Close();
                dataGridView1.Rows[row].Cells[1] = new DataGridViewComboBoxCell();
                DataGridViewComboBoxCell dgvcbc = (DataGridViewComboBoxCell)dataGridView1.Rows[row].Cells[1];
                dgvcbc.Items.Clear();
                foreach (string cod in codigos) {
                    dgvcbc.Items.Add(cod.ToString().Split(';')[1]);
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        public void fillunimedidas() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT fsid, fsdesc FROM tbunimedida";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);
                foreach (DataRow da in datatable.Rows) {
                    unidadesdemedida.Add(da[0].ToString() + ";" + da[1].ToString());
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        public void fillcategorias() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT category FROM tbsku_cat";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        public void getdescripcion(int row, string numero) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT skudesc FROM sku WHERE sku = '" + numero + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                conn.Close();
                foreach (DataRow desc in tb.Rows) {
                    dataGridView1[2, row].Value = desc[0];
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            if (dataGridView1.Rows.Count == 0) {
                MessageBox.Show("No has seleccionado nada aun");
            } else {
                if (textBox2.Text == "") {
                    MessageBox.Show("Debes llenar el campo de Motivo.");
                } else {
                    if (double.Parse(label28.Text) >= 0) {
                        createreq();
                        Close();
                    } else {
                        MessageBox.Show("Presupuesto agotado, revise con Finanzas");
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            AddMaterial am = new AddMaterial();
            am.ShowInTaskbar = false;
            am.usuario = usuario;
            am.ShowDialog();
            if (am.DialogResult == DialogResult.OK) {
                foreach (DataRow dr in am.dt.Rows) {
                    dataGridView1.Rows.Add(dr[0], dr[1], dr[2], dr[3], dr[4], dr[5], dr[6], dr[7], dr[8], dr[9], dr[10]);
                }
                actualizarconsumo();
            }
        }
        private void CreateRequi_Load(object sender, EventArgs e) {
            textBox5.Text = usuario;
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            getcuentas();
            get_locaciones();
            dataGridView1.Columns.Add("fscategoria", "Categoria");
            dataGridView1.Columns.Add("fscodigo", "Codigo");
            dataGridView1.Columns.Add("fsdescripcion", "Descripcion");
            dataGridView1.Columns.Add("fscantidad", "Cantidad");
            dataGridView1.Columns.Add("fsunimedida", "Unidad de Medida");
            dataGridView1.Columns.Add("fscostoestimado", "Costo Estimado Total");
            dataGridView1.Columns.Add("fsmoneda", "Moneda");
            dataGridView1.Columns.Add("fsanexos", "Anexos");
            dataGridView1.Columns.Add("fsarchivos", "Archivos");
            dataGridView1.Columns.Add("fsabsoluto", "Valor Absoluto en Dlls");
            dataGridView1.Columns.Add("fscomentario", "Comentario");
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            getbag();
            casignado = 0;
            actualizarconsumo();
        }
        
        private void sendmail() {
            string gerentemail = getgerente();
            MailMessage mail = new MailMessage("aramis@posey.com", gerentemail);
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
        private void textBox2_TextChanged(object sender, EventArgs e) {
            if (textBox2.Text.Length >= 399) {
                MessageBox.Show("Motivo demasiado extenso, comuniquese con el administrador");
            }
            
        }

        private void label6_Click(object sender, EventArgs e) {
            //sendmail2();
        }
        private void sendmail2() {
            string gerentemail = getgerente();
            MailMessage mail = new MailMessage("aramis@posey.com", "amartinez@posey.com");
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Nueva Requisicion de Compra para Aprobar";
            mail.Body = "Se ha generado una nueva requisicion de Compra, ingrese al Sistema ARAMIS por favor para continuar.";
            try {
                client.Send(mail);
                MessageBox.Show("Correo Enviado Compras");
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
            
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
            if (Regex.IsMatch(e.KeyChar.ToString(), @"[^a-z^A-Z^0-9^ ^+^\-^\/^\b^\*^\(^\)]")) {
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }
    }
}
