using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasProject {
    class ComboBoxSKU {
        public string fsid { get; set; }
        public string fssku { get; set; }
        public string fsdesc { get; set; }
        public string fscategory { get; set; }

        public override string ToString() {
            return fsdesc;
        }
    }
}
