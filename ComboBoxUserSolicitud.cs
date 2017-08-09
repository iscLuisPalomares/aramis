using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasProject {
    class ComboBoxUserSolicitud {
        public string fsid { get; set; }
        public string fsusername { get; set; }
        public string fsfullname { get; set; }
        
        public override string ToString() {
            return fsfullname;
        }
    }
}
