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
    public partial class ChangeSKU : Form {
        public ChangeSKU() {
            InitializeComponent();
        }
        public string skuid { get; set; }
        public string sku { get; set; }
        public string desc { get; set; }
        public string usuario { get; set; }
        public string user_id { get; set; }
        
        private void getcategories() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT category FROM tbsku_cat";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);
                conn.Close();
                foreach (DataRow dr in datatable.Rows) {
                    comboBox1.Items.Add(dr[0].ToString());
                }
                comboBox1.SelectedIndex = 0;
            } catch (Exception) { }
        }
        public void setadjustment() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "UPDATE sku SET "
                    + "sku = '" + textBox1.Text + "', "
                    + "skudesc = '" + textBox2.Text + "', "
                    + "category = '" + comboBox1.SelectedItem.ToString() + "' "
                    + "WHERE id='" + skuid + "'";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Actualizado", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateUser_Load(object sender, EventArgs e) {
            textBox1.Text = sku;
            textBox2.Text = desc;
            getcategories();
        }
        private void button1_Click(object sender, EventArgs e) {
            setadjustment();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
