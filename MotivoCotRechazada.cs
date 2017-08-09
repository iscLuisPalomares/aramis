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
    public partial class MotivoCotRechazada : Form {
        public MotivoCotRechazada() {
            InitializeComponent();
        }
        public string motivo = "";

        private void button1_Click(object sender, EventArgs e) {
            motivo = textBox1.Text;
            Close();
        }

        private void MotivoCotRechazada_Load(object sender, EventArgs e) {

        }
    }
}
