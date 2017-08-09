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
    public partial class EditDelegado : Form {
        public EditDelegado() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string user_id { get; set; }
        public string delegadoid { get; set; }
        public string delegadoname { get; set; }

        public void removepermit() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "UPDATE tbdelegados SET fsapproveajustes = 0 WHERE fsid = " + delegadoid + ";";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Actualizado", "Listo");
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateUser_Load(object sender, EventArgs e) {
            label2.Text = delegadoname;
        }
        private void button1_Click(object sender, EventArgs e) {
            removepermit();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
