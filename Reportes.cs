using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class Reportes : Form {
        public Reportes() {
            InitializeComponent();
        }
        public string userid { get; set; }
        public string username { get; set; }
        
        private void Usuarios_Load(object sender, EventArgs e) {

        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            
        }
        private void button2_Click(object sender, EventArgs e) {
            
        }
        private void pictureBox2_Click(object sender, EventArgs e) {
            RepPOAprobados repo = new RepPOAprobados();
            repo.user_id = userid;
            repo.usuario = username;
            repo.FormClosed += Repo_FormClosed;
            Visible = false;
            repo.ShowDialog();
        }
        private void Repo_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox8_Click(object sender, EventArgs e) {
            RepGastoVendorChart reg = new RepGastoVendorChart();
            reg.user_id = userid;
            reg.usuario = username;
            reg.FormClosed += Reg_FormClosed;
            Visible = false;
            reg.ShowDialog();
        }
        private void Reg_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox9_Click(object sender, EventArgs e) {
            RepGastoDeptoChart reg = new RepGastoDeptoChart();
            reg.user_id = userid;
            reg.usuario = username;
            reg.FormClosed += Reg_FormClosed1;
            Visible = false;
            reg.ShowDialog();
        }
        private void Reg_FormClosed1(object sender, FormClosedEventArgs e) {
            Visible = true;
        }

        private void pictureBox7_Click(object sender, EventArgs e) {
            RepGastoAjustes repajustes = new RepGastoAjustes();
            repajustes.user_id = userid;
            repajustes.usuario = username;
            repajustes.FormClosed += Repajustes_FormClosed;
            Visible = false;
            repajustes.ShowDialog();
        }
        private void Repajustes_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox3_Click(object sender, EventArgs e) {
            RepVendorDuties reg = new RepVendorDuties();
            reg.user_id = userid;
            reg.usuario = username;
            reg.FormClosed += Reg_FormClosed2;
            Visible = false;
            reg.ShowDialog();
        }
        private void Reg_FormClosed2(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox4_Click(object sender, EventArgs e) {
            RepDelegados del = new RepDelegados();
            del.user_id = userid;
            del.usuario = username;
            Visible = false;
            del.FormClosed += Del_FormClosed;
            del.ShowDialog();
        }
        private void Del_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox6_Click(object sender, EventArgs e) {
            RepLineasConPO lineas = new RepLineasConPO();
            lineas.user_id = userid;
            lineas.usuario = username;
            Visible = false;
            lineas.FormClosed += Lineas_FormClosed;
            lineas.ShowDialog();
        }

        private void Lineas_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }

        private void pictureBox5_Click(object sender, EventArgs e) {
            RepLineasRecibidas recibidas = new RepLineasRecibidas();
            Visible = false;
            recibidas.FormClosed += Recibidas_FormClosed;
            recibidas.ShowDialog();
        }

        private void Recibidas_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
    }
}
