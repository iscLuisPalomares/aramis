using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {

    public partial class MonitorLineas : Form {
        public MonitorLineas() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_id { get; set; }
        public string user_depto { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT materialrequerido.fsid as 'ID', "
                    + "fsrequisicion as 'Requisicion', fspurchaseorder as 'Orden de Compra'"
                    + ", fscodigo as 'Codigo', fsdesc as 'Descripcion', "
                    + "fscantidad as 'Cantidad', fsunimedida as 'U/M'"
                    + ", materialrequerido.fsstatus as 'Status', recibido as 'Recibido', "
                    + "tblpurchaseorders.fsstatuscomment as 'Comentario PO' "
                    + "FROM materialrequerido "
                    + "left join tblpurchaseorders "
                    + "on materialrequerido.fspurchaseorder = tblpurchaseorders.fsid "
                    + "WHERE fsrequisicion IN " 
                    + "(SELECT id_req FROM requisiciones WHERE createdby = '" + usuario + "')";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public void getcancelables() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT fsid as 'ID', "
                    + "fsrequisicion as 'Requisicion', fscodigo as 'Codigo', fsdesc as 'Descripcion', "
                    + "fscantidad as 'Cantidad', fsunimedida as 'U/M', fsstatus as 'Status', recibido as 'Recibido' "
                    + "FROM materialrequerido "
                    + "WHERE fsrequisicion IN "
                    + "(SELECT id_req FROM requisiciones WHERE createdby = '" + usuario + "') "
                    + "and fsstatus in ('Requisicion Aprobada')";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView2.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getusuariosdepto();
            getdata();
            getcancelables();
        }
        private void getusuariosdepto() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select id, username, fulname "
                    + "from users "
                    + "where depto = ("
                    + "select depto from users where username = '" + usuario + "'"
                    + ")";
                if (usuario == "opardo") {
                    sqlquery = "select id, username, fulname from users where depto not in (31)";
                }
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                ComboBoxUserSolicitud us = new ComboBoxUserSolicitud();
                comboBox1.Items.Clear();
                foreach (DataRow dr in table.Rows) {
                    us = new ComboBoxUserSolicitud();
                    us.fsid = dr[0].ToString();
                    us.fsusername = dr[1].ToString();
                    us.fsfullname = dr[2].ToString();
                    comboBox1.Items.Add(us);
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void cargaritemsdelotro() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT fsid as 'ID', "
                    + "fsrequisicion as 'Requisicion', fspurchaseorder as 'Orden de Compra'"
                    + ", fscodigo as 'Codigo', fsdesc as 'Descripcion', "
                    + "fscantidad as 'Cantidad', fsunimedida as 'U/M', fsstatus as 'Status', recibido as 'Recibido' "
                    + "FROM materialrequerido WHERE fsrequisicion IN "
                    + "(SELECT id_req FROM requisiciones "
                    + "WHERE createdby = @username "
                    + "AND (createdate >= @desde and createdate <= @hasta)) "
                    + "order by fsid desc";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@username", (comboBox1.SelectedItem as ComboBoxUserSolicitud).fsusername);
                adapter.SelectCommand.Parameters.AddWithValue("@desde", dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00.000");
                adapter.SelectCommand.Parameters.AddWithValue("@hasta", dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59.999");
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView3.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            ConfLineaDel conf = new ConfLineaDel();
            conf.nodelinea = dataGridView2[0, e.RowIndex].Value.ToString();
            conf.FormClosed += Conf_FormClosed;
            conf.ShowDialog();
        }

        private void Conf_FormClosed(object sender, FormClosedEventArgs e) {
            getcancelables();
            getdata();
        }

        private void button2_Click(object sender, EventArgs e) {
            cargaritemsdelotro();
        }
    }
}
