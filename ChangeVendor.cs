using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class ChangeVendor : Form {
        public ChangeVendor() {
            InitializeComponent();
        }
        public string aslid     { get; set; }
        public string flag      { get; set; }
        public string vendorname { get; set; }
        
        private void CreateUser_Load(object sender, EventArgs e) {
            getvendors();
            button1.DialogResult = DialogResult.OK;
        }
        private void getvendors() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                string sqlquery = "SELECT id, suppname FROM asl ORDER BY suppname";
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                comboBox1.Items.Clear();
                foreach (DataRow ddd in table.Rows) {
                    comboBox1.Items.Add(ddd[0].ToString() + "|" + ddd[1].ToString());
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
                
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
                
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            vendorname = comboBox1.SelectedItem.ToString().Split('|')[0];
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
