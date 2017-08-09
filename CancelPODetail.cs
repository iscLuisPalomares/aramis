using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CancelPODetail : Form {
        public CancelPODetail() {
            InitializeComponent();
        }

        public string idpo          { get; set; }
        public string usuario       { get; set; }
        public string account       { get; set; }
        public string accountid     { get; set; }
        public double costototal    { get; set; }
        public string user_id       { get; set; }

        private string getaccountid(string accountnum) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT id FROM accounts where acctnumber = '" + accountnum + "'";
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);
                string number = "";
                DataRow da = almacentb.Rows[0];
                number = da[0].ToString();
                conn.Close();
                return number;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
                return "";
            }
        }
        private void cancelarpo() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION\n";
                sqlquery += "UPDATE tblpurchaseorders SET datecancel = " + "GETDATE()" + ""
                    + ", canceledby = '" + user_id + "', " +
                    "fsstatus = 'PO Cancelado' WHERE fsid = '" + idpo + "';\n";
                foreach (DataGridViewRow dr in dataGridView1.Rows) {
                    sqlquery += "UPDATE materialrequerido SET fsstatus = 'PO Cancelado' WHERE fsid = " 
                        + dr.Cells["ID"].Value.ToString() + ";\n";
                    
                }
                sqlquery += "update "
                    + "buckets set buckets.gasto = gastos.[Total cotizado dlls], buckets.asignado = 0 FROM buckets bucks "
                    + "INNER JOIN(SELECT bucketid, sum(fsabsolutodlls) AS 'Total estimado dlls' "
                    + ", sum(absolutdllscot) AS 'Total cotizado dlls', fsstatus FROM materialrequerido "
                    + "WHERE fsstatus = 'PO Recibido' GROUP BY fsstatus, bucketid) gastos "
                    + "ON bucks.id_bucket = gastos.bucketid update "
                    + "buckets set buckets.asignado = ("
                    + "CASE WHEN asignados.[Total cotizado] is null OR asignados.[Total cotizado] = 0 "
                    + "then asignados.[Total estimado dlls] "
                    + "else asignados.[Total cotizado] end) "
                    + "FROM buckets bucks INNER JOIN("
                    + "SELECT bucketid, sum(fsabsolutodlls) AS 'Total estimado dlls', sum(absolutdllscot) as 'Total cotizado' "
                    + "FROM materialrequerido WHERE fsstatus in ('Requisicion Creada', 'Requisicion Aprobada' "
                    + ", 'Cotizacion Creada', 'Cotizacion Aprobada', 'PO Creado', 'PO Recibiendo'"
                    + ", 'PO Aprobado', 'PO creado') GROUP BY bucketid "
                    + ") asignados ON bucks.id_bucket  = asignados.bucketid "
                    + "update buckets set balance = ajustado - gasto - asignado \n";
                sqlquery += "COMMIT;";
                SqlCommand ejecucion = new SqlCommand();
                ejecucion.Connection = conn;
                ejecucion.CommandType = CommandType.Text;
                ejecucion.CommandText = sqlquery;
                ejecucion.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("PO Cancelado", "Listo");
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
       
        private string getreqnums() {
            string numeros = "";
            foreach (DataGridViewRow dr in dataGridView1.Rows) {
                numeros += "'" + dr.Cells[12].Value.ToString() + "',";
                if (!dr.IsNewRow) {

                }
            }
            string nuevo = numeros.Remove(numeros.Length - 1, 1);
            return nuevo;
        }

        private void sendmail() {
            string gerentemail = getgerente();
            MailMessage mail = new MailMessage("aramis@posey.com", gerentemail);
            MessageBox.Show(gerentemail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "Cotizacion cancelada";
            mail.Body = "Se ha cancelado una cotizacion.";
            try {
                client.Send(mail);
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
        }
        private string getgerente() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlalmacenistas = "select correo from users where " +
                    "id = (select gerente from deptos where " +
                    "id = (select depto from users where id = " + user_id + "))";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable tabla = new DataTable();
                adapter.Fill(tabla);
                DataRow dr = tabla.Rows[0];
                conn.Close();
                return dr[0].ToString();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
                return "";
            }
        }

        private void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "select fsid as 'ID', comprador.fulname as 'Comprador', "
                    + "fscomments as 'Comentarios', supplier.suppname as 'Proveedor', "
                    + "fsterms as 'Terminos', fsdaterequired as 'Fecha Requerida', fsobservaciones as 'Observaciones' from tblPurchaseOrders "
                    + "join users comprador on comprador.id = fsbuyer "
                    + "join asl supplier on supplier.id = fsvendor WHERE fsid = '" + idpo + "'";
                string sqlquery4 = "select fsid as 'ID', fscodigo as SKU, fsdesc as Descripcion, "
                    + "fsunimedida as 'U/M', absolutdllscot as Costo, bucketid as Bucket, fscuenta as 'Cuenta' "
                    + "from materialrequerido where fspurchaseorder = '" + idpo + "'";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                SqlDataAdapter adapter4 = new SqlDataAdapter(sqlquery4, conn);
                DataTable tb = new DataTable();
                DataTable tb4 = new DataTable();
                adapter.Fill(tb);
                adapter4.Fill(tb4);
                dataGridView1.DataSource = tb4;
                //Get purchase order information set in all textboxs
                DataRow dr = tb.Rows[0];
                textBox1.Text = dr["Comprador"].ToString();
                textBox2.Text = dr["Comentarios"].ToString();
                textBox3.Text = dr["Proveedor"].ToString();
                textBox6.Text = dr["Terminos"].ToString();
                textBox7.Text = dr["Fecha Requerida"].ToString();
                textBox8.Text = dr["Observaciones"].ToString();


                DataRow accountnumberrow = tb4.Rows[0];
                account = accountnumberrow["Cuenta"].ToString();
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void button4_Click(object sender, EventArgs e) {
            FilesPO fr = new FilesPO();
            fr.poid = idpo;
            fr.usuario = usuario;
            fr.ShowInTaskbar = false;
            fr.ShowDialog();
        }
        private void CreateRequi_Load(object sender, EventArgs e) {
            getdata();
            textBox5.Text = usuario;
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
        }
        private void pictureBox2_Click(object sender, EventArgs e) {
            cancelarpo();
            Close();
        }
    }
}