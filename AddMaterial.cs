using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ComprasProject {
    public partial class AddMaterial : Form {
        public AddMaterial() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string user_id { set; get; }
        public DataTable dt = new DataTable();
        List<string> codigos = new List<string>();

        double cambio = 0;

        public string gettipodecambio() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT TOP 1 valor FROM tbtipodecambio ORDER BY valor DESC";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable tb = new DataTable();
                adapter.Fill(tb);
                conn.Close();
                string valor = "";
                foreach (DataRow desc in tb.Rows) {
                    valor = desc[0].ToString();
                }
                DataRow dr = tb.Rows[0];
                valor = dr[0].ToString();
                return valor;
            } catch (SqlException) {
                return "";
            }
        }
        
        public void fillunimedidas() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT fsid, fsdesc FROM tbunimedida";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);
                foreach (DataRow da in datatable.Rows) {
                    comboBox3.Items.Add(da[1].ToString());
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        
        private void button1_Click(object sender, EventArgs e) {
            
            agregaralcarrito(0);
            Close();
        }
        private void agregaralcarrito(int i) {
            try {
                if (textBox6.Text.Length > 0) {
                    double costototal = 0;
                    string absoluto = "";
                    double temp = 0;
                    if (comboBox4.SelectedIndex == 1) {
                        costototal = double.Parse(textBox6.Text);
                        temp = costototal / cambio;
                        absoluto = temp.ToString();
                        absoluto = Math.Round(temp, 2).ToString();
                    } else {
                        absoluto = textBox6.Text;
                    }
                    try { double numero = double.Parse(numericUpDown1.Value.ToString()); } catch (Exception) {
                        MessageBox.Show("Cantidad debe ser un numero entero");
                        return;
                    }
                    dataGridView1.Rows.Add((comboBox5.SelectedItem as ComboBoxSKU).fscategory,
                        (comboBox5.SelectedItem as ComboBoxSKU).fssku,
                        (comboBox5.SelectedItem as ComboBoxSKU).fsdesc,
                        numericUpDown1.Value.ToString(),
                        comboBox3.SelectedItem.ToString(),
                        textBox6.Text,
                        comboBox4.SelectedItem.ToString(),
                        "",
                        textBox5.Text,
                        absoluto, textBox2.Text);
                    
                    textBox5.Text = "";
                    textBox6.Text = "";
                    textBox2.Text = "";
                    dt.Clear();
                    foreach (DataGridViewRow row in dataGridView1.Rows) {
                        DataRow dRow = dt.NewRow();
                        foreach (DataGridViewCell cell in row.Cells) {
                            dRow[cell.ColumnIndex] = cell.Value;
                        }
                        dt.Rows.Add(dRow);
                    }
                }
                if (i == 0) { Close(); }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                List<string> files = new List<string>();
                files = dialog.FileNames.ToList();
                textBox5.Text = "";
                foreach (string onefile in files) {
                    textBox5.Text += onefile + "|";
                }
            }
        }
        private void button4_Click(object sender, EventArgs e) {
            if (textBox2.Text.Length >= 200 || comboBox5.Items.Count == 0) {
                MessageBox.Show("El comentario es demasiado extenso o no selecciono producto");
            } else {
                agregaralcarrito(1);
            }
        }
        
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }
        private void getallskulike(string lookfor) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select sku.id, sku.sku, sku.skudesc, sku.category "
                    + "from sku "
                    + "join (select id, concat(sku.sku, sku.skudesc, sku.category) as 'For Search' from SKU) conca "
                    + "on sku.id = conca.id where conca.[For Search] like(@lookforvalue)";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@lookforvalue", "%"+lookfor+"%");
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);
                conn.Close();
                if (datatable.Rows.Count == 0) {
                    return;
                }
                comboBox5.Items.Clear();
                ComboBoxSKU cbs;
                foreach (DataRow dr in datatable.Rows) {
                    cbs = new ComboBoxSKU();
                    cbs.fsid = dr[0].ToString();
                    cbs.fssku = dr[1].ToString();
                    cbs.fsdesc = dr[2].ToString();
                    cbs.fscategory = dr[3].ToString();
                    comboBox5.Items.Add(cbs);
                }
                comboBox5.SelectedIndex = 0;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddMaterial_Load(object sender, EventArgs e) {
            fillunimedidas();
            cambio = double.Parse(gettipodecambio());
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            dt.Columns.Add("Categoria");
            dt.Columns.Add("Código");
            dt.Columns.Add("Descripción");
            dt.Columns.Add("Cantidad");
            dt.Columns.Add("Unidad de Medida");
            dt.Columns.Add("Costo Estimado Total");
            dt.Columns.Add("Moneda");
            dt.Columns.Add("Anexos");
            dt.Columns.Add("Archivos");
            dt.Columns.Add("Valor Absoluto en Dlls");
            dt.Columns.Add("Comentario");
            //agregar columnas al datagridview
            dataGridView1.Columns.Add("fscategoria", "Categoria");
            dataGridView1.Columns.Add("fscodigo", "Codigo");
            dataGridView1.Columns.Add("fsdescripcion", "Descripcion");
            dataGridView1.Columns.Add("fscantidad", "Cantidad");
            dataGridView1.Columns.Add("fsunimedida", "Unidad de Medida");
            dataGridView1.Columns.Add("fscostoestimado", "Costo Estimado Total");
            dataGridView1.Columns.Add("fsmoneda", "Moneda");
            dataGridView1.Columns.Add("fsanexos", "Anexos");
            dataGridView1.Columns.Add("fsarchivos", "Archivos");
            dataGridView1.Columns.Add("fsabsoluto", "Valor Absoluto en Dlls");
            dataGridView1.Columns.Add("fscomentario", "Comentario");

            button1.DialogResult = DialogResult.OK;
            button2.DialogResult = DialogResult.Cancel;
            
        }
        private void textBox6_TextChanged(object sender, EventArgs e) {

        }

        private void button5_Click(object sender, EventArgs e) {
            CreateSKU cs = new CreateSKU();
            cs.usuario = usuario;
            cs.FormClosed += Cs_FormClosed;
            cs.ShowDialog();
        }

        private void Cs_FormClosed(object sender, FormClosedEventArgs e) {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {
            if (textBox2.Text.Length >= 499) {
                MessageBox.Show("Limite alcanzado");
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                textBox1.Text = ((sender as ComboBox).SelectedItem as ComboBoxSKU).fsdesc;
            } catch (Exception) {
                
            }
        }

        private void button6_Click(object sender, EventArgs e) {
            if (textBox3.Text.Length >= 1) {
                getallskulike(textBox3.Text);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
            if (Regex.IsMatch(e.KeyChar.ToString(), @"[^a-z^A-Z^0-9^ ^+^\-^\/^\b^\*^\(^\)]")) {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e) {
            if (Regex.IsMatch(e.KeyChar.ToString(), @"[^a-z^A-Z^0-9^ ^+^\-^\/^\b^\*^\(^\)]")) {
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }
    }
}
