using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CancelPOList : Form {
        public CancelPOList() {
            InitializeComponent();
        }
        public string usuario       { get; set; }
        public string user_id       { get; set; }
        public string tipo          { get; set; }
        public string user_depto    { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid as ID, comprador.fulname as Usuario, creador.fulname as Comprador, suppliers.suppname as Supplier "
                    + "from tblPurchaseOrders "
                    + "join users comprador on tblPurchaseOrders.fsbuyer = comprador.id "
                    + "join users creador on tblPurchaseOrders.fscreatedby = creador.id "
                    + "join asl suppliers on tblpurchaseorders.fsvendor = suppliers.id "
                    + "where fsstatus = 'PO Aprobado'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
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

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            CancelPODetail cd = new CancelPODetail();
            cd.usuario = usuario;
            cd.user_id = user_id;
            cd.idpo = dataGridView1[0, e.RowIndex].Value.ToString();
            cd.FormClosed += Cd_FormClosed;
            cd.ShowInTaskbar = false;
            cd.ShowDialog();
        }

        private void Cd_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
    }
}
