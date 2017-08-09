using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CreateSKU : Form {
        public CreateSKU() {
            InitializeComponent();
        }
        public string usuario { get; set; }

        private void button1_Click(object sender, EventArgs e) {
            if (textBox1.Text.Length >= 100) {
                MessageBox.Show("Numero de parte demasiado extenso");
            } else if (textBox2.Text.Length >= 300) {
                MessageBox.Show("Descripcion demasiado extensa");
            } else {
                setcrearsku();
            }
        }
        public void getcategorias() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT id, category FROM tbsku_cat";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);
                comboBox1.Items.Clear();
                foreach (DataRow dr in datatable.Rows) {
                    comboBox1.Items.Add(dr[1].ToString());
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void setcrearsku() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                if (textBox1.Text == "" || textBox2.Text == "" || textBox8.Text == "" || textBox9.Text == "") {
                    MessageBox.Show("Te hace falta algunos campos por llenar");
                    return;
                }
                string sqlquery = "INSERT INTO SKU (sku, skudesc, createdate, createdby, category) VALUES ('" + textBox1.Text + "','" +
                        textBox2.Text + "','" + textBox9.Text + "','" + textBox8.Text + "','" + comboBox1.SelectedItem.ToString() + "')";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                textBox1.Text = "";
                textBox2.Text = "";
                textBox9.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateDepto_Load(object sender, EventArgs e) {
            textBox9.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            textBox8.Text = usuario;
            getcategorias();
            comboBox1.SelectedIndex = 0;
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            if (textBox1.Text.Contains("'")) {
                textBox1.Text = "";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {
            if (textBox2.Text.Contains("'")) {
                textBox2.Text = "";
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (Regex.IsMatch(e.KeyChar.ToString(), @"[^a-z^A-Z^0-9^ ^+^\-^\/^\b^\*^\(^\)]")) {
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
            if (Regex.IsMatch(e.KeyChar.ToString(), @"[^a-z^A-Z^0-9^ ^+^\-^\/^\b^\*^\(^\)]")) {
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }
    }
}
