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
    public partial class EditLinea : Form {
        public EditLinea() {
            InitializeComponent();
        }

        public string lineaid = "";
        public string usuario = "";
        private void EditLinea_Load(object sender, EventArgs e) {
            getdata();
            getunidadesdemedida();
        }

        public void getunidadesdemedida() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsdesc from tbunimedida";
                string querycategorias = "select category from tbsku_cat";
                string querycategoriasreporte = "select fsdescripcion from tbcategorias_reportesku order by fsdescripcion";

                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adaptercat = new SqlDataAdapter(querycategorias, conn);
                SqlDataAdapter adaptercatreporte = new SqlDataAdapter(querycategoriasreporte, conn);
                DataTable table = new DataTable();
                DataTable tablecats = new DataTable();
                DataTable tablecats_reporte = new DataTable();
                adapter.Fill(table);
                adaptercat.Fill(tablecats);
                adaptercatreporte.Fill(tablecats_reporte);
                conn.Close();
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                foreach (DataRow dr in table.Rows) {
                    comboBox1.Items.Add(dr[0].ToString());
                }
                foreach (DataRow dr in tablecats.Rows) {
                    comboBox2.Items.Add(dr[0].ToString());
                }
                foreach (DataRow dr in tablecats_reporte.Rows) {
                    comboBox3.Items.Add(dr[0].ToString());
                }
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid, fsrequisicion, fscodigo, fsdesc, fsunimedida "
                    + "from materialrequerido where fsid = @lineaid ;";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@lineaid", lineaid);
                DataTable table = new DataTable();
                adapter.Fill(table);
                textBox1.Text = table.Rows[0][0].ToString();
                textBox2.Text = table.Rows[0][1].ToString();
                textBox3.Text = table.Rows[0][2].ToString();
                textBox4.Text = table.Rows[0][3].ToString();
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (skuexists()) {
                setentity();
            } else {
                dardealta();
                setentity();
            }
        }
        private void dardealta() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "insert into sku (sku, skudesc, createdate, createdby, category) values "
                    + "(@sku, @desc, getdate(), @username, @categoria)";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.Parameters.AddWithValue("@sku", textBox3.Text);
                ejecucion.Parameters.AddWithValue("@desc", textBox4.Text);
                ejecucion.Parameters.AddWithValue("@username", usuario);
                ejecucion.Parameters.AddWithValue("@categoria", comboBox2.SelectedItem.ToString());
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
            } catch (Exception) {
                MessageBox.Show("No se pudo dar de alta");
                Close();
            }
        }
        private bool skuexists() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select id from sku where sku = @sku";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.Add(new SqlParameter("@sku", textBox3.Text));
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                if (table.Rows.Count >= 1) { return true; } else { return false; }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void setentity() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "update materialrequerido set "
                    + "fscodigo = @sku, fsdesc = @desc, fsunimedida = @um "
                    + "where fsid = @id;\n"
                    + "update sku set fscategoria_reportesku = @skucatreporte where sku = @sku;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.Parameters.AddWithValue("@sku", textBox3.Text);
                ejecucion.Parameters.AddWithValue("@desc", textBox4.Text);
                ejecucion.Parameters.AddWithValue("@um", comboBox1.SelectedItem.ToString());
                ejecucion.Parameters.AddWithValue("@id", textBox1.Text);
                ejecucion.Parameters.AddWithValue("@skucatreporte", comboBox3.SelectedItem.ToString());
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Linea de requisición actualizada.");
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
