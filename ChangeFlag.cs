using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ChangeFlag : Form {
        public ChangeFlag() {
            InitializeComponent();
        }
        public string aslid     { get; set; }
        public string flag      { get; set; }
        public string suppname    { get; set; }
        public string suppcity { get; set; }
        public string suppcontactname { get; set; }
        public string suppemail { get; set; }
        public string suppphone { get; set; }
        public string pais { get; set; }

        public void setflag() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "";
                if (checkBox1.Checked) {
                    sqlquery = "UPDATE asl SET "
                        + "flag = 1, "
                        + "suppname = '" + textBox1.Text + "', "
                        + "suppcity = '" + textBox2.Text + "', "
                        + "suppcontactname = '" + textBox3.Text + "', "
                        + "suppemail = '" + textBox4.Text + "', "
                        + "suppphone = '" + textBox5.Text + "', "
                        + "Pais = '" + comboBox1.SelectedItem.ToString() + "' "
                        + "WHERE id = " + aslid + "";
                } else {
                    sqlquery = "UPDATE asl SET "
                        + "flag = 0, "
                        + "suppname = '" + textBox1.Text + "', "
                        + "suppcity = '" + textBox2.Text + "', "
                        + "suppcontactname = '" + textBox3.Text + "', "
                        + "suppemail = '" + textBox4.Text + "', "
                        + "suppphone = '" + textBox5.Text + "', "
                        + "Pais = '" + comboBox1.SelectedItem.ToString() + "' "
                        + "WHERE id = " + aslid + "";
                }
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
            textBox1.Text = suppname;
            textBox2.Text = suppcity;
            textBox3.Text = suppcontactname;
            textBox4.Text = suppemail;
            textBox5.Text = suppphone;
            if (pais == "MEXICO") { comboBox1.SelectedIndex = 0; }
            if (pais == "USA") { comboBox1.SelectedIndex = 1; }
            if (pais == "CHINA") { comboBox1.SelectedIndex = 2; }

            if (flag == "1") { checkBox1.Checked = true; } 
            else if (flag == "" || flag == "0" || flag == null) { checkBox1.Checked = false; }
        }
        private void button1_Click(object sender, EventArgs e) {
            setflag();
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
