using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ApprovedAjustes : Form {
        public ApprovedAjustes() {
            InitializeComponent();
        }

        public string usuario       { get; set; }
        public string user_id       { get; set; }
        public string tipo          { get; set; }
        public string user_depto    { get; set; }
        private string createdate   { get; set; }
        private int itemsporpagina = 0;
        private int itemsimpresosalmomento = 0;
        private string ajustenum;
        private string bucketid = "";
        double paginaactual = 0;
        double tvabsoluto = 0;
        double tinventario = 0;
        double ttotal = 0;
        bool guardarono = false;

        public void getdata() {
            try {
                dataGridView1.DataSource = null;
                try {
                    dataGridView1.Columns.Remove("Imprimir");
                    dataGridView1.Columns.Remove("Actualizar");
                } catch (Exception) { }
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT fsid, fsajuste, fscreatedby, fsimporteneto, fsrazonheadid" 
                    + ", fsstatus, fsbucketid FROM tbajustes WHERE fsstatus = 'Ajuste Aprobado' OR fsstatus = 'Impreso' order by fsid";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                if (dataGridView1.Columns.Count <= 7) {
                    try {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                    } catch (Exception) { }
                    DataGridViewButtonColumn col = new DataGridViewButtonColumn();
                    col.UseColumnTextForButtonValue = true;
                    col.Text = "Imprimir";
                    col.Name = "Imprimir";
                    DataGridViewButtonColumn col2 = new DataGridViewButtonColumn();
                    col2.UseColumnTextForButtonValue = true;
                    col2.Text = "Actualizar";
                    col2.Name = "Actualizar";
                    dataGridView1.Columns.Add(col);
                    dataGridView1.Columns.Add(col2);
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public void getajusteslineas() {
            try {
                try {
                    dataGridView2.Rows.Clear();
                } catch (Exception) { }
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select sku, fscantidadensistema, fscostounitario, fscostoext, fsrazondetail" 
                    + ", fsresponsable, fscostoext, fsvalorinventario, fscantidadfisica, fsdiferencia, fsvalorabsoluto, fslocacion"
                    + " from tbajusteslineas where fsajusteid =" + ajustenum + " and fsstatus in ('Ajuste Aprobado', 'Impreso', 'Ajustado');";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView2.DataSource = table;
                if (dataGridView2.RowCount >= 1) {
                    foreach (DataGridViewRow dr in dataGridView2.Rows) {
                        if (!dr.IsNewRow) {
                            try {
                                tvabsoluto += Math.Abs(double.Parse(dr.Cells[10].Value.ToString()));
                                tinventario += Math.Abs(double.Parse(dr.Cells[7].Value.ToString()));
                                ttotal += double.Parse(dr.Cells[6].Value.ToString());
                            } catch (Exception ex) {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                }
                sqlquery = "select fscreatedate from tbajustes where fsid = " + ajustenum + ";";
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery, conn);
                DataTable table2 = new DataTable();
                adapter2.Fill(table2);
                createdate = table2.Rows[0][0].ToString();
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void updatestatusaImpreso() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "UPDATE tbajustes SET fsstatus = 'Impreso' WHERE fsid = " + ajustenum + ";";
                string sqlquery2 = "UPDATE tbajusteslineas SET fsstatus = 'Impreso' WHERE fsajusteid = " + ajustenum;
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                SqlCommand ejecucion2 = new SqlCommand();
                ejecucion2.Connection = conn;
                ejecucion2.CommandType = CommandType.Text;
                ejecucion2.CommandText = sqlquery2;
                ejecucion2.ExecuteNonQuery();
                conn.Close();
            } catch (Exception) { }
        }
        private void updatestatusaAjustado() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                double totaltransa = 0;
                foreach (DataGridViewRow dr in dataGridView2.Rows) {
                    totaltransa += Math.Round(double.Parse(dr.Cells[3].Value.ToString()), 2);
                }
                string querybucket = "update "
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
                string sqlquery = "UPDATE tbajustes SET fsstatus = 'Ajustado' WHERE fsid = " 
                    + ajustenum + "";
                string sqlquery2 = "UPDATE tbajusteslineas SET fsstatus = 'Ajustado' WHERE fsajusteid = " 
                    + ajustenum + "";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                ejecucion.CommandText = querybucket;
                ejecucion.ExecuteNonQuery();
                ejecucion.CommandText = sqlquery2;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                getdata();
                MessageBox.Show("Registros Actualizados", "Listo");
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata();
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Imprimir") {
                ajustenum = dataGridView1[0, e.RowIndex].Value.ToString();
                
                getajusteslineas();
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.PrintPreviewControl.Zoom = 0.90;
                printPreviewDialog1.Height = 500;
                printPreviewDialog1.Width = 900;
                printPreviewDialog1.FormClosing += PrintPreviewDialog1_FormClosing;
                printPreviewDialog1.ShowDialog();
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Actualizar") {
                if (dataGridView1[5, e.RowIndex].Value.ToString() == "Impreso") {
                    ajustenum = dataGridView1[0, e.RowIndex].Value.ToString();
                    bucketid = dataGridView1[6, e.RowIndex].Value.ToString();
                    MessageBox.Show(bucketid);
                    updatestatusaAjustado();
                } else {
                    MessageBox.Show("No se ha impreso almenos una vez");
                }
            }
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            //draw posey picture
            Image img = Properties.Resources.PoseyLogo;
            e.Graphics.DrawImage(img, 20,20, 200, 70);
            int headersoffset = 150;
            int titleoffset = 180;
            //estas variables deben cambiarse juntas para formar una tabla consistente
            int delta = 16;
            int fontsize = 7;
            int registrosporhojaimpresa = 35;
            Font font = new Font("Calibri", fontsize, FontStyle.Regular);
            Font font2 = new Font("Calibri", 5, FontStyle.Regular);
            Font fonttitledata = new Font("Calibri", 12, FontStyle.Bold);
            Font fonttitle = new Font("Calibri", 22, FontStyle.Bold);
            paginaactual++;
            e.Graphics.DrawString("AJUSTES APROBADOS", fonttitle, Brushes.Black, new Point(300, 20));
            e.Graphics.DrawString("Folio A" + ajustenum, fonttitledata, Brushes.Black, new Point(600, 20));
            e.Graphics.DrawString("Aprobado por " + getapprover(), fonttitledata, Brushes.Black, new Point(600, 50));
            e.Graphics.DrawString("Fecha " + Convert.ToDateTime(createdate).ToString("yyyyMMdd"), fonttitledata, Brushes.Black, new Point(600, 80));
            //SKU    CANTIDAD       COSTO UNI     COSTO EXT      RAZON      RESPONSABLE       CHECK
            e.Graphics.DrawString("SKU", font, Brushes.Black, new Point(30, headersoffset));
            e.Graphics.DrawString("Locación", font, Brushes.Black, new Point(90, headersoffset));
            e.Graphics.DrawString("QTY\nSistema", font, Brushes.Black, new Point(140, headersoffset));
            e.Graphics.DrawString("QTY\nFisica", font, Brushes.Black, new Point(190, headersoffset));
            e.Graphics.DrawString("Diferencia", font, Brushes.Black, new Point(230, headersoffset));
            e.Graphics.DrawString("Costo/\nUnidad", font, Brushes.Black, new Point(290, headersoffset));
            e.Graphics.DrawString("Costo/\nExtendido", font, Brushes.Black, new Point(360, headersoffset));
            e.Graphics.DrawString("Razón", font, Brushes.Black, new Point(490, headersoffset));
            e.Graphics.DrawString("Responsable", font, Brushes.Black, new Point(670, headersoffset));
            e.Graphics.DrawString("Check", font, Brushes.Black, new Point(780, headersoffset));
            //_________________________________________________________________________________________
            e.Graphics.DrawLine(Pens.Black, 25, titleoffset - 5, 810, titleoffset - 5);
            
            //Escribir pagina n1 de n2
            double totaldepaginas = Math.Ceiling(double.Parse(dataGridView2.Rows.Count.ToString()) / registrosporhojaimpresa);
            e.Graphics.DrawString("Pagina " + paginaactual + " de " + totaldepaginas, font, Brushes.Black, new Point(700, 1030));

            for (int i = itemsimpresosalmomento;i < dataGridView2.Rows.Count;i++) {
                if (i >= dataGridView2.Rows.Count) {
                    e.HasMorePages = false;
                } else {
                    
                    if (itemsporpagina < registrosporhojaimpresa) {
                        itemsimpresosalmomento++;
                        itemsporpagina++;
                        e.Graphics.DrawString(dataGridView2[0,i].Value.ToString(), font, Brushes.Black, new Point(30, titleoffset));
                        e.Graphics.DrawString(dataGridView2[11, i].Value.ToString(), font, Brushes.Black, new Point(90, titleoffset));
                        e.Graphics.DrawString(dataGridView2[1, i].Value.ToString(), font, Brushes.Black, new Point(140, titleoffset));
                        e.Graphics.DrawString(dataGridView2[8, i].Value.ToString(), font, Brushes.Black, new Point(190, titleoffset));
                        e.Graphics.DrawString(dataGridView2[9, i].Value.ToString(), font, Brushes.Black, new Point(230, titleoffset));
                        e.Graphics.DrawString("$ " 
                            + Math.Round(double.Parse(dataGridView2[2, i].Value.ToString()), 2), font, Brushes.Black, new Point(300, titleoffset));
                        e.Graphics.DrawString("$ " 
                            + Math.Round(double.Parse(dataGridView2[3, i].Value.ToString()), 2), font, Brushes.Black, new Point(360, titleoffset));
                        e.Graphics.DrawString(dataGridView2[4, i].Value.ToString(), font2, Brushes.Black, new Point(410, titleoffset));
                        e.Graphics.DrawString(dataGridView2[5, i].Value.ToString(), font, Brushes.Black, new Point(670, titleoffset));
                        e.Graphics.DrawRectangle(Pens.Black, 780, titleoffset + 3, 10, 10);
                        //___________________________________________________________________________________________________
                        e.Graphics.DrawLine(Pens.Black, new Point(25, titleoffset + delta), new Point(810, titleoffset + delta));
                        //   |          |           |           |           |           |           |           |           |
                        e.Graphics.DrawLine(Pens.Black, new Point(25, titleoffset - 5), new Point(25, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(85, titleoffset - 5), new Point(85, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(135, titleoffset - 5), new Point(135, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(185, titleoffset - 5), new Point(185, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(225, titleoffset - 5), new Point(225, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(290, titleoffset - 5), new Point(290, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(350, titleoffset - 5), new Point(350, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(405, titleoffset - 5), new Point(405, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(665, titleoffset - 5), new Point(665, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(775, titleoffset - 5), new Point(775, titleoffset + delta));
                        e.Graphics.DrawLine(Pens.Black, new Point(810, titleoffset - 5), new Point(810, titleoffset + delta));
                        titleoffset = titleoffset + delta;
                    } else {
                        itemsporpagina = 0;
                        e.HasMorePages = true;
                        return;
                    }
                }
            }
            //_____________________________________________________________________________________________________
            if (itemsimpresosalmomento >= dataGridView2.Rows.Count) {
                string str1 = "Perder\n";
                string str2 = "Ganar\n";
                string str3 = "Razon\n";
                try {
                    string connectionstring = Program.stringconnection;
                    SqlConnection conn = new SqlConnection(connectionstring);
                    string sqlquery = "SELECT fsrazondetail, count(fsrazondetail) as suma, sum(fscostoext) as neto, "
                        + "sum(fsvalorafavor) as ganancia, sum(fsvalorencontra) as perdida "
                        + "from tbajusteslineas where fsajusteid = " + ajustenum + " group by fsrazondetail";
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                    DataTable tb = new DataTable();
                    adapter.Fill(tb);
                    dataGridView3.DataSource = tb;
                    
                    foreach (DataGridViewRow dr in dataGridView3.Rows) {
                        str3 += dr.Cells[0].Value.ToString() + "\n";
                        str2 += "  $" + Math.Round(double.Parse(dr.Cells[3].Value.ToString()), 2) + "\n";
                        str1 += "  $" + Math.Round(double.Parse(dr.Cells[4].Value.ToString()), 2) + "\n";
                    }
                    conn.Close();
                } catch (SqlException ex) {
                    MessageBox.Show(ex.ToString());
                    Close();
                }
                e.HasMorePages = false;
                itemsimpresosalmomento = 0;
                itemsporpagina = 0;
                double total = 0;
                foreach (DataGridViewRow dr in dataGridView2.Rows) {
                    total = total + double.Parse(dr.Cells[2].Value.ToString());
                }
                e.Graphics.DrawString("Total: $" + ttotal , font, 
                    Brushes.Black, new Point(650, titleoffset + delta + 20));
                e.Graphics.DrawString("Discrepancias: " + (Math.Round(tvabsoluto / tinventario, 4) * 100).ToString() + "%", 
                    font, Brushes.Black, new Point(500, titleoffset + delta + 20));
                e.Graphics.DrawString("Veracidad: " + Math.Round((1 - Math.Abs(tvabsoluto / tinventario)) * 100, 4).ToString() + "%", 
                    font, Brushes.Black, new Point(500, titleoffset + delta + 40));

                e.Graphics.DrawString(str2, font, Brushes.Black, new Point(300, titleoffset + delta + 20));
                e.Graphics.DrawString(str1, font, Brushes.Black, new Point(400, titleoffset + delta + 20));
                e.Graphics.DrawString(str3, font, Brushes.Black, new Point(50, titleoffset + delta + 20 + 70));
                paginaactual = 0;
            }
        }
        private void PrintPreviewDialog1_FormClosing(object sender, FormClosingEventArgs e) {
            guardarono = false;
            getdata();
        }
        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e) {
            if (guardarono == false) {
                guardarono = true;
            } else if (guardarono == true) {
                updatestatusaImpreso();
                guardarono = false;
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            ajustenum = textBox1.Text;
            getajusteslineas();
            if (dataGridView2.Rows.Count >= 1) {
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.PrintPreviewControl.Zoom = 0.85;
                printPreviewDialog1.Height = 500;
                printPreviewDialog1.Width = 900;
                printPreviewDialog1.FormClosing += PrintPreviewDialog1_FormClosing;
                printPreviewDialog1.ShowDialog();
            }
        }
        private void printPreviewDialog1_Load(object sender, EventArgs e) {
        }
        private string getapprover() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT fsapprovedby FROM tbajustes WHERE fsid = " + ajustenum + ";";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                return table.Rows[0][0].ToString();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                return "";
            }
        }
    }
}
