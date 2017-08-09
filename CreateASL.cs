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

namespace ComprasProject
{
    public partial class CreateASL : Form
    {
        public CreateASL()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            setcrearasl();
            setadjustmenttrigger();
        }
        public string usuario { get; set; }
        public string user_id { set; get; }
        public void setadjustmenttrigger()
        {
            try
            {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "INSERT INTO bitacora (id_usuario, usuario, operacion, tabla, fecha) VALUES ('" +
                    user_id + "','" +
                    usuario + "','" +
                    "ASL CREATED " + "','" +
                    "ASL" + "'," +
                    "GETDATE()" + ")";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void setcrearasl()
        {
            try
            {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || comboBox2.SelectedItem.ToString() == "" || textBox5.Text == "" || textBox6.Text == "" || textBox7.Text == "" || textBox8.Text == "" || textBox9.Text == "" || comboBox1.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("Te hace falta algunos campos por llenar");
                    return;
                }
                string sqlquery = "INSERT INTO ASL (suppname, suppdesc, suppcity, exportflag, suppcontactname, " +
                    "suppemail, suppphone,  createdate, createdby, Pais) VALUES ('" + textBox1.Text + "','" +
                        textBox2.Text + "','" + textBox3.Text + "','" + comboBox2.SelectedItem.ToString() + "', '" + textBox5.Text + 
                        "', '" + textBox6.Text + "','" + textBox7.Text + "','"+ textBox9.Text +"','" +textBox8.Text+ "','"+ comboBox1.SelectedItem.ToString()+"')";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
                textBox9.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateDepto_Load(object sender, EventArgs e)
        {
            textBox9.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            textBox8.Text = usuario;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
