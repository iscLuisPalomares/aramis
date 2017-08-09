using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ComprasProject {
    public partial class RepVendorDuties : Form {
        public RepVendorDuties() {
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
                string sqlquery = "select vendor.id as 'Proveedor ID', vendor.suppname as 'Proveedor', "
                    + "round(sum(matreq.absolutdllscot), 2) as 'Saldo Dlls' "
                    + "from materialrequerido matreq join tblPurchaseOrders pos "
                    + "on matreq.fspurchaseorder = pos.fsid join asl vendor "
                    + " on vendor.id = pos.fsvendor "
                    + "where pos.fsid >= 1000 and(pos.fsstatus = 'PO Aprobado' OR pos.fsstatus = 'PO Recibiendo')"
                    + "group by vendor.suppname, vendor.id "
                    + "order by round(sum(matreq.absolutdllscot), 2) desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Proveedor ID"].Width = 70;
                dataGridView1.Columns["Proveedor"].Width = 250;
                foreach (DataGridViewColumn dc in dataGridView1.Columns) {
                    dc.DefaultCellStyle.Font = new Font("Arial", 10F, GraphicsUnit.Point);
                }
                conn.Close();
                fillchart();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }
        private void fillchart() {
            try {
                chart1.Visible = true;
                DataPoint dato;
                Series serie = new Series();
                serie.ChartType = SeriesChartType.Pie;
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    dato = new DataPoint(0D, double.Parse(dr.Cells["Saldo Dlls"].Value.ToString()));
                    dato.Label = dr.Cells["Proveedor"].Value.ToString();
                    dato.ToolTip = dr.Cells["Proveedor"].Value.ToString() + "  $" + dr.Cells["Saldo Dlls"].Value.ToString();
                    dato.LegendToolTip = "  $" + dr.Cells["Saldo Dlls"].Value.ToString();
                    dato.LabelToolTip = "T0";
                    serie.Points.Add(dato);
                }
                serie.CustomProperties = "PieLabelStyle=Disabled";
                //chart1.Series[0].ToolTip = "Name #SERIESNAME : X - #VALX{F2} , Y - #VALY{F2}";
                chart1.Series.Clear();
                chart1.Series.Add(serie);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata();
        }
        private void button1_Click(object sender, EventArgs e) {
            Close();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            try {
                string vendor = dataGridView1["Proveedor ID", e.RowIndex].Value.ToString();
                getlineas(vendor);
            } catch (Exception) { }
        }
        private void getlineas(string vendorid) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid as 'ID', fspurchaseorder as 'PO', "
                    + "fscodigo as 'Codigo', fsdesc as 'Descripcion', fscantidad as 'Cantidad', "
                    + "fsunimedida as 'U/M', fsstatus as 'Status', saldo as 'Saldo', recibido as 'Recibido', "
                    + "absolutdllscot as 'Costo Total Dlls', absdllscotuni as 'Costo Unidad' "
                    + "from materialrequerido matreq where matreq.fspurchaseorder in ( "
                    + "select fsid from tblPurchaseOrders where fsvendor = " + vendorid + " and fsid >= 1000 "
                    + "and(fsstatus = 'PO Aprobado' OR fsstatus = 'PO Recibiendo')) "
                    + "order by absolutdllscot desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView2.DataSource = table;
                foreach (DataGridViewColumn dc in dataGridView2.Columns) {
                    dc.DefaultCellStyle.Font = new Font("Arial", 10F, GraphicsUnit.Point);
                }
                conn.Close();
                tabControl1.SelectedIndex = 1;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
    }
}
