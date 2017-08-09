using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class AddDelegado : Form {
        public AddDelegado() {
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
                string sqlquery = "SELECT id as 'ID', fulname as 'Nombre' FROM users WHERE tipo = 'Aprobador'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);
                conn.Close();
                foreach (DataRow dr in datatable.Rows) {
                    try {
                        ComboboxItem n = new ComboboxItem();
                        n.Text = dr["Nombre"].ToString();
                        n.Value = dr["ID"].ToString();
                        comboBox1.Items.Add(n);
                    } catch (Exception ex) {
                        MessageBox.Show(ex.ToString());
                    }
                }
                comboBox1.SelectedIndex = 0;
                
            } catch (Exception) { }
        }
        public void setpermits() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "IF (NOT EXISTS (SELECT * FROM tbdelegados WHERE "
                    + "fsuserid = @combo1value ))\n"
                    + "BEGIN\n"
                    + "INSERT INTO tbdelegados (fsuserid, " 
                    + ((comboBox2.SelectedItem as ComboboxItem).Value.ToString()) + ", fsvencimiento) "
                    + "VALUES(@combo1value, 1, @date1);\n"
                    + "END\n"
                    + "ELSE\n"
                    + "BEGIN\n"
                    + "UPDATE tbdelegados SET " + ((comboBox2.SelectedItem as ComboboxItem).Value.ToString()) + " = 1, "
                    + "fsvencimiento = @date1 WHERE "
                    + "fsuserid = @combo1value ;\n"
                    + "END";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.Parameters.AddWithValue("@combo1value", (comboBox1.SelectedItem as ComboboxItem).Value);
                ejecucion.Parameters.AddWithValue("@date1", dateTimePicker1.Value.ToString("yyyy-MM-dd") 
                    + " " + dateTimePicker2.Value.ToString("HH:mm:ss"));
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Actualizado", "Listo");
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateUser_Load(object sender, EventArgs e) {
            getcategories();
            ComboboxItem n = new ComboboxItem();
            n.Text = "Aprobar Ajustes";
            n.Value = "fsapproveajustes";
            comboBox2.Items.Add(n);
            comboBox2.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e) {
            setpermits();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
    public class ComboboxItem {
        public string Text { get; set; }
        public string Value { get; set; }

        public override string ToString() {
            return Text;
        }
    }
}
