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
    public partial class ConfLineaDel : Form {
        public ConfLineaDel() {
            InitializeComponent();
        }
        public string nodelinea { get; set; }

        private void button1_Click(object sender, EventArgs e) {
            cancelar_linea();
        }
        private void cancelar_linea() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION;\n";
                sqlquery += "UPDATE materialrequerido SET fsstatus = 'Cancelado' WHERE fsid = " + nodelinea + ";\n";
                sqlquery += "COMMIT TRANSACTION;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Linea Cancelada", "Listo");
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void ConfLineaDel_Load(object sender, EventArgs e) {
            label2.Text = nodelinea;
        }
    }
}
