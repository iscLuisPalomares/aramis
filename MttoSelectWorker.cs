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
    public partial class MttoSelectWorker : Form {
        public MttoSelectWorker() {
            InitializeComponent();
        }
        public string selectedemployee;
        public string mttoreq;
        public string archivo1;
        public string archivo2;
        private void button1_Click(object sender, EventArgs e) {
            selectedemployee = comboBox1.SelectedItem.ToString();
            Close();
        }

        private void MttoSelectWorker_Load(object sender, EventArgs e) {
            button1.DialogResult = DialogResult.OK;
            getoperarios();
            setfilebuttons();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void getoperarios() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select * from tbmttoemployees";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                comboBox1.Items.Clear();
                foreach (DataRow dr in table.Rows) {
                    comboBox1.Items.Add(dr[2].ToString());
                }
                comboBox1.SelectedIndex = 0;
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void setfilebuttons() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select mtto.fsid as 'ID', convert(varchar, mtto.fsfecha, 105) as 'Fecha de requisición', users.fulname as 'Nombre Completo' "
                    + ", mtto.fsdescripcion as 'Descripción de Trabajo', mtto.fsfile1 as 'File 1', mtto.fsfile2 as 'File 2' "
                    + "from tbmttoreq mtto join users on users.id = mtto.fsidusuario "
                    + "where fsid = @id";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@id", mttoreq);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                if (table.Rows[0][4].ToString().Length > 0) {
                    archivo1 = table.Rows[0][4].ToString();
                } else { button3.Visible = false; }
                if (table.Rows[0][5].ToString().Length > 0) {
                    archivo2 = table.Rows[0][5].ToString();
                } else { button4.Visible = false; }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            try {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FileName = archivo1;
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    System.IO.File.Copy(@"\\mexfs01\TJTemp\Opardo\FOLIOS\mttoreq\" + mttoreq + @"\" + archivo1,
                    saveFileDialog1.FileName, true);
                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                }
            } catch (Exception) {
                MessageBox.Show("Error al abrir archivo");
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            try {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FileName = archivo2;
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    System.IO.File.Copy(@"\\mexfs01\TJTemp\Opardo\FOLIOS\mttoreq\" + mttoreq + @"\" + archivo2,
                    saveFileDialog1.FileName, true);
                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                }
            } catch (Exception) {
                MessageBox.Show("Error al abrir archivo");
            }
        }
    }
}
