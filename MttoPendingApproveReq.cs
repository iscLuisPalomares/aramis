using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ComprasProject {
    public partial class MttoPendingApproveReq : Form {
        public MttoPendingApproveReq() {
            InitializeComponent();
        }

        private void PendingApproveMttoReq_Load(object sender, EventArgs e) {
            getpendingmttoreqs();
        }
        public string usuario = "";
        public string user_id = "";
        private void getpendingmttoreqs() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select mtto.fsid as 'ID', convert(varchar, mtto.fsfecha, 105) as 'Fecha de requisición', users.fulname as 'Nombre Completo' "
                    + ", mtto.fsdescripcion as 'Descripción de Trabajo' from tbmttoreq mtto join users on users.id = mtto.fsidusuario "
                    + "where fsstatus = 'Requisicion Creada'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
                dataGridView1.Columns["Nombre Completo"].Width = 200;
                dataGridView1.Columns["Descripción de Trabajo"].Width = 450;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            MttoApproveReq app = new MttoApproveReq();
            app.usuario = usuario;
            app.user_id = user_id;
            app.mttoreq = dataGridView1[0, e.RowIndex].Value.ToString();
            Visible = false;
            app.FormClosed += App_FormClosed;
            app.ShowDialog();
        }

        private void App_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
