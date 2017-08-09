using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CreateExtraCharge : Form {
        public CreateExtraCharge() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string userid { get; set; }
        public string lineaid { get; set; }

        public string id, codigo, descripcion, cantidad, um, cuenta;
        private string bucketnumber;

        private void CreateExtraCharge_Load(object sender, EventArgs e) {
            fillcargos();

            textBox2.Text = usuario;
            textBox3.Text = DateTime.Today.ToString("yyyy-MM-dd");
            getbucketnumber();
        }

        private void getbucketnumber() {
            try {
                string sqlquery = "select top 1 id_bucket "
                    + "from buckets "
                    + "join Accounts on Accounts.id = buckets.id_cuenta "
                    + "where Accounts.acctnumber = '01-04-10203-00-00' order by id_bucket desc";
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                if (table.Rows.Count > 0) {
                    bucketnumber = table.Rows[0][0].ToString();
                } else {
                    MessageBox.Show("No hay buckets para este periodo ");
                    Close();
                }
            } catch (Exception) {
                MessageBox.Show("Se presento un problema, intente de nuevo");
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                setcargo();
            } catch (Exception) {
                MessageBox.Show("Problema con la informacion, intente de nuevo");
            }
        }
        private void setcargo() {
            try {
                string connectionstring = Program.stringconnection;
                string sqlquery = "set xact_abort on \n BEGIN TRANSACTION \n";
                sqlquery += "DECLARE @reqid INTEGER; \n";
                sqlquery += "INSERT INTO materialrequerido (fsrequisicion, " +
                    "fscantidad, fsunimedida, fscodigo, fsdesc, fscostoestimado, " +
                    "fsmoneda, fsabsolutodlls, fsstatus, fscuenta, bucketid, comentario) VALUES ("
                    + "(select fsrequisicion from materialrequerido where fsid = " + lineaid + ")"
                    + ", 1"
                    + ",'Cargo'"
                    + ",'" + (comboBox1.SelectedItem as CBCargos).fscodigo + "'"
                    + ",'" + (comboBox1.SelectedItem as CBCargos).fsdesc + "'"
                    + ", 0"
                    + ", (select fsmoneda from materialrequerido where fsid = " + lineaid + ")"
                    + ", 0"
                    + ",'Requisicion Aprobada'"
                    + ", (select fscuenta from materialrequerido where fsid = " + lineaid + ")"
                    + ", (select bucketid from materialrequerido where fsid = " + lineaid + ")"
                    + ", '" + textBox4.Text + "'"
                    + ");\n";
                sqlquery += "SELECT SCOPE_IDENTITY();\n";
                sqlquery += "COMMIT TRANSACTION;";
                MessageBox.Show(sqlquery);
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                SqlCommand comm = new SqlCommand(sqlquery, conn);
                string newlinenumber = comm.ExecuteScalar().ToString();
                MessageBox.Show("Listo");
                conn.Close();
                Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void fillcargos() {
            try {
                string connectionstring = Program.stringconnection;
                string sqlquery = "select * from sku where category = 'Cargos extra'";
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable tabla = new DataTable();
                adapter.Fill(tabla);
                comboBox1.Items.Clear();
                CBCargos cargos = new CBCargos();
                foreach (DataRow dr in tabla.Rows) {
                    cargos = new CBCargos();
                    cargos.fscodigo = dr["sku"].ToString();
                    cargos.fsdesc = dr["skudesc"].ToString();
                    comboBox1.Items.Add(cargos);
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            textBox1.Text = (comboBox1.SelectedItem as CBCargos).fscodigo;
        }
    }
}
