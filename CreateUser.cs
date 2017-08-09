using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class CreateUser : Form {
        public CreateUser() {
            InitializeComponent();
        }
        public string user_id { get; set; }
        public string usuario { get; set; }
        public void getdeptos() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT * FROM Deptos";
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);
                foreach (DataRow da in almacentb.Rows) {
                    comboBox1.Items.Add(da[1].ToString());
                }
                comboBox1.SelectedIndex = 0;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        public void setcrearuser() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                int deptoid = getdeptoid();
                if (deptoid == 0) { return; }
                conn.Open();
                if (textBox1.Text == "" || comboBox1.SelectedItem.ToString() == "" 
                    || comboBox2.SelectedItem.ToString() == "" || textBox4.Text == "") {
                    MessageBox.Show("Te hace falta algunos campos por llenar");
                } else {
                    string sqlquery = "INSERT INTO Users (username, depto, tipo, createdate, createdby, pass, mac, fulname, correo) VALUES ('"
                        + textBox1.Text + "','" + deptoid.ToString() + "','" + comboBox2.SelectedItem.ToString() + "','"
                        + textBox4.Text + "','" + textBox5.Text + "','" + Encrypt(textBox2.Text) + "','"
                        + GetMacAddress() + "','" + textBox6.Text + "', '" + textBox7.Text + "')";
                    SqlCommand ejecucion = new SqlCommand();
                    ejecucion.Connection = conn;
                    ejecucion.CommandType = CommandType.Text;
                    ejecucion.CommandText = sqlquery;
                    ejecucion.ExecuteNonQuery();
                    conn.Close();
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    textBox5.Text = usuario;
                    textBox6.Text = "";
                    MessageBox.Show("Nuevo usuario creado", "Listo");
                }
            } catch (SqlException) {
                MessageBox.Show("Usuario ya existe", "Error");
            }
        }
        private int getdeptoid() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlalmacenistas = "SELECT id FROM Deptos WHERE name = '" + comboBox1.SelectedItem.ToString() + "'";
                SqlDataAdapter adapteralmacen = new SqlDataAdapter(sqlalmacenistas, conn);
                DataTable table = new DataTable();
                DataTable almacentb = new DataTable();
                adapteralmacen.Fill(almacentb);
                DataRow da = almacentb.Rows[0];
                conn.Close();
                return int.Parse(da[0].ToString());
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                Close();
                return 0;
            }
        }
        private void CreateUser_Load(object sender, EventArgs e) {
            textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            textBox5.Text = usuario;
            comboBox2.SelectedIndex = 0;
            getdeptos();
        }
        //ENCRIPTAR USUARIO
        public static string Encrypt(string plainText) {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        static readonly string PasswordHash = "uy545";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        private string GetMacAddress() {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed &&
                    !string.IsNullOrEmpty(tempMac) &&
                    tempMac.Length >= MIN_MAC_ADDR_LENGTH) {
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }
            return macAddress;
        }
        
        
        private void button1_Click(object sender, EventArgs e) {
            if (textBox2.Text.ToString() != textBox3.Text.ToString()) {
                MessageBox.Show("Contraseñas no coinciden", "Error");
                return;
            }
            setcrearuser();
        }
        private void CreateUser_SizeChanged(object sender, EventArgs e) {
            panel1.Height = Height - 40;
            panel1.Width = Width;
        }
        private void textBox4_TextChanged(object sender, EventArgs e) {

        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
