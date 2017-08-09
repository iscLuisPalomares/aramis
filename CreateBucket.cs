using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CreateBucket : Form {
        public CreateBucket() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_id { get; set; }
        public string accountid { get; set; }

        
        public void getcuentas() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                string sqlalmacenistas = "select acct.id as 'ID Cuenta', acct.acctnumber as 'Numero de Cuenta' "
                    + ", acct.acctdesc as 'Descripcion de Cuenta', deps.name as 'Departamento' "
                    + ", deps.id as 'Depto ID' from Accounts acct join Deptos deps on deps.id = acct.depto";

                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);

                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();

                adapteralmacen.Fill(almacentb);
                ComboBoxBucketsAccounts bucketacct = new ComboBoxBucketsAccounts();
                foreach (DataRow da in almacentb.Rows) {
                    bucketacct = new ComboBoxBucketsAccounts();
                    bucketacct.fsid = da[0].ToString();
                    bucketacct.fsacctnumber = da[1].ToString();
                    bucketacct.fsacctdesc = da[2].ToString();
                    bucketacct.fsdepto = da[3].ToString();
                    bucketacct.fsdeptoid = da[4].ToString();
                    comboBox2.Items.Add(bucketacct);
                }
                comboBox2.SelectedIndex = 0;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void getperiods() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                string sqlalmacenistas = "SELECT * FROM acctperiods";
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);
                foreach (DataRow da in almacentb.Rows) {
                    comboBox1.Items.Add(da[1].ToString());
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void setcreatebucket() {
            try {
                string connectionstring = Program.stringconnection;

                if (comboBox1.SelectedItem.ToString() == "" || textBox3.Text == "" || textBox4.Text == "") {
                    MessageBox.Show("Te hacen falta algunos campos por llenar");
                    return;
                }
                double balance = 0;
                try {
                    balance = double.Parse(textBox1.Text) - double.Parse(textBox2.Text) - double.Parse(textBox6.Text);
                } catch (Exception e) {
                    MessageBox.Show(e.ToString());
                }
                if (bucketexists()) {
                    MessageBox.Show("Ya existe o se presento un problema al crear bucket");
                } else {
                    SqlConnection conn = new SqlConnection(connectionstring);
                    conn.Open();
                    string sqlquery = "INSERT INTO buckets (id_cuenta, budget, periodo"
                        + ", ajustado, gasto, asignado, balance) VALUES (" + (comboBox2.SelectedItem as ComboBoxBucketsAccounts).fsid + ",'" 
                        + textBox3.Text + "','" + comboBox1.SelectedItem.ToString() + "', '" + textBox1.Text + "', " + textBox2.Text + ",' " + textBox6.Text + "','" + balance.ToString() + "')";
                    SqlCommand ejecucion = new SqlCommand();
                    ejecucion.Connection = conn;
                    ejecucion.CommandType = CommandType.Text;
                    ejecucion.CommandText = sqlquery;
                    ejecucion.ExecuteNonQuery();
                    conn.Close();
                    textBox3.Text = "";
                    textBox6.Text = "";
                    textBox2.Text = "";
                    textBox1.Text = "";
                    textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    MessageBox.Show("Nuevo bucket creado", "Listo");
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public bool bucketexists() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT COUNT(id_bucket) FROM buckets "
                    + "WHERE id_cuenta = " + (comboBox2.SelectedItem as ComboBoxBucketsAccounts).fsid 
                    + " AND periodo = '" + comboBox1.SelectedItem.ToString() + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                if (int.Parse(table.Rows[0][0].ToString()) > 0) {
                    return true;
                } else {
                    return false;
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                return true;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            setcreatebucket();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void CreateAccount_Load(object sender, EventArgs e) {
            getperiods();
            getcuentas();
            textBox5.Text = usuario;
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}
