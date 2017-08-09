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
    public partial class EditDepto : Form {
        public EditDepto() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string user_id { get; set; }
        public string deptoid { get; set; }
        
        public void updatedeptos() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "UPDATE deptos SET "
                    + "name = '" + textBox1.Text + "', "
                    + "moddesc = '" + textBox2.Text + "', "
                    + "gerente = " + (comboBox1.SelectedItem as ComboboxItem).Value + " "
                    + "WHERE id = " + deptoid + ";";
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
            getusers();
        }
        private void button1_Click(object sender, EventArgs e) {
            updatedeptos();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void getusers() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT id as 'ID', fulname as 'Nombre' FROM users WHERE tipo = 'Aprobador'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow dr in table.Rows) {
                    ComboboxItem item = new ComboboxItem();
                    item.Text = dr["Nombre"].ToString();
                    item.Value = dr["ID"].ToString();
                    comboBox1.Items.Add(item);
                }

                sqlquery = "SELECT name as 'Nombre', moddesc as 'Descripcion', gerente as 'Gerente' FROM deptos WHERE id = " + deptoid + ";";
                SqlDataAdapter adapterdepto = new SqlDataAdapter(sqlquery, conn);
                DataTable tabledepto = new DataTable();
                adapterdepto.Fill(tabledepto);
                textBox1.Text = tabledepto.Rows[0]["Nombre"].ToString();
                textBox2.Text = tabledepto.Rows[0]["Descripcion"].ToString();
                string gerenteid = tabledepto.Rows[0]["Gerente"].ToString();

                int indexparacombobox = 0;
                for (int i = 0; i <= comboBox1.Items.Count - 1;i++) {
                    if ((comboBox1.Items[i] as ComboboxItem).Value.ToString() == gerenteid) {
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
