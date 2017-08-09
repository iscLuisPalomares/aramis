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
    public partial class CreateAccount : Form {
        public CreateAccount() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            setcreateaccount();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public void setcreateaccount() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                if (textBox1.Text == "" || comboBox1.SelectedItem.ToString() == "" || textBox3.Text == "" || textBox4.Text == "") {
                    MessageBox.Show("Te hace falta algunos campos por llenar");
                    return;
                }
                string sqlquery = "INSERT INTO Accounts (acctnumber, depto, acctdesc, createdate, createdby) VALUES ('" + textBox1.Text + "','" +
                        comboBox1.SelectedItem.ToString().Split('|')[1] + "','" + textBox3.Text + "','" + textBox4.Text + "','" + textBox5.Text + "')";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                textBox1.Text = "";
                textBox3.Text = "";
                textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MessageBox.Show("Nueva cuenta creada", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public string user_id { get; set; }
        public void setadjustmenttrigger() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "INSERT INTO bitacora (id_usuario, usuario, operacion, tabla, fecha) VALUES ('" +
                    user_id + "','" +
                    usuario + "','" +
                    "ACCOUNT CREATED " + "','" +
                    "Accounts" + "'," +
                    "GETDATE()" + ")";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        public void getdeptos() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                string sqlalmacenistas = "SELECT * FROM Deptos";

                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);

                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();

                adapteralmacen.Fill(almacentb);

                foreach (DataRow da in almacentb.Rows) {
                    comboBox1.Items.Add(da[1].ToString() + "|" + da[0].ToString());
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void CreateAccount_Load(object sender, EventArgs e) {
            getdeptos();
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            textBox5.Text = usuario;
        }
    }
}
