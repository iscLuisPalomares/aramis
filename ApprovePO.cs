using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ApprovePO : Form {
        public ApprovePO() {
            InitializeComponent();
        }

        public string idpo          { get; set; }
        public string usuario       { get; set; }
        public string account       { get; set; }
        public string accountid     { get; set; }
        public double costototal    { get; set; }
        public string user_id       { get; set; }

        private string getaccountid(string accountnum) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT id FROM accounts where acctnumber = '" + accountnum + "'";
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
                MessageBox.Show(ex.Message);
                Close();
                return "";
            }
        }
        private void disapprovecot() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                string sqlquery = "UPDATE tbcotizaciones SET fsapprovedate = " + "GETDATE()" + ", fsapprovedby = '" + usuario + "', " +
                    "fsstatus = 'Cotizacion Desaprobada' WHERE fsid = '" + idpo + "'; ";
                string sqlquery2 = "UPDATE materialrequerido SET fsstatus = 'Cotizacion Desaprobada' WHERE fsid IN (" +
                    "SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fsidcotizacion = '" + idpo + "')";
                
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

                MessageBox.Show("Cotizacion Desaprobada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void aprobarcotizacion() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                //query para tabla cotizaciones
                string sqlquery = "UPDATE tblPurchaseOrders SET fsdateapproved = " + "GETDATE()" + ", fsapprovedby = '" + user_id + "', " +
                    "fsstatus = 'PO Aprobado' WHERE fsid = '" + idpo + "'; ";
                //query para tabla de materialrequerido
                string sqlquery2 = "UPDATE materialrequerido SET fsstatus = 'PO Aprobado' WHERE fsid IN (" +
                    "SELECT fsidmaterialrequerido FROM tbcotmaterialrequerido WHERE fspurchaseorder = '" + idpo + "')";
                
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

                MessageBox.Show("Cotizacion aprobada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private string getreqnums() {
            string numeros = "";
            foreach (DataGridViewRow dr in dataGridView1.Rows) {
                numeros += "'" + dr.Cells[12].Value.ToString() + "',";
                if (!dr.IsNewRow) {

                }
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
        private void actualizarconsumo() {
            //pass across all rows in datagridview
            try {
                for (int i = 0;i < dataGridView1.Rows.Count;i++) {
                    //create variable for dollars
                    double dolares = double.Parse(dataGridView1[10, i].Value.ToString());
                    //obtener el numero de cuenta del dgv
                    string accountnum = dataGridView1[12, i].Value.ToString();
                    //get account id out of the account number
                    string accountid = getaccountid(accountnum);
                    //pass through all rows of dgv number 3
                    for (int j = 0;j < dataGridView3.RowCount;j++) {
                        //if it is a new row, we dont continue
                        if (!dataGridView3.Rows[j].IsNewRow) {
                            //si el id de la cuenta es igual al que esta en el dgv de buckets para actualizar
                            if (dataGridView3[1, j].Value.ToString() == accountid) {
                                double ajustado = double.Parse(dataGridView3[3, j].Value.ToString());
                                double gasto = double.Parse(dataGridView3[4, j].Value.ToString());
                                double asignado = double.Parse(dataGridView3[5, j].Value.ToString());
                                double balance = double.Parse(dataGridView3[6, j].Value.ToString());
                                asignado = asignado + dolares;
                                balance = ajustado - gasto - asignado;
                                dataGridView3[6, j].Value = balance.ToString();
                                dataGridView3[5, j].Value = asignado.ToString();
                            }
                        }
                    }
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
            
        }
        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT * FROM tblPurchaseOrders WHERE fsid = '" + idpo + "'";
                string sqlquery4 = "select * from materialrequerido where fspurchaseorder = '" + idpo + "'";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter4 = new SqlDataAdapter(sqlquery4, conn);
                DataTable tb = new DataTable();
                DataTable tb4 = new DataTable();
                adapter.Fill(tb);
                adapter4.Fill(tb4);
                dataGridView1.DataSource = tb4;

                //Get purchase order information set in all textboxs
                DataRow dr = tb.Rows[0];
                textBox1.Text = dr[6].ToString();
                textBox2.Text = dr[7].ToString();
                textBox3.Text = dr[8].ToString();
                textBox6.Text = dr[9].ToString();
                textBox7.Text = dr[10].ToString();
                textBox8.Text = dr[11].ToString();


                DataRow accountnumberrow = tb4.Rows[0];
                account = accountnumberrow[12].ToString();
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void bucketdespues() {
            
        }

        private void button1_Click(object sender, EventArgs e) {
            aprobarcotizacion();
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void button4_Click(object sender, EventArgs e) {
            FilesPO fr = new FilesPO();
            fr.poid = idpo;
            fr.usuario = usuario;
            fr.ShowInTaskbar = false;
            fr.ShowDialog();
        }
        private void CreateRequi_Load(object sender, EventArgs e) {
            getdata();
            bucketdespues();
            textBox5.Text = usuario;
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            setantes();
            actualizarconsumo();
        }
        private void pictureBox1_Click(object sender, EventArgs e) {
            aprobarcotizacion();
            Close();
        }
        private void pictureBox2_Click(object sender, EventArgs e) {
            disapprovecot();
            Close();
        }
    }
}