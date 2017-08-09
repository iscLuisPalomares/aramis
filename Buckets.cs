using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class Buckets : Form {
        public Buckets() {
            InitializeComponent();
        }

        public string usuario       { get; set; }
        public string user_id       { get; set; }
        public string tipo          { get; set; }
        public string user_depto    { get; set; }

        public void getdata_for_datagridview() {
            try {
                string query = "SELECT id_bucket, " +
                    " accounts.acctnumber, " +
                    " accounts.depto, " +
                    " buckets.balance, " +
                    " buckets.budget, " +
                    " buckets.ajustado, " +
                    " buckets.gasto, " +
                    " buckets.asignado, " +
                    " buckets.periodo " +
                    "FROM accounts LEFT JOIN buckets ON buckets.id_cuenta = accounts.id " +
                    "WHERE ";
                //create accounts and periods lists instances
                List<string> cuentas = new List<string>();
                List<string> periodos = new List<string>();
                //pupulate lists with selected elements
                foreach (var item in checkedListBox1.CheckedItems) {
                    cuentas.Add(item.ToString().Split(',')[0]);
                }
                foreach (var item in checkedListBox2.CheckedItems) {
                    periodos.Add(item.ToString());
                }

                //concatenate query string to each account period
                foreach (object cuenta in cuentas) {
                    foreach (object periodo in periodos) {
                        query += " acctnumber = '" + cuenta.ToString() + "' AND periodo = '" + periodo.ToString() + "' OR ";
                    }
                }
                query = query.Remove(query.Length - 3, 3);
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                SqlDataAdapter adaptergridview = new SqlDataAdapter(query, conn);
                DataTable gridviewtable = new DataTable();
                adaptergridview.Fill(gridviewtable);
                dataGridView1.DataSource = gridviewtable;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        public void getdata_for_accountslist() {
            try {
                checkedListBox1.Items.Clear();
                checkedListBox2.Items.Clear();
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlcuentas = "select accounts.id, acctnumber, Deptos.name, acctdesc from accounts left join Deptos on accounts.depto = deptos.id";
                SqlDataAdapter adaptercuentas = new SqlDataAdapter(sqlcuentas, conn);
                DataTable cuentastable = new DataTable();
                adaptercuentas.Fill(cuentastable);
                DataSet ds = new DataSet();
                foreach (DataRow da in cuentastable.Rows) {
                    checkedListBox1.Items.Add(da[1].ToString() + ", " + da[2].ToString() + ", " + da[3].ToString());
                }
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void getdata_for_periodslist() {
            try {
                List<string> selected_accounts = new List<string>();
                List<string> ids = new List<string>();
                foreach (var item in checkedListBox1.CheckedItems) {
                    selected_accounts.Add(item.ToString().Split(',')[0]);
                }
                string query = "SELECT distinct id FROM Accounts WHERE ";
                foreach (object obj in selected_accounts) {
                    query += " acctnumber = '" + obj.ToString() + "' OR ";
                }
                query = query.Remove(query.Length - 3, 3);

                checkedListBox2.Items.Clear();
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                SqlDataAdapter adaptercuentas = new SqlDataAdapter(query, conn);
                DataTable cuentastable = new DataTable();
                adaptercuentas.Fill(cuentastable);
                DataSet ds = new DataSet();
                foreach (DataRow da in cuentastable.Rows) {
                    ids.Add(da[0].ToString());
                }
                conn.Close();
                
                query = "SELECT distinct periodo FROM buckets WHERE ";
                foreach (object obj in ids) {
                    query += " id_cuenta = '" + obj.ToString() + "' OR ";
                }
                query = query.Remove(query.Length - 3, 3);
                checkedListBox2.Items.Clear();
                connectionstring = Program.stringconnection;
                conn = new SqlConnection(connectionstring);
                conn.Open();
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(query, conn);
                cuentastable = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);
                ds = new DataSet();
                foreach (DataRow da in almacentb.Rows) {
                    checkedListBox2.Items.Add(da[0].ToString());
                }
                conn.Close();
            } catch (SqlException e) {
                MessageBox.Show(e.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            CreateBucket cb = new CreateBucket();
            cb.usuario = usuario;
            cb.ShowDialog();
        }
        private void button3_Click(object sender, EventArgs e) {
            if (checkedListBox1.CheckedItems.Count >= 1) {
                getdata_for_periodslist();
            } else {
                checkedListBox2.Items.Clear();
            }
        }
        private void button4_Click(object sender, EventArgs e) {
            if (checkedListBox1.CheckedItems.Count >= 1 && checkedListBox2.CheckedItems.Count >= 1) {
                getdata_for_datagridview();
                tabControl1.SelectedIndex = 1;
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata_for_accountslist();
        }
        private void Usuarios_SizeChanged(object sender, EventArgs e) {

        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            //usuario dio click al ajustado
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ajustado") {
                ChangeAjustado ca = new ChangeAjustado();
                ca.ajustado_actual = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                ca.id_bucket = dataGridView1[0, e.RowIndex].Value.ToString();
                ca.usuario = usuario;
                ca.user_id = user_id;
                ca.FormClosed += Ca_FormClosed;
                ca.ShowDialog();
            }
            //usuario dio click al budget y budget es menor a 100,000
            if (dataGridView1.Columns[e.ColumnIndex].Name == "budget" && int.Parse(dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString()) < 100000) {
                ChangeBudget cb = new ChangeBudget();
                cb.ajustado_actual = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                cb.id_bucket = dataGridView1[0, e.RowIndex].Value.ToString();

                cb.FormClosing += Cb_FormClosing;
                cb.FormClosed += Cb_FormClosed;
                cb.ShowDialog();
            }
        }

        private void Ca_FormClosed(object sender, FormClosedEventArgs e) {
            getdata_for_datagridview();
        }

        private void Cb_FormClosed(object sender, FormClosedEventArgs e) {
            getdata_for_datagridview();
        }

        private void Cb_FormClosing(object sender, FormClosingEventArgs e) {
            if (checkedListBox1.CheckedItems.Count >= 1 && checkedListBox2.CheckedItems.Count >= 1) {
                getdata_for_datagridview();
                tabControl1.SelectedIndex = 1;
            }

        }
        bool todoschecados = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (!todoschecados) {
                for (int i = 0;i < checkedListBox1.Items.Count;i++) {
                    checkedListBox1.SetItemChecked(i, true);
                }
                todoschecados = true;
            } else {
                for (int i = 0;i < checkedListBox1.Items.Count;i++) {
                    checkedListBox1.SetItemChecked(i, false);
                }
                todoschecados = false;
            }
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e) {

        }
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e) {

        }
        bool todoschecados2 = false;
        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            if (!todoschecados2) {
                for (int i = 0;i < checkedListBox2.Items.Count;i++) {
                    checkedListBox2.SetItemChecked(i, true);
                }
                todoschecados2 = true;
            } else {
                for (int i = 0;i < checkedListBox2.Items.Count;i++) {
                    checkedListBox2.SetItemChecked(i, false);
                }
                todoschecados2 = false;
            }
        }
    }
}
