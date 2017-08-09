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
    public partial class SKU : Form {
        public SKU() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_id { get; set; }
        public string user_depto { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "";
                if (comboBox1.SelectedItem.ToString() == "Todas") {
                    sqlquery = "SELECT * from sku";
                } else {
                    sqlquery = "SELECT * FROM sku WHERE category = '" + comboBox1.SelectedItem.ToString() + "'";
                }
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
            getcategorias();
            comboBox1.SelectedIndex = 0;
        }
        private void getcategorias() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT category FROM tbsku_cat";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);
                conn.Close();
                comboBox1.Items.Add("Todas");
                foreach (DataRow dr in datatable.Rows) {
                    comboBox1.Items.Add(dr[0].ToString());
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            CreateSKU cs = new CreateSKU();
            cs.usuario = usuario;
            cs.ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if (dataGridView1.Columns[e.ColumnIndex].Index == 1 || dataGridView1.Columns[e.ColumnIndex].Index == 2) {
                    ChangeSKU cs = new ChangeSKU();
                    cs.skuid = dataGridView1[0, e.RowIndex].Value.ToString();
                    cs.sku = dataGridView1[1, e.RowIndex].Value.ToString();
                    cs.desc = dataGridView1[2, e.RowIndex].Value.ToString();
                    cs.FormClosed += Cs_FormClosed;
                    cs.ShowDialog();
                }
            } catch (Exception) { }
        }
        private void Cs_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }
    }
}
