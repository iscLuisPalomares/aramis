using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComprasProject {
    public partial class MttoRecibirTrabajo : Form {
        public MttoRecibirTrabajo() {
            InitializeComponent();
        }
        public string mttolinea;
        public string usuario;
        public string user_id;
        private void MttoRecibirTrabajo_Load(object sender, EventArgs e) {
            button1.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
