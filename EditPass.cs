using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class EditPass : Form {
        public EditPass() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string user_id { get; set; }
        public string deptoid { get; set; }

        public bool getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT pass FROM users WHERE id = '" + user_id + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                try {
                    if (Decrypt(table.Rows[0][0].ToString()) == textBox1.Text) {
                        return true;
                    } else {
                        return false;
                    }
                } catch (Exception) {
                    return false;
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public void changepass() {
            try {
                if (textBox2.Text == textBox3.Text) {
                    if (getdata()) {
                        string connectionstring = Program.stringconnection;
                        SqlConnection conn = new SqlConnection(connectionstring);
                        conn.Open();
                        string sqlquery = "UPDATE users SET pass = '" + Encrypt(textBox2.Text) + "' WHERE id = " + user_id + ";";
                        SqlCommand ejecucion = new SqlCommand();
                        ejecucion.Connection = conn;
                        ejecucion.CommandType = CommandType.Text;
                        ejecucion.CommandText = sqlquery;
                        ejecucion.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Actualizado", "Listo");
                        Close();
                    } else {
                        MessageBox.Show("No pudimos realizar el cambio de pass");
                    }
                } else {
                    MessageBox.Show("Las contraseñas no coinciden");
                }
                
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateUser_Load(object sender, EventArgs e) {
            
        }
        private void button1_Click(object sender, EventArgs e) {
            changepass();
        }
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
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
        public static string Decrypt(string encryptedText) {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        static readonly string PasswordHash = "uy545";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";
    }
}
