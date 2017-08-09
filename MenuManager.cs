using System;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class MenuManager : Form {
        public MenuManager() {
            InitializeComponent();
        }

        public string usuario { get; set; }
        public string user_id { get; set; }
        public string tipo { get; set; }
        public string user_depto { get; set; }
        public string user_deptoid { get; set; }
        public string gerenteid { get; set; }
        
        //CODIGO DEL FORM
        private void Menu_Load(object sender, EventArgs e) {
            label1.Text += usuario;
        }
        
        //Log out - Iniciar Sesion
        private void pictureBox5_Click(object sender, EventArgs e) {
            Hide();
            Login l = new Login();
            l.ShowDialog();
            Close();
        }
        
        //Requisiciones
        private void pictureBox9_Click(object sender, EventArgs e) {
            PendingRequi r = new PendingRequi();
            r.usuario = usuario;
            r.user_id = user_id;
            r.tipo = tipo;
            r.depto = user_deptoid;
            r.FormClosed += R_FormClosed;
            Visible = false;
            r.ShowDialog();
        }
        private void R_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        //Files PO
        private void pictureBox8_Click(object sender, EventArgs e) {
            CotizacionesParaPO ac = new CotizacionesParaPO();
            ac.usuario = usuario;
            ac.user_id = user_id;
            ac.FormClosed += Ac_FormClosed;
            Visible = false;
            ac.ShowDialog();
        }
        private void Ac_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void Cr_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        
        private void pictureBox16_Click(object sender, EventArgs e) {
            PendingCotizaciones r = new PendingCotizaciones();
            r.usuario = usuario;
            r.user_id = user_id;
            r.tipo = tipo;
            r.FormClosed += R_FormClosed1;
            Visible = false;
            r.ShowDialog();
        }
        private void R_FormClosed1(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox14_Click(object sender, EventArgs e) {
            Cotizaciones c = new Cotizaciones();
            c.user_id = user_id;
            c.usuario = usuario;
            c.ShowInTaskbar = false;
            c.ShowDialog();
        }
        private void pictureBox18_Click(object sender, EventArgs e) {
            ImprimirPOdev nnn = new ImprimirPOdev();
            nnn.ShowInTaskbar = false;
            nnn.ShowDialog();
        }
        private void panel1_Paint(object sender, PaintEventArgs e) {

        }
        private void pictureBox19_Click(object sender, EventArgs e) {
            Suppliers pe = new Suppliers();
            pe.ShowDialog();
        }
        private void pictureBox20_Click(object sender, EventArgs e) {
            MonitorLineas mm = new MonitorLineas();
            mm.usuario = usuario;
            mm.FormClosed += Mm_FormClosed;
            Visible = false;
            mm.ShowDialog();
        }
        private void Mm_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox21_Click(object sender, EventArgs e) {
            CreateAjuste cr = new CreateAjuste();
            cr.usuario = usuario;
            cr.user_id = user_id;
            cr.user_depto = user_depto;
            cr.user_deptoid = user_deptoid;
            cr.deptoid = user_deptoid;
            cr.gerenteid = gerenteid;
            cr.tipo = tipo;
            cr.FormClosed += Cr_FormClosed;
            Visible = false;
            cr.ShowDialog();
        }
        private void pbapproveajuste_Click(object sender, EventArgs e) {
            PendingAjustes pa = new PendingAjustes();
            pa.usuario = usuario;
            pa.user_id = user_id;
            Visible = false;
            pa.FormClosed += Pa_FormClosed;
            pa.ShowDialog();
        }
        private void Pa_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox1_Click_1(object sender, EventArgs e) {
            MonitorAjustes ma = new MonitorAjustes();
            Visible = false;
            ma.FormClosed += Ma_FormClosed;
            ma.usuario = usuario;
            ma.user_id = user_id;
            ma.ShowDialog();
        }
        private void Ma_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox2_Click_1(object sender, EventArgs e) {
            ApprovedAjustes aa = new ApprovedAjustes();
            aa.user_id = user_id;
            aa.usuario = usuario;
            aa.FormClosed += Aa_FormClosed;
            Visible = false;
            aa.ShowDialog();
        }
        private void Aa_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox3_Click_1(object sender, EventArgs e) {
            Delegados d = new Delegados();
            d.usuario = usuario;
            d.user_id = user_id;
            d.user_depto = user_depto;
            d.tipo = tipo;
            d.ShowDialog();
        }

        private void changePassToolStripMenuItem_Click(object sender, EventArgs e) {
            EditPass ep = new EditPass();
            ep.user_id = user_id;
            ep.FormClosed += Ep_FormClosed;
            Visible = false;
            ep.ShowDialog();
        }

        private void Ep_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e) {
            PendingRequiAdmin r = new PendingRequiAdmin();
            r.usuario = usuario;
            r.user_id = user_id;
            r.tipo = tipo;
            r.depto = user_deptoid;
            r.FormClosed += R_FormClosed;
            Visible = false;
            r.ShowDialog();
        }

        private void pbrequi_Click(object sender, EventArgs e) {
            CreateRequi cr = new CreateRequi();
            cr.usuario = usuario;
            cr.user_id = user_id;
            cr.user_depto = user_depto;
            cr.user_deptoid = user_deptoid;
            cr.deptoid = user_deptoid;
            cr.gerenteid = gerenteid;
            cr.tipo = tipo;
            cr.FormClosed += Cr_FormClosed2;
            Visible = false;
            cr.ShowDialog();
        }
        private void Cr_FormClosed2(object sender, FormClosedEventArgs e) {
            Visible = true;
        }

        private void pictureBox6_Click(object sender, EventArgs e) {
            Reportes rep = new Reportes();
            rep.userid = user_id;
            rep.username = usuario;
            rep.FormClosed += Rep_FormClosed;
            Visible = false;
            rep.ShowDialog();
        }

        private void Rep_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
    }
}
