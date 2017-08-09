using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ChangeDivisa : Form {
        public ChangeDivisa() {
            InitializeComponent();
        }
        public string idlinea       { get; set; }
        public string qty           { get; set; }
        public string usuario       { get; set; }
        public string user_id       { get; set; }
        public string saldo         { get; set; }
        public string recibido      { get; set; }
        public string cuenta        { get; set; }
        public string costounidad   { get; set; }
        public string idpo          { get; set; }
        public string accountid     { get; set; }
        public string requisicionid { get; set; }
        public string bucketid      { get; set; }
        public double costorecibo;
        
        private void button1_Click(object sender, EventArgs e) {
            try {
                double cambio = double.Parse(textBox2.Text);
                SqlConnection conn = new SqlConnection(Program.stringconnection);
                conn.Open();
                string sqlquery = "INSERT INTO tbtipodecambio (valor, fecha, usuario) "
                    + "VALUES (" + cambio + ", GETDATE(), '" + usuario + "')";
                SqlCommand com = new SqlCommand(sqlquery, conn);
                com.ExecuteNonQuery();
                MessageBox.Show("Actualizado");
                conn.Close();
                Close();
            } catch (Exception) {
                MessageBox.Show("Se presento un problema");
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
