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
    public partial class ChangePeriod : Form
    {
        public ChangePeriod()
        {
            InitializeComponent();
        }
        public string id_bucket { get; set; }
        public void getdeptos()
        {
            try
            {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();

                string sqlalmacenistas = "SELECT * FROM acctperiods";
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);
                
                foreach (DataRow da in almacentb.Rows)
                {
                    comboBox1.Items.Add(da[1].ToString());
                }
                comboBox1.SelectedIndex = 0;

                conn.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }
        
        public void setperiod()
        {
            try
            {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                
                string sqlquery = "UPDATE buckets SET periodo = '"+comboBox1.SelectedItem.ToString()+"' WHERE id_bucket='"+id_bucket+"'";
                
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Periodo Actualizado", "Listo");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateUser_Load(object sender, EventArgs e)
        {
            getdeptos();
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            setperiod();
        }

        private void CreateUser_SizeChanged(object sender, EventArgs e)
        {
            panel1.Location = new Point(0, this.Height / 2);
            panel1.Height = Height;
            panel1.Width = Width;
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
