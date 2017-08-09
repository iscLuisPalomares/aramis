using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class PendingCotizaciones : Form {
        public PendingCotizaciones() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string depto { get; set; }
        public string user_id { get; set; }

        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select "
                    + "cotz.fsid as 'ID',cotz.createdate as 'Fecha',cotz.fscomentario as 'Comentario'"
                    + ", supp.suppname as 'Proveedor', cotz.fscostototal as 'Costo Total'"
                    + ",cotz.fsimpuestos as 'Impuestos %'"
                    + ",cotz.fsdivisa as 'Moneda', STUFF(("
                    + "select distinct(', ' + usr.fulname) from users usr "
                    + "join requisiciones req on req.createdby = usr.username "
                    + "join materialrequerido matreq on matreq.fsrequisicion = req.id_req "
                    + "join tbcotmaterialrequerido cotmat on cotmat.fsidmaterialrequerido = matreq.fsid "
                    + "join tbcotizaciones coti on coti.fsid = cotmat.fsidcotizacion "
                    + "where cotz.fsid = coti.fsid for xml path ('') "
                    + "), 1, 1, '') as 'Requisitor(es)' "
                    + ", STUFF((select distinct(',' + deps.name)"
                    + "from deptos deps "
                    + "join requisiciones req on req.deptoid = deps.id "
                    + "join materialrequerido matreq on matreq.fsrequisicion = req.id_req "
                    + "join tbcotmaterialrequerido cotmat on cotmat.fsidmaterialrequerido = matreq.fsid "
                    + "join tbcotizaciones coti on coti.fsid = cotmat.fsidcotizacion "
                    + "where cotz.fsid = coti.fsid for xml path ('') "
                    + "), 1, 1, '') as 'Departamento(s)' "
                    + "from tbcotizaciones cotz join asl supp on supp.id = cotz.fssupplier "
                    + "where cotz.fsstatus = 'Cotizacion Creada'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void button1_Click(object sender, EventArgs e) {
            Close();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            try {
                ApproveCotizacion ar = new ApproveCotizacion();
                ar.idcot = dataGridView1[0, e.RowIndex].Value.ToString();
                ar.FormClosed += Ar_FormClosed;
                ar.costototal = double.Parse(dataGridView1[4, e.RowIndex].Value.ToString());
                ar.ShowInTaskbar = false;
                ar.usuario = usuario;
                ar.ShowDialog();
            } catch (Exception) { }
        }

        private void Ar_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }

        private void PendingRequi_Load(object sender, EventArgs e) {
            getdata();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                ContextMenuStrip m = new ContextMenuStrip();
                m.Items.Add("Archivos");
                m.Items.Add("Editar");
                m.ItemClicked += M_ItemClicked;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        private void M_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            ToolStripItem btn = e.ClickedItem;
            if (btn.Text == "Archivos") {
                try {
                    
                } catch (Exception) { MessageBox.Show("Problema al abrir archivos"); }
            }
            if (btn.Text == "Editar") {
                try {
                    EditLinea el = new EditLinea();
                } catch (Exception) {
                    
                }
            }
        }
    }
}
