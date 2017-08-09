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
    public partial class CreateDepto : Form {
        public CreateDepto() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            setcreardepto();
        }
        public string user_id { set; get; }
        public string usuario { get; set; }

        public void setadjustmenttrigger() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "INSERT INTO bitacora (id_usuario, usuario, operacion, tabla, fecha) VALUES ('" +
                    user_id + "','" +
                    usuario + "','" +
                    "DPARTMENT CREATED " + "','" +
                    "DEPARTAMENTOS" + "','" +
                    DateTime.Now.ToString() + "')";
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
        public void setcreardepto() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "") {
                    MessageBox.Show("Te hace falta algunos campos por llenar");
                    return;
                }
                string sqlquery = "INSERT INTO Deptos (name, moddesc, createdate, createdby, gerente) "
                    + "VALUES ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + user_id + "', " + (comboBox1.SelectedItem as ComboboxItem).Value + ")";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateDepto_Load(object sender, EventArgs e) {
            textBox3.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            textBox4.Text = usuario;
            getusers();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void getusers() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT id as 'ID', fulname as 'Nombre' FROM users";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow dr in table.Rows) {
                    ComboboxItem item = new ComboboxItem();
                    item.Text = dr["Nombre"].ToString();
                    item.Value = dr["ID"].ToString();
                    comboBox1.Items.Add(item);
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
            } catch (Exception) {

            }
        }
    }
}
