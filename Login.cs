using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class Login : Form {
        public Login() {
            InitializeComponent();
        }
        public string usuario       { get; set; }
        public string tipo          { get; set; }
        public string user_id       { get; set; }
        public string user_depto    { get; set; }
        public string user_deptoid  { get; set; }
        
        public bool getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM Users WHERE username = '" + textBox1.Text + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                try {
                    if (Decrypt(table.Rows[0][6].ToString()) == textBox2.Text) {
                        tipo = table.Rows[0][3].ToString();
                        user_deptoid = table.Rows[0][2].ToString();
                        user_id = table.Rows[0][0].ToString();
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
        public void getuserinfo(string username) {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "SELECT * FROM Users WHERE username = '" + username + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                conn.Close();
                try {
                    tipo = table.Rows[0][3].ToString();
                    user_deptoid = table.Rows[0][2].ToString();
                    user_id = table.Rows[0][0].ToString();
                } catch (Exception) {
                    MessageBox.Show("Problema con la informacion del usuario");
                }
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        //boolean type is returned
        public bool IsAuthenticated(string srvr, string usr, string pwd) {
            bool authenticated = false;//declare the variable to be returned
            try { //try to use active directory credentials
                DirectoryEntry entry = new DirectoryEntry(@"", @"POSEY\" + usr, pwd); //create instance of a DirectoryEntry class
                //and use constructor which takes srvr, user and password as parameters
                object nativeObject = entry.NativeObject; //validate
                authenticated = true; //change boolean variable to be true
            } catch (DirectoryServicesCOMException cex) {
                MessageBox.Show(cex.Message);//if is not validated give message
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);//if is not validated give message
            }
            return authenticated; //return value
        }
        private void button1_Click(object sender, EventArgs e) {
            if (label5.Text.Length <= 5) {
                //autenticacion por credenciales de windows
                authbywindowscredentials();
                //autenticacion por nombre de usuario
                //authbyusername();
            } else {
                MessageBox.Show("Favor de Actualizar el sistema");
            }

        }
        private void authbyusername() {
            usuario = Environment.UserName;
            try {
                SqlConnection conn = new SqlConnection(Program.stringconnection);
                conn.Open();
                string sqlquery = "select * from users where username = @user ;";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@user", usuario);
                conn.Close();
                DataTable table = new DataTable();
                adapter.Fill(table);
                if (table.Rows.Count > 0) {
                    selectmenutype();
                }
            } catch (Exception) {
                MessageBox.Show("Usuario no registrado");
            }
        }
        private string getusername() {
            return Environment.UserName;
        }
        private void authbywindowscredentials() {
            usuario = textBox1.Text;
            if (IsAuthenticated("", textBox1.Text, textBox2.Text)) {
                selectmenutype();
            }
        }
        private void selectmenutype() {
            getuserinfo(usuario);
            Hide();
            if (usuario == "opardo") {
                MenuApAjuste ma = new MenuApAjuste();
                ma.usuario = usuario;
                ma.tipo = tipo;
                ma.user_id = user_id;
                ma.user_deptoid = user_deptoid;
                ma.ShowDialog();
                return;
            }
            if (tipo == "Manager") {
                MenuManager mf = new MenuManager();
                mf.usuario = usuario;
                mf.tipo = tipo;
                mf.user_id = user_id;
                mf.user_deptoid = user_deptoid;
                mf.ShowDialog();
            }
            if (tipo == "Requisitor" && usuario != "opardo") {
                MenuRequisitor mf = new MenuRequisitor();
                mf.usuario = usuario;
                mf.tipo = tipo;
                mf.user_id = user_id;
                mf.user_deptoid = user_deptoid;
                mf.ShowDialog();
            }
            if (tipo == "Almacen") {
                MenuAlmacen mf = new MenuAlmacen();
                mf.usuario = usuario;
                mf.tipo = tipo;
                mf.user_id = user_id;
                mf.user_deptoid = user_deptoid;
                mf.ShowDialog();
            }
            if (tipo == "Aprobador") {
                MenuAprobador mf = new MenuAprobador();
                mf.usuario = usuario;
                mf.tipo = tipo;
                mf.user_id = user_id;
                mf.user_deptoid = user_deptoid;
                mf.ShowDialog();
            }
            if (tipo == "Compras") {
                MenuCompras mf = new MenuCompras();
                mf.usuario = usuario;
                mf.tipo = tipo;
                mf.user_id = user_id;
                mf.user_deptoid = user_deptoid;
                mf.ShowDialog();
            }
            if (tipo == "Finanzas") {
                MenuFinanzas mf = new MenuFinanzas();
                mf.usuario = usuario;
                mf.tipo = tipo;
                mf.user_id = user_id;
                mf.user_deptoid = user_deptoid;
                mf.ShowDialog();
            }
            if (tipo == "Admin") {
                Menu m = new Menu();
                m.usuario = usuario;
                m.tipo = tipo;
                m.user_id = user_id;
                m.user_deptoid = user_deptoid;
                m.ShowDialog();
            }
            Close();
        }

        private void Login_Load(object sender, EventArgs e) {
            AcceptButton = button1;
            validarversion();
        }
        loading cargando = new loading();
        private void validarversion() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select top(1) fsver from tbversion;";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.SelectCommand.CommandTimeout = 150;
                DataTable table = new DataTable();
                adapter.Fill(table);
                DataRow dr = table.Rows[0];
                if (dr[0].ToString() != label4.Text) {
                    label5.Text = "Actualizacion Disponible\nCLICK AQUI";
                }
                conn.Close();
            } catch (SqlException) {
                MessageBox.Show("Lo sentimos, no se pudo establecer la conexion con el servidor.");
                Close();
            }
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

        private void label1_Click(object sender, EventArgs e) {
            
        }
        private void label3_Click(object sender, EventArgs e) {
            string creadormail = "lpalomares@posey.com";
            MailMessage mail = new MailMessage("aramis@posey.com", creadormail);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.posey.com";
            mail.Subject = "TEST";
            mail.Body = "Probando";
            try {
                //client.Send(mail);
            } catch (Exception) {
                MessageBox.Show("Problema al enviar correo");
            }
            exceltodatagridview exce = new exceltodatagridview();
            exce.ShowInTaskbar = false;
            exce.ShowDialog();
        }
        private void label4_Click(object sender, EventArgs e) {
            MessageBox.Show(
                "UPDATEABLE BY SETUP", 
                "UPDATES LOG");
        }
        private void label5_Click(object sender, EventArgs e) {
            Process.Start(@"\\mexfs01\TJTemp\!Soft\ARAMIS\downloadARAMIS");
        }
    }
}
