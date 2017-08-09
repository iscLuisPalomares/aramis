using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CreateAllPOrder : Form {
        public CreateAllPOrder() {
            InitializeComponent();
        }
        public string usuario       { get; set; }
        public string user_id       { get; set; }
        public string vendorname    { get; set; }
        public string cotizacion    { get; set; }
        public string totalcost     { get; set; }
        public string vendorid      { get; set; }
        public string impuestos     { get; set; }

        private string getuserdepto() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "select id, name as 'Nombre' from deptos where id in (select depto from users where users.username in (select createdby from requisiciones where id_req in (";
                foreach (DataGridViewRow linea in dataGridView1.Rows) {
                    sqlquery += linea.Cells["Req"].Value.ToString() + ", ";
                }
                sqlquery = sqlquery.Remove(sqlquery.Length - 2);
                sqlquery += ")))";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                string regresar = "";
                foreach (DataRow ddd in table.Rows) {
                    regresar += ddd["Nombre"].ToString() + ", ";
                }
                regresar = regresar.Remove(regresar.Length - 2);
                conn.Close();
                return regresar;
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
                return "";
            }
        }
        private void actualizarconsumo() {
            try {
                foreach (DataGridViewRow dr1 in dataGridView1.Rows) {
                    string buckid = dr1.Cells["Bucket"].Value.ToString();
                    double sumartotal = double.Parse(dr1.Cells["Total Dlls"].Value.ToString());
                    foreach (DataGridViewRow ddr2 in dataGridView2.Rows) {
                        if (ddr2.Cells[0].Value.ToString() == buckid) {
                            double bucketasignado = double.Parse(ddr2.Cells[5].Value.ToString());
                            bucketasignado = bucketasignado - sumartotal;
                            double bucketajustado = double.Parse(ddr2.Cells[3].Value.ToString());
                            double bucketgasto = double.Parse(ddr2.Cells[4].Value.ToString());
                            double bucketbalance = double.Parse(ddr2.Cells[5].Value.ToString());
                            bucketbalance = bucketajustado - bucketgasto - bucketasignado;
                            foreach (DataGridViewRow dddr3 in dataGridView3.Rows) {
                                if (dddr3.Cells[0].Value.ToString() == buckid) {
                                    dddr3.Cells[5].Value = bucketasignado;
                                    dddr3.Cells[6].Value = bucketbalance;
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }

        }
        private string getaccountid(string accountnum) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT id FROM accounts where acctnumber = '" + accountnum + "'";
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
                MessageBox.Show(ex.ToString());
                Close();
                return "";
            }
        }
        private void setnewpurchaseorder() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                sqlquery += "DECLARE @num int;\n";
                sqlquery += "declare @identvalue int; \n"
                    + "select @identvalue = IDENT_CURRENT('tblPurchaseOrders'); \n"
                    + "declare @lastcolumnvalue int; \n"
                    + "select @lastcolumnvalue = max(fsid) from tblPurchaseOrders; \n"
                    + "if @identvalue > @lastcolumnvalue \n"
                    + "\tdbcc checkident ('tblPurchaseOrders', reseed, @lastcolumnvalue); \n";
                sqlquery += "INSERT INTO tblpurchaseorders (fscreatedate, fscreatedby, fsbuyer, fscomments, " +
                    "fsvendor, fsterms, fsdaterequired, fsobservaciones"
                    + ", fsstatus, fsdepto, fstotalcost, fscotizacionid, fsimpuestos, fsstatuscomment) "
                    + "VALUES ('" +
                    textBox4.Text + "','" +
                    user_id + "','" +
                    user_id + "','" +
                    textBox8.Text + "','" +
                    vendorname + "','" +
                    textBox9.Text + "','" +
                    dateTimePicker1.Value.ToString() + "','" +
                    textBox11.Text + "','" +
                    "PO Aprobado" + "','" +
                    textBox7.Text + "','" +
                    totalcost + "','" + 
                    cotizacion + "','" +
                    impuestos + "','"
                    + textBox2.Text +"'); SELECT @num = SCOPE_IDENTITY();\n";
                sqlquery += "UPDATE tbcotizaciones SET fsstatus = 'PO Aprobado' WHERE fsid = '" + cotizacion + "';\n";
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    sqlquery += "UPDATE materialrequerido SET fsstatus = 'PO creado', " +
                        "fspurchaseorder = @num WHERE fsid = " + dr.Cells["ID"].Value.ToString() + ";\n";
                }
                List<string> archivos = new List<string>();
                archivos = textBox6.Text.ToString().Split('|').ToList();
                foreach (string onefile in archivos) {
                    if (onefile != "") {
                        sqlquery += "INSERT INTO po_files (fsname, fsdate, fsuserid, fscotid) VALUES ('" +
                        System.IO.Path.GetFileName(onefile) + "'," +
                        "GETDATE()" + ",'" +
                        user_id + "',@num);\n";
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
                    try {
                        foreach (string onefile in archivos) {
                            if (onefile != "") {
                                if (!System.IO.Directory.Exists(@"\\mexfs01\TJTemp\Opardo\FOLIOS\porders\" + usuario + @"\" + id.ToString())) {
                                    System.IO.Directory.CreateDirectory(@"\\mexfs01\TJTemp\Opardo\FOLIOS\porders\" + usuario + @"\" + id.ToString());
                                }
                                System.IO.File.Copy(onefile, @"\\mexfs01\TJTemp\Opardo\FOLIOS\porders\" + usuario + @"\" + id.ToString() + @"\" + System.IO.Path.GetFileName(onefile), true);
                            }
                        }
                    } catch (Exception) {
                        MessageBox.Show("No se copiaron los archivos");
                    }
                    textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    MessageBox.Show("Orden de compra creada, su numero de PO es: \n" + id.ToString(), "Listo");
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void setnewbag() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                
                string sqlquery = "update "
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
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
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
                string sqlquery = "SELECT * FROM buckets WHERE id_cuenta IN (" +
                    "SELECT id FROM accounts WHERE acctnumber IN (" +
                    getreqnums() + "))" + " AND periodo = '" + DateTime.Now.ToString("yyyy-MM") + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                DataTable table3 = new DataTable();
                adapter.Fill(table);
                adapter.Fill(table3);
                dataGridView2.DataSource = table;
                dataGridView3.DataSource = table3;
                conn.Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT * FROM requisiciones WHERE id_req IN (" +
                    "SELECT fsrequisicion FROM materialrequerido WHERE fsid IN (" +
                        "SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = '" + cotizacion + "'" +
                    ")" +
                ")";
                string sqlquery4 = "select fsid as 'ID', fscodigo as 'Codigo', fsdesc as 'Descripcion', "
                    + "fscantidad as 'Cantidad', fsunimedida as 'U/M', "
                    + "fstotalcost as 'Costo Total', divisacot as 'Moneda', "
                    + "absolutdllscot as 'Total Dlls', bucketid as 'Bucket', fscuenta as 'Cuenta', fsrequisicion as 'Req'"
                    + "from materialrequerido  "
                    + "where fsid in ("
                    + "select fsidmaterialrequerido from tbcotmaterialrequerido where fsidcotizacion = '" + cotizacion + "')";
                conn.Open();
                
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter4 = new SqlDataAdapter(sqlquery4, conn);
                DataTable tb = new DataTable();
                DataTable tb4 = new DataTable();
                adapter.Fill(tb);
                adapter4.Fill(tb4);
                dataGridView1.DataSource = tb4;
                foreach (DataGridViewColumn dc in dataGridView1.Columns) {
                    if (dc.Name == "ID") {
                        dc.Width = 70;
                    } else {
                        dc.Width = 100;
                    }
                    if (dc.Name == "Bucket" || dc.Name == "Cuenta") {
                        dc.Visible = false;
                    }
                }
                
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        private void getlineas() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM materialrequerido WHERE fsid IN (" +
                    "SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = '" + cotizacion + "')";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        private string getvendorname(string vendor) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT suppname FROM asl where id = '" + vendor + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                string name = "";
                DataRow da = tb.Rows[0];
                name = da[0].ToString();
                conn.Close();
                return name;
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
                return "";
            }
        }
        private void CreateRequi_Load(object sender, EventArgs e) {
            getdata();
            textBox4.Text = DateTime.Now.ToString();
            textBox3.Text = usuario;
            textBox5.Text = usuario;
            textBox1.Text = getvendorname(vendorname);
            textBox7.Text = getuserdepto();
            textBox11.Text = "Nota: derivado de las disposiciones Fiscales ante la ley le solicitamos nos proporcione la carta de Opinion Positiva de Cumplimiento en forma mensual";
            setantes();
            actualizarconsumo();
        }
        private void button1_Click(object sender, EventArgs e) {
            setnewpurchaseorder();
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                List<string> files = new List<string>();
                files = dialog.FileNames.ToList();
                textBox6.Text = "";
                foreach (string onefile in files) {
                    textBox6.Text += onefile + "|";
                }
            }
        }
        private void button4_Click(object sender, EventArgs e) {
            ChangeVendor cv = new ChangeVendor();
            cv.ShowDialog();
            if (cv.DialogResult == DialogResult.OK) {
                vendorname = cv.vendorname;
                textBox1.Text = getvendorname(vendorname);
                getdata();
            }
        }
        private void Cv_FormClosed(object sender, FormClosedEventArgs e) {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e) {
            
        }
    }
}
