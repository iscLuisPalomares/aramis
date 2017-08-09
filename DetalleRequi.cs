using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class DetalleRequi : Form {
        public DetalleRequi() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string idreq { get; set; }
        public string account { get; set; }

        List<string> files = new List<string>();
        List<string> codigos = new List<string>();
        List<string> unidadesdemedida = new List<string>();

        public string getaccountid() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT id FROM accounts where acctnumber = '" + textBox3.Text + "'";
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
        public void getbag() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT balance, budget, ajustado, gasto, asignado, id_bucket FROM buckets WHERE id_cuenta = '" + getaccountid() + "' AND periodo = '" + DateTime.Now.ToString("yyyy-MM") + "'";
                string sqlquery2 = "select Deptos.name from accounts left join deptos on accounts.depto = Deptos.id where acctnumber = '" + account + "'";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlquery2, conn);
                DataTable tb = new DataTable();
                DataTable datatable2 = new DataTable();
                adapter.Fill(tb);
                adapter2.Fill(datatable2);
                try {
                    DataRow dr = datatable2.Rows[0];
                    label9.Text = "Departamento: " + dr[0].ToString();
                    DataRow da = tb.Rows[0];

                    label20.Text = da[4].ToString();
                    label19.Text = da[2].ToString();
                    label18.Text = da[3].ToString();
                    label17.Text = da[1].ToString();
                    label16.Text = da[0].ToString();
                } catch (Exception) {
                    label7.Text = "Balance: ";
                }

                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT * FROM requisiciones WHERE id_req = '" + idreq.ToString() + "'";
                string sqlquery3 = "SELECT name FROM Deptos LEFT JOIN accounts ON deptos.id = Accounts.depto WHERE accounts.acctnumber = '" + account + "'";
                string sqlquery4 = "SELECT * FROM materialrequerido WHERE fsrequisicion = '" + idreq + "'";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter3 = new SqlDataAdapter(sqlquery3, conn);
                SqlDataAdapter adapter4 = new SqlDataAdapter(sqlquery4, conn);
                DataTable tb = new DataTable();
                DataTable tb3 = new DataTable();
                DataTable tb4 = new DataTable();
                adapter.Fill(tb);
                adapter3.Fill(tb3);
                adapter4.Fill(tb4);
                dataGridView1.DataSource = tb4;
                try {
                    DataRow dr = tb.Rows[0];
                    textBox2.Text = dr[6].ToString();
                    label9.Text = "Departamento: " + tb3.Rows[0][0].ToString();
                    textBox3.Text = account;
                    textBox1.Text = dr[10].ToString();
                    if (dr[9].ToString() == "true") {
                        checkBox1.Checked = true;
                    } else {
                        checkBox1.Checked = false;
                    }
                } catch (Exception) {
                    label7.Text = "Se presento un error";
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void bucketdespues() {
            try {
                double cbalance = double.Parse(label16.Text);
                double cbudget = double.Parse(label17.Text);
                double cgasto = double.Parse(label18.Text);
                double cajustado = double.Parse(label19.Text);
                double casignado = double.Parse(label20.Text);
                if (dataGridView1.RowCount >= 1) {
                    foreach (DataGridViewRow dr in dataGridView1.Rows) {
                        if (!dr.IsNewRow) {
                            casignado = casignado + double.Parse(dr.Cells[8].Value.ToString());
                            cbalance = cajustado - cgasto - casignado;
                        }
                    }
                } else {
                    cbalance = cajustado - cgasto - casignado;
                }

                label25.Text = cbalance.ToString();
                label24.Text = cbudget.ToString();
                label23.Text = cgasto.ToString();
                label22.Text = cajustado.ToString();
                label21.Text = casignado.ToString();
            } catch (Exception) {
                label25.Text = "";
                label24.Text = "";
                label23.Text = "";
                label22.Text = "";
                label21.Text = "";
                MessageBox.Show("error");
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void CreateRequi_Load(object sender, EventArgs e) {
            getdata();
            getbag();
            bucketdespues();
            textBox5.Text = usuario;
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            getbag();
        }
        private void button4_Click(object sender, EventArgs e) {
            FilesRequi fr = new FilesRequi();
            fr.reqid = idreq;
            fr.usuario = usuario;
            fr.ShowInTaskbar = false;
            fr.ShowDialog();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            try {

            } catch (Exception) {
                
            }
        }
    }
}