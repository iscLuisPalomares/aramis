using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CreateCotizacion : Form {
        public CreateCotizacion() {
            InitializeComponent();
        }
        public string usuario       { get; set; }
        public string tipo          { get; set; }
        public string user_id       { get; set; }
        public string user_depto    { get; set; }
        public string deptoid       { get; set; }
        public string tipodecambio  { get; set; }
        public string divisa        { get; set; }
        public DataTable tabla      { get; set; }

        public bool comboboxesready = false;
        bool casilla1 = false;
        bool casilla2 = false;
        bool casilla3 = false;

        private double gettotal() {
            double total = 0;
            foreach (DataGridViewRow dr in dataGridView1.Rows) {
                total += double.Parse(dr.Cells[6].Value.ToString());
            }
            MessageBox.Show(total.ToString());
            return Math.Round(total, 2);
        }
        
        private void setnewcotizacion1winner() {
            divisa = cbdivisa1.SelectedItem.ToString();
            if (subtotal1.Enabled == false) {
                MessageBox.Show("no procede la 1");
                return;
            }
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                int num = 0;
                string sqlquery = "";
                string updatematerial = "";

                double dllscot = double.Parse(subtotal1.Text);
                if (cbdivisa1.SelectedItem.ToString() == "MXP") {
                    dllscot = dllscot / double.Parse(tipodecambio);
                }
                int unique = 0;
                if (cbunico.Checked) { unique = 1; } else { unique = 0; }

                updatematerial = "UPDATE materialrequerido SET divisacot = '" +
                    cbdivisa1.SelectedItem.ToString() +
                    "', absolutdllscot = " + dllscot + " WHERE fsid IN " +
                    "(SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = ";
                sqlquery = "INSERT INTO tbcotizaciones (createdate, createdby, fscostototal, fssupplier, fsganadora, " +
                    "fscomentario, fsimpuestos, fsdivisa, fsstatus, fsisuniquevendor) VALUES ('" +
                    DateTime.Now.ToString() + "','" +
                    user_id + "','" +
                    subtotal1.Text + "','" +
                    (cb1.SelectedItem as ComboBoxVendors).fsid + "','" +
                    "1','" +
                    comment1.Text + "','" +
                    cbiva1.SelectedItem.ToString() + "','" +
                    cbdivisa1.SelectedItem.ToString() + "', 'Cotizacion Creada', " + unique + "); SELECT SCOPE_IDENTITY()";

                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                num = int.Parse(ejecucion.ExecuteScalar().ToString());
                updatematerial += num + ")";
                ejecucion.CommandText = updatematerial;
                ejecucion.ExecuteNonQuery();

                setmaterialcotizado(num);
                set_uploadfiles(num, files1.Text.Split('|').ToList());
                conn.Close();
                sendmail();
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MessageBox.Show("Cotizacion creada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void setnewcotizacion2winner() {
            divisa = cbdivisa2.SelectedItem.ToString();
            if (subtotal2.Enabled == false) {
                MessageBox.Show("no procede la 2");
                return;
            }
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                int num = 0;
                string sqlquery = "";
                string updatematerial = "";

                double dllscot = double.Parse(subtotal2.Text);
                if (cbdivisa2.SelectedItem.ToString() == "MXP") {
                    dllscot = dllscot / double.Parse(tipodecambio);
                }
                int unique = 0;
                if (cbunico.Checked) { unique = 1; } else { unique = 0; }

                updatematerial = "UPDATE materialrequerido SET divisacot = '" +
                    cbdivisa2.SelectedItem.ToString() +
                    "', absolutdllscot = " + dllscot + " WHERE fsid IN " +
                    "(SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = ";
                sqlquery = "INSERT INTO tbcotizaciones (createdate, createdby, fscostototal, fssupplier, fsganadora, " +
                    "fscomentario, fsimpuestos, fsdivisa, fsstatus, fsisuniquevendor) VALUES ('" +
                    DateTime.Now.ToString() + "','" +
                    user_id + "','" +
                    subtotal2.Text + "','" +
                    (cb2.SelectedItem as ComboBoxVendors).fsid + "','" +
                    "1','" +
                    comment2.Text + "','" +
                    cbiva2.SelectedItem.ToString() + "','" +
                    cbdivisa2.SelectedItem.ToString() + "', 'Cotizacion Creada', " + unique + "); SELECT SCOPE_IDENTITY()";

                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                num = int.Parse(ejecucion.ExecuteScalar().ToString());
                updatematerial += num + ")";
                ejecucion.CommandText = updatematerial;
                ejecucion.ExecuteNonQuery();

                setmaterialcotizado(num);
                set_uploadfiles(num, files2.Text.Split('|').ToList());
                conn.Close();
                sendmail();
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MessageBox.Show("Cotizacion creada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void setnewcotizacion3winner() {
            divisa = cbdivisa3.SelectedItem.ToString();
            if (subtotal3.Enabled == false) {
                MessageBox.Show("no procede la 3");
                return;
            }
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                int num = 0;
                string sqlquery = "";
                string updatematerial = "";

                double dllscot = double.Parse(subtotal3.Text);
                if (cbdivisa3.SelectedItem.ToString() == "MXP") {
                    dllscot = dllscot / double.Parse(tipodecambio);
                }
                updatematerial = "UPDATE materialrequerido SET divisacot = '" +
                    cbdivisa3.SelectedItem.ToString() +
                    "', absolutdllscot = " + dllscot + " WHERE fsid IN " +
                    "(SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = ";
                int unique = 0;
                if (cbunico.Checked) { unique = 1; } else { unique = 0; }
                sqlquery = "INSERT INTO tbcotizaciones (createdate, createdby, fscostototal, fssupplier, fsganadora, " +
                    "fscomentario, fsimpuestos, fsdivisa, fsstatus, fsisuniquevendor) VALUES ('" +
                    DateTime.Now.ToString() + "','" +
                    user_id + "','" +
                    subtotal3.ToString() + "','" +
                    (cb3.SelectedItem as ComboBoxVendors).fsid + "','" +
                    "1','" +
                    comment3.Text + "','" +
                    cbiva3.SelectedItem.ToString() + "','" +
                    cbdivisa3.SelectedItem.ToString() + "', 'Cotizacion Creada', " + unique + "); SELECT SCOPE_IDENTITY()";

                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                num = int.Parse(ejecucion.ExecuteScalar().ToString());
                updatematerial += num + ")";
                ejecucion.CommandText = updatematerial;
                ejecucion.ExecuteNonQuery();
                setmaterialcotizado(num);
                set_uploadfiles(num, files3.Text.Split('|').ToList());
                conn.Close();
                sendmail();
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MessageBox.Show("Cotizacion creada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void setnewcotizacion1loser() {
            if (subtotal1.Text == "") {
                MessageBox.Show("no procede la 1");
                return;
            }
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                int num = 0;
                string sqlquery = "";
                int unique = 0;
                if (cbunico.Checked) { unique = 1; } else { unique = 0; }
                sqlquery = "INSERT INTO tbcotizaciones (createdate, createdby, fscostototal, fssupplier, fsganadora, " +
                    "fscomentario, fsimpuestos, fsdivisa, fsstatus, fsisuniquevendor) VALUES ('" +
                    DateTime.Now.ToString() + "','" +
                    user_id + "','" +
                    subtotal1.Text + "','" +
                    (cb1.SelectedItem as ComboBoxVendors).fsid + "','" +
                    "0','" +
                    comment1.Text + "','" +
                    cbiva1.SelectedItem.ToString() + "','" +
                    cbdivisa1.SelectedItem.ToString() + "', 'Cotizacion No Ganadora', " + unique + "); SELECT SCOPE_IDENTITY()";

                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                num = int.Parse(ejecucion.ExecuteScalar().ToString());
                conn.Close();
                setmaterialcotizado(num);
                set_uploadfiles(num, files1.Text.Split('|').ToList());
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MessageBox.Show("Cotizacion creada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void setnewcotizacion2loser() {
            if (subtotal2.Enabled == false) {
                MessageBox.Show("no procede la 2");
                return;
            }
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                int num = 0;
                string sqlquery = "";
                int unique = 0;
                if (cbunico.Checked) { unique = 1; } else { unique = 0; }
                sqlquery = "INSERT INTO tbcotizaciones (createdate, createdby, fscostototal, fssupplier, fsganadora, " +
                    "fscomentario, fsimpuestos, fsdivisa, fsstatus, fsisuniquevendor) VALUES ('" +
                    DateTime.Now.ToString() + "','" +
                    user_id + "','" +
                    subtotal2.Text + "','" +
                    (cb2.SelectedItem as ComboBoxVendors).fsid + "','" +
                    "0','" +
                    comment2.Text + "','" +
                    cbiva2.SelectedItem.ToString() + "','" +
                    cbdivisa2.SelectedItem.ToString() + "', 'Cotizacion No Ganadora', " + unique + "); SELECT SCOPE_IDENTITY()";

                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                num = int.Parse(ejecucion.ExecuteScalar().ToString());

                conn.Close();

                setmaterialcotizado(num);
                set_uploadfiles(num, files2.Text.Split('|').ToList());
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MessageBox.Show("Cotizacion creada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void setnewcotizacion3loser() {
            if (subtotal3.Enabled == false) {
                MessageBox.Show("no procede la 3");
                return;
            }
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                int num = 0;
                string sqlquery = "";
                int unique = 0;
                if (cbunico.Checked) { unique = 1; } else { unique = 0; }
                sqlquery = "INSERT INTO tbcotizaciones (createdate, createdby, fscostototal, fssupplier, fsganadora, " +
                    "fscomentario, fsimpuestos, fsdivisa, fsstatus, fsisuniquevendor) VALUES ('" +
                    DateTime.Now.ToString() + "','" 
                    + user_id + "','" 
                    + subtotal3.Text + "','" 
                    + (cb3.SelectedItem as ComboBoxVendors).fsid + "','" 
                    + "0','" 
                    + comment3.Text + "','" 
                    + cbiva3.SelectedItem.ToString() + "','" 
                    + cbdivisa3.SelectedItem.ToString() + "', 'Cotizacion No Ganadora', " + unique + "); SELECT SCOPE_IDENTITY()";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                num = int.Parse(ejecucion.ExecuteScalar().ToString());

                conn.Close();

                setmaterialcotizado(num);
                set_uploadfiles(num, files3.Text.Split('|').ToList());
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MessageBox.Show("Cotizacion creada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void setmaterialcotizado(int num) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                foreach (DataGridViewRow item in dataGridView1.Rows) {
                    string sqlquery = "INSERT INTO tbcotmaterialrequerido (fsidcotizacion, fsidmaterialrequerido) VALUES ('" +
                    num + "','" +
                    item.Cells[0].Value.ToString() + "');";
                    double unidadendlls = 0;
                    if (double.Parse(item.Cells["Cantidad"].Value.ToString()) > 0) {
                        unidadendlls = Math.Round(double.Parse(item.Cells["Total Dlls"].Value.ToString()) / double.Parse(item.Cells["Cantidad"].Value.ToString()), 2);
                    }
                    string sqlqueryupdate = "UPDATE materialrequerido SET "
                        + "fsstatus = 'Cotizado', "
                        + "fstotalcost = '" + item.Cells["Costo Extendido"].Value.ToString() + "', "
                        + "fscostounitario = '" + item.Cells["Costo Unitario"].Value.ToString() + "', "
                        + "fsdesc = '" + item.Cells["Descripcion"].Value.ToString() + "', "
                        + "divisacot = '" + divisa + "', "
                        + "absolutdllscot = '" + item.Cells["Total Dlls"].Value.ToString() + "', "
                        + "absdllscotuni = " + unidadendlls + " "
                        + "WHERE fsid = '" + item.Cells[0].Value.ToString() + "'";
                    SqlCommand ejecucion = new SqlCommand();
                    ejecucion.Connection = conn;
                    ejecucion.CommandType = CommandType.Text;
                    ejecucion.CommandText = sqlquery;
                    ejecucion.ExecuteNonQuery();
                    ejecucion.CommandText = sqlqueryupdate;
                    ejecucion.ExecuteNonQuery();
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
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
                            System.IO.Path.GetFileName(onefile) + "','" +
                            DateTime.Now.ToString() + "')";
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
        private void reviewoffers() {
            double costototal = 0;
            bool first = false;
            bool second = false;
            bool third = false;
            //obtener la suma total del costo de todas las lineas de material
            try {
                foreach (DataGridViewRow ddr in dataGridView1.Rows) {
                    try {
                        costototal += double.Parse(ddr.Cells["Total Dlls"].Value.ToString());
                    } 
                    catch (Exception) { MessageBox.Show("Problema con los costos del material (lineas), revise por favor"); return; }
                }
            } catch (Exception) { }
            //si la PRIMIER COTIZACION esta cerca del costo total, se enciende una bandera
            try {
                if (cbdivisa1.SelectedIndex == 0) {
                    if (costototal >= (double.Parse(subtotal1.Text) / double.Parse(tipodecambio)) - .1 &&
                        costototal <= (double.Parse(subtotal1.Text) / double.Parse(tipodecambio)) + .1) {
                        first = true;
                    }
                } else if (cbdivisa1.SelectedIndex == 1) {
                    if (costototal >= double.Parse(subtotal1.Text) - .1 &&
                        costototal <= double.Parse(subtotal1.Text) + .1) {
                        first = true;
                    }
                }
            } catch (Exception) { }
            //si la SEGUNDA COTIZACION esta cerca del costo total, se enciende una bandera
            try {
                if (cbdivisa2.SelectedIndex == 0) {
                    if (costototal >= (double.Parse(subtotal2.Text) / double.Parse(tipodecambio)) - .1 &&
                        costototal <= (double.Parse(subtotal2.Text) / double.Parse(tipodecambio)) + .1) {
                        second = true;
                    }
                } else if (cbdivisa2.SelectedIndex == 1) {
                    if (costototal >= double.Parse(subtotal2.Text) - .1 &&
                        costototal <= double.Parse(subtotal2.Text) + .1) {
                        second = true;
                    }
                }
            } catch (Exception) { }
            //si la TERCER COTIZACION esta cerca del costo total, se enciende una bandera
            try {
                if (cbdivisa3.SelectedIndex == 0) {
                    if (costototal >= (double.Parse(subtotal3.Text) / double.Parse(tipodecambio)) - .1 &&
                        costototal <= (double.Parse(subtotal3.Text) / double.Parse(tipodecambio)) + .1) {
                        third = true;
                    }
                } else if (cbdivisa3.SelectedIndex == 1) {
                    if (costototal >= double.Parse(subtotal3.Text) - .1 &&
                        costototal <= double.Parse(subtotal3.Text) + .1) {
                        third = true;
                    }
                }
            } catch (Exception) { }
            //si la primer cotizacion es la unica que coincide con el costo
            
            if (first && !second && !third) {
                if (casilla1) { setnewcotizacion1winner(); }
                if (casilla2) { setnewcotizacion2loser(); }
                if (casilla3) { setnewcotizacion3loser(); }
                Close();
            } else if (!first && second && !third) { //si la segunda cotizacion es la unica que coincide con el costo
                if (casilla1) { setnewcotizacion1loser(); }
                if (casilla2) { setnewcotizacion2winner(); }
                if (casilla3) { setnewcotizacion3loser(); }
                Close();
            } else if (!first && !second && third) { //si la tercer cotizacion es la unica que coincide con el costo
                if (casilla1) { setnewcotizacion1loser(); }
                if (casilla2) { setnewcotizacion2loser(); }
                if (casilla3) { setnewcotizacion3winner(); }
                Close();
            } else if (first && second && !third) {
                MessageBox.Show("Elegir entre primera y segunda");
                rb1.Visible = true;
                rb2.Visible = true;
            } else if (first && !second && third) {
                MessageBox.Show("Elegir entre primera y tercera");
                rb1.Visible = true;
                rb3.Visible = true;
            } else if (!first && second && third) {
                MessageBox.Show("Elegir entre segunda y tercera");
                rb2.Visible = true;
                rb3.Visible = true;
            } else if (first && second && third) {
                MessageBox.Show("Elegir entre las tres");
                rb1.Visible = true;
                rb2.Visible = true;
                rb3.Visible = true;
            } else {
                MessageBox.Show("Revise los costos totales por favor");
            }
        }

        private void getvendors() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM asl";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                cb1.Items.Clear();
                cb2.Items.Clear();
                cb3.Items.Clear();
                ComboBoxVendors cv;
                foreach (DataRow dr in table.Rows) {
                    cv = new ComboBoxVendors();
                    cv.fsid = dr[0].ToString();
                    cv.fssuppname = dr[1].ToString();
                    cv.fssuppdesc = dr[2].ToString();
                    cv.fssuppcity = dr[3].ToString();
                    cv.fsexportflag = dr[4].ToString();
                    cv.fscontactname = dr[5].ToString();
                    cv.fscontactemail = dr[6].ToString();
                    cv.fscontactphone = dr[7].ToString();
                    cv.fscountry = dr[10].ToString();
                    cv.fsflag = dr[11].ToString();

                    cb1.Items.Add(cv);
                    cb2.Items.Add(cv);
                    cb3.Items.Add(cv);
                }
                if (table.Rows[0][11].ToString() == "1") {
                    cbflag1.Checked = true;
                    cbflag2.Checked = true;
                    cbflag3.Checked = true;
                } else {
                    cbflag1.Checked = false;
                    cbflag2.Checked = false;
                    cbflag3.Checked = false;
                }
                cb1.SelectedIndex = 0;
                cb2.SelectedIndex = 0;
                cb3.SelectedIndex = 0;

                conn.Close();
                comboboxesready = true;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void getvendorflags() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT flag FROM asl WHERE id = '"
                    + (cb1.SelectedItem as ComboBoxVendors).fsid + "'";
                string sqlquery2 = "SELECT flag FROM asl WHERE id = '"
                    + (cb2.SelectedItem as ComboBoxVendors).fsid + "'";
                string sqlquery3 = "SELECT flag FROM asl WHERE id = '"
                    + (cb3.SelectedItem as ComboBoxVendors).fsid + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery2, conn);
                SqlDataAdapter adapter3 = new SqlDataAdapter(sqlquery3, conn);

                DataTable table = new DataTable();
                DataTable table2 = new DataTable();
                DataTable table3 = new DataTable();

                adapter.Fill(table);
                adapter2.Fill(table2);
                adapter3.Fill(table3);

                if (table.Rows[0][0].ToString() == "1") {
                    cbflag1.Checked = true;
                } else {
                    cbflag1.Checked = false;
                }
                if (table2.Rows[0][0].ToString() == "1") {
                    cbflag2.Checked = true;
                } else {
                    cbflag2.Checked = false;
                }
                if (table3.Rows[0][0].ToString() == "1") {
                    cbflag3.Checked = true;
                } else {
                    cbflag3.Checked = false;
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void updatebuckets() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    string sqlcuentas = "SELECT * FROM buckets WHERE id_bucket = '" + row.Cells[17].Value.ToString() + "'";
                    SqlDataAdapter adaptercuentas = new SqlDataAdapter(sqlcuentas, conn);
                    DataTable cuentastable = new DataTable();
                    adaptercuentas.Fill(cuentastable);
                    
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
                }
                conn.Close();

            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void calculartotalindlls() {
            try {
                double totaldelinea = 0;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    totaldelinea = double.Parse(dr.Cells["Costo Extendido"].Value.ToString());
                    if (radioButton5.Checked) {
                        dr.Cells["Total Dlls"].Value = (totaldelinea / double.Parse(tipodecambio)).ToString();
                    } else {
                        dr.Cells["Total Dlls"].Value = totaldelinea;
                    }
                }
            } catch (Exception) {

            }
        }
        private void abilitartextbox() {
            double costo = 0;
            foreach (DataGridViewRow ddr in dataGridView1.Rows) {
                try {
                    costo += double.Parse(ddr.Cells["Total Dlls"].Value.ToString());
                } catch (Exception) { }
            }
            if (costo < 500) {
                cbactivar1.Checked = true;
                cbactivar2.Checked = false;
                cbactivar3.Checked = false;
            } else if (costo >= 500 && costo < 1000) {
                cbactivar1.Checked = true;
                cbactivar2.Checked = true;
                cbactivar3.Checked = false;
            } else if (costo >= 1000) {
                cbactivar3.Checked = true;
                cbactivar1.Checked = true;
                cbactivar2.Checked = true;
            }
        }
        private void habilitar3ralinea() {
            //tercer linea
            subtotal3.Enabled = true;
            cb3.Enabled = true;
            comment3.Enabled = true;
            cbiva3.Enabled = true;
            files3.Enabled = true;
            archivos3.Enabled = true;
            cbdivisa3.Enabled = true;
        }
        private void habilitar2dalinea() {
            //segunda linea
            subtotal2.Enabled = true;
            cb2.Enabled = true;
            comment2.Enabled = true;
            cbiva2.Enabled = true;
            files2.Enabled = true;
            archivos2.Enabled = true;
            cbdivisa2.Enabled = true;
        }
        private void deshabilitar3ralinea() {
            //tercer linea
            subtotal3.Enabled = false;
            cb3.Enabled = false;
            comment3.Enabled = false;
            cbiva3.Enabled = false;
            files3.Enabled = false;
            archivos3.Enabled = false;
            cbdivisa3.Enabled = false;
        }
        private void deshabilitar2dalinea() {
            //segunda linea
            subtotal2.Enabled = false;
            cb2.Enabled = false;
            comment2.Enabled = false;
            cbiva2.Enabled = false;
            files2.Enabled = false;
            archivos2.Enabled = false;
            cbdivisa2.Enabled = false;
        }
        private void habilitar1erlinea() {
            //primer linea
            subtotal1.Enabled = true;
            cb1.Enabled = true;
            comment1.Enabled = true;
            cbiva1.Enabled = true;
            files1.Enabled = true;
            archivos1.Enabled = true;
            cbdivisa1.Enabled = true;
        }
        private void deshabilitar1erlinea() {
            //primer linea
            subtotal1.Enabled = false;
            cb1.Enabled = false;
            comment1.Enabled = false;
            cbiva1.Enabled = false;
            files1.Enabled = false;
            archivos1.Enabled = false;
            cbdivisa1.Enabled = false;
        }
        private void resetradio() {
            radioButton4.Checked = true;
            rb1.Checked = false;
            rb2.Checked = false;
            rb3.Checked = false;

            rb1.Visible = false;
            rb2.Visible = false;
            rb3.Visible = false;
        }
        private string getcorreoautorizado() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select correo from users where id = (select top 1 delegados.fsid as 'ID' "
                    + "from tbdelegados delegados join users usuarios on delegados.fsuserid = usuarios.id "
                    + "WHERE fsapproveajustes = 1 AND delegados.fsvencimiento > SYSDATETIME())";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                if (table.Rows.Count == 0) {
                    return "amartinez@posey.com";
                }
                return table.Rows[0][0].ToString();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                return "";
            }
        }
        private void sendmail() {
            string correo = getcorreoautorizado();
            if (Program.stringconnection == "Data Source=MEXATLAS\\REQUI;Database=testing;User ID=sa;Password=R3qu1@16;") {
                correo = "lpalomares@posey.com";
            }
            MailMessage mail = new MailMessage("aramis@posey.com", correo);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Nueva Cotizacion para Aprobar";
            mail.Body = "Se ha generado una nueva cotizacion, ingrese al Sistema ARAMIS por favor para continuar.";
            try {
                client.Send(mail);
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
        }
        public string gettipodecambio() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT TOP 1 valor FROM tbtipodecambio ORDER BY id DESC";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                conn.Close();
                string valor = "";
                foreach (DataRow desc in tb.Rows) {
                    valor = desc[0].ToString();
                }
                DataRow dr = tb.Rows[0];
                valor = dr[0].ToString();
                return valor;
            } catch (SqlException) {
                return "";
            }
        }
        
        private void CreateRequi_Load(object sender, EventArgs e) {
            tipodecambio = gettipodecambio();
            radioButton5.Checked = true;
            cbiva1.SelectedIndex = 0;
            cbiva2.SelectedIndex = 0;
            cbiva3.SelectedIndex = 0;
            cbdivisa1.SelectedIndex = 0;
            cbdivisa2.SelectedIndex = 0;
            cbdivisa3.SelectedIndex = 0;

            textBox4.Text = DateTime.Now.ToString();
            textBox5.Text = usuario;
            dataGridView1.DataSource = tabla;
            dataGridView1.Columns["ID"].ReadOnly = true;
            dataGridView1.Columns["ID"].Width = 60;
            dataGridView1.Columns["Cantidad"].ReadOnly = true;
            dataGridView1.Columns["Cantidad"].Width = 60;
            dataGridView1.Columns["U/M"].Width = 80;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;
            dataGridView1.Columns[5].ReadOnly = true;
            dataGridView1.Columns[6].ReadOnly = false;
            dataGridView1.Columns[7].ReadOnly = true;
            dataGridView1.Columns[8].ReadOnly = true;
            getvendors();
        }

        private void button1_Click(object sender, EventArgs e) {
            double costodetodoelmaterial = 0;
            double totaldecasillas = 0;
            casilla1 = cbactivar1.Checked;
            casilla2 = cbactivar2.Checked;
            casilla3 = cbactivar3.Checked;
            if (casilla1) {
                if (subtotal1.Text != "") { totaldecasillas += 1; } else { MessageBox.Show("Falta información"); return; }
            }
            if (casilla2) {
                if (subtotal2.Text != "") { totaldecasillas += 1; } else { MessageBox.Show("Falta información"); return; }
            }
            if (casilla3) {
                if (subtotal3.Text != "") { totaldecasillas += 1; } else { MessageBox.Show("Falta información"); return; }
            }
            foreach (DataGridViewRow ddr in dataGridView1.Rows) {
                try {
                    costodetodoelmaterial += double.Parse(ddr.Cells["Total Dlls"].Value.ToString());
                } catch (Exception) {
                    MessageBox.Show("Problema con los costos del material (lineas), revise por favor");
                    return;
                }
            }
            if (cbunico.Checked) {
                if (totaldecasillas >= 1 || costodetodoelmaterial >= 500) {

                } else {
                    MessageBox.Show("Desactive la casilla de Proveedor Unico");
                    return;
                }
            } else {
                if (costodetodoelmaterial < 500 && totaldecasillas >= 1) {

                } else if (costodetodoelmaterial >= 500 && costodetodoelmaterial < 1000 && totaldecasillas >= 2) {

                } else if (costodetodoelmaterial >= 1000 && totaldecasillas >= 3) {

                } else {
                    MessageBox.Show("Revisar Costos Unitarios");
                    return;
                }
            }
            if (rb1.Checked && cbactivar1.Checked) {
                if (casilla1) { setnewcotizacion1winner(); }
                if (casilla2) { setnewcotizacion2loser(); }
                if (casilla3) { setnewcotizacion3loser(); }
                Close();
            } else if (rb2.Checked && cbactivar2.Checked) {
                if (casilla1) { setnewcotizacion1loser(); }
                if (casilla2) { setnewcotizacion2winner(); }
                if (casilla3) { setnewcotizacion3loser(); }
                Close();
            } else if (rb3.Checked && cbactivar3.Checked) {
                if (casilla1) { setnewcotizacion1loser(); }
                if (casilla2) { setnewcotizacion2loser(); }
                if (casilla3) { setnewcotizacion3winner(); }
                Close();
            } else {
                reviewoffers();
            }
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
                files1.Text = "";
                foreach (string onefile in files) {
                    files1.Text += onefile + "|";
                }
            }
        }
        private void button4_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                List<string> files = new List<string>();
                files = dialog.FileNames.ToList();
                files2.Text = "";
                foreach (string onefile in files) {
                    files2.Text += onefile + "|";
                }
            }
        }
        private void button5_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                List<string> files = new List<string>();
                files = dialog.FileNames.ToList();
                files3.Text = "";
                foreach (string onefile in files) {
                    files3.Text += onefile + "|";
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            double costounitario = 0;
            double piezas = 0;
            double costolinea = 0;
            try {
                costounitario = double.Parse(dataGridView1["Costo Unitario", e.RowIndex].Value.ToString());
                piezas = double.Parse(dataGridView1["Cantidad", e.RowIndex].Value.ToString());
                costolinea = costounitario * piezas;
                abilitartextbox();
                dataGridView1["Costo Extendido", e.RowIndex].Value = costolinea.ToString();
                if (radioButton5.Checked) {
                    double dlls = costolinea / double.Parse(tipodecambio);
                    double rounded = Math.Round(dlls, 2);
                    dataGridView1["Total Dlls", e.RowIndex].Value = rounded.ToString();
                } else if (radioButton6.Checked) {
                    dataGridView1["Total Dlls", e.RowIndex].Value = costolinea.ToString();
                }
                double labeltotal = 0;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    try {
                        labeltotal += double.Parse(dr.Cells["Total Dlls"].Value.ToString());
                    } catch (Exception) { }
                }
                label17.Text = "Total Cotizado: $" + labeltotal.ToString() + " Dlls";
            } catch (Exception) { }
        }

        private void textBox8_TextChanged(object sender, EventArgs e) {
            resetradio();
        }
        private void textBox2_TextChanged(object sender, EventArgs e) {
            resetradio();
        }
        private void textBox3_TextChanged(object sender, EventArgs e) {
            resetradio();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboboxesready) {
                getvendorflags();
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboboxesready) {
                getvendorflags();
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboboxesready) {
                getvendorflags();
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e) {
            if (cbactivar1.Checked) {
                habilitar1erlinea();
                resetradio();
            }
            if (!cbactivar1.Checked) {
                deshabilitar1erlinea();
                resetradio();
            }
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e) {
            if (cbactivar2.Checked) {
                habilitar2dalinea();
                resetradio();
            }
            if (!cbactivar2.Checked) {
                deshabilitar2dalinea();
                resetradio();
            }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e) {
            if (cbactivar3.Checked) {
                habilitar3ralinea();
                resetradio();
            }
            if (!cbactivar3.Checked) {
                deshabilitar3ralinea();
                resetradio();
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e) {
            calculartotalindlls();
            if (radioButton5.Checked) {
                cbdivisa1.SelectedIndex = 0;
                cbdivisa2.SelectedIndex = 0;
                cbdivisa3.SelectedIndex = 0;
            } else if (radioButton6.Checked) {
                cbdivisa1.SelectedIndex = 1;
                cbdivisa2.SelectedIndex = 1;
                cbdivisa3.SelectedIndex = 1;
            }
        }
        
        private void comboBox1_MouseHover(object sender, EventArgs e) {
            
        }
        private void comboBoxvendor_Leave(object sender, EventArgs e) {
            ComboBox cb = (ComboBox)sender;
            foreach (ComboBoxVendors cbvendors in (sender as ComboBox).Items) {
                if (cbvendors.fssuppname == cb.Text.ToString()) {
                    return;
                }
            }
            MessageBox.Show("Proveedor no existe.");
            (sender as ComboBox).SelectedIndex = 0;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            
        }

        private void button6_Click(object sender, EventArgs e) {
            CreateExtraCharge extra = new CreateExtraCharge();
            extra.FormClosed += Extra_FormClosed; ;
            extra.lineaid = dataGridView1[0, 0].Value.ToString();
            extra.usuario = usuario;
            extra.userid = user_id;
            extra.ShowDialog();
            
        }

        private void Extra_FormClosed(object sender, FormClosedEventArgs e) {
            
        }
        
    }
}
