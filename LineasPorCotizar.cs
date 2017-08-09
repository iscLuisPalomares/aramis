using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class LineasPorCotizar : Form {
        public LineasPorCotizar() {
            InitializeComponent();
        }
        public string usuario { get; set; }
        public string tipo { get; set; }
        public string user_id { get; set; }
        public string user_depto { get; set; }
        public string deptoid { get; set; }

        private DataTable getselecteditems() {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Codigo");
            dt.Columns.Add("Descripcion");
            dt.Columns.Add("Cantidad");
            dt.Columns.Add("U/M");
            dt.Columns.Add("Cuenta");
            dt.Columns.Add("Costo Unitario");
            dt.Columns.Add("Costo Extendido");
            dt.Columns.Add("Total Dlls");
            foreach (DataGridViewRow dgvr in dataGridView1.Rows) {
                if (dgvr.Selected) {
                    dt.Rows.Add(
                        dgvr.Cells["ID"].Value.ToString(),
                        dgvr.Cells["Codigo"].Value.ToString(),
                        dgvr.Cells["Descripcion"].Value.ToString(),
                        dgvr.Cells["Cantidad"].Value.ToString(),
                        dgvr.Cells["U/M"].Value.ToString(),
                        dgvr.Cells["Cuenta"].Value.ToString(),
                        dgvr.Cells["Costo Unitario"].Value.ToString(),
                        dgvr.Cells["Costo Extendido"].Value.ToString(),
                        dgvr.Cells["Cotizado en Dlls"].Value.ToString()
                    );
                }
            }
            return dt;
        }
        public void getdata() {
            try {
                string connectionstring = Program.stringconnection;
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                string sqlquery = "select fsid as 'ID', fsrequisicion as 'Requisicion', "
                    +  "users.fulname as 'Usuario', "
                    + "req.dateapproved as 'Fecha Aprobacion', "
                    + "uuuu.fulname as 'Aprobado por', "
                    + "fscodigo as 'Codigo', "
                    + "fsdesc as 'Descripcion', fscantidad as 'Cantidad', "
                    + "fsunimedida as 'U/M', fscuenta as 'Cuenta', "
                    + "fscostounitario as 'Costo Unitario', fstotalcost as 'Costo Extendido', "
                    + "absolutdllscot as 'Cotizado en Dlls', comentario as 'Comentario', "
                    + "tbsku.fscategoria_reportesku as 'Categoria para reportes' "
                    + "from materialrequerido "
                    + "join requisiciones req on req.id_req = materialrequerido.fsrequisicion "
                    + "join users on users.username = req.createdby "
                    + "join users uuuu on uuuu.username = req.approvedby "
                    + "join sku tbsku on tbsku.sku = fscodigo "
                    + "where materialrequerido.fsstatus = 'Requisicion Aprobada' "
                    + "order by 'Fecha Aprobacion'";
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["ID"].Width = 60;
                dataGridView1.Columns["Requisicion"].Width = 70;
                dataGridView1.Columns["Fecha Aprobacion"].Width = 150;
                dataGridView1.Columns["Aprobado por"].Width = 200;
                dataGridView1.Columns["Descripcion"].Width = 300;
                dataGridView1.Columns["Comentario"].Width = 300;
                dataGridView1.Columns["Categoria para reportes"].Width = 300;
                dataGridView1.Columns["Costo Unitario"].Visible = false;
                dataGridView1.Columns["Costo Extendido"].Visible = false;
                dataGridView1.Columns["Cotizado en Dlls"].Visible = false;
                dataGridView1.Columns["Cuenta"].Visible = false;
                conn.Close();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void Usuarios_Load(object sender, EventArgs e) {
            getdata();
        }
        private void button1_Click(object sender, EventArgs e) {
            Hide();
            Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            getdata();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            FilesRequi fr = new FilesRequi();
            fr.reqid = dataGridView1[1, e.RowIndex].Value.ToString();
            fr.usuario = usuario;
            fr.ShowInTaskbar = false;
            fr.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e) {
            CreateCotizacion cpo = new CreateCotizacion();
            cpo.usuario = usuario;
            cpo.user_id = user_id;
            cpo.tabla = getselecteditems();
            if (cpo.tabla.Rows.Count < 1) {
                MessageBox.Show("No hay nada seleccionado");
            } else {
                if (cpo.tabla.Rows.Count >= 19) {
                    MessageBox.Show("Por favor seleccione menos lineas, max 19");
                } else {
                    cpo.deptoid = deptoid;
                    cpo.user_depto = user_depto;
                    cpo.FormClosed += Cpo_FormClosed;
                    cpo.ShowInTaskbar = false;
                    cpo.ShowDialog();
                }
            }
        }
        private void Cpo_FormClosed(object sender, FormClosedEventArgs e) {
            getdata();
        }

        private void Extra_FormClosed(object sender, FormClosedEventArgs e) {
            
        }
        int rowindex = 0;
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                rowindex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
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
                    FilesRequi fr = new FilesRequi();
                    fr.reqid = dataGridView1[1, rowindex].Value.ToString();
                    fr.usuario = usuario;
                    fr.ShowInTaskbar = false;
                    fr.ShowDialog();
                } catch (Exception) { MessageBox.Show("Se presento un problema, intente de nuevo."); }
            }
            if (btn.Text == "Editar") {
                try {
                    EditLinea el = new EditLinea();
                    el.lineaid = dataGridView1[0, rowindex].Value.ToString();
                    el.usuario = usuario;
                    el.ShowDialog();
                } catch (Exception) { MessageBox.Show("Se presento un problema, intente de nuevo."); }
            }
        }
    }
}