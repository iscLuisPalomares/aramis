using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class EditUser : Form {
        public EditUser() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string user_id { get; set; }
        public string deptoid { get; set; }
        
        public void updateuser() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "UPDATE users SET "
                    + "depto = " + (comboBox1.SelectedItem as ComboboxItem).Value + " "
                    + "WHERE id = " + user_id + " ;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Actualizado", "Listo");
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateUser_Load(object sender, EventArgs e) {
            getdeptos();
        }
        private void button1_Click(object sender, EventArgs e) {
            updateuser();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void getdeptos() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT id as 'ID', name as 'Nombre' FROM deptos;";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow dr in table.Rows) {
                    ComboboxItem item = new ComboboxItem();
                    item.Text = dr["Nombre"].ToString();
                    item.Value = dr["ID"].ToString();
                    comboBox1.Items.Add(item);
                }

                sqlquery = "SELECT depto as 'Departamento' FROM users WHERE id = " + user_id + ";";
                SqlDataAdapter adapterdepto = new SqlDataAdapter(sqlquery, conn);
                DataTable tabledepto = new DataTable();
                adapterdepto.Fill(tabledepto);
                string deptoid = tabledepto.Rows[0]["Departamento"].ToString();
                int indexparacombobox = 0;
                for (int i = 0; i <= comboBox1.Items.Count - 1;i++) {
                    if ((comboBox1.Items[i] as ComboboxItem).Value.ToString() == deptoid) {
                        indexparacombobox = i;
                        break;
                    }
                }
                comboBox1.SelectedIndex = indexparacombobox;
                conn.Close();
            } catch (Exception) {

            }
        }
    }
}
