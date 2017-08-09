using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CreateAjuste : Form {
        public CreateAjuste() {
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
        public string idbucket          { get; set; }
        public string costoajuste       { get; set; }

        List<string> deptos = new List<string>();
        List<string> files = new List<string>();
        List<string> codigos = new List<string>();
        List<string> unidadesdemedida = new List<string>();

        double casignado = 0;
        double cbalance = 0;
        double cajustado = 0;
        double cgasto = 0;
        double cbudget = 0;

        DataTable tablarazones;

        public void createajuste() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                sqlquery += "DECLARE @num int;\n";
                sqlquery += "INSERT INTO tbajustes (fscreatedate, fscreatedby, fsaccountnum, " + 
                    "fsajuste, fsrazonheadid, fsimporteneto, fsstatus, fsbucketid, fslugar) VALUES (" +
                    "GETDATE()" + ",'" + 
                    usuario + "','" +
                    textBox1.Text + "','" +
                    textBox2.Text + "','" +
                    textBox2.Text + "','" +
                    costoajuste + "','" +
                    "Ajuste Creado" + "','" + 
                    idbucket + "','" +
                    comboBox1.SelectedItem.ToString() + "'); SELECT @num = SCOPE_IDENTITY();\n";
                double tres = double.Parse(label25.Text);
                double cuatro = double.Parse(label26.Text);
                double cinco = double.Parse(label24.Text);
                double seis = tres - cuatro - cinco;
                sqlquery += "UPDATE buckets SET ajustado = '" 
                    + tres.ToString() + "', gasto = '" + cuatro.ToString() + "', asignado = '" 
                    + cinco.ToString() + "', balance = '" + seis.ToString() + "' WHERE id_bucket='" + idbucket + "';\n";
                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    if (!row.IsNewRow) {
                        try {
                            foreach (DataGridViewCell cell in row.Cells) {
                                if (cell.Value.ToString().Length >= 100) {
                                    MessageBox.Show("Se presento un problema con las lineas de ajuste.\nRevise por favor que no excedan 100 caracteres");
                                    return;
                                }
                            }
                            sqlquery += "INSERT INTO tbajusteslineas (fsajusteid, sku,"
                                + "fscostounitario, fscostoext, fsrazondetail, "
                                + "fsresponsable, fsbucketid, fsstatus, fslocacion, fscantidadensistema, fscantidadfisica, "
                                + "fsdiferencia, fsconfiabilidad, fsfecha, fsconto, fsvalorencontra, fsvalorafavor, "
                                + "fsvalorabsoluto, fsvalorinventario) VALUES ("
                                + "@num" + ",'"
                                + row.Cells["fssku"].Value.ToString() + "','"
                                + row.Cells["fscostounitario"].Value.ToString() + "','"
                                + row.Cells["fscostoextendido"].Value.ToString() + "','"
                                + row.Cells["fsrazon"].Value.ToString() + "','"
                                + row.Cells["fsresponsable"].Value.ToString() + "','"
                                + idbucket
                                + "','Ajuste Creado','"
                                + row.Cells["fslocacion"].Value.ToString() + "','"
                                + row.Cells["fscantidadensistema"].Value.ToString() + "','"
                                + row.Cells["fscantidadfisica"].Value.ToString() + "','"
                                + row.Cells["fsdiferencia"].Value.ToString() + "','"
                                + row.Cells["fsconfiabilidad"].Value.ToString() + "','"
                                + row.Cells["fsfecha"].Value.ToString() + "','"
                                + row.Cells["fsconto"].Value.ToString() + "','"
                                + row.Cells["fsvalorencontra"].Value.ToString() + "','"
                                + row.Cells["fsvalorafavor"].Value.ToString() + "','"
                                + row.Cells["fsvalorabsoluto"].Value.ToString() + "','"
                                + row.Cells["fsvalorinventario"].Value.ToString() + "'"
                                + ");\n";
                        } catch (Exception es) {
                            MessageBox.Show(es.ToString());
                        }
                    }
                }
                List<string> archivos = new List<string>();
                archivos = textBox3.Text.Split('|').ToList();
                foreach (string onefile in archivos) {
                    if (onefile == "") {
                    } else {
                        sqlquery += "INSERT INTO tbajustesfiles (fsfilename, fsdate, fsajusteid) VALUES "
                            + "('" + System.IO.Path.GetFileName(onefile) + "'," + "GETDATE()" + "," +
                        "@num" + ");\n";
                    }
                }
                sqlquery += "SELECT @num;\n";
                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                
                try {
                    int id = int.Parse(ejecucion.ExecuteScalar().ToString());
                    conn.Close();
                    textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    MessageBox.Show("Solicitud de aprobacion de ajuste creado, su numero es: \n" + id.ToString(), "Listo");
                    sendmail();
                    foreach (string onefile in archivos) {
                        if (onefile != "") {
                            if (!System.IO.Directory.Exists(@"\\mexfs01\TJTemp\Opardo\FOLIOS\ajustes\" + id.ToString())) {
                                System.IO.Directory.CreateDirectory(@"\\mexfs01\TJTemp\Opardo\FOLIOS\ajustes\" + id.ToString());
                            }
                            System.IO.File.Copy(onefile, @"\\mexfs01\TJTemp\Opardo\FOLIOS\ajustes\" + id.ToString() +
                                @"\" + System.IO.Path.GetFileName(onefile), true);
                            MessageBox.Show("Archivos anexados...");
                        }
                    }
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void sendmail() {
            //string creadormail = getcorreo();
            string vpmail = "lpalomares@posey.com";
            if (Program.stringconnection == "Data Source=MEXATLAS\\REQUI;Database=testing;User ID=sa;Password=R3qu1@16;") {
                vpmail = "lpalomares@posey.com";
            }
            MailMessage mail = new MailMessage("aramis@posey.com", vpmail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Preaprobar ajuste";
            mail.Body = "Se ha registrado una nueva solicitud de aprobacion de ajuste, ingrese al Sistema ARAMIS por favor para continuar.";
            try {
                client.Send(mail);
                MessageBox.Show("Correo enviado");
            } catch (Exception) {
                MessageBox.Show("Se presento un problema al enviar correo");
            }
        }
        private bool allrowshavecost() {
            double acumulado = 0;
            try {
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    acumulado += double.Parse(dr.Cells["fscostoextendido"].Value.ToString());
                }
                costoajuste = acumulado.ToString();
                return true;
            } catch (Exception) {
                return false;
            }
        }
        public void updatebucket() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlcuentas = "SELECT * FROM buckets WHERE id_bucket = '" + idbucket + "'";
                SqlDataAdapter adaptercuentas = new SqlDataAdapter(sqlcuentas, conn);
                double tres = double.Parse(label25.Text);
                double cuatro = double.Parse(label26.Text);
                double cinco = double.Parse(label24.Text);
                double seis = tres - cuatro - cinco;
                string sqlquery = "UPDATE buckets SET ajustado = '" +
                   tres.ToString() + "', gasto = '" + cuatro.ToString() + "', asignado = '" +
                   cinco.ToString() + "', balance = '" + seis.ToString() + "' WHERE id_bucket='" + idbucket + "'";
                string connectionstring2 = Program.stringconnection;
                SqlConnection conn2 = new SqlConnection(connectionstring2);
                conn2.Open();
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn2;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                conn2.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void setlineasparaajustar(string id) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    if (!row.IsNewRow) {
                        try {
                            string sqlquery = "INSERT INTO tbajusteslineas (fsajusteid, sku,"
                                + "fscostounitario, fscostoext, fsrazondetail, "
                                + "fsresponsable, fsbucketid, fsstatus, fslocacion, fscantidadensistema, fscantidadfisica, "
                                + "fsdiferencia, fsconfiabilidad, fsfecha, fsconto, fsvalorencontra, fsvalorafavor, "
                                + "fsvalorabsoluto, fsvalorinventario) VALUES ('" 
                                + id + "','" 
                                + row.Cells["fssku"].Value.ToString() + "','" 
                                + row.Cells["fscostounitario"].Value.ToString() + "','" 
                                + row.Cells["fscostoextendido"].Value.ToString() + "','"
                                + row.Cells["fsrazon"].Value.ToString() + "','" 
                                + row.Cells["fsresponsable"].Value.ToString() + "','" 
                                + idbucket 
                                + "','Ajuste Creado','"
                                + row.Cells["fslocacion"].Value.ToString() + "','"
                                + row.Cells["fscantidadensistema"].Value.ToString() + "','"
                                + row.Cells["fscantidadfisica"].Value.ToString() + "','"
                                + row.Cells["fsdiferencia"].Value.ToString() + "','"
                                + row.Cells["fsconfiabilidad"].Value.ToString() + "','"
                                + row.Cells["fsfecha"].Value.ToString() + "','"
                                + row.Cells["fsconto"].Value.ToString() + "','"
                                + row.Cells["fsvalorencontra"].Value.ToString() + "','"
                                + row.Cells["fsvalorafavor"].Value.ToString() + "','"
                                + row.Cells["fsvalorabsoluto"].Value.ToString() + "','"
                                + row.Cells["fsvalorinventario"].Value.ToString() + "'"
                                + ")";
                            SqlCommand ejecucion = new SqlCommand();
                            ejecucion.Connection = conn;
                            ejecucion.CommandType = CommandType.Text;
                            ejecucion.CommandText = sqlquery;
                            ejecucion.ExecuteNonQuery();
                        } catch (Exception es) {
                            MessageBox.Show(es.ToString());
                        }
                    }
                }
                conn.Close();
                set_uploadfiles(int.Parse(id));
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public void set_uploadfiles(int id) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                try {
                    List<string> archivos = new List<string>();
                    archivos = textBox3.Text.Split('|').ToList();
                    foreach (string onefile in archivos) {
                        if (onefile == "") {
                        } else {
                            string sqlquery = "INSERT INTO tbajustesfiles (fsfilename, fsdate, fsajusteid) VALUES ('" +
                            System.IO.Path.GetFileName(onefile) + "'," +
                            "GETDATE()" + ",'" +
                            id.ToString() + "')";
                            SqlCommand ejecucion = new SqlCommand();
                            ejecucion.Connection = conn;
                            ejecucion.CommandType = CommandType.Text;
                            ejecucion.CommandText = sqlquery;
                            ejecucion.ExecuteNonQuery();
                            if (!System.IO.Directory.Exists(@"\\mexfs01\TJTemp\Opardo\FOLIOS\ajustes\" + id.ToString())) {
                                System.IO.Directory.CreateDirectory(@"\\mexfs01\TJTemp\Opardo\FOLIOS\ajustes\" + id.ToString());
                            }
                            System.IO.File.Copy(onefile, @"\\mexfs01\TJTemp\Opardo\FOLIOS\ajustes\" + id.ToString() +
                                @"\" + System.IO.Path.GetFileName(onefile), true);
                        }
                    }
                } catch (Exception) {
                }
                
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void filltablarazones() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT * FROM tbrazonesajuste";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                
                tablarazones = new DataTable();
                adapter.Fill(tablarazones);
                
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public string getaccountid() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT id FROM accounts WHERE acctnumber = '" + textBox1.Text + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                string number = "";
                DataRow da = tb.Rows[0];
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
                string query1 = "SELECT balance, budget, ajustado, gasto, asignado, id_bucket FROM buckets WHERE id_cuenta = '" + 
                    getaccountid() + "' AND periodo = '" + DateTime.Now.ToString("yyyy-MM") + "'";
                string sqlquery2 = "select Deptos.name from accounts left join deptos on accounts.depto = Deptos.id where acctnumber = '" + textBox1.Text + "'";
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter(query1, conn);
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery2, conn);
                DataTable datatabletb = new DataTable();
                DataTable datatable2 = new DataTable();
                adapter1.Fill(datatabletb);
                adapter2.Fill(datatable2);
                try {
                    DataRow dr = datatable2.Rows[0];
                    label9.Text = "Departamento:        " + dr[0].ToString();
                    DataRow da = datatabletb.Rows[0];
                    label23.Text = da[0].ToString();
                    label22.Text = da[1].ToString();
                    label21.Text = da[3].ToString();
                    label20.Text = da[2].ToString();
                    label19.Text = da[4].ToString();
                    actualizarconsumo();
                    idbucket = da[5].ToString();
                } catch (Exception exe) {
                    MessageBox.Show(exe.ToString());
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void actualizarconsumo() {
            try {
                double totalmovimiento = 0;
                cbalance = double.Parse(label23.Text);
                cbudget = double.Parse(label22.Text);
                cgasto = double.Parse(label21.Text);
                cajustado = double.Parse(label20.Text);
                casignado = double.Parse(label19.Text);
                
                double tvabsoluto = 0;
                double tinventario = 0;
                
                if (dataGridView1.RowCount >= 1) {
                    foreach (DataGridViewRow dr in dataGridView1.Rows) {
                        if (!dr.IsNewRow) {
                            totalmovimiento = totalmovimiento + double.Parse(dr.Cells["fsvalorinventario"].Value.ToString());
                            casignado = casignado + double.Parse(dr.Cells["fscostoextendido"].Value.ToString());
                            cbalance = cajustado - cgasto - casignado;
                            try {
                                tvabsoluto += double.Parse(dr.Cells["fsvalorabsoluto"].Value.ToString());
                                tinventario += double.Parse(dr.Cells["fsvalorinventario"].Value.ToString());
                            } catch (Exception ex) {
                                MessageBox.Show(ex.ToString());
                            }
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
                label10.Text = "Discrepancia: " + Math.Abs(Math.Round(tvabsoluto / totalmovimiento, 4) * 100) + "%";
                label11.Text = "Veracidad: " + Math.Round((1 - Math.Abs(tvabsoluto / totalmovimiento)) * 100, 2) + "%";
                label8.Text = "Costo de Ajuste: $ " + getneto();
            } catch (Exception) {
                label28.Text = "";
                label27.Text = "";
                label26.Text = "";
                label25.Text = "";
                label24.Text = "";
            }
        }
        private string getneto() {
            try {
                double negativos = 0;
                double positivos = 0;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    negativos += double.Parse(dr.Cells[10].Value.ToString());
                    positivos += double.Parse(dr.Cells[11].Value.ToString());
                }
                double neto = positivos - Math.Abs(negativos);

                return neto.ToString();
            } catch (Exception) {
                return "Error, no neteable";
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            filltablarazones();
            if (textBox2.Text.Length >= 200 || textBox2.Text.Length < 10) {
                MessageBox.Show("Razon demasiado extensa o muy corta");
            } else {
                if (dataGridView1.Rows.Count == 0) {
                    MessageBox.Show("No has agregado ajustes");
                } else {
                    if (textBox2.Text == "") {
                        MessageBox.Show("Falta llenar el campo de razon.");
                    } else {
                        if (allrowshavecost() && double.Parse(label28.Text) >= 0) {
                            createajuste(); 
                        } else {
                            MessageBox.Show("No todas las lineas tienen costo o balance no alcanza");
                        }
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void button4_Click(object sender, EventArgs e) {
            try {
                System.IO.File.Copy(@"\\mexfs01\TJTemp\Opardo\FOLIOS\ARAMIS\template.xlsx",
                    System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\templater.xlsx", true);
                System.Diagnostics.Process.Start(System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\templater.xlsx");
            } catch (Exception) {
                MessageBox.Show("Error al abrir archivo");
            }
        }
        private void button5_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                List<string> files = new List<string>();
                files = dialog.FileNames.ToList();
                textBox3.Text = "";
                foreach (string onefile in files) {
                    textBox3.Text += onefile + "|";
                }
            }
        }

        private void CreateRequi_Load(object sender, EventArgs e) {
            textBox5.Text = usuario;
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            get_locaciones();
            getbag();
            
            dataGridView1.Columns.Add("fslocacion", "Locacion");
            dataGridView1.Columns.Add("fssku", "SKU");
            dataGridView1.Columns.Add("fscantidadensistema", "Cantidad en Sistema");
            dataGridView1.Columns.Add("fscantidadfisica", "Cantidad Fisica");
            dataGridView1.Columns.Add("fsdiferencia", "Diferencia");
            dataGridView1.Columns.Add("fsconfiabilidad", "Porcentaje de Confiabilidad");
            dataGridView1.Columns.Add("fscostounitario", "Costo Unitario");
            dataGridView1.Columns.Add("fscostoextendido", "Costo Extendido");
            dataGridView1.Columns.Add("fsfecha", "Fecha");
            dataGridView1.Columns.Add("fsconto", "CONTO");
            dataGridView1.Columns.Add("fsvalorencontra", "Valor en Contra");
            dataGridView1.Columns.Add("fsvalorafavor", "Valor a Favor");
            dataGridView1.Columns.Add("fsvalorabsoluto", "Valor Absoluto");
            dataGridView1.Columns.Add("fsvalorinventario", "Valor del Inventario");
            dataGridView1.Columns.Add("fsrazon", "Razon");
            dataGridView1.Columns.Add("fsresponsable", "Responsable");
            dataGridView1.MouseClick += DataGridView1_MouseClick;
            label10.Text = "Discrepancias: ";
            label11.Text = "Veracidad: ";
        }

        private void DataGridView1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                ContextMenuStrip m = new ContextMenuStrip();
                m.Items.Add("Paste");
                m.ItemClicked += M_ItemClicked;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }
        private void M_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            ToolStripItem btn = e.ClickedItem;
            if (btn.Text == "Paste") {
                try {
                    PasteClipboard();
                } catch (Exception) { MessageBox.Show("Fatal Error"); }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            getbag();
            casignado = 0;
            actualizarconsumo();
        }
        private void PasteClipboard() {
            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.Text)) {
                if (dataGridView1.RowCount > 0) {
                    dataGridView1.Rows.Clear();
                }
                string[] pastedRows = Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
                int j = 0;
                
                foreach (string pastedRow in pastedRows) {
                    string[] pastedRowCells = pastedRow.Split(new char[] { '\t' });
                    dataGridView1.Rows.Add();
                    int myRowIndex = dataGridView1.Rows.Count - 1;
                    using (DataGridViewRow myDataGridViewRow = dataGridView1.Rows[j]) {
                        for (int i = 0;i < pastedRowCells.Length;i++) {
                            string pegar = pastedRowCells[i].Trim(new Char[] { ' ', '$', '(', ')', ',', '%' });
                            string pega = pegar.Replace(",", "");
                            string value = pega;
                            
                            if (pastedRowCells[i].Contains("(")) {
                                pegar = "-" + pegar;
                            } else {
                                if (pegar == "-") {
                                    pegar = "0";
                                }
                            }
                            myDataGridViewRow.Cells[i].Value = pegar;
                        }
                    }
                    j++;
                }
            }
            actualizarconsumo();
        }
        private bool validarlineas() {
            try {
                if (textBox2.Text.Length > 200 || textBox2.Text == "" || textBox2.Text == null) {
                    return false;
                }
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    if (dr.Cells["fslocacion"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fssku"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fscantidadensistema"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fscantidadfisica"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fsdiferencia"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fsconfiabilidad"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fscostounitario"].Value.ToString() == "") { return false; }
                    if (double.Parse(dr.Cells["fscostounitario"].Value.ToString()) < 0) { return false; }
                    if (dr.Cells["fscostoextendido"].Value.ToString() == "") { return false; }
                    if (double.Parse(dr.Cells["fscostoextendido"].Value.ToString()) < 0) { return false; }
                    if (dr.Cells["fsfecha"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fsconto"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fsvalorencontra"].Value.ToString() == "") { return false; }
                    if (double.Parse(dr.Cells["fsvalorencontra"].Value.ToString()) > 0) { return false; }
                    if (dr.Cells["fsvalorafavor"].Value.ToString() == "") { return false; }
                    if (double.Parse(dr.Cells["fsvalorafavor"].Value.ToString()) < 0) { return false; }
                    if (dr.Cells["fsvalorabsoluto"].Value.ToString() == "") { return false; }
                    if (double.Parse(dr.Cells["fsvalorabsoluto"].Value.ToString()) < 0) { return false; }
                    if (dr.Cells["fsvalorinventario"].Value.ToString() == "") { return false; }
                    if (dr.Cells["fsrazon"].Value.ToString() == "") { return false; }
                    if (!esrazonvalida(dr.Cells["fsrazon"].Value.ToString())) { return false; }
                    if (dr.Cells["fsresponsable"].Value.ToString() == "") { return false; }
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }
        private bool esrazonvalida(string razon) {
            foreach (DataRow dr in tablarazones.Rows) {
                if (razon == dr[1].ToString()) {
                    return true;
                }
            }
            return false;
        }
    }
}
