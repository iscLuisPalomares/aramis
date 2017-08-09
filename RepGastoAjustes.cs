using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ComprasProject {
    public partial class RepGastoAjustes : Form {
        public RepGastoAjustes() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string user_id { get; set; }
        public string tipo { get; set; }
        public string user_depto { get; set; }
        DataTable table;
        DataTable table2;

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid as 'ID', fscreatedate as 'Fecha Creada', "
                    + "fsajuste as 'Motivo', round(fsimporteneto, 2) as 'Importe Neto', "
                    + "fsapprovedby as 'Aprobó', fsapprovedate as 'Fecha Aprobada' "
                    + "from tbajustes where "
                    + "fsapprovedate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00.000' "
                    + "and fsapprovedate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59.999' "
                    + "and fsstatus in ('Ajustado', 'Impreso') order by fscreatedate";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["ID"].Width = 70;
                dataGridView1.Columns["Fecha Creada"].Width = 170;
                dataGridView1.Columns["Motivo"].Width = 250;
                dataGridView1.Columns["Importe Neto"].Width = 100;
                dataGridView1.Columns["Fecha Aprobada"].Width = 170;
                tabControl1.SelectedIndex = 0;
                conn.Close();
                fillchart();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void fillchart() {
            try {
                chart1.Visible = true;
                DataPoint dato;
                Series serie = new Series();
                serie.ChartType = SeriesChartType.Pie;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    dato = new DataPoint(0D, double.Parse(dr.Cells["Importe Neto"].Value.ToString()));
                    dato.Label = dr.Cells["Motivo"].Value.ToString();
                    dato.ToolTip = dr.Cells["Motivo"].Value.ToString() + "  $" + dr.Cells["Importe Neto"].Value.ToString();
                    dato.LegendToolTip = "  $" + dr.Cells["Importe Neto"].Value.ToString();
                    //dato.LabelToolTip = "T0";
                    serie.Points.Add(dato);
                }
                //serie.Points[0]
                serie.CustomProperties = "PieLabelStyle=Disabled";
                chart1.Series[0].ToolTip = "#SERIESNAME : #VALY{F2}";
                chart1.Series.Clear();
                chart1.Series.Add(serie);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            string ajusteid = dataGridView1["ID", e.RowIndex].Value.ToString();
            getlineas(ajusteid);
        }
        private void getlineas(string ajusteid) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid as 'ID', fsajusteid as 'Ajuste', "
                    + "fslocacion as 'Locacion', sku as 'SKU', "
                    + "fscantidadensistema as 'Qty Sistema', "
                    + "fscantidadfisica as 'Qty Fisica', fsdiferencia as 'Diferencia', "
                    + "fscostounitario as 'Costo Unitario', fscostoext as 'Costo Extendido', "
                    + "fsrazondetail as 'Razon' FROM tbajusteslineas WHERE fsajusteid = " + ajusteid + ";";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView2.DataSource = table;
                conn.Close();
                tabControl1.SelectedIndex = 1;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void RepGastoAjustes_Load(object sender, EventArgs e) {
            getperiods();
            getperioddatatables();
            fillperiod1chart();
            fillperiod2chart();
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
        }
        private void getperiods() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select distinct(periodo) "
                    + "from buckets join tbajustes on tbajustes.fsbucketid = id_bucket order by periodo desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                foreach (DataRow dr in table.Rows) {
                    comboBox1.Items.Add(dr[0].ToString());
                    comboBox2.Items.Add(dr[0].ToString());
                }
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                conn.Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void getperioddatatables() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid as 'ID', fscreatedate as 'Fecha Creada', "
                    + "fsajuste as 'Motivo', round(fsimporteneto, 2) as 'Importe Neto', "
                    + "fsapprovedby as 'Aprobó', fsapprovedate as 'Fecha Aprobada', buckets.periodo as 'Periodo' "
                    + "from tbajustes join buckets on id_bucket = fsbucketid where "
                    + "periodo = '" + comboBox1.SelectedItem.ToString() + "' "
                    + "and fsstatus in ('Ajustado', 'Impreso') order by fscreatedate";
                string sqlquery2 = "select fsid as 'ID', fscreatedate as 'Fecha Creada', "
                    + "fsajuste as 'Motivo', round(fsimporteneto, 2) as 'Importe Neto', "
                    + "fsapprovedby as 'Aprobó', fsapprovedate as 'Fecha Aprobada', buckets.periodo as 'Periodo' "
                    + "from tbajustes join buckets on id_bucket = fsbucketid where "
                    + "periodo = '" + comboBox2.SelectedItem.ToString() + "' "
                    + "and fsstatus in ('Ajustado', 'Impreso') order by fscreatedate";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery2, conn);
                table = new DataTable();
                table2 = new DataTable();
                adapter.Fill(table);
                adapter2.Fill(table2);
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void fillperiod1chart() {
            try {
                chart2.Palette = ChartColorPalette.Pastel;
                chart2.Visible = true;
                DataPoint dato;
                Series serie = new Series();
                serie.ChartType = SeriesChartType.Pie;
                foreach (DataRow dr in table.Rows) {
                    dato = new DataPoint(0D, double.Parse(dr["Importe Neto"].ToString()));
                    dato.Label = dr["Motivo"].ToString();
                    dato.ToolTip = dr["Motivo"].ToString() + "  $" + dr["Importe Neto"].ToString();
                    dato.LegendToolTip = "  $" + dr["Importe Neto"].ToString();
                    dato.LabelToolTip = "T0";
                    serie.Points.Add(dato);
                }
                serie.CustomProperties = "PieLabelStyle=Disabled";
                chart2.Series.Clear();
                chart2.Series.Add(serie);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void fillperiod2chart() {
            try {
                chart3.Visible = true;
                DataPoint dato;
                Series serie = new Series();
                serie.ChartType = SeriesChartType.Pie;
                foreach (DataRow dr in table2.Rows) {
                    dato = new DataPoint(0D, double.Parse(dr["Importe Neto"].ToString()));
                    dato.Label = dr["Motivo"].ToString();
                    dato.ToolTip = dr["Motivo"].ToString() + "  $" + dr["Importe Neto"].ToString();
                    dato.LegendToolTip = "  $" + dr["Importe Neto"].ToString();
                    dato.LabelToolTip = "T0";
                    serie.Points.Add(dato);
                }
                serie.CustomProperties = "PieLabelStyle=Disabled";
                chart3.Series.Clear();
                chart3.Series.Add(serie);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            getperioddatatables();
            fillperiod1chart();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            getperioddatatables();
            fillperiod2chart();
        }
    }
}
