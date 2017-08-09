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

namespace ComprasProject
{
    public partial class ChangeBudget : Form
    {
        public ChangeBudget()
        {
            InitializeComponent();
        }
        public string id_bucket
        {
            get;
            set;
        }
        public string ajustado_actual
        {
            get;
            set;
        }
        public string usuario
        {
            get;
            set;
        }
        public string user_id
        {
            get;
            set;
        }

        public void setadjustment()
        {
            try
            {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                
                string sqlquery = "UPDATE buckets SET budget = '"+textBox1.Text+"' WHERE id_bucket='"+id_bucket+"'";
                
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Actualizado", "Listo");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void setadjustmenttrigger()
        {
            try
            {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "INSERT INTO bitacora (id_usuario, usuario, operacion, tabla, fecha, value) VALUES ('" +
                    user_id + "','" +
                    usuario + "','" +
                    "UPDATE BUDGET" + "','" +
                    "Buckets" + "'," +
                    "GETDATE()" + ",'" +
                    textBox1.Text + "')";
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

        private void CreateUser_Load(object sender, EventArgs e)
        {
            textBox1.Text = ajustado_actual;
        }
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            setadjustment();
            setadjustmenttrigger();
        }

        private void CreateUser_SizeChanged(object sender, EventArgs e)
        {
            
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
