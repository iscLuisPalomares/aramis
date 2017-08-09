using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ComprasProject {
    public partial class RepGastoVendorChart : Form {
        public RepGastoVendorChart() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string user_id { get; set; }
        public string tipo { get; set; }
        public string user_depto { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select suppliers.id as 'ID', "
                    + "suppliers.suppname as 'Proveedor', "
                    + "round(sum(matreq.absolutdllscot), 2) as 'Costo Dlls' "
                    + "from materialrequerido matreq "
                    + "join tblPurchaseOrders pos on pos.fsid = matreq.fspurchaseorder "
                    + "join asl suppliers on suppliers.id = pos.fsvendor "
                    + "where matreq.fsid in ( "
                    + "select fsidlinea "
                    + "from tblrecibos where fsdate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' "
                    + "and fsdate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59' "
                    + ")"
                    + "group by suppliers.id, suppliers.suppname "
                    + "order by round(sum(matreq.absolutdllscot), 2) desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Proveedor"].Width = 200;
                conn.Close();
                fillchart();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void getlineas(string vendorid) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select matreq.fsid as 'ID', "
                    + "matreq.fscantidad as 'Cantidad', "
                    + "matreq.fsunimedida as 'U/M', "
                    + "matreq.fscodigo as 'Codigo', "
                    + "matreq.fsdesc as 'Descripcion', "
                    + "round(matreq.absolutdllscot, 2) as 'Costo Dlls', "
                    + "matreq.comentario as 'Comentario' "
                    + "from materialrequerido matreq "
                    + "join tblPurchaseOrders pos on pos.fsid = matreq.fspurchaseorder "
                    + "join asl suppliers on suppliers.id = pos.fsvendor "
                    + "where matreq.fsid in ( "
                    + "select fsidlinea "
                    + "from tblrecibos where fsdate >= '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' "
                    + "and fsdate <= '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59' "
                    + ") and pos.fsvendor = " + vendorid + " "
                    + "order by round(matreq.absolutdllscot, 2) desc";
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
        private void fillchart() {
            try {
                chart1.Visible = true;
                DataPoint dato;
                Series serie = new Series();
                serie.ChartType = SeriesChartType.Pie;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    dato = new DataPoint(0D, double.Parse(dr.Cells["Costo Dlls"].Value.ToString()));
                    dato.Label = dr.Cells["Proveedor"].Value.ToString();
                    dato.ToolTip = dr.Cells["Proveedor"].Value.ToString() + "  $" + dr.Cells["Costo Dlls"].Value.ToString();
                    dato.LegendToolTip = "  $" + dr.Cells["Costo Dlls"].Value.ToString();
                    dato.LabelToolTip = "T1";
                    serie.Points.Add(dato);
                }
                serie.CustomProperties = "PieLabelStyle=Disabled";
                chart1.Series[0].ToolTip = "Name #SERIESNAME : X - #VALX{F2} , Y - #VALY{F2}";
                chart1.Series.Clear();
                chart1.Series.Add(serie);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            getdata();
        }
        private void button3_Click(object sender, EventArgs e) {
            
        }
        
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            string vendor = dataGridView1["ID", e.RowIndex].Value.ToString();
            getlineas(vendor);
        }
        
    }
}
