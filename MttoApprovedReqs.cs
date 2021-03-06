﻿using System;
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
    public partial class MttoApprovedReqs : Form {
        public MttoApprovedReqs() {
            InitializeComponent();
        }
        public string user_id;
        public string usuario;

        private void MttoApprovedReqs_Load(object sender, EventArgs e) {
            getapprovedmttoreqs();
        }
        private void getapprovedmttoreqs() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select mtto.fsid as 'ID', convert(varchar, mtto.fsfecha, 105) as 'Fecha de requisición', users.fulname as 'Nombre Completo' "
                    + ", mtto.fsdescripcion as 'Descripción de Trabajo' "
                    + "from tbmttoreq mtto join users on users.id = mtto.fsidusuario "
                    + "where fsstatus = 'Requisicion Aprobada'";
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

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
