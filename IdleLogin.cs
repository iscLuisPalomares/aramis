using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComprasProject
{
    public partial class IdleLogin : Form
    {
        public IdleLogin()
        {
            InitializeComponent();
        }
        string tipo = "";
        public bool getdata()
        {
            try
            {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM Users WHERE username = '"+textBox1.Text+"'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                try
                {
                    if (Decrypt(table.Rows[0][6].ToString()) == textBox2.Text)
                    {
                        tipo = tipo = table.Rows[0][3].ToString();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } catch (Exception)
                {
                    return false;
                }
                
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            string usuario = textBox1.Text;
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                return;
            }
            if (getdata())
            {
                Hide();
                Menu m = new Menu();
                m.usuario = usuario;
                m.tipo = tipo;
                m.ShowDialog();
                Close();
            } else
            {
                MessageBox.Show("Usuario y/o contraseña incorrecto(s)", "Error");
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            AcceptButton = button1;
        }

        //DESENCRIPTAR PASSWORD DE SQL
        public static string Decrypt(string encryptedText)
        {
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
