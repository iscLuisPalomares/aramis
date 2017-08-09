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
    public partial class EditCategoriaReportes : Form {
        public EditCategoriaReportes() {
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
                string querycategoriasreporte = "select fsdescripcion from tbcategorias_reportesku order by fsdescripcion";

                SqlDataAdapter adaptercatreporte = new SqlDataAdapter(querycategoriasreporte, conn);
                DataTable tablecats_reporte = new DataTable();
                adaptercatreporte.Fill(tablecats_reporte);
                conn.Close();
                comboBox3.Items.Clear();
                foreach (DataRow dr in tablecats_reporte.Rows) {
                    comboBox3.Items.Add(dr[0].ToString());
                }
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
            setentity();
        }
        

        public void setentity() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "update sku set fscategoria_reportesku = @skucatreporte where sku = @sku;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.Parameters.AddWithValue("@sku", textBox3.Text);
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
