using System;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class MenuAprobador : Form {
        public MenuAprobador() {
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
        //Usuarios
        private void pictureBox1_Click(object sender, EventArgs e) {
            Usuarios u = new Usuarios();
            u.usuario = usuario;
            u.tipo = tipo;
            u.user_depto = user_depto;
            u.user_id = user_id;
            u.FormClosed += U_FormClosed;
            Visible = false;
            u.ShowDialog();
        }
        private void U_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        //Accounts
        private void pictureBox2_Click(object sender, EventArgs e) {
            Accounts ca = new Accounts();
            ca.usuario = usuario;
            ca.tipo = tipo;
            ca.user_id = user_id;
            ca.user_depto = user_depto;
            ca.FormClosed += Ca_FormClosed;
            Visible = false;
            ca.ShowDialog();
        }
        private void Ca_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        //Departments
        private void pictureBox3_Click(object sender, EventArgs e) {
            Departamentos cd = new Departamentos();
            cd.usuario = usuario;
            cd.tipo = tipo;
            cd.user_id = user_id;
            cd.user_depto = user_depto;
            cd.FormClosed += Cd_FormClosed;
            Visible = false;
            cd.ShowDialog();
        }
        private void Cd_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        //Log out - Iniciar Sesion
        private void pictureBox5_Click(object sender, EventArgs e) {
            Hide();
            Login l = new Login();
            l.ShowDialog();
            Close();
        }
        //Approved Suppliers
        private void pictureBox6_Click(object sender, EventArgs e) {
            Suppliers ca = new Suppliers();
            ca.usuario = usuario;
            ca.tipo = tipo;
            ca.user_id = user_id;
            ca.user_depto = user_depto;
            ca.FormClosed += Ca_FormClosed1;
            Visible = false;
            ca.ShowDialog();
        }
        private void Ca_FormClosed1(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        //SKU - Numeros de parte
        private void pictureBox7_Click(object sender, EventArgs e) {
            SKU sk = new SKU();
            sk.usuario = usuario;
            sk.tipo = tipo;
            sk.user_id = user_id;
            sk.user_depto = user_depto;
            sk.FormClosed += Sk_FormClosed;
            Visible = false;
            sk.ShowDialog();
        }
        private void Sk_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
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
        //Buckets
        private void pictureBox10_Click(object sender, EventArgs e) {
            Buckets bu = new Buckets();
            bu.usuario = usuario;
            bu.user_id = user_id;
            bu.user_depto = user_depto;
            bu.tipo = tipo;
            bu.FormClosed += Bu_FormClosed;
            Visible = false;
            bu.ShowDialog();
        }
        private void Bu_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        //Files Req
        private void pictureBox11_Click(object sender, EventArgs e) {
            RecibosPO pr = new RecibosPO();
            pr.usuario = usuario;
            pr.user_id = user_id;
            pr.FormClosed += Pr_FormClosed;
            Visible = false;
            pr.ShowDialog();
        }
        private void Pr_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        //Crear requisicion
        private void pictureBox12_Click(object sender, EventArgs e) {
            CreateRequi cr = new CreateRequi();
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
        private void Cr_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox15_Click(object sender, EventArgs e) {
            LineasPorCotizar lt = new LineasPorCotizar();
            lt.usuario = usuario;
            lt.user_id = user_id;
            lt.FormClosed += Lt_FormClosed;
            Visible = false;
            lt.ShowDialog();
        }
        private void Lt_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox16_Click(object sender, EventArgs e) {
            PendingCotizacionesB r = new PendingCotizacionesB();
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
        private void pictureBox17_Click(object sender, EventArgs e) {
            ImprimirListaPOs il = new ImprimirListaPOs();
            il.user_id = user_id;
            il.usuario = usuario;
            il.FormClosed += Il_FormClosed;
            Visible = false;
            il.ShowDialog();
        }
        private void Il_FormClosed(object sender, FormClosedEventArgs e) {
            Visible = true;
        }
        private void pictureBox13_Click(object sender, EventArgs e) {
            MessageBox.Show("Aprobar POs");
            return;
            //PendingPOs ppo = new PendingPOs();
            //ppo.usuario = usuario;
            //ppo.user_id = user_id;
            //ppo.depto = user_depto;
            //ppo.ShowInTaskbar = false;
            //ppo.ShowDialog();
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
            PendingAjustesB pa = new PendingAjustesB();
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
    }
}
