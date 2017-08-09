using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ImprimirPO : Form {
        public ImprimirPO() {
            InitializeComponent();
        }
        public string idpo;
        public string subtotal;
        public string user_id;
        public string usuario;
        public string impuestos;
        public string compradorid;
        public string comentario;
        public string proveedorid;
        public string cotizacionid;
        public string terminos;
        public string fecharequerida;
        public string observaciones;
        public List<string> requisitores;
        private double costototalviejo = 0;
        private double costototalnuevo = 0;
        private double tipodecambio = 0;

        private void refresh() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquerypodata = "select fsid as 'ID', fstotalcost as 'Costo Total', "
                    + "fsimpuestos as 'Impuestos', comprador.fulname as 'Comprador', fsbuyer as 'Comprador ID', "
                    + "fscomments as 'Comentarios', fsvendor as 'Proveedor', fscotizacionid as 'Cotizacion ID', "
                    + "fsterms as 'Terminos', fsdaterequired as 'Fecha Requerida' from tblPurchaseOrders "
                    + "join users comprador on comprador.id = fsbuyer "
                    + "where fsid = " + idpo + ";";
                SqlDataAdapter adapterpodata = new SqlDataAdapter(sqlquerypodata, conn);
                DataTable tablepodata = new DataTable();
                adapterpodata.Fill(tablepodata);
                conn.Close();
                subtotal = tablepodata.Rows[0]["Costo Total"].ToString();
                impuestos = tablepodata.Rows[0]["Impuestos"].ToString();
                compradorid = tablepodata.Rows[0]["Comprador ID"].ToString();
                comentario = tablepodata.Rows[0]["Comentarios"].ToString();
                proveedorid = tablepodata.Rows[0]["Proveedor"].ToString();
                cotizacionid = tablepodata.Rows[0]["Cotizacion ID"].ToString();
                terminos = tablepodata.Rows[0]["Terminos"].ToString();
                fecharequerida = tablepodata.Rows[0]["Fecha Requerida"].ToString();
                getdata();
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "select fsid as 'ID',fscodigo as 'Codigo', fscantidad as 'Cantidad', "
                    + "fsdesc as 'Descripcion', fscostounitario as 'Costo Unitario', fstotalcost as 'Costo Extendido', "
                    + "absolutdllscot as 'Costo Total en Dlls', bucketid as 'ID Bucket' "
                    + "from materialrequerido where fspurchaseorder = " + idpo + " AND fsstatus in ('PO Creado', 'PO Recibiendo', 'PO Recibido');";
                string sqlqueryvendor = "select suppname from asl where id = " + proveedorid + "";
                string sqlquerybuyer = "select fulname from Users where id = " + compradorid + "";
                string sqldivisa = "select fsdivisa from tbcotizaciones where fsid = " + cotizacionid + "";
                sqldivisa = "select cotizacion.fsdivisa, tblPurchaseOrders.fsdepto, tblPurchaseOrders.fsstatuscomment "
                    + "from tblPurchaseOrders join tbcotizaciones cotizacion "
                    + "on cotizacion.fsid = tblPurchaseOrders.fscotizacionid where tblPurchaseOrders.fsid = " + idpo + ";";
                string sqlqueryrequisitores = "select createdby, deptoid from requisiciones where id_req in " +
                    "(select fsrequisicion from materialrequerido where fspurchaseorder = '" + idpo + "')";
                string sqlqueryrequisitors = "select fulname from users where username in ("
                    + "select createdby from requisiciones where id_req in ("
                    + "select fsrequisicion from materialrequerido where fspurchaseorder = '" + idpo + "'))";
                string querytipodecambio = "select top(1) valor from tbtipodecambio order by id desc";
                
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adaptervendor = new SqlDataAdapter(sqlqueryvendor, conn);
                SqlDataAdapter adapterbuyer = new SqlDataAdapter(sqlquerybuyer, conn);
                SqlDataAdapter adapterdivisa = new SqlDataAdapter(sqldivisa, conn);
                SqlDataAdapter adapterrequisitores = new SqlDataAdapter(sqlqueryrequisitores, conn);
                SqlDataAdapter adapterrequisitors = new SqlDataAdapter(sqlqueryrequisitors, conn);
                SqlDataAdapter adaptertipodecambio = new SqlDataAdapter(querytipodecambio, conn);
                
                DataTable tb = new DataTable();
                DataTable tbcopy = new DataTable();
                DataTable tb2 = new DataTable();
                DataTable tb3 = new DataTable();
                DataTable tb4 = new DataTable();
                DataTable tb5 = new DataTable();
                DataTable tb6 = new DataTable();
                DataTable tb7 = new DataTable();
                DataTable tb8 = new DataTable();
                DataTable tb9 = new DataTable();

                adapter.Fill(tb);
                adapter.Fill(tbcopy);
                adaptervendor.Fill(tb2);
                adapterbuyer.Fill(tb3);
                adapterdivisa.Fill(tb4);
                adapterrequisitores.Fill(tb5);
                adapterrequisitors.Fill(tb6);
                adaptertipodecambio.Fill(tb9);

                //guardar tipo de cambio en una variable global
                tipodecambio = double.Parse(tb9.Rows[0][0].ToString());
                
                textBox3.Text = tb3.Rows[0][0].ToString();
                textBox4.Text = "";
                textBox5.Text = "";
                foreach (DataRow dr in tb5.Rows) {
                    textBox5.Text += dr[1] + ",";
                }
                foreach (DataRow dr in tb6.Rows) {
                    textBox4.Text += dr[0] + ",";
                }

                textBox7.Text = tb2.Rows[0][0].ToString();
                label12.Text = tb4.Rows[0][0].ToString();
                textBox5.Text = tb4.Rows[0][1].ToString();
                textBox10.Text = tb4.Rows[0][2].ToString();
                label4.Text = "A" + idpo;
                textBox4.Text = textBox4.Text.Remove(textBox4.Text.Length - 1);
                dataGridView1.DataSource = tb;
                if (dataGridView1.Rows.Count == 0) {
                    MessageBox.Show("PO Vacio, contacte al administrador para eliminar PO");
                    Close();
                    return;
                }
                dataGridView2.DataSource = tbcopy;
                dataGridView1.Columns["ID"].ReadOnly = true;
                dataGridView1.Columns["Codigo"].ReadOnly = false;
                dataGridView1.Columns["Cantidad"].ReadOnly = true;
                dataGridView1.Columns["Descripcion"].ReadOnly = false;
                dataGridView1.Columns["Costo Unitario"].ReadOnly = false;
                dataGridView1.Columns["Costo Extendido"].ReadOnly = true;
                dataGridView1.Columns["Costo Total en Dlls"].ReadOnly = true;
                dataGridView1.Columns["ID Bucket"].ReadOnly = true;

                string querydate = "SELECT fsdaterequired FROM tblPurchaseOrders WHERE fsid = " + idpo + ";";
                SqlDataAdapter adapterdate = new SqlDataAdapter(querydate, conn);
                DataTable da = new DataTable();
                adapterdate.Fill(da);
                DateTime dt = Convert.ToDateTime(da.Rows[0][0].ToString());
                textBox9.Text = dt.ToString("yyyyMMdd");

                string sqlquerypocomments = "SELECT fscomments FROM tblPurchaseOrders WHERE fsid = " + idpo + ";";

                string sqlquerycomment = "select motivo from requisiciones where id_req in "
                    + "(select distinct(materialrequerido.fsrequisicion) from materialrequerido where fsid in (";
                foreach (DataGridViewRow drr in dataGridView1.Rows) {
                    sqlquerycomment += drr.Cells[0].Value.ToString() + ", ";
                }
                sqlquerycomment = sqlquerycomment.Remove(sqlquerycomment.Length - 2);
                sqlquerycomment += "))";
                textBox6.Text = "";
                SqlDataAdapter adapterpocoments = new SqlDataAdapter(sqlquerypocomments, conn);
                adapterpocoments.Fill(tb8);

                foreach (DataRow dr in tb8.Rows) {
                    textBox6.Text += dr[0].ToString() + ", ";
                }
                textBox6.Text = textBox6.Text.Remove(textBox6.Text.Length - 2);
                if (textBox6.Text == "") {
                    SqlDataAdapter adaptercomments = new SqlDataAdapter(sqlquerycomment, conn);
                    adaptercomments.Fill(tb7);
                    foreach (DataRow dr in tb7.Rows) {
                        textBox6.Text += dr[0].ToString() + ", ";
                    }
                    textBox6.Text = textBox6.Text.Remove(textBox6.Text.Length - 2);
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
        private void getdeptos() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquerydeptos = "SELECT name FROM deptos WHERE id IN (" + textBox5.Text.Remove(textBox5.Text.Length - 1) + ")";
                conn.Open();
                SqlDataAdapter adapterrequisitores = new SqlDataAdapter(sqlquerydeptos, conn);
                DataTable tb = new DataTable();
                adapterrequisitores.Fill(tb);
                textBox5.Text = "";
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void ImprimirPO_Load(object sender, EventArgs e) {
            getdata();
            textBox1.Text = observaciones;
            textBox2.Text = DateTime.Now.ToString("yyyyMMdd");
            textBox9.Text = Convert.ToDateTime(fecharequerida).ToString("yyyyMMdd");
            textBox8.Text = terminos;
            foreach (DataGridViewRow dr in dataGridView1.Rows) {
                costototalviejo += double.Parse(dr.Cells["Costo Extendido"].Value.ToString());
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            if (textBox6.Text.Length > 200) { MessageBox.Show("Campo de comentarios es demasiado extenso.s"); }
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.PrintPreviewControl.Zoom = 0.90;
            printPreviewDialog1.Height = 500;
            printPreviewDialog1.Width = 900;
            printPreviewDialog1.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e) {
            try {
                Image img = Properties.Resources.purchase;
                double taxes;
                if (impuestos == null || impuestos == "") { taxes = 0; } else {
                    taxes = double.Parse(impuestos);
                }
                string tex = "";
                double impuestototal = Math.Round(double.Parse(subtotal) * (taxes / 100), 2);
                double total = Math.Round(double.Parse(subtotal) + impuestototal, 2);
                e.Graphics.DrawImage(img, 0, 0, e.PageBounds.Width, e.PageBounds.Height);
                using (var fonttitle = new Font("Calibri", 20, FontStyle.Bold))
                using (var fontdate = new Font("Calibri", 12, FontStyle.Bold))
                using (var fontcomment = new Font("Calibri", 8, FontStyle.Regular))
                using (var font = new Font("Calibri", 9, FontStyle.Regular))
                using (var fontrequesters = new Font("Calibri", 9, FontStyle.Regular | FontStyle.Bold))
                    //cambios de fuente 26 mayo 2017 7:37
                using (var fontmono = new Font(FontFamily.GenericMonospace, 7, FontStyle.Regular)) {
                    e.Graphics.DrawString("A-" + idpo, fonttitle, Brushes.Red, 705f, 46f);
                    e.Graphics.DrawString(textBox2.Text, fontdate, Brushes.Black, 67f, 157f);
                    e.Graphics.DrawString(textBox3.Text, fontdate, Brushes.Black, 347f, 157f);

                    tex = textBox4.Text;
                    try {
                        if (tex.Length >= 30) {
                            string[] arreglo = Split(tex, 30).ToArray();
                            for (int i = 0;i <= arreglo.Length - 1;i++) {
                                try {
                                    if (arreglo.Length - 1 - i >= 2) {
                                        e.Graphics.DrawString(arreglo[i] + "-", fontrequesters, Brushes.Black, 111f, 185f);
                                        e.Graphics.DrawString(arreglo[i + 1] + "-", fontrequesters, Brushes.Black, 111f, 185f + 12);
                                        i++;
                                    } else if (arreglo.Length - 1 - i == 1) {
                                        e.Graphics.DrawString(arreglo[i] + "-", fontrequesters, Brushes.Black, 111f, 185f);
                                        e.Graphics.DrawString(arreglo[i + 1] + "", fontrequesters, Brushes.Black, 111f, 185f + 12);
                                        i++;
                                    } else if (arreglo.Length - 1 - i == 0) {
                                        e.Graphics.DrawString(arreglo[i] + "", fontrequesters, Brushes.Black, 111f, 185f);
                                    }
                                    if (i < arreglo.Length - 1) {
                                        //ejey += 26.3f;
                                    }
                                } catch (Exception ex) {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        } else {
                            e.Graphics.DrawString(tex, fontrequesters, Brushes.Black, 111f, 185f);
                        }
                    } catch (Exception ex) {
                        MessageBox.Show(ex.ToString());
                    }

                    e.Graphics.DrawString(textBox5.Text, fontdate, Brushes.Black, 430f, 185f);
                    float stringsize = 0f;
                    stringsize = e.Graphics.MeasureString(textBox6.Text, fontcomment).Width;
                    if (stringsize >= 630) { MessageBox.Show("Es probable que el texto sea demasiado extenso"); }
                    e.Graphics.DrawString(textBox6.Text, fontcomment, Brushes.Black, 180f, 212f);
                    e.Graphics.DrawString(textBox7.Text, font, Brushes.Black, 180f, 225f);
                    e.Graphics.DrawString(textBox8.Text, fontdate, Brushes.Black, 107f, 237f);
                    e.Graphics.DrawString(textBox9.Text, fontdate, Brushes.Black, 650f, 237f);
                    e.Graphics.DrawString("$" + double.Parse(subtotal).ToString("N"), font, Brushes.Black, 705f, 865f);
                    e.Graphics.DrawString("$" + double.Parse(impuestototal.ToString()).ToString("N"), font, Brushes.Black, 705f, 886f);
                    e.Graphics.DrawString("$" + double.Parse(total.ToString()).ToString("N"), font, Brushes.Black, 705f, 905f);
                    if (label12.Text == "USD") {
                        e.Graphics.DrawString("X", font, Brushes.Black, 765f, 935f);
                    } else if (label12.Text == "MXP") {
                        e.Graphics.DrawString("X", font, Brushes.Black, 765f, 920f);
                    }
                    float ejey = 332f;
                    foreach (DataGridViewRow dr in dataGridView1.Rows) {
                        e.Graphics.DrawString(dr.Cells["Cantidad"].Value.ToString(), font, Brushes.Black, 70f, ejey);
                        tex = dr.Cells["Codigo"].Value.ToString();
                        try {
                            if (tex.Length >= 11) {
                                e.Graphics.DrawString(tex.Substring(0, 10).ToString()
                                    + "-", font, Brushes.Black, 160f, ejey);
                                e.Graphics.DrawString(tex.Substring(10).ToString(), font, Brushes.Black, 160f, ejey + 12);
                            } else {
                                e.Graphics.DrawString(tex, font, Brushes.Black, 160f, ejey);
                            }
                        } catch (Exception ex) {
                            MessageBox.Show(ex.Message);
                        }

                        e.Graphics.DrawString("$" + double.Parse(dr.Cells["Costo Unitario"].Value.ToString()).ToString("N"), font, Brushes.Black, 640f, ejey);
                        e.Graphics.DrawString("$" + double.Parse(dr.Cells["Costo Extendido"].Value.ToString()).ToString("N"), font, Brushes.Black, 720f, ejey);

                        tex = dr.Cells["Descripcion"].Value.ToString();
                        try {
                            if (tex.Length >= 63) {
                            //if (tex.Length >= 49) {
                                string[] arreglo = Split(tex, 63).ToArray();
                                //string[] arreglo = Split(tex, 49).ToArray();
                                int totallineas = arreglo.Length;
                                for (int i = 0;i <= arreglo.Length - 1;i++) {
                                    try {
                                        if (arreglo.Length - 1 - i >= 2) {
                                            e.Graphics.DrawString(arreglo[i] + "-", fontmono, Brushes.Black, 242f, ejey);
                                            e.Graphics.DrawString(arreglo[i + 1] + "-", fontmono, Brushes.Black, 242f, ejey + 12);
                                            i++;
                                        } else if (arreglo.Length - 1 - i == 1) {
                                            e.Graphics.DrawString(arreglo[i] + "-", fontmono, Brushes.Black, 242f, ejey);
                                            e.Graphics.DrawString(arreglo[i + 1] + "", fontmono, Brushes.Black, 242f, ejey + 12);
                                            i++;
                                        } else if (arreglo.Length - 1 - i == 0) {
                                            e.Graphics.DrawString(arreglo[i] + "", fontmono, Brushes.Black, 242f, ejey);
                                        }
                                        if (i < arreglo.Length - 1) {
                                            ejey += 26.3f;
                                        }
                                    } catch (Exception ex) {
                                        MessageBox.Show(ex.ToString());
                                    }
                                }
                            } else {
                                e.Graphics.DrawString(tex, fontmono, Brushes.Black, 242f, ejey);
                            }
                        } catch (Exception ex) {
                            MessageBox.Show(ex.ToString());
                        }
                        ejey += 26.3f;
                    }
                    int ejeyint = (int)ejey;
                    try {
                        e.Graphics.DrawLine(Pens.Black, new Point(54, 860), new Point(801, ejeyint + 3));
                        if (textBox1.Text.Length == 0) {
                            e.Graphics.DrawString("Nota: derivado de las disposiciones\nFiscales ante la ley "
                            + "le solicitamos nos\nproporcione la carta de Opinion Positiva\nde "
                            + "Cumplimiento en forma mensual", font, Brushes.Red, 25f, 880f);
                        } else {
                            ejey = 880f;
                            tex = observaciones;
                            try {
                                if (tex.Length >= 50) {
                                    string[] arreglo = Split(tex, 50).ToArray();
                                    int totallineas = arreglo.Length;
                                    for (int i = 0;i <= arreglo.Length - 1;i++) {
                                        try {
                                            if (arreglo.Length - 1 - i >= 2) {
                                                e.Graphics.DrawString(arreglo[i]
                                                    + "-", font, Brushes.Red, 25f, ejey);
                                                e.Graphics.DrawString(arreglo[i + 1]
                                                    + "-", font, Brushes.Red, 25f, ejey + 12);
                                                i++;
                                            } else if (arreglo.Length - 1 - i == 1) {
                                                e.Graphics.DrawString(arreglo[i]
                                                    + "-", font, Brushes.Red, 25f, ejey);
                                                e.Graphics.DrawString(arreglo[i + 1]
                                                    + "", font, Brushes.Red, 25f, ejey + 12);
                                                i++;
                                            } else if (arreglo.Length - 1 - i == 0) {
                                                e.Graphics.DrawString(arreglo[i]
                                                    + "", font, Brushes.Red, 25f, ejey);
                                            }
                                            if (i < arreglo.Length - 1) {
                                                ejey += 26.3f;
                                            }
                                        } catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                                    }
                                } else { e.Graphics.DrawString(tex, font, Brushes.Red, 25f, ejey); }
                            } catch (Exception ex1) { MessageBox.Show(ex1.ToString()); }
                        }
                    } catch (Exception ex2) { MessageBox.Show(ex2.ToString()); }
                }
            } catch (Exception ex3) { MessageBox.Show(ex3.ToString()); }
        } 
        private void btnimprimir_Click(object sender, EventArgs e) {
            PrintDialog pdi = new PrintDialog();
            pdi.Document = printDocument1;
            if (pdi.ShowDialog() == DialogResult.OK) {
                printDocument1.Print();
            } else {
                MessageBox.Show("Print Cancelled");
            }
        }
        private void PrintDocument1_BeginPrint(object sender, PrintEventArgs e) {
            
        }
        private void button3_Click(object sender, EventArgs e) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                sqlquery += "UPDATE tblpurchaseorders SET datecancel = '" + DateTime.Now.ToString() 
                    + "', canceledby = " + user_id + ", " 
                    + "fsstatus = 'PO Cancelado' WHERE fsid = " + idpo + ";\n";
                sqlquery += "UPDATE materialrequerido SET fsstatus = 'Requisicion Aprobada' WHERE fspurchaseorder = " + idpo + ";\n";
                sqlquery += "UPDATE tbcotizaciones SET fsstatus = 'PO Cancelado', fsganadora = 0 WHERE fsid = " + cotizacionid + ";\n";
                
                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                MessageBox.Show(sqlquery);
                conn.Close();

                MessageBox.Show("PO Cancelado", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private string querybuckets() {
            string sqlquery = "";
            foreach (DataGridViewRow dr in dataGridView1.Rows) {
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
            }
            return sqlquery;
        }
        private double gettotal() {
            double nuevototal = 0;
            foreach (DataGridViewRow dr in dataGridView1.Rows) {
                nuevototal = nuevototal + double.Parse(dr.Cells["Costo Extendido"].Value.ToString());
            }
            return nuevototal;
        }
        private void guardarcambios() {
            //return;
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                double diferencia = 0;
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                double totalnuevo = gettotal();//corregir
                for (int i = 0; i <= dataGridView1.RowCount - 1; i++) {
                    double unidadendlls = 0;
                    if (double.Parse(dataGridView1["Cantidad", i].Value.ToString()) > 0) {
                        unidadendlls = Math.Round(double.Parse(dataGridView1["Costo Total en Dlls", i].Value.ToString()) / double.Parse(dataGridView1["Cantidad", i].Value.ToString()), 2);
                    }
                    sqlquery += "UPDATE materialrequerido SET "
                        + "fscostounitario = " + dataGridView1["Costo Unitario", i].Value.ToString() + ", "
                        + "fstotalcost = " + dataGridView1["Costo Extendido", i].Value.ToString() + ", "
                        + "absolutdllscot = " + dataGridView1["Costo Total en Dlls", i].Value.ToString() + ", "
                        + "fsdesc = '" + dataGridView1["Descripcion", i].Value.ToString() + "', " 
                        + "absdllscotuni = " + unidadendlls + ", "
                        + "fscodigo = '" + dataGridView1["Codigo", i].Value.ToString() + "' "
                        + "WHERE fsid = " + dataGridView1["ID", i].Value.ToString() + ";\n";
                    costototalnuevo = double.Parse(dataGridView1["Costo Extendido", i].Value.ToString());
                    costototalviejo = double.Parse(dataGridView2["Costo Extendido", i].Value.ToString());
                    diferencia = costototalviejo - costototalnuevo;
                    if (diferencia < 0) {
                        MessageBox.Show("Revise los nuevos costos, alguno puede ser mayor");
                        return;
                    }
                }
                //correccion
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
                sqlquery += "UPDATE tblPurchaseOrders SET fstotalcost = " + totalnuevo.ToString() + ", "
                    + "fscomments = '" + textBox6.Text + "', "
                    + "fsdaterequired = '" + dateTimePicker1.Value.ToString("yyyyMMdd") + "', "
                    + "fsdepto = '" + textBox5.Text + "', "
                    + "fsstatuscomment = '" + textBox10.Text + "' "
                    + "WHERE fsid = " + idpo + ";\n";
                sqlquery += "UPDATE tblPurchaseOrders SET fsdepto = '" + textBox5.Text + "' WHERE fsid = " + idpo + ";\n";
                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                
                conn.Close();
                MessageBox.Show("Informacion actualizada", "Listo");
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            double costounitario = 0;
            double piezas = 0;
            double costolinea = 0;
            try {
                costounitario = double.Parse(dataGridView1["Costo Unitario", e.RowIndex].Value.ToString());
                piezas = double.Parse(dataGridView1["Cantidad", e.RowIndex].Value.ToString());
                costolinea = costounitario * piezas;
                dataGridView1["Costo Extendido", e.RowIndex].Value = costolinea.ToString();
                if (label12.Text == "USD") {
                    dataGridView1["Costo Total en Dlls", e.RowIndex].Value = costolinea.ToString();
                } else {
                    dataGridView1["Costo Total en Dlls", e.RowIndex].Value = Math.Round(costolinea / tipodecambio, 2).ToString();
                }
            } catch (Exception) { }
        }
        private void btn_savechanges(object sender, EventArgs e) {
            guardarcambios();
            refresh();
            
        }
        static IEnumerable<string> Split2(string str, int chunkSize) {
            return Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        static IEnumerable<string> Split(string str, int maxChunkSize) {
            for (int i = 0;i < str.Length;i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Descripcion") {
                    //MessageBox.Show(dataGridView1["Descripcion", e.RowIndex].Value.ToString());
                }
            } catch (Exception) { }
        }

        string lineaid = "";
        string descr = "";
        string costo = "";
        string bucket = "";
        int posx = 0;
        int posy = 0;
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                ContextMenuStrip m = new ContextMenuStrip();
                m.Items.Add("Eliminar");
                m.ItemClicked += M_ItemClicked;
                lineaid = dataGridView1["ID", e.RowIndex].Value.ToString();
                descr = dataGridView1["Descripcion", e.RowIndex].Value.ToString();
                costo = dataGridView1["Costo Total en Dlls", e.RowIndex].Value.ToString();
                bucket = dataGridView1["ID Bucket", e.RowIndex].Value.ToString();
                m.Show(dataGridView1, new Point(posx, posy));
            }
        }
        private void M_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            ToolStripItem btn = e.ClickedItem;
            if (btn.Text == "Eliminar") {
                try {
                    EliminarLinea el = new EliminarLinea();
                    el.lineaid = lineaid;
                    el.descripcion = descr;
                    el.costo = costo;
                    el.bucket = bucket;
                    el.FormClosed += El_FormClosed;
                    el.ShowDialog();
                } catch (Exception) { MessageBox.Show("Fatal Error"); }
            }
        }
        private void El_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
            textBox2.Text = DateTime.Now.ToString("yyyyMMdd");
            textBox9.Text = Convert.ToDateTime(fecharequerida).ToString("yyyyMMdd");
            textBox8.Text = terminos;
            foreach (DataGridViewRow dr in dataGridView1.Rows) {
                costototalviejo += double.Parse(dr.Cells["Costo Extendido"].Value.ToString());
            }
        }
        private void dataGridView1_MouseMove(object sender, MouseEventArgs e) {
            posx = e.X;
            posy = e.Y;
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e) {

        }
    }
}
